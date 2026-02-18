using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ePortal.ViewModels.APPX.Separation
{
    public class ApprovalFormModel
    {
        public ApprvFrmDateTimeViewModel ApprvFrmDateTimeViewModel { get; set; }
        public ApprvFormEmpDetailsViewModel ApprvFormEmpDetailsViewModel { get; set; }
        public List<ApprvFormApprovalDtlsViewModel> ApprvFormApprovalDtlsViewModel { get; set; }
    }
    public class ApprvFrmDateTimeViewModel
    {
        public int SelectedDay { get; set; }
        public int SelectedMonth { get; set; }
        public int SelectedYear { get; set; }

        public List<SelectListItem> Days { get; set; }
        public List<SelectListItem> Months { get; set; }
        public List<SelectListItem> Years { get; set; }
    }
    public class ApprvFormEmpDetailsViewModel
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
        public string lblResignType { get; set; }
        public string RelievingDate { get; set; }

        // Dropdown selections
        public string SelectedoptDay { get; set; }
        public string SelectedoptMonth { get; set; }
        public string SelectedoptYear { get; set; }

        public IEnumerable<SelectListItem> Days { get; set; }
        public IEnumerable<SelectListItem> Months { get; set; }
        public IEnumerable<SelectListItem> Years { get; set; }
        public ApprvFormFillAPPAUTHDetails ApprvFormFillAPPAUTHDetails { get; set; }
    }
    public class ApprvFormFillAPPAUTHDetails
    {
        public string lbl_appauth { get; set; }
        public string hdauthlevel { get; set; }
        public string hdauthcode { get; set; }
        public string hdauthemailid { get; set; }
        public string hdauthname { get; set; }
    }
    public class ApprvFormApprovalDtlsViewModel
    {
        public string APPNAME { get; set; }        // Name
        public string APPECODE { get; set; }       // Employee Code
        public string DESIG { get; set; }          // Functional Designation
        public string STATUS { get; set; }         // Status
        public string APPSUBMITEDATE { get; set; } // Approval Date
        public string APPREMARK { get; set; }      // Remarks
    }
    public class SubmitApprovalFormViewModel
    {
        public string ResigID { get; set; }
        public string SelectedDay { get; set; }
        public string SelectedMonth { get; set; }
        public string SelectedMonthText { get; set; }
        public string SelectedYear { get; set; }
        public string ResignType { get; set; }
        public string RadiobtnAppAtatus { get; set; }
        public string Remarks { get; set; }
        public string AuthCode { get; set; }
        public string AuthLevel { get; set; }
        public string AppAuth { get; set; }
        public string AssName { get; set; }
        public string AsseCode { get; set; }
        public string RelDate { get; set; }
        public string Reason { get; set; }
        public string Emailid { get; set; }
        public string AuthMailId { get; set; }
        public string AuthName { get; set; }
    }


}