using Sandbox;
using Tools;

public class ProgressBar : Widget
{
	public ProgressBar( Widget parent = null ) : base( parent )
	{

	}

	float _value;
	public float Value
	{
		get => _value;
		set
		{
			if ( _value == value ) return;
			_value = value;
			Update();
		}
	}

	public float Curve = 2.0f;
	public Color Color = Theme.Green;

	protected override void OnPaint()
	{
		Paint.Antialiasing = true;
		Draw( LocalRect, Value.Clamp( 0, 1 ), Color, Curve );
	}

	public static void Draw( Rect rect, float progress, Color color, float curve = 2.0f )
	{
		progress = progress.Clamp( 0, 1 );

		// background
		Paint.ClearPen();
		Paint.SetBrush( color.Darken( 0.5f ).Desaturate( 0.4f ).WithAlpha( 0.2f ) );
		Paint.DrawRect( rect, curve );

		// process
		var prect = rect.Shrink( 1 );
		prect.Width *= progress;
		Paint.SetBrush( color );
		Paint.DrawRect( prect, curve - 1 );
	}
}
