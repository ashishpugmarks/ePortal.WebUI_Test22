using Microsoft.AspNetCore.Mvc;
using ePortal.ViewModels;
using System.Globalization;
using System.Configuration;
using ePortal.DomainClasses;
using System.Data;
using ePortal.WebUI.Filters;
using ePortal.Shared.Interface;
using ePortal.WebUI.Controllers;
using ePortal.Application.Contracts;
using Microsoft.AspNetCore.Mvc.Rendering;
using ePortal.Shared;
using ClosedXML.Excel;
using ePortal.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter("",13)]
    public class A00SubmitController : Controller
    {
        private readonly IA00Service _objHomeA00;
        A00SearchModel _objA00SearchModel;
        private readonly ILogger<A00SubmitController> _logger;
        private readonly IConfiguration _settings;
        private readonly ISessionService _sessionService;
        private readonly EPortalDBContext _ePortalDBContext;
        public A00SubmitController(EPortalDBContext ePortalDBContext, IA00Service IA00Service, ILogger<A00SubmitController> logger, IConfiguration settings, ISessionService sessionService)
        {
            _objHomeA00 = IA00Service;
            _objA00SearchModel = new A00SearchModel();

            _logger = logger;
            _settings = settings;
            _sessionService = sessionService;
            _ePortalDBContext = ePortalDBContext;
        }
        [HttpGet]
        private ActionResult A00(string returnMsg = null, long? A00DTLTBID = null, string type = null, long? OPERATIONID = null, long? DIVISIONID = null, long? DEPARTMENTID = null, long? SECTIONID = null, decimal? SYKIID = null, string ECODE = null, string EMPNAME = null, short? STATUSID = null, bool? isReset = null)
        {
            try
            {
                if (isReset == true)
                {
                    if (A00DTLTBID != 0 && A00DTLTBID != null)
                    {
                        _objA00SearchModel.a00dtltbid = A00DTLTBID;
                        var record = _objHomeA00.GetA00Dtl(_objA00SearchModel);
                        //var record = _Assetdatabasecontext.A00DTLTB.Where(x => x.A00DTLTBID == A00DTLTBID).FirstOrDefault();
                        if (record.STATUSCD == 10)
                        {
                            return RedirectToAction("A00", new
                            {
                                A00DTLTBID = A00DTLTBID,
                                Type = type,
                                OPERATIONID = OPERATIONID,
                                DIVISIONID = DIVISIONID,
                                DEPARTMENTID = DEPARTMENTID,
                                SECTIONID = SECTIONID,
                                SYKIID = SYKIID,
                                ECODE = ECODE,
                                EMPNAME = EMPNAME,
                                STATUSID = STATUSID
                            });
                        }
                        else
                        {
                            return RedirectToAction("A00");
                        }
                    }
                    else
                    {
                        return RedirectToAction("A00");
                    }
                }
                else
                {
                    ViewBag.A00DTLTBID = A00DTLTBID;
                    SetSearchValue(OPERATIONID, DIVISIONID, DEPARTMENTID, SECTIONID, SYKIID, ECODE, EMPNAME, STATUSID);
                    if (returnMsg != null)
                    {
                        ViewBag.Msg = returnMsg;
                    }

                    TempData["PageHead"] = "A00 Concept Form";
                    ViewBag.Years = DateTime.Now.Year;
                    type = type == null ? "NEW" : type.ToUpper();
                    ViewBag.pageType = type;
                    ViewBag.actionType = "Submit";
                    ViewBag.isUploadedFilesExist = "N";
                    #region In New Mode

                    List<SYKI> iList = new List<SYKI>();
                    iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });


                    var kiData = _objHomeA00.GetA00SYKIList();
                    foreach (var item in kiData._SYKIList)
                    {
                        iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                    }
                    var ActiveKiID = SYKIID > 0 ? SYKIID : kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
                    var operation = _objHomeA00.GetA00ADORGLEVELList(ActiveKiID);
                    List<SearchResultList> budgByList = new List<SearchResultList>();
                    budgByList.Add(new SearchResultList { LEVELDESCRIP = "Select", ADORGLEVELID = 0 });
                    foreach (var item in operation._ADOrgLevelList)
                    {
                        budgByList.Add(new SearchResultList { LEVELDESCRIP = item.LEVELDESCRIP, ADORGLEVELID = item.ADORGLEVELID });
                    }

                    List<SelectListItem> datalist = new List<SelectListItem>();

                    var InvforcastList = _objHomeA00.GetA00_INVFORCASTList();
                    foreach (var Datat4 in InvforcastList._InvforcastList)
                    {
                        datalist.Add(new SelectListItem { Text = Datat4.INCFORCASTDETAIL, Value = Datat4.INVFORCASTID.ToString() });
                    }
                    ViewBag.SKYLIST = datalist;



                    A00ViewModel objModel = new A00ViewModel();
                    objModel.getAllDaysList = objModel.getAllWeekDaysList();
                    objModel.GetAllmonth = objModel.getMonth();
                    objModel.Getdate = objModel.GetAllDate();
                    #endregion
                    ViewBag.isOtherSelected = "N";

                    var A00InvForcast = _objHomeA00.GetA00INVFORCASTList();
                    #region When View/Action Button Pressed
                    if (type == "VIEW" || type == "ACTION")
                    {
                        //var _A00ExistData = _Assetdatabasecontext.A00DTLTB.Where(x => x.A00DTLTBID == A00DTLTBID).FirstOrDefault();
                        _objA00SearchModel.a00dtltbid = A00DTLTBID;
                        var _A00ExistData = _objHomeA00.GetA00Dtl(_objA00SearchModel);

                        objModel.Projecttitle = _A00ExistData.PRJCTTLE;
                        objModel.Backgrounds = _A00ExistData.PRJCTTXT;
                        objModel.BusinessKPI = _A00ExistData.BUKPITXT;
                        objModel.PurposeA00 = _A00ExistData.PURPSTXT;
                        objModel.TargetA00 = _A00ExistData.TRGTINDCD;
                        objModel.RequirmentA00 = _A00ExistData.RQUMTTXT;
                        objModel.ImagePath = _A00ExistData.ATTACHMENT;
                        objModel.BudgetedSelected = _A00ExistData.BUDGETFLG.ToString();
                        objModel.StartDate = _A00ExistData.STARTDT?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        objModel.EndDate = _A00ExistData.ENDDT?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        objModel.Ifothers = _A00ExistData.FRCSTOTHER;

                        if(A00DTLTBID != null)
                        {
                            if (type == "ACTION")
                            {
                                var currStatus = _A00ExistData.STATUSCD;
                                //var sentBackOrSaveDraft = new List<int> { 10, 120, 55, 75, 95 }; //115 added
                                var sentBackOrSaveDraft = new List<int> { 10, 35, 55, 75, 95, 115 }; //115 added
                                if (sentBackOrSaveDraft.Contains(currStatus))
                                {
                                    ViewBag.actionType = "Submit";
                                }
                                else
                                {
                                    //Utilities.UserRole userData = new Utilities.UserData().getUserRole(Convert.ToInt64(_sessionService.Get<string>("userID")));
                                    var userData = _objHomeA00.getUserRole(Convert.ToInt64(_sessionService.Get<string>("userID")), ActiveKiID);

                                    if (userData.isDeptHead || userData.isCoOrdHead || userData.isDivHead || userData.isExeCoOrdHead || userData.isOperatingHead)
                                    {
                                        ViewBag.actionType = "Approval";
                                        this.generateApprovalHistory(A00DTLTBID ?? 0);
                                    }
                                }
                            }
                            else
                            {
                                ViewBag.actionType = "View";
                                this.generateApprovalHistory(A00DTLTBID ?? 0);
                            }
                        }
                        List<SelectListItem> mlist = new List<SelectListItem>();
                        mlist.Add(new SelectListItem { Text = "Select", Value = "", Disabled = true });

                        if (_A00ExistData.BUDGETFLG.ToString() == "1")
                        {
                            mlist.Add(new SelectListItem { Text = "Yes", Value = "10", Selected = true });
                            mlist.Add(new SelectListItem { Text = "No", Value = "11" });
                        }
                        else if (_A00ExistData.BUDGETFLG.ToString() == "0")
                        {
                            mlist.Add(new SelectListItem { Text = "No", Value = "11", Selected = true });
                            mlist.Add(new SelectListItem { Text = "Yes", Value = "10" });
                        }

                        ViewBag.BudgetedValue = mlist;
                        ViewBag.selectedBudgetedVal = _A00ExistData.BUDGETFLG.ToString();
                        var selectedBudgBy = budgByList.Where(x => x.ADORGLEVELID == _A00ExistData.ADORGLEVELID).FirstOrDefault();
                        ViewBag.List = new SelectList(budgByList, "ADORGLEVELID", "LEVELDESCRIP", selectedBudgBy?.ADORGLEVELID);
                        var selectedSyKI = iList.Where(x => x.SYKIID == _A00ExistData.BUDGETSYKIID).FirstOrDefault();
                        ViewBag.SKY = new SelectList(iList, "SYKIID", "KICODE", selectedSyKI?.SYKIID);
                        //var ief = _Assetdatabasecontext.A00INVFORCAST.Join(_Assetdatabasecontext.INVFORCAST, a => a.INVFORCASTID, 
                        ///b => b.INVFORCASTID, (a, b) => new { a, b }).Where(x => x.a.A00DTLTBID == A00DTLTBID && x.a.ACTIVE == 1).
                        ///Select(m => new { INVFORCASTID =//// m.a.INVFORCASTID, INCFORCASTDETAIL = m.b.INCFORCASTDETAIL }).Distinct().ToList();

                        var ief = A00InvForcast._A00InvforcastList.Join(InvforcastList._InvforcastList,
                            a => a.INVFORCASTID, b => b.INVFORCASTID, (a, b) => new { a, b }).Where(x => x.a.A00DTLTBID == A00DTLTBID && x.a.ACTIVE == 1).
                            Select(m => new { INVFORCASTID = m.a.INVFORCASTID, INCFORCASTDETAIL = m.b.INCFORCASTDETAIL }).Distinct().ToList();

                        if (type == "ACTION")
                        {
                            datalist.Where(x => ief.Select(y => y.INVFORCASTID.ToString()).ToList().Contains(x.Value)).ToList().ForEach(z => z.Selected = true);
                            if (_A00ExistData.FRCSTOTHER != null)
                            {
                                //var OtherSelected = _Assetdatabasecontext.INVFORCAST.Where(x => x.INCFORCASTDETAIL.ToUpper() == "OTHERS").FirstOrDefault();
                                var OtherSelected = InvforcastList._InvforcastList.Where(x => x.INCFORCASTDETAIL.ToUpper() == "OTHERS").FirstOrDefault();
                                datalist.Where(x => x.Value == OtherSelected.INVFORCASTID.ToString()).ToList().ForEach(x => x.Selected = true);
                                ViewBag.isOtherSelected = "Y";
                            }
                            ViewBag.SKYLIST = datalist;
                            //string filePath = Path.Combine(serverpath.getFileUploadPath(),"A00");
                            string filePath = Path.Combine(serverpath.getFileUploadPath(), "\\A00\\");
                            string FileName = Path.GetFileNameWithoutExtension(_A00ExistData.ATTACHMENT);
                            string FileExtension = Path.GetExtension(_A00ExistData.ATTACHMENT);

                            FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;
                        }
                        else
                        {
                            List<SelectListItem> Vdatalist = new List<SelectListItem>();
                            foreach (var Datat4 in ief)
                            {
                                Vdatalist.Add(new SelectListItem { Text = Datat4.INCFORCASTDETAIL, Value = Datat4.INVFORCASTID.ToString() });
                            }
                            if (_A00ExistData.FRCSTOTHER != null)
                            {
                                //var OtherSelected = _Assetdatabasecontext.INVFORCAST.Where(x => x.INCFORCASTDETAIL.ToUpper() == "OTHERS").FirstOrDefault();
                                var OtherSelected = InvforcastList._InvforcastList.Where(x => x.INCFORCASTDETAIL.ToUpper() == "OTHERS").FirstOrDefault();
                                Vdatalist.Add(new SelectListItem
                                {
                                    Text = OtherSelected.INCFORCASTDETAIL.ToString(),
                                    Value = OtherSelected.INVFORCASTID.ToString(),
                                    Selected = true
                                });
                                ViewBag.isOtherSelected = "Y";
                            }
                            ViewBag.SKYLIST = Vdatalist;
                        }

                    }
                    else
                    {
                        List<SelectListItem> mlist = new List<SelectListItem>();
                        mlist.Add(new SelectListItem { Text = "Select", Value = "", Selected = true });
                        mlist.Add(new SelectListItem { Text = "Yes", Value = "10" });
                        mlist.Add(new SelectListItem { Text = "No", Value = "11" });
                        ViewBag.BudgetedValue = mlist;
                        ViewBag.List = new SelectList(budgByList, "ADORGLEVELID", "LEVELDESCRIP", 0);
                        ViewBag.SKY = new SelectList(iList, "SYKIID", "KICODE", 0);
                    }
                    #endregion

                    return View(objModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "A00");
                return View();
            }
        }
        public void SetSearchValue(long? OPERATIONID = null, long? DIVISIONID = null, long? DEPARTMENTID = null, long? SECTIONID = null, decimal? SYKIID = null, string ECODE = null, string EMPNAME = null, short? STATUSID = null)
        {
            ViewBag.OPERATIONID = OPERATIONID;
            ViewBag.DIVISIONID = DIVISIONID;
            ViewBag.DEPARTMENTID = DEPARTMENTID;
            ViewBag.SECTIONID = SECTIONID;
            ViewBag.SYKIID = SYKIID;
            ViewBag.ECODE = ECODE;
            ViewBag.EMPNAME = EMPNAME;
            ViewBag.STATUSID = STATUSID;
        }
        private void generateApprovalHistory(long A00DTLTBID)
        {
            //09-Sept-2021 change start
            List<ApprovalViewHistoryModel> history = new List<ApprovalViewHistoryModel>();
            _objA00SearchModel.a00dtltbid = A00DTLTBID;
            var approvalRecord = _objHomeA00.GetA00APPROVAL(_objA00SearchModel);
            var idList = new List<long?> { approvalRecord.DEPTHDID, approvalRecord.COORDDID, approvalRecord.DIVHDHDID, approvalRecord.EXECOHDID, approvalRecord.OHID, approvalRecord.PPCHOOHID };
            var empList = _objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE;
            var empData = empList.Where(x => idList.Contains(x.ADEMPCODE)).Select(y => new { Id = y.ADEMPCODE, Name = y.FIRSTNAME + " " + y.LASTNAME }).ToList();
            history.Add(
                       new ApprovalViewHistoryModel
                       {
                           DEPTHDID = approvalRecord.DEPTHDAPPDATE != null ? approvalRecord.DEPTHDID : null,
                           DEPTHDAPPDATE = approvalRecord.DEPTHDAPPDATE,
                           DEPTHDAPPTXT = approvalRecord.DEPTHDAPPTXT,
                           COORDDID = approvalRecord.COORDDID,
                           COORDAPPDATE = approvalRecord.COORDAPPDATE != null ? approvalRecord.COORDAPPDATE : null,
                           COORDAPPTXT = approvalRecord.COORDAPPTXT,
                           DIVHDHDID = approvalRecord.DIVHDAPPDATE != null ? approvalRecord.DIVHDHDID : null,
                           DIVHDAPPDATE = approvalRecord.DIVHDAPPDATE,
                           DIVHDAPPTXT = approvalRecord.DIVHDAPPTXT,
                           EXECOHDID = approvalRecord.EXECOAPPDATE != null ? approvalRecord.EXECOHDID : null,
                           EXECOAPPDATE = approvalRecord.EXECOAPPDATE,
                           EXECOAPPTXT = approvalRecord.EXECOAPPTXT,
                           OHID = approvalRecord.OHAPPDATE != null ? approvalRecord.OHID : null,
                           OHAPPDATE = approvalRecord.OHAPPDATE,
                           OHAPPTXT = approvalRecord.OHAPPTXT,
                           PPCHOOHID = approvalRecord.PPCHOOHDATE != null ? approvalRecord.PPCHOOHID : null,
                           PPCHOOHDATE = approvalRecord.PPCHOOHDATE,
                           PPCHOOHTXT = approvalRecord.PPCHOOHTXT
                       });
            history[0].DeptHeadName = empData.Where(x => x.Id == history[0].DEPTHDID).Select(y => y.Name).FirstOrDefault();
            history[0].CoOrdName = empData.Where(x => x.Id == history[0].COORDDID).Select(y => y.Name).FirstOrDefault();
            history[0].DivHeadName = empData.Where(x => x.Id == history[0].DIVHDHDID).Select(y => y.Name).FirstOrDefault();
            history[0].ExeCoOrdName = empData.Where(x => x.Id == history[0].EXECOHDID).Select(y => y.Name).FirstOrDefault();
            history[0].OpHeadName = empData.Where(x => x.Id == history[0].OHID).Select(y => y.Name).FirstOrDefault();
            history[0].PPCHOOHName = empData.Where(x => x.Id == history[0].PPCHOOHID).Select(y => y.Name).FirstOrDefault();

            ViewBag.ApprovalHistory = history;
            //09-Sept-2021 change end
        }
        [HttpPost]
        private ActionResult A00(A00ViewModel model, IFormCollection fc)
        {
            var redirectAction = "A00";
            try
            {
                if (validateInput(model, fc) == true)
                {
                    var A00InvForcast = _objHomeA00.GetA00INVFORCASTList();
                    #region
                    //A00DTLTB ALTD = new A00DTLTB();
                    InsertA00DtlTb ALTD = new InsertA00DtlTb();
                    //A00APPROVAL approval = new A00APPROVAL();
                    InsertA00APPROVAL approval = new InsertA00APPROVAL();

                    //string UploadPath = Server.MapPath(ConfigurationManager.AppSettings["Get_FileUpload_Path"].ToString() + "A00\\");
                    //string UploadPath = Path.Combine(serverpath.getFileUploadPath(),"A00");
                    string UploadPath = Path.Combine(serverpath.getFileUploadPath(), "A00");
                    var statusText = "saved";
                    string FileName = null;
                    if (model.ImageFile != null)
                    {
                        FileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                        string FileExtension = Path.GetExtension(model.ImageFile.FileName);

                        FileName = Convert.ToString(_sessionService.Get<string>("userID")) + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + "_" + DateTime.Now.Second.ToString() + "_" + DateTime.Now.Millisecond.ToString() + FileExtension;
                        model.ImagePath = Path.Combine(UploadPath , FileName);
                        //model.ImageFile.SaveAs(model.ImagePath);
                        string directoryPath = Path.GetDirectoryName(model.ImagePath);
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }
                        using (var stream = new FileStream(model.ImagePath, FileMode.Create))
                        {
                            model.ImageFile.CopyTo(stream);
                        }
                    }

                    string[] SKYLIST = null;
                    if (!string.IsNullOrEmpty(fc["SKYLIST"]))
                    {
                        SKYLIST = fc["SKYLIST"].ToString().Split(',');
                    }
                    try
                    {
                        #region Region-A1
                        if (model.A00DTLTBID != 0)
                        {
                            //ALTD = _Assetdatabasecontext.A00DTLTB.Where(x => x.A00DTLTBID == model.A00DTLTBID).FirstOrDefault();
                            _objA00SearchModel.a00dtltbid = model.A00DTLTBID;
                            var record = _objHomeA00.GetA00Dtl(_objA00SearchModel);
                            ALTD.A00DTLTBID = record.A00DTLTBID;
                            ALTD.ADDEDBY = record.ADDEDBY;
                            ALTD.DATEADDEDID = record.DATEADDEDID;
                            ALTD.OPERATION = record.OPERATION;
                            ALTD.DIVISION = record.DIVISION;
                            ALTD.DEPARTMENT = record.DEPARTMENT;
                            ALTD.SECTION = record.SECTION;


                            //approval = _Assetdatabasecontext.A00APPROVAL.Where(x => x.A00DTLTBID == model.A00DTLTBID).FirstOrDefault();
                            _objA00SearchModel.a00dtltbid = model.A00DTLTBID;
                            var approvalRecord1 = _objHomeA00.GetA00APPROVAL(_objA00SearchModel);
                            approval.A00APPROVALID = approvalRecord1.A00APPROVALID;
                            approval.A00APPROVALID = approvalRecord1.A00APPROVALID;
                            approval.A00DTLTBID = approvalRecord1.A00DTLTBID;
                            approval.DEPTHDID = approvalRecord1.DEPTHDID;
                            approval.DEPTHDAPPDATE = approvalRecord1.DEPTHDAPPDATE;
                            approval.DEPTHDAPPTXT = approvalRecord1.DEPTHDAPPTXT;
                            approval.COORDDID = approvalRecord1.COORDDID;
                            approval.COORDAPPDATE = approvalRecord1.COORDAPPDATE;
                            approval.COORDAPPTXT = approvalRecord1.COORDAPPTXT;
                            approval.DIVHDHDID = approvalRecord1.DIVHDHDID;
                            approval.DIVHDAPPDATE = approvalRecord1.DIVHDAPPDATE;
                            approval.DIVHDAPPTXT = approvalRecord1.DIVHDAPPTXT;
                            approval.EXECOHDID = approvalRecord1.EXECOHDID;
                            approval.EXECOAPPDATE = approvalRecord1.EXECOAPPDATE;
                            approval.EXECOAPPTXT = approvalRecord1.EXECOAPPTXT;
                            approval.OHID = approvalRecord1.OHID;
                            approval.OHAPPDATE = approvalRecord1.OHAPPDATE;
                            approval.OHAPPTXT = approvalRecord1.OHAPPTXT;
                        }
                        else
                        {
                            //ALTD.A00DTLTBID = _Assetdatabasecontext.A00DTLTB.AsEnumerable().OrderByDescending(x => x.A00DTLTBID).Select(x => x.A00DTLTBID).FirstOrDefault() + 1;
                            ALTD.A00DTLTBID = _objHomeA00.GetA00MaxA00DTLTBID().A00DTLTBID + 1;
                            //approval.A00APPROVALID = _Assetdatabasecontext.A00APPROVAL.AsEnumerable().OrderByDescending(x => x.A00APPROVALID).Select(x => x.A00APPROVALID).FirstOrDefault() + 1;
                            approval.A00APPROVALID = _objHomeA00.GetA00MaxA00APPROVAL().A00APPROVALID + 1;
                        }
                        ALTD.PRJCTTLE = model.Projecttitle;
                        ALTD.PRJCTTXT = model.Backgrounds;
                        ALTD.BUKPITXT = model.BusinessKPI;
                        ALTD.PURPSTXT = model.PurposeA00;
                        ALTD.TRGTINDCD = model.TargetA00;
                        ALTD.RQUMTTXT = model.RequirmentA00;
                        if (model.Budgeted == 10)
                        {
                            ALTD.BUDGETFLG = 1;
                            ALTD.BUDGETSYKIID = Convert.ToDecimal(model.KICODE);
                        }
                        else
                        {
                            ALTD.BUDGETFLG = 0;
                            ALTD.BUDGETSYKIID = null;
                        }
                        var kiList = _objHomeA00.GetA00SYKIList();
                        ALTD.SYKIID = kiList._SYKIList.Where(x => x.ACTIVE == 1).Select(y => y.SYKIID).FirstOrDefault();
                        //ALTD.SYKIID = _AssetDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(y => y.SYKIID).FirstOrDefault();
                        if (Convert.ToInt16(fc["LEVELDESCRIP"]) == 0)
                        {
                            ALTD.ADORGLEVELID = null;
                        }
                        else
                        {
                            ALTD.ADORGLEVELID = Convert.ToInt16(fc["LEVELDESCRIP"].ToString());
                        }

                        ALTD.STARTDT = convertDateToDDMM(fc["start"]);
                        ALTD.ENDDT = convertDateToDDMM(fc["end"]); // date to be stored as dd/mm/yyyy

                        if (SKYLIST.Contains("9"))
                        {
                            ALTD.FRCSTOTHER = model.Ifothers;
                        }
                        else
                        {
                            ALTD.FRCSTOTHER = "";
                        }
                        if (FileName != null)
                        {
                            ALTD.ATTACHMENT = FileName;
                        }
                        #endregion

                        if (!string.IsNullOrEmpty(fc["btnSubmit"]) && fc["btnSubmit"].ToString().ToUpper() == "SUBMIT")
                        {
                            ALTD.STATUSCD = 0; // will be updated after checking role in below section for Submit
                            statusText = "submitted";
                            ALTD.ADDEDBY = Convert.ToInt32(_sessionService.Get<string>("userID"));
                            redirectAction = "search";
                        }
                        else if (!string.IsNullOrEmpty(fc["btnSaveDraft"]) && fc["btnSaveDraft"].ToString().ToUpper() == "SAVE AS DRAFT")
                        {
                            ALTD.STATUSCD = 10;
                            ALTD.ADDEDBY = Convert.ToInt32(_sessionService.Get<string>("userID"));
                            redirectAction = null;
                        }
                        else if (!string.IsNullOrEmpty(fc["btnApprove"]) && fc["btnApprove"].ToString().ToUpper() == "APPROVE")
                        {
                            ALTD.STATUSCD = 1; // will be updated after checking role in below section for Approval
                            statusText = "approved";
                            redirectAction = "search";
                        }
                        else if (!string.IsNullOrEmpty(fc["btnSendBack"]) && fc["btnSendBack"].ToString().ToUpper() == "SEND BACK")
                        {
                            ALTD.STATUSCD = 3;
                            statusText = "sent back";
                            redirectAction = "search";
                        }
                        else if (!string.IsNullOrEmpty(fc["btnReject"]) && fc["btnReject"].ToString().ToUpper() == "REJECT")
                        {
                            ALTD.STATUSCD = 2; // will be updated after checking role in below section for Rejection
                            statusText = "rejected";
                            redirectAction = "search";
                        }

                        var listsession = Convert.ToInt32(_sessionService.Get<string>("userID"));

                        //var sykiData = _AssetDBContext.SYKI.Where(x => x.ACTIVE == 1).ToList();
                        var sykiData = kiList;
                        var sykiId = sykiData._SYKIList.Count > 0 ? sykiData._SYKIList[0].SYKIID : 0;
                        //var operation22 = _AssetDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == listsession && x.SYKI == sykiId).
                        //        Select(x => new { x.DEPARTMENTID, x.OPERATIONID, x.DIVISIONID, x.SECTIONID }).FirstOrDefault();

                        _objA00SearchModel.ADEMPCODE = listsession;
                        _objA00SearchModel.SYKI = sykiId;
                        var operation22 = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel);

                        if (ALTD.STATUSCD == 0 || ALTD.STATUSCD == 10)
                        {
                            #region Region-A2
                            try
                            {
                                if (operation22.DEPARTMENTID != null)
                                {
                                    ALTD.DEPARTMENT = Convert.ToInt16(operation22.DEPARTMENTID);
                                }
                                else
                                {
                                    ALTD.DEPARTMENT = 0;
                                }

                                if (operation22.OPERATIONID != null)
                                {
                                    ALTD.OPERATION = Convert.ToInt16(operation22.OPERATIONID);
                                }
                                else
                                {
                                    ALTD.OPERATION = 0;
                                }

                                if (operation22.DIVISIONID != null)
                                {
                                    ALTD.DIVISION = Convert.ToInt16(operation22.DIVISIONID);
                                }
                                else
                                {
                                    ALTD.DIVISION = 0;
                                }

                                if (operation22.SECTIONID != null)
                                {
                                    ALTD.SECTION = Convert.ToInt32(operation22.SECTIONID);
                                }
                                else
                                {
                                    ALTD.SECTION = 0;
                                }
                            }

                            catch (Exception ex)
                            {

                            }
                            #endregion
                        }
                        ALTD.ACTIVE = 1;
                        var currentTime = DateTime.Now;
                        if (model.A00DTLTBID == 0)
                        {
                            ALTD.DATEADDEDID = currentTime;
                        }

                        ALTD.LASTMODDATE = currentTime;
                        ALTD.LSTMODBYID = Convert.ToInt32(_sessionService.Get<string>("userID"));

                        if (statusText.Trim().ToUpper() == "SUBMITTED")
                        {
                            approval.DEPTHDID = null;
                            approval.DEPTHDAPPTXT = null;
                            approval.DEPTHDAPPDATE = null;

                            approval.COORDDID = null;
                            approval.COORDAPPTXT = null;
                            approval.COORDAPPDATE = null;

                            approval.DIVHDHDID = null;
                            approval.DIVHDAPPTXT = null;
                            approval.DIVHDAPPDATE = null;

                            approval.EXECOHDID = null;
                            approval.EXECOAPPTXT = null;
                            approval.EXECOAPPDATE = null;

                            approval.OHID = null;
                            approval.OHAPPTXT = null;
                            approval.OHAPPDATE = null;
                        }

                        bool isDeptHead = false, isCoOrdHead = false, isDivHead = false, isExeCoOrdHead = false, isOperatingHead = false;
                        approval.A00DTLTBID = ALTD.A00DTLTBID;
                        //Utilities.UserRole userData = new Utilities.UserData().getUserRole(listsession);
                        var userData = _objHomeA00.getUserRole(listsession, sykiId);
                        var userSentBackStatus = (short)(userData.isDeptHead ? 35 : (userData.isCoOrdHead ? 55 : (userData.isDivHead ? 75 : (userData.isExeCoOrdHead ? 95 : (userData.isOperatingHead ? 115 : 10)))));
                        isDeptHead = userData.isDeptHead;
                        isCoOrdHead = userData.isCoOrdHead;
                        isDivHead = userData.isDivHead;
                        isExeCoOrdHead = userData.isExeCoOrdHead;
                        isOperatingHead = userData.isOperatingHead;
                        var nextApproverToEmail = "";
                        #region Update Roles in Approval Table
                        var _adOrgLevelHead = _objHomeA00.GetA00_ADORGLEVELHEAD()._A00_ADORGLEVELHEAD;
                        if (!isOperatingHead) // current user role check
                        {
                            if (!isExeCoOrdHead) // current user role check
                            {
                                //var opHeadValue = _AssetDBContext.ADORGLEVELHEAD.Where(x =>
                                //                     x.ADORGLEVELID == operation22.OPERATIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                var opHeadValue = _adOrgLevelHead.Where(x =>
                                                     x.ADORGLEVELID == operation22.OPERATIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                if (!isDivHead) // current user role check
                                {
                                    //var divHeadValue = _AssetDBContext.ADORGLEVELHEAD.Where(x =>
                                    //        x.ADORGLEVELID == operation22.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                    var divHeadValue = _adOrgLevelHead.Where(x =>
                                            x.ADORGLEVELID == operation22.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                    if (!isCoOrdHead) // current user role check
                                    {
                                        if (!isDeptHead) // current user role check
                                        {
                                            if (!string.IsNullOrEmpty(fc["btnSubmit"]) && fc["btnSubmit"].ToString().ToUpper() == "SUBMIT")
                                            {
                                                //approval.DEPTHDID = _AssetDBContext.ADORGLEVELHEAD.Where(x => x.ADORGLEVELID == operation22.DEPARTMENTID && x.ISACTIVE == 1).Select(x => x.ADEMPCODE).FirstOrDefault(); // Dept Head as Approver //Change by Satyaveer on 11-Oct-2020
                                                approval.DEPTHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == operation22.DEPARTMENTID && x.ISACTIVE == 1).Select(x => x.ADEMPCODE).FirstOrDefault(); // Dept Head as Approver //Change by Satyaveer on 11-Oct-2020

                                                //nextApproverToEmail = _AssetDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == approval.DEPTHDID).Select(y => y.EMAILID).FirstOrDefault();
                                                _objA00SearchModel.DEPTHDID = approval.DEPTHDID;
                                                nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEE(_objA00SearchModel).EMAILID;
                                            }
                                            ALTD.STATUSCD = (short)(ALTD.STATUSCD == 0 ? 20 : 10);
                                        }
                                        else
                                        {
                                            if (ALTD.STATUSCD != 0 && ALTD.STATUSCD != 10) // approval table update for not save or submit
                                            {
                                                approval.DEPTHDID = listsession;
                                                approval.DEPTHDAPPDATE = currentTime;
                                                if (!string.IsNullOrEmpty(fc["btnApprove"]) && fc["btnApprove"].ToString().ToUpper() == "APPROVE")
                                                {
                                                    approval.DEPTHDAPPTXT = model.Remarks;
                                                }
                                                else if (!string.IsNullOrEmpty(fc["btnSendBack"]) && fc["btnSendBack"].ToString().ToUpper() == "SEND BACK")
                                                {
                                                    approval.DEPTHDAPPTXT = "SEND BACK." + model.Remarks;
                                                }
                                                else if (!string.IsNullOrEmpty(fc["btnReject"]) && fc["btnReject"].ToString().ToUpper() == "REJECT")
                                                {
                                                    approval.DEPTHDAPPTXT = "REJECTED." + model.Remarks;
                                                }

                                            }
                                            if (ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 || ALTD.STATUSCD == 10) // submit, approve or save
                                            {
                                                //approval.COORDDID = _AssetDBContext.ADORGCOORDINATOR.Where(x =>
                                                //x.ADORGLEVELID == operation22.OPERATIONID).Select(x => x.COORDINATOR).FirstOrDefault(); // Co-ordinator as Approver
                                                _objA00SearchModel.OPERATIONID = operation22.OPERATIONID;
                                                approval.COORDDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).COORDINATOR; // Co-ordinator as Approver


                                                //nextApproverToEmail = _AssetDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == approval.COORDDID).Select(y => y.EMAILID).FirstOrDefault();
                                                _objA00SearchModel.COORDDID = approval.COORDDID;
                                                nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEE(_objA00SearchModel).EMAILID;

                                                ALTD.STATUSCD = (short)(ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 ? 40 : 10);
                                                if (approval.COORDDID == null)
                                                {
                                                    approval.DIVHDHDID = divHeadValue;
                                                    //nextApproverToEmail = _AssetDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == approval.DIVHDHDID).Select(y => y.EMAILID).FirstOrDefault();
                                                    _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                                    nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEE(_objA00SearchModel).EMAILID;
                                                    ALTD.STATUSCD = ALTD.STATUSCD == 40 ? (short)60 : ALTD.STATUSCD;
                                                }
                                            }
                                            else if (ALTD.STATUSCD == 2) //dept head reject
                                            {
                                                ALTD.STATUSCD = 30;
                                            }
                                            else if (ALTD.STATUSCD == 3) //dept head send back
                                            {
                                                ALTD.STATUSCD = userSentBackStatus;//
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (ALTD.STATUSCD != 0 && ALTD.STATUSCD != 10)
                                        {
                                            approval.COORDAPPDATE = currentTime;
                                            approval.COORDDID = listsession;
                                            if (!string.IsNullOrEmpty(fc["btnApprove"]) && fc["btnApprove"].ToString().ToUpper() == "APPROVE")
                                            {
                                                approval.COORDAPPTXT = model.Remarks;
                                            }
                                            else if (!string.IsNullOrEmpty(fc["btnSendBack"]) && fc["btnSendBack"].ToString().ToUpper() == "SEND BACK")
                                            {
                                                approval.COORDAPPTXT = "SEND BACK." + model.Remarks;
                                            }
                                            else if (!string.IsNullOrEmpty(fc["btnReject"]) && fc["btnReject"].ToString().ToUpper() == "REJECT")
                                            {
                                                approval.COORDAPPTXT = "REJECTED." + model.Remarks;
                                            }
                                        }
                                        if (ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 || ALTD.STATUSCD == 10)
                                        {
                                            approval.DIVHDHDID = divHeadValue; // Div Head as Approver

                                            //nextApproverToEmail = _AssetDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == approval.DIVHDHDID).Select(y => y.EMAILID).FirstOrDefault();
                                            _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEE(_objA00SearchModel).EMAILID;

                                            ALTD.STATUSCD = (short)(ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 ? 60 : 10);
                                        }
                                        else if (ALTD.STATUSCD == 2) //co ord reject
                                        {
                                            ALTD.STATUSCD = 50;
                                        }
                                        else if (ALTD.STATUSCD == 3) //co ord send back
                                        {
                                            ALTD.STATUSCD = userSentBackStatus; //55;
                                        }
                                    }
                                }
                                else
                                {
                                    if (ALTD.STATUSCD != 0 && ALTD.STATUSCD != 10)
                                    {
                                        approval.DIVHDAPPDATE = currentTime;
                                        approval.DIVHDHDID = listsession;
                                        if (!string.IsNullOrEmpty(fc["btnApprove"]) && fc["btnApprove"].ToString().ToUpper() == "APPROVE")
                                        {
                                            approval.DIVHDAPPTXT = model.Remarks;
                                        }
                                        else if (!string.IsNullOrEmpty(fc["btnSendBack"]) && fc["btnSendBack"].ToString().ToUpper() == "SEND BACK")
                                        {
                                            approval.DIVHDAPPTXT = "SEND BACK." + model.Remarks;
                                        }
                                        else if (!string.IsNullOrEmpty(fc["btnReject"]) && fc["btnReject"].ToString().ToUpper() == "REJECT")
                                        {
                                            approval.DIVHDAPPTXT = "REJECTED." + model.Remarks;
                                        }
                                    }
                                    if (ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 || ALTD.STATUSCD == 10) // div head approval
                                    {
                                        //approval.EXECOHDID = _AssetDBContext.ADORGCOORDINATOR.Where(x =>
                                        //        x.ADORGLEVELID == operation22.OPERATIONID).Select(x => x.EXECOORDINATOR).FirstOrDefault();
                                        _objA00SearchModel.OPERATIONID = operation22.OPERATIONID;
                                        approval.EXECOHDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).EXECOORDINATOR; // Co-ordinator as Approver


                                        //nextApproverToEmail = _AssetDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == approval.EXECOHDID).Select(y => y.EMAILID).FirstOrDefault();
                                        _objA00SearchModel.EXECOHDID = approval.EXECOHDID;
                                        nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEE(_objA00SearchModel).EMAILID;

                                        ALTD.STATUSCD = (short)(ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 ? 80 : 10);
                                        if (approval.EXECOHDID == null)
                                        {
                                            //07-Sept-2021 change start
                                            //approval.OHID = opHeadValue;

                                            if (_objHomeA00.getDataFromAdorgcoordinator(ALTD.OPERATION) != null)
                                            {
                                                approval.OHID = _objHomeA00.getDataFromAdorgcoordinator(ALTD.OPERATION).OPHEAD;
                                            }
                                            else
                                            {
                                                approval.OHID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == ALTD.OPERATION).Select(x => x.ADEMPCODE).FirstOrDefault();
                                            }
                                            //07-Sept-2021 change end


                                            //nextApproverToEmail = _AssetDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == approval.OHID).Select(y => y.EMAILID).FirstOrDefault();
                                            _objA00SearchModel.OHID = approval.OHID;
                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEE(_objA00SearchModel).EMAILID;

                                            ALTD.STATUSCD = ALTD.STATUSCD == 80 ? (short)100 : ALTD.STATUSCD;
                                        }
                                    }
                                    else if (ALTD.STATUSCD == 2) //div head reject
                                    {
                                        ALTD.STATUSCD = 70;
                                    }
                                    else if (ALTD.STATUSCD == 3) //div head send back
                                    {
                                        ALTD.STATUSCD = userSentBackStatus; //75;
                                    }
                                }
                            }
                            else
                            {
                                if (ALTD.STATUSCD != 0 && ALTD.STATUSCD != 10)
                                {
                                    approval.EXECOAPPDATE = currentTime;
                                    approval.EXECOHDID = listsession;
                                    if (!string.IsNullOrEmpty(fc["btnApprove"]) && fc["btnApprove"].ToString().ToUpper() == "APPROVE")
                                    {
                                        approval.EXECOAPPTXT = model.Remarks;
                                    }
                                    else if (!string.IsNullOrEmpty(fc["btnSendBack"]) && fc["btnSendBack"].ToString().ToUpper() == "SEND BACK")
                                    {
                                        approval.EXECOAPPTXT = "SEND BACK." + model.Remarks;
                                    }
                                    else if (!string.IsNullOrEmpty(fc["btnReject"]) && fc["btnReject"].ToString().ToUpper() == "REJECT")
                                    {
                                        approval.EXECOAPPTXT = "REJECTED." + model.Remarks;
                                    }
                                }
                                if (ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 || ALTD.STATUSCD == 10) // ex co ord approval
                                {
                                    //07-Sept-2021 change start
                                    //approval.OHID = _adOrgLevelHead.Where(x =>
                                    //                 x.ADORGLEVELID == operation22.OPERATIONID).Select(x => x.ADEMPCODE).FirstOrDefault();

                                    if (_objHomeA00.getDataFromAdorgcoordinator(ALTD.OPERATION) != null)
                                    {
                                        approval.OHID = _objHomeA00.getDataFromAdorgcoordinator(ALTD.OPERATION).OPHEAD;
                                    }
                                    else
                                    {
                                        approval.OHID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == ALTD.OPERATION).Select(x => x.ADEMPCODE).FirstOrDefault();
                                    }
                                    //07-Sept-2021 change end

                                    _objA00SearchModel.OHID = approval.OHID;
                                    nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEE(_objA00SearchModel).EMAILID;

                                    ALTD.STATUSCD = (short)(ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 ? 100 : 10);
                                }
                                else if (ALTD.STATUSCD == 2) //ex co ord reject
                                {
                                    ALTD.STATUSCD = 90;
                                }
                                else if (ALTD.STATUSCD == 3) //ex co ord send back
                                {
                                    ALTD.STATUSCD = userSentBackStatus;//95;
                                }
                            }
                        }
                        else
                        {
                            if (ALTD.STATUSCD != 0 && ALTD.STATUSCD != 10)
                            {
                                approval.OHAPPDATE = currentTime;
                                approval.OHID = listsession;
                                if (!string.IsNullOrEmpty(fc["btnApprove"]) && fc["btnApprove"].ToString().ToUpper() == "APPROVE")
                                {
                                    approval.OHAPPTXT = model.Remarks;
                                }

                                else if (!string.IsNullOrEmpty(fc["btnSendBack"]) && fc["btnSendBack"].ToString().ToUpper() == "SEND BACK")
                                {
                                    approval.OHAPPTXT = "SEND BACK." + model.Remarks;
                                }
                                else if (!string.IsNullOrEmpty(fc["btnReject"]) && fc["btnReject"].ToString().ToUpper() == "REJECT")
                                {
                                    approval.OHAPPTXT = "REJECTED." + model.Remarks;
                                }
                            }
                            if (ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 || ALTD.STATUSCD == 10) // Op Head approval
                            {
                                ALTD.STATUSCD = (short)(ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 ? 105 : 10);
                            }
                            else if (ALTD.STATUSCD == 2) //OH reject
                            {
                                ALTD.STATUSCD = 110;
                            }
                            else if (ALTD.STATUSCD == 3) //OH send back
                            {
                                ALTD.STATUSCD = userSentBackStatus;//115;
                            }
                        }
                        #endregion

                        //Remove all approval history while requister press Save As Draft button by satyaveer on 12-Oct-2020  -  Start
                        if (statusText.Trim().ToUpper() == "SAVED")
                        {
                            approval.DEPTHDID = null;
                            approval.DEPTHDAPPTXT = null;
                            approval.DEPTHDAPPDATE = null;

                            approval.COORDDID = null;
                            approval.COORDAPPTXT = null;
                            approval.COORDAPPDATE = null;

                            approval.DIVHDHDID = null;
                            approval.DIVHDAPPTXT = null;
                            approval.DIVHDAPPDATE = null;

                            approval.EXECOHDID = null;
                            approval.EXECOAPPTXT = null;
                            approval.EXECOAPPDATE = null;

                            approval.OHID = null;
                            approval.OHAPPTXT = null;
                            approval.OHAPPDATE = null;
                        }
                        //Remove all approval history while requister press Save As Draft button by satyaveer on 12-Oct-2020  -  End

                        if (model.A00DTLTBID == 0)
                        {
                            //    _Assetdatabasecontext.A00DTLTB.Add(ALTD);
                            model.A00DTLTBID = ALTD.A00DTLTBID;
                            //    _Assetdatabasecontext.SaveChanges();

                            //    _Assetdatabasecontext.A00APPROVAL.Add(approval);
                        }
                        //_Assetdatabasecontext.SaveChanges();

                        _objHomeA00.InsertUpdateA00Detail(ALTD);
                        _objHomeA00.InsertUpdateA00APPROVAL(approval);

                        var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(ALTD.ADDEDBY);
                        if (!string.IsNullOrEmpty(nextApproverToEmail))
                        {
                            sendMailToApprover(nextApproverToEmail, ALTD.PRJCTTLE, ALTD.DATEADDEDID.Date, requesterDetails.Name, requesterDetails.EmpCode);
                        }
                        List<long> invForecastList = SKYLIST.Where(x => x != "9").Select(y => Convert.ToInt64(y)).ToList();
                        //var allPrevRecIfExists = _Assetdatabasecontext.A00INVFORCAST.Where(x => x.A00DTLTBID == model.A00DTLTBID && x.ACTIVE == 1).
                        //        Select(y => y.INVFORCASTID).ToList();
                        var allPrevRecIfExists = A00InvForcast._A00InvforcastList.Where(x => x.A00DTLTBID == model.A00DTLTBID && x.ACTIVE == 1).Select(y => y.INVFORCASTID).ToList();
                        allPrevRecIfExists = allPrevRecIfExists.Count > 0 ? allPrevRecIfExists : new List<long>();
                        //insert data into a00invforcast
                        if (invForecastList.Count > 0)
                        {
                            var newSelected = invForecastList.Where(x => !allPrevRecIfExists.Contains(x)).ToList();
                            var duplicates = invForecastList.Where(x => allPrevRecIfExists.Contains(x)).ToList();
                            //var inactiveRecords = _Assetdatabasecontext.A00INVFORCAST.Where(x => !duplicates.Contains(x.INVFORCASTID) && !newSelected.Contains(x.INVFORCASTID) && x.A00DTLTBID == model.A00DTLTBID && x.ACTIVE == 1).ToList();
                            var inactiveRecords = A00InvForcast._A00InvforcastList.Where(x => !duplicates.Contains(x.INVFORCASTID) && !newSelected.Contains(x.INVFORCASTID) && x.A00DTLTBID == model.A00DTLTBID && x.ACTIVE == 1).ToList();
                            foreach (var item in inactiveRecords)
                            {
                                item.ACTIVE = 0;
                            }
                            //long forecasuniqueid = _Assetdatabasecontext.A00INVFORCAST.AsEnumerable().OrderByDescending(x => x.A00INVFORCASTID).Select(x => x.A00INVFORCASTID).FirstOrDefault() + 1;
                            long forecasuniqueid = A00InvForcast._A00InvforcastList.AsEnumerable().OrderByDescending(x => x.A00INVFORCASTID).Select(x => x.A00INVFORCASTID).FirstOrDefault() + 1;
                            for (int i = 0; i < newSelected.Count; i++)
                            {

                                //A00INVFORCAST forecastobj = new A00INVFORCAST();
                                InsertA00INVFORCAST forecastobj = new InsertA00INVFORCAST();
                                forecastobj.A00INVFORCASTID = forecasuniqueid + i;
                                forecastobj.A00DTLTBID = ALTD.A00DTLTBID;
                                forecastobj.DATEADDEDID = currentTime;
                                forecastobj.INVFORCASTID = newSelected[i];
                                forecastobj.ACTIVE = 1;

                                _objHomeA00.InsertUpdateA00INVFORCAST(forecastobj);
                                //_Assetdatabasecontext.A00INVFORCAST.Add(forecastobj);
                            }
                            //_Assetdatabasecontext.SaveChanges();
                        }
                        else
                        {
                            //var existingActiveRec = _Assetdatabasecontext.A00INVFORCAST.Where(x => x.A00DTLTBID == model.A00DTLTBID && x.ACTIVE == 1).ToList();
                            var existingActiveRec = A00InvForcast._A00InvforcastList.Where(x => x.A00DTLTBID == model.A00DTLTBID && x.ACTIVE == 1).ToList();
                            foreach (var item in existingActiveRec)
                            {
                                item.ACTIVE = 0;
                            }
                            //_Assetdatabasecontext.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Msg = "Error : " + ex.Message.ToString();
                        return View("Error");
                    }
                    #endregion
                    ViewBag.Msg = "Record " + statusText + " successfully.";
                    //if (statusText == "submitted")
                    //{
                    //approval = _Assetdatabasecontext.A00APPROVAL.Where(x => x.A00DTLTBID == approval.A00APPROVALID).FirstOrDefault();
                    //}
                }
                else
                {
                    if (redirectAction == "search")
                    {
                        return RedirectToAction("SearchbACK", "A00Submit", new { OPERATIONID = Convert.ToInt32(fc["hdnOPERATIONID"].ToString()), DIVISIONID = Convert.ToInt32(fc["hdnDIVISIONID"].ToString()), DEPARTMENTID = Convert.ToInt32(fc["hdnDEPARTMENTID"].ToString()), SECTIONID = Convert.ToInt32(fc["hdnSECTIONID"].ToString()), SYKIID = Convert.ToDecimal(fc["hdnSYKIID"].ToString()), ECODE = fc["hdnECODE"], EMPNAME = fc["hdnEMPNAME"], STATUSID = Convert.ToInt16(fc["hdnSTATUSID"].ToString()) });
                    }
                    else
                    {
                        if (redirectAction != null)
                        {
                            return RedirectToAction("A00", new { OPERATIONID = (fc["hdnOPERATIONID"] != "" ? Convert.ToInt32(fc["hdnOPERATIONID"].ToString()) : 0), DIVISIONID = (fc["hdnDIVISIONID"] != "" ? Convert.ToInt32(fc["hdnDIVISIONID"].ToString()) : 0), DEPARTMENTID = (fc["hdnDEPARTMENTID"] != "" ? Convert.ToInt32(fc["hdnDEPARTMENTID"].ToString()) : 0), SECTIONID = (fc["hdnSECTIONID"] != "" ? Convert.ToInt32(fc["hdnSECTIONID"].ToString()) : 0), SYKIID = (fc["hdnSYKIID"] != "" ? Convert.ToDecimal(fc["hdnSYKIID"].ToString()) : 0), ECODE = fc["hdnECODE"], EMPNAME = fc["hdnEMPNAME"], STATUSID = (fc["hdnSTATUSID"] != "" ? Convert.ToInt16(fc["hdnSTATUSID"].ToString()) : 0) });
                        }
                        else
                        {
                            return RedirectToAction("A00", new { A00DTLTBID = model.A00DTLTBID, Type = "Action", OPERATIONID = (fc["hdnOPERATIONID"] != "" ? Convert.ToInt32(fc["hdnOPERATIONID"].ToString()) : 0), DIVISIONID = (fc["hdnDIVISIONID"] != "" ? Convert.ToInt32(fc["hdnDIVISIONID"].ToString()) : 0), DEPARTMENTID = (fc["hdnDEPARTMENTID"] != "" ? Convert.ToInt32(fc["hdnDEPARTMENTID"].ToString()) : 0), SECTIONID = (fc["hdnSECTIONID"] != "" ? Convert.ToInt32(fc["hdnSECTIONID"].ToString()) : 0), SYKIID = (fc["hdnSYKIID"] != "" ? Convert.ToDecimal(fc["hdnSYKIID"].ToString()) : 0), ECODE = fc["hdnECODE"], EMPNAME = fc["hdnEMPNAME"], STATUSID = (fc["hdnSTATUSID"] != "" ? Convert.ToInt16(fc["hdnSTATUSID"].ToString()) : 0) });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Msg = "Error : " + ex.Message.ToString();
                _logger.LogError(ex, "A00");
                return View("Error");
            }

            if (redirectAction != null && redirectAction != "search")
            {
                return RedirectToAction("A00", new { OPERATIONID = (fc["hdnOPERATIONID"] != "" ? Convert.ToInt32(fc["hdnOPERATIONID"].ToString()) : 0), DIVISIONID = (fc["hdnDIVISIONID"] != "" ? Convert.ToInt32(fc["hdnDIVISIONID"].ToString()) : 0), DEPARTMENTID = (fc["hdnDEPARTMENTID"] != "" ? Convert.ToInt32(fc["hdnDEPARTMENTID"].ToString()) : 0), SECTIONID = (fc["hdnSECTIONID"] != "" ? Convert.ToInt32(fc["hdnSECTIONID"].ToString()) : 0), SYKIID = (fc["hdnSYKIID"] != "" ? Convert.ToDecimal(fc["hdnSYKIID"].ToString()) : 0), ECODE = fc["hdnECODE"], EMPNAME = fc["hdnEMPNAME"], STATUSID = (fc["hdnSTATUSID"] != "" ? Convert.ToInt16(fc["hdnSTATUSID"].ToString()) : 0) });
            }
            else
            {
                if (redirectAction == "search")
                {
                    return RedirectToAction("SearchBack", "A00Submit", new { returnMsgOnSearch = ViewBag.Msg, OPERATIONID = (fc["hdnOPERATIONID"] != "" ? Convert.ToInt32(fc["hdnOPERATIONID"].ToString()) : 0), DIVISIONID = (fc["hdnDIVISIONID"] != "" ? Convert.ToInt32(fc["hdnDIVISIONID"].ToString()) : 0), DEPARTMENTID = (fc["hdnDEPARTMENTID"] != "" ? Convert.ToInt32(fc["hdnDEPARTMENTID"].ToString()) : 0), SECTIONID = (fc["hdnSECTIONID"] != "" ? Convert.ToInt32(fc["hdnSECTIONID"].ToString()) : 0), SYKIID = (fc["hdnSYKIID"] != "" ? Convert.ToDecimal(fc["hdnSYKIID"].ToString()) : 0), ECODE = fc["hdnECODE"], EMPNAME = fc["hdnEMPNAME"], STATUSID = (fc["hdnSTATUSID"] != "" ? Convert.ToInt16(fc["hdnSTATUSID"].ToString()) : 0) });
                }
                else
                {
                    return RedirectToAction("A00", new { A00DTLTBID = model.A00DTLTBID, returnMsg = ViewBag.Msg, Type = "Action", OPERATIONID = (fc["hdnOPERATIONID"] != "" ? Convert.ToInt32(fc["hdnOPERATIONID"].ToString()) : 0), DIVISIONID = (fc["hdnDIVISIONID"] != "" ? Convert.ToInt32(fc["hdnDIVISIONID"].ToString()) : 0), DEPARTMENTID = (fc["hdnDEPARTMENTID"] != "" ? Convert.ToInt32(fc["hdnDEPARTMENTID"].ToString()) : 0), SECTIONID = (fc["hdnSECTIONID"] != "" ? Convert.ToInt32(fc["hdnSECTIONID"].ToString()) : 0), SYKIID = (fc["hdnSYKIID"] != "" ? Convert.ToDecimal(fc["hdnSYKIID"].ToString()) : 0), ECODE = fc["hdnECODE"], EMPNAME = fc["hdnEMPNAME"], STATUSID = (fc["hdnSTATUSID"] != "" ? Convert.ToInt16(fc["hdnSTATUSID"].ToString()) : 0) });
                }
            }
        }
        public bool validateInput(A00ViewModel model, IFormCollection fc)
        {
            bool _isValid = true;
            if (model.ImageFile != null && _isValid == true)
            {
                string FileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                string FileExtension = Path.GetExtension(model.ImageFile.FileName);
                if (FileExtension != ".pdf")
                {
                    ViewBag.Msg = "Only pdf file allowed";
                    _isValid = false;
                }
            }
            else
            {
                if (fc["isFileExist"] == "N")
                {
                    ViewBag.Msg = "Please upload AS IS & TO BE process file";
                    _isValid = false;
                }
                else
                {
                    _isValid = true;
                }
            }

            if (fc["Projecttitle"].ToString().Trim() == "" && _isValid == true)
            {
                ViewBag.Msg = "Project title can't be left blank";
                _isValid = false;
            }
            if (fc["Backgrounds"].ToString().Trim() == "" && _isValid == true)
            {
                ViewBag.Msg = "Background & Situation Analysis can't be left blank";
                _isValid = false;
            }
            if (fc["BusinessKPI"].ToString().Trim() == "" && _isValid == true)
            {
                ViewBag.Msg = "Business KPI can't be left blank";
                _isValid = false;
            }
            if (fc["Budgeted"].ToString().Trim() == "" && _isValid == true)
            {
                ViewBag.Msg = "Please select Budgeted(Y/N)";
                _isValid = false;
            }
            if (_isValid == true)
            {
                DateTime dTCurrent = DateTime.Now;
                DateTime dtFullStartDate = convertDateToDDMM(fc["start"]);
                if ((dTCurrent - dtFullStartDate).TotalDays > 0)
                {
                    ViewBag.Msg = "Start date must be greter than or equal to today";
                    _isValid = false;
                }

                if (fc["end"].ToString().Trim() == "" && _isValid == true)
                {
                    ViewBag.Msg = "Please select end date";
                    _isValid = false;
                }

                if (_isValid == true)
                {
                    DateTime dtFullEndDate = convertDateToDDMM(fc["end"]);
                    if ((dtFullStartDate - dtFullEndDate).TotalDays > 0)
                    {
                        ViewBag.Msg = "End date must be greater than or equal to Start Date";
                        _isValid = false;
                    }
                }
            }
            if (fc["Budgeted"].ToString().Trim() == "10")
            {
                if (fc["KICODE"].ToString().Trim() == "" && _isValid == true)
                {
                    ViewBag.Msg = "Please select ki";
                    _isValid = false;
                }
                if (fc["LEVELDESCRIP"].ToString().Trim() == "" && _isValid == true)
                {
                    ViewBag.Msg = "Please select Budgeted by";
                    _isValid = false;
                }
            }

            if (fc["start"].ToString().Trim() == "" && _isValid == true)
            {
                ViewBag.Msg = "Please select start date";
                _isValid = false;
            }
            string[] SKYLIST = null;
            if (string.IsNullOrEmpty(fc["SKYLIST"]) && _isValid == true)
            {
                ViewBag.Msg = "Please select Investment Effect Forecast";
                _isValid = false;
            }
            else if (_isValid == true)
            {
                SKYLIST = fc["SKYLIST"].ToString().Split(',');
                if (SKYLIST.Contains("Others") && _isValid == true)
                {
                    if (fc["Ifothers"].ToString().Trim() == "" && _isValid == true)
                    {
                        ViewBag.Msg = "If others selected, please specify others details";
                        _isValid = false;
                    }
                }
            }

            if (fc["PurposeA00"].ToString().Trim() == "" && _isValid == true)
            {
                ViewBag.Msg = "A00 purpose can't be left blank";
                _isValid = false;
            }

            if (fc["TargetA00"].ToString().Trim() == "" && _isValid == true)
            {
                ViewBag.Msg = "A00 target can't be left blank";
                _isValid = false;
            }

            if (fc["RequirmentA00"].ToString().Trim() == "" && _isValid == true)
            {
                ViewBag.Msg = "A00 requirement can't be left blank";
                _isValid = false;
            }
            return _isValid;
        }
        public static bool IsDateBeforeOrToday(string input)
        {
            DateTime pDate;
            if (!DateTime.TryParseExact(input, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out pDate))
            {
                return false;
            }
            return DateTime.Today <= pDate;
        }

        public ActionResult DownloadFile(string FileName)
        {
            //string path = Server.MapPath(ConfigurationManager.AppSettings["Get_FileUpload_Path"].ToString() + "A00\\");
            string path = Path.Combine(serverpath.getFileUploadPath(), "A00", FileName);

            //  byte[] fileBytes = System.IO.File.ReadAllBytes(path + FileName);

            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);


            // return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
        }


        public ActionResult BackToSearch(long? OPERATIONID = null, long? DIVISIONID = null, long? DEPARTMENTID = null, long? SECTIONID = null, decimal? SYKIID = null, string ECODE = null, string EMPNAME = null, short? STATUSID = null)
        {
            return RedirectToAction("SearchBack", "A00Submit", new { OPERATIONID = OPERATIONID, DIVISIONID = DIVISIONID, DEPARTMENTID = DEPARTMENTID, SECTIONID = SECTIONID, SYKIID = SYKIID, ECODE = ECODE, EMPNAME = EMPNAME, STATUSID = STATUSID });
        }

        private DateTime convertDateToDDMM(string input)
        {
            //Change start on 22-July-2021
            if (input.Trim().ToString().Contains("-") == true)
            {
                string[] splittedDate = input.Trim().ToString().Replace("-", "/").Split('/');
                string ddmmDate = splittedDate[0].PadLeft(0, '2').ToString() + "/" + splittedDate[1].PadLeft(0, '2').ToString() + "/" + splittedDate[2].PadLeft(0, '2').ToString();
                return DateTime.ParseExact(ddmmDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                string[] splittedDate = input.Trim().ToString().Split('/');
                string ddmmDate = splittedDate[0].PadLeft(0, '2').ToString() + "/" + splittedDate[1].PadLeft(0, '2').ToString() + "/" + splittedDate[2].PadLeft(0, '2').ToString();
                return DateTime.ParseExact(ddmmDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            //Change end on 22-July-2021
        }

        private void sendMailToApprover(string receiver, string project_title, DateTime requested_on, string RequesterName, string RequesterEMPCode)
        {
            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = receiver;// user mail id
            string strSubject = "A00 Request from - " + RequesterName + ", Employee Code - " + RequesterEMPCode;
            string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                             "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>A00 Request from  " + RequesterName + " - Emp Code (" + RequesterEMPCode + ")</font></b></td>" +
                             "</tr><tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td width=125 height=22 valign=top>Employee Code</td><td width=389 valign=top>" + RequesterEMPCode + "</td>" +
                             "</tr><tr><td width=125 height=23 valign=top>Employee Name</td><td width=389 valign=top>" + RequesterName + "</td>" +
                             " </tr><tr> " +
                             "<td width=125 valign=top>Project Title</td><td width=389 valign=top>" + project_title + "</td></tr><tr><td width=125 valign=top>Requested on</td>" +
                             "<td width=389 valign=top>" + requested_on.Date.ToString("D") + "</td></tr>" +
                             "<tr><td valign=top colspan=2>Please login <a href='https://portal.honda2wheelersindia.com/SSO'> Employee Portal</a> for approval process.</td></tr>" +
                             "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }
        private void sendMailAOORequestApproved(string receiver, string project_title, DateTime requested_on, string RequesterName, string RequesterEMPCode)
        {
            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = receiver;// user mail id
            string strSubject = "A00 Request has been approved by - " + _sessionService.Get<string>("userName") + ", Employee Code - " + _sessionService.Get<string>("userID");
            string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                             "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>A00 Request has been approved by - " + _sessionService.Get<string>("userName") + " - Emp Code (" + _sessionService.Get<string>("userID") + ")</font></b></td>" +
                             "</tr><tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td width=125 height=22 valign=top>Employee Code</td><td width=389 valign=top>" + RequesterEMPCode + "</td>" +
                             "</tr><tr><td width=125 height=23 valign=top>Employee Name</td><td width=389 valign=top>" + RequesterName + "</td>" +
                             " </tr><tr> " +
                             "<td width=125 valign=top>Project Title</td><td width=389 valign=top>" + project_title + "</td></tr><tr><td width=125 valign=top>Requested on</td>" +
                             "<td width=389 valign=top>" + requested_on.Date.ToString("D") + "</td></tr>" +
                             "<tr><td valign=top colspan=2>Please login <a href='https://portal.honda2wheelersindia.com/SSO'> Employee Portal</a> for approval process.</td></tr>" +
                             "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }
        private void sendMailToRequesterForReject(string receiver, string project_title, DateTime requested_on, string RequesterName, string RequesterEMPCode)
        {
            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = receiver;// user mail id
            string strSubject = "A00 Request rejected by - " + _sessionService.Get<string>("userName") + ", Employee Code - " + _sessionService.Get<string>("userID");
            string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                             "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>A00 Request rejected by - " + _sessionService.Get<string>("userName") + " - Emp Code (" + _sessionService.Get<string>("userID") + ")</font></b></td>" +
                             "</tr><tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td width=125 height=22 valign=top>Employee Code</td><td width=389 valign=top>" + RequesterEMPCode + "</td>" +
                             "</tr><tr><td width=125 height=23 valign=top>Employee Name</td><td width=389 valign=top>" + RequesterName + "</td>" +
                             " </tr><tr> " +
                             "<td width=125 valign=top>Project Title</td><td width=389 valign=top>" + project_title + "</td></tr><tr><td width=125 valign=top>Rejected on</td>" +
                             "<td width=389 valign=top>" + requested_on.Date.ToString("D") + "</td></tr>" +
                             "<tr><td valign=top colspan=2>Please login <a href='https://portal.honda2wheelersindia.com/SSO'> Employee Portal</a> for more details.</td></tr>" +
                             "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }

        private void sendMailToRequesterForSendBack(string receiver, string project_title, DateTime requested_on, string RequesterName, string RequesterEMPCode)
        {
            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = receiver;// user mail id
            string strSubject = "A00 Request send back by - " + _sessionService.Get<string>("userName") + ", Employee Code - " + _sessionService.Get<string>("userID");
            string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                             "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>A00 Request send back by - " + _sessionService.Get<string>("userName") + " - Emp Code (" + _sessionService.Get<string>("userID") + ")</font></b></td>" +
                             "</tr><tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td width=125 height=22 valign=top>Employee Code</td><td width=389 valign=top>" + RequesterEMPCode + "</td>" +
                             "</tr><tr><td width=125 height=23 valign=top>Employee Name</td><td width=389 valign=top>" + RequesterName + "</td>" +
                             " </tr><tr> " +
                             "<td width=125 valign=top>Project Title</td><td width=389 valign=top>" + project_title + "</td></tr><tr><td width=125 valign=top>Send Back on</td>" +
                             "<td width=389 valign=top>" + requested_on.Date.ToString("D") + "</td></tr>" +
                             "<tr><td valign=top colspan=2>Please login <a href='https://portal.honda2wheelersindia.com/SSO'> Employee Portal</a> for more details.</td></tr>" +
                             "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }


        #region DepartmentA00-Controller  
        public JsonResult GetA00RequestForApproval(short StatusId = 1)
        {
            A00ViewModel model = new A00ViewModel();
            try
            {
                int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

                //var kiData1 = _objHomeA00.GetA00SYKIList();
                //var activeKi = kiData1._SYKIList.Where(x => x.ACTIVE == 1).FirstOrDefault();
                //_objA00SearchModel.ADEMPCODE = userId;
                //_objA00SearchModel.SYKI = activeKi.SYKIID;                
                //var data = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel);
                //if (data != null)
                //{
                //    SearchParameterList paramsData = new SearchParameterList();
                //    paramsData.OPERATIONID = data.OPERATIONID != null ? data.OPERATIONID : 0;
                //    paramsData.DIVISIONID = data.DIVISIONID != null ? data.DIVISIONID : 0;
                //    paramsData.DEPARTMENTID = data.DEPARTMENTID != null ? data.DEPARTMENTID : 0;
                //    paramsData.SECTIONID = data.SECTIONID != null ? data.SECTIONID : 0;
                //}
                SearchParameterList paramsData = new SearchParameterList();
                paramsData.OPERATIONID = 0;
                paramsData.DIVISIONID = 0;
                paramsData.DEPARTMENTID = 0;
                paramsData.SECTIONID = 0;
                paramsData.SYKIID = 0;
                paramsData.EmpName = string.Empty;
                paramsData.ECode = string.Empty;
                paramsData.StatusId = StatusId;
                //sa CR7306
                //getSearchData(model, paramsData);
                var lst = _objHomeA00.GetA00_ADORGCOORDINATORLIST(userId, "CO").Select(x => x.ADORGLEVELID).ToList();
                if (lst.Count > 0)
                {
                    getSearchData_ApprovalList(model, paramsData);
                }
                else
                {
                    getSearchData(model, paramsData);
                }
                //ea CR7306
                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetA00RequestForApproval");
            }
            return Json(model.ResultList);
        }
        [HttpGet]
        public ActionResult Search(short status = 1, string returnMsgOnSearch = null)
        {
            //TempData["PageHead"] = "A00 Search";
            try
            {
                if (returnMsgOnSearch != null)
                {
                    ViewBag.Msg = returnMsgOnSearch;
                }
                var data = getSearchFilterData(new SearchParameterList());
                SearchParameterList paramsData = new SearchParameterList();
                paramsData.OPERATIONID = data.OPERATIONID != null ? data.OPERATIONID : 0;
                paramsData.DIVISIONID = data.DIVISIONID != null ? data.DIVISIONID : 0;
                paramsData.DEPARTMENTID = data.DEPARTMENTID != null ? data.DEPARTMENTID : 0;
                paramsData.SECTIONID = data.SECTIONID != null ? data.SECTIONID : 0;

                paramsData.SYKIID = data.SYKIID != null ? data.SYKIID : 0;
                paramsData.EmpName = data.EmpName;
                paramsData.ECode = data.ECode;
                paramsData.StatusId = status;
                A00ViewModel model = new A00ViewModel();
                getSearchData(model, paramsData);
                TempData["PageHead"] = "A00 Search";
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Search");
            }
            return View();
        }
        public ActionResult Search(A00ViewModel model)
        {
            TempData["PageHead"] = "A00 Search";
            SearchParameterList paramsData = new SearchParameterList();
            paramsData.OPERATIONID = Convert.ToInt32(Request.Form["hdnOpId"].ToString());
            paramsData.DIVISIONID = Convert.ToInt32(Request.Form["hdnDivId"].ToString());
            paramsData.DEPARTMENTID = Convert.ToInt32(Request.Form["hdnDeptId"].ToString());
            paramsData.SECTIONID = Convert.ToInt32(Request.Form["hdnSectionId"].ToString());

            paramsData.SYKIID = Convert.ToInt32(Request.Form["SearchParams.SYKI"].ToString());
            paramsData.EmpName = Request.Form["SearchParams.EmpName"].ToString();
            paramsData.ECode = Request.Form["SearchParams.ECode"].ToString();
            paramsData.StatusId = Convert.ToInt16(Request.Form["SearchParams.Status"].ToString());
            if (!string.IsNullOrEmpty(Request.Form["SearchParams.SYKI"].ToString()))
            {
                paramsData.SYKIID = Convert.ToInt32(Request.Form["SearchParams.SYKI"].ToString());
            }
            ;
            paramsData.ECode = Request.Form["SearchParams.ECode"].ToString();

            getSearchData(model, paramsData);

            return View(model);
            //if (statusCode == 1)
            //{
            //    currentUserName = _Asslogin.ADEMPLOYEE.Where(x => x.ADEMPCODE == userId).Select(y => y.FIRSTNAME + y.LASTNAME).FirstOrDefault();
            //}
        }
        [HttpGet]
        public ActionResult SearchBack(string returnMsgOnSearch = null, long? OPERATIONID = null, long? DIVISIONID = null, long? DEPARTMENTID = null, long? SECTIONID = null, decimal? SYKIID = null, string ECODE = null, string EMPNAME = null, short? STATUSID = 1)
        {
            TempData["PageHead"] = "A00 Search";
            if (returnMsgOnSearch != null)
            {
                ViewBag.Msg = returnMsgOnSearch;
            }

            //SearchResultsViewModel model = new SearchResultsViewModel();
            A00ViewModel model = new A00ViewModel();

            SearchParameterList paramsData = new SearchParameterList();
            paramsData.OPERATIONID = Convert.ToInt32(OPERATIONID?.ToString());
            paramsData.DIVISIONID = Convert.ToInt32(DIVISIONID?.ToString());
            paramsData.DEPARTMENTID = Convert.ToInt32(DEPARTMENTID?.ToString());
            paramsData.SECTIONID = Convert.ToInt32(SECTIONID?.ToString());
            paramsData.SYKIID = Convert.ToInt32(SYKIID?.ToString());
            paramsData.ECode = ECODE?.ToString();
            paramsData.EmpName = EMPNAME?.ToString();
            paramsData.StatusId = Convert.ToInt16(STATUSID?.ToString());
            if (!string.IsNullOrEmpty(Request.Query["SearchParams.SYKI"].ToString()))
            {
                paramsData.SYKIID = Convert.ToInt32(Request.Query["SearchParams.SYKI"].ToString());
            }
            ;
            paramsData.ECode = Request.Query["SearchParams.ECode"].ToString();
            getSearchData(model, paramsData);

            return View("Search", model);
        }
        private SearchParameterList getSearchFilterData(SearchParameterList paramsList)
        {
            List<SearchParameterList> list = new List<SearchParameterList>();
            SearchParameterList data = new SearchParameterList();
            List<SearchParameterList> kiData = new List<SearchParameterList>();
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            kiData.Add(new SearchParameterList() { SYKIID = 0, SYKI = "-Select ki-" });

            var kiData1 = _objHomeA00.GetA00SYKIList();

            var activeKi = kiData1._SYKIList.Where(x => x.ACTIVE == 1).FirstOrDefault();
            //var kii = _Asslogin.SYKI.ToList();
            var kii = kiData1._SYKIList.OrderByDescending(x => x.SYKIID).ToList();

            foreach (var tblKiCode in kii)
            {
                kiData.Add(new SearchParameterList() { SYKIID = tblKiCode.SYKIID, SYKI = tblKiCode.KICODE });
            }

            var selectedKi = (paramsList.SYKIID == 0 || paramsList.SYKIID == null) ? activeKi.SYKIID : paramsList.SYKIID;
            ViewBag.SYKIData = new SelectList(kiData, "SYKIID", "SYKI", selectedKi);
            data.StatusId = paramsList.StatusId;

            var statusList = new List<SearchParameterList>() {

            new SearchParameterList {
                StatusId = 0,
                Status = "My Requests"
            },
            new SearchParameterList
            {
                StatusId = 5,
                Status = "Save As Draft"
            },
            new SearchParameterList
            {
                StatusId = 2,
                Status = "Approved"
            },
            new SearchParameterList {
                StatusId = 1,
                Status = "Pending"
            },
            new SearchParameterList
            {
                StatusId = 4,
                Status = "Send Back"
            },
            new SearchParameterList
            {
                StatusId = 3,
                Status = "Rejected"
            },//Added By Eshant as Discussed for A00 points
            new SearchParameterList
            {
                StatusId = 6,
                Status = "Deficiency"
            }
            };
            //Added By Eshant as Discussed for Sorting for A00 points
            statusList = statusList.OrderBy(x => x.Status).ToList();
            // ViewBag.StatusList = statusList;
            // ViewBag.StatusList = new SelectList(statusList, "StatusId", "Status", paramsList.StatusId);
            ViewBag.StatusList = new SelectList(statusList.OrderBy(i => i.Status).ToList(), "StatusId", "Status", paramsList.StatusId);//changed by eshant 4-jul-22 for sorting



            //if (TempData["PageHead"] != null)
            //{
            //    ViewBag.StatusList = new SelectList(statusList, "StatusId", "Status", paramsList.StatusId);
            //}
            //else
            //{
            //    ViewBag.StatusList = new SelectList(statusList, "StatusId", "Status", 1);
            //}

            //data.Status = statusList[data.StatusId].Status;
            data.ECode = paramsList.ECode;
            data.EmpName = paramsList.EmpName;
            //var empDetails = _Asslogin.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == userId && x.SYKI == activeKi.SYKIID).OrderByDescending(x => x.ACTIVE).FirstOrDefault();

            _objA00SearchModel.ADEMPCODE = userId;
            _objA00SearchModel.SYKI = (decimal)selectedKi;
            data.SYKIID = selectedKi;// activeKi.SYKIID;

            //var empDetails = _Asslogin.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == userId && x.SYKI == activeKi.SYKIID).OrderByDescending(x => x.ACTIVE).FirstOrDefault();
            var empDetails = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel);
            if (empDetails != null)
            {
                data.OPERATION = empDetails.OPERATION != null ? empDetails.OPERATION : "--No record--";
                data.OPERATIONID = empDetails.OPERATIONID != null ? empDetails.OPERATIONID : 0;
                data.DIVISION = empDetails.DIVISION != null ? empDetails.DIVISION : "--No record--";
                data.DIVISIONID = empDetails.DIVISIONID != null ? empDetails.DIVISIONID : 0;
                data.DEPARTMENT = empDetails.DEPARTMENT != null ? empDetails.DEPARTMENT : "--No record--";
                data.DEPARTMENTID = empDetails.DEPARTMENTID != null ? empDetails.DEPARTMENTID : 0;
                data.SECTION = empDetails.SECTION != null ? empDetails.SECTION : "--No record--";
                data.SECTIONID = empDetails.SECTIONID != null ? empDetails.SECTIONID : 0;
            }
            else
            {
                data.OPERATION = "--No record--";
                data.OPERATIONID = 0;
                data.DIVISION = "--No record--";
                data.DIVISIONID = 0;
                data.DEPARTMENT = "--No record--";
                data.DEPARTMENTID = 0;
                data.SECTION = "--No record--";
                data.SECTIONID = 0;
            }
            list.Add(data);
            ViewBag.List = list;

            return data;
        }
        private dynamic getLastApproverName(A00APPROVALList approvalRecord)
        {
            //09-Sept-2021 change start
            //var idList = new List<long?> { approvalRecord.DEPTHDID, approvalRecord.COORDDID, approvalRecord.DIVHDHDID, approvalRecord.EXECOHDID, approvalRecord.OHID };
            var idList = new List<long?> { approvalRecord.DEPTHDID, approvalRecord.COORDDID, approvalRecord.DIVHDHDID, approvalRecord.EXECOHDID, approvalRecord.OHID, approvalRecord.PPCHOOHID };
            //09-Sept-2021 change start

            //var empData = _Asslogin.ADEMPLOYEE.Where(x => idList.Contains(x.ADEMPCODE)).Select(y => new { Id = y.ADEMPCODE, Name = y.FIRSTNAME + " " + y.LASTNAME }).ToList();
            var empData = _objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE.Where(x => idList.Contains(x.ADEMPCODE)).Select(y => new { Id = y.ADEMPCODE, Name = y.FIRSTNAME + " " + y.LASTNAME }).ToList();

            long? lastApprovedId = 0;
            //09-Sept-2021 change start
            if (approvalRecord.PPCHOOHID != null && approvalRecord.PPCHOOHDATE != null)
            {
                lastApprovedId = approvalRecord.PPCHOOHID;
            }
            //if (approvalRecord.OHID != null && approvalRecord.OHAPPDATE != null)
            else if (approvalRecord.OHID != null && approvalRecord.OHAPPDATE != null)
            //09-Sept-2021 change end
            {
                lastApprovedId = approvalRecord.OHID;
            }
            else if (approvalRecord.EXECOHDID != null && approvalRecord.EXECOAPPDATE != null)
            {
                lastApprovedId = approvalRecord.EXECOHDID;
            }
            else if (approvalRecord.DIVHDHDID != null && approvalRecord.DIVHDAPPDATE != null)
            {
                lastApprovedId = approvalRecord.DIVHDHDID;
            }
            else if (approvalRecord.COORDDID != null && approvalRecord.COORDAPPDATE != null)
            {
                lastApprovedId = approvalRecord.COORDDID;
            }
            else if (approvalRecord.DEPTHDID != null && approvalRecord.DEPTHDAPPDATE != null)
            {
                lastApprovedId = approvalRecord.DEPTHDID;
            }
            return empData.Where(x => x.Id == lastApprovedId).Select(y => y.Name).FirstOrDefault();

        }
        private void getSearchData(A00ViewModel model, SearchParameterList paramsData)
        {
            bool isDeptHead = false, isCoOrdHead = false, isDivHead = false, isExeCoOrdHead = false, isOperatingHead = false;
            Dictionary<int, List<int>> statusMapForRoles = new Dictionary<int, List<int>>();
            long? operation, division, department, section;
            SearchParameterList paramlist = new SearchParameterList();
            List<SearchResultList> empCodeList = new List<SearchResultList>();
            //--saveasdraft = '10' submit = '20',Dept.Head approved = '60', Dept.Head reject = 30,
            //           ----Div.Head reject = '70',

            List<int> deptStatus = new List<int> { 0, 20, 1, 30, 35, 10 };
            List<int> coOrdStatus = new List<int> { 0, 40, 1, 50, 55, 10 };
            List<int> divStatus = new List<int> { 0, 60, 1, 70, 75, 10 };
            List<int> exeCoOrdStatus = new List<int> { 0, 80, 1, 90, 95, 10 };
            List<int> ohStatus = new List<int> { 0, 100, 1, 110, 115, 10 };
            List<int> generalUserStatus = new List<int> { 0, 2, 3, 5, 6, 10 };

            var userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            operation = paramsData.OPERATIONID;
            division = paramsData.DIVISIONID;
            department = paramsData.DEPARTMENTID;
            section = paramsData.SECTIONID;
            model.SearchParams = new SearchParameterList();
            model.SearchParams.OPERATIONID = operation;
            model.SearchParams.DIVISIONID = division;
            model.SearchParams.DEPARTMENTID = department;
            model.SearchParams.SECTIONID = section;
            var sykiId = paramsData.SYKIID;//0;

            // SearchParams.StatusId
            model.SearchParams.StatusId = paramsData.StatusId;
            var empCode = string.IsNullOrEmpty(paramsData.ECode) ? 0 : Convert.ToInt64(paramsData.ECode);
            var empName = paramsData.EmpName;
            model.SearchParams.ECode = paramsData.ECode;
            model.SearchParams.EmpName = empName;
            model.SearchParams.SYKIID = sykiId;
            paramlist.SYKIID = sykiId;
            paramlist.ECode = paramsData.ECode;
            paramlist.EmpName = empName;

            paramlist.StatusId = model.SearchParams.StatusId;
            //var paramData = getSearchFilterData(paramlist);
            var paramData = getSearchFilterData(paramlist);
            model.SearchParams.SYKI = paramData.SYKI;
            model.SearchParams.Status = paramData.Status;

            var userData = _objHomeA00.getUserRole(Convert.ToInt64(_sessionService.Get<string>("userID")), sykiId);

            isDeptHead = userData.isDeptHead;
            isCoOrdHead = userData.isCoOrdHead;
            isDivHead = userData.isDivHead;
            isExeCoOrdHead = userData.isExeCoOrdHead;
            isOperatingHead = userData.isOperatingHead;

            //09-Sept-2021 change start
            bool isPPCHOOperatingHead = _objHomeA00.isPPCHOOH(userId);
            //09-Sept-2021 change end

            var status = model.SearchParams.StatusId;

            var statusCode = isDeptHead ? deptStatus[status] : (isCoOrdHead ? coOrdStatus[status] : (isDivHead ? divStatus[status] :
                (isExeCoOrdHead ? exeCoOrdStatus[status] : (isOperatingHead ? ohStatus[status] : generalUserStatus[status]))));

            var ApprovedStatusList = new List<int>();
            //09-Sept-2021 change start
            if (isDeptHead)
                ApprovedStatusList = new List<int> { 40, 60, 80, 100, 101, 105 };
            else if (isCoOrdHead)
                ApprovedStatusList = new List<int> { 60, 80, 100, 101, 105 };
            else if (isDivHead)
                ApprovedStatusList = new List<int> { 80, 100, 101, 105 };
            else if (isExeCoOrdHead)
                ApprovedStatusList = new List<int> { 100, 101, 105 };
            //else if (isOperatingHead)
            else if (isOperatingHead || isPPCHOOperatingHead)
                ApprovedStatusList = new List<int> { 101, 105 };
            //09-Sept-2021 change end
            var sentBacks = new List<int> { 10, 35, 55, 75, 95, 115 };

            var pendingApprovals = new List<int> { 20, 40, 60, 80, 100 };

            var allReq = _objHomeA00.GetA00DtlFullTable()._A00DtlViewModelList.Where(x => x.ADDEDBY == (statusCode == 0 ? userId : x.ADDEDBY)).ToList();

            //09-Sept-2021 change start
            var rejectList = new List<int> { 30, 50, 70, 90, 110 };
            //09-Sept-2021 change end


            //07-Sept-2021 change start
            //var operationList = _objHomeA00.GetA00_ADORGCOORDINATORLIST(userId, isCoOrdHead ? "CO" : (isExeCoOrdHead ? "EXCO" : "NA"))._A00_ADORGCOORDINATOR.Select(x=>x.ADORGLEVELID).ToList();

            //09-Sept-2021 change start
            //var operationList = _objHomeA00.GetA00_ADORGCOORDINATORLIST(userId, isCoOrdHead ? "CO" : (isExeCoOrdHead ? "EXCO" : "NA")).Select(x => x.ADORGLEVELID).ToList();
            var operationList = _objHomeA00.GetA00_ADORGCOORDINATORLIST(userId, isCoOrdHead ? "CO" : (isExeCoOrdHead ? "EXCO" : (isOperatingHead ? "OH" : "NA"))).Select(x => x.ADORGLEVELID).ToList();
            //09-Sept-2021 change end

            //07-Sept-2021 change end

            #region - Before modification
            // var records = (
            //     allReq.Join(_objHomeA00.GetA00APPROVALFullTable()._A00APPROVALList, a => a.A00DTLTBID,
            //     b => b.A00DTLTBID, (a, b) => new { TLTB = a, approval = b }).Where(ab => ab.approval.A00DTLTBID ==
            // ab.TLTB.A00DTLTBID && ab.TLTB.SYKIID == (sykiId == 0 ? ab.TLTB.SYKIID : sykiId) &&

            // //07-Sept-2021 change start
            // //ab.TLTB.OPERATION == (operation == 0 ? ab.TLTB.OPERATION : operation) &&

            // //09-Sept-2021 change start            
            // //((isCoOrdHead || isExeCoOrdHead) ? operationList.Contains((long)ab.TLTB.OPERATION) : (ab.TLTB.OPERATION == (operation == 0 ? ab.TLTB.OPERATION : operation))) &&
            // ((isCoOrdHead || isExeCoOrdHead) ? operationList.Contains((long)ab.TLTB.OPERATION) : (ab.TLTB.OPERATION == (isPPCHOOperatingHead ? ab.TLTB.OPERATION : (operation == 0 ? ab.TLTB.OPERATION : operation)))) &&
            // //09-Sept-2021 change start
            // //07-Sept-2021 change end

            // ab.TLTB.DIVISION == (division == 0 ? ab.TLTB.DIVISION : division) &&
            // ab.TLTB.DEPARTMENT == (department == 0 ? ab.TLTB.DEPARTMENT : department) &&
            // ab.TLTB.SECTION == (section == 0 ? ab.TLTB.SECTION : section)

            // && (sentBacks.Contains(statusCode) ? sentBacks.Contains(ab.TLTB.STATUSCD) : ((statusCode == 0 || statusCode == 1 ? true : ab.TLTB.STATUSCD == statusCode)))

            // && (pendingApprovals.Contains(statusCode) ?
            // (isDeptHead ? ab.approval.DEPTHDID == userId && ab.approval.DEPTHDAPPDATE == null :
            // (isCoOrdHead ? ab.approval.COORDDID == userId && ab.approval.COORDAPPDATE == null :
            // (isDivHead ? ab.approval.DIVHDHDID == userId && ab.approval.DIVHDAPPDATE == null :
            // (isExeCoOrdHead ? ab.approval.EXECOHDID == userId && ab.approval.EXECOAPPDATE == null :
            // //09-Sept-2021 change start
            // (isOperatingHead ? ab.approval.OHID == userId && ab.approval.OHAPPDATE == null : true)))))
            // //((isOperatingHead || isPPCHOOperatingHead) ? ((ab.approval.OHID == userId && ab.approval.OHAPPDATE == null) || (ab.approval.PPCHOOHID == userId && ab.approval.PPCHOOHDATE == null)) : true)))))
            // //09-Sept-2021 change end
            // : true)

            //&& (status == 0 ? ab.TLTB.ADDEDBY == userId && ab.TLTB.STATUSCD != 10 : true)

            // && (rejectList.Contains(statusCode) ? ((ab.approval.DEPTHDAPPDATE != null && ab.approval.DEPTHDID == userId) || //approved
            // (ab.approval.COORDAPPDATE != null && ab.approval.COORDDID == userId) || (ab.approval.DIVHDAPPDATE != null && ab.approval.DIVHDHDID == userId) ||
            // (ab.approval.EXECOAPPDATE != null && ab.approval.EXECOHDID == userId) || (ab.approval.OHAPPDATE != null && ab.approval.OHID == userId)) : true)

            // &&

            // (sentBacks.Contains(statusCode) && status == 4 ? (isDeptHead ? ab.approval.DEPTHDID == userId : (isCoOrdHead ? ab.approval.COORDDID == userId :
            //   (isDivHead ? ab.approval.DIVHDHDID == userId : (isExeCoOrdHead ? ab.approval.EXECOHDID == userId : (isOperatingHead ? ab.approval.OHID == userId :
            //   (ab.TLTB.ADDEDBY == userId)))))) : true)

            //   && ab.TLTB.ADDEDBY == (statusCode == 10 ? userId : ab.TLTB.ADDEDBY) && 

            //   (status == 2 ?
            // (isDeptHead ? ApprovedStatusList.Contains(ab.TLTB.STATUSCD) && ab.approval.DEPTHDID == userId && ab.approval.DEPTHDAPPDATE != null :
            // (isCoOrdHead ? ApprovedStatusList.Contains(ab.TLTB.STATUSCD) && ab.approval.COORDDID == userId && ab.approval.COORDAPPDATE != null :
            // (isDivHead ? ApprovedStatusList.Contains(ab.TLTB.STATUSCD) && ab.approval.DIVHDHDID == userId && ab.approval.DIVHDAPPDATE != null :
            // (isExeCoOrdHead ? ApprovedStatusList.Contains(ab.TLTB.STATUSCD) && ab.approval.EXECOHDID == userId && ab.approval.EXECOAPPDATE != null :

            // //09-Sept-2021 change start
            // (isOperatingHead ? ApprovedStatusList.Contains(ab.TLTB.STATUSCD) && ab.approval.OHID == userId && ab.approval.OHAPPDATE != null : true)))))
            // //(isOperatingHead || isPPCHOOperatingHead ? ApprovedStatusList.Contains(ab.TLTB.STATUSCD) && ab.approval.OHID == userId && ab.approval.OHAPPDATE != null : true)))))
            //  //09-Sept-2021 change end
            //  : true)
            // )).ToList();
            #endregion

            #region Modification Here
            var records = (
                allReq.Join(_objHomeA00.GetA00APPROVALFullTable()._A00APPROVALList, a => a.A00DTLTBID,
                b => b.A00DTLTBID, (a, b) => new { TLTB = a, approval = b }).Where(ab => ab.approval.A00DTLTBID ==
            ab.TLTB.A00DTLTBID && ab.TLTB.SYKIID == (sykiId == 0 ? ab.TLTB.SYKIID : sykiId)
            &&

             //07-Sept-2021 change start
             //ab.TLTB.OPERATION == (operation == 0 ? ab.TLTB.OPERATION : operation) &&

             //09-Sept-2021 change start            
             //((isCoOrdHead || isExeCoOrdHead) ? operationList.Contains((long)ab.TLTB.OPERATION) : (ab.TLTB.OPERATION == (operation == 0 ? ab.TLTB.OPERATION : operation))) &&
             //((isCoOrdHead || isExeCoOrdHead) ? operationList.Contains((long)ab.TLTB.OPERATION) : (ab.TLTB.OPERATION == (isPPCHOOperatingHead ? ab.TLTB.OPERATION : (operation == 0 ? ab.TLTB.OPERATION : operation)))) &&
             ((isCoOrdHead || isExeCoOrdHead) ? operationList.Contains((long)ab.TLTB.OPERATION) :
             (isOperatingHead ? operationList.Contains((long)ab.TLTB.DIVISION) || operationList.Contains((long)ab.TLTB.OPERATION) ||
             (ab.TLTB.OPERATION == (isPPCHOOperatingHead ? ab.TLTB.OPERATION : (operation == 0 ? ab.TLTB.OPERATION : operation))) :
             (ab.TLTB.OPERATION == (isPPCHOOperatingHead ? ab.TLTB.OPERATION : (operation == 0 ? ab.TLTB.OPERATION : operation))))) &&
             //09-Sept-2021 change end
             // //07-Sept-2021 change end

             ab.TLTB.DIVISION == (division == 0 ? ab.TLTB.DIVISION : division) &&
             ab.TLTB.DEPARTMENT == (department == 0 ? ab.TLTB.DEPARTMENT : department) &&
             ab.TLTB.SECTION == (section == 0 ? ab.TLTB.SECTION : section)

             // //09-Sept-2021 change start            
             // //&& (sentBacks.Contains(statusCode) ? sentBacks.Contains(ab.TLTB.STATUSCD) : (statusCode == 0 || statusCode == 1 ? true : ab.TLTB.STATUSCD == statusCode))
             && (sentBacks.Contains(statusCode) ? sentBacks.Contains(ab.TLTB.STATUSCD) || (ab.TLTB.STATUSCD == 116) : (statusCode == 0 || statusCode == 1 || isPPCHOOperatingHead ? true : ab.TLTB.STATUSCD == statusCode))
            // //09-Sept-2021 change end            

            //09-Sept-2021 change start
            && ((pendingApprovals.Contains(statusCode) ?
            (isDeptHead ? ab.approval.DEPTHDID == userId && ab.approval.DEPTHDAPPDATE == null :
            (isCoOrdHead ? ab.approval.COORDDID == userId && ab.approval.COORDAPPDATE == null :
            (isDivHead ? ab.approval.DIVHDHDID == userId && ab.approval.DIVHDAPPDATE == null :
            (isExeCoOrdHead ? ab.approval.EXECOHDID == userId && ab.approval.EXECOAPPDATE == null :
            (isOperatingHead ? ab.approval.OHID == userId && ab.approval.OHAPPDATE == null : true)))))
            : true) || (isPPCHOOperatingHead ? ab.approval.PPCHOOHID == userId && ab.approval.PPCHOOHDATE == null && ab.TLTB.STATUSCD == 101 : false))
            //09-Sept-2021 change end

            && (status == 0 ? ab.TLTB.ADDEDBY == userId && ab.TLTB.STATUSCD != 10 : true)

             && ((rejectList.Contains(statusCode) ? ab.TLTB.STATUSCD == statusCode && ((ab.approval.DEPTHDAPPDATE != null && ab.approval.DEPTHDID == userId) ||
             (ab.approval.COORDAPPDATE != null && ab.approval.COORDDID == userId) || (ab.approval.DIVHDAPPDATE != null && ab.approval.DIVHDHDID == userId) ||
             (ab.approval.EXECOAPPDATE != null && ab.approval.EXECOHDID == userId) || (ab.approval.OHAPPDATE != null && ab.approval.OHID == userId)) : true)
             || (isPPCHOOperatingHead ? ab.approval.PPCHOOHDATE != null && ab.approval.PPCHOOHID == userId && ab.TLTB.STATUSCD == 111 : false))

             &&

            ((sentBacks.Contains(statusCode) && status == 4 ? (isDeptHead ? ab.approval.DEPTHDID == userId : (isCoOrdHead ? ab.approval.COORDDID == userId :
              (isDivHead ? ab.approval.DIVHDHDID == userId : (isExeCoOrdHead ? ab.approval.EXECOHDID == userId : (isOperatingHead ? ab.approval.OHID == userId :
              (ab.TLTB.ADDEDBY == userId)))))) : true) || (isPPCHOOperatingHead ? ab.approval.PPCHOOHID == userId && ab.TLTB.STATUSCD == 116 : false))

                && ab.TLTB.ADDEDBY == (statusCode == 10 ? userId : ab.TLTB.ADDEDBY)

                &&

              (status == 2 ?
            (isDeptHead ? ApprovedStatusList.Contains(ab.TLTB.STATUSCD) && ab.approval.DEPTHDID == userId && ab.approval.DEPTHDAPPDATE != null :
            (isCoOrdHead ? ApprovedStatusList.Contains(ab.TLTB.STATUSCD) && ab.approval.COORDDID == userId && ab.approval.COORDAPPDATE != null :
            (isDivHead ? ApprovedStatusList.Contains(ab.TLTB.STATUSCD) && ab.approval.DIVHDHDID == userId && ab.approval.DIVHDAPPDATE != null :
            (isExeCoOrdHead ? ApprovedStatusList.Contains(ab.TLTB.STATUSCD) && ab.approval.EXECOHDID == userId && ab.approval.EXECOAPPDATE != null

              //09-Sept-2021 change start
              //(isOperatingHead ? ApprovedStatusList.Contains(ab.TLTB.STATUSCD) && ab.approval.OHID == userId && ab.approval.OHAPPDATE != null : true)))))
              : (isOperatingHead || isPPCHOOperatingHead ? ApprovedStatusList.Contains(ab.TLTB.STATUSCD) && ((ab.approval.OHID == userId && ab.approval.OHAPPDATE != null) || (ab.approval.PPCHOOHID == userId && ab.approval.PPCHOOHDATE != null)) : true)))))
             //09-Sept-2021 change end
             : true)
            )).ToList();
            #endregion

            var kiData1 = _objHomeA00.GetA00SYKIList();

            #region get details
            //var resultList = records.Join(_Asslogin.ADEMPLOYEE.Where(x =>
            //var resultList1 = records.Join(_objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE.Where(x =>
            //(string.IsNullOrEmpty(empName) || (x.FIRSTNAME + " " + x.LASTNAME).Contains(empName)) &&
            //(empCode == 0 || x.ADEMPCODE == empCode)),
            // c => c.TLTB.ADDEDBY, d => d.ADEMPCODE, (c, d) => new
            // {
            //     approvalData = c,
            //     empList = d
            // });
            var resultList = records.Join(_objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE.Where(x =>
            (string.IsNullOrEmpty(empName) || (x.FIRSTNAME + " " + x.LASTNAME).Contains(empName)) &&
            (empCode == 0 || x.ADEMPCODE == empCode)),
             c => c.TLTB.ADDEDBY, d => d.ADEMPCODE, (c, d) => new
             {
                 approvalData = c,
                 empList = d
             })
             .Select(x => new SearchResultList
             {
                 A00DTLTBID = x.approvalData.TLTB.A00DTLTBID,
                 SYKIID = x.approvalData.TLTB.SYKIID,
                 STATUSCD = x.approvalData.TLTB.STATUSCD,
                 //SYKIID = x.approvalData.TLTB.SYKIID,
                 //KICODE = _Asslogin.SYKI.Where(x1 => x1.SYKIID == x.approvalData.TLTB.SYKIID).Select(x1 => x1.KICODE).FirstOrDefault(),
                 KICODE = kiData1._SYKIList.Where(x1 => x1.SYKIID == x.approvalData.TLTB.SYKIID).Select(x1 => x1.KICODE).FirstOrDefault(),
                 ECode = x.empList.ADEMPCODE,
                 EmpName = x.empList.FIRSTNAME + " " + x.empList.LASTNAME,
                 ProjectTitle = x.approvalData.TLTB.PRJCTTLE,
                 RequestDate = x.approvalData.TLTB.DATEADDEDID,
                 ApprovedBy = getLastApproverName(x.approvalData.approval),

                 //09-Sept-2021 change start
                 //ApprovedOn = (x.approvalData.approval.OHID != null && x.approvalData.approval.OHAPPDATE != null ? x.approvalData.approval.OHAPPDATE :
                 //   (x.approvalData.approval.EXECOHDID != null && x.approvalData.approval.EXECOAPPDATE != null ?
                 //   x.approvalData.approval.EXECOAPPDATE : (x.approvalData.approval.DIVHDHDID != null && x.approvalData.approval.DIVHDAPPDATE != null ?
                 //   x.approvalData.approval.DIVHDAPPDATE :
                 //   (x.approvalData.approval.COORDDID != null && x.approvalData.approval.COORDAPPDATE != null ? x.approvalData.approval.COORDAPPDATE :
                 //   (x.approvalData.approval.DEPTHDID != null && x.approvalData.approval.DEPTHDAPPDATE != null ? x.approvalData.approval.DEPTHDAPPDATE : null))))),

                 ApprovedOn = (x.approvalData.approval.PPCHOOHID != null && x.approvalData.approval.PPCHOOHDATE != null ? x.approvalData.approval.PPCHOOHDATE :
                    (x.approvalData.approval.OHID != null && x.approvalData.approval.OHAPPDATE != null ? x.approvalData.approval.OHAPPDATE :
                    (x.approvalData.approval.EXECOHDID != null && x.approvalData.approval.EXECOAPPDATE != null ?
                    x.approvalData.approval.EXECOAPPDATE : (x.approvalData.approval.DIVHDHDID != null && x.approvalData.approval.DIVHDAPPDATE != null ?
                    x.approvalData.approval.DIVHDAPPDATE :
                    (x.approvalData.approval.COORDDID != null && x.approvalData.approval.COORDAPPDATE != null ? x.approvalData.approval.COORDAPPDATE :
                    (x.approvalData.approval.DEPTHDID != null && x.approvalData.approval.DEPTHDAPPDATE != null ? x.approvalData.approval.DEPTHDAPPDATE : null)))))),
                 //09-Sept-2021 change end

                 IsDeficiencyClosed = _objHomeA00.IsA00DeficiencyClosed(x.approvalData.TLTB.A00DTLTBID, x.approvalData.TLTB.SYKIID),
                 IsDeficiencyRaised = _objHomeA00.IsA00DeficiencyRaised(x.approvalData.TLTB.A00DTLTBID, x.approvalData.TLTB.SYKIID),
                 DeficiencyStatus = _objHomeA00.GetDeficiencyStatus(x.approvalData.TLTB.A00DTLTBID, x.approvalData.TLTB.SYKIID),

                 //07-Sept-2021 change start
                 OPERATION = x.approvalData.TLTB.OPERATION.ToString()
                 //07-Sept-2021 change end
             }).ToList();
            model.ResultList = resultList.OrderByDescending(x => x.RequestDate).ThenBy(x => x.ProjectTitle).ToList(); // gaurav dua
            #endregion

            //paramlist.SYKI = _Asslogin.SYKI.Where(x => x.SYKIID == paramlist.SYKIID).Select(x => x.KICODE).FirstOrDefault();
            paramlist.SYKI = kiData1._SYKIList.Where(x => x.SYKIID == paramlist.SYKIID).Select(x => x.KICODE).FirstOrDefault();
        }

        //sa CR7306
        #region Fetch Approval List
        private void getSearchData_ApprovalList(A00ViewModel model, SearchParameterList paramsData)
        {
            Dictionary<int, List<int>> statusMapForRoles = new Dictionary<int, List<int>>();
            long? operation, division, department, section;
            SearchParameterList paramlist = new SearchParameterList();
            List<SearchResultList> empCodeList = new List<SearchResultList>();

            List<int> deptStatus = new List<int> { 0, 20, 1, 30, 35, 10 };
            List<int> coOrdStatus = new List<int> { 0, 40, 1, 50, 55, 10 };
            List<int> divStatus = new List<int> { 0, 60, 1, 70, 75, 10 };
            List<int> exeCoOrdStatus = new List<int> { 0, 80, 1, 90, 95, 10 };
            List<int> ohStatus = new List<int> { 0, 100, 1, 110, 115, 10 };
            List<int> generalUserStatus = new List<int> { 0, 2, 3, 5, 6, 10 };

            var userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            operation = paramsData.OPERATIONID;
            division = paramsData.DIVISIONID;
            department = paramsData.DEPARTMENTID;
            section = paramsData.SECTIONID;
            model.SearchParams = new SearchParameterList();
            model.SearchParams.OPERATIONID = operation;
            model.SearchParams.DIVISIONID = division;
            model.SearchParams.DEPARTMENTID = department;
            model.SearchParams.SECTIONID = section;
            var sykiId = paramsData.SYKIID;//0;
            model.SearchParams.StatusId = paramsData.StatusId;
            var empCode = string.IsNullOrEmpty(paramsData.ECode) ? 0 : Convert.ToInt64(paramsData.ECode);
            var empName = paramsData.EmpName;
            model.SearchParams.ECode = paramsData.ECode;
            model.SearchParams.EmpName = empName;
            model.SearchParams.SYKIID = sykiId;
            paramlist.SYKIID = sykiId;
            paramlist.ECode = paramsData.ECode;
            paramlist.EmpName = empName;

            paramlist.StatusId = model.SearchParams.StatusId;
            var paramData = getSearchFilterData(paramlist);
            model.SearchParams.SYKI = paramData.SYKI;
            model.SearchParams.Status = paramData.Status;

            var userData = _objHomeA00.getUserRole(Convert.ToInt64(_sessionService.Get<string>("userID")), sykiId);

            bool isPPCHOOperatingHead = _objHomeA00.isPPCHOOH(userId);

            var status = model.SearchParams.StatusId;
            var statusCode = coOrdStatus[status];
            var ApprovedStatusList = new List<int>();

            ApprovedStatusList = new List<int> { 60, 80, 100, 101, 105 };
            var sentBacks = new List<int> { 10, 35, 55, 75, 95, 115 };

            var pendingApprovals = new List<int> { 20, 40, 60, 80, 100 };

            var allReq = _objHomeA00.GetA00DtlFullTable()._A00DtlViewModelList.Where(x => x.ADDEDBY == (statusCode == 0 ? userId : x.ADDEDBY)).ToList();

            var rejectList = new List<int> { 30, 50, 70, 90, 110 };

            #region Modification Here
            var records = (
                allReq.Join(_objHomeA00.GetA00APPROVALFullTable()._A00APPROVALList, a => a.A00DTLTBID,
                b => b.A00DTLTBID, (a, b) => new { TLTB = a, approval = b }).Where(ab => ab.approval.A00DTLTBID ==
            ab.TLTB.A00DTLTBID && ab.TLTB.SYKIID == (sykiId == 0 ? ab.TLTB.SYKIID : sykiId)
            && _objHomeA00.getDataFromAdorgcoordinator(ab.TLTB.OPERATION).COORDINATOR == userId
            && ab.TLTB.STATUSCD == (pendingApprovals.Contains(statusCode) == true ? (short)statusCode : (short)ab.TLTB.STATUSCD)
            )).ToList();
            #endregion

            var kiData1 = _objHomeA00.GetA00SYKIList();

            #region get details
            var resultList = records.Join(_objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE.Where(x =>
            (string.IsNullOrEmpty(empName) || (x.FIRSTNAME + " " + x.LASTNAME).Contains(empName)) &&
            (empCode == 0 || x.ADEMPCODE == empCode)),
             c => c.TLTB.ADDEDBY, d => d.ADEMPCODE, (c, d) => new
             {
                 approvalData = c,
                 empList = d
             })
             .Select(x => new SearchResultList
             {
                 A00DTLTBID = x.approvalData.TLTB.A00DTLTBID,
                 SYKIID = x.approvalData.TLTB.SYKIID,
                 STATUSCD = x.approvalData.TLTB.STATUSCD,
                 KICODE = kiData1._SYKIList.Where(x1 => x1.SYKIID == x.approvalData.TLTB.SYKIID).Select(x1 => x1.KICODE).FirstOrDefault(),
                 ECode = x.empList.ADEMPCODE,
                 EmpName = x.empList.FIRSTNAME + " " + x.empList.LASTNAME,
                 ProjectTitle = x.approvalData.TLTB.PRJCTTLE,
                 RequestDate = x.approvalData.TLTB.DATEADDEDID,
                 ApprovedBy = getLastApproverName(x.approvalData.approval),
                 ApprovedOn = (x.approvalData.approval.PPCHOOHID != null && x.approvalData.approval.PPCHOOHDATE != null ? x.approvalData.approval.PPCHOOHDATE :
                    (x.approvalData.approval.OHID != null && x.approvalData.approval.OHAPPDATE != null ? x.approvalData.approval.OHAPPDATE :
                    (x.approvalData.approval.EXECOHDID != null && x.approvalData.approval.EXECOAPPDATE != null ?
                    x.approvalData.approval.EXECOAPPDATE : (x.approvalData.approval.DIVHDHDID != null && x.approvalData.approval.DIVHDAPPDATE != null ?
                    x.approvalData.approval.DIVHDAPPDATE :
                    (x.approvalData.approval.COORDDID != null && x.approvalData.approval.COORDAPPDATE != null ? x.approvalData.approval.COORDAPPDATE :
                    (x.approvalData.approval.DEPTHDID != null && x.approvalData.approval.DEPTHDAPPDATE != null ? x.approvalData.approval.DEPTHDAPPDATE : null)))))),
                 IsDeficiencyClosed = _objHomeA00.IsA00DeficiencyClosed(x.approvalData.TLTB.A00DTLTBID, x.approvalData.TLTB.SYKIID),
                 IsDeficiencyRaised = _objHomeA00.IsA00DeficiencyRaised(x.approvalData.TLTB.A00DTLTBID, x.approvalData.TLTB.SYKIID),
                 DeficiencyStatus = _objHomeA00.GetDeficiencyStatus(x.approvalData.TLTB.A00DTLTBID, x.approvalData.TLTB.SYKIID),
                 OPERATION = x.approvalData.TLTB.OPERATION.ToString()
             }).ToList();
            model.ResultList = resultList.OrderByDescending(x => x.RequestDate).ThenBy(x => x.ProjectTitle).ToList(); // gaurav dua
            #endregion

            paramlist.SYKI = kiData1._SYKIList.Where(x => x.SYKIID == paramlist.SYKIID).Select(x => x.KICODE).FirstOrDefault();
        }
        #endregion
        //ea CR7306

        #endregion

        #region ITDivisionApproval-Controller
        public ActionResult ITDivisionApprovalSearch()
        {
            //Change start on 22-July-2021
            //TempData["PageHead"] = "Approved A00 view for Deficiency Report/ Allocation";
            TempData["PageHead"] = "A00 Allocation/ Raise Deficiency";
            //Change end on 22-July-2021

            SearchParameterList paramsList = new SearchParameterList();
            A00ViewModel model = new A00ViewModel();
            try
            {
                var paramData = ITDivisionApprovalgetSearchFilterData(new SearchParameterList());
                //model.SearchParams.SYKI = paramData.SYKIID;
                //model.SearchParams.Status = paramData.Status;

                ITDivisionGetApprovedA00SearchData(model, paramData);

                //List<ADORGLEVEL> _opList = _objHomeA00.GetOrgLevelList((long)1, (long)paramData.SYKIID);
                List<A00ADORGLEVELList> _opList = _objHomeA00.GetA00ADORGLEVELList(paramData.SYKIID)._ADOrgLevelList;
                ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.DivList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.DepList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.SecList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
                var A00DeficiencyRaisedBY = Convert.ToInt32(_sessionService.Get<string>("userID"));
                var DeficiencyDetails = _objHomeA00.GetDeficiencyRaisedDetails(model.A00DTLTBID, model.SYKI);
                ViewBag.DeficiencyDetails = DeficiencyDetails;
                var actionBy = DeficiencyDetails.Select(x => x.EmpActionBy).FirstOrDefault();
                ViewBag.DeficiencyStatus = _objHomeA00.GetDeficiencyStatus(model.A00DTLTBID, model.SYKI);
                ViewBag.CanPerformAction = actionBy != 0 ? (A00DeficiencyRaisedBY == actionBy ? 1 : 2) : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ITDivisionApprovalSearch");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult ITDivisionApprovalSearch(A00ViewModel model, string value)
        {
            //Change start on 22-July-2021
            //TempData["PageHead"] = "Approved A00 view for Deficiency Report/ Allocation";
            TempData["PageHead"] = "A00 Allocation/ Raise Deficiency";
            //Change end on 22-July-2021

            SearchParameterList paramlist = new SearchParameterList();

            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee"); ;

            //long? ii = null; //Changed on 22-July-2021
            //var sykiId = ii; //Changed on 22-July-2021
            var sykiId = 0;
            if (!string.IsNullOrEmpty(Request.Form["SearchParams.SYKI"].ToString()))
            {
                sykiId = Convert.ToInt32(Request.Form["SearchParams.SYKI"].ToString());
            }
            paramlist.SYKIID = sykiId;
            paramlist.ECode = Request.Form["SearchParams.ECode"].ToString();
            paramlist.EmpName = Request.Form["SearchParams.EmpName"].ToString();
            paramlist.StatusId = Convert.ToInt16(Request.Form["SearchParams.Status"].ToString());

            //paramlist.OPERATIONID = Request.Form["SearchParams.OPERATIONID"] != "" ? Convert.ToInt64(Request.Query["SearchParams.OPERATIONID"]) : 0;
            //paramlist.DIVISIONID = Request.Form["SearchParams.DIVISIONID"] != "" ? Convert.ToInt64(Request.Query["SearchParams.DIVISIONID"]) : 0;
            //paramlist.DEPARTMENTID = Request.Form["SearchParams.DEPARTMENTID"] != "" ? Convert.ToInt64(Request.Query["SearchParams.DEPARTMENTID"]) : 0;
            //paramlist.SECTIONID = Request.Form["SearchParams.SECTIONID"] != "" ? Convert.ToInt64(Request.Query["SearchParams.SECTIONID"]) : 0;

            paramlist.OPERATIONID = Request.Form["SearchParams.OPERATIONID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.OPERATIONID"]) : 0;
            paramlist.DIVISIONID = Request.Form["SearchParams.DIVISIONID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.DIVISIONID"]) : 0;
            paramlist.DEPARTMENTID = Request.Form["SearchParams.DEPARTMENTID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.DEPARTMENTID"]) : 0;
            paramlist.SECTIONID = Request.Form["SearchParams.SECTIONID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.SECTIONID"]) : 0;

            #region Block-1
            var userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

            model.SearchParams = new SearchParameterList();
            model.SearchParams.StatusId = paramlist.StatusId;
            var empCode = string.IsNullOrEmpty(Request.Form["SearchParams.ECode"].ToString()) ? 0 : Convert.ToInt64(Request.Form["SearchParams.ECode"].ToString());
            var empName = Request.Form["SearchParams.EmpName"].ToString();
            model.SearchParams.ECode = Request.Form["SearchParams.ECode"].ToString();
            model.SearchParams.EmpName = empName;
            model.SearchParams.SYKIID = sykiId;
            #endregion

            var paramData = ITDivisionApprovalgetSearchFilterData(paramlist);

            model.SearchParams.SYKI = paramData.SYKI;
            model.SearchParams.Status = paramData.Status;

            ITDivisionGetApprovedA00SearchData(model, paramData);

            #region added by kiran

            //sykiId = 0;//Changed on 22-July-2021
            //List<ADORGLEVEL>

            //List<ADORGLEVEL> _opList = _objHomeA00.GetOrgLevelList((long)1, sykiId);
            List<A00ADORGLEVELList> _opList = _objHomeA00.GetA00ADORGLEVELList((decimal)sykiId)._ADOrgLevelList;
            ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP", paramlist.OPERATIONID);

            List<SearchParameterList> _divList = _objHomeA00.BindDivision(paramlist.OPERATIONID, sykiId);// _objHomeA00.GetOrgLevelList((long)2);
            ViewBag.DivList = new SelectList(_divList, "DIVISIONID", "DIVISION", paramlist.DIVISIONID);

            List<SearchParameterList> _depList = _objHomeA00.BindDepartment(paramlist.DIVISIONID, 0, sykiId);// _objHomeA00.GetOrgLevelList((long)3);
            ViewBag.DepList = new SelectList(_depList, "DEPARTMENTID", "DEPARTMENT", paramlist.DEPARTMENTID);


            List<SearchParameterList> _secList = _objHomeA00.BindSection(paramlist.DEPARTMENTID, 0, 0, sykiId); //_objHomeA00.GetOrgLevelList((long)4);
            ViewBag.SecList = new SelectList(_secList, "SECTIONID", "SECTION", paramlist.SECTIONID);

            paramlist.OPERATIONID = employeeDetails._OpId;
            paramlist.OPERATION = employeeDetails._OpDesc;

            paramlist.DIVISIONID = employeeDetails._DivId;
            paramlist.DIVISION = employeeDetails._DivDesc;

            paramlist.DEPARTMENTID = employeeDetails._DepId;
            paramlist.DEPARTMENT = employeeDetails._DepDesc;

            paramlist.SECTIONID = employeeDetails._SecId;
            paramlist.SECTION = employeeDetails._SecDescrip;
            #endregion
            if (value == "Export to excel")
            {
                //Change start on 22-July-2021
                //List<ApprovedA00ListforExcel> approvedA00ListforExcel = model.ResultList.Select(x => (new ApprovedA00ListforExcel { KI_Code = x.KICODE, E_Code = x.ECode, Name = x.EmpName, Approved_Date = x.ApprovedOn, Request_Date = x.RequestDate, Operation = x.OPERATION, ProjectTitle = x.ProjectTitle, IsDeficiencyExist = x.IsDeficiencyExist, Main_PIC_Name = x.Main_PIC_Name, Allocation = x.IsAllocationExist, ApprovedBy = x.ApprovedBy, IsIT_ConfirmationExist = x.ITConfirmationStatus })).ToList();
                List<ApprovedA00ListforExcel> approvedA00ListforExcel = model.ResultList.Select(x => (new ApprovedA00ListforExcel { KI_Code = x.KICODE, E_Code = x.ECode, Name = x.EmpName, Approved_Date = x.ApprovedOn, Request_Date = x.RequestDate, Operation = x.OPERATION, ProjectTitle = x.ProjectTitle, IsDeficiencyExist = x.IsDeficiencyExist, Main_PIC_Name = x.Main_PIC_Name, Allocation = x.IsAllocationExist, ApprovedBy = x.ApprovedBy, IsIT_ConfirmationExist = x.ITConfirmationStatus, STATUSCD = x.STATUSCD })).ToList();
                //Change end on 22-July-2021

                string ExcelName = "Approved A00 List-" + Convert.ToString(DateTime.Now).Replace(" ", "_").Replace(":", "_").Replace("/", "_") + ".xlsx";
                try
                {
                    System.Data.DataTable dt = new System.Data.DataTable("Approved A00 List");
                    //Change start on 22-July-2021
                    dt.Columns.AddRange(new DataColumn[14] {
                        new DataColumn("S.No"),
                        new DataColumn("Ki"),
                        new DataColumn("Operation"),
                        new DataColumn("E Code"),
                        new DataColumn("Name"),
                        new DataColumn("Project Title"),
                        new DataColumn("Request Date"),
                        new DataColumn("Approved By"),
                        new DataColumn("Approved Date"),
                        new DataColumn("Deficiency"),
                        new DataColumn("Allocation"),
                        new DataColumn("IT Confirmation"),
                        new DataColumn("Sent back (Convert to CR)"),
                        new DataColumn("Main PIC Nam")
                    });
                    //Change end on 22-July-2021
                    int intCntr = 0;
                    foreach (var item in approvedA00ListforExcel)
                    {
                        intCntr++;
                        var Request_Date = item.Request_Date.ToString() == "01-Jan-1900 00:00:00" ? " " : item.Request_Date.ToString();
                        var Approved_Date = item.Request_Date.ToString() == "01-Jan-1900 00:00:00" ? " " : item.Request_Date.ToString();
                        String strStatus = "";

                        if (item.IsDeficiencyExist == 0)
                        {
                            strStatus = "No";
                        }
                        else
                        {
                            strStatus = "Yes";
                        }
                        string strAllocation = "";
                        if (item.Allocation == false)
                        {
                            strAllocation = "Pending";
                        }
                        else
                        {
                            strAllocation = "Allocated";
                        }
                        string strITConfirmationsts = "";
                        if (item.IsIT_ConfirmationExist == 8)
                        {
                            strITConfirmationsts = "Yes";
                        }
                        else
                        {
                            strITConfirmationsts = "No";
                        }

                        //Change start on 22-July-2021
                        string strCOnvertToCR = "";
                        if (item.STATUSCD == 125)
                        {
                            strCOnvertToCR = "Yes";
                        }
                        else
                        {
                            strCOnvertToCR = "No";
                        }
                        //dt.Rows.Add(intCntr, item.KI_Code, item.Operation, item.E_Code, item.Name, item.ProjectTitle, Request_Date, item.ApprovedBy, Approved_Date, strStatus, strAllocation, strITConfirmationsts, item.Main_PIC_Name);
                        dt.Rows.Add(intCntr, item.KI_Code, item.Operation, item.E_Code, item.Name, item.ProjectTitle, Request_Date, item.ApprovedBy, Approved_Date, strStatus, strAllocation, strITConfirmationsts, strCOnvertToCR, item.Main_PIC_Name);
                        //Change end on 22-July-2021
                    }
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ExcelName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ITDivisionApprovalSearch");
                    throw (ex);
                }
            }
            else
            {
                return View(model);
            }

        }
        #endregion
        private SearchParameterList ITDivisionApprovalgetSearchFilterData(SearchParameterList paramsList)
        {
            List<SearchParameterList> list = new List<SearchParameterList>();
            SearchParameterList data = new SearchParameterList();

            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

            #region Bind Ki drop down
            List<SearchParameterList> kiData = new List<SearchParameterList>();
            kiData.Add(new SearchParameterList() { SYKIID = 0, SYKI = "Select" });
            var kiData1 = _objHomeA00.GetA00SYKIList();
            var activeKi = kiData1._SYKIList.Where(x => x.ACTIVE == 1).FirstOrDefault();
            var kii = kiData1._SYKIList.OrderByDescending(x => x.SYKIID).Select(x => new SearchParameterList { SYKIID = x.SYKIID, SYKI = x.KICODE }).ToList();
            kiData.AddRange(kii);
            //09-Sept-2021 change start
            //var selectedKi = (paramsList.SYKIID == 0 || paramsList.SYKIID == null) ? activeKi.SYKIID : (decimal)paramsList.SYKIID;
            var selectedKi = (paramsList.SYKIID == 0 || paramsList.SYKIID == null) ? 0 : (decimal)paramsList.SYKIID;
            //09-Sept-2021 change start
            ViewBag.SYKIData = new SelectList(kiData, "SYKIID", "SYKI", selectedKi);
            #endregion

            #region Bind status drop down
            data.StatusId = paramsList.StatusId;
            var statusList = new List<SearchParameterList>() {
            new SearchParameterList {
                StatusId = 0,
                Status = "Pending"
            },
            new SearchParameterList {
                StatusId = 1,
                Status = "Allocated"
            },
            new SearchParameterList {
                StatusId = 2,
                Status = "All"
            }
            //Change start on 22-July-2021
            ,
            new SearchParameterList {
                StatusId = 3,
                Status = "Sent back (Convert to CR)"
            }
            //Change start on 22-July-2021
            };
            ViewBag.StatusList = new SelectList(statusList.OrderBy(i => i.Status).ToList(), "StatusId", "Status", paramsList.StatusId);//changed by eshant 4-jul-22 for sorting
            #endregion

            data.ECode = paramsList.ECode;
            data.EmpName = paramsList.EmpName;
            data.OPERATIONID = paramsList.OPERATIONID;
            data.DIVISIONID = paramsList.DIVISIONID;
            data.SECTIONID = paramsList.SECTIONID;
            data.DEPARTMENTID = paramsList.DEPARTMENTID;
            data.SYKIID = selectedKi;

            #region Bind Login Employee Details
            _objA00SearchModel.ADEMPCODE = userId;
            _objA00SearchModel.SYKI = selectedKi;

            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");

            _objA00SearchModel.ADEMPCODE = userId;
            _objA00SearchModel.SYKIID = selectedKi;

            var vwITApproval = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS_ITApproval(_objA00SearchModel);

            var adorglevel = _objHomeA00.GetA00_IT_ADORGLEVELList(_objA00SearchModel);
            #endregion
            #region Check IT-Division Head
            var empDetails1 = vwITApproval._A00_VW_ASSOCIATELVLDETAILS_FULL.Join(adorglevel._ADOrgLevelList.AsEnumerable(),
               b => b.DIVISIONID, c => c.ADORGLEVELID, (b, c) => new { b, c }).FirstOrDefault();
            if (empDetails1 != null)
            {
                ViewBag.isITDivisionHead = true;
                data.isITDivisionHead = "Y";
            }
            else
            {
                ViewBag.isITDivisionHead = false;
                data.isITDivisionHead = "N";
            }
            #endregion

            list.Add(data);
            ViewBag.List = list;
            return data;
        }
        private void ITDivisionGetApprovedA00SearchData(A00ViewModel model, SearchParameterList paramsData)
        {
            //SearchParameterList paramlist = new SearchParameterList();
            List<SearchResultList> empCodeList = new List<SearchResultList>();

            var userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

            model.SearchParams = new SearchParameterList();
            model.SearchParams.OPERATIONID = paramsData.OPERATIONID;
            model.SearchParams.DIVISIONID = paramsData.DIVISIONID;
            model.SearchParams.DEPARTMENTID = paramsData.DEPARTMENTID;
            model.SearchParams.SECTIONID = paramsData.SECTIONID;
            var sykiId = paramsData.SYKIID;//0;

            // SearchParams.StatusId
            model.SearchParams.StatusId = paramsData.StatusId;
            var empCode = string.IsNullOrEmpty(paramsData.ECode) ? 0 : Convert.ToInt64(paramsData.ECode);
            var empName = paramsData.EmpName;
            model.SearchParams.ECode = paramsData.ECode;
            model.SearchParams.EmpName = empName;
            model.SearchParams.SYKIID = sykiId;
            //paramlist.SYKIID = sykiId;
            //paramlist.ECode = paramsData.ECode;
            //paramlist.EmpName = empName;

            //paramlist.StatusId = model.SearchParams.StatusId;
            model.SearchParams.SYKI = paramsData.SYKI;
            model.SearchParams.Status = paramsData.Status;

            var status = model.SearchParams.StatusId;

            #region Filter Query
            var kiData1 = _objHomeA00.GetA00SYKIList();

            //(  105 ---See)
            var resultList = _objHomeA00.GetA00APPROVALFullTable()._A00APPROVALList.
                Where(x => x.OHAPPDATE != null).AsEnumerable().
               Join(_objHomeA00.GetA00DtlFullTable()._A00DtlViewModelList,
               a => a.A00DTLTBID, b => b.A00DTLTBID, (a, b) => new { A00APPROVAL = a, A00DTLTB = b }).
               Join(_objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE, c => c.A00DTLTB.ADDEDBY, d => d.ADEMPCODE, (c, d) => new { c, d })
               .Where(f =>
              //Change start on 22-July-2021
              //(status == 2 || (status == 0 ? !_objHomeA00.GetA00Allocation().Contains(f.c.A00DTLTB.A00DTLTBID) : _objHomeA00.GetA00Allocation().Contains(f.c.A00DTLTB.A00DTLTBID))) &&
              //(status != 3 ? (status == 2 || (status == 0 ? !_objHomeA00.GetA00Allocation().Contains(f.c.A00DTLTB.A00DTLTBID) : _objHomeA00.GetA00Allocation().Contains(f.c.A00DTLTB.A00DTLTBID))) : 0 == 0) &&
              (status == 3 || status == 2 || (status == 0 ? !_objHomeA00.GetA00Allocation().Contains(f.c.A00DTLTB.A00DTLTBID) : _objHomeA00.GetA00Allocation().Contains(f.c.A00DTLTB.A00DTLTBID))) &&
            //Change end on 22-July-2021
            (string.IsNullOrEmpty(empName) ? true : ((f.d.FIRSTNAME + " " + f.d.LASTNAME).Contains(empName))) &&

            //Change start on 22-July-2021
            //(f.c.A00DTLTB.ADDEDBY == (empCode == 0 ? f.c.A00DTLTB.ADDEDBY : empCode) && (f.c.A00DTLTB.STATUSCD == 105)
            (f.c.A00DTLTB.ADDEDBY == (empCode == 0 ? f.c.A00DTLTB.ADDEDBY : empCode) &&
            (status == 3 ? f.c.A00DTLTB.STATUSCD == 125 : (status == 2 ? f.c.A00DTLTB.STATUSCD == 125 || f.c.A00DTLTB.STATUSCD == 105 : f.c.A00DTLTB.STATUSCD == 105))
          //Change end on 22-July-2021
          )

            && (f.c.A00DTLTB.SYKIID == (sykiId == 0 ? f.c.A00DTLTB.SYKIID : sykiId))
            && ((paramsData.OPERATIONID == 0 || paramsData.OPERATIONID == null) || f.c.A00DTLTB.OPERATION == paramsData.OPERATIONID)
            && ((paramsData.DIVISIONID == 0 || paramsData.DIVISIONID == null) || f.c.A00DTLTB.DIVISION == paramsData.DIVISIONID)
            && ((paramsData.DEPARTMENTID == 0 || paramsData.DEPARTMENTID == null) || f.c.A00DTLTB.DEPARTMENT == paramsData.DEPARTMENTID)
            && ((paramsData.SECTIONID == 0 || paramsData.SECTIONID == null) || f.c.A00DTLTB.SECTION == paramsData.SECTIONID)).
           Select(s1 => new SearchResultList
           {
               A00DTLTBID = s1.c.A00DTLTB.A00DTLTBID,
               SYKIID = s1.c.A00DTLTB.SYKIID,
               KICODE = kiData1._SYKIList.Where(x1 => x1.SYKIID == s1.c.A00DTLTB.SYKIID).Select(x1 => x1.KICODE).FirstOrDefault(),
               ECode = s1.c.A00DTLTB.ADDEDBY,
               EmpName = s1.d.FIRSTNAME + " " + s1.d.LASTNAME,
               ProjectTitle = s1.c.A00DTLTB.PRJCTTLE,
               RequestDate = s1.c.A00DTLTB.DATEADDEDID,
               ApprovedBy = _objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE.Where(x1 => x1.ADEMPCODE == s1.c.A00APPROVAL.OHID).Select(x1 => x1.FIRSTNAME + " " + x1.LASTNAME).FirstOrDefault(),
               ApprovedOn = (s1.c.A00APPROVAL.OHAPPDATE),
               OPERATION = _objHomeA00.Getoperationdetails(s1.c.A00DTLTB.ADDEDBY, (int)s1.c.A00DTLTB.SYKIID),
               IsDeficiencyClosed = _objHomeA00.IsA00DeficiencyClosed(s1.c.A00DTLTB.A00DTLTBID, s1.c.A00DTLTB.SYKIID),
               DeficiencyStatus = _objHomeA00.GetDeficiencyStatus(s1.c.A00DTLTB.A00DTLTBID, s1.c.A00DTLTB.SYKIID),
               IsAllocationExist = _objHomeA00.IsAllocationExist(s1.c.A00DTLTB.A00DTLTBID, s1.c.A00DTLTB.SYKIID),
               Main_PIC_Name = _objHomeA00.GetAllocationDetails(s1.c.A00DTLTB.A00DTLTBID, s1.c.A00DTLTB.SYKIID).Select(x => x.MainPic).FirstOrDefault(),
               ITConfirmationStatus = _objHomeA00.IsITConfirmationDone(s1.c.A00DTLTB.A00DTLTBID, s1.c.A00DTLTB.SYKIID),
               IsDeficiencyExist = _objHomeA00.IsDeficiencyExist(s1.c.A00DTLTB.A00DTLTBID, s1.c.A00DTLTB.SYKIID),
               //Change start on 22-July-2021
               STATUSCD = s1.c.A00DTLTB.STATUSCD
               //Change end on 22-July-2021
           }).ToList();
            #endregion

            model.ResultList = resultList.OrderByDescending(x => x.RequestDate).ThenBy(x => x.ProjectTitle).ToList();
        }

        public ActionResult BindOperationDivisionDepartmentSectionBySykiId(long SYKIID)
        {
            List<SearchParameterList> list = new List<SearchParameterList>();
            SearchParameterList data = new SearchParameterList();
            _objA00SearchModel = new A00SearchModel();
            var userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            _objA00SearchModel.ADEMPCODE = userId;
            _objA00SearchModel.SYKI = (decimal)SYKIID;

            var empDetails = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel);
            if (empDetails != null)
            {
                data.OPERATION = empDetails.OPERATION != null ? empDetails.OPERATION : "--No record--";
                data.OPERATIONID = empDetails.OPERATIONID != null ? empDetails.OPERATIONID : 0;
                data.DIVISION = empDetails.DIVISION != null ? empDetails.DIVISION : "--No record--";
                data.DIVISIONID = empDetails.DIVISIONID != null ? empDetails.DIVISIONID : 0;
                data.DEPARTMENT = empDetails.DEPARTMENT != null ? empDetails.DEPARTMENT : "--No record--";
                data.DEPARTMENTID = empDetails.DEPARTMENTID != null ? empDetails.DEPARTMENTID : 0;
                data.SECTION = empDetails.SECTION != null ? empDetails.SECTION : "--No record--";
                data.SECTIONID = empDetails.SECTIONID != null ? empDetails.SECTIONID : 0;
                list.Add(data);
            }

            List<SearchParameterList> divList = list;
            return Json(divList);
        }

        public ActionResult BindOperationByUserIDandSykiId(long SYKIID)
        {
            List<SearchParameterList> _opList = new List<SearchParameterList>();
            var userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            _objA00SearchModel.ADEMPCODE = userId;
            _objA00SearchModel.SYKI = (decimal)SYKIID;
            var empDetails = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel);
            if (empDetails != null)
            {
                _opList = new List<SearchParameterList>
                {
                    new SearchParameterList
                    {
                         OPERATION = empDetails.OPERATION, OPERATIONID = empDetails.OPERATIONID,
                         DIVISION = empDetails.DIVISION, DIVISIONID = empDetails.DIVISIONID,
                         DEPARTMENT = empDetails.DEPARTMENT, DEPARTMENTID = empDetails.DEPARTMENTID,
                         SECTION = empDetails.SECTION, SECTIONID = empDetails.SECTIONID
                    }
                };
            }
            //List<A00ADORGLEVELList> _opList1 = _objHomeA00.GetA00ADORGLEVELList((decimal)SYKIID)._ADOrgLevelList;
            return Json(_opList);
        }
        public ActionResult BindOperationBySykiId(long SYKIID)
        {
            List<A00ADORGLEVELList> _opList = _objHomeA00.GetA00ADORGLEVELList((decimal)SYKIID)._ADOrgLevelList;
            //ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP");
            return Json(_opList);
        }
        public ActionResult BindDivisionByOperationId(long id, long SYKIID)
        {
            List<SearchParameterList> divList = _objHomeA00.BindDivision(id, SYKIID);
            return Json(divList);
        }
        public ActionResult BindDeptByDivisionId(long id, long SYKIID)
        {
            List<SearchParameterList> depList = _objHomeA00.BindDepartment(id, 0, SYKIID);
            return Json(depList);
        }
        public ActionResult BindSecByDepartmentId(long id, long SYKIID)
        {
            List<SearchParameterList> SecList = _objHomeA00.BindSection(id, 0, 0, SYKIID);
            return Json(SecList);
        }

        //[HttpPost]
        //public ActionResult InsertDeffienceforbind()
        //{
        //    var currentTime = DateTime.Now;
        //    A00Deficiency objdeficiency = new A00Deficiency();
        //    objdeficiency.AddedBY = Convert.ToInt32(_sessionService.Get<string>("userID"));
        //    objdeficiency.AddedDate = DateTime.Now;
        //    _objHomeA00.InsertDeffience(objdeficiency);
        //    return Json(objdeficiency, JsonRequestBehavior.AllowGet);
        //}


        //public ActionResult BindEmployee()
        //{


        //    List<ADORGLEVEL> _opList = _objHomeA00.GetA00ADEMPLOYEE();
        //    ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP", paramlist.OPERATIONID);



        //    var empData = _objHomeA00.GetA00ADEMPLOYEE().
        //   _A00ADEMPLOYEE.Where(x1 => x1.ADEMPCODE == x1.ADEMPCODE).
        //    Select(x1 => x1.FIRSTNAME + x1.LASTNAME).ToList();
        //    return Json(empData, JsonRequestBehavior.AllowGet);
        //}

        [HttpGet]
        public ActionResult ITDivisionView(string returnMsg = null, long? A00DTLTBID = null, string type = null,
            long? OPERATIONID = null, long? DIVISIONID = null, long? DEPARTMENTID = null, long? SECTIONID = null, decimal? SYKIID = null,
            string ECODE = null, string EMPNAME = null, short? STATUSID = null)
        {
            try
            {
                TempData["PageHead"] = "Approved A00 Details";
                var kiData1 = _objHomeA00.GetA00SYKIList();
                List<SYKI> iList = new List<SYKI>();
                iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });

                var kiData = (from data in kiData1._SYKIList select data).ToList();
                foreach (var item in kiData)
                {
                    iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                }

                var ActiveKiID = SYKIID > 0 ? SYKIID : kiData.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();

                SetSearchValue(OPERATIONID, DIVISIONID, DEPARTMENTID, SECTIONID, ActiveKiID, ECODE, EMPNAME, STATUSID);
                if (returnMsg != null)
                {
                    ViewBag.Msg = returnMsg;
                }
                ViewBag.DeficiencyDetails = _objHomeA00.GetDeficiencyRaisedDetails(A00DTLTBID, ActiveKiID);
                ViewBag.DeficiencyStatus = _objHomeA00.GetDeficiencyStatus(A00DTLTBID, ActiveKiID);


                TempData["PageHead"] = "A00 IT Approval";
                ViewBag.AllocationDetails = _objHomeA00.GetAllocationDetails(A00DTLTBID, ActiveKiID);
                var A00ItConfirmationID = _objHomeA00.GetA00ItConfirmationDetails(A00DTLTBID, (decimal)ActiveKiID).ITCONFIRMATIONID;
                ViewBag.ITConfirmationDetails = _objHomeA00.GetA00ItConfirmationHistory(A00ItConfirmationID);
                ViewBag.Years = DateTime.Now.Year;
                type = type == null ? "NEW" : type.ToUpper();
                ViewBag.pageType = type;
                ViewBag.actionType = "Submit";
                ViewBag.isUploadedFilesExist = "N";
                int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

                #region In New Mode

                var operation = _objHomeA00.GetA00ADORGLEVELList(ActiveKiID);
                List<SearchResultList> budgByList = new List<SearchResultList>();
                budgByList.Add(new SearchResultList { LEVELDESCRIP = "Select", ADORGLEVELID = 0 });
                foreach (var item in operation._ADOrgLevelList)
                {
                    budgByList.Add(new SearchResultList { LEVELDESCRIP = item.LEVELDESCRIP, ADORGLEVELID = item.ADORGLEVELID });
                }


                List<SelectListItem> datalist = new List<SelectListItem>();
                var InvforcastList = _objHomeA00.GetA00_INVFORCASTList();

                foreach (var Datat4 in InvforcastList._InvforcastList)
                {
                    datalist.Add(new SelectListItem { Text = Datat4.INCFORCASTDETAIL, Value = Datat4.INVFORCASTID.ToString() });
                }
                ViewBag.SKYLIST = datalist;



                A00ViewModel objModel = new A00ViewModel();
                objModel.getAllDaysList = objModel.getAllWeekDaysList();
                objModel.GetAllmonth = objModel.getMonth();
                objModel.Getdate = objModel.GetAllDate();
                objModel.SYKI = (decimal)ActiveKiID;
                objModel.A00DTLTBID = (long)A00DTLTBID;
                #endregion
                ViewBag.isOtherSelected = "N";

                #region When View/Action Button Pressed
                if (type == "VIEW")
                {
                    _objA00SearchModel.a00dtltbid = A00DTLTBID;
                    var _A00ExistData = _objHomeA00.GetA00Dtl(_objA00SearchModel);
                    objModel.Projecttitle = _A00ExistData.PRJCTTLE;
                    objModel.Backgrounds = _A00ExistData.PRJCTTXT;
                    objModel.BusinessKPI = _A00ExistData.BUKPITXT;
                    objModel.PurposeA00 = _A00ExistData.PURPSTXT;
                    objModel.TargetA00 = _A00ExistData.TRGTINDCD;
                    objModel.RequirmentA00 = _A00ExistData.RQUMTTXT;
                    objModel.ImagePath = _A00ExistData.ATTACHMENT;
                    objModel.BudgetedSelected = _A00ExistData.BUDGETFLG.ToString();
                    objModel.StartDate = _A00ExistData.STARTDT?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objModel.EndDate = _A00ExistData.ENDDT?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objModel.Ifothers = _A00ExistData.FRCSTOTHER;
                    ViewBag.request_date = _A00ExistData.DATEADDEDID.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objModel.ADDEDBY1 = _objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE.Where(x1 => x1.ADEMPCODE == _A00ExistData.ADDEDBY).Select(x1 => x1.FIRSTNAME + " " + x1.LASTNAME).FirstOrDefault();
                    ViewBag.userName = objModel.ADDEDBY1;
                    //var vwList = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS_FullList();
                    //objModel.OPERATION_PROPOSING = vwList._A00_VW_ASSOCIATELVLDETAILS_FULL.Where(x => x.SYKI == KIID && x.ADEMPCODE == _A00ExistData.ADDEDBY).Select(x1 => x1.OPERATION).FirstOrDefault();

                    _objA00SearchModel.ADEMPCODE = _A00ExistData.ADDEDBY;
                    _objA00SearchModel.SYKI = (long)_A00ExistData.SYKIID;
                    objModel.OPERATION_PROPOSING = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel).OPERATION;

                    //Change start on 22-July-2021
                    if (_A00ExistData.STATUSCD == 125)
                    {
                        ViewBag.A00ConvertedToCRDtl = _objHomeA00.getConvertedToCRDtl(A00DTLTBID);
                    }
                    else
                    {
                        ViewBag.A00ConvertedToCRDtl = null;
                    }
                    //Change end on 22-July-2021

                    ViewBag.OPERATION_P = objModel.OPERATION_PROPOSING;


                    if (A00DTLTBID != null)
                    {
                        ViewBag.actionType = "View";
                        this.generateApprovalHistory(A00DTLTBID ?? 0);
                    }

                    List<SelectListItem> mlist = new List<SelectListItem>();
                    mlist.Add(new SelectListItem { Text = "Select", Value = "", Disabled = true });

                    if (_A00ExistData.BUDGETFLG.ToString() == "1")
                    {
                        mlist.Add(new SelectListItem { Text = "Yes", Value = "10", Selected = true });
                        mlist.Add(new SelectListItem { Text = "No", Value = "11" });
                    }
                    else if (_A00ExistData.BUDGETFLG.ToString() == "0")
                    {
                        mlist.Add(new SelectListItem { Text = "No", Value = "11", Selected = true });
                        mlist.Add(new SelectListItem { Text = "Yes", Value = "10" });
                    }

                    ViewBag.BudgetedValue = mlist;
                    ViewBag.selectedBudgetedVal = _A00ExistData.BUDGETFLG.ToString();
                    var selectedBudgBy = budgByList.Where(x => x.ADORGLEVELID == _A00ExistData.ADORGLEVELID).FirstOrDefault();
                    ViewBag.List = new SelectList(budgByList, "ADORGLEVELID", "LEVELDESCRIP", selectedBudgBy?.ADORGLEVELID);
                    var selectedSyKI = iList.Where(x => x.SYKIID == _A00ExistData.BUDGETSYKIID).FirstOrDefault();
                    ViewBag.SKY = new SelectList(iList, "SYKIID", "KICODE", selectedSyKI?.SYKIID);



                    var A00InvForcast = _objHomeA00.GetA00INVFORCASTList();

                    var ief = A00InvForcast._A00InvforcastList.Join(InvforcastList._InvforcastList, a => a.INVFORCASTID, b => b.INVFORCASTID, (a, b) => new { a, b }).
                      Where(x => x.a.A00DTLTBID == A00DTLTBID).Select(m => new { INVFORCASTID = m.a.INVFORCASTID, INCFORCASTDETAIL = m.b.INCFORCASTDETAIL }).Distinct().ToList();
                    if (type == "ACTION")
                    {
                        datalist.Where(x => ief.Select(y => y.INVFORCASTID.ToString()).ToList().Contains(x.Value)).ToList().ForEach(z => z.Selected = true);
                        if (_A00ExistData.FRCSTOTHER != null)
                        {
                            var OtherSelected = InvforcastList._InvforcastList.Where(x => x.INCFORCASTDETAIL.ToUpper() == "OTHERS").FirstOrDefault();
                            datalist.Where(x => x.Value == OtherSelected.INVFORCASTID.ToString()).Select(x => x.Selected = true);
                            ViewBag.isOtherSelected = "Y";
                        }
                        ViewBag.SKYLIST = datalist;
                        //string filePath = Path.Combine(serverpath.getFileUploadPath(),"A00");
                        string filePath = Path.Combine(serverpath.getFileUploadPath(), "A00");
                        string FileName = Path.GetFileNameWithoutExtension(_A00ExistData.ATTACHMENT);
                        string FileExtension = Path.GetExtension(_A00ExistData.ATTACHMENT);

                        FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;
                    }
                    else
                    {
                        List<SelectListItem> Vdatalist = new List<SelectListItem>();
                        foreach (var Datat4 in ief)
                        {
                            Vdatalist.Add(new SelectListItem { Text = Datat4.INCFORCASTDETAIL, Value = Datat4.INVFORCASTID.ToString() });
                        }
                        if (_A00ExistData.FRCSTOTHER != null)
                        {
                            var OtherSelected = InvforcastList._InvforcastList.Where(x => x.INCFORCASTDETAIL.ToUpper() == "OTHERS").FirstOrDefault();
                            Vdatalist.Add(new SelectListItem { Text = OtherSelected.INCFORCASTDETAIL.ToString(), Value = OtherSelected.INVFORCASTID.ToString() });
                            ViewBag.isOtherSelected = "Y";
                        }
                        ViewBag.SKYLIST = Vdatalist;
                    }
                }
                else
                {
                    List<SelectListItem> mlist = new List<SelectListItem>();
                    mlist.Add(new SelectListItem { Text = "Select", Selected = true });
                    mlist.Add(new SelectListItem { Text = "Yes", Value = "10" });
                    mlist.Add(new SelectListItem { Text = "No", Value = "11" });
                    ViewBag.BudgetedValue = mlist;
                    ViewBag.List = new SelectList(budgByList, "ADORGLEVELID", "LEVELDESCRIP", 0);
                    ViewBag.SKY = new SelectList(iList, "SYKIID", "KICODE", 0);
                }
                #endregion

                //var activeKi = kiData1._SYKIList.Where(x => x.ACTIVE == 1).FirstOrDefault();

                //InsertDeffienceforbind();
                //BindEmployee();

                return View(objModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ITDivisionView");
                return View();
            }
        }
        public void ITDivisionApprovalSetSearchValue(long? OPERATIONID = null, long? DIVISIONID = null, long? DEPARTMENTID = null,
            long? SECTIONID = null, decimal? SYKIID = null, string ECODE = null, string EMPNAME = null, short? STATUSID = null)
        {
            ViewBag.OPERATIONID = OPERATIONID;
            ViewBag.DIVISIONID = DIVISIONID;
            ViewBag.DEPARTMENTID = DEPARTMENTID;
            ViewBag.SECTIONID = SECTIONID;
            ViewBag.SYKIID = SYKIID;
            ViewBag.ECODE = ECODE;
            ViewBag.EMPNAME = EMPNAME;
            ViewBag.STATUSID = STATUSID;
        }
        private void ITDivisionApprovalgenerateApprovalHistory(long A00DTLTBID)
        {
            //09-Sept-2021 change start
            List<ApprovalViewHistoryModel> history = new List<ApprovalViewHistoryModel>();
            var approvalRecord = _objHomeA00.GetA00APPROVALFullTable()._A00APPROVALList.Where(x => x.A00DTLTBID == A00DTLTBID).FirstOrDefault();
            var idList = new List<long?> { approvalRecord.DEPTHDID, approvalRecord.COORDDID, approvalRecord.DIVHDHDID, approvalRecord.EXECOHDID, approvalRecord.OHID, approvalRecord.PPCHOOHID };

            var empData = _objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE.Where(x => idList.Contains(x.ADEMPCODE)).Select(y => new { Id = y.ADEMPCODE, Name = y.FIRSTNAME + " " + y.LASTNAME }).ToList();
            history.Add(
                       new ApprovalViewHistoryModel
                       {
                           DEPTHDID = approvalRecord.DEPTHDAPPDATE != null ? approvalRecord.DEPTHDID : null,
                           DEPTHDAPPDATE = approvalRecord.DEPTHDAPPDATE,
                           DEPTHDAPPTXT = approvalRecord.DEPTHDAPPTXT,
                           COORDDID = approvalRecord.COORDDID,
                           COORDAPPDATE = approvalRecord.COORDAPPDATE != null ? approvalRecord.COORDAPPDATE : null,
                           COORDAPPTXT = approvalRecord.COORDAPPTXT,
                           DIVHDHDID = approvalRecord.DIVHDAPPDATE != null ? approvalRecord.DIVHDHDID : null,
                           DIVHDAPPDATE = approvalRecord.DIVHDAPPDATE,
                           DIVHDAPPTXT = approvalRecord.DIVHDAPPTXT,
                           EXECOHDID = approvalRecord.EXECOAPPDATE != null ? approvalRecord.EXECOHDID : null,
                           EXECOAPPDATE = approvalRecord.EXECOAPPDATE,
                           EXECOAPPTXT = approvalRecord.EXECOAPPTXT,
                           OHID = approvalRecord.OHAPPDATE != null ? approvalRecord.OHID : null,
                           OHAPPDATE = approvalRecord.OHAPPDATE,
                           OHAPPTXT = approvalRecord.OHAPPTXT,
                           PPCHOOHID = approvalRecord.PPCHOOHDATE != null ? approvalRecord.PPCHOOHID : null,
                           PPCHOOHDATE = approvalRecord.PPCHOOHDATE,
                           PPCHOOHTXT = approvalRecord.PPCHOOHTXT
                       });
            history[0].DeptHeadName = empData.Where(x => x.Id == history[0].DEPTHDID).Select(y => y.Name).FirstOrDefault();
            history[0].CoOrdName = empData.Where(x => x.Id == history[0].COORDDID).Select(y => y.Name).FirstOrDefault();
            history[0].DivHeadName = empData.Where(x => x.Id == history[0].DIVHDHDID).Select(y => y.Name).FirstOrDefault();
            history[0].ExeCoOrdName = empData.Where(x => x.Id == history[0].EXECOHDID).Select(y => y.Name).FirstOrDefault();
            history[0].OpHeadName = empData.Where(x => x.Id == history[0].OHID).Select(y => y.Name).FirstOrDefault();
            history[0].PPCHOOHName = empData.Where(x => x.Id == history[0].PPCHOOHID).Select(y => y.Name).FirstOrDefault();

            ViewBag.ApprovalHistory = history;
            //09-Sept-2021 change end
        }
        public ActionResult ITDivisionApprovalSearchBack(string returnMsgOnSearch = null, long? OPERATIONID = null, long? DIVISIONID = null,
            long? DEPARTMENTID = null, long? SECTIONID = null, decimal? SYKIID = null, string ECODE = null, string EMPNAME = null, short? STATUSID = null)
        {
            SearchParameterList paramlist = new SearchParameterList();

            //SearchResultsViewModel model = new SearchResultsViewModel();
            A00ViewModel model = new A00ViewModel();
            model.SearchParams = new SearchParameterList();

            model.SearchParams.OPERATIONID = Convert.ToInt32(OPERATIONID?.ToString());
            model.SearchParams.DIVISIONID = Convert.ToInt32(DIVISIONID?.ToString());
            model.SearchParams.DEPARTMENTID = Convert.ToInt32(DEPARTMENTID?.ToString());
            model.SearchParams.SECTIONID = Convert.ToInt32(SECTIONID?.ToString());
            model.SearchParams.SYKIID = Convert.ToInt32(SYKIID?.ToString());
            model.SearchParams.ECode = ECODE?.ToString();
            model.SearchParams.EmpName = EMPNAME?.ToString();
            model.SearchParams.StatusId = Convert.ToInt16(STATUSID?.ToString());

            #region Block-1
            int operation, division, department, section;
            var userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            operation = Convert.ToInt32(OPERATIONID?.ToString());
            division = Convert.ToInt32(DIVISIONID?.ToString());
            department = Convert.ToInt32(DEPARTMENTID?.ToString());
            section = Convert.ToInt32(SECTIONID?.ToString());
            var sykiId = Convert.ToInt32(SYKIID?.ToString());
            var empCode = Convert.ToInt64(ECODE?.ToString());
            var empName = EMPNAME?.ToString();
            paramlist.SYKIID = SYKIID;
            paramlist.ECode = model.SearchParams.ECode;
            paramlist.EmpName = EMPNAME;
            paramlist.StatusId = model.SearchParams.StatusId;
            #endregion

            var kiData1 = _objHomeA00.GetA00SYKIList();

            var paramData = ITDivisionApprovalgetSearchFilterData(paramlist);

            model.SearchParams.SYKI = paramData.SYKI;
            model.SearchParams.Status = paramData.Status;

            #region Filter Query
            var activeKi = kiData1._SYKIList.Where(x => x.ACTIVE == 1).FirstOrDefault();
            //var vwList = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS_FullList()._A00_VW_ASSOCIATELVLDETAILS_FULL;
            //var vwList1 = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS_FullList();
            var resultList = _objHomeA00.GetA00APPROVALFullTable()._A00APPROVALList.Where(x => x.OHAPPDATE != null).AsEnumerable().
            Join(_objHomeA00.GetA00DtlFullTable()._A00DtlViewModelList, a => a.A00DTLTBID, b => b.A00DTLTBID, (a, b) => new { A00APPROVAL = a, A00DTLTB = b }).
            Join(_objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE, c => c.A00DTLTB.ADDEDBY, d => d.ADEMPCODE, (c, d) => new { c, d }).Where(f =>
             (string.IsNullOrEmpty(empName) ? true : ((f.d.FIRSTNAME + " " + f.d.LASTNAME).Contains(empName))) && (f.c.A00DTLTB.ADDEDBY ==
             (empCode == 0 ? f.c.A00DTLTB.ADDEDBY : empCode)) && (f.c.A00DTLTB.SYKIID == (sykiId == 0 ? f.c.A00DTLTB.SYKIID : sykiId))).
            Select(s1 => new SearchResultList
            {
                A00DTLTBID = s1.c.A00DTLTB.A00DTLTBID,
                SYKIID = s1.c.A00DTLTB.SYKIID,
                KICODE = kiData1._SYKIList.Where(x1 => x1.SYKIID == s1.c.A00DTLTB.SYKIID).Select(x1 => x1.KICODE).FirstOrDefault(),
                ECode = s1.c.A00DTLTB.ADDEDBY,
                EmpName = s1.d.FIRSTNAME + " " + s1.d.LASTNAME,
                ProjectTitle = s1.c.A00DTLTB.PRJCTTLE,
                RequestDate = s1.c.A00DTLTB.DATEADDEDID,
                ApprovedBy = _objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE.Where(x1 => x1.ADEMPCODE == s1.c.A00APPROVAL.OHID).Select(x1 => x1.FIRSTNAME + " " + x1.LASTNAME).FirstOrDefault(),
                ApprovedOn = (s1.c.A00APPROVAL.OHAPPDATE),
                OPERATION = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(new A00SearchModel { ADEMPCODE = s1.c.A00DTLTB.ADDEDBY, SYKIID = (long)s1.c.A00DTLTB.SYKIID }).OPERATION,
                //OPERATION = vwList1._A00_VW_ASSOCIATELVLDETAILS_FULL.Where(x => x.ADEMPCODE == s1.c.A00DTLTB.ADDEDBY).Select(x1 => x1.OPERATION).FirstOrDefault()

            }).ToList();
            #endregion

            model.ResultList = resultList.OrderByDescending(x => x.RequestDate).ThenBy(x => x.ProjectTitle).ToList();
            paramlist.SYKI = kiData1._SYKIList.Where(x => x.SYKIID == paramlist.SYKIID).Select(x => x.KICODE).FirstOrDefault();

            return View("ITDivisionApprovalSearch", model);
        }

        public ActionResult ViewA001(string returnMsg = null, long? A00DTLTBID = null, string type = null, long? OPERATIONID = null, long? DIVISIONID = null, long? DEPARTMENTID = null, long? SECTIONID = null, decimal? SYKIID = null, string ECODE = null, string EMPNAME = null, short? STATUSID = null, bool? isReset = null)
        {
            try
            {
                if (isReset == true)
                {
                    if (A00DTLTBID != 0 && A00DTLTBID != null)
                    {
                        _objA00SearchModel.a00dtltbid = A00DTLTBID;
                        var record = _objHomeA00.GetA00Dtl(_objA00SearchModel);
                        //var record = _Assetdatabasecontext.A00DTLTB.Where(x => x.A00DTLTBID == A00DTLTBID).FirstOrDefault();
                        if (record.STATUSCD == 10)
                        {
                            return RedirectToAction("ViewA001", new
                            {
                                A00DTLTBID = A00DTLTBID,
                                Type = type,
                                OPERATIONID = OPERATIONID,
                                DIVISIONID = DIVISIONID,
                                DEPARTMENTID = DEPARTMENTID,
                                SECTIONID = SECTIONID,
                                SYKIID = SYKIID,
                                ECODE = ECODE,
                                EMPNAME = EMPNAME,
                                STATUSID = STATUSID
                            });
                        }
                        else
                        {
                            return RedirectToAction("ViewA001");
                        }
                    }
                    else
                    {
                        return RedirectToAction("ViewA001");
                    }
                }
                else
                {
                    List<SYKI> iList = new List<SYKI>();
                    iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });

                    var kiData = _objHomeA00.GetLatestA00SYKIList();
                    foreach (var item in kiData._SYKIList.Where(x => x.ACTIVE == 1))
                    {
                        iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                    }
                    var ActiveKiID = SYKIID > 0 ? SYKIID : kiData._SYKIList.Where(x => x.ACTIVE == 1).OrderByDescending(x => x.SYKIID).Select(x => x.SYKIID).FirstOrDefault();

                    ViewBag.A00DTLTBID = A00DTLTBID;
                    SetSearchValue(OPERATIONID, DIVISIONID, DEPARTMENTID, SECTIONID, SYKIID, ECODE, EMPNAME, STATUSID);
                    if (returnMsg != null)
                    {
                        ViewBag.Msg = returnMsg;
                    }

                    TempData["PageHead"] = "A00 Concept Form";
                    ViewBag.Years = DateTime.Now.Year;
                    type = type == null ? "NEW" : type.ToUpper();
                    ViewBag.pageType = type;
                    ViewBag.actionType = "Submit";
                    ViewBag.isUploadedFilesExist = "N";
                    #region In New Mode 
                    //Added on 06-07-2022 for Manintaining the Budget By Drop down for New and FIlled A00-Eshant
                    Int64 currentActiveKi = 0;
                    if (type == "NEW")
                    {
                        currentActiveKi = _objHomeA00.GetCurrentActiveKi();
                    }
                    else
                    {
                        currentActiveKi = (long)SYKIID;
                    }
                    var operation = _objHomeA00.GetA00ADORGLEVELList(currentActiveKi);
                    //Close End 06-07-2022 
                    List<SearchResultList> budgByList = new List<SearchResultList>();
                    budgByList.Add(new SearchResultList { LEVELDESCRIP = "Select", ADORGLEVELID = 0 });
                    var budgByLis1 = operation._ADOrgLevelList.Select(x => new SearchResultList
                    {
                        LEVELDESCRIP = x.LEVELDESCRIP,
                        ADORGLEVELID = x.ADORGLEVELID
                    }).OrderBy(x => x.LEVELDESCRIP).ToList();
                    budgByList.AddRange(budgByLis1);

                    List<SelectListItem> datalist = new List<SelectListItem>();

                    var InvforcastList = _objHomeA00.GetA00_INVFORCASTList();
                    foreach (var Datat4 in InvforcastList._InvforcastList)
                    {
                        datalist.Add(new SelectListItem { Text = Datat4.INCFORCASTDETAIL, Value = Datat4.INVFORCASTID.ToString() });
                    }
                    ViewBag.SKYLIST = datalist;


                    A00ViewModel objModel = new A00ViewModel();
                    objModel.getAllDaysList = objModel.getAllWeekDaysList();
                    objModel.GetAllmonth = objModel.getMonth();
                    objModel.Getdate = objModel.GetAllDate();
                    #endregion
                    ViewBag.isOtherSelected = "N";

                    var A00InvForcast = _objHomeA00.GetA00INVFORCASTList();
                    #region When View/Action Button Pressed
                    if (type == "VIEW" || type == "ACTION")
                    {
                        //var _A00ExistData = _Assetdatabasecontext.A00DTLTB.Where(x => x.A00DTLTBID == A00DTLTBID).FirstOrDefault();
                        _objA00SearchModel.a00dtltbid = A00DTLTBID;
                        var _A00ExistData = _objHomeA00.GetA00Dtl(_objA00SearchModel);

                        objModel.Projecttitle = _A00ExistData.PRJCTTLE;
                        objModel.Backgrounds = _A00ExistData.PRJCTTXT;
                        objModel.BusinessKPI = _A00ExistData.BUKPITXT;
                        objModel.PurposeA00 = _A00ExistData.PURPSTXT;
                        objModel.TargetA00 = _A00ExistData.TRGTINDCD;
                        objModel.RequirmentA00 = _A00ExistData.RQUMTTXT;
                        objModel.ImagePath = _A00ExistData.ATTACHMENT;
                        objModel.BudgetedSelected = _A00ExistData.BUDGETFLG.ToString();
                        objModel.StartDate = _A00ExistData.STARTDT?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        objModel.EndDate = _A00ExistData.ENDDT?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        objModel.Ifothers = _A00ExistData.FRCSTOTHER;
                        //Added by Eshant For Bugeted By Dropdown 13-06-22 on View001
                        objModel.ADORGLEVELID = Convert.ToInt32(_A00ExistData.OPERATION);

                        //Change start on 22-July-2021
                        if (_A00ExistData.STATUSCD == 125)
                        {
                            ViewBag.A00ConvertedToCRDtl = _objHomeA00.getConvertedToCRDtl(A00DTLTBID);
                        }
                        else
                        {
                            ViewBag.A00ConvertedToCRDtl = null;
                        }
                        //Change end on 22-July-2021

                        if (A00DTLTBID != null)
                        {
                            if (type == "ACTION")
                            {
                                var currStatus = _A00ExistData.STATUSCD;
                                //09-Sept-2021 change start
                                //var sentBackOrSaveDraft = new List<int> { 10, 35, 55, 75, 95, 115 }; //115 added
                                var sentBackOrSaveDraft = new List<int> { 10, 35, 55, 75, 95, 115, 116 }; //116 added
                                //09-Sept-2021 change end

                                if (sentBackOrSaveDraft.Contains(currStatus))
                                {
                                    ViewBag.actionType = "Submit";
                                }
                                else
                                {
                                    //SA CR7306 Updated Code
                                    int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
                                    var lst = _objHomeA00.GetA00_ADORGCOORDINATORLIST(userId, "CO").Select(x => x.ADORGLEVELID).ToList();
                                    if (lst.Count > 0)
                                    {
                                        ViewBag.actionType = "Approval";
                                        this.generateApprovalHistory(A00DTLTBID ?? 0);
                                    }
                                    else
                                    {
                                        //Utilities.UserRole userData = new Utilities.UserData().getUserRole(Convert.ToInt64(Session["UserId"]));
                                        var userData = _objHomeA00.getUserRole(Convert.ToInt64(_sessionService.Get<string>("userID")), _A00ExistData.SYKIID);

                                        if (userData.isDeptHead || userData.isCoOrdHead || userData.isDivHead || userData.isExeCoOrdHead || userData.isOperatingHead)
                                        {
                                            ViewBag.actionType = "Approval";
                                            this.generateApprovalHistory(A00DTLTBID ?? 0);
                                        }
                                    }
                                    ////Utilities.UserRole userData = new Utilities.UserData().getUserRole(Convert.ToInt64(_sessionService.Get<string>("userID")));
                                    //var userData = _objHomeA00.getUserRole(Convert.ToInt64(_sessionService.Get<string>("userID")), _A00ExistData.SYKIID);

                                    //if (userData.isDeptHead || userData.isCoOrdHead || userData.isDivHead || userData.isExeCoOrdHead || userData.isOperatingHead)
                                    //{
                                    //    ViewBag.actionType = "Approval";
                                    //    this.generateApprovalHistory(A00DTLTBID ?? 0);
                                    //}
                                    //EA CR7306 Updated Code
                                }
                            }
                            else
                            {
                                ViewBag.actionType = "View";
                                this.generateApprovalHistory(A00DTLTBID ?? 0);
                                objModel.A00DTLTBID = (long)A00DTLTBID;
                                objModel.SYKI = (decimal)ActiveKiID;
                            }
                        }
                        List<SelectListItem> mlist = new List<SelectListItem>();
                        mlist.Add(new SelectListItem { Text = "Select", Value = "", Disabled = true });

                        if (_A00ExistData.BUDGETFLG.ToString() == "1")
                        {
                            mlist.Add(new SelectListItem { Text = "Yes", Value = "10", Selected = true });
                            mlist.Add(new SelectListItem { Text = "No", Value = "11" });
                        }
                        else if (_A00ExistData.BUDGETFLG.ToString() == "0")
                        {
                            mlist.Add(new SelectListItem { Text = "No", Value = "11", Selected = true });
                            mlist.Add(new SelectListItem { Text = "Yes", Value = "10" });
                        }
                        ViewBag.AllocationDetails = _objHomeA00.GetAllocationDetails(A00DTLTBID, ActiveKiID);
                        var DeficiencyDetails = _objHomeA00.GetDeficiencyRaisedDetails(A00DTLTBID, ActiveKiID);
                        ViewBag.DeficiencyDetails = DeficiencyDetails;
                        var A00ItConfirmationID = _objHomeA00.GetA00ItConfirmationDetails(A00DTLTBID, (decimal)ActiveKiID).ITCONFIRMATIONID;
                        ViewBag.ITConfirmationDetails = _objHomeA00.GetA00ItConfirmationHistory(A00ItConfirmationID);
                        ViewBag.ProjectStatusUpdateList = _objHomeA00.GetA00ProjectUpdateStatusList(A00DTLTBID, (decimal)ActiveKiID)._A00ProjectUpdateStatusList.OrderBy(x => x.ProjectStatusUpdateID).ToList();
                        ViewBag.BudgetedValue = mlist;
                        ViewBag.selectedBudgetedVal = _A00ExistData.BUDGETFLG.ToString();
                        var selectedBudgBy = budgByList.Where(x => x.ADORGLEVELID == _A00ExistData.ADORGLEVELID).FirstOrDefault();
                        ViewBag.List = new SelectList(budgByList, "ADORGLEVELID", "LEVELDESCRIP", selectedBudgBy?.ADORGLEVELID);


                        //var ief = _Assetdatabasecontext.A00INVFORCAST.Join(_Assetdatabasecontext.INVFORCAST, a => a.INVFORCASTID, b => b.INVFORCASTID, (a, b) => new { a, b }).Where(x => x.a.A00DTLTBID == A00DTLTBID && x.a.ACTIVE == 1).Select(m => new { INVFORCASTID = m.a.INVFORCASTID, INCFORCASTDETAIL = m.b.INCFORCASTDETAIL }).Distinct().ToList();

                        var ief = A00InvForcast._A00InvforcastList.Join(InvforcastList._InvforcastList,
                            a => a.INVFORCASTID, b => b.INVFORCASTID, (a, b) => new { a, b }).Where(x => x.a.A00DTLTBID == A00DTLTBID && x.a.ACTIVE == 1).
                            Select(m => new { INVFORCASTID = m.a.INVFORCASTID, INCFORCASTDETAIL = m.b.INCFORCASTDETAIL }).Distinct().ToList();

                        if (type == "ACTION")
                        {
                            var selectedSyKI = iList.Where(x => x.SYKIID == _A00ExistData.BUDGETSYKIID).FirstOrDefault();
                            ViewBag.SKY = new SelectList(iList, "SYKIID", "KICODE", selectedSyKI?.SYKIID);
                            datalist.Where(x => ief.Select(y => y.INVFORCASTID.ToString()).ToList().Contains(x.Value)).ToList().ForEach(z => z.Selected = true);
                            if (_A00ExistData.FRCSTOTHER != null)
                            {
                                //var OtherSelected = _Assetdatabasecontext.INVFORCAST.Where(x => x.INCFORCASTDETAIL.ToUpper() == "OTHERS").FirstOrDefault();
                                var OtherSelected = InvforcastList._InvforcastList.Where(x => x.INCFORCASTDETAIL.ToUpper() == "OTHERS").FirstOrDefault();
                                datalist.Where(x => x.Value == OtherSelected.INVFORCASTID.ToString()).ToList().ForEach(x => x.Selected = true);
                                ViewBag.isOtherSelected = "Y";
                            }
                            ViewBag.SKYLIST = datalist;
                            //string filePath = Path.Combine(serverpath.getFileUploadPath(),"A00");
                            string filePath = Path.Combine(serverpath.getFileUploadPath(), "A00");
                            string FileName = Path.GetFileNameWithoutExtension(_A00ExistData.ATTACHMENT);
                            string FileExtension = Path.GetExtension(_A00ExistData.ATTACHMENT);

                            FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;
                        }
                        else
                        {
                            List<SYKI> iList1 = new List<SYKI>();
                            iList1.Add(new SYKI { KICODE = "Select", SYKIID = 0 });
                            foreach (var item in kiData._SYKIList)
                            {
                                iList1.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                            }
                            var selectedSyKI = iList1.Where(x => x.SYKIID == _A00ExistData.BUDGETSYKIID).FirstOrDefault();

                            ViewBag.SKY = new SelectList(iList1, "SYKIID", "KICODE", selectedSyKI?.SYKIID);

                            List<SelectListItem> Vdatalist = new List<SelectListItem>();
                            foreach (var Datat4 in ief)
                            {
                                Vdatalist.Add(new SelectListItem { Text = Datat4.INCFORCASTDETAIL, Value = Datat4.INVFORCASTID.ToString() });
                            }
                            if (_A00ExistData.FRCSTOTHER != null)
                            {
                                //var OtherSelected = _Assetdatabasecontext.INVFORCAST.Where(x => x.INCFORCASTDETAIL.ToUpper() == "OTHERS").FirstOrDefault();
                                var OtherSelected = InvforcastList._InvforcastList.Where(x => x.INCFORCASTDETAIL.ToUpper() == "OTHERS").FirstOrDefault();
                                Vdatalist.Add(new SelectListItem
                                {
                                    Text = OtherSelected.INCFORCASTDETAIL.ToString(),
                                    Value = OtherSelected.INVFORCASTID.ToString(),
                                    Selected = true
                                });
                                ViewBag.isOtherSelected = "Y";
                            }
                            ViewBag.SKYLIST = Vdatalist;
                        }

                    }
                    else
                    {
                        List<SelectListItem> mlist = new List<SelectListItem>();
                        mlist.Add(new SelectListItem { Text = "Select", Value = "", Selected = true });
                        mlist.Add(new SelectListItem { Text = "Yes", Value = "10" });
                        mlist.Add(new SelectListItem { Text = "No", Value = "11" });
                        ViewBag.BudgetedValue = mlist;
                        ViewBag.List = new SelectList(budgByList, "ADORGLEVELID", "LEVELDESCRIP", 0);
                        ViewBag.SKY = new SelectList(iList, "SYKIID", "KICODE", 0);
                    }
                    #endregion

                    return View(objModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ViewA001"); ;
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewA001(A00ViewModel model, IFormCollection fc)
        {
            var redirectAction = "ViewA001";
            try
            {
                var Iscordinator = 0;//CR7306
                var divsionId = "0";//CR7306
                var operationId = "0";//CR7306
                var initiatorId = "0";//CR7306
                //Change start on 22-July-2021
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    //Change end on 22-July-2021
                    if (validateInput(model, fc) == true)
                    {
                        var kiList = _objHomeA00.GetA00SYKIList();
                        var ActiveKiID = (!string.IsNullOrWhiteSpace(fc["hdnSYKIID"]) && fc["hdnSYKIID"] != "0") ? Convert.ToDecimal(fc["hdnSYKIID"].ToString()) : kiList._SYKIList.Where(x => x.ACTIVE == 1).Select(y => y.SYKIID).FirstOrDefault();

                        var A00InvForcast = _objHomeA00.GetA00INVFORCASTList();
                        #region
                        InsertA00DtlTb ALTD = new InsertA00DtlTb();
                        InsertA00APPROVAL approval = new InsertA00APPROVAL();
                        //string UploadPath = Path.Combine(serverpath.getFileUploadPath(),"A00");
                        string UploadPath = Path.Combine(serverpath.getFileUploadPath(), "A00");
                        var statusText = "saved";
                        string FileName = null;
                        if (model.ImageFile != null)
                        {
                            FileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                            string FileExtension = Path.GetExtension(model.ImageFile.FileName);

                            FileName = Convert.ToString(_sessionService.Get<string>("userID")) + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + "_" + DateTime.Now.Second.ToString() + "_" + DateTime.Now.Millisecond.ToString() + FileExtension;
                            model.ImagePath = Path.Combine(UploadPath , FileName);
                            //model.ImageFile.SaveAs(model.ImagePath);
                            string directoryPath = Path.GetDirectoryName(model.ImagePath);
                            if (!Directory.Exists(directoryPath))
                            {
                                Directory.CreateDirectory(directoryPath);
                            }
                            using (var stream = new FileStream(model.ImagePath, FileMode.Create))
                            {
                                model.ImageFile.CopyTo(stream);
                            }
                        }

                        string[] SKYLIST = null;
                        if (!string.IsNullOrEmpty(fc["SKYLIST"]))
                        {
                            SKYLIST = fc["SKYLIST"].ToString().Split(',');
                        }
                        try
                        {
                            #region Region- Get data from A00DTLTB and A00APPROVAL table
                            //09-Sept-2021 change start
                            int currDTLTBReqStatus = -11; //Just initialize we can change value (-11 not used any where)
                            //09-Sept-2021 change end
                            if (model.A00DTLTBID != 0)
                            {
                                //ALTD = _Assetdatabasecontext.A00DTLTB.Where(x => x.A00DTLTBID == model.A00DTLTBID).FirstOrDefault();
                                _objA00SearchModel.a00dtltbid = model.A00DTLTBID;
                                var record = _objHomeA00.GetA00Dtl(_objA00SearchModel);
                                ALTD.A00DTLTBID = record.A00DTLTBID;
                                ALTD.ADDEDBY = record.ADDEDBY;
                                ALTD.DATEADDEDID = record.DATEADDEDID;
                                ALTD.OPERATION = record.OPERATION;
                                ALTD.DIVISION = record.DIVISION;
                                ALTD.DEPARTMENT = record.DEPARTMENT;
                                ALTD.SECTION = record.SECTION;

                                //09-Sept-2021 change start
                                currDTLTBReqStatus = record.STATUSCD;
                                //09-Sept-2021 change end

                                //approval = _Assetdatabasecontext.A00APPROVAL.Where(x => x.A00DTLTBID == model.A00DTLTBID).FirstOrDefault();
                                _objA00SearchModel.a00dtltbid = model.A00DTLTBID;
                                var approvalRecord1 = _objHomeA00.GetA00APPROVAL(_objA00SearchModel);
                                approval.A00APPROVALID = approvalRecord1.A00APPROVALID;
                                approval.A00APPROVALID = approvalRecord1.A00APPROVALID;
                                approval.A00DTLTBID = approvalRecord1.A00DTLTBID;
                                approval.DEPTHDID = approvalRecord1.DEPTHDID;
                                approval.DEPTHDAPPDATE = approvalRecord1.DEPTHDAPPDATE;
                                approval.DEPTHDAPPTXT = approvalRecord1.DEPTHDAPPTXT;
                                approval.COORDDID = approvalRecord1.COORDDID;
                                approval.COORDAPPDATE = approvalRecord1.COORDAPPDATE;
                                approval.COORDAPPTXT = approvalRecord1.COORDAPPTXT;
                                approval.DIVHDHDID = approvalRecord1.DIVHDHDID;
                                approval.DIVHDAPPDATE = approvalRecord1.DIVHDAPPDATE;
                                approval.DIVHDAPPTXT = approvalRecord1.DIVHDAPPTXT;
                                approval.EXECOHDID = approvalRecord1.EXECOHDID;
                                approval.EXECOAPPDATE = approvalRecord1.EXECOAPPDATE;
                                approval.EXECOAPPTXT = approvalRecord1.EXECOAPPTXT;
                                approval.OHID = approvalRecord1.OHID;
                                approval.OHAPPDATE = approvalRecord1.OHAPPDATE;
                                approval.OHAPPTXT = approvalRecord1.OHAPPTXT;

                                //09-Sept-2021 change start
                                approval.PPCHOOHID = approvalRecord1.PPCHOOHID;
                                approval.PPCHOOHDATE = approvalRecord1.PPCHOOHDATE;
                                approval.PPCHOOHTXT = approvalRecord1.PPCHOOHTXT;
                                //09-Sept-2021 change end

                                //sa CR7306
                                if (Convert.ToString(Convert.ToInt32(_sessionService.Get<string>("userID"))) == Convert.ToString(approvalRecord1.COORDDID))
                                {
                                    Iscordinator = 1;
                                    initiatorId = Convert.ToString(record.ADDEDBY);
                                }
                                //ea CR7306
                            }
                            else
                            {
                                //ALTD.A00DTLTBID = _Assetdatabasecontext.A00DTLTB.AsEnumerable().OrderByDescending(x => x.A00DTLTBID).Select(x => x.A00DTLTBID).FirstOrDefault() + 1;
                                ALTD.A00DTLTBID = _objHomeA00.GetA00MaxA00DTLTBID().A00DTLTBID + 1;
                                //approval.A00APPROVALID = _Assetdatabasecontext.A00APPROVAL.AsEnumerable().OrderByDescending(x => x.A00APPROVALID).Select(x => x.A00APPROVALID).FirstOrDefault() + 1;
                                approval.A00APPROVALID = _objHomeA00.GetA00MaxA00APPROVAL().A00APPROVALID + 1;
                            }
                            ALTD.PRJCTTLE = model.Projecttitle;
                            ALTD.PRJCTTXT = model.Backgrounds;
                            ALTD.BUKPITXT = model.BusinessKPI;
                            ALTD.PURPSTXT = model.PurposeA00;
                            ALTD.TRGTINDCD = model.TargetA00;
                            ALTD.RQUMTTXT = model.RequirmentA00;
                            if (model.Budgeted == 10)
                            {
                                ALTD.BUDGETFLG = 1;
                                ALTD.BUDGETSYKIID = Convert.ToDecimal(model.KICODE);
                            }
                            else
                            {
                                ALTD.BUDGETFLG = 0;
                                ALTD.BUDGETSYKIID = null;
                            }

                            ALTD.SYKIID = ActiveKiID;

                            if (Convert.ToInt16(fc["LEVELDESCRIP"]) == 0)
                            {
                                ALTD.ADORGLEVELID = null;
                            }
                            else
                            {
                                ALTD.ADORGLEVELID = Convert.ToInt16(fc["LEVELDESCRIP"].ToString());
                            }

                            ALTD.STARTDT = convertDateToDDMM(fc["start"]);
                            ALTD.ENDDT = convertDateToDDMM(fc["end"]); // date to be stored as dd/mm/yyyy

                            if (SKYLIST.Contains("9"))
                            {
                                ALTD.FRCSTOTHER = model.Ifothers;
                            }
                            else
                            {
                                ALTD.FRCSTOTHER = "";
                            }
                            if (FileName != null)
                            {
                                ALTD.ATTACHMENT = FileName;
                            }
                            #endregion

                            #region Set Statuscd according to button
                            if (!string.IsNullOrEmpty(fc["btnSubmit"]) && fc["btnSubmit"].ToString().ToUpper() == "SUBMIT")
                            {
                                ALTD.STATUSCD = 0; // will be updated after checking role in below section for Submit
                                statusText = "submitted";
                                ALTD.ADDEDBY = Convert.ToInt32(_sessionService.Get<string>("userID"));
                                redirectAction = "search";
                            }
                            else if (!string.IsNullOrEmpty(fc["btnSaveDraft"]) && fc["btnSaveDraft"].ToString().ToUpper() == "SAVE AS DRAFT")
                            {
                                ALTD.STATUSCD = 10;
                                ALTD.ADDEDBY = Convert.ToInt32(_sessionService.Get<string>("userID"));
                                redirectAction = null;
                            }
                            else if (!string.IsNullOrEmpty(fc["btnApprove"]) && fc["btnApprove"].ToString().ToUpper() == "APPROVE")
                            {
                                ALTD.STATUSCD = 1; // will be updated after checking role in below section for Approval
                                statusText = "approved";
                                redirectAction = "search";
                            }
                            else if (!string.IsNullOrEmpty(fc["btnSendBack"]) && fc["btnSendBack"].ToString().ToUpper() == "SEND BACK")
                            {
                                ALTD.STATUSCD = 3;
                                statusText = "sent back";
                                redirectAction = "search";
                            }
                            else if (!string.IsNullOrEmpty(fc["btnReject"]) && fc["btnReject"].ToString().ToUpper() == "REJECT")
                            {
                                ALTD.STATUSCD = 2; // will be updated after checking role in below section for Rejection
                                statusText = "rejected";
                                redirectAction = "search";
                            }
                            #endregion

                            var listsession = Convert.ToInt32(_sessionService.Get<string>("userID"));
                            var userData = _objHomeA00.getUserRole(listsession, ALTD.SYKIID);//CR7306 Updated Code
                            bool isCoOrdHead = false;//CR7306 Updated Code
                            isCoOrdHead = userData.isCoOrdHead; //CR7306 Updated Code

                            _objA00SearchModel.ADEMPCODE = listsession;
                            _objA00SearchModel.SYKI = ActiveKiID;
                            var operation22 = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel);
                            //sa CR7306
                            //if (Iscordinator == 1) //CR7306 Updated Code
                            if (Iscordinator == 1 && isCoOrdHead == false) //CR7306 Updated Code
                            {
                                var vHead = _objHomeA00.GetA00_ADORGLEVELHEAD()._A00_ADORGLEVELHEAD;
                                A00SearchModel obj = new A00SearchModel();
                                obj.ADEMPCODE = long.Parse(initiatorId);
                                var vObj = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(obj);
                                //divsionId = Convert.ToString(vObj.SUPSUPERVISOREMPCODE); //CR7306 Updated Code
                                var _divsionId = "";
                                if (vObj.DIVISIONID != null && vObj.DIVISIONID != 0)
                                {
                                    _divsionId = Convert.ToString(vObj.DIVISIONID); //CR7306 Updated Code
                                    var _divID = vHead.Where(x => x.ADORGLEVELID == long.Parse(_divsionId)).FirstOrDefault()?.ADEMPCODE;
                                    divsionId = _divID.ToString();
                                    operationId = string.Empty;
                                }
                                else
                                {
                                    if (vObj.OPERATIONID != null && vObj.OPERATIONID != 0)
                                    {
                                        var _operationId = Convert.ToString(vObj.OPERATIONID);//CR7306 Updated Code
                                        var _opID = vHead.Where(x => x.ADORGLEVELID == long.Parse(_operationId)).FirstOrDefault()?.ADEMPCODE;
                                        operationId = _opID.ToString();
                                        divsionId = string.Empty;
                                    }
                                }
                            }
                            //ea CR7306
                            if (ALTD.STATUSCD == 0 || ALTD.STATUSCD == 10)
                            {
                                #region Region-A2
                                try
                                {
                                    if (operation22.DEPARTMENTID != null)
                                    {
                                        ALTD.DEPARTMENT = Convert.ToInt16(operation22.DEPARTMENTID);
                                    }
                                    else
                                    {
                                        ALTD.DEPARTMENT = 0;
                                    }

                                    if (operation22.OPERATIONID != null)
                                    {
                                        ALTD.OPERATION = Convert.ToInt16(operation22.OPERATIONID);
                                    }
                                    else
                                    {
                                        ALTD.OPERATION = 0;
                                    }

                                    if (operation22.DIVISIONID != null)
                                    {
                                        ALTD.DIVISION = Convert.ToInt16(operation22.DIVISIONID);
                                    }
                                    else
                                    {
                                        ALTD.DIVISION = 0;
                                    }

                                    if (operation22.SECTIONID != null)
                                    {
                                        ALTD.SECTION = Convert.ToInt32(operation22.SECTIONID);
                                    }
                                    else
                                    {
                                        ALTD.SECTION = 0;
                                    }
                                }

                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, " ViewA001");
                                }
                                #endregion
                            }
                            ALTD.ACTIVE = 1;
                            var currentTime = DateTime.Now;
                            if (model.A00DTLTBID == 0)
                            {
                                ALTD.DATEADDEDID = currentTime;
                            }

                            ALTD.LASTMODDATE = currentTime;
                            ALTD.LSTMODBYID = Convert.ToInt32(_sessionService.Get<string>("userID"));

                            #region Submit - approval is null
                            if (statusText.Trim().ToUpper() == "SUBMITTED")
                            {
                                approval.DEPTHDID = null;
                                approval.DEPTHDAPPTXT = null;
                                approval.DEPTHDAPPDATE = null;

                                approval.COORDDID = null;
                                approval.COORDAPPTXT = null;
                                approval.COORDAPPDATE = null;

                                approval.DIVHDHDID = null;
                                approval.DIVHDAPPTXT = null;
                                approval.DIVHDAPPDATE = null;

                                approval.EXECOHDID = null;
                                approval.EXECOAPPTXT = null;
                                approval.EXECOAPPDATE = null;

                                approval.OHID = null;
                                approval.OHAPPTXT = null;
                                approval.OHAPPDATE = null;

                                //09-Sept-2021 change start
                                approval.PPCHOOHID = null;
                                approval.PPCHOOHTXT = null;
                                approval.PPCHOOHDATE = null;
                                //09-Sept-2021 change end
                            }
                            #endregion

                            //bool isDeptHead = false, isCoOrdHead = false, isDivHead = false, isExeCoOrdHead = false, isOperatingHead = false; //CR7306 Updated Code
                            bool isDeptHead = false,  isDivHead = false, isExeCoOrdHead = false, isOperatingHead = false;
                            approval.A00DTLTBID = ALTD.A00DTLTBID;
                            //Utilities.UserRole userData = new Utilities.UserData().getUserRole(listsession);
                            //var userData = _objHomeA00.getUserRole(listsession, ALTD.SYKIID); //CR7306 Updated Code

                            //09-Sept-2021 change start
                            long? PPCHOOH_ID = _objHomeA00.getA00GetHOOH(ALTD.OPERATION, ALTD.DIVISION);
                            bool isPPCHOOperatingHead = PPCHOOH_ID == null ? false : (PPCHOOH_ID != ALTD.LSTMODBYID ? false : true);
                            //var userSentBackStatus = (short)(userData.isDeptHead ? 35 : (userData.isCoOrdHead ? 55 : (userData.isDivHead ? 75 : (userData.isExeCoOrdHead ? 95 : (userData.isOperatingHead ? 115 : 10)))));
                            var userSentBackStatus = (short)(userData.isDeptHead ? 35 : (userData.isCoOrdHead ? 55 : (userData.isDivHead ? 75 : (userData.isExeCoOrdHead ? 95 : (userData.isOperatingHead && currDTLTBReqStatus != 101 ? 115 : (isPPCHOOperatingHead ? 116 : 10))))));
                            //09-Sept-2021 change end

                            isDeptHead = userData.isDeptHead;
                            //isCoOrdHead = userData.isCoOrdHead; //CR7306 Updated Code
                            isDivHead = userData.isDivHead;
                            isExeCoOrdHead = userData.isExeCoOrdHead;
                            isOperatingHead = userData.isOperatingHead;

                            var nextApproverToEmail = ""; var rejectRequesterEmail = ""; var sendBackRequesterEmail = ""; var AOORequestApprovedToEmail = "";
                            #region Update Roles in Approval Table
                            var _adOrgLevelHead = _objHomeA00.GetA00_ADORGLEVELHEAD()._A00_ADORGLEVELHEAD;

                            //sa CR7306
                            //if (Iscordinator == 1) //CR7306 Updated Code
                            if (Iscordinator == 1 && isCoOrdHead == false) //CR7306 Updated Code
                            {
                                //sa CR7306 Updated Code

                                if (ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 || ALTD.STATUSCD == 10) // submit, approve or save
                                {


                                    if (!string.IsNullOrEmpty(divsionId))
                                    {
                                        approval.DIVHDHDID = long.Parse(divsionId);// (long)divsionId;
                                        _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                        nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                        ALTD.STATUSCD = (short)(ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 ? 60 : 10); //CR7306 Updated Code
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(operationId))
                                        {
                                            approval.OHID = long.Parse(operationId);// (long)divsionId;
                                            _objA00SearchModel.OHID = approval.OHID;
                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.OHID).EMAILID;
                                            ALTD.STATUSCD = (short)(ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 ? 100 : 10);
                                        }
                                    }
                                }
                                else if (ALTD.STATUSCD == 2) //dept head reject
                                {
                                    rejectRequesterEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(ALTD.ADDEDBY).EMAILID;
                                    ALTD.STATUSCD = 30;
                                }
                                else if (ALTD.STATUSCD == 3) //dept head send back
                                {
                                    sendBackRequesterEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(ALTD.ADDEDBY).EMAILID;
                                    ALTD.STATUSCD = userSentBackStatus;//
                                }
                                else if (ALTD.STATUSCD == 4) //OH send back
                                {
                                    ALTD.STATUSCD = userSentBackStatus;//115;
                                }

                                //approval.DIVHDHDID = long.Parse(divsionId);// (long)divsionId;
                                //_objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                //nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                ////ALTD.STATUSCD = (short)(ALTD.STATUSCD == 0 ? 60 : 10); //CR7306 Updated Code
                                //ALTD.STATUSCD = (short)(ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 ? 60 : 10); //CR7306 Updated Code
                                //ea CR7306 Updated Code
                            }
                            else
                            {
                            //09-Sept-2021 change start
                            if (!isPPCHOOperatingHead) //current user role check
                            {
                                //09-Sept-2021 change end
                                if (!isOperatingHead) // current user role check
                                {
                                    if (!isExeCoOrdHead) // current user role check
                                    {
                                        //var opHeadValue = _AssetDBContext.ADORGLEVELHEAD.Where(x =>
                                        //                     x.ADORGLEVELID == operation22.OPERATIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                        var opHeadValue = _adOrgLevelHead.Where(x =>
                                                             x.ADORGLEVELID == operation22.OPERATIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                        if (!isDivHead) // current user role check
                                        {
                                            //var divHeadValue = _AssetDBContext.ADORGLEVELHEAD.Where(x =>
                                            //        x.ADORGLEVELID == operation22.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                            var divHeadValue = _adOrgLevelHead.Where(x =>
                                                    x.ADORGLEVELID == operation22.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                            if (!isCoOrdHead) // current user role check
                                            {
                                                if (!isDeptHead) // current user role check
                                                {
                                                    #region Requester
                                                    if (!string.IsNullOrEmpty(fc["btnSubmit"]) && fc["btnSubmit"].ToString().ToUpper() == "SUBMIT")
                                                    {
                                                        //approval.DEPTHDID = _AssetDBContext.ADORGLEVELHEAD.Where(x => x.ADORGLEVELID == operation22.DEPARTMENTID && x.ISACTIVE == 1).Select(x => x.ADEMPCODE).FirstOrDefault(); // Dept Head as Approver //Change by Satyaveer on 11-Oct-2020
                                                        var deptHeadValue = _adOrgLevelHead.Where(x => x.ADORGLEVELID == operation22.DEPARTMENTID && x.ISACTIVE == 1).Select(x => x.ADEMPCODE).FirstOrDefault(); // Dept Head as Approver //Change by Satyaveer on 11-Oct-2020
                                                        if (deptHeadValue == null || deptHeadValue == 0 || deptHeadValue == divHeadValue) //// -- ADDED BY VISHAL ON 07-Jul-23
                                                        {
                                                            _objA00SearchModel.OPERATIONID = operation22.OPERATIONID;
                                                            approval.COORDDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).COORDINATOR;
                                                            if (approval.COORDDID == null || approval.COORDDID == 0)
                                                            {
                                                                approval.DIVHDHDID = divHeadValue;
                                                                _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                                                nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                                                ALTD.STATUSCD = (short)(ALTD.STATUSCD == 0 ? 60 : 10);
                                                            }
                                                            else
                                                            {
                                                                _objA00SearchModel.COORDDID = approval.COORDDID;
                                                                nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.COORDDID).EMAILID;
                                                                ALTD.STATUSCD = (short)(ALTD.STATUSCD == 0 ? 40 : 10);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            approval.DEPTHDID = deptHeadValue;
                                                            _objA00SearchModel.DEPTHDID = approval.DEPTHDID;
                                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DEPTHDID).EMAILID;
                                                            ALTD.STATUSCD = (short)(ALTD.STATUSCD == 0 ? 20 : 10);
                                                        }
                                                    }
                                                    #endregion
                                                }
                                                else
                                                {
                                                    #region Department Head
                                                    if (ALTD.STATUSCD != 0 && ALTD.STATUSCD != 10) // approval table update for not save or submit
                                                    {
                                                        approval.DEPTHDID = listsession;
                                                        approval.DEPTHDAPPDATE = currentTime;
                                                        if (!string.IsNullOrEmpty(fc["btnApprove"]) && fc["btnApprove"].ToString().ToUpper() == "APPROVE")
                                                        {
                                                            approval.DEPTHDAPPTXT = model.Remarks;
                                                        }
                                                        else if (!string.IsNullOrEmpty(fc["btnSendBack"]) && fc["btnSendBack"].ToString().ToUpper() == "SEND BACK")
                                                        {
                                                            approval.DEPTHDAPPTXT = "SEND BACK." + model.Remarks;
                                                        }
                                                        else if (!string.IsNullOrEmpty(fc["btnReject"]) && fc["btnReject"].ToString().ToUpper() == "REJECT")
                                                        {
                                                            approval.DEPTHDAPPTXT = "REJECTED." + model.Remarks;
                                                        }

                                                    }
                                                    if (ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 || ALTD.STATUSCD == 10) // submit, approve or save
                                                    {
                                                        //approval.COORDDID = _AssetDBContext.ADORGCOORDINATOR.Where(x =>
                                                        //x.ADORGLEVELID == operation22.OPERATIONID).Select(x => x.COORDINATOR).FirstOrDefault(); // Co-ordinator as Approver
                                                        _objA00SearchModel.OPERATIONID = operation22.OPERATIONID;
                                                        approval.COORDDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).COORDINATOR; // Co-ordinator as Approver

                                                        //nextApproverToEmail = _AssetDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == approval.COORDDID).Select(y => y.EMAILID).FirstOrDefault();

                                                        ALTD.STATUSCD = (short)(ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 ? 40 : 10);
                                                        if (approval.COORDDID == null)
                                                        {
                                                            approval.DIVHDHDID = divHeadValue;
                                                            //nextApproverToEmail = _AssetDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == approval.DIVHDHDID).Select(y => y.EMAILID).FirstOrDefault();
                                                            _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                                            ALTD.STATUSCD = ALTD.STATUSCD == 40 ? (short)60 : ALTD.STATUSCD;
                                                        }
                                                        else
                                                        {
                                                            _objA00SearchModel.COORDDID = approval.COORDDID;
                                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.COORDDID).EMAILID;
                                                        }
                                                    }
                                                    else if (ALTD.STATUSCD == 2) //dept head reject
                                                    {
                                                        rejectRequesterEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(ALTD.ADDEDBY).EMAILID;
                                                        ALTD.STATUSCD = 30;
                                                    }
                                                    else if (ALTD.STATUSCD == 3) //dept head send back
                                                    {
                                                        sendBackRequesterEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(ALTD.ADDEDBY).EMAILID;
                                                        ALTD.STATUSCD = userSentBackStatus;//
                                                    }
                                                    else if (ALTD.STATUSCD == 4) //OH send back
                                                    {
                                                        ALTD.STATUSCD = userSentBackStatus;//115;
                                                    }
                                                    #endregion
                                                }
                                            }
                                            else
                                            {
                                                #region Co-ordinator
                                                if (ALTD.STATUSCD != 0 && ALTD.STATUSCD != 10)
                                                {
                                                    approval.COORDAPPDATE = currentTime;
                                                    approval.COORDDID = listsession;
                                                    if (!string.IsNullOrEmpty(fc["btnApprove"]) && fc["btnApprove"].ToString().ToUpper() == "APPROVE")
                                                    {
                                                        approval.COORDAPPTXT = model.Remarks;
                                                    }
                                                    else if (!string.IsNullOrEmpty(fc["btnSendBack"]) && fc["btnSendBack"].ToString().ToUpper() == "SEND BACK")
                                                    {
                                                        approval.COORDAPPTXT = "SEND BACK." + model.Remarks;
                                                    }
                                                    else if (!string.IsNullOrEmpty(fc["btnReject"]) && fc["btnReject"].ToString().ToUpper() == "REJECT")
                                                    {
                                                        approval.COORDAPPTXT = "REJECTED." + model.Remarks;
                                                    }
                                                }
                                                if (ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 || ALTD.STATUSCD == 10)
                                                {
                                                    divHeadValue = _adOrgLevelHead.Where(x => x.ADORGLEVELID == ALTD.DIVISION).Select(x => x.ADEMPCODE).FirstOrDefault();
                                                    approval.DIVHDHDID = divHeadValue; // Div Head as Approver

                                                    //nextApproverToEmail = _AssetDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == approval.DIVHDHDID).Select(y => y.EMAILID).FirstOrDefault();
                                                    // Committed by CR-6843 
                                                    //_objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                                    //nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;

                                                    //ALTD.STATUSCD = (short)(ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 ? 60 : 10);
                                                    // Added by CR-6843 
                                                    if (divHeadValue != 0)
                                                    {
                                                        _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                                        nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;

                                                        ALTD.STATUSCD = (short)(ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 ? 60 : 10);
                                                    }
                                                    else {
                                                        _objA00SearchModel.OPERATIONID = operation22.OPERATIONID;
                                                        approval.EXECOHDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).EXECOORDINATOR; // Co-ordinator as Approver

                                                        ALTD.STATUSCD = (short)(ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 ? 80 : 10);
                                                        if (approval.EXECOHDID == null)
                                                        {                                                          

                                                            if (_objHomeA00.getDataFromAdorgcoordinator(ALTD.DIVISION).OPHEAD != null) //First check operating head against division
                                                            {
                                                                approval.OHID = _objHomeA00.getDataFromAdorgcoordinator(ALTD.DIVISION).OPHEAD;
                                                            }
                                                            else if (_objHomeA00.getDataFromAdorgcoordinator(ALTD.OPERATION).OPHEAD != null) //Here checking operating head against operation
                                                            {
                                                                approval.OHID = _objHomeA00.getDataFromAdorgcoordinator(ALTD.OPERATION).OPHEAD;
                                                            }
                                                            else
                                                            {
                                                                approval.OHID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == ALTD.OPERATION).Select(x => x.ADEMPCODE).FirstOrDefault();
                                                            }
                                                            _objA00SearchModel.OHID = approval.OHID;
                                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.OHID).EMAILID;

                                                            ALTD.STATUSCD = ALTD.STATUSCD == 80 ? (short)100 : ALTD.STATUSCD;
                                                        }
                                                        else
                                                        {
                                                            _objA00SearchModel.EXECOHDID = approval.EXECOHDID;
                                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.EXECOHDID).EMAILID;
                                                        }
                                                    }

                                                }
                                                // Ended by CR-6843 
                                                else if (ALTD.STATUSCD == 2) //co ord reject
                                                {
                                                    rejectRequesterEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(ALTD.ADDEDBY).EMAILID;
                                                    ALTD.STATUSCD = 50;
                                                }
                                                else if (ALTD.STATUSCD == 3) //co ord send back
                                                {
                                                    sendBackRequesterEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(ALTD.ADDEDBY).EMAILID;
                                                    ALTD.STATUSCD = userSentBackStatus; //55;
                                                }
                                                else if (ALTD.STATUSCD == 4) //OH send back
                                                {
                                                    ALTD.STATUSCD = userSentBackStatus;//115;
                                                }
                                                #endregion
                                            }
                                        }
                                        else
                                        {
                                            #region Div-Head
                                            if (ALTD.STATUSCD != 0 && ALTD.STATUSCD != 10)
                                            {
                                                approval.DIVHDAPPDATE = currentTime;
                                                approval.DIVHDHDID = listsession;
                                                if (!string.IsNullOrEmpty(fc["btnApprove"]) && fc["btnApprove"].ToString().ToUpper() == "APPROVE")
                                                {
                                                    approval.DIVHDAPPTXT = model.Remarks;
                                                }
                                                else if (!string.IsNullOrEmpty(fc["btnSendBack"]) && fc["btnSendBack"].ToString().ToUpper() == "SEND BACK")
                                                {
                                                    approval.DIVHDAPPTXT = "SEND BACK." + model.Remarks;
                                                }
                                                else if (!string.IsNullOrEmpty(fc["btnReject"]) && fc["btnReject"].ToString().ToUpper() == "REJECT")
                                                {
                                                    approval.DIVHDAPPTXT = "REJECTED." + model.Remarks;
                                                }
                                            }
                                            if (ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 || ALTD.STATUSCD == 10) // div head approval
                                            {
                                                //approval.EXECOHDID = _AssetDBContext.ADORGCOORDINATOR.Where(x =>
                                                //        x.ADORGLEVELID == operation22.OPERATIONID).Select(x => x.EXECOORDINATOR).FirstOrDefault();
                                                _objA00SearchModel.OPERATIONID = operation22.OPERATIONID;
                                                approval.EXECOHDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).EXECOORDINATOR; // Co-ordinator as Approver


                                                //nextApproverToEmail = _AssetDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == approval.EXECOHDID).Select(y => y.EMAILID).FirstOrDefault();

                                                ALTD.STATUSCD = (short)(ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 ? 80 : 10);
                                                if (approval.EXECOHDID == null)
                                                {
                                                    //07-Sept-2021 change start
                                                    //approval.OHID = opHeadValue;

                                                    //09-Sept-2021 change start
                                                    //if (_objHomeA00.getDataFromAdorgcoordinator(ALTD.OPERATION) != null)
                                                    //{
                                                    //    approval.OHID = _objHomeA00.getDataFromAdorgcoordinator(ALTD.OPERATION).OPHEAD;
                                                    //}
                                                    //else
                                                    //{
                                                    //    approval.OHID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == ALTD.OPERATION).Select(x => x.ADEMPCODE).FirstOrDefault();
                                                    //}

                                                    if (_objHomeA00.getDataFromAdorgcoordinator(ALTD.DIVISION).OPHEAD != null) //First check operating head against division
                                                    {
                                                        approval.OHID = _objHomeA00.getDataFromAdorgcoordinator(ALTD.DIVISION).OPHEAD;
                                                    }
                                                    else if (_objHomeA00.getDataFromAdorgcoordinator(ALTD.OPERATION).OPHEAD != null) //Here checking operating head against operation
                                                    {
                                                        approval.OHID = _objHomeA00.getDataFromAdorgcoordinator(ALTD.OPERATION).OPHEAD;
                                                    }
                                                    else
                                                    {
                                                        approval.OHID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == ALTD.OPERATION).Select(x => x.ADEMPCODE).FirstOrDefault();
                                                    }
                                                    //09-Sept-2021 change end
                                                    //07-Sept-2021 change end

                                                    //nextApproverToEmail = _AssetDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == approval.OHID).Select(y => y.EMAILID).FirstOrDefault();
                                                    _objA00SearchModel.OHID = approval.OHID;
                                                    nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.OHID).EMAILID;

                                                    ALTD.STATUSCD = ALTD.STATUSCD == 80 ? (short)100 : ALTD.STATUSCD;
                                                }
                                                else
                                                {
                                                    _objA00SearchModel.EXECOHDID = approval.EXECOHDID;
                                                    nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.EXECOHDID).EMAILID;
                                                }
                                            }
                                            else if (ALTD.STATUSCD == 2) //div head reject
                                            {
                                                rejectRequesterEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(ALTD.ADDEDBY).EMAILID;
                                                ALTD.STATUSCD = 70;
                                            }
                                            else if (ALTD.STATUSCD == 3) //div head send back
                                            {
                                                sendBackRequesterEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(ALTD.ADDEDBY).EMAILID;
                                                ALTD.STATUSCD = userSentBackStatus; //75;
                                            }
                                            else if (ALTD.STATUSCD == 4) //OH send back
                                            {
                                                ALTD.STATUSCD = userSentBackStatus;//115;
                                            }
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region Exe-Coordinator
                                        if (ALTD.STATUSCD != 0 && ALTD.STATUSCD != 10)
                                        {
                                            approval.EXECOAPPDATE = currentTime;
                                            approval.EXECOHDID = listsession;
                                            if (!string.IsNullOrEmpty(fc["btnApprove"]) && fc["btnApprove"].ToString().ToUpper() == "APPROVE")
                                            {
                                                approval.EXECOAPPTXT = model.Remarks;
                                            }
                                            else if (!string.IsNullOrEmpty(fc["btnSendBack"]) && fc["btnSendBack"].ToString().ToUpper() == "SEND BACK")
                                            {
                                                approval.EXECOAPPTXT = "SEND BACK." + model.Remarks;
                                            }
                                            else if (!string.IsNullOrEmpty(fc["btnReject"]) && fc["btnReject"].ToString().ToUpper() == "REJECT")
                                            {
                                                approval.EXECOAPPTXT = "REJECTED." + model.Remarks;
                                            }
                                        }
                                        if (ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 || ALTD.STATUSCD == 10) // ex co ord approval
                                        {
                                            //approval.OHID = _AssetDBContext.ADORGLEVELHEAD.Where(x =>
                                            //                 x.ADORGLEVELID == operation22.OPERATIONID).Select(x => x.ADEMPCODE).FirstOrDefault();

                                            //07-Sept-2021 change start
                                            //approval.OHID = _adOrgLevelHead.Where(x =>
                                            //                 x.ADORGLEVELID == operation22.OPERATIONID).Select(x => x.ADEMPCODE).FirstOrDefault();

                                            //09-Sept-2021 change start
                                            //if (_objHomeA00.getDataFromAdorgcoordinator(ALTD.OPERATION) != null)
                                            //{
                                            //    approval.OHID = _objHomeA00.getDataFromAdorgcoordinator(ALTD.OPERATION).OPHEAD;
                                            //}
                                            //else
                                            //{
                                            //    approval.OHID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == ALTD.OPERATION).Select(x => x.ADEMPCODE).FirstOrDefault();
                                            //}

                                            if (_objHomeA00.getDataFromAdorgcoordinator(ALTD.DIVISION).OPHEAD != null) //First check operating head against division
                                            {
                                                approval.OHID = _objHomeA00.getDataFromAdorgcoordinator(ALTD.DIVISION).OPHEAD;
                                            }
                                            else if (_objHomeA00.getDataFromAdorgcoordinator(ALTD.OPERATION).OPHEAD != null) //Here checking operating head against operation
                                            {
                                                approval.OHID = _objHomeA00.getDataFromAdorgcoordinator(ALTD.OPERATION).OPHEAD;
                                            }
                                            else
                                            {
                                                approval.OHID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == ALTD.OPERATION).Select(x => x.ADEMPCODE).FirstOrDefault();
                                            }
                                            //09-Sept-2021 change end

                                            //07-Sept-2021 change end

                                            //nextApproverToEmail = _AssetDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == approval.OHID).Select(y => y.EMAILID).FirstOrDefault();
                                            _objA00SearchModel.OHID = approval.OHID;
                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.OHID).EMAILID;

                                            ALTD.STATUSCD = (short)(ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 ? 100 : 10);
                                        }
                                        else if (ALTD.STATUSCD == 2) //ex co ord reject
                                        {
                                            rejectRequesterEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(ALTD.ADDEDBY).EMAILID;
                                            ALTD.STATUSCD = 90;
                                        }
                                        else if (ALTD.STATUSCD == 3) //ex co ord send back
                                        {
                                            sendBackRequesterEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(ALTD.ADDEDBY).EMAILID;
                                            ALTD.STATUSCD = userSentBackStatus;//95;
                                        }
                                        else if (ALTD.STATUSCD == 4) //OH send back
                                        {
                                            ALTD.STATUSCD = userSentBackStatus;//115;
                                        }
                                        #endregion
                                    }
                                }
                                else
                                {
                                    #region OH
                                    if (ALTD.STATUSCD != 0 && ALTD.STATUSCD != 10)
                                    {
                                        approval.OHAPPDATE = currentTime;
                                        approval.OHID = listsession;
                                        if (!string.IsNullOrEmpty(fc["btnApprove"]) && fc["btnApprove"].ToString().ToUpper() == "APPROVE")
                                        {
                                            approval.OHAPPTXT = model.Remarks;
                                        }

                                        else if (!string.IsNullOrEmpty(fc["btnSendBack"]) && fc["btnSendBack"].ToString().ToUpper() == "SEND BACK")
                                        {
                                            approval.OHAPPTXT = "SEND BACK." + model.Remarks;
                                        }
                                        else if (!string.IsNullOrEmpty(fc["btnReject"]) && fc["btnReject"].ToString().ToUpper() == "REJECT")
                                        {
                                            approval.OHAPPTXT = "REJECTED." + model.Remarks;
                                        }
                                    }
                                    if (ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 || ALTD.STATUSCD == 10) // Op Head approval
                                    {
                                        //09-Sept-2021 change start
                                        //AOORequestApprovedToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(ALTD.ADDEDBY).EMAILID;
                                        //if (ALTD.STATUSCD == 0)
                                        //{
                                        //    approval.OHID = ALTD.ADDEDBY;
                                        //    approval.OHAPPTXT = "Auto Approved";
                                        //    approval.OHAPPDATE = DateTime.Now;
                                        //}

                                        //ALTD.STATUSCD = (short)(ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 ? 105 : 10);

                                        if (PPCHOOH_ID != null)
                                        {
                                            //Set STATUSCD=101 and request send to PPC-HO-Operating head
                                            ALTD.STATUSCD = (short)(ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 ? 101 : 10);
                                            approval.PPCHOOHID = PPCHOOH_ID;  //Assgin PPCHOOH-ID
                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(PPCHOOH_ID).EMAILID;
                                        }
                                        else
                                        {
                                            //PPCHOOH not exist then request finally approved by OH
                                            //Here STATUSCD-0 (Submit by button pressed), STATUSCD-1 (Approve button pressed)
                                            AOORequestApprovedToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(ALTD.ADDEDBY).EMAILID;
                                            if (ALTD.STATUSCD == 0)
                                            {
                                                approval.OHID = ALTD.ADDEDBY;
                                                approval.OHAPPTXT = "Auto Approved";
                                                approval.OHAPPDATE = DateTime.Now;
                                            }

                                            ALTD.STATUSCD = (short)(ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 ? 105 : 10);
                                        }
                                        //09-Sept-2021 change end
                                    }
                                    else if (ALTD.STATUSCD == 2) //OH reject
                                    {
                                        rejectRequesterEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(ALTD.ADDEDBY).EMAILID;
                                        ALTD.STATUSCD = 110;
                                    }
                                    else if (ALTD.STATUSCD == 3) //OH send back
                                    {
                                        sendBackRequesterEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(ALTD.ADDEDBY).EMAILID;
                                        ALTD.STATUSCD = userSentBackStatus;//115;
                                    }
                                    else if (ALTD.STATUSCD == 4) //OH send back
                                    {
                                        ALTD.STATUSCD = userSentBackStatus;//115;
                                    }
                                    #endregion
                                }
                                //09-Sept-2021 change start
                            }
                            else
                            {
                                #region PPC-HO-OH
                                if (ALTD.STATUSCD != 0 && ALTD.STATUSCD != 10)
                                {
                                    approval.PPCHOOHDATE = currentTime;
                                    approval.PPCHOOHID = listsession;
                                    if (!string.IsNullOrEmpty(fc["btnApprove"]) && fc["btnApprove"].ToString().ToUpper() == "APPROVE")
                                    {
                                        approval.PPCHOOHTXT = model.Remarks;
                                    }
                                    else if (!string.IsNullOrEmpty(fc["btnSendBack"]) && fc["btnSendBack"].ToString().ToUpper() == "SEND BACK")
                                    {
                                        approval.PPCHOOHTXT = "SEND BACK." + model.Remarks;
                                    }
                                    else if (!string.IsNullOrEmpty(fc["btnReject"]) && fc["btnReject"].ToString().ToUpper() == "REJECT")
                                    {
                                        approval.PPCHOOHTXT = "REJECTED." + model.Remarks;
                                    }
                                }

                                if (ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 || ALTD.STATUSCD == 10) // PPC HO-Op Head approval
                                {
                                    AOORequestApprovedToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(ALTD.ADDEDBY).EMAILID;
                                    if (ALTD.STATUSCD == 0)
                                    {
                                        approval.PPCHOOHID = ALTD.ADDEDBY;
                                        approval.PPCHOOHTXT = "Auto Approved";
                                        approval.PPCHOOHDATE = DateTime.Now;
                                    }
                                    ALTD.STATUSCD = (short)(ALTD.STATUSCD == 0 || ALTD.STATUSCD == 1 ? 105 : 10);
                                }
                                else if (ALTD.STATUSCD == 2) //PPC HO-OH reject
                                {
                                    rejectRequesterEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(ALTD.ADDEDBY).EMAILID;
                                    ALTD.STATUSCD = 111;
                                }
                                else if (ALTD.STATUSCD == 3) //PPC HO-OH send back
                                {
                                    sendBackRequesterEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(ALTD.ADDEDBY).EMAILID;
                                    ALTD.STATUSCD = userSentBackStatus;//116;
                                }
                                else if (ALTD.STATUSCD == 4) //PPC HO-OH send back
                                {
                                    ALTD.STATUSCD = userSentBackStatus;//116;
                                }
                                #endregion
                            }
                                //09-Sept-2021 change end
                            }
                            //ea CR7306
                            #endregion

                            #region Send mail and update data - Save as Draft
                            //Remove all approval history while requister press Save As Draft button by satyaveer on 12-Oct-2020  -  Start
                            if (statusText.Trim().ToUpper() == "SAVED")
                            {
                                approval.DEPTHDID = null;
                                approval.DEPTHDAPPTXT = null;
                                approval.DEPTHDAPPDATE = null;

                                approval.COORDDID = null;
                                approval.COORDAPPTXT = null;
                                approval.COORDAPPDATE = null;

                                approval.DIVHDHDID = null;
                                approval.DIVHDAPPTXT = null;
                                approval.DIVHDAPPDATE = null;

                                approval.EXECOHDID = null;
                                approval.EXECOAPPTXT = null;
                                approval.EXECOAPPDATE = null;

                                approval.OHID = null;
                                approval.OHAPPTXT = null;
                                approval.OHAPPDATE = null;

                                //09-Sept-2021 change start
                                approval.PPCHOOHID = null;
                                approval.PPCHOOHTXT = null;
                                approval.PPCHOOHDATE = null;
                                //09-Sept-2021 change end
                            }
                            //Remove all approval history while requister press Save As Draft button by satyaveer on 12-Oct-2020  -  End

                            if (model.A00DTLTBID == 0)
                            {
                                //    _Assetdatabasecontext.A00DTLTB.Add(ALTD);
                                model.A00DTLTBID = ALTD.A00DTLTBID;
                                //    _Assetdatabasecontext.SaveChanges();

                                //    _Assetdatabasecontext.A00APPROVAL.Add(approval);
                            }
                            //_Assetdatabasecontext.SaveChanges();
                            _ePortalDBContext.Database.BeginTransaction();

                            //sa CR7306
                            //if (Iscordinator == 1) //CR7306 Updated Code
                            if (Iscordinator == 1 && isCoOrdHead == false)  //CR7306 Updated Code
                            {
                                approval.COORDAPPDATE = currentTime;
                                approval.COORDAPPTXT = model.Remarks;
                                _objHomeA00.InsertUpdateA00DetailCordinator(ALTD);
                                _objHomeA00.InsertUpdateA00APPROVALCordinator(approval);
                            }
                            else
                            {
                                _objHomeA00.InsertUpdateA00Detail(ALTD);
                                _objHomeA00.InsertUpdateA00APPROVAL(approval);
                            }

                            //_objHomeA00.InsertUpdateA00Detail(ALTD);
                            //_objHomeA00.InsertUpdateA00APPROVAL(approval);


                            //var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(ALTD.ADDEDBY);
                            var requesterDetails = new Single_A00ADEMPLOYEE();
                            //if (Iscordinator == 1)//CR7306 Updated Code
                            if (Iscordinator == 1 && isCoOrdHead == false)  //CR7306 Updated Code
                            {
                                var id = long.Parse(initiatorId);
                                requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(id);
                            }
                            else
                            {
                                requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(ALTD.ADDEDBY);
                            }
                            //ea CR7306
                            var A00RaisedDetails = _objHomeA00.A00RaisedDetails(model.A00DTLTBID, ActiveKiID);

                            if (!string.IsNullOrEmpty(nextApproverToEmail))
                            {
                                sendMailToApprover(nextApproverToEmail, ALTD.PRJCTTLE, ALTD.DATEADDEDID.Date, requesterDetails.Name, requesterDetails.EmpCode);
                            }
                            else if (!string.IsNullOrEmpty(rejectRequesterEmail))
                            {
                                sendMailToRequesterForReject(rejectRequesterEmail, ALTD.PRJCTTLE, ALTD.DATEADDEDID.Date, requesterDetails.Name, requesterDetails.EmpCode);
                            }
                            else if (!string.IsNullOrEmpty(sendBackRequesterEmail))
                            {
                                sendMailToRequesterForSendBack(sendBackRequesterEmail, ALTD.PRJCTTLE, ALTD.DATEADDEDID.Date, requesterDetails.Name, requesterDetails.EmpCode);
                            }
                            else if (!string.IsNullOrEmpty(AOORequestApprovedToEmail))
                            {
                                sendMailAOORequestApproved(AOORequestApprovedToEmail, ALTD.PRJCTTLE, ALTD.DATEADDEDID.Date, requesterDetails.Name, requesterDetails.EmpCode);
                            }

                            //09-Sept-2021 change start
                            //if (isOperatingHead == true && ALTD.STATUSCD == 105)
                            //{
                            //    sendMailAOORequestApprovedToTeam("AP_HMSI_UG_New_Project_SIS@honda.hmsi.in", ALTD.PRJCTTLE, ALTD.DATEADDEDID.Date, A00RaisedDetails.Name, A00RaisedDetails.EmpCode);
                            //}

                            if (ALTD.STATUSCD == 105)
                            {
                                sendMailAOORequestApprovedToTeam("AP_HMSI_UG_New_Project_SIS@honda.hmsi.in", ALTD.PRJCTTLE, ALTD.DATEADDEDID.Date, A00RaisedDetails.Name, A00RaisedDetails.EmpCode);
                            }
                            //09-Sept-2021 change start

                            List<long> invForecastList = SKYLIST.Where(x => x != "9").Select(y => Convert.ToInt64(y)).ToList();
                            //var allPrevRecIfExists = _Assetdatabasecontext.A00INVFORCAST.Where(x => x.A00DTLTBID == model.A00DTLTBID && x.ACTIVE == 1).
                            //        Select(y => y.INVFORCASTID).ToList();
                            var allPrevRecIfExists = A00InvForcast._A00InvforcastList.Where(x => x.A00DTLTBID == model.A00DTLTBID && x.ACTIVE == 1).Select(y => y.INVFORCASTID).ToList();
                            allPrevRecIfExists = allPrevRecIfExists.Count > 0 ? allPrevRecIfExists : new List<long>();
                            //insert data into a00invforcast
                            if (invForecastList.Count > 0)
                            {
                                var newSelected = invForecastList.Where(x => !allPrevRecIfExists.Contains(x)).ToList();
                                var duplicates = invForecastList.Where(x => allPrevRecIfExists.Contains(x)).ToList();
                                //var inactiveRecords = _Assetdatabasecontext.A00INVFORCAST.Where(x => !duplicates.Contains(x.INVFORCASTID) && !newSelected.Contains(x.INVFORCASTID) && x.A00DTLTBID == model.A00DTLTBID && x.ACTIVE == 1).ToList();
                                var inactiveRecords = A00InvForcast._A00InvforcastList.Where(x => !duplicates.Contains(x.INVFORCASTID) && !newSelected.Contains(x.INVFORCASTID) && x.A00DTLTBID == model.A00DTLTBID && x.ACTIVE == 1).ToList();
                                foreach (var item in inactiveRecords)
                                {
                                    item.ACTIVE = 0;
                                }
                                //long forecasuniqueid = _Assetdatabasecontext.A00INVFORCAST.AsEnumerable().OrderByDescending(x => x.A00INVFORCASTID).Select(x => x.A00INVFORCASTID).FirstOrDefault() + 1;
                                long forecasuniqueid = A00InvForcast._A00InvforcastList.AsEnumerable().OrderByDescending(x => x.A00INVFORCASTID).Select(x => x.A00INVFORCASTID).FirstOrDefault() + 1;
                                for (int i = 0; i < newSelected.Count; i++)
                                {

                                    //A00INVFORCAST forecastobj = new A00INVFORCAST();
                                    InsertA00INVFORCAST forecastobj = new InsertA00INVFORCAST();
                                    forecastobj.A00INVFORCASTID = forecasuniqueid + i;
                                    forecastobj.A00DTLTBID = ALTD.A00DTLTBID;
                                    forecastobj.DATEADDEDID = currentTime;
                                    forecastobj.INVFORCASTID = newSelected[i];
                                    forecastobj.ACTIVE = 1;

                                    _objHomeA00.InsertUpdateA00INVFORCAST(forecastobj);
                                    //_Assetdatabasecontext.A00INVFORCAST.Add(forecastobj);
                                }
                                //_Assetdatabasecontext.SaveChanges();
                            }
                            else
                            {
                                //var existingActiveRec = _Assetdatabasecontext.A00INVFORCAST.Where(x => x.A00DTLTBID == model.A00DTLTBID && x.ACTIVE == 1).ToList();
                                var existingActiveRec = A00InvForcast._A00InvforcastList.Where(x => x.A00DTLTBID == model.A00DTLTBID && x.ACTIVE == 1).ToList();
                                foreach (var item in existingActiveRec)
                                {
                                    item.ACTIVE = 0;
                                }
                                //_Assetdatabasecontext.SaveChanges();
                            }
                            #endregion
                            _ePortalDBContext.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            if (_ePortalDBContext.Database.CurrentTransaction != null)
                                _ePortalDBContext.Database.RollbackTransaction();
                            ViewBag.Msg = "Error : " + ex.Message.ToString();
                            _logger.LogError(ex, "ViewA001");
                            return View("Error");
                        }
                        #endregion
                        ViewBag.Msg = "A00 " + statusText + " successfully.";
                    }
                    else
                    {
                        if (redirectAction == "search")
                        {
                            return RedirectToAction("SearchbACK", "A00Submit", new { OPERATIONID = Convert.ToInt32(fc["hdnOPERATIONID"].ToString()), DIVISIONID = Convert.ToInt32(fc["hdnDIVISIONID"].ToString()), DEPARTMENTID = Convert.ToInt32(fc["hdnDEPARTMENTID"].ToString()), SECTIONID = Convert.ToInt32(fc["hdnSECTIONID"].ToString()), SYKIID = Convert.ToDecimal(fc["hdnSYKIID"].ToString()), ECODE = fc["hdnECODE"], EMPNAME = fc["hdnEMPNAME"], STATUSID = Convert.ToInt16(fc["hdnSTATUSID"].ToString()) });
                        }
                        else
                        {
                            if (redirectAction != null)
                            {
                                return RedirectToAction("ViewA001", new { OPERATIONID = (fc["hdnOPERATIONID"] != "" ? Convert.ToInt32(fc["hdnOPERATIONID"].ToString()) : 0), DIVISIONID = (fc["hdnDIVISIONID"] != "" ? Convert.ToInt32(fc["hdnDIVISIONID"].ToString()) : 0), DEPARTMENTID = (fc["hdnDEPARTMENTID"] != "" ? Convert.ToInt32(fc["hdnDEPARTMENTID"].ToString()) : 0), SECTIONID = (fc["hdnSECTIONID"] != "" ? Convert.ToInt32(fc["hdnSECTIONID"].ToString()) : 0), SYKIID = (fc["hdnSYKIID"] != "" ? Convert.ToDecimal(fc["hdnSYKIID"].ToString()) : 0), ECODE = fc["hdnECODE"], EMPNAME = fc["hdnEMPNAME"], STATUSID = (fc["hdnSTATUSID"] != "" ? Convert.ToInt16(fc["hdnSTATUSID"].ToString()) : 0) });
                            }
                            else
                            {
                                return RedirectToAction("ViewA001", new { A00DTLTBID = model.A00DTLTBID, Type = "Action", OPERATIONID = (fc["hdnOPERATIONID"] != "" ? Convert.ToInt32(fc["hdnOPERATIONID"].ToString()) : 0), DIVISIONID = (fc["hdnDIVISIONID"] != "" ? Convert.ToInt32(fc["hdnDIVISIONID"].ToString()) : 0), DEPARTMENTID = (fc["hdnDEPARTMENTID"] != "" ? Convert.ToInt32(fc["hdnDEPARTMENTID"].ToString()) : 0), SECTIONID = (fc["hdnSECTIONID"] != "" ? Convert.ToInt32(fc["hdnSECTIONID"].ToString()) : 0), SYKIID = (fc["hdnSYKIID"] != "" ? Convert.ToDecimal(fc["hdnSYKIID"].ToString()) : 0), ECODE = fc["hdnECODE"], EMPNAME = fc["hdnEMPNAME"], STATUSID = (fc["hdnSTATUSID"] != "" ? Convert.ToInt16(fc["hdnSTATUSID"].ToString()) : 0) });
                            }
                        }
                    }
                    //Change start on 22-July-2021  
                }
                //Change end on 22-July-2021
            }
            catch (Exception ex)
            {
                if (_ePortalDBContext.Database.CurrentTransaction != null)
                    _ePortalDBContext.Database.RollbackTransaction();
                ViewBag.Msg = "Error : " + ex.Message.ToString();
                _logger.LogError(ex, "ViewA001");
                return View("Error");
            }

            if (redirectAction != null && redirectAction != "search")
            {
                return RedirectToAction("ViewA001", new { OPERATIONID = (fc["hdnOPERATIONID"] != "" ? Convert.ToInt32(fc["hdnOPERATIONID"].ToString()) : 0), DIVISIONID = (fc["hdnDIVISIONID"] != "" ? Convert.ToInt32(fc["hdnDIVISIONID"].ToString()) : 0), DEPARTMENTID = (fc["hdnDEPARTMENTID"] != "" ? Convert.ToInt32(fc["hdnDEPARTMENTID"].ToString()) : 0), SECTIONID = (fc["hdnSECTIONID"] != "" ? Convert.ToInt32(fc["hdnSECTIONID"].ToString()) : 0), SYKIID = (fc["hdnSYKIID"] != "" ? Convert.ToDecimal(fc["hdnSYKIID"].ToString()) : 0), ECODE = fc["hdnECODE"], EMPNAME = fc["hdnEMPNAME"], STATUSID = (fc["hdnSTATUSID"] != "" ? Convert.ToInt16(fc["hdnSTATUSID"].ToString()) : 0) });
            }
            else
            {
                if (redirectAction == "search")
                {
                    return RedirectToAction("SearchBack", "A00Submit", new { returnMsgOnSearch = ViewBag.Msg, OPERATIONID = (fc["hdnOPERATIONID"] != "" ? Convert.ToInt32(fc["hdnOPERATIONID"].ToString()) : 0), DIVISIONID = (fc["hdnDIVISIONID"] != "" ? Convert.ToInt32(fc["hdnDIVISIONID"].ToString()) : 0), DEPARTMENTID = (fc["hdnDEPARTMENTID"] != "" ? Convert.ToInt32(fc["hdnDEPARTMENTID"].ToString()) : 0), SECTIONID = (fc["hdnSECTIONID"] != "" ? Convert.ToInt32(fc["hdnSECTIONID"].ToString()) : 0), SYKIID = (fc["hdnSYKIID"] != "" ? Convert.ToDecimal(fc["hdnSYKIID"].ToString()) : 0), ECODE = fc["hdnECODE"], EMPNAME = fc["hdnEMPNAME"], STATUSID = (fc["hdnSTATUSID"] != "" ? Convert.ToInt16(fc["hdnSTATUSID"].ToString()) : 0) });
                }
                else
                {
                    return RedirectToAction("ViewA001", new { A00DTLTBID = model.A00DTLTBID, returnMsg = ViewBag.Msg, Type = "Action", OPERATIONID = (fc["hdnOPERATIONID"] != "" ? Convert.ToInt32(fc["hdnOPERATIONID"].ToString()) : 0), DIVISIONID = (fc["hdnDIVISIONID"] != "" ? Convert.ToInt32(fc["hdnDIVISIONID"].ToString()) : 0), DEPARTMENTID = (fc["hdnDEPARTMENTID"] != "" ? Convert.ToInt32(fc["hdnDEPARTMENTID"].ToString()) : 0), SECTIONID = (fc["hdnSECTIONID"] != "" ? Convert.ToInt32(fc["hdnSECTIONID"].ToString()) : 0), SYKIID = (fc["hdnSYKIID"] != "" ? Convert.ToDecimal(fc["hdnSYKIID"].ToString()) : 0), ECODE = fc["hdnECODE"], EMPNAME = fc["hdnEMPNAME"], STATUSID = (fc["hdnSTATUSID"] != "" ? Convert.ToInt16(fc["hdnSTATUSID"].ToString()) : 0) });
                }
            }
        }

        public ActionResult A00ProjectStatusUpdatedApprovalListForSIS()
        {
            TempData["PageHead"] = "A00 IT Confirmation Approved list : Project Status Update";
            SearchParameterList paramsList = new SearchParameterList();
            A00ViewModel model = new A00ViewModel();
            try
            {
                var paramData = ITConfirmationgetSearchFilterData(new SearchParameterList());
                //model.SearchParams.SYKI = paramData.SYKIID;
                //model.SearchParams.Status = paramData.Status;

                ApprovedITConfirmationSearchData(model, paramData);

                List<A00ADORGLEVELList> _opList = _objHomeA00.GetA00ADORGLEVELList((decimal)paramData.SYKIID)._ADOrgLevelList;
                ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.DivList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.DepList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.SecList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "A00ProjectStatusUpdatedApprovalListForSIS");
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult A00ProjectStatusUpdatedApprovalListForSIS(A00ViewModel model, string BtnExport)
        {
            TempData["PageHead"] = "A00 IT Confirmation Approved list : Project Status Update";
            SearchParameterList paramlist = new SearchParameterList();

            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");

            var sykiId = 0;
            if (!string.IsNullOrEmpty(Request.Form["SearchParams.SYKI"].ToString()))
            {
                sykiId = Convert.ToInt32(Request.Form["SearchParams.SYKI"].ToString());
            }
            paramlist.SYKIID = sykiId;
            paramlist.ECode = Request.Form["SearchParams.ECode"].ToString();
            paramlist.EmpName = Request.Form["SearchParams.EmpName"].ToString();
            //paramlist.StatusId = Convert.ToInt16(Request.Query["SearchParams.Status"].ToString());
            paramlist.StatusId = Convert.ToInt16(Request.Form["SearchParams.Status"].ToString());

            paramlist.OPERATIONID = Request.Form["SearchParams.OPERATIONID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.OPERATIONID"]) : 0;
            paramlist.DIVISIONID = Request.Form["SearchParams.DIVISIONID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.DIVISIONID"]) : 0;
            paramlist.DEPARTMENTID = Request.Form["SearchParams.DEPARTMENTID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.DEPARTMENTID"]) : 0;
            paramlist.SECTIONID = Request.Form["SearchParams.SECTIONID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.SECTIONID"]) : 0;

            #region Block-1
            var userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

            model.SearchParams = new SearchParameterList();
            model.SearchParams.StatusId = paramlist.StatusId;
            var empCode = string.IsNullOrEmpty(Request.Form["SearchParams.ECode"].ToString()) ? 0 : Convert.ToInt64(Request.Form["SearchParams.ECode"].ToString());
            var empName = Request.Form["SearchParams.EmpName"].ToString();
            model.SearchParams.ECode = Request.Form["SearchParams.ECode"].ToString();
            model.SearchParams.EmpName = empName;
            model.SearchParams.SYKIID = sykiId;
            #endregion

            var paramData = ITConfirmationgetSearchFilterData(paramlist);

            model.SearchParams.SYKI = paramData.SYKI;
            model.SearchParams.Status = paramData.Status;

            ApprovedITConfirmationSearchData(model, paramData);

            #region added by kiran

            //List<ADORGLEVEL>

            List<A00ADORGLEVELList> _opList = _objHomeA00.GetA00ADORGLEVELList((decimal)sykiId)._ADOrgLevelList;
            ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP", paramlist.OPERATIONID);

            List<SearchParameterList> _divList = _objHomeA00.BindDivision(paramlist.OPERATIONID, sykiId);
            ViewBag.DivList = new SelectList(_divList, "DIVISIONID", "DIVISION", paramlist.DIVISIONID);

            List<SearchParameterList> _depList = _objHomeA00.BindDepartment(paramlist.DIVISIONID, 0, sykiId);
            ViewBag.DepList = new SelectList(_depList, "DEPARTMENTID", "DEPARTMENT", paramlist.DEPARTMENTID);


            List<SearchParameterList> _secList = _objHomeA00.BindSection(paramlist.DEPARTMENTID, 0, 0, sykiId);
            ViewBag.SecList = new SelectList(_secList, "SECTIONID", "SECTION", paramlist.SECTIONID);

            paramlist.OPERATIONID = employeeDetails._OpId;
            paramlist.OPERATION = employeeDetails._OpDesc;

            paramlist.DIVISIONID = employeeDetails._DivId;
            paramlist.DIVISION = employeeDetails._DivDesc;

            paramlist.DEPARTMENTID = employeeDetails._DepId;
            paramlist.DEPARTMENT = employeeDetails._DepDesc;

            paramlist.SECTIONID = employeeDetails._SecId;
            paramlist.SECTION = employeeDetails._SecDescrip;
            #endregion

            if (BtnExport == "Export to excel")
            {
                try
                {
                    //List<SearchResultList> a00ActivityDataforExcel = model.ResultList.Select(x => (new SearchResultList { KI_Code = x.KICODE, E_Code = x.ECode, Name = x.EmpName, Start_Date = x.ActivityStartDate, End_Date = x.ActivityEndDate, Status = x.ActivitySchedule, Remark = x.ActivityRemark, Activity_Title = x.ActivityTitle, STATUSCD = x.STATUSCD, ActivityDetail = x.ActivityDetail, ActivityRemarkDate = x.ActivityRemarkDate })).ToList();

                    string ExcelName = "A00 Project Status Update-" + Convert.ToString(DateTime.Now).Replace(" ", "_").Replace(":", "_").Replace("/", "_") + ".xlsx";
                    System.Data.DataTable dt = new System.Data.DataTable("A00 Project Status Update");
                    dt.Columns.AddRange(new DataColumn[11] {
                        new DataColumn("S.No"),
                        new DataColumn("KI Code"),
                        new DataColumn("Operation"),
                        new DataColumn("Project Code"),
                        new DataColumn("Project Title"),
                        new DataColumn("A00 Approval Date"),
                        new DataColumn("A00 Assigned Date"),
                        new DataColumn("IT Confirmation Date"),
                        new DataColumn("IT Confirmation Vs A00 Approval"),
                        new DataColumn("HSDM/CR"),
                        new DataColumn("Current Stage")
                    });
                    int intCntr = 0;
                    foreach (var item in model.ResultList)
                    {
                        intCntr++;
                        var DaysDiffBetA00ApprovalAndITConfirmation = item.ITConfirmationDate != null ? (Convert.ToDateTime(item.ITConfirmationDate) - Convert.ToDateTime(item.A00ApprovedOn)).Days + " Days" : (DateTime.Now - Convert.ToDateTime(item.A00ApprovedOn)).Days + " Days";

                        //Commented on 10-July-2021
                        //var currentStage = string.IsNullOrEmpty(item.ProjectStage) && item.HSDMCR == "HSDM" ? "U0" : (string.IsNullOrEmpty(item.ProjectStage) && item.HSDMCR == "CR" ? "RFQ" : item.ProjectStage);


                        //Change start on 10-July-2021
                        var projectStatus = item.ProjectUpdateStatus == 1 ? "Pending" : item.ProjectUpdateStatus == 2 ? "InProcess" : item.ProjectUpdateStatus == 3 ? "Hold" : item.ProjectUpdateStatus == 4 ? "Completed" : item.ProjectUpdateStatus == 5 ? "Not Applicable" : "Pending";
                        var currentStage = string.IsNullOrEmpty(item.ProjectStage) && item.HSDMCR == "HSDM" ? "U0" : (string.IsNullOrEmpty(item.ProjectStage) && item.HSDMCR == "CR" ? "RFQ" : item.ProjectStage);
                        //Change start on 22-July-2021
                        //currentStage = item.IsAllocationExist == false ? "Allocation Pending" : (item.ITConfirmationStatus != 8 ? "IT Confirmation Pending" :
                        //    (projectStatus != "" ? (currentStage != "-" ? currentStage + " - " + projectStatus : projectStatus) : (currentStage != "" ? currentStage : "")));
                        currentStage = item.STATUSCD == 125 ? "Sent back (Convert to CR)" : (item.IsAllocationExist == false ? "Allocation Pending" : (item.ITConfirmationStatus != 8 ? "IT Confirmation Pending" :
                            (projectStatus != "" ? (currentStage != "-" ? currentStage + " - " + projectStatus : projectStatus) : (currentStage != "" ? currentStage : ""))));
                        //Change end on 22-July-2021
                        //Change end on 10-July-2021

                        var A00ApprovedOn = item.A00ApprovedOn != null ? item.A00ApprovedOn.Value.ToString("yyyy-MM-dd") : "";
                        var ApprovedOn = item.ApprovedOn != null ? item.ApprovedOn.Value.ToString("yyyy-MM-dd") : "";
                        var ITConfirmationDate = item.ITConfirmationDate != null ? item.ITConfirmationDate.Value.ToString("yyyy-MM-dd") : "";

                        dt.Rows.Add(intCntr, item.KICODE, item.OPERATION, item.ProjectCode, item.ProjectTitle, A00ApprovedOn,
                            ApprovedOn, ITConfirmationDate, DaysDiffBetA00ApprovalAndITConfirmation, item.HSDMCR, currentStage);
                    }
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ExcelName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "A00ProjectStatusUpdatedApprovalListForSIS");
                    throw (ex);
                }
            }
            else
            {
                return View(model);
            }
        }

        #region Approved A00 : Raise deficiency and Allocate project to respective application PIC & infra PIC
        public ActionResult ITDivisionA00Deficiency(string returnMsg = null, long? A00DTLTBID = null, string type = null,
            long? OPERATIONID = null, long? DIVISIONID = null, long? DEPARTMENTID = null, long? SECTIONID = null, decimal? SYKIID = null,
            string ECODE = null, string EMPNAME = null, short? STATUSID = null)
        {
            try
            {
                TempData["PageHead"] = "Report Deficiency for Approved A00 by Division Heads/ OH";
                //check deficiency status
                int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

                var kiData1 = _objHomeA00.GetA00SYKIList();
                List<SYKI> iList = new List<SYKI>();
                iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });

                var kiData = (from data in kiData1._SYKIList select data).ToList();
                foreach (var item in kiData)
                {
                    iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                }
                var ActiveKiID = SYKIID > 0 ? SYKIID : kiData.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();

                ViewBag.DeficiencyStatus = _objHomeA00.GetDeficiencyStatus(A00DTLTBID, ActiveKiID);
                ViewBag.GetUserRoleForDeficiency = _objHomeA00.GetUserRoleForDeficiency(userId, ActiveKiID);
                SetSearchValue(OPERATIONID, DIVISIONID, DEPARTMENTID, SECTIONID, ActiveKiID, ECODE, EMPNAME, STATUSID);
                if (returnMsg != null)
                {
                    ViewBag.Msg = returnMsg;
                }


                TempData["PageHead"] = "A00 IT Approval";
                ViewBag.Years = DateTime.Now.Year;
                type = type == null ? "NEW" : type.ToUpper();
                ViewBag.pageType = type;
                ViewBag.actionType = "Deficiency";
                ViewBag.isUploadedFilesExist = "N";
                //int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

                #region In New Mode               

                var operation = _objHomeA00.GetA00ADORGLEVELList(ActiveKiID);
                List<SearchResultList> budgByList = new List<SearchResultList>();
                budgByList.Add(new SearchResultList { LEVELDESCRIP = "Select", ADORGLEVELID = 0 });
                foreach (var item in operation._ADOrgLevelList)
                {
                    budgByList.Add(new SearchResultList { LEVELDESCRIP = item.LEVELDESCRIP, ADORGLEVELID = item.ADORGLEVELID });
                }


                List<SelectListItem> datalist = new List<SelectListItem>();
                var InvforcastList = _objHomeA00.GetA00_INVFORCASTList();
                InvforcastList._InvforcastList
                    .Select(Datat4 => new SelectListItem { Text = Datat4.INCFORCASTDETAIL, Value = Datat4.INVFORCASTID.ToString() });
                ViewBag.SKYLIST = InvforcastList._InvforcastList;
                //foreach (var Datat4 in InvforcastList._InvforcastList)
                //{
                //    datalist.Add(new SelectListItem { Text = Datat4.INCFORCASTDETAIL, Value = Datat4.INVFORCASTID.ToString() });
                //}
                //ViewBag.SKYLIST = datalist;

                A00ViewModel objModel = new A00ViewModel();
                objModel.getAllDaysList = objModel.getAllWeekDaysList();
                objModel.GetAllmonth = objModel.getMonth();
                objModel.Getdate = objModel.GetAllDate();
                #endregion
                ViewBag.isOtherSelected = "N";
                var A00DeficiencyRaisedBY = Convert.ToInt32(_sessionService.Get<string>("userID"));

                #region When View/Action Button Pressed
                if (type == "VIEW" || type == "ACTION")
                {
                    _objA00SearchModel.a00dtltbid = A00DTLTBID;
                    var _A00ExistData = _objHomeA00.GetA00Dtl(_objA00SearchModel);
                    objModel.Projecttitle = _A00ExistData.PRJCTTLE;
                    objModel.Backgrounds = _A00ExistData.PRJCTTXT;
                    objModel.BusinessKPI = _A00ExistData.BUKPITXT;
                    objModel.PurposeA00 = _A00ExistData.PURPSTXT;
                    objModel.TargetA00 = _A00ExistData.TRGTINDCD;
                    objModel.RequirmentA00 = _A00ExistData.RQUMTTXT;
                    objModel.ImagePath = _A00ExistData.ATTACHMENT;
                    objModel.BudgetedSelected = _A00ExistData.BUDGETFLG.ToString();
                    objModel.StartDate = _A00ExistData.STARTDT?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objModel.EndDate = _A00ExistData.ENDDT?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objModel.Ifothers = _A00ExistData.FRCSTOTHER;
                    ViewBag.request_date = _A00ExistData.DATEADDEDID.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objModel.ADDEDBY1 = _objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE.Where(x1 => x1.ADEMPCODE == _A00ExistData.ADDEDBY).Select(x1 => x1.FIRSTNAME + " " + x1.LASTNAME).FirstOrDefault();
                    ViewBag.userName = objModel.ADDEDBY1;
                    //var vwList = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS_FullList();
                    //objModel.OPERATION_PROPOSING = vwList._A00_VW_ASSOCIATELVLDETAILS_FULL.Where(x => x.ADEMPCODE == _A00ExistData.ADDEDBY).Select(x1 => x1.OPERATION).FirstOrDefault();

                    _objA00SearchModel.ADEMPCODE = _A00ExistData.ADDEDBY;
                    _objA00SearchModel.SYKI = (long)_A00ExistData.SYKIID;
                    objModel.OPERATION_PROPOSING = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel).OPERATION;
                    ViewBag.OPERATION_P = objModel.OPERATION_PROPOSING;

                    if (A00DTLTBID != null && _A00ExistData != null)
                    {
                        var currStatus = _A00ExistData.STATUSCD;
                        if (currStatus == 105)
                        {
                            ViewBag.actionType = "Deficiency";
                            this.generateApprovalHistory(A00DTLTBID ?? 0);
                        }
                        else
                        {
                            return RedirectToAction("ITDivisionApprovalSearch");
                        }
                    }

                    List<SelectListItem> mlist = new List<SelectListItem>();
                    mlist.Add(new SelectListItem { Text = "Select", Value = "", Disabled = true });

                    if (_A00ExistData.BUDGETFLG.ToString() == "1")
                    {
                        mlist.Add(new SelectListItem { Text = "Yes", Value = "10", Selected = true });
                        mlist.Add(new SelectListItem { Text = "No", Value = "11" });
                    }
                    else if (_A00ExistData.BUDGETFLG.ToString() == "0")
                    {
                        mlist.Add(new SelectListItem { Text = "No", Value = "11", Selected = true });
                        mlist.Add(new SelectListItem { Text = "Yes", Value = "10" });
                    }

                    ViewBag.BudgetedValue = mlist;
                    ViewBag.selectedBudgetedVal = _A00ExistData.BUDGETFLG.ToString();
                    var selectedBudgBy = budgByList.Where(x => x.ADORGLEVELID == _A00ExistData.ADORGLEVELID).FirstOrDefault();
                    ViewBag.List = new SelectList(budgByList, "ADORGLEVELID", "LEVELDESCRIP", selectedBudgBy?.ADORGLEVELID);
                    var selectedSyKI = iList.Where(x => x.SYKIID == _A00ExistData.BUDGETSYKIID).FirstOrDefault();
                    ViewBag.SKY = new SelectList(iList, "SYKIID", "KICODE", selectedSyKI?.SYKIID);



                    var A00InvForcast = _objHomeA00.GetA00INVFORCASTList();

                    var ief = A00InvForcast._A00InvforcastList.Join(InvforcastList._InvforcastList, a => a.INVFORCASTID, b => b.INVFORCASTID, (a, b) => new { a, b }).
                      Where(x => x.a.A00DTLTBID == A00DTLTBID).Select(m => new { INVFORCASTID = m.a.INVFORCASTID, INCFORCASTDETAIL = m.b.INCFORCASTDETAIL }).Distinct().ToList();
                    if (type == "ACTION")
                    {
                        datalist.Where(x => ief.Select(y => y.INVFORCASTID.ToString()).ToList().Contains(x.Value)).ToList().ForEach(z => z.Selected = true);
                        if (_A00ExistData.FRCSTOTHER != null)
                        {
                            var OtherSelected = InvforcastList._InvforcastList.Where(x => x.INCFORCASTDETAIL.ToUpper() == "OTHERS").FirstOrDefault();
                            datalist.Where(x => x.Value == OtherSelected.INVFORCASTID.ToString()).Select(x => x.Selected = true);
                            ViewBag.isOtherSelected = "Y";
                        }
                        ViewBag.SKYLIST = datalist;
                        //string filePath = Path.Combine(serverpath.getFileUploadPath(),"A00");
                        string filePath = Path.Combine(serverpath.getFileUploadPath(), "A00");
                        string FileName = Path.GetFileNameWithoutExtension(_A00ExistData.ATTACHMENT);
                        string FileExtension = Path.GetExtension(_A00ExistData.ATTACHMENT);

                        FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;
                    }
                    else
                    {
                        List<SelectListItem> Vdatalist = new List<SelectListItem>();
                        foreach (var Datat4 in ief)
                        {
                            Vdatalist.Add(new SelectListItem { Text = Datat4.INCFORCASTDETAIL, Value = Datat4.INVFORCASTID.ToString() });
                        }
                        if (_A00ExistData.FRCSTOTHER != null)
                        {
                            var OtherSelected = InvforcastList._InvforcastList.Where(x => x.INCFORCASTDETAIL.ToUpper() == "OTHERS").FirstOrDefault();
                            Vdatalist.Add(new SelectListItem { Text = OtherSelected.INCFORCASTDETAIL.ToString(), Value = OtherSelected.INVFORCASTID.ToString() });
                            ViewBag.isOtherSelected = "Y";
                        }
                        ViewBag.SKYLIST = Vdatalist;
                    }
                }
                else
                {
                    List<SelectListItem> mlist = new List<SelectListItem>();
                    mlist.Add(new SelectListItem { Text = "Select", Selected = true });
                    mlist.Add(new SelectListItem { Text = "Yes", Value = "10" });
                    mlist.Add(new SelectListItem { Text = "No", Value = "11" });
                    ViewBag.BudgetedValue = mlist;
                    ViewBag.List = new SelectList(budgByList, "ADORGLEVELID", "LEVELDESCRIP", 0);
                    ViewBag.SKY = new SelectList(iList, "SYKIID", "KICODE", 0);
                }
                #endregion

                //var activeKi = kiData1._SYKIList.Where(x => x.ACTIVE == 1).FirstOrDefault();
                var DeficiencyDetails = _objHomeA00.GetDeficiencyRaisedDetails(A00DTLTBID, ActiveKiID);
                ViewBag.DeficiencyDetails = DeficiencyDetails;
                var actionBy = DeficiencyDetails.Select(x => x.EmpActionBy).FirstOrDefault();
                ViewBag.CanPerformAction = actionBy != 0 ? (A00DeficiencyRaisedBY == actionBy ? 1 : 2) : 0;
                ViewBag.AllocationDetails = _objHomeA00.GetAllocationDetails(A00DTLTBID, ActiveKiID);

                //InsertDeffienceforbind();
                //BindEmployee();

                return View(objModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ITDivisionA00Deficiency");
                return View();
            }
        }

        #region
        //Development by utkarshita

        [HttpPost]
        public ActionResult SaveA00Deficiency(IFormCollection fc)
        {
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var remark = fc["DeficiencyRemark"];
            IFormFile file = Request.Form.Files.Count > 0 ? HttpContext.Request.Form.Files[0] : null;
            string fileName = file?.FileName;
            var isDeficiencyFileExist = fc["isDeficiencyFileExist"];
            long SYKI = !string.IsNullOrWhiteSpace(fc["hdnSYKIID"]) ? Convert.ToInt64(fc["hdnSYKIID"]) : 0;
            long A00DTLTBID = !string.IsNullOrWhiteSpace(fc["A00DTLTBID"]) ? Convert.ToInt64(fc["A00DTLTBID"]) : 0;
            if (file != null && !string.IsNullOrWhiteSpace(file.FileName))
            {
                //string UploadPath = Server.MapPath(ConfigurationManager.AppSettings["folderPath"].ToString() + "\\A00\\A00Deficiency");
                string UploadPath = Path.Combine(serverpath.getFileUploadPath(), "A00", "A00Deficiency");
                fileName = Path.GetFileName(file.FileName);
                fileName = Convert.ToString(_sessionService.Get<string>("userID")) + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + "_" + DateTime.Now.Second.ToString() + "_" + DateTime.Now.Millisecond.ToString() + fileName;
                string FilePath = Path.Combine(UploadPath, fileName);
                //file[0].SaveAs(FilePath);
                string directoryPath = Path.GetDirectoryName(FilePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                using (var stream = new FileStream(FilePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            else
            {
                fileName = "NA";
            }
            var emailId = _objHomeA00.GetUserEmailForDeficiencyRaised(A00DTLTBID, SYKI);
            var ProjectTitle = _objHomeA00.GetA00ProjectTitle(A00DTLTBID, SYKI);
            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(userId);
            sendMailTODeficiencyOwnerForDeficiencyUpdate(emailId, ProjectTitle, DateTime.Now, requesterDetails.Name, userId.ToString());
            _objHomeA00.SaveA00Deficiency(remark, fileName, SYKI, isDeficiencyFileExist, userId, A00DTLTBID);
            return RedirectToAction("ITDivisionApprovalSearch");
        }

        public ActionResult A00DeficiencyUpdate(string returnMsg = null, long? A00DTLTBID = null, string type = null,
            long? OPERATIONID = null, long? DIVISIONID = null, long? DEPARTMENTID = null, long? SECTIONID = null, decimal? SYKIID = null,
            string ECODE = null, string EMPNAME = null, short? STATUSID = null)
        {
            try
            {

                TempData["PageHead"] = "Report Deficiency for Approved A00 by Division Heads/ OH";
                //check deficiency status
                int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

                var kiData1 = _objHomeA00.GetA00SYKIList();
                List<SYKI> iList = new List<SYKI>();
                iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });

                var kiData = (from data in kiData1._SYKIList select data).ToList();
                foreach (var item in kiData)
                {
                    iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                }
                var ActiveKiID = SYKIID > 0 ? SYKIID : kiData.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();

                ViewBag.DeficiencyStatus = _objHomeA00.GetDeficiencyStatus(A00DTLTBID, ActiveKiID);
                ViewBag.GetUserRoleForDeficiency = _objHomeA00.GetUserRoleForDeficiency(userId, ActiveKiID);
                SetSearchValue(OPERATIONID, DIVISIONID, DEPARTMENTID, SECTIONID, ActiveKiID, ECODE, EMPNAME, STATUSID);
                if (returnMsg != null)
                {
                    ViewBag.Msg = returnMsg;
                }


                TempData["PageHead"] = "A00 IT Approval";
                ViewBag.Years = DateTime.Now.Year;
                type = type == null ? "NEW" : type.ToUpper();
                ViewBag.pageType = type;
                ViewBag.actionType = "Deficiency";
                ViewBag.isUploadedFilesExist = "N";
                //int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

                #region In New Mode               

                var operation = _objHomeA00.GetA00ADORGLEVELList(ActiveKiID);
                List<SearchResultList> budgByList = new List<SearchResultList>();
                budgByList.Add(new SearchResultList { LEVELDESCRIP = "Select", ADORGLEVELID = 0 });
                foreach (var item in operation._ADOrgLevelList)
                {
                    budgByList.Add(new SearchResultList { LEVELDESCRIP = item.LEVELDESCRIP, ADORGLEVELID = item.ADORGLEVELID });
                }


                List<SelectListItem> datalist = new List<SelectListItem>();
                var InvforcastList = _objHomeA00.GetA00_INVFORCASTList();

                foreach (var Datat4 in InvforcastList._InvforcastList)
                {
                    datalist.Add(new SelectListItem { Text = Datat4.INCFORCASTDETAIL, Value = Datat4.INVFORCASTID.ToString() });
                }
                ViewBag.SKYLIST = datalist;

                A00ViewModel objModel = new A00ViewModel();
                objModel.getAllDaysList = objModel.getAllWeekDaysList();
                objModel.GetAllmonth = objModel.getMonth();
                objModel.Getdate = objModel.GetAllDate();
                #endregion
                ViewBag.isOtherSelected = "N";
                var A00AddedBY = Convert.ToInt32(_sessionService.Get<string>("userID"));
                #region When View/Action Button Pressed
                if (type == "VIEW" || type == "ACTION")
                {
                    _objA00SearchModel.a00dtltbid = A00DTLTBID;
                    var _A00ExistData = _objHomeA00.GetA00Dtl(_objA00SearchModel);
                    objModel.Projecttitle = _A00ExistData.PRJCTTLE;
                    objModel.Backgrounds = _A00ExistData.PRJCTTXT;
                    objModel.BusinessKPI = _A00ExistData.BUKPITXT;
                    objModel.PurposeA00 = _A00ExistData.PURPSTXT;
                    objModel.TargetA00 = _A00ExistData.TRGTINDCD;
                    objModel.RequirmentA00 = _A00ExistData.RQUMTTXT;
                    objModel.ImagePath = _A00ExistData.ATTACHMENT;
                    objModel.BudgetedSelected = _A00ExistData.BUDGETFLG.ToString();
                    objModel.StartDate = _A00ExistData.STARTDT?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objModel.EndDate = _A00ExistData.ENDDT?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objModel.Ifothers = _A00ExistData.FRCSTOTHER;
                    ViewBag.request_date = _A00ExistData.DATEADDEDID.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objModel.ADDEDBY1 = _objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE.Where(x1 => x1.ADEMPCODE == _A00ExistData.ADDEDBY).Select(x1 => x1.FIRSTNAME + " " + x1.LASTNAME).FirstOrDefault();
                    ViewBag.userName = objModel.ADDEDBY1;

                    //var vwList = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS_FullList();
                    //objModel.OPERATION_PROPOSING = vwList._A00_VW_ASSOCIATELVLDETAILS_FULL.Where(x => x.ADEMPCODE == _A00ExistData.ADDEDBY).Select(x1 => x1.OPERATION).FirstOrDefault();
                    _objA00SearchModel.ADEMPCODE = _A00ExistData.ADDEDBY;
                    _objA00SearchModel.SYKI = (long)_A00ExistData.SYKIID;
                    objModel.OPERATION_PROPOSING = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel).OPERATION;
                    ViewBag.OPERATION_P = objModel.OPERATION_PROPOSING;


                    if (A00DTLTBID != null && _A00ExistData != null)
                    {
                        var currStatus = _A00ExistData.STATUSCD;
                        if (currStatus == 105)
                        {
                            ViewBag.actionType = "Deficiency";
                            this.generateApprovalHistory(A00DTLTBID ?? 0);
                        }
                        else
                        {
                            return RedirectToAction("ITDivisionApprovalSearch");
                        }
                    }

                    List<SelectListItem> mlist = new List<SelectListItem>();
                    mlist.Add(new SelectListItem { Text = "Select", Value = "", Disabled = true });

                    if (_A00ExistData.BUDGETFLG.ToString() == "1")
                    {
                        mlist.Add(new SelectListItem { Text = "Yes", Value = "10", Selected = true });
                        mlist.Add(new SelectListItem { Text = "No", Value = "11" });
                    }
                    else if (_A00ExistData.BUDGETFLG.ToString() == "0")
                    {
                        mlist.Add(new SelectListItem { Text = "No", Value = "11", Selected = true });
                        mlist.Add(new SelectListItem { Text = "Yes", Value = "10" });
                    }

                    ViewBag.BudgetedValue = mlist;
                    ViewBag.selectedBudgetedVal = _A00ExistData.BUDGETFLG.ToString();
                    var selectedBudgBy = budgByList.Where(x => x.ADORGLEVELID == _A00ExistData.ADORGLEVELID).FirstOrDefault();
                    ViewBag.List = new SelectList(budgByList, "ADORGLEVELID", "LEVELDESCRIP", selectedBudgBy?.ADORGLEVELID);
                    var selectedSyKI = iList.Where(x => x.SYKIID == _A00ExistData.BUDGETSYKIID).FirstOrDefault();
                    ViewBag.SKY = new SelectList(iList, "SYKIID", "KICODE", selectedSyKI?.SYKIID);



                    var A00InvForcast = _objHomeA00.GetA00INVFORCASTList();

                    var ief = A00InvForcast._A00InvforcastList.Join(InvforcastList._InvforcastList, a => a.INVFORCASTID, b => b.INVFORCASTID, (a, b) => new { a, b }).
                      Where(x => x.a.A00DTLTBID == A00DTLTBID).Select(m => new { INVFORCASTID = m.a.INVFORCASTID, INCFORCASTDETAIL = m.b.INCFORCASTDETAIL }).Distinct().ToList();
                    if (type == "ACTION")
                    {
                        datalist.Where(x => ief.Select(y => y.INVFORCASTID.ToString()).ToList().Contains(x.Value)).ToList().ForEach(z => z.Selected = true);
                        if (_A00ExistData.FRCSTOTHER != null)
                        {
                            var OtherSelected = InvforcastList._InvforcastList.Where(x => x.INCFORCASTDETAIL.ToUpper() == "OTHERS").FirstOrDefault();
                            datalist.Where(x => x.Value == OtherSelected.INVFORCASTID.ToString()).Select(x => x.Selected = true);
                            ViewBag.isOtherSelected = "Y";
                        }
                        ViewBag.SKYLIST = datalist;
                        //string filePath = Server.MapPath(Configurastring filePath = Path.Combine(serverpath.getFileUploadPath(), "A00"); tionManager.AppSettings["folderPath"].ToString() + "\\A00\\");
                        string filePath = Path.Combine(serverpath.getFileUploadPath(), "A00");
                        string FileName = Path.GetFileNameWithoutExtension(_A00ExistData.ATTACHMENT);
                        string FileExtension = Path.GetExtension(_A00ExistData.ATTACHMENT);

                        FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;
                    }
                    else
                    {
                        List<SelectListItem> Vdatalist = new List<SelectListItem>();
                        foreach (var Datat4 in ief)
                        {
                            Vdatalist.Add(new SelectListItem { Text = Datat4.INCFORCASTDETAIL, Value = Datat4.INVFORCASTID.ToString() });
                        }
                        if (_A00ExistData.FRCSTOTHER != null)
                        {
                            var OtherSelected = InvforcastList._InvforcastList.Where(x => x.INCFORCASTDETAIL.ToUpper() == "OTHERS").FirstOrDefault();
                            Vdatalist.Add(new SelectListItem { Text = OtherSelected.INCFORCASTDETAIL.ToString(), Value = OtherSelected.INVFORCASTID.ToString() });
                            ViewBag.isOtherSelected = "Y";
                        }
                        ViewBag.SKYLIST = Vdatalist;
                    }
                }
                else
                {
                    List<SelectListItem> mlist = new List<SelectListItem>();
                    mlist.Add(new SelectListItem { Text = "Select", Selected = true });
                    mlist.Add(new SelectListItem { Text = "Yes", Value = "10" });
                    mlist.Add(new SelectListItem { Text = "No", Value = "11" });
                    ViewBag.BudgetedValue = mlist;
                    ViewBag.List = new SelectList(budgByList, "ADORGLEVELID", "LEVELDESCRIP", 0);
                    ViewBag.SKY = new SelectList(iList, "SYKIID", "KICODE", 0);
                }
                #endregion

                //var activeKi = kiData1._SYKIList.Where(x => x.ACTIVE == 1).FirstOrDefault();
                var DeficiencyDetails = _objHomeA00.GetDeficiencyRaisedDetails(A00DTLTBID, ActiveKiID);
                ViewBag.DeficiencyDetails = DeficiencyDetails;
                ViewBag.CanPerformAction = A00AddedBY == DeficiencyDetails.Select(x => x.ActionFor).FirstOrDefault() ? true : false;
                ViewBag.AllocationDetails = _objHomeA00.GetAllocationDetails(A00DTLTBID, ActiveKiID);
                //InsertDeffienceforbind();
                //BindEmployee();

                return View(objModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "A00DeficiencyUpdate");
                return View();
            }
        }

        [HttpPost]
        public ActionResult SaveA00DeficiencyUpdate(IFormCollection fc)
        {
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var remark = fc["DeficiencyRemark"];
            //IFormFile file = Request.Form.Files[0];
            IFormFile file = Request.Form.Files.Count > 0 ? HttpContext.Request.Form.Files[0] : null;
            string fileName = file?.FileName;
            var isDeficiencyFileExist = fc["isDeficiencyFileExist"];
            long SYKI = !string.IsNullOrWhiteSpace(fc["hdnSYKIID"]) ? Convert.ToInt64(fc["hdnSYKIID"]) : 0;
            long A00DTLTBID = !string.IsNullOrWhiteSpace(fc["A00DTLTBID"]) ? Convert.ToInt64(fc["A00DTLTBID"]) : 0;
            //string UploadPath = Server.MapPath(ConfigurationManager.AppSettings["folderPath"].ToString() + "\\A00\\A00Deficiency");
            string UploadPath = Path.Combine(serverpath.getFileUploadPath(), "A00", "A00Deficiency");

            if (file != null && !string.IsNullOrEmpty(file.FileName))
            {
                fileName = Path.GetFileName(file.FileName);
                fileName = Convert.ToString(_sessionService.Get<string>("userID")) + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + "_" + DateTime.Now.Second.ToString() + "_" + DateTime.Now.Millisecond.ToString() + fileName;
                string FilePath = Path.Combine(UploadPath, fileName);
                //file[0].SaveAs(FilePath);
                string directoryPath = Path.GetDirectoryName(FilePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                using (var stream = new FileStream(FilePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            else
            {
                fileName = "NA";
            }
            var emailId = _objHomeA00.GetDeficiencyUpdateEmialId(A00DTLTBID, SYKI);
            var ProjectTitle = _objHomeA00.GetA00ProjectTitle(A00DTLTBID, SYKI);
            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(userId);
            sendMailForDeficiencyUpdate(emailId, ProjectTitle, DateTime.Now, requesterDetails.Name, userId.ToString());
            _objHomeA00.SaveA00DeficiencyUpdate(remark, fileName, SYKI, isDeficiencyFileExist, userId, A00DTLTBID);
            return RedirectToAction("Search");

        }

        public ActionResult SaveA00DeficiencyClosure(IFormCollection fc)
        {
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var remark = fc["DeficiencyRemark"];
            int deficiencyStatus = Convert.ToInt16(fc["deficiencyStatus"]);
            long SYKI = !string.IsNullOrWhiteSpace(fc["hdnSYKIID"]) ? Convert.ToInt64(fc["hdnSYKIID"]) : 0;
            long A00DTLTBID = !string.IsNullOrWhiteSpace(fc["A00DTLTBID"]) ? Convert.ToInt64(fc["A00DTLTBID"]) : 0;
            _objHomeA00.SaveA00DeficiencyClosure(userId, A00DTLTBID, SYKI, remark, deficiencyStatus);
            return RedirectToAction("ITDivisionApprovalSearch");

        }

        public ActionResult ITDivisionA00Allocation(string returnMsg = null, long? A00DTLTBID = null, string type = null,
            long? OPERATIONID = null, long? DIVISIONID = null, long? DEPARTMENTID = null, long? SECTIONID = null, decimal? SYKIID = null,
            string ECODE = null, string EMPNAME = null, short? STATUSID = null)
        {
            try
            {
                TempData["PageHead"] = "Approved A00 Allocation by Division Heads/ OH";

                var kiData1 = _objHomeA00.GetA00SYKIList();
                List<SYKI> iList = new List<SYKI>();
                iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });

                var kiData = (from data in kiData1._SYKIList select data).ToList();
                foreach (var item in kiData)
                {
                    iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                }
                var ActiveKiID = SYKIID > 0 ? SYKIID : kiData.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();

                SetSearchValue(OPERATIONID, DIVISIONID, DEPARTMENTID, SECTIONID, ActiveKiID, ECODE, EMPNAME, STATUSID);
                if (returnMsg != null)
                {
                    ViewBag.Msg = returnMsg;
                }

                TempData["PageHead"] = "A00 IT Approval";
                ViewBag.DeficiencyDetails = _objHomeA00.GetDeficiencyRaisedDetails(A00DTLTBID, ActiveKiID);
                ViewBag.AllocationDetails = _objHomeA00.GetAllocationDetails(A00DTLTBID, ActiveKiID);
                ViewBag.Years = DateTime.Now.Year;
                type = type == null ? "NEW" : type.ToUpper();
                ViewBag.pageType = type;
                ViewBag.actionType = "Allocation";
                ViewBag.isUploadedFilesExist = "N";
                int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

                #region In New Mode


                var operation = _objHomeA00.GetA00ADORGLEVELList(ActiveKiID);
                List<SearchResultList> budgByList = new List<SearchResultList>();
                budgByList.Add(new SearchResultList { LEVELDESCRIP = "Select", ADORGLEVELID = 0 });
                foreach (var item in operation._ADOrgLevelList)
                {
                    budgByList.Add(new SearchResultList { LEVELDESCRIP = item.LEVELDESCRIP, ADORGLEVELID = item.ADORGLEVELID });
                }


                List<SelectListItem> datalist = new List<SelectListItem>();
                var InvforcastList = _objHomeA00.GetA00_INVFORCASTList();

                foreach (var Datat4 in InvforcastList._InvforcastList)
                {
                    datalist.Add(new SelectListItem { Text = Datat4.INCFORCASTDETAIL, Value = Datat4.INVFORCASTID.ToString() });
                }
                ViewBag.SKYLIST = datalist;

                A00ViewModel objModel = new A00ViewModel();
                objModel.getAllDaysList = objModel.getAllWeekDaysList();
                objModel.GetAllmonth = objModel.getMonth();
                objModel.Getdate = objModel.GetAllDate();
                #endregion
                ViewBag.isOtherSelected = "N";

                #region When View/Action Button Pressed
                if (type == "VIEW" || type == "ACTION")
                {
                    _objA00SearchModel.a00dtltbid = A00DTLTBID;
                    var _A00ExistData = _objHomeA00.GetA00Dtl(_objA00SearchModel);
                    objModel.Projecttitle = _A00ExistData.PRJCTTLE;
                    objModel.Backgrounds = _A00ExistData.PRJCTTXT;
                    objModel.BusinessKPI = _A00ExistData.BUKPITXT;
                    objModel.PurposeA00 = _A00ExistData.PURPSTXT;
                    objModel.TargetA00 = _A00ExistData.TRGTINDCD;
                    objModel.RequirmentA00 = _A00ExistData.RQUMTTXT;
                    objModel.ImagePath = _A00ExistData.ATTACHMENT;
                    objModel.BudgetedSelected = _A00ExistData.BUDGETFLG.ToString();
                    objModel.StartDate = _A00ExistData.STARTDT?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objModel.EndDate = _A00ExistData.ENDDT?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objModel.Ifothers = _A00ExistData.FRCSTOTHER;
                    ViewBag.request_date = _A00ExistData.DATEADDEDID.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objModel.ADDEDBY1 = _objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE.Where(x1 => x1.ADEMPCODE == _A00ExistData.ADDEDBY).Select(x1 => x1.FIRSTNAME + " " + x1.LASTNAME).FirstOrDefault();
                    ViewBag.userName = objModel.ADDEDBY1;

                    //var vwList = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS_FullList();
                    //objModel.OPERATION_PROPOSING = vwList._A00_VW_ASSOCIATELVLDETAILS_FULL.Where(x => x.ADEMPCODE == _A00ExistData.ADDEDBY).Select(x1 => x1.OPERATION).FirstOrDefault();

                    _objA00SearchModel.ADEMPCODE = _A00ExistData.ADDEDBY;
                    _objA00SearchModel.SYKI = (long)_A00ExistData.SYKIID;
                    objModel.OPERATION_PROPOSING = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel).OPERATION;
                    ViewBag.OPERATION_P = objModel.OPERATION_PROPOSING;


                    if (A00DTLTBID != null && _A00ExistData != null)
                    {
                        var currStatus = _A00ExistData.STATUSCD;
                        if (currStatus == 105)
                        {
                            ViewBag.actionType = "Allocation";
                            this.generateApprovalHistory(A00DTLTBID ?? 0);
                        }
                        else
                        {
                            return RedirectToAction("ITDivisionApprovalSearch");
                        }
                    }

                    List<SelectListItem> mlist = new List<SelectListItem>();
                    mlist.Add(new SelectListItem { Text = "Select", Value = "", Disabled = true });

                    if (_A00ExistData.BUDGETFLG.ToString() == "1")
                    {
                        mlist.Add(new SelectListItem { Text = "Yes", Value = "10", Selected = true });
                        mlist.Add(new SelectListItem { Text = "No", Value = "11" });
                    }
                    else if (_A00ExistData.BUDGETFLG.ToString() == "0")
                    {
                        mlist.Add(new SelectListItem { Text = "No", Value = "11", Selected = true });
                        mlist.Add(new SelectListItem { Text = "Yes", Value = "10" });
                    }

                    ViewBag.BudgetedValue = mlist;
                    ViewBag.selectedBudgetedVal = _A00ExistData.BUDGETFLG.ToString();
                    var selectedBudgBy = budgByList.Where(x => x.ADORGLEVELID == _A00ExistData.ADORGLEVELID).FirstOrDefault();
                    ViewBag.List = new SelectList(budgByList, "ADORGLEVELID", "LEVELDESCRIP", selectedBudgBy?.ADORGLEVELID);
                    var selectedSyKI = iList.Where(x => x.SYKIID == _A00ExistData.BUDGETSYKIID).FirstOrDefault();
                    ViewBag.SKY = new SelectList(iList, "SYKIID", "KICODE", selectedSyKI?.SYKIID);



                    var A00InvForcast = _objHomeA00.GetA00INVFORCASTList();

                    var ief = A00InvForcast._A00InvforcastList.Join(InvforcastList._InvforcastList, a => a.INVFORCASTID, b => b.INVFORCASTID, (a, b) => new { a, b }).
                      Where(x => x.a.A00DTLTBID == A00DTLTBID).Select(m => new { INVFORCASTID = m.a.INVFORCASTID, INCFORCASTDETAIL = m.b.INCFORCASTDETAIL }).Distinct().ToList();
                    if (type == "ACTION")
                    {
                        datalist.Where(x => ief.Select(y => y.INVFORCASTID.ToString()).ToList().Contains(x.Value)).ToList().ForEach(z => z.Selected = true);
                        if (_A00ExistData.FRCSTOTHER != null)
                        {
                            var OtherSelected = InvforcastList._InvforcastList.Where(x => x.INCFORCASTDETAIL.ToUpper() == "OTHERS").FirstOrDefault();
                            datalist.Where(x => x.Value == OtherSelected.INVFORCASTID.ToString()).Select(x => x.Selected = true);
                            ViewBag.isOtherSelected = "Y";
                        }
                        ViewBag.SKYLIST = datalist;
                        string filePath = Path.Combine(serverpath.getFileUploadPath(), "A00");
                        string FileName = Path.GetFileNameWithoutExtension(_A00ExistData.ATTACHMENT);
                        string FileExtension = Path.GetExtension(_A00ExistData.ATTACHMENT);

                        FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;
                    }
                    else
                    {
                        List<SelectListItem> Vdatalist = new List<SelectListItem>();
                        foreach (var Datat4 in ief)
                        {
                            Vdatalist.Add(new SelectListItem { Text = Datat4.INCFORCASTDETAIL, Value = Datat4.INVFORCASTID.ToString() });
                        }
                        if (_A00ExistData.FRCSTOTHER != null)
                        {
                            var OtherSelected = InvforcastList._InvforcastList.Where(x => x.INCFORCASTDETAIL.ToUpper() == "OTHERS").FirstOrDefault();
                            Vdatalist.Add(new SelectListItem { Text = OtherSelected.INCFORCASTDETAIL.ToString(), Value = OtherSelected.INVFORCASTID.ToString() });
                            ViewBag.isOtherSelected = "Y";
                        }
                        ViewBag.SKYLIST = Vdatalist;
                    }
                }
                else
                {
                    List<SelectListItem> mlist = new List<SelectListItem>();
                    mlist.Add(new SelectListItem { Text = "Select", Selected = true });
                    mlist.Add(new SelectListItem { Text = "Yes", Value = "10" });
                    mlist.Add(new SelectListItem { Text = "No", Value = "11" });
                    ViewBag.BudgetedValue = mlist;
                    ViewBag.List = new SelectList(budgByList, "ADORGLEVELID", "LEVELDESCRIP", 0);
                    ViewBag.SKY = new SelectList(iList, "SYKIID", "KICODE", 0);
                }
                #endregion

                //var activeKi = kiData1._SYKIList.Where(x => x.ACTIVE == 1).FirstOrDefault();
                A00SearchModel _objA00SearchModel1 = new A00SearchModel();
                _objA00SearchModel1.ADEMPCODE = userId;
                _objA00SearchModel1.SYKI = (decimal)ActiveKiID;
                var UserOperationID = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel1);

                List<GetEmployeeA00AllocationVM> _employeeList = _objHomeA00.GetEmployeeA00AllocationList((long)UserOperationID.OPERATIONID, (long)ActiveKiID);
                ViewBag.EmployeeList = new SelectList(_employeeList, "Id", "Employee");
                //InsertDeffienceforbind();
                //BindEmployee();

                return View(objModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ITDivisionA00Allocation");
                return View();
            }
        }
        [HttpPost]
        public ActionResult SaveA00Allocation(IFormCollection fc)
        {
            try
            {
                int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
                var ApplicationPIC = Convert.ToInt64(fc["ApplicationPIC"]);
                var InfrastructurePIC = Convert.ToInt64(fc["InfrastructurePIC"]);
                var OtherMembersList = fc["OtherMembers"];
                var rdoMainPic = Convert.ToInt16(fc["MainPIC"]);
                var OtherMembers = string.Join(",", OtherMembersList);
                long SYKI = !string.IsNullOrWhiteSpace(fc["hdnSYKIID"]) ? Convert.ToInt64(fc["hdnSYKIID"]) : 0;
                long A00DTLTBID = !string.IsNullOrWhiteSpace(fc["A00DTLTBID"]) ? Convert.ToInt64(fc["A00DTLTBID"]) : 0;
                //mail
                var ApplicationPICDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(ApplicationPIC);
                var InfrastructurePICDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(InfrastructurePIC);
                var ProjectTitle = _objHomeA00.GetA00ProjectTitle(A00DTLTBID, SYKI);
                var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(userId);
                sendMailToApplicationPICForA00Allocation(ApplicationPICDetails.EMAILID, ProjectTitle, DateTime.Now, requesterDetails.Name, userId.ToString());
                sendMailToInfrastructurePICForA00Allocation(InfrastructurePICDetails.EMAILID, ProjectTitle, DateTime.Now, requesterDetails.Name, userId.ToString());

                //save allocation
                _objHomeA00.SaveA00Allocation(SYKI, ApplicationPIC, InfrastructurePIC, OtherMembers, userId, A00DTLTBID, rdoMainPic);

                return RedirectToAction("ITDivisionApprovalSearch");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveA00Allocation");
                return View();
            }
        }


        private void sendMailForDeficiencyUpdate(string receiver, string project_title, DateTime requested_on, string RequesterName, string RequesterEMPCode)
        {
            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = receiver;
            string strSubject = "Deficiency is updated by A00 owner - " + RequesterName + ", Employee Code - " + RequesterEMPCode;
            string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                             "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Deficiency is updated by A00 owner -  " + RequesterName + " - Employee Code (" + RequesterEMPCode + ")</font></b></td>" +
                             "</tr><tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td width=125 height=22 valign=top>Employee Code</td><td width=389 valign=top>" + RequesterEMPCode + "</td>" +
                             "</tr><tr><td width=125 height=23 valign=top>Employee Name</td><td width=389 valign=top>" + RequesterName + "</td>" +
                             " </tr><tr> " +
                             "<td width=125 valign=top>Project Title</td><td width=389 valign=top>" + project_title + "</td></tr><tr><td width=125 valign=top>Requested on</td>" +
                             "<td width=389 valign=top>" + requested_on.Date.ToString("D") + "</td></tr>" +
                             "<tr><td valign=top colspan=2>Please login <a href='https://portal.honda2wheelersindia.com/SSO'> Employee Portal</a> for approval process.</td></tr>" +
                             "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }

        private void sendMailTODeficiencyOwnerForDeficiencyUpdate(string receiver, string project_title, DateTime requested_on, string RequesterName, string RequesterEMPCode)
        {
            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = receiver;
            string strSubject = "Deficiency is raised by - " + RequesterName + ", Employee Code - " + RequesterEMPCode;
            string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                             "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Deficiency is raised by -  " + RequesterName + " - Employee Code (" + RequesterEMPCode + ")</font></b></td>" +
                             "</tr><tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td width=125 height=22 valign=top>Employee Code</td><td width=389 valign=top>" + RequesterEMPCode + "</td>" +
                             "</tr><tr><td width=125 height=23 valign=top>Employee Name</td><td width=389 valign=top>" + RequesterName + "</td>" +
                             " </tr><tr> " +
                             "<td width=125 valign=top>Project Title</td><td width=389 valign=top>" + project_title + "</td></tr><tr><td width=125 valign=top>Requested on</td>" +
                             "<td width=389 valign=top>" + requested_on.Date.ToString("D") + "</td></tr>" +
                             "<tr><td valign=top colspan=2>Please login <a href='https://portal.honda2wheelersindia.com/SSO'> Employee Portal</a> for further process.</td></tr>" +
                             "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }

        private void sendMailToApplicationPICForA00Allocation(string receiver, string project_title, DateTime requested_on, string RequesterName, string RequesterEMPCode)
        {
            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = receiver;
            string strSubject = "New A00 has been assigned to you by - " + RequesterName + ", Employee Code - " + RequesterEMPCode;
            string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                             "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>New A00 has been assigned to you by -  " + RequesterName + " - Employee Code (" + RequesterEMPCode + ")</font></b></td>" +
                             "</tr><tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td width=125 height=22 valign=top>Employee Code</td><td width=389 valign=top>" + RequesterEMPCode + "</td>" +
                             "</tr><tr><td width=125 height=23 valign=top>Employee Name</td><td width=389 valign=top>" + RequesterName + "</td>" +
                             " </tr><tr> " +
                             "<td width=125 valign=top>Project Title</td><td width=389 valign=top>" + project_title + "</td></tr><tr><td width=125 valign=top>Requested on</td>" +
                             "<td width=389 valign=top>" + requested_on.Date.ToString("D") + "</td></tr>" +
                             "<tr><td valign=top colspan=2>Please login <a href='https://portal.honda2wheelersindia.com/SSO'> Employee Portal</a> for approval process.</td></tr>" +
                             "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }

        private void sendMailToInfrastructurePICForA00Allocation(string receiver, string project_title, DateTime requested_on, string RequesterName, string RequesterEMPCode)
        {
            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = receiver;
            string strSubject = "New A00 has been assigned to you by - " + RequesterName + ", Employee Code - " + RequesterEMPCode;
            string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                             "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>New A00 has been assigned to you by -  " + RequesterName + " - Employee Code (" + RequesterEMPCode + ")</font></b></td>" +
                             "</tr><tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td width=125 height=22 valign=top>Employee Code</td><td width=389 valign=top>" + RequesterEMPCode + "</td>" +
                             "</tr><tr><td width=125 height=23 valign=top>Employee Name</td><td width=389 valign=top>" + RequesterName + "</td>" +
                             " </tr><tr> " +
                             "<td width=125 valign=top>Project Title</td><td width=389 valign=top>" + project_title + "</td></tr><tr><td width=125 valign=top>Requested on</td>" +
                             "<td width=389 valign=top>" + requested_on.Date.ToString("D") + "</td></tr>" +
                             "<tr><td valign=top colspan=2>Please login <a href='https://portal.honda2wheelersindia.com/SSO'> Employee Portal</a> for approval process.</td></tr>" +
                             "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }

        private void sendMailAOORequestApprovedToTeam(string receiver, string project_title, DateTime requested_on, string RequesterName, string RequesterEMPCode)
        {
            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = receiver;// user mail id
            string strSubject = "A00 Request has been submitted by - " + RequesterName + ", Employee Code - " + RequesterEMPCode;
            string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                             "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>A00 Request has been submitted by - " + RequesterName + " - Emp Code (" + RequesterEMPCode + ")</font></b></td>" +
                             "</tr><tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td width=125 height=22 valign=top>Employee Code</td><td width=389 valign=top>" + RequesterEMPCode + "</td>" +
                             "</tr><tr><td width=125 height=23 valign=top>Employee Name</td><td width=389 valign=top>" + RequesterName + "</td>" +
                             " </tr><tr> " +
                             "<td width=125 valign=top>Project Title</td><td width=389 valign=top>" + project_title + "</td></tr><tr><td width=125 valign=top>Requested on</td>" +
                             "<td width=389 valign=top>" + requested_on.Date.ToString("D") + "</td></tr>" +
                             "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }

        public ActionResult ITConfirmation()
        {
            TempData["PageHead"] = "Allocated A00/ IT Confirmation";
            SearchParameterList paramsList = new SearchParameterList();
            A00ViewModel model = new A00ViewModel();
            try
            {
                var paramData = ITConfirmationgetSearchFilterData(new SearchParameterList());
                //model.SearchParams.SYKI = paramData.SYKIID;
                //model.SearchParams.Status = paramData.Status;

                ITConfirmationGetA00SearchData(model, paramData);

                //List<ADORGLEVEL> _opList = _objHomeA00.GetOrgLevelList((long)1, (long)paramData.SYKIID);
                List<A00ADORGLEVELList> _opList = _objHomeA00.GetA00ADORGLEVELList((decimal)paramData.SYKIID)._ADOrgLevelList;
                ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.DivList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.DepList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.SecList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ITConfirmation");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult ITConfirmation(A00ViewModel model)
        {
            try
            {
                TempData["PageHead"] = "Allocated A00/ IT Confirmation";
                SearchParameterList paramlist = new SearchParameterList();

                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");

                var sykiId = 0;
                if (!string.IsNullOrEmpty(Request.Form["SearchParams.SYKI"].ToString()))
                {
                    sykiId = Convert.ToInt32(Request.Form["SearchParams.SYKI"].ToString());
                }
                paramlist.SYKIID = sykiId;
                paramlist.ECode = Request.Form["SearchParams.ECode"].ToString();
                paramlist.EmpName = Request.Form["SearchParams.EmpName"].ToString();
                //paramlist.StatusId = Convert.ToInt16(Request.Query["SearchParams.Status"].ToString());
                paramlist.StatusId = Convert.ToInt16(Request.Form["SearchParams.Status"].ToString());

                paramlist.OPERATIONID = Request.Form["SearchParams.OPERATIONID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.OPERATIONID"]) : 0;
                paramlist.DIVISIONID = Request.Form["SearchParams.DIVISIONID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.DIVISIONID"]) : 0;
                paramlist.DEPARTMENTID = Request.Form["SearchParams.DEPARTMENTID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.DEPARTMENTID"]) : 0;
                paramlist.SECTIONID = Request.Form["SearchParams.SECTIONID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.SECTIONID"]) : 0;

                #region Block-1
                var userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

                model.SearchParams = new SearchParameterList();
                model.SearchParams.StatusId = paramlist.StatusId;
                var empCode = string.IsNullOrEmpty(Request.Form["SearchParams.ECode"].ToString()) ? 0 : Convert.ToInt64(Request.Form["SearchParams.ECode"].ToString());
                var empName = Request.Form["SearchParams.EmpName"].ToString();
                model.SearchParams.ECode = Request.Form["SearchParams.ECode"].ToString();
                model.SearchParams.EmpName = empName;
                model.SearchParams.SYKIID = sykiId;
                #endregion

                var paramData = ITConfirmationgetSearchFilterData(paramlist);

                model.SearchParams.SYKI = paramData.SYKI;
                model.SearchParams.Status = paramData.Status;

                ITConfirmationGetA00SearchData(model, paramData);

                #region added by kiran

                //List<ADORGLEVEL>

                //List<ADORGLEVEL> _opList = _objHomeA00.GetOrgLevelList((long)1, sykiId);
                List<A00ADORGLEVELList> _opList = _objHomeA00.GetA00ADORGLEVELList((decimal)sykiId)._ADOrgLevelList;
                ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP", paramlist.OPERATIONID);

                List<SearchParameterList> _divList = _objHomeA00.BindDivision(paramlist.OPERATIONID, sykiId);// _objHomeA00.GetOrgLevelList((long)2);
                ViewBag.DivList = new SelectList(_divList, "DIVISIONID", "DIVISION", paramlist.DIVISIONID);

                List<SearchParameterList> _depList = _objHomeA00.BindDepartment(paramlist.DIVISIONID, 0, sykiId);// _objHomeA00.GetOrgLevelList((long)3);
                ViewBag.DepList = new SelectList(_depList, "DEPARTMENTID", "DEPARTMENT", paramlist.DEPARTMENTID);


                List<SearchParameterList> _secList = _objHomeA00.BindSection(paramlist.DEPARTMENTID, 0, 0, sykiId); //_objHomeA00.GetOrgLevelList((long)4);
                ViewBag.SecList = new SelectList(_secList, "SECTIONID", "SECTION", paramlist.SECTIONID);

                paramlist.OPERATIONID = employeeDetails._OpId;
                paramlist.OPERATION = employeeDetails._OpDesc;

                paramlist.DIVISIONID = employeeDetails._DivId;
                paramlist.DIVISION = employeeDetails._DivDesc;

                paramlist.DEPARTMENTID = employeeDetails._DepId;
                paramlist.DEPARTMENT = employeeDetails._DepDesc;

                paramlist.SECTIONID = employeeDetails._SecId;
                paramlist.SECTION = employeeDetails._SecDescrip;
                #endregion
                return View(model);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ITConfirmation");
                return View("Error");
            }

        }

        public JsonResult GetA00ITConfirmationRequestForApproval(short StatusId = 0)
        {
            A00ViewModel model = new A00ViewModel();
            try
            {
                SearchParameterList paramsData = new SearchParameterList();
                paramsData.StatusId = StatusId;
                var paramData = ITConfirmationgetSearchFilterData(paramsData);

                paramData.OPERATIONID = 0;
                paramData.DIVISIONID = 0;
                paramData.DEPARTMENTID = 0;
                paramData.SECTIONID = 0;
                paramData.SYKIID = 0;
                paramData.EmpName = string.Empty;
                paramData.ECode = string.Empty;
                paramData.StatusId = StatusId;

                ITConfirmationApprovalGetA00SearchData(model, paramData);
                return Json(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetA00ITConfirmationRequestForApproval");
            }
            return Json(model.ResultList);
        }
        public ActionResult ITConfirmationApprovalRequest(short status = 0)
        {
            TempData["PageHead"] = "IT Confirmation Approval";
            SearchParameterList paramsList = new SearchParameterList();
            A00ViewModel model = new A00ViewModel();
            try
            {
                SearchParameterList paramsData = new SearchParameterList();
                paramsData.StatusId = status;
                var paramData = ITConfirmationgetSearchFilterData(paramsData);

                ITConfirmationApprovalGetA00SearchData(model, paramData);

                //List<ADORGLEVEL> _opList = _objHomeA00.GetOrgLevelList((long)1, (long)paramData.SYKIID);
                List<A00ADORGLEVELList> _opList = _objHomeA00.GetA00ADORGLEVELList(paramData.SYKIID)._ADOrgLevelList;
                ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.DivList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.DepList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.SecList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ITConfirmationApprovalRequest");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult ITConfirmationApprovalRequest(A00ViewModel model)
        {
            try
            {


                TempData["PageHead"] = "IT Confirmation Approval";
                SearchParameterList paramlist = new SearchParameterList();

                //Employee_Details employeeDetails = (Employee_Details)Session["Employee"];
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");

                var sykiId = 0;
                if (!string.IsNullOrEmpty(Request.Form["SearchParams.SYKI"].ToString()))
                {
                    sykiId = Convert.ToInt32(Request.Form["SearchParams.SYKI"].ToString());
                }
                paramlist.SYKIID = sykiId;
                paramlist.ECode = Request.Form["SearchParams.ECode"].ToString();
                paramlist.EmpName = Request.Form["SearchParams.EmpName"].ToString();
                //paramlist.StatusId = Convert.ToInt16(Request.Query["SearchParams.Status"].ToString());
                paramlist.StatusId = Convert.ToInt16(Request.Form["SearchParams.Status"].ToString());

                paramlist.OPERATIONID = Request.Form["SearchParams.OPERATIONID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.OPERATIONID"]) : 0;
                paramlist.DIVISIONID = Request.Form["SearchParams.DIVISIONID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.DIVISIONID"]) : 0;
                paramlist.DEPARTMENTID = Request.Form["SearchParams.DEPARTMENTID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.DEPARTMENTID"]) : 0;
                paramlist.SECTIONID = Request.Form["SearchParams.SECTIONID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.SECTIONID"]) : 0;


                #region Block-1
                var userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

                model.SearchParams = new SearchParameterList();
                model.SearchParams.StatusId = paramlist.StatusId;
                var empCode = string.IsNullOrEmpty(Request.Form["SearchParams.ECode"].ToString()) ? 0 : Convert.ToInt64(Request.Form["SearchParams.ECode"].ToString());
                var empName = Request.Form["SearchParams.EmpName"].ToString();
                model.SearchParams.ECode = Request.Form["SearchParams.ECode"].ToString();
                model.SearchParams.EmpName = empName;
                model.SearchParams.SYKIID = sykiId;
                #endregion

                var paramData = ITConfirmationgetSearchFilterData(paramlist);

                model.SearchParams.SYKI = paramData.SYKI;
                model.SearchParams.Status = paramData.Status;

                ITConfirmationApprovalGetA00SearchData(model, paramData);

                #region added by kiran

                //List<ADORGLEVEL>

                //List<ADORGLEVEL> _opList = _objHomeA00.GetOrgLevelList((long)1, (long)paramData.SYKIID);
                List<A00ADORGLEVELList> _opList = _objHomeA00.GetA00ADORGLEVELList(paramData.SYKIID)._ADOrgLevelList;
                ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP", paramlist.OPERATIONID);

                List<SearchParameterList> _divList = _objHomeA00.BindDivision(paramlist.OPERATIONID, (long)paramData.SYKIID);// _objHomeA00.GetOrgLevelList((long)2);
                ViewBag.DivList = new SelectList(_divList, "DIVISIONID", "DIVISION", paramlist.DIVISIONID);

                List<SearchParameterList> _depList = _objHomeA00.BindDepartment(paramlist.DIVISIONID, 0, (long)paramData.SYKIID);// _objHomeA00.GetOrgLevelList((long)3);
                ViewBag.DepList = new SelectList(_depList, "DEPARTMENTID", "DEPARTMENT", paramlist.DEPARTMENTID);


                List<SearchParameterList> _secList = _objHomeA00.BindSection(paramlist.DEPARTMENTID, 0, 0, (long)paramData.SYKIID); //_objHomeA00.GetOrgLevelList((long)4);
                ViewBag.SecList = new SelectList(_secList, "SECTIONID", "SECTION", paramlist.SECTIONID);

                paramlist.OPERATIONID = employeeDetails._OpId;
                paramlist.OPERATION = employeeDetails._OpDesc;

                paramlist.DIVISIONID = employeeDetails._DivId;
                paramlist.DIVISION = employeeDetails._DivDesc;

                paramlist.DEPARTMENTID = employeeDetails._DepId;
                paramlist.DEPARTMENT = employeeDetails._DepDesc;

                paramlist.SECTIONID = employeeDetails._SecId;
                paramlist.SECTION = employeeDetails._SecDescrip;
                #endregion
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ITConfirmationApprovalRequest");
                return View("Error");
            }
        }

        private SearchParameterList ITConfirmationgetSearchFilterData(SearchParameterList paramsList)
        {
            List<SearchParameterList> list = new List<SearchParameterList>();
            SearchParameterList data = new SearchParameterList();

            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

            #region Bind Ki drop down
            List<SearchParameterList> kiData = new List<SearchParameterList>();
            kiData.Add(new SearchParameterList() { SYKIID = 0, SYKI = "Select" });
            var kiData1 = _objHomeA00.GetA00SYKIList();
            var activeKi = kiData1._SYKIList.Where(x => x.ACTIVE == 1).FirstOrDefault();
            var kii = kiData1._SYKIList.OrderByDescending(x => x.SYKIID).Select(x => new SearchParameterList { SYKIID = x.SYKIID, SYKI = x.KICODE }).ToList();
            kiData.AddRange(kii);

            //Commented on 10-July-2021
            //var selectedKi = (paramsList.SYKIID == 0 || paramsList.SYKIID == null) ? activeKi.SYKIID : (decimal)paramsList.SYKIID;

            //Change start on 10-July-2021
            var selectedKi = (paramsList.SYKIID == null) ? activeKi.SYKIID : (decimal)paramsList.SYKIID;
            //Change end on 10-July-2021

            ViewBag.SYKIData = new SelectList(kiData, "SYKIID", "SYKI", selectedKi);
            #endregion

            #region Bind status drop down
            data.StatusId = paramsList.StatusId;
            var statusList = new List<SearchParameterList>() {
            new SearchParameterList {
                StatusId = 0,
                Status = "Pending"
            },
            new SearchParameterList {
                StatusId = 1,
                Status = "Completed"
            },
            new SearchParameterList {
                StatusId = 2,
                Status = "All"
            }
            //Change start on 22-July-2021
            ,
            new SearchParameterList {
                StatusId = 3,
                //Status = "Converted for CR"
                Status = "Sent back (Convert to CR)"
            }
            //Change start on 22-July-2021
            };

            ViewBag.StatusList = new SelectList(statusList.OrderBy(i => i.Status).ToList(), "StatusId", "Status", paramsList.StatusId);//changed by eshant 4-jul-22 for sorting
            #endregion

            data.ECode = paramsList.ECode;
            data.EmpName = paramsList.EmpName;
            data.OPERATIONID = paramsList.OPERATIONID;
            data.DIVISIONID = paramsList.DIVISIONID;
            data.SECTIONID = paramsList.SECTIONID;
            data.DEPARTMENTID = paramsList.DEPARTMENTID;
            data.SYKIID = selectedKi;

            #region Bind Login Employee Details
            _objA00SearchModel.ADEMPCODE = userId;
            _objA00SearchModel.SYKI = selectedKi;

            //Employee_Details employeeDetails = (Employee_Details)Session["Employee"];
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");

            _objA00SearchModel.ADEMPCODE = userId;
            _objA00SearchModel.SYKIID = selectedKi;

            var vwITApproval = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS_ITApproval(_objA00SearchModel);

            var adorglevel = _objHomeA00.GetA00_IT_ADORGLEVELList(_objA00SearchModel);
            #endregion
            #region Check IT-Division Head
            var empDetails1 = vwITApproval._A00_VW_ASSOCIATELVLDETAILS_FULL.Join(adorglevel._ADOrgLevelList.AsEnumerable(),
               b => b.DIVISIONID, c => c.ADORGLEVELID, (b, c) => new { b, c }).FirstOrDefault();
            if (empDetails1 != null)
            {
                ViewBag.isITDivisionHead = true;
                data.isITDivisionHead = "Y";
            }
            else
            {
                ViewBag.isITDivisionHead = false;
                data.isITDivisionHead = "N";
            }
            #endregion

            list.Add(data);
            ViewBag.List = list;
            return data;
        }

        private void ITConfirmationGetA00SearchData(A00ViewModel model, SearchParameterList paramsData)
        {
            List<SearchResultList> empCodeList = new List<SearchResultList>();

            var userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

            model.SearchParams = new SearchParameterList();
            model.SearchParams.OPERATIONID = paramsData.OPERATIONID;
            model.SearchParams.DIVISIONID = paramsData.DIVISIONID;
            model.SearchParams.DEPARTMENTID = paramsData.DEPARTMENTID;
            model.SearchParams.SECTIONID = paramsData.SECTIONID;
            var sykiId = paramsData.SYKIID;//0;

            // SearchParams.StatusId
            model.SearchParams.StatusId = paramsData.StatusId;
            var empCode = string.IsNullOrEmpty(paramsData.ECode) ? 0 : Convert.ToInt64(paramsData.ECode);
            var empName = paramsData.EmpName;
            model.SearchParams.ECode = paramsData.ECode;
            model.SearchParams.EmpName = empName;
            model.SearchParams.SYKIID = sykiId;

            model.SearchParams.SYKI = paramsData.SYKI;
            model.SearchParams.Status = paramsData.Status;

            var status = model.SearchParams.StatusId;

            var kiData1 = _objHomeA00.GetA00SYKIList();
            var activeKi = kiData1._SYKIList.Where(x => x.ACTIVE == 1).FirstOrDefault();

            bool isNormalUser = false; bool isDeptHead = false, isCoOrdHead = false, isDivHead = false, isExeCoOrdHead = false, isOperatingHead = false;
            _objA00SearchModel.ADEMPCODE = userId;
            _objA00SearchModel.SYKI = paramsData.SYKIID > 0 ? (decimal)paramsData.SYKIID : activeKi.SYKIID;
            var MainPICOperation = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel);
            _objA00SearchModel.OPERATIONID = MainPICOperation.OPERATIONID;
            var userData = _objHomeA00.getUserRole(userId, _objA00SearchModel.SYKI);
            isDeptHead = userData.isDeptHead;
            isCoOrdHead = userData.isCoOrdHead;
            isDivHead = userData.isDivHead;
            isExeCoOrdHead = userData.isExeCoOrdHead;
            isOperatingHead = userData.isOperatingHead;
            var ITConfApprovedStatus = new List<short?> { };
            var ITConfApprovedStatus1 = new List<short?> { };
            var ITConfApprovedStatus2 = new List<short?> { };
            var ITConfApprovedStatus3 = new List<short?> { };
            var ITConfApprovedStatus4 = new List<short?> { };
            if (isDeptHead)
            {
                ITConfApprovedStatus = new List<short?> { 3, 4, 5, 6, 7, 8 };
                ITConfApprovedStatus1 = new List<short?> { 1 };
                ITConfApprovedStatus2 = new List<short?> { 2 };
                ITConfApprovedStatus3 = new List<short?> { null };
                ITConfApprovedStatus4 = new List<short?> { 2, 3, 4, 5, 6, 7, 8 };
            }
            else if (isCoOrdHead)
            {
                ITConfApprovedStatus = new List<short?> { 4, 5, 6, 7, 8 };
                ITConfApprovedStatus1 = new List<short?> { 3 };
                ITConfApprovedStatus2 = new List<short?> { null };
            }
            else if (isDivHead)
            {
                ITConfApprovedStatus = new List<short?> { 6, 7, 8 };
                ITConfApprovedStatus1 = new List<short?> { 3, 4 };
                ITConfApprovedStatus2 = new List<short?> { 5 };
                ITConfApprovedStatus3 = new List<short?> { null };
            }
            else if (isExeCoOrdHead)
            {
                ITConfApprovedStatus = new List<short?> { 7, 8 };
                ITConfApprovedStatus1 = new List<short?> { 5, 6 };
                ITConfApprovedStatus2 = new List<short?> { null };
            }
            else if (isOperatingHead)
            {
                ITConfApprovedStatus = new List<short?> { 8 };
                ITConfApprovedStatus1 = new List<short?> { 6, 7 };
                ITConfApprovedStatus2 = new List<short?> { null };
            }
            else
            {
                isNormalUser = true;
                ITConfApprovedStatus = new List<short?> { 1, 2, 3, 4, 5, 6, 7, 8 };
            }


            #region Filter Query
            if (status == 0)
            {
                var resultList = _objHomeA00.GetA00APPROVALFullTable()._A00APPROVALList.
                    Where(x => x.OHAPPDATE != null).AsEnumerable().
                   Join(_objHomeA00.GetA00DtlFullTable()._A00DtlViewModelList,
                   a => a.A00DTLTBID, b => b.A00DTLTBID, (a, b) => new { A00APPROVAL = a, A00DTLTB = b }).
                   Join(_objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE, c => c.A00DTLTB.ADDEDBY, d => d.ADEMPCODE, (c, d) => new { c, d }).
                   Join(_objHomeA00.GetA00AllocationList()._A00AllocationList, e => e.c.A00DTLTB.A00DTLTBID, al => al.A00ID, (e, al) => new { e, al })

                   .Where(f =>
                   (f.al.ApplicationPICECode == userId || f.al.InfraPICECode == userId)
                && (!_objHomeA00.GetA00ItConfirmationList()._A00ITConfirmationList.Where(x => x.Status != null).Select(x => x.A00ALLOCATIONID).Contains(f.al.A00AllocationID))
                && (string.IsNullOrEmpty(empName) ? true : ((f.e.d.FIRSTNAME + " " + f.e.d.LASTNAME).Contains(empName)))
                && (f.e.c.A00DTLTB.ADDEDBY == (empCode == 0 ? f.e.c.A00DTLTB.ADDEDBY : empCode)

                //22-Sept-2021 change start
                //&& (f.e.c.A00DTLTB.STATUSCD == 105)
                && ((status == 0 || status == 1) ? f.e.c.A00DTLTB.STATUSCD == 105 : (status == 3 ? f.e.c.A00DTLTB.STATUSCD == 125 : (f.e.c.A00DTLTB.STATUSCD == 125 || f.e.c.A00DTLTB.STATUSCD == 105)))
                //22-Sept-2021 change end

                && (f.e.c.A00DTLTB.SYKIID == (sykiId == 0 ? f.e.c.A00DTLTB.SYKIID : sykiId))
                && ((paramsData.OPERATIONID == 0 || paramsData.OPERATIONID == null) || f.e.c.A00DTLTB.OPERATION == paramsData.OPERATIONID)
                && ((paramsData.DIVISIONID == 0 || paramsData.DIVISIONID == null) || f.e.c.A00DTLTB.DIVISION == paramsData.DIVISIONID)
                && ((paramsData.DEPARTMENTID == 0 || paramsData.DEPARTMENTID == null) || f.e.c.A00DTLTB.DEPARTMENT == paramsData.DEPARTMENTID)
                && ((paramsData.SECTIONID == 0 || paramsData.SECTIONID == null) || f.e.c.A00DTLTB.SECTION == paramsData.SECTIONID))).
               Select(s1 => new SearchResultList
               {
                   A00DTLTBID = s1.e.c.A00DTLTB.A00DTLTBID,
                   SYKIID = s1.e.c.A00DTLTB.SYKIID,
                   KICODE = kiData1._SYKIList.Where(x1 => x1.SYKIID == s1.e.c.A00DTLTB.SYKIID).Select(x1 => x1.KICODE).FirstOrDefault(),
                   OPERATION = _objHomeA00.Getoperationdetails(s1.e.c.A00DTLTB.ADDEDBY, (int)s1.e.c.A00DTLTB.SYKIID),
                   ProjectCode = string.Empty,
                   ProjectTitle = s1.e.c.A00DTLTB.PRJCTTLE,
                   A00ApprovedOn = s1.e.c.A00APPROVAL.OHAPPDATE,
                   ApprovedOn = (s1.al.AllocatedDate),//A00 Assigned Date
                   ITConfirmationDate = null,
                   //DaysDiffBetA00ApprovalAndITConfirmation = (s1.itconf.ITCONFIRMATIONApprovalDate != null ? (s1.itconf.ITCONFIRMATIONApprovalDate- s1.e.c.A00APPROVAL.OHAPPDATE) : (DateTime.Now- s1.e.c.A00APPROVAL.OHAPPDATE)),
                   ECode = s1.e.c.A00DTLTB.ADDEDBY,
                   EmpName = s1.e.d.FIRSTNAME + " " + s1.e.d.LASTNAME,
                   RequestDate = s1.e.c.A00DTLTB.DATEADDEDID,
                   ApprovedBy = _objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE.Where(x1 => x1.ADEMPCODE == s1.al.AllocatedBY).Select(x1 => x1.FIRSTNAME + " " + x1.LASTNAME).FirstOrDefault(),
                   UserId = userId,
                   MainPIC = _objHomeA00.GetA00MainPIC(s1.e.c.A00DTLTB.A00DTLTBID, s1.e.c.A00DTLTB.SYKIID),
                   ApproverUserId = null,
                   ITConfirmationStatus = null,
                   Main_PIC_Name = _objHomeA00.GetAllocationDetails(s1.e.c.A00DTLTB.A00DTLTBID, s1.e.c.A00DTLTB.SYKIID).Select(x => x.MainPic).FirstOrDefault(),
                   //IsDeficiencyClosed = _objHomeA00.IsA00DeficiencyClosed(s1.c.A00DTLTB.A00DTLTBID, s1.c.A00DTLTB.SYKIID)
               }).ToList();
                model.ResultList = resultList.OrderByDescending(x => x.RequestDate).ThenBy(x => x.ProjectTitle).ToList();
            }
            else
            {
                var resultList = _objHomeA00.GetA00APPROVALFullTable()._A00APPROVALList.
                    Where(x => x.OHAPPDATE != null).AsEnumerable().
                   Join(_objHomeA00.GetA00DtlFullTable()._A00DtlViewModelList,
                   a => a.A00DTLTBID, b => b.A00DTLTBID, (a, b) => new { A00APPROVAL = a, A00DTLTB = b }).
                   Join(_objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE, c => c.A00DTLTB.ADDEDBY, d => d.ADEMPCODE, (c, d) => new { c, d }).
                   Join(_objHomeA00.GetA00AllocationList()._A00AllocationList, e => e.c.A00DTLTB.A00DTLTBID, al => al.A00ID, (e, al) => new { e, al }).
                   Join(_objHomeA00.GetA00ItConfirmationList()._A00ITConfirmationList.Where(x => x.Status != null), g => g.al.A00AllocationID, itconf => itconf.A00ALLOCATIONID, (g, itconf) => new { g, itconf = itconf })
                   .Where(f =>
                   (string.IsNullOrEmpty(empName) ? true : ((f.g.e.d.FIRSTNAME + " " + f.g.e.d.LASTNAME).Contains(empName)))
                && (f.g.e.c.A00DTLTB.ADDEDBY == (empCode == 0 ? f.g.e.c.A00DTLTB.ADDEDBY : empCode)

                //22-Sept-2021 change start
                //&& (f.g.e.c.A00DTLTB.STATUSCD == 105)
                && ((status == 0 || status == 1) ? f.g.e.c.A00DTLTB.STATUSCD == 105 : (status == 3 ? f.g.e.c.A00DTLTB.STATUSCD == 125 : (f.g.e.c.A00DTLTB.STATUSCD == 125 || f.g.e.c.A00DTLTB.STATUSCD == 105)))
                //22-Sept-2021 change end

                //&& (status == 0 ? (f.itconf.Status == 2 ? ITConfApprovedStatus1.Contains(f.itconf.Status) : ITConfApprovedStatus2.Contains(f.itconf.Status)) : ITConfApprovedStatus.Contains(f.itconf.Status))
                //&& (status == 0 ? !ITConfApprovedStatus.Contains(f.itconf.Status) : ITConfApprovedStatus.Contains(f.itconf.Status))

                && (isDeptHead ? (status == 0 ? ((f.itconf.Status == 1 && userId == f.itconf.ITConfirmationLatestHistoryVM.ITConfirmationSubmittedTo) ? ITConfApprovedStatus1.Contains(f.itconf.Status) : ((f.itconf.Status == 2 && userId == f.itconf.ITConfirmationLatestHistoryVM.ITConfirmationSubmittedTo) ? ITConfApprovedStatus2.Contains(f.itconf.Status) : ITConfApprovedStatus3.Contains(f.itconf.Status))) : ((f.itconf.Status == 2 && userId == f.itconf.ITConfirmationLatestHistoryVM.ActionBy) ? (ITConfApprovedStatus4.Contains(f.itconf.Status)) : (ITConfApprovedStatus.Contains(f.itconf.Status)))) : true)
                && (isCoOrdHead ? (status == 0 ? ((f.itconf.Status == 3 && userId == f.itconf.ITConfirmationLatestHistoryVM.ITConfirmationSubmittedTo) ? ITConfApprovedStatus1.Contains(f.itconf.Status) : ITConfApprovedStatus2.Contains(f.itconf.Status)) : (ITConfApprovedStatus.Contains(f.itconf.Status))) : true)
                && (isDivHead ? (status == 0 ? (((f.itconf.Status == 3 || f.itconf.Status == 4) && userId == f.itconf.ITConfirmationLatestHistoryVM.ITConfirmationSubmittedTo) ? ITConfApprovedStatus1.Contains(f.itconf.Status) : ((f.itconf.Status == 5 && userId == f.itconf.ITConfirmationLatestHistoryVM.ITConfirmationSubmittedTo) ? ITConfApprovedStatus2.Contains(f.itconf.Status) : ITConfApprovedStatus3.Contains(f.itconf.Status))) : (ITConfApprovedStatus.Contains(f.itconf.Status))) : true)
                && (isExeCoOrdHead ? (status == 0 ? (((f.itconf.Status == 5 || f.itconf.Status == 6) && userId == f.itconf.ITConfirmationLatestHistoryVM.ITConfirmationSubmittedTo) ? ITConfApprovedStatus1.Contains(f.itconf.Status) : ITConfApprovedStatus2.Contains(f.itconf.Status)) : (ITConfApprovedStatus.Contains(f.itconf.Status))) : true)
                && (isOperatingHead ? (status == 0 ? ((f.itconf.Status == 6 || f.itconf.Status == 7 && userId == f.itconf.ITConfirmationLatestHistoryVM.ITConfirmationSubmittedTo) ? ITConfApprovedStatus1.Contains(f.itconf.Status) : ITConfApprovedStatus2.Contains(f.itconf.Status)) : (ITConfApprovedStatus.Contains(f.itconf.Status))) : true)
                && (status == 3 ? (f.itconf.ITConfirmationFilledBy == userId && f.itconf.Status != 8) : true)//To get my request
                && (f.g.e.c.A00DTLTB.SYKIID == (sykiId == 0 ? f.g.e.c.A00DTLTB.SYKIID : sykiId))
                && ((paramsData.OPERATIONID == 0 || paramsData.OPERATIONID == null) || f.g.e.c.A00DTLTB.OPERATION == paramsData.OPERATIONID)
                && ((paramsData.DIVISIONID == 0 || paramsData.DIVISIONID == null) || f.g.e.c.A00DTLTB.DIVISION == paramsData.DIVISIONID)
                && ((paramsData.DEPARTMENTID == 0 || paramsData.DEPARTMENTID == null) || f.g.e.c.A00DTLTB.DEPARTMENT == paramsData.DEPARTMENTID)
                && ((paramsData.SECTIONID == 0 || paramsData.SECTIONID == null) || f.g.e.c.A00DTLTB.SECTION == paramsData.SECTIONID))).
               Select(s1 => new SearchResultList
               {
                   A00DTLTBID = s1.g.e.c.A00DTLTB.A00DTLTBID,
                   SYKIID = s1.g.e.c.A00DTLTB.SYKIID,
                   KICODE = kiData1._SYKIList.Where(x1 => x1.SYKIID == s1.g.e.c.A00DTLTB.SYKIID).Select(x1 => x1.KICODE).FirstOrDefault(),
                   OPERATION = _objHomeA00.Getoperationdetails(s1.g.e.c.A00DTLTB.ADDEDBY, (int)s1.g.e.c.A00DTLTB.SYKIID),
                   ProjectCode = s1.itconf.ProjectCode,
                   ProjectTitle = s1.g.e.c.A00DTLTB.PRJCTTLE,
                   A00ApprovedOn = s1.g.e.c.A00APPROVAL.OHAPPDATE,
                   ApprovedOn = (s1.g.al.AllocatedDate),//A00 Assigned Date
                   ITConfirmationDate = _objHomeA00.GetA00ItConfirmationHistoryDetails((decimal)s1.itconf.ITCONFIRMATIONID).Status == 8 ? s1.itconf.ITCONFIRMATIONApprovalDate : null,
                   //DaysDiffBetA00ApprovalAndITConfirmation = (s1.itconf.ITCONFIRMATIONApprovalDate != null ? (s1.itconf.ITCONFIRMATIONApprovalDate- s1.g.e.c.A00APPROVAL.OHAPPDATE) : (DateTime.Now- s1.g.e.c.A00APPROVAL.OHAPPDATE)),
                   ECode = s1.g.e.c.A00DTLTB.ADDEDBY,
                   EmpName = s1.g.e.d.FIRSTNAME + " " + s1.g.e.d.LASTNAME,
                   RequestDate = s1.g.e.c.A00DTLTB.DATEADDEDID,
                   ApprovedBy = _objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE.Where(x1 => x1.ADEMPCODE == s1.g.al.AllocatedBY).Select(x1 => x1.FIRSTNAME + " " + x1.LASTNAME).FirstOrDefault(),
                   UserId = userId,
                   MainPIC = _objHomeA00.GetA00MainPIC(s1.g.e.c.A00DTLTB.A00DTLTBID, s1.g.e.c.A00DTLTB.SYKIID),
                   ApproverUserId = _objHomeA00.GetA00ItConfirmationHistoryDetails((decimal)s1.itconf.ITCONFIRMATIONID).ITConfirmationSubmittedTo,
                   ITConfirmationStatus = _objHomeA00.GetA00ItConfirmationHistoryDetails((decimal)s1.itconf.ITCONFIRMATIONID).Status,
                   //IsDeficiencyClosed = _objHomeA00.IsA00DeficiencyClosed(s1.c.A00DTLTB.A00DTLTBID, s1.c.A00DTLTB.SYKIID)
               }).ToList();
                model.ResultList = resultList.OrderByDescending(x => x.RequestDate).ThenBy(x => x.ProjectTitle).ToList();
            }

            #endregion
        }
        private void ITConfirmationApprovalGetA00SearchData(A00ViewModel model, SearchParameterList paramsData)
        {
            List<SearchResultList> empCodeList = new List<SearchResultList>();

            var userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

            model.SearchParams = new SearchParameterList();
            model.SearchParams.OPERATIONID = paramsData.OPERATIONID;
            model.SearchParams.DIVISIONID = paramsData.DIVISIONID;
            model.SearchParams.DEPARTMENTID = paramsData.DEPARTMENTID;
            model.SearchParams.SECTIONID = paramsData.SECTIONID;
            var sykiId = paramsData.SYKIID;//0;

            // SearchParams.StatusId
            model.SearchParams.StatusId = paramsData.StatusId;
            var empCode = string.IsNullOrEmpty(paramsData.ECode) ? 0 : Convert.ToInt64(paramsData.ECode);
            var empName = paramsData.EmpName;
            model.SearchParams.ECode = paramsData.ECode;
            model.SearchParams.EmpName = empName;
            model.SearchParams.SYKIID = sykiId;

            model.SearchParams.SYKI = paramsData.SYKI;
            model.SearchParams.Status = paramsData.Status;

            var status = model.SearchParams.StatusId;

            var kiData1 = _objHomeA00.GetA00SYKIList();
            var activeKi = kiData1._SYKIList.Where(x => x.ACTIVE == 1).FirstOrDefault();

            bool isNormalUser = false; bool isDeptHead = false, isCoOrdHead = false, isDivHead = false, isExeCoOrdHead = false, isOperatingHead = false;
            _objA00SearchModel.ADEMPCODE = userId;
            _objA00SearchModel.SYKI = sykiId > 0 ? (decimal)sykiId : activeKi.SYKIID;
            var MainPICOperation = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel);
            _objA00SearchModel.OPERATIONID = MainPICOperation.OPERATIONID;
            var userData = _objHomeA00.getUserRole(userId, _objA00SearchModel.SYKI);
            isDeptHead = userData.isDeptHead;
            isCoOrdHead = userData.isCoOrdHead;
            isDivHead = userData.isDivHead;
            isExeCoOrdHead = userData.isExeCoOrdHead;
            isOperatingHead = userData.isOperatingHead;
            var ITConfApprovedStatus = new List<short?> { };
            var ITConfApprovedStatus1 = new List<short?> { };
            var ITConfApprovedStatus2 = new List<short?> { };
            var ITConfApprovedStatus3 = new List<short?> { };
            var ITConfApprovedStatus4 = new List<short?> { };
            if (isDeptHead)
            {
                ITConfApprovedStatus = new List<short?> { 3, 4, 5, 6, 7, 8, 9 };
                ITConfApprovedStatus1 = new List<short?> { 1 };
                ITConfApprovedStatus2 = new List<short?> { 2 };
                ITConfApprovedStatus3 = new List<short?> { null };
                ITConfApprovedStatus4 = new List<short?> { 2, 3, 4, 5, 6, 7, 8, 9 };
            }
            else if (isCoOrdHead)
            {
                ITConfApprovedStatus = new List<short?> { 4, 5, 6, 7, 8, 9 };
                ITConfApprovedStatus1 = new List<short?> { 3 };
                ITConfApprovedStatus2 = new List<short?> { null };
            }
            else if (isDivHead)
            {
                ITConfApprovedStatus = new List<short?> { 5, 6, 7, 8, 9 };
                ITConfApprovedStatus1 = new List<short?> { 3, 4 };
                ITConfApprovedStatus2 = new List<short?> { 5 };
                ITConfApprovedStatus3 = new List<short?> { null };
            }
            else if (isExeCoOrdHead)
            {
                ITConfApprovedStatus = new List<short?> { 7, 8, 9 };
                ITConfApprovedStatus1 = new List<short?> { 5, 6 };
                ITConfApprovedStatus2 = new List<short?> { null };
            }
            else if (isOperatingHead)
            {
                ITConfApprovedStatus = new List<short?> { 8, 9 };
                //ITConfApprovedStatus1 = new List<short?> { 6, 7 };// Changes by TTL CR7748 - SR115431
                ITConfApprovedStatus1 = new List<short?> { 5, 6, 7 };// Changes by TTL CR7748 - SR115431
                ITConfApprovedStatus2 = new List<short?> { null };
            }
            else
            {
                isNormalUser = true;
                ITConfApprovedStatus = new List<short?> { 1, 2, 3, 4, 5, 6, 7, 8 };
                ITConfApprovedStatus1 = new List<short?> { 9 };
            }


            #region Filter Query
            var resultList = _objHomeA00.GetA00APPROVALFullTable()._A00APPROVALList.
                Where(x => x.OHAPPDATE != null).AsEnumerable().
               Join(_objHomeA00.GetA00DtlFullTable()._A00DtlViewModelList,
               a => a.A00DTLTBID, b => b.A00DTLTBID, (a, b) => new { A00APPROVAL = a, A00DTLTB = b }).
               Join(_objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE, c => c.A00DTLTB.ADDEDBY, d => d.ADEMPCODE, (c, d) => new { c, d }).
               Join(_objHomeA00.GetA00AllocationList()._A00AllocationList, e => e.c.A00DTLTB.A00DTLTBID, al => al.A00ID, (e, al) => new { e, al }).
               Join(_objHomeA00.GetA00ItConfirmationList()._A00ITConfirmationList.Where(x => x.Status != null), g => g.al.A00AllocationID, itconf => itconf.A00ALLOCATIONID, (g, itconf) => new { g, itconf = itconf })
               .Where(f =>
               (string.IsNullOrEmpty(empName) ? true : ((f.g.e.d.FIRSTNAME + " " + f.g.e.d.LASTNAME).Contains(empName)))
            && (f.g.e.c.A00DTLTB.ADDEDBY == (empCode == 0 ? f.g.e.c.A00DTLTB.ADDEDBY : empCode)
            //22-Sept-2021 change start
            //&& (f.g.e.c.A00DTLTB.STATUSCD == 105)
            && ((status == 0 || status == 1) ? f.g.e.c.A00DTLTB.STATUSCD == 105 : (status == 3 ? f.g.e.c.A00DTLTB.STATUSCD == 125 : (f.g.e.c.A00DTLTB.STATUSCD == 125 || f.g.e.c.A00DTLTB.STATUSCD == 105)))
            //22-Sept-2021 change end

            && (isNormalUser ? (status == 0 ? ((f.itconf.Status == 9 && userId == f.itconf.ITConfirmationLatestHistoryVM.ITConfirmationSubmittedTo) ? ITConfApprovedStatus1.Contains(f.itconf.Status) : ITConfApprovedStatus2.Contains(f.itconf.Status)) : (ITConfApprovedStatus.Contains(f.itconf.Status))) : true)

            //
            && (isDeptHead ? (status == 0 ? (((f.itconf.ITConfirmationLatestHistoryVM.ITCONFSUBMITTEDTOSTATUS == 2 || f.itconf.Status == 9) && userId == f.itconf.ITConfirmationLatestHistoryVM.ITConfirmationSubmittedTo) ? f.itconf.ITConfirmationLatestHistoryVM.ITCONFSUBMITTEDTOSTATUS == 2 : ((f.itconf.ITConfirmationLatestHistoryVM.ITCONFSUBMITTEDTOSTATUS == 3 && userId == f.itconf.ITConfirmationLatestHistoryVM.ITConfirmationSubmittedTo) ? f.itconf.ITConfirmationLatestHistoryVM.ITCONFSUBMITTEDTOSTATUS == 3 : false)) : ((f.itconf.Status == 2 && userId == f.itconf.ITConfirmationLatestHistoryVM.ActionBy) ? (ITConfApprovedStatus4.Contains(f.itconf.Status)) : (ITConfApprovedStatus.Contains(f.itconf.Status)))) : true)
            && (isCoOrdHead ? (status == 0 ? (((f.itconf.ITConfirmationLatestHistoryVM.ITCONFSUBMITTEDTOSTATUS == 4 || f.itconf.Status == 9) && userId == f.itconf.ITConfirmationLatestHistoryVM.ITConfirmationSubmittedTo) ? f.itconf.ITConfirmationLatestHistoryVM.ITCONFSUBMITTEDTOSTATUS == 4 : false) : (ITConfApprovedStatus.Contains(f.itconf.Status))) : true)
            && (isDivHead ? (status == 0 ? (((f.itconf.ITConfirmationLatestHistoryVM.ITCONFSUBMITTEDTOSTATUS == 5 || f.itconf.Status == 9) && userId == f.itconf.ITConfirmationLatestHistoryVM.ITConfirmationSubmittedTo) ? f.itconf.ITConfirmationLatestHistoryVM.ITCONFSUBMITTEDTOSTATUS == 5 : ((f.itconf.ITConfirmationLatestHistoryVM.ITCONFSUBMITTEDTOSTATUS == 6 && userId == f.itconf.ITConfirmationLatestHistoryVM.ITConfirmationSubmittedTo) ? f.itconf.ITConfirmationLatestHistoryVM.ITCONFSUBMITTEDTOSTATUS == 6 : false)) : (ITConfApprovedStatus.Contains(f.itconf.Status))) : true)
            && (isExeCoOrdHead ? (status == 0 ? (((f.itconf.ITConfirmationLatestHistoryVM.ITCONFSUBMITTEDTOSTATUS == 7 || f.itconf.Status == 9) && userId == f.itconf.ITConfirmationLatestHistoryVM.ITConfirmationSubmittedTo) ? f.itconf.ITConfirmationLatestHistoryVM.ITCONFSUBMITTEDTOSTATUS == 7 : false) : (ITConfApprovedStatus.Contains(f.itconf.Status))) : true)
            && (isOperatingHead ? (status == 0 ? (((f.itconf.ITConfirmationLatestHistoryVM.ITCONFSUBMITTEDTOSTATUS == 8 || f.itconf.Status == 9) && userId == f.itconf.ITConfirmationLatestHistoryVM.ITConfirmationSubmittedTo) ? ITConfApprovedStatus1.Contains(f.itconf.Status) : false) : (ITConfApprovedStatus.Contains(f.itconf.Status))) : true)
            //            
            && (status == 3 ? (f.itconf.ITConfirmationFilledBy == userId && f.itconf.Status != 8) : true)//To get my request

            && (f.g.e.c.A00DTLTB.SYKIID == (sykiId == 0 ? f.g.e.c.A00DTLTB.SYKIID : sykiId))
            && ((paramsData.OPERATIONID == 0 || paramsData.OPERATIONID == null) || f.g.e.c.A00DTLTB.OPERATION == paramsData.OPERATIONID)
            && ((paramsData.DIVISIONID == 0 || paramsData.DIVISIONID == null) || f.g.e.c.A00DTLTB.DIVISION == paramsData.DIVISIONID)
            && ((paramsData.DEPARTMENTID == 0 || paramsData.DEPARTMENTID == null) || f.g.e.c.A00DTLTB.DEPARTMENT == paramsData.DEPARTMENTID)
            && ((paramsData.SECTIONID == 0 || paramsData.SECTIONID == null) || f.g.e.c.A00DTLTB.SECTION == paramsData.SECTIONID))).
           Select(s1 => new SearchResultList
           {
               A00DTLTBID = s1.g.e.c.A00DTLTB.A00DTLTBID,
               SYKIID = s1.g.e.c.A00DTLTB.SYKIID,
               KICODE = kiData1._SYKIList.Where(x1 => x1.SYKIID == s1.g.e.c.A00DTLTB.SYKIID).Select(x1 => x1.KICODE).FirstOrDefault(),
               OPERATION = _objHomeA00.Getoperationdetails(s1.g.e.c.A00DTLTB.ADDEDBY, (int)s1.g.e.c.A00DTLTB.SYKIID),
               ProjectCode = s1.itconf.ProjectCode,
               ProjectTitle = s1.g.e.c.A00DTLTB.PRJCTTLE,
               A00ApprovedOn = s1.g.e.c.A00APPROVAL.OHAPPDATE,
               ApprovedOn = (s1.g.al.AllocatedDate),//A00 Assigned Date
               ITConfirmationDate = _objHomeA00.GetA00ItConfirmationHistoryDetails((decimal)s1.itconf.ITCONFIRMATIONID).Status == 8 ? s1.itconf.ITCONFIRMATIONApprovalDate : null,
               //DaysDiffBetA00ApprovalAndITConfirmation = (s1.itconf.ITCONFIRMATIONApprovalDate != null ? (s1.itconf.ITCONFIRMATIONApprovalDate- s1.g.e.c.A00APPROVAL.OHAPPDATE) : (DateTime.Now- s1.g.e.c.A00APPROVAL.OHAPPDATE)),
               ECode = s1.g.e.c.A00DTLTB.ADDEDBY,
               EmpName = s1.g.e.d.FIRSTNAME + " " + s1.g.e.d.LASTNAME,
               RequestDate = s1.g.e.c.A00DTLTB.DATEADDEDID,
               ApprovedBy = _objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE.Where(x1 => x1.ADEMPCODE == s1.g.al.AllocatedBY).Select(x1 => x1.FIRSTNAME + " " + x1.LASTNAME).FirstOrDefault(),
               UserId = userId,
               MainPIC = _objHomeA00.GetA00MainPIC(s1.g.e.c.A00DTLTB.A00DTLTBID, s1.g.e.c.A00DTLTB.SYKIID),
               ApproverUserId = _objHomeA00.GetA00ItConfirmationHistoryDetails((decimal)s1.itconf.ITCONFIRMATIONID).ITConfirmationSubmittedTo,
               ITConfirmationStatus = _objHomeA00.GetA00ItConfirmationHistoryDetails((decimal)s1.itconf.ITCONFIRMATIONID).Status,
               //IsDeficiencyClosed = _objHomeA00.IsA00DeficiencyClosed(s1.c.A00DTLTB.A00DTLTBID, s1.c.A00DTLTB.SYKIID)
               //22-Sept-2021 change start
               //STATUSCD = s1.g.e.c.A00DTLTB.STATUSCD
               //22-Sept-2021 change end
           }).ToList();

            model.ResultList = resultList.OrderByDescending(x => x.RequestDate).ThenBy(x => x.ProjectTitle).ToList();
            #endregion
        }
        private void ITConfirmationGetA00SearchDataForApproval(A00ViewModel model, SearchParameterList paramsData)
        {
            List<SearchResultList> empCodeList = new List<SearchResultList>();

            var userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

            model.SearchParams = new SearchParameterList();
            model.SearchParams.OPERATIONID = paramsData.OPERATIONID;
            model.SearchParams.DIVISIONID = paramsData.DIVISIONID;
            model.SearchParams.DEPARTMENTID = paramsData.DEPARTMENTID;
            model.SearchParams.SECTIONID = paramsData.SECTIONID;
            var sykiId = paramsData.SYKIID;//0;

            // SearchParams.StatusId
            model.SearchParams.StatusId = paramsData.StatusId;
            var empCode = string.IsNullOrEmpty(paramsData.ECode) ? 0 : Convert.ToInt64(paramsData.ECode);
            var empName = paramsData.EmpName;
            model.SearchParams.ECode = paramsData.ECode;
            model.SearchParams.EmpName = empName;
            model.SearchParams.SYKIID = sykiId;

            model.SearchParams.SYKI = paramsData.SYKI;
            model.SearchParams.Status = paramsData.Status;

            var status = model.SearchParams.StatusId;

            bool isDeptHead = false, isCoOrdHead = false, isDivHead = false, isExeCoOrdHead = false, isOperatingHead = false;
            _objA00SearchModel.ADEMPCODE = userId;
            _objA00SearchModel.SYKI = (decimal)paramsData.SYKIID;
            var MainPICOperation = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel);
            _objA00SearchModel.OPERATIONID = MainPICOperation.OPERATIONID;
            var userData = _objHomeA00.getUserRole(userId, paramsData.SYKIID);
            isDeptHead = userData.isDeptHead;
            isCoOrdHead = userData.isCoOrdHead;
            isDivHead = userData.isDivHead;
            isExeCoOrdHead = userData.isExeCoOrdHead;
            isOperatingHead = userData.isOperatingHead;
            var ITConfApprovedStatus = new List<short?> { };
            var ITConfPendingStatus = new List<short?> { };
            if (isDeptHead)
            {
                ITConfApprovedStatus = new List<short?> { 2, 3, 4, 5, 6, 7, 8 };
            }
            else if (isCoOrdHead)
            {
                ITConfApprovedStatus = new List<short?> { 4, 5, 6, 7, 8 };
            }
            else if (isDivHead)
            {
                ITConfApprovedStatus = new List<short?> { 5, 6, 7, 8 };
            }
            else if (isExeCoOrdHead)
            {
                ITConfApprovedStatus = new List<short?> { 7, 8 };
            }
            else if (isOperatingHead)
            {
                ITConfApprovedStatus = new List<short?> { 8 };
            }
            else
            {
                ITConfApprovedStatus = new List<short?> { 1, 2, 3, 4, 5, 6, 7, 8 };
            }

            #region Filter Query
            var kiData1 = _objHomeA00.GetA00SYKIList();
            var resultList = _objHomeA00.GetA00APPROVALFullTable()._A00APPROVALList.
                Where(x => x.OHAPPDATE != null).AsEnumerable().
               Join(_objHomeA00.GetA00DtlFullTable()._A00DtlViewModelList,
               a => a.A00DTLTBID, b => b.A00DTLTBID, (a, b) => new { A00APPROVAL = a, A00DTLTB = b }).
               Join(_objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE, c => c.A00DTLTB.ADDEDBY, d => d.ADEMPCODE, (c, d) => new { c, d }).
               Join(_objHomeA00.GetA00AllocationList()._A00AllocationList, e => e.c.A00DTLTB.A00DTLTBID, al => al.A00ID, (e, al) => new { e, al }).
               Join(_objHomeA00.GetA00ItConfirmationList()._A00ITConfirmationList, g => g.al.A00AllocationID, itconf => itconf.A00ALLOCATIONID, (g, itconf) => new { g, itconf = itconf })
               .Where(f =>
               (string.IsNullOrEmpty(empName) ? true : ((f.g.e.d.FIRSTNAME + " " + f.g.e.d.LASTNAME).Contains(empName)))
            && (f.g.e.c.A00DTLTB.ADDEDBY == (empCode == 0 ? f.g.e.c.A00DTLTB.ADDEDBY : empCode)
            && (f.g.e.c.A00DTLTB.STATUSCD == 105)
            && (status == 0 ? !ITConfApprovedStatus.Contains(f.itconf.Status) : ITConfApprovedStatus.Contains(f.itconf.Status))
            && (f.g.e.c.A00DTLTB.SYKIID == (sykiId == 0 ? f.g.e.c.A00DTLTB.SYKIID : sykiId))
            && ((paramsData.OPERATIONID == 0 || paramsData.OPERATIONID == null) || f.g.e.c.A00DTLTB.OPERATION == paramsData.OPERATIONID)
            && ((paramsData.DIVISIONID == 0 || paramsData.DIVISIONID == null) || f.g.e.c.A00DTLTB.DIVISION == paramsData.DIVISIONID)
            && ((paramsData.DEPARTMENTID == 0 || paramsData.DEPARTMENTID == null) || f.g.e.c.A00DTLTB.DEPARTMENT == paramsData.DEPARTMENTID)
            && ((paramsData.SECTIONID == 0 || paramsData.SECTIONID == null) || f.g.e.c.A00DTLTB.SECTION == paramsData.SECTIONID))).
           Select(s1 => new SearchResultList
           {
               A00DTLTBID = s1.g.e.c.A00DTLTB.A00DTLTBID,
               SYKIID = s1.g.e.c.A00DTLTB.SYKIID,
               KICODE = kiData1._SYKIList.Where(x1 => x1.SYKIID == s1.g.e.c.A00DTLTB.SYKIID).Select(x1 => x1.KICODE).FirstOrDefault(),
               OPERATION = _objHomeA00.Getoperationdetails(s1.g.e.c.A00DTLTB.ADDEDBY, (int)s1.g.e.c.A00DTLTB.SYKIID),
               ProjectCode = s1.itconf.ProjectCode,
               ProjectTitle = s1.g.e.c.A00DTLTB.PRJCTTLE,
               A00ApprovedOn = s1.g.e.c.A00APPROVAL.OHAPPDATE,
               ApprovedOn = (s1.g.al.AllocatedDate),//A00 Assigned Date
               ITConfirmationDate = s1.itconf.ITCONFIRMATIONApprovalDate,
               //DaysDiffBetA00ApprovalAndITConfirmation = (s1.itconf.ITCONFIRMATIONApprovalDate != null ? (s1.itconf.ITCONFIRMATIONApprovalDate- s1.g.e.c.A00APPROVAL.OHAPPDATE) : (DateTime.Now- s1.g.e.c.A00APPROVAL.OHAPPDATE)),
               ECode = s1.g.e.c.A00DTLTB.ADDEDBY,
               EmpName = s1.g.e.d.FIRSTNAME + " " + s1.g.e.d.LASTNAME,
               RequestDate = s1.g.e.c.A00DTLTB.DATEADDEDID,
               ApprovedBy = _objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE.Where(x1 => x1.ADEMPCODE == s1.g.al.AllocatedBY).Select(x1 => x1.FIRSTNAME + " " + x1.LASTNAME).FirstOrDefault(),
               UserId = userId,
               MainPIC = _objHomeA00.GetA00MainPIC(s1.g.e.c.A00DTLTB.A00DTLTBID, s1.g.e.c.A00DTLTB.SYKIID),
               ApproverUserId = _objHomeA00.GetA00ItConfirmationHistoryDetails((decimal)s1.itconf.ITCONFIRMATIONID).ITConfirmationSubmittedTo,
               ITConfirmationStatus = _objHomeA00.GetA00ItConfirmationHistoryDetails((decimal)s1.itconf.ITCONFIRMATIONID).Status,
               //IsDeficiencyClosed = _objHomeA00.IsA00DeficiencyClosed(s1.c.A00DTLTB.A00DTLTBID, s1.c.A00DTLTB.SYKIID)
           }).ToList();
            #endregion

            model.ResultList = resultList.OrderByDescending(x => x.RequestDate).ThenBy(x => x.ProjectTitle).ToList();

        }

        public ActionResult ITConfirmationSaveData(long A00DTLTBID, decimal SYKIID)
        {
            try
            {
                TempData["PageHead"] = "IT Confirmation Form";
                var DeficiencyStatus = _objHomeA00.GetDeficiencyStatus(A00DTLTBID, SYKIID);
                ViewBag.DeficiencyStatus = DeficiencyStatus;
                ITConfirmationVM ProjectDetals = new ITConfirmationVM();
                var _ProjecDetails = _objHomeA00.GetA00ProjectDetails(A00DTLTBID, SYKIID);
                var A00UserDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_ProjecDetails.AddedBy);
                var UserDept = _objHomeA00.GetEmpDept(_ProjecDetails.AddedBy, SYKIID);
                var UserName = _objHomeA00.GetEmpName(_ProjecDetails.AddedBy);
                var MainPICDetails = _objHomeA00.GetA00MainPIC(A00DTLTBID, SYKIID);
                var ITDept = _objHomeA00.GetEmpDept(MainPICDetails, SYKIID);
                var ITName = _objHomeA00.GetEmpName(MainPICDetails);
                int SNo = _objHomeA00.GetA00AllocationSNo(A00DTLTBID, SYKIID);
                var ProjectCode = DateTime.Now.ToString("MMyyyy") + "-" + string.Format("{0:D4}", A00DTLTBID);

                ProjectDetals = _objHomeA00.GetA00ItConfirmationDetails(A00DTLTBID, SYKIID);
                if (ProjectDetals != null && ProjectDetals.ITCONFIRMATIONID > 0)
                {
                    ProjectDetals.A00DTLTBID = A00DTLTBID;
                    ProjectDetals.SYKIID = SYKIID;
                    ProjectDetals.ApplicationPIC = _objHomeA00.GetA00ApplicationPIC(A00DTLTBID, SYKIID);
                    ProjectDetals.InfraStructurePIC = _objHomeA00.GetA00InfraStructurePIC(A00DTLTBID, SYKIID);
                    ProjectDetals.ProjectCode_Date = ProjectCode;
                    ProjectDetals.ProjectName = _ProjecDetails.ProjectName;
                    ProjectDetals.ProjectDate = _ProjecDetails.ProjectDate.ToShortDateString();
                    ProjectDetals.UserPL = _ProjecDetails.AddedBy;
                    ProjectDetals.UserPL_Name = _ProjecDetails.AddedBy + " / " + UserName;
                    ProjectDetals.UserDept = UserDept;
                    ProjectDetals.ITPL = MainPICDetails;
                    ProjectDetals.ITPL_Name = MainPICDetails + " / " + ITName;
                    ProjectDetals.ITDept = ITDept;
                }
                else
                {
                    ProjectDetals = new ITConfirmationVM();
                    ProjectDetals.A00DTLTBID = A00DTLTBID;
                    ProjectDetals.SYKIID = SYKIID;
                    ProjectDetals.ApplicationPIC = _objHomeA00.GetA00ApplicationPIC(A00DTLTBID, SYKIID);
                    ProjectDetals.InfraStructurePIC = _objHomeA00.GetA00InfraStructurePIC(A00DTLTBID, SYKIID);
                    ProjectDetals.ProjectCode_Date = ProjectCode;
                    ProjectDetals.ProjectName = _ProjecDetails.ProjectName;
                    ProjectDetals.ProjectDate = _ProjecDetails.ProjectDate.ToShortDateString();
                    ProjectDetals.UserPL = _ProjecDetails.AddedBy;
                    ProjectDetals.UserPL_Name = _ProjecDetails.AddedBy + " / " + UserName;
                    ProjectDetals.UserDept = UserDept;
                    ProjectDetals.ITPL = MainPICDetails;
                    ProjectDetals.ITPL_Name = MainPICDetails + " / " + ITName;
                    ProjectDetals.ITDept = ITDept;
                }
                ViewBag.Application = ProjectDetals.Application;
                ViewBag.HSDMCR = ProjectDetals.HSDM;
                return View(ProjectDetals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ITConfirmationSaveData");
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ITConfirmationSaveData(ITConfirmationVM model, IFormCollection fc)
        {
            ModelState.Keys.Except(Request.Form.Keys).ToList().ForEach(key => ModelState.Remove(key));

            try
            {
                A00SearchModel _objA00Search2ndPICModel = new A00SearchModel();
                var UserId = Convert.ToInt32(_sessionService.Get<string>("userID"));
                var MainPICUserId = 0;
                var ApplicationPic = Convert.ToInt32(model.ApplicationPIC);
                var InfraPic = Convert.ToInt32(model.InfraStructurePIC);
                int PICUserId_2nd = 0;
                int UserType = 0;
                //Get 2ndPIC UserId
                if (UserId == ApplicationPic)
                {
                    UserType = 1;
                    model.UserType = UserType;
                    MainPICUserId = ApplicationPic;
                    PICUserId_2nd = InfraPic;
                }
                else if (UserId == InfraPic)
                {
                    UserType = 2;
                    model.UserType = UserType;
                    MainPICUserId = InfraPic;
                    PICUserId_2nd = ApplicationPic;
                }
                if (ModelState.IsValid)
                {
                    InsertA00APPROVAL approval = new InsertA00APPROVAL();

                    #region Main PIC
                    bool isDeptHead = false, isCoOrdHead = false, isDivHead = false, isExeCoOrdHead = false, isOperatingHead = false;
                    _objA00SearchModel.ADEMPCODE = MainPICUserId;
                    _objA00SearchModel.SYKI = model.SYKIID;
                    var MainPICOperation = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel);
                    _objA00SearchModel.OPERATIONID = MainPICOperation.OPERATIONID;
                    var userData = _objHomeA00.getUserRole(MainPICUserId, model.SYKIID);
                    isDeptHead = userData.isDeptHead;
                    isCoOrdHead = userData.isCoOrdHead;
                    isDivHead = userData.isDivHead;
                    isExeCoOrdHead = userData.isExeCoOrdHead;
                    isOperatingHead = userData.isOperatingHead;
                    #endregion
                    #region 2nd PIC
                    _objA00Search2ndPICModel.ADEMPCODE = PICUserId_2nd;
                    _objA00Search2ndPICModel.SYKI = model.SYKIID;
                    var Operation_2ndPIC = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00Search2ndPICModel);
                    #endregion

                    //22-Sept-2021 change start
                    #region Save attachment
                    string UploadPath = Path.Combine(serverpath.getFileUploadPath(), "A00");
                    string FileName = null;
                    //if (model.ImageFile != null)
                    //{
                    //    FileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                    //    string FileExtension = Path.GetExtension(model.ImageFile.FileName);

                    //    FileName = Convert.ToString(_sessionService.Get<string>("userID")) + "_" + model.ProjectCode_Date + FileExtension;
                    //    model.Attachment_Name = FileName;
                    //    model.ImagePath = UploadPath + FileName;
                    //    model.ImageFile.SaveAs(model.ImagePath);
                    //}
                    if (model.ImageFile != null)
                    {
                        FileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                        string FileExtension = Path.GetExtension(model.ImageFile.FileName);

                        var userId = _sessionService.Get<string>("userID");
                        FileName = $"{userId}_{model.ProjectCode_Date}{FileExtension}";

                        model.Attachment_Name = FileName;
                        model.ImagePath = Path.Combine(UploadPath, FileName);
                        string directoryPath = Path.GetDirectoryName(model.ImagePath);
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }
                        using (var stream = new FileStream(model.ImagePath, FileMode.Create))
                        {
                            model.ImageFile.CopyTo(stream);
                        }
                    }
                    #endregion
                    //22-Sept-2021 change start

                    string nextApproverToEmail = string.Empty;
                    #region Update Roles in A00ITConfirmation Table
                    var _adOrgLevelHead = _objHomeA00.GetA00_ADORGLEVELHEAD()._A00_ADORGLEVELHEAD;
                    if (!string.IsNullOrEmpty(fc["btnSubmit"]) && fc["btnSubmit"].ToString().ToUpper() == "SUBMIT")
                    {
                        #region When form submitted
                        if (!isOperatingHead) // current user role check
                        {
                            if (!isExeCoOrdHead) // current user role check
                            {
                                //var opHeadValue = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.OPERATIONID).Select(x => x.ADEMPCODE).FirstOrDefault();

                                if (!isDivHead) // current user role check
                                {
                                    if (!isCoOrdHead) // current user role check
                                    {
                                        if (!isDeptHead) // current user role check
                                        {
                                            //// - Comment by Vishal on 11-july-23
                                            //approval.DEPTHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.DEPARTMENTID && x.ISACTIVE == 1).Select(x => x.ADEMPCODE).FirstOrDefault(); // Dept Head as Approver
                                            //if (approval.DEPTHDID == null || approval.DEPTHDID == 0)
                                            //{
                                            ///// - Comment End

                                            //Main PIC Dept.Head
                                            var divHead = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault(); //// ADDED BY VISHAL ON 07-July-23
                                            var deptHead = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.DEPARTMENTID && x.ISACTIVE == 1).Select(x => x.ADEMPCODE).FirstOrDefault(); // Dept Head as Approver
                                            if (deptHead == null || deptHead == 0 || deptHead == divHead)
                                            {
                                                //// - Comment by Vishal on 11-july-23
                                                //approval.DEPTHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == Operation_2ndPIC.DEPARTMENTID && x.ISACTIVE == 1).Select(x => x.ADEMPCODE).FirstOrDefault(); // Dept Head as Approver
                                                //if (approval.DEPTHDID == null || approval.DEPTHDID == 0)
                                                //{
                                                ///// - Comment End

                                                //Get 2nd PIC Dept. Head
                                                var secPICDivHead = _adOrgLevelHead.Where(x => x.ADORGLEVELID == Operation_2ndPIC.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                                var secPICDeptHead = _adOrgLevelHead.Where(x => x.ADORGLEVELID == Operation_2ndPIC.DEPARTMENTID && x.ISACTIVE == 1).Select(x => x.ADEMPCODE).FirstOrDefault(); // Dept Head as Approver
                                                if (secPICDeptHead == null || secPICDeptHead == 0 || secPICDeptHead == secPICDivHead)
                                                {
                                                    //Main PIC COORDDID                                                    
                                                    approval.COORDDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).COORDINATOR;
                                                    if (approval.COORDDID == null || approval.COORDDID == 0)
                                                    {
                                                        //Main PIC DIVHDHDID
                                                        approval.DIVHDHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                                        if (approval.DIVHDHDID == null || approval.DIVHDHDID == 0)
                                                        {
                                                            //2ndPIC DIVHDHD
                                                            approval.DIVHDHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == Operation_2ndPIC.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                                            if (approval.DIVHDHDID == null || approval.DIVHDHDID == 0)
                                                            {
                                                                //MainPIC EXECOHDID
                                                                approval.EXECOHDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).EXECOORDINATOR;
                                                                if (approval.EXECOHDID == null || approval.EXECOHDID == 0)
                                                                {
                                                                    _objA00SearchModel.OHID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).OPHEAD;
                                                                    model.Status = 1;
                                                                    model.ITCONFSUBMITTEDTOUSERTYPE = (short)UserType;
                                                                    model.ITCONFIRMATIONSUBMITTEDTO = _objA00SearchModel.OHID;
                                                                    model.ITCONFSUBMITTEDTOSTATUS = 8;
                                                                    var result = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                                                    nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.OHID).EMAILID;
                                                                    var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                                                    if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                                    {
                                                                        sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    model.Status = 1;
                                                                    model.ITCONFSUBMITTEDTOUSERTYPE = (short)UserType;
                                                                    model.ITCONFIRMATIONSUBMITTEDTO = approval.EXECOHDID;
                                                                    model.ITCONFSUBMITTEDTOSTATUS = 7;
                                                                    _objA00SearchModel.EXECOHDID = approval.EXECOHDID;
                                                                    var result = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                                                    nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.EXECOHDID).EMAILID;
                                                                    var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                                                    if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                                    {
                                                                        sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                model.Status = 1;
                                                                model.ITCONFSUBMITTEDTOUSERTYPE = (short)(UserType == 1 ? 2 : 1);
                                                                model.ITCONFIRMATIONSUBMITTEDTO = approval.DIVHDHDID;
                                                                model.ITCONFSUBMITTEDTOSTATUS = 6;
                                                                _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                                                var result = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                                                nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                                                var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                                                if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                                {
                                                                    sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            model.Status = 1;
                                                            model.ITCONFSUBMITTEDTOUSERTYPE = (short)UserType;
                                                            model.ITCONFIRMATIONSUBMITTEDTO = approval.DIVHDHDID;
                                                            model.ITCONFSUBMITTEDTOSTATUS = 5;
                                                            _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                                            var result = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                                            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                                            if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                            {
                                                                sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        model.Status = 1;
                                                        model.ITCONFSUBMITTEDTOUSERTYPE = (short)UserType;
                                                        model.ITCONFIRMATIONSUBMITTEDTO = approval.COORDDID;
                                                        model.ITCONFSUBMITTEDTOSTATUS = 4;
                                                        _objA00SearchModel.COORDDID = approval.COORDDID;
                                                        var result = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                                        nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.COORDDID).EMAILID;
                                                        var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                                        if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                        {
                                                            sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    approval.DEPTHDID = secPICDeptHead;
                                                    model.Status = 1;
                                                    model.ITCONFSUBMITTEDTOUSERTYPE = (short)(UserType == 1 ? 2 : 1);
                                                    model.ITCONFIRMATIONSUBMITTEDTO = approval.DEPTHDID;
                                                    model.ITCONFSUBMITTEDTOSTATUS = 3;
                                                    _objA00SearchModel.DEPTHDID = approval.DEPTHDID;
                                                    var result = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                                    nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DEPTHDID).EMAILID;
                                                    var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                                    if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                    {
                                                        sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                approval.DEPTHDID = deptHead;
                                                model.Status = 1;
                                                model.ITCONFSUBMITTEDTOUSERTYPE = (short)UserType;
                                                model.ITCONFIRMATIONSUBMITTEDTO = approval.DEPTHDID;
                                                model.ITCONFSUBMITTEDTOSTATUS = 2;
                                                _objA00SearchModel.DEPTHDID = approval.DEPTHDID;
                                                var result = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                                nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DEPTHDID).EMAILID;
                                                var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                                if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                {
                                                    sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //Get 2nd PIC Dept. Head
                                            approval.DEPTHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == Operation_2ndPIC.DEPARTMENTID && x.ISACTIVE == 1).Select(x => x.ADEMPCODE).FirstOrDefault(); // Dept Head as Approver
                                            if (approval.DEPTHDID == null || approval.DEPTHDID == 0)
                                            {
                                                //Main PIC COORDDID                                                    
                                                approval.COORDDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).COORDINATOR;
                                                if (approval.COORDDID == null || approval.COORDDID == 0)
                                                {
                                                    //Main PIC DIVHDHDID
                                                    approval.DIVHDHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                                    if (approval.DIVHDHDID == null || approval.DIVHDHDID == 0)
                                                    {
                                                        //2ndPIC DIVHDHD
                                                        approval.DIVHDHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == Operation_2ndPIC.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                                        if (approval.DIVHDHDID == null || approval.DIVHDHDID == 0)
                                                        {
                                                            //MainPIC EXECOHDID
                                                            approval.EXECOHDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).EXECOORDINATOR;
                                                            if (approval.EXECOHDID == null || approval.EXECOHDID == 0)
                                                            {
                                                                _objA00SearchModel.OHID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).OPHEAD;
                                                                model.Status = 2;
                                                                model.ITCONFSUBMITTEDTOUSERTYPE = (short)UserType;
                                                                model.ITCONFIRMATIONSUBMITTEDTO = _objA00SearchModel.OHID;
                                                                model.ITCONFSUBMITTEDTOSTATUS = 8;
                                                                var result = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                                                nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.OHID).EMAILID;
                                                                var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                                                if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                                {
                                                                    sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                model.Status = 2;
                                                                model.ITCONFSUBMITTEDTOUSERTYPE = (short)UserType;
                                                                model.ITCONFIRMATIONSUBMITTEDTO = approval.EXECOHDID;
                                                                model.ITCONFSUBMITTEDTOSTATUS = 7;
                                                                _objA00SearchModel.EXECOHDID = approval.EXECOHDID;
                                                                var result = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                                                nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.EXECOHDID).EMAILID;
                                                                var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                                                if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                                {
                                                                    sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            model.Status = 2;
                                                            model.ITCONFSUBMITTEDTOUSERTYPE = (short)(UserType == 1 ? 2 : 1);
                                                            model.ITCONFIRMATIONSUBMITTEDTO = approval.DIVHDHDID;
                                                            model.ITCONFSUBMITTEDTOSTATUS = 6;
                                                            _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                                            var result = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                                            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                                            if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                            {
                                                                sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        model.Status = 2;
                                                        model.ITCONFSUBMITTEDTOUSERTYPE = (short)UserType;
                                                        model.ITCONFIRMATIONSUBMITTEDTO = approval.DIVHDHDID;
                                                        model.ITCONFSUBMITTEDTOSTATUS = 5;
                                                        _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                                        var result = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                                        nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                                        var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                                        if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                        {
                                                            sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    model.Status = 2;
                                                    model.ITCONFSUBMITTEDTOUSERTYPE = (short)UserType;
                                                    model.ITCONFIRMATIONSUBMITTEDTO = approval.COORDDID;
                                                    model.ITCONFSUBMITTEDTOSTATUS = 4;
                                                    _objA00SearchModel.COORDDID = approval.COORDDID;
                                                    var result = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                                    nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.COORDDID).EMAILID;
                                                    var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                                    if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                    {
                                                        sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (approval.DEPTHDID != MainPICUserId)
                                                {
                                                    model.Status = 2;
                                                    model.ITCONFSUBMITTEDTOUSERTYPE = (short)(UserType == 1 ? 2 : 1);
                                                    model.ITCONFIRMATIONSUBMITTEDTO = approval.DEPTHDID;
                                                    model.ITCONFSUBMITTEDTOSTATUS = 3;
                                                    _objA00SearchModel.DEPTHDID = approval.DEPTHDID;
                                                    TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                                    nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DEPTHDID).EMAILID;
                                                    var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                                    if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                    {
                                                        sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                    }
                                                }
                                                else
                                                {
                                                    //Main PIC COORDDID                                                    
                                                    approval.COORDDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).COORDINATOR;
                                                    if (approval.COORDDID == null || approval.COORDDID == 0)
                                                    {
                                                        //Main PIC DIVHDHDID
                                                        approval.DIVHDHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                                        if (approval.DIVHDHDID == null || approval.DIVHDHDID == 0)
                                                        {
                                                            //2ndPIC DIVHDHD
                                                            approval.DIVHDHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == Operation_2ndPIC.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                                            if (approval.DIVHDHDID == null || approval.DIVHDHDID == 0)
                                                            {
                                                                //MainPIC EXECOHDID
                                                                approval.EXECOHDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).EXECOORDINATOR;
                                                                if (approval.EXECOHDID == null || approval.EXECOHDID == 0)
                                                                {
                                                                    _objA00SearchModel.OHID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).OPHEAD;
                                                                    model.Status = 2;
                                                                    model.ITCONFSUBMITTEDTOUSERTYPE = (short)UserType;
                                                                    model.ITCONFIRMATIONSUBMITTEDTO = _objA00SearchModel.OHID;
                                                                    model.ITCONFSUBMITTEDTOSTATUS = 8;
                                                                    TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                                                    nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.OHID).EMAILID;
                                                                    var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                                                    if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                                    {
                                                                        sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    model.Status = 2;
                                                                    model.ITCONFSUBMITTEDTOUSERTYPE = (short)UserType;
                                                                    model.ITCONFIRMATIONSUBMITTEDTO = approval.EXECOHDID;
                                                                    model.ITCONFSUBMITTEDTOSTATUS = 7;
                                                                    _objA00SearchModel.EXECOHDID = approval.EXECOHDID;
                                                                    TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                                                    nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.EXECOHDID).EMAILID;
                                                                    var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                                                    if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                                    {
                                                                        sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                model.Status = 2;
                                                                model.ITCONFSUBMITTEDTOUSERTYPE = (short)(UserType == 1 ? 2 : 1);
                                                                model.ITCONFIRMATIONSUBMITTEDTO = approval.DIVHDHDID;
                                                                model.ITCONFSUBMITTEDTOSTATUS = 6;
                                                                _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                                                TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                                                nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                                                var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                                                if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                                {
                                                                    sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            model.Status = 2;
                                                            model.ITCONFSUBMITTEDTOUSERTYPE = (short)UserType;
                                                            model.ITCONFIRMATIONSUBMITTEDTO = approval.DIVHDHDID;
                                                            model.ITCONFSUBMITTEDTOSTATUS = 5;
                                                            _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                                            TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                                            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                                            if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                            {
                                                                sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        model.Status = 2;
                                                        model.ITCONFSUBMITTEDTOUSERTYPE = (short)UserType;
                                                        model.ITCONFIRMATIONSUBMITTEDTO = approval.COORDDID;
                                                        model.ITCONFSUBMITTEDTOSTATUS = 4;
                                                        _objA00SearchModel.COORDDID = approval.COORDDID;
                                                        TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                                        nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.COORDDID).EMAILID;
                                                        var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                                        if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                        {
                                                            sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //Main PIC DIVHDHDID
                                        approval.DIVHDHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                        if (approval.DIVHDHDID == null || approval.DIVHDHDID == 0)
                                        {
                                            //2ndPIC DIVHDHD
                                            approval.DIVHDHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == Operation_2ndPIC.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                            if (approval.DIVHDHDID == null || approval.DIVHDHDID == 0)
                                            {
                                                //MainPIC EXECOHDID
                                                approval.EXECOHDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).EXECOORDINATOR;
                                                if (approval.EXECOHDID == null || approval.EXECOHDID == 0)
                                                {
                                                    _objA00SearchModel.OHID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).OPHEAD;
                                                    model.Status = 4;
                                                    model.ITCONFSUBMITTEDTOUSERTYPE = (short)UserType;
                                                    model.ITCONFIRMATIONSUBMITTEDTO = _objA00SearchModel.OHID;
                                                    model.ITCONFSUBMITTEDTOSTATUS = 8;
                                                    TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                                    nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.OHID).EMAILID;
                                                    var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                                    if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                    {
                                                        sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                    }
                                                }
                                                else
                                                {
                                                    model.Status = 4;
                                                    model.ITCONFSUBMITTEDTOUSERTYPE = (short)UserType;
                                                    model.ITCONFIRMATIONSUBMITTEDTO = approval.EXECOHDID;
                                                    model.ITCONFSUBMITTEDTOSTATUS = 7;
                                                    _objA00SearchModel.EXECOHDID = approval.EXECOHDID;
                                                    TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                                    nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.EXECOHDID).EMAILID;
                                                    var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                                    if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                    {
                                                        sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                model.Status = 4;
                                                model.ITCONFSUBMITTEDTOUSERTYPE = (short)(UserType == 1 ? 2 : 1);
                                                model.ITCONFIRMATIONSUBMITTEDTO = approval.DIVHDHDID;
                                                model.ITCONFSUBMITTEDTOSTATUS = 6;
                                                _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                                TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                                nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                                var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                                if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                {
                                                    sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            model.Status = 4;
                                            model.ITCONFSUBMITTEDTOUSERTYPE = (short)UserType;
                                            model.ITCONFIRMATIONSUBMITTEDTO = approval.DIVHDHDID;
                                            model.ITCONFSUBMITTEDTOSTATUS = 5;
                                            _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                            TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                            if (!string.IsNullOrEmpty(nextApproverToEmail))
                                            {
                                                sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //2ndPIC DIVHDHD
                                    approval.DIVHDHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == Operation_2ndPIC.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                    if (approval.DIVHDHDID == null || approval.DIVHDHDID == 0)
                                    {
                                        //MainPIC EXECOHDID
                                        approval.EXECOHDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).EXECOORDINATOR;
                                        if (approval.EXECOHDID == null || approval.EXECOHDID == 0)
                                        {
                                            _objA00SearchModel.OHID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).OPHEAD;
                                            model.Status = 5;
                                            model.ITCONFSUBMITTEDTOUSERTYPE = (short)UserType;
                                            model.ITCONFIRMATIONSUBMITTEDTO = _objA00SearchModel.OHID;
                                            model.ITCONFSUBMITTEDTOSTATUS = 8;
                                            TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.OHID).EMAILID;
                                            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                            if (!string.IsNullOrEmpty(nextApproverToEmail))
                                            {
                                                sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                            }
                                        }
                                        else
                                        {
                                            model.Status = 5;
                                            model.ITCONFSUBMITTEDTOUSERTYPE = (short)UserType;
                                            model.ITCONFIRMATIONSUBMITTEDTO = approval.EXECOHDID;
                                            model.ITCONFSUBMITTEDTOSTATUS = 7;
                                            _objA00SearchModel.EXECOHDID = approval.EXECOHDID;
                                            TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.EXECOHDID).EMAILID;
                                            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                            if (!string.IsNullOrEmpty(nextApproverToEmail))
                                            {
                                                sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (approval.DIVHDHDID != MainPICUserId)
                                        {
                                            model.Status = 5;
                                            model.ITCONFSUBMITTEDTOUSERTYPE = (short)(UserType == 1 ? 2 : 1);
                                            model.ITCONFIRMATIONSUBMITTEDTO = approval.DIVHDHDID;
                                            model.ITCONFSUBMITTEDTOSTATUS = 6;
                                            _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                            TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                            if (!string.IsNullOrEmpty(nextApproverToEmail))
                                            {
                                                sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                            }
                                        }
                                        else
                                        {
                                            //MainPIC EXECOHDID
                                            approval.EXECOHDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).EXECOORDINATOR;
                                            if (approval.EXECOHDID == null || approval.EXECOHDID == 0)
                                            {
                                                _objA00SearchModel.OHID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).OPHEAD;
                                                model.Status = 5;
                                                model.ITCONFSUBMITTEDTOUSERTYPE = (short)UserType;
                                                model.ITCONFIRMATIONSUBMITTEDTO = _objA00SearchModel.OHID;
                                                model.ITCONFSUBMITTEDTOSTATUS = 8;
                                                TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                                nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.OHID).EMAILID;
                                                var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                                if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                {
                                                    sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                }
                                            }
                                            else
                                            {
                                                model.Status = 5;
                                                model.ITCONFSUBMITTEDTOUSERTYPE = (short)UserType;
                                                model.ITCONFIRMATIONSUBMITTEDTO = approval.EXECOHDID;
                                                model.ITCONFSUBMITTEDTOSTATUS = 7;
                                                _objA00SearchModel.EXECOHDID = approval.EXECOHDID;
                                                TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                                nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.EXECOHDID).EMAILID;
                                                var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                                if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                {
                                                    sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                _objA00SearchModel.OHID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).OPHEAD;
                                if (approval.OHID != null || approval.OHID != 0)
                                {
                                    model.Status = 7;
                                    model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                    model.ITCONFIRMATIONSUBMITTEDTO = _objA00SearchModel.OHID;
                                    model.ITCONFSUBMITTEDTOSTATUS = 8;
                                    TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSubmit"].ToString().ToUpper(), _objA00SearchModel);

                                    nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.OHID).EMAILID;
                                    var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(MainPICUserId);
                                    if (!string.IsNullOrEmpty(nextApproverToEmail))
                                    {
                                        sendMailToNextLevelAfterITConfirmationFormFilled(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                    }
                                }
                            }
                        }
                        else
                        {
                            //MainPIC OH
                            model.Status = 8;
                            model.ITCONFIRMATIONSUBMITTEDTO = approval.OHID;
                            model.IsOHLoggedIn = true;
                            //model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                            _objA00SearchModel.OHID = approval.OHID;
                            TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                            if (!string.IsNullOrEmpty(requesterDetails.EMAILID))
                            {
                                sendMailToNextLevelAfterITConfirmationFormFilled(requesterDetails.EMAILID, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                            }
                        }
                        #endregion
                    }
                    else if (!string.IsNullOrEmpty(fc["btnSaveDraft"]) && fc["btnSaveDraft"].ToString().ToUpper() == "SAVE AS DRAFT")
                    {
                        TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSaveDraft"].ToString().ToUpper(), _objA00SearchModel);
                    }
                    #endregion

                    //var result = _objHomeA00.ITConfirmationSaveData(model, listsession);
                }
                //return View();
                return RedirectToAction("ITConfirmation");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ITConfirmationSaveData");
                throw new Exception(ex.Message);
            }
        }

        public ActionResult A00ITConfirmationApproval()
        {
            TempData["PageHead"] = "A00 IT Confirmation Approval List";
            SearchParameterList paramsList = new SearchParameterList();
            A00ViewModel model = new A00ViewModel();
            try
            {
                var paramData = A00ITConfirmationApprovalSearchFilterData(new SearchParameterList());
                //model.SearchParams.SYKI = paramData.SYKIID;
                //model.SearchParams.Status = paramData.Status;

                A00ITConfirmationApprovalGetA00SearchData(model, paramData);

                //List<ADORGLEVEL> _opList = _objHomeA00.GetOrgLevelList((long)1, (long)paramData.SYKIID);
                List<A00ADORGLEVELList> _opList = _objHomeA00.GetA00ADORGLEVELList(paramData.SYKIID)._ADOrgLevelList;
                ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.DivList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.DepList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.SecList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "A00ITConfirmationApproval");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult A00ITConfirmationApprovalSearch(A00ViewModel model)
        {
            try
            {

                TempData["PageHead"] = "Approved A00 View to IT Division Head/OH (for Allocation/ Deficiency Report)";
                SearchParameterList paramlist = new SearchParameterList();

                //Employee_Details employeeDetails = (Employee_Details)Session["Employee"];
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");

                var sykiId = 0;
                if (!string.IsNullOrEmpty(Request.Form["SearchParams.SYKI"].ToString()))
                {
                    sykiId = Convert.ToInt32(Request.Form["SearchParams.SYKI"].ToString());
                }
                paramlist.SYKIID = sykiId;
                paramlist.ECode = Request.Form["SearchParams.ECode"].ToString();
                paramlist.EmpName = Request.Form["SearchParams.EmpName"].ToString();
                //paramlist.StatusId = Convert.ToInt16(Request.Query["SearchParams.Status"].ToString());
                paramlist.StatusId = Convert.ToInt16(Request.Form["SearchParams.Status"].ToString());

                paramlist.OPERATIONID = Request.Form["SearchParams.OPERATIONID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.OPERATIONID"]) : 0;
                paramlist.DIVISIONID = Request.Form["SearchParams.DIVISIONID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.DIVISIONID"]) : 0;
                paramlist.DEPARTMENTID = Request.Form["SearchParams.DEPARTMENTID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.DEPARTMENTID"]) : 0;
                paramlist.SECTIONID = Request.Form["SearchParams.SECTIONID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.SECTIONID"]) : 0;

                #region Block-1
                var userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

                model.SearchParams = new SearchParameterList();
                model.SearchParams.StatusId = paramlist.StatusId;
                var empCode = string.IsNullOrEmpty(Request.Form["SearchParams.ECode"].ToString()) ? 0 : Convert.ToInt64(Request.Form["SearchParams.ECode"].ToString());
                var empName = Request.Form["SearchParams.EmpName"].ToString();
                model.SearchParams.ECode = Request.Form["SearchParams.ECode"].ToString();
                model.SearchParams.EmpName = empName;
                model.SearchParams.SYKIID = sykiId;
                #endregion

                var paramData = A00ITConfirmationApprovalSearchFilterData(paramlist);

                model.SearchParams.SYKI = paramData.SYKI;
                model.SearchParams.Status = paramData.Status;

                A00ITConfirmationApprovalGetA00SearchData(model, paramData);

                #region added by kiran

                //List<ADORGLEVEL>

                //List<ADORGLEVEL> _opList = _objHomeA00.GetOrgLevelList((long)1, (long)paramData.SYKIID);
                List<A00ADORGLEVELList> _opList = _objHomeA00.GetA00ADORGLEVELList(paramData.SYKIID)._ADOrgLevelList;
                ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP", paramlist.OPERATIONID);

                List<SearchParameterList> _divList = _objHomeA00.BindDivision(paramlist.OPERATIONID, (long)paramData.SYKIID);// _objHomeA00.GetOrgLevelList((long)2);
                ViewBag.DivList = new SelectList(_divList, "DIVISIONID", "DIVISION", paramlist.DIVISIONID);

                List<SearchParameterList> _depList = _objHomeA00.BindDepartment(paramlist.DIVISIONID, 0, (long)paramData.SYKIID);// _objHomeA00.GetOrgLevelList((long)3);
                ViewBag.DepList = new SelectList(_depList, "DEPARTMENTID", "DEPARTMENT", paramlist.DEPARTMENTID);


                List<SearchParameterList> _secList = _objHomeA00.BindSection(paramlist.DEPARTMENTID, 0, 0, (long)paramData.SYKIID); //_objHomeA00.GetOrgLevelList((long)4);
                ViewBag.SecList = new SelectList(_secList, "SECTIONID", "SECTION", paramlist.SECTIONID);

                paramlist.OPERATIONID = employeeDetails._OpId;
                paramlist.OPERATION = employeeDetails._OpDesc;

                paramlist.DIVISIONID = employeeDetails._DivId;
                paramlist.DIVISION = employeeDetails._DivDesc;

                paramlist.DEPARTMENTID = employeeDetails._DepId;
                paramlist.DEPARTMENT = employeeDetails._DepDesc;

                paramlist.SECTIONID = employeeDetails._SecId;
                paramlist.SECTION = employeeDetails._SecDescrip;
                #endregion
                return View(model);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "A00ITConfirmationApproval");
                return View("Error");
            }
        }

        private SearchParameterList A00ITConfirmationApprovalSearchFilterData(SearchParameterList paramsList)
        {
            List<SearchParameterList> list = new List<SearchParameterList>();
            SearchParameterList data = new SearchParameterList();

            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

            #region Bind Ki drop down
            List<SearchParameterList> kiData = new List<SearchParameterList>();
            kiData.Add(new SearchParameterList() { SYKIID = 0, SYKI = "Select" });
            var kiData1 = _objHomeA00.GetA00SYKIList();
            var activeKi = kiData1._SYKIList.Where(x => x.ACTIVE == 1).FirstOrDefault();
            var kii = kiData1._SYKIList.OrderByDescending(x => x.SYKIID).Select(x => new SearchParameterList { SYKIID = x.SYKIID, SYKI = x.KICODE }).ToList();
            kiData.AddRange(kii);
            var selectedKi = (paramsList.SYKIID == 0 || paramsList.SYKIID == null) ? activeKi.SYKIID : (decimal)paramsList.SYKIID;
            ViewBag.SYKIData = new SelectList(kiData, "SYKIID", "SYKI", selectedKi);
            #endregion

            #region Bind status drop down
            data.StatusId = paramsList.StatusId;
            var statusList = new List<SearchParameterList>() {
            new SearchParameterList {
                StatusId = 0,
                Status = "Pending"
            },
            new SearchParameterList
            {
                StatusId = 1,
                Status = "Completed"
            }};
            //ViewBag.StatusList = new SelectList(statusList, "StatusId", "Status", paramsList.StatusId);
            ViewBag.StatusList = new SelectList(statusList.OrderBy(i => i.Status).ToList(), "StatusId", "Status", paramsList.StatusId);//changed by eshant 4-jul-22 for sorting
            #endregion

            data.ECode = paramsList.ECode;
            data.EmpName = paramsList.EmpName;
            data.OPERATIONID = paramsList.OPERATIONID;
            data.DIVISIONID = paramsList.DIVISIONID;
            data.SECTIONID = paramsList.SECTIONID;
            data.DEPARTMENTID = paramsList.DEPARTMENTID;
            data.SYKIID = selectedKi;

            #region Bind Login Employee Details
            _objA00SearchModel.ADEMPCODE = userId;
            _objA00SearchModel.SYKI = selectedKi;

            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");

            _objA00SearchModel.ADEMPCODE = userId;
            _objA00SearchModel.SYKIID = selectedKi;

            var vwITApproval = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS_ITApproval(_objA00SearchModel);

            var adorglevel = _objHomeA00.GetA00_IT_ADORGLEVELList(_objA00SearchModel);
            #endregion
            #region Check IT-Division Head
            var empDetails1 = vwITApproval._A00_VW_ASSOCIATELVLDETAILS_FULL.Join(adorglevel._ADOrgLevelList.AsEnumerable(),
               b => b.DIVISIONID, c => c.ADORGLEVELID, (b, c) => new { b, c }).FirstOrDefault();
            if (empDetails1 != null)
            {
                ViewBag.isITDivisionHead = true;
                data.isITDivisionHead = "Y";
            }
            else
            {
                ViewBag.isITDivisionHead = false;
                data.isITDivisionHead = "N";
            }
            #endregion

            list.Add(data);
            ViewBag.List = list;
            return data;
        }

        private void A00ITConfirmationApprovalGetA00SearchData(A00ViewModel model, SearchParameterList paramsData)
        {
            //SearchParameterList paramlist = new SearchParameterList();
            List<SearchResultList> empCodeList = new List<SearchResultList>();

            var userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

            model.SearchParams = new SearchParameterList();
            model.SearchParams.OPERATIONID = paramsData.OPERATIONID;
            model.SearchParams.DIVISIONID = paramsData.DIVISIONID;
            model.SearchParams.DEPARTMENTID = paramsData.DEPARTMENTID;
            model.SearchParams.SECTIONID = paramsData.SECTIONID;
            var sykiId = paramsData.SYKIID;//0;

            // SearchParams.StatusId
            model.SearchParams.StatusId = paramsData.StatusId;
            var empCode = string.IsNullOrEmpty(paramsData.ECode) ? 0 : Convert.ToInt64(paramsData.ECode);
            var empName = paramsData.EmpName;
            model.SearchParams.ECode = paramsData.ECode;
            model.SearchParams.EmpName = empName;
            model.SearchParams.SYKIID = sykiId;
            //paramlist.SYKIID = sykiId;
            //paramlist.ECode = paramsData.ECode;
            //paramlist.EmpName = empName;

            //paramlist.StatusId = model.SearchParams.StatusId;
            model.SearchParams.SYKI = paramsData.SYKI;
            model.SearchParams.Status = paramsData.Status;

            var status = model.SearchParams.StatusId;


            #region Filter Query
            var kiData1 = _objHomeA00.GetA00SYKIList();
            //.Join(category, ppc => ppc.pc.CatId, c => c.Id, (ppc, c) => new { ppc, c })
            //(  105 ---See)
            var resultList = _objHomeA00.GetA00APPROVALFullTable()._A00APPROVALList.
                Where(x => x.OHAPPDATE != null).AsEnumerable().
               Join(_objHomeA00.GetA00DtlFullTable()._A00DtlViewModelList,
               a => a.A00DTLTBID, b => b.A00DTLTBID, (a, b) => new { A00APPROVAL = a, A00DTLTB = b }).
               Join(_objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE, c => c.A00DTLTB.ADDEDBY, d => d.ADEMPCODE, (c, d) => new { c, d }).
               Join(_objHomeA00.GetA00AllocationList()._A00AllocationList, e => e.c.A00DTLTB.A00DTLTBID, al => al.A00ID, (e, al) => new { e, al })
               .Where(f =>
               (_objHomeA00.GetA00Allocation().Contains(f.e.c.A00DTLTB.A00DTLTBID)) &&
            (string.IsNullOrEmpty(empName) ? true : ((f.e.d.FIRSTNAME + " " + f.e.d.LASTNAME).Contains(empName))) &&
                                       (f.e.c.A00DTLTB.ADDEDBY == (empCode == 0 ? f.e.c.A00DTLTB.ADDEDBY : empCode) && (f.e.c.A00DTLTB.STATUSCD == 105))

            && ((f.al.ApplicationPICECode == userId) || (f.al.InfraPICECode == userId))
            && (f.e.c.A00DTLTB.SYKIID == (sykiId == 0 ? f.e.c.A00DTLTB.SYKIID : sykiId))
            && ((paramsData.OPERATIONID == 0 || paramsData.OPERATIONID == null) || f.e.c.A00DTLTB.OPERATION == paramsData.OPERATIONID)
            && ((paramsData.DIVISIONID == 0 || paramsData.DIVISIONID == null) || f.e.c.A00DTLTB.DIVISION == paramsData.DIVISIONID)
            && ((paramsData.DEPARTMENTID == 0 || paramsData.DEPARTMENTID == null) || f.e.c.A00DTLTB.DEPARTMENT == paramsData.DEPARTMENTID)
            && ((paramsData.SECTIONID == 0 || paramsData.SECTIONID == null) || f.e.c.A00DTLTB.SECTION == paramsData.SECTIONID)).
           Select(s1 => new SearchResultList
           {
               A00DTLTBID = s1.e.c.A00DTLTB.A00DTLTBID,
               SYKIID = s1.e.c.A00DTLTB.SYKIID,
               KICODE = kiData1._SYKIList.Where(x1 => x1.SYKIID == s1.e.c.A00DTLTB.SYKIID).Select(x1 => x1.KICODE).FirstOrDefault(),
               ECode = s1.e.c.A00DTLTB.ADDEDBY,
               EmpName = s1.e.d.FIRSTNAME + " " + s1.e.d.LASTNAME,
               ProjectTitle = s1.e.c.A00DTLTB.PRJCTTLE,
               RequestDate = s1.e.c.A00DTLTB.DATEADDEDID,
               ApprovedBy = _objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE.Where(x1 => x1.ADEMPCODE == s1.al.AllocatedBY).Select(x1 => x1.FIRSTNAME + " " + x1.LASTNAME).FirstOrDefault(),
               ApprovedOn = (s1.al.AllocatedDate),
               OPERATION = _objHomeA00.Getoperationdetails(s1.e.c.A00DTLTB.ADDEDBY, (int)s1.e.c.A00DTLTB.SYKIID)
               //IsDeficiencyClosed = _objHomeA00.IsA00DeficiencyClosed(s1.c.A00DTLTB.A00DTLTBID, s1.c.A00DTLTB.SYKIID)
           }).ToList();
            #endregion

            model.ResultList = resultList.OrderByDescending(x => x.RequestDate).ThenBy(x => x.ProjectTitle).ToList();

        }

        public ActionResult ViewITConfirmation(long A00DTLTBID, decimal SYKIID)
        {
            try
            {
                TempData["PageHead"] = "View IT Confirmation";
                ITConfirmationVM ITConfirmationVM = new ITConfirmationVM();
                ITConfirmationVM = _objHomeA00.GetA00ItConfirmationDetails(A00DTLTBID, SYKIID);
                if (ITConfirmationVM != null && ITConfirmationVM.A00ALLOCATIONID > 0)
                {
                    var UserPLName = _objHomeA00.GetEmpName((long)ITConfirmationVM.UserPL);
                    var ITPLName = _objHomeA00.GetEmpName((long)ITConfirmationVM.ITPL);
                    ITConfirmationVM.A00DTLTBID = A00DTLTBID;
                    ITConfirmationVM.UserPL_Name = ITConfirmationVM.UserPL + " / " + UserPLName;
                    ITConfirmationVM.ITPL_Name = ITConfirmationVM.ITPL + " / " + ITPLName;
                    ITConfirmationVM.SYKIID = ITConfirmationVM.SYKIID > 0 ? ITConfirmationVM.SYKIID : SYKIID;
                    var ITConfirmationApprovalDetails = _objHomeA00.GetA00ItConfirmationHistory(ITConfirmationVM.ITCONFIRMATIONID);
                    var LatestITConfirmationIds = ITConfirmationApprovalDetails.GroupBy(x => new
                    {
                        ACTIONBY = x.ACTIONBY
                    }).Select(x => x.Max(y => y.HistoryID)).ToList();
                    ViewBag.ITConfirmationDetails = ITConfirmationApprovalDetails.Where(x => LatestITConfirmationIds.Contains(x.HistoryID)).Select(x => new ITConfirmationApprovalHistoryListVM
                    {
                        ACTIONBY = x.ACTIONBY,
                        HistoryID = x.HistoryID,
                        A00ITCONFIRMATIONID = x.A00ITCONFIRMATIONID,
                        A00ID = x.A00ID,
                        SYKIID = x.SYKIID,
                        ACTIONTYPE = x.ACTIONTYPE,
                        STATUS = x.STATUS,
                        REMARKS = x.REMARKS,
                        EmpName = x.EmpName,
                        ACTIONON = x.ACTIONON,
                        ACTIVE = x.ACTIVE,
                        USERTYPE = x.USERTYPE,
                        ITCONFIRMATIONSUBMITTEDTO = x.ITCONFIRMATIONSUBMITTEDTO,
                        ITCONFSUBMITTEDTOUSERTYPE = x.ITCONFSUBMITTEDTOUSERTYPE,
                        ITCONFSUBMITTEDTOSTATUS = x.ITCONFSUBMITTEDTOSTATUS,
                        Designation = x.Designation,
                        PICType = x.PICType
                    }).ToList();
                    ViewBag.Application = ITConfirmationVM.Application;
                    ViewBag.HSDMCR = ITConfirmationVM.HSDM;
                }
                return View(ITConfirmationVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ViewITConfirmation");
                throw new Exception(ex.Message);
            }

        }
        public ActionResult ITConfirmationApproval(long A00DTLTBID, decimal SYKIID)
        {
            try
            {
                TempData["PageHead"] = "IT Confirmation Status View and Approval Workflow";
                var ITConfirmationVM = _objHomeA00.GetA00ItConfirmationDetails(A00DTLTBID, SYKIID);
                var UserPLName = _objHomeA00.GetEmpName((long)ITConfirmationVM.UserPL);
                var ITPLName = _objHomeA00.GetEmpName((long)ITConfirmationVM.ITPL);
                ITConfirmationVM.A00DTLTBID = A00DTLTBID;
                ITConfirmationVM.UserPL_Name = ITConfirmationVM.UserPL + " / " + UserPLName;
                ITConfirmationVM.ITPL_Name = ITConfirmationVM.ITPL + " / " + ITPLName;
                ITConfirmationVM.SYKIID = ITConfirmationVM.SYKIID > 0 ? ITConfirmationVM.SYKIID : SYKIID;
                ViewBag.ITConfirmationDetails = _objHomeA00.GetA00ItConfirmationHistory(ITConfirmationVM.ITCONFIRMATIONID);
                ViewBag.Application = ITConfirmationVM.Application;
                ViewBag.HSDMCR = ITConfirmationVM.HSDM;

                var UserId = Convert.ToInt32(_sessionService.Get<string>("userID"));
                var userData = _objHomeA00.getUserRole(UserId, SYKIID);
                var isDeptHead = userData.isDeptHead;
                var isCoOrdHead = userData.isCoOrdHead;
                var isDivHead = userData.isDivHead;
                var isExeCoOrdHead = userData.isExeCoOrdHead;
                var isOperatingHead = userData.isOperatingHead;
                ITConfirmationVM.IsOHLoggedIn = isOperatingHead;
                return View(ITConfirmationVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ITConfirmationApproval");
                throw new Exception(ex.Message);
            }

        }

        [HttpPost]
        public ActionResult ITConfirmationApproval(ITConfirmationVM model, IFormCollection fc)
        {
            try
            {
                A00SearchModel _objA00Search2ndPICModel = new A00SearchModel();
                //Added by Eshant 0n 27-06-2022 for getting and setting current Syki
                Int64 currentActiveKi = _objHomeA00.GetCurrentActiveKi();
                //

                var iCurrentActiveSYKI = _objHomeA00.GetA00ItConfirmationHistoryDetails(model.ITCONFIRMATIONID);
                var ITConfirmationApprovalHistory = _objHomeA00.GetA00ItConfirmationHistoryDetails(model.ITCONFIRMATIONID);
                var UserId = Convert.ToInt32(_sessionService.Get<string>("userID"));
                var MainPICUserId = Convert.ToInt32(_sessionService.Get<string>("userID"));
                var MainPICType = model.MAINPIC_ID;//1.App, 2.Infra
                var NotMainPICType = model.MAINPIC_ID == 1 ? 2 : 1;//1.App, 2.Infra
                var ApplicationPic = Convert.ToInt32(model.ApplicationPIC);
                var InfraPic = Convert.ToInt32(model.InfraStructurePIC);

                string nextApproverToEmail = string.Empty;
                string SendBackToEmail = string.Empty;

                InsertA00APPROVAL approval = new InsertA00APPROVAL();
                #region Main PIC
                bool isDeptHead = false, isCoOrdHead = false, isDivHead = false, isExeCoOrdHead = false, isOperatingHead = false;
                //Modified below line By Eshant on 27-06-22 to model.sykid to set current active syki
                model.SYKIID = currentActiveKi;
                _objA00SearchModel.ADEMPCODE = MainPICType == 1 ? ApplicationPic : InfraPic;// MainPICUserId;
                _objA00SearchModel.SYKI = model.SYKIID;
                var MainPICOperation = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel);
                _objA00SearchModel.OPERATIONID = MainPICOperation.OPERATIONID;


                var userData = _objHomeA00.getUserRole(MainPICUserId, model.SYKIID);
                isDeptHead = userData.isDeptHead;
                isCoOrdHead = userData.isCoOrdHead;
                isDivHead = userData.isDivHead;
                isExeCoOrdHead = userData.isExeCoOrdHead;
                isOperatingHead = userData.isOperatingHead;
                #endregion
                #region 2ndPIC
                _objA00Search2ndPICModel.ADEMPCODE = MainPICType == 1 ? InfraPic : ApplicationPic;
                _objA00Search2ndPICModel.SYKI = model.SYKIID;
                var Operation_2ndPIC_Details = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00Search2ndPICModel);
                _objA00Search2ndPICModel.OPERATIONID = Operation_2ndPIC_Details.OPERATIONID;
                #endregion
                var _adOrgLevelHead = _objHomeA00.GetA00_ADORGLEVELHEAD()._A00_ADORGLEVELHEAD;

                if (!string.IsNullOrEmpty(fc["btnApprove"]) && fc["btnApprove"].ToString().ToUpper() == "APPROVE")
                {
                    if (isDeptHead && ITConfirmationApprovalHistory.ITConfirmationSubmittedTo == UserId)
                    {
                        if (ITConfirmationApprovalHistory.ITCONFSUBMITTEDTOUSERTYPE == MainPICType)
                        {
                            if (ITConfirmationApprovalHistory.ITCONFSUBMITTEDTOSTATUS == 2)
                            {
                                //get Dept Head UserID for the app/infra PIC

                                if (MainPICType == 1)
                                {
                                    //get Dept Head userid based on infra PIC
                                    //InfraPic
                                    // if DeptHead is not found then get app coordinator based on ApplicationPic
                                    // if coordinator is not found then get app DivHead based on ApplicationPic
                                    // if DivHead is not found then get app ExCoordinator based on ApplicationPic
                                    // if ExCoordinator is not found then get app OH based on ApplicationPic

                                    //_objA00Search2ndPICModel.ADEMPCODE = InfraPic;
                                    //_objA00Search2ndPICModel.SYKI = model.SYKIID;
                                    //var Operation_2ndPIC_Details = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel);
                                    //Get 2nd PIC Dept. Head

                                    approval.DEPTHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == Operation_2ndPIC_Details.DEPARTMENTID && x.ISACTIVE == 1).Select(x => x.ADEMPCODE).FirstOrDefault(); // Dept Head as Approver
                                    if (approval.DEPTHDID == null || approval.DEPTHDID == 0)
                                    {
                                        //Main PIC COORDDID                                        
                                        approval.COORDDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).COORDINATOR;
                                        if (approval.COORDDID == null || approval.COORDDID == 0)
                                        {
                                            //Main PIC DIVHDHDID
                                            approval.DIVHDHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                            if (approval.DIVHDHDID == null || approval.DIVHDHDID == 0)
                                            {
                                                //2ndPIC DIVHDHD
                                                approval.DIVHDHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == Operation_2ndPIC_Details.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                                if (approval.DIVHDHDID == null || approval.DIVHDHDID == 0)
                                                {
                                                    //MainPIC EXECOHDID
                                                    approval.EXECOHDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).EXECOORDINATOR;
                                                    if (approval.EXECOHDID == null || approval.EXECOHDID == 0)
                                                    {
                                                        //MainPIC OH
                                                        approval.OHID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.OPERATIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                                        if (approval.OHID == null || approval.OHID == 0)
                                                        {

                                                        }
                                                        else
                                                        {
                                                            model.Status = 2;
                                                            model.UserType = 1;
                                                            model.ITCONFIRMATIONSUBMITTEDTO = approval.OHID;
                                                            model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                                            model.ITCONFSUBMITTEDTOSTATUS = 8;
                                                            _objA00SearchModel.OHID = approval.OHID;
                                                            TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.OHID).EMAILID;
                                                            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                            if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                            {
                                                                sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        model.Status = 2;
                                                        model.UserType = 1;
                                                        model.ITCONFIRMATIONSUBMITTEDTO = approval.EXECOHDID;
                                                        model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                                        model.ITCONFSUBMITTEDTOSTATUS = 7;
                                                        _objA00SearchModel.EXECOHDID = approval.EXECOHDID;
                                                        TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                        nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.EXECOHDID).EMAILID;
                                                        var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                        if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                        {
                                                            sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    model.Status = 2;
                                                    model.UserType = 1;
                                                    model.ITCONFIRMATIONSUBMITTEDTO = approval.DIVHDHDID;
                                                    model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                                    model.ITCONFSUBMITTEDTOSTATUS = 6;
                                                    _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                                    TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                    nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                                    var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                    if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                    {
                                                        sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                model.Status = 2;
                                                model.UserType = 1;
                                                model.ITCONFIRMATIONSUBMITTEDTO = approval.DIVHDHDID;
                                                model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                                model.ITCONFSUBMITTEDTOSTATUS = 5;
                                                _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                                TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                                var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                {
                                                    sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            model.Status = 2;
                                            model.UserType = 1;
                                            model.ITCONFIRMATIONSUBMITTEDTO = approval.COORDDID;
                                            model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                            model.ITCONFSUBMITTEDTOSTATUS = 4;
                                            _objA00SearchModel.COORDDID = approval.COORDDID;
                                            TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.COORDDID).EMAILID;
                                            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                            if (!string.IsNullOrEmpty(nextApproverToEmail))
                                            {
                                                sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (approval.DEPTHDID != MainPICUserId)
                                        {
                                            model.Status = 2;
                                            model.UserType = 1;
                                            model.ITCONFIRMATIONSUBMITTEDTO = approval.DEPTHDID;
                                            model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                            model.ITCONFSUBMITTEDTOSTATUS = 3;
                                            _objA00SearchModel.DEPTHDID = approval.DEPTHDID;
                                            TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DEPTHDID).EMAILID;
                                            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                            if (!string.IsNullOrEmpty(nextApproverToEmail))
                                            {
                                                sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                            }
                                        }
                                        else
                                        {
                                            //Main PIC COORDDID
                                            approval.COORDDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).COORDINATOR;
                                            if (approval.COORDDID == null || approval.COORDDID == 0)
                                            {
                                                //Main PIC DIVHDHDID
                                                approval.DIVHDHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                                if (approval.DIVHDHDID == null || approval.DIVHDHDID == 0)
                                                {
                                                    //2ndPIC DIVHDHD
                                                    approval.DIVHDHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == Operation_2ndPIC_Details.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                                    if (approval.DIVHDHDID == null || approval.DIVHDHDID == 0)
                                                    {
                                                        //MainPIC EXECOHDID
                                                        approval.EXECOHDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).EXECOORDINATOR;
                                                        if (approval.EXECOHDID == null || approval.EXECOHDID == 0)
                                                        {
                                                            //MainPIC OH
                                                            approval.OHID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.OPERATIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                                            if (approval.OHID == null || approval.OHID == 0)
                                                            {

                                                            }
                                                            else
                                                            {
                                                                model.Status = 2;
                                                                model.UserType = 1;
                                                                model.ITCONFIRMATIONSUBMITTEDTO = approval.OHID;
                                                                model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                                                model.ITCONFSUBMITTEDTOSTATUS = 8;
                                                                _objA00SearchModel.OHID = approval.OHID;
                                                                TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                                nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.OHID).EMAILID;
                                                                var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                                if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                                {
                                                                    sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            model.Status = 2;
                                                            model.UserType = 1;
                                                            model.ITCONFIRMATIONSUBMITTEDTO = approval.EXECOHDID;
                                                            model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                                            model.ITCONFSUBMITTEDTOSTATUS = 7;
                                                            _objA00SearchModel.EXECOHDID = approval.EXECOHDID;
                                                            TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.EXECOHDID).EMAILID;
                                                            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                            if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                            {
                                                                sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        model.Status = 2;
                                                        model.UserType = 1;
                                                        model.ITCONFIRMATIONSUBMITTEDTO = approval.DIVHDHDID;
                                                        model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                                        model.ITCONFSUBMITTEDTOSTATUS = 6;
                                                        _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                                        TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                        nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                                        var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                        if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                        {
                                                            sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    model.Status = 2;
                                                    model.UserType = 1;
                                                    model.ITCONFIRMATIONSUBMITTEDTO = approval.DIVHDHDID;
                                                    model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                                    model.ITCONFSUBMITTEDTOSTATUS = 5;
                                                    _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                                    TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                    nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                                    var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                    if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                    {
                                                        sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                model.Status = 2;
                                                model.UserType = 1;
                                                model.ITCONFIRMATIONSUBMITTEDTO = approval.COORDDID;
                                                model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                                model.ITCONFSUBMITTEDTOSTATUS = 4;
                                                _objA00SearchModel.COORDDID = approval.COORDDID;
                                                TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.COORDDID).EMAILID;
                                                var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                {
                                                    sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (MainPICType == 2)
                                {
                                    //get Dept Head userid based on app PIC
                                    //ApplicationPic
                                    // if DeptHead is not found then get infra coordinator based on InfraPic
                                    // if coordinator is not found then get infra DivHead based on InfraPic
                                    // if DivHead is not found then get infra ExCoordinator based on InfraPic
                                    // if ExCoordinator is not found then get infra OH based on InfraPic
                                    //_objA00SearchModel.ADEMPCODE = ApplicationPic;
                                    //_objA00SearchModel.SYKI = model.SYKIID;
                                    //var Operation_2ndPIC_Details = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel);
                                    //Get 2nd PIC Dept. Head
                                    approval.DEPTHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == Operation_2ndPIC_Details.DEPARTMENTID && x.ISACTIVE == 1).Select(x => x.ADEMPCODE).FirstOrDefault(); // Dept Head as Approver
                                    if (approval.DEPTHDID == null || approval.DEPTHDID == 0)
                                    {
                                        //Main PIC COORDDID
                                        approval.COORDDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).COORDINATOR;
                                        if (approval.COORDDID == null || approval.COORDDID == 0)
                                        {
                                            //Main PIC DIVHDHDID
                                            approval.DIVHDHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                            if (approval.DIVHDHDID == null || approval.DIVHDHDID == 0)
                                            {
                                                //2ndPIC DIVHDHD
                                                approval.DIVHDHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == Operation_2ndPIC_Details.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                                if (approval.DIVHDHDID == null || approval.DIVHDHDID == 0)
                                                {
                                                    //MainPIC EXECOHDID
                                                    approval.EXECOHDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).EXECOORDINATOR;
                                                    if (approval.EXECOHDID == null || approval.EXECOHDID == 0)
                                                    {
                                                        //MainPIC OH
                                                        approval.OHID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.OPERATIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                                        if (approval.OHID == null || approval.OHID == 0)
                                                        {

                                                        }
                                                        else
                                                        {
                                                            model.Status = 2;
                                                            model.UserType = 2;
                                                            model.ITCONFIRMATIONSUBMITTEDTO = approval.OHID;
                                                            model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                                            model.ITCONFSUBMITTEDTOSTATUS = 8;
                                                            _objA00SearchModel.OHID = approval.OHID;
                                                            TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.OHID).EMAILID;
                                                            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                            if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                            {
                                                                sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        model.Status = 2;
                                                        model.UserType = 2;
                                                        model.ITCONFIRMATIONSUBMITTEDTO = approval.EXECOHDID;
                                                        model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                                        model.ITCONFSUBMITTEDTOSTATUS = 7;
                                                        _objA00SearchModel.EXECOHDID = approval.EXECOHDID;
                                                        TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                        nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.EXECOHDID).EMAILID;
                                                        var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                        if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                        {
                                                            sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    model.Status = 2;
                                                    model.UserType = 2;
                                                    model.ITCONFIRMATIONSUBMITTEDTO = approval.DIVHDHDID;
                                                    model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                                    model.ITCONFSUBMITTEDTOSTATUS = 6;
                                                    _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                                    TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                    nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                                    var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                    if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                    {
                                                        sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                model.Status = 2;
                                                model.UserType = 2;
                                                model.ITCONFIRMATIONSUBMITTEDTO = approval.DIVHDHDID;
                                                model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                                model.ITCONFSUBMITTEDTOSTATUS = 5;
                                                _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                                TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                                var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                {
                                                    sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            model.Status = 2;
                                            model.UserType = 2;
                                            model.ITCONFIRMATIONSUBMITTEDTO = approval.COORDDID;
                                            model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                            model.ITCONFSUBMITTEDTOSTATUS = 4;
                                            _objA00SearchModel.COORDDID = approval.COORDDID;
                                            TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.COORDDID).EMAILID;
                                            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                            if (!string.IsNullOrEmpty(nextApproverToEmail))
                                            {
                                                sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (approval.DEPTHDID != MainPICUserId)
                                        {
                                            model.Status = 2;
                                            model.UserType = 2;
                                            model.ITCONFIRMATIONSUBMITTEDTO = approval.DEPTHDID;
                                            model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                            model.ITCONFSUBMITTEDTOSTATUS = 3;
                                            _objA00SearchModel.DEPTHDID = approval.DEPTHDID;
                                            TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DEPTHDID).EMAILID;
                                            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                            if (!string.IsNullOrEmpty(nextApproverToEmail))
                                            {
                                                sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                            }
                                        }
                                        else
                                        {
                                            //Main PIC COORDDID
                                            approval.COORDDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).COORDINATOR;
                                            if (approval.COORDDID == null || approval.COORDDID == 0)
                                            {
                                                //Main PIC DIVHDHDID
                                                approval.DIVHDHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                                if (approval.DIVHDHDID == null || approval.DIVHDHDID == 0)
                                                {
                                                    //2ndPIC DIVHDHD
                                                    approval.DIVHDHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == Operation_2ndPIC_Details.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                                    if (approval.DIVHDHDID == null || approval.DIVHDHDID == 0)
                                                    {
                                                        //MainPIC EXECOHDID
                                                        approval.EXECOHDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).EXECOORDINATOR;
                                                        if (approval.EXECOHDID == null || approval.EXECOHDID == 0)
                                                        {
                                                            //MainPIC OH
                                                            approval.OHID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.OPERATIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                                            if (approval.OHID == null || approval.OHID == 0)
                                                            {

                                                            }
                                                            else
                                                            {
                                                                model.Status = 2;
                                                                model.UserType = 2;
                                                                model.ITCONFIRMATIONSUBMITTEDTO = approval.OHID;
                                                                model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                                                model.ITCONFSUBMITTEDTOSTATUS = 8;
                                                                _objA00SearchModel.OHID = approval.OHID;
                                                                TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                                nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.OHID).EMAILID;
                                                                var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                                if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                                {
                                                                    sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            model.Status = 2;
                                                            model.UserType = 2;
                                                            model.ITCONFIRMATIONSUBMITTEDTO = approval.EXECOHDID;
                                                            model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                                            model.ITCONFSUBMITTEDTOSTATUS = 7;
                                                            _objA00SearchModel.EXECOHDID = approval.EXECOHDID;
                                                            TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.EXECOHDID).EMAILID;
                                                            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                            if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                            {
                                                                sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        model.Status = 2;
                                                        model.UserType = 2;
                                                        model.ITCONFIRMATIONSUBMITTEDTO = approval.DIVHDHDID;
                                                        model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                                        model.ITCONFSUBMITTEDTOSTATUS = 6;
                                                        _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                                        TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                        nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                                        var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                        if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                        {
                                                            sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    model.Status = 2;
                                                    model.UserType = 2;
                                                    model.ITCONFIRMATIONSUBMITTEDTO = approval.DIVHDHDID;
                                                    model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                                    model.ITCONFSUBMITTEDTOSTATUS = 5;
                                                    _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                                    TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                    nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                                    var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                    if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                    {
                                                        sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                model.Status = 2;
                                                model.UserType = 2;
                                                model.ITCONFIRMATIONSUBMITTEDTO = approval.COORDDID;
                                                model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                                model.ITCONFSUBMITTEDTOSTATUS = 4;
                                                _objA00SearchModel.COORDDID = approval.COORDDID;
                                                TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.COORDDID).EMAILID;
                                                var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                {
                                                    sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (ITConfirmationApprovalHistory.ITCONFSUBMITTEDTOUSERTYPE == NotMainPICType)
                        {
                            if (ITConfirmationApprovalHistory.ITCONFSUBMITTEDTOSTATUS == 3)
                            {
                                //get UserID for Coordinator

                                if (MainPICType == 1)
                                {
                                    //get next Coordinatorid based on login user; 
                                    //get ITCONFIRMATIONSUBMITTEDTO where A00ITConfirmationID=ITConfirmationApprovalHistory.A00ITConfirmationID and Status=2 for DivHead
                                    // if coordinator is not found then get app DivHead based on ApplicationPic
                                    // if DivHead is not found then get app ExCoordinator based on ApplicationPic
                                    // if ExCoordinator is not found then get app OH based on ApplicationPic
                                    //var DeptHeadID_2ndPIC = _objHomeA00.GetA00ItConfirmationHistory(model.ITCONFIRMATIONID).Where(x => x.STATUS == 2).OrderByDescending(x => x.HistoryID).Select(x => x.ITCONFIRMATIONSUBMITTEDTO).FirstOrDefault();
                                    //_objA00SearchModel.ADEMPCODE = (long)DeptHeadID_2ndPIC;//get ITCONFIRMATIONSUBMITTEDTO where A00ITConfirmationID=ITConfirmationApprovalHistory.A00ITConfirmationID and Status=2 for DivHead
                                    //_objA00SearchModel.SYKI = model.SYKIID;
                                    //var Operation_2ndPIC_Details = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel);
                                    //Main PIC COORDDID
                                    approval.COORDDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).COORDINATOR;
                                    if (approval.COORDDID == null || approval.COORDDID == 0)
                                    {
                                        //Main PIC DIVHDHDID
                                        approval.DIVHDHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                        if (approval.DIVHDHDID == null || approval.DIVHDHDID == 0)
                                        {
                                            //2ndPIC DIVHDHD
                                            approval.DIVHDHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == Operation_2ndPIC_Details.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                            if (approval.DIVHDHDID == null || approval.DIVHDHDID == 0)
                                            {
                                                //MainPIC EXECOHDID
                                                approval.EXECOHDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).EXECOORDINATOR;
                                                if (approval.EXECOHDID == null || approval.EXECOHDID == 0)
                                                {
                                                    //MainPIC OH
                                                    approval.OHID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.OPERATIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                                    if (approval.OHID == null || approval.OHID == 0)
                                                    {

                                                    }
                                                    else
                                                    {
                                                        model.Status = 3;
                                                        model.UserType = 2;
                                                        model.ITCONFIRMATIONSUBMITTEDTO = approval.OHID;
                                                        model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                                        model.ITCONFSUBMITTEDTOSTATUS = 8;
                                                        _objA00SearchModel.OHID = approval.OHID;
                                                        TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                        nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.OHID).EMAILID;
                                                        var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                        if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                        {
                                                            sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    model.Status = 3;
                                                    model.UserType = 2;
                                                    model.ITCONFIRMATIONSUBMITTEDTO = approval.EXECOHDID;
                                                    model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                                    model.ITCONFSUBMITTEDTOSTATUS = 7;
                                                    _objA00SearchModel.EXECOHDID = approval.EXECOHDID;
                                                    TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                    nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.EXECOHDID).EMAILID;
                                                    var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                    if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                    {
                                                        sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                model.Status = 3;
                                                model.UserType = 2;
                                                model.ITCONFIRMATIONSUBMITTEDTO = approval.DIVHDHDID;
                                                model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                                model.ITCONFSUBMITTEDTOSTATUS = 6;
                                                _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                                TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                                var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                {
                                                    sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            model.Status = 3;
                                            model.UserType = 2;
                                            model.ITCONFIRMATIONSUBMITTEDTO = approval.DIVHDHDID;
                                            model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                            model.ITCONFSUBMITTEDTOSTATUS = 5;
                                            _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                            TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                            if (!string.IsNullOrEmpty(nextApproverToEmail))
                                            {
                                                sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        model.Status = 3;
                                        model.UserType = 2;
                                        model.ITCONFIRMATIONSUBMITTEDTO = approval.COORDDID;
                                        model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                        model.ITCONFSUBMITTEDTOSTATUS = 4;
                                        _objA00SearchModel.COORDDID = approval.COORDDID;
                                        TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                        nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.COORDDID).EMAILID;
                                        var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                        if (!string.IsNullOrEmpty(nextApproverToEmail))
                                        {
                                            sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                        }
                                    }
                                }
                                else if (MainPICType == 2)
                                {
                                    //get next Coordinatorid based on infra PIC Dept Head ID; 
                                    //get ITCONFIRMATIONSUBMITTEDTO where A00ITConfirmationID=ITConfirmationApprovalHistory.A00ITConfirmationID and Status=2 for DivHead
                                    // if coordinator is not found then get infra DivHead based on InfraPic
                                    // if DivHead is not found then get infra ExCoordinator based on InfraPic
                                    // if ExCoordinator is not found then get infra OH based on InfraPic
                                    //var DeptHeadID_2ndPIC = _objHomeA00.GetA00ItConfirmationHistory(model.ITCONFIRMATIONID).Where(x => x.STATUS == 2).OrderByDescending(x => x.HistoryID).Select(x => x.ITCONFIRMATIONSUBMITTEDTO).FirstOrDefault();
                                    //_objA00SearchModel.ADEMPCODE = (long)DeptHeadID_2ndPIC;//get ITCONFIRMATIONSUBMITTEDTO where A00ITConfirmationID=ITConfirmationApprovalHistory.A00ITConfirmationID and Status=2 for DivHead
                                    //_objA00SearchModel.SYKI = model.SYKIID;
                                    //var Operation_2ndPIC_Details = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel);
                                    //Main PIC COORDDID
                                    approval.COORDDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).COORDINATOR;
                                    if (approval.COORDDID == null || approval.COORDDID == 0)
                                    {
                                        //Main PIC DIVHDHDID
                                        approval.DIVHDHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                        if (approval.DIVHDHDID == null || approval.DIVHDHDID == 0)
                                        {
                                            //2ndPIC DIVHDHD
                                            approval.DIVHDHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == Operation_2ndPIC_Details.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                            if (approval.DIVHDHDID == null || approval.DIVHDHDID == 0)
                                            {
                                                //MainPIC EXECOHDID
                                                approval.EXECOHDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).EXECOORDINATOR;
                                                if (approval.EXECOHDID == null || approval.EXECOHDID == 0)
                                                {
                                                    //MainPIC OH
                                                    approval.OHID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.OPERATIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                                    if (approval.OHID == null || approval.OHID == 0)
                                                    {

                                                    }
                                                    else
                                                    {
                                                        model.Status = 3;
                                                        model.UserType = 1;
                                                        model.ITCONFIRMATIONSUBMITTEDTO = approval.OHID;
                                                        model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                                        model.ITCONFSUBMITTEDTOSTATUS = 8;
                                                        _objA00SearchModel.OHID = approval.OHID;
                                                        TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                        nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.OHID).EMAILID;
                                                        var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                        if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                        {
                                                            sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    model.Status = 3;
                                                    model.UserType = 1;
                                                    model.ITCONFIRMATIONSUBMITTEDTO = approval.EXECOHDID;
                                                    model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                                    model.ITCONFSUBMITTEDTOSTATUS = 7;
                                                    _objA00SearchModel.EXECOHDID = approval.EXECOHDID;
                                                    TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                    nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.EXECOHDID).EMAILID;
                                                    var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                    if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                    {
                                                        sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                model.Status = 3;
                                                model.UserType = 1;
                                                model.ITCONFIRMATIONSUBMITTEDTO = approval.DIVHDHDID;
                                                model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                                model.ITCONFSUBMITTEDTOSTATUS = 6;
                                                _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                                TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                                var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                {
                                                    sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            model.Status = 3;
                                            model.UserType = 1;
                                            model.ITCONFIRMATIONSUBMITTEDTO = approval.DIVHDHDID;
                                            model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                            model.ITCONFSUBMITTEDTOSTATUS = 5;
                                            _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                            TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                            if (!string.IsNullOrEmpty(nextApproverToEmail))
                                            {
                                                sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        model.Status = 3;
                                        model.UserType = 1;
                                        model.ITCONFIRMATIONSUBMITTEDTO = approval.COORDDID;
                                        model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                        model.ITCONFSUBMITTEDTOSTATUS = 4;
                                        _objA00SearchModel.COORDDID = approval.COORDDID;
                                        TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                        nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.COORDDID).EMAILID;
                                        var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                        if (!string.IsNullOrEmpty(nextApproverToEmail))
                                        {
                                            sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (isCoOrdHead && ITConfirmationApprovalHistory.ITConfirmationSubmittedTo == UserId)
                    {
                        if (ITConfirmationApprovalHistory.ITCONFSUBMITTEDTOUSERTYPE == MainPICType)
                        {
                            if (ITConfirmationApprovalHistory.ITCONFSUBMITTEDTOSTATUS == 4)
                            {
                                //get Dept Head UserID for the app/infra PIC

                                if (MainPICType == 1)
                                {
                                    //get Dept Head userid based on infra PIC
                                    //InfraPic
                                    // if DeptHead is not found then get app coordinator based on ApplicationPic
                                    // if coordinator is not found then get app DivHead based on ApplicationPic
                                    // if DivHead is not found then get app ExCoordinator based on ApplicationPic
                                    // if ExCoordinator is not found then get app OH based on ApplicationPic

                                    //var DeptHeadID_2ndPIC = _objHomeA00.GetA00ItConfirmationHistory(model.ITCONFIRMATIONID).Where(x => x.STATUS == 2).OrderByDescending(x => x.HistoryID).Select(x => x.ITCONFIRMATIONSUBMITTEDTO).FirstOrDefault();
                                    //_objA00SearchModel.ADEMPCODE = (long)DeptHeadID_2ndPIC;//get ITCONFIRMATIONSUBMITTEDTO where A00ITConfirmationID=ITConfirmationApprovalHistory.A00ITConfirmationID and Status=2 for DivHead
                                    //_objA00SearchModel.SYKI = model.SYKIID;
                                    //var Operation_2ndPIC_Details = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel);
                                    //Main PIC DIVHDHDID
                                    approval.DIVHDHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                    if (approval.DIVHDHDID == null || approval.DIVHDHDID == 0)
                                    {
                                        //2ndPIC DIVHDHD
                                        approval.DIVHDHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == Operation_2ndPIC_Details.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                        if (approval.DIVHDHDID == null || approval.DIVHDHDID == 0)
                                        {
                                            //MainPIC EXECOHDID
                                            approval.EXECOHDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).EXECOORDINATOR;
                                            if (approval.EXECOHDID == null || approval.EXECOHDID == 0)
                                            {
                                                //MainPIC OH
                                                approval.OHID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.OPERATIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                                if (approval.OHID == null || approval.OHID == 0)
                                                {

                                                }
                                                else
                                                {
                                                    model.Status = 4;
                                                    model.UserType = 1;
                                                    model.ITCONFIRMATIONSUBMITTEDTO = approval.OHID;
                                                    model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                                    model.ITCONFSUBMITTEDTOSTATUS = 8;
                                                    _objA00SearchModel.OHID = approval.OHID;
                                                    TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                    nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.OHID).EMAILID;
                                                    var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                    if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                    {
                                                        sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                model.Status = 4;
                                                model.UserType = 1;
                                                model.ITCONFIRMATIONSUBMITTEDTO = approval.EXECOHDID;
                                                model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                                model.ITCONFSUBMITTEDTOSTATUS = 7;
                                                _objA00SearchModel.EXECOHDID = approval.EXECOHDID;
                                                TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.EXECOHDID).EMAILID;
                                                var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                {
                                                    sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            model.Status = 4;
                                            model.UserType = 1;
                                            model.ITCONFIRMATIONSUBMITTEDTO = approval.DIVHDHDID;
                                            model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                            model.ITCONFSUBMITTEDTOSTATUS = 6;
                                            _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                            TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                            if (!string.IsNullOrEmpty(nextApproverToEmail))
                                            {
                                                sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        model.Status = 4;
                                        model.UserType = 1;
                                        model.ITCONFIRMATIONSUBMITTEDTO = approval.DIVHDHDID;
                                        model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                        model.ITCONFSUBMITTEDTOSTATUS = 5;
                                        _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                        TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                        nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                        var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                        if (!string.IsNullOrEmpty(nextApproverToEmail))
                                        {
                                            sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                        }
                                    }
                                }
                                else if (MainPICType == 2)
                                {
                                    //get Dept Head userid based on app PIC
                                    //ApplicationPic
                                    // if DeptHead is not found then get infra coordinator based on InfraPic
                                    // if coordinator is not found then get infra DivHead based on InfraPic
                                    // if DivHead is not found then get infra ExCoordinator based on InfraPic
                                    // if ExCoordinator is not found then get infra OH based on InfraPic
                                    //var DeptHeadID_2ndPIC = _objHomeA00.GetA00ItConfirmationHistory(model.ITCONFIRMATIONID).Where(x => x.STATUS == 2).OrderByDescending(x => x.HistoryID).Select(x => x.ITCONFIRMATIONSUBMITTEDTO).FirstOrDefault();
                                    //_objA00SearchModel.ADEMPCODE = (long)DeptHeadID_2ndPIC;//get ITCONFIRMATIONSUBMITTEDTO where A00ITConfirmationID=ITConfirmationApprovalHistory.A00ITConfirmationID and Status=2 for DivHead
                                    //_objA00SearchModel.SYKI = model.SYKIID;
                                    //var Operation_2ndPIC_Details = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel);
                                    //Main PIC DIVHDHDID
                                    approval.DIVHDHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                    if (approval.DIVHDHDID == null || approval.DIVHDHDID == 0)
                                    {
                                        //2ndPIC DIVHDHD
                                        approval.DIVHDHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == Operation_2ndPIC_Details.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                        if (approval.DIVHDHDID == null || approval.DIVHDHDID == 0)
                                        {
                                            //MainPIC EXECOHDID
                                            approval.EXECOHDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).EXECOORDINATOR;
                                            if (approval.EXECOHDID == null || approval.EXECOHDID == 0)
                                            {
                                                //MainPIC OH
                                                approval.OHID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.OPERATIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                                if (approval.OHID == null || approval.OHID == 0)
                                                {

                                                }
                                                else
                                                {
                                                    model.Status = 4;
                                                    model.UserType = 2;
                                                    model.ITCONFIRMATIONSUBMITTEDTO = approval.OHID;
                                                    model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                                    model.ITCONFSUBMITTEDTOSTATUS = 8;
                                                    _objA00SearchModel.OHID = approval.OHID;
                                                    TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                    nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.OHID).EMAILID;
                                                    var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                    if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                    {
                                                        sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                model.Status = 4;
                                                model.UserType = 2;
                                                model.ITCONFIRMATIONSUBMITTEDTO = approval.EXECOHDID;
                                                model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                                model.ITCONFSUBMITTEDTOSTATUS = 7;
                                                _objA00SearchModel.EXECOHDID = approval.EXECOHDID;
                                                TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.EXECOHDID).EMAILID;
                                                var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                {
                                                    sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            model.Status = 4;
                                            model.UserType = 2;
                                            model.ITCONFIRMATIONSUBMITTEDTO = approval.DIVHDHDID;
                                            model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                            model.ITCONFSUBMITTEDTOSTATUS = 6;
                                            _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                            TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                            if (!string.IsNullOrEmpty(nextApproverToEmail))
                                            {
                                                sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        model.Status = 4;
                                        model.UserType = 2;
                                        model.ITCONFIRMATIONSUBMITTEDTO = approval.DIVHDHDID;
                                        model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                        model.ITCONFSUBMITTEDTOSTATUS = 5;
                                        _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                        TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                        nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                        var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                        if (!string.IsNullOrEmpty(nextApproverToEmail))
                                        {
                                            sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (isDivHead && ITConfirmationApprovalHistory.ITConfirmationSubmittedTo == UserId)
                    {
                        if (ITConfirmationApprovalHistory.ITCONFSUBMITTEDTOUSERTYPE == MainPICType)
                        {
                            if (ITConfirmationApprovalHistory.ITCONFSUBMITTEDTOSTATUS == 5)
                            {
                                //get Dept Head UserID for the app/infra PIC

                                if (MainPICType == 1)
                                {
                                    //get Dept Head userid based on infra PIC
                                    //InfraPic
                                    // if DeptHead is not found then get app coordinator based on ApplicationPic
                                    // if coordinator is not found then get app DivHead based on ApplicationPic
                                    // if DivHead is not found then get app ExCoordinator based on ApplicationPic
                                    // if ExCoordinator is not found then get app OH based on ApplicationPic

                                    //var DeptHeadID_2ndPIC = _objHomeA00.GetA00ItConfirmationHistory(model.ITCONFIRMATIONID).Where(x => x.STATUS == 2).OrderByDescending(x => x.HistoryID).Select(x => x.ITCONFIRMATIONSUBMITTEDTO).FirstOrDefault();
                                    //_objA00SearchModel.ADEMPCODE = (long)DeptHeadID_2ndPIC;//get ITCONFIRMATIONSUBMITTEDTO where A00ITConfirmationID=ITConfirmationApprovalHistory.A00ITConfirmationID and Status=2 for DivHead
                                    //_objA00SearchModel.SYKI = model.SYKIID;
                                    //var Operation_2ndPIC_Details = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel);
                                    //2ndPIC DIVHDHD
                                    approval.DIVHDHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == Operation_2ndPIC_Details.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                    if (approval.DIVHDHDID == null || approval.DIVHDHDID == 0)
                                    {
                                        //MainPIC EXECOHDID
                                        approval.EXECOHDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).EXECOORDINATOR;
                                        if (approval.EXECOHDID == null || approval.EXECOHDID == 0)
                                        {
                                            //MainPIC OH
                                            approval.OHID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.OPERATIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                            if (approval.OHID == null || approval.OHID == 0)
                                            {

                                            }
                                            else
                                            {
                                                model.Status = 5;
                                                model.UserType = 1;
                                                model.ITCONFIRMATIONSUBMITTEDTO = approval.OHID;
                                                model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                                model.ITCONFSUBMITTEDTOSTATUS = 8;
                                                _objA00SearchModel.OHID = approval.OHID;
                                                TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.OHID).EMAILID;
                                                var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                {
                                                    sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            model.Status = 5;
                                            model.UserType = 1;
                                            model.ITCONFIRMATIONSUBMITTEDTO = approval.EXECOHDID;
                                            model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                            model.ITCONFSUBMITTEDTOSTATUS = 7;
                                            _objA00SearchModel.EXECOHDID = approval.EXECOHDID;
                                            TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.EXECOHDID).EMAILID;
                                            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                            if (!string.IsNullOrEmpty(nextApproverToEmail))
                                            {
                                                sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (approval.DIVHDHDID != MainPICUserId)
                                        {
                                            model.Status = 5;
                                            model.UserType = 1;
                                            model.ITCONFIRMATIONSUBMITTEDTO = approval.DIVHDHDID;
                                            model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                            model.ITCONFSUBMITTEDTOSTATUS = 6;
                                            _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                            TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                            if (!string.IsNullOrEmpty(nextApproverToEmail))
                                            {
                                                sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                            }
                                        }
                                        else
                                        {
                                            //MainPIC EXECOHDID
                                            approval.EXECOHDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).EXECOORDINATOR;
                                            if (approval.EXECOHDID == null || approval.EXECOHDID == 0)
                                            {
                                                //MainPIC OH
                                                approval.OHID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.OPERATIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                                if (approval.OHID == null || approval.OHID == 0)
                                                {

                                                }
                                                else
                                                {
                                                    model.Status = 5;
                                                    model.UserType = 1;
                                                    model.ITCONFIRMATIONSUBMITTEDTO = approval.OHID;
                                                    model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                                    model.ITCONFSUBMITTEDTOSTATUS = 8;
                                                    _objA00SearchModel.OHID = approval.OHID;
                                                    TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                    nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.OHID).EMAILID;
                                                    var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                    if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                    {
                                                        sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                model.Status = 5;
                                                model.UserType = 1;
                                                model.ITCONFIRMATIONSUBMITTEDTO = approval.EXECOHDID;
                                                model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                                model.ITCONFSUBMITTEDTOSTATUS = 7;
                                                _objA00SearchModel.EXECOHDID = approval.EXECOHDID;
                                                TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.EXECOHDID).EMAILID;
                                                var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                {
                                                    sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (MainPICType == 2)
                                {
                                    //get Dept Head userid based on app PIC
                                    //ApplicationPic
                                    // if DeptHead is not found then get infra coordinator based on InfraPic
                                    // if coordinator is not found then get infra DivHead based on InfraPic
                                    // if DivHead is not found then get infra ExCoordinator based on InfraPic
                                    // if ExCoordinator is not found then get infra OH based on InfraPic
                                    //var DeptHeadID_2ndPIC = _objHomeA00.GetA00ItConfirmationHistory(model.ITCONFIRMATIONID).Where(x => x.STATUS == 2).OrderByDescending(x => x.HistoryID).Select(x => x.ITCONFIRMATIONSUBMITTEDTO).FirstOrDefault();
                                    //_objA00SearchModel.ADEMPCODE = (long)DeptHeadID_2ndPIC;//get ITCONFIRMATIONSUBMITTEDTO where A00ITConfirmationID=ITConfirmationApprovalHistory.A00ITConfirmationID and Status=2 for DivHead
                                    //_objA00SearchModel.SYKI = model.SYKIID;
                                    //var Operation_2ndPIC_Details = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel);
                                    //2ndPIC DIVHDHD
                                    approval.DIVHDHDID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == Operation_2ndPIC_Details.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                    if (approval.DIVHDHDID == null || approval.DIVHDHDID == 0)
                                    {
                                        //MainPIC EXECOHDID
                                        approval.EXECOHDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).EXECOORDINATOR;
                                        if (approval.EXECOHDID == null || approval.EXECOHDID == 0)
                                        {
                                            //MainPIC OH
                                            approval.OHID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.OPERATIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                            if (approval.OHID == null || approval.OHID == 0)
                                            {

                                            }
                                            else
                                            {
                                                model.Status = 5;
                                                model.UserType = 2;
                                                model.ITCONFIRMATIONSUBMITTEDTO = approval.OHID;
                                                model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                                model.ITCONFSUBMITTEDTOSTATUS = 8;
                                                _objA00SearchModel.OHID = approval.OHID;
                                                TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.OHID).EMAILID;
                                                var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                {
                                                    sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            model.Status = 5;
                                            model.UserType = 2;
                                            model.ITCONFIRMATIONSUBMITTEDTO = approval.EXECOHDID;
                                            model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                            model.ITCONFSUBMITTEDTOSTATUS = 7;
                                            _objA00SearchModel.EXECOHDID = approval.EXECOHDID;
                                            TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.EXECOHDID).EMAILID;
                                            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                            if (!string.IsNullOrEmpty(nextApproverToEmail))
                                            {
                                                sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (approval.DIVHDHDID != MainPICUserId)
                                        {
                                            model.Status = 5;
                                            model.UserType = 2;
                                            model.ITCONFIRMATIONSUBMITTEDTO = approval.DIVHDHDID;
                                            model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                            model.ITCONFSUBMITTEDTOSTATUS = 6;
                                            _objA00SearchModel.DIVHDHDID = approval.DIVHDHDID;
                                            TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.DIVHDHDID).EMAILID;
                                            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                            if (!string.IsNullOrEmpty(nextApproverToEmail))
                                            {
                                                sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                            }
                                        }
                                        else
                                        {
                                            //MainPIC EXECOHDID
                                            approval.EXECOHDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).EXECOORDINATOR;
                                            if (approval.EXECOHDID == null || approval.EXECOHDID == 0)
                                            {
                                                //MainPIC OH
                                                approval.OHID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.OPERATIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                                if (approval.OHID == null || approval.OHID == 0)
                                                {

                                                }
                                                else
                                                {
                                                    model.Status = 5;
                                                    model.UserType = 2;
                                                    model.ITCONFIRMATIONSUBMITTEDTO = approval.OHID;
                                                    model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                                    model.ITCONFSUBMITTEDTOSTATUS = 8;
                                                    _objA00SearchModel.OHID = approval.OHID;
                                                    TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                    nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.OHID).EMAILID;
                                                    var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                    if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                    {
                                                        sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                model.Status = 5;
                                                model.UserType = 2;
                                                model.ITCONFIRMATIONSUBMITTEDTO = approval.EXECOHDID;
                                                model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                                model.ITCONFSUBMITTEDTOSTATUS = 7;
                                                _objA00SearchModel.EXECOHDID = approval.EXECOHDID;
                                                TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                                nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.EXECOHDID).EMAILID;
                                                var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                                if (!string.IsNullOrEmpty(nextApproverToEmail))
                                                {
                                                    sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (ITConfirmationApprovalHistory.ITCONFSUBMITTEDTOUSERTYPE == NotMainPICType)
                        {
                            if (ITConfirmationApprovalHistory.ITCONFSUBMITTEDTOSTATUS == 6)
                            {
                                //get UserID for Coordinator

                                if (MainPICType == 1)
                                {
                                    //get next Coordinatorid based on app PIC Dept Head ID; get ActionBY(App Dept Head) where A00ITConfirmationID=ITConfirmationApprovalHistory.A00ITConfirmationID and Status=2
                                    // if coordinator is not found then get app DivHead based on ApplicationPic
                                    // if DivHead is not found then get app ExCoordinator based on ApplicationPic
                                    // if ExCoordinator is not found then get app OH based on ApplicationPic

                                    //MainPIC EXECOHDID
                                    approval.EXECOHDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).EXECOORDINATOR;
                                    if (approval.EXECOHDID == null || approval.EXECOHDID == 0)
                                    {
                                        //MainPIC OH
                                        approval.OHID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.OPERATIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                        if (approval.OHID == null || approval.OHID == 0)
                                        {

                                        }
                                        else
                                        {
                                            model.Status = 6;
                                            model.UserType = 2;
                                            model.ITCONFIRMATIONSUBMITTEDTO = approval.OHID;
                                            model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                            model.ITCONFSUBMITTEDTOSTATUS = 8;
                                            _objA00SearchModel.OHID = approval.OHID;
                                            TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.OHID).EMAILID;
                                            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                            if (!string.IsNullOrEmpty(nextApproverToEmail))
                                            {
                                                sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        model.Status = 6;
                                        model.UserType = 2;
                                        model.ITCONFIRMATIONSUBMITTEDTO = approval.EXECOHDID;
                                        model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                        model.ITCONFSUBMITTEDTOSTATUS = 7;
                                        _objA00SearchModel.EXECOHDID = approval.EXECOHDID;
                                        TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                        nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.EXECOHDID).EMAILID;
                                        var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                        if (!string.IsNullOrEmpty(nextApproverToEmail))
                                        {
                                            sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                        }
                                    }
                                }
                                else if (MainPICType == 2)
                                {
                                    //get next Coordinatorid based on infra PIC Dept Head ID; get ActionBY(infra Dept Head) where A00ITConfirmationID=ITConfirmationApprovalHistory.A00ITConfirmationID and Status=2
                                    // if coordinator is not found then get infra DivHead based on InfraPic
                                    // if DivHead is not found then get infra ExCoordinator based on InfraPic
                                    // if ExCoordinator is not found then get infra OH based on InfraPic

                                    //MainPIC EXECOHDID
                                    approval.EXECOHDID = _objHomeA00.getA00_ADORGCOORDINATOR(_objA00SearchModel).EXECOORDINATOR;
                                    if (approval.EXECOHDID == null || approval.EXECOHDID == 0)
                                    {
                                        //MainPIC OH
                                        approval.OHID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.OPERATIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                        if (approval.OHID == null || approval.OHID == 0)
                                        {

                                        }
                                        else
                                        {
                                            model.Status = 6;
                                            model.UserType = 1;
                                            model.ITCONFIRMATIONSUBMITTEDTO = approval.OHID;
                                            model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                            model.ITCONFSUBMITTEDTOSTATUS = 8;
                                            _objA00SearchModel.OHID = approval.OHID;
                                            TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                            nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.OHID).EMAILID;
                                            var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                            if (!string.IsNullOrEmpty(nextApproverToEmail))
                                            {
                                                sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        model.Status = 6;
                                        model.UserType = 1;
                                        model.ITCONFIRMATIONSUBMITTEDTO = approval.EXECOHDID;
                                        model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                        model.ITCONFSUBMITTEDTOSTATUS = 7;
                                        _objA00SearchModel.EXECOHDID = approval.EXECOHDID;
                                        TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                        nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.EXECOHDID).EMAILID;
                                        var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                        if (!string.IsNullOrEmpty(nextApproverToEmail))
                                        {
                                            sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (isExeCoOrdHead && ITConfirmationApprovalHistory.ITConfirmationSubmittedTo == UserId)
                    {
                        if (ITConfirmationApprovalHistory.ITCONFSUBMITTEDTOUSERTYPE == MainPICType)
                        {
                            if (ITConfirmationApprovalHistory.ITCONFSUBMITTEDTOSTATUS == 7)
                            {
                                //get Dept Head UserID for the app/infra PIC

                                if (MainPICType == 1)
                                {
                                    //get Dept Head userid based on infra PIC
                                    //InfraPic
                                    // if DeptHead is not found then get app coordinator based on ApplicationPic
                                    // if coordinator is not found then get app DivHead based on ApplicationPic
                                    // if DivHead is not found then get app ExCoordinator based on ApplicationPic
                                    // if ExCoordinator is not found then get app OH based on ApplicationPic

                                    //MainPIC OH
                                    approval.OHID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.OPERATIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                    if (approval.OHID == null || approval.OHID == 0)
                                    {

                                    }
                                    else
                                    {
                                        model.Status = 7;
                                        model.UserType = 1;
                                        model.ITCONFIRMATIONSUBMITTEDTO = approval.OHID;
                                        model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                        model.ITCONFSUBMITTEDTOSTATUS = 8;
                                        _objA00SearchModel.OHID = approval.OHID;
                                        TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                        nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.OHID).EMAILID;
                                        var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                        if (!string.IsNullOrEmpty(nextApproverToEmail))
                                        {
                                            sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                        }
                                    }
                                }
                                else if (MainPICType == 2)
                                {
                                    //get Dept Head userid based on app PIC
                                    //ApplicationPic
                                    // if DeptHead is not found then get infra coordinator based on InfraPic
                                    // if coordinator is not found then get infra DivHead based on InfraPic
                                    // if DivHead is not found then get infra ExCoordinator based on InfraPic
                                    // if ExCoordinator is not found then get infra OH based on InfraPic
                                    //MainPIC OH
                                    approval.OHID = _adOrgLevelHead.Where(x => x.ADORGLEVELID == MainPICOperation.OPERATIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                                    if (approval.OHID == null || approval.OHID == 0)
                                    {

                                    }
                                    else
                                    {
                                        model.Status = 7;
                                        model.UserType = 2;
                                        model.ITCONFIRMATIONSUBMITTEDTO = approval.OHID;
                                        model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                        model.ITCONFSUBMITTEDTOSTATUS = 8;
                                        _objA00SearchModel.OHID = approval.OHID;
                                        TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                        nextApproverToEmail = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(approval.OHID).EMAILID;
                                        var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                        if (!string.IsNullOrEmpty(nextApproverToEmail))
                                        {
                                            sendMailToNextLevelAfterITConfirmationApproval(nextApproverToEmail, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (isOperatingHead && ITConfirmationApprovalHistory.ITConfirmationSubmittedTo == UserId)
                    {
                        if (ITConfirmationApprovalHistory.ITCONFSUBMITTEDTOUSERTYPE == MainPICType)
                        {
                            if (ITConfirmationApprovalHistory.ITCONFSUBMITTEDTOSTATUS == 8)
                            {
                                //get Dept Head UserID for the app/infra PIC

                                if (MainPICType == 1)
                                {
                                    //get Dept Head userid based on infra PIC
                                    //InfraPic
                                    // if DeptHead is not found then get app coordinator based on ApplicationPic
                                    // if coordinator is not found then get app DivHead based on ApplicationPic
                                    // if DivHead is not found then get app ExCoordinator based on ApplicationPic
                                    // if ExCoordinator is not found then get app OH based on ApplicationPic

                                    //MainPIC OH
                                    model.Status = 8;
                                    model.UserType = 1;
                                    model.ITCONFIRMATIONSUBMITTEDTO = approval.OHID;
                                    model.IsOHLoggedIn = true;
                                    //model.ITCONFSUBMITTEDTOUSERTYPE = 1;
                                    _objA00SearchModel.OHID = approval.OHID;
                                    TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                    var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                    if (!string.IsNullOrEmpty(requesterDetails.EMAILID))
                                    {
                                        sendMailToNextLevelAfterITConfirmationApproval(requesterDetails.EMAILID, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                    }
                                }
                                else if (MainPICType == 2)
                                {
                                    //get Dept Head userid based on app PIC
                                    //ApplicationPic
                                    // if DeptHead is not found then get infra coordinator based on InfraPic
                                    // if coordinator is not found then get infra DivHead based on InfraPic
                                    // if DivHead is not found then get infra ExCoordinator based on InfraPic
                                    // if ExCoordinator is not found then get infra OH based on InfraPic
                                    //MainPIC OH
                                    model.Status = 8;
                                    model.UserType = 2;
                                    model.ITCONFIRMATIONSUBMITTEDTO = approval.OHID;
                                    model.IsOHLoggedIn = true;
                                    //model.ITCONFSUBMITTEDTOUSERTYPE = 2;
                                    _objA00SearchModel.OHID = approval.OHID;
                                    TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnApprove"].ToString().ToUpper(), _objA00SearchModel);

                                    var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                                    if (!string.IsNullOrEmpty(requesterDetails.EMAILID))
                                    {
                                        sendMailToNextLevelAfterITConfirmationApproval(requesterDetails.EMAILID, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                                    }
                                }
                            }
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(fc["btnSendBack"]) && fc["btnSendBack"].ToString().ToUpper() == "SEND BACK")
                {
                    model.Status = 9;
                    model.UserType = (short)MainPICType;
                    model.ITCONFIRMATIONSUBMITTEDTO = MainPICType == 1 ? ApplicationPic : InfraPic;
                    model.ITCONFSUBMITTEDTOUSERTYPE = (short)MainPICType;
                    model.ITCONFSUBMITTEDTOSTATUS = 1;
                    _objA00SearchModel.ADEMPCODE = MainPICType == 1 ? ApplicationPic : InfraPic;
                    TempData["result"] = _objHomeA00.ITConfirmationSaveData(model, MainPICUserId, fc["btnSendBack"].ToString().ToUpper(), _objA00SearchModel);

                    var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(_objA00SearchModel.ADEMPCODE);
                    if (!string.IsNullOrEmpty(requesterDetails.EMAILID))
                    {
                        sendMailForITConfirmationSendBack(requesterDetails.EMAILID, model.ProjectName, DateTime.Now, requesterDetails.Name, requesterDetails.EmpCode);
                    }
                }
                return RedirectToAction("ITConfirmationApprovalRequest");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ITConfirmationApproval");
                throw new Exception(ex.Message);
            }
        }

        private void sendMailToNextLevelAfterITConfirmationFormFilled(string receiver, string project_title, DateTime requested_on, string RequesterName, string RequesterEMPCode)
        {
            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = receiver;
            string strSubject = "IT Confirmation Request is Submitted by - " + RequesterName + " - Emp Code(" + RequesterEMPCode + ")";
            string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                             "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>IT Confirmation Request is Submitted by -  " + RequesterName + " - Emp Code (" + RequesterEMPCode + ")</font></b></td>" +
                             "</tr><tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td width=125 height=22 valign=top>Employee Code</td><td width=389 valign=top>" + RequesterEMPCode + "</td>" +
                             "</tr><tr><td width=125 height=23 valign=top>Employee Name</td><td width=389 valign=top>" + RequesterName + "</td>" +
                             " </tr><tr> " +
                             "<td width=125 valign=top>Project Title</td><td width=389 valign=top>" + project_title + "</td></tr><tr><td width=125 valign=top>Created on</td>" +
                             "<td width=389 valign=top>" + requested_on.Date.ToString("D") + "</td></tr>" +
                             "<tr><td valign=top colspan=2>Please login <a href='https://portal.honda2wheelersindia.com/SSO'> Employee Portal</a> for approval process.</td></tr>" +
                             "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }
        private void sendMailToNextLevelAfterITConfirmationApproval(string receiver, string project_title, DateTime requested_on, string RequesterName, string RequesterEMPCode)
        {
            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = receiver;
            string userName = _sessionService.Get<string>("userName");
            string userID = _sessionService.Get<string>("userID");
            string strSubject = "IT Confirmation Request has been approved by - " + userName + " - Emp Code - " + userID;
            string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                             "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>IT Confirmation Request has been approved by -  " + userName + " - Emp Code (" + userID + ")</font></b></td>" +
                             "</tr><tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td width=125 height=22 valign=top>Employee Code</td><td width=389 valign=top>" + RequesterEMPCode + "</td>" +
                             "</tr><tr><td width=125 height=23 valign=top>Employee Name</td><td width=389 valign=top>" + RequesterName + "</td>" +
                             " </tr><tr> " +
                             "<td width=125 valign=top>Project Title</td><td width=389 valign=top>" + project_title + "</td></tr><tr><td width=125 valign=top>Requested on</td>" +
                             "<td width=389 valign=top>" + requested_on.Date.ToString("D") + "</td></tr>" +
                             "<tr><td valign=top colspan=2>Please login <a href='https://portal.honda2wheelersindia.com/SSO'> Employee Portal</a> for approval process.</td></tr>" +
                             "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }

        private void sendMailForITConfirmationSendBack(string receiver, string project_title, DateTime requested_on, string RequesterName, string RequesterEMPCode)
        {
            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = receiver;
            string userName = _sessionService.Get<string>("userName");
            string userID = _sessionService.Get<string>("userID");
            string strSubject = "IT Confirmation Request is Send Back by - " + userName + " - Emp Code - " + userID;
            string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                             "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>IT Confirmation Request is Send Back by - " + userName + " - Emp Code (" + userID + ")</font></b></td>" +
                             "</tr><tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td width=125 height=22 valign=top>Employee Code</td><td width=389 valign=top>" + RequesterEMPCode + "</td>" +
                             "</tr><tr><td width=125 height=23 valign=top>Employee Name</td><td width=389 valign=top>" + RequesterName + "</td>" +
                             " </tr><tr> " +
                             "<td width=125 valign=top>Project Title</td><td width=389 valign=top>" + project_title + "</td></tr><tr><td width=125 valign=top>Rejected on</td>" +
                             "<td width=389 valign=top>" + requested_on.Date.ToString("D") + "</td></tr>" +
                             "<tr><td valign=top colspan=2>Please login <a href='https://portal.honda2wheelersindia.com/SSO'> Employee Portal</a> for approval process.</td></tr>" +
                             "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }

        #endregion

        #endregion


        //Dinesh
        #region A00 project Update
        public ActionResult A00ProjectStatusUpdatedApprovalList()
        {
            TempData["PageHead"] = "A00 IT Confirmation Approved list : Project Status Update";
            SearchParameterList paramsList = new SearchParameterList();
            A00ViewModel model = new A00ViewModel();
            try
            {
                var paramData = ITConfirmationgetSearchFilterData(new SearchParameterList());
                //model.SearchParams.SYKI = paramData.SYKIID;
                //model.SearchParams.Status = paramData.Status;

                ApprovedITConfirmationSearchData(model, paramData);

                List<A00ADORGLEVELList> _opList = _objHomeA00.GetA00ADORGLEVELList((decimal)paramData.SYKIID)._ADOrgLevelList;
                ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.DivList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.DepList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.SecList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "A00ProjectStatusUpdatedApprovalList");
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult A00ProjectStatusUpdatedApprovalList(A00ViewModel model, string BtnExport)
        {
            TempData["PageHead"] = "A00 IT Confirmation Approved list : Project Status Update";
            SearchParameterList paramlist = new SearchParameterList();

            //Employee_Details employeeDetails = (Employee_Details)Session["Employee"];
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");

            var sykiId = 0;
            if (!string.IsNullOrEmpty(Request.Form["SearchParams.SYKI"].ToString()))
            {
                sykiId = Convert.ToInt32(Request.Form["SearchParams.SYKI"].ToString());
            }
            paramlist.SYKIID = sykiId;
            paramlist.ECode = Request.Form["SearchParams.ECode"].ToString();
            paramlist.EmpName = Request.Form["SearchParams.EmpName"].ToString();
            //paramlist.StatusId = Convert.ToInt16(Request.Query["SearchParams.Status"].ToString());
            paramlist.StatusId = Convert.ToInt16(Request.Form["SearchParams.Status"].ToString());

            paramlist.OPERATIONID = Request.Form["SearchParams.OPERATIONID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.OPERATIONID"]) : 0;
            paramlist.DIVISIONID = Request.Form["SearchParams.DIVISIONID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.DIVISIONID"]) : 0;
            paramlist.DEPARTMENTID = Request.Form["SearchParams.DEPARTMENTID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.DEPARTMENTID"]) : 0;
            paramlist.SECTIONID = Request.Form["SearchParams.SECTIONID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.SECTIONID"]) : 0;

            #region Block-1
            var userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

            model.SearchParams = new SearchParameterList();
            model.SearchParams.StatusId = paramlist.StatusId;
            var empCode = string.IsNullOrEmpty(Request.Form["SearchParams.ECode"].ToString()) ? 0 : Convert.ToInt64(Request.Form["SearchParams.ECode"].ToString());
            var empName = Request.Form["SearchParams.EmpName"].ToString();
            model.SearchParams.ECode = Request.Form["SearchParams.ECode"].ToString();
            model.SearchParams.EmpName = empName;
            model.SearchParams.SYKIID = sykiId;
            #endregion

            var paramData = ITConfirmationgetSearchFilterData(paramlist);

            model.SearchParams.SYKI = paramData.SYKI;
            model.SearchParams.Status = paramData.Status;

            ApprovedITConfirmationSearchData(model, paramData);

            #region added by kiran

            //List<ADORGLEVEL>

            List<A00ADORGLEVELList> _opList = _objHomeA00.GetA00ADORGLEVELList((decimal)sykiId)._ADOrgLevelList;
            ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP", paramlist.OPERATIONID);

            List<SearchParameterList> _divList = _objHomeA00.BindDivision(paramlist.OPERATIONID, sykiId);
            ViewBag.DivList = new SelectList(_divList, "DIVISIONID", "DIVISION", paramlist.DIVISIONID);

            List<SearchParameterList> _depList = _objHomeA00.BindDepartment(paramlist.DIVISIONID, 0, sykiId);
            ViewBag.DepList = new SelectList(_depList, "DEPARTMENTID", "DEPARTMENT", paramlist.DEPARTMENTID);


            List<SearchParameterList> _secList = _objHomeA00.BindSection(paramlist.DEPARTMENTID, 0, 0, sykiId);
            ViewBag.SecList = new SelectList(_secList, "SECTIONID", "SECTION", paramlist.SECTIONID);

            paramlist.OPERATIONID = employeeDetails._OpId;
            paramlist.OPERATION = employeeDetails._OpDesc;

            paramlist.DIVISIONID = employeeDetails._DivId;
            paramlist.DIVISION = employeeDetails._DivDesc;

            paramlist.DEPARTMENTID = employeeDetails._DepId;
            paramlist.DEPARTMENT = employeeDetails._DepDesc;

            paramlist.SECTIONID = employeeDetails._SecId;
            paramlist.SECTION = employeeDetails._SecDescrip;
            #endregion

            if (BtnExport == "Export to excel")
            {
                try
                {
                    //List<SearchResultList> a00ActivityDataforExcel = model.ResultList.Select(x => (new SearchResultList { KI_Code = x.KICODE, E_Code = x.ECode, Name = x.EmpName, Start_Date = x.ActivityStartDate, End_Date = x.ActivityEndDate, Status = x.ActivitySchedule, Remark = x.ActivityRemark, Activity_Title = x.ActivityTitle, STATUSCD = x.STATUSCD, ActivityDetail = x.ActivityDetail, ActivityRemarkDate = x.ActivityRemarkDate })).ToList();

                    string ExcelName = "A00 Project Status Update-" + Convert.ToString(DateTime.Now).Replace(" ", "_").Replace(":", "_").Replace("/", "_") + ".xlsx";
                    System.Data.DataTable dt = new System.Data.DataTable("A00 Project Status Update");
                    dt.Columns.AddRange(new DataColumn[11] {
                        new DataColumn("S.No"),
                        new DataColumn("KI Code"),
                        new DataColumn("Operation"),
                        new DataColumn("Project Code"),
                        new DataColumn("Project Title"),
                        new DataColumn("A00 Approval Date"),
                        new DataColumn("A00 Assigned Date"),
                        new DataColumn("IT Confirmation Date"),
                        new DataColumn("IT Confirmation Vs A00 Approval"),
                        new DataColumn("HSDM/CR"),
                        new DataColumn("Current Stage")
                    });
                    int intCntr = 0;
                    foreach (var item in model.ResultList)
                    {
                        intCntr++;
                        var DaysDiffBetA00ApprovalAndITConfirmation = item.ITConfirmationDate != null ? (Convert.ToDateTime(item.ITConfirmationDate) - Convert.ToDateTime(item.A00ApprovedOn)).Days + " Days" : (DateTime.Now - Convert.ToDateTime(item.A00ApprovedOn)).Days + " Days";

                        //Commented on 10-July-2021
                        //var currentStage = string.IsNullOrEmpty(item.ProjectStage) && item.HSDMCR == "HSDM" ? "U0" : (string.IsNullOrEmpty(item.ProjectStage) && item.HSDMCR == "CR" ? "RFQ" : item.ProjectStage);


                        //Change start on 10-July-2021
                        var projectStatus = item.ProjectUpdateStatus == 1 ? "Pending" : item.ProjectUpdateStatus == 2 ? "InProcess" : item.ProjectUpdateStatus == 3 ? "Hold" : item.ProjectUpdateStatus == 4 ? "Completed" : item.ProjectUpdateStatus == 5 ? "Not Applicable" : "Pending";
                        var currentStage = string.IsNullOrEmpty(item.ProjectStage) && item.HSDMCR == "HSDM" ? "U0" : (string.IsNullOrEmpty(item.ProjectStage) && item.HSDMCR == "CR" ? "RFQ" : item.ProjectStage);
                        //Change start on 22-July-2021
                        //currentStage = item.IsAllocationExist == false ? "Allocation Pending" : (item.ITConfirmationStatus != 8 ? "IT Confirmation Pending" :
                        //    (projectStatus != "" ? (currentStage != "-" ? currentStage + " - " + projectStatus : projectStatus) : (currentStage != "" ? currentStage : "")));
                        currentStage = item.STATUSCD == 125 ? "Sent back (Convert to CR)" : (item.IsAllocationExist == false ? "Allocation Pending" : (item.ITConfirmationStatus != 8 ? "IT Confirmation Pending" :
                            (projectStatus != "" ? (currentStage != "-" ? currentStage + " - " + projectStatus : projectStatus) : (currentStage != "" ? currentStage : ""))));
                        //Change end on 22-July-2021
                        //Change end on 10-July-2021

                        var A00ApprovedOn = item.A00ApprovedOn != null ? item.A00ApprovedOn.Value.ToString("yyyy-MM-dd") : "";
                        var ApprovedOn = item.ApprovedOn != null ? item.ApprovedOn.Value.ToString("yyyy-MM-dd") : "";
                        var ITConfirmationDate = item.ITConfirmationDate != null ? item.ITConfirmationDate.Value.ToString("yyyy-MM-dd") : "";

                        dt.Rows.Add(intCntr, item.KICODE, item.OPERATION, item.ProjectCode, item.ProjectTitle, A00ApprovedOn,
                            ApprovedOn, ITConfirmationDate, DaysDiffBetA00ApprovalAndITConfirmation, item.HSDMCR, currentStage);
                    }
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ExcelName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "A00ProjectStatusUpdatedApprovalList");
                    throw (ex);
                }
            }
            else
            {
                return View(model);
            }
        }

        private void ApprovedITConfirmationSearchData(A00ViewModel model, SearchParameterList paramsData)
        {
            try
            {
                List<SearchResultList> empCodeList = new List<SearchResultList>();

                var userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

                model.SearchParams = new SearchParameterList();
                model.SearchParams.OPERATIONID = paramsData.OPERATIONID;
                model.SearchParams.DIVISIONID = paramsData.DIVISIONID;
                model.SearchParams.DEPARTMENTID = paramsData.DEPARTMENTID;
                model.SearchParams.SECTIONID = paramsData.SECTIONID;
                var sykiId = paramsData.SYKIID;//0;

                // SearchParams.StatusId
                model.SearchParams.StatusId = paramsData.StatusId;
                var empCode = string.IsNullOrEmpty(paramsData.ECode) ? 0 : Convert.ToInt64(paramsData.ECode);
                var empName = paramsData.EmpName;
                model.SearchParams.ECode = paramsData.ECode;
                model.SearchParams.EmpName = empName;
                model.SearchParams.SYKIID = sykiId;

                model.SearchParams.SYKI = paramsData.SYKI;
                model.SearchParams.Status = paramsData.Status;

                var status = model.SearchParams.StatusId;
                List<short?> ProjectStatus = new List<short?> { 4, 5 };
                var ProjectStage = "";
                var kiData1 = _objHomeA00.GetA00SYKIList();

                #region Filter Query - IT Confirmation Request
                //var ProjectUpdateStatus = _objHomeA00.GetA00ProjectUpdateStatusList(, sykiId);
                var resultList = _objHomeA00.GetA00APPROVALFullTable()._A00APPROVALList.
                        Where(x => x.OHAPPDATE != null).AsEnumerable().
                       Join(_objHomeA00.GetA00DtlFullTable()._A00DtlViewModelList,
                       a => a.A00DTLTBID, b => b.A00DTLTBID, (a, b) => new { A00APPROVAL = a, A00DTLTB = b }).
                       Join(_objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE, c => c.A00DTLTB.ADDEDBY, d => d.ADEMPCODE, (c, d) => new { c, d }).

                       Join(_objHomeA00.GetA00AllocationList()._A00AllocationList, e => e.c.A00DTLTB.A00DTLTBID, al => al.A00ID, (e, al) => new { e, al }).
                       Join(_objHomeA00.GetA00ItConfirmationList()._A00ITConfirmationList.Where(x => x.Status == 8), g => g.al.A00AllocationID, itconf => itconf.A00ALLOCATIONID,
                       (g, itconf) => new { g, itconf = itconf })

                       .Where(f =>
                       (string.IsNullOrEmpty(empName) ? true : ((f.g.e.d.FIRSTNAME + " " + f.g.e.d.LASTNAME).Contains(empName)))
                    && (f.g.e.c.A00DTLTB.ADDEDBY == (empCode == 0 ? f.g.e.c.A00DTLTB.ADDEDBY : empCode)
//Change start on 22-July-2021
//&& (f.g.e.c.A00DTLTB.STATUSCD == 105)
&& ((status == 0 || status == 1) ? f.g.e.c.A00DTLTB.STATUSCD == 105 : (status == 3 ? f.g.e.c.A00DTLTB.STATUSCD == 125 : (f.g.e.c.A00DTLTB.STATUSCD == 125 || f.g.e.c.A00DTLTB.STATUSCD == 105)))
                   //Change end on 22-July-2021

                   //&& (status == 0 ? !_objHomeA00.GetA00ProjectUpdateStatusList(f.itconf.A00DTLTBID, f.itconf.SYKIID)._A00ProjectUpdateStatusList.Select(x => x.PROJECTSTATUS).Contains((short)4) : _objHomeA00.GetA00ProjectUpdateStatusList(f.itconf.A00DTLTBID, f.itconf.SYKIID)._A00ProjectUpdateStatusList.Select(x => x.PROJECTSTATUS).Contains((short)4))

                   //Commented on 10-July-2021
                   //&& (status == 0 ? !(_objHomeA00.GetA00ProjectUpdateStatusList(f.itconf.A00DTLTBID, f.itconf.SYKIID)._A00ProjectUpdateStatusList.Where(x => (x.PROJECTSTATUS == (short?)4 || x.PROJECTSTATUS == (short?)5) && ((x.PROJECTSTAGE == "P0" || x.PROJECTSTAGE == "Go-Live"))).Select(x => x.PROJECTSTATUS).Contains((short?)4) || _objHomeA00.GetA00ProjectUpdateStatusList(f.itconf.A00DTLTBID, f.itconf.SYKIID)._A00ProjectUpdateStatusList.Where(x => (x.PROJECTSTATUS == (short?)4 || x.PROJECTSTATUS == (short?)5) && ((x.PROJECTSTAGE == "P0" || x.PROJECTSTAGE == "Go-Live"))).Select(x => x.PROJECTSTATUS).Contains((short?)5)) : (_objHomeA00.GetA00ProjectUpdateStatusList(f.itconf.A00DTLTBID, f.itconf.SYKIID)._A00ProjectUpdateStatusList.Where(x => (x.PROJECTSTATUS == 4 || x.PROJECTSTATUS == 5) && ((x.PROJECTSTAGE == "P0" || x.PROJECTSTAGE == "Go-Live"))).Select(x => x.PROJECTSTATUS).Contains((short)4) || _objHomeA00.GetA00ProjectUpdateStatusList(f.itconf.A00DTLTBID, f.itconf.SYKIID)._A00ProjectUpdateStatusList.Where(x => (x.PROJECTSTATUS == (short?)4 || x.PROJECTSTATUS == (short?)5) && ((x.PROJECTSTAGE == "P0" || x.PROJECTSTAGE == "Go-Live"))).Select(x => x.PROJECTSTATUS).Contains((short?)5)))
                   && (
                   //For Pending Request
                   status == 0 ? !(_objHomeA00.GetA00ProjectUpdateStatusList(f.itconf.A00DTLTBID, f.itconf.SYKIID)._A00ProjectUpdateStatusList.Where(x =>
                   (x.PROJECTSTATUS == (short?)4 || x.PROJECTSTATUS == (short?)5) && ((x.PROJECTSTAGE == "P0" || x.PROJECTSTAGE == "Go-Live"))).Select(x => x.PROJECTSTATUS).
                   Contains((short?)4) || _objHomeA00.GetA00ProjectUpdateStatusList(f.itconf.A00DTLTBID, f.itconf.SYKIID)._A00ProjectUpdateStatusList.
                   Where(x => (x.PROJECTSTATUS == (short?)4 || x.PROJECTSTATUS == (short?)5) && ((x.PROJECTSTAGE == "P0" || x.PROJECTSTAGE == "Go-Live"))).
                   Select(x => x.PROJECTSTATUS).Contains((short?)5))
                   //For Completed Request
                   : status == 1 ?
                   (_objHomeA00.GetA00ProjectUpdateStatusList(f.itconf.A00DTLTBID, f.itconf.SYKIID)._A00ProjectUpdateStatusList.
                   Where(x => (x.PROJECTSTATUS == 4 || x.PROJECTSTATUS == 5) && ((x.PROJECTSTAGE == "P0" || x.PROJECTSTAGE == "Go-Live"))).
                   Select(x => x.PROJECTSTATUS).Contains((short)4) || _objHomeA00.GetA00ProjectUpdateStatusList(f.itconf.A00DTLTBID, f.itconf.SYKIID)._A00ProjectUpdateStatusList.
                   Where(x => (x.PROJECTSTATUS == (short?)4 || x.PROJECTSTATUS == (short?)5) && ((x.PROJECTSTAGE == "P0" || x.PROJECTSTAGE == "Go-Live"))).
                   Select(x => x.PROJECTSTATUS).Contains((short?)5))
                   //For All Request
                   //Change start on 10-July-2021
                   :
                   (1 == 1)
                   //Change end on 10-July-2021
                   )
                    && (f.g.e.c.A00DTLTB.SYKIID == (sykiId == 0 ? f.g.e.c.A00DTLTB.SYKIID : sykiId))
                    && ((paramsData.OPERATIONID == 0 || paramsData.OPERATIONID == null) || f.g.e.c.A00DTLTB.OPERATION == paramsData.OPERATIONID)
                    && ((paramsData.DIVISIONID == 0 || paramsData.DIVISIONID == null) || f.g.e.c.A00DTLTB.DIVISION == paramsData.DIVISIONID)
                    && ((paramsData.DEPARTMENTID == 0 || paramsData.DEPARTMENTID == null) || f.g.e.c.A00DTLTB.DEPARTMENT == paramsData.DEPARTMENTID)
                    && ((paramsData.SECTIONID == 0 || paramsData.SECTIONID == null) || f.g.e.c.A00DTLTB.SECTION == paramsData.SECTIONID))).
                   Select(s1 => new SearchResultList
                   {
                       A00DTLTBID = s1.g.e.c.A00DTLTB.A00DTLTBID,
                       SYKIID = s1.g.e.c.A00DTLTB.SYKIID,
                       KICODE = kiData1._SYKIList.Where(x1 => x1.SYKIID == s1.g.e.c.A00DTLTB.SYKIID).Select(x1 => x1.KICODE).FirstOrDefault(),
                       OPERATION = _objHomeA00.Getoperationdetails(s1.g.e.c.A00DTLTB.ADDEDBY, (int)s1.g.e.c.A00DTLTB.SYKIID),
                       DIVISION = _objHomeA00.Getdivisiondetails(s1.g.e.c.A00DTLTB.ADDEDBY, (int)s1.g.e.c.A00DTLTB.SYKIID),//Added By Eshant on 17-Aug-22 to show divison on A00 
                       ProjectCode = s1.itconf.ProjectCode,
                       ProjectTitle = s1.g.e.c.A00DTLTB.PRJCTTLE,
                       A00ApprovedOn = s1.g.e.c.A00APPROVAL.OHAPPDATE,
                       ApprovedOn = (s1.g.al.AllocatedDate),//A00 Assigned Date
                       ITConfirmationDate = _objHomeA00.GetA00ItConfirmationHistoryDetails((decimal)s1.itconf.ITCONFIRMATIONID).Status == 8 ? s1.itconf.ITCONFIRMATIONApprovalDate : null,
                       ECode = s1.g.e.c.A00DTLTB.ADDEDBY,
                       EmpName = s1.g.e.d.FIRSTNAME + " " + s1.g.e.d.LASTNAME,
                       RequestDate = s1.g.e.c.A00DTLTB.DATEADDEDID,
                       ApprovedBy = _objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE.Where(x1 => x1.ADEMPCODE == s1.g.al.AllocatedBY).Select(x1 => x1.FIRSTNAME + " " + x1.LASTNAME).FirstOrDefault(),
                       UserId = userId,
                       MainPIC = _objHomeA00.GetA00MainPIC(s1.g.e.c.A00DTLTB.A00DTLTBID, s1.g.e.c.A00DTLTB.SYKIID),
                       ApproverUserId = _objHomeA00.GetA00ItConfirmationHistoryDetails((decimal)s1.itconf.ITCONFIRMATIONID).ITConfirmationSubmittedTo,
                       ITConfirmationStatus = _objHomeA00.GetA00ItConfirmationHistoryDetails((decimal)s1.itconf.ITCONFIRMATIONID).Status,
                       HSDMCR = s1.itconf.HSDM,
                       //Commented on 10-July-2021
                       //ProjectUpdateStatus = _objHomeA00.GetA00ProjectUpdateStatusList(s1.g.e.c.A00DTLTB.A00DTLTBID, (decimal)s1.g.e.c.A00DTLTB.SYKIID)._A00ProjectUpdateStatusList.Select(x => x.PROJECTSTATUS).FirstOrDefault(),

                       //Change start on 10-July-2021
                       IsAllocationExist = true,
                       ProjectUpdateStatus = _objHomeA00.GetA00ProjectUpdateStatusList(s1.g.e.c.A00DTLTB.A00DTLTBID, (decimal)s1.g.e.c.A00DTLTB.SYKIID).
                       _A00ProjectUpdateStatusList.OrderByDescending(x => x.ProjectStatusUpdateID).Select(x => x.PROJECTSTATUS).FirstOrDefault(),
                       //Change end on 10-July-2021
                       ProjectStage = _objHomeA00.GetA00ProjectUpdateStatusList(s1.g.e.c.A00DTLTB.A00DTLTBID, (decimal)s1.g.e.c.A00DTLTB.SYKIID).
                       _A00ProjectUpdateStatusList.OrderByDescending(x => x.ProjectStatusUpdateID).Select(x => x.PROJECTSTAGE).FirstOrDefault(),
                       //AddedBy Eshant to Display Current stage Target Date on 31-01-2023
                       TargetActionOn = _objHomeA00.GetA00ProjectUpdateStatusList(s1.g.e.c.A00DTLTB.A00DTLTBID, (decimal)s1.g.e.c.A00DTLTB.SYKIID).
                       _A00ProjectUpdateStatusList.OrderByDescending(x => x.ProjectStatusUpdateID).Select(x => x.ActionOn).FirstOrDefault(),
                       //Change start on 22-July-2021
                       STATUSCD = s1.g.e.c.A00DTLTB.STATUSCD
                       //Change end on 22-July-2021
                   }).ToList();
                #endregion

                //Change start on 10-July-2021
                #region Filter Query - All A00 Request
                var resultList1 = _objHomeA00.GetA00APPROVALFullTable()._A00APPROVALList.
                 Where(x => x.OHAPPDATE != null).AsEnumerable().
                Join(_objHomeA00.GetA00DtlFullTable()._A00DtlViewModelList,
                a => a.A00DTLTBID, b => b.A00DTLTBID, (a, b) => new { A00APPROVAL = a, A00DTLTB = b }).
                Join(_objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE, c => c.A00DTLTB.ADDEDBY, d => d.ADEMPCODE, (c, d) => new { c, d })
                .Where(f =>
                (string.IsNullOrEmpty(empName) ? true : ((f.d.FIRSTNAME + " " + f.d.LASTNAME).Contains(empName)))
             && (f.c.A00DTLTB.ADDEDBY == (empCode == 0 ? f.c.A00DTLTB.ADDEDBY : empCode)
//Change start on 22-July-2021
//&& (f.c.A00DTLTB.STATUSCD == 105)
&& ((status == 0 || status == 1) ? f.c.A00DTLTB.STATUSCD == 105 : (status == 3 ? f.c.A00DTLTB.STATUSCD == 125 : (f.c.A00DTLTB.STATUSCD == 125 || f.c.A00DTLTB.STATUSCD == 105)))
            //Change end on 22-July-2021

            && (
            //For Pending Request
            status == 0 ? !(_objHomeA00.GetA00ProjectUpdateStatusList(f.c.A00DTLTB.A00DTLTBID, f.c.A00DTLTB.SYKIID)._A00ProjectUpdateStatusList.Where(x =>
            (x.PROJECTSTATUS == (short?)4 || x.PROJECTSTATUS == (short?)5) && ((x.PROJECTSTAGE == "P0" || x.PROJECTSTAGE == "Go-Live"))).Select(x => x.PROJECTSTATUS).
            Contains((short?)4) || _objHomeA00.GetA00ProjectUpdateStatusList(f.c.A00DTLTB.A00DTLTBID, f.c.A00DTLTB.SYKIID)._A00ProjectUpdateStatusList.
            Where(x => (x.PROJECTSTATUS == (short?)4 || x.PROJECTSTATUS == (short?)5) && ((x.PROJECTSTAGE == "P0" || x.PROJECTSTAGE == "Go-Live"))).
            Select(x => x.PROJECTSTATUS).Contains((short?)5))
            //For Completed Request
            : status == 1 ?
            (_objHomeA00.GetA00ProjectUpdateStatusList(f.c.A00DTLTB.A00DTLTBID, f.c.A00DTLTB.SYKIID)._A00ProjectUpdateStatusList.
            Where(x => (x.PROJECTSTATUS == 4 || x.PROJECTSTATUS == 5) && ((x.PROJECTSTAGE == "P0" || x.PROJECTSTAGE == "Go-Live"))).
            Select(x => x.PROJECTSTATUS).Contains((short)4) || _objHomeA00.GetA00ProjectUpdateStatusList(f.c.A00DTLTB.A00DTLTBID, f.c.A00DTLTB.SYKIID)._A00ProjectUpdateStatusList.
            Where(x => (x.PROJECTSTATUS == (short?)4 || x.PROJECTSTATUS == (short?)5) && ((x.PROJECTSTAGE == "P0" || x.PROJECTSTAGE == "Go-Live"))).
            Select(x => x.PROJECTSTATUS).Contains((short?)5))
            //For All Request
            :
            (1 == 1)
            )
             && (f.c.A00DTLTB.SYKIID == (sykiId == 0 ? f.c.A00DTLTB.SYKIID : sykiId))
             && ((paramsData.OPERATIONID == 0 || paramsData.OPERATIONID == null) || f.c.A00DTLTB.OPERATION == paramsData.OPERATIONID)
             && ((paramsData.DIVISIONID == 0 || paramsData.DIVISIONID == null) || f.c.A00DTLTB.DIVISION == paramsData.DIVISIONID)
             && ((paramsData.DEPARTMENTID == 0 || paramsData.DEPARTMENTID == null) || f.c.A00DTLTB.DEPARTMENT == paramsData.DEPARTMENTID)
             && ((paramsData.SECTIONID == 0 || paramsData.SECTIONID == null) || f.c.A00DTLTB.SECTION == paramsData.SECTIONID))).
            Select(s1 => new SearchResultList
            {
                A00DTLTBID = s1.c.A00DTLTB.A00DTLTBID,
                SYKIID = s1.c.A00DTLTB.SYKIID,
                KICODE = kiData1._SYKIList.Where(x1 => x1.SYKIID == s1.c.A00DTLTB.SYKIID).Select(x1 => x1.KICODE).FirstOrDefault(),
                OPERATION = _objHomeA00.Getoperationdetails(s1.c.A00DTLTB.ADDEDBY, (int)s1.c.A00DTLTB.SYKIID),
                DIVISION = _objHomeA00.Getdivisiondetails(s1.c.A00DTLTB.ADDEDBY, (int)s1.c.A00DTLTB.SYKIID),//Added By Eshant on 17-Aug-22 to show divison on A00 
                ProjectCode = _objHomeA00.GetA00ItConfirmationList()._A00ITConfirmationList.Where(x => x.Status == 8 && x.A00DTLTBID == s1.c.A00DTLTB.A00DTLTBID).Select(x => x.ProjectCode).FirstOrDefault(),
                ProjectTitle = s1.c.A00DTLTB.PRJCTTLE,
                A00ApprovedOn = s1.c.A00APPROVAL.OHAPPDATE,
                A00PPCApprovedOn = s1.c.A00APPROVAL.PPCHOOHDATE,//Added by Eshant on 26-08-2022 for Req change in No. of days calculation on A00 project status update page
                ApprovedOn = null,
                ITConfirmationDate = null,
                ECode = s1.c.A00DTLTB.ADDEDBY,
                EmpName = s1.d.FIRSTNAME + " " + s1.d.LASTNAME,
                RequestDate = s1.c.A00DTLTB.DATEADDEDID,
                ApprovedBy = null,
                UserId = userId,
                MainPIC = 0,
                ApproverUserId = null,
                //ITConfirmationStatus = _objHomeA00.GetA00ItConfirmationList()._A00ITConfirmationList.Where(x => x.A00DTLTBID == s1.c.A00DTLTB.A00DTLTBID).
                //Select(x => x.ITConfirmationLatestHistoryVM.Status).FirstOrDefault(), 
                ITConfirmationStatus = _objHomeA00.GetA00ItConfirmationList()._A00ITConfirmationList.Where(x => x.A00DTLTBID == s1.c.A00DTLTB.A00DTLTBID && x.ITConfirmationLatestHistoryVM != null).Count() > 0 ? _objHomeA00.GetA00ItConfirmationList()._A00ITConfirmationList.Where(x => x.A00DTLTBID == s1.c.A00DTLTB.A00DTLTBID).
                Select(x => x.ITConfirmationLatestHistoryVM.Status).FirstOrDefault() : null,
                HSDMCR = "-",
                IsAllocationExist = _objHomeA00.GetA00AllocationSNo(s1.c.A00DTLTB.A00DTLTBID, s1.c.A00DTLTB.SYKIID) > 0 ? true : false,
                ProjectUpdateStatus = null,
                ProjectStage = "-",
                //Change start on 22-July-2021
                STATUSCD = s1.c.A00DTLTB.STATUSCD
                //Change end on 22-July-2021
            }).ToList();
                #endregion

                #region Merge - IT Confirmation and A00 Request
                var query = from c in resultList1.AsEnumerable()
                            join uc in resultList.AsEnumerable()
                                        on c.A00DTLTBID equals uc.A00DTLTBID into lf
                            from uc in lf.DefaultIfEmpty()
                            select new SearchResultList
                            {
                                A00DTLTBID = c.A00DTLTBID,
                                SYKIID = uc == null ? c.SYKIID : uc.SYKIID,
                                KICODE = uc == null ? c.KICODE : uc.KICODE,
                                OPERATION = uc == null ? c.OPERATION : uc.OPERATION,
                                DIVISION = uc == null ? c.DIVISION : uc.DIVISION,//Added By Eshant on 17-Aug-22 to show divison on A00 
                                ProjectCode = uc == null ? c.ProjectCode : uc.ProjectCode,
                                ProjectTitle = uc == null ? c.ProjectTitle : uc.ProjectTitle,
                                A00ApprovedOn = uc == null ? c.A00ApprovedOn : uc.A00ApprovedOn,
                                A00PPCApprovedOn = uc == null ? c.A00PPCApprovedOn : uc.A00PPCApprovedOn,//Added by Eshant on 26-08-2022 for Req change in No. of days calculation on A00 project status update page
                                ApprovedOn = uc == null ? c.ApprovedOn : uc.ApprovedOn,
                                ITConfirmationDate = uc == null ? c.ITConfirmationDate : uc.ITConfirmationDate,
                                ECode = uc == null ? c.ECode : uc.ECode,
                                EmpName = uc == null ? c.EmpName : uc.EmpName,
                                RequestDate = uc == null ? c.RequestDate : uc.RequestDate,
                                ApprovedBy = uc == null ? c.ApprovedBy : uc.ApprovedBy,
                                UserId = uc == null ? c.UserId : uc.UserId,
                                MainPIC = uc == null ? c.MainPIC : uc.MainPIC,
                                ApproverUserId = uc == null ? c.ApproverUserId : uc.ApproverUserId,
                                ITConfirmationStatus = uc == null ? c.ITConfirmationStatus : uc.ITConfirmationStatus,
                                HSDMCR = uc == null ? c.HSDMCR : uc.HSDMCR,
                                IsAllocationExist = uc == null ? c.IsAllocationExist : uc.IsAllocationExist,
                                ProjectUpdateStatus = uc == null ? c.ProjectUpdateStatus : uc.ProjectUpdateStatus,
                                ProjectStage = uc == null ? c.ProjectStage : uc.ProjectStage,
                                TargetActionOn = uc == null ? c.TargetActionOn : uc.TargetActionOn,//AddedBy Eshant to Display Current stage Target Date on 31-01-2023
                                //Change start on 22-July-2021
                                STATUSCD = uc == null ? c.STATUSCD : uc.STATUSCD
                                //Change end on 22-July-2021
                            };
                #endregion
                //Change end on 10-July-2021

                //Commented on 10-July-2021
                //model.ResultList = resultList.OrderByDescending(x => x.RequestDate).ThenBy(x => x.ProjectTitle).ToList();

                //Change start on 22-July-2021
                model.ResultList = query.OrderByDescending(x => x.A00ApprovedOn).ThenBy(x => x.ProjectTitle).ToList();
                //Change end on 22-July-2021
            }
            catch (Exception ex)
            {
                  _logger.LogError(ex, "ApprovedITConfirmationSearchData");
            }
        }

        [HttpGet]
        public ActionResult ViewA00ForProjectStatusUpdate(string returnMsg = null, long? A00DTLTBID = null, string type = null,
            long? OPERATIONID = null, long? DIVISIONID = null, long? DEPARTMENTID = null, long? SECTIONID = null, decimal? SYKIID = null,
            string ECODE = null, string EMPNAME = null, short? STATUSID = null)
        {
            try
            {
                TempData["PageHead"] = "Project Status View - A00";
                var kiData1 = _objHomeA00.GetA00SYKIList();
                List<SYKI> iList = new List<SYKI>();
                iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });

                var kiData = (from data in kiData1._SYKIList select data).ToList();
                foreach (var item in kiData)
                {
                    iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                }

                var ActiveKiID = SYKIID > 0 ? SYKIID : kiData.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();

                SetSearchValue(OPERATIONID, DIVISIONID, DEPARTMENTID, SECTIONID, ActiveKiID, ECODE, EMPNAME, STATUSID);
                if (returnMsg != null)
                {
                    ViewBag.Msg = returnMsg;
                }
                ViewBag.DeficiencyDetails = _objHomeA00.GetDeficiencyRaisedDetails(A00DTLTBID, ActiveKiID);
                ViewBag.DeficiencyStatus = _objHomeA00.GetDeficiencyStatus(A00DTLTBID, ActiveKiID);


                TempData["PageHead"] = "A00 IT Approval";
                ViewBag.AllocationDetails = _objHomeA00.GetAllocationDetails(A00DTLTBID, ActiveKiID);
                var A00ItConfirmationID = _objHomeA00.GetA00ItConfirmationDetails(A00DTLTBID, (decimal)ActiveKiID).ITCONFIRMATIONID;
                ViewBag.ITConfirmationDetails = _objHomeA00.GetA00ItConfirmationHistory(A00ItConfirmationID);
                ViewBag.ProjectStatusUpdateList = _objHomeA00.GetA00ProjectUpdateStatusList(A00DTLTBID, (decimal)ActiveKiID)._A00ProjectUpdateStatusList.OrderBy(x => x.ProjectStatusUpdateID).ToList();
                ViewBag.Years = DateTime.Now.Year;
                type = type == null ? "NEW" : type.ToUpper();
                ViewBag.pageType = type;
                ViewBag.actionType = "Submit";
                ViewBag.isUploadedFilesExist = "N";
                int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

                #region In New Mode

                var operation = _objHomeA00.GetA00ADORGLEVELList(ActiveKiID);
                List<SearchResultList> budgByList = new List<SearchResultList>();
                budgByList.Add(new SearchResultList { LEVELDESCRIP = "Select", ADORGLEVELID = 0 });
                foreach (var item in operation._ADOrgLevelList)
                {
                    budgByList.Add(new SearchResultList { LEVELDESCRIP = item.LEVELDESCRIP, ADORGLEVELID = item.ADORGLEVELID });
                }


                List<SelectListItem> datalist = new List<SelectListItem>();
                var InvforcastList = _objHomeA00.GetA00_INVFORCASTList();

                foreach (var Datat4 in InvforcastList._InvforcastList)
                {
                    datalist.Add(new SelectListItem { Text = Datat4.INCFORCASTDETAIL, Value = Datat4.INVFORCASTID.ToString() });
                }
                ViewBag.SKYLIST = datalist;



                A00ViewModel objModel = new A00ViewModel();
                objModel.getAllDaysList = objModel.getAllWeekDaysList();
                objModel.GetAllmonth = objModel.getMonth();
                objModel.Getdate = objModel.GetAllDate();
                objModel.SYKI = (decimal)ActiveKiID;
                objModel.A00DTLTBID = (long)A00DTLTBID;
                #endregion
                ViewBag.isOtherSelected = "N";

                #region When View/Action Button Pressed
                if (type == "VIEW")
                {
                    _objA00SearchModel.a00dtltbid = A00DTLTBID;
                    var _A00ExistData = _objHomeA00.GetA00Dtl(_objA00SearchModel);
                    objModel.Projecttitle = _A00ExistData.PRJCTTLE;
                    objModel.Backgrounds = _A00ExistData.PRJCTTXT;
                    objModel.BusinessKPI = _A00ExistData.BUKPITXT;
                    objModel.PurposeA00 = _A00ExistData.PURPSTXT;
                    objModel.TargetA00 = _A00ExistData.TRGTINDCD;
                    objModel.RequirmentA00 = _A00ExistData.RQUMTTXT;
                    objModel.ImagePath = _A00ExistData.ATTACHMENT;
                    objModel.BudgetedSelected = _A00ExistData.BUDGETFLG.ToString();
                    objModel.StartDate = _A00ExistData.STARTDT?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objModel.EndDate = _A00ExistData.ENDDT?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objModel.Ifothers = _A00ExistData.FRCSTOTHER;
                    ViewBag.request_date = _A00ExistData.DATEADDEDID.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    objModel.ADDEDBY1 = _objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE.Where(x1 => x1.ADEMPCODE == _A00ExistData.ADDEDBY).Select(x1 => x1.FIRSTNAME + " " + x1.LASTNAME).FirstOrDefault();
                    ViewBag.userName = objModel.ADDEDBY1;

                    //var vwList = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS_FullList();
                    //objModel.OPERATION_PROPOSING = vwList._A00_VW_ASSOCIATELVLDETAILS_FULL.Where(x => x.ADEMPCODE == _A00ExistData.ADDEDBY).Select(x1 => x1.OPERATION).FirstOrDefault();
                    _objA00SearchModel.ADEMPCODE = _A00ExistData.ADDEDBY;
                    _objA00SearchModel.SYKI = (long)_A00ExistData.SYKIID;

                    //Commented on 10-July-2021
                    //objModel.OPERATION_PROPOSING = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel).OPERATION;

                    //Change start on 10-July-2021
                    objModel.OPERATION_PROPOSING = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel) != null ? _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel).OPERATION : null;
                    //Change end on 10-July-2021
                    ViewBag.OPERATION_P = objModel.OPERATION_PROPOSING;

                    //Change start on 22-July-2021
                    if (_A00ExistData.STATUSCD == 125)
                    {
                        ViewBag.A00ConvertedToCRDtl = _objHomeA00.getConvertedToCRDtl(A00DTLTBID);
                    }
                    else
                    {
                        ViewBag.A00ConvertedToCRDtl = null;
                    }
                    //Change end on 22-July-2021

                    if (A00DTLTBID != null)
                    {
                        ViewBag.actionType = "View";
                        this.generateApprovalHistory(A00DTLTBID ?? 0);
                    }

                    List<SelectListItem> mlist = new List<SelectListItem>();
                    mlist.Add(new SelectListItem { Text = "Select", Value = "", Disabled = true });

                    if (_A00ExistData.BUDGETFLG.ToString() == "1")
                    {
                        mlist.Add(new SelectListItem { Text = "Yes", Value = "10", Selected = true });
                        mlist.Add(new SelectListItem { Text = "No", Value = "11" });
                    }
                    else if (_A00ExistData.BUDGETFLG.ToString() == "0")
                    {
                        mlist.Add(new SelectListItem { Text = "No", Value = "11", Selected = true });
                        mlist.Add(new SelectListItem { Text = "Yes", Value = "10" });
                    }

                    ViewBag.BudgetedValue = mlist;
                    ViewBag.selectedBudgetedVal = _A00ExistData.BUDGETFLG.ToString();
                    var selectedBudgBy = budgByList.Where(x => x.ADORGLEVELID == _A00ExistData.ADORGLEVELID).FirstOrDefault();
                    ViewBag.List = new SelectList(budgByList, "ADORGLEVELID", "LEVELDESCRIP", selectedBudgBy?.ADORGLEVELID);
                    var selectedSyKI = iList.Where(x => x.SYKIID == _A00ExistData.BUDGETSYKIID).FirstOrDefault();
                    ViewBag.SKY = new SelectList(iList, "SYKIID", "KICODE", selectedSyKI?.SYKIID);



                    var A00InvForcast = _objHomeA00.GetA00INVFORCASTList();

                    var ief = A00InvForcast._A00InvforcastList.Join(InvforcastList._InvforcastList, a => a.INVFORCASTID, b => b.INVFORCASTID, (a, b) => new { a, b }).
                      Where(x => x.a.A00DTLTBID == A00DTLTBID).Select(m => new { INVFORCASTID = m.a.INVFORCASTID, INCFORCASTDETAIL = m.b.INCFORCASTDETAIL }).Distinct().ToList();
                    if (type == "ACTION")
                    {
                        datalist.Where(x => ief.Select(y => y.INVFORCASTID.ToString()).ToList().Contains(x.Value)).ToList().ForEach(z => z.Selected = true);
                        if (_A00ExistData.FRCSTOTHER != null)
                        {
                            var OtherSelected = InvforcastList._InvforcastList.Where(x => x.INCFORCASTDETAIL.ToUpper() == "OTHERS").FirstOrDefault();
                            datalist.Where(x => x.Value == OtherSelected.INVFORCASTID.ToString()).Select(x => x.Selected = true);
                            ViewBag.isOtherSelected = "Y";
                        }
                        ViewBag.SKYLIST = datalist;
                        string filePath = Path.Combine(serverpath.getFileUploadPath(), "A00");
                        string FileName = Path.GetFileNameWithoutExtension(_A00ExistData.ATTACHMENT);
                        string FileExtension = Path.GetExtension(_A00ExistData.ATTACHMENT);

                        FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;
                    }
                    else
                    {
                        List<SelectListItem> Vdatalist = new List<SelectListItem>();
                        foreach (var Datat4 in ief)
                        {
                            Vdatalist.Add(new SelectListItem { Text = Datat4.INCFORCASTDETAIL, Value = Datat4.INVFORCASTID.ToString() });
                        }
                        if (_A00ExistData.FRCSTOTHER != null)
                        {
                            var OtherSelected = InvforcastList._InvforcastList.Where(x => x.INCFORCASTDETAIL.ToUpper() == "OTHERS").FirstOrDefault();
                            Vdatalist.Add(new SelectListItem { Text = OtherSelected.INCFORCASTDETAIL.ToString(), Value = OtherSelected.INVFORCASTID.ToString() });
                            ViewBag.isOtherSelected = "Y";
                        }
                        ViewBag.SKYLIST = Vdatalist;
                    }
                }
                else
                {
                    List<SelectListItem> mlist = new List<SelectListItem>();
                    mlist.Add(new SelectListItem { Text = "Select", Selected = true });
                    mlist.Add(new SelectListItem { Text = "Yes", Value = "10" });
                    mlist.Add(new SelectListItem { Text = "No", Value = "11" });
                    ViewBag.BudgetedValue = mlist;
                    ViewBag.List = new SelectList(budgByList, "ADORGLEVELID", "LEVELDESCRIP", 0);
                    ViewBag.SKY = new SelectList(iList, "SYKIID", "KICODE", 0);
                }
                #endregion

                return View(objModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ViewA00ForProjectStatusUpdate");
                return View();
            }
        }
        public ActionResult ViewA00ProjectStatus(long A00DTLTBID, decimal SYKIID)
        {
            try
            {
                var ITConfirmationVM = _objHomeA00.GetA00ItConfirmationDetails(A00DTLTBID, SYKIID);
                TempData["PageHead"] = "IT Confirmation : Project Status View – " + ITConfirmationVM.HSDM;
                var UserPLName = _objHomeA00.GetEmpName((long)ITConfirmationVM.UserPL);
                var ITPLName = _objHomeA00.GetEmpName((long)ITConfirmationVM.ITPL);
                ITConfirmationVM.A00DTLTBID = A00DTLTBID;
                ITConfirmationVM.UserPL_Name = ITConfirmationVM.UserPL + " / " + UserPLName;
                ITConfirmationVM.ITPL_Name = ITConfirmationVM.ITPL + " / " + ITPLName;
                var ITConfirmationApprovalDetails = _objHomeA00.GetA00ItConfirmationHistory(ITConfirmationVM.ITCONFIRMATIONID);
                var LatestITConfirmationIds = ITConfirmationApprovalDetails.GroupBy(x => new
                {
                    ACTIONBY = x.ACTIONBY
                }).Select(x => x.Max(y => y.HistoryID)).ToList();
                ViewBag.ITConfirmationDetails = ITConfirmationApprovalDetails.Where(x => LatestITConfirmationIds.Contains(x.HistoryID)).Select(x => new ITConfirmationApprovalHistoryListVM
                {
                    ACTIONBY = x.ACTIONBY,
                    HistoryID = x.HistoryID,
                    A00ITCONFIRMATIONID = x.A00ITCONFIRMATIONID,
                    A00ID = x.A00ID,
                    SYKIID = x.SYKIID,
                    ACTIONTYPE = x.ACTIONTYPE,
                    STATUS = x.STATUS,
                    REMARKS = x.REMARKS,
                    EmpName = x.EmpName,
                    ACTIONON = x.ACTIONON,
                    ACTIVE = x.ACTIVE,
                    USERTYPE = x.USERTYPE,
                    ITCONFIRMATIONSUBMITTEDTO = x.ITCONFIRMATIONSUBMITTEDTO,
                    ITCONFSUBMITTEDTOUSERTYPE = x.ITCONFSUBMITTEDTOUSERTYPE,
                    ITCONFSUBMITTEDTOSTATUS = x.ITCONFSUBMITTEDTOSTATUS,
                    Designation = x.Designation,
                    PICType = x.PICType
                }).ToList();
                ViewBag.Application = ITConfirmationVM.Application;
                ViewBag.HSDMCR = ITConfirmationVM.HSDM;

                ViewBag.ProjectStatusUpdateList = _objHomeA00.GetA00ProjectUpdateStatusList(A00DTLTBID, SYKIID)._A00ProjectUpdateStatusList.OrderBy(x => x.ProjectStatusUpdateID).ToList();

                var UserId = Convert.ToInt32(_sessionService.Get<string>("userID"));
                var userData = _objHomeA00.getUserRole(UserId, SYKIID);
                var isDeptHead = userData.isDeptHead;
                var isCoOrdHead = userData.isCoOrdHead;
                var isDivHead = userData.isDivHead;
                var isExeCoOrdHead = userData.isExeCoOrdHead;
                var isOperatingHead = userData.isOperatingHead;
                ITConfirmationVM.IsOHLoggedIn = isOperatingHead;
                var A00ProjectStateUpdateVM = _objHomeA00.GetA00ProjectStateUpdate(A00DTLTBID, SYKIID);

                ViewBag.ProjectStage = A00ProjectStateUpdateVM.PROJECTSTAGE;
                if (string.IsNullOrEmpty(A00ProjectStateUpdateVM.ITCONFAPPROVEDON))
                {
                    ViewBag.ItConfrApprovedOn = "";
                }
                else
                {
                    ViewBag.ItConfrApprovedOn = A00ProjectStateUpdateVM.ITCONFAPPROVEDON;
                }
                if (string.IsNullOrEmpty(A00ProjectStateUpdateVM.PROJECTSTATUSREMARKS))
                {
                    ViewBag.Remark = "";
                }
                else
                {
                    ViewBag.Remark = A00ProjectStateUpdateVM.PROJECTSTATUSREMARKS;
                }
                if (A00ProjectStateUpdateVM.PROJECTSTATUS == 0)
                {
                    ViewBag.UpdateState = "";
                }
                else
                {
                    ViewBag.UpdateState = A00ProjectStateUpdateVM.PROJECTSTATUS;
                }
                ViewBag.AttachmentNote = A00ProjectStateUpdateVM.ATTACHMENTNOTE;
                return View(ITConfirmationVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ViewA00ProjectStatus");
                throw new Exception(ex.Message);
            }
        }
        public ActionResult A00ProjectStatusUpdate(long A00DTLTBID, decimal SYKIID)
        {
            try
            {
                var ITConfirmationVM = _objHomeA00.GetA00ItConfirmationDetails(A00DTLTBID, SYKIID);
                TempData["PageHead"] = "Project Status Update - " + ITConfirmationVM.HSDM;
                ViewBag.Application = ITConfirmationVM.Application;
                ViewBag.ApprovalHistory = null;
                ViewBag.SYKIID = SYKIID;
                var A00ProjectStateUpdateVM = _objHomeA00.GetA00ProjectStateUpdate(A00DTLTBID, SYKIID);
                if (ITConfirmationVM.HSDM == "HSDM")
                {
                    if (string.IsNullOrEmpty(A00ProjectStateUpdateVM.PROJECTSTAGE))
                    {
                        ViewBag.ProjectStage = "U0";
                    }
                    else if (A00ProjectStateUpdateVM.PROJECTSTAGE == "U0")
                    {
                        if (A00ProjectStateUpdateVM.PROJECTSTATUS == 4 || A00ProjectStateUpdateVM.PROJECTSTATUS == 5)
                        {
                            ViewBag.ProjectStage = "J0/J1";
                        }
                        else
                        {
                            ViewBag.ProjectStage = "U0";
                        }

                    }
                    else if (A00ProjectStateUpdateVM.PROJECTSTAGE == "J0/J1")
                    {
                        if (A00ProjectStateUpdateVM.PROJECTSTATUS == 4 || A00ProjectStateUpdateVM.PROJECTSTATUS == 5)
                        {
                            ViewBag.ProjectStage = "J2/J3";
                        }
                        else
                        {
                            ViewBag.ProjectStage = "J0/J1";
                        }
                    }
                    else if (A00ProjectStateUpdateVM.PROJECTSTAGE == "J2/J3")
                    {
                        if (A00ProjectStateUpdateVM.PROJECTSTATUS == 4 || A00ProjectStateUpdateVM.PROJECTSTATUS == 5)
                        {
                            ViewBag.ProjectStage = "J4";
                        }
                        else
                        {
                            ViewBag.ProjectStage = "J2/J3";
                        }
                    }
                    else if (A00ProjectStateUpdateVM.PROJECTSTAGE == "J4")
                    {
                        if (A00ProjectStateUpdateVM.PROJECTSTATUS == 4 || A00ProjectStateUpdateVM.PROJECTSTATUS == 5)
                        {
                            ViewBag.ProjectStage = "J5";
                        }
                        else
                        {
                            ViewBag.ProjectStage = "J4";
                        }
                    }
                    else if (A00ProjectStateUpdateVM.PROJECTSTAGE == "J5")
                    {
                        if (A00ProjectStateUpdateVM.PROJECTSTATUS == 4 || A00ProjectStateUpdateVM.PROJECTSTATUS == 5)
                        {
                            ViewBag.ProjectStage = "P0";
                        }
                        else
                        {
                            ViewBag.ProjectStage = "J5";
                        }
                    }
                    else if (A00ProjectStateUpdateVM.PROJECTSTAGE == "P0" && (A00ProjectStateUpdateVM.PROJECTSTATUS == 4 || A00ProjectStateUpdateVM.PROJECTSTATUS == 5))
                    {
                        ViewBag.ProjectStage = "P0";
                    }
                }
                else if (ITConfirmationVM.HSDM == "CR")
                {
                    if (string.IsNullOrEmpty(A00ProjectStateUpdateVM.PROJECTSTAGE))
                    {
                        ViewBag.ProjectStage = "RFQ";
                    }
                    else if (A00ProjectStateUpdateVM.PROJECTSTAGE == "RFQ")
                    {
                        if (A00ProjectStateUpdateVM.PROJECTSTATUS == 4 || A00ProjectStateUpdateVM.PROJECTSTATUS == 5)
                        {
                            ViewBag.ProjectStage = "SOW & Agreement";
                        }
                        else
                        {
                            ViewBag.ProjectStage = "RFQ";
                        }
                    }
                    else if (A00ProjectStateUpdateVM.PROJECTSTAGE == "SOW & Agreement")
                    {
                        if (A00ProjectStateUpdateVM.PROJECTSTATUS == 4 || A00ProjectStateUpdateVM.PROJECTSTATUS == 5)
                        {
                            ViewBag.ProjectStage = "Purchase Order";
                        }
                        else
                        {
                            ViewBag.ProjectStage = "SOW & Agreement";
                        }
                    }
                    else if (A00ProjectStateUpdateVM.PROJECTSTAGE == "Purchase Order")
                    {
                        if (A00ProjectStateUpdateVM.PROJECTSTATUS == 4 || A00ProjectStateUpdateVM.PROJECTSTATUS == 5)
                        {
                            ViewBag.ProjectStage = "Project Documentation";
                        }
                        else
                        {
                            ViewBag.ProjectStage = "Purchase Order";
                        }
                    }
                    else if (A00ProjectStateUpdateVM.PROJECTSTAGE == "Project Documentation")
                    {
                        if (A00ProjectStateUpdateVM.PROJECTSTATUS == 4 || A00ProjectStateUpdateVM.PROJECTSTATUS == 5)
                        {
                            ViewBag.ProjectStage = "Development";
                        }
                        else
                        {
                            ViewBag.ProjectStage = "Project Documentation";
                        }
                    }
                    else if (A00ProjectStateUpdateVM.PROJECTSTAGE == "Development")
                    {
                        if (A00ProjectStateUpdateVM.PROJECTSTATUS == 4 || A00ProjectStateUpdateVM.PROJECTSTATUS == 5)
                        {
                            ViewBag.ProjectStage = "Unit Testing";
                        }
                        else
                        {
                            ViewBag.ProjectStage = "Development";
                        }
                    }
                    else if (A00ProjectStateUpdateVM.PROJECTSTAGE == "Unit Testing")
                    {
                        if (A00ProjectStateUpdateVM.PROJECTSTATUS == 4 || A00ProjectStateUpdateVM.PROJECTSTATUS == 5)
                        {
                            ViewBag.ProjectStage = "UAT";
                        }
                        else
                        {
                            ViewBag.ProjectStage = "Unit Testing";
                        }
                    }
                    else if (A00ProjectStateUpdateVM.PROJECTSTAGE == "UAT")
                    {
                        if (A00ProjectStateUpdateVM.PROJECTSTATUS == 4 || A00ProjectStateUpdateVM.PROJECTSTATUS == 5)
                        {
                            ViewBag.ProjectStage = "Go-Live";
                        }
                        else
                        {
                            ViewBag.ProjectStage = "UAT";
                        }
                    }
                    else if (A00ProjectStateUpdateVM.PROJECTSTAGE == "Go-Live" && (A00ProjectStateUpdateVM.PROJECTSTATUS == 4 || A00ProjectStateUpdateVM.PROJECTSTATUS == 5))
                    {
                        ViewBag.ProjectStage = "Go-Live";
                    }
                }
                ViewBag.ActualProjectStage = A00ProjectStateUpdateVM.PROJECTSTAGE;
                if (string.IsNullOrEmpty(A00ProjectStateUpdateVM.ITCONFAPPROVEDON))
                {
                    ViewBag.ItConfrApprovedOn = "NA";
                }
                else
                {
                    ViewBag.ItConfrApprovedOn = A00ProjectStateUpdateVM.ITCONFAPPROVEDON;
                }
                if (string.IsNullOrEmpty(A00ProjectStateUpdateVM.PROJECTSTATUSREMARKS))
                {
                    ViewBag.Remark = "";
                }
                else
                {
                    ViewBag.Remark = A00ProjectStateUpdateVM.PROJECTSTATUSREMARKS;
                }
                if (A00ProjectStateUpdateVM.PROJECTSTATUS == 0)
                {
                    ViewBag.UpdateState = "";
                }
                else
                {
                    ViewBag.UpdateState = A00ProjectStateUpdateVM.PROJECTSTATUS;
                }
                var UserPLName = _objHomeA00.GetEmpName((long)ITConfirmationVM.UserPL);
                var ITPLName = _objHomeA00.GetEmpName((long)ITConfirmationVM.ITPL);
                ITConfirmationVM.A00DTLTBID = A00DTLTBID;
                ITConfirmationVM.UserPL_Name = ITConfirmationVM.UserPL + " / " + UserPLName;
                ITConfirmationVM.ITPL_Name = ITConfirmationVM.ITPL + " / " + ITPLName;
                ITConfirmationVM.SYKIID = ITConfirmationVM.SYKIID > 0 ? ITConfirmationVM.SYKIID : SYKIID;
                var ITConfirmationApprovalDetails = _objHomeA00.GetA00ItConfirmationHistory(ITConfirmationVM.ITCONFIRMATIONID);
                var LatestITConfirmationIds = ITConfirmationApprovalDetails.GroupBy(x => new
                {
                    ACTIONBY = x.ACTIONBY
                }).Select(x => x.Max(y => y.HistoryID)).ToList();
                ViewBag.ITConfirmationDetails = ITConfirmationApprovalDetails.Where(x => LatestITConfirmationIds.Contains(x.HistoryID)).Select(x => new ITConfirmationApprovalHistoryListVM
                {
                    ACTIONBY = x.ACTIONBY,
                    HistoryID = x.HistoryID,
                    A00ITCONFIRMATIONID = x.A00ITCONFIRMATIONID,
                    A00ID = x.A00ID,
                    SYKIID = x.SYKIID,
                    ACTIONTYPE = x.ACTIONTYPE,
                    STATUS = x.STATUS,
                    REMARKS = x.REMARKS,
                    EmpName = x.EmpName,
                    ACTIONON = x.ACTIONON,
                    ACTIVE = x.ACTIVE,
                    USERTYPE = x.USERTYPE,
                    ITCONFIRMATIONSUBMITTEDTO = x.ITCONFIRMATIONSUBMITTEDTO,
                    ITCONFSUBMITTEDTOUSERTYPE = x.ITCONFSUBMITTEDTOUSERTYPE,
                    ITCONFSUBMITTEDTOSTATUS = x.ITCONFSUBMITTEDTOSTATUS,
                    Designation = x.Designation,
                    PICType = x.PICType
                }).ToList();
                var ProjectStatusUpdateList = _objHomeA00.GetA00ProjectUpdateStatusList(A00DTLTBID, SYKIID)._A00ProjectUpdateStatusList.OrderBy(x => x.ProjectStatusUpdateID).ToList();
                ViewBag.ProjectStatusUpdateList = ProjectStatusUpdateList;
                var lastCompletedStage = ProjectStatusUpdateList.Where(x => (x.PROJECTSTATUS == 4 || x.PROJECTSTATUS == 5)).OrderByDescending(x => x.ProjectStatusUpdateID).Select(x => new { LastProjectStage = x.PROJECTSTAGE + " Done on " + x.ActionOn.Value.ToShortDateString() }).FirstOrDefault();
                if (lastCompletedStage != null)
                {
                    ViewBag.LastCompletedStage = lastCompletedStage.LastProjectStage;
                }
                else
                {
                    ViewBag.LastCompletedStage = "NA";
                }
                return View(ITConfirmationVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "A00ProjectStatusUpdate");
                throw new Exception(ex.Message);
            }

        }

        [HttpPost]
        public ActionResult SaveA00ProjectStatus(IFormCollection fc)
        {
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var ProjectStage = fc["hdnProjectStage"];
            var ProjectStatusDate = fc["ProjectStatusDate"];
            string[] splittedDate = ProjectStatusDate.ToString().Trim().ToString().Split('/');
            string ddmmDate = splittedDate[0].PadLeft(0, '2').ToString() + "/" + splittedDate[1].PadLeft(0, '2').ToString() + "/" + splittedDate[2].PadLeft(0, '2').ToString();
            DateTime StatusDate = DateTime.ParseExact(ddmmDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var ItConfrApprovedOn = fc["hdnItConfrApprovedOn"];// ProjectStage + " Done on " + StatusDate.ToShortDateString();
            var ProjectStatus = Convert.ToInt16(fc["ddlStatus"]);
            var remark = fc["Remark"];
            IFormFile file = Request.Form.Files?[0];
            string fileName = file?.FileName;
            long SYKI = Convert.ToInt64(fc["hdnSYKIID"]);
            long A00DTLTBID = Convert.ToInt64(fc["A00DTLTBID"]);
            if (file != null && !string.IsNullOrEmpty(file.FileName))
            {
                //string UploadPath = Server.MapPath(ConfigurationManager.AppSettings["folderPath"].ToString() + "\\A00\\A00ProjectStatus\\");
                string UploadPath = Path.Combine(serverpath.getFileUploadPath(), "A00", "A00ProjectStatus");
                fileName = Path.GetFileName(file.FileName);
                fileName = Convert.ToString(_sessionService.Get<string>("userID")) + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + "_" + DateTime.Now.Second.ToString() + "_" + DateTime.Now.Millisecond.ToString() + fileName;
                string FilePath = Path.Combine(UploadPath, fileName);
                string directoryPath = Path.GetDirectoryName(FilePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                //file[0].SaveAs(FilePath);
                using (var stream = new FileStream(FilePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            else
            {
                fileName = "NA";
            }
            //****
            //var emailId = _objHomeA00.GetUserEmailForDeficiencyRaised(A00DTLTBID, SYKI);
            //var ProjectTitle = _objHomeA00.GetA00ProjectTitle(A00DTLTBID, SYKI);
            //var requesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(userId);
            //sendMailTODeficiencyOwnerForDeficiencyUpdate(emailId, ProjectTitle, DateTime.Now, requesterDetails.Name, userId.ToString());
            TempData["result"] = _objHomeA00.SaveA00ProjectStatus(ProjectStage, ItConfrApprovedOn, ProjectStatus, remark, fileName, SYKI, userId, A00DTLTBID, StatusDate);
            return RedirectToAction("A00ProjectStatusUpdatedApprovalList");
        }

        #endregion

        #region Activity
        public ActionResult A00Activity(int? ActivityId)
        {
            TempData["PageHead"] = "Activity Form";
            //ViewBag.SYKIID = SYKIID;
            //ViewBag.A00DTLTBID = A00DTLTBID;
            if (ActivityId > 0)
            {
                var A00AcitivtyVM = _objHomeA00.A00ActivityDetails((int)ActivityId);
                //A00AcitivtyVM.ActivityEndDate = convertDateToDDMM(Convert.ToString(A00AcitivtyVM.ActivityEndDate));
                //A00AcitivtyVM.ActivityStartDate = convertDateToDDMM(Convert.ToString(A00AcitivtyVM.ActivityStartDate));
                return View(A00AcitivtyVM);
            }

            return View();
        }

        [HttpPost]
        public ActionResult A00Activity(A00AcitivtyVM a00AcitivtyVM, IFormCollection fc)
        {
            try
            {
                TempData["PageHead"] = "Activity Form";
                a00AcitivtyVM.ActivityEndDate = fc["ActivityEndDate"];
                a00AcitivtyVM.ActivityStartDate = fc["ActivityStartDate"];
                //A00AcitivtyVM a00AcitivtyVM = new A00AcitivtyVM();

                a00AcitivtyVM.ActionBy = Convert.ToInt32(_sessionService.Get<string>("userID"));
                if (!string.IsNullOrEmpty(fc["btnSaveAsDraft"]) && fc["btnSaveAsDraft"].ToString().ToUpper() == "SAVE AS DRAFT")
                {
                    a00AcitivtyVM.Status = 0;
                }
                else if (!string.IsNullOrEmpty(fc["btnSubmit"]) && fc["btnSubmit"].ToString().ToUpper() == "SUBMIT")
                {
                    a00AcitivtyVM.Status = 1;
                }
                var kiData1 = _objHomeA00.GetA00SYKIList();
                var activeKi = kiData1._SYKIList.Where(x => x.ACTIVE == 1).FirstOrDefault();

                a00AcitivtyVM.SYKID = Convert.ToInt32(activeKi.SYKIID);
                //a00AcitivtyVM.A00DTLTBID = Convert.ToInt16(fc["hdnA00DTLTBID"]);
                //a00AcitivtyVM.ActivityName = fc["ActivityName"];
                //a00AcitivtyVM.ActivityDetails = fc["ActivityDetails"];
                _objA00SearchModel.ADEMPCODE = Convert.ToInt64(a00AcitivtyVM.ActionBy);
                _objA00SearchModel.SYKI = Convert.ToInt64(a00AcitivtyVM.SYKID);
                var operation22 = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel);
                if (operation22.DEPARTMENTID != null)
                {
                    a00AcitivtyVM.DEPARTMENTID = Convert.ToInt16(operation22.DEPARTMENTID);
                }
                else
                {
                    a00AcitivtyVM.DEPARTMENTID = 0;
                }

                if (operation22.OPERATIONID != null)
                {
                    a00AcitivtyVM.OPERATIONID = Convert.ToInt16(operation22.OPERATIONID);
                }
                else
                {
                    a00AcitivtyVM.OPERATIONID = 0;
                }

                if (operation22.DIVISIONID != null)
                {
                    a00AcitivtyVM.DIVISIONID = Convert.ToInt16(operation22.DIVISIONID);
                }
                else
                {
                    a00AcitivtyVM.DIVISIONID = 0;
                }

                if (operation22.SECTIONID != null)
                {
                    a00AcitivtyVM.SECTIONID = Convert.ToInt32(operation22.SECTIONID);
                }
                else
                {
                    a00AcitivtyVM.SECTIONID = 0;
                }

                //ModelState.Clear();

                TempData["result"] = _objHomeA00.A00ActivitySaveData(a00AcitivtyVM);
                //if (a00AcitivtyVM.Status == 1)
                //    return RedirectToAction("A00ActivityList");
                //else
                //    return View(a00AcitivtyVM);
                return RedirectToAction("A00ActivityList");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "A00Activity");
                throw new Exception(ex.Message);
            }
        }
        public ActionResult A00ActivityUpdate(int ActivityId)
        {
            TempData["PageHead"] = "Activity Update";
            var A00AcitivtyVM = _objHomeA00.A00ActivityDetails(ActivityId);
            ViewBag.ActivitySchedule = A00AcitivtyVM.ActivitySchedule;
            if (A00AcitivtyVM.ActivitySchedule == 2)
            {
                TempData["result"] = 3;
                return RedirectToAction("A00ActivityList");
            }
            ViewBag.A00ActivityID = ActivityId;
            return View(A00AcitivtyVM);

        }

        [HttpPost]
        public ActionResult A00ActivityUpdate(IFormCollection fc)
        {
            try
            {
                int ActivitySchedule = Convert.ToInt16(fc["ActivitySchedule"]);
                int A00ActivityID = Convert.ToInt16(fc["hdnA00ActivityID"]);
                var ActivityRemarks = fc["ActivityRemarks"];
                var ActionBy = Convert.ToInt32(_sessionService.Get<string>("userID"));
                TempData["result"] = _objHomeA00.A00ActivityUpdate(A00ActivityID, ActivitySchedule, ActivityRemarks, ActionBy);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "A00ActivityUpdate");
                throw new Exception(ex.Message);
            }
            return RedirectToAction("A00ActivityList");
        }

        public ActionResult A00ActivityList()
        {
            TempData["PageHead"] = "Activity List";
            SearchParameterList paramsList = new SearchParameterList();
            A00ViewModel model = new A00ViewModel();
            try
            {
                //Dinesh
                paramsList.StatusId = 1;
                var paramData = A00ActivityDetailsSearchFilterData(paramsList);
                //***
                //model.SearchParams.SYKI = paramData.SYKIID;
                //model.SearchParams.Status = paramData.Status;

                A00ActivityDetailsSearchData(model, paramData);

                //List<ADORGLEVEL> _opList = _objHomeA00.GetOrgLevelList((long)1);
                List<A00ADORGLEVELList> _opList = _objHomeA00.GetA00ADORGLEVELList((decimal)paramData.SYKIID)._ADOrgLevelList;
                ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.DivList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.DepList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.SecList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "A00ActivityList");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult A00ActivityList(A00ViewModel model, string value)
        {
            TempData["PageHead"] = "Activity List";
            SearchParameterList paramlist = new SearchParameterList();

            //Employee_Details employeeDetails = (Employee_Details)Session["Employee"];
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");

            var sykiId = 0;
            if (!string.IsNullOrEmpty(Request.Form["SearchParams.SYKI"].ToString()))
            {
                sykiId = Convert.ToInt32(Request.Form["SearchParams.SYKI"].ToString());
            }
            paramlist.SYKIID = sykiId;
            paramlist.ECode = Request.Form["SearchParams.ECode"].ToString();
            paramlist.EmpName = Request.Form["SearchParams.EmpName"].ToString();
            //paramlist.StatusId = Convert.ToInt16(Request.Query["SearchParams.Status"].ToString());
            paramlist.StatusId = Convert.ToInt16(Request.Form["SearchParams.Status"].ToString());

            paramlist.OPERATIONID = Request.Form["SearchParams.OPERATIONID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.OPERATIONID"]) : 0;
            paramlist.DIVISIONID = Request.Form["SearchParams.DIVISIONID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.DIVISIONID"]) : 0;
            paramlist.DEPARTMENTID = Request.Form["SearchParams.DEPARTMENTID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.DEPARTMENTID"]) : 0;
            paramlist.SECTIONID = Request.Form["SearchParams.SECTIONID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.SECTIONID"]) : 0;

            #region Block-1
            var userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

            model.SearchParams = paramlist;// new SearchParameterList();
            model.SearchParams.StatusId = paramlist.StatusId;
            var empCode = string.IsNullOrEmpty(Request.Form["SearchParams.ECode"].ToString()) ? 0 : Convert.ToInt64(Request.Form["SearchParams.ECode"].ToString());
            var empName = Request.Form["SearchParams.EmpName"].ToString();
            model.SearchParams.ECode = Request.Form["SearchParams.ECode"].ToString();
            model.SearchParams.EmpName = empName;
            model.SearchParams.SYKIID = sykiId;
            #endregion

            var paramData = A00ActivityDetailsSearchFilterData(paramlist);

            model.SearchParams.SYKI = paramData.SYKI;
            model.SearchParams.Status = paramData.Status;

            A00ActivityDetailsSearchData(model, paramlist);

            //DInesh
            if (value == "Export to excel")
            {
                try
                {
                    List<A00ActivityDataforExcel> a00ActivityDataforExcel = model.ResultList.Select(x => (new A00ActivityDataforExcel { KI_Code = x.KICODE, E_Code = x.ECode, Name = x.EmpName, Start_Date = x.ActivityStartDate, End_Date = x.ActivityEndDate, Status = x.ActivitySchedule, Remark = x.ActivityRemark, Activity_Title = x.ActivityTitle, STATUSCD = x.STATUSCD, ActivityDetail = x.ActivityDetail, ActivityRemarkDate = x.ActivityRemarkDate })).ToList();

                    string ExcelName = "Activity Data-" + Convert.ToString(DateTime.Now).Replace(" ", "_").Replace(":", "_").Replace("/", "_") + ".xlsx";
                    System.Data.DataTable dt = new System.Data.DataTable("Activity Data");
                    dt.Columns.AddRange(new DataColumn[11] {
                        new DataColumn("S.No"),
                        new DataColumn("KI Code"),
                        new DataColumn("E Code"),
                           new DataColumn("Name"),
                        new DataColumn("Activity Title"),
                        new DataColumn("Activity Details"),
                        new DataColumn("Activity Start Date"),
                        new DataColumn("Activity End Date"),
                         new DataColumn("Remarks"),
                        new DataColumn("Remarks update on"),
                        new DataColumn("Status")
                    });
                    int intCntr = 0;
                    foreach (var item in a00ActivityDataforExcel)
                    {
                        intCntr++;
                        var Start_Date = item.Start_Date.ToString() == "01-Jan-1900 00:00:00" ? " " : item.Start_Date.ToString();
                        var End_Date = item.End_Date.ToString() == "01-Jan-1900 00:00:00" ? " " : item.End_Date.ToString();
                        var ActivityRemarkDate = item.ActivityRemarkDate.ToString() == "01-Jan-1900 00:00:00" ? " " : item.ActivityRemarkDate.ToString();
                        String strStatus = "";
                        if (item.STATUSCD == 1)
                        {
                            if (item.Status == 1)
                            {
                                strStatus = "In Process";
                            }
                            else if (item.Status == 2)
                            {
                                strStatus = "Completed";
                            }
                            else
                            {
                                strStatus = "Not Started";
                            }
                        }
                        else
                        {
                            strStatus = "Save As Draft";
                        }
                        dt.Rows.Add(intCntr, item.KI_Code, item.E_Code, item.Name, item.Activity_Title, item.ActivityDetail, Start_Date, End_Date, item.Remark, ActivityRemarkDate, strStatus);
                    }
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ExcelName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "A00ActivityList");
                    throw (ex);
                }
            }
            else
            {


                return View(model);
            }

        }
        private SearchParameterList A00ActivityDetailsSearchFilterData(SearchParameterList paramsList)
        {
            List<SearchParameterList> list = new List<SearchParameterList>();
            SearchParameterList data = new SearchParameterList();

            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

            #region Bind Ki drop down
            List<SearchParameterList> kiData = new List<SearchParameterList>();
            kiData.Add(new SearchParameterList() { SYKIID = 0, SYKI = "Select" });

            var kiData1 = _objHomeA00.GetA00SYKIList();
            var activeKi = kiData1._SYKIList.Where(x => x.ACTIVE == 1).FirstOrDefault();
            var kii = kiData1._SYKIList.OrderByDescending(x => x.SYKIID).Select(x => new SearchParameterList { SYKIID = x.SYKIID, SYKI = x.KICODE }).ToList();
            kiData.AddRange(kii);
            var selectedKi = (paramsList.SYKIID == 0 || paramsList.SYKIID == null) ? activeKi.SYKIID : (decimal)paramsList.SYKIID;
            ViewBag.SYKIData = new SelectList(kiData, "SYKIID", "SYKI", selectedKi);

            #endregion

            #region Bind status drop down
            data.StatusId = paramsList.StatusId;
            var statusList = new List<SearchParameterList>() {
            new SearchParameterList {
                StatusId = 0,
                Status = "Not Started"
            },
            new SearchParameterList {
                StatusId = 1,
                Status = "In Process"
            },
            new SearchParameterList {
                StatusId = 2,
                Status = "Completed"
            }};
            //ViewBag.StatusList = new SelectList(statusList, "StatusId", "Status", paramsList.StatusId);
            ViewBag.StatusList = new SelectList(statusList.OrderBy(i => i.Status).ToList(), "StatusId", "Status", paramsList.StatusId);//changed by eshant 4-jul-22 for sorting
            #endregion

            //data.ECode = paramsList.ECode;
            //data.EmpName = paramsList.EmpName;
            //data.OPERATIONID = paramsList.OPERATIONID;
            //data.DIVISIONID = paramsList.DIVISIONID;
            //data.SECTIONID = paramsList.SECTIONID;
            //data.DEPARTMENTID = paramsList.DEPARTMENTID;
            data.SYKIID = selectedKi;
            paramsList.SYKIID = selectedKi;

            #region Bind Login Employee Details
            _objA00SearchModel.ADEMPCODE = userId;
            _objA00SearchModel.SYKI = selectedKi;

            var empDetails = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel);
            if (empDetails != null)
            {
                data.OPERATION = empDetails.OPERATION != null ? empDetails.OPERATION : "--No record--";
                data.OPERATIONID = empDetails.OPERATIONID != null ? empDetails.OPERATIONID : 0;
                paramsList.OPERATIONID = paramsList.OPERATIONID > 0 ? paramsList.OPERATIONID : data.OPERATIONID;
                data.DIVISION = empDetails.DIVISION != null ? empDetails.DIVISION : "--No record--";
                data.DIVISIONID = empDetails.DIVISIONID != null ? empDetails.DIVISIONID : 0;
                paramsList.DIVISIONID = paramsList.DIVISIONID > 0 ? paramsList.DIVISIONID : data.DIVISIONID;
                data.DEPARTMENT = empDetails.DEPARTMENT != null ? empDetails.DEPARTMENT : "--No record--";
                data.DEPARTMENTID = empDetails.DEPARTMENTID != null ? empDetails.DEPARTMENTID : 0;
                paramsList.DEPARTMENTID = paramsList.DEPARTMENTID > 0 ? paramsList.DEPARTMENTID : data.DEPARTMENTID;
                data.SECTION = empDetails.SECTION != null ? empDetails.SECTION : "--No record--";
                data.SECTIONID = empDetails.SECTIONID != null ? empDetails.SECTIONID : 0;
                paramsList.SECTIONID = paramsList.SECTIONID > 0 ? paramsList.SECTIONID : data.SECTIONID;
            }
            else
            {
                data.OPERATION = "--No record--";
                data.OPERATIONID = 0;
                data.DIVISION = "--No record--";
                data.DIVISIONID = 0;
                data.DEPARTMENT = "--No record--";
                data.DEPARTMENTID = 0;
                data.SECTION = "--No record--";
                data.SECTIONID = 0;
            }
            #endregion


            list.Add(data);
            ViewBag.List = list;
            return paramsList;
        }
        private void A00ActivityDetailsSearchData(A00ViewModel model, SearchParameterList paramsData)
        {
            //SearchParameterList paramlist = new SearchParameterList();
            List<SearchResultList> empCodeList = new List<SearchResultList>();

            var userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

            model.SearchParams = new SearchParameterList();
            model.SearchParams.OPERATIONID = paramsData.OPERATIONID;
            model.SearchParams.DIVISIONID = paramsData.DIVISIONID;
            model.SearchParams.DEPARTMENTID = paramsData.DEPARTMENTID;
            model.SearchParams.SECTIONID = paramsData.SECTIONID;
            var sykiId = paramsData.SYKIID;//0;

            // SearchParams.StatusId
            model.SearchParams.StatusId = paramsData.StatusId;
            var empCode = string.IsNullOrEmpty(paramsData.ECode) ? 0 : Convert.ToInt64(paramsData.ECode);
            var empName = paramsData.EmpName;
            model.SearchParams.ECode = paramsData.ECode;
            model.SearchParams.EmpName = empName;
            model.SearchParams.SYKIID = sykiId;

            model.SearchParams.SYKI = paramsData.SYKI;
            model.SearchParams.Status = paramsData.Status;

            var status = model.SearchParams.StatusId;

            bool isNormalUser = false, isSecHead = false, isDeptHead = false, isCoOrdHead = false, isDivHead = false, isExeCoOrdHead = false, isOperatingHead = false;
            _objA00SearchModel.ADEMPCODE = userId;
            _objA00SearchModel.SYKI = (decimal)paramsData.SYKIID;
            var MainPICOperation = _objHomeA00.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel);
            //_objA00SearchModel.OPERATIONID = MainPICOperation.OPERATIONID;
            var userData = _objHomeA00.getUserRoles(userId, paramsData.SYKIID);
            isSecHead = userData.isSecHead;
            isDeptHead = userData.isDeptHead;
            isCoOrdHead = userData.isCoOrdHead;
            isDivHead = userData.isDivHead;
            isExeCoOrdHead = userData.isExeCoOrdHead;
            isOperatingHead = userData.isOperatingHead;
            isNormalUser = isSecHead == false && isDeptHead == false && isCoOrdHead == false && isDivHead == false && isExeCoOrdHead == false && isOperatingHead == false ? true : false;

            #region Filter Query
            var kiData1 = _objHomeA00.GetA00SYKIList();
            List<SearchResultList> result = new List<SearchResultList>();
            var ActivityList = _objHomeA00.GetA00ActivityList()._A00ActivityList;
            var EmployeeList = _objHomeA00.GetA00ADEMPLOYEE()._A00ADEMPLOYEE;
            var resultList = ActivityList.
                   Join(EmployeeList,
                   a => a.ActionBy, b => b.ADEMPCODE, (a, b) => new { Activity = a, Employee = b })
                   .Where(f => f.Activity.ActivitySchedule == status
                   && (f.Activity.ActionBy == userId)
                   && (f.Activity.ActionBy == (empCode == 0 ? f.Activity.ActionBy : empCode))
                       //&& (string.IsNullOrEmpty(empName) ? true : ((f.Employee.FIRSTNAME + " " + f.Employee.LASTNAME).Contains(empName)))
                       && (string.IsNullOrEmpty(empName) || ((f.Employee.FIRSTNAME + " " + f.Employee.LASTNAME).Contains(empName)))
                       && (f.Activity.SYKID == (sykiId == 0 ? f.Activity.SYKID : sykiId))
                       && ((paramsData.OPERATIONID == 0 || paramsData.OPERATIONID == null) || f.Activity.OPERATIONID == paramsData.OPERATIONID)
                       && ((paramsData.DIVISIONID == 0 || paramsData.DIVISIONID == null) || f.Activity.DIVISIONID == paramsData.DIVISIONID)
                       && ((paramsData.DEPARTMENTID == 0 || paramsData.DEPARTMENTID == null) || f.Activity.DEPARTMENTID == paramsData.DEPARTMENTID)
                       && ((paramsData.SECTIONID == 0 || paramsData.SECTIONID == null) || f.Activity.SECTIONID == paramsData.SECTIONID)
                       ).
               Select(s1 => new SearchResultList
               {
                   SYKIID = s1.Activity.SYKID,
                   KICODE = kiData1._SYKIList.Where(x1 => x1.SYKIID == s1.Activity.SYKID).Select(x1 => x1.KICODE).FirstOrDefault(),
                   ECode = Convert.ToInt64(s1.Activity.ActionBy),
                   EmpName = s1.Employee.FIRSTNAME + " " + s1.Employee.LASTNAME,
                   UserId = userId,
                   ActivityTitle = s1.Activity.ActivityName,
                   STARTDT = (DateTime)s1.Activity.StartDate,
                   ActivityStartDate = s1.Activity.ActivityStartDate,
                   ActivityEndDate = s1.Activity.ActivityEndDate,
                   ActivityId = s1.Activity.ActivityID,
                   ActivitySchedule = s1.Activity.ActivitySchedule,
                   STATUSCD = (short)s1.Activity.Status,
                   ActivityRemark = s1.Activity.Remark,
                   ActivityDetail = s1.Activity.ActivityDetails,
                   ActivityRemarkDate = s1.Activity.ActivityRemarkDate,
               }).ToList(); //19-05
            result.AddRange(resultList);
            if (isNormalUser == false)
            {
                var resultList1 = ActivityList.
                       Join(EmployeeList,
                       a => a.ActionBy, b => b.ADEMPCODE, (a, b) => new { Activity = a, Employee = b })
                       .Where(f => f.Activity.ActivitySchedule == status
                       && f.Activity.ActionBy != userId
                       && (f.Activity.ActionBy == (empCode == 0 ? f.Activity.ActionBy : empCode))
                       //&& (string.IsNullOrEmpty(empName) ? true : ((f.Employee.FIRSTNAME + " " + f.Employee.LASTNAME).Contains(empName)))
                       && (string.IsNullOrEmpty(empName) || ((f.Employee.FIRSTNAME + " " + f.Employee.LASTNAME).Contains(empName)))
                       && (f.Activity.SYKID == (sykiId == 0 ? f.Activity.SYKID : sykiId))
                       && ((paramsData.OPERATIONID == 0 || paramsData.OPERATIONID == null) || f.Activity.OPERATIONID == paramsData.OPERATIONID)
                       && ((paramsData.DIVISIONID == 0 || paramsData.DIVISIONID == null) || f.Activity.DIVISIONID == paramsData.DIVISIONID)
                       && ((paramsData.DEPARTMENTID == 0 || paramsData.DEPARTMENTID == null) || f.Activity.DEPARTMENTID == paramsData.DEPARTMENTID)
                       && ((paramsData.SECTIONID == 0 || paramsData.SECTIONID == null) || f.Activity.SECTIONID == paramsData.SECTIONID)
                       ).
                   Select(s1 => new SearchResultList
                   {
                       SYKIID = s1.Activity.SYKID,
                       KICODE = kiData1._SYKIList.Where(x1 => x1.SYKIID == s1.Activity.SYKID).Select(x1 => x1.KICODE).FirstOrDefault(),
                       ECode = Convert.ToInt64(s1.Activity.ActionBy),
                       EmpName = s1.Employee.FIRSTNAME + " " + s1.Employee.LASTNAME,
                       UserId = userId,
                       ActivityTitle = s1.Activity.ActivityName,
                       STARTDT = (DateTime)s1.Activity.StartDate,
                       ActivityStartDate = s1.Activity.ActivityStartDate,
                       ActivityEndDate = s1.Activity.ActivityEndDate,
                       ActivityId = s1.Activity.ActivityID,
                       ActivitySchedule = s1.Activity.ActivitySchedule,
                       STATUSCD = (short)s1.Activity.Status,
                       ActivityRemark = s1.Activity.Remark,
                       ActivityDetail = s1.Activity.ActivityDetails,
                       ActivityRemarkDate = s1.Activity.ActivityRemarkDate,
                   }).ToList();
                result.AddRange(resultList1);

            }
            #endregion

            model.ResultList = result.Where(x => x.ActivityStartDate != null).Distinct().OrderBy(x => x.STARTDT).Select(x => x).ToList();
            //TempData["ExportActivity"] = model.ResultList;

        }
        public ActionResult A00ActivityView(int ActivityId)
        {
            TempData["PageHead"] = "Activity View";
            var A00AcitivtyVM = _objHomeA00.A00ActivityDetails(ActivityId);
            ViewBag.ActivitySchedule = A00AcitivtyVM.ActivitySchedule;
            ViewBag.A00ActivityID = ActivityId;
            var ActivityHistoryDetails = _objHomeA00.A00ActivityHistory(ActivityId);
            ViewBag.ActivityHistoryDetails = ActivityHistoryDetails;
            return View(A00AcitivtyVM);
        }
        #endregion

        //Change start on 22-July-2021
        #region Send Back - Convert to CR
        public ActionResult A00ConvertToCR(IFormCollection fc)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    long A00DTLTBID = !string.IsNullOrWhiteSpace(fc["hdnA00DTLTBID"]) ? Convert.ToInt64(fc["hdnA00DTLTBID"]) : 0;
                    int CHANGEDBY = Convert.ToInt32(_sessionService.Get<string>("userID"));
                    int ConvertToCrUpdatedStatus = _objHomeA00.UpdateA00ConvertToCR(A00DTLTBID);  //1 = Success, 0 = Error //////change statuscd - 125 in A00DTLTB table
                    int ConvertToCrInsertStatus = _objHomeA00.InsertA00ConvertToCR(A00DTLTBID, CHANGEDBY, fc["ConvertToCRRemark"]);  //1 = Success, 0 = Error

                    if (ConvertToCrUpdatedStatus == 1 && ConvertToCrInsertStatus == 1)
                    {
                        TempData["msg"] = "A00 sent back successfully for Convert to CR";

                        _objA00SearchModel.a00dtltbid = A00DTLTBID;
                        var A00DtlTb = _objHomeA00.GetA00Dtl(_objA00SearchModel);

                        //var a00RequesterEmailID = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(A00DtlTb.ADDEDBY).EMAILID; //A00 requester details
                        var a00RequesterDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(A00DtlTb.ADDEDBY); //A00 requester details
                        var a00ActionByDetails = _objHomeA00.Get_Single_A00ADEMPLOYEEEmail(CHANGEDBY); //A00 action by details
                        //sendMailForA00ConvertToCR(a00RequesterEmailID, A00DtlTb.PRJCTTLE);
                        sendMailForA00ConvertToCR(a00RequesterDetails, a00ActionByDetails, A00DtlTb.PRJCTTLE);
                    }
                    else
                    {
                        TempData["msg"] = "Error in convert to CR";
                    }
                    return RedirectToAction("ITDivisionApprovalSearch");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Msg = "Error : " + ex.Message.ToString();
                _logger.LogError(ex, "A00ConvertToCR");
                return View("Error");
            }
        }

        private void sendMailForA00ConvertToCR(Single_A00ADEMPLOYEE a00RequesterDetails, Single_A00ADEMPLOYEE a00ActionByDetails, string projectTitle)
        {
            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = a00RequesterDetails.EMAILID;

            string strSubject = "A00 Request Sent back (Convert to CR) by - " + a00ActionByDetails.Name + ", Employee Code - " + a00ActionByDetails.EmpCode;
            string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                             "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>A00 Request send back by - " + a00ActionByDetails.Name + " - Emp Code (" + a00ActionByDetails.EmpCode + ")</font></b></td>" +
                             "</tr><tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td width=520 valign=top> Dear " + a00RequesterDetails.Name + " San, <br />This is to inform you that your A00 request (" + projectTitle +
                             ") has been sent back (Convert to Change Request). Kindly raise Service request in IT Sahayak.</td></tr>" +
                             "<tr><td valign=top width=520>Please login <a href='https://portal.honda2wheelersindia.com/SSO'> Employee Portal</a> for more details.</td></tr>" +
                             "<tr><td width=520>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";


            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }
        #endregion
        //Change end on 22-July-2021
        [HttpPost]
        public ActionResult AddRemark([FromBody] A00AddRemarksViewModel model /*long A00ID, string REMARK*/)
        {
            short retVal = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                long loginUser = Convert.ToInt64(_sessionService.Get<string>("userID").ToString());
                retVal = _objHomeA00.AddRemark(model.A00ID, model.REMARK, loginUser);
            }
            catch (Exception ex)
            {
                retVal = -1;
                _logger.LogError(ex, "AddRemark");
                throw (ex);
            }
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult GetRemark(int A00ID)
        {
            List<A00REMARKSVM> data = new List<A00REMARKSVM>();
            data = _objHomeA00.A00RemarksHistory(A00ID);
            return Json(data);

        }
    }

}
