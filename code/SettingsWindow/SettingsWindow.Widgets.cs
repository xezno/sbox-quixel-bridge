using Sandbox;
using System.Collections.Generic;
using Tools;

namespace QuixelBridge;

partial class SettingsWindow
{
	private static LineEdit AddLineEdit( string label, string subtitle, Layout parent, string value = "" )
	{
		AddTitle( label, parent );
		AddSubtitle( subtitle, parent );

		var lineEdit = new LineEdit( value );
		parent.Add( lineEdit, 1 );

		return lineEdit;
	}

	private static string SelectedAddonPath;
	private static ComboBox AddAddonPicker( string title, string subtitle, Layout parent, IReadOnlyList<LocalAddon> addons )
	{
		AddTitle( title, parent );
		AddSubtitle( subtitle, parent );

		var comboBox = new ComboBox();

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

			if ( addon.GetRootPath() == BridgeImporter.Settings.ProjectPath )
				comboBox.CurrentIndex = comboBoxIndex;

			comboBoxIndex++;
		}

		parent.Add( comboBox, 1 );
		return comboBox;
	}
	private static ComboBox AddComboBox( string label, string subtitle, Layout parent, List<string> items, string selectedItem )
	{
		AddTitle( label, parent );
		AddSubtitle( subtitle, parent );

		var comboBox = new ComboBox();

		foreach ( var item in items )
		{
			comboBox.AddItem( item );
		}

		comboBox.TrySelectNamed( selectedItem );

		parent.Add( comboBox, 1 );
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

	private static Label AddSubtitle( string text, Layout parent )
	{
		var label = new Label( text );
		label.SetSizeMode( SizeMode.Default, SizeMode.CanShrink );
		label.SetStyles( "color: gray;" );

		parent.Add( label, 1 );

		return label;
	}

	private static Label AddTitle( string text, Layout parent )
	{
		var label = new Label( text );
		label.SetSizeMode( SizeMode.Default, SizeMode.CanShrink );
		label.SetStyles( "color: white; font-size: 12px;" );

		parent.Add( label, 1 );

		return label;
	}
}
