using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.CustomerMgmt
{
    public class CustomerSyncInput
    {
       public string? REQUESTTYPE {  get; set; }
        public string? CUSTACCGROUP { get; set; }
        public string? REQDATEFROM { get; set; }
        public string? REQDATETO { get; set; }
        public string? CUSTCODE { get; set; }
        public string? STATUS { get; set; }
        public string? UserId { get; set; }
        public string? cmheaderid { get; set; }
    }
    public class sapWt
    {
        //CUSTOMER_CODE,SPART,WITHHOLDINGTAXTYPE,WITHHOLDINGTAXCODE,VALIDFROM,VALIDTO,WITHHOLDINGTAXNUMBER
        public string? CUSTOMER_CODE { get; set; }
        public string? SPART { get; set; }
        public string? WITHHOLDINGTAXTYPE { get; set; }
        public string? WITHHOLDINGTAXCODE { get; set; }
        public string? VALIDFROM { get; set; }
        public string? VALIDTO { get; set; }
        public string? WITHHOLDINGTAXNUMBER { get; set; }      

    }
    public class sapCust    {
            public string? CUSTOMER_REF_NO { get; set; }
            public int CMHEADERID { get; set; }
            public int REQUESTERID { get; set; }          
            public string? REQUESTDATE { get; set; }
            public string? LASTMODIFIEDBY { get; set; }           
            public string? LASTMODIFIEDDATE { get; set; }
            public int? SYKIID { get; set; }
            public string? INDICATOR { get; set; }
            public string? REQUEST_TYPE { get; set; }
            public string? SECTION_APPROVER { get; set; }
            public string? DEPT_APPROVER { get; set; }
            public string? FINANCE_APPROVER { get; set; }
            public string? FINANCE1_APPROVER { get; set; }
            public string? SECTION_REMARKS { get; set; }
            public string? SECTION_APPROVE_DATE { get; set; }
            public short? ISSECTION_APPROVED { get; set; }
            public string? DEPT_REMARKS { get; set; }
            public DateTime? DEPT_APPROVE_DATE { get; set; }
            public short? ISDEPT_APPROVED { get; set; }
            public string? FINANCE_REMARKS { get; set; }
            public string? FINANCE_APPROVE_DATE { get; set; }
            public short? ISFINANCE_APPROVED { get; set; }
            public string? FINANCE1_REMARKS { get; set; }
            public string? FINANCE1_APPROVE_DATE { get; set; }
            public short? ISFINANCE1_APPROVED { get; set; }

            public string? HEADER_STATUS { get; set; }
            public string? UPDATED_DATE_IN_SAP { get; set; }           
            public string? EMAILID { get; set; }
            public short? ISACTIVE { get; set; }
            public string? GEN_REQUEST_NO { get; set; }
            public string? REQUEST_TYPE1 { get; set; } 
            public int? CMDETAILID { get; set; }
            public string? CUST_ACC_TYPE { get; set; }
            public string? SALES_ORG { get; set; }
            public string? COMPANY_CODE { get; set; }
            public string? DIVISION_GRP { get; set; }
            public string? DISTRIBTUION_CHH { get; set; }
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
            public string? REGION { get; set; }
            public string? TIMEZONE { get; set; }
            public string? TRANSPORTATIONCODE { get; set; }
            public string? MOBILEPHONE { get; set; }
            public string? FAX { get; set; }
            public string? EMAIL { get; set; }
            public string? INDUSTRY { get; set; }
            public string? TAXNUMBER3 { get; set; }
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
            public string? CURRENCY { get; set; }
            public string? PAYMENTMETHODS { get; set; }
            public string? HOUSEBANK { get; set; }
            public string? PMTMETHSUPL { get; set; }
            public string? WITHHOLDINGTAXTYPE { get; set; }
            public string? WITHHOLDINGTAXCODE { get; set; }
            public DateTime? VALIDFROM { get; set; }
            public DateTime? VALIDTO { get; set; }
            public string? WITHHOLDINGTAXNUMBER { get; set; }
            public string? INCOTERM { get; set; }
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
            public string? REQUESTEDID { get; set; }
            public string? REQUESTED_DATE { get; set; }
            public string? UPDATEDBY { get; set; }
            public DateTime? UPDATEDDAE { get; set; }
            public bool? DETAIL_ISACTIVE { get; set; }
            public string? REQ_NUM { get; set; }
            public string? VENDORNO { get; set; }
            public string? DETAIL_STATUS { get; set; }
            public string? SAP_REMARKS { get; set; }
            public string? CUST_DEALER_CODE { get; set; }
            public string? REQUESTEDEMAILID { get; set; }
            public string? FINEMAILID { get; set; }
            public string? BANK_MANDATE_FILE { get; set; }
            public string? BANK_CANCELED_FILE { get; set; }
            public string? CIN_PAN_NO_FILE { get; set; }
            public string? CIN_GST_NO_FILE { get; set; }
            public string? COMP_RTO_FILE { get; set; }
            public string? COMP_LOI_FILE { get; set; }
            
        }

    }
