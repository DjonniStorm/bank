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
        _clerkId = BankDbContext.InsertClerkToDatabaseAndReturn().Id;
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
