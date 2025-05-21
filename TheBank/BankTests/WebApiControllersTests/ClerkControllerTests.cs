using BankContracts.BindingModels;
using BankContracts.ViewModels;
using BankDatabase.Models;
using BankTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace BankTests.WebApiControllersTests;

[TestFixture]
internal class ClerkControllerTests : BaseWebApiControllerTest
{
    [TearDown]
    public void TearDown()
    {
        BankDbContext.RemoveClerksFromDatabase();
    }

    [Test]
    public async Task GetAllRecords_WhenHaveRecords_ShouldSuccess_Test()
    {
        // Arrange
        var clerk1 = BankDbContext.InsertClerkToDatabaseAndReturn(name: "Иван", surname: "Иванов", email: "ivanov1@mail.com", login: "zal***", phone: "+7-777-777-78-90");
        var clerk2 = BankDbContext.InsertClerkToDatabaseAndReturn(name: "Петр", surname: "Петров", email: "petrov2@mail.com", login: "nepupkin", phone: "+7-777-777-78-91");
        // Act
        var response = await HttpClient.GetAsync("/api/clerks");
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var data = await GetModelFromResponseAsync<List<ClerkViewModel>>(response);
        Assert.That(data, Is.Not.Null);
        Assert.That(data, Has.Count.EqualTo(2));
        AssertElement(data.First(x => x.Id == clerk1.Id), clerk1);
        AssertElement(data.First(x => x.Id == clerk2.Id), clerk2);
    }

    [Test]
    public async Task GetAllRecords_WhenNoRecords_ShouldSuccess_Test()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/clerks");
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var data = await GetModelFromResponseAsync<List<ClerkViewModel>>(response);
        Assert.That(data, Is.Not.Null);
        Assert.That(data, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task GetRecord_ById_WhenHaveRecord_ShouldSuccess_Test()
    {
        // Arrange
        var clerk = BankDbContext.InsertClerkToDatabaseAndReturn();
        // Act
        var response = await HttpClient.GetAsync($"/api/clerks/{clerk.Id}");
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        AssertElement(await GetModelFromResponseAsync<ClerkViewModel>(response), clerk);
    }

    [Test]
    public async Task GetRecord_ById_WhenNoRecord_ShouldNotFound_Test()
    {
        // Act
        var response = await HttpClient.GetAsync($"/api/clerks/{Guid.NewGuid()}");
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Post_ShouldSuccess_Test()
    {
        // Arrange
        var model = CreateModel();
        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/clerks/register", model);
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        AssertElement(BankDbContext.GetClerkFromDatabase(model.Id!), model);
    }

    [Test]
    public async Task Post_WhenHaveRecordWithSameId_ShouldBadRequest_Test()
    {
        // Arrange
        var model = CreateModel();
        BankDbContext.InsertClerkToDatabaseAndReturn(id: model.Id);
        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/clerks/register", model);
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Post_WhenSendEmptyData_ShouldBadRequest_Test()
    {
        // Act
        var response = await HttpClient.PostAsync("/api/clerks/register", MakeContent(string.Empty));
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Put_ShouldSuccess_Test()
    {
        // Arrange
        var model = CreateModel();
        BankDbContext.InsertClerkToDatabaseAndReturn(id: model.Id);
        // Act
        var response = await HttpClient.PutAsJsonAsync("/api/clerks", model);
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        BankDbContext.ChangeTracker.Clear();
        AssertElement(BankDbContext.GetClerkFromDatabase(model.Id!), model);
    }

    [Test]
    public async Task Put_WhenNoFoundRecord_ShouldBadRequest_Test()
    {
        // Arrange
        var model = CreateModel();
        // Act
        var response = await HttpClient.PutAsJsonAsync("/api/clerks", model);
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    private static void AssertElement(ClerkViewModel? actual, Clerk expected)
    {
        Assert.That(actual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.Name, Is.EqualTo(expected.Name));
            Assert.That(actual.Surname, Is.EqualTo(expected.Surname));
            Assert.That(actual.MiddleName, Is.EqualTo(expected.MiddleName));
            Assert.That(actual.Login, Is.EqualTo(expected.Login));
            Assert.That(actual.Email, Is.EqualTo(expected.Email));
            Assert.That(actual.PhoneNumber, Is.EqualTo(expected.PhoneNumber));
        });
    }

    private static ClerkBindingModel CreateModel(
        string? id = null,
        string name = "Иван",
        string surname = "Иванов",
        string middleName = "Иванович",
        string login = "ivanov",
        string password = "password",
        string email = "ivanov@mail.com",
        string phoneNumber = "+79999999999")
        => new()
        {
            Id = id ?? Guid.NewGuid().ToString(),
            Name = name,
            Surname = surname,
            MiddleName = middleName,
            Login = login,
            Password = password,
            Email = email,
            PhoneNumber = phoneNumber
        };

    private static void AssertElement(Clerk? actual, ClerkBindingModel expected)
    {
        Assert.That(actual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.Name, Is.EqualTo(expected.Name));
            Assert.That(actual.Surname, Is.EqualTo(expected.Surname));
            Assert.That(actual.MiddleName, Is.EqualTo(expected.MiddleName));
            Assert.That(actual.Login, Is.EqualTo(expected.Login));
            Assert.That(actual.Email, Is.EqualTo(expected.Email));
            Assert.That(actual.PhoneNumber, Is.EqualTo(expected.PhoneNumber));
        });
    }
}
