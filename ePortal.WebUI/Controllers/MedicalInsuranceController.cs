using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.Shared.Services;
using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;


namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    public class MedicalInsuranceController : Controller
    {
        
        private readonly IMedicalInsuranceService _MedicalInsuranceService;
        private readonly ISessionService _sessionService;
        private readonly ILogger<MedicalInsuranceController> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IAppConfigurationService _configuration;
        private readonly IEportalESS objess;

        public MedicalInsuranceController(ILogger<MedicalInsuranceController> logger, IMedicalInsuranceService MedicalInsuranceService, ISessionService sessionService, IWebHostEnvironment env, IAppConfigurationService appConfiguration, IEportalESS _objess)
        {
            _logger = logger;
            _MedicalInsuranceService = MedicalInsuranceService;
            _sessionService = sessionService;
            _env = env;
            _configuration = appConfiguration;
            objess=_objess;
        }
        #region Save/Update/Detail
        // GET: MedicalInsurance
        [HttpGet]
        public ActionResult Dashboard()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                //Employee_Details emp = (Employee_Details)Session["Employee"];
                Employee_Details emp = _sessionService.Get<Employee_Details>("Employee");
                ViewBag.UserId = _sessionService.Get<string>("userID");
                ViewBag.IsExist = _MedicalInsuranceService.GetIsExistNewJoinee(Convert.ToInt64(_sessionService.Get<string>("userID")));
                PolicyAndPlantMappingViewModel PPVM = _MedicalInsuranceService.GetDashboardInfo(Convert.ToInt64(emp.Designation_Id), Convert.ToInt64(_sessionService.Get<string>("userID")));
                ViewBag.CompanyPaid_Cnt = PPVM.CompanyPaid_Cnt;
                ViewBag.AssociatedPaid_Cnt = PPVM.AssociatedPaid_Cnt;
                ViewBag.TotalCompanyPaid = PPVM.TotalCompanyPaid;
                ViewBag.TotalAssociatedPaid = PPVM.TotalAssociatedPaid;
                ViewBag.PlantLocation = PPVM.Plant;
                ViewBag.Renewal_StartDate = PPVM.Renewal_StartDate;
                ViewBag.Renewal_EndDate = PPVM.Renewal_EndDate;
                ViewBag.ApprovalStatus = PPVM.ApprovalStatus;
                ViewBag.IsRenewal = _MedicalInsuranceService.GetIsExistRenewal(PPVM.PlantId, 1);
                MedicalInsuranceViewModel MIVM = _MedicalInsuranceService.UserPendingRequest(Convert.ToInt64(_sessionService.Get<string>("userID")));
                return View(MIVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw (ex);
               
                
            }
        }

        // GET: MedicalInsurance/RequestDetails/5
        public ActionResult RequestDetails(int id)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return View("Details", _MedicalInsuranceService.GetPolicyDetails(id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw (ex);                
            }
        }

        // GET: MedicalInsurance/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                return View(_MedicalInsuranceService.GetActivePolicyDetails(id));
            }
            catch (Exception ex)
            {
               _logger.LogError(ex, ex.Message);
                throw (ex);
            }
        }

        public async Task<MedicalInsuranceViewModel> GetEmployeeDetailBySAP()
        {
            try {
                //var strPhotoServer = serverpath.getServerPath();
                MedicalInsuranceViewModel MIVM = new MedicalInsuranceViewModel();
                Employee_Details obj = _sessionService.Get<Employee_Details>("Employee");
                // EportalESS objess = new EportalESS();//SAP Connection Object
                DataTable dt = new DataTable();
                MIVM.EmpCode = Convert.ToInt64(obj._ECode);
                MIVM.Name = obj.Employee_Name;
                MIVM.DOB = obj.DOB;
                MIVM.Gender = obj.Gender == "M" ? "Male" : "Female";
                MIVM.SelfPhoto = "~/Uploads/Photographs/" + Convert.ToInt64(obj._ECode) + "." + "jpg";
                //string strPhotoPath = serverpath.getPhotoPath();
                //var img = @"" + strPhotoServer + "Uploads/Photographs/" + Convert.ToInt64(obj._ECode) + "." + "jpg";
                dt = await objess.GetFamilylist(_sessionService.Get<string>("userID"), "");
                List<DependentDetailViewModel> DDVMList = new List<DependentDetailViewModel>();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        DDVMList.Add(new DependentDetailViewModel
                        {
                            DepName = dt.Rows[i][8].ToString().ToUpper() + " " + dt.Rows[i][9].ToString().ToUpper(),
                            DepGender = dt.Rows[i][26].ToString(),
                            DepRelation = dt.Rows[i][32].ToString(),
                            DepDOB = Convert.ToDateTime(dt.Rows[i][19]).ToString("dd-MMM-yyyy"),
                        });
                    }
                   MIVM.SAPDependentList = DDVMList;
                }
                return MIVM;

            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
                RedirectToAction("Home", "Home");
            }
            return null;
        }

        // GET: MedicalInsurance/Create
        [HttpGet]
        public async Task<ActionResult> NewJoinee()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                MedicalInsuranceViewModel MIVM = new MedicalInsuranceViewModel();
                Employee_Details emp = _sessionService.Get<Employee_Details>("Employee");
                MIVM =await GetEmployeeDetailBySAP();
                PolicyAndPlantMappingViewModel PPVM = _MedicalInsuranceService.GetDashboardInfo(Convert.ToInt64(emp.Designation_Id), Convert.ToInt64(_sessionService.Get<string>("userID")));
                ViewBag.TotalCompanyPaid = PPVM.TotalCompanyPaid;
                ViewBag.TotalAssociatedPaid = PPVM.TotalAssociatedPaid;
                #region "Get if data exist"

                MedicalInsuranceViewModel MIVM_OBJ = _MedicalInsuranceService.GetActivePolicyDetails(Convert.ToInt64(_sessionService.Get<string>("userID")));
                if (MIVM_OBJ.NomineeDetail != null)
                {
                    MIVM.NomineeDetail = MIVM_OBJ.NomineeDetail;
                }
                if (MIVM_OBJ.Dependents != null)
                {
                    MIVM.Dependents = MIVM_OBJ.Dependents.OrderBy(m => m.DepItemId).ToList();
                }
                #endregion
                return View(MIVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw (ex);
                
            }
        }

        // POST: MedicalInsurance/Create
        [HttpPost]
        public async Task<ActionResult> NewJoinee([FromBody]MedicalInsuranceViewModel MIVM)
        {
            Int16 reVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                ModelState.Remove("SearchEmpCode");
                if (ModelState.IsValid)
                {
                    MIVM.RequestType = 1; // New Joinee
                    MIVM.CreatedBy = Convert.ToInt64(_sessionService.Get<string>("userID"));
                    reVal = _MedicalInsuranceService.SaveMedicalInsuranceDetail(MIVM);
                }
            }
            catch (Exception ex)
            {
                reVal = -1;
                //throw (ex);
                _logger.LogError(ex, ex.Message);
            }
            return new JsonResult(reVal);
        }

        // GET: MedicalInsurance/Renewal
        [HttpGet]
        public async Task<ActionResult> Renewal()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                MedicalInsuranceViewModel MIVM = new MedicalInsuranceViewModel();
                Employee_Details emp = _sessionService.Get<Employee_Details>("Employee");
                MIVM =await GetEmployeeDetailBySAP();
                PolicyAndPlantMappingViewModel PPVM = _MedicalInsuranceService.GetDashboardInfo(Convert.ToInt64(emp.Designation_Id), Convert.ToInt64(_sessionService.Get<string>("userID")));
                ViewBag.TotalCompanyPaid = PPVM.TotalCompanyPaid;
                ViewBag.TotalAssociatedPaid = PPVM.TotalAssociatedPaid;
                MedicalInsuranceViewModel MIVM_OBJ = _MedicalInsuranceService.GetActivePolicyDetails(Convert.ToInt64(_sessionService.Get<string>("userID")));
                if (MIVM_OBJ.NomineeDetail != null)
                {
                    MIVM.NomineeDetail = MIVM_OBJ.NomineeDetail;
                }
                if (MIVM_OBJ.Dependents != null)
                {
                    MIVM.Dependents = MIVM_OBJ.Dependents.OrderBy(m => m.DepItemId).ToList();
                }
                return View(MIVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw (ex);
               
            }
        }

        // POST: MedicalInsurance/Renewal
        [HttpPost]
        public async Task<ActionResult> Renewal([FromBody]MedicalInsuranceViewModel MIVM)
        {
            Int16 reVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                ModelState.Remove("SearchEmpCode");
                if (ModelState.IsValid)
                {
                    MIVM.RequestType = 3; // Renewal
                    MIVM.CreatedBy = Convert.ToInt64(_sessionService.Get<string>("userID"));
                    reVal = _MedicalInsuranceService.SaveMedicalInsuranceDetail(MIVM);
                }
            }
            catch (Exception ex)
            {
                reVal = -1;
                // throw (ex);
                _logger.LogError(ex, ex.Message);
            }
            return new JsonResult(reVal);
        }

        // GET: MedicalInsurance/MidTerm
        [HttpGet]
        public async Task<ActionResult> MidTerm()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                MedicalInsuranceViewModel MIVM = new MedicalInsuranceViewModel();
                Employee_Details emp = _sessionService.Get<Employee_Details>("Employee");
                MIVM =await GetEmployeeDetailBySAP();
                MIVM.DOM = emp.DOM == null ? "" : Convert.ToDateTime(emp.DOM).ToString("dd-MMM-yyyy");
                PolicyAndPlantMappingViewModel PPVM = _MedicalInsuranceService.GetDashboardInfo(Convert.ToInt64(emp.Designation_Id), Convert.ToInt64(_sessionService.Get<string>("userID")));
                ViewBag.TotalCompanyPaid = PPVM.TotalCompanyPaid;
                ViewBag.TotalAssociatedPaid = PPVM.TotalAssociatedPaid;
                MedicalInsuranceViewModel MIVM_OBJ = _MedicalInsuranceService.GetActivePolicyDetails(Convert.ToInt64(_sessionService.Get<string>("userID")));
                if (MIVM_OBJ.NomineeDetail != null)
                {
                    MIVM.NomineeDetail = MIVM_OBJ.NomineeDetail;
                }
                if (MIVM_OBJ.Dependents != null)
                {
                    MIVM.Dependents = MIVM_OBJ.Dependents.OrderBy(m => m.DepItemId).ToList();
                }
                return View(MIVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw (ex);
            }
        }

       
        [HttpPost]
        public ActionResult MidTerm([FromBody]MedicalInsuranceViewModel MIVM)
        {
            Int16 reVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                ModelState.Remove("SearchEmpCode");
               
                if (ModelState.IsValid)
                {
                    MIVM.RequestType = 2; // Mid Term
                    MIVM.CreatedBy = Convert.ToInt64(_sessionService.Get<string>("userID"));
                    reVal = _MedicalInsuranceService.SaveMedicalInsuranceDetail(MIVM);
                }
            }
            catch (Exception ex)
            {
                reVal = -1;
                //throw (ex);
                _logger.LogError(ex, ex.Message);
            }
            return new JsonResult(reVal); 
        }
        #endregion

        #region Health Center
        // GET: MedicalInsurance/HealthCenterApproval
        [HttpGet]
        public ActionResult HealthCenterApproval()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                HealthCenterApprovalViewModel PageMdl = new HealthCenterApprovalViewModel();
                Employee_Details obj = _sessionService.Get<Employee_Details>("Employee"); 
                IEnumerable<SYPLANT> SYPlantItems = _MedicalInsuranceService.Bind_SYPlant();
                if (obj.Plant_Id == "5")
                {
                    PageMdl.PlantID = 0;
                }
                else
                {
                    PageMdl.PlantID = Convert.ToInt16(obj.Plant_Id);
                    SYPlantItems = SYPlantItems.Where(x => x.SYPLANTID == Convert.ToInt16(obj.Plant_Id)).ToList();
                }
                ViewBag.SYPlant = new MultiSelectList(SYPlantItems, "SYPLANTID", "PLANTNAME");
                List<SelectListItem> ddlappstatusitem = new List<SelectListItem>();
                ddlappstatusitem.Add(new SelectListItem { Text = "Pending", Value = "0", Selected = true });
                ddlappstatusitem.Add(new SelectListItem { Text = "Approve", Value = "1" });

                ViewBag.AppStatus = ddlappstatusitem;
                //PageMdl.filterData = _MedicalInsuranceService.HealthCenterApprovalList(PageMdl);
                return View(PageMdl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw (ex);
            }
        }

        [HttpPost]
        public ActionResult HealthCenterApproval(HealthCenterApprovalViewModel PageMdl)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details obj = _sessionService.Get<Employee_Details>("Employee");
                IEnumerable<SYPLANT> SYPlantItems = _MedicalInsuranceService.Bind_SYPlant();
                List<SelectListItem> ddlappstatusitem = new List<SelectListItem>();
                ddlappstatusitem.Add(new SelectListItem { Text = "Pending", Value = "0", Selected = true });
                ddlappstatusitem.Add(new SelectListItem { Text = "Approve", Value = "1" });
                ViewBag.AppStatus = ddlappstatusitem;

                PageMdl.filterData = _MedicalInsuranceService.HealthCenterApprovalList(PageMdl);
                if (obj.Plant_Id == "5")
                {
                    PageMdl.PlantID = 0;
                }
                else
                {
                    PageMdl.PlantID = Convert.ToInt16(obj.Plant_Id);
                    SYPlantItems = SYPlantItems.Where(x => x.SYPLANTID == Convert.ToInt16(obj.Plant_Id)).ToList();
                }
                ViewBag.SYPlant = new MultiSelectList(SYPlantItems, "SYPLANTID", "PLANTNAME");
                return View("HealthCenterApproval", PageMdl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw (ex);                
            }
        }

        // GET: MedicalInsurance/DetailsHealthCenter/5
        [HttpGet]
        public ActionResult DetailsHealthCenter(int id)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                MedicalInsuranceViewModel MIVM = _MedicalInsuranceService.GetAllPolicyDetails(id);
                if (MIVM.EmpCode > 0)
                {
                    return PartialView("_DetailsForHealthCenter", MIVM);
                }
                else
                {
                    return new JsonResult("error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new JsonResult("error");
            }
        }

        // GET: MedicalInsurance/_EditForHealthCenter/5
        [HttpGet]
        public ActionResult EditForHealthCenter(int id)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                MedicalInsuranceViewModel MIVM = _MedicalInsuranceService.GetPolicyDetails(id);
                if (MIVM.EmpCode > 0)
                {
                    return PartialView("_EditForHealthCenter", MIVM);
                }
                else
                {
                    return new JsonResult("error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new JsonResult("error");
            }
        }

        [HttpPost]
        //  public ActionResult EditForHealthCenter(Int64 APPROVALID, Int16 AppStatus, Int16 ReqType, string Remark)
        public ActionResult EditForHealthCenter([FromBody] HealtCenterApprovalInputParam param)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Int16 retValue = _MedicalInsuranceService.RequestApproval(param.APPROVALID, param.AppStatus, param.Remark, Convert.ToInt64(_sessionService.Get<string>("userID")));
                if (retValue == 1)
                {
                    SendEmailForNotifiaction(param.APPROVALID, param.AppStatus, param.ReqType);
                }

                return new JsonResult(retValue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new JsonResult(-1);
            }
        }

        public ActionResult DownloadProof(Int64 id, Int64 empCode)
        {
            try
            {
                FileViewModel file = _MedicalInsuranceService.GetProofForDownload(id, empCode);
                if (file.File == null && file.FileContentType == null)
                {
                    return View();
                }
                return File(file.File, file.FileContentType, file.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw (ex);
                            }
        }

        [HttpPost]
        public ActionResult ExportToExcel(HealthCenterApprovalViewModel PageMdl)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                List<VW_MEDINS_EMPDETAIL> VMList = _MedicalInsuranceService.GetHealthCenterDataForExcel(PageMdl);
                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[13] {
                                            new DataColumn("SNO"),
                                            new DataColumn("Request Type"),
                                            new DataColumn("Emp Code"),
                                            new DataColumn("Requestor Name"),
                                            new DataColumn("Designation"),
                                            new DataColumn("Mobile"),
                                            new DataColumn("Name"),
                                            new DataColumn("Gender"),
                                            new DataColumn("D.O.B"),
                                            new DataColumn("Relationship"),
                                            new DataColumn("Type"),
                                            new DataColumn("Change Type"),
                                            new DataColumn("Paid Type")
            });
                foreach (var item in VMList)
                {
                    var _dob = item.DOB.ToString() == "01-Jan-1900 00:00:00" ? " " : item.DOB.ToString();
                    dt.Rows.Add(item.SNO, item.REQUESTTYPE, item.EMP_CODE, item.ENAME, item.DESIGNATION, item.TMOBILE, item.PERNAME, item.GENDER, _dob, item.RELATIONSHIP, item.RECTYPE, item.CHANGETYPE, item.PAIDTYPE);
                }
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Health_Center_Report.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw (ex);
                
            }
        }

        [HttpPost]
        public ActionResult GetPdf([FromBody]HealthCenterApprovalViewModel PageMdl)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                //PageMdl.filterData = _MedicalInsuranceService.HealthCenterApprovalList(PageMdl);
                List<MedicalInsuranceViewModel> iList = new List<MedicalInsuranceViewModel>();
                if (PageMdl.EmpIds != null)
                {
                    foreach (var emp in PageMdl.EmpIds)
                    {
                        MedicalInsuranceViewModel MIVM = _MedicalInsuranceService.GetActivePolicyDetails(Convert.ToInt64(emp));
                        iList.Add(MIVM);
                    }
                    PageMdl.Emp_Detail_List = iList;
                }
                return new JsonResult(PageMdl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw (ex);
               
            }
        }

        [HttpGet]
        public ActionResult HealthCenterReport()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                HealthCenterApprovalViewModel PageMdl = new HealthCenterApprovalViewModel();
                Employee_Details obj = _sessionService.Get<Employee_Details>("Employee");
                IEnumerable<SYPLANT> SYPlantItems = _MedicalInsuranceService.Bind_SYPlant();
                if (obj.Plant_Id == "5")
                {
                    PageMdl.PlantID = 0;
                }
                else
                {
                    PageMdl.PlantID = Convert.ToInt16(obj.Plant_Id);
                    SYPlantItems = SYPlantItems.Where(x => x.SYPLANTID == Convert.ToInt16(obj.Plant_Id)).ToList();
                }
                ViewBag.SYPlant = new MultiSelectList(SYPlantItems, "SYPLANTID", "PLANTNAME");
                List<SelectListItem> ddlappstatusitem = new List<SelectListItem>();
                //ddlappstatusitem.Add(new SelectListItem { Text = "Pending", Value = "0", Selected = true });
                ddlappstatusitem.Add(new SelectListItem { Text = "Approve", Value = "1", Selected = true });

                ViewBag.AppStatus = ddlappstatusitem;
                PageMdl.ApprovalStatus = 1; // Approve
                //PageMdl.filterData = _MedicalInsuranceService.HealthCenterApprovalList(PageMdl);
                return View(PageMdl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw (ex);
                
            }
        }

        [HttpPost]
        public ActionResult HealthCenterReport(HealthCenterApprovalViewModel PageMdl)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details obj = _sessionService.Get<Employee_Details>("Employee");
                IEnumerable<SYPLANT> SYPlantItems = _MedicalInsuranceService.Bind_SYPlant();
                List<SelectListItem> ddlappstatusitem = new List<SelectListItem>();
                //ddlappstatusitem.Add(new SelectListItem { Text = "Pending", Value = "0", Selected = true });
                ddlappstatusitem.Add(new SelectListItem { Text = "Approve", Value = "1", Selected = true });

                ViewBag.AppStatus = ddlappstatusitem;
                PageMdl.filterData = _MedicalInsuranceService.HealthCenterApprovalList(PageMdl);
                if (obj.Plant_Id == "5")
                {
                    PageMdl.PlantID = 0;
                }
                else
                {
                    PageMdl.PlantID = Convert.ToInt16(obj.Plant_Id);
                    SYPlantItems = SYPlantItems.Where(x => x.SYPLANTID == Convert.ToInt16(obj.Plant_Id)).ToList();
                }
                ViewBag.SYPlant = new MultiSelectList(SYPlantItems, "SYPLANTID", "PLANTNAME");
                return View("HealthCenterReport", PageMdl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw (ex);
               
            }
        }

        [HttpGet]
        public ActionResult MasterDataReport()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                MasterReportViewModel MRVM = new MasterReportViewModel();
                Employee_Details obj = _sessionService.Get<Employee_Details>("Employee");
                IEnumerable<SYPLANT> SYPlantItems = _MedicalInsuranceService.Bind_SYPlant();
                if (obj.Plant_Id == "5")
                {
                    MRVM.PlantID = 0;
                }
                else
                {
                    MRVM.PlantID = Convert.ToInt16(obj.Plant_Id);
                    SYPlantItems = SYPlantItems.Where(x => x.SYPLANTID == Convert.ToInt16(obj.Plant_Id)).ToList();
                }
                ViewBag.SYPlant = new MultiSelectList(SYPlantItems, "SYPLANTID", "PLANTNAME");
                return View(MRVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw (ex);
                
            }
        }

        [HttpPost]
        public ActionResult MasterDataReport(int currentPageIndex, int plantId, string Command, string ApprovalDateFrom, string ApprovalDateTo, string ReqDateFrom, string ReqDateTo)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                #region Search master data report
                if (Command == "search")
                {
                    MasterReportViewModel MRVM = new MasterReportViewModel();
                    Employee_Details obj = _sessionService.Get<Employee_Details>("Employee");
                    IEnumerable<SYPLANT> SYPlantItems = _MedicalInsuranceService.Bind_SYPlant();
                    MRVM = _MedicalInsuranceService.GetMasterDataReport(currentPageIndex, plantId, ApprovalDateFrom, ApprovalDateTo, ReqDateFrom, ReqDateTo);
                    if (obj.Plant_Id == "5")
                    {
                        MRVM.PlantID = 0;
                    }
                    else
                    {
                        MRVM.PlantID = Convert.ToInt16(obj.Plant_Id);
                        SYPlantItems = SYPlantItems.Where(x => x.SYPLANTID == Convert.ToInt16(obj.Plant_Id)).ToList();
                    }
                    ViewBag.SYPlant = new MultiSelectList(SYPlantItems, "SYPLANTID", "PLANTNAME");
                    return View(MRVM);
                }
                #endregion

                #region Download master data report excel
                else
                {
                    List<VW_MEDINS_MSTDETAILS> VWList = _MedicalInsuranceService.GetMasterDataForExcel(plantId, ApprovalDateFrom, ApprovalDateTo, ReqDateFrom, ReqDateTo);
                    DataTable dt = new DataTable("Grid");
                    dt.Columns.AddRange(new DataColumn[15] {
                                            new DataColumn("Sno"),
                                            new DataColumn("Request Type"),
                                            new DataColumn("Emp Code"),
                                            new DataColumn("Requestor Name"),
                                            new DataColumn("Designation"),
                                            new DataColumn("D.O.J"),
                                            new DataColumn("Mobile"),
                                            new DataColumn("Name"),
                                            new DataColumn("Gender"),
                                            new DataColumn("D.O.B"),
                                            new DataColumn("Age"),
                                            new DataColumn("Relationship"),
                                            new DataColumn("Type"),
                                            new DataColumn("Change Type"),
                                            new DataColumn("Paid Type"),
                    });
                    long srNo = 0;
                    foreach (var item in VWList)
                    {
                        srNo += 1;
                        var _dob = item.DOB == null ? " " : item.DOB.ToString() == "01-Jan-1900 00:00:00" ? " " : item.DOB.ToString();
                        var _age = item.DOB == null ? " " : item.DOB.ToString() == "01-Jan-1900 00:00:00" ? " " : item.AGE;
                        var _doj = item.DOJ == null ? " " : item.DOJ.ToString() == "01-Jan-1900 00:00:00" ? " " : item.DOJ.ToString();
                        dt.Rows.Add(srNo, item.REQUESTTYPE, item.EMP_CODE, item.ENAME, item.DESIGNATION, _doj, item.TMOBILE, item.PERNAME, item.GENDER, _dob, _age, item.RELATIONSHIP, item.RECTYPE, item.CHANGETYPE, item.PAIDTYPE);
                    }
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Master_Report.xlsx");
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw (ex);
               
                                
            }
           
        }
        #endregion

        #region Master Pages
        // GET: MedicalInsurance/RenewalPeriodSetting
        [HttpGet]
        public ActionResult RenewalPeriodSetting()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                RenewalPeriodViewModel RPVM = new RenewalPeriodViewModel();
                RPVM.PeriodSettingList = _MedicalInsuranceService.GetRenewalPeriodList();
                IEnumerable<SYPLANT> SYPlantItems = _MedicalInsuranceService.Bind_SYPlant();
                ViewBag.SYPlant = new MultiSelectList(SYPlantItems, "SYPLANTID", "PLANTNAME");
                return View(RPVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw (ex);
            }
        }

        // POST: MedicalInsurance/RenewalPeriodSetting
        [HttpPost]
        public ActionResult RenewalPeriodSetting([FromBody] RenewalPeriodViewModel RPVM)
        {
            try
            {
                Int16 retVal = 0;
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (ModelState.IsValid)
                {
                    RPVM.CreatedBy = Convert.ToString(_sessionService.Get<string>("userID"));
                    retVal = _MedicalInsuranceService.SaveRenewalPeriod(RPVM);
                    if (retVal == 1)
                    {
                        if (RPVM.Plant_Ids != null && RPVM.UserType == 1)
                        {
                            foreach (var plant in RPVM.Plant_Ids)
                            {
                                EmployeePolicyLocationMapping EPLM = new EmployeePolicyLocationMapping();
                                EPLM.EmployeePolicyList = _MedicalInsuranceService.GetEmpForSendMail(Convert.ToInt16(plant));
                                EPLM.EmployeePolicyList = EPLM.EmployeePolicyList.Where(x => x.EmpId < 100000).ToList();
                                foreach (EmployeePolicyLocationMapping Obj_EPLM in EPLM.EmployeePolicyList)
                                {
                                    if (!String.IsNullOrEmpty(Obj_EPLM.Emp_Email))
                                    {
                                        SendEmailForRenewalPeriod(Obj_EPLM, RPVM.FromDate, RPVM.ToDate);
                                    }
                                }
                            };
                        }
                    }
                    return new JsonResult(retVal);
                }
                else
                {
                    return new JsonResult(retVal);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new JsonResult(ex.InnerException.Message);
            }
        }

        // PUT: MedicalInsurance/DeactivateRenewalPeriod
        [HttpPut]
        public ActionResult DeactivateRenewalPeriod([FromBody]RenewalPeriodViewModel RPVM)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                RPVM.ModifiedBy = Convert.ToString(_sessionService.Get<string>("userID"));
                Int16 res = _MedicalInsuranceService.DeactivateRenewalPeriod(RPVM);
                return new JsonResult(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new JsonResult(ex.InnerException.Message);
            }
        }

        // GET: MedicalInsurance/EmployeePolicyLocation
        [HttpGet]
        public ActionResult EmployeePolicyLocation()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                EmployeePolicyLocationMapping EPLM = new EmployeePolicyLocationMapping();
                EPLM.EmployeePolicyList = _MedicalInsuranceService.GetEmployeePolicyLocationList();
                IEnumerable<SYPLANT> SYPlantItems = _MedicalInsuranceService.Bind_SYPlant();
                ViewBag.SYPlant = new MultiSelectList(SYPlantItems, "SYPLANTID", "PLANTNAME");
                return View(EPLM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw (ex);
            }
        }

        [HttpPost]
        public JsonResult EmployeePolicyLocationUpload(string Plantid, [FromForm] IFormFile file)
        {
            try
            {
                List<string> listEmployee = new List<string>();
                EmployeePolicyLocationMapping EPLM = new EmployeePolicyLocationMapping();
                EPLM.PlantId = Convert.ToInt16(Plantid);
                if (file.Length > 0)
                {
                    EPLM.Upload = file;
                }

                Int16 retVal = 0;
                if (_sessionService.Get<string>("userID") == null)
                {
                    return new JsonResult(-1);
                }
                //if (ModelState.IsValid)
                //{
                //    #region "Added by vineet to read input steam for employee code"
                //    string strEcode = "";
                //    long FileLen;
                //    System.IO.Stream MyStream;
                //    FileLen = EPLM.Upload.Length;
                //    byte[] input = new byte[FileLen];
                //    // Initialize the stream.
                //    MyStream = EPLM.Upload.InputStream;
                //    // Read the file into the byte array.
                //    MyStream.Read(input, 0, FileLen);
                //    MyStream.Position = 0;
                //    using (var reader = new StreamReader(MyStream))
                //    {
                //        int lineno = 0;
                //        if (lineno == 0)

                //            while (!reader.EndOfStream)
                //            {
                //                lineno += 1;
                //                var line = reader.ReadLine();
                //                if (lineno != 1)
                //                {
                //                    strEcode = strEcode + line + ",";
                //                    listEmployee.Add(line);
                //                }
                //            }
                //    }

                //    #endregion

                //    EPLM.CreatedBy = Convert.ToString(_sessionService.Get<string>("userID"));
                //    EPLM.Employee = strEcode.Substring(0, strEcode.Length - 1);
                //    retVal = _MedicalInsuranceService.SaveEmployeePolicyLocation(EPLM);
                //    return new JsonResult(retVal);
                //}

                if (ModelState.IsValid && file != null && file.Length > 0)
                {
                    string strEcode = "";

                    using (var stream = file.OpenReadStream())
                    using (var reader = new StreamReader(stream))
                    {
                        int lineno = 0;
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            lineno++;

                            if (lineno != 1 && !string.IsNullOrWhiteSpace(line))
                            {
                                strEcode += line + ",";
                                listEmployee.Add(line);
                            }
                        }
                    }

                    EPLM.CreatedBy = _sessionService.Get<string>("userID");
                    EPLM.Employee = strEcode.TrimEnd(',');
                    retVal = _MedicalInsuranceService.SaveEmployeePolicyLocation(EPLM);

                    return new JsonResult(retVal);
                }


                else
                {
                    return new JsonResult(retVal);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new JsonResult(0);
            }
        }

        // POST: MedicalInsurance/EmployeePolicyLocation
        //[HttpPost]
        //public ActionResult EmployeePolicyLocation(EmployeePolicyLocationMapping EPLM)
        //{
        //    try
        //    {
        //        Int16 retVal = 0;
        //        if (Session["UserId"] == null)
        //        {
        //            return RedirectToAction("Index", "Login");
        //        }
        //        if (ModelState.IsValid)
        //        {
        //            EPLM.CreatedBy = Convert.ToString(Session["UserId"]);
        //            retVal = _MedicalInsuranceService.SaveEmployeePolicyLocation(EPLM);
        //            return Json(retVal, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            return Json(retVal, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(ex.InnerException.Message, JsonRequestBehavior.AllowGet);
        //    }
        //}

        // GET: MedicalInsurance/PolicyAndDesignationMapping

        [HttpPost]
        public IActionResult PolicyAndDesignationMapping(List<PolicyAndPlantMappingViewModel> PPVM)
        {
            try
            {
                //PolicyAndPlantMappingViewModel PPVM = new PolicyAndPlantMappingViewModel();
                Int16 retVal = 0;
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                PPVM.ForEach(m => m.CreatedBy = Convert.ToInt64(_sessionService.Get<string>("userID")));
                retVal = _MedicalInsuranceService.SaveMappingDetail(PPVM);
                return new JsonResult(retVal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new JsonResult(ex.InnerException.Message);
            }
        }

        // POST: MedicalInsurance/PolicyAndDesignationMapping
        [HttpGet]
        public ActionResult PolicyAndDesignationMapping()
        {
            try
            {
                PolicyMappingViewModal PDVM = new PolicyMappingViewModal();
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                PDVM.PlantPolicyDesgList = _MedicalInsuranceService.GetMappingList();
                IEnumerable<SYPLANT> SYPlantItems = _MedicalInsuranceService.Bind_SYPlant();
                IEnumerable<ADDESIGNATION> ADDeasignationItems = _MedicalInsuranceService.Bind_ADDesignation();
                PDVM.PolicyMaster = _MedicalInsuranceService.GetPolicyTypeList();
                ViewBag.SYPlant = new MultiSelectList(SYPlantItems, "SYPLANTID", "PLANTNAME");
                ViewBag.ADDeasignation = new MultiSelectList(ADDeasignationItems, "ADDESIGNATIONID", "DESCRIP");
                PDVM.PolicyPlantMap = new List<PolicyAndPlantMappingViewModel>();
                foreach (var obj in PDVM.PolicyMaster)
                {
                    PolicyAndPlantMappingViewModel iColl = new PolicyAndPlantMappingViewModel();
                    iColl.PolicyType = obj.TypeCode;
                    iColl.PolicyTypeId = obj.PolicyTypeId;
                    PDVM.PolicyPlantMap.Add(iColl);
                }
                ModelState.Clear();
                return View(PDVM);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw (ex);
            }
        }

        [HttpGet]
        public ActionResult GetPlantChange(string Plantid)
        {
            PolicyMappingViewModal PDVM = new PolicyMappingViewModal();

            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                //PolicyAndDesignationMappingViewModel PDVM = new PolicyAndDesignationMappingViewModel();
                PDVM.PlantMappID = Convert.ToInt32(Plantid);
                PDVM.PlantPolicyDesgList = _MedicalInsuranceService.GetMappingList();
                IEnumerable<SYPLANT> SYPlantItems = _MedicalInsuranceService.Bind_SYPlant();
                IEnumerable<ADDESIGNATION> ADDeasignationItems = _MedicalInsuranceService.Bind_ADDesignation();
                PDVM.PolicyMaster = _MedicalInsuranceService.GetPolicyTypeList();
                ViewBag.SYPlant = new MultiSelectList(SYPlantItems, "SYPLANTID", "PLANTNAME");
                ViewBag.ADDeasignation = new MultiSelectList(ADDeasignationItems, "ADDESIGNATIONID", "DESCRIP");
                PDVM.PolicyPlantMap = new List<PolicyAndPlantMappingViewModel>();
                foreach (var o in PDVM.PolicyMaster)
                {
                    PolicyAndPlantMappingViewModel oParam = new PolicyAndPlantMappingViewModel();
                    oParam.PlantId = Convert.ToInt16(PDVM.PlantMappID);
                    oParam.PolicyTypeId = o.PolicyTypeId;
                    List<PolicyAndPlantMappingViewModel> iNewList = _MedicalInsuranceService.GetPolicyDesgMappingList(new PolicyAndPlantMappingViewModel { PlantId = Convert.ToInt16(PDVM.PlantMappID), PolicyTypeId = o.PolicyTypeId });
                    if (iNewList.Count() > 0)
                    {
                        if (iNewList.FirstOrDefault().DesignationIds != null)
                        {
                            oParam.DesignationIds = iNewList.FirstOrDefault().DesignationIds;

                        }
                        oParam.MappingId = iNewList.FirstOrDefault().MappingId;
                        oParam.AssociatedPaid_Cnt = iNewList.FirstOrDefault().AssociatedPaid_Cnt;
                        oParam.CompanyPaid_Cnt = iNewList.FirstOrDefault().CompanyPaid_Cnt;
                    }
                    PDVM.PolicyPlantMap.Add(oParam);
                }
                ModelState.Clear();
                return View("PolicyAndDesignationMapping", PDVM);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw (ex);
            }
        }
        #endregion

        #region Other Associate User
        // GET: MedicalInsurance
        [HttpGet]
        public ActionResult AdminDashboard()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details emp = _sessionService.Get<Employee_Details>("Employee");
                //ViewBag.UserId = Session["UserId"];
                //ViewBag.IsExist = _MedicalInsuranceService.GetIsExistNewJoinee(Convert.ToInt64(Session["UserId"]));
                PolicyAndPlantMappingViewModel PPVM = _MedicalInsuranceService.GetAdminDashboardInfo(Convert.ToInt64(emp.Designation_Id), Convert.ToInt64(_sessionService.Get<string>("userID")));
                //ViewBag.CompanyPaid_Cnt = PPVM.CompanyPaid_Cnt;
                //ViewBag.AssociatedPaid_Cnt = PPVM.AssociatedPaid_Cnt;
                //ViewBag.TotalCompanyPaid = PPVM.TotalCompanyPaid;
                //ViewBag.TotalAssociatedPaid = PPVM.TotalAssociatedPaid;
                ViewBag.PlantLocation = PPVM.Plant;
                ViewBag.Renewal_StartDate = PPVM.Renewal_StartDate;
                ViewBag.Renewal_EndDate = PPVM.Renewal_EndDate;
                ViewBag.ApprovalStatus = PPVM.ApprovalStatus;
                ViewBag.IsRenewal = _MedicalInsuranceService.GetIsExistRenewal(PPVM.PlantId, 2);
                List<MedicalInsuranceViewModel> MIVMList = _MedicalInsuranceService.AdminPendingRequest(Convert.ToInt64(_sessionService.Get<string>("userID")));
                ViewBag.MIVMList = MIVMList;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw (ex);
            }
        }

        [HttpGet]
        public ActionResult NewjoineeOtherAssociate()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                ViewBag.TotalCompanyPaid = 0;
                ViewBag.TotalAssociatedPaid = 0;
                MedicalInsuranceViewModel MIVM = new MedicalInsuranceViewModel();
                return View(MIVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw (ex);
            }
        }

        [HttpPost]
        public ActionResult NewjoineeOtherAssociate(MedicalInsuranceViewModel MIVM)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details emp = _sessionService.Get<Employee_Details>("Employee");
                PolicyAndPlantMappingViewModel PPVM = _MedicalInsuranceService.GetOtherAssociateInfo(Convert.ToInt64(MIVM.SearchEmpCode));
                ViewBag.TotalCompanyPaid = PPVM.TotalCompanyPaid;
                ViewBag.TotalAssociatedPaid = PPVM.TotalAssociatedPaid;
                MedicalInsuranceViewModel MIVM_OBJ = _MedicalInsuranceService.NewJoineeOtherAssociateByEmpCode(Convert.ToInt64(MIVM.SearchEmpCode), Convert.ToInt64(emp.Plant_Id));
                return View("NewjoineeOtherAssociate", MIVM_OBJ);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw (ex);
            }
        }

        [HttpGet]
        public ActionResult MidTermOtherAssociate()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                ViewBag.TotalCompanyPaid = 0;
                ViewBag.TotalAssociatedPaid = 0;
                ViewBag.ApprovalStatus = 0;
                MedicalInsuranceViewModel MIVM = new MedicalInsuranceViewModel();
                return View(MIVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw (ex);
            }
        }

        [HttpPost]
        public ActionResult MidTermOtherAssociate(MedicalInsuranceViewModel MIVM)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                PolicyAndPlantMappingViewModel PPVM = _MedicalInsuranceService.GetOtherAssociateInfo(Convert.ToInt64(MIVM.SearchEmpCode));
                ViewBag.TotalCompanyPaid = PPVM.TotalCompanyPaid;
                ViewBag.TotalAssociatedPaid = PPVM.TotalAssociatedPaid;
                ViewBag.ApprovalStatus = PPVM.ApprovalStatus;
                MedicalInsuranceViewModel MIVM_OBJ = _MedicalInsuranceService.OtherAssociateByEmpCode(Convert.ToInt64(MIVM.SearchEmpCode));
                return View("MidTermOtherAssociate", MIVM_OBJ);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw (ex);
            }
        }

        [HttpGet]
        public ActionResult RenewalOtherAssociate()
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                ViewBag.TotalCompanyPaid = 0;
                ViewBag.TotalAssociatedPaid = 0;
                ViewBag.ApprovalStatus = 0;
                MedicalInsuranceViewModel MIVM = new MedicalInsuranceViewModel();
                return View(MIVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw (ex);
            }
        }

        [HttpPost]
        public ActionResult RenewalOtherAssociate(MedicalInsuranceViewModel MIVM)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                PolicyAndPlantMappingViewModel PPVM = _MedicalInsuranceService.GetOtherAssociateInfo(Convert.ToInt64(MIVM.SearchEmpCode));
                ViewBag.TotalCompanyPaid = PPVM.TotalCompanyPaid;
                ViewBag.TotalAssociatedPaid = PPVM.TotalAssociatedPaid;
                ViewBag.ApprovalStatus = PPVM.ApprovalStatus;
                MedicalInsuranceViewModel MIVM_OBJ = _MedicalInsuranceService.OtherAssociateByEmpCode(Convert.ToInt64(MIVM.SearchEmpCode));
                return View("RenewalOtherAssociate", MIVM_OBJ);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw (ex);
            }
        }

        [HttpPost]
        public ActionResult SaveOtherAssociateUser([FromBody]MedicalInsuranceViewModel MIVM)
        {
            Int16 reVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                ModelState.Remove("SearchEmpCode");
                if (ModelState.IsValid)
                {
                    MIVM.CreatedBy = Convert.ToInt64(_sessionService.Get<string>("userID"));
                    reVal = _MedicalInsuranceService.SaveOtherAssociateUserDetail(MIVM);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                reVal = -1;
                throw (ex);
            }
            return new JsonResult(reVal);
        }
        #endregion

        //CR-4182 change start
        #region Panel Consultant List
        [HttpGet]
        public ActionResult PanelConsultList()
        {
            return View();
        }
        #endregion
        //CR-4182 change end

        public void SendEmailForRenewalPeriod(EmployeePolicyLocationMapping Obj_EPLM, String FromDate, String ToDate)
        {
            try
            {
                commanEmail sendMail = new commanEmail();
                sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                sendMail.MailTo = Obj_EPLM.Emp_Email;
                string strSubject = "For Renewal Period";
                string strBody = "<p>Dear " + Obj_EPLM.Employee + " San<br/><br/>Greetings of the day<br/><br/></p><p>Your mediclaim insurance policy is due for renewal. Window is open from " + FromDate + " to " + ToDate + ". You can make the required changes.<b> No changes will be accepted by Health & Wellness team once the window is closed.</b></p><br/><br/><p>Regards<br/><br/>Health & Wellness Team</p>";
                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;
                sendMail.Send();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw (ex);
            }
        }

        public void SendEmailForNotifiaction(Int64 EmpCode, Int64 Status, Int16 ReqType)
        {
            try
            {
                ADEmployeeViewModel EVM = _MedicalInsuranceService.GetADEmployeeDetail(EmpCode);
                if (EVM.EmpId > 0 && !String.IsNullOrEmpty(EVM.Email))
                {
                    string strSubject = string.Empty;
                    switch (ReqType)
                    {
                        case 1:
                            strSubject = "For New Joinee";
                            break;
                        case 2:
                            strSubject = "For Addition/Deletion (Mid Term)";
                            break;
                        case 3:
                            strSubject = "For Renewal";
                            break;
                    }
                    string strBody = string.Empty;
                    switch (Status)
                    {
                        case 1: // Approved
                            strBody = "<p>Dear " + EVM.EmpName + " San<br/><br/>Greetings of the day<br/><br/></p><p>Your change request in mediclaim insurance has been approved by Health & Wellness.</p><br/><br/><p>Regards<br/><br/>Health & Wellness Team</p>";
                            break;
                        case 3: // SendBack
                            strBody = "<p>Dear " + EVM.EmpName + " San<br/><br/>Greetings of the day<br/><br/></p><p>Your change request in mediclaim insurance has been send back by Health & Wellness.</b></p><br/><br/><p>Regards<br/><br/>Health & Wellness Team</p>";
                            break;
                        case 4: // Rejected
                            strBody = "<p>Dear " + EVM.EmpName + " San<br/><br/>Greetings of the day<br/><br/></p><p>Your change request in mediclaim insurance has been rejected by Health & Wellness.</b></p><br/><br/><p>Regards<br/><br/>Health & Wellness Team</p>";
                            break;
                    }
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    sendMail.MailTo = EVM.Email;
                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    sendMail.Send();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw (ex);
            }
        }

    }
}
