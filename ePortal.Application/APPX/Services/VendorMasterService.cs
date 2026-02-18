using ePortal.Application.APPX.Contracts;
using ePortal.Persistence;
using ePortal.Persistence.Admin.Interface;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.ViewModels.APPX.VendorMaster;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.APPX.Services
{
    public class VendorMasterService : IVendorMasterService
    {
        private readonly IVendorMaster _repository;
        private readonly ISessionService _sessionService;
        private readonly ILogger<VendorMasterService> _logger;
        private readonly string _userId;

        public VendorMasterService(IVendorMaster repository, ISessionService sessionService, ILogger<VendorMasterService> logger)
        {
            _repository = repository;
            _sessionService = sessionService;
            _logger = logger;
            _userId = _sessionService.Get<string>("userID")?.ToString() ?? string.Empty;
        }

        public async Task<Vendor> GetCsvValidation(ParseCsvRequest request, string fullPath)
        {
            try
            {
                var lines = System.IO.File.ReadAllLines(fullPath, Encoding.UTF8);

                var vendor = new Vendor
                {
                    VENDORNAME1 = GetCellValue(lines[2]),
                    VENDORNAME2 = GetCellValue(lines[3]),
                    VENDORNAME3 = GetCellValue(lines[4]),
                    STREET1 = GetCellValue(lines[5]),
                    STREET2 = GetCellValue(lines[6]),
                    STREET3 = GetCellValue(lines[7]),
                    STREET4 = GetCellValue(lines[8]),
                    CITY = GetCellValue(lines[9]),
                    REGION = GetCellValue(lines[10]),
                    POSTALCODE = GetCellValue(lines[11]),
                    COUNTRY = GetCellValue(lines[12]),
                    MOBILENO = GetCellValue(lines[13]),
                    TELEPHONENO = GetCellValue(lines[14]),
                    EMAIL1 = GetCellValue(lines[15]),
                    EMAIL2 = GetCellValue(lines[16]),
                    EMAIL3 = GetCellValue(lines[17]),
                    MSMEINFOSTATUS = GetCellValue(lines[18]),
                    MSMECATEGORY = GetCellValue(lines[19]),
                    MSMECERTIFICATION = GetCellValue(lines[20]),
                    MSMEFROM = GetCellValue(lines[21]),
                    MSMETO = GetCellValue(lines[22]),
                    MSMECITY = GetCellValue(lines[23]),
                    TypeofIndustry = GetCellValue(lines[24]),
                    ClassificationOfYear = GetCellValue(lines[25]),
                    DateOfClassification = GetCellValue(lines[26]),
                    SERVICEAGENTGRP = GetCellValue(lines[27]),

                    BANKACCNO = GetCellValue(lines[29]),
                    BANKNAME = GetCellValue(lines[30]),
                    BANKADDRESS = GetCellValue(lines[31]),
                    BANKCITY = GetCellValue(lines[32]),
                    BANKSTATE = GetCellValue(lines[33]),
                    BANKCOUNTRY = GetCellValue(lines[34]),
                    BRANCHNAME = GetCellValue(lines[35]),
                    TYPEOFACCOUNT = GetCellValue(lines[36]),
                    IFSCCODE = GetCellValue(lines[37]),
                    SWIFTCODE = GetCellValue(lines[38]),
                    IBANNO = GetCellValue(lines[39]),
                    BANKCATEGORY = GetCellValue(lines[40]),
                    SCHEMAGROUP = GetCellValue(lines[41]),
                    ORDERCURRENCY = GetCellValue(lines[42]),
                    REFERENCE_DETAIL = GetCellValue(lines[43]),

                    I_BANK_KEY = GetCellValue(lines[45]),
                    I_BANKACCNO = GetCellValue(lines[46]),
                    I_BANKNAME = GetCellValue(lines[47]),
                    I_BANKADDRESS = GetCellValue(lines[48]),
                    I_BANKCITY = GetCellValue(lines[49]),
                    I_BANKSTATE = GetCellValue(lines[50]),
                    I_BANKCOUNTRY = GetCellValue(lines[51]),
                    I_BRANCHNAME = GetCellValue(lines[52]),
                    I_TYPEOFACCOUNT = GetCellValue(lines[53]),
                    I_SWIFTCODE = GetCellValue(lines[54]),
                    I_IBANNO = GetCellValue(lines[55]),
                    I_BANKCATEGORY = GetCellValue(lines[56]),
                    I_REFERENCE_DETAIL = GetCellValue(lines[57]),

                    PANNUMBER = GetCellValue(lines[59]),
                    GSTIN = (GetCellValue(lines[61]) ?? string.Empty).ToUpperInvariant(),
                    GSTCLASSIFICATION = GetCellValue(lines[62]),
                    E_INVOICEApplicable = GetCellValue(lines[63]),
                    LEIAPPLICABLE = GetCellValue(lines[64]),
                    LEINO = GetCellValue(lines[65])
                };

                // Post-processing rules (strings only)
                if (string.Equals(vendor.MSMEINFOSTATUS, "NO", StringComparison.OrdinalIgnoreCase))
                {
                    vendor.MSMECERTIFICATION = string.Empty;
                    vendor.MSMECATEGORY = string.Empty;
                }
                if (string.Equals(vendor.LEIAPPLICABLE, "NO", StringComparison.OrdinalIgnoreCase))
                {
                    vendor.LEINO = string.Empty;
                }

                // Sanitize names/streets
                vendor.VENDORNAME1 = (vendor.VENDORNAME1 ?? string.Empty).Replace('"', ' ').Trim();
                vendor.VENDORNAME2 = (vendor.VENDORNAME2 ?? string.Empty).Replace('"', ' ').Trim();
                vendor.VENDORNAME3 = (vendor.VENDORNAME3 ?? string.Empty).Replace('"', ' ').Trim();
                vendor.STREET1 = (vendor.STREET1 ?? string.Empty).Replace('"', ' ').Trim();
                vendor.STREET2 = (vendor.STREET2 ?? string.Empty).Replace('"', ' ').Trim();
                vendor.STREET3 = (vendor.STREET3 ?? string.Empty).Replace('"', ' ').Trim();
                vendor.STREET4 = (vendor.STREET4 ?? string.Empty).Replace('"', ' ').Trim();

                // Hidden fields
                vendor.Hidden = new HiddenFields
                {
                    HDVENDORNAME = vendor.VENDORNAME1,
                    HDVENDORADDRESS = $"{vendor.STREET1} {vendor.STREET2} {vendor.STREET3}".Trim(),
                    HDADDRESS1 = vendor.STREET1,
                    HDADDRESS2 = vendor.STREET2,
                    HDADDRESS3 = vendor.STREET3,
                    HDVENDORCITY = vendor.CITY,
                    HDVENDORACNO = vendor.BANKACCNO,
                    HDVENDORPANNO = vendor.PANNUMBER
                };

                // Map request-level fields
                vendor.REQUESTTYPE = request.RequestType ?? string.Empty;
                vendor.VENDORACCGRP = request.VendorAccGrp ?? string.Empty;
                vendor.VENDORCODE = request.VendorCode?.Trim() ?? string.Empty;

                // Call validation service (all parameters are strings)
                vendor.Result = _repository.VENDORDETAILVALIDATE(
                    vendor.REQUESTTYPE,
                    vendor.VENDORACCGRP,
                    vendor.VENDORCODE,
                    vendor.VENDORNAME1,
                    vendor.VENDORNAME2,
                    vendor.VENDORNAME3,
                    vendor.STREET1,
                    vendor.STREET2,
                    vendor.STREET3,
                    vendor.STREET4,
                    vendor.CITY,
                    vendor.REGION,
                    vendor.POSTALCODE,
                    vendor.COUNTRY,
                    vendor.MOBILENO,
                    vendor.TELEPHONENO,
                    vendor.EMAIL1,
                    vendor.EMAIL2,
                    vendor.EMAIL3,
                    vendor.MSMEINFOSTATUS,
                    vendor.MSMECATEGORY,
                    vendor.MSMECERTIFICATION,
                    vendor.SERVICEAGENTGRP,
                    vendor.BANKCOUNTRY,
                    vendor.BANKNAME,
                    vendor.BRANCHNAME,
                    vendor.BANKADDRESS,
                    vendor.TYPEOFACCOUNT,
                    vendor.BANKCITY,
                    vendor.BANKSTATE,
                    vendor.BANKACCNO,
                    vendor.IFSCCODE,
                    vendor.BANKCATEGORY,
                    vendor.SCHEMAGROUP,
                    vendor.ORDERCURRENCY,
                    vendor.PANNUMBER,
                    "", "", "", "", "", "", "", "", // placeholders to match original signature
                    vendor.GSTIN,
                    vendor.GSTCLASSIFICATION,
                    vendor.E_INVOICEApplicable,
                    vendor.MSMEFROM,
                    vendor.MSMETO,
                    vendor.MSMECITY,
                    vendor.LEIAPPLICABLE,
                    vendor.LEINO,
                    vendor.SWIFTCODE,
                    vendor.IBANNO,
                    vendor.I_BANKACCNO,
                    vendor.I_BRANCHNAME,
                    vendor.I_BANKADDRESS,
                    vendor.I_BANKCITY,
                    vendor.I_BANKSTATE,
                    vendor.I_BANKCOUNTRY,
                    vendor.I_TYPEOFACCOUNT,
                    vendor.I_BANKCATEGORY,
                    vendor.I_SWIFTCODE,
                    vendor.I_IBANNO,
                    vendor.I_BANK_KEY,
                    vendor.TypeofIndustry,
                    vendor.ClassificationOfYear,
                    vendor.DateOfClassification
                );
                return vendor;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error in Method:{MethodName}, Logged in User Id:{UserId}, Message: {ErrorMsg}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw;
            }
        }

        public async Task<DataTable> GetApprovalAuthorityTableAsync(string bankAccNo, string panNumber)
        {
            try
            {
                // Get ECODE from session
                string ecode = _sessionService.Get<string>("userID")?.ToString() ?? string.Empty;

                // VMOBJ is synchronous in your code; run on background thread to avoid blocking
                var ds = await Task.Run(() => _repository.GET_DEPARTMENTHEAD(ecode)).ConfigureAwait(false);

                DataTable sourceTable = null;
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (string.IsNullOrEmpty(bankAccNo) || string.IsNullOrEmpty(panNumber))
                    {
                        if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                            sourceTable = ds.Tables[1];
                    }
                    else
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                            sourceTable = ds.Tables[0];
                    }
                }

                if (sourceTable == null)
                {
                    sourceTable = new DataTable();
                    sourceTable.Columns.Add("EMPLOYEE", typeof(string));
                    sourceTable.Columns.Add("DEPARTMENTHEAD", typeof(string));
                }

                return sourceTable;
            }
            catch (Exception ex)
            {
                // Log error only
                _logger.LogError(ex, "Error in Method:{MethodName}, Logged in User Id:{UserId}, Message: {ErrorMsg}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw;
            }
        }

        public async Task<VendorCsvParseResult> ParseFromFileAsync(string fullPath)
        {
            try {
                var result = new VendorCsvParseResult();

                string[] lines = await File.ReadAllLinesAsync(fullPath).ConfigureAwait(false);

                if (lines == null || !(lines.Length == 63 || lines.Length == 66))
                {
                    _logger.LogWarning("Unexpected CSV line count: {Count}", lines?.Length ?? 0);
                    return result;
                }

                string GetCellValueSafe(int index)
                {
                    if (index < 0 || index >= lines.Length) return string.Empty;
                    return GetCellValue(lines[index]);
                }

                // Map fields
                result.lblvendorname = CombineNonEmpty(
                    GetCellValueSafe(2),
                    GetCellValueSafe(3),
                    GetCellValueSafe(4)
                ).ToUpperInvariant();

                result.lblstreethouseno = CombineNonEmpty(
                    GetCellValueSafe(5),
                    GetCellValueSafe(6),
                    GetCellValueSafe(7),
                    GetCellValueSafe(8)
                ).ToUpperInvariant();

                result.lblcity = GetCellValueSafe(9).ToUpperInvariant();
                result.lblregion = GetCellValueSafe(10).ToUpperInvariant();
                result.lblpostalcode = GetCellValueSafe(11);
                result.lblcountry = GetCellValueSafe(12).ToUpperInvariant();
                result.lblmobilenumber = GetCellValueSafe(13);
                result.lbltelphoneno = GetCellValueSafe(14);
                result.lblemailid1 = GetCellValueSafe(15).ToUpperInvariant();
                result.lblemailid2 = GetCellValueSafe(16).ToUpperInvariant();
                result.lblemailid3 = GetCellValueSafe(17).ToUpperInvariant();
                result.lblmsmeinfostatus = GetCellValueSafe(18).ToUpperInvariant();
                result.lblmsmecategory = GetCellValueSafe(19).ToUpperInvariant();
                result.lblmsmecertno = GetCellValueSafe(20).ToUpperInvariant();
                result.lblmsmefrom = GetCellValueSafe(21);
                result.lblmsmeto = GetCellValueSafe(22);
                result.lblmsmecity = GetCellValueSafe(23);
                result.lblNTypeOfIndustry = GetCellValueSafe(24).ToUpperInvariant();
                result.lblNClassificationOfYear = GetCellValueSafe(25).ToUpperInvariant();
                result.lblNDateOfClassification = GetCellValueSafe(26).ToUpperInvariant();
                result.lblserviceagentgrp = GetCellValueSafe(27).ToUpperInvariant();

                result.lblbankaccountno = GetCellValueSafe(29).ToUpperInvariant();
                result.lblbankname = GetCellValueSafe(30).ToUpperInvariant();
                result.lblbankaddress = GetCellValueSafe(31).ToUpperInvariant();
                result.lblbankcity = GetCellValueSafe(32).ToUpperInvariant();
                result.lblbankstate = GetCellValueSafe(33).ToUpperInvariant();
                result.lblbankcountry = GetCellValueSafe(34).ToUpperInvariant();
                result.lblbranchname = GetCellValueSafe(35).ToUpperInvariant();
                result.lbltypeofaccount = GetCellValueSafe(36).ToUpperInvariant();
                result.lblifsccode = GetCellValueSafe(37).ToUpperInvariant();
                result.lblswiftcode = GetCellValueSafe(38).ToUpperInvariant();
                result.lblibanno = GetCellValueSafe(39).ToUpperInvariant();
                result.lblbankcategory = GetCellValueSafe(40).ToUpperInvariant();
                result.lblschemagroup = GetCellValueSafe(41).ToUpperInvariant();
                result.lblordercurrency = GetCellValueSafe(42).ToUpperInvariant();
                result.lblreferencedetails = GetCellValueSafe(43).ToUpperInvariant();

                result.lblibankkey = GetCellValueSafe(45).ToUpperInvariant();
                result.lblibankaccountno = GetCellValueSafe(46).ToUpperInvariant();
                result.lblibankname = GetCellValueSafe(47).ToUpperInvariant();
                result.lblibankaddress = GetCellValueSafe(48).ToUpperInvariant();
                result.lblibankcity = GetCellValueSafe(49).ToUpperInvariant();
                result.lblibankstate = GetCellValueSafe(50).ToUpperInvariant();
                result.lblibankcountry = GetCellValueSafe(51).ToUpperInvariant();
                result.lblibranchname = GetCellValueSafe(52).ToUpperInvariant();
                result.lblitypeofaccount = GetCellValueSafe(53).ToUpperInvariant();
                result.lbliswiftcode = GetCellValueSafe(54).ToUpperInvariant();
                result.lbliibanno = GetCellValueSafe(55).ToUpperInvariant();
                result.lblibankcategory = GetCellValueSafe(56).ToUpperInvariant();
                result.lblireferenceddetails = GetCellValueSafe(57).ToUpperInvariant();

                result.lblpannumber = GetCellValueSafe(59).ToUpperInvariant();
                result.lblgstin = (GetCellValueSafe(61) ?? string.Empty).ToUpperInvariant();
                result.lblgstclassification = GetCellValueSafe(62);
                result.lblEinvoiceApplicable = GetCellValueSafe(63);
                result.lblleiapplicable = GetCellValueSafe(64).ToUpperInvariant();
                result.lblleino = GetCellValueSafe(65);

                // Clean fields
                result.lblvendorname = CleanAndTrim(result.lblvendorname);
                result.lblstreethouseno = CleanAndTrim(result.lblstreethouseno);
                result.lblbranchname = CleanAndTrim(result.lblbranchname);
                result.lblbankaddress = CleanAndTrim(result.lblbankaddress);
                result.LblVendorNameCode = CleanAndTrim(result.lblvendorname);

                // MSME business rule
                if (!string.IsNullOrEmpty(result.lblmsmeinfostatus) &&
                    result.lblmsmeinfostatus.Equals("NO", StringComparison.OrdinalIgnoreCase))
                {
                    result.lblmsmecertno = string.Empty;
                    result.lblmsmecategory = string.Empty;
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:{MethodName}, Logged in User Id:{UserId}, Message: {ErrorMsg}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw;
            }
        }

        public async Task<VendorListMatchResponse> GetVendorListMatchDetailsAsync(VendorMatchRequest request)
        {
            try
            {
                // If VMOBJ methods are synchronous, run them on a background thread to avoid blocking.
                var ods1 = await Task.Run(() => _repository.GETVENDORLISTMATCHDTL(
                    request.VendorName ?? string.Empty,
                    request.VendorAddress ?? string.Empty,
                    request.VendorCity ?? string.Empty,
                    request.VendorAcNo ?? string.Empty,
                    request.VendorPanNo ?? string.Empty,
                    request.Address1 ?? string.Empty,
                    request.Address2 ?? string.Empty,
                    request.Address3 ?? string.Empty
                )).ConfigureAwait(false);

                var ods2 = await Task.Run(() => _repository.GETVENDORLISTMATCHDTLA(
                    request.VendorName ?? string.Empty,
                    request.VendorAddress ?? string.Empty,
                    request.VendorCity ?? string.Empty,
                    request.VendorAcNo ?? string.Empty,
                    request.VendorPanNo ?? string.Empty,
                    request.Address1 ?? string.Empty,
                    request.Address2 ?? string.Empty,
                    request.Address3 ?? string.Empty
                )).ConfigureAwait(false);

                var odsExists = await Task.Run(() => _repository.GETVENDORLISTIFEXISTS(
                    request.VendorName ?? string.Empty,
                    request.VendorAddress ?? string.Empty,
                    request.VendorCity ?? string.Empty,
                    request.VendorAcNo ?? string.Empty,
                    request.VendorPanNo ?? string.Empty
                )).ConfigureAwait(false);

                var response = new VendorListMatchResponse
                {
                    ods1 = new Ods1Response
                    {
                        Table0 = DataTableToList(ods1, 0),
                        Table1 = DataTableToList(ods1, 1),
                        Table2 = DataTableToList(ods1, 2),
                        Table3 = DataTableToList(ods1, 3),
                        Table4 = DataTableToList(ods1, 4)
                    },
                    ods2 = new Ods2Response
                    {
                        Table0 = DataTableToList(ods2, 0),
                        Table1 = DataTableToList(ods2, 1)
                    },
                    odsExists = new OdsExistsResponse
                    {
                        Table0 = DataTableToList(odsExists, 0)
                    }
                };

                return response;
            }
            catch (Exception ex)
            {
                // Log error only
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw; // let controller map to HTTP response
            }
        }

        public async Task<VendorDto> GetDetailBehalfOfVendorCodeAsync(string vendorCode)
        {
            if (string.IsNullOrWhiteSpace(vendorCode))
                throw new ArgumentNullException(nameof(vendorCode));

            try
            {
                // VMOBJ appears to be synchronous; run on background thread to avoid blocking.
                var ds = await Task.Run(() => _repository.VENDORREQUESTBYVEDNORCODE(vendorCode)).ConfigureAwait(false);

                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                    return null;

                var row = ds.Tables[0].Rows[0];

                var dto = new VendorDto
                {
                    VENDORACCOUNTGRP = row["VENDORACCOUNTGRP"]?.ToString(),
                    VENDORACCGRPID = row["VENDORACCGRPID"]?.ToString(),
                    VENDORNAME = Capitalized(row["VENDORNAME"]?.ToString()),
                    VENDORCODE = row["VENDORCODE"]?.ToString()?.ToUpperInvariant(),
                    STREET = Capitalized(row["STREET"]?.ToString()),
                    CITY = Capitalized(row["CITY"]?.ToString()),
                    REGION = Capitalized(row["REGION"]?.ToString()),
                    POSTALCODE = row["POSTALCODE"]?.ToString(),
                    COUNTRY = Capitalized(row["COUNTRY"]?.ToString()),
                    MOBILENO = row["MOBILENO"]?.ToString(),
                    TELEPHONENO = row["TELEPHONENO"]?.ToString(),
                    EMAIL1 = row["EMAIL1"]?.ToString(),
                    EMAIL2 = row["EMAIL2"]?.ToString(),
                    EMAIL3 = row["EMAIL3"]?.ToString(),
                    MSMESTATUSINFO = Capitalized(row["MSMESTATUSINFO"]?.ToString()),
                    MSMECATEGORY = Capitalized(row["MSMECATEGORY"]?.ToString()),
                    MSMECERTIFICATENO = row["MSMECERTIFICATENO"]?.ToString()?.ToUpperInvariant(),
                    SERVICEAGENTGROUP = row["SERVICEAGENTGROUP"]?.ToString()?.ToUpperInvariant(),
                    MSMEFROM = row["MSMEFROM"]?.ToString(),
                    MSMETO = row["MSMETO"]?.ToString(),
                    MSMECITY = row["MSMECITY"]?.ToString(),

                    TYPE_OF_INDUSTRY = row["TYPE_OF_INDUSTRY"]?.ToString(),
                    CLASSIFICATION_OF_YEAR = row["CLASSIFICATION_OF_YEAR"]?.ToString(),
                    DATE_OF_CLASSIFICATION = row["DATE_OF_CLASSIFICATION"]?.ToString(),

                    BANKACCOUNTNO = row["BANKACCOUNTNO"]?.ToString()?.ToUpperInvariant(),
                    BANKNAME = Capitalized(row["BANKNAME"]?.ToString()),
                    BANKADDRESS = Capitalized(row["BANKADDRESS"]?.ToString()),
                    BANKCITY = Capitalized(row["BANKCITY"]?.ToString()),
                    BANKREGION = Capitalized(row["BANKREGION"]?.ToString()),
                    BANKCOUNTRY = Capitalized(row["BANKCOUNTRY"]?.ToString()),
                    BRANCHNAME = Capitalized(row["BRANCHNAME"]?.ToString()),
                    TYPEOFACCOUNT = Capitalized(row["TYPEOFACCOUNT"]?.ToString()),
                    IFSCCODE = row["IFSCCODE"]?.ToString()?.ToUpperInvariant(),
                    BANKCATEGORY = Capitalized(row["BANKCATEGORY"]?.ToString()),
                    SCHEMAGROUP = Capitalized(row["SCHEMAGROUP"]?.ToString()),
                    ORDERCURRENCY = Capitalized(row["ORDERCURRENCY"]?.ToString()),

                    REFERENCE_DETAIL = Capitalized(row["REFERENCE_DETAIL"]?.ToString()),
                    SWIFT_CODE = Capitalized(row["SWIFT_CODE"]?.ToString()),
                    IBAN_NO = Capitalized(row["IBAN_NO"]?.ToString()),

                    IBANK_KEY = row["IBANK_KEY"]?.ToString()?.ToUpperInvariant(),
                    IBANK_ACC_NO = row["IBANK_ACC_NO"]?.ToString()?.ToUpperInvariant(),
                    IBANK_NAME = row["IBANK_NAME"]?.ToString()?.ToUpperInvariant(),
                    IADDRESS = row["IADDRESS"]?.ToString()?.ToUpperInvariant(),
                    ICITY = row["ICITY"]?.ToString()?.ToUpperInvariant(),
                    IREGION_STATE = row["IREGION_STATE"]?.ToString()?.ToUpperInvariant(),
                    ICOUNTRY = row["ICOUNTRY"]?.ToString()?.ToUpperInvariant(),
                    IBRANCH_NAME = row["IBRANCH_NAME"]?.ToString()?.ToUpperInvariant(),
                    ITYPE_OF_ACCOUNT = row["ITYPE_OF_ACCOUNT"]?.ToString()?.ToUpperInvariant(),
                    ISWIFT_CODE = row["ISWIFT_CODE"]?.ToString()?.ToUpperInvariant(),
                    IBANK_CATEGORY = row["IBANK_CATEGORY"]?.ToString()?.ToUpperInvariant(),
                    IREFERENCE_DETAIL = row["IREFERENCE_DETAIL"]?.ToString()?.ToUpperInvariant(),

                    PANNO = row["PANNO"]?.ToString()?.ToUpperInvariant(),

                    GSTIN = row["GSTIN"]?.ToString()?.ToUpperInvariant(),
                    GSTCLASSIFICATION = row["GSTCLASSIFICATION"]?.ToString()?.ToUpperInvariant(),
                    EINVOICE = row["EINVOICE"]?.ToString()?.ToUpperInvariant(),

                    LEI_APPLICABLE = row["LEI_APPLICABLE"]?.ToString()?.ToUpperInvariant(),
                    LEINO = row["LEINO"]?.ToString()
                };

                return dto;
            }
            catch (Exception ex)
            {
                // log error only
                _logger.LogError(ex, "Error in Method:{MethodName}, Logged in User Id:{UserId}, Message: {ErrorMsg}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw;
            }
        }

        public VendorMasterViewModel GetWithholdingData()
        {
            string ECODE = _sessionService.Get<string>("userID")?.ToString() ?? string.Empty;
            string Error_msg = string.Empty;
            DataSet DS = _repository.GET_WHOLDINGTAXMASTER(ECODE, Error_msg);
            DataTable dtTypeList = DS.Tables[0].Select("COMPONENTTYPE=5").CopyToDataTable();
            DataTable dtCodeList = DS.Tables[0].Select("COMPONENTTYPE=6").CopyToDataTable();

            var model = new VendorMasterViewModel
            {
                WithHoldingTaxTypeList = dtTypeList.AsSelectList("COMPONENTKI", "COMPONENT", true, "Select Tax Type", ""),
                WithHoldingTaxCodeList = dtCodeList.AsSelectList("COMPONENTKI", "COMPONENT", true, "Select Tax Code", "")
            };

            return model;
        }

        public async Task<VendorRequestResult> GetVendorRequests(VendorMasterFilter obj)
        {
            try
            {
                string ecode = _sessionService.Get<string>("userID")?.ToString() ?? "";
                // Call your existing method
                DataSet ds = _repository.GETVENDORPENDINGREQUEST(
                    ecode, obj.status, obj.vendorHeaderId, obj.requestType, obj.vendorAccGroup, obj.vendorName, obj.reqDateFrom, obj.reqDateTo);

                var result = new VendorRequestResult
                {
                    PendingRequests = new List<VendorRequest>(),
                    WithholdingTaxes = new List<VendorWithholdingTax>()
                };

                // Map CUR_PENDREQ (first table)
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var req = new VendorRequest
                        {
                            EncryptedVendorHeaderId = WebUtility.UrlEncode(Encryption.Encrypt(row["VENDORHEADERID"].ToString())),
                            VendorHeaderId = row["VENDORHEADERID"].ToString(),
                            RequestType = Convert.ToInt32(row["REQUESTTYPE"]),
                            RequestTypeDesc = row["REQUESTTYPEDESC"].ToString(),
                            Syki = row["SYKI"].ToString(),
                            VendorAccountGroupId = row["VENDORACCOUNTGPID"].ToString(),
                            VendorAccountGroup = row["VENDORACCOUNTGRP"].ToString(),
                            ProcessStatus = Convert.ToInt32(row["PROCESSSTATUS"]),
                            RequestorId = row["REQUESTORID"].ToString(),
                            RequestorName = row["REQUESTORNAME"].ToString(),
                            RequestDate = row["REQUESTDATE"].ToString(),
                            StatusDescription = row["STATUSDESCRIPTION"].ToString(),

                            VendorCode = row["VENDORCODE"].ToString(),
                            VendorName1 = row["VENDORNAME1"].ToString(),
                            VendorName2 = row["VENDORNAME2"].ToString(),
                            VendorName3 = row["VENDORNAME3"].ToString(),
                            VendorName = row["VENDORNAME"].ToString(),
                            RegionId = row["REGIONID"].ToString(),
                            City = row["CITY"].ToString(),
                            Region = row["REGION"].ToString(),
                            CountryId = row["COUNTRYID"].ToString(),
                            Country = row["COUNTRY"].ToString(),
                            PostalCode = row["POSTALCODE"].ToString(),
                            MobileNo = row["MOBILENO"].ToString(),
                            TelephoneNo = row["TELEPHONENO"].ToString(),
                            Email1 = row["EMAIL1"].ToString(),
                            Email2 = row["EMAIL2"].ToString(),
                            Email3 = row["EMAIL3"].ToString(),

                            BankAccountNo = row["BANKACCOUNTNO"].ToString(),
                            BankName = row["BANKNAME"].ToString(),
                            BankAddress = row["BANKADDRESS"].ToString(),
                            BankCity = row["BANKCITY"].ToString(),
                            BankRegion = row["BANKREGION"].ToString(),
                            BankRegionDesc = row["BANKREGION"].ToString(),
                            BankCountryId = row["BANKCOUNTRYID"].ToString(),
                            BankCountry = row["BANKCOUNTRY"].ToString(),
                            BranchName = row["BRANCHNAME"].ToString(),
                            AccountTypeDescription = row["ACTYPEDESCRIPTION"].ToString(),
                            IfscCode = row["IFSCCODE"].ToString(),
                            BankCategory = row["BANKCATEGORY"].ToString(),
                            SchemaGroup = row["SCHEMAGROUP"].ToString(),
                            OrderCurrency = row["ORDERCURRENCY"].ToString(),
                            SwiftCode = row["SWIFT_CODE"].ToString(),
                            IbanNo = row["IBAN_NO"].ToString(),

                            PanNo = row["PANNO"].ToString(),
                            CstRegNo = row["CSTREGNO"].ToString(),
                            LstNo = row["LSTNO"].ToString(),
                            ServiceRegNo = row["SERVICEREGNNO"].ToString(),
                            EccNo = row["ECCNO"].ToString(),
                            ExciseRegNo = row["EXCISEREGNNO"].ToString(),
                            ExciseRange = row["EXCISERANGE"].ToString(),
                            ExciseDivision = row["EXCISEDIVISION"].ToString(),
                            Commissionerate = row["COMMISTIONERATE"].ToString(),

                            CsvFile = row["CSVFILE"].ToString(),
                            RegistrationCertAttach = row["REGISTRATIONCERTATTACH"].ToString(),
                            MandateAttachment = row["MANDATEATTACHMENT"].ToString(),
                            CcAttachment = row["CCATTACHMENT"].ToString(),
                            PanAttachment = row["PANATTECHMENT"].ToString(),
                            ServiceAttachment = row["SERVICEATTACHMENT"].ToString(),
                            CstCertAttachment = row["CSTCERTATTACHMENT"].ToString(),
                            ExciseAttachment = row["EXCISEATTACHMENT"].ToString(),
                            MsmeCertAttachment = row["MSMECERTATTACHMENT"].ToString()
                        };

                        result.PendingRequests.Add(req);
                    }
                }

                // Map CUR_WTHTAX (second table)
                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        var tax = new VendorWithholdingTax
                        {
                            VendorHeaderId = row["VENDORHEADERID"].ToString(),
                            WithholdingTaxType = row["WITHHOLDING_TAX_TYPE"].ToString(),
                            WithholdingTaxCode = row["WITHHOLDING_TAX_CODE"].ToString(),
                            Liable = row["LIABLE"].ToString(),
                            RecipientType = row["RECIPIENT_TYPE"].ToString(),
                            WithholdingTaxIdNo = row["WITHHOLDING_TAX_ID_NO"].ToString(),
                            ExemptionCertificateNo = row["EXEMPTION_CERTI_NO"].ToString(),
                            ExemptionRate = row["EXEMPTION_RATE"] == DBNull.Value ? null : Convert.ToDecimal(row["EXEMPTION_RATE"]),
                            ExemptionBegins = row["EXEMPTION_BEGINS"].ToString(),
                            ExemptionEnds = row["EXEMPTION_ENDS"].ToString(),
                            ReasonForExemption = row["REASON_FOR_EXEMPTION"].ToString()
                        };

                        result.WithholdingTaxes.Add(tax);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:{MethodName}, Logged in User Id:{UserId}, Message: {ErrorMsg}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw;
            }
        }

        public async Task<List<VendorApprovalRequest>> GetVendorApprovalRequests(VendorMasterFilter obj)
        {
            try
            {
                string ecode = _sessionService.Get<string>("userID")?.ToString() ?? "";

                // Call repository method that executes SPROC_VENDORAPPROVALREQ_GET
                DataSet ds = _repository.GETVENDORAPPROVALGREQUEST(
                    ecode, obj.status, obj.requestType, obj.vendorAccGroup, obj.vendorName);

                var result = new List<VendorApprovalRequest>();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var req = new VendorApprovalRequest
                        {
                            EncryptedVendorHeaderId = WebUtility.UrlEncode(Encryption.Encrypt(row["VENDORHEADERID"].ToString())),
                            VENDORHEADERID = Convert.ToInt32(row["VENDORHEADERID"]),
                            EMPLOYEE = row["EMPLOYEE"].ToString(),
                            REQUESTTYPEDESC = row["REQUESTTYPEDESC"].ToString(),
                            VENDORACCOUNTGRP = row["VENDORACCOUNTGRP"].ToString(),
                            REQUESTDATE = row["REQUESTDATE"].ToString(),
                            VENDORNAME = row["VENDORNAME"].ToString(),
                            HISTORYSTATUS = row["HISTORYSTATUS"].ToString(),
                            PROCESSSTATUS = row["PROCESSSTATUS"].ToString(),
                            PSTATUSID = Convert.ToInt32(row["PSTATUSID"])
                        };

                        result.Add(req);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:{MethodName}, Logged in User Id:{UserId}, Message: {ErrorMsg}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw;
            }
        }

        public async Task<List<VendorApprovalRequest>> GetVendorApprovalRequestsFin(VendorMasterFilter obj)
        {
            try
            {
                string ecode = _sessionService.Get<string>("userID")?.ToString() ?? "";

                // Call repository method that executes SPROC_VENDORAPPROVALREQ_GET
                DataSet ds = _repository.GETVENDORAPPROVALGREQFIN(
                    ecode, obj.status, obj.requestType, obj.vendorAccGroup, obj.vendorName);

                var result = new List<VendorApprovalRequest>();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var req = new VendorApprovalRequest
                        {
                            EncryptedVendorHeaderId = WebUtility.UrlEncode(Encryption.Encrypt(row["VENDORHEADERID"].ToString())),
                            VENDORHEADERID = Convert.ToInt32(row["VENDORHEADERID"].ToString()),
                            EMPLOYEE = row["EMPLOYEE"].ToString(),
                            REQUESTTYPEDESC = row["REQUESTTYPEDESC"].ToString(),
                            VENDORACCOUNTGRP = row["VENDORACCOUNTGRP"].ToString(),
                            REQUESTDATE = row["REQUESTDATE"].ToString(),
                            VENDORNAME = row["VENDORNAME"].ToString(),
                            HISTORYSTATUS = row["HISTORYSTATUS"].ToString(),
                            PROCESSSTATUS = row["PROCESSSTATUS"].ToString(),
                            PSTATUSID = Convert.ToInt32(row["PSTATUSID"])
                        };

                        result.Add(req);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:{MethodName}, Logged in User Id:{UserId}, Message: {ErrorMsg}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw;
            }
        }

        public async Task<VendorDetailsViewModel> GetVendorRequestDataById(string VMID, string basePath)
        {
            try
            {
                string vendorHeaderId = WebUtility.UrlDecode(Encryption.Decrypt(VMID));
                DataSet ds = _repository.VENDORREQUESTBYID(vendorHeaderId);

                VendorDetailsViewModel model = new VendorDetailsViewModel();
                model.EncryptedVendorHeaderId = VMID;

                if (ds.Tables.Count > 3 && ds.Tables[2].Rows.Count > 0)
                {
                    var rowOldVendorDetails = ds.Tables[2].Rows[0];
                    model.OldVendorDetails = new OldVendorDetailsViewModel
                    {
                        VendorAccountGroup = rowOldVendorDetails["VENDORACCOUNTGRP"].ToString(),
                        VendorName = rowOldVendorDetails["VENDORNAME"].ToString(),
                        VendorCode = rowOldVendorDetails["VENDORCODE"].ToString(),
                        StreetHouseNo = rowOldVendorDetails["STREET"].ToString(),
                        City = rowOldVendorDetails["CITY"].ToString(),
                        Region = rowOldVendorDetails["REGION"].ToString(),
                        PostalCode = rowOldVendorDetails["POSTALCODE"].ToString(),
                        Country = rowOldVendorDetails["COUNTRY"].ToString(),
                        MobileNumber = rowOldVendorDetails["MOBILENO"].ToString(),
                        TelephoneNumber = rowOldVendorDetails["TELEPHONENO"].ToString(),
                        Email1 = rowOldVendorDetails["EMAIL1"].ToString(),
                        Email2 = rowOldVendorDetails["EMAIL2"].ToString(),
                        Email3 = rowOldVendorDetails["EMAIL3"].ToString(),
                        MSMEStatusInfo = rowOldVendorDetails["MSMESTATUSINFO"].ToString(),
                        MSMECategory = rowOldVendorDetails["MSMECATEGORY"].ToString(),
                        MSMECertificateNo = rowOldVendorDetails["MSMECERTIFICATENO"].ToString(),
                        ServiceAgentGroup = rowOldVendorDetails["SERVICEAGENTGROUP"].ToString(),
                        MSMEFrom = rowOldVendorDetails["MSMEFROM"].ToString(),
                        MSMETo = rowOldVendorDetails["MSMETO"].ToString(),
                        MSMECity = rowOldVendorDetails["MSMECITY"].ToString(),
                        TypeOfIndustry = rowOldVendorDetails["TYPE_OF_INDUSTRY"].ToString(),
                        ClassificationOfYear = rowOldVendorDetails["CLASSIFICATION_OF_YEAR"].ToString(),
                        DateOfClassification = rowOldVendorDetails["DATE_OF_CLASSIFICATION"].ToString(),
                        BankAccountNo = rowOldVendorDetails["BANKACCOUNTNO"].ToString(),
                        BankName = rowOldVendorDetails["BANKNAME"].ToString(),
                        BankAddress = rowOldVendorDetails["BANKADDRESS"].ToString(),
                        BankCity = rowOldVendorDetails["BANKCITY"].ToString(),
                        BankState = rowOldVendorDetails["BANKREgion"].ToString(),
                        BankCountry = rowOldVendorDetails["BANKCOUNTRY"].ToString(),
                        BranchName = rowOldVendorDetails["BRANCHNAME"].ToString(),
                        TypeOfAccount = rowOldVendorDetails["TYPEOFACCOUNT"].ToString(),
                        IFSCCode = rowOldVendorDetails["IFSCCODE"].ToString(),
                        BankCategory = rowOldVendorDetails["BANKCATEGORY"].ToString(),
                        SchemaGroup = rowOldVendorDetails["SCHEMAGROUP"].ToString(),
                        OrderCurrency = rowOldVendorDetails["ORDERCURRENCY"].ToString(),
                        SwiftCode = rowOldVendorDetails["SWIFT_CODE"].ToString(),
                        IBANNo = rowOldVendorDetails["IBAN_NO"].ToString(),
                        ReferenceDetail = rowOldVendorDetails["REFERENCE_DETAIL"].ToString(),
                        PANNumber = rowOldVendorDetails["PANNO"].ToString(),
                        GSTIN = rowOldVendorDetails["GSTIN"].ToString().ToUpper(),
                        GSTClassification = rowOldVendorDetails["GSTCLASSIFICATION"].ToString().ToUpper(),
                        EInvoice = rowOldVendorDetails["EINVOICE"].ToString().ToUpper(),
                        LEIApplicable = rowOldVendorDetails["LEI_APPLICABLE"].ToString().ToUpper(),
                        LEINo = rowOldVendorDetails["LEINO"].ToString()
                    };
                }

                if (ds.Tables.Count > 3 && ds.Tables[0].Rows.Count > 0)
                {
                    var rowVendorRequest = ds.Tables[0].Rows[0];
                    //var basePath = filePath;

                    model.VendorRequest = new VendorRequestViewModel
                    {
                        ShowTrSP = false,
                        ShowTrFin = true,
                        // Requestor & meta
                        SpecialApproverId = rowVendorRequest["SPLAPPSTATUS"].ToString(),
                        RequestorName = rowVendorRequest["REQUESTORNAME"].ToString(),
                        RequestDate = rowVendorRequest["REQUESTDATE"].ToString(),
                        RequestorEmail = rowVendorRequest["EMAILID"].ToString(),
                        RequestorMobile = rowVendorRequest["TMOBILE"].ToString(),
                        Site = rowVendorRequest["SITE"].ToString(),
                        Operation = rowVendorRequest["OPERATION"].ToString(),
                        Division = rowVendorRequest["DIVISION"].ToString(),
                        Department = rowVendorRequest["DEPARTMENT"].ToString(),
                        Section = rowVendorRequest["SECTION"].ToString(),
                        RequestType = rowVendorRequest["REQUESTTYPEDESC"].ToString(),

                        // Vendor identifiers
                        VendorHeaderId = rowVendorRequest["VENDORHEADERID"].ToString(),
                        VendorAccountGroup = rowVendorRequest["VENDORACCOUNTGRP"].ToString(),
                        VendorCode = rowVendorRequest["VENDORCODE"].ToString(),
                        InsertUpdateVendorCode = rowVendorRequest["INST_UPDTVCODE"].ToString(),

                        // Vendor address & contact
                        VendorName = rowVendorRequest["VENDORNAME"].ToString().Trim(),
                        Street = rowVendorRequest["STREET"].ToString().Trim(),
                        City = rowVendorRequest["CITY"].ToString().Trim(),
                        Region = rowVendorRequest["REGION"].ToString().Trim(),
                        PostalCode = rowVendorRequest["POSTALCODE"].ToString().Trim(),
                        Country = rowVendorRequest["COUNTRY"].ToString().Trim(),
                        MobileNumber = rowVendorRequest["MOBILENO"].ToString().Trim(),
                        TelephoneNumber = rowVendorRequest["TELEPHONENO"].ToString().Trim(),
                        Email1 = rowVendorRequest["EMAIL1"].ToString().Trim(),
                        Email2 = rowVendorRequest["EMAIL2"].ToString().Trim(),
                        Email3 = rowVendorRequest["EMAIL3"].ToString().Trim(),

                        // MSME & industry
                        MsmeStatusInfo = rowVendorRequest["MSMESTATUSINFO"].ToString().Trim(),
                        MsmeCategory = rowVendorRequest["MSMECATEGORY"].ToString().Trim(),
                        MsmeCertificateNumber = rowVendorRequest["MSMECERTIFICATENO"].ToString().Trim(),
                        ServiceAgentGroup = rowVendorRequest["SERVICEAGENTGROUP"].ToString().Trim(),
                        TypeOfIndustry = rowVendorRequest["TYPE_OF_INDUSTRY"].ToString().Trim(),
                        ClassificationOfYear = rowVendorRequest["CLASSIFICATION_OF_YEAR"].ToString().Trim(),
                        DateOfClassification = rowVendorRequest["DATE_OF_CLASSIFICATION"].ToString().Trim(),

                        // Bank details
                        BankAccountNo = rowVendorRequest["BANKACCOUNTNO"].ToString().Trim(),
                        BankName = rowVendorRequest["BANKNAME"].ToString().Trim(),
                        BankAddress = rowVendorRequest["BANKADDRESS"].ToString().Trim(),
                        BankCity = rowVendorRequest["BANKCITY"].ToString().Trim(),
                        BankState = rowVendorRequest["BANKSTATE"].ToString().Trim(),
                        BankCountry = rowVendorRequest["BANKCOUNTRY"].ToString().Trim(),
                        BranchName = rowVendorRequest["BRANCHNAME"].ToString().Trim(),
                        AccountTypeDescription = rowVendorRequest["ACTYPEDESCRIPTION"].ToString().Trim(),
                        IfscCode = rowVendorRequest["IFSCCODE"].ToString().Trim(),
                        BankCategory = rowVendorRequest["BANKCATEGORY"].ToString().Trim(),
                        SchemaGroup = rowVendorRequest["SCHEMAGROUP"].ToString().Trim(),
                        OrderCurrency = rowVendorRequest["ORDERCURRENCY"].ToString().Trim(),
                        SwiftCode = rowVendorRequest["SWIFT_CODE"].ToString().Trim(),
                        IbanNo = rowVendorRequest["IBAN_NO"].ToString().Trim(),
                        ReferenceDetail = rowVendorRequest["REFERENCE_DETAIL"].ToString().Trim(),

                        // Tax details
                        PanNo = rowVendorRequest["PANNO"].ToString().Trim(),
                        Gstin = rowVendorRequest["GSTIN"].ToString().Trim(),
                        GstClassification = rowVendorRequest["GSTCLASSIFICATION"].ToString().Trim(),
                        Remarks = rowVendorRequest["REMARKS"].ToString().Trim(),
                        MsmeFrom = rowVendorRequest["MSMEFROM"].ToString().Trim(),
                        MsmeTo = rowVendorRequest["MSMETO"].ToString().Trim(),
                        MsmeCity = rowVendorRequest["MSMECITY"].ToString().Trim(),
                        LeiApplicable = rowVendorRequest["LEI_APPLICABLE"].ToString().Trim(),
                        LeiNo = rowVendorRequest["LEINO"].ToString().Trim(),

                        // Attachments (build URLs only if present)
                        RegistrationCertificateUrl = MakeUrl(basePath, rowVendorRequest["REGISTRATIONCERTATTACH"]),
                        ConflictCertificateUrl = MakeUrl(basePath, rowVendorRequest["CONFLICTCER_ATTACH"]),
                        MandateFormUrl = MakeUrl(basePath, rowVendorRequest["MANDATEATTACHMENT"]),
                        CancelChequeUrl = MakeUrl(basePath, rowVendorRequest["CCATTACHMENT"]),
                        PanCardUrl = MakeUrl(basePath, rowVendorRequest["PANATTECHMENT"]),
                        MsmeCertificateUrl = MakeUrl(basePath, rowVendorRequest["MSMECERTATTACHMENT"]),
                        GstRegistrationUrl = MakeUrl(basePath, rowVendorRequest["GSTCERTATTACHMENT"]),
                        EinvoiceUrl = MakeUrl(basePath, rowVendorRequest["EINVOICE_ATTACHMENT"]),
                        LeiAttachmentUrl = MakeUrl(basePath, rowVendorRequest["LEI_ATTACHMENT"]),

                        // after you set the URL properties (MakeUrl(...) results)
                        RegCertVerVisible = !string.IsNullOrWhiteSpace(rowVendorRequest["REGISTRATIONCERTATTACH"].ToString()),
                        ConfCertVerVisible = !string.IsNullOrWhiteSpace(rowVendorRequest["CONFLICTCER_ATTACH"].ToString()),
                        MandateVerVisible = !string.IsNullOrWhiteSpace(rowVendorRequest["MANDATEATTACHMENT"].ToString()),
                        CancelChqVerVisible = !string.IsNullOrWhiteSpace(rowVendorRequest["CCATTACHMENT"].ToString()),
                        PanVerVisible = !string.IsNullOrWhiteSpace(rowVendorRequest["PANATTECHMENT"].ToString()),
                        MsmeVerVisible = !string.IsNullOrWhiteSpace(rowVendorRequest["MSMECERTATTACHMENT"].ToString()),
                        GstVerVisible = !string.IsNullOrWhiteSpace(rowVendorRequest["GSTCERTATTACHMENT"].ToString()),
                        EinvoiceVerVisible = !string.IsNullOrWhiteSpace(rowVendorRequest["EINVOICE_ATTACHMENT"].ToString()),
                        LeiVerVisible = !string.IsNullOrWhiteSpace(rowVendorRequest["LEI_ATTACHMENT"].ToString()),

                        // UI flags
                        ShowGeneralInfo = string.Equals(rowVendorRequest["REQUESTTYPEDESC"].ToString(), "UPDATE VENDOR", StringComparison.OrdinalIgnoreCase),
                        ShowBankDetails = string.Equals(rowVendorRequest["REQUESTTYPEDESC"].ToString(), "UPDATE VENDOR", StringComparison.OrdinalIgnoreCase),
                        ShowTaxDetails = string.Equals(rowVendorRequest["REQUESTTYPEDESC"].ToString(), "UPDATE VENDOR", StringComparison.OrdinalIgnoreCase),
                        ShowVendorCodeRow = !string.IsNullOrWhiteSpace(rowVendorRequest["VENDORCODE"].ToString()),
                        ShowVendorCodeBanner = !string.IsNullOrWhiteSpace(rowVendorRequest["INST_UPDTVCODE"].ToString()),

                        // E-invoice applicable flag
                        IsEinvoiceApplicable = rowVendorRequest["EINVOICE_APPLICABLE"].ToString() == "1" ? "Yes" : "No",
                    };

                    // If request type is "UPDATE VENDOR" -> show no-match placeholders (same behavior as aspx.cs)
                    if (!string.IsNullOrEmpty(model.VendorRequest.RequestType) && model.VendorRequest.RequestType.ToUpper() == "UPDATE VENDOR")
                    {
                        model.VendorMatch = new VendorMatchViewModel
                        {
                            // Clear all match lists so view will render "No data found" blocks
                            VendorNamesPartial = new(),
                            VendorNamesExact = new(),
                            VendorAddressesPartial = new(),
                            VendorAddressesExact = new(),
                            VendorAccountsExact = new(),
                            VendorPANsExact = new(),
                            EmployeeAddresses = new(),

                            // Ensure checkbox/match area hidden
                            ShowCheckMatch = false,

                            // Keep default visibility for approval rows (you can set ShowTrFin/ShowTrSP as needed)
                        };
                        model.VendorRequest.ShowTrFin = true;
                        model.VendorRequest.ShowTrSP = false;
                    }
                    else
                    {
                        string vendorName = rowVendorRequest["VENDORNAME"].ToString().Trim();
                        string streetHouseNo = rowVendorRequest["STREET"].ToString().Trim();
                        string city = rowVendorRequest["CITY"].ToString().Trim();
                        string bankAccountNo = rowVendorRequest["BANKACCOUNTNO"].ToString().Trim();
                        string panNumber = rowVendorRequest["PANNO"].ToString().Trim();
                        string address1 = rowVendorRequest["STREET1"].ToString().Trim();
                        string address2 = rowVendorRequest["STREET2"].ToString().Trim();
                        string address3 = rowVendorRequest["STREET3"].ToString().Trim();

                        model.VendorMatch = new VendorMatchViewModel();

                        // Otherwise run the match logic (partial + actual/exact)
                        DataSet ods1 = _repository.GETVENDORLISTMATCHDTL(
                            vendorName ?? string.Empty,
                            streetHouseNo ?? string.Empty,
                            city ?? string.Empty,
                            bankAccountNo ?? string.Empty,
                            panNumber ?? string.Empty,
                            address1 ?? string.Empty,
                            address2 ?? string.Empty,
                            address3 ?? string.Empty
                        );
                        // Actual/exact matches
                        DataSet ods2 = _repository.GETVENDORLISTMATCHDTLA(
                            vendorName ?? string.Empty,
                            streetHouseNo ?? string.Empty,
                            city ?? string.Empty,
                            bankAccountNo ?? string.Empty,
                            panNumber ?? string.Empty,
                            address1 ?? string.Empty,
                            address2 ?? string.Empty,
                            address3 ?? string.Empty
                        );

                        model.VendorMatch.ShowCheckMatch = model.VendorMatch.EmployeeAddresses != null && model.VendorMatch.EmployeeAddresses.Any();

                        if (ods1 != null && ods1.Tables.Count >= 5)
                        {
                            model.VendorMatch.VendorNamesPartial = ods1.Tables[0].Rows.Count > 0 ? ToVendorDtoList(ods1.Tables[0]) : new();
                            model.VendorMatch.VendorAddressesPartial = ods1.Tables[1].Rows.Count > 0 ? ToVendorAddressDtoList(ods1.Tables[1]) : new();
                            model.VendorMatch.VendorAccountsExact = ods1.Tables[2].Rows.Count > 0 ? ToVendorAccountDtoList(ods1.Tables[2]) : new();
                            model.VendorMatch.VendorPANsExact = ods1.Tables[3].Rows.Count > 0 ? ToVendorPanDtoList(ods1.Tables[3]) : new();
                            model.VendorMatch.EmployeeAddresses = ods1.Tables[4].Rows.Count > 0 ? ToEmployeeAddressDtoList(ods1.Tables[4]) : new();
                        }

                        int splApproval = 0;
                        if (ods2 != null && ods2.Tables.Count >= 2)
                        {
                            model.VendorMatch.VendorNamesExact = ods2.Tables[0].Rows.Count > 0 ? ToVendorDtoList(ods2.Tables[0]) : new();
                            if (ods2.Tables[0].Rows.Count > 0) splApproval = 1;

                            model.VendorMatch.VendorAddressesExact = ods2.Tables[1].Rows.Count > 0 ? ToVendorAddressDtoList(ods2.Tables[1]) : new();
                            if (ods2.Tables[1].Rows.Count > 0) splApproval = 1;
                        }

                        // Show checkbox only when SPLAPPROVAL == 1 and there are employee addresses (matches original logic)
                        model.VendorMatch.ShowCheckMatch = splApproval == 1;

                        // Default approval row visibility (adjust if needed)
                        model.VendorRequest.ShowTrFin = true;
                        model.VendorRequest.ShowTrSP = false;
                    }
                }

                if (ds.Tables.Count > 3 && ds.Tables[3].Rows.Count > 0)
                {
                    var irow = ds.Tables[3].Rows[0];
                    model.IntermediaryBank = new IntermediaryBankViewModel
                    {
                        BankAccountNo = irow["BANK_ACC_NO"].ToString(),
                        BankName = irow["BANK_NAME"].ToString(),
                        Address = irow["ADDRESS"].ToString(),
                        City = irow["CITY"].ToString(),
                        BankState = irow["BANKSTATE"].ToString(),
                        BankCountry = irow["BANKCOUNTRY"].ToString(),
                        BranchName = irow["BRANCH_NAME"].ToString(),
                        AccountTypeDescription = irow["ACTYPEDESCRIPTION"].ToString(),
                        BankCategory = irow["BANKCATEGORY"].ToString(),
                        SwiftCode = irow["SWIFT_CODE"].ToString(),
                        IbanNo = irow["IBAN_NO"].ToString(),
                        BankKey = irow["BANK_KEY"].ToString(),
                        ReferenceDetail = irow["REFERENCE_DETAIL"].ToString()
                    };
                    model.ShowIntermediaryBank = true;
                }
                else
                {
                    model.ShowIntermediaryBank = false;
                }

                // Withholding data mapping (Table[4])
                if (ds.Tables.Count > 4 && ds.Tables[4].Rows.Count > 0)
                {
                    var wt = ds.Tables[4];
                    foreach (DataRow r in wt.Rows)
                    {
                        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        foreach (DataColumn c in wt.Columns)
                        {
                            dict[c.ColumnName] = r[c]?.ToString();
                        }
                        model.WithholdingData.Add(dict);
                    }
                    model.ShowWithholding = true;
                    model.ShowWithholdingDiv = true;
                }
                else
                {
                    model.ShowWithholding = false;
                    model.ShowWithholdingDiv = false;
                }

                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    var ph = ds.Tables[1];
                    foreach (DataRow r in ph.Rows)
                    {
                        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        foreach (DataColumn c in ph.Columns)
                        {
                            dict[c.ColumnName] = r[c]?.ToString();
                        }
                        model.ProcessHistoryData.Add(dict);
                    }
                    model.ShowProcessHistory = true;
                }
                else
                {
                    model.ShowProcessHistory = false;
                }
                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:{MethodName}, Logged in User Id:{UserId}, Message: {ErrorMsg}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw;
            }
        }

        public async Task<List<VendorBlockPendingRequest>> GetVendorBlockPendingRequests(VendorBlockFilter filter)
        {
            try
            {
                // Get current user id (ecode)
                string ecode = _sessionService.Get<string>("userID")?.ToString() ?? "";

                // Call repository
                DataSet ds = _repository.GETVENDORBLOCKPENDINGREQUEST(
                    ecode,
                    filter.Status ?? "",
                    filter.VendorHeaderId ?? "",
                    filter.RequestType ?? "",
                    filter.RequestCategory ?? "",
                    filter.VendorCode ?? "",
                    filter.ReqDateFrom ?? "",
                    filter.ReqDateTo ?? ""
                );

                var requests = new List<VendorBlockPendingRequest>();

                // Map first table to VendorBlockRequest list
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var req = new VendorBlockPendingRequest
                        {
                            EncVendorHeaderId = WebUtility.UrlEncode(Encryption.Encrypt(row["VENDORBLOCKHEADERID"]?.ToString())),
                            VENDORBLOCKHEADERID = row["VENDORBLOCKHEADERID"]?.ToString(),
                            REQUESTTYPE = row["REQUESTTYPE"]?.ToString(),
                            REQUESTTYPEDESC = row["REQUESTTYPEDESC"]?.ToString(),
                            SYKI = row["SYKI"]?.ToString(),
                            PROCESSSTATUS = Convert.ToInt32(row["PROCESSSTATUS"]),
                            REQUESTCATEGORYID = row["REQUESTCATEGORYID"]?.ToString(),
                            REQUESTCATE = row["REQUESTCATE"]?.ToString(),
                            REQUESTORID = row["REQUESTORID"]?.ToString(),
                            REQUESTORNAME = row["REQUESTORNAME"]?.ToString(),
                            REQUESTDATE = row["REQUESTDATE"]?.ToString(),
                            STATUSDESCRIPTION = row["STATUSDESCRIPTION"]?.ToString()
                        };

                        requests.Add(req);
                    }
                }

                return requests;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:{MethodName}, Logged in User Id:{UserId}, Message: {ErrorMsg}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw;
            }
        }

        public async Task<List<VendorBlockApprovalRequest>> GetVendorBlockApprovalRequests(VendorBlockFilter filter)
        {
            try
            {
                // Get current user id (ecode)
                string ecode = _sessionService.Get<string>("userID")?.ToString() ?? "";

                // Call repository
                DataSet ds = _repository.GETVENDORBLOCAPPROVALGREQUEST(
                    ecode,
                    filter.Status ?? "",
                    filter.RequestType ?? "",
                    filter.RequestCategory ?? "",
                    filter.VendorCode ?? ""
                );

                var requests = new List<VendorBlockApprovalRequest>();

                // Map first table to VendorBlockRequest list
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var req = new VendorBlockApprovalRequest
                        {
                            EncryptedVendorHeaderId = WebUtility.UrlEncode(Encryption.Encrypt(row["VENDORBLOCKHEADERID"]?.ToString())),
                            VENDORBLOCKHEADERID = row["VENDORBLOCKHEADERID"]?.ToString(),

                            VENDORBLOCKAPPROVALHISID = row["VENDORBLOCKAPPROVALHISID"]?.ToString(),
                            EMPLOYEE = row["EMPLOYEE"]?.ToString(),
                            REQUESTTYPEDESC = row["REQUESTTYPEDESC"]?.ToString(),
                            REQUESTCATEDESC = row["REQUESTCATEDESC"]?.ToString(),
                            REQUESTDATE = row["REQUESTDATE"]?.ToString(),
                            HISTORYSTATUS = row["HISTORYSTATUS"]?.ToString(),
                            PROCESSSTATUS = row["PROCESSSTATUS"].ToString(),
                            PSTATUSID = Convert.ToInt32(row["PSTATUSID"]),
                        };

                        requests.Add(req);
                    }
                }

                return requests;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:{MethodName}, Logged in User Id:{UserId}, Message: {ErrorMsg}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                throw;
            }
        }

        //public async Task<VendorBlockRequestViewModel> GetVendorBlockRequestDataById(string VMID)
        //{
        //    string vendorHeaderId = Encryption.Decrypt(WebUtility.UrlDecode(VMID));
        //    DataSet ds = _repository.VENDORBLOCKREQUESTBYID(vendorHeaderId);

        //    VendorBlockRequestViewModel result = new VendorBlockRequestViewModel();

        //    if (ds == null || ds.Tables.Count == 0)
        //        return result;

        //    // local helper to safely get string values
        //    string GetValue(DataRow row, string columnName)
        //    {
        //        if (row == null) return string.Empty;
        //        if (!row.Table.Columns.Contains(columnName)) return string.Empty;
        //        var val = row[columnName];
        //        return val == DBNull.Value || val == null ? string.Empty : val.ToString();
        //    }

        //    // Table 0 -> Header (single row expected)
        //    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //    {
        //        var r = ds.Tables[0].Rows[0];
        //        result.HeaderRequest = new VendorBlockHeaderRequest
        //        {
        //            AdEmpCode = GetValue(r, "ADEMPCODE"),
        //            RequestorName = GetValue(r, "REQUESTORNAME"),
        //            RequestDate = GetValue(r, "REQUESTDATE"),
        //            Mobile = GetValue(r, "TMOBILE"),
        //            Site = GetValue(r, "SITE"),
        //            Operation = GetValue(r, "OPERATION"),
        //            Division = GetValue(r, "DIVISION"),
        //            Department = GetValue(r, "DEPARTMENT"),
        //            Section = GetValue(r, "SECTION"),
        //            EmailId = GetValue(r, "EMAILID"),

        //            VendorBlockHeaderId = GetValue(r, "VENDORBLOCKHEADERID"),
        //            RequestType = GetValue(r, "REQUESTTYPE"),
        //            RequestTypeDesc = GetValue(r, "REQUESTTYPEDESC"),
        //            Syki = GetValue(r, "SYKI"),
        //            RequestCategory = GetValue(r, "REQUESTCATEGORY"),
        //            RequestCategoryDesc = GetValue(r, "REQUESTCATEDESC"),

        //            BlockPurchasing = GetValue(r, "BLOCKPURCHASING"),
        //            PurchasingStatus = GetValue(r, "PURCHASINGSTATUS"),
        //            BlockPosting = GetValue(r, "BLOCKPOSTING"),
        //            PostingStatus = GetValue(r, "POSTINGSTATUS"),

        //            ProcessStatus = GetValue(r, "PROCESSSTATUS"),
        //            StatusDescription = GetValue(r, "STATUSDESCRIPTION"),

        //            RequestorId = GetValue(r, "REQUESTORID"),
        //            Remarks = GetValue(r, "REMARKS")
        //        };
        //    }

        //    // Table 1 -> Process history (multiple rows)
        //    if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
        //    {
        //        foreach (DataRow row in ds.Tables[1].Rows)
        //        {
        //            var hist = new VendorProcessHistory
        //            {
        //                ApprovalDate = GetValue(row, "APPDATE"),
        //                Employee = GetValue(row, "EMPLOYEE"),
        //                StatusDescription = GetValue(row, "STATUSDESCRIPTION"),
        //                Remarks = GetValue(row, "REMARKS"),
        //                ApprovalDescription = GetValue(row, "APPDESC")
        //            };
        //            result.ProcessHistory.Add(hist);
        //        }
        //    }

        //    // Table 2 -> Vendor details (multiple rows)
        //    if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
        //    {
        //        foreach (DataRow row in ds.Tables[2].Rows)
        //        {
        //            var vd = new VendorDetailRequest
        //            {
        //                VendorCode = GetValue(row, "VENDORCODE"),
        //                VendorName = GetValue(row, "VENDORNAME"),
        //                City = GetValue(row, "CITY"),
        //                Region = GetValue(row, "REGION")
        //            };
        //            result.VendorDetails.Add(vd);
        //        }
        //    }

        //    return result;
        //}

        private static List<VendorDtlDto> ToVendorDtoList(DataTable table)
        {
            return table.AsEnumerable()
                        .Select(r => new VendorDtlDto
                        {
                            VendorCode = r.Field<string>("VENDORCODE") ?? string.Empty,
                            VendorName = r.Field<string>("VENDORNAME") ?? string.Empty
                        })
                        .ToList();
        }

        private static List<VendorAddressDto> ToVendorAddressDtoList(DataTable table)
        {
            return table.AsEnumerable()
                        .Select(r => new VendorAddressDto
                        {
                            VendorCode = r.Field<string>("VENDORCODE") ?? string.Empty,
                            VendorName = r.Field<string>("VENDORNAME") ?? string.Empty,
                            Address = r.Table.Columns.Contains("ADDRESS") ? r.Field<string>("ADDRESS") ?? string.Empty : string.Empty
                        })
                        .ToList();
        }

        private static List<VendorAccountDto> ToVendorAccountDtoList(DataTable table)
        {
            return table.AsEnumerable()
                        .Select(r => new VendorAccountDto
                        {
                            VendorCode = r.Field<string>("VENDORCODE") ?? string.Empty,
                            VendorName = r.Field<string>("VENDORNAME") ?? string.Empty,
                            AccountNo = r.Table.Columns.Contains("ACCOUNTNO") ? r.Field<string>("ACCOUNTNO") ?? string.Empty
                                      : (r.Table.Columns.Contains("ADDRESS") ? r.Field<string>("ADDRESS") ?? string.Empty : string.Empty)
                        })
                        .ToList();
        }

        private static List<VendorPanDto> ToVendorPanDtoList(DataTable table)
        {
            return table.AsEnumerable()
                        .Select(r => new VendorPanDto
                        {
                            VendorCode = r.Field<string>("VENDORCODE") ?? string.Empty,
                            VendorName = r.Field<string>("VENDORNAME") ?? string.Empty,
                            PanNo = r.Table.Columns.Contains("PANNO") ? r.Field<string>("PANNO") ?? string.Empty
                                  : (r.Table.Columns.Contains("ADDRESS") ? r.Field<string>("ADDRESS") ?? string.Empty : string.Empty)
                        })
                        .ToList();
        }

        private static List<EmployeeAddressDto> ToEmployeeAddressDtoList(DataTable table)
        {
            return table.AsEnumerable()
                        .Select(r => new EmployeeAddressDto
                        {
                            EmployeeCode = r.Table.Columns.Contains("VENDORCODE") ?Convert.ToString(r.Field<long>("VENDORCODE")) ?? string.Empty : string.Empty,
                            EmployeeName = r.Table.Columns.Contains("VENDORNAME") ? r.Field<string>("VENDORNAME") ?? string.Empty : string.Empty,
                            Address = r.Table.Columns.Contains("ADDRESS") ? r.Field<string>("ADDRESS") ?? string.Empty : string.Empty
                        })
                        .ToList();
        }

        #region Helpers
        private string Capitalized(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }
        private static string GetCellValue(string line)
        {
            if (string.IsNullOrEmpty(line)) return string.Empty;
            int idx = line.IndexOf(',');
            if (idx < 0) return string.Empty;
            return line.Substring(idx + 1).Trim().Trim('"');
        }
        private static string CombineNonEmpty(params string[] parts)
        {
            return string.Join(" ", parts.Where(p => !string.IsNullOrWhiteSpace(p))).Trim();
        }
        private static string CleanAndTrim(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            var cleaned = input.Replace("\"", "").Trim();
            cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"\s+", " ");
            return cleaned;
        }
        private static string MakeUrl(string basePath, object value)
        {
            var file = value?.ToString().Trim();
            if (string.IsNullOrEmpty(file)) return null;
            return Path.Combine(basePath, file);// basePath.TrimEnd('/') + "/" + file;
        }
        private static IEnumerable<IDictionary<string, object>> DataTableToList(DataSet ds, int tableIndex)
        {
            if (ds == null || ds.Tables.Count <= tableIndex) return Enumerable.Empty<IDictionary<string, object>>();

            var table = ds.Tables[tableIndex];
            var rows = new List<IDictionary<string, object>>(table.Rows.Count);

            foreach (DataRow dr in table.Rows)
            {
                var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                foreach (DataColumn col in table.Columns)
                {
                    var val = dr[col];
                    dict[col.ColumnName] = val == DBNull.Value ? null : val;
                }
                rows.Add(dict);
            }

            return rows;
        }
        #endregion
    }
}
