using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.Separation
{
    public class ClearanceFullFinalHeaderModel
    {
        public ClrFullFinalHeaderEmpDtlsViewModel ClrFullFinalHeaderEmpDtlsViewModel { get; set; }
        public List<ClrFullFinalHeaderViewModel> ClrFullFinalHeaderViewModel { get; set; }
    }
    public class ClrFullFinalHeaderEmpDtlsViewModel
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
    public class ClrFullFinalHeaderViewModel
    {
        public int CLEARENCEHEADERID { get; set; }    
        public string DESCRIPTION { get; set; }      
        public int CLEAHEADRDETAILID { get; set; }  
        public List<ClrFullFinalSubHeaderViewModel> SubHeaderDetails { get; set; }
        public bool CanSubmit { get; set; }
        public string status { get; set; }
        public string remarks { get; set; }
    }
    public class ClrFullFinalSubHeaderViewModel
    {
        public string DESCRIPTION { get; set; }       
        public decimal Amount { get; set; }       
        public string REMARKS { get; set; }       
        public string SUBMITBY { get; set; }       
        public string SUBMITTEDATE { get; set; }     
        public string STATUS { get; set; }           
        public string ATTACHDOCUMENT { get; set; }   
    }


}
