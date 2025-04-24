namespace BankContracts.Exceptions;

/// <summary>
/// Исключение для не найденного элемента в бизнес логике и контрактах
/// </summary>
/// <param name="message"></param>
public class ElementNotFoundException(string message) : Exception(message) { }
