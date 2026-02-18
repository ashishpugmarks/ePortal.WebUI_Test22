using ePortal.DomainClasses;
using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IRiskAssessmentService
    {
        #region
        // Yogesh
        List<DivisionWise_Risk_Count> Get_Risk_Count_DivisionWise(long SYKI, decimal? operationid, decimal? divisionid);
        List<DivisionWise_Risk_Count> Get_Risk_Report_Count_DivisionWise(long Id);
        List<CommonRiskAssessmentVM> Get_Risk_Details_Operation_Division_Wise(long SYKI, decimal? operationid, decimal? divisionid);

        List<CommonRiskAssessmentVM> Get_Risk_Details_Report_Operation_Division_Wise(long Id);
        List<CommonRiskAssessmentVM> Get_Risk_Details_Report_OperationHead_Division_Wise(long Id);
        List<DivisionWise_Risk_Count> Get_Risk_Report_Count_OperationHead_DivisionWise(long Id);

        List<ADORGLEVEL> BindOperation(long? SYKIID);
        List<RiskStatementLstVM> GetAllPotentialRisk();
        RiskAssessmentSYKIViewModel GetRiskRegistrationSYKIListForViewTeam();
        EmpDetails GetEmp_Details_Ki_Wise(long Id);
        RiskSelfPending GetSelfRiskPending(long Id);
        RiskSelfPending GetSelfRiskRequest(long Id);
        RiskCategoryCreVM GetRiskCategoryById(long Id);
        string DeleteRiskCategory(long Id);
        List<long?> GetOperationIdsByLoginUserId(long Id);
        List<long?> GetDivisionsIdsByLoginUserId(long Id);

        RiskStatementCreVM EditPotentialRisk(long Id);
        string DeletePotentialRisk(long Id);
        List<RiskApproval> GetRiskPendingForApprovalForDivHead(long Id);
        List<RiskApprovalOP> RiskPendingForApprovalForOPHead(long Id);

        List<RiskApproval> Get_Risk_Details_For_OpHeadORDivHead(int? id);
        List<CommonRiskAssessmentVM> Get_Risk_Assessment(long Userid, long? SYKI);
        List<DownloadReportVM> Get_Risk_AssessmentFor_Download(long Userid, long? SYKI);
        RiskAssessmentSYKIViewModel GetRiskAssessmentSYKIList();
        //mail reminder utitlity by sujit
        List<RiskMailerReminderVM> GetReminderDetailsForISSCMember_Nomination();
        RiskMailerReminderVM GetStart_Dt_End_Dt_ISSC_Nomination(long SYKI);

        string RiskGetReminderDate(long id);

        string InsertISSC_Reminder(RiskMailerReminderVM mailer);



        //ends here
        List<RiskAssessmentSearchModel> GetRiskAssessmentSunmitStatus(long? SYKIID, long? OPERATIONID, long? DIVISIONID);
        List<ADORGLEVEL> GetOrgLevelList(long typeId);
        List<RiskAssessmentPeriodForITGRCMMemberNominationListVM> GetRiskAssessmentPeriodSettingITGRCList();
        RiskAssessmentForITGRCMemberNominationVM InsertUpdateRiskAssessmentPeriodITGRCSettingDetail(RiskAssessmentPeriodForITGRCMMemberNominationListVM periodSettingVM);
        List<OperatingHead> GetISSCMemberList(long? SYKI, long? OPERATIONID, long? Divisionid);
        List<BulkUpdateEmailVM> GetISSCMemberListForBulkUpdate(string ids, long currentki);
        List<RiskAssessmentPeriodForITGRCMMemberNominationListVM> GetRisk_Assessment_period_ITGRC_List_WithID(int? id);
        BulkUpdatePeriod BulkUpdateITGRCNomination(BulkUpdatePeriod bulkUpdatePeriod);
        long GetOldISSC_MemberByCurrentISSCMemCode(long SYKI, int Empcode);
        int Check_Issc_Member_Nomination(long SYKI, int Empcode);
        int ISSC_Member_AssetDetails_FinalSubmit_Check(int Empcode, long? SYKI);
        int ISSC_Member_RiskDetails_EndDate_Check(int Empcode, long? SYKI);
        List<CommonRiskAssessmentVM> Get_Last_Ki_Asset_Details(long? Userid, long? SYKI);

        List<Risk_Approver_Remarks_ISSCM> GetRemrksForISSCMember(long SYKI, int Empcode);
        List<Risk_Deficency_Remarks> GetDeficencyRemrks(long SYKI, int Empcode);
        int Check_OrganizationMapping(long SYKI, int Empcode);
        RiskAssessmentVM Get_Edit_Risk_Details(int? Id);
        string Delete_RiskDetails(int riskId, int userId);
        string SaveAsDraftkRegister(int skyid, int userId);
        int Last_year_Asset_Detail_Only_Non_CommonAsset(long Userid, long? SYKI);
        int CheckLast_year_RiskData(long Userid, long? SYKI);
        Get_Division_ISSC_Member Get_Division_For_ISSC_Member(int? id);
        AssetRegistrationSYKIViewModel GetAssetRegistrationSYKIListForDashboard();
        int Operating_Head_AssetDetails_FinalSubmit_Check(int Empcode, long? SYKI);
        List<CommonRiskAssessmentVM> Get_ISSC_Member_Risk_For_DivisionHead(long? Userid, long? SYKI);
        List<Asset_Approver_Remarks> GetRemrks(long SYKI, int Empcode);
        int BulkUpdateAssetApplicability(AssetApplicabilityUpdateVM bulkUpdateAssetApplicability);
        DivisionHead_Details Get_DivisionHead_(long? SYKI, long Empcode);
        string FinalSubmit_RiskDetails(CommonRiskAssessmentVM assetdetails);
        string RiskOldOperatingOROldDivHeadRequestAssignToNewOpHeadOrNewDivHead(SameDivisionAndOprationApprovalVM model);
        List<SearchParameterList> BindDivision(long? op_Id);
        long? BindDivisionWithOldOperationHeadEmpCodeByOperationId(long? op_Id, long? SKYIID);

        int? GetAssetUserDeailsForOPHeadByLoginUserID(long userid, long Syki);
        int? GetAssetUserDeailsForDivisionHeadByLoginUserID(long userid, long Syki);
        long? BindOldDivisionHeadEmpCodeByDivisionId(long? divId, long? SKYIID);
        List<Asset_Dvision_Deatils_VM> Get_ISSC_Members_Division_Details_For_DivisionHead(int? id);
        List<Get_Division_ISSC_Member> Get_ISSC_Members_Detail_For_DivisionHead(int? id);
        int Division_Head_AssetDetails_FinalSubmit_Check(int Userid, long? SYKI, long ISSC_Code);
        int Risk_Approve_By_DivisionHead(CommonRiskAssessmentVM asset_details);
        int Risk_SendBack_By_DivisionHead(CommonRiskAssessmentVM asset_details);
        List<ISSCMember_details> Send_Email_To_ISSC_OperatingHead(CommonRiskAssessmentVM asset_details);
        List<ISSCMember_details> Send_Email_To_ISSC_OperatingHead_FroSendBack(CommonRiskAssessmentVM asset_details);
        AssetRegister_Start_EndDate_ISSC_Member GetStartDate_EndDate_ForAsset_Register(CommonRiskAssessmentVM asset_details);

        List<CommonRiskAssessmentVM> Get_Last_Ki_Asset_DetailsNew(long? Userid, long? SYKI, long? DivId);

        List<RiskCategoryVM> GetAllRiskCategory();
        List<RiskCategoryCreVM> GetAllRiskCategories();
        int InsertORUpdateRiskCategory(RiskCategoryCreVM riskCategoryCreVM);
        int InsertORUpdatePotentialRisk(RiskStatementCreVM riskStatementCreVM);
        List<PrimaryAssestsVM> GetPrimaryAssetByUserId(int userid, int currenr_ki);
        List<PotentialRiskVM> GetPotentialRiskByRiskCategoryId(int categoryId);
        List<RiskLeadVM> GetRiskLeadByPotential();
        List<Risk_Current_Meaasure_LevelVM> GetAllCurrentMeasureLevel();
        List<RiskFrequencyVM> GetAllFrequency();
        List<RiskImpactVM> GetAllImpact();
        List<RiskImpactVM> GetImpactByImpactTypeId(int impactTypeId);
        List<RiskImpactVM> GetImpactByImpactId(int impId);
        List<RiskFrequencyVM> GetFrequencyByFrequencyId(int FreqId);
        List<ImpactTypeVM> GetAllImpactType();
        List<Degree_LevelVM> GetAllDegreeLevelByFrequencyIdAndImpactId(int frequencyId, int impactId);
        List<RiskTreatmentLevelVM> GetRiskTreatmentBy(int levelId, int freqId, int impactId);

        string InsertORUpdateRiskRegister(RiskAssessmentVM riskAssessmentVM);
        string CheckAllPrimaryRiskCreatedOrNotByUserId(List<int> pIds, int skyid, int userId);

        #endregion

        #region
        // Sujit
        List<CommonRiskAssessmentVM> OPeratingHeads_DeatiledReport();
        int? GetRiskAssessmentUserDeailsForOPHeadByLoginUserID(long userid, long Syki);
        int InsertRiskDifiency(Risk_DifiencyReport difiency);
        List<PrimaryAssestsVM> BindPrimaryAsset(long? div_Id);
        int Risk_SendBack_By_OperatingHead(CommonRiskAssessmentVM asset_details);
        RiskAssessmentSYKIViewModelOH GetRiskAssessmentSYKIListFor_OperatingHeadDeails_Dashboard(long SYKI, int Empcode);
        int Risk_Approve_By_OperatingHead(CommonRiskAssessmentVM risk_details);
        Risk_DivisionHead_Details Get_OperatingHead_(long? SYKI, long Empcode, long Userid);
        List<Risk_ISSCMember_details> Send_Email_To_ISSC_Member_DivisionHead(Risk_DifiencyReport difiency);
        int Operating_Head_RiskDetails_FinalSubmit_Check(int Empcode, long? SYKI);
        int? GetRiskUserDeailsForOPHeadByLoginUserID(long userid, long Syki);
        int Division_Head_RiskDetails_FinalSubmit_Check(int Userid, long? SYKI, long ISSC_Code);
        RiskAssessment_Start_EndDate_ISSC_Member GetStartDate_EndDate_ForRisk_Register(CommonRiskAssessmentVM risk_details);
        RiskAssessmentSYKIViewModel GetRiskAssessmentSYKIListFor_OperatingHead_Dashboard(long SYKI, int Empcode);
        List<Get_RiskDivision_ISSC_Member> Get_ISSC_Member_Details_For_OpearingHead(int? id);
        List<Get_RiskDivision_ISSC_Member> Get_ISSC_Members_Risk_Details_For_OpearingHead(int? id);
        #endregion
    }
}
