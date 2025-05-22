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

public class CreditProgramAdapter : ICreditProgramAdapter
{
    private readonly ICreditProgramBusinessLogicContract _creditProgramBusinessLogicContract;

    private readonly ILogger _logger;

    private readonly Mapper _mapper;

    public CreditProgramAdapter(ICreditProgramBusinessLogicContract creditProgramBusinessLogicContract, ILogger logger)
    {
        _creditProgramBusinessLogicContract = creditProgramBusinessLogicContract;
        _logger = logger;
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreditProgramBindingModel, CreditProgramDataModel>();
            cfg.CreateMap<CreditProgramDataModel, CreditProgramViewModel>();
            cfg.CreateMap<CreditProgramCurrencyBindingModel, CreditProgramCurrencyDataModel>();
            cfg.CreateMap<CreditProgramCurrencyDataModel, CreditProgramCurrencyViewModel>();
        });
        _mapper = new Mapper(config);
    }

    public CreditProgramOperationResponse GetList()
    {
        try
        {
            return CreditProgramOperationResponse.OK(
                [
                    .. _creditProgramBusinessLogicContract
                        .GetAllCreditPrograms()
                        .Select(x => _mapper.Map<CreditProgramViewModel>(x)),
                ]
            );
        }
        catch (NullListException)
        {
            _logger.LogError("NullListException");
            return CreditProgramOperationResponse.NotFound("The list is not initialized");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return CreditProgramOperationResponse.InternalServerError(
                $"Error while working with data storage:{ex.InnerException?.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return CreditProgramOperationResponse.InternalServerError(ex.Message);
        }
    }
    
    public CreditProgramOperationResponse GetElement(string data)
    {
        try
        {
            return CreditProgramOperationResponse.OK(
                _mapper.Map<CreditProgramViewModel>(_creditProgramBusinessLogicContract.GetCreditProgramByData(data))
            );
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return CreditProgramOperationResponse.BadRequest("Data is empty");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException");
            return CreditProgramOperationResponse.NotFound($"Not found element by data {data}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return CreditProgramOperationResponse.InternalServerError(
                $"Error while working with data storage: {ex.InnerException?.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return CreditProgramOperationResponse.InternalServerError(ex.Message);
        }
    }
    
    public CreditProgramOperationResponse RegisterCreditProgram(CreditProgramBindingModel creditProgramModel)
    {
        try
        {
            _creditProgramBusinessLogicContract.InsertCreditProgram(_mapper.Map<CreditProgramDataModel>(creditProgramModel));
            return CreditProgramOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return CreditProgramOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return CreditProgramOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (ElementExistsException ex)
        {
            _logger.LogError(ex, "ElementExistsException");
            return CreditProgramOperationResponse.BadRequest(ex.Message);
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return CreditProgramOperationResponse.BadRequest(
                $"Error while working with data storage: {ex.InnerException?.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return CreditProgramOperationResponse.InternalServerError(ex.Message);
        }
    }
    public CreditProgramOperationResponse ChangeCreditProgramInfo(CreditProgramBindingModel creditProgramModel)
    {
        try
        {
            _creditProgramBusinessLogicContract.UpdateCreditProgram(_mapper.Map<CreditProgramDataModel>(creditProgramModel));
            return CreditProgramOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return CreditProgramOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return CreditProgramOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException");
            return CreditProgramOperationResponse.BadRequest(
                $"Not found element by Id {creditProgramModel.Id}"
            );
        }
        catch (ElementExistsException ex)
        {
            _logger.LogError(ex, "ElementExistsException");
            return CreditProgramOperationResponse.BadRequest(ex.Message);
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return CreditProgramOperationResponse.BadRequest(
                $"Error while working with data storage: {ex.InnerException?.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return CreditProgramOperationResponse.InternalServerError(ex.Message);
        }
    }

    public CreditProgramOperationResponse GetListByPeriod(string periodId)
    {
        try
        {
            return CreditProgramOperationResponse.OK(
                [
                    .. _creditProgramBusinessLogicContract
                        .GetCreditProgramByPeriod(periodId)
                        .Select(x => _mapper.Map<CreditProgramViewModel>(x)),
                ]
            );
        }
        catch (NullListException)
        {
            _logger.LogError("NullListException");
            return CreditProgramOperationResponse.NotFound("The list is not initialized");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return CreditProgramOperationResponse.InternalServerError(
                $"Error while working with data storage:{ex.InnerException?.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return CreditProgramOperationResponse.InternalServerError(ex.Message);
        }
    }

    public CreditProgramOperationResponse GetListByStorekeeper(string storekeeperId)
    {
        try
        {
            return CreditProgramOperationResponse.OK(
                [
                    .. _creditProgramBusinessLogicContract
                        .GetCreditProgramByStorekeeper(storekeeperId)
                        .Select(x => _mapper.Map<CreditProgramViewModel>(x)),
                ]
            );
        }
        catch (NullListException)
        {
            _logger.LogError("NullListException");
            return CreditProgramOperationResponse.NotFound("The list is not initialized");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return CreditProgramOperationResponse.InternalServerError(
                $"Error while working with data storage:{ex.InnerException?.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return CreditProgramOperationResponse.InternalServerError(ex.Message);
        }
    }  
}
