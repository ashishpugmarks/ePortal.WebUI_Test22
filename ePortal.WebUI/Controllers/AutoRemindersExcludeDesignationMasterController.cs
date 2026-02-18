using ePortal.Application.Contracts;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter("", 12)]
    public class AutoRemindersExcludeDesignationMasterController : Controller
    {

        private readonly IAutoRemindersExcludeDesignationMasterService _objHomeAutoRemindersExcludeDesignationMaster;
        private readonly ISessionService _sessionService;
        private readonly ILogger<AutoRemindersExcludeDesignationMasterController> _logger;
        public AutoRemindersExcludeDesignationMasterController(IAutoRemindersExcludeDesignationMasterService _IAutoRemindersExcludeDesignationMasterService, ISessionService objISessionService, ILogger<AutoRemindersExcludeDesignationMasterController> objlogger)
        {
            _objHomeAutoRemindersExcludeDesignationMaster = _IAutoRemindersExcludeDesignationMasterService;
            _sessionService = objISessionService;
            _logger = objlogger;
        }

        public IActionResult Index()
        {
            TempData["PageHead"] = "View/Update Auto Reminders Exclude Designation Master";
            try
            {
                var userid = _sessionService.Get<string>("userID");
                var ViewToList = _objHomeAutoRemindersExcludeDesignationMaster.GetAutoRemindersExcludeDesignationMasterData(userid);
                ViewBag.ViewToList = ViewToList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                throw new Exception(ex.Message);
            }
            return View();
        }


        [HttpPost]
        public JsonResult BulkUpdate(string selectIDs)
        {
            try
            {
                var userid = _sessionService.Get<string>("userID");

                //string[] ids = selectIDs.Split(',');

                //if (ids.Length > 0)
                //{
                //    foreach (var item in ids)
                //    {
                //        bool result = _objHomeAutoRemindersExcludeDesignationMaster.Update_AutoRemindersExcludeDesignationMaster(Convert.ToInt32(item), userid);
                //    }
                //}

                bool result = _objHomeAutoRemindersExcludeDesignationMaster.Update_AutoRemindersExcludeDesignationMaster(Convert.ToInt32(selectIDs), userid);

                if (result)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, error = "Data Update Issue" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                return new JsonResult(new { success = false, error = ex.Message });
            }
        }
    }
}
