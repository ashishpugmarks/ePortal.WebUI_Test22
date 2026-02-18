using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class RoleViewModel
    {
        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public long ProcessId { get; set; }
        public long RoleBPO { get; set; }
        public string Status { get; set; }
        public long Created_By { get; set; }
        public System.DateTime Created_Date { get; set; }
        public Nullable<long> Modified_By { get; set; }
        public Nullable<System.DateTime> Modified_Date { get; set; }
        public string ProcessName { get; set; }

    }
}
