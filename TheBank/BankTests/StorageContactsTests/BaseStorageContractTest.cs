using BankDatabase;
using BankTests.Infrastructure;

namespace BankTests.StorageContactsTests;

internal class BaseStorageContractTest
{
    protected BankDbContext BankDbContext { get; private set; }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        BankDbContext = new BankDbContext(new Infrastructure.ConfigurationDatabase());

        BankDbContext.Database.EnsureDeleted();
        BankDbContext.Database.EnsureCreated();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        BankDbContext.Database.EnsureDeleted();
        BankDbContext.Dispose();
    }
}
