using System.Linq;
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

	public BridgeSettings()
	{
		ProjectPath = Utility.Addons.GetAll().Where( x => x.Active ).FirstOrDefault().GetRootPath();
	}
}
