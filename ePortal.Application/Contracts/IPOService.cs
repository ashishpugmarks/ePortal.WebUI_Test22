using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IPOService
    {
        List<VendorViewModel> GetVendorList();
        Tuple<short, long> SavePORequest(POHeaderViewModel model);
        Tuple<short, List<PODetailViewModel>> SaveAttachment(long addedDate, long poHid, List<PODetailViewModel> modelList);
        Tuple<short, List<PODetailViewModel>> DeleteAttachment(string fileName, string docType, long poHid);
        //Below added by aumento as on 02092024 for SR70820================================================================
        Employee_Details GetAuthEmpById(long empCode,long loginempcode);
        //List<Employee_Details> GetAuthEmpById(long empCode);
        //===================================================================================================================        
        POHeaderViewModel GetPORequestById(long id);
        short POApproval(POAppHistoryViewModel PHVM, Employee_Details emp_dtl);
        short POCancel(POHeaderViewModel PHVM);
        List<POAppAuthSeqViewModel> GetDefaultAuthority(long loginUser);
        PRHeaderViewModel GetPRDetailByPOId(long poid);
        long GetPRStatusByPOId(string[] pono);
        long GetPONextApprovalId(long poid, long ecode);
        Search_VW_PODASHBOARD_VWMODEL GetPODASHBOARD(Search_VW_PODASHBOARD_VWMODEL objSearch);
        short SentMailToVendor(long poHeaderId);

        SearchPO POALLUserReport(SearchPO VM);
        Tuple<long, List<PR_Div_Dep_SecViewModel>> BindDivision(long op_Id, long LoginEcode, long kiid);
        Tuple<long, List<PR_Div_Dep_SecViewModel>> BindOperation(long Loginempcode, long KIID);
        Tuple<long, List<PR_Div_Dep_SecViewModel>> BindDepartment(long Loginempcode, long KIID, long op_Id, long Div_id);
        Tuple<long, List<PR_Div_Dep_SecViewModel>> BindSection(long Loginempcode, long KIID, long op_Id, long divid, long deptid);
        List<PR_Div_Dep_SecViewModel> GetKiLIST(long ecode);
        Employee_Details GetEmployeeDetail(long ecode, long KIID);
        // added by aumento for SR82542
        List<POAppAuthSeqViewModel> GetPOAuthorityHis_ById(long id, long loginUser);
        // added by aumento for SR82542
    }
}
