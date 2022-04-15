using Tools;

namespace QuixelBridge;

[Tool( "Bridge Settings", "settings", "Settings for the Quixel Bridge plugin" )]
public class TestWindow : Window
{
	[Menu( "Editor", "Quixel/Settings", "settings" )]
	public static void OpenWindow()
	{
		Log.Info( "Open Window!" );
		new TestWindow();
	}

	public TestWindow()
	{
		Title = "Quixel Bridge Settings";
		Size = new Vector2( 500, 200 );
		MaximumSize = Size;
		MinimumSize = Size;
		ResizeButtonsVisible = false;

		CreateUI();
		Show();
	}

	public void CreateUI()
	{
		Clear();

		StatusBar.Visible = false;

		var w = new Widget( null );
		w.SetLayout( LayoutMode.TopToBottom );
		w.SetSizeMode( SizeMode.Default, SizeMode.CanShrink );

		LineEdit AddLineEdit( string label, Widget parent, string value = "" )
		{
			var widget = new PropertyRow( parent );

			widget.SetLabel( label );

			var path = new LineEdit( value, parent );
			path.SetStyles( "padding: 6px; background-color: #38393c; border-radius: 2px;" );
			widget.Layout.Add( path, 1 );

			parent.Layout.Add( widget );

			return path;
		}

		Button AddPathPicker( string label, Widget parent, string value = "" )
		{
			var widget = new PropertyRow( parent );

			widget.SetLabel( label );

			var path = new LineEdit( value, parent );
			path.SetStyles( "padding: 6px; background-color: #38393c; border-radius: 2px; margin-right: 4px;" );
			widget.Layout.Add( path, 1 );

			var button = new Button( "Select folder", "folder_open", widget );
			button.Clicked += () =>
			{
				var fd = new FileDialog( null );
				fd.Title = "Find directory";
				fd.SetFindDirectory();
				parent.Layout.Add( fd );

				if ( fd.Execute() )
				{
					Log.Trace( fd.SelectedFile );
				}
			};

			widget.Layout.Add( button );
			parent.Layout.Add( widget );

			return button;
		}

		var paths = new Widget( w );
		AddPathPicker( "Addon Path", w, BridgeImporter.ProjectPath );
		AddPathPicker( "Output directory", w, BridgeImporter.ExportDirectory );
		w.Layout.Add( paths );

		var numbers = new Widget( w );
		AddLineEdit( "Server port", w, BridgeImporter.ServerPort.ToString() );
		AddLineEdit( "Scale", w, BridgeImporter.Scale.ToString() );
		AddLineEdit( "LOD increment", w, BridgeImporter.LodIncrement.ToString() );
		w.Layout.Add( numbers );

		Canvas = w;
	}

	[Sandbox.Event.Hotload]
	public void OnHotload()
	{
		CreateUI();
	}
}
