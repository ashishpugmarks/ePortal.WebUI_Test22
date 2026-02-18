using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.Separation
{
    public class ClearanceFullFinalformModel
    {
       public ClearFullFinalEmpDtlsViewModel ClearFullFinalEmpDtlsViewModel { get; set; }
        public List<ClearanceDetailViewModel> ClearanceDetailViewModel { get; set; }
    }
    public class ClearFullFinalEmpDtlsViewModel
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
        public string lbl_basicsal { get; set; }
    }
    public class ClearanceDetailViewModel
    {
        public string CLEARENCEHEADERID { get; set; }
        public string DESCRIPTION { get; set; }
        public string AMOUNT { get; set; }
        public string REMARKS { get; set; }
        public int STATUS { get; set; } 
        public IFormFile FileUpload { get; set; }
        public string ATTACHDOCUMENT { get; set; }
    }

}
