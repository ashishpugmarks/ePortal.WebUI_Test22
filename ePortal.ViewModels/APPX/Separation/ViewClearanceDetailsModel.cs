using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.Separation
{
    public class ViewClearanceDetailsModel
    {
        public ClearanceDetailsViewModel ClearanceDetailsViewModel { get; set; }
        public List<ResignationDetailViewModel> ResignationDetailViewModel { get; set; }
    }
    public class ClearanceDetailsViewModel
    {
        public string lbl_assname { get; set; }
        public string lbl_assecode { get; set; }
        public string lbl_joindate { get; set; }
        public string lbl_desg { get; set; }
        public string lbl_fundesg { get; set; }
        public string lbl_phone { get; set; }
        public string lbl_emailid { get; set; }
        public string lbl_op { get; set; }
        public string lbl_div { get; set; }
        public string lbl_dept { get; set; }
        public string lbl_sec { get; set; }
        public string lbl_site { get; set; }
        public string lbl_applydate { get; set; }
        public string lbl_reldate { get; set; }
        public string lbl_deptreldate { get; set; }
        public string lbl_subject { get; set; }
        public string lbl_reason { get; set; }
    }
    public class ResignationDetailViewModel
    {
        public string Description { get; set; }
        public string Remarks { get; set; }
        public string Ename { get; set; }
        public string SubmitBy { get; set; }
        public string SubmittedDate { get; set; }
        public string Status { get; set; }
        public List<SubHeaderDetail> SubHeaderDetails { get; set; }
    }
    public class SubHeaderDetail
    {
        public string Description { get; set; }
        public string Amount { get; set; }
        public string Remarks { get; set; }
        public string SubmitBy { get; set; }
        public string SubmittedDate { get; set; }
        public string Status { get; set; }
        public string AttachDocument { get; set; }
    }


}
