using ePortal.DomainClasses;
using ePortal.ViewModels.APPX.TourRequest;
using ePortal.ViewModels;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System.Data;
using static ePortal.ViewModels.APIMapper;
using static System.Collections.Specialized.BitVector32;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;
using System.Security.AccessControl;
using Newtonsoft.Json;

namespace ePortal.ViewModels.APPX.TourRequest
{
    public class dlgs
    {
    }
    //Changes Tour
    public class dlgs_Cancellation
    {
        public string txtCalcellationRemarks {  get; set; } 
    }
    public class dlgs_CancelTour
    {
        public string txtCalcellationRemarks { get; set; }
        public string lblTravelDate { get; set; }
        public string lblCityFrom { get; set; }
        public string lblCityTo { get; set; }
        public string lblTicketNumber { get; set; }
    }
    //Changes Tour
    public class AppCancellationRequestViewModel
    {
        public string lblRequestID { get; set; }
        public string lblTravelDate { get; set; }
        public string lblCityFrom { get; set; }
        public string lblCityTo { get; set; }
        public string lblTicketNumber { get; set; }
        public string lblTicketAmount { get; set; }
        public string lblIssueDate { get; set; }
        public string lblHotelName { get; set; }
        public string lblHotelAddress { get; set; }
        public string lblCheckinDate { get; set; }
        public string lblCheckinTime { get; set; }
        public string lblCheckoutDate { get; set; }
        public string lblCheckoutTime { get; set; }
        public string lblPickUpDetails { get; set; }
        public string lblErrMsg { get; set; }
    }

    public class FlightScheduleViewModel
    {
    }

    public class PrintableListViewModel
    {
        public string lblduplicate { get; set; }
        public string lblGroupNumber { get; set; }
        public string lblDate { get; set; }
        public string lblOperation { get; set; }
        public string lblDepartment { get; set; }
        public string lblMobileNo { get; set; }
        public string lblExt { get; set; }
        public string lblName { get; set; }
        public DataTable PrintGroupList { get; set; }
    }
    public class RequestCancellationViewModel
    {
        public string lblRequestID { get; set; }
        public string lblTravelDate { get; set; }
        public string lblCityFrom { get; set; }
        public string lblCityTo { get; set; }
        public string lblTicketNumber { get; set; }
        public string lblTicketAmount { get; set; }
        public string lblIssueDate { get; set; }
        public string lblHotelName { get; set; }
        public string lblHotelAddress { get; set; }
        public string lblCheckinDate { get; set; }
        public string lblCheckinTime { get; set; }
        public string lblCheckoutDate { get; set; }
        public string lblCheckoutTime { get; set; }
        public string lblPickUpDetails { get; set; }
    }
    public class RequestFormViewModel
    {
        public string lblPageRequestID { get; set; }
        public string lblEmpName { get; set; }
        public string hfAuthType { get; set; }
        public string txtMobile { get; set; }
        public string txtExtension { get; set; }
        public string lblDesignation { get; set; }
        public string lblOperation { get; set; }
        public string lblDivision { get; set; }
        public string lblDepartment { get; set; }
        public string lblSection { get; set; }
        public string txtObjective { get; set; }
        public bool chkAdvance { get; set; }
        public string lblTotalCharges { get; set; }
        public string lblStayChargeAPLUS { get; set; }
        public string lblStayChargeA { get; set; }
        public string lblStayChargeB { get; set; }
        public string lblStayChargeC { get; set; }
        public string lblTotalNights { get; set; }
        public string lblTotalAllowances { get; set; }
        public string lblAllownceAPLUS { get; set; }
        public string lblAllownceA { get; set; }
        public string lblAllownceB { get; set; }
        public string lblAllownceC { get; set; }
        public string lblTotalDays { get; set; }
        public string lblMiscAmount { get; set; }
        public string lblTotalAmount { get; set; }
        public string lblRequiredAmount { get; set; }
        public bool pnlAdvance { get; set; }
        public IEnumerable<gvListRF> gvList { get; set; }
    }

    public class gvListRF
    { 
        public string TourFromDate { get; set; }
        public string TourTime { get; set; }
        public string FromLoc { get; set; }
        public string ToLoc { get; set; }
        public string StayingLoc { get; set; }
        public string Mode { get; set; }
        public string TicketClass { get; set; }
        public string ModeDetail { get; set; }
        public string TicketingBy { get; set; }
        public string HotelReserv { get; set; }
        public string PickDrop { get; set; } 
    }
}

