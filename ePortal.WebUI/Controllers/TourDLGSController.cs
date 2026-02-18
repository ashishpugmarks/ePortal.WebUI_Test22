using ePortal.Persistence;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.Persistence.TourRequest.Interface;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels.APPX.TourRequest;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;

namespace ePortal.WebUI.Controllers
{
    // [CSPFilter]
    public class TourDLGSController : Controller
    {
        private readonly ITourQueries oTourQueries;
        private readonly IFlightSchedule oFlightSchedule;
        private readonly IPMS oPMS;
        private readonly ISessionService _sessionService;
        private readonly IEportalESS objess;
        private readonly IAppConfigurationService _env;
        private readonly ICommonFunctions oCommFunctions;
        private readonly ILogger<TourDLGSController> _logger;
        public TourDLGSController(ILogger<TourDLGSController> logger, ITourQueries _oTourQueries, IFlightSchedule _oFlightSchedule, IPMS _oPMS, ISessionService sessionService, IAppConfigurationService appConfiguration, IEportalESS _objess, ICommonFunctions _objCommon)
        {
            _logger = logger;
            oTourQueries = _oTourQueries;
            oFlightSchedule = _oFlightSchedule;
            oPMS = _oPMS;
            _sessionService = sessionService;
            objess = _objess;
            _env = appConfiguration;
            oCommFunctions = _objCommon;
        }



        #region "AppCancellationRequest.aspx"
        #region "App Cancellation Request ASPX : Page Load Start"
        [HttpGet]
        public IActionResult AppCancellationRequest()
        {
            AppCancellationRequestViewModel model = new AppCancellationRequestViewModel();
            string id = Request.Query["id"].ToString();
            TempData["id"] = Request.Query["id"].ToString();
            model.lblErrMsg = "Ticket Cancellation Approval Request";
            model.lblRequestID = id;
            FillRequestDetail(id, model);
            return View(model);
        }
        #endregion "App Cancellation Request ASPX : Page Load End"


        #region "App Cancellation Request ASPX : Control Events Start"
        [HttpPost]
        public IActionResult btnCancel_Click_ACR([FromBody] dlgs_Cancellation val)// Changes Tour
        {
            string err = string.Empty;
            string DetailID = string.Empty;
            try
            {
                DetailID = TempData["id"].ToString();
                string Remarks = val.txtCalcellationRemarks;
                //APPROVE TICKET CANCELLATION BY APPROVING AUTHORITY
                err = oTourQueries.ApproveCanellationRequest(DetailID, Remarks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, (_sessionService.Get<string>("userID")).ToString(), ex.Message);
            }
            var response = new
            {
                err = err
            };

            return Json(new { result = response });
        }
        #endregion "App Cancellation Request ASPX : Control Events End"

        #region "App Cancellation Request ASPX : Functions Start"
        private void FillRequestDetail(string id, AppCancellationRequestViewModel model)
        {
            //GET BOOKING REQUEST DETAILS FOR APPROVING TICKET CANCELLATION 
            DataTable dt = oTourQueries.GetReqeustBookingDetails(id);
            if (dt.Rows.Count > 0)
            {
                model.lblTravelDate = dt.Rows[0]["TRAVELFROMDATE"].ToString();
                model.lblCityFrom = dt.Rows[0]["FRMCITY"].ToString();
                model.lblCityTo = dt.Rows[0]["TOCITY"].ToString();
                model.lblTicketNumber = dt.Rows[0]["TICKETNO"].ToString();
                model.lblTicketAmount = dt.Rows[0]["TICKETAMOUNT"].ToString();
                model.lblIssueDate = dt.Rows[0]["TICKETISSUEDATE"].ToString();
                model.lblHotelName = dt.Rows[0]["BOOKEDHOTELNAME"].ToString();
                model.lblHotelAddress = dt.Rows[0]["BOOKEDHOTELADDRESS"].ToString();
                model.lblCheckinDate = dt.Rows[0]["CHECKINDATE"].ToString();
                model.lblCheckinTime = dt.Rows[0]["CHECKINTIME"].ToString();
                model.lblCheckoutDate = dt.Rows[0]["CHECKOUTDATE"].ToString();
                model.lblCheckoutTime = dt.Rows[0]["CHECKOUTTIME"].ToString();
                model.lblPickUpDetails = dt.Rows[0]["PICKUPDROPDETAILS"].ToString();

            }
        }
        #endregion "App Cancellation Request ASPX : Functions End"
        #endregion "AppCancellationRequest.aspx"



        #region "FlightSchedule.aspx"
        #region "Flight Schedule ASPX : Page Load Start"
        [HttpGet]
        public IActionResult FlightSchedule()
        {
            int UserID = Convert.ToInt32(_sessionService.Get<string>("userID"));

            if (string.IsNullOrEmpty(UserID.ToString()))
            {
                return RedirectToAction("Login", "Account");
            }

            FlightScheduleViewModel model = new FlightScheduleViewModel();
            return View(model);
        }
        #endregion "Flight Schedule ASPX : Page Load End"


        #region "Flight Schedule ASPX : Control Events Start"   
        [HttpGet]
        public IActionResult FillFlightSchedule(string strId)
        {
            DataSet objds = new DataSet();
            try
            {
                objds = oFlightSchedule.GetFlightDetails();

                DataView dv = objds.Tables[0].DefaultView;
                dv.RowFilter = "Type='" + strId + "'";

                var dt = dv.ToTable();
                var response = JsonConvert.SerializeObject(dt);
                return Json(new { result = response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, (_sessionService.Get<string>("userID")).ToString(), ex.Message);

                return StatusCode(500, new { error = "An error occurred while fetching MovementDetailsSearch details." });
            }
        }
        #endregion "Flight Schedule ASPX : Control Events End"

        #region "Flight Schedule ASPX : Functions Start" 
        #endregion "Flight Schedule ASPX : Functions End"
        #endregion "FlightSchedule.aspx"



        #region "PrintableList.aspx"
        #region "Printable List ASPX : Page Load Start"
        [HttpGet]
        public IActionResult PrintableList()
        {
            PrintableListViewModel model = new PrintableListViewModel();
            DataTable odt = new DataTable();

            string UserID = (_sessionService.Get<string>("userID")).ToString();
            string GroupID = Request.Query["id"].ToString();
            string dupFlag = Request.Query["dup"].ToString();

            TempData["GroupID"] = Request.Query["id"].ToString();
            TempData["dupFlag"] = Request.Query["dup"].ToString();

            //GET TOUR REQUEST LIST FOR SELECTED PRINT GROUP NUMBER
            odt = oTourQueries.GetPrintGroupList(UserID, GroupID);

            if (odt != null && odt.Rows.Count > 0)
            {
                odt.Columns.Add("StayCity", typeof(string));
                foreach (DataRow row in odt.Rows)
                {
                    row["StayCity"] = GetStayingCity(row["ADTOURREQUESTID"].ToString());
                }
            }
            model.PrintGroupList = odt;
            model.lblGroupNumber = GroupID;
            model.lblDate = DateTime.Today.ToString("dd-MM-yyyy");

            //GET DPT AND OP DETAILS
            string ActiveKI = oPMS.GetKIId();
            DataTable dt = new DataTable();
            dt = oCommFunctions.GetEmployeeOfficialDetails(UserID, ActiveKI);
            if (dt.Rows.Count > 0)
            {
                model.lblDepartment = dt.Rows[0]["DEPARTMENT"].ToString();
                model.lblOperation = dt.Rows[0]["OPERATION"].ToString();
            }

            //FILL DPT CORDINATOR DETAILS 
            dt = new DataTable();
            int UserCode;
            UserCode = Convert.ToInt32((_sessionService.Get<string>("userID")));
            dt = oTourQueries.EmployeeDetail(UserCode);
            if (dt.Rows.Count > 0)
            {
                model.lblMobileNo = dt.Rows[0]["TMOBILE"].ToString();
                model.lblExt = dt.Rows[0]["EXTENSIONNO"].ToString();
            }
            model.lblName = oTourQueries.GetEmpName(UserCode.ToString());

            //SET DUPLICATE FLAG
            if (dupFlag.Trim().ToUpper() == "Y")
                model.lblduplicate = "Duplicate Copy";
            else
                model.lblduplicate = "";



            return View(model);
        }
        #endregion "Printable List ASPX : Page Load End"


        #region "Printable List ASPX : Control Events Start"

        #endregion "Printable List ASPX : Control Events End"

        #region "Printable List ASPX : Functions Start"
        protected string GetStayingCity(string RequestID)
        {
            //GET FIRST STAYING CITY FOR TOUR
            return (oTourQueries.GetStayingCity(RequestID));

        }
        #endregion "Printable List ASPX : Functions End"
        #endregion "PrintableList.aspx"



        #region "RequestCancellation.aspx"
        #region "Request Cancellation ASPX : Page Load Start"
        [HttpGet]
        public IActionResult RequestCancellation()
        {
            AppCancellationRequestViewModel model = new AppCancellationRequestViewModel();
            string id = Request.Query["id"].ToString();
            TempData["id"] = Request.Query["id"].ToString();
            model.lblErrMsg = "Ticket Cancellation Approval Request";
            model.lblRequestID = id;
            FillRequestDetail(id, model);
            return View(model);
        }
        #endregion "Request Cancellation ASPX : Page Load End"


        #region "Request Cancellation ASPX : Control Events Start"
        [HttpPost]
        public IActionResult btnCancel_Click_RC([FromBody] dlgs_CancelTour obj)
        {
            string txtCalcellationRemarks = obj.txtCalcellationRemarks,  lblTicketNumber = obj.lblTicketNumber;
            string lblTravelDate = obj.lblTravelDate, lblCityFrom = obj.lblCityFrom, lblCityTo = obj.lblCityTo;
            string err = string.Empty;
            try
            {
                string DetailID = TempData["id"].ToString();
                string RequestID = TempData["RequestID"].ToString();//Tour Changes
                string Remarks = txtCalcellationRemarks;


                err = oTourQueries.CancelTicketRequest(DetailID, Remarks);
                if (err.Trim() == "")
                {
                    sendmail_RC(RequestID, lblTravelDate, lblCityFrom, lblCityTo, lblTicketNumber, txtCalcellationRemarks);
                    int TxnNumber;
                    TxnNumber = oTourQueries.CheckRequestEmailsStatus(RequestID);
                    if (TxnNumber == 1)
                    {
                        SendmailTravelDesk_RC(RequestID, lblTravelDate, txtCalcellationRemarks);
                        SendmailHotelDesk_RC(RequestID, lblTravelDate, txtCalcellationRemarks);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, (_sessionService.Get<string>("userID")).ToString(), ex.Message);
            }
            var response = new
            {
                err = err
            };

            return Json(new { result = response });
        }
        #endregion "Request Cancellation ASPX : Control Events End"

        #region "Request Cancellation ASPX : Functions Start"
        //SEND MAIL TO ALL APPROVING AUTHORITY WHEN USER CANCEL THE REQUST
        private void sendmail_RC(string RequestID, string lblTravelDate, string lblCityFrom, string lblCityTo, string lblTicketNumber, string txtCalcellationRemarks)
        {
            string UserID = (_sessionService.Get<string>("userID")).ToString();
            string UserName = (_sessionService.Get<string>("userName")).ToString();
            string strSubject = string.Empty;
            string strBody = string.Empty;
            string strAppAuthEmail = string.Empty;
            string strcc = string.Empty;
            commanEmail sendMail = new commanEmail();
            DataTable dt = new DataTable();

            //GET APPROVAL AUTHORITIES EMAILS
            dt = oTourQueries.GetRequestEmails(RequestID);
            string FirstAppAuthEmail = string.Empty;
            string SecondAppAuthEmail = string.Empty;
            string FinAppAuthEmail = string.Empty;
            string AdminAppAuthEmail = string.Empty;

            if (dt.Rows.Count > 0)
            {
                FirstAppAuthEmail = dt.Rows[0]["RECEMAIL"].ToString();
                SecondAppAuthEmail = dt.Rows[0]["APPEMAIL"].ToString();
                FinAppAuthEmail = dt.Rows[0]["FINEMAIL"].ToString();
                AdminAppAuthEmail = dt.Rows[0]["ADMINEMAIL"].ToString();

            }
            if (FirstAppAuthEmail.Trim() != "")
                strAppAuthEmail = FirstAppAuthEmail;

            if (SecondAppAuthEmail.Trim() != "")
            {
                if (strAppAuthEmail.Trim() != "")
                    strAppAuthEmail = strAppAuthEmail + "," + SecondAppAuthEmail;
                else
                    strAppAuthEmail = SecondAppAuthEmail;
            }
            if (AdminAppAuthEmail.Trim() != "")
                strcc = AdminAppAuthEmail;

            if (strAppAuthEmail != "" || strcc != "")
            {
                sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                if (serverpath.isTestServer())
                    sendMail.MailTo = serverpath.getTestEMail();
                else
                {
                    if (strAppAuthEmail != "")
                    {
                        sendMail.MailTo = strAppAuthEmail;
                        if (strcc != "")
                            sendMail.MailCc = strcc;
                    }
                    else
                        sendMail.MailTo = strcc;
                }

                strSubject = "Ticket Cancellation Request from - " + UserName + "[ " + UserID + " ]";
                strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                                 "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                                 "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                 "</td></tr>" +
                                 "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                                 "<b>Tour Request</b></td></tr><tr height=150><td colspan=2>" +
                                 "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                 "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                 "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                 "<tr><td colspan=3>&nbsp;</td></tr><tr><td valign=top colspan=2><p><b>Dear San,</b><br />" +
                                 "<br />" + UserName +
                                 " has Cancel a Ticket request, the details are as follows:<br />" +
                                 "<br />" +
                                 "<tr><td width=125 height=23 valign=top>Travel Date:</td><td width=389 valign=top>" + lblTravelDate + "</td>" +
                                 "</tr><tr><td width=125 height=23 valign=top>From City:</td><td width=389 valign=top>" + lblCityFrom + "</td>" +
                                 "</tr><tr><td width=125 height=23 valign=top>To City:</td><td width=389 valign=top>" + lblCityTo + "</td>" +
                                 "</tr><tr><td width=125 height=23 valign=top>Ticket Number:</td><td width=389 valign=top>" + lblTicketNumber + "</td>" +
                                 "</tr><tr><td width=125 height=23 valign=top>Reason of Cancellation:</td><td width=389 valign=top>" + txtCalcellationRemarks + "</td></tr>" +
                                 "<tr><td>&nbsp;</td></tr>" +
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
                    Console.WriteLine($"Exception Occurred: {ex}");
                    _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);
                }
            }
        }


        protected void SendmailTravelDesk_RC(string RequestID, string lblTravelDate, string txtCalcellationRemarks)
        {
            string UserID = (_sessionService.Get<string>("userID")).ToString();
            string UserName = (_sessionService.Get<string>("userName")).ToString();
            string strSubject = string.Empty;
            string strBody = string.Empty;
            string strTravelDeskEmail = string.Empty;
            string strcc = string.Empty;
            commanEmail sendMail = new commanEmail();
            DataTable dt = new DataTable();
            strTravelDeskEmail = oCommFunctions.GetParameterValue("TRAVEL_DESK_MAIL");

            if (strTravelDeskEmail != "")
            {
                sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                if (serverpath.isTestServer())
                    sendMail.MailTo = serverpath.getTestEMail();
                else
                {
                    if (strTravelDeskEmail != "")
                    {
                        sendMail.MailTo = strTravelDeskEmail;
                    }

                }
                strSubject = "Travel Cancellation Request from - " + UserName + "[ " + UserID + " ]";
                strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                                 "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                                 "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                 "</td></tr>" +
                                 "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                                 "<b>Tour Request</b></td></tr><tr height=150><td colspan=2>" +
                                 "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                 "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                 "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                 "<tr><td colspan=3>&nbsp;</td></tr><tr><td valign=top colspan=2><p><b>Dear San,</b><br />" +
                                 "<br />" + UserName +
                                 "  has Cancel a Tour request, the details are as follows:<br />" +
                                 "<br />" +
                                 "<tr><td width=125 height=23 valign=top>Request Id:</td><td width=389 valign=top>" + RequestID + "</td>" +
                                 "</tr><tr><td width=125 height=23 valign=top>Tour Period:</td><td width=389 valign=top>" + lblTravelDate + "</td>" +
                                 "</tr><tr>" +
                                 "<td width=125 height=23 valign=top>Reason of Cancellation:</td><td width=389 valign=top>" + txtCalcellationRemarks + "</td></tr>" +
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
                    Console.WriteLine($"Exception Occurred: {ex}");
                    _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);
                }
            }
        }

        protected void SendmailHotelDesk_RC(string RequestID, string lblTravelDate, string txtCalcellationRemarks)
        {
            string UserID = (_sessionService.Get<string>("userID")).ToString();
            string UserName = (_sessionService.Get<string>("userName")).ToString();
            string strSubject = string.Empty;
            string strBody = string.Empty;
            string strHotelDeskEmail = string.Empty;
            string strcc = string.Empty;
            commanEmail sendMail = new commanEmail();
            DataTable dt = new DataTable();
            strHotelDeskEmail = oCommFunctions.GetParameterValue("HOTEL_DESK_MAIL");

            if (strHotelDeskEmail != "")
            {
                sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                if (serverpath.isTestServer())
                    sendMail.MailTo = serverpath.getTestEMail();
                else
                {
                    if (strHotelDeskEmail != "")
                    {
                        sendMail.MailTo = strHotelDeskEmail;

                    }

                }

                strSubject = "Hotel Cancellation Request from - " + UserName + "[ " + UserID + " ]";
                strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                                 "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                                 "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                 "</td></tr>" +
                                 "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                                 "<b>Tour Request</b></td></tr><tr height=150><td colspan=2>" +
                                 "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                 "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                 "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                 "<tr><td colspan=3>&nbsp;</td></tr><tr><td valign=top colspan=2><p><b>Dear San,</b><br />" +
                                 "<br />" + UserName +
                                 " has Cancel a Tour request, the details are as follows:<br />" +
                                 "<br />" +
                                 "<tr><td width=125 height=23 valign=top>Request Id:</td><td width=389 valign=top>" + RequestID + "</td>" +
                                 "</tr><tr><td width=125 height=23 valign=top>Tour Date:</td><td width=389 valign=top>" + lblTravelDate + "</td>" +

                                 "</tr><tr>" +
                                 "<td width=125 height=23 valign=top>Reason of Cancellation:</td><td width=389 valign=top>" + txtCalcellationRemarks + "</td></tr>" +
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
                    Console.WriteLine($"Exception Occurred: {ex}");
                    _logger.LogError(ex,
                    "Error in Method: {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}",
                    MethodBase.GetCurrentMethod()?.Name, UserID, ex.Message);
                }
            }
        }

        #endregion "Request Cancellation ASPX : Functions End"
        #endregion "RequestCancellation.aspx"




        #region "RequestForm.aspx"
        #region "Request Form ASPX : Page Load Start"
        [HttpGet]
        public IActionResult RequestForm()
        {
            ArrayList oTourList = new ArrayList();
            RequestFormViewModel model = new RequestFormViewModel();
            TempData["TourList"] = oTourList;

            //FILL TOUR HEADER AND DETAILS
            string RequestID = Request.Query["id"].ToString();
            FillRequestHeader_RF(RequestID, model);
            FillRequestDetail_RF(RequestID, model);

            return View(model);
        }
        #endregion "Request Form ASPX : Page Load End"


        #region "Request Form ASPX : Control Events Start"

        #endregion "Request Form ASPX : Control Events End"

        #region "Request Form ASPX : Functions Start"
        //CHECK THE DA CALCULATED OF SELECTED DATE  OR NOT
        private Boolean DailyAllowanceCaluculation_RF(List<string> ProcessDates, string TravelDate)
        {
            foreach (string str in ProcessDates)
            {
                //PROCESSED
                if (str == TravelDate)
                    return true;
            }
            //NOT PROCESSED
            return false;
        }

        //CHECK THE STAY CHARGE CALCULATED OF SELECTED DATE  OR NOT
        private Boolean StayChargeCaluculation_RF(List<string> ProcessNights, string TravelDate, string StayingCity)
        {
            foreach (string str in ProcessNights)
            {
                //PROCESSED AND STAYING CITY EXIST
                if (str == TravelDate && StayingCity.Trim() != "")
                    return true;
            }
            //NOT PROCESSED
            return false;
        }

        private void CalculateDayNights_RF(RequestFormViewModel model)
        {
            string CityCode;
            string StayingCity = string.Empty;
            int rownum;
            int days;
            int nights;
            int chargeAP = 0;
            int chargeA = 0;
            int chargeB = 0;
            int chargeC = 0;
            int TotalNights;

            int allowAP = 0;
            int allowA = 0;
            int allowB = 0;
            int allowC = 0;
            int TotalDays;
            int EmpDesignationID;

            double NightCharge = 0;
            double TotalNightCharge = 0;
            double DailyAllowance = 0;
            double TotalDailyAllowance = 0;

            //CHECK STAY CHARGE/DAILY ALLOWANCE WHEN TOUR ADVANCE IS SELECTED
            if (model.chkAdvance == true && model.gvList.Any())
            {
                //SHOW ADVANCE PANEL
                model.pnlAdvance = true;
                string TravelDate = string.Empty;
                string CityCategory = string.Empty;
                int CityCategoryCode = 0;
                Boolean isDayCalculated = false;
                Boolean isNightCalculated = false;
                Tour oTour = new Tour();
                ArrayList oTourList = (ArrayList)TempData["TourList"];
                List<string> ProcessDates = new List<string>();
                List<string> ProcessNights = new List<string>();

                for (rownum = 0; rownum < oTourList.Count; rownum++)
                {
                    oTour = (Tour)oTourList[rownum];
                    days = 1;
                    nights = 1;
                    //GET TRAVEL DATE
                    TravelDate = oTour.TourFromDate;
                    //CHECK DATE ALREADY PROCESSED OR NOT
                    isDayCalculated = DailyAllowanceCaluculation_RF(ProcessDates, TravelDate);
                    if (isDayCalculated == true)
                    {
                        days = 0;
                    }
                    else
                    {
                        ProcessDates.Add(TravelDate);
                    }

                    //GET STAYING CITY CODE
                    CityCode = oTour.StayingLocCode;
                    StayingCity = oTour.StayingLoc.Trim();

                    //CHECK NIGHT ALREADY PROCESSED OR NOT
                    isNightCalculated = StayChargeCaluculation_RF(ProcessNights, TravelDate, StayingCity);
                    if (isNightCalculated == true)
                    {
                        nights = 0;
                    }
                    else
                    {
                        ProcessNights.Add(TravelDate);
                    }
                    //IF NO STAYING CITY IS THERE
                    if (StayingCity == "")
                    {
                        nights = 0;
                        //DAILY ALLOWANCES CALCULATED ON TO CITY CODE
                        CityCode = oTour.ToLocCode;
                    }
                    //IF CITYCODE IS 0 IE OTHER THEN SET IT TO BLANK BECAUSE IS DOESN'T STORE IN DATABASE
                    if (CityCode == "0")
                        CityCode = "";

                    //GET CATEGORY DESCRIPTION OF CITY (RETURN C IN CASE OF OTHER CITY(CITYCODE=0)
                    DataRow[] _datarow;
                    _datarow = oTourQueries.GetCityCategory(CityCode);
                    if (_datarow.Length > 0)
                    {
                        CityCategory = _datarow[0]["DESCRIP"].ToString();
                        CityCategoryCode = System.Convert.ToInt16(_datarow[0]["SYCITYCATEGORYID"]);
                    }

                    EmpDesignationID = System.Convert.ToInt16(TempData["EmpDesig"]);
                    string UserType = Convert.ToInt64(TempData["ADEMPCODE"]) > 70000000 ? "EXPAT" : "LOCAL";

                    //CALCULATE NIGHT CHARGE AND DAILY ALLOWANCE CITY WISE
                    _datarow = oTourQueries.GetEmployeeAllowanceDetail(CityCategoryCode, EmpDesignationID, TempData["AppDate"].ToString(), UserType);
                    if (_datarow.Length > 0)
                    {
                        NightCharge = System.Convert.ToDouble(_datarow[0]["LODGINGAMT"]);
                        DailyAllowance = System.Convert.ToDouble(_datarow[0]["DAILYALLOWANCEAMT"]);
                    }
                    TotalDailyAllowance += (DailyAllowance * days);
                    TotalNightCharge += (NightCharge * nights);

                    switch (CityCategory)
                    {
                        case "A+":
                            chargeAP += nights;
                            allowAP += days;
                            break;
                        case "A":
                            chargeA += nights;
                            allowA += days;
                            break;
                        case "B":
                            chargeB += nights;
                            allowB += days;
                            break;
                        case "C":
                            chargeC += nights;
                            allowC += days;
                            break;
                    }
                }
                TotalNights = chargeAP + chargeA + chargeB + chargeC;
                TotalDays = allowAP + allowA + allowB + allowC;

                model.lblStayChargeAPLUS = chargeAP.ToString();
                model.lblAllownceAPLUS = allowAP.ToString();
                model.lblStayChargeA = chargeA.ToString();
                model.lblAllownceA = allowA.ToString();
                model.lblStayChargeB = chargeB.ToString();
                model.lblAllownceB = allowB.ToString();
                model.lblStayChargeC = chargeC.ToString();
                model.lblAllownceC = allowC.ToString();

                model.lblTotalNights = TotalNights.ToString();
                model.lblTotalDays = TotalDays.ToString();

                //CALCULATE TOTAL ALLOWANCES AND CHARGE
                model.lblTotalAllowances = string.Format("{0:F2}", TotalDailyAllowance);
                model.lblTotalCharges = string.Format("{0:F2}", TotalNightCharge);
            }
            else
            {
                //HIDE ADVANCE PANEL
                model.pnlAdvance = false;
            }
            double MiscAmount = 0;
            MiscAmount = System.Convert.ToDouble(model.lblMiscAmount);
            model.lblMiscAmount = string.Format("{0:F2}", MiscAmount);

            model.lblTotalAmount = string.Format("{0:F2}", (TotalDailyAllowance + TotalNightCharge + MiscAmount));
        }

        private void FillRequestHeader_RF(string RequestID, RequestFormViewModel model)
        {
            DataTable dt = new DataTable();
            string AdvRequired = string.Empty;
            string AdminStatus = string.Empty;

            string UserCode = (_sessionService.Get<string>("userID")).ToString();
            string APPFlag = string.Empty;
            dt = oTourQueries.GetRequestHeaderPart(RequestID, UserCode);

            model.lblPageRequestID = RequestID;
            if (dt.Rows.Count > 0)
            {
                TempData["EmpDesig"] = dt.Rows[0]["ADDESIGNATIONID"].ToString();
                TempData["AppDate"] = dt.Rows[0]["APPLICATIONDATE"].ToString();
                TempData["ADEMPCODE"] = Convert.ToString(dt.Rows[0]["ADEMPCODE"]);

                model.lblEmpName = dt.Rows[0]["EMPNAME"].ToString();
                model.txtMobile = dt.Rows[0]["MOBILENO"].ToString();
                model.txtExtension = dt.Rows[0]["EXTNNO"].ToString();
                model.txtObjective = dt.Rows[0]["OBJOFJOURNEY"].ToString();
                AdvRequired = dt.Rows[0]["ISADVANCEREQ"].ToString();
                model.lblMiscAmount = dt.Rows[0]["MISCELLANEOUSAMT"].ToString();

                APPFlag = dt.Rows[0]["APPFLAG"].ToString();
                AdminStatus = dt.Rows[0]["ADMINSTATUS"].ToString();

                model.lblDesignation = dt.Rows[0]["DESIGNATION"].ToString();
                model.lblOperation = dt.Rows[0]["OPERATION"].ToString();
                model.lblDivision = dt.Rows[0]["DIVISION"].ToString();
                model.lblDepartment = dt.Rows[0]["DEPARTMENT"].ToString();
                model.lblSection = dt.Rows[0]["SECTION"].ToString();
                model.lblRequiredAmount = string.Format("{0:F2}", Convert.ToDouble(dt.Rows[0]["TOTALAMOUNTREQUIRED"]));

                //SET APPROVAL TYPE
                model.hfAuthType = APPFlag;
                if (AdvRequired == "1")
                {
                    model.chkAdvance = true;
                    model.pnlAdvance = true;
                }
                else
                {
                    model.chkAdvance = false;
                    model.pnlAdvance = false;
                }

            }
        }

        private void FillRequestDetail_RF(string RequestID, RequestFormViewModel model)
        {
            ArrayList oTourList = new ArrayList();
            oTourList = oTourQueries.GetRequestDetailPartForAdminApproval(RequestID);
            TempData["TourList"] = oTourList;

            DataTableConverter dtList = new DataTableConverter();
            DataTable dt = dtList.ArrayListToDataTable<Tour>(oTourList);

            model.gvList = dtList.TableToList<gvListRF>(dt);

            //CALCULATE STAY CHARGE AND DAILY ALLOWANCES
            CalculateDayNights_RF(model);
        }
        #endregion "Request Form ASPX : Functions End"
        #endregion "RequestForm.aspx"
    }
}
