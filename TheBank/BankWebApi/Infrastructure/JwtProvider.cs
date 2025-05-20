using BankContracts.DataModels;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BankWebApi.Infrastructure;

public class JwtProvider : IJwtProvider
{
    public string GenerateToken(StorekeeperDataModel dataModel)
    {
        return GenerateToken(dataModel.Id);
    }

    public string GenerateToken(ClerkDataModel dataModel)
    {
        return GenerateToken(dataModel.Id);
    }

    private static string GenerateToken(string id)
    {
        var token = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: [new(ClaimTypes.NameIdentifier, id)],
            expires: DateTime.UtcNow.Add(TimeSpan.FromDays(2)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
