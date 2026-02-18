using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class AutoRemindersExcludeDesignationMaster
    {
        public int SRNO { get; set; }
        public int DESIGNATION_ID { get; set; }
        public string DESIGNATION_DESCRIPTION { get; set; }
        public string IS_EXCLUDE { get; set; }
        public string SelectedIds { get; set; } 
    }
}
