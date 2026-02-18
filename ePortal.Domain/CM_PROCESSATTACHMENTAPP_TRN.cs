using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ePortal.DomainClasses
{    
    public partial class CM_PROCESSATTACHMENTAPP_TRN
    {
        [Key]
        public long ATTACHMENTAPPID { get; set; }
        public long ATTACHMENTID { get; set; }
        public short STATUS { get; set; }
        public long INITIATED_BY { get; set; }
        public DateTime INITIATED_DATE { get; set; }
        public string? INITIATOR_REMARKS { get; set; }
        public long? APPAUTH1_ECODE { get; set; }
        public DateTime? APPAUTH1_DATE { get; set; }
        public string? APPAUTH1_REMARKS { get; set; }
        public long? APPAUTH2_ECODE { get; set; }
        public DateTime? APPAUTH2_DATE { get; set; }
        public string? APPAUTH2_REMARKS { get; set; }
        public long? HRAPP_ECODE { get; set; }
        public DateTime? HRAPP_DATE { get; set; }
        public string? HRAPP_REMARKS { get; set; }
    
        //public  ADLOGINUSER ADLOGINUSER { get; set; }
        //public  ADLOGINUSER ADLOGINUSER1 { get; set; }
        //public  ADLOGINUSER ADLOGINUSER2 { get; set; }
        //public  ADLOGINUSER ADLOGINUSER3 { get; set; }
        //public  CM_PROCESSATTACHMENT_TRN CM_PROCESSATTACHMENT_TRN { get; set; }
    }
}
