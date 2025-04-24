namespace BankContracts.Exceptions;

/// <summary>
/// Исключения для валидации дат
/// </summary>
/// <param name="from">начальная дата</param>
/// <param name="to">конечная дата</param>
public class IncorrectDatesException(DateTime from, DateTime to) : Exception($"date: {from} is older than {to}") { }
