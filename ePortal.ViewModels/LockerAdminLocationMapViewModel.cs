using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ePortal.ViewModels.VehicleDTO;

namespace ePortal.ViewModels
{
    public class LockerAdminLocationMapViewModel
    {
        public int Id { get; set; }

        // Foreign Keys
        public int SiteId { get; set; }
        public int AdminCode { get; set; }

        // Display Names for List Grid
        public string? SiteName { get; set; }
        public string? AdminName { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeDepartment { get; set; }
        public string? EmployeeDesignation { get; set; }
        public string? EmployeeOperation { get; set; }
        public string? EmployeeSection { get; set; }
        public string? EmployeeDivision { get; set; }
        // Boolean/Number Flag
        public int IsActive { get; set; }

        // Dropdowns for Add/Edit Popup
        public List<LocationDto>? SiteOptions { get; set; }
        public List<EmployeeDetailDto>? AdminOptions { get; set; }
        //public List<LocationDto>? Location { get; set; }
        public string? LocationName { get; set; }
        public DateTime? RequestDate { get; set; }
        public int? Status { get; set; } // 2=Req, 3=Approve, 4=Assign, 5=Release
        public int? FloorId { get; set; }

        public string? FloorName { get; set; }
        public int? LockeId { get; set; }

        public string? LockerName { get; set; }

        public int? LockerBoxId { get; set; }
        public string? BoxNumber { get; set; }

        public string StatusText => Status switch
        {
            2 => "Requested",
            3 => "Approved",
            4 => "Assigned",
            5 => "Released",
            _ => "Unknown"
        };
    }
    public class LockerAdminLocationMapUpdateDto
    {
        public int? Id { get; set; }
        public int? SiteId { get; set; }
        public int? EmpCode { get; set; }
        public int? IsActive { get; set; }
    }
    public class LockerAdminRequestDTO
    {
        public int TotalRequest { get; set; }
        public int PendingRequest { get; set; }
        public int ApprovedRequest { get; set; }
        public int ReallocateRequest { get; set; }
        public List<LocationDto>? Location { get; set; }

        public List<LockerAdminLocationMapViewModel> AdminRequest { get; set; } = new();
    }
}
