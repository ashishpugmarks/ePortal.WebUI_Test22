using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ePortal.DomainClasses
{

    public partial class CM_PROCESSATTACHMENT_TRN
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public CM_PROCESSATTACHMENT_TRN()
        //{
        //    this.CM_PROCESSATTACHMENTAPP_TRN = new HashSet<CM_PROCESSATTACHMENTAPP_TRN>();
        //}

        [Key]
        public long ATTACHMENTID { get; set; }
        public long PROCESSID { get; set; }
        public string? SUBJECT { get; set; }
        public string? BRIEF { get; set; }
        public string? DESCRIPTION { get; set; }
        public byte[]? BANNER { get; set; }
        public string? BANNER_CONTENTTYPE { get; set; }
        public string? BANNER_NAME { get; set; }
        public byte[]? ATTACHMENT1 { get; set; }
        public string? ATTACHMENT1_CONTENTTYPE { get; set; }
        public string? ATTACHMENT1_NAME { get; set; }
        public byte[]? ATTACHMENT2 { get; set; }
        public string? ATTACHMENT2_CONTENTTYPE { get; set; }
        public string? ATTACHMENT2_NAME { get; set; }
        public System.DateTime START_DATE { get; set; }
        public System.DateTime END_DATE { get; set; }
        public short STATUS { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public long CREATED_BY { get; set; }
        public long? MODIFIED_BY { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
        public string? FUNCTIONAL_DESIGNATION { get; set; }
        public string? DESIGNATION { get; set; }
        public string? SITE { get; set; }
        public string? CIRCULARENO { get; set; }
        public short? EMAILSTATUS { get; set; }

        //public virtual CM_PROCESS_MST CM_PROCESS_MST { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<CM_PROCESSATTACHMENTAPP_TRN> CM_PROCESSATTACHMENTAPP_TRN { get; set; }
    }
}
