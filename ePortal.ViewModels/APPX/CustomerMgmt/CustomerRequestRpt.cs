using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ePortal.ViewModels.APPX.CustomerMgmt
{
    public class CustomerRequestRpt
    {
        [Column("CMHEADERID")]
        public long? CMHEADERID { get; set; }

        [Column("REQUESTERID")]
        public long? REQUESTERID { get; set; }

        [Column("REQUESTDATE")]
        public string? REQUESTDATE { get; set; }

        [Column("LASTMODIFIEDBY")]
        public int? LASTMODIFIEDBY { get; set; }

        [Column("LASTMODIFIEDDATE")]
        public string? LASTMODIFIEDDATE { get; set; }

        [Column("SYKIID")]
        public int? SYKIID { get; set; }

        [Column("REQUEST_TYPE")]
        public string? REQUEST_TYPE { get; set; }

        [Column("SECTION_APPROVER")]
        public int? SECTION_APPROVER { get; set; }

        [Column("DEPT_APPROVER")]
        public int? DEPT_APPROVER { get; set; }

        [Column("FINANCE_APPROVER")]
        public int? FINANCE_APPROVER { get; set; }

        [Column("FINANCE1_APPROVER")]
        public int? FINANCE1_APPROVER { get; set; }

        [Column("SECTION_REMARKS")]
        public string? SECTION_REMARKS { get; set; }

        [Column("SECTION_APPROVE_DATE")]
        public string? SECTION_APPROVE_DATE { get; set; }

        [Column("ISSECTION_APPROVED")]
        public int? ISSECTION_APPROVED { get; set; }

        [Column("DEPT_REMARKS")]
        public string? DEPT_REMARKS { get; set; }

        [Column("DEPT_APPROVE_DATE")]
        public string? DEPT_APPROVE_DATE { get; set; }

        [Column("ISDEPT_APPROVED")]
        public int? ISDEPT_APPROVED { get; set; }

        [Column("FINANCE_REMARKS")]
        public string? FINANCE_REMARKS { get; set; }

        [Column("FINANCE_APPROVE_DATE")]
        public string? FINANCE_APPROVE_DATE { get; set; }

        [Column("ISFINANCE_APPROVED")]
        public int? ISFINANCE_APPROVED { get; set; }

        [Column("FINANCE1_REMARKS")]
        public string? FINANCE1_REMARKS { get; set; }

        [Column("FINANCE1_APPROVE_DATE")]
        public string? FINANCE1_APPROVE_DATE { get; set; }

        [Column("ISFINANCE1_APPROVED")]
        public int? ISFINANCE1_APPROVED { get; set; }

        [Column("HEADER_STATUS")]
        public string? HEADER_STATUS { get; set; }

        [Column("UPDATED_DATE_IN_SAP")]
        public DateTime? UPDATED_DATE_IN_SAP { get; set; }

        [Column("EMAILID")]
        public string? EMAILID { get; set; }

        [Column("ISACTIVE")]
        public int? ISACTIVE { get; set; }

        [Column("GEN_REQUEST_NO")]
        public string? GEN_REQUEST_NO { get; set; }

        [Column("CUSTOMER_TYPE")]
        public string? CUSTOMER_TYPE { get; set; }

        [Column("CUST_ACC_TYPE")]
        public string? CUST_ACC_TYPE { get; set; }

        [Column("SALES_ORG")]
        public string? SALES_ORG { get; set; }

        [Column("DIVISION_GRP")]
        public string? DIVISION_GRP { get; set; }

        [Column("DISTRIBTUION_CHH")]
        public string? DISTRIBTUION_CHH { get; set; }

        [Column("CUSTOMER_CODE")]
        public string? CUSTOMER_CODE { get; set; }

        [Column("TITLE")]
        public string? TITLE { get; set; }

        [Column("NAME1")]
        public string? NAME1 { get; set; }

        [Column("REQUESTEDID")]
        public long? REQUESTEDID { get; set; }

        [Column("UPDATEDBY")]
        public int? UPDATEDBY { get; set; }

        [Column("DETAIL_ISACTIVE")]
        public int? DETAIL_ISACTIVE { get; set; }

        [Column("DETAIL_STATUS")]
        public string? DETAIL_STATUS { get; set; }
        public string? Status { get; set; }
        public string? ENAME { get; set; }
        public string? Sap_Remarks { get; set; }
        public string? USERTYPE { get; set; }

    }
    public class CustomerRequestInput
    {
        public string? REQUESTTYPE { get; set; }
        public string? CUSTACCGROUP { get; set; }
        public string? REQDATEFROM { get; set; }
        public string? REQDATETO { get; set; }
        public string? CUSTCODE { get; set; }
        public string? STATUS { get; set; }
        public string? CUSTOMERNAME { get; set; }
        public string? REQUEST_NO { get; set; }
        public string? USERTYPE { get; set; } = "U";
        public string? USERID { get; set; } 
    }
    public class CustAccountFlags
    {
        public string Ag5AG1 { get; set; } = string.Empty;
        public string Ag5AG2 { get; set; } = string.Empty;
        public string Ag5AG3 { get; set; } = string.Empty;
        public string Ag5AG5 { get; set; } = string.Empty;
        public string Ag5AG6 { get; set; } = string.Empty;
        public string Ag5AGG { get; set; } = string.Empty;
        public string Ag5AGV { get; set; } = string.Empty;
        public string Ag5AGW { get; set; } = string.Empty;
    }
    public class CMRequestModel
    {
       // CMHEADERID ENAME   GEN_REQUEST_NO CUSTOMER_CODE   NAME CUST_ACC_TYPE   REQUEST_TYPE Status  REQUESTDATE REQUEST_TYPE1
        public long? CMHEADERID { get; set; }
        public string? ENAME { get; set; }
        public string? GEN_REQUEST_NO { get; set; }
        public string? CUSTOMER_CODE { get; set; }
        public string? NAME { get; set; }
        public string? CUST_ACC_TYPE { get; set; }
        public string? REQUEST_TYPE { get; set; }
        public string? Status { get; set; }
        public DateTime? REQUESTDATE { get; set; }
        public string? REQUEST_TYPE1 { get; set; }


    }
}
