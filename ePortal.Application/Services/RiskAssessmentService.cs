using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.Infrastructure.Repositories;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ePortal.Application.Services
{
    public class RiskAssessmentService : IRiskAssessmentService
    {
        private readonly RiskAssessmentRepository _objRiskAssessmentRepository;       


        public RiskAssessmentService(RiskAssessmentRepository objRiskAssessmentRepository)
        {
            _objRiskAssessmentRepository = objRiskAssessmentRepository;           
        }

        public List<CommonRiskAssessmentVM> Get_Risk_Assessment(long Userid, long? SYKI)
        {
            var objresult = _objRiskAssessmentRepository.Get_Risk_Assessment(Userid, SYKI);
            return objresult;
        }
        public List<DivisionWise_Risk_Count> Get_Risk_Count_DivisionWise(long SYKI, decimal? operationid, decimal? divisionid)
        {
            var objresult = _objRiskAssessmentRepository.Get_Risk_Count_DivisionWise(SYKI, operationid, divisionid);
            return objresult;
        }
        public List<DivisionWise_Risk_Count> Get_Risk_Report_Count_DivisionWise(long Id)
        {
            var objresult = _objRiskAssessmentRepository.Get_Risk_Report_Count_DivisionWise(Id);
            return objresult;
        }
        public List<ADORGLEVEL> BindOperation(long? SYKIID)
        {
            return _objRiskAssessmentRepository.BindOperation(SYKIID);
        }
        public List<CommonRiskAssessmentVM> Get_Risk_Details_Operation_Division_Wise(long SYKI, decimal? operationid, decimal? divisionid)
        {
            var objresult = _objRiskAssessmentRepository.Get_Risk_Details_Operation_Division_Wise(SYKI, operationid, divisionid);
            return objresult;
        }

        public List<CommonRiskAssessmentVM> Get_Risk_Details_Report_Operation_Division_Wise(long Id)
        {
            var objresult = _objRiskAssessmentRepository.Get_Risk_Details_Report_Operation_Division_Wise(Id);
            return objresult;
        }
        public List<CommonRiskAssessmentVM> Get_Risk_Details_Report_OperationHead_Division_Wise(long Id)
        {
            var objresult = _objRiskAssessmentRepository.Get_Risk_Details_Report_OperationHead_Division_Wise(Id);
            return objresult;
        }

        public List<DivisionWise_Risk_Count> Get_Risk_Report_Count_OperationHead_DivisionWise(long Id)
        {
            var objresult = _objRiskAssessmentRepository.Get_Risk_Report_Count_OperationHead_DivisionWise(Id);
            return objresult;
        }
        public RiskAssessmentSYKIViewModel GetRiskRegistrationSYKIListForViewTeam()
        {
            var SYKIList = _objRiskAssessmentRepository.GetRiskRegistrationSYKIListForViewTeam();
            return SYKIList;
        }

        public EmpDetails GetEmp_Details_Ki_Wise(long Id)
        {
            var objresult = _objRiskAssessmentRepository.GetEmp_Details_Ki_Wise(Id);
            return objresult;
        }
        public RiskSelfPending GetSelfRiskPending(long Id)
        {
            var objresult = _objRiskAssessmentRepository.GetSelfRiskPending(Id);
            return objresult;
        }
        public RiskSelfPending GetSelfRiskRequest(long Id)
        {
            var objresult = _objRiskAssessmentRepository.GetSelfRiskRequest(Id);
            return objresult;
        }
        public List<RiskApproval> GetRiskPendingForApprovalForDivHead(long Id)
        {
            var objresult = _objRiskAssessmentRepository.GetRiskPendingForApprovalForDivHead(Id);
            return objresult;
        }
        public List<RiskApprovalOP> RiskPendingForApprovalForOPHead(long Id)
        {
            var objresult = _objRiskAssessmentRepository.RiskPendingForApprovalForOPHead(Id);
            return objresult;
        }

        public List<RiskApproval> Get_Risk_Details_For_OpHeadORDivHead(int? Id)
        {
            var objresult = _objRiskAssessmentRepository.Get_Risk_Details_For_OpHeadORDivHead(Id);
            return objresult;
        }
        public List<DownloadReportVM> Get_Risk_AssessmentFor_Download(long Userid, long? SYKI)
        {
            var objresult = _objRiskAssessmentRepository.Get_Risk_AssessmentFor_Download(Userid, SYKI);
            return objresult;
        }

        public List<ADORGLEVEL> GetOrgLevelList(long typeId)
        {
            return _objRiskAssessmentRepository.GetOrgLevelList(typeId);
        }

        public List<RiskAssessmentSearchModel> GetRiskAssessmentSunmitStatus(long? SYKIID, long? OPERATIONID, long? DIVISIONID)
        {
            var objresult = _objRiskAssessmentRepository.GetRiskAssessmentSunmitStatus(SYKIID, OPERATIONID, DIVISIONID);
            return objresult;
        }
        public RiskAssessmentSYKIViewModel GetRiskAssessmentSYKIList()
        {
            var SYKIList = _objRiskAssessmentRepository.GetRiskAssessmentSYKIList();
            return SYKIList;
        }

        public List<RiskAssessmentPeriodForITGRCMMemberNominationListVM> GetRiskAssessmentPeriodSettingITGRCList()
        {
            var objresult = _objRiskAssessmentRepository.GetRiskAssessmentPeriodSettingITGRCList();
            return objresult;
        }

        public RiskAssessmentForITGRCMemberNominationVM InsertUpdateRiskAssessmentPeriodITGRCSettingDetail(RiskAssessmentPeriodForITGRCMMemberNominationListVM periodSettingVM)
        {
            var PeriodSettingdata = _objRiskAssessmentRepository.InsertUpdateRiskAssessmentPeriodITGRCSettingDetail(periodSettingVM);
            return PeriodSettingdata;
        }

        public List<OperatingHead> GetISSCMemberList(long? SYKI, long? OPERATIONID, long? Divisionid)
        {
            var objresult = _objRiskAssessmentRepository.GetISSCMemberList(SYKI, OPERATIONID, Divisionid);
            return objresult;
        }

        public List<BulkUpdateEmailVM> GetISSCMemberListForBulkUpdate(string ids, long currentki)
        {
            var objresult = _objRiskAssessmentRepository.GetISSCMemberListForBulkUpdate(ids, currentki);
            return objresult;
        }

        

        public List<RiskAssessmentPeriodForITGRCMMemberNominationListVM> GetRisk_Assessment_period_ITGRC_List_WithID(int? id)
        {
            var objresult = _objRiskAssessmentRepository.GetRisk_Assessment_period_ITGRC_List_WithID(id);
            return objresult;
        }

        public BulkUpdatePeriod BulkUpdateITGRCNomination(BulkUpdatePeriod bulkUpdatePeriod)
        {
            var objresult = _objRiskAssessmentRepository.BulkUpdateITGRCNomination(bulkUpdatePeriod);
            return objresult;
        }

        public long GetOldISSC_MemberByCurrentISSCMemCode(long SYKI, int Empcode)
        {
            var objresult = _objRiskAssessmentRepository.GetOldISSC_MemberByCurrentISSCMemCode(SYKI, Empcode);
            return objresult;
        }

        public List<long?> GetOperationIdsByLoginUserId(long Id)
        {
            var objresult = _objRiskAssessmentRepository.GetOperationIdsByLoginUserId(Id);
            return objresult;
        }

        public List<long?> GetDivisionsIdsByLoginUserId(long Id)
        {
            var objresult = _objRiskAssessmentRepository.GetDivisionsIdsByLoginUserId(Id);
            return objresult;
        }

        public int Check_Issc_Member_Nomination(long SYKI, int Empcode)
        {
            var objresult = _objRiskAssessmentRepository.Check_Issc_Member_Nomination(SYKI, Empcode);
            return objresult;
        }

        public List<Primary_RiskDeatils_VM> Get_Primary_Asset(long Userid, long? SYKI)
        {
            var objresult = _objRiskAssessmentRepository.Get_Primary_Asset(Userid, SYKI);
            return objresult;
        }

       

        public int ISSC_Member_AssetDetails_FinalSubmit_Check(int Empcode, long? SYKI)
        {
            var objresult = _objRiskAssessmentRepository.ISSC_Member_AssetDetails_FinalSubmit_Check(Empcode, SYKI);
            return objresult;
        }
        public int ISSC_Member_RiskDetails_EndDate_Check(int Empcode, long? SYKI)
        {
            var objresult = _objRiskAssessmentRepository.ISSC_Member_RiskDetails_EndDate_Check(Empcode, SYKI);
            return objresult;
        }

        public List<CommonRiskAssessmentVM> Get_Last_Ki_Asset_Details(long? Userid, long? SYKI)
        {
            var objresult = _objRiskAssessmentRepository.Get_Last_Ki_Asset_Details(Userid, SYKI);
            return objresult;
        }

        public List<Risk_Approver_Remarks_ISSCM> GetRemrksForISSCMember(long SYKI, int Empcode)
        {
            var objresult = _objRiskAssessmentRepository.GetRemrksForISSCMember(SYKI, Empcode);
            return objresult;
        }


        public List<Risk_Deficency_Remarks> GetDeficencyRemrks(long SYKI, int Empcode)
        {
            var objresult = _objRiskAssessmentRepository.GetDeficencyRemrks(SYKI, Empcode);
            return objresult;
        }

        public int Check_OrganizationMapping(long SYKI, int Empcode)
        {
            var objresult = _objRiskAssessmentRepository.Check_OrganizationMapping(SYKI, Empcode);
            return objresult;
        }
        public RiskAssessmentVM Get_Edit_Risk_Details(int? Id)
        {
            var objresult = _objRiskAssessmentRepository.Get_Edit_Risk_Details(Id);
            return objresult;
        }

        public string Delete_RiskDetails(int riskId, int userId)
        {
            var objresult = _objRiskAssessmentRepository.Delete_RiskDetails(riskId, userId);
            return objresult;
        }

        public string SaveAsDraftkRegister(int skyid, int userId)
        {
            var objresult = _objRiskAssessmentRepository.SaveAsDraftkRegister(skyid,userId);
            return objresult;
        }

        public string CheckAllPrimaryRiskCreatedOrNotByUserId(List<int> pIds,int skyid, int userId)
        {
            var objresult = _objRiskAssessmentRepository.CheckAllPrimaryRiskCreatedOrNotByUserId(pIds,skyid, userId);
            return objresult;
        }

        public int Last_year_Asset_Detail_Only_Non_CommonAsset(long Userid, long? SYKI)
        {
            var objresult = _objRiskAssessmentRepository.Last_year_Asset_Detail_Only_Non_CommonAsset(Userid, SYKI);
            return objresult;
        }

        public int CheckLast_year_RiskData(long Userid, long? SYKI)
        {
            var objresult = _objRiskAssessmentRepository.CheckLast_year_RiskData(Userid, SYKI);
            return objresult;
        }

        public Get_Division_ISSC_Member Get_Division_For_ISSC_Member(int? id)
        {
            var objresult = _objRiskAssessmentRepository.Get_Division_For_ISSC_Member(id);
            return objresult;
        }

        public AssetRegistrationSYKIViewModel GetAssetRegistrationSYKIListForDashboard()
        {
            var SYKIList = _objRiskAssessmentRepository.GetAssetRegistrationSYKIListForDashboard();
            return SYKIList;
        }

        public int Operating_Head_AssetDetails_FinalSubmit_Check(int Empcode, long? SYKI)
        {
            var objresult = _objRiskAssessmentRepository.Operating_Head_AssetDetails_FinalSubmit_Check(Empcode, SYKI);
            return objresult;
        }

        public List<CommonRiskAssessmentVM> Get_ISSC_Member_Risk_For_DivisionHead(long? Userid, long? SYKI)
        {
            var objresult = _objRiskAssessmentRepository.Get_ISSC_Member_Risk_For_DivisionHead(Userid, SYKI);
            return objresult;
        }

        public List<Asset_Approver_Remarks> GetRemrks(long SYKI, int Empcode)
        {
            var objresult = _objRiskAssessmentRepository.GetRemrks(SYKI, Empcode);
            return objresult;
        }

        public int BulkUpdateAssetApplicability(AssetApplicabilityUpdateVM bulkUpdateAssetApplicability)
        {
            var objresult = _objRiskAssessmentRepository.BulkUpdateAssetApplicability(bulkUpdateAssetApplicability);
            return objresult;
        }

        public DivisionHead_Details Get_DivisionHead_(long? SYKI, long Empcode)
        {
            var objresult = _objRiskAssessmentRepository.Get_DivisionHead_(SYKI, Empcode);
            return objresult;
        }

       

        public string FinalSubmit_RiskDetails(CommonRiskAssessmentVM assetdetails)
        {
            var objresult = _objRiskAssessmentRepository.FinalSubmit_RiskDetails(assetdetails);
            return objresult;
        }

        public string RiskOldOperatingOROldDivHeadRequestAssignToNewOpHeadOrNewDivHead(SameDivisionAndOprationApprovalVM model)
        {
            var op = _objRiskAssessmentRepository.RiskOldOperatingOROldDivHeadRequestAssignToNewOpHeadOrNewDivHead(model);
            return op;
        }

        public List<SearchParameterList> BindDivision(long? op_Id)
        {
            var op = _objRiskAssessmentRepository.BindDivision(op_Id);
            return op;
        }

        public long? BindOldDivisionHeadEmpCodeByDivisionId(long? divId, long? SKYIID)
        {
            var op = _objRiskAssessmentRepository.BindOldDivisionHeadEmpCodeByDivisionId(divId, SKYIID);
            return op;
        }

        public long? BindDivisionWithOldOperationHeadEmpCodeByOperationId(long? divId, long? SKYIID)
        {
            var op = _objRiskAssessmentRepository.BindDivisionWithOldOperationHeadEmpCodeByOperationId(divId, SKYIID);
            return op;
        }

        public int? GetAssetUserDeailsForDivisionHeadByLoginUserID(long userid, long Syki)
        {
            var OldUserId = _objRiskAssessmentRepository.GetAssetUserDeailsForDivisionHeadByLoginUserID(userid, Syki);
            return OldUserId;
        }

        public int? GetAssetUserDeailsForOPHeadByLoginUserID(long userid, long Syki)
        {
            var OldUserId = _objRiskAssessmentRepository.GetAssetUserDeailsForOPHeadByLoginUserID(userid, Syki);
            return OldUserId;
        }

        public List<Get_Division_ISSC_Member> Get_ISSC_Members_Detail_For_DivisionHead(int? id)
        {
            var objresult = _objRiskAssessmentRepository.Get_ISSC_Members_Detail_For_DivisionHead(id);
            return objresult;
        }

        public List<Asset_Dvision_Deatils_VM> Get_ISSC_Members_Division_Details_For_DivisionHead(int? id)
        {
            var objresult = _objRiskAssessmentRepository.Get_ISSC_Members_Division_Details_For_DivisionHead(id);
            return objresult;
        }

        public int Division_Head_AssetDetails_FinalSubmit_Check(int Empcode, long? SYKI, long ISSC_Code)
        {
            var objresult = _objRiskAssessmentRepository.Division_Head_AssetDetails_FinalSubmit_Check(Empcode, SYKI, ISSC_Code);
            return objresult;
        }

       

        public int Risk_Approve_By_DivisionHead(CommonRiskAssessmentVM asset_details)
        {
            var objresult = _objRiskAssessmentRepository.Risk_Approve_By_DivisionHead(asset_details);
            return objresult;
        }

       
        public int Risk_SendBack_By_DivisionHead(CommonRiskAssessmentVM asset_details)
        {
            var objresult = _objRiskAssessmentRepository.Risk_SendBack_By_DivisionHead(asset_details);
            return objresult;
        }

        public List<ISSCMember_details> Send_Email_To_ISSC_OperatingHead(CommonRiskAssessmentVM asset_details)
        {
            var objresult = _objRiskAssessmentRepository.Send_Email_To_ISSC_OperatingHead(asset_details);
            return objresult;
        }
        public List<ISSCMember_details> Send_Email_To_ISSC_OperatingHead_FroSendBack(CommonRiskAssessmentVM asset_details)
        {
            var objresult = _objRiskAssessmentRepository.Send_Email_To_ISSC_OperatingHead_FroSendBack(asset_details);
            return objresult;
        }

        public List<CommonRiskAssessmentVM> Get_Last_Ki_Asset_DetailsNew(long? Userid, long? SYKI, long? DivId)
        {
            var objresult = _objRiskAssessmentRepository.Get_Last_Ki_Asset_DetailsNew(Userid, SYKI, DivId);
            return objresult;
        }


        public AssetRegister_Start_EndDate_ISSC_Member GetStartDate_EndDate_ForAsset_Register(CommonRiskAssessmentVM asset_details)
        {
            var objresult = _objRiskAssessmentRepository.GetStartDate_EndDate_ForAsset_Register(asset_details);
            return objresult;
        }

        public List<RiskCategoryVM> GetAllRiskCategory()
        {
            var objresult = _objRiskAssessmentRepository.GetAllRiskCategory();
            return objresult;
        }

        public List<RiskCategoryCreVM> GetAllRiskCategories()
        {
            var objresult = _objRiskAssessmentRepository.GetAllRiskCategories();
            return objresult;
        }

        public List<RiskStatementLstVM> GetAllPotentialRisk()
        {
            var objresult = _objRiskAssessmentRepository.GetAllPotentialRisk();
            return objresult;
        }

        public RiskCategoryCreVM GetRiskCategoryById(long Id)
        {
            var objresult = _objRiskAssessmentRepository.GetRiskCategoryById(Id);
            return objresult;
        }

        public string DeleteRiskCategory(long Id)
        {
            var objresult = _objRiskAssessmentRepository.DeleteRiskCategory(Id);
            return objresult;
        }

        public RiskStatementCreVM EditPotentialRisk(long Id)
        {
            var objresult = _objRiskAssessmentRepository.EditPotentialRisk(Id);
            return objresult;
        }

        public string DeletePotentialRisk(long Id)
        {
            var objresult = _objRiskAssessmentRepository.DeletePotentialRisk(Id);
            return objresult;
        }

        public int InsertORUpdateRiskCategory(RiskCategoryCreVM riskCategoryCreVM)
        {
            var objresult = _objRiskAssessmentRepository.InsertORUpdateRiskCategory(riskCategoryCreVM);
            return objresult;
        }

        public int InsertORUpdatePotentialRisk(RiskStatementCreVM riskStatementCreVM)
        {
            var objresult = _objRiskAssessmentRepository.InsertORUpdatePotentialRisk(riskStatementCreVM);
            return objresult;
        }

        public List<PotentialRiskVM> GetPotentialRiskByRiskCategoryId(int categoryId)
        {
            var objresult = _objRiskAssessmentRepository.GetPotentialRiskByRiskCategoryId(categoryId);
            return objresult;
        }

        public List<RiskLeadVM> GetRiskLeadByPotential()
        {
            var objresult = _objRiskAssessmentRepository.GetRiskLeadByPotential();
            return objresult;
        }

        public List<Risk_Current_Meaasure_LevelVM> GetAllCurrentMeasureLevel()
        {
            var objresult = _objRiskAssessmentRepository.GetAllCurrentMeasureLevel();
            return objresult;
        }

        public List<RiskFrequencyVM> GetAllFrequency()
        {
            var objresult = _objRiskAssessmentRepository.GetAllFrequency();
            return objresult;
        }

        public List<ImpactTypeVM> GetAllImpactType()
        {
            var objresult = _objRiskAssessmentRepository.GetAllImpactType();
            return objresult;
        }

        public List<RiskImpactVM> GetAllImpact()
        {
            var objresult = _objRiskAssessmentRepository.GetAllImpact();
            return objresult;
        }

        public List<RiskImpactVM> GetImpactByImpactTypeId(int impactTypeId)
        {
            var objresult = _objRiskAssessmentRepository.GetImpactByImpactTypeId(impactTypeId);
            return objresult;
        }

        public List<RiskFrequencyVM> GetFrequencyByFrequencyId(int FreqId)
        {
            var objresult = _objRiskAssessmentRepository.GetFrequencyByFrequencyId(FreqId);
            return objresult;
        }

        public List<RiskImpactVM> GetImpactByImpactId(int impId)
        {
            var objresult = _objRiskAssessmentRepository.GetImpactByImpactId(impId);
            return objresult;
        }

        public List<Degree_LevelVM> GetAllDegreeLevelByFrequencyIdAndImpactId(int frequencyId, int impactId)
        {
            var objresult = _objRiskAssessmentRepository.GetAllDegreeLevelByFrequencyIdAndImpactId(frequencyId, impactId);
            return objresult;
        }

        public List<RiskTreatmentLevelVM> GetRiskTreatmentBy(int levelId,int freqId, int impactId)
        {
            var objresult = _objRiskAssessmentRepository.GetRiskTreatmentBy(levelId,freqId, impactId);
            return objresult;
        }

        public List<PrimaryAssestsVM> GetPrimaryAssetByUserId(int userid, int currenr_ki)
        {
            var objresult = _objRiskAssessmentRepository.GetPrimaryAssetByUserId(userid, currenr_ki);
            return objresult;
        }

        public string InsertORUpdateRiskRegister(RiskAssessmentVM riskAssessmentVM)
        {
            var objresult = _objRiskAssessmentRepository.InsertORUpdateRiskRegister(riskAssessmentVM);
            return objresult;
        }

        public RiskAssessmentSYKIViewModel GetRiskAssessmentSYKIListFor_OperatingHead_Dashboard(long SYKI, int Empcode)
        {
            var objresult = _objRiskAssessmentRepository.GetRiskAssessmentSYKIListFor_OperatingHead_Dashboard(SYKI, Empcode);
            return objresult;
        }
        public int Risk_SendBack_By_OperatingHead(CommonRiskAssessmentVM risk_details)
        {
            var objresult = _objRiskAssessmentRepository.Risk_SendBack_By_OperatingHead(risk_details);
            return objresult;
        }

        public RiskAssessmentSYKIViewModelOH GetRiskAssessmentSYKIListFor_OperatingHeadDeails_Dashboard(long SYKI, int Empcode)
        {
            var objresult = _objRiskAssessmentRepository.GetRiskAssessmentSYKIListFor_OperatingHeadDeails_Dashboard(SYKI, Empcode);
            return objresult;
        }

        public int? GetRiskAssessmentUserDeailsForOPHeadByLoginUserID(long userid, long Syki)
        {
            var objresult = _objRiskAssessmentRepository.GetRiskAssessmentUserDeailsForOPHeadByLoginUserID(userid, Syki);
            return objresult;
        }
        public List<CommonRiskAssessmentVM> OPeratingHeads_DeatiledReport()
        {
            var opremarks = _objRiskAssessmentRepository.OPeratingHeads_DeatiledReport();
            return opremarks;
        }


        public int Division_Head_RiskDetails_FinalSubmit_Check(int Empcode, long? SYKI, long ISSC_Code)
        {
            var objresult = _objRiskAssessmentRepository.Division_Head_RiskDetails_FinalSubmit_Check(Empcode, SYKI, ISSC_Code);
            return objresult;
        }
        List<CommonRiskAssessmentVM> IRiskAssessmentService.Get_ISSC_Member_Risk_For_DivisionHead(long? Userid, long? SYKI)
        {

            var objresult = _objRiskAssessmentRepository.Get_ISSC_Member_Risk_For_DivisionHead(Userid, SYKI);
            return objresult;

        }
       
        public int? GetRiskUserDeailsForOPHeadByLoginUserID(long userid, long Syki)
        {
            var OldUserId = _objRiskAssessmentRepository.GetRiskUserDeailsForOPHeadByLoginUserID(userid, Syki);
            return OldUserId;
        }
        public List<Get_RiskDivision_ISSC_Member> Get_ISSC_Member_Details_For_OpearingHead(int? id)
        {
            var objresult = _objRiskAssessmentRepository.Get_ISSC_Member_Details_For_OpearingHead(id);
            return objresult;
        }
        public List<Get_RiskDivision_ISSC_Member> Get_ISSC_Members_Risk_Details_For_OpearingHead(int? id)
        {
            var objresult = _objRiskAssessmentRepository.Get_ISSC_Members_Risk_Details_For_OpearingHead(id);
            return objresult;
        }

        public int Operating_Head_RiskDetails_FinalSubmit_Check(int Empcode, long? SYKI)
        {
            var objresult = _objRiskAssessmentRepository.Operating_Head_RiskDetails_FinalSubmit_Check(Empcode, SYKI);
            return objresult;
        }
        public RiskAssessment_Start_EndDate_ISSC_Member GetStartDate_EndDate_ForRisk_Register(CommonRiskAssessmentVM risk_details)
        {
            var objresult = _objRiskAssessmentRepository.GetStartDate_EndDate_ForRisk_Register(risk_details);
            return objresult;
        }

        public Risk_DivisionHead_Details Get_OperatingHead_(long? SYKI, long Empcode, long Userid)
        {
            var objresult = _objRiskAssessmentRepository.Get_OperatingHead_(SYKI, Empcode, Userid);
            return objresult;
        }

        public int Risk_Approve_By_OperatingHead(CommonRiskAssessmentVM risk_details)
        {
            var objresult = _objRiskAssessmentRepository.Risk_Approve_By_OperatingHead(risk_details);
            return objresult;
        }


        public List<PrimaryAssestsVM> BindPrimaryAsset(long? div_Id)
        {
            var objresult = _objRiskAssessmentRepository.BindPrimaryAsset(div_Id);
            return objresult;
        }



        public List<Risk_ISSCMember_details> Send_Email_To_ISSC_Member_DivisionHead(Risk_DifiencyReport difiency)
        {
            var objresult = _objRiskAssessmentRepository.Send_Email_To_ISSC_Member_DivisionHead(difiency);
            return objresult;
        }
        public int InsertRiskDifiency(Risk_DifiencyReport difiency)
        {
            var objresult = _objRiskAssessmentRepository.InsertRiskDifiency(difiency);
            return objresult;
        }

        //mailer utility
        public List<RiskMailerReminderVM> GetReminderDetailsForISSCMember_Nomination()
        {
            var objresult = _objRiskAssessmentRepository.GetReminderDetailsForISSCMember_Nomination();
            return objresult;
        }

        public RiskMailerReminderVM GetStart_Dt_End_Dt_ISSC_Nomination(long SYKI)
        {
            var objresult = _objRiskAssessmentRepository.GetStart_Dt_End_Dt_ISSC_Nomination(SYKI);
            return objresult;
        }

        public string RiskGetReminderDate(long id)
        {
            var objresult = _objRiskAssessmentRepository.RiskGetReminderDate(id);
            return objresult;
        }

        public string InsertISSC_Reminder(RiskMailerReminderVM mailer)
        {
            var objresult = _objRiskAssessmentRepository.InsertISSC_Reminder(mailer);
            return objresult;
        }

        //ends here
    }
}
