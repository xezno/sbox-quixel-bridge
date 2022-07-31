using System.Diagnostics;
using Tools;

namespace QuixelBridge;

public class Bridge
{
	public static BridgeImporter Importer;

	[Menu( "Editor", "Quixel/Start Bridge Plugin", "play_arrow" )]
	public static void StartBridge()
	{
		StopBridge();

		Importer = new BridgeImporter();
		Importer.Run();

		if ( Process.GetProcessesByName( "Bridge" ).Length == 0 )
		{
			Log.Info( "Starting Bridge app" );
			var process = new Process();

			// Default location, we should probably let users change this
			process.StartInfo.FileName = @"C:\Program Files\Bridge\Bridge.exe";
			process.Start();
		}

		Log.Info( "Started Quixel Bridge plugin" );
	}

	[Menu( "Editor", "Quixel/Stop Bridge Plugin", "stop" )]
	public static void StopBridge()
	{
		if ( Importer == null )
			return;

		Importer?.Stop();
		Log.Info( "Stopped Quixel Bridge plugin" );
	}
}
