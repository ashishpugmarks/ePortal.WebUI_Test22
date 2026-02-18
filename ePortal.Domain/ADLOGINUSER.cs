using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ePortal.DomainClasses
{   
    
    public partial class ADLOGINUSER
    {
        [Key]
        public long ADEMPCODE { get; set; }
        public string SALUTATION { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }
        public string? EMAILID { get; set; }
        public DateTime? DOB { get; set; }
        public string? GENDER { get; set; }
        public long? ADDESIGNATIONID { get; set; }
        public long? FUNCTIONALDESIGNATIONID { get; set; }
        public long? SYSITEID { get; set; }
        public long? SYPLANTID { get; set; }
        public short? SYEMPLOYEETYPE { get; set; }
        public short? ACTIVE { get; set; }
        public DateTime? DATEADDED { get; set; }
        public DateTime? DATELSTMODE { get; set; }
        public long? ADDEDBY { get; set; }
        public long? MODIFIEDBY { get; set; }
        public DateTime? REGDATE { get; set; }
        public string? EXTENSIONNO { get; set; }
        public string? TMOBILE { get; set; }
        public long? ADEMPCODE1 { get; set; }

    }
}
