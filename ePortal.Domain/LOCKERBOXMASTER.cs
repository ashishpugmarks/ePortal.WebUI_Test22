using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    public partial class LOCKERBOXMASTER
    {
        public int BOX_ID { get; set; }

        public int LOCKER_ID { get; set; }

        public string? BOX_NO { get; set; }

        public int? STATUS { get; set; }

        public DateTime? CREATED_DATE { get; set; }

        public virtual LOCKERMASTER LOCKER { get; set; }

        public virtual ICollection<LOCKERASSIGNMENTMASTER> LOCKERASSIGNMENTMASTER { get; set; } = new List<LOCKERASSIGNMENTMASTER>();
    }
}
