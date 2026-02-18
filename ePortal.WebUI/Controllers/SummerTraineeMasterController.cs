using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Web.Mvc;

namespace ePortal.Web.Controllers
{
    [CSPFilter]
    public class SummerTraineeMasterController : Controller
    {

        private readonly IPRService _PrService;
        private readonly ISummerTraineeMasterService _STMService;
        private readonly ISessionService _sessionService;

        public SummerTraineeMasterController(ISummerTraineeMasterService SummerTraineeMasterService, IPRService PrService, ISessionService objSessionService)
        {
            _STMService = SummerTraineeMasterService;
            _PrService = PrService;
            _sessionService = objSessionService;
        }

        // GET: SummerTraineeMaster
        public ActionResult Index()
        {

            //Employee_Details employeeDetails = (Employee_Details)Session["Employee"];
            var employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            List<VM_ADORGLEVEL> _secList = new List<VM_ADORGLEVEL>();
            List<VM_ADORGLEVEL> _depList = new List<VM_ADORGLEVEL>();
            int IsUserOP = 0;
            List<ADORGLEVEL> _opList = _PrService.GetOrgLevelList((long)1);

            ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP");
            return View();
        }

        //public ActionResult DashboardList(long obj)
        //{
        //    List<SummerTraineeViewModel> result = new List<SummerTraineeViewModel>();

        //    result = _STMService.oplist(obj);
        //    return PartialView("_OperationList", result);
        //}

        public ActionResult DashboardList([FromBody] AddUserRequest request)
        {
            List<SummerTraineeViewModel> result = new List<SummerTraineeViewModel>();

            result = _STMService.oplist(request.Obj);
            return PartialView("_OperationList", result);
        }

        //public class DeleteRequest
        //{
        //    public long opID { get; set; }
        //}

        [HttpPost]
        public ActionResult Delete([FromBody] SummerTraineeViewModel request)
        {
            long opID = request.OPID;
            short retval = _STMService.Delete(opID);
            return Json(retval);
        }

        //[HttpPost]
        //public ActionResult Delete([FromBody]long opID)
        //{
        //    short retval = 0;
        //    retval = _STMService.Delete(opID);
        //    return Json(retval);
        //}
        [HttpGet]
        public ActionResult Add()
        {
            List<ADORGLEVEL> _opList = _PrService.GetOrgLevelList((long)1);

            ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP");
            return PartialView("_AddOperation");
        }
        /*public ActionResult AddUser(long obj)
        {

            //short retval;
            //string empcode = (Session["UserId"].ToString());
            var empcode = _sessionService.Get<string>("userID");
            var retval = _STMService.Add(obj,empcode);
            return Json(retval);

        }*/

        //public class AddUserRequest
        //{
        //    public long Obj { get; set; }
        //}

        [HttpPost]
        public ActionResult AddUser([FromBody] AddUserRequest request)
        {
            var empcode = _sessionService.Get<string>("userID");

            if (string.IsNullOrEmpty(empcode))
                return Json(-1); // or handle session missing case

            var retval = _STMService.Add(request.Obj, empcode);
            return Json(retval);
        }

    }
}