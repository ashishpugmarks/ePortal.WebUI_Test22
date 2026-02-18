using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.DomainClasses;

namespace ePortal.Application.Contracts
{
    public interface IICIBMService
    {
        List<VM_DGIT_ICINVEST_MST> GetInvestEffectList();
        Tuple<short, long> SaveICRequest(ICREQHEADER model);
        Tuple<short, List<VM_DGIT_ICDOCDETAIL>> SaveAttachment(long addedDate, long ICHid, List<VM_DGIT_ICDOCDETAIL> modelList);
        Tuple<short, List<VM_DGIT_ICDOCDETAIL>> DeleteAttachment(string fileName, string docType, long ICHid);
        ICREQHEADER GetICRequestById(long id);
        short ICApproval(VM_DGIT_ICAPPHISTORY PHVM, Employee_Details emp_dtl);
        Employee_Details GetAuthEmpById(long empCode, Employee_Details empDtl);
        VM_ICIBM_FIDASHBOARD_Search GetFinanceDashboard(VM_ICIBM_FIDASHBOARD_Search obj);
        List<VM_SELECTITEMLIST> GetCycle(VM_SELECTITEMLIST obj);
        short ICFinanceApproval(VM_DGIT_ICAPPHISTORY PHVM, Employee_Details emp_dtl);
        short RejectICRequest(VM_DGIT_ICAPPHISTORY PHVM, Employee_Details emp_dtl);
        short ICApprovalInitiation(VM_ICApprovalInitiation PHVM, Employee_Details emp_dtl);
        VM_ICIBM_FITAXDASHBOARD_Search GetFinanceTAXDashboard(VM_ICIBM_FITAXDASHBOARD_Search obj);
        List<VM_DGIT_ICAPPAUTHSEQ> GetICAuthority();
        short ICFinTaxApproval(VM_DGIT_ICAPPHISTORY PHVM, Employee_Details emp_dtl);
        VM_ICIBM_FIAUCDASHBOARD_Search GetFinanceAUCDashboard(VM_ICIBM_FIAUCDASHBOARD_Search obj);

        //Asset Disposal IC Request
        Tuple<short, long> SaveAssetICRequest(ICASSETDISREQHEADER model);
        Tuple<short, List<VM_DGIT_ICDOCDETAIL>> SaveAssetAttachment(long addedDate, long ICHid, List<VM_DGIT_ICDOCDETAIL> modelList);
        Tuple<short, List<VM_DGIT_ICDOCDETAIL>> DeleteAssetAttachment(string fileName, string docType, long poHid);
        ICASSETDISREQHEADER GetICAssetRequestById(long id);
        short ICAssetApproval(VM_DGIT_ICAPPHISTORY PHVM, Employee_Details emp_dtl);
        short RejectICAssetRequest(VM_DGIT_ICAPPHISTORY PHVM, Employee_Details emp_dtl);
        short ICAssetFinanceApproval(VM_DGIT_ICAPPHISTORY PHVM, Employee_Details emp_dtl);
        long SaveICMemberMaster(long AddedBy, List<VM_DGIT_ICAPPAUTHSEQ> PSVMList);
        List<VM_DGIT_ICCONFIG_MST> GetICConfig();
        short saveIC(VM_DGIT_ICCONFIG_MST data);
        short deleteIC(VM_DGIT_ICCONFIG_MST data);
        VM_DGIT_ICCONFIG_MST GetICConfig(VM_DGIT_ICCONFIG_MST data);
        short updateIC(VM_DGIT_ICCONFIG_MST data);
        Tuple<short, List<VM_DGIT_ICDOCDETAIL>> SaveAssetAttachmentFinal(long addedDate, long ICHid, List<VM_DGIT_ICDOCDETAIL> modelList);
        Tuple<short, List<VM_DGIT_ICDOCDETAIL>> SaveAttachmentFinal(long addedDate, long ICHid, List<VM_DGIT_ICDOCDETAIL> modelList);
        short ICReqCancel(VM_DGIT_ICAPPHISTORY PHVM);
        short ICAssetReqCancel(VM_DGIT_ICAPPHISTORY PHVM);
        VM_ICIBM_PPCDASHBOARD_Search GetPPCDashboard(VM_ICIBM_PPCDASHBOARD_Search pHVM);
        List<VM_SELECTITEMLIST> GetICInvestdtls();
        Tuple<short, long> UpdateAUCCode(ICREQHEADER model);
    }
}
