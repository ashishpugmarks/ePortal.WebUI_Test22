using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ePortal.ViewModels.VehicleDTO;

namespace ePortal.ViewModels
{
    public class AdminLocationMapViewModel
    {
        public int Id { get; set; }

        // Foreign Keys
        public int SiteId { get; set; }
        public int AdminCode { get; set; }

        // Display Names for List Grid
        public string SiteName { get; set; }
        public string AdminName { get; set; }

        // Boolean/Number Flag
        public int IsActive { get; set; }

        // Dropdowns for Add/Edit Popup
        public List<LocationDto> SiteOptions { get; set; }
        public List<EmployeeDetailDto> AdminOptions { get; set; }
    }
    public class AdminLocationMapUpdateDto
    {
        public int? Id { get; set; }
        public int? SiteId { get; set; }
        public int? EmpCode { get; set; }
        public int? IsActive { get; set; }
    }
}
