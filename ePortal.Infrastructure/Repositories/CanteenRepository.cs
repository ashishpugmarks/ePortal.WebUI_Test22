using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.Shared;
using ePortal.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ePortal.Infrastructure.Repositories
{
    public class CanteenRepository
    {
        private EPortalDGITDBContext _dbContext;

        private SYKI_DGIT _Syki;
        private List<CSD_MealTimeMapViewModel> TIME_DTL_LIST = new List<CSD_MealTimeMapViewModel>();
        private List<CSDValidationViewModel> _ValidationList = new List<CSDValidationViewModel>();
        private CommonRepository _CommonRepo;
        public CanteenRepository(EPortalDGITDBContext dbContext, CommonRepository commonRepo)
        {
            _dbContext = dbContext;
            _Syki = _dbContext.SYKI.Where(x => x.ACTIVE == 1).FirstOrDefault();
            TIME_DTL_LIST = GetMealTimeDtl(null, null);
            _ValidationList = GetValidationList();
            _CommonRepo = commonRepo;
        }

        public List<CSDMealTypeViewModel> GetMealTypeList(long plantId)
        {
            var mealTypeIds = (from data in _dbContext.CSD_MEALTYPE_MST
                               join _canteenMap in _dbContext.CSD_CANTEENMEALTYPE_MAP.Where(n => n.STATUS == 1) on data.CSD_MEALTYPEID equals _canteenMap.MEALTYPE_ID
                               join _canteen in _dbContext.CSD_CANTEEN_MST on _canteenMap.CANTEENID equals _canteen.CSD_CANTEENID
                               where data.STATUS == 1 && (plantId == 5 ? _canteen.PLANTID == 1 : _canteen.PLANTID == plantId)
                               select new { MealTypeId = data.CSD_MEALTYPEID }).Distinct().ToList();

            var iList = (from mealType in mealTypeIds
                         join data in _dbContext.CSD_MEALTYPE_MST on mealType.MealTypeId equals data.CSD_MEALTYPEID
                         select new CSDMealTypeViewModel
                         {
                             CSD_MEALTYPEID = data.CSD_MEALTYPEID,
                             MEAL_TYPE_DESC = data.MEAL_TYPE_DESC,
                             MEAL_QTY = data.MEAL_QTY,
                             STATUS = data.STATUS == 1 ? true : false,
                             ADDEDDATE = data.ADDEDDATE,
                             ADDEDBY = data.ADDEDBY,
                         }).OrderBy(o => o.CSD_MEALTYPEID).ToList();
            return iList.ToList();
        }
        public List<MealOptionMstViewModel> GetMealOption()
        {
            var iList = (from data in _dbContext.CSD_MEALOPTION_MST
                         where data.STATUS == 1
                         select new MealOptionMstViewModel
                         {
                             CSDMEAL_OPTIONID = data.CSDMEAL_OPTIONID,
                             MEAL_OPTION = data.MEAL_OPTION,
                             STATUS = data.STATUS,
                             ADDEDON = data.ADDEDON,
                             ADDEDBY = data.ADDEDBY,
                         }).ToList();
            return iList;
        }

        public List<CSDValidationViewModel> GetValidationList()
        {
            var iList = (from data in _dbContext.CSD_VALIDATION_MST
                         select new CSDValidationViewModel
                         {
                             CSD_VALIDMST_ID = data.CSD_VALIDMST_ID,
                             PARM_NAME = data.PARM_NAME,
                             PARM_VALUE = data.PARM_VALUE,
                             STATUS = data.STATUS,
                             DESCRIPTION = data.DESCRIPTION,
                             PARM_INPUT_TYPE = data.PARM_INPUT_TYPE,
                         }).ToList();
            return iList;
        }

        public string GetParmvalue(string PARM_NAME)
        {
            string PARM_VALUE = string.Empty;
            PARM_VALUE = (string)_ValidationList.Where(w => w.PARM_NAME == PARM_NAME).Select(s => s.PARM_VALUE).FirstOrDefault();
            return PARM_VALUE;
        }
        public string GetColValue(long mealTypeId, long mealOpID, string col_name)
        {
            string PARM_VALUE = string.Empty;
            var result = TIME_DTL_LIST.Where(w => (mealTypeId == 0 ? 1 == 1 : w.MEALTYPEID == mealTypeId) && (mealOpID == 0 ? 1 == 1 : w.MEAL_OPTIONID == mealOpID)).FirstOrDefault();
            if (result != null)
            {
                PARM_VALUE = result.GetType().GetProperties().Where(a => a.Name == col_name).Select(p => p.GetValue(result, null)).FirstOrDefault().ToString();
            }
            return PARM_VALUE;
        }

        public List<CSD_MealTimeMapViewModel> GetMealTimeDtl(long? mealTypeID, long? mealOpID)
        {
            return (from data in _dbContext.CSD_MEALTIME_MAP
                    where (mealTypeID == null ? 1 == 1 : data.MEALTYPEID == mealTypeID)
                    && (mealOpID == null ? 1 == 1 : data.MEAL_OPTIONID == mealOpID)
                    && data.STATUS == 1
                    select new CSD_MealTimeMapViewModel
                    {
                        CSD_TIMEMAPID = data.CSD_TIMEMAPID,
                        MEALTYPEID = data.MEALTYPEID,
                        BOOKING_DAY = data.BOOKING_DAY,
                        BOOKING_TIME = data.BOOKING_TIME,
                        CANCELLATION_DAY = data.CANCELLATION_DAY,
                        CANCELLATION_TIME = data.CANCELLATION_TIME,
                        GUEST_BOOKING_DAY = data.GUEST_BOOKING_DAY,
                        GUEST_BOOKING_TIME = data.GUEST_BOOKING_TIME,
                        GUEST_CANCELLATION_DAY = data.GUEST_CANCELLATION_DAY,
                        GUEST_CANCELLATION_TIME = data.GUEST_CANCELLATION_TIME,
                        MEAL_BOOKING_DURATION = data.MEAL_BOOKING_DURATION,
                        MEAL_OPTIONID = data.MEAL_OPTIONID
                    }).ToList();
        }

        public List<SelectListViewModel> GetApprovalAuth(long loginUser)
        {
            //var obj = (from data in _dbContext.ADEMPDIVDEPTSECT.Where(d => d.ADEMPCODE == loginUser && d.SYKI == _Syki.SYKIID)
            //           join _Emp in _dbContext.ADEMPLOYEE on data.SUPSUPERVISOREMPCODE equals _Emp.ADEMPCODE
            //           join _RecEmp in _dbContext.ADEMPLOYEE on data.SUPERVISOREMPCODE equals _RecEmp.ADEMPCODE
            //           select new
            //           {
            //               EmpCode = data.SUPSUPERVISOREMPCODE,
            //               EmpName = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " [" + _Emp.ADEMPCODE + "]",
            //               RecEmpCode = data.SUPERVISOREMPCODE,
            //               RecEmpName = _RecEmp.FIRSTNAME + " " + _RecEmp.LASTNAME + " [" + _RecEmp.ADEMPCODE + "]"
            //           }).FirstOrDefault();

            var obj = (from data in _dbContext.VW_ASSOCIATELVLDETAILS.Where(d => d.ADEMPCODE == loginUser && d.SYKI == _Syki.SYKIID)
                       join _Emp in _dbContext.ADEMPLOYEE on data.SUPSUPERVISOREMPCODE equals _Emp.ADEMPCODE
                       join _RecEmp in _dbContext.ADEMPLOYEE on data.SUPERVISOREMPCODE equals _RecEmp.ADEMPCODE
                       join _AdCoor in _dbContext.ADORGCOORDINATOR on data.OPERATIONID equals _AdCoor.ADORGLEVELID into _AdCoorJoin
                       from AdCoordinator in _AdCoorJoin.Where(o => o.ISACTIVE == 1).DefaultIfEmpty()
                       select new
                       {
                           EmpCode = data.SUPSUPERVISOREMPCODE,
                           EmpName = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " [" + _Emp.ADEMPCODE + "]",
                           RecEmpCode = data.SUPERVISOREMPCODE,
                           RecEmpName = _RecEmp.FIRSTNAME + " " + _RecEmp.LASTNAME + " [" + _RecEmp.ADEMPCODE + "]",
                           ECODE = data.ADEMPCODE,
                           OPERATING_HEAD = AdCoordinator.OPHEAD,
                           DIRECTOR = AdCoordinator.DIRECTOR,
                           DIRECTOR2 = AdCoordinator.DIRECTOR2,
                           DESGID = data.ADDESIGNATIONID,
                           FNDESGID = data.ADFUNCTIONALDESIGNATIONID,
                       }).FirstOrDefault();

            List<SelectListViewModel> iList = new List<SelectListViewModel>();
            if (obj != null)
            {
                string strDesginations = GetParmvalue("GUEST_BOOKING_SELFAPP");
                string[] _desgIds = strDesginations.Split(','); // { 12, 26, 27, 28, 29, 30, 31, 34 }; //// Desgination ids for self approval
                if (Convert.ToInt64(obj.FNDESGID) == 4 || _desgIds.Contains(Convert.ToInt64(obj.DESGID).ToString()))
                {
                    iList.Add(new SelectListViewModel
                    {
                        Value = Convert.ToInt64(obj.ECODE),
                        Text = _dbContext.ADEMPLOYEE.Where(w => w.ADEMPCODE == obj.ECODE).Select(s => s.FIRSTNAME + " " + s.LASTNAME + " [" + s.ADEMPCODE + "]").FirstOrDefault(),
                    });
                }
                else if (obj.ECODE > 70000000 && obj.ECODE <= 79999999)
                {
                    if (Convert.ToInt64(obj.DIRECTOR) > 70000000 && Convert.ToInt64(obj.DIRECTOR) <= 79999999)
                    {
                        if (obj.DIRECTOR != null)
                        {
                            iList.Add(new SelectListViewModel
                            {
                                Value = Convert.ToInt64(obj.ECODE),
                                Text = _dbContext.ADEMPLOYEE.Where(w => w.ADEMPCODE == obj.DIRECTOR).Select(s => s.FIRSTNAME + " " + s.LASTNAME + " [" + s.ADEMPCODE + "]").FirstOrDefault(),
                            });
                        }
                    }
                    else if (Convert.ToInt64(obj.DIRECTOR2) > 70000000 && Convert.ToInt64(obj.DIRECTOR2) <= 79999999)
                    {
                        if (obj.DIRECTOR2 != null)
                        {
                            iList.Add(new SelectListViewModel
                            {
                                Value = Convert.ToInt64(obj.ECODE),
                                Text = _dbContext.ADEMPLOYEE.Where(w => w.ADEMPCODE == obj.DIRECTOR2).Select(s => s.FIRSTNAME + " " + s.LASTNAME + " [" + s.ADEMPCODE + "]").FirstOrDefault(),
                            });
                        }
                    }
                }
                else
                {
                    if (obj.RecEmpCode != null)
                    {
                        iList.Add(new SelectListViewModel
                        {
                            Value = Convert.ToInt64(obj.RecEmpCode),
                            Text = obj.RecEmpName,
                        });
                    }
                    if (obj.EmpCode != null)
                    {
                        iList.Add(new SelectListViewModel
                        {
                            Value = Convert.ToInt64(obj.EmpCode),
                            Text = obj.EmpName,
                        });
                    }
                    if (obj.OPERATING_HEAD != null)
                    {
                        iList.Add(new SelectListViewModel
                        {
                            Value = Convert.ToInt64(obj.ECODE),
                            Text = _dbContext.ADEMPLOYEE.Where(w => w.ADEMPCODE == obj.OPERATING_HEAD).Select(s => s.FIRSTNAME + " " + s.LASTNAME + " [" + s.ADEMPCODE + "]").FirstOrDefault(),
                        });
                    }
                }
            }
            return iList;
        }

        public List<CSDMealsAvailabilityViewModel> GetAvailabilityMealList(string fromDate, string toDate, long mealType, long mealoptionID, long loginUser, long plantId)
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

            var mealTypeList = (from data in _dbContext.CSD_MEALTYPE_MST.Where(w => w.CSD_MEALTYPEID == mealType)
                                join _canteenMap in _dbContext.CSD_CANTEENMEALTYPE_MAP on data.CSD_MEALTYPEID equals _canteenMap.MEALTYPE_ID
                                join _canteen in _dbContext.CSD_CANTEEN_MST on _canteenMap.CANTEENID equals _canteen.CSD_CANTEENID
                                select new
                                {
                                    CSD_MEALTYPEID = data.CSD_MEALTYPEID,
                                    PLANTID = _canteen.PLANTID
                                }).Distinct();


            List<CSDMealsAvailabilityViewModel> availabilityList = (from _mealAvail in _dbContext.CSD_MEAL_AVAILABILITY.Where(w => (!string.IsNullOrEmpty(fromDate) ? (w.MEAL_MAPPING_DATE >= bookingFromDate) : true) && (!string.IsNullOrEmpty(toDate) ? (w.MEAL_MAPPING_DATE <= bookingToDate) : true))
                                                                    join _mealMst in _dbContext.CSD_MEALMST on _mealAvail.MEALS_ID equals _mealMst.CSD_MEALID
                                                                    join _mealType in _dbContext.CSD_MEALTYPE_MST on _mealMst.MEALTYPEID equals _mealType.CSD_MEALTYPEID
                                                                    join _optionMst in _dbContext.CSD_MEALOPTION_MST on _mealAvail.MEAL_OPTIONID equals _optionMst.CSDMEAL_OPTIONID
                                                                    join _mealPlant in mealTypeList on _mealType.CSD_MEALTYPEID equals _mealPlant.CSD_MEALTYPEID
                                                                    //join _canteenMap in _dbContext.CSD_CANTEENMEALTYPE_MAP on _mealType.CSD_MEALTYPEID equals _canteenMap.MEALTYPE_ID
                                                                    //join _canteen in _dbContext.CSD_CANTEEN_MST on _canteenMap.CANTEENID equals _canteen.CSD_CANTEENID
                                                                    where _mealMst.MEALTYPEID == mealType
                                                                    && _mealMst.STATUS == 1 && _mealAvail.STATUS == 1
                                                                    && _mealAvail.MEAL_OPTIONID == mealoptionID
                                                                    && (plantId == 5 ? _mealPlant.PLANTID == 1 : _mealPlant.PLANTID == plantId)
                                                                    //&& (plantId == 5 ? _canteen.PLANTID == 1 : _canteen.PLANTID == plantId)
                                                                    select new CSDMealsAvailabilityViewModel
                                                                    {
                                                                        CSD_MEAL_AVAILABILITY_ID = _mealAvail.CSD_MEAL_AVAILABILITY_ID,
                                                                        MEAL_MAPPING_DATE = _mealAvail.MEAL_MAPPING_DATE,
                                                                        MEALS_ID = _mealAvail.MEALS_ID,
                                                                        MealMst_MODEL = new CSDMealMstViewModel
                                                                        {
                                                                            CSD_MEALID = _mealMst.CSD_MEALID,
                                                                            MEALTYPEID = _mealMst.MEALTYPEID,
                                                                            MealTypeModel = new CSDMealTypeViewModel
                                                                            {
                                                                                CSD_MEALTYPEID = _mealType.CSD_MEALTYPEID,
                                                                                MEAL_TYPE_DESC = _mealType.MEAL_TYPE_DESC,
                                                                                MEAL_QTY = _mealType.MEAL_QTY
                                                                            },
                                                                            MEAL_NAME = _mealMst.MEAL_NAME,
                                                                            MEAL_DESC = _mealMst.MEAL_DESC,
                                                                            MEAL_PRICE = _mealMst.MEAL_PRICE,
                                                                            MEAL_PHOTO = _mealMst.MEAL_PHOTO,
                                                                            MEAL_PHOTO_NAME = _mealMst.MEAL_PHOTO_NAME,
                                                                        },
                                                                        MealCanMapList = (from mealTypeCantMap in _dbContext.CSD_CANTEENMEALTYPE_MAP.Where(c => c.MEALTYPE_ID == _mealType.CSD_MEALTYPEID)
                                                                                          join _canteenMst in _dbContext.CSD_CANTEEN_MST on mealTypeCantMap.CANTEENID equals _canteenMst.CSD_CANTEENID
                                                                                          where (plantId == 5 ? _canteenMst.PLANTID == 1 : _canteenMst.PLANTID == plantId)
                                                                                          select new CSDCanteenMealMapViewModel
                                                                                          {
                                                                                              CSD_CMTMAPID = mealTypeCantMap.CSD_CMTMAPID,
                                                                                              MEALTYPE_ID = mealTypeCantMap.MEALTYPE_ID,
                                                                                              CANTEENID = mealTypeCantMap.CANTEENID,
                                                                                              CANTEEN_NAME = _canteenMst.CANTEEN_NAME
                                                                                          }).ToList(),
                                                                        MEALOPTIONID = _optionMst.CSDMEAL_OPTIONID,
                                                                        MEALOPTION = _optionMst.MEAL_OPTION,
                                                                    }).ToList();

            return availabilityList.OrderBy(o => o.MEAL_MAPPING_DATE).ToList();
        }

        public List<CanteenMealBookingViewModel> GetBookingHistory(long mealType, string fromDate, string toDate, long loginUser)
        {
            DateTime currentDate = DateTime.Now;
            string cancellationTime = "23:59:00";
            int MealBookDay = 0;

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
            var iList = (from data in _dbContext.CSD_MEALBOOKING_TRN.Where(w => w.ADDEDBY == loginUser)
                         join _mealType in _dbContext.CSD_MEALTYPE_MST on data.MEALTYPEID equals _mealType.CSD_MEALTYPEID
                         join _mealAvail in _dbContext.CSD_MEAL_AVAILABILITY on data.MEAL_AVAILABILITY_ID equals _mealAvail.CSD_MEAL_AVAILABILITY_ID
                         join _mealMst in _dbContext.CSD_MEALMST on _mealAvail.MEALS_ID equals _mealMst.CSD_MEALID
                         join _mealOption in _dbContext.CSD_MEALOPTION_MST on data.MEALOPTIONID equals _mealOption.CSDMEAL_OPTIONID
                         where (!string.IsNullOrEmpty(fromDate) ? (data.MEAL_BOOKED_DATE >= bookingFromDate) : true)
                                                          && (!string.IsNullOrEmpty(toDate) ? (data.MEAL_BOOKED_DATE <= bookingToDate) : true)
                                                          && (mealType == 0 ? true : data.MEALTYPEID == mealType)
                         select new CanteenMealBookingViewModel
                         {
                             CSD_MEALSBOOKING_ID = data.CSD_MEALSBOOKING_ID,
                             MEALTYPEID = data.MEALTYPEID,
                             MealType = new CSDMealTypeViewModel
                             {
                                 CSD_MEALTYPEID = _mealType.CSD_MEALTYPEID,
                                 MEAL_TYPE_DESC = _mealType.MEAL_TYPE_DESC,
                                 MEAL_QTY = _mealType.MEAL_QTY
                             },
                             MEAL_BOOKED_DATE = data.MEAL_BOOKED_DATE,
                             MEAL_STATUS = data.MEAL_STATUS,
                             MEALS_AVAILABILITY_ID = data.MEAL_AVAILABILITY_ID,
                             MealMst = new CSDMealMstViewModel
                             {
                                 CSD_MEALID = _mealMst.CSD_MEALID,
                                 MEALTYPEID = _mealMst.MEALTYPEID,
                                 MEAL_NAME = _mealMst.MEAL_NAME,
                                 MEAL_PRICE = _mealMst.MEAL_PRICE,
                                 MEAL_DESC = _mealMst.MEAL_DESC,
                                 MEAL_PHOTO = _mealMst.MEAL_PHOTO,
                                 MEAL_PHOTO_NAME = _mealMst.MEAL_PHOTO_NAME,
                             },
                             ADDEDBY = data.ADDEDBY,
                             ADDEDDATE = data.ADDEDDATE,
                             MEALOPTIONID = _mealOption.CSDMEAL_OPTIONID,
                             MEALOPTION = _mealOption.MEAL_OPTION,
                         }).ToList();
            List<CanteenMealBookingViewModel> newList = new List<CanteenMealBookingViewModel>();
            foreach (var obj in iList)
            {
                if (TIME_DTL_LIST.Count > 0)
                {
                    string CANCELLATION_TIME = GetColValue(obj.MEALTYPEID, obj.MEALOPTIONID, "CANCELLATION_TIME");
                    cancellationTime = !string.IsNullOrEmpty(CANCELLATION_TIME) ? CANCELLATION_TIME : cancellationTime;

                    string CANCELLATION_ALLOW_DAYS = GetColValue(obj.MEALTYPEID, obj.MEALOPTIONID, "CANCELLATION_DAY");
                    MealBookDay = !string.IsNullOrEmpty(CANCELLATION_ALLOW_DAYS) ? Convert.ToInt32(CANCELLATION_ALLOW_DAYS) : MealBookDay;
                }
                DateTime validCancelDate = DateTime.ParseExact(obj.MEAL_BOOKED_DATE.ToString("dd-MMM-yyyy") + " " + cancellationTime, "dd-MMM-yyyy HH:mm:ss", null).AddDays(-MealBookDay);
                obj.IsEnableCancel = currentDate < validCancelDate ? true : false;
                newList.Add(obj);
            }
            return newList.OrderByDescending(o => o.MEAL_BOOKED_DATE).ThenByDescending(t => t.CSD_MEALSBOOKING_ID).ToList();
        }

        public Tuple<short, string> SaveMealBooking(long AddedBy, CanteenMealBookingViewModel model)
        {
            short retVal = 0;
            string msg = "";
            Tuple<short, string> _tuple = new Tuple<short, string>(retVal, msg);
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    CSD_MEALBOOKING_TRN BCM = new CSD_MEALBOOKING_TRN();
                    if (model.MEALS_AVAILABILITY_ID > 0)
                    {
                        #region Check Validation
                        DateTime bookingDate = DateTime.Now.Date;
                        if (!string.IsNullOrEmpty(model.strBOOKED_DATE))
                        {
                            bookingDate = DateTime.ParseExact(model.strBOOKED_DATE, "dd-MMM-yyyy", null);
                        }

                        // if (_dbContext.CSD_MEALBOOKING_TRN.Any(x => x.MEAL_BOOKED_DATE == bookingDate && x.ADDEDBY == AddedBy && x.MEAL_STATUS == 1 && x.MEALOPTIONID == model.MEALOPTIONID))


                        var exist = _dbContext.CSD_MEALBOOKING_TRN
                             .Count(x =>
                             x.MEAL_BOOKED_DATE == bookingDate &&
                             x.ADDEDBY == AddedBy &&
                             x.MEAL_STATUS == 1 &&
                             x.MEALOPTIONID == model.MEALOPTIONID) > 0;

                        if (exist)
                        {
                            retVal = 2;
                            msg = model.strBOOKED_DATE;
                            transaction.Rollback();
                            return _tuple = new Tuple<short, string>(retVal, msg); //// -- record already exist.
                        }

                        if (TIME_DTL_LIST.Count > 0)
                        {
                            string MEAL_BOOKING_DURATION = GetColValue(model.MEALTYPEID, model.MEALOPTIONID, "MEAL_BOOKING_DURATION");
                            string BOOKING_ALLOW_DAYS = GetColValue(model.MEALTYPEID, model.MEALOPTIONID, "BOOKING_DAY");
                            string MEAL_BOOKING_TIME = GetColValue(model.MEALTYPEID, model.MEALOPTIONID, "BOOKING_TIME");

                            if (string.IsNullOrEmpty(MEAL_BOOKING_DURATION) || string.IsNullOrEmpty(MEAL_BOOKING_TIME) || string.IsNullOrEmpty(BOOKING_ALLOW_DAYS))
                            {
                                retVal = 5;
                                msg = "";
                                transaction.Rollback();
                                return _tuple = new Tuple<short, string>(retVal, msg); //// -- Validation master not maintained.
                            }
                            else
                            {
                                DateTime currentDate = DateTime.Now.Date;
                                int bookingDuration = Convert.ToInt32(MEAL_BOOKING_DURATION);
                                DateTime ValidDuration = currentDate.AddDays(bookingDuration);
                                if (bookingDate > ValidDuration)
                                {
                                    retVal = 4;
                                    msg = ValidDuration.ToString("dd-MMM-yyyy");
                                    transaction.Rollback();
                                    return _tuple = new Tuple<short, string>(retVal, msg);
                                }

                                currentDate = DateTime.Now;
                                string bookingTime = MEAL_BOOKING_TIME;
                                int validDay = string.IsNullOrEmpty(BOOKING_ALLOW_DAYS) ? 0 : Convert.ToInt32(BOOKING_ALLOW_DAYS);
                                DateTime ValidDateTime = DateTime.ParseExact(model.strBOOKED_DATE + " " + bookingTime, "dd-MMM-yyyy HH:mm:ss", null).AddDays(-validDay);
                                if (currentDate > ValidDateTime)
                                {
                                    retVal = 3;
                                    msg = model.strBOOKED_DATE + " after " + ValidDateTime.ToString("HH:mm") + " hrs";
                                    transaction.Rollback();
                                    return _tuple = new Tuple<short, string>(retVal, msg); //// -- Book before n-1 day.
                                }
                            }
                        }
                        else
                        {
                            retVal = 5;
                            msg = "";
                            transaction.Rollback();
                            return _tuple = new Tuple<short, string>(retVal, msg); //// -- Validation master not maintained.
                        }
                        #endregion

                        #region Check QTY
                        long VALID_BOOK_QTY = 0;
                        //if (model.MEALTYPEID == 2) // DietMeal
                        //{
                        //    VALID_BOOK_QTY = GetParmvalue("DIET_MEAL_QTY");
                        //}
                        //if (model.MEALTYPEID == 3) // Special Meal
                        //{
                        //    VALID_BOOK_QTY = GetParmvalue("SPECIAL_MEAL_QTY");
                        //}
                        VALID_BOOK_QTY = model.MEAL_QTY;
                        int bookedCount = _dbContext.CSD_MEALBOOKING_TRN.Count(x => x.MEAL_BOOKED_DATE == bookingDate && x.MEALTYPEID == model.MEALTYPEID && x.MEAL_STATUS == 1 && x.MEALOPTIONID == model.MEALOPTIONID);

                        int guestBookedCount = (from data in _dbContext.CSD_GUESTMEALBOOKING_TRN.Where(x => x.MEAL_BOOKED_DATE == bookingDate && x.MEALTYPEID == model.MEALTYPEID && (x.MEAL_STATUS == 1 || x.MEAL_STATUS == 0) && x.MEALOPTIONID == model.MEALOPTIONID)
                                                join _dtl in _dbContext.CSD_GUESTMEALBOOKING_DTL on data.CSD_GUEST_BOOKINGID equals _dtl.CSD_GUESTMEALBOOKINGID
                                                select data).Count();

                        int totalBookedCount = (guestBookedCount + bookedCount);
                        if (totalBookedCount >= VALID_BOOK_QTY)
                        {
                            retVal = 6;
                            msg = bookingDate.ToString("dd-MMM-yyyy");
                            transaction.Rollback();
                            return _tuple = new Tuple<short, string>(retVal, msg); //// -- Meal(Diet/Special) booking has exceeded the maximum limit.
                        }
                        #endregion

                        if (_dbContext.CSD_MEALBOOKING_TRN.Count() == 0)
                        {
                            BCM.CSD_MEALSBOOKING_ID = 1;
                        }
                        else
                        {
                            BCM.CSD_MEALSBOOKING_ID = _dbContext.CSD_MEALBOOKING_TRN.Max(x => x.CSD_MEALSBOOKING_ID) + 1;
                        }

                        BCM.MEAL_AVAILABILITY_ID = model.MEALS_AVAILABILITY_ID;
                        BCM.MEALTYPEID = model.MEALTYPEID;
                        BCM.MEAL_BOOKED_DATE = bookingDate;
                        BCM.MEAL_STATUS = model.MEAL_STATUS;
                        BCM.MEALOPTIONID = model.MEALOPTIONID;
                        BCM.ADDEDBY = AddedBy;
                        BCM.ADDEDDATE = DateTime.Now;
                        _dbContext.Entry(BCM).State = EntityState.Added;
                        _dbContext.SaveChanges();

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

        public short CancelBooking(long id, long updatedBy)
        {
            short retVal = 0;
            try
            {
                CSD_MEALBOOKING_TRN model = new CSD_MEALBOOKING_TRN();
                model = _dbContext.CSD_MEALBOOKING_TRN.Where(w => w.CSD_MEALSBOOKING_ID == id).FirstOrDefault();
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

        //// Guest Meal Boking
        public List<CSDGuestMealBookingTrnViewModel> GetGuestBookedMealList(long mealType, string fromDate, string toDate, long loginUser)
        {
            DateTime currentDate = DateTime.Now;
            string cancellationTime = "23:59:00";
            int guest_MealDay = 0;

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

            var iList = (from data in _dbContext.CSD_GUESTMEALBOOKING_TRN.Where(w => w.ADDEDBY == loginUser)
                         join _ApproverAuth in _dbContext.ADEMPLOYEE on data.APPROVER_ECODE equals _ApproverAuth.ADEMPCODE into _ApproverAuthJoin
                         from _Approver in _ApproverAuthJoin.DefaultIfEmpty()
                         join _mealType in _dbContext.CSD_MEALTYPE_MST on data.MEALTYPEID equals _mealType.CSD_MEALTYPEID
                         join _mealAvail in _dbContext.CSD_MEAL_AVAILABILITY on data.MEALS_AVAILABILITY_ID equals _mealAvail.CSD_MEAL_AVAILABILITY_ID
                         join _mealOption in _dbContext.CSD_MEALOPTION_MST on data.MEALOPTIONID equals _mealOption.CSDMEAL_OPTIONID
                         join _mealMst in _dbContext.CSD_MEALMST on _mealAvail.MEALS_ID equals _mealMst.CSD_MEALID
                         where (!string.IsNullOrEmpty(fromDate) ? (data.MEAL_BOOKED_DATE >= bookingFromDate) : true)
                                                          && (!string.IsNullOrEmpty(toDate) ? (data.MEAL_BOOKED_DATE <= bookingToDate) : true)
                                                          && (mealType == 0 ? true : data.MEALTYPEID == mealType)
                         select new CSDGuestMealBookingTrnViewModel
                         {
                             CSD_GUEST_BOOKINGID = data.CSD_GUEST_BOOKINGID,
                             MEALTYPEID = data.MEALTYPEID,
                             MealType = new CSDMealTypeViewModel
                             {
                                 CSD_MEALTYPEID = _mealType.CSD_MEALTYPEID,
                                 MEAL_TYPE_DESC = _mealType.MEAL_TYPE_DESC,
                                 MEAL_QTY = _mealType.MEAL_QTY
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
                             GuestDtlList = (from _guestData in _dbContext.CSD_GUESTMEALBOOKING_DTL.Where(d => d.CSD_GUESTMEALBOOKINGID == data.CSD_GUEST_BOOKINGID)
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
                             MealMst = new CSDMealMstViewModel
                             {
                                 CSD_MEALID = _mealMst.CSD_MEALID,
                                 MEALTYPEID = _mealMst.MEALTYPEID,
                                 MEAL_NAME = _mealMst.MEAL_NAME,
                                 MEAL_PRICE = _mealMst.MEAL_PRICE,
                                 MEAL_DESC = _mealMst.MEAL_DESC,
                                 MEAL_PHOTO = _mealMst.MEAL_PHOTO,
                                 MEAL_PHOTO_NAME = _mealMst.MEAL_PHOTO_NAME,
                             },
                             ADDEDBY = data.ADDEDBY,
                             ADDEDDATE = data.ADDEDDATE,
                             MEALOPTIONID = _mealOption.CSDMEAL_OPTIONID,
                             MEALOPTION = _mealOption.MEAL_OPTION,
                             //SLOTMSTID = data.SLOTID,
                             //SLOT_TIME = _slotMst.SLOT_TIME
                         }).ToList();




            List<CSDGuestMealBookingTrnViewModel> newList = new List<CSDGuestMealBookingTrnViewModel>();
            foreach (var obj in iList)
            {
                if (TIME_DTL_LIST.Count > 0)
                {
                    string CANCELLATION_TIME = GetColValue(obj.MEALTYPEID, obj.MEALOPTIONID, "GUEST_CANCELLATION_TIME");
                    cancellationTime = !string.IsNullOrEmpty(CANCELLATION_TIME) ? CANCELLATION_TIME : cancellationTime;

                    string CANCELLATION_ALLOW_DAYS = GetColValue(obj.MEALTYPEID, obj.MEALOPTIONID, "GUEST_CANCELLATION_DAY");
                    guest_MealDay = !string.IsNullOrEmpty(CANCELLATION_ALLOW_DAYS) ? Convert.ToInt32(CANCELLATION_ALLOW_DAYS) : guest_MealDay;
                }
                DateTime validDate = DateTime.ParseExact(obj.MEAL_BOOKED_DATE.ToString("dd-MMM-yyyy") + " " + cancellationTime, "dd-MMM-yyyy HH:mm:ss", null).AddDays(-guest_MealDay);
                obj.IsEnableCancel = currentDate < validDate ? true : false;
                newList.Add(obj);
            }

            return newList.OrderByDescending(o => o.MEAL_BOOKED_DATE).ToList();
        }

        public List<CSDMealsAvailabilityViewModel> GetGuestAvailabilityMealList(string date, long? mealType, long mealopID, long plantId)
        {
            DateTime bookingDate = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(date))
            {
                bookingDate = DateTime.ParseExact(date, "dd-MMM-yyyy", null);
            }

            //var mealTypeList = (from data in _dbContext.CSD_MEALTYPE_MST.Where(w => w.CSD_MEALTYPEID == mealType)
            //                    join _canteenMap in _dbContext.CSD_CANTEENMEALTYPE_MAP on data.CSD_MEALTYPEID equals _canteenMap.MEALTYPE_ID
            //                    join _canteen in _dbContext.CSD_CANTEEN_MST on _canteenMap.CANTEENID equals _canteen.CSD_CANTEENID
            //                    select new
            //                    {
            //                        CSD_MEALTYPEID = data.CSD_MEALTYPEID,
            //                        PLANTID = _canteen.PLANTID
            //                    }).Distinct();

            //var iList = (from _mealAvail in _dbContext.CSD_MEAL_AVAILABILITY
            //             join _mealMst in _dbContext.CSD_MEALMST on _mealAvail.MEALS_ID equals _mealMst.CSD_MEALID
            //             join _mealType in _dbContext.CSD_MEALTYPE_MST on _mealMst.MEALTYPEID equals _mealType.CSD_MEALTYPEID
            //             join _mealPlant in mealTypeList on _mealType.CSD_MEALTYPEID equals _mealPlant.CSD_MEALTYPEID
            //             join _optionMst in _dbContext.CSD_MEALOPTION_MST on _mealAvail.MEAL_OPTIONID equals _optionMst.CSDMEAL_OPTIONID
            //             where (string.IsNullOrEmpty(date) ? true : _mealAvail.MEAL_MAPPING_DATE == bookingDate)
            //             && (mealType == null || mealType == 0 ? true : _mealMst.MEALTYPEID == mealType)
            //             && _mealMst.STATUS == 1 && _mealAvail.STATUS == 1
            //             && (plantId == 5 ? _mealPlant.PLANTID == 1 : _mealPlant.PLANTID == plantId)
            //             && _optionMst.CSDMEAL_OPTIONID == mealopID
            //             select new CSDMealsAvailabilityViewModel
            //             {
            //                 CSD_MEAL_AVAILABILITY_ID = _mealAvail.CSD_MEAL_AVAILABILITY_ID,
            //                 MEAL_MAPPING_DATE = _mealAvail.MEAL_MAPPING_DATE,
            //                 MEALS_ID = _mealAvail.MEALS_ID,
            //                 MealMst_MODEL = new CSDMealMstViewModel
            //                 {
            //                     CSD_MEALID = _mealMst.CSD_MEALID,
            //                     MEALTYPEID = _mealMst.MEALTYPEID,
            //                     MealTypeModel = new CSDMealTypeViewModel
            //                     {
            //                         CSD_MEALTYPEID = _mealType.CSD_MEALTYPEID,
            //                         MEAL_TYPE_DESC = _mealType.MEAL_TYPE_DESC,
            //                         MEAL_QTY = _mealType.MEAL_QTY,
            //                         MEAL_PRICE = (_dbContext.CSD_PRICE_VALIDITY.Where(p => p.MEALTYPEID == _mealType.CSD_MEALTYPEID && p.STATUS == 1).Select(s => s.MEAL_AMOUNT).FirstOrDefault())
            //                     },
            //                     MEAL_NAME = _mealMst.MEAL_NAME,
            //                     MEAL_DESC = _mealMst.MEAL_DESC,
            //                     MEAL_PRICE = _mealMst.MEAL_PRICE,
            //                     MEAL_PHOTO = _mealMst.MEAL_PHOTO,
            //                     MEAL_PHOTO_NAME = _mealMst.MEAL_PHOTO_NAME,
            //                 },
            //                 STATUS = _mealAvail.STATUS == 1 ? true : false,
            //                 ADDEDBY = _mealAvail.ADDEDBY,
            //                 ADDEDDATE = _mealAvail.ADDEDDATE,
            //                 MEALOPTIONID = _optionMst.CSDMEAL_OPTIONID,
            //                 MEALOPTION = _optionMst.MEAL_OPTION,
            //             }).ToList();


            // Pre-fetch valid prices grouped by MealType
            var mealPriceDict = _dbContext.CSD_PRICE_VALIDITY
                .Where(p => p.STATUS == 1)
                .GroupBy(p => p.MEALTYPEID)
                .Select(g => new { MealTypeId = g.Key, Amount = g.FirstOrDefault().MEAL_AMOUNT })
                .ToDictionary(x => x.MealTypeId, x => x.Amount);

            var mealTypeList = (from data in _dbContext.CSD_MEALTYPE_MST.Where(w => w.CSD_MEALTYPEID == mealType)
                                join _canteenMap in _dbContext.CSD_CANTEENMEALTYPE_MAP on data.CSD_MEALTYPEID equals _canteenMap.MEALTYPE_ID
                                join _canteen in _dbContext.CSD_CANTEEN_MST on _canteenMap.CANTEENID equals _canteen.CSD_CANTEENID
                                select new
                                {
                                    CSD_MEALTYPEID = data.CSD_MEALTYPEID,
                                    PLANTID = _canteen.PLANTID
                                }).Distinct();

            var iList = (from _mealAvail in _dbContext.CSD_MEAL_AVAILABILITY
                         join _mealMst in _dbContext.CSD_MEALMST on _mealAvail.MEALS_ID equals _mealMst.CSD_MEALID
                         join _mealType in _dbContext.CSD_MEALTYPE_MST on _mealMst.MEALTYPEID equals _mealType.CSD_MEALTYPEID
                         join _mealPlant in mealTypeList on _mealType.CSD_MEALTYPEID equals _mealPlant.CSD_MEALTYPEID
                         join _optionMst in _dbContext.CSD_MEALOPTION_MST on _mealAvail.MEAL_OPTIONID equals _optionMst.CSDMEAL_OPTIONID
                         where (string.IsNullOrEmpty(date) || _mealAvail.MEAL_MAPPING_DATE == bookingDate)
                         && (mealType == null || mealType == 0 || _mealMst.MEALTYPEID == mealType)
                         && _mealMst.STATUS == 1 && _mealAvail.STATUS == 1
                         //&& (plantId == 5 ? _mealPlant.PLANTID == 1 : _mealPlant.PLANTID == plantId)
                         && (_mealPlant.PLANTID == (plantId == 5 ? 1 : plantId))
                         && _optionMst.CSDMEAL_OPTIONID == mealopID
                         select new CSDMealsAvailabilityViewModel
                         {
                             CSD_MEAL_AVAILABILITY_ID = _mealAvail.CSD_MEAL_AVAILABILITY_ID,
                             MEAL_MAPPING_DATE = _mealAvail.MEAL_MAPPING_DATE,
                             MEALS_ID = _mealAvail.MEALS_ID,
                             MealMst_MODEL = new CSDMealMstViewModel
                             {
                                 CSD_MEALID = _mealMst.CSD_MEALID,
                                 MEALTYPEID = _mealMst.MEALTYPEID,
                                 MealTypeModel = new CSDMealTypeViewModel
                                 {
                                     CSD_MEALTYPEID = _mealType.CSD_MEALTYPEID,
                                     MEAL_TYPE_DESC = _mealType.MEAL_TYPE_DESC,
                                     MEAL_QTY = _mealType.MEAL_QTY,
                                     //MEAL_PRICE = (_dbContext.CSD_PRICE_VALIDITY.Where(p => p.MEALTYPEID == _mealType.CSD_MEALTYPEID && p.STATUS == 1).Select(s => s.MEAL_AMOUNT).FirstOrDefault())
                                     MEAL_PRICE = mealPriceDict.ContainsKey(_mealType.CSD_MEALTYPEID)
                             ? mealPriceDict[_mealType.CSD_MEALTYPEID]
                             : 0
                                 },
                                 MEAL_NAME = _mealMst.MEAL_NAME,
                                 MEAL_DESC = _mealMst.MEAL_DESC,
                                 MEAL_PRICE = _mealMst.MEAL_PRICE,
                                 MEAL_PHOTO = _mealMst.MEAL_PHOTO,
                                 MEAL_PHOTO_NAME = _mealMst.MEAL_PHOTO_NAME,
                             },
                             STATUS = true, //_mealAvail.STATUS == 1 ? true : false,
                             ADDEDBY = _mealAvail.ADDEDBY,
                             ADDEDDATE = _mealAvail.ADDEDDATE,
                             MEALOPTIONID = _optionMst.CSDMEAL_OPTIONID,
                             MEALOPTION = _optionMst.MEAL_OPTION,
                         }).ToList();

            return iList;
        }

        public Tuple<short, string, long> SaveGuestMealBooking(long id, CSDGuestMealBookingTrnViewModel model, List<GuestDtlViewModel> guestList)
        {
            short retVal = 0;
            string msg = "";
            long _headerId = 0;
            Tuple<short, string, long> _tuple = new Tuple<short, string, long>(retVal, msg, _headerId);
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    CSD_GUESTMEALBOOKING_TRN GMB = new CSD_GUESTMEALBOOKING_TRN();

                    #region Check Validation
                    DateTime bookingDate = DateTime.Now.Date;
                    if (!string.IsNullOrEmpty(model.strBOOKED_DATE))
                    {
                        bookingDate = DateTime.ParseExact(model.strBOOKED_DATE, "dd-MMM-yyyy", null);
                    }

                    //List<long> guestIds = guestList.Select(s => s.BC_GUESTID).ToList();
                    //int _recordCount = (from data in _dbContext.CSD_GUESTMEALBOOKING_TRN.Where(x => x.MEAL_BOOKED_DATE == bookingDate && x.MEAL_STATUS != 2 && x.MEAL_STATUS != 3 && x.MEALOPTIONID == model.MEALOPTIONID)
                    //                    join _GuestDTL in _dbContext.CSD_GUESTMEALBOOKING_DTL on data.CSD_GUEST_BOOKINGID equals _GuestDTL.CSD_BOOKINGDTLID
                    //                    where guestIds.Contains(_GuestDTL.GUESTDTLID)
                    //                    select data).Count();

                    //if (_recordCount > 0)
                    //{
                    //    retVal = 2;
                    //    msg = model.strBOOKED_DATE;
                    //    return _tuple = new Tuple<short, string, long>(retVal, msg, _headerId); //// -- record already exist.
                    //}

                    if (TIME_DTL_LIST.Count > 0)
                    {
                        string MEAL_BOOKING_DURATION = GetColValue(model.MEALTYPEID, model.MEALOPTIONID, "MEAL_BOOKING_DURATION");
                        string GUEST_BOOKING_DAY = GetColValue(model.MEALTYPEID, model.MEALOPTIONID, "GUEST_BOOKING_DAY");
                        string GUEST_BOOKING_TIME = GetColValue(model.MEALTYPEID, model.MEALOPTIONID, "GUEST_BOOKING_TIME");

                        if (string.IsNullOrEmpty(MEAL_BOOKING_DURATION) || string.IsNullOrEmpty(GUEST_BOOKING_TIME) || string.IsNullOrEmpty(GUEST_BOOKING_DAY))
                        {
                            retVal = 5;
                            msg = "";
                            return _tuple = new Tuple<short, string, long>(retVal, msg, _headerId); //// -- Validation master not maintained.
                        }
                        else
                        {
                            DateTime currentDate = DateTime.Now.Date;
                            int bookingDuration = Convert.ToInt32(MEAL_BOOKING_DURATION);
                            DateTime ValidDuration = currentDate.AddDays(bookingDuration);
                            if (bookingDate > ValidDuration)
                            {
                                retVal = 4;
                                msg = ValidDuration.ToString("dd-MMM-yyyy");
                                return _tuple = new Tuple<short, string, long>(retVal, msg, _headerId); //// -- Book before n-1 day.
                            }

                            currentDate = DateTime.Now;
                            string bookingTime = GUEST_BOOKING_TIME;
                            int validDay = Convert.ToInt32(GUEST_BOOKING_DAY);
                            DateTime ValidDateTime = DateTime.ParseExact(model.strBOOKED_DATE + " " + bookingTime, "dd-MMM-yyyy HH:mm:ss", null).AddDays(-validDay);
                            if (currentDate > ValidDateTime)
                            {
                                retVal = 3;
                                msg = model.strBOOKED_DATE + " after " + ValidDateTime.ToString("HH:mm") + " hrs";
                                return _tuple = new Tuple<short, string, long>(retVal, msg, _headerId); //// -- Book before n-1 day.
                            }
                        }
                    }
                    #endregion

                    #region Check QTY
                    //if (model.MEALTYPEID != 1) // RegularMeal
                    //{
                    long VALID_BOOK_QTY = 0;
                    //if (model.MEALTYPEID == 2) // DietMeal
                    //{
                    //    VALID_BOOK_QTY = GetParmvalue("DIET_MEAL_QTY");
                    //}
                    //if (model.MEALTYPEID == 3) // Special Meal
                    //{
                    //    VALID_BOOK_QTY = GetParmvalue("SPECIAL_MEAL_QTY");
                    //}
                    VALID_BOOK_QTY = model.MEAL_QTY;
                    int guestBookedCount = (from data in _dbContext.CSD_GUESTMEALBOOKING_TRN.Where(x => x.MEAL_BOOKED_DATE == bookingDate && x.MEALTYPEID == model.MEALTYPEID && (x.MEAL_STATUS == 1 || x.MEAL_STATUS == 0) && x.MEALOPTIONID == model.MEALOPTIONID)
                                            join _dtl in _dbContext.CSD_GUESTMEALBOOKING_DTL on data.CSD_GUEST_BOOKINGID equals _dtl.CSD_GUESTMEALBOOKINGID
                                            select data).Count();

                    int bookedCount = _dbContext.CSD_MEALBOOKING_TRN.Count(x => x.MEAL_BOOKED_DATE == bookingDate && x.MEALTYPEID == model.MEALTYPEID && x.MEAL_STATUS == 1 && x.MEALOPTIONID == model.MEALOPTIONID);

                    int totalBookedCount = (guestBookedCount + bookedCount);
                    if (totalBookedCount >= VALID_BOOK_QTY)
                    {
                        retVal = 6;
                        msg = bookingDate.ToString("dd-MMM-yyyy");
                        return _tuple = new Tuple<short, string, long>(retVal, msg, _headerId); //// -- Meal booking has exceeded the maximum limit.
                    }
                    //}
                    #endregion

                    if (_dbContext.CSD_GUESTMEALBOOKING_TRN.Count() == 0)
                    {
                        GMB.CSD_GUEST_BOOKINGID = 1;
                    }
                    else
                    {
                        GMB.CSD_GUEST_BOOKINGID = _dbContext.CSD_GUESTMEALBOOKING_TRN.Max(x => x.CSD_GUEST_BOOKINGID) + 1;
                    }

                    GMB.PAYMENT_TYPE = model.PAYMENT_TYPE;
                    GMB.MEALS_AVAILABILITY_ID = model.MEALS_AVAILABILITY_ID;
                    GMB.MEALTYPEID = model.MEALTYPEID;
                    GMB.MEAL_BOOKED_DATE = bookingDate;
                    GMB.VISIT_PURPOSE = model.VISIT_PURPOSE;
                    GMB.APPROVER_ECODE = model.APPROVER_ECODE;
                    if (model.APPROVER_ECODE == model.ADDEDBY)
                    {
                        GMB.MEAL_STATUS = 1;
                        GMB.APPROVE_STATUS = 1;
                        GMB.APPROVE_DATE = DateTime.Now;
                        GMB.APPROVE_REMARK = "Auto Approved";
                    }
                    else
                    {
                        GMB.APPROVE_STATUS = model.APPROVER_ECODE == 0 ? (short)-1 : (short)0;
                        GMB.MEAL_STATUS = model.MEAL_STATUS;
                    }
                    GMB.MEALOPTIONID = model.MEALOPTIONID;
                    GMB.ADDEDBY = model.ADDEDBY;
                    GMB.ADDEDDATE = DateTime.Now;
                    _dbContext.Entry(GMB).State = EntityState.Added;
                    _dbContext.SaveChanges();

                    model.CSD_GUEST_BOOKINGID = GMB.CSD_GUEST_BOOKINGID;
                    //// -- Save Guest Detail --////
                    retVal = SaveGuestDtl(model, guestList);
                    if (retVal == 2) ///// token already exist.
                    {
                        transaction.Rollback();
                        model.CSD_GUEST_BOOKINGID = 0;
                        SaveGuestMealBooking(id, model, guestList);
                    }
                    else if (retVal == 1)
                    {
                        retVal = 1;
                        msg = "";
                        _headerId = GMB.CSD_GUEST_BOOKINGID;
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

        public short SaveGuestDtl(CSDGuestMealBookingTrnViewModel model, List<GuestDtlViewModel> guestList)
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

        public short SaveGuestMealBookingDtl(CSDGuestMealBookingTrnViewModel model, GuestDtlViewModel guestModel, string tokenCode)
        {
            short retVal = 0;
            try
            {
                CSD_GUESTMEALBOOKING_DTL GMB_DTL = new CSD_GUESTMEALBOOKING_DTL();

                var anyData = _dbContext.CSD_GUESTMEALBOOKING_DTL.Count(a => a.TOKEN_CODE == tokenCode) > 0;

                if (anyData)
                {
                    retVal = 2; //// -- Token already exist.
                    return retVal;
                }

                if (_dbContext.CSD_GUESTMEALBOOKING_DTL.Count() == 0)
                {
                    GMB_DTL.CSD_BOOKINGDTLID = 1;
                }
                else
                {
                    GMB_DTL.CSD_BOOKINGDTLID = _dbContext.CSD_GUESTMEALBOOKING_DTL.Max(x => x.CSD_BOOKINGDTLID) + 1;
                }

                GMB_DTL.GUESTDTLID = guestModel.BC_GUESTID;
                GMB_DTL.CSD_GUESTMEALBOOKINGID = model.CSD_GUEST_BOOKINGID;
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
                CSD_GUESTMEALBOOKING_TRN model = new CSD_GUESTMEALBOOKING_TRN();
                model = _dbContext.CSD_GUESTMEALBOOKING_TRN.Where(w => w.CSD_GUEST_BOOKINGID == id).FirstOrDefault();
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

        public short GuestMealApproval(CSDGuestMealBookingTrnViewModel GMBT)
        {
            short retVal = 0;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    CSD_GUESTMEALBOOKING_TRN _Approval = new CSD_GUESTMEALBOOKING_TRN();
                    if (GMBT.CSD_GUEST_BOOKINGID > 0)
                    {
                        _Approval = _dbContext.CSD_GUESTMEALBOOKING_TRN.FirstOrDefault(a => a.CSD_GUEST_BOOKINGID == GMBT.CSD_GUEST_BOOKINGID && a.APPROVE_STATUS == 0);
                        if (_Approval != null)
                        {
                            short IsCommit = 0;
                            if (_Approval.APPROVER_ECODE == GMBT.APPROVER_ECODE)
                            {
                                _Approval.APPROVE_STATUS = GMBT.APPROVE_STATUS;
                                if (GMBT.APPROVE_STATUS == 1) // Approved
                                {
                                    _Approval.MEAL_STATUS = 1;
                                    SendOTPOnMailAfterMealBookingApproval(GMBT.CSD_GUEST_BOOKINGID);// Added by Aumento :: SR82498
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

        public CSDGuestMealBookingTrnViewModel GetGuestRequestDtlById(long id)
        {
            //var obj = (from data in _dbContext.CSD_GUESTMEALBOOKING_TRN.Where(w => w.CSD_GUEST_BOOKINGID == id)
            //           join _ApproverAuth in _dbContext.ADEMPLOYEE on data.APPROVER_ECODE equals _ApproverAuth.ADEMPCODE into _ApproverAuthJoin
            //           from _Approver in _ApproverAuthJoin.DefaultIfEmpty()
            //           join _mealType in _dbContext.CSD_MEALTYPE_MST on data.MEALTYPEID equals _mealType.CSD_MEALTYPEID
            //           join _mealAvail in _dbContext.CSD_MEAL_AVAILABILITY on data.MEALS_AVAILABILITY_ID equals _mealAvail.CSD_MEAL_AVAILABILITY_ID
            //           join _mealMst in _dbContext.CSD_MEALMST on _mealAvail.MEALS_ID equals _mealMst.CSD_MEALID
            //           join _AddBy in _dbContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
            //           join _VWAssociate in _dbContext.VW_ASSOCIATELVLDETAILS on data.ADDEDBY equals _VWAssociate.ADEMPCODE
            //           join _optionMst in _dbContext.CSD_MEALOPTION_MST on data.MEALOPTIONID equals _optionMst.CSDMEAL_OPTIONID
            //           //join _slotMst in _dbContext.BC_MEALSLOTMST on data.SLOTID equals _slotMst.BCSLOTMSTID
            //           where _VWAssociate.SYKI == _Syki.SYKIID
            //           select new CSDGuestMealBookingTrnViewModel
            //           {
            //               CSD_GUEST_BOOKINGID = data.CSD_GUEST_BOOKINGID,
            //               MEALTYPEID = data.MEALTYPEID,
            //               MealType = new CSDMealTypeViewModel
            //               {
            //                   CSD_MEALTYPEID = _mealType.CSD_MEALTYPEID,
            //                   MEAL_TYPE_DESC = _mealType.MEAL_TYPE_DESC,
            //                   MEAL_QTY = _mealType.MEAL_QTY
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
            //               VISIT_PURPOSE = data.VISIT_PURPOSE,
            //               GuestDtlList = (from _guestData in _dbContext.CSD_GUESTMEALBOOKING_DTL.Where(d => d.CSD_GUESTMEALBOOKINGID == data.CSD_GUEST_BOOKINGID)
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
            //               MealMst = new CSDMealMstViewModel
            //               {
            //                   CSD_MEALID = _mealMst.CSD_MEALID,
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
            //               MEALOPTIONID = _optionMst.CSDMEAL_OPTIONID,
            //               MEALOPTION = _optionMst.MEAL_OPTION,
            //               //SLOTMSTID = data.SLOTID,
            //               //SLOT_TIME = _slotMst.SLOT_TIME
            //           }).FirstOrDefault();


            var masterData = (from data1 in _dbContext.CSD_GUESTMEALBOOKING_TRN.Where(w => w.CSD_GUEST_BOOKINGID == id)
                              join _ApproverAuth1 in _dbContext.ADEMPLOYEE on data1.APPROVER_ECODE equals _ApproverAuth1.ADEMPCODE into _ApproverAuthJoin
                              from _Approver1 in _ApproverAuthJoin.DefaultIfEmpty()
                              join _mealType1 in _dbContext.CSD_MEALTYPE_MST on data1.MEALTYPEID equals _mealType1.CSD_MEALTYPEID
                              join _mealAvail1 in _dbContext.CSD_MEAL_AVAILABILITY on data1.MEALS_AVAILABILITY_ID equals _mealAvail1.CSD_MEAL_AVAILABILITY_ID
                              join _mealMst1 in _dbContext.CSD_MEALMST on _mealAvail1.MEALS_ID equals _mealMst1.CSD_MEALID
                              join _AddBy1 in _dbContext.ADEMPLOYEE on data1.ADDEDBY equals _AddBy1.ADEMPCODE
                              join _VWAssociate1 in _dbContext.VW_ASSOCIATELVLDETAILS on data1.ADDEDBY equals _VWAssociate1.ADEMPCODE
                              join _optionMst1 in _dbContext.CSD_MEALOPTION_MST on data1.MEALOPTIONID equals _optionMst1.CSDMEAL_OPTIONID
                              //join _slotMst in _dbContext.BC_MEALSLOTMST on data.SLOTID equals _slotMst.BCSLOTMSTID
                              where _VWAssociate1.SYKI == _Syki.SYKIID
                              select new
                              {
                                  data = data1,
                                  _Approver = _Approver1,
                                  _mealType = _mealType1,
                                  _mealAvail = _mealAvail1,
                                  _mealMst = _mealMst1,
                                  _AddBy = _AddBy1,
                                  _VWAssociate = _VWAssociate1,
                                  _optionMst = _optionMst1
                              }).FirstOrDefault();
            if (masterData == null) return null;

            var GuestDtlList = (from _guestData in _dbContext.CSD_GUESTMEALBOOKING_DTL.Where(d => d.CSD_GUESTMEALBOOKINGID == masterData.data.CSD_GUEST_BOOKINGID)
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

            var result = new CSDGuestMealBookingTrnViewModel
            {
                CSD_GUEST_BOOKINGID = masterData.data.CSD_GUEST_BOOKINGID,
                MEALTYPEID = masterData.data.MEALTYPEID,
                MealType = new CSDMealTypeViewModel
                {
                    CSD_MEALTYPEID = masterData._mealType.CSD_MEALTYPEID,
                    MEAL_TYPE_DESC = masterData._mealType.MEAL_TYPE_DESC,
                    MEAL_QTY = masterData._mealType.MEAL_QTY
                },
                PAYMENT_TYPE = masterData.data.PAYMENT_TYPE,
                MEAL_BOOKED_DATE = masterData.data.MEAL_BOOKED_DATE,
                MEAL_STATUS = masterData.data.MEAL_STATUS,
                MEALS_AVAILABILITY_ID = masterData.data.MEALS_AVAILABILITY_ID,
                APPROVER_ECODE = masterData.data.APPROVER_ECODE,
                APPROVER_EMAIL = (masterData._Approver == null ? "" : masterData._Approver.EMAILID),
                APPROVER_FNAME = (masterData._Approver == null ? "" : masterData._Approver.FIRSTNAME),
                APPROVER_LNAME = (masterData._Approver == null ? "" : masterData._Approver.LASTNAME),
                APPROVE_STATUS = masterData._Approver == null ? (short)-1 : masterData.data.APPROVE_STATUS,
                APPROVE_DATE = masterData.data.APPROVE_DATE,
                APPROVE_REMARK = masterData.data.APPROVE_REMARK,
                VISIT_PURPOSE = masterData.data.VISIT_PURPOSE,
                GuestDtlList = GuestDtlList,
                MealMst = new CSDMealMstViewModel
                {
                    CSD_MEALID = masterData._mealMst.CSD_MEALID,
                    MEALTYPEID = masterData._mealMst.MEALTYPEID,
                    MEAL_NAME = masterData._mealMst.MEAL_NAME,
                    MEAL_PRICE = masterData._mealMst.MEAL_PRICE,
                    MEAL_DESC = masterData._mealMst.MEAL_DESC,
                    MEAL_PHOTO = masterData._mealMst.MEAL_PHOTO,
                    MEAL_PHOTO_NAME = masterData._mealMst.MEAL_PHOTO_NAME,
                },
                Emp_Detail = new Employee_Details
                {
                    _ECode = masterData._AddBy.ADEMPCODE,
                    _EName = masterData._AddBy.FIRSTNAME + " " + masterData._AddBy.LASTNAME,
                    _EmailId = masterData._AddBy.EMAILID,
                    _DOB = DateTime.Now,
                    _SecDescrip = masterData._VWAssociate.SECTION,
                    _DepDesc = masterData._VWAssociate.DEPARTMENT,
                    _DivDesc = masterData._VWAssociate.DIVISION,
                    _OpDesc = masterData._VWAssociate.OPERATION,
                    _SiteId = masterData._VWAssociate.SYSITEID
                },
                ADDEDBY = masterData.data.ADDEDBY,
                ADDEDDATE = masterData.data.ADDEDDATE,
                MEALOPTIONID = masterData._optionMst.CSDMEAL_OPTIONID,
                MEALOPTION = masterData._optionMst.MEAL_OPTION,
                //SLOTMSTID = data.SLOTID,
                //SLOT_TIME = _slotMst.SLOT_TIME
            };



            //var obj = (from data in _dbContext.CSD_GUESTMEALBOOKING_TRN.Where(w => w.CSD_GUEST_BOOKINGID == id)
            //    join _ApproverAuth in _dbContext.ADEMPLOYEE on data.APPROVER_ECODE equals _ApproverAuth.ADEMPCODE into _ApproverAuthJoin
            //    from _Approver in _ApproverAuthJoin.DefaultIfEmpty()
            //    join _mealType in _dbContext.CSD_MEALTYPE_MST on data.MEALTYPEID equals _mealType.CSD_MEALTYPEID
            //    join _mealAvail in _dbContext.CSD_MEAL_AVAILABILITY on data.MEALS_AVAILABILITY_ID equals _mealAvail.CSD_MEAL_AVAILABILITY_ID
            //    join _mealMst in _dbContext.CSD_MEALMST on _mealAvail.MEALS_ID equals _mealMst.CSD_MEALID
            //    join _AddBy in _dbContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
            //    join _VWAssociate in _dbContext.VW_ASSOCIATELVLDETAILS on data.ADDEDBY equals _VWAssociate.ADEMPCODE
            //    join _optionMst in _dbContext.CSD_MEALOPTION_MST on data.MEALOPTIONID equals _optionMst.CSDMEAL_OPTIONID
            //    //join _slotMst in _dbContext.BC_MEALSLOTMST on data.SLOTID equals _slotMst.BCSLOTMSTID
            //    where _VWAssociate.SYKI == _Syki.SYKIID
            //    select new CSDGuestMealBookingTrnViewModel
            //    {
            //        CSD_GUEST_BOOKINGID = data.CSD_GUEST_BOOKINGID,
            //        MEALTYPEID = data.MEALTYPEID,
            //        MealType = new CSDMealTypeViewModel
            //        {
            //            CSD_MEALTYPEID = _mealType.CSD_MEALTYPEID,
            //            MEAL_TYPE_DESC = _mealType.MEAL_TYPE_DESC,
            //            MEAL_QTY = _mealType.MEAL_QTY
            //        },
            //        PAYMENT_TYPE = data.PAYMENT_TYPE,
            //        MEAL_BOOKED_DATE = data.MEAL_BOOKED_DATE,
            //        MEAL_STATUS = data.MEAL_STATUS,
            //        MEALS_AVAILABILITY_ID = data.MEALS_AVAILABILITY_ID,
            //        APPROVER_ECODE = data.APPROVER_ECODE,
            //        APPROVER_EMAIL = (_Approver == null ? "" : _Approver.EMAILID),
            //        APPROVER_FNAME = (_Approver == null ? "" : _Approver.FIRSTNAME),
            //        APPROVER_LNAME = (_Approver == null ? "" : _Approver.LASTNAME),
            //        APPROVE_STATUS = _Approver == null ? (short)-1 : data.APPROVE_STATUS,
            //        APPROVE_DATE = data.APPROVE_DATE,
            //        APPROVE_REMARK = data.APPROVE_REMARK,
            //        VISIT_PURPOSE = data.VISIT_PURPOSE,
            //        GuestDtlList = (from _guestData in _dbContext.CSD_GUESTMEALBOOKING_DTL.Where(d => d.CSD_GUESTMEALBOOKINGID == data.CSD_GUEST_BOOKINGID)
            //                        join _Guest in _dbContext.BC_GUEST_DTL on _guestData.GUESTDTLID equals _Guest.BC_GUESTID
            //                        select new GuestDtlViewModel
            //                        {
            //                            BC_GUESTID = _Guest.BC_GUESTID,
            //                            GUEST_NAME = _Guest.GUEST_NAME,
            //                            MOBILE_NUMBER = _Guest.MOBILE_NUMBER,
            //                            COMPANY = _Guest.COMPANY,
            //                            TOKEN_CODE = _guestData.TOKEN_CODE,
            //                            MEAL_PRICE = _guestData.MEAL_PRICE,
            //                            ADMIN_STATUS = _guestData.ADMIN_STAUS
            //                        }).ToList(),
            //        MealMst = new CSDMealMstViewModel
            //        {
            //            CSD_MEALID = _mealMst.CSD_MEALID,
            //            MEALTYPEID = _mealMst.MEALTYPEID,
            //            MEAL_NAME = _mealMst.MEAL_NAME,
            //            MEAL_PRICE = _mealMst.MEAL_PRICE,
            //            MEAL_DESC = _mealMst.MEAL_DESC,
            //            MEAL_PHOTO = _mealMst.MEAL_PHOTO,
            //            MEAL_PHOTO_NAME = _mealMst.MEAL_PHOTO_NAME,
            //        },
            //        Emp_Detail = new Employee_Details
            //        {
            //            _ECode = _AddBy.ADEMPCODE,
            //            _EName = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
            //            _EmailId = _AddBy.EMAILID,
            //            _DOB = DateTime.Now,
            //            _SecDescrip = _VWAssociate.SECTION,
            //            _DepDesc = _VWAssociate.DEPARTMENT,
            //            _DivDesc = _VWAssociate.DIVISION,
            //            _OpDesc = _VWAssociate.OPERATION,
            //            _SiteId = _VWAssociate.SYSITEID
            //        },
            //        ADDEDBY = data.ADDEDBY,
            //        ADDEDDATE = data.ADDEDDATE,
            //        MEALOPTIONID = _optionMst.CSDMEAL_OPTIONID,
            //        MEALOPTION = _optionMst.MEAL_OPTION,
            //        //SLOTMSTID = data.SLOTID,
            //        //SLOT_TIME = _slotMst.SLOT_TIME
            //    }).FirstOrDefault();


            return result;
        }

        public Tuple<long, short> GetGuestDtlByMno(string mNo, string date, int mealOption)
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
                var existCount = (from _guestDtl in _dbContext.CSD_GUESTMEALBOOKING_DTL.Where(g => g.GUESTDTLID == guestDtl.BC_GUESTID)
                                  join _trn in _dbContext.CSD_GUESTMEALBOOKING_TRN on _guestDtl.CSD_GUESTMEALBOOKINGID equals _trn.CSD_GUEST_BOOKINGID
                                  where _trn.MEAL_BOOKED_DATE == bookingDate && _trn.MEAL_STATUS != 2 && _trn.MEAL_STATUS != 3 && _trn.MEALOPTIONID == mealOption
                                  select _guestDtl).Count();
                if (existCount > 0)
                {
                    bookStatus = 1;
                }
                _tuple = new Tuple<long, short>(guestDtlId, bookStatus);
            }
            return _tuple;
        }

        public List<CSDBakeryItemViewModel> BakeryItem()
        {
            List<CSDBakeryItemViewModel> iList = new List<CSDBakeryItemViewModel>();
            iList = (from data in _dbContext.CSD_BAKERYITEM_MST
                     where data.STATUS == 1
                     select new CSDBakeryItemViewModel
                     {
                         CSD_ITEMID = data.CSD_ITEMID,
                         ITEM_NAME = data.ITEM_NAME,
                         ITEM_PRICE = data.ITEM_PRICE,
                         ITEM_DESC = data.ITEM_DESC
                     }).OrderBy(o => o.ITEM_NAME).ToList();
            return iList;
        }

        // Start :: Added by Aumento :: SR82498
        public void SendOTPOnMailAfterMealBookingApproval(long bookingId)
        {
            try
            {
                CSDGuestMealBookingTrnViewModel bookingDetails = GetGuestRequestDtlById(bookingId);

                if (bookingDetails == null)
                {
                    throw new ArgumentException("Invalid booking ID. No details found.");
                }


                string requestStatus = "Approved";


                Employee_Details requester = bookingDetails.Emp_Detail;
                if (requester == null)
                {
                    throw new ArgumentException("Invalid requester details.");
                }


                List<GuestDtlViewModel> guestDetailsList = bookingDetails.GuestDtlList ?? new List<GuestDtlViewModel>();


                string guestRows = "";
                if (guestDetailsList.Count > 0)
                {
                    foreach (var guest in guestDetailsList)
                    {
                        guestRows += "<tr><td>" + guest.GUEST_NAME + "</td>";
                        guestRows += "<td>" + guest.COMPANY + "</td>";
                        guestRows += "<td>" + guest.MOBILE_NUMBER + "</td>";
                        guestRows += "<td>" + guest.TOKEN_CODE + "</td></tr>";
                    }
                }



                string subject = $"Guest Meal Booking Approval Status - {requestStatus}";

                string body = $@"<table cellpadding='0' cellspacing='0' style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>
                             <tr bgcolor='#009933'>
                                 <td height='30'>&nbsp;<b><font color='#FFFFFF'>Guest Meal Booking Approval Status Approved - Emp Code ({requester._ECode})</font></b></td>
                             </tr>
                             <tr>
                                <td>
                     <table cellpadding='4' cellspacing='0' border='1' width='600px' style='border-collapse:collapse;'>
                             <tr>
                                 <td valign='top' colspan='4'>
                                 Dear San,<br/><br/>
                                 Your Guest Meal Booking request has been <b>Approved</b> by {bookingDetails.APPROVER_FNAME} San. The request details are as follows:
                                <br/><br/></td>
                             </tr>
                             <tr style='background - color:#f2f2f2; font-weight:bold; text-align:left;'>
                                 <td> GUEST NAME </td>
                                 <td> COMPANY </td>
                                 <td> MOBILE NUMBER </td>
                                <td> OTP </td>
                            </tr>
                                {guestRows}
                            <tr>
                              <td colspan = '4' style='padding-top:20px'> Note: This is a system - generated Email.Please do not reply.</td>
                           </tr>
                          </table>
                      </td>
                  </tr>
              </ table >";


                EmailCore email = new EmailCore
                {
                    MailFrom = "portal.admin@honda.hmsi.in",
                    MailTo = requester._EmailId,
                    MailSubject = subject,
                    MailBody = body
                };


                if (!email.Send())
                {
                    throw new ApplicationException("Email sending failed.");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while processing the request. Please try again later.", ex);
            }
        }
        // End :: Added by Aumento :: SR82498

        #region MealFeedback
        public short BookingFeedback(CanteenMealBookingViewModel model)
        {
            short retVal = 0;
            //try
            //{
                foreach (var point in model.FeedbackPoints)
                {
                    var existing = _dbContext.CSD_FEEDBACK_DETAILS_TRN
                        .FirstOrDefault(x => x.BOOKING_ID == model.BookingId && x.FEEDBACK_POINT_ID == point.FeedbackPointId && x.MEALTYPEID == model.MEALTYPEID && x.ADDEDBY == model.ADDEDBY);

                    if (existing != null)
                    {
                        existing.RATING = point.Rating;
                        existing.REMARK = point.Remark;
                        existing.UPDATEDDATE = DateTime.Now;
                    }
                    else
                    {
                        _dbContext.CSD_FEEDBACK_DETAILS_TRN.Add(new CSD_FEEDBACK_DETAILS_TRN
                        {
                            BOOKING_ID = model.BookingId,
                            MEALTYPEID = model.MEALTYPEID,
                            FEEDBACK_POINT_ID = point.FeedbackPointId,
                            RATING = point.Rating,
                            REMARK = point.Remark,
                            ADDEDBY = model.ADDEDBY,
                            ADDEDDATE = DateTime.Now,
                            UPDATEDDATE = DateTime.Now,
                        });
                    }
                }

                var mealFeedback = _dbContext.CSD_FEEDBACK_IMAGE.FirstOrDefault(x => x.BOOKING_ID == model.BookingId && x.ADDEDBY == model.ADDEDBY && x.MEALTYPEID == model.MEALTYPEID);

                if (!string.IsNullOrEmpty(model.EvidenceFilePath))
                {
                    if (mealFeedback != null)
                    {
                        // Update
                        mealFeedback.EVIDENCE_IMAGE_PATH = model.EvidenceFilePath;
                    }
                    else
                    {
                        // Insert
                        _dbContext.CSD_FEEDBACK_IMAGE.Add(new CSD_FEEDBACK_IMAGE
                        {
                            BOOKING_ID = model.BookingId,
                            MEALTYPEID = model.MEALTYPEID,
                            EVIDENCE_IMAGE_PATH = model.EvidenceFilePath,
                            ADDEDDATE = DateTime.Now,
                            ADDEDBY = model.ADDEDBY
                        });
                    }
                }
                _dbContext.SaveChanges();

                bool hasPoorRating = model.FeedbackPoints.Any(x => x.Rating < 4);

                if (hasPoorRating)
                {
                    SendFeedbackMailForMeal(model.BookingId, model.MEALTYPEID, model.ADDEDBY);
                }

                retVal = 1;
            //}
            //catch (Exception ex)
            //{
            //    retVal = -1;
            //}
            return retVal;
        }

        public List<FeedbackPointViewModel> GetBookingFeedback(long bookingId, long mealType, long userId)
        {
            var feedbackImage = _dbContext.CSD_FEEDBACK_IMAGE
        .Where(i => i.BOOKING_ID == bookingId
                 && i.MEALTYPEID == mealType
                 && i.ADDEDBY == userId)
        .Select(i => i.EVIDENCE_IMAGE_PATH)
        .FirstOrDefault();

            var points = _dbContext.CSD_FEEDBACK_POINTS_MST
                .Where(p => p.ISACTIVE == "Y")
                .Select(p => new FeedbackPointViewModel
                {
                    FeedbackPointId = p.FEEDBACK_POINT_ID,
                    PointName = p.POINT_NAME,
                    Rating = _dbContext.CSD_FEEDBACK_DETAILS_TRN
                                .Where(f => f.BOOKING_ID == bookingId && f.MEALTYPEID == mealType && f.FEEDBACK_POINT_ID == p.FEEDBACK_POINT_ID && f.ADDEDBY == userId)
                                .Select(f => f.RATING)
                                .FirstOrDefault(),
                    Remark = _dbContext.CSD_FEEDBACK_DETAILS_TRN
                                .Where(f => f.BOOKING_ID == bookingId && f.MEALTYPEID == mealType && f.FEEDBACK_POINT_ID == p.FEEDBACK_POINT_ID && f.ADDEDBY == userId)
                                .Select(f => f.REMARK)
                                .FirstOrDefault(),
                    EvidenceImagePath = feedbackImage
                })
                .ToList();

            return points;
        }

        public CSD_MEALBOOKING_TRN GetAllBookingFeedback(long bookingId)
        {
            CSD_MEALBOOKING_TRN model = new CSD_MEALBOOKING_TRN();
            model = _dbContext.CSD_MEALBOOKING_TRN.Where(w => w.CSD_MEALSBOOKING_ID == bookingId).FirstOrDefault();

            return model;
        }

        public MealBookingFeedbackVM GetBookingFeedback(long bookingId)
        {
            var data = (from booking in _dbContext.CSD_MEALBOOKING_TRN
                        join mealType in _dbContext.CSD_MEALTYPE_MST
                            on booking.MEALTYPEID equals mealType.CSD_MEALTYPEID
                        where booking.CSD_MEALSBOOKING_ID == bookingId
                        select new MealBookingFeedbackVM
                        {
                            Booking = booking,
                            MealTypeDesc = mealType.MEAL_TYPE_DESC
                        }).FirstOrDefault();

            return data;
        }

        public List<CanteenMealBookingViewModel> GetBookingFeedbackHistory(long mealType, string fromDate, string toDate, long loginUser, long plantId)
        {
            DateTime currentDate = DateTime.Now;
            string cancellationTime = "23:59:00";
            int MealBookDay = 0;

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

            if (mealType == 1)
            {
                var mealTypeList = (from data in _dbContext.CSD_MEALTYPE_MST.Where(w => w.CSD_MEALTYPEID == mealType)
                                    join _canteenMap in _dbContext.CSD_CANTEENMEALTYPE_MAP on data.CSD_MEALTYPEID equals _canteenMap.MEALTYPE_ID
                                    join _canteen in _dbContext.CSD_CANTEEN_MST on _canteenMap.CANTEENID equals _canteen.CSD_CANTEENID
                                    select new
                                    {
                                        CSD_MEALTYPEID = data.CSD_MEALTYPEID,
                                        PLANTID = _canteen.PLANTID
                                    }).Distinct();

                List<CanteenMealBookingViewModel> availabilityList = (from _mealAvail in _dbContext.CSD_MEAL_AVAILABILITY.Where(w => (!string.IsNullOrEmpty(fromDate) ? (w.MEAL_MAPPING_DATE >= bookingFromDate) : true) && (!string.IsNullOrEmpty(toDate) ? (w.MEAL_MAPPING_DATE <= bookingToDate) : true))
                                                                      join _mealMst in _dbContext.CSD_MEALMST on _mealAvail.MEALS_ID equals _mealMst.CSD_MEALID
                                                                      join _mealType in _dbContext.CSD_MEALTYPE_MST on _mealMst.MEALTYPEID equals _mealType.CSD_MEALTYPEID
                                                                      join _optionMst in _dbContext.CSD_MEALOPTION_MST on _mealAvail.MEAL_OPTIONID equals _optionMst.CSDMEAL_OPTIONID
                                                                      join _mealPlant in mealTypeList on _mealType.CSD_MEALTYPEID equals _mealPlant.CSD_MEALTYPEID

                                                                      where _mealMst.MEALTYPEID == mealType
                                                                      && _mealMst.STATUS == 1 && _mealAvail.STATUS == 1
                                                                      && (plantId == 5 ? _mealPlant.PLANTID == 1 : _mealPlant.PLANTID == plantId)
                                                                      select new CanteenMealBookingViewModel
                                                                      {
                                                                          CSD_MEALSBOOKING_ID = _mealAvail.CSD_MEAL_AVAILABILITY_ID,
                                                                          MEALTYPEID = _mealMst.MEALTYPEID,
                                                                          MEAL_BOOKED_DATE = _mealAvail.MEAL_MAPPING_DATE,

                                                                          MealType = new CSDMealTypeViewModel
                                                                          {
                                                                              CSD_MEALTYPEID = _mealType.CSD_MEALTYPEID,
                                                                              MEAL_TYPE_DESC = _mealType.MEAL_TYPE_DESC,
                                                                              MEAL_QTY = _mealType.MEAL_QTY
                                                                          },
                                                                          MealMst = new CSDMealMstViewModel
                                                                          {
                                                                              CSD_MEALID = _mealMst.CSD_MEALID,
                                                                              MEALTYPEID = _mealMst.MEALTYPEID,
                                                                              MEAL_NAME = _mealMst.MEAL_NAME,
                                                                              MEAL_PRICE = _mealMst.MEAL_PRICE,
                                                                              MEAL_DESC = _mealMst.MEAL_DESC,
                                                                              MEAL_PHOTO = _mealMst.MEAL_PHOTO,
                                                                              MEAL_PHOTO_NAME = _mealMst.MEAL_PHOTO_NAME,
                                                                          },

                                                                          MEALOPTIONID = _optionMst.CSDMEAL_OPTIONID,
                                                                          MEALOPTION = _optionMst.MEAL_OPTION,
                                                                      }).ToList();

                if (mealType == 1)
                {
                    var dietMealList = GetBookingFeedbackHistory(2, fromDate, toDate, loginUser, plantId);

                    availabilityList = availabilityList
                        .Where(r => !dietMealList.Any(d =>
                            d.MEAL_BOOKED_DATE == r.MEAL_BOOKED_DATE &&
                            d.MEALOPTIONID == r.MEALOPTIONID))
                        .ToList();
                }
                return availabilityList.OrderByDescending(o => o.MEAL_BOOKED_DATE).ThenByDescending(t => t.CSD_MEALSBOOKING_ID).ToList();
            }
            else
            {
                var iList = (from data in _dbContext.CSD_MEALBOOKING_TRN.Where(w => w.ADDEDBY == loginUser)
                             join _mealType in _dbContext.CSD_MEALTYPE_MST on data.MEALTYPEID equals _mealType.CSD_MEALTYPEID
                             join _mealAvail in _dbContext.CSD_MEAL_AVAILABILITY on data.MEAL_AVAILABILITY_ID equals _mealAvail.CSD_MEAL_AVAILABILITY_ID
                             join _mealMst in _dbContext.CSD_MEALMST on _mealAvail.MEALS_ID equals _mealMst.CSD_MEALID
                             join _mealOption in _dbContext.CSD_MEALOPTION_MST on data.MEALOPTIONID equals _mealOption.CSDMEAL_OPTIONID
                             where (!string.IsNullOrEmpty(fromDate) ? (data.MEAL_BOOKED_DATE >= bookingFromDate) : true)
                                  && (!string.IsNullOrEmpty(toDate) ? (data.MEAL_BOOKED_DATE <= bookingToDate) : true)
                                  && (mealType == 0 ? true : data.MEALTYPEID == mealType)
                             select new CanteenMealBookingViewModel
                             {
                                 CSD_MEALSBOOKING_ID = data.CSD_MEALSBOOKING_ID,
                                 MEALTYPEID = data.MEALTYPEID,
                                 MealType = new CSDMealTypeViewModel
                                 {
                                     CSD_MEALTYPEID = _mealType.CSD_MEALTYPEID,
                                     MEAL_TYPE_DESC = _mealType.MEAL_TYPE_DESC,
                                     MEAL_QTY = _mealType.MEAL_QTY
                                 },
                                 MEAL_BOOKED_DATE = data.MEAL_BOOKED_DATE,
                                 MEAL_STATUS = data.MEAL_STATUS,
                                 MEALS_AVAILABILITY_ID = data.MEAL_AVAILABILITY_ID,
                                 MealMst = new CSDMealMstViewModel
                                 {
                                     CSD_MEALID = _mealMst.CSD_MEALID,
                                     MEALTYPEID = _mealMst.MEALTYPEID,
                                     MEAL_NAME = _mealMst.MEAL_NAME,
                                     MEAL_PRICE = _mealMst.MEAL_PRICE,
                                     MEAL_DESC = _mealMst.MEAL_DESC,
                                     MEAL_PHOTO = _mealMst.MEAL_PHOTO,
                                     MEAL_PHOTO_NAME = _mealMst.MEAL_PHOTO_NAME,
                                 },
                                 ADDEDBY = data.ADDEDBY,
                                 ADDEDDATE = data.ADDEDDATE,
                                 MEALOPTIONID = _mealOption.CSDMEAL_OPTIONID,
                                 MEALOPTION = _mealOption.MEAL_OPTION,
                             }).ToList();

                // ⭐⭐ APPLY "ALL MEALS" LOGIC ⭐⭐
                if (mealType == 0)
                {
                    var dietMeals = iList.Where(x => x.MEALTYPEID == 2).ToList();
                    var regularMeals = iList.Where(x => x.MEALTYPEID == 1).ToList();

                    var filteredRegularMeals = regularMeals
                        .Where(r => !dietMeals.Any(d =>
                            d.MEAL_BOOKED_DATE.Date == r.MEAL_BOOKED_DATE.Date))
                        .ToList();

                    iList = new List<CanteenMealBookingViewModel>();
                    iList.AddRange(dietMeals);
                    iList.AddRange(filteredRegularMeals);
                }

                // keep your original cancellation calculation
                List<CanteenMealBookingViewModel> newList = new List<CanteenMealBookingViewModel>();
                foreach (var obj in iList)
                {
                    if (TIME_DTL_LIST.Count > 0)
                    {
                        string CANCELLATION_TIME = GetColValue(obj.MEALTYPEID, obj.MEALOPTIONID, "CANCELLATION_TIME");
                        cancellationTime = !string.IsNullOrEmpty(CANCELLATION_TIME) ? CANCELLATION_TIME : cancellationTime;

                        string CANCELLATION_ALLOW_DAYS = GetColValue(obj.MEALTYPEID, obj.MEALOPTIONID, "CANCELLATION_DAY");
                        MealBookDay = !string.IsNullOrEmpty(CANCELLATION_ALLOW_DAYS) ? Convert.ToInt32(CANCELLATION_ALLOW_DAYS) : MealBookDay;
                    }

                    DateTime validCancelDate =
                        DateTime.ParseExact(obj.MEAL_BOOKED_DATE.ToString("dd-MMM-yyyy") + " " + cancellationTime,
                                            "dd-MMM-yyyy HH:mm:ss", null)
                        .AddDays(-MealBookDay);

                    obj.IsEnableCancel = DateTime.Now < validCancelDate;
                    newList.Add(obj);
                }

                return newList.OrderByDescending(o => o.MEAL_BOOKED_DATE)
                              .ThenByDescending(t => t.CSD_MEALSBOOKING_ID)
                              .ToList();
            }
        }

        public List<CanteenMealBookingViewModel> GetAllBookingFeedbackLst(long mealType, string fromDate, string toDate, long loginUser, long plantId, long mealRating = 5)
        {
            DateTime currentDate = DateTime.Now;
            string cancellationTime = "23:59:00";
            int MealBookDay = 0;

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

            List<CanteenMealBookingViewModel> dietMealsList = new List<CanteenMealBookingViewModel>();
            List<CanteenMealBookingViewModel> regularMeals = new List<CanteenMealBookingViewModel>();
            if (mealType == 1 || mealType == 0)
            {
                var mealTypeList =
                (
                    from data in _dbContext.CSD_MEALTYPE_MST
                    where data.CSD_MEALTYPEID == 1
                    join map in _dbContext.CSD_CANTEENMEALTYPE_MAP
                        on data.CSD_MEALTYPEID equals map.MEALTYPE_ID
                    join canteen in _dbContext.CSD_CANTEEN_MST
                        on map.CANTEENID equals canteen.CSD_CANTEENID
                    select new
                    {
                        data.CSD_MEALTYPEID,
                        canteen.PLANTID
                    }
                ).Distinct();

                regularMeals =
                (
                    from avail in _dbContext.CSD_MEAL_AVAILABILITY
                        .Where(w =>
                            (!string.IsNullOrEmpty(fromDate) ? w.MEAL_MAPPING_DATE >= bookingFromDate : true) &&
                            (!string.IsNullOrEmpty(toDate) ? w.MEAL_MAPPING_DATE <= bookingToDate : true))

                    join meal in _dbContext.CSD_MEALMST
                        on avail.MEALS_ID equals meal.CSD_MEALID

                    join mealTypeMst in _dbContext.CSD_MEALTYPE_MST
                        on meal.MEALTYPEID equals mealTypeMst.CSD_MEALTYPEID

                    join option in _dbContext.CSD_MEALOPTION_MST
                        on avail.MEAL_OPTIONID equals option.CSDMEAL_OPTIONID

                    join plant in mealTypeList
                        on mealTypeMst.CSD_MEALTYPEID equals plant.CSD_MEALTYPEID

                    // ✅ FIX: GROUPED FEEDBACK JOIN
                    join feedback in
                        (
                            from f in _dbContext.CSD_FEEDBACK_DETAILS_TRN
                            group f by new { f.BOOKING_ID, f.ADDEDBY, f.MEALTYPEID } into g
                            select new
                            {
                                BOOKING_ID = g.Key.BOOKING_ID,
                                ADDEDBY = g.Key.ADDEDBY,
                                MEALTYPEID = g.Key.MEALTYPEID,
                                RATING = g.Max(x => x.RATING),
                                REMARK = g.Select(x => x.REMARK).FirstOrDefault()
                            }
                        )
                    on avail.CSD_MEAL_AVAILABILITY_ID equals feedback.BOOKING_ID

                    where meal.MEALTYPEID == 1
                          && feedback.MEALTYPEID == 1
                          && meal.STATUS == 1
                          && avail.STATUS == 1
                          && (plantId == 5 ? plant.PLANTID == 1 : plant.PLANTID == plantId)

                    select new CanteenMealBookingViewModel
                    {
                        CSD_MEALSBOOKING_ID = avail.CSD_MEAL_AVAILABILITY_ID,
                        MEALTYPEID = meal.MEALTYPEID,
                        MEAL_BOOKED_DATE = avail.MEAL_MAPPING_DATE,

                        // ✅ ONE ROW PER USER
                        ADDEDBY = feedback.ADDEDBY,
                        FEEDBACK_RATING = feedback.RATING,
                        FEEDBACK_REMARK = feedback.REMARK,

                        MealType = new CSDMealTypeViewModel
                        {
                            CSD_MEALTYPEID = mealTypeMst.CSD_MEALTYPEID,
                            MEAL_TYPE_DESC = mealTypeMst.MEAL_TYPE_DESC,
                            MEAL_QTY = mealTypeMst.MEAL_QTY
                        },

                        MealMst = new CSDMealMstViewModel
                        {
                            CSD_MEALID = meal.CSD_MEALID,
                            MEALTYPEID = meal.MEALTYPEID,
                            MEAL_NAME = meal.MEAL_NAME,
                            MEAL_PRICE = meal.MEAL_PRICE,
                            MEAL_DESC = meal.MEAL_DESC,
                            MEAL_PHOTO = meal.MEAL_PHOTO,
                            MEAL_PHOTO_NAME = meal.MEAL_PHOTO_NAME
                        },

                        MEALOPTIONID = option.CSDMEAL_OPTIONID,
                        MEALOPTION = option.MEAL_OPTION
                    }
                ).ToList();
            }
            if (mealType != 1)
            {
                var iList = (
                            from data in _dbContext.CSD_MEALBOOKING_TRN
                            join _mealType in _dbContext.CSD_MEALTYPE_MST
                                on data.MEALTYPEID equals _mealType.CSD_MEALTYPEID
                            join _mealAvail in _dbContext.CSD_MEAL_AVAILABILITY
                                on data.MEAL_AVAILABILITY_ID equals _mealAvail.CSD_MEAL_AVAILABILITY_ID
                            join _mealMst in _dbContext.CSD_MEALMST
                                on _mealAvail.MEALS_ID equals _mealMst.CSD_MEALID
                            join _mealOption in _dbContext.CSD_MEALOPTION_MST
                                on data.MEALOPTIONID equals _mealOption.CSDMEAL_OPTIONID

                            where
                                (!string.IsNullOrEmpty(fromDate) ? data.MEAL_BOOKED_DATE >= bookingFromDate : true)
                                && (!string.IsNullOrEmpty(toDate) ? data.MEAL_BOOKED_DATE <= bookingToDate : true)

                                // ✔ MealType filter (0 = all except 1)
                                && (mealType == 0
                                        ? data.MEALTYPEID != 1
                                        : data.MEALTYPEID == mealType)

                                // ✔ Feedback filter (0 = feedback for all except 1)
                                && (mealType == 0
                                        ? _dbContext.CSD_FEEDBACK_DETAILS_TRN.Any(f =>
                                              f.BOOKING_ID == data.CSD_MEALSBOOKING_ID &&
                                              f.MEALTYPEID != 1)
                                        : _dbContext.CSD_FEEDBACK_DETAILS_TRN.Any(f =>
                                              f.BOOKING_ID == data.CSD_MEALSBOOKING_ID &&
                                              f.MEALTYPEID == mealType))

                            select new CanteenMealBookingViewModel
                            {
                                CSD_MEALSBOOKING_ID = data.CSD_MEALSBOOKING_ID,
                                MEALTYPEID = data.MEALTYPEID,
                                MealType = new CSDMealTypeViewModel
                                {
                                    CSD_MEALTYPEID = _mealType.CSD_MEALTYPEID,
                                    MEAL_TYPE_DESC = _mealType.MEAL_TYPE_DESC,
                                    MEAL_QTY = _mealType.MEAL_QTY
                                },
                                MEAL_BOOKED_DATE = data.MEAL_BOOKED_DATE,
                                MEAL_STATUS = data.MEAL_STATUS,
                                MEALS_AVAILABILITY_ID = data.MEAL_AVAILABILITY_ID,
                                MealMst = new CSDMealMstViewModel
                                {
                                    CSD_MEALID = _mealMst.CSD_MEALID,
                                    MEALTYPEID = _mealMst.MEALTYPEID,
                                    MEAL_NAME = _mealMst.MEAL_NAME,
                                    MEAL_PRICE = _mealMst.MEAL_PRICE,
                                    MEAL_DESC = _mealMst.MEAL_DESC,
                                    MEAL_PHOTO = _mealMst.MEAL_PHOTO,
                                    MEAL_PHOTO_NAME = _mealMst.MEAL_PHOTO_NAME,
                                },
                                ADDEDBY = data.ADDEDBY,
                                ADDEDDATE = data.ADDEDDATE,
                                MEALOPTIONID = _mealOption.CSDMEAL_OPTIONID,
                                MEALOPTION = _mealOption.MEAL_OPTION
                            }
                        ).ToList();


                // cancellation logic (same as your original)
                foreach (var obj in iList)
                {
                    if (TIME_DTL_LIST.Count > 0)
                    {
                        string CANCELLATION_TIME = GetColValue(obj.MEALTYPEID, obj.MEALOPTIONID, "CANCELLATION_TIME");
                        cancellationTime = !string.IsNullOrEmpty(CANCELLATION_TIME) ? CANCELLATION_TIME : cancellationTime;

                        string CANCELLATION_ALLOW_DAYS = GetColValue(obj.MEALTYPEID, obj.MEALOPTIONID, "CANCELLATION_DAY");
                        MealBookDay = !string.IsNullOrEmpty(CANCELLATION_ALLOW_DAYS) ? Convert.ToInt32(CANCELLATION_ALLOW_DAYS) : MealBookDay;
                    }

                    DateTime validCancelDate = DateTime.ParseExact(
                        obj.MEAL_BOOKED_DATE.ToString("dd-MMM-yyyy") + " " + cancellationTime,
                        "dd-MMM-yyyy HH:mm:ss",
                        null
                    ).AddDays(-MealBookDay);

                    obj.IsEnableCancel = currentDate < validCancelDate;
                    dietMealsList.Add(obj);
                }
            }

            if (mealType == 1)
            {
                return regularMeals
                    .OrderByDescending(o => o.MEAL_BOOKED_DATE)
                    .ThenByDescending(t => t.CSD_MEALSBOOKING_ID)
                    .ToList();
            }
            if (mealType != 1 && mealType != 0)
            {
                return dietMealsList
                    .OrderByDescending(o => o.MEAL_BOOKED_DATE)
                    .ThenByDescending(t => t.CSD_MEALSBOOKING_ID)
                    .ToList();
            }

            var finalList = regularMeals.Concat(dietMealsList)
               .OrderByDescending(o => o.MEAL_BOOKED_DATE)
               .ThenByDescending(t => t.CSD_MEALSBOOKING_ID)
               .ToList();

            return finalList;
        }

        public List<MealBookingExcelVM> GetMealFeedbackExcelData(long mealType, string fromDate, string toDate, long plantId, long mealRating = 5)
        {
            DateTime currentDate = DateTime.Now;
            string cancellationTime = "23:59:00";
            int MealBookDay = 0;

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

            List<MealBookingExcelVM> dietList = new List<MealBookingExcelVM>();
            List<MealBookingExcelVM> regularMeals = new List<MealBookingExcelVM>();

            if (mealType == 1 || mealType == 0)
            {
                var mealTypeList = (
                    from data in _dbContext.CSD_MEALTYPE_MST
                    where data.CSD_MEALTYPEID == 1
                    join _canteenMap in _dbContext.CSD_CANTEENMEALTYPE_MAP
                        on data.CSD_MEALTYPEID equals _canteenMap.MEALTYPE_ID
                    join _canteen in _dbContext.CSD_CANTEEN_MST
                        on _canteenMap.CANTEENID equals _canteen.CSD_CANTEENID
                    select new
                    {
                        CSD_MEALTYPEID = data.CSD_MEALTYPEID,
                        PLANTID = _canteen.PLANTID
                    }
                ).Distinct();

                regularMeals = (
                    from _mealAvail in _dbContext.CSD_MEAL_AVAILABILITY
                        .Where(w =>
                            (!string.IsNullOrEmpty(fromDate)
                                ? (w.MEAL_MAPPING_DATE >= bookingFromDate)
                                : true)
                            && (!string.IsNullOrEmpty(toDate)
                                ? (w.MEAL_MAPPING_DATE <= bookingToDate)
                                : true))

                    join _mealMst in _dbContext.CSD_MEALMST
                        on _mealAvail.MEALS_ID equals _mealMst.CSD_MEALID

                    join _mealType in _dbContext.CSD_MEALTYPE_MST
                        on _mealMst.MEALTYPEID equals _mealType.CSD_MEALTYPEID

                    join _optionMst in _dbContext.CSD_MEALOPTION_MST
                        on _mealAvail.MEAL_OPTIONID equals _optionMst.CSDMEAL_OPTIONID

                    join _mealPlant in mealTypeList
                        on _mealType.CSD_MEALTYPEID equals _mealPlant.CSD_MEALTYPEID

                    // ✅ Only bookings which have feedback
                    where _dbContext.CSD_FEEDBACK_DETAILS_TRN
                            .Any(f => f.BOOKING_ID == _mealAvail.CSD_MEAL_AVAILABILITY_ID
                                   && f.MEALTYPEID == 1)
                        && _mealMst.MEALTYPEID == 1
                        && _mealMst.STATUS == 1
                        && _mealAvail.STATUS == 1
                        && (plantId == 5 ? _mealPlant.PLANTID == 1 : _mealPlant.PLANTID == plantId)

                    select new MealBookingExcelVM
                    {
                        CSD_MEALSBOOKING_ID = _mealAvail.CSD_MEAL_AVAILABILITY_ID,
                        MEALTYPEID = _mealMst.MEALTYPEID,
                        MEAL_BOOKED_DATE = _mealAvail.MEAL_MAPPING_DATE,
                        // ✅ ONLY FIRST ADDEDBY (NO DUPLICATES)
                        ADDEDBY = _dbContext.CSD_FEEDBACK_DETAILS_TRN
                     .Where(f => f.BOOKING_ID == _mealAvail.CSD_MEAL_AVAILABILITY_ID)
                     .Select(f => (long)f.ADDEDBY)
                     .FirstOrDefault(),

                        EmpName = _dbContext.ADEMPLOYEE
            .Where(e => e.ADEMPCODE ==
                _dbContext.CSD_FEEDBACK_DETAILS_TRN
                    .Where(f => f.BOOKING_ID == _mealAvail.CSD_MEAL_AVAILABILITY_ID)
                    .Select(f => (long?)f.ADDEDBY)
                    .FirstOrDefault()
            )
            .Select(e => e.FIRSTNAME + " " + e.LASTNAME)
            .FirstOrDefault(),

                        MealType = new CSDMealTypeViewModel
                        {
                            CSD_MEALTYPEID = _mealType.CSD_MEALTYPEID,
                            MEAL_TYPE_DESC = _mealType.MEAL_TYPE_DESC,
                            MEAL_QTY = _mealType.MEAL_QTY
                        },

                        MealMst = new CSDMealMstViewModel
                        {
                            CSD_MEALID = _mealMst.CSD_MEALID,
                            MEALTYPEID = _mealMst.MEALTYPEID,
                            MEAL_NAME = _mealMst.MEAL_NAME,
                            MEAL_PRICE = _mealMst.MEAL_PRICE,
                            MEAL_DESC = _mealMst.MEAL_DESC,
                            MEAL_PHOTO = _mealMst.MEAL_PHOTO,
                            MEAL_PHOTO_NAME = _mealMst.MEAL_PHOTO_NAME
                        },

                        MEALOPTIONID = _optionMst.CSDMEAL_OPTIONID,
                        MEALOPTION = _optionMst.MEAL_OPTION
                    }
                ).ToList();

                foreach (var item in regularMeals)
                {
                    item.FeedbackPoints = GetBookingFeedback(
                        item.CSD_MEALSBOOKING_ID,   // bookingId
                        item.MEALTYPEID,            // mealType
                        item.ADDEDBY                // userId
                    );
                }
            }
            if (mealType != 1)
            {
                var iList = (
                    from data in _dbContext.CSD_MEALBOOKING_TRN
                        //where data.ADDEDBY == loginUser
                    join _mealType in _dbContext.CSD_MEALTYPE_MST
                        on data.MEALTYPEID equals _mealType.CSD_MEALTYPEID
                    join _mealAvail in _dbContext.CSD_MEAL_AVAILABILITY
                        on data.MEAL_AVAILABILITY_ID equals _mealAvail.CSD_MEAL_AVAILABILITY_ID
                    join _mealMst in _dbContext.CSD_MEALMST
                        on _mealAvail.MEALS_ID equals _mealMst.CSD_MEALID
                    join _mealOption in _dbContext.CSD_MEALOPTION_MST
                        on data.MEALOPTIONID equals _mealOption.CSDMEAL_OPTIONID

                    join emp in _dbContext.ADEMPLOYEE
                        on data.ADDEDBY equals emp.ADEMPCODE into empJoin
                    from emp in empJoin.DefaultIfEmpty()

                    where (!string.IsNullOrEmpty(fromDate) ? (data.MEAL_BOOKED_DATE >= bookingFromDate) : true)
                          && (!string.IsNullOrEmpty(toDate) ? (data.MEAL_BOOKED_DATE <= bookingToDate) : true)

                                // ✔ MealType filter (0 = all except 1)
                                && (mealType == 0
                                        ? data.MEALTYPEID != 1
                                        : data.MEALTYPEID == mealType)

                                // ✔ Feedback filter (0 = feedback for all except 1)
                                && (mealType == 0
                                        ? _dbContext.CSD_FEEDBACK_DETAILS_TRN.Any(f =>
                                              f.BOOKING_ID == data.CSD_MEALSBOOKING_ID &&
                                              f.MEALTYPEID != 1)
                                        : _dbContext.CSD_FEEDBACK_DETAILS_TRN.Any(f =>
                                              f.BOOKING_ID == data.CSD_MEALSBOOKING_ID &&
                                              f.MEALTYPEID == mealType))


                    select new MealBookingExcelVM
                    {
                        CSD_MEALSBOOKING_ID = data.CSD_MEALSBOOKING_ID,
                        MEALTYPEID = data.MEALTYPEID,

                        ADDEDBY = _dbContext.CSD_FEEDBACK_DETAILS_TRN
                     .Where(f => f.BOOKING_ID == data.CSD_MEALSBOOKING_ID)
                     .Select(f => (long)f.ADDEDBY)
                     .FirstOrDefault(),

                        EmpName = _dbContext.ADEMPLOYEE
            .Where(e => e.ADEMPCODE ==
                _dbContext.CSD_FEEDBACK_DETAILS_TRN
                    .Where(f => f.BOOKING_ID == data.CSD_MEALSBOOKING_ID)
                    .Select(f => (long?)f.ADDEDBY)
                    .FirstOrDefault()
            )
            .Select(e => e.FIRSTNAME + " " + e.LASTNAME)
            .FirstOrDefault(),
                        MealType = new CSDMealTypeViewModel
                        {
                            CSD_MEALTYPEID = _mealType.CSD_MEALTYPEID,
                            MEAL_TYPE_DESC = _mealType.MEAL_TYPE_DESC,
                            MEAL_QTY = _mealType.MEAL_QTY
                        },
                        MEAL_BOOKED_DATE = data.MEAL_BOOKED_DATE,
                        MEAL_STATUS = data.MEAL_STATUS,
                        MEALS_AVAILABILITY_ID = data.MEAL_AVAILABILITY_ID,
                        MealMst = new CSDMealMstViewModel
                        {
                            CSD_MEALID = _mealMst.CSD_MEALID,
                            MEALTYPEID = _mealMst.MEALTYPEID,
                            MEAL_NAME = _mealMst.MEAL_NAME,
                            MEAL_PRICE = _mealMst.MEAL_PRICE,
                            MEAL_DESC = _mealMst.MEAL_DESC,
                            MEAL_PHOTO = _mealMst.MEAL_PHOTO,
                            MEAL_PHOTO_NAME = _mealMst.MEAL_PHOTO_NAME,
                        },
                        //ADDEDBY = data.ADDEDBY,
                        ADDEDDATE = data.ADDEDDATE,
                        MEALOPTIONID = _mealOption.CSDMEAL_OPTIONID,
                        MEALOPTION = _mealOption.MEAL_OPTION
                    }
                ).ToList();

                // cancellation logic (same as your original)
                foreach (var obj in iList)
                {
                    if (TIME_DTL_LIST.Count > 0)
                    {
                        string CANCELLATION_TIME = GetColValue(obj.MEALTYPEID, obj.MEALOPTIONID, "CANCELLATION_TIME");
                        cancellationTime = !string.IsNullOrEmpty(CANCELLATION_TIME) ? CANCELLATION_TIME : cancellationTime;

                        string CANCELLATION_ALLOW_DAYS = GetColValue(obj.MEALTYPEID, obj.MEALOPTIONID, "CANCELLATION_DAY");
                        MealBookDay = !string.IsNullOrEmpty(CANCELLATION_ALLOW_DAYS) ? Convert.ToInt32(CANCELLATION_ALLOW_DAYS) : MealBookDay;
                    }

                    DateTime validCancelDate = DateTime.ParseExact(
                        obj.MEAL_BOOKED_DATE.ToString("dd-MMM-yyyy") + " " + cancellationTime,
                        "dd-MMM-yyyy HH:mm:ss",
                        null
                    ).AddDays(-MealBookDay);

                    obj.IsEnableCancel = currentDate < validCancelDate;
                    dietList.Add(obj);
                }

                foreach (var item in dietList)
                {
                    item.FeedbackPoints = GetBookingFeedback(
                        item.CSD_MEALSBOOKING_ID,   // bookingId
                        item.MEALTYPEID,            // mealType
                        item.ADDEDBY                // userId
                    );
                }
            }

            if (mealType == 1)
            {
                return regularMeals
                    .OrderByDescending(o => o.MEAL_BOOKED_DATE)
                    .ThenByDescending(t => t.CSD_MEALSBOOKING_ID)
                    .ToList();
            }
            if (mealType != 1 && mealType != 0)
            {
                return dietList
                    .OrderByDescending(o => o.MEAL_BOOKED_DATE)
                    .ThenByDescending(t => t.CSD_MEALSBOOKING_ID)
                    .ToList();
            }

            var finalList = regularMeals.Concat(dietList)
               .OrderByDescending(o => o.MEAL_BOOKED_DATE)
               .ThenByDescending(t => t.CSD_MEALSBOOKING_ID)
               .ToList();

            return finalList;
        }

        public void SendFeedbackMailForMeal(long bookingId, long mealTypeID, long userId)
        {
            try
            {
                string strValue = _CommonRepo.GetParameterValue("CANTEEN_MEAL_FEEDBACK_MAIL");

                List<string> emailList = strValue.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();

                foreach (var _email in emailList)
                {
                    List<FeedbackPointViewModel> FeedbackDetails = GetBookingFeedback(bookingId, mealTypeID, userId);

                    if (FeedbackDetails == null)
                    {
                        throw new ArgumentException("Invalid booking ID. No details found.");
                    }

                    var _booking = GetBookingFeedback(bookingId);

                    Employee_Details requester = _CommonRepo.GetEmpDetailById(userId);

                    if (requester == null)
                    {
                        throw new ArgumentException("Invalid requester details.");
                    }

                    StringBuilder ratingRows = new StringBuilder();

                    foreach (var item in FeedbackDetails)
                    {
                        ratingRows.Append($@"
                                <tr>
                                    <td>{item.PointName}</td>
                                    <td>{item.Rating}</td>
                                    <td>{item.Remark}</td>
                                </tr>");
                    }


                    string subject = "Canteen Meal Feedback Response";

                    string body = $@"<table cellpadding='0' cellspacing='0' style='border-style:solid; border-color:#009933; border-width:1px; width:600px; font-family:Arial, sans-serif;'>

                    <tr bgcolor='#009933'>
                        <td height='30'>&nbsp;<b><font color='#FFFFFF'>Canteen Meal Feedback Response - Emp Code ({requester._ECode})</font></b></td>
                    </tr>
                    <tr>
                        <td>
                            <table cellpadding='8' cellspacing='0' border='0' width='600px' style='border-collapse:collapse;'>
                                <tr>
                                   <td valign=""top"" colspan=""2"">
                                    <b>Dear San,</b><br/><br/>

                                    A <b style=""color:#cc0000;"">Poor Meal Feedback</b> has been submitted by a user.  
                                    Please find the details below for your review:<br/><br/>

                                    <table border=""1"" cellpadding=""8"" cellspacing=""0"" width=""100%"" style=""border-collapse:collapse; font-family:Arial; font-size:13px;"">
                                        <tr style=""background-color:#f2f2f2; font-weight:bold;"">
                                            <td colspan=""2"">Booking Details</td>
                                        </tr>
                                        <tr>
                                            <td><b>Employee Name</b></td>
                                            <td>{requester._EName}</td>
                                        </tr>
                                        <tr>
                                            <td><b>Employee Code</b></td>
                                            <td>{requester._ECode}</td>
                                        </tr>
                                        <tr>
                                            <td><b>Meal Type</b></td>
                                            <td>{_booking.MealTypeDesc}</td>
                                        </tr>
                                        <tr>
                                            <td><b>Booking Date</b></td>
                                            <td>{_booking.Booking.MEAL_BOOKED_DATE}</td>
                                        </tr>

                                        <tr style=""background-color:#f2f2f2; font-weight:bold;"">
                                            <td colspan=""2"">Feedback Details</td>
                                        </tr>

                                        {ratingRows}
                                        
                                    </table>
                                    <br/><br/>
                                    This feedback requires your attention for further action.<br/><br/>

                                    <b>Regards,</b><br/>
                                    HMSI
                                </td>

                                </tr>
                                <tr>
                                    <td colspan='2' style='padding-top:20px; font-size:12px; color:#666;'>
                                        Note: This is a system-generated Email. Please do not reply.
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>";

                    EmailCore email = new EmailCore
                    {
                        MailFrom = "portal.admin@honda.hmsi.in",
                        MailTo = _email,
                        MailSubject = subject,
                        MailBody = body
                    };

                    if (!email.Send())
                    {
                        throw new ApplicationException("Email sending failed.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while processing the request. Please try again later.", ex);
            }
        }
        #endregion
    }
}