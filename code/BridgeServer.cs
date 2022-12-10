using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace QuixelBridge;

class BridgeServer
{
	private TcpListener tcpListener;
	private Thread tcpListenerThread;
	private static bool queueDirty;

	private static List<string> importQueue = new();
	private bool isRunning = true;

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

	public void EndServer()
	{
		isRunning = false;
		tcpListener.Stop();
	}

	//
	// Shit way of running things on the main thread
	// (we should probably just re-write the tcp listener
	// code to work better in a single-thread context)
	//
	[Event.Client.Frame]
	public static void OnFrame()
	{
		if ( !queueDirty )
			return;

		Log.Trace( "Import queue dirty!" );
		var importQueueCopy = importQueue.ToArray();
		queueDirty = false;
		importQueue.Clear();

		var asyncTask = async () =>
		{
			using var progress = Progress.Start( "Importing from Quixel Bridge..." );
			var progressList = new List<Progress.ProgressBar>();

			var queueItem = importQueueCopy[0];
			var quixelAssets = JsonSerializer.Deserialize<BridgeAsset[]>( queueItem );

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
				await Task.Delay( 50 );

				await BridgeImporter.Instance.ImportFrom( progressBar, quixelAsset );
			}

			//
			// Play a "success" chime if the queue has more than 1 item in it and the
			// user has the chime enabled
			//
			if ( quixelAssets.Length > 1 && BridgeSettings.Instance.EnableAudio )
			{
				var handle = Audio.Play( "quixel_import_success" );
				handle.Position = Vector3.Zero;
				handle.ListenLocal = true;
			}

			await Task.Delay( 1000 ); // Give the user an extra second to see that we completed

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
				try
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

					importQueue.Add( clientMessage );
					queueDirty = true;
				}
				catch ( Exception )
				{
					if ( isRunning )
						throw;
				}
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
