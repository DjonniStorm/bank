using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.StorageContracts;
using BankDatabase.Implementations;
using BankDatabase.Models;
using BankTests.Infrastructure;

namespace BankTests.StorageContactsTests;

[TestFixture]
internal class PeriodStorageContractTests : BaseStorageContractTest
{
    private IPeriodStorageContract _storageContract;

    private string _storekeeperId;

    [SetUp]
    public void SetUp()
    {
        _storageContract = new PeriodStorageContract(BankDbContext);
        _storekeeperId = BankDbContext.InsertStorekeeperToDatabaseAndReturn().Id;
    }

    [TearDown]
    public void TearDown()
    {
        BankDbContext.RemovePeriodsFromDatabase();
        BankDbContext.RemoveStorekeepersFromDatabase();
    }

    [Test]
    public void TryGetListWhenHaveRecords_ShouldSuccess_Test()
    {
        var period = BankDbContext.InsertPeriodToDatabaseAndReturn(storekeeperId: _storekeeperId);
        BankDbContext.InsertPeriodToDatabaseAndReturn(storekeeperId: _storekeeperId);
        BankDbContext.InsertPeriodToDatabaseAndReturn(storekeeperId: _storekeeperId);

        var list = _storageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(3));
        AssertElement(list.First(x => x.Id == period.Id), period);
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
        var period = BankDbContext.InsertPeriodToDatabaseAndReturn(storekeeperId: _storekeeperId);
        AssertElement(_storageContract.GetElementById(period.Id), period);
    }

    [Test]
    public void Try_AddElement_Test()
    {
        var period = CreateModel(storekeeperId: _storekeeperId);
        _storageContract.AddElement(period);
        AssertElement(BankDbContext.GetPeriodFromDatabase(period.Id), period);
    }

    [Test]
    public void Try_AddElement_WhenHaveRecordWithSameId_Test()
    {
        var period = CreateModel(storekeeperId: _storekeeperId);
        BankDbContext.InsertPeriodToDatabaseAndReturn(id: period.Id, storekeeperId: _storekeeperId);
        Assert.That(
            () => _storageContract.AddElement(period),
            Throws.TypeOf<ElementExistsException>()
        );
    }

    [Test]
    public void Try_UpdElement_Test()
    {
        var period = CreateModel(storekeeperId: _storekeeperId);
        BankDbContext.InsertPeriodToDatabaseAndReturn(id: period.Id, storekeeperId: _storekeeperId);
        _storageContract.UpdElement(period);
        AssertElement(BankDbContext.GetPeriodFromDatabase(period.Id), period);
    }

    [Test]
    public void Try_UpdElement_WhenNoRecordWithThisId_Test()
    {
        Assert.That(
            () => _storageContract.UpdElement(CreateModel(storekeeperId: _storekeeperId)),
            Throws.TypeOf<ElementNotFoundException>()
        );
    }

    private static PeriodDataModel CreateModel(
        string? id = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        string? storekeeperId = null
    ) =>
        new(
            id ?? Guid.NewGuid().ToString(),
            startTime ?? DateTime.UtcNow.AddDays(-1),
            endTime ?? DateTime.UtcNow,
            storekeeperId ?? Guid.NewGuid().ToString()
        );

    private static void AssertElement(PeriodDataModel actual, Period? expected)
    {
        Assert.That(expected, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.StartTime, Is.EqualTo(expected.StartTime));
            Assert.That(actual.EndTime, Is.EqualTo(expected.EndTime));
            Assert.That(actual.StorekeeperId, Is.EqualTo(expected.StorekeeperId));
        });
    }

    private static void AssertElement(Period actual, PeriodDataModel? expected)
    {
        Assert.That(expected, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.StartTime, Is.EqualTo(expected.StartTime));
            Assert.That(actual.EndTime, Is.EqualTo(expected.EndTime));
            Assert.That(actual.StorekeeperId, Is.EqualTo(expected.StorekeeperId));
        });
    }
}
