using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using HMSI.ePortal.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter("", 5)]
    public class AutoRemindersNoofDaysMasterController : Controller
    {
        private readonly IAutoRemindersNoofDaysMasterService _objHomeAutoRemindersNoofDaysMaster;
        private readonly ISessionService _sessionService;
        private readonly ILogger<AutoRemindersNoofDaysMasterController> _logger;
        public AutoRemindersNoofDaysMasterController(IAutoRemindersNoofDaysMasterService _IAutoRemindersNoofDaysMasterService, ISessionService objISessionService, ILogger<AutoRemindersNoofDaysMasterController> objlogger)
        {
            _objHomeAutoRemindersNoofDaysMaster = _IAutoRemindersNoofDaysMasterService;
            _sessionService = objISessionService;
            _logger = objlogger;
        }

        public IActionResult Index()
        {
            TempData["PageHead"] = "View/Update Auto Reminders No of Days Master";
            try
            {
                var ViewToList = _objHomeAutoRemindersNoofDaysMaster.GetAutoRemindersNoofDaysMasterData();
                ViewBag.ViewToList = ViewToList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                throw new Exception(ex.Message);
            }

            return View();
        }

        //[HttpPost]
        //public ActionResult ViewMasterData()
        //{
        //    TempData["PageHead"] = "View/Update Auto Reminders No of Days Master";
        //    try
        //    {
        //        var ViewToList = _objHomeAutoRemindersNoofDaysMaster.GetAutoRemindersNoofDaysMasterData();
        //        ViewBag.ViewToList = ViewToList;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.ToString());

        //        throw ex;
        //    }

        //    return View();

        //}
        public ActionResult Edit(int id)
        {
            try
            {
                TempData["PageHead"] = "Auto Reminders No Of Days Master - Update";
                AutoRemindersNoofDaysMaster data = _objHomeAutoRemindersNoofDaysMaster.GetAutoRemindersNoofDaysMasterBySrNo(id);
                return View(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                throw new Exception(ex.Message);
            }
        }

        //[HttpPost]
        //public ActionResult Edit(AutoRemindersNoofDaysMasterViewModel item)
        //{
        //    try
        //    {
        //        var userid = _sessionService.Get<string>("userID");
        //        bool result = _objHomeAutoRemindersNoofDaysMaster.Update_AutoRemindersNoofDaysMaster(item.SRNO, item.no_of_days, userid);
        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.ToString());

        //        throw new Exception(ex.Message);
        //    }
        //}

        [HttpPost]
        public JsonResult UpdateData(string SrNo, String NoofDays)
        {
            try
            {
                var userid = _sessionService.Get<string>("userID");
                bool result = _objHomeAutoRemindersNoofDaysMaster.Update_AutoRemindersNoofDaysMaster(Convert.ToInt32(SrNo), Convert.ToInt32(NoofDays), userid);
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
