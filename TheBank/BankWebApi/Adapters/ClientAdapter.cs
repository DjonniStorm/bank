using AutoMapper;
using BankBusinessLogic.Implementations;
using BankContracts.AdapterContracts;
using BankContracts.AdapterContracts.OperationResponses;
using BankContracts.BindingModels;
using BankContracts.BusinessLogicContracts;
using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.ViewModels;

namespace BankWebApi.Adapters;

public class ClientAdapter : IClientAdapter
{
    private readonly IClientBusinessLogicContract _clientBusinessLogicContract;

    private readonly ILogger _logger;

    private readonly Mapper _mapper;

    public ClientAdapter(IClientBusinessLogicContract clientBusinessLogicContract, ILogger logger)
    {
        _clientBusinessLogicContract = clientBusinessLogicContract;
        _logger = logger;
        var config = new MapperConfiguration(cfg =>
        {
            // Mapping for Client
            cfg.CreateMap<ClientBindingModel, ClientDataModel>();
            cfg.CreateMap<ClientDataModel, ClientViewModel>()
        .ForMember(dest => dest.DepositClients, opt => opt.MapFrom(src => src.DepositClients))
        .ForMember(dest => dest.CreditProgramClients, opt => opt.MapFrom(src => src.CreditProgramClients));

            // Mapping for Deposit
            cfg.CreateMap<DepositDataModel, DepositViewModel>()
        .ForMember(dest => dest.DepositClients, opt => opt.MapFrom(src => src.Currencies)); // Adjust if Currencies is meant to map to DepositClients

            // Mapping for ClientCreditProgram
            cfg.CreateMap<ClientCreditProgramBindingModel, ClientCreditProgramDataModel>();
            cfg.CreateMap<ClientCreditProgramDataModel, ClientCreditProgramViewModel>();

            // Mapping for DepositClient
            cfg.CreateMap<DepositClientBindingModel, DepositClientDataModel>();
            cfg.CreateMap<DepositClientDataModel, DepositClientViewModel>();
        });

        _mapper = new Mapper(config);
    }
    
    public ClientOperationResponse GetList()
    {
        try
        {
            return ClientOperationResponse.OK(
                [
                    .. _clientBusinessLogicContract
                        .GetAllClients()
                        .Select(x => _mapper.Map<ClientViewModel>(x)),
                ]
            );
        }
        catch (NullListException)
        {
            _logger.LogError("NullListException");
            return ClientOperationResponse.NotFound("The list is not initialized");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ClientOperationResponse.InternalServerError(
                $"Error while working with data storage:{ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ClientOperationResponse.InternalServerError(ex.Message);
        }
    }
    
    public ClientOperationResponse GetElement(string data)
    {
        try
        {
            return ClientOperationResponse.OK(
                _mapper.Map<ClientViewModel>(_clientBusinessLogicContract.GetClientByData(data))
            );
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return ClientOperationResponse.BadRequest("Data is empty");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException");
            return ClientOperationResponse.NotFound($"Not found element by data {data}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ClientOperationResponse.InternalServerError(
                $"Error while working with data storage: {ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ClientOperationResponse.InternalServerError(ex.Message);
        }
    }
    
    public ClientOperationResponse RegisterClient(ClientBindingModel clientModel)
    {
        try
        {
            _clientBusinessLogicContract.InsertClient(_mapper.Map<ClientDataModel>(clientModel));
            return ClientOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return ClientOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return ClientOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (ElementExistsException ex)
        {
            _logger.LogError(ex, "ElementExistsException");
            return ClientOperationResponse.BadRequest(ex.Message);
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ClientOperationResponse.BadRequest(
                $"Error while working with data storage: {ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ClientOperationResponse.InternalServerError(ex.Message);
        }
    }

    public ClientOperationResponse ChangeClientInfo(ClientBindingModel clientModel)
    {
        try
        {
            _clientBusinessLogicContract.UpdateClient(_mapper.Map<ClientDataModel>(clientModel));
            return ClientOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return ClientOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return ClientOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException");
            return ClientOperationResponse.BadRequest(
                $"Not found element by Id {clientModel.Id}"
            );
        }
        catch (ElementExistsException ex)
        {
            _logger.LogError(ex, "ElementExistsException");
            return ClientOperationResponse.BadRequest(ex.Message);
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ClientOperationResponse.BadRequest(
                $"Error while working with data storage: {ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ClientOperationResponse.InternalServerError(ex.Message);
        }
    }

    public ClientOperationResponse GetListByClerk(string clerkId)
    {
        try
        {
            return ClientOperationResponse.OK(
                [
                    .. _clientBusinessLogicContract
                        .GetClientByClerk(clerkId)
                        .Select(x => _mapper.Map<ClientViewModel>(x)),
                ]
            );
        }
        catch (NullListException)
        {
            _logger.LogError("NullListException");
            return ClientOperationResponse.NotFound("The list is not initialized");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ClientOperationResponse.InternalServerError(
                $"Error while working with data storage:{ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ClientOperationResponse.InternalServerError(ex.Message);
        }
    }
}
