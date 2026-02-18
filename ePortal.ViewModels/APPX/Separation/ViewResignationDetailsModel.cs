using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.Separation
{
    public class ViewResignationDetailsModel
    {
        public ViewEmpDetailsModel EmployeeDetails { get; set; }
        public List<ApprovalDetailModel> ApprovalDetails { get; set; }
        
    }
    public class ViewEmpDetailsModel
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
        public string lblPrintedby { get; set; }
    }
    public class ApprovalDetailModel
    {
        public string APPNAME { get; set; }        
        public string APPECODE { get; set; }     
        public string DESIG { get; set; }      
        public string STATUS { get; set; }           
        public string APPSUBMITEDATE { get; set; }    
        public string APPREMARK { get; set; }         
    }
}
