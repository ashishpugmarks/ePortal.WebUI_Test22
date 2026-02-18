using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    public partial class CSD_FEEDBACK_DETAILS_TRN
    {
        [Key]
        [Column("FEEDBACK_DETAIL_ID")]
        public int FEEDBACK_DETAIL_ID { get; set; }

        [Column("BOOKING_ID")]
        public long BOOKING_ID { get; set; }
        public long MEALTYPEID { get; set; }

        [Column("FEEDBACK_POINT_ID")]
        public int FEEDBACK_POINT_ID { get; set; }

        [Column("RATING")]
        public int? RATING { get; set; }

        [Column("REMARK")]
        [StringLength(500)]
        public string? REMARK { get; set; }

        [Column("ADDEDBY")]
        [StringLength(20)]
        public long ADDEDBY { get; set; }

        [Column("ADDEDDATE")]
        public DateTime? ADDEDDATE { get; set; }

        [Column("UPDATEDBY")]
        [StringLength(20)]
        public string? UPDATEDBY { get; set; }

        [Column("UPDATEDDATE")]
        public DateTime? UPDATEDDATE { get; set; }
    }
}
