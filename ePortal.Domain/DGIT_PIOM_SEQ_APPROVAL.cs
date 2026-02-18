using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ePortal.DomainClasses
{
    public class DGIT_PIOM_SEQ_APPROVAL
    {
        [Key]
        public long PARALLEL_GROUP_ID { get; set; }
        public long STAGE_ID { get; set; }      // Belongs to a Stage
        public string GROUP_NAME { get; set; }
        public short STATUS { get; set; }
        public DateTime DATEADDED { get; set; }
        public long ADDEDBY { get; set; }
        public DateTime? UPDATEDATE { get; set; }
        public long? UPDATEBY { get; set; }


    }
}
