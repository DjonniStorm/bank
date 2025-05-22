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

    private static readonly string[] documentHeader = ["Название программы", "Фамилия", "Имя", "Баланс"];
    private static readonly string[] depositHeader = ["Название программы", "Ставка", "Сумма", "Срок"];
    private static readonly string[] clientsByDepositHeader = ["Фамилия", "Имя", "Баланс", "Ставка", "Срок", "Период"];
    private static readonly string[] currencyHeader = ["Валюта", "Кредитная программа", "Макс. сумма", "Ставка", "Срок"];

    public async Task<List<ClientsByCreditProgramDataModel>> GetDataClientsByCreditProgramAsync(CancellationToken ct)
    {
        _logger.LogInformation("Get data ClientsByCreditProgram");
        var clients = await Task.Run(() => _clientStorage.GetList(), ct);
        var creditPrograms = await Task.Run(() => _creditProgramStorage.GetList(), ct);
        var currencies = await Task.Run(() => _currencyStorage.GetList(), ct);

        return creditPrograms
            .Where(cp => cp.Currencies.Any()) // Проверяем, что у кредитной программы есть связанные валюты
            .Select(cp => new ClientsByCreditProgramDataModel
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

        return _baseWordBuilder
            .AddHeader("Клиенты по кредитным программам")
            .AddParagraph($"Сформировано на дату {DateTime.Now}")
            .AddTable([25, 25, 25, 25], tableRows)
            .Build();
    }

    public async Task<Stream> CreateExcelDocumentClientsByCreditProgramAsync(CancellationToken ct)
    {
        _logger.LogInformation("Create Excel report ClientsByCreditProgram");
        var data = await GetDataClientsByCreditProgramAsync(ct) ?? throw new InvalidOperationException("No found data");

        var tableRows = new List<string[]>
        {
            documentHeader
        };

        foreach (var program in data)
        {
            for (int i = 0; i < program.ClientSurname.Count; i++)
            {
                tableRows.Add(
                [
                    program.CreditProgramName,
                    program.ClientSurname[i],
                    program.ClientName[i],
                    program.ClientBalance[i].ToString("N2")
                ]);
            }
        }

        return _baseExcelBuilder
            .AddHeader("Клиенты по кредитным программам", 0, 4)
            .AddParagraph($"Сформировано на дату {DateTime.Now}", 0)
            .AddTable([25, 25, 25, 25], tableRows)
            .Build();
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

        var result = new List<ClientsByDepositDataModel>();
        foreach (var client in clients)
        {
            if (client.DepositClients == null || !client.DepositClients.Any())
            {
                continue;
            }

            foreach (var depositClient in client.DepositClients)
            {
                var deposit = deposits.FirstOrDefault(d => d.Id == depositClient.DepositId);
                if (deposit == null)
                {
                    continue;
                }

                result.Add(new ClientsByDepositDataModel
                {
                    ClientSurname = client.Surname,
                    ClientName = client.Name,
                    ClientBalance = client.Balance,
                    DepositRate = deposit.InterestRate,
                    DepositPeriod = deposit.Period,
                    FromPeriod = dateStart,
                    ToPeriod = dateFinish
                });
            }
        }

        if (!result.Any())
        {
            throw new InvalidOperationException("No clients with deposits found");
        }

        return result;
    }

    public async Task<Stream> CreateDocumentClientsByDepositAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct)
    {
        _logger.LogInformation("Create report ClientsByDeposit from {dateStart} to {dateFinish}", dateStart, dateFinish);
        var data = await GetDataClientsByDepositAsync(dateStart, dateFinish, ct) ?? throw new InvalidOperationException("No found data");

        // Двухуровневый заголовок
        var tableRows = new List<string[]>
        {
            new string[] { "Клиент", "Клиент", "Клиент", "Вклад", "Вклад", "Период" },
            new string[] { "Фамилия", "Имя", "Баланс", "Ставка", "Срок", "" }
        };

        foreach (var client in data)
        {
            tableRows.Add(
            [
                client.ClientSurname,
                client.ClientName,
                client.ClientBalance.ToString("N2"),
                client.DepositRate.ToString("N2"),
                $"{client.DepositPeriod} мес.",
                $"{client.FromPeriod.ToShortDateString()} - {client.ToPeriod.ToShortDateString()}"
            ]);
        }

        return _basePdfBuilder
            .AddHeader("Клиенты по вкладам")
            .AddParagraph($"за период с {dateStart.ToShortDateString()} по {dateFinish.ToShortDateString()}")
            .AddTable([25, 25, 25, 25, 25, 25], tableRows)
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

        // Двухуровневый заголовок
        var tableRows = new List<string[]>
        {
            new string[] { "Наименование валюты", "Кредитная программа", "Кредитная программа", "Вклад", "Вклад" },
            new string[] { "", "Название", "Максимальная сумма", "Процентная ставка", "Срок" }
        };

        foreach (var currency in data)
        {
            // Вывод информации по кредитным программам
            for (int i = 0; i < currency.CreditProgramName.Count; i++)
            {
                // Вычисляем индекс депозита, если есть соответствующие
                string depositRate = "—";
                string depositPeriod = "—";

                // Проверяем, есть ли депозиты для этой валюты и не вышли ли мы за границы массива
                if (currency.DepositRate.Count > 0)
                {
                    // Берем индекс по модулю, чтобы не выйти за границы массива
                    int depositIndex = i % currency.DepositRate.Count;
                    depositRate = currency.DepositRate[depositIndex].ToString("N2");
                    depositPeriod = $"{currency.DepositPeriod[depositIndex]} мес.";
                }

                // Добавляем строку в таблицу
                tableRows.Add(
                [
                    currency.CurrencyName,
                    currency.CreditProgramName[i],
                    currency.CreditProgramMaxCost[i].ToString("N2"),
                    depositRate,
                    depositPeriod
                ]);
            }

            // Если есть депозиты, но нет кредитных программ, добавляем строки только с депозитами
            if (currency.CreditProgramName.Count == 0 && currency.DepositRate.Count > 0)
            {
                for (int j = 0; j < currency.DepositRate.Count; j++)
                {
                    tableRows.Add(
                    [
                        currency.CurrencyName,
                        "—",
                        "—",
                        currency.DepositRate[j].ToString("N2"),
                        $"{currency.DepositPeriod[j]} мес."
                    ]);
                }
            }
        }

        return _basePdfBuilder
            .AddHeader("Вклады и кредитные программы по валютам")
            .AddParagraph($"за период с {dateStart.ToShortDateString()} по {dateFinish.ToShortDateString()}")
            .AddTable([25, 30, 25, 25, 25], tableRows)
            .Build();
    }

    public async Task<List<DepositByCreditProgramDataModel>> GetDataDepositByCreditProgramAsync(CancellationToken ct)
    {
        _logger.LogInformation("Get data DepositByCreditProgram");
        var deposits = await Task.Run(() => _depositStorage.GetList(), ct);
        var creditPrograms = await Task.Run(() => _creditProgramStorage.GetList(), ct);

        // Проверяем, что у вкладов есть связанные валюты
        if (!deposits.Any(d => d.Currencies.Any()))
        {
            throw new InvalidOperationException("No deposits with currencies found");
        }

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
                tableRows.Add(
                [
                    program.CreditProgramName,
                    program.DepositRate[i].ToString("N2"),
                    program.DepositCost[i].ToString("N2"),
                    program.DepositPeriod[i].ToString()
                ]);
            }
        }

        return _baseWordBuilder
            .AddHeader("Вклады по кредитным программам")
            .AddParagraph($"Сформировано на дату {DateTime.Now}")
            .AddTable([25, 25, 25, 25], tableRows)
            .Build();
    }

    public async Task<Stream> CreateExcelDocumentDepositByCreditProgramAsync(CancellationToken ct)
    {
        _logger.LogInformation("Create Excel report DepositByCreditProgram");
        var data = await GetDataDepositByCreditProgramAsync(ct) ?? throw new InvalidOperationException("No found data");

        var tableRows = new List<string[]>
        {
            depositHeader
        };

        foreach (var program in data)
        {
            for (int i = 0; i < program.DepositRate.Count; i++)
            {
                tableRows.Add(
                [
                    program.CreditProgramName,
                    program.DepositRate[i].ToString("N2"),
                    program.DepositCost[i].ToString("N2"),
                    program.DepositPeriod[i].ToString()
                ]);
            }
        }

        return _baseExcelBuilder
            .AddHeader("Вклады по кредитным программам", 0, 4)
            .AddParagraph($"Сформировано на дату {DateTime.Now}", 0)
            .AddTable([25, 25, 25, 25], tableRows)
            .Build();
    }
}
