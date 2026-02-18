using ePortal.DomainClasses;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;

namespace ePortal.ViewModels
{
    public class TaxiRequestViewModel
    {
        public string? REQUESTID { get; set; }
        public string? EMPCODE { get; set; }
        public string? EMPNAME { get; set; }
        public string? EMAIL { get; set; }
        [DisplayName("Extension No")]
        [MaxLength(3)]
        public string? EXTENSIONO { get; set; }
        public string?    APPLICATIONDATE { get; set; }
        [DisplayName("Date of Travel - From")]
        public string? DATEOFTRAVELFROM { get; set; } = DateTime.Now.ToString("dd-MMM-yyyy");
        [DisplayName("To")]
        public string? DATEOFTRAVELTO { get; set; } = DateTime.Now.ToString("dd-MMM-yyyy");
        [DisplayName("Reporting Place (Address)")]
        public string? ADDRESS { get; set; }

        [DisplayName("Reporting Site (Address)")]
        public string? SITE { get; set; }
        public IEnumerable<SelectListItem>? SITEList { get; set; }

        [DisplayName("Contact No")]
        [MaxLength(10)]

        public long? PHONENO { get; set; }
        [DisplayName("Reporting Time[HH24: MM]")]
        public string?  REPORTINGTIME { get; set; }
        [DisplayName("Purpose of Visit")]
        public string? PURPOSEOFVISIT { get; set; }
        [DisplayName("Place of Visit")]
        public string? PLACEOFVISIT { get; set; }
        [DisplayName("Forward To [ for approval ]")]
        public string?    SUPERVISORADEMPCODE { get; set; }
        public string? SUPERVISORADEMPNAME { get; set; }
        public IEnumerable<SelectListItem>? ApproverList { get; set; }

        [DisplayName("Shift")]
        public string? SHIFT { get; set; }

        public IEnumerable<SelectListItem>? ShiftOptions { get; set; }
        [DisplayName("Number Of Person [ To Travel ]")]
        [MaxLength(2)]
        public int? NOOFPERSON { get; set; } = 1;
        [DisplayName("Remarks")]
        public string? REMARKS { get; set; }
        public string? DEPTMGRREMARKS { get; set; }
        public string? DEPTMGRSSTAUS { get; set; }

        public string? REPORTINGHOUR { get; set; }
        public string? REPORTINGMIN { get; set; }


        public bool IsEnabled { get; set; } = false;
        public bool SecondApproval { get; set; }

        public string HDN_MFG_OPID { get; set; }
        public string HDN_TAPUKARA_ID { get; set; }

        public List<TaxiRequestList>? taxiRequestList { get; set; }
        public List<TaxiUsesReport>? TaxiUsesReportList { get; set; }
        
        public VehicleReleased? VehicleReleased { get; set; }
        public List<VehicleReport>? VehicleReport { get; set; }

    }
    public class TaxiRequestRequest
    {
        public string? REQUESTID { get; set; }
        public string? EMPCODE { get; set; }
        public string? EMPNAME { get; set; }
        public string? EMAIL { get; set; }
       
        public string? EXTENSIONO { get; set; }
        public string? APPLICATIONDATE { get; set; }

        public string? DATEOFTRAVELFROM { get; set; }
       
        public string? DATEOFTRAVELTO { get; set; }
        public string? ADDRESS { get; set; }

       
        public string? SITE { get; set; }
       
        public long? PHONENO { get; set; }
      
        public string? REPORTINGTIME { get; set; }
       
        public string? PURPOSEOFVISIT { get; set; }
        
        public string? PLACEOFVISIT { get; set; }
        
        public string? SUPERVISORADEMPCODE { get; set; }
        public string? SUPERVISORADEMPNAME { get; set; }
       
        public string? SHIFT { get; set; }

        
        public int? NOOFPERSON { get; set; } = 1;
      
        public string? REMARKS { get; set; }
        public string? REPORTINGHOUR { get; set; }
        public string? REPORTINGMIN { get; set; }
        public string? APPRSTATUS { get; set; }
        public string HDN_MFG_OPID { get; set; }
        public string HDN_TAPUKARA_ID { get; set; }
    }
        public class TaxiRequestList
    {
        public string? ADVEHICLEREQUESTID { get; set; }
        public string? ADVECHICLEDETAILID { get; set; }
        public string? empcode { get; set; }
        public string? empname { get; set; }
        public string? dateoftravelfrom { get; set; }
        public string? dateoftravelto { get; set; }
        public string? dateapplied { get; set; }
        public string? AdminStatus { get; set; }
        public string? RELEASESTATUS { get; set; }
    }
    public class VehicleReleased
    {
        public string? ADVEHICLEREQUESTID { get; set; }
        public string? ADVECHICLEDETAILID { get; set; }
        public string? Name { get; set; }
        public string? Designation { get; set; }
        public string? Dept { get; set; }
        public string? Email { get; set; }
        public string? Ext { get; set; }
        public string? Phoneno { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? Time { get; set; }
        public string? Purpose { get; set; }
        public string? Place { get; set; }

        public string? ReportingSite { get; set; }
        public string? ReportingPlace { get; set; }

        public string? RELEASEMETERREADING { get; set; }
        public string? METERREADING { get; set; }
        public string? RELEASEDAT { get; set; }

        public string? VISITE_PLACE { get; set; }
        public string? TAXICONDITIONSTATUS { get; set; }
        public string? REQUESTERREMARKS { get; set; }
        public string? SLIPNO { get; set; }
        public string? REMARKS { get; set; }
        public string? SYSITEID { get; set; }
        public string? BUSROUTEID { get; set; }

        public bool IsEnabled { get; set; } = true;
        public IEnumerable<SelectListItem>? BUSROUTELIST { get; set; }
    }
    public class TaxiUsesReport
    {
        public string? EMPCODE { get; set; }
        public string? EMPNAME { get; set; }
        public DateTime? TAXIREQUIREDATE { get; set; }
        public string? VEHICLENO { get; set; }
        public string? DESCRIP { get; set; }
        public string? USEKM { get; set; }
        public string? RELEASEAT { get; set; }
        public string? REMARKS { get; set; }




    }
    
    public class VehicleRequestModel
        {
            public string? RequestId { get; set; }
            public string? Employee { get; set; }
            public string? Department { get; set; }
            public string? ReportingSite { get; set; }
            public string? NumberOfPersons { get; set; }
            public string? ApplicationDate { get; set; }
            public string? FromDate { get; set; }
            public string? ToDate { get; set; }
            public string? Time { get; set; }
            public string? Remark { get; set; }
            public string? SupervisorName { get; set; }
            public string? SupervisorEmail { get; set; }
            public string? SupervisorStatus { get; set; }
            public string? SupervisorRemarks { get; set; }
            public string? SupervisorApprovedDate { get; set; }
            public string? AdminName { get; set; }
            public string? AdminStatus { get; set; }
            public string? TaxiNumber { get; set; }
            public string? AdminRemarks { get; set; }
            public string? AdminApprovalDate { get; set; }
            public string? VendorName { get; set; }
            public string? SupSupervisorName { get; set; }
            public string? SupSupervisorStatus { get; set; }
            public string? SupSupervisorDate { get; set; }
            public string? SupSupervisorEmpCode { get; set; }
            public string? SupSupervisorEmail { get; set; }
            public string? SupSupervisorRemarks { get; set; }
            public string? SupEcode { get; set; }
            public string? ActiveStatus { get; set; }
            public string? PurposeOfVisit { get; set; }
            public string? PlaceOfVisit { get; set; }
            public string? ReportingPlace { get; set; }
            public bool IsEnabled { get; set; }
            public string? RecAuth { get; set; }
            public string? RecEmail { get; set; }
            public string? RecStatus { get; set; }
            public string? RecRem { get; set; }
            public string? RecDate { get; set; }

        }


    public class VehicleReport
    {
        public string? EMPSECTION { get; set; }
        public string? EMPNAME { get; set; }
        public string? TRAVELDATE { get; set; }
        public string? FROLLOC { get; set; }
        public string? TOLOC { get; set; }
        public string? REPORTINGTIME { get; set; }
    }

}
