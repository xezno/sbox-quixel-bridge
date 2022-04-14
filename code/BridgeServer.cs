using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BridgeForSbox;

class BridgeServer
{
	private TcpListener tcpListener;
	private Thread tcpListenerThread;
	private int port;

	private static List<string> ImportQueue { get; set; } = new List<string>();

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
		tcpListener.Stop();
	}

	//
	// Shit way of running things on the main thread
	// (we should probably just re-write the tcp listener
	// code to work better in a single-thread context)
	//
	[Sandbox.Event.Frame]
	public static void OnFrame()
	{
		if ( ImportQueue.Count == 0 )
			return;

		foreach ( var queueItem in ImportQueue )
			BridgeImporter.Instance.ImportFrom( queueItem );

		ImportQueue.Clear();
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
