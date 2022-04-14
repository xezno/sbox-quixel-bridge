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
}
