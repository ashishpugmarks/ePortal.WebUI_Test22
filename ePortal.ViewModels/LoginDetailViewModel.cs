using ePortal.DomainClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace ePortal.ViewModels
{
    public class SearchLoginDetail
    {
        [DisplayName("Operation")]
        public long OperationID { get; set; }

        [DisplayName("Division")]
        public long DivisionID { get; set; }

        [DisplayName("Department")]
        public long DEPTID { get; set; }

        [DisplayName("Section")]
        public long SECID { get; set; }

        [DisplayName("From Date")]
        public string Startdate { get; set; }

        [DisplayName("End Date")]
        public string ENDDATE { get; set; }
        
        [DisplayName("Ecode")]
        public long ecode { get; set; }

        [DisplayName("Status")]
        public short Status { get; set; }
        public List<VM_VW_DGIT_IOMREPORT> SearchResult { get; set; }
        [DisplayName("Application")]
        public long ApplicationID { get; set; }

        public long UserLoginID { get; set; }
        
    }
    public class VM_LoginDetail_SYApplication
    {
        public long ApplicationID { get; set; }
        public string APPLICATION { get; set; }
        public short STATUS { get; set; }   
    }
    public class VM_EMP_LOGINDETAIL
    {
        public long EMP_LOGINDETAILID { get; set; }
        [DisplayName("ECode")]
        public long ECODE { get; set; }
        [DisplayName("Application")]
        public long APPSYSTEMID { get; set; }
        [DisplayName("Login ID")]
        public string LOGINIDCREATED { get; set; }
        public System.DateTime DATEADDED { get; set; }
        public long ADDEDBY { get; set; }
        public Nullable<long> MODIFIEDBY { get; set; }
        public Nullable<System.DateTime> MODIFIEDDATE { get; set; }
        public short STATUS { get; set; }
        public string ApplicationName { get; set; }
        public string StrDATEADDED { get { return DATEADDED.ToString("dd-MMM-yyyy"); } }

    }
    public class AddLoginIDHeader
    {
        [DisplayName("Operation")]
        public string OperationName { get; set; }

        [DisplayName("Division")]
        public string DivisionName { get; set; }

        [DisplayName("Department")]
        public string DEPTName { get; set; }

        [DisplayName("Section")]
        public string SECName { get; set; }

        [DisplayName("Ecode")]
        public string ecode { get; set; }

        [DisplayName("Employee Name")]
        public string EMPName { get; set; }
        public VM_EMP_LOGINDETAIL ADDEDLoginDetail { get; set; }
        public List<VM_EMP_LOGINDETAIL> SearchResult { get; set; }
        public List<VM_EMP_APPLICATIONMAP_AD> ApplicationList { get; set; }
        public long UserLoginID { get; set; }

    }
    public class VM_EMP_APPLICATIONMAP_AD
    {
        public long EMP_APPLICATIONMAP_AD_ID { get; set; }
        public long APPSYSTEMID { get; set; }
        public long ADEMPCODE { get; set; }
        public short STATUS { get; set; }
        public System.DateTime DATEADDED { get; set; }
        public long ADDEDBY { get; set; }
        public Nullable<long> MODIFIEDBY { get; set; }
        public Nullable<System.DateTime> MODIFIEDDATE { get; set; }
        public string ApplicationName { get; set; }

    }


}
