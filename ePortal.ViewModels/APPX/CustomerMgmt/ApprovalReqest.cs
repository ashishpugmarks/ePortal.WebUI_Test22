using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.CustomerMgmt
{
    public class ApprovalReqest
    {
        public long? UserId { get; set; }
        public long? cmheadid { get; set; }
        public string? request_no { get; set; }
        public int? action { get; set; }
        public string? Remarks {  get; set; }
        public string? Customer_Code { get; set; }
        public string? Name1 { get; set; }
    }
    public class ApprovalResponse
    {
        public int? RS { get; set; } = 0;
        public string? message { get; set; }
        public string? redirecturl { get; set; }
    }
}
