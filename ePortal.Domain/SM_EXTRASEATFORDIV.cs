
namespace ePortal.DomainClasses
{
    using System;        
    using System.ComponentModel.DataAnnotations;
    public partial class SM_EXTRASEATFORDIV
    {
        [Key]
        public long SRNO { get; set; }
        public long DIVISIONID { get; set; }
        public long EXTRASEATCOUNT { get; set; }
        public short STATUS { get; set; }
        public long ADDEDBY { get; set; }
        public DateTime ADDEDON { get; set; }
        public long? UPDATEDBY { get; set; }
        public DateTime? UPDATEDON { get; set; }
    }
}
