using System.Collections.Generic;
using Tools;

/*
 * TODO:
 * - BUG: Figure out whether I was fucking up somewhere or whether SetStylesheetFile actually doesn't work
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
		Size = new Vector2( 400, 300 );
		MaximumSize = Size;
		MinimumSize = Size;
		ResizeButtonsVisible = false;
		CloseButtonVisible = false;

		LoadSettings();
		CreateUI();
		Show();
	}

	ComboBox entityEdit;
	LineEdit serverPortEdit, lodIncrementEdit;

	public void CreateUI()
	{
		Clear();

		StatusBar.Visible = false;

		var w = new Widget( null );
		w.SetLayout( LayoutMode.TopToBottom );
		w.SetSizeMode( SizeMode.Default, SizeMode.Default );
		w.Layout.Margin = new( 8 );
		w.SetStyles( "background-color: #38393c; border-radius: 2px;" );

		//
		// Addon Settings
		//
		{
			AddTitle( "Addon Settings", w );
			var addonSettings = new Widget( w );
			addonSettings.SetSizeMode( SizeMode.Default, SizeMode.CanShrink );
			addonSettings.SetLayout( LayoutMode.TopToBottom );

			var addonName = AddAddonPicker( "Export Addon", addonSettings, Utility.Addons.GetAll() );
			w.Layout.Add( addonSettings );
		}

		//
		// Import Settings
		//
		{
			AddTitle( "Import Settings", w );
			var importSettings = new Widget( w );
			importSettings.SetSizeMode( SizeMode.Default, SizeMode.CanShrink );
			importSettings.SetLayout( LayoutMode.TopToBottom );

			var validEntities = new List<string>() { "prop_static", "prop_physics", "prop_dynamic" };

			entityEdit = AddComboBox( "Prop Type",
								   importSettings,
								   validEntities,
								   BridgeImporter.Settings.Entity );

			serverPortEdit = AddNumberEdit( "Server port",
										 importSettings,
										 false,
										 BridgeImporter.Settings.ServerPort.ToString() );

			lodIncrementEdit = AddNumberEdit( "LOD increment",
										   importSettings,
										   false,
										   BridgeImporter.Settings.LodIncrement.ToString() );

			w.Layout.Add( importSettings );
		}

		//
		// Buttons
		//
		{
			var buttons = new Widget( w );
			buttons.SetSizeMode( SizeMode.Default, SizeMode.CanShrink );
			buttons.SetLayout( LayoutMode.LeftToRight );
			buttons.Layout.AddStretchCell();

			var cancelButton = new Button( "Cancel", "close", buttons );
			cancelButton.SetStyles( "margin-right: 4px; background-color: #201f21; border: 0px;" );
			cancelButton.Clicked += () =>
			{
				Close();
			};

			buttons.Layout.Add( cancelButton );

			var saveButton = new Button( "Save and Close", "save", buttons );
			saveButton.SetStyles( "background-color: #201f21; border: 0px;" );
			saveButton.Clicked += () =>
			{
				BridgeImporter.Settings.ProjectPath = SelectedAddonPath;
				BridgeImporter.Settings.ServerPort = int.Parse( serverPortEdit.Text );
				BridgeImporter.Settings.LodIncrement = float.Parse( lodIncrementEdit.Text );
				BridgeImporter.Settings.Entity = entityEdit.CurrentText;

				SaveSettings();
				Close();
			};

			buttons.Layout.Add( saveButton );
			buttons.SetStyles( "margin-top: 10px;" );

			w.Layout.Add( buttons );
		}

		Canvas = w;
	}

	[Sandbox.Event.Hotload]
	public void OnHotload()
	{
		CreateUI();
	}
}
