using System;
using System.Threading;

public static class Progress
{
	static ProgressWindow currentWindow;
	static int Popups;

	class ProgressSection : IDisposable
	{
		bool disposed;
		string oldTitle;

		public ProgressSection( string name )
		{
			currentWindow ??= new ProgressWindow();
			Popups++;

			// save state
			oldTitle = currentWindow.Title;

			// new state
			currentWindow.Title = name;

			currentWindow.Visible = true;
		}

		public void Dispose()
		{
			if ( disposed ) return;
			disposed = true;

			Popups--;

			if ( Popups <= 0 )
			{
				Popups = 0;
				currentWindow.Destroy();
				currentWindow = null;
			}
			else
			{
				// restore state
				currentWindow.Title = oldTitle;
			}

		}
	}

	public static IDisposable Start( string name ) => new ProgressSection( name );
	public static ProgressBar Bar( string name ) => new ProgressBar( name );


	public class ProgressBar : IDisposable
	{
		ProgressRow row;

		internal ProgressBar( string name )
		{
			if ( currentWindow == null )
				return;

			row = currentWindow.AddProgressRow( name );
		}

		public void Dispose()
		{
			if ( row == null )
				return;

			currentWindow.RemoveRow( row );
			row = null;
		}

		public void SetValues( float value, float total ) => row?.UpdateValues( value, total );
		public void SetTitle( string title ) => row?.SetTitle( title );
		public void SetProgressText( string title ) => row?.SetProgressText( title );
		public void SetSubtitle( string title ) => row?.SetSubtitle( title );
		public void SetIcon( string title ) => row?.SetIcon( title );

		internal CancellationToken SetCancel()
		{
			var cancel = new CancellationTokenSource();
			row?.SetCancel( cancel );
			return cancel.Token;
		}
	}

	internal static async Task StatementAsync( string title, string message, string button = "Okay" )
	{
		using ( var dialog = Start( title ) )
		{
			var widget = new Widget( null );
			widget.SetSizeMode( SizeMode.Default, SizeMode.CanGrow );

			widget.SetLayout( LayoutMode.TopToBottom );
			widget.Layout.Margin = 16;
			widget.Layout.Spacing = 16;
			var label = new Label( message );

			widget.Layout.Add( label );

			var footer = widget.Layout.Add( LayoutMode.LeftToRight );
			footer.AddStretchCell( -1 );

			bool clicked = false;

			var b = new Button( button );
			b.Clicked = () => clicked = true;

			footer.Add( b );

			widget.AdjustSize();

			currentWindow.AddRow( widget );

			while ( !clicked )
			{
				await Task.Delay( 100 );
			}
		}
	}
}
