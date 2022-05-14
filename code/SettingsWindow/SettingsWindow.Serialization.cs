using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace QuixelBridge;

partial class SettingsWindow
{
	private static void Load()
	{
		if ( !File.Exists( "quixel_settings.json" ) )
			return;

		var jsonInput = File.ReadAllText( "quixel_settings.json" );
		var dict = JsonSerializer.Deserialize<Dictionary<string, string>>( jsonInput );

		string entity;
		dict.TryGetValue( "Entity", out entity );
		BridgeImporter.Entity = entity;

		string projectPath;
		dict.TryGetValue( "ProjectPath", out projectPath );
		BridgeImporter.ProjectPath = projectPath;

		string serverPort;
		dict.TryGetValue( "ServerPort", out serverPort );
		BridgeImporter.ServerPort = int.Parse( serverPort );

		string lodIncrement;
		dict.TryGetValue( "LodIncrement", out lodIncrement );
		BridgeImporter.LodIncrement = float.Parse( lodIncrement );
	}

	private static void Save()
	{
		var dict = new Dictionary<string, string>();

		dict["Entity"] = BridgeImporter.Entity;
		dict["ProjectPath"] = BridgeImporter.ProjectPath;
		dict["ServerPort"] = BridgeImporter.ServerPort.ToString();
		dict["LodIncrement"] = BridgeImporter.LodIncrement.ToString();

		var jsonOutput = JsonSerializer.Serialize( dict );
		File.WriteAllText( "quixel_settings.json", jsonOutput );
	}
}
