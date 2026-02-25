using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    public partial class EmployeeLockerAllocation
    {
        public int AllocationId { get; set; }

        public decimal EmpId { get; set; }

        public decimal? LockerBoxId { get; set; }

        public decimal? AssignedBy { get; set; }

        public DateTime? AssignedDate { get; set; }

        public int Status { get; set; }

        public decimal? Active { get; set; }

        public string Remarks { get; set; }

        public decimal? CreatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }

        public decimal? ModifiedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}
