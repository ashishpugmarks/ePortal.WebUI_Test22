using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    public class PARKING_ZONE
    {
        public long ID { get; set; }
        public string ZONE_NAME { get; set; } = string.Empty;
        public string? ZONE_CODE { get; set; }
        public string? DESCRIPTION { get; set; }
        public int? TOTAL_CAPACITY { get; set; }
        public int? AVAILABLE_CAPACITY { get; set; }
        public int? LOCATION_ID { get; set; }
        public bool IS_ACTIVE { get; set; } = true;
        public DateTime CREATED_AT { get; set; } = DateTime.UtcNow;
        public DateTime? UPDATED_AT { get; set; }
    }
}
