using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ePortal.ViewModels.APPX.Separation
{
    public class EditAssociateResignationFormModel
    {
        public DateSelectionViewModel DateSelectionViewModel { get; set; }
        public EmpDtlsViewModel EmpDtlsViewModel { get; set; }
        public APPAUTHDetailsViewModel APPAUTHDetailsViewModel { get; set; }
        public ResignationDataViewModel ResignationDataViewModel { get; set; }
        public SubmitEditAssocResigModel SubmitEditAssocResigModel { get; set; }
    }
    public class DateSelectionViewModel
    {
        public int SelectedDay { get; set; }
        public int SelectedMonth { get; set; }
        public int SelectedYear { get; set; }

        public List<SelectListItem> Days { get; set; }
        public List<SelectListItem> Months { get; set; }
        public List<SelectListItem> Years { get; set; }
    }
    public class EmpDtlsViewModel
    {
        public string lbl_assname { get; set; }
        public string lbl_assecode { get; set; }
        public string lbl_joindate { get; set; }
        public string lbl_desg { get; set; }
        public string lbl_fundesg { get; set; }
        public string lbl_op { get; set; }
        public string lbl_dept { get; set; }
        public string lbl_div { get; set; }
        public string lbl_sec { get; set; }
        public string lbl_site { get; set; }
    }
    public class APPAUTHDetailsViewModel
    {
        public string lbl_appauth { get; set; }
        public string hdauthlevel { get; set; }
        public string hdauthcode { get; set; }
    }
    public class ResignationDataViewModel
    {
        public string txt_subject { get; set; }
        public string txt_body { get; set; }
        public string txt_latestaddress { get; set; }

        public string SelectedOptDay { get; set; }
        public string SelectedOptMonth { get; set; }
        public string SelectedOptYear { get; set; }
    }
    public class SubmitEditAssocResigModel
    {
        public string SelectedDay { get; set; }
        public string SelectedMonth { get; set; }
        public string SelectedMonthText { get; set; }
        public string SelectedYear { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string hdauthcode { get; set; }
        public bool AgreeTerms { get; set; }
        public string ResigID { get; set; }
        public string latestaddress { get; set; }
    }
}
