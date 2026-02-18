using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class SeatMgmtViewModel
    {
        public string CAL_MONTH_YEAR { get; set; }
        public int ALLOW_SEAT { get; set; }
        public int ALLOW_PERCENT { get; set; }
        public int PHYSICALSEAT { get; set; }
        public string MAP_FILE_NAME { get; set; }
        public int EXTRA_SEAT { get; set; }//Added by aumento for : SR100656
        public virtual List<SeatCalenderViewModel> CAL_HEADER_LIST { get; set; }
        public virtual List<SeatEmpCalViewModel> EMP_LIST { get; set; }

        public int NewJoineeCount { get; set; } //SR102091 
        public int InactiveCount { get; set; } //SR102091
    }

    public class SeatEmpCalViewModel
    {
        public long ECODE { get; set; }
        public string ENAME { get; set; }
        public long? DEPARTMENTID { get; set; }
        public string DEPARTMENT { get; set; }
        public long? DIVISIONID { get; set; }
        public string DIVISION { get; set; }
        public long? OPERATIONID { get; set; }  //SR56983_ Changes
        public string OPERATION { get; set; }
        public long? FN_DESIGNATIONID { get; set; }
        public long FLOORID { get; set; }
        public virtual List<SeatCalenderViewModel> CAL_LIST { get; set; }
        //SR102091 START
        public DateTime Date { get; set; }
        public bool IsNewJoinee { get; set; }
        public bool IsInActiveEmp { get; set; }
        //SR102091 END
    }

    public class SeatRosterRawModel
    {
        public string ADEMPCODE { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }
        public string DIVISIONID { get; set; }
        public string DIVISION { get; set; }
        public string DEPARTMENTID { get; set; }
        public string DEPARTMENT { get; set; }
        public string DESIGNATION_ID { get; set; }
        public string FLOORID { get; set; }
        public string ROSTER_DATE { get; set; }
        public string SEAT_NAME { get; set; }
        public string SEAT_NO { get; set; }
    }

    public class SeatCalenderViewModel
    {
        public int CALID { get; set; }
        public string CAL_DATE { get; set; }
        public DateTime CAL_DATE_FORMAT { get; set; }
        public string WEEK_DAY { get; set; }
        public bool IS_OFFDAY { get; set; }
        public string OFFDAY_COLOR { get; set; }
        public string WORKINGDAY_COLOR { get; set; }
        public bool IS_READONLY { get; set; }
        public bool IS_CHECKED { get; set; }
        public int DATE_WISE_COUNT { get; set; }
        public string ALLOATED_SEAT { get; set; }
        public bool IsNewJoinee { get; set; } //SR102091
        public bool IsInActiveEmp { get; set; } //SR102091
        public virtual SeatAllocationViewModel SEAT_OBJ { get; set; }
    }

    public class SeatRosterViewModel
    {
        public long RSTTRNID { get; set; }
        public long ADEMPCODE { get; set; }
        public System.DateTime ROSTER_DATE { get; set; }
        public string strROSTER_DATE { get; set; }
        public short STATUS { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDON { get; set; }
        public Nullable<long> UPDATEDBY { get; set; }
        public Nullable<System.DateTime> UPDATEDON { get; set; }
        public short IS_OHACTION { get; set; }
        public short IS_NEWJOINEEDAY { get; set; } //SR102091
    }

    public class SeatSettingViewModel
    {
        public string PARM_NAME { get; set; }
        public string PARM_VALUE { get; set; }
        public string DESCRIPTION { get; set; } //SR102091
    }

    public class SeatAllocationViewModel
    {
        public long SEAT_ID { get; set; }
        public string SEAT_NO { get; set; }
        public string SEAT_NAME { get; set; }
    }

    public class FixSeatVM
    {
        public long FIXSEATMAPID { get; set; }
        public long ADEMPCODE { get; set; }
        public string SEATNO { get; set; }
        public short STATUS { get; set; }
    }

    public class FixSeatViewModel
    {
        [DisplayName("SNo")]
        public long FIXSEATMAPID { get; set; }

        [DisplayName("Ecode")]
        public long ADEMPCODE { get; set; }

        [DisplayName("Employee Name")]
        public string ADEMPNAME { get; set; }

        public long SEATMSTID { get; set; }

        [DisplayName("Seat")]
        public string SEATNO { get; set; }

        [DisplayName("Status")]
        public short STATUS { get; set; }

        [DisplayName("Added By")]
        public long ADDEDBY { get; set; }

        [DisplayName("Added On")]
        public System.DateTime ADDEDON { get; set; }
        public Nullable<long> UPDATEDBY { get; set; }
        public Nullable<System.DateTime> UPDATEDON { get; set; }

    }

    public class SearchSeatViewModel
    {
        [DisplayName("Operation")]
        public long? OpId { get; set; }

        [DisplayName("Division")]
        public long? DivId { get; set; }

        [DisplayName("Employee Code")]
        public long Employee { get; set; }

        [DisplayName("From Date")]
        public string FromDate { get; set; }

        [DisplayName("To Date")]
        public string ToDate { get; set; }
        public int Status { get; set; } //SR102091
    }
    public class SeatingReportViewModel
    {
        [DisplayName("SNo")]
        public long SEATINGID { get; set; }

        [DisplayName("Ecode")]
        public long ADEMPCODE { get; set; }

        [DisplayName("Associate Name")]
        public string ADEMPNAME { get; set; }

        [DisplayName("Seat No")]
        public string SEATNO { get; set; }
        public string SEATNAME { get; set; }

        [DisplayName("Status")]
        public short STATUS { get; set; }

        [DisplayName("Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime DATE { get; set; }

        [DisplayName("Building/Floor")]
        public string FLOOR { get; set; }
        public string BUILDING { get; set; }

        [DisplayName("Operation")]
        public string OPERATION { get; set; }

        [DisplayName("Division")]
        public string DIVISION { get; set; }

    }

    public class DivWiseSeatVM
    {
        public List<DivisionSeatCapacity> DIV_LIST { get; set; }
        public long OPERATIONID { get; set; }
    }

    public class DivWiseSeatViewModel
    {
        [DisplayName("S No")]
        public long DWS_DTLID { get; set; }
        public long FLOOR_OPMAPPID { get; set; }
        public long OPERATIONID { get; set; }

        [DisplayName("Operation")]
        public string OPERATION { get; set; }

        [DisplayName("Seat Capacity")]
        public short OPERATION_SEAT { get; set; }
        public long DIVISIONID { get; set; }
        [DisplayName("Division")]
        public string DIVISION { get; set; }

        [DisplayName("Physical Seat")]
        public short PHYSICALSEAT { get; set; }

        public List<DivisionSeatCapacity> DIV_LIST { get; set; }

        [DisplayName("Status")]
        public bool STATUS { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDON { get; set; }
        public Nullable<long> UPDATEDBY { get; set; }
        public Nullable<System.DateTime> UPDATEDON { get; set; }
        [DisplayName("Seat Auto Update")]
        public string IsSeatAutoUpdated { get; set; } //Added by Aumento :: SR100223

    }

    public class DivisionSeatCapacity
    {
        public long DWSID { get; set; }
        public long DIVISIONID { get; set; }
        [DisplayName("Division")]
        public string DIVISION { get; set; }

        [DisplayName("Physical Seat")]
        public short PHYSICALSEAT { get; set; }
        [DisplayName("Status")]
        public bool STATUS { get; set; }
        public string IsSeatAutoUpdated { get; set; }//Added by Aumento :: SR100223
        public string isSeatautoupdateforOP { get; set; } //Added by Aumento :: SR100223
    }
    public class DivWiseCountReportViewModel
    {
        public string CAL_MONTH_YEAR { get; set; }
        public virtual List<SeatCalenderViewModel> CAL_HEADER_LIST { get; set; }
        public virtual List<SeatEmpCalViewModel> DIV_LIST { get; set; }
    }

    public class FloorOperationMappingVM
    {
        public long FLOOR_OPMAPPID { get; set; }
        public long FLOORID { get; set; }
        public long OPERATIONID { get; set; }
        public short SEATCAPACITY { get; set; }
        public short STATUS { get; set; }
    }


    public class FloorOperationMapViewModel
    {
        [DisplayName("S No")]
        public long FLOOR_OPMAPPID { get; set; }
        public long FLOORID { get; set; }
        [DisplayName("Floor")]
        public string FLOOR { get; set; }
        [DisplayName("Building")]
        public string BUILDING { get; set; }
        public long OPERATIONID { get; set; }
        [DisplayName("Operation")]
        public string OPERATION { get; set; }
        [DisplayName("Seating Capacity")]
        public short SEATCAPACITY { get; set; }
        [DisplayName("Status")]
        public short STATUS { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDDATE { get; set; }
        public Nullable<long> UPDATEDBY { get; set; }
        public Nullable<System.DateTime> UPDATEDDATE { get; set; }
        [DisplayName("Ki")]
        public long SYKIID { get; set; }
        public string IsSeatAutoUpdated { get; set; } //Added by Aumento :: SR100223
    }

    public class FloorSeatMstVM
    {
        public long SEATMSTID { get; set; }
        public long FLOORID { get; set; }
        public string SEATNAME { get; set; }
        public string SEATNO { get; set; }
        public short STATUS { get; set; }
    }

    public class FloorSeatMstViewModel
    {
        [DisplayName("SNo")]
        public long SEATMSTID { get; set; }
        public long FLOORID { get; set; }

        [DisplayName("Floor")]
        public string FLOOR { get; set; }

        [DisplayName("Building")]
        public string BUILDING { get; set; }

        [DisplayName("Seat Name")]
        public string SEATNAME { get; set; }

        [DisplayName("Seat No")]
        public string SEATNO { get; set; }

        [DisplayName("Status")]
        public short STATUS { get; set; }
        public long ADDEDBY { get; set; }
        public System.DateTime ADDEDON { get; set; }
        public Nullable<long> UPDATEDBY { get; set; }
        public Nullable<System.DateTime> UPDATEDON { get; set; }
    }

    //Added by aumento for the SR56983 ===============
    public class SeatingAllocationDetailReportViewModel
    {
        [DisplayName("SNo")]
        public Nullable<long> SEATINGID { get; set; }

        [DisplayName("Ecode")]
        public Nullable<long> ADEMPCODE { get; set; }

        [DisplayName("Associate Name")]
        public string ADEMPNAME { get; set; }

        [DisplayName("Seat No")]
        public string SEATNO { get; set; }
        public string SEATNAME { get; set; }

        [DisplayName("Status")]
        public short STATUS { get; set; }

        [DisplayName("Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public Nullable<DateTime> DATE { get; set; }

        [DisplayName("Building/Floor")]
        public string FLOOR { get; set; }
        public string BUILDING { get; set; }

        [DisplayName("Operation")]
        public string OPERATION { get; set; }

        [DisplayName("Division")]
        public string DIVISION { get; set; }

        //[DisplayName("Total ManePower")]
        //public Nullable<int>  TOTAL_MANEPOWER { get; set; }

        //[DisplayName("Total Phisical Seat")]
        //public Nullable<int> TOTAL_PHISICAL_SEAT { get; set; }

        //[DisplayName("Allocated Seat")]
        //public Nullable<int> ALLOCATED_SEAT { get; set; }

        //[DisplayName("Vacant Seat")]
        //public Nullable<int> VACANT_SEAT { get; set; }


        //[DisplayName("Remarks")]
        [DisplayName("Status")] //Added by Aumento for SR102091
        public string REMARKS { get; set; }



    }

    //Added by aumento for the SR56983 ===============
    public class SeatingAllocationSummaryReportViewModel
    {
        [DisplayName("SNo")]
        public Nullable<long> SRNO { get; set; }

        [DisplayName("Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public Nullable<DateTime> DATE { get; set; }

        [DisplayName("Building/Floor")]
        public string FLOOR { get; set; }
        public string BUILDING { get; set; }

        [DisplayName("Total ManPower")]
        public Nullable<int> TOTAL_MANEPOWER { get; set; }

        [DisplayName("Physical Seat Available")]
        public Nullable<int> SEATCAPACITY { get; set; }

        [DisplayName("Eligibility As Per (75%)")]
        public Nullable<int> TOTAL_PHISICAL_SEAT { get; set; }

        [DisplayName("Allocated Seat")]
        public Nullable<int> ALLOCATED_SEAT { get; set; }

        [DisplayName("Vacant Seat")]
        public Nullable<int> VACANT_SEAT { get; set; }


    }

    public class SearchSeatingDetailVM
    {
        public long? OpId { get; set; }

        public long? DivId { get; set; }

        public long Employee { get; set; }

        public long? FLOOR_ID { get; set; }

        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
    }

    //Added by aumento for the SR56983 ===============
    public class SearchSeatingDetailViewModel
    {
        [DisplayName("Operation")]
        public long? OpId { get; set; }

        [DisplayName("Division")]
        public long? DivId { get; set; }

        [DisplayName("Employee Code")]
        public long Employee { get; set; }

        [DisplayName("Floor Name")]
        public string FLOOR { get; set; }
        public string BUILDING { get; set; }

        [DisplayName("Floor ID")]
        public long? FLOOR_ID { get; set; }

        [DisplayName("From Date")]
        public string FromDate { get; set; }

        [DisplayName("To Date")]
        public string ToDate { get; set; }
    }

    public class ExtrSeatMstVM

    {
        public long SRNO { get; set; }
        public long DIVISIONID { get; set; }
        public long EXTRASEATCOUNT { get; set; }
        public short STATUS { get; set; }
    }


    //Added by aumento for : SR100656
    public class ExtrSeatMstViewModel

    {
        [DisplayName("SNo")]
        public long SRNO { get; set; }

        [DisplayName("DivisionId")]
        public long DIVISIONID { get; set; }

        [DisplayName("Division")]
        public string DIVISION { get; set; }

        [DisplayName("Extra Seat")]
        public long EXTRASEATCOUNT { get; set; }

        [DisplayName("Status")]
        public short STATUS { get; set; }
        public string ADDEDBY { get; set; }
        public Nullable<System.DateTime> ADDEDON { get; set; }
        public Nullable<long> UPDATEDBY { get; set; }
        public Nullable<System.DateTime> UPDATEDON { get; set; }
    }
    //Added by aumento for : SR100656

    //SR102091 START   
    public class EmpActiveInactiveforRosterViewModel
    {
        public long? ADEMPCODE { get; set; }
        public int? ACTIVITY_STATUS { get; set; }
        public string DIVISION { get; set; }
        public string OPERATION { get; set; }
        public string DEPARTMENT { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CREATED_DATE { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? UPDATED_DATE { get; set; }
        public string EMPLOYEENAME { get; set; }
    }
    public class EmpActiveInactiveforRosterLogViewModel
    {
        public long ID { get; set; }
        public long? RID { get; set; }
        public long? ADEMPCODE { get; set; }
        public int? ACTIVITY_STATUS { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CREATED_DATE { get; set; }
        public long ADDED_BY { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? UPDATED_DATE { get; set; }
        public string EMPLOYEENAME_Updated_by { get; set; }
        public string EMPLOYEENAME { get; set; }
    }
    public class MandatoryWFODaysViewModel
    {
        public long History_id { get; set; }
        public long Day_value { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public long Modified_By { get; set; }
        public string DESCRIPTION { get; set; }
        public string EMPLOYEENAME { get; set; }
    }
    //SR102091 END
}
