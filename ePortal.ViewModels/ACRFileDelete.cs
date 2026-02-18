using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class ACRFileDelete
    {
        public string fileName { get; set; }
        public string docType { get; set; }
        public long ParentID { get; set; }
        public string? PA_ID { get; set; }
    }
    public class ACRRegenerate
    {
        public string id { get; set; }
        
    }
}
