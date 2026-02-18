using ePortal.DomainClasses;
using ePortal.ViewModels;
using System;
using System.Collections.Generic;

namespace ePortal.Application.Contracts
{
    public interface IAssetDisposalService
    {
        #region Master
        List<SIS_ASSETTYPE_MST> GetAssetTypeList();
        List<SYSITE> GetSiteList();
        List<ADORGLEVEL> GetOrgLevelList();
        string GetFnCodeByOperationId(long operationId);
        string CheckValidationByFnCode(string fnCode, string parmName);
        SYSITE GetSiteDetailBySiteId(long siteId);
        List<SIS_AST_PROCESSSTATUS_MST> GetProcessStatusList();
        List<SIS_AST_PARAM_MST> GetParamList();
        List<SIS_ASSETCONDITION> GetConditionList();

        #region Approval Authority Master
        List<ApprovalAuthorityViewModel> GetApprovalAuthorityList();
        ApprovalAuthorityViewModel GetApprovalAuthorityById(long id);
        short SaveApprovalAuthority(ApprovalAuthorityViewModel AAVM, String transactionType);
        List<ApprovalAuthorityViewModel> ManageApprovalAuthorityByFinance();
        List<ApprovalAuthorityViewModel> ManageApprovalAuthorityByPPC();
        ApprovalAuthorityViewModel GetApprovalAuthorityBySiteId(long SiteId, short Status_level);
        #endregion

        #region Operation Master
        List<OPMappingViewModel> GetOperationMappingList();
        OPMappingViewModel GetOperationMappingById(long id);
        short SaveOperationMapping(OPMappingViewModel OMVM);
        long? GetOperationMappId(Employee_Details _Employee_Details);

        List<AssetOperationViewModel> GetOperationList();
        AssetOperationViewModel GetOperationDtlById(long id);
        short SaveOperation(AssetOperationViewModel AOVM);
        int GetOperationCount(long _opId);
        #endregion

        #region Validation Master
        List<ValidationViewModel> GetValidationList();
        ValidationViewModel GetValidationById(long id);
        short SaveValidation(ValidationViewModel OMVM);
        #endregion
        #endregion

        #region Asset Disposal Request
        Tuple<short, long> SaveAssetItem(AssetDisposalViewModel ADVM);
        short SaveAssetRequest(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details);
        List<AssetDisposalViewModel> GetAssetRequestList(long loginUser);
        AssetDisposalViewModel GetAssetRequestById(long id, Employee_Details _Employee_Details);
        short UploadAttachmentByRequestor(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details);
        short CancelRequest(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details, int? cancelledByAdmin);
        List<AssetDisposalViewModel> GetApprovalHistory(long loginUser);
        AssetDisposalViewModel GetDetailById(long id, Employee_Details _Employee_Details);
        List<AssetDisposalViewModel> GetRequestReport(long AssetType, long EmpCode, string FromDate, string ToDate, long loginUser);
        #endregion

        #region Maintenance Dept. Approval
        List<AssetDisposalViewModel> GetApprovalListByMaintenanceDept(Employee_Details _Employee_Details);
        short RequestUpdateByMaintenanceDept(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details);
        #endregion

        #region Taxtion Approval
        List<AssetDisposalViewModel> GetApprovalListByTaxtion(Employee_Details _Employee_Details);
        short RequestUpdateByTaxtion(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details);
        #endregion

        #region User Approval
        List<AssetDisposalViewModel> GetApprovalListByUser(Employee_Details _Employee_Details);
        short RequestUpdateByUserApproval(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details);
        #endregion

        #region Finance Approval
        List<AssetDisposalViewModel> GetApprovalListByIC(Employee_Details _Employee_Details);
        short RequestUpdateByIC(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details);
        List<AssetDisposalViewModel> GetApprovalListByIBM(Employee_Details _Employee_Details);
        short RequestUpdateByIBM(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details);
        #endregion

        //Added by TTL :: SR95154 | CR6236
        #region Plant PPC Approval
        List<AssetDisposalViewModel> GetListForPlantPPCApproval(Employee_Details _Employee_Details);
        //List<AssetDisposalViewModel> GetListForPlantPPCApproval(long loginUser, string plantId);
        short RequestUpdateByPlantPPC(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details);
        #endregion
        // End by TTL :: SR95154 | CR6236

        #region Plant Mgf. & Quality Approval
        List<AssetDisposalViewModel> GetListForPlantApproval(Employee_Details _Employee_Details);
        List<AssetDisposalViewModel> GetApprovalListByPlanQty(Employee_Details _Employee_Details);
        short RequestUpdateByQty(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details);
        List<AssetDisposalViewModel> GetApprovalListByPlanMgf(Employee_Details _Employee_Details);
        short RequestUpdateByMgf(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details);
        AssetDisposalViewModel GetAssetRequestByMgf(long id, Employee_Details _Employee_Details);
        short RequestUpdateByPlantApproval(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details);
        short UpdatePlantApprovalRequiredByMgf(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details);
        #endregion

        #region Environment / Admin Approval
        List<AssetDisposalViewModel> GetApprovalListByEnvironment(Employee_Details _Employee_Details);
        short RequestUpdateByEnvironment(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details);
        List<AssetDisposalViewModel> GetApprovalListByAdmin(Employee_Details _Employee_Details);
        short RequestUpdateByAdmin(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details);
        #endregion

        #region Security Approval
        List<AssetDisposalViewModel> GetApprovalListBySecurity(Employee_Details _Employee_Details);
        short RequestUpdateBySecurity(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details);
        #endregion

        #region Upload Invoice
        List<AssetDisposalViewModel> GetApprovalListForUploadInvoice(int Status_Level, long loginUser);
        short UploadAssetInvoice(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details);
        #endregion

        #region Asset Retirement
        List<AssetDisposalViewModel> GetApprovalListByAssetRetirement(Employee_Details _Employee_Details);
        short RequestUpdateByAssetRetirement(AssetDisposalViewModel ADVM, Employee_Details _Employee_Details);
        #endregion

        string GetValidationParmValue(string parmName);
        List<AssetDisposalViewModel> GetRequestCancellationListForAdmin();
        short UpdateIBMDate(AssetDisposalViewModel ADVM);
        
        //Code added by TTL :: CR6754
        Employee_Details GetEmpDetailById(long EmpCode);
        bool IsPlantPpcMapped(long OperationId, long SiteId, long EmpCode);
        //Code end by TTL :: CR6754
    }
}
