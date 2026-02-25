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

        public decimal EMP_ID { get; set; }

        public decimal? SYSITEID { get; set; }

        public decimal? FLOOR_ID { get; set; }

        public decimal? LOCKER_ID { get; set; }

        public decimal? BOX_ID { get; set; }

        public DateTime? REQUEST_DATE { get; set; }

        public decimal? APPROVED_BY { get; set; }

        public DateTime? APPROVED_DATE { get; set; }

        public decimal? ASSIGNED_BY { get; set; }

        public DateTime? ASSIGNED_DATE { get; set; }

        public DateTime? RELEASE_DATE { get; set; }

        public int? STATUS { get; set; }

        public string REMARKS { get; set; }

        public decimal? CREATED_BY { get; set; }

        public DateTime? CREATED_AT { get; set; }

        public decimal? MODIFIED_BY { get; set; }

        public DateTime? MODIFIED_AT { get; set; }

        public virtual LOCKERBOXMASTER BOX { get; set; }
    }
}
