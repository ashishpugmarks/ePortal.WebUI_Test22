using ePortal.DomainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class Message
    {
        public string type { get; set; }
        public string text { get; set; }
    }
    public class EmployeeInfo
    {
        public long? ADEMPCODE { get; set; }
        public long? SUPERVISOREMPCODE { get; set; }
        public long? SUPSUPERVISOREMPCODE { get; set; }
        public string? FUNCTIONALDESIGNATION { get; set; }
        public string? SECTION { get; set; }
        public string? DEPARTMENT { get; set; }
        public string? DIVISION { get; set; }
        public string? OPERATION { get; set; }
        public string? NAME { get; set; }
        public string? JOBTITLE { get; set; }
        public string? EMAILID { get; set; }
        public DateTime? DOB { get; set; }
        public string? TMOBILE { get; set; }
    }
}