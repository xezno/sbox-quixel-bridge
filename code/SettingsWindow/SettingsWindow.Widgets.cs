using Sandbox;
using System.Collections.Generic;
using Tools;

namespace QuixelBridge;

partial class SettingsWindow
{
	private static LineEdit AddNumberEdit( string label, Widget parent, bool middle, string value = "" )
	{
		var widget = new PropertyRow( parent );
		widget.SetLabel( label );

		var lineEdit = new LineEdit( value, parent );
		lineEdit.SetStyles( "padding: 2px; background-color: #38393c; border-radius: 2px; margin-top: 5px;" );
		widget.Layout.Add( lineEdit, 1 );

		parent.Layout.Add( widget );

		return lineEdit;
	}

	private static string SelectedAddonPath;
	private static ComboBox AddAddonPicker( string label, Widget parent, IReadOnlyList<LocalAddon> addons )
	{
		var widget = new PropertyRow( parent );

		widget.SetLabel( label );

		var comboBox = new ComboBox( parent );
		comboBox.SetStyles( "background-color: #38393c;" );

		int comboBoxIndex = 0;
		for ( int i = 0; i < addons.Count; i++ )
		{
			LocalAddon addon = addons[i];
			if ( !addon.Active )
				continue;

			string icon = GetAddonIcon( addon );
			comboBox.AddItem( addon.Config.Title, icon, () =>
			{
				SelectedAddonPath = addon.GetRootPath();
			} );

			if ( addon.GetRootPath() == BridgeImporter.ProjectPath )
				comboBox.CurrentIndex = comboBoxIndex;

			comboBoxIndex++;
		}

		widget.Layout.Add( comboBox, 1 );
		parent.Layout.Add( widget );

		return comboBox;
	}
	private static ComboBox AddComboBox( string label, Widget parent, List<string> items, string selectedItem )
	{
		var widget = new PropertyRow( parent );

		widget.SetLabel( label );

		var comboBox = new ComboBox( parent );
		comboBox.SetStyles( "background-color: #38393c;" );

		foreach ( var item in items )
		{
			comboBox.AddItem( item );
		}

		comboBox.TrySelectNamed( selectedItem );

		widget.Layout.Add( comboBox, 1 );
		parent.Layout.Add( widget );

		return comboBox;
	}

	private static string GetAddonIcon( LocalAddon addon )
	{
		string icon = "folder";
		if ( addon.Config.Type == "map" ) icon = "public";
		if ( addon.Config.Type == "game" ) icon = "sports_esports";
		if ( addon.Config.Type == "content" ) icon = "perm_media";
		return icon;
	}

	private static Label AddTitle( string text, Widget parent )
	{
		var label = new Label( text, parent );
		label.SetStyles( "color: white;" );

		parent.Layout.Add( label, 1 );

		return label;
	}
}
