namespace ePortal.DomainClasses
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class DGIT_PIOMAPPHISTORY
    {
        [Key]
        public long IOMAPPHISTORY_ID { get; set; }
        public long IOMHEADERID { get; set; }
        public long ADEMPCODE { get; set; }
        public short APPROVAL_STATUS { get; set; }
        public string? APPROVAL_REMARK { get; set; }

        public long PIOMAPPAUTHSEQ_ID { get; set; }
        public long ADDEDBY { get; set; }
        public DateTime ADDEDDATE { get; set; }
        public long? UPDATEBY { get; set; }
        public DateTime? UPDATEDATE { get; set; }
        public DateTime? APP_DATE { get; set; }
        public short APPTYPE { get; set; }
    }
}