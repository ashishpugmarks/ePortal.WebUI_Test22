using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class LC_HEAD_ViewModel
    {

     
            public int SRNO { get; set; }
            public string LCTRANNO { get; set; }
            public string LCTRANDATE { get; set; }
            public string EMPCODE { get; set; }
            public string PURPOSE { get; set; }
            public decimal TOTAL_AMOUNT { get; set; }
            public string TOTAL_AMT_IN_WORD { get; set; }
            public string APPROVER_STATUS { get; set; }
            public string APPROVER_NAME { get; set; }
            public string APPROVER_USER { get; set; }
            public string APPROVER_DATETIME { get; set; }
            public string ADMIN_STATUS { get; set; }
            public string ADMIN_USER { get; set; }
            public string ADMIN_NAME { get; set; }
            public string ADMIN_DATETIME { get; set; }
            public string FINANCE_STATUS { get; set; }
            public string FINANCE_NAME { get; set; }
            public string FINANCE_USER { get; set; }
            public string FINANCE_DATETIME { get; set; }
            public decimal REQUIRED_AMOUNT { get; set; }
            public decimal FUAL { get; set; }
            public decimal TOTAL_NOOFHOURS { get; set; }
            public decimal TOTAL_KMCOVERED { get; set; }
            public string ADDEDON { get; set; }
            public string ADDEDBY { get; set; }
            public string UPDATEDON { get; set; }
            public string UPDATEDBY { get; set; }
            public string DESIGNATION { get; set; }
            public string EMPNAME { get; set; }
            public string APPROVER_REMARKS { get; set; }
            public string ADMIN_REMARKS { get; set; }
            public string FINANCE_REMARKS { get; set; }
            public decimal APPROVED_FUEL { get; set; }
            public string RECOMM_USER { get; set; }
            public string RECOMM_STATUS { get; set; }
            public string RECOMM_NAME { get; set; }
            public string RECOMM_DATETIME { get; set; }
            public string POSTINGDATE { get; set; }
            public string FINAL_STATUS { get; set; }
            public string RECOMM_REMARKS { get; set; }
            public decimal TOTAL_MEAL_AMOUNT { get; set; }
            public decimal TOTAL_CONV_AMOUNT { get; set; }
            public string PDF_NAME { get; set; }
            public string SAP_DOC_NO { get; set; }
        
    }

    // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
    public class LC_OH_DIRAPPROVALFLOW_MASTERViewModel
    {
        public long SRNO { get; set; }
        public string REQUESTOR_DESG { get; set; }
        public string APPROVAL_DESG { get; set; }
        public string CREATEDBY { get; set; }
        public Nullable<System.DateTime> CREATEDDATE { get; set; }
        public string UPDATEDBY { get; set; }
        public Nullable<System.DateTime> UPDATEDDATE { get; set; }
        public string IPADDRESS { get; set; }
    }


    public class ApprovalHistoryModel
    {
        public string EmpCode { get; set; }
        public string EmployeeName { get; set; }
        public string ApprovalType { get; set; }
        public string Status { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string Remarks { get; set; }
    }
    // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
}
