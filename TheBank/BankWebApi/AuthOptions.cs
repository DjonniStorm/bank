using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BankWebApi;

/// <summary>
/// настройки для авторизации
/// переделаем потом если не будет впадлу
/// </summary>
public class AuthOptions
{
    public const string ISSUER = "Bank_AuthServer"; // издатель токена
    public const string AUDIENCE = "Bank_AuthClient"; // потребитель токена
    const string KEY = "banksuperpupersecret_secretsecretsecretkey!";   // ключ для шифрации
    public static SymmetricSecurityKey GetSymmetricSecurityKey() => new(Encoding.UTF8.GetBytes(KEY));
}
