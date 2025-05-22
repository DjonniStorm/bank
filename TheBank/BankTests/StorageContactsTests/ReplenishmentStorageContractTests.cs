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
        var uniqueId = Guid.NewGuid();
        _clerkId = BankDbContext.InsertClerkToDatabaseAndReturn(
            login: $"clerk_{uniqueId}",
            email: $"clerk_{uniqueId}@email.com",
            phone: $"+7-777-777-{uniqueId.ToString().Substring(0, 4)}"
        ).Id;
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
    public void Try_GetList_WithDateFilters_Test()
    {
        var now = DateTime.UtcNow;
        var pastDate = now.AddDays(-1);
        var futureDate = now.AddDays(1);

        // Insert replenishments with different dates
        var pastReplenishment = BankDbContext.InsertReplenishmentToDatabaseAndReturn(
            clerkId: _clerkId,
            depositId: _depositId,
            date: pastDate
        );
        var currentReplenishment = BankDbContext.InsertReplenishmentToDatabaseAndReturn(
            clerkId: _clerkId,
            depositId: _depositId,
            date: now
        );
        var futureReplenishment = BankDbContext.InsertReplenishmentToDatabaseAndReturn(
            clerkId: _clerkId,
            depositId: _depositId,
            date: futureDate
        );

        // Test date range filter
        var filteredList = _storageContract.GetList(fromDate: pastDate, toDate: now);
        Assert.That(filteredList, Has.Count.EqualTo(2));
        Assert.That(filteredList.Select(x => x.Id), Does.Contain(pastReplenishment.Id));
        Assert.That(filteredList.Select(x => x.Id), Does.Contain(currentReplenishment.Id));
        Assert.That(filteredList.Select(x => x.Id), Does.Not.Contain(futureReplenishment.Id));
    }

    [Test]
    public void Try_GetList_WithClerkIdFilter_Test()
    {
        var otherClerkId = BankDbContext.InsertClerkToDatabaseAndReturn(
            login: $"clerk_other",
            email: "clerk_other@email.com",
            phone: "+7-777-777-0000"
        ).Id;

        // Insert replenishments for different clerks
        var replenishment1 = BankDbContext.InsertReplenishmentToDatabaseAndReturn(
            clerkId: _clerkId,
            depositId: _depositId
        );
        var replenishment2 = BankDbContext.InsertReplenishmentToDatabaseAndReturn(
            clerkId: otherClerkId,
            depositId: _depositId
        );

        var filteredList = _storageContract.GetList(clerkId: _clerkId);
        Assert.That(filteredList, Has.Count.EqualTo(1));
        Assert.That(filteredList.First().Id, Is.EqualTo(replenishment1.Id));
    }

    [Test]
    public void Try_GetList_WithDepositIdFilter_Test()
    {
        var otherDepositId = BankDbContext.InsertDepositToDatabaseAndReturn(clerkId: _clerkId).Id;

        // Insert replenishments for different deposits
        var replenishment1 = BankDbContext.InsertReplenishmentToDatabaseAndReturn(
            clerkId: _clerkId,
            depositId: _depositId
        );
        var replenishment2 = BankDbContext.InsertReplenishmentToDatabaseAndReturn(
            clerkId: _clerkId,
            depositId: otherDepositId
        );

        var filteredList = _storageContract.GetList(depositId: _depositId);
        Assert.That(filteredList, Has.Count.EqualTo(1));
        Assert.That(filteredList.First().Id, Is.EqualTo(replenishment1.Id));
    }

    [Test]
    public void Try_GetList_WithCombinedFilters_Test()
    {
        var now = DateTime.UtcNow;
        var otherClerkId = BankDbContext.InsertClerkToDatabaseAndReturn(
            login: $"clerk_other",
            email: "clerk_other@email.com",
            phone: "+7-777-777-0000"
        ).Id;
        var otherDepositId = BankDbContext.InsertDepositToDatabaseAndReturn(clerkId: _clerkId).Id;

        // Insert replenishments with different combinations
        var replenishment1 = BankDbContext.InsertReplenishmentToDatabaseAndReturn(
            clerkId: _clerkId,
            depositId: _depositId,
            date: now
        );
        var replenishment2 = BankDbContext.InsertReplenishmentToDatabaseAndReturn(
            clerkId: otherClerkId,
            depositId: _depositId,
            date: now
        );
        var replenishment3 = BankDbContext.InsertReplenishmentToDatabaseAndReturn(
            clerkId: _clerkId,
            depositId: otherDepositId,
            date: now
        );

        var filteredList = _storageContract.GetList(
            fromDate: now,
            toDate: now,
            clerkId: _clerkId,
            depositId: _depositId
        );
        Assert.That(filteredList, Has.Count.EqualTo(1));
        Assert.That(filteredList.First().Id, Is.EqualTo(replenishment1.Id));
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
