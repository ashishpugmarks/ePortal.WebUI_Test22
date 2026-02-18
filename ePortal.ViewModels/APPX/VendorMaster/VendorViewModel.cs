using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ePortal.ViewModels.APPX.VendorMaster
{
    public class VendorViewModel
    {
        //Visibility
        public string VendorAccGrp { get; set; } = string.Empty;
        public string RequestType { get; set; } = string.Empty;
        public string VENDORCODE { get; set; } = string.Empty;
        public IFormFile? csvfile { get; set; }
        public bool WTHOLDING_show { get; set; }
        public bool HOLDINGDIV_show { get; set; }

        // General
        public string SplitAppId { get; set; }
        public string VendorNameCode { get; set; }

        // Old / Existing info
        public string OldVendorAccountGroup { get; set; }
        public string OldVendorName { get; set; }
        public string OldVendorCode { get; set; }
        public string OldStreetHouseNo { get; set; }
        public string OldCity { get; set; }
        public string OldRegion { get; set; }
        public string OldPostalCode { get; set; }
        public string OldCountry { get; set; }
        public string OldMobileNumber { get; set; }
        public string OldTelephoneNo { get; set; }
        public string OldEmailId1 { get; set; }
        public string OldEmailId2 { get; set; }
        public string OldEmailId3 { get; set; }
        public string OldMsmeInfoStatus { get; set; }
        public string OldMsmeCertNo { get; set; }
        public string OldMsmeCategory { get; set; }
        public DateTime? OldMsmeFrom { get; set; }
        public DateTime? OldMsmeTo { get; set; }
        public string OldMsmeCity { get; set; }
        public string OldServiceAgentGroup { get; set; }
        public string TypeOfIndustry { get; set; }
        public string ClassificationOfYear { get; set; }
        public DateTime? DateOfClassification { get; set; }

        // New info (code2)
        public string TypeOfRequest { get; set; }
        public string VendorAccountGroup { get; set; }
        public string VendorName { get; set; }
        public string VendorCode { get; set; }
        public string StreetHouseNo { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string MobileNumber { get; set; }
        public string TelephoneNo { get; set; }
        public string EmailId1 { get; set; }
        public string EmailId2 { get; set; }
        public string EmailId3 { get; set; }
        public string MsmeInfoStatus { get; set; }
        public string MsmeCertNo { get; set; }
        public string MsmeCategory { get; set; }
        public DateTime? MsmeFrom { get; set; }
        public DateTime? MsmeTo { get; set; }
        public string MsmeCity { get; set; }
        public string ServiceAgentGroup { get; set; }
        public string ClassificationOfYearNew { get; set; }
        public DateTime? DateOfClassificationNew { get; set; }

        // Bank details (code3 & code4)
        public string OldBankAccountNo { get; set; }
        public string OldBankName { get; set; }
        public string OldBankAddress { get; set; }
        public string OldBankCity { get; set; }
        public string OldBankState { get; set; }
        public string OldBankCountry { get; set; }
        public string OldBranchName { get; set; }
        public string OldTypeOfAccount { get; set; }
        public string OldIfscCode { get; set; }
        public string OldSwiftCode { get; set; }
        public string OldIbanNo { get; set; }
        public string OldBankCategory { get; set; }
        public string OldSchemaGroup { get; set; }
        public string OldOrderCurrency { get; set; }
        public string OldReferenceDetails { get; set; }

        public string OldIBankAccountNo { get; set; }
        public string OldIBankName { get; set; }
        public string OldIBankAddress { get; set; }
        public string OldIBankCity { get; set; }
        public string OldIBankState { get; set; }
        public string OldIBankCountry { get; set; }
        public string OldIBranchName { get; set; }
        public string OldITypeOfAccount { get; set; }
        public string OldISwiftCode { get; set; }
        public string OldIibanNo { get; set; }
        public string OldIBankCategory { get; set; }
        public string OldIIReferenceDetails { get; set; }

        public string BankAccountNo { get; set; }
        public string BankName { get; set; }
        public string BankAddress { get; set; }
        public string BankCity { get; set; }
        public string BankState { get; set; }
        public string BankCountry { get; set; }
        public string BranchName { get; set; }
        public string TypeOfAccount { get; set; }
        public string IfscCode { get; set; }
        public string SwiftCode { get; set; }
        public string IbanNo { get; set; }
        public string BankCategory { get; set; }
        public string SchemaGroup { get; set; }
        public string OrderCurrency { get; set; }
        public string ReferenceDetails { get; set; }

        // Intermediary bank (code4)
        public string IbankKey { get; set; }
        public string IbankAccountNo { get; set; }
        public string IbankName { get; set; }
        public string IbankAddress { get; set; }
        public string IbankCity { get; set; }
        public string IbankState { get; set; }
        public string IbankCountry { get; set; }
        public string IbranchName { get; set; }
        public string ItypeOfAccount { get; set; }
        public string IswiftCode { get; set; }
        public string IibanNo { get; set; }
        public string IbankCategory { get; set; }
        public string IreferenceDetails { get; set; }

        // Tax details (code4)
        public string OldPanNumber { get; set; }
        public string OldGstin { get; set; }
        public string OldGstClassification { get; set; }
        public string OldEinvoice { get; set; }
        public string OldLeiApplicable { get; set; }
        public string OldLeiNo { get; set; }

        public string PanNumber { get; set; }
        public string Gstin { get; set; }
        public string GstClassification { get; set; }
        public string EinvoiceApplicable { get; set; }
        public string LeiApplicable { get; set; }
        public string LeiNo { get; set; }

        // Supporting documents file names (store names or ids)
        public string RegCertificateFileName { get; set; }
        public string MsmeCertificateFileName { get; set; }
        public string ConflictCertificateFileName { get; set; }
        public string MandateFormFileName { get; set; }
        public string CancelChequeFileName { get; set; }
        public string PancardFileName { get; set; }
        public string GstCertificateFileName { get; set; }
        public string EinvoiceFileName { get; set; }
        public string LeiFileName { get; set; }

        // File uploads (bind in controller)
        public IFormFile RegCertificateFile { get; set; }
        public IFormFile MsmeCertificateFile { get; set; }
        public IFormFile ConflictCertificateFile { get; set; }
        public IFormFile MandateFormFile { get; set; }
        public IFormFile CancelChequeFile { get; set; }
        public IFormFile PancardFile { get; set; }
        public IFormFile GstCertificateFile { get; set; }
        public IFormFile EinvoiceFile { get; set; }
        public IFormFile LeiFile { get; set; }

        // Matching lists (code5 & code6)
        public List<SimpleVendorDto> VendorNameExact { get; set; }
        public List<SimpleVendorDto> VendorNamePartial { get; set; }
        public List<SimpleVendorDto> VendorAddressExact { get; set; }
        public List<SimpleVendorDto> VendorAddressPartial { get; set; }
        public List<SimpleVendorDto> VendorAccountExact { get; set; }
        public List<SimpleVendorDto> VendorPanExact { get; set; }
        public List<SimpleVendorDto> EmployeeAddressSimilar { get; set; }

        // Withholding tax (code7 & code8)
        public List<SelectListItem> WithholdingTaxTypes { get; set; }
        public List<SelectListItem> WithholdingTaxCodes { get; set; }
        public string SelectedWithholdingTaxType { get; set; }
        public string SelectedWithholdingTaxCode { get; set; }
        public bool Liable { get; set; }
        public string RecipientType { get; set; }
        public string WithholdingTaxIdNo { get; set; }
        public string ExemptionCertificateNo { get; set; }
        public string ExemptionRate { get; set; }
        public string ReasonForExemption { get; set; }
        public DateTime? DateOnWhichExemptionBegin { get; set; }
        public DateTime? DateOnWhichExemptionEnds { get; set; }

        public List<WithholdingDto> WithholdingData { get; set; }

        // Final remarks and submission (code9)
        public string VendorRemarks { get; set; }
        public bool DeclarationAccepted { get; set; }
        public string ForwardTo { get; set; }
        public List<SelectListItem> ForwardToList { get; set; }
        public string CsvVendorMstTemplate { get; set; }

        // Misc
        public string CsvFileName { get; set; }
    }

    public class SimpleVendorDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class WithholdingDto
    {
        public string WithholdingTaxType { get; set; }
        public string WithholdingTaxCode { get; set; }
        public bool Liable { get; set; }
        public string RecipientType { get; set; }
        public string WithholdingTaxIdNo { get; set; }
        public string ExemptionCertificateNo { get; set; }
        public string ExemptionRate { get; set; }
        public string ReasonForExemption { get; set; }
        public DateTime? DateOnWhichExemptionBegins { get; set; }
        public DateTime? DateOnWhichExemptionEnds { get; set; }
    }
}
