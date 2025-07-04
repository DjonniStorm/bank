using BankBusinessLogic.Implementations;
using BankBusinessLogic.OfficePackage;
using BankContracts.BusinessLogicContracts;
using BankContracts.DataModels;
using BankContracts.StorageContracts;
using BankDatabase.Implementations;
using BankTests.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Text;

namespace BankTests.ReportContractTests;

[TestFixture]
internal class ReportContractTests
{
    private ReportContract _reportContract;
    private Mock<IClientStorageContract> _clientStorage;
    private Mock<ICurrencyStorageContract> _currencyStorage;
    private Mock<ICreditProgramStorageContract> _creditProgramStorage;
    private Mock<IDepositStorageContract> _depositStorage;
    private Mock<BaseWordBuilder> _baseWordBuilder;
    private Mock<BaseExcelBuilder> _baseExcelBuilder;
    private Mock<BasePdfBuilder> _basePdfBuilder;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _clientStorage = new Mock<IClientStorageContract>();
        _currencyStorage = new Mock<ICurrencyStorageContract>();
        _creditProgramStorage = new Mock<ICreditProgramStorageContract>();
        _depositStorage = new Mock<IDepositStorageContract>();
        _baseWordBuilder = new Mock<BaseWordBuilder>();
        _baseExcelBuilder = new Mock<BaseExcelBuilder>();
        _basePdfBuilder = new Mock<BasePdfBuilder>();
        _reportContract = new ReportContract(_clientStorage.Object, _currencyStorage.Object, _creditProgramStorage.Object, _depositStorage.Object,
            _baseWordBuilder.Object, _baseExcelBuilder.Object, _basePdfBuilder.Object, new Mock<ILogger>().Object);
    }

    [SetUp]
    public void SetUp()
    {
        _clientStorage.Reset();
        _currencyStorage.Reset();
        _creditProgramStorage.Reset();
        _depositStorage.Reset();
        _baseWordBuilder.Reset();
        _baseExcelBuilder.Reset();
        _basePdfBuilder.Reset();
    }

    [Test]
    public async Task GetDataDepositByCreditProgramAsync_ReturnsData()
    {
        var ct = CancellationToken.None;
        _creditProgramStorage.Setup(x => x.GetList(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new List<CreditProgramDataModel>
            {
               new CreditProgramDataModel("1", "Программа 1", 100, 200, "sk", "p", new List<CreditProgramCurrencyDataModel>())
            });
        _depositStorage.Setup(x => x.GetList(It.IsAny<string>()))
            .Returns(new List<DepositDataModel>
            {
               new DepositDataModel("d1", 5, 1000, 12, "cl", new List<DepositCurrencyDataModel>())
            });

        var result = await _reportContract.GetDataDepositByCreditProgramAsync(null, ct);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.GreaterThanOrEqualTo(0));
    }

    [Test]
    public async Task GetDataDepositByCreditProgramAsync_WithFilter_ReturnsFilteredData()
    {
        var ct = CancellationToken.None;
        var creditProgramIds = new List<string> { "1" };
        
        _creditProgramStorage.Setup(x => x.GetList(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new List<CreditProgramDataModel>
            {
               new CreditProgramDataModel("1", "Программа 1", 100, 200, "sk", "p", new List<CreditProgramCurrencyDataModel> { new() { CurrencyId = "1" } }),
               new CreditProgramDataModel("2", "Программа 2", 100, 200, "sk", "p", new List<CreditProgramCurrencyDataModel> { new() { CurrencyId = "1" } })
            });
        _depositStorage.Setup(x => x.GetList(It.IsAny<string>()))
            .Returns(new List<DepositDataModel>
            {
               new DepositDataModel("d1", 5, 1000, 12, "cl", new List<DepositCurrencyDataModel> { new() { CurrencyId = "1" } })
            });

        var result = await _reportContract.GetDataDepositByCreditProgramAsync(creditProgramIds, ct);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].CreditProgramName, Is.EqualTo("Программа 1"));
    }

    [Test]
    public async Task CreateDocumentDepositByCreditProgramAsync_CallsWordBuilder()
    {
        var ct = CancellationToken.None;
        _creditProgramStorage.Setup(x => x.GetList(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new List<CreditProgramDataModel>
            {
                new CreditProgramDataModel("1", "Программа 1", 100, 200, "sk", "p", new List<CreditProgramCurrencyDataModel> { new() { CurrencyId = "1" } })
            });
        _depositStorage.Setup(x => x.GetList(It.IsAny<string>()))
            .Returns(new List<DepositDataModel>
            {
                new DepositDataModel("d1", 5, 1000, 12, "cl", new List<DepositCurrencyDataModel> { new() { CurrencyId = "1" } })
            });

        _baseWordBuilder.Setup(x => x.AddHeader(It.IsAny<string>())).Returns(_baseWordBuilder.Object);
        _baseWordBuilder.Setup(x => x.AddParagraph(It.IsAny<string>())).Returns(_baseWordBuilder.Object);
        _baseWordBuilder.Setup(x => x.AddTable(It.IsAny<int[]>(), It.IsAny<List<string[]>>())).Returns(_baseWordBuilder.Object);
        _baseWordBuilder.Setup(x => x.Build()).Returns(new MemoryStream(Encoding.UTF8.GetBytes("test")));

        var stream = await _reportContract.CreateDocumentDepositByCreditProgramAsync(new List<string> { "1" }, ct);

        Assert.That(stream, Is.Not.Null);
        _baseWordBuilder.Verify(x => x.AddHeader(It.IsAny<string>()), Times.Once);
        _baseWordBuilder.Verify(x => x.AddParagraph(It.IsAny<string>()), Times.Once);
        _baseWordBuilder.Verify(x => x.AddTable(It.IsAny<int[]>(), It.IsAny<List<string[]>>()), Times.Once);
        _baseWordBuilder.Verify(x => x.Build(), Times.Once);
    }

    [Test]
    public async Task CreateDocumentClientsByDepositAsync_CallsPdfBuilder()
    {
        var ct = CancellationToken.None;
        var dateStart = new DateTime(2024, 1, 1);
        var dateFinish = new DateTime(2024, 12, 31);

        _clientStorage.Setup(x => x.GetList(It.IsAny<string>()))
            .Returns(new List<ClientDataModel>
            {
                new("1", "Иван", "Иванов", 1000, "cl", new(), new())
            });
        _depositStorage.Setup(x => x.GetListAsync(dateStart, dateFinish, ct))
            .ReturnsAsync(new List<DepositDataModel>
            {
                new("d1", 5, 1000, 12, "cl", new())
            });

        _basePdfBuilder.Setup(x => x.AddHeader(It.IsAny<string>())).Returns(_basePdfBuilder.Object);
        _basePdfBuilder.Setup(x => x.AddParagraph(It.IsAny<string>())).Returns(_basePdfBuilder.Object);
        _basePdfBuilder.Setup(x => x.AddTable(It.IsAny<int[]>(), It.IsAny<List<string[]>>())).Returns(_basePdfBuilder.Object);
        _basePdfBuilder.Setup(x => x.Build()).Returns(new MemoryStream(Encoding.UTF8.GetBytes("test")));

        var stream = await _reportContract.CreateDocumentClientsByDepositAsync(dateStart, dateFinish, ct);

        Assert.That(stream, Is.Not.Null);
        _basePdfBuilder.Verify(x => x.AddHeader(It.IsAny<string>()), Times.Once);
        _basePdfBuilder.Verify(x => x.AddParagraph(It.IsAny<string>()), Times.Once);
        _basePdfBuilder.Verify(x => x.AddTable(It.IsAny<int[]>(), It.IsAny<List<string[]>>()), Times.Once);
        _basePdfBuilder.Verify(x => x.Build(), Times.Once);
    }

    [Test]
    public async Task CreateExcelDocumentClientsByCreditProgramAsync_CallsExcelBuilder()
    {
        var ct = CancellationToken.None;
        _clientStorage.Setup(x => x.GetList(It.IsAny<string>()))
            .Returns(new List<ClientDataModel>
            {
                new("1", "Иван", "Иванов", 1000, "cl", new(), new())
            });
        _creditProgramStorage.Setup(x => x.GetList(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new List<CreditProgramDataModel>
            {
                new CreditProgramDataModel("1", "Программа 1", 100, 200, "sk", "p", new List<CreditProgramCurrencyDataModel>())
            });

        _baseExcelBuilder.Setup(x => x.AddHeader(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(_baseExcelBuilder.Object);
        _baseExcelBuilder.Setup(x => x.AddParagraph(It.IsAny<string>(), It.IsAny<int>())).Returns(_baseExcelBuilder.Object);
        _baseExcelBuilder.Setup(x => x.AddTable(It.IsAny<int[]>(), It.IsAny<List<string[]>>())).Returns(_baseExcelBuilder.Object);
        _baseExcelBuilder.Setup(x => x.Build()).Returns(new MemoryStream(Encoding.UTF8.GetBytes("test")));

        var stream = await _reportContract.CreateExcelDocumentClientsByCreditProgramAsync(ct);

        Assert.That(stream, Is.Not.Null);
        _baseExcelBuilder.Verify(x => x.AddHeader(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        _baseExcelBuilder.Verify(x => x.AddParagraph(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        _baseExcelBuilder.Verify(x => x.AddTable(It.IsAny<int[]>(), It.IsAny<List<string[]>>()), Times.Once);
        _baseExcelBuilder.Verify(x => x.Build(), Times.Once);
    }

    [Test]
    public async Task CreateExcelDocumentDepositByCreditProgramAsync_CallsExcelBuilder()
    {
        var ct = CancellationToken.None;
        _creditProgramStorage.Setup(x => x.GetList(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new List<CreditProgramDataModel>
            {
                new CreditProgramDataModel("1", "Программа 1", 100, 200, "sk", "p", new List<CreditProgramCurrencyDataModel> { new() { CurrencyId = "1" } })
            });
        _depositStorage.Setup(x => x.GetList(It.IsAny<string>()))
            .Returns(new List<DepositDataModel>
            {
                new DepositDataModel("d1", 5, 1000, 12, "cl", new List<DepositCurrencyDataModel> { new() { CurrencyId = "1" } })
            });

        _baseExcelBuilder.Setup(x => x.AddHeader(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(_baseExcelBuilder.Object);
        _baseExcelBuilder.Setup(x => x.AddParagraph(It.IsAny<string>(), It.IsAny<int>())).Returns(_baseExcelBuilder.Object);
        _baseExcelBuilder.Setup(x => x.AddTable(It.IsAny<int[]>(), It.IsAny<List<string[]>>())).Returns(_baseExcelBuilder.Object);
        _baseExcelBuilder.Setup(x => x.Build()).Returns(new MemoryStream(Encoding.UTF8.GetBytes("test")));

        var stream = await _reportContract.CreateExcelDocumentDepositByCreditProgramAsync(new List<string> { "1" }, ct);

        Assert.That(stream, Is.Not.Null);
        _baseExcelBuilder.Verify(x => x.AddHeader(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        _baseExcelBuilder.Verify(x => x.AddParagraph(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        _baseExcelBuilder.Verify(x => x.AddTable(It.IsAny<int[]>(), It.IsAny<List<string[]>>()), Times.Once);
        _baseExcelBuilder.Verify(x => x.Build(), Times.Once);
    }

    [Test]
    public async Task GetDataDepositByCreditProgramAsync_NoCurrencies_ThrowsException()
    {
        var ct = CancellationToken.None;
        _creditProgramStorage.Setup(x => x.GetList(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new List<CreditProgramDataModel>
            {
                new CreditProgramDataModel("1", "Программа 1", 100, 200, "sk", "p", new List<CreditProgramCurrencyDataModel>())
            });
        _depositStorage.Setup(x => x.GetList(It.IsAny<string>()))
            .Returns(new List<DepositDataModel>
            {
                new DepositDataModel("d1", 5, 1000, 12, "cl", new List<DepositCurrencyDataModel>())
            });

        Assert.ThrowsAsync<InvalidOperationException>(() => 
            _reportContract.GetDataDepositByCreditProgramAsync(null, ct));
    }
}