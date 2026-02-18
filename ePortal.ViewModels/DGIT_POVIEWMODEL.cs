using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class DGIT_POVIEWMODEL
    {

        public Int64 POID { get; set; }
        public string PONO { get; set; }
        public string RequestorName { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string VendorName { get; set; }
        public DateTime Requestdate { get; set; }
        public string URGENCY { get; set; }
        public bool ISSELECTED { get; set; }

    }

    public class DGIT_POREQVIEWMODEL
    {

        public Int64 POID { get; set; }
        public string PONO { get; set; }
        public string RequestorName { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string VendorName { get; set; }
        public string PODesc { get; set; }
        public string Remark { get; set; }
        public DateTime Requestdate { get; set; }
        public List<POFileViewModel> POFile { get; set; }
        public List<POAppHistory> POApprovalHis { get; set; }
        public string DSCTOKENID { get; set; }
        public string DGLOC { get; set; }
    }
    public class POFileViewModel
    {
        public String FileName { get; set; }
        public String DocumentType { get; set; }
        public String FileContentType { get; set; }
        public byte[] File { get; set; }
    }
    public class POAppHistory
    {
        public String ApprovedBy { get; set; }
        public String ApprovalDate { get; set; }
        public String ApprovalRemark { get; set; }
        public String ApprovalName { get; set; }
    }
    public class POApprovalStatus
    {
        public String POID { get; set; }
        public String ApprovalRemark { get; set; }
        public String Approveby { get; set; }

        public String Approvalstatus { get; set; }
    }
}
