

using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.Shared.Services;
using ePortal.ViewModels;
using ePortal.WebUI.Controllers;
using ePortal.WebUI.Filters;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using System.Globalization;
using static System.Net.Mime.MediaTypeNames;
using Font = iTextSharp.text.Font;



namespace HMSI.ePortal.Web.Controllers
{
    [CSPFilter("",15)]
    public class AssetRegistrationController : Controller
    {
          private readonly  IAssetRegistrationService _objHomeAssetRegistration;
        private readonly ISessionService _sessionService;
        private readonly ILogger<AssetRegistrationController> _logger;
        public AssetRegistrationController(IAssetRegistrationService _IAssetRegistrationService, ISessionService objISessionService, ILogger<AssetRegistrationController> objlogger)
        {
            _objHomeAssetRegistration = _IAssetRegistrationService;
            _sessionService = objISessionService;
            _logger = objlogger;
            // _objAssetRegistrationSearchModel = _AssetRegistrationSearchModel;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SetPeriodForISSCMemberNomination(int? id)
        {
            try
            {

            
            TempData["PageHead"] = "ISSC Member Nomination - Period Setting";
            List<SYKI> iList = new List<SYKI>();
            // iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });
            var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }

            ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE");

            var PeriodSettingList = _objHomeAssetRegistration.GetAsset_Register_periodList();
            ViewBag.PeriodSettingList = PeriodSettingList;
            PeriodForISSCMemberNominationVM PeriodSetting = new PeriodForISSCMemberNominationVM();
            if (id > 0)
            {
                PeriodSetting = PeriodSettingList.Where(x => x.PeriodForISSCMemberNominationID == id).ToList().Select(x => new PeriodForISSCMemberNominationVM
                {
                    PeriodForISSCMemberNominationID = x.PeriodForISSCMemberNominationID,
                    SYKIID = x.SYKIID,
                    StartDate = x.StartDate.ToString("dd/MM/yyyy"),
                    EndDate = x.EndDate.ToString("dd/MM/yyyy"),
                    ActionType = 1,
                }).FirstOrDefault();


                ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE", PeriodSetting.SYKIID);
                ViewBag.statedt = PeriodSetting.StartDate;
                ViewBag.enddt = PeriodSetting.EndDate;
            }
            ViewBag.CurrentSYKIData = iList.Select(x => x.KICODE).FirstOrDefault();
            return View(PeriodSetting);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetPeriodForISSCMemberNomination(PeriodForISSCMemberNominationVM model)
        {
            try { 
            if (ModelState.IsValid)
            {
                var userid = Convert.ToInt32(_sessionService.Get<string>("userID"));
                model.ADDEDBY = userid;
                //model.StartDate = model.StartDate.ToString("MM/dd/yyyy");
                PeriodForISSCMemberNominationVM PeriodSetting = _objHomeAssetRegistration.InsertUpdatePeriodSettingDetail(model);
                List<SYKI> iList1 = new List<SYKI>();
                // iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });
                var kiData1 = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
                foreach (var item in kiData1._SYKIList)
                {
                    iList1.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                }
                ViewBag.SYKIData = new SelectList(iList1, "SYKIID", "KICODE");

                var PeriodSettingList1 = _objHomeAssetRegistration.GetAsset_Register_periodList();
                ViewBag.PeriodSettingList = PeriodSettingList1;
                //return View(PeriodSetting);
                List<SYKI> iList = new List<SYKI>();
                // iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });
                var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
                foreach (var item in kiData._SYKIList)
                {
                    iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                }
                ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE");
                ViewBag.CurrentSYKIData = iList.Select(x => x.KICODE).FirstOrDefault();
                var PeriodSettingList = _objHomeAssetRegistration.GetAsset_Register_periodList();
                ViewBag.PeriodSettingList = PeriodSettingList;

                var sendmailist = _objHomeAssetRegistration.GetOperatingHead((long)model.SYKIID);
                CultureInfo culture = new CultureInfo("en-GB");
                foreach (var item in sendmailist)
                {
                    SendMailTo_OperatingHead(item.Empname, item.Emailid, Convert.ToDateTime(model.StartDate, culture), Convert.ToDateTime(model.EndDate, culture), iList.Select(x => x.KICODE).FirstOrDefault());
                }
                TempData["msg"] = model.Msg;
                if (model.PeriodForISSCMemberNominationID > 0)
                {
                    return Redirect("../../AssetRegistration/SetPeriodForISSCMemberNomination");
                }
                else
                {
                    return Redirect("../AssetRegistration/SetPeriodForISSCMemberNomination");
                }
            }
            return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                throw ex;
            }
        }

        [HttpGet]
        public ActionResult SetPeriodForICancel()
        {
            return RedirectToAction("SetPeriodForISSCMemberNomination");
        }

        [HttpGet]
        public ActionResult ViewITGRCTeam()
        {
            SearchParameterList paramsList = new SearchParameterList();
            try
            {
                TempData["PageHead"] = "View/Update ISSC Member";
                List<SYKI> iList1 = new List<SYKI>();
                // iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });
                var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIListForViewTeam();
                foreach (var item in kiData._SYKIList)
                {
                    iList1.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                }

                var curki = kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
                ViewBag.curki = curki;
                ViewBag.SYKIDatanew = new SelectList(iList1, "SYKIID", "KICODE");
                ViewBag.CurrentSYKIData = kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault(); ;
                ITDivisionApprovalgetSearchFilterData(new SearchParameterList());
                // List<ADORGLEVEL> _opList = _objHomeAssetRegistration.GetOrgLevelList((long)1);
                ViewBag.OpList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.DivisionList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
                //ViewBag.DivList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
                //var sykid = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();

                var ViewToList = _objHomeAssetRegistration.GetViewToITGRCTeamList((long)curki, null, null);

                ViewBag.ViewToList = ViewToList;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                throw ex;
            }
            return View();

        }

        [HttpPost]
        //[ValidateInput(false)]
        public ActionResult ViewITGRCTeam(long? SYKIDatanew, long? OPERATIONID, long? DIVISIONID)
        {
            TempData["PageHead"] = "View/Update ISSC Member";
            SearchParameterList paramsList = new SearchParameterList();
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["btnSubmit"]))
                {
                    List<SYKI> iList1 = new List<SYKI>();
                    // iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });
                    var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIListForViewTeam();
                    foreach (var item in kiData._SYKIList)
                    {
                        iList1.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                    }
                    ViewBag.SYKIDatanew = new SelectList(iList1, "SYKIID", "KICODE");
                    ViewBag.CurrentSYKIData = kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault();
                    ITDivisionApprovalgetSearchFilterData(new SearchParameterList());
                    // List<ADORGLEVEL> _opList = _objHomeAssetRegistration.GetOrgLevelList((long)1);
                    List<ADORGLEVEL> _opList = _objHomeAssetRegistration.BindOperation(SYKIDatanew);
                    ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP");
                    List<SearchParameterList> divList = _objHomeAssetRegistration.BindDivision(OPERATIONID);
                    ViewBag.DivisionList = new SelectList(divList, "DIVISIONID", "DIVISION", DIVISIONID);
                    //ViewBag.DivList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
                    //var sykid = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();

                    var ViewToList = _objHomeAssetRegistration.GetViewToITGRCTeamList(SYKIDatanew, OPERATIONID, DIVISIONID);
                    ViewBag.ViewToList = ViewToList;
                }
                if (!string.IsNullOrEmpty(Request.Form["Excel"]))
                {
                    var result = _objHomeAssetRegistration.GetViewToITGRCTeamList(SYKIDatanew, OPERATIONID, DIVISIONID).Select((x, index) => new
                    {
                        S_No = index + 1,
                        Ki = x.SYKI,
                        Operation_Name = x.OPERATIONNAME,
                        Division_Name = x.DIVISIONNAMEN,
                        ISSC_Member = x.EmpName,
                        Employee_Code = x.ADEMPCODE
                    }
                     ).ToList();
                    //byte[] filecontent = ExportExcel(result, true, 5);
                    //return File(filecontent, ExcelContentType, "Nominated_ISSC_Member_" + DateTime.Now + ".xls");

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                throw ex;
            }

            return View();

        }

        public ActionResult BindOperationId(long? SYKIID)
        {
            try { 
            List<ADORGLEVEL> divList = _objHomeAssetRegistration.BindOperation(SYKIID);
            return Json(divList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                throw ex;
            }
        }

        public ActionResult BindDivisionByOperationId(long id)
        {
            try { 
            List<SearchParameterList> divList = _objHomeAssetRegistration.BindDivision(id);
            return Json(divList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                throw ex;
            }
        }

        public ActionResult BindDivisionWithOldOperationHeadEmpCodeByOperationId(long id, long? sYKIID)
        {
            try { 
            AssetRegisterOperationHeadData assetRegisterOperationHeadData = new AssetRegisterOperationHeadData();
            assetRegisterOperationHeadData.divList = _objHomeAssetRegistration.BindDivision(id);
            assetRegisterOperationHeadData.OperatingHeadEmpCode = _objHomeAssetRegistration.BindDivisionWithOldOperationHeadEmpCodeByOperationId(id, sYKIID);
            return Json(assetRegisterOperationHeadData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                throw ex;
            }
        }

        public ActionResult BindOldDivisionHeadEmpCodeByDivisionId(long id, long? sYKIID)
        {
            try { 
            AssetRegisterOperationHeadData assetRegisterOperationHeadData = new AssetRegisterOperationHeadData();
            assetRegisterOperationHeadData.DivisionHeadEmpCode = _objHomeAssetRegistration.BindOldDivisionHeadEmpCodeByDivisionId(id, sYKIID);
            //assetRegisterOperationHeadData.ISSCMemCode = _objHomeAssetRegistration.BindOldDivisionISSCMemCodeByDivisionId(assetRegisterOperationHeadData.DivisionHeadEmpCode, sYKIID);
            return Json(assetRegisterOperationHeadData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                throw ex;
            }
        }

        [HttpGet]
        public ActionResult SetViewItGRCCancel()
        {
            return RedirectToAction("ViewITGRCTeam");
        }

        private SearchParameterList ITDivisionApprovalgetSearchFilterData(SearchParameterList paramsList)
        {

            List<SearchParameterList> list = new List<SearchParameterList>();
            SearchParameterList data = new SearchParameterList();
            List<SearchParameterList> kiData = new List<SearchParameterList>();

            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            kiData.Add(new SearchParameterList() { SYKIID = 0, SYKI = "Select" });

            var kiData1 = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            var kii = kiData1._SYKIList.ToList().OrderBy(x => x.SYKIID);
            foreach (var tblKiCode in kii)
            {
                kiData.Add(new SearchParameterList() { SYKIID = tblKiCode.SYKIID, SYKI = tblKiCode.KICODE });
            }
            var selectedKi = (paramsList.SYKIID == 0 || paramsList.SYKIID == null) ? 0 : paramsList.SYKIID;
            ViewBag.SYKIData = new SelectList(kiData, "SYKIID", "SYKI", selectedKi);
            //  data.ECode = paramsList.ECode;
            // data.EmpName = paramsList.EmpName;
            data.OPERATIONID = paramsList.OPERATIONID;
            data.DIVISIONID = paramsList.DIVISIONID;
            //data.SECTIONID = paramsList.SECTIONID;
            // data.DEPARTMENTID = paramsList.DEPARTMENTID;

            var activeKi = kiData1._SYKIList.Where(x => x.ACTIVE == 1).FirstOrDefault();
            // Employee_Details employeeDetails =_sessionService.Get<Employee_Details>("Employee")
            list.Add(data);
            ViewBag.List = list;
            return data;


        }

        public ActionResult EditNomation(int? id)
        {
            try { 
            TempData["PageHead"] = "ISSC Member Nomination - Update Setting";
            GetNominationDetails nomind = _objHomeAssetRegistration.getNomination(id);
            List<SearchParameterList> divList = _objHomeAssetRegistration.BindDivision(nomind.OPERATIONID);
            // var emp = _objHomeAssetRegistration.GetDivisionListForISSCMemberNomination((long)nomind.Ski, (long)nomind.OPERATIONID);
            List<empName> emplist_ = new List<empName>();
            emplist_ = _objHomeAssetRegistration.GetEmpnameList(nomind.Ski, nomind.OPERATIONID, nomind.DIVISIONID);
            ViewBag.empList = new SelectList(emplist_, "empcode", "Empname", nomind.empcode);
            return View(nomind);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                throw ex;
            }
        }

        [HttpPost]
        public ActionResult EditNomation(GetNominationDetails nomind)
        {
            try { 
            var userid = Convert.ToInt32(_sessionService.Get<string>("userID"));
            int result = _objHomeAssetRegistration.Update_NominationBy_Admin(nomind.Id, nomind.empcode, userid);
            var s = _objHomeAssetRegistration.GetOperatingHead_For_NomationUpdate(nomind.Ski, nomind.OPERATIONID, nomind.DIVISIONID, (long)nomind.empcode).FirstOrDefault();
            var current_ki = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            SendMailTo_ISSCMember_After_Admin_Update_ISSC_Member(s.Empname, s.Emailid, current_ki._SYKIList.Select(x => x.KICODE).FirstOrDefault());
            return RedirectToAction("ViewITGRCTeam");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                throw ex;
            }
        }

        private void SendMailTo_ISSCMember_After_Admin_Update_ISSC_Member(string Emp, string Emp_Emailid, string ski)
        {
            EmailCore sendMail = new EmailCore();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = Emp_Emailid; // user mail id // Emp_Emailid;
            sendMail.MailCc = "anupma.sharma@honda.hmsi.in";
            string strSubject = "ISSC Members Nomination Activity";

            string strBody = "<table cellpadding=0 cellspacing=0 style='border-style:solid; border-color:skyblue; border-width:1px; width:800px;'>" +
                            "<tr style='background-color:skyblue;'><td height=30>&nbsp;<b><font> ISSC Members Nomination " + ski + "</font></b></td> </tr>" +
                            "<tr> <td> <table cellpadding=0 cellspacing=0 style='margin:10px;'>" +
                            "<tr><td><b>Dear " + Emp + " San,</b></td></tr>" +
                            "<tr><td><br></td></tr>" +
                            "<tr><td valign=top>This is to inform you that, you are nominated " + ski + " Information Security Steering Committee (ISSC) member of your Division by your Operating Head. </td></tr>" +
                            "<tr><td><br><br></td></tr>" +
                            "<tr><td>Best Regards</td></tr>" +
                            "<tr><td>Team ISMS</td></tr>" +
                            "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr>" +
                            "</table> </td> </tr> </table>";

            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }

        [HttpGet]
        public ActionResult UpdateNominationDate(int? id)
        {
            try
            {
                TempData["PageHead"] = "Period Open for Information Security Steering Committee (ISSC) Member Nomination";
                List<SYKI> iList = new List<SYKI>();
                // iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });
                var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
                foreach (var item in kiData._SYKIList)
                {
                    iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                }

                ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE");

                var PeriodSettingList = _objHomeAssetRegistration.GetAsset_Register_periodList();
                ViewBag.PeriodSettingList = PeriodSettingList;
                PeriodForISSCMemberNominationVM? PeriodSetting = new PeriodForISSCMemberNominationVM();
                if (id > 0)
                {
                    PeriodSetting = PeriodSettingList.Where(x => x.PeriodForISSCMemberNominationID == id).Select(x => new PeriodForISSCMemberNominationVM
                    {
                        PeriodForISSCMemberNominationID = x.PeriodForISSCMemberNominationID,
                        SYKIID = x.SYKIID,
                        StartDate = x.StartDate.ToString("dd/mm/yyyy"),
                        EndDate = x.EndDate.ToString("dd/mm/yyyy"),
                        ActionType = 1,
                    }).FirstOrDefault();
                    ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE", PeriodSetting.SYKIID);

                }
                return View(PeriodSetting);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult EditUpdateNominationDate(int? id)
        {
            TempData["PageHead"] = "Period Open for Information Security Steering Committee (ISSC) Member Nomination";
            List<SYKI> iList = new List<SYKI>();
            // iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });
            var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }

            ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE");

            List<ADORGLEVEL> _opList = _objHomeAssetRegistration.GetOrgLevelList((long)1);
            ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP");
            ViewBag.DivisionList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");

            var PeriodSettingList = _objHomeAssetRegistration.GetAsset_Register_periodList();
            ViewBag.PeriodSettingList = PeriodSettingList;
            PeriodForISSCMemberNominationVM? PeriodSetting = new PeriodForISSCMemberNominationVM();
            if (id > 0)
            {
                PeriodSetting = PeriodSettingList.Where(x => x.PeriodForISSCMemberNominationID == id).Select(x => new PeriodForISSCMemberNominationVM
                {
                    PeriodForISSCMemberNominationID = x.PeriodForISSCMemberNominationID,
                    SYKIID = x.SYKIID,
                    StartDate = x.StartDate.ToString("dd/MM/yyyy"),
                    EndDate = x.EndDate.ToString("dd/MM/yyyy"),
                    ActionType = 1,
                }).FirstOrDefault();
                ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE", PeriodSetting.SYKIID);

            }
            return View(PeriodSetting);
        }

        [HttpPost]
        public ActionResult EditUpdateNominationDate(PeriodForISSCMemberNominationVM model)
        {
            var userid = Convert.ToInt32(_sessionService.Get<string>("userID"));
            model.ADDEDBY = userid;
            if (model.OPERATIONID == null)
            {
                PeriodForISSCMemberNominationVM PeriodSetting = _objHomeAssetRegistration.InsertUpdatePeriodSettingDetail(model);
            }
            else if (model.OPERATIONID != null)
            {
                PeriodForISSCMemberNominationVM PeriodSetting = _objHomeAssetRegistration.UpdateNomination(model);
            }

            return RedirectToAction("UpdateNominationDate");
        }

        [HttpGet]
        public ActionResult PeriodSettingITGRC()
        {
            TempData["PageHead"] = "Period Open for Information Asset Register Update";
            List<SYKI> iList = new List<SYKI>();
            // iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });
            var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }

            ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE");

            List<ADORGLEVEL> _opList = _objHomeAssetRegistration.GetOrgLevelList((long)1);
            ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP");
            ViewBag.DivisionList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
            var PeriodSettingList = _objHomeAssetRegistration.GetAsset_Register_period_ITGRC_List();
            ViewBag.PeriodSettingList = PeriodSettingList;
            ViewBag.CurrentSYKIData = iList.Select(x => x.KICODE).FirstOrDefault();
            return View();
        }

        [HttpGet]
        public ActionResult SameDivisionAndOperationApproval()
        {
            TempData["PageHead"] = "Same Division And Operation Approval";
            List<SYKI> iList = new List<SYKI>();
            // iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });
            var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }

            ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE");

            List<ADORGLEVEL> _opList = _objHomeAssetRegistration.GetOrgLevelList((long)1);
            ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP");
            ViewBag.DivisionList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
            ViewBag.CurrentSYKIData = iList.Select(x => x.KICODE).FirstOrDefault();
            return View();
        }

        [HttpPost]
        public ActionResult SameDivisionAndOperationApproval(SameDivisionAndOprationApprovalVM model)
        {
            var userid = Convert.ToInt32(_sessionService.Get<string>("userID"));
            model.CreatedBy = userid;
            var msg = _objHomeAssetRegistration.OldOperatingOROldDivHeadRequestAssignToNewOpHeadOrNewDivHead(model);
            TempData["msg"] = msg;
            return RedirectToAction("SameDivisionAndOperationApproval");
        }


        [HttpPost]
        public JsonResult AssetApplicabilityUpdate(List<decimal> selectIDs)
        {
            AssetApplicabilityUpdateVM assetApplicabilityUpdateVM = new AssetApplicabilityUpdateVM();
            assetApplicabilityUpdateVM.lstCommonAAssetsRegListVM = new List<CommonAAssetsRegListVM>();
            try
            {
                //string[] ids = selectIDs.Split(',');
                assetApplicabilityUpdateVM.SelectedIds = selectIDs;
                if (selectIDs.Count() > 0)
                {
                    assetApplicabilityUpdateVM.lstCommonAAssetsRegListVM = _objHomeAssetRegistration.Get_Common_Asset_Details(selectIDs);

                    //foreach (var item in selectIDs)
                    //{
                    //    var newData = _objHomeAssetRegistration.Get_Edit_Asset_Details(Convert.ToInt16(item));
                    //    if (newData != null)
                    //    {
                    //        CommonAAssetsRegListVM objModel = new CommonAAssetsRegListVM();
                    //        objModel.ID = newData.ID;
                    //        objModel.Primary = newData.PrimaryId;
                    //        objModel.Secondary = newData.Secondary;
                    //        objModel.AssetsType = newData.AssetsType;
                    //        objModel.Assetlocation = newData.Assetlocation;
                    //        objModel.AssetOwner = newData.AssetOwner;
                    //        objModel.Custodian = newData.Custodian;
                    //        objModel.AssetUser = newData.AssetUser;
                    //        objModel.Retention_Remarks = newData.Retention_Remarks;
                    //        objModel.ActionType = newData.ActionType;
                    //        objModel.AssetsID = newData.AssetsID;
                    //        objModel.Retention_Period = newData.Retention_Period;
                    //        objModel.ClassificationID = newData.ClassificationID;
                    //        objModel.Reason_NA = newData.ISSC_Member_Reason;
                    //        assetApplicabilityUpdateVM.lstCommonAAssetsRegListVM.Add(objModel);
                    //    }
                    //}

                    return Json(assetApplicabilityUpdateVM);
                }
            }
            catch (Exception ex)
            {
            }
            return Json(assetApplicabilityUpdateVM);
        }

        [HttpPost]
        public ActionResult BulkUpdateAssetApplicability(AssetApplicabilityUpdateVM model, string SelectedIds)
        {
            var userid = Convert.ToInt32(_sessionService.Get<string>("userID"));
            CultureInfo culture = new CultureInfo("en-GB");
            model.CreatedBy = userid;
            model.SelectedIds = SelectedIds.Split(',').Select(x => Convert.ToDecimal(x)).ToList();
            var result = _objHomeAssetRegistration.BulkUpdateAssetApplicability(model);
            TempData["msg"] = result == 1 ? "Data has been updated succesufully." : "There is some problem, please try again later.";

            //return RedirectToAction("AssetRegister?IsEdit=Y"); // Added By Aumento :: SR102194
            return RedirectToAction("AssetRegister", "AssetRegistration", new { IsEdit = "Y" }); // Added By Aumento :: SR102194

        }


        [HttpPost]
        public JsonResult BulkUpdate(string selectIDs)
        {
            //List<PeriodForITGRCMMemberNominationListVM> lstPeriodForITGRCMMemberNominationListVM = new List<PeriodForITGRCMMemberNominationListVM>();
            BulkUpdatePeriodForITGRCMMemberNominationVM BulkUpdate = new BulkUpdatePeriodForITGRCMMemberNominationVM();
            BulkUpdate.lstPeriodForITGRC = new List<PeriodForITGRCMMemberNominationListVM>();
            BulkUpdate.PeriodForITGRC = new PeriodForITGRCMMemberNominationListVM();
            try
            {
                //string trimIds = selectIDs.Remove(selectIDs.LastIndexOf(',')).TrimEnd();
                string[] ids = selectIDs.Split(',');
                BulkUpdate.PeriodForITGRC.SelectedIds = selectIDs;
                if (ids.Length > 0)
                {
                    foreach (var item in ids)
                    {
                        var newLst = _objHomeAssetRegistration.GetAsset_Register_period_ITGRC_List_WithID(Convert.ToInt16(item));
                        if (newLst.Count > 0 && newLst != null)
                        {
                            foreach (var items in newLst)
                            {
                                PeriodForITGRCMMemberNominationListVM objModel = new PeriodForITGRCMMemberNominationListVM();
                                objModel.SYKI = items.SYKI;
                                objModel.OperationName = items.OperationName;
                                objModel.DivisionName = items.DivisionName;
                                DateTime sdate = Convert.ToDateTime(items.StartDate);
                                DateTime edate = Convert.ToDateTime(items.EndDate);
                                objModel.StartDate = sdate.ToString("dd/MM/yyyy");
                                objModel.EndDate = edate.ToString("dd/MM/yyyy");
                                BulkUpdate.lstPeriodForITGRC.Add(objModel);
                                BulkUpdate.PeriodForITGRC.SYKI = items.SYKI;
                            }

                        }
                    }
                }

                return Json(BulkUpdate);
            }
            catch (Exception ex)
            {
            }
            return Json(BulkUpdate);
        }

        [HttpGet]
        public ActionResult PeriodSettingITGRCEdit(int? id)
        {

            TempData["PageHead"] = "Period Open for Information Asset Register Update";
            List<SYKI> iList = new List<SYKI>();
            // iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });
            var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }

            ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE");

            List<ADORGLEVEL> _opList = _objHomeAssetRegistration.GetOrgLevelList((long)1);
            ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP");
            ViewBag.DivisionList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");

            var PeriodSettingList = _objHomeAssetRegistration.GetAsset_Register_period_ITGRC_List_WithID(id);
            ViewBag.PeriodSettingList = PeriodSettingList;

            PeriodForITGRCMemberNominationListNewVM? PeriodSetting = new PeriodForITGRCMemberNominationListNewVM();
            if (id > 0)
            {
                PeriodSetting = PeriodSettingList.Where(x => x.ID == id).Select(x => new PeriodForITGRCMemberNominationListNewVM
                {
                    ID = (decimal)x.ID,
                    SYKIID = (decimal)x.SYKIID,
                    SYKI = x.SYKI,
                    OPERATIONID = (long)x.OPERATIONID,
                    DIVISIONID = (long)x.DIVISIONID,
                    OperationName = x.OperationName,
                    DivisionName = x.DivisionName,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    CreatedBy = (long)x.CreatedBy,
                    CreationDate = (DateTime)x.CreationDate,
                }).FirstOrDefault();
                ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE", PeriodSetting.SYKIID);
                List<SearchParameterList> divList = _objHomeAssetRegistration.BindDivision(PeriodSetting.OPERATIONID);
                ViewBag.DivisionList = new SelectList(divList, "DIVISIONID", "DIVISION", PeriodSetting.DIVISIONID);
            }

            return View(PeriodSetting);
        }

        [HttpPost]
        public ActionResult InsertUpdatePeriodITGRCSetting(PeriodForITGRCMMemberNominationListVM model)
        {
            PeriodForITGRCMemberNominationListVM NominationListVM = new PeriodForITGRCMemberNominationListVM();

            var userid = Convert.ToInt32(_sessionService.Get<string>("userID"));
            NominationListVM.CreatedBy = userid;
            NominationListVM.StartDate = model.StartDate;
            NominationListVM.EndDate = model.EndDate;
            NominationListVM.SYKIID = (decimal)model.SYKIID;
            CultureInfo culture = new CultureInfo("en-GB");
            if (model.ID == 0 || model.ID == null)
            {
                NominationListVM = _objHomeAssetRegistration.InsertUpdatePeriodITGRCSettingDetail(NominationListVM);
            }
            else if (model.ID != 0)
            {
                //PeriodForITGRCMemberNominationListVM PeriodSetting = _objHomeAssetRegistration.UpdateITGRCNomination(model);
            }

            if (NominationListVM.Status == 1)
            {
                TempData["msg"] = NominationListVM.Msg;

                var sendmailist = _objHomeAssetRegistration.GetISSCMemberList((long)model.SYKIID, (long)model.OPERATIONID, (long)model.DIVISIONID);

                foreach (var item in sendmailist)
                {
                    SendMailTo_ISSC_Member_ToFill_Asset_Details(item.Empname, item.Emailid, Convert.ToDateTime(model.StartDate, culture), Convert.ToDateTime(model.EndDate, culture));
                }
            }
            //else if (model.Status == 2)
            //{
            //    TempData["msg"] = model.Msg;

            //    var sendmailist = _objHomeAssetRegistration.GetISSCMemberList((long)model.SYKIID, (long)model.OPERATIONID, (long)model.DIVISIONID);

            //    foreach (var item in sendmailist)
            //    {
            //        SendMailTo_ISSC_Member_ToFill_Asset_Details(item.Empname, item.Emailid, (DateTime)model.StartDate, (DateTime)model.EndDate);
            //    }
            //}
            else
            {
                TempData["msg"] = NominationListVM.Msg;
            }
            return RedirectToAction("PeriodSettingITGRC");
        }

        [HttpPost]
        public ActionResult BulkUpdatePeriodITGRCSettingNew(BulkUpdatePeriod model)
        {
            var userid = Convert.ToInt32(_sessionService.Get<string>("userID"));
            CultureInfo culture = new CultureInfo("en-GB");
            string[] ids = model.SelectedIds.Split(',');
            if (ids.Length > 0)
            {
                foreach (var item in ids)
                {
                    BulkUpdatePeriod bulkObj = new BulkUpdatePeriod();
                    bulkObj.CreatedBy = userid;
                    bulkObj.ID = Convert.ToDecimal(item);
                    bulkObj.StartDate = model.StartDate;
                    bulkObj.EndDate = model.EndDate;
                    var result = _objHomeAssetRegistration.BulkUpdateITGRCNomination(bulkObj);

                }
                var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
                var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
                var emailList = _objHomeAssetRegistration.GetISSCMemberForBulkMailList((long)current_ki, ids);

                foreach (var item in emailList)
                {
                    SendMailTo_ISSC_Member_ToFill_Asset_Details(item.Empname, item.Emailid, Convert.ToDateTime(model.StartDate, culture), Convert.ToDateTime(model.EndDate, culture));
                }
                TempData["msg"] = "Data has been update succesufully.";
            }
            return RedirectToAction("PeriodSettingITGRC");
        }


        [HttpPost]
        public ActionResult InsertUpdatePeriodITGRCSettingNew(PeriodForITGRCMemberNominationListNewVM model, int? id)
        {
            var userid = Convert.ToInt32(_sessionService.Get<string>("userID"));
            CultureInfo culture = new CultureInfo("en-GB");
            model.CreatedBy = userid;
            if (id != 0)
            {
                PeriodForITGRCMemberNominationListNewVM PeriodSetting = _objHomeAssetRegistration.UpdateITGRCNomination(model);
            }
            if (model.Status == 2)
            {
                TempData["msg"] = model.Msg;

                var sendmailist = _objHomeAssetRegistration.GetISSCMemberList((long)model.SYKIID, (long)model.OPERATIONID, (long)model.DIVISIONID);

                foreach (var item in sendmailist)
                {
                    SendMailTo_ISSC_Member_ToFill_Asset_Details(item.Empname, item.Emailid, Convert.ToDateTime(model.StartDate, culture), Convert.ToDateTime(model.EndDate, culture));
                }
            }
            else
            {
                TempData["msg"] = model.Msg;
            }
            return RedirectToAction("PeriodSettingITGRC");
        }

        private void SendMailTo_ISSC(string Emp, string Emp_Emailid, DateTime StateDate, DateTime EndDate)
        {
            EmailCore sendMail = new EmailCore();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = Emp_Emailid;// user mail id
            sendMail.MailCc = "anupma.sharma@honda.hmsi.in";
            string strSubject = "Nomination For Asset portal";
            string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-width:1px; width:1024px;'>" +
                             "<tr><td height=30>&nbsp;<b><font>Hi  - " + Emp + " San</font></b></td>" +
                             "</tr><tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td width=125 height=22 valign=top></td><td width=1024 valign=top>Requested to nominate ISSC memeber between the follwing date</td>" +
                             "</tr><tr> " +
                             "<td width=125 valign=top>Start Date</td><td width=389 valign=top>" + StateDate.ToString("dd/MM/yyyy") + "</td></tr><tr><td width=125 valign=top>End Date</td>" +
                             "<td width=389 valign=top>" + EndDate.ToString("dd/MM/yyyy") + "</td></tr>" +
                             "<tr><td valign=top colspan=2>Please login <a href='https://portal.honda2wheelersindia.com/SSO'> Employee Portal</a> for approval process.</td></tr>" +
                             "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";
            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }

        private void SendMailTo_ISSC_Member_ToFill_Asset_Details(string Emp, string Emp_Emailid, DateTime StateDate, DateTime EndDate)
        {
            var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            var ski = kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault();
            EmailCore sendMail = new EmailCore();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = Emp_Emailid;// user mail id
            sendMail.MailCc = "anupma.sharma@honda.hmsi.in";
            string strSubject = "Information Asset Register Updation";
            string strBody = "<table cellpadding=0 cellspacing=0 style='border-style:solid; border-color:skyblue; border-width:1px; width:800px;'>" +
                               "<tr style='background-color:skyblue;'>Information Asset Register Updation " + ski + "</tr>" +
                               "<tr> <td> <table cellpadding=0 cellspacing=0 style='margin:10px;'>" +
                               "<tr><td><b>Dear " + Emp + " San,</b></td></tr>" +
                               "<tr><td><br></td></tr>" +
                               "<tr><td valign=top>This is to inform you that you are eligible for ISO27001 - Information Asset Register updation for  " + ski + ".</td></tr>" +
                               "<tr><td><br></td></tr>" +
                               "<tr><td valign=top>Information Asset Register updation window will be open from " + StateDate.ToString("dd/MM/yyyy") + " to " + EndDate.ToString("dd/MM/yyyy") + ".</td></tr>" +
                               "<tr><td><br></td></tr>" +
                               "<tr><td valign=top colspan=2>Please Login into <a href='https://portal.honda2wheelersindia.com/SSO'>E-Portal</a> and ensure timely completion of activity.</td></tr>" +
                               "<tr><td><br><br></td></tr>" +
                               "<tr><td>Best Regards</td></tr>" +
                               "<tr><td>Team ISMS</td></tr>" +
                               "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr>" +
                               "</table> </td> </tr> </table>";
            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }

        [HttpGet]
        public ActionResult CommonAssetsRegister(int? id)
        {
            TempData["PageHead"] = "Common Assets Management";
            //List<SYKI> iList = new List<SYKI>();
            //// iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });
            //var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            //foreach (var item in kiData._SYKIList)
            //{
            //    iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            //}

            //ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE");

            //List<AssetType> AssetTypelist_ = new List<AssetType>();
            //AssetTypelist_ = _objHomeAssetRegistration.GetAssetTypeList();
            //ViewBag.AssetType = new SelectList(AssetTypelist_, "AssetID", "AssetName");

            List<AssetClassification> AssetClassificationlist_ = new List<AssetClassification>();
            AssetClassificationlist_ = _objHomeAssetRegistration.GetAssetClassificationList();
            ViewBag.AssetClassification = new SelectList(AssetClassificationlist_, "CLASSIFICATIONID", "CLASSIFICATION");

            var PeriodSettingList = _objHomeAssetRegistration.Get_CommonAsset_Register_List();
            ViewBag.PeriodSettingList = PeriodSettingList;
            CommonAAssetsRegListVM? PeriodSetting = new CommonAAssetsRegListVM();
            if (id > 0)
            {
                PeriodSetting = PeriodSettingList.Where(x => x.ID == id).Select(x => new CommonAAssetsRegListVM
                {
                    ID = x.ID,
                    Primary = x.Primary,
                    //  SYKIName = x.SYKIName,
                    // Secondary = x.Secondary,
                    //AssetsType = x.AssetsType,
                    //Classification = x.Classification,
                    // AssetsID = x.AssetsID,
                    ClassificationID = x.ClassificationID,
                    Active = x.Active,
                    CreatedBy = x.CreatedBy,
                    CreationDate = x.CreationDate,
                }).FirstOrDefault();
                //ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE", PeriodSetting.SYKIID);
                return View(PeriodSetting);
            }
            return View("CommonAssetsRegister");
        }

        [HttpPost]
        public ActionResult InsertUpdateCommonAssetSetting(CommonAssetsRegListVM model, int? id)
        {
            var userid = Convert.ToInt32(_sessionService.Get<string>("userID"));
            model.CreatedBy = userid;
            if (id == 0 || id == null)
            {
                CommonAssetsRegListVM PeriodSetting = _objHomeAssetRegistration.InsertUpdateCommonAssetRegisterSettingDetail(model);
            }
            else if (id != 0)
            {
                CommonAssetsRegListVM PeriodSetting = _objHomeAssetRegistration.UpdateCommonAssetRegister(model);
            }
            return RedirectToAction("CommonAssetsRegister");
        }

        private void SendMailTo_OperatingHead(string Emp, string Emp_Emailid, DateTime StateDate, DateTime EndDate, string ski)
        {
            EmailCore sendMail = new EmailCore();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = Emp_Emailid; // user mail id // Emp_Emailid;
            sendMail.MailCc = "anupma.sharma@honda.hmsi.in";
            string strSubject = "ISSC Members Nomination Activity";
            string strBody = "<table cellpadding=0 cellspacing=0 style='border-style:solid; border-color:skyblue; border-width:1px; width:800px;'>" +
                                "<tr style='background-color:skyblue;'><td height=30>&nbsp;<b><font> ISSC Members Nomination " + ski + "</font></b></td> </tr>" +
                                "<tr> <td> <table cellpadding=0 cellspacing=0 style='margin:10px;'>" +
                                "<tr><td><b>Dear Sir,</b></td></tr>" +
                                "<tr><td><br></td></tr>" +
                                "<tr><td valign=top>This is to inform you that Information Security Steering Committee (ISSC) members nomination activity has been initiated for " + ski + ".</td></tr>" +
                                "<tr><td valign=top>ISSC Member nomination window will be open from " + StateDate.ToString("dd/MM/yyyy") + " to " + EndDate.ToString("dd/MM/yyyy") + ".</td></tr>" +
                                "<tr><td><br></td></tr>" +
                                "<tr><td valign=top colspan=2>You are requested to Login into <a href='https://portal.honda2wheelersindia.com/SSO'>E-Portal</a> and nominate " + ski + " ISSC member.</td></tr>" +
                                "<tr><td><br><br></td></tr>" +
                                "<tr><td>Best Regards</td></tr>" +
                                "<tr><td>Team SIS</td></tr>" +
                                "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr>" +
                                "</table> </td> </tr> </table>";
            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }

        private void SendMailTo_OperatingHead_After_Update(string Emp, string Emp_Emailid)
        {
            EmailCore sendMail = new EmailCore();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = Emp_Emailid;// user mail id
            sendMail.MailCc = "anupma.sharma@honda.hmsi.in";
            string strSubject = "ISSC Member Update For Asset portal";
            string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-width:1px; width:1024px;'>" +
                             "<tr><td height=30>&nbsp;<b><font>Hi  - " + Emp + " San</font></b></td>" +
                             "</tr><tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td width=125 height=22 valign=top>Employee Code</td><td width=389 valign=top>As per your Request the ISSC memeberhas been updated.</td>" +
                             "</tr><tr> " +
                             "<td width=125 valign=top></td><td width=389 valign=top></td></tr><tr><td width=125 valign=top></td>" +
                             "<td width=389 valign=top></td></tr>" +
                             "<tr><td valign=top colspan=2>Please login <a href='https://portal.honda2wheelersindia.com/SSO'> Employee Portal</a> for approval process.</td></tr>" +
                             "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";
            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }

        [HttpGet]
        public ActionResult AssetDashboard()
        {
            TempData["PageHead"] = "Manage Information Asset Register";
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var result = _objHomeAssetRegistration.Get_Division_For_ISSC_Member(userId);
            ViewBag.Division = result.DivisionName;
            ViewBag.ISSC = result.ISSCMember;
            ViewBag.ISSC_MemberCode = userId;
    
            ViewBag.ISSCStatus = result.ISSC_Member_Submit_Status;
            ViewBag.ISSCFinalDate = result.ISSC_Member_Submit_Date;

            ViewBag.Divisionstatus = result.DivisionHead_Submit_Status;
            ViewBag.DivisionFinalDate = result.DivisionHead_Submit_Date;

            ViewBag.OperatingStatus = result.OperatingHead_Submit_Status;
            ViewBag.OperatingFinalDate = result.OperatingHead__Submit_Date;
            ViewBag.DHeadName = result.DivisionHead_Name;
            ViewBag.OHeadName = result.OperatingHead_Name;
            //Added by Aumento for SR91196
            ViewBag.ITGRCHeadName = result.ITGRC_Head_Name;
            ViewBag.ITGRCstatus = result.ITGRCHead_Submit_Status;
            ViewBag.ITGRCFinalDate = result.ITGRCHead_Submit_Date;
            //Added by Aumento for SR91196
            ViewBag.IsOpMatch = result.IS_ITOPRATION;  // Added By Aumento :: SR102194
            //Added by aumento as on 17062024 for SR71836---------------------------------------------

            //Added by Aumento for SR91196
            //Added by aumento as on 17062024 for SR71836---------------------------------------------

            if (result.OperatingHead__Submit_Date!=null || result.OperatingHead_Submit_Status!=null)
            {
                ViewBag.IsEditbtnVisible = "True";
            }
            else if(result.DivisionHead_Submit_Date!=null && result.DivisionHead_Submit_Status!=null && (result.OperatingHead_Name == null || result.OperatingHead_Name == ""))
            {
                ViewBag.IsEditbtnVisible = "True";
            }
            else
            {
                ViewBag.IsEditbtnVisible = "False";
            }

            //-----------------------------------------------------------------------------------------

            DateTime? dt = result.EndDate;
            ViewBag.endate = String.Format("{0:dd/MM/yyyy}", dt);

            DateTime end_dates = Convert.ToDateTime(result.EndDate);
            int nodaysleft = end_dates.Subtract(DateTime.Now.Date).Days;
            if (result.ISSCMember != null && result.OperatingHead_Submit_Status == null)
                ViewBag.nodaysleft = nodaysleft + " Days";
            else
                ViewBag.nodaysleft = "";

            List<SYKI> iList1 = new List<SYKI>();
            // iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });
            var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIListForDashboard();
            foreach (var item in kiData._SYKIList)
            {
                iList1.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            ViewBag.Lastki_details = iList1;
            return View();
        }

        [HttpGet]
        public ActionResult OldAssetRegisterData()
        {
            TempData["PageHead"] = "Information Asset Register updation Form";

            List<SYKI> iList = new List<SYKI>();
            // iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });
            var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
            long OldISSCMemID = _objHomeAssetRegistration.GetOldISSC_MemberByCurrentISSCMemCode((long)current_ki, userId);

            var pasd_ = _objHomeAssetRegistration.Get_Primary_Asset(OldISSCMemID, (long)current_ki).Distinct().Select(x => x.PrimaryAsset).Distinct().ToList();
            List<Primary_AssetDeatils_VM> pasd1 = new List<Primary_AssetDeatils_VM>();
            foreach (var item in pasd_)
            {
                pasd1.Add(new Primary_AssetDeatils_VM { Id = item, PrimaryAsset = item });
            }

            ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE");
            ViewBag.PrimaryId = new SelectList(pasd1, "ID", "PrimaryAsset");

            List<Asset_Year> year = new List<Asset_Year>();

            for (int i = 0; i <= 20; i++)
            {
                //var rt_year = DateTime.Now.AddYears(i).Year;
                var rt_year = i;
                year.Add(new Asset_Year { Id = Convert.ToString(rt_year), Text = Convert.ToString(rt_year) });
            }
            ViewBag.Retention_Year = new SelectList(year, "Id", "Text");
            CommonAAssetsRegListVM PeriodSetting = new CommonAAssetsRegListVM();
            PeriodSetting.SYKIID = iList.Select(x => x.SYKIID).FirstOrDefault();
            ViewBag.Asset_details_List = _objHomeAssetRegistration.Get_Asset_Details_With_Common_Asset(OldISSCMemID, (long)current_ki).ToList();
            ViewBag.EnddateCheck =  _objHomeAssetRegistration.ISSC_Member_AssetDetails_EndDate_Check((int)OldISSCMemID, (long)current_ki);
            ViewBag.Final_Submit_Check = _objHomeAssetRegistration.ISSC_Member_AssetDetails_FinalSubmit_Check((int)OldISSCMemID, (long)current_ki);
            var lastki = current_ki - 1;
            ViewBag.Last_Ki_Asset_details_List = _objHomeAssetRegistration.Get_Last_Ki_Asset_Details(OldISSCMemID, (long)lastki);
            ViewBag.Remarks = _objHomeAssetRegistration.GetRemrksForISSCMember((long)current_ki, (int)OldISSCMemID);
            ViewBag.DRemarks = _objHomeAssetRegistration.GetDeficencyRemrks((long)current_ki, (int)OldISSCMemID);
            ViewBag.OrgnizationMapping_check = _objHomeAssetRegistration.Check_OrganizationMapping((long)current_ki, (int)OldISSCMemID);
            return View(PeriodSetting);
        }

        [HttpGet]
        public ActionResult AssetRegister(int? Id, string IsEdit) // Added By Aumento :: SR102194
        {
            try
            {

          
            TempData["PageHead"] = "Information Asset Register updation Form";

            List<SYKI> iList = new List<SYKI>();
            // iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });
            var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
            ViewBag.OldISSC_MemberCode = _objHomeAssetRegistration.GetOldISSC_MemberByCurrentISSCMemCode((long)current_ki, userId);
            int nominationcheck = _objHomeAssetRegistration.Check_Issc_Member_Nomination((long)current_ki, userId);

            if (nominationcheck == 0)
                TempData["msg"] = "Only Information Security Steering Committee Member is Authorized to update Asset Register.";


            if (nominationcheck == 0)
                return RedirectToAction("AssetDashboard");

             var pasd_ = _objHomeAssetRegistration.Get_Primary_Asset(userId, (long)current_ki).Distinct().Select(x => x.PrimaryAsset).Distinct().ToList();
            List<Primary_AssetDeatils_VM> pasd1 = new List<Primary_AssetDeatils_VM>();
            foreach (var item in pasd_)
            {
                pasd1.Add(new Primary_AssetDeatils_VM { Id = item, PrimaryAsset = item });
            }


            if (Id == null)
            {
                List<Primary_AssetDeatils_VM> pasd = new List<Primary_AssetDeatils_VM>
            {
                new Primary_AssetDeatils_VM { Id="-1",PrimaryAsset="Other"}
            };
                pasd1.AddRange(pasd);
            }
            if (Id != null)
            {
                List<Primary_AssetDeatils_VM> pasd_Na = new List<Primary_AssetDeatils_VM>
                {
                    //new Primary_AssetDeatils_VM { Id="-2",PrimaryAsset="NA"}
                };
                pasd1.AddRange(pasd_Na);
            }

            ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE");
            ViewBag.PrimaryId = new SelectList(pasd1, "Id", "PrimaryAsset");
                ViewBag.IsEdit = IsEdit;  // Added By Aumento :: SR102194

                List<Asset_Year> year = new List<Asset_Year>();

            for (int i = 0; i <= 20; i++)
            {
                //var rt_year = DateTime.Now.AddYears(i).Year;
                var rt_year = i;
                year.Add(new Asset_Year { Id = Convert.ToString(rt_year), Text = Convert.ToString(rt_year) });
            }
            ViewBag.Retention_Year = new SelectList(year, "Id", "Text");
            CommonAAssetsRegListVM PeriodSetting = new CommonAAssetsRegListVM();
            PeriodSetting.SYKIID = iList.Select(x => x.SYKIID).FirstOrDefault();
                //  Added by TTL ::  SR103777 > CR6964  
                //ViewBag.Asset_details_List = _objHomeAssetRegistration.Get_Asset_Details_With_Common_Asset(userId, (long)current_ki).ToList();
                var Asset_details_List_ = _objHomeAssetRegistration.Get_Asset_Details_With_Common_Asset(userId, (long)current_ki).ToList();
                ViewBag.Asset_details_List = Asset_details_List_;
                ViewBag.ISSCMEMBERDATETIME = Asset_details_List_.Max(x => x.FINALSUBMITDATE_ISSCMEMBER);
                //  Added by TTL ::  SR103777 > CR6964  
                ViewBag.EnddateCheck = _objHomeAssetRegistration.ISSC_Member_AssetDetails_EndDate_Check(userId, (long)current_ki);
            ViewBag.Final_Submit_Check = _objHomeAssetRegistration.ISSC_Member_AssetDetails_FinalSubmit_Check(userId, (long)current_ki);
            var lastki = current_ki - 1;
            ViewBag.Last_Ki_Asset_details_List = _objHomeAssetRegistration.Get_Last_Ki_Asset_Details(userId, (long)lastki);

            ViewBag.Remarks = _objHomeAssetRegistration.GetRemrksForISSCMember((long)current_ki, userId);
                var Result = _objHomeAssetRegistration.GetRemrksForISSCMember((long)current_ki, userId);  // Added By Aumento :: SR102194
                ViewBag.IS_ITOPRATION = Result.FirstOrDefault()?.IS_ITOPRATION;  // Added By Aumento :: SR102194

                ViewBag.DRemarks = _objHomeAssetRegistration.GetDeficencyRemrks((long)current_ki, userId);

            ViewBag.OrgnizationMapping_check = _objHomeAssetRegistration.Check_OrganizationMapping((long)current_ki, userId);

            if (Id != null)
            {
                CommonAAssetsRegListVM EditAsset_details = _objHomeAssetRegistration.Get_Edit_Asset_Details(Id);
                EditAsset_details.SYKIID = iList.Select(x => x.SYKIID).FirstOrDefault();
                ViewBag.Primary_Asset_Value = EditAsset_details.PrimaryId;
                ViewBag.Retention_Year_value = EditAsset_details.Retention_Year;
                ViewBag.Action_Value = EditAsset_details.ActionType;
                return View(EditAsset_details);
            }
                //Added by aumento as on 01012025 for SR71836------------------------------------------------------------------------
                //Block of Code Removed From Here Added By Aumento :: SR102194
                //if (_objHomeAssetRegistration.ISSC_Member_AssetDetails_FinalSubmit_Check(userId, (long)current_ki) != 0)
                //{


                //    var result1 = _objHomeAssetRegistration.Get_Division_For_ISSC_Member(Convert.ToInt32(_sessionService.Get<string>("userID")));
                //    if (result1.OperatingHead__Submit_Date != null || result1.OperatingHead_Submit_Status != null)
                //    {
                //        ViewBag.IsEditbtnVisible = "True";
                //        if (ViewBag.IsEditbtnVisible == "True")
                //        {
                //            _objHomeAssetRegistration.UpdateAssetRegister((long)current_ki, Convert.ToInt32(_sessionService.Get<string>("userID")));
                //        }
                //    }
                //    else if (result1.DivisionHead_Submit_Date != null && result1.DivisionHead_Submit_Status != null && (result1.OperatingHead_Name == null || result1.OperatingHead_Name == ""))
                //    {
                //        ViewBag.IsEditbtnVisible = "True";
                //        if (ViewBag.IsEditbtnVisible == "True")
                //        {
                //            _objHomeAssetRegistration.UpdateAssetRegister((long)current_ki, Convert.ToInt32(_sessionService.Get<string>("userID")));
                //        }
                //    }
                //    else
                //    {
                //        ViewBag.IsEditbtnVisible = "False";
                //    }



                //    var divison_head = _objHomeAssetRegistration.Get_DivisionHead_((long)current_ki, Convert.ToInt32(_sessionService.Get<string>("userID")));

                //    if (divison_head != null)
                //    {
                //        PeriodSetting.Division_Head_EmpCode = divison_head.DivisionEmpCode;
                //    }

                //    var result = _objHomeAssetRegistration.FinalSubmit_AssetDetails(PeriodSetting);
                //    //TempData["ResultMsg"] = result;

                //    if (divison_head != null)
                //    {
                //        SendMailTo_DivisionHead_After_Issc_Member(divison_head.Empname, divison_head.Emailid, kiData._SYKIList.Select(x => x.KICODE).FirstOrDefault(), divison_head.ISSC_Member_Name, divison_head.DivisionName);
                //    }
                //    return RedirectToAction("AssetRegister");

                //}
                ////-----------------------------------------------------------------------------------------
                return View(PeriodSetting);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult AssetRegister(CommonAAssetsRegListVM assetdetails)
        {
            TempData["PageHead"] = "Information Asset Register updation Form";
            List<SYKI> iList = new List<SYKI>();
            // iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });
            var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
            List<Primary_AssetDeatils_VM> pasd1 = _objHomeAssetRegistration.Get_Primary_Asset(userId, (long)current_ki).ToList();
            List<Primary_AssetDeatils_VM> pasd = new List<Primary_AssetDeatils_VM>
            {
                new Primary_AssetDeatils_VM { Id="-1",PrimaryAsset="Other"}
            };
            pasd1.AddRange(pasd);
            ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE");
            ViewBag.PrimaryId = new SelectList(pasd1, "Id", "PrimaryAsset");
            List<Asset_Year> year = new List<Asset_Year>();

            for (int i = 0; i <= 20; i++)
            {
                //var rt_year = DateTime.Now.AddYears(i).Year;
                var rt_year = i;
                year.Add(new Asset_Year { Id = Convert.ToString(rt_year), Text = Convert.ToString(rt_year) });
            }
            ViewBag.Retention_Year = new SelectList(year, "Id", "Text");
            assetdetails.CreatedBy = userId;
            var result = _objHomeAssetRegistration.Insert_AssetDetails(assetdetails);
            ViewBag.Asset_details_List = _objHomeAssetRegistration.Get_Asset_Details_With_Common_Asset(userId, (long)current_ki).ToList();
            TempData["ResultMsg"] = result;

            _sessionService.Set<bool>("Modified", true);// Added by TTL ::  SR103777 > CR6964 

            if (assetdetails.ID > 0)
                //return Redirect("../../AssetRegistration/AssetRegister"); // Added by TTL ::  SR103777 > CR6964  
                return RedirectToAction("AssetRegister", "AssetRegistration", new { IsEdit = "Y" }); // Added by TTL ::  SR103777 > CR6964  
            else
                // return Redirect("../AssetRegistration/AssetRegister"); // Added by TTL ::  SR103777 > CR6964  
                return RedirectToAction("AssetRegister", "AssetRegistration", new { IsEdit = "Y" }); // Added by TTL ::  SR103777 > CR6964  
            //return RedirectToAction("AssetRegister");
        }

        public ActionResult LastYear_Ki_AssetDetails()
        {
            List<SYKI> iList = new List<SYKI>();
            // iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });
            var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
            var result = _objHomeAssetRegistration.Last_year_Asset_Detail_Only_Non_CommonAsset(userId, (long)current_ki);
            TempData["ResultMsg"] = result;
            //return RedirectToAction("AssetRegister"); // Added by TTL ::  SR103777 > CR6964  
            return RedirectToAction("AssetRegister", "AssetRegistration", new { IsEdit = "Y" }); // Added by TTL ::  SR103777 > CR6964  
        }

        [HttpGet]
        public ActionResult DeleteAssetRegister(int Id)
        {
            CommonAAssetsRegListVM asset = new CommonAAssetsRegListVM();
            asset.ID = Id;
            asset.UpdatedBy = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var result = _objHomeAssetRegistration.Delete_AssetDetails(asset);
            TempData["ResultMsg"] = result;
            _sessionService.Set<bool>("Modified",true);  // Added by TTL ::  SR103777 > CR6964  
            //return RedirectToAction("AssetRegister"); // Added by TTL ::  SR103777 > CR6964  
            return RedirectToAction("AssetRegister", "AssetRegistration", new { IsEdit = "Y" }); // Added by TTL ::  SR103777 > CR6964  

        }

        // public ActionResult FinalSubmit_AssetDetails() // Added By Aumento :: SR102194
        public ActionResult FinalSubmit_AssetDetails(string IsEdit) // Added By Aumento :: SR102194
        {
            CommonAAssetsRegListVM asset = new CommonAAssetsRegListVM();
            List<CommonAAssetsRegListVM> get_common_asset_details = new List<CommonAAssetsRegListVM>();
            List<SYKI> iList = new List<SYKI>();
            // iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });
            var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();

            //Added by aumento as on 17062024 for SR71836------------------------------------------------------------------------
            var result1 = _objHomeAssetRegistration.Get_Division_For_ISSC_Member(Convert.ToInt32(_sessionService.Get<string>("userID")));

            // START :: Added By Aumento :: SR102194
            var AutoApproveToDiv = (result1.Div_Head_EmpCode == result1.Op_Head_EmpCode ? true : false);

            long? itgrccode = null;
            string itgrcid = _objHomeAssetRegistration._GetITGRC();
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

            if (AutoApproveToDiv)
            {
                _objHomeAssetRegistration.UpdateAssetRegisterAutoApprove((long)current_ki, Convert.ToInt32(_sessionService.Get<string>("userID")), AutoApproveToDiv, itgrccode);
            }

            bool match = _objHomeAssetRegistration.IsOperationMatch(userId);
            if (match)
            {
                if (itgrcid != null)
                {
                    itgrccode = Convert.ToInt64(itgrcid);
                }
            }
            // END :: Added By Aumento :: SR102194   
           
            if (result1.OperatingHead__Submit_Date != null || result1.OperatingHead_Submit_Status != null)
            {
                ViewBag.IsEditbtnVisible = "True";
                if (ViewBag.IsEditbtnVisible == "True")
                {
                    // _objHomeAssetRegistration.UpdateAssetRegister((long)current_ki, Convert.ToInt32(HttpContext.Session["UserId"])); // Added By Aumento :: SR102194
                    _objHomeAssetRegistration.UpdateAssetRegister((long)current_ki, Convert.ToInt32(_sessionService.Get<string>("userID")), AutoApproveToDiv, itgrccode); // Added By Aumento :: SR102194
                }
            }
            else if (result1.DivisionHead_Submit_Date != null && result1.DivisionHead_Submit_Status != null && (result1.OperatingHead_Name == null || result1.OperatingHead_Name == ""))
            {
                ViewBag.IsEditbtnVisible = "True";
                if (ViewBag.IsEditbtnVisible == "True")
                {
                    // _objHomeAssetRegistration.UpdateAssetRegister((long)current_ki, Convert.ToInt32(HttpContext.Session["UserId"])); // Added By Aumento :: SR102194
                    _objHomeAssetRegistration.UpdateAssetRegister((long)current_ki, Convert.ToInt32(_sessionService.Get<string>("userID")), AutoApproveToDiv, itgrccode); // Added By Aumento :: SR102194
                }
            }
            else
            {
                ViewBag.IsEditbtnVisible = "False";
            }

          
            //--------------------------------------------------------------------------------------------------------------------
            

            get_common_asset_details = _objHomeAssetRegistration.GetCommon_Asset_SubmittedDetails((long)current_ki, Convert.ToInt32(_sessionService.Get<string>("userID"))).ToList();

            int _count = 0;
            foreach (var item in get_common_asset_details)
            {
                if ((item.Secondary == null) && (item.ISSC_Member_Reason == null))
                {
                    _count = 1;
                    break;
                }
            }
            if (_count > 0)
            {
                TempData["msg"] = "Please update all top secret asset applicability before final submit.";
                //return Redirect("../AssetRegistration/AssetRegister"); // Added By Aumento :: SR102194
                return RedirectToAction("AssetRegister", "AssetRegistration", new { IsEdit = "Y" }); // Added By Aumento :: SR102194
            }
            var divison_head = _objHomeAssetRegistration.Get_DivisionHead_((long)current_ki, Convert.ToInt32(_sessionService.Get<string>("userID")));

            asset.SYKIID = current_ki;
            asset.CreatedBy = Convert.ToInt32(_sessionService.Get<string>("userID"));
            //Added by aumento as on 17062024 for SR71836---------------------------------------------  
            if (divison_head != null)
            {
                asset.Division_Head_EmpCode = divison_head.DivisionEmpCode;
            }
            //-----------------------------------------------------------------------------------------
            var result = _objHomeAssetRegistration.FinalSubmit_AssetDetails(asset);
            TempData["ResultMsg"] = result;
            // Send EMail to Division Head 
            //Added by aumento as on 17062024 for SR71836---------------------------------------------  
            //SendMailTo_DivisionHead_After_Issc_Member(divison_head.Empname, divison_head.Emailid, kiData._SYKIList.Select(x => x.KICODE).FirstOrDefault(), divison_head.ISSC_Member_Name, divison_head.DivisionName); 
            if (divison_head != null)
            {
                SendMailTo_DivisionHead_After_Issc_Member(divison_head.Empname, divison_head.Emailid, kiData._SYKIList.Select(x => x.KICODE).FirstOrDefault(), divison_head.ISSC_Member_Name, divison_head.DivisionName);
            }
            //-----------------------------------------------------------------------------------------
            _sessionService.Set<bool>("Modified",false); // Added by TTL ::  SR103777 > CR6964 
            return RedirectToAction("AssetDashboard");
        }

        private void SendMailTo_DivisionHead_After_Issc_Member(string Emp, string Emp_Emailid, string ski, string ISSC_MemberName, string DivisionName)
        {
            EmailCore sendMail = new EmailCore();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = Emp_Emailid; // user mail id // Emp_Emailid;
            sendMail.MailCc = "anupma.sharma@honda.hmsi.in";
            string strSubject = "Information Asset Register Approval Request";
            string strBody = "<table cellpadding=0 cellspacing=0 style='border-style:solid; border-color:skyblue; border-width:1px; width:800px;'>" +
                  "<tr style='background-color:skyblue;'>Information Asset Register Approval Request " + ski + "</tr>" +
                                 "<tr> <td> <table cellpadding=0 cellspacing=0 style='margin:10px;'>" +
                                 "<tr><td><b>Dear " + Emp + " San,</b></td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top>This is to inform you that ISO27001 - Information Asset Register has been submitted for " + ski + ".</td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top>Division Name - " + DivisionName + "</td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top>ISSC Member Name -  " + ISSC_MemberName + "</td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top colspan=2>Please Login into <a href='https://portal.honda2wheelersindia.com/SSO'>E-Portal</a> for approval process.</td></tr>" +
                                 "<tr><td><br><br></td></tr>" +
                                 "<tr><td>Best Regards</td></tr>" +
                                 "<tr><td>Team ISMS</td></tr>" +
                                 "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr>" +
                                 "</table> </td> </tr> </table>";
            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            //bool status = sendMail.Send();
        }

        [HttpGet]
        public ActionResult LastYearKiAssetDetails1(int SkyiId, int? ISSC_Empcode)
        {
            TempData["PageHead"] = "Last Year Information Asset Registers";
            long? userid = ISSC_Empcode == null ? Convert.ToInt32(_sessionService.Get<string>("userID")) : ISSC_Empcode;
            ViewBag.Last_Ki_Asset_details_List = _objHomeAssetRegistration.Get_Last_Ki_Asset_Details(userid, SkyiId);
            return View();
        }

        [HttpGet]
        public ActionResult LastYearKiAssetDetails(int SkyiId, int? DivId)
        {
            try
            {
            TempData["PageHead"] = "Last Year Information Asset Registers";
            long? userid = Convert.ToInt32(_sessionService.Get<string>("userID"));
            ViewBag.Last_Ki_Asset_details_List = _objHomeAssetRegistration.Get_Last_Ki_Asset_DetailsNew(userid, SkyiId, DivId);
            return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
           
        }

        [HttpGet]
        public ActionResult Get_Last_Ki_Asset_Details_OH(int SkyiId, int? OPID)
        {
            TempData["PageHead"] = "Last Year Information Asset Registers";
            long? userid = Convert.ToInt32(_sessionService.Get<string>("userID"));
            ViewBag.Last_Ki_Asset_details_List = _objHomeAssetRegistration.Get_Last_Ki_Asset_DetailsOperationID(userid, SkyiId, OPID);
            return View();
        }
        //Added by Aumento for SR91196
        [HttpGet]
        public ActionResult AssetDashboardForITGRCHead()
        {
            
            TempData["PageHead"] = "Information Asset Register Approval";
            int userId =  Convert.ToInt32(_sessionService.Get<string>("userID"));
            string result = _objHomeAssetRegistration._GetITGRC();
            if (userId.ToString() != result)
            {
                return RedirectToAction("Home", "Home");
            }
            ViewBag.AssetsDetailList = _objHomeAssetRegistration.Get_ISSC_Members_Detail_For_ITGRCHead(userId);
            ViewBag.ITGRCList = _objHomeAssetRegistration.Get_ISSC_Members_Division_Details_For_ITGRCHead(userId);
            var res_ITGRC = _objHomeAssetRegistration.Get_ISSC_Members_Detail_For_ITGRCHead(userId); // Added By Aumento :: SR102194
            ViewBag.IS_ITOPRATION = res_ITGRC.FirstOrDefault()?.IS_ITOPRATION; // Added By Aumento :: SR102194

            return View();
        }
        [HttpGet]
        public ActionResult AssetDetailsApproveByITGRCHead(int ISSC_Code)
        {
            TempData["PageHead"] = "Information Asset Register Approval";
            List<SYKI> iList = new List<SYKI>();
            var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();

            ViewBag.EnddateCheck = _objHomeAssetRegistration.ISSC_Member_AssetDetails_EndDate_Check(ISSC_Code, (long)current_ki);
            ViewBag.Final_Submit_Check = _objHomeAssetRegistration.ITGRC_Head_AssetDetails_FinalSubmit_Check(userId, (long)current_ki, ISSC_Code);

            var result = _objHomeAssetRegistration.Get_ISSC_Member_Asset_For_DivisionHead(ISSC_Code, (long)current_ki);
            ViewBag.Assetdetails = result;

            ViewBag.TopSecret = result.Where(x => x.ClassificationID == 1 && x.Reason_NA == null).Count();
            ViewBag.Secret = result.Where(x => x.ClassificationID == 2 && x.Reason_NA == null).Count();
            ViewBag.Internal = result.Where(x => x.ClassificationID == 3 && x.Reason_NA == null).Count();
            CommonAAssetsRegListVM asset_deatils = new CommonAAssetsRegListVM();
            asset_deatils.ISSC_Member_EmpCode = ISSC_Code;
            asset_deatils.SYKIID = current_ki;
            return View(asset_deatils);
        }

        [HttpPost]
        public ActionResult AssetDetailsApproveByITGRCHead(CommonAAssetsRegListVM asset_deatils)
        {
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            asset_deatils.ITGRC_Head_EmpCode = userId;
            List<SYKI> iList = new List<SYKI>();
            CultureInfo culture = new CultureInfo("en-GB");
            var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }

            var ITGRC_head = _objHomeAssetRegistration.Get_ITGRCHead_((long)asset_deatils.SYKIID, asset_deatils.ISSC_Member_EmpCode);
            if (!string.IsNullOrEmpty(Request.Form["btnFinalSubmit"]))
            {
                var ResponseResult = _objHomeAssetRegistration.Asset_Approve_By_ITGRCHead(asset_deatils);
                TempData["ResultMsgITGCHead"] = ResponseResult;
                var result = _objHomeAssetRegistration.Send_Email_To_ISSC_OperatingHead(asset_deatils).Select(x => x).FirstOrDefault();
                if (ITGRC_head != null && result != null)
                {
                    SendMailTo_Operatinghead_After_ITGRCHead(result.EmpName, result.EmpEmail, kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault(), ITGRC_head.ISSC_Member_Name, ITGRC_head.DivisionName, ITGRC_head.Empname);
                }
                if (ITGRC_head != null)
                {
                    SendMailTo_ISSCMember_After_ITGRCHeadApprove(ITGRC_head.ISSC_MemberEmail, kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault(), ITGRC_head.ISSC_Member_Name, ITGRC_head.DivisionName, ITGRC_head.Empname);
                }
            }
            else if (!string.IsNullOrEmpty(Request.Form["btnsendback"]))
            {
                var ResponseResult = _objHomeAssetRegistration.Asset_SendBack_By_ITGRCHead(asset_deatils);
                TempData["ResultMsgITGCHead"] = ResponseResult;
                var s = _objHomeAssetRegistration.GetStartDate_EndDate_ForAsset_Register(asset_deatils);
                var result = _objHomeAssetRegistration.Send_Email_To_ISSC_OperatingHead_FroSendBack(asset_deatils);
                SendMailTo_ISSCMember_After_ITGRCHeadSendBack(ITGRC_head.ISSC_MemberEmail, kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault(), ITGRC_head.ISSC_Member_Name, ITGRC_head.Empname, Convert.ToDateTime(s.StartDate, culture), Convert.ToDateTime(s.EndDate, culture));

            }
            return RedirectToAction("AssetDashboardForITGRCHead");
        }
        private void SendMailTo_ISSCMember_After_ITGRCHeadSendBack(string Emp_Emailid, string ski, string ISSC_MemberName, string ApproverName, DateTime StateDate, DateTime EndDate)
        {
            try
            {
                EmailCore sendMail = new EmailCore();
                sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                sendMail.MailTo = Emp_Emailid; // user mail id // Emp_Emailid;
                sendMail.MailCc = "anupma.sharma@honda.hmsi.in";
                string strSubject = "Information Asset Register Send Back";
                string strBody = "<table cellpadding=0 cellspacing=0 style='border-style:solid; border-color:skyblue; border-width:1px; width:800px;'>" +
                      "<tr style='background-color:skyblue;'>Information Asset Register send back " + ski + "</tr>" +
                                     "<tr> <td> <table cellpadding=0 cellspacing=0 style='margin:10px;'>" +
                                     "<tr><td><b>Dear " + ISSC_MemberName + " San,</b></td></tr>" +
                                     "<tr><td><br></td></tr>" +
                                     "<tr><td valign=top>This is to inform you that ISO27001 - Information Asset Register has been send back by  " + ApproverName + " for " + ski + ".</td></tr>" +
                                     "<tr><td><br></td></tr>" +
                                     "<tr><td valign=top>Kindly update Information Asset Register as window will be open from " + StateDate.ToString("dd/MM/yyyy") + " to " + EndDate.ToString("dd/MM/yyyy") + ".</td></tr>" +
                                    "<tr><td><br></td></tr>" +
                                     "<tr><td valign=top colspan=2>Please Login into <a href='https://portal.honda2wheelersindia.com/SSO'>E-Portal</a> for approval process.</td></tr>" +
                                     "<tr><td><br><br></td></tr>" +
                                     "<tr><td>Best Regards</td></tr>" +
                                     "<tr><td>Team ISMS</td></tr>" +
                                     "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr>" +
                                     "</table> </td> </tr> </table>";
                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;
                bool status = sendMail.Send();
            }
            catch (Exception ex)
            {
                // Log the error details for debugging (you can replace Console.WriteLine with proper logging)
                Console.WriteLine("Error sending email: " + ex.Message);
            }            
        }
        private void SendMailTo_Operatinghead_After_ITGRCHead(string Emp, string Emp_Emailid, string ski, string ISSC_MemberName, string DivisionName, string ITGRCName)
        {
            try
            {
                EmailCore sendMail = new EmailCore();
                sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                sendMail.MailTo = Emp_Emailid; // user mail id // Emp_Emailid;
                sendMail.MailCc = "anupma.sharma@honda.hmsi.in";
                string strSubject = "Information Asset Register Approval Request";
                string strBody = "<table cellpadding=0 cellspacing=0 style='border-style:solid; border-color:skyblue; border-width:1px; width:800px;'>" +
                                     "<tr style='background-color:skyblue;'>Information Asset Register Approval Request " + ski + "</tr>" +
                                     "<tr> <td> <table cellpadding=0 cellspacing=0 style='margin:10px;'>" +
                                     "<tr><td><b>Dear " + Emp + " San,</b></td></tr>" +
                                     "<tr><td><br></td></tr>" +
                                     "<tr><td valign=top>This is to inform you that ISO27001 - Information Asset Register has been submitted for " + ski + ".</td></tr>" +
                                     "<tr><td><br></td></tr>" +
                                     "<tr><td valign=top>ITGRC Name - " + ITGRCName + "</td></tr>" +
                                     "<tr><td><br></td></tr>" +
                                     "<tr><td valign=top>Division Name - " + DivisionName + "</td></tr>" +
                                     "<tr><td><br></td></tr>" +
                                     "<tr><td valign=top>ISSC Member Name -  " + ISSC_MemberName + "</td></tr>" +
                                     "<tr><td><br></td></tr>" +
                                     "<tr><td valign=top colspan=2>Please Login into <a href='https://portal.honda2wheelersindia.com/SSO'>E-Portal</a> for approval process.</td></tr>" +
                                     "<tr><td><br><br></td></tr>" +
                                     "<tr><td>Best Regards</td></tr>" +
                                     "<tr><td>Team ISMS</td></tr>" +
                                     "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr>" +
                                     "</table> </td> </tr> </table>";
                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;
                bool status = sendMail.Send();
            }
            catch (Exception ex)
            {
                // Log the error details for debugging (you can replace Console.WriteLine with proper logging)
                Console.WriteLine("Error sending email: " + ex.Message);
            }            
        }
        private void SendMailTo_ISSCMember_After_ITGRCHeadApprove(string Emp_Emailid, string ski, string ISSC_MemberName, string DivisionName, string ApproverName)
        {
            try
            {
                EmailCore sendMail = new EmailCore();
                sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                sendMail.MailTo = Emp_Emailid; // user mail id // Emp_Emailid;
                sendMail.MailCc = "anupma.sharma@honda.hmsi.in";
                string strSubject = "Information Asset Register Approved";
                string strBody = "<table cellpadding=0 cellspacing=0 style='border-style:solid; border-color:skyblue; border-width:1px; width:800px;'>" +
                    "<tr style='background-color:skyblue;'>Information Asset Register Approved " + ski + "</tr>" +
                                     "<tr> <td> <table cellpadding=0 cellspacing=0 style='margin:10px;'>" +
                                     "<tr><td><b>Dear " + ISSC_MemberName + " San,</b></td></tr>" +
                                     "<tr><td><br></td></tr>" +
                                     "<tr><td valign=top>This is to inform you that ISO27001 - Information Asset Register has been approved by  " + ApproverName + " for " + ski + ".</td></tr>" +
                                     "<tr><td valign=top colspan=2>Please Login into <a href='https://portal.honda2wheelersindia.com/SSO'>E-Portal</a> for approval process.</td></tr>" +
                                     "<tr><td><br><br></td></tr>" +
                                     "<tr><td>Best Regards</td></tr>" +
                                     "<tr><td>Team ISMS</td></tr>" +
                                     "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr>" +
                                     "</table> </td> </tr> </table>";
                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;
                bool status = sendMail.Send();
            }
            catch (Exception ex)
            {
                // Log the error details for debugging (you can replace Console.WriteLine with proper logging)
                Console.WriteLine("Error sending email: " + ex.Message);
            }
            
        }

        //Added by Aumento for SR91196

        [HttpGet]
        public ActionResult AssetDashboardForDivisionHead()
        {
            TempData["PageHead"] = "Information Asset Register Approval";
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
            List<Get_Division_ISSC_Member> lstDivision_ISSC_Member = new List<Get_Division_ISSC_Member>();
            List<Asset_Dvision_Deatils_VM> lstDivision_Details = new List<Asset_Dvision_Deatils_VM>();
            var oldUserId = _objHomeAssetRegistration.GetAssetUserDeailsForDivisionHeadByLoginUserID(userId, (long)current_ki);

            if (oldUserId != null)
            {
                lstDivision_ISSC_Member = _objHomeAssetRegistration.Get_ISSC_Members_Detail_For_DivisionHead(oldUserId);
                var lstDivision_ISSC_Member_1 = _objHomeAssetRegistration.Get_ISSC_Members_Detail_For_DivisionHead(userId);
                if(lstDivision_ISSC_Member_1.Any())
                {
                    if (lstDivision_ISSC_Member.Any())
                        lstDivision_ISSC_Member.AddRange(lstDivision_ISSC_Member_1);
                    else
                    {
                        lstDivision_ISSC_Member = lstDivision_ISSC_Member_1;
                    }
                }
                lstDivision_Details = _objHomeAssetRegistration.Get_ISSC_Members_Division_Details_For_DivisionHead(oldUserId);
                var lstDivision_Details_1 = _objHomeAssetRegistration.Get_ISSC_Members_Division_Details_For_DivisionHead(userId);
                if (lstDivision_Details_1.Any())
                {
                    if (lstDivision_Details.Any())
                        lstDivision_Details.AddRange(lstDivision_Details_1);
                    else
                    {
                        lstDivision_Details = lstDivision_Details_1;
                    }
                }

                ViewBag.AssetsDetailList = lstDivision_ISSC_Member;
                ViewBag.DivisionList = lstDivision_Details;
                ViewBag.IS_ITOPRATION = lstDivision_ISSC_Member.FirstOrDefault()?.IS_ITOPRATION;// Added By Aumento :: SR102194
            }
            else
            {
                // START :: Added By Aumento :: SR102194
                // ViewBag.AssetsDetailList = _objHomeAssetRegistration.Get_ISSC_Members_Detail_For_DivisionHead(userId); 
                // ViewBag.DivisionList = _objHomeAssetRegistration.Get_ISSC_Members_Division_Details_For_DivisionHead(userId);
                var isscMemberDetails = _objHomeAssetRegistration.Get_ISSC_Members_Detail_For_DivisionHead(userId);
                ViewBag.AssetsDetailList = isscMemberDetails;
                ViewBag.DivisionList = _objHomeAssetRegistration.Get_ISSC_Members_Division_Details_For_DivisionHead(userId);
                ViewBag.IS_ITOPRATION = isscMemberDetails.FirstOrDefault()?.IS_ITOPRATION; //new change of SR91196_a
            }// END :: Added By Aumento :: SR102194



            //var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIListForDashboard();
            //foreach (var item in kiData._SYKIList)
            //{
            //    iList1.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            //}
            //ViewBag.Lastki_details = iList1;
            return View();
        }

        [HttpGet]
        public ActionResult AssetDashboardForDivisionHead1()
        {
            TempData["PageHead"] = "Information Asset Register Approval";
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var result = _objHomeAssetRegistration.Get_ISSC_Member_Details_For_DivisionHead(userId);
            ViewBag.Division = result.DivisionName;
            ViewBag.ISSC = result.ISSCMember;

            ViewBag.ISSCStatus = result.ISSC_Member_Submit_Status;
            ViewBag.ISSCFinalDate = result.ISSC_Member_Submit_Date;

            ViewBag.Divisionstatus = result.DivisionHead_Submit_Status;
            ViewBag.DivisionFinalDate = result.DivisionHead_Submit_Date;

            ViewBag.OperatingStatus = result.OperatingHead_Submit_Status;
            ViewBag.OperatingFinalDate = result.OperatingHead__Submit_Date;

            ViewBag.ISSCMember_EmpCode = result.ISSCMember_EmpCode;
            DateTime? dt = result.EndDate;
            ViewBag.endate = String.Format("{0:dd/MM/yyyy}", dt);

            DateTime end_dates = Convert.ToDateTime(result.EndDate);
            int nodaysleft = end_dates.Subtract(DateTime.Now.Date).Days;
            if (result.ISSCMember != null && result.OperatingHead_Submit_Status == null)
                ViewBag.nodaysleft = nodaysleft + " Days";
            else
                ViewBag.nodaysleft = "";

            List<SYKI> iList1 = new List<SYKI>();
            var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIListForDashboard();
            foreach (var item in kiData._SYKIList)
            {
                iList1.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            ViewBag.Lastki_details = iList1;
            return View();
        }

        [HttpGet]
        public ActionResult AssetDetailsApproveByDivisionHead(int ISSC_Code)
        {
            TempData["PageHead"] = "Information Asset Register Approval";
            List<SYKI> iList = new List<SYKI>();
            var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();

            ViewBag.EnddateCheck = _objHomeAssetRegistration.ISSC_Member_AssetDetails_EndDate_Check(ISSC_Code, (long)current_ki);
            ViewBag.Final_Submit_Check = _objHomeAssetRegistration.Division_Head_AssetDetails_FinalSubmit_Check(userId, (long)current_ki, ISSC_Code);

            var result = _objHomeAssetRegistration.Get_ISSC_Member_Asset_For_DivisionHead(ISSC_Code, (long)current_ki);
            ViewBag.Assetdetails = result;

            ViewBag.TopSecret = result.Where(x => x.ClassificationID == 1 && x.Reason_NA == null).Count();
            ViewBag.Secret = result.Where(x => x.ClassificationID == 2 && x.Reason_NA == null).Count();
            ViewBag.Internal = result.Where(x => x.ClassificationID == 3 && x.Reason_NA == null).Count();
            CommonAAssetsRegListVM asset_deatils = new CommonAAssetsRegListVM();
            asset_deatils.ISSC_Member_EmpCode = ISSC_Code;
            asset_deatils.SYKIID = current_ki;
            return View(asset_deatils);
        }

        [HttpPost]
        public ActionResult AssetDetailsApproveByDivisionHead(CommonAAssetsRegListVM asset_deatils)
        {
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            asset_deatils.Division_Head_EmpCode = userId;
            List<SYKI> iList = new List<SYKI>();
            CultureInfo culture = new CultureInfo("en-GB");
            var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }

            var divison_head = _objHomeAssetRegistration.Get_DivisionHead_((long)asset_deatils.SYKIID, asset_deatils.ISSC_Member_EmpCode);
            if (!string.IsNullOrEmpty(Request.Form["btnFinalSubmit"]))
            {
                var ResponseResult = _objHomeAssetRegistration.Asset_Approve_By_DivisionHead(asset_deatils);
                TempData["ResultMsg"] = ResponseResult;
                // Send EMail tp  Operating Head  
                var result = _objHomeAssetRegistration.Send_Email_To_ISSC_OperatingHead(asset_deatils).Select(x => x).FirstOrDefault();
                //Added by aumento as on 17062024 for SR71836--------------------------------------------------------------------------
                //SendMailTo_Operatinghead_After_DivisionHead(result.EmpName, result.EmpEmail, kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault(), divison_head.ISSC_Member_Name, divison_head.DivisionName);
                if (divison_head != null && result!=null)
                {
                    SendMailTo_Operatinghead_After_DivisionHead(result.EmpName, result.EmpEmail, kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault(), divison_head.ISSC_Member_Name, divison_head.DivisionName);
                }


                // Send EMail to  ISSC Member 
                // SendMailTo_ISSCMember_After_DivisionHeadApprove(divison_head.ISSC_MemberEmail, kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault(), divison_head.ISSC_Member_Name, divison_head.DivisionName, divison_head.Empname);
                if (divison_head != null)
                {
                    SendMailTo_ISSCMember_After_DivisionHeadApprove(divison_head.ISSC_MemberEmail, kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault(), divison_head.ISSC_Member_Name, divison_head.DivisionName, divison_head.Empname);
                }
                //----------------------------------------------------------------------------------------------------------------------
            }
            else if (!string.IsNullOrEmpty(Request.Form["btnsendback"]))
            {
                var ResponseResult = _objHomeAssetRegistration.Asset_SendBack_By_DivisionHead(asset_deatils);
                TempData["ResultMsg"] = ResponseResult;
                var s = _objHomeAssetRegistration.GetStartDate_EndDate_ForAsset_Register(asset_deatils);
                // Send EMail to ISSC Member
                var result = _objHomeAssetRegistration.Send_Email_To_ISSC_OperatingHead_FroSendBack(asset_deatils);
                SendMailTo_ISSCMember_After_DivisionHeadSendBack(divison_head.ISSC_MemberEmail, kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault(), divison_head.ISSC_Member_Name, divison_head.DivisionName, divison_head.Empname, Convert.ToDateTime(s.StartDate, culture), Convert.ToDateTime(s.EndDate, culture));

            }
            return RedirectToAction("AssetDashboardForDivisionHead");
        }

        private void SendMailTo_Operatinghead_After_DivisionHead(string Emp, string Emp_Emailid, string ski, string ISSC_MemberName, string DivisionName)
        {
            EmailCore sendMail = new EmailCore();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = Emp_Emailid; // user mail id // Emp_Emailid;
            sendMail.MailCc = "anupma.sharma@honda.hmsi.in";
            string strSubject = "Information Asset Register Approval Request";
            string strBody = "<table cellpadding=0 cellspacing=0 style='border-style:solid; border-color:skyblue; border-width:1px; width:800px;'>" +
                                 "<tr style='background-color:skyblue;'>Information Asset Register Approval Request " + ski + "</tr>" +
                                 "<tr> <td> <table cellpadding=0 cellspacing=0 style='margin:10px;'>" +
                                 "<tr><td><b>Dear " + Emp + " San,</b></td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top>This is to inform you that ISO27001 - Information Asset Register has been submitted for " + ski + ".</td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top>Division Name - " + DivisionName + "</td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top>ISSC Member Name -  " + ISSC_MemberName + "</td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top colspan=2>Please Login into <a href='https://portal.honda2wheelersindia.com/SSO'>E-Portal</a> for approval process.</td></tr>" +
                                 "<tr><td><br><br></td></tr>" +
                                 "<tr><td>Best Regards</td></tr>" +
                                 "<tr><td>Team ISMS</td></tr>" +
                                 "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr>" +
                                 "</table> </td> </tr> </table>";
            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }

        private void SendMailTo_ISSCMember_After_DivisionHeadApprove(string Emp_Emailid, string ski, string ISSC_MemberName, string DivisionName, string ApproverName)
        {
            EmailCore sendMail = new EmailCore();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = Emp_Emailid; // user mail id // Emp_Emailid;
            sendMail.MailCc = "anupma.sharma@honda.hmsi.in";
            string strSubject = "Information Asset Register Approved";
            string strBody = "<table cellpadding=0 cellspacing=0 style='border-style:solid; border-color:skyblue; border-width:1px; width:800px;'>" +
                "<tr style='background-color:skyblue;'>Information Asset Register Approved " + ski + "</tr>" +
                                 "<tr> <td> <table cellpadding=0 cellspacing=0 style='margin:10px;'>" +
                                 "<tr><td><b>Dear " + ISSC_MemberName + " San,</b></td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top>This is to inform you that ISO27001 - Information Asset Register has been approved by  " + ApproverName + " for " + ski + ".</td></tr>" +
                                 "<tr><td valign=top colspan=2>Please Login into <a href='https://portal.honda2wheelersindia.com/SSO'>E-Portal</a> for approval process.</td></tr>" +
                                 "<tr><td><br><br></td></tr>" +
                                 "<tr><td>Best Regards</td></tr>" +
                                 "<tr><td>Team ISMS</td></tr>" +
                                 "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr>" +
                                 "</table> </td> </tr> </table>";
            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }

        private void SendMailTo_ISSCMember_After_DivisionHeadSendBack(string Emp_Emailid, string ski, string ISSC_MemberName, string DivisionName, string ApproverName, DateTime StateDate, DateTime EndDate)
        {
            EmailCore sendMail = new EmailCore();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = Emp_Emailid; // user mail id // Emp_Emailid;
            sendMail.MailCc = "anupma.sharma@honda.hmsi.in";
            string strSubject = "Information Asset Register Send Back";
            string strBody = "<table cellpadding=0 cellspacing=0 style='border-style:solid; border-color:skyblue; border-width:1px; width:800px;'>" +
                  "<tr style='background-color:skyblue;'>Information Asset Register send back " + ski + "</tr>" +
                                 "<tr> <td> <table cellpadding=0 cellspacing=0 style='margin:10px;'>" +
                                 "<tr><td><b>Dear " + ISSC_MemberName + " San,</b></td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top>This is to inform you that ISO27001 - Information Asset Register has been send back by  " + ApproverName + " for " + ski + ".</td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top>Kindly update Information Asset Register as window will be open from " + StateDate.ToString("dd/MM/yyyy") + " to " + EndDate.ToString("dd/MM/yyyy") + ".</td></tr>" +
                                "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top colspan=2>Please Login into <a href='https://portal.honda2wheelersindia.com/SSO'>E-Portal</a> for approval process.</td></tr>" +
                                 "<tr><td><br><br></td></tr>" +
                                 "<tr><td>Best Regards</td></tr>" +
                                 "<tr><td>Team ISMS</td></tr>" +
                                 "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr>" +
                                 "</table> </td> </tr> </table>";
            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }

        #region Export To Excel Not Use Code
        ////[HttpGet]
        ////public ActionResult ViewItGRCExportExcel()
        ////{
        ////    var result = _objHomeAssetRegistration.GetViewToITGRCTeamList(null, null, null).Select((x, index) => new
        ////    {
        ////        S_No = index + 1,
        ////        Ki = x.SYKI,
        ////        Operation_Name = x.OPERATIONNAME,
        ////        Division_Name = x.DIVISIONNAMEN,
        ////        ISSC_Member = x.EmpName + " " + x.ADEMPCODE
        ////    }
        ////    ).ToList();
        ////    byte[] filecontent = ExportExcel(result, true, 5);
        ////    return File(filecontent, ExcelContentType, "Nominated_ISSC_Member_" + DateTime.Now + ".xls");
        ////}

        ////[HttpPost]
        ////public ActionResult ViewItGRCExportExcelPost(long? SYKIDatanew, long? OPERATIONID, long? DIVISIONID)
        ////{
        ////    var result = _objHomeAssetRegistration.GetViewToITGRCTeamList(null, null, null).Select((x, index) => new
        ////    {
        ////        S_No = index + 1,
        ////        Ki = x.SYKI,
        ////        Operation_Name = x.OPERATIONNAME,
        ////        Division_Name = x.DIVISIONNAMEN,
        ////        ISSC_Member = x.EmpName + " " + x.ADEMPCODE
        ////    }
        ////    ).ToList();
        ////    byte[] filecontent = ExportExcel(result, true, 5);
        ////    return File(filecontent, ExcelContentType, "Nominated_ISSC_Member_" + DateTime.Now + ".xls");
        ////}

        //public static byte[] ExportExcel<T>(List<T> Assets, bool showSlno = true, int colorUpTo = 3)
        //{
        //    //return ExportExcel(ListToDataTable<T>(data), showSlno);
        //    List<T> data = Assets;
        //    //List<BalanceSheetExportLibReportVM> data1 = Lib;
        //    var dataCount = data.Count > 0 ? data.Count : 1;
        //    byte[] result = null;
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //    using (ExcelPackage package = new ExcelPackage())
        //    {
        //        ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(string.Format("{0} Data", "Excel"));
        //        int startRowFrom = 1;
        //        // add the content into the Excel file
        //        workSheet.Cells["A" + startRowFrom].LoadFromCollection(data, true);
        //        // autofit width of cells with small content
        //        workSheet.Cells.AutoFitColumns();
        //        result = package.GetAsByteArray();
        //    }
        //    return result;
        //}

        //public static string ExcelContentType
        //{
        //    get
        //    { return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; }
        //}
        #endregion

        [HttpGet]
        public ActionResult AssetDashboardForOperatingHead()
        {
            TempData["PageHead"] = "Information Asset Register Approval";
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var kiData_ = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            var current_ki = kiData_._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
            var oldUserId = _objHomeAssetRegistration.GetAssetUserDeailsForOPHeadByLoginUserID(userId, (long)current_ki);
            List<Get_Division_ISSC_Member> lstDivision_ISSC_Member = new List<Get_Division_ISSC_Member>();
            if (oldUserId != null)
            {
                lstDivision_ISSC_Member = _objHomeAssetRegistration.Get_ISSC_Members_Asset_Details_For_OpearingHead(oldUserId);
                var lst_Division_ISSC_Member_1 = _objHomeAssetRegistration.Get_ISSC_Members_Asset_Details_For_OpearingHead(userId);
                if (lst_Division_ISSC_Member_1.Any())
                {
                    if (lstDivision_ISSC_Member.Any())
                        lstDivision_ISSC_Member.AddRange(lst_Division_ISSC_Member_1);
                    else
                        lstDivision_ISSC_Member = lst_Division_ISSC_Member_1;
                }
                ViewBag.IS_ITOPRATION = lstDivision_ISSC_Member.FirstOrDefault()?.IS_ITOPRATION; // Added By Aumento :: SR102194
                ViewBag.AutoApprove = lstDivision_ISSC_Member.FirstOrDefault()?.AutoApprove; // Added By Aumento :: SR102194
            }
            else
            {
                lstDivision_ISSC_Member = _objHomeAssetRegistration.Get_ISSC_Members_Asset_Details_For_OpearingHead(userId);
                ViewBag.IS_ITOPRATION = lstDivision_ISSC_Member.FirstOrDefault()?.IS_ITOPRATION;// Added By Aumento :: SR102194
                ViewBag.AutoApprove = lstDivision_ISSC_Member.FirstOrDefault()?.AutoApprove; // Added By Aumento :: SR102194
            }

            //ViewBag.Division = result.DivisionName;
            //ViewBag.ISSC = result.ISSCMember;

            //ViewBag.ISSCStatus = result.ISSC_Member_Submit_Status;
            //ViewBag.ISSCFinalDate = result.ISSC_Member_Submit_Date;

            //ViewBag.Divisionstatus = result.DivisionHead_Submit_Status;
            //ViewBag.DivisionFinalDate = result.DivisionHead_Submit_Date;

            //ViewBag.OperatingStatus = result.OperatingHead_Submit_Status;
            //ViewBag.OperatingFinalDate = result.OperatingHead__Submit_Date;

            //ViewBag.ISSCMember_EmpCode = result.ISSCMember_EmpCode;
            List<SYKI> iList1 = new List<SYKI>();
            //var kiData_ = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            foreach (var item in kiData_._SYKIList)
            {
                iList1.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            //var current_ki = kiData_._SYKIList.Select(x => x.SYKIID).FirstOrDefault();

            DateTime? dt = lstDivision_ISSC_Member.Select(x => x.EndDate).FirstOrDefault();
            ViewBag.endate = String.Format("{0:dd/MM/yyyy}", dt);
            ViewBag.ISSC_MemberData = lstDivision_ISSC_Member;

            DateTime end_dates = Convert.ToDateTime(lstDivision_ISSC_Member.Select(x => x.EndDate).FirstOrDefault());
            int nodaysleft = end_dates.Subtract(DateTime.Now.Date).Days;

            int Op_Head_Appoval_Count = lstDivision_ISSC_Member.Where(x => x.OperatingHead_Submit_Status == 1).Count();
            int total_list_count = lstDivision_ISSC_Member.Count();

            if (Op_Head_Appoval_Count != total_list_count)
                ViewBag.nodaysleft = nodaysleft + " Days";
            else
                ViewBag.nodaysleft = "";




            List<AssetRegistrationSYKIListForOH> last_year = new List<AssetRegistrationSYKIListForOH>();
            //var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIListFor_OperatingHead_Dashboard((long)current_ki, userId);
            if (oldUserId != null)
            {
                AssetRegistrationSYKIViewModelOH assetRegistrationSYKIViewModel = new AssetRegistrationSYKIViewModelOH();
                assetRegistrationSYKIViewModel = _objHomeAssetRegistration.GetAssetRegistrationSYKIListFor_OperatingHeadDeails_Dashboard((long)current_ki, (int)oldUserId);
                var assetRegistrationSYKIViewModel_1 = _objHomeAssetRegistration.GetAssetRegistrationSYKIListFor_OperatingHeadDeails_Dashboard((long)current_ki, userId);

                if(assetRegistrationSYKIViewModel_1._SYKIList.Any())
                {
                    if (assetRegistrationSYKIViewModel._SYKIList.Any())
                        assetRegistrationSYKIViewModel._SYKIList.AddRange(assetRegistrationSYKIViewModel_1._SYKIList);
                    else
                    {
                        assetRegistrationSYKIViewModel._SYKIList = assetRegistrationSYKIViewModel_1._SYKIList;
                    }
                }

                foreach (var item in assetRegistrationSYKIViewModel._SYKIList)
                {
                    last_year.Add(new AssetRegistrationSYKIListForOH { KICODE = item.KICODE, SYKIID = item.SYKIID, DIVISIONNAMEN = item.DIVISIONNAMEN, ADEMPCODE = item.ADEMPCODE, OperationName = item.OperationName, OPID = item.OPID });
                }
                ViewBag.Lastki_details = last_year;
            }
            else
            {
                var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIListFor_OperatingHeadDeails_Dashboard((long)current_ki, userId);

                foreach (var item in kiData._SYKIList)
                {
                    last_year.Add(new AssetRegistrationSYKIListForOH { KICODE = item.KICODE, SYKIID = item.SYKIID, DIVISIONNAMEN = item.DIVISIONNAMEN, ADEMPCODE = item.ADEMPCODE, OperationName = item.OperationName, OPID = item.OPID });
                }
                ViewBag.Lastki_details = last_year;
            }

            return View();
        }


        [HttpGet]
        public ActionResult AssetDashboardForOperatingHead1()
        {
            TempData["PageHead"] = "Information Asset Register Approval";
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var result = _objHomeAssetRegistration.Get_ISSC_Member_Details_For_OpearingHead(userId);

            //ViewBag.Division = result.DivisionName;
            //ViewBag.ISSC = result.ISSCMember;

            //ViewBag.ISSCStatus = result.ISSC_Member_Submit_Status;
            //ViewBag.ISSCFinalDate = result.ISSC_Member_Submit_Date;

            //ViewBag.Divisionstatus = result.DivisionHead_Submit_Status;
            //ViewBag.DivisionFinalDate = result.DivisionHead_Submit_Date;

            //ViewBag.OperatingStatus = result.OperatingHead_Submit_Status;
            //ViewBag.OperatingFinalDate = result.OperatingHead__Submit_Date;

            //ViewBag.ISSCMember_EmpCode = result.ISSCMember_EmpCode;
            List<SYKI> iList1 = new List<SYKI>();
            var kiData_ = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            foreach (var item in kiData_._SYKIList)
            {
                iList1.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            var current_ki = kiData_._SYKIList.Select(x => x.SYKIID).FirstOrDefault();

            DateTime? dt = result.Select(x => x.EndDate).FirstOrDefault();
            ViewBag.endate = String.Format("{0:dd/MM/yyyy}", dt);
            ViewBag.ISSC_MemberData = result;

            DateTime end_dates = Convert.ToDateTime(result.Select(x => x.EndDate).FirstOrDefault());
            int nodaysleft = end_dates.Subtract(DateTime.Now.Date).Days;

            int Op_Head_Appoval_Count = result.Where(x => x.OperatingHead_Submit_Status == 1).Count();
            int total_list_count = result.Count();

            if (Op_Head_Appoval_Count != total_list_count)
                ViewBag.nodaysleft = nodaysleft + " Days";
            else
                ViewBag.nodaysleft = "";




            List<AssetRegistrationSYKIList> last_year = new List<AssetRegistrationSYKIList>();
            var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIListFor_OperatingHead_Dashboard((long)current_ki, userId);
            foreach (var item in kiData._SYKIList)
            {
                last_year.Add(new AssetRegistrationSYKIList { KICODE = item.KICODE, SYKIID = item.SYKIID, DIVISIONNAMEN = item.DIVISIONNAMEN, ADEMPCODE = item.ADEMPCODE });
            }
            ViewBag.Lastki_details = last_year;
            return View();
        }

        [HttpGet]
        public ActionResult AssetDetailsApproveByOperatingHead(int ISSC_Code)
        {
            TempData["PageHead"] = "Information Asset Register Approval";
            List<SYKI> iList = new List<SYKI>();
            var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();

            ViewBag.EnddateCheck =  _objHomeAssetRegistration.ISSC_Member_AssetDetails_EndDate_Check(ISSC_Code, (long)current_ki);
            ViewBag.Final_Submit_Check =  _objHomeAssetRegistration.Operating_Head_AssetDetails_FinalSubmit_Check(ISSC_Code, (long)current_ki);

            var result = _objHomeAssetRegistration.Get_ISSC_Member_Asset_For_DivisionHead(ISSC_Code, (long)current_ki);
            ViewBag.Assetdetails = result;
            ViewBag.TopSecret = result.Where(x => x.ClassificationID == 1 && x.Reason_NA == null).Count();
            ViewBag.Secret = result.Where(x => x.ClassificationID == 2 && x.Reason_NA == null).Count();
            ViewBag.Internal = result.Where(x => x.ClassificationID == 3 && x.Reason_NA == null).Count();
            CommonAAssetsRegListVM asset_deatils = new CommonAAssetsRegListVM();
            asset_deatils.ISSC_Member_EmpCode = ISSC_Code;
            asset_deatils.SYKIID = current_ki;
            ViewBag.Remarks = _objHomeAssetRegistration.GetRemrksForOPHead((long)current_ki, ISSC_Code);

            var Result = _objHomeAssetRegistration.GetRemrksForOPHead((long)current_ki, ISSC_Code); // Added By Aumento :: SR102194
            ViewBag.IS_ITOPRATION = Result.FirstOrDefault()?.IS_ITOPRATION; // Added By Aumento :: SR102194
            return View(asset_deatils);
        
        }

        [HttpPost]
        public ActionResult AssetDetailsApproveByOperatingHead(CommonAAssetsRegListVM asset_deatils)
        {
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            asset_deatils.Operating_Head_EmpCode = userId;
            List<SYKI> iList = new List<SYKI>();
            CultureInfo culture = new CultureInfo("en-GB");
            var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            if (userId > 0)
            {
                var divison_head = _objHomeAssetRegistration.Get_OperatingHead_((long)asset_deatils.SYKIID, asset_deatils.ISSC_Member_EmpCode, userId);

                if (!string.IsNullOrEmpty(Request.Form["btnFinalSubmit"]))
                {
                    var ResponseResult = _objHomeAssetRegistration.Asset_Approve_By_OperatingHead(asset_deatils);
                    TempData["ResultMsg"] = ResponseResult;
                    // Send EMail to  ISSC Member and Operating Head  
                    // var result // Send EMail to  Operating Head  
                    var result = _objHomeAssetRegistration.Get_DivisionHead_((long)asset_deatils.SYKIID, asset_deatils.ISSC_Member_EmpCode);
                    //Added by aumento as on 17062024 for SR71836-------------------------------------------------------------------------- 
                    //SendMailTo_DivisionHead_After_Operatinghead(result.Empname, result.Emailid, kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault(), divison_head.ISSC_Member_Name, divison_head.DivisionName, divison_head.Empname);
                    if (divison_head != null && result!=null)
                    {
                        SendMailTo_DivisionHead_After_Operatinghead(result.Empname, result.Emailid, kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault(), divison_head.ISSC_Member_Name, divison_head.DivisionName, divison_head.Empname);
                    }                    

                    // Send EMail to  ISSC Member 
                    //SendMailTo_ISSCMember_AfterOperatingHeadApprove(divison_head.ISSC_MemberEmail, kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault(), divison_head.ISSC_Member_Name, divison_head.DivisionName, divison_head.Empname);
                    if (divison_head != null)
                    {
                        SendMailTo_ISSCMember_AfterOperatingHeadApprove(divison_head.ISSC_MemberEmail, kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault(), divison_head.ISSC_Member_Name, divison_head.DivisionName, divison_head.Empname);
                    }
                    //-----------------------------------------------------------------------------------------------------------------------

                }
                else if (!string.IsNullOrEmpty(Request.Form["btnsendback"]))
                {
                    var ResponseResult = _objHomeAssetRegistration.Asset_SendBack_By_OperatingHead(asset_deatils);
                    TempData["ResultMsg"] = ResponseResult;
                    // Send EMail to ISSC Member
                    // var result = _objHomeAssetRegistration.Send_Email_To_ISSC_(asset_deatils);
                    var s = _objHomeAssetRegistration.GetStartDate_EndDate_ForAsset_Register(asset_deatils);
                    // Send EMail to ISSC Member
                    var result = _objHomeAssetRegistration.Send_Email_To_ISSC_OperatingHead_FroSendBack(asset_deatils);
                    SendMailTo_ISSCMember_After_OperatingHeadSendBack(divison_head.ISSC_MemberEmail, kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault(), divison_head.ISSC_Member_Name, divison_head.DivisionName, divison_head.Empname, Convert.ToDateTime(s.StartDate, culture), Convert.ToDateTime(s.EndDate, culture));

                }
            }
            return RedirectToAction("AssetDashboardForOperatingHead");
        }

        private void SendMailTo_ISSCMember_AfterOperatingHeadApprove(string Emp_Emailid, string ski, string ISSC_MemberName, string DivisionName, string ApproverName)
        {
            EmailCore sendMail = new EmailCore();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = Emp_Emailid; // user mail id // Emp_Emailid;
            sendMail.MailCc = "anupma.sharma@honda.hmsi.in";
            string strSubject = "Information Asset Register Approved";
            string strBody = "<table cellpadding=0 cellspacing=0 style='border-style:solid; border-color:skyblue; border-width:1px; width:800px;'>" +
                                 "<tr style='background-color:skyblue;'>Information Asset Register Approved " + ski + "</tr>" +
                                 "<tr> <td> <table cellpadding=0 cellspacing=0 style='margin:10px;'>" +

                                 "<tr><td><b>Dear " + ISSC_MemberName + " San,</b></td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top>This is to inform you that ISO27001 - Information Asset Register has been approved by  " + ApproverName + " for " + ski + ".</td></tr>" +
                                 "<tr><td valign=top colspan=2>Please Login into <a href='https://portal.honda2wheelersindia.com/SSO'>E-Portal</a> for approval process.</td></tr>" +
                                 "<tr><td><br><br></td></tr>" +
                                 "<tr><td>Best Regards</td></tr>" +
                                 "<tr><td>Team ISMS</td></tr>" +
                                 "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr>" +
                                 "</table> </td> </tr> </table>";
            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }

        private void SendMailTo_DivisionHead_After_Operatinghead(string Emp, string Emp_Emailid, string ski, string ISSC_MemberName, string DivisionName, string ApproverName)
        {
            EmailCore sendMail = new EmailCore();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = Emp_Emailid; // user mail id // Emp_Emailid;
            sendMail.MailCc = "anupma.sharma@honda.hmsi.in";
            string strSubject = "Information Asset Register Approved";
            string strBody = "<table cellpadding=0 cellspacing=0 style='border-style:solid; border-color:skyblue; border-width:1px; width:800px;'>" +
                                 "<tr style='background-color:skyblue;'>Information Asset Register Approved " + ski + "</tr>" +
                                 "<tr> <td> <table cellpadding=0 cellspacing=0 style='margin:10px;'>" +

                                 "<tr><td><b>Dear " + Emp + " San,</b></td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top>This is to inform you that ISO27001 - Information Asset Register has been approved by  " + ApproverName + " for " + ski + ".</td></tr>" +
                                 "<tr><td valign=top colspan=2>Please Login into <a href='https://portal.honda2wheelersindia.com/SSO'>E-Portal</a> for approval process.</td></tr>" +
                                 "<tr><td><br><br></td></tr>" +
                                 "<tr><td>Best Regards</td></tr>" +
                                 "<tr><td>Team ISMS</td></tr>" +
                                 "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr>" +
                                 "</table> </td> </tr> </table>";
            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }

        private void SendMailTo_ISSCMember_After_OperatingHeadSendBack(string Emp_Emailid, string ski, string ISSC_MemberName, string DivisionName, string ApproverName, DateTime StateDate, DateTime EndDate)
        {
            EmailCore sendMail = new EmailCore();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = Emp_Emailid; // user mail id // Emp_Emailid;
            sendMail.MailCc = "anupma.sharma@honda.hmsi.in";
            string strSubject = "Information Asset Register Send Back";
            string strBody = "<table cellpadding=0 cellspacing=0 style='border-style:solid; border-color:skyblue; border-width:1px; width:800px;'>" +
                                "<tr style='background-color:skyblue;'>Information Asset Register send back " + ski + "</tr>" +
                                 "<tr> <td> <table cellpadding=0 cellspacing=0 style='margin:10px;'>" +

                                 "<tr><td><b>Dear " + ISSC_MemberName + " San,</b></td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top>This is to inform you that ISO27001 - Information Asset Register has been send back by  " + ApproverName + " for " + ski + ".</td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top>Kindly update Information Asset Register as window will be open from " + StateDate.ToString("dd/MM/yyyy") + " to " + EndDate.ToString("dd/MM/yyyy") + ".</td></tr>" +
                                "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top colspan=2>Please Login into <a href='https://portal.honda2wheelersindia.com/SSO'>E-Portal</a> for approval process.</td></tr>" +
                                 "<tr><td><br><br></td></tr>" +
                                 "<tr><td>Best Regards</td></tr>" +
                                 "<tr><td>Team ISMS</td></tr>" +
                                 "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr>" +
                                 "</table> </td> </tr> </table>";
            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }

        public ActionResult DeficiencyAssetRegister()
        {
            TempData["PageHead"] = "IT GRC Team Feedback";
            List<SYKI> iList = new List<SYKI>();
            var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            decimal current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
            List<ADORGLEVEL> opList = _objHomeAssetRegistration.BindOperation((long)current_ki);

            ViewBag.OpList = new SelectList(opList, "ADORGLEVELID", "LEVELDESCRIP");
            ViewBag.DivisionList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");

            ViewBag.PassetList = new SelectList(string.Empty, "Id", "Text");
            ViewBag.SassetList = new SelectList(string.Empty, "Id", "Text");

            return View();
        }

        public ActionResult BindDivisionByPrimaryAsset(long id)
        {
            var divList = _objHomeAssetRegistration.BindPrimaryAsset(id).Select(x => x.PrimaryAssetText).Distinct().ToList();
            return Json(divList);
        }
        public ActionResult BindDivisionBySecondaryAsset(string primaryid, long div_id)
        {
            List<PrimaryAsset> divList = _objHomeAssetRegistration.BindSecondaryyAsset(primaryid, div_id);
            return Json(divList);
        }

        [HttpPost]
        public ActionResult DeficiencyAssetRegister(DifiencyReport didiency)
        {
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            didiency.createdby = userId;
            _objHomeAssetRegistration.InsertAssetDifiency(didiency);

            List<SYKI> iList = new List<SYKI>();
            CultureInfo culture = new CultureInfo("en-GB");
            var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            long currentki = Convert.ToInt64(kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault());

            // Send EMail to  ISSC Member and Division Head  
            //  var result = _objHomeAssetRegistration.Send_Email_To_ISSC_Member_DivisionHead(didiency);
            // Send EMail to ISSC Member
            var result = _objHomeAssetRegistration.Send_Email_To_ISSC_Member_DivisionHead(didiency);
            if (result != null)
            {
                CommonAAssetsRegListVM asset_detail = new CommonAAssetsRegListVM();
                asset_detail.ISSC_Member_EmpCode = result.Select(x => x.EmpCode).FirstOrDefault();
                asset_detail.SYKIID = currentki;
                var s = _objHomeAssetRegistration.GetStartDate_EndDate_ForAsset_Register(asset_detail);
                var divison_head = _objHomeAssetRegistration.Get_OperatingHead_Old(currentki, result.Select(x => x.EmpCode).FirstOrDefault());
                SendMailTo_ISSCMember_After_DifiencyReportSendBack(divison_head.ISSC_MemberEmail, kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault(), divison_head.ISSC_Member_Name, divison_head.DivisionName, divison_head.ISSC_Member_Name, Convert.ToDateTime(s.StartDate, culture), Convert.ToDateTime(s.EndDate, culture));


                TempData["msg"] = "Deficiency feedback submitted sucessfully.";
            }
            return RedirectToAction("DeficiencyAssetRegister");
        }

        private void SendMailTo_ISSCMember_After_DifiencyReportSendBack(string Emp_Emailid, string ski, string ISSC_MemberName, string DivisionName, string ApproverName, DateTime StateDate, DateTime EndDate)
        {
            EmailCore sendMail = new EmailCore();
             sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            sendMail.MailTo = Emp_Emailid; // user mail id // Emp_Emailid;
            sendMail.MailCc = "anupma.sharma@honda.hmsi.in";
            string strSubject = "Information Asset Register Send Back";
            string strBody = "<table cellpadding=0 cellspacing=0 style='border-style:solid; border-color:skyblue; border-width:1px; width:800px;'>" +
                                "<tr style='background-color:skyblue;'>Information Asset Register send back " + ski + "</tr>" +
                                 "<tr> <td> <table cellpadding=0 cellspacing=0 style='margin:10px;'>" +
                                 "<tr><td><b>Dear " + ISSC_MemberName + " San,</b></td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top>This is to inform you that ISO27001 - Information Asset Register has been send back by ISMS team with deficiancy report.</td></tr>" +
                                 "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top>Kindly update Information Asset Register as window will be open from " + StateDate.ToString("dd/MM/yyyy") + " to " + EndDate.ToString("dd/MM/yyyy") + ".</td></tr>" +
                                "<tr><td><br></td></tr>" +
                                 "<tr><td valign=top colspan=2>Please Login into <a href='https://portal.honda2wheelersindia.com/SSO'>E-Portal</a> for approval process.</td></tr>" +
                                 "<tr><td><br><br></td></tr>" +
                                 "<tr><td>Best Regards</td></tr>" +
                                 "<tr><td>Team ISMS</td></tr>" +
                                 "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr>" +
                                 "</table> </td> </tr> </table>";
            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            bool status = sendMail.Send();
        }

        public ActionResult AdminDashboard()
        {
            TempData["PageHead"] = "View Dashboard";
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            EmpDetails emp = _objHomeAssetRegistration.GetEmp_Details_Ki_Wise(userId);
            List<DivisionWise_Asset_Count> discount = new List<DivisionWise_Asset_Count>();

            List<SYKI> iList1 = new List<SYKI>();
            var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIListForViewTeam();
            foreach (var item in kiData._SYKIList)
            {
                //if (item.ACTIVE == 1)
                iList1.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            ViewBag.SYKIDatanew = new SelectList(iList1, "SYKIID", "KICODE");
            ViewBag.CurrentSYKIData = kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault();
            decimal current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
            List<ADORGLEVEL> opList = _objHomeAssetRegistration.BindOperation((long)current_ki);
            ITDivisionApprovalgetSearchFilterData(new SearchParameterList());
            ViewBag.OpList = new SelectList(opList, "ADORGLEVELID", "LEVELDESCRIP");
            ViewBag.DivisionList = new SelectList(string.Empty, "DIVISIONID", "DIVISION");

            var result = _objHomeAssetRegistration.Get_Asset_Details_Operation_Division_Wise((long)current_ki, 0, 0);
            ViewBag.Assetdetails = result;
            ViewBag.TopSecret = result.Where(x => x.ClassificationID == 1 && x.Reason_NA == null).Count();
            ViewBag.Secret = result.Where(x => x.ClassificationID == 2 && x.Reason_NA == null).Count();
            ViewBag.Internal = result.Where(x => x.ClassificationID == 3 && x.Reason_NA == null).Count();


            var resultcount = _objHomeAssetRegistration.Get_Asset_Count_DivisionWise((long)current_ki, 0, 0);
            for (int i = 0; i < resultcount.Count; i++)
            {
                DivisionWise_Asset_Count d = new DivisionWise_Asset_Count();
                d.DivisionName = resultcount[i].DivisionName;
                d.TSCount = resultcount[i].TSCount;
                d.SCount = resultcount[i].SCount;
                d.ISCount = resultcount[i].ISCount;
                discount.Add(d);
            }
            ViewBag.divcount = discount;


            return View();
        }

        public ActionResult Dashboard()
        {
            TempData["PageHead"] = "View Dashboard";
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            EmpDetails emp = _objHomeAssetRegistration.GetEmp_Details_Ki_Wise(userId);
            List<DivisionWise_Asset_Count> discount = new List<DivisionWise_Asset_Count>();

            // For Admin
            if (userId == 6399)
            {
                List<SYKI> iList1 = new List<SYKI>();
                var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIListForViewTeam();
                foreach (var item in kiData._SYKIList)
                {
                    //if (item.ACTIVE == 1)
                    iList1.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                }
                ViewBag.SYKIDatanew = new SelectList(iList1, "SYKIID", "KICODE");
                ViewBag.CurrentSYKIData = kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault();
                decimal current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
                List<ADORGLEVEL> opList = _objHomeAssetRegistration.BindOperation((long)current_ki);
                ITDivisionApprovalgetSearchFilterData(new SearchParameterList());
                ViewBag.OpList = new SelectList(opList, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.DivisionList = new SelectList(string.Empty, "DIVISIONID", "DIVISION");

                var result = _objHomeAssetRegistration.Get_Asset_Details_Operation_Division_Wise((long)current_ki, 0, 0);
                ViewBag.Assetdetails = result;
                ViewBag.TopSecret = result.Where(x => x.ClassificationID == 1 && x.Reason_NA == null).Count();
                ViewBag.Secret = result.Where(x => x.ClassificationID == 2 && x.Reason_NA == null).Count();
                ViewBag.Internal = result.Where(x => x.ClassificationID == 3 && x.Reason_NA == null).Count();


                var resultcount = _objHomeAssetRegistration.Get_Asset_Count_DivisionWise((long)current_ki, 0, 0);
                for (int i = 0; i < resultcount.Count; i++)
                {
                    DivisionWise_Asset_Count d = new DivisionWise_Asset_Count();
                    d.DivisionName = resultcount[i].DivisionName;
                    d.TSCount = resultcount[i].TSCount;
                    d.SCount = resultcount[i].SCount;
                    d.ISCount = resultcount[i].ISCount;
                    discount.Add(d);
                }
                ViewBag.divcount = discount;
            }
            else if (emp.Degination == "Operating Head")  // For Operating Head
            {
                List<SYKI> iList1 = new List<SYKI>();
                var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIListForViewTeam();
                foreach (var item in kiData._SYKIList)
                {
                    if (item.ACTIVE == 1)
                        iList1.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                }
                ViewBag.SYKIDatanew = new SelectList(iList1, "SYKIID", "KICODE");
                ViewBag.CurrentSYKIData = kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault();
                decimal current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
                List<ADORGLEVEL> opList = _objHomeAssetRegistration.BindOperation((long)current_ki);
                ITDivisionApprovalgetSearchFilterData(new SearchParameterList());
                ViewBag.OpList = new SelectList(opList.Where(x => x.ADORGLEVELID == emp.OperationId), "ADORGLEVELID", "LEVELDESCRIP", emp.OperationId);
                List<SearchParameterList> divList = _objHomeAssetRegistration.BindDivision(emp.OperationId);
                ViewBag.DivisionList = new SelectList(divList, "DIVISIONID", "DIVISION", emp.DivisionId);

                var result = _objHomeAssetRegistration.Get_Asset_Details_Operation_Division_Wise((long)current_ki, (long)emp.OperationId, 0);
                ViewBag.Assetdetails = result;
                ViewBag.TopSecret = result.Where(x => x.ClassificationID == 1 && x.Reason_NA == null).Count();
                ViewBag.Secret = result.Where(x => x.ClassificationID == 2 && x.Reason_NA == null).Count();
                ViewBag.Internal = result.Where(x => x.ClassificationID == 3 && x.Reason_NA == null).Count();

                var resultcount = _objHomeAssetRegistration.Get_Asset_Count_DivisionWise((long)current_ki, (long)emp.OperationId, 0);
                for (int i = 0; i < resultcount.Count; i++)
                {
                    DivisionWise_Asset_Count d = new DivisionWise_Asset_Count();
                    d.DivisionName = resultcount[i].DivisionName;
                    d.TSCount = resultcount[i].TSCount;
                    d.SCount = resultcount[i].SCount;
                    d.ISCount = resultcount[i].ISCount;
                    discount.Add(d);
                }
                ViewBag.divcount = discount;
            }
            else // ISSC_Member and Division Head
            {
                List<SYKI> iList1 = new List<SYKI>();
                var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIListForViewTeam();
                foreach (var item in kiData._SYKIList)
                {
                    if (item.ACTIVE == 1)
                        iList1.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                }
                ViewBag.SYKIDatanew = new SelectList(iList1, "SYKIID", "KICODE");
                ViewBag.CurrentSYKIData = kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault();
                decimal current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
                List<ADORGLEVEL> opList = _objHomeAssetRegistration.BindOperation((long)current_ki);
                ITDivisionApprovalgetSearchFilterData(new SearchParameterList());
                ViewBag.OpList = new SelectList(opList.Where(x => x.ADORGLEVELID == emp.OperationId), "ADORGLEVELID", "LEVELDESCRIP", emp.OperationId);
                List<SearchParameterList> divList = _objHomeAssetRegistration.BindDivision(emp.OperationId);
                ViewBag.DivisionList = new SelectList(divList.Where(x => x.DIVISIONID == emp.DivisionId), "DIVISIONID", "DIVISION", emp.DivisionId);

                var result = _objHomeAssetRegistration.Get_Asset_Details_Operation_Division_Wise((long)current_ki, (long)emp.OperationId, emp.DivisionId);
                ViewBag.Assetdetails = result;
                ViewBag.TopSecret = result.Where(x => x.ClassificationID == 1 && x.Reason_NA == null).Count();
                ViewBag.Secret = result.Where(x => x.ClassificationID == 2 && x.Reason_NA == null).Count();
                ViewBag.Internal = result.Where(x => x.ClassificationID == 3 && x.Reason_NA == null).Count();

                var resultcount = _objHomeAssetRegistration.Get_Asset_Count_DivisionWise((long)current_ki, (long)emp.OperationId, emp.DivisionId);
                for (int i = 0; i < resultcount.Count; i++)
                {
                    DivisionWise_Asset_Count d = new DivisionWise_Asset_Count();
                    d.DivisionName = resultcount[i].DivisionName;
                    d.TSCount = resultcount[i].TSCount;
                    d.SCount = resultcount[i].SCount;
                    d.ISCount = resultcount[i].ISCount;
                    discount.Add(d);
                }
                ViewBag.divcount = discount;
            }
            return View();
        }

        [HttpPost]
        public ActionResult AssestSearch(long SYKIDatanew, decimal? OPERATIONID, decimal? DIVISIONID)
        {
            TempData["PageHead"] = "Asset Register Report";
            var result = _objHomeAssetRegistration.Get_Asset_Details_Operation_Division_Wise(SYKIDatanew, OPERATIONID == null ? 0 : OPERATIONID, DIVISIONID == null ? 0 : DIVISIONID);
            ViewBag.Assetdetails = result;
            ViewBag.TopSecret = result.Where(x => x.ClassificationID == 1 && x.Reason_NA == null).Count();
            ViewBag.Secret = result.Where(x => x.ClassificationID == 2 && x.Reason_NA == null).Count();
            ViewBag.Internal = result.Where(x => x.ClassificationID == 3 && x.Reason_NA == null).Count();
            return View();
        }

        [HttpGet]
        public ActionResult AssetRemarkByDivisionHead(int ISSC_Code)
        {
            TempData["PageHead"] = "Information Security Register updation";
            List<SYKI> iList = new List<SYKI>();
            var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();

            ViewBag.EnddateCheck = _objHomeAssetRegistration.ISSC_Member_AssetDetails_EndDate_Check(ISSC_Code, (long)current_ki);
            ViewBag.Final_Submit_Check = _objHomeAssetRegistration.Operating_Head_AssetDetails_FinalSubmit_Check(userId, (long)current_ki);

            var result = _objHomeAssetRegistration.Get_ISSC_Member_Asset_For_DivisionHead(ISSC_Code, (long)current_ki);
            ViewBag.Assetdetails = result;
            ViewBag.TopSecret = result.Where(x => x.ClassificationID == 1 && x.Reason_NA == null).Count();
            ViewBag.Secret = result.Where(x => x.ClassificationID == 2 && x.Reason_NA == null).Count();
            ViewBag.Internal = result.Where(x => x.ClassificationID == 3 && x.Reason_NA == null).Count();
            CommonAAssetsRegListVM asset_deatils = new CommonAAssetsRegListVM();
            asset_deatils.ISSC_Member_EmpCode = ISSC_Code;
            asset_deatils.SYKIID = current_ki;
            ViewBag.Remarks = _objHomeAssetRegistration.GetRemrks((long)current_ki, ISSC_Code);
            return View(asset_deatils);
        }
        public ActionResult ViewOrganizationMappingAssetRegister()
        {
            TempData["PageHead"] = "View Organization  Mapping";
            A_SearchParameterList paramsList = new A_SearchParameterList();
            AssetRegistrationViewModel model = new AssetRegistrationViewModel();
            try
            {
                //Dinesh
                var paramData = OrgMappingSearchFilterData(paramsList);
                //***
                //model.SearchParams.SYKI = paramData.SYKIID;
                //model.SearchParams.Status = paramData.Status;

                OrgMappingDetailsSearchData(model, paramData);
                // var result=  _objHomeAssetRegistration.GetOrgMappingData(paramData);
                // ViewBag.OrgList = result;

                //List<ADORGLEVEL> _opList = _objHomeA00.GetOrgLevelList((long)1);
                List<ADORGLEVEL> _opList = _objHomeAssetRegistration.BindOperation((long)paramData.SYKIID);
                ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.DivList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.DepList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.SecList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
            }
            catch (Exception ex)
            {
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult ViewOrganizationMappingAssetRegister(AssetRegistrationViewModel model)
        {
            TempData["PageHead"] = "View Organization Mapping";
            A_SearchParameterList paramlist = new A_SearchParameterList();
            try
            {
                var sykiId = 0;
                if (!string.IsNullOrEmpty(Request.Form["SearchParams.SYKI"]))
                {
                    sykiId = Convert.ToInt32(Request.Form["SearchParams.SYKI"].ToString());
                }
                paramlist.SYKIID = sykiId;
                paramlist.OPERATIONID = Request.Form["SearchParams.OPERATIONID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.OPERATIONID"]) : 0;
                paramlist.DIVISIONID = Request.Form["SearchParams.DIVISIONID"] != "" ? Convert.ToInt64(Request.Form["SearchParams.DIVISIONID"]) : 0;
                //Dinesh
                var paramData = OrgMappingSearchFilterData(paramlist);
                //***
                //model.SearchParams.SYKI = paramData.SYKIID;
                //model.SearchParams.Status = paramData.Status;

                OrgMappingDetailsSearchData(model, paramData);
                // var result=  _objHomeAssetRegistration.GetOrgMappingData(paramData);
                // ViewBag.OrgList = result;
                List<ADORGLEVEL> _opList = _objHomeAssetRegistration.BindOperation((long)paramData.SYKIID);
                ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP", paramlist.OPERATIONID);
                ViewBag.DivList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP", paramlist.DIVISIONID);
            }
            catch (Exception ex)
            {
            }
            return View(model);
        }

        private A_SearchParameterList OrgMappingSearchFilterData(A_SearchParameterList paramsList)
        {
            List<A_SearchParameterList> list = new List<A_SearchParameterList>();
            A_SearchParameterList data = new A_SearchParameterList();

            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

            #region Bind Ki drop down
            List<SearchParameterList> kiData = new List<SearchParameterList>();
            kiData.Add(new SearchParameterList() { SYKIID = 0, SYKI = "Select" });

            var kiData1 = _objHomeAssetRegistration.GetAssetRegistrationSYKIListForViewTeam();
            var activeKi = kiData1._SYKIList.Where(x => x.ACTIVE == 1).FirstOrDefault();
            var kii = kiData1._SYKIList.OrderByDescending(x => x.SYKIID).Select(x => new SearchParameterList { SYKIID = x.SYKIID, SYKI = x.KICODE }).ToList();
            kiData.AddRange(kii);
            var selectedKi = (paramsList.SYKIID == 0 || paramsList.SYKIID == null) ? activeKi.SYKIID : paramsList.SYKIID;
            ViewBag.SYKIData = new SelectList(kiData, "SYKIID", "SYKI", selectedKi);

            #endregion

            #region Bind status drop down
            #endregion
            data.SYKIID = selectedKi;
            paramsList.SYKIID = selectedKi;

            #region Bind Login Employee Details
            AssetRegistrationSearchModel _objA00SearchModel = new AssetRegistrationSearchModel();
            _objA00SearchModel.ADEMPCODE = userId;
            _objA00SearchModel.SYKIID = selectedKi;

            var empDetails = _objHomeAssetRegistration.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel);
            if (empDetails != null)
            {
                data.OPERATION = empDetails.OPERATION != null ? empDetails.OPERATION : "--No record--";
                data.OPERATIONID = empDetails.OPERATIONID != null ? empDetails.OPERATIONID : 0;
                paramsList.OPERATIONID = (paramsList.OPERATIONID == 0 || paramsList.OPERATIONID == null) ? paramsList.OPERATIONID : data.OPERATIONID;
                data.DIVISION = empDetails.DIVISION != null ? empDetails.DIVISION : "--No record--";
                data.DIVISIONID = empDetails.DIVISIONID != null ? empDetails.DIVISIONID : 0;
                paramsList.DIVISIONID = (paramsList.DIVISIONID == 0 || paramsList.DIVISIONID == null) ? paramsList.DIVISIONID : data.DIVISIONID;
            }
            else
            {
                data.OPERATION = "--No record--";
                data.OPERATIONID = 0;
                data.DIVISION = "--No record--";
                data.DIVISIONID = 0;
            }
            #endregion


            list.Add(data);
            ViewBag.List = list;
            return paramsList;
        }
        private void OrgMappingDetailsSearchData(AssetRegistrationViewModel model, A_SearchParameterList paramsData)
        {
            var userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

            model.SearchParams = new A_SearchParameterList();
            model.SearchParams.OPERATIONID = paramsData.OPERATIONID;
            model.SearchParams.DIVISIONID = paramsData.DIVISIONID;
            var sykiId = paramsData.SYKIID;//0;

            // SearchParams.StatusId
            model.SearchParams.SYKIID = sykiId;

            model.SearchParams.SYKI = paramsData.SYKI;

            var status = model.SearchParams.StatusId;
            var result = _objHomeAssetRegistration.GetOrgMappingData(paramsData);
            model.orgMappingVM = result.ToList();

        }
        public ActionResult BindOperationByUserIDandSykiId(long SYKIID)
        {
            List<A_SearchParameterList> _opList = new List<A_SearchParameterList>();
            var userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            AssetRegistrationSearchModel _objA00SearchModel = new AssetRegistrationSearchModel();
            _objA00SearchModel.ADEMPCODE = userId;
            _objA00SearchModel.SYKIID = SYKIID;
            var empDetails = _objHomeAssetRegistration.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel);
            if (empDetails != null)
            {
                _opList = new List<A_SearchParameterList>
                {
                    new A_SearchParameterList
                    {
                         OPERATION = empDetails.OPERATION, OPERATIONID = empDetails.OPERATIONID,
                         DIVISION = empDetails.DIVISION, DIVISIONID = empDetails.DIVISIONID,
                    }
                };
            }
            //List<A00ADORGLEVELList> _opList1 = _objHomeA00.GetA00ADORGLEVELList((decimal)SYKIID)._ADOrgLevelList;
            return Json(_opList);
        }
        public ActionResult AddEditOrgMapping(int? OrgMappingId)
        {
            TempData["PageHead"] = "Organization Mapping";
            A_SearchParameterList paramsList = new A_SearchParameterList();
            List<A_SearchParameterList> list = new List<A_SearchParameterList>();
            A_SearchParameterList data = new A_SearchParameterList();

            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

            #region Bind Ki drop down
            List<A_SearchParameterList> kiData = new List<A_SearchParameterList>();

            var kiData1 = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            var activeKi = kiData1._SYKIList.Where(x => x.ACTIVE == 1).FirstOrDefault();
            var kii = kiData1._SYKIList.OrderByDescending(x => x.SYKIID).Select(x => new A_SearchParameterList { SYKIID = x.SYKIID, SYKI = x.KICODE }).ToList();
            kiData.AddRange(kii);
            var selectedKi = (paramsList.SYKIID == 0 || paramsList.SYKIID == null) ? activeKi.SYKIID : paramsList.SYKIID;
            ViewBag.SYKIData = new SelectList(kiData, "SYKIID", "SYKI", selectedKi);
            // ViewBag.SYKIData = new SelectList(kiData, "SYKIID", "SYKI");
            #endregion

            #region Bind status drop down
            #endregion
            data.SYKIID = selectedKi;
            paramsList.SYKIID = selectedKi;

            #region Bind Login Employee Details
            AssetRegistrationSearchModel _objA00SearchModel = new AssetRegistrationSearchModel();
            _objA00SearchModel.ADEMPCODE = userId;
            _objA00SearchModel.SYKIID = selectedKi;
            // _objA00SearchModel.SYKIID = 0;
            var empDetails = _objHomeAssetRegistration.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel);
            if (empDetails != null)
            {
                data.OPERATION = empDetails.OPERATION != null ? empDetails.OPERATION : "--No record--";
                data.OPERATIONID = empDetails.OPERATIONID != null ? empDetails.OPERATIONID : 0;
                paramsList.OPERATIONID = paramsList.OPERATIONID > 0 ? paramsList.OPERATIONID : data.OPERATIONID;
                data.DIVISION = empDetails.DIVISION != null ? empDetails.DIVISION : "--No record--";
                data.DIVISIONID = empDetails.DIVISIONID != null ? empDetails.DIVISIONID : 0;
                paramsList.DIVISIONID = paramsList.DIVISIONID > 0 ? paramsList.DIVISIONID : data.DIVISIONID;
                data.LastSYKI = empDetails.LastSYKI != null ? empDetails.LastSYKI : "--No record--";
                data.LastSKID = empDetails.LastSYKIID;
                paramsList.LastSKID = paramsList.LastSKID > 0 ? paramsList.LastSKID : data.LastSKID;
            }
            else
            {
                data.OPERATION = "--No record--";
                data.OPERATIONID = 0;
                data.DIVISION = "--No record--";
                data.DIVISIONID = 0;
                data.LastSYKI = "--No record--";
                data.LastSKID = 0;
            }
            #endregion
            OrgMappingVM orgMappingVM = new OrgMappingVM();
            if (OrgMappingId > 0)
            {
                orgMappingVM = _objHomeAssetRegistration.GetAddEditOrgMappingData(OrgMappingId);
            }
            List<ADORGLEVEL> _opList = _objHomeAssetRegistration.BindOperation((long)selectedKi);
            List<ADORGLEVEL> _opList1 = _objHomeAssetRegistration.BindOperation((long)empDetails.LastSYKIID);
            ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP");
            ViewBag.OpList1 = new SelectList(_opList1, "ADORGLEVELID", "LEVELDESCRIP");
            if (OrgMappingId > 0)
            {
                List<SearchParameterList> divList = _objHomeAssetRegistration.BindDivision(Convert.ToInt64(orgMappingVM.CurrentOperationID));
                ViewBag.DivList = new SelectList(divList, "DIVISIONID", "DIVISION");
                List<SearchParameterList> divList1 = _objHomeAssetRegistration.BindDivision(Convert.ToInt64(orgMappingVM.PreviousOperationID));
                ViewBag.DivList1 = new SelectList(divList1, "DIVISIONID", "DIVISION");
            }
            else
            {
                ViewBag.DivList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.DivList1 = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
            }
            list.Add(data);
            ViewBag.List = list;

            return View(orgMappingVM);
        }
        public ActionResult BindOperationByUserIDandCurrentSykiId(long SYKIID)
        {
            List<A_SearchParameterList> _opList = new List<A_SearchParameterList>();
            var userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            AssetRegistrationSearchModel _objA00SearchModel = new AssetRegistrationSearchModel();
            _objA00SearchModel.ADEMPCODE = userId;
            _objA00SearchModel.SYKIID = SYKIID;
            var empDetails = _objHomeAssetRegistration.GetA00_VW_ASSOCIATELVLDETAILS(_objA00SearchModel);
            if (empDetails != null)
            {
                _opList = new List<A_SearchParameterList>
                {
                    new A_SearchParameterList
                    {
                         OPERATION = empDetails.OPERATION, OPERATIONID = empDetails.OPERATIONID,
                         DIVISION = empDetails.DIVISION, DIVISIONID = empDetails.DIVISIONID,
                         LastSYKI=empDetails.LastSYKI, LastSKID=empDetails.LastSYKIID,
                         LastDIVISION=empDetails.LastDIVISION, LastDIVISIONID=empDetails.LastDIVISIONID,
                         LastOPERATION=empDetails.LastOPERATION, LastOPERATIONID=empDetails.LastOPERATIONID,
                    }
                };
            }
            //List<A00ADORGLEVELList> _opList1 = _objHomeA00.GetA00ADORGLEVELList((decimal)SYKIID)._ADOrgLevelList;
            return Json(_opList);
        }
        [HttpPost]
        //public ActionResult AddEditOrgMapping(OrgMappingVM orgMappingVM, FormCollection fc)
        public ActionResult AddEditOrgMapping(OrgMappingVM orgMappingVM)
        {
            try
            {
                TempData["PageHead"] = "Organization Mapping";

                var userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
                if (orgMappingVM.OrgMappingId > 0)
                {
                    orgMappingVM.UpdatedBY = userId;
                    orgMappingVM.UpdatedDate = System.DateTime.Now;
                }
                else
                {
                    orgMappingVM.CreatedBY = userId;
                    orgMappingVM.CreatedDate = System.DateTime.Now;
                }
                TempData["result"] = _objHomeAssetRegistration.OrgMappingSaveData(orgMappingVM);
                if (orgMappingVM.OrgMappingId > 0)
                {
                    TempData["result"] = 2;
                }

                return RedirectToAction("ViewOrganizationMappingAssetRegister");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public ActionResult BindOperationBySykiId(long SYKIID)
        {
            List<ADORGLEVEL> _opList = _objHomeAssetRegistration.BindOperation((long)SYKIID);
            //ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP");
            return Json(_opList);
        }

        public ActionResult Mailer_For_ISSC_Nomination(int? id)
        {
            TempData["PageHead"] = "Reminder Mail For ISSC Nomination";
            List<SYKI> iList = new List<SYKI>();
            var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }

            ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE");
            ViewBag.CurrentSYKIData = DateTime.Now.Date;
            var currentki = iList.Select(x => x.SYKIID).FirstOrDefault();
            var MaileReminderlist = _objHomeAssetRegistration.GetReminderDetailsForISSCMember_Nomination();
            ViewBag.MaileReminderlist = MaileReminderlist;
            MailerReminderVM MaileReminder = _objHomeAssetRegistration.GetStart_Dt_End_Dt_ISSC_Nomination((long)currentki);
            if (id > 0 && MaileReminder!=null)
            {
                MaileReminder.ReminerDate = _objHomeAssetRegistration.GetReminderDate((long)id);
                MaileReminder.ReminderId = id;
            }
            return View(MaileReminder);
        }

        [HttpPost]
        public ActionResult Mailer_For_ISSC_Nomination(MailerReminderVM MaileReminder)
        {
            MaileReminder.ReminderFor = 1;
            var result = _objHomeAssetRegistration.InsertISSC_Reminder(MaileReminder);
            if (MaileReminder.ReminderId > 0)
                return Redirect("../../AssetRegistration/Mailer_For_ISSC_Nomination");
            else
                return RedirectToAction("Mailer_For_ISSC_Nomination");
        }

        public ActionResult Mailer_For_ISSC_Member(int? id)
        {
            TempData["PageHead"] = "Reminder Mail For Asset Register Updation/ Approval";
            List<SYKI> iList = new List<SYKI>();
            var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
            foreach (var item in kiData._SYKIList)
            {
                iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
            }

            ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE");
            ViewBag.CurrentSYKIData = DateTime.Now.Date;
            var currentki = iList.Select(x => x.SYKIID).FirstOrDefault();
            var MaileReminderlist = _objHomeAssetRegistration.GetReminderDetailsForISSCMember_Approval();
            ViewBag.MaileReminderlist = MaileReminderlist;
            MailerReminderVM MaileReminder = _objHomeAssetRegistration.GetStart_Dt_End_Dt_ISSC_Member((long)currentki);

            DateTime st = Convert.ToDateTime(MaileReminder.StartDate);
            MaileReminder.StartDate = st.ToString("dd/MM/yyyy");

            DateTime et = Convert.ToDateTime(MaileReminder.EndDate);
            MaileReminder.EndDate = et.ToString("dd/MM/yyyy");

            if (id > 0)
            {
                MaileReminder.ReminerDate = _objHomeAssetRegistration.GetReminderDate((long)id);
                MaileReminder.ReminderId = id;
            }
            return View(MaileReminder);
        }

        [HttpPost]
        public ActionResult Mailer_For_ISSC_Member(MailerReminderVM MaileReminder)
        {
            MaileReminder.ReminderFor = 2;
            var result = _objHomeAssetRegistration.InsertISSC_Reminder(MaileReminder);
            if (MaileReminder.ReminderId > 0)
                return Redirect("../../AssetRegistration/Mailer_For_ISSC_Member");
            else
                return RedirectToAction("Mailer_For_ISSC_Member");
        }

        public ActionResult BindChart1(long? SYKIID)
        {
            List<ADORGLEVEL> divList = _objHomeAssetRegistration.BindOperation(SYKIID);
            var result = _objHomeAssetRegistration.Get_Asset_Details_Operation_Division_Wise((long)SYKIID, 0, 0);

            int top1 = result.Where(x => x.ClassificationID == 1 && x.Reason_NA == null).Count();
            int clas1 = result.Where(x => x.ClassificationID == 2 && x.Reason_NA == null).Count();
            int internal1 = result.Where(x => x.ClassificationID == 3 && x.Reason_NA == null).Count();
            string s = top1 + "," + clas1 + "," + internal1;

            return Json(s);
        }

        public ActionResult BindChart2(long? SYKIID)
        {
            var resultcount = _objHomeAssetRegistration.Get_Asset_Count_DivisionWise((long)SYKIID, 0, 0);
            List<DivisionWise_Asset_Count> discount = new List<DivisionWise_Asset_Count>();

            for (int i = 0; i < resultcount.Count; i++)
            {
                DivisionWise_Asset_Count d = new DivisionWise_Asset_Count();
                d.DivisionName = resultcount[i].DivisionName;
                d.TSCount = resultcount[i].TSCount;
                d.SCount = resultcount[i].SCount;
                d.ISCount = resultcount[i].ISCount;
                discount.Add(d);
            }
            return Json(discount);
        }

        public JsonResult GetISSCMemberNomination()
        {
            List<ISSC_Member_Nomination_deatilsVM> pendinglist = new List<ISSC_Member_Nomination_deatilsVM>();
            try
            {
                int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
                List<SYKI> iList = new List<SYKI>();
                // iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });
                var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
                foreach (var item in kiData._SYKIList)
                {
                    iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                }
                var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();

                pendinglist = _objHomeAssetRegistration.Get_ISSC_member_Nomintion_pending((long)current_ki, userId);
                return Json(pendinglist);
            }
            catch (Exception ex)
            {
            }
            return Json(pendinglist);
        }

        public JsonResult GetAssetRegisterUpdateDetails()
        {
            List<Get_Division_ISSC_Member> pendinglist = new List<Get_Division_ISSC_Member>();
            try
            {
                int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
                pendinglist = _objHomeAssetRegistration.Asset_pendingDetails_ISSCMembers(userId);
                return Json(pendinglist);
            }
            catch (Exception ex)
            {
            }
            return Json(pendinglist);
        }

        public JsonResult GetAssetRegisterApproveDetails()
        {
            List<Get_Division_ISSC_Member> pendinglist = new List<Get_Division_ISSC_Member>();
            try
            {
                int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
                pendinglist = _objHomeAssetRegistration.Get_Asset_Register_Details_For_OpearingHead(userId);
                return Json(pendinglist);
            }
            catch (Exception ex)
            {
            }
            return Json(pendinglist);
        }


        [HttpGet]
        public ActionResult AssetRegisterSubmitStatus()
        {
            SearchParameterList paramsList = new SearchParameterList();
            try
            {
                TempData["PageHead"] = "Asset Register Submit Status";
                List<SYKI> iList1 = new List<SYKI>();
                var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
                foreach (var item in kiData._SYKIList)
                {
                    iList1.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                }
                var curki = kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
                ViewBag.curki = curki;
                ViewBag.SYKIDatanew = new SelectList(iList1, "SYKIID", "KICODE");
                ViewBag.CurrentSYKIData = kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault(); ;
                ITDivisionApprovalgetSearchFilterData(new SearchParameterList());
                ViewBag.OpList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");
                ViewBag.DivisionList = new SelectList(string.Empty, "ADORGLEVELID", "LEVELDESCRIP");

                var ViewToList = _objHomeAssetRegistration.GetAssetRegisterSunmitStatus((long)curki, null, null);

                ViewBag.ViewToList = ViewToList;

            }
            catch (Exception ex)
            {
            }
            return View();

        }

        [HttpPost]
        public ActionResult AssetRegisterSubmitStatus(long? SYKIDatanew, long? OPERATIONID, long? DIVISIONID)
        {
            TempData["PageHead"] = "Asset Register Submit Status";
            SearchParameterList paramsList = new SearchParameterList();
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["btnSubmit"]))
                {
                    List<SYKI> iList1 = new List<SYKI>();
                    var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIListForViewTeam();
                    foreach (var item in kiData._SYKIList)
                    {
                        iList1.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                    }
                    ViewBag.SYKIDatanew = new SelectList(iList1, "SYKIID", "KICODE");
                    ViewBag.CurrentSYKIData = kiData._SYKIList.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault();
                    ITDivisionApprovalgetSearchFilterData(new SearchParameterList());

                    List<ADORGLEVEL> _opList = _objHomeAssetRegistration.BindOperation(SYKIDatanew);
                    ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP");
                    List<SearchParameterList> divList = _objHomeAssetRegistration.BindDivision(OPERATIONID);
                    ViewBag.DivisionList = new SelectList(divList, "DIVISIONID", "DIVISION", DIVISIONID);

                    var ViewToList = _objHomeAssetRegistration.GetAssetRegisterSunmitStatus(SYKIDatanew, OPERATIONID, DIVISIONID);
                    ViewBag.ViewToList = ViewToList;
                }

            }
            catch (Exception ex)
            {
            }

            return View();

        }

        //Added by Aumento Start 
        // public ActionResult GeneratePdfReport(AssetRegister assetregister) //  Added by TTL ::  SR103777 > CR6964  
        public ActionResult GeneratePdfReport(AssetRegister assetregister, string IsEdit) //  Added by TTL ::  SR103777 > CR6964  
        {
            try
            {
                bool isModified = _sessionService.Get<bool>("Modified");  //  Added by TTL ::  SR103777 > CR6964  

                //AssetRegisterReport assetregisterReport = new AssetRegisterReport();  //  Added by TTL ::  SR103777 > CR6964  
                AssetRegisterReport assetregisterReport = new AssetRegisterReport(isModified, IsEdit);  //  Added by TTL ::  SR103777 > CR6964  

                byte[] abytes = assetregisterReport.PrepareReport(GetAssetRegisterDetail());
                string pdfUrl = Url.Action("DownloadPdf", "AssetRegistration");

                return Json(new { success = true, url = pdfUrl });
                
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // public ActionResult DownloadPdf() //  Added by TTL ::  SR103777 > CR6964 
        public ActionResult DownloadPdf(string IsEdit) //  Added by TTL ::  SR103777 > CR6964 
        {

            bool isModified = _sessionService.Get<bool>("Modified");

            //AssetRegisterReport assetregisterReport = new AssetRegisterReport(); //  Added by TTL ::  SR103777 > CR6964 
            AssetRegisterReport assetregisterReport = new AssetRegisterReport(isModified, IsEdit); //  Added by TTL ::  SR103777 > CR6964 
            byte[] abytes = assetregisterReport.PrepareReport(GetAssetRegisterDetail());

            // Return the PDF file
            return File(abytes, "application/pdf", "AssetRegisterReport.pdf");
        }

         
        public List<AssetRegister> GetAssetRegisterDetail()
        {
            try
            {
                    List<AssetRegister> AssetRegisterList = new List<AssetRegister>();
                AssetRegister AssetList = new AssetRegister();
                List<SYKI> iList = new List<SYKI>();
                var kiData = _objHomeAssetRegistration.GetAssetRegistrationSYKIList();
                foreach (var item in kiData._SYKIList)
                {
                    iList.Add(new SYKI { KICODE = item.KICODE, SYKIID = item.SYKIID });
                }
                int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
                var current_ki = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();
                var current_kiCode = kiData._SYKIList.Select(x => x.KICODE).FirstOrDefault();
                ViewBag.Asset_details_List = _objHomeAssetRegistration.Get_Asset_Details_With_Common_Asset(userId, (long)current_ki).ToList();
                var FooterDetail = _objHomeAssetRegistration.Get_AssetRegister_DigitalSign_Detail(userId, (long)current_ki);
                var HeaderDetail = _objHomeAssetRegistration.Get_AssetRegister_Header_Detail(userId, (long)current_ki);
                if (HeaderDetail == null)
                {
                    throw new Exception("Please add data into the Asset Register Document Control Master.");
                }
                if (ViewBag.Asset_details_List == null || ViewBag.Asset_details_List.Count == 0)
                {
                    throw new Exception("No Data Found.");
                }
                int index = 1;
                foreach (var item in ViewBag.Asset_details_List)
                {
                    var as_type = item.AssetsID;
                    var cls_type = item.ClassificationID;
                    var s = item.Retention_Period;

                    AssetList = new AssetRegister();
                    AssetList.Id = index;
                    AssetList.PrimaryAsset = item.Primary;
                    AssetList.SecondaryAsset = item.Secondary;
                    AssetList.ShowITGRC = _objHomeAssetRegistration.IsOperationMatch(userId); // Added By Aumento :: SR102194

                    if (as_type == 1)
                    {
                        AssetList.AssetsType = "Information Hard";
                    }
                    if (as_type == 2)
                    {
                        AssetList.AssetsType = "Information Soft";
                    }
                    if (as_type == 3)
                    {
                        AssetList.AssetsType = "Physical";
                    }
                    if (as_type == 4)
                    {
                        AssetList.AssetsType = "Software";
                    }
                    if (as_type == 5)
                    {
                        AssetList.AssetsType = "Service";
                    }
                    if (as_type == 6)
                    {
                        AssetList.AssetsType = "People";
                    }
                    AssetList.AssetsLocation = item.Assetlocation;
                    AssetList.AssetsOwner = item.AssetOwner;
                    AssetList.AssetsCustodian = item.Custodian;
                    AssetList.AssetUser = item.AssetUser;
                    if (cls_type == 1)
                    {
                        AssetList.Classification = "Top secret";
                    }
                    if (cls_type == 2)
                    {
                        AssetList.Classification = "Secret";
                    }
                    if (cls_type == 3)
                    {
                        AssetList.Classification = "Internal";
                    }

                    if (s != null && s != "_")
                    {
                        string[] result = s.Split('_');
                        var mth = result[0];
                        var year = result[1];
                        var yr = ""; var mth_ = "";
                        if (year == "1" || year == "0")
                        {
                            yr = "Year";
                        }
                        else
                        {
                            yr = "Years";
                        }
                        if (mth == "1" || mth == "0")
                        {
                            mth_ = "Month";
                        }
                        else
                        {
                            mth_ = "Months";
                        }
                        AssetList.RetentionPeriod = year + yr + " " + mth + mth_;

                    }
                    else
                    {
                        AssetList.RetentionPeriod = "NA";
                    }
                    AssetList.Remarks = item.Retention_Remarks;
                    if (item.ActionType == 1)
                    {
                        if (item.Reason_NA == null)
                        {
                            AssetList.Applicability = "Yes";
                        }
                        else
                        {
                            AssetList.Applicability = "No";
                        }
                    }
                    else
                    {
                        AssetList.Applicability = " - ";
                    }
                    AssetList.ReasonForNA = item.Reason_NA;
                    AssetList.ISSCMember_Name = FooterDetail.ISSCMember_Name == null ? "" : FooterDetail.ISSCMember_Name;
                    AssetList.ISSC_Member_Submit_Date = FooterDetail.ISSC_Member_Submit_Date;
                    AssetList.DivisionHead_Name = FooterDetail.DivisionHead_Name == null ? "" : FooterDetail.DivisionHead_Name;
                    AssetList.DivisionHead_Submit_Date = FooterDetail.DivisionHead_Submit_Date;
                    AssetList.OperatingHead_Name = FooterDetail.OperatingHead_Name == null ? "" : FooterDetail.OperatingHead_Name;
                    AssetList.OperatingHead_Submit_Date = FooterDetail.OperatingHead_Submit_Date;
                    //Added by Aumento for SR91196
                    AssetList.ITGRC_Head_Name = FooterDetail.ITGRC_Head_Name == null ? "" : FooterDetail.ITGRC_Head_Name;
                    AssetList.ITGRC_Head_Submit_Date = FooterDetail.ITGRCHead_Submit_Date;
                    //Added by Aumento for SR91196
                    AssetList.DOCUMENT_TITLE = HeaderDetail.DOCUMENT_TITLE == null ? "" : HeaderDetail.DOCUMENT_TITLE;
                    AssetList.DATE_OF_RELEASE = HeaderDetail.DATE_OF_RELEASE;
                    AssetList.DOC_VERSION_NO = HeaderDetail.DOC_VERSION_NO == null ? "" : HeaderDetail.DOC_VERSION_NO;
                    AssetList.DOCUMENT_NUMBER = HeaderDetail.DOCUMENT_NUMBER == null ? "" : HeaderDetail.DOCUMENT_NUMBER;
                    //AssetList.AMENDMENT = HeaderDetail.AMENDMENT == null ? "" : HeaderDetail.AMENDMENT;
                    //AssetList.AMENDMENT = item.AMENDMENT_NO == null ? "1" : Convert.ToString(item.AMENDMENT_NO);
                    AssetList.AMENDMENT = item.AMENDMENT_NO == null ? 0 : item.AMENDMENT_NO;
                    AssetList.OPERATION = HeaderDetail.OPERATION == null ? "" : HeaderDetail.OPERATION;
                    AssetList.DIVISION = HeaderDetail.DIVISION == null ? "" : HeaderDetail.DIVISION;
                    //if(HeaderDetail != null)
                    //{
                    //    AssetList.DOCUMENT_TITLE = HeaderDetail.DOCUMENT_TITLE == null ? "" : HeaderDetail.DOCUMENT_TITLE;
                    //    AssetList.DATE_OF_RELEASE = HeaderDetail.DATE_OF_RELEASE;
                    //    AssetList.DOC_VERSION_NO = HeaderDetail.DOC_VERSION_NO == null ? "" : HeaderDetail.DOC_VERSION_NO;
                    //    AssetList.DOCUMENT_NUMBER = HeaderDetail.DOCUMENT_NUMBER == null ? "" : HeaderDetail.DOCUMENT_NUMBER;
                    //    AssetList.AMENDMENT = HeaderDetail.AMENDMENT == null ? "" : HeaderDetail.AMENDMENT;
                    //}

                    AssetList.KICODE = current_kiCode;
                    AssetRegisterList.Add(AssetList);
                    index++;
                }
                return AssetRegisterList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }        
    }
    //Added by Aumento End
    //Added by Aumento Start
    public class AssetRegister
    {
        public int Id { get; set; }
        public string PrimaryAsset { get; set; }
        public string SecondaryAsset { get; set; }
        public string AssetsType { get; set; }
        public string AssetsLocation { get; set; }
        public string AssetsOwner { get; set; }
        public string AssetsCustodian { get; set; }
        public string AssetUser { get; set; }
        public string Classification { get; set; }
        public string RetentionPeriod { get; set; }
        public string Remarks { get; set; }
        public string Applicability { get; set; }
        public string ReasonForNA { get; set; }
        public string ISSCMember_Name { get; set; }
        public string DivisionHead_Name { get; set; }
        public string OperatingHead_Name { get; set; }
        public DateTime? ISSC_Member_Submit_Date { get; set; }
        public DateTime? DivisionHead_Submit_Date { get; set; }
        public DateTime? OperatingHead_Submit_Date { get; set; }
        public string DOCUMENT_TITLE { get; set; }
        public DateTime? DATE_OF_RELEASE { get; set; }
        public string DOC_VERSION_NO { get; set; }
        public string DOCUMENT_NUMBER { get; set; }

        //public string AMENDMENT { get; set; }
        public decimal? AMENDMENT { get; set; }
        public string KICODE { get; set; }
        public string OPERATION { get; set; }
        public string DIVISION { get; set; }
        //Added by Aumento for SR91196
        public string ITGRC_Head_Name { get; set; }
        public DateTime? ITGRC_Head_Submit_Date { get; set; }
        //Added by Aumento for SR91196
        public bool ShowITGRC { get; set; } // Added By Aumento :: SR102194

    }
    public class AssetRegisterReport
    {
        int _totalColumn = 13;
        Document _document;
        Font _fontStyle;
        PdfPTable _pdfTable = new PdfPTable(13);
        PdfPCell _pdfPCell;
        MemoryStream _memoryStream = new MemoryStream();
        List<AssetRegister> _assetregister = new List<AssetRegister>();

        //  Added by TTL ::  SR103777 > CR6964 
        private bool _IsModified { get; set; }
        private string _IsEdit { get; set; }
        public AssetRegisterReport(bool isModified, string isEdit)
        {
            _IsModified = isModified;
            _IsEdit = isEdit;
        }
        //  Added by TTL ::  SR103777 > CR6964 
        public byte[] PrepareReport(List<AssetRegister> assetregister)
        {
            _assetregister = assetregister;

            _document = new Document(PageSize.A4, 0f, 0f, 0f, 0f);
            _document.SetPageSize(PageSize.A4);
            _document.SetMargins(20f, 20f, 20f, 70f);
            _pdfTable.WidthPercentage = 100;
            _pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;
            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            string ISSCMember_Name = assetregister.Max(item => item.ISSCMember_Name).ToString();

            string ISSC_Member_Submit_Date = (_IsModified && _IsEdit == "Y") ? "" : assetregister.Max(item => item.ISSC_Member_Submit_Date).ToString();   //  Added by TTL ::  SR103777 > CR6964 
            //string ISSC_Member_Submit_Date =  assetregister.Max(item => item.ISSC_Member_Submit_Date).ToString();   //  Added by TTL ::  SR103777 > CR6964 
            string DivisionHead_Name = assetregister.Max(item => item.DivisionHead_Name).ToString();
            //string DivisionHead_Submit_Date = assetregister.Max(item => item.DivisionHead_Submit_Date).ToString();   //  Added by TTL ::  SR103777 > CR6964 
            string DivisionHead_Submit_Date = (_IsModified && _IsEdit == "Y") ? "" : assetregister.Max(item => item.DivisionHead_Submit_Date).ToString();   //  Added by TTL ::  SR103777 > CR6964 
            //Added by Aumento for SR91196
            string ITGRCHead_Name = assetregister.Max(item => item.ITGRC_Head_Name).ToString();
            //string ITGRCHead_Submit_Date = assetregister.Max(item => item.ITGRC_Head_Submit_Date).ToString();   //  Added by TTL ::  SR103777 > CR6964 
            string ITGRCHead_Submit_Date = (_IsModified && _IsEdit == "Y") ? "" : assetregister.Max(item => item.ITGRC_Head_Submit_Date).ToString();   //  Added by TTL ::  SR103777 > CR6964 
            //Added by Aumento for SR91196
            //if (!String.IsNullOrEmpty(DivisionHead_Submit_Date))
            //{
            //    DivisionHead_Submit_Date = DivisionHead_Submit_Date.ToString("dd/MM/yyyy");
            //}
            string OperatingHead_Name = assetregister.Max(item => item.OperatingHead_Name).ToString();
            //string OperatingHead_Submit_Date = assetregister.Max(item => item.OperatingHead_Submit_Date).ToString();   //  Added by TTL ::  SR103777 > CR6964 
            string OperatingHead_Submit_Date = (_IsModified && _IsEdit == "Y") ? "" : assetregister.Max(item => item.OperatingHead_Submit_Date).ToString();   //  Added by TTL ::  SR103777 > CR6964 
                                                                                                                                                              //Added by Aumento for SR91196
            bool ShowITGRC = assetregister.Max(item => item.ShowITGRC); // Added By Aumento :: SR102194

            Footer footerEvent = new Footer(ISSCMember_Name, ISSC_Member_Submit_Date, DivisionHead_Name, DivisionHead_Submit_Date, OperatingHead_Name, OperatingHead_Submit_Date, ITGRCHead_Name, ITGRCHead_Submit_Date, ShowITGRC);
            //Added by Aumento for SR91196
            //Footer footerEvent = new Footer(ISSCMember_Name, ISSC_Member_Submit_Date, DivisionHead_Name, DivisionHead_Submit_Date, OperatingHead_Name, OperatingHead_Submit_Date);
            //PdfWriter.GetInstance(_document,_memoryStream);
            PdfWriter pdfWriter = PdfWriter.GetInstance(_document, _memoryStream);
            pdfWriter.PageEvent = footerEvent;
            _document.Open();
            _pdfTable.SetWidths(new float[] { 20f, 40f, 40f, 80f, 50f, 50f, 50f, 50f, 50f, 50f, 50f, 50f, 50f });
            String DocumentTitle = assetregister.Max(item => item.DOCUMENT_TITLE).ToString();
            String DateofRelease = assetregister.Max(item => item.DATE_OF_RELEASE).ToString();
            String DocVersionNo = assetregister.Max(item => item.DOC_VERSION_NO).ToString();
            String DocumentNumber = assetregister.Max(item => item.DOCUMENT_NUMBER).ToString();
            //String KICODE = assetregister.Max(item => item.KICODE).ToString();
            String KICODE = assetregister.Max(item => item.KICODE).ToString().ToLower();
            String AMENDMENT = assetregister.Max(item => item.AMENDMENT).ToString();
            String OPERATION = assetregister.Max(item => item.OPERATION).ToString();
            String DIVISION = assetregister.Max(item => item.DIVISION).ToString();
            this.ReportHeader(DocumentTitle, DateofRelease, DocVersionNo, DocumentNumber, KICODE, AMENDMENT, OPERATION, DIVISION);
            this.ReportBody();
            _pdfTable.HeaderRows = 10;
            _document.Add(_pdfTable);
            _document.Close();
            return _memoryStream.ToArray();
        }
        private void ReportHeader(String DocumentTitle, String DateofRelease, String DocVersionNo, String DocumentNumber, String KICODE, String AMENDMENT,String OPERATION,String DIVISION)
        {
            if (AMENDMENT == "0")
            {
                AMENDMENT = "";
            }
            //Main Title
            
            String ReportTitle = "Information Asset Register - " + KICODE + " - Amendment - " + AMENDMENT;
            
            //Change by aumento for logo========================================

            _fontStyle = FontFactory.GetFont("Tahoma", 14f, 1);
            _pdfPCell = new PdfPCell(new Phrase(ReportTitle, _fontStyle));
            _pdfPCell.Colspan = _totalColumn - 2;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.Border = 0;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfPCell);
            
            //Below change added by aumento for logo-------------------------------------------------
            //string relativePathimg = "assets/images/logoex.png";
            // string relativePathimg = "Uploads\\HondaLogo\\Logo.png";
            // string baseDirectory1 = AppDomain.CurrentDomain.BaseDirectory;
            // string fullPathimg = Path.Combine(baseDirectory1, relativePathimg);
            string fullPathimg = serverpath.getFileUploadPath("HondaLogo/Logo.png");
            //----------------------------------------------------------------------------------------- 
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(fullPathimg);
            logo.ScaleAbsolute(50f, 50f); // Adjust the size as needed
            PdfPCell logoCell = new PdfPCell(logo);
            logoCell.Colspan = 2;
            logoCell.BackgroundColor = BaseColor.WHITE;
            logoCell.ExtraParagraphSpace = 0;
            logoCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            logoCell.Border = 0;
            _pdfTable.AddCell(logoCell);

            _pdfTable.CompleteRow();

            ////Blank Row 
            //_fontStyle = FontFactory.GetFont("Tahoma", 14f, 1);
            //_pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
            //_pdfPCell.Colspan = _totalColumn;
            //_pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            //_pdfPCell.Border = 0;
            //_pdfPCell.BackgroundColor = BaseColor.WHITE;
            //_pdfPCell.ExtraParagraphSpace = 0;
            //_pdfTable.AddCell(_pdfPCell);
            //_pdfTable.CompleteRow();

            //End Logo change===========================================

            //Sub Title
            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 1);
            _pdfPCell = new PdfPCell(new Phrase("Document Control", _fontStyle));
            _pdfPCell.Colspan = 9;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            //_pdfPCell.Border = 1;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfPCell);

            _fontStyle = FontFactory.GetFont("Tahoma", 11f, 1);
            _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
            _pdfPCell.Colspan = (_totalColumn - 9);
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.Border = 0;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfPCell);

            _pdfTable.CompleteRow();

            //Sub table Header
            _fontStyle = FontFactory.GetFont("Tahoma", 10f, 1);
            _pdfPCell = new PdfPCell(new Phrase("Sr.No.", _fontStyle));
            _pdfPCell.Colspan = 2;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            //_pdfPCell.Border = 1;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Type of Information", _fontStyle));
            _pdfPCell.Colspan = 2;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Document Data", _fontStyle));
            _pdfPCell.Colspan = 5;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
            _pdfPCell.Colspan = (_totalColumn - 9);
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.Border = 0;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfPCell);

            _pdfTable.CompleteRow();

            //Sub table Body Row 1
            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 0);
            _pdfPCell = new PdfPCell(new Phrase("1", _fontStyle));
            _pdfPCell.Colspan = 2;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Document Title", _fontStyle));
            _pdfPCell.Colspan = 2;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfPCell);

             DocumentTitle = DocumentTitle + " - " + OPERATION + " - " + DIVISION;
            _pdfPCell = new PdfPCell(new Phrase(DocumentTitle, _fontStyle));
            _pdfPCell.Colspan = 5;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
            _pdfPCell.Colspan = (_totalColumn - 9);
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.Border = 0;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfPCell);

            _pdfTable.CompleteRow();

            //Sub table Body Row 2----------------------------------
            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 0);
            _pdfPCell = new PdfPCell(new Phrase("2", _fontStyle));
            _pdfPCell.Colspan = 2;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Date of Release", _fontStyle));
            _pdfPCell.Colspan = 2;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfPCell);



            DateTime DateofRelease_ = Convert.ToDateTime(DateofRelease);
            string formattedDate = DateofRelease_.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
            _pdfPCell = new PdfPCell(new Phrase(formattedDate, _fontStyle));
            _pdfPCell.Colspan = 5;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
            _pdfPCell.Colspan = (_totalColumn - 9);
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.Border = 0;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfPCell);

            _pdfTable.CompleteRow();
            //Sub table Body Row 3----------------------------------
            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 0);
            _pdfPCell = new PdfPCell(new Phrase("3", _fontStyle));
            _pdfPCell.Colspan = 2;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Doc. Version No.", _fontStyle));
            _pdfPCell.Colspan = 2;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(DocVersionNo, _fontStyle));
            _pdfPCell.Colspan = 5;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
            _pdfPCell.Colspan = (_totalColumn - 9);
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.Border = 0;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfPCell);

            _pdfTable.CompleteRow();
            //Sub table Body Row 4----------------------------------
            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 0);
            _pdfPCell = new PdfPCell(new Phrase("4", _fontStyle));
            _pdfPCell.Colspan = 2;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Document Number", _fontStyle));
            _pdfPCell.Colspan = 2;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(DocumentNumber, _fontStyle));
            _pdfPCell.Colspan = 5;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
            _pdfPCell.Colspan = (_totalColumn - 9);
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.Border = 0;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfPCell);

            _pdfTable.CompleteRow();
            //Blank Row 
            _fontStyle = FontFactory.GetFont("Tahoma", 14f, 1);
            _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
            _pdfPCell.Colspan = _totalColumn;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.Border = 0;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfPCell);
            _pdfTable.CompleteRow();
        }
        private void ReportBody()
        {
            //Header
            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _pdfPCell = new PdfPCell(new Phrase("Sr.No", _fontStyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Primary Asset", _fontStyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Secondary Asset", _fontStyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Asset Type", _fontStyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Asset Location", _fontStyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            _pdfTable.AddCell(_pdfPCell);


            _pdfPCell = new PdfPCell(new Phrase("Asset Owner", _fontStyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            _pdfTable.AddCell(_pdfPCell);


            _pdfPCell = new PdfPCell(new Phrase("Asset Custodian", _fontStyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Asset User", _fontStyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Classification", _fontStyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Retention Period", _fontStyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Remarks", _fontStyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Applicability", _fontStyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            _pdfTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Reason For N/A", _fontStyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            _pdfTable.AddCell(_pdfPCell);

            _pdfTable.CompleteRow();

            //Body
            _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1);
            int sno = 1;
            foreach (AssetRegister A in _assetregister)
            {
                _pdfPCell = new PdfPCell(new Phrase(sno++.ToString(), _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(A.PrimaryAsset, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(A.SecondaryAsset, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);


                _pdfPCell = new PdfPCell(new Phrase(A.AssetsType, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(A.AssetsLocation, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(A.AssetsOwner, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(A.AssetsCustodian, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(A.AssetUser, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(A.Classification, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(A.RetentionPeriod, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(A.Remarks, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(A.Applicability, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfPCell = new PdfPCell(new Phrase(A.ReasonForNA, _fontStyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);

                _pdfTable.CompleteRow();
            }
        }
    }
    public class Footer : PdfPageEventHelper
    {
        private string _MemberName, _MemberDate, _DivHeadName, _DivHeadDate, _OperatingHeadName, _OperatingHeadDate;
        private string _ITGRCHeadName, _ITGRCHeadDate;        //Added by Aumento for SR91196
        private bool _ShowITGRC; // Added By Aumento :: SR102194
        //public Footer(string MemberName, string MemberDate, string DivHeadName, string DivHeadDate, string OperatingHeadName, string OperatingHeadDate)
        // public Footer(string MemberName, string MemberDate, string DivHeadName, string DivHeadDate, string OperatingHeadName, string OperatingHeadDate, string ITGRCHeadName, string ITGRCHeadDate) //Added by Aumento for SR91196 // Added By Aumento :: SR102194
        public Footer(string MemberName, string MemberDate, string DivHeadName, string DivHeadDate, string OperatingHeadName, string OperatingHeadDate, string ITGRCHeadName, string ITGRCHeadDate, bool ShowITGRC) // Added By Aumento :: SR102194
        {
            _MemberName = MemberName;
            if (MemberDate.Trim() != "")
            {
                DateTime MemberDT_ = Convert.ToDateTime(MemberDate.Trim());
                _MemberDate = MemberDT_.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                _MemberDate = MemberDate;
            }
            _DivHeadName = DivHeadName;
            if (DivHeadDate.Trim() != "")
            {
                DateTime DivHeadDT_ = Convert.ToDateTime(DivHeadDate.Trim());
                _DivHeadDate = DivHeadDT_.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                _DivHeadDate = DivHeadDate;
            }
            _OperatingHeadName = OperatingHeadName;
            if (OperatingHeadDate.Trim() != "")
            {
                DateTime OperatingHeadDT_ = Convert.ToDateTime(OperatingHeadDate.Trim());
                _OperatingHeadDate = OperatingHeadDT_.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                _OperatingHeadDate = OperatingHeadDate;
            }
            //Added by Aumento for SR91196
            _ITGRCHeadName = ITGRCHeadName;
            _ShowITGRC = ShowITGRC; // Added By Aumento :: SR102194
            if (ITGRCHeadDate.Trim() != "")
            {
                DateTime ITGRCHeadHeadDT_ = Convert.ToDateTime(ITGRCHeadDate.Trim());
                _ITGRCHeadDate = ITGRCHeadHeadDT_.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                _ITGRCHeadDate = ITGRCHeadDate;
            }
            //Added by Aumento for SR91196            
        }
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            PdfPTable footerTable = new PdfPTable(13);
            PdfPCell _pdfPCell;
            Font _fontStyle;
            footerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            footerTable.DefaultCell.Border = 0;

            //Authority Name
            _fontStyle = FontFactory.GetFont("Tahoma", 9f, 0);
            _pdfPCell = new PdfPCell(new Phrase(_MemberName, _fontStyle));
            _pdfPCell.Colspan = 2;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            footerTable.AddCell(_pdfPCell);

            // START :: Added By Aumento :: SR102194

            //Added by Aumento for SR91196
            //_pdfPCell = new PdfPCell(new Phrase(_ITGRCHeadName, _fontStyle));
            //_pdfPCell.Colspan = 2;
            //_pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            //_pdfPCell.BackgroundColor = BaseColor.WHITE;
            //_pdfPCell.ExtraParagraphSpace = 0;
            //footerTable.AddCell(_pdfPCell);
            //Added by Aumento for SR91196

            if (_ShowITGRC)
            {
                _pdfPCell = new PdfPCell(new Phrase(_ITGRCHeadName, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                footerTable.AddCell(_pdfPCell);
            }
            // END :: Added By Aumento :: SR102194

            _pdfPCell = new PdfPCell(new Phrase(_DivHeadName, _fontStyle));
            _pdfPCell.Colspan = 3;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            footerTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(_OperatingHeadName, _fontStyle));
            _pdfPCell.Colspan = 3;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            footerTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
            _pdfPCell.Colspan = 5;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.Border = 0;
            _pdfPCell.ExtraParagraphSpace = 0;
            footerTable.AddCell(_pdfPCell);

            footerTable.CompleteRow();

            //Authority Approve Date
            _fontStyle = FontFactory.GetFont("Tahoma", 9f, 0);
            _pdfPCell = new PdfPCell(new Phrase(_MemberDate, _fontStyle));
            _pdfPCell.Colspan = 2;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            footerTable.AddCell(_pdfPCell);

            // START :: Added By Aumento :: SR102194

            //Added by Aumento for SR91196
            // _pdfPCell = new PdfPCell(new Phrase(_ITGRCHeadDate, _fontStyle));
            // _pdfPCell.Colspan = 2;
            // _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            //_pdfPCell.BackgroundColor = BaseColor.WHITE;
            // _pdfPCell.ExtraParagraphSpace = 0;
            //  footerTable.AddCell(_pdfPCell);
            //Added by Aumento for SR91196

            if (_ShowITGRC)
            {
                _pdfPCell = new PdfPCell(new Phrase(_ITGRCHeadDate, _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                footerTable.AddCell(_pdfPCell);
            }

            // END :: Added By Aumento :: SR102194

            _pdfPCell = new PdfPCell(new Phrase(_DivHeadDate, _fontStyle));
            _pdfPCell.Colspan = 3;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            footerTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(_OperatingHeadDate, _fontStyle));
            _pdfPCell.Colspan = 3;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            footerTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
            _pdfPCell.Colspan = 5;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.Border = 0;
            _pdfPCell.ExtraParagraphSpace = 0;
            footerTable.AddCell(_pdfPCell);

            footerTable.CompleteRow();

            //Authority Detail
            _fontStyle = FontFactory.GetFont("Tahoma", 9f, 1);
            _pdfPCell = new PdfPCell(new Phrase("ISSC Member", _fontStyle));
            _pdfPCell.Colspan = 2;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            footerTable.AddCell(_pdfPCell);

            // START :: Added By Aumento :: SR102194

            //Added by Aumento for SR91196
            // _pdfPCell = new PdfPCell(new Phrase("ITGRC Head", _fontStyle));
            // _pdfPCell.Colspan = 2;
            // _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            // _pdfPCell.BackgroundColor = BaseColor.WHITE;
            // _pdfPCell.ExtraParagraphSpace = 0;
            // footerTable.AddCell(_pdfPCell);
            //Added by Aumento for SR91196

            if (_ShowITGRC)
            {
                _pdfPCell = new PdfPCell(new Phrase("ITGRC Head", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                footerTable.AddCell(_pdfPCell);
            }
            // END :: Added By Aumento :: SR102194

            _pdfPCell = new PdfPCell(new Phrase("Div Head", _fontStyle));
            _pdfPCell.Colspan = 3;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            footerTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Operating Head", _fontStyle));
            _pdfPCell.Colspan = 3;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            footerTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
            _pdfPCell.Colspan = 5;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.Border = 0;
            _pdfPCell.ExtraParagraphSpace = 0;
            footerTable.AddCell(_pdfPCell);

            footerTable.CompleteRow();

            //Sign By
            _fontStyle = FontFactory.GetFont("Tahoma", 9f, 1);
            _pdfPCell = new PdfPCell(new Phrase("Prepared By", _fontStyle));
            _pdfPCell.Colspan = 2;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            footerTable.AddCell(_pdfPCell);

            // START :: Added By Aumento :: SR102194

            //Added by Aumento for SR91196
            // _pdfPCell = new PdfPCell(new Phrase("Approved By", _fontStyle));
            // _pdfPCell.Colspan = 2;
            // _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            // _pdfPCell.BackgroundColor = BaseColor.WHITE;
            //_pdfPCell.ExtraParagraphSpace = 0;
            //footerTable.AddCell(_pdfPCell);
            //Added by Aumento for SR91196

            if (_ShowITGRC)
            {
                _pdfPCell = new PdfPCell(new Phrase("Approved By", _fontStyle));
                _pdfPCell.Colspan = 2;
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfPCell.ExtraParagraphSpace = 0;
                footerTable.AddCell(_pdfPCell);

            }
            // END :: Added By Aumento :: SR102194

            _pdfPCell = new PdfPCell(new Phrase("Reviewed By", _fontStyle));
            _pdfPCell.Colspan = 3;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            footerTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase("Approved By", _fontStyle));
            _pdfPCell.Colspan = 3;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            footerTable.AddCell(_pdfPCell);

            _pdfPCell = new PdfPCell(new Phrase(" ", _fontStyle));
            _pdfPCell.Colspan = 5;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.Border = 0;
            _pdfPCell.ExtraParagraphSpace = 0;
            footerTable.AddCell(_pdfPCell);

            footerTable.CompleteRow();

            footerTable.WriteSelectedRows(0, -1, document.LeftMargin, document.BottomMargin, writer.DirectContent);
        }
    }
    //Added by Aumento End





}


