using Sandbox;
using System.Reflection;
using Tools;

public class PropertyRow : Widget
{
	DisplayInfo Info;

	int LabelWidth = 125;

	public PropertyRow( Widget parent ) : base( parent )
	{
		SetLayout( LayoutMode.LeftToRight );
		Layout.Margin = new( LabelWidth, 2, 8, 2 );
		MinimumSize = 23;
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
		Info.Name = text;
	}

	public void SetLabel( PropertyInfo info )
	{
		Info = DisplayInfo.ForProperty( info );
	}

	public T SetWidget<T>( T w ) where T : Widget
	{
		Layout.Add( w, 1 );

		if ( Info.Placeholder != null && w is LineEdit e )
		{
			e.PlaceholderText = Info.Placeholder;
		}

		return w;
	}

	protected override void OnPaint()
	{
		base.OnPaint();

		if ( string.IsNullOrEmpty( Info.Name ) )
			return;

		var size = new Rect( 0, Size );
		size.width = LabelWidth - 16;

		if ( size.height > 28 )
			size.height = 28;

		size.left += 16;
		Paint.SetDefaultFont();
		Paint.SetPen( Theme.Grey.Lighten( 0.3f ) );
		Paint.DrawText( size, Info.Name, TextFlag.RightCenter );

		//	Paint.SetPen( Theme.Black.WithAlpha( 0.2f ) );
		//	Paint.DrawLine( new Vector2( 0, size.bottom-1 ), new Vector2( Size.x, size.bottom-1 ) );

	}
}
