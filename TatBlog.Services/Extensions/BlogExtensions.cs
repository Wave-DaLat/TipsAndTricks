using System.Text.RegularExpressions;

namespace TatBlog.Services.Extensions;

public static class BlogExtensions
{
    public static IEnumerable<string> SplitCamelCase(this string input)
    {
        return Regex.Split(input, @"([A-Z]?[a-z]+)").Where(str => !string.IsNullOrEmpty(str));
    }

    public static string FirstCharUppercase(this string input)
    {
        return $"{char.ToUpper(input[0])}{input.Substring(1)}";
    }

    public static string GenerateSlug(this string slug)
    {
        var splitToValidFormat = slug.Split(new[] { " ", ",", ";", ".", "-", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < splitToValidFormat.Length; i++)
        {
            splitToValidFormat[i] = splitToValidFormat[i].FirstCharUppercase();
        }
        var refixAlphabet = splitToValidFormat;
        var slugFormat = string.Join("", refixAlphabet);
        var reflectionSlug = String.Join("-", slugFormat.SplitCamelCase());

        return reflectionSlug.ToLower();
    }
}
