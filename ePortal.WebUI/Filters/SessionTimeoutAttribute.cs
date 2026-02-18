using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ePortal.WebUI.Filters
{
    public class SessionTimeoutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //var httpContext = context.HttpContext;

            //// Get controller name
            //var controllerName = context.RouteData.Values["controller"]?.ToString();

            //// Skip check for Login controller
            //if (!string.Equals(controllerName, "Login", StringComparison.OrdinalIgnoreCase))
            //{
            //    var session = httpContext.Session;
            //    var userId = session.GetString("userID");

            //    if (string.IsNullOrEmpty(userId))
            //    {
            //        context.Result = new RedirectResult("~/Login");
            //        return;
            //    }
            //}

            var path = context.HttpContext.Request.Path.Value.ToLower();
            var rawPath = context.HttpContext.Features.Get<IHttpRequestFeature>()?.RawTarget;

            // Allow login or public access
            if (path.Contains("login") || path == "/" || path.Contains("/tokenbridge/handlefromnewapp") || path.Contains("/tokenbridge/oldlogin") || path.Contains("/piomrequest/regeneratedoc")) // //Changed By TTL against CR6695 as on 29 - 07 - 2025
            {
                base.OnActionExecuting(context);
                return;
            }

            var session = context.HttpContext.Session;
            if (session.GetString("userID") == null)
            {
                // Redirect to login if session expired
                context.Result = new RedirectToRouteResult(
                    //new RouteValueDictionary(new { controller = "Login", action = "Index" }));
                    new RouteValueDictionary(new { controller = "Home", action = "RedirectToOldAppLogin" })); //Changed By TTL against CR6695 as on 29 - 07 - 2025
            }

            base.OnActionExecuting(context);
        }
    }
}