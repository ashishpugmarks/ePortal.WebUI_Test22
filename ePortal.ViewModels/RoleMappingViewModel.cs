using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class RoleMappingViewModel
    {
        public long MappingId { get; set; }
        public long MenuId { get; set; }
        public long RoleId { get; set; }
        public string Status { get; set; }
        public long CreatedBy { get; set; }
        public System.DateTime Created_Date { get; set; }
        public Nullable<long> Modified_By { get; set; }
        public Nullable<System.DateTime> Modified_Date { get; set; }
        public string MenuName { get; set; }
        public string RoleName { get; set; }

    }
}
