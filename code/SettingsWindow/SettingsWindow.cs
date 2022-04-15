using Tools;
/*
 * TODO:
 * - BUG: Figure out whether I was fucking up somewhere or whether SetStylesheetFile actually doesn't work
 * - Save settings to a file(!)
 * - Find a nice way to open a dialog and tell the user that their settings have been saved
 * - Clean everything up a little
 */
namespace QuixelBridge;

[Tool( "Bridge Settings", "settings", "Settings for the Quixel Bridge plugin" )]
public class TestWindow : Window
{
	[Menu( "Editor", "Quixel/Settings", "settings" )]
	public static void OpenWindow()
	{
		_ = new TestWindow();
	}

	public TestWindow()
	{
		Title = "Quixel Bridge Settings";
		Size = new Vector2( 500, 225 );
		MaximumSize = Size;
		MinimumSize = Size;
		ResizeButtonsVisible = false;

		CreateUI();
		Show();
	}

	private LineEdit AddNumberEdit( string label, Widget parent, bool middle, string value = "" )
	{
		var widget = new Widget( parent );
		widget.SetLayout( LayoutMode.TopToBottom );

		var l = new Label( widget );
		l.Text = label;
		widget.Layout.Add( l );

		var lineEdit = new LineEdit( value, parent );
		lineEdit.SetStyles( "padding: 6px; background-color: #38393c; border-radius: 2px; " );
		widget.Layout.Add( lineEdit, 1 );

		if ( middle )
			widget.SetStyles( "margin-left: 4px; margin-right: 4px;" );

		parent.Layout.Add( widget );

		return lineEdit;
	}

	private LineEdit AddDirectoryPicker( string label, Widget parent, string value = "" )
	{
		var widget = new PropertyRow( parent );

		widget.SetLabel( label );

		var lineEdit = new LineEdit( value, parent );
		lineEdit.SetStyles( "padding: 6px; background-color: #38393c; border-radius: 2px; margin-right: 4px;" );
		widget.Layout.Add( lineEdit, 1 );

		var button = new Button( "Select folder", "folder_open", widget );
		button.Clicked += () =>
		{
			var fd = new FileDialog( null );
			fd.Title = "Find directory";
			fd.SetFindDirectory();
			parent.Layout.Add( fd );

			if ( fd.Execute() )
			{
				lineEdit.Text = fd.SelectedFile;
			}
		};

		widget.Layout.Add( button );
		parent.Layout.Add( widget );

		return lineEdit;
	}

	public void CreateUI()
	{
		Clear();

		StatusBar.Visible = false;

		var w = new Widget( null );
		w.SetLayout( LayoutMode.TopToBottom );
		w.SetSizeMode( SizeMode.Default, SizeMode.CanShrink );
		w.Layout.Margin = new( 8 );

		//
		// Paths
		//
		var paths = new Widget( w );
		paths.SetLayout( LayoutMode.TopToBottom );

		var projectPathEdit = AddDirectoryPicker( "Addon Path", w, BridgeImporter.ProjectPath );
		var outputPathEdit = AddDirectoryPicker( "Output directory", w, BridgeImporter.ExportDirectory );

		w.Layout.Add( paths );

		//
		// Numbers
		//
		var numbers = new Widget( w );
		numbers.SetLayout( LayoutMode.LeftToRight );

		var serverPortEdit = AddNumberEdit( "Server port", numbers, false, BridgeImporter.ServerPort.ToString() );
		var scaleEdit = AddNumberEdit( "Scale", numbers, true, BridgeImporter.Scale.ToString() );
		var lodIncrementEdit = AddNumberEdit( "LOD increment", numbers, false, BridgeImporter.LodIncrement.ToString() );

		numbers.SetStyles( "padding-bottom: 0; padding-top: 16px; margin-bottom: 6px;" );
		w.Layout.Add( numbers );

		//
		// Buttons
		//
		var buttons = new Widget( w );
		buttons.SetLayout( LayoutMode.LeftToRight );
		buttons.Layout.AddStretchCell();

		var saveButton = new Button( "Save", "save", buttons );
		saveButton.Clicked += () =>
		{
			BridgeImporter.ProjectPath = projectPathEdit.Text;
			BridgeImporter.ExportDirectory = outputPathEdit.Text;
			BridgeImporter.ServerPort = int.Parse( serverPortEdit.Text );
			BridgeImporter.Scale = float.Parse( scaleEdit.Text );
			BridgeImporter.LodIncrement = float.Parse( lodIncrementEdit.Text );
		};
		buttons.Layout.Add( saveButton );

		w.Layout.Add( buttons );
		Canvas = w;
	}

	[Sandbox.Event.Hotload]
	public void OnHotload()
	{
		CreateUI();
	}
}
