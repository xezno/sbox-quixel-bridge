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

	public static string PathRelativeTo( this string str, string relativePath )
	{
		return str.Replace( relativePath + "/", "" );
	}

	// s&box uses / as separator
	public static string NormalizePath( this string str )
	{
		var res = str;
		res = res.Replace( "\\", "/" );

		return res;
	}
}
