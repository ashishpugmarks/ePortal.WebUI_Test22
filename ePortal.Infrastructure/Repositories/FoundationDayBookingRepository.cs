

using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.ViewModels;

namespace ePortal.Infrastructure.Repositories
{
    public class FoundationDayBookingRepository
    {
        private EPortalDGITDBContext _dbContext;
        private EPortalDBContext _empLoginDBContext;
        private CommonRepository _cr;
        private SYKI_DGIT _Syki;

        public FoundationDayBookingRepository(EPortalDGITDBContext objEPortalDGITDBContext, EPortalDBContext objEPortalDBContext, CommonRepository objCommonRepository)
        {
            _dbContext = objEPortalDGITDBContext;
             _empLoginDBContext= objEPortalDBContext;
            _cr = objCommonRepository;
            _Syki = _dbContext.SYKI.Where(x => x.ACTIVE == 1).FirstOrDefault();
        }

        public FD_VALIDATION_ViewModel GetValidationData(long siteid)
        {
            long transport_value = 0;

            //Employee_Details emp = (Employee_Details)HttpContext.Current.Session["Employee"];
            if (GetParameterValue("Transport_allow_foundation_validation").Split(',').Contains(siteid.ToString()))
            {
                transport_value = 1;
            }
            var obj = (from data in _dbContext.FD_BOKKING_SLOT_MAP.Where(l => l.SITE_ID == siteid)
                       where (DateTime.Now >= data.STARTDATE && DateTime.Now <= data.ENDDATE)
                       select new FD_VALIDATION_ViewModel
                       {
                           BC_MST_VALIDATION_ID = data.ID,
                           VISIT_BOOKING_DATE = data.ALLOW_DATE,
                           VISIT_CANCEL_DATE = data.CANCELENDTIME,
                           VISIT_END_DATE = data.ENDDATE,
                           SITE_ID= transport_value,
                           VISIT_START_DATE = data.STARTDATE
                       }).OrderBy(m=>m.BC_MST_VALIDATION_ID).FirstOrDefault();
            return obj;
        }

        public FoundationDayBookingDtlViewModel GetMealByDate(long mealtype, DateTime bookingDate)
        {
            FoundationDayBookingDtlViewModel Meal_obj = (from data in _dbContext.BC_MEALS_AVAILABILITY
                                                         join _MealMst in _dbContext.BC_MST_MEALS on data.MEALS_ID equals _MealMst.BC_MEALID
                                                         where _MealMst.MEALTYPEID == mealtype && data.MEAL_MAPPING_DATE == bookingDate
                                                         select new FoundationDayBookingDtlViewModel
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

        public Tuple<short, string> SaveFamilyMealBooking(FoundationDayBookingTrnViewModel model)
        {
            short retVal = 0;
            string msg = "";
            Tuple<short, string> _tuple = new Tuple<short, string>(retVal, msg);
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    #region Check Validation
                    DateTime bookingDate = DateTime.Now.Date;
                    if (!string.IsNullOrEmpty(model.strBOOKED_DATE))
                    {
                        bookingDate = DateTime.ParseExact(model.strBOOKED_DATE, "dd-MM-yyyy", null);
                    }

                    DateTime preRecValidation = DateTime.Now.Date.AddDays(-90);
                    if (_dbContext.FD_VISITBOOKING_TRN.Where(x => x.ADDEDBY == model.ADDEDBY && x.MEAL_BOOKED_DATE > preRecValidation && x.MEAL_STATUS == 1).FirstOrDefault()!=null)
                    {
                        retVal = 2;
                        msg = model.strBOOKED_DATE;
                        transaction.Rollback();
                        return _tuple = new Tuple<short, string>(retVal, msg); //// -- record already exist.
                    }
                    #endregion
                    FD_VISITBOOKING_TRN GMB = new FD_VISITBOOKING_TRN();
                    if (_dbContext.FD_VISITBOOKING_TRN.Count() == 0)
                    {
                        GMB.BC_FAMILY_BOOKINGID = 1;
                    }
                    else
                    {
                        GMB.BC_FAMILY_BOOKINGID = _dbContext.FD_VISITBOOKING_TRN.Max(x => x.BC_FAMILY_BOOKINGID) + 1;
                    }
                    // #Upgrade Aumento Start :: CR7504
                    //GMB.TRANSPORT_TYPE = model.TRANSPORT_TYPE;
                    //GMB.VEHICLE_TYPE = model.VEHICLE_TYPE;
                    GMB.TRANSPORT_TYPE = model.TRANSPORT_TYPE == null ? 0 : model.TRANSPORT_TYPE;
                    GMB.VEHICLE_TYPE = model.VEHICLE_TYPE == null ? 0 : model.VEHICLE_TYPE;
                    // #Upgrade Aumento End :: CR7504
                    GMB.MEAL_BOOKED_DATE = bookingDate;
                    GMB.MEAL_STATUS = 1;
                    GMB.ADDEDBY = model.ADDEDBY;
                    GMB.SLOTID = model.lngbookingid;
                    GMB.ADDEDDATE = DateTime.Now;
                    GMB.WHATSAPPNUMBER = (long)model.WHATSAPPNUMBER;  // #Upgrade Aumento added :: CR7504
                    GMB.PERSONALEMAIL = model.PERSONALEMAIL;  // #Upgrade Aumento added :: CR7504
                    _dbContext.Entry(GMB).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                   
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

        public short SaveFamilyMealBookingDtl(FoundationDayBookingTrnViewModel model, List<FoundationDayBookingDtlViewModel> familyList)
        {
            short retVal = 0;
            if (model.BC_FAMILY_BOOKINGID > 0)
            {
                foreach (FoundationDayBookingDtlViewModel Gdtl in familyList)
                {
                    #region INSERT/MODIFY GUEST DTL
                    int dtlFlag = 0;
                    FD_VISITBOOKING_DTL BGD = new FD_VISITBOOKING_DTL();
                    if (Gdtl.BC_FAMILYMEALBOOKING_DTLID > 0)
                    {
                        BGD = _dbContext.FD_VISITBOOKING_DTL.Where(g => g.BC_FAMILYMEALBOOKING_DTLID == Gdtl.BC_FAMILYMEALBOOKING_DTLID).FirstOrDefault();
                    }
                    else
                    {
                        BGD = new FD_VISITBOOKING_DTL();
                        if (_dbContext.FD_VISITBOOKING_DTL.Count() == 0)
                        {
                            BGD.BC_FAMILYMEALBOOKING_DTLID = 1;
                        }
                        else
                        {
                            BGD.BC_FAMILYMEALBOOKING_DTLID = _dbContext.FD_VISITBOOKING_DTL.Max(x => x.BC_FAMILYMEALBOOKING_DTLID) + 1;
                        }
                        dtlFlag = 1;
                    }
                    BGD.BC_FAMILY_BOOKINGID = model.BC_FAMILY_BOOKINGID;
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
                    _dbContext.Entry(BGD).State = dtlFlag == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _dbContext.SaveChanges();
                    #endregion
                }
                retVal = 1;
            }
            return retVal;
        }

        public List<FoundationDayBookingTrnViewModel> GetFamilyBookedMealList(long Site, string fromDate, string toDate, long loginUser)
        {
            DateTime currentDate = DateTime.Now;
            string cancellationTime = "23:59:00";
            int fmy_MealDay = 0;
            FD_VALIDATION_ViewModel ValidationMst = GetValidationData(Site);
            

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
            var iList = (from data in _dbContext.FD_VISITBOOKING_TRN.Where(w => w.ADDEDBY == loginUser)
                         join _slotMst in _dbContext.FD_BOKKING_SLOT_MAP on data.SLOTID equals _slotMst.ID
                         select new FoundationDayBookingTrnViewModel
                         {
                             BC_FAMILY_BOOKINGID = data.BC_FAMILY_BOOKINGID,
                             MEAL_BOOKED_DATE = data.MEAL_BOOKED_DATE,
                             MEAL_STATUS = data.MEAL_STATUS,
                             cancellationtime=_slotMst.CANCELENDTIME,
                             TRANSPORT_TYPE=data.TRANSPORT_TYPE,
                             MEALBOOKING_DTL = (from _fmDtl in _dbContext.FD_VISITBOOKING_DTL.Where(d => d.BC_FAMILY_BOOKINGID == data.BC_FAMILY_BOOKINGID)
                                                select new FoundationDayBookingDtlViewModel
                                                {
                                                    BC_FAMILYMEALBOOKING_DTLID = _fmDtl.BC_FAMILYMEALBOOKING_DTLID,
                                                    BC_FAMILY_BOOKINGID = _fmDtl.BC_FAMILY_BOOKINGID,
                                                    MEMBER_NAME = _fmDtl.MEMBER_NAME,
                                                    RELATION_TYPE = _fmDtl.RELATION_TYPE,
                                                }).ToList(),
                             ADDEDBY = data.ADDEDBY,
                             ADDEDDATE = data.ADDEDDATE,
                         }).ToList();
            List<FoundationDayBookingTrnViewModel> newList = new List<FoundationDayBookingTrnViewModel>();
            foreach (var obj in iList)
            {
                DateTime validDate = DateTime.Now;//DateTime.ParseExact(obj.MEAL_BOOKED_DATE.ToString("dd-MMM-yyyy") + " " + cancellationTime, "dd-MMM-yyyy HH:mm:ss", null).AddDays(-fmy_MealDay);
                obj.IsEnableCancel = obj.cancellationtime > validDate ? true : false;
                newList.Add(obj);
            }
            return newList.OrderByDescending(o => o.MEAL_BOOKED_DATE).ToList();
           
        }
        public short CancelBooking(long id, long updatedBy)
        {
            short retVal = 0;
            try
            {
                FD_VISITBOOKING_TRN model = new FD_VISITBOOKING_TRN();
                model = _dbContext.FD_VISITBOOKING_TRN.Where(w => w.BC_FAMILY_BOOKINGID == id && w.ADDEDBY== updatedBy).FirstOrDefault();
                if (model != null)
                {
                    model.MEAL_STATUS = 2;
                    model.LSTMODIFIEDBY = updatedBy;
                    model.LSTMODDATE = DateTime.Now;
                    _dbContext.Entry(model).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
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
        public FoundationDayBookingTrnViewModel GetRequestDtlById(long id)
        {
            //var obj = (from data in _dbContext.FD_VISITBOOKING_TRN.Where(w => w.BC_FAMILY_BOOKINGID == id)
            //           join _slotMst in _dbContext.FD_BOKKING_SLOT_MAP on data.SLOTID equals _slotMst.ID
            //           join _AddBy in _dbContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
            //           join _VWAssociate in _dbContext.VW_ASSOCIATELVLDETAILS.Where(v => v.SYKI == _Syki.SYKIID) on data.ADDEDBY equals _VWAssociate.ADEMPCODE
            //           select new FoundationDayBookingTrnViewModel
            //           {
            //               BC_FAMILY_BOOKINGID = data.BC_FAMILY_BOOKINGID,
            //               MEAL_BOOKED_DATE = data.MEAL_BOOKED_DATE,
            //               MEAL_STATUS = data.MEAL_STATUS,
            //               MEALBOOKING_DTL = (from _fmDtl in _dbContext.FD_VISITBOOKING_DTL.Where(d => d.BC_FAMILY_BOOKINGID == data.BC_FAMILY_BOOKINGID)
            //                                  select new FoundationDayBookingDtlViewModel
            //                                  {
            //                                      BC_FAMILYMEALBOOKING_DTLID = _fmDtl.BC_FAMILYMEALBOOKING_DTLID,
            //                                      BC_FAMILY_BOOKINGID = _fmDtl.BC_FAMILY_BOOKINGID,
            //                                      MEMBER_NAME = _fmDtl.MEMBER_NAME,
            //                                      RELATION_TYPE = _fmDtl.RELATION_TYPE,
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
            //           }).FirstOrDefault();
            // Step 1: Fetch the main booking object
            var obj = (from data in _dbContext.FD_VISITBOOKING_TRN
                       join _slotMst in _dbContext.FD_BOKKING_SLOT_MAP on data.SLOTID equals _slotMst.ID
                       join _AddBy in _dbContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                       join _VWAssociate in _dbContext.VW_ASSOCIATELVLDETAILS.Where(v => v.SYKI == _Syki.SYKIID) on data.ADDEDBY equals _VWAssociate.ADEMPCODE
                       where data.BC_FAMILY_BOOKINGID == id
                       select new FoundationDayBookingTrnViewModel
                       {
                           BC_FAMILY_BOOKINGID = data.BC_FAMILY_BOOKINGID,
                           MEAL_BOOKED_DATE = data.MEAL_BOOKED_DATE,
                           MEAL_STATUS = data.MEAL_STATUS,
                           TRANSPORT_TYPE = data.TRANSPORT_TYPE,
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
                       }).FirstOrDefault();

            if (obj != null)
            {
                // Step 2: Populate the nested collection separately
                obj.MEALBOOKING_DTL = _dbContext.FD_VISITBOOKING_DTL
                    .Where(d => d.BC_FAMILY_BOOKINGID == obj.BC_FAMILY_BOOKINGID)
                    .Select(_fmDtl => new FoundationDayBookingDtlViewModel
                    {
                        BC_FAMILYMEALBOOKING_DTLID = _fmDtl.BC_FAMILYMEALBOOKING_DTLID,
                        BC_FAMILY_BOOKINGID = _fmDtl.BC_FAMILY_BOOKINGID,
                        MEMBER_NAME = _fmDtl.MEMBER_NAME,
                        RELATION_TYPE = _fmDtl.RELATION_TYPE,

                    }).ToList();
            }

            return obj;
        }
        public SearchFDREPORT_ViewModel FDReport(SearchFDREPORT_ViewModel vm)
        {
            DateTime frdate = DateTime.Now;
            DateTime trdate = DateTime.Now;
            if (!string.IsNullOrEmpty(vm.Startdate))
            {
                frdate = DateTime.ParseExact(vm.Startdate, "dd-MMM-yyyy", null);
            }
            if (!string.IsNullOrEmpty(vm.ENDDATE))
            {
                trdate = DateTime.ParseExact(vm.ENDDATE + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
            }

            var obj = (from data in _dbContext.FD_VISITBOOKING_TRN
                       join emp in _dbContext.ADEMPLOYEE on data.ADDEDBY equals emp.ADEMPCODE
                       join dtl in _dbContext.FD_VISITBOOKING_DTL on data.BC_FAMILY_BOOKINGID equals dtl.BC_FAMILY_BOOKINGID
                       join sl in _dbContext.FD_BOKKING_SLOT_MAP on data.SLOTID equals sl.ID
                       join s in _dbContext.SYSITE on sl.SITE_ID equals s.SYSITEID
                       where (vm.ecode == 0 ? true : data.ADDEDBY == vm.ecode)
                       && (vm.ReqStatus == -1 ? true : data.MEAL_STATUS == vm.ReqStatus)
                       && (string.IsNullOrEmpty(vm.Startdate) ? true : data.ADDEDDATE >= frdate)
                       && (string.IsNullOrEmpty(vm.ENDDATE) ? true : data.ADDEDDATE <= trdate)
                       select new FDREPORT_ViewModel
                       {
                            BC_FAMILYMEALBOOKING_DTLID=dtl.BC_FAMILYMEALBOOKING_DTLID,
                            BC_FAMILY_BOOKINGID=dtl.BC_FAMILY_BOOKINGID,
                            BookingDate=sl.ALLOW_DATE,
                            MEMBER_NAME=dtl.MEMBER_NAME,
                            RELATION_TYPE=dtl.RELATION_TYPE,
                            ReqEcode=data.ADDEDBY.ToString(),
                            RequestDate=data.ADDEDDATE,
                            Req_NAME=emp.FIRSTNAME +" "+emp.LASTNAME,
                            STATUS=data.MEAL_STATUS,
                            MobileNo=emp.TMOBILE,
                            SiteDesc=s.DESCRIP,
                           TRANSPORT_TYPE = data.TRANSPORT_TYPE,
                           VEHICLE_TYPE = data.VEHICLE_TYPE,
                           WHATSAPPNUMBER = data.WHATSAPPNUMBER, // #Upgrade Aumento added :: CR7504
                           PERSONALEMAIL = data.PERSONALEMAIL // #Upgrade Aumento added :: CR7504 
                       }).ToList();

            vm.SearchResult = obj;
            return vm;
        }

        public String GetParameterValue(string strParmaName)
        {
            var SelectedParameter = (from v in _empLoginDBContext.SYPARAMETERS
                                     where v.PARAMNAME == strParmaName
                                     select v
                                  ).ToList();
            return (SelectedParameter.FirstOrDefault().PARAMVALUE);
        }
    }
}