using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class SAPPayload
    {
        public class getAttendanceData
        {
            public string? userid { get; set; }
            public string? strdate { get; set; }
            public string? strcheck { get; set; }
        }
    }
}
