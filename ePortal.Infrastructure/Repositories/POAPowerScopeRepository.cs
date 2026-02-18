using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ePortal.Infrastructure.Repositories
{
    public class POAPowerScopeRepository
    {
        private EPortalDBContext _POADBContext;       
        private SYKI _Syki;
        public POAPowerScopeRepository(EPortalDBContext POADBContext)
        {
            _POADBContext = POADBContext;
            _Syki = _POADBContext.SYKI.Where(x => x.ACTIVE == 1).FirstOrDefault();
        }

        public Tuple<short, long> SavePOADetail(List<ADPOWERSCOPEMASTERViewModel> AdPowerScopeList, string Ecode)
        {
            short retVal = 0; long retHeaderId = 0;
            Tuple<short, long> _retVal_tuple;
            int FlagAdd = 0;
            int max = 0;
            using (var transaction = _POADBContext.Database.BeginTransaction())
            {
                try
                {
                    decimal ec = decimal.Parse(Ecode);

                    var Empcount = _POADBContext.ADPOWERSCOPEMASTER.Where(x => x.ADEMPCODE == ec).Count();
                    if (Empcount > 0)
                    {
                        var query = (from post in _POADBContext.ADPOWERSCOPEMASTER
                                     where post.ADEMPCODE == ec
                                     select new POAPowerScopeADPOWERMASTERViewModel
                                     {
                                         POWERCODE = post.POWERCODE

                                     }).ToList();


                        foreach (var item in query)
                        {

                            ADPOWERSCOPEMASTER DEL = _POADBContext.ADPOWERSCOPEMASTER.Single(x => x.ADEMPCODE == ec && x.POWERCODE == item.POWERCODE);
                            _POADBContext.Entry(DEL).State = EntityState.Deleted;

                            _POADBContext.SaveChanges();

                            max = max + 1;
                        }
                        //transaction.Commit();


                    }

                    var count = _POADBContext.ADPOWERSCOPEMASTER.Count().ToString();
                    if (count == "0")
                    {
                        max = 1;
                    }
                    else
                    {

                        max = Int32.Parse(_POADBContext.ADPOWERSCOPEMASTER.Max(i => i.SRNO).ToString()) + 1;

                    }
                    FlagAdd = 1;



                    if (AdPowerScopeList.Count() > 0)
                    {


                        foreach (var item in AdPowerScopeList)
                        {

                            ADPOWERSCOPEMASTER PSM = new ADPOWERSCOPEMASTER();

                            var PCODE = _POADBContext.ADPOWERMASTER.Where(x => x.POWERNAME == item.POWER.ToString().Trim()).Select(x => x.POWERCODE).FirstOrDefault().ToString();

                            PSM.SRNO = max;
                            PSM.ADEMPCODE = Int32.Parse(item.ADEMPCODE);
                            PSM.ATTACHMENTFILENAME = item.ATTACHMENTFILENAME;
                            PSM.POWERCODE = PCODE;
                            PSM.SCOPE = item.SCOPE;
                            PSM.REMARKS = item.REMARKS;
                            PSM.ADDEDBY = item.ADDEDBY;
                            PSM.DATEADDED = item.DATEADDED;
                            PSM.MODIFIEDBY = 0;

                            //_POADBContext.Entry(PSM).State = EntityState.Added;
                            _POADBContext.Entry(PSM).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                            _POADBContext.SaveChanges();

                            //_POADBContext.Entry(PSM).State = EntityState.Detached;

                            max = max + 1;
                        }
                        transaction.Commit();
                    }
                    retVal = 1;
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

        public List<POAPowerScopeADPOWERMASTERViewModel> GetPower(long empCode)
        {


            List<POAPowerScopeADPOWERMASTERViewModel> listdata = (from r in _POADBContext.ADPOWERMASTER
                                                                  orderby r.SRNO
                                                                  select new POAPowerScopeADPOWERMASTERViewModel()
                                                                  {
                                                                      POWERNAME = r.POWERNAME,
                                                                      POWERCODE = r.POWERCODE
                                                                  }).ToList();
            return listdata;
            //var _obj = (from data in _AcrDBContext.ADPOWERMASTER
            //            select data.POWERNAME).ToList();

        }
        public List<Employee_Details> PortalAutocompleteSuggestionsForPOA(string Key, string designation)
        {
            //int isSearchDesg = 0;


            //bool isNumeric = int.TryParse(Key, out n);
            //long _value = (isNumeric ? Convert.ToInt64(Key) : 0);

            long strKIID = (long)_POADBContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
            List<Employee_Details> empdata_ans = new List<Employee_Details>();

            var empdata = (from userdata in _POADBContext.ADEMPDIVDEPTSECT.Where(m => m.SYKI == strKIID)
                           join emp in _POADBContext.ADEMPLOYEE.Where(m => m.ACTIVE == 1) on userdata.ADEMPCODE equals emp.ADEMPCODE
                           //join d in _AcrDBContext.ADDESIGNATION.Where(m => m.ACTIVE == 1 ) on userdata.ADDESIGNATIONID equals d.ADDESIGNATIONID
                           join d in _POADBContext.ADDESIGNATION.Where(m => m.ACTIVE == 1) on userdata.ADDESIGNATIONID equals d.ADDESIGNATIONID
                           join fg in _POADBContext.ADFUNCTIONALDESIGNATION.Where(m => m.ACTIVE == 1) on userdata.ADFUNCTIONALDESIGNATIONID equals fg.ADFUNCTIONALDESIGNATIONID into ls
                           from fg in ls.DefaultIfEmpty()
                               //where (isSearchDesg == 1 ? (fg.DESCRIP.ToUpper().Contains(designation.ToUpper()) || d.DESCRIP.ToUpper().Contains(designation.ToUpper())) : true)
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
            //foreach (var portaluser in portalUserequery)
            //{
            //    portalUserDtos.Add(Mapper.Map<PORTALUSER, PortalUser>(portaluser));
            //}
            //empdata_ans = portalUserDtos;
            return portalUserDtos;

        }
        //==================================================================================================================================================================================

        public List<POAPowerScopeADPOWERMASTERViewModel> PortalAutocompleteSuggestionsForPOAPower(string Key, string designation)
        {
            //int isSearchDesg = 0;
            List<POAPowerScopeADPOWERMASTERViewModel> portalUserDtos = new List<POAPowerScopeADPOWERMASTERViewModel>();

            //bool isNumeric = int.TryParse(Key, out n);
            //long _value = (isNumeric ? Convert.ToInt64(Key) : 0);
            try
            {

                //long strKIID = (long)_POADBContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;


                List<POAPowerScopeADPOWERMASTERViewModel> empdata = new List<POAPowerScopeADPOWERMASTERViewModel>();

                empdata = (from emp in _POADBContext.ADPOWERMASTER.DefaultIfEmpty()
                           select new POAPowerScopeADPOWERMASTERViewModel
                           {
                               POWERCODE = emp.POWERCODE,
                               POWERNAME = emp.POWERNAME,
                               //PNAME = emp.POWERCODE + "-" + emp.POWERNAME,
                           }).ToList();

                //var empdata = (from emp in _POADBContext.ADPOWERMASTER.Where(x=>x.POWERCODE.StartsWith(Key) || x.POWERNAME.StartsWith(Key))                             
                //               select new
                //               {
                //                   POWERCODE = emp.POWERCODE,
                //                   POWERNAME = emp.POWERNAME,                              
                //                   _PNAME = emp.POWERCODE + " " + emp.POWERNAME,
                //               }).Distinct().ToList();

                //var empdata = _POADBContext.ADPOWERMASTER
                //                .Select(m => new { m.POWERCODE, m.POWERNAME })
                //                .Distinct()
                //                .ToList();



                portalUserDtos = (from userdata in empdata
                                  where (userdata.POWERCODE.ToString().StartsWith(Key) || userdata.POWERNAME.ToUpper().Contains(Key.ToUpper()))
                                  select new POAPowerScopeADPOWERMASTERViewModel
                                  {
                                      POWERCODE = userdata.POWERCODE,
                                      POWERNAME = userdata.POWERNAME,

                                  }).ToList();


            }
            catch (Exception e)
            {
                e.Message.ToString();
            }
            return portalUserDtos;
        }

        public List<ADPOWERSCOPEMASTERViewModel> GetPOADetailsByECode1(long Ecode)
        {
            //List<ADPOWERSCOPEMASTERViewModel> _obj = new List<ADPOWERSCOPEMASTERViewModel>();

            var _obj = (from data in _POADBContext.ADPOWERSCOPEMASTER.Where(x => x.ADEMPCODE == Ecode)
                        select new ADPOWERSCOPEMASTERViewModel
                        {
                            SRNO = data.SRNO,
                            POWER = data.POWERCODE,
                            SCOPE = data.SCOPE,
                            REMARKS = data.REMARKS

                        }).OrderBy(o => o.SRNO).ToList();

            return _obj;


            //    foreach (ADPOWERSCOPEMASTERViewModel obj in _POADBContext.ADPOWERSCOPEMASTER)
            //    {
            //        bool IsBeforeSendBackRecord = _POADBContext.ADPOWERSCOPEMASTER.Select<>
            //        if ((IsBeforeSendBackRecord ? (!_obj.acrAppHis.Any(r => r.ACRID == obj.ACRID && r.ADEMPCODE == obj.ADEMPCODE && r.ACRAPPHISTORY_ID > lastSendBackAppHisId)) : (!_obj.acrAppHis.Any(x => x.ACRID == obj.ACRID && x.ADEMPCODE == obj.ADEMPCODE))))
            //        {
            //            _obj.acrAppHis.Add(new ACRAppHistoryViewModel
            //            {
            //                ACRAPPHISTORY_ID = 0,
            //                ACRID = obj.ACRID,
            //                ADEMPCODE = obj.ADEMPCODE,
            //                APPROVAL_STATUS = 0,
            //                APPROVAL_REMARK = "",
            //                APPEMP_NAME = obj.ADEMPNAME + "[" + obj.ADEMPCODE + "]",
            //            });
            //        }
            //    }
            //}

        }
        public List<Root> GetPOADetailsByECode(string Ecode, string Power)
        {
            try
            {
                decimal EMpcode = decimal.Parse(Ecode);
                Power = string.IsNullOrEmpty(Power) == true ? "" : Power;
                if (EMpcode != 0 && Power == "")
                {
                    var query =
                  (from post in _POADBContext.ADPOWERSCOPEMASTER

                   join meta in _POADBContext.ADPOWERMASTER on post.POWERCODE equals meta.POWERCODE
                   where post.ADEMPCODE == EMpcode
                   select new Root
                   {
                       Power = meta.POWERNAME,
                       ADEMPCODE_ = post.ADEMPCODE,
                       Scope = post.SCOPE,
                       Remarks = post.REMARKS
                   }).ToList();
                    return query;
                }
                else if (EMpcode == 0 && Power != "")
                {
                    var query =
                    (from post in _POADBContext.ADPOWERSCOPEMASTER

                     join meta in _POADBContext.ADPOWERMASTER on post.POWERCODE equals meta.POWERCODE
                     where post.POWERCODE == Power
                     select new Root
                     {
                         Power = meta.POWERNAME,
                         ADEMPCODE_ = post.ADEMPCODE,
                         Scope = post.SCOPE,
                         Remarks = post.REMARKS
                     }).ToList();
                    return query;
                }
                else if (EMpcode != 0 && Power != "")
                {
                    var query =
                    (from post in _POADBContext.ADPOWERSCOPEMASTER
                     join meta in _POADBContext.ADPOWERMASTER on post.POWERCODE equals meta.POWERCODE
                     where post.ADEMPCODE == EMpcode && post.POWERCODE == Power
                     select new Root
                     {
                         Power = meta.POWERNAME,
                         ADEMPCODE_ = post.ADEMPCODE,
                         Scope = post.SCOPE,
                         Remarks = post.REMARKS
                     }).ToList();
                    return query;
                }
                else
                {
                    var query =
                   (from post in _POADBContext.ADPOWERSCOPEMASTER
                    join meta in _POADBContext.ADPOWERMASTER on post.POWERCODE equals meta.POWERCODE
                    select new Root
                    {
                        Power = meta.POWERNAME,
                        ADEMPCODE_ = post.ADEMPCODE,
                        Scope = post.SCOPE,
                        Remarks = post.REMARKS
                    }).ToList();
                    return query;
                }






            }
            catch (Exception ex)
            {

                throw ex;
            }



        }

        public List<Root> GetPowerScopeViewDetailAllData()
        {
            try
            {
                //var query1 =
                // (from post in _POADBContext.ADPOWERSCOPEMASTER
                //  join meta in _POADBContext.ADPOWERMASTER on post.POWERCODE equals meta.POWERCODE
                //  select new 
                //  {
                //      Power1 = meta.POWERNAME,
                //      ADEMPCODE1 = post.ADEMPCODE,
                //      Scope1 = post.SCOPE,
                //      Remarks1 = post.REMARKS
                //  }).ToList();


                var query =
                  (from post in _POADBContext.ADPOWERSCOPEMASTER
                   join meta in _POADBContext.ADPOWERMASTER on post.POWERCODE equals meta.POWERCODE

                   select new Root
                   {
                       Power = meta.POWERNAME,
                       ADEMPCODE_ = post.ADEMPCODE,
                       Scope = post.SCOPE,
                       Remarks = post.REMARKS
                   }).ToList();

                //List<Root> query = new List<Root>();

                //foreach (var item in query1)
                //{
                //    Root R1 = new Root();
                //    R1.Power = item.Power1;
                //    R1.ADEMPCODE = item.ADEMPCODE1.ToString();
                //    R1.Scope = item.Scope1;
                //    R1.Remarks = item.Remarks1;


                //    query.Add(R1);
                //}


                return query;
            }
            catch (Exception ex)
            {

                throw ex;
            }



        }

        public List<POAPowerScopeADPOWERMASTERViewModel> GetPowerMasterData()
        {
            try
            {
                var query =
                  (from post in _POADBContext.ADPOWERMASTER
                   orderby post.SRNO
                   select new POAPowerScopeADPOWERMASTERViewModel
                   {
                       SRNO = post.SRNO,
                       POWERCODE = post.POWERCODE,
                       POWERNAME = post.POWERNAME
                   }).ToList();
                return query;
            }
            catch (Exception ex)
            {

                throw ex;
            }



        }

        public List<Root> GetPOADetailsSelectedByECode(string Ecode)
        {


            decimal EMpcode = decimal.Parse(Ecode);

            try
            {
                var query =
                      (from post in _POADBContext.ADPOWERSCOPEMASTER

                       join meta in _POADBContext.ADPOWERMASTER on post.POWERCODE equals meta.POWERCODE
                       where post.ADEMPCODE == EMpcode
                       select new Root
                       {
                           Power = meta.POWERNAME,
                           Scope = post.SCOPE,
                           Remarks = post.REMARKS,
                           ATTACHMENTFILENAME = post.ATTACHMENTFILENAME
                       }).ToList();

                return query;

            }
            catch (Exception ex)
            {

                throw ex;
            }



        }
        public List<Root> GetScopeByEmpCode(string ecode, string PowerCode)

        {
            decimal EPCODE = decimal.Parse(ecode);

            try
            {

                var Result = (from r in _POADBContext.ADPOWERSCOPEMASTER
                              where r.ADEMPCODE == EPCODE && r.POWERCODE == PowerCode
                              select new Root
                              {
                                  Scope = r.SCOPE,
                                  Remarks = r.REMARKS,
                                  ATTACHMENTFILENAME = r.ATTACHMENTFILENAME
                              }).ToList();

                return Result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Tuple<short, long> SavePower(string PCODE, string PNAME)
        {
            short retVal = 0; long retHeaderId = 0;
            Tuple<short, long> _retVal_tuple;
            int FlagAdd = 0;
            int max = 0;
            using (var transaction = _POADBContext.Database.BeginTransaction())
            {
                try
                {

                    var count = _POADBContext.ADPOWERMASTER.Count().ToString();
                    if (count == "0")
                    {
                        max = 1;
                    }
                    else
                    {

                        max = Int32.Parse(_POADBContext.ADPOWERMASTER.Max(i => i.SRNO).ToString()) + 1;

                    }
                    FlagAdd = 1;

                    if (PCODE != "")
                    {

                        ADPOWERMASTER PSM = new ADPOWERMASTER();

                        PSM.SRNO = max;
                        PSM.POWERCODE = PCODE;
                        PSM.POWERNAME = PNAME;

                        //_POADBContext.Entry(PSM).State = EntityState.Added;
                        _POADBContext.Entry(PSM).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                        _POADBContext.SaveChanges();

                        //_POADBContext.Entry(PSM).State = EntityState.Detached;



                        transaction.Commit();
                    }
                    retVal = 1;
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
        public short DeleteAttachment(string POWERCODE, string POWERNAME, long SRNO)
        {
            short retVal = 0;
            if (!string.IsNullOrEmpty(POWERCODE) && !string.IsNullOrEmpty(POWERNAME) && SRNO > 0)
            {
                //ADPOWERMASTER DT = _POADBContext.ADPOWERMASTER.Where(x => x.POWERCODE == POWERCODE && x.POWERNAME == POWERNAME && x.SRNO == SRNO).FirstOrDefault();
                ADPOWERMASTER DT = _POADBContext.ADPOWERMASTER.Where(x => x.POWERCODE == POWERCODE && x.POWERNAME == POWERNAME).FirstOrDefault();
                if (DT != null)
                {
                    _POADBContext.ADPOWERMASTER.Remove(DT);
                    _POADBContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }

    }
}
