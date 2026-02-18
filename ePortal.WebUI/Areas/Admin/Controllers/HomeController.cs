using ePortal.Application.Contracts;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.WebUI.Controllers;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.RegularExpressions;
using System.Data;
using ePortal.Shared;

namespace ePortal.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[CSPFilter]
    //[SessionTimeout]
    public class HomeController : AdminBaseController
    {

        private readonly ILogin _loginService;   // same as old
        private readonly ILogger<HomeController> _logger;
        private readonly IPassword _objEmail;

        public HomeController(IPassword objEmail, IEmpLoginService empLoginService,ILogin loginService, ILogger<HomeController> logger, IConfiguration settings, ISessionService sessionService, IDataManagement _oDataMgmt, IConnectionString connStr, IWebHostEnvironment env) : base(_oDataMgmt, connStr, logger, settings, sessionService, env)
        {
            _loginService = loginService;
            _logger = logger;           
            _objEmail = objEmail;
        }

        public IActionResult Index()
        {
            var model = new HomeViewModel();

            var userId = HttpContext.Session.GetString("userID");
            if (!string.IsNullOrEmpty(userId))
            {
                var result = _loginService.check_LoginExpiry(userId, out string expiryDate, out int _);

                if (result == 1)
                {
                    model.ShowError = true;
                    model.ErrorMessage =
                        $"Your login ID will expire on {expiryDate}. " +
                        "For renewal, kindly contact your supervisor to raise request in Employee Portal " +
                        "(Service >> Self Service process >> User Id Management).";
                }
            }

            return View(model);
        }      

        public IActionResult ChangePassword()
        {
            return View();
        }

        public static bool IsPasswordStrong(string password)
        {
            return Regex.IsMatch(password, @"^(?=.{10,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\s).*$");
        }

        [HttpPost]
        public ActionResult ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            short retVal = 0;
            string msg = "";
            string txtoldpassword = string.Empty;


            if (!ModelState.IsValid)
            {
                return Json(new { val = -1, msg = "Please provide all the manadatory details." });
            }


            try
            {    

                string strtxtNewPwd = model.NewPassword;
                string oldpwd = model.OldPassword;
                string confirmNewPwd = model.NewPassword;

                if (strtxtNewPwd == oldpwd)
                {
                    return Json(new { val = 4, msg = "New Password and Old Password should not be same." });
                }

                if (strtxtNewPwd!=confirmNewPwd)
                {
                    return Json(new { val = 4, msg = "New password and confirm password you entered are not same." });
                }                


                bool containsAtLeastOneSpecialChar = strtxtNewPwd.Any(ch => !Char.IsLetterOrDigit(ch));
                if (IsPasswordStrong(strtxtNewPwd) == false || containsAtLeastOneSpecialChar == false)
                {
                    retVal = 2;
                    msg = "Password must be atleast 10 characters long and should be a mix of at least One Uppercase letter, One Lowercase letter, One Special character and One Numeral."; //10-Char Pass
                }
                else
                {
                    DataSet objDs = _objEmail.GetEmail(HttpContext.Session.GetString("userID").ToString(), "2");
                    for (int i = 0; i < objDs.Tables[0].Rows.Count; i++)
                    {
                        //txtEmail.Text = objDs.Tables[0].Rows[i][0].ToString();
                        txtoldpassword = objDs.Tables[0].Rows[i][1].ToString();
                    }



                    if (txtoldpassword != Encryption.EncodePasswordToBase64(oldpwd))
                    {
                        //errorpanel.Style.Add(HtmlTextWriterStyle.Display, "inline");
                        retVal = 3;
                        msg = "You have provided an incorrect Old Password.";
                    }
                    else
                    {
                        //Password objChangePwd = new Password();
                        strtxtNewPwd = Encryption.EncodePasswordToBase64(strtxtNewPwd);
                        string result = _objEmail.updatepassword(HttpContext.Session.GetString("userID").ToString(), strtxtNewPwd, "2");
                        string[] strmsg = result.Split(new Char[] { '#' });
                        string errResult = Convert.ToString(strmsg[0]);
                        string errMsg = Convert.ToString(strmsg[1]);
                        if (errResult == "1")
                        {
                            retVal = 1;
                            msg = "Your Password has been changed.";
                            //change_password.Visible = false;
                        }
                        else
                        {
                            retVal = 0;
                            msg = errMsg;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = -1;                
                _logger.LogError("Error in ChangePassword." + ex.Message.ToString());
            }
            return Json(new { val = retVal, msg = msg });
        }
    }
}
