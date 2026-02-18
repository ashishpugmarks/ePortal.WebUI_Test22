using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.Separation
{
    public class ManageResignationRequestModel
    {
        public List<ClearanceFormViewModel> ClearanceFormViewModel { get; set; }
        public List<PendingApprovalViewModel> PendingApprovalViewModel { get; set; }
        public List<ClearHeaderViewModel> ClearHeaderViewModel { get; set; }
        public List<DeptClearanceViewModel> DeptClearanceViewModel { get; set; }
        public HideShowClearanceViewModel HideShowClearanceViewModel { get; set; }
    }
    public class ClearanceFormViewModel
    {
        public string ADEMPCODE { get; set; }     
        public string EMPNAME { get; set; }       
        public string RELIEVING_DATE_AUTH { get; set; } 
        public string RESIGNED_DATE { get; set; } 
        public string CLSTATUS { get; set; }     
        public string RESIGNATIONID { get; set; }
    }
    public class PendingApprovalViewModel
    {
        public string ADEMPCODE { get; set; }
        public string EMPNAME { get; set; }
        public string RELIEVING_DATE_SELF { get; set; }
        public string RESIGNED_DATE { get; set; }
        public string STATUS { get; set; }
        public string RESIGNATIONID { get; set; }
    }
    public class ClearHeaderViewModel
    {
        public string Adempcode { get; set; }
        public string EMPNAME { get; set; }
        public string RELIEVING_DATE_AUTH { get; set; }
        public string Resigned_Date { get; set; }
        public string status { get; set; }
        public string Resignationid { get; set; }
    }
    public class DeptClearanceViewModel
    {
        public string ADEMPCODE { get; set; }
        public string EMPNAME { get; set; }
        public string RELIEVING_DATE_AUTH { get; set; }
        public string RESIGNED_DATE { get; set; }
        public string DPTCLSTATUS { get; set; }
        public string RESIGNATIONID { get; set; }
    }
    public class HideShowClearanceViewModel
    {
        public bool ShowHeaderList { get; set; }
        public bool ShowSubHeaderList { get; set; }
    }

}
