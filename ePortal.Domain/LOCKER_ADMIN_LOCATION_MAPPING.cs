using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    public partial class LOCKER_ADMIN_LOCATION_MAPPING
    {
        public int ID { get; set; }
        public int SITE_ID { get; set; }
        public int ADMIN_CODE { get; set; }
        public int IS_ACTIVE { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public int? CREATED_BY { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
    }
}
