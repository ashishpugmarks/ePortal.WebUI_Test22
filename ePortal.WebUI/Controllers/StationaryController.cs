using System.Data;
using System.Text;
using ePortal.Persistence;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.Persistence.TourRequest.Interface;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.ViewModels.APPX.Stationary;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    [SessionTimeout]
    public class StationaryController : Controller
    {
        private readonly ITourQueries _TourQueryService;
        private readonly IAD_ITEM_MASTER _AdItemMaster;
        private readonly IStationary _Stationary;

        private readonly ICommonFunctions _CommonFunctions;
        private readonly IPMS_DAL _iPMS_DAL;
        private readonly ISessionService _sessionService;



        public StationaryController(ITourQueries TourQueryService, ICommonFunctions CommonFunctions, ISessionService sessionService, IPMS_DAL iPMS_DAL, IAD_ITEM_MASTER AdItemMaster, IStationary Stationary)
        {
            _TourQueryService = TourQueryService;
            _CommonFunctions = CommonFunctions;
            _sessionService = sessionService;
            _iPMS_DAL = iPMS_DAL;
            _AdItemMaster = AdItemMaster;
            _Stationary = Stationary;
        }

        public IActionResult StationaryRequest()
        {
            Employee_Details LoginEmpDetails = _sessionService.Get<Employee_Details>("Employee");
            long userId = Convert.ToInt64(_sessionService.Get<string>("userID"));
            string selectedPlantId = "-1";
            //Get Selected Plant Id
            DataTable dtSelectedPlant = _AdItemMaster.GetUser_Detail(userId.ToString());
            if (dtSelectedPlant != null && dtSelectedPlant.Rows.Count > 0)
            {
                selectedPlantId = dtSelectedPlant.Rows[0]["syplantid"].ToString();
            }
            StationaryViewModel model = new StationaryViewModel()
            {
                Plants = _CommonFunctions.GetPlant().AsSelectList(valueField: "SYPLANTID", textField: "PLANTNAME", true, "--Select--", "-1"),
                PlantCode = selectedPlantId,
                Approvers = _Stationary.GetApprovalList(userId.ToString()).AsSelectList(valueField: "ADEMPCODE", textField: "EMPNAME", true, "-- select --", ""),
                StationeryItems = _Stationary.Get_Stationarymaster("0", "1", selectedPlantId).AsSelectList(valueField: "ADSTATIONARYID", textField: "DESCRIP", true, "--Select--", "-1"),
            };

            //Get Employee Details
            DataTable dtEmp = _TourQueryService.EmployeeDetail(Convert.ToInt32(userId.ToString()));
            if (dtEmp.Rows.Count > 0)
            {
                model.MobileNumber = dtEmp.Rows[0]["TMOBILE"].ToString();
                model.ExtensionNumber = dtEmp.Rows[0]["EXTENSIONNO"].ToString();
            }

            //Initialize Stationary Item Grid
            _sessionService.Set<List<StationaryItem>>("STATIONARY_ITEMS", new List<StationaryItem>());


            return View(model);
        }

        [HttpPost]
        public IActionResult StationaryRequest(StationaryViewModel model)
        {
            try
            {
                Employee_Details LoginEmpDetails = _sessionService.Get<Employee_Details>("Employee");
                long userId = Convert.ToInt64(_sessionService.Get<string>("userID"));
                model.StationaryCart = _sessionService.Get<List<StationaryItem>>("STATIONARY_ITEMS");
                StringBuilder xmlStationaryList = new StringBuilder();
                if (model.StationaryCart != null && model.StationaryCart.Any())
                {
                    xmlStationaryList.Append("<STATIONARY>");
                    foreach (StationaryItem s in model.StationaryCart)
                    {

                        xmlStationaryList.Append("<STATIONARY>");
                        xmlStationaryList.Append("<STATIONARYID>" + s.Item + "</STATIONARYID>");
                        xmlStationaryList.Append("<QTY>" + s.Qty + "</QTY>");
                        xmlStationaryList.Append("<REMARKS>" + s.Remark + "</REMARKS>");
                        xmlStationaryList.Append("</STATIONARY>");
                    }
                    xmlStationaryList.Append("</STATIONARY>");
                }
                string strEmpCode = string.Empty;
                string strappauth = string.Empty;
                string strappname = string.Empty;
                string strappauthemailid = string.Empty;
                string strMobileNo = string.Empty;
                string strExtNo = string.Empty;
                string err = string.Empty;

                strEmpCode = userId.ToString();
                strappauth = model.ApproverId;
                strappname = model.ApproverName;
                strMobileNo = model.MobileNumber;
                strExtNo = model.ExtensionNumber;
                if (strappauth == "" || strExtNo == "")
                {
                    model.exceptionInfo.ErrorPanelVisiblity = true;
                    model.exceptionInfo.ErrorMessage = "Please Enter Extantion no and Select Approval Authorty";
                    model.exceptionInfo.ControlId = "txtExtension";
                }
                if (model.StationaryCart == null || !model.StationaryCart.Any())
                {
                    model.exceptionInfo.ErrorPanelVisiblity = true;
                    model.exceptionInfo.ErrorMessage = "Please Add at least one Stationary";
                    model.exceptionInfo.ControlId = "gvList";
                }
                //Post data when validation passed
                if (model.exceptionInfo.ErrorPanelVisiblity == false)
                {
                    err = _Stationary.InsertStationaryReq(strEmpCode, strMobileNo, strExtNo, xmlStationaryList.ToString(), strappauth);
                }

                // if error in updating the process, then display it
                if (err.Trim() != "")
                {
                    model.exceptionInfo.ErrorPanelVisiblity = true;
                    model.exceptionInfo.ErrorMessage = err;
                    model.exceptionInfo.ControlId = "ddlplant";
                }
                else
                {
                    if (model.exceptionInfo.ErrorPanelVisiblity == false)
                    {
                        strappauthemailid = _CommonFunctions.GetEMailID(strappauth);
                        //Email send to Approval authority
                        SendMail(strappauthemailid, strappname, model);
                        model.exceptionInfo.ErrorPanelVisiblity = false;
                        model.exceptionInfo.ErrorMessage = "Ok";
                        model.exceptionInfo.ControlId = "";
                    }
                }
                if (model.exceptionInfo.ErrorPanelVisiblity == false)
                {
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
                        message = model.exceptionInfo.ErrorMessage,
                        exception = ""
                    };
                    return Ok(result);
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

        public IActionResult EditStationaryRequest(string id)
        {
            Employee_Details LoginEmpDetails = _sessionService.Get<Employee_Details>("Employee");
            long userId = Convert.ToInt64(_sessionService.Get<string>("userID"));
            string selectedPlantId = "-1";
            //Get Selected Plant Id
            DataTable dtSelectedPlant = _AdItemMaster.GetUser_Detail(userId.ToString());
            if (dtSelectedPlant != null && dtSelectedPlant.Rows.Count > 0)
            {
                selectedPlantId = dtSelectedPlant.Rows[0]["syplantid"].ToString();
            }
            StationaryViewModel model = new StationaryViewModel()
            {
                Plants = _CommonFunctions.GetPlant().AsSelectList(valueField: "SYPLANTID", textField: "PLANTNAME", true, "--Select--", "-1"),
                PlantCode = selectedPlantId,
                StationeryItems = _Stationary.Get_Stationarymaster("0", "1", selectedPlantId).AsSelectList(valueField: "ADSTATIONARYID", textField: "DESCRIP", true, "--Select--", "-1"),
                RequestId = id
            };

            DataTable dtHeader = FillRequestHeader(id);
            if (dtHeader != null && dtHeader.Rows.Count > 0)
            {
                model.InitiatorName = dtHeader.Rows[0]["EMPNAME"].ToString();
                model.MobileNumber = dtHeader.Rows[0]["MOBILENO"].ToString();
                model.ExtensionNumber = dtHeader.Rows[0]["EXTENTIONNO"].ToString();
                model.InitiatedDate = dtHeader.Rows[0]["DATEADDED"].ToString();
            }
            FillRequestDetail(id, ref model);
            return View(model);
        }

        [HttpPost]
        public IActionResult EditStationaryRequest(StationaryViewModel model)
        {
            try
            {
                Employee_Details LoginEmpDetails = _sessionService.Get<Employee_Details>("Employee");
                long userId = Convert.ToInt64(_sessionService.Get<string>("userID"));
                model.StationaryCart = _sessionService.Get<List<StationaryItem>>("STATIONARY_ITEMS");
                StringBuilder xmlStationaryList = new StringBuilder();
                if (model.StationaryCart != null && model.StationaryCart.Any())
                {
                    xmlStationaryList.Append("<STATIONARY>");
                    foreach (StationaryItem s in model.StationaryCart)
                    {

                        xmlStationaryList.Append("<STATIONARY>");
                        xmlStationaryList.Append("<STATIONARYID>" + s.Item + "</STATIONARYID>");
                        xmlStationaryList.Append("<QTY>" + s.Qty + "</QTY>");
                        xmlStationaryList.Append("<REMARKS>" + s.Remark + "</REMARKS>");
                        xmlStationaryList.Append("</STATIONARY>");
                    }
                    xmlStationaryList.Append("</STATIONARY>");
                }
                string strEmpCode = string.Empty;
                string strappauth = string.Empty;
                string strappname = string.Empty;
                string strappauthemailid = string.Empty;
                string strMobileNo = string.Empty;
                string strExtNo = string.Empty;
                string err = string.Empty;

                strEmpCode = userId.ToString();
                strappauth = model.ApproverId;
                strappname = model.ApproverName;
                strMobileNo = model.MobileNumber;
                strExtNo = model.ExtensionNumber;
                if (strappauth == "" || strExtNo == "")
                {
                    model.exceptionInfo.ErrorPanelVisiblity = true;
                    model.exceptionInfo.ErrorMessage = "Please Enter Extantion no and Select Approval Authorty";
                    model.exceptionInfo.ControlId = "txtExtension";
                }
                if (model.StationaryCart == null || !model.StationaryCart.Any())
                {
                    model.exceptionInfo.ErrorPanelVisiblity = true;
                    model.exceptionInfo.ErrorMessage = "Please Add at least one Stationary";
                    model.exceptionInfo.ControlId = "gvList";
                }
                //Post data when validation passed
                if (model.exceptionInfo.ErrorPanelVisiblity == false)
                {
                    err = _Stationary.UpdateStationaryReq(model.RequestId, strEmpCode, strMobileNo, strExtNo, xmlStationaryList.ToString());
                }

                // if error in updating the process, then display it
                if (err.Trim() != "")
                {
                    model.exceptionInfo.ErrorPanelVisiblity = true;
                    model.exceptionInfo.ErrorMessage = err;
                    model.exceptionInfo.ControlId = "ddlplant";
                }
                if (model.exceptionInfo.ErrorPanelVisiblity == false)
                {
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
                        message = model.exceptionInfo.ErrorMessage,
                        exception = ""
                    };
                    return Ok(result);
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

        public IActionResult CancelStationaryRequest(string id)
        {
            Employee_Details LoginEmpDetails = _sessionService.Get<Employee_Details>("Employee");
            long userId = Convert.ToInt64(_sessionService.Get<string>("userID"));
            StationaryViewModel model = new StationaryViewModel()
            {
                RequestId = id
            };

            DataTable dtHeader = FillRequestHeader(id);
            if (dtHeader != null && dtHeader.Rows.Count > 0)
            {
                model.InitiatorName = dtHeader.Rows[0]["EMPNAME"].ToString();
                model.MobileNumber = dtHeader.Rows[0]["MOBILENO"].ToString();
                model.ExtensionNumber = dtHeader.Rows[0]["EXTENTIONNO"].ToString();
                model.InitiatedDate = dtHeader.Rows[0]["DATEADDED"].ToString();
            }
            FillRequestDetail(id, ref model);
            return View(model);
        }

        [HttpPost]
        public IActionResult CancelStationaryRequest(StationaryViewModel model)
        {
            string strcancelremark = string.Empty;
            string RequestID = string.Empty;
            try
            {
                strcancelremark = model.Remarks;
                RequestID = model.RequestId;
                string UserCode = _sessionService.Get<string>("userID");
                string strResult = string.Empty;
                if (strcancelremark == "")
                {
                    model.exceptionInfo.ErrorPanelVisiblity = true;
                    model.exceptionInfo.ErrorMessage = "Please Enter Cancel Remarks";
                    model.exceptionInfo.ControlId = "txtCancellationRemarks";
                }
                if (model.exceptionInfo.ErrorPanelVisiblity == false)
                {
                    strResult = _Stationary.CancelStationaryRequest(RequestID, strcancelremark, UserCode);
                }

                if (strResult.Trim() != "")
                {
                    model.exceptionInfo.ErrorPanelVisiblity = true;
                    model.exceptionInfo.ErrorMessage = "Please Enter Cancel Remarks";
                    model.exceptionInfo.ControlId = "txtCancellationRemarks";

                    var result = new
                    {
                        status = 0,
                        message = model.exceptionInfo.ErrorMessage,
                        exception = ""
                    };
                    return Ok(result);
                }
                else
                {
                    var result = new
                    {
                        status = 1,
                        message = "Request Submitted Successfully",
                        exception = ""
                    };
                    return Ok(result);
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

        public IActionResult StationaryRequestDetail(string id)
        {
            Employee_Details LoginEmpDetails = _sessionService.Get<Employee_Details>("Employee");
            long userId = Convert.ToInt64(_sessionService.Get<string>("userID"));
            StationaryViewModel model = new StationaryViewModel();
            //IF TRANSACTION ID IS NOT NULL THEN
            if (!string.IsNullOrEmpty(id))
            {
                DataTable dt = new DataTable();
                dt = _Stationary.GetRequestDetail(id);
                if (dt != null && dt.Rows.Count > 0)
                {
                    model = new StationaryViewModel()
                    {
                        RequestId = dt.Rows[0]["ADSTYREQUESTID"].ToString(),
                        InitiatorName = dt.Rows[0]["EMPNAME"].ToString(),
                        InitiatedDate = dt.Rows[0]["DATEADDED"].ToString(),


                        ApproverName = dt.Rows[0]["APPNAME"].ToString(),
                        ApprovalDate = dt.Rows[0]["SUPERVISORAPPROVEDDATE"].ToString(),
                        ApproverEmail = dt.Rows[0]["APPEMAILID"].ToString(),
                        ApproverRemarks = dt.Rows[0]["SUPERVISORREMARKS"].ToString(),
                        ApprovalStatus = dt.Rows[0]["APPSTATUS"].ToString(),

                        AdminApprover = dt.Rows[0]["ADMINNAME"].ToString(),
                        AdminApprovalStatus = dt.Rows[0]["ADMINSTATUS"].ToString(),
                        AdminRemarks = dt.Rows[0]["ADMINREMARKS"].ToString(),
                        AdminApprovalDate = dt.Rows[0]["ADMINAPPROVALDATE"].ToString(),
                        AdminApproverEmail = dt.Rows[0]["ADMINEMAILID"].ToString()
                    };
                }
            }
            return View(model);
        }

        public IActionResult StationaryRequestApproval(string id)
        {
            Employee_Details LoginEmpDetails = _sessionService.Get<Employee_Details>("Employee");
            long userId = Convert.ToInt64(_sessionService.Get<string>("userID"));
            StationaryViewModel model = new StationaryViewModel()
            {
                RequestId = id
            };

            DataTable dtHeader = FillRequestHeader(id);
            if (dtHeader != null && dtHeader.Rows.Count > 0)
            {
                model.InitiatorName = dtHeader.Rows[0]["EMPNAME"].ToString();
                model.MobileNumber = dtHeader.Rows[0]["MOBILENO"].ToString();
                model.ExtensionNumber = dtHeader.Rows[0]["EXTENTIONNO"].ToString();
                model.InitiatedDate = dtHeader.Rows[0]["DATEADDED"].ToString();
            }
            FillRequestDetail(id, ref model);
            return View(model);
        }

        [HttpPost]
        public IActionResult StationaryRequestApproval(StationaryViewModel model)
        {
            try
            {
                string appstatus = string.Empty;
                string Remarks = string.Empty;
                string errMsg = string.Empty;
                string userId = _sessionService.Get<string>("userID");
                appstatus = model.ApprovalStatusId;
                Remarks = model.ApproverRemarks;

                errMsg = _Stationary.UpdateApprovalStatus(model.RequestId, appstatus, Remarks, userId);
                if (errMsg.Trim() == "")
                {
                    var result = new
                    {
                        status = 1,
                        message = "Request Submitted Successfully",
                        exception = ""
                    };
                    return Ok(result);
                }
                else
                {
                    model.exceptionInfo.ErrorPanelVisiblity = true;
                    model.exceptionInfo.ErrorMessage = "Please Enter Remarks";
                    model.exceptionInfo.ControlId = "txtAPPRemarks";

                    var result = new
                    {
                        status = 0,
                        message = model.exceptionInfo.ErrorMessage,
                        exception = ""
                    };
                    return Ok(result);
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

        public JsonResult GetStationaryMaster(string ItemId, string PlantId)
        {
            DataTable dt = _Stationary.Get_Stationarymaster(ItemId, "1", PlantId);
            if (dt.Rows.Count > 0)
            {
                var result = new
                {
                    MaxQtyVisiblity = true,
                    UOM = dt.Rows[0]["UOM"].ToString(),
                    MaxQty = dt.Rows[0]["MAXQTY"].ToString(),
                    MaxQtyLable = "Max request Value :- " + dt.Rows[0]["MAXQTY"].ToString(),
                };
                return Json(result);
            }
            else
            {
                var result = new
                {
                    MaxQtyVisiblity = false,
                    UOM = "",
                    MaxQty = "",
                    MaxQtyLable = "",
                };
                return Json(result);
            }
        }
        [HttpPost]
        public PartialViewResult AddItems(StationaryViewModel model)
        {
            model.StationaryCart = new List<StationaryItem>();
            //List<StationaryItem> objItems = new List<StationaryItem>();
            try
            {
                //Get Existing Items Added
                model.StationaryCart = _sessionService.Get<List<StationaryItem>>("STATIONARY_ITEMS");
                //Validaton
                //Validate Item
                if (model.StationeryItemId.ToString() == "-1")
                {
                    model.exceptionInfo.ErrorPanelVisiblity = true;
                    model.exceptionInfo.ErrorMessage = "Stationary Item is a mandatory field.";
                    model.exceptionInfo.ControlId = "ddlitem";
                    return PartialView("_PartialStationaryItem", model);
                }
                //Validate Qty
                if (model.Quantity.ToString() == "")
                {
                    model.exceptionInfo.ErrorPanelVisiblity = true;
                    model.exceptionInfo.ErrorMessage = "Quantity is a mandatory field.";
                    model.exceptionInfo.ControlId = "txtqty";
                    return PartialView("_PartialStationaryItem", model);
                }
                //Validate Qty with Max Qty
                if (model.Quantity > model.MaxQuantity)
                {
                    model.exceptionInfo.ErrorPanelVisiblity = true;
                    model.exceptionInfo.ErrorMessage = "Quantity should not be greater than " + model.MaxQuantity.ToString();
                    model.exceptionInfo.ControlId = "txtqty";
                    return PartialView("_PartialStationaryItem", model);
                }
                //Validate existing item
                if (model.StationaryCart.Any(x => x.Item == model.StationeryItemId.ToString()))
                {
                    model.exceptionInfo.ErrorPanelVisiblity = true;
                    model.exceptionInfo.ErrorMessage = "Stationary is already added";
                    model.exceptionInfo.ControlId = "gvList";
                    return PartialView("_PartialStationaryItem", model);
                }
                //Add new Item
                DataTable dtItemDescr = _Stationary.Get_Stationarymaster(model.StationeryItemId.ToString(), "", "0");
                model.StationaryCart.Add(new StationaryItem
                {
                    Item = model.StationeryItemId.ToString(),
                    ItemId = model.StationeryItemId.ToString(),
                    Qty = model.Quantity.ToString(),
                    Remark = model.Remarks,
                    ItemDescription = dtItemDescr.Rows[0]["DESCRIP"].ToString()
                });
                //Update Items
                _sessionService.Set<List<StationaryItem>>("STATIONARY_ITEMS", model.StationaryCart);
            }
            catch { }
            return PartialView("_PartialStationaryItem", model);
        }
        [HttpPost]
        public PartialViewResult DeleteItem(string ItemId)
        {
            StationaryViewModel model = new StationaryViewModel();
            try
            {
                //Get Existing Items Added
                model.StationaryCart = _sessionService.Get<List<StationaryItem>>("STATIONARY_ITEMS");
                if (model.StationaryCart != null && model.StationaryCart.Any())
                {
                    if (model.StationaryCart.Any(x => x.Item == ItemId))
                    {
                        model.StationaryCart.Remove(model.StationaryCart.Where(x => x.Item == ItemId).FirstOrDefault());
                    }
                    _sessionService.Set<List<StationaryItem>>("STATIONARY_ITEMS", model.StationaryCart);
                }
            }
            catch
            {
            }
            return PartialView("_PartialStationaryItem", model);
        }

        private void SendMail(string strApprovalAuthEmail, string strappname, StationaryViewModel model)
        {
            string userId = _sessionService.Get<string>("userID");
            string userName = _sessionService.Get<string>("userName");
            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";

            if (serverpath.isTestServer())
                sendMail.MailTo = serverpath.getTestEMail();
            else
                sendMail.MailTo = strApprovalAuthEmail;


            string strSubject = "Stationary Request from - " + userId + " (Employee Code: " + userName + ")";
            string strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                             "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                             "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                             "</td></tr>" +
                             "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                             "</td></tr><tr height=150><td colspan=2>" +
                             "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                             "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                             "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                             "<tr><td valign=top colspan=2><b> Dear " + strappname + " San </b></td></tr>" +
                             "<tr><td colspan=2>&nbsp;</td></tr>" +
                             "<tr><td colspan=2>&nbsp;</td></tr>" +
                             "<tr><td valign=top colspan=2><b>" + userName + " San has submitted a Stationry Request. The details are as follows:</b></td></tr>" +
                             "<tr><td colspan=2>&nbsp;</td></tr>" +
                             GetDynamincTable(model) +
                             "<tr><td colspan=2>&nbsp;</td></tr>" +
                             "<tr><td colspan=2>Please login <a href=" + serverpath.getServerPath() + "index.aspx> Employee Portal</a> for your approval.</td></tr>" +
                             "<tr valign=bottom><td colspan=2><b>Best Regards</b><br /> Team - EPortal<br /><br /><strong>Note: It is a system generated Email, please do not reply.</strong></td> " +
                             "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                             "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images//Table_layout_06.gif border=0 /></td> " +
                             "</tr></table> ";


            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            try
            {
                bool status = sendMail.Send();
            }
            catch (Exception ex)
            {
                //Response.Write("Exception Occured:   " + ex);
            }
            finally
            {
                //Response.Write("Your E-mail has been sent sucessfully");
            }
        }

        private string GetDynamincTable(StationaryViewModel model)
        {
            //DtTbl = objsty.GetRequestDetailPart(strRequestId);
            // create a string type variable to generate dynamic table
            string dynTable = "";
            int a = 1;

            // start with table tag with following attributes
            dynTable = "<table style=\"border-left:1px solid #C1DAD7;\" cellspacing=\"0\" cellpadding=\"2\" border=\"1\">";

            dynTable += "<tr><td width=45 style=\"background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;\">S.No</td>" +
                        "<td width=120 style=\"background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;\">Stationary</td>" +
                        "<td width=120 style=\"background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;\">Qty</td>" +
                        "<td width=120 style=\"background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;\">Remark</td>" +
                        "</tr>";
            int tRows = 0;
            // outer loop to generate table rows
            foreach (var item in model.StationaryCart)
            {
                //start table row
                dynTable += "<tr>";

                // inner loop to generate columns
                for (int tCols = 0; tCols < 4; tCols++)
                {
                    // create column
                    if ((2 + tRows) % 2 == 0)
                    {
                        dynTable += "<td style=\"background-color:#EDF3F3; border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;text-align:left;font-size:10px;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;\" valign=\"top\">";
                    }
                    else
                    {
                        dynTable += "<td style=\"background-color:#F5FAFA; border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;text-align:left;font-size:10px;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;\" valign=\"top\">";
                    }
                    if (tCols.ToString() == "0")
                    {
                        dynTable += a.ToString();
                        a++;
                    }
                    else if (tCols.ToString() == "1")
                    {
                        dynTable += item.ItemDescription;
                    }
                    else if (tCols.ToString() == "2")
                    {
                        dynTable += item.Qty;
                    }
                    else if (tCols.ToString() == "3")
                    {
                        dynTable += item.Remark;
                    }


                    // close td column tag
                    dynTable += "</td>";
                }

                // close table row
                dynTable += "</tr>";

                tRows++;
            }
            // close the table tag
            dynTable += "</table>";
            return dynTable;
        }

        private DataTable FillRequestHeader(string RequestID)
        {
            Employee_Details LoginEmpDetails = _sessionService.Get<Employee_Details>("Employee");
            string userId = _sessionService.Get<string>("userID");
            DataTable dt = new DataTable();
            string UserCode = userId;
            return _Stationary.GetRequestDetail(RequestID);
        }
        private StationaryViewModel FillRequestDetail(string RequestID, ref StationaryViewModel model)
        {
            DataTable dt = new DataTable();
            dt = _Stationary.GetRequestDetailPart(RequestID);
            for (int rownum = 0; rownum < dt.Rows.Count; rownum++)
            {
                DataTable dtItemDescr = _Stationary.Get_Stationarymaster(dt.Rows[rownum]["ADSTATIONARYID"].ToString(), "", "0");
                model.StationaryCart.Add(new StationaryItem
                {
                    Item = dt.Rows[rownum]["ADSTATIONARYID"].ToString(),
                    ItemId = dt.Rows[rownum]["ADSTATIONARYID"].ToString(),
                    Qty = dt.Rows[rownum]["REQUESTQTY"].ToString(),
                    Remark = dt.Rows[rownum]["REMARK"].ToString(),
                    ItemDescription = dtItemDescr.Rows[0]["DESCRIP"].ToString()
                });
            }
            _sessionService.Set<List<StationaryItem>>("STATIONARY_ITEMS", model.StationaryCart);
            return model;
        }
    }
}
