using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.ViewModels;
using ePortal.DomainClasses;


namespace ePortal.Application.Contracts
{
    public interface IPRService
    {
        Tuple<short, long> SavePRRequest(PRHeaderViewModel model);
        short SendMailByRequestorAfterHoldandUploadDoc(PRHeaderViewModel model); //  Added by Aumento as on 16072024
        short AddReleaseHoldentry(PRHeaderViewModel model);   //added by aumento for SR86919
        Tuple<short, List<PRDetailViewModel>> SaveAttachment(long addedDate, long poHid, List<PRDetailViewModel> modelList);
        Tuple<short, List<PRDetailViewModel>> DeleteAttachment(string fileName, string docType, long poHid);
        Employee_Details GetAuthEmpById(long empCode, int designationId, string designation, Employee_Details empDtl);
        PRHeaderViewModel GetPRRequestById(long id);
        short PRApproval(PRAppHistoryViewModel PHVM, Employee_Details emp_dtl);
        short PRCancel(PRHeaderViewModel PHVM);
        List<PRAppAuthSeqViewModel> GetDefaultAuthority(long loginUser, decimal indentAmt, long PRTYPE, Employee_Details obj, long indent_type, bool ITServiceMatSISAppStatus, bool ISIPCheck, PRHeaderViewModel PRDATA);//SIS PR Change,IP Check // { PRDATA} Added by Aumento ::  SR68003
        SearchIndent PRDashboard(SearchIndent PHVM);
        List<ADORGLEVEL> GetOrgLevelList(long typeId);
        Tuple<short, List<ADORGLEVEL>> BindOperationForOPMap(long loginUser);
        List<PR_Div_Dep_SecViewModel> BindDivision(long op_Id);
        List<PR_Div_Dep_SecViewModel> BindDepartment(long div_Id, long op_Id);
        List<PR_Div_Dep_SecViewModel> BindSection(long dep_Id, long div_Id, long op_Id);
        PRPURStatusViewModel GetPRPURStatusById(long id);
        short UpdatePRStatus(PRPURStatusViewModel PPVM, Employee_Details employeeDetails);
        short ChangeSLACategory(long PRID, short selectedSLACategory, long UpdatedBy);
        SearchIndentUser PRUserDashboard(SearchIndentUser PHVM);
        SearchIndentUser PRAgeingHis(SearchIndentUser PHVM);

        List<PR_Div_Dep_SecViewModel> BindPlant(long Plant_Id);
        long GetPRNextApprovalId(long PRID, long ecode);

        List<PRBuyerMstViewModel> GetBuyerMstList(long? GPType, long? ADOrgLevelId);
        PRBuyerMapViewModel GetPRBuyerByHeaderId(long id);
        short AssignBuyer(PRBuyerMapViewModel PBVM);
        short PRChangeCategory(long PRID, short selectedCategory, long UpdatedBy);
        short UpdateAllocationStatus(long PRID, long UpdatedBy);
        SearchIndent PRBuyerDashboard(SearchIndent VM);
        List<DGIT_PRCAT_MST> BindPRCategory();

        List<PR_DGIT_PRADDAPP_MST> Get_PRADDAPP_MST_List(PR_DGIT_PRADDAPP_MST PR);

        List<Employee_Details> PortalAutocompleteSuggestionsFunDesig(string term);

        List<Employee_Details> PortalAutocompleteSuggestionsActDesig(string term);

        long AddEditDgitPraddapp_mst(List<PR_DGIT_PRADDAPP_MST> data);

        List<AD_orglevel_type> getOrgLevelType();

        List<AD_orglevel> GetOrgUnitData(int id);

        //================================================Start=================================================
        //                                 Allocator Data Management 29-08-2022 (Aumento)
        //======================================================================================================
        List<DGIT_PRCAT_MST> getDgitPRCatMst();

        List<PR_dgit_properationmap> Get_Properationmap_List(PR_dgit_properationmap PR);
        List<AD_orglevel> GetOperationList();

        long SaveAllocatorMaster(List<PR_dgit_properationmap> data);

        List<PR_dgit_properationmap> getPR_properationmapReport(PR_dgit_properationmap pr);
        int Get_EmployeeMap_Count(PR_dgit_properationmap PR);
        //=======================================End(Allocator)===============================

        //================================================Start=================================================
        //                                 Buyer Data Management 29-08-2022 (Aumento)
        //======================================================================================================
        List<AD_orglevel> PortalAutocompleteOrgLevel(string Key);

        List<PRBuyerMstViewModel> getPRBuyerMstList(PRBuyerMstViewModel obj);
        long SaveBuyerMaster(List<PRBuyerMstViewModel> data);

        List<PRBuyerMstViewModel> getPR_BuyermapReport(PRBuyerMstViewModel obj);

        List<Employee_Details> PortalAutocompleteSuggestionsEmployee(string term, int Catid, int Orgid);
        //=======================================End(Buyer)===============================

        //--- SR52365==============================
        Tuple<long, long, long, long, List<PR_Div_Dep_SecViewModel>> BindDivision(long op_Id, long _syKi, long adempcode);
        Tuple<long, long, long, long, List<PR_Div_Dep_SecViewModel>> BindDepartment(long div_Id, long _syKi, long adempcode);
        Tuple<long, long, long, long, List<PR_Div_Dep_SecViewModel>> BindSection(long div_Id, long dept_Id, long _syKi, long adempcode);
        List<SYKI> GetKICodeList(long userId);

        SearchIndentUser PRPIUserDashboard(SearchIndentUser VM);
        //==============================
        List<PRBuyerMstViewModel> GetABuyerMstList(long? ADOrgLevelId);

        //Added by aumento as on 19092024 for the SR71870============================================================
        List<SYKI> GetKICodeList_ForPRDBOperationwise(long userId);

        Employee_Details GetEmployeeDetail(long ecode, long KIID);
        Tuple<long, List<PR_Div_Dep_SecViewModel>> BindDivision_PRDB(long op_Id, long LoginEcode, long kiid);
        Tuple<long, List<PR_Div_Dep_SecViewModel>> BindOperation(long Loginempcode, long KIID);
        Tuple<long, List<PR_Div_Dep_SecViewModel>> BindSection(long ecode, long kIID, long operationID, long divisionID, long dEPTID);
        Tuple<long, List<PR_Div_Dep_SecViewModel>> BindDepartment(long ecode, long kIID, long operationID, long divisionID);

        SearchIndentUser PRDashboadReport(SearchIndentUser PHVM);

        object GetPRReleasePlants(); //  Added by Aumento ::  SR68003
        //===========================================================================================================
        List<DateTime> GetHolidaysByPlant(DateTime startDate, DateTime endDate, long siteId); //Added by TTL on 24-May-2025 against SR99130 > CR6361
    }
}
