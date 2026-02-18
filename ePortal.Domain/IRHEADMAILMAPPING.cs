using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    public class IRHEADMAILMAPPING
    {
        [Column("ID")]
        public long ID { get; set; }

        [Column("SYPLANT")]
        public long SYPLANT { get; set; }

        [Column("TO_ADEMPCODE")]
        public long TO_ADEMPCODE { get; set; }

        [Column("CC_ADEMPCODE")]
        public long CC_ADEMPCODE { get; set; }

        [Column("CREATED_DATE")]
        public DateTime CREATED_DATE { get; set; }

        [Column("CREATED_BY")]
        public long CREATED_BY { get; set; }

        [Column("ACTIVE")]
        public long ACTIVE { get; set; }
    }
}
