using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.StorageContracts;
using BankDatabase.Implementations;
using BankDatabase.Models;
using BankTests.Infrastructure;

namespace BankTests.StorageContactsTests;

[TestFixture]
internal class StorekeeperStorageContractTests : BaseStorageContractTest
{
    private IStorekeeperStorageContract _storageContract;

    [SetUp]
    public void SetUp()
    {
        _storageContract = new StorekeeperStorageContract(BankDbContext);
    }

    [TearDown]
    public void TearDown()
    {
        BankDbContext.RemoveStorekeepersFromDatabase();
    }

    [Test]
    public void TryGetListWhenHaveRecords_ShouldSuccess_Test()
    {
        var storekeeper = BankDbContext.InsertStorekeeperToDatabaseAndReturn();
        BankDbContext.InsertStorekeeperToDatabaseAndReturn(
            login: "xomyak2",
            email: "email1@email.com",
            phone: "+9-888-888-88-68"
        );
        BankDbContext.InsertStorekeeperToDatabaseAndReturn(
            login: "xomyak3",
            email: "email3@email.com",
            phone: "+9-888-888-88-78"
        );

        var list = _storageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(3));
        AssertElement(list.First(x => x.Id == storekeeper.Id), storekeeper);
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
        var storekeeper = BankDbContext.InsertStorekeeperToDatabaseAndReturn();
        AssertElement(_storageContract.GetElementById(storekeeper.Id), storekeeper);
    }

    [Test]
    public void Try_AddElement_Test()
    {
        var storekeeper = CreateModel();
        _storageContract.AddElement(storekeeper);
        AssertElement(BankDbContext.GetStorekeeperFromDatabase(storekeeper.Id), storekeeper);
    }

    [Test]
    public void Try_AddElement_WhenHaveRecordWithSameId_Test()
    {
        var storekeeper = CreateModel();
        BankDbContext.InsertStorekeeperToDatabaseAndReturn(id: storekeeper.Id);
        Assert.That(
            () => _storageContract.AddElement(storekeeper),
            Throws.TypeOf<ElementExistsException>()
        );
    }

    [Test]
    public void Try_AddElement_WhenHaveRecordWithSameEmail_Test()
    {
        var storekeeper = CreateModel();
        BankDbContext.InsertStorekeeperToDatabaseAndReturn(email: storekeeper.Email);
        Assert.That(
            () => _storageContract.AddElement(storekeeper),
            Throws.TypeOf<ElementExistsException>()
        );
    }

    [Test]
    public void Try_AddElement_WhenHaveRecordWithSameLogin_Test()
    {
        var storekeeper = CreateModel(login: "cheburek");
        BankDbContext.InsertStorekeeperToDatabaseAndReturn(
            email: "email@email.ru",
            login: "cheburek"
        );
        Assert.That(
            () => _storageContract.AddElement(storekeeper),
            Throws.TypeOf<ElementExistsException>()
        );
    }

    [Test]
    public void Try_UpdElement_Test()
    {
        var storekeeper = CreateModel();
        BankDbContext.InsertStorekeeperToDatabaseAndReturn(storekeeper.Id, name: "Женя");
        _storageContract.UpdElement(storekeeper);
        AssertElement(BankDbContext.GetStorekeeperFromDatabase(storekeeper.Id), storekeeper);
    }

    [Test]
    public void Try_UpdElement_WhenNoRecordWithThisId_Test()
    {
        Assert.That(
            () => _storageContract.UpdElement(CreateModel()),
            Throws.TypeOf<ElementNotFoundException>()
        );
    }

    private static StorekeeperDataModel CreateModel(
        string? id = null,
        string? name = "vasya",
        string? surname = "petrov",
        string? middlename = "petrovich",
        string? login = "vasyapupkin",
        string? passwd = "*******",
        string? email = "email@email.com",
        string? phone = "+7-777-777-77-77"
    ) =>
        new(
            id ?? Guid.NewGuid().ToString(),
            name,
            surname,
            middlename,
            login,
            passwd,
            email,
            phone
        );

    private static void AssertElement(StorekeeperDataModel actual, Storekeeper? expected)
    {
        Assert.That(expected, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.Name, Is.EqualTo(expected.Name));
            Assert.That(actual.MiddleName, Is.EqualTo(expected.MiddleName));
            Assert.That(actual.Login, Is.EqualTo(expected.Login));
            Assert.That(actual.Password, Is.EqualTo(expected.Password));
            Assert.That(actual.Email, Is.EqualTo(expected.Email));
            Assert.That(actual.PhoneNumber, Is.EqualTo(expected.PhoneNumber));
        });
    }

    private static void AssertElement(Storekeeper actual, StorekeeperDataModel? expected)
    {
        Assert.That(expected, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.Name, Is.EqualTo(expected.Name));
            Assert.That(actual.MiddleName, Is.EqualTo(expected.MiddleName));
            Assert.That(actual.Login, Is.EqualTo(expected.Login));
            Assert.That(actual.Password, Is.EqualTo(expected.Password));
            Assert.That(actual.Email, Is.EqualTo(expected.Email));
            Assert.That(actual.PhoneNumber, Is.EqualTo(expected.PhoneNumber));
        });
    }
}
