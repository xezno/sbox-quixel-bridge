﻿using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tools;

namespace QuixelBridge;

public class BridgeSettings
{
	[JsonPropertyName( "ProjectPath" )]
	public string ProjectPath { get; set; }

	[JsonPropertyName( "ServerPort" )]
	public int ServerPort { get; set; } = 24981;

	[JsonPropertyName( "LodIncrement" )]
	public float LodIncrement { get; set; } = 25.0f;

	[JsonPropertyName( "Entity" )]
	public string Entity { get; set; } = "prop_static";

	[JsonPropertyName( "EnableAudio" )]
	public bool EnableAudio { get; set; } = false;

	[JsonIgnore]
	private const string SettingsFile = "quixel_bridge_settings.json";

	public static void LoadSettings()
	{
		if ( !File.Exists( SettingsFile ) )
			return;

		var jsonInput = File.ReadAllText( SettingsFile );
		var settings = JsonSerializer.Deserialize<BridgeSettings>( jsonInput );

		BridgeImporter.Settings = settings;
	}

	public static void SaveSettings()
	{
		var jsonOutput = JsonSerializer.Serialize( BridgeImporter.Settings );
		File.WriteAllText( SettingsFile, jsonOutput );
	}

	public BridgeSettings()
	{
		ProjectPath = Utility.Addons.GetAll().Where( x => x.Active ).FirstOrDefault().GetRootPath();
	}
}
