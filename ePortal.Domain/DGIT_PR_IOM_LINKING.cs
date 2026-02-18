using System.ComponentModel.DataAnnotations;

namespace ePortal.DomainClasses
{
     public class DGIT_PR_IOM_LINKING
    {
        [Key]
        public long DPIL_ID { get; set; }
        public long DPIL_IOM_HDR_ID { get; set; } 
        public string DPIL_INDENT_NO { get; set; }
        public string? COLUMN1 { get; set; }
        public DateTime? COLUMN2 { get; set; }
        public string? COLUMN3 { get; set; }
        public DateTime? DPIL_UPDATEDATE { get; set; }
        public int DPIL_STATUS { get; set; }
    }

}
