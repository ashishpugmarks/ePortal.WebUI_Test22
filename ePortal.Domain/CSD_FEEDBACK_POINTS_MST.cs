using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    public partial class CSD_FEEDBACK_POINTS_MST
    {
        [Key]
        [Column("FEEDBACK_POINT_ID")]
        public int FEEDBACK_POINT_ID { get; set; }

        [Required]
        [Column("POINT_NAME")]
        [StringLength(100)]
        public string POINT_NAME { get; set; }

        [Column("ISACTIVE")]
        [StringLength(1)]
        public string ISACTIVE { get; set; } = "Y";
    }
}
