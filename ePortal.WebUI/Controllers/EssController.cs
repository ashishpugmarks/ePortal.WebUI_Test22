using System.Data;
using System.Globalization;
using System.Net;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ePortal.Infrastructure.Repositories;
using ePortal.Persistence;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.ViewModels.APPX;
using ePortal.WebUI.Filters;

using static ePortal.ViewModels.APPX.EssViewModel;
using DocumentFormat.OpenXml.Spreadsheet;
using ePortal.Shared.Services;
using System.Data.Entity.Core.Metadata.Edm;
using iText.StyledXmlParser.Jsoup.Helper;
using System.Text.Json;
using DocumentFormat.OpenXml.VariantTypes;
using System.Threading;
using static ePortal.ViewModels.APIMapper;
using System.Linq;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.css;
using iTextSharp.tool.xml.pipeline.html;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.parser;
using System.Text.RegularExpressions;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    [SessionTimeout]
    public class EssController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly IEssServices _objEss;
        private readonly CommonRepository _cmr;
        private readonly IEportalESS _ePortalESS;
        private readonly IPMS_DAL _objpms;
        private readonly ILeaveApps_DAL _objleave;
        private readonly IExcelExport _excelExport;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly ICommonFunctions _common;
        private readonly IEmpIDDetail objempid;
        private readonly IAppConfigurationService _appConfig;
        public EssController(IEssServices essServices, ISessionService sessionService, CommonRepository commonRepository, IEportalESS objePortalESS, IPMS_DAL objpms, ILeaveApps_DAL objleave, IExcelExport excelExport, ICompositeViewEngine viewEngine, ITempDataProvider tempDataProvider, ICommonFunctions common, IEmpIDDetail _objempid, IAppConfigurationService appConfig)
        {
            _sessionService = sessionService;
            _objEss = essServices;
            _cmr = commonRepository;
            _ePortalESS = objePortalESS;
            _objpms = objpms;
            _objleave = objleave;
            _excelExport = excelExport;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _common = common;
            objempid = _objempid;
            _appConfig = appConfig;
        }

        public IActionResult EssDetail()
        {
            var employee = _sessionService.Get<Employee_Details>("Employee");

            var parmValues = _cmr.GetParameterValue("DESG_STAFFHOMEPAGE_VALIDATION")
                     .Split(',', StringSplitOptions.RemoveEmptyEntries);

            if (parmValues.Contains(employee.Designation_Id.ToString()))
            {
                return RedirectToAction("EssDetailStaff");
            }

            return View();
        }
        public IActionResult EmployeeAccountStatement()
        {
            int curYear = DateTime.Now.Year;
            int curMonth = DateTime.Now.Month;

            var fiscalYears = Enumerable.Range(2014, curYear - 2014 + 1)
                .Select(y => new SelectListItem { Text = y.ToString(), Value = y.ToString() })
                .ToList();

            // Fiscal month mapping (April = 1, … March = 12)
            int fiscalMonth = ((curMonth + 8) % 12) + 1;

            // Month list (Apr–Mar fiscal year)
            var months = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Apr", Value = "1" },
                    new SelectListItem { Text = "May", Value = "2" },
                    new SelectListItem { Text = "Jun", Value = "3" },
                    new SelectListItem { Text = "Jul", Value = "4" },
                    new SelectListItem { Text = "Aug", Value = "5" },
                    new SelectListItem { Text = "Sep", Value = "6" },
                    new SelectListItem { Text = "Oct", Value = "7" },
                    new SelectListItem { Text = "Nov", Value = "8" },
                    new SelectListItem { Text = "Dec", Value = "9" },
                    new SelectListItem { Text = "Jan", Value = "10" },
                    new SelectListItem { Text = "Feb", Value = "11" },
                    new SelectListItem { Text = "Mar", Value = "12" }
                };

            var model = new EmployeeAccountStatementViewModel
            {
                FiscalYear = curYear.ToString(),
                MonthFrom = fiscalMonth.ToString(),
                MonthTo = fiscalMonth.ToString(),
                FiscalYears = fiscalYears,
                Months = months,

                Categories = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Tour & Travel", Value = "N" },
                    new SelectListItem { Text = "Ticket Settlement", Value = "Z" },
                    new SelectListItem { Text = "Product Loan", Value = "U" },
                    new SelectListItem { Text = "Medical Loan", Value = "Y" },
                    new SelectListItem { Text = "Imprest Account", Value = "4" },
                    new SelectListItem { Text = "Salary Advance", Value = "T" }
                }
            };
            return View(model);
        }
        [HttpPost]
        public ActionResult EmployeeAccountStatement_searchbtn([FromBody] EmployeeAccountStatementViewModel objEss)
        {

            string userId = _sessionService.Get<string>("userID");
            string category = objEss?.Category;
            string monthFrom = objEss?.MonthFrom;
            string monthTo = objEss?.MonthTo;
            string fiscalYear = objEss?.FiscalYear ?? DateTime.Now.Year.ToString();

            DataTable dt = new DataTable();
            //dt = null;
            //tbl_travel.Visible = false;
            ViewBag.div_main_travel = false;
            //tbl_ticket.Visible = false;
            //div_main_ticket.Visible = false;
            //tbl_ProductLoan.Visible = false;
            //div_main_ProductLoan.Visible = false;
            //tbl_MedicalLoan.Visible = false;
            //div_main_MedicalLoan.Visible = false;
            //tbl_Imprest.Visible = false;
            //div_main_Imprest.Visible = false;
            //tbl_SalaryAdvance.Visible = false;
            //div_main_SalaryAdvance.Visible = false;
            //diverrmsg.Visible = false;
            ////currentbalance.Visible = false;
            //GET_EMPACCSTATEMENT_BALANCE();

            // Get balance (you may need to adapt this to return a value)
            ViewBag.Balance = GET_EMPACCSTATEMENT_BALANCE(userId, category, monthFrom, monthTo, fiscalYear);

            try
            {
                dt = GET_EMPACCSTATEMENT_DATATABLE(userId, category, monthFrom, monthTo, fiscalYear);
                // if (ddlcategory.SelectedValue == "N")
                if (objEss != null && objEss.Category == "N")
                {
                    ViewBag.div_main_travel = true;
                    objEss.TravelRecords = dt.AsEnumerable()
                        .Select(row => new TravelRecord
                        {
                            STATUS = row["STATUS"]?.ToString(),
                            PSTNG_DATE = row["PSTNG_DATE"]?.ToString(),
                            ALLOC_NMBR = row["ALLOC_NMBR"]?.ToString(),
                            DEBIT = row["DEBIT"]?.ToString(),
                            CREDIT = row["CREDIT"]?.ToString(),
                            CUMULATIVE_BALANCE = row["CUMULATIVE_BALANCE"]?.ToString(),
                            PARTICULAR = row["PARTICULAR"]?.ToString(),
                            ITEM_TEXT = row["ITEM_TEXT"]?.ToString()
                        }).ToList();
                    //div_main_travel.Visible = true;
                    //tbl_travel.Visible = true;
                    //Rep_Travel.DataSource = dt;
                    //Rep_Travel.DataBind();
                    //lbltravelrecordcnt.Text = "Total Record Found: " + dt.Rows.Count;
                    //if (dt.Rows.Count == 0)
                    //{
                    //    Nodatadiv_Travel.Visible = true;
                    //    div_travel.Visible = false;
                    //}
                }
                if (objEss != null && objEss.Category == "Z")
                {
                    ViewBag.div_main_ticket = true;
                    ViewBag.tbl_ticket = true;
                    ViewBag.lblticketrecordcnt = "Total Record Found: " + dt.Rows.Count;

                    if (dt.Rows.Count == 0)
                    {
                        ViewBag.Nodatadiv_Ticket = true;
                        ViewBag.div_ticket = false;
                    }
                    else
                    {
                        ViewBag.Nodatadiv_Ticket = false;
                        ViewBag.div_ticket = true;

                        if (dt.Rows.Count > 20)
                        {
                            ViewBag.div_ticket_class = "freezetbl";
                        }
                        else
                        {
                            ViewBag.div_ticket_class = "freezetblnone";
                        }
                    }

                    objEss.TicketList = dt.AsEnumerable()
                        .Select(row => new TravelRecord
                        {
                            STATUS = row["STATUS"]?.ToString(),
                            PSTNG_DATE = row["PSTNG_DATE"]?.ToString(),
                            ALLOC_NMBR = row["ALLOC_NMBR"]?.ToString(),
                            DEBIT = row["DEBIT"]?.ToString(),
                            CREDIT = row["CREDIT"]?.ToString(),
                            CUMULATIVE_BALANCE = row["CUMULATIVE_BALANCE"]?.ToString(),
                            PARTICULAR = row["PARTICULAR"]?.ToString(),
                            ITEM_TEXT = row["ITEM_TEXT"]?.ToString()
                        }).ToList();
                }
                //if (ddlcategory.SelectedValue == "U")
                if (objEss != null && objEss.Category == "U")
                {
                    ViewBag.div_main_ProductLoan = true;
                    ViewBag.tbl_ProductLoan = true;
                    ViewBag.lblproductloanrecordcnt = "Total Record Found: " + dt.Rows.Count;

                    if (dt.Rows.Count == 0)
                    {
                        ViewBag.Nodatadiv_Productloan = true;
                        ViewBag.div_ProductLoan = false;
                    }
                    else
                    {
                        ViewBag.Nodatadiv_Productloan = false;
                        ViewBag.div_ProductLoan = true;

                        if (dt.Rows.Count > 20)
                        {
                            ViewBag.div_ProductLoan_class = "freezetbl";
                        }
                        else
                        {
                            ViewBag.div_ProductLoan_class = "freezetblnone";
                        }
                    }

                    objEss.ProductLoanList = dt.AsEnumerable()
                        .Select(row => new TravelRecord
                        {
                            STATUS = row["STATUS"]?.ToString(),
                            PSTNG_DATE = row["PSTNG_DATE"]?.ToString(),
                            ALLOC_NMBR = row["ALLOC_NMBR"]?.ToString(),
                            DEBIT = row["DEBIT"]?.ToString(),
                            CREDIT = row["CREDIT"]?.ToString(),
                            CUMULATIVE_BALANCE = row["CUMULATIVE_BALANCE"]?.ToString(),
                            PARTICULAR = row["PARTICULAR"]?.ToString(),
                            ITEM_TEXT = row["ITEM_TEXT"]?.ToString()
                        }).ToList();
                }

                //if (ddlcategory.SelectedValue == "Y")
                if (objEss != null && objEss.Category == "Y")
                {
                    ViewBag.div_main_MedicalLoan = true;
                    ViewBag.tbl_MedicalLoan = true;
                    ViewBag.lblmedicalloanrecordcnt = "Total Record Found: " + dt.Rows.Count;

                    if (dt.Rows.Count == 0)
                    {
                        ViewBag.Nodatadiv_Medicalloan = true;
                        ViewBag.div_MedicalLoan = false;
                    }
                    else
                    {
                        ViewBag.Nodatadiv_Medicalloan = false;
                        ViewBag.div_MedicalLoan = true;

                        if (dt.Rows.Count > 20)
                        {
                            ViewBag.div_MedicalLoan_class = "freezetbl";
                        }
                        else
                        {
                            ViewBag.div_MedicalLoan_class = "freezetblnone";
                        }
                    }

                    objEss.MedicalLoanList = dt.AsEnumerable()
                        .Select(row => new TravelRecord
                        {
                            STATUS = row["STATUS"]?.ToString(),
                            PSTNG_DATE = row["PSTNG_DATE"]?.ToString(),
                            ALLOC_NMBR = row["ALLOC_NMBR"]?.ToString(),
                            DEBIT = row["DEBIT"]?.ToString(),
                            CREDIT = row["CREDIT"]?.ToString(),
                            CUMULATIVE_BALANCE = row["CUMULATIVE_BALANCE"]?.ToString(),
                            PARTICULAR = row["PARTICULAR"]?.ToString(),
                            ITEM_TEXT = row["ITEM_TEXT"]?.ToString()
                        }).ToList();
                }


                //if (ddlcategory.SelectedValue == "4")
                if (objEss != null && objEss.Category == "4")
                {
                    ViewBag.div_main_Imprest = true;
                    ViewBag.tbl_Imprest = true;
                    ViewBag.lblimprestrecordcnt = "Total Record Found: " + dt.Rows.Count;

                    if (dt.Rows.Count == 0)
                    {
                        ViewBag.Nodatadiv_Imprest = true;
                        ViewBag.div_Imprest = false;
                    }
                    else
                    {
                        ViewBag.Nodatadiv_Imprest = false;
                        ViewBag.div_Imprest = true;

                        if (dt.Rows.Count > 20)
                        {
                            ViewBag.div_Imprest_class = "freezetbl";
                        }
                        else
                        {
                            ViewBag.div_Imprest_class = "freezetblnone";
                        }
                    }

                    objEss.ImprestList = dt.AsEnumerable()
                        .Select(row => new TravelRecord
                        {
                            STATUS = row["STATUS"]?.ToString(),
                            PSTNG_DATE = row["PSTNG_DATE"]?.ToString(),
                            ALLOC_NMBR = row["ALLOC_NMBR"]?.ToString(),
                            DEBIT = row["DEBIT"]?.ToString(),
                            CREDIT = row["CREDIT"]?.ToString(),
                            CUMULATIVE_BALANCE = row["CUMULATIVE_BALANCE"]?.ToString(),
                            PARTICULAR = row["PARTICULAR"]?.ToString(),
                            ITEM_TEXT = row["ITEM_TEXT"]?.ToString()
                        }).ToList();
                }
                //if (ddlcategory.SelectedValue == "T")
                if (objEss != null && objEss.Category == "T")
                {
                    ViewBag.div_main_SalaryAdvance = true;
                    ViewBag.tbl_SalaryAdvance = true;
                    ViewBag.lblsaladvrecordcnt = "Total Record Found: " + dt.Rows.Count;

                    if (dt.Rows.Count == 0)
                    {
                        ViewBag.Nodatadiv_SalaryAdvance = true;
                        ViewBag.div_SalaryAdvance = false;
                    }
                    else
                    {
                        ViewBag.Nodatadiv_SalaryAdvance = false;
                        ViewBag.div_SalaryAdvance = true;

                        if (dt.Rows.Count > 20)
                        {
                            ViewBag.div_SalaryAdvance_class = "freezetbl";
                        }
                        else
                        {
                            ViewBag.div_SalaryAdvance_class = "freezetblnone";
                        }
                    }

                    objEss.SalaryAdvanceList = dt.AsEnumerable()
                        .Select(row => new TravelRecord
                        {
                            STATUS = row["STATUS"]?.ToString(),
                            PSTNG_DATE = row["PSTNG_DATE"]?.ToString(),
                            ALLOC_NMBR = row["ALLOC_NMBR"]?.ToString(),
                            DEBIT = row["DEBIT"]?.ToString(),
                            CREDIT = row["CREDIT"]?.ToString(),
                            CUMULATIVE_BALANCE = row["CUMULATIVE_BALANCE"]?.ToString(),
                            PARTICULAR = row["PARTICULAR"]?.ToString(),
                            ITEM_TEXT = row["ITEM_TEXT"]?.ToString()
                        }).ToList();


                }
            }
            catch (Exception ex)
            {
                //    viewModel.ShowErrorMessage = true;
                //    viewModel.ErrorMessage = ex.Message;
            }

            //return Json(viewModel);
            return View("_GetEmployeeAccountStatement", objEss);
        }

        public IActionResult GET_EMPACCSTATEMENT_BALANCE(string userId, string category, string monthFrom, string monthTo, string fiscalYear)
        {
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            //DataView DV = new DataView();
            dt = null;

            int CurYear = DateTime.Now.Year;
            int Curmonth = DateTime.Now.Month;
            int curfmont = 1;
            if (Curmonth == 4)
                curfmont = 01;
            if (Curmonth == 5)
                curfmont = 02;
            if (Curmonth == 6)
                curfmont = 03;
            if (Curmonth == 7)
                curfmont = 04;
            if (Curmonth == 8)
                curfmont = 05;
            if (Curmonth == 9)
                curfmont = 06;
            if (Curmonth == 10)
                curfmont = 07;
            if (Curmonth == 11)
                curfmont = 08;
            if (Curmonth == 12)
                curfmont = 09;
            if (Curmonth == 1)
                curfmont = 10;
            if (Curmonth == 2)
                curfmont = 11;
            if (Curmonth == 3)
                curfmont = 12;
            Int32 strecode = Convert.ToInt32(_sessionService.Get<string>("userID"));
            string strecode1 = strecode.ToString("0000000000");

            //*************Retreiving address detail; of employee from SAP
            //dt = objessaddress.GetEmployeeACStatement(strecode1, ddlcategory.SelectedValue.ToString(), ddlmonthfrom.SelectedValue.ToString(), ddlmonthto.SelectedValue.ToString(), ddlfiscal.SelectedValue.ToString());
            dt = _ePortalESS.GetEmployeeACStatement(strecode1, category, monthFrom, monthTo, fiscalYear).Result;


            //dt.DefaultView.RowFilter = "PARTICULAR='Closing Balance'";
            //dt1 = dt.DefaultView.ToTable();

            return View();
        }

        private DataTable GET_EMPACCSTATEMENT_DATATABLE(string userId, string category, string monthFrom, string monthTo, string fiscalYear)
        {
            DataTable dt = new DataTable();
            //DataTable dt1 = new DataTable();
            //dt = null;
            Int32 strecode = Convert.ToInt32(userId);
            string strecode1 = strecode.ToString("0000000000");

            //EportalESS objessaddress = new EportalESS();
            //*************Retreiving address detail; of employee from SAP
            //dt = objessaddress.GetEmployeeACStatement(strecode1, ddlcategory.SelectedValue.ToString(), ddlmonthfrom.SelectedValue.ToString(), ddlmonthto.SelectedValue.ToString(), ddlfiscal.SelectedValue.ToString());
            dt = _ePortalESS.GetEmployeeACStatement(strecode1, category, monthFrom, monthTo, fiscalYear).Result;

            // Define columns
            dt.Columns.Add("TransactionType", typeof(string));
            dt.Columns.Add("Date", typeof(DateTime));
            dt.Columns.Add("TransactionId", typeof(string));
            dt.Columns.Add("Debit", typeof(decimal));
            dt.Columns.Add("Credit", typeof(decimal));
            dt.Columns.Add("Balance", typeof(decimal));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("Status", typeof(string));

            //***************End of address detail
            //objDtl.Zfii_Emp_Acc_Statement(ddlcategory.SelectedValue.ToString(), strecode1, ddlmonthfrom.SelectedValue.ToString(), ddlmonthto.SelectedValue.ToString(), ddlfiscal.SelectedValue.ToString(), out Return0, ref empacc);
            // objDtl.Zfii_Emp_Acc_Statement("U", "0000009844", "05", "05", "2016", out Return0, ref empacc);

            DataTable dt_empacc = new DataTable();
            dt_empacc.Columns.Add("PARTICULAR");
            dt_empacc.Columns.Add("PSTNG_DATE");
            dt_empacc.Columns.Add("ALLOC_NMBR");
            dt_empacc.Columns.Add("DEBIT");
            dt_empacc.Columns.Add("CREDIT");
            dt_empacc.Columns.Add("CUMULATIVE_BALANCE");
            dt_empacc.Columns.Add("ITEM_TEXT");
            dt_empacc.Columns.Add("STATUS");
            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                var dateValue = dt.Rows[i][1]?.ToString();
                if (!string.IsNullOrWhiteSpace(dateValue) && DateTime.TryParse(dateValue, out DateTime parsed))
                {
                    dateValue = parsed.ToString("dd.MM.yyyy");
                }
                else
                {
                    dateValue = "";
                }

                DataRow drrow;
                drrow = dt_empacc.NewRow();
                drrow[0] = dt.Rows[i][0].ToString();
                drrow[1] = dateValue;//dt.Rows[i][1].ToString();//.Substring(8, 2) + "." + dt.Rows[i][1].ToString().Substring(5, 2) + "." + dt.Rows[i][1].ToString().Substring(0, 4);
                drrow[2] = dt.Rows[i][2].ToString();
                drrow[3] = decimal.Round(Convert.ToDecimal(dt.Rows[i][3].ToString()), 2).ToString("N", new CultureInfo("en-US"));
                drrow[4] = decimal.Round(Convert.ToDecimal(dt.Rows[i][4].ToString()), 2).ToString("N", new CultureInfo("en-US"));
                drrow[5] = decimal.Round(Convert.ToDecimal(dt.Rows[i][5].ToString()), 2).ToString("N", new CultureInfo("en-US"));
                drrow[6] = dt.Rows[i][6].ToString();
                drrow[7] = dt.Rows[i][7].ToString();
                if (dt.Rows[i][0].ToString() == "Closing Balance")
                {
                    drrow[1] = "";
                    drrow[2] = "";
                    drrow[6] = "";
                    drrow[7] = "";
                }
                if (dt.Rows[i][0].ToString() == "Opening Balance")
                {
                    drrow[1] = "";
                    drrow[2] = "";
                    drrow[6] = "";
                    drrow[7] = "";
                }
                dt_empacc.Rows.Add(drrow);
            }

            return dt_empacc;
        }


        public IActionResult Form16Display()
        {
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");

            string strpanno = employeeDetails.PancardNo;
            //string strfnyear = session.GetString("fnsyear");
            var strfnyear = HttpContext.Session.GetString("fnsyear");
            string filePath = System.IO.Path.Combine(serverpath.getFileUploadPath(), "Form16", strfnyear ?? "");
            var fileInfo = new FileInfo(filePath);

            if (fileInfo.Exists)
            {
                string contentType = "application/pdf"; // only handling PDF here
                return PhysicalFile(fileInfo.FullName, contentType, fileInfo.Name);
            }
            else
            {
                return Content("Form 16 not available for selected Financial Year");
            }
        }




        //protected void travelexcelbtn_Click(object sender, ImageClickEventArgs e)
        [HttpPost]
        public ActionResult ExportTravelExcel([FromBody] EmployeeAccountStatementViewModel objEss)
        {
            short retVal = 0;
            try
            {
                string category = objEss?.Category;
                string monthFrom = objEss?.MonthFrom;
                string monthTo = objEss?.MonthTo;
                string fiscalYear = objEss?.FiscalYear ?? DateTime.Now.Year.ToString();

                DataTable dt = GET_EMPACCSTATEMENT_DATATABLE(
                    _sessionService.Get<string>("userID"),
                    category, monthFrom, monthTo, fiscalYear);

                int[] columnList = { 0, 1, 2, 3, 4, 5, 6, 7 };
                string[] headerList = { "Particulars", "Posting Date", "Tour Period", "Advance Availed", "Advance Settled", "Cumulative Balance", "Remarks", "Status" };

                if (dt.Rows.Count > 0)
                {
                    string fileName = "Travel_" + DateTime.Now.ToString("ddMMyyyy") + ".xls";

                    var fileBytes = _excelExport.ExportDetails(dt, columnList, headerList, ExportFormat.Excel, fileName);

                    TempData["EXCELFILE"] = JsonSerializer.Serialize(fileBytes);
                    TempData["FILENAME"] = fileName;
                    retVal = 1;
                    return Json(retVal);
                }
            }
            catch (Exception ex)
            {
                retVal = (short)-1;
            }
            return Json(retVal);
        }
        //protected void Ticketexcelbtn_Click(object sender, ImageClickEventArgs e)
        [HttpPost]
        public ActionResult ExportTicketExecel([FromBody] EmployeeAccountStatementViewModel objEss)
        {
            short retVal = 0;
            try
            {
                string category = objEss?.Category;
                string monthFrom = objEss?.MonthFrom;
                string monthTo = objEss?.MonthTo;
                string fiscalYear = objEss?.FiscalYear ?? DateTime.Now.Year.ToString();

                DataTable dt = GET_EMPACCSTATEMENT_DATATABLE(
                    _sessionService.Get<string>("userID"),
                    category, monthFrom, monthTo, fiscalYear);

                int[] columnList = { 0, 1, 2, 3, 4, 5, 6, 7 };
                string[] headerList = { "Particulars", "Posting Date", "Ticket No.", "Ticket Fare", "Ticket Settled", "Cumulative Balance", "Remarks", "Status" };

                if (dt.Rows.Count > 0)
                {
                    string fileName = "Ticket_" + DateTime.Now.ToString("ddMMyyyy") + ".xls";

                    var fileBytes = _excelExport.ExportDetails(dt, columnList, headerList, ExportFormat.Excel, fileName);

                    TempData["EXCELFILE"] = JsonSerializer.Serialize(fileBytes);
                    TempData["FILENAME"] = fileName;
                    retVal = 1;
                    return Json(retVal);
                }
            }
            catch (Exception ex)
            {
                retVal = (short)-1;
            }
            return Json(retVal);
        }
        //protected void Productexcelbtn_Click(object sender, ImageClickEventArgs e)

        [HttpPost]
        public ActionResult ExportProductExcel([FromBody] EmployeeAccountStatementViewModel objEss)
        {
            short retVal = 0;
            try
            {
                string category = objEss?.Category;
                string monthFrom = objEss?.MonthFrom;
                string monthTo = objEss?.MonthTo;
                string fiscalYear = objEss?.FiscalYear ?? DateTime.Now.Year.ToString();

                DataTable dt = GET_EMPACCSTATEMENT_DATATABLE(
                    _sessionService.Get<string>("userID"),
                    category, monthFrom, monthTo, fiscalYear);

                int[] columnList = { 0, 1, 3, 4, 5, 6, 7 };
                string[] headerList = { "Particulars", "Posting Date", "Loan Availed", "Loan Settled", "Cumulative Balance", "Remarks", "Status" };

                if (dt.Rows.Count > 0)
                {
                    string fileName = "Product_" + DateTime.Now.ToString("ddMMyyyy") + ".xls";

                    var fileBytes = _excelExport.ExportDetails(dt, columnList, headerList, ExportFormat.Excel, fileName);

                    TempData["EXCELFILE"] = JsonSerializer.Serialize(fileBytes);
                    TempData["FILENAME"] = fileName;
                    retVal = 1;
                    return Json(retVal);
                }
            }
            catch (Exception ex)
            {
                retVal = (short)-1;
            }
            return Json(retVal);
        }
        //protected void Medicalexcelbtn_Click(object sender, ImageClickEventArgs e)
        [HttpPost]
        public ActionResult ExportMedicalExcel([FromBody] EmployeeAccountStatementViewModel objEss)
        {
            short retVal = 0;
            try
            {
                string category = objEss?.Category;
                string monthFrom = objEss?.MonthFrom;
                string monthTo = objEss?.MonthTo;
                string fiscalYear = objEss?.FiscalYear ?? DateTime.Now.Year.ToString();

                DataTable dt = GET_EMPACCSTATEMENT_DATATABLE(
                    _sessionService.Get<string>("userID"),
                    category, monthFrom, monthTo, fiscalYear);

                int[] columnList = { 0, 1, 3, 4, 5, 6, 7 };
                string[] headerList = { "Particulars", "Posting Date", "Loan Availed", "Loan Settled", "Cumulative Balance", "Remarks", "Status" };

                if (dt.Rows.Count > 0)
                {
                    string fileName = "Medical_" + DateTime.Now.ToString("ddMMyyyy") + ".xls";

                    var fileBytes = _excelExport.ExportDetails(dt, columnList, headerList, ExportFormat.Excel, fileName);

                    TempData["EXCELFILE"] = JsonSerializer.Serialize(fileBytes);
                    TempData["FILENAME"] = fileName;
                    retVal = 1;
                    return Json(retVal);
                }
            }
            catch (Exception ex)
            {
                retVal = (short)-1;
            }
            return Json(retVal);
        }
        //protected void Imprestexcelbtn_Click(object sender, ImageClickEventArgs e)

        [HttpPost]
        public ActionResult ExportImprestExcel([FromBody] EmployeeAccountStatementViewModel objEss)
        {
            short retVal = 0;
            try
            {
                string category = objEss?.Category;
                string monthFrom = objEss?.MonthFrom;
                string monthTo = objEss?.MonthTo;
                string fiscalYear = objEss?.FiscalYear ?? DateTime.Now.Year.ToString();

                DataTable dt = GET_EMPACCSTATEMENT_DATATABLE(
                    _sessionService.Get<string>("userID"),
                    category, monthFrom, monthTo, fiscalYear);

                int[] columnList = { 0, 1, 3, 4, 5, 6, 7 };
                string[] headerList = { "Particulars", "Posting Date", "Imprest Availed", "Imprest Settled", "Cumulative Balance", "Remarks", "Status" };

                if (dt.Rows.Count > 0)
                {
                    string fileName = "Imprest_" + DateTime.Now.ToString("ddMMyyyy") + ".xls";

                    var fileBytes = _excelExport.ExportDetails(dt, columnList, headerList, ExportFormat.Excel, fileName);

                    TempData["EXCELFILE"] = JsonSerializer.Serialize(fileBytes);
                    TempData["FILENAME"] = fileName;
                    retVal = 1;
                    return Json(retVal);
                }
            }
            catch (Exception ex)
            {
                retVal = (short)-1;
            }
            return Json(retVal);
        }
        //protected void Salaryexcelbtn_Click(object sender, ImageClickEventArgs e)

        [HttpPost]
        public ActionResult ExportSalaryExcel([FromBody] EmployeeAccountStatementViewModel objEss)
        {
            short retVal = 0;
            try
            {
                string category = objEss?.Category;
                string monthFrom = objEss?.MonthFrom;
                string monthTo = objEss?.MonthTo;
                string fiscalYear = objEss?.FiscalYear ?? DateTime.Now.Year.ToString();

                DataTable dt = GET_EMPACCSTATEMENT_DATATABLE(
                    _sessionService.Get<string>("userID"),
                    category, monthFrom, monthTo, fiscalYear);

                int[] columnList = { 0, 1, 3, 4, 5, 6, 7 };
                string[] headerList = { "Particulars", "Posting Date", "Advance Availed", "Advance Settled", "Cumulative Balance", "Remarks", "Status" };

                if (dt.Rows.Count > 0)
                {
                    string fileName = "SalaryAdvance_" + DateTime.Now.ToString("ddMMyyyy") + ".xls";

                    var fileBytes = _excelExport.ExportDetails(dt, columnList, headerList, ExportFormat.Excel, fileName);

                    TempData["EXCELFILE"] = JsonSerializer.Serialize(fileBytes);
                    TempData["FILENAME"] = fileName;
                    retVal = 1;
                    return Json(retVal);
                }
            }
            catch (Exception ex)
            {
                retVal = (short)-1;
            }
            return Json(retVal);
        }
        public ActionResult DownloadExcel()
        {
            try
            {
                if (TempData["EXCELFILE"] == null)
                    return View();

                var json = TempData["EXCELFILE"].ToString();

                var fileObj = JsonSerializer.Deserialize<TempFileModel>(json);

                byte[] fileBytes = Convert.FromBase64String(fileObj.FileContents);

                return File(fileBytes, fileObj.ContentType, fileObj.FileDownloadName);
            }
            catch
            {
                return RedirectToAction("ErrorPage");
            }
        }
        [HttpPost]
        public ActionResult ExportToExcel([FromBody] EmployeeAccountStatementViewModel objEss)
        {

            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            short retVal = 0;
            string category = objEss?.Category;
            string monthFrom = objEss?.MonthFrom;
            string monthTo = objEss?.MonthTo;
            string fiscalYear = objEss?.FiscalYear ?? DateTime.Now.Year.ToString();

            try
            {
                DataTable dt = new DataTable();

                dt = GET_EMPACCSTATEMENT_DATATABLE(_sessionService.Get<string>("userID"), category, monthFrom, monthTo, fiscalYear);
                if (objEss != null && objEss.Category == "N")
                {
                    ViewBag.div_main_travel = true;
                    var exportList = dt.AsEnumerable()
                         .Select(row => new EssViewModel.TravelRecord
                         {
                             STATUS = row["STATUS"]?.ToString(),
                             PSTNG_DATE = row["PSTNG_DATE"]?.ToString(),
                             ALLOC_NMBR = row["ALLOC_NMBR"]?.ToString(),
                             DEBIT = row["DEBIT"]?.ToString(),
                             CREDIT = row["CREDIT"]?.ToString(),
                             CUMULATIVE_BALANCE = row["CUMULATIVE_BALANCE"]?.ToString(),
                             PARTICULAR = row["PARTICULAR"]?.ToString(),
                             ITEM_TEXT = row["ITEM_TEXT"]?.ToString()
                         }).ToList();

                    // Assuming excelHtml takes a List<TravelRecord>
                    string str = this.excelHtml(exportList);

                    TempData["EXCELFILE"] = str;
                    retVal = 1;
                }
            }
            catch (Exception ex)
            {
                retVal = (short)-1;
            }
            return Json(retVal);
        }
        public string excelHtml(List<EssViewModel.TravelRecord> _exportObj)
        {
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            string str = "";
            if (_exportObj.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 11pt;font-family:Arial'>");
                stringBuilder.Append("<tr style='background-color: lightgray;'>");
                stringBuilder.Append("<th style='width:4%;text-align:center;border: 1px solid;'>Particulars</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Posting Date</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Tour Period</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Advance Availed</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Advance Settled</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Cumulative Balance</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Remarks</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Status</th>");
                stringBuilder.Append("</tr>");
                int srNo = 1;
                foreach (EssViewModel.TravelRecord ESS in _exportObj)
                {
                    stringBuilder.Append("<tr>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + ESS.PARTICULAR + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + ESS.PSTNG_DATE + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + ESS.ALLOC_NMBR + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + ESS.DEBIT + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + ESS.CREDIT + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + ESS.CUMULATIVE_BALANCE + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + ESS.ITEM_TEXT + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + ESS.STATUS + "</td>");
                    stringBuilder.Append("</tr>");
                }
                stringBuilder.Append("</table>");
                str = stringBuilder.ToString();
            }
            return str;
        }

        public IActionResult Form16details()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var kiList = _objpms.GetKiList()
                .AsEnumerable()
                .Where(r => r.Field<decimal>("SYKIID") > 18)
                .OrderBy(r => r.Field<decimal>("SYKIID"))
                .Select(r => new SelectListItem
                {
                    Value = r.Field<decimal>("SYKIID").ToString(),
                    Text = r.Field<string>("FINANCIALYEAR")
                })
                .ToList();

            string strKiId = _objpms.GetKIId();
            string selectedKiId = (Convert.ToInt16(strKiId)).ToString();

            // Pass selected value into SelectList
            ViewBag.FiscalYears = new SelectList(kiList, "Value", "Text", selectedKiId);

            return View();
        }


        public IActionResult Form16details_searchbtn([FromBody] EssViewModel objEss)
        {

            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
            string strpanno = _Employee_Details.PancardNo.ToString();
            string strfnyear = objEss.FINANCIALYEAR; //ddlki.SelectedItem.ToString();
            //string filePath = serverpath.getFileUploadPath() + "Form16\\" + strpanno + "_" + strfnyear + ".pdf";
            if (Convert.ToInt32(strfnyear.Substring(0, 4)) > 2017)
            {
                strfnyear = (Convert.ToInt32(strfnyear.Substring(0, 4)) + 1).ToString() + "-" + (Convert.ToInt32(strfnyear.Substring(7, 2)) + 1).ToString();
            }

            string folderPath = Path.Combine(serverpath.getFileUploadPath(), "Form16");

            // Check if folder exists, if not create it only handle error
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string[] files = Directory.GetFiles(
                folderPath,
                strpanno + "*.pdf",
                SearchOption.AllDirectories
            );

            //string[] files = Directory.GetFiles(serverpath.getFileUploadPath() + "Form16\\", strpanno + "*.pdf", System.IO.SearchOption.AllDirectories);

            DataTable dt_form16 = new DataTable();
            dt_form16.Columns.Add("filepath");
            dt_form16.Columns.Add("Filename");
            foreach (string dfile in files)
            {
                string[] datafile = dfile.Split('\\');
                int rownum = datafile.Length;
                string[] filename = datafile[rownum - 1].Split('_');

                string strfilename = datafile[rownum - 1].Replace(".pdf", "");
                DataRow drrow;
                drrow = dt_form16.NewRow();

                if (strfilename.Length == 20)
                {
                    if (strfilename.Substring(strfilename.Length - 9) == strfnyear)
                    {
                        drrow[0] = datafile[rownum - 1].ToString();
                        drrow[1] = strfilename.Substring(11, strfilename.Length - 11);//filename[1].ToString().Replace(".pdf", "");
                        dt_form16.Rows.Add(drrow);
                    }
                }
                if (strfilename.Length != 20)
                {
                    if (strfilename.Substring(strfilename.Length - 7) == strfnyear)
                    {
                        drrow[0] = datafile[rownum - 1].ToString();
                        drrow[1] = strfilename.Substring(11, strfilename.Length - 11);//filename[1].ToString().Replace(".pdf", "");
                        dt_form16.Rows.Add(drrow);
                    }

                }

            }
            objEss.dt_form16 = dt_form16.AsEnumerable()
                       .Select(row => new EssViewModel.Form16details
                       {
                           FILEPATH = row["filepath"]?.ToString(),
                           FILENAME = row["Filename"]?.ToString()
                       }).ToList();
            return View("_GetForm16details", objEss);
        }


        [HttpGet]
        public IActionResult DownloadForm16(string filepath)
        {
            try
            {
                string basePath = serverpath.getFileUploadPath();
                string filePath = System.IO.Path.Combine(basePath, "Form16", filepath);

                if (System.IO.File.Exists(filePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    return File(fileBytes,
                                System.Net.Mime.MediaTypeNames.Application.Octet,
                                filepath);
                }
                else
                {
                    TempData["ErrorMessage"] = "File not found.";
                    return RedirectToAction("ErrorPage");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage");
            }
        }
        public IActionResult TaxworkingDetails()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var kiList = _objpms.GetKiList()
                .AsEnumerable()
                .Where(r => r.Field<decimal>("SYKIID") > 18)
                .OrderBy(r => r.Field<decimal>("SYKIID"))
                .Select(r => new SelectListItem
                {
                    Value = r.Field<decimal>("SYKIID").ToString(),
                    Text = r.Field<string>("FINANCIALYEAR")
                })
                .ToList();

            string strKiId = _objpms.GetKIId();
            string selectedKiId = (Convert.ToInt16(strKiId)).ToString();

            // Pass selected value into SelectList
            ViewBag.FiscalYears = new SelectList(kiList, "Value", "Text", selectedKiId);

            return View();
        }
        //searchbtn_Click
        [HttpPost]
        public async Task<IActionResult> TaxworkingDetailsSearch([FromBody] EssViewModel objEss)
        {
            // Save FYKI to session
            string strFYKI = objEss.FINANCIALYEAR;
            _sessionService.Set("FYKI", strFYKI);

            int day = DateTime.Now.Day;

            if (day >= 22 && day <= 28)
            {
                return RedirectToPage("Ess", "TaxWorkingNA");
            }

            DateTime currentDate = DateTime.Now.Date;
            DateTime fromDate = new DateTime(2018, 4, 26);
            DateTime toDate = new DateTime(2018, 6, 28);

            if (currentDate >= fromDate && currentDate <= toDate)
            {
                return RedirectToPage("Ess", "EssDetail");
            }

            // Get user ID
            var uid = _sessionService.Get<string>("userID");

            if (Request.Query.ContainsKey("taxid"))
            {
                string encrypted = Request.Query["taxid"];
                uid = Encryption.Decrypt(WebUtility.UrlDecode(encrypted));
            }
            var fyki = _sessionService.Get<string>("FYKI");

            //if (DateTime.Now.Month < 10) mon = "0" + DateTime.Now.Month.ToString(); else mon = DateTime.Now.Month.ToString();
            //if (DateTime.Now.Month <= 3)
            //{
            //    year = taxyear1;//year = taxyear2;
            //}
            //else
            //{
            //    year = taxyear1;
            //}

            string taxyear1 = fyki.Substring(0, 4);
            string taxyear2 = fyki.Substring(5, 4);

            string mon = DateTime.Now.Month.ToString("00");
            string year = DateTime.Now.Month <= 3 ? taxyear1 : taxyear2;

            string key = $"{year}{mon}01";

            // Fetch SAP data
            var dt = await _ePortalESS.Gettaxworkingtax(uid, key);

            //******************** Getting TAX Working detail Form SAP

            if (dt == null || dt.Rows.Count == 0)
                return NotFound("");

            //end of tax working detail
            //Tax_Dataset ds = new Tax_Dataset();

            ////employee detail insertion On report Head

            //DataRow dr_report_head = ds.Tables["report_Head"].NewRow();

            //*******After Month Detail Field Calculation On report Head
            string strmonth = DateTime.Now.Day < 29
                ? DateTime.Now.AddMonths(-2).ToString("MMM")
                : DateTime.Now.AddMonths(-1).ToString("MMM");

            var fullName = await GetFullNameAsync(uid);

            // Header detail
            var taxModel = new TaxworkingNewViewModel
            {
                TaxDate = DateTime.Today.ToString("dd-MMM-yy"),
                AfterSalaryMonth = strmonth,
                AssetYear = $"{Convert.ToInt32(taxyear1) + 1}-{Convert.ToInt32(taxyear2) + 1}",
                FinancialYear = $"01.04.{taxyear1} To 31.03.{taxyear2}",
                EmpName = fullName,
                EmpCode = dt.Rows[0][0].ToString(),
                PanNumber = dt.Rows[0][2].ToString(),
                JoiningDate = DateTime.ParseExact(dt.Rows[0][3].ToString(), "yyyyMMdd", null).ToString("dd/MM/yyyy"),
                Operation = (dt.Rows[0][4]?.ToString() ?? "").TrimEnd('&'),
                Designation = dt.Rows[0][5].ToString(),
                TaxRegime = dt.Rows[0][1429].ToString()
            };
            //end of employee detail insertion

            //***********Financial Year Calculation
            string finance_year = "";
            // FOR TESTING dr_report_head[1] = fyki.ToString();//(DateTime.Now.Year-1).ToString() + " - " + (DateTime.Now.Year ).ToString();
            finance_year = "01.04." + (taxyear1).ToString() + " To  31.03." + (taxyear2).ToString();

            //******* End of Financial Year Calculation


            List<RegularIncomeViewModel> RegularIncome = new();

            decimal strtotrim = 0.00m;
            int j = 86;
            decimal total_salary = 0.00m;
            decimal total_Esalary_Apr = 0.00m;
            decimal total_Esalary_May = 0.00m;
            decimal total_Esalary_Jun = 0.00m;
            decimal total_Esalary_Jul = 0.00m;
            decimal total_Esalary_Aug = 0.00m;
            decimal total_Esalary_Sep = 0.00m;
            decimal total_Esalary_Oct = 0.00m;
            decimal total_Esalary_Nov = 0.00m;
            decimal total_Esalary_Dec = 0.00m;
            decimal total_Esalary_Jan = 0.00m;
            decimal total_Esalary_Feb = 0.00m;
            decimal total_Esalary_Mar = 0.00m;

            for (int i = 0, k = 6; i <= 23; i++, k++)
            {
                if (Convert.ToDouble(dt.Rows[0][1038 + i]) != 0)
                {
                    total_salary = total_salary + Convert.ToDecimal(dt.Rows[0][1038 + i]);

                    var rincome = new RegularIncomeViewModel
                    {
                        dr_earn_head_0 = dt.Columns[k].ColumnName.Replace("_", " ").Replace("4", " ").ToUpper(),
                        dr_earn_head_1 = Convert.ToDecimal(dt.Rows[0][k + j * 0]),
                        dr_earn_head_2 = Convert.ToDecimal(dt.Rows[0][k + j * 1]),
                        dr_earn_head_3 = Convert.ToDecimal(dt.Rows[0][k + j * 2]),
                        dr_earn_head_4 = Convert.ToDecimal(dt.Rows[0][k + j * 3]),
                        dr_earn_head_5 = Convert.ToDecimal(dt.Rows[0][k + j * 4]),
                        dr_earn_head_6 = Convert.ToDecimal(dt.Rows[0][k + j * 5]),
                        dr_earn_head_7 = Convert.ToDecimal(dt.Rows[0][k + j * 6]),
                        dr_earn_head_8 = Convert.ToDecimal(dt.Rows[0][k + j * 7]),
                        dr_earn_head_9 = Convert.ToDecimal(dt.Rows[0][k + j * 8]),
                        dr_earn_head_10 = Convert.ToDecimal(dt.Rows[0][k + j * 9]),
                        dr_earn_head_11 = Convert.ToDecimal(dt.Rows[0][k + j * 10]),
                        dr_earn_head_12 = Convert.ToDecimal(dt.Rows[0][k + j * 11]),

                        dr_earn_head_13 = Convert.ToDecimal(dt.Rows[0][1038 + i])
                    };

                    total_Esalary_Apr += rincome.dr_earn_head_1;
                    total_Esalary_May += rincome.dr_earn_head_2;
                    total_Esalary_Jun += rincome.dr_earn_head_3;
                    total_Esalary_Jul += rincome.dr_earn_head_4;
                    total_Esalary_Aug += rincome.dr_earn_head_5;
                    total_Esalary_Sep += rincome.dr_earn_head_6;
                    total_Esalary_Oct += rincome.dr_earn_head_7;
                    total_Esalary_Nov += rincome.dr_earn_head_8;
                    total_Esalary_Dec += rincome.dr_earn_head_9;
                    total_Esalary_Jan += rincome.dr_earn_head_10;
                    total_Esalary_Feb += rincome.dr_earn_head_11;
                    total_Esalary_Mar += rincome.dr_earn_head_12;

                    RegularIncome.Add(rincome);
                }
            }

                //=====
                //Below Change on 17122022 By[Aumento]=======================================================


                if (Convert.ToDouble(dt.Rows[0][1498]) != 0)
                {
                    total_salary += Convert.ToDecimal(dt.Rows[0][1498]);

                    var rincomeRow = new RegularIncomeViewModel
                    {
                        dr_earn_head_0 = "ROLE ALL",
                        dr_earn_head_1 = Convert.ToDecimal(dt.Rows[0][1486]),
                        dr_earn_head_2 = Convert.ToDecimal(dt.Rows[0][1487]),
                        dr_earn_head_3 = Convert.ToDecimal(dt.Rows[0][1488]),
                        dr_earn_head_4 = Convert.ToDecimal(dt.Rows[0][1489]),
                        dr_earn_head_5 = Convert.ToDecimal(dt.Rows[0][1490]),
                        dr_earn_head_6 = Convert.ToDecimal(dt.Rows[0][1491]),
                        dr_earn_head_7 = Convert.ToDecimal(dt.Rows[0][1492]),
                        dr_earn_head_8 = Convert.ToDecimal(dt.Rows[0][1493]),
                        dr_earn_head_9 = Convert.ToDecimal(dt.Rows[0][1494]),
                        dr_earn_head_10 = Convert.ToDecimal(dt.Rows[0][1495]),
                        dr_earn_head_11 = Convert.ToDecimal(dt.Rows[0][1496]),
                        dr_earn_head_12 = Convert.ToDecimal(dt.Rows[0][1497]),
                        dr_earn_head_13 = Convert.ToDecimal(dt.Rows[0][1498])
                    };

                    // accumulate totals
                    total_Esalary_Apr += rincomeRow.dr_earn_head_1;
                    total_Esalary_May += rincomeRow.dr_earn_head_2;
                    total_Esalary_Jun += rincomeRow.dr_earn_head_3;
                    total_Esalary_Jul += rincomeRow.dr_earn_head_4;
                    total_Esalary_Aug += rincomeRow.dr_earn_head_5;
                    total_Esalary_Sep += rincomeRow.dr_earn_head_6;
                    total_Esalary_Oct += rincomeRow.dr_earn_head_7;
                    total_Esalary_Nov += rincomeRow.dr_earn_head_8;
                    total_Esalary_Dec += rincomeRow.dr_earn_head_9;
                    total_Esalary_Jan += rincomeRow.dr_earn_head_10;
                    total_Esalary_Feb += rincomeRow.dr_earn_head_11;
                    total_Esalary_Mar += rincomeRow.dr_earn_head_12;

                    RegularIncome.Add(rincomeRow);
                }
           
            taxModel.RegularIncome = RegularIncome;

            taxModel.RegularIncomeTotal.rincome_head = "T REG INCOME";
            taxModel.RegularIncomeTotal.rincome_head_April = total_Esalary_Apr;
            taxModel.RegularIncomeTotal.rincome_head_May = total_Esalary_May;
            taxModel.RegularIncomeTotal.rincome_head_June = total_Esalary_Jun;
            taxModel.RegularIncomeTotal.rincome_head_July = total_Esalary_Jul;
            taxModel.RegularIncomeTotal.rincome_head_August = total_Esalary_Aug;
            taxModel.RegularIncomeTotal.rincome_head_September = total_Esalary_Sep;
            taxModel.RegularIncomeTotal.rincome_head_October = total_Esalary_Oct;
            taxModel.RegularIncomeTotal.rincome_head_November = total_Esalary_Nov;
            taxModel.RegularIncomeTotal.rincome_head_December = total_Esalary_Dec;
            taxModel.RegularIncomeTotal.rincome_head_January = total_Esalary_Jan;
            taxModel.RegularIncomeTotal.rincome_head_February = total_Esalary_Feb;
            taxModel.RegularIncomeTotal.rincome_head_March = total_Esalary_Mar;
            taxModel.RegularIncomeTotal.rincome_head_Total = total_Esalary_Apr + total_Esalary_May + total_Esalary_Jun +
                  total_Esalary_Jul + total_Esalary_Aug + total_Esalary_Sep +
                  total_Esalary_Oct + total_Esalary_Nov + total_Esalary_Dec +
                  total_Esalary_Jan + total_Esalary_Feb + total_Esalary_Mar;
            //=======================================================================================================
            //=====
            //reprincome.DataSource = ds.Tables["Earning_Head"];
            //reprincome.DataBind();

            //Earing head total
            var rincome_head = new RegularIncomeViewModel
            {
                rincome_head = "T REG INCOME",
                rincome_head_April = total_Esalary_Apr,
                rincome_head_May = total_Esalary_May,
                rincome_head_June = total_Esalary_Jun,
                rincome_head_July = total_Esalary_Jul,
                rincome_head_August = total_Esalary_Aug,
                rincome_head_September = total_Esalary_Sep,
                rincome_head_October = total_Esalary_Oct,
                rincome_head_November = total_Esalary_Nov,
                rincome_head_December = total_Esalary_Dec,
                rincome_head_January = total_Esalary_Jan,
                rincome_head_February = total_Esalary_Feb,
                rincome_head_March = total_Esalary_Mar,
                rincome_head_Total = total_Esalary_Apr + total_Esalary_May + total_Esalary_Jun +
                   total_Esalary_Jul + total_Esalary_Aug + total_Esalary_Sep +
                   total_Esalary_Oct + total_Esalary_Nov + total_Esalary_Dec +
                   total_Esalary_Jan + total_Esalary_Feb + total_Esalary_Mar
            };

            List<IrregularIncomeViewModel> IrregularIncome = new();
            //--------------------------------
            decimal total_IRsalary_Apr = 0.00m;
            decimal total_IRsalary_May = 0.00m;
            decimal total_IRsalary_Jun = 0.00m;
            decimal total_IRsalary_Jul = 0.00m;
            decimal total_IRsalary_Aug = 0.00m;
            decimal total_IRsalary_Sep = 0.00m;
            decimal total_IRsalary_Oct = 0.00m;
            decimal total_IRsalary_Nov = 0.00m;
            decimal total_IRsalary_Dec = 0.00m;
            decimal total_IRsalary_Jan = 0.00m;
            decimal total_IRsalary_Feb = 0.00m;
            decimal total_IRsalary_Mar = 0.00m;
            //inserting earning head in report
            for (int i = 0, k = 29; i < 30; i++, k++)
            {
                if (Convert.ToDouble(dt.Rows[0][1061 + i]) != 0)
                {
                    total_salary = total_salary + Convert.ToDecimal(dt.Rows[0][1061 + i]);

                    var irregularIncome = new IrregularIncomeViewModel
                    {
                        Pay_Head = dt.Columns[k].ColumnName.Replace("_", " ").Replace("4", " ").ToUpper(),
                        April = Convert.ToDecimal(dt.Rows[0][k + j * 0]),
                        May = Convert.ToDecimal(dt.Rows[0][k + j * 1]),
                        June = Convert.ToDecimal(dt.Rows[0][k + j * 2]),
                        July = Convert.ToDecimal(dt.Rows[0][k + j * 3]),
                        August = Convert.ToDecimal(dt.Rows[0][k + j * 4]),
                        September = Convert.ToDecimal(dt.Rows[0][k + j * 5]),
                        October = Convert.ToDecimal(dt.Rows[0][k + j * 6]),
                        November = Convert.ToDecimal(dt.Rows[0][k + j * 7]),
                        December = Convert.ToDecimal(dt.Rows[0][k + j * 8]),
                        January = Convert.ToDecimal(dt.Rows[0][k + j * 9]),
                        February = Convert.ToDecimal(dt.Rows[0][k + j * 10]),
                        March = Convert.ToDecimal(dt.Rows[0][k + j * 11]),
                        Total = Convert.ToDecimal(dt.Rows[0][1061 + i])
                    };

                    total_IRsalary_Apr += irregularIncome.April;
                    total_IRsalary_May += irregularIncome.May;
                    total_IRsalary_Jun += irregularIncome.June;
                    total_IRsalary_Jul += irregularIncome.July;
                    total_IRsalary_Aug += irregularIncome.August;
                    total_IRsalary_Sep += irregularIncome.September;
                    total_IRsalary_Oct += irregularIncome.October;
                    total_IRsalary_Nov += irregularIncome.November;
                    total_IRsalary_Dec += irregularIncome.December;
                    total_IRsalary_Jan += irregularIncome.January;
                    total_IRsalary_Feb += irregularIncome.February;
                    total_IRsalary_Mar += irregularIncome.March;

                    IrregularIncome.Add(irregularIncome);
                }
            }

            // LTA_ARREAR
            if (Convert.ToDouble(dt.Rows[0][1446]) != 0)
            {
                total_salary += Convert.ToDecimal(dt.Rows[0][1446]);

                var dr_earn_head = new IrregularIncomeViewModel
                {
                    Pay_Head = "LTA_ARREAR",
                    April = Convert.ToDecimal(dt.Rows[0][1434]),
                    May = Convert.ToDecimal(dt.Rows[0][1435]),
                    June = Convert.ToDecimal(dt.Rows[0][1436]),
                    July = Convert.ToDecimal(dt.Rows[0][1437]),
                    August = Convert.ToDecimal(dt.Rows[0][1438]),
                    September = Convert.ToDecimal(dt.Rows[0][1439]),
                    October = Convert.ToDecimal(dt.Rows[0][1440]),
                    November = Convert.ToDecimal(dt.Rows[0][1441]),
                    December = Convert.ToDecimal(dt.Rows[0][1442]),
                    January = Convert.ToDecimal(dt.Rows[0][1443]),
                    February = Convert.ToDecimal(dt.Rows[0][1444]),
                    March = Convert.ToDecimal(dt.Rows[0][1445]),
                    Total = Convert.ToDecimal(dt.Rows[0][1446])
                };

                total_IRsalary_Apr += dr_earn_head.April;
                total_IRsalary_May += dr_earn_head.May;
                total_IRsalary_Jun += dr_earn_head.June;
                total_IRsalary_Jul += dr_earn_head.July;
                total_IRsalary_Aug += dr_earn_head.August;
                total_IRsalary_Sep += dr_earn_head.September;
                total_IRsalary_Oct += dr_earn_head.October;
                total_IRsalary_Nov += dr_earn_head.November;
                total_IRsalary_Dec += dr_earn_head.December;
                total_IRsalary_Jan += dr_earn_head.January;
                total_IRsalary_Feb += dr_earn_head.February;
                total_IRsalary_Mar += dr_earn_head.March;

                IrregularIncome.Add(dr_earn_head);
                // ds.Tables["IEarning_Head"].Rows.Add(dr_earn_head);
            }

            if (Convert.ToDouble(dt.Rows[0][1459]) != 0)
            {
                total_salary += Convert.ToDecimal(dt.Rows[0][1459]);

                var dr_earn_head = new IrregularIncomeViewModel
                {
                    Pay_Head = "LTA_RECOVERY",
                    April = Convert.ToDecimal(dt.Rows[0][1447]),
                    May = Convert.ToDecimal(dt.Rows[0][1448]),
                    June = Convert.ToDecimal(dt.Rows[0][1449]),
                    July = Convert.ToDecimal(dt.Rows[0][1450]),
                    August = Convert.ToDecimal(dt.Rows[0][1451]),
                    September = Convert.ToDecimal(dt.Rows[0][1452]),
                    October = Convert.ToDecimal(dt.Rows[0][1453]),
                    November = Convert.ToDecimal(dt.Rows[0][1454]),
                    December = Convert.ToDecimal(dt.Rows[0][1455]),
                    January = Convert.ToDecimal(dt.Rows[0][1456]),
                    February = Convert.ToDecimal(dt.Rows[0][1457]),
                    March = Convert.ToDecimal(dt.Rows[0][1458]),
                    Total = Convert.ToDecimal(dt.Rows[0][1459])
                };

                total_IRsalary_Apr += dr_earn_head.April;
                total_IRsalary_May += dr_earn_head.May;
                total_IRsalary_Jun += dr_earn_head.June;
                total_IRsalary_Jul += dr_earn_head.July;
                total_IRsalary_Aug += dr_earn_head.August;
                total_IRsalary_Sep += dr_earn_head.September;
                total_IRsalary_Oct += dr_earn_head.October;
                total_IRsalary_Nov += dr_earn_head.November;
                total_IRsalary_Dec += dr_earn_head.December;
                total_IRsalary_Jan += dr_earn_head.January;
                total_IRsalary_Feb += dr_earn_head.February;
                total_IRsalary_Mar += dr_earn_head.March;

                IrregularIncome.Add(dr_earn_head);
            }

            //Below Change on 07112022 By[Aumento]=======================================================
            if (Convert.ToDouble(dt.Rows[0][1485]) != 0)
            {
                total_salary += Convert.ToDecimal(dt.Rows[0][1485]);

                var dr_earn_head = new IrregularIncomeViewModel
                {
                    Pay_Head = "FUEL REIMB",
                    April = Convert.ToDecimal(dt.Rows[0][1473]),
                    May = Convert.ToDecimal(dt.Rows[0][1474]),
                    June = Convert.ToDecimal(dt.Rows[0][1475]),
                    July = Convert.ToDecimal(dt.Rows[0][1476]),
                    August = Convert.ToDecimal(dt.Rows[0][1477]),
                    September = Convert.ToDecimal(dt.Rows[0][1478]),
                    October = Convert.ToDecimal(dt.Rows[0][1479]),
                    November = Convert.ToDecimal(dt.Rows[0][1480]),
                    December = Convert.ToDecimal(dt.Rows[0][1481]),
                    January = Convert.ToDecimal(dt.Rows[0][1482]),
                    February = Convert.ToDecimal(dt.Rows[0][1483]),
                    March = Convert.ToDecimal(dt.Rows[0][1484]),

                    Total = Convert.ToDecimal(dt.Rows[0][1485])
                };

                total_IRsalary_Apr += dr_earn_head.April;
                total_IRsalary_May += dr_earn_head.May;
                total_IRsalary_Jun += dr_earn_head.June;
                total_IRsalary_Jul += dr_earn_head.July;
                total_IRsalary_Aug += dr_earn_head.August;
                total_IRsalary_Sep += dr_earn_head.September;
                total_IRsalary_Oct += dr_earn_head.October;
                total_IRsalary_Nov += dr_earn_head.November;
                total_IRsalary_Dec += dr_earn_head.December;
                total_IRsalary_Jan += dr_earn_head.January;
                total_IRsalary_Feb += dr_earn_head.February;
                total_IRsalary_Mar += dr_earn_head.March;


                IrregularIncome.Add(dr_earn_head);
            }
            taxModel.IrregularIncome = IrregularIncome;

            //I reg Earing head total
            taxModel.IrregularIncomeTotal.irincome_head = "T REG INCOME";
            taxModel.IrregularIncomeTotal.irincome_head_April = total_IRsalary_Apr;
            taxModel.IrregularIncomeTotal.irincome_head_May = total_IRsalary_May;
            taxModel.IrregularIncomeTotal.irincome_head_June = total_IRsalary_Jun;
            taxModel.IrregularIncomeTotal.irincome_head_July = total_IRsalary_Jul;
            taxModel.IrregularIncomeTotal.irincome_head_August = total_IRsalary_Aug;
            taxModel.IrregularIncomeTotal.irincome_head_September = total_IRsalary_Sep;
            taxModel.IrregularIncomeTotal.irincome_head_October = total_IRsalary_Oct;
            taxModel.IrregularIncomeTotal.irincome_head_November = total_IRsalary_Nov;
            taxModel.IrregularIncomeTotal.irincome_head_December = total_IRsalary_Dec;
            taxModel.IrregularIncomeTotal.irincome_head_January = total_IRsalary_Jan;
            taxModel.IrregularIncomeTotal.irincome_head_February = total_IRsalary_Feb;
            taxModel.IrregularIncomeTotal.irincome_head_March = total_IRsalary_Mar;
            taxModel.IrregularIncomeTotal.irincome_head_Total = total_IRsalary_Apr + total_IRsalary_May + total_IRsalary_Jun +
                  total_IRsalary_Jul + total_IRsalary_Aug + total_IRsalary_Sep +
                  total_IRsalary_Oct + total_IRsalary_Nov + total_IRsalary_Dec +
                  total_IRsalary_Jan + total_IRsalary_Feb + total_IRsalary_Mar;


            //=======================================================================================================


            //repiincome.DataSource = ds.Tables["IEarning_Head"];
            //repiincome.DataBind();

            //I reg Earing head total
            //var iregmodel = new IrregularIncomeViewModel
            //{
            //    irincome_head = "T REG INCOME",
            //    irincome_head_April = total_IRsalary_Apr.ToString(),
            //    irincome_head_May = total_IRsalary_May.ToString(),
            //    irincome_head_June = total_IRsalary_Jun.ToString(),
            //    irincome_head_July = total_IRsalary_Jul.ToString(),
            //    irincome_head_August = total_IRsalary_Aug.ToString(),
            //    irincome_head_September = total_IRsalary_Sep.ToString(),
            //    irincome_head_October = total_IRsalary_Oct.ToString(),
            //    irincome_head_November = total_IRsalary_Nov.ToString(),
            //    irincome_head_December = total_IRsalary_Dec.ToString(),
            //    irincome_head_January = total_IRsalary_Jan.ToString(),
            //    irincome_head_February = total_IRsalary_Feb.ToString(),
            //    irincome_head_March = total_IRsalary_Mar.ToString(),
            //    irincome_head_Total = (
            //         total_IRsalary_Apr + total_IRsalary_May + total_IRsalary_Jun +
            //         total_IRsalary_Jul + total_IRsalary_Aug + total_IRsalary_Sep +
            //         total_IRsalary_Oct + total_IRsalary_Nov + total_IRsalary_Dec +
            //         total_IRsalary_Jan + total_IRsalary_Feb + total_IRsalary_Mar
            //     ).ToString()
            //};
            //yyyy
            List<CEarningHeadViewModel> CEarningHead = new();
            //var CEarningHead = new List<CEarningHeadViewModel>();
            //add NPS earning head in report

            total_salary = total_salary + Convert.ToDecimal(dt.Rows[0][1091]);

            var dr_NPS_head = new CEarningHeadViewModel
            {
                Pay_Head = dt.Columns[59].ColumnName.Replace("_", " ").Replace("4", " ").ToUpper(),
                April = Convert.ToDecimal(dt.Rows[0][59]),
                May = Convert.ToDecimal(dt.Rows[0][145]),
                June = Convert.ToDecimal(dt.Rows[0][231]),
                July = Convert.ToDecimal(dt.Rows[0][317]),
                August = Convert.ToDecimal(dt.Rows[0][403]),
                September = Convert.ToDecimal(dt.Rows[0][489]),
                October = Convert.ToDecimal(dt.Rows[0][575]),
                November = Convert.ToDecimal(dt.Rows[0][661]),
                December = Convert.ToDecimal(dt.Rows[0][747]),
                January = Convert.ToDecimal(dt.Rows[0][833]),
                February = Convert.ToDecimal(dt.Rows[0][919]),
                March = Convert.ToDecimal(dt.Rows[0][1005]),
                Total = Convert.ToDecimal(dt.Rows[0][1091])
            };
            //end of NPS head
            CEarningHead.Add(dr_NPS_head);
            taxModel.CEarningHead = CEarningHead;
            //var model = new EssViewModel.TaxworkingNewViewModel 
            //{ 
            //    CEarningHead = CEarningHead 
            //};

            //Other earning head in report

            var DIncomeHead = new List<DEarningHeadViewModel>();

            total_salary = total_salary + Convert.ToDecimal(dt.Rows[0][1382]);
            // Example: Arrears of salary
            var arrears = new DEarningHeadViewModel
            {
                Pay_Head = "Arrears of salary",                
                April = Convert.ToDecimal(dt.Rows[0][1370]),
                May = Convert.ToDecimal(dt.Rows[0][1371]),
                June = Convert.ToDecimal(dt.Rows[0][1372]),
                July = Convert.ToDecimal(dt.Rows[0][1373]),
                August = Convert.ToDecimal(dt.Rows[0][1374]),
                September = Convert.ToDecimal(dt.Rows[0][1375]),
                October = Convert.ToDecimal(dt.Rows[0][1376]),
                November = Convert.ToDecimal(dt.Rows[0][1377]),
                December = Convert.ToDecimal(dt.Rows[0][1378]),
                January = Convert.ToDecimal(dt.Rows[0][1379]),
                February = Convert.ToDecimal(dt.Rows[0][1380]),
                March = Convert.ToDecimal(dt.Rows[0][1381]),
                Total = Convert.ToDecimal(dt.Rows[0][1382])
            };
            DIncomeHead.Add(arrears);

            // Example: loop for other earning heads
            for (int i = 0, k = 60; i < 2; i++, k++)
            {
                total_salary = total_salary + Convert.ToDecimal(dt.Rows[0][1092 + i]);
                var dhead = new DEarningHeadViewModel
                {
                    Pay_Head = dt.Columns[k].ColumnName.Replace("_", " ").Replace("4", " ").ToUpper(),
                    //dincome_head = dt.Columns[k].ColumnName.Replace("_", " ").Replace("4", " ").ToUpper(),
                    dincome_head_April = Convert.ToDecimal(dt.Rows[0][k +j* 0]),
                    dincome_head_May = Convert.ToDecimal(dt.Rows[0][k + j * 1]),
                    dincome_head_June = Convert.ToDecimal(dt.Rows[0][k + j * 2]),
                    dincome_head_July = Convert.ToDecimal(dt.Rows[0][k + j * 3]),
                    dincome_head_August = Convert.ToDecimal(dt.Rows[0][k + j * 4]),
                    dincome_head_September = Convert.ToDecimal(dt.Rows[0][k + j * 5]),
                    dincome_head_October = Convert.ToDecimal(dt.Rows[0][k + j * 6]),
                    dincome_head_November = Convert.ToDecimal(dt.Rows[0][k + j * 7]),
                    dincome_head_December = Convert.ToDecimal(dt.Rows[0][k + j * 8]),
                    dincome_head_January = Convert.ToDecimal(dt.Rows[0][k + j * 9]),
                    dincome_head_February = Convert.ToDecimal(dt.Rows[0][k + j * 10]),
                    dincome_head_March = Convert.ToDecimal(dt.Rows[0][k + j * 11]),
                    dincome_head_Total = Convert.ToDecimal(dt.Rows[0][1092 + i])
                };
                DIncomeHead.Add(dhead);
            }
            taxModel.DIncomeHead = DIncomeHead;
            //repdincome.DataSource = ds.Tables["DEarning_Head"];
            //repdincome.DataBind();

            ////end of earning head insertion

            //total of D head
            //I reg Earing head total
            var totalDIncome = new DEarningHeadViewModel
            {               
                dincome_head = "Total",
                dincome_head_April = Convert.ToDecimal("0.00"),
                dincome_head_May = Convert.ToDecimal("0.00"),
                dincome_head_June = Convert.ToDecimal("0.00"),
                dincome_head_July = Convert.ToDecimal("0.00"),
                dincome_head_August = Convert.ToDecimal("0.00"),
                dincome_head_September = Convert.ToDecimal("0.00"),
                dincome_head_October = Convert.ToDecimal("0.00"),
                dincome_head_November = Convert.ToDecimal("0.00"),
                dincome_head_December = Convert.ToDecimal("0.00"),
                dincome_head_January = Convert.ToDecimal("0.00"),
                dincome_head_February = Convert.ToDecimal("0.00"),
                dincome_head_March = Convert.ToDecimal("0.00"),
                dincome_head_Total = Convert.ToDecimal(dt.Rows[0][1382]) + Convert.ToDecimal(dt.Rows[0][1092])  + Convert.ToDecimal(dt.Rows[0][1093])
            };
          
            taxModel.DIncomeTotal = totalDIncome;
            //var totalSalaryHeads = new List<TotalSalaryHeadViewModel>();
            List<TotalSalaryHeadViewModel> totalSalaryHeads = new();

            var drSalary = new TotalSalaryHeadViewModel
            {
                Pay_Head = "Total",
                April = Convert.ToDecimal(dt.Rows[0][1396]),
                May = Convert.ToDecimal(dt.Rows[0][1397]),
                June = Convert.ToDecimal(dt.Rows[0][1398]),
                July = Convert.ToDecimal(dt.Rows[0][1399]),
                August = Convert.ToDecimal(dt.Rows[0][1400]),
                September = Convert.ToDecimal(dt.Rows[0][1401]),
                October = Convert.ToDecimal(dt.Rows[0][1402]),
                November = Convert.ToDecimal(dt.Rows[0][1403]),
                December = Convert.ToDecimal(dt.Rows[0][1404]),
                January = Convert.ToDecimal(dt.Rows[0][1405]),
                February = Convert.ToDecimal(dt.Rows[0][1406]),
                March = Convert.ToDecimal(dt.Rows[0][1407]),
                Total = decimal.Round(total_salary, 2)
            };

            totalSalaryHeads.Add(drSalary);

            taxModel.TotalIncomeRecords = totalSalaryHeads;
            //end of total salary insertion
            //reptotincome.DataSource = ds.Tables["Total_Salary_Head"];
            //reptotincome.DataBind();

            /// insertion of deduction head in report
            var DeductionHeadmodel = new TaxworkingNewViewModel();

            //// insertion of deduction head in report
            for (int i = 0, k = 62; i < 26; i++, k++)
            {
                var head = new DeductionHead
                {
                    PayHead = dt.Columns[k].ColumnName.Replace("_", " ").Replace("4", " ").ToUpper(),
                    April = Convert.ToDecimal(dt.Rows[0][k +j* 0]),
                    May = Convert.ToDecimal(dt.Rows[0][k + j * 1]),
                    June = Convert.ToDecimal(dt.Rows[0][k + j * 2]),
                    July = Convert.ToDecimal(dt.Rows[0][k + j * 3]),
                    August = Convert.ToDecimal(dt.Rows[0][k + j * 4]),
                    September = Convert.ToDecimal(dt.Rows[0][k + j * 5]),
                    October = Convert.ToDecimal(dt.Rows[0][k + j * 6]),
                    November = Convert.ToDecimal(dt.Rows[0][k + j * 7]),
                    December = Convert.ToDecimal(dt.Rows[0][k + j * 8]),
                    January = Convert.ToDecimal(dt.Rows[0][k + j * 9]),
                    February = Convert.ToDecimal(dt.Rows[0][k + j * 10]),
                    March = Convert.ToDecimal(dt.Rows[0][k + j * 11])
                };

                //decimal tot_dedu = decimal.Round(Convert.ToDecimal(dr_dedu_head[1]) + Convert.ToDecimal(dr_dedu_head[2]) + Convert.ToDecimal(dr_dedu_head[3]) + Convert.ToDecimal(dr_dedu_head[4]) + Convert.ToDecimal(dr_dedu_head[5]) + Convert.ToDecimal(dr_dedu_head[6]) + Convert.ToDecimal(dr_dedu_head[7]) + Convert.ToDecimal(dr_dedu_head[8]) + Convert.ToDecimal(dr_dedu_head[9]) + Convert.ToDecimal(dr_dedu_head[10]) + Convert.ToDecimal(dr_dedu_head[11]) + Convert.ToDecimal(dr_dedu_head[12]), 2);
                //dr_dedu_head[13] = tot_dedu;//dt.Rows[0][748 + i];
                //if (Convert.ToDouble(dr_dedu_head[13]) != 0)
                //{
                //    ds.Tables["Dedu_Head"].Rows.Add(dr_dedu_head);
                //}

                head.tot_dedu = decimal.Round(
                    head.April + head.May + head.June + head.July + head.August +
                    head.September + head.October + head.November + head.December +
                    head.January + head.February + head.March, 2);

                if (head.tot_dedu != 0)
                {
                    DeductionHeadmodel.DeductionHeads.Add(head);
                }
            }

            taxModel.DeductionHeads = DeductionHeadmodel.DeductionHeads;
            
            // Insert total deduction
            // DataRow dr_dedn_total = ds.Tables["Total_Dedu_Head"].NewRow();
            var totalHead = new DeductionHead
            {
                PayHead = "DEDN.",
                April = Convert.ToDecimal(dt.Rows[0][1149]),
                May = Convert.ToDecimal(dt.Rows[0][1150]),
                June = Convert.ToDecimal(dt.Rows[0][1151]),
                July = Convert.ToDecimal(dt.Rows[0][1152]),
                August = Convert.ToDecimal(dt.Rows[0][1153]),
                September = Convert.ToDecimal(dt.Rows[0][1154]),
                October = Convert.ToDecimal(dt.Rows[0][1155]),
                November = Convert.ToDecimal(dt.Rows[0][1156]),
                December = Convert.ToDecimal(dt.Rows[0][1157]),
                January = Convert.ToDecimal(dt.Rows[0][1158]),
                February = Convert.ToDecimal(dt.Rows[0][1159]),
                March = Convert.ToDecimal(dt.Rows[0][1160])
            };

            totalHead.tot_dedu = decimal.Round(
                totalHead.April + totalHead.May + totalHead.June + totalHead.July +
                totalHead.August + totalHead.September + totalHead.October +
                totalHead.November + totalHead.December + totalHead.January +
                totalHead.February + totalHead.March, 2);

            DeductionHeadmodel.DeductionTotal = totalHead;
            taxModel.DeductionTotal = DeductionHeadmodel.DeductionTotal;

            //repdedtotal.DataSource = ds.Tables["Total_Dedu_Head"];
            //repdedtotal.DataBind();
            ////end of total dedn insertion

            //// insertion of Perk head in report
            var perkHeads = new List<PerkRecord>();

            for (int i = 0, k = 88; i < 4; i++, k++)
            {
                var perkhead = new PerkRecord
                {
                    Pay_Head = dt.Columns[k].ColumnName.Replace("_", " ").Replace("4", " ").ToUpper(),
                    April = Convert.ToDecimal(dt.Rows[0][k + j * 0]),
                    May = Convert.ToDecimal(dt.Rows[0][k + j * 1]),
                    June = Convert.ToDecimal(dt.Rows[0][k + j * 2]),
                    July = Convert.ToDecimal(dt.Rows[0][k + j * 3]),
                    August = Convert.ToDecimal(dt.Rows[0][k + j * 4]),
                    September = Convert.ToDecimal(dt.Rows[0][k + j * 5]),
                    October = Convert.ToDecimal(dt.Rows[0][k + j * 6]),
                    November = Convert.ToDecimal(dt.Rows[0][k + j * 7]),
                    December = Convert.ToDecimal(dt.Rows[0][k + j * 8]),
                    January = Convert.ToDecimal(dt.Rows[0][k + j * 9]),
                    February = Convert.ToDecimal(dt.Rows[0][k + j * 10]),
                    March = Convert.ToDecimal(dt.Rows[0][k + j * 11]),
                    Total = Convert.ToDecimal(dt.Rows[0][1120 + i]) // using your total column
                };

                perkHeads.Add(perkhead);
            }
            taxModel.PerkRecords = perkHeads;
            //reppark.DataSource = ds.Tables["Perk_Head"];
            //reppark.DataBind();
            // total perks value
            ViewBag.TotalPerks = dt.Rows[0][1417].ToString();
            //end of perk head insertion
            //var model1 = new TaxworkingNewViewModel();
            //data insertion for 80c
            //for (int i = 0, j1 = 0; i <= 24; i++, j1 = j1 + 2)
            //{
            //    if (dt.Rows[0][1310 + i].ToString() != "") //1040
            //    {
            //        DataRow dr;
            //        dr = ds.Tables["deduction80c"].NewRow();
            //        dr[0] = dt.Rows[0][1310 + i];
            //        dr[1] = dt.Rows[0][1258 + j1];
            //        dr[2] = dt.Rows[0][1259 + j1];
            //        ds.Tables["deduction80c"].Rows.Add(dr);
            //    }
            //}
            // 80C deductions

            var deduction80C = new List<Deduction80C>();
            for (int i = 0, j1 = 0; i <= 24; i++, j1 += 2)
            {
                if (!string.IsNullOrEmpty(dt.Rows[0][1310 + i]?.ToString()))
                {
                    var deductionC = new Deduction80C
                    {
                        Header = dt.Rows[0][1310 + i].ToString(),
                        Proposed = Convert.ToDecimal(dt.Rows[0][1258 + j1].ToString()),
                        Actual = Convert.ToDecimal(dt.Rows[0][1259 + j1].ToString())
                    };
                    deduction80C.Add(deductionC);
                }
            }
            taxModel.Deduction80C = deduction80C;
            //rep80c.DataSource = ds.Tables["deduction80c"];
            //rep80c.DataBind();

            //for (int i1 = 0, j2 = 0; i1 <= 29; i1++, j2 = j2 + 2)
            //{
            //    if (dt.Rows[0][1336 + i1].ToString() != "")
            //    {
            //        DataRow dr;
            //        dr = ds.Tables["deduction80d"].NewRow();
            //        dr[0] = dt.Rows[0][1336 + i1];
            //        dr[1] = dt.Rows[0][1196 + j2];
            //        dr[2] = dt.Rows[0][1197 + j2];
            //        ds.Tables["deduction80d"].Rows.Add(dr);
            //    }
            //}

            // 80D deductions
            var deduction80D = new List<Deduction80D>();
            for (int i1 = 0, j2 = 0; i1 <= 29; i1++, j2 += 2)
            {
                if (!string.IsNullOrEmpty(dt.Rows[0][1336 + i1]?.ToString()))
                {
                    var deductionD = new Deduction80D
                    {
                        Header = dt.Rows[0][1336 + i1].ToString(),
                        Proposed = Convert.ToDecimal(dt.Rows[0][1196 + j2]),
                        Actual = Convert.ToDecimal(dt.Rows[0][1197 + j2])
                    };
                    deduction80D.Add(deductionD);
                }
            }
            taxModel.Deduction80D = deduction80D;
            //rep80d.DataSource = ds.Tables["deduction80d"];
            //rep80d.DataBind();
            //End Of 80c Data

            //footer row value insertion
            //var footermodel = new TaxworkingNewViewModel
            //{
            taxModel.NoticePay = dt.Rows[0][1409].ToString();
            taxModel.TotalPCS = dt.Rows[0][1409].ToString();
            taxModel.TotalHMSISalary = (total_salary + Convert.ToDecimal(dt.Rows[0][1417]) + Convert.ToDecimal(dt.Rows[0][1409])).ToString();
            taxModel.PreviousEmploymentSalary = dt.Rows[0][1125].ToString();
            taxModel.GrossSalary = dt.Rows[0][1138].ToString();

            taxModel.RentDetails = finance_year;
            taxModel.RentLocation = dt.Rows[0][1335].ToString();
            taxModel.RentAmount = dt.Rows[0][1193].ToString();

            taxModel.FinalHRA = dt.Rows[0][1144].ToString();
            taxModel.UniformAllowance = dt.Rows[0][1047].ToString();
            taxModel.LTAAmount = dt.Rows[0][1142].ToString();
            taxModel.LeaveEncashment = dt.Rows[0][1148].ToString();

            taxModel.TotalExemption = dt.Rows[0][1411].ToString();
            taxModel.EducationAllowance = dt.Rows[0][1140].ToString();
            taxModel.AttireAllowance = dt.Rows[0][1430].ToString();
            taxModel.TotalAnyExemption = dt.Rows[0][1412].ToString();
            taxModel.TotalExemptionSum = dt.Rows[0][1428].ToString();
            taxModel.TotalSalary = (Convert.ToDecimal(dt.Rows[0][1138]) - (Convert.ToDecimal(dt.Rows[0][1411]) + Convert.ToDecimal(dt.Rows[0][1140]) + Convert.ToDecimal(dt.Rows[0][1430]))).ToString();

            taxModel.StandardDeduction = dt.Rows[0][1366].ToString();
            taxModel.ProfessionalTaxDeduction = dt.Rows[0][1137].ToString();
            taxModel.EntertainmentAllowance = "0.00";
            taxModel.TotalDeduction16 = dt.Rows[0][1418].ToString();
            taxModel.IncomeChargeable = (Convert.ToDecimal(dt.Rows[0][1138]) - (Convert.ToDecimal(dt.Rows[0][1411]) + Convert.ToDecimal(dt.Rows[0][1140]) + Convert.ToDecimal(dt.Rows[0][1430]) + Convert.ToDecimal(dt.Rows[0][1418]))).ToString();

            taxModel.HouseProperty = dt.Rows[0][1420].ToString();
            taxModel.IncomeOtherSources = dt.Rows[0][1421].ToString();
            taxModel.TotalOtherIncome = dt.Rows[0][1422].ToString();
            taxModel.GrossTotal = (Convert.ToDecimal(dt.Rows[0][1138]) + Convert.ToDecimal(dt.Rows[0][1422]) - (Convert.ToDecimal(dt.Rows[0][1411]) + Convert.ToDecimal(dt.Rows[0][1140]) + Convert.ToDecimal(dt.Rows[0][1430]) + Convert.ToDecimal(dt.Rows[0][1418]))).ToString();

            taxModel.EmployeePF = dt.Rows[0][1429].ToString() == "New Tax" ? "0.00" : dt.Rows[0][1145].ToString();
            taxModel.EmployerNPS = dt.Rows[0][1091].ToString();
            taxModel.TotalDeductionChapter6A = dt.Rows[0][1135].ToString();
            taxModel.TaxableIncome = dt.Rows[0][1126].ToString();
            taxModel.OneTimePayment = dt.Rows[0][1147].ToString();
            taxModel.TaxThereon = dt.Rows[0][1127].ToString();
            taxModel.TaxLiability = dt.Rows[0][1309].ToString();
            taxModel.Surcharge = dt.Rows[0][1128].ToString();
            taxModel.HealthCess = dt.Rows[0][1129].ToString();
            taxModel.TaxWithSurcharge = dt.Rows[0][1132].ToString();

            taxModel.TaxDedPE = dt.Rows[0][1368].ToString();
            taxModel.TaxDedOtherSources = dt.Rows[0][1367].ToString();
            taxModel.TaxDedHMSI = dt.Rows[0][1369].ToString();
            taxModel.YTDTax = dt.Rows[0][1133].ToString();
            taxModel.BalanceTax = dt.Rows[0][1192].ToString();
            taxModel.TaxCurrentMonth = dt.Rows[0][1189].ToString();
            taxModel.TaxLiabilityOneTime = dt.Rows[0][1190].ToString();
            taxModel.TaxProjectedMonth = dt.Rows[0][1191].ToString();
            //};

            // Generate PDF
            byte[] pdfBytes = await GeneratePdftax(taxModel);

            if (pdfBytes == null || pdfBytes.Length == 0)
                return StatusCode(500, "Failed to generate PDF.");

            //string embedHtml =
            //    $"<object data=\"data:application/pdf;base64,{Convert.ToBase64String(pdfBytes)}\" " +
            //    "type=\"application/pdf\" width=\"99%\" height=\"500px\"></object>";

            //return Content(embedHtml, "text/html");
            string fileName = "";

            if (pdfBytes != null && pdfBytes.Length > 0)
            {
                string newFileName = "Temp" + "_" + DateTime.Now.Ticks + ".pdf";
                string rootFilePath = _appConfig.GetGeneralSettings().Get_FileUpload_Path;
                string folderPath = Path.Combine(rootFilePath, "ESS", "Temp");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string fullFilePath = Path.Combine(folderPath, newFileName);
                System.IO.File.WriteAllBytes(fullFilePath, pdfBytes);

                fileName = newFileName;
            }

            string filePath = "../../../Uploads/ESS/Temp/";
            string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"688px\"></object>";
            string _path = string.Format(embed, filePath + fileName);

            return Json(new
            {
                FILEPATH = _path,
                TEMP_FILENAME = fileName,
            });

        }
        [HttpGet]

        private async Task<byte[]> GeneratePdftax(TaxworkingNewViewModel model)
        {

            string htmlContent = await RenderViewToStringAsync("TaxworkingNew", model);

            using var memStream = new MemoryStream();
            using var doc = new Document(PageSize.A4, 20, 20, 20, 5);
            PdfWriter writer = PdfWriter.GetInstance(doc, memStream);
            doc.Open();

            using (var srdDocToString = new StringReader(htmlContent))
            {
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, srdDocToString);
            }

            if (writer.PageNumber == 0)
            {
                doc.Add(new Paragraph("No content available"));
            }

            doc.Close();
            return memStream.ToArray();
        }
        [HttpGet]
        public IActionResult SalarySlipAnnexure()
        {

            var kiList = _objpms.GetKiList()
                .AsEnumerable()
                .Where(r => r.Field<decimal>("SYKIID") > 19)
                .OrderBy(r => r.Field<decimal>("SYKIID"))
                .Select(r => new SelectListItem
                {
                    Value = r.Field<decimal>("SYKIID").ToString(),
                    Text = r.Field<string>("KICODE")
                })
                .ToList();

            string strKiId = _objpms.GetKIId();
            string selectedKiId = Convert.ToInt16(strKiId).ToString();

            ViewBag.FiscalYears = new SelectList(kiList, "Value", "Text", selectedKiId);

            // Build model with initial search results
            var model = new EssViewModel { KiId = selectedKiId };
            model = BuildSalarySlipAnnexureModel(model);

            return View(model); // full view with layout
        }

        // POST: AJAX call for fragment only
        [HttpPost]
        public IActionResult SalarySlipAnnexure_Searchbtn([FromBody] EssViewModel model)
        {
            var objModel = BuildSalarySlipAnnexureModel(model);
            return PartialView("_GetSalarySlipAnnexure", objModel);
        }

        private EssViewModel BuildSalarySlipAnnexureModel(EssViewModel model)
        {
            try
            {
                var objdt = _objpms.GetKiList();
                objdt.DefaultView.RowFilter = "SYKIID=" + model.KiId;
                string financialyear = objdt.DefaultView.ToTable().Rows[0]["FINANCIALYEAR"].ToString();

                string strfstratdate = financialyear.Substring(0, 4) + "0401";
                string strfenddate = financialyear.Substring(5, 4) + "0331";

                if (Convert.ToInt16(model.KiId) < 24)
                {
                    DataTable dt = _ePortalESS
                        .GetSalaryBreakup(_sessionService.Get<string>("userID"), "1", strfstratdate, strfenddate)
                        .Result;

                    var salaryList = new List<SalaryRow>();
                    foreach (DataRow row in dt.Rows)
                    {
                        var dto = new SalaryRow
                        {
                            StartDate = row[0].ToString(), // DateTime.Parse(row[1].ToString()).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                            EndDate = row[1].ToString(), //DateTime.Parse(row[2].ToString()).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                            ActionDesc = row[2].ToString(),
                            TfiAmt = decimal.Round(Convert.ToDecimal(row[3], CultureInfo.InvariantCulture) * 12, 0)
                        };
                        salaryList.Add(dto);
                    }
                    model.SalaryRows = salaryList;
                }
            }
            catch (Exception ex)
            {
                model.SalaryRows = new List<SalaryRow>();
            }

            return model;
        }
        [HttpPost]
        public async Task<IActionResult> SalarySlipAnnexure_RowEditing([FromBody] EssViewModel objEss)
        {
            try
            {
                var userid = _sessionService.Get<string>("userID");

                var model = BuildSalarySlipAnnexureModel(new EssViewModel
                {
                    KiId = objEss.KiId
                });

                if (model?.SalaryRows == null || objEss.rowIndex < 0 || objEss.rowIndex >= model.SalaryRows.Count)
                {
                    return BadRequest("Invalid row index");
                }
                var row = model.SalaryRows[objEss.rowIndex];

                var stdt = row.StartDate?.ToString();
                var endt = row.EndDate?.ToString();
                var kiId = objEss.KiId;

                //var kiId = TempData["KIID"]?.ToString();
                //var stdt = TempData["STDT"]?.ToString();
                //var endt = TempData["ENDT"]?.ToString();

                if (string.IsNullOrEmpty(stdt) || string.IsNullOrEmpty(endt))
                {
                    return RedirectToAction("SalarySlipAnnexure", "Ess");
                }
                var dtemp = _common.GetEmployeeOfficialDetails(userid, kiId);

                // Parse using dd.MM.yyyy format
                DateTime startDate = DateTime.ParseExact(stdt, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                DateTime endDate = DateTime.ParseExact(endt, "dd.MM.yyyy", CultureInfo.InvariantCulture);

                // Convert to yyyyMMdd format
                string stdate = startDate.ToString("yyyyMMdd");
                string enddate = endDate.ToString("yyyyMMdd");

                //DataTable dt = await _ePortalESS.GetSalaryBreakup(userid, "2", stdate, enddate).Result;

                var dtSalaryBreakup = await _ePortalESS.GetSalaryBreakup2(userid, "2", stdate, enddate);


                var dtTFIdt = dtSalaryBreakup.Tables["TFIdt"];

                var ComponentsItems = new List<TFIComponentsItemsViewModel>();

                decimal totalSumA = 0;

                foreach (DataRow rowFTI in dtTFIdt.Rows)
                {
                    decimal amount = Convert.ToDecimal(rowFTI["AMOUNT"]) * 12;
                    totalSumA += amount;

                    ComponentsItems.Add(new TFIComponentsItemsViewModel
                    {
                        Description = rowFTI["DESCRIPTION"]?.ToString(),
                        Amount = amount
                    });
                }


                var dtPERK = dtSalaryBreakup.Tables["PERKdt"];

                var perquisitesItems = new List<PerquisitesItemsViewModel>();

                decimal totalPerquisitesB = 0;
                foreach (DataRow rowPERK in dtPERK.Rows)
                {
                    decimal amount = Convert.ToDecimal(rowPERK["AMOUNT"]);
                    totalPerquisitesB += amount;

                    perquisitesItems.Add(new PerquisitesItemsViewModel
                    {
                        Description = rowPERK["DESCRIPTION"]?.ToString(),
                        Amount = Convert.ToDecimal(rowPERK["AMOUNT"])
                    });
                }
                var dtVARI = dtSalaryBreakup.Tables["VARIdt"];

                var companyItems = new List<CompanyItemsViewModel>();

                decimal totalSumC = 0;
                foreach (DataRow rowVARI in dtVARI.Rows)
                {
                    decimal amount = Convert.ToDecimal(rowVARI["AMOUNT"]);
                    totalSumC += amount;

                    companyItems.Add(new CompanyItemsViewModel
                    {
                        Description = rowVARI["DESCRIPTION"]?.ToString(),
                        Amount = Convert.ToDecimal(rowVARI["AMOUNT"])
                    });
                }

                var empModel = new EssViewModel
                {
                    EmpCode = dtemp.Rows[0]["ADEMPCODE"].ToString(),
                    EmpName = dtemp.Rows[0]["EMPNAME"].ToString(),
                    Operation = dtemp.Rows[0]["OPERATION"].ToString(),
                    Division = dtemp.Rows[0]["DIVISION"].ToString(),
                    Department = dtemp.Rows[0]["DEPARTMENT"].ToString(),
                    Designation = dtemp.Rows[0]["DESIGNATION"].ToString(),
                    StartDate = stdt,
                    EndDate = endt,
                    EffectiveDateText = stdt,
                    TFIComponentsItems = ComponentsItems,
                    PerquisitesItems = perquisitesItems,
                    CompanyItems = companyItems,
                    TotalSumA = totalSumA,
                    TotalPerquisitesB = totalPerquisitesB,
                    TotalSumC = totalSumC
                };
                //dt.DefaultView.RowFilter = "BREAKUPTYPE='TFI'";
                //TFIdt = dt.DefaultView.ToTable();
                //dt.DefaultView.RowFilter = "BREAKUPTYPE='PERK'";
                //PERKdt = dt.DefaultView.ToTable();
                //dt.DefaultView.RowFilter = "BREAKUPTYPE='VARI'";
                //VARIdt = dt.DefaultView.ToTable();

                // Generate the PDF bytes directly
                byte[] pdfBytes = await GeneratePdf(empModel);

                string fileName = "";

                if (pdfBytes != null && pdfBytes.Length > 0)
                {
                    string newFileName = "Temp" + "_" + DateTime.Now.Ticks + ".pdf";
                    string rootFilePath = _appConfig.GetGeneralSettings().Get_FileUpload_Path;
                    string folderPath = Path.Combine(rootFilePath, "ESS", "Temp");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    DirectoryInfo di = new DirectoryInfo(folderPath);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }

                    string fullFilePath = Path.Combine(folderPath, newFileName);
                    System.IO.File.WriteAllBytes(fullFilePath, pdfBytes);

                    fileName = newFileName;
                }

                string filePath = "../../../Uploads/ESS/Temp/";
                string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"688px\"></object>";
                string _path = string.Format(embed, filePath + fileName);

                return Json(new
                {
                    FILEPATH = _path,
                    TEMP_FILENAME = fileName,
                });
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        // Helper method to generate PDF bytes
        private async Task<byte[]> GeneratePdf(EssViewModel model)
        {

            string htmlContent = await RenderViewToStringAsync("AnnexureDetail", model);

            using var memStream = new MemoryStream();
            using var doc = new Document(PageSize.A4, 20, 20, 20, 5);

            PdfWriter writer = PdfWriter.GetInstance(doc, memStream);
            doc.Open();

            using (var srdDocToString = new StringReader(htmlContent))
            {
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, srdDocToString);
            }

            if (writer.PageNumber == 0)
            {
                doc.Add(new Paragraph("No content available"));
            }

            doc.Close();
            return memStream.ToArray();
        }

        private async Task<string> RenderViewToStringAsync(string viewName, object model)
        {
            var actionContext = new ActionContext(HttpContext, RouteData, ControllerContext.ActionDescriptor);

            using var sw = new StringWriter();
            var viewResult = _viewEngine.FindView(actionContext, viewName, false);

            if (viewResult.View == null)
            {
                throw new ArgumentNullException($"{viewName} not found");
            }

            var viewDictionary = new ViewDataDictionary(
                new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(),
                new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary())
            {
                Model = model
            };

            var tempData = new TempDataDictionary(HttpContext, _tempDataProvider);

            var viewContext = new ViewContext(
                actionContext,
                viewResult.View,
                viewDictionary,
                tempData,
                sw,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);
            return sw.ToString();
        }

        public IActionResult Remuneration()
        {
            // Equivalent of ViewState["PID"] = 0;
            TempData["PID"] = 0;

            //if (DateTime.Now.Month.ToString().Length == 1)
            //    date_mon = "0" + DateTime.Now.Month.ToString();
            //if (DateTime.Now.Day.ToString().Length == 1)
            //    date_day = "0" + DateTime.Now.Day.ToString();

            //txtfrom.Text = "01/" + date_mon + "/" + DateTime.Now.Year;

            //DateClass getday = new DateClass();
            //txtto.Text = getday.GetDaysInMonth(DateTime.Now.Month, DateTime.Now.Year) + "/" + date_mon + "/" + DateTime.Now.Year;
            //Button1.Style.Add(HtmlTextWriterStyle.Cursor, "hand");

            var now = DateTime.Now;
            var dateMon = now.Month.ToString("D2"); // pad with leading zero
            var dateDay = now.Day.ToString("D2");

            PunchReportViewModel objModel = new PunchReportViewModel
            {
                //txtfrom = $"01/{dateMon}/{now.Year}",
                //txtto = $"{DateTime.DaysInMonth(now.Year, now.Month)}/{dateMon}/{now.Year}"

                txtfrom = "01/03/2024",
                txtto = "01/07/2024"
            };

            return View(objModel);
        }

        [HttpPost]
        public IActionResult RemunerationClick([FromBody] PunchReportViewModel model)
        {

            DateTime validationDate;
            DateTime validationDateTo;

            if (DateTime.Now.Month <= 3)
                validationDate = DateTime.ParseExact($"01/03/{DateTime.Now.Year - 2}", "dd/MM/yyyy", null);
            else
                validationDate = DateTime.ParseExact($"01/03/{DateTime.Now.Year - 1}", "dd/MM/yyyy", null);

            validationDateTo = DateTime.ParseExact("01/07/2024", "dd/MM/yyyy", null);
            var selectedOption = model.SelectedOption;

            // IF Payslip Result In Period is enabled
            if (selectedOption == "Period") // if (RadioButton3.Checked == true)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(model.txtfrom))
                    {
                        return Json(new { status = "error", message = "Please Enter Date in dd/MM/yyyy format, eg. 01/01/1800" });
                    }
                    if (string.IsNullOrWhiteSpace(model.txtto))
                    {
                        return Json(new { status = "error", message = "Please Enter Date in dd/MM/yyyy format, eg. 01/01/1800" });
                    }

                    var dateFrom = DateTime.ParseExact(model.txtfrom, "dd/MM/yyyy", null);
                    var dateTo = DateTime.ParseExact(model.txtto, "dd/MM/yyyy", null);

                    if (dateFrom > dateTo)
                    {
                        return Json(new { status = "error", message = "To date must be greater than From date" });
                    }
                    if (dateFrom < validationDate)
                    {
                        return Json(new { status = "error", message = $"From date must be greater than {validationDate:dd-MMM-yyyy}" });
                    }
                    if (dateTo > validationDateTo)
                    {
                        return Json(new { status = "error", message = "To date must be less than 01-Jul-2024" });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { status = "error", message = "Date Error: Check Date field on page" });
                }
            }
            //***************
            //sap parameters
            try
            {
                //var objess = new EportalESS();
                DataTable dt = new DataTable();

                //*********************
                //****************Condition Checking (based on this condion salary slip data are retreiving from SAP )
                if (selectedOption == "Period") // if (RadioButton3.Checked == true)
                {
                    var dateFrom = DateTime.ParseExact(model.txtfrom, "dd/MM/yyyy", null);
                    var dateTo = DateTime.ParseExact(model.txtto, "dd/MM/yyyy", null);

                    string df = dateFrom.ToString("yyyyMMdd");
                    string dtStr = dateTo.ToString("yyyyMMdd");
                    dt = _ePortalESS.GetPayrollResultList(_sessionService.Get<string>("userID"), df, dtStr).Result;
                }
                if (selectedOption == "Current") //if (RadioButton1.Checked == true)
                {
                    string dateMon = DateTime.Now.Month.ToString("D2");
                    string strDate = $"01/{dateMon}/{DateTime.Now.Year}";
                    DateTime currDate = DateTime.ParseExact(strDate, "dd/MM/yyyy", null);

                    if (currDate < validationDate)
                    {
                        //model.RemunerationResults = new List<RemunerationRecord>();
                        return Json(new { status = "error", message = $"From date must be greater than {validationDate:dd-MMM-yyyy}" });
                    }

                    var userId = _sessionService.Get<string>("userID");
                    string from = $"{DateTime.Now.Year}{dateMon}01";
                    string to = $"{DateTime.Now.Year}{dateMon}{DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)}";

                    dt = _ePortalESS.GetPayrollResultList(userId, from, to).Result;
                }
                else if (selectedOption == "Previous") //if (RadioButton2.Checked == true)
                {
                    // Calculate previous month
                    int prevMonth = DateTime.Now.Month - 1;
                    int year = DateTime.Now.Year;

                    if (prevMonth == 0)
                    {
                        prevMonth = 12;
                        year--;
                    }

                    string dateMon = prevMonth.ToString("D2"); // pad with leading zero
                    string strDate = $"01/{dateMon}/{year}";
                    DateTime currDate = DateTime.ParseExact(strDate, "dd/MM/yyyy", null);

                    if (currDate < validationDate)
                    {
                        return Json(new { status = "error", message = $"From date must be greater than {validationDate:dd-MMM-yyyy}" });
                    }


                    // Equivalent SAP call
                    var userId = _sessionService.Get<string>("userID");
                    string from = $"{year}{dateMon}01";
                    string to = $"{year}{dateMon}{DateTime.DaysInMonth(year, prevMonth)}";

                    dt = _ePortalESS.GetPayrollResultList(userId, from, to).Result;
                }
                var results = new List<EssViewModel.RemunerationRecord>();

                foreach (DataRow row in dt.Rows)
                {
                    var record = new EssViewModel.RemunerationRecord
                    {
                        SequenceNumber = Convert.ToInt32(row["SEQUENCENUMBER"]),
                        FpBegin = DateTime.ParseExact(row["FPBEGIN"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        FpEnd = DateTime.ParseExact(row["FPEND"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        PayDate = DateTime.ParseExact(row["PAYDATE"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        PayTypeText = row["PAYTYPE_TEXT"].ToString()
                    };

                    results.Add(record);
                }

                model.RemunerationResults = results;
                return View("_GetRemuneration", model);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = "There is an error on page, kindly contact SIS Helpdesk" });
            }
        }

        [HttpPost]

        public async Task<IActionResult> EditRemuneration([FromBody] RemunerationRequest model)

        {

            int seqno = model.Seqno;
            DateTime paymentDate = model.PaymentDate;
            DateTime validDateCheck = DateTime.ParseExact("30/04/2021", "dd/MM/yyyy", CultureInfo.InvariantCulture);

            // Business rule
            if (DateTime.Now.Date > validDateCheck && paymentDate.Date >= DateTime.Now.Date)
            {
                return Json(new { status = "error", message = "Payslip can't be viewed before payment date" });

            }

            // Fetch payslip data
            DataTable dt = await _ePortalESS.GetPaySlip(
                _sessionService.Get<string>("userID"),
                seqno.ToString(),
                ""
            );

            if (dt == null || dt.Rows.Count == 0)
                return Content("No payslip data found.", "text/plain");
            // For simplicity, assume TEXT_COL contains lines of payslip text
            var lines = dt.AsEnumerable()
                          .Select(r => Convert.ToString(r["TEXT_COL"]))
                          .Where(s => !string.IsNullOrWhiteSpace(s))
                          .ToList();

            var sb = new StringBuilder();

            var firstRow = dt.Rows[1][1].ToString().Split("|");/// lines[0];


            sb.AppendLine("<!DOCTYPE html><html><head><meta charset='UTF-8'/><style>");
            sb.AppendLine("body { font-family: Arial, sans-serif; font-size: 10px; margin: 20px; }");
            sb.AppendLine(".container { max-width: 900px; margin: auto; border: 1px solid #000; padding: 20px; }");
            sb.AppendLine("h2 { text-align: center; margin: 5px 0; }");
            sb.AppendLine("table { width: 100%; border-collapse: collapse; margin-bottom: 20px; }");
            sb.AppendLine("td { padding: 6px 8px; text-align: left; font-size:8px; }");
            sb.AppendLine(".outer-border { border: 1px solid #000; }");
            sb.AppendLine(".outer-border td { border-left: 1px solid #000; border-right: 1px solid #000; }");
            sb.AppendLine(".outer-border tr:first-child td { border-top: 1px solid #000; border-bottom: 1px solid #000; }");
            sb.AppendLine(".outer-border tr:last-child td { border-bottom: 1px solid #000; }");
            sb.AppendLine(".first-child{ border-top: 1px solid #000; border-bottom: 1px solid #000; }");
            sb.AppendLine(".last-child { border-bottom: 1px solid #000; }");
            sb.AppendLine(".outer-border tr td { border-left: 1px solid #000; border-right: 1px solid #000; }");
            sb.AppendLine(".header-row { text-align: center; border-top: 1px solid #000; border-bottom: 1px solid #000; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<div class='container'>");
            sb.AppendLine($"<h2>{H(dt.Rows[0][1].ToString())}</h2>");
            sb.AppendLine("<table class='outer-border'>");

            // Employee Details (first row)

            sb.AppendLine("<tr >");
            sb.AppendLine($"<td class='last-child'>{H(firstRow[1])}</td><td colspan='2' class='last-child'>{H(firstRow[2])}</td>");
            sb.AppendLine($"<td class='last-child' colspan='3'>{H(firstRow[3])}</td>");
            sb.AppendLine($"<td class='last-child' colspan='2'>{H(firstRow[4])}</td>");
            sb.AppendLine("</tr>");

            for (int i = 3; i < 10; i++)

            {

                var border = i == 9 ? "last-child" : "";
                var Row = dt.Rows[i][1].ToString().Split("|");/// lines[0];
                var Col2 = Row[3].Split(":");
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td class='{border}'>{H(Row[1])}</td><td colspan='2' class='{border}'>{H(Row[2])}</td>");
                sb.AppendLine($"<td class='{border}'>{H(Col2[0])}</td><td colspan='2' class='{border}'>{H(Col2[1])}</td>");
                sb.AppendLine($"<td class='{border}'>{H(Row[4])}</td><td class='{border}'>{H(Row[5])}</td>");
                sb.AppendLine("</tr>");

            }

            // Earnings & Deductions Header

            sb.AppendLine("<tr class='header-row'>");
            sb.AppendLine("<td class='last-child'>Earnings</td><td class='last-child'>Current Month</td><td class='last-child'>Arrears</td><td class='last-child'>YTD Amount</td>");
            sb.AppendLine("<td class='last-child'>Deductions</td><td class='last-child'>Current Month</td><td class='last-child'>Arrears</td><td class='last-child'>YTD Amount</td>");
            sb.AppendLine("</tr>");

            // Earnings & Deductions Rows (loop through vm.Rows)

            for (int i = 14; i < 30; i++)

            {

                var Row = dt.Rows[i][1].ToString().Split("|");/// lines[0];
                sb.AppendLine("<tr>");
                var border = i == 29 ? "last-child" : "";
                for (int j = 1; j < Row.Length - 1; j++)
                {
                    sb.AppendLine($"<td class='{border}'>{H(Row[j])}</td>");
                }
                sb.AppendLine("</tr>");
            }

            for (int i = 31; i < 32; i++)
            {
                var Row = dt.Rows[i][1].ToString().Split("|");/// lines[0];
                var border = "last-child";
                sb.AppendLine("<tr>");
                for (int j = 1; j < Row.Length - 1; j++)
                {
                    sb.AppendLine($"<td class='{border}'>{H(Row[j])}</td>");
                }
                sb.AppendLine("</tr>");
            }

            // Net Pay & Leaves

            var Leave = dt.Rows[33][1].ToString().Split("|");

            sb.AppendLine($"<tr class='header-row'><td colspan='2' class='last-child'>{H(Leave[1])}</td><td colspan='3' class='last-child'>{H(Leave[2])}</td><td class='last-child'>{H(Leave[3])}</td><td class='last-child'>{H(Leave[4])}</td><td class='last-child'>{H(Leave[5])}</td></tr>");

            // Loan Header

            sb.AppendLine("<tr class='header-row'><td class='last-child'>Loan Type</td><td class='last-child'>Total dedu. Amount</td><td colspan='2' class='last-child'>E.M.I</td><td class='last-child'>Total No. of Instl.</td><td class='last-child'>Current Instl. No.</td><td colspan='2' class='last-child'>Balance Deductable Amount </td></tr>");

            //Loan Rows

            for (int i = 38; i < 45; i++)
            {
                var border = i == 44 ? "last-child" : "";
                var Row = dt.Rows[i][1].ToString().Split("|");/// lines[0];
                sb.AppendLine($"<tr><td class='{border}'>{H(Row[1])}</td><td class='{border}'>{H(Row[2])}</td><td colspan='2' class='{border}'>{H(Row[3])}</td><td class='{border}'>{H(Row[4])}</td><td class='{border}'>{H(Row[5])}</td><td colspan='2' class='{border}'>{H(Row[6])}</td></tr>");
            }
            // Income Tax Header & Row
            sb.AppendLine("<tr class='header-row'><td class='last-child'>&nbsp;</td><td class='last-child'>Paid till Pre. Month</td><td  colspan='2' class='last-child'>Current Month Deduction</td><td   colspan='2' class='last-child'>Total Paid</td><td class='last-child'>Estimated Future Laib</td><td class='last-child'>Total For Year</td></tr>");
            for (int i = 49; i < 52; i++)
            {
                var border = i == 51 ? "last-child" : "";
                var Row = dt.Rows[i][1].ToString().Split("|");/// lines[0];
                sb.AppendLine($"<tr><td class='{border}'>{H(Row[1])}</td><td class='{border}'>{H(Row[2])}</td><td colspan='2' class='{border}'>{H(Row[3])}</td><td  colspan='2' class='{border}'>{H(Row[4])}</td><td class='{border}'>{H(Row[5])}</td><td class='{border}'>{H(Row[6])}</td></tr>");
            }
            sb.AppendLine("</table>");
            // Footer
            sb.AppendLine("<div class='footer'>");
            sb.AppendLine("<p>Know your PF Balance: By giving a missed call to 011-22901406 from your registered mobile number<br/> or Login to epfindia.gov.in for E-passbook.</p>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            PayslipViewModel data = new PayslipViewModel();

            var html = WebUtility.HtmlDecode(sb.ToString());

            byte[] pdfBytes = HtmlToPdfApplyHeadStyle(html);

            if (pdfBytes == null || pdfBytes.Length == 0)
                return StatusCode(500, "Failed to generate PDF.");

            return File(pdfBytes, "application/pdf", "Payslip.pdf");

        }
        private string H(string? s)
        {
            var text = string.IsNullOrEmpty(s) ? "&nbsp;" : s.Trim().Replace("\"", "");
            return text = string.IsNullOrEmpty(text) ? "&nbsp;" : text;
        }
        private string F(decimal? d)
        {
            if (!d.HasValue) return "";
            // Indian number formatting
            var ci = new CultureInfo("en-IN");
            return d.Value.ToString("#,0.00", ci);
        }
        public byte[] HtmlToPdfApplyHeadStyle(string html, string? unicodeTtfPath = null)
        {
            if (string.IsNullOrWhiteSpace(html))
                throw new ArgumentException("HTML is empty.");

            // Fix codepage issues (e.g., 1252)
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using var ms = new MemoryStream();
            var document = new Document(PageSize.A4, 36, 36, 36, 36);
            var writer = PdfWriter.GetInstance(document, ms);
            writer.CloseStream = false;

            document.Open();

            // --- CSS Resolver (parses <style> blocks and inline styles) ---
            var cssResolver = new StyleAttrCSSResolver();
            // If you also want to add extra CSS programmatically:
            // ICSSFile extraCss = XMLWorkerHelper.GetCSS(new StringReader("td { border:1px solid #000; }"));
            // cssResolver.AddCss(extraCss);

            // --- Font provider (optional but recommended for Unicode like ₹ or Devanagari) ---
            var fontProvider = new XMLWorkerFontProvider(XMLWorkerFontProvider.DONTLOOKFORFONTS);
            //if (!string.IsNullOrWhiteSpace(unicodeTtfPath) && File.Exists(unicodeTtfPath))
            //{
            //    fontProvider.Register(unicodeTtfPath, "NotoSans"); // Then use in CSS: body { font-family: NotoSans; }
            //}

            var cssAppliers = new CssAppliersImpl(fontProvider);

            // --- HTML pipeline context ---
            var htmlContext = new HtmlPipelineContext(cssAppliers);
            htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());

            // --- Build pipeline: CSS -> HTML -> PDF ---
            var pipeline = new CssResolverPipeline(cssResolver,
                            new HtmlPipeline(htmlContext,
                            new PdfWriterPipeline(document, writer)));

            var worker = new XMLWorker(pipeline, true);
            var parser = new XMLParser(worker);

            // IMPORTANT: html must contain REAL tags ("<style>", not "&lt;style&gt;")
            html = Regex.Replace(html, "&(?!(amp|apos|quot|lt|gt);)", "&amp;");
            using var htmlStream = new MemoryStream(Encoding.UTF8.GetBytes(html));
            using var srHtml = new StreamReader(htmlStream, Encoding.UTF8);
            parser.Parse(srHtml);

            document.Close();

            if (writer.PageNumber == 0)
                throw new InvalidOperationException("No pages created. HTML may be invalid or parsing failed.");

            return ms.ToArray();
        }
        [HttpGet]
        public IActionResult RealtimeAccess_PunchReport()
        {

            var userId = _sessionService.Get<string>("userID");
            var employeeDetails = _sessionService.Get<Employee_Details>("Employee");

            var model = new PunchReportViewModel
            {
                FullName = employeeDetails?.Employee_Name,
                FromDate = DateTime.Today
            };

            DataTable objDt = _objleave.EmployeeDataGrid(userId);

            if (objDt.Rows.Count > 0)
            {
                model.TotalValue = objDt.Rows.Count;

                var employees = objDt.AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Text = r["EMPNAME"].ToString(),
                        Value = r["ADEMPCODE"].ToString()
                    }).ToList();

                employees.Insert(0, new SelectListItem { Text = "--Select--", Value = "" });

                var exists = employees.FirstOrDefault(e => e.Value == userId);
                if (exists == null)
                {
                    employees.Insert(1, new SelectListItem { Text = model.FullName, Value = userId });
                }

                model.EmployeeList = employees;
                model.cmbemp = int.TryParse(userId, out var id) ? id : 0;
            }
            else
            {
                model.EmployeeList = new List<SelectListItem>
                {
                    new SelectListItem { Text = "--Select--", Value = "" },
                    new SelectListItem { Text = model.FullName, Value = userId }
                };
                model.cmbemp = int.TryParse(userId, out var id) ? id : 0;
            }

            return View(model);
        }
        [HttpPost]
        public IActionResult RealtimeAccess_PunchReportClick([FromBody] PunchReportViewModel model)
        {
            var personnelNumber = model.PersonnelNumber;
            var fromDateStr = model.FromDate.ToString("dd-MMM-yyyy");
            var reportType = model.ReportType;

            //model.ShowSummary = true;
            //*******************
            //Fetching Attendance detail From SAP
            DataTable dt1 = null;
            DataTable dtcnt = null;
            if (reportType == "Access")
            {
                dt1 = _objleave.GetAccessReport(_sessionService.Get<string>("userID"), fromDateStr, personnelNumber);

                var pList = dt1.AsEnumerable().Select(row => new PunchReportViewModel
                {
                    EMPNAME = row["EMPNAME"]?.ToString(),
                    ADEMPCODE = row["ADEMPCODE"]?.ToString(), // fix: avoid InvalidCastException
                    PUNCHINDATETIME = row["PUNCHINDATETIME"] == DBNull.Value
                      ? (DateTime?)null
                      : Convert.ToDateTime(row["PUNCHINDATETIME"]),
                    EXPECTED_HALFDAYTIME = row["EXPECTED_HALFDAYTIME"]?.ToString(),
                    PUNCHOUTDATETIME = row["PUNCHOUTDATETIME"]?.ToString(),
                    InOut = row["in_out"]?.ToString(),
                    LOCATION = row["LOCATION"]?.ToString()
                }).ToList();


                model.PunchReporRecords = pList;
            }

            if (reportType == "Punch")
            {
                dt1 = _objleave.GetRealTimeatt(_sessionService.Get<string>("userID"), fromDateStr, personnelNumber);

                if (dt1 == null || dt1.Rows.Count == 0)
                {
                    var totalAssociates1 = string.IsNullOrEmpty(personnelNumber)
                                         ? model.TotalValue
                                         : 1;
                    model.TotalAssociates = totalAssociates1;
                    return PartialView("_GetRealtimeAccess_PunchReport", model);
                }

                // Build final table with grouped rows
                var finalTable = dt1.Clone();
                var grouped = dt1.AsEnumerable()
                                 .GroupBy(row => new
                                 {
                                     EmpCode = row["ADEMPCODE"].ToString(),
                                     PunchDate = Convert.ToDateTime(row["PUNCHINDATETIME"]).Date
                                 });

                foreach (var group in grouped)
                {
                    bool isFirst = true;
                    foreach (DataRow row in group.OrderBy(r => Convert.ToDateTime(r["PUNCHINDATETIME"])))
                    {
                        var newRow = finalTable.NewRow();
                        newRow.ItemArray = row.ItemArray.Clone() as object[];

                        if (!isFirst)
                        {
                            newRow["EXPECTED_HALFDAYTIME"] = "-";
                            newRow["PUNCHOUTDATETIME"] = "-";
                        }

                        finalTable.Rows.Add(newRow);
                        isFirst = false;
                    }
                }

                var noteMessage = (finalTable.Rows.Count > 0 && finalTable.Rows[0]["EXPECTED_HALFDAYTIME"].ToString() != "-")
                    ? "Note: The expected out-time for Half Day and Full Day is calculated based on the first punch registered in attendance machine (not on access machine)."
                    : null;

                var aList = finalTable.AsEnumerable().Select(row => new PunchReportViewModel
                {
                    EMPNAME = row["EMPNAME"]?.ToString(),
                    ADEMPCODE = row["ADEMPCODE"]?.ToString(),
                    PUNCHINDATETIME = row["PUNCHINDATETIME"] == DBNull.Value
                    ? (DateTime?)null
                    : Convert.ToDateTime(row["PUNCHINDATETIME"]),
                    EXPECTED_HALFDAYTIME = row["EXPECTED_HALFDAYTIME"]?.ToString(),
                    PUNCHOUTDATETIME = row["PUNCHOUTDATETIME"]?.ToString(),
                    InOut = row["in_out"]?.ToString(),
                    LOCATION = row["LOCATION"]?.ToString(),
                    //TotalAssociates = totalAssociates,
                    //AssociatePunch = associatePunch,
                    NoteMessage = noteMessage
                }).ToList();

                model.PunchReporRecords = aList;

                model.ShowHalfDayColumn = true;

            }

            var totalAssociates = string.IsNullOrEmpty(personnelNumber)
                            ? model.TotalValue
                            : 1;

            dtcnt = dt1.DefaultView.ToTable(true, "ADEMPCODE");
            var associatePunch = dtcnt.Rows.Count;

            model.TotalAssociates = totalAssociates;
            model.AssociatePunch = associatePunch;

            return PartialView("_GetRealtimeAccess_PunchReport", model);
        }
        public IActionResult HLISubsidyStatement()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            // Button1.Style.Add(HtmlTextWriterStyle.Cursor, "hand");
            ViewData["ButtonCursor"] = "pointer";
            var model = new HLISubsidyStatementViewModel
            {
                FromDate = DateTime.Today,
                ToDate = DateTime.Today
            };
            return View();
        }
        [HttpPost]
        public IActionResult HLISubsidyStatementClick([FromBody] HLISubsidyStatementViewModel model)
        {
            var userId = _sessionService.Get<string>("userID");

            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            DataTable filteredData = new DataTable();

            string result = model.rpOption;

            DataTable dt = _ePortalESS.GetHLISStatement("50AG", "E" + userId, "", "").Result;

            if (result == "Result")
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    int rowIndex = 0; // Change this to the desired row index
                    string columnName = "MESSAGE"; // Change this to the desired column name
                    // Check if the column exists in the DataTable
                    if (dt.Columns.Contains(columnName))
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            // Access the value of the "message" column for each row
                            string messageValue = row[columnName]?.ToString();

                            if (!string.IsNullOrEmpty(messageValue) &&
                                messageValue.ToLower().Contains("invalid employee"))
                            {
                                model.Message = "Invalid Employee";
                                return View("HLISubsidyStatement", model);
                            }
                        }
                    }
                    else
                    {
                        var filteredRows = dt.AsEnumerable();

                        if (model.FromDate != null && model.ToDate != null)
                        {
                            filteredRows = filteredRows.Where(r =>
                            {
                                DateTime dvalut = DateTime.Parse(r["DVALUT"].ToString());
                                return dvalut >= model.FromDate && dvalut <= model.ToDate;
                            });
                        }

                        if (filteredRows.Any())
                        {
                            filteredData = filteredRows.CopyToDataTable();
                        }

                    }
                }
            }
            model.HLISubsidyRecords = filteredData
              .AsEnumerable()
              .Select(r => new HLISubsidyRecord
              {
                  KONTRH = r["KONTRH"]?.ToString(),
                  NAME = r["NAME"]?.ToString(),
                  BU_SORT1 = r["BU_SORT1"]?.ToString(),
                  ZUOND = r["ZUOND"]?.ToString(),
                  LOANPROPADD = r["LOANPROPADD"]?.ToString(),
                  REFER = string.IsNullOrEmpty(r["REFER"]?.ToString()) ? 0 : Convert.ToInt32(r["REFER"]),
                  DVALUT = string.IsNullOrEmpty(r["DVALUT"]?.ToString()) ? DateTime.MinValue : DateTime.Parse(r["DVALUT"].ToString()),
                  DVALUT1 = r["DVALUT1"]?.ToString(),
                  PKOND = string.IsNullOrEmpty(r["PKOND"]?.ToString()) ? 0 : Convert.ToDecimal(r["PKOND"]),
                  BBASIS = string.IsNullOrEmpty(r["BBASIS"]?.ToString()) ? 0 : Convert.ToDecimal(r["BBASIS"]),
                  MI = string.IsNullOrEmpty(r["MI"]?.ToString()) ? 0 : Convert.ToDecimal(r["MI"]),
                  MP = string.IsNullOrEmpty(r["MP"]?.ToString()) ? 0 : Convert.ToDecimal(r["MP"]),
                  EMI = string.IsNullOrEmpty(r["EMI"]?.ToString()) ? 0 : Convert.ToDecimal(r["EMI"]),
                  PEB = string.IsNullOrEmpty(r["PEB"]?.ToString()) ? 0 : Convert.ToDecimal(r["PEB"]),
                  INT_SUB = string.IsNullOrEmpty(r["INT_SUB"]?.ToString()) ? 0 : Convert.ToDecimal(r["INT_SUB"])
              })
              .ToList();
            return View("_GetHLISubsidyStatement", model);
        }
        [HttpPost]
        public ActionResult ExportGridToExcel([FromBody] HLISubsidyStatementViewModel model)
        {
            short retVal = 0;
            try
            {
                var userId = _sessionService.Get<string>("userID");
                if (userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                // Get data from service
                DataTable dt = _ePortalESS.GetHLISStatement("50AG", "E" + userId, "", "").Result;

                if (dt != null && dt.Rows.Count > 0)
                {
                    string columnName = "MESSAGE"; // Change this to the desired column name

                    if (dt.Columns.Contains(columnName))
                    {
                        // Check if the column exists in the DataTable
                        foreach (DataRow row in dt.Rows)
                        {
                            // Access the value of the "message" column for each row
                            string messageValue = row[columnName]?.ToString(); // Use null-conditional operator for safety

                            // Check if the "message" column contains "invalid employee"
                            if (!string.IsNullOrEmpty(messageValue) &&
                                messageValue.ToLower().Contains("invalid employee"))
                            {
                                model.Message = "No Record Available";
                                retVal = -1;
                                return Json(retVal);
                            }
                        }
                    }

                    // Filter rows by date range
                    DataRow[] filteredRows;
                    if (model.FromDate == DateTime.MinValue || model.ToDate == DateTime.MinValue)
                    {
                        filteredRows = dt.Select();
                    }
                    else
                    {
                        filteredRows = dt.Select($"DVALUT >= '{model.FromDate:yyyy-MM-dd}' AND DVALUT <= '{model.ToDate:yyyy-MM-dd}'");
                    }

                    // Create a new DataTable with the filtered rows
                    DataTable filteredData = new DataTable();
                    if (filteredRows.Length > 0)
                    {
                        filteredData = filteredRows.CopyToDataTable();
                    }

                    if (dt.Rows.Count > 0)
                    {
                        // Column indexes and headers
                        int[] columnList = {
                                dt.Columns["KONTRH"].Ordinal,
                                dt.Columns["NAME"].Ordinal,
                                dt.Columns["BU_SORT1"].Ordinal,
                                dt.Columns["ZUOND"].Ordinal,
                                dt.Columns["LOANPROPADD"].Ordinal,
                                dt.Columns["REFER"].Ordinal,
                                dt.Columns["DVALUT"].Ordinal,
                                dt.Columns["DVALUT1"].Ordinal,
                                dt.Columns["PKOND"].Ordinal,
                                dt.Columns["BBASIS"].Ordinal,
                                dt.Columns["MI"].Ordinal,
                                dt.Columns["MP"].Ordinal,
                                dt.Columns["EMI"].Ordinal,
                                dt.Columns["PEB"].Ordinal,
                                dt.Columns["INT_SUB"].Ordinal
                            };

                        string[] headerList = {
                            "Employee Code", "Name Of Employee", "Designation", "Bank Name",
                            "Loan Against Property", "Loan Sanction Period (in Months)", "Calculation Date",
                            "Installment", "Interest Rate", "Principal Beginning Balance", "Monthly Interest",
                            "Monthly Principal", "EMI", "Principal Ending Balance",
                            "50% Interest Subsidy Paid as per SBI Rate"
                        };

                        string fileName = "HLISusidyStatement_" + DateTime.Now.ToString("ddMMyyyy") + ".xls";

                        var fileBytes = _excelExport.ExportDetails(dt, columnList, headerList, ExportFormat.Excel, fileName);

                        TempData["EXCELFILE"] = JsonSerializer.Serialize(fileBytes);
                        TempData["FILENAME"] = fileName;
                        retVal = 1;
                        return Json(retVal);
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }

            return Json(retVal);
        }
        public ActionResult DownloadExcel_HLI1()
        {
            try
            {
                if (TempData["EXCELFILE"] == null)
                    return View();

                var json = TempData["EXCELFILE"].ToString();

                var fileObj = JsonSerializer.Deserialize<TempFileModel>(json);

                byte[] fileBytes = Convert.FromBase64String(fileObj.FileContents);

                return File(fileBytes, fileObj.ContentType, fileObj.FileDownloadName);
            }
            catch
            {
                return RedirectToAction("ErrorPage");
            }
        }
        public ActionResult DownloadExcel_HLI()
        {
            try
            {
                string str = _sessionService.Get<string>("EXCELFILE");
                if (string.IsNullOrEmpty(str))
                {
                    return View();
                }

                Response.ContentType = "application/vnd.ms-excel";
                return File(Encoding.UTF8.GetBytes(str), "application/vnd.ms-excel", "HLISusidyStatement.xls");
            }
            catch
            {
                return RedirectToAction("ErrorPage");
            }
        }
        public IActionResult OverstayReport()
        {
            var userId = _sessionService.Get<string>("userID");

            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            // Get employee from session
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");

            var model = new OverstayReportViewModel
            {
                FullName = _Employee_Details?.Employee_Name
            };
            DataTable objDt = _objleave.EmployeeDataGrid(userId);

            if (objDt.Rows.Count > 0)
            {
                model.EmpList = objDt.AsEnumerable()
                    .Select(r => new SelectListItem
                    {
                        Text = r["EMPNAME"].ToString(),
                        Value = r["ADEMPCODE"].ToString()
                    })
                    .ToList();

                // Ensure current user is in list, otherwise insert manually
                if (!model.EmpList.Any(e => e.Value == userId))
                {
                    model.EmpList.Insert(0, new SelectListItem
                    {
                        Text = model.FullName,
                        Value = userId
                    });
                }
            }
            else
            {
                // No employees, insert current user
                model.EmpList.Add(new SelectListItem
                {
                    Text = model.FullName,
                    Value = userId
                });
            }

            // Build year dropdown
            int currentYear = DateTime.Now.Year;
            int month = DateTime.Now.Month;

            int startYear = month <= 3 ? currentYear - 2 : currentYear - 1;
            for (int year = currentYear; year >= startYear; year--)
            {
                model.YearsList.Add(new SelectListItem { Value = year.ToString(), Text = year.ToString() });
            }

            // Current selections
            model.cmbyear = currentYear;
            model.cmbmonth = DateTime.Now.Month.ToString("D2");
            model.MonthList = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "--Select Month--" },
                new SelectListItem { Value = "01", Text = "January" },
                new SelectListItem { Value = "02", Text = "February" },
                new SelectListItem { Value = "03", Text = "March" },
                new SelectListItem { Value = "04", Text = "April" },
                new SelectListItem { Value = "05", Text = "May" },
                new SelectListItem { Value = "06", Text = "June" },
                new SelectListItem { Value = "07", Text = "July" },
                new SelectListItem { Value = "08", Text = "August" },
                new SelectListItem { Value = "09", Text = "September" },
                new SelectListItem { Value = "10", Text = "October" },
                new SelectListItem { Value = "11", Text = "November" },
                new SelectListItem { Value = "12", Text = "December" }
            }, "Value", "Text", DateTime.Now.Month.ToString("D2"));

            return View(model);
        }
        [HttpPost]
        public IActionResult OverstayReport([FromBody] OverstayReportViewModel model)
        {
            string date = $"{model.cmbyear}{model.cmbmonth:D2}01";
            DateTime currdate = model.CurrDate;
            DateTime validationdate = model.ValidationDate;
            DataTable dt1;

            if (model.ReportOption == "RadioButton1")
            {
                dt1 = _ePortalESS.GetOverstayReport(model.FullName, date, "x").Result;
            }
            else if (model.ReportOption == "RadioButton2")
            {
                dt1 = (currdate == validationdate)
                    ? _ePortalESS.GetOverstayReport(model.FullName, date, "x").Result
                    : _ePortalESS.GetOverstayReport(model.FullName, date, "").Result;
            }
            else
            {
                dt1 = new DataTable();
            }
            var reportList = new List<OverstayReportRecords>();
            foreach (DataRow row in dt1.Rows)
            {
                try
                {
                    string attDate = row[1]?.ToString();
                    string timeIn = row[4]?.ToString();
                    string timeOut = row[5]?.ToString();
                    string appOverstay = row[6]?.ToString().Trim();
                    string actOverstay = row[7]?.ToString().Trim();
                    string e_shift = row[3]?.ToString();
                    string AttendanceRemarks = row[8]?.ToString();
                    string Remarks = row[9]?.ToString();

                    reportList.Add(new OverstayReportRecords
                    {
                        Emp_Shift = string.IsNullOrEmpty(e_shift)? "": e_shift,
                        Emp_Prst_Remark = string.IsNullOrEmpty(AttendanceRemarks) ? "" : AttendanceRemarks,
                        Emp_Uabs_Remark = string.IsNullOrEmpty(Remarks) ? "" : Remarks,
                        Emp_Ldate = !string.IsNullOrEmpty(attDate)
                            ? $"{attDate.Substring(8, 2)}.{attDate.Substring(5, 2)}.{attDate.Substring(0, 4)}"
                            : "",
                        Emp_In_Time = !string.IsNullOrEmpty(timeIn)
                            ? $"{timeIn.Substring(0, 2)}:{timeIn.Substring(2, 2)}"
                            : "",
                        Emp_Out_Time = !string.IsNullOrEmpty(timeOut)
                            ? $"{timeOut.Substring(0, 2)}:{timeOut.Substring(2, 2)}"
                            : "",
                        Emp_App_Overstay = !string.IsNullOrEmpty(appOverstay)
                            ? appOverstay.Substring(0, 5)
                            : "",
                        Emp_Act_Overstay = !string.IsNullOrEmpty(actOverstay)
                            ? actOverstay.Substring(0, 5)
                            : ""
                    });
                }
                catch { }

                model.OverstayRecords = reportList;
            }
            return PartialView("_GetOverstayReport", model.OverstayRecords);
        }
        public IActionResult EmployeeBlockProcess()
        {
            var model = new EmployeeBlockProcessModel
            {
                ShowEmployeePanel = true
            };

            ViewBag.InvalidDataMessage = string.Empty;
            return View();
        }
        //protected void Button1_Click(object sender, EventArgs e)

        [HttpPost]
        public JsonResult EmployeeBlockProcess([FromBody] EmployeeBlockProcessModel request)
        {
            int flag = 0;
            try
            {
                string Status1 = "BLOCK";
                string status = _ePortalESS.GetEmployeeBlockDTL(request.EmpCode, Status1).Result;
                flag = status == "Employee code has been Blocked Successfully" ? 1 : 2;

                DateTime today = DateTime.Today;
                string result = objempid.InsertEmpblockDetail(
                    request.EmpCode,
                    today,
                    _sessionService.Get<string>("userID"),
                    status,
                    request.BlockReason
                );

                return Json(new { flag = flag });
            }
            catch (Exception ex)
            {
                return Json(new { flag = -1, error = ex.Message });
            }
        }

        /// <summary>
        ///    protected void Button3_Click(object sender, EventArgs e)
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        public IActionResult EmployeeBlockProcessDetail()
        {
            try
            {
                string uid = _sessionService.Get<string>("userID");
                DataTable dt = objempid.GetB_Emp_DTL(uid);

                var EmpBlockList = new List<EmpBlockProcessRecords>();
                foreach (DataRow row in dt.Rows)
                {
                    EmpBlockList.Add(new EmpBlockProcessRecords
                    {
                        ADEMPCODE = row["ADEMPCODE"].ToString(),
                        CREATED_ON = !string.IsNullOrEmpty(row["CREATED_ON"].ToString()) ? DateTime.ParseExact(row["CREATED_ON"].ToString(), "dd-MM-yy", CultureInfo.InvariantCulture) : new DateTime(),
                        BSTATUS = row["BSTATUS"].ToString(),
                        Reason = row["REASON"].ToString()
                    });
                }

                return PartialView("_GetEmployeeBlockProcess", EmpBlockList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        /// <summary>
        ///     protected void UploadButton_Click(object sender, EventArgs e)
        /// </summary>
        /// <returns></returns>
        // Added by aumento on 27112024 for SR82082
        [HttpPost]
        public IActionResult EmployeeBlockProcessUploadFile(IFormFile file)
        {
            string uid = _sessionService.Get<string>("userID");
            string fileName = uid + "_" + Path.GetFileName(file.FileName);
            string uploadPath = Path.Combine(serverpath.getFileUploadPath(), "Uploads", "EmpBlockUploadedFile");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            string filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // Read CSV data
            var result = ReadColumnDataFromCSV(filePath);
            var validData = result.Item1;
            var invalidData = result.Item2;
            var invalidFormatData = result.Item3;
            string AlertMessage = string.Empty;
            string AlertType = string.Empty;
            if (validData.Count == 0)
            {
                TempData["AlertMessage"] = AlertMessage = "Please Enter Employee Code In Downloaded Excel File.";
                TempData["AlertType"] = AlertType = "error";
                var response = new
                {
                    status = AlertType,
                    message = AlertMessage
                };
                return Json(response);
            }

            foreach (var entry in validData)
            {
                string empCode = entry.Key;
                string reason = entry.Value;

                string status = _ePortalESS.GetEmployeeBlockDTL(empCode, "BLOCK").Result;
                int flag = status == "Employee code has been Blocked Successfully" ? 1 : 2;

                DateTime today = DateTime.Today;
                string resultStr = objempid.InsertEmpblockDetail(empCode, today, uid, status, reason);

                if (flag == 1)
                {
                    TempData["AlertMessage"] = AlertMessage = $"Employee {empCode} Blocked Successfully.";
                    TempData["AlertType"] = AlertType = "success";
                }
                else
                {
                    TempData["AlertMessage"] = AlertMessage = $"Employee {empCode} Block Failed.";
                    TempData["AlertType"] = AlertType = "error";

                    invalidFormatData.Add(new KeyValuePair<string, string>(AlertType, AlertMessage));
                }
            }

            if (invalidFormatData.Count > 0 || invalidData.Count > 0)
            {
                // Build HTML table for invalid entries
                string htmlTable = "<table class='table table-bordered'><thead><tr><th>Sr. No.</th><th>Employee Code</th><th>Reason</th><th>Status</th></tr></thead><tbody>";
                int srNo = 1;

                foreach (var entry in invalidFormatData.Concat(invalidData))
                {
                    string employeeCode = entry.Key;
                    string reason = entry.Value;
                    string status = "";

                    if (invalidFormatData.Contains(entry))
                    {
                        if (!int.TryParse(employeeCode, out _))
                            status = "Invalid Format of employeeCode";
                    }
                    else if (string.IsNullOrWhiteSpace(employeeCode) && string.IsNullOrWhiteSpace(reason))
                        status = "Missing Employee Code & Reason";
                    else if (string.IsNullOrWhiteSpace(employeeCode))
                        status = "Missing Employee Code";
                    else if (string.IsNullOrWhiteSpace(reason))
                        status = "Missing Reason";

                    htmlTable += $"<tr style='background-color:#ffe6e6;'><td>{srNo++}</td><td>{employeeCode}</td><td>{reason}</td><td style='color:red;'>{status}</td></tr>";
                }

                htmlTable += "</tbody></table>";
                ViewBag.InvalidEntries = htmlTable;
                var response = new
                {
                    status = AlertType,
                    message = htmlTable
                };
                return Json(response);
            }
            else
            {
                ViewBag.InvalidEntries = "No invalid entries found.";
                TempData["AlertMessage"] = AlertMessage = "Successfully Blocked.";
                TempData["AlertType"] = AlertType = "success";
                var response = new
                {
                    status = AlertType,
                    message = AlertMessage
                };
                return Json(response);
            }
        }
        // Ended  by aumento on 27112024 for SR82082


        public (List<KeyValuePair<string, string>> ValidData,
                List<KeyValuePair<string, string>> InvalidData,
                List<KeyValuePair<string, string>> InvalidFormatData)
            ReadColumnDataFromCSV(string filePath)
        {
            var validData = new List<KeyValuePair<string, string>>();
            var invalidData = new List<KeyValuePair<string, string>>();
            var invalidFormatData = new List<KeyValuePair<string, string>>();

            try
            {
                string columnName1 = "Employee Code";
                string columnName2 = "Reason";

                using (var reader = new StreamReader(filePath))
                {
                    // Read header line
                    var headerLine = reader.ReadLine();
                    if (string.IsNullOrEmpty(headerLine))
                        throw new Exception("CSV file is empty or improperly formatted.");

                    var headers = headerLine.Split(',');
                    int columnIndex1 = Array.IndexOf(headers, columnName1);
                    int columnIndex2 = Array.IndexOf(headers, columnName2);

                    if (columnIndex1 == -1 || columnIndex2 == -1)
                        throw new Exception($"One or both columns '{columnName1}' or '{columnName2}' not found.");

                    // Read data lines
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        var fields = line.Split(',');

                        if (fields.Length > columnIndex1 && fields.Length > columnIndex2)
                        {
                            string employeeCode = fields[columnIndex1].Trim();
                            string reason = fields[columnIndex2].Trim();

                            bool isInteger = int.TryParse(employeeCode, out _);

                            if (string.IsNullOrWhiteSpace(employeeCode) || string.IsNullOrWhiteSpace(reason))
                            {
                                invalidData.Add(new KeyValuePair<string, string>(employeeCode, reason));
                            }
                            else if (!isInteger)
                            {
                                invalidFormatData.Add(new KeyValuePair<string, string>(employeeCode, reason));
                            }
                            else
                            {
                                validData.Add(new KeyValuePair<string, string>(employeeCode, reason));
                            }
                        }
                    }
                }

                return (validData, invalidData, invalidFormatData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
        public IActionResult EmployeeUnBlockProcess()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var model = new EmpUnBlockProcessModel
            {
                ShowEmployeePanel = true
            };

            ViewBag.InvalidDataMessage = string.Empty;
            return View();
        }
        //protected void Button1_Click(object sender, EventArgs e)

        [HttpPost]
        public JsonResult EmployeeUnBlockProcess([FromBody] EmpUnBlockProcessModel request)
        {

            int flag = 0;
            try
            {
                string Status1 = "UNBLOCK";
                string status = _ePortalESS.GetEmployeeUnblockDTL(request.EmpCode, Status1).Result;
                flag = status == "Employee code has been Unblocked Successfully" ? 1 : 2;

                DateTime today = DateTime.Today;
                string result = objempid.InsertEmpUnblockDetail(
                    request.EmpCode,
                    today,
                    _sessionService.Get<string>("userID"),
                    status,
                    request.UnBlockReason
                );

                return Json(new { flag = flag });
            }
            catch (Exception ex)
            {
                return Json(new { flag = -1, error = ex.Message });
            }
        }

        /// <summary>
        ///    protected void Button3_Click(object sender, EventArgs e)
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        public IActionResult EmployeeUnblockProcessDetail()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                string uid = _sessionService.Get<string>("userID");
                DataTable dt = objempid.GetUNB_Emp_DTL(uid);

                var EmpUnBlockList = new List<EmpUnBlockProcessModel.EmpUnBlockProcessRecords>();
                foreach (DataRow row in dt.Rows)
                {
                    EmpUnBlockList.Add(new EmpUnBlockProcessModel.EmpUnBlockProcessRecords
                    {
                        ADEMPCODE = row["ADEMPCODE"].ToString(),
                        CREATED_ON = !string.IsNullOrEmpty(row["CREATED_ON"].ToString()) ? DateTime.ParseExact(row["CREATED_ON"].ToString(), "dd-MM-yy", CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy") : "",
                        UNBSTATUS = row["UNBSTATUS"].ToString(),
                        Reason = row["REASON"].ToString()
                    });
                }

                return PartialView("_GetEmployeeUnblockProcess", EmpUnBlockList);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }
        /// <summary>
        ///     protected void UploadButton_Click(object sender, EventArgs e)
        /// </summary>
        /// <returns></returns>
        // Added by aumento on 27112024 for SR82082
        [HttpPost]
        public IActionResult EmployeeUnBlockProcessUploadFile(IFormFile file)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            string uid = _sessionService.Get<string>("userID");
            string fileName = uid + "_" + Path.GetFileName(file.FileName);
            string uploadPath = Path.Combine(serverpath.getFileUploadPath(), "Uploads", "EmpUnblockUploadedFile");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            string filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // Read CSV data
            var result = ReadColumnDataFromCSV(filePath);
            var validData = result.Item1;
            var invalidData = result.Item2;
            var invalidFormatData = result.Item3;
            string AlertType = string.Empty;
            string AlertMessage = string.Empty;
            if (validData.Count == 0)
            {
                TempData["AlertMessage"] = AlertMessage  = "Please Enter Employee Code In Downloaded Excel File.";
                TempData["AlertType"] = AlertType = "error";
                var response = new
                {
                    status = AlertMessage,
                    message = AlertMessage
                };
                return Json(response);
            }

            foreach (var entry in validData)
            {
                string empCode = entry.Key;
                string reason = entry.Value;

                string status = _ePortalESS.GetEmployeeBlockDTL(empCode, "UNBLOCK").Result;
                int flag = status == "Employee code has been Unblocked Successfully" ? 1 : 2;

                DateTime today = DateTime.Today;
                string resultStr = objempid.InsertEmpUnblockDetail(empCode, today, uid, status, reason);

                if (flag == 1)
                {
                    TempData["AlertMessage"] = AlertMessage = $"Employee {empCode} UnBlocked Successfully.";
                    TempData["AlertType"] = AlertType = "success";
                }
                else
                {
                    TempData["AlertMessage"] = AlertMessage = $"Employee {empCode} UnBlock Failed.";
                    TempData["AlertType"] = AlertType = "error";
                    invalidFormatData.Add(new KeyValuePair<string, string>(AlertType, AlertMessage));
                }
            }

            if (invalidFormatData.Count > 0 || invalidData.Count > 0)
            {
                // Build HTML table for invalid entries
                string htmlTable = "<table class='table table-bordered'><thead><tr><th>Sr. No.</th><th>Employee Code</th><th>Reason</th><th>Status</th></tr></thead><tbody>";
                int srNo = 1;

                foreach (var entry in invalidFormatData.Concat(invalidData))
                {
                    string employeeCode = entry.Key;
                    string reason = entry.Value;
                    string status = "";

                    if (invalidFormatData.Contains(entry))
                    {
                        if (!int.TryParse(employeeCode, out _))
                            status = "Invalid Format of employeeCode";
                    }
                    else if (string.IsNullOrWhiteSpace(employeeCode) && string.IsNullOrWhiteSpace(reason))
                        status = "Missing Employee Code & Reason";
                    else if (string.IsNullOrWhiteSpace(employeeCode))
                        status = "Missing Employee Code";
                    else if (string.IsNullOrWhiteSpace(reason))
                        status = "Missing Reason";

                    htmlTable += $"<tr style='background-color:#ffe6e6;'><td>{srNo++}</td><td>{employeeCode}</td><td>{reason}</td><td style='color:red;'>{status}</td></tr>";
                }

                htmlTable += "</tbody></table>";
                ViewBag.InvalidEntries = htmlTable;
                var response = new
                {
                    status = AlertType,
                    message = htmlTable
                };
                return Json(response);
            }
            else
            {
                ViewBag.InvalidEntries = "No invalid entries found.";
                TempData["AlertMessage"] = AlertMessage = "Successfully Unblocked.";
                TempData["AlertType"] = AlertType = "success";
                var response = new
                {
                    status = AlertType,
                    message = AlertMessage
                };
                return Json(response);
            }
        }
        // Ended  by aumento on 27112024 for SR82082

        public IActionResult Address()
        {
            var userId = _sessionService.Get<string>("userID");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Login");
            }
            var model = new EssViewModel
            {
                PersonalNumber = userId
                //SelectedAddressType = HttpContext.Session.GetString("Cmb_value")
            };
            LoadAddresses(model);
            return View(model);
            //return RedirectToAction("AddressLoad", model);
        }
        /// <summary>
        /// PERSONAL NUMBER TEXT BOX CHANGE EVENT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        [HttpPost]
        public async Task<IActionResult> AddressLoad(EssViewModel objAddress)
        {
            try
            {
                await LoadAddresses(objAddress);
            }
            catch
            {
                return RedirectToAction("ErrorPage");
            }

            return View("Address", objAddress);
        }


        private async Task LoadAddresses(EssViewModel model)
        {
            var dt = await _ePortalESS.GetAddressDetail(model.PersonalNumber, model.SelectedAddressType);

            foreach (DataRow row in dt.Rows)
            {
                var validFrom = row[5]?.ToString();
                var validEnd = row[4]?.ToString();

                model.Addresses.Add(new AddressRecords
                {
                    ValidBegin = !string.IsNullOrEmpty(validFrom)
                        ? DateTime.ParseExact(validFrom, "yyyyMMdd", null)
                        : DateTime.MinValue,

                    ValidEnd = !string.IsNullOrEmpty(validEnd)
                        ? DateTime.ParseExact(validEnd, "yyyyMMdd", null)
                        : (DateTime?)null,

                    StreetAndHouseNo = row["StreetAndHouseNo"]?.ToString(),
                    PostalCodeCity = row["PostalCode"]?.ToString(),
                    City = row["City"]?.ToString()
                });
            }

            model.FullName = await GetFullNameAsync(model.PersonalNumber);
        }
        /// <summary>
        /// GET FULL NAME IN STRING
        /// </summary>
        /// <returns></returns> 
        private async Task<string> GetFullNameAsync(string userid)
        {
            DataTable dt = await _ePortalESS.GetFullName(userid);

            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[dt.Rows.Count - 1];
                return $"{row["Nameofformofaddress"]} {row["Firstname"]} {row["Lastname"]}";
            }

            return string.Empty;
        }
        // Address Ended

        [HttpGet]
        public async Task<ActionResult<List<AddressdetailsViewModel>>> Addressdetails()
        {
            var userId = _sessionService.Get<string>("userID");
            var personnelNumber = HttpContext.Session.GetString("pernerno");
            var validEnd = HttpContext.Session.GetString("validend");
            var validBegin = HttpContext.Session.GetString("validbegin");
            var subtype = HttpContext.Session.GetString("Subtype");

            // Format SAP date strings (dd.MM.yyyy → yyyyMMdd)
            var high = DateTime.ParseExact(validEnd, "dd.MM.yyyy", null).ToString("yyyyMMdd");
            var low = DateTime.ParseExact(validBegin, "dd.MM.yyyy", null).ToString("yyyyMMdd");

            DataTable dt = await _ePortalESS.GetAddressDetail(userId, subtype ?? "");

            var iList = new List<AddressdetailsViewModel>();

            if (dt.Rows.Count > 0)
            {

                var fullName = await GetFullNameAsync(userId);

                iList = dt.AsEnumerable().Select(row => new AddressdetailsViewModel
                {
                    PersonalNumber = userId,
                    ValidFrom = !string.IsNullOrEmpty(row["ValidBegin"]?.ToString())
                                ? DateTime.ParseExact(row["ValidBegin"].ToString(), "yyyyMMdd", null)
                                : (DateTime?)null,
                    ValidTo = !string.IsNullOrEmpty(row["ValidEnd"]?.ToString())
                                ? DateTime.ParseExact(row["ValidEnd"].ToString(), "yyyyMMdd", null)
                                : (DateTime?)null,
                    Subtype = row["Subtype"]?.ToString(),
                    City = row["City"]?.ToString(),
                    Country = row["NameOfCountry"]?.ToString(),
                    District = row["District"]?.ToString(),
                    StreetAndHouseNo = $"{row["StreetAndHouseNo"]} ,{row["ScndAddressLine"]}",
                    PostalCode = row["PostalCodeCity"]?.ToString(),
                    TelephoneNo = row["TelephoneNumber"]?.ToString(),
                    Co = row["CoName"]?.ToString(),
                    State = row["NameOfState"]?.ToString(),
                    FullName = fullName
                }).ToList();
            }

            return View(iList);
        }


        public IActionResult Bankinformation()
        {
            return View();
        }

        public async Task<IActionResult> Bankinformationdetail()
        {

            try
            {
                string personnelNumber = HttpContext.Session.GetString("B_pernerno") ?? "";
                string high = HttpContext.Session.GetString("B_validend") ?? "";
                string low = HttpContext.Session.GetString("B_validbegin") ?? "";

                // SAP connection
                //RfcConfigParameters parms = _ePortalESS.sapconnection();
                //RfcDestination rfcDest = RfcDestinationManager.GetDestination(parms);
                //RfcRepository rfcRep = rfcDest.Repository;
                //IRfcFunction function = rfcRep.CreateFunction("BAPI_BANKDETAIL_GETDETAIL");

                //function.SetValue("EMPLOYEENUMBER", personnelNumber.Trim());
                //function.SetValue("SUBTYPE", "0");
                //function.SetValue("OBJECTID", "");
                //function.SetValue("LOCKINDICATOR", "");
                //function.SetValue("VALIDITYEND", high);
                //function.SetValue("VALIDITYBEGIN", low);
                //function.SetValue("RECORDNUMBER", "0");

                //RfcSessionManager.BeginContext(rfcDest);
                //function.Invoke(rfcDest);

                //var model = new BankDetailsViewModel
                //{
                //    AccountNo = function.GetString("ACCOUNTNO"),
                //    PayeePostalCodeCity = function.GetString("PAYEEPOSTALCODECITY"),
                //    Payee = function.GetString("PAYEE"),
                //    BankCountry = function.GetString("NAMEOFBANKCOUNTRY"),
                //    BankKey = function.GetString("BANKKEY"),
                //    PaymentMethod = function.GetString("PAYMENTMETHOD"),
                //    PaymentMethodName = function.GetString("NAMEOFPAYMENTMETHOD"),
                //    Currency = function.GetString("CURRENCY"),
                //    PayeeCity = function.GetString("PAYEECITY"),
                //    BankName = function.GetString("NAMEOFBANKKEY"),
                //    FullName = await GetFullName(personnelNumber)
                //};
                //return View(model);
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage");
            }

        }
        public async Task<string> GetFullName(string personnelNumber)
        {
            DataTable dt = _ePortalESS.GetFullName(personnelNumber).Result;
            if (dt.Rows.Count > 0)
            {
                string fullname = dt.Rows[dt.Rows.Count - 1]["Nameofformofaddress"].ToString() + " " + dt.Rows[dt.Rows.Count - 1]["Firstname"].ToString() + " " + dt.Rows[dt.Rows.Count - 1]["Lastname"].ToString();
                return fullname;
            }
            return string.Empty;
        }
        public IActionResult EssDetailStaff()
        {
            return View();
        }
        public IActionResult ESSLTAReport()
        {
            return View();
        }

    }
}
