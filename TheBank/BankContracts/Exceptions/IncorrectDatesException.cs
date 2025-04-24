namespace BankContracts.Exceptions;

public class IncorrectDatesException(DateTime from, DateTime to) : Exception($"date: {from} is older than {to}") { }
