using ePortal.DomainClasses;
using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface IAT_ApprovalService
    {

        #region capitalize asset
        #region Approval Master
      

        Tuple<int, long,int, string> GetAssetCode(string AssetCode/*, string AssetType*/);

       

        #endregion

        #region Approval Operation Mapping
        List<AT_APPROVAL_OPERATION_MAPPING> ATOMGetDataList();

        LibResult GetOperationlist();

        AT_APPROVAL_OPERATION_MAPPING ATOMEdit(int id);

        LibResult GetDivisionList(long Operation);

        LibResult GetEVPUser(long div_id);

        LibResult GetSykilist();
        LibResult GetSykilistforatom();

        LibResult ATOMAdd(string ApprovalType, int OperationID, int SYKIID, int Division, int EvpAuth, int LoginCode);

        LibResult ATOMSearch(string ApprovalType, int Operation, int Syki);

        bool ATOMDelete(int id);

        #endregion

        #region  Capitalized Asset Transfer

        LibResult GetDepartment();

        LibResult Maxsrno();

        bool CapitalizedEditDelete(int id);
        LibResult AutocomplitTranNo();

        LibResult Requestor_Detail(string AssetType, string Transferor, int loginCode);
        LibResult GetTransferor(string AssetType, int loginCode);

        LibResult BindApprovalType(int LoginCode);

        List<Employee_Details> AutocompleteSuggestions(string term, string department);
        LibResult SaveTransaction(AT_ASSSET_TRANSFER_HEADER hd, List<AT_ASSET_TRANSFER_DETAIL> dt, List<AT_APPROVAL_AUTHORITY> _at, string flag, string AssetType, int LoginCode);

        Employee_Details GetAuthEmpById(int empCode, Employee_Details empDtl, int TranNo, string Department);

        List<AT_AssetTransferHeaderViewModel> GetCapitalizeList(int LoginCode, string flag);

        List<AT_APPROVAL_AUTHORITY_LOG> History(int LoginCode, int TranNo);
        List<AT_AssetTransferHeaderViewModel> CapitalizeListSearch(string TransactionType, /*string AssetType,*/ int TransactionNo, string TransfereeUser, string Status, int LoginCode, string flag);

        List<AT_AssetTransferHeaderViewModel> CapitalizeStatusSearch(string Status, int LoginCode, string flag);

        LibResult CapitalizeListDelete(int id, string flag);

       

        AT_ASSSET_TRANSFER_HEADER CapitalizedEdit(int id);


        LibResult CapitalizedDetail(int id, int LoginCode, int APPAUTHSRNO);

        List<AT_AssetTransferAuthorityViewModel> CapitalizedAuthority(int id);

        LibResult EditTransaction(AT_ASSSET_TRANSFER_HEADER hd, List<AT_ASSET_TRANSFER_DETAIL> dt, List<AT_APPROVAL_AUTHORITY> _at, string flag, int LoginCode,string AssetType);

        #endregion

        #region approval list

        short UpdateApproval(string remarks, int Logincode, int TranNo, string Status, List<AT_ASSET_TRANSFER_DETAIL> dt);

        AT_AssetTransferHeaderViewModel CapitalizedAssetTransferEdit(int id);
        List<AT_AssetTransferApprovalViewModel> ApprovalListSearchData(string TranjectionType, string AssetType, int TranjectionNo, string Status, int LoginCode, string flag);

        List<AT_AssetTransferApprovalViewModel> ApprovalListSearchDataStatus(string Status, int LoginCode, string flag);
        List<AT_AssetTransferApprovalViewModel> GetApprovalList(int LoginCode);
        List<AT_AssetTransferApprovalViewModel> SecurityGetApprovalList(int LoginCode);
        List<AT_AssetTransferAuthorityViewModel> Gethead(short LoginCode, int TranNo, string TranType, int Transfree, string ApprovalType, long? operationMappId);
        List<AT_AssetTransferApprovalViewModel> GetTaxationApprovalList(int LoginCode);

        List<AT_AssetTransferApprovalViewModel> GetFinanceApprovalList(int LoginCode);

        List<AT_AssetTransferApprovalViewModel> GetFinanceApproveList(int LoginCode);

        List<AT_AssetTransferApprovalViewModel> GetFinalFinanceApprovalList(int LoginCode, string flag);
        #endregion

        #region Taxation Approval
        AT_ASSSET_TRANSFER_HEADER Get_AT_ASSSET_TRANSFER_HEADER_ModelData(int id);

        short UpdateFinanceApproval(string remarks, int Logincode, int TranNo, string Status);

        LibResult UploadFile(int TranNo, string filepath, long EmpCode);
        LibResult GetinstampUpload(int TranNo, string filepath, long EmpCode);

        List<object> GetAppAuthLogHistory(int TranNo);

        LibResult GetNewLocation(string Key, string TransfreeType);
        #endregion

        #region commanapproval master
        List<AT_COMMON_APPROVAL_MASTER> GetCommanApprovalList();

        LibResult BindPlantName();

        LibResult AutocomplitName(int Ecode);

        //List<Employee_Details> getempbyplant(int term);
        AT_COMMON_APPROVAL_MASTER AT_CommanApprovalEdit(int id);
        bool CommanApprovalDelete(int id);
        LibResult GetCommanApprovalSave(int srno, int Sysiteid, string Department, int ECode, decimal Sequenceno, int LoginCode, string Mode);
        LibResult GetCommanApprovalEdit(int srno, decimal Sysiteid, string Department, int ECode, decimal Sequenceno, int LoginCode);

        LibResult ATOMUpdate(int srno, string ApproveType, int Operation, int SYKIID, int Division, int EvpAuth, int LoginCode);

        List<AT_COMMON_APPROVAL_MASTER> GetCommanApprovalSearch(string Department);
        #endregion

        #region Asset Attachment

        bool UploadFileAndSave(int TranNo, string AssetCode, string file);

        bool UploadFileAndSaveCap(int TranNo, string AssetCode, string file);

        #endregion 
        #endregion

        #region Non Capitalize Asset
        AT_ASSSET_TRANSFER_HEADER NonCapitalizedEdit(int id);

        GetAssetFormHeader Get_AssetRegister_Header_Detail(long? UserID, long Tran_No, string filename);

        bool filesave(string filename, long Tran_No);
        List<AT_ASSET_TRANSFER_DETAIL> DetailRecord(long? UserID, long Tran_No);

        GetAuthDetail GetAuthorityDetail(long? UserID, long Tran_No);
        #endregion

        #region MIS Report

        List<AT_AssetTransferMISReportViewModel> MISReportSearch(string TransectionType, string AssetType, DateTime FromDate, DateTime ToDate, int LoginCode, string Plant);

        LibResult GetPlanForMISReport();

        #endregion

        #region Manage Asset Transfer By Admin
        List<AT_AssetTransferHeaderViewModel> ManageAssetTransfer(int LoginCode, string flag);

        AT_APPROVAL_AUTHORITY AuthEdit(int id);

        LibResult AuthoritySave(int Empcode, string EmpName, int srno, string Department,int LoginCode);

        long? GetOperationMappId(Employee_Details _Employee_Details);

        LibResult ManageRequestor_Detail(string AssetType, string Transferor, int AddedBy, int loginCode);
        #endregion

        //SR109866-CR7276

        #region Capitalized Asset transfer Report
        List<AT_AssetTransferHeaderViewModel> GetCapitalizeAssetTransferReportList(int LoginCode, string flag);

        List<AT_AssetTransferHeaderViewModel> CapitalizeAssetTransferReportListSearch(string TransactionType, /*string AssetType,*/ int TransactionNo, string TransfereeUser, string Status, int LoginCode, string flag);

        List<AT_AssetTransferHeaderViewModel> CapitalizeAssetTransferReportStatusSearch(string Status, int LoginCode, string flag);
        #endregion
        //SR109866-CR7276

    }
}
