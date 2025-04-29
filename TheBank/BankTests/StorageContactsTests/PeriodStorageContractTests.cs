using BankContracts.DataModels;
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

        var list = _storageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(3));
        AssertElement(list.First(x => x.Id == period.Id), period);
    }

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
}