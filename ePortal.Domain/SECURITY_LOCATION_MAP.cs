using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    [Table("SECURITY_LOCATION_MAP")]
    public class SECURITY_LOCATION_MAP
    {
        [Key]
        public long Id { get; set; }
        public long SiteId { get; set; }
        public virtual SYSITE? Site { get; set; }
        public long EmpCode { get; set; }
        public virtual ADEMPLOYEE? Employee { get; set; }
        public short IsActive { get; set; } = 1;
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
