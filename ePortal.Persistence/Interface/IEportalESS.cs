using ePortal.DomainClasses;
using ePortal.ViewModels;
using ePortal.ViewModels.APPX.CustomerMgmt;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Interface
{
    public interface IEportalESS
    {
        Task<DataTable> GetAssetCodeData(string AssetCode);
        Task<DataTable> GetAssetsDetails(string fncode, string profitCenter);
        Task<DataTable> GetAucCodeData(string AucCode);
        Task<DataTable> GetFamilylist(string userid, string subtype);
        Task<DataTable> GetOverstayReport(string userid, string strdate, string strcheck);
        Task<string> Local_Conveyance_PostDataToSAP(string EMP_ID, string DOCDATE, string REF_DOC_NO, string POSTINGDATE, string ConvAmount, string MealAmout, string FromDate, string Todate);
        Task<DataTable> GetCustomerDetails();
        Task<string> PostInvoiceInSAP(List<NSP_PaymentDetails> DataList, DateTime? DOCDATE_);
        Task<string> ReversePostedInvoiceInSAP(string DocNo, string FiscalYear, string Reason, string PostingDate);
        public Task<DataSet> GetPOGetails(string POnumber);

        // 
        Task<DataTable> GetEmployeeACStatement(string userid, string subtype, string monthfrom, string monthto, string finyear);
        Task<DataTable> GetPayrollResultList(string userid, string fromdate, string todate);
        Task<DataTable> GetHLISStatement(string CompCode, string EmpCode, string FinTranNo, string PostingPeriod);
        Task<DataTable> GetFullName(string userid);
        Task<DataTable> GetAddressDetail(string userid, string AddressType);
        Task<DataTable> Gettaxworkingtax(string userid, string strdate);
        Task<DataTable> GetSalaryBreakup(string userid, string act_type, string strfstartdate, string strfenddate);
        Task<DataSet> GetSalaryBreakup2(string userid, string act_type, string strfstartdate, string strfenddate);
        Task<DataTable> GetPaySlip(string userid, string strsqano, string strpayv);
        Task<string> GetEmployeeUnblockDTL(string EmpCode, string Status1);
        Task<string> GetEmployeeBlockDTL(string EmpCode, string Status1);
        //Customer Master 
        public Task<DataSet> GET_VENDOR_MASTER(string VENDORNO, string COMPANYCODE);
        public Task<DataSet> GET_CUSTOMER_MASTER(string CUSTOMERNO, string COMPANYCODE);
        public Task<DataTable> GET_DIVSION_List(string CUSTOMERNO);
        public Task<DataTable> GET_Bank_Verification(string BANK_CTRYCODE, string BANKKEY);
        public Task<DataTable> POST_CUSTOMER_MASTER(List<sapCust> _sapCust, List<sapWt> _sapWt);
    }
}
