namespace BankContracts.Exceptions;

/// <summary>
/// Исключение при работе с хранилищем
/// </summary>
/// <param name="message"></param>
public class StorageException(string message) : Exception(message) { }
