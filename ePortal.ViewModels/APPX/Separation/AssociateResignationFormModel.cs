using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ePortal.ViewModels.APPX.Separation
{
    public class AssociateResignationFormModel
    {
        public AssRegFromDateSelectionViewModel AssRegFromDateSelectionViewModel { get; set; }
        public AssRegFromEmpDtlsViewModel AssRegFromEmpDtlsViewModel { get; set; }
        public AssRegFromAPPAUTHDetailsViewModel AssRegFromAPPAUTHDetailsViewModel { get; set; }
        public AssRegFromResignationDataViewModel AssRegFromResignationDataViewModel { get; set; }
        public AssRegFromSubmitEditAssocResigModel AssRegFromSubmitEditAssocResigModel { get; set; }
    }
    public class AssRegFromDateSelectionViewModel
    {
        public int SelectedDay { get; set; }
        public int SelectedMonth { get; set; }
        public int SelectedYear { get; set; }

        public List<SelectListItem> Days { get; set; }
        public List<SelectListItem> Months { get; set; }
        public List<SelectListItem> Years { get; set; }
    }
    public class AssRegFromEmpDtlsViewModel
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
    public class AssRegFromAPPAUTHDetailsViewModel
    {
        public string lbl_appauth { get; set; }
        public string hdauthlevel { get; set; }
        public string hdauthcode { get; set; }
        public string hdauthemailid { get; set; }
        public string hdauthname { get; set; }
    }
    public class AssRegFromResignationDataViewModel
    {
        public string txt_subject { get; set; }
        public string txt_body { get; set; }
        public string txt_latestaddress { get; set; }

        public string SelectedOptDay { get; set; }
        public string SelectedOptMonth { get; set; }
        public string SelectedOptYear { get; set; }
    }
    public class AssRegFromSubmitEditAssocResigModel
    {
        public string SelectedDay { get; set; }
        public string SelectedMonth { get; set; }
        public string SelectedMonthText { get; set; }
        public string SelectedYear { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string hdauthcode { get; set; }
        public string hdauthlevel { get; set; }
        public string hdauthemailid { get; set; }
        public string hdauthname { get; set; }
        public bool AgreeTerms { get; set; }
        public string ResigID { get; set; }
        public string latestaddress { get; set; }
        public string lbl_appauth { get; set; }
        public string lbl_assname { get; set; }
        public string lbl_assecode { get; set; }
    }
}
