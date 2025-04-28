using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.StorageContracts;
using BankDatabase.Implementations;
using BankDatabase.Models;
using BankTests.Infrastructure;

namespace BankTests.StorageContactsTests;

[TestFixture]
internal class ClerkStorageContractTests : BaseStorageContractTest
{
    private IClerkStorageContract _storageContract;

    [SetUp]
    public void SetUp()
    {
        _storageContract = new ClerkStorageContract(BankDbContext);
    }

    [TearDown]
    public void TearDown() 
    {
        BankDbContext.RemoveClerksFromDatabase();
    }

    [Test]
    public void TryGetListWhenHaveRecords_ShouldSucces_Test()
    {
        var clerk = BankDbContext.InsertClerkToDatabaseAndReturn();
        BankDbContext.InsertClerkToDatabaseAndReturn(login: "xomyak", email: "email1@email.com", phone: "+9-888-888-88-88");
        BankDbContext.InsertClerkToDatabaseAndReturn(login: "xomyak3", email: "email3@email.com", phone: "+9-888-888-88-78");

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
        var clerk = BankDbContext.InsertClerkToDatabaseAndReturn();
        AssertElement(_storageContract.GetElementById(clerk.Id), clerk);
    }

    [Test]
    public void Try_AddElement_Test()
    {
        var clerk = CreateModel();
        _storageContract.AddElement(clerk);
        AssertElement(BankDbContext.GetClerkFromDatabase(clerk.Id), clerk);
    }

    [Test]
    public void Try_AddElement_WhenHaveRecordWithSameId_Test()
    {
        var clerk = CreateModel();
        BankDbContext.InsertClerkToDatabaseAndReturn(id: clerk.Id);
        Assert.That(() => _storageContract.AddElement(clerk), Throws.TypeOf<ElementExistsException>());
    }

    [Test]
    public void Try_AddElement_WhenHaveRecordWithSameEmail_Test()
    {
        var clerk = CreateModel();
        BankDbContext.InsertClerkToDatabaseAndReturn(email: clerk.Email);
        Assert.That(() => _storageContract.AddElement(clerk), Throws.TypeOf<ElementExistsException>());
    }

    [Test]
    public void Try_AddElement_WhenHaveRecordWithSameLogin_Test()
    {
        var clerk = CreateModel(login: "cheburek");
        BankDbContext.InsertClerkToDatabaseAndReturn(email: "email@email.ru", login: "cheburek");
        Assert.That(() => _storageContract.AddElement(clerk), Throws.TypeOf<ElementExistsException>());
    }

    [Test]
    public void Try_UpdElement_Test()
    {
        var clerk = CreateModel();
        BankDbContext.InsertClerkToDatabaseAndReturn(clerk.Id, name: "Женя");
        _storageContract.UpdElement(clerk);
        AssertElement(BankDbContext.GetClerkFromDatabase(clerk.Id), clerk);
    }

    [Test]
    public void Try_UpdElement_WhenNoRecordWithThisId_Test()
    {
        Assert.That(() => _storageContract.UpdElement(CreateModel()), Throws.TypeOf<ElementNotFoundException>());
    }

    private static ClerkDataModel CreateModel(string? id = null, string? name = "vasya", string? surname = "petrov", string? middlename = "petrovich", string? login = "vasyapupkin", string? passwd = "*******", string? email = "email@email.com", string? phone = "+7-777-777-77-77")
        => new (id ?? Guid.NewGuid().ToString(), name, surname, middlename, login, passwd, email, phone);

    private static void AssertElement(ClerkDataModel actual, Clerk? expected)
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

    private static void AssertElement(Clerk actual, ClerkDataModel? expected)
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
