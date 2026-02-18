using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ePortal.ViewModels.APPX.QMS
{
    public class ISODocModel
    {
        public ISODocModel()
        {
            ddlPlantList = new();
            ddlDeptList = new();
            ddlSectionList = new();
            ddlformatnumberList = new();
            ddlDayAList = new();
            ddlMonthAList = new List<SelectListItem>
            {
                new SelectListItem{Text = "Jan" , Value = "1"},
                new SelectListItem{Text = "Feb" , Value = "2"},
                new SelectListItem{Text = "Mar" , Value = "3"},
                new SelectListItem{Text = "Apr" , Value = "4"},
                new SelectListItem{Text = "May" , Value = "5"},
                new SelectListItem{Text = "Jun" , Value = "6"},
                new SelectListItem{Text = "Jul" , Value = "7"},
                new SelectListItem{Text = "Aug" , Value = "8"},
                new SelectListItem{Text = "Sep" , Value = "9"},
                new SelectListItem{Text = "Oct" , Value = "10"},
                new SelectListItem{Text = "Nov" , Value = "11"},
                new SelectListItem{Text = "Dec" , Value = "12"},
            };
            ddlYearAList = new();

            ddlRevDayRList = new();
            ddlRevMonthRList = this.ddlMonthAList;
            ddlRevYearRList = new();

            ddlImpDayRList = new();
            ddlImpMonthRList = this.ddlMonthAList;
            ddlImpYearRList = new();

            ddlDayDList = new();
            ddlMonthDList = this.ddlMonthAList;
            ddlYearDList = new();
        }
        public List<SelectListItem> ddlPlantList { get; set; }
        public List<SelectListItem> ddlDeptList { get; set; }
        public List<SelectListItem> ddlSectionList { get; set; }
        public List<SelectListItem> ddlformatnumberList { get; set; }
        public List<SelectListItem> ddlDayAList { get; set; }
        public List<SelectListItem> ddlMonthAList { get; set; }
        public List<SelectListItem> ddlYearAList { get; set; }


        public List<SelectListItem> ddlRevDayRList { get; set; }
        public List<SelectListItem> ddlRevMonthRList { get; set; }
        public List<SelectListItem> ddlRevYearRList { get; set; }


        public List<SelectListItem> ddlImpDayRList { get; set; }
        public List<SelectListItem> ddlImpMonthRList { get; set; }
        public List<SelectListItem> ddlImpYearRList { get; set; }

        public List<SelectListItem> ddlDayDList { get; set; }
        public List<SelectListItem> ddlMonthDList { get; set; }
        public List<SelectListItem> ddlYearDList { get; set; }



        public string? ddlPlant { get; set; }
        public string? ddlDept { get; set; }
        public string? ddlSection { get; set; }
        public string? ddlformatnumber { get; set; }
        public string? txtDocNo9 { get; set; }
        public string? txtDocNo { get; set; }
        public string? txtDocNo1 { get; set; }
        public string? txtDocNo2 { get; set; }
        public string? txtDocTitle { get; set; }
        public string? txtRevNoA { get; set; }
        public string? ddlDayA { get; set; }
        public string? ddlMonthA { get; set; }
        public string? ddlYearA { get; set; }
        public string? txtCurrRevNoR { get; set; }
        public string? txtNewRevNoR { get; set; }
        public string? ddlRevDayR { get; set; }
        public string? ddlRevMonthR { get; set; }
        public string? ddlRevYearR { get; set; }
        public string? ddlImpDayR { get; set; }
        public string? ddlImpMonthR { get; set; }
        public string? ddlImpYearR { get; set; }
        public string? txtCurrRevD { get; set; }

        public string? ddlDayD { get; set; }
        public string? ddlMonthD { get; set; }
        public string? ddlYearD { get; set; }
        public string? txtReason { get; set; }
        public string? lblAppAuth { get; set; }

        public string? changeType_rdomaster { get; set; }
        public IFormFile? txtFileUpload { get; set; }
        public string? hdnFile { get; set; }

        public bool IsChecked_rbtnAdd { get; set; }
        public bool IsChecked_rbtnRev { get; set; }
        public bool IsChecked_rbtnDel { get; set;}


        public string? hdAPPECODE { get; set; } // viewstate
        public string? hdAPPEMAILID { get; set; } // viewstate

        public string? hdFilenameExist { get; set; } // viewstate
        public string? hdIsoDocId { get; set; } // viewstate
    }
   
    public class ISODocRequestDetailViewModel()
    {
        public string? ltlReqID { get; set; }
        public string? ltlEmployee { get; set; }
        public string? ltlAppDate { get; set; }
        public string? ltlReqStatus { get; set; }
        public string? ltldocno { get; set; }
        public string? ltldoctitle { get; set; }
        public string? ltlchangetype { get; set; }
        public string? ltldepartment { get; set; }
        public string? ltlAppStatus { get; set; }
        public string? ltlStatus { get; set; }
        public string? ltlAppAuth { get; set; }
        public string? ltlDate { get; set; }
        public string? ltlAuthEmail { get; set; }
        public string? ltlAuthRem { get; set; }
        public string? ltlAdminAuth { get; set; }
        public string? ltlAdminRem { get; set; }
        public string? ltlAdminAppDate { get; set; }
        public string? ltladminemail { get; set; }
        public string? ltladminheadauth { get; set; }
        public string? ltladminheademail { get; set; }
        public string? ltladminheadstatus { get; set; }
        public string? ltladminheadremarks { get; set; }
        public string? ltladminheaddate { get; set; }


    }

    public class ISODocRequestAppRejectViewModel
    {
        public string? RequestIDC { get; set; }
        public string? RequestIDA { get; set; }

        public string lblHeader { get; set; } = string.Empty;
        public string IsoDocId { get; set; } = string.Empty;

        public string EMPEMAIL {  get; set; }
        public string chg { get; set; }

        public string? lblEmpName { get; set;}
        public string? lblEmpCode { get; set;}
        public string? lblMobile { get; set;}
        public string? lblRequestDate { get; set;}
        public string? lblPlant { get; set;}
        public string? hdnplant { get; set;}
        public string? lblDept { get; set; }

        public string? lblRevisionNoA { get; set; }
        public string? lblImpDateA { get; set; }
        public string? lblCurrRevisonR { get; set; }
        public string? lblRevisonDateR { get; set; }
        public string? lblImpDateR { get; set; }
        public string? lblCurrRevisonD { get; set; }
        public string? lblImpDateD { get; set; }
        public string? lblDocNo { get; set; }
        public string? lblDocTitle { get; set; }
        public string? lblReason { get; set; }
        public string? lblNewRevisonR { get; set; }
        public string? hdFilenameExist { get; set; }


        public string? txtAPPRemarks { get; set; }
        public string? txtCancallation { get; set; }
        public string? changeType_rdomaster { get; set; }
        public string rblApprovalStatus { get; set; }
    }

    public class ISODocRequestAppRejectSaveModel 
    {
        public string Cancelid { get; set; } = string.Empty; //vks querystring
        public string Appid { get; set;} = string.Empty; //vks querystring

        public string? txtCancallation { get; set; }
        public string? txtAPPRemarks { get; set; }
        public string? rblApprovalStatus { get; set; }
        public string? hdnplant { get; set; }
        public string? lblEmpCode { get; set; }
        public string? lblEmpName { get; set; }
        public string? lblDocNo { get; set; }
        public string? lblDocTitle { get; set; }
        public string? EmpEmail { get; set; } //vks viewmodel
        public string? chg { get;set; } //vks viewmodel
    }

    public class ISODocViewModel : ISODocModel
    {
        
    }



    public class ISODocSaveModel
    {
        public string ddlPlant { get; set; } = null!;
        public string ddlDept { get; set; } = null!;
        public string? ddlSection { get; set; } //vks check form data
        public string? ddlformatnumberSelectedText { get; set; }
        public string? txtDocNo9 { get; set; }
        public string? txtDocNo { get; set; }
        public string? txtDocNo1 { get; set; }
        public string? txtDocNo2 { get; set; }
        public string? txtDocTitle { get; set; }
        public string? hdnDocTitle { get; set; }
        public string? ddlDayA { get; set; }
        public string? ddlMonthASelectedText { get; set; }
        public string? ddlYearA { get; set; }
        public string? txtCurrRevNoR { get; set; }
        public string? hdnCurrRevNoR { get; set; }
        public string? hdnNewRevNoR { get; set; }
        public string? ddlRevDayR { get; set; }
        public string? ddlRevMonthRSelectedText { get; set; }
        public string? ddlRevYearR { get; set; }
        public string? ddlImpDayR { get; set; }
        public string? ddlImpMonthRSelectedText { get; set; }
        public string? ddlImpYearR { get; set; }
        public string? txtCurrRevD { get; set; }
        public string? ddlDayD { get; set; }
        public string? ddlMonthDSelectedText { get; set; }
        public string? ddlYearD { get; set; }
        public string? txtReason { get; set; }
        public string? lblAppAuth { get; set; }
        public IFormFile? txtFileUpload { get; set; }

        public bool IsChecked_rbtnAdd { get; set; }
        public bool IsChecked_rbtnRev { get; set; }

        public string? hdstrAppAuth { get; set; } // viewstate
        public string? hdAPPEMAILID { get; set; } // viewstate
        public string? hdFilenameExist { get; set; } // viewstate
        public string? hdIsoDocId { get; set; } // viewstate
    }

    public class ISODocUpdateModel : ISODocSaveModel
    {
        public string? OldIsoDocId { get; set;}
    }

    public class BindTextChangedISO_DTO
    {
        public string ddlformatnumber { get; set; } = string.Empty;
        public string txtDocNo9 { get; set; } = string.Empty;
        public string txtDocNo { get; set; } = string.Empty;
        public string txtDocNo1 { get; set; } = string.Empty;
        public string txtDocNo2 { get; set; } = string.Empty;
        public bool IsChecked_rbtnAdd { get; set; }
        public bool IsChecked_rbtnRev { get; set; }
        public bool IsChecked_rbtnDel { get; set; }

    }

    public class ISODocEditViewModel : ISODocModel
    {
        public string? OldIsoDocId { get; set; }
        public string? ChangeType {get; set;}
    }

    public class AppAuthority_DTO
    {
        public bool IsChecked_rbtnAdd { get; set; }
        public bool IsChecked_rbtnRev { get; set; }
        public bool IsChecked_rbtnDel { get; set; }

    }


    public class QMSOperationResult
    {
        public bool Status { get; set; }
        public string? Message { get; set; }
        public string? RefID { get; set; }
        public object? obj { get; set; }
    }
}
