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
    public class AT_ApprovalService : IAT_ApprovalService
    {
        private readonly AssetTransferRepository repo;
        public AT_ApprovalService(AssetTransferRepository _repo)
        {
            repo = _repo;
        }

        #region capitalize asset
        #region ApprovalMaster

        public Tuple<int, long,int, string> GetAssetCode(string AssetCode/*, string AssetType*/)
        {
           return repo.GetAssetCode(AssetCode/*, AssetType*/);
        }
        
        #endregion

        #region  Approval Operation Mapping
        public List<AT_APPROVAL_OPERATION_MAPPING> ATOMGetDataList()
        {
            return repo.ATOMGetDataList();
        }

        public AT_APPROVAL_OPERATION_MAPPING ATOMEdit(int id)
        {
            return repo.ATOMEdit(id);
        }


        public LibResult GetOperationlist()
        {
            return repo.GetOperationlist();
        }

        public LibResult GetDivisionList(long Operation)
        {
            return repo.GetDivisionlist(Operation);
        }

        public LibResult GetEVPUser(long div_id)
        {
            return repo.GetEVPUser(div_id);
        }

        public LibResult GetSykilist()
        {
            return repo.GetSykilist();
        }
        public LibResult GetSykilistforatom()
        {
            return repo.GetSykilistforatom();
        }

        public LibResult ATOMAdd(string ApprovalType, int OperationID, int SYKIID, int Division, int EvpAuth, int LoginCode)
        {
            return repo.ATOMAdd(ApprovalType, OperationID, SYKIID, Division, EvpAuth, LoginCode);
        }

        public LibResult ATOMSearch(string ApprovalType, int Operation, int Syki)
        {
            return repo.ATOMSearch(ApprovalType, Operation, Syki);
        }

        public bool ATOMDelete(int id)
        {
            return repo.ATOMDelete(id);
        }
        #endregion

        #region Capitalized Asset Transfer

        public bool CapitalizedEditDelete(int id)
        {
            return repo.CapitalizedEditDelete(id);
        }
        public List<Employee_Details> AutocompleteSuggestions(string term, string department)
        {
            return repo.AutocompleteSuggestions(term, department);
        }

        public LibResult AutocomplitTranNo()
        {
            return repo.AutocomplitTranNo();
        }

        public LibResult BindApprovalType(int LoginCode)
        {
            return repo.BindApprovalType(LoginCode);
        }

        public LibResult Requestor_Detail(string AssetType,string Transferor, int LoginCode)
        {
            return repo.Requestor_Detail(AssetType,Transferor, LoginCode);
        }

        public LibResult GetTransferor(string AssetType, int LoginCode)
        {
            return repo.GetTransferor(AssetType, LoginCode);
        }

        public LibResult GetDepartment()
        {
            return repo.GetDepartment();
        }

        public LibResult Maxsrno()
        {
            return repo.Maxsrno();
        }
        public LibResult SaveTransaction(AT_ASSSET_TRANSFER_HEADER hd, List<AT_ASSET_TRANSFER_DETAIL> dt, List<AT_APPROVAL_AUTHORITY> _at, string flag, string AssetType, int LoginCode)
        {
            return repo.SaveTransaction(hd, dt, _at, flag, AssetType, LoginCode);
        }

        public Employee_Details GetAuthEmpById(int empCode, Employee_Details empDtl, int TranNo, string Department)
        {
            return repo.GetAuthEmpById(empCode, empDtl, TranNo, Department);
        }

        public List<AT_AssetTransferHeaderViewModel> GetCapitalizeList(int LoginCode, string flag)
        {
            return repo.GetCapitalizeList(LoginCode, flag);
        }
        public List<AT_APPROVAL_AUTHORITY_LOG> History(int LoginCode, int TranNo)
        {
            return repo.History(LoginCode, TranNo);
        }

        public List<AT_AssetTransferHeaderViewModel> CapitalizeListSearch(string TransactionType,/* string AssetType,*/ int TransactionNo, string TransfereeUser, string Status, int LoginCode, string flag)
        {
            return repo.CapitalizeListSearch(TransactionType, /*AssetType,*/ TransactionNo, TransfereeUser, Status, LoginCode, flag);
        }

        public List<AT_AssetTransferHeaderViewModel> CapitalizeStatusSearch(string Status, int LoginCode, string flag)
        {
            return repo.CapitalizeStatusSearch(Status, LoginCode, flag);
        }

        public LibResult CapitalizeListDelete(int id, string flag)
        {
            return repo.CapitalizeListDelete(id, flag);
        }

      

        public AT_ASSSET_TRANSFER_HEADER CapitalizedEdit(int id)
        {
            return repo.CapitalizedEdit(id);
        }



        public LibResult CapitalizedDetail(int id, int LoginCode, int APPAUTHSRNO)
        {
            return repo.CapitalizedDetail(id, LoginCode, APPAUTHSRNO);
        }

        public List<AT_AssetTransferAuthorityViewModel> CapitalizedAuthority(int id)
        {
            return repo.CapitalizedAuthority(id);
        }

        public LibResult EditTransaction(AT_ASSSET_TRANSFER_HEADER hd, List<AT_ASSET_TRANSFER_DETAIL> dt, List<AT_APPROVAL_AUTHORITY> _at, string flag, int LoginCode,string AssetType)
        {
            return repo.EditTransaction(hd, dt, _at, flag, LoginCode, AssetType);
        }
        #endregion

        #region
        public AT_AssetTransferHeaderViewModel CapitalizedAssetTransferEdit(int id)
        {
            return repo.CapitalizedAssetTransferEdit(id);
        }

        public short UpdateApproval(string Remarks, int LoginCode, int TranNo, string Status, List<AT_ASSET_TRANSFER_DETAIL> dt)
        {
            return repo.UpdateApproval(Remarks, LoginCode, TranNo, Status, dt);
        }

        public List<AT_AssetTransferApprovalViewModel> ApprovalListSearchData(string TranjectionType, string AssetType, int TranjectionNo, string Status, int LoginCode, string flag)
        {
            return repo.ApprovalListSearchData(TranjectionType, AssetType, TranjectionNo, Status, LoginCode, flag);
        }

        public List<AT_AssetTransferApprovalViewModel> ApprovalListSearchDataStatus(string Status, int LoginCode, string flag)
        {
            return repo.ApprovalListSearchDataStatus(Status, LoginCode, flag);
        }
        public List<AT_AssetTransferApprovalViewModel> GetApprovalList(int LoginCode)
        {
            return repo.GetApprovalList(LoginCode);
        }

        public List<AT_AssetTransferApprovalViewModel> SecurityGetApprovalList(int LoginCode)
        {
            return repo.SecurityGetApprovalList(LoginCode);
        }

        public List<AT_AssetTransferApprovalViewModel> GetTaxationApprovalList(int LoginCode)
        {
            return repo.GetTaxationApprovalList(LoginCode);
        }
        public List<AT_AssetTransferApprovalViewModel> GetFinanceApprovalList(int LoginCode)
        {
            return repo.GetFinanceApprovalList(LoginCode);
        }

        public List<AT_AssetTransferApprovalViewModel> GetFinanceApproveList(int LoginCode)
        {
            return repo.GetFinanceApproveList(LoginCode);
        }

        public List<AT_AssetTransferApprovalViewModel> GetFinalFinanceApprovalList(int LoginCode, string flag)
        {
            return repo.GetFinalFinanceApprovalList(LoginCode, flag);
        }
        public List<AT_AssetTransferAuthorityViewModel> Gethead(short LoginCode, int TranNo, string TranType, int Transfree, string ApprovalType, long? operationMappId)
        {
            return repo.Gethead(LoginCode, TranNo, TranType, Transfree, ApprovalType, operationMappId);
        }
        #endregion

        #region Taxation Approval
        public AT_ASSSET_TRANSFER_HEADER Get_AT_ASSSET_TRANSFER_HEADER_ModelData(int id)
        {
            return repo.Get_AT_ASSSET_TRANSFER_HEADER_ModelData(id);
        }

        public short UpdateFinanceApproval(string Remarks, int LoginCode, int TranNo, string Status)
        {
            return repo.UpdateFinanceApproval(Remarks, LoginCode, TranNo, Status);
        }

       

        public LibResult UploadFile(int TranNo, string filepath, long EmpCode)
        {
            return repo.UploadFile(TranNo, filepath, EmpCode);
        }

        public LibResult GetinstampUpload(int TranNo, string filepath, long EmpCode)
        {
            return repo.GetinstampUpload(TranNo, filepath, EmpCode);
        }

        public List<object> GetAppAuthLogHistory(int TranNo)
        {
            return repo.GetAppAuthLogHistory(TranNo);
        }

        public LibResult GetNewLocation(string Key, string TransfreeType)
        {
            return repo.GetNewLocation(Key, TransfreeType);
        }
        #endregion

        #region CommanApprovalMaster

        public LibResult AutocomplitName(int Ecode)
        {
            return repo.AutocomplitName(Ecode);
        }

        public AT_COMMON_APPROVAL_MASTER AT_CommanApprovalEdit(int id)
        {
            return repo.AT_CommanApprovalEdit(id);
        }

        public List<AT_COMMON_APPROVAL_MASTER> GetCommanApprovalList()
        {
            return repo.GetCommanApprovalList();
        }

        public LibResult BindPlantName()
        {
            return repo.BindPlantName();
        }

        public bool CommanApprovalDelete(int id)
        {
            return repo.CommanApprovalDelete(id);
        }
        public LibResult GetCommanApprovalSave(int srno, int Sysiteid, string Department, int ECode, decimal Sequenceno, int LoginCode, string Mode)
        {
            return repo.GetCommanApprovalSave(srno, Sysiteid, Department, ECode, Sequenceno, LoginCode, Mode);
        }

        public LibResult GetCommanApprovalEdit(int srno, decimal Sysiteid, string Department, int ECode, decimal Sequenceno, int LoginCode)
        {
            return repo.GetCommanApprovalEdit(srno, Sysiteid, Department, ECode, Sequenceno, LoginCode);
        }

        public LibResult ATOMUpdate(int srno, string ApproveType, int Operation, int SYKIID, int Division, int EvpAuth ,int LoginCode)
        {
            return repo.ATOMUpdate(srno, ApproveType, Operation, SYKIID, Division, EvpAuth, LoginCode);
        }


        public List<AT_COMMON_APPROVAL_MASTER> GetCommanApprovalSearch(string Department)
        {
            return repo.GetCommanApprovalSearch(Department);
        }
        #endregion


        #region Asset Attachment

        public bool UploadFileAndSave(int TranNo, string AssetCode, string file)
        {
            return repo.UploadFileAndSave(TranNo, AssetCode, file);
        }

        public bool UploadFileAndSaveCap(int TranNo, string AssetCode, string file)
        {
            return repo.UploadFileAndSaveCap(TranNo, AssetCode, file);
        }


        #endregion 
        #endregion

        #region non capitalize asset
        public AT_ASSSET_TRANSFER_HEADER NonCapitalizedEdit(int id)
        {
            return repo.NonCapitalizedEdit(id);
        }

        #endregion

        #region AssetTransfer Form Dowmload
        public GetAssetFormHeader Get_AssetRegister_Header_Detail(long? UserID, long Tran_No, string filename)
        {
            return repo.Get_AssetRegister_Header_Detail(UserID, Tran_No, filename);

        }

        public bool filesave(string filename, long Tran_No)
        {
            return repo.filesave(filename, Tran_No);

        }

        public List<AT_ASSET_TRANSFER_DETAIL> DetailRecord(long? UserID, long Tran_No)
        {
            return repo.DetailRecord(UserID, Tran_No);
        }

        public GetAuthDetail GetAuthorityDetail(long? UserID, long Tran_No)
        {
            return repo.GetAuthorityDetail(UserID, Tran_No);

        } 
        #endregion

        #region  MISReport
        public List<AT_AssetTransferMISReportViewModel> MISReportSearch(string TransectionType, string AssetType, DateTime FromDate, DateTime ToDate, int LoginCode, string Plant)
        {
            return repo.MISReportSearch(TransectionType, AssetType, FromDate, ToDate, LoginCode, Plant);
        }

        public LibResult GetPlanForMISReport()
        {
            return repo.GetPlanForMISReport();
        }
        #endregion


        #region Manage Asset Transfer By Admin
        public List<AT_AssetTransferHeaderViewModel> ManageAssetTransfer(int LoginCode, string flag)
        {
            return repo.ManageAssetTransfer(LoginCode, flag);
        }

        public AT_APPROVAL_AUTHORITY AuthEdit(int id)
        {
            return repo.AuthEdit(id);
        }

        public LibResult AuthoritySave(int Empcode, string EmpName,int srno, string Department, int LoginCode)
        {
            return repo.AuthoritySave(Empcode, EmpName, srno, Department,LoginCode);
        }

        public long? GetOperationMappId(Employee_Details _Employee_Details)
        {
            List<long?> _orgList = new List<long?>();
            if (_Employee_Details._SecId.HasValue)
                _orgList.Add(_Employee_Details._SecId);
            if (_Employee_Details._DepId.HasValue)
                _orgList.Add(_Employee_Details._DepId);
            if (_Employee_Details._DivId.HasValue)
                _orgList.Add(_Employee_Details._DivId);
            if (_Employee_Details._OpId.HasValue)
                _orgList.Add(_Employee_Details._OpId);
            return repo.GetOperationMappId(_orgList);
        }

        public LibResult ManageRequestor_Detail(string AssetType, string Transferor, int AddedBy, int LoginCode)
        {
            return repo.ManageRequestor_Detail(AssetType, Transferor, AddedBy, LoginCode);
        }
        #endregion

        //SR109866-CR7276

        #region Capitalized Asset transfer Report
        public List<AT_AssetTransferHeaderViewModel> GetCapitalizeAssetTransferReportList(int LoginCode, string flag)
        {
            return repo.GetCapitalizeAssetTransferReportList(LoginCode, flag);
        }

        public List<AT_AssetTransferHeaderViewModel> CapitalizeAssetTransferReportListSearch(string TransactionType,/* string AssetType,*/ int TransactionNo, string TransfereeUser, string Status, int LoginCode, string flag)
        {
            return repo.CapitalizeAssetTransferReportListSearch(TransactionType, /*AssetType,*/ TransactionNo, TransfereeUser, Status, LoginCode, flag);
        }

        public List<AT_AssetTransferHeaderViewModel> CapitalizeAssetTransferReportStatusSearch(string Status, int LoginCode, string flag)
        {
            return repo.CapitalizeAssetTransferReportStatusSearch(Status, LoginCode, flag);
        }

        #endregion
        //SR109866-CR7276

    }
}
