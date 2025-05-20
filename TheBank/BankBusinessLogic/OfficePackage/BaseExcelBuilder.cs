namespace BankBusinessLogic.OfficePackage;

public abstract class BaseExcelBuilder
{
    public abstract BaseExcelBuilder AddHeader(string header, int startIndex, int count);
    public abstract BaseExcelBuilder AddParagraph(string text, int columnIndex);
    public abstract BaseExcelBuilder AddTable(int[] columnsWidths, List<string[]> data);
    public abstract Stream Build();
}
