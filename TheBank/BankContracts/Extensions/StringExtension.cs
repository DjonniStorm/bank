namespace BankContracts.Extensions;

public static class StringExtension
{
    public static bool IsEmpty(this string str)
    {
        return string.IsNullOrWhiteSpace(str);
    }

    public static bool IsGuid(this string str)
    {
        return Guid.TryParse(str, out _);
    }
}
