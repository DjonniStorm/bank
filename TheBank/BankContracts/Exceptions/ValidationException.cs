namespace BankContracts.Exceptions;

/// <summary>
/// Класс-исключение при валидации данных в дата моделях
/// </summary>
public class ValidationException(string message) : Exception(message) { }