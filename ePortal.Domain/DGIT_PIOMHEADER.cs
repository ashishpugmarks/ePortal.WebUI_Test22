namespace ePortal.DomainClasses
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class DGIT_PIOMHEADER
    {
        [Key]
        public long IOMHEADERID { get; set; }
        public string IOM_DESC { get; set; }
        public short STATUS { get; set; }
        public short PROCESS_STATUS { get; set; }
        public DateTime DATEADDED { get; set; }
        public long ADDEDBY { get; set; }
        public DateTime? UPDATEDATE { get; set; }
        public long? UPDATEDBY { get; set; }
        public short APP_TYPE { get; set; }
        public long? IOMCATID { get; set; }
        public short ISEDITABLE { get; set; }
    }
}