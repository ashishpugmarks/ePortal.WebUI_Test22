using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.Infrastructure.Repositories;
using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Services
{
    public class MedicalInsuranceService : IMedicalInsuranceService
    {
        MedicalInsuranceRepository objMedicalRepository;
        public MedicalInsuranceService(MedicalInsuranceRepository medicalInsuranceRepository)
        {
            objMedicalRepository = medicalInsuranceRepository;
        }
       
        #region Deshboard
        public int GetIsExistNewJoinee(long Emp_Code)
        {
            return objMedicalRepository.GetIsExistNewJoinee(Emp_Code);
        }
        public int GetIsExistRenewal(long SiteId, long UserType)
        {
            return objMedicalRepository.GetIsExistRenewal(SiteId, UserType);
        }

        public PolicyAndPlantMappingViewModel GetDashboardInfo(long DesId, long EmpCode)
        {
            return objMedicalRepository.GetDashboardInfo(DesId, EmpCode);
        }
        public MedicalInsuranceViewModel UserPendingRequest(long EmpCode)
        {
            return objMedicalRepository.UserPendingRequest(EmpCode);
        }
        #endregion

        #region Save/Update/Detail
        public Int16 SaveMedicalInsuranceDetail(MedicalInsuranceViewModel MIVM)
        {
            return objMedicalRepository.SaveMedicalInsuranceDetail(MIVM);
        }
        public MedicalInsuranceViewModel GetActivePolicyDetails(Int64 id)
        {
            return objMedicalRepository.GetActivePolicyDetails(id);
        }
        #endregion

        #region Master
        public IEnumerable<SYSITE> Bind_SYSite()
        {
            return objMedicalRepository.Bind_SYSite();
        }
        public IEnumerable<SYPLANT> Bind_SYPlant()
        {
            return objMedicalRepository.Bind_SYPlant();
        }
        public IEnumerable<ADDESIGNATION> Bind_ADDesignation()
        {
            return objMedicalRepository.Bind_ADDesignation();
        }
        public List<PolicyMasterViewModel> GetPolicyTypeList()
        {
            return objMedicalRepository.GetPolicyTypeList();
        }

        public Int16 SaveRenewalPeriod(RenewalPeriodViewModel RPVM)
        {
            return objMedicalRepository.SaveRenewalPeriod(RPVM);
        }
        public List<EmployeePolicyLocationMapping> GetEmpForSendMail(Int16 PlantId)
        {
            return objMedicalRepository.GetEmpForSendMail(PlantId);
        }

        public ADEmployeeViewModel GetADEmployeeDetail(long EmpCode)
        {
            return objMedicalRepository.GetADEmployeeDetail(EmpCode);
        }

        public Int16 DeactivateRenewalPeriod(RenewalPeriodViewModel RPVM)
        {
            return objMedicalRepository.DeactivateRenewalPeriod(RPVM);
        }
        public List<RenewalPeriodViewModel> GetRenewalPeriodList()
        {
            return objMedicalRepository.GetRenewalPeriodList();
        }

        public short SaveEmployeePolicyLocation(EmployeePolicyLocationMapping EPLM)
        {
            return objMedicalRepository.SaveEmployeePolicyLocation(EPLM);

        }
        public List<EmployeePolicyLocationMapping> GetEmployeePolicyLocationList()
        {
            return objMedicalRepository.GetEmployeePolicyLocationList();
        }

        public Int16 SaveMappingDetail(List<PolicyAndPlantMappingViewModel> PPVM)
        {
            return objMedicalRepository.SaveMappingDetail(PPVM);
        }
        public List<PolicyAndDesignationMappingViewModel> GetMappingList()
        {
            return objMedicalRepository.GetMappingList();
        }

        public List<PolicyAndPlantMappingViewModel> GetPolicyDesgMappingList(PolicyAndPlantMappingViewModel oData)
        {
            List<MEDINS_TYPEPLANT_MAP> oList = objMedicalRepository.GetPolicyDesgMappingList(oData);
            List<PolicyAndPlantMappingViewModel> retList = new List<PolicyAndPlantMappingViewModel>();
            if (oList != null && oList.Count() >= 0)
            {
                foreach (var iVal in oList.Where(x => x.STATUS == 1))
                {
                    PolicyAndPlantMappingViewModel obj = new PolicyAndPlantMappingViewModel();
                    //obj.DesignationIds = iVal.MEDINS_DESG_POLICYTYPE_MAP.Where(x => x.STAUS == 1).Select(x => x.DESG_ID).ToArray();
                    obj.DesignationIds = objMedicalRepository.GetDesgMappingData(iVal.MAPPINGID);
                    obj.AssociatedPaid_Cnt = iVal.ASSOCIATEPAID_CNT;
                    obj.CompanyPaid_Cnt = iVal.COMPANYPAID_CNT;
                    obj.PlantId = iVal.PLANTID;
                    obj.MappingId = iVal.MAPPINGID;
                    obj.PolicyTypeId = iVal.POLICYTYPE_ID;
                    obj.PolicyTypeId = iVal.POLICYTYPE_ID;
                    retList.Add(obj);
                }
            }
            return retList;
        }
        #endregion

        #region HealthCenter
        public MedicalInsuranceViewModel GetPolicyDetails(Int64 id)
        {
            return objMedicalRepository.GetPolicyDetails(id);
        }
        public List<HealthCenterViewModel> HealthCenterApprovalList(HealthCenterApprovalViewModel PageMdl)
        {
            return objMedicalRepository.HealthCenterApprovalList(PageMdl);
        }

        public short RequestApproval(long APPROVALID, short AppStatus, string Remark, long UserId)
        {
            return objMedicalRepository.RequestApproval(APPROVALID, AppStatus, Remark, UserId);
        }
        public MedicalInsuranceViewModel GetAllPolicyDetails(Int64 id)
        {
            return objMedicalRepository.GetAllPolicyDetails(id);
        }
        public List<VW_MEDINS_EMPDETAIL> GetHealthCenterDataForExcel(HealthCenterApprovalViewModel PageMdl)
        {
            return objMedicalRepository.GetHealthCenterDataForExcel(PageMdl);
        }
        public List<VW_MEDINS_MSTDETAILS> GetMasterDataForExcel(int plantId, string _ApprovalDateFrom, string _ApprovalDateTo, string _ReqDateFrom, string _ReqDateTo)
        {
            return objMedicalRepository.GetMasterDataForExcel(plantId, _ApprovalDateFrom, _ApprovalDateTo, _ReqDateFrom, _ReqDateTo);
        }
        public MasterReportViewModel GetMasterDataReport(int currentPage, int plantId, string _ApprovalDateFrom, string _ApprovalDateTo, string _ReqDateFrom, string _ReqDateTo)
        {
            return objMedicalRepository.GetMasterDataReport(currentPage, plantId, _ApprovalDateFrom, _ApprovalDateTo, _ReqDateFrom, _ReqDateTo);
        }
        public FileViewModel GetProofForDownload(Int64 Id, Int64 EmpCode)
        {
            return objMedicalRepository.GetProofForDownload(Id, EmpCode);
        }
        #endregion

        #region Other Associate User
        public List<MedicalInsuranceViewModel> AdminPendingRequest(long EmpCode)
        {
            return objMedicalRepository.AdminPendingRequest(EmpCode);
        }
        public PolicyAndPlantMappingViewModel GetAdminDashboardInfo(long DesId, long EmpCode)
        {
            return objMedicalRepository.GetAdminDashboardInfo(DesId, EmpCode);
        }
        public PolicyAndPlantMappingViewModel GetOtherAssociateInfo(long EmpCode)
        {
            return objMedicalRepository.GetOtherAssociateInfo(EmpCode);
        }
        public short SaveOtherAssociateUserDetail(MedicalInsuranceViewModel MIVM)
        {
            return objMedicalRepository.SaveOtherAssociateUserDetail(MIVM);
        }

        public MedicalInsuranceViewModel NewJoineeOtherAssociateByEmpCode(Int64 EmpCode, Int64 PlantId)
        {
            return objMedicalRepository.NewJoineeOtherAssociateByEmpCode(EmpCode, PlantId);
        }
        public MedicalInsuranceViewModel OtherAssociateByEmpCode(long EmpCode)
        {
            return objMedicalRepository.OtherAssociateByEmpCode(EmpCode);
        }
        #endregion

    }
}
