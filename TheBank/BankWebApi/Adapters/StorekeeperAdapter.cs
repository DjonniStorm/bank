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

public class StorekeeperAdapter : IStorekeeperAdapter
{
    private readonly IStorekeeperBusinessLogicContract _storekeeperBusinessLogicContract;

    private readonly ILogger _logger;

    private readonly Mapper _mapper;

    public StorekeeperAdapter(IStorekeeperBusinessLogicContract storekeeperBusinessLogicContract, ILogger logger)
    {
        _storekeeperBusinessLogicContract = storekeeperBusinessLogicContract;
        _logger = logger;
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<StorekeeperBindingModel, StorekeeperDataModel>();
            cfg.CreateMap<StorekeeperDataModel, StorekeeperViewModel>();
        });
        _mapper = new Mapper(config);
    }
    
    public StorekeeperOperationResponse GetList()
    {
        try
        {
            return StorekeeperOperationResponse.OK(
                [
                    .. _storekeeperBusinessLogicContract
                        .GetAllStorekeepers()
                        .Select(x => _mapper.Map<StorekeeperViewModel>(x)),
                ]
            );
        }
        catch (NullListException)
        {
            _logger.LogError("NullListException");
            return StorekeeperOperationResponse.NotFound("The list is not initialized");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return StorekeeperOperationResponse.InternalServerError(
                $"Error while working with data storage:{ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return StorekeeperOperationResponse.InternalServerError(ex.Message);
        }
    }
    
    public StorekeeperOperationResponse GetElement(string data)
    {
        try
        {
            return StorekeeperOperationResponse.OK(
                _mapper.Map<StorekeeperViewModel>(_storekeeperBusinessLogicContract.GetStorekeeperByData(data))
            );
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return StorekeeperOperationResponse.BadRequest("Data is empty");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException");
            return StorekeeperOperationResponse.NotFound($"Not found element by data {data}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return StorekeeperOperationResponse.InternalServerError(
                $"Error while working with data storage: {ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return StorekeeperOperationResponse.InternalServerError(ex.Message);
        }
    }
    
    public StorekeeperOperationResponse RegisterStorekeeper(StorekeeperBindingModel storekeeperModel)
    {
        try
        {
            _storekeeperBusinessLogicContract.InsertStorekeeper(_mapper.Map<StorekeeperDataModel>(storekeeperModel));
            return StorekeeperOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return StorekeeperOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return StorekeeperOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (ElementExistsException ex)
        {
            _logger.LogError(ex, "ElementExistsException");
            return StorekeeperOperationResponse.BadRequest(ex.Message);
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return StorekeeperOperationResponse.BadRequest(
                $"Error while working with data storage: {ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return StorekeeperOperationResponse.InternalServerError(ex.Message);
        }
    }

    public StorekeeperOperationResponse ChangeStorekeeperInfo(StorekeeperBindingModel storekeeperModel)
    {
        try
        {
            _storekeeperBusinessLogicContract.UpdateStorekeeper(_mapper.Map<StorekeeperDataModel>(storekeeperModel));
            return StorekeeperOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return StorekeeperOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return StorekeeperOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException");
            return StorekeeperOperationResponse.BadRequest(
                $"Not found element by Id {storekeeperModel.Id}"
            );
        }
        catch (ElementExistsException ex)
        {
            _logger.LogError(ex, "ElementExistsException");
            return StorekeeperOperationResponse.BadRequest(ex.Message);
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return StorekeeperOperationResponse.BadRequest(
                $"Error while working with data storage: {ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return StorekeeperOperationResponse.InternalServerError(ex.Message);
        }
    } 
}
