using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.StorageContracts;
using BankDatabase.Implementations;
using BankDatabase.Models;
using BankTests.Infrastructure;

namespace BankTests.StorageContactsTests;

[TestFixture]
internal class ReplenishmentStorageContractTests : BaseStorageContractTest
{
    private IReplenishmentStorageContract _storageContract;

    private string _depositId;

    private string _clerkId;

    [SetUp]
    public void SetUp()
    {
        _storageContract = new ReplenishmentStorageContract(BankDbContext);
        _clerkId = BankDbContext.InsertClerkToDatabaseAndReturn().Id;
        _depositId = BankDbContext.InsertDepositToDatabaseAndReturn(clerkId: _clerkId).Id;
    }

    [TearDown]
    public void TearDown()
    {
        BankDbContext.RemoveReplenishmentsFromDatabase();
        BankDbContext.RemoveDepositsFromDatabase();
        BankDbContext.RemoveClerksFromDatabase();
    }

    [Test]
    public void TryGetListWhenHaveRecords_ShouldSuccess_Test()
    {
        var replenishment = BankDbContext.InsertReplenishmentToDatabaseAndReturn(
            clerkId: _clerkId,
            depositId: _depositId
        );
        BankDbContext.InsertReplenishmentToDatabaseAndReturn(
            clerkId: _clerkId,
            depositId: _depositId
        );
        BankDbContext.InsertReplenishmentToDatabaseAndReturn(
            clerkId: _clerkId,
            depositId: _depositId
        );

        var list = _storageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(3));
        AssertElement(list.First(x => x.Id == replenishment.Id), replenishment);
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
        var replenishment = BankDbContext.InsertReplenishmentToDatabaseAndReturn(
            clerkId: _clerkId,
            depositId: _depositId
        );
        AssertElement(_storageContract.GetElementById(replenishment.Id), replenishment);
    }

    [Test]
    public void Try_AddElement_Test()
    {
        var replenishment = CreateModel(clerkId: _clerkId, depositId: _depositId);
        _storageContract.AddElement(replenishment);
        AssertElement(BankDbContext.GetReplenishmentFromDatabase(replenishment.Id), replenishment);
    }

    [Test]
    public void Try_AddElement_WhenHaveRecordWithSameId_Test()
    {
        var replenishment = CreateModel(clerkId: _clerkId, depositId: _depositId);
        BankDbContext.InsertReplenishmentToDatabaseAndReturn(
            id: replenishment.Id,
            clerkId: _clerkId,
            depositId: _depositId
        );
        Assert.That(
            () => _storageContract.AddElement(replenishment),
            Throws.TypeOf<ElementExistsException>()
        );
    }

    [Test]
    public void Try_UpdElement_Test()
    {
        var replenishment = CreateModel(clerkId: _clerkId, depositId: _depositId);
        BankDbContext.InsertReplenishmentToDatabaseAndReturn(
            id: replenishment.Id,
            clerkId: _clerkId,
            depositId: _depositId
        );
        _storageContract.UpdElement(replenishment);
        AssertElement(BankDbContext.GetReplenishmentFromDatabase(replenishment.Id), replenishment);
    }

    [Test]
    public void Try_UpdElement_WhenNoRecordWithThisId_Test()
    {
        Assert.That(
            () =>
                _storageContract.UpdElement(CreateModel(clerkId: _clerkId, depositId: _depositId)),
            Throws.TypeOf<ElementNotFoundException>()
        );
    }

    private static ReplenishmentDataModel CreateModel(
        string? id = null,
        decimal amount = 1,
        DateTime? date = null,
        string? depositId = null,
        string? clerkId = null
    ) =>
        new(
            id ?? Guid.NewGuid().ToString(),
            amount,
            date ?? DateTime.UtcNow,
            depositId ?? Guid.NewGuid().ToString(),
            clerkId ?? Guid.NewGuid().ToString()
        );

    private static void AssertElement(ReplenishmentDataModel actual, Replenishment? expected)
    {
        Assert.That(expected, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.Amount, Is.EqualTo(expected.Amount));
            Assert.That(actual.Date, Is.EqualTo(expected.Date));
            Assert.That(actual.DepositId, Is.EqualTo(expected.DepositId));
            Assert.That(actual.ClerkId, Is.EqualTo(expected.ClerkId));
        });
    }

    private static void AssertElement(Replenishment actual, ReplenishmentDataModel? expected)
    {
        Assert.That(expected, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.Amount, Is.EqualTo(expected.Amount));
            Assert.That(actual.Date, Is.EqualTo(expected.Date));
            Assert.That(actual.DepositId, Is.EqualTo(expected.DepositId));
            Assert.That(actual.ClerkId, Is.EqualTo(expected.ClerkId));
        });
    }
}
