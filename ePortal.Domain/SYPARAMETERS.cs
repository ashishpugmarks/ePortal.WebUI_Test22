using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ePortal.DomainClasses
{
    public partial class SYPARAMETERS
    {
        [Key]
        public long SYPARAMETERSID { get; set; }
        public string? PARAMNAME { get; set; }
        public string? PARAMDESCRIPTION { get; set; }
        public string? PARAMVALUE { get; set; }
        public System.DateTime DATEADDED { get; set; }
        public long? ADDEDBY { get; set; }
        public long? MODIFIEDBY { get; set; }
        public DateTime? DATELSTMOD { get; set; }
        public string? USED_IN { get; set; }
        public string? MODULE { get; set; }
    }
}
