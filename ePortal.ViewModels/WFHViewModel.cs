using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ePortal.ViewModels
{
    public class ASRWFH_HEADER_ViewModel
    {
        [DisplayName("SrNo")]
        public long ASRWFHID { get; set; }

        [DisplayName("Emp-Code")]
        public long ADEMPCODE { get; set; }

        [DisplayName("Employee Name")]
        public string EmpName { get; set; }

        public string EmpEmail { get; set; }

        [DisplayName("Request Type")]
        public short? REQUESTTYPE { get; set; }

        [DisplayName("Applied For")]
        public short? APPLYFOR { get; set; }

        [DisplayName("Duration")]
        public short? DURATION { get; set; }

        [DisplayName("Shift")]
        public long? WEHSHIFTID { get; set; }
        public string WEHSHIFT { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Start Date")]
        public System.DateTime? STARTDATE { get; set; }
        public string StartDateStr { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("End Date")]
        public System.DateTime? ENDDATE { get; set; }
        public string EndDateStr { get; set; }

        [DisplayName("Start Time")]
        public string STARTTIME { get; set; }

        [DisplayName("End Time")]
        public string ENDTIME { get; set; }

        [DisplayName("Remarks")]
        [AllowHtml]
        public string REMARKS { get; set; }

        [DisplayName("Added By")]
        public long ADDEDBY { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Added Date")]
        public System.DateTime DATEADDED { get; set; }
        public System.DateTime? DATELSTMOD { get; set; }
        public long? MODIFIEDBY { get; set; }

        [DisplayName("Cancel Remarks")]
        public string EMPCANCELREMARK { get; set; }

        [DisplayName("Status")]
        public short STATUS { get; set; }
        [DisplayName("No. of days")]
        public int OffDays { get; set; }
        public string WorkingDays { get; set; }
        public virtual ASRWFH_APPROVALHIS_ViewModel ASRWFH_APPROVALHIS { get; set; }
        //SR86752 Start
        [DisplayName("Request Type")]
        public short? WFH_REQUESTTYPE { get; set; }

        [DisplayName("Specify remarks")]
        [AllowHtml]
        public string WFH_REQUESTTYPEREMARKS { get; set; }

        [DisplayName("Recommended By")]
        public string RECOMMENDEDBY { get; set; }

        [DisplayName("Recommended Status")]
        public short? ISRECAPPROVED { get; set; }

        [DisplayName("Recommended Remarks")]
        public string RECREMARKS { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Recommended Date")]
        public System.DateTime? RECAPPROVEDDATE { get; set; }
       

        [DisplayName("Approval By")]
        public string APPROVALBY { get; set; }

        [DisplayName("Approval Status")]
        public short? ISAPPAPPROVED { get; set; }

        [DisplayName("Approval Remarks")]
        public string APPREMARKS { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Approval Date")]
        public System.DateTime? APPAPPROVEDDATE { get; set; }

        [DisplayName("WFH Source")]
        public short? AUTO_WFH_STATUS { get; set; }

        public long? RECADEMPCODE { get; set; }

        public long? APPADEMPCODE { get; set; }

        //SR86752 End
    }

    public class ASRWFH_APPROVALHIS_ViewModel
    {
        public long ASRWFHAPPROVALHIS { get; set; }
        public long? ASRWFHID { get; set; }

        [DisplayName("Recommended By")]
        public long? RECADEMPCODE { get; set; }
        public string REC_EMAIL { get; set; }
        public short? ISRECAPPROVED { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Recommended Date")]
        public System.DateTime? RECAPPROVEDDATE { get; set; }

        [DisplayName("Remarks")]
        public string RECREMARKS { get; set; }

        [DisplayName("Approval By")]
        public long? APPADEMPCODE { get; set; }
        public string APP_EMAIL { get; set; }

        [DisplayName("Approval Status")]
        public short? ISAPPAPPROVED { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Approval Date")]
        public System.DateTime? APPAPPROVEDDATE { get; set; }

        [DisplayName("Approval Remarks")]
        public string APPREMARKS { get; set; }
        public short? SAPUPDATESTATUS { get; set; }
        public string SAPUPDATEREMARKS { get; set; }
        public long? MODIFIEDBY { get; set; }
        public System.DateTime? DATEISTMOD { get; set; }

        public virtual ASRWFH_HEADER_ViewModel ASRWFH_HEADER { get; set; }
    }

    public class WFHAppAuthViewModel
    {
        public long? EmpCode { get; set; }
        public string EmpName { get; set; }
        public short EmpAppStatus { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime EmpAppDate { get; set; }
        public long? RecEmpCode { get; set; }
        public string RecEmpName { get; set; }
        public short RecAppStatus { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime RecAppDate { get; set; }
    }

    public class ShiftViewModel
    {
        public long SYSHIFTID { get; set; }
        public string CODE { get; set; }
        public string DESCRIP { get; set; }
        public short ACTIVE { get; set; }
        public long SYSITEID { get; set; }
        public System.DateTime? START_TIME { get; set; }
        public System.DateTime? END_TIME { get; set; }
        public string SAPSIFTDESC { get; set; }
        public decimal? HALFDAYHOUR { get; set; }
    }

    public class WFHReportViewModel
    {
        [DisplayName("Operation")]
        public long? OpId { get; set; }

        [DisplayName("Division")]
        public long? DivId { get; set; }

        [DisplayName("Department")]
        public long? DepId { get; set; }

        [DisplayName("Section")]
        public long? SecId { get; set; }

        [DisplayName("Request Type")]
        public long RequestType { get; set; }

        [DisplayName("Employee Code")]
        public long Employee { get; set; }

        [DisplayName("From Date")]
        public string FromDate { get; set; }

        [DisplayName("To Date")]
        public string ToDate { get; set; }
        //SR86752 Start
        [DisplayName("Request Status")]
        public long? RequestStatus { get; set; }
        //SR86752 End
    }
    //SR86752 Start
    public class WFHAdminReportViewModel
    {
        [DisplayName("Operation")]
        public long? OpId { get; set; }

        [DisplayName("Division")]
        public long? DivId { get; set; }

        [DisplayName("Department")]
        public long? DepId { get; set; }

        [DisplayName("Section")]
        public long? SecId { get; set; }

        [DisplayName("Request Type")]
        public long RequestType { get; set; }

        [DisplayName("Employee Code")]
        public long Employee { get; set; }

        [DisplayName("From Date")]
        public string FromDate { get; set; }

        [DisplayName("To Date")]
        public string ToDate { get; set; }

        [DisplayName("Request Status")]
        public long? RequestStatus { get; set; }
    }
    //SR86752 End
    public class WFHOpViewModel
    {
        public long OPERATIONID { get; set; }
        public string OPERATION { get; set; }
    }

    public class WFHDivViewModel
    {
        public long DIVISIONID { get; set; }
        public string DIVISION { get; set; }
    }
    public class WFHDepViewModel
    {
        public long DEPARTMENTID { get; set; }
        public string DEPARTMENT { get; set; }
    }

    public class WFHSecViewModel
    {
        public long SECTIONID { get; set; }
        public string SECTION { get; set; }
    }
    //Added by TTL on 31-July-2025 against SR104870 ? CR6961 - Start
    public class WFHOffDays
    {
        public short OFFDAYS { get; set; }
        public short TOTALDAYS { get; set; }
        public short WORKINGDAYS { get; set; }
        public bool ISVALID { get; set; }
        public bool ISREJECTVALID { get; set; }
        public bool ISACTIVEDATE { get; set; }
    }
    //Added by TTL on 31-July-2025 against SR104870 ? CR6961 - End
}
