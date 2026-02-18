

using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.Infrastructure.Repositories;
using ePortal.ViewModels;
using System;

namespace ePortal.Application.Services
{

    public class AssetRegistrationService : IAssetRegistrationService
    {
       private readonly AssetRegistrationRepository _objAssetRegistrationRepositry;
        public AssetRegistrationService(AssetRegistrationRepository objAssetRegistrationRepository)
        {
            _objAssetRegistrationRepositry = objAssetRegistrationRepository;
        }

        public AssetRegistrationSYKIViewModel GetAssetRegistrationSYKIList()
        {
            var SYKIList = _objAssetRegistrationRepositry.GetAssetRegistrationSYKIList();
            return SYKIList;
        }

        public AssetRegistrationSYKIViewModel GetAssetRegistrationSYKIListForViewTeam()
        {
            var SYKIList = _objAssetRegistrationRepositry.GetAssetRegistrationSYKIListForViewTeam();
            return SYKIList;
        }

        public int? GetAssetUserDeailsForDivisionHeadByLoginUserID(long userid,long Syki)
        {
            var OldUserId = _objAssetRegistrationRepositry.GetAssetUserDeailsForDivisionHeadByLoginUserID(userid, Syki);
            return OldUserId;
        }

        public int? GetAssetUserDeailsForOPHeadByLoginUserID(long userid, long Syki)
        {
            var OldUserId = _objAssetRegistrationRepositry.GetAssetUserDeailsForOPHeadByLoginUserID(userid, Syki);
            return OldUserId;
        }

        public AssetRegistrationSYKIViewModel GetAssetRegistrationSYKIListForDashboard()
        {
            var SYKIList = _objAssetRegistrationRepositry.GetAssetRegistrationSYKIListForDashboard();
            return SYKIList;
        }

        public PeriodForISSCMemberNominationVM InsertUpdatePeriodSettingDetail(PeriodForISSCMemberNominationVM periodSettingVM)
        {
            var PeriodSettingdata = _objAssetRegistrationRepositry.InsertUpdatePeriodSettingDetail(periodSettingVM);
            return periodSettingVM;

        }
        //public PeriodForISSCMemberNominationVM DeletePeriodForISSCMemberNomination(PeriodForISSCMemberNominationVM periodSettingVMD)
        //{
        //    var PeriodSettingdata = _objAssetRegistrationRepositry.InsertUpdatePeriodSettingDetail(periodSettingVMD);
        //    return periodSettingVMD;

        //}
        public List<PeriodForISSCMemberNominationListVM> GetAsset_Register_periodList()
        {
            var objresult = _objAssetRegistrationRepositry.GetPeriodSettingList();
            return objresult;
        }

        //==========================//
        public A00DataViewModel GetA00ADORGLEVELList()
        {
            var AdOrgLevelList = _objAssetRegistrationRepositry.GetA00ADORGLEVELList();
            return AdOrgLevelList;
        }
        public A00_VW_ASSOCIATELVLDETAILS GetA00_VW_ASSOCIATELVLDETAILS(AssetRegistrationSearchModel objSearchModel)
        {
            var objResult = _objAssetRegistrationRepositry.GetA00_VW_ASSOCIATELVLDETAILS(objSearchModel);
            return objResult;
        }

        //public List<divison> GetDivions()
        //{
        //    var list = _objAssetRegistrationRepositry.getDivisionList();
        //    return list;

        //}
        public List<ADORGLEVEL> GetOrgLevelList(long typeId)
        {
            return _objAssetRegistrationRepositry.GetOrgLevelList(typeId);
        }
        public List<ADORGLEVEL> BindOperation(long? SYKIID)
        {
            return _objAssetRegistrationRepositry.BindOperation(SYKIID);
        }
        public List<SearchParameterList> BindDivision(long? op_Id)
        {
            var op = _objAssetRegistrationRepositry.BindDivision(op_Id);
            return op;
        }

        public long? BindDivisionWithOldOperationHeadEmpCodeByOperationId(long? divId, long? SKYIID)
        {
            var op = _objAssetRegistrationRepositry.BindDivisionWithOldOperationHeadEmpCodeByOperationId(divId, SKYIID);
            return op;
        }

        public string OldOperatingOROldDivHeadRequestAssignToNewOpHeadOrNewDivHead(SameDivisionAndOprationApprovalVM model)
        {
            var op = _objAssetRegistrationRepositry.OldOperatingOROldDivHeadRequestAssignToNewOpHeadOrNewDivHead(model);
            return op;
        }

        public long? BindOldDivisionHeadEmpCodeByDivisionId(long? divId, long? SKYIID)
        {
            var op = _objAssetRegistrationRepositry.BindOldDivisionHeadEmpCodeByDivisionId(divId, SKYIID);
            return op;
        }

        public long? BindOldDivisionISSCMemCodeByDivisionId(long? DivisionHeadEmpCode, long? SKYIID)
        {
            var op = _objAssetRegistrationRepositry.BindOldDivisionISSCMemCodeByDivisionId(DivisionHeadEmpCode, SKYIID);
            return op;
        }


        public List<AssetRegistrationSearchModel> GetViewToITGRCTeamList(long? SYKIID, long? OPERATIONID, long? DIVISIONID)
        {
            var objresult = _objAssetRegistrationRepositry.GetViewToITGRCTeamList(SYKIID, OPERATIONID, DIVISIONID);
            return objresult;
        }

        public PeriodForISSCMemberNominationVM UpdateNomination(PeriodForISSCMemberNominationVM periodSettingVM)
        {
            var objresult = _objAssetRegistrationRepositry.UpdateNomination(periodSettingVM);
            return objresult;
        }

        public GetNominationDetails getNomination(long? Id)
        {
            var objresult = _objAssetRegistrationRepositry.getNomination(Id);
            return objresult;
        }

        public List<empName> GetEmpnameList(long? SYKI, long? OPERATIONID, long? Divisionid)
        {
            var objresult = _objAssetRegistrationRepositry.GetEmpnameList(SYKI, OPERATIONID, Divisionid);
            return objresult;
        }

        public int Update_NominationBy_Admin(long? Id, long? Empid, long? adminid)
        {
            var objresult = _objAssetRegistrationRepositry.Update_NominationBy_Admin(Id, Empid, adminid);
            return objresult;
        }

        public List<OperatingHead> GetOperatingHead(long? SYKI)
        {
            var objresult = _objAssetRegistrationRepositry.GetOperatingHead(SYKI);
            return objresult;
        }
        public List<OperatingHead> GetOperatingHead_For_NomationUpdate(long? SYKI, long? OPERATIONID, long? Divisionid, long EmpId)
        {
            var objresult = _objAssetRegistrationRepositry.GetOperatingHead_For_NomationUpdate(SYKI, OPERATIONID, Divisionid, EmpId);
            return objresult;
        }
        /// <summary>
        /// 
        /// </summary>
        public PeriodForITGRCMemberNominationListVM InsertUpdatePeriodITGRCSettingDetail(PeriodForITGRCMemberNominationListVM periodSettingVM)
        {
            var PeriodSettingdata = _objAssetRegistrationRepositry.InsertUpdatePeriodITGRCSettingDetail(periodSettingVM);
            return periodSettingVM;
        }

        public List<PeriodForITGRCMMemberNominationListVM> GetAsset_Register_period_ITGRC_List()
        {
            var objresult = _objAssetRegistrationRepositry.GetPeriodSettingITGRCList();
            return objresult;
        }

        public PeriodForITGRCMemberNominationListNewVM UpdateITGRCNomination(PeriodForITGRCMemberNominationListNewVM periodSettingVM)
        {
            var objresult = _objAssetRegistrationRepositry.UpdateITGRCNomination(periodSettingVM);
            return objresult;
        }

        public BulkUpdatePeriod BulkUpdateITGRCNomination(BulkUpdatePeriod bulkUpdatePeriod)
        {
            var objresult = _objAssetRegistrationRepositry.BulkUpdateITGRCNomination(bulkUpdatePeriod);
            return objresult;
        }

        public List<OperatingHead> GetISSCMemberList(long? SYKI, long? OPERATIONID, long? Divisionid)
        {
            var objresult = _objAssetRegistrationRepositry.GetISSCMemberList(SYKI, OPERATIONID, Divisionid);
            return objresult;
        }

        public List<OperatingHead> GetISSCMemberForBulkMailList(long? SYKI, string[] ids)
        {
            var objresult = _objAssetRegistrationRepositry.GetISSCMemberForBulkMailList(SYKI, ids);
            return objresult;
        }

        public List<PeriodForITGRCMMemberNominationListVM> GetAsset_Register_period_ITGRC_List_WithID(int? id)
        {
            var objresult = _objAssetRegistrationRepositry.GetPeriodSettingITGRCListWithID(id);
            return objresult;
        }


        /// <summary>
        /// 
        /// </summary>
        public CommonAssetsRegListVM InsertUpdateCommonAssetRegisterSettingDetail(CommonAssetsRegListVM periodSettingVM)
        {
            var PeriodSettingdata = _objAssetRegistrationRepositry.InsertUpdateCommonAssetRegisterSettingDetail(periodSettingVM);
            return periodSettingVM;
        }

        public CommonAssetsRegListVM UpdateCommonAssetRegister(CommonAssetsRegListVM periodSettingVM)
        {
            var objresult = _objAssetRegistrationRepositry.UpdateCommonAssetRegister(periodSettingVM);
            return objresult;
        }

        /// <summary>
        /// Common Assets Register
        /// </summary>
        /// <returns></returns>
        public List<AssetType> GetAssetTypeList()
        {
            var list = _objAssetRegistrationRepositry.GetAssetList();
            return list;
        }

        public List<AssetClassification> GetAssetClassificationList()
        {
            var list = _objAssetRegistrationRepositry.GetAssetClassificationList();
            return list;
        }

        public List<CommonAAssetsRegListVM> Get_CommonAsset_Register_List()
        {
            var objresult = _objAssetRegistrationRepositry.Get_CommonAsset_Register_List();
            return objresult;
        }

        //public List<CommonAAssetsRegListVM> Get_CommonAsset_Register_WithID(int? id)
        //{
        //    var objresult = _objAssetRegistrationRepositry.Get_CommonAsset_Register_WithID(id);
        //    return objresult;
        //}

        public Get_Division_ISSC_Member Get_Division_For_ISSC_Member(int? id)
        {
            var objresult = _objAssetRegistrationRepositry.Get_Division_For_ISSC_Member(id);
            return objresult;
        }

        public List<Primary_AssetDeatils_VM> Get_Primary_Asset(long Userid, long? SYKI)
        {
            var objresult = _objAssetRegistrationRepositry.Get_Primary_Asset(Userid, SYKI);
            return objresult;
        }
        public List<CommonAAssetsRegListVM> Get_Asset_Details_With_Common_Asset(long Userid, long? SYKI)
        {
            var objresult = _objAssetRegistrationRepositry.Get_Asset_Details_With_Common_Asset(Userid, SYKI);
            return objresult;
        }
        public int Insert_AssetDetails(CommonAAssetsRegListVM assetdetails)
        {
            var objresult = _objAssetRegistrationRepositry.Insert_AssetDetails(assetdetails);
            return objresult;
        }
        public int Last_year_Asset_Detail_Only_Non_CommonAsset(long Userid, long? SYKI)
        {
            var objresult = _objAssetRegistrationRepositry.Last_year_Asset_Detail_Only_Non_CommonAsset(Userid, SYKI);
            return objresult;
        }
        public CommonAAssetsRegListVM Get_Edit_Asset_Details(int? Id)
        {
            var objresult = _objAssetRegistrationRepositry.Get_Edit_Asset_Details(Id);
            return objresult;
        }
        public int Delete_AssetDetails(CommonAAssetsRegListVM assetdetails)
        {
            var objresult = _objAssetRegistrationRepositry.Delete_AssetDetails(assetdetails);
            return objresult;
        }

        public int FinalSubmit_AssetDetails(CommonAAssetsRegListVM assetdetails)
        {
            var objresult = _objAssetRegistrationRepositry.FinalSubmit_AssetDetails(assetdetails);
            return objresult;
        }
        public int ISSC_Member_AssetDetails_FinalSubmit_Check(int Empcode, long? SYKI)
        {
            var objresult = _objAssetRegistrationRepositry.ISSC_Member_AssetDetails_FinalSubmit_Check(Empcode, SYKI);
            return objresult;
        }
        public int ISSC_Member_AssetDetails_EndDate_Check(int Empcode, long? SYKI)
        {
            var objresult = _objAssetRegistrationRepositry.ISSC_Member_AssetDetails_EndDate_Check(Empcode, SYKI);
            return objresult;
        }
        public List<CommonAAssetsRegListVM> Get_Last_Ki_Asset_DetailsNew(long? Userid, long? SYKI,long? DivId)
        {
            var objresult = _objAssetRegistrationRepositry.Get_Last_Ki_Asset_DetailsNew(Userid, SYKI, DivId);
            return objresult;
        }

        public List<CommonAAssetsRegListVM> Get_Last_Ki_Asset_DetailsOperationID(long? Userid, long? SYKI, long? OPID)
        {
            var objresult = _objAssetRegistrationRepositry.Get_Last_Ki_Asset_DetailsOperationID(Userid, SYKI, OPID);
            return objresult;
        }

        public List<CommonAAssetsRegListVM> Get_Last_Ki_Asset_Details(long? Userid, long? SYKI)
        {
            var objresult = _objAssetRegistrationRepositry.Get_Last_Ki_Asset_Details(Userid, SYKI);
            return objresult;
        }

        public DivisionHead_Details Get_DivisionHead_(long? SYKI, long Empcode)
        {
            var objresult = _objAssetRegistrationRepositry.Get_DivisionHead_(SYKI, Empcode);
            return objresult;
        }

        public DivisionHead_Details Get_OperatingHead_(long? SYKI, long Empcode,long Userid)
        {
            var objresult = _objAssetRegistrationRepositry.Get_OperatingHead_(SYKI, Empcode, Userid);
            return objresult;
        }

        public DivisionHead_Details Get_OperatingHead_Old(long? SYKI, long Empcode)
        {
            var objresult = _objAssetRegistrationRepositry.Get_OperatingHead_Old(SYKI, Empcode);
            return objresult;
        }

        public Get_Division_ISSC_Member Get_ISSC_Member_Details_For_DivisionHead(int? id)
        {
            var objresult = _objAssetRegistrationRepositry.Get_ISSC_Member_Details_For_DivisionHead(id);
            return objresult;
        }

        public List<Get_Division_ISSC_Member> Get_ISSC_Members_Detail_For_DivisionHead(int? id)
        {
            var objresult = _objAssetRegistrationRepositry.Get_ISSC_Members_Detail_For_DivisionHead(id);
            return objresult;
        }

        //Added by Aumento for SR91196
        public List<Get_Division_ISSC_Member> Get_ISSC_Members_Detail_For_ITGRCHead(int? id)
        {
            var objresult = _objAssetRegistrationRepositry.Get_ISSC_Members_Detail_For_ITGRCHead(id);
            return objresult;
        }
        public List<Asset_Dvision_Deatils_VM> Get_ISSC_Members_Division_Details_For_ITGRCHead(int? id)
        {
            var objresult = _objAssetRegistrationRepositry.Get_ISSC_Members_Division_Details_For_ITGRCHead(id);
            return objresult;
        }
        public int ITGRC_Head_AssetDetails_FinalSubmit_Check(int Empcode, long? SYKI, long ISSC_Code)
        {
            var objresult = _objAssetRegistrationRepositry.ITGRC_Head_AssetDetails_FinalSubmit_Check(Empcode, SYKI, ISSC_Code);
            return objresult;
        }
        public DivisionHead_Details Get_ITGRCHead_(long? SYKI, long Empcode)
        {
            var objresult = _objAssetRegistrationRepositry.Get_ITGRCHead_(SYKI, Empcode);
            return objresult;
        }
        public int Asset_Approve_By_ITGRCHead(CommonAAssetsRegListVM asset_details)
        {
            var objresult = _objAssetRegistrationRepositry.Asset_Approve_By_ITGRCHead(asset_details);
            return objresult;
        }
        public int Asset_SendBack_By_ITGRCHead(CommonAAssetsRegListVM asset_details)
        {
            var objresult = _objAssetRegistrationRepositry.Asset_SendBack_By_ITGRCHead(asset_details);
            return objresult;
        }
        //Added by Aumento for SR91196 
        public List<Asset_Dvision_Deatils_VM> Get_ISSC_Members_Division_Details_For_DivisionHead(int? id)
        {
            var objresult = _objAssetRegistrationRepositry.Get_ISSC_Members_Division_Details_For_DivisionHead(id);
            return objresult;
        }

        public List<CommonAAssetsRegListVM> Get_ISSC_Member_Asset_For_DivisionHead(long? Userid, long? SYKI)
        {
            var objresult = _objAssetRegistrationRepositry.Get_ISSC_Member_Asset_For_DivisionHead(Userid, SYKI);
            return objresult;
        }
        public int Asset_Approve_By_DivisionHead(CommonAAssetsRegListVM asset_details)
        {
            var objresult = _objAssetRegistrationRepositry.Asset_Approve_By_DivisionHead(asset_details);
            return objresult;
        }
        public int Asset_SendBack_By_DivisionHead(CommonAAssetsRegListVM asset_details)
        {
            var objresult = _objAssetRegistrationRepositry.Asset_SendBack_By_DivisionHead(asset_details);
            return objresult;
        }

        public List<ISSCMember_details> Send_Email_To_ISSC_OperatingHead(CommonAAssetsRegListVM asset_details)
        {
            var objresult = _objAssetRegistrationRepositry.Send_Email_To_ISSC_OperatingHead(asset_details);
            return objresult;
        }
        public List<ISSCMember_details> Send_Email_To_ISSC_OperatingHead_FroSendBack(CommonAAssetsRegListVM asset_details)
        {
            var objresult = _objAssetRegistrationRepositry.Send_Email_To_ISSC_OperatingHead_FroSendBack(asset_details);
            return objresult;
        }

        public int Division_Head_AssetDetails_FinalSubmit_Check(int Empcode, long? SYKI, long ISSC_Code)
        {
            var objresult = _objAssetRegistrationRepositry.Division_Head_AssetDetails_FinalSubmit_Check(Empcode, SYKI, ISSC_Code);
            return objresult;
        }
        public List<Get_Division_ISSC_Member> Get_ISSC_Member_Details_For_OpearingHead(int? id)
        {
            var objresult = _objAssetRegistrationRepositry.Get_ISSC_Member_Details_For_OpearingHead(id);
            return objresult;
        }

        public List<Get_Division_ISSC_Member> Get_ISSC_Members_Asset_Details_For_OpearingHead(int? id)
        {
            var objresult = _objAssetRegistrationRepositry.Get_ISSC_Members_Asset_Details_For_OpearingHead(id);
            return objresult;
        }

        public int Asset_Approve_By_OperatingHead(CommonAAssetsRegListVM asset_details)
        {
            var objresult = _objAssetRegistrationRepositry.Asset_Approve_By_OperatingHead(asset_details);
            return objresult;
        }
        public int Asset_SendBack_By_OperatingHead(CommonAAssetsRegListVM asset_details)
        {
            var objresult = _objAssetRegistrationRepositry.Asset_SendBack_By_OperatingHead(asset_details);
            return objresult;
        }
        public ISSCMember_details Send_Email_To_ISSC_(CommonAAssetsRegListVM asset_details)
        {
            var objresult = _objAssetRegistrationRepositry.Send_Email_To_ISSC_(asset_details);
            return objresult;
        }
        public int Operating_Head_AssetDetails_FinalSubmit_Check(int Empcode, long? SYKI)
        {
            var objresult = _objAssetRegistrationRepositry.Operating_Head_AssetDetails_FinalSubmit_Check(Empcode, SYKI);
            return objresult;
        }

        public List<PrimaryAsset> BindPrimaryAsset(long? div_Id)
        {
            var objresult = _objAssetRegistrationRepositry.BindPrimaryAsset(div_Id);
            return objresult;
        }

        public List<PrimaryAsset> BindSecondaryyAsset(string P_Id, long? div_Id)
        {
            var objresult = _objAssetRegistrationRepositry.BindSecondaryyAsset(P_Id, div_Id);
            return objresult;
        }
        public int InsertAssetDifiency(DifiencyReport difiency)
        {
            var objresult = _objAssetRegistrationRepositry.InsertAssetDifiency(difiency);
            return objresult;
        }
       
        public List<ISSCMember_details> Send_Email_To_ISSC_Member_DivisionHead(DifiencyReport difiency)
        {
            var objresult = _objAssetRegistrationRepositry.Send_Email_To_ISSC_Member_DivisionHead(difiency);
            return objresult;
        }

        public EmpDetails GetEmp_Details_Ki_Wise(long Id)
        {
            var objresult = _objAssetRegistrationRepositry.GetEmp_Details_Ki_Wise(Id);
            return objresult;
        }

        public List<CommonAAssetsRegListVM> Get_Asset_Details_Operation_Division_Wise(long SYKI, decimal? operationid, decimal? divisionid)
        {
            var objresult = _objAssetRegistrationRepositry.Get_Asset_Details_Operation_Division_Wise(SYKI, operationid, divisionid);
            return objresult;
        }

        public List<DivisionWise_Asset_Count> Get_Asset_Count_DivisionWise(long SYKI, decimal? operationid, decimal? divisionid)
        {
            var objresult = _objAssetRegistrationRepositry.Get_Asset_Count_DivisionWise(SYKI, operationid, divisionid);
            return objresult;
        }

        public List<Asset_Approver_Remarks> GetRemrks(long SYKI, int Empcode)
        {
            var objresult = _objAssetRegistrationRepositry.GetRemrks(SYKI, Empcode);
            return objresult;
        }

        public List<Asset_Approver_Remarks_ISSCM> GetRemrksForISSCMember(long SYKI, int Empcode)
        {
            var objresult = _objAssetRegistrationRepositry.GetRemrksForISSCMember(SYKI, Empcode);
            return objresult;
        }

        public List<Asset_Approver_Remarks> GetRemrksForOPHead(long SYKI, int Empcode)
        {
            var objresult = _objAssetRegistrationRepositry.GetRemrksForOPHead(SYKI, Empcode);
            return objresult;
        }

        public List<Asset_Deficency_Remarks> GetDeficencyRemrks(long SYKI, int Empcode)
        {
            var objresult = _objAssetRegistrationRepositry.GetDeficencyRemrks(SYKI, Empcode);
            return objresult;
        }

        public int Check_OrganizationMapping(long SYKI, int Empcode)
        {
            var objresult = _objAssetRegistrationRepositry.Check_OrganizationMapping(SYKI, Empcode);
            return objresult;
        }

        public int Check_Issc_Member_Nomination(long SYKI, int Empcode)
        {
            var objresult = _objAssetRegistrationRepositry.Check_Issc_Member_Nomination(SYKI, Empcode);
            return objresult;
        }

        public long GetOldISSC_MemberByCurrentISSCMemCode(long SYKI, int Empcode)
        {
            var objresult = _objAssetRegistrationRepositry.GetOldISSC_MemberByCurrentISSCMemCode(SYKI, Empcode);
            return objresult;
        }

        public AssetRegistrationSYKIViewModel GetAssetRegistrationSYKIListFor_OperatingHead_Dashboard(long SYKI, int Empcode)
        {
            var objresult = _objAssetRegistrationRepositry.GetAssetRegistrationSYKIListFor_OperatingHead_Dashboard(SYKI, Empcode);
            return objresult;
        }

        public AssetRegistrationSYKIViewModelOH GetAssetRegistrationSYKIListFor_OperatingHeadDeails_Dashboard(long SYKI, int Empcode)
        {
            var objresult = _objAssetRegistrationRepositry.GetAssetRegistrationSYKIListFor_OperatingHeadDeails_Dashboard(SYKI, Empcode);
            return objresult;
        }

        //Dinesh
        public List<OrgMappingVM> GetOrgMappingData(A_SearchParameterList _ParamList)
        {
            var objresult = _objAssetRegistrationRepositry.GetOrgMappingData(_ParamList);
            return objresult;
        }
        public AssetRegistrationSYKIViewModel GetAssetAllSYKIList()
        {
            var objresult = _objAssetRegistrationRepositry.GetAssetAllSYKIList();
            return objresult;
        }
        public int OrgMappingSaveData(OrgMappingVM orgMappingVM)
        {
            var objresult = _objAssetRegistrationRepositry.OrgMappingSaveData(orgMappingVM);
            return objresult;
        }
        public OrgMappingVM GetAddEditOrgMappingData(int? OrgMappingId)
        {
            var objresult = _objAssetRegistrationRepositry.GetAddEditOrgMappingData(OrgMappingId);
            return objresult;
        }
        //****
        public AssetRegister_Start_EndDate_ISSC_Member GetStartDate_EndDate_ForAsset_Register(CommonAAssetsRegListVM asset_details)
        {
            var objresult = _objAssetRegistrationRepositry.GetStartDate_EndDate_ForAsset_Register(asset_details);
            return objresult;
        }
        public MailerReminderVM GetStart_Dt_End_Dt_ISSC_Nomination(long SYKI)
        {
            var objresult = _objAssetRegistrationRepositry.GetStart_Dt_End_Dt_ISSC_Nomination(SYKI);
            return objresult;
        }
        public List<MailerReminderVM> GetReminderDetailsForISSCMember_Nomination()
        {
            var objresult = _objAssetRegistrationRepositry.GetReminderDetailsForISSCMember_Nomination();
            return objresult;
        }

        public string InsertISSC_Reminder(MailerReminderVM mailer)
        {
            var objresult = _objAssetRegistrationRepositry.InsertISSC_Reminder(mailer);
            return objresult;
        }

        public string GetReminderDate(long id)
        {
            var objresult = _objAssetRegistrationRepositry.GetReminderDate(id);
            return objresult;
        }

        public List<MailerReminderVM> GetReminderDetailsForISSCMember_Approval()
        {
            var objresult = _objAssetRegistrationRepositry.GetReminderDetailsForISSCMember_Approval();
            return objresult;
        }

        public MailerReminderVM GetStart_Dt_End_Dt_ISSC_Member(long SYKI)
        {
            var objresult = _objAssetRegistrationRepositry.GetStart_Dt_End_Dt_ISSC_Member(SYKI);
            return objresult;
        }

        public List<CommonAAssetsRegListVM> GetCommon_Asset_SubmittedDetails(long SYKI, long Userid)
        {
            var objresult = _objAssetRegistrationRepositry.GetCommon_Asset_SubmittedDetails(SYKI, Userid);
            return objresult;
        }

        public List<ISSC_Member_Nomination_deatilsVM> Get_ISSC_member_Nomintion_pending(long SYKI, long Userid)
        {
            var objresult = _objAssetRegistrationRepositry.Get_ISSC_member_Nomintion_pending(SYKI, Userid);
            return objresult;
        }

        public List<Get_Division_ISSC_Member> Asset_pendingDetails_ISSCMembers(int? id)
        {
            var objresult = _objAssetRegistrationRepositry.Asset_pendingDetails_ISSCMembers(id);
            return objresult;
        }
        public List<Get_Division_ISSC_Member> Get_Asset_Register_Details_For_OpearingHead(int? id)
        {
            var objresult = _objAssetRegistrationRepositry.Get_Asset_Register_Details_For_OpearingHead(id);
            return objresult;
        }

        public List<AssetRegistrationSearchModel> GetAssetRegisterSunmitStatus(long? SYKIID, long? OPERATIONID, long? DIVISIONID)
        {
            var objresult = _objAssetRegistrationRepositry.GetAssetRegisterSunmitStatus(SYKIID, OPERATIONID, DIVISIONID);
            return objresult;
        }

        public int BulkUpdateAssetApplicability(AssetApplicabilityUpdateVM bulkUpdateAssetApplicability)
        {
            var objresult = _objAssetRegistrationRepositry.BulkUpdateAssetApplicability(bulkUpdateAssetApplicability);
            return objresult;
        }

        public List<CommonAAssetsRegListVM> Get_Common_Asset_Details(List<decimal> Ids)
        {
            var objresult = _objAssetRegistrationRepositry.Get_Common_Asset_Details(Ids);
            return objresult;
        }

        //Added By Aumento Start
        public Get_AssetRegister_DigitalSign_Detail Get_AssetRegister_DigitalSign_Detail(long? UserID, long? SYKIID)
        {
            var objresult = _objAssetRegistrationRepositry.Get_AssetRegister_DigitalSign_Detail(UserID, SYKIID);
            return objresult;
        }
        public Get_AssetRegister_Header_Detail Get_AssetRegister_Header_Detail(long? UserID, long? SYKIID)
        {
            var objresult = _objAssetRegistrationRepositry.Get_AssetRegister_Header_Detail(UserID, SYKIID);
            return objresult;
        }
        //Added by aumento as on 17062024 for SR71836------------------------------------------------------------
        // public int UpdateAssetRegister(long SYKI, int Empcode)
        // {
        //   var objresult = _objAssetRegistrationRepositry.UpdateAssetRegister(SYKI, Empcode);
        //  return objresult;
        // }

        // START :: Added By Aumento :: SR102194
        public int UpdateAssetRegister(long SYKI, int Empcode, bool AutoApproveToDiv, long? itgrccode)
        {
            var objresult = _objAssetRegistrationRepositry.UpdateAssetRegister(SYKI, Empcode, AutoApproveToDiv, itgrccode);
            return objresult;
        }

        public int UpdateAssetRegisterAutoApprove(long SYKI, int Empcode, bool AutoApproveToDiv, long? itgrccode)
        {
            var objresult = _objAssetRegistrationRepositry.UpdateAssetRegisterAutoApprove(SYKI, Empcode, AutoApproveToDiv, itgrccode);
            return objresult;
        }
        // END :: Added By Aumento :: SR102194

        //-------------------------------------------------------------------------------------------------------

        //public int UpdateAmendment(long? SYKI)
        //{
        //    var objresult = _objAssetRegistrationRepositry.UpdateAmendment(SYKI);
        //    return objresult;
        //}
        //Added By Aumento End
        //Added by Aumento for SR91196
        public String _GetITGRC()
        {
            return _objAssetRegistrationRepositry.GetITGRCValue();
        }
        // START :: Added By Aumento :: SR102194
        public bool IsOperationMatch(long? userId)
        {
            return _objAssetRegistrationRepositry.IsOperationMatch(userId);
        }
        // ENND :: Added By Aumento :: SR102194

        List<ADORGLEVEL> IAssetRegistrationService.GetOrgLevelList(long typeId)
        {
            return _objAssetRegistrationRepositry.GetOrgLevelList(typeId);
        }

        List<ADORGLEVEL> IAssetRegistrationService.BindOperation(long? SYKIID)
        {
            return _objAssetRegistrationRepositry.BindOperation(SYKIID);
        }
        //Added by Aumento for SR91196
    }

}
