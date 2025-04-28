using BankContracts.DataModels;
using BankContracts.StorageContracts;
using BankDatabase.Implementations;
using BankDatabase.Models;
using BankTests.Infrastructure;
using System.Xml.Linq;

namespace BankTests.StorageContactsTests;

[TestFixture]
internal class CreditProgramStorageContractTests : BaseStorageContractTest
{
    private ICreditProgramStorageContract _storageContract;

    private string _storekeeperId;

    private string _periodId;

    [SetUp]
    public void SetUp()
    {
        _storageContract = new CreditProgramStorageContract(BankDbContext);
        _storekeeperId = BankDbContext.InsertStorekeeperToDatabaseAndReturn().Id;
        _periodId = BankDbContext.InsertPeriodToDatabaseAndReturn(storekeeperId: _storekeeperId).Id;
    }

    [TearDown]
    public void TearDown()
    {
        BankDbContext.RemoveCreditProgramsFromDatabase();
        BankDbContext.RemovePeriodsFromDatabase();
        BankDbContext.RemoveStorekeepersFromDatabase();
    }

    [Test]
    public void TryGetListWhenHaveRecords_ShouldSucces_Test()
    {
        var clerk = BankDbContext.InsertCreditProgramToDatabaseAndReturn(storeleeperId: _storekeeperId, periodId: _periodId);
        BankDbContext.InsertCreditProgramToDatabaseAndReturn(name: "bankrot2", storeleeperId: _storekeeperId, periodId: _periodId);
        BankDbContext.InsertCreditProgramToDatabaseAndReturn(name: "bankrot3", storeleeperId: _storekeeperId, periodId: _periodId);

        var list = _storageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(3));
        AssertElement(list.First(x => x.Id == clerk.Id), clerk);
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
        var creditProgram = BankDbContext.InsertCreditProgramToDatabaseAndReturn(storeleeperId: _storekeeperId, periodId: _periodId);
        AssertElement(_storageContract.GetElementById(creditProgram.Id), creditProgram);
    }

    [Test]
    public void Try_AddElement_Test()
    {
        var credit = CreateModel(name: "unique name", periodId: _periodId, storekeeperId: _storekeeperId);
        _storageContract.AddElement(credit);
        AssertElement(BankDbContext.GetCreditProgramFromDatabase(credit.Id), credit);
    }

    private static CreditProgramDataModel CreateModel(string? id = null, string? name = "name", decimal cost = 1, decimal maxCost = 2, string? storekeeperId = null, string? periodId = null, List<CreditProgramCurrencyDataModel>? currency = null) 
        => new(id ?? Guid.NewGuid().ToString(), name, cost, maxCost, storekeeperId ?? Guid.NewGuid().ToString(), periodId ?? Guid.NewGuid().ToString(), currency ?? []);

    private static void AssertElement(CreditProgramDataModel actual, CreditProgram? expected)
    {
        Assert.That(expected, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.Name, Is.EqualTo(expected.Name));
            Assert.That(actual.Cost, Is.EqualTo(expected.Cost));
            Assert.That(actual.MaxCost, Is.EqualTo(expected.MaxCost));
            Assert.That(actual.StorekeeperId, Is.EqualTo(expected.StorekeeperId));
            Assert.That(actual.PeriodId, Is.EqualTo(expected.PeriodId));
        });
    }

    private static void AssertElement(CreditProgram actual, CreditProgramDataModel? expected)
    {
        Assert.That(expected, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.Name, Is.EqualTo(expected.Name));
            Assert.That(actual.Cost, Is.EqualTo(expected.Cost));
            Assert.That(actual.MaxCost, Is.EqualTo(expected.MaxCost));
            Assert.That(actual.StorekeeperId, Is.EqualTo(expected.StorekeeperId));
            Assert.That(actual.PeriodId, Is.EqualTo(expected.PeriodId));
        });
    }
}
