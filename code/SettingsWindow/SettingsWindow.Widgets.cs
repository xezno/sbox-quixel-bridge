﻿using Sandbox;
using System.Collections.Generic;
using Tools;

namespace QuixelBridge;

partial class SettingsWindow
{
	private static LineEdit AddNumberEdit( string label, Widget parent, bool middle, string value = "" )
	{
		var widget = new Widget( parent );
		widget.SetLayout( LayoutMode.TopToBottom );

		var l = new Label( widget );
		l.Text = label;
		widget.Layout.Add( l );

		var lineEdit = new LineEdit( value, parent );
		lineEdit.SetStyles( "padding: 6px; background-color: #38393c; border-radius: 2px; margin-top: 5px;" );
		widget.Layout.Add( lineEdit, 1 );

		if ( middle )
			widget.SetStyles( "margin-left: 4px; margin-right: 4px;" );

		parent.Layout.Add( widget );

		return lineEdit;
	}

	private static string SelectedAddonPath;
	private static ComboBox AddAddonPicker( string label, Widget parent, IReadOnlyList<LocalAddon> addons )
	{
		var widget = new PropertyRow( parent );

		widget.SetLabel( label );

		var comboBox = new ComboBox( parent );
		comboBox.SetStyles( "padding: 6px; background-color: #38393c; border-radius: 2px; margin-right: 4px;" );

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
		label.SetStyles( "font-size: 12px; font-weight: 600; text-transform: uppercase; font-family: Poppins;" );

		parent.Layout.Add( label, 1 );

		return label;
	}
}
