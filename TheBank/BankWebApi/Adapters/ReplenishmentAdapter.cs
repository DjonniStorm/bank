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

public class ReplenishmentAdapter : IReplenishmentAdapter
{
    private readonly IReplenishmentBusinessLogicContract _replenishmentBusinessLogicContract;

    private readonly ILogger _logger;

    private readonly Mapper _mapper;

    public ReplenishmentAdapter(IReplenishmentBusinessLogicContract replenishmentBusinessLogicContract, ILogger logger)
    {
        _replenishmentBusinessLogicContract = replenishmentBusinessLogicContract;
        _logger = logger;
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ReplenishmentBindingModel, ReplenishmentDataModel>();
            cfg.CreateMap<ReplenishmentDataModel, ReplenishmentViewModel>();
        });
        _mapper = new Mapper(config);
    }
    
    public ReplenishmentOperationResponse GetList()
    {
        try
        {
            return ReplenishmentOperationResponse.OK(
                [
                    .. _replenishmentBusinessLogicContract
                        .GetAllReplenishments()
                        .Select(x => _mapper.Map<ReplenishmentViewModel>(x)),
                ]
            );
        }
        catch (NullListException)
        {
            _logger.LogError("NullListException");
            return ReplenishmentOperationResponse.NotFound("The list is not initialized");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ReplenishmentOperationResponse.InternalServerError(
                $"Error while working with data storage:{ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ReplenishmentOperationResponse.InternalServerError(ex.Message);
        }
    }
    
    public ReplenishmentOperationResponse GetElement(string data)
    {
        try
        {
            return ReplenishmentOperationResponse.OK(
                _mapper.Map<ReplenishmentViewModel>(_replenishmentBusinessLogicContract.GetReplenishmentByData(data))
            );
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return ReplenishmentOperationResponse.BadRequest("Data is empty");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException");
            return ReplenishmentOperationResponse.NotFound($"Not found element by data {data}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ReplenishmentOperationResponse.InternalServerError(
                $"Error while working with data storage: {ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ReplenishmentOperationResponse.InternalServerError(ex.Message);
        }
    }
    
    public ReplenishmentOperationResponse GetListByClerk(string clerkId)
    {
        try
        {
            return ReplenishmentOperationResponse.OK(
                [
                    .. _replenishmentBusinessLogicContract
                        .GetAllReplenishmentsByClerk(clerkId)
                        .Select(x => _mapper.Map<ReplenishmentViewModel>(x)),
                ]
            );
        }
        catch (NullListException)
        {
            _logger.LogError("NullListException");
            return ReplenishmentOperationResponse.NotFound("The list is not initialized");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ReplenishmentOperationResponse.InternalServerError(
                $"Error while working with data storage:{ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ReplenishmentOperationResponse.InternalServerError(ex.Message);
        }
    }
    
    public ReplenishmentOperationResponse GetListByDeposit(string depositId)
    {
        try
        {
            return ReplenishmentOperationResponse.OK(
                [
                    .. _replenishmentBusinessLogicContract
                        .GetAllReplenishmentsByDeposit(depositId)
                        .Select(x => _mapper.Map<ReplenishmentViewModel>(x)),
                ]
            );
        }
        catch (NullListException)
        {
            _logger.LogError("NullListException");
            return ReplenishmentOperationResponse.NotFound("The list is not initialized");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ReplenishmentOperationResponse.InternalServerError(
                $"Error while working with data storage:{ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ReplenishmentOperationResponse.InternalServerError(ex.Message);
        }
    }
    
    public ReplenishmentOperationResponse GetListByDate(DateTime fromDate, DateTime toDate)
    {
        try
        {
            return ReplenishmentOperationResponse.OK(
                [
                    .. _replenishmentBusinessLogicContract
                        .GetAllReplenishmentsByDate(fromDate, toDate)
                        .Select(x => _mapper.Map<ReplenishmentViewModel>(x)),
                ]
            );
        }
        catch (NullListException)
        {
            _logger.LogError("NullListException");
            return ReplenishmentOperationResponse.NotFound("The list is not initialized");
        }
        catch (IncorrectDatesException)
        {
            _logger.LogError("IncorrectDatesException");
            return ReplenishmentOperationResponse.BadRequest("Incorrect dates");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ReplenishmentOperationResponse.InternalServerError(
                $"Error while working with data storage:{ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ReplenishmentOperationResponse.InternalServerError(ex.Message);
        }
    }
    
    public ReplenishmentOperationResponse RegisterReplenishment(ReplenishmentBindingModel replenishmentModel)
    {
        try
        {
            _replenishmentBusinessLogicContract.InsertReplenishment(_mapper.Map<ReplenishmentDataModel>(replenishmentModel));
            return ReplenishmentOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return ReplenishmentOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return ReplenishmentOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (ElementExistsException ex)
        {
            _logger.LogError(ex, "ElementExistsException");
            return ReplenishmentOperationResponse.BadRequest(ex.Message);
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ReplenishmentOperationResponse.BadRequest(
                $"Error while working with data storage: {ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ReplenishmentOperationResponse.InternalServerError(ex.Message);
        }
    }

    public ReplenishmentOperationResponse ChangeReplenishmentInfo(ReplenishmentBindingModel replenishmentModel)
    {
        try
        {
            _replenishmentBusinessLogicContract.UpdateReplenishment(_mapper.Map<ReplenishmentDataModel>(replenishmentModel));
            return ReplenishmentOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return ReplenishmentOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return ReplenishmentOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException");
            return ReplenishmentOperationResponse.BadRequest(
                $"Not found element by Id {replenishmentModel.Id}"
            );
        }
        catch (ElementExistsException ex)
        {
            _logger.LogError(ex, "ElementExistsException");
            return ReplenishmentOperationResponse.BadRequest(ex.Message);
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ReplenishmentOperationResponse.BadRequest(
                $"Error while working with data storage: {ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ReplenishmentOperationResponse.InternalServerError(ex.Message);
        }
    }
}
