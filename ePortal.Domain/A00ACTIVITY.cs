namespace ePortal.DomainClasses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class A00ACTIVITY
    {
        //public A00ACTIVITY()
        //{
        //    A00ACTIVITYHISTORY = new HashSet<A00ACTIVITYHISTORY>();
        //}
        [Key]
        public decimal ID { get; set; }
        public string ACTIVITYNAME { get; set; }
        public string ACTIVITYDETAILS { get; set; }
        public Nullable<System.DateTime> ACTIVITYSTARTDATE { get; set; }
        public Nullable<System.DateTime> ACTIVITYENDDATE { get; set; }
        public Nullable<decimal> ACTIONBY { get; set; }
        public Nullable<System.DateTime> ACTIONON { get; set; }
        public Nullable<short> ACTIVE { get; set; }
        public Nullable<short> STATUS { get; set; }
        public Nullable<short> ACTIVITYSCHEDULE { get; set; }
        public Nullable<long> A00DTLTBID { get; set; }
        public Nullable<long> SYKID { get; set; }
        public Nullable<int> DIVISIONID { get; set; }
        public Nullable<int> OPERATIONID { get; set; }
        public Nullable<int> DEPARTMENTID { get; set; }
        public Nullable<int> SECTIONID { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<A00ACTIVITYHISTORY> A00ACTIVITYHISTORY { get; set; }
    }
}