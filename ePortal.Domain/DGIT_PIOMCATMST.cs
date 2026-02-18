namespace ePortal.DomainClasses
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class DGIT_PIOMCATMST
    {
        [Key]
        public long IOMCATMSTID { get; set; }
        public string CATDESC { get; set; }
        public short STATUS { get; set; }
        public DateTime DATEADDED { get; set; }
        public long ADDEDBY { get; set; }
        public short ISHIGHLIGHTED { get; set; }
    }
}