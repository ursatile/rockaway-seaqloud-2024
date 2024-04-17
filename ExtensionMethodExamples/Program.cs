// See https://aka.ms/new-console-template for more information

var s = "Hello World";
s.Flip();
StringExtensions.Flip(s);
var quotedString = s.Quote('#');
Console.Write(quotedString);

public static class StringExtensions {
	public static string Flip(this string s) {
		return new string(s.Reverse().ToArray());
	}
	public static string Quote(this string s, char q) {
		return q + s + q;
	}
}