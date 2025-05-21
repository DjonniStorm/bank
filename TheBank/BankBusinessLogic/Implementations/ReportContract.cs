using BankBusinessLogic.OfficePackage;
using BankContracts.BusinessLogicContracts;
using BankContracts.DataModels;
using BankContracts.StorageContracts;
using Microsoft.Extensions.Logging;

namespace BankBusinessLogic.Implementations;

public class ReportContract(IClientStorageContract clientStorage, ICurrencyStorageContract currencyStorage,
    ICreditProgramStorageContract creditProgramStorage, IDepositStorageContract depositStorage,
    BaseWordBuilder baseWordBuilder, BaseExcelBuilder baseExcelBuilder, BasePdfBuilder basePdfBuilder, ILogger logger) : IReportContract
{
    private readonly IClientStorageContract _clientStorage = clientStorage;
    private readonly ICurrencyStorageContract _currencyStorage = currencyStorage;
    private readonly ICreditProgramStorageContract _creditProgramStorage = creditProgramStorage;
    private readonly IDepositStorageContract _depositStorage = depositStorage;
    private readonly BaseWordBuilder _baseWordBuilder = baseWordBuilder;
    private readonly BaseExcelBuilder _baseExcelBuilder = baseExcelBuilder;
    private readonly BasePdfBuilder _basePdfBuilder = basePdfBuilder;
    private readonly ILogger _logger = logger;
    private readonly MailWorker _mailWorker = new MailWorker();

    private static readonly string[] documentHeader = ["Название программы", "Фамилия", "Имя", "Баланс"];
    private static readonly string[] depositHeader = ["Название программы", "Ставка", "Сумма", "Срок"];
    private static readonly string[] clientsByDepositHeader = ["Фамилия", "Имя", "Баланс", "Ставка", "Срок", "Период"];
    private static readonly string[] currencyHeader = ["Валюта", "Кредитная программа", "Макс. сумма", "Ставка", "Срок"];

    public async Task<List<ClientsByCreditProgramDataModel>> GetDataClientsByCreditProgramAsync(CancellationToken ct)
    {
        _logger.LogInformation("Get data ClientsByCreditProgram");
        var clients = await Task.Run(() => _clientStorage.GetList(), ct);
        var creditPrograms = await Task.Run(() => _creditProgramStorage.GetList(), ct);

        return creditPrograms.Select(cp => new ClientsByCreditProgramDataModel
        {
            CreditProgramName = cp.Name,
            ClientSurname = clients.Where(c => c.CreditProgramClients.Any(cpc => cpc.CreditProgramId == cp.Id))
                .Select(c => c.Surname).ToList(),
            ClientName = clients.Where(c => c.CreditProgramClients.Any(cpc => cpc.CreditProgramId == cp.Id))
                .Select(c => c.Name).ToList(),
            ClientBalance = clients.Where(c => c.CreditProgramClients.Any(cpc => cpc.CreditProgramId == cp.Id))
                .Select(c => c.Balance).ToList()
        }).ToList();
    }

    public async Task<Stream> CreateDocumentClientsByCreditProgramAsync(CancellationToken ct)
    {
        _logger.LogInformation("Create report ClientsByCreditProgram");
        var data = await GetDataClientsByCreditProgramAsync(ct) ?? throw new InvalidOperationException("No found data");

        var tableRows = new List<string[]>
        {
            documentHeader
        };

        foreach (var program in data)
        {
            for (int i = 0; i < program.ClientSurname.Count; i++)
            {
                tableRows.Add(new string[]
                {
                    program.CreditProgramName,
                    program.ClientSurname[i],
                    program.ClientName[i],
                    program.ClientBalance[i].ToString("N2")
                });
            }
        }

        var stream = _baseWordBuilder
            .AddHeader("Клиенты по кредитным программам")
            .AddParagraph($"Сформировано на дату {DateTime.Now}")
            .AddTable([3000, 3000, 3000, 3000], tableRows)
            .Build();

        try
        {
            _mailWorker.sendYandex();
            _logger.LogInformation("Report sent via email successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send report via email");
        }

        return stream;
    }

    public async Task<List<ClientsByDepositDataModel>> GetDataClientsByDepositAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct)
    {
        _logger.LogInformation("Get data ClientsByDeposit from {dateStart} to {dateFinish}", dateStart, dateFinish);
        if (dateStart > dateFinish)
        {
            throw new ArgumentException("Start date cannot be later than finish date");
        }

        var clients = await Task.Run(() => _clientStorage.GetList(), ct);
        var deposits = await Task.Run(() => _depositStorage.GetList(), ct);

        return clients.SelectMany(c => c.DepositClients.Select(dc => new ClientsByDepositDataModel
        {
            ClientSurname = c.Surname,
            ClientName = c.Name,
            ClientBalance = c.Balance,
            DepositRate = deposits.First(d => d.Id == dc.DepositId).InterestRate,
            DepositPeriod = deposits.First(d => d.Id == dc.DepositId).Period,
            FromPeriod = dateStart,
            ToPeriod = dateFinish
        })).ToList();
    }

    public async Task<Stream> CreateDocumentClientsByDepositAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct)
    {
        _logger.LogInformation("Create report ClientsByDeposit from {dateStart} to {dateFinish}", dateStart, dateFinish);
        var data = await GetDataClientsByDepositAsync(dateStart, dateFinish, ct) ?? throw new InvalidOperationException("No found data");

        var tableRows = new List<string[]>
        {
            clientsByDepositHeader
        };

        foreach (var client in data)
        {
            tableRows.Add(new string[]
            {
                client.ClientSurname,
                client.ClientName,
                client.ClientBalance.ToString("N2"),
                client.DepositRate.ToString("N2"),
                client.DepositPeriod.ToString(),
                $"{client.FromPeriod.ToShortDateString()} - {client.ToPeriod.ToShortDateString()}"
            });
        }

        return _basePdfBuilder
            .AddHeader("Клиенты по вкладам")
            .AddParagraph($"за период с {dateStart.ToShortDateString()} по {dateFinish.ToShortDateString()}")
            .AddTable([3000, 3000, 3000, 3000, 3000, 3000], tableRows)
            .Build();
    }

    public async Task<List<CreditProgramAndDepositByCurrencyDataModel>> GetDataDepositAndCreditProgramByCurrencyAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct)
    {
        _logger.LogInformation("Get data DepositAndCreditProgramByCurrency from {dateStart} to {dateFinish}", dateStart, dateFinish);
        if (dateStart > dateFinish)
        {
            throw new ArgumentException("Start date cannot be later than finish date");
        }

        var currencies = await Task.Run(() => _currencyStorage.GetList(), ct);
        var creditPrograms = await Task.Run(() => _creditProgramStorage.GetList(), ct);
        var deposits = await Task.Run(() => _depositStorage.GetList(), ct);

        return currencies.Select(c => new CreditProgramAndDepositByCurrencyDataModel
        {
            CurrencyName = c.Name,
            CreditProgramName = creditPrograms.Where(cp => cp.Currencies.Any(cc => cc.CurrencyId == c.Id))
                .Select(cp => cp.Name).ToList(),
            CreditProgramMaxCost = creditPrograms.Where(cp => cp.Currencies.Any(cc => cc.CurrencyId == c.Id))
                .Select(cp => (int)cp.MaxCost).ToList(),
            DepositRate = deposits.Where(d => d.Currencies.Any(dc => dc.CurrencyId == c.Id))
                .Select(d => d.InterestRate).ToList(),
            DepositPeriod = deposits.Where(d => d.Currencies.Any(dc => dc.CurrencyId == c.Id))
                .Select(d => d.Period).ToList(),
            FromPeriod = dateStart,
            ToPeriod = dateFinish
        }).ToList();
    }

    public async Task<Stream> CreateDocumentDepositAndCreditProgramByCurrencyAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct)
    {
        _logger.LogInformation("Create report DepositAndCreditProgramByCurrency from {dateStart} to {dateFinish}", dateStart, dateFinish);
        var data = await GetDataDepositAndCreditProgramByCurrencyAsync(dateStart, dateFinish, ct) ?? throw new InvalidOperationException("No found data");

        var tableRows = new List<string[]>
        {
            currencyHeader
        };

        foreach (var currency in data)
        {
            for (int i = 0; i < currency.CreditProgramName.Count; i++)
            {
                tableRows.Add(new string[]
                {
                    currency.CurrencyName,
                    currency.CreditProgramName[i],
                    currency.CreditProgramMaxCost[i].ToString("N2"),
                    currency.DepositRate[i].ToString("N2"),
                    currency.DepositPeriod[i].ToString()
                });
            }
        }

        return _basePdfBuilder
            .AddHeader("Вклады и кредитные программы по валютам")
            .AddParagraph($"за период с {dateStart.ToShortDateString()} по {dateFinish.ToShortDateString()}")
            .AddTable([3000, 3000, 3000, 3000, 3000], tableRows)
            .Build();
    }

    public async Task<List<DepositByCreditProgramDataModel>> GetDataDepositByCreditProgramAsync(CancellationToken ct)
    {
        _logger.LogInformation("Get data DepositByCreditProgram");
        var deposits = await Task.Run(() => _depositStorage.GetList(), ct);
        var creditPrograms = await Task.Run(() => _creditProgramStorage.GetList(), ct);

        return creditPrograms.Select(cp => new DepositByCreditProgramDataModel
        {
            CreditProgramName = cp.Name,
            DepositRate = deposits.Select(d => d.InterestRate).ToList(),
            DepositCost = deposits.Select(d => d.Cost).ToList(),
            DepositPeriod = deposits.Select(d => d.Period).ToList()

        }).ToList();
    }

    public async Task<Stream> CreateDocumentDepositByCreditProgramAsync(CancellationToken ct)
    {
        _logger.LogInformation("Create report DepositByCreditProgram");
        var data = await GetDataDepositByCreditProgramAsync(ct) ?? throw new InvalidOperationException("No found data");

        var tableRows = new List<string[]>
        {
            depositHeader
        };

        foreach (var program in data)
        {
            for (int i = 0; i < program.DepositRate.Count; i++)
            {
                tableRows.Add(new string[]
                {
                    program.CreditProgramName,
                    program.DepositRate[i].ToString("N2"),
                    program.DepositCost[i].ToString("N2"),
                    program.DepositPeriod[i].ToString()
                });
            }
        }

        return _baseWordBuilder
            .AddHeader("Вклады по кредитным программам")
            .AddParagraph($"Сформировано на дату {DateTime.Now}")
            .AddTable([3000, 3000, 3000, 3000], tableRows)
            .Build();
    }
}
