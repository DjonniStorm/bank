namespace BankBusinessLogic.OfficePackage;

public abstract class BasePdfBuilder
{
    public abstract BasePdfBuilder AddHeader(string header);
    public abstract BasePdfBuilder AddParagraph(string text);
    public abstract BasePdfBuilder CreateTable(int[] columnsWidths, List<string[]> data);
    public abstract Stream Build();
}
