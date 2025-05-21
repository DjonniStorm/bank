using System.Xml.Linq;
using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.StorageContracts;
using BankDatabase.Implementations;
using BankDatabase.Models;
using BankTests.Infrastructure;

namespace BankTests.StorageContactsTests;

[TestFixture]
internal class CreditProgramStorageContractTests : BaseStorageContractTest
{
    private ICreditProgramStorageContract _storageContract;

    private string _storekeeperId;

    private string _periodId;

    private string _currenyId;

    [SetUp]
    public void SetUp()
    {
        _storageContract = new CreditProgramStorageContract(BankDbContext);
        _storekeeperId = BankDbContext.InsertStorekeeperToDatabaseAndReturn().Id;
        _periodId = BankDbContext.InsertPeriodToDatabaseAndReturn(storekeeperId: _storekeeperId).Id;
        _currenyId = BankDbContext.InsertCurrencyToDatabaseAndReturn(storekeeperId: _storekeeperId).Id;
    }

    [TearDown]
    public void TearDown()
    {
        BankDbContext.RemoveCreditProgramsFromDatabase();
        BankDbContext.RemovePeriodsFromDatabase();
        BankDbContext.RemoveCurrenciesFromDatabase();
        BankDbContext.RemoveStorekeepersFromDatabase();
    }

    [Test]
    public void TryGetListWhenHaveRecords_ShouldSucces_Test()
    {
        var creditProgramId = Guid.NewGuid().ToString();
        var creditProgram = BankDbContext.InsertCreditProgramToDatabaseAndReturn(
            storekeeperId: _storekeeperId,
            periodId: _periodId,
            creditProgramCurrency: [( _currenyId, creditProgramId )]
        );
        BankDbContext.InsertCreditProgramToDatabaseAndReturn(
            name: "bankrot2",
            storekeeperId: _storekeeperId,
            periodId: _periodId
        );
        BankDbContext.InsertCreditProgramToDatabaseAndReturn(
            name: "bankrot3",
            storekeeperId: _storekeeperId,
            periodId: _periodId
        );

        var list = _storageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(3));
        AssertElement(list.First(x => x.Id == creditProgram.Id), creditProgram);
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
        var creditProgram = BankDbContext.InsertCreditProgramToDatabaseAndReturn(
            storekeeperId: _storekeeperId,
            periodId: _periodId
        );
        AssertElement(_storageContract.GetElementById(creditProgram.Id), creditProgram);
    }

    [Test]
    public void Try_AddElement_Test()
    {
        var credit = CreateModel(
            name: "unique name",
            periodId: _periodId,
            storekeeperId: _storekeeperId
        );
        _storageContract.AddElement(credit);
        AssertElement(BankDbContext.GetCreditProgramFromDatabase(credit.Id), credit);
    }

    [Test]
    public void Try_AddElement_WhenHaveRecordWithSameName_Test()
    {
        var credit = CreateModel(name: "1", storekeeperId: _storekeeperId, periodId: _periodId);
        BankDbContext.InsertCreditProgramToDatabaseAndReturn(
            name: "1",
            periodId: _periodId,
            storekeeperId: _storekeeperId,
            creditProgramCurrency: [(_currenyId, credit.Id)]
        );
        Assert.That(
            () => _storageContract.AddElement(credit),
            Throws.TypeOf<ElementExistsException>()
        );
    }

    [Test]
    public void Try_UpdElement_Test()
    {
        var credit = CreateModel(
            name: "unique name",
            periodId: _periodId,
            storekeeperId: _storekeeperId
        );
        BankDbContext.InsertCreditProgramToDatabaseAndReturn(
            credit.Id,
            periodId: _periodId,
            storekeeperId: _storekeeperId,
            creditProgramCurrency: [(_currenyId, credit.Id)]
        );
        _storageContract.UpdElement(credit);
        AssertElement(BankDbContext.GetCreditProgramFromDatabase(credit.Id), credit);
    }

    [Test]
    public void Try_UpdElement_WhenNoRecordWithThisId_Test()
    {
        Assert.That(
            () =>
                _storageContract.UpdElement(
                    CreateModel(storekeeperId: _storekeeperId, periodId: _periodId)
                ),
            Throws.TypeOf<ElementNotFoundException>()
        );
    }

    private static CreditProgramDataModel CreateModel(
        string? id = null,
        string? name = "name",
        decimal cost = 1,
        decimal maxCost = 2,
        string? storekeeperId = null,
        string? periodId = null,
        List<CreditProgramCurrencyDataModel>? currency = null
    ) =>
        new(
            id ?? Guid.NewGuid().ToString(),
            name,
            cost,
            maxCost,
            storekeeperId ?? Guid.NewGuid().ToString(),
            periodId ?? Guid.NewGuid().ToString(),
            currency ?? []
        );

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
        if (actual.Currencies is not null && actual.Currencies.Count > 0)
        {
            Assert.That(expected.CurrencyCreditPrograms, Is.Not.Null);
            Assert.That(
                actual.Currencies,
                Has.Count.EqualTo(expected.CurrencyCreditPrograms.Count)
            );
            for (int i = 0; i < actual.Currencies.Count; ++i)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(
                        actual.Currencies[i].CreditProgramId,
                        Is.EqualTo(expected.CurrencyCreditPrograms[i].CreditProgramId)
                    );
                    Assert.That(
                        actual.Currencies[i].CurrencyId,
                        Is.EqualTo(expected.CurrencyCreditPrograms[i].CurrencyId)
                    );
                });
            }
        }
        else
        {
            Assert.That(expected.CurrencyCreditPrograms, Is.Null);
        }
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
        if (actual.CurrencyCreditPrograms is not null && actual.CurrencyCreditPrograms.Count > 0)
        {
            Assert.That(expected.Currencies, Is.Not.Null);
            Assert.That(
                actual.CurrencyCreditPrograms,
                Has.Count.EqualTo(expected.Currencies.Count)
            );
            for (int i = 0;i < actual.CurrencyCreditPrograms.Count;++i)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(
                        actual.CurrencyCreditPrograms[i].CreditProgramId,
                        Is.EqualTo(expected.Currencies[i].CreditProgramId)
                    );
                    Assert.That(
                        actual.CurrencyCreditPrograms[i].CurrencyId,
                        Is.EqualTo(expected.Currencies[i].CurrencyId)
                    );
                });
            }
        }
        else
        {
            Assert.That(expected.Currencies, Is.Empty);
        }
    }
}
