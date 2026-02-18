using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.CustomerMgmt
{
    public class CMMASTER_DATA
    {
        [Key]
        [Required]
        [StringLength(10)]
        public string Code { get; set; }

        [Required]
        [StringLength(150)]
        public string CodeDesc { get; set; }

        [Required]
        [StringLength(100)]
        public string GroupName { get; set; }

        [Required]
        [StringLength(100)]
        public string PgroupName { get; set; }

        [Required]
        [StringLength(10)]
        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        [StringLength(1)]
        public string IsActive { get; set; }

        [Range(0, 99999)]
        public int? CmdId { get; set; }
    }

    public class CustomerMgmtResponse
    {
        public string? code { get; set; }
        public string? message { get; set; }

    }
}
