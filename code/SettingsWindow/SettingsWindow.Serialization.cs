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
		var settings = JsonSerializer.Deserialize<BridgeSettings>( jsonInput );

		BridgeImporter.Settings = settings;
	}

	private static void Save()
	{
		var jsonOutput = JsonSerializer.Serialize( BridgeImporter.Settings );
		File.WriteAllText( "quixel_settings.json", jsonOutput );
	}
}
