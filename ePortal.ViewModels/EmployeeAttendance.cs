using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class EmployeeAttendance
    {
        public string? EMPLOYEENO { get; set; }
        public DateTime EMP_LDATE { get; set; }
        public string? EMP_MONTH { get; set; }
        public string? EMP_SHIFT { get; set; }
        public string? EMP_IN_TIME { get; set; }
        public string? EMP_OUT_TIME { get; set; }
        public string? EMP_APP_OVERSTAY { get; set; }
        public string? EMP_ACT_OVERSTAY { get; set; }
        public string? EMP_PRST_REMARK { get; set; }
        public string? EMP_UABS_REMARK { get; set; }
        public string? COLOR { get; set; }
    }
}
