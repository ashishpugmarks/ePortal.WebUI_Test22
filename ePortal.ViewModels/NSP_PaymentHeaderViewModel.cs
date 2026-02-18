using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class NSP_PaymentHeaderViewModel
    {
        public int HeaderID { get; set; }
        public Nullable<int> RequestRefNo { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public Nullable<System.DateTime> RequestDate { get; set; }
        public string Category { get; set; }
        public string CostCentre { get; set; }
        public string Budgeted { get; set; }
        public string PurposeofExpenses { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string VendorPANNo { get; set; }

        public string VendorAddress { get; set; }
        public string BankName { get; set; }
        public string BankAccountNo { get; set; }
        public string BankIFSCCode { get; set; }
        public string FY_RequestType { get; set; }
        public Nullable<decimal> FY_ExRate { get; set; }
        public Nullable<System.DateTime> FY_ExRateDate { get; set; }
        public string FY_Taxation { get; set; }
        public string FY_GSTType { get; set; }
        public string FY_GST { get; set; }
        public Nullable<decimal> FY_SGSTRATE { get; set; }
        public string FY_TDS { get; set; }
        public Nullable<decimal> FY_TDSRate { get; set; }
        public string FY_TDSREASON { get; set; }
        public int ADDEDBY { get; set; }
        public DateTime ADDEDON { get; set; }
        public int UPDATEDBY { get; set; }
        public DateTime UPDATEDON { get; set; }
        public Nullable<int> STATUS { get; set; }
        public string Location { get; set; }
        public string TotalAmount { get; set; }
        public Nullable<decimal> TotalNetAmount { get; set; }
        public string TotalNetAmountInWord { get; set; }
        public Nullable<decimal> CGSTAMOUNT { get; set; }
        public string D_Attachment1 { get; set; }
        public short? ApprovalStatus { get; set; }

       
        public string response { get; set; }
        public string ApprovalRemarks { get; set; }
        public List<NSP_ApprovalAuthority> appAuthority { get; set; }
        public List<NSP_ApprovalAuthority> MultiRoleAuthority { get; set; }
        public List<NSP_PaymentDetailsViewModel> appInvoiceDetails { get; set; }
        public string CurrencyType { get; set; }
        public string RequestType { get; set; }
        public Nullable<decimal> FY_CGSTRATE { get; set; }
        public string FinanceRemark { get; set; }
        public string TAXATIONREMARK { get; set; }

        public string TDSUnderSectionCode { get; set; }
        public string GSTIN { get; set; }
        public Nullable<decimal> TDSBaseAmount { get; set; }

        public Nullable<decimal> FY_TDS_AMOUNT { get; set; }
        public Nullable<decimal> FY_IGSTRATE { get; set; }
        public Nullable<decimal> SGSTAMOUNT { get; set; }
        public Nullable<decimal> IGSTAMOUNT { get; set; }
        public Nullable<decimal> TOTAL_GST_AMOUNT { get; set; }

        public string FY_GSTREASON { get; set; }

        public Nullable<DateTime> POSTING_DATE { get; set; }
        public string SAP_DOCNO { get; set; }
        public string BUDGETED_ATTACH { get; set; }

        public string PAY_PRO_LOCATION { get; set; }
        public Nullable<int> PAY_PRO_LOCATIONID { get; set; }

        public Nullable<int> LOCATIONID { get; set; }

        public Nullable<decimal> FINAL_TOTAL { get; set; }
        public Nullable<decimal> APPROVED_FINAL_TOTAL { get; set; }

        public Nullable<int> ReqCode { get; set; }

        public string SAP_STATUS { get; set; }

       
    }

    public class NSP_PaymentDetailsViewModel
    {
        public int DetailID { get; set; }
        public Nullable<int> HeaderID { get; set; }
        public Nullable<DateTime> InvoiceDate { get; set; }
        public string InvoiceNo { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
        public Nullable<decimal> Amount { get; set; }

        public Nullable<decimal> APPROVED_AMOUNT { get; set; }
        public string PO_Document { get; set; }
        public string Invoice_Document { get; set; }
        public string ApprovalNote_Document { get; set; }
        public string Other_Document { get; set; }
        public int ADDEDBY { get; set; }
        public DateTime ADDEDON { get; set; }
        public int UPDATEDBY { get; set; }
        public DateTime UPDATEDON { get; set; }
        public string CURRENCYTYPE { get; set; }

        public Nullable<decimal> BaseAmount { get; set; }
        public string TaxType { get; set; }
        public string CostCenter { get; set; }
        public string GSTType { get; set; }
        public string GSTTaxCode { get; set; }
        public Nullable<decimal> CGSTRate { get; set; }
        public Nullable<decimal> CGSTAmount { get; set; }
        public Nullable<decimal> SGSTRate { get; set; }
        public Nullable<decimal> SGSTAmount { get; set; }
        public Nullable<decimal> IGSTRate { get; set; }
        public Nullable<decimal> IGSTAmount { get; set; }
        public Nullable<decimal> TotalGSTAmount { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public string NilGSTReason { get; set; }
        public string NilTDSReason { get; set; }
        public Nullable<decimal> TDSBaseAmount { get; set; }
        public string TDS { get; set; }

        public string TDSType { get; set; }
        public Nullable<decimal> TDSRate { get; set; }
        public Nullable<decimal> TDSAmount { get; set; }
        public Nullable<decimal> TotalNetAmount { get; set; }

        public string ISPOSTED { get; set; }
        public string ISREVERSEPOSTED { get; set; }
        public string POSTDOCNO { get; set; }
        public Nullable<System.DateTime> POSTDOCDATE { get; set; }
        public string REVERSEPOSTDOCNO { get; set; }
        public Nullable<System.DateTime> REVERSEPOSTDOCDATE { get; set; }
        public string POSTREVERSEMESSAGE { get; set; }

        public string SAP_STATUS { get; set; }

        public bool PostingDtFound { get; set; }
        public string VENDORCODE { get; set; }
        public string VENDORNAME { get; set; }
        public string CATEGORY { get; set; }

        public string Split { get; set; }

        public int RowSrNo { get; set; }


        public string COMMISSION { get; set; }
        public Nullable<decimal> COMMISSIONAMOUNT { get; set; }

        public Nullable<decimal> CESSRATE { get; set; }
        public Nullable<decimal> CESSAMOUNT { get; set; }

        public string GSTIN { get; set; }

        public Nullable<int> RequestRefNo { get; set; }

        public string IS_ESI { get; set; }
        public Nullable<decimal> ESI_RATE { get; set; }
        public Nullable<decimal> ESI_AMOUNT { get; set; }
        public Nullable<decimal> DISCOUNT_AMOUNT { get; set; }

    }
    public class NSP_ApprovalAuthority
    {
        public int APPROVALID { get; set; }
        public Nullable<int> HEADERID { get; set; }
        public Nullable<int> APPSEQ { get; set; }
        public Nullable<int> EMPCODE { get; set; }
        public string APPROVALTYPE { get; set; }
        public string DESIGNATION { get; set; }
        public string EMPHEAD { get; set; }
        public Nullable<short> APPROVAL_STATUS { get; set; }
        public string EMP_NAME { get; set; }
        public string APPREMARK { get; set; }
        public int ADDEDBY { get; set; }
        public Nullable<DateTime> ADDEDON { get; set; }
        public int UPDATEDBY { get; set; }
        public Nullable<System.DateTime> UPDATEDON { get; set; }
        public short FNDESID { get; set; }
        public Nullable<int> APPLOGID { get; set; }

        public string Department { get; set; }

    }

    public class NSP_PaymentDetails
    {
        public string BusinessCenter { get; set; }
        public string BusinessPlace { get; set; }
        public string CostCenter { get; set; }
        public string ProfitCenter { get; set; }
        public string AllocNumber { get; set; }
        public string ItemText { get; set; }
        public string LIFNR { get; set; }
        public string GLAccount { get; set; }
        public string TaxCode { get; set; }
        public string ZType { get; set; }
        public decimal AmountDocumentCurrency { get; set; }
        public string WTType { get; set; }
        public string WTCode { get; set; }
        public string SPGLInd { get; set; }
        public string Currency { get; set; }
        public string DocType { get; set; }
        public string HeaderText { get; set; }
        public string RefDocNo { get; set; }
        public  string Mode { get; set; }
        public string PostingDocNo { get; set; }
        public string KUNNR { get; set; }
        public int SRNO { get; set; }

        public Nullable<decimal>  DISCOUNT_AMOUNT { get; set; }
        public Nullable<decimal> TDS_BASE_AMOUNT { get; set; }
        public Nullable<System.DateTime> DOCDATE { get; set; }
    }


    public class POSTReverseRequest
    {
        public int RequestNo { get; set; }
        public int DtId { get; set; }
        public string InvoiceNo { get; set; }
        public string DocType { get; set; }
        public string CostCenter { get; set; }
        public string BusinessCenter { get; set; }
        public string Mode { get; set; }
        public string DocNo { get; set; }
        public string FiscalYear { get; set; }
        public string Reason { get; set; }
        public List<NSP_PaymentDetails> PostingDt { get; set; }

        public string  PostingDate { get; set; }



    }

    public class Dealer
    {
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string DealerBank { get; set; }
        public string DealerAccCode { get; set; }
        public string Address { get; set; }
        public string PANNO { get; set; }
    }


}
