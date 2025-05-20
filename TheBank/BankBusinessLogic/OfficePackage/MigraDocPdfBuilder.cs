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
        _document.Info.Title = "Bank Report";
        _document.DefaultPageSetup.LeftMargin = Unit.FromCentimeter(2);
        _document.DefaultPageSetup.RightMargin = Unit.FromCentimeter(2);
        _document.DefaultPageSetup.TopMargin = Unit.FromCentimeter(2);
        _document.DefaultPageSetup.BottomMargin = Unit.FromCentimeter(2);
        DefineStyles();
    }

    public override BasePdfBuilder AddHeader(string header)
    {
        var section = _document.AddSection();
        var paragraph = section.AddParagraph(header, "Heading1");
        paragraph.Format.SpaceAfter = Unit.FromPoint(10);
        return this;
    }

    public override BasePdfBuilder AddParagraph(string text)
    {
        var section = _document.LastSection ?? _document.AddSection();
        var paragraph = section.AddParagraph(text, "Normal");
        paragraph.Format.SpaceAfter = Unit.FromPoint(10);
        return this;
    }

    public override BasePdfBuilder AddTable(int[] columnsWidths, List<string[]> data)
    {
        if (columnsWidths == null || columnsWidths.Length == 0)
            throw new ArgumentNullException(nameof(columnsWidths));
        if (data == null || data.Count == 0)
            throw new ArgumentNullException(nameof(data));
        if (data.Any(x => x.Length != columnsWidths.Length))
            throw new InvalidOperationException("widths.Length != data.Length");

        var section = _document.LastSection ?? _document.AddSection();
        var table = section.AddTable();
        table.Style = "Table";
        table.Borders.Width = 0.75;
        table.Borders.Color = Colors.Black;
        table.Rows.LeftIndent = 0;

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
        headerRow.Format.Alignment = ParagraphAlignment.Center;
        headerRow.VerticalAlignment = VerticalAlignment.Center;
        headerRow.Shading.Color = Colors.LightGray;

        for (int j = 0; j < data[0].Length; j++)
        {
            var cell = headerRow.Cells[j];
            cell.AddParagraph(data[0][j]);
            cell.Format.Alignment = ParagraphAlignment.Center;
            cell.VerticalAlignment = VerticalAlignment.Center;
            cell.Borders.Width = 0.5;
        }

        // Промежуточные строки — обычные
        for (int i = 1; i < data.Count - 1; i++)
        {
            var row = table.AddRow();
            row.Format.Alignment = ParagraphAlignment.Left;
            row.VerticalAlignment = VerticalAlignment.Center;

            for (int j = 0; j < data[i].Length; j++)
            {
                var cell = row.Cells[j];
                cell.AddParagraph(data[i][j]);
                cell.Format.Alignment = ParagraphAlignment.Left;
                cell.VerticalAlignment = VerticalAlignment.Center;
                cell.Borders.Width = 0.5;
            }
        }

        // Последняя строка — жирная (например, итог)
        if (data.Count > 1)
        {
            var lastRow = table.AddRow();
            lastRow.Format.Font.Bold = true;
            lastRow.Format.Alignment = ParagraphAlignment.Center;
            lastRow.VerticalAlignment = VerticalAlignment.Center;
            lastRow.Shading.Color = Colors.LightGray;

            for (int j = 0; j < data[^1].Length; j++)
            {
                var cell = lastRow.Cells[j];
                cell.AddParagraph(data[^1][j]);
                cell.Format.Alignment = ParagraphAlignment.Center;
                cell.VerticalAlignment = VerticalAlignment.Center;
                cell.Borders.Width = 0.5;
            }
        }

        section.AddParagraph(); // Добавляем пустую строку после таблицы
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
        stream.Position = 0; // Важно установить позицию в начало потока
        return stream;
    }

    private void DefineStyles()
    {
        // Определяем стиль для обычного текста
        var style = _document.Styles["Normal"];
        style.Font.Name = "Times New Roman";
        style.Font.Size = 12;
        style.ParagraphFormat.SpaceAfter = Unit.FromPoint(10);

        // Определяем стиль для заголовка
        style = _document.Styles.AddStyle("Heading1", "Normal");
        style.Font.Bold = true;
        style.Font.Size = 14;
        style.ParagraphFormat.SpaceAfter = Unit.FromPoint(10);
        style.ParagraphFormat.Alignment = ParagraphAlignment.Center;

        // Определяем стиль для таблицы
        style = _document.Styles.AddStyle("Table", "Normal");
        style.Font.Name = "Times New Roman";
        style.Font.Size = 10;
        style.ParagraphFormat.SpaceAfter = Unit.FromPoint(5);
    }
}
