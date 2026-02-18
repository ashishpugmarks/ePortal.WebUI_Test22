using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class UserViewModel
    {
        public string UserId { get; set; }
        public string UserType { get; set; }  //User, Admin
        public string NetworkSource { get; set; } //Intranet, Internet, Both 
        public string OperatorId { get; set; }
        public string FnDesignation { get; set; } //check the datatype and convert 
        public string Designation { get; set; } //check the datatype and convert 
        public string SourceLocation { get; set; } //check the datatype and convert 
        public string OrgStructure { get; set; } //check the datatype and convert 
        public int BPOId { get; set; } //check the datatype and convert 

    }
}
