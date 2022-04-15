using Sandbox;
using System.Reflection;
using Tools;

namespace QuixelBridge;

public class PropertyRow : Widget
{
	private DisplayInfo info;
	private int labelWidth = 125;

	public PropertyRow( Widget parent, int labelWidth = 125 ) : base( parent )
	{
		this.labelWidth = labelWidth;

		SetLayout( LayoutMode.LeftToRight );
		Layout.Margin = new( labelWidth, 2, 0, 2 );
	}

	public PropertyRow( Widget parent, object target, string propertyName ) : this( parent )
	{
		var prop = target.GetType().GetProperty( propertyName );
		SetLabel( prop );

		var w = CanEditAttribute.CreateEditorFor( prop );
		if ( w != null )
		{
			w.DataBinding = new PropertyBind( target, prop );
			SetWidget( w );
		}
	}

	public void SetLabel( string text )
	{
		info.Name = text;
	}

	public void SetLabel( PropertyInfo info )
	{
		this.info = DisplayInfo.ForProperty( info );
	}

	public T SetWidget<T>( T w ) where T : Widget
	{
		Layout.Add( w, 1 );

		if ( info.Placeholder != null && w is LineEdit e )
		{
			e.PlaceholderText = info.Placeholder;
		}

		return w;
	}

	protected override void OnPaint()
	{
		base.OnPaint();

		if ( string.IsNullOrEmpty( info.Name ) )
			return;

		var size = new Rect( 0, Size );

		size.width = labelWidth;
		size.left += 2;

		if ( size.height > 28 )
			size.height = 28;

		Paint.SetDefaultFont();
		Paint.DrawText( size, info.Name, TextFlag.LeftCenter );
	}
}
