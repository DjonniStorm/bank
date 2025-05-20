using BankContracts.BindingModels;
using BankContracts.ViewModels;
using BankDatabase;
using BankDatabase.Models;
using BankTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace BankTests.WebApiControllersTests;

[TestFixture]
internal class ClientControllerTests : BaseWebApiControllerTest
{
    private Clerk _clerk;

    [SetUp]
    public void SetUp()
    {
        _clerk = BankDbContext.InsertClerkToDatabaseAndReturn();
    }


    [TearDown]
    public void TearDown()
    {
        BankDbContext.RemoveClientsFromDatabase();
        BankDbContext.RemoveClerksFromDatabase();
    }

    [Test]
    public async Task GetList_WhenHaveRecords_ShouldSuccess_Test()
    {
        // Arrange
        var client1 = BankDbContext.InsertClientToDatabaseAndReturn(name: "Иван", surname: "Иванов", clerkId: _clerk.Id);
        var client2 = BankDbContext.InsertClientToDatabaseAndReturn(name: "Петр", surname: "Петров", clerkId: _clerk.Id);
        // Act
        var response = await HttpClient.GetAsync("/api/clients/getrecords");
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var data = await GetModelFromResponseAsync<List<ClientViewModel>>(response);
        Assert.That(data, Is.Not.Null);
        Assert.That(data, Has.Count.EqualTo(2));
        AssertElement(data.First(x => x.Id == client1.Id), client1);
        AssertElement(data.First(x => x.Id == client2.Id), client2);
    }

    [Test]
    public async Task GetList_WhenNoRecords_ShouldSuccess_Test()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/clients/getrecords");
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var data = await GetModelFromResponseAsync<List<ClientViewModel>>(response);
        Assert.That(data, Is.Not.Null);
        Assert.That(data, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task GetElement_ById_WhenHaveRecord_ShouldSuccess_Test()
    {
        // Arrange
        var client = BankDbContext.InsertClientToDatabaseAndReturn(clerkId: _clerk.Id);
        // Act
        var response = await HttpClient.GetAsync($"/api/clients/getrecord/{client.Id}");
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        AssertElement(await GetModelFromResponseAsync<ClientViewModel>(response), client);
    }

    [Test]
    public async Task GetElement_ById_WhenNoRecord_ShouldNotFound_Test()
    {
        // Act
        var response = await HttpClient.GetAsync($"/api/clients/getrecord/{Guid.NewGuid()}");
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Post_ShouldSuccess_Test()
    {
        // Arrange
        var model = CreateModel();
        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/clients/register", model);
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        AssertElement(BankDbContext.GetClientFromDatabase(model.Id!), model);
    }

    [Test]
    public async Task Post_WhenHaveRecordWithSameId_ShouldBadRequest_Test()
    {
        // Arrange
        var model = CreateModel();
        BankDbContext.InsertClientToDatabaseAndReturn(id: model.Id,clerkId: _clerk.Id);
        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/clients/register", model);
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Post_WhenSendEmptyData_ShouldBadRequest_Test()
    {
        // Act
        var response = await HttpClient.PostAsync("/api/clients/register", MakeContent(string.Empty));
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Put_ShouldSuccess_Test()
    {
        // Arrange
        var model = CreateModel();
        BankDbContext.InsertClientToDatabaseAndReturn(id: model.Id, clerkId: _clerk.Id);
        // Act
        var response = await HttpClient.PutAsJsonAsync("/api/clients/changeinfo", model);
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        BankDbContext.ChangeTracker.Clear();
        AssertElement(BankDbContext.GetClientFromDatabase(model.Id!), model);
    }

    [Test]
    public async Task ChangeInfo_WhenNoFoundRecord_ShouldBadRequest_Test()
    {
        // Arrange
        var model = CreateModel();
        // Act
        var response = await HttpClient.PutAsJsonAsync("/api/clients/changeinfo", model);
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    private static void AssertElement(ClientViewModel? actual, Client expected)
    {
        Assert.That(actual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.Name, Is.EqualTo(expected.Name));
            Assert.That(actual.Surname, Is.EqualTo(expected.Surname));
            Assert.That(actual.Balance, Is.EqualTo(expected.Balance));
            Assert.That(actual.ClerkId, Is.EqualTo(expected.ClerkId));
        });
    }

    private static ClientBindingModel CreateModel(string? id = null, string name = "Иван", string surname = "Иванов", decimal balance = 1000, string? clerkId = null)
        => new()
        {
            Id = id ?? Guid.NewGuid().ToString(),
            Name = name,
            Surname = surname,
            Balance = balance,
            ClerkId = clerkId
        };

    private static void AssertElement(Client? actual, ClientBindingModel expected)
    {
        Assert.That(actual, Is.Not.Null);
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
