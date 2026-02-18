using ePortal.Persistence.Interface;
using ePortal.ViewModels.APPX.CustomerMgmt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.Data;
//using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
//using System.Text;
using System.Text.Json;

namespace ePortal.Persistence.Services
{
    public class EportalESS : IEportalESS
    {
        #region "Local Variables"
        DataSet ds = new DataSet();

        private readonly IDataManagement oDataMgmt;
        private readonly IConnectionString objCnStr;
        private readonly IConfiguration _configuration;

        //OracleCommand oCmd;
        //DataSet ds;
        //DataTable Dt;
        //OracleConnection objConn;
        //string strConn;
        //string strErrMsg = string.Empty;
        #endregion

        public EportalESS(IDataManagement _oDataMgmt, IConnectionString _objCnStr, IConfiguration iConfiguration)
        {
            oDataMgmt = _oDataMgmt;
            objCnStr = _objCnStr;
            _configuration = iConfiguration;
        }
        public async Task<DataTable> GetAssetsDetails(string fncode, string profitCenter)
        {
            var payload = new { fncode, profitCenter };
            string responseBody = await SendPostRequestAsync("GetAssetsDetails", payload);

            if (string.IsNullOrEmpty(responseBody))
            {
                return new DataTable(); // Return an empty DataTable instead of failing
            }
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jsonData["message"].ToString()) ?? new DataTable();
        }
        public async Task<DataTable> GetCustomerDetails()
        {
            string responseBody = await SendPostRequestAsync("GetCustomerDetails", null);

            if (string.IsNullOrEmpty(responseBody))
            {
                return new DataTable(); // Return an empty DataTable instead of failing
            }
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jsonData["message"].ToString()) ?? new DataTable();
        }

        public async Task<DataTable> GetAssetCodeData(string AssetCode)
        {
            var payload = new { AssetCode };
            string responseBody = await SendPostRequestAsync("GetAssetCodeData", payload);

            if (string.IsNullOrEmpty(responseBody))
            {
                return new DataTable(); // Return an empty DataTable instead of failing
            }
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jsonData["message"].ToString()) ?? new DataTable();
        }
        public async Task<DataTable> GetAucCodeData(string AucCode)
        {
            var payload = new { AucCode };
            string responseBody = await SendPostRequestAsync("GetAucCodeData", payload);

            if (string.IsNullOrEmpty(responseBody))
            {
                return new DataTable(); // Return an empty DataTable instead of failing
            }
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jsonData["message"].ToString()) ?? new DataTable();
        }
        public async Task<DataTable> GetFamilylist(string userid, string subtype)
        {
            DataTable dt = new DataTable();
            var payload = new { userid, subtype };
            string responseBody = await SendPostRequestAsync("GetFamilylist", payload);

            if (string.IsNullOrEmpty(responseBody))
            {
                return new DataTable(); // Return an empty DataTable instead of failing
            }
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jsonData["message"].ToString()) ?? new DataTable();
            //RfcConfigParameters parms = new RfcConfigParameters();
            //parms = sapconnection();
            //RfcDestination rfcDest = RfcDestinationManager.GetDestination(parms);
            //RfcRepository rfcRep = rfcDest.Repository;
            //IRfcFunction function = rfcRep.CreateFunction("BAPI_FAMILY_GETDETAILEDLIST");
            //function.SetValue("EMPLOYEENUMBER", userid);
            //function.SetValue("SUBTYPE", "");
            //function.SetValue("TIMEINTERVALLOW", "18000101");
            //function.SetValue("TIMEINTERVALHIGH", "99991231");
            //RfcSessionManager.BeginContext(rfcDest);
            //function.Invoke(rfcDest);
            //IRfcTable tblReturn = function.GetTable("FAMILY");

            //for (int liElement = 0; liElement < tblReturn.ElementCount; liElement++)
            //{
            //    RfcElementMetadata metadata = tblReturn.GetElementMetadata(liElement);
            //    dt.Columns.Add(metadata.Name);
            //}

            //foreach (IRfcStructure row in tblReturn)
            //{
            //    DataRow sapdr = dt.NewRow();
            //    for (int liElement = 0; liElement < tblReturn.ElementCount; liElement++)
            //    {
            //        RfcElementMetadata metadata = tblReturn.GetElementMetadata(liElement);
            //        sapdr[metadata.Name] = row.GetString(metadata.Name);
            //    }
            //    if (sapdr[4].ToString() == "9999-12-31")
            //    {
            //        dt.Rows.Add(sapdr);
            //    }
            //}
            //RfcSessionManager.EndContext(rfcDest);
            //return dt;
        }

        //public DataTable GetAssetCodeData(string AssetCode)
        //{
        //try
        //{
        //RfcConfigParameters parms = new RfcConfigParameters();
        //parms = sapconnection();
        //RfcDestination rfcDest = RfcDestinationManager.GetDestination(parms);
        //RfcRepository rfcRep = rfcDest.Repository;
        //IRfcFunction function = rfcRep.CreateFunction("ZFI_CAP_ASSET_RFC");

        //function.SetValue("ASSET", AssetCode);
        //RfcSessionManager.BeginContext(rfcDest);
        //function.Invoke(rfcDest);
        //IRfcTable tblReturn = function.GetTable("GT_ASSET_DETAILS");
        //DataTable dt = new DataTable();
        //for (int liElement = 0; liElement < tblReturn.ElementCount; liElement++)
        //{
        //    RfcElementMetadata metadata = tblReturn.GetElementMetadata(liElement);
        //    dt.Columns.Add(metadata.Name);
        //}

        //foreach (IRfcStructure row in tblReturn)
        //{
        //    DataRow sapdr = dt.NewRow();
        //    for (int liElement = 0; liElement < tblReturn.ElementCount; liElement++)
        //    {
        //        RfcElementMetadata metadata = tblReturn.GetElementMetadata(liElement);
        //        sapdr[metadata.Name] = row.GetString(metadata.Name);
        //    }
        //    dt.Rows.Add(sapdr);
        //}
        //RfcSessionManager.EndContext(rfcDest);
        //        return dt;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public DataTable GetAucCodeData(string AucCode)
        //{
        //    try
        //    {
        //    RfcConfigParameters parms = new RfcConfigParameters();
        //    parms = sapconnection();
        //    RfcDestination rfcDest = RfcDestinationManager.GetDestination(parms);
        //    RfcRepository rfcRep = rfcDest.Repository;
        //    IRfcFunction function = rfcRep.CreateFunction("ZRFC_NONCAP_ASSET");

        //    function.SetValue("ASSET", AucCode);
        //    RfcSessionManager.BeginContext(rfcDest);
        //    function.Invoke(rfcDest);
        //    IRfcTable tblReturn = function.GetTable("GT_NONCAP_FINAL");
        //DataTable dt = new DataTable();
        //    for (int liElement = 0; liElement < tblReturn.ElementCount; liElement++)
        //    {
        //        RfcElementMetadata metadata = tblReturn.GetElementMetadata(liElement);
        //        dt.Columns.Add(metadata.Name);
        //    }

        //    foreach (IRfcStructure row in tblReturn)
        //    {
        //        DataRow sapdr = dt.NewRow();
        //        for (int liElement = 0; liElement < tblReturn.ElementCount; liElement++)
        //        {
        //            RfcElementMetadata metadata = tblReturn.GetElementMetadata(liElement);
        //            sapdr[metadata.Name] = row.GetString(metadata.Name);
        //        }
        //        dt.Rows.Add(sapdr);
        //    }
        //    RfcSessionManager.EndContext(rfcDest);
        //    return dt;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public DataTable GetAssetsDetails(string fncode, string profitCenter)
        //{
        //    //RfcConfigParameters parms = new RfcConfigParameters();
        //    //parms = sapconnection();
        //    //RfcDestination rfcDest = RfcDestinationManager.GetDestination(parms);
        //    //RfcRepository rfcRep = rfcDest.Repository;
        //    //IRfcFunction function = rfcRep.CreateFunction("ZPORTAL_ASSET_DETAILS_OUTBOUND");
        //    //function.SetValue("FUNCTIONAL_AREA", fncode);
        //    //function.SetValue("PROFIT_CENTER", profitCenter);
        //    //RfcSessionManager.BeginContext(rfcDest);
        //    //function.Invoke(rfcDest);
        //    //IRfcTable tblReturn = function.GetTable("GT_ASSET_DETAILS");
        //    DataTable dt = new DataTable();
        //    //for (int liElement = 0; liElement < tblReturn.ElementCount; liElement++)
        //    //{
        //    //    RfcElementMetadata metadata = tblReturn.GetElementMetadata(liElement);
        //    //    dt.Columns.Add(metadata.Name);
        //    //}

        //    //foreach (IRfcStructure row in tblReturn)
        //    //{
        //    //    DataRow sapdr = dt.NewRow();
        //    //    for (int liElement = 0; liElement < tblReturn.ElementCount; liElement++)
        //    //    {
        //    //        RfcElementMetadata metadata = tblReturn.GetElementMetadata(liElement);
        //    //        sapdr[metadata.Name] = row.GetString(metadata.Name);
        //    //    }
        //    //    dt.Rows.Add(sapdr);
        //    //}
        //    //RfcSessionManager.EndContext(rfcDest);



        //    return dt;
        //}

        private async Task<string> SendPostRequestAsync(string endpoint, object payload)
        {
            try
            {
                // Configure HttpClientHandler for SSL validation
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, certChain, sslPolicyErrors) =>
                    {
                        // Allow if there are no SSL policy errors
                        if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
                        {
                            return true;
                        }

                        // Optionally allow name mismatch errors in non-production environments
                        return sslPolicyErrors == System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch;
                    }
                };

                using var httpClient = new HttpClient(handler);

                string baseURL = _configuration["SAP_RFC_CONFIG:URL"];
                string username = _configuration["SAP_RFC_CONFIG:UserName"];
                string passkey = _configuration["SAP_RFC_CONFIG:Passkey"];

                string secretKey = ComputeMD5Hash(passkey);
                string jsonPayload = payload != null ? JsonSerializer.Serialize(payload) : string.Empty;
                var token = GenerateToken(username, jsonPayload, secretKey);

                var requestBody = new { token };
                string jsonString = JsonSerializer.Serialize(requestBody);

                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(baseURL + endpoint, content);
                string responseBody = null;
                //response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    responseBody = await response.Content.ReadAsStringAsync();
                }
                return responseBody;
            }
            catch (Exception ex)
            {
                // Log exception properly (use Serilog/NLog)
                Console.WriteLine($"Error{endpoint}: {ex.Message}");
                throw;
            }
        }

        public static string ComputeMD5Hash(string plainText)
        {
            using (MD5 md5 = MD5.Create())
            {
                // Convert the plain text into a byte array
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

                // Compute the hash as a byte array
                byte[] hashBytes = md5.ComputeHash(plainTextBytes);

                // Convert byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
        private string GenerateToken(string username, string payload, string secrectkey)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secrectkey));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                //var claims = new[]
                //    {
                //    new Claim(JwtRegisteredClaimNames.Sub, username),
                //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //    new Claim("Payload",payload)
                //};

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                if (!string.IsNullOrEmpty(payload))
                {
                    claims.Add(new Claim("Payload", payload));
                }

                var token = new JwtSecurityToken(issuer: _configuration["SAP_RFC_CONFIG:Issuer"],
                                                  audience: _configuration["SAP_RFC_CONFIG:Audience"],
                                                  claims: claims,
                                                  expires: DateTime.Now.AddDays(7),
                                                  signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //Local Conveyance
        public async Task<DataTable> GetOverstayReport(string userid, string strdate, string strcheck)
        {
            //RfcConfigParameters parms = new RfcConfigParameters();
            //parms = sapconnection();
            //RfcDestination rfcDest = RfcDestinationManager.GetDestination(parms);
            //RfcRepository rfcRep = rfcDest.Repository;
            //IRfcFunction function = rfcRep.CreateFunction("ZHR_BAPI_OVERSTAY_RPT");
            //function.SetValue("IN_PERNR", userid);
            //function.SetValue("IN_BEGDA", strdate);
            //function.SetValue("IN_CHECK", strcheck);
            //RfcSessionManager.BeginContext(rfcDest);
            //function.Invoke(rfcDest);
            //IRfcTable tblReturn = function.GetTable("EMP_OVERSTAY");
            //DataTable dt = new DataTable();
            //for (int liElement = 0; liElement < tblReturn.ElementCount; liElement++)
            //{
            //    RfcElementMetadata metadata = tblReturn.GetElementMetadata(liElement);
            //    dt.Columns.Add(metadata.Name);
            //}

            //foreach (IRfcStructure row in tblReturn)
            //{
            //    DataRow sapdr = dt.NewRow();
            //    for (int liElement = 0; liElement < tblReturn.ElementCount; liElement++)
            //    {
            //        RfcElementMetadata metadata = tblReturn.GetElementMetadata(liElement);
            //        sapdr[metadata.Name] = row.GetString(metadata.Name);
            //    }
            //    dt.Rows.Add(sapdr);
            //}
            //RfcSessionManager.EndContext(rfcDest);
            //return dt;


            var payload = new { userid, strdate, strcheck };
            string responseBody = await SendPostRequestAsync("GetOverstayReport", payload);

            if (string.IsNullOrEmpty(responseBody))
            {
                return new DataTable(); // Return an empty DataTable instead of failing
            }
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jsonData["message"].ToString()) ?? new DataTable();

        }

        public async Task<string> Local_Conveyance_PostDataToSAP(string EMP_ID, string DOCDATE, string REF_DOC_NO, string POSTINGDATE, string ConvAmount, string MealAmout, string FromDate, string Todate)
        {
            //string SuccessMsg = string.Empty;

            //RfcConfigParameters parms = new RfcConfigParameters();
            //parms = sapconnection();
            //RfcDestination rfcDest = RfcDestinationManager.GetDestination(parms);
            //RfcRepository rfcRep = rfcDest.Repository;

            //IRfcFunction function = rfcRep.CreateFunction("ZFI_LOCAL_CONVEYANCE_POST");

            //function.SetValue("EMP_ID", EMP_ID);
            //function.SetValue("DOC_DATE", DOCDATE);
            //function.SetValue("PSTNG_DATE", POSTINGDATE);
            //function.SetValue("REF_DOC_NO", REF_DOC_NO);

            //IRfcTable TravelTable = function.GetTable("IT_TRAVEL");
            //IRfcTable RefreshTable = function.GetTable("IT_REFRESH");

            //IRfcStructure TravelData = rfcRep.GetStructureMetadata("ZFI_TRAVEL").CreateStructure();
            //TravelData.SetValue("COSTCENTER", "");
            //TravelData.SetValue("TRAVEL_AMT", ConvAmount); //travelReimbursement
            //TravelData.SetValue("MIS_AMT", "0"); // tollTaxMiscAmount
            //TravelData.SetValue("SGTXT", "Travel Reimbursement ");
            //TravelData.SetValue("ASSIG", FromDate); //  fromdate  01-04-24   
            //TravelTable.Append(TravelData);

            //IRfcStructure RefreshData = rfcRep.GetStructureMetadata("ZFI_REFRESH").CreateStructure();
            //var Todate_ = "to" + Todate;

            //RefreshData.SetValue("COSTCENTER", "");
            //RefreshData.SetValue("REF_AMT", MealAmout); // refreshment
            //RefreshData.SetValue("FOOD_AMT", "0"); //FoodAamt
            //RefreshData.SetValue("SGTXT", " Refreshment Food Expenses");
            //RefreshData.SetValue("ASSIG", Todate_); // todate  To  01-04-24
            //RefreshTable.Append(RefreshData);

            //try
            //{

            //    RfcSessionManager.BeginContext(rfcDest);
            //    function.Invoke(rfcDest);

            //    var REMARKS = function.GetValue("REMARKS");
            //    RfcSessionManager.EndContext(rfcDest);

            //    return SuccessMsg = "S#" + REMARKS.ToString();

            //}
            //catch (Exception ex)
            //{
            //    RfcSessionManager.EndContext(rfcDest);

            //    return SuccessMsg = "E#" + ex.InnerException.ToString();
            //}

            string message = string.Empty;
            var payload = new { EMP_ID, DOCDATE, REF_DOC_NO, POSTINGDATE, ConvAmount, MealAmout, FromDate, Todate };
            string responseBody = await SendPostRequestAsync("GetLocal_Conveyance_PostDataToSAP", payload);

            if (string.IsNullOrEmpty(responseBody))
            {
                return ""; // Return an empty DataTable instead of failing
            }
            //var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);
            //return Newtonsoft.Json.JsonConvert.DeserializeObject<string>(jsonData["message"].ToString()) ?? string.Empty;



            // Parse the JSON string into a JObject
            JObject jsonObject = JObject.Parse(responseBody);

            // Extract the "message" field safely
            message = jsonObject["message"]?.ToString() ?? string.Empty;

            return message;


            //return responseBody;
        }
        public async Task<string> PostInvoiceInSAP(List<ePortal.ViewModels.NSP_PaymentDetails> DataList, DateTime? DOCDATE_)
        {
            string message = string.Empty;
            string _docDate = Convert.ToDateTime(DOCDATE_).ToString("dd.MM.yyyy");
            var payload = new { DataList , DOCDATE_ =_docDate };
            string responseBody = await SendPostRequestAsync("PostInvoiceInSAP", payload);

            if (string.IsNullOrEmpty(responseBody))
            {
                return "";
            }
            JObject jsonObject = JObject.Parse(responseBody);
            message = jsonObject["message"]?.ToString() ?? string.Empty;

            return message;
        }
        public async Task<string> ReversePostedInvoiceInSAP(string DocNo, string FiscalYear, string Reason, string PostingDate)
        {
            string message = string.Empty;
            //string _docDate = Convert.ToString(DOCDATE_) ?? "";
            var payload = new { DocNo, FiscalYear , Reason , PostingDate };
            string responseBody = await SendPostRequestAsync("ReversePostedInvoiceInSAP", payload);

            if (string.IsNullOrEmpty(responseBody))
            {
                return "";
            }
            JObject jsonObject = JObject.Parse(responseBody);
            message = jsonObject["message"]?.ToString() ?? string.Empty;

            return message;
        }
        public async Task<DataSet> GetPOGetails(string POnumber)
        {
            DataSet dt = new DataSet();
            var payload = new { POnumber };
            string responseBody = await SendPostRequestAsync("GetPOGetails", payload);

            if (string.IsNullOrEmpty(responseBody))
            {
                return new DataSet(); // Return an empty DataTable instead of failing
            }
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet>(jsonData["message"].ToString()) ?? new DataTable();
            //RfcConfigParameters parms = new RfcConfigParameters();
            //parms = sapconnection();
            //RfcDestination rfcDest = RfcDestinationManager.GetDestination(parms);
            //RfcRepository rfcRep = rfcDest.Repository;
            //IRfcFunction function = rfcRep.CreateFunction("ZPORTAL_POPR_DETAILS_OUTBOUND");
            //function.SetValue("PO_NUMBER", POnumber);
            //RfcSessionManager.BeginContext(rfcDest);
            //function.Invoke(rfcDest);
            //IRfcTable tblReturn = function.GetTable("GT_PO_HEADER_DETAILS");
            //IRfcTable tblReturn1 = function.GetTable("GT_PO_ITEM_DETAILS");

            //DataTable dt = new DataTable();
            //for (int liElement = 0; liElement < tblReturn.ElementCount; liElement++)
            //{
            //    RfcElementMetadata metadata = tblReturn.GetElementMetadata(liElement);
            //    dt.Columns.Add(metadata.Name);
            //}

            //foreach (IRfcStructure row in tblReturn)
            //{
            //    DataRow sapdr = dt.NewRow();
            //    for (int liElement = 0; liElement < tblReturn.ElementCount; liElement++)
            //    {
            //        RfcElementMetadata metadata = tblReturn.GetElementMetadata(liElement);
            //        sapdr[metadata.Name] = row.GetString(metadata.Name);
            //    }
            //    dt.Rows.Add(sapdr);
            //}
            //DataTable dt1 = new DataTable();
            //for (int liElement = 0; liElement < tblReturn1.ElementCount; liElement++)
            //{
            //    RfcElementMetadata metadata = tblReturn1.GetElementMetadata(liElement);
            //    dt1.Columns.Add(metadata.Name);
            //}

            //foreach (IRfcStructure row in tblReturn1)
            //{
            //    DataRow sapdr = dt1.NewRow();
            //    for (int liElement = 0; liElement < tblReturn1.ElementCount; liElement++)
            //    {
            //        RfcElementMetadata metadata = tblReturn1.GetElementMetadata(liElement);
            //        sapdr[metadata.Name] = row.GetString(metadata.Name);
            //    }
            //    dt1.Rows.Add(sapdr);
            //}
            //RfcSessionManager.EndContext(rfcDest);

            //DataSet ds = new DataSet();
            //ds.Tables.Add(dt);
            //ds.Tables.Add(dt1);

            //return ds;
        }
        //Start --******* ESS
        public async Task<DataTable> GetEmployeeACStatement(string userid, string subtype, string monthfrom, string monthto, string finyear)
        {
            //    RfcConfigParameters parms = new RfcConfigParameters();
            //    parms = sapconnection();
            //    RfcDestination rfcDest = RfcDestinationManager.GetDestination(parms);
            //    RfcRepository rfcRep = rfcDest.Repository;
            //    IRfcFunction function = rfcRep.CreateFunction("ZFII_EMP_ACC_STATEMENT");
            //    function.SetValue("I_EMPLOYEE", userid);
            //    function.SetValue("I_CATEGORY", subtype);
            //    function.SetValue("I_FISCAL_PERIOD_FROM", monthfrom);
            //    function.SetValue("I_FISCAL_PERIOD_TO", monthto);
            //    function.SetValue("I_FISCAL_YEAR", finyear);
            //    RfcSessionManager.BeginContext(rfcDest);
            //    function.Invoke(rfcDest);
            //    IRfcTable tblReturn = function.GetTable("IT_EMP_ACC_STA");
            //    DataTable dt = new DataTable();
            //    for (int liElement = 0; liElement < tblReturn.ElementCount; liElement++)
            //    {
            //        RfcElementMetadata metadata = tblReturn.GetElementMetadata(liElement);
            //        dt.Columns.Add(metadata.Name);
            //    }

            //    foreach (IRfcStructure row in tblReturn)
            //    {
            //        DataRow sapdr = dt.NewRow();
            //        for (int liElement = 0; liElement < tblReturn.ElementCount; liElement++)
            //        {
            //            RfcElementMetadata metadata = tblReturn.GetElementMetadata(liElement);
            //            sapdr[metadata.Name] = row.GetString(metadata.Name);
            //        }
            //        dt.Rows.Add(sapdr);
            //    }
            //    RfcSessionManager.EndContext(rfcDest);
            //    return dtmonthfrom
            var payload = new { userid, subtype, monthfrom, monthto, finyear };
            string responseBody = await SendPostRequestAsync("GetEmployeeACStatement", payload);

            if (string.IsNullOrEmpty(responseBody))
            {
                return new DataTable(); // Return an empty DataTable instead of failing
            }
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jsonData["message"].ToString()) ?? new DataTable();
        }
        public async Task<DataTable> GetPayrollResultList(string userid, string fromdate, string todate)
        //public DataTable GetPayrollResultList(string userid, string fromdate, string todate)
        {

            var payload = new { userid, fromdate, todate };
            string responseBody = await SendPostRequestAsync("GetPayrollResultList", payload);

            if (string.IsNullOrEmpty(responseBody))
            {
                return new DataTable(); // Return an empty DataTable instead of failing
            }
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jsonData["message"].ToString()) ?? new DataTable();

            //RfcConfigParameters parms = new RfcConfigParameters();
            //parms = sapconnection();
            //RfcDestination rfcDest = RfcDestinationManager.GetDestination(parms);
            //RfcRepository rfcRep = rfcDest.Repository;
            //IRfcFunction function = rfcRep.CreateFunction("BAPI_GET_PAYROLL_RESULT_LIST");
            //function.SetValue("EMPLOYEENUMBER", userid);
            //function.SetValue("FROMDATE", fromdate);
            //function.SetValue("TODATE", todate);
            //RfcSessionManager.BeginContext(rfcDest);
            //function.Invoke(rfcDest);
            //IRfcTable tblReturn = function.GetTable("RESULTS");
            //DataTable dt = new DataTable();
            //for (int liElement = 0; liElement < tblReturn.ElementCount; liElement++)
            //{
            //    RfcElementMetadata metadata = tblReturn.GetElementMetadata(liElement);
            //    dt.Columns.Add(metadata.Name);
            //}

            //foreach (IRfcStructure row in tblReturn)
            //{
            //    DataRow sapdr = dt.NewRow();
            //    for (int liElement = 0; liElement < tblReturn.ElementCount; liElement++)
            //    {
            //        RfcElementMetadata metadata = tblReturn.GetElementMetadata(liElement);
            //        sapdr[metadata.Name] = row.GetString(metadata.Name);
            //    }
            //    dt.Rows.Add(sapdr);
            //}
            //RfcSessionManager.EndContext(rfcDest);
            //return dt;
        }


        //Below Added by aumento for HLI Subsidy==========================================================================
        public async Task<DataTable> GetHLISStatement(string CompCode, string EmpCode, string FinTranNo, string PostingPeriod)
        {
            var payload = new { CompCode, EmpCode, FinTranNo, PostingPeriod };
            string responseBody = await SendPostRequestAsync("GetDestination", payload);

            if (string.IsNullOrEmpty(responseBody))
            {
                return new DataTable(); // Return an empty DataTable instead of failing
            }
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jsonData["message"].ToString()) ?? new DataTable();

            //RfcConfigParameters parms = new RfcConfigParameters();
            //parms = sapconnection();
            //RfcDestination rfcDest = RfcDestinationManager.GetDestination(parms);
            //RfcRepository rfcRep = rfcDest.Repository;
            //IRfcFunction function = rfcRep.CreateFunction("ZRFC_EMP_HOMELOAN");
            //function.SetValue("S_BUKRS", CompCode);
            //function.SetValue("S_PART", EmpCode);
            //function.SetValue("S_RFHA", FinTranNo);
            //function.SetValue("S_DVALUT", PostingPeriod);
            //RfcSessionManager.BeginContext(rfcDest);
            //function.Invoke(rfcDest);
            //IRfcTable tblReturn = function.GetTable("GT_EMP_LOAN");
            //IRfcTable tblReturn1 = function.GetTable("RETURN");
            //DataTable dt = new DataTable();
            //for (int liElement = 0; liElement < tblReturn.ElementCount; liElement++)
            //{
            //    RfcElementMetadata metadata = tblReturn.GetElementMetadata(liElement);
            //    dt.Columns.Add(metadata.Name);
            //}

            //for (int liElement = 0; liElement < tblReturn1.ElementCount; liElement++)
            //{
            //    RfcElementMetadata metadata = tblReturn1.GetElementMetadata(liElement);
            //    dt.Columns.Add(metadata.Name);
            //}

            //foreach (IRfcStructure row in tblReturn)
            //{
            //    DataRow sapdr = dt.NewRow();
            //    for (int liElement = 0; liElement < tblReturn.ElementCount; liElement++)
            //    {
            //        RfcElementMetadata metadata = tblReturn.GetElementMetadata(liElement);
            //        sapdr[metadata.Name] = row.GetString(metadata.Name);
            //    }
            //    dt.Rows.Add(sapdr);
            //}


            //foreach (IRfcStructure row in tblReturn1)
            //{
            //    DataRow sapdr = dt.NewRow();
            //    for (int liElement = 0; liElement < tblReturn1.ElementCount; liElement++)
            //    {
            //        RfcElementMetadata metadata = tblReturn1.GetElementMetadata(liElement);
            //        sapdr[metadata.Name] = row.GetString(metadata.Name);
            //    }
            //    dt.Rows.Add(sapdr);
            //}
            //RfcSessionManager.EndContext(rfcDest);
            //return dt;
        }
        //========================================================================================

        public async Task<DataTable> GetFullName(string userid)
        {
            var payload = new { userid };
            string responseBody = await SendPostRequestAsync("GetFullName", payload);

            if (string.IsNullOrEmpty(responseBody))
            {
                return new DataTable(); // Return an empty DataTable instead of failing
            }
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jsonData["message"].ToString()) ?? new DataTable();
            //RfcConfigParameters parms = new RfcConfigParameters();
            //parms = sapconnection();
            //RfcDestination rfcDest = RfcDestinationManager.GetDestination(parms);
            //RfcRepository rfcRep = rfcDest.Repository;
            //IRfcFunction function = rfcRep.CreateFunction("BAPI_PERSDATA_GETDETAILEDLIST");
            //function.SetValue("EMPLOYEENUMBER", userid);
            //function.SetValue("TIMEINTERVALLOW", "18000101");
            //function.SetValue("TIMEINTERVALHIGH", "99991231");

            //RfcSessionManager.BeginContext(rfcDest);
            //IRfcTable tblReturn = function.GetTable("PERSONALDATA");
            //function.Invoke(rfcDest);
            //DataTable sapTable = new DataTable();

            //for (int liElement = 0; liElement < tblReturn.ElementCount; liElement++)
            //{
            //    RfcElementMetadata metadata = tblReturn.GetElementMetadata(liElement);
            //    sapTable.Columns.Add(metadata.Name);
            //}

            //foreach (IRfcStructure row in tblReturn)
            //{
            //    DataRow sapdr = sapTable.NewRow();
            //    for (int liElement = 0; liElement < tblReturn.ElementCount; liElement++)
            //    {
            //        RfcElementMetadata metadata = tblReturn.GetElementMetadata(liElement);
            //        sapdr[metadata.Name] = row.GetString(metadata.Name);
            //    }
            //    sapTable.Rows.Add(sapdr);
            //}
            //RfcSessionManager.EndContext(rfcDest);

            //return sapTable;
        }
        public async Task<DataTable> GetAddressDetail(string userid, string AddressType)
        {
            var payload = new { userid, AddressType };
            string responseBody = await SendPostRequestAsync("GetAddressDetail", payload);

            if (string.IsNullOrEmpty(responseBody))
            {
                return new DataTable(); // Return an empty DataTable instead of failing
            }
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jsonData["message"].ToString()) ?? new DataTable();
            //RfcConfigParameters parms = new RfcConfigParameters();
            //parms = objessaddress.sapconnection();
            //RfcDestination rfcDest = RfcDestinationManager.GetDestination(parms);
            //RfcRepository rfcRep = rfcDest.Repository;
            //IRfcFunction function = rfcRep.CreateFunction("BAPI_ADDRESSEMP_GETDETAIL");
            //function.SetValue("EMPLOYEENUMBER", txtpersonelnumber.Text.Trim());
            //function.SetValue("SUBTYPE", subtype);
            //function.SetValue("OBJECTID", "");
            //function.SetValue("LOCKINDICATOR", "");
            //function.SetValue("RECORDNUMBER", "0");
            //function.SetValue("VALIDITYBEGIN", low);
            //function.SetValue("VALIDITYEND", high);

            //RfcSessionManager.BeginContext(rfcDest);
            //function.Invoke(rfcDest);

            //City = function.GetString("CITY");
            //Nameofcountry = function.GetString("NAMEOFCOUNTRY");
            //District = function.GetString("DISTRICT");
            //Streetandhouseno = function.GetString("STREETANDHOUSENO");
            //Scndaddressline = function.GetString("SCNDADDRESSLINE");
            //Postalcodecity = function.GetString("POSTALCODECITY");
            //Telephonenumber = function.GetString("TELEPHONENUMBER");
            //Coname = function.GetString("CONAME");
            //Nameofstate = function.GetString("NAMEOFSTATE");

            //return sapTable;
        }
        //************************* for testing
        public async Task<DataTable> Gettaxworkingtax(string userid, string strdate)
        {
            string message = string.Empty;
            var payload = new { userid, strdate };
            string responseBody = await SendPostRequestAsync("Gettaxworkingtax", payload);
            if (string.IsNullOrEmpty(responseBody))
            {
                return new DataTable();
            }
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jsonData["message"].ToString()) ?? new DataTable();
        }

        public async Task<DataTable> GetSalaryBreakup(string userid, string ACT_TYPE, string strfstratdate, string strfenddate)
        {

            string message = string.Empty;
            var payload = new { userid, ACT_TYPE, strfstratdate, strfenddate };
            string responseBody = await SendPostRequestAsync("GetSalaryBreakup", payload);
            if (string.IsNullOrEmpty(responseBody))
            {
                return new DataTable();
            }
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jsonData["message"].ToString()) ?? new DataTable();
        }
        public async Task<DataSet> GetSalaryBreakup2(string userid, string ACT_TYPE, string strfstratdate, string strfenddate)
        {

            string message = string.Empty;
            var payload = new { userid, ACT_TYPE, strfstratdate, strfenddate };
            string responseBody = await SendPostRequestAsync("GetSalaryBreakup2", payload);
            if (string.IsNullOrEmpty(responseBody))
            {
                return new DataSet();
            }
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet>(jsonData["message"].ToString()) ?? new DataSet();
        }
        public async Task<DataTable> GetPaySlip(string userid, string strsqano, string strpayv)
        {
            string message = string.Empty;
            var payload = new { userid, strsqano, strpayv };
            string responseBody = await SendPostRequestAsync("GetPaySlip", payload);
            if (string.IsNullOrEmpty(responseBody))
            {
                return new DataTable();
            }
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jsonData["message"].ToString()) ?? new DataTable();
        }
        public async Task<string> GetEmployeeUnblockDTL(string EmpCode, string Status1)
        {
            string message = string.Empty;
            var payload = new { EmpCode, Status1 };
            string responseBody = await SendPostRequestAsync("GetEmployeeUnblockDTL", payload);

            if (string.IsNullOrEmpty(responseBody))
            {
                return "";
            }
            JObject jsonObject = JObject.Parse(responseBody);
            message = jsonObject["message"]?.ToString() ?? string.Empty;

            return message;
        }

        public async Task<string> GetEmployeeBlockDTL(string EmpCode, string Status1)
        {
            string message = string.Empty;
            var payload = new { EmpCode, Status1 };
            string responseBody = await SendPostRequestAsync("GetEmployeeBlockDTL", payload);

            if (string.IsNullOrEmpty(responseBody))
            {
                return "";
            }
            JObject jsonObject = JObject.Parse(responseBody);
            message = jsonObject["message"]?.ToString() ?? string.Empty;

            return message;
        }
        ////Customer Master 
        ///
        public async Task<DataSet> GET_VENDOR_MASTER(string VENDORNO, string COMPANYCODE)
        {
            DataSet dt = new DataSet();
            var payload = new { VENDORNO, COMPANYCODE };
            string responseBody = await SendPostRequestAsync("GET_VENDOR_MASTER", payload);

            if (string.IsNullOrEmpty(responseBody))
            {
                return new DataSet(); // Return an empty DataTable instead of failing
            }
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet>(jsonData["message"].ToString()) ?? new DataTable();

        }
        public async Task<DataSet> GET_CUSTOMER_MASTER(string CUSTOMERNO, string COMPANYCODE)
        {
            DataSet dt = new DataSet();
            var payload = new { VENDORNO = CUSTOMERNO, COMPANYCODE = COMPANYCODE };
            string responseBody = await SendPostRequestAsync("GET_CUSTOMER_MASTER", payload);

            if (string.IsNullOrEmpty(responseBody))
            {
                return new DataSet(); // Return an empty DataTable instead of failing
            }
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet>(jsonData["message"].ToString()) ?? new DataTable();

        }
        // public DataTable GET_DIVSION_List(string CUSTOMERNO)
        public async Task<DataTable> GET_DIVSION_List(string CUSTOMERNO)
        {
            DataTable dt = new DataTable();
            var payload = new { CUSTOMERNO = CUSTOMERNO };
            string responseBody = await SendPostRequestAsync("GET_DIVSION_List", payload);

            if (string.IsNullOrEmpty(responseBody))
            {
                return new DataTable(); // Return an empty DataTable instead of failing
            }
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jsonData["message"].ToString()) ?? new DataTable();

        }
        public async Task<DataTable> GET_Bank_Verification(string BANK_CTRYCODE, string BANKKEY)
        {
            var payload = new { BANK_CTRYCODE, BANKKEY };
            string responseBody = await SendPostRequestAsync("GET_Bank_Verification", payload);
            if (string.IsNullOrEmpty(responseBody))
            {
                return new DataTable(); // Return an empty DataTable instead of failing
            }
            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jsonData["message"].ToString()) ?? new DataTable();
        }
        public async Task<DataTable> POST_CUSTOMER_MASTER(List<sapCust> _sapCust, List<sapWt> _sapWt)
        {
            DataTable sapTable = null;
            try
            {
                var payload = new { _sapCust, _sapWt };
                string responseBody = await SendPostRequestAsync("POST_CUSTOMER_MASTER", payload);
                if (string.IsNullOrEmpty(responseBody))
                {
                    return new DataTable(); // Return an empty DataTable instead of failing
                }
                var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jsonData["message"].ToString()) ?? new DataTable();


            }
            catch (Exception ex) { throw new Exception(ex.Message); }

            return sapTable;
        }
    }
}
