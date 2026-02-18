using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.WebPages.Html;

namespace ePortal.ViewModels
{
    public class RouteChangeViewModel
    {
        [Required]
        public string EmployeeCode { get; set; }

        [Required(ErrorMessage = "Please select a site.")]
        public string SiteId { get; set; }
        public IEnumerable<SiteModel> Sites { get; set; }

        [Required(ErrorMessage = "Please select a request type.")]
        public string RequestType { get; set; }

        public string CurrentStopId { get; set; }
        public IEnumerable<BusStopModel> CurrentStops { get; set; }

        public string CurrentRouteId { get; set; }
        public IEnumerable<BusRouteModel> CurrentRoutes { get; set; }

        [Required(ErrorMessage = "Please select new stop.")]
        public string NewStopId { get; set; }
        public IEnumerable<BusStopModel> NewStops { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        public string StartDate { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        public string Remarks { get; set; }
        public string CancelRemarks { get; set; }

        public decimal MonthlyPay { get; set; }

        [Required(ErrorMessage = "You must accept the terms.")]
        public bool AcceptTerms { get; set; }

        public string SelectedSiteId { get; set; }

        public int SelectedRequestId { get; set; }


    }

    public class SiteModel { public string SiteId { get; set; } public string SiteName { get; set; } public string SelectedSite { get; set; } }
    public class BusStopModel { public string ADBUSSTOPID { get; set; } public string DESCRIP { get; set; } public string SelectedCurrentStop { get; set; } }
    public class BusRouteModel { public string ADBUSROUTEID { get; set; } public string DESCRIP { get; set; } public string SelectedRoute { get; set; } }

    public class RouteChangeRequest
    {
        public long RequestId { get; set; }
        public string EmployeeName { get; set; }
        public string Request { get; set; }
        public string CurrentRoute { get; set; }
        public string CurrentStop { get; set; }
        public string NewRoute { get; set; }
        public string NewStop { get; set; }
        public string UsageStartDate { get; set; }
        public string Address { get; set; }
        public decimal NewCharge { get; set; }
        public string Remarks { get; set; }
        public string AdminStatus { get; set; }
        public string AdminRemarks { get; set; }
        public string NewStopId { get; set; }
        public string CancelRemarks { get; set; }
        public int SelectedRequestId { get; set; }
    }
}

