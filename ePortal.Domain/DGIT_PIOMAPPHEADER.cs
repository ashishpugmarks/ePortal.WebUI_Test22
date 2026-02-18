namespace ePortal.DomainClasses
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class DGIT_PIOMAPPHEADER
    {
        [Key]
        public long IOMAPPHEADERID { get; set; }
        public long IOMHEADERID { get; set; }
        public string IOMAPPHEADER { get; set; }
        public string APPHEADERDESC { get; set; }
        public short STATUS { get; set; }
        public long ADDEDBY { get; set; }
        public DateTime ADDEDDATE { get; set; }
        public long? UPDATEBY { get; set; }
        public DateTime? UPDATEDATE { get; set; }
    }
}