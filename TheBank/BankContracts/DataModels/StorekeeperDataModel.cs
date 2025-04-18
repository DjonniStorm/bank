using BankContracts.Exceptions;
using BankContracts.Extensions;
using BankContracts.Infrastructure;
using System.Text.RegularExpressions;

namespace BankContracts.DataModels;

/// <summary>
/// Дата модель кладовщика
/// </summary>
/// <param name="id">уникальный Guid индентификатор</param>
/// <param name="name">имя</param>
/// <param name="surname">фамилия</param>
/// <param name="middleName">отчество (лучше не придумал)</param>
/// <param name="login">логин</param>
/// <param name="password">пароль</param>
/// <param name="email">адрес электронной почты</param>
/// <param name="phoneNumber">номер телефона</param>
public class StorekeeperDataModel(
    string id, string name, string surname, string middleName,
    string login, string password, string email, string phoneNumber) : IValidation
{
    public string Id { get; private set; } = id;

    public string Name { get; private set; } = name;

    public string Surname { get; private set; } = surname;

    public string MiddleName { get; private set; } = middleName;

    public string Login {  get; private set; } = login;

    public string Password { get; private set; } = password;

    public string Email { get; private set; } = email;

    public string PhoneNumber { get; private set; } = phoneNumber;

    public void Validate()
    {
        if (Id.IsEmpty())
        {
            throw new ValidationException("Field Id is null or empty");
        }
        if (!Id.IsGuid())
        {
            throw new ValidationException("The value in the field Id is not a unique identifier");
        }
        if (Name.IsEmpty())
        {
            throw new ValidationException("Field Name is null or empty");
        }
        if (Surname.IsEmpty())
        {
            throw new ValidationException("Field Surname is null or empty");
        }
        if (MiddleName.IsEmpty())
        {
            throw new ValidationException("Field MiddleName is null or empty");
        }
        if (Login.IsEmpty())
        {
            throw new ValidationException("Field Login is null or empty");
        }
        if (Password.IsEmpty())
        {
            throw new ValidationException("Field Password is null or empty");
        }
        if (Email.IsEmpty())
        {
            throw new ValidationException("Field Email is null or empty");
        }
        if (!Regex.IsMatch(Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
        {
            throw new ValidationException("Field Email is not a valid email address");
        }
        if (PhoneNumber.IsEmpty())
        {
            throw new ValidationException("Field PhoneNumber is null or empty");
        }
        if (!Regex.IsMatch(PhoneNumber, @"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$"))
        {
            throw new ValidationException("Field PhoneNumber is not a phone number");
        }
    }
}
