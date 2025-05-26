using AutoMapper;
using BankContracts.AdapterContracts;
using BankContracts.AdapterContracts.OperationResponses;
using BankContracts.BusinessLogicContracts;
using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.ViewModels;

namespace BankWebApi.Adapters;

public class ReportAdapter : IReportAdapter
{
    private readonly IReportContract _reportContract;

    private readonly ILogger _logger;

    private readonly Mapper _mapper;

    public ReportAdapter(IReportContract reportContract, ILogger logger)
    {
        _reportContract = reportContract;
        _logger = logger;
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ClientsByCreditProgramDataModel, ClientsByCreditProgramViewModel>();
            cfg.CreateMap<ClientsByDepositDataModel, ClientsByDepositViewModel>();
            cfg.CreateMap<DepositByCreditProgramDataModel, DepositByCreditProgramViewModel>();
            cfg.CreateMap<CreditProgramAndDepositByCurrencyDataModel, CreditProgramAndDepositByCurrencyViewModel>();
        });
        _mapper = new Mapper(config);
    }

    private static ReportOperationResponse SendStream(Stream stream, string fileName)
    {
        stream.Position = 0;
        return ReportOperationResponse.OK(stream, fileName);
    }

    public async Task<ReportOperationResponse> CreateDocumentClientsByCreditProgramAsync(List<string>? creditProgramIds, CancellationToken ct)
    {
        try
        {
            return SendStream(await _reportContract.CreateDocumentClientsByCreditProgramAsync(creditProgramIds, ct),
                "clientsbycreditprogram.docx");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "InvalidOperationException");
            return ReportOperationResponse.InternalServerError($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ReportOperationResponse.InternalServerError($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ReportOperationResponse.InternalServerError(ex.Message);
        }
    }

    public async Task<ReportOperationResponse> CreateExcelDocumentClientsByCreditProgramAsync(List<string>? creditProgramIds, CancellationToken ct)
    {
        try
        {
            return SendStream(await _reportContract.CreateExcelDocumentClientsByCreditProgramAsync(creditProgramIds, ct),
                "clientsbycreditprogram.xlsx");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "InvalidOperationException");
            return ReportOperationResponse.InternalServerError($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ReportOperationResponse.InternalServerError($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ReportOperationResponse.InternalServerError(ex.Message);
        }
    }

    public async Task<ReportOperationResponse> CreateDocumentClientsByDepositAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct)
    {
        try
        {
            return SendStream(await _reportContract.CreateDocumentClientsByDepositAsync(dateStart, dateFinish, ct), "clientbydeposit.pdf");
        }
        catch (IncorrectDatesException ex)
        {
            _logger.LogError(ex, "IncorrectDatesException");
            return ReportOperationResponse.BadRequest($"Incorrect dates: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "InvalidOperationException");
            return ReportOperationResponse.InternalServerError($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ReportOperationResponse.InternalServerError($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return
            ReportOperationResponse.InternalServerError(ex.Message);
        }
    }

    public async Task<ReportOperationResponse> CreateDocumentDepositAndCreditProgramByCurrencyAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct)
    {
        try
        {
            return SendStream(await _reportContract.CreateDocumentDepositAndCreditProgramByCurrencyAsync(dateStart, dateFinish, ct),
            "depositandcreditprogrambycurrency.pdf");
        }
        catch (IncorrectDatesException ex)
        {
            _logger.LogError(ex, "IncorrectDatesException");
            return ReportOperationResponse.BadRequest($"Incorrect dates: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "InvalidOperationException");
            return ReportOperationResponse.InternalServerError($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ReportOperationResponse.InternalServerError($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return
            ReportOperationResponse.InternalServerError(ex.Message);
        }
    }

    public async Task<ReportOperationResponse> CreateDocumentDepositByCreditProgramAsync(List<string>? creditProgramIds, CancellationToken ct)
    {
        try
        {
            return SendStream(await _reportContract.CreateDocumentDepositByCreditProgramAsync(creditProgramIds, ct), "depositbycreditprogram.docx");
        }
        catch (IncorrectDatesException ex)
        {
            _logger.LogError(ex, "IncorrectDatesException");
            return ReportOperationResponse.BadRequest($"Incorrect dates: {ex.Message}");
        }
        catch (InvalidOperationException ex) when (ex.Message == "No deposits with currencies found")
        {
            _logger.LogError(ex, "InvalidOperationException");
            return ReportOperationResponse.BadRequest("No deposits with currencies found");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "InvalidOperationException");
            return ReportOperationResponse.InternalServerError($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ReportOperationResponse.InternalServerError($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ReportOperationResponse.InternalServerError(ex.Message);
        }
    }

    public async Task<ReportOperationResponse> CreateExcelDocumentDepositByCreditProgramAsync(List<string>? creditProgramIds, CancellationToken ct)
    {
        try
        {
            return SendStream(await _reportContract.CreateExcelDocumentDepositByCreditProgramAsync(creditProgramIds, ct), "depositbycreditprogram.xlsx");
        }
        catch (IncorrectDatesException ex)
        {
            _logger.LogError(ex, "IncorrectDatesException");
            return ReportOperationResponse.BadRequest($"Incorrect dates: {ex.Message}");
        }
        catch (InvalidOperationException ex) when (ex.Message == "No deposits with currencies found")
        {
            _logger.LogError(ex, "InvalidOperationException");
            return ReportOperationResponse.BadRequest("No deposits with currencies found");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "InvalidOperationException");
            return ReportOperationResponse.InternalServerError($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ReportOperationResponse.InternalServerError($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ReportOperationResponse.InternalServerError(ex.Message);
        }
    }

    public async Task<ReportOperationResponse> GetDataClientsByCreditProgramAsync(List<string>? creditProgramIds, CancellationToken ct)
    {
        try
        {
            return ReportOperationResponse.OK((await _reportContract.GetDataClientsByCreditProgramAsync(creditProgramIds, ct))
                .Select(x => _mapper.Map<ClientsByCreditProgramViewModel>(x)).ToList());
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "InvalidOperationException");
            return ReportOperationResponse.InternalServerError($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ReportOperationResponse.InternalServerError($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ReportOperationResponse.InternalServerError(ex.Message);
        }
    }

    public async Task<ReportOperationResponse> GetDataClientsByDepositAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct)
    {
        try
        {
            return ReportOperationResponse.OK((await _reportContract.GetDataClientsByDepositAsync(dateStart, dateFinish, ct))
                .Select(x => _mapper.Map<ClientsByDepositViewModel>(x)).ToList());
        }
        catch (IncorrectDatesException ex)
        {
            _logger.LogError(ex, "IncorrectDatesException");
            return ReportOperationResponse.BadRequest($"Incorrect dates: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "InvalidOperationException");
            return ReportOperationResponse.InternalServerError($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ReportOperationResponse.InternalServerError($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return
            ReportOperationResponse.InternalServerError(ex.Message);
        }
    }

    public async Task<ReportOperationResponse> GetDataDepositAndCreditProgramByCurrencyAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct)
    {
        try
        {
            return ReportOperationResponse.OK((await _reportContract.GetDataDepositAndCreditProgramByCurrencyAsync(dateStart, dateFinish, ct))
                .Select(x => _mapper.Map<CreditProgramAndDepositByCurrencyViewModel>(x)).ToList());
        }
        catch (IncorrectDatesException ex)
        {
            _logger.LogError(ex, "IncorrectDatesException");
            return ReportOperationResponse.BadRequest($"Incorrect dates: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "InvalidOperationException");
            return ReportOperationResponse.InternalServerError($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ReportOperationResponse.InternalServerError($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return
            ReportOperationResponse.InternalServerError(ex.Message);
        }
    }

    public async Task<ReportOperationResponse> GetDataDepositByCreditProgramAsync(List<string>? creditProgramIds, CancellationToken ct)
    {
        try
        {
            return ReportOperationResponse.OK([.. (await _reportContract.GetDataDepositByCreditProgramAsync(creditProgramIds, ct)).Select(x => _mapper.Map<DepositByCreditProgramViewModel>(x))]);
        }
        catch (IncorrectDatesException ex)
        {
            _logger.LogError(ex, "IncorrectDatesException");
            return ReportOperationResponse.BadRequest($"Incorrect dates: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "InvalidOperationException");
            return ReportOperationResponse.InternalServerError($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (StorageException ex)
        {
            _logger.LogError(ex, "StorageException");
            return ReportOperationResponse.InternalServerError($"Error while working with data storage: {ex.InnerException!.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            return ReportOperationResponse.InternalServerError(ex.Message);
        }
    }
}
