using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ePortal.DomainClasses
{
    
    
    public partial class ADEMPLOYEE
    {
        [Key]
        public long ADEMPCODE { get; set; }
        public string? SALUTATION { get; set; }
        public string? FIRSTNAME { get; set; }
        public string? LASTNAME { get; set; }
        public string? CALLINGNAME { get; set; }
        public string? EMAILID { get; set; }
        public DateTime? DOB { get; set; }
        public string? GENDER { get; set; }
        public string? TPHONE { get; set; }
        public string? TMOBILE { get; set; }
        public string? BLOODGROUP { get; set; }
        public System.DateTime REGDATE { get; set; }
        public System.DateTime EXPIRYDATE { get; set; }
        public short ACTIVE { get; set; }
        public System.DateTime? DATEADDED { get; set; }
        public DateTime? DATELSTMODE { get; set; }
        public long ADDEDBY { get; set; }
        public long? MODIFIEDBY { get; set; }
        public string? JOBTITLE { get; set; }
        public string? EXTENSIONNO { get; set; }
        public string? DIRECTLANDLINENO { get; set; }
        public long? LOCATION { get; set; }
        public long SYEMPLOYEETYPEID { get; set; }
        public string? OPERATION { get; set; }
        public string? JACKETSIZE { get; set; }
        public string? TROUSERSIZE { get; set; }
        public string? SHOESIZE { get; set; }
        public long? SYSITEID { get; set; }
        public string? ESSTQUAL { get; set; }
        public DateTime? DOC { get; set; }
        public decimal? PASTEXP { get; set; }
        public string? EMERGENCYCONTNO { get; set; }
        public string? LOCKERNO { get; set; }
        public string? SHIRTSIZE { get; set; }
        public string? TSHIRTSIZE { get; set; }
        public string? WARMERLWSIZE { get; set; }
        public string? WARMERUPSIZE { get; set; }
        public DateTime? CONFIRMATIONDATE { get; set; }
        public string? EXTENSIONDATE { get; set; }
        public string? PANCARDNO { get; set; }
        public string? CONFIRMATIONTXT { get; set; }
        public string? WINTERJACKETSIZE { get; set; }
        public string? MARITALSTATUS { get; set; }
        public DateTime? MARITALDATE { get; set; }
        public string? EMPLTYPE { get; set; }

    }
}
