public static class Theme
{
	public static Color White;
	public static Color Grey;
	public static Color Black;

	public static Color Red;
	public static Color Green;
	public static Color Blue;
	public static Color Yellow;

	public static Color ControlBackground;
	public static Color Primary;

	public static float RowHeight;
	public static float ControlRadius;

	static Theme()
	{
		Init();
	}

	[Event.Hotload]
	public static void Init()
	{
		Blue = Color.Parse( "#A7CCFF" ) ?? default;
		Green = Color.Parse( "#B0E24D" ) ?? default;
		Red = Color.Parse( "#FB5A5A" ) ?? default;
		Yellow = Color.Parse( "#E6DB74" ) ?? default;

		White = Color.Parse( "#F8F8F8" ) ?? default;
		Grey = Color.Parse( "#808080" ) ?? default;
		Black = Color.Parse( "#111111" ) ?? default;

		Primary = Color.Parse( "#3472E6" ) ?? default;

		ControlBackground = Color.Parse( "#201F21" ) ?? default;
		ControlRadius = 3.0f;

		RowHeight = 22;
	}
}
