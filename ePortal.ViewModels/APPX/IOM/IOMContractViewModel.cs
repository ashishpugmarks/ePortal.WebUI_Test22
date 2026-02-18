using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ePortal.ViewModels.APPX.IOM
{
    #region IOMRequestForm
    public class IOMRequestFormViewModel : IOMRequestForm
    {
        public List<SelectListItem> ddlFromList { get; set; }
        public List<SelectListItem> ddlagreementList { get; set; }
        public List<SelectListItem> ddlcontractList { get; set; }
        public List<SelectListItem> ddlMannerOfPaymentList { get; set; }
        public List<SelectListItem> ddlmstagreementList { get; set; }

        public List<SelectListItem> optDayList { get; set; }
        public List<SelectListItem> optMonthList { get; set; }
        public List<SelectListItem> optYearList { get; set; }
        public List<SelectListItem> optEDList { get; set; }
        public List<SelectListItem> optEMList { get; set; }
        public List<SelectListItem> optEYList { get; set; }

        public List<SelectListItem> ddlreturndList { get; set; }
        public List<SelectListItem> ddlreturnmList { get; set; }
        public List<SelectListItem> ddlreturnyList { get; set; }

        public List<SelectListItem> ddlreturn2dList { get; set; }
        public List<SelectListItem> ddlreturn2yList { get; set; }
        public List<SelectListItem> ddlreturn2mList { get; set; }
        public List<SelectListItem> ddl_appauthList { get; set; }

        public string txtTo { get; set; } = null!;


        public IOMRequestFormViewModel()
        {
            ddlFromList = new List<SelectListItem>();
            ddlcontractList = new List<SelectListItem>();
            ddl_appauthList = new List<SelectListItem>();

            ddlagreementList = new List<SelectListItem>
            {
                new SelectListItem{Text = "Drafting" , Value = "1"},
                new SelectListItem{Text = "Vetting" , Value = "2"},
            };
            ddlMannerOfPaymentList = new List<SelectListItem>
            {
                new SelectListItem{Text = "-Select manner of payment-" , Value = string.Empty},
                new SelectListItem{Text = "Monthly" , Value = "1"},
                new SelectListItem{Text = "Quarterly" , Value = "2"},
                new SelectListItem{Text = "Half yearly" , Value = "3"},
                new SelectListItem{Text = "Yearly" , Value = "4"},
                new SelectListItem{Text = "Milestone Basis" , Value = "5"}
            };

            optMonthList = new List<SelectListItem>
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
            optEMList = this.optMonthList;
            ddlreturnmList = this.optMonthList;
            ddlreturn2mList = this.optMonthList;
        }

    }

    public class IOMRequestForm
    {
        public string ddlfrom { get; set; }
        public string ddlfromText { get; set; }
        public string ddlagreement { get; set; }
        public string? ddlagreementText { get; set; }
        public string? ddlcontract { get; set; }
        public string? ddlcontractText { get; set; }
        public string? ddlMannerOfPayment { get; set; }
        public string? ddlmstagreement { get; set; }


        public string? txtcontactecode { get; set; }
        public string? HDFTRINING { get; set; }
        public string? txtsuretyamount { get; set; }
        public string? txtnamesurety { get; set; }

        // From date
        public string? ddlreturnd { get; set; }
        public string? ddlreturnm { get; set; }
        public string? ddlreturny { get; set; }

        // To date 
        public string ddlreturn2d { get; set; }
        public string ddlreturn2m { get; set; }
        public string ddlreturn2y { get; set; }

        // Effective Date
        public string? optED { get; set; }
        public string? optEM { get; set; }
        public string? optEY { get; set; }

        // Date of expiry
        public string? optDay { get; set; }
        public string? optMonth { get; set; }
        public string? optYear { get; set; }

        // Term 
        public string? ddlTermyear { get; set; }
        public string? ddlTermmonth { get; set; }


        public string? txtvendorname { get; set; }
        public string rdomaster { get; set; } = null!;
        public string? txtconsiderable { get; set; }
        public string? txtPurpose { get; set; }
        public string? txtRemarks { get; set; }


        public string? hdnexpiryday { get; set; }
        public string? hdnexpirymon { get; set; }
        public string? hdnexpiryyear { get; set; }

        public string? hdauthcode { get; set; }
        public string? hdauthname { get; set; }
        public string? hdauthemailid { get; set; }
        public string? lbl_appauth { get; set; }
        public string? hdauthlevel { get; set; }
        public string? ddl_appauth { get; set; }

        public string CheckDeclaration { get; set; }

        public IFormFile? fileUploadAttachmentAA { get; set; }
        public IFormFile? fileUploadAttachmentRA { get; set; }
    }

    public class IOMRequestFormSaveModel : IOMRequestForm
    {
        public string? hdIOM { get; set; }
        public string? hdnagreeFile { get; set; }
        public string? hdnrefFile { get; set; }
        public bool? FileUploadAttachmentAAEnabled { get; set; }
        public bool? FileUploadAttachmentRAEnabled { get; set; }

    }

    public class IOMRequestEditFormViewModel : IOMRequestFormViewModel
    {
        public string linkappnote { get; set; }
        public string linkappnoteText { get; set; }

        public string linkbtnagreement { get; set; }
        public string linkbtnagreementText { get; set; }
    }
    #endregion

    #region ManageContract
    public class ManageContractViewModel
    {
        public long IOMID { get; set; }
        public string? AGREEMENTTYPEDESC { get; set; }
        public string? CONTRACTTYPEDESC { get; set; }
        public string? EFFECTIVEDATE { get; set; }
        public string? EXPIRYDATE { get; set; }
        public string? VENDORNAME { get; set; }
        public string? STATUSNAME { get; set; }
        public string? USERTYPE { get; set; }
        public int? processstatusid { get; set; }
    }

    public class ViewIOMDetail
    {
        public string hdiomid { get; set; }
        public string hdreqname { get; set; }
        public string hdreqcode { get; set; }
        public string hdreqemail { get; set; }

        public string? lbl_assname { get; set; }
        public string? lbl_site { get; set; }
        public string? lbl_op { get; set; }
        public string? lbl_div { get; set; }
        public string? lbl_dept { get; set; }
        public string? lbl_sec { get; set; }


        public string? lblagreementid { get; set; }
        public string? lblprevagrperiod { get; set; }
        public string? prevappnotelink { get; set; } 
        public string? prevappnotelinkText { get; set; } 
        public string? prevfinaldoclink { get; set; } 
        public string? prevfinaldoclinkText { get; set; } 


        public string? lblUNIQUEID { get; set; }
        public string? lblfrom { get; set; }
        public string? lblto { get; set; }
        public string? lblagreementtype { get; set; }
        public string? lblcontracttype { get; set; }
        public string? lbleffdate { get; set; }
        public string? lblexpdate { get; set; }
        public string? lblterm { get; set; }
        public string? lblvendorname { get; set; }
        public string? lblconsiderable { get; set; }
        public string? lblmannerofpayment { get; set; }
        public string? agreementlink { get; set; } 
        public string? agreementlinkText { get; set; }
        public string? antibriberylink { get; set; } 
        public string? antibriberylinkText { get; set; }
        public string? ndadocumentlink { get; set; }
        public string? ndadocumentlinkText { get; set; }
        public string? Iblconsiderable { get; set; }
        public string? lblpurpose { get; set; }
        public string? lblremarks { get; set; }
        public string? lblmstagr { get; set; }
        public string? lblcontractcode { get; set; }
        public string? lblcontractname { get; set; }
        public string? lblsuretyamount { get; set; }
        public string? lblnameofsurety { get; set; }
        public string? lblretureddateF { get; set; }
        public string? lblretureddateT { get; set; }

        public string rdoStatus { get; set; }
        public string? chkTerms { get; set; }


        public IEnumerable<grdApprovalDetails>? grdApprovalDetails { get; set; } 
        public IEnumerable<grdCommunicationHistory>? grdCommunicationHistory { get; set; }
    }

    public class grdApprovalDetails
    {
        public string? ADDEDDATE { get; set; }
        public string? ADDEDBY { get; set; }
        public string? STATUSNAME { get; set; }
        public string? REMARKS { get; set; }
        public string? ATTACHMENTPATH { get; set; }
    }

    public class grdCommunicationHistory
    {
        public string? ADDEDDATE { get; set; }
        public string? ADDEDBY { get; set; }
        public string? USERTYPE { get; set; }
        public string? REMARKS { get; set; }
        public string? ATTACHMENT { get; set; }
    }

    #endregion

    #region ManageApproval
    public class ManageIOMApprovalViewModel
    {
        public long IOMID { get; set; }
        public string? AGREEMENTTYPEDESC { get; set; }
        public string? CONTRACTTYPEDESC { get; set; }
        public string? EFFECTIVEDATE { get; set; }
        public string? EXPIRYDATE { get; set; }
        public string? VENDORNAME { get; set; }
        public string? STATUSNAME { get; set; }
        public string? REQUESTDBY { get; set; }
        public string? EMPDESIGNATION { get; set; }
    }

    public class IOMApprovalFormViewModel : ViewIOMDetail
    {
        public IOMApprovalFormViewModel()
        {
            this.rdoStatus = "1";
            grdPrivContractHistory = new();
            ddl_appauthList = new();
        }

        public string? txtbackdateremark { get;set; }
        public string? txt_Remarks { get;set; }
        public string? hdauthcode { get;set; }
        public string? hdauthemailid { get;set; }
        public string? hdauthname { get;set; }
        public string? lbl_appauth { get;set; }
        public string? hdbackdate { get;set; }
        public string? hdauthlevel { get;set; }
        public string? hddelaydate { get;set; }
        public string? ddl_appauth { get;set; }
        public List<SelectListItem> ddl_appauthList { get; set; }
        public List<grdPrivContractHistory> grdPrivContractHistory { get; set; }
    }

    public class grdPrivContractHistory
    {
        public long IOMID { get; set; }
        public string? AGREEMENTTYPEDESC { get; set; }
        public string? VENDORNAME { get; set; }
        public string? EFFECTIVEDATE { get; set; }
        public string? EXPIRYDATE { get; set; }
        public int? processstatusid { get; set; }
        public string? STATUSNAME { get; set; }
        public string? REQUESTDBY { get; set; }
        public string? finaldoc { get; set; }

    }

    public class IOMApprovalFormSave
    {
        public string hdiomid { get; set; }
        public string EMPDESIGNATION { get; set; }
        public string hdbackdate { get; set; }
        public string txtbackdateremark { get; set; }
        public string hddelaydate { get; set; }
        public bool chkTerms { get; set; }
        public string rdoStatus { get; set; }
        public string txt_Remarks { get; set; }
        public string lbl_appauth { get; set; }
        public string ddl_appauth { get; set; }

        public string hdauthlevel { get; set; }
        public string hdauthcode { get; set; }
        public string hdauthname { get; set; }
        public string hdauthemailid { get; set; }
        public string hdreqemail { get; set; }
        public string hdreqname { get; set; }
        public string lblagreementtype { get; set; }
        public string lblcontracttype { get; set; }
        public string lbleffdate { get; set; }
        public string lblexpdate { get; set; }
        public string lblvendorname { get; set; }

    }


    public class SubmitFinalDocumentViewModel : ViewIOMDetail
    {

        public string? txtbackdateremark { get; set; }
        public string? txt_Remarks { get; set; }
        public IFormFile? filefinaldoc { get; set; }

    }
    public class SubmitFinalDocumentSave
    {
        public string txt_Remarks { get; set; }
        public string hdiomid { get; set; }
        public string txtvendorname { get; set; }
        public IFormFile? filefinaldoc { get; set; }
    }

    public class UserAcknowledgementViewModel : ViewIOMDetail
    {
        public string? txt_Remarks { get; set; }

    }

    public class UserAcknowledgementSave
    {
        public string hdiomid { get; set; }
        public string txt_Remarks { get; set; }
        public string lblagreementtype { get; set; }
        public string lblcontracttype { get; set; }
        public string lbleffdate { get; set; }
        public string lblexpdate { get; set; }
        public string lblvendorname { get; set; }
    }

    public class UserCommunicationViewModel : ViewIOMDetail
    {
        public string HDENAME { get; set; }
        public string HDEMAILID { get; set; }
        public string lblRequesterName { get; set; }
        public string hd_Requester { get; set; }
        public string hd_Email { get; set; }
        public string txtMessageRemarks { get; set; }
        public IFormFile? RefUploadDocument { get; set; }

        public string hdRefDocument { get; set; } //vks

    }

    public class UserCommunicationSave
    {
        public string hdiomid { get; set; }
        public string txtMessageRemarks { get; set; }
        public IFormFile? RefUploadDocument { get; set; }
        public string HDEMAILID { get; set; }
        public string HDENAME { get; set; }
        public string lbl_assname { get; set; }
        public string lblagreementtype { get; set; }

        public string lblcontracttype { get; set; }
        public string lbleffdate { get; set; }
        public string lblexpdate { get; set; }
        public string lblvendorname { get; set; }

    }

    #endregion


    #region  ContractList

    public class ContractListViewModel
    {
        public ContractListViewModel()
        {
            grdcontractlist = new List<grdcontractlist>();
            ddlcontracttypeList = new();
        }
        public List<SelectListItem> ddlcontracttypeList { get; set; }
        public IEnumerable<grdcontractlist> grdcontractlist { get; set; }
        public ContractListViewModelFilter filter { get; set; }

        //  public string CONTRACTTYPEDESC { get; set; }
    }

    public class ContractListViewModelFilter
    {
        public string? ddlcontracttype { get; set; }
        public string? txtvendorname { get; set; }
        public string? txtStarttDate { get; set; }
        public string? txtEndtDate { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
        public int totalItems { get; set; }
        public int totalPages { get; set; }
        public bool hasPrevious { get; set; }
        public bool hasNext { get; set; }
    }

    public class grdcontractlist
    {
        public string? AGREEMENTHEADERID { get; set; }
        public string? CONTRACTTYPEDESC { get; set; }
        public string? VENDORNAME { get; set; }
        public string? AGREEMENTDATE { get; set; }
        public string? EXPIRYDATE { get; set; }
        public string? FINALDOC { get; set; }
        public string? EXPDAYS { get; set; }
    }

    public class grdSubcontractlist
    {
        public string? IOMID { get; set; }
        public string? AGREEMENTTYPEDESC { get; set; }
        public string? VENDORNAME { get; set; }
        public string? EFFECTIVEDATE { get; set; }
        public string? EXPIRYDATE { get; set; }
        public string? processstatusid { get; set; }
        public string? REQUESTDBY { get; set; }
        public string? STATUSNAME { get; set; }
        public string? UNIQUEID { get; set; }
        public string? finaldoc { get; set; }

    }

    #endregion


    public class ContractRenewalViewModel : IOMRequestFormViewModel
    {
        public ContractRenewalViewModel()
        {
            grdPrivContractHistory = new();
        }
        public string? HDAGREEMENTID { get; set; }
        public string? lblUNIQUEID { get; set; }
        public string? lblagreementid { get; set; }
        public string? appnotelink { get; set; }
        public string? appnotelinkText { get; set; }
        public string? FINALDOCLINK { get; set; }
        public string? FINALDOCLINKText { get; set; }
        public IFormFile? fileuploadfinaldoc { get; set; }

        public List<grdPrivContractHistory> grdPrivContractHistory { get; set; }

    }

    public class ContractRenewalSave
    {
        public string HDAGREEMENTID { get; set; } = null!;
        public string? hdauthlevel { get; set; }
        public string? ddl_appauth { get; set; }
        public string? hdauthcode { get; set; }
        public string? hdauthname { get; set; }
        public string? hdauthemailid { get; set; }
        public string? ddlfrom { get; set; }
        public string? ddlagreement { get; set; }
        public string? ddlcontract { get; set; }
        public string? ddlcontractText { get; set; }
        public string? optED { get; set; }
        public string? optEM { get; set; }
        public string? optEY { get; set; }
        public string? ddlTermmonth { get; set; }
        public string? ddlTermyear { get; set; }
        public string? hdnexpiryday { get; set; }
        public string? optDay { get; set; }
        public string? optMonth { get; set; }
        public string? optYear { get; set; }
        public string? hdnexpirymon { get; set; }
        public string? hdnexpiryyear { get; set; }
        public string? ddlMannerOfPayment { get; set; }
        public string? txtPurpose { get; set; }
        public string? txtRemarks { get; set; }
        public string? txtvendorname { get; set; }
        public string? rdomaster { get; set; }
        public string? ddlmstagreement { get; set; }
        public string? lbl_appauth { get; set; }
        public string? txtconsiderable { get; set;}
        public string? appnotelinkText { get; set; }
        public IFormFile? fileuploadfinaldoc { get; set; }

    }

    public class CloseContractViewModel : ViewIOMDetail
    {
        public string? HDAGREEMENTID { get; set; }
        public string? AgreementlinkText { get; set; }
        public string? Agreementlink { get; set; }
        public string? approvalnotelinkText { get; set; }
        public string? approvalnotelink { get; set; }
        public string? lblmstagr { get; set; }
        public string? txt_closeRemarks { get;set; }
    }
    public class CloseContractSave
    {
        public string HDAGREEMENTID { get; set; } = null!;
        public string? txt_closeRemarks { get; set; }
    }

    public class ViewAgreementViewModel : ViewIOMDetail
    {
        public string? appnotelink { get; set; }
        public string? appnotelinkText { get; set; }
        public string? finaldoclink { get; set; }
        public string? finaldoclinkText { get; set; }
    }


    public class ContractAmendmentViewModel : IOMRequestFormViewModel
    {
        public ContractAmendmentViewModel()
        {
            grdPrivContractHistory = new();
        }
        public string? HDAGREEMENTID { get; set; }
        public string? lblUNIQUEID { get; set; }
        public string? lblagreementid { get; set; }
        public string? appnotelink { get; set; }
        public string? appnotelinkText { get; set; }
        public string? FINALDOCLINK { get; set; }
        public string? FINALDOCLINKText { get; set; }
        public IFormFile? fileuploadfinaldoc { get; set; }
        public List<grdPrivContractHistory> grdPrivContractHistory { get; set; }
    }

    public class ContractAmendmentSave
    {
        public string HDAGREEMENTID { get; set; } = null!;
        public string? hdauthlevel { get; set; }
        public string? ddl_appauth { get; set; }
        public string? hdauthcode { get; set; }
        public string? hdauthname { get; set; }
        public string? hdauthemailid { get; set; }
        public string? ddlfrom { get; set; }
        public string? ddlagreement { get; set; }
        public string? ddlcontract { get; set; }
        public string? ddlcontractText { get; set; }
        public string? optED { get; set; }
        public string? optEM { get; set; }
        public string? optEY { get; set; }
        public string? ddlTermmonth { get; set; }
        public string? ddlTermyear { get; set; }
        public string? hdnexpiryday { get; set; }
        public string? optDay { get; set; }
        public string? optMonth { get; set; }
        public string? optYear { get; set; }
        public string? hdnexpirymon { get; set; }
        public string? hdnexpiryyear { get; set; }
        public string? ddlMannerOfPayment { get; set; }
        public string? txtPurpose { get; set; }
        public string? txtRemarks { get; set; }
        public string? txtvendorname { get; set; }
        public string? rdomaster { get; set; }
        public string? ddlmstagreement { get; set; }
        public string? lbl_appauth { get; set; }
        public string? txtconsiderable { get; set; }
        public string? appnotelinkText { get; set; }
        public IFormFile? fileuploadfinaldoc { get; set; }
    }

    public class VerificationRequestFormViewModel : IOMRequestFormViewModel
    {
        public string HDAGREEMENTID { get; set; } = null!;
        public string? hdauthlevel { get; set; }
        public string? ddl_appauth { get; set; }
        public string? hdauthcode { get; set; }
        public string? hdauthname { get; set; }
        public string? hdauthemailid { get; set; }
        public string? ddlfrom { get; set; }
        public string? ddlagreement { get; set; }
        public string? ddlcontract { get; set; }
        public string? ddlcontractText { get; set; }
        public string? optED { get; set; }
        public string? optEM { get; set; }
        public string? optEY { get; set; }
        public string? ddlTermmonth { get; set; }
        public string? ddlTermyear { get; set; }
        public string? hdnexpiryday { get; set; }
        public string? optDay { get; set; }
        public string? optMonth { get; set; }
        public string? optYear { get; set; }
        public string? hdnexpirymon { get; set; }
        public string? hdnexpiryyear { get; set; }
        public string? ddlMannerOfPayment { get; set; }
        public string? txtPurpose { get; set; }
        public string? txtRemarks { get; set; }
        public string? txtvendorname { get; set; }
        public string? rdomaster { get; set; }
        public string? ddlmstagreement { get; set; }
        public string? lbl_appauth { get; set; }
        public string? txtconsiderable { get; set; }
        public string? appnotelinkText { get; set; }
        public IFormFile? fileuploadfinaldoc { get; set; }
    }

    public class VerificationRequestFormSave : IOMRequestForm { }

    #region IOMDashboard
    public class IOMDashboardViewModel
    {
        public IOMDashboardViewModel()
        {
            ddlcontracttypeList = new();
            grdwipdetail = new List<grdwipdetail>();
        }

        public string? hdoperationid { get; set; }
        public string? hddivision { get; set; }
        public string? hddepartment { get; set; }
        public string? ddlcontracttype { get; set;}

        public List<SelectListItem> ddlcontracttypeList { get; set; }
        public IEnumerable<grdwipdetail> grdwipdetail { get; set; }
    }

    public class IOMDashboardProgressChartViewModel
    {
        public IOMDashboardProgressChartViewModel()
        {
            grdPendingcount = new List<grdPendingcount>();
            grdrenpending = new List<grdrenpending>();
            CHART_COMPLETEDCONT_STATS = new List<CHART_COMPLETEDCONT_STATS>();
            CHART_PENDINGCONT_STATS = new List<CHART_PENDINGCONT_STATS>();
        }
        public IEnumerable<grdPendingcount> grdPendingcount { get; set; }
        public IEnumerable<grdrenpending> grdrenpending { get; set; }
        public IEnumerable<CHART_COMPLETEDCONT_STATS> CHART_COMPLETEDCONT_STATS { get; set; }
        public IEnumerable<CHART_PENDINGCONT_STATS> CHART_PENDINGCONT_STATS { get; set; }
    }

    public class IOMDashboardContractDetailsViewModel
    {
        public IOMDashboardContractDetailsViewModel()
        {
            grddashdetail = new List<grddashdetail>();
            ddlvendorlist = new();
        }

        public List<SelectListItem>? ddlvendorlist { get; set; }
        public IEnumerable<grddashdetail> grddashdetail { get; set; }   
    }
    public class GetIOMDashboardProgressChartData_DTO
    {
        public string hdoperationid { get; set; } = null!;
        public string hddivision { get; set; } = null!;
        public string hddepartment { get; set; } = null!;
        public string ddlcontracttype { get; set; } = string.Empty;

    }
    public class IOMDashboardContractDetails_DTO
    {
        public string HDREQUESTTYPE { get; set; } = null!;
        public string VENDORNAME { get; set; } = string.Empty;

        public string contracttypeid { get; set; } = null!;
        public string hdoperationid { get; set; } = null!;

    }
    public class CHART_COMPLETEDCONT_STATS
    {

    }
    public class CHART_PENDINGCONT_STATS
    {

    }

    public class grdwipdetail
    {
        public string iomid { get; set; } = null!;
        public string? vendorname { get; set; }
        public string? agrementtype { get; set; }
        public string? effectivedate { get; set; }
        public string? EXPIRYDATE { get; set; }
        public string? statusname { get; set; }
    }

    public class grdPendingcount
    {
        public string contracttypeid { get; set; } = null!;
        public string? ContractType { get; set; }
        public string? Cnt { get; set; }
    }

    public class grdrenpending
    {
        public string contracttypeid { get; set; } = null!;
        public string? ContractType { get; set; }
        public string? Cnt { get; set; }
    }

    public class grddashdetail
    {
        public string? ContractType { get; set; }
        public string? vendorname { get; set; }
        public string? dateofagreement { get; set; }
        public string? dateofexpiry { get; set; }
        public string? finaldoc { get; set; }
    }

    #endregion

    public class IOMContractOperationResult
    {
        public bool Status { get; set; }
        public string? Message { get; set; }
        public string? RefID { get; set; }
        public object? obj { get; set; }
    }
}
