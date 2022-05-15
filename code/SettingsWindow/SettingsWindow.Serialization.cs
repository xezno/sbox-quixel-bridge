using System.IO;
using System.Text.Json;

namespace QuixelBridge;

partial class SettingsWindow
{
	private const string SettingsFile = "quixel_bridge_settings.json";

	private static void LoadSettings()
	{
		if ( !File.Exists( SettingsFile ) )
			return;

		var jsonInput = File.ReadAllText( SettingsFile );
		var settings = JsonSerializer.Deserialize<BridgeSettings>( jsonInput );

		BridgeImporter.Settings = settings;
	}

	private static void SaveSettings()
	{
		var jsonOutput = JsonSerializer.Serialize( BridgeImporter.Settings );
		File.WriteAllText( SettingsFile, jsonOutput );
	}
}
