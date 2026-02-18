using ePortal.Shared.Interface;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ePortal.WebUI.Controllers
{
    [SessionTimeout]
    [CSPFilter]
    public class PolicyController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly ILogger<PolicyController> _logger;

        public PolicyController(ISessionService sessionService,
                                ILogger<PolicyController> logger)
        {
            _sessionService = sessionService;
            _logger = logger;
        }

        public IActionResult BEPLPolicies()
        {
            var userCode = _sessionService.Get<string>("userID");
            if (string.IsNullOrEmpty(userCode))
                return RedirectToAction("Login", "Account");

            return View();
        }

        public IActionResult StaffandLAPolicy()
        {
            var userCode = _sessionService.Get<string>("userID");
            if (string.IsNullOrEmpty(userCode))
                return RedirectToAction("Login", "Account");

            return View();
        }

        public IActionResult pdfview(string f)
        {
            var userCode = _sessionService.Get<string>("userID");
            if (string.IsNullOrEmpty(userCode))
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrEmpty(f))
            {
                return NotFound();
            }

            return View(model: f);
        }
    }
}
