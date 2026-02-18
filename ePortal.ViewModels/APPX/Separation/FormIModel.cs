using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.Separation
{
    public class FormIModel
    {
        public FormIEmpDtlsViewModel FormIEmpDtlsViewModel { get; set; }
        public FormISubmitViewModel FormISubmitViewModel { get; set; }
    }
    public class FormIEmpDtlsViewModel
    {
        public string lbl_resignationdate { get; set; }
        public string lbl_assname { get; set; }
        public string lbl_assecode { get; set; }
        public string lbl_daddress { get; set; }
        public string lbl_org { get; set; }
        public string lbl_ecodedesg { get; set; }
        public string lbl_joindate { get; set; }
        public string lbl_Relivingdate { get; set; }
        public string lbl_serviceperiod { get; set; }
        public string lbl_basicamt { get; set; }
        public string lbl_gratuityamt { get; set; }
    }
    public class FormISubmitViewModel
    {
        [Required(ErrorMessage = "Email ID is mandatory")]
        [EmailAddress(ErrorMessage = "Email ID must contain '@' and be a valid format")]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "You must agree to the terms")]
        public bool Agree { get; set; }

        public string ResigID { get; set; }
        public string BasicAmt { get; set; }
        public string GratuityAmt { get; set; }
        public string RelievingDate { get; set; }
    }

}
