using System.Collections.Generic;
using System.Diagnostics;
using Tools;

namespace QuixelBridge;

public partial class SettingsWindow : Dialog
{
	private ComboBox entityEdit;
	private LineEdit serverPortEdit, lodIncrementEdit;
	private CheckBox audioEnabledEdit;
	private static string selectedAddonPath;

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

		CreateUI();
		Show();
	}

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
				Utility.Projects.GetAll() );
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
				BridgeSettings.Instance.Entity );

			importSettings.AddSpacingCell( 8 );

			serverPortEdit = AddLineEdit(
				"Server port",
				"The port to listen on (should be the same in Quixel Bridge)",
				importSettings,
				BridgeSettings.Instance.ServerPort.ToString() );

			importSettings.AddSpacingCell( 8 );

			lodIncrementEdit = AddLineEdit(
				"LOD increment",
				"How often LODs should get switched out",
				importSettings,
				BridgeSettings.Instance.LodIncrement.ToString() );

			importSettings.AddSpacingCell( 8 );

			audioEnabledEdit = AddCheckBox(
				"Play Sound on Complete",
				"Sound will only play if you have more than one item in the queue",
				importSettings,
				BridgeSettings.Instance.EnableAudio );
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
			buttons.Layout.Margin = 20;
			buttons.Layout.Spacing = 8;

			// Dumb way of getting a button to look like a link, can't find any other good way to do this
			// (adding a label with <a> tags doesn't work)
			var link = new Button( "View on GitHub" );
			link.SetStyles( "padding: 0; background-color: transparent; text-decoration: underline; text-align: left; border: none; width: auto;" );
			link.Clicked += () =>
			{
				var githubUrl = "https://github.com/xezno/sbox-quixel-bridge#readme";
				Process.Start( new ProcessStartInfo( githubUrl )
				{
					UseShellExecute = true
				} );
			};

			buttons.Layout.Add( link, 1 );
			buttons.Layout.AddStretchCell();

			var saveButton = new Button( "Save and Close", buttons );
			saveButton.ButtonType = "primary";
			saveButton.Clicked += () =>
			{
				BridgeSettings.Instance.ProjectPath = selectedAddonPath;
				BridgeSettings.Instance.ServerPort = int.Parse( serverPortEdit.Text );
				BridgeSettings.Instance.LodIncrement = float.Parse( lodIncrementEdit.Text );
				BridgeSettings.Instance.Entity = entityEdit.CurrentText;
				BridgeSettings.Instance.EnableAudio = audioEnabledEdit.Value;

				BridgeSettings.Instance.SaveToDisk();
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
