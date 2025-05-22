using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.StorageContracts;
using BankDatabase.Implementations;
using BankDatabase.Models;
using BankTests.Infrastructure;

namespace BankTests.StorageContactsTests;

[TestFixture]
internal class DepositStorageContractTests : BaseStorageContractTest
{
    private IDepositStorageContract _storageContract;

    private string _clerkId;

    [SetUp]
    public void SetUp()
    {
        _storageContract = new DepositStorageContract(BankDbContext);
        var uniqueId = Guid.NewGuid();
        _clerkId = BankDbContext.InsertClerkToDatabaseAndReturn(
            login: $"clerk_{uniqueId}",
            email: $"clerk_{uniqueId}@email.com",
            phone: $"+7-777-777-{uniqueId.ToString().Substring(0, 4)}"
        ).Id;
    }

    [TearDown]
    public void TearDown()
    {
        BankDbContext.RemoveDepositsFromDatabase();
        BankDbContext.RemoveClerksFromDatabase();
    }

    [Test]
    public void TryGetListWhenHaveRecords_ShouldSuccess_Test()
    {
        var deposit = BankDbContext.InsertDepositToDatabaseAndReturn(clerkId: _clerkId);
        BankDbContext.InsertDepositToDatabaseAndReturn(clerkId: _clerkId);
        BankDbContext.InsertDepositToDatabaseAndReturn(clerkId: _clerkId);

        var list = _storageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(3));
        AssertElement(list.First(x => x.Id == deposit.Id), deposit);
    }

    [Test]
    public void Try_GetList_WhenNoRecords_Test()
    {
        var list = _storageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.Empty);
    }

    [Test]
    public void Try_GetElementById_WhenHaveRecord_Test()
    {
        var deposit = BankDbContext.InsertDepositToDatabaseAndReturn(clerkId: _clerkId);
        AssertElement(_storageContract.GetElementById(deposit.Id), deposit);
    }

    [Test]
    public void Try_AddElement_Test()
    {
        var deposit = CreateModel(clerkId: _clerkId);
        _storageContract.AddElement(deposit);
        AssertElement(BankDbContext.GetDepositFromDatabase(deposit.Id), deposit);
    }

    [Test]
    public void Try_AddElement_WhenHaveRecordWithSameId_Test()
    {
        var deposit = CreateModel(clerkId: _clerkId);
        BankDbContext.InsertDepositToDatabaseAndReturn(id: deposit.Id, clerkId: _clerkId);
        Assert.That(
            () => _storageContract.AddElement(deposit),
            Throws.TypeOf<ElementExistsException>()
        );
    }

    [Test]
    public void Try_UpdElement_Test()
    {
        var deposit = CreateModel(clerkId: _clerkId);
        BankDbContext.InsertDepositToDatabaseAndReturn(id: deposit.Id, clerkId: _clerkId);
        _storageContract.UpdElement(deposit);
        AssertElement(BankDbContext.GetDepositFromDatabase(deposit.Id), deposit);
    }

    [Test]
    public void Try_UpdElement_WhenNoRecordWithThisId_Test()
    {
        Assert.That(
            () => _storageContract.UpdElement(CreateModel(clerkId: _clerkId)),
            Throws.TypeOf<ElementNotFoundException>()
        );
    }

    [Test]
    public void Try_GetList_ByClerkId_Test()
    {
        var uniqueId1 = Guid.NewGuid();
        var uniqueId2 = Guid.NewGuid();
        var clerkId1 = BankDbContext.InsertClerkToDatabaseAndReturn(
            email: $"clerk1_{uniqueId1}@email.com",
            login: $"clerk1_{uniqueId1}",
            phone: $"+7-777-777-{uniqueId1.ToString().Substring(0, 4)}"
        ).Id;
        var clerkId2 = BankDbContext.InsertClerkToDatabaseAndReturn(
            email: $"clerk2_{uniqueId2}@email.com",
            login: $"clerk2_{uniqueId2}",
            phone: $"+7-777-777-{uniqueId2.ToString().Substring(0, 4)}"
        ).Id;
        
        BankDbContext.InsertDepositToDatabaseAndReturn(clerkId: clerkId1);
        BankDbContext.InsertDepositToDatabaseAndReturn(clerkId: clerkId1);
        BankDbContext.InsertDepositToDatabaseAndReturn(clerkId: clerkId2);

        var list = _storageContract.GetList(clerkId1);
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(2));
        Assert.That(list.All(x => x.ClerkId == clerkId1), Is.True);
    }

    [Test]
    public void Try_GetElementByInterestRate_WhenHaveRecord_Test()
    {
        var deposit = BankDbContext.InsertDepositToDatabaseAndReturn(clerkId: _clerkId, interestRate: 15.5f);
        var result = _storageContract.GetElementByInterestRate(15.5f);
        Assert.That(result, Is.Not.Null);
        AssertElement(result, deposit);
    }

    [Test]
    public void Try_GetElementByInterestRate_WhenNoRecord_Test()
    {
        var result = _storageContract.GetElementByInterestRate(15.5f);
        Assert.That(result, Is.Null);
    }

    [Test]
    public void Try_AddElement_WhenHaveRecordWithSameInterestRate_Test()
    {
        // Создаем первый депозит с определенной процентной ставкой
        var deposit1 = CreateModel(clerkId: _clerkId, interestRate: 10.5f);
        _storageContract.AddElement(deposit1);

        // Создаем второй депозит с такой же процентной ставкой
        var deposit2 = CreateModel(clerkId: _clerkId, interestRate: 10.5f);
        
        // Проверяем, что можно добавить депозит с такой же процентной ставкой
        _storageContract.AddElement(deposit2);
        var result = BankDbContext.GetDepositFromDatabase(deposit2.Id);
        Assert.That(result, Is.Not.Null);
        AssertElement(result, deposit2);
    }

    [Test]
    public void Try_UpdElement_WithCurrencies_Test()
    {
        // Создаем валюты
        var storekeeperId = BankDbContext.InsertStorekeeperToDatabaseAndReturn(
            login: $"storekeeper_{Guid.NewGuid()}",
            email: $"storekeeper_{Guid.NewGuid()}@email.com",
            phone: $"+7-777-777-{Guid.NewGuid().ToString().Substring(0, 4)}"
        ).Id;
        
        var currency1 = BankDbContext.InsertCurrencyToDatabaseAndReturn(
            abbreviation: "USD",
            storekeeperId: storekeeperId
        );
        var currency2 = BankDbContext.InsertCurrencyToDatabaseAndReturn(
            abbreviation: "EUR",
            storekeeperId: storekeeperId
        );

        // Создаем депозит
        var deposit = BankDbContext.InsertDepositToDatabaseAndReturn(clerkId: _clerkId);
        
        // Обновляем депозит с валютами
        var updatedDeposit = CreateModel(
            id: deposit.Id,
            clerkId: _clerkId,
            deposits: new List<DepositCurrencyDataModel>
            {
                new(deposit.Id, currency1.Id),
                new(deposit.Id, currency2.Id)
            }
        );
        
        _storageContract.UpdElement(updatedDeposit);
        var result = BankDbContext.GetDepositFromDatabase(deposit.Id);
        Assert.That(result.DepositCurrencies, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task Try_GetListAsync_ByDateRange_Test()
    {
        var startDate = DateTime.Now.AddDays(-1);
        var endDate = DateTime.Now.AddDays(1);
        
        var deposit = BankDbContext.InsertDepositToDatabaseAndReturn(clerkId: _clerkId);
        
        var list = await _storageContract.GetListAsync(startDate, endDate, CancellationToken.None);
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(1));
        AssertElement(list.First(), deposit);
    }

    [Test]
    public void Try_AddElement_WhenDatabaseError_Test()
    {
        var deposit = CreateModel(clerkId: _clerkId);
        // Симулируем ошибку базы данных, пытаясь добавить депозит с несуществующим ID клерка
        var nonExistentClerkId = Guid.NewGuid().ToString();
        var depositWithInvalidClerk = CreateModel(clerkId: nonExistentClerkId);
        
        Assert.That(
            () => _storageContract.AddElement(depositWithInvalidClerk),
            Throws.TypeOf<StorageException>()
        );
    }

    private static DepositDataModel CreateModel(
        string? id = null,
        float interestRate = 10,
        decimal cost = 10,
        int period = 10,
        string? clerkId = null,
        List<DepositCurrencyDataModel>? deposits = null
    ) =>
        new(
            id ?? Guid.NewGuid().ToString(),
            interestRate,
            cost,
            period,
            clerkId ?? Guid.NewGuid().ToString(),
            deposits ?? []
        );

    private static void AssertElement(DepositDataModel actual, Deposit? expected)
    {
        Assert.That(expected, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.InterestRate, Is.EqualTo(expected.InterestRate));
            Assert.That(actual.Cost, Is.EqualTo(expected.Cost));
            Assert.That(actual.Period, Is.EqualTo(expected.Period));
            Assert.That(actual.ClerkId, Is.EqualTo(expected.ClerkId));
        });
    }

    private static void AssertElement(Deposit actual, DepositDataModel? expected)
    {
        Assert.That(expected, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.InterestRate, Is.EqualTo(expected.InterestRate));
            Assert.That(actual.Cost, Is.EqualTo(expected.Cost));
            Assert.That(actual.Period, Is.EqualTo(expected.Period));
            Assert.That(actual.ClerkId, Is.EqualTo(expected.ClerkId));
        });
    }
}
