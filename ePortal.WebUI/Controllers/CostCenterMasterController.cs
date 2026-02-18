using ClosedXML.Excel;
using ePortal.Persistence.TravelBilling.Interface;
using System.Data;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.Persistence.Interface;
using Serilog.Configuration;
using ePortal.Shared;
using ePortal.WebUI.Filters;
using System.Reflection;


namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    public class CostCenterMasterController : AdminBaseController
    {
        DataTable odt = new DataTable();

        //cCostCenterMaster oCostCenterMaster = new cCostCenterMaster();

        //cHelpDataProvider oHelpDataProvider = new cHelpDataProvider();

        private readonly IcCostCenterMaster oCostCenterMaster;
        private readonly IcHelpDataProvider oHelpDataProvider;
        private readonly ILogger<CostCenterMasterController> _logger;


        public CostCenterMasterController(IcCostCenterMaster _oCostCenterMaster, IcHelpDataProvider _oHelpDataProvider, ILogger<CostCenterMasterController> logger, IConfiguration settings, ISessionService sessionService, IDataManagement _oDataMgmt, IConnectionString connStr) : base(_oDataMgmt, connStr, logger, settings, sessionService)
        {
            oCostCenterMaster = _oCostCenterMaster;
            oHelpDataProvider = _oHelpDataProvider;

            _logger = logger;
        }

        // GET: CostCenterMaster
        public ActionResult Index()
        {
            //string Userid = Session["UserId"] == null ? null : Session["UserId"].ToString();
            string Userid = _userId == null ? null : _userId.ToString();

            if (Userid == null)
                return RedirectToAction("Index", "Login");

            return View();
        }

        public ActionResult Add()
        {
            //string Userid = Session["UserId"] == null ? null : Session["UserId"].ToString();
            string Userid = _userId == null ? null : _userId.ToString();

            if (Userid == null)
                return RedirectToAction("Index", "Login");

            ViewBag.SYSites = GetSites();
            ViewBag.SYDivision = GetSYDivision();

            return View();
        }

        public ActionResult Edit(int id)
        {
            //string Userid = Session["UserId"] == null ? null : Session["UserId"].ToString();
            string Userid = _userId == null ? null : _userId.ToString();

            if (Userid == null)
                return RedirectToAction("Index", "Login");

            if (id == null)
                return BadRequest();
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ViewBag.SYSites = GetSites();
            ViewBag.SYDivision = GetSYDivision();

            var list = new List<ADCOSTCENTERMASTER>();

            odt = oCostCenterMaster.GetCostCenterMasterBySrNo(id);

            foreach (DataRow row in odt.Rows)
            {
                var costCenter = new ADCOSTCENTERMASTER
                {
                    ADCOSTCENTERID = Convert.ToInt32(row["ADCOSTCENTERID"]),
                    ADCOSTCENTERCODE = Convert.ToInt32(row["ADCOSTCENTERCODE"]),
                    ADCOSTCENTERNAME = row["ADCOSTCENTERNAME"].ToString(),
                    ADSYSITE = row["ADSYSITE"].ToString(),
                    ADDIVISION = row["ADDIVISION"].ToString()
                };

                list.Add(costCenter);
            }

            var obj = list.LastOrDefault();

            if (obj == null)
                //return HttpNotFound();
                return NotFound();

            return View(obj);
        }

        public ActionResult ViewLog(int id)
        {
            //string Userid = Session["UserId"] == null ? null : Session["UserId"].ToString();
            string Userid = _userId == null ? null : _userId.ToString();

            if (Userid == null)
                return RedirectToAction("Index", "Login");

            if (id == null)
                return BadRequest();
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var list = new List<ADCOSTCENTERMASTER>();

            odt = oCostCenterMaster.GetCostCenterMasterBySrNo(id);

            foreach (DataRow row in odt.Rows)
            {
                var costCenter = new ADCOSTCENTERMASTER
                {
                    ADCOSTCENTERID = Convert.ToInt32(row["ADCOSTCENTERID"]),
                    ADCOSTCENTERCODE = Convert.ToInt32(row["ADCOSTCENTERCODE"]),
                    ADCOSTCENTERNAME = row["ADCOSTCENTERNAME"].ToString(),
                    ADSYSITE = row["ADSYSITE"].ToString(),
                    ADDIVISION = row["ADDIVISION"].ToString()
                };

                list.Add(costCenter);
            }

            var obj = list.LastOrDefault();

            if (obj == null)
                return NotFound();
                //return HttpNotFound();

            return View(obj);
        }

        public ActionResult GetCostCenterMasterLogList(string ADCOSTCENTERID)
        {
            try
            {
                //string Userid = Session["UserId"] == null ? null : Session["UserId"].ToString();
                string Userid = _userId == null ? null : _userId.ToString();

                if (Userid == null)
                    return RedirectToAction("Index", "Login");

                odt = oCostCenterMaster.GetCostCenterMasterLogList(ADCOSTCENTERID);

                var list = new List<ADCOSTCENTERMASTER_LOG>();

                foreach (DataRow row in odt.Rows)
                {
                    var costCenter = new ADCOSTCENTERMASTER_LOG
                    {
                        ADCOSTCENTERID = Convert.ToInt32(row["ADCOSTCENTERID"]),
                        //ADCOSTCENTERCODE = Convert.ToInt32(row["ADCOSTCENTERCODE"]),
                        ADCOSTCENTERNAME = row["ADCOSTCENTERNAME"].ToString(),
                        ADSYSITE = row["ADSYSITE"].ToString(),
                        ADDIVISION = row["ADDIVISION"].ToString(),
                        LOG_MODE = row["LOG_MODE"].ToString(),
                        ADDEDBY = row["ADDEDBY"].ToString(),
                        ADDEDDATE = row["ADDEDDATE"].ToString(),
                        MODIFIEDBY = row["MODIFIEDBY"].ToString(),
                        DATELSTMOD = row["DATELSTMOD"].ToString()

                    };

                    list.Add(costCenter);
                }

                return Json(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method: " + MethodBase.GetCurrentMethod()?.Name + ",Logged in User Id:" + _userId, ex.Message);
                return Json(new { error = ex.Message });
            }
        }

        //Functions....
        public ActionResult GetCostCenterMasterList([FromQuery]string SearchTex)
        {
            try
            {
                //string Userid = Session["UserId"] == null ? null : Session["UserId"].ToString();
                string Userid = _userId == null ? null : _userId.ToString();

                if (Userid == null)
                    return RedirectToAction("Index", "Login");

                odt = oCostCenterMaster.GetCostCenterMasterList(SearchTex);

                var list = new List<ADCOSTCENTERMASTER>();

                foreach (DataRow row in odt.Rows)
                {
                    var costCenter = new ADCOSTCENTERMASTER
                    {
                        ADCOSTCENTERID = Convert.ToInt32(row["ADCOSTCENTERID"]),
                        //ADCOSTCENTERCODE = Convert.ToInt32(row["ADCOSTCENTERCODE"]),
                        ADCOSTCENTERNAME = row["ADCOSTCENTERNAME"].ToString(),
                        ADSYSITE = row["ADSYSITE"].ToString(),
                        ADDIVISION = row["ADDIVISION"].ToString()
                    };

                    list.Add(costCenter);
                }

                return Json(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method: " + MethodBase.GetCurrentMethod()?.Name + ",Logged in User Id:" + _userId, ex.Message);
                return Json(new { error = ex.Message });
            }
        }

        public ActionResult DeleteCostCenterBySrNo(string SrNo)
        {
            try
            {
                //string Userid = Session["UserId"] == null ? null : Session["UserId"].ToString();
                string Userid = _userId == null ? null : _userId.ToString();

                if (Userid == null)
                    return RedirectToAction("Index", "Login");

                bool result = oCostCenterMaster.DeleteCostCenterBySrNo(SrNo, Userid);

                object Ans = new { status = "true", message = "" };

                return Json(Ans);
            }
            catch (Exception ex)
            {
                object Ans = new { status = "false", message = ex.Message };

                return Json(Ans);
            }
        }

        [HttpPost]
        public ActionResult SaveData([FromBody]ADCOSTCENTERMASTER inputJson)
        {
            try
            {
                //string Userid = Session["UserId"] == null ? null : Session["UserId"].ToString();
                string Userid = _userId == null ? null : _userId.ToString();

                if (Userid == null)
                    return RedirectToAction("Index", "Login");

                inputJson.ADDEDBY = Convert.ToInt32(Userid);

                bool result = oCostCenterMaster.InsertUpdateadCostCenterMaster(inputJson);

                object Ans = new { status = "true", message = "" };

                return Json(Ans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method: " + MethodBase.GetCurrentMethod()?.Name + ",Logged in User Id:" + _userId, ex.Message);
                object Ans = new { status = "false", message = ex.Message };

                return Json(Ans);
            }
        }

        public ActionResult Upload(IFormFile file)
        {
            try
            {
                //string Userid = Session["UserId"] == null ? null : Session["UserId"].ToString();
                string Userid = _userId == null ? null : _userId.ToString();

                if (Userid == null)
                    return RedirectToAction("Index", "Login");

                //if (file != null && file.ContentLength > 0)
                if (file != null && file.Length > 0)
                {
                    string filename = $"ErrorLog_{DateTime.Now.ToString("ddMMyyyyHHmmss")}.txt";

                    //string logFilePath = Server.MapPath($"~/Uploads/TravelBilling/Logs/{filename}");
                    string logFilePath = Path.Combine(serverpath.getFileUploadPath(), "TravelBilling","Logs",filename);

                    var logDirectory = Path.GetDirectoryName(logFilePath);

                    if (!Directory.Exists(logDirectory))
                        Directory.CreateDirectory(logDirectory);

                    var allowedExtensions = new[] { ".xls", ".xlsx" };
                    var fileExtension = Path.GetExtension(file.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                        throw new Exception("Invalid file type. Please upload an Excel file.");

                    var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    var extension = Path.GetExtension(file.FileName);
                    var dateStamp = DateTime.Now.ToString("ddMMyyyyHHmmss");
                    var newFileName = $"{fileName}_{dateStamp}{extension}";

                    //var uploadPath = Path.Combine(Server.MapPath("~/Uploads/TravelBilling/CostCenter"));
                    var uploadPath = Path.Combine(serverpath.getFileUploadPath(), "TravelBilling", "CostCenter");

                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    var filePath = Path.Combine(uploadPath, newFileName);

                    //file.SaveAs(filePath);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyToAsync(stream);
                    }

                    DataTable dataTable = ReadExcelToDataTable(filePath);

                    if (dataTable.Rows.Count == 0)
                        throw new Exception("No data found in the file.");

                    //string[] lines = System.IO.File.ReadAllLines(Path.Combine(Server.MapPath("~/Uploads/TravelBilling/SettingFile/CostCenter.txt")));
                    var SettingPath = Path.Combine(serverpath.getFileUploadPath(), "TravelBilling", "SettingFile", "CostCenter.txt");
                    string[] lines = System.IO.File.ReadAllLines(SettingPath);

                    Dictionary<string, FieldMapping> _fieldMapping = new Dictionary<string, FieldMapping>();

                    foreach (var itm in lines)
                    {
                        try
                        {
                            FieldMapping obj = new FieldMapping();

                            string[] columns = itm.Split(',');

                            obj.FieldName = columns[0];
                            obj.FieldIndex = Convert.ToInt32(columns[1]);
                            obj.FieldType = columns[2];

                            _fieldMapping.Add(columns[0], obj);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }

                    bool hasErrors = false;

                    using (StreamWriter writer = new StreamWriter(logFilePath, true))
                    {
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            DataRow dr = dataTable.Rows[i];
                            try
                            {
                                var inputJson = new ADCOSTCENTERMASTER();
                                bool flag = false;

                                foreach (var itm in _fieldMapping)
                                {
                                    string val = dr[itm.Value.FieldIndex] == null ? "" : dr[itm.Value.FieldIndex].ToString().Trim();

                                    if (itm.Key == "CostCenterName")
                                    {
                                        if (string.IsNullOrEmpty(val))
                                        {
                                            writer.WriteLine($"Row {i + 2}: 'Cost Center Name' field is mandatory.");
                                            flag = true;
                                            break;
                                        }
                                        else
                                        {
                                            inputJson.ADCOSTCENTERNAME = val;
                                        }
                                    }

                                    if (itm.Key == "Site")
                                    {
                                        if (string.IsNullOrEmpty(val))
                                        {
                                            writer.WriteLine($"Row {i + 2}: 'Site' field is mandatory.");
                                            flag = true;
                                            break;
                                        }
                                        else
                                        {
                                            inputJson.ADSYSITE = val;
                                        }
                                    }

                                    if (itm.Key == "Division")
                                        inputJson.ADDIVISION = val;
                                }

                                if (flag)
                                {
                                    hasErrors = true;
                                    continue;
                                }

                                inputJson.ADCOSTCENTERCODE = -1;
                                inputJson.ADDEDBY = Convert.ToInt32(Userid);

                                bool result = oCostCenterMaster.ImportCostCenterMaster(inputJson);
                            }
                            catch (Exception ex)
                            {
                                writer.WriteLine($"Row {i + 2}: Error occurred - {ex.Message}");
                            }
                        }

                        writer.Flush();
                    }

                    object Ans;

                    if (hasErrors && new FileInfo(logFilePath).Length > 0)
                    {
                        Ans = new { status = "true", message = "File uploaded with errors.", logfile = $"~/Uploads/TravelBilling/Logs/{filename}" };
                    }
                    else
                    {
                        Ans = new { status = "true", message = "File uploaded successfully.", logfile = "" };

                        if (System.IO.File.Exists(logFilePath))
                            System.IO.File.Delete(logFilePath);
                    }

                    return Json(Ans);
                }
                else
                    throw new Exception("Please select a file.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method: " + MethodBase.GetCurrentMethod()?.Name + ",Logged in User Id:" + _userId, ex.Message);
                object Ans = new { status = "false", message = ex.Message, logfile = "" };

                return Json(Ans);
            }
        }

        public ActionResult ExportToExcel(string SearchTex)
        {
            //string Userid = Session["UserId"] == null ? null : Session["UserId"].ToString();
            string Userid = _userId == null ? null : _userId.ToString();

            if (Userid == null)
                return RedirectToAction("Index", "Login");

            DataTable dataTable = oCostCenterMaster.GetCostCenterMasterList(SearchTex);

            string fileName = $"CostCenterMaster_{DateTime.Now:ddMMyyyy_HHmmss}.xlsx";

            //string filePath = Server.MapPath($"~/Uploads/TravelBilling/Export/{fileName}");
            string filePath = Path.Combine(serverpath.getFileUploadPath(),"TravelBilling","Export",fileName);

            dataTable.Columns.Remove("ADCOSTCENTERID");

            ExportDataTableToExcel(dataTable, filePath);

            byte[] fileContents = System.IO.File.ReadAllBytes(filePath);

            System.IO.File.Delete(filePath);

            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        //private DataTable ReadExcelToDataTable(string filePath)
        //{
        //    DataTable dataTable = new DataTable();
        //    Excel.Application excelApp = null;
        //    Excel.Workbook workbook = null;
        //    Excel.Worksheet worksheet = null;
        //    Excel.Range range = null;

        //    try
        //    {
        //        excelApp = new Excel.Application();
        //        workbook = excelApp.Workbooks.Open(filePath);
        //        worksheet = (Excel.Worksheet)workbook.Sheets[1];
        //        range = worksheet.UsedRange;

        //        int rowCount = range.Rows.Count;
        //        int colCount = range.Columns.Count;

        //        for (int col = 1; col <= colCount; col++)
        //        {
        //            string columnName = Convert.ToString((range.Cells[1, col] as Excel.Range).Value2);
        //            dataTable.Columns.Add(columnName ?? $"Column{col}");
        //        }

        //        for (int row = 2; row <= rowCount; row++)
        //        {
        //            DataRow dataRow = dataTable.NewRow();
        //            for (int col = 1; col <= colCount; col++)
        //            {
        //                dataRow[col - 1] = Convert.ToString((range.Cells[row, col] as Excel.Range).Value2);
        //            }
        //            dataTable.Rows.Add(dataRow);
        //        }
        //    }
        //    finally
        //    {
        //        if (workbook != null) workbook.Close(false);
        //        if (excelApp != null) excelApp.Quit();

        //        if (range != null) Marshal.ReleaseComObject(range);
        //        if (worksheet != null) Marshal.ReleaseComObject(worksheet);
        //        if (workbook != null) Marshal.ReleaseComObject(workbook);
        //        if (excelApp != null) Marshal.ReleaseComObject(excelApp);

        //        range = null;
        //        worksheet = null;
        //        workbook = null;
        //        excelApp = null;

        //        GC.Collect();
        //        GC.WaitForPendingFinalizers();
        //    }

        //    return dataTable;
        //}
        private DataTable ReadExcelToDataTable(string filePath)
        {
            var dataTable = new DataTable();

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1); // First sheet
                bool firstRow = true;

                foreach (var row in worksheet.RowsUsed())
                {
                    if (firstRow)
                    {
                        // Add columns from the first row
                        foreach (var cell in row.Cells())
                        {
                            dataTable.Columns.Add(cell.Value.ToString());
                        }
                        firstRow = false;
                    }
                    else
                    {
                        // Add data rows
                        var dataRow = dataTable.NewRow();
                        int i = 0;
                        foreach (var cell in row.Cells(1, dataTable.Columns.Count))
                        {
                            dataRow[i++] = cell.Value.ToString();
                        }
                        dataTable.Rows.Add(dataRow);
                    }
                }
            }

            return dataTable;
        }

        //Helper Function
        private IEnumerable<SelectListItem> GetSites()
        {
            odt = oHelpDataProvider.GetSYSitesList();

            var SYSites = new List<SelectListItem>();

            if (odt.Rows.Count > 0)
            {
                foreach (DataRow dr in odt.Rows)
                {
                    try
                    {
                        var obj = new SelectListItem { Value = $"{dr["SYSITEID"].ToString()}", Text = dr["DESCRIP"].ToString() };

                        SYSites.Add(obj);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in Method: " + MethodBase.GetCurrentMethod()?.Name + ",Logged in User Id:" + _userId, ex.Message);
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            }

            return SYSites;
        }

        private IEnumerable<SelectListItem> GetSYDivision()
        {
            odt = oHelpDataProvider.GetSYDivisionList();

            var SYDivision = new List<SelectListItem>();

            if (odt.Rows.Count > 0)
            {
                foreach (DataRow dr in odt.Rows)
                {
                    try
                    {
                        var obj = new SelectListItem { Value = $"{dr["ADORGLEVELSAPID"].ToString()}", Text = dr["LEVELDESCRIP"].ToString() };

                        SYDivision.Add(obj);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in Method: " + MethodBase.GetCurrentMethod()?.Name + ",Logged in User Id:" + _userId, ex.Message);
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            }

            return SYDivision;
        }

        //private void ExportDataTableToExcel(DataTable dataTable, string filePath)
        //{
        //    Excel.Application excelApp = null;
        //    Excel.Workbook workbook = null;
        //    Excel.Worksheet worksheet = null;

        //    try
        //    {
        //        excelApp = new Excel.Application();
        //        workbook = excelApp.Workbooks.Add(Type.Missing);
        //        worksheet = (Excel.Worksheet)workbook.ActiveSheet;
        //        worksheet.Name = "Sheet1";

        //        // Adding column headers
        //        for (int col = 0; col < dataTable.Columns.Count; col++)
        //        {
        //            worksheet.Cells[1, col + 1] = dataTable.Columns[col].ColumnName;
        //            worksheet.Cells[1, col + 1].Font.Bold = true;
        //            worksheet.Cells[1, col + 1].Interior.Color = Excel.XlRgbColor.rgbLightGray;

        //            Excel.Range headerCell = worksheet.Cells[1, col + 1];
        //            headerCell.Value = dataTable.Columns[col].ColumnName;
        //            headerCell.Font.Bold = true;
        //            headerCell.Interior.Color = Excel.XlRgbColor.rgbLightGray;

        //            AddBorders(headerCell);
        //        }

        //        // Adding rows
        //        for (int row = 0; row < dataTable.Rows.Count; row++)
        //        {
        //            for (int col = 0; col < dataTable.Columns.Count; col++)
        //            {
        //                var cellValue = dataTable.Rows[row][col];
        //                Excel.Range dataCell = worksheet.Cells[row + 2, col + 1];
        //                dataCell.Value = cellValue;

        //                if (dataTable.Columns[col].DataType == typeof(DateTime))
        //                    dataCell.NumberFormat = "dd-mm-yyyy";
        //                else if (dataTable.Columns[col].DataType == typeof(decimal) || dataTable.Columns[col].DataType == typeof(double))
        //                    dataCell.NumberFormat = "#,##0.00";
        //                else
        //                    dataCell.NumberFormat = "@";

        //                AddBorders(dataCell);
        //            }
        //        }

        //        worksheet.Columns.AutoFit();

        //        workbook.SaveAs(filePath);
        //    }
        //    finally
        //    {
        //        if (workbook != null) workbook.Close(false, Type.Missing, Type.Missing);
        //        if (excelApp != null) excelApp.Quit();

        //        if (worksheet != null) Marshal.ReleaseComObject(worksheet);
        //        if (workbook != null) Marshal.ReleaseComObject(workbook);
        //        if (excelApp != null) Marshal.ReleaseComObject(excelApp);

        //        worksheet = null;
        //        workbook = null;
        //        excelApp = null;

        //        GC.Collect();
        //        GC.WaitForPendingFinalizers();
        //    }
        //}

        //private void AddBorders(Excel.Range range)
        //{
        //    range.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
        //    range.Borders[Excel.XlBordersIndex.xlEdgeBottom].ColorIndex = Excel.XlRgbColor.rgbBlack;
        //    range.Borders[Excel.XlBordersIndex.xlEdgeBottom].TintAndShade = 0;
        //    range.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlThin;

        //    range.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
        //    range.Borders[Excel.XlBordersIndex.xlEdgeLeft].ColorIndex = Excel.XlRgbColor.rgbBlack;
        //    range.Borders[Excel.XlBordersIndex.xlEdgeLeft].TintAndShade = 0;
        //    range.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlThin;

        //    range.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
        //    range.Borders[Excel.XlBordersIndex.xlEdgeRight].ColorIndex = Excel.XlRgbColor.rgbBlack;
        //    range.Borders[Excel.XlBordersIndex.xlEdgeRight].TintAndShade = 0;
        //    range.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = Excel.XlBorderWeight.xlThin;

        //    range.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
        //    range.Borders[Excel.XlBordersIndex.xlEdgeTop].ColorIndex = Excel.XlRgbColor.rgbBlack;
        //    range.Borders[Excel.XlBordersIndex.xlEdgeTop].TintAndShade = 0;
        //    range.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlThin;
        //}

        private void ExportDataTableToExcel(DataTable dataTable, string filePath)
        {
            try
            {
                // Create a new workbook
                using (var workbook = new XLWorkbook())
                {
                    // Add a worksheet named "Sheet1"
                    var worksheet = workbook.Worksheets.Add("Sheet1");

                    // Adding column headers
                    for (int col = 0; col < dataTable.Columns.Count; col++)
                    {
                        var headerCell = worksheet.Cell(1, col + 1);
                        headerCell.Value = GetTextName(dataTable.Columns[col].ColumnName.ToString());
                        headerCell.Style.Font.Bold = true;
                        headerCell.Style.Fill.BackgroundColor = XLColor.LightBlue;

                        AddBorders(headerCell);  // Custom function for adding borders
                    }

                    // Adding rows
                    for (int row = 0; row < dataTable.Rows.Count; row++)
                    {
                        for (int col = 0; col < dataTable.Columns.Count; col++)
                        {
                            var cell = worksheet.Cell(row + 2, col + 1);
                            var cellValue = dataTable.Rows[row][col];

                            // Set cell value
                            //cell.Value = (XLCellValue)cellValue;
                            cell.SetValue(cellValue?.ToString());
                            // Apply number formats
                            if (dataTable.Columns[col].DataType == typeof(DateTime))
                                cell.Style.DateFormat.Format = "dd-MM-yyyy";
                            else if (dataTable.Columns[col].DataType == typeof(decimal) || dataTable.Columns[col].DataType == typeof(double))
                                cell.Style.NumberFormat.Format = "#,##0.00";
                            else
                                cell.Style.NumberFormat.Format = "@"; // Text format

                            // Add borders to the cell
                            AddBorders(cell);
                        }
                    }

                    // Auto fit the columns
                    worksheet.Columns().AdjustToContents();

                    // Save the workbook to the specified file path
                    workbook.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method: " + MethodBase.GetCurrentMethod()?.Name + ",Logged in User Id:" + _userId, ex.Message);
                // Handle exceptions as needed (e.g., logging)
                throw new Exception($"Error occurred while exporting data to Excel: {ex.Message}", ex);
            }
        }

        // Helper function to add borders (replace this with your actual border logic)
        private void AddBorders(IXLCell cell)
        {
            var borderStyle = XLBorderStyleValues.Thin;
            cell.Style.Border.TopBorder = borderStyle;
            cell.Style.Border.BottomBorder = borderStyle;
            cell.Style.Border.LeftBorder = borderStyle;
            cell.Style.Border.RightBorder = borderStyle;
        }
        public static string GetTextName(string columnName)
        {
            switch (columnName)
            {
                case "ADCOSTCENTERNAME":
                    return "Cost Center";
                case "ADSYSITE":
                    return "Site";
                case "ADDIVISION":
                    return "Division";

                default:
                    return columnName;
            }
        }
    }

    public class FieldMapping
    {
        public string FieldName { get; set; }

        public int FieldIndex { get; set; }

        public string FieldType { get; set; }
    }

}