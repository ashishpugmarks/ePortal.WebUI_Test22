using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.Separation
{
    public class SetDepartmentClearanceModel
    {
        public SetDeptClrEmpDtlsViewModel SetDeptClrEmpDtlsViewModel { get; set; }
        public SubmitSetDeptClrEmpDtlsViewModel SubmitSetDeptClrEmpDtlsViewModel { get; set; }
    }
    public class SetDeptClrEmpDtlsViewModel
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
    public class SubmitSetDeptClrEmpDtlsViewModel
    {
        public string ConfDoc { get; set; }
        public string OtherDoc { get; set; }
        public string LibraryBook { get; set; }
        public string ItAssest { get; set; }
        public string Camere { get; set; }
        public string Key { get; set; }
        public string TravelBill { get; set; }
        public string BasicSal { get; set; }
        public string Regid_Ecode { get; set; }
    }
}
