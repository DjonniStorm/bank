using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using System.Text;

namespace BankBusinessLogic.OfficePackage;

public class MigraDocPdfBuilder : BasePdfBuilder
{
    private readonly Document _document;

    public MigraDocPdfBuilder()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        _document = new Document();
        DefineStyles();
    }

    public override BasePdfBuilder AddHeader(string header)
    {
        _document.AddSection().AddParagraph(header, "NormalBold");
        return this;
    }

    public override BasePdfBuilder AddParagraph(string text)
    {
        _document.LastSection.AddParagraph(text, "Normal");
        return this;
    }

    public override BasePdfBuilder CreateTable(int[] columnsWidths, List<string[]> data)
    {
        if (columnsWidths == null || columnsWidths.Length == 0)
            throw new ArgumentNullException(nameof(columnsWidths));
        if (data == null || data.Count == 0)
            throw new ArgumentNullException(nameof(data));
        if (data.Any(x => x.Length != columnsWidths.Length))
            throw new InvalidOperationException("widths.Length != data.Length");

        var section = _document.LastSection ?? _document.AddSection();
        var table = section.AddTable();
        table.Borders.Width = 0.75;

        // Добавляем столбцы с заданной шириной
        foreach (var width in columnsWidths)
        {
            var column = table.AddColumn(Unit.FromPoint(width));
            column.Format.Alignment = ParagraphAlignment.Left;
        }

        // Первая строка — заголовок (жирный)
        var headerRow = table.AddRow();
        headerRow.HeadingFormat = true;
        headerRow.Format.Font.Bold = true;
        for (int j = 0; j < data[0].Length; j++)
        {
            headerRow.Cells[j].AddParagraph(data[0][j]);
            headerRow.Cells[j].Format.Alignment = ParagraphAlignment.Center;
            headerRow.Cells[j].VerticalAlignment = VerticalAlignment.Center;
        }

        // Промежуточные строки — обычные
        for (int i = 1; i < data.Count - 1; i++)
        {
            var row = table.AddRow();
            for (int j = 0; j < data[i].Length; j++)
            {
                row.Cells[j].AddParagraph(data[i][j]);
                row.Cells[j].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[j].VerticalAlignment = VerticalAlignment.Center;
            }
        }

        // Последняя строка — жирная (например, итог)
        if (data.Count > 1)
        {
            var lastRow = table.AddRow();
            lastRow.Format.Font.Bold = true;
            for (int j = 0; j < data[^1].Length; j++)
            {
                lastRow.Cells[j].AddParagraph(data[^1][j]);
                lastRow.Cells[j].Format.Alignment = ParagraphAlignment.Center;
                lastRow.Cells[j].VerticalAlignment = VerticalAlignment.Center;
            }
        }

        return this;
    }

    public override Stream Build()
    {
        var stream = new MemoryStream();
        var renderer = new PdfDocumentRenderer(true)
        {
            Document = _document
        };
        renderer.RenderDocument();
        renderer.PdfDocument.Save(stream);
        return stream;
    }

    private void DefineStyles()
    {
        var style = _document.Styles.AddStyle("NormalBold", "Normal");
        style.Font.Bold = true;
    }
}
