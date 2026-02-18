using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    public class T_HR_CHRONICLES
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HRC_ROWID { get; set; }

        [Required]
        [MaxLength(200)]
        public string HRC_TITLE { get; set; }

        [MaxLength(500)]
        public string? HRC_DESCRIPTION { get; set; }

        public DateTime? HRC_VALID_FROM { get; set; }

        public DateTime? HRC_VALID_TILL { get; set; }

        [Required]
        [MaxLength(500)]
        public string HRC_DOC_NAME { get; set; }

        [Required]
        [MaxLength(100)]
        public string HRC_DOC_TYPE { get; set; }

        [Required]
        [MaxLength(50)]
        public string HRC_CREATED_BY { get; set; }

        [Required]
        public DateTime HRC_CREATED_DATE { get; set; }

        [MaxLength(50)]
        public string? HRC_UPDATED_BY { get; set; }

        public DateTime? HRC_UPDATED_DATE { get; set; }

        [Required]
        [MaxLength(50)]
        public string HRC_STATUS { get; set; } = "Active";
    }
}
