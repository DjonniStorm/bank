using BankBusinessLogic.OfficePackage;
using BankContracts.BindingModels;
using BankContracts.BusinessLogicContracts;
using BankContracts.DataModels;
using BankContracts.StorageContracts;
using BankContracts.ViewModels;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Logging;

namespace BankBusinessLogic.Implementations;

public class ReportContract(IClientStorageContract clientStorage, ICurrencyStorageContract currencyStorage,
    ICreditProgramStorageContract creditProgramStorage, IDepositStorageContract depositStorage,
    BaseWordBuilder baseWordBuilder, BaseExcelBuilder baseExcelBuilder, BasePdfBuilder basePdfBuilder) : IReportContract
{
    private readonly IClientStorageContract _clientStorage = clientStorage;
    private readonly ICurrencyStorageContract _currencyStorage = currencyStorage;
    private readonly ICreditProgramStorageContract _creditProgramStorage = creditProgramStorage;
    private readonly IDepositStorageContract _depositStorage = depositStorage;
    private readonly BaseWordBuilder _baseWordBuilder = baseWordBuilder;
    private readonly BaseExcelBuilder _baseExcelBuilder = baseExcelBuilder;
    private readonly BasePdfBuilder _basePdfBuilder = basePdfBuilder;

    internal static readonly string[] documentHeaderDepositByCreditProgram = ["Кредитная программа", "Процентная ставка", "Сумм", "Срок"];
    internal static readonly string[] tableHeaderDepositByCreditProgram = ["Кредитная программа", "Процентная ставка", "Сумм", "Срок"];

    public Task<List<DepositByCreditProgramDataModel>> GetDataDepositByCreditProgramAsync(CancellationToken ct)
    {
        return GetDepositByCreditProgramListAsync(ct);
    }

    public async Task<Stream> CreateDocumentDepositByCreditProgramAsync(CancellationToken ct)
    {
        var data = await GetDepositByCreditProgramListAsync(ct) ?? throw new InvalidOperationException("No found data");

        // Формируем строки таблицы
        var tableRows = new List<string[]>
        {
            documentHeaderDepositByCreditProgram
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

        return _basePdfBuilder
            .AddHeader("Вклады по кредитным программам")
            .CreateTable(
                new[] { 1500, 1500, 1500, 1500 },
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
            .AddParagraph($"Сформировано на дату {DateTime.Now:dd.MM.yyyy HH:mm}")
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
}

