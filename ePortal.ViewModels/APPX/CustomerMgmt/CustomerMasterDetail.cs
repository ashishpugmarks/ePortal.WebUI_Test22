using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ePortal.ViewModels.APPX.CustomerMgmt
{
    public class CustomerMasterDetail
    {
        public long? CMDETAILID { get; set; }
        public long? CMDETAILID1 { get; set; }
        public long? CMHEADERID { get; set; }
        public string? CM_REQ_TYPE { get; set; } //Customer request type create/extend/update
        public string? COMPANY_CODE { get; set; }
        public string? CUST_ACC_TYPE { get; set; }
        public string? CUST_ACC_TYPE1 { get; set; }
        public string? SALES_ORG { get; set; }
        public string? DIVISION_GRP { get; set; }
        public string? DISTRIBTUION_CHH { get; set; }
        public string? REQUEST_TYPE { get; set; }
        public string? CUSTOMER_CODE { get; set; }
        public string? TITLE { get; set; }
        public string? NAME1 { get; set; }
        public string? NAME2 { get; set; }
        public string? NAME3 { get; set; }
        public string? NAME4 { get; set; }
        public string? SEARCHTERM { get; set; }
        public string? STREETHOUSENUMBER { get; set; }
        public string? STREET2 { get; set; }
        public string? STREET3 { get; set; }
        public string? STREET4 { get; set; }
        public string? STREET5 { get; set; }
        public string? POSTALCODE { get; set; }
        public string? CITY { get; set; }
        public string? COUNTRY { get; set; }
        public string? CREGION { get; set; }
        public string? CTIMEZONE { get; set; }
        public string? TRANSPORTATIONCODE { get; set; }
        public string? MOBILEPHONE { get; set; }
        public string? FAX { get; set; }
        public string? EMAIL { get; set; }
        public string? INDUSTRY { get; set; }
        public string? TAXNUMBER3 { get; set; }
        public string? VENDORNO { get; set; }
        public string? CITYCODE { get; set; }
        public string? CTRY { get; set; }
        public string? BANKKEY { get; set; }
        public string? BANKACCOUNT { get; set; }
        public string? ACCOUNTHOLDER { get; set; }
        public string? BANKCONTROLKEY { get; set; }
        public string? BANKNAME { get; set; }
        public string? BANKREGION { get; set; }
        public string? BANKSTREET { get; set; }
        public string? BANKCITY { get; set; }
        public string? BANKBRANCH { get; set; }
        public string? CUSTOMERCLASS { get; set; }
        public string? INDUSTRY1 { get; set; }
        public string? INDUSTRYCODE1 { get; set; }
        public string? INDUSTRYNAME { get; set; }
        public string? FIRSTNAME { get; set; }
        public string? RECONSACCOUNT { get; set; }
        public string? TERMOFPAYMENT { get; set; }
        public string? BANKCURRENCY { get; set; }
        public string? SALESCURRENCY { get; set; }
        public string? PAYMENTMETHODS { get; set; }
        public string? HOUSEBANK { get; set; }
        public string? PMTMETHSUPL { get; set; }
        public string? WITHHOLDINGTAXTYPE { get; set; }
        public string? WITHHOLDINGTAXCODE { get; set; }
        public string? VALIDFROM { get; set; }
        public string? VALIDTO { get; set; }
        public string? WITHHOLDINGTAXNUMBER { get; set; }
        public string? INCOTERM1 { get; set; }
        public string? INCOTERM2 { get; set; }
        public string? CREDITCONTROLAREA { get; set; }
        public string? ACCASSIGMENTGROUP { get; set; }
        public string? TAXCLASSIFICATION { get; set; }
        public string? EXCHANGERATETYPE { get; set; }
        public string? SALESDISTRICT { get; set; }
        public string? SALESOFFICE { get; set; }
        public string? SALESGROUP { get; set; }
        public string? CUSTOMERGROUP { get; set; }
        public string? PRICEGROUP { get; set; }
        public string? CUSTPRICPROC1 { get; set; }
        public string? DELIVERYPRIORITYGROUP { get; set; }
        public string? SHIPPINGCONDITION { get; set; }
        public string? CSTNO { get; set; }
        public string? LSTNO { get; set; }
        public string? INVOICINGDATES { get; set; }
        public string? INVOICINGLISTDATES { get; set; }
        public string? SERREGNO { get; set; }
        public string? PANNUMBER { get; set; }
        public string? PAYMENTGUARANTEEPROC { get; set; }
        public string? E_INVOICE_APPLICABLE { get; set; }
        public long? REQUESTEDID { get; set; }
        public string? REQUESTED_DATE { get; set; }
        public long? UPDATEDBY { get; set; }
        public DateTime? UPDATEDDAE { get; set; }
        public short ISACTIVE { get; set; }
        public string STATUS { get; set; }
        public string? SAP_REMARKS { get; set; }
        public string? CUST_DEALER_CODE { get; set; }
        public string? REQUESTED_EMAILID { get; set; }
        public string? FIN_EMAILID { get; set; }
        public string? BANK_MANDATE_FILE { get; set; }
        public string? BANK_CANCELED_FILE { get; set; }
        public string? CIN_EINVOICE_DOC_FILE { get; set; }
        public string? CIN_PAN_NO_FILE { get; set; }
        public string? CIN_GST_NO_FILE { get; set; }
        public string? COMP_RTO_FILE { get; set; }
        public string? COMP_LOI_FILE { get; set; }
        public string? GEN_REQUEST_NO { get; set; }
        public string? REMARKS { get; set; }
        public string? OTHER_DOC_FILE { get; set; }
        public IFormFile? FILE1 { get; set; }
        public IFormFile? FILE2 { get; set; }
        public IFormFile? FILE3 { get; set; }
        public CustomerRequesterDetail? REQUESTERDETAILS { get; set; }

    }

    public class MasterDataBinding
    {
        public string CODE { get; set; }
        public string CODE_DESC { get; set; }
    }

    public class CustomerResponseData
    {
        public int? RS { get; set; }=0;
        public int? CMHEADERID { get; set; } = 0;
        public int? CMDEAILID1 { get; set; } = 0;
        public int? CMDEAILID2 { get; set; } = 0;
        public string? MESSAGE    { get; set; } = string.Empty;
        public string? VIEWSTATE { get; set; } = string.Empty;
        public string? FILE1 { get; set; } = string.Empty;
        public string? FILE2 { get; set; } = string.Empty;
        public string? FILE3 { get; set; } = string.Empty;
        public string? FILE4 {  get; set; } = string.Empty;
    }   
    
    public class FinalSubmit
    {
        public long? USERID { get; set; } = 0;
       public long? cmhid { get; set; }=0;
        public long? SEC_HEAD_ID { get; set; } = 0;
       public long? DEPT_DIV_ID { get; set; } = 0;
       public string? REMARKS { get; set; }=string.Empty;
       public string? hidGenDetailID { get;set; }=string.Empty;
       public string? DealerCode { get; set; } = string.Empty;
       public string? Name1 { get; set; } = string.Empty;
    }

    public class CustomerRequesterDetail
    {
        public string ? requesterid {  get; set; } = string.Empty;
        public string? ENAME { get; set; } = string.Empty;
        public string? EMAILID { get; set; } = string.Empty;
        public string? TMOBILE { get; set; } = string.Empty;

    }

}
