using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.ViewModels;
using ePortal.DomainClasses;

namespace ePortal.Application.Contracts 
{
    public interface ISMService
    {
        Tuple<short, long> SaveSMRequest(SMHeaderViewModel model);
        Tuple<short, List<SMDetailViewModel>> SaveAttachment(long addedDate, long poHid, List<SMDetailViewModel> modelList);
        Tuple<short, List<VM_PADetailViewModel>> SavePAAttachment(long addedDate, List<VM_PADetailViewModel> modelList, string PA_HeaderID);
        short PARequest(VM_PaymentAdvise_Master data);
        List<VM_PaymentAdvise_Master> GetPADetails(VM_PaymentAdvise_Master obj);
		Tuple<short, List<SMDetailViewModel>> DeleteAttachment(string fileName, string docType, long smHid);
        Tuple<short, List<VM_PADetailViewModel>> DeletePAAttachment(string fileName, string docType, long PA_HeaderID, string PA_ID);
        List<VM_PADetailViewModel> PAdocs(string PA_ID);
        Employee_Details GetAuthEmpById(long empCode, int designationId, string designation, Employee_Details empDtl);
        SMHeaderViewModel GetSMRequestById(long id);
        short SMApproval(SMAppHistoryViewModel SAVM, Employee_Details emp_dtl, List<SMDetailViewModel> SmDetailList);
        short SMCancel(SMHeaderViewModel SHVM);
        Tuple<short, List<SMAppAuthSeqViewModel>> GetDefaultAuthority(long loginUser, decimal amount, short SM_Type, Employee_Details _Employee_Details);
        POHeaderViewModel GetPODetailByPOId(string poNo);
        VendorViewModel GetVendorByCode(string vcode);
        List<VendorViewModel> VendorAutocompleteSuggestions(string term);
        SearchSMViewModel SMDashboard(SearchSMViewModel SSM, long plantId);

        short UpdateDocStatus(long smheaderId, long updatedBy);
        short SMFinApproval(SMAppHistoryViewModel SAVM, Employee_Details emp_dtl, List<SMDetailViewModel> SmDetailList);
        SearchSMViewModel SMTaxationDashboard(SearchSMViewModel SSM, long loginUser);
        short SMTaxationApproval(SMAppHistoryViewModel SAVM);
        List<SMTaxationAuthViewModel> GetTaxationAuthority(short TaxTypeId, long plantId);
        short UpdateTaxationAuth(List<SMAppHistoryViewModel> iList, long updatedBy);

        List<PR_Div_Dep_SecViewModel> BindDepartment(long div_Id, long op_Id);
        List<PR_Div_Dep_SecViewModel> BindSection(long dep_Id, long div_Id, long op_Id);
        SearchIOCGMaster BindIOCGMasterList(SearchIOCGMaster _SM);
        List<PR_Div_Dep_SecViewModel> BindKI();
        List<VM_VW_SMIOCGAPPROVER_LIST> GetIOCGMasterByID(VM_VW_SMIOCGAPPROVER_LIST _SM);
        Tuple<short, string> SaveIOCGMasterData(VM_VW_SMIOCGAPPROVER_LIST _SM);
        Tuple<short, string> EditIOCGMasterData(VM_VW_SMIOCGAPPROVER_LIST _SM);
		short deletePAReq(DeletePA obj);
        long GetSMNO(string InvoiceNo, DateTime InvoiceDate, string VendorCode);
        //============Below added by aumento on 16052023 for SR50703=====================================================================================================================
        List<A00ADORGLEVELList> PortalAutocompleteSuggestionsForORG(long ddOLvType);
        Tuple<short, List<A00ADORGLEVELList>> PortalAutocompleteSuggestionsForORG_New(long ddOLvType);

        Tuple<short, long> SaveAdorglevel(long AddedBy, string ddOLvType, string Organization);
        List<SMDGIT_SMIOCGBLOCKViewModel> GetSMIOCGBLOCKData();

        Tuple<short, List<SMDGIT_SMIOCGBLOCKViewModel>> UpdateStatus(long SMIOCGBLOCKID, long ADORGLEVELID, long SRNO);
        Tuple<short, List<SMDGIT_SMIOCGBLOCKViewModel>> SaveUpdateStatus(List<SMDGIT_SMIOCGBLOCKViewModel> modelList);
        //============================================================================================================================================================================================
        
        //Added by aumento for SESMRN as on 23112023===========================
        IEnumerable<SYSITE> Bind_SYSite();//Added by aumento for SYSITE
        long GETSYSITEUSERID(string UserId);
        SearchSMViewModel SMIPDashboard(SearchSMViewModel VM, long plantId);
        //=====================================================================
    }
}
