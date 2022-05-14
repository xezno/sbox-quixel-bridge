using Tools;

/*
 * TODO:
 * - BUG: Figure out whether I was fucking up somewhere or whether SetStylesheetFile actually doesn't work
 * - Find a nice way to open a dialog and tell the user that their settings have been saved
 * - Clean everything up a little
 */
namespace QuixelBridge;

[Tool( "Bridge Settings", "settings", "Settings for the Quixel Bridge plugin" )]
public partial class SettingsWindow : Window
{
	[Menu( "Editor", "Quixel/Settings", "settings" )]
	public static void OpenWindow()
	{
		_ = new SettingsWindow();
	}

	public SettingsWindow()
	{
		Title = "Quixel Bridge Settings";
		Size = new Vector2( 400, 250 );
		MaximumSize = Size;
		MinimumSize = Size;
		ResizeButtonsVisible = false;
		CloseButtonVisible = false;

		Load();
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
		w.Layout.Margin = new( 8 );

		//
		// Paths
		//
		AddTitle( "Addon", w );
		var paths = new Widget( w );
		paths.SetLayout( LayoutMode.TopToBottom );

		var addonName = AddAddonPicker( "Export Addon", w, Utility.Addons.GetAll() );

		w.Layout.Add( paths );

		//
		// Numbers
		//
		AddTitle( "Imports", w );
		var numbers = new Widget( w );
		numbers.SetLayout( LayoutMode.TopToBottom );

		var serverPortEdit = AddNumberEdit( "Server port", numbers, false, BridgeImporter.ServerPort.ToString() );
		var scaleEdit = AddNumberEdit( "Scale", numbers, true, BridgeImporter.Scale.ToString() );
		var lodIncrementEdit = AddNumberEdit( "LOD increment", numbers, false, BridgeImporter.LodIncrement.ToString() );

		w.Layout.Add( numbers );

		//
		// Buttons
		//
		var buttons = new Widget( w );
		buttons.SetLayout( LayoutMode.LeftToRight );
		buttons.Layout.AddStretchCell();

		var cancelButton = new Button( "Cancel", "close", buttons );
		cancelButton.SetStyles( "margin-right: 4px; background-color: #38393c; border: 0px;" );
		cancelButton.Clicked += () =>
		{
			Close();
		};

		buttons.Layout.Add( cancelButton );

		var saveButton = new Button( "Save and Close", "save", buttons );
		saveButton.SetStyles( "background-color: #38393c; border: 0px;" );
		saveButton.Clicked += () =>
		{
			BridgeImporter.ProjectPath = SelectedAddonPath;
			BridgeImporter.ServerPort = int.Parse( serverPortEdit.Text );
			BridgeImporter.Scale = float.Parse( scaleEdit.Text );
			BridgeImporter.LodIncrement = float.Parse( lodIncrementEdit.Text );

			Save();
			Close();
		};

		buttons.Layout.Add( saveButton );
		buttons.SetStyles( "margin-top: 10px;" );

		w.Layout.Add( buttons );
		Canvas = w;
	}

	[Sandbox.Event.Hotload]
	public void OnHotload()
	{
		CreateUI();
	}
}
