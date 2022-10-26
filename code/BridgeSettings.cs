using System.IO;
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

	[JsonIgnore]
	private static BridgeSettings instance;

	[JsonIgnore]
	public static BridgeSettings Instance
	{
		get
		{
			if ( instance == null )
				instance = LoadFromDisk();

			return instance;
		}
	}

	/// <summary>
	/// Loads settings from disk, or creates a default settings file if
	/// a settings file does not already exist.
	/// </summary>
	private static BridgeSettings LoadFromDisk()
	{
		if ( !File.Exists( SettingsFile ) )
			return CreateDefaultSettings();

		var jsonInput = File.ReadAllText( SettingsFile );
		var settings = JsonSerializer.Deserialize<BridgeSettings>( jsonInput );

		settings.ProjectPath ??= Utility.Projects.GetAll().Where( x => x.Active ).FirstOrDefault().GetRootPath();

		return settings;
	}

	/// <summary>
	/// Saves the current settings to disk.
	/// </summary>
	public void SaveToDisk()
	{
		var jsonOutput = JsonSerializer.Serialize( this );
		File.WriteAllText( SettingsFile, jsonOutput );
	}

	/// <summary>
	/// Create and save the default settings file.
	/// </summary>
	private static BridgeSettings CreateDefaultSettings()
	{
		var settings = new BridgeSettings();
		settings.SaveToDisk();

		return settings;
	}
}
