using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ePortal.ViewModels.VehicleDTO;

namespace ePortal.ViewModels
{
    public class SecurityLocationMapViewModelVP
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public int EmpCode { get; set; }

        public string SiteName { get; set; }
        public string EmpName { get; set; }

        public int IsActive { get; set; }

        public List<LocationDto> SiteOptions { get; set; }
        public List<EmployeeDetailDto> EmployeeOptions { get; set; }
    }
}
