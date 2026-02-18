using ePortal.Application.Contracts;
using ePortal.Persistence;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using ePortal.WebUI.Filters;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    public class BenevolentController : Controller
    {
        private readonly ILogger<BenevolentController> _logger;
        private readonly IConfiguration _settings;
        private readonly ISessionService _sessionService;
        private readonly IBenevolent _benService;
        //private readonly ISearchEmp _SearchEmpDAL;
        public BenevolentController(ILogger<HomeController> logger, IConfiguration settings, ISessionService sessionService, IBenevolent benService)
        {
            _settings = settings;
            _sessionService = sessionService;

            _benService = benService;
            //_SearchEmpDAL = SearchEmpDAL;
        }
        public ActionResult Index()
        {
            List<BENEVOLENT_MST> data = _benService.GetBenevolentMST();
            return View();
        }
        [HttpGet]
        public ActionResult BenevolentMSTList()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            short a = _benService.SentMailToVendor();
            return View();
        }

        [HttpPost]
        public ActionResult BenevolentMSTList([FromBody] SearchBenevolent SI)
        {
            try
            {

                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                List<BenevolentViewModel> Data = _benService.GetBenevolentMSTList(SI);
                return PartialView("_GetBenevolentMSTList", Data);

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex,"BenevolentMSTList : ");
                throw Ex;
            }
        }
        public ActionResult AddBenevolentMST()
        {
            return View();
        }
        public ActionResult EditBenevolentMST(long id)
        {
            try
            {

                BENEVOLENT_MST data = _benService.GetEmpBenevolentMST(id);
                return View(data);

            }
            catch (Exception Ex)
            {
                _logger.LogError("EditBenevolentMST : " + Ex.ToString());
                throw Ex;
            }
        }

        [HttpPost]
        public JsonResult AddBenevolentMS([FromBody] BENEVOLENT_MST mst)
        {
            try
            {
                mst.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                short sh = _benService.AddBenevolentMST(mst);
                //List<BENEVOLENT_MST> data = _benService.GetBenevolentMST();
                return Json(sh);
            }
            catch (Exception Ex)
            {
                _logger.LogError("AddBenevolentMS : " + Ex.ToString());
                throw Ex;
            }
        }

        [HttpPost]
        public JsonResult EditBenevolentMS([FromBody] BENEVOLENT_MST mst)
        {
            try
            {
                mst.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                short sh = _benService.EditBenevolentMST(mst);
                return Json(sh);

            }
            catch (Exception Ex)
            {
                _logger.LogError("AddBenevolentMS : " + Ex.ToString());
                throw Ex;
            }
        }

        public JsonResult GetEmpCode(long id)
        {
            try
            {
                //long id=8607;
                int data = _benService.GetEmpCode(id);
                //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                //string sJSON = oSerializer.Serialize(data);
                return Json(data);
            }
            catch (Exception Ex)
            {
                _logger.LogError("GetEmpCode : " + Ex.ToString());
                throw Ex;
            }
        }

        public JsonResult GetEmpDetail(long id)
        {
            try
            {
                //long id=8607;
                BenevolentViewModel data = _benService.GetEmpDetail(id);
                //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                //string sJSON = oSerializer.Serialize(data); Commented By Saheb
                string sJSON = JsonConvert.SerializeObject(data);
                return Json(sJSON);
            }
            catch (Exception Ex)
            {
                _logger.LogError("GetEmpDetail : " + Ex.ToString());
                throw Ex;
            }
        }

        public string AutocompleteSuggestions(string term)
        {
            try
            {
                string designation = "";

                try
                {

                    TempData.Keep();
                    designation = TempData["Designation"].ToString();
                    if (designation == "-Select-")
                        designation = "";
                }
                catch (Exception ex) { }
                List<Employee_Details> portaluser = _benService.PortalAutocompleteSuggestions(term, designation);
                int i = 0;
                List<string> list = new List<string>();
                foreach (var dataitem in portaluser)
                {

                    list.Add(dataitem._ECode.ToString() + "-" + dataitem._EFirstName.ToString() + " " + dataitem._ELastName.ToString() + "");
                }
                //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                //string sJSON = oSerializer.Serialize(list); Commented By Saheb
                string sJSON = JsonConvert.SerializeObject(list);
                return sJSON;
            }
            catch (Exception Ex)
            {
                _logger.LogError("AutocompleteSuggestions : " + Ex.ToString());
                throw Ex;
            }
        }

        [HttpGet]
        public ActionResult ContributionBenMSTList()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        [HttpPost]
        public ActionResult ContributionBenMSTList([FromBody] SearchBenevolent SI)
        {
            try
            {

                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                SI.UserId = Convert.ToInt64(_sessionService.Get<string>("userID"));

                List<BenevolentViewModel> Data = _benService.ContributionBenMSTList(SI);
                //List<BenevolentViewModel> Data = _benService.GetContributionReport();
                return PartialView("_GetContributionBenMSTList", Data);
            }
            catch (Exception Ex)
            {
                _logger.LogError("ContributionBenMSTList : " + Ex.ToString());
                throw Ex;
            }
        }

        public ActionResult AddContribution(long id)
        {
            ViewBag.id = id;
            return View();
        }

        public JsonResult DemiseEmpDetail(long id)
        {
            try
            {
                BENEVOLENT_MST mst = _benService.GetEmpBenevolentMST(id);
                BenevolentViewModel vm = _benService.GetEmpDetail(mst.EMPLOYEECODE);


                object data = new { t1 = mst, t2 = vm };
                //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer(); Commented by Saheb
                string js = JsonConvert.SerializeObject(data);
                //string sJSON = oSerializer.Serialize(data);
                //return Json(js, JsonRequestBehavior.AllowGet);
                return Json(js);
            }
            catch (Exception Ex)
            {
                _logger.LogError("DemiseEmpDetail : " + Ex.ToString());
                throw Ex;
            }
        }

        [HttpPost]
        public JsonResult AddContributionData([FromBody] BENEVOLENT_DT dt)
        {
            try
            {
                dt.CONSENTBYEMPLOYEE = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                dt.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                short sh = _benService.AddContribution(dt);
                //return Json(sh, JsonRequestBehavior.AllowGet);
                return Json(sh);
            }
            catch (Exception Ex)
            {
                _logger.LogError("AddContributionData : " + Ex.ToString());
                throw Ex;
            }
        }

        [HttpGet]
        public ActionResult ContributionReport()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        [HttpPost]
        public ActionResult ContributionReport([FromBody] SearchBenevolent SI)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                //SI.emp = Convert.ToInt64(Session["UserId"]);
                List<BenevolentViewModel> Data = _benService.GetContributionReport(SI);
                //List<BenevolentViewModel> Data = _benService.GetContributionReport();
                return PartialView("_ContributionReport", Data);
            }
            catch (Exception Ex)
            {
                _logger.LogError("ContributionReport : " + Ex.ToString());
                throw Ex;
            }
        }

        public ActionResult GetDemiseEmployeeDetail(long id)
        {
            try
            {
                BenevolentViewModel data = _benService.GetDemiseEmployeeDetail(id);

                //SearchEmp objDetail = new SearchEmp(); Commented By Saheb

                string strPhotoServer = string.Empty;
                string strTransid = data.EMPLOYEECODE.ToString();
                if (strTransid.Length == 2)
                {
                    strTransid = "0" + "0" + strTransid;
                }
                else if (strTransid.Length == 3)
                {
                    strTransid = "0" + strTransid;
                }
                string strPhotoPath = serverpath.getPhotoPath();

                string strImagePath = string.Empty;

                strImagePath = strPhotoPath + strTransid + "." + "jpg";

                if (!System.IO.File.Exists(strImagePath))
                {
                    strImagePath = strPhotoPath + "0264" + "." + "jpg";
                }

                string img = strImagePath;
                Bitmap b = new Bitmap(img);
                Iresize(b, 100, 150);
                b = null;

                strPhotoServer = serverpath.getServerPath();

                if (System.IO.File.Exists(strPhotoPath + strTransid + "." + "jpg"))
                {
                    ViewBag.img = @"" + strPhotoServer + "Uploads/Photographs/" + strTransid + "." + "jpg";
                }
                else
                {
                    //ViewBag.img = strImagePath;
                    ViewBag.img = @"" + strPhotoServer + "Uploads/Photographs/0264.jpg";
                }
                return View(data);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex,"GetDemiseEmployeeDetail");
                throw Ex;
            }
        }
        public ActionResult GetContributionReportDetail(long id)
        {
            try
            {
                BenevolentViewModel data = _benService.GetContributionReportDetail(id);

                //SearchEmp objDetail = new SearchEmp(); Commented By Saheb

                string strPhotoServer = string.Empty;
                string strTransid = data.EMPLOYEECODE.ToString();
                if (strTransid.Length == 2)
                {
                    strTransid = "0" + "0" + strTransid;
                }
                else if (strTransid.Length == 3)
                {
                    strTransid = "0" + strTransid;
                }
                string strPhotoPath = serverpath.getPhotoPath();

                string strImagePath = string.Empty;

                strImagePath = strPhotoPath + strTransid + "." + "jpg";

                if (!System.IO.File.Exists(strImagePath))
                {
                    strImagePath = strPhotoPath + "0264" + "." + "jpg";
                }

                string img = strImagePath;
                Bitmap b = new Bitmap(img);
                Iresize(b, 100, 150);
                b = null;

                strPhotoServer = serverpath.getServerPath();

                if (System.IO.File.Exists(strPhotoPath + strTransid + "." + "jpg"))
                {
                    ViewBag.img = @"" + strPhotoServer + "Uploads/Photographs/" + strTransid + "." + "jpg";
                }
                else
                {
                    //ViewBag.img = strImagePath;
                    ViewBag.img = @"" + strPhotoServer + "Uploads/Photographs/0264.jpg";
                }
                return View(data);
            }
            catch (Exception Ex)
            {
                _logger.LogError("GetContributionReportDetail : " + Ex.ToString());
                throw Ex;
            }
        }

        [HttpPost]
        public ActionResult ExportToExcelUser([FromBody] SearchBenevolent SI)
        {

            short retVal = 0;
            try
            {
                //Employee_Details employeeDetails = (Employee_Details)Session["Employee"];
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                //SearchIndentUser _headerObj = _benService.GetBenevolentMST();
                //List<BENEVOLENT_MST> _headerObj= _benService.GetBenevolentMST();
                List<BenevolentViewModel> _headerObj = _benService.GetContributionReport(SI);
                //if (reportType == 1)
                //{
                //    _headerList = (employeeDetails._FnDesigId == null ? _headerList.Where(h => h.ADEMPCODE == employeeDetails._ECode).ToList() : _headerList);
                //}
                string str = this.excelHtmlUser(_headerObj);
                //TempData["EXCELFILE"] = str;

                string filePath = Path.Combine(Path.GetTempPath(), "ContributionReport.xls");//Added By Saheb
                System.IO.File.WriteAllText(filePath, str, Encoding.UTF8);
                TempData.Remove("EXCELFILE");
                
                TempData["EXCELFILE"] = filePath;
                retVal = 1;
            }
            catch (Exception ex)
            {
                _logger.LogError("ExportToExcelUser : " + ex);
                retVal = (short)-1;
            }
            return Json(retVal);
        }

        public ActionResult DownloadExcelUser()
        {
            //Added By Saheb
            try
            {
                if (TempData["EXCELFILE"] == null)
                {
                    return View();
                }

                string filePath = TempData["EXCELFILE"].ToString();

                if (!System.IO.File.Exists(filePath))
                {
                    return RedirectToAction("ErrorPage");
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

                return File(fileBytes, "application/vnd.ms-excel", "ContributionReport.xls");
            }
            catch (Exception ex)
            {
                _logger.LogError("DownloadExcelUser : " + ex);
                return RedirectToAction("ErrorPage");
            }
            //Commented By Saheb
            //try
            //{
            //    if (TempData["EXCELFILE"] == null)
            //    {
            //        return View();
            //    }
            //    string str = (string)TempData["EXCELFILE"];
            //    //HttpContext.Response.AddHeader("content-disposition", "attachment; filename=ContributionReport.xls"); 
            //    Response.Headers.Append("Content-Disposition", "attachment; filename=ContributionReport.xls");
            //    Response.ContentType = "application/vnd.ms-excel";
            //    return File(Encoding.UTF8.GetBytes(str.ToString()), "application/vnd.ms-excel");
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError("DownloadExcelUser : " + ex);
            //    return RedirectToAction("ErrorPage");
            //}

        }

        public string excelHtmlUser(List<BenevolentViewModel> _headerList)
        {
            try
            {
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                string str = "";
                if (_headerList.Count > 0)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 11pt;font-family:Arial'>");
                    stringBuilder.Append("<tr style='background-color: lightgray;'>");
                    stringBuilder.Append("<th style='width:4%;text-align:center;border: 1px solid;'>S.No.</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Employee Code (Demise)</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Employee Name (Demise) </th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Employee Demise Date </th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Employee Code</th>");
                    stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Employee Name</th>");
                    stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Operation</th>");
                    stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Contribution Period</th>");
                    stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Amount</th>");
                    stringBuilder.Append("</tr>");
                    int srNo = 1;
                    foreach (var indent in _headerList)
                    {

                        stringBuilder.Append("<tr>");
                        stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + srNo++ + "</td>");
                        stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + indent.EMPLOYEECODE + "</td>");
                        stringBuilder.Append("<td style='border:1px solid;'>" + indent.EmpName + "</td>");
                        stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + indent.DEMISEDATE.ToString("dd-MMMM-yyyy") + "</td>");

                        stringBuilder.Append("<td style='border:1px solid;'>" + indent.CONSENTBYEMPLOYEE + "</td>");
                        stringBuilder.Append("<td style='border:1px solid;'>" + indent.CONSENTBYEmpName + "</td>");
                        stringBuilder.Append("<td style='border:1px solid;'>" + indent.Opration + "</td>");
                        stringBuilder.Append("<td style='border:1px solid;'>" + indent.ContributionPeriod + "</td>");
                        stringBuilder.Append("<td style='border:1px solid;'>" + indent.AMOUNT + "</td>");
                        stringBuilder.Append("</tr>");
                    }
                    stringBuilder.Append("</table>");
                    str = stringBuilder.ToString();
                }
                return str;
            }
            catch (Exception Ex)
            {
                _logger.LogError("excelHtmlUser : " + Ex.ToString());
                throw Ex;
            }
        }

        private void Iresize(System.Drawing.Image img, int MaxWidth, int MaxHeight)
        {
            double widthRatio = (double)img.Width / (double)MaxWidth;
            double heightRatio = (double)img.Height / (double)MaxHeight;
            double ratio = Math.Max(widthRatio, heightRatio);
            int newWidth = (int)(img.Width / heightRatio);
            int newHeight = (int)(img.Height / ratio);
        }
    }
}
