using System.Text.Json;
using BankContracts.BusinessLogicContracts;
using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.Extensions;
using BankContracts.StorageContracts;
using Microsoft.Extensions.Logging;

namespace BankBusinessLogic.Implementations;

/// <summary>
/// реализация бизнес логики для клиента
/// </summary>
/// <param name="clientStorageContract">контракт клиента</param>
/// <param name="clerkStorageContract">контракт клерка</param>
/// <param name="logger">логгер</param>
internal class ClientBusinessLogicContract(
    IClientStorageContract clientStorageContract,
    IClerkStorageContract clerkStorageContract,
    ILogger logger
) : IClientBusinessLogicContract
{
    private readonly IClientStorageContract _clientStorageContract = clientStorageContract;
    private readonly IClerkStorageContract _clerkStorageContract = clerkStorageContract;
    private readonly ILogger _logger = logger;

    public List<ClientDataModel> GetAllClients()
    {
        _logger.LogInformation("get all clients");
        return _clientStorageContract.GetList();
    }

    public List<ClientDataModel> GetClientByClerk(string clerkId)
    {
        _logger.LogInformation("GetClientByClerk params: {clerkId}", clerkId);
        if (clerkId.IsEmpty())
        {
            throw new ArgumentNullException(nameof(clerkId));
        }
        if (!clerkId.IsGuid())
        {
            throw new ValidationException(
                "The value in the field clerkId is not a unique identifier."
            );
        }
        return _clientStorageContract.GetList(clerkId: clerkId)
            ?? throw new NullListException($"{clerkId}");
    }

    public ClientDataModel GetClientByData(string data)
    {
        _logger.LogInformation("Get client by data: {data}", data);
        if (data.IsEmpty())
        {
            throw new ArgumentNullException(nameof(data));
        }
        if (data.IsGuid())
        {
            return _clientStorageContract.GetElementById(data)
                ?? throw new ElementNotFoundException($"element not found: {data}");
        }
        throw new ElementNotFoundException($"element not found: {data}");
    }

    public void InsertClient(ClientDataModel clientDataModel)
    {
        _logger.LogInformation(
            "Insert client: {client}",
            JsonSerializer.Serialize(clientDataModel)
        );
        ArgumentNullException.ThrowIfNull(clientDataModel);
        clientDataModel.Validate();
        _clientStorageContract.AddElement(clientDataModel);
    }

    public void UpdateClient(ClientDataModel clientDataModel)
    {
        _logger.LogInformation(
            "Update client: {client}",
            JsonSerializer.Serialize(clientDataModel)
        );
        ArgumentNullException.ThrowIfNull(clientDataModel);
        clientDataModel.Validate();
        _clientStorageContract.UpdElement(clientDataModel);
    }
}
