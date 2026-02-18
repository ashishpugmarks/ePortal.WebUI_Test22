using ePortal.Persistence;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Admin.Services;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.ViewModels.APPX.Utility;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Cmp;
using System.Data;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    [SessionTimeout]
    public class UtilityController : Controller
    {
        public string filePath = serverpath.getFileUploadPath() + "\\Utility";
        private readonly IUtilityDesk _UtilityDeskService;
        private readonly ICommonFunctions _CommonFunctions;
        private readonly IPMS_DAL _iPMS_DAL;
        private readonly ISessionService _sessionService;
        public UtilityController(IUtilityDesk UtilityDeskService, ICommonFunctions CommonFunctions, ISessionService sessionService, IPMS_DAL iPMS_DAL)
        {
            _UtilityDeskService = UtilityDeskService;
            _CommonFunctions = CommonFunctions;
            _sessionService = sessionService;
            _iPMS_DAL = iPMS_DAL;
        }

        public IActionResult UtilityRequest()
        {
            Employee_Details LoginEmpDetails = _sessionService.Get<Employee_Details>("Employee");
            long userId = Convert.ToInt64(_sessionService.Get<string>("userID"));
            UtilityRequestViewModel contactInfo = _UtilityDeskService.GetContactInfo(userId);
            UtilityRequestViewModel approvalAuthority = _UtilityDeskService.GetSelectedApprovalAutority(userId);

            UtilityRequestViewModel model = new UtilityRequestViewModel()
            {
                Sites = _CommonFunctions.GetSITE().AsSelectList(valueField: "sysiteid", textField: "SITE", true, "-Select Site-", "0"),
                ServiceCatalogs = _UtilityDeskService.get_ServiceCatalog().AsSelectList_DS(valueField: "servicecatalogid", textField: "servicecatalog", true, "-Select Service Catalog-", "0"),
                ApprovalAuthorities = _UtilityDeskService.GetApprovalAuthority(strempcode: _sessionService.Get<string>("userID")).AsSelectList_DS(valueField: "CODE", textField: "EMPNAME", true, "- Select Approval Authority -", "0"),
                Locations = _CommonFunctions.GetLocation(strsite: LoginEmpDetails.Site_Id).AsSelectList(valueField: "LOCATIONVALUE", textField: "LOCATION", true, "-Select-", "-1"),
                SiteId = LoginEmpDetails.Site_Id,
                ExtensionNo = contactInfo.ExtensionNo,
                PhoneNo = contactInfo.PhoneNo,
                ApprovalAuthorityCode = approvalAuthority.ApprovalAuthorityCode
            };
            return View(model);
        }
        
        [HttpPost]
        public IActionResult UtilityRequest(UtilityRequestViewModel model)
        {
            try
            {
                string strEmpCode = string.Empty;
                string strExtNo = string.Empty;
                string strContactNo = string.Empty;
                string strDirectNo = string.Empty;
                string strLocation = string.Empty;
                string strLine = string.Empty;
                string strStation = string.Empty;
                string strServiceCatalog = string.Empty;
                string strClassification = string.Empty;
                string strScope = string.Empty;
                string strTitle = string.Empty;
                string strbudget = string.Empty;
                string strFilename;
                string strName = "";
                string strMessage;
                string strSectMgrEmail = string.Empty;
                string strsctmgrName = string.Empty;
                string strSectMgrCode = string.Empty;
                string strsctmgrname = string.Empty;
                string strdptmgrcode = string.Empty;
                string strappauth = string.Empty;
                string strSite = string.Empty;
                string responseMessage = string.Empty;

                Employee_Details LoginEmpDetails = _sessionService.Get<Employee_Details>("Employee");
                long userId = Convert.ToInt64(_sessionService.Get<string>("userID"));
                string userName = _sessionService.Get<string>("userName");

                strEmpCode = userId.ToString();
                strExtNo = model.ExtensionNo;
                strContactNo = model.PhoneNo;
                strDirectNo = model.Landline;
                strLocation = (model.Location.Split('~')[0]).ToString();
                strLine = model.Line;
                strStation = model.Station;
                strServiceCatalog = model.ServiceCatalogId;
                strClassification = model.ClassificationId;
                strScope = model.ScopeOfWork;
                strbudget = model.BudgetHead;
                strTitle = model.WorkTitle;                
                strFilename = model.Attachment?.FileName;
                strSite = model.SiteId;

                string test = DateTime.Now.Day.ToString() + "" + DateTime.Now.Month.ToString() + "" + DateTime.Now.Year.ToString() + "" + DateTime.Now.Hour.ToString() + "" + DateTime.Now.Minute.ToString() + "" + DateTime.Now.Second.ToString();

                if (Path.GetFileName(strFilename) != "" && Path.GetFileName(strFilename) != null)
                {
                    strName = test + "" + Path.GetFileName(strFilename);
                }
                string path = serverpath.getFileUploadPath();
                strMessage = uploadFile(model, strName, path);
                strappauth = model.ApprovalAuthorityCode;

                // submit utility request
                string strResult = _UtilityDeskService.SubmitUtilityReq(strEmpCode, strExtNo, strContactNo, strDirectNo, strLocation, strLine, strStation, strServiceCatalog, strClassification, strbudget, strTitle, strScope, strName, strappauth, strSite);
                string[] strStatus = strResult.Split(new Char[] { '#' });
                string errResult = Convert.ToString(strStatus[0]);
                int intRequestNo = Convert.ToInt32(strStatus[1]);
                string errMsg = Convert.ToString(strStatus[2]);

                if (errResult == "0" || errResult == "2")
                {
                    responseMessage = errMsg;
                }

                if (errResult == "1")
                {
                    string strSubject = string.Empty;
                    string strBody = string.Empty;
                    Employee_Details objEmpDetails = _UtilityDeskService.GetEmployeeDetails(Convert.ToInt64(strappauth));
                    if (objEmpDetails != null)
                    {
                        strSectMgrEmail = objEmpDetails.EMail_Id;
                        strsctmgrname = objEmpDetails.Employee_Name;
                    }

                    // send mail
                    if (strSectMgrEmail != "")
                    {
                        commanEmail sendMail = new commanEmail();
                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        //sendMail.MailTo = strSectMgrEmail;

                        if (serverpath.isTestServer())
                            sendMail.MailTo = serverpath.getTestEMail();
                        else
                            sendMail.MailTo = strSectMgrEmail;

                        strSubject = "Utility Request from - " + userName + " [ " + userId.ToString() + " ]";
                        strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                                         "<tr><td height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                                         "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                         "</td></tr>" +
                                         "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                                         "<b>Utility Request </b></td></tr><tr height=150><td colspan=2>" +
                                         "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                         "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                         "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                         "<tr><td colspan=3>&nbsp;</td></tr><tr><td valign=top colspan=2><p><b>Dear " + strsctmgrname + " San,</b><br />" +
                                         "<br />" + userName + " [ " + userId.ToString() + " ] " +
                                         "has given a Utility request ID - " + intRequestNo + ", the details are as follows:<br />" +
                                         "<br />" +
                                         "<tr><td width=125 height=23 valign=top>Service Catalog:</td><td width=389 valign=top>" + model.ServiceCatalogDescription + "</td>" +
                                         "</tr><tr><td width=125 height=23 valign=top>Classification:</td><td width=389 valign=top>" + model.ClassificationDescription + "</td>" +
                                         "</tr><tr><td width=125 height=23 valign=top>Budget Head:</td><td width=389 valign=top>" + model.BudgetHead + "</td>" +
                                         "</tr><tr>" +
                                         "<td width=125 valign=top>Work Title</td><td width=389 valign=top>" + model.WorkTitle + "</td></tr>" +
                                         "<tr><td valign=top colspan=2>Please login Employee Portal for further action.</td></tr></p>" +
                                         "</td><td width=20>&nbsp;</td></tr><tr valign=bottom> " +
                                         "<td colspan=2><b>Best Regards</b><br /> Team Portal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                                         "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                         "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images//Table_layout_06.gif border=0 /></td> " +
                                         "</tr></table> ";

                        sendMail.MailSubject = strSubject;
                        sendMail.MailBody = strBody;
                        try
                        {
                            bool status1 = sendMail.Send();
                        }
                        catch (Exception ex)
                        {
                            responseMessage += @"\n" + ex.Message;
                        }
                    }
                    var result = new
                    {
                        status = 1,
                        message = "Request Created Successfully",
                        exception = ""
                    };
                    return Ok(result);
                }
                else
                {
                    var result = new
                    {
                        status = 0,
                        message = responseMessage,
                        exception = ""
                    };
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                var result = new
                {
                    status = -1,
                    message = "Something went wrong",
                    exception = ex.Message
                };
                return BadRequest(result);
            }
        }

        public IActionResult EditUtilityReq(long id)
        {
            Employee_Details LoginEmpDetails = _sessionService.Get<Employee_Details>("Employee");
            long userId = Convert.ToInt64(_sessionService.Get<string>("userID"));

            UtilityRequestViewModel contactInfo = _UtilityDeskService.GetContactInfo(userId);
            UtilityRequestViewModel approvalAuthority = _UtilityDeskService.GetSelectedApprovalAutority(userId);

            //Get Utility Request Details
            System.Data.DataTable dt = _UtilityDeskService.GetUtilityRequestDetail(id);

            UtilityRequestViewModel model = new UtilityRequestViewModel()
            {
                Sites = _CommonFunctions.GetSITE().AsSelectList(valueField: "sysiteid", textField: "SITE", true, "-Select Site-", "0"),
                ServiceCatalogs = _UtilityDeskService.get_ServiceCatalog().AsSelectList_DS(valueField: "servicecatalogid", textField: "servicecatalog", true, "-Select Service Catalog-", "0"),
                ApprovalAuthorities = _UtilityDeskService.GetApprovalAuthority(strempcode: _sessionService.Get<string>("userID")).AsSelectList_DS(valueField: "CODE", textField: "EMPNAME", true, "- Select Approval Authority -", "0"),
                Locations = _CommonFunctions.GetLocation(strsite: dt.Rows[0]["SYSITEID"].ToString()).AsSelectList(valueField: "SYLOCATIONID", textField: "LOCATION", true, "-Select-", "-1"),
                SiteId = LoginEmpDetails.Site_Id,
                ExtensionNo = dt.Rows[0]["EXTN"].ToString(),
                PhoneNo = dt.Rows[0]["CONTACT"].ToString(),
                Landline = dt.Rows[0]["DIRECTNO"].ToString(),
                Location = dt.Rows[0]["LOC"].ToString(),
                Line = dt.Rows[0]["LINE"].ToString(),
                Station = dt.Rows[0]["STATION"].ToString(),
                ServiceCatalogId = dt.Rows[0]["SERVICECATALOG"].ToString(),
                ClassificationId = dt.Rows[0]["CLASSI"].ToString(),
                BudgetHead = dt.Rows[0]["BUDGETHEAD"].ToString(),
                WorkTitle = dt.Rows[0]["TITLE"].ToString(),
                ScopeOfWork = dt.Rows[0]["SCOPEWORK"].ToString(),
                SavedAttachmentName = dt.Rows[0]["ATTACHMENT"].ToString(),
                ApprovalAuthorityCode = dt.Rows[0]["SCTMGRCODE"].ToString(),
                RequestId = id.ToString()
            };
            if (!(dt.Rows[0]["ATTACHMENT"].ToString() == null || dt.Rows[0]["ATTACHMENT"].ToString() == ""))
            {
                string strPath = serverpath.getServerPath();
                model.NavigateUrl = "/Utility/OpenDocument?docid=" + id.ToString();
            }
            return View(model);
        }
        
        [HttpPost]
        public IActionResult EditUtilityReq(UtilityRequestViewModel model)
        {
            try
            {
                string strEmpCode = string.Empty;
                string strExtNo = string.Empty;
                string strContactNo = string.Empty;
                string strDirectNo = string.Empty;
                string strLocation = string.Empty;
                string strLine = string.Empty;
                string strStation = string.Empty;
                string strServiceCatalog = string.Empty;
                string strClassification = string.Empty;
                string strScope = string.Empty;
                string strTitle = string.Empty;
                string strbudget = string.Empty;
                string strFilename;
                string strName = "";
                string strMessage;
                string strSectMgrEmail = string.Empty;
                string strsctmgrName = string.Empty;
                string strSectMgrCode = string.Empty;
                string strsctmgrname = string.Empty;
                string strdptmgrcode = string.Empty;
                string strappauth = string.Empty;
                string strSite = string.Empty;
                string responseMessage = string.Empty;

                Employee_Details LoginEmpDetails = _sessionService.Get<Employee_Details>("Employee");
                long userId = Convert.ToInt64(_sessionService.Get<string>("userID"));
                string userName = _sessionService.Get<string>("userName");

                strEmpCode = userId.ToString();
                strExtNo = model.ExtensionNo;
                strContactNo = model.PhoneNo;
                strDirectNo = model.Landline;
                strLocation = (model.Location.Split('~')[0]).ToString();
                strLine = model.Line;
                strStation = model.Station;
                strServiceCatalog = model.ServiceCatalogId;
                strClassification = model.ClassificationId;
                strScope = model.ScopeOfWork;
                strbudget = model.BudgetHead;
                strTitle = model.WorkTitle;
                if (model.Attachment != null && model.Attachment.Length > 0)
                    strFilename = model.Attachment.FileName;
                else
                    strFilename = "";
                strSite = model.SiteId;

                string test = DateTime.Now.Day.ToString() + "" + DateTime.Now.Month.ToString() + "" + DateTime.Now.Year.ToString() + "" + DateTime.Now.Hour.ToString() + "" + DateTime.Now.Minute.ToString() + "" + DateTime.Now.Second.ToString();

                if (Path.GetFileName(strFilename) != "" && Path.GetFileName(strFilename) != null)
                {
                    strName = test + "" + Path.GetFileName(strFilename);
                }
                else
                {
                    strName = model.SavedAttachmentName;
                }
                string path = serverpath.getFileUploadPath();
                strMessage = uploadFile(model, strName, path);
                strappauth = model.ApprovalAuthorityCode;

                // submit utility request
                string strResult = _UtilityDeskService.EditUtilityReq(model.RequestId, strEmpCode, strExtNo, strContactNo, strDirectNo, strLocation, strLine, strStation, strServiceCatalog, strClassification, strbudget, strTitle, strScope, strName, strappauth);
                string[] strStatus = strResult.Split(new Char[] { '#' });
                string errResult = Convert.ToString(strStatus[0]);
                //int intRequestNo = Convert.ToInt32(strStatus[1]);
                string errMsg = Convert.ToString(strStatus[1]);

                if (errResult == "0" || errResult == "2")
                {
                    responseMessage = errMsg;
                }

                if (errResult == "1")
                {
                    string strSubject = string.Empty;
                    string strBody = string.Empty;
                    Employee_Details objEmpDetails = _UtilityDeskService.GetEmployeeDetails(Convert.ToInt64(strappauth));
                    if (objEmpDetails != null)
                    {
                        strSectMgrEmail = objEmpDetails.EMail_Id;
                        strsctmgrname = objEmpDetails.Employee_Name;
                    }

                    // send mail
                    if (strSectMgrEmail != "")
                    {
                        commanEmail sendMail = new commanEmail();
                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        //sendMail.MailTo = strSectMgrEmail;

                        if (serverpath.isTestServer())
                            sendMail.MailTo = serverpath.getTestEMail();
                        else
                            sendMail.MailTo = strSectMgrEmail;

                        strSubject = "Utility Request from - " + userName + " [ " + userId.ToString() + " ]";
                        strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                                         "<tr><td height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                                         "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                         "</td></tr>" +
                                         "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                                         "<b>Utility Request </b></td></tr><tr height=150><td colspan=2>" +
                                         "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                         "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                         "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                         "<tr><td colspan=3>&nbsp;</td></tr><tr><td valign=top colspan=2><p><b>Dear " + strsctmgrname + " San,</b><br />" +
                                         "<br />" + userName + " [ " + userId.ToString() + " ] " +
                                         "has given a Utility request ID - " + model.RequestId + ", the details are as follows:<br />" +
                                         "<br />" +
                                         "<tr><td width=125 height=23 valign=top>Service Catalog:</td><td width=389 valign=top>" + model.ServiceCatalogDescription + "</td>" +
                                         "</tr><tr><td width=125 height=23 valign=top>Classification:</td><td width=389 valign=top>" + model.ClassificationDescription + "</td>" +
                                         "</tr><tr><td width=125 height=23 valign=top>Budget Head:</td><td width=389 valign=top>" + model.BudgetHead + "</td>" +
                                         "</tr><tr>" +
                                         "<td width=125 valign=top>Work Title</td><td width=389 valign=top>" + model.WorkTitle + "</td></tr>" +
                                         "<tr><td valign=top colspan=2>Please login Employee Portal for further action.</td></tr></p>" +
                                         "</td><td width=20>&nbsp;</td></tr><tr valign=bottom> " +
                                         "<td colspan=2><b>Best Regards</b><br /> Team Portal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                                         "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                         "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images//Table_layout_06.gif border=0 /></td> " +
                                         "</tr></table> ";

                        sendMail.MailSubject = strSubject;
                        sendMail.MailBody = strBody;
                        try
                        {
                            bool status1 = sendMail.Send();
                        }
                        catch (Exception ex)
                        {
                            responseMessage += @"\n" + ex.Message;
                        }
                    }
                    var result = new
                    {
                        status = 1,
                        message = "Request Updated Successfully",
                        exception = ""
                    };
                    return Ok(result);
                }
                else
                {
                    var result = new
                    {
                        status = 0,
                        message = responseMessage,
                        exception = ""
                    };
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                var result = new
                {
                    status = -1,
                    message = "Something went wrong",
                    exception = ex.Message
                };
                return BadRequest(result);
            }
        }

        public IActionResult CancelNewUtReq(long id)
        {
            Employee_Details LoginEmpDetails = _sessionService.Get<Employee_Details>("Employee");
            long userId = Convert.ToInt64(_sessionService.Get<string>("userID"));

            UtilityRequestViewModel contactInfo = _UtilityDeskService.GetContactInfo(userId);
            UtilityRequestViewModel approvalAuthority = _UtilityDeskService.GetSelectedApprovalAutority(userId);

            //Get Utility Request Details
            DataTable dt = _UtilityDeskService.GetUtilityRequestDetail(id);

            UtilityRequestViewModel model = new UtilityRequestViewModel()
            {
                SiteId = LoginEmpDetails.Site_Id,
                ExtensionNo = dt.Rows[0]["EXTN"].ToString(),
                PhoneNo = dt.Rows[0]["CONTACT"].ToString(),
                Landline = dt.Rows[0]["DIRECTNO"].ToString(),
                Location = dt.Rows[0]["LOC"].ToString(),
                Line = dt.Rows[0]["LINE"].ToString(),
                Station = dt.Rows[0]["STATION"].ToString(),
                ServiceCatalogId = dt.Rows[0]["SERVICECATALOG"].ToString(),
                ClassificationId = dt.Rows[0]["CLASSI"].ToString(),
                BudgetHead = dt.Rows[0]["BUDGETHEAD"].ToString(),
                WorkTitle = dt.Rows[0]["TITLE"].ToString(),
                ScopeOfWork = dt.Rows[0]["SCOPEWORK"].ToString(),
                SavedAttachmentName = dt.Rows[0]["ATTACHMENT"].ToString(),
                ApprovalAuthorityCode = dt.Rows[0]["SCTMGRCODE"].ToString(),
                RequestId = id.ToString(),
                EmpName = dt.Rows[0]["EMPNAME"].ToString(),
                ServiceCatalogDescription = dt.Rows[0]["CATALOG"].ToString(),
                LocationDescription = dt.Rows[0]["LOCATION"].ToString(),
                ClassificationDescription = dt.Rows[0]["CLASSIFICATION"].ToString(),
            };
            if (!(dt.Rows[0]["ATTACHMENT"].ToString() == null || dt.Rows[0]["ATTACHMENT"].ToString() == ""))
            {
                string strPath = serverpath.getServerPath();
                model.NavigateUrl = "/Utility/OpenDocument?docid=" + id.ToString();
            }
            Employee_Details objEmpDetails = _UtilityDeskService.GetEmployeeDetails(Convert.ToInt64(model.ApprovalAuthorityCode));
            model.SectionManager = $"{objEmpDetails.Employee_Name}-{model.ApprovalAuthorityCode}";
            return View(model);
        }
        
        [HttpPost]
        public IActionResult CancelNewUtReq(UtilityRequestViewModel model)
        {
            try
            {
                string strSectMgrEmail = string.Empty;
                string strsctmgrname = string.Empty;

                string strSubject = string.Empty;
                string strBody = string.Empty;

                long approverId = Convert.ToInt64(model.ApprovalAuthorityCode);

                Employee_Details sectionManagerInfo = _UtilityDeskService.GetEmployeeDetails(approverId);
                strSectMgrEmail = sectionManagerInfo.EMail_Id;
                strsctmgrname = sectionManagerInfo.Employee_Name;

                string strResult = _UtilityDeskService.CancelUtReq(model.CancellationRemarks, model.RequestId.ToString());
                string[] strStatus = strResult.Split(new Char[] { '#' });
                string errResult = Convert.ToString(strStatus[0]);
                string errMsg = Convert.ToString(strStatus[1]);
                string responseMessage = string.Empty;

                if (errResult == "0" || errResult == "2")
                {
                    responseMessage = errMsg;
                }
                if (errResult == "1")
                {
                    // send mail
                    if (strSectMgrEmail != "")
                    {
                        commanEmail sendMail = new commanEmail();
                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        //sendMail.MailTo = strSectMgrEmail;

                        if (serverpath.isTestServer())
                            sendMail.MailTo = serverpath.getTestEMail();
                        else
                            sendMail.MailTo = strSectMgrEmail;

                        strSubject = "Cancellation of Utility Request from - " + model.EmpName;
                        strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                                 "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                                 "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                 "</td></tr>" +
                                 "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                                 "<b>Utility Request</b></td></tr><tr height=150><td colspan=2>" +
                                 "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                 "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                 "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                 "<tr><td colspan=3>&nbsp;</td></tr><tr><td valign=top colspan=2><p><b>Dear " + strsctmgrname + " San,</b><br />" +
                                 "<br />" + model.EmpName +
                                 " has given a Utility request, the details are as follows:<br />" +
                                 "<br />" +
                                 "<tr><td width=125 height=23 valign=top>Service Catalog:</td><td width=389 valign=top>" + model.ServiceCatalogDescription + "</td>" +
                                 "</tr><tr><td width=125 height=23 valign=top>Classification:</td><td width=389 valign=top>" + model.ClassificationDescription + "</td>" +
                                 "</tr><tr><td width=125 height=23 valign=top>Budget Head:</td><td width=389 valign=top>" + model.BudgetHead + "</td>" +
                                 "</tr><tr>" +
                                 "<td width=125 valign=top>Work Title</td><td width=389 valign=top>" + model.WorkTitle + "</td></tr>" +
                                 "<tr><td valign=top colspan=2>Please login Employee Portal for further action.</td></tr></p>" +
                                 "</td><td width=20>&nbsp;</td></tr><tr valign=bottom> " +
                                 "<td colspan=2><b>Best Regards</b><br /> Team Portal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                                 "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                 "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images//Table_layout_06.gif border=0 /></td> " +
                                 "</tr></table> ";

                        sendMail.MailSubject = strSubject;
                        sendMail.MailBody = strBody;
                        try
                        {
                            bool status1 = sendMail.Send();
                        }
                        catch (Exception ex)
                        {
                            responseMessage += @"\n" + ex.Message;
                        }
                    }
                    var result = new
                    {
                        status = 1,
                        message = "Request Cancelled Successfully",
                        exception = ""
                    };
                    return Ok(result);
                }
                else
                {
                    var result = new
                    {
                        status = 0,
                        message = responseMessage,
                        exception = ""
                    };
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                var result = new
                {
                    status = -1,
                    message = "Something went wrong",
                    exception = ex.Message
                };
                return BadRequest(result);
            }
        }

        public IActionResult UpdateUtilityRequest(long id)
        {
            string strextension = string.Empty;
            string strcontact = string.Empty;
            string strname = string.Empty;
            string strlocation = string.Empty;
            string strline = string.Empty;
            string strstation = string.Empty;
            string strcatalog = string.Empty;
            string strclassification = string.Empty;
            string strbudgethead = string.Empty;
            string strworktitle = string.Empty;
            string strscope = string.Empty;
            string strattachment = string.Empty;
            string strsctmgrcode = string.Empty;
            string strsctmgrname = string.Empty;
            string strSectMgrStatus = string.Empty;
            
            UtilityRequestViewModel model = new UtilityRequestViewModel();
            long userId = Convert.ToInt64(_sessionService.Get<string>("userID"));

            //Get Utility Request Details
            DataTable dt = _UtilityDeskService.GetUtilityRequestDetail(id);
            if (dt != null && dt.Rows.Count > 0)
            {
                model = new UtilityRequestViewModel()
                {
                    RequestId = id.ToString(),
                    EmpCode = dt.Rows[0]["EMPCODE"].ToString(),
                    EmpName = dt.Rows[0]["EMPNAME"].ToString(),
                    ExtensionNo = dt.Rows[0]["EXTN"].ToString(),
                    PhoneNo = dt.Rows[0]["CONTACT"].ToString(),
                    LocationDescription = dt.Rows[0]["LOCATION"].ToString(),
                    Line = dt.Rows[0]["LINE"].ToString(),
                    Station = dt.Rows[0]["STATION"].ToString(),
                    ServiceCatalogDescription = dt.Rows[0]["CATALOG"].ToString(),
                    ClassificationDescription = dt.Rows[0]["CLASSIFICATION"].ToString(),
                    BudgetHead = dt.Rows[0]["BUDGETHEAD"].ToString(),
                    WorkTitle = dt.Rows[0]["TITLE"].ToString(),
                    ScopeOfWork = dt.Rows[0]["SCOPEWORK"].ToString(),
                    SavedAttachmentName = dt.Rows[0]["ATTACHMENT"].ToString(),
                    IsDeptApproved = dt.Rows[0]["ISDPTRAPPROVED"].ToString(),
                    IsSectApproved = dt.Rows[0]["ISSCTAPPROVED"].ToString(),
                };

                if (dt.Rows[0]["ISSCTAPPROVED"].ToString() == "1")
                {
                    model.SectionManagerStatus = "Approved";
                    model.UpdateUTRControls.SectionManagerStatusVisible = false;
                    model.SectionManagerRemarks = dt.Rows[0]["SECTMGRREMARK"].ToString();
                    model.UpdateUTRControls.TxtSctMgrRemarkVisblity = false;
                    strSectMgrStatus = "1";
                }

                if (dt.Rows[0]["ISSCTAPPROVED"].ToString() == "0")
                {
                    model.UpdateUTRControls.Panel1Visiblity = false;
                    strSectMgrStatus = "0";
                }

                if (!(dt.Rows[0]["ATTACHMENT"].ToString() == null || dt.Rows[0]["ATTACHMENT"].ToString() == ""))
                {
                    string strPath = serverpath.getServerPath();
                    model.NavigateUrl = "/Utility/OpenDocument?docid=" + id.ToString();
                }
                strsctmgrcode = dt.Rows[0]["SCTMGRCODE"].ToString();

                dt = _UtilityDeskService.GetMailIds(Convert.ToInt64(model.EmpCode));

                if (dt != null && dt.Rows.Count > 0)
                {
                    model.UserMailId = dt.Rows[0]["USEREMAILID"].ToString();
                    model.DeptMgrMailId = dt.Rows[0]["DEPTEMAILID"].ToString();
                    model.DeptMgrEmpCode = dt.Rows[0]["DEPTCODE"].ToString();
                    model.DeptMgrName = dt.Rows[0]["DPTMGRNAME"].ToString();

                    if (strSectMgrStatus == "1")
                    {
                        model.UpdateUTRControls.AppPanelVisiblity = false;
                        model.UpdateUTRControls.LblAppAuthVisiblity = false;
                    }
                    else
                    {
                        model.UpdateUTRControls.LblAppAuthText = model.DeptMgrName + " (Emp Code - " + model.DeptMgrEmpCode + ")";
                    }
                }

                Employee_Details objEmpDetails = _UtilityDeskService.GetEmployeeDetails(Convert.ToInt64(strsctmgrcode));

                if (objEmpDetails != null && model.IsSectApproved == "1")
                {
                    model.UpdateUTRControls.LblSctMgrNameText = "- (" + objEmpDetails.Employee_Name + "-" + strsctmgrcode + ")";
                }
            }
            return View(model);
        }
        
        [HttpPost]
        public IActionResult UpdateUtilityRequest(UtilityRequestViewModel model)
        {
            string strutemailid = string.Empty;
            string strdeptmgrstatus = string.Empty;
            string strsectmgrstatus = string.Empty;
            string strdeptmgrremark = string.Empty;
            string strsectmgrremark = string.Empty;
            string strrequestid = string.Empty;
            string strsctmgrcode = string.Empty;
            string strsctmgrname = string.Empty;
            string struserempcode = string.Empty;
            string responseMessage = string.Empty;
            try
            {
                long userId = Convert.ToInt64(_sessionService.Get<string>("userID"));
                string userName = _sessionService.Get<string>("userName");

                try
                {
                    DataTable dtMailIds = _UtilityDeskService.GetMailIds(Convert.ToInt64(model.EmpCode));
                    if (dtMailIds != null && dtMailIds.Rows.Count > 0)
                    {
                        model.UserMailId = dtMailIds.Rows[0]["USEREMAILID"].ToString();
                        model.DeptMgrMailId = dtMailIds.Rows[0]["DEPTEMAILID"].ToString();
                        model.DeptMgrEmpCode = dtMailIds.Rows[0]["DEPTCODE"].ToString();
                        model.DeptMgrName = dtMailIds.Rows[0]["DPTMGRNAME"].ToString();
                    }
                }
                catch { }

                try
                {
                    Employee_Details LoginEmpDetails = _UtilityDeskService.GetEmployeeDetails(userId);
                    if (LoginEmpDetails != null)
                    {
                        strsctmgrcode = LoginEmpDetails.Employee_Code;
                        strsctmgrname = LoginEmpDetails.Employee_Name;
                    }
                }
                catch (Exception ex) {
                    responseMessage = ex.Message;
                    var result = new
                    {
                        status = 0,
                        message = responseMessage,
                        exception = ""
                    };
                    return BadRequest(result);
                }

                strdeptmgrstatus = model.IsDeptApproved;
                strsectmgrstatus = model.IsSectApproved;
                strdeptmgrremark = model.DeptManagerRemarks;
                strsectmgrremark = model.SectionManagerRemarks;
                struserempcode = model.EmpCode;
                strrequestid = model.RequestId;

                if (strsectmgrstatus != "-1" && strdeptmgrstatus == "0")
                {
                    // update utility request
                    string strResult = _UtilityDeskService.EditUtReqBySctMgr(strrequestid, strsectmgrstatus, strsectmgrremark, model.DeptMgrEmpCode, userId.ToString());
                    string[] strStatus = strResult.Split(new Char[] { '#' });
                    string errResult = Convert.ToString(strStatus[0]);
                    string errMsg = Convert.ToString(strStatus[1]);

                    if (errResult == "0" || errResult == "2")
                    {
                        responseMessage = errMsg;
                        var result = new
                        {
                            status = 0,
                            message = responseMessage,
                            exception = ""
                        };
                        return BadRequest(result);
                    }
                    // send mail to dept. head for approval
                    else if (errResult == "1")
                    {
                        string strSubject = string.Empty;
                        string strBody = string.Empty;

                        commanEmail sendMail = new commanEmail();
                        if (strsectmgrstatus == "1" && model.DeptMgrMailId != "")
                        {
                            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                            if (serverpath.isTestServer())
                                sendMail.MailTo = serverpath.getTestEMail();
                            else
                                sendMail.MailTo = model.DeptMgrMailId;

                            strSubject = "Utility Request from - " + model.EmpName;
                            strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                                             "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                                             "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                             "</td></tr>" +
                                             "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                                             "<b>Utility Request</b></td></tr><tr height=150><td colspan=2>" +
                                             "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                             "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                             "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                             "<tr><td colspan=3>&nbsp;</td></tr><tr><td valign=top colspan=2><p><b>Dear " + model.DeptMgrName + " San,</b><br />" +
                                             "<br />" + model.EmpName +
                                             "has given a Utility request, the details are as follows:<br />" +
                                             "<br />" +
                                             "<tr><td width=125 height=23 valign=top>Service Catalog:</td><td width=389 valign=top>" + model.ServiceCatalogDescription + "</td>" +
                                             "</tr><tr><td width=125 height=23 valign=top>Classification:</td><td width=389 valign=top>" + model.ClassificationDescription + "</td>" +
                                             "</tr><tr><td width=125 height=23 valign=top>Budget Head:</td><td width=389 valign=top>" + model.BudgetHead + "</td>" +
                                             "</tr><tr>" +
                                             "<td width=125 valign=top>Work Title</td><td width=389 valign=top>" + model.WorkTitle + "</td></tr>" +
                                             "<tr><td valign=top colspan=2>Please login Employee Portal for further action.</td></tr></p>" +
                                             "</td><td width=20>&nbsp;</td></tr><tr valign=bottom> " +
                                             "<td colspan=2><b>Best Regards</b><br /> Team Portal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                                             "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                             "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images//Table_layout_06.gif border=0 /></td> " +
                                             "</tr></table> ";

                            sendMail.MailSubject = strSubject;
                            sendMail.MailBody = strBody;
                            try
                            {
                                bool status1 = sendMail.Send();
                            }
                            catch (Exception ex)
                            {
                                responseMessage += @"\n" + ex.Message;
                            }
                        }
                        // send mail to user for rejection
                        else if (strsectmgrstatus == "2" && model.UserMailId != "")
                        {
                            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                            if (serverpath.isTestServer())
                                sendMail.MailTo = serverpath.getTestEMail();
                            else
                                sendMail.MailTo = model.UserMailId;


                            strSubject = "Utility Request is rejected.";
                            strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                                             "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                                             "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                             "</td></tr>" +
                                             "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                                             "<b>Utility Request from  </b></td></tr><tr height=150><td colspan=2>" +
                                             "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                             "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                             "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                             "<tr><td colspan=3>&nbsp;</td></tr><tr><td valign=top colspan=2><p><b>Dear " + model.EmpName + " San,</b><br />" +
                                             "<br />" + strsctmgrname +
                                             "has rejected your utility request.<br />" +
                                             "<br />" +
                                             "<tr><td width=125 height=23 valign=top>Remarks:</td><td width=389 valign=top>" + model.SectionManagerRemarks + "</td></tr>" +
                                             "<tr><td valign=top colspan=2>Please login Employee Portal for further action.</td></tr></p>" +
                                             "</td><td width=20>&nbsp;</td></tr><tr valign=bottom> " +
                                             "<td colspan=2><b>Best Regards</b><br /> Team Portal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                                             "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                             "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images//Table_layout_06.gif border=0 /></td> " +
                                             "</tr></table> ";

                            sendMail.MailSubject = strSubject;
                            sendMail.MailBody = strBody;
                            try
                            {
                                bool status1 = sendMail.Send();
                            }
                            catch (Exception ex)
                            {
                                responseMessage += @"\n" + ex.Message;
                            }
                        }
                        
                        var result = new
                        {
                            status = 1,
                            message = "Request Updated Successfully",
                            exception = ""
                        };
                        return Ok(result);
                    }
                    else
                    {
                        responseMessage = errMsg;
                        var result = new
                        {
                            status = 0,
                            message = responseMessage,
                            exception = ""
                        };
                        return BadRequest(result);
                    }
                }
                else if (strsectmgrstatus == "1" && strdeptmgrstatus != "-1")
                {
                    string strResult = _UtilityDeskService.EditUtReqByDptMgr(strrequestid, strdeptmgrstatus, strdeptmgrremark);
                    string[] strStatus = strResult.Split(new Char[] { '#' });
                    string errResult = Convert.ToString(strStatus[0]);
                    string errMsg = Convert.ToString(strStatus[1]);

                    if (errResult == "0" || errResult == "2")
                    {
                        responseMessage = errMsg;
                        var result = new
                        {
                            status = 0,
                            message = responseMessage,
                            exception = ""
                        };
                        return BadRequest(result);
                    }
                    else if (errResult == "1")
                    {
                        string strSubject = string.Empty;
                        string strBody = string.Empty;
                        string strAuthEmailTo = string.Empty;
                        string strAuthEmailCc = string.Empty;
                        DataSet objds = _UtilityDeskService.GetAminEmailId();
                        if (objds.Tables[0].Rows.Count > 0)
                        {
                            strAuthEmailTo = objds.Tables[0].Rows[0][0].ToString();
                            strAuthEmailCc = objds.Tables[0].Rows[0][1].ToString();
                        }

                        commanEmail sendMail = new commanEmail();
                        //strutemailid = "akhilesh.gupta1@honda.hmsi.in";
                        if (strdeptmgrstatus == "1" && strutemailid != "")
                        {
                            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                            if (serverpath.isTestServer())
                                sendMail.MailTo = serverpath.getTestEMail();
                            else
                                sendMail.MailTo = strAuthEmailTo;


                            strSubject = "Utility Request from - " + model.EmpName;
                            strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                                             "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                                             "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                             "</td></tr>" +
                                             "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                                             "<b>Utility Request from  </b></td></tr><tr height=150><td colspan=2>" +
                                             "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                             "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                             "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                             "<tr><td colspan=3>&nbsp;</td></tr><tr><td valign=top colspan=2><p><b>Dear Utility Admin,</b><br />" +
                                             "<br />" + model.EmpName +
                                             "has given a Utility request, the details are as follows:<br />" +
                                             "<br />" +
                                             "<tr><td width=125 height=23 valign=top>Service Catalog:</td><td width=389 valign=top>" + model.ServiceCatalogDescription + "</td>" +
                                             "</tr><tr><td width=125 height=23 valign=top>Classification:</td><td width=389 valign=top>" + model.ClassificationDescription + "</td>" +
                                             "</tr><tr><td width=125 height=23 valign=top>Budget Head:</td><td width=389 valign=top>" + model.BudgetHead + "</td>" +
                                             "</tr><tr>" +
                                             "<td width=125 valign=top>Work Title</td><td width=389 valign=top>" + model.WorkTitle + "</td></tr>" +
                                             "<tr><td valign=top colspan=2>Please login Employee Portal for further action.</td></tr></p>" +
                                             "</td><td width=20>&nbsp;</td></tr><tr valign=bottom> " +
                                             "<td colspan=2><b>Best Regards</b><br /> Team Portal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                                             "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                             "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images//Table_layout_06.gif border=0 /></td> " +
                                             "</tr></table> ";

                            sendMail.MailSubject = strSubject;
                            sendMail.MailBody = strBody;
                            try
                            {
                                bool status1 = sendMail.Send();
                            }
                            catch (Exception ex)
                            {
                                responseMessage += @"\n" + ex.Message;
                            }
                        }
                        else if (strdeptmgrstatus == "2" && model.UserMailId != "")
                        {
                            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                            if (serverpath.isTestServer())
                                sendMail.MailTo = serverpath.getTestEMail();
                            else
                                sendMail.MailTo = model.UserMailId;

                            strSubject = "Utility Request is rejected.";
                            strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                                             "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                                             "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                             "</td></tr>" +
                                             "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                                             "<b>Utility Request from  </b></td></tr><tr height=150><td colspan=2>" +
                                             "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                             "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                             "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                             "<tr><td colspan=3>&nbsp;</td></tr><tr><td valign=top colspan=2><p><b>Dear " + model.EmpName + " San,</b><br />" +
                                             "<br />" + model.DeptMgrName +
                                             "has rejected your utility request.<br />" +
                                             "<br />" +
                                             "<tr><td width=125 height=23 valign=top>Remarks:</td><td width=389 valign=top>" + model.SectionManagerRemarks + "</td></tr>" +
                                             "<tr><td valign=top colspan=2>Please login Employee Portal for further action.</td></tr></p>" +
                                             "</td><td width=20>&nbsp;</td></tr><tr valign=bottom> " +
                                             "<td colspan=2><b>Best Regards</b><br /> Team Portal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                                             "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                             "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images//Table_layout_06.gif border=0 /></td> " +
                                             "</tr></table> ";


                            sendMail.MailSubject = strSubject;
                            sendMail.MailBody = strBody;
                            try
                            {
                                bool status1 = sendMail.Send();
                            }
                            catch (Exception ex)
                            {
                                responseMessage += @"\n" + ex.Message;
                            }
                        }

                        var result = new
                        {
                            status = 1,
                            message = "Request Updated Successfully",
                            exception = ""
                        };
                        return Ok(result);
                    }
                    else
                    {
                        responseMessage = errMsg;
                        var result = new
                        {
                            status = 0,
                            message = responseMessage,
                            exception = ""
                        };
                        return BadRequest(result);
                    }
                }
                else
                {
                    var result = new
                    {
                        status = 0,
                        message = "Request could not be posted.",
                        exception = ""
                    };
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                var result = new
                {
                    status = -1,
                    message = "Something went wrong",
                    exception = ex.Message
                };
                return BadRequest(result);
            }
        }

        public IActionResult ViewUtilityRequest(long id)
        {
            Employee_Details LoginEmpDetails = _sessionService.Get<Employee_Details>("Employee");
            long userId = Convert.ToInt64(_sessionService.Get<string>("userID"));

            UtilityRequestViewModel contactInfo = _UtilityDeskService.GetContactInfo(userId);
            UtilityRequestViewModel approvalAuthority = _UtilityDeskService.GetSelectedApprovalAutority(userId);

            //Get Utility Request Details
            DataTable dt = _UtilityDeskService.GetUtilityRequestDetail(id);

            UtilityRequestViewModel model = new UtilityRequestViewModel()
            {
                SiteId = LoginEmpDetails.Site_Id,
                ExtensionNo = dt.Rows[0]["EXTN"].ToString(),
                PhoneNo = dt.Rows[0]["CONTACT"].ToString(),
                Landline = dt.Rows[0]["DIRECTNO"].ToString(),
                Location = dt.Rows[0]["LOC"].ToString(),
                Line = dt.Rows[0]["LINE"].ToString(),
                Station = dt.Rows[0]["STATION"].ToString(),
                ServiceCatalogId = dt.Rows[0]["SERVICECATALOG"].ToString(),
                ClassificationId = dt.Rows[0]["CLASSI"].ToString(),
                BudgetHead = dt.Rows[0]["BUDGETHEAD"].ToString(),
                WorkTitle = dt.Rows[0]["TITLE"].ToString(),
                ScopeOfWork = dt.Rows[0]["SCOPEWORK"].ToString(),
                SavedAttachmentName = dt.Rows[0]["ATTACHMENT"].ToString(),
                ApprovalAuthorityCode = dt.Rows[0]["SCTMGRCODE"].ToString(),
                RequestId = id.ToString(),
                EmpName = dt.Rows[0]["EMPNAME"].ToString(),
                ServiceCatalogDescription = dt.Rows[0]["CATALOG"].ToString(),
                LocationDescription = dt.Rows[0]["LOCATION"].ToString(),
                ClassificationDescription = dt.Rows[0]["CLASSIFICATION"].ToString(),
                SectionManagerStatus = dt.Rows[0]["SCTMGRSTATUS"].ToString(),
                SectionManagerRemarks = dt.Rows[0]["SCTREMARK"].ToString(),
                DeptManagerStatus = dt.Rows[0]["DPTMGRSTATUS"].ToString(),
                DeptManagerRemarks = dt.Rows[0]["DPTREMARK"].ToString(),
                SubmitDate = dt.Rows[0]["SUBMITDATE"].ToString(),
                NoOfQuotes = dt.Rows[0]["NOOFQUOTES"].ToString(),
                Price = dt.Rows[0]["PRICE"].ToString(),
                Remarks = dt.Rows[0]["REMARK"].ToString(),
                PoNo = dt.Rows[0]["PONO"].ToString(),
                PoDate = dt.Rows[0]["PODATE"].ToString(),
                UtStatus = dt.Rows[0]["UTSTATUS"].ToString(),
                CloseRemarks = dt.Rows[0]["CLOSEREMARK"].ToString(),
                LogDate = dt.Rows[0]["DATEADDED"].ToString(),
                ResponsibleAuthorityCode = dt.Rows[0]["EMPNAE"].ToString() + " [" + dt.Rows[0]["CONTACTNUMBER"].ToString() + "]",
                SiteDescription = dt.Rows[0]["SITE"].ToString()
            };
            return View(model);
        }
        
        [HttpPost]
        public IActionResult ViewUtilityRequest([FromBody] UtilityRequestViewModel model)
        {
            return View();
        }

        public IActionResult OpenDocument(long docId)
        {
            try
            {
                var document = _UtilityDeskService.GetUtilityDocumentInfo(docId);
                return PhysicalFile(physicalPath: document.PhysicalPath, contentType: document.ContentType, fileDownloadName: document.FileDownloadName);
            }
            catch (Exception)
            {
                return Ok("The requested file was not found on the Server. The file may be moved or deleted.");
            }
        }
        
        public JsonResult GetLocations(string SiteId)
        {
            var selectList = _CommonFunctions.GetLocation(strsite: SiteId).AsSelectList(valueField: "LOCATIONVALUE", textField: "LOCATION", true, "-Select-", "-1");
            return Json(selectList);
        }
        public JsonResult GetClassification(string ServiceCatalogId)
        {
            var selectList = _UtilityDeskService.get_Classification(servicecatalogid: ServiceCatalogId).AsSelectList_DS(valueField: "classificationid", textField: "classification", true, "-Select Classification-", "0");
            return Json(selectList);
        }
        public JsonResult GetResponsibleUserDetails(string Location)
        {
            UtilityRequestViewModel viewModel = new UtilityRequestViewModel();
            string LocationCode = string.Empty;
            string EmpDeptCode = string.Empty;
            string ResponsibleUser = string.Empty;
            string UserMobileNumber = string.Empty;
            DataTable dt = new DataTable();
            //GET EMPLOYEE DEPARTMENT
            //GET CURRENT KI
            string strKiId = _iPMS_DAL.GetKIId();
            dt = _CommonFunctions.GetEmployeeOfficialDetails(_sessionService.Get<string>("userID"), strKiId);
            if (dt.Rows.Count > 0)
                EmpDeptCode = dt.Rows[0]["ADDEPARTMENTID"].ToString();

            LocationCode = Location;//cboLocation.SelectedValue;

            //GET RESPONSIBLE USER FOR SELECTED LOCATION AND DEPARTMENT
            dt = new DataTable();
            dt = _UtilityDeskService.GetResponsibleUserDetails(EmpDeptCode, LocationCode);

            if (dt.Rows.Count > 0)
            {
                ResponsibleUser = dt.Rows[0]["EMPNAME"].ToString();
                UserMobileNumber = dt.Rows[0]["CONTACTNUMBER"].ToString();
                viewModel.ResponsibleAuthorityCode = ResponsibleUser + " [" + UserMobileNumber + "]";
            }
            return Json(null);
        }

        public string uploadFile(UtilityRequestViewModel model, string strFileName, string strFolderName)
        {
            //string test = DateTime.Now.Day.ToString() + "" + DateTime.Now.Month.ToString() + "" + DateTime.Now.Year.ToString() + "" + DateTime.Now.Hour.ToString() + "" + DateTime.Now.Minute.ToString() + "" + DateTime.Now.Second.ToString();
            if (strFileName == "")
            { return "Invalid file name supplied"; }

            if (model.Attachment == null || model.Attachment.Length == 0)
            { return "Invalid file content"; }

            //strFileName = test + "" + System.IO.Path.GetFileName(strFileName);

            if (strFolderName == "")
            { return "Path not found"; }

            try
            {
                if (model.Attachment.Length <= 2048000)
                {

                    using var memoryStream = new MemoryStream();
                    model.Attachment.CopyTo(memoryStream);
                    byte[] fileBytes = memoryStream.ToArray();
                    System.IO.File.WriteAllBytes(filePath + "\\" + strFileName.ToString(), fileBytes);
                    return "1";
                }
                else
                {
                    return "Unable to upload the file, It exceeds the maximum limit";
                }
            }
            catch (UnauthorizedAccessException ex)
            { return ex.Message + "Permission to upload file denied"; }
        }
    }
}
