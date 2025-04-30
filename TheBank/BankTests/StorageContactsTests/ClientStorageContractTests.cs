using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.StorageContracts;
using BankDatabase.Implementations;
using BankDatabase.Models;
using BankTests.Infrastructure;

namespace BankTests.StorageContactsTests;

[TestFixture]
internal class ClientStorageContractTests : BaseStorageContractTest
{
    private string _clerkId;

    private IClientStorageContract _clientStorageContract;

    [SetUp]
    public void SetUp()
    {
        _clerkId = BankDbContext.InsertClerkToDatabaseAndReturn().Id;
        _clientStorageContract = new ClientStorageContract(BankDbContext);
    }

    [TearDown]
    public void TearDown()
    {
        BankDbContext.RemoveClientsFromDatabase();
        BankDbContext.RemoveClerksFromDatabase();
    }

    [Test]
    public void TryGetListWhenHaveRecords_ShouldSucces_Test()
    {
        var client = BankDbContext.InsertClientToDatabaseAndReturn(clerkId: _clerkId);
        BankDbContext.InsertClientToDatabaseAndReturn(clerkId: _clerkId);
        BankDbContext.InsertClientToDatabaseAndReturn(clerkId: _clerkId);

        var list = _clientStorageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(3));
        AssertElement(list.First(x => x.Id == client.Id), client);
    }

    [Test]
    public void TryGetList_WhenNoRecords_Test()
    {
        var list = _clientStorageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.Empty);
    }

    [Test]
    public void TryGetList_ByClerk_WhenHaveRecords_Test()
    {
        var clerk = BankDbContext.InsertClerkToDatabaseAndReturn(
            email: "slava@ilya.com",
            login: "login",
            phone: "+7-987-555-55-55"
        );

        BankDbContext.InsertClientToDatabaseAndReturn(clerkId: clerk.Id);
        BankDbContext.InsertClientToDatabaseAndReturn(clerkId: clerk.Id);
        BankDbContext.InsertClientToDatabaseAndReturn(clerkId: clerk.Id);
        BankDbContext.InsertClientToDatabaseAndReturn(clerkId: clerk.Id);

        var list = _clientStorageContract.GetList(clerkId: clerk.Id);
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(4));
    }

    [Test]
    public void Try_GetElementById_WhenHaveRecord_Test()
    {
        var client = BankDbContext.InsertClientToDatabaseAndReturn(clerkId: _clerkId);
        AssertElement(_clientStorageContract.GetElementById(client.Id), client);
    }

    [Test]
    public void Try_AddElement_Test()
    {
        var client = CreateModel(_clerkId);
        _clientStorageContract.AddElement(client);
        AssertElement(BankDbContext.GetClientFromDatabase(client.Id), client);
    }

    [Test]
    public void Try_UpdElement_Test()
    {
        var client = CreateModel(_clerkId);
        BankDbContext.InsertClientToDatabaseAndReturn(id: client.Id, clerkId: _clerkId);
        _clientStorageContract.UpdElement(client);
        AssertElement(BankDbContext.GetClientFromDatabase(client.Id), client);
    }

    [Test]
    public void Try_UpdElement_WhenNoRecordWithThisId_Test()
    {
        Assert.That(
            () => _clientStorageContract.UpdElement(CreateModel(_clerkId)),
            Throws.TypeOf<ElementNotFoundException>()
        );
    }

    private static ClientDataModel CreateModel(
        string clerkid,
        string? id = null,
        string? name = "null",
        string? surname = "surname",
        decimal balance = 1,
        List<DepositClientDataModel>? depositClients = null,
        List<ClientCreditProgramDataModel>? clientCredits = null
    ) =>
        new(
            id ?? Guid.NewGuid().ToString(),
            name,
            surname,
            balance,
            clerkid,
            depositClients ?? [],
            clientCredits ?? []
        );

    private static void AssertElement(ClientDataModel actual, Client? expected)
    {
        Assert.That(expected, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.Name, Is.EqualTo(expected.Name));
            Assert.That(actual.Surname, Is.EqualTo(expected.Surname));
            Assert.That(actual.Balance, Is.EqualTo(expected.Balance));
            Assert.That(actual.ClerkId, Is.EqualTo(expected.ClerkId));
        });
    }

    private static void AssertElement(Client actual, ClientDataModel? expected)
    {
        Assert.That(expected, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.Name, Is.EqualTo(expected.Name));
            Assert.That(actual.Surname, Is.EqualTo(expected.Surname));
            Assert.That(actual.Balance, Is.EqualTo(expected.Balance));
            Assert.That(actual.ClerkId, Is.EqualTo(expected.ClerkId));
        });
    }
}
