using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class ProcessMasterViewModel
    {
        public decimal PROCESS_ID { get; set; }
        public string PROCESS_NAME { get; set; }
        public decimal CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public short STATUS { get; set; }
        public string Status1 { get; set; }
    }

   
}
