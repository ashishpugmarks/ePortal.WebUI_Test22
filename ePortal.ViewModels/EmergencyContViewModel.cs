using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class EmergencyContViewModel
    {
        public long EMERGENCY_ID { get; set; }
        public string OPERATION { get; set; }
        public Nullable<short> STD_CODE { get; set; }
        public Nullable<int> TELEPHONE_NO { get; set; }
        public Nullable<long> MOBILE_NO { get; set; }
        public string Site { get; set; }
        public long SiteID { get; set; }
        public short STATUS { get; set; }
       
    }
}
