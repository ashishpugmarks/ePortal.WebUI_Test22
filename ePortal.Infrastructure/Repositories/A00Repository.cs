using ePortal.DomainClasses;
using ePortal.ViewModels;
using System.Globalization;
using ePortal.Infrastructure.DbContexts;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ePortal.Infrastructure.Repositories
{
    public class A00Repository
    {
        private readonly EPortalDBContext _pdDBContext;
        private readonly SYKI _CurrentActiveSyki;//Added by Eshant on 27-06-22 to get current active syki
        public A00Repository(EPortalDBContext dbContext)
        {
            _pdDBContext = dbContext;
            _CurrentActiveSyki = _pdDBContext.SYKI.Where(x => x.ACTIVE == 1).FirstOrDefault();//Added by Eshant on 27-06-22 to get current active syki
        }
        //Added below method GetCurrentActiveKi by Eshant on 27-06-22 to get current active syki
        public Int64 GetCurrentActiveKi()
        {
            return Convert.ToInt64(_CurrentActiveSyki.SYKIID);
        }
        //End
        //public A00DtlViewModel GetA00Detail(A00SearchModel objSearchModel)
        public A00DtlViewModel GetA00Dtl(A00SearchModel objSearchModel)
        {
            var result = _pdDBContext.A00DTLTB.Where(x => x.A00DTLTBID == objSearchModel.a00dtltbid).Select(x => x).FirstOrDefault();
            A00DtlViewModel dvm = new A00DtlViewModel();
            if (result != null)
            {

                dvm.A00DTLTBID = result.A00DTLTBID;
                dvm.SYKIID = result.SYKIID;
                dvm.STATUSCD = result.STATUSCD;
                dvm.PRJCTTLE = result.PRJCTTLE;
                dvm.PRJCTTXT = result.PRJCTTXT;
                dvm.BUKPITXT = result.BUKPITXT;
                dvm.PURPSTXT = result.PURPSTXT;
                dvm.TRGTINDCD = result.TRGTINDCD;
                dvm.RQUMTTXT = result.RQUMTTXT;
                dvm.ATTACHMENT = result.ATTACHMENT;
                dvm.BUDGETFLG = result.BUDGETFLG;
                dvm.STARTDT = result.STARTDT;
                dvm.ENDDT = result.ENDDT;
                dvm.FRCSTOTHER = result.FRCSTOTHER;
                dvm.ADDEDBY = result.ADDEDBY;
                dvm.OPERATION = result.OPERATION;
                dvm.DIVISION = result.DIVISION;
                dvm.DEPARTMENT = result.DEPARTMENT;
                dvm.SECTION = result.SECTION;
                dvm.DATEADDEDID = result.DATEADDEDID;
                dvm.BUDGETSYKIID = result.BUDGETSYKIID;
                dvm.ADORGLEVELID = result.ADORGLEVELID;
            }
            return dvm;
        }

        public A00DataViewModel GetA00DtlFullTable()
        {
            var result = _pdDBContext.A00DTLTB.ToList();
            A00DataViewModel dvm = new A00DataViewModel();
            if (result.Count > 0)
            {
                foreach (var lvlList in result)
                {
                    dvm._A00DtlViewModelList.Add
                        (new A00DtlViewModelList
                        {
                            A00DTLTBID = lvlList.A00DTLTBID,
                            SYKIID = lvlList.SYKIID,
                            STATUSCD = lvlList.STATUSCD,
                            PRJCTTLE = lvlList.PRJCTTLE,
                            PRJCTTXT = lvlList.PRJCTTXT,
                            BUKPITXT = lvlList.BUKPITXT,
                            PURPSTXT = lvlList.PURPSTXT,
                            TRGTINDCD = lvlList.TRGTINDCD,
                            RQUMTTXT = lvlList.RQUMTTXT,
                            ATTACHMENT = lvlList.ATTACHMENT,
                            BUDGETFLG = lvlList.BUDGETFLG,
                            STARTDT = lvlList.STARTDT,
                            ENDDT = lvlList.ENDDT,
                            FRCSTOTHER = lvlList.FRCSTOTHER,
                            ADDEDBY = lvlList.ADDEDBY,
                            OPERATION = lvlList.OPERATION,
                            DIVISION = lvlList.DIVISION,
                            DEPARTMENT = lvlList.DEPARTMENT,
                            SECTION = lvlList.SECTION,
                            DATEADDEDID = lvlList.DATEADDEDID,
                            BUDGETSYKIID = lvlList.BUDGETSYKIID,
                            ADORGLEVELID = lvlList.ADORGLEVELID
                        });
                }
            }
            return dvm;
        }

        public A00SYKIViewModel GetA00SYKIList()
        {
            A00SYKIViewModel a00SYKIViewModel = new A00SYKIViewModel();

            var SYKIList = (from data in _pdDBContext.SYKI
                            select data).ToList();
            if (SYKIList.Count > 0)
            {
                foreach (var kiList in SYKIList)
                {
                    a00SYKIViewModel._SYKIList.Add(new A00SYKIList { KICODE = kiList.KICODE, SYKIID = kiList.SYKIID, ACTIVE = kiList.ACTIVE });
                }
            }
            return a00SYKIViewModel;
        }
        //Changes By Ankit-15-01-2021, 
        /// <summary>
        /// Get the current ki and the future ki
        /// </summary>
        /// <returns></returns>
        public A00SYKIViewModel GetLatestA00SYKIList()
        {
            A00SYKIViewModel a00SYKIViewModel = new A00SYKIViewModel();

            var SYKIList = (from data in _pdDBContext.A00SYKI
                            select data).ToList();
            if (SYKIList.Count > 0)
            {
                foreach (var kiList in SYKIList)
                {
                    a00SYKIViewModel._SYKIList.Add(new A00SYKIList { KICODE = kiList.KICODE, SYKIID = kiList.SYKIID, ACTIVE = kiList.ACTIVE });
                }
            }
            return a00SYKIViewModel;
        }



        //public SearchParameterList GetOPERATIONlist()
        //{


        //    SearchParameterList dvm = new SearchParameterList();
        //    var AdOrgLevelList = (from data1 in _pdDBContext.VW_OPERATION.Where(x => x.ACTIVE == 1).OrderByDescending(x => x.LEVELDESCRIP)

        //                          select data1).ToList();

        //    if (AdOrgLevelList.Count > 0)
        //    {
        //        foreach (var lvlList in AdOrgLevelList)
        //        {
        //            dvm.SearchParameterList.Add(new SearchParameterList { LEVELDESCRIP = lvlList.LEVELDESCRIP, ADORGLEVELID = lvlList.ADORGLEVELID });
        //        }
        //    }
        //    return dvm;

        //}

        // GetDIVISIONlist


        //public A00DIVISIONlist GetDIVISIONlist()
        //{


        //    // var aa = _pdDBContext.VW_ASSOCIATELVLDETAILSDEPT.ToList();
        //    A00DIVISIONlist dvm = new A00DIVISIONlist();
        //   // _pdDBContext.MEDINS_POLICYTYPE_MST
        //    var AdOrgLevelList = (from data1 in _pdDBContext.VW_DIVISION
        //    .Where(x => x.ACTIVE == 1).OrderByDescending(x => x.LEVELDESCRIP)
        //                          select data1).ToList();

        //    if (AdOrgLevelList.Count > 0)
        //    {
        //        foreach (var lvlList in AdOrgLevelList)
        //        {
        //            dvm._DIVISIONlist.Add(new DIVISIONlist { LEVELDESCRIP = lvlList.LEVELDESCRIP, ADORGLEVELID = lvlList.ADORGLEVELID });
        //        }
        //    }
        //    return dvm;

        //}

        public List<SearchParameterList> BindDivision(long? op_Id, long? SYKIID)
        {
            var iList = _pdDBContext.VW_ASSOCIATELVLDETAILS.Where(e => e.ACTIVE == 1 && e.SYKI == SYKIID && e.DIVISIONID != null && e.DIVISIONID != 0
                         && e.OPERATIONID == op_Id).Select(x => new SearchParameterList
                         {
                             DIVISION = x.DIVISION,
                             DIVISIONID = x.DIVISIONID
                         }).Distinct().OrderBy(x => x.DIVISION).ToList();
            return iList;
        }

        public List<SearchParameterList> BindDepartment(long? div_Id, long? op_Id, long SYKIID)
        {
            List<SearchParameterList> iList = new List<SearchParameterList>();
            var iColl = (from data in _pdDBContext.VW_ASSOCIATELVLDETAILS.Where
                         (e => e.ACTIVE == 1
                         && e.SYKI == SYKIID
                         && e.DEPARTMENTID != null && e.DEPARTMENTID != 0
                         && e.DIVISIONID == div_Id
                       )
                         select new
                         {
                             data.DEPARTMENTID,
                             data.DEPARTMENT
                         }).Distinct().OrderBy(e => e.DEPARTMENT).ToList();
            foreach (var obj in iColl)
            {
                iList.Add(new SearchParameterList
                {
                    DEPARTMENTID = Convert.ToInt64(obj.DEPARTMENTID == null ? 0 : obj.DEPARTMENTID),
                    DEPARTMENT = obj.DEPARTMENT
                });
            }
            return iList;
        }

        public List<SearchParameterList> BindSecion(long? dep_Id, long? div_Id, long? op_Id, long SYKIID)
        {

            List<SearchParameterList> iList = new List<SearchParameterList>();
            var iColl = (from data in _pdDBContext.VW_ASSOCIATELVLDETAILS.Where(e => e.ACTIVE == 1
                         && e.SYKI == SYKIID
                         && e.SECTION != null && e.SECTIONID != 0
                         && e.DEPARTMENTID == dep_Id

                         )
                         select new
                         {
                             data.SECTIONID,
                             data.SECTION
                         }).Distinct().OrderBy(e => e.SECTION).ToList();
            foreach (var obj in iColl)
            {
                iList.Add(new SearchParameterList
                {
                    SECTIONID = Convert.ToInt64(obj.SECTIONID == null ? 0 : obj.SECTIONID),
                    SECTION = obj.SECTION
                }
                );
            }
            return iList;
        }






        public A00DataViewModel GetA00ADORGLEVELList(decimal? SYKIID)
        {
            A00DataViewModel dvm = new A00DataViewModel();

            var AdOrgLevelList = (from data1 in _pdDBContext.ADORGLEVEL.Where(x => x.ADORGLEVELTYPEID == 1 && x.ACTIVE != 0 && x.SYKIID == SYKIID)
                                  select data1).OrderBy(x => x.LEVELDESCRIP)
                                  .Select(lvlList =>
                                  new A00ADORGLEVELList { LEVELDESCRIP = lvlList.LEVELDESCRIP, ADORGLEVELID = lvlList.ADORGLEVELID })
                                  .ToList();
            dvm._ADOrgLevelList = AdOrgLevelList;
            //if (AdOrgLevelList.Count > 0)
            //{
            //    foreach (var lvlList in AdOrgLevelList)
            //    {
            //        dvm._ADOrgLevelList.Add(new A00ADORGLEVELList { LEVELDESCRIP = lvlList.LEVELDESCRIP, ADORGLEVELID = lvlList.ADORGLEVELID });
            //    }
            //}
            return dvm;
        }

        public A00DataViewModel GetA00_IT_ADORGLEVELList(A00SearchModel objSearchModel)
        {           //added by eshant for getting PARENTLEVELID, earlier it was hardcoded for 97 ki
            long adlvlId = _pdDBContext.ADORGLEVEL.Where(a => a.LEVELDESCRIP == "Strategic Information System" && a.SYKIID == _CurrentActiveSyki.SYKIID).Select(s => s.ADORGLEVELID).FirstOrDefault();
            //end
            //   select adorglevelid from adorglevel where leveldescrip = 'Strategic Information System' and sykiid = (select s.sykiid from syki s where active = 1)

            A00DataViewModel dvm = new A00DataViewModel();
            var AdOrgLevelList = _pdDBContext.ADORGLEVEL.Where(a => a.PARENTLEVELID == adlvlId &&
            a.ACTIVE == 1 && a.SYKIID == _CurrentActiveSyki.SYKIID && a.ADORGLEVELTYPEID == 2).ToList();
            //Commented By Eshant to set Current Active ki -- 12-Jul-2022
            // a.ACTIVE == 1 && a.SYKIID == objSearchModel.SYKIID && a.ADORGLEVELTYPEID == 2).ToList();
            if (AdOrgLevelList.Count > 0)
            {
                foreach (var lvlList in AdOrgLevelList)
                {
                    dvm._ADOrgLevelList.Add(new A00ADORGLEVELList
                    {
                        LEVELDESCRIP = lvlList.LEVELDESCRIP,
                        ADORGLEVELID = lvlList.ADORGLEVELID,
                        PARENTLEVELID = lvlList.PARENTLEVELID,
                        ACTIVE = lvlList.ACTIVE,
                        SYKIID = lvlList.SYKIID,
                        ADORGLEVELTYPEID = lvlList.ADORGLEVELTYPEID
                    });
                }
            }
            return dvm;
        }
        //kiran
        public A00DataViewModel GetA00_INVFORCASTList()
        {
            A00DataViewModel dvm = new A00DataViewModel();
            //INCFORCASTDETAIL
            var invforcasr = _pdDBContext.INVFORCAST.AsEnumerable().OrderByDescending(x => x.INCFORCASTDETAIL)
                .Select(lvlList => new A00_INVFORCASTList
                {
                    INCFORCASTDETAIL = lvlList.INCFORCASTDETAIL,
                    INVFORCASTID = lvlList.INVFORCASTID
                }).ToList();
            dvm._InvforcastList = invforcasr;

            // dvm.A00DTLTBID = _pdDBContext.A00DTLTB.AsEnumerable().OrderByDescending(x => x.A00DTLTBID).Select(x => x.A00DTLTBID).FirstOrDefault();

            //if (invforcasr.Count > 0)
            //{
            //    foreach (var lvlList in invforcasr)
            //    {
            //        dvm._InvforcastList.Add(new A00_INVFORCASTList { INCFORCASTDETAIL = lvlList.INCFORCASTDETAIL, INVFORCASTID = lvlList.INVFORCASTID });
            //    }
            //}
            return dvm;
        }

        //public A00PartialClass GetA00PartialClass()
        //{
        //    A00PartialClass dtlTb = new A00PartialClass();
        //    dtlTb = _pdDBContext.A00DTLTB.AsEnumerable().FirstOrDefault();
        //    return dtlTb;
        //}

        public A00MaxA00DTLTBID GetA00MaxA00DTLTBID()
        {
            A00MaxA00DTLTBID dvm = new A00MaxA00DTLTBID();
            dvm.A00DTLTBID = _pdDBContext.A00DTLTB.AsEnumerable().OrderByDescending(x => x.A00DTLTBID).Select(x => x.A00DTLTBID).FirstOrDefault();
            return dvm;
        }

        public A00MaxA00APPROVAL GetA00MaxA00APPROVAL()
        {
            A00MaxA00APPROVAL map = new A00MaxA00APPROVAL();
            map.A00APPROVALID = _pdDBContext.A00APPROVAL.AsEnumerable().OrderByDescending(x => x.A00APPROVALID).Select(x => x.A00APPROVALID).FirstOrDefault();
            return map;
        }

        public A00DataViewModel GetA00INVFORCASTList()
        {
            A00DataViewModel dvm = new A00DataViewModel();
            //var invforcasr = _pdDBContext.A00INVFORCAST.ToList();
            //if (invforcasr.Count > 0)
            //{
            //    foreach (var lvlList in invforcasr)
            //    {
            //        dvm._A00InvforcastList.Add(new A00INVFORCASTList
            //        {
            //            A00INVFORCASTID = lvlList.A00INVFORCASTID,
            //            A00DTLTBID = lvlList.A00DTLTBID,
            //            INVFORCASTID = lvlList.INVFORCASTID,
            //            ACTIVE = lvlList.ACTIVE
            //        });
            //    }
            //}
            var invforcasr = _pdDBContext.A00INVFORCAST.Select(lvlList =>
            new A00INVFORCASTList
            {
                A00INVFORCASTID = lvlList.A00INVFORCASTID,
                A00DTLTBID = lvlList.A00DTLTBID,
                INVFORCASTID = lvlList.INVFORCASTID,
                ACTIVE = lvlList.ACTIVE
            }
            ).ToList();
            dvm._A00InvforcastList = invforcasr;


            return dvm;
        }

        public A00_APPROVAL GetA00APPROVAL(A00SearchModel objSearchModel)
        {
            var result = _pdDBContext.A00APPROVAL.Where(x => x.A00DTLTBID == objSearchModel.a00dtltbid).FirstOrDefault();
            A00_APPROVAL a00Approval = new A00_APPROVAL();
            if (result != null)
            {
                a00Approval.A00APPROVALID = result.A00APPROVALID;
                a00Approval.A00DTLTBID = result.A00DTLTBID;
                a00Approval.DEPTHDID = result.DEPTHDID;
                a00Approval.DEPTHDAPPDATE = result.DEPTHDAPPDATE;
                a00Approval.DEPTHDAPPTXT = result.DEPTHDAPPTXT;
                a00Approval.COORDDID = result.COORDDID;
                a00Approval.COORDAPPDATE = result.COORDAPPDATE;
                a00Approval.COORDAPPTXT = result.COORDAPPTXT;
                a00Approval.DIVHDHDID = result.DIVHDHDID;
                a00Approval.DIVHDAPPDATE = result.DIVHDAPPDATE;
                a00Approval.DIVHDAPPTXT = result.DIVHDAPPTXT;
                a00Approval.EXECOHDID = result.EXECOHDID;
                a00Approval.EXECOAPPDATE = result.EXECOAPPDATE;
                a00Approval.EXECOAPPTXT = result.EXECOAPPTXT;
                a00Approval.OHID = result.OHID;
                a00Approval.OHAPPDATE = result.OHAPPDATE;
                a00Approval.OHAPPTXT = result.OHAPPTXT;

                //09-Sept-2021 change start
                a00Approval.PPCHOOHID = result.PPCHOOHID;
                a00Approval.PPCHOOHDATE = result.PPCHOOHDATE;
                a00Approval.PPCHOOHTXT = result.PPCHOOHTXT;
                //09-Sept-2021 change end
            }
            return a00Approval;
        }

        public A00DataViewModel GetA00APPROVALFullTable()
        {
            A00DataViewModel al = new A00DataViewModel();
            //var result = _pdDBContext.A00APPROVAL.ToList();
            //if (result.Count > 0)
            //{
            //    foreach (var lst in result)
            //    {
            //        al._A00APPROVALList.Add(new A00APPROVALList
            //        {
            //            A00APPROVALID = lst.A00APPROVALID,
            //            A00DTLTBID = lst.A00DTLTBID,
            //            DEPTHDID = lst.DEPTHDID,
            //            DEPTHDAPPDATE = lst.DEPTHDAPPDATE,
            //            DEPTHDAPPTXT = lst.DEPTHDAPPTXT,
            //            COORDDID = lst.COORDDID,
            //            COORDAPPDATE = lst.COORDAPPDATE,
            //            COORDAPPTXT = lst.COORDAPPTXT,
            //            DIVHDHDID = lst.DIVHDHDID,
            //            DIVHDAPPDATE = lst.DIVHDAPPDATE,
            //            DIVHDAPPTXT = lst.DIVHDAPPTXT,
            //            EXECOHDID = lst.EXECOHDID,
            //            EXECOAPPDATE = lst.EXECOAPPDATE,
            //            EXECOAPPTXT = lst.EXECOAPPTXT,
            //            OHID = lst.OHID,
            //            OHAPPDATE = lst.OHAPPDATE,
            //            OHAPPTXT = lst.OHAPPTXT,

            //            //09-Sept-2021 change start
            //            PPCHOOHID = lst.PPCHOOHID,
            //            PPCHOOHDATE = lst.PPCHOOHDATE,
            //            PPCHOOHTXT = lst.PPCHOOHTXT
            //            //09-Sept-2021 change end
            //        });
            //    }
            //}

            var result = _pdDBContext.A00APPROVAL
                .Select(lst => new A00APPROVALList
                {
                    A00APPROVALID = lst.A00APPROVALID,
                    A00DTLTBID = lst.A00DTLTBID,
                    DEPTHDID = lst.DEPTHDID,
                    DEPTHDAPPDATE = lst.DEPTHDAPPDATE,
                    DEPTHDAPPTXT = lst.DEPTHDAPPTXT,
                    COORDDID = lst.COORDDID,
                    COORDAPPDATE = lst.COORDAPPDATE,
                    COORDAPPTXT = lst.COORDAPPTXT,
                    DIVHDHDID = lst.DIVHDHDID,
                    DIVHDAPPDATE = lst.DIVHDAPPDATE,
                    DIVHDAPPTXT = lst.DIVHDAPPTXT,
                    EXECOHDID = lst.EXECOHDID,
                    EXECOAPPDATE = lst.EXECOAPPDATE,
                    EXECOAPPTXT = lst.EXECOAPPTXT,
                    OHID = lst.OHID,
                    OHAPPDATE = lst.OHAPPDATE,
                    OHAPPTXT = lst.OHAPPTXT,

                    //09-Sept-2021 change start
                    PPCHOOHID = lst.PPCHOOHID,
                    PPCHOOHDATE = lst.PPCHOOHDATE,
                    PPCHOOHTXT = lst.PPCHOOHTXT
                    //09-Sept-2021 change end
                }).ToList();
            al._A00APPROVALList = result;
            return al;
        }

        //07-Sept-2021 change start
        public A00_ADORGCOORDINATOR getDataFromAdorgcoordinator(long? ADORGLEVELID)
        {
            A00_ADORGCOORDINATOR cord = new A00_ADORGCOORDINATOR();
            var varCord = _pdDBContext.ADORGCOORDINATOR.Where(x => x.ADORGLEVELID == ADORGLEVELID && x.ISACTIVE == 1).FirstOrDefault();
            if (varCord != null)
            {
                cord.COORDINATOR = varCord.COORDINATOR;
                cord.EXECOORDINATOR = varCord.EXECOORDINATOR;
                cord.OPHEAD = varCord.OPHEAD;
            }
            return cord;
        }

        //public A00DataViewModel GetA00_ADORGCOORDINATORLIST(long? ADEMPCODE, string userType)
        public List<A00_ADORGCOORDINATORDataList> GetA00_ADORGCOORDINATORLIST(long? ADEMPCODE, string userType)
        {
            List<A00_ADORGCOORDINATORDataList> adl = new List<A00_ADORGCOORDINATORDataList>();
            try
            {
                //09-Sept-2021 change start
                //adl = _pdDBContext.ADORGCOORDINATOR.Where(x => x.ISACTIVE == 1 && (userType == "EXCO" ? x.EXECOORDINATOR == ADEMPCODE :
                //(userType == "CO" ? x.COORDINATOR == ADEMPCODE : x.COORDINATOR == 0))).Select(a => new A00_ADORGCOORDINATORDataList { ADORGLEVELID = a.ADORGLEVELID }).ToList();

                adl = _pdDBContext.ADORGCOORDINATOR.Where(x => x.ISACTIVE == 1 && (userType == "EXCO" ? x.EXECOORDINATOR == ADEMPCODE :
                (userType == "CO" ? x.COORDINATOR == ADEMPCODE : (userType == "OH" ? x.OPHEAD == ADEMPCODE : x.OPHEAD == 0)))).Select(a => new A00_ADORGCOORDINATORDataList { ADORGLEVELID = a.ADORGLEVELID }).ToList();
                //09-Sept-2021 change end
            }
            catch (Exception ex)
            {

            }
            return adl;
        }
        //07-Sept-2021 change end

        public A00DataViewModel GetA00ADEMPLOYEE()
        {
            A00DataViewModel dvm = new A00DataViewModel();
            //var empData = _pdDBContext.ADEMPLOYEE.ToList();
            //if (empData.Count > 0)
            //{
            //    foreach (var lvlList in empData)
            //    {
            //        dvm._A00ADEMPLOYEE.Add(new A00ADEMPLOYEE
            //        {
            //            ADEMPCODE = lvlList.ADEMPCODE,
            //            FIRSTNAME = lvlList.FIRSTNAME,
            //            LASTNAME = lvlList.LASTNAME
            //        });
            //    }
            //}

            var empData = _pdDBContext.ADEMPLOYEE
                .Select(lvlList => new A00ADEMPLOYEE
                {
                    ADEMPCODE = lvlList.ADEMPCODE,
                    FIRSTNAME = lvlList.FIRSTNAME,
                    LASTNAME = lvlList.LASTNAME
                }).ToList();
            dvm._A00ADEMPLOYEE = empData;
            return dvm;
        }

        public A00_VW_ASSOCIATELVLDETAILS GetA00_VW_ASSOCIATELVLDETAILS(A00SearchModel objSearchModel)
        {
            //Think to Removed SYKI -- By Eshant 11-07-22 by not done need to check more

            //var result = _pdDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == objSearchModel.ADEMPCODE && x.SYKI == objSearchModel.SYKI).Select(x => new A00_VW_ASSOCIATELVLDETAILS
            var result = _pdDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == objSearchModel.ADEMPCODE).Select(x => new A00_VW_ASSOCIATELVLDETAILS
            {
                SYKI = x.SYKI,//Added By Eshant on 18-08-22 for order by clause added below 
                DEPARTMENTID = x.DEPARTMENTID,
                OPERATIONID = x.OPERATIONID,
                DIVISIONID = x.DIVISIONID,
                SECTIONID = x.SECTIONID,
                OPERATION = x.OPERATION,
                DIVISION = x.DIVISION,
                DEPARTMENT = x.DEPARTMENT,
                SECTION = x.SECTION,
                ADFUNCTIONALDESIGNATIONID = x.ADFUNCTIONALDESIGNATIONID,
                SUPSUPERVISOREMPCODE = x.SUPSUPERVISOREMPCODE,//CR7306
            }).OrderByDescending(z => z.SYKI).FirstOrDefault();//Added order By clause by Eshant on 18-08-22
            return result;


        }


        public List<ADORGLEVEL> GetOrgLevelList(long typeId, long? SYKIID)
        {
            var iList = from data in _pdDBContext.ADORGLEVEL
                        where data.ACTIVE == 1 && data.SYKIID == SYKIID
                        && data.ADORGLEVELTYPEID == typeId
                        orderby data.LEVELDESCRIP descending
                        select data;
            return iList.ToList();



        }

        /// <summary>
        ////// kkk
        /// </summary>
        /// <param name="objSearchModel"></param>
        /// <returns></returns>
        public A00_VW_ASSOCIATELVLDETAILS1 GetA00_VW_ASSOCIATELVLDETAILS1(A00SearchModel objSearchModel)
        {
            //var result = _pdDBContext.VW_EMPLOYEEHEADDETAILS
            //    //.Where(x => x.ADEMPCODE == objSearchModel.ADEMPCODE && x.SYKI == objSearchModel.SYKI).Select(x => new
            var result = _pdDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == objSearchModel.ADEMPCODE && x.SYKI == objSearchModel.SYKI)
                .Select(x => new
                {
                    x.DEPARTMENTID,
                    x.OPERATIONID,
                    x.DIVISIONID,
                    x.SECTIONID,
                    x.OPERATION,
                    x.DIVISION,
                    x.DEPARTMENT,
                    x.SECTION,
                    x.ADFUNCTIONALDESIGNATIONID,
                }).FirstOrDefault();

            A00_VW_ASSOCIATELVLDETAILS1 va = new A00_VW_ASSOCIATELVLDETAILS1();
            va.DEPARTMENTID = result.DEPARTMENTID;
            va.OPERATIONID = result.OPERATIONID;
            va.DIVISIONID = result.DIVISIONID;
            va.SECTIONID = result.SECTIONID;
            va.OPERATION = result.OPERATION;
            va.DIVISION = result.DIVISION;
            va.DEPARTMENT = result.DEPARTMENT;
            va.SECTION = result.SECTION;

            va.ADFUNCTIONALDESIGNATIONID = result.ADFUNCTIONALDESIGNATIONID;
            return va;


        }




        public A00DataViewModel GetA00_VW_ASSOCIATELVLDETAILS_ITApproval(A00SearchModel objSearchModel)
        {
            int[] ids = { 3, 4 };
            A00DataViewModel al = new A00DataViewModel();
            var result = _pdDBContext.VW_ASSOCIATELVLDETAILS
              .Where(x => x.SYKI == objSearchModel.SYKI)
              .Select(lst => new A00_VW_ASSOCIATELVLDETAILS_FULL
              {
                  DEPARTMENTID = lst.DEPARTMENTID,
                  OPERATIONID = lst.OPERATIONID,
                  DIVISIONID = lst.DIVISIONID,
                  SECTIONID = lst.SECTIONID,
                  OPERATION = lst.OPERATION,
                  DIVISION = lst.DIVISION,
                  DEPARTMENT = lst.DEPARTMENT,
                  SECTION = lst.SECTION,
                  ADFUNCTIONALDESIGNATIONID = lst.ADFUNCTIONALDESIGNATIONID
              }).ToList();
            al._A00_VW_ASSOCIATELVLDETAILS_FULL = result;
            //if (result.Count > 0)
            //{
            //    foreach (var lst in result)
            //    {
            //        al._A00_VW_ASSOCIATELVLDETAILS_FULL.Add(new A00_VW_ASSOCIATELVLDETAILS_FULL
            //        {
            //            DEPARTMENTID = lst.DEPARTMENTID,
            //            OPERATIONID = lst.OPERATIONID,
            //            DIVISIONID = lst.DIVISIONID,
            //            SECTIONID = lst.SECTIONID,
            //            OPERATION = lst.OPERATION,
            //            DIVISION = lst.DIVISION,
            //            DEPARTMENT = lst.DEPARTMENT,
            //            SECTION = lst.SECTION,
            //            ADFUNCTIONALDESIGNATIONID = lst.ADFUNCTIONALDESIGNATIONID
            //        });
            //    }
            //}
            return al;
        }

        public A00DataViewModel GetA00_VW_ASSOCIATELVLDETAILS_FullList()
        {
            A00DataViewModel vw = new A00DataViewModel();
            var result = _pdDBContext.VW_ASSOCIATELVLDETAILS
                .Select(lv => new A00_VW_ASSOCIATELVLDETAILS_FULL
                {
                    SECTIONID = lv.SECTIONID,
                    DEPARTMENTID = lv.DEPARTMENTID,
                    DIVISIONID = lv.DIVISIONID,
                    OPERATIONID = lv.OPERATIONID,
                    ADEMPCODE = lv.ADEMPCODE,
                    SYKI = lv.SYKI,
                    SUPERVISOREMPCODE = lv.SUPERVISOREMPCODE,
                    SUPSUPERVISOREMPCODE = lv.SUPSUPERVISOREMPCODE,
                    ADDESIGNATIONID = lv.ADDESIGNATIONID,
                    FUNCTIONALDESIGNATION = lv.FUNCTIONALDESIGNATION,
                    ADFUNCTIONALDESIGNATIONID = lv.ADFUNCTIONALDESIGNATIONID,
                    SYSITEID = lv.SYSITEID,
                    SYPLANTID = lv.SYPLANTID,
                    ZONE = lv.ZONE,
                    SYLOCATIONID = lv.SYLOCATIONID,
                    ACTIVE = lv.ACTIVE,
                    SECTION = lv.SECTION,
                    SEC_COSTCENTRE = lv.SEC_COSTCENTRE,
                    DEPARTMENT = lv.DEPARTMENT,
                    DEPT_COSTCENTRE = lv.DEPT_COSTCENTRE,
                    DIVISION = lv.DIVISION,
                    DIV_COSTCENTRE = lv.DIV_COSTCENTRE,
                    OPERATION = lv.OPERATION,
                    OP_COSTCENTRE = lv.OP_COSTCENTRE,
                    OPERATIONID_1 = lv.OPERATIONID_1,
                    OPERATION_1 = lv.OPERATION_1,
                    OP1_COSTCENTRE = lv.OP1_COSTCENTRE

                }).ToList();

            vw._A00_VW_ASSOCIATELVLDETAILS_FULL = result;
            //if (result.Count > 0)
            //{
            //    foreach (var lv in result)
            //    {
            //        vw._A00_VW_ASSOCIATELVLDETAILS_FULL.Add(new A00_VW_ASSOCIATELVLDETAILS_FULL
            //        {
            //            SECTIONID = lv.SECTIONID,
            //            DEPARTMENTID = lv.DEPARTMENTID,
            //            DIVISIONID = lv.DIVISIONID,
            //            OPERATIONID = lv.OPERATIONID,
            //            ADEMPCODE = lv.ADEMPCODE,
            //            SYKI = lv.SYKI,
            //            SUPERVISOREMPCODE = lv.SUPERVISOREMPCODE,
            //            SUPSUPERVISOREMPCODE = lv.SUPSUPERVISOREMPCODE,
            //            ADDESIGNATIONID = lv.ADDESIGNATIONID,
            //            FUNCTIONALDESIGNATION = lv.FUNCTIONALDESIGNATION,
            //            ADFUNCTIONALDESIGNATIONID = lv.ADFUNCTIONALDESIGNATIONID,
            //            SYSITEID = lv.SYSITEID,
            //            SYPLANTID = lv.SYPLANTID,
            //            ZONE = lv.ZONE,
            //            SYLOCATIONID = lv.SYLOCATIONID,
            //            ACTIVE = lv.ACTIVE,
            //            SECTION = lv.SECTION,
            //            SEC_COSTCENTRE = lv.SEC_COSTCENTRE,
            //            DEPARTMENT = lv.DEPARTMENT,
            //            DEPT_COSTCENTRE = lv.DEPT_COSTCENTRE,
            //            DIVISION = lv.DIVISION,
            //            DIV_COSTCENTRE = lv.DIV_COSTCENTRE,
            //            OPERATION = lv.OPERATION,
            //            OP_COSTCENTRE = lv.OP_COSTCENTRE,
            //            OPERATIONID_1 = lv.OPERATIONID_1,
            //            OPERATION_1 = lv.OPERATION_1,
            //            OP1_COSTCENTRE = lv.OP1_COSTCENTRE

            //        });
            //    }
            //}
            return vw;
        }




        public string Getoperationdetails(long empcode, int Syki)
        {
            var OperationName = _pdDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == empcode && x.SYKI == Syki).OrderByDescending(x => x.SYKI).Select(x => x.OPERATION).FirstOrDefault();
            return OperationName;
        }
        //added below method by eshant on 17-08-2022 to show division
        public string Getdivisiondetails(long empcode, int Syki)
        {
            var DivisionName = _pdDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == empcode && x.SYKI == Syki).OrderByDescending(x => x.SYKI).Select(x => x.DIVISION).FirstOrDefault();
            return DivisionName;
        }
        public A00DataViewModel GetA00_ADORGLEVELHEAD()
        {
            A00DataViewModel dvm = new A00DataViewModel();
            var empData = _pdDBContext.ADORGLEVELHEAD.Where(x => x.ISACTIVE == 1).ToList();
            if (empData.Count > 0)
            {
                foreach (var lvlList in empData)
                {
                    dvm._A00_ADORGLEVELHEAD.Add(new A00_ADORGLEVELHEAD
                    {
                        ADEMPCODE = lvlList.ADEMPCODE,
                        ADORGLEVELID = lvlList.ADORGLEVELID,
                        ADORGLEVELHEADID = lvlList.ADORGLEVELHEADID,
                        ISACTIVE = lvlList.ISACTIVE
                    });
                }
            }
            return dvm;
        }

        public Single_A00ADEMPLOYEE Get_Single_A00ADEMPLOYEE(A00SearchModel objSearchModel)
        {
            Single_A00ADEMPLOYEE dvm = new Single_A00ADEMPLOYEE();
            dvm.EMAILID = _pdDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == objSearchModel.DEPTHDID).Select(y => y.EMAILID).FirstOrDefault();
            return dvm;

            // _pdDBContext.ADEMPLOYEE
        }
        //Code By Ankit-Swaransoft - 15-01-2021
        /// <summary>
        /// Get Emp Email, Name and Emp Code
        /// </summary>
        /// <param name="ADEMPCODE"></param>
        /// <returns></returns>
        public Single_A00ADEMPLOYEE Get_Single_A00ADEMPLOYEEEmail(long? ADEMPCODE)
        {
            var dvm = _pdDBContext.ADEMPLOYEE.AsEnumerable().Where(x => x.ADEMPCODE == ADEMPCODE).Select(y => new Single_A00ADEMPLOYEE
            {
                EMAILID = y.EMAILID,
                Name = y.FIRSTNAME + " " + y.LASTNAME,
                EmpCode = Convert.ToString(y.ADEMPCODE)
            }).FirstOrDefault();
            return dvm;
        }

        public UserRole getUserRole(long empCode, decimal? SYKI)
        {
            try
            {


                var SYkID = SYKI != null && SYKI > 0 ? SYKI : _pdDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
                bool isDeptHead = false, isCoOrdHead = false, isDivHead = false, isExeCoOrdHead = false, isOperatingHead = false;
                var operation22 = _pdDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == empCode && x.SYKI == SYkID).
                                    Select(x => new { x.SECTIONID, x.DEPARTMENTID, x.DIVISIONID, x.OPERATIONID }).FirstOrDefault();
                if (operation22 != null)
                {
                    //22-Sept-2021 changee start
                    var coordOrExecCoordData = _pdDBContext.ADORGCOORDINATOR.Where(x => x.ADORGLEVELID == operation22.OPERATIONID && x.ISACTIVE == 1).FirstOrDefault();
                    //isDeptHead = _pdDBContext.ADORGLEVELHEAD.Any(x => x.ADORGLEVELID == operation22.DEPARTMENTID && x.ADEMPCODE == empCode && x.ISACTIVE == 1);
                    isDeptHead = (_pdDBContext.ADORGLEVELHEAD.FirstOrDefault(x => x.ADORGLEVELID == operation22.DEPARTMENTID && x.ADEMPCODE == empCode && x.ISACTIVE == 1)) == null ? false : true;
                    if (!isDeptHead)
                    {
                        isCoOrdHead = coordOrExecCoordData?.COORDINATOR == empCode;
                        if (!isCoOrdHead)
                        {
                            //isDivHead = _pdDBContext.ADORGLEVELHEAD.Any(x => x.ADORGLEVELID == operation22.DIVISIONID && x.ADEMPCODE == empCode && x.ISACTIVE == 1);
                            isDivHead = (_pdDBContext.ADORGLEVELHEAD.FirstOrDefault(x => x.ADORGLEVELID == operation22.DIVISIONID && x.ADEMPCODE == empCode && x.ISACTIVE == 1)) == null ? false : true;
                            if (!isDivHead)
                            {
                                isExeCoOrdHead = coordOrExecCoordData?.EXECOORDINATOR == empCode;
                                if (!isExeCoOrdHead)
                                {
                                    //07-Sept-2021 change start
                                    //isOperatingHead = _pdDBContext.ADORGLEVELHEAD.Any(x => x.ADORGLEVELID == operation22.OPERATIONID && x.ADEMPCODE == empCode);

                                    //09-Sept-2021 change start
                                    //isOperatingHead = _pdDBContext.ADORGCOORDINATOR.Any(x => x.ADORGLEVELID == operation22.OPERATIONID && x.OPHEAD == empCode);
                                    isOperatingHead = _pdDBContext.ADORGCOORDINATOR.FirstOrDefault(x => x.ADORGLEVELID == operation22.DIVISIONID && x.OPHEAD == empCode && x.ISACTIVE == 1) == null ? false : true;
                                    if (!isOperatingHead)
                                    {
                                        isOperatingHead = _pdDBContext.ADORGCOORDINATOR.FirstOrDefault(x => x.ADORGLEVELID == operation22.OPERATIONID && x.OPHEAD == empCode && x.ISACTIVE == 1) == null ? false : true;
                                    }
                                    //09-Sept-2021 change end

                                    //07-Sept-2021 change end
                                }
                            }
                        }
                    }
                    //22-Sept-2021 changee end

                }

                return new UserRole { isDeptHead = isDeptHead, isCoOrdHead = isCoOrdHead, isDivHead = isDivHead, isExeCoOrdHead = isExeCoOrdHead, isOperatingHead = isOperatingHead };
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }
        /// <summary>
        /// Get User Roles with SECTIONID, DEPARTMENTID, DIVISIONID, OPERATIONID
        /// </summary>
        /// <param name="empCode"></param>
        /// <param name="SYKI"></param>
        /// <returns></returns>
        public UserRole getUserRoles(long empCode, decimal? SYKI)
        {
            var SYkID = SYKI != null && SYKI > 0 ? SYKI : _pdDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
            bool isSecHead = false, isDeptHead = false, isCoOrdHead = false, isDivHead = false, isExeCoOrdHead = false, isOperatingHead = false;
            var operation22 = _pdDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == empCode && x.SYKI == SYkID).
                                Select(x => new { x.SECTIONID, x.DEPARTMENTID, x.DIVISIONID, x.OPERATIONID }).FirstOrDefault();
            if (operation22 != null)
            {
                //22-Sept-2021 changee start
                var coordOrExecCoordData = _pdDBContext.ADORGCOORDINATOR.Where(x => x.ADORGLEVELID == operation22.OPERATIONID && x.ISACTIVE == 1).FirstOrDefault();
                isSecHead = _pdDBContext.ADORGLEVELHEAD.FirstOrDefault(x => x.ADORGLEVELID == operation22.SECTIONID && x.ADEMPCODE == empCode && x.ISACTIVE == 1) == null ? false : true;
                if (!isSecHead)
                {
                    isDeptHead = _pdDBContext.ADORGLEVELHEAD.FirstOrDefault(x => x.ADORGLEVELID == operation22.DEPARTMENTID && x.ADEMPCODE == empCode && x.ISACTIVE == 1) == null ? false : true;
                    if (!isDeptHead)
                    {
                        isCoOrdHead = coordOrExecCoordData?.COORDINATOR == empCode;
                        if (!isCoOrdHead)
                        {
                            isDivHead = _pdDBContext.ADORGLEVELHEAD.FirstOrDefault(x => x.ADORGLEVELID == operation22.DIVISIONID && x.ADEMPCODE == empCode && x.ISACTIVE == 1) == null ? false : true;
                            if (!isDivHead)
                            {
                                isExeCoOrdHead = coordOrExecCoordData?.EXECOORDINATOR == empCode;
                                if (!isExeCoOrdHead)
                                {
                                    //07-Sept-2021 change start
                                    //isOperatingHead = _pdDBContext.ADORGLEVELHEAD.Any(x => x.ADORGLEVELID == operation22.OPERATIONID && x.ADEMPCODE == empCode);

                                    //09-Sept-2021 change start
                                    //isOperatingHead = _pdDBContext.ADORGCOORDINATOR.Any(x => x.ADORGLEVELID == operation22.OPERATIONID && x.OPHEAD == empCode);
                                    isOperatingHead = _pdDBContext.ADORGCOORDINATOR.FirstOrDefault(x => x.ADORGLEVELID == operation22.DIVISIONID && x.OPHEAD == empCode && x.ISACTIVE == 1) == null ? false : true;
                                    if (!isOperatingHead)
                                    {
                                        isOperatingHead = _pdDBContext.ADORGCOORDINATOR.FirstOrDefault(x => x.ADORGLEVELID == operation22.OPERATIONID && x.OPHEAD == empCode && x.ISACTIVE == 1) == null ? false : true;
                                    }
                                    //09-Sept-2021 change end

                                    //07-Sept-2021 change end
                                }
                            }
                        }
                    }
                }
                //22-Sept-2021 changee end
            }
            return new UserRole { isSecHead = isSecHead, isDeptHead = isDeptHead, isCoOrdHead = isCoOrdHead, isDivHead = isDivHead, isExeCoOrdHead = isExeCoOrdHead, isOperatingHead = isOperatingHead };
        }

        public bool GetUserRoleForDeficiency(long empCode, decimal? SYKI)
        {
            var operation22 = _pdDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == empCode && x.SYKI == SYKI).
                                Select(x => new { x.DEPARTMENTID, x.OPERATIONID, x.DIVISIONID, x.SECTIONID }).FirstOrDefault();

            var isOperatingHead = _pdDBContext.ADORGLEVELHEAD.FirstOrDefault(x =>
                                         x.ADORGLEVELID == operation22.OPERATIONID && x.ADEMPCODE == empCode) == null ? false : true;
            return isOperatingHead;
        }
        public A00_ADORGCOORDINATOR getA00_ADORGCOORDINATOR(A00SearchModel objSearchModel)
        {
            A00_ADORGCOORDINATOR cord = new A00_ADORGCOORDINATOR();
            var varCord = _pdDBContext.ADORGCOORDINATOR.Where(x => x.ADORGLEVELID == objSearchModel.OPERATIONID && x.ISACTIVE == 1).FirstOrDefault();
            if (varCord != null)
            {
                cord.COORDINATOR = varCord.COORDINATOR;
                cord.EXECOORDINATOR = varCord.EXECOORDINATOR;
            }
            return cord;
        }

        public InsertA00DtlTb InsertUpdateA00Detail(InsertA00DtlTb collection)
        {
            var ALTD = _pdDBContext.A00DTLTB.Where(x => x.A00DTLTBID == collection.A00DTLTBID).FirstOrDefault();
            if (ALTD == null)
            {
                ALTD = new A00DTLTB();
            }

            //ALTD.A00DTLTBID = collection.A00DTLTBID;
            ALTD.PRJCTTLE = collection.PRJCTTLE;
            ALTD.PRJCTTXT = collection.PRJCTTXT;
            ALTD.BUKPITXT = collection.BUKPITXT;
            ALTD.PURPSTXT = collection.PURPSTXT;
            ALTD.TRGTINDCD = collection.TRGTINDCD;
            ALTD.RQUMTTXT = collection.RQUMTTXT;
            ALTD.BUDGETFLG = collection.BUDGETFLG;
            ALTD.BUDGETSYKIID = collection.BUDGETSYKIID;
            ALTD.SYKIID = collection.SYKIID;
            ALTD.ADORGLEVELID = collection.ADORGLEVELID;
            ALTD.STARTDT = collection.STARTDT;
            ALTD.ENDDT = collection.ENDDT;
            ALTD.FRCSTOTHER = collection.FRCSTOTHER;
            if (collection.ATTACHMENT != null)
            {
                ALTD.ATTACHMENT = collection.ATTACHMENT;
            }
            ALTD.STATUSCD = collection.STATUSCD;
            ALTD.ADDEDBY = collection.ADDEDBY;
            ALTD.DEPARTMENT = collection.DEPARTMENT;
            ALTD.OPERATION = collection.OPERATION;
            ALTD.DIVISION = collection.DIVISION;
            ALTD.SECTION = collection.SECTION;
            ALTD.ACTIVE = collection.ACTIVE;
            ALTD.DATEADDEDID = collection.DATEADDEDID;
            ALTD.LASTMODDATE = collection.LASTMODDATE;
            ALTD.LSTMODBYID = collection.LSTMODBYID;
            ALTD.STATUSCD = collection.STATUSCD;

            if (ALTD.A00DTLTBID == 0)
            {
                ALTD.A00DTLTBID = collection.A00DTLTBID;
                _pdDBContext.Entry(ALTD).State = EntityState.Added;
                _pdDBContext.SaveChanges();
            }
            else
            {
                _pdDBContext.Entry(ALTD).State = EntityState.Modified;
                _pdDBContext.SaveChanges();
            }


            return collection;
        }

        public InsertA00APPROVAL InsertUpdateA00APPROVAL(InsertA00APPROVAL collection)
        {
            var approval = _pdDBContext.A00APPROVAL.Where(x => x.A00APPROVALID == collection.A00APPROVALID).FirstOrDefault();
            if (approval == null)
            {
                approval = new A00APPROVAL();
            }

            //approval.A00APPROVALID = collection.A00APPROVALID;
            approval.DEPTHDID = collection.DEPTHDID;
            approval.DEPTHDAPPTXT = collection.DEPTHDAPPTXT;
            approval.DEPTHDAPPDATE = collection.DEPTHDAPPDATE;

            approval.COORDDID = collection.COORDDID;
            approval.COORDAPPTXT = collection.COORDAPPTXT;
            approval.COORDAPPDATE = collection.COORDAPPDATE;

            approval.DIVHDHDID = collection.DIVHDHDID;
            approval.DIVHDAPPTXT = collection.DIVHDAPPTXT;
            approval.DIVHDAPPDATE = collection.DIVHDAPPDATE;

            approval.EXECOHDID = collection.EXECOHDID;
            approval.EXECOAPPTXT = collection.EXECOAPPTXT;
            approval.EXECOAPPDATE = collection.EXECOAPPDATE;

            approval.OHID = collection.OHID;
            approval.OHAPPTXT = collection.OHAPPTXT;
            approval.OHAPPDATE = collection.OHAPPDATE;

            //09-Sept-2021 change start
            approval.PPCHOOHID = collection.PPCHOOHID;
            approval.PPCHOOHTXT = collection.PPCHOOHTXT;
            approval.PPCHOOHDATE = collection.PPCHOOHDATE;
            //09-Sept-2021 change start

            approval.A00DTLTBID = collection.A00DTLTBID;

            if (approval.A00APPROVALID == 0)
            {
                approval.A00APPROVALID = collection.A00APPROVALID;
                _pdDBContext.Entry(approval).State = EntityState.Added;
                _pdDBContext.SaveChanges();
            }
            else
            {
                _pdDBContext.Entry(approval).State = EntityState.Modified;
                _pdDBContext.SaveChanges();
            }

            return collection;
        }
        //sa CR7306
        public InsertA00DtlTb InsertUpdateA00DetailCordinator(InsertA00DtlTb collection)
        {
            var ALTD = _pdDBContext.A00DTLTB.Where(x => x.A00DTLTBID == collection.A00DTLTBID).FirstOrDefault();
            if (ALTD == null)
            {
                ALTD = new A00DTLTB();
            }

            if (collection.ATTACHMENT != null)
            {
                ALTD.ATTACHMENT = collection.ATTACHMENT;
            }
            ALTD.STARTDT = collection.STARTDT;
            ALTD.ENDDT = collection.ENDDT;
            ALTD.STATUSCD = collection.STATUSCD;
            ALTD.LASTMODDATE = collection.LASTMODDATE;
            ALTD.LSTMODBYID = collection.LSTMODBYID;

            if (ALTD.A00DTLTBID == 0)
            {
                ALTD.A00DTLTBID = collection.A00DTLTBID;
                _pdDBContext.Entry(ALTD).State = EntityState.Added;
                _pdDBContext.SaveChanges();
            }
            else
            {
                _pdDBContext.Entry(ALTD).State = EntityState.Modified;
                _pdDBContext.SaveChanges();
            }


            return collection;
        }

        public InsertA00APPROVAL InsertUpdateA00APPROVALCordinator(InsertA00APPROVAL collection)
        {
            var approval = _pdDBContext.A00APPROVAL.Where(x => x.A00APPROVALID == collection.A00APPROVALID).FirstOrDefault();
            if (approval == null)
            {
                approval = new A00APPROVAL();
            }

            approval.DIVHDHDID = collection.DIVHDHDID;
            approval.OHID = collection.OHID; //CR7306 Updated Code
            approval.COORDAPPTXT = collection.COORDAPPTXT;
            approval.COORDAPPDATE = collection.COORDAPPDATE;

            if (approval.A00APPROVALID == 0)
            {
                approval.A00APPROVALID = collection.A00APPROVALID;
                _pdDBContext.Entry(approval).State = EntityState.Added;
                _pdDBContext.SaveChanges();
            }
            else
            {
                _pdDBContext.Entry(approval).State = EntityState.Modified;
                _pdDBContext.SaveChanges();
            }
            return collection;
        }

        //ea CR7306

        public InsertA00INVFORCAST InsertUpdateA00INVFORCAST(InsertA00INVFORCAST collection)
        {
            var forecastobj = new A00INVFORCAST();
            forecastobj.A00INVFORCASTID = collection.A00INVFORCASTID;
            forecastobj.A00DTLTBID = collection.A00DTLTBID;
            forecastobj.DATEADDEDID = collection.DATEADDEDID;
            forecastobj.INVFORCASTID = collection.INVFORCASTID;
            forecastobj.ACTIVE = 1;
            _pdDBContext.Entry(forecastobj).State = EntityState.Added;
            _pdDBContext.SaveChanges();

            return collection;
        }


        public A00Deficiency InsertDeffience(A00Deficiency collection)
        {
            var allocation = new A00Deficiency();
            // return allocation;

            allocation.A00DeficiencyID = collection.A00DeficiencyID;
            allocation.A00ID = collection.A00ID;
            allocation.AddedBY = collection.AddedBY;
            allocation.DeficiencyText = collection.DeficiencyText;
            allocation.Attachment = collection.Attachment;

            _pdDBContext.Entry(allocation).State = EntityState.Added;
            _pdDBContext.SaveChanges();
            return collection;
        }

        public int SaveA00Deficiency(string Remark, string fileName, long SYKI, string isDeficiencyFileExist, int userId, long A00DTLTBID)
        {
            var deficiency = new A00_DEFICIENCY();
            // return allocation;
            var ACTIONFOR = _pdDBContext.A00DTLTB.Where(x => x.SYKIID == SYKI && x.A00DTLTBID == A00DTLTBID).Select(x => x.ADDEDBY).FirstOrDefault();
            deficiency.A00DTLTBID = A00DTLTBID;
            deficiency.SYKIID = SYKI;
            deficiency.REMARKS = Remark;
            deficiency.ATTACHMENTNOTE = fileName;
            deficiency.ACTIONBY = userId;
            deficiency.ACTIONON = DateTime.Now;
            deficiency.ACTIONFOR = ACTIONFOR;
            deficiency.ACTIVE = 1;
            deficiency.STATUS = 1;
            _pdDBContext.Entry(deficiency).State = EntityState.Added;
            _pdDBContext.SaveChanges();
            return 1;
        }

        public int SaveA00DeficiencyUpdate(string Remark, string fileName, long SYKI, string isDeficiencyFileExist, int userId, long A00DTLTBID)
        {

            var deficiency = new A00_DEFICIENCY();
            // return allocation;
            var ACTIONFOR = _pdDBContext.A00_DEFICIENCY.Where(x => x.A00DTLTBID == A00DTLTBID && x.SYKIID == SYKI).OrderByDescending(x => x.ID).Select(x => x.ACTIONBY).FirstOrDefault();
            deficiency.A00DTLTBID = A00DTLTBID;
            deficiency.SYKIID = SYKI;
            deficiency.REMARKS = Remark;
            deficiency.ATTACHMENTNOTE = fileName;
            deficiency.ACTIONBY = userId;
            deficiency.ACTIONON = DateTime.Now;
            deficiency.ACTIONFOR = Convert.ToDecimal(ACTIONFOR);
            deficiency.ACTIVE = 1;
            deficiency.STATUS = 2;
            _pdDBContext.Entry(deficiency).State = EntityState.Added;
            _pdDBContext.SaveChanges();
            return 1;
        }

        public int SaveA00DeficiencyClosure(int userId, long A00DTLTBID, long SYKI, string Remark, int deficiencyStatus)
        {

            var deficiency = new A00_DEFICIENCY();
            var ACTIONFOR = _pdDBContext.A00_DEFICIENCY.Where(x => x.A00DTLTBID == A00DTLTBID && x.SYKIID == SYKI).OrderByDescending(x => x.ID).Select(x => x.ACTIONBY).FirstOrDefault();
            if (deficiencyStatus == 4)
            {
                deficiency.A00DTLTBID = A00DTLTBID;
                deficiency.SYKIID = SYKI;
                deficiency.ATTACHMENTNOTE = "NA";
                deficiency.REMARKS = Remark;
                deficiency.ACTIONBY = userId;
                deficiency.ACTIONON = DateTime.Now;
                deficiency.ACTIONFOR = 0;
                deficiency.ACTIVE = 1;
                deficiency.STATUS = 4;
                _pdDBContext.Entry(deficiency).State = EntityState.Added;
                _pdDBContext.SaveChanges();
            }
            else if (deficiencyStatus == 3)
            {
                deficiency.A00DTLTBID = A00DTLTBID;
                deficiency.SYKIID = SYKI;
                deficiency.ATTACHMENTNOTE = "NA";
                deficiency.REMARKS = Remark;
                deficiency.ACTIONBY = userId;
                deficiency.ACTIONON = DateTime.Now;
                deficiency.ACTIONFOR = Convert.ToDecimal(ACTIONFOR);
                deficiency.ACTIVE = 1;
                deficiency.STATUS = 3;
                _pdDBContext.Entry(deficiency).State = EntityState.Added;
                _pdDBContext.SaveChanges();
            }
            // return allocation;
            return 1;
        }

        public List<GetEmployeeA00AllocationVM> GetEmployeeA00AllocationList(long OPERATIONID, long SYKIID)
        {
            List<GetEmployeeA00AllocationVM> GetEmployeeA00AllocationList = new List<GetEmployeeA00AllocationVM>();
            var iList = (from t1 in _pdDBContext.VW_ASSOCIATELVLDETAILS
                         join t2 in _pdDBContext.ADEMPLOYEE on t1.ADEMPCODE equals t2.ADEMPCODE
                         where t1.ACTIVE == 1 //&& t1.SYKI == SYKIID
                         //where t1.ACTIVE == 1 && t1.SYKI == SYKIID //Commented By Eshant on 23-09-2022 and removed SYKIID check to get old current active employees
                         && t1.OPERATIONID == OPERATIONID
                         orderby t2.FIRSTNAME
                         select new { t2.ADEMPCODE, t2.FIRSTNAME, t2.LASTNAME }).ToList();
            if (iList.Count > 0)
            {
                foreach (var lvlList in iList)
                {
                    GetEmployeeA00AllocationList.Add(new GetEmployeeA00AllocationVM
                    {
                        Employee = lvlList.FIRSTNAME + ' ' + lvlList.LASTNAME,
                        Id = lvlList.ADEMPCODE,
                    });
                }
            }
            return GetEmployeeA00AllocationList;
        }

        //public int SaveA00Allocation(long SYKI,int userId, long A00DTLTBID, string ApplicationPIC, string InfrastructurePIC, string OtherMembers)
        //{
        //    var allocation = new A00Allocation();
        //    // return allocation;
        //    var ACTIONFOR = _pdDBContext.A00DTLTB.Where(x => x.SYKIID == SYKI).Select(x => x.ADDEDBY).FirstOrDefault();
        //    allocation.A00AllocationID = A00DTLTBID;
        //    allocation.A00ID = SYKI;
        //    allocation.ApplicationPICECode = ApplicationPIC;
        //    allocation.InfraPICECode = InfrastructurePIC;
        //    allocation.MainPIC = OtherMembers;
        //    allocation.AddedBY = userId;
        //    allocation.AddedDate = DateTime.Now.ToShortDateString();
        //    allocation.LastUpdateDate = DateTime.Now.ToShortDateString();
        //    allocation.LastUpdatedBy = userId.ToString();
        //    _pdDBContext.Entry(allocation).State = EntityState.Added;
        //    _pdDBContext.SaveChanges();
        //    return 1;
        //}

        public List<GetDeficiencyRaisedVM> GetDeficiencyRaisedDetails(long? A00DTLTBID, decimal? SYKIID)
        {
            List<GetDeficiencyRaisedVM> GetDeficiencyRaisedList = new List<GetDeficiencyRaisedVM>();
            var iList = (from t1 in _pdDBContext.A00_DEFICIENCY
                         join t2 in _pdDBContext.ADEMPLOYEE on t1.ACTIONBY equals t2.ADEMPCODE
                         where t1.ACTIVE == 1 && t1.A00DTLTBID == A00DTLTBID && t1.SYKIID == SYKIID
                         orderby t1.ID
                         select new { t2.ADEMPCODE, t2.FIRSTNAME, t2.LASTNAME, t1.REMARKS, t1.ACTIONON, t1.ATTACHMENTNOTE, t1.ACTIONFOR, t1.ACTIONBY }).ToList();
            if (iList.Count > 0)
            {
                foreach (var lvlList in iList)
                {
                    GetDeficiencyRaisedList.Add(new GetDeficiencyRaisedVM
                    {
                        File = lvlList.ATTACHMENTNOTE !="NA" ? "A00Deficiency/" + lvlList.ATTACHMENTNOTE : "A00Deficiency" + lvlList.ATTACHMENTNOTE,
                        Remarks = lvlList.REMARKS,
                        ActionBy = lvlList.FIRSTNAME + ' ' + lvlList.LASTNAME,
                        ActionOn = lvlList.ACTIONON.ToShortDateString(),
                        ActionFor = (long)lvlList.ACTIONFOR,
                        EmpActionBy = (int)lvlList.ACTIONBY,
                    });
                }
            }
            return GetDeficiencyRaisedList;
        }

        public int GetDeficiencyStatus(long? A00DTLTBID, decimal? SYKIID)
        {
            var isITConfirmationFilled = _pdDBContext.A00ITCONFIRMATION.Where(x => x.A00ID == A00DTLTBID && x.SYKIID == SYKIID).Select(x => x.ID).FirstOrDefault();
            var Status = _pdDBContext.A00_DEFICIENCY.Where(x => x.A00DTLTBID == A00DTLTBID && x.SYKIID == SYKIID).OrderByDescending(x => x.ID).Select(x => x.STATUS).FirstOrDefault();
            Status = isITConfirmationFilled > 0 ? (short)5 : Status;
            return Status;
        }

        public short? IsDeficiencyExist(long? A00DTLTBID, decimal? SYKIID)
        {
            var Status = _pdDBContext.A00_DEFICIENCY.Where(x => x.A00DTLTBID == A00DTLTBID && x.SYKIID == SYKIID).OrderByDescending(x => x.ID).Select(x => x.STATUS).FirstOrDefault();
            return Status;
        }

        public short? IsITConfirmationDone(long? A00DTLTBID, decimal? SYKIID)
        {
            var Status = _pdDBContext.A00ITCONFIRMATION.Where(x => x.A00ID == A00DTLTBID && x.SYKIID == SYKIID).Select(x => x.AppTeamSTATUS).FirstOrDefault();
            return Status;
        }

        public int SaveA00Allocation(long SYKI, long ApplicationPIC, long InfrastructurePIC, string OtherMembers, int userId, long A00DTLTBID, int rdoMainPic)
        {
            var IsAllocationExist = _pdDBContext.A00_ALLOCATION.FirstOrDefault(x => x.A00DTLTBID == A00DTLTBID && x.SYKIID == SYKI) == null ? false : true;
            var allocation = new A00_ALLOCATION();
            if (IsAllocationExist == false)
            {
                allocation.A00DTLTBID = A00DTLTBID;
                allocation.SYKIID = SYKI;
                allocation.APPLICATIONPIC = ApplicationPIC;
                allocation.INFRASTRUCTUREPIC = InfrastructurePIC;
                allocation.OTHERMEMBERS = OtherMembers;
                allocation.ALLOCATEDBY = userId;
                allocation.ALLOCATEDON = DateTime.Now;
                allocation.ACTIVE = 1;
                allocation.STATUS = 1;
                allocation.MainPIC = rdoMainPic;
                _pdDBContext.Entry(allocation).State = EntityState.Added;
                _pdDBContext.SaveChanges();
            }
            else if (IsAllocationExist == true)
            {
                allocation = _pdDBContext.A00_ALLOCATION.FirstOrDefault(x => x.A00DTLTBID == A00DTLTBID && x.SYKIID == SYKI);
                allocation.A00DTLTBID = A00DTLTBID;
                allocation.SYKIID = SYKI;
                allocation.APPLICATIONPIC = ApplicationPIC;
                allocation.INFRASTRUCTUREPIC = InfrastructurePIC;
                allocation.OTHERMEMBERS = OtherMembers;
                allocation.ALLOCATEDBY = userId;
                allocation.ALLOCATEDON = DateTime.Now;
                allocation.ACTIVE = 1;
                allocation.STATUS = 1;
                allocation.MainPIC = rdoMainPic;
                _pdDBContext.Entry(allocation).State = EntityState.Modified;
                _pdDBContext.SaveChanges();
            }
            return 1;
        }


        public List<long> GetA00Allocation()
        {
            var A00AllocationList = _pdDBContext.A00_ALLOCATION.Where(x => x.ACTIVE == 1).Select(x => x.A00DTLTBID).ToList();
            return A00AllocationList;
        }

        public List<GetAllocationVM> GetAllocationDetails(long? A00DTLTBID, decimal? SYKIID)
        {
            List<GetAllocationVM> AllocationList = new List<GetAllocationVM>();
            var iList = _pdDBContext.A00_ALLOCATION.Where(X => X.A00DTLTBID == A00DTLTBID && X.SYKIID == SYKIID && X.ACTIVE == 1).ToList();
            {
                foreach (var lvlList in iList)
                {
                    var ApplicationPic = _pdDBContext.ADEMPLOYEE.Where(X => X.ADEMPCODE == lvlList.APPLICATIONPIC).Select(X => new { X.FIRSTNAME, X.LASTNAME }).FirstOrDefault();
                    string _ApplicationPic = ApplicationPic.FIRSTNAME + ' ' + ApplicationPic.LASTNAME;
                    var InfrastructurePIC = _pdDBContext.ADEMPLOYEE.Where(X => X.ADEMPCODE == lvlList.INFRASTRUCTUREPIC).Select(X => new { X.FIRSTNAME, X.LASTNAME }).FirstOrDefault();
                    string _InfrastructurePIC = InfrastructurePIC.FIRSTNAME + ' ' + InfrastructurePIC.LASTNAME;
                    string OtherMember = lvlList.OTHERMEMBERS;
                    string _OtherMember = string.Empty;
                    string _MainPic = string.Empty;
                    int MainPic = lvlList.MainPIC != null ? (int)lvlList.MainPIC : 0;
                    if (MainPic == 1)
                    {
                        _MainPic = _ApplicationPic;
                    }
                    else
                    {
                        _MainPic = _InfrastructurePIC;
                    }
                    if (OtherMember != null)
                    {
                        string[] arr = OtherMember.Split(',');
                        for (int i = 0; i < arr.Length; i++)
                        {
                            int EmpCode = Convert.ToInt32(arr[i]);
                            var result = _pdDBContext.ADEMPLOYEE.Where(X => X.ADEMPCODE == EmpCode).Select(X => new { X.FIRSTNAME, X.LASTNAME }).FirstOrDefault();
                            string EmpName = result.FIRSTNAME + ' ' + result.LASTNAME;
                            _OtherMember += EmpName + ",";
                        }
                    }

                    var AllocatedBy = _pdDBContext.ADEMPLOYEE.Where(X => X.ADEMPCODE == lvlList.ALLOCATEDBY).Select(X => new { X.FIRSTNAME, X.LASTNAME }).FirstOrDefault();
                    string _AllocatedBy = AllocatedBy.FIRSTNAME + ' ' + AllocatedBy.LASTNAME;

                    AllocationList.Add(new GetAllocationVM
                    {
                        ApplicationPic = _ApplicationPic,
                        InfrastructurePic = _InfrastructurePIC,
                        OtherMember = _OtherMember,
                        AllocatedBy = _AllocatedBy,
                        AllocationOn = Convert.ToDateTime(lvlList.ALLOCATEDON).ToShortDateString(),
                        MainPic = _MainPic,
                    });
                }
            }
            return AllocationList;
        }

        public bool IsA00DeficiencyRaised(long? A00DTLTBID, decimal? SYKIID)
        {
            var IsDeficiencyRaised = _pdDBContext.A00_DEFICIENCY.FirstOrDefault(x => x.A00DTLTBID == A00DTLTBID && x.SYKIID == SYKIID && (x.STATUS == 1 || x.STATUS == 3) && x.ACTIVE == 1) == null ? false : true;
            return IsDeficiencyRaised;
        }
        public bool IsA00DeficiencyClosed(long? A00DTLTBID, decimal? SYKIID)
        {
            var Status = _pdDBContext.A00_DEFICIENCY.Where(x => x.A00DTLTBID == A00DTLTBID && x.SYKIID == SYKIID).OrderByDescending(x => x.ID).Select(x => x.STATUS).FirstOrDefault();

            var IsDeficiencyClosed = _pdDBContext.A00_DEFICIENCY.FirstOrDefault(x => x.A00DTLTBID == A00DTLTBID && x.SYKIID == SYKIID && x.STATUS == 4 && x.ACTIVE == 1) == null ? false : true;
            return IsDeficiencyClosed;
        }

        //public List<long>  IsUserItConfirmation(int UserId, long? A00DTLTBID, decimal? SYKIID)
        //{
        //    var IsUserItConfirmation = _pdDBContext.A00_ALLOCATION.Where(x => x.ACTIVE == 1 && x.A00DTLTBID == A00DTLTBID && x.SYKIID == SYKIID && (x.APPLICATIONPIC == UserId || x.INFRASTRUCTUREPIC == UserId)).Select(x => x.A00DTLTBID).ToList();
        //    return IsUserItConfirmation;
        //}

        //public int GetInfrastructure(int UserId, long? A00DTLTBID, decimal? SYKIID)
        //{
        //    var GetInfrastructure = _pdDBContext.A00_ALLOCATION.Where(x => x.A00DTLTBID == 2 && x.SYKIID == SYKIID && (x.APPLICATIONPIC == UserId || x.INFRASTRUCTUREPIC == UserId) && x.STATUS == 1 && x.ACTIVE == 1).Select(x => x.INFRASTRUCTUREPIC).FirstOrDefault();
        //    int INFRASTRUCTUREPIC = (int)GetInfrastructure;
        //    return INFRASTRUCTUREPIC;
        //}

        public A00DataViewModel GetA00AllocationList()
        {
            A00DataViewModel dvm = new A00DataViewModel();
            dvm._A00AllocationList = _pdDBContext.A00_ALLOCATION.Select(x => new A00Allocation()
            {
                A00AllocationID = x.ID,
                A00ID = x.A00DTLTBID,
                SKYID = x.SYKIID,
                ApplicationPICECode = x.APPLICATIONPIC,
                InfraPICECode = x.INFRASTRUCTUREPIC,
                MainPICTypeID = x.MainPIC,
                AllocatedBY = x.ALLOCATEDBY,
                AllocatedDate = x.ALLOCATEDON,
            }).ToList();
            return dvm;
        }

        public A00DataViewModel GetA00ItConfirmationList()
        {
            try
            {
                A00DataViewModel dvm = new A00DataViewModel();
                dvm._A00ITConfirmationList = _pdDBContext.A00ITCONFIRMATION.AsEnumerable().Where(x => x.ACTIVE == 1).Select(x => new ITConfirmationVM
                {
                    ITCONFIRMATIONID = x.ID,
                    A00ALLOCATIONID = x.A00ALLOCATIONID,
                    A00DTLTBID = x.A00ID,
                    SYKIID = x.SYKIID,
                    MAINPIC_ID = x.MAINPIC_ID,
                    ProjectCode = x.PROJECTCODE,
                    ProjectCode_Date = x.PROJECTCODE,
                    ProjectDate = Convert.ToString(x.PROJECTDATE),
                    ProjectName = x.PROJECTNAME,
                    ImpactedOperation = x.IMPACTEDOPERATIONS,
                    UserPL = x.USERPL,
                    UserDept = x.USERDEPT,
                    ITPL = x.ITPL,
                    ITDept = x.ITDEPT,
                    SAA = x.SAA,
                    LWEA = x.LWEA,
                    Development = x.DEVELOPMENT,
                    LicenseRequired = x.LICENSEREQUIRED,
                    InfrastructureRequirement = x.IREQUIREMENT,
                    OEM = x.LRIFYES,
                    IR = x.IRIFYES,
                    OTC = x.TONETIMECOST,
                    RC = x.TRECURRINGCOST,
                    TTimeLine = x.TTIMELINE,
                    Application = x.APPLICATION,
                    NOFS = x.NOOFSCREEN,
                    HSDM = x.HSDMCR,
                    UOTarget = x.U0TARGETDATE,
                    Remarks = x.REMARKS,
                    Status = x.AppTeamSTATUS,
                    ITConfirmationFilledBy = x.ITCONFIRMATIONFILLEDBY,
                    ITCONFIRMATIONApprovalDate = x.LASTMODDATE,
                    ITConfirmationLatestHistoryVM = _pdDBContext.A00ITCONFAPPROVALHISTORY.Where(y => y.ITCONFIRMATIONSUBMITTEDSTATUS != null && y.A00ID == x.A00ID).OrderByDescending(y => y.ID).Select(y => new ITConfirmationApprovalHistoryVM
                    {
                        A00ID = y.A00ID,
                        SYKIID = y.SYKIID,
                        ActionBy = y.ACTIONBY,
                        Status = y.STATUS,
                        ActionType = y.ACTIONTYPE,
                        UserType = y.USERTYPE,
                        ITConfirmationSubmittedTo = y.ITCONFIRMATIONSUBMITTEDTO,
                        ITCONFSUBMITTEDTOUSERTYPE = y.ITCONFSUBMITTEDTOUSERTYPE,
                        ITCONFSUBMITTEDTOSTATUS = y.ITCONFIRMATIONSUBMITTEDSTATUS,
                    }).FirstOrDefault()
                }).ToList();
                return dvm;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        public string GetUserEmailForDeficiencyRaised(long? A00DTLTBID, long? SYKI)
        {
            var ADDEDBY = _pdDBContext.A00DTLTB.Where(x => x.A00DTLTBID == A00DTLTBID && x.SYKIID == SYKI).Select(x => x.ADDEDBY).FirstOrDefault();
            var emailId = _pdDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == ADDEDBY).Select(x => x.EMAILID).FirstOrDefault();
            return emailId;
        }
        public string GetDeficiencyUpdateEmialId(long? A00DTLTBID, long? SYKI)
        {
            var ACTIONFOR = _pdDBContext.A00_DEFICIENCY.Where(x => x.A00DTLTBID == A00DTLTBID && x.SYKIID == SYKI).OrderByDescending(x => x.ID).Select(x => x.ACTIONBY).FirstOrDefault();
            var emailId = _pdDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == ACTIONFOR).Select(x => x.EMAILID).FirstOrDefault();
            return emailId;
        }
        public string GetDeficiencyUpdaterName(long? A00DTLTBID, long? SYKI)
        {
            var ACTIONFOR = _pdDBContext.A00_DEFICIENCY.Where(x => x.A00DTLTBID == A00DTLTBID && x.SYKIID == SYKI).OrderByDescending(x => x.ID).Select(x => x.ACTIONBY).FirstOrDefault();
            var emailId = _pdDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == ACTIONFOR).Select(x => x.EMAILID).FirstOrDefault();
            return emailId;
        }

        public string GetA00ProjectTitle(long? A00DTLTBID, long? SYKI)
        {
            var ProjectTitle = _pdDBContext.A00DTLTB.Where(x => x.A00DTLTBID == A00DTLTBID && x.SYKIID == SYKI).Select(x => x.PRJCTTLE).FirstOrDefault();
            return ProjectTitle;
        }
        public ProjectVm GetA00ProjectDetails(long? A00DTLTBID, decimal SYKI)
        {
            ProjectVm ProjectDetails = new ProjectVm();
            ProjectDetails = _pdDBContext.A00DTLTB.Where(x => x.A00DTLTBID == A00DTLTBID && x.SYKIID == SYKI).Select(x => new ProjectVm()
            {
                ProjectName = x.PRJCTTLE,
                ProjectDate = x.DATEADDEDID,
                AddedBy = x.ADDEDBY,
            }).FirstOrDefault();
            return ProjectDetails;
        }

        public string GetEmpDept(long? EmpCode, decimal? SYKIID)
        {
            var DeptName = _pdDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == EmpCode && x.SYKI == SYKIID && x.ACTIVE == 1).Select(x => x.DEPARTMENT).FirstOrDefault();
            return DeptName;
        }

        public string GetEmpName(long? EmpCode)
        {
            //var EmpDetails = _pdDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == EmpCode && x.ACTIVE == 1).FirstOrDefault();
            //Above line commented by eshant to get name of employee if he left from org, so active is removed. 15-Jul-2022
            var EmpDetails = _pdDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == EmpCode).FirstOrDefault();
            string EmpName = EmpDetails.FIRSTNAME + "  " + EmpDetails.LASTNAME;
            return EmpName;
        }

        public long GetA00MainPIC(long? A00DTLTBID, decimal? SYKI)
        {
            long MainPICEmpCode = 0;
            //var AllocatonDetails = _pdDBContext.A00_ALLOCATION.Where(x => x.A00DTLTBID == A00DTLTBID && x.SYKIID == SYKI && x.ACTIVE == 1).FirstOrDefault();
            //Above line commented by Eshant to get latest allocated record -- 16-Jul-2022 added below updated query. for Update A00 Project Status
            var AllocatonDetails = _pdDBContext.A00_ALLOCATION.Where(x => x.A00DTLTBID == A00DTLTBID && x.ACTIVE == 1).OrderByDescending(o => o.ID).FirstOrDefault();
            int MainPIC = Convert.ToInt32(AllocatonDetails.MainPIC);
            if (MainPIC == 1)
            {
                MainPICEmpCode = (long)AllocatonDetails.APPLICATIONPIC;
            }
            else
            {
                MainPICEmpCode = (long)AllocatonDetails.INFRASTRUCTUREPIC;
            }
            return MainPICEmpCode;
        }

        //22-Sept-2021 change start
        public int ITConfirmationSaveData(ITConfirmationVM ITConfirmation, int userId, string btnText, A00SearchModel _objA00SearchModel)
        {
            try
            {
                _pdDBContext.Database.BeginTransaction();
                int result = 0;
                A00ITCONFIRMATION A00ITCONFIRMATION = new A00ITCONFIRMATION();
                A00ITCONFAPPROVALHISTORY A00ITCONFAPPROVALHISTORY = new A00ITCONFAPPROVALHISTORY();
                var AllocatonDetails = _pdDBContext.A00_ALLOCATION.Where(x => x.A00DTLTBID == ITConfirmation.A00DTLTBID && x.ACTIVE == 1).FirstOrDefault();
                var A00ITCONFIRMATIONUpdate = _pdDBContext.A00ITCONFIRMATION.Where(x => x.A00ID == ITConfirmation.A00DTLTBID && x.ACTIVE == 1).FirstOrDefault();
                //Commented By Eshant SYKIID to remove syki id check for old syki check 12-Jul-22
                //var AllocatonDetails = _pdDBContext.A00_ALLOCATION.Where(x => x.A00DTLTBID == ITConfirmation.A00DTLTBID && x.SYKIID == ITConfirmation.SYKIID && x.ACTIVE == 1).FirstOrDefault();
                //var A00ITCONFIRMATIONUpdate = _pdDBContext.A00ITCONFIRMATION.Where(x => x.A00ID == ITConfirmation.A00DTLTBID && x.SYKIID == ITConfirmation.SYKIID && x.ACTIVE == 1).FirstOrDefault();
                if (btnText == "SAVE AS DRAFT")
                {
                    #region Save as draft
                    if (A00ITCONFIRMATIONUpdate != null)
                    {
                        A00ITCONFIRMATIONUpdate.A00ID = ITConfirmation.A00DTLTBID;
                        A00ITCONFIRMATIONUpdate.SYKIID = ITConfirmation.SYKIID;
                        A00ITCONFIRMATIONUpdate.A00ALLOCATIONID = AllocatonDetails.ID;
                        A00ITCONFIRMATIONUpdate.MAINPIC_ID = (short)AllocatonDetails.MainPIC;
                        A00ITCONFIRMATIONUpdate.PROJECTCODE = ITConfirmation.ProjectCode_Date;
                        A00ITCONFIRMATIONUpdate.PROJECTDATE = Convert.ToDateTime(ITConfirmation.ProjectDate);
                        A00ITCONFIRMATIONUpdate.PROJECTNAME = ITConfirmation.ProjectName;
                        A00ITCONFIRMATIONUpdate.IMPACTEDOPERATIONS = ITConfirmation.ImpactedOperation;
                        A00ITCONFIRMATIONUpdate.USERPL = ITConfirmation.UserPL;
                        A00ITCONFIRMATIONUpdate.USERDEPT = ITConfirmation.UserDept;
                        A00ITCONFIRMATIONUpdate.ITPL = ITConfirmation.ITPL;
                        A00ITCONFIRMATIONUpdate.ITDEPT = ITConfirmation.ITDept;
                        A00ITCONFIRMATIONUpdate.SAA = ITConfirmation.SAA;
                        A00ITCONFIRMATIONUpdate.LWEA = ITConfirmation.LWEA;
                        A00ITCONFIRMATIONUpdate.DEVELOPMENT = ITConfirmation.Development;
                        A00ITCONFIRMATIONUpdate.LICENSEREQUIRED = ITConfirmation.LicenseRequired;
                        A00ITCONFIRMATIONUpdate.LRIFYES = ITConfirmation.OEM;
                        A00ITCONFIRMATIONUpdate.IREQUIREMENT = ITConfirmation.InfrastructureRequirement;
                        A00ITCONFIRMATIONUpdate.IRIFYES = ITConfirmation.IR;
                        A00ITCONFIRMATIONUpdate.TONETIMECOST = ITConfirmation.OTC;
                        A00ITCONFIRMATIONUpdate.TRECURRINGCOST = ITConfirmation.RC;
                        A00ITCONFIRMATIONUpdate.TTIMELINE = ITConfirmation.TTimeLine;
                        A00ITCONFIRMATIONUpdate.APPLICATION = ITConfirmation.Application;
                        A00ITCONFIRMATIONUpdate.NOOFSCREEN = ITConfirmation.NOFS;
                        A00ITCONFIRMATIONUpdate.HSDMCR = ITConfirmation.HSDM;
                        A00ITCONFIRMATIONUpdate.U0TARGETDATE = ITConfirmation.UOTarget;
                        A00ITCONFIRMATIONUpdate.REMARKS = ITConfirmation.Remarks;
                        A00ITCONFIRMATIONUpdate.ACTIVE = 1;
                        //A00ITCONFIRMATIONUpdate.AppTeamSTATUS = 0;
                        //A00ITCONFIRMATIONUpdate.INFRATEAMSTATUS = 0;
                        //A00ITCONFIRMATIONUpdate.ITCONFIRMATIONTOAPPTEAM = 0;
                        //A00ITCONFIRMATIONUpdate.ITCONFIRMATIONTOINFRATEAM = 0;
                        A00ITCONFIRMATIONUpdate.LSTMODBYID = userId;
                        A00ITCONFIRMATIONUpdate.LASTMODDATE = DateTime.Now;

                        //22-Sept-2021 change start
                        if (ITConfirmation.Attachment_Name != null && ITConfirmation.Attachment_Name != "")
                        {
                            A00ITCONFIRMATIONUpdate.ATTACHMENT = ITConfirmation.Attachment_Name;
                        }
                        //22-Sept-2021 change end

                        _pdDBContext.Entry(A00ITCONFIRMATIONUpdate).State = EntityState.Modified;
                        _pdDBContext.SaveChanges();
                    }
                    else
                    {
                        A00ITCONFIRMATION.A00ALLOCATIONID = AllocatonDetails.ID;
                        A00ITCONFIRMATION.MAINPIC_ID = (short)AllocatonDetails.MainPIC;
                        A00ITCONFIRMATION.A00ID = ITConfirmation.A00DTLTBID;
                        A00ITCONFIRMATION.SYKIID = ITConfirmation.SYKIID;
                        A00ITCONFIRMATION.PROJECTCODE = ITConfirmation.ProjectCode_Date;
                        A00ITCONFIRMATION.PROJECTDATE = Convert.ToDateTime(ITConfirmation.ProjectDate);
                        A00ITCONFIRMATION.PROJECTNAME = ITConfirmation.ProjectName;
                        A00ITCONFIRMATION.IMPACTEDOPERATIONS = ITConfirmation.ImpactedOperation;
                        A00ITCONFIRMATION.USERPL = ITConfirmation.UserPL;
                        A00ITCONFIRMATION.USERDEPT = ITConfirmation.UserDept;
                        A00ITCONFIRMATION.ITPL = ITConfirmation.ITPL;
                        A00ITCONFIRMATION.ITDEPT = ITConfirmation.ITDept;
                        A00ITCONFIRMATION.SAA = ITConfirmation.SAA;
                        A00ITCONFIRMATION.LWEA = ITConfirmation.LWEA;
                        A00ITCONFIRMATION.DEVELOPMENT = ITConfirmation.Development;
                        A00ITCONFIRMATION.LICENSEREQUIRED = ITConfirmation.LicenseRequired;
                        A00ITCONFIRMATION.LRIFYES = ITConfirmation.OEM;
                        A00ITCONFIRMATION.IREQUIREMENT = ITConfirmation.InfrastructureRequirement;
                        A00ITCONFIRMATION.IRIFYES = ITConfirmation.IR;
                        A00ITCONFIRMATION.TONETIMECOST = ITConfirmation.OTC;
                        A00ITCONFIRMATION.TRECURRINGCOST = ITConfirmation.RC;
                        A00ITCONFIRMATION.TTIMELINE = ITConfirmation.TTimeLine;
                        A00ITCONFIRMATION.APPLICATION = ITConfirmation.Application;
                        A00ITCONFIRMATION.NOOFSCREEN = ITConfirmation.NOFS;
                        A00ITCONFIRMATION.HSDMCR = ITConfirmation.HSDM;
                        A00ITCONFIRMATION.U0TARGETDATE = ITConfirmation.UOTarget;
                        A00ITCONFIRMATION.REMARKS = ITConfirmation.Remarks;
                        A00ITCONFIRMATION.ACTIVE = 1;
                        //A00ITCONFIRMATION.AppTeamSTATUS = 0;
                        //A00ITCONFIRMATION.INFRATEAMSTATUS = 0;
                        //A00ITCONFIRMATION.ITCONFIRMATIONTOAPPTEAM = 0;
                        //A00ITCONFIRMATION.ITCONFIRMATIONTOINFRATEAM = 0;
                        A00ITCONFIRMATION.ITCONFIRMATIONFILLEDBY = userId;
                        A00ITCONFIRMATION.ITCONFIRMATIONFILLEDON = DateTime.Now;

                        //22-Sept-2021 change start
                        if (ITConfirmation.Attachment_Name != null && ITConfirmation.Attachment_Name != "")
                        {
                            A00ITCONFIRMATION.ATTACHMENT = ITConfirmation.Attachment_Name;
                        }
                        //22-Sept-2021 change end

                        _pdDBContext.Entry(A00ITCONFIRMATION).State = EntityState.Added;
                        _pdDBContext.SaveChanges();
                    }
                    result = 1;
                    #endregion
                }
                else if (btnText == "SUBMIT")
                {
                    #region Submit
                    if (A00ITCONFIRMATIONUpdate != null)
                    {
                        A00ITCONFIRMATIONUpdate.A00ALLOCATIONID = AllocatonDetails.ID;
                        A00ITCONFIRMATIONUpdate.MAINPIC_ID = (short)AllocatonDetails.MainPIC;
                        A00ITCONFIRMATIONUpdate.A00ID = ITConfirmation.A00DTLTBID;
                        A00ITCONFIRMATIONUpdate.SYKIID = ITConfirmation.SYKIID;
                        A00ITCONFIRMATIONUpdate.PROJECTCODE = ITConfirmation.ProjectCode_Date;
                        A00ITCONFIRMATIONUpdate.PROJECTDATE = Convert.ToDateTime(ITConfirmation.ProjectDate);
                        A00ITCONFIRMATIONUpdate.PROJECTNAME = ITConfirmation.ProjectName;
                        A00ITCONFIRMATIONUpdate.IMPACTEDOPERATIONS = ITConfirmation.ImpactedOperation;
                        A00ITCONFIRMATIONUpdate.USERPL = ITConfirmation.UserPL;
                        A00ITCONFIRMATIONUpdate.USERDEPT = ITConfirmation.UserDept;
                        A00ITCONFIRMATIONUpdate.ITPL = ITConfirmation.ITPL;
                        A00ITCONFIRMATIONUpdate.ITDEPT = ITConfirmation.ITDept;
                        A00ITCONFIRMATIONUpdate.SAA = ITConfirmation.SAA;
                        A00ITCONFIRMATIONUpdate.LWEA = ITConfirmation.LWEA;
                        A00ITCONFIRMATIONUpdate.DEVELOPMENT = ITConfirmation.Development;
                        A00ITCONFIRMATIONUpdate.LICENSEREQUIRED = ITConfirmation.LicenseRequired;
                        A00ITCONFIRMATIONUpdate.LRIFYES = ITConfirmation.OEM;
                        A00ITCONFIRMATIONUpdate.IREQUIREMENT = ITConfirmation.InfrastructureRequirement;
                        A00ITCONFIRMATIONUpdate.IRIFYES = ITConfirmation.IR;
                        A00ITCONFIRMATIONUpdate.TONETIMECOST = ITConfirmation.OTC;
                        A00ITCONFIRMATIONUpdate.TRECURRINGCOST = ITConfirmation.RC;
                        A00ITCONFIRMATIONUpdate.TTIMELINE = ITConfirmation.TTimeLine;
                        A00ITCONFIRMATIONUpdate.APPLICATION = ITConfirmation.Application;
                        A00ITCONFIRMATIONUpdate.NOOFSCREEN = ITConfirmation.NOFS;
                        A00ITCONFIRMATIONUpdate.HSDMCR = ITConfirmation.HSDM;
                        A00ITCONFIRMATIONUpdate.U0TARGETDATE = ITConfirmation.UOTarget;
                        A00ITCONFIRMATIONUpdate.REMARKS = ITConfirmation.Remarks;
                        A00ITCONFIRMATIONUpdate.ACTIVE = 1;
                        A00ITCONFIRMATIONUpdate.AppTeamSTATUS = 1;
                        //A00ITCONFIRMATIONUpdate.INFRATEAMSTATUS = 1;
                        //A00ITCONFIRMATIONUpdate.ITCONFIRMATIONTOAPPTEAM = ;
                        //A00ITCONFIRMATIONUpdate.ITCONFIRMATIONTOINFRATEAM = AllocatonDetails.INFRASTRUCTUREPIC;
                        A00ITCONFIRMATIONUpdate.ITCONFIRMATIONFILLEDBY = userId;
                        A00ITCONFIRMATIONUpdate.ITCONFIRMATIONFILLEDON = DateTime.Now;
                        A00ITCONFIRMATIONUpdate.LSTMODBYID = userId;
                        A00ITCONFIRMATIONUpdate.LASTMODDATE = DateTime.Now;

                        //22-Sept-2021 change start
                        if (ITConfirmation.Attachment_Name != null && ITConfirmation.Attachment_Name != "")
                        {
                            A00ITCONFIRMATIONUpdate.ATTACHMENT = ITConfirmation.Attachment_Name;
                        }
                        //22-Sept-2021 change end

                        _pdDBContext.Entry(A00ITCONFIRMATIONUpdate).State = EntityState.Modified;
                        _pdDBContext.SaveChanges();

                        A00ITCONFAPPROVALHISTORY.A00ID = ITConfirmation.A00DTLTBID;
                        A00ITCONFAPPROVALHISTORY.SYKIID = ITConfirmation.SYKIID;
                        A00ITCONFAPPROVALHISTORY.A00ITCONFIRMATIONID = A00ITCONFIRMATIONUpdate.ID;
                        A00ITCONFAPPROVALHISTORY.ACTIONTYPE = 1;
                        A00ITCONFAPPROVALHISTORY.ITCONFIRMATIONSUBMITTEDTO = ITConfirmation.ITCONFIRMATIONSUBMITTEDTO;
                        A00ITCONFAPPROVALHISTORY.STATUS = (short)(ITConfirmation.Status);
                        A00ITCONFAPPROVALHISTORY.REMARKS = ITConfirmation.Remarks;
                        A00ITCONFAPPROVALHISTORY.ACTIONBY = userId;
                        A00ITCONFAPPROVALHISTORY.ACTIONON = DateTime.Now;
                        A00ITCONFAPPROVALHISTORY.ACTIVE = 1;
                        A00ITCONFAPPROVALHISTORY.USERTYPE = (short)ITConfirmation.UserType;
                        A00ITCONFAPPROVALHISTORY.ITCONFSUBMITTEDTOUSERTYPE = (short)ITConfirmation.ITCONFSUBMITTEDTOUSERTYPE;
                        A00ITCONFAPPROVALHISTORY.ITCONFIRMATIONSUBMITTEDSTATUS = (short)ITConfirmation.ITCONFSUBMITTEDTOSTATUS;

                        _pdDBContext.Entry(A00ITCONFAPPROVALHISTORY).State = EntityState.Added;
                        _pdDBContext.SaveChanges();
                    }
                    else
                    {
                        A00ITCONFIRMATION.A00ID = ITConfirmation.A00DTLTBID;
                        A00ITCONFIRMATION.SYKIID = ITConfirmation.SYKIID;
                        A00ITCONFIRMATION.A00ALLOCATIONID = AllocatonDetails.ID;
                        A00ITCONFIRMATION.MAINPIC_ID = (short)AllocatonDetails.MainPIC;
                        A00ITCONFIRMATION.PROJECTCODE = ITConfirmation.ProjectCode_Date;
                        A00ITCONFIRMATION.PROJECTDATE = Convert.ToDateTime(ITConfirmation.ProjectDate);
                        A00ITCONFIRMATION.PROJECTNAME = ITConfirmation.ProjectName;
                        A00ITCONFIRMATION.IMPACTEDOPERATIONS = ITConfirmation.ImpactedOperation;
                        A00ITCONFIRMATION.USERPL = ITConfirmation.UserPL;
                        A00ITCONFIRMATION.USERDEPT = ITConfirmation.UserDept;
                        A00ITCONFIRMATION.ITPL = ITConfirmation.ITPL;
                        A00ITCONFIRMATION.ITDEPT = ITConfirmation.ITDept;
                        A00ITCONFIRMATION.SAA = ITConfirmation.SAA;
                        A00ITCONFIRMATION.LWEA = ITConfirmation.LWEA;
                        A00ITCONFIRMATION.DEVELOPMENT = ITConfirmation.Development;
                        A00ITCONFIRMATION.LICENSEREQUIRED = ITConfirmation.LicenseRequired;
                        A00ITCONFIRMATION.LRIFYES = ITConfirmation.OEM;
                        A00ITCONFIRMATION.IREQUIREMENT = ITConfirmation.InfrastructureRequirement;
                        A00ITCONFIRMATION.IRIFYES = ITConfirmation.IR;
                        A00ITCONFIRMATION.TONETIMECOST = ITConfirmation.OTC;
                        A00ITCONFIRMATION.TRECURRINGCOST = ITConfirmation.RC;
                        A00ITCONFIRMATION.TTIMELINE = ITConfirmation.TTimeLine;
                        A00ITCONFIRMATION.APPLICATION = ITConfirmation.Application;
                        A00ITCONFIRMATION.NOOFSCREEN = ITConfirmation.NOFS;
                        A00ITCONFIRMATION.HSDMCR = ITConfirmation.HSDM;
                        A00ITCONFIRMATION.U0TARGETDATE = ITConfirmation.UOTarget;
                        A00ITCONFIRMATION.REMARKS = ITConfirmation.Remarks;
                        A00ITCONFIRMATION.ACTIVE = 1;
                        A00ITCONFIRMATION.AppTeamSTATUS = 1;
                        //A00ITCONFIRMATION.INFRATEAMSTATUS = 1;
                        //A00ITCONFIRMATION.ITCONFIRMATIONTOAPPTEAM = ;
                        //A00ITCONFIRMATION.ITCONFIRMATIONTOINFRATEAM = AllocatonDetails.INFRASTRUCTUREPIC;
                        A00ITCONFIRMATION.ITCONFIRMATIONFILLEDBY = userId;
                        A00ITCONFIRMATION.ITCONFIRMATIONFILLEDON = DateTime.Now;
                        A00ITCONFIRMATION.ID = Convert.ToInt64(ITConfirmation.ITCONFIRMATIONID);

                        //22-Sept-2021 change start
                        if (ITConfirmation.Attachment_Name != null && ITConfirmation.Attachment_Name != "")
                        {
                            A00ITCONFIRMATION.ATTACHMENT = ITConfirmation.Attachment_Name;
                        }
                        //22-Sept-2021 change end

                        _pdDBContext.Entry(A00ITCONFIRMATION).State = EntityState.Added;
                        _pdDBContext.SaveChanges();

                        var A00ITCONFIRMATIONID = _pdDBContext.A00ITCONFIRMATION.Where(x => x.A00ID == ITConfirmation.A00DTLTBID && x.SYKIID == ITConfirmation.SYKIID && x.ACTIVE == 1).Select(x => x.ID).FirstOrDefault();

                        A00ITCONFAPPROVALHISTORY.A00ID = ITConfirmation.A00DTLTBID;
                        A00ITCONFAPPROVALHISTORY.SYKIID = ITConfirmation.SYKIID;
                        A00ITCONFAPPROVALHISTORY.A00ITCONFIRMATIONID = A00ITCONFIRMATIONID;
                        A00ITCONFAPPROVALHISTORY.ACTIONTYPE = 1;
                        A00ITCONFAPPROVALHISTORY.ITCONFIRMATIONSUBMITTEDTO = ITConfirmation.ITCONFIRMATIONSUBMITTEDTO;
                        A00ITCONFAPPROVALHISTORY.STATUS = (short)(ITConfirmation.Status);
                        A00ITCONFAPPROVALHISTORY.REMARKS = ITConfirmation.Remarks;
                        A00ITCONFAPPROVALHISTORY.ACTIONBY = userId;
                        A00ITCONFAPPROVALHISTORY.ACTIONON = DateTime.Now;
                        A00ITCONFAPPROVALHISTORY.ACTIVE = 1;
                        A00ITCONFAPPROVALHISTORY.USERTYPE = (short)ITConfirmation.UserType;
                        A00ITCONFAPPROVALHISTORY.ITCONFSUBMITTEDTOUSERTYPE = ITConfirmation.ITCONFSUBMITTEDTOUSERTYPE;
                        A00ITCONFAPPROVALHISTORY.ITCONFIRMATIONSUBMITTEDSTATUS = ITConfirmation.ITCONFSUBMITTEDTOSTATUS;

                        _pdDBContext.Entry(A00ITCONFAPPROVALHISTORY).State = EntityState.Added;
                        _pdDBContext.SaveChanges();
                    }
                    result = 2;
                    #endregion
                }
                else if (btnText == "APPROVE")
                {
                    #region Approve
                    if (A00ITCONFIRMATIONUpdate != null)
                    {
                        A00ITCONFIRMATIONUpdate.IMPACTEDOPERATIONS = ITConfirmation.ImpactedOperation;
                        A00ITCONFIRMATIONUpdate.DEVELOPMENT = ITConfirmation.Development;
                        A00ITCONFIRMATIONUpdate.SAA = ITConfirmation.SAA;
                        A00ITCONFIRMATIONUpdate.LWEA = ITConfirmation.LWEA;
                        A00ITCONFIRMATIONUpdate.IREQUIREMENT = ITConfirmation.InfrastructureRequirement;
                        A00ITCONFIRMATIONUpdate.IRIFYES = ITConfirmation.IR;
                        A00ITCONFIRMATIONUpdate.LICENSEREQUIRED = ITConfirmation.LicenseRequired;
                        A00ITCONFIRMATIONUpdate.LRIFYES = ITConfirmation.OEM;
                        A00ITCONFIRMATIONUpdate.TONETIMECOST = ITConfirmation.OTC;
                        A00ITCONFIRMATIONUpdate.TRECURRINGCOST = ITConfirmation.RC;
                        A00ITCONFIRMATIONUpdate.TTIMELINE = ITConfirmation.TTimeLine;
                        A00ITCONFIRMATIONUpdate.U0TARGETDATE = ITConfirmation.UOTarget;
                        if (ITConfirmation.IsOHLoggedIn)
                        {
                            A00ITCONFIRMATIONUpdate.APPLICATION = ITConfirmation.Application;
                            A00ITCONFIRMATIONUpdate.NOOFSCREEN = ITConfirmation.NOFS;
                            A00ITCONFIRMATIONUpdate.HSDMCR = ITConfirmation.HSDM;
                        }

                        A00ITCONFIRMATIONUpdate.AppTeamSTATUS = (short)ITConfirmation.Status;
                        //A00ITCONFIRMATIONUpdate.ITCONFIRMATIONFILLEDBY = userId;
                        //A00ITCONFIRMATIONUpdate.ITCONFIRMATIONFILLEDON = DateTime.Now;
                        A00ITCONFIRMATIONUpdate.LSTMODBYID = userId;
                        A00ITCONFIRMATIONUpdate.LASTMODDATE = DateTime.Now;

                        _pdDBContext.Entry(A00ITCONFIRMATIONUpdate).State = EntityState.Modified;
                        _pdDBContext.SaveChanges();

                        A00ITCONFAPPROVALHISTORY.A00ID = ITConfirmation.A00DTLTBID;
                        A00ITCONFAPPROVALHISTORY.SYKIID = ITConfirmation.SYKIID;
                        A00ITCONFAPPROVALHISTORY.A00ITCONFIRMATIONID = A00ITCONFIRMATIONUpdate.ID;
                        A00ITCONFAPPROVALHISTORY.ACTIONTYPE = 1;
                        A00ITCONFAPPROVALHISTORY.ITCONFIRMATIONSUBMITTEDTO = ITConfirmation.ITCONFIRMATIONSUBMITTEDTO;
                        A00ITCONFAPPROVALHISTORY.ITCONFSUBMITTEDTOUSERTYPE = ITConfirmation.ITCONFSUBMITTEDTOUSERTYPE;
                        A00ITCONFAPPROVALHISTORY.ITCONFIRMATIONSUBMITTEDSTATUS = ITConfirmation.ITCONFSUBMITTEDTOSTATUS;
                        A00ITCONFAPPROVALHISTORY.STATUS = (short)(ITConfirmation.Status);
                        A00ITCONFAPPROVALHISTORY.REMARKS = ITConfirmation.ApprovalRemarks;
                        A00ITCONFAPPROVALHISTORY.ACTIONBY = userId;
                        A00ITCONFAPPROVALHISTORY.USERTYPE = (short)ITConfirmation.UserType;
                        A00ITCONFAPPROVALHISTORY.ACTIONON = DateTime.Now;
                        A00ITCONFAPPROVALHISTORY.ACTIVE = 1;

                        _pdDBContext.Entry(A00ITCONFAPPROVALHISTORY).State = EntityState.Added;
                        _pdDBContext.SaveChanges();

                        result = 3;
                    }
                    #endregion
                }
                else if (btnText == "SEND BACK")
                {
                    #region Send back
                    if (A00ITCONFIRMATIONUpdate != null)
                    {
                        A00ITCONFIRMATIONUpdate.IMPACTEDOPERATIONS = ITConfirmation.ImpactedOperation;
                        A00ITCONFIRMATIONUpdate.DEVELOPMENT = ITConfirmation.Development;
                        A00ITCONFIRMATIONUpdate.SAA = ITConfirmation.SAA;
                        A00ITCONFIRMATIONUpdate.LWEA = ITConfirmation.LWEA;
                        A00ITCONFIRMATIONUpdate.IREQUIREMENT = ITConfirmation.InfrastructureRequirement;
                        A00ITCONFIRMATIONUpdate.IRIFYES = ITConfirmation.IR;
                        A00ITCONFIRMATIONUpdate.LICENSEREQUIRED = ITConfirmation.LicenseRequired;
                        A00ITCONFIRMATIONUpdate.LRIFYES = ITConfirmation.OEM;
                        A00ITCONFIRMATIONUpdate.TONETIMECOST = ITConfirmation.OTC;
                        A00ITCONFIRMATIONUpdate.TRECURRINGCOST = ITConfirmation.RC;
                        A00ITCONFIRMATIONUpdate.TTIMELINE = ITConfirmation.TTimeLine;
                        A00ITCONFIRMATIONUpdate.U0TARGETDATE = ITConfirmation.UOTarget;
                        if (ITConfirmation.IsOHLoggedIn)
                        {
                            A00ITCONFIRMATIONUpdate.APPLICATION = ITConfirmation.Application;
                            A00ITCONFIRMATIONUpdate.NOOFSCREEN = ITConfirmation.NOFS;
                            A00ITCONFIRMATIONUpdate.HSDMCR = ITConfirmation.HSDM;
                        }

                        A00ITCONFIRMATIONUpdate.AppTeamSTATUS = (short)ITConfirmation.Status;
                        //A00ITCONFIRMATIONUpdate.ITCONFIRMATIONFILLEDBY = userId;
                        //A00ITCONFIRMATIONUpdate.ITCONFIRMATIONFILLEDON = DateTime.Now;
                        A00ITCONFIRMATIONUpdate.LSTMODBYID = userId;
                        A00ITCONFIRMATIONUpdate.LASTMODDATE = DateTime.Now;

                        _pdDBContext.Entry(A00ITCONFIRMATIONUpdate).State = EntityState.Modified;
                        _pdDBContext.SaveChanges();

                        A00ITCONFAPPROVALHISTORY.A00ID = ITConfirmation.A00DTLTBID;
                        A00ITCONFAPPROVALHISTORY.SYKIID = ITConfirmation.SYKIID;
                        A00ITCONFAPPROVALHISTORY.A00ITCONFIRMATIONID = A00ITCONFIRMATIONUpdate.ID;
                        A00ITCONFAPPROVALHISTORY.ACTIONTYPE = 2;
                        A00ITCONFAPPROVALHISTORY.ITCONFIRMATIONSUBMITTEDTO = ITConfirmation.ITCONFIRMATIONSUBMITTEDTO;
                        A00ITCONFAPPROVALHISTORY.ITCONFSUBMITTEDTOUSERTYPE = ITConfirmation.ITCONFSUBMITTEDTOUSERTYPE;
                        A00ITCONFAPPROVALHISTORY.ITCONFIRMATIONSUBMITTEDSTATUS = ITConfirmation.ITCONFSUBMITTEDTOSTATUS;
                        A00ITCONFAPPROVALHISTORY.STATUS = (short)(ITConfirmation.Status);
                        A00ITCONFAPPROVALHISTORY.REMARKS = ITConfirmation.ApprovalRemarks;
                        A00ITCONFAPPROVALHISTORY.ACTIONBY = userId;
                        A00ITCONFAPPROVALHISTORY.USERTYPE = (short)ITConfirmation.UserType;
                        A00ITCONFAPPROVALHISTORY.ACTIONON = DateTime.Now;
                        A00ITCONFAPPROVALHISTORY.ACTIVE = 1;

                        _pdDBContext.Entry(A00ITCONFAPPROVALHISTORY).State = EntityState.Added;
                        _pdDBContext.SaveChanges();

                        result = 4;
                    }
                    #endregion
                }
                _pdDBContext.Database.CommitTransaction();
                return result;
            }
            catch (Exception)
            {
                _pdDBContext.Database.RollbackTransaction();
                throw;
            }
        }
        //22-Sept-2021 change end

        public ITConfirmationVM GetA00ItConfirmationDetails(long? A00DTLTBID, decimal SYKI)
        {
            try
            {
                ITConfirmationVM ITConfirmationVM = new ITConfirmationVM();
                var AllocationDetails = _pdDBContext.A00_ALLOCATION.Where(x => x.A00DTLTBID == A00DTLTBID && x.SYKIID == SYKI && x.ACTIVE == 1).FirstOrDefault();
                var ITConfirmationDetails = _pdDBContext.A00ITCONFIRMATION.Where(x => x.A00ID == A00DTLTBID && x.SYKIID == SYKI && x.ACTIVE == 1).FirstOrDefault();
                if (AllocationDetails != null && ITConfirmationDetails != null)
                {
                    ITConfirmationVM.ApplicationPIC = AllocationDetails != null ? AllocationDetails.APPLICATIONPIC : null;
                    ITConfirmationVM.InfraStructurePIC = AllocationDetails != null ? AllocationDetails.INFRASTRUCTUREPIC : null;
                    ITConfirmationVM.ITCONFIRMATIONID = ITConfirmationDetails.ID;
                    ITConfirmationVM.A00ALLOCATIONID = ITConfirmationDetails.A00ALLOCATIONID;
                    ITConfirmationVM.MAINPIC_ID = ITConfirmationDetails.MAINPIC_ID;
                    ITConfirmationVM.ProjectCode = ITConfirmationDetails.PROJECTCODE;
                    ITConfirmationVM.ProjectCode_Date = ITConfirmationDetails.PROJECTCODE;
                    ITConfirmationVM.ProjectDate = ITConfirmationDetails.PROJECTDATE.ToString();
                    ITConfirmationVM.ProjectName = ITConfirmationDetails.PROJECTNAME;
                    ITConfirmationVM.ImpactedOperation = ITConfirmationDetails.IMPACTEDOPERATIONS;
                    ITConfirmationVM.UserPL = ITConfirmationDetails.USERPL;
                    ITConfirmationVM.UserDept = ITConfirmationDetails.USERDEPT;
                    ITConfirmationVM.ITPL = ITConfirmationDetails.ITPL;
                    ITConfirmationVM.ITDept = ITConfirmationDetails.ITDEPT;
                    ITConfirmationVM.SAA = ITConfirmationDetails.SAA;
                    ITConfirmationVM.LWEA = ITConfirmationDetails.LWEA;
                    ITConfirmationVM.Development = ITConfirmationDetails.DEVELOPMENT;
                    ITConfirmationVM.LicenseRequired = ITConfirmationDetails.LICENSEREQUIRED;
                    ITConfirmationVM.InfrastructureRequirement = ITConfirmationDetails.IREQUIREMENT;
                    ITConfirmationVM.OEM = ITConfirmationDetails.LRIFYES;
                    ITConfirmationVM.IR = ITConfirmationDetails.IRIFYES;
                    ITConfirmationVM.OTC = ITConfirmationDetails.TONETIMECOST;
                    ITConfirmationVM.RC = ITConfirmationDetails.TRECURRINGCOST;
                    ITConfirmationVM.TTimeLine = ITConfirmationDetails.TTIMELINE;
                    ITConfirmationVM.Application = ITConfirmationDetails.APPLICATION;
                    ITConfirmationVM.NOFS = ITConfirmationDetails.NOOFSCREEN;
                    ITConfirmationVM.HSDM = ITConfirmationDetails.HSDMCR;
                    ITConfirmationVM.UOTarget = ITConfirmationDetails.U0TARGETDATE;
                    ITConfirmationVM.Remarks = ITConfirmationDetails.REMARKS;
                    ITConfirmationVM.Status = ITConfirmationDetails.AppTeamSTATUS;
                    //22-Sept-2021 change start
                    ITConfirmationVM.ImagePath = ITConfirmationDetails.ATTACHMENT;
                    //22-Sept-2021 change end
                }
                return ITConfirmationVM;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        public bool IsAllocationExist(long? A00DTLTBID, decimal? SYKI)
        {
            var IsAllocationExist = _pdDBContext.A00_ALLOCATION.FirstOrDefault(x => x.A00DTLTBID == A00DTLTBID && x.SYKIID == SYKI) == null ? false : true;
            return IsAllocationExist;
        }

        public A00RaisedDetails A00RaisedDetails(long? A00DTLTBID, decimal? SYKI)
        {
            var A00AddedBy = _pdDBContext.A00DTLTB.Where(x => x.A00DTLTBID == A00DTLTBID && x.SYKIID == SYKI).Select(x => x.ADDEDBY).FirstOrDefault();
            var result = _pdDBContext.ADEMPLOYEE.Where(X => X.ADEMPCODE == A00AddedBy).Select(X => new A00RaisedDetails
            {
                EmpCode = X.ADEMPCODE.ToString(),
                Name = X.FIRSTNAME + " " + X.LASTNAME,
            }).FirstOrDefault();
            return result;
        }

        public int GetA00AllocationSNo(long? A00DTLTBID, decimal? SYKI)
        {
            //var Id = _pdDBContext.A00_ALLOCATION.Where(x => x.A00DTLTBID == A00DTLTBID && x.SYKIID == SYKI).Select(x => x.ID).FirstOrDefault();
            //Above line commented by Eshant and removed SYKIID and get Latest ID for A00Allocation to Update A00 Record
            var Id = _pdDBContext.A00_ALLOCATION.Where(x => x.A00DTLTBID == A00DTLTBID).OrderByDescending(x => x.ID).Select(x => x.ID).FirstOrDefault();
            return (int)Id;
        }

        public long GetA00ApplicationPIC(long? A00DTLTBID, decimal SYKI)
        {
            var AllocatonDetails = _pdDBContext.A00_ALLOCATION.Where(x => x.A00DTLTBID == A00DTLTBID && x.SYKIID == SYKI && x.ACTIVE == 1).FirstOrDefault();
            long ApplicationPIC = Convert.ToInt64(AllocatonDetails.APPLICATIONPIC);
            return ApplicationPIC;
        }
        public long GetA00InfraStructurePIC(long? A00DTLTBID, decimal SYKI)
        {
            var AllocatonDetails = _pdDBContext.A00_ALLOCATION.Where(x => x.A00DTLTBID == A00DTLTBID && x.SYKIID == SYKI && x.ACTIVE == 1).FirstOrDefault();
            long InfraStructurePIC = Convert.ToInt64(AllocatonDetails.INFRASTRUCTUREPIC);
            return InfraStructurePIC;
        }

        public ITConfirmationApprovalHistoryVM GetA00ItConfirmationHistoryDetails(decimal A00ItConfirmationID)
        {
            try
            {
                ITConfirmationApprovalHistoryVM ITConfirmationApprovalHistory = new ITConfirmationApprovalHistoryVM();
                var ITConfirmationApprHistoryDetails = _pdDBContext.A00ITCONFAPPROVALHISTORY.Where(x => x.A00ITCONFIRMATIONID == A00ItConfirmationID && x.ACTIVE == 1).OrderByDescending(x => x.ID).FirstOrDefault();
                ITConfirmationApprovalHistory.A00ID = ITConfirmationApprHistoryDetails.A00ID;
                ITConfirmationApprovalHistory.SYKIID = ITConfirmationApprHistoryDetails.SYKIID;

                ITConfirmationApprovalHistory.ActionType = ITConfirmationApprHistoryDetails.ACTIONTYPE;
                ITConfirmationApprovalHistory.UserType = ITConfirmationApprHistoryDetails.USERTYPE;
                ITConfirmationApprovalHistory.ITConfirmationSubmittedTo = ITConfirmationApprHistoryDetails.ITCONFIRMATIONSUBMITTEDTO;
                ITConfirmationApprovalHistory.ITCONFSUBMITTEDTOUSERTYPE = ITConfirmationApprHistoryDetails.ITCONFSUBMITTEDTOUSERTYPE;
                ITConfirmationApprovalHistory.Status = ITConfirmationApprHistoryDetails.STATUS;
                ITConfirmationApprovalHistory.ITCONFSUBMITTEDTOSTATUS = ITConfirmationApprHistoryDetails.ITCONFIRMATIONSUBMITTEDSTATUS;

                ITConfirmationApprovalHistory.ActionBy = ITConfirmationApprHistoryDetails.ACTIONBY;
                ITConfirmationApprovalHistory.ActionByName = _pdDBContext.ADEMPLOYEE.Where(X => X.ADEMPCODE == ITConfirmationApprHistoryDetails.ACTIONBY).Select(X => X.FIRSTNAME + " " + X.LASTNAME).FirstOrDefault();
                ITConfirmationApprovalHistory.ActionOn = ITConfirmationApprHistoryDetails.ACTIONON;
                ITConfirmationApprovalHistory.Designation = GetDesignationForITConfirmation(ITConfirmationApprHistoryDetails.STATUS);
                ITConfirmationApprovalHistory.PICType = ITConfirmationApprHistoryDetails.USERTYPE == 1 ? "App PIC" : "Infra PIC";
                ITConfirmationApprovalHistory.Remarks = ITConfirmationApprHistoryDetails.REMARKS;
                return ITConfirmationApprovalHistory;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string GetDesignationForITConfirmation(short? Status)
        {
            var designation = "";
            switch (Status)
            {
                case 1:
                    designation = "Main PIC";
                    break;
                case 2:
                    designation = "Dept. Head";
                    break;
                case 3:
                    designation = "Dept. Head";
                    break;
                case 4:
                    designation = "Coordinator";
                    break;
                case 5:
                    designation = "Div. Head";
                    break;
                case 6:
                    designation = "Div. Head";
                    break;
                case 7:
                    designation = "Ex. Coordinator";
                    break;
                case 8:
                    designation = "Operating Head";
                    break;
                default:
                    designation = "Main PIC";
                    break;
            }
            return designation;
        }
        public List<ITConfirmationApprovalHistoryListVM> GetA00ItConfirmationHistory(decimal A00ItConfirmationID)
        {
            try
            {
                var ITConfirmationApprHistoryList = _pdDBContext.A00ITCONFAPPROVALHISTORY.AsEnumerable().OrderBy(x => x.ID).Where(x => x.A00ITCONFIRMATIONID == A00ItConfirmationID && x.ACTIVE == 1)
                    .Select(x => new ITConfirmationApprovalHistoryListVM
                    {
                        HistoryID = x.ID,
                        A00ITCONFIRMATIONID = x.A00ITCONFIRMATIONID,
                        A00ID = x.A00ID,
                        SYKIID = x.SYKIID,
                        ACTIONTYPE = x.ACTIONTYPE,
                        STATUS = x.STATUS,
                        REMARKS = x.REMARKS,
                        EmpName = _pdDBContext.ADEMPLOYEE.Where(X => X.ADEMPCODE == x.ACTIONBY).Select(X => X.FIRSTNAME + " " + X.LASTNAME).FirstOrDefault(),
                        ACTIONBY = x.ACTIONBY,
                        ACTIONON = x.ACTIONON,
                        ACTIVE = x.ACTIVE,
                        USERTYPE = x.USERTYPE,
                        ITCONFIRMATIONSUBMITTEDTO = x.ITCONFIRMATIONSUBMITTEDTO,
                        ITCONFSUBMITTEDTOUSERTYPE = x.ITCONFSUBMITTEDTOUSERTYPE,
                        ITCONFSUBMITTEDTOSTATUS = x.ITCONFIRMATIONSUBMITTEDSTATUS,
                        Designation = GetDesignationForITConfirmation(x.STATUS),
                        PICType = x.USERTYPE == 1 ? "App" : "Infra"
                    }).ToList();

                return ITConfirmationApprHistoryList;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public int SaveA00ProjectStatus(string ProjectStage, string ItConfrApprovedOn, int ProjectStatus, string Remark, string fileName, long SYKI, int userId, long A00DTLTBID, DateTime ProjectUpdateDate)
        {
            var A00PROJECTSTATUS = new A00PROJECTSTATUSUPDATE();
            A00PROJECTSTATUS.A00ID = A00DTLTBID;
            A00PROJECTSTATUS.SYKIID = SYKI;
            A00PROJECTSTATUS.PROJECTSTAGE = ProjectStage;
            A00PROJECTSTATUS.ITCONFAPPROVEDON = ItConfrApprovedOn;
            A00PROJECTSTATUS.PROJECTSTATUS = (short)ProjectStatus;
            A00PROJECTSTATUS.REMARKS = Remark;
            A00PROJECTSTATUS.ATTACHMENTNOTE = fileName;
            A00PROJECTSTATUS.ACTIONBY = userId;
            A00PROJECTSTATUS.ACTIONON = ProjectUpdateDate;
            A00PROJECTSTATUS.CREATIONDATE = DateTime.Now;
            A00PROJECTSTATUS.ACTIVE = 1;

            _pdDBContext.Entry(A00PROJECTSTATUS).State = EntityState.Added;
            _pdDBContext.SaveChanges();
            return 1;
        }
        private DateTime convertDateToDDMM(string input)
        {
            char delimiter = input.Contains("/") ? '/' : '-';
            string[] splittedDate = input.Trim().ToString().Split(delimiter);
            string ddmmDate = splittedDate[0].PadLeft(0, '2').ToString() + "/" + splittedDate[1].PadLeft(0, '2').ToString() + "/" + splittedDate[2].PadLeft(0, '2').ToString();
            return DateTime.ParseExact(ddmmDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
        public int A00ActivitySaveData(A00AcitivtyVM a00AcitivtyVM)
        {
            var A00ACTIVITY = new A00ACTIVITY();
            A00ACTIVITY = _pdDBContext.A00ACTIVITY.Find(a00AcitivtyVM.ActivityID);
            if (A00ACTIVITY != null)
            {
                A00ACTIVITY.ACTIVITYNAME = a00AcitivtyVM.ActivityName;
                A00ACTIVITY.ACTIVITYDETAILS = a00AcitivtyVM.ActivityDetails;
                A00ACTIVITY.ACTIVITYSTARTDATE = convertDateToDDMM(a00AcitivtyVM.ActivityStartDate);
                A00ACTIVITY.ACTIVITYENDDATE = convertDateToDDMM(a00AcitivtyVM.ActivityEndDate);
                A00ACTIVITY.STATUS = (short)a00AcitivtyVM.Status;
                A00ACTIVITY.ACTIVITYSCHEDULE = 0;
                A00ACTIVITY.ACTIONBY = a00AcitivtyVM.ActionBy;
                A00ACTIVITY.ACTIONON = DateTime.Now;
                A00ACTIVITY.ACTIVE = 1;
                A00ACTIVITY.SYKID = a00AcitivtyVM.SYKID;
                A00ACTIVITY.A00DTLTBID = a00AcitivtyVM.A00DTLTBID;
                A00ACTIVITY.DEPARTMENTID = a00AcitivtyVM.DEPARTMENTID;
                A00ACTIVITY.OPERATIONID = a00AcitivtyVM.OPERATIONID;
                A00ACTIVITY.DIVISIONID = a00AcitivtyVM.DIVISIONID;
                A00ACTIVITY.SECTIONID = a00AcitivtyVM.SECTIONID;

                _pdDBContext.Entry(A00ACTIVITY).State = EntityState.Modified;
                _pdDBContext.SaveChanges();
            }
            else
            {
                A00ACTIVITY = new A00ACTIVITY();
                A00ACTIVITY.ACTIVITYNAME = a00AcitivtyVM.ActivityName;
                A00ACTIVITY.ACTIVITYDETAILS = a00AcitivtyVM.ActivityDetails;
                A00ACTIVITY.ACTIVITYSTARTDATE = convertDateToDDMM(a00AcitivtyVM.ActivityStartDate);
                A00ACTIVITY.ACTIVITYENDDATE = convertDateToDDMM(a00AcitivtyVM.ActivityEndDate);
                A00ACTIVITY.STATUS = (short)a00AcitivtyVM.Status;
                A00ACTIVITY.ACTIVITYSCHEDULE = 0;
                A00ACTIVITY.ACTIONBY = a00AcitivtyVM.ActionBy;
                A00ACTIVITY.ACTIONON = DateTime.Now;
                A00ACTIVITY.ACTIVE = 1;
                A00ACTIVITY.SYKID = a00AcitivtyVM.SYKID;
                A00ACTIVITY.A00DTLTBID = a00AcitivtyVM.A00DTLTBID;
                A00ACTIVITY.DEPARTMENTID = a00AcitivtyVM.DEPARTMENTID;
                A00ACTIVITY.OPERATIONID = a00AcitivtyVM.OPERATIONID;
                A00ACTIVITY.DIVISIONID = a00AcitivtyVM.DIVISIONID;
                A00ACTIVITY.SECTIONID = a00AcitivtyVM.SECTIONID;

                _pdDBContext.Entry(A00ACTIVITY).State = EntityState.Added;
                _pdDBContext.SaveChanges();
            }
            return (int)A00ACTIVITY.STATUS;
        }
        public A00AcitivtyVM A00ActivityDetails(int Id)
        {
            A00AcitivtyVM A00AcitivtyVM = new A00AcitivtyVM();
            var AcitivtyVM = (from Activity in _pdDBContext.A00ACTIVITY.Where(x => x.ID == Id && x.ACTIVE == 1)
                              from ActivityH in _pdDBContext.A00ACTIVITYHISTORY.Where(x => x.A00ACTIVITYID == Activity.ID).OrderByDescending(x => x.ID).DefaultIfEmpty()
                              select new
                              {
                                  ActivityID = Activity.ID,
                                  ActivityName = Activity.ACTIVITYNAME,
                                  ActivityDetails = Activity.ACTIVITYDETAILS,
                                  ActivityStartDate = Activity.ACTIVITYSTARTDATE,
                                  ActivityEndDate = Activity.ACTIVITYENDDATE,
                                  ActivitySchedule = (short)Activity.ACTIVITYSCHEDULE,
                                  Remark = ActivityH.REMARKS
                              }).ToList();
            if (AcitivtyVM != null)
            {
                A00AcitivtyVM = AcitivtyVM.Select(Activity => new A00AcitivtyVM
                {
                    ActivityID = Activity.ActivityID,
                    ActivityName = Activity.ActivityName,
                    ActivityDetails = Activity.ActivityDetails,
                    ActivityStartDate = Activity.ActivityStartDate.Value.ToString("dd/MM/yyyy"),
                    ActivityEndDate = Activity.ActivityEndDate.Value.ToString("dd/MM/yyyy"),
                    ActivitySchedule = Activity.ActivitySchedule,
                    Remark = Activity.Remark
                }).FirstOrDefault();
            }
            return A00AcitivtyVM;
        }
        public int A00ActivityUpdate(int A00ActivityID, int ActivitySchedule, string ActivityRemarks, int ActionBy)
        {
            try
            {
                _pdDBContext.Database.BeginTransaction();
                var A00ACTIVITYHISTORY = new A00ACTIVITYHISTORY();
                var A00ACTIVITY = new A00ACTIVITY();
                A00ACTIVITYHISTORY.A00ACTIVITYID = A00ActivityID;
                A00ACTIVITYHISTORY.REMARKS = ActivityRemarks;
                A00ACTIVITYHISTORY.ACTIVITYSCHEDULE = (short)ActivitySchedule;
                A00ACTIVITYHISTORY.ACTIONBY = ActionBy;
                A00ACTIVITYHISTORY.ACTIONON = DateTime.Now;
                A00ACTIVITYHISTORY.ACTIVE = 1;

                _pdDBContext.Entry(A00ACTIVITYHISTORY).State = EntityState.Added;
                _pdDBContext.SaveChanges();
                var tbl = _pdDBContext.A00ACTIVITY.Find((decimal)A00ActivityID);
                tbl.ACTIVITYSCHEDULE = (short)ActivitySchedule;
                _pdDBContext.Entry(tbl).State = EntityState.Modified;
                _pdDBContext.SaveChanges();
                _pdDBContext.Database.CommitTransaction();

                return 2;
            }
            catch (Exception)
            {
                _pdDBContext.Database.RollbackTransaction();
                throw;
            }
        }
        public A00DataViewModel GetA00ActivityList()
        {
            A00DataViewModel dvm = new A00DataViewModel();
            try
            {
                var ActivityList = _pdDBContext.A00ACTIVITY.Select(x => new
                {
                    ActivityID = x.ID,
                    ActivityName = x.ACTIVITYNAME,
                    ActivityDetails = x.ACTIVITYDETAILS,
                    StartDate = x.ACTIVITYSTARTDATE,
                    ActivityStartDate = x.ACTIVITYSTARTDATE,
                    ActivityEndDate = x.ACTIVITYENDDATE,
                    ActionBy = x.ACTIONBY,
                    Status = x.STATUS,
                    ActivitySchedule = x.ACTIVITYSCHEDULE,
                    SYKID = x.SYKID,
                    OperationId = x.OPERATIONID,
                    DepartmentId = x.DEPARTMENTID,
                    DivisionId = x.DIVISIONID,
                    SectionId = x.SECTIONID,
                    Remark = _pdDBContext.A00ACTIVITYHISTORY.Where(a => a.A00ACTIVITYID == x.ID).Select(h => new { h.REMARKS, h.ID, h.ACTIONON }).ToList(),
                    // EmpName = GetEmpName(x.ACTIONBY),
                }).ToList(); //Dinesh

                dvm._A00ActivityList = ActivityList.Select(x => new A00AcitivtyVM()
                {
                    ActivityID = x.ActivityID,
                    ActivityName = x.ActivityName,
                    ActivityDetails = x.ActivityDetails,
                    StartDate = x.StartDate,
                    ActivityStartDate = x.ActivityStartDate.Value.ToString("dd/MM/yyyy"),
                    ActivityEndDate = x.ActivityEndDate.Value.ToString("dd/MM/yyyy"),
                    ActionBy = x.ActionBy,
                    Status = x.Status,
                    ActivitySchedule = x.ActivitySchedule,
                    SYKID = x.SYKID,
                    OPERATIONID = x.OperationId,
                    DEPARTMENTID = x.DepartmentId,
                    DIVISIONID = x.DivisionId,
                    SECTIONID = x.SectionId,
                    Remark = x.Remark.OrderByDescending(h => h.ID).Select(h => h.REMARKS).FirstOrDefault(),
                    ActivityRemarkDate = x.Remark.OrderByDescending(h => h.ID).Select(h => h.ACTIONON).FirstOrDefault()
                    // EmpName = GetEmpName(x.ACTIONBY),
                }).ToList(); //Dinesh

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dvm;
        }
        public A00ProjectStateUpdate GetA00ProjectStateUpdate(long? A00DTLTBID, decimal SYKI)
        {
            try
            {
                A00ProjectStateUpdate a00ProjectStateUpdate = new A00ProjectStateUpdate();
                var ProjectStatusDetails = _pdDBContext.A00PROJECTSTATUSUPDATE.Where(x => x.A00ID == A00DTLTBID && x.SYKIID == SYKI && x.ACTIVE == 1).OrderByDescending(x => x.ID).FirstOrDefault();
                //Commented by Eshant on 14-Jul-2022: change order by ID instead of Action On, as Action on can have same date for multiple stages. ID can be used for last action
                //var ProjectStatusDetails = _pdDBContext.A00PROJECTSTATUSUPDATE.Where(x => x.A00ID == A00DTLTBID && x.SYKIID == SYKI && x.ACTIVE == 1).OrderByDescending(x => x.ACTIONON).FirstOrDefault();
                if (ProjectStatusDetails != null)
                {
                    a00ProjectStateUpdate.PROJECTSTAGE = ProjectStatusDetails.PROJECTSTAGE;
                    a00ProjectStateUpdate.ITCONFAPPROVEDON = ProjectStatusDetails.ITCONFAPPROVEDON;
                    a00ProjectStateUpdate.PROJECTSTATUSREMARKS = ProjectStatusDetails.REMARKS;
                    a00ProjectStateUpdate.PROJECTSTATUS = ProjectStatusDetails.PROJECTSTATUS;
                    a00ProjectStateUpdate.ATTACHMENTNOTE = ProjectStatusDetails.ATTACHMENTNOTE;
                }
                return a00ProjectStateUpdate;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //Change start on 10-July-2021
        public A00DataViewModel GetA00ProjectUpdateStatusList(long? A00DTLTBID, decimal? SYKI)
        {
            try
            {
                A00DataViewModel dvm = new A00DataViewModel();
                dvm._A00ProjectUpdateStatusList = _pdDBContext.A00PROJECTSTATUSUPDATE.Where(x => x.A00ID == A00DTLTBID && x.SYKIID == SYKI && x.ACTIVE == 1).Select(x => new A00ProjectStateUpdate
                {
                    ProjectStatusUpdateID = x.ID,
                    PROJECTSTAGE = x.PROJECTSTAGE,
                    ITCONFAPPROVEDON = x.ITCONFAPPROVEDON,
                    PROJECTSTATUSREMARKS = x.REMARKS,
                    PROJECTSTATUS = x.PROJECTSTATUS,
                    ATTACHMENTNOTE = x.ATTACHMENTNOTE,
                    ActionOn = x.ACTIONON,
                    EmpName = _pdDBContext.ADEMPLOYEE.Where(X => X.ADEMPCODE == x.ACTIONBY).Select(X => X.FIRSTNAME + " " + X.LASTNAME).FirstOrDefault(),
                }).ToList();
                return dvm;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //Change end on 10-July-2021

        public List<A00AcitivtyHistoryVM> A00ActivityHistory(int Id)
        {
            List<A00AcitivtyHistoryVM> A00AcitivtyHVM = new List<A00AcitivtyHistoryVM>();
            A00AcitivtyHVM = (from ActivityH in _pdDBContext.A00ACTIVITYHISTORY.Where(x => x.A00ACTIVITYID == Id)
                              from Emplayee in _pdDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == ActivityH.ACTIONBY).DefaultIfEmpty()
                              select new A00AcitivtyHistoryVM
                              {
                                  A00ActivityHistoryID = ActivityH.ID,
                                  FIRSTNAME = Emplayee.FIRSTNAME,
                                  LASTNAME = Emplayee.LASTNAME,
                                  ActionOn = ActivityH.ACTIONON,
                                  Remarks = ActivityH.REMARKS,
                                  ActivitySchedule = (int)ActivityH.ACTIVITYSCHEDULE
                              }).ToList();
            return A00AcitivtyHVM.OrderBy(x => x.A00ActivityHistoryID).ToList();
        }

        //Change start on 22-July-2021
        public int UpdateA00ConvertToCR(long A00DTLTBID)
        {
            try
            {
                var _a00DtlTB = _pdDBContext.A00DTLTB.Find(A00DTLTBID);
                _a00DtlTB.STATUSCD = 125;
                _pdDBContext.Entry(_a00DtlTB).State = EntityState.Modified;
                _pdDBContext.SaveChanges();
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int InsertA00ConvertToCR(long A00DTLTBID, int CHANGEDBY, string CONVERTTOCRREMARKS)
        {
            try
            {
                //var IsConvertExist = _pdDBContext.A00CONVERTTOCR.Any(x => x.A00DTLTBID == A00DTLTBID);
                var IsConvertExist = _pdDBContext.A00CONVERTTOCR.FirstOrDefault(x => x.A00DTLTBID == A00DTLTBID) == null ? false : true;
                if (IsConvertExist == false)
                {
                    var _A00CONVERTTOCR = new A00CONVERTTOCR();
                    _A00CONVERTTOCR.A00DTLTBID = A00DTLTBID;
                    _A00CONVERTTOCR.CHANGEDBY = CHANGEDBY;
                    _A00CONVERTTOCR.CHANGEDON = DateTime.Now;
                    _A00CONVERTTOCR.CONVERTTOCRREMARKS = CONVERTTOCRREMARKS;
                    _pdDBContext.Entry(_A00CONVERTTOCR).State = EntityState.Added;
                    _pdDBContext.SaveChanges();
                }
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public A00ConvertToCRModel getConvertedToCRDtl(long? A00DTLTBID)
        {
            var result = _pdDBContext.A00CONVERTTOCR.Where(x => x.A00DTLTBID == A00DTLTBID).FirstOrDefault();
            A00ConvertToCRModel ccr = new A00ConvertToCRModel();
            ccr.A00DTLTBID = (long)result.A00DTLTBID;
            ccr.CHANGEBY = _pdDBContext.ADEMPLOYEE.Where(X => X.ADEMPCODE == result.CHANGEDBY).Select(X => X.FIRSTNAME + " " + X.LASTNAME).FirstOrDefault();
            ccr.CHANGEON = result.CHANGEDON;
            ccr.CONVERTTOCRREMARKS = result.CONVERTTOCRREMARKS;
            return ccr;
        }
        //Change end on 22-July-2021

        public short AddRemark(long A00ID, string REMARK, long loginUser)
        {
            short retval = 0;
            try
            {
                A00REMARKS objA00REMARK = new A00REMARKS();

                objA00REMARK = new A00REMARKS();
                if (_pdDBContext.A00REMARKS.Count() == 0)
                {
                    objA00REMARK.A00REMARKSID = 1;
                }
                else
                {
                    objA00REMARK.A00REMARKSID = _pdDBContext.A00REMARKS.Max(x => x.A00REMARKSID) + 1;
                }

                objA00REMARK.A00DTLID = A00ID;
                objA00REMARK.REMARKS = REMARK;
                objA00REMARK.ADDEDBY = loginUser;
                objA00REMARK.ADDEDDATE = DateTime.Now;

                _pdDBContext.Entry(objA00REMARK).State = EntityState.Added;
                _pdDBContext.SaveChanges();

                retval = 1;
            }
            catch (Exception ex)
            {
                retval = -1;
            }
            return retval;
        }

        public List<A00REMARKSVM> A00RemarksHistory(int A00Id)
        {
            List<A00REMARKSVM> A00RemarksHistoryVM = new List<A00REMARKSVM>();
            A00RemarksHistoryVM = (from A00REMARKS in _pdDBContext.A00REMARKS.AsEnumerable().Where(x => x.A00DTLID == A00Id)
                                   from Emplayee in _pdDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == A00REMARKS.ADDEDBY).DefaultIfEmpty()
                                   select new A00REMARKSVM
                                   {
                                       A00REMARKSID = A00REMARKS.A00REMARKSID,
                                       ADDEDBY = A00REMARKS.ADDEDBY,
                                       FIRSTNAME = Emplayee.FIRSTNAME,
                                       LASTNAME = Emplayee.LASTNAME,
                                       REMARKS = A00REMARKS.REMARKS,
                                       //ADDEDDATE = A00REMARKS.ADDEDDATE,
                                       stringAddedDate = A00REMARKS.ADDEDDATE.ToString("dd-MM-yyyy hh:mm:ss")
                                   }).ToList();
            return A00RemarksHistoryVM.OrderByDescending(x => x.A00REMARKSID).ToList();
        }
    }
}
