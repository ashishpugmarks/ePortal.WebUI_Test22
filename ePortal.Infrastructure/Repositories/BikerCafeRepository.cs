using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.Infrastructure.Repositories;
using ePortal.Shared.Interface;
using ePortal.Shared.Services;
using ePortal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ePortal.Infrastructure.Repositories
{
    public class BikerCafeRepository
    {
        private EPortalDGITDBContext _dbContext;
        private EPortalDBContext _db2Context;

        private SYKI_DGIT _Syki;
        private readonly IConfiguration _configuration;
        private string moveToOldAppURL;

        //public BikerCafeRepository()
        //{
        //    _dbContext = new ePortalDgitEntities();
        //    _db2Context = new ePortalEntities2();
        //    _Syki = _dbContext.SYKI.Where(x => x.ACTIVE == 1).FirstOrDefault();
        //}

        public BikerCafeRepository(EPortalDGITDBContext dbContext, EPortalDBContext db2Context, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _db2Context = db2Context;
            _Syki = _dbContext.SYKI.Where(x => x.ACTIVE == 1).FirstOrDefault();
            _configuration = configuration;
            moveToOldAppURL = _configuration["Switch_New_Old_New:Call_Old_App_URL"].ToString();
        }
        //sa CR7220
        public int GetFamilyVisitValidation(DateTime bookingDate, long EmpId)
        {
            //_dbContext.VW_ASSOCIATELVLDETAILS

            //(from data in _dbContext.HMSIHOLIDAYS );



            int Flag = 1;
            var targetDays = new[] { "Thursday", "Friday", "Saturday" };
            var dayName = bookingDate.DayOfWeek.ToString();
            if (targetDays.Contains(dayName) == false)
            {
                Flag = 0;
                //obj.Message = "Booking not allowed";
            }
            if (Flag == 1)
            {

                var v_SYKIid = (from data in _dbContext.SYKI
                                where data.ACTIVE == 1
                                select new
                                {
                                    id = data.SYKIID
                                }).ToList();

                var id = v_SYKIid[0].id;

                var v_siteId = (from data in _dbContext.VW_ASSOCIATELVLDETAILS
                                where data.ADEMPCODE == EmpId
                                && data.SYKI == id
                                //and data.
                                select new
                                {
                                    id = data.SYSITEID
                                }).ToList();
                var vsiteId = v_siteId[0].id;
                var lst = (from data in _db2Context.HMSIHOLIDAYS
                           where data.ACTIVE == 1 && data.SYSITEID == vsiteId
                           //&& data.MONTHDATEYEAR.Date == bookingDate.Date
                           && data.MONTHDATEYEAR.Day == bookingDate.Day
                           && data.MONTHDATEYEAR.Month == bookingDate.Month
                           && data.MONTHDATEYEAR.Year == bookingDate.Year
                           select new
                           {
                               Flag = 0
                           }).ToList();
                if (lst.Count > 0)
                {
                    Flag = 0;
                }
            }

            return Flag;
        }
        //ea CR7220
        public List<BcMenuViewModel> GetBCMenu()
        {
            //var iList = (from data in _dbContext.BC_MENUMST
            //             where data.STATUS == 1
            //             select new BcMenuViewModel
            //             {
            //                 BCMENUID = data.BCMENUID,
            //                 MENU_NAME = data.MENU_NAME,
            //                 MENU_DESC = data.MENU_DESC,
            //                 MENU_URL = data.MENU_URL,
            //                 MENU_ICON = data.MENU_ICON,
            //                 MENUICON_CONTENT_TYPE = data.MENUICON_CONTENT_TYPE,
            //                 MENUICON_FILENAME = data.MENUICON_FILENAME,
            //                 STATUS = data.STATUS,
            //                 DISPLAY_ORDER = data.DISPLAY_ORDER
            //             }).ToList().OrderBy(o => o.DISPLAY_ORDER).ToList();

            var iList = _dbContext.BC_MENUMST.Where(data => data.STATUS == 1)
                        .AsEnumerable() // shift to in-memory after filtering
                         .Select(data => new BcMenuViewModel
                         {
                             BCMENUID = data.BCMENUID,
                             MENU_NAME = data.MENU_NAME,
                             MENU_DESC = data.MENU_DESC,
                             //MENU_URL =  data.ISUPGRADED==1? moveToOldAppURL + data.MENU_URL: data.MENU_URL,
                             MENU_URL = data.ISUPGRADED == 1 ? data.MENU_URL : moveToOldAppURL + data.MENU_URL,
                             MENU_ICON = data.MENU_ICON,
                             MENUICON_CONTENT_TYPE = data.MENUICON_CONTENT_TYPE,
                             MENUICON_FILENAME = data.MENUICON_FILENAME,
                             STATUS = data.STATUS,
                             DISPLAY_ORDER = data.DISPLAY_ORDER
                         }).ToList().OrderBy(o => o.DISPLAY_ORDER).ToList();
            return iList;

        }

        public BC_VALIDATION_ViewModel GetValidationData()
        {
            var obj = (from data in _dbContext.BC_MST_VALIDATION
                       select new BC_VALIDATION_ViewModel
                       {
                           BC_MST_VALIDATION_ID = data.BC_MST_VALIDATION_ID,
                           MEAL_BOOKING_DURATION = data.MEAL_BOOKING_DURATION,
                           MEAL_BOOKING_TIME = data.MEAL_BOOKING_TIME,
                           MEAL_CANCELLATION_TIME = data.MEAL_CANCELLATION_TIME,
                           MEAL_CHANGE_TIME = data.MEAL_CHANGE_TIME,
                           MEAL_UPDATE_PERIOD = data.MEAL_UPDATE_PERIOD,
                           INDIANMEAL_QTY = data.INDIANMEAL_QTY,
                           JAPANESEMEAL_QTY = data.JAPANESEMEAL_QTY,
                           MEAL_BOOKING_DAY = data.MEAL_BOOKING_DAY,
                           ALACARTESUBSIDY = data.ALACARTESUBSIDY,
                           ALACARTESUBSIDY_MAX = data.ALACARTESUBSIDY_MAX,
                           ALACARTE_BOOKING_TIME = data.ALACARTE_BOOKING_TIME,
                           GUEST_BOOKING_TIME = data.GUEST_BOOKING_TIME,
                           GUEST_CANCELLATION_TIME = data.GUEST_CANCELLATION_TIME,
                           GUEST_BOOKING_DAY = data.GUEST_BOOKING_DAY,
                           GUEST_BOOKING_SELFAPP = data.GUEST_BOOKING_SELFAPP,
                           JAPANESEMEAL_BOOKING_DAY = data.JAPANESEMEAL_BOOKING_DAY,
                           JAPANESEMEAL_BOOKING_TIME = data.JAPANESEMEAL_BOOKING_TIME,
                           JAPANESEMEAL_CANCELLATION_TIME = data.JAPANESEMEAL_CANCELLATION_TIME,
                           FMY_MEAL_BOOKING_DAY = data.FMY_MEAL_BOOKING_DAY,
                           FMY_MEAL_BOOKING_TIME = data.FMY_MEAL_BOOKING_TIME,
                           FMY_MEAL_CANCELLATION_TIME = data.FMY_MEAL_CANCELLATION_TIME,
                           FMY_VISIT_MEMBER_QTY = data.FMY_VISIT_MEMBER_QTY,
                           FMY_VISIT_MAX_MEMBER_ALLOW = data.FMY_VISIT_MAX_MEMBER_ALLOW,
                           FMY_VISIT_FOLLOW_SITE_CALENDER = data.FMY_VISIT_FOLLOW_SITE_CALENDER,
                           FMY_VISIT_TILL_DATE = data.FMY_VISIT_TILL_DATE,
                       }).FirstOrDefault();
            return obj;
        }

        public List<MealTypeViewModel> GetMealTypeList(short mealCatId)
        {
            //var iList = (from data in _dbContext.BC_MST_MEAL_TYPE
            //             where data.STATUS == 1 && data.MEAL_CATEGORY == mealCatId
            //             select new MealTypeViewModel
            //             {
            //                 BC_MEALTYPEID = data.BC_MEALTYPEID,
            //                 MEAL_TYPE_DESC = data.MEAL_TYPE_DESC,
            //                 READER_CODE = data.READER_CODE,
            //                 STATUS = data.STATUS == 1 ? true : false,
            //                 ADDEDDATE = data.ADDEDDATE,
            //                 ADDEDBY = data.ADDEDBY,
            //             }).ToList();
            //return iList;

            var iList = _dbContext.BC_MST_MEAL_TYPE
                        .Where(data => data.STATUS == 1 && data.MEAL_CATEGORY == mealCatId)
                        .AsEnumerable() // shift to in-memory after filtering
                        .Select(data => new MealTypeViewModel
                        {
                            BC_MEALTYPEID = data.BC_MEALTYPEID,
                            MEAL_TYPE_DESC = data.MEAL_TYPE_DESC,
                            READER_CODE = data.READER_CODE,
                            STATUS = true,   //data.STATUS == 1 ? true:false,  // now evaluated in C#
                            ADDEDDATE = data.ADDEDDATE,
                            ADDEDBY = data.ADDEDBY,
                        })
                        .ToList();

            return iList;
        }

        public List<MealSlotViewModel> GetMealSlots()
        {
            var iList = (from data in _dbContext.BC_MEALSLOTMST
                         where data.STATUS == 1
                         select new MealSlotViewModel
                         {
                             BCSLOTMSTID = data.BCSLOTMSTID,
                             SLOT_DESC = data.SLOT_DESC,
                             SLOT_TIME = data.SLOT_TIME,
                             MEALTYPEID = data.MEALTYPEID,
                             STATUS = data.STATUS,
                             ADDEDDATE = data.ADDEDDATE,
                             ADDEDBY = data.ADDEDBY
                         }).ToList();
            return iList;
        }

        public List<MealBookingTrnViewModel> GetAvailabilityMealList(string fromDate, string toDate, long mealType, long loginUser)
        {
            DateTime bookingFromDate = DateTime.Now.Date;
            DateTime bookingToDate = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(fromDate))
            {
                bookingFromDate = DateTime.ParseExact(fromDate, "dd-MMM-yyyy", null);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                bookingToDate = DateTime.ParseExact(toDate + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
            }

            List<MealBookingTrnViewModel> bookedTRNList = (from data in _dbContext.BC_MEALSBOOKING_TRN.Where(b => b.MEAL_STATUS == 1 && b.ADDEDBY == loginUser)
                                                           join _slotMst in _dbContext.BC_MEALSLOTMST on data.SLOTID equals _slotMst.BCSLOTMSTID
                                                           where (!string.IsNullOrEmpty(fromDate) ? (data.MEAL_BOOKED_DATE >= bookingFromDate) : true)
                                                           && (!string.IsNullOrEmpty(toDate) ? (data.MEAL_BOOKED_DATE <= bookingToDate) : true)
                                                           && (mealType == 0 ? true : data.MEALTYPEID == mealType)
                                                           select new MealBookingTrnViewModel
                                                           {
                                                               BC_MEALSBOOKING_ID = data.BC_MEALSBOOKING_ID,
                                                               MEALTYPEID = data.MEALTYPEID,
                                                               MEAL_BOOKED_DATE = data.MEAL_BOOKED_DATE,
                                                               //strBOOKED_DATE = data.MEAL_BOOKED_DATE.ToString("dd-MMM-yyyy"),
                                                               MEAL_STATUS = data.MEAL_STATUS,
                                                               MEALS_AVAILABILITY_ID = data.MEALS_AVAILABILITY_ID,
                                                               SLOTMSTID = data.SLOTID,
                                                               SLOT_TIME = _slotMst.SLOT_TIME
                                                           }).ToList();


            var rawList = (from _mealAvail in _dbContext.BC_MEALS_AVAILABILITY
               .Where(w => (!string.IsNullOrEmpty(fromDate) ? (w.MEAL_MAPPING_DATE >= bookingFromDate) : true)
                         && (!string.IsNullOrEmpty(toDate) ? (w.MEAL_MAPPING_DATE <= bookingToDate) : true))
                           join _mealMst in _dbContext.BC_MST_MEALS on _mealAvail.MEALS_ID equals _mealMst.BC_MEALID
                           join _mealType in _dbContext.BC_MST_MEAL_TYPE on _mealMst.MEALTYPEID equals _mealType.BC_MEALTYPEID
                           where (mealType == 0 ? true : _mealMst.MEALTYPEID == mealType)
                              && _mealMst.STATUS == 1 && _mealAvail.STATUS == 1
                           select new
                           {
                               _mealAvail,
                               _mealMst,
                               _mealType
                           }).ToList();



            //List<MealBookingTrnViewModel> availabilityList = (from _mealAvail in _dbContext.BC_MEALS_AVAILABILITY.Where(w => (!string.IsNullOrEmpty(fromDate) ? (w.MEAL_MAPPING_DATE >= bookingFromDate) : true)
            //                                                  && (!string.IsNullOrEmpty(toDate) ? (w.MEAL_MAPPING_DATE <= bookingToDate) : true))
            //                                                  join _mealMst in _dbContext.BC_MST_MEALS on _mealAvail.MEALS_ID equals _mealMst.BC_MEALID
            //                                                  join _mealType in _dbContext.BC_MST_MEAL_TYPE on _mealMst.MEALTYPEID equals _mealType.BC_MEALTYPEID
            //                                                  where (mealType == 0 ? true : _mealMst.MEALTYPEID == mealType)
            //                                                  && _mealMst.STATUS == 1 && _mealAvail.STATUS == 1
            //                                                  select new MealBookingTrnViewModel
            //                                                  {
            //                                                      BC_MEALSBOOKING_ID = 0,
            //                                                      MealType = new MealTypeViewModel
            //                                                      {
            //                                                          BC_MEALTYPEID = _mealType.BC_MEALTYPEID,
            //                                                          MEAL_TYPE_DESC = _mealType.MEAL_TYPE_DESC,
            //                                                      },
            //                                                      MEAL_BOOKED_DATE = _mealAvail.MEAL_MAPPING_DATE,
            //                                                      //strBOOKED_DATE = _mealAvail.MEAL_MAPPING_DATE.ToString("dd-MMM-yyyy"),
            //                                                      MealAvailabilityModel = new Meals_AvailabilityViewModel
            //                                                      {
            //                                                          BC_MEALS_AVAILABILITY_ID = _mealAvail.BC_MEALS_AVAILABILITY_ID,
            //                                                          MEAL_MAPPING_DATE = _mealAvail.MEAL_MAPPING_DATE,
            //                                                          MEALS_ID = _mealAvail.MEALS_ID,
            //                                                          MealMst_MODEL = new BCMstMealViewModel
            //                                                          {
            //                                                              BC_MEALID = _mealMst.BC_MEALID,
            //                                                              MEALTYPEID = _mealMst.MEALTYPEID,
            //                                                              MealTypeModel = new MealTypeViewModel
            //                                                              {
            //                                                                  BC_MEALTYPEID = _mealType.BC_MEALTYPEID,
            //                                                                  MEAL_TYPE_DESC = _mealType.MEAL_TYPE_DESC,
            //                                                              },
            //                                                              MEAL_NAME = _mealMst.MEAL_NAME,
            //                                                              MEAL_DESC = _mealMst.MEAL_DESC,
            //                                                              MEAL_PRICE = _mealMst.MEAL_PRICE,
            //                                                              MEAL_PHOTO = _mealMst.MEAL_PHOTO,
            //                                                              MEAL_PHOTO_NAME = _mealMst.MEAL_PHOTO_NAME,
            //                                                          },
            //                                                          STATUS = _mealAvail.STATUS == 1 ? true : false,
            //                                                          ADDEDBY = _mealAvail.ADDEDBY
            //                                                      }
            //                                                  }).ToList();


            List<MealBookingTrnViewModel> availabilityList = rawList.Select(x => new MealBookingTrnViewModel
            {
                BC_MEALSBOOKING_ID = 0,
                MealType = new MealTypeViewModel
                {
                    BC_MEALTYPEID = x._mealType.BC_MEALTYPEID,
                    MEAL_TYPE_DESC = x._mealType.MEAL_TYPE_DESC,
                },
                MEAL_BOOKED_DATE = x._mealAvail.MEAL_MAPPING_DATE,
                MealAvailabilityModel = new Meals_AvailabilityViewModel
                {
                    BC_MEALS_AVAILABILITY_ID = x._mealAvail.BC_MEALS_AVAILABILITY_ID,
                    MEAL_MAPPING_DATE = x._mealAvail.MEAL_MAPPING_DATE,
                    MEALS_ID = x._mealAvail.MEALS_ID,
                    STATUS = x._mealAvail.STATUS == 1, // Now it's safe to use bool logic
                    ADDEDBY = x._mealAvail.ADDEDBY,
                    MealMst_MODEL = new BCMstMealViewModel
                    {
                        BC_MEALID = x._mealMst.BC_MEALID,
                        MEALTYPEID = x._mealMst.MEALTYPEID,
                        MealTypeModel = new MealTypeViewModel
                        {
                            BC_MEALTYPEID = x._mealType.BC_MEALTYPEID,
                            MEAL_TYPE_DESC = x._mealType.MEAL_TYPE_DESC,
                        },
                        MEAL_NAME = x._mealMst.MEAL_NAME,
                        MEAL_DESC = x._mealMst.MEAL_DESC,
                        MEAL_PRICE = x._mealMst.MEAL_PRICE,
                        MEAL_PHOTO = x._mealMst.MEAL_PHOTO,
                        MEAL_PHOTO_NAME = x._mealMst.MEAL_PHOTO_NAME,
                    }
                }
            }).ToList();



            List<MealBookingTrnViewModel> _finalList = new List<MealBookingTrnViewModel>();
            if (availabilityList.Count > 0)
            {
                DateTime currentDate = DateTime.Now;
                string cancellationTime = "23:59:00";
                string cancellationJapTime = "23:59:00";

                int IndianMealDay = 0;
                string IndianMealTime = "11:59:00";

                int JapMealDay = 0;
                string JapMealTime = "11:59:00";

                BC_VALIDATION_ViewModel ValidationMst = GetValidationData();
                if (ValidationMst != null)
                {
                    if (!string.IsNullOrEmpty(ValidationMst.MEAL_CANCELLATION_TIME))
                    {
                        cancellationTime = ValidationMst.MEAL_CANCELLATION_TIME;
                    }
                    if (!string.IsNullOrEmpty(ValidationMst.JAPANESEMEAL_CANCELLATION_TIME))
                    {
                        cancellationJapTime = ValidationMst.JAPANESEMEAL_CANCELLATION_TIME;
                    }

                    IndianMealDay = !string.IsNullOrEmpty(ValidationMst.MEAL_BOOKING_DAY) ? Convert.ToInt32(ValidationMst.MEAL_BOOKING_DAY) : IndianMealDay;

                    IndianMealTime = !string.IsNullOrEmpty(ValidationMst.MEAL_BOOKING_TIME) ? ValidationMst.MEAL_BOOKING_TIME : IndianMealTime;

                    JapMealDay = !string.IsNullOrEmpty(ValidationMst.JAPANESEMEAL_BOOKING_DAY) ? Convert.ToInt32(ValidationMst.JAPANESEMEAL_BOOKING_DAY) : JapMealDay;

                    JapMealTime = !string.IsNullOrEmpty(ValidationMst.JAPANESEMEAL_BOOKING_TIME) ? ValidationMst.JAPANESEMEAL_BOOKING_TIME : JapMealTime;

                }
                foreach (MealBookingTrnViewModel obj in availabilityList)
                {
                    MealBookingTrnViewModel bookedObj = bookedTRNList.Where(w => w.MEALS_AVAILABILITY_ID == obj.MealAvailabilityModel.BC_MEALS_AVAILABILITY_ID && w.MEAL_BOOKED_DATE == obj.MealAvailabilityModel.MEAL_MAPPING_DATE).FirstOrDefault();
                    if (bookedObj != null)
                    {
                        obj.BC_MEALSBOOKING_ID = bookedObj.BC_MEALSBOOKING_ID;
                        obj.MEAL_STATUS = bookedObj.MEAL_STATUS;
                        obj.MEAL_BOOKED_DATE = bookedObj.MEAL_BOOKED_DATE;
                        obj.SLOTMSTID = bookedObj.SLOTMSTID;
                        obj.SLOT_TIME = bookedObj.SLOT_TIME;
                        //obj.strBOOKED_DATE = bookedObj.MEAL_BOOKED_DATE.ToString("dd-MMM-yyyy");
                    }
                    if (obj.MealType.BC_MEALTYPEID == 1) // - Indian
                    {
                        DateTime validIndMealDate = DateTime.ParseExact(obj.MEAL_BOOKED_DATE.ToString("dd-MMM-yyyy") + " " + IndianMealTime, "dd-MMM-yyyy HH:mm:ss", null).AddDays(-IndianMealDay);
                        obj.IsSelectedForBooking = currentDate <= validIndMealDate ? true : false;

                        DateTime validIndCancelDate = DateTime.ParseExact(obj.MEAL_BOOKED_DATE.ToString("dd-MMM-yyyy") + " " + cancellationTime, "dd-MMM-yyyy HH:mm:ss", null).AddDays(-IndianMealDay);
                        obj.IsEnableCancel = currentDate < validIndCancelDate ? true : false;
                    }
                    else // JAPANESE
                    {
                        DateTime validJapMealDate = DateTime.ParseExact(obj.MEAL_BOOKED_DATE.ToString("dd-MMM-yyyy") + " " + JapMealTime, "dd-MMM-yyyy HH:mm:ss", null).AddDays(-JapMealDay);
                        obj.IsSelectedForBooking = currentDate <= validJapMealDate ? true : false;

                        DateTime validJapCancelDate = DateTime.ParseExact(obj.MEAL_BOOKED_DATE.ToString("dd-MMM-yyyy") + " " + cancellationJapTime, "dd-MMM-yyyy HH:mm:ss", null).AddDays(-JapMealDay);
                        obj.IsEnableCancel = currentDate < validJapCancelDate ? true : false;
                    }

                    _finalList.Add(obj);
                }
            }
            return _finalList.OrderBy(o => o.MEAL_BOOKED_DATE).ToList();
        }

        public Tuple<short, string> SaveMealBooking(long AddedBy, List<MealBookingTrnViewModel> modelList)
        {
            short retVal = 0;
            string msg = "";
            Tuple<short, string> _tuple = new Tuple<short, string>(retVal, msg);
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (MealBookingTrnViewModel model in modelList)
                    {
                        BC_MEALSBOOKING_TRN_DGIT BCM = new BC_MEALSBOOKING_TRN_DGIT();
                        if (model.MEALS_AVAILABILITY_ID > 0)
                        {

                            #region Check Validation
                            DateTime bookingDate = DateTime.Now.Date;
                            if (!string.IsNullOrEmpty(model.strBOOKED_DATE))
                            {
                                bookingDate = DateTime.ParseExact(model.strBOOKED_DATE, "dd-MMM-yyyy", null);
                            }

                            var checkStatus = _dbContext.BC_MEALSBOOKING_TRN.Count(x => x.MEAL_BOOKED_DATE == bookingDate && x.ADDEDBY == AddedBy && x.MEAL_STATUS == 1) > 0;

                            if (checkStatus)
                            {
                                retVal = 2;
                                msg = model.strBOOKED_DATE;
                                transaction.Rollback();
                                return _tuple = new Tuple<short, string>(retVal, msg); //// -- record already exist.
                            }



                            BC_VALIDATION_ViewModel ValidationMst = GetValidationData();
                            if (ValidationMst != null)
                            {
                                if (string.IsNullOrEmpty(ValidationMst.MEAL_BOOKING_DURATION) || string.IsNullOrEmpty(ValidationMst.MEAL_BOOKING_TIME) || string.IsNullOrEmpty(ValidationMst.JAPANESEMEAL_BOOKING_TIME))
                                {
                                    retVal = 5; //// -- Validation master not maintained.
                                }
                                else
                                {
                                    DateTime currentDate = DateTime.Now.Date;
                                    int bookingDuration = Convert.ToInt32(ValidationMst.MEAL_BOOKING_DURATION);
                                    DateTime ValidDuration = currentDate.AddDays(bookingDuration);
                                    if (bookingDate > ValidDuration)
                                    {
                                        retVal = 4;
                                        msg = ValidDuration.ToString("dd-MMM-yyyy");
                                        transaction.Rollback();
                                        return _tuple = new Tuple<short, string>(retVal, msg); //// -- Book before n-1 day.
                                    }

                                    currentDate = DateTime.Now;
                                    if (model.MEALTYPEID == 1)
                                    {
                                        string bookingTime = ValidationMst.MEAL_BOOKING_TIME;
                                        int validDay = string.IsNullOrEmpty(ValidationMst.MEAL_BOOKING_DAY) ? 0 : Convert.ToInt32(ValidationMst.MEAL_BOOKING_DAY);
                                        DateTime ValidDateTime = DateTime.ParseExact(model.strBOOKED_DATE + " " + bookingTime, "dd-MMM-yyyy HH:mm:ss", null).AddDays(-validDay);
                                        if (currentDate > ValidDateTime)
                                        {
                                            retVal = 3;
                                            msg = model.strBOOKED_DATE + " after " + ValidDateTime.ToString("HH:mm") + " hrs";
                                            transaction.Rollback();
                                            return _tuple = new Tuple<short, string>(retVal, msg); //// -- Book before n-1 day.
                                        }
                                    }
                                    else
                                    {
                                        string JapbookingTime = ValidationMst.JAPANESEMEAL_BOOKING_TIME;
                                        int japValidDay = string.IsNullOrEmpty(ValidationMst.JAPANESEMEAL_BOOKING_DAY) ? 0 : Convert.ToInt32(ValidationMst.JAPANESEMEAL_BOOKING_DAY);
                                        DateTime JapValidDateTime = DateTime.ParseExact(model.strBOOKED_DATE + " " + JapbookingTime, "dd-MMM-yyyy HH:mm:ss", null).AddDays(-japValidDay);
                                        if (currentDate > JapValidDateTime)
                                        {
                                            retVal = 3;
                                            msg = model.strBOOKED_DATE + " after " + JapValidDateTime.ToString("HH:mm") + " hrs";
                                            transaction.Rollback();
                                            return _tuple = new Tuple<short, string>(retVal, msg); //// -- Book before n-1 day.
                                        }
                                    }
                                }
                            }
                            if (retVal == 5 || ValidationMst == null)
                            {
                                retVal = 5;
                                msg = "";
                                transaction.Rollback();
                                return _tuple = new Tuple<short, string>(retVal, msg); //// -- Validation master not maintained.
                            }
                            #endregion

                            #region Check QTY
                            int guestBookedCount = _dbContext.BC_GUESTMEALBOOKING_TRN.Count(x => x.MEAL_BOOKED_DATE == bookingDate && x.MEALTYPEID == model.MEALTYPEID && x.MEAL_STATUS == 1);
                            int adminBookedCount = _dbContext.BC_ADMIN_MEALSBOOKING_TRN.Count(x => x.MEAL_BOOKED_DATE == bookingDate && x.MEALTYPEID == model.MEALTYPEID && x.MEAL_STATUS == 1);
                            int bookedCount = _dbContext.BC_MEALSBOOKING_TRN.Count(x => x.MEAL_BOOKED_DATE == bookingDate && x.MEALTYPEID == model.MEALTYPEID && x.MEAL_STATUS == 1);
                            int totalBookedCount = (guestBookedCount + adminBookedCount + bookedCount);
                            if (model.MEALTYPEID == 1 ? (totalBookedCount >= Convert.ToInt32(ValidationMst.INDIANMEAL_QTY)) : (totalBookedCount >= Convert.ToInt32(ValidationMst.JAPANESEMEAL_QTY)))
                            {
                                retVal = 6;
                                msg = bookingDate.ToString("dd-MMM-yyyy");
                                transaction.Rollback();
                                return _tuple = new Tuple<short, string>(retVal, msg); //// -- Meal booking has exceeded the maximum limit.
                            }

                            else
                            {
                                guestBookedCount = _dbContext.BC_GUESTMEALBOOKING_TRN.Count(x => x.MEAL_BOOKED_DATE == bookingDate && x.MEALTYPEID == model.MEALTYPEID && x.MEAL_STATUS == 1 && x.SLOTID == model.SLOTMSTID);
                                adminBookedCount = _dbContext.BC_ADMIN_MEALSBOOKING_TRN.Count(x => x.MEAL_BOOKED_DATE == bookingDate && x.MEALTYPEID == model.MEALTYPEID && x.MEAL_STATUS == 1 && x.SLOTID == model.SLOTMSTID);
                                bookedCount = _dbContext.BC_MEALSBOOKING_TRN.Count(x => x.MEAL_BOOKED_DATE == bookingDate && x.MEALTYPEID == model.MEALTYPEID && x.MEAL_STATUS == 1 && x.SLOTID == model.SLOTMSTID);
                                totalBookedCount = (guestBookedCount + adminBookedCount + bookedCount);
                                int slotCapcity = 0;
                                BC_MEALSLOTMST_DGIT slotMst = (from data in _dbContext.BC_MEALSLOTMST
                                                               where data.BCSLOTMSTID == model.SLOTMSTID
                                                               select data).FirstOrDefault();
                                if (slotMst != null)
                                {
                                    slotCapcity = slotMst.CAPACITY;
                                }
                                if (totalBookedCount >= slotCapcity)
                                {
                                    retVal = 7;
                                    msg = "Slot " + slotMst.SLOT_TIME + " has exceeded the maximum limit for " + bookingDate.ToString("dd-MMM-yyyy");
                                    transaction.Rollback();
                                    return _tuple = new Tuple<short, string>(retVal, msg); //// -- Slot has exceeded the maximum limit.
                                }
                            }
                            #endregion

                            if (_dbContext.BC_MEALSBOOKING_TRN.Count() == 0)
                            {
                                BCM.BC_MEALSBOOKING_ID = 1;
                            }
                            else
                            {
                                BCM.BC_MEALSBOOKING_ID = _dbContext.BC_MEALSBOOKING_TRN.Max(x => x.BC_MEALSBOOKING_ID) + 1;
                            }

                            BCM.MEALS_AVAILABILITY_ID = model.MEALS_AVAILABILITY_ID;
                            BCM.MEALTYPEID = model.MEALTYPEID;
                            BCM.MEAL_BOOKED_DATE = bookingDate;
                            BCM.MEAL_STATUS = model.MEAL_STATUS;
                            BCM.SLOTID = model.SLOTMSTID;
                            BCM.ADDEDBY = AddedBy;
                            BCM.ADDEDDATE = DateTime.Now;
                            _dbContext.Entry(BCM).State = EntityState.Added;
                            _dbContext.SaveChanges();

                        }
                    }
                    transaction.Commit();
                    retVal = 1;
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
            }
            _tuple = new Tuple<short, string>(retVal, msg);
            return _tuple;
        }

        public List<MealBookingTrnViewModel> GetBookedMealList(long mealType, string fromDate, string toDate, long loginUser)
        {
            DateTime currentDate = DateTime.Now;
            string cancellationTime = "23:59:00";
            string cancellationJapTime = "23:59:00";
            int IndianMealDay = 0;
            int JapMealDay = 0;

            BC_VALIDATION_ViewModel ValidationMst = GetValidationData();
            if (ValidationMst != null)
            {
                if (!string.IsNullOrEmpty(ValidationMst.MEAL_CANCELLATION_TIME))
                {
                    cancellationTime = ValidationMst.MEAL_CANCELLATION_TIME;
                }

                if (!string.IsNullOrEmpty(ValidationMst.JAPANESEMEAL_CANCELLATION_TIME))
                {
                    cancellationJapTime = ValidationMst.JAPANESEMEAL_CANCELLATION_TIME;
                }

                IndianMealDay = !string.IsNullOrEmpty(ValidationMst.MEAL_BOOKING_DAY) ? Convert.ToInt32(ValidationMst.MEAL_BOOKING_DAY) : IndianMealDay;
                JapMealDay = !string.IsNullOrEmpty(ValidationMst.JAPANESEMEAL_BOOKING_DAY) ? Convert.ToInt32(ValidationMst.JAPANESEMEAL_BOOKING_DAY) : JapMealDay;
            }

            DateTime bookingFromDate = DateTime.Now.Date;
            DateTime bookingToDate = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(fromDate))
            {
                bookingFromDate = DateTime.ParseExact(fromDate, "dd-MMM-yyyy", null);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                bookingToDate = DateTime.ParseExact(toDate + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
            }
            var iList = (from data in _dbContext.BC_MEALSBOOKING_TRN.Where(w => w.ADDEDBY == loginUser)
                         join _mealType in _dbContext.BC_MST_MEAL_TYPE on data.MEALTYPEID equals _mealType.BC_MEALTYPEID
                         join _mealAvail in _dbContext.BC_MEALS_AVAILABILITY on data.MEALS_AVAILABILITY_ID equals _mealAvail.BC_MEALS_AVAILABILITY_ID
                         join _mealMst in _dbContext.BC_MST_MEALS on _mealAvail.MEALS_ID equals _mealMst.BC_MEALID
                         join _slotMst in _dbContext.BC_MEALSLOTMST on data.SLOTID equals _slotMst.BCSLOTMSTID
                         where (!string.IsNullOrEmpty(fromDate) ? (data.MEAL_BOOKED_DATE >= bookingFromDate) : true)
                                                          && (!string.IsNullOrEmpty(toDate) ? (data.MEAL_BOOKED_DATE <= bookingToDate) : true)
                                                          && (mealType == 0 ? true : data.MEALTYPEID == mealType)
                         select new MealBookingTrnViewModel
                         {
                             BC_MEALSBOOKING_ID = data.BC_MEALSBOOKING_ID,
                             MEALTYPEID = data.MEALTYPEID,
                             MealType = new MealTypeViewModel
                             {
                                 BC_MEALTYPEID = _mealType.BC_MEALTYPEID,
                                 MEAL_TYPE_DESC = _mealType.MEAL_TYPE_DESC,
                             },
                             MEAL_BOOKED_DATE = data.MEAL_BOOKED_DATE,
                             MEAL_STATUS = data.MEAL_STATUS,
                             MEALS_AVAILABILITY_ID = data.MEALS_AVAILABILITY_ID,
                             MealMst = new BCMstMealViewModel
                             {
                                 BC_MEALID = _mealMst.BC_MEALID,
                                 MEALTYPEID = _mealMst.MEALTYPEID,
                                 MEAL_NAME = _mealMst.MEAL_NAME,
                                 MEAL_PRICE = _mealMst.MEAL_PRICE,
                                 MEAL_DESC = _mealMst.MEAL_DESC,
                                 MEAL_PHOTO = _mealMst.MEAL_PHOTO,
                                 MEAL_PHOTO_NAME = _mealMst.MEAL_PHOTO_NAME,
                             },
                             ADDEDBY = data.ADDEDBY,
                             ADDEDDATE = data.ADDEDDATE,
                             SLOTMSTID = data.SLOTID,
                             SLOT_TIME = _slotMst.SLOT_TIME
                         }).ToList();
            List<MealBookingTrnViewModel> newList = new List<MealBookingTrnViewModel>();
            foreach (var obj in iList)
            {
                if (obj.MEALTYPEID == 1)
                {
                    DateTime validIndCancelDate = DateTime.ParseExact(obj.MEAL_BOOKED_DATE.ToString("dd-MMM-yyyy") + " " + cancellationTime, "dd-MMM-yyyy HH:mm:ss", null).AddDays(-IndianMealDay);
                    obj.IsEnableCancel = currentDate < validIndCancelDate ? true : false;
                }
                else
                {
                    DateTime validJapCancelDate = DateTime.ParseExact(obj.MEAL_BOOKED_DATE.ToString("dd-MMM-yyyy") + " " + cancellationJapTime, "dd-MMM-yyyy HH:mm:ss", null).AddDays(-JapMealDay);
                    obj.IsEnableCancel = currentDate < validJapCancelDate ? true : false;
                }
                newList.Add(obj);
            }

            return newList.OrderByDescending(o => o.MEAL_BOOKED_DATE).ThenByDescending(t => t.BC_MEALSBOOKING_ID).ToList();
        }

        public short CancelBooking(long id, long updatedBy)
        {
            short retVal = 0;
            try
            {
                BC_MEALSBOOKING_TRN_DGIT model = new BC_MEALSBOOKING_TRN_DGIT();
                model = _dbContext.BC_MEALSBOOKING_TRN.Where(w => w.BC_MEALSBOOKING_ID == id).FirstOrDefault();
                if (model != null)
                {
                    model.MEAL_STATUS = 2;
                    model.LSTMODIFIEDBY = updatedBy;
                    model.LSTMODDATE = DateTime.Now;
                    _dbContext.Entry(model).State = EntityState.Modified;
                    _dbContext.SaveChanges();

                    retVal = 1;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }

        //public short SaveSubsidizedMealToken(SubsidizedMealTokenViewModel model)
        //{
        //    short retVal = 0;
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        retVal = -1;
        //    }
        //    return retVal;
        //}


        // -- Guest Meal Booking
        public List<GuestMealBookingTrnViewModel> GetGuestBookedMealList(long mealType, string fromDate, string toDate, long loginUser)
        {
            DateTime currentDate = DateTime.Now;
            string cancellationTime = "23:59:00";
            int guest_MealDay = 0;
            BC_VALIDATION_ViewModel ValidationMst = GetValidationData();
            if (ValidationMst != null)
            {
                if (!string.IsNullOrEmpty(ValidationMst.GUEST_CANCELLATION_TIME))
                {
                    cancellationTime = ValidationMst.GUEST_CANCELLATION_TIME;
                }

                guest_MealDay = !string.IsNullOrEmpty(ValidationMst.GUEST_BOOKING_DAY) ? Convert.ToInt32(ValidationMst.GUEST_BOOKING_DAY) : guest_MealDay;

            }

            DateTime bookingFromDate = DateTime.Now.Date;
            DateTime bookingToDate = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(fromDate))
            {
                bookingFromDate = DateTime.ParseExact(fromDate, "dd-MMM-yyyy", null);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                bookingToDate = DateTime.ParseExact(toDate + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
            }


            var iList = (from data in _dbContext.BC_GUESTMEALBOOKING_TRN.Where(w => w.ADDEDBY == loginUser)
                         join _ApproverAuth in _dbContext.ADEMPLOYEE on data.APPROVER_ECODE equals _ApproverAuth.ADEMPCODE into _ApproverAuthJoin
                         from _Approver in _ApproverAuthJoin.DefaultIfEmpty()
                         join _mealType in _dbContext.BC_MST_MEAL_TYPE on data.MEALTYPEID equals _mealType.BC_MEALTYPEID
                         join _mealAvail in _dbContext.BC_MEALS_AVAILABILITY on data.MEALS_AVAILABILITY_ID equals _mealAvail.BC_MEALS_AVAILABILITY_ID
                         join _mealMst in _dbContext.BC_MST_MEALS on _mealAvail.MEALS_ID equals _mealMst.BC_MEALID
                         join _slotMst in _dbContext.BC_MEALSLOTMST on data.SLOTID equals _slotMst.BCSLOTMSTID
                         where (!string.IsNullOrEmpty(fromDate) ? (data.MEAL_BOOKED_DATE >= bookingFromDate) : true)
                                                          && (!string.IsNullOrEmpty(toDate) ? (data.MEAL_BOOKED_DATE <= bookingToDate) : true)
                                                          && (mealType == 0 ? true : data.MEALTYPEID == mealType)
                         select new GuestMealBookingTrnViewModel
                         {
                             BC_GUEST_BOOKINGID = data.BC_GUEST_BOOKINGID,
                             MEALTYPEID = data.MEALTYPEID,
                             MealType = new MealTypeViewModel
                             {
                                 BC_MEALTYPEID = _mealType.BC_MEALTYPEID,
                                 MEAL_TYPE_DESC = _mealType.MEAL_TYPE_DESC,
                             },
                             PAYMENT_TYPE = data.PAYMENT_TYPE,
                             MEAL_BOOKED_DATE = data.MEAL_BOOKED_DATE,
                             MEAL_STATUS = data.MEAL_STATUS,
                             MEALS_AVAILABILITY_ID = data.MEALS_AVAILABILITY_ID,
                             APPROVER_ECODE = data.APPROVER_ECODE,
                             APPROVER_EMAIL = (_Approver == null ? "" : _Approver.EMAILID),
                             APPROVER_FNAME = (_Approver == null ? "" : _Approver.FIRSTNAME),
                             APPROVER_LNAME = (_Approver == null ? "" : _Approver.LASTNAME),
                             APPROVE_STATUS = _Approver == null ? (short)-1 : data.APPROVE_STATUS,
                             APPROVE_DATE = data.APPROVE_DATE,
                             APPROVE_REMARK = data.APPROVE_REMARK,
                             GuestDtlList = (from _guestData in _dbContext.BC_GUESTMEALBOOKING_DTL.Where(d => d.BC_GUESTMEALBOOKINGID == data.BC_GUEST_BOOKINGID)
                                             join _Guest in _dbContext.BC_GUEST_DTL on _guestData.GUESTDTLID equals _Guest.BC_GUESTID
                                             select new GuestDtlViewModel
                                             {
                                                 BC_GUESTID = _Guest.BC_GUESTID,
                                                 GUEST_NAME = _Guest.GUEST_NAME,
                                                 MOBILE_NUMBER = _Guest.MOBILE_NUMBER,
                                                 COMPANY = _Guest.COMPANY,
                                                 TOKEN_CODE = _guestData.TOKEN_CODE,
                                                 MEAL_PRICE = _guestData.MEAL_PRICE
                                             }).ToList(),
                             MealMst = new BCMstMealViewModel
                             {
                                 BC_MEALID = _mealMst.BC_MEALID,
                                 MEALTYPEID = _mealMst.MEALTYPEID,
                                 MEAL_NAME = _mealMst.MEAL_NAME,
                                 MEAL_PRICE = _mealMst.MEAL_PRICE,
                                 MEAL_DESC = _mealMst.MEAL_DESC,
                                 MEAL_PHOTO = _mealMst.MEAL_PHOTO,
                                 MEAL_PHOTO_NAME = _mealMst.MEAL_PHOTO_NAME,
                             },
                             ADDEDBY = data.ADDEDBY,
                             ADDEDDATE = data.ADDEDDATE,
                             SLOTMSTID = data.SLOTID,
                             SLOT_TIME = _slotMst.SLOT_TIME
                         }).ToList();
            List<GuestMealBookingTrnViewModel> newList = new List<GuestMealBookingTrnViewModel>();
            foreach (var obj in iList)
            {
                DateTime validDate = DateTime.ParseExact(obj.MEAL_BOOKED_DATE.ToString("dd-MMM-yyyy") + " " + cancellationTime, "dd-MMM-yyyy HH:mm:ss", null).AddDays(-guest_MealDay);
                obj.IsEnableCancel = currentDate < validDate ? true : false;
                newList.Add(obj);
            }

            return newList.OrderByDescending(o => o.MEAL_BOOKED_DATE).ToList();
        }

        public List<Meals_AvailabilityViewModel> GetGuestAvailabilityMealList(string date, long? mealType)
        {
            DateTime bookingDate = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(date))
            {
                bookingDate = DateTime.ParseExact(date, "dd-MMM-yyyy", null);
            }
            //var iList = (from _mealAvail in _dbContext.BC_MEALS_AVAILABILITY
            //             join _mealMst in _dbContext.BC_MST_MEALS on _mealAvail.MEALS_ID equals _mealMst.BC_MEALID
            //             join _mealType in _dbContext.BC_MST_MEAL_TYPE on _mealMst.MEALTYPEID equals _mealType.BC_MEALTYPEID
            //             join _mPrice in _dbContext.BC_PRICE_VALIDITY on _mealMst.MEALTYPEID equals _mPrice.MEALTYPEID into _mPriceJoin
            //             from _mealPrice in _mPriceJoin.DefaultIfEmpty()
            //             where (string.IsNullOrEmpty(date) || _mealAvail.MEAL_MAPPING_DATE == bookingDate)
            //             && (mealType == null || mealType == 0 || _mealMst.MEALTYPEID == mealType)
            //             && _mealMst.STATUS == 1 && _mealAvail.STATUS == 1
            //             //CR5635 start
            //             && (bookingDate >= _mealPrice.FROM_DATE && bookingDate <= _mealPrice.TO_DATE)
            //             //CR5635 end
            //             select new Meals_AvailabilityViewModel
            //             {
            //                 BC_MEALS_AVAILABILITY_ID = _mealAvail.BC_MEALS_AVAILABILITY_ID,
            //                 MEAL_MAPPING_DATE = _mealAvail.MEAL_MAPPING_DATE,
            //                 MEALS_ID = _mealAvail.MEALS_ID,
            //                 MealMst_MODEL = new BCMstMealViewModel
            //                 {
            //                     BC_MEALID = _mealMst.BC_MEALID,
            //                     MEALTYPEID = _mealMst.MEALTYPEID,
            //                     MealTypeModel = new MealTypeViewModel
            //                     {
            //                         BC_MEALTYPEID = _mealType.BC_MEALTYPEID,
            //                         MEAL_TYPE_DESC = _mealType.MEAL_TYPE_DESC,
            //                         MEAL_PRICE = _mealPrice == null ? (decimal)0 : _mealPrice.MEAL_AMOUNT,
            //                     },
            //                     MEAL_NAME = _mealMst.MEAL_NAME,
            //                     MEAL_DESC = _mealMst.MEAL_DESC,
            //                     MEAL_PRICE = _mealMst.MEAL_PRICE,
            //                     MEAL_PHOTO = _mealMst.MEAL_PHOTO,
            //                     MEAL_PHOTO_NAME = _mealMst.MEAL_PHOTO_NAME,
            //                 },
            //                 STATUS = _mealAvail.STATUS == 1 ? true : false,
            //                 ADDEDBY = _mealAvail.ADDEDBY,
            //                 ADDEDDATE = _mealAvail.ADDEDDATE
            //             }).ToList();


            //var iList = (from _mealAvail in _dbContext.BC_MEALS_AVAILABILITY
            //             join _mealMst in _dbContext.BC_MST_MEALS on _mealAvail.MEALS_ID equals _mealMst.BC_MEALID
            //             join _mealType in _dbContext.BC_MST_MEAL_TYPE on _mealMst.MEALTYPEID equals _mealType.BC_MEALTYPEID
            //             join _mPrice in _dbContext.BC_PRICE_VALIDITY on _mealMst.MEALTYPEID equals _mPrice.MEALTYPEID into _mPriceJoin
            //             from _mealPrice in _mPriceJoin.DefaultIfEmpty()
            //             where (string.IsNullOrEmpty(date) ? true : _mealAvail.MEAL_MAPPING_DATE == bookingDate)          
            //             && (mealType == null || mealType == 0 ? true : _mealMst.MEALTYPEID == mealType)
            //             && _mealMst.STATUS == 1 && _mealAvail.STATUS == 1
            //             //CR5635 start
            //             && (bookingDate >= _mealPrice.FROM_DATE && bookingDate <= _mealPrice.TO_DATE)

            // Phase 1: Fetch minimal necessary data (Oracle-safe)
            var dbQuery = (from _mealAvail in _dbContext.BC_MEALS_AVAILABILITY
                           join _mealMst in _dbContext.BC_MST_MEALS on _mealAvail.MEALS_ID equals _mealMst.BC_MEALID
                           join _mealType in _dbContext.BC_MST_MEAL_TYPE on _mealMst.MEALTYPEID equals _mealType.BC_MEALTYPEID
                           join _mealPrice in _dbContext.BC_PRICE_VALIDITY on _mealMst.MEALTYPEID equals _mealPrice.MEALTYPEID into _mPriceJoin
                           from _mealPrice in _mPriceJoin.DefaultIfEmpty()
                           where (string.IsNullOrEmpty(date) || _mealAvail.MEAL_MAPPING_DATE == bookingDate)
                    && (mealType == null || mealType == 0 || _mealMst.MEALTYPEID == mealType)
                           && _mealMst.STATUS == 1 && _mealAvail.STATUS == 1
                           && (bookingDate >= _mealPrice.FROM_DATE && bookingDate <= _mealPrice.TO_DATE)
                           select new
                           {
                               Availability = _mealAvail,
                               Meal = _mealMst,
                               MealType = _mealType,
                               MealPrice = _mealPrice
                           }).ToList(); // Now you’re in memory (safe)

            // Phase 2: Complex filtering in memory using C#
            var iList = dbQuery
                //.Where(x =>
                //    (string.IsNullOrEmpty(date) || x.Availability.MEAL_MAPPING_DATE == bookingDate)
                //    && (mealType == null || mealType == 0 || x.Meal.MEALTYPEID == mealType)
                //    && (x.MealPrice == null || (bookingDate >= x.MealPrice.FROM_DATE && bookingDate <= x.MealPrice.TO_DATE))
                //)
                .Select(x => new Meals_AvailabilityViewModel
                {
                    BC_MEALS_AVAILABILITY_ID = x.Availability.BC_MEALS_AVAILABILITY_ID,
                    MEAL_MAPPING_DATE = x.Availability.MEAL_MAPPING_DATE,
                    MEALS_ID = x.Availability.MEALS_ID,
                    MealMst_MODEL = new BCMstMealViewModel
                    {
                        BC_MEALID = x.Meal.BC_MEALID,
                        MEALTYPEID = x.Meal.MEALTYPEID,
                        MealTypeModel = new MealTypeViewModel
                        {
                            BC_MEALTYPEID = x.MealType.BC_MEALTYPEID,
                            MEAL_TYPE_DESC = x.MealType.MEAL_TYPE_DESC,
                            MEAL_PRICE = x.MealPrice == null ? 0 : x.MealPrice.MEAL_AMOUNT,
                        },
                        MEAL_NAME = x.Meal.MEAL_NAME,
                        MEAL_DESC = x.Meal.MEAL_DESC,
                        MEAL_PRICE = x.Meal.MEAL_PRICE,
                        MEAL_PHOTO = x.Meal.MEAL_PHOTO,
                        MEAL_PHOTO_NAME = x.Meal.MEAL_PHOTO_NAME
                    },
                    STATUS = x.Availability.STATUS == 1,
                    ADDEDBY = x.Availability.ADDEDBY,
                    ADDEDDATE = x.Availability.ADDEDDATE
                }).ToList();




            return iList;
        }

        public Tuple<short, string, long> SaveGuestMealBooking(long id, GuestMealBookingTrnViewModel model, List<GuestDtlViewModel> guestList)
        {
            short retVal = 0;
            string msg = "";
            long _headerId = 0;
            Tuple<short, string, long> _tuple = new Tuple<short, string, long>(retVal, msg, _headerId);
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    BC_GUESTMEALBOOKING_TRN_DGIT GMB = new BC_GUESTMEALBOOKING_TRN_DGIT();
                    BC_VALIDATION_ViewModel ValidationMst = GetValidationData();

                    long APPROVER_ECODE = 0;
                    if (model.PAYMENT_TYPE == 2) //// -- Company Paid
                    {
                        if (ValidationMst != null)
                        {
                            string[] _desigIds;
                            _desigIds = !string.IsNullOrEmpty(ValidationMst.GUEST_BOOKING_SELFAPP) ? ValidationMst.GUEST_BOOKING_SELFAPP.Trim().Split(',') : new string[] { "" };
                            APPROVER_ECODE = GetApprovalAuth(model.ADDEDBY, _desigIds);
                            if (APPROVER_ECODE == 0)
                            {
                                retVal = 8; //// -- Approval authority does not exist.
                                msg = "";
                                return _tuple = new Tuple<short, string, long>(retVal, msg, _headerId);
                            }
                        }
                        else
                        {
                            retVal = 5;
                            msg = "";
                            return _tuple = new Tuple<short, string, long>(retVal, msg, _headerId); //// -- Validation master not maintained.
                        }
                    }
                    else
                    {
                        model.MEAL_STATUS = 1;
                    }

                    #region Check Validation
                    DateTime bookingDate = DateTime.Now.Date;
                    if (!string.IsNullOrEmpty(model.strBOOKED_DATE))
                    {
                        bookingDate = DateTime.ParseExact(model.strBOOKED_DATE, "dd-MMM-yyyy", null);
                    }

                    List<long> guestIds = guestList.Select(s => s.BC_GUESTID).ToList();
                    int _recordCount = (from data in _dbContext.BC_GUESTMEALBOOKING_TRN.Where(x => x.MEAL_BOOKED_DATE == bookingDate && x.MEAL_STATUS != 2 && x.MEAL_STATUS != 3)
                                        join _GuestDTL in _dbContext.BC_GUESTMEALBOOKING_DTL on data.BC_GUEST_BOOKINGID equals _GuestDTL.BC_GUESTMEALBOOKINGID
                                        where guestIds.Contains(_GuestDTL.GUESTDTLID)
                                        select data).Count();

                    if (_recordCount > 0)
                    {
                        retVal = 2;
                        msg = model.strBOOKED_DATE;
                        return _tuple = new Tuple<short, string, long>(retVal, msg, _headerId); //// -- record already exist.
                    }

                    if (ValidationMst != null)
                    {
                        if (string.IsNullOrEmpty(ValidationMst.MEAL_BOOKING_DURATION) || string.IsNullOrEmpty(ValidationMst.GUEST_BOOKING_TIME))
                        {
                            retVal = 5; //// -- Validation master not maintained.
                        }
                        else
                        {
                            DateTime currentDate = DateTime.Now.Date;
                            int bookingDuration = Convert.ToInt32(ValidationMst.MEAL_BOOKING_DURATION);
                            DateTime ValidDuration = currentDate.AddDays(bookingDuration);
                            if (bookingDate > ValidDuration)
                            {
                                retVal = 4;
                                msg = ValidDuration.ToString("dd-MMM-yyyy");
                                return _tuple = new Tuple<short, string, long>(retVal, msg, _headerId); //// -- Book before n-1 day.
                            }

                            currentDate = DateTime.Now;
                            string bookingTime = ValidationMst.GUEST_BOOKING_TIME;
                            int validDay = Convert.ToInt32(ValidationMst.GUEST_BOOKING_DAY);
                            DateTime ValidDateTime = DateTime.ParseExact(model.strBOOKED_DATE + " " + bookingTime, "dd-MMM-yyyy HH:mm:ss", null).AddDays(-validDay);
                            if (currentDate > ValidDateTime)
                            {
                                retVal = 3;
                                msg = model.strBOOKED_DATE + " after " + ValidDateTime.ToString("HH:mm") + " hrs";
                                return _tuple = new Tuple<short, string, long>(retVal, msg, _headerId); //// -- Book before n-1 day.
                            }
                        }
                    }
                    if (retVal == 5 || ValidationMst == null)
                    {
                        retVal = 5;
                        msg = "";
                        return _tuple = new Tuple<short, string, long>(retVal, msg, _headerId); //// -- Validation master not maintained.
                    }
                    #endregion

                    #region Check QTY
                    int guestBookedCount = _dbContext.BC_GUESTMEALBOOKING_TRN.Count(x => x.MEAL_BOOKED_DATE == bookingDate && x.MEALTYPEID == model.MEALTYPEID && x.MEAL_STATUS == 1);
                    int adminBookedCount = _dbContext.BC_ADMIN_MEALSBOOKING_TRN.Count(x => x.MEAL_BOOKED_DATE == bookingDate && x.MEALTYPEID == model.MEALTYPEID && x.MEAL_STATUS == 1);
                    int bookedCount = _dbContext.BC_MEALSBOOKING_TRN.Count(x => x.MEAL_BOOKED_DATE == bookingDate && x.MEALTYPEID == model.MEALTYPEID && x.MEAL_STATUS == 1);
                    int totalBookedCount = (guestBookedCount + adminBookedCount + bookedCount);
                    if (model.MEALTYPEID == 1 ? (totalBookedCount >= Convert.ToInt32(ValidationMst.INDIANMEAL_QTY)) : (totalBookedCount >= Convert.ToInt32(ValidationMst.JAPANESEMEAL_QTY)))
                    {
                        retVal = 6;
                        msg = bookingDate.ToString("dd-MMM-yyyy");
                        return _tuple = new Tuple<short, string, long>(retVal, msg, _headerId); //// -- Meal booking has exceeded the maximum limit.
                    }
                    else
                    {
                        guestBookedCount = _dbContext.BC_GUESTMEALBOOKING_TRN.Count(x => x.MEAL_BOOKED_DATE == bookingDate && x.MEALTYPEID == model.MEALTYPEID && x.MEAL_STATUS == 1 && x.SLOTID == model.SLOTMSTID);
                        adminBookedCount = _dbContext.BC_ADMIN_MEALSBOOKING_TRN.Count(x => x.MEAL_BOOKED_DATE == bookingDate && x.MEALTYPEID == model.MEALTYPEID && x.MEAL_STATUS == 1 && x.SLOTID == model.SLOTMSTID);
                        bookedCount = _dbContext.BC_MEALSBOOKING_TRN.Count(x => x.MEAL_BOOKED_DATE == bookingDate && x.MEALTYPEID == model.MEALTYPEID && x.MEAL_STATUS == 1 && x.SLOTID == model.SLOTMSTID);
                        totalBookedCount = (guestBookedCount + adminBookedCount + bookedCount);
                        int slotCapcity = 0;
                        BC_MEALSLOTMST_DGIT slotMst = (from data in _dbContext.BC_MEALSLOTMST
                                                       where data.BCSLOTMSTID == model.SLOTMSTID
                                                       select data).FirstOrDefault();
                        if (slotMst != null)
                        {
                            slotCapcity = slotMst.CAPACITY;
                        }
                        if (totalBookedCount >= slotCapcity)
                        {
                            retVal = 7;
                            msg = "Slot " + slotMst.SLOT_TIME + " has exceeded the maximum limit for " + bookingDate.ToString("dd-MMM-yyyy");
                            transaction.Rollback();
                            return _tuple = new Tuple<short, string, long>(retVal, msg, _headerId); //// -- Slot has exceeded the maximum limit.
                        }
                    }
                    #endregion

                    if (_dbContext.BC_GUESTMEALBOOKING_TRN.Count() == 0)
                    {
                        GMB.BC_GUEST_BOOKINGID = 1;
                    }
                    else
                    {
                        GMB.BC_GUEST_BOOKINGID = _dbContext.BC_GUESTMEALBOOKING_TRN.Max(x => x.BC_GUEST_BOOKINGID) + 1;
                    }

                    GMB.PAYMENT_TYPE = model.PAYMENT_TYPE;
                    GMB.MEALS_AVAILABILITY_ID = model.MEALS_AVAILABILITY_ID;
                    GMB.MEALTYPEID = model.MEALTYPEID;
                    GMB.MEAL_BOOKED_DATE = bookingDate;
                    GMB.APPROVER_ECODE = APPROVER_ECODE;
                    if (APPROVER_ECODE == model.ADDEDBY)
                    {
                        GMB.MEAL_STATUS = 1;
                        GMB.APPROVE_STATUS = 1;
                        GMB.APPROVE_DATE = DateTime.Now;
                        GMB.APPROVE_REMARK = "Auto Approved";
                    }
                    else
                    {
                        GMB.APPROVE_STATUS = APPROVER_ECODE == 0 ? (short)-1 : (short)0;
                        GMB.MEAL_STATUS = model.MEAL_STATUS;
                    }
                    GMB.SLOTID = model.SLOTMSTID;
                    GMB.ADDEDBY = model.ADDEDBY;
                    GMB.ADDEDDATE = DateTime.Now;
                    _dbContext.Entry(GMB).State = EntityState.Added;
                    _dbContext.SaveChanges();

                    model.BC_GUEST_BOOKINGID = GMB.BC_GUEST_BOOKINGID;
                    //// -- Save Guest Detail --////
                    retVal = SaveGuestDtl(model, guestList);
                    if (retVal == 2) ///// token already exist.
                    {
                        transaction.Rollback();
                        model.BC_GUEST_BOOKINGID = 0;
                        SaveGuestMealBooking(id, model, guestList);
                    }
                    else if (retVal == 1)
                    {
                        retVal = 1;
                        msg = "";
                        _headerId = GMB.BC_GUEST_BOOKINGID;
                        transaction.Commit();
                    }
                    else
                    {
                        retVal = 0;
                        msg = "";
                        transaction.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
            }
            _tuple = new Tuple<short, string, long>(retVal, msg, _headerId);
            return _tuple;
        }

        public short SaveGuestDtl(GuestMealBookingTrnViewModel model, List<GuestDtlViewModel> guestList)
        {
            short retVal = 0;
            if (model.MEALS_AVAILABILITY_ID > 0)
            {
                foreach (GuestDtlViewModel Gdtl in guestList)
                {
                    #region INSERT/MODIFY GUEST DTL
                    int dtlFlag = 0;
                    BC_GUEST_DTL_DGIT BGD = new BC_GUEST_DTL_DGIT();
                    if (Gdtl.BC_GUESTID > 0)
                    {
                        BGD = _dbContext.BC_GUEST_DTL.Where(g => g.BC_GUESTID == Gdtl.BC_GUESTID).FirstOrDefault();
                    }
                    else
                    {
                        BGD = new BC_GUEST_DTL_DGIT();
                        if (_dbContext.BC_GUEST_DTL.Count() == 0)
                        {
                            BGD.BC_GUESTID = 1;
                        }
                        else
                        {
                            BGD.BC_GUESTID = _dbContext.BC_GUEST_DTL.Max(x => x.BC_GUESTID) + 1;
                        }
                        dtlFlag = 1;
                    }
                    BGD.GUEST_NAME = Gdtl.GUEST_NAME;
                    BGD.MOBILE_NUMBER = Gdtl.MOBILE_NUMBER;
                    BGD.COMPANY = Gdtl.COMPANY;
                    BGD.STATUS = 1;
                    if (dtlFlag == 1)
                    {
                        BGD.ADDEDBY = model.ADDEDBY;
                        BGD.ADDEDDATE = DateTime.Now;
                    }
                    else
                    {
                        BGD.LSTMODIFIEDBY = model.ADDEDBY;
                        BGD.LSTMODDATE = DateTime.Now;
                    }
                    _dbContext.Entry(BGD).State = dtlFlag == 1 ? EntityState.Added : EntityState.Modified;
                    _dbContext.SaveChanges();
                    #endregion

                    #region INSERT GUEST MEAL BOOKING DTL
                    Gdtl.BC_GUESTID = BGD.BC_GUESTID;
                    Random generator = new Random();
                    string tokenCode = generator.Next(0, 1000000).ToString("D6");
                    retVal = SaveGuestMealBookingDtl(model, Gdtl, tokenCode);
                    if (retVal != 1)
                    {
                        return retVal;
                    }
                    #endregion
                }
                retVal = 1;
            }
            return retVal;
        }

        public short SaveGuestMealBookingDtl(GuestMealBookingTrnViewModel model, GuestDtlViewModel guestModel, string tokenCode)
        {
            short retVal = 0;
            try
            {
                BC_GUESTMEALBOOKING_DTL_DGIT GMB_DTL = new BC_GUESTMEALBOOKING_DTL_DGIT();

                var checkStatus = _dbContext.BC_GUESTMEALBOOKING_DTL.Count(a => a.TOKEN_CODE == tokenCode) > 0;

                if (checkStatus)
                {
                    retVal = 2; //// -- Token already exist.
                    return retVal;
                }

                if (_dbContext.BC_GUESTMEALBOOKING_DTL.Count() == 0)
                {
                    GMB_DTL.BC_BOOKINGDTLID = 1;
                }
                else
                {
                    GMB_DTL.BC_BOOKINGDTLID = _dbContext.BC_GUESTMEALBOOKING_DTL.Max(x => x.BC_BOOKINGDTLID) + 1;
                }

                GMB_DTL.GUESTDTLID = guestModel.BC_GUESTID;
                GMB_DTL.BC_GUESTMEALBOOKINGID = model.BC_GUEST_BOOKINGID;
                GMB_DTL.TOKEN_CODE = tokenCode;
                GMB_DTL.MEAL_PRICE = model.MEAL_PRICE;
                GMB_DTL.ADDEDBY = model.ADDEDBY;
                GMB_DTL.ADDEDDATE = DateTime.Now;
                _dbContext.Entry(GMB_DTL).State = EntityState.Added;
                _dbContext.SaveChanges();
                retVal = 1;
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }

        public long GetApprovalAuth(long _ecode, string[] _designationIds)
        {
            long _approver_Ecode = 0;
            var authObj = (from data in _dbContext.ADEMPLOYEE
                           join _VW in _dbContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VW.ADEMPCODE
                           join _AdCoor in _dbContext.ADORGCOORDINATOR on _VW.OPERATIONID equals _AdCoor.ADORGLEVELID into _AdCoorJoin
                           from AdCoordinator in _AdCoorJoin.Where(o => o.ISACTIVE == 1).DefaultIfEmpty()
                           where data.ADEMPCODE == _ecode && data.ACTIVE == 1 && _VW.SYKI == _Syki.SYKIID
                           select new
                           {
                               ECODE = data.ADEMPCODE,
                               OPERATING_HEAD = AdCoordinator.OPHEAD,
                               DIRECTOR = AdCoordinator.DIRECTOR,
                               DIRECTOR2 = AdCoordinator.DIRECTOR2,
                               DESGID = _VW.ADDESIGNATIONID,
                               FNDESGID = _VW.ADFUNCTIONALDESIGNATIONID,
                           }).FirstOrDefault();

            if (authObj != null)
            {
                //long[] _desgIds = { 12, 26, 27, 28, 29, 30, 31, 34 }; //// Desgination ids for self approval
                if (Convert.ToInt64(authObj.FNDESGID) == 4 || _designationIds.Contains(Convert.ToInt64(authObj.DESGID).ToString()))
                {
                    _approver_Ecode = authObj.ECODE;
                }
                else
                {
                    if (authObj.ECODE > 70000000 && authObj.ECODE <= 79999999)
                    {
                        if (Convert.ToInt64(authObj.DIRECTOR) > 70000000 && Convert.ToInt64(authObj.DIRECTOR) <= 79999999)
                        {
                            _approver_Ecode = Convert.ToInt64(authObj.DIRECTOR);
                        }
                        else if (Convert.ToInt64(authObj.DIRECTOR2) > 70000000 && Convert.ToInt64(authObj.DIRECTOR2) <= 79999999)
                        {
                            _approver_Ecode = Convert.ToInt64(authObj.DIRECTOR2);
                        }
                    }
                    else
                    {
                        _approver_Ecode = Convert.ToInt64(authObj.OPERATING_HEAD);
                    }
                }
            }
            return _approver_Ecode;
        }

        public short CancelGuestMealBooking(long id, long updatedBy)
        {
            short retVal = 0;
            try
            {
                BC_GUESTMEALBOOKING_TRN_DGIT model = new BC_GUESTMEALBOOKING_TRN_DGIT();
                model = _dbContext.BC_GUESTMEALBOOKING_TRN.Where(w => w.BC_GUEST_BOOKINGID == id).FirstOrDefault();
                if (model != null)
                {
                    model.MEAL_STATUS = 2;
                    model.LSTMODIFIEDBY = updatedBy;
                    model.LSTMODDATE = DateTime.Now;
                    _dbContext.Entry(model).State = EntityState.Modified;
                    _dbContext.SaveChanges();

                    retVal = 1;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }



        public short GuestMealApproval(GuestMealBookingTrnViewModel GMBT)
        {
            short retVal = 0;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    BC_GUESTMEALBOOKING_TRN_DGIT _Approval = new BC_GUESTMEALBOOKING_TRN_DGIT();
                    if (GMBT.BC_GUEST_BOOKINGID > 0)
                    {
                        _Approval = _dbContext.BC_GUESTMEALBOOKING_TRN.FirstOrDefault(a => a.BC_GUEST_BOOKINGID == GMBT.BC_GUEST_BOOKINGID && a.APPROVE_STATUS == 0);
                        if (_Approval != null)
                        {
                            short IsCommit = 0;
                            if (_Approval.APPROVER_ECODE == GMBT.APPROVER_ECODE)
                            {
                                _Approval.APPROVE_STATUS = GMBT.APPROVE_STATUS;
                                if (GMBT.APPROVE_STATUS == 1) // Approved
                                {
                                    _Approval.MEAL_STATUS = 1;
                                }
                                else if (GMBT.APPROVE_STATUS == 2) // Rejected
                                {
                                    _Approval.MEAL_STATUS = 3;
                                }
                                _Approval.APPROVE_DATE = DateTime.Now;
                                _Approval.APPROVE_REMARK = GMBT.APPROVE_REMARK;
                                _Approval.LSTMODDATE = DateTime.Now;
                                _Approval.LSTMODIFIEDBY = GMBT.LSTMODIFIEDBY;
                                IsCommit = 1;
                            }

                            if (IsCommit == 1)
                            {
                                _dbContext.Entry(_Approval).State = EntityState.Modified;
                                _dbContext.SaveChanges();
                                retVal = 1;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    retVal = -1;
                }

                if (retVal == 1)
                {
                    transaction.Commit();
                }
                else
                {
                    transaction.Rollback();
                }
            }
            return retVal;
        }

        public List<GuestDtlViewModel> GuestAutocomplete(string Key)
        {
            List<GuestDtlViewModel> guestList = new List<GuestDtlViewModel>();
            guestList = (from data in _dbContext.BC_GUEST_DTL
                         where (data.MOBILE_NUMBER.ToString().StartsWith(Key) || data.GUEST_NAME.ToUpper().Contains(Key.ToUpper()) || data.COMPANY.ToUpper().Contains(Key.ToUpper()))
                         select new GuestDtlViewModel
                         {
                             BC_GUESTID = data.BC_GUESTID,
                             MOBILE_NUMBER = data.MOBILE_NUMBER,
                             GUEST_NAME = data.GUEST_NAME,
                             COMPANY = data.COMPANY,
                         }).ToList();
            return guestList;
        }

        public GuestMealBookingTrnViewModel GetGuestRequestDtlById(long id)
        {
            //var obj = (from data in _dbContext.BC_GUESTMEALBOOKING_TRN.Where(w => w.BC_GUEST_BOOKINGID == id)
            //           join _ApproverAuth in _dbContext.ADEMPLOYEE on data.APPROVER_ECODE equals _ApproverAuth.ADEMPCODE into _ApproverAuthJoin
            //           from _Approver in _ApproverAuthJoin.DefaultIfEmpty()
            //           join _mealType in _dbContext.BC_MST_MEAL_TYPE on data.MEALTYPEID equals _mealType.BC_MEALTYPEID
            //           join _mealAvail in _dbContext.BC_MEALS_AVAILABILITY on data.MEALS_AVAILABILITY_ID equals _mealAvail.BC_MEALS_AVAILABILITY_ID
            //           join _mealMst in _dbContext.BC_MST_MEALS on _mealAvail.MEALS_ID equals _mealMst.BC_MEALID
            //           join _AddBy in _dbContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
            //           join _VWAssociate in _dbContext.VW_ASSOCIATELVLDETAILS on data.ADDEDBY equals _VWAssociate.ADEMPCODE
            //           join _slotMst in _dbContext.BC_MEALSLOTMST on data.SLOTID equals _slotMst.BCSLOTMSTID
            //           where _VWAssociate.SYKI == _Syki.SYKIID
            //           select new GuestMealBookingTrnViewModel
            //           {
            //               BC_GUEST_BOOKINGID = data.BC_GUEST_BOOKINGID,
            //               MEALTYPEID = data.MEALTYPEID,
            //               MealType = new MealTypeViewModel
            //               {
            //                   BC_MEALTYPEID = _mealType.BC_MEALTYPEID,
            //                   MEAL_TYPE_DESC = _mealType.MEAL_TYPE_DESC,
            //               },
            //               PAYMENT_TYPE = data.PAYMENT_TYPE,
            //               MEAL_BOOKED_DATE = data.MEAL_BOOKED_DATE,
            //               MEAL_STATUS = data.MEAL_STATUS,
            //               MEALS_AVAILABILITY_ID = data.MEALS_AVAILABILITY_ID,
            //               APPROVER_ECODE = data.APPROVER_ECODE,
            //               APPROVER_EMAIL = (_Approver == null ? "" : _Approver.EMAILID),
            //               APPROVER_FNAME = (_Approver == null ? "" : _Approver.FIRSTNAME),
            //               APPROVER_LNAME = (_Approver == null ? "" : _Approver.LASTNAME),
            //               APPROVE_STATUS = _Approver == null ? (short)-1 : data.APPROVE_STATUS,
            //               APPROVE_DATE = data.APPROVE_DATE,
            //               APPROVE_REMARK = data.APPROVE_REMARK,
            //               GuestDtlList = (from _guestData in _dbContext.BC_GUESTMEALBOOKING_DTL.Where(d => d.BC_GUESTMEALBOOKINGID == data.BC_GUEST_BOOKINGID)
            //                               join _Guest in _dbContext.BC_GUEST_DTL on _guestData.GUESTDTLID equals _Guest.BC_GUESTID
            //                               select new GuestDtlViewModel
            //                               {
            //                                   BC_GUESTID = _Guest.BC_GUESTID,
            //                                   GUEST_NAME = _Guest.GUEST_NAME,
            //                                   MOBILE_NUMBER = _Guest.MOBILE_NUMBER,
            //                                   COMPANY = _Guest.COMPANY,
            //                                   TOKEN_CODE = _guestData.TOKEN_CODE,
            //                                   MEAL_PRICE = _guestData.MEAL_PRICE,
            //                                   ADMIN_STATUS = _guestData.ADMIN_STAUS
            //                               }).ToList(),
            //               MealMst = new BCMstMealViewModel
            //               {
            //                   BC_MEALID = _mealMst.BC_MEALID,
            //                   MEALTYPEID = _mealMst.MEALTYPEID,
            //                   MEAL_NAME = _mealMst.MEAL_NAME,
            //                   MEAL_PRICE = _mealMst.MEAL_PRICE,
            //                   MEAL_DESC = _mealMst.MEAL_DESC,
            //                   MEAL_PHOTO = _mealMst.MEAL_PHOTO,
            //                   MEAL_PHOTO_NAME = _mealMst.MEAL_PHOTO_NAME,
            //               },
            //               Emp_Detail = new Employee_Details
            //               {
            //                   _ECode = _AddBy.ADEMPCODE,
            //                   _EName = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
            //                   _EmailId = _AddBy.EMAILID,
            //                   _DOB = DateTime.Now,
            //                   _SecDescrip = _VWAssociate.SECTION,
            //                   _DepDesc = _VWAssociate.DEPARTMENT,
            //                   _DivDesc = _VWAssociate.DIVISION,
            //                   _OpDesc = _VWAssociate.OPERATION,
            //                   _SiteId = _VWAssociate.SYSITEID
            //               },
            //               ADDEDBY = data.ADDEDBY,
            //               ADDEDDATE = data.ADDEDDATE,
            //               SLOTMSTID = data.SLOTID,
            //               SLOT_TIME = _slotMst.SLOT_TIME
            //           }).FirstOrDefault();
            //return obj;


            var data = (from trn in _dbContext.BC_GUESTMEALBOOKING_TRN
                        where trn.BC_GUEST_BOOKINGID == id
                        join _Approver in _dbContext.ADEMPLOYEE on trn.APPROVER_ECODE equals _Approver.ADEMPCODE into approverJoin
                        from approver in approverJoin.DefaultIfEmpty()
                        join _mealType in _dbContext.BC_MST_MEAL_TYPE on trn.MEALTYPEID equals _mealType.BC_MEALTYPEID
                        join _mealAvail in _dbContext.BC_MEALS_AVAILABILITY on trn.MEALS_AVAILABILITY_ID equals _mealAvail.BC_MEALS_AVAILABILITY_ID
                        join _mealMst in _dbContext.BC_MST_MEALS on _mealAvail.MEALS_ID equals _mealMst.BC_MEALID
                        join _AddBy in _dbContext.ADEMPLOYEE on trn.ADDEDBY equals _AddBy.ADEMPCODE
                        join _VWAssociate in _dbContext.VW_ASSOCIATELVLDETAILS on trn.ADDEDBY equals _VWAssociate.ADEMPCODE
                        join _slotMst in _dbContext.BC_MEALSLOTMST on trn.SLOTID equals _slotMst.BCSLOTMSTID
                        where _VWAssociate.SYKI == _Syki.SYKIID
                        select new
                        {
                            trn,
                            approver,
                            _mealType,
                            _mealAvail,
                            _mealMst,
                            _AddBy,
                            _VWAssociate,
                            _slotMst
                        }).FirstOrDefault();

            if (data == null) return null;

            var guestDtlList = (from _guestData in _dbContext.BC_GUESTMEALBOOKING_DTL
                                where _guestData.BC_GUESTMEALBOOKINGID == data.trn.BC_GUEST_BOOKINGID
                                join _Guest in _dbContext.BC_GUEST_DTL on _guestData.GUESTDTLID equals _Guest.BC_GUESTID
                                select new GuestDtlViewModel
                                {
                                    BC_GUESTID = _Guest.BC_GUESTID,
                                    GUEST_NAME = _Guest.GUEST_NAME,
                                    MOBILE_NUMBER = _Guest.MOBILE_NUMBER,
                                    COMPANY = _Guest.COMPANY,
                                    TOKEN_CODE = _guestData.TOKEN_CODE,
                                    MEAL_PRICE = _guestData.MEAL_PRICE,
                                    ADMIN_STATUS = _guestData.ADMIN_STAUS
                                }).ToList();




            var obj = new GuestMealBookingTrnViewModel
            {
                BC_GUEST_BOOKINGID = data.trn.BC_GUEST_BOOKINGID,
                MEALTYPEID = data.trn.MEALTYPEID,
                MealType = new MealTypeViewModel
                {
                    BC_MEALTYPEID = data._mealType.BC_MEALTYPEID,
                    MEAL_TYPE_DESC = data._mealType.MEAL_TYPE_DESC,
                },
                PAYMENT_TYPE = data.trn.PAYMENT_TYPE,
                MEAL_BOOKED_DATE = data.trn.MEAL_BOOKED_DATE,
                MEAL_STATUS = data.trn.MEAL_STATUS,
                MEALS_AVAILABILITY_ID = data.trn.MEALS_AVAILABILITY_ID,
                APPROVER_ECODE = data.trn.APPROVER_ECODE,
                APPROVER_EMAIL = data.approver?.EMAILID ?? "",
                APPROVER_FNAME = data.approver?.FIRSTNAME ?? "",
                APPROVER_LNAME = data.approver?.LASTNAME ?? "",
                APPROVE_STATUS = data.approver == null ? (short)-1 : data.trn.APPROVE_STATUS,
                APPROVE_DATE = data.trn.APPROVE_DATE,
                APPROVE_REMARK = data.trn.APPROVE_REMARK,
                GuestDtlList = guestDtlList,
                MealMst = new BCMstMealViewModel
                {
                    BC_MEALID = data._mealMst.BC_MEALID,
                    MEALTYPEID = data._mealMst.MEALTYPEID,
                    MEAL_NAME = data._mealMst.MEAL_NAME,
                    MEAL_PRICE = data._mealMst.MEAL_PRICE,
                    MEAL_DESC = data._mealMst.MEAL_DESC,
                    MEAL_PHOTO = data._mealMst.MEAL_PHOTO,
                    MEAL_PHOTO_NAME = data._mealMst.MEAL_PHOTO_NAME,
                },
                Emp_Detail = new Employee_Details
                {
                    _ECode = data._AddBy.ADEMPCODE,
                    _EName = data._AddBy.FIRSTNAME + " " + data._AddBy.LASTNAME,
                    _EmailId = data._AddBy.EMAILID,
                    _DOB = DateTime.Now,
                    _SecDescrip = data._VWAssociate.SECTION,
                    _DepDesc = data._VWAssociate.DEPARTMENT,
                    _DivDesc = data._VWAssociate.DIVISION,
                    _OpDesc = data._VWAssociate.OPERATION,
                    _SiteId = data._VWAssociate.SYSITEID
                },
                ADDEDBY = data.trn.ADDEDBY,
                ADDEDDATE = data.trn.ADDEDDATE,
                SLOTMSTID = data.trn.SLOTID,
                SLOT_TIME = data._slotMst.SLOT_TIME
            };


            return obj;
        }

        public Tuple<long, short> GetGuestDtlByMno(string mNo, string date)
        {
            long guestDtlId = 0; short bookStatus = 0;
            Tuple<long, short> _tuple = new Tuple<long, short>(guestDtlId, bookStatus);
            DateTime bookingDate = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(date))
            {
                bookingDate = DateTime.ParseExact(date, "dd-MMM-yyyy", null);
            }
            long mobileNo = Convert.ToInt64(mNo);

            GuestDtlViewModel guestDtl = new GuestDtlViewModel();
            guestDtl = (from data in _dbContext.BC_GUEST_DTL
                        where data.MOBILE_NUMBER == mobileNo
                        select new GuestDtlViewModel
                        {
                            BC_GUESTID = data.BC_GUESTID,
                            MOBILE_NUMBER = data.MOBILE_NUMBER,
                            GUEST_NAME = data.GUEST_NAME,
                            COMPANY = data.COMPANY,
                        }).FirstOrDefault();

            if (guestDtl != null)
            {
                guestDtlId = guestDtl.BC_GUESTID;
                var existCount = (from _guestDtl in _dbContext.BC_GUESTMEALBOOKING_DTL.Where(g => g.GUESTDTLID == guestDtl.BC_GUESTID)
                                  join _trn in _dbContext.BC_GUESTMEALBOOKING_TRN on _guestDtl.BC_GUESTMEALBOOKINGID equals _trn.BC_GUEST_BOOKINGID
                                  where _trn.MEAL_BOOKED_DATE == bookingDate && _trn.MEAL_STATUS != 2 && _trn.MEAL_STATUS != 3
                                  select _guestDtl).Count();
                if (existCount > 0)
                {
                    bookStatus = 1;
                }
                _tuple = new Tuple<long, short>(guestDtlId, bookStatus);
            }
            return _tuple;
        }

        // -- Ala Carte Food --//
        public List<AlaCarteHeaderViewModel> GetAlaCarteRequestList(string fromDate, string toDate, short category, long loginUser)
        {
            DateTime orderFromDate = DateTime.Now.Date;
            DateTime orderToDate = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(fromDate))
            {
                orderFromDate = DateTime.ParseExact(fromDate, "dd-MMM-yyyy", null);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                orderToDate = DateTime.ParseExact(toDate + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
            }
            var headerList = (from data in _dbContext.BC_ALACARTEFOOD_HDR.Where(w => w.ADDEDBY == loginUser)
                              join _AddBy in _dbContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                              join _VWAssociate in _dbContext.VW_ASSOCIATELVLDETAILS on data.ADDEDBY equals _VWAssociate.ADEMPCODE
                              where
                                     // (!string.IsNullOrEmpty(fromDate) ? (data.ORDER_DATE >= orderFromDate) : true)
                                     //&& (!string.IsNullOrEmpty(toDate) ? (data.ORDER_DATE <= orderToDate) : true)
                                     (string.IsNullOrEmpty(fromDate) || (data.ORDER_DATE >= orderFromDate))
                                    && (string.IsNullOrEmpty(toDate) || (data.ORDER_DATE <= orderToDate))
                                    && _VWAssociate.SYKI == _Syki.SYKIID
                              select new AlaCarteHeaderViewModel
                              {
                                  ALACARTEFOODHDR_ID = data.ALACARTEFOODHDR_ID,
                                  ORDER_DATE = data.ORDER_DATE,
                                  ORDER_STATUS = data.ORDER_STATUS,
                                  //TrnModel = (from _TrnData in _dbContext.BC_ALACARTEFOOD_TRN.Where(d => d.ALACARTEFOODHDR_ID == data.ALACARTEFOODHDR_ID)
                                  //            join _ItemMst in _dbContext.BC_ALACARTEFOOD_MST on _TrnData.ITEMID equals _ItemMst.BC_FOODMSTID
                                  //            //where (category == 0 ? true : _ItemMst.ITEM_CATEGORY == category)
                                  //            select new AlaCarteTrnViewModel
                                  //            {
                                  //                ALACARTEFOODTRN_ID = _TrnData.ALACARTEFOODTRN_ID,
                                  //                ALACARTEFOODHDR_ID = _TrnData.ALACARTEFOODHDR_ID,
                                  //                ITEMID = _TrnData.ITEMID,
                                  //                ItemModel = new AlaCarteItemMstViewModel
                                  //                {
                                  //                    BC_FOODMSTID = _ItemMst.BC_FOODMSTID,
                                  //                    ITEM_NAME = _ItemMst.ITEM_NAME,
                                  //                    ITEM_CATEGORY = _ItemMst.ITEM_CATEGORY,
                                  //                    LEAD_TIME = _ItemMst.LEAD_TIME,
                                  //                },
                                  //                ITEM_PRICE = _TrnData.ITEM_PRICE,
                                  //                ITEM_QTY = _TrnData.ITEM_QTY,
                                  //                DELIVERY_TYPE = _TrnData.DELIVERY_TYPE
                                  //            }).ToList(),
                                  Emp_Detail = new Employee_Details
                                  {
                                      _ECode = _AddBy.ADEMPCODE,
                                      _EName = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                                      _EmailId = _AddBy.EMAILID,
                                      _DOB = DateTime.Now,
                                      _SecDescrip = _VWAssociate.SECTION,
                                      _DepDesc = _VWAssociate.DEPARTMENT,
                                      _DivDesc = _VWAssociate.DIVISION,
                                      _OpDesc = _VWAssociate.OPERATION,
                                      _SiteId = _VWAssociate.SYSITEID
                                  },
                                  ADDEDBY = data.ADDEDBY,
                                  ADDEDDATE = data.ADDEDDATE,
                                  LSTMODDATE = data.LSTMODDATE,
                                  LSTMODIFIEDBY = data.LSTMODIFIEDBY,
                              }).ToList();


            // 🔄 Now load TRN list for each header separately
            foreach (var header in headerList)
            {
                var trnQuery = from trn in _dbContext.BC_ALACARTEFOOD_TRN
                               join item in _dbContext.BC_ALACARTEFOOD_MST on trn.ITEMID equals item.BC_FOODMSTID
                               where trn.ALACARTEFOODHDR_ID == header.ALACARTEFOODHDR_ID
                               select new AlaCarteTrnViewModel
                               {
                                   ALACARTEFOODTRN_ID = trn.ALACARTEFOODTRN_ID,
                                   ALACARTEFOODHDR_ID = trn.ALACARTEFOODHDR_ID,
                                   ITEMID = trn.ITEMID,
                                   ItemModel = new AlaCarteItemMstViewModel
                                   {
                                       BC_FOODMSTID = item.BC_FOODMSTID,
                                       ITEM_NAME = item.ITEM_NAME,
                                       ITEM_CATEGORY = item.ITEM_CATEGORY,
                                       LEAD_TIME = item.LEAD_TIME
                                   },
                                   ITEM_PRICE = trn.ITEM_PRICE,
                                   ITEM_QTY = trn.ITEM_QTY,
                                   DELIVERY_TYPE = trn.DELIVERY_TYPE
                               };

                //// Optional: apply category filter
                //if (category > 0)
                //    trnQuery = trnQuery.Where(x => x.ItemModel.ITEM_CATEGORY == category);

                header.TrnModel = trnQuery.ToList();
            }


            return headerList.OrderByDescending(o => o.ORDER_DATE).ToList();
        }

        public List<AlaCarteItemMstViewModel> GetAlaCarteItemList()
        {
            List<AlaCarteItemMstViewModel> items = new List<AlaCarteItemMstViewModel>();
            items = _dbContext.BC_ALACARTEFOOD_MST
                     .Where(data => data.STATUS == 1)
                     .AsEnumerable()
                     .Select(data => new AlaCarteItemMstViewModel
                     {
                         BC_FOODMSTID = data.BC_FOODMSTID,
                         ITEM_CATEGORY = data.ITEM_CATEGORY,
                         ITEM_NAME = data.ITEM_NAME,
                         ITEM_PRICE = data.ITEM_PRICE,
                         DINE_IN = data.DINE_IN == 1 ? true : false,
                         TAKE_WAY = data.TAKE_WAY == 1 ? true : false,
                         LEAD_TIME = data.LEAD_TIME,
                         STATUS = data.STATUS,
                         ITEM_PHOTO = data.ITEM_PHOTO,
                         ITEM_PHOTO_NAME = data.ITEM_PHOTO_NAME
                     }).ToList();
            return items;

            //List<AlaCarteItemMstViewModel> items = new List<AlaCarteItemMstViewModel>();
            //try
            //{
            //    items = (from data in _dbContext.BC_ALACARTEFOOD_MST
            //             where data.STATUS == 1
            //             select new AlaCarteItemMstViewModel
            //             {
            //                 BC_FOODMSTID = data.BC_FOODMSTID,
            //                 ITEM_CATEGORY = data.ITEM_CATEGORY,
            //                 ITEM_NAME = data.ITEM_NAME,
            //                 ITEM_PRICE = data.ITEM_PRICE,
            //                 DINE_IN = data.DINE_IN == 1 ? true : false,
            //                 TAKE_WAY = data.TAKE_WAY == 1 ? true : false,
            //                 LEAD_TIME = data.LEAD_TIME,
            //                 STATUS = data.STATUS,
            //                 ITEM_PHOTO = data.ITEM_PHOTO,
            //                 ITEM_PHOTO_NAME = data.ITEM_PHOTO_NAME
            //             }).ToList();
            //}
            //catch (Exception)
            //{

            //    throw;
            //}


            //return items;
        }

        public Tuple<short, string> SaveAlaCarteOrder(long AddedBy, List<AlaCarteTrnViewModel> modelList)
        {
            short retVal = 0;
            string msg = "";
            Tuple<short, string> _tuple = new Tuple<short, string>(retVal, msg);
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    BC_ALACARTEFOOD_HDR_DGIT BAH = new BC_ALACARTEFOOD_HDR_DGIT();

                    if (_dbContext.BC_ALACARTEFOOD_HDR.Count() == 0)
                    {
                        BAH.ALACARTEFOODHDR_ID = 1;
                    }
                    else
                    {
                        BAH.ALACARTEFOODHDR_ID = _dbContext.BC_ALACARTEFOOD_HDR.Max(x => x.ALACARTEFOODHDR_ID) + 1;
                    }

                    BAH.ORDER_DATE = DateTime.Now;
                    BAH.ORDER_STATUS = 0;
                    BAH.ADDEDBY = AddedBy;
                    BAH.ADDEDDATE = DateTime.Now;
                    _dbContext.Entry(BAH).State = EntityState.Added;
                    _dbContext.SaveChanges();

                    short _retVal = SaveAlaCarteTrn(AddedBy, BAH.ALACARTEFOODHDR_ID, modelList);
                    if (_retVal == 1)
                    {
                        transaction.Commit();
                        retVal = 1;
                    }
                    else
                    {
                        retVal = 0;
                        transaction.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
            }
            _tuple = new Tuple<short, string>(retVal, msg);
            return _tuple;
        }

        public short SaveAlaCarteTrn(long AddedBy, long HeaderId, List<AlaCarteTrnViewModel> modelList)
        {
            short retVal = 0;
            foreach (AlaCarteTrnViewModel model in modelList)
            {
                BC_ALACARTEFOOD_TRN_DGIT BAT = new BC_ALACARTEFOOD_TRN_DGIT();
                if (HeaderId > 0)
                {
                    if (_dbContext.BC_ALACARTEFOOD_TRN.Count() == 0)
                    {
                        BAT.ALACARTEFOODTRN_ID = 1;
                    }
                    else
                    {
                        BAT.ALACARTEFOODTRN_ID = _dbContext.BC_ALACARTEFOOD_TRN.Max(x => x.ALACARTEFOODTRN_ID) + 1;
                    }

                    BAT.ALACARTEFOODHDR_ID = HeaderId;
                    BAT.ITEMID = model.ITEMID;
                    BAT.ITEM_PRICE = model.ITEM_PRICE;
                    BAT.ITEM_QTY = model.ITEM_QTY;
                    BAT.DELIVERY_TYPE = model.DELIVERY_TYPE;
                    BAT.STATUS = 1;
                    BAT.ADDEDBY = AddedBy;
                    BAT.ADDEDDATE = DateTime.Now;
                    _dbContext.Entry(BAT).State = EntityState.Added;
                    _dbContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }

        public AlaCarteHeaderViewModel GetAlaCarteDtlById(long id)
        {
            AlaCarteHeaderViewModel result = new AlaCarteHeaderViewModel();

            //var obj = (from data in _dbContext.BC_ALACARTEFOOD_HDR
            //           join _AddBy in _dbContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
            //           join _VWAssociate in _dbContext.VW_ASSOCIATELVLDETAILS on data.ADDEDBY equals _VWAssociate.ADEMPCODE
            //           where data.ALACARTEFOODHDR_ID == id && _VWAssociate.SYKI == _Syki.SYKIID
            //           select new AlaCarteHeaderViewModel
            //           {
            //               ALACARTEFOODHDR_ID = data.ALACARTEFOODHDR_ID,
            //               ORDER_DATE = data.ORDER_DATE,
            //               ORDER_STATUS = data.ORDER_STATUS,
            //               TrnModel = (from _TrnData in _dbContext.BC_ALACARTEFOOD_TRN.Where(d => d.ALACARTEFOODHDR_ID == data.ALACARTEFOODHDR_ID)
            //                           join _ItemMst in _dbContext.BC_ALACARTEFOOD_MST on _TrnData.ITEMID equals _ItemMst.BC_FOODMSTID
            //                           select new AlaCarteTrnViewModel
            //                           {
            //                               ALACARTEFOODTRN_ID = _TrnData.ALACARTEFOODTRN_ID,
            //                               ALACARTEFOODHDR_ID = _TrnData.ALACARTEFOODHDR_ID,
            //                               ITEMID = _TrnData.ITEMID,
            //                               ItemModel = new AlaCarteItemMstViewModel
            //                               {
            //                                   BC_FOODMSTID = _ItemMst.BC_FOODMSTID,
            //                                   ITEM_NAME = _ItemMst.ITEM_NAME,
            //                                   ITEM_CATEGORY = _ItemMst.ITEM_CATEGORY,
            //                                   ITEM_PRICE = _ItemMst.ITEM_PRICE,
            //                                   LEAD_TIME = _ItemMst.LEAD_TIME,


            //                               },
            //                               ITEM_PRICE = _TrnData.ITEM_PRICE,
            //                               ITEM_QTY = _TrnData.ITEM_QTY,
            //                               DELIVERY_TYPE = _TrnData.DELIVERY_TYPE,
            //                               ITEM_STATUS = _TrnData.ITEM_STATUS,
            //                           }).ToList(),
            //               Emp_Detail = new Employee_Details
            //               {
            //                   _ECode = _AddBy.ADEMPCODE,
            //                   _EName = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
            //                   _EmailId = _AddBy.EMAILID,
            //                   _DOB = DateTime.Now,
            //                   _SecDescrip = _VWAssociate.SECTION,
            //                   _DepDesc = _VWAssociate.DEPARTMENT,
            //                   _DivDesc = _VWAssociate.DIVISION,
            //                   _OpDesc = _VWAssociate.OPERATION,
            //                   _SiteId = _VWAssociate.SYSITEID
            //               },
            //               ADDEDBY = data.ADDEDBY,
            //               ADDEDDATE = data.ADDEDDATE,
            //               LSTMODDATE = data.LSTMODDATE,
            //               LSTMODIFIEDBY = data.LSTMODIFIEDBY,
            //               ACCEPTED_DATE = data.ACCEPTED_DATE,

            //           }).FirstOrDefault();


            //var obj = (from data in _dbContext.BC_ALACARTEFOOD_HDR
            //           join _AddBy in _dbContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
            //           join _VWAssociate in _dbContext.VW_ASSOCIATELVLDETAILS on data.ADDEDBY equals _VWAssociate.ADEMPCODE
            //           where data.ALACARTEFOODHDR_ID == id && _VWAssociate.SYKI == _Syki.SYKIID


            // Step 1: Get the header data
            var header = (from data in _dbContext.BC_ALACARTEFOOD_HDR
                          join _AddBy in _dbContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                          join _VWAssociate in _dbContext.VW_ASSOCIATELVLDETAILS on data.ADDEDBY equals _VWAssociate.ADEMPCODE
                          where data.ALACARTEFOODHDR_ID == id && _VWAssociate.SYKI == _Syki.SYKIID
                          select new
                          {
                              data.ALACARTEFOODHDR_ID,
                              data.ORDER_DATE,
                              data.ORDER_STATUS,
                              data.ADDEDBY,
                              data.ADDEDDATE,
                              data.LSTMODDATE,
                              data.LSTMODIFIEDBY,
                              data.ACCEPTED_DATE,
                              _AddBy,
                              _VWAssociate
                          }).FirstOrDefault();

            if (header == null) return result;


            //           select new AlaCarteHeaderViewModel
            //           {
            //               ALACARTEFOODHDR_ID = data.ALACARTEFOODHDR_ID,
            //               ORDER_DATE = data.ORDER_DATE,
            //               ORDER_STATUS = data.ORDER_STATUS,
            //               TrnModel = (from _TrnData in _dbContext.BC_ALACARTEFOOD_TRN.Where(d => d.ALACARTEFOODHDR_ID == data.ALACARTEFOODHDR_ID)
            //                           join _ItemMst in _dbContext.BC_ALACARTEFOOD_MST on _TrnData.ITEMID equals _ItemMst.BC_FOODMSTID
            //                           select new AlaCarteTrnViewModel
            //                           {
            //                               ALACARTEFOODTRN_ID = _TrnData.ALACARTEFOODTRN_ID,
            //                               ALACARTEFOODHDR_ID = _TrnData.ALACARTEFOODHDR_ID,
            //                               ITEMID = _TrnData.ITEMID,
            //                               ItemModel = new AlaCarteItemMstViewModel
            //                               {
            //                                   BC_FOODMSTID = _ItemMst.BC_FOODMSTID,
            //                                   ITEM_NAME = _ItemMst.ITEM_NAME,
            //                                   ITEM_CATEGORY = _ItemMst.ITEM_CATEGORY,
            //                                   ITEM_PRICE = _ItemMst.ITEM_PRICE,
            //                                   LEAD_TIME = _ItemMst.LEAD_TIME,


            //                               },
            //                               ITEM_PRICE = _TrnData.ITEM_PRICE,
            //                               ITEM_QTY = _TrnData.ITEM_QTY,
            //                               DELIVERY_TYPE = _TrnData.DELIVERY_TYPE,
            //                               ITEM_STATUS = _TrnData.ITEM_STATUS,
            //                           }).ToList(),

            // Step 2: Load transactions separately
            var trnList = (from _TrnData in _dbContext.BC_ALACARTEFOOD_TRN
                           where _TrnData.ALACARTEFOODHDR_ID == header.ALACARTEFOODHDR_ID
                           join _ItemMst in _dbContext.BC_ALACARTEFOOD_MST on _TrnData.ITEMID equals _ItemMst.BC_FOODMSTID
                           select new AlaCarteTrnViewModel
                           {
                               ALACARTEFOODTRN_ID = _TrnData.ALACARTEFOODTRN_ID,
                               ALACARTEFOODHDR_ID = _TrnData.ALACARTEFOODHDR_ID,
                               ITEMID = _TrnData.ITEMID,
                               ITEM_PRICE = _TrnData.ITEM_PRICE,
                               ITEM_QTY = _TrnData.ITEM_QTY,
                               DELIVERY_TYPE = _TrnData.DELIVERY_TYPE,
                               ITEM_STATUS = _TrnData.ITEM_STATUS,
                               ItemModel = new AlaCarteItemMstViewModel
                               {
                                   BC_FOODMSTID = _ItemMst.BC_FOODMSTID,
                                   ITEM_NAME = _ItemMst.ITEM_NAME,
                                   ITEM_CATEGORY = _ItemMst.ITEM_CATEGORY,
                                   ITEM_PRICE = _ItemMst.ITEM_PRICE,
                                   LEAD_TIME = _ItemMst.LEAD_TIME,
                               }
                           }).ToList();

            //           select new AlaCarteHeaderViewModel
            //           {
            //               ALACARTEFOODHDR_ID = data.ALACARTEFOODHDR_ID,
            //               ORDER_DATE = data.ORDER_DATE,
            //               ORDER_STATUS = data.ORDER_STATUS,
            //               TrnModel = (from _TrnData in _dbContext.BC_ALACARTEFOOD_TRN.Where(d => d.ALACARTEFOODHDR_ID == data.ALACARTEFOODHDR_ID)
            //                           join _ItemMst in _dbContext.BC_ALACARTEFOOD_MST on _TrnData.ITEMID equals _ItemMst.BC_FOODMSTID
            //                           select new AlaCarteTrnViewModel
            //                           {
            //                               ALACARTEFOODTRN_ID = _TrnData.ALACARTEFOODTRN_ID,
            //                               ALACARTEFOODHDR_ID = _TrnData.ALACARTEFOODHDR_ID,
            //                               ITEMID = _TrnData.ITEMID,
            //                               ItemModel = new AlaCarteItemMstViewModel
            //                               {
            //                                   BC_FOODMSTID = _ItemMst.BC_FOODMSTID,
            //                                   ITEM_NAME = _ItemMst.ITEM_NAME,
            //                                   ITEM_CATEGORY = _ItemMst.ITEM_CATEGORY,
            //                                   ITEM_PRICE = _ItemMst.ITEM_PRICE,
            //                                   LEAD_TIME = _ItemMst.LEAD_TIME,


            //                               },
            //                               ITEM_PRICE = _TrnData.ITEM_PRICE,
            //                               ITEM_QTY = _TrnData.ITEM_QTY,
            //                               DELIVERY_TYPE = _TrnData.DELIVERY_TYPE,
            //                               ITEM_STATUS = _TrnData.ITEM_STATUS,
            //                           }).ToList(),
            //               Emp_Detail = new Employee_Details
            //               {
            //                   _ECode = _AddBy.ADEMPCODE,
            //                   _EName = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
            //                   _EmailId = _AddBy.EMAILID,
            //                   _DOB = DateTime.Now,
            //                   _SecDescrip = _VWAssociate.SECTION,
            //                   _DepDesc = _VWAssociate.DEPARTMENT,
            //                   _DivDesc = _VWAssociate.DIVISION,
            //                   _OpDesc = _VWAssociate.OPERATION,
            //                   _SiteId = _VWAssociate.SYSITEID
            //               },
            //               ADDEDBY = data.ADDEDBY,
            //               ADDEDDATE = data.ADDEDDATE,
            //               LSTMODDATE = data.LSTMODDATE,
            //               LSTMODIFIEDBY = data.LSTMODIFIEDBY,
            //               ACCEPTED_DATE = data.ACCEPTED_DATE,

            //           }).FirstOrDefault();

            // Step 3: Map everything into the final ViewModel
            result = new AlaCarteHeaderViewModel
            {
                ALACARTEFOODHDR_ID = header.ALACARTEFOODHDR_ID,
                ORDER_DATE = header.ORDER_DATE,
                ORDER_STATUS = header.ORDER_STATUS,
                TrnModel = trnList,
                ADDEDBY = header.ADDEDBY,
                ADDEDDATE = header.ADDEDDATE,
                LSTMODDATE = header.LSTMODDATE,
                LSTMODIFIEDBY = header.LSTMODIFIEDBY,
                ACCEPTED_DATE = header.ACCEPTED_DATE,
                Emp_Detail = new Employee_Details
                {
                    _ECode = header._AddBy.ADEMPCODE,
                    _EName = header._AddBy.FIRSTNAME + " " + header._AddBy.LASTNAME,
                    _EmailId = header._AddBy.EMAILID,
                    _DOB = DateTime.Now,
                    _SecDescrip = header._VWAssociate.SECTION,
                    _DepDesc = header._VWAssociate.DEPARTMENT,
                    _DivDesc = header._VWAssociate.DIVISION,
                    _OpDesc = header._VWAssociate.OPERATION,
                    _SiteId = header._VWAssociate.SYSITEID
                }
            };



            return result;
        }

        public short CancelAlaCarteOrder(long id, long updatedBy)
        {
            short retVal = 0;
            try
            {
                BC_ALACARTEFOOD_HDR_DGIT model = new BC_ALACARTEFOOD_HDR_DGIT();
                model = _dbContext.BC_ALACARTEFOOD_HDR.Where(w => w.ALACARTEFOODHDR_ID == id).FirstOrDefault();
                if (model != null)
                {
                    model.ORDER_STATUS = 3;
                    model.LSTMODIFIEDBY = updatedBy;
                    model.LSTMODDATE = DateTime.Now;
                    _dbContext.Entry(model).State = EntityState.Modified;
                    _dbContext.SaveChanges();

                    retVal = 1;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }

        // -- Family Meal Booking -- //
        public FamilMealBookingDtlViewModel GetMealByDate(long mealtype, DateTime bookingDate)
        {
            FamilMealBookingDtlViewModel Meal_obj = (from data in _dbContext.BC_MEALS_AVAILABILITY
                                                     join _MealMst in _dbContext.BC_MST_MEALS on data.MEALS_ID equals _MealMst.BC_MEALID
                                                     where _MealMst.MEALTYPEID == mealtype && data.MEAL_MAPPING_DATE == bookingDate
                                                     select new FamilMealBookingDtlViewModel
                                                     {
                                                         MEALS_AVAILABILITY_ID = data.BC_MEALS_AVAILABILITY_ID,
                                                         MealMst = new BCMstMealViewModel
                                                         {
                                                             MEALTYPEID = _MealMst.MEALTYPEID,
                                                             MEAL_NAME = _MealMst.MEAL_NAME,
                                                             MEAL_PRICE = _MealMst.MEAL_PRICE,
                                                             MEAL_DESC = _MealMst.MEAL_DESC,
                                                             MEAL_PHOTO = _MealMst.MEAL_PHOTO,
                                                             MEAL_PHOTO_NAME = _MealMst.MEAL_PHOTO_NAME,
                                                         }
                                                     }).FirstOrDefault();
            return Meal_obj;
        }
        //Added by Bhupesh - NTT
        #region Calculate Financial Year
        public DateTime GetCurrentFinacialstartDate(DateTime todayDate)
        {
            if (todayDate.Month <= 3)
            {
                DateTime startDate = new DateTime(todayDate.Year - 1, 4, 1);
                return startDate;
            }
            else
            {
                DateTime startDate = new DateTime(todayDate.Year, 4, 1);
                return startDate;
            }

        }
        public DateTime GetCurrentFinacialendDate(DateTime todayDate)
        {
            if (todayDate.Month <= 3)
            {
                DateTime endDate = new DateTime(todayDate.Year, 3, 31);
                return endDate;
            }
            else
            {
                DateTime endDate = new DateTime(todayDate.Year + 1, 3, 31);
                return endDate;
            }

        }
        #endregion
        public Tuple<short, string> SaveFamilyMealBooking(FamilyMealBookingTrnViewModel model)
        {
            short retVal = 0;
            string msg = "";
            Tuple<short, string> _tuple = new Tuple<short, string>(retVal, msg);
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    //sa CR7220
                    DateTime bookingDate = DateTime.Now.Date;

                    if (!string.IsNullOrEmpty(model.strBOOKED_DATE))
                    {
                        bookingDate = DateTime.ParseExact(model.strBOOKED_DATE, "dd-MMM-yyyy", null);
                    }
                    DateTime startDate = new DateTime(2025, 10, 1);
                    DateTime endDate = new DateTime(2025, 10, 31);
                    if (bookingDate.Date >= startDate.Date && bookingDate.Date <= endDate.Date)
                    {


                        int Flag = GetFamilyVisitValidation(bookingDate, model.ADDEDBY);
                        if (Flag == 0)
                        {
                            retVal = 9;
                            msg = "";
                            return _tuple = new Tuple<short, string>(retVal, msg);
                        }
                    }
                    else
                    {
                        retVal = 9;
                        msg = "";
                        transaction.Rollback();
                        return _tuple = new Tuple<short, string>(retVal, msg);
                    }
                    //ea CR7220
                    BC_FAMILYMEALBOOKING_TRN_DGIT GMB = new BC_FAMILYMEALBOOKING_TRN_DGIT();
                    BC_VALIDATION_ViewModel ValidationMst = GetValidationData();

                    #region Check Validation
                    //sa CR7220
                    //DateTime bookingDate = DateTime.Now.Date; 

                    //if (!string.IsNullOrEmpty(model.strBOOKED_DATE))
                    //{
                    //    bookingDate = DateTime.ParseExact(model.strBOOKED_DATE, "dd-MMM-yyyy", null);
                    //}
                    //ea CR7220
                    DateTime financialStartDate = GetCurrentFinacialstartDate(bookingDate); //Added by Bhupesh - NTT
                    DateTime financialEndDate = GetCurrentFinacialendDate(bookingDate); //Added by Bhupesh - NTT
                    //Added by Bhupesh - NTT

                    var checkStatus = _dbContext.BC_FAMILYMEALBOOKING_TRN.Count(x => x.ADDEDBY == model.ADDEDBY && x.MEAL_STATUS == 1 &&
                    (
                    //sa CR7220
                    //x.ADDEDDATE >= financialStartDate && x.ADDEDDATE <= financialEndDate

                    //&& x.MEAL_BOOKED_DATE.Day  == bookingDate.Day
                    //&& x.MEAL_BOOKED_DATE.Month == bookingDate.Month
                    //&& x.MEAL_BOOKED_DATE.Year == bookingDate.Year

                    (x.MEAL_BOOKED_DATE.Day >= startDate.Day
                    && x.MEAL_BOOKED_DATE.Month >= startDate.Month
                    && x.MEAL_BOOKED_DATE.Year >= startDate.Year)

                    && (x.MEAL_BOOKED_DATE.Day <= endDate.Day
                    && x.MEAL_BOOKED_DATE.Month <= endDate.Month
                    && x.MEAL_BOOKED_DATE.Year <= endDate.Year)

                    //ea CR7220
                    )
                    ) > 0;

                    if (checkStatus)
                    {
                        retVal = 2;
                        msg = model.strBOOKED_DATE;
                        transaction.Rollback();
                        return _tuple = new Tuple<short, string>(retVal, msg); //// -- record already exist.
                    }
                    if (ValidationMst != null)
                    {
                        if (string.IsNullOrEmpty(ValidationMst.MEAL_BOOKING_DURATION) || string.IsNullOrEmpty(ValidationMst.FMY_MEAL_BOOKING_TIME))
                        {
                            retVal = 5; //// -- Validation master not maintained.
                        }
                        else
                        {
                            DateTime currentDate = DateTime.Now.Date;
                            int bookingDuration = Convert.ToInt32(ValidationMst.MEAL_BOOKING_DURATION);
                            DateTime ValidDuration = currentDate.AddDays(bookingDuration);
                            if (bookingDate > ValidDuration)
                            {
                                retVal = 4;
                                msg = ValidDuration.ToString("dd-MMM-yyyy");
                                return _tuple = new Tuple<short, string>(retVal, msg); //// -- Book before n-1 day.
                            }

                            currentDate = DateTime.Now;
                            string bookingTime = ValidationMst.FMY_MEAL_BOOKING_TIME;
                            int validDay = Convert.ToInt32(ValidationMst.FMY_MEAL_BOOKING_DAY);
                            DateTime ValidDateTime = DateTime.ParseExact(model.strBOOKED_DATE + " " + bookingTime, "dd-MMM-yyyy HH:mm:ss", null).AddDays(-validDay);
                            if (currentDate > ValidDateTime)
                            {
                                retVal = 3;
                                msg = model.strBOOKED_DATE + " after " + ValidDateTime.ToString("HH:mm") + " hrs";
                                return _tuple = new Tuple<short, string>(retVal, msg); //// -- Book before n-1 day.
                            }
                        }
                    }
                    if (retVal == 5 || ValidationMst == null)
                    {
                        retVal = 5;
                        msg = "";
                        return _tuple = new Tuple<short, string>(retVal, msg); //// -- Validation master not maintained.
                    }
                    #endregion

                    #region Check QTY
                    //// Check Member Qty in Request 
                    int memberCount = model.MEALBOOKING_DTL.Count();
                    if (ValidationMst.FMY_VISIT_MAX_MEMBER_ALLOW > 0)
                    {
                        if (memberCount > Convert.ToInt32(ValidationMst.FMY_VISIT_MAX_MEMBER_ALLOW))
                        {
                            retVal = 8;
                            msg = ValidationMst.FMY_VISIT_MAX_MEMBER_ALLOW.ToString();
                            transaction.Rollback();
                            return _tuple = new Tuple<short, string>(retVal, msg);
                        }
                    }

                    //// Check Member Qty
                    int bookedCount = 0;
                    bookedCount = (from _dtl in _dbContext.BC_FAMILYMEALBOOKING_DTL.Where(w => w.STATUS == 1)
                                   join _trn in _dbContext.BC_FAMILYMEALBOOKING_TRN.Where(x => x.MEAL_BOOKED_DATE == bookingDate && x.MEAL_STATUS == 1) on _dtl.BC_FAMILY_BOOKINGID equals _trn.BC_FAMILY_BOOKINGID
                                   select _dtl).Count();
                    if (bookedCount >= Convert.ToInt32(ValidationMst.FMY_VISIT_MEMBER_QTY))
                    {
                        retVal = 6;
                        msg = bookingDate.ToString("dd-MMM-yyyy");
                        transaction.Rollback();
                        return _tuple = new Tuple<short, string>(retVal, msg); //// -- visit request booking has exceeded the maximum limit.
                    }
                    //// End ////

                    //// Check Slot Qty
                    bookedCount = 0;
                    bookedCount = _dbContext.BC_FAMILYMEALBOOKING_TRN.Count(x => x.MEAL_BOOKED_DATE == bookingDate && x.MEAL_STATUS == 1);
                    int slotCapcity = 0;
                    BC_MEALSLOTMST_DGIT slotMst = (from data in _dbContext.BC_MEALSLOTMST
                                                   where data.BCSLOTMSTID == model.SLOTID
                                                   select data).FirstOrDefault();
                    if (slotMst != null)
                    {
                        slotCapcity = slotMst.CAPACITY;
                    }
                    if (bookedCount >= slotCapcity)
                    {
                        retVal = 7;
                        msg = "Slot " + slotMst.SLOT_TIME + " has exceeded the maximum limit for " + bookingDate.ToString("dd-MMM-yyyy");
                        transaction.Rollback();
                        return _tuple = new Tuple<short, string>(retVal, msg); //// -- Slot has exceeded the maximum limit.
                    }
                    //// End ////
                    #endregion

                    if (_dbContext.BC_FAMILYMEALBOOKING_TRN.Count() == 0)
                    {
                        GMB.BC_FAMILY_BOOKINGID = 1;
                    }
                    else
                    {
                        GMB.BC_FAMILY_BOOKINGID = _dbContext.BC_FAMILYMEALBOOKING_TRN.Max(x => x.BC_FAMILY_BOOKINGID) + 1;
                    }

                    ////
                    //Random generator = new Random();
                    //string tokenCode = generator.Next(0, 1000000).ToString("D6");
                    ////

                    GMB.MEAL_BOOKED_DATE = bookingDate;
                    GMB.MEAL_STATUS = 1;
                    GMB.SLOTID = model.SLOTID;
                    //GMB.TOKEN_CODE = tokenCode;
                    GMB.ADDEDBY = model.ADDEDBY;
                    GMB.ADDEDDATE = DateTime.Now;
                    _dbContext.Entry(GMB).State = EntityState.Added;
                    _dbContext.SaveChanges();

                    model.BC_FAMILY_BOOKINGID = GMB.BC_FAMILY_BOOKINGID;

                    //// -- Save Family Detail --////
                    short _val = SaveFamilyMealBookingDtl(model, model.MEALBOOKING_DTL);
                    if (_val == 1)
                    {
                        retVal = 1;
                        msg = "";
                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
            }
            _tuple = new Tuple<short, string>(retVal, msg);
            return _tuple;
        }

        public short SaveFamilyMealBookingDtl(FamilyMealBookingTrnViewModel model, List<FamilMealBookingDtlViewModel> familyList)
        {
            short retVal = 0;
            if (model.BC_FAMILY_BOOKINGID > 0)
            {
                foreach (FamilMealBookingDtlViewModel Gdtl in familyList)
                {
                    #region INSERT/MODIFY GUEST DTL
                    int dtlFlag = 0;
                    BC_FAMILYMEALBOOKING_DTL_DGIT BGD = new BC_FAMILYMEALBOOKING_DTL_DGIT();
                    if (Gdtl.BC_FAMILYMEALBOOKING_DTLID > 0)
                    {
                        BGD = _dbContext.BC_FAMILYMEALBOOKING_DTL.Where(g => g.BC_FAMILYMEALBOOKING_DTLID == Gdtl.BC_FAMILYMEALBOOKING_DTLID).FirstOrDefault();
                    }
                    else
                    {
                        BGD = new BC_FAMILYMEALBOOKING_DTL_DGIT();
                        if (_dbContext.BC_FAMILYMEALBOOKING_DTL.Count() == 0)
                        {
                            BGD.BC_FAMILYMEALBOOKING_DTLID = 1;
                        }
                        else
                        {
                            BGD.BC_FAMILYMEALBOOKING_DTLID = _dbContext.BC_FAMILYMEALBOOKING_DTL.Max(x => x.BC_FAMILYMEALBOOKING_DTLID) + 1;
                        }
                        dtlFlag = 1;
                    }
                    BGD.BC_FAMILY_BOOKINGID = model.BC_FAMILY_BOOKINGID;
                    //BGD.MEALS_AVAILABILITY_ID = Gdtl.MEALS_AVAILABILITY_ID;
                    //BGD.MEALTYPEID = Gdtl.MEALTYPEID;
                    BGD.MEMBER_NAME = Gdtl.MEMBER_NAME;
                    BGD.RELATION_TYPE = Gdtl.RELATION_TYPE;
                    BGD.STATUS = 1;
                    if (dtlFlag == 1)
                    {
                        BGD.ADDEDBY = model.ADDEDBY;
                        BGD.ADDEDDATE = DateTime.Now;
                    }
                    else
                    {
                        BGD.LSTMODIFIEDBY = model.ADDEDBY;
                        BGD.LSTMODDATE = DateTime.Now;
                    }
                    _dbContext.Entry(BGD).State = dtlFlag == 1 ? EntityState.Added : EntityState.Modified;
                    _dbContext.SaveChanges();
                    #endregion
                }
                retVal = 1;
            }
            return retVal;
        }

        public List<FamilyMealBookingTrnViewModel> GetFamilyBookedMealList(long mealType, string fromDate, string toDate, long loginUser)
        {
            DateTime currentDate = DateTime.Now;
            string cancellationTime = "23:59:00";
            int fmy_MealDay = 0;
            BC_VALIDATION_ViewModel ValidationMst = GetValidationData();
            if (ValidationMst != null)
            {
                if (!string.IsNullOrEmpty(ValidationMst.FMY_MEAL_CANCELLATION_TIME))
                {
                    cancellationTime = ValidationMst.FMY_MEAL_CANCELLATION_TIME;
                }

                fmy_MealDay = !string.IsNullOrEmpty(ValidationMst.FMY_MEAL_BOOKING_DAY) ? Convert.ToInt32(ValidationMst.FMY_MEAL_BOOKING_DAY) : fmy_MealDay;

            }

            DateTime bookingFromDate = DateTime.Now.Date;
            DateTime bookingToDate = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(fromDate))
            {
                bookingFromDate = DateTime.ParseExact(fromDate, "dd-MMM-yyyy", null);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                bookingToDate = DateTime.ParseExact(toDate + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
            }
            var iList = (from data in _dbContext.BC_FAMILYMEALBOOKING_TRN.Where(w => w.ADDEDBY == loginUser)
                         join _slotMst in _dbContext.BC_MEALSLOTMST on data.SLOTID equals _slotMst.BCSLOTMSTID
                         where (!string.IsNullOrEmpty(fromDate) ? (data.MEAL_BOOKED_DATE >= bookingFromDate) : true)
                                                          && (!string.IsNullOrEmpty(toDate) ? (data.MEAL_BOOKED_DATE <= bookingToDate) : true)
                         //&& (mealType == 0 ? true : _fmDtl.MEALTYPEID == mealType)
                         select new FamilyMealBookingTrnViewModel
                         {
                             BC_FAMILY_BOOKINGID = data.BC_FAMILY_BOOKINGID,
                             MEAL_BOOKED_DATE = data.MEAL_BOOKED_DATE,
                             MEAL_STATUS = data.MEAL_STATUS,
                             MEALBOOKING_DTL = (from _fmDtl in _dbContext.BC_FAMILYMEALBOOKING_DTL.Where(d => d.BC_FAMILY_BOOKINGID == data.BC_FAMILY_BOOKINGID)
                                                    //join _mealType in _dbContext.BC_MST_MEAL_TYPE on _fmDtl.MEALTYPEID equals _mealType.BC_MEALTYPEID
                                                    //join _mealAvail in _dbContext.BC_MEALS_AVAILABILITY on _fmDtl.MEALS_AVAILABILITY_ID equals _mealAvail.BC_MEALS_AVAILABILITY_ID
                                                    //join _mealMst in _dbContext.BC_MST_MEALS on _mealAvail.MEALS_ID equals _mealMst.BC_MEALID
                                                select new FamilMealBookingDtlViewModel
                                                {
                                                    BC_FAMILYMEALBOOKING_DTLID = _fmDtl.BC_FAMILYMEALBOOKING_DTLID,
                                                    BC_FAMILY_BOOKINGID = _fmDtl.BC_FAMILY_BOOKINGID,
                                                    MEMBER_NAME = _fmDtl.MEMBER_NAME,
                                                    RELATION_TYPE = _fmDtl.RELATION_TYPE,
                                                    //MEALTYPEID = _fmDtl.MEALTYPEID,
                                                    //MEALS_AVAILABILITY_ID = _fmDtl.MEALS_AVAILABILITY_ID
                                                }).ToList(),
                             ADDEDBY = data.ADDEDBY,
                             ADDEDDATE = data.ADDEDDATE,
                             SLOTID = data.SLOTID,
                             SLOT_TIME = _slotMst.SLOT_TIME,
                             TOKEN_CODE = data.TOKEN_CODE
                         }).ToList();
            List<FamilyMealBookingTrnViewModel> newList = new List<FamilyMealBookingTrnViewModel>();
            foreach (var obj in iList)
            {
                DateTime validDate = DateTime.ParseExact(obj.MEAL_BOOKED_DATE.ToString("dd-MMM-yyyy") + " " + cancellationTime, "dd-MMM-yyyy HH:mm:ss", null).AddDays(-fmy_MealDay);
                obj.IsEnableCancel = currentDate < validDate ? true : false;
                newList.Add(obj);
            }
            return newList.OrderByDescending(o => o.MEAL_BOOKED_DATE).ToList();
        }
        public short CancelFamilyMealBooking(long id, long updatedBy)
        {
            short retVal = 0;
            try
            {
                BC_FAMILYMEALBOOKING_TRN_DGIT model = new BC_FAMILYMEALBOOKING_TRN_DGIT();
                model = _dbContext.BC_FAMILYMEALBOOKING_TRN.Where(w => w.BC_FAMILY_BOOKINGID == id).FirstOrDefault();
                if (model != null)
                {
                    model.MEAL_STATUS = 2;
                    model.LSTMODIFIEDBY = updatedBy;
                    model.LSTMODDATE = DateTime.Now;
                    _dbContext.Entry(model).State = EntityState.Modified;
                    _dbContext.SaveChanges();

                    retVal = 1;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }
        public List<MealSlotViewModel> GetMealSlotsByBookingDate(string bDate)
        {
            bool IsOffDay = false; long _siteId = 0;
            BC_VALIDATION_ViewModel ValidationMst = GetValidationData();
            if (ValidationMst != null)
            {
                _siteId = ValidationMst.FMY_VISIT_FOLLOW_SITE_CALENDER;
            }
            List<MealSlotViewModel> newList = new List<MealSlotViewModel>();
            if (!string.IsNullOrEmpty(bDate))
            {
                DateTime _date = DateTime.ParseExact(bDate, "dd-MMM-yyyy", null);
                //IsOffDay = _dbContext.HMSIHOLIDAYS.Any(h => h.MONTHDATEYEAR == _date && h.ACTIVE == 1 && h.SYSITEID == _siteId);
                //newList = (from data in _dbContext.BC_MEALSLOTMST
                //           where data.STATUS == 1 && data.MEALTYPEID == 3
                //           && (IsOffDay ? data.IS_OFFDAY_SLOT == 1 : data.IS_OFFDAY_SLOT == 0)
                //           select new MealSlotViewModel
                //           {
                //               BCSLOTMSTID = data.BCSLOTMSTID,
                //               SLOT_DESC = data.SLOT_DESC,
                //               SLOT_TIME = data.SLOT_TIME,
                //               MEALTYPEID = data.MEALTYPEID,
                //               STATUS = data.STATUS,
                //               ADDEDDATE = data.ADDEDDATE,
                //               ADDEDBY = data.ADDEDBY
                //           }).ToList();
                newList = (from data in _dbContext.BC_MEALSLOTMST
                           join _SlotMap in _dbContext.BC_FMLY_SLOT_MAP on data.BCSLOTMSTID equals _SlotMap.SLOT_ID
                           where data.STATUS == 1 && data.MEALTYPEID == 3
                           && _SlotMap.ALLOW_DATE == _date
                           && _SlotMap.STATUS == 1
                           select new MealSlotViewModel
                           {
                               BCSLOTMSTID = data.BCSLOTMSTID,
                               SLOT_DESC = data.SLOT_DESC,
                               SLOT_TIME = data.SLOT_TIME,
                               MEALTYPEID = data.MEALTYPEID,
                               STATUS = data.STATUS,
                               ADDEDDATE = data.ADDEDDATE,
                               ADDEDBY = data.ADDEDBY
                           }).ToList();
            }
            return newList;
        }

        public FamilyMealBookingTrnViewModel GetFamilyRequestDtlById(long id)
        {
            //var obj = (from data in _dbContext.BC_FAMILYMEALBOOKING_TRN.Where(w => w.BC_FAMILY_BOOKINGID == id)
            //           join _slotMst in _dbContext.BC_MEALSLOTMST on data.SLOTID equals _slotMst.BCSLOTMSTID
            //           join _AddBy in _dbContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
            //           join _VWAssociate in _dbContext.VW_ASSOCIATELVLDETAILS.Where(v => v.SYKI == _Syki.SYKIID) on data.ADDEDBY equals _VWAssociate.ADEMPCODE
            //           select new FamilyMealBookingTrnViewModel
            //           {
            //               BC_FAMILY_BOOKINGID = data.BC_FAMILY_BOOKINGID,
            //               MEAL_BOOKED_DATE = data.MEAL_BOOKED_DATE,
            //               MEAL_STATUS = data.MEAL_STATUS,
            //               MEALBOOKING_DTL = (from _fmDtl in _dbContext.BC_FAMILYMEALBOOKING_DTL.Where(d => d.BC_FAMILY_BOOKINGID == data.BC_FAMILY_BOOKINGID)
            //                                      //join _mealType in _dbContext.BC_MST_MEAL_TYPE on _fmDtl.MEALTYPEID equals _mealType.BC_MEALTYPEID
            //                                      //join _mealAvail in _dbContext.BC_MEALS_AVAILABILITY on _fmDtl.MEALS_AVAILABILITY_ID equals _mealAvail.BC_MEALS_AVAILABILITY_ID into _JoinMeatAvl
            //                                      //from MeatAvl in _JoinMeatAvl.DefaultIfEmpty()
            //                                      //join _mealMst in _dbContext.BC_MST_MEALS on MeatAvl.MEALS_ID equals _mealMst.BC_MEALID into _JoinMealMst
            //                                      //from MealMst2 in _JoinMealMst.DefaultIfEmpty()
            //                                  select new FamilMealBookingDtlViewModel
            //                                  {
            //                                      BC_FAMILYMEALBOOKING_DTLID = _fmDtl.BC_FAMILYMEALBOOKING_DTLID,
            //                                      BC_FAMILY_BOOKINGID = _fmDtl.BC_FAMILY_BOOKINGID,
            //                                      MEMBER_NAME = _fmDtl.MEMBER_NAME,
            //                                      RELATION_TYPE = _fmDtl.RELATION_TYPE,
            //                                      //MEALTYPEID = _fmDtl.MEALTYPEID,
            //                                      //MEALTYPE = _mealType.MEAL_TYPE_DESC,
            //                                      //MEALS_AVAILABILITY_ID = _fmDtl.MEALS_AVAILABILITY_ID,
            //                                      //MealMst = new BCMstMealViewModel
            //                                      //{
            //                                      //    MEAL_DESC = MealMst2 == null ? "" : MealMst2.MEAL_DESC,
            //                                      //    MEAL_NAME = MealMst2 == null ? "" : MealMst2.MEAL_NAME,
            //                                      //    MEAL_PRICE = MealMst2 == null ? 0 : MealMst2.MEAL_PRICE
            //                                      //}
            //                                  }).ToList(),
            //               Emp_Detail = new Employee_Details
            //               {
            //                   _ECode = _AddBy.ADEMPCODE,
            //                   _EName = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
            //                   _EmailId = _AddBy.EMAILID,
            //                   _DOB = DateTime.Now,
            //                   _SecDescrip = _VWAssociate.SECTION,
            //                   _DepDesc = _VWAssociate.DEPARTMENT,
            //                   _DivDesc = _VWAssociate.DIVISION,
            //                   _OpDesc = _VWAssociate.OPERATION,
            //                   _SiteId = _VWAssociate.SYSITEID
            //               },
            //               ADDEDBY = data.ADDEDBY,
            //               ADDEDDATE = data.ADDEDDATE,
            //               SLOTID = data.SLOTID,
            //               SLOT_TIME = _slotMst.SLOT_TIME,
            //               TOKEN_CODE = data.TOKEN_CODE
            //           }).FirstOrDefault();
            //return obj;

            var mainData = (from data in _dbContext.BC_FAMILYMEALBOOKING_TRN
                            join _slotMst in _dbContext.BC_MEALSLOTMST on data.SLOTID equals _slotMst.BCSLOTMSTID
                            join _AddBy in _dbContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                            join _VWAssociate in _dbContext.VW_ASSOCIATELVLDETAILS.Where(v => v.SYKI == _Syki.SYKIID) on data.ADDEDBY equals _VWAssociate.ADEMPCODE
                            where data.BC_FAMILY_BOOKINGID == id
                            select new FamilyMealBookingTrnViewModel
                            {
                                BC_FAMILY_BOOKINGID = data.BC_FAMILY_BOOKINGID,
                                MEAL_BOOKED_DATE = data.MEAL_BOOKED_DATE,
                                MEAL_STATUS = data.MEAL_STATUS,
                                ADDEDBY = data.ADDEDBY,
                                ADDEDDATE = data.ADDEDDATE,
                                SLOTID = data.SLOTID,
                                SLOT_TIME = _slotMst.SLOT_TIME,
                                TOKEN_CODE = data.TOKEN_CODE,
                                Emp_Detail = new Employee_Details
                                {
                                    _ECode = _AddBy.ADEMPCODE,
                                    _EName = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                                    _EmailId = _AddBy.EMAILID,
                                    _DOB = DateTime.Now,
                                    _SecDescrip = _VWAssociate.SECTION,
                                    _DepDesc = _VWAssociate.DEPARTMENT,
                                    _DivDesc = _VWAssociate.DIVISION,
                                    _OpDesc = _VWAssociate.OPERATION,
                                    _SiteId = _VWAssociate.SYSITEID
                                }
                            }).FirstOrDefault();


            if (mainData != null)
            {
                mainData.MEALBOOKING_DTL = _dbContext.BC_FAMILYMEALBOOKING_DTL
                    .Where(d => d.BC_FAMILY_BOOKINGID == mainData.BC_FAMILY_BOOKINGID)
                    .Select(_fmDtl => new FamilMealBookingDtlViewModel
                    {
                        BC_FAMILYMEALBOOKING_DTLID = _fmDtl.BC_FAMILYMEALBOOKING_DTLID,
                        BC_FAMILY_BOOKINGID = _fmDtl.BC_FAMILY_BOOKINGID,
                        MEMBER_NAME = _fmDtl.MEMBER_NAME,
                        RELATION_TYPE = _fmDtl.RELATION_TYPE
                    }).ToList();
            }

            return mainData;
        }

        //-----------Meeting Food ------------

        public List<MeetingFoodItemMstViewModel> GetMeetingFoodItemList()
        {
            List<MeetingFoodItemMstViewModel> items = new List<MeetingFoodItemMstViewModel>();

            items = (from data in _dbContext.BC_MEETINGFOOD_MST
                     where data.STATUS == 1
                     select new MeetingFoodItemMstViewModel
                     {
                         BC_FOODMSTID = data.BC_MEETINGFOODMSTID,
                         ITEM_CATEGORY = data.ITEM_CATEGORY,
                         ITEM_NAME = data.ITEM_NAME,
                         ITEM_PRICE = data.ITEM_PRICE,
                         STATUS = data.STATUS,
                         ITEM_PHOTO = data.ITEM_PHOTO,
                         ITEM_PHOTO_NAME = data.ITEM_PHOTO_NAME
                     }).ToList();

            return items;
        }

        public Tuple<short, string> SaveMeetingFoodOrder(long AddedBy, long OPID, string Eventdate, string Eventtime, string Venue, string Remarks, List<MeetingFoodTrnViewModel> modelList, string noofGuests, string desg)
        {
            short retVal = 0;
            string msg = "";
            Tuple<short, string> _tuple = new Tuple<short, string>(retVal, msg);
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    BC_MEETINGFOOD_HDR BAH = new BC_MEETINGFOOD_HDR();

                    if (_dbContext.BC_MEETINGFOOD_HDR.Count() == 0)
                    {
                        BAH.MEETINGFOODHDR_ID = 1;
                    }
                    else
                    {
                        BAH.MEETINGFOODHDR_ID = _dbContext.BC_MEETINGFOOD_HDR.Max(x => x.MEETINGFOODHDR_ID) + 1;
                    }
                    var _Approval = _dbContext.ADORGCOORDINATOR.Where(a => a.ADORGLEVELID == OPID && a.ISACTIVE == 1).Select(a => a.OPHEAD).First();

                    string ids = _db2Context.SYPARAMETERS.Where(x => x.PARAMNAME == "Tea_Coffee_Request").Select(x => x.PARAMVALUE).FirstOrDefault();
                    List<string> tempDesgs = ids.Split(',').ToList();
                    if (tempDesgs.Contains(desg))
                    {
                        _Approval = AddedBy;
                    }


                    BAH.ORDER_DATE = DateTime.Now;
                    BAH.ORDER_STATUS = 0;
                    BAH.ADDEDBY = AddedBy;
                    BAH.EVENTDATE = Convert.ToDateTime(Eventdate);
                    BAH.EVENTTIME = Eventtime;
                    BAH.VENUE = Venue;
                    BAH.REMARKS = Remarks;
                    BAH.ADDEDDATE = DateTime.Now;
                    BAH.OHAPPROVAL = _Approval;
                    BAH.OHSTATUS = 0;
                    BAH.ADMINSTATUS = 0;
                    BAH.NOOFGUESTS = noofGuests;
                    _dbContext.Entry(BAH).State = EntityState.Added;
                    _dbContext.SaveChanges();

                    short _retVal = SaveMeetingFoodTrn(AddedBy, BAH.MEETINGFOODHDR_ID, modelList);
                    if (_retVal == 1)
                    {
                        transaction.Commit();
                        retVal = 1;
                    }
                    else
                    {
                        retVal = 0;
                        transaction.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
            }
            _tuple = new Tuple<short, string>(retVal, msg);
            return _tuple;
        }

        public short SaveMeetingFoodTrn(long AddedBy, long HeaderId, List<MeetingFoodTrnViewModel> modelList)
        {
            short retVal = 0;
            foreach (MeetingFoodTrnViewModel model in modelList)
            {

                BC_MEETINGFOOD_TRN BAT = new BC_MEETINGFOOD_TRN();
                if (HeaderId > 0)
                {
                    if (_dbContext.BC_MEETINGFOOD_TRN.Count() == 0)
                    {
                        BAT.MEETINGFOODTRN_ID = 1;
                    }
                    else
                    {
                        BAT.MEETINGFOODTRN_ID = _dbContext.BC_MEETINGFOOD_TRN.Max(x => x.MEETINGFOODTRN_ID) + 1;
                    }

                    BAT.MEETINGFOODHDR_ID = HeaderId;
                    BAT.ITEMID = model.ITEMID;
                    BAT.ITEM_PRICE = model.ITEM_PRICE;
                    BAT.ITEM_QTY = model.ITEM_QTY;
                    BAT.STATUS = 1;
                    BAT.ADDEDBY = AddedBy;
                    BAT.ADDEDDATE = DateTime.Now;
                    _dbContext.Entry(BAT).State = EntityState.Added;
                    _dbContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }

        public List<MeetingFoodHeaderViewModel> GetMeetingFoodRequestList(string fromDate, string toDate, short category, long loginUser)
        {
            DateTime orderFromDate = DateTime.Now.Date;
            DateTime orderToDate = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(fromDate))
            {
                orderFromDate = DateTime.ParseExact(fromDate, "dd-MMM-yyyy", null);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                orderToDate = DateTime.ParseExact(toDate + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
            }

            var mailList = (from data1 in _dbContext.BC_MEETINGFOOD_HDR.Where(w => w.ADDEDBY == loginUser)
                            join _AddBy1 in _dbContext.ADEMPLOYEE on data1.ADDEDBY equals _AddBy1.ADEMPCODE
                            join _VWAssociate1 in _dbContext.VW_ASSOCIATELVLDETAILS on data1.ADDEDBY equals _VWAssociate1.ADEMPCODE
                            where (string.IsNullOrEmpty(fromDate) || (data1.ORDER_DATE >= orderFromDate))
                                  && (!string.IsNullOrEmpty(toDate) || (data1.ORDER_DATE <= orderToDate))
                                  && _VWAssociate1.SYKI == _Syki.SYKIID
                            select new
                            {
                                data = data1,
                                _AddBy = _AddBy1,
                                _VWAssociate = _VWAssociate1
                            }).ToList();

            var iList = mailList.Select(x => new MeetingFoodHeaderViewModel
            {
                MEETINGFOODFOODHDR_ID = x.data.MEETINGFOODHDR_ID,
                ORDER_DATE = x.data.ORDER_DATE,
                ORDER_STATUS = x.data.ORDER_STATUS,
                EVENT_DATE = x.data.EVENTDATE,
                VENUE = x.data.VENUE,
                TrnModel = (from _TrnData in _dbContext.BC_MEETINGFOOD_TRN.Where(d => d.MEETINGFOODHDR_ID == x.data.MEETINGFOODHDR_ID)
                            join _ItemMst in _dbContext.BC_MEETINGFOOD_MST on _TrnData.ITEMID equals _ItemMst.BC_MEETINGFOODMSTID
                            //where (category == 0 ? true : _ItemMst.ITEM_CATEGORY == category)
                            select new MeetingFoodTrnViewModel
                            {
                                MEETINGFOODFOODTRN_ID = _TrnData.MEETINGFOODTRN_ID,
                                MEETINGFOODFOODHDR_ID = _TrnData.MEETINGFOODHDR_ID,
                                ITEMID = _TrnData.ITEMID,
                                ItemModel = new MeetingFoodItemMstViewModel
                                {
                                    BC_FOODMSTID = _ItemMst.BC_MEETINGFOODMSTID,
                                    ITEM_NAME = _ItemMst.ITEM_NAME,
                                    ITEM_CATEGORY = _ItemMst.ITEM_CATEGORY,

                                },
                                TOTAL_PRICE = _TrnData.ITEM_QTY * _TrnData.ITEM_PRICE,
                                ITEM_PRICE =  _TrnData.ITEM_PRICE,
                                ITEM_QTY = _TrnData.ITEM_QTY

                            }).ToList(),
                Emp_Detail = new Employee_Details
                {
                    _ECode = x._AddBy.ADEMPCODE,
                    _EName = x._AddBy.FIRSTNAME + " " + x._AddBy.LASTNAME,
                    _EmailId = x._AddBy.EMAILID,
                    _DOB = DateTime.Now,
                    _SecDescrip = x._VWAssociate.SECTION,
                    _DepDesc = x._VWAssociate.DEPARTMENT,
                    _DivDesc = x._VWAssociate.DIVISION,
                    _OpDesc = x._VWAssociate.OPERATION,
                    _SiteId = x._VWAssociate.SYSITEID
                },
                ADDEDBY = x.data.ADDEDBY,
                ADDEDDATE = x.data.ADDEDDATE,
                LSTMODDATE = x.data.LSTMODDATE,
                LSTMODIFIEDBY = x.data.LSTMODIFIEDBY,
            }).OrderByDescending(o => o.ORDER_DATE).ToList();


            //var iList = (from data in _dbContext.BC_MEETINGFOOD_HDR.Where(w => w.ADDEDBY == loginUser)
            //             join _AddBy in _dbContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
            //             join _VWAssociate in _dbContext.VW_ASSOCIATELVLDETAILS on data.ADDEDBY equals _VWAssociate.ADEMPCODE
            //             where (!string.IsNullOrEmpty(fromDate) ? (data.ORDER_DATE >= orderFromDate) : true)
            //                   && (!string.IsNullOrEmpty(toDate) ? (data.ORDER_DATE <= orderToDate) : true)
            //                   && _VWAssociate.SYKI == _Syki.SYKIID
            //             select new MeetingFoodHeaderViewModel
            //             {
            //                 MEETINGFOODFOODHDR_ID = data.MEETINGFOODHDR_ID,
            //                 ORDER_DATE = data.ORDER_DATE,
            //                 ORDER_STATUS = data.ORDER_STATUS,
            //                 EVENT_DATE = data.EVENTDATE,
            //                 VENUE = data.VENUE,
            //                 TrnModel = (from _TrnData in _dbContext.BC_MEETINGFOOD_TRN.Where(d => d.MEETINGFOODHDR_ID == data.MEETINGFOODHDR_ID)
            //                             join _ItemMst in _dbContext.BC_MEETINGFOOD_MST on _TrnData.ITEMID equals _ItemMst.BC_MEETINGFOODMSTID
            //                             //where (category == 0 ? true : _ItemMst.ITEM_CATEGORY == category)
            //                             select new MeetingFoodTrnViewModel
            //                             {
            //                                 MEETINGFOODFOODTRN_ID = _TrnData.MEETINGFOODTRN_ID,
            //                                 MEETINGFOODFOODHDR_ID = _TrnData.MEETINGFOODHDR_ID,
            //                                 ITEMID = _TrnData.ITEMID,
            //                                 ItemModel = new MeetingFoodItemMstViewModel
            //                                 {
            //                                     BC_FOODMSTID = _ItemMst.BC_MEETINGFOODMSTID,
            //                                     ITEM_NAME = _ItemMst.ITEM_NAME,
            //                                     ITEM_CATEGORY = _ItemMst.ITEM_CATEGORY,

            //                                 },
            //                                 ITEM_PRICE = _TrnData.ITEM_PRICE,
            //                                 ITEM_QTY = _TrnData.ITEM_QTY

            //                             }).ToList(),
            //                 Emp_Detail = new Employee_Details
            //                 {
            //                     _ECode = _AddBy.ADEMPCODE,
            //                     _EName = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
            //                     _EmailId = _AddBy.EMAILID,
            //                     _DOB = DateTime.Now,
            //                     _SecDescrip = _VWAssociate.SECTION,
            //                     _DepDesc = _VWAssociate.DEPARTMENT,
            //                     _DivDesc = _VWAssociate.DIVISION,
            //                     _OpDesc = _VWAssociate.OPERATION,
            //                     _SiteId = _VWAssociate.SYSITEID
            //                 },
            //                 ADDEDBY = data.ADDEDBY,
            //                 ADDEDDATE = data.ADDEDDATE,
            //                 LSTMODDATE = data.LSTMODDATE,
            //                 LSTMODIFIEDBY = data.LSTMODIFIEDBY,
            //             }).ToList().OrderByDescending(o => o.ORDER_DATE).ToList();


            return iList;
        }
        //public MeetingFoodHeaderViewModel GetMeetingFoodDtlById(long id)
        //{

        //    var obj = (from data in _dbContext.BC_MEETINGFOOD_HDR.Where(w => w.MEETINGFOODHDR_ID == id)
        //               join _ApproverAuth in _dbContext.ADEMPLOYEE on data.OHAPPROVAL equals _ApproverAuth.ADEMPCODE into _ApproverAuthJoin
        //               from _Approver in _ApproverAuthJoin.DefaultIfEmpty()
        //               join _AddBy in _dbContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
        //               join _VWAssociate in _dbContext.VW_ASSOCIATELVLDETAILS on data.ADDEDBY equals _VWAssociate.ADEMPCODE
        //               where _VWAssociate.SYKI == _Syki.SYKIID
        //               select new MeetingFoodHeaderViewModel
        //               {
        //                   MEETINGFOODFOODHDR_ID = data.MEETINGFOODHDR_ID,

        //                   TrnModel = (from _TrnData in _dbContext.BC_MEETINGFOOD_TRN.Where(d => d.MEETINGFOODHDR_ID == data.MEETINGFOODHDR_ID)
        //                               join _ItemMst in _dbContext.BC_MEETINGFOOD_MST on _TrnData.ITEMID equals _ItemMst.BC_MEETINGFOODMSTID
        //                               //where (category == 0 ? true : _ItemMst.ITEM_CATEGORY == category)
        //                               select new MeetingFoodTrnViewModel
        //                               {
        //                                   MEETINGFOODFOODTRN_ID = _TrnData.MEETINGFOODTRN_ID,
        //                                   MEETINGFOODFOODHDR_ID = _TrnData.MEETINGFOODHDR_ID,
        //                                   ITEMID = _TrnData.ITEMID,
        //                                   ItemModel = new MeetingFoodItemMstViewModel
        //                                   {
        //                                       BC_FOODMSTID = _ItemMst.BC_MEETINGFOODMSTID,
        //                                       ITEM_NAME = _ItemMst.ITEM_NAME,
        //                                       ITEM_CATEGORY = _ItemMst.ITEM_CATEGORY,

        //                                   },
        //                                   ITEM_PRICE = _TrnData.ITEM_PRICE,
        //                                   ITEM_QTY = _TrnData.ITEM_QTY

        //                               }).ToList(),
        //                   OHAPPROVAL = data.OHAPPROVAL,
        //                   OHSTATUS = data.OHSTATUS,
        //                   VENUE = data.VENUE,
        //                   EVENT_DATE = data.EVENTDATE,
        //                   REMARKS = data.REMARKS,
        //                   ORDER_DATE = data.ORDER_DATE,
        //                   ORDER_STATUS = data.ORDER_STATUS,
        //                   Emp_Detail = new Employee_Details
        //                   {
        //                       _ECode = _AddBy.ADEMPCODE,
        //                       _EName = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
        //                       _EmailId = _AddBy.EMAILID,
        //                       _DOB = DateTime.Now,
        //                       _SecDescrip = _VWAssociate.SECTION,
        //                       _DepDesc = _VWAssociate.DEPARTMENT,
        //                       _DivDesc = _VWAssociate.DIVISION,
        //                       _OpDesc = _VWAssociate.OPERATION,
        //                       _SiteId = _VWAssociate.SYSITEID
        //                   }
        //               }).FirstOrDefault();
        //    return obj;
        //}

        public MeetingFoodHeaderViewModel GetMeetingFoodDtlById(long id)
        {
            var headerData = (from data in _dbContext.BC_MEETINGFOOD_HDR
                              where data.MEETINGFOODHDR_ID == id
                              join _AddBy in _dbContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                              join _VWAssociate in _dbContext.VW_ASSOCIATELVLDETAILS on data.ADDEDBY equals _VWAssociate.ADEMPCODE
                              join _ApproverAuth in _dbContext.ADEMPLOYEE on data.OHAPPROVAL equals _ApproverAuth.ADEMPCODE into approverJoin
                              from _Approver in approverJoin.DefaultIfEmpty()
                              where _VWAssociate.SYKI == _Syki.SYKIID
                              select new
                              {
                                  Header = data,
                                  AddedBy = _AddBy,
                                  VWAssociate = _VWAssociate,
                              }).FirstOrDefault();

            if (headerData == null) return null;

            var trnList = (from trn in _dbContext.BC_MEETINGFOOD_TRN
                           where trn.MEETINGFOODHDR_ID == id
                           join item in _dbContext.BC_MEETINGFOOD_MST on trn.ITEMID equals item.BC_MEETINGFOODMSTID
                           select new MeetingFoodTrnViewModel
                           {
                               MEETINGFOODFOODTRN_ID = trn.MEETINGFOODTRN_ID,
                               MEETINGFOODFOODHDR_ID = trn.MEETINGFOODHDR_ID,
                               ITEMID = trn.ITEMID,
                               ItemModel = new MeetingFoodItemMstViewModel
                               {
                                   BC_FOODMSTID = item.BC_MEETINGFOODMSTID,
                                   ITEM_NAME = item.ITEM_NAME,
                                   ITEM_CATEGORY = item.ITEM_CATEGORY
                               },
                               ITEM_PRICE = trn.ITEM_PRICE,
                               ITEM_QTY = trn.ITEM_QTY
                           }).ToList();

            var result = new MeetingFoodHeaderViewModel
            {
                MEETINGFOODFOODHDR_ID = headerData.Header.MEETINGFOODHDR_ID,
                TrnModel = trnList,
                OHAPPROVAL = headerData.Header.OHAPPROVAL,
                OHSTATUS = headerData.Header.OHSTATUS,
                VENUE = headerData.Header.VENUE,
                EVENT_DATE = headerData.Header.EVENTDATE,
                REMARKS = headerData.Header.REMARKS,
                ORDER_DATE = headerData.Header.ORDER_DATE,
                ORDER_STATUS = headerData.Header.ORDER_STATUS,
                Emp_Detail = new Employee_Details
                {
                    _ECode = headerData.AddedBy?.ADEMPCODE ?? 0, //if null then it returns 0
                    _EName = (headerData.AddedBy?.FIRSTNAME ?? "") + " " + (headerData.AddedBy?.LASTNAME ?? ""),
                    _EmailId = headerData.AddedBy?.EMAILID,
                    _DOB = DateTime.Now,
                    _SecDescrip = headerData.VWAssociate?.SECTION,
                    _DepDesc = headerData.VWAssociate?.DEPARTMENT,
                    _DivDesc = headerData.VWAssociate?.DIVISION,
                    _OpDesc = headerData.VWAssociate?.OPERATION,
                    _SiteId = headerData.VWAssociate?.SYSITEID
                }
            };

            return result;
        }



        public short MeetingFoodApproval(MeetingFoodHeaderViewModel GMBT)
        {
            short retVal = 0;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    BC_MEETINGFOOD_HDR _Approval = new BC_MEETINGFOOD_HDR();
                    if (GMBT.MEETINGFOODFOODHDR_ID > 0)
                    {
                        _Approval = _dbContext.BC_MEETINGFOOD_HDR.FirstOrDefault(a => a.MEETINGFOODHDR_ID == GMBT.MEETINGFOODFOODHDR_ID && a.OHSTATUS == 0);
                        if (_Approval != null)
                        {
                            short IsCommit = 0;
                            if (_Approval.OHAPPROVAL == GMBT.OHAPPROVAL)
                            {
                                _Approval.OHSTATUS = GMBT.OHSTATUS;
                                if (GMBT.OHSTATUS == 1) // Approved
                                {
                                    _Approval.ORDER_STATUS = 1;
                                }
                                else if (GMBT.OHSTATUS == 2) // Rejected
                                {
                                    _Approval.ORDER_STATUS = 4;
                                }
                                _Approval.OHAPPROVALDATE = DateTime.Now;
                                _Approval.OHREMARKS = GMBT.OHREMARKS;
                                _Approval.LSTMODDATE = DateTime.Now;
                                _Approval.LSTMODIFIEDBY = GMBT.LSTMODIFIEDBY;
                                IsCommit = 1;
                            }

                            if (IsCommit == 1)
                            {
                                _dbContext.Entry(_Approval).State = EntityState.Modified;
                                _dbContext.SaveChanges();
                                retVal = 1;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    retVal = -1;
                }

                if (retVal == 1)
                {
                    transaction.Commit();
                }
                else
                {
                    transaction.Rollback();
                }
            }
            return retVal;
        }

        public short CancelMeetingFoodOrder(long id, long updatedBy)
        {
            short retVal = 0;
            try
            {
                BC_MEETINGFOOD_HDR model = new BC_MEETINGFOOD_HDR();
                model = _dbContext.BC_MEETINGFOOD_HDR.Where(w => w.MEETINGFOODHDR_ID == id).FirstOrDefault();
                if (model != null)
                {
                    model.ORDER_STATUS = 3;
                    model.LSTMODIFIEDBY = updatedBy;
                    model.LSTMODDATE = DateTime.Now;
                    _dbContext.Entry(model).State = EntityState.Modified;
                    _dbContext.SaveChanges();

                    retVal = 1;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }

    }
}

