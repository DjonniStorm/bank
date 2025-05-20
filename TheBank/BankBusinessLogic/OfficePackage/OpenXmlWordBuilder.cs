using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;

namespace BankBusinessLogic.OfficePackage;

public class OpenXmlWordBuilder : BaseWordBuilder
{
    private readonly Document _document;

    private readonly Body _body;

    public OpenXmlWordBuilder()
    {
        _document = new Document();
        _body = _document.AppendChild(new Body());
    }

    public override BaseWordBuilder AddHeader(string header)
    {
        var paragraph = _body.AppendChild(new Paragraph());
        var run = paragraph.AppendChild(new Run());
        run.AppendChild(new RunProperties(new Bold()));
        run.AppendChild(new Text(header));

        return this;
    }

    public override BaseWordBuilder AddParagraph(string text)
    {
        var paragraph = _body.AppendChild(new Paragraph());
        var run = paragraph.AppendChild(new Run());
        run.AppendChild(new Text(text));

        return this;
    }

    public override BaseWordBuilder AddTable(int[] widths, List<string[]> data)
    {
        if (widths == null || widths.Length == 0)
        {
            throw new ArgumentNullException(nameof(widths));
        }

        if (data == null || data.Count == 0)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (data.Any(x => x.Length != widths.Length))
        {
            throw new InvalidOperationException("widths.Length != data.Length");
        }

        var table = new Table();
        table.AppendChild(new TableProperties(
            new TableBorders(
                new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 }
            )
        ));

        // Заголовок
        var tr = new TableRow();
        for (var j = 0; j < widths.Length; ++j)
        {
            tr.Append(new TableCell(
                new TableCellProperties(new TableCellWidth() { Width = widths[j].ToString() }),
                new Paragraph(new Run(new RunProperties(new Bold()), new Text(data.First()[j])))));
        }
        table.Append(tr);

        // Данные
        table.Append(data.Skip(1).Select(x =>
            new TableRow(x.Select(y => new TableCell(new Paragraph(new Run(new Text(y))))))));

        _body.Append(table);

        return this;
    }

    public override Stream Build()
    {
        var stream = new MemoryStream();
        using var wordDocument = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document);
        var mainPart = wordDocument.AddMainDocumentPart();
        mainPart.Document = _document;
        return stream;
    }
}
