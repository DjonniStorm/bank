namespace BankContracts.Exceptions;

/// <summary>
/// Исключение для null списка
/// </summary>
/// <param name="message">сообщение</param>
public class NullListException(string message) : Exception(message) { }
