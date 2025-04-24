namespace BankContracts.Exceptions;

/// <summary>
/// Ошибка для не найденного элемента в бизнес логике и контрактах
/// </summary>
/// <param name="message"></param>
public class ElementNotFoundException(string message) : Exception(message) { }
