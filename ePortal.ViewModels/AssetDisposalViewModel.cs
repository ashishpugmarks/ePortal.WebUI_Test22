using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ePortal.ViewModels
{
    public class AssetDisposalViewModel
    {
        [DisplayName("SNo.")]
        public long DISPOSALHEADERID { get; set; }

        [DisplayName("Emp-Code")]
        public long ADEMPCODE { get; set; }

        [DisplayName("Request By")]
        public string EMP_NAME { get; set; }

        [DisplayName("Asset Type")]
        public long ASSETTYPEID { get; set; }

        [DisplayName("Asset Type")]
        public string ASSETTYPE { get; set; }

        [DisplayName("Invoice / Photograph / Bill of entry")]
        public IFormFile BACKUP_ATTACHMENT { get; set; }

        [DisplayName("Invoice / Photograph// Bill of entry")]
        public string BACKUP_ATTACHMENT_NAME { get; set; }

        [DisplayName("Scrap Approval / Technical Note")]
        public IFormFile TRANSFERSLIP_ATTACHMENT { get; set; }

        [DisplayName("Scrap Approval / Technical Note")]
        public string TRANSFERSLIP_ATTACHMENT_NAME { get; set; }

        [DisplayName("Multilation Attachment")]
        public IFormFile MUTILATION_ATTACHMENT { get; set; }

        [DisplayName("Multilation Attachment")]
        public string MUTILATION_ATTACHMENT_NAME { get; set; }

        [DisplayName("Quotation Attachment")]
        public string QUOTATION_ATTACHMENT_NAME { get; set; }

        [DisplayName("Invoice / Bill of entry Attachment")]
        public string INVOICE_ATTACHMENT_NAME { get; set; }

        [DisplayName("Syki")]
        public long SYKI { get; set; }

        public string KICODE { get; set; }

        [DisplayName("Process Status")]
        public long PROCESSSTATUSID { get; set; }

        [DisplayName("Process Status")]
        public string PROCESSSTATUS { get; set; }

        public short STATUS_LEVEL { get; set; }

        [DisplayName("Remarks")]
        public string REMARKS { get; set; }

        [DisplayName("Invoice Attachment")]
        public string INVOICE_ATTACHMENT { get; set; }

        [DisplayName("Asset Disposal Invoice")]
        public string ASSETDISPOSALINVOICE { get; set; }

        [DisplayName("Added By")]
        public long ADDEDBY { get; set; }

        public string ADDEDBY_NAME { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Request Date")]
        public DateTime? DATEADDED { get; set; }

        public long? MODIFIEDBY { get; set; }

        public DateTime? DATELSTMOD { get; set; }

        [DisplayName("Status")]
        public bool ACTIVE { get; set; }

        public long TOTAL_AMOUNT { get; set; }

        public virtual Employee_Details Emp_Detail { get; set; }

        public virtual DisposalCustomerViewModel CustomerDetail { get; set; }

        [JsonProperty("DisposalDetailList")]
        public virtual IList<DisposalDetailViewModel> DisposalDetailList { get; set; }

        public virtual ApprovalHisViewModel Approval_History { get; set; }

        public virtual IList<ApprovalHisViewModel> AppHistoryList { get; set; }

        public virtual IList<AssetDetailViewModel> SapAssetList { get; set; }

        [DisplayName("No. of Asset")]
        public long AssetCount { get; set; }
        public long IBMAssetCount { get; set; }

        [DisplayName("Forwarded To")]
        public string ForwardedTo { get; set; }

        [DisplayName("Plant Approval Required")]
        public short? IsPlantRequired { get; set; }

        public short? P1FAppStatus { get; set; }

        public short? P2FAppStatus { get; set; }

        public short? P3FAppStatus { get; set; }

        public short? P4FAppStatus { get; set; }

        [DisplayName("1F Attachment")]
        public string P1F_ATTACHMENT_NAME { get; set; }

        [DisplayName("2F Attachment")]
        public string P2F_ATTACHMENT_NAME { get; set; }

        [DisplayName("3F Attachment")]
        public string P3F_ATTACHMENT_NAME { get; set; }

        [DisplayName("4F Attachment")]
        public string P4F_ATTACHMENT_NAME { get; set; }

        [DisplayName("1F")]
        public string C1F_APPROVEBY_NAME { get; set; }
        [DisplayName("2F")]
        public string C2F_APPROVEBY_NAME { get; set; }
        [DisplayName("3F")]
        public string C3F_APPROVEBY_NAME { get; set; }
        [DisplayName("4F")]
        public string C4F_APPROVEBY_NAME { get; set; }

        public short? ISUPLOADINV_BYTAXATION { get; set; }
        public long? PLANTID { get; set; }
        public short IsFinalSubmit { get; set; }
        public short IsEnableForApp { get; set; }
        public short IsReVerify_TotalAmt { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("IBM Approval Date")]
        public DateTime? IBM_APPDATE { get; set; }
        //------------------START-[Code Added by Aumento on 08-Jan-2024]------------------------------
        [DisplayName("IC Month")]
        public string ICMONTH { get; set; }
        [DisplayName("Implementation Month")]
        public string IMPLEMENTMONTH { get; set; }
        //------------------END-[Code Added by Aumento on 08-Jan-2024]--------------------------------

        public int? finalPrint { get; set; }
    }

    [Serializable]
    public class DisposalDetailViewModel
    {
        [DisplayName("SNo.")]
        public long DISPOSALDETAILID { get; set; }

        public long DISPOSALHEADERID { get; set; }

        [DisplayName("Asset Code")]
        public string? ASSETCODE { get; set; }

        [DisplayName("Asset Description")]
        public string? ASSETDESCRIPTION { get; set; }

        [DisplayName("Asset Detail")]
        public string? ASSETDETAIL { get; set; }

        [DisplayName("Vendor Name")]
        public string? VENDORNAME { get; set; }

        [DisplayName("Vendor Code")]
        public string? VENDORCODE { get; set; }

        [DisplayName("Invoice No.")]
        public string? INVOICENO { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Invoice Date")]
        public DateTime? INVOICEDATE { get; set; }

        [DisplayName("Bill Entry")]
        public string? BILLENTRY { get; set; }

        [DisplayName("License No.")]
        public string? LICENSENO { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("License Date")]
        public DateTime? PURCHASEDDATE { get; set; }

        [DisplayName("Original Cost"), DisplayFormat(DataFormatString = "{0:0,0.00}")]
        public decimal? ORIGINALCOST { get; set; }

        [DisplayName("Accumulated Depreciation"), DisplayFormat(DataFormatString = "{0:0,0.00}")]
        public decimal? DEPRECIATIONCOST { get; set; }

        [DisplayName("Net Value"), DisplayFormat(DataFormatString = "{0:0,0.00}")]
        public decimal? WDV { get; set; }

        [DisplayName("Sales Price"), DisplayFormat(DataFormatString = "{0:0,0.00}")]
        public decimal? SALESPRICE { get; set; }

        [DisplayName("Profit/Loss"), DisplayFormat(DataFormatString = "{0:0,0.00}")]
        public decimal? PROFIT_LOSS { get; set; }

        [DisplayName("Mgf./Qty Approval")]
        public short? MGF_QLTYSTATUS { get; set; }

        [DisplayName("Env. Approval")]
        public short? EVNSTATUS { get; set; }

        [DisplayName("Taxation Approval")]
        public short? TAXTION_STATUS { get; set; }

        [DisplayName("Maintenance Dept. Approval")]
        public short? MAINTENANCEDEPT_STATUS { get; set; }

        [DisplayName("IC Approval")]
        public short? IC_STATUS { get; set; }

        [DisplayName("IBM Approval")]
        public short? IBM_STATUS { get; set; }

        [DisplayName("Asset Category")]
        public short? ASSET_CATEGORY { get; set; }

        [DisplayName("Asset Condition")]
        public short? CONDITIONID { get; set; }

        public string ASSET_CONDITION { get; set; }

        [DisplayName("Asset Required")]
        public string ASSET_REQUIRED { get; set; }

        [DisplayName("1F")]
        public string CONDITION_1F { get; set; }

        [DisplayName("2F")]
        public string CONDITION_2F { get; set; }

        [DisplayName("3F")]
        public string CONDITION_3F { get; set; }

        [DisplayName("4F")]
        public string CONDITION_4F { get; set; }

        public long? C1F_APPROVEBY { get; set; }
        public long? C2F_APPROVEBY { get; set; }
        public long? C3F_APPROVEBY { get; set; }
        public long? C4F_APPROVEBY { get; set; }

        [DisplayName("Vibration Sensor")]
        public short? VIBRATION_SENSOR { get; set; }

        [DisplayName("Partial/Full")]
        public short? PARTIAL_FULL { get; set; }

        [DisplayName("Valuation for GST")]
        public string VALUATION_GST { get; set; }

        [DisplayName("Tentative Scrap Cost"), DisplayFormat(DataFormatString = "{0:0,0.00}")]
        public decimal? TENTATIVE { get; set; }

        [DisplayName("Added By")]
        public long ADDEDBY { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Added Date")]
        public DateTime DATEADDED { get; set; }

        public long? MODIFIEDBY { get; set; }

        public DateTime? DATELSTMOD { get; set; }

        [DisplayName("Status")]
        public bool ACTIVE { get; set; }

        [DisplayName("Approval Status")]
        public short APPROVAL_STATUS { get; set; }

        [DisplayName("Serial Number")]
        public string SERIALNUMBER { get; set; }

        [DisplayName("Plant")]
        public long? PLANTID { get; set; }
        public string PLANTNAME { get; set; }

        [DisplayName("PO Number")]
        public string PONUMBER { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Captalized Date")]
        public DateTime? CAPTALIZED_DATE { get; set; }
    }

    [Serializable]
    public class DisposalCustomerViewModel
    {
        public long DISPOSALCUSTDETAILID { get; set; }

        public long DISPOSALHEADERID { get; set; }

        [DisplayName("Customer Code")]
        public string CUSTOMERCODE { get; set; }

        [DisplayName("Customer Name")]
        public string CUSTOMERNAME { get; set; }

        [DisplayName("Customer Address")]
        public string CUSTADDRESS { get; set; }

        [DisplayName("Customer GSTIN")]
        public string CUSTGSTIN { get; set; }

        [DisplayName("Total Basic Value")]
        public decimal BASICVALUE { get; set; }

        [DisplayName("GST Rate With HSN")]
        public string GSTRATE { get; set; }

        [DisplayName("Payment Details")]
        public string PAYMENTDETAILS { get; set; }

        [DisplayName("Status")]
        public bool ACTIVE { get; set; }

        [DisplayName("Added By")]
        public long ADDEDBY { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Added Date")]
        public DateTime? DATEADDED { get; set; }

        public long? MODIFIEDBY { get; set; }

        public DateTime? DATELSTMOD { get; set; }
    }

    public class ApprovalHisViewModel
    {
        [DisplayName("SNo.")]
        public long APPROVALHISTORYID { get; set; }

        public long DISPOSALHEADERID { get; set; }

        [DisplayName("Emp-Code")]
        public long APPCODE { get; set; }

        [DisplayName("Employee")]
        public string APPEMP_NAME { get; set; }

        public long PROCESSSTATUSID { get; set; }

        [DisplayName("Remarks")]
        public string REMARKS { get; set; }

        [DisplayName("Approval Attachment")]
        public string ATTACHMENT { get; set; }

        [DisplayName("Approval Status")]
        public short APPROVALSTATUS { get; set; }

        [DisplayName("Upload Invoice By")]
        public short uploadedInvoiceBy { get; set; }

        public short STATUS_LEVEL { get; set; }

        public string ARS_MSG { get; set; }
        public string DISPLAY_MSG { get; set; }

        public long ADDEDBY { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}"), DisplayName("Approval Date")]
        public DateTime DATEADDED { get; set; }

        public short? SEQ_NUMBER { get; set; }
    }

    public class AttachmentViewModel
    {
        public byte[] fileBytes { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }

    #region Master
    public class AssetDetailViewModel
    {
        [DisplayName("AssetCode")]
        public string ASSETCODE { get; set; }

        //Change by aumento as on 02082023 for SR51823============================
        [DisplayName("Asset Class Name")]
        public string ASSETCLASSNAME { get; set; }

        [DisplayName("Cost Center")]
        public string COSTCENTER { get; set; }
        //========================================================================

        [DisplayName("Asset Desc.")]
        public string ASSETDESCRIPTION { get; set; }

        [DisplayName("Asset Detail")]
        public string ASSETDETAIL { get; set; }

        [DisplayName("Vendor Name")]
        public string VENDORNAME { get; set; }

        [DisplayName("Vendor Code")]
        public string VENDORCODE { get; set; }

        [DisplayName("Invoice No.")]
        public string? INVOICENO { get; set; }

        [DisplayName("Invoice Date")]
        public string? INVOICEDATE { get; set; }

        [DisplayName("Bill Entry")]
        public string? BILLENTRY { get; set; }

        [DisplayName("License No.")]
        public string? LICENSENO { get; set; }

        [DisplayName("Purchased Date")]
        public string? PURCHASEDDATE { get; set; }

        [DisplayName("Original Cost")]
        public decimal ORIGINALCOST { get; set; }

        [DisplayName("Accumulated Depreciation")]
        public decimal DEPRECIATIONCOST { get; set; }

        [DisplayName("Net Value")]
        public decimal WDV { get; set; }

        [DisplayName("Sales Price")]
        public decimal SALESPRICE { get; set; }

        [DisplayName("Serial Number")]
        public string SERIALNUMBER { get; set; }

        [DisplayName("Plant")]
        public long PLANTID { get; set; }
        public string PLANTNAME { get; set; }
        public string PROFITCENTER { get; set; }

        [DisplayName("PO Number")]
        public string PONUMBER { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}"), DisplayName("Captalized Date")]
        public DateTime? CAPTALIZED_DATE { get; set; }
    }

    public class CustomerDetailViewModel
    {
        [DisplayName("Customer Code")]
        public string CUSTOMERCODE { get; set; }

        [DisplayName("Customer Name")]
        public string CUSTOMERNAME { get; set; }

        public string CUSTOMERNAME_CODE { get; set; }

        [DisplayName("Customer Address")]
        public string CUSTADDRESS { get; set; }

        public string CITY1 { get; set; }

        public string POSTALCODE { get; set; }

        [DisplayName("Customer GSTIN")]
        public string CUSTGSTIN { get; set; }

        [DisplayName("GST Rate")]
        public string GSTRATE { get; set; }

        [DisplayName("Payment Details")]
        public string PAYMENTDETAILS { get; set; }
    }

    public class ApprovalAuthorityViewModel
    {
        public long APPAUTHID { get; set; }

        [Required, DisplayName("Emp Code")]
        public long EMPCODE { get; set; }

        [DisplayName("Employee")]
        public string EMPNAME { get; set; }

        public string EMPFNAME { get; set; }

        public string EMPLNAME { get; set; }

        public string EMPEMAIL { get; set; }

        [Required, DisplayName("Site")]
        public long? SYSITEID { get; set; }

        public string SITE { get; set; }

        [Required, DisplayName("Org Level")]
        public long ADORGLEVELID { get; set; }

        public string ORGLEVEL { get; set; }

        [Required, DisplayName("Approval Type")]
        public long APPROVAL_STATUSID { get; set; }

        public string APPROVALTYPE { get; set; }

        public short STATUS_LEVEL { get; set; }

        public short CATEGORY { get; set; }

        public long PROCESS_STATUSID { get; set; }

        public long ADDEDBY { get; set; }

        [DisplayName("Added By")]
        public string ADDEDBY_NAME { get; set; }

        [DisplayName("Added Date"), DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? DATEADDED { get; set; }

        [DisplayName("Modified By"), DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public long? MODIFIEDBY { get; set; }

        [DisplayName("Modified Date")]
        public DateTime? DATELSTMOD { get; set; }

        [DisplayName("Status")]
        public bool ACTIVE { get; set; }
    }

    public class ProcessStatusViewModel
    {
        public long PROCESSSTATUS_MSTID { get; set; }

        [DisplayName("Process Status")]
        public string STATUSDESCRIPTION { get; set; }

        [DisplayName("Added By")]
        public long ADDEDBY { get; set; }

        [DisplayName("Added Date"), DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime DATEADDED { get; set; }

        [DisplayName("Modified By")]
        public long? MODIFIEDBY { get; set; }

        [DisplayName("Modified Date"), DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? DATELSTMOD { get; set; }

        [DisplayName("Status")]
        public short ACTIVE { get; set; }

        [DisplayName("Process Level")]
        public short STATUS_LEVEL { get; set; }
    }

    public class ValidationViewModel
    {
        public long VALIDATIONID { get; set; }

        [DisplayName("Parameter")]
        public long PARAM_MSTID { get; set; }

        public virtual ParamMstViewModel Parm_mst { get; set; }

        [DisplayName("Param Value")]
        public string PARAMVALUE { get; set; }

        [DisplayName("Location")]
        public long?[] SITE { get; set; }

        public long ADDEDBY { get; set; }

        [DisplayName("Added By")]
        public string ADDEDBY_NAME { get; set; }

        [DisplayName("Added Date"), DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime DATEADDED { get; set; }

        [DisplayName("Modified By")]
        public long? MODIFIEDBY { get; set; }

        [DisplayName("Modified Date"), DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? DATELSTMOD { get; set; }

        [DisplayName("Status")]
        public bool ACTIVE { get; set; }
    }

    public class ParamMstViewModel
    {
        public long PARAM_MSTID { get; set; }

        [DisplayName("Param Name")]
        public string PARAMNAME { get; set; }

        [DisplayName("Param Description")]
        public string PARAMDESCRIPTION { get; set; }

        [DisplayName("Value Type")]
        public string PARAMVALUETYPE { get; set; }

        public string PARAMTYPE { get; set; }

        [DisplayName("Added By")]
        public long ADDEDBY { get; set; }

        [DisplayName("Added Date"), DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime DATEADDED { get; set; }

        [DisplayName("Modified By")]
        public long? MODIFIEDBY { get; set; }

        [DisplayName("Modified Date"), DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? DATELSTMOD { get; set; }

        [DisplayName("Status")]
        public bool ACTIVE { get; set; }
    }

    public class OPMappingViewModel
    {
        public long OPMAPPINGID { get; set; }

        [DisplayName("Org Level")]
        public long ADORGLEVELID { get; set; }

        [DisplayName("Org Level")]
        public string ORGLEVEL { get; set; }

        [DisplayName("Function Area Code")]
        public string FUNCTIONAREACODE { get; set; }

        public long ADDEDBY { get; set; }

        [DisplayName("Added By")]
        public string ADDEDBY_NAME { get; set; }

        [DisplayName("Added Date"), DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime DATEADDED { get; set; }

        [DisplayName("Modified By")]
        public long? MODIFIEDBY { get; set; }

        [DisplayName("Modified Date"), DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? DATELSTMOD { get; set; }

        [DisplayName("Status")]
        public bool ACTIVE { get; set; }
    }
    public class AssetOperationViewModel
    {
        public long ASSETOPID { get; set; }

        [DisplayName("Operation")]
        public long OPERATIONID { get; set; }

        [DisplayName("Operation")]
        public string OPERATION { get; set; }

        public long ADDEDBY { get; set; }

        [DisplayName("Added By")]
        public string ADDEDBY_NAME { get; set; }

        [DisplayName("Added Date"), DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime DATEADDED { get; set; }

        [DisplayName("Modified By")]
        public long? MODIFIEDBY { get; set; }

        [DisplayName("Modified Date"), DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? DATELSTMOD { get; set; }

        [DisplayName("Status")]
        public bool ACTIVE { get; set; }
    }
    #endregion

    public class UserApprovalAuthority
    {
        public long Empcode { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public long? OrgLevel { get; set; }

        public long? SectionManager { get; set; }

        public long? DepartmentManager { get; set; }

        public long? DivisionHead { get; set; }

        public long? OperationHead { get; set; }

        public long? Coordinator { get; set; }

        public long? EXECoordinator { get; set; }

        public long? Director { get; set; }

        public long? DivisionId { get; set; }

        public long? OperationId { get; set; }

        public short? IsSkip { get; set; }
        public short? IsSkipExe { get; set; }
        public long? Director2 { get; set; }

    }
    public class AssetRequest
    {
        public long Id { get; set; }
        public string PageType { get; set; }
    }

    public class RequestReport
    {
        public long AssetType { get; set; }
        public long EmpCode { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
    public class PrintRequestModel
    {
        public AssetDisposalViewModel ADVM { get; set; }
        public int finalPrint { get; set; }
    }
}
