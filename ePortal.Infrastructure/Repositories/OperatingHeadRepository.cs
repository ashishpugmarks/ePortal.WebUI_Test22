using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ePortal.Infrastructure.Repositories
{
    public class OperatingHeadRepository
    {
        private EPortalDBContext _ohDBContext;
        public OperatingHeadRepository(EPortalDBContext ohDBContext)
        {
            _ohDBContext = ohDBContext;
        }        

        public OperatingHeadSYKIViewModel GetAssetRegistrationSYKIList()
        {
            OperatingHeadSYKIViewModel operatingHeadSYKIViewModel = new OperatingHeadSYKIViewModel();

            var SYKIList = (from data in _ohDBContext.SYKI.Where(x => x.ACTIVE == 1)
                            select data).ToList();
            if (SYKIList.Count > 0)
            {
                foreach (var kiList in SYKIList)
                {
                    operatingHeadSYKIViewModel._SYKIList.Add(new OperatingHeadSYKIList { KICODE = kiList.KICODE, SYKIID = kiList.SYKIID, ACTIVE = kiList.ACTIVE });
                }
            }
            return operatingHeadSYKIViewModel;
        }

        public long? GetOperationID(long Empcode, long Sykid)
        {
            var OPERATIONID = _ohDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == Sykid && x.ADEMPCODE == Empcode).Select(x => x.OPERATIONID).FirstOrDefault();

            return OPERATIONID;
        }

        //public List<OperatingHeadSearchModel1> GetDivisionListForISSCMemberNomination(long Sykid, long Operationid)
        //{
        //    var lastKi = (Sykid - 1);
        //    List<OperatingHeadSearchModel1> Operatinglist = new List<OperatingHeadSearchModel1>();
        //    var opetinghead = new OperatingHeadSearchModel1();

        //    var list1 = _ohDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == Sykid && x.ADFUNCTIONALDESIGNATIONID==2 
        //    && x.OPERATIONID == Operationid  && x.ADDESIGNATIONID <= 14).Select(x => x).ToList();

        //   var list2 = list1.Select(x => new
        //    {
        //        DIVISIONID = x.DIVISIONID,
        //        DIVISION = x.DIVISION,
        //        ISSCMEMBER = _ohDBContext.ADEMPLOYEE.Where(e => e.ADEMPCODE.ToString().Contains(x.ADEMPCODE.ToString()))
        //            .Select(e => new empName { empcode = e.ADEMPCODE, Empname = e.FIRSTNAME + " " + e.LASTNAME }).ToList(),
        //        OLDISSCMEMBERCode = _ohDBContext.ASSET_REGISTER_MEMNOMINATION
        //            .Where(m => m.SYKIID == lastKi && m.DIVISIONID == x.DIVISIONID).Select(m => m.ISSCEMPCODE).FirstOrDefault()
        //    }).ToList();

        //    var list = list2.Select(x => new OperatingHeadSearchModel1
        //    {
        //        DIVISIONID = x.DIVISIONID,
        //        DIVISION = x.DIVISION,
        //        ISSCMEMBER = x.ISSCMEMBER,
        //        OLDISSCMEMBER = _ohDBContext.ADEMPLOYEE.Where(e => e.ADEMPCODE == x.OLDISSCMEMBERCode)
        //            .Select(e => e.FIRSTNAME + " " + e.LASTNAME).FirstOrDefault()
        //    }).ToList();

        //    return list;
        //}

        public List<OperatingHeadSearchModel1> GetDivisionListForISSCMemberNomination(long Sykid, long Operationid, long empcode)
        {
            var exceptionList = new List<int> { 1, 3, 5, 7, 8, 9, 13, 16, 18, 19, 20, 21, 22, 23, 24, 25, 33 };

            var lastKi = (Sykid - 1);
            List<OperatingHeadSearchModel1> Operatinglist = new List<OperatingHeadSearchModel1>();
            var opetinghead = new OperatingHeadSearchModel1();

            // var list1 = _ohDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == Sykid && (x.ADDESIGNATIONID >= 0 && x.ADDESIGNATIONID <= 14) && (x.ADFUNCTIONALDESIGNATIONID >= 2 && x.ADFUNCTIONALDESIGNATIONID <= 3)
            // && x.OPERATIONID == Operationid && x.DIVISIONID !=null).Select(x => new { x.DIVISIONID, x.DIVISION }).Distinct().ToList();

            var list1 = _ohDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == Sykid
         && x.OPERATIONID == Operationid && x.DIVISIONID != null
         && x.ADDESIGNATIONID != 1 && x.ADDESIGNATIONID != 3 && x.ADDESIGNATIONID != 7 && x.ADDESIGNATIONID != 8 && x.ADDESIGNATIONID != 9
         && x.ADDESIGNATIONID != 13 && x.ADDESIGNATIONID != 16 && x.ADDESIGNATIONID != 18 && x.ADDESIGNATIONID != 19 && x.ADDESIGNATIONID != 20
         && x.ADDESIGNATIONID != 21 && x.ADDESIGNATIONID != 22 && x.ADDESIGNATIONID != 23 && x.ADDESIGNATIONID != 24 && x.ADDESIGNATIONID != 25 && x.ADDESIGNATIONID != 33)
         .Select(x => new { x.DIVISIONID, x.DIVISION }).Distinct().ToList();

            //var emplist = _ohDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == Sykid && (x.ADDESIGNATIONID >= 5 && x.ADDESIGNATIONID <= 14) && x.ADFUNCTIONALDESIGNATIONID == 2
            // && x.OPERATIONID == Operationid).Select(x => x.ADEMPCODE).ToList();

            var lastopid = _ohDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == lastKi && x.ADEMPCODE == empcode).Select(x => x.OPERATIONID).FirstOrDefault();

            var lastdivision = _ohDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == lastKi && x.OPERATIONID == lastopid && x.DIVISIONID != null)
                .Select(x => new
                {
                    x.DIVISIONID,
                    x.OPERATIONID,
                    x.DIVISION,
                    old = _ohDBContext.ASSET_REGISTER_MEMNOMINATION.Where(m => m.SYKIID == lastKi && m.DIVISIONID == x.DIVISIONID && m.OPERATIONID == x.OPERATIONID).Select(m => m.ISSCEMPCODE).FirstOrDefault()
                }).Distinct().ToList();

            var lastkidata = lastdivision.Select(x => new
            {
                old = _ohDBContext.ASSET_REGISTER_MEMNOMINATION.Where(m => m.SYKIID == lastKi && m.DIVISIONID == x.DIVISIONID && m.OPERATIONID == x.OPERATIONID).Select(m => m.ISSCEMPCODE).FirstOrDefault()
            }).ToList();

            var lastemp = lastkidata.Select(x => new
            {
                emp = _ohDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == x.old).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault()
            }).ToList();

            // OLDISSCMEMBERCode = _ohDBContext.ASSET_REGISTER_MEMNOMINATION.Where(m => m.SYKIID == lastKi && m.DIVISIONID == lastoperationid).Select(m => m.ISSCEMPCODE).ToList()
            //&& x1.DIVISION.Contains("Health & Wellness")
            //&& x1.ADDESIGNATIONID != 4 && x1.ADDESIGNATIONID != 5 && x1.ADDESIGNATIONID != 11 && x1.ADDESIGNATIONID != 14

            var finalemplist = list1.Select(x => new
            {
                DIVISIONID = x.DIVISIONID,
                DIVISION = x.DIVISION,
                ISSCMEMBER = _ohDBContext.VW_ASSOCIATELVLDETAILS.Where(x1 => x1.SYKI == Sykid && x1.DIVISION.Contains("Health & Wellness") && x1.DIVISIONID == x.DIVISIONID).Count() > 0 ?
                _ohDBContext.VW_ASSOCIATELVLDETAILS.Where(x1 => x1.SYKI == Sykid && x1.ADDESIGNATIONID != 1 && x1.ADDESIGNATIONID != 3 && x1.ADDESIGNATIONID != 7 && x1.ADDESIGNATIONID != 8 && x1.ADDESIGNATIONID != 9
                && x1.ADDESIGNATIONID != 13 && x1.ADDESIGNATIONID != 16 && x1.ADDESIGNATIONID != 18 && x1.ADDESIGNATIONID != 19 && x1.ADDESIGNATIONID != 20
                && x1.ADDESIGNATIONID != 21 && x1.ADDESIGNATIONID != 22 && x1.ADDESIGNATIONID != 23 && x1.ADDESIGNATIONID != 24 && x1.ADDESIGNATIONID != 25 && x1.ADDESIGNATIONID != 33
                && x1.OPERATIONID == Operationid && x1.DIVISIONID == x.DIVISIONID)
                      .Select(e => new empName { empcode = e.ADEMPCODE, Empname = _ohDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == e.ADEMPCODE).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault() }).OrderBy(y => y.Empname).ToList()
                      :
                _ohDBContext.VW_ASSOCIATELVLDETAILS.Where(x1 => x1.SYKI == Sykid && x1.ADDESIGNATIONID != 1 && x1.ADDESIGNATIONID != 3 && x1.ADDESIGNATIONID != 7 && x1.ADDESIGNATIONID != 8 && x1.ADDESIGNATIONID != 9
                && x1.ADDESIGNATIONID != 13 && x1.ADDESIGNATIONID != 16 && x1.ADDESIGNATIONID != 18 && x1.ADDESIGNATIONID != 19 && x1.ADDESIGNATIONID != 20
                && x1.ADDESIGNATIONID != 21 && x1.ADDESIGNATIONID != 22 && x1.ADDESIGNATIONID != 23 && x1.ADDESIGNATIONID != 24 && x1.ADDESIGNATIONID != 25 && x1.ADDESIGNATIONID != 33
                && x1.OPERATIONID == Operationid && x1.DIVISIONID == x.DIVISIONID && x1.ADFUNCTIONALDESIGNATIONID >= 2)
                      .Select(e => new empName { empcode = e.ADEMPCODE, Empname = _ohDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == e.ADEMPCODE).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault() }).OrderBy(y => y.Empname).ToList(),
                OLDISSCMEMBERCode = lastdivision.Where(x1 => x1.DIVISION == x.DIVISION).Select(x1 => x1.old).FirstOrDefault(),
                Current_Ki_Empcode = _ohDBContext.ASSET_REGISTER_MEMNOMINATION.Where(p => p.SYKIID == Sykid && p.DIVISIONID == x.DIVISIONID).Count() == 0 ? 0 : Convert.ToDecimal(_ohDBContext.ASSET_REGISTER_MEMNOMINATION.Where(p => p.SYKIID == Sykid && p.DIVISIONID == x.DIVISIONID).Select(p => p.ISSCEMPCODE).FirstOrDefault())
            }).ToList();

            var list = finalemplist.Select(x => new OperatingHeadSearchModel1
            {
                DIVISIONID = x.DIVISIONID,
                DIVISION = x.DIVISION,
                ISSCMEMBER = x.ISSCMEMBER,
                //OLDISSCMEMBER="",
                //OLDISSCMEMBER = x.OLDISSCMEMBERCode
                OLDISSCMEMBER = _ohDBContext.ADEMPLOYEE.Where(e => e.ADEMPCODE == x.OLDISSCMEMBERCode).Select(e => e.FIRSTNAME + " " + e.LASTNAME).FirstOrDefault(),
                Current_Ki_Empcode = (long)x.Current_Ki_Empcode
            }).ToList();
            return list;
        }

        public List<empName> getEmpList()
        {
            List<empName> emplist = new List<empName>();
            //  var empNamelist = _ohDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == 22 && x.ADDESIGNATIONID == 14 && x.OPERATIONID == 7342 && x.DIVISIONID == 7704).ToList();
            var EmpNameList = _ohDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == 22 && x.ADDESIGNATIONID == 14 && x.OPERATIONID == 7342 && x.ADFUNCTIONALDESIGNATIONID == 2).ToList();
            List<string> elist = new List<string>();
            foreach (var item in EmpNameList)
            {
                elist.Add(item.ADEMPCODE.ToString());
            }
            //var empdata = _ohDBContext.ADEMPLOYEE.Where(x=>elist.Contains(x.ADEMPCODE.ToString()));
            // var empNamelist1 = _ohDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE).ToList();
            var empNamelist = _ohDBContext.ADEMPLOYEE.ToList();
            emplist.Add(new empName() { Empname = "Select" });
            foreach (var item in empNamelist)
            {
                foreach (string eitem in elist)
                {
                    if (eitem == item.ADEMPCODE.ToString())
                    {
                        var items = new empName();
                        items.Empname = item.FIRSTNAME + " " + item.LASTNAME;
                        items.empcode = item.ADEMPCODE;
                        emplist.Add(items);
                        break;
                    }
                }

            }
            return emplist;
        }

        public OperatingHeadAddNominationVM InsertUpdateMemberNomination(OperatingHeadAddNominationVM AddNominationVM)
        {
            var NominationList = _ohDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.SYKIID == AddNominationVM.SYKIID && x.OPERATIONID == AddNominationVM.OperationID && x.ACTIVE == 1).ToList();

            if (NominationList.Count == 0)
            {
                List<ASSET_REGISTER_MEMNOMINATION> MEMNOMINATIONList = new List<ASSET_REGISTER_MEMNOMINATION>();
                int i = 0;
                foreach (var DIVISIONID in AddNominationVM.DIVISIONIDs)
                {
                    ASSET_REGISTER_MEMNOMINATION Nomination = new ASSET_REGISTER_MEMNOMINATION
                    {
                        SYKIID = AddNominationVM.SYKIID,
                        OPERATIONID = AddNominationVM.OperationID,
                        DIVISIONID = DIVISIONID,
                        ISSCEMPCODE = AddNominationVM.ISSCMEMBERs[i],
                        STATUS = (short)AddNominationVM.Status,
                        ACTIVE = 1,
                        ADDEDBY = AddNominationVM.ADDEDBY,
                        ADDEDDATE = DateTime.Now

                    };
                    MEMNOMINATIONList.Add(Nomination);
                    _ohDBContext.Entry(Nomination).State = EntityState.Added;
                    _ohDBContext.SaveChanges();
                    _ohDBContext.Entry(Nomination).State = EntityState.Detached;
                    i++;
                }
                if (MEMNOMINATIONList.Any())
                {
                    //_ohDBContext.ASSET_REGISTER_MEMNOMINATION.AddRange(MEMNOMINATIONList);
                    //_ohDBContext.SaveChanges();
                    AddNominationVM.SaveMsg = "Members has been nominated successfully.";
                    AddNominationVM.SaveStatus = 1;
                }
            }
            else
            {
                List<ASSET_REGISTER_MEMNOMINATION> MEMNOMINATIONList = new List<ASSET_REGISTER_MEMNOMINATION>();
                var existingDivisionIDs = NominationList.Select(x => x.DIVISIONID).ToList();
                var nonExistingDivisionIDs = AddNominationVM.DIVISIONIDs.Where(x => !existingDivisionIDs.Contains(x)).ToList();
                int i = 0;
                foreach (var DIVISIONID in nonExistingDivisionIDs)
                {
                    ASSET_REGISTER_MEMNOMINATION Nomination = new ASSET_REGISTER_MEMNOMINATION
                    {
                        SYKIID = AddNominationVM.SYKIID,
                        OPERATIONID = AddNominationVM.OperationID,
                        DIVISIONID = DIVISIONID,
                        ISSCEMPCODE = AddNominationVM.ISSCMEMBERs[i],
                        STATUS = (short)AddNominationVM.Status,
                        ACTIVE = 1,
                        ADDEDBY = AddNominationVM.ADDEDBY,
                        ADDEDDATE = DateTime.Now
                    };
                    MEMNOMINATIONList.Add(Nomination);
                    _ohDBContext.Entry(Nomination).State = EntityState.Added;
                    _ohDBContext.SaveChanges();
                    i++;
                }
                if (MEMNOMINATIONList.Any())
                    _ohDBContext.ASSET_REGISTER_MEMNOMINATION.AddRange(MEMNOMINATIONList);
                int j = 0;
                foreach (var nomination in NominationList)
                {
                    nomination.DIVISIONID = nomination.DIVISIONID == AddNominationVM.DIVISIONIDs[j] ? nomination.DIVISIONID : AddNominationVM.DIVISIONIDs[j];
                    nomination.ISSCEMPCODE = nomination.ISSCEMPCODE == AddNominationVM.ISSCMEMBERs[j] ? nomination.ISSCEMPCODE : AddNominationVM.ISSCMEMBERs[j];
                    nomination.UPDATEDBY = AddNominationVM.ADDEDBY;
                    nomination.UPDATEDDATE = DateTime.Now;
                    nomination.STATUS = (short)AddNominationVM.Status;
                    j++;
                }
                _ohDBContext.SaveChanges();
                AddNominationVM.SaveMsg = "Member's nomination has been updated successfully.";
                AddNominationVM.SaveStatus = 2;
            }

            return AddNominationVM;
        }

        public bool CheckDate_PeriodSetting(long Sykid)
        {
            bool isTrue = false;
            //  var startDate = _ohDBContext.ASSET_REGISTER_PERIODSETTING.Where(x => x.SYKIID == Sykid).Select(x => x.STARTDATE).FirstOrDefault();
            var endDate = _ohDBContext.ASSET_REGISTER_PERIODSETTING.Where(x => x.SYKIID == Sykid).Select(x => x.ENDDATE).FirstOrDefault();
            if (DateTime.Now.Date > Convert.ToDateTime(endDate).Date)
            {
                isTrue = true;
            }
            return isTrue;
        }

        public int Operatinghead_recordexist(long? Userid, long Sykid)
        {
            var check = _ohDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.ADDEDBY == Userid && x.SYKIID == Sykid && x.STATUS == 1).Count();
            return check;
        }

        public ISSCMember_details GetIsscMemberDetails(long empcode)
        {
            var ISSCmember = _ohDBContext.ADEMPLOYEE.Where(e => e.ADEMPCODE == empcode).Select(x => new ISSCMember_details
            {
                EmpName = x.FIRSTNAME + " " + x.LASTNAME,
                EmpEmail = x.EMAILID
            }).FirstOrDefault();
            return ISSCmember;
        }

    }
}
