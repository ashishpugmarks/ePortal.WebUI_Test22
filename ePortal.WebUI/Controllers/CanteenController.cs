
using ePortal.Application.Contracts;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    [SessionTimeout]
    public class CanteenController : Controller
    {

        private readonly ICanteenService _csdService;
        private readonly ISessionService _sessionService;
        private readonly ILogger<HomeController> _logger;
        public CanteenController(ICanteenService csdService, ISessionService sessionService, ILogger<HomeController> logger)
        {
            _csdService = csdService;
            _sessionService = sessionService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> CanteenServiceHistory()
        {
            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            //Employee_Details _Employee_Details = (Employee_Details)Session["Employee"];
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");

            List<CSDMealTypeViewModel> mealTypeList = await _csdService.GetMealTypeList(Convert.ToInt64(_Employee_Details._PlantId));
            ViewBag.MEALTYPES = new SelectList(mealTypeList, "CSD_MEALTYPEID", "MEAL_TYPE_DESC");

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            ViewBag.strFromDate = startDate.ToString("dd-MMM-yyyy");
            ViewBag.strToDate = endDate.ToString("dd-MMM-yyyy");
            ViewBag.mealType = "";

            List<CanteenMealBookingViewModel> iList = await _csdService.GetBookingHistory(0, startDate.ToString("dd-MMM-yyyy"), endDate.ToString("dd-MMM-yyyy"), Convert.ToInt64(_sessionService.Get<string>("userID")));
            return View(iList);
        }

        [HttpPost]
        public async Task<ActionResult> CanteenServiceHistory(long? typeId, string fromDate, string toDate)
        {
            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            //Employee_Details _Employee_Details = (Employee_Details)Session["Employee"];
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");

            List<CSDMealTypeViewModel> mealTypeList = await _csdService.GetMealTypeList(Convert.ToInt64(_Employee_Details._PlantId));
            ViewBag.MEALTYPES = new SelectList(mealTypeList, "CSD_MEALTYPEID", "MEAL_TYPE_DESC");

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            ViewBag.strFromDate = !string.IsNullOrEmpty(fromDate) ? fromDate : startDate.ToString("dd-MMM-yyyy");
            ViewBag.strToDate = !string.IsNullOrEmpty(toDate) ? toDate : endDate.ToString("dd-MMM-yyyy");
            ViewBag.mealType = typeId;

            List<CanteenMealBookingViewModel> iList = await _csdService.GetBookingHistory(Convert.ToInt64(typeId), fromDate, toDate, Convert.ToInt64(_sessionService.Get<string>("userID")));
            return View(iList);
        }

        [HttpGet]
        public async Task<ActionResult> CanteenMealBooking()
        {
            CanteenMealBookingViewModel model = new CanteenMealBookingViewModel();
            try
            {
                //Employee_Details _Employee_Details = (Employee_Details)Session["Employee"];
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");

                List<CSDMealTypeViewModel> mealTypeList = await _csdService.GetMealTypeList(Convert.ToInt64(_Employee_Details._PlantId));
                ViewBag.MEALTYPELIST = mealTypeList;

                List<MealOptionMstViewModel> optionList = await _csdService.GetMealOptionList();
                ViewBag.MEALOPTIONLIST = new SelectList(optionList, "CSDMEAL_OPTIONID", "MEAL_OPTION");

                int mealBookingDuration = 0;
                string MEAL_BOOKING_DURATION = await _csdService.GetColValue(0, 0, "MEAL_BOOKING_DURATION");
                mealBookingDuration = string.IsNullOrEmpty(MEAL_BOOKING_DURATION) ? 0 : Convert.ToInt32(MEAL_BOOKING_DURATION);
                ViewBag.MEAL_BOOKING_DURATION = mealBookingDuration;

                model.strBookingFrom = DateTime.Now.ToString("dd-MMM-yyyy");
                model.strBookingTo = DateTime.Now.AddDays(mealBookingDuration).ToString("dd-MMM-yyyy");
            }
            catch (Exception ex)
            {
                model = new CanteenMealBookingViewModel();
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> CanteenMealBooking([FromBody] CanteenMealBookingViewModel model)
        {
            try
            {
                //Employee_Details _Employee_Details = (Employee_Details)Session["Employee"];
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");

                List<CSDMealsAvailabilityViewModel> modelList = await _csdService.GetAvailabilityMealList(model.strBookingFrom, model.strBookingTo, model.MEALTYPEID, model.MEALOPTIONID, Convert.ToInt64(_sessionService.Get<string>("userID")), Convert.ToInt64(_Employee_Details._PlantId));
                return PartialView("_MealAvailabilityList", modelList);
            }
            catch (Exception ex)
            {
                return Json("error");
            }
        }

        //[HttpPut]
        //public async Task<ActionResult> CanteenMealBooking(int id, CanteenMealBookingViewModel model)
        //{
        //    short retVal = 0; string msg = string.Empty;
        //    try
        //    {
        //        if (model.MEALS_AVAILABILITY_ID > 0)
        //        {
        //            Tuple<short, string> _tuple = await _csdService.SaveMealBooking(Convert.ToInt64(_sessionService.Get<string>("userID")), model);
        //            retVal = _tuple.Item1;
        //            msg = _tuple.Item2;
        //        }
        //        else
        //        {
        //            _logger.LogError("CanteenMealBooking: model cannot be null");
        //            retVal = -1;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        retVal = -1;
        //    }
        //    return Json(new { val = retVal, msg = msg });
        //}

        [HttpPut]
        public async Task<ActionResult> CanteenMealBooking(int id, CanteenMealBookingViewModel model)
        {
            short retVal = 0; string msg = string.Empty;
            try
            {
                if (model.MEALS_AVAILABILITY_ID > 0)
                {
                    Tuple<short, string> _tuple = await _csdService.SaveMealBooking(Convert.ToInt64(_sessionService.Get<string>("userID")), model);
                    retVal = _tuple.Item1;
                    msg = _tuple.Item2;
                }
                else
                {
                    _logger.LogError("CanteenMealBooking: model cannot be null");
                    retVal = -1;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(new { val = retVal, msg = msg });
        }

        [HttpPost]
        public async Task<ActionResult> SaveCanteenMealBooking([FromBody] CanteenMealBookingViewModel model)
        {
            short retVal = 0; string msg = string.Empty;
            try
            {
                if (model.MEALS_AVAILABILITY_ID > 0)
                {
                    Tuple<short, string> _tuple = await _csdService.SaveMealBooking(Convert.ToInt64(_sessionService.Get<string>("userID")), model);
                    retVal = _tuple.Item1;
                    msg = _tuple.Item2;
                }
                else
                {
                    _logger.LogError("CanteenMealBooking: model cannot be null");
                    retVal = -1;
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

                retVal = await _csdService.CancelBooking(id, Convert.ToInt64(_sessionService.Get<string>("userID")));
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        //-- Guest Meal Booking
        public async Task<ActionResult> ManageGuestMealBooking()
        {
            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            //Employee_Details _Employee_Details = (Employee_Details)Session["Employee"];
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");

            List<CSDMealTypeViewModel> _MealTypeList = await _csdService.GetMealTypeList(Convert.ToInt64(_Employee_Details._PlantId));
            ViewBag.MEALTYPES = new SelectList(_MealTypeList, "CSD_MEALTYPEID", "MEAL_TYPE_DESC");

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            ViewBag.strFromDate = startDate.ToString("dd-MMM-yyyy");
            ViewBag.strToDate = endDate.ToString("dd-MMM-yyyy");
            ViewBag.mealType = "";

            List<CSDGuestMealBookingTrnViewModel> iList = _csdService.GetGuestBookedMealList(0, startDate.ToString("dd-MMM-yyyy"), endDate.ToString("dd-MMM-yyyy"), Convert.ToInt64(_sessionService.Get<string>("userID")));

            return View(iList);
        }

        [HttpPost]
        public async Task<ActionResult> ManageGuestMealBooking(long? typeId, string fromDate, string toDate)
        {
            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            //Employee_Details _Employee_Details = (Employee_Details)Session["Employee"];
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");

            List<CSDMealTypeViewModel> _MealTypeList = await _csdService.GetMealTypeList(Convert.ToInt64(_Employee_Details._PlantId));
            ViewBag.MEALTYPES = new SelectList(_MealTypeList, "CSD_MEALTYPEID", "MEAL_TYPE_DESC");

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            ViewBag.strFromDate = !string.IsNullOrEmpty(fromDate) ? fromDate : startDate.ToString("dd-MMM-yyyy");
            ViewBag.strToDate = !string.IsNullOrEmpty(toDate) ? toDate : endDate.ToString("dd-MMM-yyyy");
            ViewBag.mealType = typeId;

            List<CSDGuestMealBookingTrnViewModel> iList = _csdService.GetGuestBookedMealList(Convert.ToInt64(typeId), fromDate, toDate, Convert.ToInt64(_sessionService.Get<string>("userID")));
            return View(iList);
        }

        [HttpGet]
        public async Task<ActionResult> GuestMealBooking()
        {
            CSDGuestMealBookingTrnViewModel model = new CSDGuestMealBookingTrnViewModel();
            try
            {
                //if (_sessionService.Get<string>("userID") == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}
                //Employee_Details _Employee_Details = (Employee_Details)Session["Employee"];
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");

                List<CSDMealTypeViewModel> _MealTypeList = await _csdService.GetMealTypeList(Convert.ToInt64(_Employee_Details._PlantId));
                ViewBag.MEALTYPES = new SelectList(_MealTypeList, "CSD_MEALTYPEID", "MEAL_TYPE_DESC");

                List<MealOptionMstViewModel> optionList = await _csdService.GetMealOptionList();
                ViewBag.MEALOPTIONLIST = new SelectList(optionList, "CSDMEAL_OPTIONID", "MEAL_OPTION");

                List<SelectListViewModel> _AppList = _csdService.GetApprovalAuth(Convert.ToInt64(_sessionService.Get<string>("userID")));
                if (_AppList.Count > 0)
                {
                    ViewBag.APPAUTH_LIST = new SelectList(_AppList, "Value", "Text");
                }
                else
                {
                    _AppList.Add(new SelectListViewModel { Value = 0, Text = "No record" });
                    ViewBag.APPAUTH_LIST = new SelectList(_AppList, "Value", "Text");
                }

                int mealBookingDuration = 0; int guestBookingday = 0;
                string MEAL_BOOKING_DURATION = await _csdService.GetColValue(0, 0, "MEAL_BOOKING_DURATION");
                string GUEST_BOOKING_DAY = await _csdService.GetColValue(0, 0, "GUEST_BOOKING_DAY");
                //string GUEST_BOOKING_TIME = _csdService.GetParmvalue("GUEST_BOOKING_TIME");

                mealBookingDuration = string.IsNullOrEmpty(MEAL_BOOKING_DURATION) ? 0 : Convert.ToInt32(MEAL_BOOKING_DURATION);
                guestBookingday = string.IsNullOrEmpty(GUEST_BOOKING_DAY) ? 0 : Convert.ToInt32(GUEST_BOOKING_DAY);

                ViewBag.MEAL_BOOKING_DURATION = mealBookingDuration;
                ViewBag.GUEST_BOKING_DAY = guestBookingday;

                model.strBOOKED_DATE = DateTime.Now.AddDays(guestBookingday).ToString("dd-MMM-yyyy");
                model.MealAvailabilityList = new List<CSDMealsAvailabilityViewModel>();
            }
            catch (Exception ex)
            {
                model = new CSDGuestMealBookingTrnViewModel();
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult GuestMealBooking([FromBody] CSDGuestMealBookingTrnViewModel model)
        {
            try
            {
                //if (_sessionService.Get<string>("userID") == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}
                //List<MealSlotViewModel> _slotList = _csdService.GetMealSlots();
                //ViewBag.Slots = _slotList;

                //Employee_Details _Employee_Details = (Employee_Details)Session["Employee"];
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");

                List<CSDMealsAvailabilityViewModel> _AvailabilityList = _csdService.GetGuestAvailabilityMealList(model.strBOOKED_DATE, model.MEALTYPEID, model.MEALOPTIONID, Convert.ToInt64(_Employee_Details._PlantId));
                return PartialView("_AvailabilityListForGuest", _AvailabilityList);
            }
            catch (Exception ex)
            {
                return Json("error");
            }
        }

        [HttpPut]
        public ActionResult GuestMealBooking(long id, CSDGuestMealBookingTrnViewModel model, List<GuestDtlViewModel> guestList)
        {
            short retVal = 0;
            string msg = "";
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                model.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                Tuple<short, string> _tuple = _csdService.SaveGuestMealBooking(id, model, guestList);
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
        public ActionResult SaveGuestMealBooking([FromBody] CSDGuestMealBookingTrnViewModel model)
        {
            short retVal = 0;
            string msg = "";
            try
            {
                //if (_sessionService.Get<string>("userID") == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                //saveGuestMealDetailsCNT

                //CSDGuestMealBookingTrnViewModel model = new CSDGuestMealBookingTrnViewModel();
                //model.APPROVER_ECODE = model1.APPROVER_ECODE;
                //model.CSD_GUEST_BOOKINGID = model1.CSD_GUEST_BOOKINGID;
                //model.GUEST_LIST = model1.GUEST_LIST;
                //model.MEALOPTIONID = model1.MEALOPTIONID;
                //model.MEALS_AVAILABILITY_ID = model1.MEALS_AVAILABILITY_ID;
                //model.MEALTYPEID = model1.MEALTYPEID;
                //model.MEAL_PRICE = model1.MEAL_PRICE;
                //model.MEAL_QTY = model1.MEAL_QTY;
                //model.MEAL_STATUS = model1.MEAL_STATUS;
                //model.VISIT_PURPOSE = model1.VISIT_PURPOSE;
                //model.strBOOKED_DATE = model1.strBOOKED_DATE;

                model.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                List<GuestDtlViewModel> guestList = model.GUEST_LIST;
                Tuple<short, string> _tuple = _csdService.SaveGuestMealBooking(model.MEALS_AVAILABILITY_ID, model, guestList);
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
        public ActionResult CancelGuestMealBooking(long id)
        {
            short retVal = 0;
            try
            {
                //if (_sessionService.Get<string>("userID") == null)
                //{
                //    return RedirectToAction("Index", "Login");
                //}
                retVal = _csdService.CancelGuestMealBooking(id, Convert.ToInt64(_sessionService.Get<string>("userID")));
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpGet]
        public IActionResult GuestAutocomplete(string term)
        {
            // Retrieve the list of guests based on the search term
            List<GuestDtlViewModel> guestList = _csdService.GuestAutocomplete(term);

            // Format each guest's information into a string
            var result = guestList.Select(dataItem =>
                $"{dataItem.MOBILE_NUMBER} | {dataItem.GUEST_NAME} | {dataItem.COMPANY}"
            ).ToList();

            // Return the result as a JSON response
            return Json(result);
        }

        //public string GuestAutocomplete(string term)
        //{
        //    List<GuestDtlViewModel> GuestList = _csdService.GuestAutocomplete(term);
        //    List<string> list = new List<string>();
        //    foreach (var dataitem in GuestList)
        //    {
        //        list.Add(dataitem.MOBILE_NUMBER.ToString() + " | " + dataitem.GUEST_NAME.ToString() + " | " + dataitem.COMPANY.ToString() + "");
        //    }
        //    System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        //    string sJSON = oSerializer.Serialize(list);
        //    return sJSON;
        //}

        public string GetGuestData(string mobileNo, string bookingDate, int mealOption)
        {
            Tuple<long, short> _tuple = _csdService.GetGuestDtlByMno(mobileNo, bookingDate, mealOption);
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
            return View("GuestMealDetail", await _csdService.GetGuestRequestDtlById(id));
        }

        [HttpGet]
        public async Task<ActionResult> GuestMealApproval(string id)
        {
            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}

            long _ReqId = Convert.ToInt64((id));
            CSDGuestMealBookingTrnViewModel obj = await _csdService.GetGuestRequestDtlById(_ReqId);

            short IsValidDateTime = 1;
            string MEAL_BOOKING_DURATION = await _csdService.GetColValue(obj.MEALTYPEID, obj.MEALOPTIONID, "MEAL_BOOKING_DURATION");
            string GUEST_BOOKING_DAY = await _csdService.GetColValue(obj.MEALTYPEID, obj.MEALOPTIONID, "GUEST_BOOKING_DAY");
            string GUEST_BOOKING_TIME = await _csdService.GetColValue(obj.MEALTYPEID, obj.MEALOPTIONID, "GUEST_BOOKING_TIME");

            DateTime currentDate = DateTime.Now;
            string bookingTime = string.IsNullOrEmpty(GUEST_BOOKING_TIME) ? "23:59:59" : GUEST_BOOKING_TIME;
            int validDay = Convert.ToInt32(GUEST_BOOKING_DAY);
            DateTime ValidDateTime = DateTime.ParseExact(obj.MEAL_BOOKED_DATE.ToString("dd-MMM-yyyy") + " " + bookingTime, "dd-MMM-yyyy HH:mm:ss", null).AddDays(-validDay);
            if (currentDate > ValidDateTime)
            {
                IsValidDateTime = 0;
            }

            ViewBag.ISVALIDFORAPPROVAL = IsValidDateTime;
            return View("GuestMealApproval", obj);
        }

        [HttpPost]
        public async Task<ActionResult> GuestMealApproval([FromBody] CSDGuestMealBookingTrnViewModel model)
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
                retVal = await _csdService.GuestMealApproval(model);
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpGet]
        public async Task<ActionResult> AdminGuestMealDetail(long id)
        {
            //if (_sessionService.Get<string>("userID") == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            return View("AdminGuestMealDetail", await _csdService.GetGuestRequestDtlById(id));
        }

        //// Bakery
        [HttpGet]
        public ActionResult ViewBakeryItem()
        {
            List<CSDBakeryItemViewModel> iList = new List<CSDBakeryItemViewModel>();
            iList = _csdService.BakeryItem();
            return View(iList);
        }

        #region MealFeedback

        public async Task<ActionResult> CanteenServiceFeedback()
        {
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");

            List<CSDMealTypeViewModel> mealTypeList = await _csdService.GetMealTypeList(Convert.ToInt64(_Employee_Details._PlantId));
            ViewBag.MEALTYPES = new SelectList(mealTypeList, "CSD_MEALTYPEID", "MEAL_TYPE_DESC");

            /*DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            */

            DateTime now = DateTime.Now;

            var startDate = now.AddDays(-2);
            var endDate = now;

            ViewBag.strFromDate = startDate.ToString("dd-MMM-yyyy");
            ViewBag.strToDate = endDate.ToString("dd-MMM-yyyy");

            ViewBag.mealType = "";

            List<CanteenMealBookingViewModel> iList = await _csdService.GetBookingHistory(0, startDate.ToString("dd-MMM-yyyy"), endDate.ToString("dd-MMM-yyyy"), Convert.ToInt64(_sessionService.Get<string>("userID")));
            return View(iList);
        }

        [HttpPost]
        public async Task<ActionResult> CanteenServiceFeedback(long? typeId, string fromDate, string toDate)
        {
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");

            List<CSDMealTypeViewModel> mealTypeList = await _csdService.GetMealTypeList(Convert.ToInt64(_Employee_Details._PlantId));
            ViewBag.MEALTYPES = new SelectList(mealTypeList, "CSD_MEALTYPEID", "MEAL_TYPE_DESC");

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            ViewBag.strFromDate = !string.IsNullOrEmpty(fromDate) ? fromDate : startDate.ToString("dd-MMM-yyyy");
            ViewBag.strToDate = !string.IsNullOrEmpty(toDate) ? toDate : endDate.ToString("dd-MMM-yyyy");
            ViewBag.mealType = typeId;

            List<CanteenMealBookingViewModel> iList = await _csdService.GetBookingFeedbackHistory(Convert.ToInt64(typeId), fromDate, toDate, Convert.ToInt64(_sessionService.Get<string>("userID")), Convert.ToInt64(_Employee_Details.Plant_Id));
            return View(iList);
        }

        public async Task<ActionResult> ManageMealFeedback()
        {
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");

            List<CSDMealTypeViewModel> mealTypeList = await _csdService.GetMealTypeList(Convert.ToInt64(_Employee_Details._PlantId));
            ViewBag.MEALTYPES = new SelectList(mealTypeList, "CSD_MEALTYPEID", "MEAL_TYPE_DESC", mealTypeList.First().CSD_MEALTYPEID);

            DateTime now = DateTime.Now;
            long mealRating = 5;
            ViewBag.SelectedRating = "";
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            ViewBag.strFromDate = startDate.ToString("dd-MMM-yyyy");
            ViewBag.strToDate = endDate.ToString("dd-MMM-yyyy");
            ViewBag.mealType = "";

            List<CanteenMealBookingViewModel> iList = await _csdService.GetAllBookingFeedbackLst(0, startDate.ToString("dd-MMM-yyyy"), endDate.ToString("dd-MMM-yyyy"), Convert.ToInt64(_sessionService.Get<string>("userID")), Convert.ToInt64(_Employee_Details.Plant_Id), mealRating);
            return View(iList);
        }

        [HttpPost]
        public async Task<ActionResult> ManageMealFeedback(long? typeId, string fromDate, string toDate, long mealRating = 5)
        {
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");

            List<CSDMealTypeViewModel> mealTypeList = await _csdService.GetMealTypeList(Convert.ToInt64(_Employee_Details._PlantId));
            ViewBag.MEALTYPES = new SelectList(mealTypeList, "CSD_MEALTYPEID", "MEAL_TYPE_DESC");

            DateTime now = DateTime.Now;
            if (mealRating == 0)
            {
                ViewBag.SelectedRating = "";
                mealRating = 5;
            }
            else
            {
                ViewBag.SelectedRating = mealRating;
            }
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            ViewBag.strFromDate = !string.IsNullOrEmpty(fromDate) ? fromDate : startDate.ToString("dd-MMM-yyyy");
            ViewBag.strToDate = !string.IsNullOrEmpty(toDate) ? toDate : endDate.ToString("dd-MMM-yyyy");
            ViewBag.mealType = typeId;

            List<CanteenMealBookingViewModel> iList = await _csdService.GetAllBookingFeedbackLst(Convert.ToInt64(typeId), fromDate, toDate, Convert.ToInt64(_sessionService.Get<string>("userID")), Convert.ToInt64(_Employee_Details.Plant_Id), mealRating);
            return View(iList);
        }

        public IActionResult ExportMealBookingToExcel(long mealType, string fromDate, string toDate, long mealRating = 5)
        {
            Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");

            if (mealRating == 0)
            {
                ViewBag.SelectedRating = "";
                mealRating = 5;
            }
            else
            {
                ViewBag.SelectedRating = mealRating;
            }

            var data = _csdService.GetMealFeedbackExcelData(Convert.ToInt64(mealType), fromDate, toDate, Convert.ToInt64(_Employee_Details.Plant_Id), mealRating);

            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Meal Booking Report");

            worksheet.Cell(1, 1).Value = "Booking ID";
            worksheet.Cell(1, 2).Value = "Meal Booked By";
            worksheet.Cell(1, 3).Value = "Meal Desc";
            worksheet.Cell(1, 4).Value = "Meal Date";
            worksheet.Cell(1, 5).Value = "Meal Name";
            worksheet.Cell(1, 6).Value = "Meal Option";
            worksheet.Cell(1, 7).Value = "Meal Type";

            int feedbackStartCol = 8;

            if (data.Any() && data.First().FeedbackPoints != null)
            {
                int col = feedbackStartCol;
                int index = 1;

                foreach (var fp in data.First().FeedbackPoints)
                {
                    //worksheet.Cell(1, col++).Value = $"{fp.PointName} Name";
                    worksheet.Cell(1, col++).Value = $"{fp.PointName} Rating";
                    worksheet.Cell(1, col++).Value = $"{fp.PointName} Remark";
                    index++;
                }
            }

            int row = 2;

            foreach (var item in data)
            {
                worksheet.Cell(row, 1).Value = item.CSD_MEALSBOOKING_ID;
                worksheet.Cell(row, 2).Value = item.ADDEDBY + "-" + item.EmpName;
                worksheet.Cell(row, 3).Value = item.MealMst.MEAL_DESC;
                worksheet.Cell(row, 4).Value = item.MEAL_BOOKED_DATE;
                worksheet.Cell(row, 5).Value = item.MealMst.MEAL_NAME;
                worksheet.Cell(row, 6).Value = item.MEALOPTION;
                worksheet.Cell(row, 7).Value = item.MealType.MEAL_TYPE_DESC;

                int col = feedbackStartCol;

                if (item.FeedbackPoints != null && item.FeedbackPoints.Any())
                {
                    foreach (var feedback in item.FeedbackPoints)
                    {
                        //worksheet.Cell(row, col++).Value = feedback.PointName;
                        worksheet.Cell(row, col++).Value = feedback.Rating;
                        worksheet.Cell(row, col++).Value = feedback.Remark;
                    }
                }

                row++;
            }
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"MealBookingReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
            );
        }

        [HttpPost]
        public IActionResult SaveFeedback()
        {
            //try
            //{
                var bookingId = Convert.ToInt64(Request.Form["BookingId"]);
                var mealType = Convert.ToInt32(Request.Form["MEALTYPEID"]);
                var pointsJson = Request.Form["FeedbackPointsJson"];

                var points = JsonConvert.DeserializeObject<List<FeedbackPointViewModel>>(pointsJson);

                IFormFile file = Request.Form.Files["EvidenceFile"];

                string dbFilePath = null;

                if (file != null && file.Length > 0)
                {
                    string ext = Path.GetExtension(file.FileName).ToLower();
                    string[] allowed = { ".jpg", ".jpeg", ".png", ".pdf" };

                    if (!allowed.Contains(ext))
                        return Json(-2);

                    if (file.Length > 500 * 1024)
                        return Json(-3);

                    string uploadFolder = Path.Combine(serverpath.getFileUploadPath(), "MealFeedback");

                    //string uploadFolder = Path.Combine(
                    //    Directory.GetCurrentDirectory(),
                    //    "wwwroot",
                    //    "Uploads",
                    //    "MealFeedback"
                    //);

                    if (!Directory.Exists(uploadFolder))
                        Directory.CreateDirectory(uploadFolder);

                    string uniqueName = Guid.NewGuid().ToString() + ext;

                    string physicalPath = Path.Combine(uploadFolder, uniqueName);

                    dbFilePath = "/Uploads/MealFeedback/" + uniqueName;

                    using (var stream = new FileStream(physicalPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
                long userId = Convert.ToInt64(_sessionService.Get<string>("userID"));

                var model = new CanteenMealBookingViewModel
                {
                    BookingId = bookingId,
                    MEALTYPEID = mealType,
                    FeedbackPoints = points,
                    EvidenceFilePath = dbFilePath,
                    ADDEDBY = userId
                };

                var retVal = _csdService.BookingFeedback(model);

                return Json(retVal);
            //}
            //catch
            //{
            //    return Json(-1);
            //}
        }

        [HttpGet]
        public IActionResult GetFeedback(int bookingId, long mealType, long uId = 0)
        {
            long userId = 0;

            if (uId != 0)
                userId = uId;
            else
                userId = Convert.ToInt64(_sessionService.Get<string>("userID"));

            var points = _csdService.GetBookingFeedback(bookingId, mealType, userId);

            return Json(points);
        }
        #endregion
    }
}