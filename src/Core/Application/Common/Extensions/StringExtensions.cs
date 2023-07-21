namespace Cleanception.Application.Common.Extensions;
public static class StringExtensions
{
    public static bool HasValue(this string? str)
       => !string.IsNullOrWhiteSpace(str) && !string.IsNullOrEmpty(str);

    public static bool IsEmpty(this string? str)
   => string.IsNullOrWhiteSpace(str) || string.IsNullOrEmpty(str);

    public static string ListStringify(this List<string>? lst)
    {
        if (lst == null)
            return string.Empty;

        return $"[{string.Join(", ", lst)}]";
    }

    public static string GenerateRandomNumber()
    {
        Random rnd = new Random();
        string result = string.Empty;

        for (int j = 0; j < 15; j++)
        {
            result += rnd.Next(0, 9);
        }

        return result;
    }
}
