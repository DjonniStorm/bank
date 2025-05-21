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
            // DepositBindingModel -> DepositDataModel
            cfg.CreateMap<DepositBindingModel, DepositDataModel>()
       .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id ?? string.Empty))
       .ForMember(dest => dest.InterestRate, opt => opt.MapFrom(src => src.InterestRate))
       .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.Cost))
       .ForMember(dest => dest.Period, opt => opt.MapFrom(src => src.Period))
       .ForMember(dest => dest.ClerkId, opt => opt.MapFrom(src => src.ClerkId ?? string.Empty))
       .ForMember(dest => dest.Currencies, opt => opt.MapFrom(src => src.DepositCurrencies ?? new List<DepositCurrencyBindingModel>()));

            // DepositDataModel -> DepositViewModel
            cfg.CreateMap<DepositDataModel, DepositViewModel>()
       .ForMember(dest => dest.DepositCurrencies, opt => opt.MapFrom(src => src.Currencies != null ? src.Currencies : new List<DepositCurrencyDataModel>()));

            // DepositCurrencyBindingModel -> DepositCurrencyDataModel
            cfg.CreateMap<DepositCurrencyBindingModel, DepositCurrencyDataModel>()
       .ForMember(dest => dest.DepositId, opt => opt.MapFrom(src => src.DepositId ?? string.Empty))
       .ForMember(dest => dest.CurrencyId, opt => opt.MapFrom(src => src.CurrencyId ?? string.Empty));

            // DepositCurrencyDataModel -> DepositCurrencyViewModel
            cfg.CreateMap<DepositCurrencyDataModel, DepositCurrencyViewModel>();

            // DepositCurrencyViewModel -> DepositCurrencyBindingModel
            cfg.CreateMap<DepositCurrencyViewModel, DepositCurrencyBindingModel>();

            // Явный маппинг DepositCurrencyDataModel -> DepositCurrencyBindingModel
            cfg.CreateMap<DepositCurrencyDataModel, DepositCurrencyBindingModel>()
       .ForMember(dest => dest.DepositId, opt => opt.MapFrom(src => src.DepositId))
       .ForMember(dest => dest.CurrencyId, opt => opt.MapFrom(src => src.CurrencyId));
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
                $"Error while working with data storage: {ex.InnerException?.Message}"
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
