using AutoMapper;
using BankContracts.AdapterContracts;
using BankContracts.AdapterContracts.OperationResponses;
using BankContracts.BindingModels;
using BankContracts.BusinessLogicContracts;
using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.ViewModels;

namespace BankWebApi.Adapters;

public class PeriodAdapter : IPeriodAdapter
{
    private readonly IPeriodBusinessLogicContract _periodBusinessLogicContract;

    private readonly ILogger _logger;

    private readonly Mapper _mapper;

    public PeriodAdapter(IPeriodBusinessLogicContract periodBusinessLogicContract, ILogger logger)
    {
        _periodBusinessLogicContract = periodBusinessLogicContract;
        _logger = logger;
        var config = new MapperConfiguration(cfg => 
        {
            cfg.CreateMap<PeriodBindingModel, PeriodDataModel>();
            cfg.CreateMap<PeriodDataModel, PeriodViewModel>();
        });
        _mapper = new Mapper(config);
    }

    public PeriodOperationResponse GetList()
    {
        try
        {
            return PeriodOperationResponse.OK(
                [
                    .. _periodBusinessLogicContract
                        .GetAllPeriods()
                        .Select(x => _mapper.Map<PeriodViewModel>(x)),
                ]
            );
        }
        catch (NullListException)
        {
            _logger.LogError("NullListException");
            return PeriodOperationResponse.NotFound("The list is not initialized");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return PeriodOperationResponse.InternalServerError(
                $"Error while working with data storage:{ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return PeriodOperationResponse.InternalServerError(ex.Message);
        }
    }
    public PeriodOperationResponse GetElement(string data)
    {
        try
        {
            return PeriodOperationResponse.OK(
                _mapper.Map<PeriodViewModel>(_periodBusinessLogicContract.GetPeriodByData(data))
            );
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return PeriodOperationResponse.BadRequest("Data is empty");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException");
            return PeriodOperationResponse.NotFound($"Not found element by data {data}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return PeriodOperationResponse.InternalServerError(
                $"Error while working with data storage: {ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return PeriodOperationResponse.InternalServerError(ex.Message);
        }
    }

    public PeriodOperationResponse GetListByStorekeeper(string storekeeperId)
    {
        try
        {
            return PeriodOperationResponse.OK(
                [
                    .. _periodBusinessLogicContract
                        .GetAllPeriodsByStorekeeper(storekeeperId)
                        .Select(x => _mapper.Map<PeriodViewModel>(x)),
                ]
            );
        }
        catch (NullListException)
        {
            _logger.LogError("NullListException");
            return PeriodOperationResponse.NotFound("The list is not initialized");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return PeriodOperationResponse.InternalServerError(
                $"Error while working with data storage:{ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return PeriodOperationResponse.InternalServerError(ex.Message);
        }
    }

    public PeriodOperationResponse GetListByStartTime(DateTime fromDate)
    {
        try
        {
            return PeriodOperationResponse.OK(
                [
                    .. _periodBusinessLogicContract
                        .GetAllPeriodsByStartTime(fromDate)
                        .Select(x => _mapper.Map<PeriodViewModel>(x)),
                ]
            );
        }
        catch (NullListException)
        {
            _logger.LogError("NullListException");
            return PeriodOperationResponse.NotFound("The list is not initialized");
        }
        catch (IncorrectDatesException)
        {
            _logger.LogError("IncorrectDatesException");
            return PeriodOperationResponse.BadRequest("Incorrect dates");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return PeriodOperationResponse.InternalServerError(
                $"Error while working with data storage:{ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return PeriodOperationResponse.InternalServerError(ex.Message);
        }
    }

    public PeriodOperationResponse GetListByEndTime(DateTime toDate)
    {
        try
        {
            return PeriodOperationResponse.OK(
                [
                    .. _periodBusinessLogicContract
                        .GetAllPeriodsByStartTime(toDate)
                        .Select(x => _mapper.Map<PeriodViewModel>(x)),
                ]
            );
        }
        catch (NullListException)
        {
            _logger.LogError("NullListException");
            return PeriodOperationResponse.NotFound("The list is not initialized");
        }
        catch (IncorrectDatesException)
        {
            _logger.LogError("IncorrectDatesException");
            return PeriodOperationResponse.BadRequest("Incorrect dates");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return PeriodOperationResponse.InternalServerError(
                $"Error while working with data storage:{ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return PeriodOperationResponse.InternalServerError(ex.Message);
        }
    }

    public PeriodOperationResponse RegisterPeriod(PeriodBindingModel periodModel)
    {
        try
        {
            _periodBusinessLogicContract.InsertPeriod(_mapper.Map<PeriodDataModel>(periodModel));
            return PeriodOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return PeriodOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return PeriodOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (ElementExistsException ex)
        {
            _logger.LogError(ex, "ElementExistsException");
            return PeriodOperationResponse.BadRequest(ex.Message);
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return PeriodOperationResponse.BadRequest(
                $"Error while working with data storage: {ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return PeriodOperationResponse.InternalServerError(ex.Message);
        }
    }

    public PeriodOperationResponse ChangePeriodInfo(PeriodBindingModel periodModel)
    {
        try
        {
            _periodBusinessLogicContract.UpdatePeriod(_mapper.Map<PeriodDataModel>(periodModel));
            return PeriodOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return PeriodOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return PeriodOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException");
            return PeriodOperationResponse.BadRequest(
                $"Not found element by Id {periodModel.Id}"
            );
        }
        catch (ElementExistsException ex)
        {
            _logger.LogError(ex, "ElementExistsException");
            return PeriodOperationResponse.BadRequest(ex.Message);
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return PeriodOperationResponse.BadRequest(
                $"Error while working with data storage: {ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return PeriodOperationResponse.InternalServerError(ex.Message);
        }
    }
}
