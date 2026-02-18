using ePortal.DomainClasses;
using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IMedicalInsuranceService
    {
        int GetIsExistNewJoinee(long Emp_Code);
        int GetIsExistRenewal(long SiteId, long UserType);
        MedicalInsuranceViewModel UserPendingRequest(long EmpCode);
        PolicyAndPlantMappingViewModel GetDashboardInfo(long DesId, long EmpCode);

        Int16 SaveMedicalInsuranceDetail(MedicalInsuranceViewModel MIVM);
        MedicalInsuranceViewModel GetPolicyDetails(Int64 id);
        MedicalInsuranceViewModel GetActivePolicyDetails(Int64 id);
        List<HealthCenterViewModel> HealthCenterApprovalList(HealthCenterApprovalViewModel PageMdl);
        List<VW_MEDINS_EMPDETAIL> GetHealthCenterDataForExcel(HealthCenterApprovalViewModel PageMdl);
        List<VW_MEDINS_MSTDETAILS> GetMasterDataForExcel(int plantId, string _ApprovalDateFrom, string _ApprovalDateTo, string _ReqDateFrom, string _ReqDateTo);
        MasterReportViewModel GetMasterDataReport(int currentPage, int plantId, string _ApprovalDateFrom, string _ApprovalDateTo, string _ReqDateFrom, string _ReqDateTo);

        IEnumerable<SYSITE> Bind_SYSite();
        IEnumerable<SYPLANT> Bind_SYPlant();
        IEnumerable<ADDESIGNATION> Bind_ADDesignation();
        List<PolicyMasterViewModel> GetPolicyTypeList();

        Int16 SaveRenewalPeriod(RenewalPeriodViewModel RPVM);
        Int16 DeactivateRenewalPeriod(RenewalPeriodViewModel RPVM);
        List<RenewalPeriodViewModel> GetRenewalPeriodList();
        List<EmployeePolicyLocationMapping> GetEmpForSendMail(Int16 PlantId);
        ADEmployeeViewModel GetADEmployeeDetail(Int64 EmpCode);


        Int16 SaveEmployeePolicyLocation(EmployeePolicyLocationMapping EPLM);
        List<EmployeePolicyLocationMapping> GetEmployeePolicyLocationList();

        Int16 SaveMappingDetail(List<PolicyAndPlantMappingViewModel> PPVM);
        List<PolicyAndDesignationMappingViewModel> GetMappingList();
        List<PolicyAndPlantMappingViewModel> GetPolicyDesgMappingList(PolicyAndPlantMappingViewModel oData);
        FileViewModel GetProofForDownload(Int64 Id, Int64 EmpCode);

        Int16 RequestApproval(Int64 APPROVALID, Int16 AppStatus, string Remark, long UserId);
        MedicalInsuranceViewModel GetAllPolicyDetails(Int64 id);

        List<MedicalInsuranceViewModel> AdminPendingRequest(long EmpCode);
        PolicyAndPlantMappingViewModel GetAdminDashboardInfo(long DesId, long EmpCode);
        PolicyAndPlantMappingViewModel GetOtherAssociateInfo(long EmpCode);
        MedicalInsuranceViewModel NewJoineeOtherAssociateByEmpCode(Int64 EmpCode, Int64 PlantId);
        MedicalInsuranceViewModel OtherAssociateByEmpCode(Int64 EmpCode);
        Int16 SaveOtherAssociateUserDetail(MedicalInsuranceViewModel MIVM);

    }
}
