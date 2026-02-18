using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.Separation
{
    public class ManageResignationHistoryModel
    {
        public List<ClearanceFormHistoryViewModel> ClearanceFormHistoryViewModel { get; set; }
        public List<PendingApprovalHistoryViewModel> PendingApprovalHistoryViewModel { get; set; }
        public List<ClearHeaderHistoryViewModel> ClearHeaderHistoryViewModel { get; set; }
        public List<DeptClearanceHistoryViewModel> DeptClearanceHistoryViewModel { get; set; }
    }
    public class ClearanceFormHistoryViewModel
    {
        public string ADEMPCODE { get; set; }
        public string EMPNAME { get; set; }
        public string RELIEVING_DATE_AUTH { get; set; }
        public string RESIGNED_DATE { get; set; }
        public string STATUS { get; set; }
        public string RESIGNATIONID { get; set; }
        public string REMARKS { get; set; }
    }
    public class PendingApprovalHistoryViewModel
    {
        public string ADEMPCODE { get; set; }
        public string EMPNAME { get; set; }
        public string RELIEVING_DATE_SELF { get; set; }
        public string RESIGNED_DATE { get; set; }
        public string STATUS { get; set; }
        public string RESIGNATIONID { get; set; }
    }
    public class ClearHeaderHistoryViewModel
    {
        public string Adempcode { get; set; }
        public string EMPNAME { get; set; }
        public string RELIEVING_DATE_AUTH { get; set; }
        public string Resigned_Date { get; set; }
        public string status { get; set; }
        public string Resignationid { get; set; }
    }
    public class DeptClearanceHistoryViewModel
    {
        public string ADEMPCODE { get; set; }
        public string EMPNAME { get; set; }
        public string RELIEVING_DATE_AUTH { get; set; }
        public string RESIGNED_DATE { get; set; }
        public string DPTCLSTATUS { get; set; }
        public string RESIGNATIONID { get; set; }
    }
}
