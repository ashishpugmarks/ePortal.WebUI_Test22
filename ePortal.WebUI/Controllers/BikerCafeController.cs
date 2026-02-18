using ePortal.Shared;
using ePortal.ViewModels;
using System.Data;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using ePortal.Application.Contracts;
using Microsoft.AspNetCore.Mvc.Rendering;
using ePortal.Shared.Interface;
using ePortal.WebUI.Filters;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    [SessionTimeout]
    public class BikerCafeController : Controller
    {
        private readonly IBikerCafeService _bcService;
        private readonly ISessionService _sessionService;
        private readonly ILogger<HomeController> _logger;
        private readonly IBikerCafe_DAL _oBikercafe;
        private readonly IEportalESS _ePortalESS;
        public BikerCafeController(IBikerCafeService bcService, ISessionService sessionService, ILogger<HomeController> logger, IBikerCafe_DAL oBikercafe, IEportalESS ePortalESS)
        {
            _bcService = bcService;
            _sessionService = sessionService;
            _logger = logger;
            _oBikercafe = oBikercafe;
            _ePortalESS = ePortalESS;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> MealBookingHistory()
        {
            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}


            List<MealTypeViewModel> _MealTypeList = await _bcService.GetMealTypeList(1);
            ViewBag.MEALTYPES = new SelectList(_MealTypeList, "BC_MEALTYPEID", "MEAL_TYPE_DESC");

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            ViewBag.strFromDate = startDate.ToString("dd-MMM-yyyy");
            ViewBag.strToDate = endDate.ToString("dd-MMM-yyyy");
            ViewBag.mealType = "";

            List<MealBookingTrnViewModel> iList = await _bcService.GetBookedMealList(0, startDate.ToString("dd-MMM-yyyy"), endDate.ToString("dd-MMM-yyyy"), Convert.ToInt64(_sessionService.Get<string>("userID")));
            return View(iList);

        }

        [HttpPost]
        public async Task<ActionResult> MealBookingHistory(long? typeId, string fromDate, string toDate)
        {
            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            List<MealTypeViewModel> _MealTypeList = await _bcService.GetMealTypeList(1);
            ViewBag.MEALTYPES = new SelectList(_MealTypeList, "BC_MEALTYPEID", "MEAL_TYPE_DESC");

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            ViewBag.strFromDate = !string.IsNullOrEmpty(fromDate) ? fromDate : startDate.ToString("dd-MMM-yyyy");
            ViewBag.strToDate = !string.IsNullOrEmpty(toDate) ? toDate : endDate.ToString("dd-MMM-yyyy");
            ViewBag.mealType = typeId;

            List<MealBookingTrnViewModel> iList = await _bcService.GetBookedMealList(Convert.ToInt64(typeId), fromDate, toDate, Convert.ToInt64(_sessionService.Get<string>("userID")));
            return View(iList);
        }

        [HttpGet]
        public async Task<ActionResult> ManageMealBooking()
        {
            MealBookingTrnViewModel model = new MealBookingTrnViewModel();
            try
            {
                //if (_sessionService.Get<string>("userID") == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                List<MealTypeViewModel> _MealTypeList = await _bcService.GetMealTypeList(1);
                ViewBag.MEALTYPES = new SelectList(_MealTypeList, "BC_MEALTYPEID", "MEAL_TYPE_DESC");

                int mealBookingDuration = 0; int mealBookingday = 0;
                BC_VALIDATION_ViewModel validModel = await _bcService.GetValidationData();
                mealBookingDuration = string.IsNullOrEmpty(validModel.MEAL_BOOKING_DURATION) ? 0 : Convert.ToInt32(validModel.MEAL_BOOKING_DURATION);
                mealBookingday = string.IsNullOrEmpty(validModel.MEAL_BOOKING_DAY) ? 0 : Convert.ToInt32(validModel.MEAL_BOOKING_DAY);

                ViewBag.MEAL_BOOKING_DURATION = mealBookingDuration;
                ViewBag.MEAL_BOKING_DAY = mealBookingday;

                //model.strBookingFrom = DateTime.Now.AddDays(mealBookingday).ToString("dd-MMM-yyyy");
                model.strBookingFrom = DateTime.Now.ToString("dd-MMM-yyyy");
                model.strBookingTo = DateTime.Now.AddDays(mealBookingDuration).ToString("dd-MMM-yyyy");
            }
            catch (Exception ex)
            {
                model = new MealBookingTrnViewModel();
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ManageMealBooking([FromBody] MealBookingTrnViewModel model)
        {
            try
            {
                //if (_sessionService.Get<string>("userID") == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                List<MealSlotViewModel> _slotList = await _bcService.GetMealSlots();
                ViewBag.Slots = _slotList;

                List<MealBookingTrnViewModel> modelList = await _bcService.GetAvailabilityMealList(model.strBookingFrom, model.strBookingTo, model.MEALTYPEID, Convert.ToInt64(_sessionService.Get<string>("userID")));
                return PartialView("_AvailabilityList", modelList);
            }
            catch (Exception ex)
            {
                return Json("error");
            }
        }

        [HttpPut]
        public async Task<ActionResult> ManageMealBooking([FromBody] List<MealBookingTrnViewModel> model)
        {
            short retVal = 0;
            string msg = "";
            try
            {
                //if (_sessionService.Get<string>("userID") == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                if (model == null || model.Count == 0) retVal = -1;
                else
                {
                    Tuple<short, string> _tuple = await _bcService.SaveMealBooking(Convert.ToInt64(_sessionService.Get<string>("userID")), model);
                    retVal = _tuple.Item1;
                    msg = _tuple.Item2;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(new { val = retVal, msg = msg });
        }

        [HttpPost]
        public async Task<ActionResult> SaveManageMealBooking([FromBody] List<MealBookingTrnViewModel> model)
        {
            short retVal = 0;
            string msg = "";
            try
            {
                //if (_sessionService.Get<string>("userID") == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                if (model == null || model.Count == 0) retVal = -1;
                else
                {
                    Tuple<short, string> _tuple = await _bcService.SaveMealBooking(Convert.ToInt64(_sessionService.Get<string>("userID")), model);
                    retVal = _tuple.Item1;
                    msg = _tuple.Item2;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(new { val = retVal, msg = msg });
        }

        [HttpPost]
        public async Task<ActionResult> CancelBooking(long id)
        {
            short retVal = 0;
            try
            {
                //if (_sessionService.Get<string>("userID") == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                if (id == 0) retVal = -1;
                else retVal = await _bcService.CancelBooking(id, Convert.ToInt64(_sessionService.Get<string>("userID")));
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }


        public async Task<ActionResult> SubsidizedMealToken()
        {
            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            //BikerCafe oBikercafe = new BikerCafe();
            DataTable dt = new DataTable();
            List<SubsidizedMealTokenViewModel> iList = new List<SubsidizedMealTokenViewModel>();

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            ViewBag.strFromDate = startDate.ToString("dd-MMM-yyyy");
            ViewBag.strToDate = endDate.ToString("dd-MMM-yyyy");
            ViewBag.tokenCode = "";
            int IsMealBooked = 0; int IsMealTime = 0;
            List<MealBookingTrnViewModel> modelList = await _bcService.GetAvailabilityMealList(DateTime.Now.ToString("dd-MMM-yyyy"), DateTime.Now.ToString("dd-MMM-yyyy"), 0, Convert.ToInt64(_sessionService.Get<string>("userID")));

            if (modelList.Where(x => x.MEAL_STATUS == 1).ToList().Count > 0)
            {
                IsMealBooked = 1;
            }
            ViewBag.IsMealBooked = IsMealBooked;
            DateTime currentDate = DateTime.Now;
            BC_VALIDATION_ViewModel validModel = await _bcService.GetValidationData();
            string bookingTime = string.IsNullOrEmpty(validModel.ALACARTE_BOOKING_TIME) ? string.Empty : (validModel.ALACARTE_BOOKING_TIME);
            DateTime ValidDateTime = DateTime.ParseExact(currentDate.ToString("dd-MMM-yyyy") + " " + bookingTime, "dd-MMM-yyyy HH:mm:ss", null);
            if (currentDate > ValidDateTime)
            {
                IsMealTime = 1;
            }
            ViewBag.IsMealTime = IsMealTime;

            dt = _oBikercafe.SubsidizedMealTokenList("", Convert.ToInt64(_sessionService.Get<string>("userID")), "", startDate.ToString("dd-MMM-yyyy"), endDate.ToString("dd-MMM-yyyy"));
            if (dt.Rows.Count > 0)
            {
                iList = GetSubsidizedMealTokenList(dt);
            }
            return View(iList);
        }

        [HttpPost]
        public async Task<ActionResult> SubsidizedMealToken(string tokenCode, string fromDate, string toDate)
        {
            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            //BikerCafe oBikercafe = new BikerCafe();
            DataTable dt = new DataTable();
            List<SubsidizedMealTokenViewModel> iList = new List<SubsidizedMealTokenViewModel>();

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            ViewBag.strFromDate = !string.IsNullOrEmpty(fromDate) ? fromDate : startDate.ToString("dd-MMM-yyyy");
            ViewBag.strToDate = !string.IsNullOrEmpty(toDate) ? toDate : endDate.ToString("dd-MMM-yyyy");
            ViewBag.tokenCode = tokenCode;
            int IsMealBooked = 0; int IsMealTime = 0;
            List<MealBookingTrnViewModel> modelList = await _bcService.GetAvailabilityMealList(DateTime.Now.ToString("dd-MMM-yyyy"), DateTime.Now.ToString("dd-MMM-yyyy"), 0, Convert.ToInt64(_sessionService.Get<string>("userID")));

            if (modelList.Where(x => x.MEAL_STATUS == 1).ToList().Count > 0)
            {
                IsMealBooked = 1;
            }
            ViewBag.IsMealBooked = IsMealBooked;
            DateTime currentDate = DateTime.Now;
            BC_VALIDATION_ViewModel validModel = await _bcService.GetValidationData();
            string bookingTime = string.IsNullOrEmpty(validModel.ALACARTE_BOOKING_TIME) ? string.Empty : (validModel.ALACARTE_BOOKING_TIME);
            DateTime ValidDateTime = DateTime.ParseExact(currentDate.ToString("dd-MMM-yyyy") + " " + bookingTime, "dd-MMM-yyyy HH:mm:ss", null);
            if (currentDate > ValidDateTime)
            {
                IsMealTime = 1;
            }
            ViewBag.IsMealTime = IsMealTime;

            dt = _oBikercafe.SubsidizedMealTokenList(tokenCode, Convert.ToInt64(_sessionService.Get<string>("userID")), "", fromDate, toDate);
            if (dt.Rows.Count > 0)
            {
                iList = GetSubsidizedMealTokenList(dt);
            }
            return View(iList);
        }

        [HttpPut]
        public ActionResult SubsidizedMealToken([FromBody] SubsidizedMealTokenViewModel model)
        {
            short retVal = 0;
            string msg = "";
            try
            {
                //if (_sessionService.Get<string>("userID") == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                retVal = InsertSubsidizedData(model);
                if (retVal == 4) //// token already exist.
                {
                    SubsidizedMealToken(model);
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        public List<SubsidizedMealTokenViewModel> GetSubsidizedMealTokenList(DataTable dt)
        {
            List<SubsidizedMealTokenViewModel> iList = new List<SubsidizedMealTokenViewModel>();
            iList = (from DataRow row in dt.AsEnumerable()
                     select new SubsidizedMealTokenViewModel
                     {
                         BC_SUBSIDIZED_MEAL_TOKEN_ID = Convert.ToInt64(row["BC_SUBSIDIZED_MEAL_TOKEN_ID"].ToString()),
                         EMPLOYEE_ID = Convert.ToInt64(row["EMPLOYEE_ID"].ToString()),
                         TOKEN_CODE = row["TOKEN_CODE"].ToString(),
                         TOKEN_DATE = Convert.ToDateTime(row["TOKEN_DATE"].ToString()),
                         strTOKEN_DATE = Convert.ToDateTime(row["TOKEN_DATE"].ToString()).ToString("dd-MMM-yyyy"),
                         STATUS = Convert.ToInt16(row["STATUS"].ToString()),
                         ADDEDBY = Convert.ToInt64(row["ADDEDBY"].ToString()),
                         ADDEDDATE = Convert.ToDateTime(row["ADDEDDATE"].ToString()),
                         MEAL_AMOUNT = Convert.ToDecimal(Convert.ToString(row["TOTAL_MEAL_AMOUNT"]) == "" ? 0 : row["TOTAL_MEAL_AMOUNT"]),
                         SUBSIDIZED_AMOUNT = Convert.ToDecimal(Convert.ToString(row["SUBSIDIZED_AMOUNT"]) == "" ? 0 : row["SUBSIDIZED_AMOUNT"]),
                         PAYABLE_AMOUNT = Convert.ToDecimal(Convert.ToString(row["PAYABLE_AMOUNT"]) == "" ? 0 : row["PAYABLE_AMOUNT"]),
                     }).OrderBy(o => o.TOKEN_DATE).ToList();
            return iList;
        }

        public short InsertSubsidizedData(SubsidizedMealTokenViewModel model)
        {
            short retVal = 0;
            string msg = "";
            try
            {
                //BikerCafe oBikercafe = new BikerCafe();
                Random generator = new Random();
                model.TOKEN_CODE = generator.Next(0, 1000000).ToString("D6");
                model.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                model.EMPLOYEE_ID = Convert.ToInt64(_sessionService.Get<string>("userID"));
                model.strTOKEN_DATE = DateTime.Now.ToString("dd-MMM-yyyy");
                model.STATUS = 1;
                msg = _oBikercafe.SaveSubsidizedMealToken(model.BC_SUBSIDIZED_MEAL_TOKEN_ID, model.TOKEN_CODE, model.EMPLOYEE_ID, model.strTOKEN_DATE, model.STATUS, model.ADDEDBY);
                string[] cmdarg = msg.ToString().Split(new char[] { '#' });
                retVal = Convert.ToInt16(cmdarg[0].ToString());
                if (retVal == 1)
                {
                    //Employee_Details _Employee_Details = (Employee_Details)Session["Employee"];
                    Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");

                    if (_Employee_Details != null)
                    {
                        if (!string.IsNullOrEmpty(_Employee_Details.EMail_Id))
                        {
                            SendTokenMail(model.TOKEN_CODE, model.strTOKEN_DATE, _Employee_Details);
                        }
                        if (!string.IsNullOrEmpty(_Employee_Details.MobileNo))
                        {
                            SendSMS(model.TOKEN_CODE, model.strTOKEN_DATE, _Employee_Details);
                        }
                    }
                }
            }
            catch (Exception)
            {
                retVal = -1;
            }
            return retVal;
        }

        public short SendTokenMail(string token, string tokenDate, Employee_Details _Employee_Details)
        {
            short retVal = 0;
            try
            {
                if (!string.IsNullOrEmpty(_Employee_Details.EMail_Id))
                {
                    EmailCore sendMail = new EmailCore();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    if (serverpath.isTestServer())
                        sendMail.MailTo = serverpath.getTestEMail();
                    else
                        sendMail.MailTo = _Employee_Details.EMail_Id;

                    string strSubject = "OTP for A la carte meal";
                    string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF> OTP is generated for " + tokenDate + "</font></b></td></tr>" +
                                     "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + _Employee_Details.Employee_Name + " San ,</br></br> Please use OTP " + token + " for A la carte meal in Dreamers Cafe in between 12:00 PM to 02:00 PM. </br></br></br></td></tr>" +
                                     "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    try
                    {
                        bool status = sendMail.Send();
                    }
                    catch (Exception ex)
                    {
                        retVal = -1;
                    }
                    finally
                    {
                        retVal = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }

        public short SendSMS(string token, string tokenDate, Employee_Details _Employee_Details)
        {
            short retVal = 0;
            try
            {
                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(tokenDate) && !string.IsNullOrEmpty(_Employee_Details.MobileNo))
                {
                    string msg = "Dear " + _Employee_Details.Employee_Name + ", Please use OTP " + token + " for A la carte meal in Dreamers Cafe in between 12:00 PM to 02:00 PM. HONDA 2W";
                    string mobileNo = _Employee_Details.MobileNo;
                    string url = "http://smscounter.com/api/otp_api.php?api_key=TcKuH7iI4Z2x0wqz&pass=9kcP7K68oz&senderid=HONDAM&template_id=1707168430242548371&message=" + msg + "&dest_mobileno=" + mobileNo + "&mtype=TXT";

                    string StrSMSSentID = "";
                    WebClient ObjWebClient = new WebClient();

                    StrSMSSentID = ObjWebClient.DownloadString(url);
                    ObjWebClient.Dispose();

                    retVal = 1;
                    return retVal;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }

        //-- Guest Meal Booking
        public async Task<ActionResult> ManageGuestMealBooking()
        {
            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            List<MealTypeViewModel> _MealTypeList = await _bcService.GetMealTypeList(1);
            ViewBag.MEALTYPES = new SelectList(_MealTypeList, "BC_MEALTYPEID", "MEAL_TYPE_DESC");

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            ViewBag.strFromDate = startDate.ToString("dd-MMM-yyyy");
            ViewBag.strToDate = endDate.ToString("dd-MMM-yyyy");
            ViewBag.mealType = "";

            List<GuestMealBookingTrnViewModel> iList = await _bcService.GetGuestBookedMealList(0, startDate.ToString("dd-MMM-yyyy"), endDate.ToString("dd-MMM-yyyy"), Convert.ToInt64(_sessionService.Get<string>("userID")));

            return View(iList);
        }

        [HttpPost]
        public async Task<ActionResult> ManageGuestMealBooking(long? typeId, string fromDate, string toDate)
        {
            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            List<MealTypeViewModel> _MealTypeList = await _bcService.GetMealTypeList(1);
            ViewBag.MEALTYPES = new SelectList(_MealTypeList, "BC_MEALTYPEID", "MEAL_TYPE_DESC");

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            ViewBag.strFromDate = !string.IsNullOrEmpty(fromDate) ? fromDate : startDate.ToString("dd-MMM-yyyy");
            ViewBag.strToDate = !string.IsNullOrEmpty(toDate) ? toDate : endDate.ToString("dd-MMM-yyyy");
            ViewBag.mealType = typeId;

            List<GuestMealBookingTrnViewModel> iList = await _bcService.GetGuestBookedMealList(Convert.ToInt64(typeId), fromDate, toDate, Convert.ToInt64(_sessionService.Get<string>("userID")));
            return View(iList);
        }

        [HttpGet]
        public async Task<ActionResult> GuestMealBooking()
        {
            GuestMealBookingTrnViewModel model = new GuestMealBookingTrnViewModel();
            try
            {
                //if (_sessionService.Get<string>("userID") == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                List<MealTypeViewModel> _MealTypeList = await _bcService.GetMealTypeList(1);
                ViewBag.MEALTYPES = new SelectList(_MealTypeList, "BC_MEALTYPEID", "MEAL_TYPE_DESC");

                int mealBookingDuration = 0; int guestBookingday = 0;
                BC_VALIDATION_ViewModel validModel = await _bcService.GetValidationData();
                mealBookingDuration = string.IsNullOrEmpty(validModel.MEAL_BOOKING_DURATION) ? 0 : Convert.ToInt32(validModel.MEAL_BOOKING_DURATION);
                guestBookingday = string.IsNullOrEmpty(validModel.GUEST_BOOKING_DAY) ? 0 : Convert.ToInt32(validModel.GUEST_BOOKING_DAY);

                ViewBag.MEAL_BOOKING_DURATION = mealBookingDuration;
                ViewBag.GUEST_BOKING_DAY = guestBookingday;

                model.strBOOKED_DATE = DateTime.Now.AddDays(guestBookingday).ToString("dd-MMM-yyyy");
                model.MealAvailabilityList = await _bcService.GetGuestAvailabilityMealList(model.strBOOKED_DATE, null);
            }
            catch (Exception ex)
            {
                model = new GuestMealBookingTrnViewModel();
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> GuestMealBooking([FromBody] GuestMealBookingTrnViewModel model)
        {
            try
            {
                //if (_sessionService.Get<string>("userID") == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                List<MealSlotViewModel> _slotList = await _bcService.GetMealSlots();
                ViewBag.Slots = _slotList;

                List<Meals_AvailabilityViewModel> _AvailabilityList = await _bcService.GetGuestAvailabilityMealList(model.strBOOKED_DATE, model.MEALTYPEID);
                return PartialView("_AvailabilityListForGuest", _AvailabilityList);
            }
            catch (Exception ex)
            {
                return Json("error");
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveGuestMealBooking([FromBody] GuestMealBookingTrnViewModel model)
        {
            short retVal = 0;
            string msg = "";
            try
            {
                //if (_sessionService.Get<string>("userID") == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}



                model.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                Tuple<short, string> _tuple = await _bcService.SaveGuestMealBooking(model.MEALS_AVAILABILITY_ID, model, model.GUEST_LIST);
                retVal = _tuple.Item1;
                msg = _tuple.Item2;
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(new { val = retVal, msg = msg });
        }

        [HttpPost]
        public async Task<ActionResult> CancelGuestMealBooking(long id)
        {
            short retVal = 0;
            try
            {
                //if (_sessionService.Get<string>("userID") == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                if (id == 0) retVal = -1;
                else retVal = await _bcService.CancelGuestMealBooking(id, Convert.ToInt64(_sessionService.Get<string>("userID")));
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpGet]
        public async Task<IActionResult> GuestAutocomplete(string term)
        {
            // Retrieve the list of guests based on the search term
            List<GuestDtlViewModel> guestList = await _bcService.GuestAutocomplete(term);

            // Format each guest's information into a string
            var result = guestList.Select(dataItem =>
                $"{dataItem.MOBILE_NUMBER} | {dataItem.GUEST_NAME} | {dataItem.COMPANY}"
            ).ToList();

            // Return the result as a JSON response
            return Json(result);
        }


        //public string GuestAutocomplete(string term)
        //{
        //    List<GuestDtlViewModel> GuestList = _bcService.GuestAutocomplete(term);

        //    List<string> list = new List<string>();
        //    foreach (var dataitem in GuestList)
        //    {
        //        list.Add(dataitem.MOBILE_NUMBER.ToString() + " | " + dataitem.GUEST_NAME.ToString() + " | " + dataitem.COMPANY.ToString() + "");
        //    }
        //    System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        //    string sJSON = oSerializer.Serialize(list);
        //    return sJSON;
        //}

        public async Task<string> GetGuestData(string mobileNo, string bookingDate)
        {
            Tuple<long, short> _tuple = await _bcService.GetGuestDtlByMno(mobileNo, bookingDate);
            return _tuple.Item1 + "|" + _tuple.Item2;
        }

        public async Task<ActionResult> GuestMealDetail(long id)
        {
            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            //long _ReqId;
            //try
            //{
            //    _ReqId = Convert.ToInt64(Encryption.Decrypt(Server.UrlDecode(id)));
            //}
            //catch (Exception ex)
            //{
            //    _ReqId = Convert.ToInt64(Encryption.Decrypt(id));
            //}
            return View("GuestMealDetail", await _bcService.GetGuestRequestDtlById(id));
        }

        [HttpGet]
        public async Task<ActionResult> GuestMealApproval(string id)
        {
            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            long _ReqId = Convert.ToInt64((id));
            GuestMealBookingTrnViewModel obj = await _bcService.GetGuestRequestDtlById(_ReqId);

            short IsValidDateTime = 1;
            BC_VALIDATION_ViewModel validModel = await _bcService.GetValidationData();
            if (validModel != null)
            {
                DateTime currentDate = DateTime.Now;
                string bookingTime = validModel.GUEST_BOOKING_TIME;
                int validDay = Convert.ToInt32(validModel.GUEST_BOOKING_DAY);
                DateTime ValidDateTime = DateTime.ParseExact(obj.MEAL_BOOKED_DATE.ToString("dd-MMM-yyyy") + " " + bookingTime, "dd-MMM-yyyy HH:mm:ss", null).AddDays(-validDay);
                if (currentDate > ValidDateTime)
                {
                    IsValidDateTime = 0;
                }
            }
            ViewBag.ISVALIDFORAPPROVAL = IsValidDateTime;
            return View("GuestMealApproval", obj);
        }

        [HttpPost]
        public async Task<ActionResult> GuestMealApproval([FromBody] GuestMealBookingTrnViewModel model)
        {
            short retVal = 0;
            try
            {
                //if (_sessionService.Get<string>("userID") == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                //Employee_Details _Employee_Details = (Employee_Details)Session["Employee"];
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");


                model.LSTMODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                model.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                model.APPROVER_ECODE = Convert.ToInt64(_sessionService.Get<string>("userID"));
                retVal = await _bcService.GuestMealApproval(model);
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        // -- Ala Carte Food -- //
        [HttpGet]
        public async Task<ActionResult> AlaCarteOrderHistory()
        {
            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            ViewBag.strFromDate = startDate.ToString("dd-MMM-yyyy");
            ViewBag.strToDate = endDate.ToString("dd-MMM-yyyy");
            ViewBag.category = "";

            List<AlaCarteHeaderViewModel> iList = await _bcService.GetAlaCarteRequestList(startDate.ToString("dd-MMM-yyyy"), endDate.ToString("dd-MMM-yyyy"), 0, Convert.ToInt64(_sessionService.Get<string>("userID")));
            return View(iList);
        }

        [HttpPost]
        public async Task<ActionResult> AlaCarteOrderHistory(long? categoryId, string fromDate, string toDate)
        {
            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            ViewBag.strFromDate = !string.IsNullOrEmpty(fromDate) ? fromDate : startDate.ToString("dd-MMM-yyyy");
            ViewBag.strToDate = !string.IsNullOrEmpty(toDate) ? toDate : endDate.ToString("dd-MMM-yyyy");
            ViewBag.category = categoryId;

            List<AlaCarteHeaderViewModel> iList = await _bcService.GetAlaCarteRequestList(fromDate, toDate, Convert.ToInt16(categoryId), Convert.ToInt64(_sessionService.Get<string>("userID")));
            return View(iList);
        }

        [HttpGet]
        public async Task<ActionResult> AlaCarteFoodOrder()
        {
            AlaCarteHeaderViewModel model = new AlaCarteHeaderViewModel();
            try
            {
                //if (_sessionService.Get<string>("userID") == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                model.strORDER_DATE = DateTime.Now.ToString("dd-MMM-yyyy");
                model.ItemModelList = await _bcService.GetAlaCarteItemList();
            }
            catch (Exception ex)
            {
                model = new AlaCarteHeaderViewModel();
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> AlaCarteFoodOrder(List<AlaCarteTrnViewModel> model)
        {
            short retVal = 0;
            string msg = "";
            try
            {
                //if (_sessionService.Get<string>("userID") == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                Tuple<short, string> _tuple = await _bcService.SaveAlaCarteOrder(Convert.ToInt64(_sessionService.Get<string>("userID")), model);
                retVal = _tuple.Item1;
                msg = _tuple.Item2;
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(new { val = retVal, msg = msg });
        }

        public ActionResult AlaCarteOrderDetail(long id)
        {
            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            if (id < 0)
            {
                return View("AlaCarteOrderDetail", _bcService.GetAlaCarteDtlById(id));
            }
            else
            {
                return View("AlaCarteOrderDetail", new AlaCarteHeaderViewModel());
            }

        }
        public async Task<ActionResult> GuestMealDetailAdmin(long id)
        {

            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            //long _ReqId;
            //try
            //{
            //    _ReqId = Convert.ToInt64(Encryption.Decrypt(Server.UrlDecode(id)));
            //}
            //catch (Exception ex)
            //{
            //    _ReqId = Convert.ToInt64(Encryption.Decrypt(id));
            //}
            return View("GuestMealDetailAdmin", await _bcService.GetGuestRequestDtlById(id));
        }

        [HttpPost]
        public ActionResult CancelAlaCarteOrder(long id)
        {
            short retVal = 0;
            try
            {
                //if (_sessionService.Get<string>("userID") == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                retVal = _bcService.CancelAlaCarteOrder(id, Convert.ToInt64(_sessionService.Get<string>("userID")));
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        //// Family Booking 
        public async Task<List<FamilMealBookingDtlViewModel>> GetFamilyDtl()
        {
            List<FamilMealBookingDtlViewModel> _finaFamilyDtl = new List<FamilMealBookingDtlViewModel>();
            try
            {
                //Employee_Details employeeDetails = (Employee_Details)Session["Employee"];
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");

                _finaFamilyDtl.Add(new FamilMealBookingDtlViewModel
                {
                    MEMBER_NAME = employeeDetails._EName,
                    RELATION_TYPE = "Self"
                });

                //EportalESS objess = new EportalESS();//SAP Connection Object
                DataTable dt = new DataTable();

                //To-Do SAP Connection
                dt = await _ePortalESS.GetFamilylist(_sessionService.Get<string>("userID"), "");


                if (dt.Rows.Count > 0)
                {
                    List<FamilMealBookingDtlViewModel> _familyDtl = (from DataRow row in dt.AsEnumerable()
                                                                     select new FamilMealBookingDtlViewModel
                                                                     {
                                                                         MEMBER_NAME = row["FIRSTNAME"].ToString() + " " + row["LASTNAME"].ToString(),
                                                                         RELATION_TYPE = row.ItemArray[32].ToString(),
                                                                     }).ToList();
                    if (_familyDtl.Count > 0)
                    {
                        _finaFamilyDtl.AddRange(_familyDtl);
                    }
                }
            }
            catch (Exception ex)
            {
                _finaFamilyDtl = new List<FamilMealBookingDtlViewModel>();
            }
            return _finaFamilyDtl;
        }

        [HttpGet]
        public async Task<ActionResult> FamilyVisitBooking()
        {
            FamilyMealBookingTrnViewModel model = new FamilyMealBookingTrnViewModel();
            try
            {
                //if (_sessionService.Get<string>("userID") == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                List<MealTypeViewModel> _MealTypeList = await _bcService.GetMealTypeList(2);
                ViewBag.MEALTYPES = new SelectList(_MealTypeList, "BC_MEALTYPEID", "MEAL_TYPE_DESC");

                List<MealSlotViewModel> _slotList = await _bcService.GetMealSlots();
                ViewBag.SLOTS = _slotList;

                int mealBookingDuration = 0; int familyBookingday = 0; string end_date = "";
                BC_VALIDATION_ViewModel validModel = await _bcService.GetValidationData();
                mealBookingDuration = string.IsNullOrEmpty(validModel.MEAL_BOOKING_DURATION) ? 0 : Convert.ToInt32(validModel.MEAL_BOOKING_DURATION);
                familyBookingday = string.IsNullOrEmpty(validModel.FMY_MEAL_BOOKING_DAY) ? 0 : Convert.ToInt32(validModel.FMY_MEAL_BOOKING_DAY);
                end_date = validModel.FMY_VISIT_TILL_DATE == null ? DateTime.Now.ToString("dd-MM-yyyy") : Convert.ToDateTime(validModel.FMY_VISIT_TILL_DATE).ToString("dd-MM-yyyy");

                ViewBag.MEAL_BOOKING_DURATION = mealBookingDuration;
                ViewBag.GUEST_BOKING_DAY = familyBookingday;
                ViewBag.END_DATE = end_date;

                model.strBOOKED_DATE = DateTime.Now.AddDays(familyBookingday).ToString("dd-MMM-yyyy");
                model.MEALBOOKING_DTL = await GetFamilyDtl();
            }
            catch (Exception ex)
            {
                model = new FamilyMealBookingTrnViewModel();
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult FamilyVisitBooking(long parmValue, string parmDate)
        {
            int resVal = 0;
            FamilMealBookingDtlViewModel model = new FamilMealBookingDtlViewModel();
            try
            {
                DateTime bookingDate = DateTime.ParseExact(parmDate, "dd-MMM-yyyy", null);
                model = _bcService.GetMealByDate(parmValue, bookingDate);
                if (model != null)
                {
                    resVal = 1;
                }
            }
            catch (Exception ex)
            {
                model = new FamilMealBookingDtlViewModel();
            }
            return Json(new { resVal = resVal, result = model });
        }

        [HttpPost]
        public ActionResult FamilyVisitBookingPUT([FromBody] FamilyMealBookingTrnViewModel_VM model)
        {
            short retVal = 0;
            string msg = "";
            try
            {
                //if (_sessionService.Get<string>("userID") == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                FamilyMealBookingTrnViewModel modelTest = new FamilyMealBookingTrnViewModel();
                modelTest.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                //modelTest.MEAL_BOOKED_DATE = model.MEAL_BOOKED_DATE;
                modelTest.strBOOKED_DATE = model.strBOOKED_DATE;
                modelTest.SLOTID = model.SLOTID;
                modelTest.MEAL_STATUS = model.MEAL_STATUS;
                modelTest.MEALBOOKING_DTL = model.MEALBOOKING_DTL;


                Tuple<short, string> _tuple = _bcService.SaveFamilyMealBooking(modelTest);
                retVal = _tuple.Item1;
                msg = _tuple.Item2;
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(new { val = retVal, msg = msg });
        }

        public async Task<ActionResult> ManageFamilyVisitBooking()
        {
            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            List<MealTypeViewModel> _MealTypeList = await _bcService.GetMealTypeList(2);
            ViewBag.MEALTYPES = new SelectList(_MealTypeList, "BC_MEALTYPEID", "MEAL_TYPE_DESC");

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            ViewBag.strFromDate = startDate.ToString("dd-MMM-yyyy");
            ViewBag.strToDate = endDate.ToString("dd-MMM-yyyy");
            ViewBag.mealType = "";

            List<FamilyMealBookingTrnViewModel> iList = _bcService.GetFamilyBookedMealList(0, startDate.ToString("dd-MMM-yyyy"), endDate.ToString("dd-MMM-yyyy"), Convert.ToInt64(_sessionService.Get<string>("userID")));

            return View(iList);
        }

        [HttpPost]
        public async Task<ActionResult> ManageFamilyVisitBooking(long? typeId, string fromDate, string toDate)
        {
            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            List<MealTypeViewModel> _MealTypeList = await _bcService.GetMealTypeList(2);
            ViewBag.MEALTYPES = new SelectList(_MealTypeList, "BC_MEALTYPEID", "MEAL_TYPE_DESC");

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            ViewBag.strFromDate = !string.IsNullOrEmpty(fromDate) ? fromDate : startDate.ToString("dd-MMM-yyyy");
            ViewBag.strToDate = !string.IsNullOrEmpty(toDate) ? toDate : endDate.ToString("dd-MMM-yyyy");
            ViewBag.mealType = typeId;

            List<FamilyMealBookingTrnViewModel> iList = _bcService.GetFamilyBookedMealList(Convert.ToInt64(typeId), fromDate, toDate, Convert.ToInt64(_sessionService.Get<string>("userID")));
            return View(iList);
        }

        [HttpPost]
        public ActionResult CancelFamilyVisitBooking(long id)
        {
            short retVal = 0;
            try
            {
                //if (_sessionService.Get<string>("userID") == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                retVal = _bcService.CancelFamilyMealBooking(id, Convert.ToInt64(_sessionService.Get<string>("userID")));
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        public ActionResult BindSlotByBookingDate(string bDate)
        {
            List<MealSlotViewModel> _slotList = _bcService.GetMealSlotsByBookingDate(bDate);
            return Json(_slotList);
        }

        public ActionResult FamilyVisitDetail(long id)
        {
            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            return View("FamilyVisitDetail", _bcService.GetFamilyRequestDtlById(id));
        }

        //----Meeting food----------------
        [HttpGet]
        public ActionResult MeetingFoodOrder()
        {
            MeetingFoodHeaderViewModel model = new MeetingFoodHeaderViewModel();
            try
            {
                //if (_sessionService.Get<string>("userID") == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                model.strORDER_DATE = DateTime.Now.ToString("dd-MMM-yyyy");
                model.ItemModelList = _bcService.GetMeetingFoodItemList();
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, now.Day);
                ViewBag.strFromDate = startDate.ToString("dd-MMM-yyyy");
            }
            catch (Exception ex)
            {
                model = new MeetingFoodHeaderViewModel();
            }
            return View("MeetingFood/MeetingFoodOrder", model);
        }

        [HttpPost]
        public ActionResult MeetingFoodOrder([FromBody] MeetingFoodHeaderViewModel model)
        {
            short retVal = 0;
            string msg = "";
            try
            {
                //if (_sessionService.Get<string>("userID") == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                //Employee_Details _Employee_Details = (Employee_Details)Session["Employee"];
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");

                string OPID = _Employee_Details.Operation_Id;
                string desg = _Employee_Details.Designation_Id;
                Tuple<short, string> _tuple = _bcService.SaveMeetingFoodOrder(Convert.ToInt64(_sessionService.Get<string>("userID")), Convert.ToInt64(OPID), model.EVENT_DATE.ToString(), model.EVENTTIME, model.VENUE, model.REMARKS, model.TrnModel, model.NoOfGuests, desg);
                //Tuple<short, string> _tuple = _bcService.SaveMeetingFoodOrder(Convert.ToInt64(_sessionService.Get<string>("userID")), Convert.ToInt64(OPID), model.EVENT_DATE_OFFSET.ToString(), model.EVENTTIME, model.VENUE, model.REMARKS, model.TrnModel, model.NoOfGuests, desg);

                retVal = _tuple.Item1;
                msg = _tuple.Item2;
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(new { val = retVal, msg = msg });
        }

        [HttpGet]
        public ActionResult MeetingFoodOrderHistory()
        {
            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            ViewBag.strFromDate = startDate.ToString("dd-MMM-yyyy");
            ViewBag.strToDate = endDate.ToString("dd-MMM-yyyy");
            ViewBag.category = "";

            List<MeetingFoodHeaderViewModel> iList = _bcService.GetMeetingFoodRequestList(startDate.ToString("dd-MMM-yyyy"), endDate.ToString("dd-MMM-yyyy"), 0, Convert.ToInt64(_sessionService.Get<string>("userID")));
            return View("MeetingFood/MeetingFoodOrderHistory", iList);
        }

        [HttpPost]
        public ActionResult MeetingFoodOrderHistory(long? categoryId, string fromDate, string toDate)
        {
            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            ViewBag.strFromDate = !string.IsNullOrEmpty(fromDate) ? fromDate : startDate.ToString("dd-MMM-yyyy");
            ViewBag.strToDate = !string.IsNullOrEmpty(toDate) ? toDate : endDate.ToString("dd-MMM-yyyy");
            ViewBag.category = categoryId;

            List<MeetingFoodHeaderViewModel> iList = _bcService.GetMeetingFoodRequestList(fromDate, toDate, Convert.ToInt16(categoryId), Convert.ToInt64(_sessionService.Get<string>("userID")));
            return View("MeetingFood/MeetingFoodOrderHistory", iList);
        }


        [HttpGet]
        public ActionResult MeetingFoodApproval(string id)
        {

            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            long _ReqId = Convert.ToInt64((id));
            MeetingFoodHeaderViewModel obj = _bcService.GetMeetingFoodDtlById(_ReqId);
            short IsValidDateTime = 1;
            if (obj != null)
            {
                DateTime currentDate = DateTime.Now;
                string bookingTime = obj.EVENTTIME;
                DateTime ValidDateTime = obj.EVENT_DATE.AddDays(1);
                if (currentDate > ValidDateTime)
                {
                    IsValidDateTime = 0;
                }

            }
            ViewBag.ISVALIDFORAPPROVAL = IsValidDateTime;
            return View("MeetingFood/MeetingFoodApproval", obj);
        }

        [HttpPost]
        public ActionResult MeetingFoodApproval([FromBody] MeetingFoodHeaderViewModel model)
        {
            short retVal = 0;
            try
            {
                //if (_sessionService.Get<string>("userID") == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                //Employee_Details _Employee_Details = (Employee_Details)Session["Employee"];
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");

                model.LSTMODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                retVal = _bcService.MeetingFoodApproval(model);
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        public ActionResult MeetingFoodOrderDetail(long id)
        {
            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            return View("MeetingFood/MeetingFoodOrderDetail", _bcService.GetMeetingFoodDtlById(id));
        }

        [HttpPost]
        public ActionResult CancelMeetingFoodOrder(int id)
        {


            short retVal = 0;
            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            if (id == 0)
            {
                //Log error
            }
            else
            {
                long empcode = Convert.ToInt64(_sessionService.Get<string>("userID"));
                retVal = _bcService.CancelMeetingFoodBooking(id, empcode);
            }

            return Json(retVal);
        }
    }
}
