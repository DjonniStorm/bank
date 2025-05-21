using BankContracts.ViewModels;
using BankDatabase.Models;
using BankTests.Infrastructure;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace BankTests.WebApiControllersTests;

[TestFixture]
internal class ReportControllerTests : BaseWebApiControllerTest
{
    [TearDown]
    public void TearDown()
    {
        BankDbContext.RemoveClientsFromDatabase();
        BankDbContext.RemoveCreditProgramsFromDatabase();
        BankDbContext.RemoveDepositsFromDatabase();
        BankDbContext.RemoveCurrenciesFromDatabase();
    }

    [Test]
    public async Task GetDepositByCreditProgram_WhenHaveRecords_ShouldSuccess_Test()
    {
        //Arrange
        var storekeeper = BankDbContext.InsertStorekeeperToDatabaseAndReturn(
            email: $"storekeeper_{Guid.NewGuid()}@email.com",
            login: $"storekeeper_{Guid.NewGuid()}",
            phone: $"+7-{Guid.NewGuid():N}"
        );
        var period = BankDbContext.InsertPeriodToDatabaseAndReturn(storekeeperId: storekeeper.Id);
        
        // Создаем валюты
        var currency1 = BankDbContext.InsertCurrencyToDatabaseAndReturn(
            name: "Рубль",
            abbreviation: "RUB",
            storekeeperId: storekeeper.Id
        );
        var currency2 = BankDbContext.InsertCurrencyToDatabaseAndReturn(
            name: "Доллар",
            abbreviation: "USD",
            storekeeperId: storekeeper.Id
        );

        // Создаем кредитные программы и связываем их с валютами
        var creditProgram1 = BankDbContext.InsertCreditProgramToDatabaseAndReturn(
            name: "Credit Program 1",
            storekeeperId: storekeeper.Id,
            periodId: period.Id,
            creditProgramCurrency: [(currency1.Id, Guid.NewGuid().ToString())]
        );
        var creditProgram2 = BankDbContext.InsertCreditProgramToDatabaseAndReturn(
            name: "Credit Program 2",
            storekeeperId: storekeeper.Id,
            periodId: period.Id,
            creditProgramCurrency: [(currency2.Id, Guid.NewGuid().ToString())]
        );

        var clerk = BankDbContext.InsertClerkToDatabaseAndReturn(
            email: $"clerk_{Guid.NewGuid()}@email.com",
            login: $"clerk_{Guid.NewGuid()}",
            phone: $"+7-{Guid.NewGuid():N}"
        );
        
        // Создаем вклады и связываем их с валютами
        var deposit1 = BankDbContext.InsertDepositToDatabaseAndReturn(
            interestRate: 5.5f,
            cost: 10000,
            period: 12,
            clerkId: clerk.Id
        );
        var deposit2 = BankDbContext.InsertDepositToDatabaseAndReturn(
            interestRate: 6.5f,
            cost: 20000,
            period: 24,
            clerkId: clerk.Id
        );

        // Связываем вклады с валютами
        BankDbContext.DepositCurrencies.Add(new DepositCurrency { DepositId = deposit1.Id, CurrencyId = currency1.Id });
        BankDbContext.DepositCurrencies.Add(new DepositCurrency { DepositId = deposit2.Id, CurrencyId = currency2.Id });
        BankDbContext.SaveChanges();

        //Act
        var response = await HttpClient.GetAsync("/api/report/getdepositbycreditprogram");
        var result = await GetModelFromResponseAsync<List<DepositByCreditProgramViewModel>>(response);

        //Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        
        var firstProgram = result.First(x => x.CreditProgramName == "Credit Program 1");
        Assert.That(firstProgram.DepositRate, Has.Count.EqualTo(1));
        Assert.That(firstProgram.DepositRate[0], Is.EqualTo(5.5f));
        Assert.That(firstProgram.DepositCost[0], Is.EqualTo(10000));
        Assert.That(firstProgram.DepositPeriod[0], Is.EqualTo(12));

        var secondProgram = result.First(x => x.CreditProgramName == "Credit Program 2");
        Assert.That(secondProgram.DepositRate, Has.Count.EqualTo(1));
        Assert.That(secondProgram.DepositRate[0], Is.EqualTo(6.5f));
        Assert.That(secondProgram.DepositCost[0], Is.EqualTo(20000));
        Assert.That(secondProgram.DepositPeriod[0], Is.EqualTo(24));
    }

    [Test]
    public async Task LoadDepositByCreditProgram_WhenNoData_ShouldBadRequest_Test()
    {
        //Arrange
        var storekeeper = BankDbContext.InsertStorekeeperToDatabaseAndReturn(
            email: $"storekeeper_{Guid.NewGuid()}@email.com",
            login: $"storekeeper_{Guid.NewGuid()}",
            phone: $"+7-{Guid.NewGuid():N}"
        );
        var period = BankDbContext.InsertPeriodToDatabaseAndReturn(storekeeperId: storekeeper.Id);
        
        // Создаем валюты
        var currency = BankDbContext.InsertCurrencyToDatabaseAndReturn(
            name: "Рубль",
            abbreviation: "RUB",
            storekeeperId: storekeeper.Id
        );

        // Создаем кредитную программу и связываем её с валютой
        var creditProgram = BankDbContext.InsertCreditProgramToDatabaseAndReturn(
            name: "Credit Program 1",
            storekeeperId: storekeeper.Id,
            periodId: period.Id,
            creditProgramCurrency: [(currency.Id, Guid.NewGuid().ToString())]
        );

        var clerk = BankDbContext.InsertClerkToDatabaseAndReturn(
            email: $"clerk_{Guid.NewGuid()}@email.com",
            login: $"clerk_{Guid.NewGuid()}",
            phone: $"+7-{Guid.NewGuid():N}"
        );
        
        // Создаем вклад, но НЕ связываем его с валютой
        var deposit = BankDbContext.InsertDepositToDatabaseAndReturn(
            interestRate: 5.5f,
            cost: 10000,
            period: 12,
            clerkId: clerk.Id
        );

        BankDbContext.SaveChanges();

        //Act
        var response = await HttpClient.GetAsync("/api/report/loaddepositbycreditprogram");

        //Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task LoadDepositByCreditProgram_WhenHaveRecords_ShouldSuccess_Test()
    {
        //Arrange
        var storekeeper = BankDbContext.InsertStorekeeperToDatabaseAndReturn(
            email: $"storekeeper_{Guid.NewGuid()}@email.com",
            login: $"storekeeper_{Guid.NewGuid()}",
            phone: $"+7-{Guid.NewGuid():N}"
        );
        var period = BankDbContext.InsertPeriodToDatabaseAndReturn(storekeeperId: storekeeper.Id);
        
        // Создаем валюты
        var currency1 = BankDbContext.InsertCurrencyToDatabaseAndReturn(
            name: "Рубль",
            abbreviation: "RUB",
            storekeeperId: storekeeper.Id
        );
        var currency2 = BankDbContext.InsertCurrencyToDatabaseAndReturn(
            name: "Доллар",
            abbreviation: "USD",
            storekeeperId: storekeeper.Id
        );

        // Создаем кредитные программы и связываем их с валютами
        var creditProgram1 = BankDbContext.InsertCreditProgramToDatabaseAndReturn(
            name: "Credit Program 1",
            storekeeperId: storekeeper.Id,
            periodId: period.Id,
            creditProgramCurrency: [(currency1.Id, Guid.NewGuid().ToString())]
        );
        var creditProgram2 = BankDbContext.InsertCreditProgramToDatabaseAndReturn(
            name: "Credit Program 2",
            storekeeperId: storekeeper.Id,
            periodId: period.Id,
            creditProgramCurrency: [(currency2.Id, Guid.NewGuid().ToString())]
        );

        var clerk = BankDbContext.InsertClerkToDatabaseAndReturn(
            email: $"clerk_{Guid.NewGuid()}@email.com",
            login: $"clerk_{Guid.NewGuid()}",
            phone: $"+7-{Guid.NewGuid():N}"
        );
        
        // Создаем вклады и связываем их с валютами
        var deposit1 = BankDbContext.InsertDepositToDatabaseAndReturn(
            interestRate: 5.5f,
            cost: 10000,
            period: 12,
            clerkId: clerk.Id
        );
        var deposit2 = BankDbContext.InsertDepositToDatabaseAndReturn(
            interestRate: 6.5f,
            cost: 20000,
            period: 24,
            clerkId: clerk.Id
        );

        // Связываем вклады с валютами
        BankDbContext.DepositCurrencies.Add(new DepositCurrency { DepositId = deposit1.Id, CurrencyId = currency1.Id });
        BankDbContext.DepositCurrencies.Add(new DepositCurrency { DepositId = deposit2.Id, CurrencyId = currency2.Id });
        BankDbContext.SaveChanges();

        //Act
        var response = await HttpClient.GetAsync("/api/report/loaddepositbycreditprogram");

        //Assert
        await AssertStreamAsync(response, "deposit_by_credit_program.docx");
    }

    [Test]
    public async Task LoadClientsByCreditProgram_WhenHaveRecords_ShouldSuccess_Test()
    {
        //Arrange
        var storekeeper = BankDbContext.InsertStorekeeperToDatabaseAndReturn(
            email: $"storekeeper_{Guid.NewGuid()}@email.com",
            login: $"storekeeper_{Guid.NewGuid()}",
            phone: $"+7-{Guid.NewGuid():N}"
        );
        var clerk = BankDbContext.InsertClerkToDatabaseAndReturn(
            email: $"clerk_{Guid.NewGuid()}@email.com",
            login: $"clerk_{Guid.NewGuid()}",
            phone: $"+7-{Guid.NewGuid():N}"
        );
        var period = BankDbContext.InsertPeriodToDatabaseAndReturn(storekeeperId: storekeeper.Id);
        
        // Создаем валюты
        var currency1 = BankDbContext.InsertCurrencyToDatabaseAndReturn(
            name: "Рубль",
            abbreviation: "RUB",
            storekeeperId: storekeeper.Id
        );
        var currency2 = BankDbContext.InsertCurrencyToDatabaseAndReturn(
            name: "Доллар",
            abbreviation: "USD",
            storekeeperId: storekeeper.Id
        );

        // Создаем кредитные программы и связываем их с валютами
        var creditProgram1 = BankDbContext.InsertCreditProgramToDatabaseAndReturn(
            name: "Credit Program 1",
            storekeeperId: storekeeper.Id,
            periodId: period.Id,
            creditProgramCurrency: [(currency1.Id, Guid.NewGuid().ToString())]
        );
        var creditProgram2 = BankDbContext.InsertCreditProgramToDatabaseAndReturn(
            name: "Credit Program 2",
            storekeeperId: storekeeper.Id,
            periodId: period.Id,
            creditProgramCurrency: [(currency2.Id, Guid.NewGuid().ToString())]
        );

        // Создаем клиентов
        var client1 = BankDbContext.InsertClientToDatabaseAndReturn(
            surname: "Ivanov",
            name: "Ivan",
            balance: 10000,
            clerkId: clerk.Id
        );
        var client2 = BankDbContext.InsertClientToDatabaseAndReturn(
            surname: "Petrov",
            name: "Petr",
            balance: 20000,
            clerkId: clerk.Id
        );

        // Связываем клиентов с кредитными программами
        BankDbContext.CreditProgramClients.Add(new ClientCreditProgram { ClientId = client1.Id, CreditProgramId = creditProgram1.Id });
        BankDbContext.CreditProgramClients.Add(new ClientCreditProgram { ClientId = client2.Id, CreditProgramId = creditProgram2.Id });
        BankDbContext.SaveChanges();

        //Act
        var response = await HttpClient.GetAsync("/api/report/loadclientsbycreditprogram");

        //Assert
        await AssertStreamAsync(response, "clients_by_credit_program.docx");
    }

    [Test]
    public async Task LoadClientsByDeposit_WhenHaveRecords_ShouldSuccess_Test()
    {
        //Arrange
        var storekeeper = BankDbContext.InsertStorekeeperToDatabaseAndReturn(
            email: $"storekeeper_{Guid.NewGuid()}@email.com",
            login: $"storekeeper_{Guid.NewGuid()}",
            phone: $"+7-{Guid.NewGuid():N}"
        );
        var clerk = BankDbContext.InsertClerkToDatabaseAndReturn(
            email: $"clerk_{Guid.NewGuid()}@email.com",
            login: $"clerk_{Guid.NewGuid()}",
            phone: $"+7-{Guid.NewGuid():N}"
        );

        // Создаем клиентов
        var client1 = BankDbContext.InsertClientToDatabaseAndReturn(
            surname: "Ivanov",
            name: "Ivan",
            balance: 10000,
            clerkId: clerk.Id
        );
        var client2 = BankDbContext.InsertClientToDatabaseAndReturn(
            surname: "Petrov",
            name: "Petr",
            balance: 20000,
            clerkId: clerk.Id
        );

        // Создаем вклады
        var deposit1 = BankDbContext.InsertDepositToDatabaseAndReturn(
            interestRate: 5.5f,
            cost: 10000,
            period: 12,
            clerkId: clerk.Id
        );
        var deposit2 = BankDbContext.InsertDepositToDatabaseAndReturn(
            interestRate: 6.5f,
            cost: 20000,
            period: 24,
            clerkId: clerk.Id
        );

        // Связываем клиентов с вкладами
        BankDbContext.DepositClients.Add(new DepositClient { ClientId = client1.Id, DepositId = deposit1.Id });
        BankDbContext.DepositClients.Add(new DepositClient { ClientId = client2.Id, DepositId = deposit2.Id });
        BankDbContext.SaveChanges();

        // Проверяем, что связи созданы
        var client1After = BankDbContext.Clients
            .Include(x => x.DepositClients)
            .First(x => x.Id == client1.Id);
        var client2After = BankDbContext.Clients
            .Include(x => x.DepositClients)
            .First(x => x.Id == client2.Id);

        Assert.That(client1After.DepositClients, Is.Not.Null);
        Assert.That(client1After.DepositClients, Has.Count.EqualTo(1));
        Assert.That(client2After.DepositClients, Is.Not.Null);
        Assert.That(client2After.DepositClients, Has.Count.EqualTo(1));

        var dateStart = DateTime.UtcNow.AddDays(-1);
        var dateFinish = DateTime.UtcNow.AddDays(1);

        //Act
        var response = await HttpClient.GetAsync($"/api/report/loadclientsbydeposit?fromDate={dateStart:yyyy-MM-dd}&toDate={dateFinish:yyyy-MM-dd}");

        //Assert
        await AssertStreamAsync(response, "clients_by_deposit.pdf");
    }

    [Test]
    public async Task LoadDepositAndCreditProgramByCurrency_WhenHaveRecords_ShouldSuccess_Test()
    {
        //Arrange
        var storekeeper = BankDbContext.InsertStorekeeperToDatabaseAndReturn(
            email: $"storekeeper_{Guid.NewGuid()}@email.com",
            login: $"storekeeper_{Guid.NewGuid()}",
            phone: $"+7-{Guid.NewGuid():N}"
        );
        var period = BankDbContext.InsertPeriodToDatabaseAndReturn(storekeeperId: storekeeper.Id);
        
        // Создаем валюты
        var currency1 = BankDbContext.InsertCurrencyToDatabaseAndReturn(
            name: "Рубль",
            abbreviation: "RUB",
            storekeeperId: storekeeper.Id
        );
        var currency2 = BankDbContext.InsertCurrencyToDatabaseAndReturn(
            name: "Доллар",
            abbreviation: "USD",
            storekeeperId: storekeeper.Id
        );

        // Создаем кредитные программы и связываем их с валютами
        var creditProgram1 = BankDbContext.InsertCreditProgramToDatabaseAndReturn(
            name: "Credit Program 1",
            storekeeperId: storekeeper.Id,
            periodId: period.Id,
            creditProgramCurrency: [(currency1.Id, Guid.NewGuid().ToString())]
        );
        var creditProgram2 = BankDbContext.InsertCreditProgramToDatabaseAndReturn(
            name: "Credit Program 2",
            storekeeperId: storekeeper.Id,
            periodId: period.Id,
            creditProgramCurrency: [(currency2.Id, Guid.NewGuid().ToString())]
        );

        var clerk = BankDbContext.InsertClerkToDatabaseAndReturn(
            email: $"clerk_{Guid.NewGuid()}@email.com",
            login: $"clerk_{Guid.NewGuid()}",
            phone: $"+7-{Guid.NewGuid():N}"
        );
        
        // Создаем вклады и связываем их с валютами
        var deposit1 = BankDbContext.InsertDepositToDatabaseAndReturn(
            interestRate: 5.5f,
            cost: 10000,
            period: 12,
            clerkId: clerk.Id
        );
        var deposit2 = BankDbContext.InsertDepositToDatabaseAndReturn(
            interestRate: 6.5f,
            cost: 20000,
            period: 24,
            clerkId: clerk.Id
        );

        // Связываем вклады с валютами
        BankDbContext.DepositCurrencies.Add(new DepositCurrency { DepositId = deposit1.Id, CurrencyId = currency1.Id });
        BankDbContext.DepositCurrencies.Add(new DepositCurrency { DepositId = deposit2.Id, CurrencyId = currency2.Id });
        BankDbContext.SaveChanges();

        var dateStart = DateTime.UtcNow.AddDays(-1);
        var dateFinish = DateTime.UtcNow.AddDays(1);

        //Act
        var response = await HttpClient.GetAsync($"/api/report/loaddepositandcreditprogrambycurrency?fromDate={dateStart:yyyy-MM-dd}&toDate={dateFinish:yyyy-MM-dd}");

        //Assert
        await AssertStreamAsync(response, "deposit_and_credit_program_by_currency.pdf");
    }

    [Test]
    public async Task ExcelClientByCreditProgram_WhenHaveRecords_ShouldSuccess_Test()
    {
        //Arrange
        var storekeeper = BankDbContext.InsertStorekeeperToDatabaseAndReturn(
            email: $"storekeeper_{Guid.NewGuid()}@email.com",
            login: $"storekeeper_{Guid.NewGuid()}",
            phone: $"+7-{Guid.NewGuid():N}"
        );
        var clerk = BankDbContext.InsertClerkToDatabaseAndReturn(
            email: $"clerk_{Guid.NewGuid()}@email.com",
            login: $"clerk_{Guid.NewGuid()}",
            phone: $"+7-{Guid.NewGuid():N}"
        );
        var period = BankDbContext.InsertPeriodToDatabaseAndReturn(storekeeperId: storekeeper.Id);
        
        // Создаем валюты
        var currency1 = BankDbContext.InsertCurrencyToDatabaseAndReturn(
            name: "Рубль",
            abbreviation: "RUB",
            storekeeperId: storekeeper.Id
        );
        var currency2 = BankDbContext.InsertCurrencyToDatabaseAndReturn(
            name: "Доллар",
            abbreviation: "USD",
            storekeeperId: storekeeper.Id
        );

        // Создаем кредитные программы и связываем их с валютами
        var creditProgram1 = BankDbContext.InsertCreditProgramToDatabaseAndReturn(
            name: "Credit Program 1",
            storekeeperId: storekeeper.Id,
            periodId: period.Id,
            creditProgramCurrency: [(currency1.Id, Guid.NewGuid().ToString())]
        );
        var creditProgram2 = BankDbContext.InsertCreditProgramToDatabaseAndReturn(
            name: "Credit Program 2",
            storekeeperId: storekeeper.Id,
            periodId: period.Id,
            creditProgramCurrency: [(currency2.Id, Guid.NewGuid().ToString())]
        );

        // Создаем клиентов
        var client1 = BankDbContext.InsertClientToDatabaseAndReturn(
            surname: "Ivanov",
            name: "Ivan",
            balance: 10000,
            clerkId: clerk.Id
        );
        var client2 = BankDbContext.InsertClientToDatabaseAndReturn(
            surname: "Petrov",
            name: "Petr",
            balance: 20000,
            clerkId: clerk.Id
        );

        // Связываем клиентов с кредитными программами
        BankDbContext.CreditProgramClients.Add(new ClientCreditProgram { ClientId = client1.Id, CreditProgramId = creditProgram1.Id });
        BankDbContext.CreditProgramClients.Add(new ClientCreditProgram { ClientId = client2.Id, CreditProgramId = creditProgram2.Id });
        BankDbContext.SaveChanges();

        //Act
        var response = await HttpClient.GetAsync("/api/report/loadexcelclientbycreditprogram");

        //Assert
        await AssertStreamAsync(response, "clients_by_credit_program.xlsx");
    }

    [Test]
    public async Task ExcelDepositByCreditProgram_WhenHaveRecords_ShouldSuccess_Test()
    {
        //Arrange
        var storekeeper = BankDbContext.InsertStorekeeperToDatabaseAndReturn(
            email: $"storekeeper_{Guid.NewGuid()}@email.com",
            login: $"storekeeper_{Guid.NewGuid()}",
            phone: $"+7-{Guid.NewGuid():N}"
        );
        var period = BankDbContext.InsertPeriodToDatabaseAndReturn(storekeeperId: storekeeper.Id);
        
        // Создаем валюты
        var currency1 = BankDbContext.InsertCurrencyToDatabaseAndReturn(
            name: "Рубль",
            abbreviation: "RUB",
            storekeeperId: storekeeper.Id
        );
        var currency2 = BankDbContext.InsertCurrencyToDatabaseAndReturn(
            name: "Доллар",
            abbreviation: "USD",
            storekeeperId: storekeeper.Id
        );

        // Создаем кредитные программы и связываем их с валютами
        var creditProgram1 = BankDbContext.InsertCreditProgramToDatabaseAndReturn(
            name: "Credit Program 1",
            storekeeperId: storekeeper.Id,
            periodId: period.Id,
            creditProgramCurrency: [(currency1.Id, Guid.NewGuid().ToString())]
        );
        var creditProgram2 = BankDbContext.InsertCreditProgramToDatabaseAndReturn(
            name: "Credit Program 2",
            storekeeperId: storekeeper.Id,
            periodId: period.Id,
            creditProgramCurrency: [(currency2.Id, Guid.NewGuid().ToString())]
        );

        var clerk = BankDbContext.InsertClerkToDatabaseAndReturn(
            email: $"clerk_{Guid.NewGuid()}@email.com",
            login: $"clerk_{Guid.NewGuid()}",
            phone: $"+7-{Guid.NewGuid():N}"
        );
        
        // Создаем вклады и связываем их с валютами
        var deposit1 = BankDbContext.InsertDepositToDatabaseAndReturn(
            interestRate: 5.5f,
            cost: 10000,
            period: 12,
            clerkId: clerk.Id
        );
        var deposit2 = BankDbContext.InsertDepositToDatabaseAndReturn(
            interestRate: 6.5f,
            cost: 20000,
            period: 24,
            clerkId: clerk.Id
        );

        // Связываем вклады с валютами
        BankDbContext.DepositCurrencies.Add(new DepositCurrency { DepositId = deposit1.Id, CurrencyId = currency1.Id });
        BankDbContext.DepositCurrencies.Add(new DepositCurrency { DepositId = deposit2.Id, CurrencyId = currency2.Id });
        BankDbContext.SaveChanges();

        //Act
        var response = await HttpClient.GetAsync("/api/report/loadexceldepositbycreditprogram");

        //Assert
        await AssertStreamAsync(response, "deposit_by_credit_program.xlsx");
    }

    private static async Task AssertStreamAsync(HttpResponseMessage response, string fileNameForSave = "")
    {
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        using var data = await response.Content.ReadAsStreamAsync();
        Assert.That(data, Is.Not.Null);
        Assert.That(data.Length, Is.GreaterThan(0));
        await SaveStreamAsync(data, fileNameForSave);
    }
    private static async Task SaveStreamAsync(Stream stream, string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return;
        }
        var path = Path.Combine(Directory.GetCurrentDirectory(), fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        stream.Position = 0;
        using var fileStream = new FileStream(path, FileMode.OpenOrCreate);
        await stream.CopyToAsync(fileStream);
    }
}
