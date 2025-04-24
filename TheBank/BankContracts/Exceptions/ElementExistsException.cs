namespace BankContracts.Exceptions;

/// <summary>
/// Исключение при работе с хранилищем
/// при добавлении существующего элемента
/// </summary>
/// <param name="message">сообщение</param>
public class ElementExistsException(string message) : Exception(message) { }
