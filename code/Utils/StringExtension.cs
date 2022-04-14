namespace QuixelBridge;

public static class StringExtension
{
	public static string ToPathString( this string str )
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

	/// <summary>
	/// Normalize path so that it uses "/" as separating character
	/// </summary>
	public static string NormalizePath( this string str )
	{
		var res = str;
		res = res.Replace( "\\", "/" );

		return res;
	}
}
