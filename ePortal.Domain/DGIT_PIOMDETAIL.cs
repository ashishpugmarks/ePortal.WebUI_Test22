namespace ePortal.DomainClasses
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class DGIT_PIOMDETAIL
    {
        [Key]
        public long IOMDTL_ID { get; set; }
        public long IOMHEADERID { get; set; }
        public short STATUS { get; set; }
        public long ADDEDBY { get; set; }
        public DateTime ADDEDDATE { get; set; }
        public long? UPDATEDBY { get; set; }
        public DateTime? UPDATEDATE { get; set; }
        public string DOC_TYPE { get; set; }
        public string FILENAME { get; set; }
        public string? ADDITIONAL_INFO { get; set; }
    }
}