using System.Threading;
using Tools;

public class ProgressRow : Widget
{
	public string IconText;
	public string Title;
	public string BottomRight;
	public string BottomLeft;

	public float Current;
	public float Total;
	public float Progress => Current / Total;

	Button cancelButton;

	public ProgressRow( Widget parent ) : base( parent )
	{
		MinimumSize = 64;
		SetSizeMode( SizeMode.Default, SizeMode.CanGrow );
		Title = "";
		IconText = "schedule";

		cancelButton = new Button( null, "cancel", this );
		cancelButton.Visible = false;
	}

	public void SetTitle( string title )
	{
		Title = title;
		Update();
	}

	public void SetProgressText( string title )
	{
		BottomRight = title;
		Update();
	}

	public void SetSubtitle( string title )
	{
		BottomLeft = title;
		Update();
	}

	public void SetIcon( string title )
	{
		IconText = title;
		Update();
	}

	public void SetInfoText( string title )
	{
		BottomLeft = title;
		Update();
	}

	public void UpdateValues( float current, float total )
	{
		Current = current;
		Total = total;
		Update();
	}

	protected override void DoLayout()
	{
		base.DoLayout();

		if ( cancelButton != null )
		{
			//cancelButton.AdjustSize();

			var pos = Size - cancelButton.Size;
			pos.y /= 2.0f;
			pos.x -= 8.0f;

			cancelButton.Position = pos;
		}
	}

	protected override void OnPaint()
	{
		Paint.Antialiasing = true;

		var rect = LocalRect;

		var color = Theme.Green;

		Paint.ClearPen();
		Paint.SetBrush( Theme.Grey.WithAlpha( 0.1f ) );
		var r = rect;
		r.Height = 1;
		Paint.DrawRect( r );

		Paint.SetPen( color.WithAlpha( 0.3f ) );
		var iconRect = Paint.DrawIcon( rect.Shrink( 16, 0 ), IconText, 32, TextFlag.LeftCenter );

		var contentRect = LocalRect;
		contentRect.Left += iconRect.Right + 16;
		contentRect.Right -= 16.0f;

		if ( cancelButton != null && cancelButton.Visible )
		{
			contentRect.Right = cancelButton.Position.x - 16.0f;
		}

		var barRect = contentRect;
		barRect.Top += (contentRect.Height / 2.0f);
		barRect.Height = 6.0f;
		barRect.Top -= barRect.Height / 2.0f;
		ProgressBar.Draw( barRect, Progress, color );

		Paint.SetDefaultFont( 8 );
		Paint.SetPen( Theme.White.WithAlpha( 0.9f ) );
		Paint.DrawText( contentRect.Shrink( 4, 11 ), Title, TextFlag.Left | TextFlag.Top );

		if ( !string.IsNullOrEmpty( BottomRight ) )
		{
			Paint.SetDefaultFont( 8 );
			Paint.SetPen( Theme.White.WithAlpha( 0.7f ) );
			Paint.DrawText( contentRect.Shrink( 4, 11 ), BottomRight, TextFlag.Bottom | TextFlag.Right );
		}

		if ( !string.IsNullOrEmpty( BottomLeft ) )
		{
			Paint.SetDefaultFont( 8 );
			Paint.SetPen( Theme.White.WithAlpha( 0.7f ) );
			Paint.DrawText( contentRect.Shrink( 4, 11 ), BottomLeft, TextFlag.Bottom | TextFlag.Left );
		}
	}

	internal void SetCancel( CancellationTokenSource cancel )
	{
		cancelButton.Cursor = CursorShape.Finger;
		cancelButton.Clicked = () => cancel.Cancel();
		cancelButton.Visible = true;

		Update();
	}
}
