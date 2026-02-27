using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    public partial class FLOORMASTER
    {
        public int FLOOR_ID { get; set; }

        public int SYSITEID { get; set; }

        public string? FLOOR_NAME { get; set; }

        public int? STATUS { get; set; }

        public DateTime? CREATED_DATE { get; set; }

        public virtual ICollection<LOCKERMASTER> LOCKERMASTER { get; set; } = new List<LOCKERMASTER>();
    }
}
