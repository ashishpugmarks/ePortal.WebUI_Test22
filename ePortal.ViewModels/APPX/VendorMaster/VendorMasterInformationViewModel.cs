using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.VendorMaster
{
    #region Vendor Master

    #region CSV
    public class Vendor
    {
        public string Result { get; set; }
        public string REQUESTTYPE { get; set; }
        public string VENDORACCGRP { get; set; }
        public string VENDORCODE { get; set; }

        public string VENDORNAME1 { get; set; }
        public string VENDORNAME2 { get; set; }
        public string VENDORNAME3 { get; set; }

        public string STREET1 { get; set; }
        public string STREET2 { get; set; }
        public string STREET3 { get; set; }
        public string STREET4 { get; set; }

        public string CITY { get; set; }
        public string REGION { get; set; }
        public string POSTALCODE { get; set; }
        public string COUNTRY { get; set; }

        public string MOBILENO { get; set; }
        public string TELEPHONENO { get; set; }

        public string EMAIL1 { get; set; }
        public string EMAIL2 { get; set; }
        public string EMAIL3 { get; set; }

        public string MSMEINFOSTATUS { get; set; }
        public string MSMECATEGORY { get; set; }
        public string MSMECERTIFICATION { get; set; }
        public string SERVICEAGENTGRP { get; set; }

        public string MSMEFROM { get; set; }
        public string MSMETO { get; set; }
        public string MSMECITY { get; set; }

        public string TypeofIndustry { get; set; }
        public string ClassificationOfYear { get; set; }
        public string DateOfClassification { get; set; }

        public string BANKACCNO { get; set; }
        public string BANKNAME { get; set; }
        public string BANKADDRESS { get; set; }
        public string BANKCITY { get; set; }
        public string BANKSTATE { get; set; }
        public string BANKCOUNTRY { get; set; }
        public string BRANCHNAME { get; set; }
        public string TYPEOFACCOUNT { get; set; }
        public string IFSCCODE { get; set; }
        public string BANKCATEGORY { get; set; }
        public string SCHEMAGROUP { get; set; }
        public string ORDERCURRENCY { get; set; }
        public string SWIFTCODE { get; set; }
        public string IBANNO { get; set; }
        public string REFERENCE_DETAIL { get; set; }

        public string I_BANKACCNO { get; set; }
        public string I_BANKNAME { get; set; }
        public string I_BANKADDRESS { get; set; }
        public string I_BANKCITY { get; set; }
        public string I_BANKSTATE { get; set; }
        public string I_BANKCOUNTRY { get; set; }
        public string I_BRANCHNAME { get; set; }
        public string I_TYPEOFACCOUNT { get; set; }
        public string I_BANKCATEGORY { get; set; }
        public string I_SWIFTCODE { get; set; }
        public string I_IBANNO { get; set; }
        public string I_BANK_KEY { get; set; }
        public string I_REFERENCE_DETAIL { get; set; }

        public string PANNUMBER { get; set; }
        public string GSTIN { get; set; }
        public string GSTCLASSIFICATION { get; set; }
        public string E_INVOICEApplicable { get; set; }
        public string LEIAPPLICABLE { get; set; }
        public string LEINO { get; set; }

        public HiddenFields Hidden { get; set; } = new HiddenFields();
    }

    public class HiddenFields
    {
        public string HDVENDORNAME { get; set; }
        public string HDVENDORADDRESS { get; set; }
        public string HDADDRESS1 { get; set; }
        public string HDADDRESS2 { get; set; }
        public string HDADDRESS3 { get; set; }
        public string HDVENDORCITY { get; set; }
        public string HDVENDORACNO { get; set; }
        public string HDVENDORPANNO { get; set; }
    }
    #endregion

    #region VendorMasterForm
    public class VendorFormViewModel
    {
        public bool CreateVendor { get; set; }

        public bool GeneralPurchase { get; set; }
        public bool GPImport { get; set; }
        public bool GroupCompanies { get; set; }
        public bool BOPVendor { get; set; }
        public bool ContractualEmp { get; set; }

        public string VendorAccountGroup { get; set; }
        public string VendorCode { get; set; }

        public bool ShowAccGroup { get; set; }
        public bool ShowAccGroupList { get; set; }
        public string AccGroupLabel { get; set; }
        public bool ShowVendorCode { get; set; }
    }
    public class VendorMasterViewModel
    {
        public IEnumerable<SelectListItem>? WithHoldingTaxTypeList { get; set; }
        public IEnumerable<SelectListItem>? WithHoldingTaxCodeList { get; set; }
        public string? SelectedWithHoldingTaxType { get; set; }
        public string? SelectedWithHoldingTaxCode { get; set; }
    }
    public class ParseCsvRequest
    {
        public string FileName { get; set; }         // returned from UploadFile, e.g. "original#timestamp.csv"
        public string RequestType { get; set; }      // "1" or "2"
        public string VendorAccGrp { get; set; }     // "5AG1","5AG2","5AG3","5AGB","5AGA"
        public string VendorCode { get; set; }       // optional/required depending on flow
    }
    public class VendorMatchRequest
    {
        public string VendorName { get; set; }
        public string VendorAddress { get; set; }
        public string VendorCity { get; set; }
        public string VendorAcNo { get; set; }
        public string VendorPanNo { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
    }
    public class VendorListMatchResponse
    {
        public Ods1Response ods1 { get; set; }
        public Ods2Response ods2 { get; set; }
        public OdsExistsResponse odsExists { get; set; }
    }

    public class Ods1Response
    {
        public IEnumerable<IDictionary<string, object>> Table0 { get; set; }
        public IEnumerable<IDictionary<string, object>> Table1 { get; set; }
        public IEnumerable<IDictionary<string, object>> Table2 { get; set; }
        public IEnumerable<IDictionary<string, object>> Table3 { get; set; }
        public IEnumerable<IDictionary<string, object>> Table4 { get; set; }
    }

    public class Ods2Response
    {
        public IEnumerable<IDictionary<string, object>> Table0 { get; set; }
        public IEnumerable<IDictionary<string, object>> Table1 { get; set; }
    }

    public class OdsExistsResponse
    {
        public IEnumerable<IDictionary<string, object>> Table0 { get; set; }
    }
    public class VendorDto
    {
        public string VENDORACCOUNTGRP { get; set; }
        public string VENDORACCGRPID { get; set; }
        public string VENDORNAME { get; set; }
        public string VENDORCODE { get; set; }
        public string STREET { get; set; }
        public string CITY { get; set; }
        public string REGION { get; set; }
        public string POSTALCODE { get; set; }
        public string COUNTRY { get; set; }
        public string MOBILENO { get; set; }
        public string TELEPHONENO { get; set; }
        public string EMAIL1 { get; set; }
        public string EMAIL2 { get; set; }
        public string EMAIL3 { get; set; }
        public string MSMESTATUSINFO { get; set; }
        public string MSMECATEGORY { get; set; }
        public string MSMECERTIFICATENO { get; set; }
        public string SERVICEAGENTGROUP { get; set; }
        public string MSMEFROM { get; set; }
        public string MSMETO { get; set; }
        public string MSMECITY { get; set; }
        public string TYPE_OF_INDUSTRY { get; set; }
        public string CLASSIFICATION_OF_YEAR { get; set; }
        public string DATE_OF_CLASSIFICATION { get; set; }
        public string BANKACCOUNTNO { get; set; }
        public string BANKNAME { get; set; }
        public string BANKADDRESS { get; set; }
        public string BANKCITY { get; set; }
        public string BANKREGION { get; set; }
        public string BANKCOUNTRY { get; set; }
        public string BRANCHNAME { get; set; }
        public string TYPEOFACCOUNT { get; set; }
        public string IFSCCODE { get; set; }
        public string BANKCATEGORY { get; set; }
        public string SCHEMAGROUP { get; set; }
        public string ORDERCURRENCY { get; set; }
        public string REFERENCE_DETAIL { get; set; }
        public string SWIFT_CODE { get; set; }
        public string IBAN_NO { get; set; }
        public string IBANK_KEY { get; set; }
        public string IBANK_ACC_NO { get; set; }
        public string IBANK_NAME { get; set; }
        public string IADDRESS { get; set; }
        public string ICITY { get; set; }
        public string IREGION_STATE { get; set; }
        public string ICOUNTRY { get; set; }
        public string IBRANCH_NAME { get; set; }
        public string ITYPE_OF_ACCOUNT { get; set; }
        public string ISWIFT_CODE { get; set; }
        public string IBANK_CATEGORY { get; set; }
        public string IREFERENCE_DETAIL { get; set; }
        public string PANNO { get; set; }
        public string GSTIN { get; set; }
        public string GSTCLASSIFICATION { get; set; }
        public string EINVOICE { get; set; }
        public string LEI_APPLICABLE { get; set; }
        public string LEINO { get; set; }
        // add any other fields you may need
    }
    public class VendorCsvParseResult
    {
        public string lblvendorname { get; set; }
        public string lblstreethouseno { get; set; }
        public string lblbranchname { get; set; }
        public string lblbankaddress { get; set; }
        public string LblVendorNameCode { get; set; }

        public string lblcity { get; set; }
        public string lblregion { get; set; }
        public string lblpostalcode { get; set; }
        public string lblcountry { get; set; }
        public string lblmobilenumber { get; set; }
        public string lbltelphoneno { get; set; }
        public string lblemailid1 { get; set; }
        public string lblemailid2 { get; set; }
        public string lblemailid3 { get; set; }
        public string lblmsmeinfostatus { get; set; }
        public string lblmsmecategory { get; set; }
        public string lblmsmecertno { get; set; }
        public string lblmsmefrom { get; set; }
        public string lblmsmeto { get; set; }
        public string lblmsmecity { get; set; }
        public string lblNTypeOfIndustry { get; set; }
        public string lblNClassificationOfYear { get; set; }
        public string lblNDateOfClassification { get; set; }
        public string lblserviceagentgrp { get; set; }

        public string lblbankaccountno { get; set; }
        public string lblbankname { get; set; }
        public string lblbankcity { get; set; }
        public string lblbankstate { get; set; }
        public string lblbankcountry { get; set; }
        public string lbltypeofaccount { get; set; }
        public string lblifsccode { get; set; }
        public string lblswiftcode { get; set; }
        public string lblibanno { get; set; }
        public string lblbankcategory { get; set; }
        public string lblschemagroup { get; set; }
        public string lblordercurrency { get; set; }
        public string lblreferencedetails { get; set; }

        public string lblibankkey { get; set; }
        public string lblibankaccountno { get; set; }
        public string lblibankname { get; set; }
        public string lblibankaddress { get; set; }
        public string lblibankcity { get; set; }
        public string lblibankstate { get; set; }
        public string lblibankcountry { get; set; }
        public string lblibranchname { get; set; }
        public string lblitypeofaccount { get; set; }
        public string lbliswiftcode { get; set; }
        public string lbliibanno { get; set; }
        public string lblibankcategory { get; set; }
        public string lblireferenceddetails { get; set; }

        public string lblpannumber { get; set; }
        public string lblgstin { get; set; }
        public string lblgstclassification { get; set; }
        public string lblEinvoiceApplicable { get; set; }
        public string lblleiapplicable { get; set; }
        public string lblleino { get; set; }

        // UI flags
        public bool rdogpimportChecked { get; set; }
        public IEnumerable<SelectListItem>? ForwardTo { get; set; }
    }
    public class WithholdingTaxModel
    {
        //public decimal Id { get; set; }
        public string? Withholding_Tax_Type { get; set; }
        public string? Withholding_Tax_Code { get; set; }
        public bool Liable { get; set; }
        public string? Recipient_Type { get; set; }
        public string? Withholding_Tax_Id_No { get; set; }
        public string? Exemption_Certi_No { get; set; }
        public string? Exemption_Rate { get; set; }
        public DateTime? Date_On_Which_Exemption_Begins { get; set; }
        public DateTime? Date_On_Which_Exemption_Ends { get; set; }
        public string? Reason_For_Exemption { get; set; }
    }
    public class VendorMasterDetails
    {
        public string VendorHeaderId { get; set; }
        public string REQUESTID { get; set; }
        public bool chkWithHoldingTax { get; set; }
        public bool chkdeclaration { get; set; }
        public string HDCSVVENDORMSTTEMPLATE { get; set; }      // optional path or filename client provides
        public string csvfilename { get; set; }

        public string FORWARDTO { get; set; } = string.Empty;
        public List<WithholdingTaxModel> withholdingtax { get; set; } // map to DataTable server-side or strong-type array

        // files as base64 (filename + base64 content)
        public IFormFile HDFILE_REGCERTIFICATENO { get; set; }
        public IFormFile HDFILE_MANDATEFORM { get; set; }
        public IFormFile HDFILE_CANCELCHEQUE { get; set; }
        public IFormFile HDFILE_PANCARD { get; set; }
        public IFormFile HDFILE_MSMECERTIFICATE { get; set; }
        public IFormFile HDNFILE_GSTCERTIFICATE { get; set; }
        public IFormFile HDNFILE_LEI { get; set; }
        public IFormFile HDNFILE_EINVOICE { get; set; }
        public IFormFile HDFILE_conflictCERTIFICATE { get; set; }
        public string? REQECODE { get; set; }
        public string? REQUESTTYPE { get; set; }
        public string? VENDORACCGRP { get; set; }
        public string? VENDORCODE { get; set; }
        public string? CSVATTACHMENT { get; set; }
        public string? DEPTHEAD { get; set; }
        public string? DEPTHEADNAME { get; set; }
        public string? DEPTHEADEMAIL { get; set; }
        public string? VENDORNAME1 { get; set; }
        public string? VENDORNAME2 { get; set; }
        public string? VENDORNAME3 { get; set; }
        public string? STREET1 { get; set; }
        public string? STREET2 { get; set; }
        public string? STREET3 { get; set; }
        public string? STREET4 { get; set; }
        public string? CITY { get; set; }
        public string? REGION { get; set; }
        public string? POSTALCODE { get; set; }
        public string? COUNTRY { get; set; }
        public string? MOBILENO { get; set; }
        public string? TELEPHONENO { get; set; }
        public string? EMAIL1 { get; set; }
        public string? EMAIL2 { get; set; }
        public string? EMAIL3 { get; set; }
        public string? MSMEINFOSTATUS { get; set; }
        public string? MSMECATEGORY { get; set; }
        public string? MSMECERTIFICATION { get; set; }
        public string? SERVICEAGENTGRP { get; set; }
        public string? BANKCOUNTRY { get; set; }
        public string? BANKNAME { get; set; }
        public string? BRANCHNAME { get; set; }
        public string? BANKADDRESS { get; set; }
        public string? TYPEOFACCOUNT { get; set; }
        public string? BANKCITY { get; set; }
        public string? BANKSTATE { get; set; }
        public string? BANKACCNO { get; set; }
        public string? IFSCCODE { get; set; }
        public string? SWIFTCODE { get; set; }
        public string? IBANNO { get; set; }
        public string? BANKCATEGORY { get; set; }
        public string? SCHEMAGROUP { get; set; }
        public string? ORDERCURRENCY { get; set; }
        public string? PANNUMBER { get; set; }
        public string? CSTREGNUMBER { get; set; }
        public string? LSTNUMBER { get; set; }
        public string? SERVICEREGNUMBER { get; set; }
        public string? ECCNUMBER { get; set; }
        public string? EXCISEREGNO { get; set; }
        public string? EXCISERANGE { get; set; }
        public string? EXCISEDIVISION { get; set; }
        public string? COMMISTIONERATE { get; set; }
        public string? FILE_REGCERTIFICATENO { get; set; }
        public string? FILE_MANDATEFORM { get; set; }
        public string? FILE_CANCELCHEQUE { get; set; }
        public string? FILE_PANCARD { get; set; }
        public string? FILE_SERVICEREGCERTIFICATE { get; set; }
        public string? FILE_CSTCERTIFICATE { get; set; }
        public string? FILE_EXCISEREGCERTIFICATE { get; set; }
        public string? FILE_MSMECERTIFICATE { get; set; }
        public string? GSTIN { get; set; }
        public string? GSTCLASSIFICATION { get; set; }
        public string? REMARKS { get; set; }
        public string? FILE_GSTCERTIFICATE { get; set; }
        public short E_INVOICEApplicable { get; set; }
        public string? FILE_EINVOICE { get; set; }
        public string? MSMEFROM { get; set; }
        public string? MSMETO { get; set; }
        public string? MSMECITY { get; set; }
        public string? LEIAPPLICABLE { get; set; }
        public string? LEINO { get; set; }
        public string? FILE_LEICERTIFICATE { get; set; }
        public string? SPLAPPROVAL { get; set; }
        public string? FILE_CONFLICTCERTIFICATE { get; set; }
        public string I_BANKACCNO { get; set; } = string.Empty;
        public string I_BANKNAME { get; set; } = string.Empty;
        public string I_BANKADDRESS { get; set; } = string.Empty;
        public string I_BANKCITY { get; set; } = string.Empty;
        public string I_BANKSTATE { get; set; } = string.Empty;
        public string I_BANKCOUNTRY { get; set; } = string.Empty;
        public string I_BRANCHNAME { get; set; } = string.Empty;
        public string I_TYPEOFACCOUNT { get; set; } = string.Empty;
        public string I_BANKCATEGORY { get; set; } = string.Empty;
        public string I_SWIFTCODE { get; set; } = string.Empty;
        public string I_IBANNO { get; set; } = string.Empty;
        public string I_BANK_KEY { get; set; } = string.Empty;
        public string I_REFERENCE_DETAIL { get; set; } = string.Empty;
        public string? REFERENCE_DETAIL { get; set; }
        public string? INTER_REFERENCE_DETAIL { get; set; }
        public string? TYPE_OF_INDUSTRY { get; set; }
        public string? CLASSIFICATION_OF_YEAR { get; set; }
        public string? DATE_OF_CLASSIFICATION { get; set; }
        public string? LEI_APPLICABLE { get; set; } = string.Empty;

        // 🔹 Anchor properties (fallback values from client-side dto.append)
        public string AnchRegnCertificateno_enable { get; set; }
        public string AnchMsmeCertificate_enable { get; set; }
        public string AnchConfCertificate_enable { get; set; }
        public string AnchmandateForm_enable { get; set; }
        public string AnchCancelCheque_enable { get; set; }
        public string AnchPanCard_enable { get; set; }
        public string AnchGSTCertificate_enable { get; set; }
        public string AnchEINVOICE_enable { get; set; }
        public string AnchLEIC_enable { get; set; }
    }
    public class UploadResult
    {
        public bool Status { get; set; }
        public string Emsg { get; set; } = string.Empty;
        public string Filename { get; set; } = string.Empty;
    }
    #endregion

    #region ManageRequest
    public class VendorMasterFilter
    {
        public string status { get; set; }
        public string vendorHeaderId { get; set; }
        public string requestType { get; set; }
        public string vendorAccGroup { get; set; }
        public string vendorName { get; set; }
        public string reqDateFrom { get; set; }
        public string reqDateTo { get; set; }
    }
    public class VendorRequest
    {
        // Header Info
        public string EncryptedVendorHeaderId { get; set; }
        public string VendorHeaderId { get; set; }
        public int RequestType { get; set; }
        public string RequestTypeDesc { get; set; }
        public string Syki { get; set; }
        public string VendorAccountGroupId { get; set; }
        public string VendorAccountGroup { get; set; }
        public int ProcessStatus { get; set; }
        public string RequestorId { get; set; }
        public string RequestorName { get; set; }
        public string RequestDate { get; set; }
        public string StatusDescription { get; set; }

        // Vendor Details
        public string VendorCode { get; set; }
        public string VendorName1 { get; set; }
        public string VendorName2 { get; set; }
        public string VendorName3 { get; set; }
        public string VendorName { get; set; }
        public string RegionId { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string Street3 { get; set; }
        public string Street4 { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string CountryId { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string MobileNo { get; set; }
        public string TelephoneNo { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public string Email3 { get; set; }
        public string MsmeStatusInfo { get; set; }
        public string MsmeCategory { get; set; }
        public string MsmeCertificateNo { get; set; }
        public string ServiceAgentGroup { get; set; }

        // Bank Details
        public string BankAccountNo { get; set; }
        public string BankName { get; set; }
        public string BankAddress { get; set; }
        public string BankCity { get; set; }
        public string BankRegion { get; set; }
        public string BankRegionDesc { get; set; }
        public string BankCountryId { get; set; }
        public string BankCountry { get; set; }
        public string BranchName { get; set; }
        public string TypeOfAccountId { get; set; }
        public string AccountTypeDescription { get; set; }
        public string IfscCode { get; set; }
        public string BankCategory { get; set; }
        public string SchemaGroup { get; set; }
        public string OrderCurrency { get; set; }
        public string ReferenceDetail { get; set; }
        public string SwiftCode { get; set; }
        public string IbanNo { get; set; }

        // Tax Details
        public string PanNo { get; set; }
        public string CstRegNo { get; set; }
        public string LstNo { get; set; }
        public string ServiceRegNo { get; set; }
        public string EccNo { get; set; }
        public string ExciseRegNo { get; set; }
        public string ExciseRange { get; set; }
        public string ExciseDivision { get; set; }
        public string Commissionerate { get; set; }

        // Attachments
        public string CsvFile { get; set; }
        public string RegistrationCertAttach { get; set; }
        public string MandateAttachment { get; set; }
        public string CcAttachment { get; set; }
        public string PanAttachment { get; set; }
        public string ServiceAttachment { get; set; }
        public string CstCertAttachment { get; set; }
        public string ExciseAttachment { get; set; }
        public string MsmeCertAttachment { get; set; }
    }
    public class VendorWithholdingTax
    {
        public string VendorHeaderId { get; set; }
        public string WithholdingTaxType { get; set; }
        public string WithholdingTaxCode { get; set; }
        public string Liable { get; set; }
        public string RecipientType { get; set; }
        public string WithholdingTaxIdNo { get; set; }
        public string ExemptionCertificateNo { get; set; }
        public decimal? ExemptionRate { get; set; }
        public string ExemptionBegins { get; set; }
        public string ExemptionEnds { get; set; }
        public string ReasonForExemption { get; set; }
    }
    public class VendorRequestResult
    {
        public List<VendorRequest> PendingRequests { get; set; }
        public List<VendorWithholdingTax> WithholdingTaxes { get; set; }
    }
    #endregion

    #region ViewVendorMaster
    public class VendorDetailsViewModel
    {
        public string EncryptedVendorHeaderId { get; set; }
        public OldVendorDetailsViewModel? OldVendorDetails { get; set; }
        public VendorRequestViewModel? VendorRequest { get; set; }
        public IntermediaryBankViewModel? IntermediaryBank { get; set; }
        public bool ShowIntermediaryBank { get; set; }

        // Withholding data (generic row-based structure)
        public List<Dictionary<string, string>> WithholdingData { get; set; } = new List<Dictionary<string, string>>();
        public bool ShowWithholding { get; set; }
        public bool ShowWithholdingDiv { get; set; }

        // Process history (generic, column-agnostic)
        public List<Dictionary<string, string>> ProcessHistoryData { get; set; } = new List<Dictionary<string, string>>();
        public bool ShowProcessHistory { get; set; }

        // Match Details
        public VendorMatchViewModel? VendorMatch { get; set; }
        public IEnumerable<SelectListItem>? SPAppAuthorityList { get; set; }
        public string? SelectedSPAuthority { get; set; }
        public string? SelectedStatus { get; set; }
        public string? ApprovalRemarks { get; set; }
        public bool IsDeclarationChecked { get; set; }
    }
    public class OldVendorDetailsViewModel
    {
        // Vendor Info
        public string VendorAccountGroup { get; set; }
        public string VendorName { get; set; }
        public string VendorCode { get; set; }
        public string StreetHouseNo { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string MobileNumber { get; set; }
        public string TelephoneNumber { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public string Email3 { get; set; }

        // MSME Info
        public string MSMEStatusInfo { get; set; }
        public string MSMECategory { get; set; }
        public string MSMECertificateNo { get; set; }
        public string ServiceAgentGroup { get; set; }
        public string MSMEFrom { get; set; }
        public string MSMETo { get; set; }
        public string MSMECity { get; set; }
        public string TypeOfIndustry { get; set; }
        public string ClassificationOfYear { get; set; }
        public string DateOfClassification { get; set; }

        // Bank Details
        public string BankAccountNo { get; set; }
        public string BankName { get; set; }
        public string BankAddress { get; set; }
        public string BankCity { get; set; }
        public string BankState { get; set; }
        public string BankCountry { get; set; }
        public string BranchName { get; set; }
        public string TypeOfAccount { get; set; }
        public string IFSCCode { get; set; }
        public string BankCategory { get; set; }
        public string SchemaGroup { get; set; }
        public string OrderCurrency { get; set; }
        public string SwiftCode { get; set; }
        public string IBANNo { get; set; }
        public string ReferenceDetail { get; set; }

        // Tax Details
        public string PANNumber { get; set; }
        public string GSTIN { get; set; }
        public string GSTClassification { get; set; }
        public string EInvoice { get; set; }
        public string LEIApplicable { get; set; }
        public string LEINo { get; set; }
    }
    public class VendorRequestViewModel
    {
        // Control visibility of the two rows
        public bool ShowTrFin { get; set; } = true;
        public bool ShowTrSP { get; set; } = true;

        // Optional message already used
        public string? SPSpecialMessage { get; set; }
        // Requestor & request meta
        public string RequestorName { get; set; }
        public string RequestDate { get; set; }
        public string RequestorEmail { get; set; }
        public string SpecialApproverId { get; set; }
        public string RequestorMobile { get; set; }
        public string Site { get; set; }
        public string Operation { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string RequestType { get; set; }

        // Vendor identifiers
        public string VendorHeaderId { get; set; }
        public string VendorAccountGroup { get; set; }
        public string VendorCode { get; set; }
        public string InsertUpdateVendorCode { get; set; }

        // Vendor address & contact
        public string VendorName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string MobileNumber { get; set; }
        public string TelephoneNumber { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public string Email3 { get; set; }

        // MSME & industry
        public string MsmeStatusInfo { get; set; }
        public string MsmeCategory { get; set; }
        public string MsmeCertificateNumber { get; set; }
        public string ServiceAgentGroup { get; set; }
        public string TypeOfIndustry { get; set; }
        public string ClassificationOfYear { get; set; }
        public string DateOfClassification { get; set; }

        // Bank details
        public string BankAccountNo { get; set; }
        public string BankName { get; set; }
        public string BankAddress { get; set; }
        public string BankCity { get; set; }
        public string BankState { get; set; }
        public string BankCountry { get; set; }
        public string BranchName { get; set; }
        public string AccountTypeDescription { get; set; }
        public string IfscCode { get; set; }
        public string BankCategory { get; set; }
        public string SchemaGroup { get; set; }
        public string OrderCurrency { get; set; }
        public string SwiftCode { get; set; }
        public string IbanNo { get; set; }
        public string ReferenceDetail { get; set; }

        // Tax details
        public string PanNo { get; set; }
        public string Gstin { get; set; }
        public string GstClassification { get; set; }
        public string Remarks { get; set; }
        public string MsmeFrom { get; set; }
        public string MsmeTo { get; set; }
        public string MsmeCity { get; set; }
        public string LeiApplicable { get; set; }
        public string LeiNo { get; set; }

        // Attachments (URLs or file names)
        public string RegistrationCertificateUrl { get; set; }
        public string ConflictCertificateUrl { get; set; }
        public string MandateFormUrl { get; set; }
        public string CancelChequeUrl { get; set; }
        public string PanCardUrl { get; set; }
        public string MsmeCertificateUrl { get; set; }
        public string GstRegistrationUrl { get; set; }
        public string IsEinvoiceApplicable { get; set; }
        public string EinvoiceUrl { get; set; }
        public string LeiAttachmentUrl { get; set; }

        // Verification lebel's visibility
        public bool RegCertVerVisible { get; set; }
        public bool ConfCertVerVisible { get; set; }
        public bool MandateVerVisible { get; set; }
        public bool CancelChqVerVisible { get; set; }
        public bool PanVerVisible { get; set; }
        public bool MsmeVerVisible { get; set; }
        public bool GstVerVisible { get; set; }
        public bool EinvoiceVerVisible { get; set; }
        public bool LeiVerVisible { get; set; }


        // UI flags
        public bool ShowGeneralInfo { get; set; }
        public bool ShowBankDetails { get; set; }
        public bool ShowTaxDetails { get; set; }
        public bool ShowHoldingTax { get; set; }
        public bool ShowVendorCodeRow { get; set; }
        public bool ShowVendorCodeBanner { get; set; }
    }
    public class IntermediaryBankViewModel
    {
        public string BankAccountNo { get; set; }
        public string BankName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string BankState { get; set; }
        public string BankCountry { get; set; }
        public string BranchName { get; set; }
        public string AccountTypeDescription { get; set; }
        public string BankCategory { get; set; }
        public string SwiftCode { get; set; }
        public string IbanNo { get; set; }
        public string BankKey { get; set; }
        public string ReferenceDetail { get; set; }
    }
    public class VendorMatchViewModel
    {
        public bool ShowCheckMatch { get; set; }
        // Name - Exact and Partial
        public List<VendorDtlDto> VendorNamesExact { get; set; } = new();
        public List<VendorDtlDto> VendorNamesPartial { get; set; } = new();

        // Address - Exact and Partial
        public List<VendorAddressDto> VendorAddressesExact { get; set; } = new();
        public List<VendorAddressDto> VendorAddressesPartial { get; set; } = new();

        // Bank Detail - Exact
        public List<VendorAccountDto> VendorAccountsExact { get; set; } = new();

        // PAN No - Exact
        public List<VendorPanDto> VendorPANsExact { get; set; } = new();

        // Employee with similar address
        public List<EmployeeAddressDto> EmployeeAddresses { get; set; } = new();
    }
    #endregion

    #region ManageApprovalRequest
    public class VendorApprovalRequest
    {
        public string EncryptedVendorHeaderId { get; set; }
        public int VENDORHEADERID { get; set; }
        public string EMPLOYEE { get; set; }
        public string REQUESTTYPEDESC { get; set; }
        public string VENDORACCOUNTGRP { get; set; }
        public string REQUESTDATE { get; set; }
        public string VENDORNAME { get; set; }
        public string HISTORYSTATUS { get; set; }
        public string PROCESSSTATUS { get; set; }
        public int PSTATUSID { get; set; }
    }

    #endregion

    #region EditVendorMasterForm
    public class EditVendorFormViewModel
    {
        public string VMID { get; set; }
        public string VendorId { get; set; }
        public string RequestType { get; set; }
        public string ProcessStatus { get; set; }
        public string VendorAccountGpId { get; set; }
        public string VendorCode { get; set; }

        // Attachments
        public string RegistrationCert { get; set; }
        public string ConflictCert { get; set; }
        public string MsmeCert { get; set; }
        public string MandateForm { get; set; }
        public string CancelCheque { get; set; }
        public string PanCard { get; set; }
        public string GstCert { get; set; }
        public string EInvoiceApplicable { get; set; }
        public string EInvoiceFile { get; set; }
        public string LeiFile { get; set; }
        public string FilePath { get; set; }

        // Remarks
        public string SendBackRemark { get; set; }

        // InterBank & WithHolding
        public bool HasInterBank { get; set; }
        public List<Dictionary<string, object>> WithHolding { get; set; } = new();

        // Department heads dropdown
        public IEnumerable<SelectListItem>? DepartmentHeads { get; set; }
    }
    #endregion


    #region VendorApprovalForm
    public class VendorDtlDto
    {
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
    }

    public class VendorAddressDto : VendorDtlDto
    {
        public string Address { get; set; }
    }

    public class VendorAccountDto : VendorDtlDto
    {
        public string AccountNo { get; set; }
    }

    public class VendorPanDto : VendorDtlDto
    {
        public string PanNo { get; set; }
    }

    public class EmployeeAddressDto
    {
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string Address { get; set; }
    }

    public class VendorApprovalModel
    {
        public string Vmid { get; set; }                // encrypted VMID from querystring
        public string Status { get; set; }              // ddlstatus value
        public string Remarks { get; set; }             // txt_Remarks
        public bool RegCertVerified { get; set; }
        public bool ConflictCertVerified { get; set; }
        public bool MsmeVerified { get; set; }
        public bool CancelChequeVerified { get; set; }
        public bool MandateFormVerified { get; set; }
        public bool PanCardVerified { get; set; }
        public bool GstVerified { get; set; }
        public bool EInvoiceVerified { get; set; }
        public string SplAppId { get; set; }            // HDSPLAPPID.Value
        public bool MatchChecked { get; set; }          // chk_match
        public bool DeclarationChecked { get; set; }    // chkdeclaration
        public string AppAuthority { get; set; }        // cboSPAppAuthority (optional)


        public string RequestorName { get; set; }
        public string RequestorEmail { get; set; }
        public string RequestType { get; set; }
        public string AccGroup { get; set; }
        public string VendorName { get; set; }
        public string RequestDate { get; set; }
    }
    #endregion

    #endregion


    #region Vendor Block / Unblock

    #region VendorBlockForm
    public enum RequestCategory { Block, Unblock }
    public enum RequestType { SingleVendor, MassVendor }
    public class VendorListItem
    {
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
    }
    public class VendorBlockViewModel
    {
        [Required]
        public RequestCategory RequestCategory { get; set; } = RequestCategory.Block;
        [Required]
        public RequestType RequestType { get; set; } = RequestType.SingleVendor;
        public bool PurchasingData { get; set; }
        public bool PostingData { get; set; }

        [MaxLength(11)]
        public string? VendorCode { get; set; }
        public IFormFile? UploadFile { get; set; }
        public string? UploadedFileName { get; set; }
        public bool ShowDetails { get; set; }
        public bool ShowForm { get; set; }
        public string RequestTypeText => RequestType == RequestType.SingleVendor ? "Single Vendor" : "Mass Vendor";
        public string RequestCategoryText => RequestCategory == RequestCategory.Block ? "Block" : "Unblock";
        public List<VendorListItem> VendorList { get; set; } = new List<VendorListItem>();
        public string? Remarks { get; set; }
        public string? ForwardTo { get; set; }
        public string? VendorCodeList { get; set; }
        public string? VendorHeaderId { get; set; }
        public IEnumerable<SelectListItem>? ForwardToList { get; set; }


        public string? LabelPurchasing { get; set; }
        public string? LabelPosting { get; set; }
        public bool ErrMsgDivVisible { get; set; } = false;
        public string ErrMsgDivHtml { get; set; } = string.Empty;
        public bool ErrorFlag { get; set; } = false;
        public string Message { get; set; } = string.Empty;

        public string DeptHead{ get; set; } = string.Empty;
        public string DeptHeadName { get; set; } = string.Empty;
        public string DeptHeadEmail { get; set; } = string.Empty;
    }
    #endregion

    #region ManageBlockRequest
    public class VendorBlockFilter
    {
        public string Status { get; set; }
        public string RequestType { get; set; }
        public string RequestCategory { get; set; }
        public string VendorCode { get; set; }
        public string ReqDateFrom { get; set; }
        public string ReqDateTo { get; set; }
        public string VendorHeaderId { get; set; } = string.Empty;
    }

    public class VendorBlockPendingRequest
    {
        public string? EncVendorHeaderId { get; set; }
        public string? MyProperty { get; set; }
        public string? VENDORBLOCKHEADERID { get; set; }
        public string? REQUESTTYPE { get; set; }
        public string? REQUESTTYPEDESC { get; set; }
        public string? SYKI { get; set; }
        public int PROCESSSTATUS { get; set; }
        public string? REQUESTCATEGORYID { get; set; }
        public string? REQUESTCATE { get; set; }
        public string? REQUESTORID { get; set; }
        public string? REQUESTORNAME { get; set; }
        public string? REQUESTDATE { get; set; }
        public string? STATUSDESCRIPTION { get; set; }
    }
    #endregion

    #region ViewVendorBlock
    public class VendorBlockRequestViewModel
    {
        public string VendorHeaderId { get; set; }
        // Requestor Information
        public string RequestorName { get; set; }
        public string RequestDate { get; set; }
        public string RequestorEmail { get; set; }
        public string MobileNo { get; set; }
        public string Site { get; set; }
        public string Operation { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }

        // Request Details
        public string RequestTypeDesc { get; set; }
        public string PurchasingStatus { get; set; }
        public string PostingStatus { get; set; }
        public string PurchasingId { get; set; }
        public string PostingId { get; set; }
        public string Remarks { get; set; }
        public bool ShowTrApproval => PostingId != "0";

        // Collections
        public List<VendorListItem> Vendors { get; set; } = new List<VendorListItem>();
        public List<ProcessHistoryItem> ProcessHistory { get; set; } = new List<ProcessHistoryItem>();
        public string Status { get; set; }
    }

    public class ProcessHistoryItem
    {
        public string AppDate { get; set; }
        public string Employee { get; set; }
        public string StatusDescription { get; set; }
        public string Remarks { get; set; }
    }

    #endregion

    #region EditVendorBlockForm
    public class EditVendorBlockFormViewModel
    {
        public string VMID { get; set; }
        public string VendorHeaderId { get; set; }
        public string RequestType { get; set; }           // "1" = single, else mass
        public string RequestCategoryDesc { get; set; }   // "Block" / "Unblock"
        public string BlockPurchasing { get; set; }       // "1" or "0"
        public string BlockPosting { get; set; }          // "1" or "0"
        public string ProcessStatus { get; set; }         // e.g., "0"
        public string SendBackRemark { get; set; }
        public IEnumerable<SelectListItem> DepartmentHeads { get; set; }
    }
    #endregion

    #region ManageBlockApprovalRequest
    public class VendorBlockApprovalRequest
    {
        public string EncryptedVendorHeaderId { get; set; }
        public string VENDORBLOCKHEADERID { get; set; }
        public string VENDORBLOCKAPPROVALHISID { get; set; }
        public string EMPLOYEE { get; set; }
        public string REQUESTTYPEDESC { get; set; }
        public string REQUESTCATEDESC { get; set; }
        public string REQUESTDATE { get; set; }
        public string HISTORYSTATUS { get; set; }
        public string PROCESSSTATUS { get; set; }
        public int PSTATUSID { get; set; }
    }
    #endregion

    #endregion
}
