using AutoMapper;
using BankContracts.AdapterContracts;
using BankContracts.AdapterContracts.OperationResponses;
using BankContracts.BindingModels;
using BankContracts.BusinessLogicContracts;
using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.ViewModels;

namespace BankWebApi.Adapters;

public class DepositAdapter : IDepositAdapter
{
    private readonly IDepositBusinessLogicContract _depositBusinessLogicContract;

    private readonly ILogger _logger;

    private readonly Mapper _mapper;

    public DepositAdapter(IDepositBusinessLogicContract depositBusinessLogicContract, ILogger logger)
    {
        _depositBusinessLogicContract = depositBusinessLogicContract;
        _logger = logger;
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<DepositBindingModel, DepositDataModel>();
            cfg.CreateMap<DepositDataModel, DepositViewModel>();
            cfg.CreateMap<DepositCurrencyBindingModel, DepositCurrencyDataModel>();
            cfg.CreateMap<DepositCurrencyDataModel, DepositCurrencyViewModel>();
        });
        _mapper = new Mapper(config);
    }
    public DepositOperationResponse GetList()
    {
        try
        {
            return DepositOperationResponse.OK(
                [
                    .. _depositBusinessLogicContract
                        .GetAllDeposits()
                        .Select(x => _mapper.Map<DepositViewModel>(x)),
                ]
            );
        }
        catch (NullListException)
        {
            _logger.LogError("NullListException");
            return DepositOperationResponse.NotFound("The list is not initialized");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return DepositOperationResponse.InternalServerError(
                $"Error while working with data storage:{ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return DepositOperationResponse.InternalServerError(ex.Message);
        }
    }
    public DepositOperationResponse GetElement(string data)
    {
        try
        {
            return DepositOperationResponse.OK(
                _mapper.Map<DepositViewModel>(_depositBusinessLogicContract.GetDepositByData(data))
            );
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return DepositOperationResponse.BadRequest("Data is empty");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException");
            return DepositOperationResponse.NotFound($"Not found element by data {data}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return DepositOperationResponse.InternalServerError(
                $"Error while working with data storage: {ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return DepositOperationResponse.InternalServerError(ex.Message);
        }
    }
    public DepositOperationResponse MakeDeposit(DepositBindingModel depositModel)
    {
        try
        {
            _depositBusinessLogicContract.InsertDeposit(_mapper.Map<DepositDataModel>(depositModel));
            return DepositOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return DepositOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return DepositOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (ElementExistsException ex)
        {
            _logger.LogError(ex, "ElementExistsException");
            return DepositOperationResponse.BadRequest(ex.Message);
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return DepositOperationResponse.BadRequest(
                $"Error while working with data storage: {ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return DepositOperationResponse.InternalServerError(ex.Message);
        }
    }

    public DepositOperationResponse ChangeDepositInfo(DepositBindingModel depositModel)
    {
        try
        {
            _depositBusinessLogicContract.UpdateDeposit(_mapper.Map<DepositDataModel>(depositModel));
            return DepositOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return DepositOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return DepositOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException");
            return DepositOperationResponse.BadRequest(
                $"Not found element by Id {depositModel.Id}"
            );
        }
        catch (ElementExistsException ex)
        {
            _logger.LogError(ex, "ElementExistsException");
            return DepositOperationResponse.BadRequest(ex.Message);
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return DepositOperationResponse.BadRequest(
                $"Error while working with data storage: {ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return DepositOperationResponse.InternalServerError(ex.Message);
        }
    }

    public DepositOperationResponse GetListByClerk(string clerkId)
    {
        try
        {
            return DepositOperationResponse.OK(
                [
                    .. _depositBusinessLogicContract
                        .GetDepositByClerk(clerkId)
                        .Select(x => _mapper.Map<DepositViewModel>(x)),
                ]
            );
        }
        catch (NullListException)
        {
            _logger.LogError("NullListException");
            return DepositOperationResponse.NotFound("The list is not initialized");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return DepositOperationResponse.InternalServerError(
                $"Error while working with data storage:{ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return DepositOperationResponse.InternalServerError(ex.Message);
        }
    }
}
