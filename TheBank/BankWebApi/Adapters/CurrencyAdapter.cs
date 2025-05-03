using AutoMapper;
using BankBusinessLogic.Implementations;
using BankContracts.AdapterContracts;
using BankContracts.AdapterContracts.OperationResponses;
using BankContracts.BindingModels;
using BankContracts.BusinessLogicContracts;
using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.ViewModels;
using BankDatabase.Models;

namespace BankWebApi.Adapters;

public class CurrencyAdapter : ICurrencyAdapter
{
    private readonly ICurrencyBusinessLogicContract _currencyBusinessLogicContract;

    private readonly ILogger _logger;

    private readonly Mapper _mapper;

    public CurrencyAdapter(ICurrencyBusinessLogicContract currencyBusinessLogicContract, ILogger logger)
    {
        _currencyBusinessLogicContract = currencyBusinessLogicContract;
        _logger = logger;
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CurrencyBindingModel, CurrencyDataModel>();
            cfg.CreateMap<CurrencyDataModel, CurrencyViewModel>();
        });
        _mapper = new Mapper(config);
    }
    
    public CurrencyOperationResponse GetList()
    {
        try
        {
            return CurrencyOperationResponse.OK(
                [
                    .. _currencyBusinessLogicContract
                        .GetAllCurrencies()
                        .Select(x => _mapper.Map<CurrencyViewModel>(x)),
                ]
            );
        }
        catch (NullListException)
        {
            _logger.LogError("NullListException");
            return CurrencyOperationResponse.NotFound("The list is not initialized");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return CurrencyOperationResponse.InternalServerError(
                $"Error while working with data storage:{ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return CurrencyOperationResponse.InternalServerError(ex.Message);
        }
    }
    
    public CurrencyOperationResponse GetElement(string data)
    {
        try
        {
            return CurrencyOperationResponse.OK(
                _mapper.Map<CurrencyViewModel>(_currencyBusinessLogicContract.GetCurrencyByData(data))
            );
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return CurrencyOperationResponse.BadRequest("Data is empty");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException");
            return CurrencyOperationResponse.NotFound($"Not found element by data {data}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return CurrencyOperationResponse.InternalServerError(
                $"Error while working with data storage: {ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return CurrencyOperationResponse.InternalServerError(ex.Message);
        }
    }
    
    public CurrencyOperationResponse MakeCurrency(CurrencyBindingModel currencyModel)
    {
        try
        {
            _currencyBusinessLogicContract.InsertCurrency(_mapper.Map<CurrencyDataModel>(currencyModel));
            return CurrencyOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return CurrencyOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return CurrencyOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (ElementExistsException ex)
        {
            _logger.LogError(ex, "ElementExistsException");
            return CurrencyOperationResponse.BadRequest(ex.Message);
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return CurrencyOperationResponse.BadRequest(
                $"Error while working with data storage: {ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return CurrencyOperationResponse.InternalServerError(ex.Message);
        }
    }

    public CurrencyOperationResponse ChangeCurrencyInfo(CurrencyBindingModel currencyModel)
    {
        try
        {
            _currencyBusinessLogicContract.UpdateCurrency(_mapper.Map<CurrencyDataModel>(currencyModel));
            return CurrencyOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return CurrencyOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return CurrencyOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException");
            return CurrencyOperationResponse.BadRequest(
                $"Not found element by Id {currencyModel.Id}"
            );
        }
        catch (ElementExistsException ex)
        {
            _logger.LogError(ex, "ElementExistsException");
            return CurrencyOperationResponse.BadRequest(ex.Message);
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return CurrencyOperationResponse.BadRequest(
                $"Error while working with data storage: {ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return CurrencyOperationResponse.InternalServerError(ex.Message);
        }
    }

    public CurrencyOperationResponse GetListByStorekeeper(string storekeeperId)
    {
        try
        {
            return CurrencyOperationResponse.OK(
                [
                    .. _currencyBusinessLogicContract
                        .GetCurrencyByStorekeeper(storekeeperId)
                        .Select(x => _mapper.Map<CurrencyViewModel>(x)),
                ]
            );
        }
        catch (NullListException)
        {
            _logger.LogError("NullListException");
            return CurrencyOperationResponse.NotFound("The list is not initialized");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return CurrencyOperationResponse.InternalServerError(
                $"Error while working with data storage:{ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return CurrencyOperationResponse.InternalServerError(ex.Message);
        }
    }
}
