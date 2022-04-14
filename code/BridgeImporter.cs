using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeForSbox;

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

	public async void ImportFrom( string jsonData )
	{
		var quixelAssets = JsonSerializer.Deserialize<QuixelAsset[]>( jsonData );

		var asyncTask = async () =>
		{
			using var progress = Progress.Start( "Importing assets..." );
			foreach ( QuixelAsset quixelAsset in quixelAssets )
			{
				using var p = Progress.Bar( $"Importing {quixelAsset.Name}" );
				p.SetValues( 0.0f, 1.0f );
				p.SetSubtitle( "Importing from Quixel... (1/2)" );
				await Task.Delay( 100 );

				if ( ExportAsset( quixelAsset, out string location ) )
				{
					var relativePath = Path.GetRelativePath( ProjectPath, location );

					p.SetValues( 0.5f, 1.0f );
					p.SetSubtitle( "Compiling asset... (2/2)" );
					await Task.Delay( 200 );

					for ( int i = 0; i < quixelAsset.Meshes.Count; i++ )
					{
						Mesh mesh = quixelAsset.Meshes[i];
						var mdlPath = Path.Join( relativePath, $"{quixelAsset.Id}_{i}.vmdl" )
							.Replace( "\\", "/" ); // s&box uses / as separator

						var asset = Tools.AssetSystem.All.First( x => x.Path == mdlPath );
						asset.Compile( true ); // Force a full compile

						Log.Trace( $"Exported to: {asset}" );
						Log.Trace( $"TODO: Highlight in asset browser" );
					}
				}
				else
				{
					Log.Error( "Failed to export.\n" );
				}
			}
		};

		_ = asyncTask();
	}

	private bool ExportAsset( QuixelAsset quixelAsset, out string location )
	{
		string dirName = new DirectoryInfo( quixelAsset.Path ).Name;
		quixelAsset.DirectoryName = dirName;

		location = $@"{ProjectPath}/{ExportDirectory}/{quixelAsset.Type}/{quixelAsset.DirectoryName.ToLower()}";

		if ( !CopyFiles( ref quixelAsset, location ) )
		{
			Log.Error( $"Failed to copy files!" );
			return false;
		}

		if ( !CreateMaterial( quixelAsset ) )
		{
			Log.Error( $"Failed to create material!" );
			return false;
		}

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

	private bool CopyFiles( ref QuixelAsset quixelAsset, string location )
	{
		// Create destination directories
		Directory.CreateDirectory( location );
		Directory.CreateDirectory( $"{location}/assets" );

		void CopyAssets<T>( List<T> items, QuixelAsset asset ) where T : IBaseAsset
		{
			for ( int i = 0; i < items.Count; i++ )
			{
				T item = items[i];
				string destination = item.Path.Replace( asset.Path, $"{location}/assets" );

				Directory.CreateDirectory( Path.GetDirectoryName( destination ) );
				File.Copy( item.Path, destination, true );

				item.Path = destination;
				item.Name = item.Name.ToLower();

				items[i] = item;
			}
		}

		CopyAssets( quixelAsset.Meshes, quixelAsset );
		CopyAssets( quixelAsset.LODs, quixelAsset );
		CopyAssets( quixelAsset.Textures, quixelAsset );

		//
		// Normalize paths and IDs
		//
		quixelAsset.Id = quixelAsset.Id.ToLower();
		quixelAsset.Name = quixelAsset.Name.ToLower();
		quixelAsset.DirectoryName = quixelAsset.DirectoryName.ToLower();
		quixelAsset.Path = location;

		return true;
	}

	private static bool CreateMaterial( QuixelAsset quixelAsset )
	{
		var vmatLocation = $@"{quixelAsset.Path}/materials/";
		Directory.CreateDirectory( vmatLocation );
		vmatLocation += $"/{quixelAsset.Id}.vmat";

		var baseVmat = new Template( "templates/Material.template" );
		var pairs = new Dictionary<string, string>();

		//
		// Defaults (colors)
		//
		pairs.Add( "Diffuse", "[1.000000 1.000000 1.000000 0.000000]" );
		pairs.Add( "Normal", "[1.000000 1.000000 1.000000 0.000000]" );
		pairs.Add( "Roughness", "[1.000000 1.000000 1.000000 0.000000]" );
		pairs.Add( "AmbientOcclusion", "[1.000000 1.000000 1.000000 0.000000]" );

		pairs.Add( "Metallic", "[0.000000 0.000000 0.000000 0.000000]" );

		// Get all used textures
		quixelAsset.Textures.ForEach( texture =>
		{
			var path = texture.Path.Replace( ProjectPath + "/", "" ).Replace( '\\', '/' );
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
					Console.WriteLine( $"Unsupported texture type '{texture.Type}', deleting" );
					File.Delete( texture.Path );
					break;
			}
		} );

		// Write
		File.WriteAllText( vmatLocation, baseVmat.Parse( pairs ) );

		return true;
	}

	private static bool CreateModel( QuixelAsset quixelAsset, int meshIndex )
	{
		var vmatLocation = $"{quixelAsset.Path.Replace( ProjectPath + "/", "" ).Replace( '\\', '/' )}/materials/{quixelAsset.Id}.vmat";
		string vmdlLocation = $@"{quixelAsset.Path}/{quixelAsset.Id}_{meshIndex}.vmdl";

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
				{ "Mesh", quixelAsset.LODs[i].Path.Replace( ProjectPath + "/", "" ).Replace( '\\', '/' ) }
			} );

			lodCount++;
		}

		// Write vmdl
		var vmdlBase = new Template( "templates/Model.template" );

		File.WriteAllText( vmdlLocation, vmdlBase.Parse( new()
		{
			{ "Material", vmatLocation },
			{ "Lods", lods },
			{ "Meshes", meshes }
		} ) );

		return true;
	}
}
