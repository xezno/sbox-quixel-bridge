using System.Collections.Generic;
using Tools;

/*
 * TODO:
 * - BUG: Figure out whether I was fucking up somewhere or whether SetStylesheetFile actually doesn't work
 */
namespace QuixelBridge;

[Tool( "Bridge Settings", "settings", "Settings for the Quixel Bridge plugin" )]
public partial class SettingsWindow : Dialog
{
	[Menu( "Editor", "Quixel/Settings", "settings" )]
	public static void OpenWindow()
	{
		_ = new SettingsWindow( null );
	}

	public SettingsWindow( Widget parent ) : base( null )
	{
		Window.Title = "Quixel Bridge Settings";
		Window.SetWindowIcon( "settings" );
		Window.Size = new Vector2( 400, 200 );
		Window.MaximumSize = Size;

		LoadSettings();
		CreateUI();
		Show();
	}

	ComboBox entityEdit;
	LineEdit serverPortEdit, lodIncrementEdit;

	public void CreateUI()
	{
		SetLayout( LayoutMode.TopToBottom );
		Layout.Spacing = 4;

		//
		// Addon Settings
		//
		{
			var addonSettings = Layout.Add( LayoutMode.TopToBottom );
			addonSettings.Margin = 20;
			addonSettings.Spacing = 8;

			var addonName = AddAddonPicker(
				"Export Addon",
				"The addon where your imports will get sent to",
				addonSettings,
				Utility.Addons.GetAll() );
		}

		Layout.AddStretchCell( 1 );
		Layout.AddSeparator();
		Layout.AddStretchCell( 1 );

		//
		// Import Settings
		//
		{
			var importSettings = Layout.Add( LayoutMode.TopToBottom );
			importSettings.Margin = 20;
			importSettings.Spacing = 8;

			var validEntities = new List<string>() { "prop_static", "prop_physics", "prop_dynamic" };

			entityEdit = AddComboBox(
				"Entity Type",
				"The entity type each imported model will use",
				importSettings,
				validEntities,
				BridgeImporter.Settings.Entity );

			importSettings.AddSpacingCell( 8 );

			serverPortEdit = AddLineEdit(
				"Server port",
				"The port to listen on (should be the same in Quixel Bridge)",
				importSettings,
				BridgeImporter.Settings.ServerPort.ToString() );

			importSettings.AddSpacingCell( 8 );

			lodIncrementEdit = AddLineEdit(
				"LOD increment",
				"How often LODs should get switched out",
				importSettings,
				BridgeImporter.Settings.LodIncrement.ToString() );
		}

		Layout.AddStretchCell( 1 );
		Layout.AddSeparator();
		Layout.AddStretchCell( 1 );

		//
		// Buttons
		//
		{
			var buttons = new Widget( this );
			buttons.SetSizeMode( SizeMode.Default, SizeMode.CanShrink );
			buttons.SetLayout( LayoutMode.LeftToRight );
			buttons.Layout.AddStretchCell();
			buttons.Layout.Margin = 20;
			buttons.Layout.Spacing = 8;

			var saveButton = new Button( "Save and Close", buttons );
			saveButton.ButtonType = "primary";
			saveButton.Clicked += () =>
			{
				BridgeImporter.Settings.ProjectPath = SelectedAddonPath;
				BridgeImporter.Settings.ServerPort = int.Parse( serverPortEdit.Text );
				BridgeImporter.Settings.LodIncrement = float.Parse( lodIncrementEdit.Text );
				BridgeImporter.Settings.Entity = entityEdit.CurrentText;

				SaveSettings();
				Close();
			};

			var cancelButton = new Button( "Cancel", buttons );
			cancelButton.Clicked += () =>
			{
				Close();
			};

			buttons.Layout.Add( saveButton );
			buttons.Layout.Add( cancelButton );

			Layout.Add( buttons );
		}
	}

	[Sandbox.Event.Hotload]
	public void OnHotload()
	{
		CreateUI();
	}
}
