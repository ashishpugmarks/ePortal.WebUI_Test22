

using ePortal.DomainClasses;
using ePortal.ViewModels;

namespace ePortal.Application.Contracts
{
    public interface IAssetRegistrationService
    {
        AssetRegistrationSYKIViewModel GetAssetRegistrationSYKIList();

        AssetRegistrationSYKIViewModel GetAssetRegistrationSYKIListForViewTeam();
        int? GetAssetUserDeailsForDivisionHeadByLoginUserID(long userid,long Syki);
        int? GetAssetUserDeailsForOPHeadByLoginUserID(long userid, long Syki);
        AssetRegistrationSYKIViewModel GetAssetRegistrationSYKIListForDashboard();
        PeriodForISSCMemberNominationVM InsertUpdatePeriodSettingDetail(PeriodForISSCMemberNominationVM periodSettingVM);
        List<PeriodForISSCMemberNominationListVM> GetAsset_Register_periodList();

        // PeriodForISSCMemberNominationVM DeletePeriodForISSCMemberNomination(PeriodForISSCMemberNominationVM periodSettingVMD);
        //============//
        A00DataViewModel GetA00ADORGLEVELList();
        A00_VW_ASSOCIATELVLDETAILS GetA00_VW_ASSOCIATELVLDETAILS(AssetRegistrationSearchModel objSearchModel);

        // List<divison> GetDivions();
        //===========3rd=============//
        List<ADORGLEVEL> GetOrgLevelList(long typeId);
        List<ADORGLEVEL> BindOperation(long? SYKIID);
        List<SearchParameterList> BindDivision(long? op_Id);

        long? BindDivisionWithOldOperationHeadEmpCodeByOperationId(long? op_Id, long? SKYIID);

        long? BindOldDivisionHeadEmpCodeByDivisionId(long? divId, long? SKYIID);
        long? BindOldDivisionISSCMemCodeByDivisionId(long? DivisionHeadEmpCode, long? SKYIID);

        string OldOperatingOROldDivHeadRequestAssignToNewOpHeadOrNewDivHead(SameDivisionAndOprationApprovalVM model);
        List<AssetRegistrationSearchModel> GetViewToITGRCTeamList(long? SYKIID, long? OPERATIONID, long? DIVISIONID);

        PeriodForISSCMemberNominationVM UpdateNomination(PeriodForISSCMemberNominationVM periodSettingVM);

        GetNominationDetails getNomination(long? Id);

        List<empName> GetEmpnameList(long? SYKI, long? OPERATIONID, long? Divisionid);

        List<OperatingHead> GetOperatingHead(long? SYKI);

        List<OperatingHead> GetOperatingHead_For_NomationUpdate(long? SYKI, long? OPERATIONID, long? Divisionid, long EmpId);
        int Update_NominationBy_Admin(long? Id, long? Empid, long? adminid);


        PeriodForITGRCMemberNominationListVM InsertUpdatePeriodITGRCSettingDetail(PeriodForITGRCMemberNominationListVM periodSettingVM);
        List<PeriodForITGRCMMemberNominationListVM> GetAsset_Register_period_ITGRC_List();

        List<PeriodForITGRCMMemberNominationListVM> GetAsset_Register_period_ITGRC_List_WithID(int? id);

        List<OperatingHead> GetISSCMemberList(long? SYKI, long? OPERATIONID, long? Divisionid);

        List<OperatingHead> GetISSCMemberForBulkMailList(long? SYKI, string[] ids);

        PeriodForITGRCMemberNominationListNewVM UpdateITGRCNomination(PeriodForITGRCMemberNominationListNewVM periodSettingVM);
        BulkUpdatePeriod BulkUpdateITGRCNomination(BulkUpdatePeriod bulkUpdatePeriod);

        List<AssetType> GetAssetTypeList();
        List<AssetClassification> GetAssetClassificationList();

        CommonAssetsRegListVM InsertUpdateCommonAssetRegisterSettingDetail(CommonAssetsRegListVM periodSettingVM);

        CommonAssetsRegListVM UpdateCommonAssetRegister(CommonAssetsRegListVM periodSettingVM);

        List<CommonAAssetsRegListVM> Get_CommonAsset_Register_List();

        // List<CommonAAssetsRegListVM> Get_CommonAsset_Register_WithID(int? id);

        Get_Division_ISSC_Member Get_Division_For_ISSC_Member(int? id);
        List<Primary_AssetDeatils_VM> Get_Primary_Asset(long Userid, long? SYKI);
        List<CommonAAssetsRegListVM> Get_Asset_Details_With_Common_Asset(long Userid, long? SYKI);
        int Insert_AssetDetails(CommonAAssetsRegListVM assetdetails);
        CommonAAssetsRegListVM Get_Edit_Asset_Details(int? Id);
        int Delete_AssetDetails(CommonAAssetsRegListVM assetdetails);
        int FinalSubmit_AssetDetails(CommonAAssetsRegListVM assetdetails);
        int ISSC_Member_AssetDetails_FinalSubmit_Check(int Empcode, long? SYKI);
        int ISSC_Member_AssetDetails_EndDate_Check(int Empcode, long? SYKI);
        List<CommonAAssetsRegListVM> Get_Last_Ki_Asset_DetailsNew(long? Userid, long? SYKI,long? DivId);
        List<CommonAAssetsRegListVM> Get_Last_Ki_Asset_DetailsOperationID(long? Userid, long? SYKI, long? OPID);
        List<CommonAAssetsRegListVM> Get_Last_Ki_Asset_Details(long? Userid, long? SYKI);
        DivisionHead_Details Get_DivisionHead_(long? SYKI, long Empcode);
        DivisionHead_Details Get_OperatingHead_(long? SYKI, long Empcode, long Userid);
        DivisionHead_Details Get_OperatingHead_Old(long? SYKI, long Empcode);
        Get_Division_ISSC_Member Get_ISSC_Member_Details_For_DivisionHead(int? id);

        List<Asset_Dvision_Deatils_VM> Get_ISSC_Members_Division_Details_For_DivisionHead(int? id);
        List<Get_Division_ISSC_Member> Get_ISSC_Members_Detail_For_DivisionHead(int? id);
        //Added by Aumento for SR91196
        List<Get_Division_ISSC_Member> Get_ISSC_Members_Detail_For_ITGRCHead(int? id);
        List<Asset_Dvision_Deatils_VM> Get_ISSC_Members_Division_Details_For_ITGRCHead(int? id);
        int ITGRC_Head_AssetDetails_FinalSubmit_Check(int Userid, long? SYKI, long ISSC_Code);
        DivisionHead_Details Get_ITGRCHead_(long? SYKI, long Empcode);
        int Asset_Approve_By_ITGRCHead(CommonAAssetsRegListVM asset_details);
        int Asset_SendBack_By_ITGRCHead(CommonAAssetsRegListVM asset_details);
        //Added by Aumento for SR91196
        int Last_year_Asset_Detail_Only_Non_CommonAsset(long Userid, long? SYKI);
        List<CommonAAssetsRegListVM> Get_ISSC_Member_Asset_For_DivisionHead(long? Userid, long? SYKI);
        int Asset_Approve_By_DivisionHead(CommonAAssetsRegListVM asset_details);
        int Asset_SendBack_By_DivisionHead(CommonAAssetsRegListVM asset_details);
        List<ISSCMember_details> Send_Email_To_ISSC_OperatingHead(CommonAAssetsRegListVM asset_details);
        List<ISSCMember_details> Send_Email_To_ISSC_OperatingHead_FroSendBack(CommonAAssetsRegListVM asset_details);
        int Division_Head_AssetDetails_FinalSubmit_Check(int Userid, long? SYKI,long ISSC_Code);
        List<Get_Division_ISSC_Member> Get_ISSC_Member_Details_For_OpearingHead(int? id);
        List<Get_Division_ISSC_Member> Get_ISSC_Members_Asset_Details_For_OpearingHead(int? id);
        int Asset_Approve_By_OperatingHead(CommonAAssetsRegListVM asset_details);
        int Asset_SendBack_By_OperatingHead(CommonAAssetsRegListVM asset_details);
        ISSCMember_details Send_Email_To_ISSC_(CommonAAssetsRegListVM asset_details);
        int Operating_Head_AssetDetails_FinalSubmit_Check(int Empcode, long? SYKI);
        List<PrimaryAsset> BindPrimaryAsset(long? div_Id);
        List<PrimaryAsset> BindSecondaryyAsset(string P_Id, long? div_Id);
        int InsertAssetDifiency(DifiencyReport difiency);        
         List<ISSCMember_details> Send_Email_To_ISSC_Member_DivisionHead(DifiencyReport difiency);
        EmpDetails GetEmp_Details_Ki_Wise(long Id);
        List<CommonAAssetsRegListVM> Get_Asset_Details_Operation_Division_Wise(long SYKI, decimal? operationid, decimal? divisionid);
        List<DivisionWise_Asset_Count> Get_Asset_Count_DivisionWise(long SYKI, decimal? operationid, decimal? divisionid);
        List<Asset_Approver_Remarks> GetRemrks(long SYKI, int Empcode);
        List<Asset_Approver_Remarks_ISSCM> GetRemrksForISSCMember(long SYKI, int Empcode);
        List<Asset_Approver_Remarks> GetRemrksForOPHead(long SYKI, int Empcode);
        List<Asset_Deficency_Remarks> GetDeficencyRemrks(long SYKI, int Empcode);
        int Check_OrganizationMapping(long SYKI, int Empcode);
        int Check_Issc_Member_Nomination(long SYKI, int Empcode);

        long GetOldISSC_MemberByCurrentISSCMemCode(long SYKI, int Empcode);
        AssetRegistrationSYKIViewModel GetAssetRegistrationSYKIListFor_OperatingHead_Dashboard(long SYKI, int Empcode);
        AssetRegistrationSYKIViewModelOH GetAssetRegistrationSYKIListFor_OperatingHeadDeails_Dashboard(long SYKI, int Empcode);
        List<OrgMappingVM> GetOrgMappingData(A_SearchParameterList _ParamList); //Dinesh
        AssetRegistrationSYKIViewModel GetAssetAllSYKIList();
        int OrgMappingSaveData(OrgMappingVM orgMappingVM);
        OrgMappingVM GetAddEditOrgMappingData(int? OrgMappingId);
        AssetRegister_Start_EndDate_ISSC_Member GetStartDate_EndDate_ForAsset_Register(CommonAAssetsRegListVM asset_details);

        MailerReminderVM GetStart_Dt_End_Dt_ISSC_Nomination(long SYKI);
        List<MailerReminderVM> GetReminderDetailsForISSCMember_Nomination();
        string InsertISSC_Reminder(MailerReminderVM mailer);

        string GetReminderDate(long id);

        List<MailerReminderVM> GetReminderDetailsForISSCMember_Approval();

        MailerReminderVM GetStart_Dt_End_Dt_ISSC_Member(long SYKI);
        List<CommonAAssetsRegListVM> GetCommon_Asset_SubmittedDetails(long SYKI, long Userid);

        List<ISSC_Member_Nomination_deatilsVM> Get_ISSC_member_Nomintion_pending(long SYKI, long Userid);
        List<Get_Division_ISSC_Member> Asset_pendingDetails_ISSCMembers(int? id);
        List<Get_Division_ISSC_Member> Get_Asset_Register_Details_For_OpearingHead(int? id);

        List<AssetRegistrationSearchModel> GetAssetRegisterSunmitStatus(long? SYKIID, long? OPERATIONID, long? DIVISIONID);

        int BulkUpdateAssetApplicability(AssetApplicabilityUpdateVM bulkUpdateAssetApplicability);
        List<CommonAAssetsRegListVM> Get_Common_Asset_Details(List<decimal> Ids);

        //Added By Aumento Start
        Get_AssetRegister_DigitalSign_Detail Get_AssetRegister_DigitalSign_Detail(long? UserID, long? SYKIID);

        Get_AssetRegister_Header_Detail Get_AssetRegister_Header_Detail(long? UserID, long? SYKIID);

        //Added by aumento as on 17062024 for SR71836------------------------------------------------------------
        //Added by aumento as on 17062024 for SR71836------------------------------------------------------------
        //  int UpdateAssetRegister(long SYKI, int Empcode); // Added By Aumento :: SR102194 
        int UpdateAssetRegister(long SYKI, int Empcode, bool AutoApproveToDiv, long? itgrccode); // Added By Aumento :: SR102194
        int UpdateAssetRegisterAutoApprove(long SYKI, int Empcode, bool AutoApproveToDiv, long? itgrccode); // Added By Aumento :: SR102194

        //-------------------------------------------------------------------------------------------------------
        bool IsOperationMatch(long? userId); // Added By Aumento :: SR102194

        //int UpdateAmendment(long? SYKI);

        //Added by Aumento End  
        //Added by Aumento for SR91196
        String _GetITGRC();
        //Added by Aumento for SR91196  
    }

}
