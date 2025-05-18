using BankContracts.DataModels;

namespace BankWebApi.Infrastructure;

public interface IJwtProvider
{
    string GenerateToken(StorekeeperDataModel dataModel);
}
