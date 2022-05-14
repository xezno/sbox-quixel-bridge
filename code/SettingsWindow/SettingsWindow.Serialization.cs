using Sandbox;
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

		BridgeImporter.ProjectPath = dict["ProjectPath"];
		BridgeImporter.ServerPort = int.Parse( dict["ServerPort"] );
		BridgeImporter.Scale = float.Parse( dict["Scale"] );
		BridgeImporter.LodIncrement = float.Parse( dict["LodIncrement"] );
	}

	private static void Save()
	{
		var dict = new Dictionary<string, string>();

		dict["ProjectPath"] = BridgeImporter.ProjectPath;
		dict["ServerPort"] = BridgeImporter.ServerPort.ToString();
		dict["Scale"] = BridgeImporter.Scale.ToString();
		dict["LodIncrement"] = BridgeImporter.LodIncrement.ToString();

		var jsonOutput = JsonSerializer.Serialize( dict );
		File.WriteAllText( "quixel_settings.json", jsonOutput );
	}
}
