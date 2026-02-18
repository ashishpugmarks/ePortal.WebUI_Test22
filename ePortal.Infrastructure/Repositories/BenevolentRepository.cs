using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.Shared.Interface;
using ePortal.ViewModels;

namespace ePortal.Infrastructure.Repositories
{
    public class BenevolentRepository
    {
        private readonly EPortalDBContext _dbContext;
        private readonly ISessionService _sessionService;
        public BenevolentRepository(EPortalDBContext dbContext, ISessionService sessionService)
        {
            _dbContext = dbContext;
            _sessionService = sessionService;
        }
        public List<BenevolentViewModel> GetBenevolentMSTList(SearchBenevolent sr)
        {
            List<BenevolentViewModel> data = (from m in _dbContext.ASR_BENEVOLENT_MST
                                              where (sr != null && sr.DemiseEmpCode != 0? m.EMPLOYEECODE == sr.DemiseEmpCode : true)
                                              orderby m.DEMISEDATE descending
                                              select new BenevolentViewModel
                                              {
                                                  ASR_BENEVOLENTID = m.ASR_BENEVOLENTID,
                                                  EMPLOYEECODE = m.EMPLOYEECODE,    
                                                  DEMISEDATE = m.DEMISEDATE,
                                                  DEMISEREASONE = m.DEMISEREASONE,
                                                  STATUS = m.STATUS,
                                                  TO_DT = m.TO_DT,
                                                  FROM_DT = m.FROM_DT,
                                                  EmpName = _dbContext.ADEMPLOYEE.Where(a => a.ADEMPCODE == m.EMPLOYEECODE).Select(a => a.FIRSTNAME + " " + a.LASTNAME).FirstOrDefault()
                                                  //DATEADDED = m.DATEADDED,
                                                  //ADDEDBY = m.ADDEDBY
                                              }).ToList();
            return data;
        }

        public List<BENEVOLENT_MST> GetBenevolentMST()
        {
            List<BENEVOLENT_MST> data = (from m in _dbContext.ASR_BENEVOLENT_MST
                                         select new BENEVOLENT_MST
                                         {
                                             ASR_BENEVOLENTID = m.ASR_BENEVOLENTID,
                                             EMPLOYEECODE = m.EMPLOYEECODE,
                                             DEMISEDATE = m.DEMISEDATE,
                                             DEMISEREASONE = m.DEMISEREASONE,
                                             STATUS = m.STATUS,
                                             TO_DT = m.TO_DT,
                                             FROM_DT = m.FROM_DT
                                             //DATEADDED = m.DATEADDED,
                                             //ADDEDBY = m.ADDEDBY
                                         }).ToList();
            return data;
        }

        public BENEVOLENT_MST GetEmpBenevolentMST(long id)
        {

            var data1 = _dbContext.ASR_BENEVOLENT_MST.Where(a => a.FROM_DT >= DateTime.Now).Count();
            BENEVOLENT_MST data = (from m in _dbContext.ASR_BENEVOLENT_MST
                                   where m.ASR_BENEVOLENTID == id
                                   select new BENEVOLENT_MST
                                   {
                                       ASR_BENEVOLENTID = m.ASR_BENEVOLENTID,
                                       EMPLOYEECODE = m.EMPLOYEECODE,
                                       DEMISEDATE = m.DEMISEDATE,
                                       DEMISEREASONE = m.DEMISEREASONE,
                                       ADDEDBY = (
                                                    _dbContext.ASR_BENEVOLENT_MST.Where(a => a.ASR_BENEVOLENTID == m.ASR_BENEVOLENTID
                                                    && m.FROM_DT > DateTime.Now
                                                    &&
                                                    //m.DATEADDED== CurDate
                                                    //DbFunctions.TruncateTime(m.DATEADDED) == DbFunctions.TruncateTime(DateTime.Now)
                                                m.DATEADDED.Date == DateTime.Now.Date
                                                    ).Count() == 1 ? 1 : 0
                                       ),
                                       STATUS = m.STATUS,
                                       TO_DT = m.TO_DT,
                                       FROM_DT = m.FROM_DT,
                                       DATEADDED = m.DATEADDED,

                                   }).FirstOrDefault();
            return data;
        }

        public short AddBenevolentMST(BENEVOLENT_MST mst)
        {
            short retVal = 0;
            try
            {
                long id = _dbContext.ASR_BENEVOLENT_MST.OrderByDescending(u => u.ASR_BENEVOLENTID).Select(a => a.ASR_BENEVOLENTID).FirstOrDefault();
                ASR_BENEVOLENT_MST data = new ASR_BENEVOLENT_MST();
                data.ASR_BENEVOLENTID = id + 1;
                data.EMPLOYEECODE = mst.EMPLOYEECODE;
                data.DEMISEDATE = mst.DEMISEDATE;
                data.DEMISEREASONE = mst.DEMISEREASONE;
                data.STATUS = _dbContext.ADEMPLOYEE.Where(a => a.ADEMPCODE == mst.EMPLOYEECODE).Select(a => a.ACTIVE).FirstOrDefault();
                data.FROM_DT = mst.FROM_DT;
                data.TO_DT = mst.TO_DT;
                data.DATEADDED = DateTime.Now;
                data.ADDEDBY = mst.ADDEDBY;
                _dbContext.ASR_BENEVOLENT_MST.Add(data);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }

        public short EditBenevolentMST(BENEVOLENT_MST mst)
        {
            short retVal = 0;
            try
            {
                //long id = _dbContext.ASR_BENEVOLENT_MST.OrderByDescending(u => u.ASR_BENEVOLENTID).Select(a => a.ASR_BENEVOLENTID).FirstOrDefault();
                ASR_BENEVOLENT_MST data = _dbContext.ASR_BENEVOLENT_MST.Find(mst.ASR_BENEVOLENTID);
                //data.ASR_BENEVOLENTID = id + 1;
                //data.EMPLOYEECODE = mst.EMPLOYEECODE;
                data.DEMISEDATE = mst.DEMISEDATE;
                data.DEMISEREASONE = mst.DEMISEREASONE;
                data.STATUS = Convert.ToInt16(mst.STATUS);
                data.FROM_DT = mst.FROM_DT;
                data.TO_DT = mst.TO_DT;
                data.DATELSTMOD = DateTime.Now;
                data.MODIFIEDBY = mst.ADDEDBY;
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }
        public List<Employee_Details> PortalAutocompleteSuggestions(string Key, string designation)
        {
            int isSearchDesg = 0;
            var oDesg = _dbContext.ADDESIGNATION.Where(m => m.ACTIVE == 1 && m.DESCRIP.ToUpper().Contains(designation.ToUpper())).ToList();
            var oFDesg = _dbContext.ADFUNCTIONALDESIGNATION.Where(m => m.ACTIVE == 1 && m.DESCRIP.ToUpper().Contains(designation.ToUpper())).ToList();

            if (oDesg.Count > 0 || oFDesg.Count > 0)
            {
                isSearchDesg = 1;
            }

            long strKIID = (long)_dbContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
            //var empdata = (from userdata in _dbContext.ADEMPDIVDEPTSECT.Where(m => m.SYKI == strKIID)
            //               join emp in _dbContext.ADEMPLOYEE.Where(m => m.ACTIVE == 1) on userdata.ADEMPCODE equals emp.ADEMPCODE
            //               join d in _dbContext.ADDESIGNATION.Where(m => m.ACTIVE == 1) on userdata.ADDESIGNATIONID equals d.ADDESIGNATIONID
            //               join _VW in _dbContext.VW_ASSOCIATELVLDETAILS.Where(m => m.ACTIVE == 1 && m.SYKI == strKIID) on emp.ADEMPCODE equals _VW.ADEMPCODE
            //               join fg in _dbContext.ADFUNCTIONALDESIGNATION.Where(m => m.ACTIVE == 1) on userdata.ADFUNCTIONALDESIGNATIONID equals fg.ADFUNCTIONALDESIGNATIONID into ls
            //               from fg in ls.DefaultIfEmpty()
            //               where (isSearchDesg == 1 ? (fg.DESCRIP.ToUpper().Contains(designation.ToUpper()) || d.DESCRIP.ToUpper().Contains(designation.ToUpper())) : true)
            //               select new
            //               {
            //                   ADEMPCODE = emp.ADEMPCODE,
            //                   FIRSTNAME = emp.FIRSTNAME,
            //                   LASTNAME = emp.LASTNAME,
            //                   _ENAME = emp.FIRSTNAME + " " + emp.LASTNAME,
            //                   Opration = _VW.OPERATION,
            //                   DepDesc = _VW.DEPARTMENT,
            //                   DivDesc = _VW.DIVISION,
            //                   SecDescrip = _VW.SECTION
            //               }
            //               ).ToList();

            var empdata = (from _VW in _dbContext.VW_ASSOCIATELVLDETAILS
                           join userdata in _dbContext.ADEMPLOYEE on _VW.ADEMPCODE equals userdata.ADEMPCODE
                           where (userdata.ADEMPCODE.ToString().StartsWith(Key) || userdata.FIRSTNAME.ToUpper().Contains(Key.ToUpper())
                           || userdata.LASTNAME.ToUpper().Contains(Key.ToUpper()))
                           select new
                           {
                               ADEMPCODE = userdata.ADEMPCODE,
                               FIRSTNAME = userdata.FIRSTNAME,
                               LASTNAME = userdata.LASTNAME,
                               _ENAME = userdata.FIRSTNAME + " " + userdata.LASTNAME,
                               Opration = _VW.OPERATION,
                               DepDesc = _VW.DEPARTMENT,
                               DivDesc = _VW.DIVISION,
                               SecDescrip = _VW.SECTION,
                               _syKIID = _VW.SYKI
                           }).OrderByDescending(o => o._syKIID).ToList().Take(1);

            List<Employee_Details> portalUserDtos = new List<Employee_Details>();
            portalUserDtos = (from userdata in empdata
                              where (userdata.ADEMPCODE.ToString().StartsWith(Key) || userdata.FIRSTNAME.ToUpper().Contains(Key.ToUpper()) || userdata.LASTNAME.ToUpper().Contains(Key.ToUpper()) || (userdata._ENAME.ToUpper()).Contains(Key.ToUpper()))
                              orderby userdata.ADEMPCODE
                              select new Employee_Details
                              {
                                  _ECode = userdata.ADEMPCODE,
                                  _EFirstName = userdata.FIRSTNAME,
                                  _ELastName = userdata.LASTNAME,
                                  _OpDesc = userdata.Opration,
                                  _DepDesc = userdata.DepDesc,
                                  _DivDesc = userdata.DivDesc,
                                  _SecDescrip = userdata.SecDescrip
                              }).ToList();

            return portalUserDtos;
        }

        public int GetEmpCode(long id)
        {
            int data = 0;
            data = _dbContext.ASR_BENEVOLENT_MST.Where(a => a.EMPLOYEECODE == id).Count();
            return data;
        }
        public BenevolentViewModel GetEmpDetail(long id)
        {
            BenevolentViewModel data = new BenevolentViewModel();
            long strKIID = (long)_dbContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
            //data = (from userdata in _dbContext.ADEMPDIVDEPTSECT.Where(m => m.SYKI == strKIID)
            //        join emp in _dbContext.ADEMPLOYEE.Where(m => m.ACTIVE == 1 && m.ADEMPCODE == id) on userdata.ADEMPCODE equals emp.ADEMPCODE
            //        join d in _dbContext.ADDESIGNATION.Where(m => m.ACTIVE == 1) on userdata.ADDESIGNATIONID equals d.ADDESIGNATIONID
            //        join _VW in _dbContext.VW_ASSOCIATELVLDETAILS.Where(m => m.ACTIVE == 1 && m.SYKI == strKIID) on emp.ADEMPCODE equals _VW.ADEMPCODE
            //        //join fg in _dbContext.ADFUNCTIONALDESIGNATION.Where(m => m.ACTIVE == 1) on userdata.ADFUNCTIONALDESIGNATIONID equals fg.ADFUNCTIONALDESIGNATIONID into ls
            //        //from fg in ls.DefaultIfEmpty()
            //        //where (isSearchDesg == 1 ? (fg.DESCRIP.ToUpper().Contains(designation.ToUpper()) || d.DESCRIP.ToUpper().Contains(designation.ToUpper())) : true)
            //        select new BenevolentViewModel
            //        {
            //            EmpCode = emp.ADEMPCODE,
            //            EmpName = emp.FIRSTNAME + " " + emp.LASTNAME,
            //            Opration = _VW.OPERATION,
            //            Department = _VW.DEPARTMENT,
            //            Division = _VW.DIVISION,
            //            Section = _VW.SECTION
            //        }
            //              ).FirstOrDefault();


            var empDetails = (from _VW in _dbContext.VW_ASSOCIATELVLDETAILS.Where(m => m.ADEMPCODE == id)
                              join emp in _dbContext.ADEMPLOYEE on _VW.ADEMPCODE equals emp.ADEMPCODE
                              select new
                              {
                                  EmpCode = emp.ADEMPCODE,
                                  EmpName = emp.FIRSTNAME + " " + emp.LASTNAME,
                                  Opration = _VW.OPERATION,
                                  Department = _VW.DEPARTMENT,
                                  Division = _VW.DIVISION,
                                  Section = _VW.SECTION,
                                  _syKIID = _VW.SYKI
                              }).OrderByDescending(o => o._syKIID).ToList().Take(1);

            data = (from emp in empDetails
                    select new BenevolentViewModel
                    {
                        EmpCode = emp.EmpCode,
                        EmpName = emp.EmpName,
                        Opration = emp.Opration,
                        Department = emp.Department,
                        Division = emp.Division,
                        Section = emp.Section
                    }).FirstOrDefault();

            return data;
        }

        public short AddContribution(BENEVOLENT_DT dt)
        {
            short retVal = 0;
            try
            {
                long id = _dbContext.ASR_BENEVOLENT_DT.OrderByDescending(u => u.ASR_BENEVOLENT_DTID).Select(a => a.ASR_BENEVOLENT_DTID).FirstOrDefault();
                ASR_BENEVOLENT_DT data = new ASR_BENEVOLENT_DT();
                data.ASR_BENEVOLENT_DTID = id + 1;
                data.ASR_BENEVOLENTID = dt.ASR_BENEVOLENTID;
                data.AMOUNT = dt.AMOUNT;
                data.CONSENTBYEMPLOYEE = dt.CONSENTBYEMPLOYEE;
                data.STATUS = _dbContext.ADEMPLOYEE.Where(a => a.ADEMPCODE == dt.CONSENTBYEMPLOYEE).Select(a => a.ACTIVE).FirstOrDefault();
                data.DATEADDED = DateTime.Now;
                data.ADDEDBY = dt.ADDEDBY;
                _dbContext.ASR_BENEVOLENT_DT.Add(data);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }

        public List<BenevolentViewModel> ContributionBenMSTList(SearchBenevolent sr)
        {
            DateTime CurDate = DateTime.Now.Date;
            var aa = (from d in _dbContext.ASR_BENEVOLENT_DT
                      where d.CONSENTBYEMPLOYEE == sr.UserId
                      select d.ASR_BENEVOLENTID).ToList();

            long strKIID = (long)_dbContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
            List<BenevolentViewModel> data = new List<BenevolentViewModel>();
            if (sr.Status == 1)
            {
                data = (from m in _dbContext.ASR_BENEVOLENT_MST
                        where
                         !aa.Contains(m.ASR_BENEVOLENTID)
                        &&
                        (CurDate >= m.FROM_DT && CurDate <= m.TO_DT)
                        && (sr.DemiseEmpCode != 0 ? m.EMPLOYEECODE == sr.DemiseEmpCode : true)
                        && m.STATUS == 1
                        orderby m.DEMISEDATE
                        select new BenevolentViewModel
                        {
                            ASR_BENEVOLENTID = m.ASR_BENEVOLENTID,
                            EMPLOYEECODE = m.EMPLOYEECODE,
                            EmpName = _dbContext.ADEMPLOYEE.Where(a => a.ADEMPCODE == m.EMPLOYEECODE).Select(a => a.FIRSTNAME + " " + a.LASTNAME).FirstOrDefault(),
                            DEMISEDATE = m.DEMISEDATE,
                            DEMISEREASONE = m.DEMISEREASONE,
                            //STATUS = m.STATUS,
                            STATUS = (_dbContext.ASR_BENEVOLENT_DT.Where(a => a.CONSENTBYEMPLOYEE == sr.UserId && a.ASR_BENEVOLENTID == m.ASR_BENEVOLENTID).Count() == 1 ? 1 : 0),
                            TO_DT = m.TO_DT,
                            FROM_DT = m.FROM_DT,
                            Opration = _dbContext.VW_ASSOCIATELVLDETAILS.Where(a => a.SYKI == strKIID && a.ADEMPCODE == m.EMPLOYEECODE).Select(a => a.OPERATION).FirstOrDefault(),
                        }).ToList();
            }
            else
            {
                data = (from m in _dbContext.ASR_BENEVOLENT_MST
                        where
                         aa.Contains(m.ASR_BENEVOLENTID)
                        &&
                        (CurDate >= m.FROM_DT && CurDate <= m.TO_DT)
                        && (sr.DemiseEmpCode != 0 ? m.EMPLOYEECODE == sr.DemiseEmpCode : true)
                        && m.STATUS == 1
                        orderby m.DEMISEDATE
                        select new BenevolentViewModel
                        {
                            ASR_BENEVOLENTID = m.ASR_BENEVOLENTID,
                            EMPLOYEECODE = m.EMPLOYEECODE,
                            EmpName = _dbContext.ADEMPLOYEE.Where(a => a.ADEMPCODE == m.EMPLOYEECODE).Select(a => a.FIRSTNAME + " " + a.LASTNAME).FirstOrDefault(),
                            DEMISEDATE = m.DEMISEDATE,
                            DEMISEREASONE = m.DEMISEREASONE,
                            //STATUS = m.STATUS,
                            STATUS = (_dbContext.ASR_BENEVOLENT_DT.Where(a => a.CONSENTBYEMPLOYEE == sr.UserId && a.ASR_BENEVOLENTID == m.ASR_BENEVOLENTID).Count() == 1 ? 1 : 0),
                            TO_DT = m.TO_DT,
                            FROM_DT = m.FROM_DT,
                            Opration = _dbContext.VW_ASSOCIATELVLDETAILS.Where(a => a.SYKI == strKIID && a.ADEMPCODE == m.EMPLOYEECODE).Select(a => a.OPERATION).FirstOrDefault(),
                        }).ToList();
            }

            return data;
        }

        public List<BenevolentViewModel> GetContributionReport(SearchBenevolent sr)
        {

            DateTime ReqDateFrom = DateTime.Now.Date;
            DateTime ReqDateTo = DateTime.Now.Date;
            DateTime reportfrom = DateTime.ParseExact("15-JUL-2021", "dd-MMM-yyyy", null);
            if (!string.IsNullOrEmpty(sr.From_Dt))
            {
                ReqDateFrom = DateTime.ParseExact(sr.From_Dt, "dd-MMM-yyyy", null);
            }
            if (!string.IsNullOrEmpty(sr.To_Dt))
            {
                ReqDateTo = DateTime.ParseExact(sr.To_Dt, "dd-MMM-yyyy", null);
            }
            var ass = (from m in _dbContext.ASR_BENEVOLENT_MST
                       join dt in _dbContext.ASR_BENEVOLENT_DT on m.ASR_BENEVOLENTID equals dt.ASR_BENEVOLENTID
                       where (((!string.IsNullOrEmpty(sr.From_Dt) ? (ReqDateFrom >= m.FROM_DT) : true) || (!string.IsNullOrEmpty(sr.From_Dt) ? (ReqDateFrom <= m.TO_DT) : true))
                                              && ((!string.IsNullOrEmpty(sr.To_Dt) ? (ReqDateTo >= m.FROM_DT) : true) || (!string.IsNullOrEmpty(sr.To_Dt) ? (ReqDateTo <= m.TO_DT) : true)))
                       select m).ToList();


            long strKIID = (long)_dbContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
            List<BenevolentViewModel> data = (from m in _dbContext.ASR_BENEVOLENT_MST
                                              join dt in _dbContext.ASR_BENEVOLENT_DT on m.ASR_BENEVOLENTID equals dt.ASR_BENEVOLENTID
                                              where (sr.DemiseEmpCode != 0 ? m.EMPLOYEECODE == sr.DemiseEmpCode : true)
                                              && (sr.ContEmpCode != 0 ? dt.CONSENTBYEMPLOYEE == sr.ContEmpCode : true)
                                                 &&
                                                  (
                                                      ((!string.IsNullOrEmpty(sr.From_Dt)) && (!string.IsNullOrEmpty(sr.To_Dt))) ?
                                                      ((!string.IsNullOrEmpty(sr.From_Dt) ? (ReqDateFrom >= m.FROM_DT) && (ReqDateFrom <= m.TO_DT) : true) ||
                                                      (!string.IsNullOrEmpty(sr.To_Dt) ? (ReqDateTo >= m.FROM_DT) && (ReqDateTo <= m.TO_DT) : true)) :

                                                      ((string.IsNullOrEmpty(sr.From_Dt)) && (!string.IsNullOrEmpty(sr.To_Dt))) ?
                                                      ((!string.IsNullOrEmpty(sr.To_Dt) ? (ReqDateTo >= m.FROM_DT) && (ReqDateTo <= m.TO_DT) : true) &&
                                                      (!string.IsNullOrEmpty(sr.From_Dt) ? (ReqDateFrom >= m.FROM_DT) && (ReqDateFrom <= m.TO_DT) : true)
                                                      ) :

                                                      //((string.IsNullOrEmpty(sr.From_Dt)) && (!string.IsNullOrEmpty(sr.To_Dt))) ?
                                                      //((!string.IsNullOrEmpty(sr.From_Dt) ? (ReqDateFrom >= m.FROM_DT) && (ReqDateFrom <= m.TO_DT) : true) &&
                                                      //(!string.IsNullOrEmpty(sr.To_Dt) ? (ReqDateTo >= m.FROM_DT) && (ReqDateTo <= m.TO_DT) : true)) :

                                                      ((!string.IsNullOrEmpty(sr.From_Dt)) && (string.IsNullOrEmpty(sr.To_Dt))) ?
                                                      ((!string.IsNullOrEmpty(sr.From_Dt) ? (ReqDateFrom >= m.FROM_DT) && (ReqDateFrom <= m.TO_DT) : true) &&
                                                      (!string.IsNullOrEmpty(sr.To_Dt) ? (ReqDateTo >= m.FROM_DT) && (ReqDateTo <= m.TO_DT) : true))
                                                      : true
                                                  )

                                              select new BenevolentViewModel
                                              {
                                                  EMPLOYEECODE = m.EMPLOYEECODE,
                                                  EmpName = _dbContext.ADEMPLOYEE.Where(a => a.ADEMPCODE == m.EMPLOYEECODE).Select(a => a.FIRSTNAME + " " + a.LASTNAME).FirstOrDefault(),
                                                  DEMISEDATE = m.DEMISEDATE,
                                                  CONSENTBYEMPLOYEE = dt.CONSENTBYEMPLOYEE,
                                                  CONSENTBYEmpName = _dbContext.ADEMPLOYEE.Where(a => a.ADEMPCODE == dt.CONSENTBYEMPLOYEE).Select(a => a.FIRSTNAME + " " + a.LASTNAME).FirstOrDefault(),
                                                  Opration = _dbContext.VW_ASSOCIATELVLDETAILS.Where(a => a.SYKI == strKIID && a.ADEMPCODE == dt.CONSENTBYEMPLOYEE).Select(a => a.OPERATION).FirstOrDefault(),
                                                  ContributionPeriod = m.FROM_DT.ToString() + " To " + m.TO_DT.ToString(),
                                                  AMOUNT = dt.AMOUNT,
                                                  ASR_BENEVOLENT_DTID = dt.ASR_BENEVOLENT_DTID
                                              }).ToList();
            return data;
        }


        public BenevolentViewModel GetContributionReportDetail(long id)
        {
            var dt = _dbContext.ASR_BENEVOLENT_DT.Find(id);
            var EmpID = _dbContext.ASR_BENEVOLENT_MST.Where(a => a.ASR_BENEVOLENTID == dt.ASR_BENEVOLENTID).Select(a => a.EMPLOYEECODE).FirstOrDefault();
            var DimEmp = GetEmpDetail(EmpID);

            long strKIID = (long)_dbContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
            BenevolentViewModel data = (from t in _dbContext.ASR_BENEVOLENT_DT
                                        join m in _dbContext.ASR_BENEVOLENT_MST on t.ASR_BENEVOLENTID equals m.ASR_BENEVOLENTID
                                        where t.ASR_BENEVOLENT_DTID == id
                                        select new BenevolentViewModel
                                        {
                                            ASR_BENEVOLENTID = m.ASR_BENEVOLENTID,
                                            EMPLOYEECODE = m.EMPLOYEECODE,
                                            EmpName = DimEmp.EmpName,
                                            Opration = DimEmp.Opration,
                                            Division = DimEmp.Division,
                                            Department = DimEmp.Department,
                                            Section = DimEmp.Section,
                                            DEMISEDATE = m.DEMISEDATE,
                                            DEMISEREASONE = m.DEMISEREASONE,
                                            ContributionPeriod = m.FROM_DT.ToString() + " to " + m.TO_DT.ToString(),
                                            CONSENTBYEMPLOYEE = t.CONSENTBYEMPLOYEE,
                                            CONSENTBYEmpName = _dbContext.ADEMPLOYEE.Where(a => a.ADEMPCODE == dt.CONSENTBYEMPLOYEE).Select(a => a.FIRSTNAME + " " + a.LASTNAME).FirstOrDefault(),
                                            CONSENTBYEmpOpration = _dbContext.VW_ASSOCIATELVLDETAILS.Where(a => a.SYKI == strKIID && a.ADEMPCODE == dt.CONSENTBYEMPLOYEE).Select(a => a.OPERATION).FirstOrDefault(),
                                            AMOUNT = dt.AMOUNT

                                        }).FirstOrDefault();

            return data;
        }

        public BenevolentViewModel GetDemiseEmployeeDetail(long id)
        {
            //var DimEmp = GetEmpDetail(id);

            long strKIID = (long)_dbContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
            BenevolentViewModel data = (from e in _dbContext.ADEMPLOYEE
                                            //from m in _dbContext.ASR_BENEVOLENT_MST
                                            //join e in _dbContext.ADEMPLOYEE on m.EMPLOYEECODE equals e.ADEMPCODE
                                        join vw in _dbContext.VW_ASSOCIATELVLDETAILS.Where(a => a.SYKI == strKIID) on e.ADEMPCODE equals vw.ADEMPCODE into leftvw from vw in leftvw.DefaultIfEmpty()
                                        //join m in _dbContext.ASR_BENEVOLENT_MST on t.ASR_BENEVOLENTID equals m.ASR_BENEVOLENTID
                                        where e.ADEMPCODE == id
                                        select new BenevolentViewModel
                                        {
                                            //ASR_BENEVOLENTID = m.ASR_BENEVOLENTID,
                                            EMPLOYEECODE = e.ADEMPCODE,
                                            EmpName = e.FIRSTNAME + " " + e.LASTNAME,
                                            Opration = vw.OPERATION,
                                            Division = vw.DIVISION,
                                            Department = vw.DEPARTMENT,
                                            Section = vw.SECTION,
                                            //DEMISEDATE = m.DEMISEDATE,
                                            //DEMISEREASONE = m.DEMISEREASONE,
                                            //ContributionPeriod = m.FROM_DT.ToString() + " to " + m.TO_DT.ToString()
                                        }).FirstOrDefault();
            return data;
        }
    }
}
