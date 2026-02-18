using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    public class CONTINUOUSATTENDANCELOG
    {
        [Column("ID")]
        public long ID { get; set; }

        [Column("ADEMPCODE")]
        public long ADEMPCODE { get; set; }

        [Column("TDATE")]
        public DateTime TDATE { get; set; }

        [Column("CREATED_DATE")]
        public DateTime CREATED_DATE { get; set; }

        [Column("CREATED_BY")]
        public long CREATED_BY { get; set; }

        [Column("TDAYS")]
        public int? TDAYS { get; set; }
    }
}
