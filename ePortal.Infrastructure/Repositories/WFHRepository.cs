

using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ePortal.Infrastructure.Repositories
{
    public class WFHRepository
    {
        private EPortalDBContext _dbContext;
        private SYKI _Syki;
        public WFHRepository(EPortalDBContext objEPortalDBContext)
        {
            _dbContext = objEPortalDBContext;
            _Syki = _dbContext.SYKI.Where(x => x.ACTIVE == 1).FirstOrDefault();
        }

        public List<ShiftViewModel> GetShiftBySite(long SiteId, long? id)
        {
            DateTime Shift_ValidDate = DateTime.ParseExact("01-Apr-2022", "dd-MMM-yyyy", null);

            var iList = (from data in _dbContext.SYSITESHIFT
                         where data.DATEADDED >= Shift_ValidDate
                          && data.ACTIVE == 1 && data.SYSITEID == SiteId
                         && (id == null || data.SYSHIFTID == id)
                         select new ShiftViewModel
                         {
                             SYSHIFTID = data.SYSHIFTID,
                             CODE = data.CODE,
                             DESCRIP = data.DESCRIP,
                             ACTIVE = data.ACTIVE,
                             SYSITEID = data.SYSITEID,
                             START_TIME = data.START_TIME,
                             END_TIME = data.END_TIME,
                             HALFDAYHOUR = data.HALFDAYHOUR,
                             SAPSIFTDESC = data.SAPSIFTDESC
                         }).ToList();
            return iList;
        }

        public short GetOffDays(string startDate, string endDate, long siteId, long operationId)
        {
            long? _calSiteId = _dbContext.ASRCALOPMAPPING.Where(c => c.CALOPERATIONID == operationId && c.EMPSITE == siteId && c.ACTIVE == 1).Select(s => s.CALSITEID).FirstOrDefault();
            long _siteId = (_calSiteId == null || _calSiteId == 0) ? siteId : (long)_calSiteId;
            short offDaysCount = 0;
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                DateTime _sdate = DateTime.ParseExact(startDate, "dd-MMM-yyyy", null);
                DateTime _edate = DateTime.ParseExact(endDate, "dd-MMM-yyyy", null);
                offDaysCount = (short)_dbContext.HMSIHOLIDAYS.Where(h => h.MONTHDATEYEAR >= _sdate && h.MONTHDATEYEAR <= _edate && h.ACTIVE == 1 && h.SYSITEID == _siteId).Count();
            }
            return offDaysCount;
        }

        public WFHAppAuthViewModel GetApprovalAuth(long loginUser)
        {
            var obj = (from data in _dbContext.ADEMPDIVDEPTSECT.Where(d => d.ADEMPCODE == loginUser && d.SYKI == _Syki.SYKIID && d.ACTIVE==1)
                       join _Emp in _dbContext.ADEMPLOYEE on data.SUPSUPERVISOREMPCODE equals _Emp.ADEMPCODE
                       join _RecEmp in _dbContext.ADEMPLOYEE on data.SUPERVISOREMPCODE equals _RecEmp.ADEMPCODE
                       select new WFHAppAuthViewModel
                       {
                           EmpCode = data.SUPSUPERVISOREMPCODE,
                           EmpName = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " [" + _Emp.ADEMPCODE + "]",
                           RecEmpCode = data.SUPERVISOREMPCODE,
                           RecEmpName = _RecEmp.FIRSTNAME + " " + _RecEmp.LASTNAME + " [" + _RecEmp.ADEMPCODE + "]"
                       }).FirstOrDefault();

            return obj;
        }

        public Tuple<short, long> SaveWFHRequest(ASRWFH_HEADER_ViewModel AVM)
        {
            short retVal = 0; long retHeaderId = 0;
            Tuple<short, long> _retVal_tuple;
            DateTime WFH_ValidDate = DateTime.ParseExact("20-May-2020", "dd-MMM-yyyy", null);
            DateTime REM_ValidDate = DateTime.ParseExact("01-Jun-2020", "dd-MMM-yyyy", null);
            DateTime FLEXI_Validate = DateTime.ParseExact("01-Nov-2021", "dd-MMM-yyyy", null);

            if (AVM.STARTDATE < FLEXI_Validate && (AVM.WEHSHIFTID == 80 || AVM.WEHSHIFTID == 81))
            {
                retVal = 5;
                return _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId);
            }
            if (AVM.STARTDATE < REM_ValidDate && AVM.REQUESTTYPE == 2)
            {
                retVal = 4;
                return _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId);
            }
            if (AVM.STARTDATE < WFH_ValidDate && AVM.REQUESTTYPE == 1)
            {
                retVal = 3;
                return _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId);
            }

            //Auto WFH - WFH Request pending start
            long autoWFHCount = (from data in _dbContext.ASRWFH_HEADER
                                 //join _App in _dbContext.ASRWFH_APPROVALHIS on data.ASRWFHID equals _App.ASRWFHID
                                 where ((AVM.STARTDATE >= data.STARTDATE && AVM.STARTDATE <= data.ENDDATE)
                                 || (AVM.ENDDATE >= data.STARTDATE && AVM.ENDDATE <= data.ENDDATE))
                                 && data.ADEMPCODE == AVM.ADEMPCODE && data.STATUS == 0 && data.AUTO_WFH_STATUS == 1 && data.REQUESTTYPE == AVM.REQUESTTYPE 
                                 && data.APPLYFOR == AVM.APPLYFOR
                                 //&& (_App.ISRECAPPROVED == 0 || (_App.ISRECAPPROVED == 1 && (_App.ISAPPAPPROVED == 1 || _App.ISAPPAPPROVED == 0)))
                                 select data).Count();
            if (autoWFHCount > 0)
            {
                retVal = 6;
                return _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId);
            }
            //Auto WFH - WFH Request pending end

            long record_Count = (from data in _dbContext.ASRWFH_HEADER
                                 join _App in _dbContext.ASRWFH_APPROVALHIS on data.ASRWFHID equals _App.ASRWFHID
                                 where ((AVM.STARTDATE >= data.STARTDATE && AVM.STARTDATE <= data.ENDDATE)
                                 || (AVM.ENDDATE >= data.STARTDATE && AVM.ENDDATE <= data.ENDDATE))
                                 && data.ADEMPCODE == AVM.ADEMPCODE && data.STATUS == 1
                                 && data.REQUESTTYPE == AVM.REQUESTTYPE && data.APPLYFOR == AVM.APPLYFOR
                                 && (_App.ISRECAPPROVED == 0 || (_App.ISRECAPPROVED == 1 && (_App.ISAPPAPPROVED == 1 || _App.ISAPPAPPROVED == 0)))
                                 // && (_App.ISAPPAPPROVED == 1 || _App.ISAPPAPPROVED == 0)
                                 select data).Count();

            ////_dbContext.ASRWFH_HEADER.Any(x => ((x.STARTDATE >= AVM.STARTDATE && x.ENDDATE <= AVM.ENDDATE) || (x.STARTDATE >= AVM.STARTDATE && x.STARTDATE <= AVM.ENDDATE) || (x.ENDDATE >= AVM.STARTDATE && x.ENDDATE <= AVM.ENDDATE)) && x.ADEMPCODE == AVM.ADEMPCODE && (x.EMPCANCELREMARK == "" || x.EMPCANCELREMARK == null))
            if (record_Count > 0)
            {
                retVal = 2;
                return _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId);
            }
            // ----------------save----------------------
            //if (AVM.REQUESTTYPE == null || AVM.STATUS == 0) //Auto WFH - WFH Request pending
            if (AVM.REQUESTTYPE == null) //Auto WFH - WFH Request pending
            {
                retVal = -1;
                return _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId);
            }

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    ASRWFH_HEADER _Header = new ASRWFH_HEADER();
                    if (AVM.ASRWFHID == 0)
                    {
                        retHeaderId = _dbContext.ASRWFH_HEADER.Count() == 0 ? (long)1 : _dbContext.ASRWFH_HEADER.Max(a => a.ASRWFHID) + 1;
                        _Header.ASRWFHID = retHeaderId;
                        _Header.REQUESTTYPE = AVM.REQUESTTYPE;
                        _Header.APPLYFOR = AVM.APPLYFOR;
                        _Header.DURATION = AVM.DURATION;
                        _Header.WEHSHIFTID = AVM.WEHSHIFTID;
                        _Header.STARTDATE = AVM.STARTDATE;
                        _Header.ENDDATE = AVM.ENDDATE;
                        _Header.STARTTIME = AVM.STARTTIME;
                        _Header.ENDTIME = AVM.ENDTIME;
                        _Header.REMARKS = AVM.REMARKS;
                        _Header.ADEMPCODE = AVM.ADEMPCODE;
                        _Header.ADDEDBY = AVM.ADDEDBY;
                        _Header.DATEADDED = AVM.DATEADDED;
                        _Header.STATUS = AVM.STATUS;
                        _dbContext.Entry(_Header).State = EntityState.Added;
                        _Header.WFH_REQUESTTYPE = AVM.WFH_REQUESTTYPE; //SR86752
                        _Header.WFH_REQUESTTYPEREMARKS = AVM.WFH_REQUESTTYPEREMARKS; //SR86752
                        _dbContext.SaveChanges();
                    }
                    WFHAppAuthViewModel AppAuth = GetApprovalAuth(AVM.ADEMPCODE);
                    ////// ----- Insert data in WFH approval table
                    ASRWFH_APPROVALHIS _Approval = new ASRWFH_APPROVALHIS();
                    _Approval.ASRWFHAPPROVALHIS = _dbContext.ASRWFH_APPROVALHIS.Count() == 0 ? (long)1 : _dbContext.ASRWFH_APPROVALHIS.Max(h => h.ASRWFHAPPROVALHIS) + 1;
                    _Approval.ASRWFHID = _Header.ASRWFHID;
                    _Approval.ISAPPAPPROVED = 0;
                    _Approval.APPADEMPCODE = AppAuth == null ? null : AppAuth.EmpCode;
                    _Approval.ISRECAPPROVED = 0;
                    _Approval.RECADEMPCODE = AppAuth == null ? null : AppAuth.RecEmpCode;
                    _dbContext.Entry(_Approval).State = EntityState.Added;
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
            _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId);
            return _retVal_tuple;
        }

        public short EditWFHRequest(ASRWFH_HEADER_ViewModel AVM)
        {
            short retVal = 0;
            DateTime WFH_ValidDate = DateTime.ParseExact("20-May-2020", "dd-MMM-yyyy", null);
            DateTime REM_ValidDate = DateTime.ParseExact("01-Jun-2020", "dd-MMM-yyyy", null);
            if (AVM.STARTDATE < REM_ValidDate && AVM.REQUESTTYPE == 2)
            {
                return 4;
            }
            if (AVM.STARTDATE < WFH_ValidDate && AVM.REQUESTTYPE == 1)
            {
                return 3;
            }

            long record_Count = (from data in _dbContext.ASRWFH_HEADER
                                 join _App in _dbContext.ASRWFH_APPROVALHIS on data.ASRWFHID equals _App.ASRWFHID
                                 where ((AVM.STARTDATE >= data.STARTDATE && AVM.STARTDATE <= data.ENDDATE)
                                || (AVM.ENDDATE >= data.STARTDATE && AVM.ENDDATE <= data.ENDDATE))
                                && data.ADEMPCODE == AVM.ADEMPCODE && data.STATUS == 1
                                && data.REQUESTTYPE == AVM.REQUESTTYPE && data.APPLYFOR == AVM.APPLYFOR
                                && (_App.ISRECAPPROVED == 0 || (_App.ISRECAPPROVED == 1 && (_App.ISAPPAPPROVED == 1 || _App.ISAPPAPPROVED == 0)))
                                && data.ASRWFHID != AVM.ASRWFHID
                                 select data).Count();


            if (record_Count > 0)
            {
                return 2;
            }



            // ---------------Edit------------------------ -

            //if (AVM.REQUESTTYPE == null || AVM.STATUS == 0) //Auto WFH - WFH Request pending
            if (AVM.REQUESTTYPE == null) //Auto WFH - WFH Request pending
            {
                return -1;
            }

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    ASRWFH_HEADER _Header = new ASRWFH_HEADER();
                    if (AVM.ASRWFHID > 0)
                    {
                        _Header = _dbContext.ASRWFH_HEADER.Where(a => a.ASRWFHID == AVM.ASRWFHID).FirstOrDefault();
                    }
                    if (_Header != null)
                    {
                        _Header.REQUESTTYPE = AVM.REQUESTTYPE;
                        _Header.APPLYFOR = AVM.APPLYFOR;
                        _Header.DURATION = AVM.DURATION;
                        _Header.WEHSHIFTID = AVM.WEHSHIFTID;
                        _Header.STARTDATE = AVM.STARTDATE;
                        _Header.ENDDATE = AVM.ENDDATE;
                        _Header.STARTTIME = AVM.STARTTIME;
                        _Header.ENDTIME = AVM.ENDTIME;
                        _Header.REMARKS = AVM.REMARKS;
                        _Header.ADEMPCODE = AVM.ADEMPCODE;
                        _Header.MODIFIEDBY = AVM.MODIFIEDBY;
                        _Header.DATELSTMOD = AVM.DATELSTMOD;
                        //_Header.STATUS = AVM.STATUS;  //Auto WFH - WFH Request pending
                        _Header.STATUS = 1;  //Auto WFH - WFH Request pending
                        _Header.WFH_REQUESTTYPE = AVM.WFH_REQUESTTYPE; //SR86752
                        _Header.WFH_REQUESTTYPEREMARKS = AVM.WFH_REQUESTTYPEREMARKS; //SR86752
                        _dbContext.Entry(_Header).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                        long? _asrWFHID = AVM.ASRWFHID;

                        var approvalContextModel = _dbContext.ASRWFH_APPROVALHIS.Where(x => x.ASRWFHID == _asrWFHID);

                        //Auto WFH - WFH Request pending start
                        //if (_dbContext.ASRWFH_APPROVALHIS.Where(x => x.ASRWFHID == _asrWFHID).Count() == 0)
                        //{
                        //    WFHAppAuthViewModel AppAuth = GetApprovalAuth(AVM.ADEMPCODE);
                        //    ASRWFH_APPROVALHIS _Approval = new ASRWFH_APPROVALHIS();
                        //    _Approval.ASRWFHAPPROVALHIS = _dbContext.ASRWFH_APPROVALHIS.Count() == 0 ? (long)1 : _dbContext.ASRWFH_APPROVALHIS.Max(h => h.ASRWFHAPPROVALHIS) + 1;
                        //    _Approval.ASRWFHID = _Header.ASRWFHID;
                        //    _Approval.ISAPPAPPROVED = 0;
                        //    _Approval.APPADEMPCODE = AppAuth == null ? null : AppAuth.EmpCode;
                        //    _Approval.ISRECAPPROVED = 0;
                        //    _Approval.RECADEMPCODE = AppAuth == null ? null : AppAuth.RecEmpCode;
                        //    _dbContext.Entry(_Approval).State = EntityState.Added;
                        //    _dbContext.SaveChanges();
                        //}
                        // changes from naveen start here
                        ASRWFH_APPROVALHIS _Approval = new ASRWFH_APPROVALHIS();
                        WFHAppAuthViewModel AppAuth = GetApprovalAuth(AVM.ADEMPCODE);
                        if (approvalContextModel.Count() == 0)// changes from naveen end here
                        {


                            _Approval.ASRWFHAPPROVALHIS = _dbContext.ASRWFH_APPROVALHIS.Count() == 0 ? (long)1 : _dbContext.ASRWFH_APPROVALHIS.Max(h => h.ASRWFHAPPROVALHIS) + 1;
                            _Approval.ASRWFHID = _Header.ASRWFHID;
                            _Approval.ISAPPAPPROVED = 0;
                            _Approval.APPADEMPCODE = AppAuth == null ? null : AppAuth.EmpCode;
                            _Approval.ISRECAPPROVED = 0;
                            _Approval.RECADEMPCODE = AppAuth == null ? null : AppAuth.RecEmpCode;
                            _dbContext.Entry(_Approval).State = EntityState.Added;
                            _dbContext.SaveChanges();
                        }
                        // changes from naveen start here
                        else if (approvalContextModel.FirstOrDefault().RECADEMPCODE == null && approvalContextModel.FirstOrDefault().APPADEMPCODE == null)
                        {
                            _Approval = _dbContext.ASRWFH_APPROVALHIS.Where(x => x.ASRWFHID == AVM.ASRWFHID).FirstOrDefault();
                            if (_Approval != null)
                            {

                                _Approval.APPADEMPCODE = AppAuth == null ? null : AppAuth.EmpCode;
                                _Approval.RECADEMPCODE = AppAuth == null ? null : AppAuth.RecEmpCode;
                                _dbContext.Entry(_Approval).State = EntityState.Modified;
                                _dbContext.SaveChanges();
                            }
                        }
                        // changes from naveen end here
                        //SA changes by TTL CR-6365
                        else if (approvalContextModel.FirstOrDefault().RECADEMPCODE != AppAuth.RecEmpCode)
                        {
                            _Approval = _dbContext.ASRWFH_APPROVALHIS.Where(x => x.ASRWFHID == AVM.ASRWFHID).FirstOrDefault();
                            if (_Approval != null)
                            {

                                _Approval.APPADEMPCODE = AppAuth == null ? null : AppAuth.EmpCode;
                                _Approval.RECADEMPCODE = AppAuth == null ? null : AppAuth.RecEmpCode;
                                _dbContext.Entry(_Approval).State = EntityState.Modified;
                                _dbContext.SaveChanges();
                            }
                        }
                        //EA changes by TTL CR-6365
                        //Auto WFH - WFH Request pending end
                        transaction.Commit();
                        retVal = 1;
                    }
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
            }
            return retVal;
        }

        //public short EditWFHRequest(ASRWFH_HEADER_ViewModel AVM)
        //{
        //    short retVal = 0;
        //    DateTime WFH_ValidDate = DateTime.ParseExact("20-May-2020", "dd-MMM-yyyy", null);
        //    DateTime REM_ValidDate = DateTime.ParseExact("01-Jun-2020", "dd-MMM-yyyy", null);
        //    if (AVM.STARTDATE < REM_ValidDate && AVM.REQUESTTYPE == 2)
        //    {
        //        return 4;
        //    }
        //    if (AVM.STARTDATE < WFH_ValidDate && AVM.REQUESTTYPE == 1)
        //    {
        //        return 3;
        //    }

        //    long record_Count = (from data in _dbContext.ASRWFH_HEADER
        //                         join _App in _dbContext.ASRWFH_APPROVALHIS on data.ASRWFHID equals _App.ASRWFHID
        //                         where ((AVM.STARTDATE >= data.STARTDATE && AVM.STARTDATE <= data.ENDDATE)
        //                        || (AVM.ENDDATE >= data.STARTDATE && AVM.ENDDATE <= data.ENDDATE))
        //                        && data.ADEMPCODE == AVM.ADEMPCODE && data.STATUS == 1
        //                        && data.REQUESTTYPE == AVM.REQUESTTYPE && data.APPLYFOR == AVM.APPLYFOR
        //                        && (_App.ISRECAPPROVED == 0 || (_App.ISRECAPPROVED == 1 && (_App.ISAPPAPPROVED == 1 || _App.ISAPPAPPROVED == 0)))
        //                        && data.ASRWFHID != AVM.ASRWFHID
        //                         select data).Count();


        //    if (record_Count > 0)
        //    {
        //        return 2;
        //    }



        //    // ---------------Edit------------------------ -

        //    //if (AVM.REQUESTTYPE == null || AVM.STATUS == 0) //Auto WFH - WFH Request pending
        //    if (AVM.REQUESTTYPE == null) //Auto WFH - WFH Request pending
        //    {
        //        return -1;
        //    }

        //    using (DbContextTransaction transaction = _dbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            ASRWFH_HEADER _Header = new ASRWFH_HEADER();
        //            if (AVM.ASRWFHID > 0)
        //            {
        //                _Header = _dbContext.ASRWFH_HEADER.Where(a => a.ASRWFHID == AVM.ASRWFHID).FirstOrDefault();
        //            }
        //            if (_Header != null)
        //            {
        //                _Header.REQUESTTYPE = AVM.REQUESTTYPE;
        //                _Header.APPLYFOR = AVM.APPLYFOR;
        //                _Header.DURATION = AVM.DURATION;
        //                _Header.WEHSHIFTID = AVM.WEHSHIFTID;
        //                _Header.STARTDATE = AVM.STARTDATE;
        //                _Header.ENDDATE = AVM.ENDDATE;
        //                _Header.STARTTIME = AVM.STARTTIME;
        //                _Header.ENDTIME = AVM.ENDTIME;
        //                _Header.REMARKS = AVM.REMARKS;
        //                _Header.ADEMPCODE = AVM.ADEMPCODE;
        //                _Header.MODIFIEDBY = AVM.MODIFIEDBY;
        //                _Header.DATELSTMOD = AVM.DATELSTMOD;
        //                //_Header.STATUS = AVM.STATUS;  //Auto WFH - WFH Request pending
        //                _Header.STATUS = 1;  //Auto WFH - WFH Request pending
        //                _dbContext.Entry(_Header).State = EntityState.Modified;
        //                _dbContext.SaveChanges();
        //                long? _asrWFHID = AVM.ASRWFHID;

        //                //Auto WFH - WFH Request pending start
        //                if (_dbContext.ASRWFH_APPROVALHIS.Where(x => x.ASRWFHID == _asrWFHID).Count() == 0)
        //                {
        //                    WFHAppAuthViewModel AppAuth = GetApprovalAuth(AVM.ADEMPCODE);
        //                    ASRWFH_APPROVALHIS _Approval = new ASRWFH_APPROVALHIS();
        //                    _Approval.ASRWFHAPPROVALHIS = _dbContext.ASRWFH_APPROVALHIS.Count() == 0 ? (long)1 : _dbContext.ASRWFH_APPROVALHIS.Max(h => h.ASRWFHAPPROVALHIS) + 1;
        //                    _Approval.ASRWFHID = _Header.ASRWFHID;
        //                    _Approval.ISAPPAPPROVED = 0;
        //                    _Approval.APPADEMPCODE = AppAuth == null ? null : AppAuth.EmpCode;
        //                    _Approval.ISRECAPPROVED = 0;
        //                    _Approval.RECADEMPCODE = AppAuth == null ? null : AppAuth.RecEmpCode;
        //                    _dbContext.Entry(_Approval).State = EntityState.Added;
        //                    _dbContext.SaveChanges();
        //                }
        //                //Auto WFH - WFH Request pending end
        //                transaction.Commit();
        //                retVal = 1;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            retVal = -1;
        //            transaction.Rollback();
        //        }
        //    }
        //    return retVal;
        //}

        public ASRWFH_HEADER_ViewModel GetWFHRequestDetail(long id)
        {
            var _header = (from data in _dbContext.ASRWFH_HEADER
                           join _ShiftJoin in _dbContext.SYSITESHIFT on data.WEHSHIFTID equals _ShiftJoin.SYSHIFTID into _ShiftJoin
                           from _Shift in _ShiftJoin.DefaultIfEmpty()
                           join _Emp in _dbContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
                           where data.ASRWFHID == id && (data.STATUS == 1 || data.STATUS == 0)
                           select new ASRWFH_HEADER_ViewModel
                           {
                               ASRWFHID = data.ASRWFHID,
                               REQUESTTYPE = data.REQUESTTYPE,
                               APPLYFOR = data.APPLYFOR,
                               DURATION = data.DURATION,
                               WEHSHIFTID = data.WEHSHIFTID,
                               WEHSHIFT = _Shift == null ? "" : _Shift.CODE,
                               STARTDATE = data.STARTDATE,
                               ENDDATE = data.ENDDATE,
                               STARTTIME = data.STARTTIME,
                               ENDTIME = data.ENDTIME,
                               REMARKS = data.REMARKS,
                               EMPCANCELREMARK = data.EMPCANCELREMARK,
                               ADEMPCODE = data.ADEMPCODE,
                               EmpName = _Emp.FIRSTNAME + " " + _Emp.LASTNAME,
                               EmpEmail = _Emp.EMAILID,
                               ADDEDBY = data.ADDEDBY,
                               DATEADDED = data.DATEADDED,
                               STATUS = data.STATUS //Auto WFH - WFH Request pending
                               ,WFH_REQUESTTYPE = data.WFH_REQUESTTYPE, //SR86752
                               WFH_REQUESTTYPEREMARKS = data.WFH_REQUESTTYPEREMARKS //SR86752
                           }).FirstOrDefault();

            List<ASRWFH_APPROVALHIS_ViewModel> list = GetWFHDetailForApproval(id);
            try
            {
                if (list.Count > 0)
                {
                    _header.ASRWFH_APPROVALHIS = list[0];
                }
                else
                {
                    _header.ASRWFH_APPROVALHIS = new ASRWFH_APPROVALHIS_ViewModel();
                }
            }
            catch (Exception ex)
            {

            }
            return _header;
        }

        public List<ASRWFH_APPROVALHIS_ViewModel> GetWFHDetailForApproval(long id)
        {
            var AppObj = (from AppHis in _dbContext.ASRWFH_APPROVALHIS
                          join _RecEmp in _dbContext.ADEMPLOYEE on AppHis.RECADEMPCODE equals _RecEmp.ADEMPCODE
                          join _AppEmp in _dbContext.ADEMPLOYEE on AppHis.APPADEMPCODE equals _AppEmp.ADEMPCODE
                          where AppHis.ASRWFHID == id
                          select new ASRWFH_APPROVALHIS_ViewModel
                          {
                              ASRWFHAPPROVALHIS = AppHis.ASRWFHAPPROVALHIS,
                              ASRWFHID = AppHis.ASRWFHID,
                              APPADEMPCODE = AppHis.APPADEMPCODE,
                              APPAPPROVEDDATE = AppHis.APPAPPROVEDDATE,
                              APPREMARKS = AppHis.APPREMARKS,
                              APP_EMAIL = _AppEmp.EMAILID,
                              ISAPPAPPROVED = AppHis.ISAPPAPPROVED,
                              RECADEMPCODE = AppHis.RECADEMPCODE,
                              RECAPPROVEDDATE = AppHis.RECAPPROVEDDATE,
                              RECREMARKS = AppHis.RECREMARKS,
                              REC_EMAIL = _RecEmp.EMAILID,
                              ISRECAPPROVED = AppHis.ISRECAPPROVED,
                              SAPUPDATEREMARKS = AppHis.SAPUPDATEREMARKS,
                              SAPUPDATESTATUS = AppHis.SAPUPDATESTATUS,
                          }).ToList();
            return AppObj;
        }


        public short UpdateWFHApproval(ASRWFH_APPROVALHIS_ViewModel AAVM)
        {
            short retVal = 0;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    ASRWFH_APPROVALHIS _Approval = new ASRWFH_APPROVALHIS();
                    if (AAVM.ASRWFHID > 0)
                    {
                        _Approval = _dbContext.ASRWFH_APPROVALHIS.FirstOrDefault(a => a.ASRWFHID == AAVM.ASRWFHID);
                        if (_Approval != null)
                        {
                            if (AAVM.ISAPPAPPROVED == 3) //// Cancel By requestor
                            {
                                _Approval.ISAPPAPPROVED = AAVM.ISAPPAPPROVED;
                                _Approval.ISRECAPPROVED = AAVM.ISAPPAPPROVED;
                                _Approval.DATEISTMOD = DateTime.Now;
                                _dbContext.Entry(_Approval).State = EntityState.Modified;
                                _dbContext.SaveChanges();

                                ASRWFH_HEADER _Header = _dbContext.ASRWFH_HEADER.FirstOrDefault(a => a.ASRWFHID == AAVM.ASRWFHID);
                                _Header.EMPCANCELREMARK = AAVM.APPREMARKS;
                                _Header.DATELSTMOD = DateTime.Now;
                                _dbContext.Entry(_Header).State = EntityState.Modified;
                                _dbContext.SaveChanges();

                                transaction.Commit();
                                retVal = 1;
                            }
                            else
                            { ////// Approve & Reject by approval authority
                                short IsCommit = 0;
                                if (_Approval.RECADEMPCODE == _Approval.APPADEMPCODE)
                                {
                                    _Approval.ISAPPAPPROVED = AAVM.ISAPPAPPROVED;
                                    _Approval.APPAPPROVEDDATE = DateTime.Now;
                                    _Approval.APPREMARKS = AAVM.APPREMARKS;
                                    _Approval.DATEISTMOD = DateTime.Now;

                                    _Approval.ISRECAPPROVED = AAVM.ISRECAPPROVED;
                                    _Approval.RECAPPROVEDDATE = DateTime.Now;
                                    _Approval.RECREMARKS = AAVM.RECREMARKS;
                                    _Approval.DATEISTMOD = DateTime.Now;
                                    IsCommit = 1;
                                }
                                else if (AAVM.APPADEMPCODE == _Approval.APPADEMPCODE)
                                {
                                    _Approval.ISAPPAPPROVED = AAVM.ISAPPAPPROVED;
                                    _Approval.APPAPPROVEDDATE = DateTime.Now;
                                    _Approval.APPREMARKS = AAVM.APPREMARKS;
                                    _Approval.DATEISTMOD = DateTime.Now;
                                    IsCommit = 1;
                                }
                                else if (AAVM.RECADEMPCODE == _Approval.RECADEMPCODE)
                                {
                                    _Approval.ISRECAPPROVED = AAVM.ISRECAPPROVED;
                                    _Approval.RECAPPROVEDDATE = DateTime.Now;
                                    _Approval.RECREMARKS = AAVM.RECREMARKS;
                                    _Approval.DATEISTMOD = DateTime.Now;

                                    IsCommit = 1;
                                }
                                if (IsCommit == 1)
                                {
                                    _dbContext.Entry(_Approval).State = EntityState.Modified;
                                    _dbContext.SaveChanges();
                                    transaction.Commit();
                                    retVal = 1;
                                }
                            }
                        }
                        //Auto WFH - WFH Request pending change start
                        else
                        {
                            if (AAVM.ISAPPAPPROVED == 3) //// Auto generated request cancel By requestor
                            {
                                ASRWFH_HEADER _Header = _dbContext.ASRWFH_HEADER.FirstOrDefault(a => a.ASRWFHID == AAVM.ASRWFHID);
                                _Header.EMPCANCELREMARK = AAVM.APPREMARKS;
                                _Header.DATELSTMOD = DateTime.Now;
                                _Header.AUTO_WFH_STATUS = 2;
                                _dbContext.Entry(_Header).State = EntityState.Modified;
                                _dbContext.SaveChanges();
                                transaction.Commit();
                                retVal = 1;
                            }
                        }
                        //Auto WFH - WFH Request pending change end
                    }
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
            }
            return retVal;
        }

        public short UpdateWFHApprovalList(ASRWFH_APPROVALHIS_ViewModel AAVM)
        {
            short retVal = 0;
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    //foreach (ASRWFH_APPROVALHIS_ViewModel AAVM in AAVMList)
                    //{
                    ASRWFH_APPROVALHIS _Approval = new ASRWFH_APPROVALHIS();
                    if (AAVM.ASRWFHID > 0)
                    {
                        _Approval = _dbContext.ASRWFH_APPROVALHIS.FirstOrDefault(a => a.ASRWFHID == AAVM.ASRWFHID);
                        if (_Approval != null)
                        {
                            short IsCommit = 0;
                            if (_Approval.RECADEMPCODE == _Approval.APPADEMPCODE)
                            {
                                _Approval.ISAPPAPPROVED = AAVM.ISAPPAPPROVED;
                                _Approval.APPAPPROVEDDATE = DateTime.Now;
                                _Approval.APPREMARKS = AAVM.APPREMARKS;
                                _Approval.DATEISTMOD = DateTime.Now;

                                _Approval.ISRECAPPROVED = AAVM.ISRECAPPROVED;
                                _Approval.RECAPPROVEDDATE = DateTime.Now;
                                _Approval.RECREMARKS = AAVM.RECREMARKS;
                                _Approval.DATEISTMOD = DateTime.Now;
                                IsCommit = 1;
                            }
                            else if (AAVM.APPADEMPCODE == _Approval.APPADEMPCODE)
                            {
                                _Approval.ISAPPAPPROVED = AAVM.ISAPPAPPROVED;
                                _Approval.APPAPPROVEDDATE = DateTime.Now;
                                _Approval.APPREMARKS = AAVM.APPREMARKS;
                                _Approval.DATEISTMOD = DateTime.Now;
                                IsCommit = 1;
                            }
                            else if (AAVM.RECADEMPCODE == _Approval.RECADEMPCODE)
                            {
                                _Approval.ISRECAPPROVED = AAVM.ISRECAPPROVED;
                                _Approval.RECAPPROVEDDATE = DateTime.Now;
                                _Approval.RECREMARKS = AAVM.RECREMARKS;
                                _Approval.DATEISTMOD = DateTime.Now;
                                IsCommit = 1;
                            }
                            if (IsCommit == 1)
                            {
                                _dbContext.Entry(_Approval).State = EntityState.Modified;
                                _dbContext.SaveChanges();
                            }
                        }
                    }
                    //}
                    transaction.Commit();
                    retVal = 1;
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
            }
            return retVal;
        }

        public List<ASRWFH_HEADER_ViewModel> GetWFHReport(WFHReportViewModel WRVM)
        {
            DateTime ReqDateFrom = DateTime.Now.Date;
            DateTime ReqDateTo = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(WRVM.FromDate))
            {
                ReqDateFrom = DateTime.ParseExact(WRVM.FromDate, "dd-MMM-yyyy", null);
            }
            if (!string.IsNullOrEmpty(WRVM.ToDate))
            {
                ReqDateTo = DateTime.ParseExact(WRVM.ToDate + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
            }
            
            var _headerList = (from data in _dbContext.ASRWFH_HEADER
                               join _App in _dbContext.ASRWFH_APPROVALHIS on data.ASRWFHID equals _App.ASRWFHID
                               join _ShiftJoin in _dbContext.SYSITESHIFT on data.WEHSHIFTID equals _ShiftJoin.SYSHIFTID into _ShiftJoin
                               from _Shift in _ShiftJoin.DefaultIfEmpty()
                               join _Emp in _dbContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
                               join _Vw in _dbContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _Vw.ADEMPCODE
                               where data.STATUS == 1 && _Vw.SYKI == _Syki.SYKIID
                               && (WRVM.OpId == 0 || _Vw.OPERATIONID == WRVM.OpId)
                               && (WRVM.DivId == 0 || _Vw.DIVISIONID == WRVM.DivId)
                               && (WRVM.DepId == 0 || _Vw.DEPARTMENTID == WRVM.DepId)
                               && (WRVM.SecId == null || _Vw.SECTIONID == WRVM.SecId)
                               && (WRVM.Employee == 0 || data.ADEMPCODE == WRVM.Employee)
                               && (WRVM.RequestType == 0 || data.REQUESTTYPE == WRVM.RequestType)
                               && (WRVM.FromDate == "" || data.DATEADDED >= ReqDateFrom)
                               && (WRVM.ToDate == "" || data.DATEADDED <= ReqDateTo)
                               && _App.ISAPPAPPROVED == 1
                               select new ASRWFH_HEADER_ViewModel
                               {
                                   ASRWFHID = data.ASRWFHID,
                                   REQUESTTYPE = data.REQUESTTYPE,
                                   APPLYFOR = data.APPLYFOR,
                                   DURATION = data.DURATION,
                                   WEHSHIFTID = data.WEHSHIFTID,
                                   WEHSHIFT = _Shift == null ? "" : _Shift.CODE,
                                   STARTDATE = data.STARTDATE,
                                   ENDDATE = data.ENDDATE,
                                   STARTTIME = data.STARTTIME,
                                   ENDTIME = data.ENDTIME,
                                   REMARKS = data.REMARKS,
                                   ADEMPCODE = data.ADEMPCODE,
                                   EmpName = _Emp.FIRSTNAME + " " + _Emp.LASTNAME,
                                   ADDEDBY = data.ADDEDBY,
                                   DATEADDED = data.DATEADDED,
                                   WFH_REQUESTTYPE = data.WFH_REQUESTTYPE, //SR86752
                                   WFH_REQUESTTYPEREMARKS = data.WFH_REQUESTTYPEREMARKS,  //SR86752
                                   OffDays = _dbContext.HMSIHOLIDAYS.Where(h => h.MONTHDATEYEAR >= data.STARTDATE && h.MONTHDATEYEAR <= data.ENDDATE && h.ACTIVE == 1 && h.SYSITEID == _Vw.SYSITEID).Count()
                               }).ToList();
            return _headerList.OrderByDescending(o => o.DATEADDED).ThenBy(t => t.EmpName).ToList();
        }

        public List<ADORGLEVEL> GetOrgLevelList(long typeId)
        {
            var iList = from data in _dbContext.ADORGLEVEL
                        where data.ACTIVE == 1 && data.SYKIID == _Syki.SYKIID && data.ADORGLEVELTYPEID == typeId
                        select data;
            return iList.ToList();
        }

        public List<WFHDivViewModel> BindDivision(long op_Id)
        {
            List<WFHDivViewModel> iList = new List<WFHDivViewModel>();
            var iColl = (from data in _dbContext.VW_ASSOCIATELVLDETAILS.Where(e => e.ACTIVE == 1
                         && e.SYKI == _Syki.SYKIID && e.DIVISIONID != null && e.DIVISIONID != 0
                         && e.OPERATIONID == (op_Id == 0 ? e.OPERATIONID : op_Id))
                         select new
                         {
                             data.DIVISIONID,
                             data.DIVISION
                         }).Distinct().ToList();
            foreach (var obj in iColl)
            {
                iList.Add(new WFHDivViewModel
                {
                    DIVISIONID = Convert.ToInt64(obj.DIVISIONID == null ? 0 : obj.DIVISIONID),
                    DIVISION = obj.DIVISION
                });
            }
            return iList;
        }

        public List<WFHDepViewModel> BindDepartment(long div_Id, long op_Id)
        {
            List<WFHDepViewModel> iList = new List<WFHDepViewModel>();
            var iColl = (from data in _dbContext.VW_ASSOCIATELVLDETAILS.Where(e => e.ACTIVE == 1
                         && e.SYKI == _Syki.SYKIID && e.DEPARTMENTID != null && e.DEPARTMENTID != 0
                         && e.DIVISIONID == (div_Id == 0 ? e.DIVISIONID : div_Id)
                         /*&& e.OPERATIONID == (op_Id == 0 ? e.OPERATIONID : op_Id)*/)
                         select new
                         {
                             data.DEPARTMENTID,
                             data.DEPARTMENT
                         }).Distinct().ToList();
            foreach (var obj in iColl)
            {
                iList.Add(new WFHDepViewModel
                {
                    DEPARTMENTID = Convert.ToInt64(obj.DEPARTMENTID == null ? 0 : obj.DEPARTMENTID),
                    DEPARTMENT = obj.DEPARTMENT
                });
            }
            return iList;
        }

        public List<WFHSecViewModel> BindSection(long dep_Id, long div_Id, long op_Id)
        {
            List<WFHSecViewModel> iList = new List<WFHSecViewModel>();
            var iColl = (from data in _dbContext.VW_ASSOCIATELVLDETAILS.Where(e => e.ACTIVE == 1
                         && e.SYKI == _Syki.SYKIID && e.SECTION != null && e.SECTIONID != 0
                         && e.DEPARTMENTID == (dep_Id == 0 ? e.DEPARTMENTID : dep_Id)
                         //&& e.DIVISIONID == (div_Id == 0 ? e.DIVISIONID : div_Id)
                         //&& e.OPERATIONID == (op_Id == 0 ? e.OPERATIONID : op_Id)
                         )
                         select new
                         {
                             data.SECTIONID,
                             data.SECTION
                         }).Distinct().ToList();
            foreach (var obj in iColl)
            {
                iList.Add(new WFHSecViewModel
                {
                    SECTIONID = Convert.ToInt64(obj.SECTIONID == null ? 0 : obj.SECTIONID),
                    SECTION = obj.SECTION
                });
            }
            return iList;
        }

        public ASRWFH_HEADER_ViewModel GetRejectedRequest(string startDate, string endDate, long EmpCode)
        {
            ASRWFH_HEADER_ViewModel _obj = new ASRWFH_HEADER_ViewModel();
            if (!string.IsNullOrEmpty(startDate))
            {
                DateTime _sdate = DateTime.ParseExact(startDate, "dd-MMM-yyyy", null);

                _obj = (from data in _dbContext.ASRWFH_HEADER
                        join _App in _dbContext.ASRWFH_APPROVALHIS on data.ASRWFHID equals _App.ASRWFHID
                        where data.ADEMPCODE == EmpCode
                        && data.STARTDATE == _sdate
                        && (_App.ISRECAPPROVED == 2 || _App.ISAPPAPPROVED == 2)
                        select new ASRWFH_HEADER_ViewModel
                        {
                            ASRWFHID = data.ASRWFHID,
                            REQUESTTYPE = data.REQUESTTYPE,
                            APPLYFOR = data.APPLYFOR,
                            DURATION = data.DURATION,
                            WEHSHIFTID = data.WEHSHIFTID,
                            STARTDATE = data.STARTDATE,
                            ENDDATE = data.ENDDATE,
                            STARTTIME = data.STARTTIME,
                            ENDTIME = data.ENDTIME,
                            ADEMPCODE = data.ADEMPCODE,
                            ASRWFH_APPROVALHIS = new ASRWFH_APPROVALHIS_ViewModel
                            {
                                ASRWFHAPPROVALHIS = _App.ASRWFHAPPROVALHIS,
                                ASRWFHID = _App.ASRWFHID,
                                APPADEMPCODE = _App.APPADEMPCODE,
                                APPAPPROVEDDATE = _App.APPAPPROVEDDATE,
                                ISAPPAPPROVED = _App.ISAPPAPPROVED,
                                RECADEMPCODE = _App.RECADEMPCODE,
                                RECAPPROVEDDATE = _App.RECAPPROVEDDATE,
                                ISRECAPPROVED = _App.ISRECAPPROVED,
                                DATEISTMOD = _App.DATEISTMOD
                            }
                        }).OrderByDescending(o => o.ASRWFHID).FirstOrDefault();
            }
            return _obj;
        }

        //--WFH issue correction
        public WFHAppAuthViewModel GetViewApprovalAuth(long _ReqID)
        {
            var obj = (from data in _dbContext.ASRWFH_APPROVALHIS.Where(a => a.ASRWFHID == _ReqID)
                       join _Emp in _dbContext.ADEMPLOYEE on data.APPADEMPCODE equals _Emp.ADEMPCODE
                       join _RecEmp in _dbContext.ADEMPLOYEE on data.RECADEMPCODE equals _RecEmp.ADEMPCODE
                       select new WFHAppAuthViewModel
                       {
                           EmpCode = data.APPADEMPCODE,
                           EmpName = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " [" + _Emp.ADEMPCODE + "]",
                           RecEmpCode = data.RECADEMPCODE,
                           RecEmpName = _RecEmp.FIRSTNAME + " " + _RecEmp.LASTNAME + " [" + _RecEmp.ADEMPCODE + "]"
                       }).FirstOrDefault();
            return obj;
        }
        //--WFH issue correction

        //SR86752 Start
        public List<ASRWFH_HEADER_ViewModel> GetWFHAdminReport(WFHReportViewModel WRVM)
        {
            DateTime ReqDateFrom = DateTime.Now.Date;
            DateTime ReqDateTo = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(WRVM.FromDate))
            {
                ReqDateFrom = DateTime.ParseExact(WRVM.FromDate, "dd-MMM-yyyy", null);
            }
            if (!string.IsNullOrEmpty(WRVM.ToDate))
            {
                ReqDateTo = DateTime.ParseExact(WRVM.ToDate + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
            }
            var _headerList = (from data in _dbContext.ASRWFH_HEADER
                               join _App in _dbContext.ASRWFH_APPROVALHIS on data.ASRWFHID equals _App.ASRWFHID
                               join _ShiftJoin in _dbContext.SYSITESHIFT on data.WEHSHIFTID equals _ShiftJoin.SYSHIFTID into _ShiftJoin
                               from _Shift in _ShiftJoin.DefaultIfEmpty()
                               join _Emp in _dbContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
                               join _ReqEmp in _dbContext.ADEMPLOYEE on _App.RECADEMPCODE equals _ReqEmp.ADEMPCODE
                               join _AppEmp in _dbContext.ADEMPLOYEE on _App.APPADEMPCODE equals _AppEmp.ADEMPCODE
                               join _Vw in _dbContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _Vw.ADEMPCODE
                               where data.STATUS == 1 && _Vw.SYKI == _Syki.SYKIID
                                  && (WRVM.OpId == 0 || _Vw.OPERATIONID == WRVM.OpId)
                               && (WRVM.DivId == 0 || _Vw.DIVISIONID == WRVM.DivId)
                               && (WRVM.DepId == 0 || _Vw.DEPARTMENTID == WRVM.DepId)
                               && (WRVM.SecId == 0 || _Vw.SECTIONID == WRVM.SecId)
                               && (WRVM.Employee == 0 || data.ADEMPCODE == WRVM.Employee)
                               && (WRVM.RequestType == 0 || data.REQUESTTYPE == WRVM.RequestType)
                               && (WRVM.FromDate == "" || data.DATEADDED >= ReqDateFrom)
                               && (WRVM.ToDate == "" || data.DATEADDED <= ReqDateTo)
                               && (WRVM.RequestStatus == null || _App.ISAPPAPPROVED == WRVM.RequestStatus)
                               select new ASRWFH_HEADER_ViewModel
                               {
                                   ASRWFHID = data.ASRWFHID,
                                   REQUESTTYPE = data.REQUESTTYPE,
                                   APPLYFOR = data.APPLYFOR,
                                   DURATION = data.DURATION,
                                   WEHSHIFTID = data.WEHSHIFTID,
                                   WEHSHIFT = _Shift == null ? "" : _Shift.CODE,
                                   STARTDATE = data.STARTDATE,
                                   ENDDATE = data.ENDDATE,
                                   STARTTIME = data.STARTTIME,
                                   ENDTIME = data.ENDTIME,
                                   REMARKS = data.REMARKS,
                                   ADEMPCODE = data.ADEMPCODE,
                                   EmpName = _Emp.FIRSTNAME + " " + _Emp.LASTNAME,
                                   ADDEDBY = data.ADDEDBY,
                                   DATEADDED = data.DATEADDED,
                                   WFH_REQUESTTYPE = data.WFH_REQUESTTYPE,
                                   WFH_REQUESTTYPEREMARKS = data.WFH_REQUESTTYPEREMARKS,
                                   STATUS = _App.ISAPPAPPROVED ?? 0,
                                   OffDays = _dbContext.HMSIHOLIDAYS.Where(h => h.MONTHDATEYEAR >= data.STARTDATE && h.MONTHDATEYEAR <= data.ENDDATE && h.ACTIVE == 1 && h.SYSITEID == _Vw.SYSITEID).Count(),
                                   RECOMMENDEDBY =  _ReqEmp.FIRSTNAME + " " + _ReqEmp.LASTNAME,
                                   ISRECAPPROVED = _App.ISRECAPPROVED,
                                   RECREMARKS = _App.RECREMARKS,
                                   RECAPPROVEDDATE = _App.RECAPPROVEDDATE,
                                   APPROVALBY =  _AppEmp.FIRSTNAME + " " + _AppEmp.LASTNAME,
                                   ISAPPAPPROVED = _App.ISAPPAPPROVED,
                                   APPREMARKS = _App.APPREMARKS,
                                   APPAPPROVEDDATE = _App.APPAPPROVEDDATE,
                                   AUTO_WFH_STATUS = data.AUTO_WFH_STATUS,
                                   RECADEMPCODE = _App.RECADEMPCODE,
                                   APPADEMPCODE = _App.APPADEMPCODE
                               }).ToList();
            return _headerList.OrderByDescending(o => o.DATEADDED).ThenBy(t => t.EmpName).ToList();
        }
        //SR86752 End
    }
}
