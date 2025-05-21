namespace BankBusinessLogic.OfficePackage;

public abstract class BaseWordBuilder
{
    public abstract BaseWordBuilder AddHeader(string header);
    public abstract BaseWordBuilder AddParagraph(string text);
    public abstract BaseWordBuilder AddTable(int[] widths, List<string[]> data);
    public abstract Stream Build();
}
