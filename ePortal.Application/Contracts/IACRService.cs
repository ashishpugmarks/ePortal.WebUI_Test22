using ePortal.DomainClasses;
using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IACRService
    {
        List<VendorViewModel> GetVendorList();
        Tuple<short, long> SaveACRRequest(ACRHeaderViewModel model);
        Tuple<short, List<ACRDetailViewModel>> SaveAttachment(long addedDate, long poHid, List<ACRDetailViewModel> modelList);
        Tuple<short, List<ACRDetailViewModel>> DeleteAttachment(string fileName, string docType, long poHid);
        //Employee_Details GetAuthEmpById(long empCode);

        Employee_Details GetAuthEmpById(long empCode, int designationId, string designation, Employee_Details empDtl);
        Employee_Details GetOAuthEmpById(long empCode, int designationId, string designation, Employee_Details empDtl);

        ACRHeaderViewModel GetACRRequestById(long id);
        short ACRApproval(ACRAppHistoryViewModel PHVM, Employee_Details emp_dtl);
        short ACRCancel(ACRHeaderViewModel PHVM);
        List<ACRAppAuthSeqViewModel> GetDefaultAuthority(long loginUser);
        POHeaderViewModel GetPODetailByPOId(string poNo);
        long GetPRStatusByPOId(string pono);
        //============Change Done on 27082022 For Add Other Category by (Aumento)=====================================================================================================================
        List<Employee_Details> PortalAutocompleteSuggestions(string term, string designation);
        //===============================================================================================================================================================================================
        //Added by Aumento Start
        List<PR_Div_Dep_SecViewModel> BindKI();
        SearchACRViewModel ACRDashboard(SearchACRViewModel SSM, long plantId);
        short PARequest(VM_ACR_PaymentAdvise_Master data);
        //long GetSMNO(string InvoiceNo, DateTime InvoiceDate, string VendorCode);
        short UpdateDocStatus(long smheaderId, long updatedBy, Employee_Details emp_dtl); //Updated By Aumento as on 28022024
        Tuple<short, List<VM_ACR_PADetailViewModel>> SavePAAttachment(long addedDate, List<VM_ACR_PADetailViewModel> modelList, string PA_HeaderID);
        List<VM_ACR_PaymentAdvise_Master> GetPADetails(VM_ACR_PaymentAdvise_Master obj);
        Tuple<short, List<VM_ACR_PADetailViewModel>> DeletePAAttachment(string fileName, string docType, long PA_HeaderID, string PA_ID);
        short deletePAReq(DeleteACRPA obj);
        List<VM_ACR_PADetailViewModel> PAdocs(string PA_ID);
        short ACRFinApproval(ACRAppHistoryViewModel SAVM, Employee_Details emp_dtl, List<ACRDetailViewModel> SmDetailList);

        IEnumerable<SYSITE> Bind_SYSite();//Added by aumento for SYSITE

        //Added by aumento as on 28092024 for the SR80813 ===============================================
        long GetACRNextApprovalId(long ACRID, long ecode);
        //Ended by aumento as on 28092024 for the SR80813 ===============================================
        LibResult AutocompleteSuggestionsVendor(string Key); // added by aumento :: SR111540
        LibResult AutocompleteSuggestionsPONumber(string Key); // added by aumento :: SR111540
    }
}
