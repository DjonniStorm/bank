using AutoMapper;
using BankContracts.AdapterContracts;
using BankContracts.AdapterContracts.OperationResponses;
using BankContracts.BindingModels;
using BankContracts.BusinessLogicContracts;
using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.ViewModels;

namespace BankWebApi.Adapters;

public class ClerkAdapter : IClerkAdapter
{
    private readonly IClerkBusinessLogicContract _clerkBusinessLogicContract;

    private readonly ILogger _logger;

    private readonly Mapper _mapper;

    public ClerkAdapter(IClerkBusinessLogicContract clerkBusinessLogicContract, ILogger logger)
    {
        _clerkBusinessLogicContract = clerkBusinessLogicContract;
        _logger = logger;
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ClerkBindingModel, ClerkDataModel>();
            cfg.CreateMap<ClerkDataModel, ClerkViewModel>();
        });
        _mapper = new Mapper(config);
    }

    public ClerkOperationResponse GetList()
    {
        try
        {
            return ClerkOperationResponse.OK(
                [
                    .. _clerkBusinessLogicContract
                        .GetAllClerks()
                        .Select(x => _mapper.Map<ClerkViewModel>(x)),
                ]
            );
        }
        catch (NullListException)
        {
            _logger.LogError("NullListException");
            return ClerkOperationResponse.NotFound("The list is not initialized");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ClerkOperationResponse.InternalServerError(
                $"Error while working with data storage:{ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ClerkOperationResponse.InternalServerError(ex.Message);
        }
    }

    public ClerkOperationResponse GetElement(string data)
    {
        try
        {
            return ClerkOperationResponse.OK(
                _mapper.Map<ClerkViewModel>(_clerkBusinessLogicContract.GetClerkByData(data))
            );
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return ClerkOperationResponse.BadRequest("Data is empty");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException");
            return ClerkOperationResponse.NotFound($"Not found element by data {data}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ClerkOperationResponse.InternalServerError(
                $"Error while working with data storage: {ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ClerkOperationResponse.InternalServerError(ex.Message);
        }
    }

    public ClerkOperationResponse RegisterClerk(ClerkBindingModel clerkModel)
    {
        try
        {
            _clerkBusinessLogicContract.InsertClerk(_mapper.Map<ClerkDataModel>(clerkModel));
            return ClerkOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return ClerkOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return ClerkOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (ElementExistsException ex)
        {
            _logger.LogError(ex, "ElementExistsException");
            return ClerkOperationResponse.BadRequest(ex.Message);
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ClerkOperationResponse.BadRequest(
                $"Error while working with data storage: {ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ClerkOperationResponse.InternalServerError(ex.Message);
        }
    }

    public ClerkOperationResponse ChangeClerkInfo(ClerkBindingModel clerkModel)
    {
        try
        {
            _clerkBusinessLogicContract.UpdateClerk(_mapper.Map<ClerkDataModel>(clerkModel));
            return ClerkOperationResponse.NoContent();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "ArgumentNullException");
            return ClerkOperationResponse.BadRequest("Data is empty");
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "ValidationException");
            return ClerkOperationResponse.BadRequest($"Incorrect data transmitted: {ex.Message}");
        }
        catch (ElementNotFoundException ex)
        {
            _logger.LogError(ex, "ElementNotFoundException");
            return ClerkOperationResponse.BadRequest(
                $"Not found element by Id {clerkModel.Id}"
            );
        }
        catch (ElementExistsException ex)
        {
            _logger.LogError(ex, "ElementExistsException");
            return ClerkOperationResponse.BadRequest(ex.Message);
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ClerkOperationResponse.BadRequest(
                $"Error while working with data storage: {ex.InnerException!.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ClerkOperationResponse.InternalServerError(ex.Message);
        }
    }
}
