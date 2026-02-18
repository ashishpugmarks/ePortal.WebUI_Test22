using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class SecurityLocationMapViewModel
    {
        public long Id { get; set; }
        public long SiteId { get; set; }
        public long EmpCode { get; set; }
        public short IsActive { get; set; } = 1;
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public List<SelectListItmesVm>? SiteOptions { get; set; }
        public List<SelectListItmesVm>? EmployeeOptions { get; set; }

        public string? SiteName { get; set; }
        public string? EmployeeName { get; set; }

    }
}
