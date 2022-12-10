using System;

public class ProgressWindow : Window
{
	Widget Content;

	public ProgressWindow() : base( null )
	{
		MinimumSize = new Vector2( 500, 10 );
		IsDialog = true;
		StatusBar = null;
		Title = "Progress Popup";

		Content = new Widget( this );
		Content.Size = 1;

		Content.SetLayout( LayoutMode.TopToBottom );
		Content.Layout.Spacing = 1;
		AdjustSize();

		SetModal( true, true );
	}

	public ProgressRow AddProgressRow( string name )
	{
		var row = new ProgressRow( Content );
		row.SetTitle( name );
		AddRow( row );
		return row;
	}

	public void AddRow( Widget row )
	{
		Content.Layout.Add( row );

		row.Size = Size;
		DoLayout();
	}

	public void RemoveRow( Widget widget )
	{
		widget.Visible = false;
		widget.Destroy();

		DoLayout();
	}

	/// <summary>
	/// We want to resize the window based on the content
	/// </summary>
	protected override void DoLayout()
	{
		base.DoLayout();

		Content.Size = Size;
		Content.Height = Content.Children.Where( x => x.Visible ).Sum( x => MathF.Max( x.SizeHint.y, x.MinimumSize.y ) + 1 ) - 1;

		Content.Position = new Vector2( 0, MenuWidget.Height );
		Size = Content.Position + Content.Size + new Vector2( 0, 4 );
	}

	protected override void OnPaint()
	{
		base.OnPaint();

		Paint.Antialiasing = true;
		Paint.SetPen( Theme.ControlBackground.Darken( 0.4f ), 2.0f );
		Paint.SetBrush( Theme.ControlBackground );
		Paint.DrawRect( LocalRect.Shrink( 1 ), 0.0f );
	}
}
