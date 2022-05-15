namespace QuixelBridge;

public static class StringExtension
{
	/// <summary>
	/// Convert to a "source-friendly" / "sbox-friendly" string
	/// (separated with underscores, all lowercase)
	/// </summary>
	public static string ToSourceName( this string str )
	{
		var res = str;

		res = res.Replace( " ", "_" );
		res = res.ToLower();

		return res;
	}

	/// <summary>
	/// Get relative path
	/// </summary>
	public static string PathRelativeTo( this string str, string relativePath )
	{
		return str.Replace( relativePath + "/", "" );
	}
}
