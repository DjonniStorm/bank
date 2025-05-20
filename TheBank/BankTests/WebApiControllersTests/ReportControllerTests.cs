using BankTests.Infrastructure;
using System.Net;

namespace BankTests.WebApiControllersTests;

[TestFixture]
internal class ReportControllerTests : BaseWebApiControllerTest
{
    [TearDown]
    public void TearDown()
    {
        BankDbContext.RemoveClientsFromDatabase();
        BankDbContext.RemoveCreditProgramsFromDatabase();
        BankDbContext.RemoveDepositsFromDatabase();
        BankDbContext.RemoveCurrenciesFromDatabase();
    }

    [Test]
    public async Task LoadDepositByCreditProgram_WhenHaveRecords_ShouldSuccess_Test()
    { 
    }
    private static async Task AssertStreamAsync(HttpResponseMessage response, string fileNameForSave = "")
    {
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        using var data = await response.Content.ReadAsStreamAsync();
        Assert.That(data, Is.Not.Null);
        Assert.That(data.Length, Is.GreaterThan(0));
        await SaveStreamAsync(data, fileNameForSave);
    }
    private static async Task SaveStreamAsync(Stream stream, string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return;
        }
        var path = Path.Combine(Directory.GetCurrentDirectory(), fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        stream.Position = 0;
        using var fileStream = new FileStream(path, FileMode.OpenOrCreate);
        await stream.CopyToAsync(fileStream);
    }
}
