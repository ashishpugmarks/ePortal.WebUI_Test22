using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.Separation
{
    public class CancellationResignationModel
    {
        public CanResigFillEmpDtlsViewModel CanResigFillEmpDtlsViewModel { get; set; }
    }
    public class CanResigFillEmpDtlsViewModel
    {
        public string lbl_assname { get; set; }
        public string lbl_assecode { get; set; }
        public string lbl_joindate { get; set; }
        public string lbl_reldate { get; set; }
        public string lbl_subject { get; set; }
        public string lbl_reason { get; set; }
        public string strdptrelievingdat { get; set; }
        public string strprocessstatus { get; set; }
        public string lbl_desg { get; set; }
        public string lbl_fundesg { get; set; }
        public string lbl_op { get; set; }
        public string lbl_dept { get; set; }
        public string lbl_div { get; set; }
        public string lbl_sec { get; set; }
        public string lbl_site { get; set; }
    }

}
