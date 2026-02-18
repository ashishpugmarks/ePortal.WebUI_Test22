using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    public partial class CSD_FEEDBACK_IMAGE
    {
        [Key]
        [Column("FEEDBACK_IMAGE_ID")]
        public int FEEDBACK_IMAGE_ID { get; set; }

        [Column("BOOKING_ID")]
        public long BOOKING_ID { get; set; }

        [Column("MEALTYPEID")]
        public long MEALTYPEID { get; set; }

        [Column("EVIDENCE_IMAGE_PATH")]
        [StringLength(500)]
        public string? EVIDENCE_IMAGE_PATH { get; set; }

        [Column("ADDEDDATE")]
        public DateTime? ADDEDDATE { get; set; }

        [Column("ADDEDBY")]
        public long ADDEDBY { get; set; }
    }
}
