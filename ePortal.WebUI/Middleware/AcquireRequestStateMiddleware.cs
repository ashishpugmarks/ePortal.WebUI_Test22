using System.Text.Json;
using ePortal.Application.Contracts;
using ePortal.Application.Services;
using ePortal.Shared.Interface;
using ePortal.ViewModels;

namespace ePortal.WebUI.Middleware
{
    public class AcquireRequestStateMiddleware
    {
        private readonly RequestDelegate _next;

        public AcquireRequestStateMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IHomePageService _homePageService, ISessionService _sessionService)
        {
            var session = context.Session;
            var request = context.Request;
            var response = context.Response;
            var path = request.Path.Value ?? string.Empty;

            if (session.GetString("userID") != null && request.Headers["Accept"].ToString().Contains("text/html"))
            {
                string userId = session.GetString("userID")!;
                string userType = session.GetString("usertype") ?? "0";
                string isSSOLogin = session.GetString("ISSSOLOGIN") ?? "0";

                await _homePageService.CreateUserMenuLog(userId, path);

                //--------------------------------------  START ----------------------------------------------
                // Uncomment this line when the upgraded Home page replaces the legacy version in the future..
                //--------------------------------------------------------------------------------------------

                //if (!path.Equals("/Login/OTPAuthentication", StringComparison.OrdinalIgnoreCase) &&
                //    !path.Equals("/Home/Home", StringComparison.OrdinalIgnoreCase))
                //{
                //    string redirectUrl = await _homePageService.GetMandatorytoFilledPage(
                //        Convert.ToDecimal(userId),
                //        Convert.ToDecimal(userType),
                //        1,
                //        Convert.ToDecimal(isSSOLogin));

                //    if (!string.IsNullOrEmpty(redirectUrl) && !(redirectUrl).Equals(context.Request.Path, StringComparison.OrdinalIgnoreCase))
                //    {                        
                //        context.Response.Redirect(redirectUrl); 
                //        return;
                //    }
                //}

                //--------------------------------------  END ----------------------------------------------

                string retVal = await _homePageService.ISAUTH_ACCESS(userId, path);
                if (retVal == "0")
                {
                    context.Response.Redirect("/Home/Home");
                    return;
                }

                if (retVal == "2")
                {

                    MenuViewModel menu = new MenuViewModel();
                    menu.SubMenuItems = _sessionService.Get<List<MenuViewModel>>("MENU");


                    //var menuJson = session.GetString("MENU");
                    if (menu.SubMenuItems != null && menu.SubMenuItems.Count > 0)
                    {
                        //menu = JsonSerializer.Deserialize<MenuViewModel>(menuJson);
                        if (!menu.SubMenuItems.Any(m => m.URL == path))
                        {
                            //context.Response.Redirect("~/Home/Home");
                            context.Response.Redirect("/Home/Home");
                            return;
                        }
                    }
                }
            }

            await _next(context);
        }
    }

}
