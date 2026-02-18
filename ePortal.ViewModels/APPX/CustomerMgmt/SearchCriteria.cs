using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.CustomerMgmt
{
    public class SearchCriteria
    {
        public string? REQ_TYPE { get; set; }
        public string? ACC_GRP { get; set; }
        public string? DATE_FROM { get; set; }
        public string? DATE_TO { get; set; }
        public string? CUSTOMER_CODE { get; set; }
        public string? STATUS { get; set; }
        public string? CUSTOMER_NAME { get; set; }
        public string? REQUEST_NO { get; set; }
    }
}

