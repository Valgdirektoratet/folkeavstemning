namespace Shared.Configuration;

public static class StringExtension
{
    public static string ToLinuxPortableCharacterSet(this string val) =>
        val.Replace("æ", "e").Replace("ø", "o").Replace("å", "a")
            .Replace("Æ", "E").Replace("Ø", "O").Replace("Å", "A");
}
