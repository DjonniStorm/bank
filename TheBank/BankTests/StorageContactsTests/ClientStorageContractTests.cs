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
    private IClientStorageContract _clientStorageContract;

    private string _clerkId;

    private string _depositId;

    private string _creditProgramId;

    private string _storekeeperId;

    private string _periodId;

    [SetUp]
    public void SetUp()
    {
        _clientStorageContract = new ClientStorageContract(BankDbContext);
        _clerkId = BankDbContext.InsertClerkToDatabaseAndReturn().Id;
        _storekeeperId = BankDbContext.InsertStorekeeperToDatabaseAndReturn().Id;
        _depositId = BankDbContext.InsertDepositToDatabaseAndReturn(clerkId: _clerkId).Id;
        _periodId = BankDbContext.InsertPeriodToDatabaseAndReturn(storekeeperId: _storekeeperId).Id;
        _creditProgramId = BankDbContext
            .InsertCreditProgramToDatabaseAndReturn(
                storeleeperId: _storekeeperId,
                periodId: _periodId
            )
            .Id;
    }

    [TearDown]
    public void TearDown()
    {
        BankDbContext.RemoveClientsFromDatabase();
        BankDbContext.RemoveDepositsFromDatabase();
        BankDbContext.RemovePeriodsFromDatabase();
        BankDbContext.RemoveCreditProgramsFromDatabase();
        BankDbContext.RemoveClerksFromDatabase();
        BankDbContext.RemoveStorekeepersFromDatabase();
    }

    [Test]
    public void TryGetListWhenHaveRecords_ShouldSucces_Test()
    {
        var clientId = Guid.NewGuid().ToString();
        var client = BankDbContext.InsertClientToDatabaseAndReturn(
            id: clientId,
            clerkId: _clerkId,
            depositClients: [(_depositId, clientId)]
        );
        BankDbContext.InsertClientToDatabaseAndReturn(
            clerkId: _clerkId,
            depositClients:
            [
                (BankDbContext.InsertDepositToDatabaseAndReturn(clerkId: _clerkId).Id, clientId),
            ]
        );
        BankDbContext.InsertClientToDatabaseAndReturn(
            clerkId: _clerkId,
            creditProgramClients: [(clientId, _creditProgramId)]
        );

        var list = _clientStorageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(3));
        AssertElement(list.First(x => x.Id == clientId), client);
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
        var clientId = Guid.NewGuid().ToString();
        var client = CreateModel(
            _clerkId,
            clientId,
            depositClients: [new DepositClientDataModel(_depositId, clientId)],
            clientCredits: [new ClientCreditProgramDataModel(clientId, _creditProgramId)]
        );
        _clientStorageContract.AddElement(client);
        AssertElement(BankDbContext.GetClientFromDatabase(client.Id), client);
    }

    [Test]
    public void Try_UpdElement_Test()
    {
        var clientId = Guid.NewGuid().ToString();
        var client = CreateModel(
            _clerkId,
            id: clientId,
            depositClients: [new DepositClientDataModel(_depositId, clientId)]
        );
        BankDbContext.InsertClientToDatabaseAndReturn(
            id: clientId,
            clerkId: _clerkId,
            creditProgramClients: [(clientId, _creditProgramId)]
        );
        _clientStorageContract.UpdElement(client);
        BankDbContext.ChangeTracker.Clear();
        AssertElement(BankDbContext.GetClientFromDatabase(clientId), client);
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

        if (expected.DepositClients is not null)
        {
            Assert.That(actual.DepositClients, Is.Not.Null);
            Assert.That(actual.DepositClients, Has.Count.EqualTo(expected.DepositClients.Count));
            for (int i = 0; i < actual.DepositClients.Count; ++i)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(
                        actual.DepositClients[i].ClientId,
                        Is.EqualTo(expected.DepositClients[i].ClientId)
                    );
                    Assert.That(
                        actual.DepositClients[i].DepositId,
                        Is.EqualTo(expected.DepositClients[i].DepositId)
                    );
                });
            }
        }
        else
        {
            Assert.That(actual.DepositClients, Is.Null);
        }

        if (expected.CreditProgramClients is not null)
        {
            Assert.That(actual.CreditProgramClients, Is.Not.Null);
            Assert.That(
                actual.CreditProgramClients,
                Has.Count.EqualTo(expected.CreditProgramClients.Count)
            );
            for (int i = 0; i < actual.CreditProgramClients.Count; ++i)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(
                        actual.CreditProgramClients[i].ClientId,
                        Is.EqualTo(expected.CreditProgramClients[i].ClientId)
                    );
                    Assert.That(
                        actual.CreditProgramClients[i].CreditProgramId,
                        Is.EqualTo(expected.CreditProgramClients[i].CreditProgramId)
                    );
                });
            }
        }
        else
        {
            Assert.That(actual.CreditProgramClients, Is.Null);
        }
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

        if (expected.DepositClients is not null)
        {
            Assert.That(actual.DepositClients, Is.Not.Null);
            Assert.That(actual.DepositClients, Has.Count.EqualTo(expected.DepositClients.Count));
            for (int i = 0; i < actual.DepositClients.Count; ++i)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(
                        actual.DepositClients[i].ClientId,
                        Is.EqualTo(expected.DepositClients[i].ClientId)
                    );
                    Assert.That(
                        actual.DepositClients[i].DepositId,
                        Is.EqualTo(expected.DepositClients[i].DepositId)
                    );
                });
            }
        }
        else
        {
            Assert.That(actual.DepositClients, Is.Null);
        }

        if (expected.CreditProgramClients is not null)
        {
            Assert.That(actual.CreditProgramClients, Is.Not.Null);
            Assert.That(
                actual.CreditProgramClients,
                Has.Count.EqualTo(expected.CreditProgramClients.Count)
            );
            for (int i = 0; i < actual.CreditProgramClients.Count; ++i)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(
                        actual.CreditProgramClients[i].ClientId,
                        Is.EqualTo(expected.CreditProgramClients[i].ClientId)
                    );
                    Assert.That(
                        actual.CreditProgramClients[i].CreditProgramId,
                        Is.EqualTo(expected.CreditProgramClients[i].CreditProgramId)
                    );
                });
            }
        }
        else
        {
            Assert.That(actual.CreditProgramClients, Is.Null);
        }
    }
}
