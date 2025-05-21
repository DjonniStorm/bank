using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

namespace BankBusinessLogic.OfficePackage;

public class OpenXmlExcelBuilder : BaseExcelBuilder
{
    private readonly SheetData _sheetData;

    private readonly MergeCells _mergeCells;

    private readonly Columns _columns;

    private uint _rowIndex = 0;

    public OpenXmlExcelBuilder()
    {
        _sheetData = new SheetData();
        _mergeCells = new MergeCells();
        _columns = new Columns();
        _rowIndex = 1;
    }

    public override BaseExcelBuilder AddHeader(string header, int startIndex, int count)
    {
        CreateCell(startIndex, _rowIndex, header, StyleIndex.BoldTextWithoutBorder);
        for (int i = startIndex + 1; i < startIndex + count; ++i)
        {
            CreateCell(i, _rowIndex, "", StyleIndex.SimpleTextWithoutBorder);
        }

        _mergeCells.Append(new MergeCell()
        {
            Reference =
            new StringValue($"{GetExcelColumnName(startIndex)}{_rowIndex}:{GetExcelColumnName(startIndex + count - 1)}{_rowIndex}")
        });

        _rowIndex++;
        return this;
    }

    public override BaseExcelBuilder AddParagraph(string text, int columnIndex)
    {
        CreateCell(columnIndex, _rowIndex++, text, StyleIndex.SimpleTextWithoutBorder);
        return this;
    }

    public override BaseExcelBuilder AddTable(int[] columnsWidths, List<string[]> data)
    {
        if (columnsWidths == null || columnsWidths.Length == 0)
        {
            throw new ArgumentNullException(nameof(columnsWidths));
        }

        if (data == null || data.Count == 0)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (data.Any(x => x.Length != columnsWidths.Length))
        {
            throw new InvalidOperationException("widths.Length != data.Length");
        }

        uint counter = 1;
        int coef = 1;
        _columns.Append(columnsWidths.Select(x => new Column
        {
            Min = counter,
            Max = counter++,
            Width = x * coef,
            CustomWidth = true
        }));

        for (var j = 0; j < data.First().Length; ++j)
        {
            CreateCell(j, _rowIndex, data.First()[j], StyleIndex.BoldTextWithBorder);
        }

        _rowIndex++;
        for (var i = 1; i < data.Count; ++i)
        {
            for (var j = 0; j < data[i].Length; ++j)
            {
                CreateCell(j, _rowIndex, data[i][j], StyleIndex.SimpleTextWithBorder);
            }

            _rowIndex++;
        }

        return this;
    }

    public override Stream Build()
    {
        var stream = new MemoryStream();
        using var spreadsheetDocument = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);
        var workbookpart = spreadsheetDocument.AddWorkbookPart();
        GenerateStyle(workbookpart);
        workbookpart.Workbook = new Workbook();
        var worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
        worksheetPart.Worksheet = new Worksheet();
        if (_columns.HasChildren)
        {
            worksheetPart.Worksheet.Append(_columns);
        }

        worksheetPart.Worksheet.Append(_sheetData);
        var sheets = spreadsheetDocument.WorkbookPart!.Workbook.AppendChild(new Sheets());
        var sheet = new Sheet()
        {
            Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
            SheetId = 1,
            Name = "Лист 1"
        };

        sheets.Append(sheet);
        if (_mergeCells.HasChildren)
        {
            worksheetPart.Worksheet.InsertAfter(_mergeCells, worksheetPart.Worksheet.Elements<SheetData>().First());
        }
        return stream;
    }

    private static void GenerateStyle(WorkbookPart workbookPart)
    {
        var workbookStylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
        workbookStylesPart.Stylesheet = new Stylesheet();

        var fonts = new Fonts() { Count = 2, KnownFonts = BooleanValue.FromBoolean(true) };
        fonts.Append(new DocumentFormat.OpenXml.Spreadsheet.Font
        {
            FontSize = new FontSize() { Val = 11 },
            FontName = new FontName() { Val = "Calibri" },
            FontFamilyNumbering = new FontFamilyNumbering() { Val = 2 },
            FontScheme = new FontScheme() { Val = new EnumValue<FontSchemeValues>(FontSchemeValues.Minor) }
        });
        fonts.Append(new DocumentFormat.OpenXml.Spreadsheet.Font
        {
            FontSize = new FontSize() { Val = 11 },
            FontName = new FontName() { Val = "Calibri" },
            FontFamilyNumbering = new FontFamilyNumbering() { Val = 2 },
            FontScheme = new FontScheme() { Val = new EnumValue<FontSchemeValues>(FontSchemeValues.Minor) },
            Bold = new Bold()
        });
        workbookStylesPart.Stylesheet.Append(fonts);

        // Default Fill
        var fills = new Fills() { Count = 1 };
        fills.Append(new Fill
        {
            PatternFill = new PatternFill() { PatternType = new EnumValue<PatternValues>(PatternValues.None) }
        });
        workbookStylesPart.Stylesheet.Append(fills);

        // Default Border
        var borders = new Borders() { Count = 2 };
        borders.Append(new Border
        {
            LeftBorder = new LeftBorder(),
            RightBorder = new RightBorder(),
            TopBorder = new TopBorder(),
            BottomBorder = new BottomBorder(),
            DiagonalBorder = new DiagonalBorder()
        });
        borders.Append(new Border
        {
            LeftBorder = new LeftBorder() { Style = BorderStyleValues.Thin },
            RightBorder = new RightBorder() { Style = BorderStyleValues.Thin },
            TopBorder = new TopBorder() { Style = BorderStyleValues.Thin },
            BottomBorder = new BottomBorder() { Style = BorderStyleValues.Thin }
        });
        workbookStylesPart.Stylesheet.Append(borders);

        // Default cell format and a date cell format
        var cellFormats = new CellFormats() { Count = 4 };
        cellFormats.Append(new CellFormat
        {
            NumberFormatId = 0,
            FormatId = 0,
            FontId = 0,
            BorderId = 0,
            FillId = 0,
            Alignment = new Alignment()
            {
                Horizontal = HorizontalAlignmentValues.Left,
                Vertical = VerticalAlignmentValues.Center,
                WrapText = true
            }
        });
        cellFormats.Append(new CellFormat
        {
            NumberFormatId = 0,
            FormatId = 0,
            FontId = 0,
            BorderId = 1,
            FillId = 0,
            Alignment = new Alignment()
            {
                Horizontal = HorizontalAlignmentValues.Left,
                Vertical = VerticalAlignmentValues.Center,
                WrapText = true
            }
        });
        cellFormats.Append(new CellFormat
        {
            NumberFormatId = 0,
            FormatId = 0,
            FontId = 1,
            BorderId = 0,
            FillId = 0,
            Alignment = new Alignment()
            {
                Horizontal = HorizontalAlignmentValues.Center,
                Vertical = VerticalAlignmentValues.Center,
                WrapText = true
            }
        });
        cellFormats.Append(new CellFormat
        {
            NumberFormatId = 0,
            FormatId = 0,
            FontId = 1,
            BorderId = 1,
            FillId = 0,
            Alignment = new Alignment()
            {
                Horizontal = HorizontalAlignmentValues.Center,
                Vertical = VerticalAlignmentValues.Center,
                WrapText = true
            }
        });
        workbookStylesPart.Stylesheet.Append(cellFormats);
    }

    private enum StyleIndex
    {
        SimpleTextWithoutBorder = 0,
        SimpleTextWithBorder = 1,
        BoldTextWithoutBorder = 2,
        BoldTextWithBorder = 3
    }

    private void CreateCell(int columnIndex, uint rowIndex, string text, StyleIndex styleIndex)
    {
        var columnName = GetExcelColumnName(columnIndex);
        var cellReference = columnName + rowIndex;
        var row = _sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex! == rowIndex);
        if (row == null)
        {
            row = new Row() { RowIndex = rowIndex };
            _sheetData.Append(row);
        }

        var newCell = row.Elements<Cell>()
            .FirstOrDefault(c => c.CellReference != null && c.CellReference.Value == columnName + rowIndex);
        if (newCell == null)
        {
            Cell? refCell = null;
            foreach (Cell cell in row.Elements<Cell>())
            {
                if (cell.CellReference?.Value != null && cell.CellReference.Value.Length == cellReference.Length)
                {
                    if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                    {
                        refCell = cell;
                        break;
                    }
                }
            }
            newCell = new Cell() { CellReference = cellReference };
            row.InsertBefore(newCell, refCell);
        }

        newCell.CellValue = new CellValue(text);
        newCell.DataType = CellValues.String;
        newCell.StyleIndex = (uint)styleIndex;
    }

    private static string GetExcelColumnName(int columnNumber)
    {
        columnNumber += 1;
        int dividend = columnNumber;
        string columnName = string.Empty;
        int modulo;

        while (dividend > 0)
        {
            modulo = (dividend - 1) % 26;
            columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
            dividend = (dividend - modulo) / 26;
        }

        return columnName;
    }
}
