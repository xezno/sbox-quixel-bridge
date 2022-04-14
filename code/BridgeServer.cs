using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BridgeForSbox;

class BridgeServer
{
	private TcpListener tcpListener;
	private Thread tcpListenerThread;
	private static bool QueueDirty { get; set; } = false;
	private static List<string> ImportQueue { get; set; } = new List<string>();
	private readonly int port;

	public BridgeServer( int port )
	{
		this.port = port;
	}

	public void StartServer()
	{
		tcpListenerThread = new( new ThreadStart( ListenThread ) )
		{
			IsBackground = true
		};

		tcpListenerThread.Start();
	}

	public void EndServer() => tcpListener.Stop();

	//
	// Shit way of running things on the main thread
	// (we should probably just re-write the tcp listener
	// code to work better in a single-thread context)
	//
	[Sandbox.Event.Frame]
	public static void OnFrame()
	{
		if ( !QueueDirty )
			return;

		Log.Trace( "Import queue dirty!" );
		var importQueueCopy = ImportQueue.ToArray();
		QueueDirty = false;
		ImportQueue.Clear();

		var asyncTask = async () =>
		{
			using var progress = Progress.Start( "Importing from Quixel Bridge..." );
			var progressList = new List<Progress.ProgressBar>();

			var queueItem = importQueueCopy[0];
			var quixelAssets = JsonSerializer.Deserialize<QuixelAsset[]>( queueItem );

			//
			// Add to progress window
			//
			for ( int i = 0; i < quixelAssets.Length; i++ )
			{
				var quixelAsset = quixelAssets[i];
				var progressBar = Progress.Bar( $"Import '{quixelAsset.Name}'" );
				progressList.Add( progressBar );
				progressBar.SetSubtitle( "Waiting..." );
				progressBar.SetValues( 0.0f, 1.0f );
			}

			//
			// Start compiling
			//
			for ( int i = 0; i < quixelAssets.Length; i++ )
			{
				var quixelAsset = quixelAssets[i];
				var progressBar = progressList[i];
				progressBar.SetSubtitle( "Importing... (1/2)" );
				progressBar.SetValues( 0.33f, 1.0f );
				await Task.Delay( 100 );

				BridgeImporter.Instance.ImportFrom( progressBar, quixelAsset );
			}

			progressList.ForEach( x => x.Dispose() );
		};

		_ = asyncTask();
	}

	private void ListenThread()
	{
		try
		{
			tcpListener = new TcpListener( IPAddress.Parse( "127.0.0.1" ), port );
			tcpListener.Start();

			byte[] bytes = new byte[1024];
			while ( true )
			{
				using var connectedTcpClient = tcpListener.AcceptTcpClient();
				using NetworkStream stream = connectedTcpClient.GetStream();

				int length;
				string clientMessage = "";

				while ( (length = stream.Read( bytes, 0, bytes.Length )) != 0 )
				{
					byte[] incomingData = new byte[length];
					Array.Copy( bytes, 0, incomingData, 0, length );
					clientMessage += Encoding.ASCII.GetString( incomingData );
				}

				ImportQueue.Add( clientMessage );
				QueueDirty = true;
			}
		}
		catch ( Exception ex ) // s&box does not like it when crashing on a non-main thread!!! handle it ourselves here
		{
			var split = ex.ToString().Split( '\n' );
			for ( int i = 0; i < split.Length; i++ )
			{
				string s = split[i];
				if ( i != 0 )
					s = "\t" + s;

				Log.Error( s );
			}
		}
	}
}
