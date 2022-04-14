using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace QuixelBridge;

public class BridgeImporter
{
	// TODO: Expose these
	#region Properties
	public static string ProjectPath { get; set; } = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\sbox\\addons\\megascans";
	public static string ExportDirectory { get; set; } = "megascans";
	public static int ServerPort { get; set; } = 24981;
	public static float Scale { get; set; } = 0.3937f;
	#endregion

	private BridgeServer listener;

	public static BridgeImporter Instance { get; set; }

	public void Run()
	{
		Instance = this;

		ProjectPath = ProjectPath.Replace( '\\', '/' );

		// Starts the server in background.
		listener = new BridgeServer( ServerPort );
		listener.StartServer();
	}

	public void Stop()
	{
		listener.EndServer();
	}

	public void ImportFrom( Progress.ProgressBar progressBar, QuixelAsset quixelAsset )
	{
		if ( ExportAsset( quixelAsset, out string path ) )
		{
			var relativePath = Path.GetRelativePath( ProjectPath, path );

			for ( int i = 0; i < quixelAsset.Meshes.Count; i++ )
			{
				Mesh mesh = quixelAsset.Meshes[i];
				var mdlPath = Path.Join( relativePath, $"{quixelAsset.Name.ToPathString()}_{quixelAsset.Id}.vmdl" ).NormalizePath();

				progressBar.SetSubtitle( "Compiling... (2/2)" );
				progressBar.SetValues( 0.66f, 1.0f );

				var asset = Tools.AssetSystem.All.FirstOrDefault( x => x.Path == mdlPath );
				if ( asset == null )
				{
					Log.Warning( $"Couldn't find the asset that just got exported? Did it fail?" );
				}
				else
				{
					asset.Compile( true ); // Force a full compile
					Log.Trace( $"Exported to: {asset}" );
					Log.Trace( $"TODO: Highlight in asset browser" );
				}

				progressBar.SetSubtitle( "Done." );
				progressBar.SetValues( 1.0f, 1.0f );
			}
		}
		else
		{
			Log.Error( "Failed to export.\n" );
		}
	}

	private bool ExportAsset( QuixelAsset quixelAsset, out string path )
	{
		string dirName = new DirectoryInfo( quixelAsset.Path ).Name;
		quixelAsset.DirectoryName = dirName;

		//
		// Set location path
		//
		{
			path = $"{ProjectPath}/{ExportDirectory}/";
			foreach ( var cat in quixelAsset.Categories )
			{
				if ( cat == "3d" || cat == "2d" || cat == "surface" )
					continue;

				path += $"{cat}/";
			}

			path = path.NormalizePath();
		}

		//
		// Copy files
		//
		if ( !CopyFiles( ref quixelAsset, path ) )
		{
			Log.Error( $"Failed to copy files!" );
			return false;
		}

		//
		// Create material
		//
		if ( !CreateMaterial( quixelAsset ) )
		{
			Log.Error( $"Failed to create material!" );
			return false;
		}

		//
		// Create meshes (multiple for some things e.g. grass)
		//
		for ( int i = 0; i < quixelAsset.Meshes.Count; i++ )
		{
			if ( !CreateModel( quixelAsset, i ) )
			{
				Log.Error( $"Failed to create model!" );
				return false;
			}
		}

		return true;
	}

	private bool CopyFiles( ref QuixelAsset quixelAsset, string path )
	{
		// Create destination directories
		Directory.CreateDirectory( path );

		var assetPath = $"{path}/assets";
		Directory.CreateDirectory( assetPath );

		void CopyAssets<T>( List<T> items, QuixelAsset asset, string subDir ) where T : IBaseAsset
		{
			var joinedSubDir = $"{assetPath}/{subDir}";
			Directory.CreateDirectory( joinedSubDir );

			for ( int i = 0; i < items.Count; i++ )
			{
				T item = items[i];
				string destination = item.Path.Replace( asset.Path, joinedSubDir );

				Directory.CreateDirectory( Path.GetDirectoryName( destination ) );
				File.Copy( item.Path, destination, true );

				item.Path = destination;
				item.Name = item.Name.ToLower();

				items[i] = item;
			}
		}

		CopyAssets( quixelAsset.Meshes, quixelAsset, "meshes" );
		CopyAssets( quixelAsset.LODs, quixelAsset, "meshes" );
		CopyAssets( quixelAsset.Textures, quixelAsset, "textures" );

		//
		// Normalize paths and IDs
		//
		quixelAsset.Id = quixelAsset.Id.ToLower();
		quixelAsset.Name = quixelAsset.Name.ToLower();
		quixelAsset.DirectoryName = quixelAsset.DirectoryName.ToLower();
		quixelAsset.Path = path;

		return true;
	}

	private static bool CreateMaterial( QuixelAsset quixelAsset )
	{
		var vmatPath = $"{quixelAsset.Path}/materials/";
		Directory.CreateDirectory( vmatPath );
		vmatPath += $"/{quixelAsset.Name.ToPathString()}_{quixelAsset.Id}.vmat";

		var baseVmat = new Template( "templates/Material.template" );
		var pairs = new Dictionary<string, string>
		{
			//
			// Defaults (colors)
			//
			{ "Diffuse", "[1.000000 1.000000 1.000000 0.000000]" },
			{ "Normal", "[1.000000 1.000000 1.000000 0.000000]" },
			{ "Roughness", "[1.000000 1.000000 1.000000 0.000000]" },
			{ "AmbientOcclusion", "[1.000000 1.000000 1.000000 0.000000]" },

			// Metallic should be 0 by default
			{ "Metallic", "[0.000000 0.000000 0.000000 0.000000]" }
		};

		// Get all used textures
		quixelAsset.Textures.ForEach( texture =>
		{
			var path = texture.Path.PathRelativeTo( ProjectPath ).NormalizePath();
			switch ( texture.Type )
			{
				case "albedo":
					pairs["Diffuse"] = path;
					break;
				case "normal":
					pairs["Normal"] = path;
					break;
				case "roughness":
					pairs["Roughness"] = path;
					break;
				case "ao":
					pairs["AmbientOcclusion"] = path;
					break;
				case "metalness":
					pairs["Metallic"] = path;
					break;
				case "opacity":
				case "displacement":
				case "transmission":
					// TODO
					break;
				default:
					Log.Error( $"Unsupported texture type \"{texture.Type}\". Deleting." );
					File.Delete( texture.Path );
					break;
			}
		} );

		// Write
		File.WriteAllText( vmatPath, baseVmat.Parse( pairs ) );

		return true;
	}

	private static bool CreateModel( QuixelAsset quixelAsset, int meshIndex )
	{
		var vmatPath = $"{quixelAsset.Path.PathRelativeTo( ProjectPath )}/materials/{quixelAsset.Name.ToPathString()}_{quixelAsset.Id}.vmat";
		var vmdlPath = $"{quixelAsset.Path}/{quixelAsset.Name.ToPathString()}_{quixelAsset.Id}.vmdl";

		var lods = "";
		var meshes = "";
		var lodCount = 0;

		var meshName = quixelAsset.Meshes[meshIndex].Name;

		// Remove LOD[0-9].fbx from file name
		var regex = new Regex( $"lod[0-9].fbx$" );
		meshName = regex.Replace( meshName, "" );

		for ( int i = 0; i < quixelAsset.LODs.Count; i++ )
		{
			// Don't include LODs for other meshes
			if ( !quixelAsset.LODs[i].Name.Contains( meshName ) )
				continue;

			var baseLod = new Template( "templates/Lod.template" );
			lods += baseLod.Parse( new()
			{
				{ "Threshold", (lodCount * 25).ToString() },
				{ "Mesh", $"unnamed_{lodCount + 1}" }
			} );

			var baseMesh = new Template( "templates/Mesh.template" );
			meshes += baseMesh.Parse( new()
			{
				{ "Scale", Scale.ToString() },
				{ "Mesh", quixelAsset.LODs[i].Path.PathRelativeTo( ProjectPath ).NormalizePath() }
			} );

			lodCount++;
		}

		// Write vmdl
		var vmdlBase = new Template( "templates/Model.template" );

		File.WriteAllText( vmdlPath, vmdlBase.Parse( new()
		{
			{ "Material", vmatPath },
			{ "Lods", lods },
			{ "Meshes", meshes }
		} ) );

		return true;
	}
}
