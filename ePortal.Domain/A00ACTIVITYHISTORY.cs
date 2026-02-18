namespace ePortal.DomainClasses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class A00ACTIVITYHISTORY
    {
        [Key]
        public decimal ID { get; set; }
        public Nullable<decimal> A00ACTIVITYID { get; set; }
        public string REMARKS { get; set; }
        public Nullable<short> ACTIVITYSCHEDULE { get; set; }
        public Nullable<decimal> ACTIONBY { get; set; }
        public Nullable<System.DateTime> ACTIONON { get; set; }
        public Nullable<short> ACTIVE { get; set; }

        //public virtual A00ACTIVITY A00ACTIVITY { get; set; }
    }
}


