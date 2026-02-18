
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.ViewModels;

namespace ePortal.Application.Contracts
{
    public interface IIOMService
    {
        Tuple<short, long> SaveIOMRequest(IOMHeaderViewModel model);
        Tuple<short, List<IOMDetailViewModel>> SaveAttachment(long addedDate, long iomHid, List<IOMDetailViewModel> modelList);
        Tuple<short, List<IOMDetailViewModel>> DeleteAttachment(string fileName, string docType, long iomHid);
        Employee_Details GetAuthEmpById(long empCode, Employee_Details empDtl);
        IOMHeaderViewModel GetIOMRequestById(long id);
        short IOMApproval(IOMAppHistoryViewModel IHVM, Employee_Details emp_dtl);
        void SendAnnotatedPdfOnFinalApproval(IOMAppHistoryViewModel IHVM, Employee_Details emp_dtl, string attachment); //Added by TTL CR5813 (08-Mar-2025)
        short SendMailByApprovalAuthorityForHoldRequest(IOMHeaderViewModel model, Employee_Details employeeDetails); //  Added by Aumento 13072024
        short IOMCancel(IOMHeaderViewModel IHVM);
        List<IOMAppAuthSeqViewModel> GetDefaultAuthority(long loginUser);
        List<Employee_Details> PortalAutocompleteSuggestions(string term, string designation);
        List<Employee_Details> AutocompleteDesignation(string term);
        long GetIOMNextApprovalId(long IOMID, long ecode);

        //Changed by TTL on 18th July 2025 against SR101913 > CR6738 - Start
        List<VM_DGIT_IOMCATMST> BindIOMCategory(string divisionId = "");
        //Changed by TTL on 18th July 2025 against SR101913 > CR6738 - End
        Tuple<short, long> SaveAdditionalIOMRequest(IOMHeaderViewModel model);
        List<IOMAppAuthSeqViewModel> PrevAuthority(string AppType, long Adempcode);
        SearchIOM IOMUserReport(SearchIOM VM);
        Tuple<long, List<PR_Div_Dep_SecViewModel>> BindDivision(long op_Id, long LoginEcode, long kiid);
        Tuple<long, List<PR_Div_Dep_SecViewModel>> BindOperation(long Loginempcode, long KIID);
        Tuple<long, List<PR_Div_Dep_SecViewModel>> BindDepartment(long Loginempcode, long KIID, long op_Id, long Div_id);
        Tuple<long, List<PR_Div_Dep_SecViewModel>> BindSection(long Loginempcode, long KIID, long op_Id, long divid, long deptid);
        List<PR_Div_Dep_SecViewModel> GetKiLIST(long ecode);
        Employee_Details GetEmployeeDetail(long ecode, long KIID);
        //Added by Aumento for SR99176
        List<PR_Div_Dep_SecViewModel> GetKICodeLIst();
        List<Employee_Details> GetAllOperation(long KIID);
        List<Employee_Details> GetDivisionByOp(long OpCode, long KIID);
        List<Employee_Details> GetDepartmentByDiv(long DivCode, long OpCode, long KIID);
        List<Employee_Details> GetSectionByDept(long DeptCode, long DivCode, long OpCode, long KIID);
        SearchIOM IOMAuditReport(SearchIOM VM);
        //Added by Aumento for SR99176

        // Added by TTL on 17-July-2025 against SR93758 > CR5975 - Start
        #region IOMDashboardChart
        Tuple<List<IOMDashboardGraphViewModel>, long> IOMDashboardGraphData(SearchIOM VM);
        SearchIOMDashboadReportList IOMDashboadReportList(SearchIOMDashboadReportList VM);
        SearchIOMDashboadReportList IOMDashboadReportListByDesignationGroup(SearchIOMDashboadReportList VM);
        Tuple<long, List<PR_Div_Dep_SecViewModel>> BindIOMGraphOperation(long Loginempcode, long KIID);
        #endregion
        // Added by TTL on 17-July-2025 against SR93758 > CR5975 - End

        //Added by TTL on 28-July-2025 against SR104160 > CR6821 - Start
        List<DropdownList> GetEmployeeListAsPerOrgLevel(long orgLevelId, long? KIID);
        List<DropdownList> GetEmployeeListAsPerOrgLevel(long[] orgLevelId, long? KIID);
        //Added by TTL on 28-July-2025 against SR104160 > CR6821 - End

        //Added by TTL on 28th Oct 2025 against SR109889 > CR7269 - Start
        List<PRDetailViewModel> GetPRDocumentsByIndentNo(List<string> IndentNos);

        //Added by TTL on 28th Oct 2025 against SR109889 > CR7269 - Start
    }
}
