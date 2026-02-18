using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    [SessionTimeout]
    public class OperatingHeadController : Controller
    {
       
        private readonly IOperatingHead _objHomeOperatingHead;
        private readonly ISessionService _sessionService;
        private readonly ILogger<PresidentDeskController> _logger;
        PresidentDeskSearchModel _objPresidentDeskSearchModel;

        public OperatingHeadController(IOperatingHead objHomeOperatingHead, ISessionService sessionService, ILogger<PresidentDeskController> logger)
        {
            _objHomeOperatingHead = objHomeOperatingHead;
            _sessionService = sessionService;
            _logger = logger;
            _objPresidentDeskSearchModel = new PresidentDeskSearchModel();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ISSCMemberOperatingHead()
        {
            TempData["PageHead"] = "Information Security Steering Committee (ISSC) Members Nomination";
            List<SYKI> iList = new List<SYKI>();
            // iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));

            var kiData = _objHomeOperatingHead.GetOperatingHeadSYKIList();
            iList = kiData._SYKIList.Select(x => new SYKI { KICODE = x.KICODE, SYKIID = x.SYKIID }).ToList();
            var sykid = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();

            ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE");
            var OperationID = _objHomeOperatingHead.GetOperationID(userId, (long)sykid);
            List<OperatingHeadSearchModel1> list = new List<OperatingHeadSearchModel1>();
            list = _objHomeOperatingHead.GetDivisionListForISSCMemberNomination((long)sykid, (long)OperationID, userId);
            List<empName> emplist = new List<empName>();
            emplist = _objHomeOperatingHead.GetEmpnameList();
            bool date = _objHomeOperatingHead.CheckDate_PeriodSetting((long)sykid);
            ViewBag.PeriodCheck = date == true ? 1 : 0;
            TempData["periodsettingdate"] = date;
            //ViewBag.Empname = new SelectList(emplist, "Empname", "Empname");
            ViewBag.OperatingModel = list;
            OperatingHeadAddNominationVM AddNominationVM = new OperatingHeadAddNominationVM();
            ViewBag.Check = _objHomeOperatingHead.Operatinghead_recordexist(userId, (long)sykid);
            return View(AddNominationVM);

        }
        [HttpPost]
        public ActionResult ISSCMemberOperatingHead(OperatingHeadAddNominationVM AddNominationVM, string action)
        {
            //if (Request.Form["btnSaveDraft"] != null)
                if(action== "Save as Draft")
            {
                AddNominationVM.Status = 0;
                TempData["msg"] = "Data has been saved as draft successfully.";
            }
            //else if (Request.Form["btnSubmit"] != null)
            else if (action == "Submit")
            {
                AddNominationVM.Status = 1;
                TempData["msg"] = "Data has been submitted successfully.";
            }

            TempData["PageHead"] = "Information Security Steering Committee (ISSC) Members Nomination";
            List<SYKI> iList = new List<SYKI>();
            // iList.Add(new SYKI { KICODE = "Select", SYKIID = 0 });
            int userId = Convert.ToInt32(_sessionService.Get<string>("userID"));
            var kiData = _objHomeOperatingHead.GetOperatingHeadSYKIList();
            iList = kiData._SYKIList.Select(x => new SYKI { KICODE = x.KICODE, SYKIID = x.SYKIID }).ToList();
            var sykid = kiData._SYKIList.Select(x => x.SYKIID).FirstOrDefault();

            ViewBag.SYKIData = new SelectList(iList, "SYKIID", "KICODE");
            var OperationID = _objHomeOperatingHead.GetOperationID(userId, (long)sykid);
            List<OperatingHeadSearchModel1> list = new List<OperatingHeadSearchModel1>();
            list = _objHomeOperatingHead.GetDivisionListForISSCMemberNomination((long)sykid, (long)OperationID, userId);
            //List<empName> emplist = new List<empName>();
            //emplist = _objHomeOperatingHead.GetEmpnameList();
            //ViewBag.Empname = new SelectList(emplist, "Empname", "Empname");
            ViewBag.OperatingModel = list;
            AddNominationVM.ADDEDBY = userId;
            AddNominationVM.OperationID = (long)OperationID;
            var nominationData = _objHomeOperatingHead.InsertUpdateMemberNomination(AddNominationVM);
            //if (Request.Form["btnSubmit"] != null)
            if (action == "Submit")
            {
                var s = AddNominationVM.ISSCMEMBERs.ToList();
                foreach (var item in s)
                {
                    var emdetails = _objHomeOperatingHead.GetIsscMemberDetails(item);
                    SendMailTo_ISSCMember(emdetails.EmpName, emdetails.EmpEmail, kiData._SYKIList.Select(x => x.KICODE).FirstOrDefault());
                }
            }
            return RedirectToAction("ISSCMemberOperatingHead");
            // return RedirectToAction("ISSCMemberOperatingHead");
            //return View(nominationData);
        }

        private void SendMailTo_ISSCMember(string Emp, string Emp_Emailid, string ski)
        {
            commanEmail sendMail = new commanEmail();
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
    }
}
