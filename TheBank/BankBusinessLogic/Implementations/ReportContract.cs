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

    public Task<List<DepositByCreditProgramDataModel>> GetDataDepositByCreditProgramAsync(CancellationToken ct)
    {
        _logger.LogInformation("Get data DepositByCreditProgram");
        return GetDepositByCreditProgramListAsync(ct);
    }

    public async Task<Stream> CreateDocumentDepositByCreditProgramAsync(CancellationToken ct)
    {
        var data = await GetDepositByCreditProgramListAsync(ct) ?? throw new InvalidOperationException("No found data");

        // Заголовки таблицы
        var tableHeader = new[] { "Кредитная программа", "Процентная ставка", "Сумма", "Срок" };

        // Формируем строки таблицы
        var tableRows = new List<string[]>
        {
            tableHeader
        };

        foreach (var item in data)
        {
            // Строка с названием кредитной программы
            tableRows.Add(new[] { item.CreditProgramName, "", "", "" });

            // Строки с параметрами вкладов
            int count = Math.Min(Math.Min(item.DepositRate.Count, item.DepositCost.Count), item.DepositPeriod.Count);
            for (int i = 0; i < count; i++)
            {
                tableRows.Add(new[]
                {
                "",
                item.DepositRate[i].ToString("0.##"),
                item.DepositCost[i].ToString("0.##"),
                item.DepositPeriod[i].ToString()
            });
            }
        }

        return _baseWordBuilder
            .AddHeader("Вклады по кредитным программам")
            .AddTable(
                new[] { 2000, 2000, 2000, 2000 },
                tableRows
            )
            .Build();
    }

    private async Task<List<DepositByCreditProgramDataModel>> GetDepositByCreditProgramListAsync(CancellationToken ct)
    {
        // Получаем все кредитные программы
        var creditPrograms = _creditProgramStorage.GetList();
        // Получаем все вклады
        var deposits = _depositStorage.GetList();

        // Группируем вклады по кредитной программе
        var result = creditPrograms.Select(cp =>
        {
            var relatedDeposits = deposits
                .Where(d => d.Currencies.Any(c => cp.Currencies.Any(cc => cc.CurrencyId == c.CurrencyId)))
                .ToList();

            return new DepositByCreditProgramDataModel
            {
                CreditProgramName = cp.Name,
                DepositRate = relatedDeposits.Select(d => d.InterestRate).ToList(),
                DepositCost = relatedDeposits.Select(d => d.Cost).ToList(),
                DepositPeriod = relatedDeposits.Select(d => d.Period).ToList()
            };
        })
        // Оставляем только те программы, у которых есть вклады
        .Where(x => x.DepositRate.Count > 0)
        .ToList();

        return result;
    }

    public Task<List<ClientsByCreditProgramDataModel>> GetDataClientsByCreditProgramAsync(CancellationToken ct)
    {
        return GetClientsByCreditProgramListAsync(ct);
    }

    public async Task<Stream> CreateDocumentClientsByCreditProgramAsync(CancellationToken ct)
    {
        var data = await GetClientsByCreditProgramListAsync(ct) ?? throw new InvalidOperationException("No found data");

        // Заголовки таблицы
        var tableHeader = new[] { "Кредитная программа", "Фамилия клиента", "Имя клиента", "Баланс" };

        // Формируем строки таблицы
        var tableRows = new List<string[]>
        {
            tableHeader
        };

        foreach (var item in data)
        {
            // Строка с названием кредитной программы
            tableRows.Add(new[] { item.CreditProgramName, "", "", "" });

            // Строки с клиентами
            int count = Math.Min(Math.Min(item.ClientSurname.Count, item.ClientName.Count), item.ClientBalance.Count);
            for (int i = 0; i < count; i++)
            {
                tableRows.Add(new[]
                {
                "",
                item.ClientSurname[i],
                item.ClientName[i],
                item.ClientBalance[i].ToString("0.##")
            });
            }
        }

        return _baseWordBuilder
            .AddHeader("Клиенты по кредитным программам")
            .AddTable(
                new[] { 2000, 2000, 2000, 2000 },
                tableRows
            )
            .Build();
    }
    private async Task<List<ClientsByCreditProgramDataModel>> GetClientsByCreditProgramListAsync(CancellationToken ct)
    {
        // Получаем все кредитные программы
        var creditPrograms = _creditProgramStorage.GetList();
        // Получаем всех клиентов
        var clients = _clientStorage.GetList();

        var result = creditPrograms.Select(cp =>
        {
            // Находим клиентов, у которых есть эта кредитная программа
            var relatedClients = clients
                .Where(c => c.CreditProgramClients != null && c.CreditProgramClients.Any(cc => cc.CreditProgramId == cp.Id))
                .ToList();

            return new ClientsByCreditProgramDataModel
            {
                CreditProgramName = cp.Name,
                ClientSurname = relatedClients.Select(c => c.Surname).ToList(),
                ClientName = relatedClients.Select(c => c.Name).ToList(),
                ClientBalance = relatedClients.Select(c => c.Balance).ToList()
            };
        })
        // Оставляем только те программы, у которых есть клиенты
        .Where(x => x.ClientSurname.Count > 0)
        .ToList();

        return result;
    }

    public Task<List<ClientsByDepositDataModel>> GetDataClientsByDepositAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct)
    {
        _logger.LogInformation("Get data ClientsByDeposit from {dateStart} to {dateFinish}", dateStart, dateFinish);
        return GetClientsByDepositListAsync(dateStart, dateFinish, ct);
    }

    public async Task<Stream> CreateDocumentClientsByDepositAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct)
    {
        var data = await GetClientsByDepositListAsync(dateStart, dateFinish, ct) ?? throw new InvalidOperationException("No found data");

        // Заголовки таблицы
        var tableHeader = new[] { "Фамилия клиента", "Имя клиента", "Баланс", "Ставка", "Срок"};

        _logger.LogInformation("Create report SalesByPeriod from {dateStart} to {dateFinish}", dateStart, dateFinish);

        // Формируем строки таблицы
        var tableRows = new List<string[]>
        {
        tableHeader
        };

        foreach (var item in data)
        {
            tableRows.Add(new[]
            {
            item.ClientSurname,
            item.ClientName,
            item.ClientBalance.ToString("0.##"),
            item.DepositRate.ToString("0.##"),
            item.DepositPeriod.ToString(),
        });
        }

        return _basePdfBuilder
            .AddHeader("Клиенты по вкладам")
            .AddParagraph($"с {dateStart.ToShortDateString()} по {dateFinish.ToShortDateString()}")
            .CreateTable(
                new[] { 1500, 1500, 1500, 1000, 1000},
                tableRows
            )
            .Build();
    }

    private async Task<List<ClientsByDepositDataModel>> GetClientsByDepositListAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct)
    {
        // Получаем всех клиентов
        var clients = _clientStorage.GetList();
        // Получаем все вклады за период (если есть поле даты, фильтруйте по нему)
        var deposits = await _depositStorage.GetListAsync(dateStart, dateFinish, ct);

        var result = new List<ClientsByDepositDataModel>();

        foreach (var client in clients)
        {
            if (client.DepositClients == null || client.DepositClients.Count == 0)
                continue;

            foreach (var depositClient in client.DepositClients)
            {
                var deposit = deposits.FirstOrDefault(d => d.Id == depositClient.DepositId);
                if (deposit == null)
                    continue;

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

        return result;
    }

    public Task<List<CreditProgramAndDepositByCurrencyDataModel>> GetDataDepositAndCreditProgramByCurrencyAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct)
    {
        _logger.LogInformation("Get data DepositAndCreditProgramByCurrency from {dateStart} to {dateFinish}", dateStart, dateFinish);
        return GetDepositAndCreditProgramByCurrencyListAsync(dateStart, dateFinish, ct);
    }


    public async Task<Stream> CreateDocumentDepositAndCreditProgramByCurrencyAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct)
    {
        var data = await GetDepositAndCreditProgramByCurrencyListAsync(dateStart, dateFinish, ct) ?? throw new InvalidOperationException("No found data");

        // Заголовки таблицы
        var tableHeader = new[] { "Валюта", "Кредитная программа", "Макс. сумма", "Ставка по вкладу", "Срок вклада" };

        // Формируем строки таблицы
        var tableRows = new List<string[]>
        {
            tableHeader
        };

        foreach (var item in data)
        {
            int count = Math.Max(
                Math.Max(item.CreditProgramName.Count, item.CreditProgramMaxCost.Count),
                Math.Max(item.DepositRate.Count, item.DepositPeriod.Count)
            );

            for (int i = 0; i < count; i++)
            {
                tableRows.Add(new[]
                {
                i == 0 ? item.CurrencyName : "",
                i < item.CreditProgramName.Count ? item.CreditProgramName[i] : "",
                i < item.CreditProgramMaxCost.Count ? item.CreditProgramMaxCost[i].ToString() : "",
                i < item.DepositRate.Count ? item.DepositRate[i].ToString("0.##") : "",
                i < item.DepositPeriod.Count ? item.DepositPeriod[i].ToString() : ""
            });
            }
        }

        return _basePdfBuilder
            .AddHeader("Кредитные программы и вклады по валютам")
            .AddParagraph($"с {dateStart:dd.MM.yyyy} по {dateFinish:dd.MM.yyyy}")
            .CreateTable(
                new[] { 1500, 2000, 1500, 1500, 1500 },
                tableRows
            )
            .Build();
    }

    private async Task<List<CreditProgramAndDepositByCurrencyDataModel>> GetDepositAndCreditProgramByCurrencyListAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct)
    {
        // Получаем все валюты
        var currencies = _currencyStorage.GetList();
        // Получаем все кредитные программы
        var creditPrograms = _creditProgramStorage.GetList();
        // Получаем все вклады за период (если есть поле даты, фильтруйте по нему)
        var deposits = await _depositStorage.GetListAsync(dateStart, dateFinish, ct);

        var result = new List<CreditProgramAndDepositByCurrencyDataModel>();

        foreach (var currency in currencies)
        {
            // Кредитные программы, связанные с этой валютой
            var relatedCreditPrograms = creditPrograms
                .Where(cp => cp.Currencies != null && cp.Currencies.Any(cc => cc.CurrencyId == currency.Id))
                .ToList();

            // Вклады, связанные с этой валютой
            var relatedDeposits = deposits
                .Where(d => d.Currencies != null && d.Currencies.Any(dc => dc.CurrencyId == currency.Id))
                .ToList();

            if (relatedCreditPrograms.Count == 0 && relatedDeposits.Count == 0)
                continue;

            result.Add(new CreditProgramAndDepositByCurrencyDataModel
            {
                CurrencyName = currency.Name,
                CreditProgramName = relatedCreditPrograms.Select(cp => cp.Name).ToList(),
                CreditProgramMaxCost = relatedCreditPrograms.Select(cp => (int)cp.MaxCost).ToList(),
                DepositRate = relatedDeposits.Select(d => d.InterestRate).ToList(),
                DepositPeriod = relatedDeposits.Select(d => d.Period).ToList(),
                FromPeriod = dateStart,
                ToPeriod = dateFinish
            });
        }

        return result;
    }
}
