using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    public partial class LOCKERMASTER
    {
        public decimal LOCKER_ID { get; set; }

        public decimal FLOOR_ID { get; set; }

        public string LOCKER_CODE { get; set; }

        public decimal? STATUS { get; set; }

        public DateTime? CREATED_DATE { get; set; }

        public virtual FLOORMASTER FLOOR { get; set; }

        public virtual ICollection<LOCKERBOXMASTER> LOCKERBOXMASTER { get; set; } = new List<LOCKERBOXMASTER>();
    }
}
