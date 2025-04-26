using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.StorageContracts;
using BankDatabase.Implementations;
using BankDatabase.Models;
using BankTests.Infrastructure;
using Microsoft.VisualStudio.CodeCoverage;

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
        var certificate = BankDbContext.InsertClerkToDatabaseAndReturn();
        AssertElement(_storageContract.GetElementById(certificate.Id), certificate);
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
