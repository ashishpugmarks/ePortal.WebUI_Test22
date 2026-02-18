using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class NewsLetterHomeViewModel  //Added by Bhupesh - NTT for CR-4894
    {
        public long ID { get; set; }
        public string DESCRIPTION { get; set; }
        public string DOCUMENT_NAME { get; set; }
        public string STATUS { get; set; }
        public string ADDED_BY { get; set; }
        public Nullable<System.DateTime> ADDED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
    }
}
