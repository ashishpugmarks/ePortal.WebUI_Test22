using ePortal.ViewModels;
//using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using ePortal.Application.Contracts;
using ePortal.Shared.Interface;
using ePortal.Application.Services;
using ePortal.WebUI.Filters;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    public class BirthdayListController : Controller
    {
        private readonly ILogger<BirthdayListController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBirthdayListService _BirthdayListService;
        private readonly ISessionService _sessionService;


        public BirthdayListController(ILogger<BirthdayListController> logger, IHttpContextAccessor httpContextAccessor, IBirthdayListService BirthdayListService, ISessionService sessionService)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _BirthdayListService = BirthdayListService;
            _sessionService = sessionService;
        }

        public IActionResult BirthdayListReport(string eCode)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return View("BirthdayListReport");

        }
        /*[HttpPost]
        public JsonResult GetBirthdayListData()
        {
            List<Root> BirthdayList = new List<Root>();
            try
            {
                int i = 0;
                //List<Root> BirthdayList = new List<Root>();
                BirthdayList = _BirthdayListService.GetBirthdayListData();
                //List<string> list = new List<string>();
                //foreach (var dataitem in POAPowerList)
                //{
                //    list.Add(dataitem.POWERNAME.ToString());
                //}

                System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                string sJSON = oSerializer.Serialize(BirthdayList);
                return Json(sJSON, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }*/

        [HttpPost]
        public IActionResult GetBirthdayListData()
        {
            try
            {
                var birthdayList = _BirthdayListService.GetBirthdayListData();

                // No need to manually serialize to JSON in ASP.NET Core
                return Json(birthdayList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
