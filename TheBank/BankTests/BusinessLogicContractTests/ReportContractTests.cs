using BankBusinessLogic.Implementations;
using BankBusinessLogic.OfficePackage;
using BankContracts.DataModels;
using BankContracts.StorageContracts;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;

namespace BankTests.BusinessLogicContractTests;

[TestFixture]
internal class ReportContractTestss
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
    }

    [Test]
    public async Task GetDataDepositByCreditProgramAsync_ReturnsData()
    {
        var ct = CancellationToken.None;
        _creditProgramStorage.Setup(x => x.GetList(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new List<CreditProgramDataModel>
            {
               new CreditProgramDataModel("1", "Кредит 1", 100, 200, "sk", "p", new List<CreditProgramCurrencyDataModel>())
            });
        _depositStorage.Setup(x => x.GetList(It.IsAny<string>()))
            .Returns(new List<DepositDataModel>
            {
               new DepositDataModel("d1", 5, 1000, 12, "cl", new List<DepositCurrencyDataModel>())
            });

        var result = await _reportContract.GetDataDepositByCreditProgramAsync(ct);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.GreaterThanOrEqualTo(0));
    }

    [Test]
    public async Task CreateDocumentDepositByCreditProgramAsync_CallsWordBuilder()
    {
        var ct = CancellationToken.None;
        _creditProgramStorage.Setup(x => x.GetList(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new List<CreditProgramDataModel>
            {
                new CreditProgramDataModel("1", "Кредит 1", 100, 200, "sk", "p", new List<CreditProgramCurrencyDataModel>())
            });
        _depositStorage.Setup(x => x.GetList(It.IsAny<string>()))
            .Returns(new List<DepositDataModel>
            {
                new DepositDataModel("d1", 5, 1000, 12, "cl", new List<DepositCurrencyDataModel>())
            });

        _baseWordBuilder.Setup(x => x.AddHeader(It.IsAny<string>())).Returns(_baseWordBuilder.Object);
        _baseWordBuilder.Setup(x => x.AddTable(It.IsAny<int[]>(), It.IsAny<List<string[]>>())).Returns(_baseWordBuilder.Object);
        _baseWordBuilder.Setup(x => x.Build()).Returns(new MemoryStream(Encoding.UTF8.GetBytes("test")));

        var stream = await _reportContract.CreateDocumentDepositByCreditProgramAsync(ct);

        Assert.That(stream, Is.Not.Null);
        _baseWordBuilder.Verify(x => x.AddHeader(It.IsAny<string>()), Times.Once);
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
        _basePdfBuilder.Setup(x => x.CreateTable(It.IsAny<int[]>(), It.IsAny<List<string[]>>())).Returns(_basePdfBuilder.Object);
        _basePdfBuilder.Setup(x => x.Build()).Returns(new MemoryStream(Encoding.UTF8.GetBytes("test")));

        var stream = await _reportContract.CreateDocumentClientsByDepositAsync(dateStart, dateFinish, ct);

        Assert.That(stream, Is.Not.Null);
        _basePdfBuilder.Verify(x => x.AddHeader(It.IsAny<string>()), Times.Once);
        _basePdfBuilder.Verify(x => x.AddParagraph(It.IsAny<string>()), Times.Once);
        _basePdfBuilder.Verify(x => x.CreateTable(It.IsAny<int[]>(), It.IsAny<List<string[]>>()), Times.Once);
        _basePdfBuilder.Verify(x => x.Build(), Times.Once);
    }
}