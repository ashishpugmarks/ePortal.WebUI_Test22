using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.DomainClasses;
using ePortal.ViewModels;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ePortal.Infrastructure.Repositories
{
    public class LoginDetailRepository
    {
        private ePortalEntities2 _IOMDBContext;
        private ICProcessEntities _ICDBContext;


        private SYKI _Syki;
        public LoginDetailRepository()
        {
            _IOMDBContext = new ePortalEntities2();
            _ICDBContext = new ICProcessEntities();
            _Syki = _IOMDBContext.SYKI.Where(x => x.ACTIVE == 1).FirstOrDefault();
        }


        public List<Employee_Details> PortalAutocompleteSuggestions(string Key)
        {


            long strKIID = (long)_IOMDBContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
            var empdata = (from userdata in _IOMDBContext.ADEMPDIVDEPTSECT.Where(m => m.SYKI == strKIID)
                           join emp in _IOMDBContext.ADEMPLOYEE.Where(m => m.ACTIVE == 1) on userdata.ADEMPCODE equals emp.ADEMPCODE
                           join d in _IOMDBContext.ADDESIGNATION.Where(m => m.ACTIVE == 1) on userdata.ADDESIGNATIONID equals d.ADDESIGNATIONID
                           join fg in _IOMDBContext.ADFUNCTIONALDESIGNATION.Where(m => m.ACTIVE == 1) on userdata.ADFUNCTIONALDESIGNATIONID equals fg.ADFUNCTIONALDESIGNATIONID into ls
                           from fg in ls.DefaultIfEmpty()
                           select new
                           {
                               ADEMPCODE = emp.ADEMPCODE,
                               FIRSTNAME = emp.FIRSTNAME,
                               LASTNAME = emp.LASTNAME,
                               _ENAME = emp.FIRSTNAME + " " + emp.LASTNAME,
                           }
                           ).ToList();
            List<Employee_Details> portalUserDtos = new List<Employee_Details>();
            portalUserDtos = (from userdata in empdata
                              where (userdata.ADEMPCODE.ToString().StartsWith(Key) || userdata.FIRSTNAME.ToUpper().Contains(Key.ToUpper()) || userdata.LASTNAME.ToUpper().Contains(Key.ToUpper()) || (userdata._ENAME.ToUpper()).Contains(Key.ToUpper()))
                              select new Employee_Details
                              {
                                  _ECode = userdata.ADEMPCODE,
                                  _EFirstName = userdata.FIRSTNAME,
                                  _ELastName = userdata.LASTNAME,
                              }).ToList();
            // Start Added by Aumento :: SR78338
            var ThirdPartyEmps = _IOMDBContext.TP_EMP_DETAIL
                .Where(j => j.IS_DEBOARDED == 0 && j.EMPCODE.ToString().StartsWith(Key) || j.ASSOCIATENAME.ToUpper().Contains(Key.ToUpper()))
                .Select(j => new Employee_Details
                {
                    _ECode = j.EMPCODE,
                    _EFirstName = j.ASSOCIATENAME,
                    _ELastName = "",
                })
                .ToList();

            portalUserDtos = portalUserDtos.Union(ThirdPartyEmps).ToList();
            // End Added by Aumento :: SR78338

            return portalUserDtos;
        }
        public List<VM_LoginDetail_SYApplication> BindApplication()
        {
            return (from Odata in _ICDBContext.SYAPPLICATION
                    where Odata.ACTIVE == 1
                    select new VM_LoginDetail_SYApplication
                    {
                        APPLICATION = Odata.DESCRIP,
                        ApplicationID = Odata.SYAPPLICATIONID,
                    }).ToList();
        }

        public SearchLoginDetail LoginDetailList(SearchLoginDetail VM)
        {
            return VM;           
        }
        public AddLoginIDHeader LoginDetail(AddLoginIDHeader VM)
        {
            AddLoginIDHeader objret = new AddLoginIDHeader();
            long ecode = Convert.ToInt64(VM.ecode);

            // Start Added by Aumento :: SR78338
            var Ademployees = (from k in _IOMDBContext.ADEMPLOYEE where k.ADEMPCODE == ecode && k.ACTIVE == 1 select k).FirstOrDefault();
            var ThirdpartyEmpSeries = (from k in _IOMDBContext.TP_EMP_DETAIL where k.EMPCODE == ecode select k).FirstOrDefault();


            try
            {
                if (Ademployees == null && ThirdpartyEmpSeries != null)
                {

                    var empObj = _IOMDBContext.TP_EMP_DETAIL.Where(emp => emp.EMPCODE == ecode).FirstOrDefault();
                    if (empObj == null)
                    {
                        objret = null;
                    }
                    else
                    {
                        var EmpName = empObj.ASSOCIATENAME;
                        var EmpCOde = empObj.EMPCODE;
                        var OwnshipCode = empObj.OWNERSHIP;


                        objret = (from data in _ICDBContext.VW_ASSOCIATELVLDETAILS1
                                  where data.SYKI == _Syki.SYKIID
                                     && data.ACTIVE == 1
                                     && data.ADEMPCODE == OwnshipCode
                                  select new AddLoginIDHeader
                                  {
                                      ecode = EmpCOde.ToString(),
                                      DEPTName = data.DEPARTMENT,
                                      DivisionName = data.DIVISION,
                                      OperationName = data.OPERATION,
                                      SECName = data.SECTION,
                                      EMPName = EmpName,
                                      SearchResult = (from DTL in _ICDBContext.EMP_LOGINDETAIL.Where(M => M.ECODE == EmpCOde)
                                                      join ap in _ICDBContext.SYAPPLICATION on DTL.APPSYSTEMID equals ap.SYAPPLICATIONID
                                                      select new VM_EMP_LOGINDETAIL
                                                      {
                                                          ADDEDBY = DTL.ADDEDBY,
                                                          APPSYSTEMID = DTL.APPSYSTEMID,
                                                          DATEADDED = DTL.DATEADDED,
                                                          ECODE = DTL.ECODE,
                                                          LOGINIDCREATED = DTL.LOGINIDCREATED,
                                                          STATUS = DTL.STATUS,
                                                          EMP_LOGINDETAILID = DTL.EMP_LOGINDETAILID,
                                                          MODIFIEDBY = DTL.MODIFIEDBY,
                                                          MODIFIEDDATE = DTL.MODIFIEDDATE,
                                                          ApplicationName = ap.DESCRIP,
                                                      }).ToList(),
                                      ApplicationList = (from lst in _ICDBContext.EMP_APPLICATIONMAP_AD.Where(m => m.ADEMPCODE == VM.UserLoginID)
                                                         join _app in _ICDBContext.SYAPPLICATION on lst.APPSYSTEMID equals _app.SYAPPLICATIONID
                                                         select new VM_EMP_APPLICATIONMAP_AD
                                                         {
                                                             APPSYSTEMID = _app.SYAPPLICATIONID,
                                                             ApplicationName = _app.DESCRIP
                                                         }).ToList(),
                                  }).FirstOrDefault();
                    }
                }

                else
                {
                    // End Added by Aumento :: SR78338


                    objret = (from data in _ICDBContext.VW_ASSOCIATELVLDETAILS1.Where(k => k.SYKI == _Syki.SYKIID)
                              join emp in _ICDBContext.ADEMPLOYEEs1 on data.ADEMPCODE equals emp.ADEMPCODE
                              where (data.ACTIVE == 1 && emp.ACTIVE == 1
                              && data.ADEMPCODE == ecode
                              )
                              select new AddLoginIDHeader
                              {
                                  ecode = data.ADEMPCODE.ToString(),
                                  DEPTName = data.DEPARTMENT,
                                  DivisionName = data.DIVISION,
                                  OperationName = data.OPERATION,
                                  SECName = data.SECTION,
                                  EMPName = emp.FIRSTNAME + " " + emp.LASTNAME
                                  //SearchResult = (from DTL in _ICDBContext.EMP_LOGINDETAIL.Where(M => M.ECODE == data.ADEMPCODE)
                                  //                join ap in _ICDBContext.SYAPPLICATION on DTL.APPSYSTEMID equals ap.SYAPPLICATIONID
                                  //                select new VM_EMP_LOGINDETAIL
                                  //                {
                                  //                    ADDEDBY = DTL.ADDEDBY,
                                  //                    APPSYSTEMID = DTL.APPSYSTEMID,
                                  //                    DATEADDED = DTL.DATEADDED,
                                  //                    ECODE = DTL.ECODE,
                                  //                    LOGINIDCREATED = DTL.LOGINIDCREATED,
                                  //                    STATUS = DTL.STATUS,
                                  //                    EMP_LOGINDETAILID = DTL.EMP_LOGINDETAILID,
                                  //                    MODIFIEDBY = DTL.MODIFIEDBY,
                                  //                    MODIFIEDDATE = DTL.MODIFIEDDATE,
                                  //                    ApplicationName = ap.DESCRIP,
                                  //                }).ToList(),
                                  //ApplicationList = (from lst in _ICDBContext.EMP_APPLICATIONMAP_AD.Where(m => m.ADEMPCODE == VM.UserLoginID)
                                  //                   join _app in _ICDBContext.SYAPPLICATION on lst.APPSYSTEMID equals _app.SYAPPLICATIONID
                                  //                   select new VM_EMP_APPLICATIONMAP_AD
                                  //                   {
                                  //                       APPSYSTEMID = _app.SYAPPLICATIONID,
                                  //                       ApplicationName = _app.DESCRIP
                                  //                   }
                                  //                 ).ToList(),
                              }).FirstOrDefault();

                    objret.SearchResult = (from DTL in _ICDBContext.EMP_LOGINDETAIL.Where(M => M.ECODE == objret.ecode)
                                           join ap in _ICDBContext.SYAPPLICATION on DTL.APPSYSTEMID equals ap.SYAPPLICATIONID
                                           select new VM_EMP_LOGINDETAIL
                                           {
                                               ADDEDBY = DTL.ADDEDBY,
                                               APPSYSTEMID = DTL.APPSYSTEMID,
                                               DATEADDED = DTL.DATEADDED,
                                               ECODE = DTL.ECODE,
                                               LOGINIDCREATED = DTL.LOGINIDCREATED,
                                               STATUS = DTL.STATUS,
                                               EMP_LOGINDETAILID = DTL.EMP_LOGINDETAILID,
                                               MODIFIEDBY = DTL.MODIFIEDBY,
                                               MODIFIEDDATE = DTL.MODIFIEDDATE,
                                               ApplicationName = ap.DESCRIP,
                                           }).ToList();

                    objret.ApplicationList = (from lst in _ICDBContext.EMP_APPLICATIONMAP_AD.Where(m => m.ADEMPCODE == VM.UserLoginID)
                                              join _app in _ICDBContext.SYAPPLICATION on lst.APPSYSTEMID equals _app.SYAPPLICATIONID
                                              select new VM_EMP_APPLICATIONMAP_AD
                                              {
                                                  APPSYSTEMID = _app.SYAPPLICATIONID,
                                                  ApplicationName = _app.DESCRIP
                                              }
                                     ).ToList();




                } //Added by Aumento :: SR78338

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return objret;
        }

        public Tuple<short, long> SaveIDDetail(VM_EMP_LOGINDETAIL model)
        {
            short retVal = 0; long retHeaderId = 0;
            Tuple<short, long> _retVal_tuple;
            using (var transaction = _ICDBContext.Database.BeginTransaction())
            {
                try
                {
                    EMP_LOGINDETAIL DPH = new EMP_LOGINDETAIL();
                    int FlagAdd = 0;
                    if (model.EMP_LOGINDETAILID > 0)
                    {
                        DPH = _ICDBContext.EMP_LOGINDETAIL.Where(x => x.EMP_LOGINDETAILID == model.EMP_LOGINDETAILID).SingleOrDefault();
                    }
                    else
                    {
                        DPH = new EMP_LOGINDETAIL();
                        if (_ICDBContext.EMP_LOGINDETAIL.Count() == 0)
                        {
                            DPH.EMP_LOGINDETAILID = 1;
                        }
                        else
                        {
                            DPH.EMP_LOGINDETAILID = _ICDBContext.EMP_LOGINDETAIL.Max(x => x.EMP_LOGINDETAILID) + 1;
                        }
                        FlagAdd = 1;
                    }

                    if (FlagAdd == 1)
                    {
                        if (_ICDBContext.EMP_LOGINDETAIL.Where(x => x.LOGINIDCREATED == model.LOGINIDCREATED.Trim().ToUpper() && x.STATUS == 1).Count() > 0)
                        {
                            _retVal_tuple = new Tuple<short, long>(3, 0);
                            return _retVal_tuple;
                        }
                        DPH.ADDEDBY = model.ADDEDBY;
                        DPH.DATEADDED = DateTime.Now;
                        DPH.APPSYSTEMID = model.APPSYSTEMID;
                        DPH.ECODE = model.ECODE;
                        DPH.LOGINIDCREATED = model.LOGINIDCREATED.Trim().ToUpper();
                        DPH.STATUS = 1;
                    }
                    else
                    {
                        DPH.MODIFIEDBY = model.MODIFIEDBY;
                        DPH.STATUS = model.STATUS;
                    }

                    _ICDBContext.Entry(DPH).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                    _ICDBContext.SaveChanges();
                    transaction.Commit();
                    retVal = 1;
                    retHeaderId = DPH.EMP_LOGINDETAILID;
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId);
                return _retVal_tuple;
            }
        }


    }
}
