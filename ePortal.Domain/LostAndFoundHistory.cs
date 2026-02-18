using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.DomainClasses
{
    [Table("LOST_AND_FOUND_HISTORY")]
    public class LostAndFoundHistory
    {
        [Key]
        public long ID { get; set; }

        public long LOST_AND_FOUND_ID { get; set; }

        public short LAF_STATUS { get; set; }  

        public string? REMARKS { get; set; }

        public long? ADEMPCODE { get; set; }

        public DateTime CHANGEDDATE { get; set; } = DateTime.Now;
        public long ADDEDBY { get; set; }
        public DateTime ADDEDDATE { get; set; }
        public long? UPDATEBY { get; set; }
        public DateTime? UPDATEDATE { get; set; }
    }
}
