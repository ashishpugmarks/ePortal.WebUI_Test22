using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ePortal.WebUI.Controllers
{
    public class AppErrorController : Controller
    {
        // GET: AppError
        public ActionResult ErrorPage()
        {
            return View("~/Views/Shared/Error.cshtml");
        }

        public ActionResult HttpError404()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
        public ActionResult HttpError500()
        {
            return View("~/Views/Shared/Error.cshtml");
        }

        public ActionResult General()
        {
            return View("~/Views/Shared/Error.cshtml");
        }

      
        //[Route("Error/404")]
        //public IActionResult HttpError404() => View();

        //[Route("Error/500")]
        //public IActionResult HttpError500() => View();

        //[Route("Error/General")]
        //public IActionResult General()
        //{
        //    var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        //    var exception = feature?.Error;

        //    // You can log this using Serilog/NLog/ElmahCore or write to file
        //    return View("General", exception);
        //}
    }
}
