namespace ePortal.DomainClasses
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class DGIT_PIOMAPPAUTHSEQ
    {
        [Key]
        public long IOMAPPAUTH_ID { get; set; }
        public long PARALLEL_GROUP_ID { get; set; }

        public long IOMHEADERID { get; set; }
        public long ADEMPCODE { get; set; }
        public short APP_SEQ { get; set; }
        public short STATUS { get; set; }
        public long ADDEDBY { get; set; }
        public DateTime ADDEDDATE { get; set; }
        public long? UPDATEBY { get; set; }
        public DateTime? UPDATEDATE { get; set; }
        public short APPTYPE { get; set; }
        public string IOMAPPHEADER { get; set; }
    }
}
