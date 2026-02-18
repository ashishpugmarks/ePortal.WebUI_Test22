using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ePortal.ViewModels
{
    public class ContineousWorkingViewModel
    {
        public string? Date { get; set; }
    }
    public class SearchContineousWorkingViewModel
    {
        public string? DATETO { get; set; }
        public long? DIVISIONID { get; set; }
        public string? DIVISION { get; set; }
        public long? DEPARTMENTID { get; set; }
        public string? DEPARTMENT { get; set; }
    }
    public class ContineousWorkingRowViewModel
    {
        public long ADEMPCODE { get; set; }
        public string? ASSOCIATE_NAME { get; set; }
        public string? OPERATION { get; set; }
        public string? DIVISION { get; set; }
        public string? DEPARTMENT { get; set; }
        public string? SECTION { get; set; }
        public string? DESIGNATION { get; set; }
        // public string? LOCATION { get; set; }
        public string? SITE_DESCRIP { get; set; }
        public string? STATUS { get; set; }
        public string? CONTINUOUS_WORKING_FROM { get; set; }
        public string? DATETO { get; set; }
        public int CONTINUOUSDAYS { get; set; }
        public string? PUNCHDATE { get; set; }
        public string? PunchIN { get; set; }
        public string? PunchOUT { get; set; }
        public string? SHIFT { get; set; }
    }

}

