namespace ePortal.DomainClasses
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class DGIT_PIOMREPORT
    {
        [Key]
        public long IOMHEADERID { get; set; }
        public string IOM_DESC { get; set; }
        public short STATUS { get; set; }
        public short PROCESS_STATUS { get; set; }
        public DateTime DATEADDED { get; set; }
        public long ADDEDBY { get; set; }
        public DateTime? UPDATEDATE { get; set; }
        public long? UPDATEDBY { get; set; }
        public short APP_TYPE { get; set; }
        public long? IOMCATID { get; set; }
        public decimal SYKIID { get; set; }
        public string KICODE { get; set; }
        public long? OPERATIONID { get; set; }
        public string OPERATION { get; set; }
        public long? DIVISIONID { get; set; }
        public string DIVISION { get; set; }
        public long? DEPARTMENTID { get; set; }
        public string DEPARTMENT { get; set; }
        public long? SECTIONID { get; set; }
        public string SECTION { get; set; }
        public string CATDESC { get; set; }
        public string ADDEDBYNAME { get; set; }
    }
}