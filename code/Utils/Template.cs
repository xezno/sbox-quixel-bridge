using System.Collections.Generic;
using System.IO;

namespace BridgeForSbox;

/// <summary>
/// Shitty simple template format parser
/// </summary>
public class Template
{
	// HACK
	private const string BasePath = "addons/quixel.bridge/code/";
	private string TemplateContents { get; set; }

	public Template( string templatePath )
	{
		var dirName = Path.GetDirectoryName( templatePath );
		var fileName = Path.GetFileName( templatePath );
		templatePath = Path.Join( BasePath, dirName, fileName );
		TemplateContents = File.ReadAllText( templatePath );
	}

	public string Parse( Dictionary<string, string> values )
	{
		//
		// example template:
		// "Blah<#= value #>blah"
		//
		// called with Parse( new[]{ ( "value", "Hello!" ) } ):
		// "BlahHello!blah"
		//

		var str = TemplateContents;
		foreach ( var pair in values )
		{
			str = str.Replace( $"<#= {pair.Key} #>", pair.Value );
		}

		return str;
	}
}
