using ePortal.ViewModels;
using ePortal.ViewModels.APPX.CustomerMgmt;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.APPX.Contracts
{
    public interface ICustomerMgmtService
    {
        public CustomerMgmtResponse AddCustomerMasterData(CMMASTER_DATA data, string Actiontype);
        public DataTable FetchCodesByGroupName(string groupName);
        public List<CustomerReqApproverDetails> GetCustomerApprovalDetails(string customerId);
        public List<CMApprovalAuthority> GetFinanceApproverData();
        public List<SwitchApprovalAuthority> GeSwitchApproverData(int userid);
        public string GeApproverName(string userid);
        public string SaveFinanceApproverData(CMApproverMatrx data, ref int res);
        public List<CustomerMasterDataMng> ShowFieldMaster();
        public List<CustomerMasterDataMng> SaveMandatoryData(CustomerMasterDataMng data,ref string res);
        public List<CustomerMasterDataMng> AddMandatoryData(CustomerMasterDataMng data, ref string res);
        public List<CustomerRequestRpt> ShowCustomerRequestData(CustomerRequestInput _data, ref string res);
        public DataSet BindMasterData(string groupname);
        public DataSet GetSectionHead(int _empCode);
        public DataSet BindTransportGRP(string RequestType, string Country);
        public DataTable SetMandatoryField(CustAccountFlags data);
        public DataTable GetDetailDraft(string UserId, string RequestNo, ref string err);
        public DataTable GetCustomerMasterDataWithPGRP(string Group_Name, string PGroup_Name);
        public CustomerResponseData SaveGeneral(CustomerMasterDetail data, string CIN_GST_NO_FILE_IN, string email_id);
        public CustomerResponseData SaveBankData(CustomerMasterDetail data, string BANK_MANDATE_FILE_IN, string BANK_CANCELED_FILE_IN, string email_id);
        public CustomerResponseData SaveIndustryData(CustomerMasterDetail data, string email_id);
        public CustomerResponseData SaveSaleData(CustomerMasterDetail data, string email_id);
        public CustomerResponseData SaveCINData(CustomerMasterDetail data, string CIN_PAN_NO_FILE_IN, string CIN_GST_NO_FILE_IN, string CIN_CIN_DOC_FILE_IN, string email_id);
        public CustomerResponseData SaveCompanyData(CustomerMasterDetail data, string COMP_RTO_FILE_IN, string COMP_LOI_FILE_IN, string OTHER_DOC_FILE_IN, string email_id);
        public CustomerResponseData SubmitCutomerRequest(FinalSubmit data);
        public CustomerResponseData ResetCMRequest(long USERID, string RequestType);
        public CustomerMasterDetail GetPreviewData(string request, long userid);
        public CustomerMasterDetail GetCustomerRequestDetails(string RequestNo, string userid);
        public CMRequestHistoryDetail GetApprovalHistory(string RequestDetailID, string userId);
        public string btnExcelExport(List<CustomerRequestRpt> dt);
        public List<CMRequestModel> BindMMCreationRequestDetails(long userid);
        public ApprovalResponse ApprovalRequestSubmit(ApprovalReqest data);
        public List<CustomerRequestRpt> ShowCustomerSyncData(CustomerSyncInput _data);
        public Task<CustomerResponseData> SyncCustomerRequest(string cmheaderid, string custAccType, string UserId);
    }
}
