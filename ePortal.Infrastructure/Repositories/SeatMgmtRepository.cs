using System.Data;
using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.Persistence.Interface;
using ePortal.ViewModels;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static ePortal.ViewModels.APIMapper;

namespace ePortal.Infrastructure.Repositories
{
    public class SeatMgmtRepository
    {
        private readonly IConnectionString _conn;
        private EPortalDGITDBContext _dbContext;
        private SYKI_DGIT _Syki;

        public SeatMgmtRepository(EPortalDGITDBContext dbContext, IConnectionString conn)
        {
            _dbContext = dbContext;
            _Syki = _dbContext.SYKI.Where(x => x.ACTIVE == 1).FirstOrDefault();
            _conn = conn;
        }        

        public bool IsOffDay(string date, long siteId, long operationId)
        {
            long? _calSiteId = _dbContext.ASRCALOPMAPPING.Where(c => c.CALOPERATIONID == operationId && c.EMPSITE == siteId && c.ACTIVE == 1).Select(s => s.CALSITEID).FirstOrDefault();
            long _siteId = (_calSiteId == null || _calSiteId == 0) ? siteId : (long)_calSiteId;
            bool IsOffDay = false;
            if (!string.IsNullOrEmpty(date))
            {
                DateTime _date = DateTime.ParseExact(date, "dd-MMM-yyyy", null);
                IsOffDay = _dbContext.HMSIHOLIDAYS.Count(h => h.MONTHDATEYEAR == _date && h.ACTIVE == 1 && h.SYSITEID == _siteId) > 0;
            }
            return IsOffDay;
        }

        public string GetParmValByParmName(string ParmName)
        {
            return _dbContext.SM_SETTING_MST.Where(f => f.PARM_NAME == ParmName && f.STATUS == 1).Select(s => s.PARM_VALUE).FirstOrDefault();
        }

        public List<SeatSettingViewModel> GetParmListByName()
        {
            return (from data in _dbContext.SM_SETTING_MST.Where(f => f.STATUS == 1)
                    select new SeatSettingViewModel
                    {
                        PARM_NAME = data.PARM_NAME,
                        PARM_VALUE = data.PARM_VALUE,
                    }).ToList();
        }

        public Tuple<short, string> SaveRoster(List<SeatRosterViewModel> ModelList, long ActionBy)
        {
            short retVal = 0;
            string msg = "";
            Tuple<short, string> _tuple = new Tuple<short, string>(retVal, msg);

            //using (DbContextTransaction transaction = _dbContext.Database.BeginTransaction())
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (SeatRosterViewModel model in ModelList)
                    {
                        int FlagAdd = 0;
                        DateTime rosterDate = DateTime.Now.Date;
                        if (!string.IsNullOrEmpty(model.strROSTER_DATE))
                        {
                            rosterDate = DateTime.ParseExact(model.strROSTER_DATE, "dd-MMM-yyyy", null);
                        }

                        #region Div head(N+1) days
                        if (model.IS_OHACTION == 0)
                        {
                            DateTime currentDate = DateTime.Now.Date;
                            string currentTime = DateTime.Now.ToString("HH:mm:ss");

                            //
                            bool isRosterFillBeforeFreeze = rosterFillBeforeFreeze(rosterDate);

                            //short isDivHead_Next_Day = IsReadOnlyForDivHead(rosterDate, currentDate, currentTime) == false ? (short)1 : (short)0;
                            short isDivHead_Next_Day = isRosterFillBeforeFreeze == true ? (short)0 : (short)1;
                            model.IS_OHACTION = isDivHead_Next_Day;
                        }
                        #endregion

                        SM_ROSTER_TRN BCM = new SM_ROSTER_TRN();
                        BCM = _dbContext.SM_ROSTER_TRN.Where(a => a.ROSTER_DATE == rosterDate && a.ADEMPCODE == model.ADEMPCODE).FirstOrDefault();
                        if (BCM == null)
                        {
                            BCM = new SM_ROSTER_TRN();
                            if (_dbContext.SM_ROSTER_TRN.Count() == 0)
                            {
                                BCM.RSTTRNID = 1;
                            }
                            else
                            {
                                BCM.RSTTRNID = _dbContext.SM_ROSTER_TRN.Max(m => m.RSTTRNID) + 1;
                            }
                            FlagAdd = 1;
                        }
                        else
                        {
                            if (model.IS_OHACTION == 1) //// Operating Head Action
                            {
                                SM_SEATALLOCATION_TRN ALLOCATION_OBJ = _dbContext.SM_SEATALLOCATION_TRN.Where(l => l.RSTTRNID == BCM.RSTTRNID).FirstOrDefault();
                                if (ALLOCATION_OBJ != null)
                                {
                                    ALLOCATION_OBJ.STATUS = 0;
                                    ALLOCATION_OBJ.UPDATEDBY = ActionBy;
                                    ALLOCATION_OBJ.UPDATEDON = DateTime.Now;
                                    _dbContext.Entry(ALLOCATION_OBJ).State = EntityState.Modified;
                                    _dbContext.SaveChanges();
                                }
                            }
                        }

                        BCM.ADEMPCODE = model.ADEMPCODE;
                        BCM.ROSTER_DATE = rosterDate;
                        BCM.STATUS = model.STATUS;
                        BCM.IS_OHACTION = model.IS_OHACTION;
                        BCM.IS_NEWJOINEEDAY = model.IS_NEWJOINEEDAY; //SR102091

                        if (FlagAdd == 1)
                        {
                            BCM.ADDEDBY = ActionBy;
                            BCM.ADDEDON = DateTime.Now;
                        }
                        else
                        {
                            BCM.UPDATEDBY = ActionBy;
                            BCM.UPDATEDON = DateTime.Now;
                        }
                        _dbContext.Entry(BCM).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
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

        public bool IsReadOnly(DateTime CAL_DATE, string ROASTER_ALLOED_DAYS, DateTime _ROASTER_FRZ_DATE, DateTime currentWeekEndDate, DateTime currentDate, string currentTime)
        {
            bool IS_READONLY = true;
            DateTime CAL_DATE_TIME = DateTime.ParseExact(CAL_DATE.ToString("dd-MMM-yyyy") + " " + currentTime, "dd-MMM-yyyy HH:mm:ss", null);
            DateTime CURRENT_DATETIME = DateTime.ParseExact(currentDate.ToString("dd-MMM-yyyy") + " " + currentTime, "dd-MMM-yyyy HH:mm:ss", null);
            if (CAL_DATE > currentDate)
            {
                string strValiDate = currentWeekEndDate.AddDays(Convert.ToInt32(ROASTER_ALLOED_DAYS)).ToString("dd-MMM-yyyy");
                DateTime ValidDate = DateTime.ParseExact(strValiDate + " " + "23:59:59", "dd-MMM-yyyy HH:mm:ss", null);

                if (CAL_DATE_TIME.Date > currentWeekEndDate.Date && CAL_DATE_TIME.Date <= ValidDate.Date)
                {
                    IS_READONLY = CURRENT_DATETIME > _ROASTER_FRZ_DATE ? true : false;
                }
                else if (CAL_DATE_TIME.Date > currentWeekEndDate.Date)
                {
                    IS_READONLY = false;
                }
            }
            return IS_READONLY;
        }

        #region Roster open for N+1 days (Division head)
        //public bool IsReadOnlyForDivHead(DateTime CAL_DATE, DateTime _ROASTER_FRZ_DATE, DateTime currentDate, string currentTime, long? _siteID, long? _OpId)
        public bool IsReadOnlyForDivHead(DateTime CAL_DATE, DateTime currentDate, string currentTime)
        {
            #region Div head (N+1) days
            string DIV_ROASTER_FRZ_TIME = "22:00:00";
            string DIV_ROASTER_FRZ_DAY = "1";
            DIV_ROASTER_FRZ_DAY = GetParmValByParmName("DIVHEADROSTERALLOWDAY");
            DIV_ROASTER_FRZ_TIME = GetParmValByParmName("DIVHEADROSTERALLOWTIME");
            DIV_ROASTER_FRZ_DAY = !string.IsNullOrEmpty(DIV_ROASTER_FRZ_DAY) ? DIV_ROASTER_FRZ_DAY : "1";
            DIV_ROASTER_FRZ_TIME = !string.IsNullOrEmpty(DIV_ROASTER_FRZ_TIME) ? DIV_ROASTER_FRZ_TIME : "22:00:00";

            DateTime allowDate = DateTime.Now.AddDays(Convert.ToInt32(DIV_ROASTER_FRZ_DAY)).Date;
            allowDate = DateTime.ParseExact(allowDate.ToString("dd-MMM-yyyy") + " " + DIV_ROASTER_FRZ_TIME, "dd-MMM-yyyy HH:mm:ss", null);
            #endregion

            bool IS_READONLY = true;
            DateTime CAL_DATE_TIME = DateTime.ParseExact(CAL_DATE.ToString("dd-MMM-yyyy") + " " + currentTime, "dd-MMM-yyyy HH:mm:ss", null);
            DateTime CURRENT_DATETIME = DateTime.ParseExact(currentDate.ToString("dd-MMM-yyyy") + " " + currentTime, "dd-MMM-yyyy HH:mm:ss", null);
            DateTime RST_FRZ_DATETIME = DateTime.ParseExact(currentDate.ToString("dd-MMM-yyyy") + " " + DIV_ROASTER_FRZ_TIME, "dd-MMM-yyyy HH:mm:ss", null);

            //Seat no. visible change start
            string DIVHEAD_ROASTER_FRZ_DATE = "0";
            DIVHEAD_ROASTER_FRZ_DATE = GetParmValByParmName("DIVHEAD_ROASTER_FRZ_DATE");
            currentDate = currentDate.AddDays(Convert.ToInt32(DIVHEAD_ROASTER_FRZ_DATE)).Date;
            //Seat no. visible change end

            if (CAL_DATE >= currentDate)
            {
                if (CURRENT_DATETIME > RST_FRZ_DATETIME)
                {
                    //DateTime nextDay = CURRENT_DATETIME.AddDays(0).AddSeconds(-1).Date;
                    DateTime nextDay = CURRENT_DATETIME.AddDays(Convert.ToInt32(DIVHEAD_ROASTER_FRZ_DATE)).Date;
                    if (nextDay.Date == CAL_DATE.Date)
                    {
                        IS_READONLY = true;
                    }
                    else
                    {
                        IS_READONLY = false;
                    }
                }
                else
                {
                    if (CAL_DATE_TIME.Date <= allowDate.Date)
                    {
                        IS_READONLY = CAL_DATE_TIME > allowDate ? true : false;
                    }
                }

                //if (CAL_DATE_TIME.Date <= allowDate.Date)
                //{
                //    IS_READONLY = CAL_DATE_TIME > allowDate ? true : false;
                //}
            }
            return IS_READONLY;
        }
        #endregion

        public bool IsReadOnlyForOPHead(DateTime CAL_DATE, DateTime _ROASTER_FRZ_DATE, DateTime currentDate, string currentTime)
        {
            bool IS_READONLY = true;
            DateTime CAL_DATE_TIME = DateTime.ParseExact(CAL_DATE.ToString("dd-MMM-yyyy") + " " + currentTime, "dd-MMM-yyyy HH:mm:ss", null);
            DateTime CURRENT_DATETIME = DateTime.ParseExact(currentDate.ToString("dd-MMM-yyyy") + " " + currentTime, "dd-MMM-yyyy HH:mm:ss", null);
            if (CAL_DATE >= currentDate)
            {
                if (CAL_DATE_TIME.Date == _ROASTER_FRZ_DATE.Date)
                {
                    IS_READONLY = CAL_DATE_TIME > _ROASTER_FRZ_DATE ? true : false;
                }
                else if (CAL_DATE_TIME.Date > _ROASTER_FRZ_DATE.Date)
                {
                    IS_READONLY = false;
                }
            }
            return IS_READONLY;
        }


        //public Tuple<int, int, int, int> GetSeatCountByDivID(long DivisionID, long SiteID)//Added by aumento : SR100656 //Commented for SR102091
        public Tuple<int, int, int, int> GetSeatCountByDivID(long DivisionID, long SiteID, int NewJoineeCount, int InactiveCount)//SR102091 Added NewJoineeCount, InactiveCount
        {
            int allowSeat = 0; int ALLOWED_PERCENT = 0; int PHYSICALSEAT = 0; int extraSeat = 0; int EXTRASEATFORDIV = 0;//Added by aumento : SR100656 - Add two variable extraSeat And EXTRASEATFORDIV
            Tuple<int, int, int, int> _tuple = new Tuple<int, int, int, int>(allowSeat, ALLOWED_PERCENT, PHYSICALSEAT, extraSeat);//Added by aumento : SR100656
            try
            {
                int ACTUAL_PERCENT = 0;
                DateTime CrntDate = DateTime.Now.Date;
                string _PERCENT = GetParmValByParmName("ACTUAL_PHY_SEAT");
                ACTUAL_PERCENT = !string.IsNullOrEmpty(_PERCENT) ? Convert.ToInt32(_PERCENT) : 0;

                if (ACTUAL_PERCENT > 0)
                {
                    //SR102091 START
                    int? INACTIVE_COUNT = 0; int NewJoiner_Count = 0;
                    if (NewJoineeCount != null)
                    {
                        NewJoiner_Count = NewJoineeCount;
                    }
                    if (InactiveCount != null)
                    {
                        INACTIVE_COUNT = InactiveCount;
                    }
                    //SR102091 END
                    int? seatCount = _dbContext.SM_DIVISIONWISESEATDETAILS.
                        Where(w => w.ADORGLEVELID == DivisionID && w.STATUS == 1).
                        Select(s => s.PHYSICALSEAT).FirstOrDefault();
                    PHYSICALSEAT = seatCount == null ? 0 : (int)seatCount;
                    PHYSICALSEAT = PHYSICALSEAT - (int)INACTIVE_COUNT - NewJoiner_Count; //SR102091

                    int? allowPercentage = _dbContext.SM_ALLOWPERCENT.
                        Where(a => a.STATUS == 1 && a.SITEID == SiteID && (CrntDate >= a.FROM_DATE && CrntDate <= a.TO_DATE)).
                        Select(s => s.ALLOWED_PERCENT).FirstOrDefault();
                    ALLOWED_PERCENT = allowPercentage == null ? 0 : (int)allowPercentage;

                    //Added by aumento : SR100656
                    long? ExtraSeat = _dbContext.SM_EXTRASEATFORDIV.
                        Where(a => a.DIVISIONID == DivisionID && a.STATUS == 1).
                        Select(s => s.EXTRASEATCOUNT).FirstOrDefault();
                    EXTRASEATFORDIV = ExtraSeat == null ? 0 : (int)ExtraSeat;

                    //Added by aumento : SR100656
                    allowSeat = (PHYSICALSEAT * ALLOWED_PERCENT / ACTUAL_PERCENT);
                    extraSeat = EXTRASEATFORDIV;//Added by aumento : SR100656
                }
            }
            catch (Exception ex)

            {
                allowSeat = 0;
            }
            _tuple = new Tuple<int, int, int, int>(allowSeat, ALLOWED_PERCENT, PHYSICALSEAT, extraSeat);//Added by aumento : SR100656
            return _tuple;
        }
        //public Tuple<int, int, int> GetSeatCountByOpID(long OpID, long SiteID)//Commented for SR102091
        public Tuple<int, int, int> GetSeatCountByOpID(long OpID, long SiteID, int NewJoineeCount, int InactiveCount)//SR102091 Added NewJoineeCount, InactiveCount
        {
            int allowSeat = 0; int ALLOWED_PERCENT = 0; int PHYSICALSEAT = 0;
            Tuple<int, int, int> _tuple = new Tuple<int, int, int>(allowSeat, ALLOWED_PERCENT, PHYSICALSEAT);
            try
            {
                int ACTUAL_PERCENT = 0;
                DateTime CrntDate = DateTime.Now.Date;
                string _PERCENT = GetParmValByParmName("ACTUAL_PHY_SEAT");
                ACTUAL_PERCENT = !string.IsNullOrEmpty(_PERCENT) ? Convert.ToInt32(_PERCENT) : 0;

                if (ACTUAL_PERCENT > 0)
                {
                    //SR102091 START
                    int? INACTIVE_COUNT = 0; int NewJoiner_Count = 0;
                    if (NewJoineeCount != null)
                    {
                        NewJoiner_Count = NewJoineeCount;
                    }
                    if (InactiveCount != null)
                    {
                        INACTIVE_COUNT = InactiveCount;
                    }
                    //SR102091 END
                    var divWiseList = (from data in _dbContext.SM_FLOOR_OPMAP.Where(x => x.OPERATIONID == OpID && x.STATUS == 1)
                                       join _SD in _dbContext.SM_DIVISIONWISESEATDETAILS on data.FLOOR_OPMAPPID equals _SD.FLOOR_OPMAPPID
                                       select _SD.PHYSICALSEAT).ToList();
                    PHYSICALSEAT = divWiseList == null ? 0 : divWiseList.Sum(val => val);

                    PHYSICALSEAT = PHYSICALSEAT - (int)INACTIVE_COUNT - NewJoiner_Count; //SR102091

                    int? allowPercentage = _dbContext.SM_ALLOWPERCENT.
                        Where(a => a.STATUS == 1 && a.SITEID == SiteID && (CrntDate >= a.FROM_DATE && CrntDate <= a.TO_DATE)).
                        Select(s => s.ALLOWED_PERCENT).FirstOrDefault();
                    ALLOWED_PERCENT = allowPercentage == null ? 0 : (int)allowPercentage;

                    allowSeat = (PHYSICALSEAT * ALLOWED_PERCENT / ACTUAL_PERCENT);
                }
            }
            catch (Exception ex)
            {
                allowSeat = 0;
            }
            _tuple = new Tuple<int, int, int>(allowSeat, ALLOWED_PERCENT, PHYSICALSEAT);
            return _tuple;
        }

        //SR102091 START
        public Tuple<int, int> GetNewJoineeAndInactiveCount(List<SeatEmpCalViewModel> empList, List<SeatCalenderViewModel> calHeaderList, int parmValue)
        {
            int newJoineeCount = 0;
            int inactiveCount = 0;

            if (calHeaderList == null || calHeaderList.Count == 0)
                return Tuple.Create(0, 0);

            DateTime monthStart = calHeaderList.First().CAL_DATE_FORMAT.Date;
            DateTime monthEnd = calHeaderList.Last().CAL_DATE_FORMAT.Date;
            DateTime today = DateTime.Today;

            DateTime targetDate;

            //calender month == current month
            if (today.Month == monthStart.Month && today.Year == monthStart.Year)
            {
                targetDate = today.AddDays(1); // current date+1 day
            }
            else if (parmValue < 0)
            {
                targetDate = monthEnd; // previous month = last day of month
            }
            else
            {
                targetDate = monthStart; // next month = first day of month
            }

            foreach (var emp in empList)
            {
                foreach (var cal in emp.CAL_LIST)
                {
                    DateTime parsedDate;
                    if (DateTime.TryParseExact(cal.CAL_DATE, "dd-MMM-yyyy",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out parsedDate))
                    {
                        if (parsedDate.Date == targetDate)
                        {
                            if (cal.IsNewJoinee)
                                newJoineeCount++;

                            if (cal.IsInActiveEmp)
                                inactiveCount++;
                        }
                    }
                }
            }
            return Tuple.Create(newJoineeCount, inactiveCount);
        }
        //SR102091 END

        public List<SeatEmpCalViewModel> GetEmpDivisionHead(long userId, List<SeatCalenderViewModel> Cal_List, Employee_Details Emp_Dtl)
        {
            string ROASTER_ALLOED_DAYS = "0";
            string ROASTER_FRZ_TIME = "23:59:59";
            string ROASTER_FRZ_DAY = "SUNDAY";
            DateTime _ROASTER_FRZ_DATE = DateTime.Now;
            DateTime currentDate = DateTime.Now.Date;

            string currentTime = DateTime.Now.ToString("HH:mm:ss");
            ROASTER_ALLOED_DAYS = GetParmValByParmName("ROASTER_ALLOED_DAYS");
            ROASTER_FRZ_DAY = GetParmValByParmName("ROASTER_FRZ_DAY");
            ROASTER_FRZ_TIME = GetParmValByParmName("ROASTER_FRZ_TIME");
            ROASTER_ALLOED_DAYS = !string.IsNullOrEmpty(ROASTER_ALLOED_DAYS) ? ROASTER_ALLOED_DAYS : "0";
            ROASTER_FRZ_DAY = !string.IsNullOrEmpty(ROASTER_FRZ_DAY) ? ROASTER_FRZ_DAY : "SUNDAY";
            ROASTER_FRZ_TIME = !string.IsNullOrEmpty(ROASTER_FRZ_TIME) ? ROASTER_FRZ_TIME : "23:59:59";

            DayOfWeek currentDay = currentDate.DayOfWeek;
            int daysTillCurrentDay = currentDay - DayOfWeek.Monday;
            DateTime currentWeekStartDate = currentDate.AddDays(-daysTillCurrentDay).Date;
            DateTime currentWeekEndDate = currentWeekStartDate.AddDays(7).AddSeconds(-1).Date;

            DateTime[] currWeekdates = Enumerable.Range(0, 1 + currentWeekEndDate.Subtract(currentWeekStartDate).Days).Select(i => currentWeekStartDate.AddDays(i)).ToArray();
            DateTime new_FRZ_DATE = currWeekdates.Where(x => x.DayOfWeek.ToString().ToUpper() == ROASTER_FRZ_DAY.ToUpper().Trim()).FirstOrDefault();
            if (new_FRZ_DATE != null)
            {
                _ROASTER_FRZ_DATE = DateTime.ParseExact(new_FRZ_DATE.ToString("dd-MMM-yyyy") + " " + ROASTER_FRZ_TIME, "dd-MMM-yyyy HH:mm:ss", null);
            }

            List<SeatEmpCalViewModel> _EmpList = new List<SeatEmpCalViewModel>();
            try
            {
                List<long> _desgIds = new List<long>();
                string _ROSTER_ALLOW_DESG = GetParmValByParmName("ROSTER_ALLOW_DESG");
                if (!string.IsNullOrEmpty(_ROSTER_ALLOW_DESG))
                {
                    _desgIds = _ROSTER_ALLOW_DESG.Split(',').Select(s => long.Parse(s)).ToList();
                }

                List<long> _siteIds = new List<long>();
                string _ROSTER_ALLOW_SITE = GetParmValByParmName("ROSTER_ALLOW_SITE");
                if (!string.IsNullOrEmpty(_ROSTER_ALLOW_SITE))
                {
                    _siteIds = _ROSTER_ALLOW_SITE.Split(',').Select(s => long.Parse(s)).ToList();
                }

                _EmpList = (from _desID in _desgIds
                            join data in _dbContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == _Syki.SYKIID && x.ACTIVE == 1) on _desID equals data.ADDESIGNATIONID
                            join _siteId in _siteIds on data.SYSITEID equals _siteId
                            join _opMap in _dbContext.SM_FLOOR_OPMAP.Where(o => o.STATUS == 1) on data.OPERATIONID equals _opMap.OPERATIONID
                            join _emp in _dbContext.ADEMPLOYEE.Where(w => w.ACTIVE == 1) on data.ADEMPCODE equals _emp.ADEMPCODE
                            where (Emp_Dtl._OpId == 0 || Emp_Dtl._OpId == null ? true : data.OPERATIONID == Emp_Dtl._OpId)
                            && (Emp_Dtl._DivId == 0 || Emp_Dtl._DivId == null ? true : data.DIVISIONID == Emp_Dtl._DivId)
                            && (Emp_Dtl._DepId == 0 || Emp_Dtl._DepId == null ? true : data.DEPARTMENTID == Emp_Dtl._DepId)
                            && (Emp_Dtl._SecId == 0 || Emp_Dtl._SecId == null ? true : data.SECTIONID == Emp_Dtl._SecId)
                            && (Emp_Dtl._FnDesigId == 0 || Emp_Dtl._FnDesigId == null ? data.ADEMPCODE == userId : true)
                            && _opMap.OPERATIONID == Emp_Dtl._OpId
                            select new SeatEmpCalViewModel
                            {
                                ECODE = _emp.ADEMPCODE,
                                ENAME = (_emp.FIRSTNAME + " " + _emp.LASTNAME).Trim(),
                                DIVISIONID = data.DIVISIONID,
                                DIVISION = data.DIVISION,
                                DEPARTMENTID = data.DEPARTMENTID,
                                DEPARTMENT = data.DEPARTMENT,
                                FN_DESIGNATIONID = data.ADFUNCTIONALDESIGNATIONID == null ? 0 : data.ADFUNCTIONALDESIGNATIONID,
                                CAL_LIST = (from calObj in Cal_List
                                            join _Trn in _dbContext.SM_ROSTER_TRN.Where(t => t.ADEMPCODE == _emp.ADEMPCODE && t.STATUS == 1) on calObj.CAL_DATE_FORMAT equals _Trn.ROSTER_DATE into TrnJoin
                                            from _TrnData in TrnJoin.DefaultIfEmpty()
                                            select new SeatCalenderViewModel
                                            {
                                                CALID = calObj.CALID,
                                                CAL_DATE = calObj.CAL_DATE,
                                                IS_OFFDAY = calObj.IS_OFFDAY,
                                                OFFDAY_COLOR = calObj.OFFDAY_COLOR,
                                                WORKINGDAY_COLOR = calObj.WORKINGDAY_COLOR,
                                                WEEK_DAY = calObj.WEEK_DAY,
                                                IS_CHECKED = _TrnData == null ? false : true,
                                                //IS_READONLY = IsReadOnly(Convert.ToDateTime(calObj.CAL_DATE), ROASTER_ALLOED_DAYS, _ROASTER_FRZ_DATE, currentWeekEndDate, currentDate, currentTime), (Div head (N+1) days)

                                                //IS_READONLY = IsReadOnlyForDivHead(Convert.ToDateTime(calObj.CAL_DATE), allowDate, currentDate, currentTime, Emp_Dtl._SiteId, Emp_Dtl._OpId) == false ? false :
                                                IS_READONLY = IsReadOnlyForDivHead(Convert.ToDateTime(calObj.CAL_DATE), currentDate, currentTime) == false ? false :
                                                IsReadOnly(Convert.ToDateTime(calObj.CAL_DATE), ROASTER_ALLOED_DAYS, _ROASTER_FRZ_DATE, currentWeekEndDate, currentDate, currentTime),
                                                //SR102091 START
                                                IsNewJoinee = IsNewJoineeDay(Convert.ToDateTime(calObj.CAL_DATE), _emp.ADEMPCODE),
                                                IsInActiveEmp = IsInActiveEmployee(Convert.ToDateTime(calObj.CAL_DATE), _emp.ADEMPCODE)
                                                //SR102091 END
                                            }).ToList(),
                            }).OrderBy(o => o.DIVISIONID).ThenBy(t => t.DEPARTMENTID).ThenByDescending(d => d.FN_DESIGNATIONID).ThenBy(e => e.ENAME).ToList();
            }
            catch (Exception ex)
            {
                _EmpList = new List<SeatEmpCalViewModel>();
            }
            return _EmpList;
        }

        public List<SeatEmpCalViewModel> GetEmpOpHead(long userId, List<SeatCalenderViewModel> Cal_List, Employee_Details Emp_Dtl)
        {
            //string ROASTER_ALLOED_DAYS = "0";
            string ROASTER_FRZ_TIME = "23:59:59";
            int ROASTER_FRZ_DAY = 0;
            DateTime _ROASTER_FRZ_DATE = DateTime.Now;
            DateTime currentDate = DateTime.Now.Date;
            string currentTime = DateTime.Now.ToString("HH:mm:ss");

            //ROASTER_ALLOED_DAYS = GetParmValByParmName("OPHEAD_ROASTER_FRZ_DATE");
            ROASTER_FRZ_DAY = Convert.ToInt32(GetParmValByParmName("OPHEAD_ROASTER_FRZ_DATE"));
            ROASTER_FRZ_TIME = GetParmValByParmName("OPHEAD_ROASTER_FRZ_TIME");
            //ROASTER_ALLOED_DAYS = !string.IsNullOrEmpty(ROASTER_ALLOED_DAYS) ? ROASTER_ALLOED_DAYS : "0";
            ROASTER_FRZ_TIME = !string.IsNullOrEmpty(ROASTER_FRZ_TIME) ? ROASTER_FRZ_TIME : "23:59:59";
            _ROASTER_FRZ_DATE = (DateTime.ParseExact(_ROASTER_FRZ_DATE.ToString("dd-MMM-yyyy") + " " + ROASTER_FRZ_TIME, "dd-MMM-yyyy HH:mm:ss", null).AddDays(ROASTER_FRZ_DAY));

            //DayOfWeek currentDay = currentDate.DayOfWeek;
            //int daysTillCurrentDay = currentDay - DayOfWeek.Monday;
            //DateTime currentWeekStartDate = currentDate.AddDays(-daysTillCurrentDay).Date;
            //DateTime currentWeekEndDate = currentWeekStartDate.AddDays(7).AddSeconds(-1).Date;

            //DateTime[] currWeekdates = Enumerable.Range(0, 1 + currentWeekEndDate.Subtract(currentWeekStartDate).Days).Select(i => currentWeekStartDate.AddDays(i)).ToArray();
            //DateTime new_FRZ_DATE = currWeekdates.Where(x => x.DayOfWeek.ToString().ToUpper() == ROASTER_FRZ_DAY.ToUpper().Trim()).FirstOrDefault();
            //if (new_FRZ_DATE != null)
            //{
            //    _ROASTER_FRZ_DATE = DateTime.ParseExact(new_FRZ_DATE.ToString("dd-MMM-yyyy") + " " + ROASTER_FRZ_TIME, "dd-MMM-yyyy HH:mm:ss", null);
            //}

            List<SeatEmpCalViewModel> _EmpList = new List<SeatEmpCalViewModel>();
            try
            {
                List<long> _desgIds = new List<long>();
                string _ROSTER_ALLOW_DESG = GetParmValByParmName("ROSTER_ALLOW_DESG");
                if (!string.IsNullOrEmpty(_ROSTER_ALLOW_DESG))
                {
                    _desgIds = _ROSTER_ALLOW_DESG.Split(',').Select(s => long.Parse(s)).ToList();
                }

                List<long> _siteIds = new List<long>();
                string _ROSTER_ALLOW_SITE = GetParmValByParmName("ROSTER_ALLOW_SITE");
                if (!string.IsNullOrEmpty(_ROSTER_ALLOW_SITE))
                {
                    _siteIds = _ROSTER_ALLOW_SITE.Split(',').Select(s => long.Parse(s)).ToList();
                }

                _EmpList = (from _desID in _desgIds
                            join data in _dbContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == _Syki.SYKIID && x.ACTIVE == 1) on _desID equals data.ADDESIGNATIONID
                            join _siteId in _siteIds on data.SYSITEID equals _siteId
                            join _opMap in _dbContext.SM_FLOOR_OPMAP.Where(o => o.STATUS == 1) on data.OPERATIONID equals _opMap.OPERATIONID
                            join _emp in _dbContext.ADEMPLOYEE.Where(w => w.ACTIVE == 1) on data.ADEMPCODE equals _emp.ADEMPCODE
                            where (Emp_Dtl._OpId == 0 || Emp_Dtl._OpId == null ? true : data.OPERATIONID == Emp_Dtl._OpId)
                            && (Emp_Dtl._DivId == 0 || Emp_Dtl._DivId == null ? true : data.DIVISIONID == Emp_Dtl._DivId)
                            && (Emp_Dtl._DepId == 0 || Emp_Dtl._DepId == null ? true : data.DEPARTMENTID == Emp_Dtl._DepId)
                            && (Emp_Dtl._SecId == 0 || Emp_Dtl._SecId == null ? true : data.SECTIONID == Emp_Dtl._SecId)
                            && (Emp_Dtl._FnDesigId == 0 || Emp_Dtl._FnDesigId == null ? data.ADEMPCODE == userId : true)
                            && _opMap.OPERATIONID == Emp_Dtl._OpId
                            select new SeatEmpCalViewModel
                            {
                                ECODE = _emp.ADEMPCODE,
                                ENAME = (_emp.FIRSTNAME + " " + _emp.LASTNAME).Trim(),
                                DIVISIONID = data.DIVISIONID,
                                DIVISION = data.DIVISION,
                                DEPARTMENTID = data.DEPARTMENTID,
                                DEPARTMENT = data.DEPARTMENT,
                                FN_DESIGNATIONID = data.ADFUNCTIONALDESIGNATIONID == null ? 0 : data.ADFUNCTIONALDESIGNATIONID,
                                CAL_LIST = (from calObj in Cal_List
                                            join _Trn in _dbContext.SM_ROSTER_TRN.Where(t => t.ADEMPCODE == _emp.ADEMPCODE && t.STATUS == 1) on calObj.CAL_DATE_FORMAT equals _Trn.ROSTER_DATE into TrnJoin
                                            from _TrnData in TrnJoin.DefaultIfEmpty()
                                            select new SeatCalenderViewModel
                                            {
                                                CALID = calObj.CALID,
                                                CAL_DATE = calObj.CAL_DATE,
                                                IS_OFFDAY = calObj.IS_OFFDAY,
                                                OFFDAY_COLOR = calObj.OFFDAY_COLOR,
                                                WORKINGDAY_COLOR = calObj.WORKINGDAY_COLOR,
                                                WEEK_DAY = calObj.WEEK_DAY,
                                                IS_CHECKED = _TrnData == null ? false : true,
                                                //IS_READONLY =  (_ROASTER_FRZ_DATE > DateTime.ParseExact(calObj.CAL_DATE + " " + currentTime, "dd-MMM-yyyy HH:mm:ss", null)) ? true : false
                                                //IS_READONLY = (DateTime.ParseExact(calObj.CAL_DATE + " " + currentTime, "dd-MMM-yyyy HH:mm:ss", null) > _ROASTER_FRZ_DATE) ? true : false,
                                                IS_READONLY = IsReadOnlyForOPHead(Convert.ToDateTime(calObj.CAL_DATE), _ROASTER_FRZ_DATE, currentDate, currentTime),
                                                //SR102091 START
                                                IsNewJoinee = IsNewJoineeDay(Convert.ToDateTime(calObj.CAL_DATE), _emp.ADEMPCODE),
                                                IsInActiveEmp = IsInActiveEmployee(Convert.ToDateTime(calObj.CAL_DATE), _emp.ADEMPCODE)
                                                //SR102091 END
                                            }).ToList(),
                            }).OrderBy(o => o.DIVISIONID).ThenBy(t => t.DEPARTMENTID).ThenByDescending(d => d.FN_DESIGNATIONID).ThenBy(e => e.ENAME).ToList();
            }
            catch (Exception ex)
            {
                _EmpList = new List<SeatEmpCalViewModel>();
            }
            return _EmpList;
        }

        //    public async Task<List<SeatEmpCalViewModel>> ViewRosterList(long userId, List<SeatCalenderViewModel> Cal_List, Employee_Details Emp_Dtl)
        //    {
        //        var _EmpList = new List<SeatEmpCalViewModel>();
        //        try
        //        {
        //            // Get allowed designation and site IDs
        //            var _desgIds = GetParmValByParmName("ROSTER_ALLOW_DESG")?
        //                .Split(',').Select(long.Parse).ToList() ?? new List<long>();

        //            var _siteIds = GetParmValByParmName("ROSTER_ALLOW_SITE")?
        //                .Split(',').Select(long.Parse).ToList() ?? new List<long>();

        //            // Fetch employee details with joins
        //            //var empDetails = (from data in _dbContext.VW_ASSOCIATELVLDETAILS.AsNoTracking()
        //            //                  join opMap in _dbContext.SM_FLOOR_OPMAP.AsNoTracking().Where(o => o.STATUS == 1)
        //            //                      on data.OPERATIONID equals opMap.OPERATIONID
        //            //                  join emp in _dbContext.ADEMPLOYEE.AsNoTracking().Where(w => w.ACTIVE == 1)
        //            //                      on data.ADEMPCODE equals emp.ADEMPCODE
        //            //                  where data.SYKI == _Syki.SYKIID
        //            //                        && data.ACTIVE == 1
        //            //                        && data.ADDESIGNATIONID.HasValue && _desgIds.Contains(data.ADDESIGNATIONID.Value)
        //            //                        && data.SYSITEID.HasValue && _siteIds.Contains(data.SYSITEID.Value)
        //            //                        && (Emp_Dtl._OpId == 0 || Emp_Dtl._OpId == null || data.OPERATIONID == Emp_Dtl._OpId)
        //            //                        && (Emp_Dtl._DivId == 0 || Emp_Dtl._DivId == null || data.DIVISIONID == Emp_Dtl._DivId)
        //            //                  select new
        //            //                  {
        //            //                      emp.ADEMPCODE,
        //            //                      emp.FIRSTNAME,
        //            //                      emp.LASTNAME,
        //            //                      data.DIVISIONID,
        //            //                      data.DIVISION,
        //            //                      data.DEPARTMENTID,
        //            //                      data.DEPARTMENT,
        //            //                      data.ADFUNCTIONALDESIGNATIONID,
        //            //                      opMap.FLOORID
        //            //                  }).ToList();

        //            var empDetails = await
        //(from data in _dbContext.VW_ASSOCIATELVLDETAILS
        //     .AsNoTracking()
        //     .Where(x => x.SYKI == _Syki.SYKIID && x.ACTIVE == 1 &&
        //                 x.ADDESIGNATIONID.HasValue && _desgIds.Contains(x.ADDESIGNATIONID.Value) &&
        //                 x.SYSITEID.HasValue && _siteIds.Contains(x.SYSITEID.Value))
        // join op in _dbContext.SM_FLOOR_OPMAP.AsNoTracking().Where(o => o.STATUS == 1)
        //     on data.OPERATIONID equals op.OPERATIONID
        // join emp in _dbContext.ADEMPLOYEE.AsNoTracking().Where(e => e.ACTIVE == 1)
        //     on data.ADEMPCODE equals emp.ADEMPCODE
        // where (Emp_Dtl._OpId == 0 || Emp_Dtl._OpId == null || data.OPERATIONID == Emp_Dtl._OpId)
        //       && (Emp_Dtl._DivId == 0 || Emp_Dtl._DivId == null || data.DIVISIONID == Emp_Dtl._DivId)
        // select new
        // {
        //     emp.ADEMPCODE,
        //     emp.FIRSTNAME,
        //     emp.LASTNAME,
        //     data.DIVISIONID,
        //     data.DIVISION,
        //     data.DEPARTMENTID,
        //     data.DEPARTMENT,
        //     data.ADFUNCTIONALDESIGNATIONID,
        //     op.FLOORID
        // }).ToListAsync();


        //            //var employeeCodes = empDetails.Select(e => e.ADEMPCODE).ToList();
        //            //var calendarDates = Cal_List.Select(c => c.CAL_DATE_FORMAT).ToList();

        //            //// Fetch roster data
        //            //var rosterData = _dbContext.SM_ROSTER_TRN.AsNoTracking()
        //            //    .Where(t => employeeCodes.Contains(t.ADEMPCODE)
        //            //                && calendarDates.Contains(t.ROSTER_DATE)
        //            //                && t.STATUS == 1)
        //            //    .Select(t => new { t.ADEMPCODE, t.ROSTER_DATE })
        //            //    .ToList();


        //            var employeeCodes = empDetails.Select(e => e.ADEMPCODE).ToList();
        //            var calendarDates = Cal_List.Select(c => c.CAL_DATE_FORMAT).ToList();

        //            var rosterData = await _dbContext.SM_ROSTER_TRN
        //                .AsNoTracking()
        //                .Where(t => employeeCodes.Contains(t.ADEMPCODE) &&
        //                            calendarDates.Contains(t.ROSTER_DATE) &&
        //                            t.STATUS == 1)
        //                .ToListAsync();

        //            var rosterDict = rosterData
        //                .GroupBy(x => (x.ADEMPCODE, x.ROSTER_DATE))
        //                .ToDictionary(g => g.Key, g => g.First());


        //            var seatAllocData = await
        //(from seat in _dbContext.SM_SEATALLOCATION_TRN.AsNoTracking()
        // join rst in _dbContext.SM_ROSTER_TRN.AsNoTracking()
        //     on seat.RSTTRNID equals rst.RSTTRNID
        // join mst in _dbContext.SM_FLOORSEATMST.AsNoTracking()
        //     on seat.SEATMSTID equals mst.SEATMSTID
        // where seat.STATUS == 1
        // select new
        // {
        //     rst.ADEMPCODE,
        //     rst.ROSTER_DATE,
        //     SEAT = new SeatAllocationViewModel
        //     {
        //         SEAT_NAME = mst.SEATNAME,
        //         SEAT_NO = mst.SEATNO
        //     }
        // }).ToListAsync();

        //            var seatDict = seatAllocData
        //                .GroupBy(x => (x.ADEMPCODE, x.ROSTER_DATE))
        //                .ToDictionary(g => g.Key, g => g.First().SEAT);



        //            _EmpList = empDetails
        //.Select(emp => new SeatEmpCalViewModel
        //{
        //    ECODE = emp.ADEMPCODE,
        //    ENAME = (emp.FIRSTNAME + " " + emp.LASTNAME).Trim(),
        //    DIVISIONID = emp.DIVISIONID,
        //    DIVISION = emp.DIVISION,
        //    DEPARTMENTID = emp.DEPARTMENTID,
        //    DEPARTMENT = emp.DEPARTMENT,
        //    FN_DESIGNATIONID = emp.ADFUNCTIONALDESIGNATIONID ?? 0,
        //    FLOORID = emp.FLOORID,

        //    CAL_LIST = Cal_List.Select(cal => {
        //        rosterDict.TryGetValue((emp.ADEMPCODE, cal.CAL_DATE_FORMAT), out var rosterEntry);
        //        seatDict.TryGetValue((emp.ADEMPCODE, cal.CAL_DATE_FORMAT), out var seat);

        //        return new SeatCalenderViewModel
        //        {
        //            CALID = cal.CALID,
        //            CAL_DATE = cal.CAL_DATE,
        //            IS_OFFDAY = cal.IS_OFFDAY,
        //            OFFDAY_COLOR = cal.OFFDAY_COLOR,
        //            WORKINGDAY_COLOR = cal.WORKINGDAY_COLOR,
        //            WEEK_DAY = cal.WEEK_DAY,
        //            IS_CHECKED = rosterEntry != null,
        //            SEAT_OBJ = seat,
        //            IsNewJoinee = IsNewJoineeDay(Convert.ToDateTime(cal.CAL_DATE), emp.ADEMPCODE),
        //            IsInActiveEmp = IsInActiveEmployee(Convert.ToDateTime(cal.CAL_DATE), emp.ADEMPCODE)
        //        };
        //    }).ToList()
        //})
        //.OrderBy(o => o.DIVISIONID)
        //.ThenBy(t => t.DEPARTMENTID)
        //.ThenBy(e => e.ENAME)
        //.ToList();


        //        }
        //        catch
        //        {
        //            _EmpList = new List<SeatEmpCalViewModel>();
        //        }
        //        return _EmpList;
        //    }


        //public List<SeatEmpCalViewModel> ViewRosterList(long userId, List<SeatCalenderViewModel> Cal_List, Employee_Details Emp_Dtl)
        //{
        //    var _EmpList = new List<SeatEmpCalViewModel>();
        //    try
        //    {
        //        // Get allowed designation and site IDs
        //        var _desgIds = GetParmValByParmName("ROSTER_ALLOW_DESG")?
        //            .Split(',').Select(long.Parse).ToList() ?? new List<long>();

        //        var _siteIds = GetParmValByParmName("ROSTER_ALLOW_SITE")?
        //            .Split(',').Select(long.Parse).ToList() ?? new List<long>();

        //        // Fetch employee details with joins
        //        var empDetails = (from data in _dbContext.VW_ASSOCIATELVLDETAILS.AsNoTracking()
        //                          join opMap in _dbContext.SM_FLOOR_OPMAP.AsNoTracking().Where(o => o.STATUS == 1)
        //                              on data.OPERATIONID equals opMap.OPERATIONID
        //                          join emp in _dbContext.ADEMPLOYEE.AsNoTracking().Where(w => w.ACTIVE == 1)
        //                              on data.ADEMPCODE equals emp.ADEMPCODE
        //                          where data.SYKI == _Syki.SYKIID
        //                                && data.ACTIVE == 1
        //                                && data.ADDESIGNATIONID.HasValue && _desgIds.Contains(data.ADDESIGNATIONID.Value)
        //                                && data.SYSITEID.HasValue && _siteIds.Contains(data.SYSITEID.Value)
        //                                && (Emp_Dtl._OpId == 0 || Emp_Dtl._OpId == null || data.OPERATIONID == Emp_Dtl._OpId)
        //                                && (Emp_Dtl._DivId == 0 || Emp_Dtl._DivId == null || data.DIVISIONID == Emp_Dtl._DivId)
        //                          select new
        //                          {
        //                              emp.ADEMPCODE,
        //                              emp.FIRSTNAME,
        //                              emp.LASTNAME,
        //                              data.DIVISIONID,
        //                              data.DIVISION,
        //                              data.DEPARTMENTID,
        //                              data.DEPARTMENT,
        //                              data.ADFUNCTIONALDESIGNATIONID,
        //                              opMap.FLOORID
        //                          }).ToList();

        //        var employeeCodes = empDetails.Select(e => e.ADEMPCODE).ToList();
        //        var calendarDates = Cal_List.Select(c => c.CAL_DATE_FORMAT).ToList();

        //        // Fetch roster data
        //        var rosterData = _dbContext.SM_ROSTER_TRN.AsNoTracking()
        //            .Where(t => employeeCodes.Contains(t.ADEMPCODE)
        //                        && calendarDates.Contains(t.ROSTER_DATE)
        //                        && t.STATUS == 1)
        //            .Select(t => new { t.ADEMPCODE, t.ROSTER_DATE })
        //            .ToList();

        //        // Fetch seat allocations
        //        var seatAllocations = (from seatAllocation in _dbContext.SM_SEATALLOCATION_TRN.AsNoTracking()
        //                               join SRT in _dbContext.SM_ROSTER_TRN.AsNoTracking()
        //                                   on seatAllocation.RSTTRNID equals SRT.RSTTRNID
        //                               join seatMst in _dbContext.SM_FLOORSEATMST.AsNoTracking()
        //                                   on seatAllocation.SEATMSTID equals seatMst.SEATMSTID
        //                               where seatAllocation.STATUS == 1
        //                               select new
        //                               {
        //                                   SRT.ADEMPCODE,
        //                                   SRT.ROSTER_DATE,
        //                                   SeatData = new SeatAllocationViewModel
        //                                   {
        //                                       SEAT_NAME = seatMst.SEATNAME,
        //                                       SEAT_NO = seatMst.SEATNO
        //                                   }
        //                               }).ToList();

        //        // Convert to dictionaries for fast lookup
        //        var rosterDict = rosterData.ToDictionary(x => (x.ADEMPCODE, x.ROSTER_DATE));
        //        var seatDict = seatAllocations
        //            .GroupBy(s => (s.ADEMPCODE, s.ROSTER_DATE))
        //            .ToDictionary(g => g.Key, g => g.Select(x => x.SeatData).FirstOrDefault());

        //        // Build final list
        //        _EmpList = empDetails.Select(emp => new SeatEmpCalViewModel
        //        {
        //            ECODE = emp.ADEMPCODE,
        //            ENAME = (emp.FIRSTNAME + " " + emp.LASTNAME).Trim(),
        //            DIVISIONID = emp.DIVISIONID,
        //            DIVISION = emp.DIVISION,
        //            DEPARTMENTID = emp.DEPARTMENTID,
        //            DEPARTMENT = emp.DEPARTMENT,
        //            FN_DESIGNATIONID = emp.ADFUNCTIONALDESIGNATIONID ?? 0,
        //            FLOORID = emp.FLOORID,
        //            CAL_LIST = Cal_List.Select(calObj =>
        //            {
        //                rosterDict.TryGetValue((emp.ADEMPCODE, calObj.CAL_DATE_FORMAT), out var rosterEntry);
        //                seatDict.TryGetValue((emp.ADEMPCODE, calObj.CAL_DATE_FORMAT), out var seatObj);

        //                return new SeatCalenderViewModel
        //                {
        //                    CALID = calObj.CALID,
        //                    CAL_DATE = calObj.CAL_DATE,
        //                    IS_OFFDAY = calObj.IS_OFFDAY,
        //                    OFFDAY_COLOR = calObj.OFFDAY_COLOR,
        //                    WORKINGDAY_COLOR = calObj.WORKINGDAY_COLOR,
        //                    WEEK_DAY = calObj.WEEK_DAY,
        //                    IS_CHECKED = rosterEntry != null,
        //                    SEAT_OBJ = seatObj,
        //                    IsNewJoinee = IsNewJoineeDay(Convert.ToDateTime(calObj.CAL_DATE), emp.ADEMPCODE),
        //                    IsInActiveEmp = IsInActiveEmployee(Convert.ToDateTime(calObj.CAL_DATE), emp.ADEMPCODE)
        //                };
        //            }).ToList()
        //        })
        //        .OrderBy(o => o.DIVISIONID)
        //        .ThenBy(t => t.DEPARTMENTID)
        //        .ThenBy(e => e.ENAME)
        //        .ToList();
        //    }
        //    catch
        //    {
        //        _EmpList = new List<SeatEmpCalViewModel>();
        //    }
        //    return _EmpList;
        //}

        //    public Expression<Func<T, bool>> BuildInExpression<T, TProperty>(
        //Expression<Func<T, TProperty>> selector,
        //IEnumerable<TProperty> values)
        //    {
        //        var param = selector.Parameters[0];
        //        Expression? body = Expression.Constant(false);

        //        foreach (var val in values)
        //        {
        //            var equals = Expression.Equal(selector.Body, Expression.Constant(val));
        //            body = Expression.OrElse(body, equals);
        //        }

        //        return Expression.Lambda<Func<T, bool>>(body, param);
        //    }

        //    public List<SeatEmpCalViewModel> ViewRosterList(
        //    long userId,
        //    List<SeatCalenderViewModel> Cal_List,
        //    Employee_Details Emp_Dtl)
        //    {
        //        try
        //        {
        //            // ----------------------------------------------------
        //            // Load Designations Allowed
        //            // ----------------------------------------------------
        //            string desgStr = _dbContext.SM_SETTING_MST
        //                .Where(f => f.PARM_NAME == "ROSTER_ALLOW_DESG" && f.STATUS == 1)
        //                .Select(s => s.PARM_VALUE)
        //                .FirstOrDefault() ?? "";

        //            var desgIds = desgStr
        //                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
        //                .Select(long.Parse)
        //                .ToList();

        //            //if (desgIds.Count == 0)
        //            //    return new List<SeatEmpCalViewModel>();


        //            // ----------------------------------------------------
        //            // Load Sites Allowed
        //            // ----------------------------------------------------
        //            string siteStr = _dbContext.SM_SETTING_MST
        //                .Where(f => f.PARM_NAME == "ROSTER_ALLOW_SITE" && f.STATUS == 1)
        //                .Select(s => s.PARM_VALUE)
        //                .FirstOrDefault() ?? "";

        //            var siteIds = siteStr
        //                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
        //                .Select(long.Parse)
        //                .ToList();

        //            //if (siteIds.Count == 0)
        //            //    return new List<SeatEmpCalViewModel>();


        //            // ----------------------------------------------------
        //            // Build Oracle-Friendly IN Expressions
        //            // ----------------------------------------------------
        //            var desgCondition = BuildInExpression<VW_ASSOCIATELVLDETAILS, long?>(
        //                x => x.ADDESIGNATIONID, desgIds.Select(id => (long?)id));

        //            var siteCondition = BuildInExpression<VW_ASSOCIATELVLDETAILS, long?>(
        //                x => x.SYSITEID, siteIds.Select(id => (long?)id));

        //            var calendarDates = Cal_List.Select(c => c.CAL_DATE_FORMAT).ToList();

        //            var calCondition = BuildInExpression<SM_ROSTER_TRN, DateTime>(
        //                x => x.ROSTER_DATE, calendarDates);


        //            //if (desgCondition == null || siteCondition == null)
        //            //    throw new Exception("test");

        //            //Expression<Func<VW_ASSOCIATELVLDETAILS, bool>> desgFinal = desgCondition;
        //            //Expression<Func<VW_ASSOCIATELVLDETAILS, bool>> siteFinal = siteCondition;

        //            // ================================================================
        //            // 1. EMPLOYEE DETAILS QUERY (FULLY SERVER SIDE)
        //            // ================================================================
        //            var empDetails = (
        //                from data in _dbContext.VW_ASSOCIATELVLDETAILS.AsNoTracking()
        //                    .Where(desgCondition).Where(siteCondition)
        //                join opMap in _dbContext.SM_FLOOR_OPMAP.AsNoTracking()
        //                        .Where(o => o.STATUS == 1)
        //                    on data.OPERATIONID equals opMap.OPERATIONID
        //                join emp in _dbContext.ADEMPLOYEE.AsNoTracking()
        //                        .Where(w => w.ACTIVE == 1)
        //                    on data.ADEMPCODE equals emp.ADEMPCODE
        //                where data.SYKI == _Syki.SYKIID
        //                      && data.ACTIVE == 1
        //                      && (Emp_Dtl._OpId == null || Emp_Dtl._OpId == 0 || data.OPERATIONID == Emp_Dtl._OpId)
        //                      && (Emp_Dtl._DivId == null || Emp_Dtl._DivId == 0 || data.DIVISIONID == Emp_Dtl._DivId)
        //                select new
        //                {
        //                    data.ADEMPCODE,
        //                    emp.FIRSTNAME,
        //                    emp.LASTNAME,
        //                    data.DIVISIONID,
        //                    data.DIVISION,
        //                    data.DEPARTMENTID,
        //                    data.DEPARTMENT,
        //                    data.ADFUNCTIONALDESIGNATIONID,
        //                    opMap.FLOORID
        //                }
        //            ).ToList();

        //            if (!empDetails.Any())
        //                return new List<SeatEmpCalViewModel>();


        //            // EmployeeCodes IN expression
        //            var employeeCodes = empDetails.Select(e => e.ADEMPCODE).Distinct().ToList();
        //            var empCondition = BuildInExpression<SM_ROSTER_TRN, long>(
        //                x => x.ADEMPCODE, employeeCodes);


        //            // ================================================================
        //            // 2. ROSTER QUERY (SERVER-SIDE)
        //            // ================================================================
        //            var rosterData = _dbContext.SM_ROSTER_TRN.AsNoTracking()
        //                .Where(t => t.STATUS == 1)
        //                .Where(empCondition)
        //                .Where(calCondition)
        //                .ToList()
        //                .GroupBy(x => (x.ADEMPCODE, x.ROSTER_DATE))
        //                .ToDictionary(g => g.Key, g => g.First());


        //            // ================================================================
        //            // 3. SEAT ALLOCATION QUERY (SERVER-SIDE)
        //            // ================================================================
        //            var seatAllocations = (
        //                from sa in _dbContext.SM_SEATALLOCATION_TRN.AsNoTracking()
        //                    .Where(s => s.STATUS == 1)
        //                join rt in _dbContext.SM_ROSTER_TRN.AsNoTracking()
        //                        .Where(empCondition).Where(calCondition)
        //                    on sa.RSTTRNID equals rt.RSTTRNID
        //                join seat in _dbContext.SM_FLOORSEATMST.AsNoTracking()
        //                    on sa.SEATMSTID equals seat.SEATMSTID
        //                select new
        //                {
        //                    rt.ADEMPCODE,
        //                    rt.ROSTER_DATE,
        //                    SeatData = new SeatAllocationViewModel
        //                    {
        //                        SEAT_NAME = seat.SEATNAME,
        //                        SEAT_NO = seat.SEATNO
        //                    }
        //                }
        //            )
        //            .ToList()
        //            .GroupBy(x => (x.ADEMPCODE, x.ROSTER_DATE))
        //            .ToDictionary(g => g.Key, g => g.First().SeatData);


        //            // ================================================================
        //            // 4. FINAL MODEL CREATION
        //            // ================================================================
        //            var finalList = empDetails
        //                .Select(emp => new SeatEmpCalViewModel
        //                {
        //                    ECODE = emp.ADEMPCODE,
        //                    ENAME = (emp.FIRSTNAME + " " + emp.LASTNAME).Trim(),

        //                    DIVISIONID = emp.DIVISIONID,
        //                    DIVISION = emp.DIVISION,
        //                    DEPARTMENTID = emp.DEPARTMENTID,
        //                    DEPARTMENT = emp.DEPARTMENT,
        //                    FN_DESIGNATIONID = emp.ADFUNCTIONALDESIGNATIONID ?? 0,
        //                    FLOORID = emp.FLOORID,

        //                    CAL_LIST = Cal_List.Select(calObj =>
        //                    {
        //                        rosterData.TryGetValue((emp.ADEMPCODE, calObj.CAL_DATE_FORMAT), out var rosterEntry);
        //                        seatAllocations.TryGetValue((emp.ADEMPCODE, calObj.CAL_DATE_FORMAT), out var seatObj);

        //                        return new SeatCalenderViewModel
        //                        {
        //                            CALID = calObj.CALID,
        //                            CAL_DATE = calObj.CAL_DATE,
        //                            IS_OFFDAY = calObj.IS_OFFDAY,
        //                            OFFDAY_COLOR = calObj.OFFDAY_COLOR,
        //                            WORKINGDAY_COLOR = calObj.WORKINGDAY_COLOR,
        //                            WEEK_DAY = calObj.WEEK_DAY,
        //                            IS_CHECKED = rosterEntry != null,
        //                            SEAT_OBJ = seatObj,

        //                            // Custom logic
        //                            IsNewJoinee = IsNewJoineeDay(Convert.ToDateTime(calObj.CAL_DATE), emp.ADEMPCODE),
        //                            IsInActiveEmp = IsInActiveEmployee(Convert.ToDateTime(calObj.CAL_DATE), emp.ADEMPCODE)
        //                        };
        //                    }).ToList()
        //                })
        //                .OrderBy(o => o.DIVISIONID)
        //                .ThenBy(t => t.DEPARTMENTID)
        //                .ThenBy(e => e.ENAME)
        //                .ToList();


        //            return finalList;
        //        }
        //        catch
        //        {
        //            return new List<SeatEmpCalViewModel>();
        //        }
        //    }


        public List<SeatEmpCalViewModel> ViewRosterList(long userId, List<SeatCalenderViewModel> Cal_List, Employee_Details Emp_Dtl)
        {
            List<SeatEmpCalViewModel> _EmpList = new List<SeatEmpCalViewModel>();
            try
            {
                List<long> _desgIds = new List<long>();

                string _ROSTER_ALLOW_DESG = GetParmValByParmName("ROSTER_ALLOW_DESG");  //_dbContext.SM_SETTING_MST.Where(f => f.PARM_NAME == "ROSTER_ALLOW_DESG" && f.STATUS == 1).Select(s => s.PARM_VALUE).FirstOrDefault();

                if (!string.IsNullOrEmpty(_ROSTER_ALLOW_DESG))
                {
                    _desgIds = _ROSTER_ALLOW_DESG.Split(',').Select(s => long.Parse(s)).ToList();
                }

                List<long> _siteIds = new List<long>();
                string _ROSTER_ALLOW_SITE = GetParmValByParmName("ROSTER_ALLOW_SITE");  //_dbContext.SM_SETTING_MST.Where(f => f.PARM_NAME == "ROSTER_ALLOW_SITE" && f.STATUS == 1).Select(s => s.PARM_VALUE).FirstOrDefault();
                if (!string.IsNullOrEmpty(_ROSTER_ALLOW_SITE))
                {
                    _siteIds = _ROSTER_ALLOW_SITE.Split(',').Select(s => long.Parse(s)).ToList();
                }

                /*Below Code Optimized By Aumento as on 25092024 Start*/
                var empDetails = (from _desID in _desgIds
                                  join data in _dbContext.VW_ASSOCIATELVLDETAILS.AsNoTracking()
                                        .Where(x => x.SYKI == _Syki.SYKIID && x.ACTIVE == 1)
                                  on _desID equals data.ADDESIGNATIONID
                                  join _siteId in _siteIds on data.SYSITEID equals _siteId
                                  join _opMap in _dbContext.SM_FLOOR_OPMAP.AsNoTracking().Where(o => o.STATUS == 1)
                                  on data.OPERATIONID equals _opMap.OPERATIONID
                                  join _emp in _dbContext.ADEMPLOYEE.AsNoTracking().Where(w => w.ACTIVE == 1)
                                  on data.ADEMPCODE equals _emp.ADEMPCODE
                                  where (Emp_Dtl._OpId == 0 || Emp_Dtl._OpId == null ? true : data.OPERATIONID == Emp_Dtl._OpId)
                                        && (Emp_Dtl._DivId == 0 || Emp_Dtl._DivId == null ? true : data.DIVISIONID == Emp_Dtl._DivId)
                                  select new
                                  {
                                      _emp.ADEMPCODE,
                                      _emp.FIRSTNAME,
                                      _emp.LASTNAME,
                                      data.DIVISIONID,
                                      data.DIVISION,
                                      data.DEPARTMENTID,
                                      data.DEPARTMENT,
                                      data.ADFUNCTIONALDESIGNATIONID,
                                      _opMap.FLOORID
                                  }).ToList();


                var employeeCodes = empDetails.Select(e => e.ADEMPCODE).ToList();
                var calendarDates = Cal_List.Select(c => c.CAL_DATE_FORMAT).ToList();

                var rosterData = _dbContext.SM_ROSTER_TRN.AsNoTracking()
                                    .Where(t => employeeCodes.Contains(t.ADEMPCODE) && calendarDates.Contains(t.ROSTER_DATE) && t.STATUS == 1)
                                    .ToList();

                var seatAllocations = (from seatAllocation in _dbContext.SM_SEATALLOCATION_TRN.AsNoTracking()
                                       join SRT in _dbContext.SM_ROSTER_TRN.AsNoTracking()
                                           on seatAllocation.RSTTRNID equals SRT.RSTTRNID
                                       join seatMst in _dbContext.SM_FLOORSEATMST.AsNoTracking()
                                           on seatAllocation.SEATMSTID equals seatMst.SEATMSTID
                                       where seatAllocation.STATUS == 1
                                       select new
                                       {
                                           SRT.ADEMPCODE,
                                           SRT.ROSTER_DATE,
                                           SeatData = new SeatAllocationViewModel
                                           {
                                               SEAT_NAME = seatMst.SEATNAME,
                                               SEAT_NO = seatMst.SEATNO
                                           }
                                       }).ToList();

                _EmpList = empDetails.Select(emp => new SeatEmpCalViewModel
                {
                    ECODE = emp.ADEMPCODE,
                    ENAME = (emp.FIRSTNAME + " " + emp.LASTNAME).Trim(),
                    DIVISIONID = emp.DIVISIONID,
                    DIVISION = emp.DIVISION,
                    DEPARTMENTID = emp.DEPARTMENTID,
                    DEPARTMENT = emp.DEPARTMENT,
                    FN_DESIGNATIONID = emp.ADFUNCTIONALDESIGNATIONID ?? 0,
                    FLOORID = emp.FLOORID,

                    CAL_LIST = Cal_List.Select(calObj =>
                    {
                        var rosterEntry = rosterData.FirstOrDefault(t => t.ADEMPCODE == emp.ADEMPCODE && t.ROSTER_DATE == calObj.CAL_DATE_FORMAT);

                        var seatObj = seatAllocations
                                        .Where(s => s.ADEMPCODE == emp.ADEMPCODE && s.ROSTER_DATE == calObj.CAL_DATE_FORMAT)
                                        .Select(s => s.SeatData)
                                        .FirstOrDefault();

                        return new SeatCalenderViewModel
                        {
                            CALID = calObj.CALID,
                            CAL_DATE = calObj.CAL_DATE,
                            IS_OFFDAY = calObj.IS_OFFDAY,
                            OFFDAY_COLOR = calObj.OFFDAY_COLOR,
                            WORKINGDAY_COLOR = calObj.WORKINGDAY_COLOR,
                            WEEK_DAY = calObj.WEEK_DAY,
                            IS_CHECKED = rosterEntry != null,
                            SEAT_OBJ = seatObj,
                            //SR102091 START
                            IsNewJoinee = IsNewJoineeDay(Convert.ToDateTime(calObj.CAL_DATE), emp.ADEMPCODE),
                            IsInActiveEmp = IsInActiveEmployee(Convert.ToDateTime(calObj.CAL_DATE), emp.ADEMPCODE)
                            //SR102091 END
                        };
                    }).ToList()
                })
                .OrderBy(o => o.DIVISIONID)
                .ThenBy(t => t.DEPARTMENTID)
                .ThenBy(e => e.ENAME)
                .ToList();
                /*End*/
            }
            catch (Exception ex)
            {
                _EmpList = new List<SeatEmpCalViewModel>();
            }
            return _EmpList;
        }





        //public async Task<List<SeatEmpCalViewModel>> ViewRosterList(
        //long userId,
        //List<SeatCalenderViewModel> Cal_List,
        //Employee_Details Emp_Dtl)
        //{
        //    string dateCsv = string.Join(",", Cal_List.Select(x => x.CAL_DATE_FORMAT));

        //    var finalList = new List<SeatRosterRawModel>();

        //    using (var conn = _dbContext.Database.GetDbConnection())
        //    {
        //        await conn.OpenAsync();

        //        using (var cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = "GET_ROSTER_DATA";
        //            cmd.CommandType = CommandType.StoredProcedure;

        //            cmd.Parameters.Add(new OracleParameter("p_Syki", _Syki.SYKIID));
        //            cmd.Parameters.Add(new OracleParameter("p_OpId", Emp_Dtl._OpId));
        //            cmd.Parameters.Add(new OracleParameter("p_DivId", Emp_Dtl._DivId));
        //            cmd.Parameters.Add(new OracleParameter("p_DateCsv", dateCsv));

        //            var cursorParam = new OracleParameter("p_Result", OracleDbType.RefCursor)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            cmd.Parameters.Add(cursorParam);

        //            using (var reader = await cmd.ExecuteReaderAsync())
        //            {
        //                while (await reader.ReadAsync())
        //                {
        //                    finalList.Add(new SeatRosterRawModel
        //                    {
        //                        ADEMPCODE = reader["ADEMPCODE"]?.ToString(),
        //                        FIRSTNAME = reader["FIRSTNAME"]?.ToString(),
        //                        LASTNAME = reader["LASTNAME"]?.ToString(),
        //                        DIVISIONID = reader["DIVISIONID"]?.ToString(),
        //                        DIVISION = reader["DIVISION"]?.ToString(),
        //                        DEPARTMENTID = reader["DEPARTMENTID"]?.ToString(),
        //                        DEPARTMENT = reader["DEPARTMENT"]?.ToString(),
        //                        DESIGNATION_ID = reader["ADFUNCTIONALDESIGNATIONID"]?.ToString(),
        //                        FLOORID = reader["FLOORID"]?.ToString(),
        //                        ROSTER_DATE = reader["ROSTER_DATE"]?.ToString(),
        //                        SEAT_NAME = reader["SEATNAME"]?.ToString(),
        //                        SEAT_NO = reader["SEATNO"]?.ToString()
        //                    });
        //                }
        //            }
        //        }
        //    }

        //    return BuildFinalModel(finalList, Cal_List);
        //}

        //public List<SeatEmpCalViewModel> ViewRosterList(
        //        long userId,
        //        List<SeatCalenderViewModel> Cal_List,
        //        Employee_Details Emp_Dtl)
        //{
        //    string dateCsv = string.Join(",", Cal_List.Select(x => x.CAL_DATE_FORMAT));

        //    var finalList = new List<SeatRosterRawModel>();

        //    using (var conn = _dbContext.Database.GetDbConnection())
        //    {
        //        conn.Open();

        //        using (var cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = "GET_ROSTER_DATA1";
        //            cmd.CommandType = CommandType.StoredProcedure;

        //            cmd.Parameters.Add(new OracleParameter("p_Syki", _Syki.SYKIID));
        //            cmd.Parameters.Add(new OracleParameter("p_OpId", Emp_Dtl._OpId));
        //            cmd.Parameters.Add(new OracleParameter("p_DivId", Emp_Dtl._DivId));
        //            cmd.Parameters.Add(new OracleParameter("p_DateCsv", dateCsv));

        //            var cursorParam = new OracleParameter("p_Result", OracleDbType.RefCursor)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            cmd.Parameters.Add(cursorParam);

        //            using (var reader = cmd.ExecuteReader())
        //            {
        //                while ( reader.Read())
        //                {
        //                    finalList.Add(new SeatRosterRawModel
        //                    {
        //                        ADEMPCODE = reader["ADEMPCODE"]?.ToString(),
        //                        FIRSTNAME = reader["FIRSTNAME"]?.ToString(),
        //                        LASTNAME = reader["LASTNAME"]?.ToString(),
        //                        DIVISIONID = reader["DIVISIONID"]?.ToString(),
        //                        DIVISION = reader["DIVISION"]?.ToString(),
        //                        DEPARTMENTID = reader["DEPARTMENTID"]?.ToString(),
        //                        DEPARTMENT = reader["DEPARTMENT"]?.ToString(),
        //                        DESIGNATION_ID = reader["ADFUNCTIONALDESIGNATIONID"]?.ToString(),
        //                        FLOORID = reader["FLOORID"]?.ToString(),
        //                        ROSTER_DATE = reader["ROSTER_DATE"]?.ToString(),
        //                        SEAT_NAME = reader["SEATNAME"]?.ToString(),
        //                        SEAT_NO = reader["SEATNO"]?.ToString()
        //                    });
        //                }
        //            }
        //        }
        //    }

        //    return BuildFinalModel(finalList, Cal_List);
        //}



        //private List<SeatEmpCalViewModel> BuildFinalModel(
        //    List<SeatRosterRawModel> rawList,
        //    List<SeatCalenderViewModel> Cal_List)
        //{
        //    var final = rawList
        //        .GroupBy(x => x.ADEMPCODE)
        //        .Select(g =>
        //        {
        //            var first = g.First();

        //            return new SeatEmpCalViewModel
        //            {
        //                ECODE = Convert.ToInt64(first.ADEMPCODE),
        //                ENAME = (first.FIRSTNAME + " " + first.LASTNAME).Trim(),
        //                DIVISIONID = Convert.ToInt64(first.DIVISIONID),
        //                DIVISION = first.DIVISION,
        //                DEPARTMENTID = Convert.ToInt64(first.DEPARTMENTID),
        //                DEPARTMENT = first.DEPARTMENT,
        //                FN_DESIGNATIONID = Convert.ToInt64(first.DESIGNATION_ID),
        //                FLOORID = Convert.ToInt64(first.FLOORID),

        //                CAL_LIST = Cal_List.Select(cal =>
        //                {
        //                    var row = g.FirstOrDefault(r => Convert.ToDateTime(r.ROSTER_DATE) == cal.CAL_DATE_FORMAT);

        //                    return new SeatCalenderViewModel
        //                    {
        //                        CALID = cal.CALID,
        //                        CAL_DATE = cal.CAL_DATE,
        //                        IS_OFFDAY = cal.IS_OFFDAY,
        //                        OFFDAY_COLOR = cal.OFFDAY_COLOR,
        //                        WORKINGDAY_COLOR = cal.WORKINGDAY_COLOR,
        //                        WEEK_DAY = cal.WEEK_DAY,
        //                        IS_CHECKED = row != null,
        //                        SEAT_OBJ = row != null ? new SeatAllocationViewModel
        //                        {
        //                            SEAT_NAME = row.SEAT_NAME,
        //                            SEAT_NO = row.SEAT_NO
        //                        } : null,

        //                        IsNewJoinee = IsNewJoineeDay(Convert.ToDateTime(cal.CAL_DATE), Convert.ToInt64(first.ADEMPCODE)),
        //                        IsInActiveEmp = IsInActiveEmployee(Convert.ToDateTime(cal.CAL_DATE), Convert.ToInt64(first.ADEMPCODE))
        //                    };
        //                }).ToList()
        //            };
        //        })
        //        .OrderBy(o => o.DIVISIONID)
        //        .ThenBy(t => t.DEPARTMENTID)
        //        .ThenBy(e => e.ENAME)
        //        .ToList();

        //    return final;
        //}


        public string GetMapFilename(long floorID)
        {
            return (string)_dbContext.SM_FLOORMASTER.Where(f => f.SM_FLOOR_ID == floorID).Select(s => s.MAP_FILE_NAME).FirstOrDefault();
        }
        //// Master
        public List<Employee_Details> AutocompleteEmployee(string Key)
        {
            List<Employee_Details> portalUserDtos = new List<Employee_Details>();
            portalUserDtos = (from userdata in _dbContext.ADEMPLOYEE
                              where (userdata.ADEMPCODE.ToString().StartsWith(Key) || userdata.FIRSTNAME.ToUpper().Contains(Key.ToUpper()) || userdata.LASTNAME.ToUpper().Contains(Key.ToUpper()))
                              select new Employee_Details
                              {
                                  _ECode = userdata.ADEMPCODE,
                                  _EFirstName = userdata.FIRSTNAME,
                                  _ELastName = userdata.LASTNAME,
                              }).ToList();
            return portalUserDtos;
        }
        public List<SeatAllocationViewModel> AutocompleteSeat(string Key, long eCode)
        {
            List<SeatAllocationViewModel> portalSeatDtos = new List<SeatAllocationViewModel>();
            long? _opId = _dbContext.VW_ASSOCIATELVLDETAILS.Where(v => v.SYKI == _Syki.SYKIID && v.ADEMPCODE == eCode).Select(s => s.OPERATIONID).FirstOrDefault();
            portalSeatDtos = (from seatdata in _dbContext.SM_FLOORSEATMST
                              join opMapp in _dbContext.SM_FLOOR_OPMAP.Where(f => f.STATUS == 1) on seatdata.FLOORID equals opMapp.FLOORID
                              where (seatdata.SEATNO.ToString().StartsWith(Key)
                              || seatdata.SEATNAME.ToUpper().Contains(Key.ToUpper()))
                              && seatdata.STATUS == 1
                              && opMapp.OPERATIONID == _opId
                              select new SeatAllocationViewModel
                              {
                                  SEAT_ID = seatdata.SEATMSTID,
                                  SEAT_NO = seatdata.SEATNO,
                                  SEAT_NAME = seatdata.SEATNAME,
                              }).OrderBy(o => o.SEAT_NO).ToList();
            return portalSeatDtos;
        }

        public List<FixSeatViewModel> GetFixSeatList()
        {
            List<FixSeatViewModel> newList = new List<FixSeatViewModel>();
            newList = (from data in _dbContext.SM_FIXSEATMAP
                       join _smSeat in _dbContext.SM_FLOORSEATMST on data.SEATMSTID equals _smSeat.SEATMSTID
                       join _emp in _dbContext.ADEMPLOYEE on data.ADEMPCODE equals _emp.ADEMPCODE
                       //where data.STATUS == 1
                       select new FixSeatViewModel
                       {
                           FIXSEATMAPID = data.FIXSEATMAPID,
                           SEATMSTID = data.SEATMSTID,
                           SEATNO = _smSeat.SEATNAME + "-" + _smSeat.SEATNO,
                           ADEMPCODE = data.ADEMPCODE,
                           ADEMPNAME = _emp.FIRSTNAME + " " + _emp.LASTNAME,
                           ADDEDBY = data.ADDEDBY,
                           ADDEDON = data.ADDEDON,
                           STATUS = data.STATUS
                       }).OrderBy(o => o.ADEMPCODE).ToList();
            return newList;
        }

        public Tuple<short, string> SaveFixMapping(long mapId, long eCode, string seatNo, long ActionBy, short status)
        {
            short retVal = 0;
            string msg = "";
            Tuple<short, string> _tuple = new Tuple<short, string>(retVal, msg);
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    int FlagAdd = 0;
                    long seatID = (long)AutocompleteSeat(seatNo, eCode).Select(s => s.SEAT_ID).FirstOrDefault();
                    SM_FIXSEATMAP BCM = new SM_FIXSEATMAP();
                    //// Alreadt exist
                    if (_dbContext.SM_FIXSEATMAP.Count(a => a.ADEMPCODE == eCode && a.FIXSEATMAPID != mapId) > 0)
                    {
                        retVal = 2;
                        transaction.Rollback();
                        _tuple = new Tuple<short, string>(retVal, msg);
                        return _tuple;
                    }
                    if (_dbContext.SM_FIXSEATMAP.Count(a => a.SEATMSTID == seatID && a.FIXSEATMAPID != mapId) > 0)
                    {
                        retVal = 2;
                        transaction.Rollback();
                        _tuple = new Tuple<short, string>(retVal, msg);
                        return _tuple;
                    }
                    ////
                    BCM = _dbContext.SM_FIXSEATMAP.Where(a => a.FIXSEATMAPID == mapId).FirstOrDefault();
                    if (BCM == null)
                    {
                        BCM = new SM_FIXSEATMAP();
                        if (_dbContext.SM_FIXSEATMAP.Count() == 0)
                        {
                            BCM.FIXSEATMAPID = 1;
                        }
                        else
                        {
                            BCM.FIXSEATMAPID = _dbContext.SM_FIXSEATMAP.Max(m => m.FIXSEATMAPID) + 1;
                        }
                        FlagAdd = 1;
                    }
                    BCM.ADEMPCODE = eCode;
                    BCM.SEATMSTID = seatID;
                    BCM.STATUS = status;
                    if (FlagAdd == 1)
                    {
                        BCM.ADDEDBY = ActionBy;
                        BCM.ADDEDON = DateTime.Now;
                    }
                    else
                    {
                        BCM.UPDATEDBY = ActionBy;
                        BCM.UPDATEDON = DateTime.Now;
                    }
                    _dbContext.Entry(BCM).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                    _dbContext.SaveChanges();
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

        public short UpdateStatus(long mapId, long ActionBy, short status)
        {
            short retVal = 0;
            if (mapId > 0)
            {
                SM_FIXSEATMAP DT = _dbContext.SM_FIXSEATMAP.Where(x => x.FIXSEATMAPID == mapId).FirstOrDefault();
                if (DT != null)
                {
                    DT.STATUS = status;
                    DT.UPDATEDBY = ActionBy;
                    DT.UPDATEDON = DateTime.Now;
                    _dbContext.Entry(DT).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }

        public List<SelectListViewModel> GetOrgLevelList(long typeId)
        {
            var iList = (from data in _dbContext.ADORGLEVEL
                         where data.ACTIVE == 1 && data.SYKIID == _Syki.SYKIID && data.ADORGLEVELTYPEID == typeId
                         select new SelectListViewModel
                         {
                             Value = data.ADORGLEVELID,
                             Text = data.LEVELDESCRIP
                         }).OrderBy(b => b.Text).ToList();
            return iList;
        }

        public List<SeatingReportViewModel> GetSeatingReport(SearchSeatViewModel SSVM)
        {
            DateTime ReqDateFrom = DateTime.Now.Date;
            DateTime ReqDateTo = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(SSVM.FromDate))
            {
                ReqDateFrom = DateTime.ParseExact(SSVM.FromDate, "dd-MMM-yyyy", null);
            }
            if (!string.IsNullOrEmpty(SSVM.ToDate))
            {
                ReqDateTo = DateTime.ParseExact(SSVM.ToDate + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
            }

            var iList = (from SRT in _dbContext.SM_ROSTER_TRN.Where(w => w.STATUS == 1)
                         join seat_all in _dbContext.SM_SEATALLOCATION_TRN.Where(c => c.STATUS == 1) on SRT.RSTTRNID equals seat_all.RSTTRNID
                         join seat_mst in _dbContext.SM_FLOORSEATMST on seat_all.SEATMSTID equals seat_mst.SEATMSTID
                         join floor_mst in _dbContext.SM_FLOORMASTER on seat_mst.FLOORID equals floor_mst.SM_FLOOR_ID
                         join _emp in _dbContext.ADEMPLOYEE on SRT.ADEMPCODE equals _emp.ADEMPCODE
                         join VW in _dbContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == _Syki.SYKIID) on _emp.ADEMPCODE equals VW.ADEMPCODE
                         where (SSVM.OpId == 0 ? 1 == 1 : VW.OPERATIONID == SSVM.OpId)
                               && (SSVM.DivId == 0 ? 1 == 1 : VW.DIVISIONID == SSVM.DivId)
                               && (SSVM.Employee == 0 ? 1 == 1 : _emp.ADEMPCODE == SSVM.Employee)
                               && (SSVM.FromDate == null ? 1 == 1 : SRT.ROSTER_DATE >= ReqDateFrom)
                               && (SSVM.ToDate == null ? 1 == 1 : SRT.ROSTER_DATE <= ReqDateTo)
                         select new SeatingReportViewModel
                         {
                             SEATINGID = seat_all.ALLOCATIONTRNID,
                             DATE = SRT.ROSTER_DATE,
                             SEATNAME = seat_mst.SEATNAME,
                             SEATNO = seat_mst.SEATNO,
                             BUILDING = floor_mst.BUILDING,
                             FLOOR = floor_mst.FLOOR_NAME,
                             ADEMPNAME = _emp.FIRSTNAME + " " + _emp.LASTNAME,
                             ADEMPCODE = _emp.ADEMPCODE,
                             OPERATION = VW.OPERATION,
                             DIVISION = VW.DIVISION
                         }).OrderBy(f => f.BUILDING).ThenBy(e => e.FLOOR).ThenBy(o => o.DATE).ThenBy(t => t.OPERATION).ThenBy(b => b.ADEMPNAME).ToList();
            return iList;
        }


        public List<DivWiseSeatViewModel> GetDivisionWiseSeatList()
        {
            List<DivWiseSeatViewModel> newList = new List<DivWiseSeatViewModel>();


            //newList = (from data in _dbContext.SM_DIVISIONWISESEATDETAILS
            //           join _Div in _dbContext.ADORGLEVEL on data.ADORGLEVELID equals _Div.ADORGLEVELID
            //           join _floorOpMapp in _dbContext.SM_FLOOR_OPMAP on data.FLOOR_OPMAPPID equals _floorOpMapp.FLOOR_OPMAPPID
            //           join _OP in _dbContext.ADORGLEVEL on _floorOpMapp.OPERATIONID equals _OP.ADORGLEVELID
            //           select new DivWiseSeatViewModel
            //           {
            //               DWS_DTLID = data.DWS_DTLID,
            //               OPERATIONID = _OP.ADORGLEVELID,
            //               OPERATION = _OP.LEVELDESCRIP,
            //               OPERATION_SEAT = _floorOpMapp.SEATCAPACITY,
            //               DIVISIONID = _Div.ADORGLEVELID,
            //               DIVISION = _Div.LEVELDESCRIP,
            //               PHYSICALSEAT = data.PHYSICALSEAT,
            //               ADDEDBY = data.ADDEDBY,
            //               ADDEDON = data.ADDEDON,
            //               STATUS = data.STATUS == 1 ? true : false
            //           }).OrderBy(o => o.OPERATIONID).ToList();


            newList = (from data in _dbContext.SM_DIVISIONWISESEATDETAILS
                       join _Div in _dbContext.ADORGLEVEL on data.ADORGLEVELID equals _Div.ADORGLEVELID
                       join _floorOpMapp in _dbContext.SM_FLOOR_OPMAP on data.FLOOR_OPMAPPID equals _floorOpMapp.FLOOR_OPMAPPID
                       join _OP in _dbContext.ADORGLEVEL on _floorOpMapp.OPERATIONID equals _OP.ADORGLEVELID
                       select new
                       {
                           data.DWS_DTLID,
                           data.PHYSICALSEAT,
                           data.ADDEDBY,
                           data.ADDEDON,
                           data.STATUS,
                           DivID = _Div.ADORGLEVELID,
                           Div = _Div.LEVELDESCRIP,
                           opID = _OP.ADORGLEVELID,
                           opName = _OP.LEVELDESCRIP,
                           seatCap = _floorOpMapp.SEATCAPACITY,
                           data.ISSEATAUTOUPDATED
                       }
                       ) //still sql
                       .AsEnumerable() //switch to linq to objects
                       .Select(x => new DivWiseSeatViewModel
                       {
                           DWS_DTLID = x.DWS_DTLID,
                           OPERATIONID = x.opID,
                           OPERATION = x.opName,
                           OPERATION_SEAT = x.seatCap,
                           DIVISIONID = x.DivID,
                           DIVISION = x.Div,
                           PHYSICALSEAT = x.PHYSICALSEAT,
                           ADDEDBY = x.ADDEDBY,
                           ADDEDON = x.ADDEDON,
                           STATUS = x.STATUS == 1,
                           IsSeatAutoUpdated = x.ISSEATAUTOUPDATED //Added by Aumento :: SR100223
                       })
            .OrderBy(o => o.OPERATIONID).ToList();



            //newList = (from DWS_DTLID in _dbContext.SM_DIVISIONWISESEATDETAILS.GroupBy(g => g.FLOOR_OPMAPPID).Select(s => s.Key)
            //           join data in _dbContext.SM_DIVISIONWISESEATDETAILS on DWS_DTLID equals data.DWS_DTLID
            //           join _OP in _dbContext.ADORGLEVEL on data.DWS_DTLID equals _OP.ADORGLEVELID
            //           select new DivWiseSeatViewModel
            //           {
            //               DWS_DTLID = data.DWS_DTLID,
            //               FLOOR_OPMAPPID = data.FLOOR_OPMAPPID,
            //               OPERATION = _OP.LEVELDESCRIP,
            //               DIV_LIST = (from _Div in _dbContext.ADORGLEVEL.Where(d => d.ADORGLEVELID == data.ADORGLEVELID)
            //                           select new DivisionSeatCapacity
            //                           {
            //                               DIVISIONID = _Div.ADORGLEVELID,
            //                               DIVISION = _Div.LEVELDESCRIP,
            //                               PHYSICALSEAT = data.PHYSICALSEAT,
            //                           }).ToList(),
            //               ADDEDBY = data.ADDEDBY,
            //               ADDEDON = data.ADDEDON,
            //               STATUS = data.STATUS == 1 ? true : false,           
            //           }).OrderBy(o => o.FLOOR_OPMAPPID).ToList();
            return newList;
        }

        public DivWiseSeatViewModel GetDivisionWiseSeatById(long opID)
        {
            DivWiseSeatViewModel newObj = new DivWiseSeatViewModel();

            //newObj = (from _floorOpMapp in _dbContext.SM_FLOOR_OPMAP.Where(f => f.OPERATIONID == opID)
            //          join _OP in _dbContext.ADORGLEVEL on _floorOpMapp.OPERATIONID equals _OP.ADORGLEVELID
            //          select new DivWiseSeatViewModel
            //          {
            //              OPERATIONID = _OP.ADORGLEVELID,
            //              OPERATION = _OP.LEVELDESCRIP,
            //              OPERATION_SEAT = _floorOpMapp.SEATCAPACITY,
            //              DIV_LIST = (from data in _dbContext.SM_DIVISIONWISESEATDETAILS.Where(w => w.FLOOR_OPMAPPID == _floorOpMapp.FLOOR_OPMAPPID)
            //                          join _Div in _dbContext.ADORGLEVEL on data.ADORGLEVELID equals _Div.ADORGLEVELID
            //                          select new DivisionSeatCapacity
            //                          {
            //                              DWSID = data.DWS_DTLID,
            //                              DIVISIONID = _Div.ADORGLEVELID,
            //                              DIVISION = _Div.LEVELDESCRIP,
            //                              PHYSICALSEAT = data.PHYSICALSEAT,
            //                              STATUS = data.STATUS == 1 ? true : false,
            //                              IsSeatAutoUpdated = data.ISSEATAUTOUPDATED, //Added by Aumento :: SR100223
            //                              isSeatautoupdateforOP = _floorOpMapp.ISSEATAUTOUPDATED //Added by Aumento :: SR100223
            //                          }).ToList(),
            //          }).OrderBy(o => o.OPERATIONID).FirstOrDefault();



            //newObj = (from _floorOpMapp in _dbContext.SM_FLOOR_OPMAP.Where(f => f.OPERATIONID == opID)
            //          join _OP in _dbContext.ADORGLEVEL on _floorOpMapp.OPERATIONID equals _OP.ADORGLEVELID
            //          select new
            //          {
            //              _floorOpMapp,
            //              _OP
            //          }).AsEnumerable()
            //          .Select(x=> new DivWiseSeatViewModel
            //          {
            //              OPERATIONID = x._OP.ADORGLEVELID,
            //              OPERATION = x._OP.LEVELDESCRIP,
            //              OPERATION_SEAT = x._floorOpMapp.SEATCAPACITY,
            //              DIV_LIST = (from data in _dbContext.SM_DIVISIONWISESEATDETAILS.Where(w => w.FLOOR_OPMAPPID == x._floorOpMapp.FLOOR_OPMAPPID)
            //                          join _Div in _dbContext.ADORGLEVEL on data.ADORGLEVELID equals _Div.ADORGLEVELID
            //                          select new DivisionSeatCapacity
            //                          {
            //                              DWSID = data.DWS_DTLID,
            //                              DIVISIONID = _Div.ADORGLEVELID,
            //                              DIVISION = _Div.LEVELDESCRIP,
            //                              PHYSICALSEAT = data.PHYSICALSEAT,
            //                              STATUS = data.STATUS == 1,
            //                          }).ToList(),
            //          })
            //          .OrderBy(o => o.OPERATIONID).FirstOrDefault();



            newObj = (from _floorOpMapp in _dbContext.SM_FLOOR_OPMAP.Where(f => f.OPERATIONID == opID)
                      join _OP in _dbContext.ADORGLEVEL on _floorOpMapp.OPERATIONID equals _OP.ADORGLEVELID
                      select new
                      {
                          _floorOpMapp,
                          _OP,
                          DIV_LIST = (from data in _dbContext.SM_DIVISIONWISESEATDETAILS.Where(w => w.FLOOR_OPMAPPID == _floorOpMapp.FLOOR_OPMAPPID)
                                      join _Div in _dbContext.ADORGLEVEL on data.ADORGLEVELID equals _Div.ADORGLEVELID
                                      select new
                                      {
                                          data.DWS_DTLID,
                                          _Div.ADORGLEVELID,
                                          _Div.LEVELDESCRIP,
                                          data.PHYSICALSEAT,
                                          data.STATUS,
                                          data.ISSEATAUTOUPDATED,
                                      }).ToList()
                      })
                      .AsEnumerable()
                      .Select(x => new DivWiseSeatViewModel
                      {
                          OPERATIONID = x._OP.ADORGLEVELID,
                          OPERATION = x._OP.LEVELDESCRIP,
                          OPERATION_SEAT = x._floorOpMapp.SEATCAPACITY,
                          DIV_LIST = x.DIV_LIST.Select(d => new DivisionSeatCapacity
                          {
                              DWSID = d.DWS_DTLID,
                              DIVISIONID = d.ADORGLEVELID,
                              DIVISION = d.LEVELDESCRIP,
                              PHYSICALSEAT = d.PHYSICALSEAT,
                              STATUS = d.STATUS == 1,
                              IsSeatAutoUpdated = d.ISSEATAUTOUPDATED, //Added by Aumento :: SR100223
                              isSeatautoupdateforOP = x._floorOpMapp.ISSEATAUTOUPDATED //Added by Aumento :: SR100223
                          }).ToList()
                      })
                      .OrderBy(o => o.OPERATIONID)
    .FirstOrDefault();




            //newObj = (from data in _dbContext.SM_DIVISIONWISESEATDETAILS
            //          join _Div in _dbContext.ADORGLEVEL on data.ADORGLEVELID equals _Div.ADORGLEVELID
            //          join _floorOpMapp in _dbContext.SM_FLOOR_OPMAP on data.FLOOR_OPMAPPID equals _floorOpMapp.FLOOR_OPMAPPID
            //          join _OP in _dbContext.ADORGLEVEL on _floorOpMapp.OPERATIONID equals _OP.ADORGLEVELID
            //          where _floorOpMapp.OPERATIONID == opID
            //          select new DivWiseSeatViewModel
            //          {
            //              DWS_DTLID = data.DWS_DTLID,
            //              OPERATIONID = _OP.ADORGLEVELID,
            //              OPERATION = _OP.LEVELDESCRIP,
            //              DIVISIONID = _Div.ADORGLEVELID,
            //              DIVISION = _Div.LEVELDESCRIP,
            //              PHYSICALSEAT = data.PHYSICALSEAT,
            //              ADDEDBY = data.ADDEDBY,
            //              ADDEDON = data.ADDEDON,
            //              STATUS = data.STATUS == 1 ? true : false,
            //          }).OrderBy(o => o.OPERATIONID).FirstOrDefault();

            //newObj = (from DWS_DTLID in _dbContext.SM_DIVISIONWISESEATDETAILS.Where(w=>w.FLOOR_OPMAPPID == dtlId).GroupBy(g => g.FLOOR_OPMAPPID).Select(s => s.Key)
            //           join data in _dbContext.SM_DIVISIONWISESEATDETAILS on DWS_DTLID equals data.DWS_DTLID
            //           join _OP in _dbContext.ADORGLEVEL on data.FLOOR_OPMAPPID equals _OP.ADORGLEVELID
            //           select new DivWiseSeatViewModel
            //           {
            //               DWS_DTLID = data.DWS_DTLID,
            //               FLOOR_OPMAPPID = data.FLOOR_OPMAPPID,
            //               OPERATION = _OP.LEVELDESCRIP,
            //               DIV_LIST = (from _Div in _dbContext.SM_DIVISIONWISESEATDETAILS.Where(d => d.FLOOR_OPMAPPID == data.FLOOR_OPMAPPID)
            //                           join _DivName in _dbContext.ADORGLEVEL on _Div.ADORGLEVELID equals _DivName.ADORGLEVELID
            //                           select new DivisionSeatCapacity
            //                           {
            //                               DIVISIONID = _Div.ADORGLEVELID,
            //                               DIVISION = _DivName.LEVELDESCRIP,
            //                               PHYSICALSEAT = data.PHYSICALSEAT,
            //                           }).ToList(),
            //               ADDEDBY = data.ADDEDBY,
            //               ADDEDON = data.ADDEDON,
            //               STATUS = data.STATUS == 1 ? true : false,
            //           }).OrderBy(o => o.FLOOR_OPMAPPID).FirstOrDefault();
            return newObj;
        }

        public Tuple<short, string> SaveDivisionWiseSeat(DivWiseSeatViewModel Model, long ActionBy)
        {
            short retVal = 0;
            string msg = "";
            long floorOpMappId = 0;
            Tuple<short, string> _tuple = new Tuple<short, string>(retVal, msg);
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    SM_FLOOR_OPMAP SFO = _dbContext.SM_FLOOR_OPMAP.Where(w => w.OPERATIONID == Model.OPERATIONID).FirstOrDefault();
                    if (SFO != null)
                    {
                        floorOpMappId = SFO.FLOOR_OPMAPPID;
                    }
                    if (Model.DIV_LIST != null)
                    {
                        int divPhySeatCount = Model.DIV_LIST.Sum(b => b.PHYSICALSEAT);
                        if (divPhySeatCount > SFO.SEATCAPACITY)
                        {
                            retVal = 3;
                            transaction.Rollback();
                            return new Tuple<short, string>(retVal, msg);
                        }
                        foreach (DivisionSeatCapacity obj in Model.DIV_LIST)
                        {
                            int FlagAdd = 0;
                            SM_DIVISIONWISESEATDETAILS BCM = null;
                            if (obj.DWSID > 0)
                            {
                                var checkStatus = _dbContext.SM_DIVISIONWISESEATDETAILS.Count(x => x.ADORGLEVELID == obj.DIVISIONID && x.FLOOR_OPMAPPID == floorOpMappId && x.DWS_DTLID != obj.DWSID) > 0;

                                //if (_dbContext.SM_DIVISIONWISESEATDETAILS.Any(x => x.ADORGLEVELID == obj.DIVISIONID && x.FLOOR_OPMAPPID == floorOpMappId && x.DWS_DTLID != obj.DWSID))
                                if (checkStatus)
                                {
                                    retVal = 2;
                                    transaction.Rollback();
                                    return new Tuple<short, string>(retVal, msg);
                                }
                                BCM = _dbContext.SM_DIVISIONWISESEATDETAILS.Where(a => a.DWS_DTLID == obj.DWSID).FirstOrDefault();
                            }
                            else
                            {
                                var checkStatus = _dbContext.SM_DIVISIONWISESEATDETAILS.Count(x => x.ADORGLEVELID == obj.DIVISIONID && x.FLOOR_OPMAPPID == floorOpMappId) > 0;

                                //if (_dbContext.SM_DIVISIONWISESEATDETAILS.Any(x => x.ADORGLEVELID == obj.DIVISIONID && x.FLOOR_OPMAPPID == floorOpMappId))
                                if (checkStatus)
                                {
                                    retVal = 2;
                                    transaction.Rollback();
                                    return new Tuple<short, string>(retVal, msg);
                                }
                                //BCM = _dbContext.SM_DIVISIONWISESEATDETAILS.Where(a => a.ADORGLEVELID == obj.DIVISIONID && a.FLOOR_OPMAPPID == floorOpMappId).FirstOrDefault();
                            }
                            if (BCM == null)
                            {
                                BCM = new SM_DIVISIONWISESEATDETAILS();
                                if (_dbContext.SM_DIVISIONWISESEATDETAILS.Count() == 0)
                                {
                                    BCM.DWS_DTLID = 1;
                                }
                                else
                                {
                                    BCM.DWS_DTLID = _dbContext.SM_DIVISIONWISESEATDETAILS.Max(m => m.DWS_DTLID) + 1;
                                }
                                FlagAdd = 1;
                            }

                            if (floorOpMappId > 0)
                            {
                                BCM.FLOOR_OPMAPPID = (long)floorOpMappId;
                                BCM.ADORGLEVELID = obj.DIVISIONID;
                                BCM.PHYSICALSEAT = obj.PHYSICALSEAT;
                                BCM.STATUS = obj.STATUS ? (short)1 : (short)0;
                                BCM.ISSEATAUTOUPDATED = obj.IsSeatAutoUpdated; //Added by Aumento :: SR100223
                                if (FlagAdd == 1)
                                {
                                    BCM.ADDEDBY = ActionBy;
                                    BCM.ADDEDON = DateTime.Now;
                                }
                                else
                                {
                                    BCM.UPDATEDBY = ActionBy;
                                    BCM.UPDATEDON = DateTime.Now;
                                }
                                _dbContext.Entry(BCM).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                                _dbContext.SaveChanges();
                                retVal = 1;
                            }
                        }
                    }
                    transaction.Commit();
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

        public List<SelectListViewModel> GetFloorOperationList()
        {
            var iList = from data in _dbContext.SM_FLOOR_OPMAP
                        join _op in _dbContext.ADORGLEVEL on data.OPERATIONID equals _op.ADORGLEVELID
                        where data.STATUS == 1
                        select new SelectListViewModel
                        {
                            Value = data.OPERATIONID,
                            Text = _op.LEVELDESCRIP
                        };
            return iList.ToList();
        }

        public List<SelectListViewModel> BindDivision(long op_Id)
        {
            List<long> _siteIds = new List<long>();
            string _ROSTER_ALLOW_SITE = GetParmValByParmName("ROSTER_ALLOW_SITE");
            if (!string.IsNullOrEmpty(_ROSTER_ALLOW_SITE))
            {
                _siteIds = _ROSTER_ALLOW_SITE.Split(',').Select(s => long.Parse(s)).ToList();
            }
            List<SelectListViewModel> iList = new List<SelectListViewModel>();
            //iList = (from data in _dbContext.VW_ASSOCIATELVLDETAILS.Where(e => e.ACTIVE == 1
            //             && e.SYKI == _Syki.SYKIID 
            //             && e.DIVISIONID != null 
            //             && e.DIVISIONID != 0
            //             && e.OPERATIONID == (op_Id == 0 ? e.OPERATIONID : op_Id)                         
            //             )
            //         join _siteId in _siteIds on data.SYSITEID equals _siteId
            //         select new SelectListViewModel
            //         {
            //             Value = (long)data.DIVISIONID,
            //             Text = data.DIVISION
            //         }).Distinct().ToList();
            //return iList;


            var query = _dbContext.VW_ASSOCIATELVLDETAILS
                    .AsNoTracking()
                    .Where(e => e.ACTIVE == 1
                             && e.SYKI == _Syki.SYKIID
                             && e.DIVISIONID != null
                             && e.DIVISIONID != 0
                             && (op_Id == 0 || e.OPERATIONID == op_Id));

            if (_siteIds.Count > 0)
            {
                // If SYSITEID is long?
                //query = query.Where(e => _siteIds.Contains(e.SYSITEID));

                // If SYSITEID is long? instead:
                var siteIdsNullable = _siteIds.Select(x => (long?)x).ToList();
                query = query.Where(e => siteIdsNullable.Contains(e.SYSITEID));
            }

            return query
                .Select(e => new SelectListViewModel
                {
                    Value = e.DIVISIONID.Value,
                    Text = e.DIVISION
                })
                .Distinct()
                .ToList();



        }

        public int GetFloorOperationSeatCount(long opId)
        {
            return _dbContext.SM_FLOOR_OPMAP.Where(w => w.OPERATIONID == opId).Select(s => s.SEATCAPACITY).FirstOrDefault();
        }

        public List<SeatEmpCalViewModel> GetDivisionWiseCountReport(List<SeatCalenderViewModel> CAL_LIST)
        {
            List<long> _siteIds = new List<long>();
            string _ROSTER_ALLOW_SITE = GetParmValByParmName("ROSTER_ALLOW_SITE");
            if (!string.IsNullOrEmpty(_ROSTER_ALLOW_SITE))
            {
                _siteIds = _ROSTER_ALLOW_SITE.Split(',').Select(s => long.Parse(s)).ToList();
            }
            //var _DivisionList = (from data in _dbContext.VW_ASSOCIATELVLDETAILS.Where(e => e.ACTIVE == 1
            //                    && e.SYKI == _Syki.SYKIID && e.DIVISIONID != null && e.DIVISIONID != 0)
            //                     join _siteId in _siteIds on data.SYSITEID equals _siteId
            //                     join _floorOpMap in _dbContext.SM_FLOOR_OPMAP on data.OPERATIONID equals _floorOpMap.OPERATIONID
            //                     select new
            //                     {
            //                         DIVISIONID = data.DIVISIONID,
            //                         DIVISION = data.DIVISION,
            //                         OPERATIONID = data.OPERATIONID,
            //                         OPERATION = data.OPERATION,
            //                     }).Distinct().ToList();


            var _DivisionList = (from data in _dbContext.VW_ASSOCIATELVLDETAILS
                                 where data.ACTIVE == 1
                                    && data.SYKI == _Syki.SYKIID
                                    && data.DIVISIONID != null
                                    && data.DIVISIONID != 0
                                    && data.SYSITEID.HasValue
                                    && _siteIds.Contains(data.SYSITEID.Value)
                                 join _floorOpMap in _dbContext.SM_FLOOR_OPMAP
                                     on data.OPERATIONID equals _floorOpMap.OPERATIONID
                                 select new
                                 {
                                     DIVISIONID = data.DIVISIONID,
                                     DIVISION = data.DIVISION,
                                     OPERATIONID = data.OPERATIONID,
                                     OPERATION = data.OPERATION,
                                 }).Distinct().ToList();


            List<SeatEmpCalViewModel> newList = new List<SeatEmpCalViewModel>();
            newList = (from data in _DivisionList
                       select new SeatEmpCalViewModel
                       {
                           DIVISIONID = (long)data.DIVISIONID,
                           DIVISION = data.DIVISION,
                           OPERATIONID = (long)data.OPERATIONID,
                           OPERATION = data.OPERATION,
                           CAL_LIST = (from calObj in CAL_LIST
                                       select new SeatCalenderViewModel
                                       {
                                           CALID = calObj.CALID,
                                           CAL_DATE = calObj.CAL_DATE,
                                           IS_OFFDAY = calObj.IS_OFFDAY,
                                           OFFDAY_COLOR = calObj.OFFDAY_COLOR,
                                           WORKINGDAY_COLOR = calObj.WORKINGDAY_COLOR,
                                           WEEK_DAY = calObj.WEEK_DAY,
                                           DATE_WISE_COUNT = (from SRT in _dbContext.SM_ROSTER_TRN.Where(w => w.ROSTER_DATE == calObj.CAL_DATE_FORMAT && w.STATUS == 1)
                                                              join seat_all in _dbContext.SM_SEATALLOCATION_TRN.Where(c => c.STATUS == 1) on SRT.RSTTRNID equals seat_all.RSTTRNID
                                                              join VW in _dbContext.VW_ASSOCIATELVLDETAILS.Where(v => v.SYKI == _Syki.SYKIID) on SRT.ADEMPCODE equals VW.ADEMPCODE
                                                              where VW.DIVISIONID == data.DIVISIONID && VW.OPERATIONID == data.OPERATIONID
                                                              select SRT.RSTTRNID).Count(),
                                       }).ToList(),
                       }).OrderBy(o => o.OPERATION).ThenBy(t => t.DIVISION).ToList();
            return newList;
        }

        public List<SelectListViewModel> GetFloorList()
        {
            var iList = (from data in _dbContext.SM_FLOORMASTER
                         where data.STATUS == 1
                         select data).ToList();

            List<SelectListViewModel> newList = new List<SelectListViewModel>();
            foreach (var obj in iList)
            {
                newList.Add(new SelectListViewModel
                {
                    Value = obj.SM_FLOOR_ID,
                    Text = obj.BUILDING + " / " + obj.FLOOR_NAME,
                });
            }
            return newList;
        }

        public List<FloorOperationMapViewModel> GetFloorOperationMapList()
        {
            List<FloorOperationMapViewModel> newList = new List<FloorOperationMapViewModel>();
            newList = (from data in _dbContext.SM_FLOOR_OPMAP
                       join _floor in _dbContext.SM_FLOORMASTER on data.FLOORID equals _floor.SM_FLOOR_ID
                       join _op in _dbContext.ADORGLEVEL on data.OPERATIONID equals _op.ADORGLEVELID
                       //where data.STATUS == 1
                       select new FloorOperationMapViewModel
                       {
                           FLOOR_OPMAPPID = data.FLOOR_OPMAPPID,
                           FLOORID = data.FLOORID,
                           FLOOR = _floor.FLOOR_NAME,
                           BUILDING = _floor.BUILDING,
                           OPERATIONID = data.OPERATIONID,
                           OPERATION = _op.LEVELDESCRIP,
                           SEATCAPACITY = data.SEATCAPACITY,
                           SYKIID = data.SYKIID,
                           ADDEDBY = data.ADDEDBY,
                           ADDEDDATE = data.ADDEDDATE,
                           STATUS = data.STATUS,
                           IsSeatAutoUpdated = data.ISSEATAUTOUPDATED, //Added by Aumento :: SR100223
                       }).OrderBy(o => o.FLOOR_OPMAPPID).ToList();
            return newList;
        }

        public Tuple<short, string> SaveFloorOpMapping(FloorOperationMapViewModel model, long ActionBy)
        {
            short retVal = 0;
            string msg = "";
            Tuple<short, string> _tuple = new Tuple<short, string>(retVal, msg);
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    int FlagAdd = 0;
                    SM_FLOOR_OPMAP BCM = _dbContext.SM_FLOOR_OPMAP.Where(a => a.FLOOR_OPMAPPID == model.FLOOR_OPMAPPID).FirstOrDefault();
                    if (BCM == null)
                    {
                        if (_dbContext.SM_FLOOR_OPMAP.Count(a => a.OPERATIONID == model.OPERATIONID) > 0)
                        {
                            retVal = 2;
                            transaction.Rollback();
                            _tuple = new Tuple<short, string>(retVal, msg);
                            return _tuple;
                        }

                        BCM = new SM_FLOOR_OPMAP();
                        if (_dbContext.SM_FLOOR_OPMAP.Count() == 0)
                        {
                            BCM.FLOOR_OPMAPPID = 1;
                        }
                        else
                        {
                            BCM.FLOOR_OPMAPPID = _dbContext.SM_FLOOR_OPMAP.Max(m => m.FLOOR_OPMAPPID) + 1;
                        }
                        FlagAdd = 1;
                    }
                    else
                    {
                        if (_dbContext.SM_FLOOR_OPMAP.Count(a => a.OPERATIONID == model.OPERATIONID && a.FLOOR_OPMAPPID != model.FLOOR_OPMAPPID) > 0)
                        {
                            retVal = 2;
                            transaction.Rollback();
                            _tuple = new Tuple<short, string>(retVal, msg);
                            return _tuple;
                        }
                    }

                    BCM.FLOORID = model.FLOORID;
                    BCM.OPERATIONID = model.OPERATIONID;
                    BCM.SEATCAPACITY = model.SEATCAPACITY;
                    BCM.STATUS = model.STATUS;
                    BCM.SYKIID = (short)_Syki.SYKIID;
                    if (FlagAdd == 1)
                    {
                        BCM.ADDEDBY = ActionBy;
                        BCM.ADDEDDATE = DateTime.Now;
                    }
                    else
                    {
                        BCM.UPDATEDBY = ActionBy;
                        BCM.UPDATEDDATE = DateTime.Now;
                    }
                    _dbContext.Entry(BCM).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                    _dbContext.SaveChanges();
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

        // public short UpdateOpMappingStatus(long mapId, long ActionBy, short status) //Added by Aumento :: SR100223
        public short UpdateOpMappingStatus(long mapId, long ActionBy, short status, string IsSeatAutoUpdated) //Added by Aumento :: SR100223
        {
            short retVal = 0;
            if (mapId > 0)
            {
                SM_FLOOR_OPMAP DT = _dbContext.SM_FLOOR_OPMAP.Where(x => x.FLOOR_OPMAPPID == mapId).FirstOrDefault();
                if (DT != null)
                {
                    DT.STATUS = status;
                    DT.UPDATEDBY = ActionBy;
                    DT.UPDATEDDATE = DateTime.Now;
                    //Added by Aumento :: SR100223
                    DT.ISSEATAUTOUPDATED = IsSeatAutoUpdated;

                    var DWD = _dbContext.SM_DIVISIONWISESEATDETAILS.Where(a => a.FLOOR_OPMAPPID == mapId).ToList();

                    if (DWD.Any())
                    {
                        foreach (var item in DWD)
                        {
                            item.ISSEATAUTOUPDATED = IsSeatAutoUpdated;
                        }
                    }
                    //Added by Aumento :: SR100223
                    _dbContext.Entry(DT).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }

        public List<FloorSeatMstViewModel> GetFloorSeatMstList()
        {
            List<FloorSeatMstViewModel> newList = new List<FloorSeatMstViewModel>();
            newList = (from data in _dbContext.SM_FLOORSEATMST
                       join _floor in _dbContext.SM_FLOORMASTER on data.FLOORID equals _floor.SM_FLOOR_ID
                       //where data.STATUS == 1
                       select new FloorSeatMstViewModel
                       {
                           SEATMSTID = data.SEATMSTID,
                           FLOORID = data.FLOORID,
                           FLOOR = _floor.FLOOR_NAME,
                           BUILDING = _floor.BUILDING,
                           SEATNAME = data.SEATNAME,
                           SEATNO = data.SEATNO,
                           ADDEDBY = data.ADDEDBY,
                           ADDEDON = data.ADDEDON,
                           STATUS = data.STATUS
                       }).OrderBy(o => o.FLOORID).ThenBy(t => t.SEATNO).ToList();
            return newList;
        }

        public Tuple<short, string> SaveFloorSeat(FloorSeatMstViewModel model, long ActionBy)
        {
            short retVal = 0;
            string msg = "";
            Tuple<short, string> _tuple = new Tuple<short, string>(retVal, msg);
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    int FlagAdd = 0;
                    SM_FLOORSEATMST BCM = _dbContext.SM_FLOORSEATMST.Where(a => a.SEATMSTID == model.SEATMSTID).FirstOrDefault();
                    if (BCM == null)
                    {
                        if (_dbContext.SM_FLOORSEATMST.Count(a => a.SEATNO == model.SEATNO) > 0)
                        {
                            retVal = 2;
                            transaction.Rollback();
                            _tuple = new Tuple<short, string>(retVal, msg);
                            return _tuple;
                        }

                        BCM = new SM_FLOORSEATMST();
                        if (_dbContext.SM_FLOORSEATMST.Count() == 0)
                        {
                            BCM.SEATMSTID = 1;
                        }
                        else
                        {
                            BCM.SEATMSTID = _dbContext.SM_FLOORSEATMST.Max(m => m.SEATMSTID) + 1;
                        }
                        FlagAdd = 1;
                    }
                    else
                    {
                        if (_dbContext.SM_FLOORSEATMST.Count(a => a.SEATMSTID != model.SEATMSTID && a.SEATNO == model.SEATNO) > 0)
                        {
                            retVal = 2;
                            transaction.Rollback();
                            _tuple = new Tuple<short, string>(retVal, msg);
                            return _tuple;
                        }
                    }

                    BCM.FLOORID = model.FLOORID;
                    BCM.SEATNAME = model.SEATNAME.Trim();
                    BCM.SEATNO = model.SEATNO;
                    BCM.STATUS = model.STATUS;
                    if (FlagAdd == 1)
                    {
                        BCM.ADDEDBY = ActionBy;
                        BCM.ADDEDON = DateTime.Now;
                    }
                    else
                    {
                        BCM.UPDATEDBY = ActionBy;
                        BCM.UPDATEDON = DateTime.Now;
                    }
                    _dbContext.Entry(BCM).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                    _dbContext.SaveChanges();
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

        public short UpdateSeatStatus(long seatId, long ActionBy, short status)
        {
            short retVal = 0;
            if (seatId > 0)
            {
                SM_FLOORSEATMST DT = _dbContext.SM_FLOORSEATMST.Where(x => x.SEATMSTID == seatId).FirstOrDefault();
                if (DT != null)
                {
                    DT.STATUS = status;
                    DT.UPDATEDBY = ActionBy;
                    DT.UPDATEDON = DateTime.Now;
                    _dbContext.Entry(DT).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }


        //Seat booking popup
        public bool isPopUpEnableForDivisionHead(long eCode, long? funDesignation, long? divisionID)
        {
            bool isValid = false;
            if (funDesignation != null)
            {
                List<long> _fundesgIds = new List<long>();
                string _POPUP_FUN_DESIGNATION = GetParmValByParmName("POPUP_FUN_DESIGNATION");
                if (!string.IsNullOrEmpty(_POPUP_FUN_DESIGNATION))
                {
                    _fundesgIds = _POPUP_FUN_DESIGNATION.Split(',').Select(s => long.Parse(s)).ToList();

                    if (_fundesgIds.Contains(Convert.ToInt64(funDesignation)))
                    {
                        int count = _dbContext.SM_DIVISIONWISESEATDETAILS.Where(x => x.ADORGLEVELID == divisionID && x.STATUS == 1).Count();
                        if (count > 0)
                        {

                            #region Popup till roster open days
                            string ROASTER_FRZ_TIME = "20:00:00";
                            string ROASTER_FRZ_DAY = "SUNDAY";
                            ROASTER_FRZ_DAY = GetParmValByParmName("ROASTER_FRZ_DAY");
                            ROASTER_FRZ_TIME = GetParmValByParmName("ROASTER_FRZ_TIME");
                            ROASTER_FRZ_DAY = !string.IsNullOrEmpty(ROASTER_FRZ_DAY) ? ROASTER_FRZ_DAY : "1";
                            ROASTER_FRZ_TIME = !string.IsNullOrEmpty(ROASTER_FRZ_TIME) ? ROASTER_FRZ_TIME : "20:00:00";

                            DateTime currentDate = DateTime.Now.Date;
                            DayOfWeek currentDay = currentDate.DayOfWeek;
                            int daysTillCurrentDay = currentDay - DayOfWeek.Monday;
                            DateTime currentWeekStartDate = currentDate.AddDays(-daysTillCurrentDay).Date;
                            DateTime _ROASTER_FRZ_DATE = DateTime.Now;
                            DateTime currentWeekEndDate1 = currentWeekStartDate.AddDays(7).AddSeconds(-1).Date;

                            DateTime[] currWeekdates = Enumerable.Range(0, 1 + currentWeekEndDate1.Subtract(currentWeekStartDate).Days).Select(i => currentWeekStartDate.AddDays(i)).ToArray();
                            DateTime new_FRZ_DATE = currWeekdates.Where(x => x.DayOfWeek.ToString().ToUpper() == ROASTER_FRZ_DAY.ToUpper().Trim()).FirstOrDefault();
                            if (new_FRZ_DATE != null)
                            {
                                _ROASTER_FRZ_DATE = DateTime.ParseExact(new_FRZ_DATE.ToString("dd-MMM-yyyy") + " " + ROASTER_FRZ_TIME, "dd-MMM-yyyy HH:mm:ss", null);
                            }

                            string currentTime = DateTime.Now.ToString("HH:mm:ss");
                            DateTime CAL_DATE_TIME = DateTime.ParseExact(currentDate.ToString("dd-MMM-yyyy") + " " + currentTime, "dd-MMM-yyyy HH:mm:ss", null);
                            #endregion

                            if (CAL_DATE_TIME < _ROASTER_FRZ_DATE)
                            {
                                Int32 _paramForFromToDate = Convert.ToInt32(GetParmValByParmName("ROASTER_ALLOED_DAYS"));

                                //DateTime currentDate = DateTime.Now.Date;
                                //DayOfWeek currentDay = currentDate.DayOfWeek;
                                //int daysTillCurrentDay = currentDay - DayOfWeek.Monday;
                                //DateTime currentWeekStartDate = currentDate.AddDays(-daysTillCurrentDay).Date;
                                DateTime currentWeekEndDate = currentWeekStartDate.AddDays(_paramForFromToDate).AddSeconds(-1).Date;

                                DateTime nextWeekStartDate = currentWeekEndDate.AddDays(2).AddSeconds(-1).Date;
                                DateTime nextWeekEndDate = nextWeekStartDate.AddDays(_paramForFromToDate).AddSeconds(-1).Date;

                                var rosterFilledCount = _dbContext.SM_ROSTER_TRN.Where(x => x.ROSTER_DATE >= nextWeekStartDate && x.ROSTER_DATE <= nextWeekEndDate && x.ADDEDBY == eCode).Count();
                                if (rosterFilledCount < 1)
                                {
                                    isValid = true;
                                }
                            }
                        }
                    }
                }
            }
            return isValid;
        }

        //Roster open for (N+1) days (Division Head)
        public bool rosterFillBeforeFreeze(DateTime rosterDate)
        {
            bool isRosterFillBeforeFreeze = false;

            string ROASTER_ALLOED_DAYS = "0";
            string ROASTER_FRZ_TIME = "23:59:59";
            string ROASTER_FRZ_DAY = "SUNDAY";
            DateTime _ROASTER_FRZ_DATE = DateTime.Now;
            DateTime currentDate = DateTime.Now.Date;
            string currentTime = DateTime.Now.ToString("HH:mm:ss");

            ROASTER_ALLOED_DAYS = GetParmValByParmName("ROASTER_ALLOED_DAYS");
            ROASTER_FRZ_DAY = GetParmValByParmName("ROASTER_FRZ_DAY");
            ROASTER_FRZ_TIME = GetParmValByParmName("ROASTER_FRZ_TIME");

            DayOfWeek currentDay = currentDate.DayOfWeek;
            int daysTillCurrentDay = currentDay - DayOfWeek.Monday;
            DateTime currentWeekStartDate = currentDate.AddDays(-daysTillCurrentDay).Date;
            DateTime currentWeekEndDate = currentWeekStartDate.AddDays(7).AddSeconds(-1).Date;
            DateTime[] currWeekdates = Enumerable.Range(0, 1 + currentWeekEndDate.Subtract(currentWeekStartDate).Days).Select(i => currentWeekStartDate.AddDays(i)).ToArray();
            DateTime new_FRZ_DATE = currWeekdates.Where(x => x.DayOfWeek.ToString().ToUpper() == ROASTER_FRZ_DAY.ToUpper().Trim()).FirstOrDefault();
            if (new_FRZ_DATE != null)
            {
                _ROASTER_FRZ_DATE = DateTime.ParseExact(new_FRZ_DATE.ToString("dd-MMM-yyyy") + " " + ROASTER_FRZ_TIME, "dd-MMM-yyyy HH:mm:ss", null);
            }

            DateTime CURRENT_DATETIME = DateTime.ParseExact(currentDate.ToString("dd-MMM-yyyy") + " " + currentTime, "dd-MMM-yyyy HH:mm:ss", null);
            //DateTime rosterEndDate = DateTime.ParseExact(DateTime.Now.Date.AddDays(Convert.ToInt64(GetParmValByParmName("JOB_TO_DAY"))).ToString("dd-MMM-yyyy") + " " + currentTime, "dd-MMM-yyyy HH:mm:ss", null);

            DateTime rosterEndDate = DateTime.ParseExact(currentWeekEndDate.AddDays(Convert.ToInt64(ROASTER_ALLOED_DAYS)).ToString("dd-MMM-yyyy") + " " + currentTime, "dd-MMM-yyyy HH:mm:ss", null);
            if ((CURRENT_DATETIME < _ROASTER_FRZ_DATE || rosterDate > rosterEndDate.Date) && rosterDate > currentWeekEndDate)
            {
                isRosterFillBeforeFreeze = true;
            }
            return isRosterFillBeforeFreeze;
        }
        //End (N+1) freeze days


        public List<SeatingAllocationDetailReportViewModel> GetSeatingAllocationDetail(SearchSeatingDetailViewModel SSVM) // Added by aumento for the SR56983 =============== 
        {
            long? operation = SSVM.OpId;
            long? division = SSVM.DivId;
            long employee = SSVM.Employee;
            long? Floor = SSVM.FLOOR_ID;

            DateTime ReqDateFrom = DateTime.Now.Date;
            DateTime ReqDateTo = DateTime.Now.Date;

            if (!string.IsNullOrEmpty(SSVM.FromDate))
            {
                ReqDateFrom = DateTime.ParseExact(SSVM.FromDate, "dd-MMM-yyyy", null);
            }
            if (!string.IsNullOrEmpty(SSVM.ToDate))
            {
                ReqDateTo = DateTime.ParseExact(SSVM.ToDate + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
            }

            var results = new List<SeatingAllocationDetailReportViewModel>();
            System.Data.DataTable dt = new System.Data.DataTable(); //updated by aumento for : SR100656
            //ConnectionString conn = new ConnectionString();

            try
            {
                using (var connection = new OracleConnection(_conn.getConnectingString()))
                {
                    //using (var command = new OracleCommand("GetSeatingAllocationDetails", connection))
                    using (var command = new OracleCommand("GetSeatingAllocationDetailsReport", connection)) //SR56983_ Changes
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure; //updated by aumento for : SR100656

                        command.Parameters.Add("p_Operation", OracleDbType.Int32).Value = operation;
                        command.Parameters.Add("p_Division", OracleDbType.Int32).Value = division;
                        command.Parameters.Add("p_Employee", OracleDbType.Int32).Value = employee;
                        command.Parameters.Add("p_FromDate", OracleDbType.Date).Value = ReqDateFrom;
                        command.Parameters.Add("p_ToDate", OracleDbType.Date).Value = ReqDateTo;
                        command.Parameters.Add("p_Floor", OracleDbType.Int32).Value = Floor;
                        command.Parameters.Add("CUR_EMPDTL", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output; //updated by aumento for : SR100656

                        connection.Open();

                        using (var adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }

                // Map DataTable to model
                foreach (System.Data.DataRow row in dt.Rows) //updated by aumento for : SR100656
                {
                    var model = new SeatingAllocationDetailReportViewModel
                    {
                        //SEATINGID = row.IsNull("SEATINGID") ? (long?)null : (long)row["SEATINGID"],
                        DATE = row.IsNull("TDATE") ? (DateTime?)null : (DateTime)row["TDATE"],
                        SEATNO = row.IsNull("SEATNO") ? null : (string)row["SEATNO"],
                        SEATNAME = row.IsNull("SEATNAME") ? null : (string)row["SEATNAME"],
                        BUILDING = row.IsNull("BUILDING") ? null : (string)row["BUILDING"],
                        FLOOR = row.IsNull("AFLOOR") ? null : (string)row["AFLOOR"],
                        ADEMPCODE = row.IsNull("ADEMPCODE") ? (long?)null : long.Parse(row["ADEMPCODE"].ToString()),  //SR56983_ Changes
                        ADEMPNAME = row.IsNull("ADEMPNAME") ? null : (string)row["ADEMPNAME"],
                        OPERATION = row.IsNull("OPERATION") ? null : (string)row["OPERATION"],
                        DIVISION = row.IsNull("DIVISION") ? null : (string)row["DIVISION"],
                        REMARKS = row.IsNull("REMARKS") ? null : (string)row["REMARKS"]
                    };
                    results.Add(model);
                }
            }
            catch (Exception ex)
            {
                // Consider logging the exception
                throw new ApplicationException("An error occurred while retrieving seating allocation details.", ex);
            }

            return results;
        }

        //public List<SeatingAllocationSummaryReportViewModel> GetSeatingAllocationSummary(SearchSeatViewModel SSVM) // Added by aumento for the SR56983 =============== 
        //{
        //    try
        //    {
        //        DateTime ReqDateFrom = DateTime.Now.Date;
        //        DateTime ReqDateTo = DateTime.Now.Date;

        //        int ACTUAL_PERCENT = 0;
        //        DateTime CrntDate = DateTime.Now.Date;
        //        string _PERCENT = GetParmValByParmName("ACTUAL_PHY_SEAT");
        //        ACTUAL_PERCENT = !string.IsNullOrEmpty(_PERCENT) ? Convert.ToInt32(_PERCENT) : 0;

        //        if (!string.IsNullOrEmpty(SSVM.FromDate))
        //        {
        //            ReqDateFrom = DateTime.ParseExact(SSVM.FromDate, "dd-MMM-yyyy", null);
        //        }
        //        if (!string.IsNullOrEmpty(SSVM.ToDate))
        //        {
        //            ReqDateTo = DateTime.ParseExact(SSVM.ToDate + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
        //        }
        //        var srno = 0;

        //        List<long> _desgIds = new List<long>();
        //        string _ROSTER_ALLOW_DESG = GetParmValByParmName("ROSTER_ALLOW_DESG");
        //        if (!string.IsNullOrEmpty(_ROSTER_ALLOW_DESG))
        //        {
        //            _desgIds = _ROSTER_ALLOW_DESG.Split(',').Select(s => long.Parse(s)).ToList();
        //        }

        //        List<long> _siteIds = new List<long>();
        //        string _ROSTER_ALLOW_SITE = GetParmValByParmName("ROSTER_ALLOW_SITE");
        //        if (!string.IsNullOrEmpty(_ROSTER_ALLOW_SITE))
        //        {
        //            _siteIds = _ROSTER_ALLOW_SITE.Split(',').Select(s => long.Parse(s)).ToList();
        //        }


        //        var query = (from srt in _dbContext.SM_ROSTER_TRN
        //                     join seatAll in _dbContext.SM_SEATALLOCATION_TRN on srt.RSTTRNID equals seatAll.RSTTRNID
        //                     join seatMst in _dbContext.SM_FLOORSEATMST on seatAll.SEATMSTID equals seatMst.SEATMSTID
        //                     join floorMst in _dbContext.SM_FLOORMASTER on seatMst.FLOORID equals floorMst.SM_FLOOR_ID
        //                     join emp in _dbContext.ADEMPLOYEE.Where(x => x.ACTIVE == 1) on srt.ADEMPCODE equals emp.ADEMPCODE
        //                     join vw in _dbContext.VW_ASSOCIATELVLDETAILS on emp.ADEMPCODE equals vw.ADEMPCODE
        //                     join _siteId in _siteIds on vw.SYSITEID equals _siteId
        //                     join _desID in _desgIds on vw.ADDESIGNATIONID equals _desID

        //                     where vw.SYKI == _Syki.SYKIID &&
        //                     srt.STATUS == 1 &&
        //                           //(SSVM.OpId == 0 ? 1 == 1 : vw.OPERATIONID == SSVM.OpId) &&
        //                           //(SSVM.DivId == 0 ? 1 == 1 : vw.DIVISIONID == SSVM.DivId) &&
        //                           //(SSVM.Employee == 0 ? 1 == 1 : emp.ADEMPCODE == SSVM.Employee) &&
        //                           (SSVM.FromDate == null ? 1 == 1 : srt.ROSTER_DATE >= ReqDateFrom) &&
        //                           (SSVM.ToDate == null ? 1 == 1 : srt.ROSTER_DATE <= ReqDateTo)
        //                     // (SSVM.FromDate == null ? 1 == 1 : h.MONTHDATEYEAR >= ReqDateFrom) &&
        //                     //(SSVM.ToDate == null ? 1 == 1 : h.MONTHDATEYEAR <= ReqDateTo)

        //                     let allowPercentage = _dbContext.SM_ALLOWPERCENT.
        //                   Where(a => a.STATUS == 1 && a.SITEID == floorMst.SITEID && (CrntDate >= a.FROM_DATE && CrntDate <= a.TO_DATE)).
        //                   Select(s => s.ALLOWED_PERCENT).FirstOrDefault()

        //                     let total_manepower = _dbContext.SM_FLOOR_OPMAP
        //                                                         .Where(x => x.FLOORID == floorMst.SM_FLOOR_ID)
        //                                                         .GroupBy(op => op.FLOORID)
        //                                                         .Select(a => a.Sum(op => op.SEATCAPACITY))
        //                                                         .FirstOrDefault()

        //                     //let total = _dbContext.SM_FLOOR_OPMAP.Where(data => data.STATUS == 1 && data.FLOORID == floorMst.SM_FLOOR_ID)
        //                     //                        .Join(
        //                     //                            _dbContext.SM_DIVISIONWISESEATDETAILS,
        //                     //                            data => data.FLOOR_OPMAPPID,
        //                     //                            _SD => _SD.FLOOR_OPMAPPID,
        //                     //                            (data, _SD) => _SD.PHYSICALSEAT
        //                     //                        )
        //                     //                        .Sum(val => (int?)val) ?? 0

        //                     let SeatCapacity = (from f in _dbContext.SM_FLOORSEATMST
        //                                         where f.FLOORID == floorMst.SM_FLOOR_ID &&
        //                                               f.STATUS == 1 &&
        //                                               ! f.SEATNAME.Contains("FIXSEAT")
        //                                         select f).Count()

        //                     //let total_manepower = (from v in _dbContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1)
        //                     //                       join o in _dbContext.SM_FLOOR_OPMAP on v.OPERATIONID equals o.OPERATIONID
        //                     //                       join _siteId in _siteIds on v.SYSITEID equals _siteId
        //                     //                       join _desID in _desgIds on v.ADDESIGNATIONID equals _desID
        //                     //                       where v.SYKI == _Syki.SYKIID && o.FLOORID == floorMst.SM_FLOOR_ID
        //                     //                       select v.ADEMPCODE).Count()

        //                     let Eligibility = (from o in _dbContext.SM_FLOOR_OPMAP
        //                                        where o.FLOORID == floorMst.SM_FLOOR_ID
        //                                        select new
        //                                   {

        //                                       Eligibility = o.SEATCAPACITY * 75 / 100
        //                                   }).ToList()

        //        let totalSeat = Eligibility.Sum(r => r.Eligibility)

        //        //let phisicalseat = (total_manepower * allowPercentage / ACTUAL_PERCENT)

        //                     group new
        //                     {
        //                         srt.ROSTER_DATE,
        //                         floorMst.BUILDING,
        //                         floorMst.FLOOR_NAME,
        //                         TOTAL_MANEPOWER = total_manepower,
        //                         SEAT_CAPACITY = SeatCapacity,
        //                         TOTAL_PHISICAL_SEAT = totalSeat,
        //                         ALLOCATED_SEAT = (from SRT in _dbContext.SM_ROSTER_TRN.Where(w => w.ROSTER_DATE == srt.ROSTER_DATE && w.STATUS == 1)
        //                                           join seat_all in _dbContext.SM_SEATALLOCATION_TRN.Where(c => c.STATUS == 1) on SRT.RSTTRNID equals seat_all.RSTTRNID
        //                                           join floor in _dbContext.SM_FLOORSEATMST on seat_all.SEATMSTID equals floor.SEATMSTID
        //                                           join v in _dbContext.VW_ASSOCIATELVLDETAILS on SRT.ADEMPCODE equals v.ADEMPCODE
        //                                           join _siteId in _siteIds on v.SYSITEID equals _siteId
        //                                           join _desID in _desgIds on v.ADDESIGNATIONID equals _desID
        //                                           where floor.FLOORID == seatMst.FLOORID
        //                                           && v.SYKI == _Syki.SYKIID
        //                                           select srt.RSTTRNID).Count(),

        //                         VACANT_SEAT = SeatCapacity - (from SRT in _dbContext.SM_ROSTER_TRN.Where(w => w.ROSTER_DATE == srt.ROSTER_DATE && w.STATUS == 1)
        //                                                       join seat_all in _dbContext.SM_SEATALLOCATION_TRN.Where(c => c.STATUS == 1) on SRT.RSTTRNID equals seat_all.RSTTRNID
        //                                                       join floor in _dbContext.SM_FLOORSEATMST on seat_all.SEATMSTID equals floor.SEATMSTID
        //                                                       join v in _dbContext.VW_ASSOCIATELVLDETAILS on SRT.ADEMPCODE equals v.ADEMPCODE
        //                                                       join _siteId in _siteIds on v.SYSITEID equals _siteId
        //                                                       join _desID in _desgIds on v.ADDESIGNATIONID equals _desID
        //                                                       where floor.FLOORID == seatMst.FLOORID
        //                                                       && v.SYKI == _Syki.SYKIID
        //                                                       select srt.RSTTRNID).Count(),
        //                     }
        //                     by new
        //                     {
        //                         srt.ROSTER_DATE,
        //                         floorMst.FLOOR_NAME,
        //                         floorMst.BUILDING
        //                     }
        //                        into g
        //                     select new SeatingAllocationSummaryReportViewModel
        //                     {
        //                         //SRNO = srno + 1,
        //                         DATE = g.Key.ROSTER_DATE,
        //                         BUILDING = g.Key.BUILDING,
        //                         FLOOR = g.Key.FLOOR_NAME,
        //                         TOTAL_MANEPOWER = g.Max(x => x.TOTAL_MANEPOWER),
        //                         SEATCAPACITY = g.Max(x => x.SEAT_CAPACITY),
        //                         TOTAL_PHISICAL_SEAT = g.Max(x => x.TOTAL_PHISICAL_SEAT),
        //                         ALLOCATED_SEAT = g.Max(x => x.ALLOCATED_SEAT),
        //                         VACANT_SEAT = g.Max(x => x.VACANT_SEAT)
        //                     })
        //    .OrderBy(x => x.BUILDING)
        //    .ThenBy(x => x.FLOOR)
        //    .ThenBy(x => x.DATE)
        //    .ToList();

        //        var OffDays = (from h in _dbContext.HMSIHOLIDAYS
        //                       join v in _dbContext.VW_ASSOCIATELVLDETAILS on h.SYSITEID equals v.SYSITEID
        //                       join a in _dbContext.ADEMPLOYEE on v.ADEMPCODE equals a.ADEMPCODE
        //                       join f in _dbContext.SM_FLOOR_OPMAP on v.OPERATIONID equals f.OPERATIONID
        //                       join m in _dbContext.SM_FLOORMASTER on f.FLOORID equals m.SM_FLOOR_ID
        //                       join _siteId in _siteIds on v.SYSITEID equals _siteId
        //                       join _desID in _desgIds on v.ADDESIGNATIONID equals _desID
        //                       where v.SYKI == _Syki.SYKIID &&
        //                       h.ACTIVE==1 && a.ACTIVE==1 &&
        //                            (SSVM.FromDate == null ? 1 == 1 : h.MONTHDATEYEAR >= ReqDateFrom) &&
        //                             (SSVM.ToDate == null ? 1 == 1 : h.MONTHDATEYEAR <= ReqDateTo) &&
        //                             (m.FLOOR_NAME != null) &&
        //                            (m.BUILDING != null)

        //                       select new
        //                       {
        //                           h.MONTHDATEYEAR,
        //                           m.BUILDING,
        //                           m.FLOOR_NAME
        //                       }).GroupBy(x => new { x.MONTHDATEYEAR, x.BUILDING, x.FLOOR_NAME })
        //                          .Select(g => new SeatingAllocationSummaryReportViewModel
        //                          {
        //                              DATE = g.Key.MONTHDATEYEAR,
        //                              BUILDING = g.Key.BUILDING,
        //                              FLOOR = g.Key.FLOOR_NAME,
        //                              TOTAL_MANEPOWER = 0,
        //                              SEATCAPACITY = 0,
        //                              TOTAL_PHISICAL_SEAT = 0,
        //                              ALLOCATED_SEAT = 0,
        //                              VACANT_SEAT = 0
        //                          })
        //                          .OrderBy(x => x.DATE)
        //                          .ThenBy(x => x.BUILDING)
        //                          .ThenBy(x => x.FLOOR)
        //                          .ToList();

        //        var union = query.Union(OffDays).OrderBy(x => x.DATE)
        //                  .ThenBy(x => x.BUILDING)
        //                  .ThenBy(x => x.FLOOR).ToList();

        //        return union;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}
        //SR56983_ Changes Start
        public List<SeatingAllocationSummaryReportViewModel> GetSeatingAllocationSummary(SearchSeatViewModel SSVM)
        {
            try
            {
                DateTime ReqDateFrom = string.IsNullOrEmpty(SSVM.FromDate)
                    ? DateTime.Now.Date
                    : DateTime.ParseExact(SSVM.FromDate, "dd-MMM-yyyy", null);

                DateTime ReqDateTo = string.IsNullOrEmpty(SSVM.ToDate)
                    ? DateTime.Now.Date
                    : DateTime.ParseExact(SSVM.ToDate + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);

                int percent;
                int ACTUAL_PERCENT = int.TryParse(GetParmValByParmName("ACTUAL_PHY_SEAT"), out percent)
                                     ? percent
                                     : 0;

                var _desgIds = GetParmValByParmName("ROSTER_ALLOW_DESG")
                    ?.Split(',')
                    .Select(long.Parse)
                    .ToList() ?? new List<long>();

                var _siteIds = GetParmValByParmName("ROSTER_ALLOW_SITE")
                    ?.Split(',')
                    .Select(long.Parse)
                    .ToList() ?? new List<long>();

                DateTime currentDate = DateTime.Now.Date;

                //var rosterQuery = from srt in _dbContext.SM_ROSTER_TRN
                //                  join seatAll in _dbContext.SM_SEATALLOCATION_TRN on srt.RSTTRNID equals seatAll.RSTTRNID
                //                  join seatMst in _dbContext.SM_FLOORSEATMST on seatAll.SEATMSTID equals seatMst.SEATMSTID
                //                  join floorMst in _dbContext.SM_FLOORMASTER on seatMst.FLOORID equals floorMst.SM_FLOOR_ID
                //                  join emp in _dbContext.ADEMPLOYEE.Where(x => x.ACTIVE == 1) on srt.ADEMPCODE equals emp.ADEMPCODE
                //                  join vw in _dbContext.VW_ASSOCIATELVLDETAILS on emp.ADEMPCODE equals vw.ADEMPCODE
                //                  where vw.SYKI == _Syki.SYKIID
                //                        && srt.STATUS == 1
                //                        && (SSVM.FromDate == null || srt.ROSTER_DATE >= ReqDateFrom)
                //                        && (SSVM.ToDate == null || srt.ROSTER_DATE <= ReqDateTo)
                //                        && vw.SYSITEID.HasValue && _siteIds.Contains(vw.SYSITEID.Value)
                //                        && vw.ADDESIGNATIONID.HasValue && _desgIds.Contains(vw.ADDESIGNATIONID.Value)

                //                  let allowedPercentage = _dbContext.SM_ALLOWPERCENT
                //                      .Where(a => a.STATUS == 1 && a.SITEID == floorMst.SITEID && currentDate >= a.FROM_DATE && currentDate <= a.TO_DATE)
                //                      .Select(s => s.ALLOWED_PERCENT).FirstOrDefault()

                //                  let totalManpower = (from v in _dbContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1)
                //                                       join emp in _dbContext.ADEMPLOYEE.Where(x => x.ACTIVE == 1) on v.ADEMPCODE equals emp.ADEMPCODE
                //                                       join d in _dbContext.SM_DIVISIONWISESEATDETAILS.Where(d => d.STATUS == 1) on v.DIVISIONID equals d.ADORGLEVELID
                //                                       join o in _dbContext.SM_FLOOR_OPMAP on v.OPERATIONID equals o.OPERATIONID
                //                                       join _siteId in _siteIds on v.SYSITEID equals _siteId
                //                                       join _desID in _desgIds on v.ADDESIGNATIONID equals _desID
                //                                       where v.SYKI == _Syki.SYKIID && o.FLOORID == floorMst.SM_FLOOR_ID
                //                                       && ReqDateFrom >= emp.REGDATE && ReqDateTo <= emp.EXPIRYDATE
                //                                       select v.ADEMPCODE).Count()

                //                  let seatCapacity = _dbContext.SM_FLOORSEATMST
                //                      .Where(f => f.FLOORID == floorMst.SM_FLOOR_ID && f.STATUS == 1 && !f.SEATNAME.Contains("FIXSEAT"))
                //                      .Count()

                //                  let totalSeat = (from o in _dbContext.SM_FLOOR_OPMAP
                //                                   where o.FLOORID == floorMst.SM_FLOOR_ID
                //                                   select o.SEATCAPACITY * 75 / 100).Sum()

                //                  let allocatedSeat = (from SRT in _dbContext.SM_ROSTER_TRN.Where(w => w.ROSTER_DATE == srt.ROSTER_DATE && w.STATUS == 1)
                //                                       join seat_all in _dbContext.SM_SEATALLOCATION_TRN.Where(c => c.STATUS == 1) on SRT.RSTTRNID equals seat_all.RSTTRNID
                //                                       join floor in _dbContext.SM_FLOORSEATMST on seat_all.SEATMSTID equals floor.SEATMSTID
                //                                       join v in _dbContext.VW_ASSOCIATELVLDETAILS on SRT.ADEMPCODE equals v.ADEMPCODE
                //                                       where floor.FLOORID == seatMst.FLOORID && v.SYKI == _Syki.SYKIID
                //                                       select SRT.RSTTRNID).Count()

                //                  let vacantSeat = seatCapacity - allocatedSeat
                //                  group new
                //                  {
                //                      srt.ROSTER_DATE,
                //                      floorMst.BUILDING,
                //                      floorMst.FLOOR_NAME,
                //                      TOTAL_MANEPOWER = totalManpower,
                //                      SEAT_CAPACITY = seatCapacity,
                //                      TOTAL_PHISICAL_SEAT = totalSeat,
                //                      ALLOCATED_SEAT = allocatedSeat,
                //                      VACANT_SEAT = vacantSeat
                //                  }
                //                  by new
                //                  {
                //                      srt.ROSTER_DATE,
                //                      floorMst.FLOOR_NAME,
                //                      floorMst.BUILDING
                //                  }
                //                  into g
                //                  select new SeatingAllocationSummaryReportViewModel
                //                  {
                //                      DATE = g.Key.ROSTER_DATE,
                //                      BUILDING = g.Key.BUILDING,
                //                      FLOOR = g.Key.FLOOR_NAME,
                //                      TOTAL_MANEPOWER = g.Max(x => x.TOTAL_MANEPOWER),
                //                      SEATCAPACITY = g.Max(x => x.SEAT_CAPACITY),
                //                      TOTAL_PHISICAL_SEAT = g.Max(x => x.TOTAL_PHISICAL_SEAT),
                //                      ALLOCATED_SEAT = g.Max(x => x.ALLOCATED_SEAT),
                //                      VACANT_SEAT = g.Max(x => x.VACANT_SEAT)
                //                  };

                //-----------------------------------------------------------------------------------------------------------------------------

                //var baseQuery = (from srt in _dbContext.SM_ROSTER_TRN
                //                 join seatAll in _dbContext.SM_SEATALLOCATION_TRN on srt.RSTTRNID equals seatAll.RSTTRNID
                //                 join seatMst in _dbContext.SM_FLOORSEATMST on seatAll.SEATMSTID equals seatMst.SEATMSTID
                //                 join floorMst in _dbContext.SM_FLOORMASTER on seatMst.FLOORID equals floorMst.SM_FLOOR_ID
                //                 join emp in _dbContext.ADEMPLOYEE on srt.ADEMPCODE equals emp.ADEMPCODE
                //                 join vw in _dbContext.VW_ASSOCIATELVLDETAILS on emp.ADEMPCODE equals vw.ADEMPCODE
                //                 where emp.ACTIVE == 1
                //                    && vw.SYKI == _Syki.SYKIID
                //                    && srt.STATUS == 1
                //                    && (SSVM.FromDate == null || srt.ROSTER_DATE >= ReqDateFrom)
                //                    && (SSVM.ToDate == null || srt.ROSTER_DATE <= ReqDateTo)
                //                    && vw.SYSITEID.HasValue && _siteIds.Contains(vw.SYSITEID.Value)
                //                    && vw.ADDESIGNATIONID.HasValue && _desgIds.Contains(vw.ADDESIGNATIONID.Value)
                //                 select new
                //                 {
                //                     srt.ROSTER_DATE,
                //                     floorMst.BUILDING,
                //                     floorMst.FLOOR_NAME,
                //                     floorMst.SITEID,
                //                     floorMst.SM_FLOOR_ID,
                //                     seatMst.FLOORID
                //                 }).ToList(); // Materialize to switch to in-memory

                //var floorIds = baseQuery.Select(x => x.SM_FLOOR_ID).Distinct().ToList();

                //var seatCapacities = _dbContext.SM_FLOORSEATMST
                //    .Where(f => floorIds.Contains(f.FLOORID) && f.STATUS == 1 && !f.SEATNAME.Contains("FIXSEAT"))
                //    .GroupBy(f => f.FLOORID)
                //    .Select(g => new { FLOORID = g.Key, Count = g.Count() })
                //    .ToDictionary(x => x.FLOORID, x => x.Count);

                //var totalSeats = _dbContext.SM_FLOOR_OPMAP
                //    .Where(o => floorIds.Contains(o.FLOORID))
                //    .GroupBy(o => o.FLOORID)
                //    .Select(g => new { FLOORID = g.Key, Total = g.Sum(x => x.SEATCAPACITY * 75 / 100) })
                //    .ToDictionary(x => x.FLOORID, x => x.Total);


                //var vwDetails = _dbContext.VW_ASSOCIATELVLDETAILS
                //    .Where(v => v.ACTIVE == 1 && v.SYKI == _Syki.SYKIID)
                //    .ToList();

                //var employees = _dbContext.ADEMPLOYEE
                //    .Where(e => e.ACTIVE == 1)
                //    .ToList();

                //var divisionSeatDetails = _dbContext.SM_DIVISIONWISESEATDETAILS
                //    .Where(d => d.STATUS == 1)
                //    .ToList();

                //var floorOpMap = _dbContext.SM_FLOOR_OPMAP
                //    .ToList();


                //var rosterQuery = baseQuery
                //    .GroupBy(x => new { x.ROSTER_DATE, x.BUILDING, x.FLOOR_NAME, x.SM_FLOOR_ID })
                //    .Select(g =>
                //    {
                //        var floorId = g.Key.SM_FLOOR_ID;

                //        var allocatedSeat = _dbContext.SM_ROSTER_TRN
                //            .Where(w => w.ROSTER_DATE == g.Key.ROSTER_DATE && w.STATUS == 1)
                //            .Join(_dbContext.SM_SEATALLOCATION_TRN.Where(c => c.STATUS == 1),
                //                  srt => srt.RSTTRNID,
                //                  seat => seat.RSTTRNID,
                //                  (srt, seat) => new { srt, seat })
                //            .Join(_dbContext.SM_FLOORSEATMST,
                //                  s => s.seat.SEATMSTID,
                //                  floor => floor.SEATMSTID,
                //                  (s, floor) => new { s.srt, floor })
                //            .Count(x => x.floor.FLOORID == floorId);

                //        var vacantSeat = seatCapacities.GetValueOrDefault(floorId, 0) - allocatedSeat;


                //        var totalManpower = (from v in vwDetails
                //                             join e in employees on v.ADEMPCODE equals e.ADEMPCODE
                //                             join d in divisionSeatDetails on v.DIVISIONID equals d.ADORGLEVELID
                //                             join o in floorOpMap on v.OPERATIONID equals o.OPERATIONID
                //                             where o.FLOORID == floorId
                //                                && ReqDateFrom >= e.REGDATE
                //                                && ReqDateTo <= e.EXPIRYDATE
                //                                && _siteIds.Contains(v.SYSITEID ?? 0)
                //                                && _desgIds.Contains(v.ADDESIGNATIONID ?? 0)
                //                             select v.ADEMPCODE).Distinct().Count();


                //        return new SeatingAllocationSummaryReportViewModel
                //        {
                //            DATE = g.Key.ROSTER_DATE,
                //            BUILDING = g.Key.BUILDING,
                //            FLOOR = g.Key.FLOOR_NAME,
                //            TOTAL_MANEPOWER = totalManpower,
                //            SEATCAPACITY = seatCapacities.GetValueOrDefault(floorId, 0),
                //            TOTAL_PHISICAL_SEAT = totalSeats.GetValueOrDefault(floorId, 0),
                //            ALLOCATED_SEAT = allocatedSeat,
                //            VACANT_SEAT = vacantSeat
                //        };
                //    }).ToList();

                //----------------------------------------------------------------------------------------------------------------------------------------

                var rosterQuery =
    from srt in _dbContext.SM_ROSTER_TRN
    join seatAll in _dbContext.SM_SEATALLOCATION_TRN on srt.RSTTRNID equals seatAll.RSTTRNID
    join seatMst in _dbContext.SM_FLOORSEATMST on seatAll.SEATMSTID equals seatMst.SEATMSTID
    join floorMst in _dbContext.SM_FLOORMASTER on seatMst.FLOORID equals floorMst.SM_FLOOR_ID
    join emp in _dbContext.ADEMPLOYEE.Where(x => x.ACTIVE == (short)1) on srt.ADEMPCODE equals emp.ADEMPCODE
    join vw in _dbContext.VW_ASSOCIATELVLDETAILS on emp.ADEMPCODE equals vw.ADEMPCODE
    where vw.SYKI == (long?)_Syki.SYKIID
          && srt.STATUS == (short)1
          && (SSVM.FromDate == null || srt.ROSTER_DATE >= ReqDateFrom)
          && (SSVM.ToDate == null || srt.ROSTER_DATE <= ReqDateTo)
          && vw.SYSITEID.HasValue && _siteIds.AsQueryable().Contains(vw.SYSITEID.Value)
          && vw.ADDESIGNATIONID.HasValue && _desgIds.AsQueryable().Contains(vw.ADDESIGNATIONID.Value)

    // Allowed Percentage
    let allowedPercentage =
        (from a in _dbContext.SM_ALLOWPERCENT
         where a.STATUS == (short)1
               && a.SITEID == floorMst.SITEID
               && currentDate >= a.FROM_DATE
               && currentDate <= a.TO_DATE
         orderby a.FROM_DATE
         select (int?)a.ALLOWED_PERCENT).FirstOrDefault()

    // Total Manpower
    let totalManpower =
        (from v in _dbContext.VW_ASSOCIATELVLDETAILS
         join emp2 in _dbContext.ADEMPLOYEE on v.ADEMPCODE equals emp2.ADEMPCODE
         join d in _dbContext.SM_DIVISIONWISESEATDETAILS.Where(d => d.STATUS == (short)1) on v.DIVISIONID equals d.ADORGLEVELID
         join o in _dbContext.SM_FLOOR_OPMAP on v.OPERATIONID equals o.OPERATIONID
         where v.ACTIVE == (short)1
               && v.SYKI == (long?)_Syki.SYKIID
               && v.SYSITEID.HasValue && v.ADDESIGNATIONID.HasValue
               && _siteIds.Contains(v.SYSITEID.Value)
               && _desgIds.Contains(v.ADDESIGNATIONID.Value)
               && o.FLOORID == floorMst.SM_FLOOR_ID
               && ReqDateFrom >= emp2.REGDATE
               && ReqDateTo <= emp2.EXPIRYDATE
         select v.ADEMPCODE).Count()

    // Seat Capacity
    let seatCapacity =
        _dbContext.SM_FLOORSEATMST
            .Where(f => f.FLOORID == floorMst.SM_FLOOR_ID
                     && f.STATUS == (short)1
                     && !f.SEATNAME.Contains("FIXSEAT"))
            .Count()

    // Total Seat
    let totalSeat =
        (from o in _dbContext.SM_FLOOR_OPMAP
         where o.FLOORID == floorMst.SM_FLOOR_ID
         select (int)Math.Floor((decimal)o.SEATCAPACITY * 75m / 100m)).Sum()

    //let totalSeat = (from o in _dbContext.SM_FLOOR_OPMAP
    //where o.FLOORID == floorMst.SM_FLOOR_ID
    //select o.SEATCAPACITY * 75 / 100).Sum()

                         // Allocated Seat
                     let allocatedSeat =
        (from SRT in _dbContext.SM_ROSTER_TRN
         join seat_all in _dbContext.SM_SEATALLOCATION_TRN on SRT.RSTTRNID equals seat_all.RSTTRNID
         join floor in _dbContext.SM_FLOORSEATMST on seat_all.SEATMSTID equals floor.SEATMSTID
         join v in _dbContext.VW_ASSOCIATELVLDETAILS on SRT.ADEMPCODE equals v.ADEMPCODE
         where SRT.ROSTER_DATE == srt.ROSTER_DATE
               && SRT.STATUS == (short)1
               && seat_all.STATUS == (short)1
               && floor.FLOORID == seatMst.FLOORID
               && v.SYKI == (long?)_Syki.SYKIID
         select SRT.RSTTRNID).Count()

    // Vacant Seat
    let vacantSeat = seatCapacity - allocatedSeat

    group new
    {
        srt.ROSTER_DATE,
        floorMst.BUILDING,
        floorMst.FLOOR_NAME,
        TOTAL_MANEPOWER = totalManpower,
        SEAT_CAPACITY = seatCapacity,
        TOTAL_PHISICAL_SEAT = totalSeat,
        ALLOCATED_SEAT = allocatedSeat,
        VACANT_SEAT = vacantSeat
    }
    by new
    {
        srt.ROSTER_DATE,
        floorMst.FLOOR_NAME,
        floorMst.BUILDING
    }
    into g
    select new SeatingAllocationSummaryReportViewModel
    {
        DATE = g.Key.ROSTER_DATE,
        BUILDING = g.Key.BUILDING,
        FLOOR = g.Key.FLOOR_NAME,
        TOTAL_MANEPOWER = g.Max(x => x.TOTAL_MANEPOWER),
        SEATCAPACITY = g.Max(x => x.SEAT_CAPACITY),
        TOTAL_PHISICAL_SEAT = g.Max(x => x.TOTAL_PHISICAL_SEAT),
        ALLOCATED_SEAT = g.Max(x => x.ALLOCATED_SEAT),
        VACANT_SEAT = g.Max(x => x.VACANT_SEAT)
    };


                var holidayQuery = from h in _dbContext.HMSIHOLIDAYS
                                   join v in _dbContext.VW_ASSOCIATELVLDETAILS on h.SYSITEID equals v.SYSITEID
                                   join m in _dbContext.SM_FLOORMASTER on v.SYSITEID equals m.SM_FLOOR_ID
                                   where v.SYKI == _Syki.SYKIID && h.ACTIVE == 1
                                         && (SSVM.FromDate == null || h.MONTHDATEYEAR >= ReqDateFrom)
                                         && (SSVM.ToDate == null || h.MONTHDATEYEAR <= ReqDateTo)
                                   group new { h.MONTHDATEYEAR, m.BUILDING, m.FLOOR_NAME }
                                   by new { h.MONTHDATEYEAR, m.BUILDING, m.FLOOR_NAME } into g
                                   select new SeatingAllocationSummaryReportViewModel
                                   {
                                       DATE = g.Key.MONTHDATEYEAR,
                                       BUILDING = g.Key.BUILDING,
                                       FLOOR = g.Key.FLOOR_NAME,
                                       TOTAL_MANEPOWER = 0,
                                       SEATCAPACITY = 0,
                                       TOTAL_PHISICAL_SEAT = 0,
                                       ALLOCATED_SEAT = 0,
                                       VACANT_SEAT = 0
                                   };

                return rosterQuery.Union(holidayQuery)
                    .OrderBy(x => x.DATE)
                    .ThenBy(x => x.BUILDING)
                    .ThenBy(x => x.FLOOR)
                    .ToList();


                //var combined = rosterQuery
                //    .AsEnumerable()
                //    .Union(holidayQuery)
                //    .OrderBy(x => x.DATE)
                //    .ThenBy(x => x.BUILDING)
                //    .ThenBy(x => x.FLOOR)
                //    .ToList();




            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //SR56983_ Changes End

        public dynamic GetOpList(long typeId) // Added by aumento for the SR56980 =============== 
        {
            dynamic Res;
            var iList = (from data in _dbContext.ADORGLEVEL
                         where data.ACTIVE == 1 && data.SYKIID == _Syki.SYKIID && data.ADORGLEVELTYPEID == typeId
                         select new
                         {
                             Value = data.ADORGLEVELID,
                             Text = data.LEVELDESCRIP
                         }).OrderBy(b => b.Text).ToList();
            Res = iList;
            return Res;
        }

        // Added by aumento for the SR56980 ===============

        public List<SeatEmpCalViewModel> GetViewRosterList(long userId, List<SeatCalenderViewModel> Cal_List, Employee_Details Emp_Dtl, long? OpId, long? DivId)
        {

            string ROASTER_FRZ_TIME = "23:59:59";
            int ROASTER_FRZ_DAY = 0;
            DateTime _ROASTER_FRZ_DATE = DateTime.Now;
            DateTime currentDate = DateTime.Now.Date;
            string currentTime = DateTime.Now.ToString("HH:mm:ss");


            ROASTER_FRZ_DAY = Convert.ToInt32(GetParmValByParmName("OPHEAD_ROASTER_FRZ_DATE"));
            ROASTER_FRZ_TIME = GetParmValByParmName("OPHEAD_ROASTER_FRZ_TIME");

            ROASTER_FRZ_TIME = !string.IsNullOrEmpty(ROASTER_FRZ_TIME) ? ROASTER_FRZ_TIME : "23:59:59";
            _ROASTER_FRZ_DATE = (DateTime.ParseExact(_ROASTER_FRZ_DATE.ToString("dd-MMM-yyyy") + " " + ROASTER_FRZ_TIME, "dd-MMM-yyyy HH:mm:ss", null).AddDays(ROASTER_FRZ_DAY));


            List<SeatEmpCalViewModel> _EmpList = new List<SeatEmpCalViewModel>();
            try
            {
                List<long> _desgIds = new List<long>();
                string _ROSTER_ALLOW_DESG = GetParmValByParmName("ROSTER_ALLOW_DESG");
                if (!string.IsNullOrEmpty(_ROSTER_ALLOW_DESG))
                {
                    _desgIds = _ROSTER_ALLOW_DESG.Split(',').Select(s => long.Parse(s)).ToList();
                }

                List<long> _siteIds = new List<long>();
                string _ROSTER_ALLOW_SITE = GetParmValByParmName("ROSTER_ALLOW_SITE");
                if (!string.IsNullOrEmpty(_ROSTER_ALLOW_SITE))
                {
                    _siteIds = _ROSTER_ALLOW_SITE.Split(',').Select(s => long.Parse(s)).ToList();
                }
                /*
                _EmpList = (from _desID in _desgIds
                            join data in _dbContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == _Syki.SYKIID && x.ACTIVE == 1) on _desID equals data.ADDESIGNATIONID
                            join _siteId in _siteIds on data.SYSITEID equals _siteId
                            join _opMap in _dbContext.SM_FLOOR_OPMAP.Where(o => o.STATUS == 1) on data.OPERATIONID equals _opMap.OPERATIONID
                            join _divi in _dbContext.SM_DIVISIONWISESEATDETAILS on data.DIVISIONID equals _divi.ADORGLEVELID
                            join _emp in _dbContext.ADEMPLOYEE.Where(w => w.ACTIVE == 1) on data.ADEMPCODE equals _emp.ADEMPCODE
                            where
                             //(OpId == null || Emp_Dtl._OpId == 0 || Emp_Dtl._OpId == null ? true : data.OPERATIONID == OpId)
                             //&& (DivId == null || Emp_Dtl._DivId == 0 || Emp_Dtl._DivId == null ? true : data.DIVISIONID == DivId)
                             //&& (Emp_Dtl._DepId == 0 || Emp_Dtl._DepId == null ? true : data.DEPARTMENTID == Emp_Dtl._DepId)
                             //&& (Emp_Dtl._SecId == 0 || Emp_Dtl._SecId == null ? true : data.SECTIONID == Emp_Dtl._SecId)
                             //&& (Emp_Dtl._FnDesigId == 0 || Emp_Dtl._FnDesigId == null ? data.ADEMPCODE == userId : true) &&
                             _opMap.OPERATIONID == OpId && _divi.ADORGLEVELID == DivId
                            select new SeatEmpCalViewModel
                            {
                                ECODE = _emp.ADEMPCODE,
                                ENAME = (_emp.FIRSTNAME + " " + _emp.LASTNAME).Trim(),
                                DIVISIONID = data.DIVISIONID,
                                DIVISION = data.DIVISION,
                                DEPARTMENTID = data.DEPARTMENTID,
                                DEPARTMENT = data.DEPARTMENT,
                                FN_DESIGNATIONID = data.ADFUNCTIONALDESIGNATIONID == null ? 0 : data.ADFUNCTIONALDESIGNATIONID,
                                CAL_LIST = (from calObj in Cal_List
                                            join _Trn in _dbContext.SM_ROSTER_TRN.Where(t => t.ADEMPCODE == _emp.ADEMPCODE && t.STATUS == 1) on calObj.CAL_DATE_FORMAT equals _Trn.ROSTER_DATE into TrnJoin
                                            from _TrnData in TrnJoin.DefaultIfEmpty()
                                            select new SeatCalenderViewModel
                                            {
                                                CALID = calObj.CALID,
                                                CAL_DATE = calObj.CAL_DATE,
                                                IS_OFFDAY = calObj.IS_OFFDAY,
                                                OFFDAY_COLOR = calObj.OFFDAY_COLOR,
                                                WORKINGDAY_COLOR = calObj.WORKINGDAY_COLOR,
                                                WEEK_DAY = calObj.WEEK_DAY,
                                                IS_CHECKED = _TrnData == null ? false : true,
                                                IS_READONLY = IsReadOnlyForOPHead(Convert.ToDateTime(calObj.CAL_DATE), _ROASTER_FRZ_DATE, currentDate, currentTime),
                                                DATE_WISE_COUNT = (from SRT in _dbContext.SM_ROSTER_TRN.Where(w => w.ROSTER_DATE == calObj.CAL_DATE_FORMAT && w.STATUS == 1)
                                                                   join seat_all in _dbContext.SM_SEATALLOCATION_TRN.Where(c => c.STATUS == 1) on SRT.RSTTRNID equals seat_all.RSTTRNID
                                                                   join VW in _dbContext.VW_ASSOCIATELVLDETAILS.Where(v => v.SYKI == _Syki.SYKIID) on SRT.ADEMPCODE equals VW.ADEMPCODE
                                                                   where VW.DIVISIONID == data.DIVISIONID && VW.OPERATIONID == data.OPERATIONID
                                                                   select SRT.RSTTRNID).Count(),
                                            }).ToList(),
                            }).OrderBy(o => o.DIVISIONID).ThenBy(t => t.DEPARTMENTID).ThenByDescending(d => d.FN_DESIGNATIONID).ThenBy(e => e.ENAME).ToList();
                            */
                /*Below Code Optimized By Aumento as on 25092024 Start*/
                var empDetails = (from _desID in _desgIds
                                  join data in _dbContext.VW_ASSOCIATELVLDETAILS.AsNoTracking()
                                        .Where(x => x.SYKI == _Syki.SYKIID && x.ACTIVE == 1)
                                  on _desID equals data.ADDESIGNATIONID
                                  join _siteId in _siteIds on data.SYSITEID equals _siteId
                                  join _opMap in _dbContext.SM_FLOOR_OPMAP.AsNoTracking().Where(o => o.STATUS == 1)
                                  on data.OPERATIONID equals _opMap.OPERATIONID
                                  join _divi in _dbContext.SM_DIVISIONWISESEATDETAILS
                                  on data.DIVISIONID equals _divi.ADORGLEVELID
                                  join _emp in _dbContext.ADEMPLOYEE.AsNoTracking().Where(w => w.ACTIVE == 1)
                                  on data.ADEMPCODE equals _emp.ADEMPCODE
                                  where /*(Emp_Dtl._OpId == 0 || Emp_Dtl._OpId == null ? true : data.OPERATIONID == Emp_Dtl._OpId)
                                        && (Emp_Dtl._DivId == 0 || Emp_Dtl._DivId == null ? true : data.DIVISIONID == Emp_Dtl._DivId)*/
                                         _opMap.OPERATIONID == OpId && _divi.ADORGLEVELID == DivId
                                  select new
                                  {
                                      _emp.ADEMPCODE,
                                      _emp.FIRSTNAME,
                                      _emp.LASTNAME,
                                      data.DIVISIONID,
                                      data.DIVISION,
                                      data.DEPARTMENTID,
                                      data.DEPARTMENT,
                                      data.ADFUNCTIONALDESIGNATIONID,
                                      _opMap.FLOORID
                                  }).ToList();


                var employeeCodes = empDetails.Select(e => e.ADEMPCODE).ToList();
                var calendarDates = Cal_List.Select(c => c.CAL_DATE_FORMAT).ToList();

                var rosterData = _dbContext.SM_ROSTER_TRN.AsNoTracking()
                                    .Where(t => employeeCodes.Contains(t.ADEMPCODE) && calendarDates.Contains(t.ROSTER_DATE) && t.STATUS == 1)
                                    .ToList();

                var seatAllocations = (from seatAllocation in _dbContext.SM_SEATALLOCATION_TRN.AsNoTracking()
                                       join SRT in _dbContext.SM_ROSTER_TRN.AsNoTracking()
                                           on seatAllocation.RSTTRNID equals SRT.RSTTRNID
                                       join seatMst in _dbContext.SM_FLOORSEATMST.AsNoTracking()
                                           on seatAllocation.SEATMSTID equals seatMst.SEATMSTID
                                       where seatAllocation.STATUS == 1
                                       select new
                                       {
                                           SRT.ADEMPCODE,
                                           SRT.ROSTER_DATE,
                                           SRT.RSTTRNID,
                                           SeatData = new SeatAllocationViewModel
                                           {
                                               SEAT_NAME = seatMst.SEATNAME,
                                               SEAT_NO = seatMst.SEATNO
                                           }
                                       }).ToList();
                var dateWiseCounts = (from SRT in rosterData
                                      join SA in seatAllocations on SRT.RSTTRNID equals SA.RSTTRNID
                                      join Emp in empDetails on SRT.ADEMPCODE equals Emp.ADEMPCODE
                                      group SRT by SRT.ROSTER_DATE into dateGroup
                                      select new
                                      {
                                          RosterDate = dateGroup.Key,
                                          Count = dateGroup.Count()
                                      })
                                     .ToDictionary(x => x.RosterDate, x => x.Count);
                _EmpList = empDetails.Select(emp => new SeatEmpCalViewModel
                {
                    ECODE = emp.ADEMPCODE,
                    ENAME = (emp.FIRSTNAME + " " + emp.LASTNAME).Trim(),
                    DIVISIONID = emp.DIVISIONID,
                    DIVISION = emp.DIVISION,
                    DEPARTMENTID = emp.DEPARTMENTID,
                    DEPARTMENT = emp.DEPARTMENT,
                    FN_DESIGNATIONID = emp.ADFUNCTIONALDESIGNATIONID ?? 0,
                    FLOORID = emp.FLOORID,

                    CAL_LIST = Cal_List.Select(calObj =>
                    {
                        var rosterEntry = rosterData.FirstOrDefault(t => t.ADEMPCODE == emp.ADEMPCODE && t.ROSTER_DATE == calObj.CAL_DATE_FORMAT);

                        var seatObj = seatAllocations
                                        .Where(s => s.ADEMPCODE == emp.ADEMPCODE && s.ROSTER_DATE == calObj.CAL_DATE_FORMAT)
                                        .Select(s => s.SeatData)
                                        .FirstOrDefault();

                        return new SeatCalenderViewModel
                        {
                            CALID = calObj.CALID,
                            CAL_DATE = calObj.CAL_DATE,
                            IS_OFFDAY = calObj.IS_OFFDAY,
                            OFFDAY_COLOR = calObj.OFFDAY_COLOR,
                            WORKINGDAY_COLOR = calObj.WORKINGDAY_COLOR,
                            WEEK_DAY = calObj.WEEK_DAY,
                            IS_CHECKED = rosterEntry != null,
                            DATE_WISE_COUNT = dateWiseCounts.ContainsKey(calObj.CAL_DATE_FORMAT) ? dateWiseCounts[calObj.CAL_DATE_FORMAT] : 0,
                            SEAT_OBJ = seatObj
                        };
                    }).ToList()
                })
                .OrderBy(o => o.DIVISIONID)
                .ThenBy(t => t.DEPARTMENTID)
                .ThenBy(e => e.ENAME)
                .ToList();
                /*End*/
            }
            catch (Exception ex)
            {
                _EmpList = new List<SeatEmpCalViewModel>();
            }
            return _EmpList;
        }

        public List<SelectListViewModel> BindBuilding() // Added by aumento for the SR56983 =============== 
        {

            try
            {
                var iList = (from data in _dbContext.SM_FLOORMASTER
                             where data.STATUS == 1
                             select new SelectListViewModel
                             {

                                 Value = data.SM_FLOOR_ID,
                                 Text = data.FLOOR_NAME
                             }).OrderBy(b => b.Value).ToList();

                return iList;

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public List<SelectListViewModel> GetOpeLevelList(long typeId)// Added by aumento for the SR56983 =============== 
        {
            //var iList = (from data in _dbContext.ADORGLEVEL
            //             where data.ACTIVE == 1 && data.SYKIID == _Syki.SYKIID && data.ADORGLEVELTYPEID == typeId
            //             select new SelectListViewModel
            //             {
            //                 Value = data.ADORGLEVELID,
            //                 Text = data.LEVELDESCRIP
            //             }).OrderBy(b => b.Text).ToList();

            var iList = (from o in _dbContext.SM_FLOOR_OPMAP
                         join data in _dbContext.ADORGLEVEL on o.OPERATIONID equals data.ADORGLEVELID
                         where data.ACTIVE == 1 && data.SYKIID == _Syki.SYKIID
                         && data.ADORGLEVELTYPEID == typeId
                         select new SelectListViewModel
                         {
                             Value = data.ADORGLEVELID,
                             Text = data.LEVELDESCRIP
                         }).OrderBy(b => b.Text).ToList();
            return iList;
        }

        // Start :: Added by aumento : SR100656
        public List<SelectListViewModel> GetDivisionList()
        {
            List<long> _siteIds = new List<long>();
            string _ROSTER_ALLOW_SITE = GetParmValByParmName("ROSTER_ALLOW_SITE");
            if (!string.IsNullOrEmpty(_ROSTER_ALLOW_SITE))
            {
                _siteIds = _ROSTER_ALLOW_SITE.Split(',').Select(s => long.Parse(s)).ToList();
            }

            List<SelectListViewModel> iList = new List<SelectListViewModel>();

            //iList = (from data in _dbContext.VW_ASSOCIATELVLDETAILS.Where(e => e.ACTIVE == 1
            //            && e.SYKI == _Syki.SYKIID && e.DIVISIONID != null && e.DIVISIONID != 0
            //            )
            //         join _siteId in _siteIds on data.SYSITEID equals _siteId
            //         select new SelectListViewModel
            //         {
            //             Value = (long)data.DIVISIONID,
            //             Text = data.DIVISION
            //         }).Distinct().ToList();


            iList = _dbContext.VW_ASSOCIATELVLDETAILS
                .Where(e => e.ACTIVE == 1
                    && e.SYKI == _Syki.SYKIID
                    && e.DIVISIONID != null
                    && e.DIVISIONID != 0
                    && _siteIds.Contains((long)e.SYSITEID)) // ✅ Replaces join
                .Select(e => new SelectListViewModel
                {
                    Value = (long)e.DIVISIONID,
                    Text = e.DIVISION
                })
                .Distinct()
                .OrderBy(x => x.Text)
                .ToList();



            return iList;

        }

        public List<ExtrSeatMstViewModel> GetExtrSeatMstList()
        {
            List<ExtrSeatMstViewModel> newList = new List<ExtrSeatMstViewModel>();
            newList = (from e in _dbContext.SM_EXTRASEATFORDIV
                       join l in _dbContext.ADORGLEVEL on e.DIVISIONID equals l.ADORGLEVELID into lj1
                       from l in lj1.DefaultIfEmpty()
                       join h in _dbContext.ADORGLEVELHEAD on e.DIVISIONID equals h.ADORGLEVELID into lj2
                       from h in lj2.DefaultIfEmpty()
                       where
                              l != null && l.ACTIVE == 1
                             && h != null && h.ISACTIVE == 1
                             && l.SYKIID == _Syki.SYKIID
                       orderby e.SRNO
                       select new ExtrSeatMstViewModel
                       {
                           SRNO = e.SRNO,
                           DIVISIONID = e.DIVISIONID,
                           DIVISION = l.LEVELDESCRIP,
                           EXTRASEATCOUNT = e.EXTRASEATCOUNT,
                           STATUS = e.STATUS
                       }).ToList();

            return newList;
        }

        public Tuple<short, string> SaveExtraSeatForDiv(ExtrSeatMstViewModel model, long actionBy)
        {
            short retVal = 0;
            string msg = "";

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    bool isNew = model.SRNO == 0;


                    SM_EXTRASEATFORDIV record;

                    if (isNew)
                    {
                        var duplicateDiv = _dbContext.SM_EXTRASEATFORDIV.Where(a => a.DIVISIONID == model.DIVISIONID).Count();
                        if (duplicateDiv > 0)
                        {
                            retVal = 2;
                            msg = "A record with this Division already exists.";
                            transaction.Rollback();
                            return Tuple.Create(retVal, msg);
                        }

                        record = new SM_EXTRASEATFORDIV
                        {
                            SRNO = (_dbContext.SM_EXTRASEATFORDIV.Max(x => (int?)x.SRNO) ?? 0) + 1,
                            DIVISIONID = model.DIVISIONID,
                            EXTRASEATCOUNT = model.EXTRASEATCOUNT,
                            STATUS = model.STATUS,
                            ADDEDBY = actionBy,
                            ADDEDON = DateTime.Now
                        };

                        _dbContext.Entry(record).State = EntityState.Added;
                        msg = "Record added successfully.";
                    }
                    else
                    {
                        var ExistSrno = _dbContext.SM_EXTRASEATFORDIV.Where(a => a.SRNO == model.SRNO).FirstOrDefault();
                        var withsameDivID = _dbContext.SM_EXTRASEATFORDIV.Where(a => a.DIVISIONID == model.DIVISIONID).FirstOrDefault();

                        if (ExistSrno.SRNO != withsameDivID.SRNO)
                        {
                            retVal = 2;
                            msg = "A record with this Division already exists.";
                            transaction.Rollback();
                            return Tuple.Create(retVal, msg);
                        }

                        record = _dbContext.SM_EXTRASEATFORDIV.FirstOrDefault(x => x.SRNO == model.SRNO);

                        if (record == null)
                        {
                            retVal = 3;
                            msg = "Record not found for update.";
                            transaction.Rollback();
                            return Tuple.Create(retVal, msg);
                        }

                        record.DIVISIONID = model.DIVISIONID;
                        record.EXTRASEATCOUNT = model.EXTRASEATCOUNT;
                        record.UPDATEDBY = actionBy;
                        record.UPDATEDON = DateTime.Now;

                        _dbContext.Entry(record).State = EntityState.Modified;
                        msg = "Record updated successfully.";
                    }

                    _dbContext.SaveChanges();
                    transaction.Commit();
                    retVal = 1;
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    msg = "An error occurred while saving the record." + ex.Message.ToString();
                    transaction.Rollback();
                }
            }

            return Tuple.Create(retVal, msg);
        }

        public short UpdateExtraSeatForDivStatus(long Srno, long ActionBy, short status)
        {
            short retVal = 0;
            if (Srno > 0)
            {
                SM_EXTRASEATFORDIV DT = _dbContext.SM_EXTRASEATFORDIV.Where(x => x.SRNO == Srno).FirstOrDefault();
                if (DT != null)
                {
                    DT.STATUS = status;
                    DT.UPDATEDBY = ActionBy;
                    DT.UPDATEDON = DateTime.Now;
                    _dbContext.Entry(DT).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }
        // End :: Added by aumento : SR100656

        //SR102091 Start
        public bool IsNewJoineeDay(DateTime date_, long adempcode)
        {
            DateTime _DOJ = _dbContext.ADEMPLOYEE
                                      .Where(c => c.ADEMPCODE == adempcode && c.ACTIVE == 1)
                                      .Select(s => s.REGDATE)
                                      .FirstOrDefault();

            var MDays = _dbContext.SM_SETTING_MST
                                  .Where(c => c.PARM_NAME == "WFO_MANDATORY_DAYS")
                                  .Select(s => s.PARM_VALUE)
                                  .FirstOrDefault();

            int days = Convert.ToInt32(MDays);   // convert string → int
            DateTime _ToDate = _DOJ.AddDays(days - 1); // exactly 30 days from joining
            bool IsNewJoineeDay_ = date_ >= _DOJ && date_ <= _ToDate;
            return IsNewJoineeDay_;
        }

        public bool IsInActiveEmployee(DateTime date_, long adempcode)
        {
            long? AdempCode = _dbContext.SM_EMP_ACTIVEINACTIVEFORROSTER.Where(c => c.ADEMPCODE == adempcode && c.ACTIVITY_STATUS == 0).Select(s => s.ADEMPCODE).FirstOrDefault();
            bool IsInActiveEmp_ = false;
            if (AdempCode != null && AdempCode > 0)
            {
                IsInActiveEmp_ = true;
            }
            return IsInActiveEmp_;
        }
        public List<SeatSettingViewModel> GetWFO_Mandatory_Days()
        {

            try
            {
                var list = (from v in _dbContext.SM_SETTING_MST
                            where v.PARM_NAME == "WFO_MANDATORY_DAYS"
                            select new SeatSettingViewModel
                            {
                                PARM_NAME = v.PARM_NAME,
                                PARM_VALUE = v.PARM_VALUE,
                                DESCRIPTION = v.DESCRIPTION
                            }).ToList();
                return list;
            }
            catch (Exception ex)
            {
                return new List<SeatSettingViewModel>();
            }

        }
        public bool Update_WFO_Days(string pName, string days, long ModifiedBy)
        {
            try
            {
                var list = _dbContext.SM_SETTING_MST.FirstOrDefault(r => r.PARM_NAME == pName);
                if (list != null)
                {
                    UpdateWFO_Mandatorydays_log(pName, days, ModifiedBy);
                    list.PARM_VALUE = days;
                    _dbContext.SaveChanges();
                    return true;
                    //success
                }
                else
                {
                    return false;
                    //no value found
                }
            }
            catch (Exception ex)
            {
                return false; // Database error
            }

        }
        //private void UpdateWFO_Mandatorydays_log(string pName, string days, long ModifiedBy)
        //{
        //    //var maxId = _dbContext.SM_WFOMANDATORYDAYS_LOG.Any() ? _dbContext.SM_WFOMANDATORYDAYS_LOG.Max(r => r.HISTORY_ID) : 0;

        //    long maxId=0;
        //    var checkStatus = _dbContext.SM_WFOMANDATORYDAYS_LOG.Count();

        //    if(checkStatus>0)
        //    {
        //        maxId = _dbContext.SM_WFOMANDATORYDAYS_LOG.Max(r => r.HISTORY_ID);
        //    }            


        //        var roster = new SM_WFOMANDATORYDAYS_LOG
        //        {
        //            HISTORY_ID = (maxId != 0) ? maxId + 1 : 1,
        //            DAY_VALUE = Convert.ToInt64(days),
        //            MODIFIED_BY = ModifiedBy,
        //            MODIFIED_DATE = DateTime.Now,
        //            PARAM_NAME = pName, //defualt inactive entry
        //        };
        //    _dbContext.SM_WFOMANDATORYDAYS_LOG.Add(roster);
        //    _dbContext.SaveChanges();
        //}


        private void UpdateWFO_Mandatorydays_log(string pName, string days, long ModifiedBy)
        {
            long maxId = 0;
            var checkStatus = _dbContext.SM_WFOMANDATORYDAYS_LOG.Count();

            if (checkStatus > 0)
            {
                maxId = _dbContext.SM_WFOMANDATORYDAYS_LOG.Max(r => r.HISTORY_ID);
            }

            long newId = (maxId != 0) ? maxId + 1 : 1;

            string sql = @"INSERT INTO SM_WFOMANDATORYDAYS_LOG (HISTORY_ID, DAY_VALUE, MODIFIED_BY, MODIFIED_DATE, PARAM_NAME)
                   VALUES ({0}, {1}, {2}, {3}, {4})";

            _dbContext.Database.ExecuteSqlRaw(sql, newId, Convert.ToInt64(days), ModifiedBy, DateTime.Now, pName);
        }


        public List<EmpActiveInactiveforRosterViewModel> GetAssociatedata(SearchSeatViewModel SRVM)
        {
            List<EmpActiveInactiveforRosterViewModel> _EmpList = new List<EmpActiveInactiveforRosterViewModel>();
            try
            {
                var q1 = from data in _dbContext.VW_ASSOCIATELVLDETAILS
                         join _emp in _dbContext.ADEMPLOYEE.Where(w => w.ACTIVE == 1) on data.ADEMPCODE equals _emp.ADEMPCODE
                         where data.ACTIVE == 1
                               && data.SYKI == _Syki.SYKIID
                               && (SRVM.OpId == 0 ? true : data.OPERATIONID == SRVM.OpId)
                               && (SRVM.DivId == 0 ? true : data.DIVISIONID == SRVM.DivId)
                               && (SRVM.Employee == 0 ? true : data.ADEMPCODE == SRVM.Employee)
                               && (data.ADFUNCTIONALDESIGNATIONID ?? 0) != 3
                               && (data.ADFUNCTIONALDESIGNATIONID ?? 0) != 4
                               && !(from SRA in _dbContext.SM_EMP_ACTIVEINACTIVEFORROSTER

                                    select SRA.ADEMPCODE).Contains(data.ADEMPCODE)
                         select new EmpActiveInactiveforRosterViewModel
                         {
                             EMPLOYEENAME = (_emp.FIRSTNAME + " " + _emp.LASTNAME).Trim(),
                             ADEMPCODE = _emp.ADEMPCODE,
                             ACTIVITY_STATUS = 1, // Defualt active
                             OPERATION = data.OPERATION,
                             DIVISION = data.DIVISION,
                             DEPARTMENT = data.DEPARTMENT,
                             CREATED_DATE = null,
                             UPDATED_DATE = null
                         };
                var q2 = from RA in _dbContext.SM_EMP_ACTIVEINACTIVEFORROSTER
                         join VW_TEMP in _dbContext.VW_ASSOCIATELVLDETAILS
                            on RA.ADEMPCODE equals VW_TEMP.ADEMPCODE into VW_GROUP
                         from VW in VW_GROUP.DefaultIfEmpty()
                         join EMP in _dbContext.ADEMPLOYEE.Where(W => W.ACTIVE == 1)
                            on RA.ADEMPCODE equals EMP.ADEMPCODE
                         where 
                            //VW != null
                            VW.ADEMPCODE != 0
                            && VW.SYKI == _Syki.SYKIID
                            && VW.ACTIVE == 1
                            && (SRVM.OpId == 0 ? true : VW.OPERATIONID == SRVM.OpId)
                            && (SRVM.DivId == 0 ? true : VW.DIVISIONID == SRVM.DivId)
                            && (SRVM.Employee == 0 ? true : VW.ADEMPCODE == SRVM.Employee)
                            && (SRVM.Status != -1 ? RA.ACTIVITY_STATUS == SRVM.Status : true)
                         select new EmpActiveInactiveforRosterViewModel
                         {
                             EMPLOYEENAME = (EMP.FIRSTNAME + " " + EMP.LASTNAME).Trim(),
                             ADEMPCODE = EMP.ADEMPCODE,
                             ACTIVITY_STATUS = RA.ACTIVITY_STATUS,
                             OPERATION = VW.OPERATION,
                             DIVISION = VW.DIVISION,
                             DEPARTMENT = VW.DEPARTMENT,
                             CREATED_DATE = RA.CREATED_DATE,
                             UPDATED_DATE = RA.UPDATED_DATE
                         };

                //var combinedResult = q1.Union(q2).ToList();
                var combinedResult = q1
                                       .Union(q2)
                                       .Where(r => SRVM.Status == -1 || r.ACTIVITY_STATUS == SRVM.Status)
                                       .OrderBy(r => r.OPERATION)
                                       .ThenBy(r => r.DIVISION)
                                       .ThenBy(r => r.DEPARTMENT)
                                       .ThenBy(r => r.ADEMPCODE)
                                       .ToList();
                _EmpList = combinedResult;
            }
            catch (Exception ex)
            {
                var t = ex.Message;
                _EmpList = new List<EmpActiveInactiveforRosterViewModel>();
            }
            return _EmpList;
        }
        public bool SaveEmployeeRoster(long empCode, int addedBy)
        {
            try
            {
                var record = _dbContext.SM_EMP_ACTIVEINACTIVEFORROSTER.FirstOrDefault(r => r.ADEMPCODE == empCode);
                if (record != null)
                {
                    short newStatus = (short)(record.ACTIVITY_STATUS == 1 ? 0 : 1);

                    if (newStatus == 0)
                    {
                        DeactivateFutureSeats(empCode, addedBy);
                    }

                    record.ADEMPCODE = empCode;
                    record.UPDATED_BY = addedBy;
                    record.UPDATED_DATE = DateTime.Now;
                    record.ACTIVITY_STATUS = newStatus;
                    //_dbContext.SaveChanges();

                    string sql = @"UPDATE SM_EMP_ACTIVEINACTIVEFORROSTER SET ADEMPCODE={0}, UPDATED_BY={1}, UPDATED_DATE={2}, ACTIVITY_STATUS={3} Where ADEMPCODE={4}";

                    _dbContext.Database.ExecuteSqlRaw(sql, empCode, addedBy,DateTime.Now, newStatus, empCode);

                    Updatelog(addedBy, record, newStatus);
                    return true;
                }
                else
                {
                    short defaultStatus = 0;
                    if (defaultStatus == 0)
                    {
                        DeactivateFutureSeats(empCode, addedBy);
                    }

                    //var maxId = _dbContext.SM_EMP_ACTIVEINACTIVEFORROSTER.Any() ? _dbContext.SM_EMP_ACTIVEINACTIVEFORROSTER.Max(r => r.ACTIVEINACTIVE_ID) : 0;

                    long maxId = 0;
                    var checkStatus = _dbContext.SM_EMP_ACTIVEINACTIVEFORROSTER.Count();

                    if (checkStatus > 0)
                    {
                        maxId = _dbContext.SM_EMP_ACTIVEINACTIVEFORROSTER.Max(r => r.ACTIVEINACTIVE_ID);
                    }

                    long newId = (maxId != 0) ? maxId + 1 : 1;

                    var roster = new SM_EMP_ACTIVEINACTIVEFORROSTER
                    {
                        ACTIVEINACTIVE_ID = newId,
                        ADEMPCODE = empCode,
                        ADDED_BY = addedBy,
                        CREATED_DATE = DateTime.Now,
                        ACTIVITY_STATUS = defaultStatus, //defualt inactive entry
                    };
                    //_dbContext.SM_EMP_ACTIVEINACTIVEFORROSTER.Add(roster);
                    //_dbContext.SaveChanges();

                    string sql = @"INSERT INTO SM_EMP_ACTIVEINACTIVEFORROSTER (ACTIVEINACTIVE_ID, ADEMPCODE, ADDED_BY, CREATED_DATE, ACTIVITY_STATUS)
                   VALUES ({0}, {1}, {2}, {3}, {4})";

                    _dbContext.Database.ExecuteSqlRaw(sql, newId, empCode, addedBy, DateTime.Now, defaultStatus);

                    Updatelog(addedBy, roster, defaultStatus);

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        
        private void DeactivateFutureSeats(long empCode, int addedBy)
        {
            var futureSeats = _dbContext.SM_ROSTER_TRN
                .Where(e => e.ADEMPCODE == empCode && e.ROSTER_DATE > DateTime.Today && e.STATUS == 1)
                .ToList();

            foreach (var seat in futureSeats)
            {
                seat.STATUS = 0;
                seat.UPDATEDBY = addedBy;
                seat.UPDATEDON = DateTime.Now;
            }
        }


        private void Updatelog(int addedBy, SM_EMP_ACTIVEINACTIVEFORROSTER record, short Status)
        {
            //var maxId = _dbContext.SM_EMP_ACTIVEINACTIVEFORROSTER_LOG.Any() ? _dbContext.SM_EMP_ACTIVEINACTIVEFORROSTER_LOG.Max(r => r.ACTIVEINACTIVE_HIS_ID) : 0;

            long maxId = 0;
            var checkStatus = _dbContext.SM_EMP_ACTIVEINACTIVEFORROSTER_LOG.Count();

            if (checkStatus > 0)
            {
                maxId = _dbContext.SM_EMP_ACTIVEINACTIVEFORROSTER_LOG.Max(r => r.ACTIVEINACTIVE_HIS_ID);
            }

            long newId = (maxId != 0) ? maxId + 1 : 1;

            //var rosterlog = new SM_EMP_ACTIVEINACTIVEFORROSTER_LOG
            //{
            //    ACTIVEINACTIVE_HIS_ID = (maxId != 0) ? maxId + 1 : 1,
            //    RID = record.ACTIVEINACTIVE_ID,
            //    ADEMPCODE = record.ADEMPCODE,
            //    ADDED_BY = record.ADDED_BY,
            //    CREATED_DATE = record.CREATED_DATE,
            //    UPDATED_DATE = DateTime.Now,
            //    UPDATED_BY = addedBy,
            //    ACTIVITY_STATUS = Status,
            //};
            //_dbContext.SM_EMP_ACTIVEINACTIVEFORROSTER_LOG.Add(rosterlog);
            //_dbContext.SaveChanges();

            string sql = @"INSERT INTO SM_EMP_ACTIVEINACTIVEFORROSTER_LOG (ACTIVEINACTIVE_HIS_ID, RID, ADEMPCODE, ADDED_BY, CREATED_DATE,UPDATED_DATE,UPDATED_BY,ACTIVITY_STATUS)
                   VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})";

            _dbContext.Database.ExecuteSqlRaw(sql, newId, record.ACTIVEINACTIVE_ID, record.ADEMPCODE, record.ADDED_BY, record.CREATED_DATE, DateTime.Now, addedBy, Status);
        }
        public int WFO_Mandatory_Days()
        {
            string rowval = _dbContext.SM_SETTING_MST.Where(e => e.PARM_NAME == "WFO_MANDATORY_DAYS").Select(e => e.PARM_VALUE).FirstOrDefault();
            int days = Convert.ToInt32(rowval);
            return days;
        }
        public List<EmpActiveInactiveforRosterLogViewModel> GetAssociateActiveInactiveRosterHistory(long empId)
        {
            List<EmpActiveInactiveforRosterLogViewModel> lst = new List<EmpActiveInactiveforRosterLogViewModel>();
            var ROSTERACTIVITY_HIS = from rlog in _dbContext.SM_EMP_ACTIVEINACTIVEFORROSTER_LOG
                                     join _emp in _dbContext.ADEMPLOYEE on rlog.ADEMPCODE equals _emp.ADEMPCODE
                                     join _emp_updated in _dbContext.ADEMPLOYEE on rlog.UPDATED_BY equals _emp_updated.ADEMPCODE
                                     where rlog.ADEMPCODE == empId
                                     select new EmpActiveInactiveforRosterLogViewModel
                                     {
                                         ID = rlog.ACTIVEINACTIVE_HIS_ID,
                                         RID = rlog.RID,
                                         ADEMPCODE = rlog.ADEMPCODE,
                                         CREATED_DATE = rlog.CREATED_DATE,
                                         UPDATED_DATE = rlog.UPDATED_DATE,
                                         //UPDATED_BY = rlog.UPDATED_BY,
                                         ADDED_BY = rlog.ADDED_BY,
                                         ACTIVITY_STATUS = rlog.ACTIVITY_STATUS,
                                         EMPLOYEENAME = (_emp.FIRSTNAME + " " + _emp.LASTNAME).Trim(),
                                         EMPLOYEENAME_Updated_by = (_emp_updated.FIRSTNAME + " " + _emp_updated.LASTNAME + " [" + _emp_updated.ADEMPCODE + "] ").Trim()
                                     };
            lst = ROSTERACTIVITY_HIS.OrderByDescending(e => e.ID).ToList();
            return lst;
        }
        public List<MandatoryWFODaysViewModel> GetWFOMandatoryDaysHistory(string Statement, string Description)
        {
            List<MandatoryWFODaysViewModel> lst = new List<MandatoryWFODaysViewModel>();
            var WFOMandatoryDays_history = from day_log in _dbContext.SM_WFOMANDATORYDAYS_LOG
                                           join _emp in _dbContext.ADEMPLOYEE on day_log.MODIFIED_BY equals _emp.ADEMPCODE
                                           where day_log.PARAM_NAME == Statement
                                           select new MandatoryWFODaysViewModel
                                           {
                                               History_id = day_log.HISTORY_ID,
                                               Day_value = day_log.DAY_VALUE,
                                               DESCRIPTION = Description,
                                               ModifiedDate = day_log.MODIFIED_DATE,
                                               //Modified_By = day_log.MODIFIED_BY,
                                               EMPLOYEENAME = (_emp.FIRSTNAME + " " + _emp.LASTNAME + " [" + _emp.ADEMPCODE + "] ").Trim()
                                           };

            lst = WFOMandatoryDays_history.OrderByDescending(e => e.History_id).ToList();
            return lst;
        }
        //SR102091 End
    }
}
