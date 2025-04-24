namespace BankContracts.Extensions;

/// <summary>
/// расширение для проверки дат
/// </summary>
public static class DateTimeExtensions
{
    public static bool IsDateNotOlder(this DateTime date, DateTime olderDate)
    {
        return date >= olderDate;
    }
}
