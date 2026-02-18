using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ePortal.DomainClasses { 
    public class DGIT_PIOM_STAGE
    {
        [Key]
        public long STAGE_ID { get; set; }

        public string STAGE_NAME { get; set; }
        public long IOMHEADERID { get; set; }  
        public int STAGE_ORDER { get; set; }    
        public short STATUS { get; set; }
        public DateTime DATEADDED { get; set; }
        public long ADDEDBY { get; set; }
        public DateTime? UPDATEDATE { get; set; }
        public long? UPDATEBY { get; set; }
        public short ADDITIONALTYPE { get; set; }

    }
}
