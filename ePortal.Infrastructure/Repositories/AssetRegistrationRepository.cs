

using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ePortal.Infrastructure.Repositories
{
    public class AssetRegistrationRepository
    {
        private readonly EPortalDBContext _arDBContext;
        public AssetRegistrationRepository(EPortalDBContext objEPortalDBContext)
        {
            _arDBContext = objEPortalDBContext;
        }
        public AssetRegistrationSYKIViewModel GetAssetRegistrationSYKIList()
        {
            AssetRegistrationSYKIViewModel assetRegistrationSYKIViewModel = new AssetRegistrationSYKIViewModel();

            var SYKIList = (from data in _arDBContext.SYKI.Where(x => x.ACTIVE == 1)
                            select data).ToList();
            if (SYKIList.Count > 0)
            {
                foreach (var kiList in SYKIList)
                {
                    assetRegistrationSYKIViewModel._SYKIList.Add(new AssetRegistrationSYKIList { KICODE = kiList.KICODE, SYKIID = kiList.SYKIID, ACTIVE = kiList.ACTIVE });
                }
            }
            return assetRegistrationSYKIViewModel;
        }

        public AssetRegistrationSYKIViewModel GetAssetRegistrationSYKIListForViewTeam()
        {
            AssetRegistrationSYKIViewModel assetRegistrationSYKIViewModel = new AssetRegistrationSYKIViewModel();

            var SYKIList = (from data in _arDBContext.SYKI.Where(x => x.SYKIID > 21)
                            select data).OrderByDescending(x => x.SYKIID).ToList();
            if (SYKIList.Count > 0)
            {
                foreach (var kiList in SYKIList)
                {
                    assetRegistrationSYKIViewModel._SYKIList.Add(new AssetRegistrationSYKIList { KICODE = kiList.KICODE, SYKIID = kiList.SYKIID, ACTIVE = kiList.ACTIVE });
                }
            }
            return assetRegistrationSYKIViewModel;
        }

        public AssetRegistrationSYKIViewModel GetAssetRegistrationSYKIListForDashboard()
        {
            AssetRegistrationSYKIViewModel assetRegistrationSYKIViewModel = new AssetRegistrationSYKIViewModel();

            var SYKIList = (from data in _arDBContext.SYKI.Where(x => x.SYKIID > 21 && x.ACTIVE != 1)
                            select data).OrderByDescending(x => x.SYKIID).Take(3).ToList();
            if (SYKIList.Count > 0)
            {
                foreach (var kiList in SYKIList)
                {
                    assetRegistrationSYKIViewModel._SYKIList.Add(new AssetRegistrationSYKIList { KICODE = kiList.KICODE, SYKIID = kiList.SYKIID, ACTIVE = kiList.ACTIVE });
                }
            }
            return assetRegistrationSYKIViewModel;
        }

        public PeriodForISSCMemberNominationVM InsertUpdatePeriodSettingDetail(PeriodForISSCMemberNominationVM periodSettingVM)
        {
            //var s = _arDBContext.ASSET_REGISTER_PERIODSETTING.OrderByDescending(x => x.ID).Select(x => x.ID).FirstOrDefault();
            //long periodSettingid = s == 0 ? 1 : s;
            var PeriodSetting = _arDBContext.ASSET_REGISTER_PERIODSETTING.Where(x => x.ID == periodSettingVM.PeriodForISSCMemberNominationID).FirstOrDefault();
            var ExistingKIdata = _arDBContext.ASSET_REGISTER_PERIODSETTING.Where(x => x.SYKIID == periodSettingVM.SYKIID).Select(x => new { x.SYKIID, x.ID }).FirstOrDefault();
            if (ExistingKIdata != null && ExistingKIdata.SYKIID > 0 && periodSettingVM.ActionType == 0)
            {

                periodSettingVM.Status = 3;
                periodSettingVM.Msg = "This ki is already exist. In Case of any change, please update the entry.";
                return periodSettingVM;
            }
            else
            {
                if (PeriodSetting == null)
                {
                    PeriodSetting = new ASSET_REGISTER_PERIODSETTING();
                }
                //CultureInfo provider = new CultureInfo("en-US");
                CultureInfo provider = CultureInfo.InvariantCulture;
                PeriodSetting.SYKIID = periodSettingVM.SYKIID;
                DateTime sdt = DateTime.ParseExact(periodSettingVM.StartDate, "dd/MM/yyyy", provider);
                //DateTime edt = DateTime.ParseExact(periodSettingVM.EndDate, new string[] { "MM.dd.yyyy", "MM-dd-yyyy", "MM/dd/yyyy" }, provider, DateTimeStyles.None);


                DateTime dateTime14;
                dateTime14 = DateTime.ParseExact(periodSettingVM.EndDate, "dd/MM/yyyy", provider);

                PeriodSetting.STARTDATE = sdt;
                PeriodSetting.ENDDATE = dateTime14;
                PeriodSetting.ACTIVE = 1;

                if (periodSettingVM.PeriodForISSCMemberNominationID > 0)
                {
                    PeriodSetting.UPDATEDBY = (long)periodSettingVM.ADDEDBY;
                    PeriodSetting.UPDATEDDATE = DateTime.Now;
                    _arDBContext.Entry(PeriodSetting).State = EntityState.Modified;
                    _arDBContext.SaveChanges();
                    periodSettingVM.Status = 2;
                    periodSettingVM.Msg = "Data has been updated succesufully.";
                }
                else
                {
                    //string periodSettingid = _arDBContext.ASSET_REGISTER_PERIODSETTING.Max(x => x.ID).ToString(); //Added by aumento for Anupma san UAT
                    //PeriodSetting.ID = Int64.Parse(periodSettingid) + 1; //Added by aumento for Anupma san UAT
                    PeriodSetting.ADDEDBY = (long)periodSettingVM.ADDEDBY;
                    PeriodSetting.ADDEDDATE = DateTime.Now;
                    _arDBContext.Entry(PeriodSetting).State = EntityState.Added;
                    _arDBContext.SaveChanges();
                    periodSettingVM.Status = 1;
                    periodSettingVM.Msg = "Data has been saved succesufully.";
                }
                return periodSettingVM;
            }

        }

        public List<PeriodForISSCMemberNominationListVM> GetPeriodSettingList()
        {
            //var oldData = _arDBContext.ASSET_REGISTER_PERIODSETTING.AsEnumerable().Where(x => x.ACTIVE == 1).OrderByDescending(x => x.SYKIID).ToList();
            var PeriodSettingList = _arDBContext.ASSET_REGISTER_PERIODSETTING.ToList().Where(x => x.ACTIVE == 1).OrderByDescending(x => x.SYKIID).Select((x, index) => new PeriodForISSCMemberNominationListVM
            {
                SNo = index + 1,
                PeriodForISSCMemberNominationID = (long)x.ID,
                StartDate = x.STARTDATE,
                EndDate = x.ENDDATE,
                SYKIID = x.SYKIID,
                SYKI = x.SYKI !=null? x.SYKI.KICODE:null,
                ADDEDBY = x.ADDEDBY,
               EmpName = _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == x.ADDEDBY).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault(),
                AdditionDate = x.ADDEDDATE,

            }).ToList();
            return PeriodSettingList;

        }


        //public PeriodForISSCMemberNominationVM DeletePeriodForISSCMemberNomination(PeriodForISSCMemberNominationVM periodSettingVMD)
        //{
        //    //var res = _arDBContext.ASSET_REGISTER_PERIODSETTING.OrderByDescending(x => x.ID).Select(x => x.ID).FirstOrDefault();
        //    // long periodSettingid = s == 0 ? 1 : s;
        //    var res = _arDBContext.ASSET_REGISTER_PERIODSETTING.Where(x => x.ID == periodSettingVMD.PeriodForISSCMemberNominationID).FirstOrDefault();
        //    _arDBContext.ASSET_REGISTER_PERIODSETTING.Remove(res);
        //    _arDBContext.SaveChanges();

        //    return periodSettingVMD;
        //}
        //==========================//
        public A00_VW_ASSOCIATELVLDETAILS GetA00_VW_ASSOCIATELVLDETAILS(AssetRegistrationSearchModel objSearchModel)
        {
            var result = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == objSearchModel.ADEMPCODE && x.SYKI == objSearchModel.SYKIID).Select(x => new
            //  var result = _pdDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == objSearchModel.SYKI).Select(x => new VW_EMPLOYEEHEADDETAILS
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
            A00_VW_ASSOCIATELVLDETAILS va = new A00_VW_ASSOCIATELVLDETAILS();
            if (result != null)
            {
                va.DEPARTMENTID = result.DEPARTMENTID;
                va.OPERATIONID = result.OPERATIONID;
                va.DIVISIONID = result.DIVISIONID;
                va.SECTIONID = result.SECTIONID;
                va.OPERATION = result.OPERATION;
                va.DIVISION = result.DIVISION;
                va.DEPARTMENT = result.DEPARTMENT;
                va.SECTION = result.SECTION;
                va.LastSYKI = _arDBContext.SYKI.Where(x => x.SYKIID < objSearchModel.SYKIID).OrderByDescending(x => x.SYKIID).Select(x => x.KICODE).FirstOrDefault(); //Dinesh
                va.LastSYKIID = _arDBContext.SYKI.Where(x => x.SYKIID < objSearchModel.SYKIID).OrderByDescending(x => x.SYKIID).Select(x => x.SYKIID).FirstOrDefault(); //Dinesh
                va.ADFUNCTIONALDESIGNATIONID = result.ADFUNCTIONALDESIGNATIONID;
            }
            var result1 = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == objSearchModel.ADEMPCODE && x.SYKI == va.LastSYKIID).Select(x => new
            //  var result = _pdDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == objSearchModel.SYKI).Select(x => new VW_EMPLOYEEHEADDETAILS
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
            if (result1 != null)
            {
                va.LastOPERATIONID = result1.OPERATIONID;
                va.LastDIVISIONID = result1.DIVISIONID;
                va.LastOPERATION = result1.OPERATION;
                va.LastDIVISION = result1.DIVISION;
            }
            return va;
        }
        public A00DataViewModel GetA00ADORGLEVELList()
        {
            A00DataViewModel dvm = new A00DataViewModel();

            var AdOrgLevelList = (from data1 in _arDBContext.ADORGLEVEL.Where(x => x.ADORGLEVELTYPEID == 1 && x.ACTIVE == 1)
                                      // .OrderByDescending(x => x.LEVELDESCRIP)
                                      // orderby data1.LEVELDESCRIP descending
                                  select data1).OrderByDescending(x => x.LEVELDESCRIP).ToList();

            if (AdOrgLevelList.Count > 0)
            {
                foreach (var lvlList in AdOrgLevelList)
                {
                    dvm._ADOrgLevelList.Add(new A00ADORGLEVELList { LEVELDESCRIP = lvlList.LEVELDESCRIP, ADORGLEVELID = lvlList.ADORGLEVELID });
                }
            }
            return dvm;
        }

        public List<ADORGLEVEL> GetOrgLevelList(long typeId)
        {
            var iList = from data in _arDBContext.ADORGLEVEL
                        where data.ACTIVE == 1 && (data.SYKIID >= 21)
                        && data.ADORGLEVELTYPEID == typeId
                        orderby data.LEVELDESCRIP
                        select data;
            return iList.ToList();

        }

        public List<ADORGLEVEL> BindOperation(long? id)
        {
            List<ADORGLEVEL> op = new List<ADORGLEVEL>();
            var iList = from data in _arDBContext.ADORGLEVEL
                        where data.SYKIID == id && data.ACTIVE != 0
                        && data.ADORGLEVELTYPEID == 1
                        orderby data.LEVELDESCRIP
                        select data;
            foreach (var obj in iList)
            {
                op.Add(new ADORGLEVEL
                {
                    ADORGLEVELID = obj.ADORGLEVELID,
                    LEVELDESCRIP = obj.LEVELDESCRIP
                });
            }
            return op;
        }

        public List<SearchParameterList> BindDivision(long? op_Id)
        {
            List<SearchParameterList> iList = new List<SearchParameterList>();
            var iColl = (from data in _arDBContext.VW_ASSOCIATELVLDETAILS.Where(e => e.ACTIVE == 1
                         && (e.SYKI >= 21)
                         && e.DIVISIONID != null && e.DIVISIONID != 0
                         && e.OPERATIONID == (op_Id == 0 ? e.OPERATIONID : op_Id)).OrderBy(e => e.DIVISION)
                         select new
                         {
                             data.DIVISIONID,
                             data.DIVISION
                         }).Distinct().OrderBy(x => x.DIVISION).ToList();
            foreach (var obj in iColl)
            {
                iList.Add(new SearchParameterList
                {
                    DIVISIONID = Convert.ToInt64(obj.DIVISIONID == null ? 0 : obj.DIVISIONID),
                    DIVISION = obj.DIVISION
                });
            }
            return iList;
        }

        public long? BindDivisionWithOldOperationHeadEmpCodeByOperationId(long? op_Id, long? SYKIID)
        {
            var OperatingHeadId = (from s in _arDBContext.ASSET_REGISTER_ASSETDETAILS
                                   join r in _arDBContext.ASSET_REGISTER_MEMNOMINATION on s.CREATEDBY equals r.ISSCEMPCODE
                                   where s.SKIID == SYKIID && r.SYKIID == SYKIID
                                   && r.OPERATIONID == op_Id && s.ACTIVE == 1 && s.OPERATINGHEAD_ID != null
                                   select s.OPERATINGHEAD_ID).FirstOrDefault();
            if (OperatingHeadId != null && OperatingHeadId != 0)
            {
                return Convert.ToInt16(OperatingHeadId);
            }
            else
            {
                return 0;
            }
        }

        public long? BindOldDivisionHeadEmpCodeByDivisionId(long? divId, long? SYKIID)
        {
            var DivisionHeadId = (from s in _arDBContext.ASSET_REGISTER_ASSETDETAILS
                                  join r in _arDBContext.ASSET_REGISTER_MEMNOMINATION on s.CREATEDBY equals r.ISSCEMPCODE
                                  where s.SKIID == SYKIID && r.SYKIID == SYKIID
                                  && r.DIVISIONID == divId && s.ACTIVE == 1 && s.DIVISIONHEAD_ID != null
                                  select s.DIVISIONHEAD_ID).FirstOrDefault();
            if (DivisionHeadId != null && DivisionHeadId!=0)
            {
                return Convert.ToInt16(DivisionHeadId);
            }
            else
            {
                return 0;
            }
        }

        public long? BindOldDivisionISSCMemCodeByDivisionId(long? DivisionHeadEmpCode, long? SYKIID)
        {
            var ISSCMemberDetail = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.SKIID == SYKIID && x.DIVISIONHEAD_ID == DivisionHeadEmpCode && x.ACTIVE == 1).FirstOrDefault();
            //var ISSCMemId = (from s in _arDBContext.ASSET_REGISTER_ASSETDETAILS
            //                      join r in _arDBContext.ASSET_REGISTER_MEMNOMINATION on s.CREATEDBY equals r.ISSCEMPCODE
            //                      where s.SKIID == SYKIID && r.SYKIID == SYKIID
            //                      && r.DIVISIONID == divId && s.ACTIVE == 1 && s.DIVISIONHEAD_ID != null
            //                      select s.CREATEDBY).FirstOrDefault();
            if (ISSCMemberDetail != null)
            {
                return Convert.ToInt16(ISSCMemberDetail.CREATEDBY);
            }
            else
            {
                return 0;
            }
        }

        public string OldOperatingOROldDivHeadRequestAssignToNewOpHeadOrNewDivHead(SameDivisionAndOprationApprovalVM model)
        {
            string message = "";
            if (model.NewDivHeadEmpCode > 0 && model.NewOPHeadEmpCode > 0)
            {
               
                    var matchedRecords = _arDBContext.ASSET_USER_MAPPING.Where(x => x.NEWDIVISIONEMPCODE == model.ExistingDivHeadEmpCode
                    && x.NEWOPRATIONHEADEMPCODE == model.ExistingOPHeadEmpCode && x.SYKIID == model.SYKIID).FirstOrDefault();
                    if(matchedRecords!=null)
                    {
                        matchedRecords.NEWDIVISIONEMPCODE = model.NewDivHeadEmpCode;
                        matchedRecords.NEWOPRATIONHEADEMPCODE = model.NewOPHeadEmpCode;
                        matchedRecords.MODIFIEDBY = model.CreatedBy;
                        matchedRecords.MODIFIEDDATE = DateTime.Now;
                        _arDBContext.Entry(matchedRecords).State = EntityState.Modified;
                        _arDBContext.SaveChanges();
                        message = "Operating Head and Division Head Change Successfully";
                        return message;
                    }
                    else
                    {
                        ASSET_USER_MAPPING asset = new ASSET_USER_MAPPING();
                        var rowNum = _arDBContext.ASSET_USER_MAPPING.Count();
                        asset.ASSETUSERMAPPINGID = rowNum + 1;
                        asset.SYKIID = model.SYKIID;
                        asset.DIVISIONID = model.DIVISIONID;
                        asset.OLDDIVISIONEMPCODE = model.ExistingDivHeadEmpCode;
                        asset.NEWDIVISIONEMPCODE = model.NewDivHeadEmpCode;
                        asset.OPERATIONID = model.OPERATIONID;
                        asset.OLDOPERATIONHEADCODE = model.ExistingOPHeadEmpCode;
                        asset.NEWOPRATIONHEADEMPCODE = model.NewOPHeadEmpCode;
                        asset.CREATEDBY = model.CreatedBy;
                        asset.CREATEDDATE = DateTime.Now;
                        _arDBContext.Entry(asset).State = EntityState.Added;
                        _arDBContext.SaveChanges();
                        message = "Operating Head and Division Head Change Successfully";
                        return message;
                    }

            }
            else if (model.NewDivHeadEmpCode > 0)
            {
                    var matchedRecords = _arDBContext.ASSET_USER_MAPPING.Where(x => x.NEWDIVISIONEMPCODE == model.ExistingDivHeadEmpCode
                   && x.SYKIID == model.SYKIID).FirstOrDefault();
                    if (matchedRecords != null)
                    {
                        matchedRecords.NEWDIVISIONEMPCODE = model.NewDivHeadEmpCode;
                        matchedRecords.MODIFIEDBY = model.CreatedBy;
                        matchedRecords.MODIFIEDDATE = DateTime.Now;
                        _arDBContext.Entry(matchedRecords).State = EntityState.Modified;
                        _arDBContext.SaveChanges();
                        message = "Division Head Change Successfully";
                        return message;
                    }
                    else
                    {
                        ASSET_USER_MAPPING asset = new ASSET_USER_MAPPING();
                        var rowNum = _arDBContext.ASSET_USER_MAPPING.Count();
                        asset.ASSETUSERMAPPINGID = rowNum + 1;
                        asset.SYKIID = model.SYKIID;
                        asset.DIVISIONID = model.DIVISIONID;
                        asset.OLDDIVISIONEMPCODE = model.ExistingDivHeadEmpCode;
                        asset.NEWDIVISIONEMPCODE = model.NewDivHeadEmpCode;
                        asset.OPERATIONID = model.OPERATIONID;
                        asset.OLDDIVISIONEMPCODE = model.ExistingOPHeadEmpCode;
                        asset.CREATEDBY = model.CreatedBy;
                        asset.CREATEDDATE = DateTime.Now;
                        _arDBContext.Entry(asset).State = EntityState.Added;
                        _arDBContext.SaveChanges();
                        message = "Division Head Change Successfully";
                        return message;
                    }

            }
            else if (model.NewOPHeadEmpCode > 0)
            {
                
                    var matchedRecords = _arDBContext.ASSET_USER_MAPPING.Where(x =>x.NEWOPRATIONHEADEMPCODE == model.ExistingOPHeadEmpCode && x.SYKIID == model.SYKIID).FirstOrDefault();
                    if (matchedRecords != null)
                    {
                        matchedRecords.NEWOPRATIONHEADEMPCODE = model.NewOPHeadEmpCode;
                        matchedRecords.MODIFIEDBY = model.CreatedBy;
                        matchedRecords.MODIFIEDDATE = DateTime.Now;
                       _arDBContext.Entry(matchedRecords).State = EntityState.Modified;
                       _arDBContext.SaveChanges();
                        message = "Operating Head Change Successfully";
                        return message;
                    }
                    else
                    {
                        ASSET_USER_MAPPING asset = new ASSET_USER_MAPPING();
                        var rowNum = _arDBContext.ASSET_USER_MAPPING.Count();
                        asset.ASSETUSERMAPPINGID = rowNum + 1;
                        asset.SYKIID = model.SYKIID;
                        asset.OPERATIONID = model.OPERATIONID;
                        asset.OLDOPERATIONHEADCODE = model.ExistingOPHeadEmpCode;
                        asset.NEWOPRATIONHEADEMPCODE = model.NewOPHeadEmpCode;
                        asset.CREATEDBY = model.CreatedBy;
                        asset.CREATEDDATE = DateTime.Now;
                        _arDBContext.Entry(asset).State = EntityState.Added;
                        _arDBContext.SaveChanges();
                        message = "Operating Head Change Successfully";
                        return message;
                    }

            }
            else
            {
                message = "No record update";
                return message;
            }

            //if (model.NewDivHeadEmpCode > 0 && model.NewOPHeadEmpCode > 0)
            //{
            //    using (var dbcontext = new ePortalEntities2())
            //    {
            //        var matchedRecords = dbcontext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.DIVISIONHEAD_ID == model.ExistingDivHeadEmpCode
            //        && x.OPERATINGHEAD_ID == model.ExistingOPHeadEmpCode && x.SKIID == model.SYKIID).ToList();
            //        if (matchedRecords.Count > 0)
            //        {
            //            matchedRecords.ForEach(e =>
            //            {
            //                e.DIVISIONHEAD_ID = model.NewDivHeadEmpCode;
            //                e.OPERATINGHEAD_ID = model.NewOPHeadEmpCode;
            //            });
            //            dbcontext.SaveChanges();
            //            message = "Operating Head and Division Head Change Successfully";
            //            return message;
            //        }
            //        else
            //        {
            //            message = "No record update";
            //            return message;
            //        }

            //    }
            //}
            //else if (model.NewDivHeadEmpCode > 0)
            //{
            //    using (var dbcontext = new ePortalEntities2())
            //    {
            //        var matchedRecords = dbcontext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.DIVISIONHEAD_ID == model.ExistingDivHeadEmpCode
            //        && x.SKIID == model.SYKIID).ToList();
            //        if (matchedRecords.Count > 0)
            //        {
            //            matchedRecords.ForEach(e =>
            //            {
            //                e.DIVISIONHEAD_ID = model.NewDivHeadEmpCode;
            //            });
            //            dbcontext.SaveChanges();
            //            message = "Division Head Change Successfully";
            //            return message;
            //        }
            //        else
            //        {
            //            message = "No record update";
            //            return message;
            //        }

            //    }

            //}
            //else if (model.NewOPHeadEmpCode > 0)
            //{
            //    using (var dbcontext = new ePortalEntities2())
            //    {
            //        var matchedRecords = dbcontext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.OPERATINGHEAD_ID == model.ExistingOPHeadEmpCode
            //        && x.SKIID == model.SYKIID).ToList();
            //        if (matchedRecords.Count > 0)
            //        {
            //            matchedRecords.ForEach(e =>
            //            {
            //                e.OPERATINGHEAD_ID = model.NewOPHeadEmpCode;
            //            });
            //            dbcontext.SaveChanges();
            //            message = "Operating Head Change Successfully";
            //            return message;
            //        }
            //        else
            //        {
            //            message = "No record update";
            //            return message;
            //        }

            //    }

            //}
            //else
            //{
            //    message = "No record update";
            //    return message;
            //}
        }




        public List<AssetRegistrationSearchModel> GetViewToITGRCTeamList(long? SYKIID, long? OPERATIONID, long? DIVISIONID)
        {
            // List<PeriodForISSCMemberNominationVM> PeriodSettingList = new List<PeriodForISSCMemberNominationVM>();
            // var ASSOCIATELVLDETAILS = _arDBContext.VW_ASSOCIATELVLDETAILS.ToList();
            var ViewToList = (
     from asset in _arDBContext.ASSET_REGISTER_MEMNOMINATION
     where asset.ACTIVE == 1 && asset.STATUS == 1
         && (SYKIID == null || asset.SYKIID == SYKIID)
         && (OPERATIONID == null || asset.OPERATIONID == OPERATIONID)
         && (DIVISIONID == null || asset.DIVISIONID == DIVISIONID)

     join emp in _arDBContext.ADEMPLOYEE
         on asset.ISSCEMPCODE equals emp.ADEMPCODE into empGroup
     from emp in empGroup.DefaultIfEmpty()



     select new AssetRegistrationSearchModel
     {
         SYKIID = asset.SYKIID,
         SYKI = asset.SYKI.KICODE,
         OPERATIONID = asset.OPERATIONID.ToString(),
         OPERATIONNAME = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.OPERATIONID == asset.OPERATIONID && y.SYKI == asset.SYKIID).Select(y => y.OPERATION).FirstOrDefault(),
         DIVHDHDID = asset.DIVISIONID,
         DIVISIONNAMEN  = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.SYKI == asset.SYKIID && y.DIVISIONID == asset.DIVISIONID).OrderBy(y => y.ADEMPCODE).Select(y => y.DIVISION).FirstOrDefault(),
         ADEMPCODE = (long)asset.ISSCEMPCODE,
         EmpName = emp != null ? emp.FIRSTNAME + " " + emp.LASTNAME : "",
         Id = (long)asset.ID,
         NomiationDate = asset.ADDEDDATE
     }
 ).OrderBy(x => x.OPERATIONNAME).ToList();


            return ViewToList;

        }

        public GetNominationDetails getNomination(long? Id)
        {
            var result = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.ID == Id).Select(x => x).FirstOrDefault();
            var ski = _arDBContext.SYKI.Where(x => x.SYKIID == result.SYKIID).Select(x => x.KICODE).FirstOrDefault();
            var operation = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.OPERATIONID == result.OPERATIONID && y.SYKI == result.SYKIID).Select(y => y.OPERATION).FirstOrDefault();
            var division = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.SYKI == result.SYKIID && y.DIVISIONID == result.DIVISIONID).Select(y => y.DIVISION).FirstOrDefault();

            GetNominationDetails nomidetails = new GetNominationDetails();
            nomidetails.Id = (long)result.ID;
            nomidetails.Ski = (long)result.SYKIID;
            nomidetails.Skiid = ski;
            nomidetails.operationname = operation;
            nomidetails.divisonname = division;
            nomidetails.OPERATIONID = (long)result.OPERATIONID;
            nomidetails.DIVISIONID = (long)result.DIVISIONID;
            nomidetails.empcode = (long)result.ISSCEMPCODE;
            return nomidetails;
        }

        public int Update_NominationBy_Admin(long? Id, long? Empid, long? adminid)
        {
            ASSET_REGISTER_MEMNOMINATION upd;
            //using (var dbCtx = new ePortalEntities2())
            //{
                upd = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.ID == Id).Select(x => x).FirstOrDefault();
            //}
            var assetRegDetails = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == upd.ISSCEMPCODE && x.SKIID == upd.SYKIID && x.ACTIVE == 1).FirstOrDefault();
            if(assetRegDetails!=null)
            {
                var assetUserMDetail = _arDBContext.ASSET_USER_MAPPING.Where(x => x.OLDDIVISIONEMPCODE == assetRegDetails.DIVISIONHEAD_ID && x.OLDOPERATIONHEADCODE == assetRegDetails.OPERATINGHEAD_ID
                 && x.SYKIID == assetRegDetails.SKIID).FirstOrDefault();
                if(assetUserMDetail!=null)
                {
                    assetUserMDetail.OLDISSC_MEMBERCODE = upd.ISSCEMPCODE;
                    assetUserMDetail.NEWISSC_MEMBERCODE = Empid;
                    assetUserMDetail.MODIFIEDBY = adminid;
                    assetUserMDetail.MODIFIEDDATE = DateTime.Now;
                    _arDBContext.Entry(assetUserMDetail).State = EntityState.Modified;
                    _arDBContext.SaveChanges();
                }
                else
                {
                    ASSET_USER_MAPPING assetUserM = new ASSET_USER_MAPPING();
                    var count = _arDBContext.ASSET_USER_MAPPING.Count();
                    assetUserM.ASSETUSERMAPPINGID = count + 1;
                    assetUserM.SYKIID = assetRegDetails.SKIID;
                    assetUserM.OLDDIVISIONEMPCODE = assetRegDetails.DIVISIONHEAD_ID;
                    assetUserM.OLDOPERATIONHEADCODE = assetRegDetails.OPERATINGHEAD_ID;
                    assetUserM.OLDISSC_MEMBERCODE = assetRegDetails.CREATEDBY;
                    assetUserM.NEWISSC_MEMBERCODE = Empid;
                    assetUserM.DIVISIONID = upd.DIVISIONID;
                    assetUserM.OPERATIONID = upd.OPERATIONID;
                    assetUserM.CREATEDBY = (long)adminid;
                    assetUserM.CREATEDDATE = DateTime.Now;
                    _arDBContext.Entry(assetUserM).State = EntityState.Added;
                    _arDBContext.SaveChanges();
                }
            }
            upd.ISSCEMPCODE = (long)Empid;
            upd.UPDATEDDATE = DateTime.Now;
            upd.UPDATEDBY = (long)adminid;
            //using (var dbCtx1 = new ePortalEntities2())
            //{
                _arDBContext.Entry(upd).State = EntityState.Modified;
                _arDBContext.SaveChanges();
            //}
            return 1;
        }

        public List<empName> GetEmpnameList(long? SYKI, long? OPERATIONID, long? Divisionid)
        {
            var exceptionList = new List<int> { 1, 3, 7, 8, 9, 13, 16, 18, 19, 20, 21, 22, 23, 24, 25, 33 };

            List<empName> emplist = new List<empName>();
            //  var empNamelist = _ohDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == 22 && x.ADDESIGNATIONID == 14 && x.OPERATIONID == 7342 && x.DIVISIONID == 7704).ToList();
            var divname = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == SYKI && x.OPERATIONID == OPERATIONID
             && x.DIVISIONID == Divisionid && x.DIVISIONID != null).Select(x => x.DIVISION).FirstOrDefault();

            if (divname.Contains("Health & Wellness"))
            {
                var EmpNameList = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == SYKI && x.OPERATIONID == OPERATIONID
                && x.DIVISIONID == Divisionid && x.DIVISIONID != null
                && x.ADDESIGNATIONID != 1 && x.ADDESIGNATIONID != 3 && x.ADDESIGNATIONID != 7 && x.ADDESIGNATIONID != 8 && x.ADDESIGNATIONID != 9
               && x.ADDESIGNATIONID != 13 && x.ADDESIGNATIONID != 16 && x.ADDESIGNATIONID != 18 && x.ADDESIGNATIONID != 19 && x.ADDESIGNATIONID != 20
              && x.ADDESIGNATIONID != 21 && x.ADDESIGNATIONID != 22 && x.ADDESIGNATIONID != 23 && x.ADDESIGNATIONID != 24 && x.ADDESIGNATIONID != 25 && x.ADDESIGNATIONID != 33).ToList();
                List<string> elist = new List<string>();
                foreach (var item in EmpNameList)
                {
                    elist.Add(item.ADEMPCODE.ToString());
                }
                //var empdata = _ohDBContext.ADEMPLOYEE.Where(x=>elist.Contains(x.ADEMPCODE.ToString()));
                // var empNamelist1 = _ohDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE).ToList();
                var empNamelist = _arDBContext.ADEMPLOYEE.ToList();
                //emplist.Add(new empName() { Empname = "Select" });
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
            else
            {
                var EmpNameList = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == SYKI && x.OPERATIONID == OPERATIONID
               && x.DIVISIONID == Divisionid && x.DIVISIONID != null
               && x.ADDESIGNATIONID != 1 && x.ADDESIGNATIONID != 3 && x.ADDESIGNATIONID != 7 && x.ADDESIGNATIONID != 8 && x.ADDESIGNATIONID != 9
              && x.ADDESIGNATIONID != 13 && x.ADDESIGNATIONID != 16 && x.ADDESIGNATIONID != 18 && x.ADDESIGNATIONID != 19 && x.ADDESIGNATIONID != 20
             && x.ADDESIGNATIONID != 21 && x.ADDESIGNATIONID != 22 && x.ADDESIGNATIONID != 23 && x.ADDESIGNATIONID != 24 && x.ADDESIGNATIONID != 25 && x.ADDESIGNATIONID != 33 && x.ADFUNCTIONALDESIGNATIONID >= 2).ToList();
                List<string> elist = new List<string>();
                foreach (var item in EmpNameList)
                {
                    elist.Add(item.ADEMPCODE.ToString());
                }
                //var empdata = _ohDBContext.ADEMPLOYEE.Where(x=>elist.Contains(x.ADEMPCODE.ToString()));
                // var empNamelist1 = _ohDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE).ToList();
                var empNamelist = _arDBContext.ADEMPLOYEE.ToList();
                //emplist.Add(new empName() { Empname = "Select" });
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


        }

        public List<OperatingHead> GetOperatingHead(long? SYKI)
        {
            List<OperatingHead> emplist = new List<OperatingHead>();
            //  var empNamelist = _ohDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == 22 && x.ADDESIGNATIONID == 14 && x.OPERATIONID == 7342 && x.DIVISIONID == 7704).ToList();
            var EmpNameList = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == SYKI && x.ADFUNCTIONALDESIGNATIONID == 4).Select(x => x.ADEMPCODE).Distinct().ToList();
            var emplistfillissc_member = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.SYKIID == SYKI && x.ACTIVE == 1).Select(x => x.ADDEDBY).Distinct().ToList();

            EmpNameList = EmpNameList.Where(x => !emplistfillissc_member.Contains(x)).ToList();

            //List<string> elist = new List<string>();
            //foreach (var item in EmpNameList)
            //{
            //    elist.Add(item.ToString());
            //}

            //var empdata = _ohDBContext.ADEMPLOYEE.Where(x=>elist.Contains(x.ADEMPCODE.ToString()));
            // var empNamelist1 = _ohDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE).ToList();

            var empNamelist = _arDBContext.ADEMPLOYEE.ToList();
            var empNamelists = empNamelist.Where(x => EmpNameList.Contains(x.ADEMPCODE)).ToList();
            foreach (var item in empNamelists)
            {
                var items = new OperatingHead();
                items.Empname = item.FIRSTNAME + " " + item.LASTNAME;
                items.Emailid = item.EMAILID;
                emplist.Add(items);
            }
            return emplist;
        }

        public List<OperatingHead> GetOperatingHead_For_NomationUpdate(long? SYKI, long? OPERATIONID, long? Divisionid, long EmpId)
        {
            List<OperatingHead> emplist = new List<OperatingHead>();
            // //  var empNamelist = _ohDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == 22 && x.ADDESIGNATIONID == 14 && x.OPERATIONID == 7342 && x.DIVISIONID == 7704).ToList();
            // var EmpNameList = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == SYKI && x.OPERATIONID == OPERATIONID && x.DIVISIONID == Divisionid && x.ADDESIGNATIONID == 14).ToList();
            // List<string> elist = new List<string>();
            // foreach (var item in EmpNameList)
            // {
            //     elist.Add(item.ADEMPCODE.ToString());
            // }
            // //var empdata = _ohDBContext.ADEMPLOYEE.Where(x=>elist.Contains(x.ADEMPCODE.ToString()));
            // // var empNamelist1 = _ohDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE).ToList();
            // var empNamelist = _arDBContext.ADEMPLOYEE.ToList();
            //// emplist.Add(new OperatingHead() { Empname = "Select" });
            // foreach (var item in empNamelist)
            // {
            //     foreach (string eitem in elist)
            //     {
            //         if (eitem == item.ADEMPCODE.ToString())
            //         {
            //             var items = new OperatingHead();
            //             items.Empname = item.FIRSTNAME + " " + item.LASTNAME;
            //             items.Emailid = item.EMAILID;
            //             emplist.Add(items);
            //             break;
            //         }
            //     }

            // }
            var result = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == EmpId).Select(x => new OperatingHead { Emailid = x.EMAILID, Empname = x.FIRSTNAME + " " + x.LASTNAME }).FirstOrDefault();
            emplist.Add(result);
            return emplist;
        }

        public PeriodForISSCMemberNominationVM UpdateNomination(PeriodForISSCMemberNominationVM model)
        {
            ASSEST_REGISTERUPDNOMINATION up_nomination = new ASSEST_REGISTERUPDNOMINATION();

            up_nomination.PRIODSETTINGID = (long)model.PeriodForISSCMemberNominationID;
            up_nomination.OPERTIONID = model.OPERATIONID;
            up_nomination.DIVISIONID = model.DIVISIONID;
            up_nomination.STARTDATE = Convert.ToDateTime(model.StartDate);
            up_nomination.ENDDATE = Convert.ToDateTime(model.EndDate);
            up_nomination.ACTIVE = 1;
            up_nomination.ADDEDBY = (long)model.ADDEDBY;
            up_nomination.ADDEDDATE = DateTime.Now;
            up_nomination.SYKIID = model.SYKIID;
            _arDBContext.Entry(up_nomination).State = EntityState.Added;
            _arDBContext.SaveChanges();
            model.Status = 1;
            return model;


        }

        public PeriodForITGRCMemberNominationListVM InsertUpdatePeriodITGRCSettingDetail(PeriodForITGRCMemberNominationListVM periodSettingVM)
        {

            try
            {

                var ExistingKIdata = _arDBContext.ASSET_REGISTER_ITGRC_PERIOD.Where(x => x.SKIID == periodSettingVM.SYKIID).Select(x => new { x.SKIID, x.ID }).FirstOrDefault();
                if (ExistingKIdata != null)
                {
                    periodSettingVM.Status = 3;
                    periodSettingVM.Msg = "This ki details already exist. In case of any change, please update the entry.";
                    return periodSettingVM;
                }
                else
                {
                    var operations = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.SYKI == periodSettingVM.SYKIID && x.OPERATIONID != null).Select(x => x.OPERATIONID).Distinct().ToList();

                    foreach (var item in operations)
                    {
                        var divisions = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.SYKI == periodSettingVM.SYKIID && x.OPERATIONID == item && x.DIVISIONID != null
                       && x.ADDESIGNATIONID != 1 && x.ADDESIGNATIONID != 3 && x.ADDESIGNATIONID != 7 && x.ADDESIGNATIONID != 8 && x.ADDESIGNATIONID != 9
                       && x.ADDESIGNATIONID != 13 && x.ADDESIGNATIONID != 16 && x.ADDESIGNATIONID != 18 && x.ADDESIGNATIONID != 19 && x.ADDESIGNATIONID != 20
                       && x.ADDESIGNATIONID != 21 && x.ADDESIGNATIONID != 22 && x.ADDESIGNATIONID != 23 && x.ADDESIGNATIONID != 24 && x.ADDESIGNATIONID != 25 && x.ADDESIGNATIONID != 33
                        ).Select(x => x.DIVISIONID).Distinct().ToList();

                        CultureInfo provider = CultureInfo.InvariantCulture;

                        DateTime dateTime14;
                        dateTime14 = DateTime.ParseExact(periodSettingVM.StartDate, "dd/MM/yyyy", provider);

                        DateTime dateTime15;
                        dateTime15 = DateTime.ParseExact(periodSettingVM.EndDate, "dd/MM/yyyy", provider);
                        foreach (var division_item in divisions)
                        {
                            //string ITGRC_Id1 = _arDBContext.ASSET_REGISTER_ITGRC_PERIOD.Max(x => x.ID).ToString(); //Added by aumento for Anupma san UAT
                            //long ITGRC_Id = long.Parse(ITGRC_Id1.ToString()) + 1; //Added by aumento for Anupma san UAT

                            ASSET_REGISTER_ITGRC_PERIOD up_nomination = new ASSET_REGISTER_ITGRC_PERIOD();
                            //up_nomination.ID = ITGRC_Id;//Added by aumento for Anupma san UAT
                            up_nomination.SKIID = periodSettingVM.SYKIID;
                            up_nomination.OPERATIONID = item;
                            up_nomination.DIVISIONID = division_item;
                            up_nomination.STARTDATE = dateTime14;
                            up_nomination.ENDDATE = dateTime15;
                            up_nomination.ACTIVE = 1;
                            up_nomination.CREATEDBY = (long)periodSettingVM.CreatedBy;
                            up_nomination.CREATIONDATE = DateTime.Now;
                            _arDBContext.Entry(up_nomination).State = EntityState.Added;
                            _arDBContext.SaveChanges();
                            _arDBContext.Entry(up_nomination).State = EntityState.Detached;
                            periodSettingVM.Status = 1;
                            periodSettingVM.Msg = "Data has been saved succesufully.";
                        }
                    }

                }

                return periodSettingVM;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public PeriodForITGRCMemberNominationListNewVM UpdateITGRCNomination(PeriodForITGRCMemberNominationListNewVM periodSettingVM)
        {
            ASSET_REGISTER_ITGRC_PERIOD up_nomination = new ASSET_REGISTER_ITGRC_PERIOD();
            CultureInfo provider = CultureInfo.InvariantCulture;

            DateTime dateTime14;
            dateTime14 = DateTime.ParseExact(periodSettingVM.StartDate, "dd/MM/yyyy", provider);

            DateTime dateTime15;
            dateTime15 = DateTime.ParseExact(periodSettingVM.EndDate, "dd/MM/yyyy", provider);

            up_nomination.ID = periodSettingVM.ID;
            up_nomination.SKIID = periodSettingVM.SYKIID;

            up_nomination.OPERATIONID = periodSettingVM.OPERATIONID;
            up_nomination.DIVISIONID = periodSettingVM.DIVISIONID;

            up_nomination.STARTDATE = dateTime14;
            up_nomination.ENDDATE = dateTime15;

            up_nomination.ACTIVE = 1;

            up_nomination.CREATEDBY = (long)periodSettingVM.CreatedBy;
            up_nomination.CREATIONDATE = (DateTime)periodSettingVM.CreationDate;

            up_nomination.UPDATEDBY = (long)periodSettingVM.CreatedBy;
            up_nomination.UPDATIONDATE = DateTime.Now;

            _arDBContext.Entry(up_nomination).State = EntityState.Modified;
            _arDBContext.SaveChanges();

            periodSettingVM.Status = 2;
            periodSettingVM.Msg = "Data has been update succesufully.";
            return periodSettingVM;
        }

        public BulkUpdatePeriod BulkUpdateITGRCNomination(BulkUpdatePeriod bulkUpdatePeriod)
        {
            ASSET_REGISTER_ITGRC_PERIOD up_nomination = new ASSET_REGISTER_ITGRC_PERIOD();
            up_nomination = _arDBContext.ASSET_REGISTER_ITGRC_PERIOD.Where(x => x.ID == bulkUpdatePeriod.ID).FirstOrDefault();
            if (up_nomination != null)
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime dateTime14;
                dateTime14 = DateTime.ParseExact(bulkUpdatePeriod.StartDate, "dd/MM/yyyy", provider);
                DateTime dateTime15;
                dateTime15 = DateTime.ParseExact(bulkUpdatePeriod.EndDate, "dd/MM/yyyy", provider);
                up_nomination.STARTDATE = dateTime14;
                up_nomination.ENDDATE = dateTime15;
                up_nomination.UPDATEDBY = (long)bulkUpdatePeriod.CreatedBy;
                up_nomination.UPDATIONDATE = DateTime.Now;
                _arDBContext.Entry(up_nomination).State = EntityState.Modified;
                _arDBContext.SaveChanges();
                bulkUpdatePeriod.Status = 2;
                return bulkUpdatePeriod;
            }
            else
            {
                return bulkUpdatePeriod;
            }
        }

        public int BulkUpdateAssetApplicability(AssetApplicabilityUpdateVM bulkUpdateAssetApplicability)
        {
            if (bulkUpdateAssetApplicability.SelectedIds.Count() > 0)
            {
                _arDBContext.ASSET_REGISTER_ASSETDETAILS
                        .Where(x => bulkUpdateAssetApplicability.SelectedIds.Contains(x.ID))
                        .ToList()
                        .ForEach(a =>
                        {
                            a.ISSC_MEMBER_REASON = bulkUpdateAssetApplicability.Remarks;
                            a.RETENTION_REMARK = string.Empty;
                            a.UPDATEDBY = bulkUpdateAssetApplicability.CreatedBy;
                            a.UPDATIONDATE = DateTime.Now;
                        }
                                );
                _arDBContext.SaveChanges();
                return 1;
            }
            else
            {
                return 2;
            }
        }

        public List<PeriodForITGRCMMemberNominationListVM> GetPeriodSettingITGRCList()
        {
            //List<PeriodForITGRCMMemberNominationListVM> PeriodSettingListITGRC = new List<PeriodForITGRCMMemberNominationListVM>();
            // Pre-load lookup tables once
            var sykiCache = _arDBContext.SYKI.AsNoTracking().ToList();
            var opDetailsCache = _arDBContext.VW_ASSOCIATELVLDETAILS
    .Select(x => new {
        x.OPERATIONID,
        x.SYKI,
        x.DIVISIONID,
        x.OPERATION,
        x.DIVISION,
        x.ADEMPCODE
    })
    .ToList();

            var empCache = _arDBContext.ADEMPLOYEE.Select(x => new
            {
                x.ADEMPCODE,
                x.FIRSTNAME,
                x.LASTNAME
            }).ToList();

            // Pull and project efficiently
            var PeriodSettingListITGRC = _arDBContext.ASSET_REGISTER_ITGRC_PERIOD
                .Where(x => x.ACTIVE == 1)
                .ToList() // Materialize first to avoid subqueries
                .Select((x, index) => new PeriodForITGRCMMemberNominationListVM
                {
                    SNo = index + 1,
                    ID = x.ID,
                    StartDate = x.STARTDATE.ToString(),
                    EndDate = x.ENDDATE.ToString(),
                    SYKIID = x.SKIID,
                    SYKI = sykiCache.FirstOrDefault(y => y.SYKIID == x.SKIID)?.KICODE,
                    OPERATIONID = (long)x.OPERATIONID,
                    OperationName = opDetailsCache
                        .FirstOrDefault(y => y.OPERATIONID == x.OPERATIONID && y.SYKI == x.SKIID)?.OPERATION,
                    DIVISIONID = (long)x.DIVISIONID,
                    DivisionName = opDetailsCache
                        .Where(y => y.SYKI == x.SKIID && y.DIVISIONID == x.DIVISIONID)
                        .OrderBy(y => y.ADEMPCODE)
                        .Select(y => y.DIVISION)
                        .FirstOrDefault(),
                    CreatedBy = (long)x.CREATEDBY,
                    EmpName = empCache
                        .Where(y => y.ADEMPCODE == x.CREATEDBY)
                        .Select(y => y.FIRSTNAME + " " + y.LASTNAME)
                        .FirstOrDefault(),
                    CreationDate = x.CREATIONDATE,
                    ActionType = (int)(sykiCache.FirstOrDefault(y => y.SYKIID == x.SKIID)?.ACTIVE ?? 0)
                })
                .OrderBy(x => x.OperationName)
                .ToList();

            return PeriodSettingListITGRC;
        }

        public List<PeriodForITGRCMMemberNominationListVM> GetPeriodSettingITGRCListWithID(int? id)
        {
            //List<PeriodForITGRCMMemberNominationListVM> PeriodSettingListITGRC = new List<PeriodForITGRCMMemberNominationListVM>();
            var PeriodSettingListITGRCList = _arDBContext.ASSET_REGISTER_ITGRC_PERIOD.Where(x => x.ACTIVE == 1 && x.ID == id).ToList();
            var PeriodSettingListITGRC = PeriodSettingListITGRCList.Select((x, index) => new PeriodForITGRCMMemberNominationListVM
            {
                SNo = index + 1,
                ID = (long)x.ID,
                StartDate = Convert.ToString(x.STARTDATE),
                EndDate = Convert.ToString(x.ENDDATE),
                SYKIID = (Decimal)x.SKIID,
                SYKI = _arDBContext.SYKI.Where(y => y.SYKIID == x.SKIID).Select(y => y.KICODE).FirstOrDefault(),
                OPERATIONID = (long)x.OPERATIONID,
                OperationName = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.OPERATIONID == x.OPERATIONID && y.SYKI == x.SKIID).Select(y => y.OPERATION).FirstOrDefault(),
                DIVISIONID = (long)x.DIVISIONID,
                DivisionName = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.SYKI == x.SKIID && y.DIVISIONID == x.DIVISIONID).OrderBy(y => y.ADEMPCODE).Select(y => y.DIVISION).FirstOrDefault(),
                CreatedBy = (long)x.CREATEDBY,
                EmpName = _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == x.CREATEDBY).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault(),
                CreationDate = (DateTime)x.CREATIONDATE,
                ActionType = (int)_arDBContext.SYKI.Where(y => y.SYKIID == x.SKIID).Select(y => y.ACTIVE).FirstOrDefault(),

            }).OrderBy(x => x.CreationDate)
            .ToList();
            return PeriodSettingListITGRC;
        }

        public List<OperatingHead> GetISSCMemberList(long? SYKI, long? OPERATIONID, long? Divisionid)
        {
            List<OperatingHead> emplist = new List<OperatingHead>();
            //  var empNamelist = _ohDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == 22 && x.ADDESIGNATIONID == 14 && x.OPERATIONID == 7342 && x.DIVISIONID == 7704).ToList();

            if (OPERATIONID == 0)
            {
                var EmpNameList = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.SYKIID == SYKI).ToList();
                List<string> elist = new List<string>();
                foreach (var item in EmpNameList)
                {
                    elist.Add(item.ISSCEMPCODE.ToString());
                }
                //var empdata = _ohDBContext.ADEMPLOYEE.Where(x=>elist.Contains(x.ADEMPCODE.ToString()));
                // var empNamelist1 = _ohDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE).ToList();
                var empNamelist = _arDBContext.ADEMPLOYEE.ToList();
                // emplist.Add(new OperatingHead() { Empname = "Select" });
                foreach (var item in empNamelist)
                {
                    foreach (string eitem in elist)
                    {
                        if (eitem == item.ADEMPCODE.ToString())
                        {
                            var items = new OperatingHead();
                            items.Empname = item.FIRSTNAME + " " + item.LASTNAME;
                            items.Emailid = item.EMAILID;
                            emplist.Add(items);
                            break;
                        }
                    }
                }
            }
            else
            {
                var EmpNameList = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.SYKIID == SYKI && x.OPERATIONID == (OPERATIONID == null ? x.OPERATIONID : (long)OPERATIONID) && x.DIVISIONID == (Divisionid == null ? x.DIVISIONID : (long)Divisionid)).ToList();
                List<string> elist = new List<string>();
                foreach (var item in EmpNameList)
                {
                    elist.Add(item.ISSCEMPCODE.ToString());
                }
                //var empdata = _ohDBContext.ADEMPLOYEE.Where(x=>elist.Contains(x.ADEMPCODE.ToString()));
                // var empNamelist1 = _ohDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE).ToList();
                var empNamelist = _arDBContext.ADEMPLOYEE.ToList();
                // emplist.Add(new OperatingHead() { Empname = "Select" });
                foreach (var item in empNamelist)
                {
                    foreach (string eitem in elist)
                    {
                        if (eitem == item.ADEMPCODE.ToString())
                        {
                            var items = new OperatingHead();
                            items.Empname = item.FIRSTNAME + " " + item.LASTNAME;
                            items.Emailid = item.EMAILID;
                            emplist.Add(items);
                            break;
                        }
                    }
                }
            }


            return emplist;
        }

        public List<OperatingHead> GetISSCMemberForBulkMailList(long? SYKI, string[] ids)
        {
            var lstIds= Array.ConvertAll<string, decimal>(ids, Convert.ToDecimal);
            List<OperatingHead> emplist = new List<OperatingHead>();
            var OperationIds = _arDBContext.ASSET_REGISTER_ITGRC_PERIOD.Where(x => x.SKIID == SYKI && lstIds.Contains(x.ID)).Select(x => x.OPERATIONID).Distinct().ToList();
            var DivIds = _arDBContext.ASSET_REGISTER_ITGRC_PERIOD.Where(x => x.SKIID == SYKI && lstIds.Contains(x.ID)).Select(x =>  x.DIVISIONID).Distinct().ToList();
            var EmpNameList = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.SYKIID == SYKI && OperationIds.Contains(x.OPERATIONID)
            && DivIds.Contains(x.DIVISIONID)).ToList();
                List<string> elist = new List<string>();
                foreach (var item in EmpNameList)
                {
                    elist.Add(item.ISSCEMPCODE.ToString());
                }
                var empNamelist = _arDBContext.ADEMPLOYEE.ToList();
                foreach (var item in empNamelist)
                {
                    foreach (string eitem in elist)
                    {
                        if (eitem == item.ADEMPCODE.ToString())
                        {
                            var items = new OperatingHead();
                            items.Empname = item.FIRSTNAME + " " + item.LASTNAME;
                            items.Emailid = item.EMAILID;
                            emplist.Add(items);
                            break;
                        }
                    }
                }

            return emplist;
        }

        public List<AssetType> GetAssetList()
        {
            List<AssetType> AssetTypeList = new List<AssetType>();
            var AssetList = _arDBContext.ASSET_REGISTER_TYPES.Where(x => x.STATUS == 1).ToList();

            List<string> list = new List<string>();
            foreach (var item in AssetList)
            {
                list.Add(item.ASSETID.ToString());
            }

            var Namelist = _arDBContext.ASSET_REGISTER_TYPES.ToList();
            //AssetTypeList.Add(new AssetType() { ASSETNAME = "Select" });
            foreach (var item in Namelist)
            {
                foreach (string aitem in list)
                {
                    if (aitem == item.ASSETID.ToString())
                    {
                        var items = new AssetType();
                        items.ASSETNAME = item.ASSETNAME;
                        items.AssetID = item.ASSETID;
                        AssetTypeList.Add(items);
                        break;
                    }
                }

            }
            return AssetTypeList;
        }

        public List<AssetClassification> GetAssetClassificationList()
        {
            List<AssetClassification> AssetClassificationList = new List<AssetClassification>();
            var AssetList = _arDBContext.ASSET_REGISTER_CLASSIFICATION.Where(x => x.STATUS == 1).ToList();

            List<string> list = new List<string>();
            foreach (var item in AssetList)
            {
                list.Add(item.CLASSIFICATIONID.ToString());
            }

            var Namelist = _arDBContext.ASSET_REGISTER_CLASSIFICATION.ToList();
            //AssetClassificationList.Add(new AssetClassification() { CLASSIFICATION = "Select" });
            foreach (var item in Namelist)
            {
                foreach (string aitem in list)
                {
                    if (aitem == item.CLASSIFICATIONID.ToString())
                    {
                        var items = new AssetClassification();
                        items.CLASSIFICATION = item.CLASSIFICATION;
                        items.CLASSIFICATIONID = item.CLASSIFICATIONID;
                        AssetClassificationList.Add(items);
                        break;
                    }
                }

            }
            return AssetClassificationList;
        }

        //public List<AssetClassification> GetAssetClassificationList()
        //{
        //    List<AssetClassification> AssetClassificationList = new List<AssetClassification>();
        //    //AssetClassificationList = _arDBContext.ASSET_REGISTER_CLASSIFICATION.Where(x => x.STATUS == 1).ToList();
        //    return AssetClassificationList;
        //}

        public CommonAssetsRegListVM InsertUpdateCommonAssetRegisterSettingDetail(CommonAssetsRegListVM periodSettingVM)
        {
            var ExistingKIdata = _arDBContext.ASSET_REGISTER_COMMON_ASSET.Where(x => x.PRIMARY == periodSettingVM.Primary && x.ACTIVE == 1 && x.CLASSIFICATIONID == periodSettingVM.ClassificationID).Select(x => x).FirstOrDefault();
            if (ExistingKIdata != null && ExistingKIdata.ID > 0 && periodSettingVM.Active == 0)
            {
                periodSettingVM.Status = 3;
                periodSettingVM.Msg = "This primary asset is already exist. In Case of any change, please update the entry.";
                return periodSettingVM;
            }
            else
            {
                string ID1 = _arDBContext.ASSET_REGISTER_COMMON_ASSET.Max(x => x.ID).ToString(); //Added by aumento for Issue No 195057 Testing as on 08072024
                decimal commonasset_entry_Id12 = decimal.Parse(ID1.ToString()) + 1; //Added by aumento for Issue No 195057 Testing as on 08072024
                ASSET_REGISTER_COMMON_ASSET up_nomination = new ASSET_REGISTER_COMMON_ASSET();

                // up_nomination.SKIID = periodSettingVM.SYKIID;
                up_nomination.ID = commonasset_entry_Id12;
                up_nomination.PRIMARY = periodSettingVM.Primary;
                //up_nomination.SECONDARY = periodSettingVM.Secondary;

                //up_nomination.ASSETID = periodSettingVM.AssetsID;
                up_nomination.CLASSIFICATIONID = periodSettingVM.ClassificationID;
                up_nomination.CREATEDBY = (decimal)periodSettingVM.CreatedBy;
                up_nomination.CREATIONDATE = DateTime.Now;

                // up_nomination.UPDATEDBY = (decimal)periodSettingVM.CreatedBy;
                // up_nomination.UPDATIONDATE = DateTime.Now;

                up_nomination.ACTIVE = periodSettingVM.Active;

                _arDBContext.Entry(up_nomination).State = EntityState.Added;
                _arDBContext.SaveChanges();

                periodSettingVM.Status = 1;
                periodSettingVM.Msg = "Data has been saved succesufully.";
            }
            return periodSettingVM;
        }

        public CommonAssetsRegListVM UpdateCommonAssetRegister(CommonAssetsRegListVM periodSettingVM)
        {
            ASSET_REGISTER_COMMON_ASSET up_nomination = new ASSET_REGISTER_COMMON_ASSET();

            up_nomination.ID = periodSettingVM.ID;
            // up_nomination.SKIID = periodSettingVM.SYKIID;

            up_nomination.PRIMARY = periodSettingVM.Primary;
            //up_nomination.SECONDARY = periodSettingVM.Secondary;

            // up_nomination.ASSETID = periodSettingVM.AssetsID;
            up_nomination.CLASSIFICATIONID = periodSettingVM.ClassificationID;

            up_nomination.ACTIVE = periodSettingVM.Active;

            up_nomination.CREATEDBY = (long)periodSettingVM.CreatedBy;
            up_nomination.CREATIONDATE = (DateTime)periodSettingVM.CreationDate;

            up_nomination.UPDATEDBY = (long)periodSettingVM.CreatedBy;
            up_nomination.UPDATIONDATE = DateTime.Now;

            _arDBContext.Entry(up_nomination).State = EntityState.Modified;
            _arDBContext.SaveChanges();

            periodSettingVM.Status = 2;
            periodSettingVM.Msg = "Data has been update succesufully.";
            return periodSettingVM;
        }

        public List<CommonAAssetsRegListVM> Get_CommonAsset_Register_List()
        {
            //List<PeriodForITGRCMMemberNominationListVM> PeriodSettingListITGRC = new List<PeriodForITGRCMMemberNominationListVM>();
            var PeriodSettingList = _arDBContext.ASSET_REGISTER_COMMON_ASSET.ToList().Select((x, index) => new CommonAAssetsRegListVM
            {
                SNo = index + 1,
                ID = (long)x.ID,
                //SYKIName = _arDBContext.SYKI.Where(x1 => x1.ACTIVE==1).Select(x1 => x1.KICODE).FirstOrDefault(),
                Primary = x.PRIMARY,
                //Secondary = x.SECONDARY,
                // AssetsID = (decimal)x.ASSETID,
                ClassificationID = (decimal)x.CLASSIFICATIONID,
                // AssetsType = _arDBContext.ASSET_REGISTER_TYPES.Where(y => y.ASSETID == x.ASSETID).Select(y => y.ASSETNAME).FirstOrDefault(),
                // Classification = _arDBContext.ASSET_REGISTER_CLASSIFICATION.Where(y => y.CLASSIFICATIONID == x.CLASSIFICATIONID).Select(y => y.CLASSIFICATION).FirstOrDefault(),
                Active = (decimal)x.ACTIVE,
                //CreatedBy = (long)x.CREATEDBY,
                //EmpName = _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == x.CREATEDBY).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault(),
                //CreationDate = (DateTime)x.CREATIONDATE,
                //ActionType = (int)_arDBContext.SYKI.Where(y => y.SYKIID == x.SKIID).Select(y => y.ACTIVE).FirstOrDefault(),

            }).OrderBy(x => x.Primary)
            .ToList();
            return PeriodSettingList;
        }

        //public List<CommonAAssetsRegListVM> Get_CommonAsset_Register_WithID(int? id)
        //{
        //    //List<PeriodForITGRCMMemberNominationListVM> PeriodSettingListITGRC = new List<PeriodForITGRCMMemberNominationListVM>();
        //    var PeriodSettingList = _arDBContext.ASSET_REGISTER_COMMON_ASSET.Where(x => x.ACTIVE == 1 && x.ID == id).Select((x, index) => new CommonAAssetsRegListVM
        //    {
        //        SNo = index + 1,
        //        ID = (long)x.ID,
        //        Primary = x.PRIMARY,
        //        Secondary = x.SECONDARY,
        //       // AssetsType = _arDBContext.ASSET_REGISTER_TYPES.Where(y => y.ASSETID == x.ASSETID).Select(y => y.ASSETNAME).FirstOrDefault(),
        //        Classification = _arDBContext.ASSET_REGISTER_CLASSIFICATION.Where(y => y.CLASSIFICATIONID == x.CLASSIFICATIONID).Select(y => y.CLASSIFICATION).FirstOrDefault(),
        //        Active = (decimal)x.ACTIVE,
        //        CreatedBy = (long)x.CREATEDBY,
        //        EmpName = _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == x.CREATEDBY).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault(),
        //        CreationDate = (DateTime)x.CREATIONDATE,
        //        ActionType = (int)_arDBContext.SYKI.Where(y => y.SYKIID == x.SKIID).Select(y => y.ACTIVE).FirstOrDefault(),

        //    }).OrderBy(x => x.CreationDate)
        //    .ToList();
        //    return PeriodSettingList;
        //}

        public Get_Division_ISSC_Member Get_Division_For_ISSC_Member(int? id)
        {
            //List<PeriodForITGRCMMemberNominationListVM> PeriodSettingListITGRC = new List<PeriodForITGRCMMemberNominationListVM>();
            string D_Head = string.Empty; string O_Head = string.Empty;
            var SYKIList = (from data in _arDBContext.SYKI.Where(x => x.ACTIVE == 1)
                            select data).FirstOrDefault();
            var empNamelist_old = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == SYKIList.SYKIID && x.ADEMPCODE == id).Select(x => new { x.OPERATION, x.DIVISIONID, x.DIVISION }).FirstOrDefault();

            //Added by Aumento for SR91196
            string ITGRC_Head = string.Empty;
            string ITGRC_code = GetITGRCValue();

            long? DivHeadCode = 0; // Added By Aumento :: SR102194
            decimal? OPHeadCode = 0; // Added By Aumento :: SR102194

            long ITGRC_Ecode = Convert.ToInt64(ITGRC_code);

            if (ITGRC_code != null)
            {
                var ITGRC_ = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == ITGRC_Ecode).Select(x => new { Name = x.FIRSTNAME + " " + x.LASTNAME }).FirstOrDefault();
                if (ITGRC_ != null)
                {
                    ITGRC_Head = ITGRC_.Name;
                }
            }
            //Added by Aumento for SR91196
            if (empNamelist_old != null)
            {
                var empNamelist = _arDBContext.ADORGLEVELHEAD.Where(x => x.ISACTIVE == 1 && x.ADORGLEVELID == empNamelist_old.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                DivHeadCode = empNamelist; // Added By Aumento :: SR102194

                var D_HeadName = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == empNamelist).Select(x => new { Name = x.FIRSTNAME + " " + x.LASTNAME }).FirstOrDefault();
                if (D_HeadName != null)
                {
                    D_Head = D_HeadName.Name;
                }

                var ohead_id = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.SYKIID == SYKIList.SYKIID && x.DIVISIONID == empNamelist_old.DIVISIONID && x.ACTIVE == 1).Select(x => x.ADDEDBY).FirstOrDefault();
                OPHeadCode = ohead_id; // Added By Aumento :: SR102194

                var O_HeadName = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == ohead_id).Select(x => new { Name = x.FIRSTNAME + " " + x.LASTNAME }).FirstOrDefault();

                if (O_HeadName != null)
                {
                    O_Head = O_HeadName.Name;
                }
               
               
            }

            var PeriodSettingList = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == id && x.SYKI == SYKIList.SYKIID).Select(x => new Get_Division_ISSC_Member
            {
                DivId = x.DIVISIONID,
                DivisionName = x.DIVISION,
                //ISSCMember =Convert.ToString(_arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x1 => x1.SYKIID == SYKIList.SYKIID && x1.DIVISIONID == x.DIVISIONID).Select(x1=>x1.ISSCEMPCODE).FirstOrDefault()),
                ISSCMember = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x1 => x1.SYKIID == SYKIList.SYKIID && x1.DIVISIONID == x.DIVISIONID).Count() > 0 ? _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(p1 => p1.SYKIID == SYKIList.SYKIID && p1.DIVISIONID == x.DIVISIONID).Select(p1 => p1.ISSCEMPCODE).FirstOrDefault()).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault() : null,
                EndDate = _arDBContext.ASSET_REGISTER_ITGRC_PERIOD.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.DIVISIONID == x.DIVISIONID).Select(x1 => x1.ENDDATE).FirstOrDefault(),
                // START :: Added By Aumento :: SR102194 replaced id with ADEMPCODE
                ISSC_Member_Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.FINALSUBMITDATE_ISSCMEMBER).FirstOrDefault(),
                ISSC_Member_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.ISSCMEMBER_STATUS).FirstOrDefault(),

                DivisionHead_Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.FINALSUBMITDATE_DIVISIONHEAD).FirstOrDefault(),
                DivisionHead_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.DIVISION_STATUS).FirstOrDefault(),

                OperatingHead__Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.FINALSUBMITDATE_OPERATINGHEAD).FirstOrDefault(),
                OperatingHead_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.OPERATIONG_STATUS).FirstOrDefault(),

                //Added by Aumento for SR91196
                ITGRCHead_Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.FINALSUBMITDATE_ITGRCHEAD).FirstOrDefault(),
                ITGRCHead_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.ITGRC_STATUS).FirstOrDefault(),
                // END :: Added By Aumento :: SR102194

                ITGRC_Head_Name = ITGRC_Head,
                //Div_Head_EmpCode = DivHeadCode, // Added By Aumento :: SR102194
                Div_Head_EmpCode = ((DivHeadCode == null || DivHeadCode == 0) ? OPHeadCode : DivHeadCode), // Added By Aumento :: SR102715
                Op_Head_EmpCode = OPHeadCode, // Added By Aumento :: SR102194
                //Added by Aumento for SR91196
                //DivisionHead_Name = D_Head,
                DivisionHead_Name = ((DivHeadCode == null || DivHeadCode == 0) ? O_Head : D_Head),  // Added By Aumento :: SR102715
                OperatingHead_Name = O_Head,
                // IS_ITOPRATION = IsOperationMatch(id)// Added By Aumento :: SR102194
            }).FirstOrDefault();
            if (PeriodSettingList !=null)
            {
                PeriodSettingList.IS_ITOPRATION = IsOperationMatch(id);//
            } return PeriodSettingList;
        }

        public List<Primary_AssetDeatils_VM> Get_Primary_Asset(long Userid, long? SYKI)
        {
            var s = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == Userid && x.SKIID == SYKI && x.ACTIVE == 1).Distinct().Select(x => new Primary_AssetDeatils_VM { Id = x.PRIMARY, PrimaryAsset = x.PRIMARY }).Distinct().OrderBy(x => x.PrimaryAsset).ToList();
            return s;
        }

        public List<CommonAAssetsRegListVM> Get_Asset_Details_With_Common_Asset(long Userid, long? SYKI)
        {
            var div = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == Userid && x.SYKI == SYKI).Select(x => x.DIVISIONID).FirstOrDefault();
            var datecheck = _arDBContext.ASSET_REGISTER_ITGRC_PERIOD.Where(x1 => x1.SKIID == SYKI && x1.DIVISIONID == div).Select(x => x).FirstOrDefault();
            if (datecheck != null)
            {
                var common_asset_exist_for_ki = _arDBContext.ASSET_REGISTER_COMMONEXIST.Where(x => x.ACTIVE == 1 && x.SYKIID == SYKI && x.USERID == Userid && x.ACTIONTYPE == 1).Count();
                if (common_asset_exist_for_ki == 0 && Userid > 0)
                {
                    var common_asset = _arDBContext.ASSET_REGISTER_COMMON_ASSET.Where(x => x.ACTIVE == 1).Select(x => x.PRIMARY).ToList();
                    //var s = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == Userid && x.SKIID == SYKI && x.ACTIONTYPE == 1).Select(x => x.PRIMARY).ToList();
                    //common_asset = common_asset.Where(x => !s.Contains(x)).ToList();
                    using (var transaction = _arDBContext.Database.BeginTransaction())
                    {
                        try
                        {
                            foreach (var item in common_asset)
                            {
                                var common_data = _arDBContext.ASSET_REGISTER_COMMON_ASSET.Where(x => x.ACTIVE == 1 && x.PRIMARY.Contains(item)).Select(x => x).FirstOrDefault();
                                string ID1 = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Max(x => x.ID).ToString(); //Added by aumento for Issue No 195057 Testing as on 08072024
                                long commonasset_entry_Id = long.Parse(ID1.ToString()) + 1; //Added by aumento for Issue No 195057 Testing as on 08072024
                                ASSET_REGISTER_ASSETDETAILS ass_det = new ASSET_REGISTER_ASSETDETAILS();
                                ass_det.ID = commonasset_entry_Id;//Added by aumento for Issue No 195057 Testing as on 08072024
                                ass_det.SKIID = (decimal)SYKI;
                                ass_det.PRIMARY = common_data.PRIMARY;
                                ass_det.ASSETID = common_data.ASSETID;
                                ass_det.CLASSIFICATIONID = common_data.CLASSIFICATIONID;
                                ass_det.CREATEDBY = Userid;
                                ass_det.CREATIONDATE = DateTime.Now;
                                ass_det.ACTIONTYPE = 1; // Common Asset
                                ass_det.ACTIVE = 1;
                                ass_det.SAVE = 1;// Save as draft 1
                                _arDBContext.Entry(ass_det).State = EntityState.Added;
                                _arDBContext.SaveChanges();
                                _arDBContext.Entry(ass_det).State = EntityState.Detached;
                            }

                           
                            string ID = _arDBContext.ASSET_REGISTER_COMMONEXIST.Max(x => x.ID).ToString(); //Added by aumento for Issue No 195057 Testing as on 08072024
                            decimal entry_Id = decimal.Parse(ID.ToString()) + 1; //Added by aumento for Issue No 195057 Testing as on 08072024
                            ASSET_REGISTER_COMMONEXIST commonasset_entry = new ASSET_REGISTER_COMMONEXIST();
                            commonasset_entry.ID = entry_Id;
                            commonasset_entry.SYKIID = (decimal)SYKI;
                            commonasset_entry.USERID = Userid;
                            commonasset_entry.ACTIVE = 1;
                            commonasset_entry.CREATIONDATE = DateTime.Now;
                            commonasset_entry.ACTIONTYPE = 1;
                            _arDBContext.Entry(commonasset_entry).State = EntityState.Added;
                            _arDBContext.SaveChanges();

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                        }
                    }
                }
            }
            var assets = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == Userid && x.SKIID == SYKI && x.ACTIVE == 1).Select(x => new CommonAAssetsRegListVM
            {
                ID = x.ID,
                Primary = x.PRIMARY,
                Secondary = x.SECONDARY,
                AssetsID = x.ASSETID,
                Assetlocation = x.LOCATION,
                AssetOwner = x.OWNER,
                Custodian = x.CUSTODIAN,
                AssetUser = x.ASSETUSER,
                ClassificationID = (decimal)x.CLASSIFICATIONID,
                Retention_Period = x.RETENTION,
                ActionType = x.ACTIONTYPE,
                Reason_NA = x.ISSC_MEMBER_REASON,
                Retention_Remarks = x.RETENTION_REMARK,
                AMENDMENT_NO = x.AMENDMENT_NO, //Added By Aumento as on 19042024
                FINALSUBMITDATE_ISSCMEMBER = x.FINALSUBMITDATE_ISSCMEMBER // Added by TTL ::  SR103777 > CR6964 
            }).OrderByDescending(x => x.ActionType).ToList();
            return assets;
        }
        public List<CommonAAssetsRegListVM> Get_Last_Ki_Asset_Details(long? Userid, long? SYKI)
        {
            List<CommonAAssetsRegListVM> Ilist = new List<CommonAAssetsRegListVM>(); // Added By Aumento :: SR102194
            // var lastki = SYKI - 1;
            var get_prevyear_detail = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == Userid && x.SYKI == SYKI).Select(x => new { x.OPERATIONID, x.DIVISIONID }).FirstOrDefault();
            //var get_prevyear_detail = _arDBContext.ASSET_REGISTER_ORG_MAPPING.Where(x => x.CURRENT_SYKIID == SYKI && x.CURRENT_OPERATIONID == curent_year_deatil.OPERATIONID && x.CURRENT_DIVISIONID == curent_year_deatil.DIVISIONID && x.STATUS == 1).Select(x => x).FirstOrDefault();

            if (get_prevyear_detail != null) // Added By Aumento :: SR102194
            { // Added By Aumento :: SR102194

                //var get_prevyear_detail = _arDBContext.ASSET_REGISTER_ORG_MAPPING.Where(x => x.CURRENT_SYKIID == SYKI && x.CURRENT_OPERATIONID == curent_year_deatil.OPERATIONID && x.CURRENT_DIVISIONID == curent_year_deatil.DIVISIONID && x.STATUS == 1).Select(x => x).FirstOrDefault();
                var get_prevyear_userid = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.STATUS == 1 && x.SYKIID == SYKI && x.OPERATIONID == get_prevyear_detail.OPERATIONID && x.DIVISIONID == get_prevyear_detail.DIVISIONID).Select(x => x.ISSCEMPCODE).FirstOrDefault();
                //var assets = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == get_prevyear_userid && x.SKIID == SYKI && x.ACTIVE == 1 &&
                Ilist = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == get_prevyear_userid && x.SKIID == SYKI && x.ACTIVE == 1 &&    // Added By Aumento :: SR102194 Replaced Ilist with assets
                x.OPERATIONG_STATUS == 1).Select(x => new CommonAAssetsRegListVM

                {
                    ID = x.ID,
                    Primary = x.PRIMARY,
                    Secondary = x.SECONDARY,
                    AssetsID = (decimal)x.ASSETID,
                    Assetlocation = x.LOCATION,
                    AssetOwner = x.OWNER,
                    Custodian = x.CUSTODIAN,
                    AssetUser = x.ASSETUSER,
                    ClassificationID = (decimal)x.CLASSIFICATIONID,
                    Retention_Period = x.RETENTION
                }).ToList();

            } // Added By Aumento :: SR102194
            ///return assets; // Added By Aumento :: SR102194
            return Ilist; // Added By Aumento :: SR102194
        }
        public List<CommonAAssetsRegListVM> Get_Last_Ki_Asset_DetailsNew(long? Userid, long? SYKI, long? DivId)
        {
            // var lastki = SYKI - 1;
            var get_Vm_prevyear_detail = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == Userid && x.SYKI == SYKI).ToList();
            var get_prevyear_detail  =  get_Vm_prevyear_detail.Select(x => new { x.OPERATIONID, x.DIVISIONID }).FirstOrDefault();
            //var get_prevyear_detail = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == Userid && x.SYKI == SYKI && x.DIVISIONID == DivId).Select(x => new { x.OPERATIONID, x.DIVISIONID }).FirstOrDefault();
            //var get_prevyear_detail = _arDBContext.ASSET_REGISTER_ORG_MAPPING.Where(x => x.CURRENT_SYKIID == SYKI && x.CURRENT_OPERATIONID == curent_year_deatil.OPERATIONID && x.CURRENT_DIVISIONID == curent_year_deatil.DIVISIONID && x.STATUS == 1).Select(x => x).FirstOrDefault();
            
            var get_ARMprevyear_userid = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.STATUS == 1 && x.SYKIID == SYKI && x.OPERATIONID == get_prevyear_detail.OPERATIONID && x.DIVISIONID == get_prevyear_detail.DIVISIONID).ToList();
            var get_prevyear_userid= get_ARMprevyear_userid.Select(x => x.ISSCEMPCODE).FirstOrDefault();
            var assets = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == get_prevyear_userid && x.SKIID == SYKI && x.ACTIVE == 1 && x.OPERATIONG_STATUS == 1).Select(x => new CommonAAssetsRegListVM
            {
                ID = x.ID,
                Primary = x.PRIMARY,
                Secondary = x.SECONDARY,
                AssetsID = (decimal)x.ASSETID,
                Assetlocation = x.LOCATION,
                AssetOwner = x.OWNER,
                Custodian = x.CUSTODIAN,
                AssetUser = x.ASSETUSER,
                ClassificationID = (decimal)x.CLASSIFICATIONID,
                Retention_Period = x.RETENTION
            }).ToList();
            return assets;
        }

        public List<CommonAAssetsRegListVM> Get_Last_Ki_Asset_DetailsOperationID(long? Userid, long? SYKI, long? OPID)
        {
            // var lastki = SYKI - 1;
            var get_prevyear_detail = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == Userid && x.SYKI == SYKI).Select(x => new { x.OPERATIONID, x.DIVISIONID }).FirstOrDefault();
            //var get_prevyear_detail = _arDBContext.ASSET_REGISTER_ORG_MAPPING.Where(x => x.CURRENT_SYKIID == SYKI && x.CURRENT_OPERATIONID == curent_year_deatil.OPERATIONID && x.CURRENT_DIVISIONID == curent_year_deatil.DIVISIONID && x.STATUS == 1).Select(x => x).FirstOrDefault();
            var get_prevyear_userid = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.STATUS == 1 && x.SYKIID == SYKI && x.OPERATIONID == get_prevyear_detail.OPERATIONID).Select(x => x.ISSCEMPCODE).FirstOrDefault();
            var assets = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == get_prevyear_userid && x.SKIID == SYKI && x.ACTIVE == 1 && x.OPERATIONG_STATUS == 1).Select(x => new CommonAAssetsRegListVM
            {
                ID = x.ID,
                Primary = x.PRIMARY,
                Secondary = x.SECONDARY,
                AssetsID = (decimal)x.ASSETID,
                Assetlocation = x.LOCATION,
                AssetOwner = x.OWNER,
                Custodian = x.CUSTODIAN,
                AssetUser = x.ASSETUSER,
                ClassificationID = (decimal)x.CLASSIFICATIONID,
                Retention_Period = x.RETENTION
            }).ToList();
            return assets;
        }


        public int Last_year_Asset_Detail_Only_Non_CommonAsset(long Userid, long? SYKI)
        {
            if (Userid > 0 && SYKI > 0)
            {
                var common_asset_exist_for_ki = _arDBContext.ASSET_REGISTER_COMMONEXIST.Where(x => x.ACTIVE == 1 && x.SYKIID == SYKI && x.USERID == Userid && x.ACTIONTYPE == 2).Count();
                if (common_asset_exist_for_ki == 0)
                {
                    var curent_year_deatil = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == Userid && x.SYKI == SYKI).Select(x => new { x.OPERATIONID, x.DIVISIONID }).FirstOrDefault();
                    if (curent_year_deatil != null)
                    {
                        var get_prevyear_detail = _arDBContext.ASSET_REGISTER_ORG_MAPPING.Where(x => x.CURRENT_SYKIID == SYKI && x.CURRENT_OPERATIONID == curent_year_deatil.OPERATIONID && x.CURRENT_DIVISIONID == curent_year_deatil.DIVISIONID && x.STATUS == 1).Select(x => x).FirstOrDefault();
                        if (get_prevyear_detail != null)
                        {
                            var get_prevyear_userid = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.STATUS == 1 && x.SYKIID == get_prevyear_detail.PREVIOUS_SYKIID && x.OPERATIONID == get_prevyear_detail.PREVIOUS_OPERATIONID && x.DIVISIONID == get_prevyear_detail.PREVIOUS_DIVISIONID).Select(x => x.ISSCEMPCODE).FirstOrDefault();
                            var asset_details = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.ACTIVE == 1 && x.CREATEDBY == get_prevyear_userid && x.ACTIONTYPE == 0 && x.OPERATIONG_STATUS == 1 && x.SKIID == (SYKI - 1)).Select(x => x).ToList();

                            using (var transaction = _arDBContext.Database.BeginTransaction())
                            {
                                try
                                {
                                    List<ASSET_REGISTER_ASSETDETAILS> ass_det_List = new List<ASSET_REGISTER_ASSETDETAILS>();
                                    var lastgerneratedid = _arDBContext.ASSET_REGISTER_ASSETDETAILS.OrderByDescending(x => x.ID).Select(x => x.ID).FirstOrDefault();
                                    foreach (var item in asset_details)
                                    {
                                        lastgerneratedid++;
                                        ASSET_REGISTER_ASSETDETAILS ass_det = new ASSET_REGISTER_ASSETDETAILS();
                                        ass_det.ID = lastgerneratedid;
                                        ass_det.SKIID = (decimal)SYKI;
                                        ass_det.PRIMARY = item.PRIMARY;
                                        ass_det.SECONDARY = item.SECONDARY;

                                        ass_det.ASSETID = item.ASSETID;
                                        ass_det.LOCATION = item.LOCATION;
                                        ass_det.OWNER = item.OWNER;
                                        ass_det.CUSTODIAN = item.CUSTODIAN;
                                        ass_det.ASSETUSER = item.ASSETUSER;

                                        ass_det.CLASSIFICATIONID = item.CLASSIFICATIONID;
                                        ass_det.RETENTION = item.RETENTION;
                                        ass_det.CREATEDBY = Userid;
                                        ass_det.CREATIONDATE = DateTime.Now;
                                        ass_det.ACTIONTYPE = 0; // Normal Asset
                                        ass_det.ACTIVE = 1;
                                        ass_det.SAVE = 1;// Save as draft 1
                                        ass_det_List.Add(ass_det);

                                    }

                                    if (ass_det_List.Any())
                                    {
                                        _arDBContext.ASSET_REGISTER_ASSETDETAILS.AddRange(ass_det_List);
                                       string ARC_ID = _arDBContext.ASSET_REGISTER_COMMONEXIST.Max(x => x.ID).ToString(); //Added by aumento for Issue No 195057 Testing as on 08072024
                                        long commonasset_entry_Id = long.Parse(ARC_ID.ToString()) + 1; //Added by aumento for Issue No 195057 Testing as on 08072024
                                        ASSET_REGISTER_COMMONEXIST commonasset_entry = new ASSET_REGISTER_COMMONEXIST();
                                        commonasset_entry.ID = commonasset_entry_Id; //Added by aumento for Issue No 195057 Testing as on 08072024 
                                        commonasset_entry.SYKIID = (decimal)SYKI;
                                        commonasset_entry.USERID = Userid;
                                        commonasset_entry.ACTIVE = 1;
                                        commonasset_entry.CREATIONDATE = DateTime.Now;
                                        commonasset_entry.ACTIONTYPE = 2;
                                        _arDBContext.Entry(commonasset_entry).State = EntityState.Added;
                                        _arDBContext.SaveChanges();
                                        transaction.Commit();

                                        return 1;// Data imported successfully.
                                    }
                                    else
                                        return 4; // Data can not imported contact to ISMS Team.
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    return 2;
                                }
                            }
                        }
                        return 2;
                    }
                    return 2;
                }
                return 3;//Data already imported
            }
            return 2;//There is some problem, please try again later.
        }

        public int Insert_AssetDetails(CommonAAssetsRegListVM assetdetails)
        {
            if (assetdetails != null && assetdetails.CreatedBy > 0 && assetdetails.SYKIID > 0 && !string.IsNullOrWhiteSpace(assetdetails.PrimaryId))
            {
                if (assetdetails.ID == 0)
                {
                    ASSET_REGISTER_ASSETDETAILS ass_det = new ASSET_REGISTER_ASSETDETAILS();

                    string Ass_RegId1 = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Max(x => x.ID).ToString(); //Added by aumento for Anupma san UAT
                    long Ass_RegId = long.Parse(Ass_RegId1.ToString()) + 1; //Added by aumento for Anupma san UAT

                    ass_det.ID = Ass_RegId;//Added by aumento for Anupma san UAT
                    ass_det.SKIID = assetdetails.SYKIID;
                    ass_det.PRIMARY = assetdetails.PrimaryId == "-1" ? assetdetails.Primarytxt : assetdetails.PrimaryId;
                    ass_det.SECONDARY = assetdetails.Secondary;

                    ass_det.ASSETID = assetdetails.AssetsID;
                    ass_det.LOCATION = assetdetails.Assetlocation;

                    ass_det.OWNER = assetdetails.AssetOwner;
                    ass_det.CUSTODIAN = assetdetails.Custodian;
                    ass_det.ASSETUSER = assetdetails.AssetUser;

                    ass_det.CLASSIFICATIONID = assetdetails.ClassificationID;
                    ass_det.RETENTION = assetdetails.Retention_Month + "_" + assetdetails.Retention_Year;
                    ass_det.RETENTION_REMARK = assetdetails.Retention_Remarks;
                    ass_det.CREATEDBY = assetdetails.CreatedBy;
                    ass_det.CREATIONDATE = DateTime.Now;
                    ass_det.ACTIONTYPE = 0; // Normal Asset
                    ass_det.ACTIVE = 1;
                    ass_det.SAVE = 1;// Save as draft 1
                    _arDBContext.Entry(ass_det).State = EntityState.Added;
                    _arDBContext.SaveChanges();
                    return 1;
                }
                else
                {
                    var ass_det = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.ID == assetdetails.ID).FirstOrDefault();
                    if (ass_det != null)
                    {
                        ass_det.SKIID = assetdetails.SYKIID;
                        ass_det.PRIMARY = assetdetails.PrimaryId;
                        ass_det.SECONDARY = assetdetails.Secondary;

                        ass_det.ASSETID = assetdetails.AssetsID;
                        ass_det.LOCATION = assetdetails.Assetlocation;

                        ass_det.OWNER = assetdetails.AssetOwner;
                        ass_det.CUSTODIAN = assetdetails.Custodian;
                        ass_det.ASSETUSER = assetdetails.AssetUser;

                        ass_det.CLASSIFICATIONID = assetdetails.ClassificationID != 0 ? assetdetails.ClassificationID : assetdetails.Classification_Edit_ID;
                        ass_det.RETENTION = assetdetails.Retention_Month + "_" + assetdetails.Retention_Year;
                        ass_det.RETENTION_REMARK = assetdetails.Retention_Remarks;
                        ass_det.ISSC_MEMBER_REASON = assetdetails.ISSC_Member_Reason;
                        ass_det.CREATEDBY = ass_det.CREATEDBY;
                        ass_det.CREATIONDATE = ass_det.CREATIONDATE;

                        ass_det.UPDATEDBY = assetdetails.CreatedBy;
                        ass_det.UPDATIONDATE = DateTime.Now;
                        ass_det.ACTIONTYPE = ass_det.ACTIONTYPE;
                        ass_det.ACTIVE = 1;
                        _arDBContext.Entry(ass_det).State = EntityState.Modified;
                        _arDBContext.SaveChanges();
                        return 1;
                    }
                    return 2;

                }
            }
            return 2;
        }
        public CommonAAssetsRegListVM Get_Edit_Asset_Details(int? Id)
        {
            //CommonAAssetsRegListVM s = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.ID == Id).Select(x => new CommonAAssetsRegListVM
            //{
            //    ID = x.ID,
            //    PrimaryId = x.PRIMARY,
            //    Secondary = x.SECONDARY,
            //    AssetsID = x.ASSETID,
            //    Assetlocation = x.LOCATION,
            //    AssetOwner = x.OWNER,
            //    Custodian = x.CUSTODIAN,
            //    AssetUser = x.ASSETUSER,
            //    ClassificationID = (decimal)x.CLASSIFICATIONID,
            //    Classification_Edit_ID = (decimal)x.CLASSIFICATIONID,
            //    Retention_Month = !string.IsNullOrEmpty(x.RETENTION) ? x.RETENTION.Split('_')[0] : x.RETENTION,
            //    Retention_Year = x.RETENTION != null ? x.RETENTION.Split('_')[1] : x.RETENTION,
            //    ActionType = x.ACTIONTYPE,
            //    ISSC_Member_Reason = x.ISSC_MEMBER_REASON,
            //    Retention_Remarks = x.RETENTION_REMARK
            //}).FirstOrDefault();
            var assetDetails = _arDBContext.ASSET_REGISTER_ASSETDETAILS
    .Where(x => x.ID == Id)
    .Select(x => new
    {
        x.ID,
        x.PRIMARY,
        x.SECONDARY,
        x.ASSETID,
        x.LOCATION,
        x.OWNER,
        x.CUSTODIAN,
        x.ASSETUSER,
        ClassificationID = (decimal)x.CLASSIFICATIONID,
        Classification_Edit_ID = (decimal)x.CLASSIFICATIONID,
        Retention = x.RETENTION,
        x.ACTIONTYPE,
        x.ISSC_MEMBER_REASON,
        x.RETENTION_REMARK
    })
    .FirstOrDefault();

            CommonAAssetsRegListVM s = null;
            if (assetDetails != null)
            {
                s = new CommonAAssetsRegListVM
                {
                    ID = assetDetails.ID,
                    PrimaryId = assetDetails.PRIMARY,
                    Secondary = assetDetails.SECONDARY,
                    AssetsID = assetDetails.ASSETID,
                    Assetlocation = assetDetails.LOCATION,
                    AssetOwner = assetDetails.OWNER,
                    Custodian = assetDetails.CUSTODIAN,
                    AssetUser = assetDetails.ASSETUSER,
                    ClassificationID = assetDetails.ClassificationID,
                    Classification_Edit_ID = assetDetails.Classification_Edit_ID,
                    Retention_Month = !string.IsNullOrEmpty(assetDetails.Retention) ? assetDetails.Retention.Split('_')[0] : assetDetails.Retention,
                    Retention_Year = assetDetails.Retention != null && assetDetails.Retention.Contains('_') ? assetDetails.Retention.Split('_')[1] : null,
                    ActionType = assetDetails.ACTIONTYPE,
                    ISSC_Member_Reason = assetDetails.ISSC_MEMBER_REASON,
                    Retention_Remarks = assetDetails.RETENTION_REMARK
                };
            }

            return s;
        }

        public List<CommonAAssetsRegListVM> Get_Common_Asset_Details(List<decimal> Ids)
        {
            //List<CommonAAssetsRegListVM> s = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => Ids.Contains(x.ID)).Select(x => new CommonAAssetsRegListVM
            //{
            //    ID = x.ID,
            //    Primary = x.PRIMARY,
            //    Secondary = x.SECONDARY,
            //    AssetsID = x.ASSETID,
            //    Assetlocation = x.LOCATION,
            //    AssetOwner = x.OWNER,
            //    Custodian = x.CUSTODIAN,
            //    AssetUser = x.ASSETUSER,
            //    ClassificationID = (decimal)x.CLASSIFICATIONID,
            //    Retention_Period = x.RETENTION,
            //    Classification_Edit_ID = (decimal)x.CLASSIFICATIONID,
            //    Retention_Month = x.RETENTION != null ? x.RETENTION.Split('_')[0] : x.RETENTION,
            //    Retention_Year = x.RETENTION != null ? x.RETENTION.Split('_')[1] : x.RETENTION,
            //    ActionType = x.ACTIONTYPE,
            //    Reason_NA = x.ISSC_MEMBER_REASON,
            //    Retention_Remarks = x.RETENTION_REMARK
            //}).ToList();
            var assetDetailsList = _arDBContext.ASSET_REGISTER_ASSETDETAILS
    .Where(x => Ids.Contains(x.ID))
    .Select(x => new
    {
        x.ID,
        x.PRIMARY,
        x.SECONDARY,
        x.ASSETID,
        x.LOCATION,
        x.OWNER,
        x.CUSTODIAN,
        x.ASSETUSER,
        ClassificationID = (decimal)x.CLASSIFICATIONID,
        Retention_Period = x.RETENTION,
        Classification_Edit_ID = (decimal)x.CLASSIFICATIONID,
        ActionType = x.ACTIONTYPE,
        Reason_NA = x.ISSC_MEMBER_REASON,
        Retention_Remarks = x.RETENTION_REMARK
    })
    .ToList(); // Execute query first

            // Process string operations **after** retrieving data
            List<CommonAAssetsRegListVM> s = assetDetailsList.Select(x => new CommonAAssetsRegListVM
            {
                ID = x.ID,
                Primary = x.PRIMARY,
                Secondary = x.SECONDARY,
                AssetsID = x.ASSETID,
                Assetlocation = x.LOCATION,
                AssetOwner = x.OWNER,
                Custodian = x.CUSTODIAN,
                AssetUser = x.ASSETUSER,
                ClassificationID = x.ClassificationID,
                Retention_Period = x.Retention_Period,
                Classification_Edit_ID = x.Classification_Edit_ID,
                Retention_Month = !string.IsNullOrEmpty(x.Retention_Period) ? x.Retention_Period.Split('_')[0] : x.Retention_Period,
                Retention_Year = x.Retention_Period != null && x.Retention_Period.Contains('_') ? x.Retention_Period.Split('_')[1] : null,
                ActionType = x.ActionType,
                Reason_NA = x.Reason_NA,
                Retention_Remarks = x.Retention_Remarks
            }).ToList();



            return s;
        }

        public int Delete_AssetDetails(CommonAAssetsRegListVM assetdetails)
        {
            if (assetdetails != null && assetdetails.ID > 0 && assetdetails.UpdatedBy > 0)
            {
                var ass_det = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.ID == assetdetails.ID).FirstOrDefault();
                if (ass_det != null)
                {
                    ass_det.UPDATEDBY = assetdetails.UpdatedBy;
                    ass_det.UPDATIONDATE = DateTime.Now;
                    ass_det.ACTIVE = 0;
                    _arDBContext.Entry(ass_det).State = EntityState.Modified;
                    _arDBContext.SaveChanges();
                    return 5;
                }
                return 2;
            }
            return 2;
        }

        public int FinalSubmit_AssetDetails(CommonAAssetsRegListVM assetdetails)
        {
            if (assetdetails != null && assetdetails.CreatedBy > 0 && assetdetails.SYKIID > 0)
            {
                
                    var ass_det_List = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.SKIID == assetdetails.SYKIID && x.CREATEDBY == assetdetails.CreatedBy).ToList();
                    ass_det_List.ForEach(common_data =>
                    {
                        common_data.UPDATEDBY = assetdetails.CreatedBy;
                        common_data.UPDATIONDATE = DateTime.Now;
                        common_data.SAVE = 2; // final submit
                        common_data.ISSCMEMBER_STATUS = 1;
                        common_data.FINALSUBMITDATE_ISSCMEMBER = DateTime.Now;
                    }
                    );
                    _arDBContext.SaveChanges();
                    return 1;
                }
            
            return 2;
        }

        public int ISSC_Member_AssetDetails_FinalSubmit_Check(int Empcode, long? SYKI)
        {
            var check = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.SKIID == SYKI && x.CREATEDBY == Empcode && x.SAVE == 2 && x.ACTIVE == 1 && x.ISSCMEMBER_STATUS == 1).Count();
            return check;
        }

        public int ISSC_Member_AssetDetails_EndDate_Check(int Empcode, long? SYKI)
        {
            int check = 0;
            var divisionid = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == SYKI && x.ADEMPCODE == Empcode).Select(x => x.DIVISIONID).FirstOrDefault();
            var enddate = _arDBContext.ASSET_REGISTER_ITGRC_PERIOD.Where(x => x.SKIID == SYKI && x.DIVISIONID == divisionid).Select(x => x.ENDDATE).FirstOrDefault();
            if (DateTime.Now.Date > Convert.ToDateTime(enddate))
            {
                check = 1;
            }
            return check;
        }

        public DivisionHead_Details Get_DivisionHead_(long? SYKI, long Empcode)
        {
            var empNamelist_old = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == SYKI && x.ADEMPCODE == Empcode).Select(x => new { x.OPERATION, x.DIVISIONID, x.DIVISION }).FirstOrDefault();

            var empNamelist = _arDBContext.ADORGLEVELHEAD.Where(x => x.ISACTIVE == 1 && x.ADORGLEVELID == empNamelist_old.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();

            var ISSC_Member = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == Empcode).Select(x => new { ISSCName = x.FIRSTNAME + " " + x.LASTNAME, ISSCEmail = x.EMAILID }).FirstOrDefault();
            var DivisionHead_Details = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == empNamelist).Select(x => new DivisionHead_Details
            {
                Empname = x.FIRSTNAME + " " + x.LASTNAME,
                Emailid = x.EMAILID,
                DivisionName = empNamelist_old.DIVISION,
                DivisionEmpCode = x.ADEMPCODE,
                ISSC_Member_Name = ISSC_Member.ISSCName,
                ISSC_MemberEmail = ISSC_Member.ISSCEmail
            }).FirstOrDefault();
            return DivisionHead_Details;
        }

        public DivisionHead_Details Get_OperatingHead_Old(long? SYKI, long Empcode)
        {
            var empNamelist = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == SYKI && x.ADEMPCODE == Empcode).Select(x => new { x.SUPSUPERVISOREMPCODE, x.DIVISION }).FirstOrDefault();
            var ISSC_Member = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == Empcode).Select(x => new { ISSCName = x.FIRSTNAME + " " + x.LASTNAME, ISSCEmail = x.EMAILID }).FirstOrDefault();
            var DivisionHead_Details = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == empNamelist.SUPSUPERVISOREMPCODE).Select(x => new DivisionHead_Details
            {
                Empname = x.FIRSTNAME + " " + x.LASTNAME,
                Emailid = x.EMAILID,
                DivisionName = empNamelist.DIVISION,
                DivisionEmpCode = x.ADEMPCODE,
                ISSC_Member_Name = ISSC_Member.ISSCName,
                ISSC_MemberEmail = ISSC_Member.ISSCEmail
            }).FirstOrDefault();
            return DivisionHead_Details;
        }

        public DivisionHead_Details Get_OperatingHead_(long? SYKI, long Empcode, long Userid)
        {
            var empNamelist = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == SYKI && x.ADEMPCODE == Empcode).Select(x => new { x.SUPSUPERVISOREMPCODE, x.DIVISION }).FirstOrDefault();
            var ISSC_Member = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == Empcode).Select(x => new { ISSCName = x.FIRSTNAME + " " + x.LASTNAME, ISSCEmail = x.EMAILID }).FirstOrDefault();
            var DivisionHead_Details = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == Userid).Select(x => new DivisionHead_Details
            {
                Empname = x.FIRSTNAME + " " + x.LASTNAME,
                Emailid = x.EMAILID,
                DivisionName = empNamelist.DIVISION,
                DivisionEmpCode = x.ADEMPCODE,
                ISSC_Member_Name = ISSC_Member.ISSCName,
                ISSC_MemberEmail = ISSC_Member.ISSCEmail
            }).FirstOrDefault();
            return DivisionHead_Details;
        }

        public Get_Division_ISSC_Member Get_ISSC_Member_Details_For_DivisionHead(int? id)
        {
            var SYKIList = (from data in _arDBContext.SYKI.Where(x => x.ACTIVE == 1)
                            select data).FirstOrDefault();
            // var ISSC_Member = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.DIVISIONHEAD_ID == id).Select(x => x.CREATEDBY).FirstOrDefault();
            var divionid = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.SYKI == SYKIList.SYKIID && x.ADEMPCODE == id).Select(x => x.DIVISIONID).FirstOrDefault(); ;
            var ISSC_Member = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.ACTIVE == 1 && x.SYKIID == SYKIList.SYKIID && x.DIVISIONID == divionid).Select(x => x.ISSCEMPCODE).FirstOrDefault();
            var PeriodSettingList = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == ISSC_Member && x.SYKI == SYKIList.SYKIID).Select(x => new Get_Division_ISSC_Member
            {
                DivId = x.DIVISIONID,
                DivisionName = x.DIVISION,
                ISSCMember_EmpCode = x.ADEMPCODE,
                ISSCMember = _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == x.ADEMPCODE).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault(),
                EndDate = _arDBContext.ASSET_REGISTER_ITGRC_PERIOD.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.DIVISIONID == x.DIVISIONID).Select(x1 => x1.ENDDATE).FirstOrDefault(),

                ISSC_Member_Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == ISSC_Member && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.FINALSUBMITDATE_ISSCMEMBER).FirstOrDefault(),
                ISSC_Member_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == ISSC_Member && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.ISSCMEMBER_STATUS).FirstOrDefault(),

                DivisionHead_Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == ISSC_Member && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.FINALSUBMITDATE_DIVISIONHEAD).FirstOrDefault(),
                DivisionHead_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == ISSC_Member && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.DIVISION_STATUS).FirstOrDefault(),

                OperatingHead__Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == ISSC_Member && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.FINALSUBMITDATE_OPERATINGHEAD).FirstOrDefault(),
                OperatingHead_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == ISSC_Member && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.OPERATIONG_STATUS).FirstOrDefault(),

            }).FirstOrDefault();
            return PeriodSettingList;
        }
        //Added by Aumento for SR91196
        public List<Get_Division_ISSC_Member> Get_ISSC_Members_Detail_For_ITGRCHead(int? id)
        {
            List<Get_Division_ISSC_Member> allPeriodSettingList = new List<Get_Division_ISSC_Member>();

            try
            {
                string D_Head = string.Empty;
                string O_Head = string.Empty;
                string ITGRC_Head = string.Empty;

                var SYKIID = _arDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
                string ITGRC_code = GetITGRCValue();
                long ITGRC_Ecode = Convert.ToInt64(ITGRC_code);

                if (ITGRC_code != null)
                {
                    var ITGRC_ = _arDBContext.ADEMPLOYEE
                        .Where(x => x.ADEMPCODE == ITGRC_Ecode)
                        .Select(x => new { Name = x.FIRSTNAME + " " + x.LASTNAME })
                        .FirstOrDefault();

                    if (ITGRC_ != null)
                    {
                        ITGRC_Head = ITGRC_.Name;
                    }
                }
                bool IsOpMatch = IsOperationMatch(id);

                var q1 = _arDBContext.ASSET_REGISTER_ASSETDETAILS
                    .Where(r => r.ITGRC_ID == (IsOpMatch?ITGRC_Ecode:null) && r.ISSCMEMBER_STATUS != null && r.SKIID == SYKIID)
                    .Select(x => x.CREATEDBY)
                    .Distinct()
                    .ToList();

                foreach (var ID in q1)
                {
                    var divionids1 = _arDBContext.VW_ASSOCIATELVLDETAILS
                        .Where(x => x.ACTIVE == 1 && x.SYKI == SYKIID && x.ADEMPCODE == ID)
                        .Select(x => x.DIVISIONID)
                        .ToList();

                    var ISSC_Members1 = _arDBContext.ASSET_REGISTER_MEMNOMINATION
                        .Where(x => x.ACTIVE == 1 && x.SYKIID == SYKIID && divionids1.Contains((long)x.DIVISIONID))
                        .Select(x => x.ISSCEMPCODE)
                        .ToList();

                    var empNamelist_old1 = _arDBContext.VW_ASSOCIATELVLDETAILS
                        .Where(x => x.SYKI == SYKIID && x.ADEMPCODE == ID)
                        .Select(x => new { x.OPERATION, x.DIVISIONID, x.DIVISION })
                        .FirstOrDefault();

                    if (empNamelist_old1 != null)
                    {
                        var empNamelist1 = _arDBContext.ADORGLEVELHEAD
                            .Where(x => x.ISACTIVE == 1 && x.ADORGLEVELID == empNamelist_old1.DIVISIONID)
                            .Select(x => x.ADEMPCODE)
                            .FirstOrDefault();

                        var D_HeadName1 = _arDBContext.ADEMPLOYEE
                            .Where(x => x.ADEMPCODE == empNamelist1)
                            .Select(x => new { Name = x.FIRSTNAME + " " + x.LASTNAME })
                            .FirstOrDefault();

                        if (D_HeadName1 != null) D_Head = D_HeadName1.Name;

                        var ohead_id1 = _arDBContext.ASSET_REGISTER_MEMNOMINATION
                            .Where(x => x.SYKIID == SYKIID && x.DIVISIONID == empNamelist_old1.DIVISIONID && x.ACTIVE == 1)
                            .Select(x => x.ADDEDBY)
                            .FirstOrDefault();

                        var O_HeadName1 = _arDBContext.ADEMPLOYEE
                            .Where(x => x.ADEMPCODE == ohead_id1)
                            .Select(x => new { Name = x.FIRSTNAME + " " + x.LASTNAME })
                            .FirstOrDefault();

                        if (O_HeadName1 != null) O_Head = O_HeadName1.Name;
                    }

                    //var PeriodSettingList1 = _arDBContext.VW_ASSOCIATELVLDETAILS
                    //    .Where(x => ISSC_Members1.Contains(x.ADEMPCODE) && x.SYKI == SYKIID)
                    //    .Select(x => new Get_Division_ISSC_Member
                    //    {
                    //        DivId = (long)x.DIVISIONID,
                    //        DivisionName = x.DIVISION,
                    //        ITGRC_Head_EmpCode = ITGRC_Ecode,
                    //        ISSCMember_EmpCode = x.ADEMPCODE,
                    //        ISSCMember = _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == x.ADEMPCODE).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault(),
                    //        EndDate = _arDBContext.ASSET_REGISTER_ITGRC_PERIOD.Where(x1 => x1.SKIID == SYKIID && x1.DIVISIONID == x.DIVISIONID).Select(x1 => x1.ENDDATE).FirstOrDefault(),

                    //        ISSC_Member_Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.FINALSUBMITDATE_ISSCMEMBER).FirstOrDefault(),
                    //        ISSC_Member_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.ISSCMEMBER_STATUS).FirstOrDefault(),

                    //        DivisionHead_Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.FINALSUBMITDATE_DIVISIONHEAD).FirstOrDefault(),
                    //        DivisionHead_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.DIVISION_STATUS).FirstOrDefault(),

                    //        OperatingHead__Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.FINALSUBMITDATE_OPERATINGHEAD).FirstOrDefault(),
                    //        OperatingHead_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.OPERATIONG_STATUS).FirstOrDefault(),

                    //        ITGRCHead_Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.FINALSUBMITDATE_ITGRCHEAD).FirstOrDefault(),
                    //        ITGRCHead_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.ITGRC_STATUS).FirstOrDefault(),

                    //        DivisionHead_Name = D_Head,
                    //        OperatingHead_Name = O_Head,
                    //        ITGRC_Head_Name = ITGRC_Head,
                    //        IS_ITOPRATION = IsOpMatch
                    //    })
                    //    .ToList();
                    // Step 1: Materialize the base data from the database
                    var rawList = _arDBContext.VW_ASSOCIATELVLDETAILS
                        .Where(x => ISSC_Members1.Contains(x.ADEMPCODE) && x.SYKI == SYKIID)
                        .ToList();

                    // Step 2: Project using local variables
                    var PeriodSettingList1 = rawList.Select(x => new Get_Division_ISSC_Member
                    {
                        DivId = x.DIVISIONID,
                        DivisionName = x.DIVISION,
                        ITGRC_Head_EmpCode = ITGRC_Ecode,
                        ISSCMember_EmpCode = x.ADEMPCODE,
                        ISSCMember = _arDBContext.ADEMPLOYEE
                            .Where(y => y.ADEMPCODE == x.ADEMPCODE)
                            .Select(y => y.FIRSTNAME + " " + y.LASTNAME)
                            .FirstOrDefault(),

                        EndDate = _arDBContext.ASSET_REGISTER_ITGRC_PERIOD
                            .Where(x1 => x1.SKIID == SYKIID && x1.DIVISIONID == x.DIVISIONID)
                            .Select(x1 => x1.ENDDATE)
                            .FirstOrDefault(),

                        ISSC_Member_Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS
                            .Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2)
                            .Select(x1 => x1.FINALSUBMITDATE_ISSCMEMBER)
                            .FirstOrDefault(),

                        ISSC_Member_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS
                            .Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2)
                            .Select(x1 => x1.ISSCMEMBER_STATUS)
                            .FirstOrDefault(),

                        DivisionHead_Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS
                            .Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2)
                            .Select(x1 => x1.FINALSUBMITDATE_DIVISIONHEAD)
                            .FirstOrDefault(),

                        DivisionHead_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS
                            .Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2)
                            .Select(x1 => x1.DIVISION_STATUS)
                            .FirstOrDefault(),

                        OperatingHead__Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS
                            .Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2)
                            .Select(x1 => x1.FINALSUBMITDATE_OPERATINGHEAD)
                            .FirstOrDefault(),

                        OperatingHead_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS
                            .Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2)
                            .Select(x1 => x1.OPERATIONG_STATUS)
                            .FirstOrDefault(),

                        ITGRCHead_Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS
                            .Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2)
                            .Select(x1 => x1.FINALSUBMITDATE_ITGRCHEAD)
                            .FirstOrDefault(),

                        ITGRCHead_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS
                            .Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2)
                            .Select(x1 => x1.ITGRC_STATUS)
                            .FirstOrDefault(),

                        DivisionHead_Name = D_Head,
                        OperatingHead_Name = O_Head,
                        ITGRC_Head_Name = ITGRC_Head,
                        IS_ITOPRATION = IsOpMatch
                    }).ToList();


                    allPeriodSettingList.AddRange(PeriodSettingList1);
                }
            }
            catch (Exception ex)
            {
                // Log the exception here (to file, database, or logger)
                // Example:
                Console.WriteLine("Error in Get_ISSC_Members_Detail_For_ITGRCHead: " + ex.Message);
                // Or use a logger like log4net, NLog, Serilog, etc.
            }

            return allPeriodSettingList;
        }
        // END :: Added By Aumento :: SR102194
        public List<Asset_Dvision_Deatils_VM> Get_ISSC_Members_Division_Details_For_ITGRCHead(int? id)
        {
            var SYKIID = _arDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();

            var divionDetails = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && (x.SYKI > 21 && x.SYKI < SYKIID) && x.ADEMPCODE == id).OrderByDescending(x => x.SYKI).Take(3)
                .Select(x => new
                {
                    x.SYKI,
                    SYKIName = _arDBContext.SYKI.Where(y => y.SYKIID == x.SYKI).Select(y => y.KICODE).FirstOrDefault(),
                    x.DIVISIONID,
                    x.DIVISION
                }).ToList();

            var divisionList = divionDetails.Select(x => new Asset_Dvision_Deatils_VM
            {
                SYKI = x.SYKI,
                DIVISIONID = x.DIVISIONID,
                DIVISION = x.DIVISION,
                SYKIName = x.SYKIName
            }).ToList();

            // var ISSC_Member = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.DIVISIONHEAD_ID == id).Select(x => x.CREATEDBY).FirstOrDefault();
            //var divionids = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.SYKI == SYKIID && x.ADEMPCODE == id).Select(x => x.DIVISIONID).ToList();
            //var ISSC_Members = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.ACTIVE == 1 && x.SYKIID == SYKIID && divionids.Contains((long)x.DIVISIONID)).Select(x => x.ISSCEMPCODE).ToList();
            //var PeriodSettingList = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => ISSC_Members.Contains(x.ADEMPCODE) && x.SYKI == SYKIID).Select(x => new Get_Division_ISSC_Member
            //{
            //    DivId = (long)x.DIVISIONID,
            //    DivisionName = x.DIVISION,

            //}).ToList();
            return divisionList;
        }
        public int ITGRC_Head_AssetDetails_FinalSubmit_Check(int Userid, long? SYKI, long ISSC_Code)
        {
            var check = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.SKIID == SYKI && x.ITGRC_ID == Userid && x.CREATEDBY == ISSC_Code && x.SAVE == 2 && x.ACTIVE == 1 && x.ISSCMEMBER_STATUS == 1 && x.ITGRC_STATUS == 1).Count();
            return check;
        }
        public DivisionHead_Details Get_ITGRCHead_(long? SYKI, long Empcode)
        {
            var empNamelist_old = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == SYKI && x.ADEMPCODE == Empcode).Select(x => new { x.OPERATION, x.DIVISIONID, x.DIVISION }).FirstOrDefault();

            var empNamelist = _arDBContext.ADORGLEVELHEAD.Where(x => x.ISACTIVE == 1 && x.ADORGLEVELID == empNamelist_old.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();

            var ISSC_Member = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == Empcode).Select(x => new { ISSCName = x.FIRSTNAME + " " + x.LASTNAME, ISSCEmail = x.EMAILID }).FirstOrDefault();

            string ITGRCDetailslist = GetITGRCValue();
            long ITGRC_Ecode = Convert.ToInt64(empNamelist);

            var ITGRCHead_Details = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == ITGRC_Ecode).Select(x => new DivisionHead_Details
            {
                Empname = x.FIRSTNAME + " " + x.LASTNAME,
                Emailid = x.EMAILID,
                ITGRCEmpCode = x.ADEMPCODE,
                ISSC_Member_Name = ISSC_Member.ISSCName,
                ISSC_MemberEmail = ISSC_Member.ISSCEmail,
                DivisionName = empNamelist_old.DIVISION,
                DivisionEmpCode = x.ADEMPCODE,
            }).FirstOrDefault();
            return ITGRCHead_Details;
        }
        public int Asset_Approve_By_ITGRCHead(CommonAAssetsRegListVM asset_details)
        {
            if (asset_details != null && asset_details.SYKIID > 0 && asset_details.ISSC_Member_EmpCode > 0 && asset_details.ITGRC_Head_EmpCode > 0)
            {
                //using (var db = new ePortalEntities2())
                //{
                    var result_List = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == asset_details.ISSC_Member_EmpCode && x.SKIID == asset_details.SYKIID && x.ACTIVE == 1 && x.ISSCMEMBER_STATUS == 1).ToList();
                    result_List.ForEach(ass_det =>
                    {
                        ass_det.ITGRC_ID = asset_details.ITGRC_Head_EmpCode;
                        ass_det.FINALSUBMITDATE_ITGRCHEAD = DateTime.Now;
                        ass_det.ITGRC_STATUS = 1;
                        ass_det.REVIEW_REMARKS_ITGRCHEAD = asset_details.ITGRCHead_Remarks;
                    }
                    );
                _arDBContext.SaveChanges();
                //}
                return 1;
            }
            return 2;
        }
        public int Asset_SendBack_By_ITGRCHead(CommonAAssetsRegListVM asset_details)
        {
            if (asset_details != null && asset_details.SYKIID > 0 && asset_details.ISSC_Member_EmpCode > 0)
            {
                //using (var db = new ePortalEntities2())
                //{
                    var result_List = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == asset_details.ISSC_Member_EmpCode && x.SKIID == asset_details.SYKIID && x.ACTIVE == 1 && x.ISSCMEMBER_STATUS == 1).ToList();
                    result_List.ForEach(ass_det =>
                    {
                        ass_det.ACTIVE = 1;
                        ass_det.SAVE = 1;

                        ass_det.ISSCMEMBER_STATUS = null;
                        ass_det.FINALSUBMITDATE_ISSCMEMBER = null;
                        ass_det.UPDATIONDATE = DateTime.Now;
                        ass_det.DIVISION_STATUS = null;
                        ass_det.FINALSUBMITDATE_DIVISIONHEAD = null;
                        ass_det.REVIEW_REMARKS_DIVISIONHEAD = asset_details.DivisionHead_Remarks;
                        ass_det.REVIEW_REMARKS_OPERATINGHEAD = null;
                        ass_det.ITGRC_STATUS = null;
                        ass_det.FINALSUBMITDATE_ITGRCHEAD = null;
                        ass_det.REVIEW_REMARKS_ITGRCHEAD = null;
                        // START :: Added By Aumento :: SR102194
                        if (ass_det.CLASSIFICATIONID == 1)
                        {
                            ass_det.ISSC_MEMBER_REASON = null;
                        }
                        // END :: Added By Aumento :: SR102194
                    }
                    );
                    _arDBContext.SaveChanges();
               // }
                return 3; //Send back 
            }
            return 2;//There is some problem, please try again later.
        }
        //Added by Aumento for SR91196
        public List<Get_Division_ISSC_Member> Get_ISSC_Members_Detail_For_DivisionHead(int? id)
        {
            //Added by aumento as on 17062024 for SR71836-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            string D_Head = string.Empty; string O_Head = string.Empty;
            //Added by Aumento for SR91196
            //Added by Aumento for SR91196
            bool IsOpMatch = IsOperationMatch(id); // Added By Aumento :: SR102194
            string ITGRC_Head = string.Empty;
            //Added by Aumento for SR91196

            // START :: Added By Aumento :: SR102194
            string ITGRC_code = GetITGRCValue();
            long ITGRC_Ecode = Convert.ToInt64(ITGRC_code);
            long DivHeadCode = 0;
            decimal? OPHeadCode = 0;

            if (ITGRC_code != null)
            {
                var ITGRC_ = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == ITGRC_Ecode).Select(x => new { Name = x.FIRSTNAME + " " + x.LASTNAME }).FirstOrDefault();
                if (ITGRC_ != null)
                {
                    ITGRC_Head = ITGRC_.Name;
                }
            }
            // END :: Added By Aumento :: SR102194

            //Added by Aumento for SR91196
            //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            var SYKIID = _arDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
            // var ISSC_Member = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.DIVISIONHEAD_ID == id).Select(x => x.CREATEDBY).FirstOrDefault();
            var divionids = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.SYKI == SYKIID && x.ADEMPCODE == id).Select(x => x.DIVISIONID).ToList();
            var ISSC_Members = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.ACTIVE == 1 && x.SYKIID == SYKIID && divionids.Contains((long)x.DIVISIONID)).Select(x => x.ISSCEMPCODE).ToList();

            //Added by aumento as on 17062024 for SR71836-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            var empNamelist_old = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == SYKIID && x.ADEMPCODE == id).Select(x => new { x.OPERATION, x.DIVISIONID, x.DIVISION }).FirstOrDefault();
            if (empNamelist_old != null)
            {
                var empNamelist = _arDBContext.ADORGLEVELHEAD.Where(x => x.ISACTIVE == 1 && x.ADORGLEVELID == empNamelist_old.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                DivHeadCode = empNamelist; // Added By Aumento :: SR102194
                //var empNamelist = _arDBContext.ADORGLEVELHEAD.Where(x => x.ISACTIVE == 0 && x.ADORGLEVELID == 0).Select(x => x.ADEMPCODE).FirstOrDefault();
                var D_HeadName = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == empNamelist).Select(x => new { Name = x.FIRSTNAME + " " + x.LASTNAME }).FirstOrDefault();
                if (D_HeadName != null)
                {
                    D_Head = D_HeadName.Name;
                }

                var ohead_id = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.SYKIID == SYKIID && x.DIVISIONID == empNamelist_old.DIVISIONID && x.ACTIVE == 1).Select(x => x.ADDEDBY).FirstOrDefault();
                OPHeadCode = ohead_id; // Added By Aumento :: SR102194
                //var ohead_id = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.SYKIID == SYKIList.SYKIID && x.DIVISIONID == 0 && x.ACTIVE == 1).Select(x => x.ADDEDBY).FirstOrDefault();
                var O_HeadName = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == ohead_id).Select(x => new { Name = x.FIRSTNAME + " " + x.LASTNAME }).FirstOrDefault();

                if (O_HeadName != null)
                {
                    O_Head = O_HeadName.Name;
                }
            }
            //-----------------------------------------------------------------------------------------------------

            //var PeriodSettingList = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => ISSC_Members.Contains(x.ADEMPCODE) && x.SYKI == SYKIID).Select(x => new Get_Division_ISSC_Member
            //{
            //    DivId = (long)x.DIVISIONID,
            //    DivisionName = x.DIVISION,
            //    ISSCMember_EmpCode = x.ADEMPCODE,
            //    ISSCMember = _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == x.ADEMPCODE).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault(),
            //    EndDate = _arDBContext.ASSET_REGISTER_ITGRC_PERIOD.Where(x1 => x1.SKIID == SYKIID && x1.DIVISIONID == x.DIVISIONID).Select(x1 => x1.ENDDATE).FirstOrDefault(),

            //    ISSC_Member_Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.FINALSUBMITDATE_ISSCMEMBER).FirstOrDefault(),
            //    ISSC_Member_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.ISSCMEMBER_STATUS).FirstOrDefault(),

            //    DivisionHead_Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.FINALSUBMITDATE_DIVISIONHEAD).FirstOrDefault(),
            //    DivisionHead_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.DIVISION_STATUS).FirstOrDefault(),

            //    OperatingHead__Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.FINALSUBMITDATE_OPERATINGHEAD).FirstOrDefault(),
            //    OperatingHead_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.OPERATIONG_STATUS).FirstOrDefault(),

            //    //Added by aumento as on 17062024 for SR71836-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //    //Added by Aumento for SR91196
            //    ITGRCHead_Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.FINALSUBMITDATE_ITGRCHEAD).FirstOrDefault(),
            //    ITGRCHead_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.ITGRC_STATUS).FirstOrDefault(),
            //    // START :: Added By Aumento :: SR102194
            //    ITGRC_Head_EmpCode = ITGRC_Ecode,
            //    Op_Head_EmpCode = OPHeadCode,
            //    Div_Head_EmpCode = DivHeadCode,
            //    // END :: Added By Aumento :: SR102194
            //    ITGRC_Head_Name = ITGRC_Head,
            //    //Added by Aumento for SR91196
            //    DivisionHead_Name = D_Head,
            //    OperatingHead_Name = O_Head, // Added By Aumento :: SR102194
            //    IS_ITOPRATION = IsOpMatch  // Added By Aumento :: SR102194

            //    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            //}).ToList();
            var rawList = _arDBContext.VW_ASSOCIATELVLDETAILS
    .Where(x => ISSC_Members.Contains(x.ADEMPCODE) && x.SYKI == SYKIID)
    .ToList(); // Materialize first

            var PeriodSettingList = rawList.Select(x => new Get_Division_ISSC_Member
            {
                DivId = x.DIVISIONID,
                DivisionName = x.DIVISION,
                ISSCMember_EmpCode = x.ADEMPCODE,
                ISSCMember = _arDBContext.ADEMPLOYEE
                    .Where(y => y.ADEMPCODE == x.ADEMPCODE)
                    .Select(y => y.FIRSTNAME + " " + y.LASTNAME)
                    .FirstOrDefault(),

                EndDate = _arDBContext.ASSET_REGISTER_ITGRC_PERIOD
                    .Where(x1 => x1.SKIID == SYKIID && x1.DIVISIONID == x.DIVISIONID)
                    .Select(x1 => x1.ENDDATE)
                    .FirstOrDefault(),

                ISSC_Member_Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS
                    .Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2)
                    .Select(x1 => x1.FINALSUBMITDATE_ISSCMEMBER)
                    .FirstOrDefault(),

                ISSC_Member_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS
                    .Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2)
                    .Select(x1 => x1.ISSCMEMBER_STATUS)
                    .FirstOrDefault(),

                DivisionHead_Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS
                    .Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2)
                    .Select(x1 => x1.FINALSUBMITDATE_DIVISIONHEAD)
                    .FirstOrDefault(),

                DivisionHead_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS
                    .Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2)
                    .Select(x1 => x1.DIVISION_STATUS)
                    .FirstOrDefault(),

                OperatingHead__Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS
                    .Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2)
                    .Select(x1 => x1.FINALSUBMITDATE_OPERATINGHEAD)
                    .FirstOrDefault(),

                OperatingHead_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS
                    .Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2)
                    .Select(x1 => x1.OPERATIONG_STATUS)
                    .FirstOrDefault(),

                ITGRCHead_Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS
                    .Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2)
                    .Select(x1 => x1.FINALSUBMITDATE_ITGRCHEAD)
                    .FirstOrDefault(),

                ITGRCHead_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS
                    .Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ADEMPCODE && x1.ACTIVE == 1 && x1.SAVE == 2)
                    .Select(x1 => x1.ITGRC_STATUS)
                    .FirstOrDefault(),

                ITGRC_Head_EmpCode = ITGRC_Ecode,
                Op_Head_EmpCode = OPHeadCode,
                Div_Head_EmpCode = DivHeadCode,
                ITGRC_Head_Name = ITGRC_Head,
                DivisionHead_Name = D_Head,
                OperatingHead_Name = O_Head,
                IS_ITOPRATION = IsOpMatch
            }).ToList();


            return PeriodSettingList;
        }

        public List<Asset_Dvision_Deatils_VM> Get_ISSC_Members_Division_Details_For_DivisionHead(int? id)
        {
            var SYKIID = _arDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();

            var divionDetails = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && (x.SYKI > 21 && x.SYKI < SYKIID) && x.ADEMPCODE == id).OrderByDescending(x => x.SYKI).Take(3)
                .Select(x => new
                {
                    x.SYKI,
                    SYKIName = _arDBContext.SYKI.Where(y => y.SYKIID == x.SYKI).Select(y => y.KICODE).FirstOrDefault(),
                    x.DIVISIONID,
                    x.DIVISION
                }).ToList();

            var divisionList = divionDetails.Select(x => new Asset_Dvision_Deatils_VM
            {
                SYKI = x.SYKI,
                DIVISIONID = x.DIVISIONID,
                DIVISION = x.DIVISION,
                SYKIName = x.SYKIName
            }).ToList();

            // var ISSC_Member = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.DIVISIONHEAD_ID == id).Select(x => x.CREATEDBY).FirstOrDefault();
            //var divionids = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.SYKI == SYKIID && x.ADEMPCODE == id).Select(x => x.DIVISIONID).ToList();
            //var ISSC_Members = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.ACTIVE == 1 && x.SYKIID == SYKIID && divionids.Contains((long)x.DIVISIONID)).Select(x => x.ISSCEMPCODE).ToList();
            //var PeriodSettingList = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => ISSC_Members.Contains(x.ADEMPCODE) && x.SYKI == SYKIID).Select(x => new Get_Division_ISSC_Member
            //{
            //    DivId = (long)x.DIVISIONID,
            //    DivisionName = x.DIVISION,

            //}).ToList();
            return divisionList;
        }

        public List<CommonAAssetsRegListVM> Get_ISSC_Member_Asset_For_DivisionHead(long? Userid, long? SYKI)
        {
            var assets = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == Userid && x.SKIID == SYKI && x.ACTIVE == 1 && x.ISSCMEMBER_STATUS == 1).Select(x => new CommonAAssetsRegListVM
            {
                ID = x.ID,
                Primary = x.PRIMARY,
                Secondary = x.SECONDARY,
                AssetsID = (decimal)x.ASSETID,
                Assetlocation = x.LOCATION,
                AssetOwner = x.OWNER,
                Custodian = x.CUSTODIAN,
                AssetUser = x.ASSETUSER,
                ClassificationID = (decimal)x.CLASSIFICATIONID,
                Retention_Period = x.RETENTION,
                Reason_NA = x.ISSC_MEMBER_REASON,
                ActionType = x.ACTIONTYPE,
                Retention_Remarks = x.RETENTION_REMARK
            }).ToList();
            return assets;
        }

        public int? GetAssetUserDeailsForDivisionHeadByLoginUserID(long Userid,long Syki)
        {
            var UserDetails = _arDBContext.ASSET_USER_MAPPING.Where(x => x.NEWDIVISIONEMPCODE == Userid && x.SYKIID == Syki).FirstOrDefault();
            if (UserDetails != null)
            {
                return Convert.ToInt16(UserDetails.OLDDIVISIONEMPCODE);
            }
            else
            {
                return null;
            }
        }

        public int? GetAssetUserDeailsForOPHeadByLoginUserID(long Userid, long Syki)
        {
            var UserDetails = _arDBContext.ASSET_USER_MAPPING.Where(x => x.NEWOPRATIONHEADEMPCODE == Userid && x.SYKIID == Syki).FirstOrDefault();
            if (UserDetails != null)
            {
                return Convert.ToInt16(UserDetails.OLDOPERATIONHEADCODE);
            }
            else
            {
                return null;
            }
        }

        public int Asset_Approve_By_DivisionHead(CommonAAssetsRegListVM asset_details)
        {
            if (asset_details != null && asset_details.SYKIID > 0 && asset_details.ISSC_Member_EmpCode > 0 && asset_details.Division_Head_EmpCode > 0)
            {
                //using (var db = new ePortalEntities2())
                //{
                    var result_List = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == asset_details.ISSC_Member_EmpCode && x.SKIID == asset_details.SYKIID && x.ACTIVE == 1 && x.ISSCMEMBER_STATUS == 1).ToList();
                    result_List.ForEach(ass_det =>
                    {
                        ass_det.DIVISIONHEAD_ID = asset_details.Division_Head_EmpCode;
                        ass_det.FINALSUBMITDATE_DIVISIONHEAD = DateTime.Now;
                        ass_det.DIVISION_STATUS = 1;
                        ass_det.REVIEW_REMARKS_DIVISIONHEAD = asset_details.DivisionHead_Remarks;
                    }
                    );
                _arDBContext.SaveChanges();
                //}
                return 1;
            }
            return 2;
        }

        public int Asset_SendBack_By_DivisionHead(CommonAAssetsRegListVM asset_details)
        {
            if (asset_details != null && asset_details.SYKIID > 0 && asset_details.ISSC_Member_EmpCode > 0)
            {
                //using (var db = new ePortalEntities2())
                //{
                    var result_List = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == asset_details.ISSC_Member_EmpCode && x.SKIID == asset_details.SYKIID && x.ACTIVE == 1 && x.ISSCMEMBER_STATUS == 1).ToList();
                    result_List.ForEach(ass_det =>
                    {
                        ass_det.ACTIVE = 1;
                        ass_det.SAVE = 1;

                        ass_det.ISSCMEMBER_STATUS = null;
                        //ass_det.ISSC_MEMBER_REASON = null;
                        ass_det.FINALSUBMITDATE_ISSCMEMBER = null;
                        ass_det.UPDATIONDATE = DateTime.Now;
                        ass_det.DIVISION_STATUS = null;
                        ass_det.FINALSUBMITDATE_DIVISIONHEAD = null;
                        ass_det.REVIEW_REMARKS_DIVISIONHEAD = asset_details.DivisionHead_Remarks;
                        ass_det.REVIEW_REMARKS_OPERATINGHEAD = null;

                        //Added by Aumento for SR91196
                        ass_det.ITGRC_STATUS = null;
                        ass_det.FINALSUBMITDATE_ITGRCHEAD = null;
                        ass_det.REVIEW_REMARKS_ITGRCHEAD = null;
                        //Added by Aumento for SR91196
                        // START :: Added By Aumento :: SR102194
                        if (ass_det.CLASSIFICATIONID == 1)
                        {
                            ass_det.ISSC_MEMBER_REASON = null;
                        }
                        // END :: Added By Aumento :: SR102194
                    }
                    );
                    _arDBContext.SaveChanges();
                //}
                return 3; //Send back 
            }
            return 2;//There is some problem, please try again later.
        }
        public List<ISSCMember_details> Send_Email_To_ISSC_OperatingHead(CommonAAssetsRegListVM asset_details)
        {
            List<ISSCMember_details> Emp_Details = new List<ISSCMember_details>();
            List<decimal?> empcode = new List<decimal?>();
            // var result = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == asset_details.SYKIID && x.ADEMPCODE == asset_details.ISSC_Member_EmpCode).Select(x => x.SUPSUPERVISOREMPCODE).FirstOrDefault();
            // empcode.Add(asset_details.ISSC_Member_EmpCode);
            var result = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.ACTIVE == 1 && x.STATUS == 1 && x.SYKIID == asset_details.SYKIID && x.ISSCEMPCODE == asset_details.ISSC_Member_EmpCode).Select(x => x.ADDEDBY).FirstOrDefault();
            if (result != null)
                empcode.Add(result);
            foreach (var item in empcode)
            {
                var emp = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == item).Select(x => new ISSCMember_details { EmpEmail = x.EMAILID, EmpName = x.FIRSTNAME + " " + x.LASTNAME }).FirstOrDefault();
                Emp_Details.Add(emp);
            }
            return Emp_Details;
        }

        public List<ISSCMember_details> Send_Email_To_ISSC_OperatingHead_FroSendBack(CommonAAssetsRegListVM asset_details)
        {
            List<ISSCMember_details> Emp_Details = new List<ISSCMember_details>();
            List<long?> empcode = new List<long?>();
            //  var result = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == asset_details.SYKIID && x.ADEMPCODE == asset_details.ISSC_Member_EmpCode).Select(x => x.SUPSUPERVISOREMPCODE).FirstOrDefault();
            empcode.Add(asset_details.ISSC_Member_EmpCode);
            foreach (var item in empcode)
            {
                var emp = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == item).Select(x => new ISSCMember_details { EmpEmail = x.EMAILID, EmpName = x.FIRSTNAME + " " + x.LASTNAME }).FirstOrDefault();
                Emp_Details.Add(emp);
            }
            return Emp_Details;
        }
        public AssetRegister_Start_EndDate_ISSC_Member GetStartDate_EndDate_ForAsset_Register(CommonAAssetsRegListVM asset_details)
        {
            AssetRegister_Start_EndDate_ISSC_Member Emp_Details = new AssetRegister_Start_EndDate_ISSC_Member();
            var divisionid = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == asset_details.SYKIID && x.ADEMPCODE == asset_details.ISSC_Member_EmpCode).Select(x => x.DIVISIONID).FirstOrDefault();
            Emp_Details = _arDBContext.ASSET_REGISTER_ITGRC_PERIOD.Where(x => x.SKIID == asset_details.SYKIID && x.DIVISIONID == divisionid).Select(x => new AssetRegister_Start_EndDate_ISSC_Member { StartDate = x.STARTDATE, EndDate = x.ENDDATE }).FirstOrDefault();

            return Emp_Details;
        }


        public int Division_Head_AssetDetails_FinalSubmit_Check(int Userid, long? SYKI,long ISSC_Code)
        {
            var check = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.SKIID == SYKI && x.DIVISIONHEAD_ID == Userid && x.CREATEDBY== ISSC_Code && x.SAVE == 2 && x.ACTIVE == 1 && x.ISSCMEMBER_STATUS == 1 && x.DIVISION_STATUS == 1).Count();
            return check;
        }

        public List<Get_Division_ISSC_Member> Get_ISSC_Members_Asset_Details_For_OpearingHead(int? id)
        {
            //Added by aumento as on 17062024 for SR71836-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            string D_Head = string.Empty; string O_Head = string.Empty;
            //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            var SYKIList = (from data in _arDBContext.SYKI.Where(x => x.ACTIVE == 1)
                            select data).FirstOrDefault();
            // var ISSC_Member = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.DIVISIONHEAD_ID == id).Select(x => x.CREATEDBY).FirstOrDefault();
            var operationids = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.SYKI == SYKIList.SYKIID && x.ADEMPCODE == id).Select(x => x.OPERATIONID).ToList(); ;
            var ISSC_Member = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.ACTIVE == 1 && x.SYKIID == SYKIList.SYKIID && operationids.Contains((long)x.OPERATIONID)).Select(x => x.ISSCEMPCODE).ToList();

            List<Get_Division_ISSC_Member> ISSCMemberList = new List<Get_Division_ISSC_Member>();
            foreach (var item in ISSC_Member)
            {
                //Added by aumento as on 17062024 for SR71836-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                D_Head = string.Empty; O_Head = string.Empty;
                var empNamelist_old = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == SYKIList.SYKIID && x.ADEMPCODE == item).Select(x => new { x.OPERATION, x.DIVISIONID, x.DIVISION }).FirstOrDefault();
                //Added by Aumento for SR91196
                bool IsOpMatch = IsOperationMatch(id); // Added By Aumento :: SR102194
                string ITGRC_Head = string.Empty;
                string ITGRC_code = GetITGRCValue();
                long ITGRC_Ecode = Convert.ToInt64(ITGRC_code);
                long DivHeadCode = 0; // Added By Aumento :: SR102194
                decimal? OPHeadCode = 0; // Added By Aumento :: SR102194

                if (ITGRC_code != null)
                {
                    var ITGRC_ = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == ITGRC_Ecode).Select(x => new { Name = x.FIRSTNAME + " " + x.LASTNAME }).FirstOrDefault();
                    if (ITGRC_ != null)
                    {
                        ITGRC_Head = ITGRC_.Name;
                    }
                }
                //Added by Aumento for SR91196
                if (empNamelist_old != null)
                {
                    var empNamelist = _arDBContext.ADORGLEVELHEAD.Where(x => x.ISACTIVE == 1 && x.ADORGLEVELID == empNamelist_old.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                    DivHeadCode = empNamelist; // Added By Aumento :: SR102194
                    //var empNamelist = _arDBContext.ADORGLEVELHEAD.Where(x => x.ISACTIVE == 0 && x.ADORGLEVELID == 0).Select(x => x.ADEMPCODE).FirstOrDefault();
                    var D_HeadName = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == empNamelist).Select(x => new { Name = x.FIRSTNAME + " " + x.LASTNAME }).FirstOrDefault();
                    if (D_HeadName != null)
                    {
                        D_Head = D_HeadName.Name;
                    }

                    var ohead_id = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.SYKIID == SYKIList.SYKIID && x.DIVISIONID == empNamelist_old.DIVISIONID && x.ACTIVE == 1).Select(x => x.ADDEDBY).FirstOrDefault();
                    OPHeadCode = ohead_id; // Added By Aumento :: SR102194
                    //var ohead_id = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.SYKIID == SYKIList.SYKIID && x.DIVISIONID == 0 && x.ACTIVE == 1).Select(x => x.ADDEDBY).FirstOrDefault();
                    var O_HeadName = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == ohead_id).Select(x => new { Name = x.FIRSTNAME + " " + x.LASTNAME }).FirstOrDefault();

                    if (O_HeadName != null)
                    {
                        O_Head = O_HeadName.Name;
                    }
                }
                //-----------------------------------------------------------------------------------------------------
                var PeriodSettingList = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == item && x.SYKI == SYKIList.SYKIID).Select(x => new Get_Division_ISSC_Member
                {
                    DivId = x.DIVISIONID,
                    DivisionName = x.DIVISION,
                    ISSCMember_EmpCode = x.ADEMPCODE,
                    OperationName = x.OPERATION,
                    ISSCMember = _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == x.ADEMPCODE).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault(),
                    EndDate = _arDBContext.ASSET_REGISTER_ITGRC_PERIOD.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.DIVISIONID == x.DIVISIONID).Select(x1 => x1.ENDDATE).FirstOrDefault(),

                    ISSC_Member_Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == item && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.FINALSUBMITDATE_ISSCMEMBER).FirstOrDefault(),
                    ISSC_Member_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == item && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.ISSCMEMBER_STATUS).FirstOrDefault(),

                    DivisionHead_Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == item && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.FINALSUBMITDATE_DIVISIONHEAD).FirstOrDefault(),
                    DivisionHead_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == item && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.DIVISION_STATUS).FirstOrDefault(),

                    OperatingHead__Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == item && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.FINALSUBMITDATE_OPERATINGHEAD).FirstOrDefault(),
                    OperatingHead_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == item && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.OPERATIONG_STATUS).FirstOrDefault(),

                    //Added by Aumento for SR91196
                    ITGRCHead_Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == item && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.FINALSUBMITDATE_ITGRCHEAD).FirstOrDefault(),
                    ITGRCHead_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == item && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.ITGRC_STATUS).FirstOrDefault(),

                    ITGRC_Head_Name = ITGRC_Head,
                    Div_Head_EmpCode = DivHeadCode, // Added By Aumento :: SR102194
                    Op_Head_EmpCode = OPHeadCode, // Added By Aumento :: SR102194
                    //AutoApprove = (DivHeadCode == OPHeadCode) ? true : false, // Added By Aumento :: SR102194
                    //Added by Aumento for SR91196
                    //Added by aumento as on 17062024 for SR71836-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------                    
                    DivisionHead_Name = D_Head,
                    OperatingHead_Name = O_Head, // Added By Aumento :: SR102194
                    //IS_ITOPRATION = IsOpMatch  
                    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                }).FirstOrDefault();
                if (PeriodSettingList != null)
                {
                    PeriodSettingList.IS_ITOPRATION = IsOpMatch; // Added By Aumento :: SR102194
                    PeriodSettingList.AutoApprove = (DivHeadCode == OPHeadCode) ? true : false; // Added By Aumento :: SR102194
                }
                ISSCMemberList.Add(PeriodSettingList);
            }
            return ISSCMemberList;
        }

        public List<Get_Division_ISSC_Member> Get_ISSC_Member_Details_For_OpearingHead(int? id)
        {
            var SYKIList = (from data in _arDBContext.SYKI.Where(x => x.ACTIVE == 1)
                            select data).FirstOrDefault();
            // var ISSC_Member = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.DIVISIONHEAD_ID == id).Select(x => x.CREATEDBY).FirstOrDefault();
            var operationid = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.SYKI == SYKIList.SYKIID && x.ADEMPCODE == id).Select(x => x.OPERATIONID).FirstOrDefault(); ;
            var ISSC_Member = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.ACTIVE == 1 && x.SYKIID == SYKIList.SYKIID && x.OPERATIONID == operationid).Select(x => x.ISSCEMPCODE).ToList();
            List<Get_Division_ISSC_Member> ISSCMemberList = new List<Get_Division_ISSC_Member>();
            foreach (var item in ISSC_Member)
            {
                var PeriodSettingList = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == item && x.SYKI == SYKIList.SYKIID).Select(x => new Get_Division_ISSC_Member
                {
                    DivId = x.DIVISIONID,
                    DivisionName = x.DIVISION,
                    ISSCMember_EmpCode = x.ADEMPCODE,
                    ISSCMember = _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == x.ADEMPCODE).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault(),
                    EndDate = _arDBContext.ASSET_REGISTER_ITGRC_PERIOD.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.DIVISIONID == x.DIVISIONID).Select(x1 => x1.ENDDATE).FirstOrDefault(),

                    ISSC_Member_Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == item && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.FINALSUBMITDATE_ISSCMEMBER).FirstOrDefault(),
                    ISSC_Member_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == item && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.ISSCMEMBER_STATUS).FirstOrDefault(),

                    DivisionHead_Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == item && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.FINALSUBMITDATE_DIVISIONHEAD).FirstOrDefault(),
                    DivisionHead_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == item && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.DIVISION_STATUS).FirstOrDefault(),

                    OperatingHead__Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == item && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.FINALSUBMITDATE_OPERATINGHEAD).FirstOrDefault(),
                    OperatingHead_Submit_Status = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.CREATEDBY == item && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.OPERATIONG_STATUS).FirstOrDefault(),

                }).FirstOrDefault();
                ISSCMemberList.Add(PeriodSettingList);
            }
            return ISSCMemberList;
        }

        public int Asset_Approve_By_OperatingHead(CommonAAssetsRegListVM asset_details)
        {
            if (asset_details != null && asset_details.SYKIID > 0 && asset_details.ISSC_Member_EmpCode > 0 && asset_details.Operating_Head_EmpCode > 0)
            {
                //using (var db = new ePortalEntities2())
                //{
                    //Below Code added by aumento as on 11062024 for SR71836----------------------------------------------------------------------
                    //var result_List = db.ASSET_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == asset_details.ISSC_Member_EmpCode && x.SKIID == asset_details.SYKIID && x.ACTIVE == 1 && x.DIVISION_STATUS == 1).ToList();
                    var result_List = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == asset_details.ISSC_Member_EmpCode && x.SKIID == asset_details.SYKIID && x.ACTIVE == 1).ToList();
                    //-----------------------------------------------------------------------------------------------------------------------------
                    result_List.ForEach(ass_det =>
                    {
                        ass_det.OPERATINGHEAD_ID = asset_details.Operating_Head_EmpCode;
                        ass_det.FINALSUBMITDATE_OPERATINGHEAD = DateTime.Now;
                        ass_det.OPERATIONG_STATUS = 1;
                        ass_det.REVIEW_REMARKS_OPERATINGHEAD = asset_details.OperatingHead_Remarks;
                    }
                    );
                _arDBContext.SaveChanges();
                //}
                return 1;//Approved 
            }
            return 2;//There is some problem, please try again later.
        }

        public int Asset_SendBack_By_OperatingHead(CommonAAssetsRegListVM asset_details)
        {
            if (asset_details != null && asset_details.SYKIID > 0 && asset_details.ISSC_Member_EmpCode > 0)
            {
                //using (var db = new ePortalEntities2())
                //{
                    var result_List = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == asset_details.ISSC_Member_EmpCode && x.SKIID == asset_details.SYKIID && x.ACTIVE == 1 && x.DIVISION_STATUS == 1).ToList();
                    result_List.ForEach(ass_det =>
                    {
                        ass_det.ACTIVE = 1;
                        ass_det.SAVE = 1;

                        ass_det.ISSCMEMBER_STATUS = null;
                        //ass_det.ISSC_MEMBER_REASON = null;
                        ass_det.FINALSUBMITDATE_ISSCMEMBER = null;

                        ass_det.DIVISION_STATUS = null;
                        ass_det.FINALSUBMITDATE_DIVISIONHEAD = null;
                        ass_det.REVIEW_REMARKS_DIVISIONHEAD = null;
                        ass_det.UPDATIONDATE = DateTime.Now;
                        ass_det.OPERATIONG_STATUS = null;
                        ass_det.FINALSUBMITDATE_OPERATINGHEAD = null;
                        ass_det.REVIEW_REMARKS_OPERATINGHEAD = asset_details.OperatingHead_Remarks;

                        //Added by Aumento for SR91196
                        ass_det.ITGRC_STATUS = null;
                        ass_det.FINALSUBMITDATE_ITGRCHEAD = null;
                        ass_det.REVIEW_REMARKS_ITGRCHEAD = null;
                        //Added by Aumento for SR91196
                        // START :: Added By Aumento :: SR102194
                        if (ass_det.CLASSIFICATIONID == 1)
                        {
                            ass_det.ISSC_MEMBER_REASON = null;
                        }
                        // END :: Added By Aumento :: SR102194
                    }
                    );
                    _arDBContext.SaveChanges();
                //}
                return 3; //Send back 
            }
            return 2;//There is some problem, please try again later.
        }

        public ISSCMember_details Send_Email_To_ISSC_(CommonAAssetsRegListVM asset_details)
        {
            ISSCMember_details Emp_Details = new ISSCMember_details();
            Emp_Details = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == asset_details.ISSC_Member_EmpCode).Select(x => new ISSCMember_details { EmpEmail = x.EMAILID, EmpName = x.FIRSTNAME + " " + x.LASTNAME }).FirstOrDefault();
            return Emp_Details;
        }
        public int Operating_Head_AssetDetails_FinalSubmit_Check(int Empcode, long? SYKI)
        {
            var check = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.SKIID == SYKI && x.CREATEDBY == Empcode && x.SAVE == 2 && x.ACTIVE == 1 && x.ISSCMEMBER_STATUS == 1 && x.OPERATIONG_STATUS == 1).Count();
            return check;
        }

        public List<PrimaryAsset> BindPrimaryAsset(long? div_Id)
        {
            var currentki = _arDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
            var empcode = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.DIVISIONID == div_Id && x.STATUS == 1 && x.ACTIVE == 1 && x.SYKIID == currentki).Select(x => x.ISSCEMPCODE).FirstOrDefault();
            var result = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == empcode && x.ACTIVE == 1 && x.SKIID == currentki && x.OPERATIONG_STATUS == 1 && x.SAVE == 2 && x.ISSC_MEMBER_REASON == null).Select(x => new PrimaryAsset { PrimaryAssetText = x.PRIMARY, PrimaryAssetId = x.ID }).Distinct().OrderBy(x => x.PrimaryAssetText).ToList();
            return result;
        }

        public List<PrimaryAsset> BindSecondaryyAsset(string P_Id, long? div_Id)
        {
            var empcode = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.DIVISIONID == div_Id && x.STATUS == 1 && x.ACTIVE == 1).Select(x => x.ISSCEMPCODE).FirstOrDefault();

            var result = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == empcode && x.ACTIVE == 1 && x.PRIMARY.Contains(P_Id)).Select(x => new PrimaryAsset { PrimaryAssetText = x.SECONDARY, PrimaryAssetId = x.ID }).ToList();

            return result;
        }

        public int InsertAssetDifiency(DifiencyReport difiency)
        {
            if (difiency != null && difiency.DivisionId > 0 && difiency.operationId > 0)
            {
                var currentki = _arDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
                var empcode = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.DIVISIONID == difiency.DivisionId && x.STATUS == 1 && x.ACTIVE == 1 && x.SYKIID == currentki).Select(x => x.ISSCEMPCODE).FirstOrDefault();                
                if (empcode > 0)
                {
                    using (var transaction = _arDBContext.Database.BeginTransaction())
                    {
                        try
                        {
                            //string difiyId1 = _arDBContext.ASSET_REGISTE_D_ASSET.Max(x => x.ID).ToString(); //Added by aumento for Anupma san UAT
                            //long difiyId = long.Parse(difiyId1.ToString()) + 1; //Added by aumento for Anupma san UAT

                            ASSET_REGISTE_D_ASSET difiy = new ASSET_REGISTE_D_ASSET();
                            //difiy.ID = difiyId; //Added by aumento for Anupma san UAT
                            difiy.OPERATIOID = difiency.operationId;
                            difiy.DIVISIONID = difiency.DivisionId;
                            difiy.PRIMARYASSETID = difiency.SecondaryAsset;
                            difiy.SECONDARYASSET = difiency.PrimaryAssetId;
                            difiy.FEEDBACK = difiency.FeedBack;
                            difiy.ISSCEMPCODE = empcode;
                            difiy.CREATIONDATE = DateTime.Now;
                            difiy.CREATEDBY = difiency.createdby;
                            _arDBContext.Entry(difiy).State = EntityState.Added;
                            _arDBContext.SaveChanges();
                            /*Added By Aumento as on 19042024 Start*/
                            var AMENDMENT_NO_ = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == empcode && x.SKIID == currentki && x.ACTIVE == 1).Select(x => x.AMENDMENT_NO).Max();
                            decimal AMENDMENT_NO;
                            if (AMENDMENT_NO_ != null)
                            {
                                AMENDMENT_NO = (Convert.ToDecimal(AMENDMENT_NO_) + 1);
                            }
                            else { AMENDMENT_NO = 1; }
                            /*Added By Aumento as on 19042024 End*/
                            var result_List = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == empcode && x.SKIID == currentki && x.ACTIVE == 1).ToList();
                            result_List.ForEach(ass_det =>
                            {
                                ass_det.SAVE = 1;

                                ass_det.ISSCMEMBER_STATUS = null;
                                ass_det.ISSC_MEMBER_REASON = null;
                                ass_det.FINALSUBMITDATE_ISSCMEMBER = null;

                                ass_det.DIVISION_STATUS = null;
                                ass_det.FINALSUBMITDATE_DIVISIONHEAD = null;
                                ass_det.REVIEW_REMARKS_DIVISIONHEAD = null;

                                ass_det.OPERATIONG_STATUS = null;
                                ass_det.FINALSUBMITDATE_OPERATINGHEAD = null;
                                ass_det.REVIEW_REMARKS_OPERATINGHEAD = null;
                                ass_det.AMENDMENT_NO = AMENDMENT_NO; //Added By Aumento as on 19042024
                            });
                            _arDBContext.SaveChanges();
                            transaction.Commit();
                            return 1;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                        }
                    }
                }
            }
            return 2;
        }       

        public List<ISSCMember_details> Send_Email_To_ISSC_Member_DivisionHead(DifiencyReport difiency)
        {
            List<ISSCMember_details> Emp_Details = new List<ISSCMember_details>();
            var currentki = _arDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
            var empcode = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.DIVISIONID == difiency.DivisionId && x.STATUS == 1 && x.ACTIVE == 1 && x.SYKIID == currentki).Select(x => x.ISSCEMPCODE).FirstOrDefault();
            //var divisionhead = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == empcode.ISSCEMPCODE).Select(x => x.DIVISIONHEAD_ID).FirstOrDefault();
            //var s = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == empcode.ISSCEMPCODE).Select(x => new ISSCMember_details { EmpEmail = x.EMAILID, EmpName = x.FIRSTNAME + " " + x.LASTNAME }).FirstOrDefault();
            //Emp_Details.Add(s);
            //var s1 = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == divisionhead).Select(x => new ISSCMember_details { EmpEmail = x.EMAILID, EmpName = x.FIRSTNAME + " " + x.LASTNAME }).FirstOrDefault();
            //Emp_Details.Add(s1);           

            var emp = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == empcode).Select(x => new ISSCMember_details { EmpEmail = x.EMAILID, EmpName = x.FIRSTNAME + " " + x.LASTNAME, EmpCode = x.ADEMPCODE }).FirstOrDefault();
            Emp_Details.Add(emp);

            return Emp_Details;
        }

        public EmpDetails GetEmp_Details_Ki_Wise(long Id)
        {
            var currentki = _arDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
            EmpDetails emp = new EmpDetails();
            emp = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.ADEMPCODE == Id && x.SYKI == currentki).Select(x => new EmpDetails { OperationId = x.OPERATIONID, DivisionId = x.DIVISIONID, Degination = x.FUNCTIONALDESIGNATION }).FirstOrDefault();
            return emp;
        }

        public List<CommonAAssetsRegListVM> Get_Asset_Details_Operation_Division_Wise(long SYKI, decimal? operationid, decimal? divisionid)
        {
            List<CommonAAssetsRegListVM> assetlist = new List<CommonAAssetsRegListVM>();
            var emplist = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.SYKIID == SYKI && x.ACTIVE == 1 && x.OPERATIONID == (operationid == 0 ? x.OPERATIONID : operationid) && x.DIVISIONID == (divisionid == 0 ? x.DIVISIONID : divisionid)).Select(x => x.ISSCEMPCODE).ToList();
            foreach (var item in emplist)
            {
                var assets = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == item && x.SKIID == SYKI && x.ACTIVE == 1 && x.SAVE == 2 && x.ISSCMEMBER_STATUS == 1 && x.OPERATIONG_STATUS == 1 && x.DIVISION_STATUS == 1).Select(x => new CommonAAssetsRegListVM
                {
                    ID = x.ID,
                    Primary = x.PRIMARY,
                    Secondary = x.SECONDARY,
                    AssetsID = (decimal)x.ASSETID,
                    Assetlocation = x.LOCATION,
                    AssetOwner = x.OWNER,
                    Custodian = x.CUSTODIAN,
                    AssetUser = x.ASSETUSER,
                    ClassificationID = (decimal)x.CLASSIFICATIONID,
                    Retention_Period = x.RETENTION,
                    Reason_NA = x.ISSC_MEMBER_REASON,
                    ActionType = x.ACTIONTYPE,
                    OperatingHeadName = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.ADEMPCODE == x.CREATEDBY && y.SYKI == x.SKIID).Select(y => y.OPERATION).FirstOrDefault() == null ? "" : _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.ADEMPCODE == x.CREATEDBY && y.SYKI == x.SKIID).Select(y => y.OPERATION).FirstOrDefault(),
                    DivisionHeadName = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.SYKI == x.SKIID && y.ADEMPCODE == x.CREATEDBY).Select(y => y.DIVISION).FirstOrDefault() == null ? "" : _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.SYKI == x.SKIID && y.ADEMPCODE == x.CREATEDBY).Select(y => y.DIVISION).FirstOrDefault(),
                }).ToList();//Dinesh
                assetlist.AddRange(assets);
            }
            return assetlist;
        }

        public List<DivisionWise_Asset_Count> Get_Asset_Count_DivisionWise(long SYKI, decimal? operationid, decimal? divisionid)
        {
            List<DivisionWise_Asset_Count> assetlist = new List<DivisionWise_Asset_Count>();
            var emplist = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.SYKIID == SYKI && x.ACTIVE == 1 && x.OPERATIONID == (operationid == 0 ? x.OPERATIONID : operationid) && x.DIVISIONID == (divisionid == 0 ? x.DIVISIONID : divisionid)).Select(x => new { x.ISSCEMPCODE, x.DIVISIONID }).ToList();
            foreach (var item in emplist)
            {
                DivisionWise_Asset_Count qwerty = new DivisionWise_Asset_Count();
                var assets = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == item.ISSCEMPCODE && x.SKIID == SYKI && x.ACTIVE == 1 & x.SAVE == 2 && x.ISSCMEMBER_STATUS == 1 && x.OPERATIONG_STATUS == 1 && x.DIVISION_STATUS == 1).Select(x => new
                {
                    DivisionName = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.ACTIVE == 1 && y.ADEMPCODE == item.ISSCEMPCODE && y.SYKI == SYKI && y.DIVISIONID == item.DIVISIONID).Select(y => y.DIVISION).FirstOrDefault(),
                    Id = x.ID,
                    ClassificationID = x.CLASSIFICATIONID,
                    ISSC_MEMBER_REASON = x.ISSC_MEMBER_REASON
                }).ToList();
                if (assets.Count() > 0)
                {
                    qwerty = assets.Select(x => new DivisionWise_Asset_Count { DivisionName = x.DivisionName }).FirstOrDefault();

                    var s1 = assets.Where(x => x.ClassificationID == 1 && x.ISSC_MEMBER_REASON == null).FirstOrDefault();
                    if (s1 != null)
                        qwerty.TSCount = assets.Where(x => x.ClassificationID == 1 && x.ISSC_MEMBER_REASON == null).Count();
                    else
                        qwerty.TSCount = 0;

                    var s2 = assets.Where(x => x.ClassificationID == 2 && x.ISSC_MEMBER_REASON == null).FirstOrDefault();
                    if (s2 != null)
                        qwerty.SCount = assets.Where(x => x.ClassificationID == 2 && x.ISSC_MEMBER_REASON == null).Count();
                    else
                        qwerty.SCount = 0;

                    var s3 = assets.Where(x => x.ClassificationID == 3 && x.ISSC_MEMBER_REASON == null).FirstOrDefault();
                    if (s3 != null)
                        qwerty.ISCount = assets.Where(x => x.ClassificationID == 3 && x.ISSC_MEMBER_REASON == null).Count();
                    else
                        qwerty.ISCount = 0;

                    assetlist.Add(qwerty);
                }
            }
            return assetlist;
        }

        public List<Asset_Approver_Remarks> GetRemrks(long SYKI, int Empcode)
        {
            var result = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == Empcode && x.SKIID == SYKI && x.ACTIVE == 1).Select(x => new Asset_Approver_Remarks
            {
                Divison_Remarks = x.REVIEW_REMARKS_DIVISIONHEAD,
                DivisionDate = x.FINALSUBMITDATE_DIVISIONHEAD,
                Operating_Remarks = x.REVIEW_REMARKS_OPERATINGHEAD,
                OperatingDate = x.FINALSUBMITDATE_OPERATINGHEAD

            }).ToList();
            return result;
        }

        public List<Asset_Approver_Remarks_ISSCM> GetRemrksForISSCMember(long SYKI, int Empcode)
        {
            bool isOpMatch = IsOperationMatch(Empcode); // This is a C# method, not translatable to SQL

            // Step 1: Fetch raw data from DB
            var rawData = _arDBContext.ASSET_REGISTER_ASSETDETAILS
                .Where(x => x.CREATEDBY == Empcode && x.SKIID == SYKI && x.ACTIVE == 1)
                .ToList(); // Materialize the query

            // Step 2: Project to your model in memory
            var result = rawData.Select(x => new Asset_Approver_Remarks_ISSCM
            {
                Divison_Remarks = x.REVIEW_REMARKS_DIVISIONHEAD,
                DivisionDate = x.FINALSUBMITDATE_DIVISIONHEAD,
                Operating_Remarks = x.REVIEW_REMARKS_OPERATINGHEAD,
                OperatingDate = x.FINALSUBMITDATE_OPERATINGHEAD,
                ModifiedDate = x.UPDATIONDATE,
                ITGRC_Remarks = x.REVIEW_REMARKS_ITGRCHEAD,
                ITGRCDate = x.FINALSUBMITDATE_ITGRCHEAD,
                IS_ITOPRATION = isOpMatch
            }).ToList();
            
            return result;
        }


        public List<Asset_Approver_Remarks> GetRemrksForOPHead(long SYKI, int Empcode)
        {
            bool IsOpMatch = IsOperationMatch(Empcode); // Added By Aumento :: SR102194
                                                        
            var rawData = _arDBContext.ASSET_REGISTER_ASSETDETAILS
                .Where(x => x.CREATEDBY == Empcode && x.SKIID == SYKI && x.ACTIVE == 1)
                .ToList();

            
            var result = rawData.Select(x => new Asset_Approver_Remarks
            {
                Divison_Remarks = x.REVIEW_REMARKS_DIVISIONHEAD,
                DivisionDate = x.FINALSUBMITDATE_DIVISIONHEAD,
                Operating_Remarks = x.REVIEW_REMARKS_OPERATINGHEAD,
                OperatingDate = x.FINALSUBMITDATE_OPERATINGHEAD,
                ITGRC_Remarks = x.REVIEW_REMARKS_ITGRCHEAD,
                ITGRCDate = x.FINALSUBMITDATE_ITGRCHEAD,
                IS_ITOPRATION = IsOpMatch
            }).ToList();

            return result;
        }

        public List<Asset_Deficency_Remarks> GetDeficencyRemrks(long SYKI, int Empcode)
        {
            var result1 = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.ACTIVE == 1 && y.ADEMPCODE == Empcode && y.SYKI == SYKI).Select(y => new { y.OPERATIONID, y.DIVISIONID }).FirstOrDefault();
            var result = _arDBContext.ASSET_REGISTE_D_ASSET.Where(x => x.ISSCEMPCODE == Empcode && x.OPERATIOID == result1.OPERATIONID && x.DIVISIONID == result1.DIVISIONID)
                .Select(x => new Asset_Deficency_Remarks
                {
                    PrimaryAsset = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(p => p.ID == x.PRIMARYASSETID).Select(p => p.PRIMARY).FirstOrDefault(),
                    SecondaryAsset = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(p => p.ID == x.PRIMARYASSETID).Select(p => p.SECONDARY).FirstOrDefault(),
                    Remarks = x.FEEDBACK,
                    cdate = x.CREATIONDATE
                }).ToList();
            return result;
        }

        public int Check_OrganizationMapping(long SYKI, int Empcode)
        {
            var curent_year_deatil = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == Empcode && x.SYKI == SYKI).Select(x => new { x.OPERATIONID, x.DIVISIONID }).FirstOrDefault();
            int get_prevyear_detail = _arDBContext.ASSET_REGISTER_ORG_MAPPING.Where(x => x.CURRENT_SYKIID == SYKI && x.CURRENT_OPERATIONID == curent_year_deatil.OPERATIONID && x.CURRENT_DIVISIONID == curent_year_deatil.DIVISIONID && x.STATUS == 1).Count();
            return get_prevyear_detail;
        }

        public int Check_Issc_Member_Nomination(long SYKI, int Empcode)
        {
            var curent_year_deatil = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == Empcode && x.SYKI == SYKI).Select(x => new { x.OPERATIONID, x.DIVISIONID }).FirstOrDefault();
            var check = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.STATUS == 1 && x.SYKIID == SYKI && x.OPERATIONID == curent_year_deatil.OPERATIONID && x.DIVISIONID == curent_year_deatil.DIVISIONID && x.ISSCEMPCODE == Empcode).Count();
            return check;
        }

        public long GetOldISSC_MemberByCurrentISSCMemCode(long SYKI, int Empcode)
        {
            var OldISSCMemCode = _arDBContext.ASSET_USER_MAPPING.Where(x => x.NEWISSC_MEMBERCODE == Empcode && x.SYKIID == SYKI).Select(x => new { x.OLDISSC_MEMBERCODE }).FirstOrDefault();
            if(OldISSCMemCode!=null)
            {
                return Convert.ToInt16(OldISSCMemCode.OLDISSC_MEMBERCODE);
            }
            else
            {
                return 0;
            }
            
        }

        public AssetRegistrationSYKIViewModel GetAssetRegistrationSYKIListFor_OperatingHead_Dashboard(long SYKI, int Empcode)
        {
            AssetRegistrationSYKIViewModel assetRegistrationSYKIViewModel = new AssetRegistrationSYKIViewModel();
            var curent_year_deatil = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == Empcode && x.SYKI == SYKI).Select(x => new { x.OPERATIONID, x.DIVISIONID }).FirstOrDefault();
            var get_prevyear_detail = _arDBContext.ASSET_REGISTER_ORG_MAPPING.Where(x => x.CURRENT_SYKIID == SYKI && x.CURRENT_OPERATIONID == curent_year_deatil.OPERATIONID && x.STATUS == 1).Select(x => x).FirstOrDefault();
            if (get_prevyear_detail != null)
            {
                var get_prevyear_userid = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.STATUS == 1 && x.SYKIID == get_prevyear_detail.PREVIOUS_SYKIID && x.OPERATIONID == get_prevyear_detail.PREVIOUS_OPERATIONID).Select(x => x).ToList();

                if (get_prevyear_userid.Count > 0)
                {
                    foreach (var item in get_prevyear_userid)
                    {
                        assetRegistrationSYKIViewModel._SYKIList.Add(new AssetRegistrationSYKIList
                        {
                            DIVISIONNAMEN = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == item.ISSCEMPCODE && x.SYKI == item.SYKIID).Select(x => x.DIVISION).FirstOrDefault(),
                            KICODE = _arDBContext.SYKI.Where(x => x.SYKIID == item.SYKIID).Select(x => x.KICODE).FirstOrDefault(),
                            SYKIID = item.SYKIID,
                            ADEMPCODE = item.ISSCEMPCODE,
                            OperationName = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == item.ISSCEMPCODE && x.SYKI == item.SYKIID).Select(x => x.OPERATION).FirstOrDefault(),
                        });
                    }
                }
            }
            return assetRegistrationSYKIViewModel;
        }

        public AssetRegistrationSYKIViewModelOH GetAssetRegistrationSYKIListFor_OperatingHeadDeails_Dashboard(long SYKI, int Empcode)
        {
            AssetRegistrationSYKIViewModelOH assetRegistrationSYKIViewModel = new AssetRegistrationSYKIViewModelOH();
            var curent_year_deatil = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == Empcode && x.SYKI == SYKI).Select(x => x.OPERATIONID).ToList();
            var get_prevyear_detail = _arDBContext.ASSET_REGISTER_ORG_MAPPING.Where(x => x.CURRENT_SYKIID == SYKI && curent_year_deatil.Contains(x.CURRENT_OPERATIONID) && x.STATUS == 1).Select(x => x).ToList();
            if (get_prevyear_detail != null)
            {
                var SYKIList = get_prevyear_detail.Select(y => y.PREVIOUS_SYKIID).ToList();
                var OperationIDList = get_prevyear_detail.Select(y => (decimal)y.PREVIOUS_OPERATIONID).ToList();
                var get_prevyear_userid = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.STATUS == 1 && SYKIList.Contains(x.SYKIID) && OperationIDList.Contains(x.OPERATIONID)).Select(x => x).ToList();

                if (get_prevyear_userid.Count > 0)
                {
                    foreach (var item in get_prevyear_userid)
                    {
                        assetRegistrationSYKIViewModel._SYKIList.Add(new AssetRegistrationSYKIListForOH
                        {
                            DIVISIONNAMEN = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == item.ISSCEMPCODE && x.SYKI == item.SYKIID).Select(x => x.DIVISION).FirstOrDefault(),
                            KICODE = _arDBContext.SYKI.Where(x => x.SYKIID == item.SYKIID).Select(x => x.KICODE).FirstOrDefault(),
                            SYKIID = item.SYKIID,
                            ADEMPCODE = item.ISSCEMPCODE,
                            OperationName = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == item.ISSCEMPCODE && x.SYKI == item.SYKIID).Select(x => x.OPERATION).FirstOrDefault(),
                            OPID = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == item.ISSCEMPCODE && x.SYKI == item.SYKIID).Select(x => x.OPERATIONID).FirstOrDefault(),

                        });
                    }
                }
            }
            return assetRegistrationSYKIViewModel;
        }

        //Dinesh
        public List<OrgMappingVM> GetOrgMappingData(A_SearchParameterList _ParamList)
        {
            var ViewToList = _arDBContext.ASSET_REGISTER_ORG_MAPPING.Where(x => (_ParamList.SYKIID == null || x.CURRENT_SYKIID == _ParamList.SYKIID)
           && ((_ParamList.OPERATIONID == null || _ParamList.OPERATIONID == 0) || x.CURRENT_OPERATIONID == _ParamList.OPERATIONID) && ((_ParamList.DIVISIONID == null || _ParamList.DIVISIONID == 0) || x.CURRENT_DIVISIONID == _ParamList.DIVISIONID)).Select(x => new OrgMappingVM
           {
               OrgMappingId = x.ID,
               CurrentSYKIID = x.CURRENT_SYKIID,
               PreviousSYKIID = x.PREVIOUS_SYKIID,
               CurrentKi = _arDBContext.SYKI.Where(y => y.SYKIID == x.CURRENT_SYKIID).Select(y => y.KICODE).FirstOrDefault(),
               Previouski = _arDBContext.SYKI.Where(y => y.SYKIID == x.PREVIOUS_SYKIID).Select(y => y.KICODE).FirstOrDefault(),
               CurrentOperationID = x.CURRENT_OPERATIONID,
               PreviousOperationID = x.PREVIOUS_OPERATIONID,
               // OPERATIONNAME="",
               CurrentOperation = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.OPERATIONID == x.CURRENT_OPERATIONID && y.SYKI == x.CURRENT_SYKIID).Select(y => y.OPERATION).FirstOrDefault(),
               PreviousOperation = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.OPERATIONID == x.PREVIOUS_OPERATIONID && y.SYKI == x.PREVIOUS_SYKIID).Select(y => y.OPERATION).FirstOrDefault(),
               CurrentDevisionID = x.CURRENT_DIVISIONID,
               PreviousDevisionID = x.PREVIOUS_DIVISIONID,
               // DIVISIONNAMEN="",
               CurrentDivision = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.SYKI == x.CURRENT_SYKIID && y.DIVISIONID == x.CURRENT_DIVISIONID).OrderBy(y => y.ADEMPCODE).Select(y => y.DIVISION).FirstOrDefault(),
               PreviousDivision = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.SYKI == x.PREVIOUS_SYKIID && y.DIVISIONID == x.PREVIOUS_DIVISIONID).OrderBy(y => y.ADEMPCODE).Select(y => y.DIVISION).FirstOrDefault(),
           }).OrderBy(x => x.CurrentOperation).ToList();
            return ViewToList;
        }
        public AssetRegistrationSYKIViewModel GetAssetAllSYKIList()
        {
            AssetRegistrationSYKIViewModel assetRegistrationSYKIViewModel = new AssetRegistrationSYKIViewModel();

            var SYKIList = (from data in _arDBContext.SYKI
                            select data).OrderByDescending(x => x.SYKIID).ToList();
            if (SYKIList.Count > 0)
            {
                foreach (var kiList in SYKIList)
                {
                    assetRegistrationSYKIViewModel._SYKIList.Add(new AssetRegistrationSYKIList { KICODE = kiList.KICODE, SYKIID = kiList.SYKIID, ACTIVE = kiList.ACTIVE });
                }
            }
            return assetRegistrationSYKIViewModel;
        }
        public int OrgMappingSaveData(OrgMappingVM orgMappingVM)
        {
            try
            {
                var ASSET_REGISTER_ORG_MAPPING = new ASSET_REGISTER_ORG_MAPPING();

                

                ASSET_REGISTER_ORG_MAPPING = _arDBContext.ASSET_REGISTER_ORG_MAPPING.Find(orgMappingVM.OrgMappingId);
                if (ASSET_REGISTER_ORG_MAPPING != null)
                {
                    ASSET_REGISTER_ORG_MAPPING.CURRENT_SYKIID = orgMappingVM.CurrentSYKIID;
                    ASSET_REGISTER_ORG_MAPPING.CURRENT_OPERATIONID = orgMappingVM.CurrentOperationID;
                    ASSET_REGISTER_ORG_MAPPING.CURRENT_DIVISIONID = orgMappingVM.CurrentDevisionID;
                    ASSET_REGISTER_ORG_MAPPING.PREVIOUS_SYKIID = orgMappingVM.PreviousSYKIID;
                    ASSET_REGISTER_ORG_MAPPING.PREVIOUS_OPERATIONID = orgMappingVM.PreviousOperationID;
                    ASSET_REGISTER_ORG_MAPPING.PREVIOUS_DIVISIONID = orgMappingVM.PreviousDevisionID;
                    ASSET_REGISTER_ORG_MAPPING.UPDATED_BY = orgMappingVM.UpdatedBY;
                    ASSET_REGISTER_ORG_MAPPING.UPDATED_DATE = orgMappingVM.UpdatedDate;
                    ASSET_REGISTER_ORG_MAPPING.STATUS = 1;

                    _arDBContext.Entry(ASSET_REGISTER_ORG_MAPPING).State = EntityState.Modified;
                    _arDBContext.SaveChanges();
                }
                else
                {
                    //string OrgMappingId1 = _arDBContext.ASSET_REGISTER_ORG_MAPPING.Max(x => x.ID).ToString(); //Added by aumento for Anupma san UAT
                    //long OrgMapId = long.Parse(OrgMappingId1.ToString()) + 1; //Added by aumento for Anupma san UAT

                    ASSET_REGISTER_ORG_MAPPING = new ASSET_REGISTER_ORG_MAPPING();
                    //ASSET_REGISTER_ORG_MAPPING.ID = OrgMapId; //Added by aumento for Anupma san UAT
                    ASSET_REGISTER_ORG_MAPPING.CURRENT_SYKIID = orgMappingVM.CurrentSYKIID;
                    ASSET_REGISTER_ORG_MAPPING.CURRENT_OPERATIONID = orgMappingVM.CurrentOperationID;
                    ASSET_REGISTER_ORG_MAPPING.CURRENT_DIVISIONID = orgMappingVM.CurrentDevisionID;
                    ASSET_REGISTER_ORG_MAPPING.PREVIOUS_SYKIID = orgMappingVM.PreviousSYKIID;
                    ASSET_REGISTER_ORG_MAPPING.PREVIOUS_OPERATIONID = orgMappingVM.PreviousOperationID;
                    ASSET_REGISTER_ORG_MAPPING.PREVIOUS_DIVISIONID = orgMappingVM.PreviousDevisionID;
                    ASSET_REGISTER_ORG_MAPPING.CREATED_BY = orgMappingVM.CreatedBY;
                    ASSET_REGISTER_ORG_MAPPING.CREATED_DATE = orgMappingVM.CreatedDate;
                    ASSET_REGISTER_ORG_MAPPING.STATUS = 1;

                    _arDBContext.Entry(ASSET_REGISTER_ORG_MAPPING).State = EntityState.Added;
                    _arDBContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                return 0;
            }
            return 1;
        }
        public OrgMappingVM GetAddEditOrgMappingData(int? OrgMappingId)
        {
            var ViewToList = _arDBContext.ASSET_REGISTER_ORG_MAPPING.Where(x => x.ID == OrgMappingId).Select(x => new OrgMappingVM
            {
                OrgMappingId = x.ID,
                CurrentSYKIID = x.CURRENT_SYKIID,
                PreviousSYKIID = x.PREVIOUS_SYKIID,
                CurrentKi = _arDBContext.SYKI.Where(y => y.SYKIID == x.CURRENT_SYKIID).Select(y => y.KICODE).FirstOrDefault(),
                Previouski = _arDBContext.SYKI.Where(y => y.SYKIID == x.PREVIOUS_SYKIID).Select(y => y.KICODE).FirstOrDefault(),
                CurrentOperationID = x.CURRENT_OPERATIONID,
                PreviousOperationID = x.PREVIOUS_OPERATIONID,
                // OPERATIONNAME="",
                CurrentOperation = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.OPERATIONID == x.CURRENT_OPERATIONID && y.SYKI == x.CURRENT_SYKIID).Select(y => y.OPERATION).FirstOrDefault(),
                PreviousOperation = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.OPERATIONID == x.PREVIOUS_OPERATIONID && y.SYKI == x.PREVIOUS_SYKIID).Select(y => y.OPERATION).FirstOrDefault(),
                CurrentDevisionID = x.CURRENT_DIVISIONID,
                PreviousDevisionID = x.PREVIOUS_DIVISIONID,
                // DIVISIONNAMEN="",
                CurrentDivision = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.SYKI == x.CURRENT_SYKIID && y.DIVISIONID == x.CURRENT_DIVISIONID).OrderBy(y => y.ADEMPCODE).Select(y => y.DIVISION).FirstOrDefault(),
                PreviousDivision = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.SYKI == x.PREVIOUS_SYKIID && y.DIVISIONID == x.PREVIOUS_DIVISIONID).OrderBy(y => y.ADEMPCODE).Select(y => y.DIVISION).FirstOrDefault(),
            }).OrderBy(x => x.CurrentOperation).FirstOrDefault();
            return ViewToList;
        }
        //****

        // Reminder Mail
        public MailerReminderVM GetStart_Dt_End_Dt_ISSC_Nomination(long SYKI)
        {
            MailerReminderVM maildetails = new MailerReminderVM();
            maildetails = _arDBContext.ASSET_REGISTER_PERIODSETTING.Where(x => x.SYKIID == SYKI && x.ACTIVE == 1)
                .Select(x => new MailerReminderVM { StartDate = x.STARTDATE.ToString("dd/MM/yyyy"), EndDate = x.ENDDATE.ToString("dd/MM/yyyy") }).FirstOrDefault();
            return maildetails;
        }

        public List<MailerReminderVM> GetReminderDetailsForISSCMember_Nomination()
        {
            var ViewToList = _arDBContext.ASSET_REGISTER_MAIL.Where(x => x.REMINDERFOR == 1).ToList().Select((x, index) => new MailerReminderVM
            {
                SNo = index + 1,
                ReminerDate = Convert.ToString(x.REMINDERDATE),
                SYKI = _arDBContext.SYKI.Where(x1 => x1.SYKIID == x.SKIID).Select(x1 => x1.KICODE).FirstOrDefault(),
                ReminderId = (long)x.ID
            }).OrderBy(x => x.ReminerDate).ToList();
            return ViewToList;
        }

        public string InsertISSC_Reminder(MailerReminderVM mailer)
        {
            string s = "Data Saved Sucessfully.";
            //CultureInfo provider = new CultureInfo("en-US");
            CultureInfo provider = CultureInfo.InvariantCulture;

            DateTime sdt = DateTime.ParseExact(mailer.ReminerDate, "dd/MM/yyyy", provider);

            if (mailer.ReminderId > 0)
            {
                var PeriodSetting = _arDBContext.ASSET_REGISTER_MAIL.Where(x => x.ID == mailer.ReminderId).FirstOrDefault();
                if (PeriodSetting != null)
                {
                    PeriodSetting.REMINDERDATE = sdt;
                    _arDBContext.Entry(PeriodSetting).State = EntityState.Modified;
                    _arDBContext.SaveChanges();
                    s = "Data has been updated Sucessfully.";
                }
            }
            else
            {
                ASSET_REGISTER_MAIL PeriodSetting = new ASSET_REGISTER_MAIL();
                PeriodSetting.SKIID = mailer.SYKIID;
                PeriodSetting.REMINDERDATE = sdt;
                PeriodSetting.REMINDERFOR = mailer.ReminderFor;
                _arDBContext.Entry(PeriodSetting).State = EntityState.Added;
                _arDBContext.SaveChanges();
            }
            return s;
        }

        public string GetReminderDate(long id)
        {
            var s = _arDBContext.ASSET_REGISTER_MAIL.Where(x => x.ID == id).Select(x => x.REMINDERDATE.ToString("dd/MM/yyyy")).FirstOrDefault();
            return s;
        }

        public List<MailerReminderVM> GetReminderDetailsForISSCMember_Approval()
        {
            var ViewToList = _arDBContext.ASSET_REGISTER_MAIL.Where(x => x.REMINDERFOR == 2).ToList().Select((x, index) => new MailerReminderVM
            {
                SNo = index + 1,
                ReminerDate = Convert.ToString(x.REMINDERDATE),
                SYKI = _arDBContext.SYKI.Where(x1 => x1.SYKIID == x.SKIID).Select(x1 => x1.KICODE).FirstOrDefault(),
                ReminderId = (long)x.ID
            }).OrderBy(x => x.ReminerDate).ToList();
            return ViewToList;
        }

        public MailerReminderVM GetStart_Dt_End_Dt_ISSC_Member(long SYKI)
        {
            MailerReminderVM maildetails = new MailerReminderVM();
            maildetails = _arDBContext.ASSET_REGISTER_ITGRC_PERIOD.Where(x => x.SKIID == SYKI && x.ACTIVE == 1)
                .Select(x => new MailerReminderVM { StartDate = Convert.ToString(x.STARTDATE), EndDate = Convert.ToString(x.ENDDATE) }).OrderByDescending(x => x.EndDate).FirstOrDefault();
            return maildetails;
        }

        public List<CommonAAssetsRegListVM> GetCommon_Asset_SubmittedDetails(long SYKI, long Userid)
        {
            var PeriodSettingList = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.SKIID == SYKI && x.CREATEDBY == Userid && x.ACTIONTYPE == 1).Select(x => new CommonAAssetsRegListVM
            {
                Secondary = x.SECONDARY,
                ISSC_Member_Reason = x.ISSC_MEMBER_REASON
            }).ToList();
            return PeriodSettingList;
        }

        public List<ISSC_Member_Nomination_deatilsVM> Get_ISSC_member_Nomintion_pending(long SYKI, long Userid)
        {
            List<ISSC_Member_Nomination_deatilsVM> pendinglist = new List<ISSC_Member_Nomination_deatilsVM>();
            var kicode = _arDBContext.SYKI.Where(x => x.ACTIVE == 1).Select(x => x.KICODE).FirstOrDefault();
            var empname = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == Userid).Select(x => x.FIRSTNAME + " " + x.LASTNAME).FirstOrDefault();
            var enddate = _arDBContext.ASSET_REGISTER_PERIODSETTING.Where(x => x.SYKIID == SYKI && x.ACTIVE == 1).Select(x => x.ENDDATE.ToString("dd/MM/yyyy")).FirstOrDefault();
            var PeriodSettingList = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.SYKIID == SYKI && x.ADDEDBY == Userid && x.ACTIVE == 1 && x.STATUS == 1).Count();
            var OperatingHead_Check = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == SYKI && x.ADEMPCODE == Userid).Select(x => x.ADFUNCTIONALDESIGNATIONID).FirstOrDefault();
            if (OperatingHead_Check == 4)
            {
                if (PeriodSettingList == 0)
                {
                    pendinglist = _arDBContext.ASSET_REGISTER_PERIODSETTING.Where(x => x.SYKIID == SYKI && x.ACTIVE == 1).Select(x => new ISSC_Member_Nomination_deatilsVM
                    {
                        Empcode = Userid,
                        ProjectTitle = "ISSC Member Nomination",
                        SYKI = kicode,
                        EMpName = empname,
                        EndDate = enddate
                    }).ToList();
                }
                else
                {

                }
            }
            return pendinglist;
        }

        public List<Get_Division_ISSC_Member> Asset_pendingDetails_ISSCMembers(int? id)
        {
            List<Get_Division_ISSC_Member> PeriodSettingListITGRC = new List<Get_Division_ISSC_Member>();
            var SYKIList = (from data in _arDBContext.SYKI.Where(x => x.ACTIVE == 1)
                            select data).FirstOrDefault();
            var curent_year_deatil = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == id && x.SYKI == SYKIList.SYKIID).Select(x => new { x.OPERATIONID, x.DIVISIONID }).FirstOrDefault();
            var check = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.STATUS == 1 && x.SYKIID == SYKIList.SYKIID && x.OPERATIONID == curent_year_deatil.OPERATIONID && x.DIVISIONID == curent_year_deatil.DIVISIONID && x.ISSCEMPCODE == id).Count();
            if (check > 0)
            {
                var s = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.SKIID == SYKIList.SYKIID && x.CREATEDBY == id && x.ACTIVE == 1).Select(x => x).FirstOrDefault();
                if (s != null)
                {
                    if (s.SAVE == 2 && s.ISSCMEMBER_STATUS == 1 && s.OPERATIONG_STATUS == null)
                    {
                        PeriodSettingListITGRC = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == id && x.SYKI == SYKIList.SYKIID).Select(x => new Get_Division_ISSC_Member
                        {
                            DivId = x.DIVISIONID,
                            DivisionName = x.DIVISION,
                            SYKI = SYKIList.KICODE,
                            //ISSCMember =Convert.ToString(_arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x1 => x1.SYKIID == SYKIList.SYKIID && x1.DIVISIONID == x.DIVISIONID).Select(x1=>x1.ISSCEMPCODE).FirstOrDefault()),
                            ISSCMember = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x1 => x1.SYKIID == SYKIList.SYKIID && x1.DIVISIONID == x.DIVISIONID).Count() > 0 ? _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(p1 => p1.SYKIID == SYKIList.SYKIID && p1.DIVISIONID == x.DIVISIONID).Select(p1 => p1.ISSCEMPCODE).FirstOrDefault()).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault() : null,
                            EndDate = _arDBContext.ASSET_REGISTER_ITGRC_PERIOD.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.DIVISIONID == x.DIVISIONID).Select(x1 => x1.ENDDATE).FirstOrDefault(),
                            ProjectTitle = "Asset Register Update",
                            ISSCMember_EmpCode = _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(p1 => p1.SYKIID == SYKIList.SYKIID && p1.DIVISIONID == x.DIVISIONID).Select(p1 => p1.ISSCEMPCODE).FirstOrDefault()).Select(y => y.ADEMPCODE).FirstOrDefault(),
                            ISSC_Member_Submit_Date = s.FINALSUBMITDATE_ISSCMEMBER,
                            DivisionHeadSubmitStatus = s.DIVISION_STATUS == null ? "Pending" : "Approved",
                            DivisionHead_Submit_Date = s.FINALSUBMITDATE_DIVISIONHEAD,
                            OperatingHeadSubmitStatus = s.OPERATIONG_STATUS == null ? "Pending" : "Approved",

                        }).ToList();
                    }
                    else
                    {

                    }
                }
                else
                {

                }
            }
            else
            {

            }
            return PeriodSettingListITGRC;
        }

        public List<Get_Division_ISSC_Member> Get_Asset_Register_Details_For_OpearingHead(int? id)
        {
            var SYKIList = (from data in _arDBContext.SYKI.Where(x => x.ACTIVE == 1)
                            select data).FirstOrDefault();
            List<Get_Division_ISSC_Member> ISSCMemberList = new List<Get_Division_ISSC_Member>();
            var OperatingHead_Check = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == SYKIList.SYKIID && x.ADEMPCODE == id).Select(x => x.ADFUNCTIONALDESIGNATIONID).FirstOrDefault();
            if (OperatingHead_Check == 4)
            {
                // var ISSC_Member = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.DIVISIONHEAD_ID == id).Select(x => x.CREATEDBY).FirstOrDefault();
                var operationid = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.SYKI == SYKIList.SYKIID && x.ADEMPCODE == id).Select(x => x.OPERATIONID).FirstOrDefault(); ;
                var ISSC_Member = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.ACTIVE == 1 && x.SYKIID == SYKIList.SYKIID && x.OPERATIONID == operationid).Select(x => x.ISSCEMPCODE).ToList();

                foreach (var item in ISSC_Member)
                {
                    var statuscheck = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.SKIID == SYKIList.SYKIID && x.CREATEDBY == item && x.SAVE == 2 && x.ACTIVE == 1 && x.ISSCMEMBER_STATUS == 1).Select(x => new { x.DIVISION_STATUS, x.OPERATIONG_STATUS, x.FINALSUBMITDATE_ISSCMEMBER, x.FINALSUBMITDATE_DIVISIONHEAD }).FirstOrDefault();
                    if (statuscheck != null)
                    {
                        if (statuscheck.DIVISION_STATUS != null && statuscheck.OPERATIONG_STATUS == null)
                        {
                            var PeriodSettingList = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == item && x.SYKI == SYKIList.SYKIID).Select(x => new Get_Division_ISSC_Member
                            {
                                SYKI = SYKIList.KICODE,
                                DivId = x.DIVISIONID,
                                DivisionName = x.DIVISION,
                                ISSCMember_EmpCode = x.ADEMPCODE,
                                ISSCMember = _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == x.ADEMPCODE).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault(),
                                EndDate = _arDBContext.ASSET_REGISTER_ITGRC_PERIOD.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.DIVISIONID == x.DIVISIONID).Select(x1 => x1.ENDDATE).FirstOrDefault(),
                                ProjectTitle = "Asset Register Approval",
                                // Url = "/AssetRegistration/AssetDetailsApproveByOperatingHead?ISSC_Code=" + x.ADEMPCODE + "",
                                Url = "/AssetRegistration/AssetDashboardForOperatingHead",
                                ISSC_Member_Submit_Date = statuscheck.FINALSUBMITDATE_ISSCMEMBER,
                                DivisionHeadSubmitStatus = statuscheck.DIVISION_STATUS == null ? "Pending" : "Approved",
                                DivisionHead_Submit_Date = statuscheck.FINALSUBMITDATE_DIVISIONHEAD,
                                OperatingHeadSubmitStatus = statuscheck.OPERATIONG_STATUS == null ? "Pending" : "Approved",
                            }).FirstOrDefault();
                            ISSCMemberList.Add(PeriodSettingList);
                        }
                    }
                }
            }
            else if (OperatingHead_Check == 3)
            {
                // var ISSC_Member = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.DIVISIONHEAD_ID == id).Select(x => x.CREATEDBY).FirstOrDefault();
                var divi_id = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ACTIVE == 1 && x.SYKI == SYKIList.SYKIID && x.ADEMPCODE == id).Select(x => x.DIVISIONID).FirstOrDefault(); ;
                var ISSC_Member = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.ACTIVE == 1 && x.SYKIID == SYKIList.SYKIID && x.DIVISIONID == divi_id).Select(x => x.ISSCEMPCODE).ToList();

                foreach (var item in ISSC_Member)
                {
                    var statuscheck = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.SKIID == SYKIList.SYKIID && x.CREATEDBY == item && x.SAVE == 2 && x.ACTIVE == 1 && x.ISSCMEMBER_STATUS == 1).Select(x => new { x.DIVISION_STATUS, x.OPERATIONG_STATUS, x.FINALSUBMITDATE_ISSCMEMBER, x.FINALSUBMITDATE_DIVISIONHEAD }).FirstOrDefault();
                    if (statuscheck != null)
                    {
                        if (statuscheck.DIVISION_STATUS == null && statuscheck.OPERATIONG_STATUS == null)
                        {
                            var PeriodSettingList = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == item && x.SYKI == SYKIList.SYKIID).Select(x => new Get_Division_ISSC_Member
                            {
                                SYKI = SYKIList.KICODE,
                                DivId = x.DIVISIONID,
                                DivisionName = x.DIVISION,
                                ISSCMember_EmpCode = x.ADEMPCODE,
                                ISSCMember = _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == x.ADEMPCODE).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault(),
                                EndDate = _arDBContext.ASSET_REGISTER_ITGRC_PERIOD.Where(x1 => x1.SKIID == SYKIList.SYKIID && x1.DIVISIONID == x.DIVISIONID).Select(x1 => x1.ENDDATE).FirstOrDefault(),
                                ProjectTitle = "Asset Register Approval",
                                //Url = "/AssetRegistration/AssetDetailsApproveByDivisionHead?ISSC_Code=" + x.ADEMPCODE + "",
                                Url = "/AssetRegistration/AssetDashboardForDivisionHead",
                                ISSC_Member_Submit_Date = statuscheck.FINALSUBMITDATE_ISSCMEMBER,
                                DivisionHeadSubmitStatus = statuscheck.DIVISION_STATUS == null ? "Pending" : "Approved",
                                DivisionHead_Submit_Date = statuscheck.FINALSUBMITDATE_DIVISIONHEAD,
                                OperatingHeadSubmitStatus = statuscheck.OPERATIONG_STATUS == null ? "Pending" : "Approved",
                            }).FirstOrDefault();
                            ISSCMemberList.Add(PeriodSettingList);
                        }
                    }
                }
            }
            return ISSCMemberList;
        }

        public List<AssetRegistrationSearchModel> GetAssetRegisterSunmitStatus(long? SYKIID, long? OPERATIONID, long? DIVISIONID)
        {
            var ViewToListold = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.ACTIVE == 1 && x.STATUS == 1 && x.SYKIID == SYKIID
           && (OPERATIONID == null || x.OPERATIONID == OPERATIONID) && (DIVISIONID == null || x.DIVISIONID == DIVISIONID)).ToList();
            var ViewToList = ViewToListold.Select(x => new AssetRegistrationSearchModel
            {
                SYKIID = x.SYKIID,
                SYKI = x.SYKI.KICODE,
                OPERATIONID = x.OPERATIONID.ToString(),
                OPERATIONNAME = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.OPERATIONID == x.OPERATIONID && y.SYKI == x.SYKIID).Select(y => y.OPERATION).FirstOrDefault(),
                DIVHDHDID = x.DIVISIONID,
                DIVISIONNAMEN = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(y => y.SYKI == x.SYKIID && y.DIVISIONID == x.DIVISIONID).OrderBy(y => y.ADEMPCODE).Select(y => y.DIVISION).FirstOrDefault(),

                EmpName = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ISSCEMPCODE && x1.SAVE == 2 && x1.ACTIVE == 1 && x1.ISSCMEMBER_STATUS == 1).Count() > 0 ? _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == x.ISSCEMPCODE).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault() : "",
                ISSC_EMPCODE = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ISSCEMPCODE && x1.SAVE == 2 && x1.ACTIVE == 1 && x1.ISSCMEMBER_STATUS == 1).Count() > 0 ? _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ISSCEMPCODE && x1.SAVE == 2 && x1.ACTIVE == 1 && x1.ISSCMEMBER_STATUS == 1).Select(x1 => x1.CREATEDBY).FirstOrDefault() : null,
                Div_Head = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ISSCEMPCODE && x1.SAVE == 2 && x1.ACTIVE == 1 && x1.ISSCMEMBER_STATUS == 1 && x1.DIVISION_STATUS == 1).Count() > 0 ? _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == (_arDBContext.ADORGLEVELHEAD.Where(x2 => x2.ISACTIVE == 1 && x2.ADORGLEVELID == x.DIVISIONID).Select(x2 => x2.ADEMPCODE).FirstOrDefault())).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault() : "",
                Opr_Head = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ISSCEMPCODE && x1.SAVE == 2 && x1.ACTIVE == 1 && x1.ISSCMEMBER_STATUS == 1 && x1.DIVISION_STATUS == 1 && x1.OPERATIONG_STATUS == 1).Count() > 0 ? _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == x.ADDEDBY).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault() : "",
                Div_EMPCODE = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ISSCEMPCODE && x1.SAVE == 2 && x1.ACTIVE == 1 && x1.ISSCMEMBER_STATUS == 1 && x1.DIVISION_STATUS == 1).Count() > 0 ? _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ISSCEMPCODE && x1.SAVE == 2 && x1.ACTIVE == 1 && x1.ISSCMEMBER_STATUS == 1 && x1.DIVISION_STATUS == 1).Select(x1 => x1.DIVISIONHEAD_ID).FirstOrDefault() : null,
                Op_EMPCODE = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ISSCEMPCODE && x1.SAVE == 2 && x1.ACTIVE == 1 && x1.ISSCMEMBER_STATUS == 1 && x1.DIVISION_STATUS == 1 && x1.OPERATIONG_STATUS == 1).Count() > 0 ? _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == x.ISSCEMPCODE && x1.SAVE == 2 && x1.ACTIVE == 1 && x1.ISSCMEMBER_STATUS == 1 && x1.DIVISION_STATUS == 1 && x1.OPERATIONG_STATUS == 1).Select(x1 => x1.OPERATINGHEAD_ID).FirstOrDefault() : null,
            }).OrderBy(x => x.OPERATIONNAME).ToList();
            return ViewToList;

        }

        //Added By Aumento Start
        public Get_AssetRegister_DigitalSign_Detail Get_AssetRegister_DigitalSign_Detail(long? UserID, long? SYKIID)
        {
            //Added by auemnto for the report NA change in Report===========================
            string D_Head = "NA";
            string O_Head = "NA";
           

            var SYKIList = (from data in _arDBContext.SYKI.Where(x => x.ACTIVE == 1)
                            select data).FirstOrDefault();

            var empNamelist_old = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.SYKI == SYKIList.SYKIID && x.ADEMPCODE == UserID).Select(x => new { x.OPERATION, x.DIVISIONID, x.DIVISION }).FirstOrDefault();


            if (empNamelist_old != null)
            {
                var empNamelist = _arDBContext.ADORGLEVELHEAD.Where(x => x.ISACTIVE == 1 && x.ADORGLEVELID == empNamelist_old.DIVISIONID).Select(x => x.ADEMPCODE).FirstOrDefault();
                var D_HeadName = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == empNamelist).Select(x => new { Name = x.FIRSTNAME + " " + x.LASTNAME }).FirstOrDefault();
                if (D_HeadName != null)
                {
                    D_Head = D_HeadName.Name;
                }             

               

                var ohead_id = _arDBContext.ASSET_REGISTER_MEMNOMINATION.Where(x => x.SYKIID == SYKIList.SYKIID && x.DIVISIONID == empNamelist_old.DIVISIONID && x.ACTIVE == 1).Select(x => x.ADDEDBY).FirstOrDefault();
                var O_HeadName = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == ohead_id).Select(x => new { Name = x.FIRSTNAME + " " + x.LASTNAME }).FirstOrDefault();

                if (O_HeadName != null)
                {
                    O_Head = O_HeadName.Name;
                }
                

            }
            //===================================================================


            //var DivisionHead_ECode = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == UserID && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.DIVISIONHEAD_ID).FirstOrDefault();
            //var OperatingHead_ECode = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == UserID && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.OPERATINGHEAD_ID).FirstOrDefault();
            //Added by Aumento for SR91196
            string ITGRC_Head = string.Empty;
            string ITGRC_code = GetITGRCValue();
            long ITGRC_Ecode = Convert.ToInt64(ITGRC_code);

            if (ITGRC_code != null)
            {
                var ITGRC_ = _arDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == ITGRC_Ecode).Select(x => new { Name = x.FIRSTNAME + " " + x.LASTNAME }).FirstOrDefault();
                if (ITGRC_ != null)
                {
                    ITGRC_Head = ITGRC_.Name;
                }
            }
            //Added by Aumento for SR91196
            var DigitalSign_DetailList = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == UserID && x.SYKI == SYKIID).Select(x => new Get_AssetRegister_DigitalSign_Detail
            {
                ISSCMember_Name = _arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == x.ADEMPCODE).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault(),
                ISSC_Member_Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == UserID && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.FINALSUBMITDATE_ISSCMEMBER).FirstOrDefault(),
                DivisionHead_Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == UserID && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.FINALSUBMITDATE_DIVISIONHEAD).FirstOrDefault(),
                OperatingHead_Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == UserID && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.FINALSUBMITDATE_OPERATINGHEAD).FirstOrDefault(),
                //Added by auemnto for the report NA change in Report===========================
                //DivisionHead_Name = DivisionHead_ECode == null ? "" : (_arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == DivisionHead_ECode).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault()),
                //OperatingHead_Name = OperatingHead_ECode == null ? "" : (_arDBContext.ADEMPLOYEE.Where(y => y.ADEMPCODE == OperatingHead_ECode).Select(y => y.FIRSTNAME + " " + y.LASTNAME).FirstOrDefault()),
                //Added by Aumento for SR91196
                ITGRCHead_Submit_Date = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x1 => x1.SKIID == SYKIID && x1.CREATEDBY == UserID && x1.ACTIVE == 1 && x1.SAVE == 2).Select(x1 => x1.FINALSUBMITDATE_ITGRCHEAD).FirstOrDefault(),
                ITGRC_Head_Name = ITGRC_Head,
                //Added by Aumento for SR91196
                DivisionHead_Name = D_Head,
                OperatingHead_Name = O_Head,
                //==============================================================================
            }).FirstOrDefault();
            return DigitalSign_DetailList;
        }
        public Get_AssetRegister_Header_Detail Get_AssetRegister_Header_Detail(long? UserID, long? SYKIID)
        {
            var Header_DetailList = _arDBContext.ASSETREGISTERDOCUMENTCONTROL.Where(x => x.SYKIID == SYKIID).Select(x => new Get_AssetRegister_Header_Detail
            {
                DOCUMENT_TITLE = _arDBContext.ASSETREGISTERDOCUMENTCONTROL.Where(x1 => x1.SYKIID == SYKIID).Select(y => y.DOCUMENT_TITLE).FirstOrDefault(),
                DOC_VERSION_NO = _arDBContext.ASSETREGISTERDOCUMENTCONTROL.Where(x1 => x1.SYKIID == SYKIID).Select(y => y.DOC_VERSION_NO).FirstOrDefault(),
                DATE_OF_RELEASE = _arDBContext.ASSETREGISTERDOCUMENTCONTROL.Where(x1 => x1.SYKIID == SYKIID).Select(y => y.DATE_OF_RELEASE).FirstOrDefault(),
                DOCUMENT_NUMBER = _arDBContext.ASSETREGISTERDOCUMENTCONTROL.Where(x1 => x1.SYKIID == SYKIID).Select(y => y.DOCUMENT_NUMBER).FirstOrDefault(),
                AMENDMENT = _arDBContext.ASSETREGISTERDOCUMENTCONTROL.Where(x1 => x1.SYKIID == SYKIID).Select(y => y.AMENDMENT).FirstOrDefault(),
                OPERATION = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x1 => x1.ADEMPCODE == UserID && x1.SYKI == SYKIID).Select(y => y.OPERATION).FirstOrDefault(),
                DIVISION = _arDBContext.VW_ASSOCIATELVLDETAILS.Where(x1 => x1.ADEMPCODE == UserID && x1.SYKI == SYKIID).Select(y => y.DIVISION).FirstOrDefault(),
            }).FirstOrDefault();
            return Header_DetailList;
        }

        //Added by aumento as on 17062024 for SR71836------------------------------------------------------------------------------------------
        // public int UpdateAssetRegister(long SYKI, int Empcode) // Added By Aumento :: SR102194
        public int UpdateAssetRegister(long SYKI, int Empcode, bool AutoApproveToDiv, long? itgrccode) // Added By Aumento :: SR102194
        {
            using (var transaction = _arDBContext.Database.BeginTransaction())
            {
                try
                {
                    decimal _itgrccode = (itgrccode != null) ? (decimal)itgrccode : 0; // Added By Aumento :: SR102194
                    var AMENDMENT_NO_ = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == Empcode && x.SKIID == SYKI && x.ACTIVE == 1).Select(x => x.AMENDMENT_NO).Max();
                    decimal AMENDMENT_NO;
                    if (AMENDMENT_NO_ != null)
                    {
                        AMENDMENT_NO = (Convert.ToDecimal(AMENDMENT_NO_) + 1);
                    }
                    else { AMENDMENT_NO = 1; }

                    var result_List = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == Empcode && x.SKIID == SYKI && x.ACTIVE == 1).ToList();
                    result_List.ForEach(ass_det =>
                    {
                        ass_det.SAVE = 1;

                        ass_det.ISSCMEMBER_STATUS = null;
                        ass_det.ISSC_MEMBER_REASON = null;
                        ass_det.FINALSUBMITDATE_ISSCMEMBER = null;
                        // START :: Added By Aumento :: SR102194
                        if (AutoApproveToDiv)
                        {
                            ass_det.DIVISION_STATUS = 1;
                            ass_det.FINALSUBMITDATE_DIVISIONHEAD = DateTime.Now;
                            ass_det.REVIEW_REMARKS_DIVISIONHEAD = "Auto Approved Becasue Division head and Operation head both are Same";

                        }
                        else
                        {
                            ass_det.DIVISION_STATUS = null;
                            ass_det.FINALSUBMITDATE_DIVISIONHEAD = null;
                            ass_det.REVIEW_REMARKS_DIVISIONHEAD = null;
                        }
                        // END :: Added By Aumento :: SR102194

                        ass_det.OPERATIONG_STATUS = null;
                        ass_det.FINALSUBMITDATE_OPERATINGHEAD = null;
                        ass_det.REVIEW_REMARKS_OPERATINGHEAD = null;

                        //Added by Aumento for SR91196
                        ass_det.ITGRC_STATUS = null;
                        ass_det.FINALSUBMITDATE_ITGRCHEAD = null;
                        ass_det.REVIEW_REMARKS_ITGRCHEAD = null;
                        // START :: Added By Aumento :: SR102194
                        if (_itgrccode != 0)
                        {
                            ass_det.ITGRC_ID = _itgrccode;
                        }
                        // END :: Added By Aumento :: SR102194
                        //Added by Aumento for SR91196

                        ass_det.AMENDMENT_NO = AMENDMENT_NO;
                    });
                    _arDBContext.SaveChanges();
                    transaction.Commit();
                    return 1;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }

            return 2;
        }
        // START :: Added By Aumento :: SR102194
        public int UpdateAssetRegisterAutoApprove(long SYKI, int Empcode, bool AutoApproveToDiv, long? itgrccode)
        {
            using (var transaction = _arDBContext.Database.BeginTransaction())
            {
                try
                {


                    var result_List = _arDBContext.ASSET_REGISTER_ASSETDETAILS.Where(x => x.CREATEDBY == Empcode && x.SKIID == SYKI && x.ACTIVE == 1).ToList();
                    result_List.ForEach(ass_det =>
                    {
                        ass_det.SAVE = 1;

                        if (AutoApproveToDiv)
                        {
                            ass_det.DIVISION_STATUS = 1;
                            ass_det.FINALSUBMITDATE_DIVISIONHEAD = DateTime.Now;
                            ass_det.REVIEW_REMARKS_DIVISIONHEAD = "Auto Approved Becasue Division head and Operation head both are Same";

                        }
                        else
                        {
                            ass_det.DIVISION_STATUS = null;
                            ass_det.FINALSUBMITDATE_DIVISIONHEAD = null;
                            ass_det.REVIEW_REMARKS_DIVISIONHEAD = null;
                        }


                    });

                    _arDBContext.SaveChanges();
                    transaction.Commit();
                    return 1;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }

            return 2;
        }
        // END :: Added By Aumento :: SR102194
        //public int UpdateAmendment(long? SYKI)
        //{
        //    int temp = 0;

        //    if (SYKI > 0)
        //    {
        //        var prr = _arDBContext.ASSETREGISTERDOCUMENTCONTROL
        //                    .Where(a => a.SYKIID == SYKI)
        //                    .ToList();

        //        if (prr != null && prr.Any())
        //        {
        //            //var amendment1 = _arDBContext.ASSETREGISTERDOCUMENTCONTROL
        //            //                    .Where(x => x.SYKIID == SYKI)
        //            //                    .Select(x => x.AMENDMENT)
        //            //                    .FirstOrDefault();

        //            foreach (var item in prr)
        //            {
        //                item.AMENDMENT = item.AMENDMENT + 1;
        //                _arDBContext.Entry(item).State = EntityState.Modified;
        //            }

        //            _arDBContext.SaveChanges();
        //            temp = 1;
        //        }
        //        else
        //        {
        //            temp = 2; // No records found for the provided SYKI
        //        }
        //    }
        //    else
        //    {
        //        temp = 3; // Invalid Userid or SYKI
        //    }

        //    return temp;
        //}

        //Added By Aumento End
        //Added by Aumento for SR91196
        public String GetITGRCValue()
        {
            var SelectedParameter = (from v in _arDBContext.SYPARAMETERS
                                     where v.PARAMNAME == "ASSET_REG_ITGRC_APP"
                                     select v
                                  ).ToList();

            return (SelectedParameter.FirstOrDefault().PARAMVALUE);
        }
        //Added by Aumento for SR91196
        // START :: Added By Aumento :: SR102194
        public bool IsOperationMatch(long? userId)
        {
            bool itOperation;
            string ITGRC_code = GetITGRCValue();
            long ITGRC_Ecode = Convert.ToInt64(ITGRC_code);
            var SYKIList = _arDBContext.SYKI.Where(x => x.ACTIVE == 1).FirstOrDefault();
            var user = _arDBContext.VW_ASSOCIATELVLDETAILS
                                   .Where(x => x.SYKI == SYKIList.SYKIID && x.ADEMPCODE == userId)
                                   .Select(x => new { x.OPERATIONID })
                                   .FirstOrDefault();
            var itgrc_head_id = _arDBContext.VW_ASSOCIATELVLDETAILS
                                            .Where(x => x.SYKI == SYKIList.SYKIID && x.ADEMPCODE == ITGRC_Ecode)
                                            .Select(x => new { x.OPERATIONID })
                                            .FirstOrDefault();
            if (user.OPERATIONID == itgrc_head_id.OPERATIONID)
            {
                itOperation = true;
            }
            else
            {
                itOperation = false;
            }
            return itOperation;
        }
        // END :: Added By Aumento :: SR102194
    }
}
