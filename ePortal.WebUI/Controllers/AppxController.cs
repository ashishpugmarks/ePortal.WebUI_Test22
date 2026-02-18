using System.Data;
using System.Text.RegularExpressions;
using ePortal.Persistence;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Admin.Services;
using ePortal.Persistence.Interface;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.ViewModels.APPX;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    [SessionTimeout]
    public class AppxController : Controller
    {
        private readonly IDepartment objDepartment;
        private readonly ISessionService _sessionService;
        private readonly ILogger<AppxController> _logger;
        private readonly IConfiguration _settings;
        private readonly ISearchEmp _objDetail;
        private readonly ICommonFunctions _commFun;
        private readonly IPassword _objPsswd;
        private readonly ILogin objLog;

        public AppxController(IDepartment _objDepartment, ISessionService sessionService, ILogger<AppxController> logger, IConfiguration settings, ISearchEmp objDetail, ICommonFunctions commFun, IPassword objPsswd, ILogin _objLog)
        {
            objDepartment = _objDepartment;
            _sessionService = sessionService;
            _logger = logger;
            _settings = settings;
            _objDetail = objDetail;
            _commFun = commFun;
            _objPsswd = objPsswd;
            objLog = _objLog;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult HMSIOrganisation()
        {

            DataTable dt = objDepartment.GetOrganisationChart();

            var items = new List<OrgChartItem>();

            foreach (DataRow row in dt.Rows)
            {
                var status = row["STATUS"]?.ToString() ?? "";
                if (!status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                    continue;

                items.Add(new OrgChartItem
                {
                    Description = row["DESCRIPTION"]?.ToString() ?? "",
                    FileName = row["FILENAME"]?.ToString() ?? "",
                    Status = status
                });
            }
            return View(items);
        }

       
        [HttpGet]
        public IActionResult ApprenticeEmployeeSearch()
        {
            var model = new AppEmpSearchViewModel
            {
                Operations = _objDetail.get_AllOperation().AsSelectList_DS("OPERATIONID", "OPERATION", true, "Select Operation", "0"),
                Divisions = _objDetail.get_AllDiv(0).AsSelectList_DS("divisionid", "division", true, "Select Division", "0"), //new List<SelectListItem>(),
                Departments = _objDetail.get_AllDept(0, 0).AsSelectList_DS("departmentid", "department", true, "Select Department", "0"), //new List<SelectListItem>(),
                Sections = _objDetail.get_AllSection(0, 0, 0).AsSelectList_DS("sectionid", "section", true, "Select Section", "0"), //new List<SelectListItem>(),
            };

            return View(model);
        }


        [HttpPost]
        public IActionResult ApprenticeEmployeeSearch(AppEmpSearchViewModel model)
        {
            string strOperationID = string.Empty;
            string strDivisionID = string.Empty;
            string strDepartmentID = string.Empty;
            string strSectionID = string.Empty;
            string strEmpCode = string.Empty;
            string strBloodGroup = string.Empty;
            string strFirstName = string.Empty;
            string strLastName = string.Empty;
            string strEmailID = string.Empty;
            string strDesignation = string.Empty;
            //string strFunDesignation = string.Empty;

            string strAssociateLowestLevel = string.Empty;

            strOperationID = model.OperationId.ToString();
            strDivisionID = model.DivisionId.ToString();
            strDepartmentID = model.DepartmentId.ToString();
            strSectionID = model.SectionId.ToString(); 

            strEmpCode = model.EmpCode==null?"":Convert.ToString(model.EmpCode).Trim();
            //strBloodGroup = cboBloodGroup.SelectedValue;
            strFirstName = model.FirstName==null?"": model.FirstName.ToString().Trim(); 
            strLastName = model.LastName==null?"":model.LastName.ToString().Trim(); 
            //strEmailID = txtemail.Text.Trim();
            //strDesignation = cbodesignation.SelectedValue;
            //strFunDesignation = cbofundesignation.SelectedValue;

            if (strOperationID == "" && strDivisionID == "" && strDepartmentID == "" && strSectionID == "")
            {
                //Get All Data irrespective of any any levels
                strAssociateLowestLevel = "";
            }
            else if (strSectionID != "")
            {
                strAssociateLowestLevel = strSectionID;
            }
            else if (strDepartmentID != "")
            {
                strAssociateLowestLevel = strDepartmentID;
            }
            else if (strDivisionID != "")
            {
                strAssociateLowestLevel = strDivisionID;
            }
            else if (strOperationID != "")
            {
                strAssociateLowestLevel = strOperationID;
            }

            DataTable SummaryList = new DataTable();

            //GET DATA FROM DATA ACCESS LAYER
            SummaryList = _commFun.GetApprenticeAssociateDetails("", strOperationID, strDivisionID, strDepartmentID, strSectionID, strEmpCode, strFirstName, strLastName /*strDesignation, strEmailID, strBloodGroup*/);

            

           // //dtDetails = objCom.GetApprenticeAssociateDetails("", strOperationID, strDivisionID, strDepartmentID, strSectionID, strEmpCode, strFirstName, strLastName /*strDesignation, strEmailID, strBloodGroup*/);

           // DataTable SummaryList = _commFun.GetApprenticeAssociateDetails(
           //    "",
           //    model.OperationId.ToString(),
           //    model.DivisionId.ToString(),
           //    model.DepartmentId.ToString(),
           //    model.SectionId.ToString(),               
           //    model.EmpCode.ToString(),
           //    model.FirstName.ToString(),
           //    model.LastName.ToString()
           //);
           

            model.SummaryList = SummaryList.AsEnumerable().Select(r => new AppEmpSummaryItem
            {
                Ecode = r["empcode"].ToString(),
                Name = r["name"].ToString(),
                Operation = r["DeptDivOp"].ToString(),
                Designation = r["designation"].ToString(),
                DOB = r["DOB"].ToString(),
                DOJ = r["DOJ"].ToString(),
                Location = r["SYSITE"].ToString()
            }).ToList();

          
            //var data = _infoSecService.GetSurveyData(filter);
            return PartialView("_searchAppEmpSearch", model);
        }

        [HttpGet]
        public JsonResult GetDivisions(int operationId)
        {
            //oDs = oDiv.GetFilterDivision(operationID);
            var divisions = _objDetail.get_AllDiv(operationId).AsSelectList_DS("divisionid", "division", true, "Select Division", "0");
            return Json(divisions);
        }

        [HttpGet]
        public JsonResult GetDepartments(int operationID, int divisionId)
        {
            //oDs = oDept.GetFilterDepartment(OperationID, divisionID);
            
            var departments = _objDetail.get_AllDept(operationID, divisionId).AsSelectList_DS("departmentid", "department", true, "Select Department", "0");
            return Json(departments);
        }

        [HttpGet]
        public JsonResult GetSections(int operationID, int divisionId, int departmentId)
        {
            //objPms.GetSect(strOperationId, strDivisionId, strDepartmentId);
            
            var sections = _objDetail.get_AllSection(operationID, divisionId, departmentId).AsSelectList_DS("sectionid", "section", true, "Select Section", "0");
            return Json(sections);
        }
        //StatusText
        //Email
        //OldPassword
        //chklogin

        public IActionResult ChangePassword()
        {
            ChangePasswordNintyDaysViewModel changeModel = new ChangePasswordNintyDaysViewModel();


            //Login objLog = new Login();
            int chklogin = objLog.check_LoginBefore90Days(_sessionService.Get<string>("userID"), _sessionService.Get<string>("usertype"));
            changeModel.chklogin = chklogin;

            if (chklogin == 1)
            {
                //errorpanel.Style.Add(HtmlTextWriterStyle.Display, "inline");
                //status.Text = "It is mandatory to change the password after first login.";

                changeModel.StatusText= "It is mandatory to change the password after first login.";
            }
            else
            {
                if (chklogin == 2)
                {
                    //--90/30 days password policy 
                    int passExpiryDays = objLog.getPassExpiryDays(_sessionService.Get<string>("userID"));

                    //errorpanel.Style.Add(HtmlTextWriterStyle.Display, "inline");
                    //status.Text = "It is mandatory to change the password after " + passExpiryDays.ToString() + " days."; //--90/30 days password policy 

                    changeModel.StatusText = "It is mandatory to change the password after " + passExpiryDays.ToString() + " days."; //--90/30 days password policy 
                }
                else
                {
                    changeModel.chklogin = 0;
                }
            }          

            //// Get prev pass for verification
            ////Password objEmail = new Password();
            //DataSet objDs = _objPsswd.GetEmail(_sessionService.Get<string>("userID"), _sessionService.Get<string>("usertype"));
            //for (int i = 0; i < objDs.Tables[0].Rows.Count; i++)
            //{
            //    //txtEmail.Text = objDs.Tables[0].Rows[i][0].ToString();
            //    //txtoldpassword.Value = objDs.Tables[0].Rows[i][1].ToString();

            //    changeModel.Email = objDs.Tables[0].Rows[i][0].ToString();
            //    changeModel.OldPassword = objDs.Tables[0].Rows[i][1].ToString();
            //}

            return View(changeModel);
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

                if (strtxtNewPwd != confirmNewPwd)
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
                    
                    DataSet objDs = _objPsswd.GetEmail(HttpContext.Session.GetString("userID").ToString(), _sessionService.Get<string>("usertype"));
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
                    else if (strtxtNewPwd.Contains(_sessionService.Get<string>("userID")) == true)
                    {
                        retVal = 3;
                        msg = "Password should not contain your user id or user name.";
                    }
                    else if (strtxtNewPwd.Contains(_sessionService.Get<string>("userName")) == true)
                    {
                        retVal = 3;
                        msg = "Password should not contain your user id or user name.";
                    }
                    else
                    {         

                        //Password objChangePwd = new Password();
                        strtxtNewPwd = Encryption.EncodePasswordToBase64(strtxtNewPwd);
                        string result = _objPsswd.updatepassword(_sessionService.Get<string>("userID"), strtxtNewPwd, _sessionService.Get<string>("usertype"));
                        string[] strmsg = result.Split(new Char[] { '#' });
                        string errResult = Convert.ToString(strmsg[0]);
                        string errMsg = Convert.ToString(strmsg[1]);
                        if (errResult == "1")
                        {
                            int chklogin = objLog.check_LoginBefore90Days(_sessionService.Get<string>("userID"), _sessionService.Get<string>("usertype"));

                            if (chklogin == 1)
                            {
                                objLog.update_FirstLogin(_sessionService.Get<string>("userID"), Convert.ToInt32(_sessionService.Get<string>("usertype")));

                                // Response.Redirect("~/Home/Home", false);

                                retVal = 4; //redirect user to home page
                                //return Redirect(_settings["Switch_New_Old_New:OLD_APP_URL"].ToString() + "/Home/Home");
                            }
                            else
                            {
                                retVal = 1;
                                msg = "Your Password has been changed.";
                                //change_password.Visible = false;
                            }
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

        [HttpGet]
        public IActionResult EmployeeSearch()
        {
            string strValue = string.Empty;
            strValue = "19"; //19-Trainee is excluded

            var model = new EmpSearchViewModel
            {
                Operations = _objDetail.get_AllOperation().AsSelectList_DS("OPERATIONID", "OPERATION", true, "Select Operation", "0"),
                Divisions = _objDetail.get_AllDiv(0).AsSelectList_DS("divisionid", "division", true, "Select Division", "0"), //new List<SelectListItem>(),
                Departments = _objDetail.get_AllDept(0, 0).AsSelectList_DS("departmentid", "department", true, "Select Department", "0"), //new List<SelectListItem>(),
                Sections = _objDetail.get_AllSection(0, 0, 0).AsSelectList_DS("sectionid", "section", true, "Select Section", "0"), //new List<SelectListItem>(),
                Desigantions = _commFun.GetDesignation(strValue).AsSelectList("DESIGNATIONID", "DESIGNATION", true, "Select Designation", "0"),
                FunDesigantions= _commFun.GetFuncDesig().AsSelectList("ADFUNCTIONALDESIGNATIONID", "DESCRIP", true, "Select Fnc. Designation", "0"),
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EmployeeSearch(EmpSearchViewModel model)
        {
            string strOperationID = string.Empty;
            string strDivisionID = string.Empty;
            string strDepartmentID = string.Empty;
            string strSectionID = string.Empty;
            string strEmpCode = string.Empty;
            string strBloodGroup = string.Empty;
            string strFirstName = string.Empty;
            string strLastName = string.Empty;
            string strEmailID = string.Empty;
            string strDesignation = string.Empty;
            string strFunDesignation = string.Empty;

            string strAssociateLowestLevel = string.Empty;

            strOperationID = model.OperationId==0?"":model.OperationId.ToString();
            strDivisionID = model.DivisionId == 0 ? "":model.DivisionId.ToString();
            strDepartmentID = model.DepartmentId == 0 ? "":model.DepartmentId.ToString();
            strSectionID = model.SectionId == 0 ? "":model.SectionId.ToString();

            strEmpCode = model.EmpCode == null ? "" : Convert.ToString(model.EmpCode).Trim();
            strBloodGroup = (model.BloodGroup==null || model.BloodGroup == "") ?"" : Convert.ToString(model.BloodGroup).Trim();
            strFirstName = model.FirstName == null ? "" : model.FirstName.ToString().Trim();
            strLastName = model.LastName == null ? "" : model.LastName.ToString().Trim();
            strEmailID = model.EmailId == null ? "" : model.EmailId.ToString().Trim();
            strDesignation = model.DesigantionId == 0 ? "":model.DesigantionId.ToString();
            strFunDesignation = model.FunDesigantionId == 0 ? "":model.FunDesigantionId.ToString();

            if (strOperationID == "" && strDivisionID == "" && strDepartmentID == "" && strSectionID == "")
            {
                //Get All Data irrespective of any any levels
                strAssociateLowestLevel = "";
            }
            else if (strSectionID != "")
            {
                strAssociateLowestLevel = strSectionID;
            }
            else if (strDepartmentID != "")
            {
                strAssociateLowestLevel = strDepartmentID;
            }
            else if (strDivisionID != "")
            {
                strAssociateLowestLevel = strDivisionID;
            }
            else if (strOperationID != "")
            {
                strAssociateLowestLevel = strOperationID;
            }

            DataTable SummaryList = new DataTable();

            //GET DATA FROM DATA ACCESS LAYER
            SummaryList = _commFun.GetAssociateDetails("", strOperationID, strDivisionID, strDepartmentID, strSectionID, strEmpCode, strFirstName, strLastName, strDesignation, strEmailID, strBloodGroup, strFunDesignation);


            model.SummaryList = SummaryList.AsEnumerable().Select(r => new EmpSummaryItem
            {
                Ecode = r["empcode"].ToString(),
                Name = r["name"].ToString(),
                Operation = r["DeptDivOp"].ToString(),
                Designation = r["designation"].ToString(),
                Mobile = r["mobile"].ToString(),
                EmailId = r["EMAILID"].ToString(),
                BloodGroup = r["bloodgroup"].ToString(),
                FunDesignation = r["FUNCTIONALDESIGNATION"].ToString()
            }).ToList();


            //var data = _infoSecService.GetSurveyData(filter);
            return PartialView("_searchAdvanceEmp", model);
        }
    }
}
