using ePortal.Application.Contracts;
using ePortal.ViewModels;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using ePortal.Shared.Interface;
using System.Data;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ePortal.Shared.Services;
using ePortal.Shared;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    [SessionTimeout]
    public class POAPowerScopeController : Controller
    {

        private readonly IPOAPowerScopeService _POAPowerScopeService;
        private readonly ISessionService _sessionService;
        private readonly ILogger<PresidentDeskController> _logger;
        
        public POAPowerScopeController(IPOAPowerScopeService POAPowerScopeService, ISessionService sessionService, ILogger<PresidentDeskController> logger)
        {
            _POAPowerScopeService = POAPowerScopeService;
            _sessionService = sessionService;
            _logger = logger;            
        }


        public ActionResult Index()
        {
            return View();
        }
        public ActionResult POAPowerScopeCreate()
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            List<POAPowerScopeADPOWERMASTERViewModel> POAPowerList = new List<POAPowerScopeADPOWERMASTERViewModel>();
            ViewBag.data = new SelectList(POAPowerList, "POWERNAME", "POWERNAME");
            //List<POAPowerScopeADPOWERMASTERViewModel> list = new List<POAPowerScopeADPOWERMASTERViewModel>();
            //ViewData["Data"] = list;
            return View("POAPowerScopeCreate");

        }

        //public string AutocompleteSuggestionsForPOA(string term)
        //{
        //    string designation = "";
        //    try
        //    {

        //        TempData.Keep();
        //        designation = TempData["Designation"].ToString();
        //        if (designation == "-Select-")
        //            designation = "";
        //    }
        //    catch (Exception ex) { }
        //    //List<Employee_Details> portaluser = _ACRService.PortalAutocompleteSuggestions(term, designation);
        //    List<Employee_Details> portaluser = _POAPowerScopeService.PortalAutocompleteSuggestionsForPOA(term, designation);
        //    //List<PortalUser> data = new List<PortalUser>();
        //    int i = 0;
        //    List<string> list = new List<string>();
        //    foreach (var dataitem in portaluser)
        //    {
        //        list.Add(dataitem._ECode.ToString() + "-" + dataitem._EFirstName.ToString() + " " + dataitem._ELastName.ToString() + "");
        //    }
        //    System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        //    string sJSON = oSerializer.Serialize(list);
        //    return sJSON;
        //}

        [HttpGet]
        public async Task<IActionResult> AutocompleteSuggestionsForPOA(string term)
        {
            string designation = "";
            try
            {

                TempData.Keep();
                if(TempData.ContainsKey("Designation") && TempData["Designation"] != null)
                {
                    designation = TempData["Designation"].ToString();
                    if (designation == "-Select-")
                        designation = "";
                }
                
            }
            catch (Exception ex) { }
            
            List<Employee_Details> portaluser = _POAPowerScopeService.PortalAutocompleteSuggestionsForPOA(term, designation);
           
            // Format each guest's information into a string
            var result = portaluser.Select(dataItem =>
                $"{dataItem._ECode} - {dataItem._EFirstName}  {dataItem._ELastName} "
            ).ToList();

            // Return the result as a JSON response
            return Json(result);
        }


        //public string AutocompleteSuggestionsForPOAPower(string term)
        //{
        //    string designation = "";

        //    //List<Employee_Details> portaluser = _ACRService.PortalAutocompleteSuggestions(term, designation);
        //    List<POAPowerScopeADPOWERMASTERViewModel> portaluser = _POAPowerScopeService.PortalAutocompleteSuggestionsForPOAPower(term, designation);
        //    //List<PortalUser> data = new List<PortalUser>();
        //    int i = 0;
        //    List<string> list = new List<string>();
        //    foreach (var dataitem in portaluser)
        //    {
        //        list.Add(dataitem.POWERCODE.ToString() + "-" + dataitem.POWERNAME.ToString() + " ");
        //    }
        //    System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        //    string sJSON = oSerializer.Serialize(list);
        //    return sJSON;
        //}

        [HttpGet]
        public async Task<IActionResult> AutocompleteSuggestionsForPOAPower(string term)
        {
            string designation = "";

            // Retrieve the list of guests based on the search term
            List<POAPowerScopeADPOWERMASTERViewModel> portaluser = _POAPowerScopeService.PortalAutocompleteSuggestionsForPOAPower(term, designation);

            // Format each guest's information into a string
            var result = portaluser.Select(dataItem =>
                $"{dataItem.POWERCODE} - {dataItem.POWERNAME}"
            ).ToList();

            // Return the result as a JSON response
            return Json(result);
        }


        [HttpGet]
        //public ActionResult GetPower(string eCode)
        public JsonResult GetPower(string eCode)
        {

            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            //Employee_Details _Login_Employee_Details = (Employee_Details)Session["Employee"];

            int i = 0;
            List<POAPowerScopeADPOWERMASTERViewModel> POAPowerList = new List<POAPowerScopeADPOWERMASTERViewModel>();
            POAPowerList = _POAPowerScopeService.GetPower(Convert.ToInt64(eCode));
            List<string> list = new List<string>();
            foreach (var dataitem in POAPowerList)
            {
                list.Add(dataitem.POWERNAME.ToString());
            }
            //ViewData["Data"] = list;
            //return View("POAPowerScopeCreate");
            //return View(list);
            //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //string sJSON = oSerializer.Serialize(list);
            return Json(list);



            //return View(list);
            //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //string sJSON = oSerializer.Serialize(list);
            //return sJSON;




            //List<POAPowerScopeADPOWERMASTERViewModel> POAPowerList = new List<POAPowerScopeADPOWERMASTERViewModel>();
            //POAPowerList = _POAPowerScopeService.GetPower(Convert.ToInt64(eCode));

            //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //string sJSON = oSerializer.Serialize(POAPowerList);


            //return Json(sJSON, JsonRequestBehavior.AllowGet);



        }
        private FileViewModel GetUploadFile(IFormFile file, string ADEMPCODE)
        {
            try
            {
                FileViewModel FVM = new FileViewModel();
                if (file != null && file.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        FVM.File = ms.ToArray();
                    }

                    FVM.FileName = "POA_OF_" + ADEMPCODE + ".pdf";
                    FVM.FileContentType = file.ContentType;                   
                }
                return FVM;
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in GetUploadFile(), message:"+ ex.Message);
                throw ex;
            }
        }


        //public ActionResult SavePwerScopeDetail(string Ecode, string Data, HttpPostedFileBase FILE)
        [HttpPost]
        public ActionResult SavePwerScopeDetail(string Ecode, string Data, IFormFile FILES)
        {
            short retVal = 0;
            try
            {
                List<Root> obj = JsonConvert.DeserializeObject<List<Root>>(Data);
                List<ADPOWERSCOPEMASTERViewModel> AdPowerScopeList = new List<ADPOWERSCOPEMASTERViewModel>();


                if (!string.IsNullOrEmpty(Ecode))
                {
                    //FileViewModel _file = GetUploadFile(formData.FILE, formData.D_TYPE_ID, formData.DOC_ID);
                    FileViewModel _file = GetUploadFile(FILES, Ecode);
                    //var Folder = "POA_OF_" + Ecode;

                    foreach (var item in obj)
                    {

                        ADPOWERSCOPEMASTERViewModel POAEMP = new ADPOWERSCOPEMASTERViewModel();
                        POAEMP.SRNO = Convert.ToInt64(item.SrNo);
                        POAEMP.ADEMPCODE = Ecode;
                        POAEMP.ATTACHMENTFILENAME = _file.FileName == null ? null : _file.FileName;
                        POAEMP.FILE_CONTENTTYPE = _file.FileName == null ? null : _file.FileContentType;
                        POAEMP.FILE_BYTE = _file.FileName == null ? null : _file.File;
                        POAEMP.POWER = item.Power;
                        POAEMP.SCOPE = item.Scope;
                        POAEMP.REMARKS = item.Remarks;
                        POAEMP.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                        POAEMP.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));

                        AdPowerScopeList.Add(POAEMP);

                    }

                    Tuple<short, long> retVal_tuple = _POAPowerScopeService.SavePOADetail(AdPowerScopeList, Ecode);
                    retVal = retVal_tuple.Item1;

                    //string FileName = Server.MapPath("~/Uploads/POAPowerScope/" + Folder + "/" + "POA_OF_" + Ecode + ".pdf");                    
                    //string path = Server.MapPath("~/Uploads/POAPowerScope/" + Folder + "/" );

                    //string FileName = Server.MapPath("~/Uploads/POAPowerScope/POA_OF_" + Ecode + ".pdf");
                    //string path = Server.MapPath("~/Uploads/POAPowerScope/");

                   
                    string FileName = Path.Combine(serverpath.getFileUploadPath(), "POAPowerScope/POA_OF_" + Ecode + ".pdf");
                    string path = Path.Combine(serverpath.getFileUploadPath(), "POAPowerScope/");


                    if (retVal == 1)
                    {
                        //string FileName = Server.MapPath("~/Uploads/ITD/" + SiteName +"/"+ formData.ADEMPCODE + "_"+ formData.SUB_D_TYPE_ID + "_"+ formData.FINYEAR+".pdf");
                        //string path = Server.MapPath("~/Uploads/ITD/" + SiteName+"/");
                        if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                        if (obj.Count > 0)
                        {
                            foreach (ADPOWERSCOPEMASTERViewModel I in AdPowerScopeList)
                            {
                                if (I.FILE_BYTE != null)
                                {
                                    System.IO.File.WriteAllBytes(FileName, I.FILE_BYTE.ToArray());
                                }
                            }
                        }
                    }
                }



            }
            catch (Exception ex)
            {
                retVal = -1;
                // errmsg = ex.StackTrace.ToString();
            }
            return Json(new { res = retVal });
        }

        //public ActionResult SavePwerScopeDetail(string Ecode, string Data, ADPOWERSCOPEMASTERViewModel formData)
        //{
        //    short retVal = 0;
        //    try
        //    {

        //        List<Root> obj = JsonConvert.DeserializeObject<List<Root>>(Data);
        //        List<ADPOWERSCOPEMASTERViewModel> AdPowerScopeList = new List<ADPOWERSCOPEMASTERViewModel>();
        //        if (formData.FILE.ContentLength > 0 && !string.IsNullOrEmpty(formData.ADEMPCODE))
        //        {
        //            //FileViewModel _file = GetUploadFile(formData.FILE, formData.D_TYPE_ID, formData.DOC_ID);
        //            FileViewModel _file = GetUploadFile(formData.FILE, formData.ADEMPCODE);
        //            var Folder = "POA_OF_" + Convert.ToInt64(Session["UserId"].ToString());

        //            foreach (var item in obj)
        //            {

        //                ADPOWERSCOPEMASTERViewModel POAEMP = new ADPOWERSCOPEMASTERViewModel();
        //                POAEMP.SRNO = Convert.ToInt64(item.SrNo);
        //                POAEMP.ADEMPCODE = Ecode;
        //                POAEMP.ATTACHMENTFILENAME = formData.ADEMPCODE + "_" + System.IO.Path.GetExtension(_file.FileName);
        //                POAEMP.FILE_CONTENTTYPE = _file.FileContentType;
        //                POAEMP.FILE_BYTE = _file.File;
        //                POAEMP.POWER = item.Power;
        //                POAEMP.SCOPE = item.Scope;
        //                POAEMP.REMARKS = item.Remarks;
        //                POAEMP.ADDEDBY = Convert.ToInt64(Session["UserId"].ToString());
        //                POAEMP.MODIFIEDBY = Convert.ToInt64(Session["UserId"].ToString());

        //                AdPowerScopeList.Add(POAEMP);

        //            }

        //            Tuple<short, long> retVal_tuple = _POAPowerScopeService.SavePOADetail(AdPowerScopeList, Ecode);
        //            retVal = retVal_tuple.Item1;

        //            string FileName = Server.MapPath("~/Uploads/POAPowerScope/" + Folder + "/" + ".pdf");
        //            string path = Server.MapPath("~/Uploads/POAPowerScope/" + Folder + "/");
        //            if (retVal == 1)
        //            {
        //                //string FileName = Server.MapPath("~/Uploads/ITD/" + SiteName +"/"+ formData.ADEMPCODE + "_"+ formData.SUB_D_TYPE_ID + "_"+ formData.FINYEAR+".pdf");
        //                //string path = Server.MapPath("~/Uploads/ITD/" + SiteName+"/");
        //                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
        //                if (obj.Count > 0)
        //                {
        //                    foreach (ADPOWERSCOPEMASTERViewModel I in AdPowerScopeList)
        //                    {
        //                        if (I.FILE_BYTE != null)
        //                        {
        //                            System.IO.File.WriteAllBytes(FileName, I.FILE_BYTE.ToArray());
        //                        }
        //                    }
        //                }
        //            }
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        retVal = -1;
        //        // errmsg = ex.StackTrace.ToString();
        //    }
        //    return Json(new { res = retVal }, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult POAPowerScopeViewDetail(string eCode)
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            List<POAPowerScopeADPOWERMASTERViewModel> POAPowerList = new List<POAPowerScopeADPOWERMASTERViewModel>();
            ViewBag.data = new SelectList(POAPowerList, "POWERNAME", "POWERNAME");
            //List<POAPowerScopeADPOWERMASTERViewModel> list = new List<POAPowerScopeADPOWERMASTERViewModel>();
            //ViewData["Data"] = list;
            return View("POAPowerScopeViewDetail");

        }

        public JsonResult GetPowerScopeViewDetailByEmpCode(string eCode, string Power)
        {
            int i = 0;
            List<Root> POAPowerList = new List<Root>();
            POAPowerList = _POAPowerScopeService.GetPOADetailsByECode(eCode, Power);           
            return Json(POAPowerList);
        }

        [HttpPost]
        public JsonResult GetPowerScopeViewDetailAllData()
        {
            int i = 0;
            List<Root> POAPowerList = new List<Root>();
            POAPowerList = _POAPowerScopeService.GetPowerScopeViewDetailAllData();
            //List<string> list = new List<string>();
            //foreach (var dataitem in POAPowerList)
            //{
            //    list.Add(dataitem.POWERNAME.ToString());
            //}

            //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //string sJSON = oSerializer.Serialize(POAPowerList);
            return Json(POAPowerList);
        }

        public JsonResult GetPowerScopeViewDetailselectedByEmpCode(string eCode)
        {

            DataTable dt = new DataTable();
            List<Root> POAPowerList = new List<Root>();
            POAPowerList = _POAPowerScopeService.GetPOADetailsSelectedByECode(eCode);
            if (POAPowerList.Count() >= 1)
            {
                List<POAPowerScopeADPOWERMASTERViewModel> GetPowerList = new List<POAPowerScopeADPOWERMASTERViewModel>();

                GetPowerList = _POAPowerScopeService.GetPower(Int32.Parse(eCode));
                dt.Columns.Add("PowerCode", typeof(string));
                dt.Columns.Add("Power", typeof(string));
                dt.Columns.Add("Scope", typeof(string));
                dt.Columns.Add("Remarks", typeof(string));
                dt.Columns.Add("ATTACHMENTFILENAME", typeof(string));


                foreach (var element in GetPowerList)
                {
                    var row = dt.NewRow();
                    row["Power"] = element.POWERNAME;
                    row["PowerCode"] = element.POWERCODE;

                    var SS = _POAPowerScopeService.GetScopeByEmpCode(eCode, element.POWERCODE.ToString());
                    foreach (var i in SS)
                    {
                        row["Scope"] = i.Scope == null ? "Blank" : i.Scope;
                        row["Remarks"] = i.Remarks == null ? "Blank" : i.Remarks;
                        row["ATTACHMENTFILENAME"] = i.ATTACHMENTFILENAME == null ? "Blank" : i.ATTACHMENTFILENAME;

                    }

                    dt.Rows.Add(row);
                }

                List<Root> EmpList = new List<Root>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Root emp1 = new Root();
                    emp1.Power = dt.Rows[i]["Power"].ToString();
                    emp1.Scope = dt.Rows[i]["Scope"].ToString();
                    emp1.Remarks = dt.Rows[i]["Remarks"].ToString();
                    emp1.ATTACHMENTFILENAME = dt.Rows[i]["ATTACHMENTFILENAME"].ToString();
                    EmpList.Add(emp1);
                }


                //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                //string sJSON = oSerializer.Serialize(EmpList);
                return Json(EmpList);
            }

            else
            {

                List<POAPowerScopeADPOWERMASTERViewModel> POAPowerList1 = new List<POAPowerScopeADPOWERMASTERViewModel>();
                POAPowerList1 = _POAPowerScopeService.GetPower(Convert.ToInt64(eCode));
                dt.Columns.Add("PowerCode", typeof(string));
                dt.Columns.Add("Power", typeof(string));
                dt.Columns.Add("Scope", typeof(string));
                dt.Columns.Add("Remarks", typeof(string));
                dt.Columns.Add("ATTACHMENTFILENAME", typeof(string));

                foreach (var element in POAPowerList1)
                {
                    var row = dt.NewRow();
                    row["Power"] = element.POWERNAME;
                    row["PowerCode"] = element.POWERCODE;
                    row["Scope"] = "Blank";
                    row["Remarks"] = "";
                    row["ATTACHMENTFILENAME"] = "";
                    dt.Rows.Add(row);
                }

                List<Root> EmpList = new List<Root>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Root emp1 = new Root();
                    emp1.Power = dt.Rows[i]["Power"].ToString();
                    emp1.Scope = dt.Rows[i]["Scope"].ToString();
                    emp1.Remarks = dt.Rows[i]["Remarks"].ToString();
                    emp1.ATTACHMENTFILENAME = dt.Rows[i]["ATTACHMENTFILENAME"].ToString();
                    EmpList.Add(emp1);
                }


                //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                //string sJSON = oSerializer.Serialize(EmpList);
                return Json(EmpList);
            }


        }

        public ActionResult POAPowerEntryMaster()
        {
            //if (Session["UserId"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            return View("POAPowerEntryMaster");

        }

        public ActionResult SavePower(string PCODE, string PNAME)
        {
            short retVal = 0;
            try
            {
                if (!string.IsNullOrEmpty(PCODE) && !string.IsNullOrEmpty(PNAME))
                {
                    Tuple<short, long> retVal_tuple = _POAPowerScopeService.SavePower(PCODE, PNAME);
                    retVal = retVal_tuple.Item1;
                }
                else
                {
                    retVal = -1;
                }

            }
            catch (Exception ex)
            {
                retVal = -1;
                // errmsg = ex.StackTrace.ToString();
            }
            return Json(new { res = retVal });
        }

        [HttpPost]
        public JsonResult GetDoctTypeMasterData()
        {
            int i = 0;
            List<POAPowerScopeADPOWERMASTERViewModel> POAPowerList = new List<POAPowerScopeADPOWERMASTERViewModel>();
            POAPowerList = _POAPowerScopeService.GetPowerMasterData();
            //List<string> list = new List<string>();
            //foreach (var dataitem in POAPowerList)
            //{
            //    list.Add(dataitem.POWERNAME.ToString());
            //}

            //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //string sJSON = oSerializer.Serialize(POAPowerList);
            return Json(POAPowerList);
        }

        [HttpDelete]
        public ActionResult DeleteAttachment([FromBody] AttachmentModel model)
        {
            short retVal = 0;
            List<POAPowerScopeADPOWERMASTERViewModel> IncTaxDecList = new List<POAPowerScopeADPOWERMASTERViewModel>();
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                Tuple<short, List<POAPowerScopeADPOWERMASTERViewModel>> _ret_tuple = _POAPowerScopeService.DeleteAttachment(model.POWERCODE, model.POWERNAME, Convert.ToInt64(model.SRNO));
                retVal = _ret_tuple.Item1;

            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(new { res = retVal, attachmentList = IncTaxDecList });
        }

        [HttpPost]
        public ActionResult ExportToExcelUser(string eCode, string Power)
        {
            short retVal = 0;
            try
            {
                //Employee_Details employeeDetails = (Employee_Details)Session["Employee"];
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");

                //SearchIndentUser _headerObj = _benService.GetBenevolentMST();
                //List<BENEVOLENT_MST> _headerObj= _benService.GetBenevolentMST();
                List<Root> _headerObj = _POAPowerScopeService.GetPOADetailsByECode(eCode, Power);
                //if (reportType == 1)
                //{
                //    _headerList = (employeeDetails._FnDesigId == null ? _headerList.Where(h => h.ADEMPCODE == employeeDetails._ECode).ToList() : _headerList);
                //}
                string str = this.excelHtmlUser(_headerObj);
                TempData.Remove("EXCELFILE");
                TempData["EXCELFILE"] = str;
                retVal = 1;
            }
            catch (Exception ex)
            {
                retVal = (short)-1;
            }
            return Json(retVal);
        }

        public string excelHtmlUser(List<Root> _headerList)
        {
            //Employee_Details employeeDetails = (Employee_Details)this.Session["Employee"];
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");

            string str = "";
            if (_headerList.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 11pt;font-family:Arial'>");
                stringBuilder.Append("<tr style='background-color: lightgray;'>");
                stringBuilder.Append("<th style='width:4%;text-align:center;border: 1px solid;'>S.No.</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Employee Code </th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Power </th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Scope </th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Remarks</th>");
                stringBuilder.Append("</tr>");
                int srNo = 1;
                foreach (var indent in _headerList)
                {
                    stringBuilder.Append("<tr>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + srNo++ + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + indent.ADEMPCODE_ + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + indent.Power + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + indent.Scope + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + indent.Remarks + "</td>");
                    stringBuilder.Append("</tr>");
                }
                stringBuilder.Append("</table>");
                str = stringBuilder.ToString();
            }
            return str;
        }

      
        //public ActionResult DownloadExcelUser()
        //{
        //    try
        //    {
        //        if (TempData["EXCELFILE"] == null)
        //        {
        //            return View();
        //        }
        //        string str = (string)TempData["EXCELFILE"];
        //        HttpContext.Response.AddHeader("content-disposition", "attachment; filename=POAPowerScopeViewDetailReport.xls");
        //        Response.ContentType = "application/vnd.ms-excel";
        //        return File(Encoding.UTF8.GetBytes(str.ToString()), "application/vnd.ms-excel");
        //    }
        //    catch (Exception ex)
        //    {
        //        return RedirectToAction("ErrorPage");
        //    }
        //}


        public IActionResult DownloadExcelUser()
        {
            try
            {
                if (TempData["EXCELFILE"] == null)
                {
                    return View();
                }

                string str = TempData["EXCELFILE"].ToString();
                byte[] fileBytes = Encoding.UTF8.GetBytes(str);

                return File(fileBytes, "application/vnd.ms-excel", "POAPowerScopeViewDetailReport.xls");
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage");
            }
        }


        //private FileViewModel GetUploadFile(HttpPostedFileBase file, string DocType, string PONo)
        //{
        //    try
        //    {
        //        FileViewModel FVM = new FileViewModel();
        //        if (file != null && file.ContentLength > 0)
        //        {
        //            byte[] bytes;
        //            using (BinaryReader br = new BinaryReader(file.InputStream))
        //            {
        //                bytes = br.ReadBytes(file.ContentLength);
        //            }
        //            string _FileName = file.FileName.Substring(file.FileName.LastIndexOf("\\") + 1);
        //            FVM.FileContentType = MimeMapping.GetMimeMapping(_FileName);
        //            //FVM.FileName = DocType + "_" + PONo + ".pdf";
        //            FVM.FileName = DocType + "_" + PONo + "_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
        //            FVM.File = bytes;
        //        }
        //        return FVM;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        [HttpPost]
        public IActionResult UploadPOA(IFormFile FILE, string ADEMPCODE)
        {
            short retVal = 0;
            try
            {               
                ADPOWERSCOPEMASTERViewModel POAList = new ADPOWERSCOPEMASTERViewModel();
                
                if (FILE!=null && FILE.Length > 0 && !string.IsNullOrEmpty(ADEMPCODE))
                {
                    FileViewModel _file = GetUploadFile(FILE, ADEMPCODE);
                    POAList.ATTACHMENTFILENAME = _file.FileName;
                    POAList.FILE_CONTENTTYPE = _file.FileContentType;
                    POAList.FILE_BYTE = _file.File;
                    TempData["POA_ATTACHMENT"] = POAList;
                    retVal = 1;
                    return Json(new { success = true, message = "Upload successful" });
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
                return Json(new { success = false, message = "Server Error" });
            }
            return Json(new { success = false, message = "Invalid input" });
        }

        [HttpPost]
        public JsonResult GetPowerMasterData()
        {
            int i = 0;
            List<POAPowerScopeADPOWERMASTERViewModel> POAPowerList = new List<POAPowerScopeADPOWERMASTERViewModel>();
            POAPowerList = _POAPowerScopeService.GetPowerMasterData();
            //List<string> list = new List<string>();
            //foreach (var dataitem in POAPowerList)
            //{
            //    list.Add(dataitem.POWERNAME.ToString());
            //}

            //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //string sJSON = oSerializer.Serialize(POAPowerList);
            return Json(POAPowerList);


            //int i = 0;
            //List<Root> POAPowerList = new List<Root>();
            //POAPowerList = _POAPowerScopeService.GetPowerScopeViewDetailAllData();
            ////List<string> list = new List<string>();
            ////foreach (var dataitem in POAPowerList)
            ////{
            ////    list.Add(dataitem.POWERNAME.ToString());
            ////}

            //System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //string sJSON = oSerializer.Serialize(POAPowerList);
            //return Json(sJSON, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPDF(string fileName, string ADEMPCODE)
        {
            try
            {
                //string adempcode = fileName.Substring(0, fileName.IndexOf("_"));
                //adempcode = "POA_OF_" + adempcode;
                //string file_path = "../../../Uploads/POAPowerScope/POA_OF_" + ADEMPCODE + "/"; //// Server.MapPath("~/Uploads/ACR/");
                string file_path = "../../../Uploads/POAPowerScope/"; //// Server.MapPath("~/Uploads/ACR/");
                string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"488px\"></object>";
                string _path = string.Format(embed, file_path + fileName);
                return Json(new
                {
                    FILEPATH = _path,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    FILEPATH = "",
                });
            }
        }



    }
}
