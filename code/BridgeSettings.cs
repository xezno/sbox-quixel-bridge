using System.Text.Json.Serialization;

namespace QuixelBridge;

public class BridgeSettings
{
	[JsonPropertyName( "ProjectPath" )]
	public string ProjectPath { get; set; } = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\sbox\\addons\\megascans";

	[JsonPropertyName( "ServerPort" )]
	public int ServerPort { get; set; } = 24981;

	[JsonPropertyName( "LodIncrement" )]
	public float LodIncrement { get; set; } = 25.0f;

	[JsonPropertyName( "Entity" )]
	public string Entity { get; set; } = "prop_static";
}
