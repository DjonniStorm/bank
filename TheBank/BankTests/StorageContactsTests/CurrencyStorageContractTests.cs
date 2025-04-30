using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.StorageContracts;
using BankDatabase.Implementations;
using BankDatabase.Models;
using BankTests.Infrastructure;

namespace BankTests.StorageContactsTests;

[TestFixture]
internal class CurrencyStorageContractTests : BaseStorageContractTest
{
    private ICurrencyStorageContract _storageContract;

    private string _storekeeperId;

    [SetUp]
    public void SetUp()
    {
        _storageContract = new CurrencyStorageContract(BankDbContext);
        _storekeeperId = BankDbContext.InsertStorekeeperToDatabaseAndReturn().Id;
    }

    [TearDown]
    public void TearDown()
    {
        BankDbContext.RemoveCurrenciesFromDatabase();
        BankDbContext.RemoveStorekeepersFromDatabase();
    }

    [Test]
    public void TryGetListWhenHaveRecords_ShouldSuccess_Test()
    {
        var currency = BankDbContext.InsertCurrencyToDatabaseAndReturn(
            storekeeperId: _storekeeperId
        );
        BankDbContext.InsertCurrencyToDatabaseAndReturn(
            abbreviation: "$",
            storekeeperId: _storekeeperId
        );
        BankDbContext.InsertCurrencyToDatabaseAndReturn(
            abbreviation: "eur",
            storekeeperId: _storekeeperId
        );

        var list = _storageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(3));
        AssertElement(_storageContract.GetElementById(currency.Id), currency);
    }

    [Test]
    public void Try_GetElementById_WhenHaveRecord_Test()
    {
        var currency = BankDbContext.InsertCurrencyToDatabaseAndReturn(
            storekeeperId: _storekeeperId
        );
        AssertElement(_storageContract.GetElementById(currency.Id), currency);
    }

    [Test]
    public void Try_GetElementByAbbreviation_WhenHaveRecord_Test()
    {
        var currency = BankDbContext.InsertCurrencyToDatabaseAndReturn(
            abbreviation: "X",
            storekeeperId: _storekeeperId
        );
        AssertElement(_storageContract.GetElementByAbbreviation(currency.Abbreviation), currency);
    }

    [Test]
    public void Try_GetList_WhenNoRecords_Test()
    {
        var list = _storageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.Empty);
    }

    [Test]
    public void Try_AddElement_Test()
    {
        var currency = CreateModel(storekeeperId: _storekeeperId);
        _storageContract.AddElement(currency);
        AssertElement(BankDbContext.GetCurrencyFromDatabase(currency.Id), currency);
    }

    [Test]
    public void Try_AddElement_WhenHaveRecordWithSameId_Test()
    {
        var currency = CreateModel(storekeeperId: _storekeeperId);
        BankDbContext.InsertCurrencyToDatabaseAndReturn(
            id: currency.Id,
            storekeeperId: _storekeeperId
        );
        Assert.That(
            () => _storageContract.AddElement(currency),
            Throws.TypeOf<ElementExistsException>()
        );
    }

    [Test]
    public void Try_AddElement_WhenHaveRecordWithSameAbbreviation_Test()
    {
        var currency = CreateModel(storekeeperId: _storekeeperId, abbreviation: "хамстер коин");
        BankDbContext.InsertCurrencyToDatabaseAndReturn(
            storekeeperId: _storekeeperId,
            abbreviation: currency.Abbreviation
        );
        Assert.That(
            () => _storageContract.AddElement(currency),
            Throws.TypeOf<ElementExistsException>()
        );
    }

    [Test]
    public void Try_UpdElement_Test()
    {
        var currency = CreateModel(storekeeperId: _storekeeperId, abbreviation: "хамстер коин");
        BankDbContext.InsertCurrencyToDatabaseAndReturn(
            id: currency.Id,
            storekeeperId: _storekeeperId
        );
        _storageContract.UpdElement(currency);
        AssertElement(BankDbContext.GetCurrencyFromDatabase(currency.Id), currency);
    }

    [Test]
    public void Try_UpdElement_WhenNoRecordWithThisId_Test()
    {
        Assert.That(
            () => _storageContract.UpdElement(CreateModel()),
            Throws.TypeOf<ElementNotFoundException>()
        );
    }

    private static CurrencyDataModel CreateModel(
        string? id = null,
        string? name = "pop",
        string? abbreviation = "rub",
        decimal cost = 10,
        string? storekeeperId = null
    ) =>
        new(
            id ?? Guid.NewGuid().ToString(),
            name,
            abbreviation,
            cost,
            storekeeperId ?? Guid.NewGuid().ToString()
        );

    private static void AssertElement(CurrencyDataModel actual, Currency? expected)
    {
        Assert.That(expected, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.Name, Is.EqualTo(expected.Name));
            Assert.That(actual.Abbreviation, Is.EqualTo(expected.Abbreviation));
            Assert.That(actual.Cost, Is.EqualTo(expected.Cost));
            Assert.That(actual.StorekeeperId, Is.EqualTo(expected.StorekeeperId));
        });
    }

    private static void AssertElement(Currency actual, CurrencyDataModel? expected)
    {
        Assert.That(expected, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.Name, Is.EqualTo(expected.Name));
            Assert.That(actual.Abbreviation, Is.EqualTo(expected.Abbreviation));
            Assert.That(actual.Cost, Is.EqualTo(expected.Cost));
            Assert.That(actual.StorekeeperId, Is.EqualTo(expected.StorekeeperId));
        });
    }
}
