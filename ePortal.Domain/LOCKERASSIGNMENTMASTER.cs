using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    public partial class LOCKERASSIGNMENTMASTER
    {
        public int ASSIGN_ID { get; set; }

        public int EMP_ID { get; set; }

        public int? SYSITEID { get; set; }

        public int? FLOOR_ID { get; set; }

        public int? LOCKER_ID { get; set; }

        public int? BOX_ID { get; set; }

        public DateTime? REQUEST_DATE { get; set; }

        public int? APPROVED_BY { get; set; }

        public DateTime? APPROVED_DATE { get; set; }

        public int? ASSIGNED_BY { get; set; }

        public DateTime? ASSIGNED_DATE { get; set; }

        public DateTime? RELEASE_DATE { get; set; }

        public int? STATUS { get; set; }

        public string? REMARKS { get; set; }

        public int? CREATED_BY { get; set; }

        public DateTime? CREATED_AT { get; set; }

        public int? MODIFIED_BY { get; set; }

        public DateTime? MODIFIED_AT { get; set; }

        public virtual LOCKERBOXMASTER BOX { get; set; }
    }
}
