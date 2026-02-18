using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ePortal.WebUI.Filters
{
    public class SessionVerificationAttribute : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Check if the session has expired (if the user session is not available)
            var userSession = context.HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userSession))
            {
                // If session expired, redirect to logout or login
                context.Result = new RedirectToActionResult("Logout", "Account", null);
            }
        }
    }
}
