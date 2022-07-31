using Sandbox;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuixelBridge;

public class BridgeImporter
{
	private BridgeServer listener;

	public static BridgeImporter Instance { get; set; }

	public void Run()
	{
		Instance = this;

		listener = new BridgeServer( BridgeSettings.Instance.ServerPort );
		listener.StartServer();
	}

	public void Stop()
	{
		listener.EndServer();
	}

	public async Task ImportFrom( Progress.ProgressBar progressBar, BridgeAsset quixelAsset )
	{
		if ( ExportAsset( quixelAsset, out string path ) )
		{
			var relativePath = Path.GetRelativePath( BridgeSettings.Instance.ProjectPath, path );

			for ( int i = 0; i < quixelAsset.Meshes.Count; i++ )
			{
				Mesh mesh = quixelAsset.Meshes[i];
				var mdlPath = Path.Join( relativePath, $"{quixelAsset.Name.ToSourceName()}_{quixelAsset.Id}.vmdl" ).NormalizeFilename();

				progressBar.SetSubtitle( "Compiling... (2/2)" );
				progressBar.SetValues( 0.66f, 1.0f );

				await Task.Delay( 50 ); // Wait for asset to propagate through asset system
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

	private bool ExportAsset( BridgeAsset quixelAsset, out string path )
	{
		string dirName = new DirectoryInfo( quixelAsset.Path ).Name;
		quixelAsset.DirectoryName = dirName;

		//
		// Set location path
		//
		{
			path = $"{BridgeSettings.Instance.ProjectPath}/megascans/";
			foreach ( var cat in quixelAsset.Categories )
			{
				if ( cat == "3d" || cat == "2d" || cat == "surface" )
					continue;

				path += $"{cat.ToSourceName()}/";
			}
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

	private bool CopyFiles( ref BridgeAsset quixelAsset, string path )
	{
		// Create destination directories
		Directory.CreateDirectory( path );

		var assetPath = $"{path}assets";
		Directory.CreateDirectory( assetPath );

		void CopyAssets<T>( List<T> items, BridgeAsset asset, string subDir ) where T : IBaseAsset
		{
			var joinedSubDir = $"{assetPath}/{subDir}";
			Directory.CreateDirectory( joinedSubDir );

			for ( int i = 0; i < items.Count; i++ )
			{
				T item = items[i];
				string destination = item.Path.Replace( asset.Path, joinedSubDir );
				destination = destination.Replace( item.Name, item.Name.ToSourceName() );

				Directory.CreateDirectory( Path.GetDirectoryName( destination ) );
				File.Copy( item.Path, destination, true );

				item.Path = destination;
				item.Name = item.Name.ToSourceName();

				items[i] = item;
			}
		}

		// Copy meshes & lods
		if ( quixelAsset.Meshes.Count > 0 )
		{
			CopyAssets( quixelAsset.Meshes, quixelAsset, "meshes" );
			CopyAssets( quixelAsset.LODs, quixelAsset, "meshes" );
		}

		// Copy textures
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

	private static bool CreateMaterial( BridgeAsset quixelAsset )
	{
		var vmatPath = $"{quixelAsset.Path}materials/";
		Directory.CreateDirectory( vmatPath );
		vmatPath += $"{quixelAsset.Name.ToSourceName()}_{quixelAsset.Id}.vmat";

		string template = quixelAsset.Type switch
		{
			"atlas" => "templates/Materials/Decal.template",
			_ => "templates/Materials/Complex.template"
		};

		Template baseVmat = new( template );

		var pairs = new Dictionary<string, string>
		{
			//
			// Defaults (colors)
			//
			{ "Diffuse", "[1.000000 1.000000 1.000000 0.000000]" },
			{ "Normal", "[1.000000 1.000000 1.000000 0.000000]" },
			{ "Roughness", "[1.000000 1.000000 1.000000 0.000000]" },
			{ "AmbientOcclusion", "[1.000000 1.000000 1.000000 0.000000]" },
			{ "Translucency", "[1.000000 1.000000 1.000000 0.000000]" },

			// Metallic should be 0 by default
			{ "Metallic", "[0.000000 0.000000 0.000000 0.000000]" },

			// Displasment (for the parallax of decals)
			{ "Displacement", "[1.000000 1.000000 1.000000 0.000000]" }
		};

		// Get all used textures
		quixelAsset.Textures.ForEach( texture =>
		{
			var path = texture.Path.PathRelativeTo( BridgeSettings.Instance.ProjectPath ).NormalizeFilename();
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
					pairs["Translucency"] = path;
					break;
				case "displacement":
					pairs["Displacement"] = path;
					break;
				case "translucency":
					pairs["Translucency"] = path;
					break;
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

	private static bool CreateModel( BridgeAsset quixelAsset, int meshIndex )
	{
		// TODO: What the fuck
		var vmatPath = $"{quixelAsset.Path.PathRelativeTo( BridgeSettings.Instance.ProjectPath )}materials/{quixelAsset.Name.ToSourceName()}_{quixelAsset.Id}.vmat";
		var vmdlPath = $"{quixelAsset.Path}{quixelAsset.Name.ToSourceName()}_{quixelAsset.Id}.vmdl";

		var meshes = "";
		var lods = "";

		var meshName = quixelAsset.Meshes[meshIndex].Name;

		// Remove LOD[0-9].fbx from file name
		var regex = new Regex( $"lod[0-9].fbx$" );
		meshName = regex.Replace( meshName, "" );

		for ( int i = 0; i < quixelAsset.LODs.Count; i++ )
		{
			// Don't include LODs for other meshes
			if ( !quixelAsset.LODs[i].Name.Contains( meshName ) )
				continue;

			// LODs
			{
				var baseLod = new Template( "templates/Lod.template" );
				lods += baseLod.Parse( new()
				{
					{ "Threshold", (i * BridgeSettings.Instance.LodIncrement).ToString() },
					{ "Mesh", $"unnamed_{i + 1}" }
				} );
			}

			// Mesh
			{
				var baseMesh = new Template( "templates/Mesh.template" );
				meshes += baseMesh.Parse( new()
				{
					{ "Mesh", quixelAsset.LODs[i].Path.PathRelativeTo( BridgeSettings.Instance.ProjectPath ).NormalizeFilename() }
				} );
			}
		}

		// Write vmdl
		var vmdlBase = new Template( "templates/Model.template" );

		File.WriteAllText( vmdlPath, vmdlBase.Parse( new()
		{
			{ "Material", vmatPath },
			{ "Lods", lods },
			{ "Meshes", meshes },
			{ "Entity", BridgeSettings.Instance.Entity }
		} ) );

		return true;
	}
}
