using System.Data;
using ClosedXML.Excel;

namespace ePortal.WebUI.Helpers
{
    public class ExcelExportHelper
    {
        //public byte[] ExportToExcel(DataTable dt, string sheetName = "Report")
        //{
        //    using (var workbook = new XLWorkbook())
        //    {
        //        var ws = workbook.Worksheets.Add(sheetName);

        //        // Add DataTable as table (includes headers automatically)
        //        ws.Cell(1, 1).InsertTable(dt, true);

        //        // Optional formatting
        //        ws.Columns().AdjustToContents();

        //        using (var stream = new MemoryStream())
        //        {
        //            workbook.SaveAs(stream);
        //            return stream.ToArray();
        //        }
        //    }
        //}

        public byte[] ExportToExcel(DataTable dt, string sheetName = "Report")
        {
            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add(sheetName);

                int row = 1;
                int col = 1;

                // === 1️⃣ Add headers dynamically ===
                foreach (DataColumn column in dt.Columns)
                {
                    ws.Cell(row, col).Value = column.ColumnName;
                    ws.Cell(row, col).Style.Font.Bold = true;
                    ws.Cell(row, col).Style.Fill.BackgroundColor = XLColor.LightGray;
                    ws.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    col++;
                }

                // === 2️⃣ Add data rows ===
                row = 2;
                foreach (DataRow dr in dt.Rows)
                {
                    col = 1;
                    foreach (DataColumn column in dt.Columns)
                    {
                        ws.Cell(row, col).Value = dr[column]?.ToString();
                        ws.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        col++;
                    }
                    row++;
                }

                // === 3️⃣ Adjust columns ===
                ws.Columns().AdjustToContents();

                // === 4️⃣ Save to memory and return as byte[] ===
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }


    }
}
