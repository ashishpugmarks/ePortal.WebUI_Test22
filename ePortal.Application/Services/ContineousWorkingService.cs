using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;
using ePortal.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static ePortal.ViewModels.SAPPayload;

namespace ePortal.Application.Services
{
    public class ContineousWorkingService : IContineousWorkingService
    {
        private readonly ContineousWorkingRepository _ContineousWorkingRepository;
        private readonly SapRfcConfig _sapConfig;

        public ContineousWorkingService(ContineousWorkingRepository ContineousWorkingRepository, IOptions<SapRfcConfig> sapConfig)
        {
            _ContineousWorkingRepository = ContineousWorkingRepository;
            _sapConfig = sapConfig.Value;

        }
        public async Task<List<ContineousWorkingRowViewModel>> ContineousWorkingDashboard(
     SearchContineousWorkingViewModel SWM, string empCode)
        {
            var sqlList = _ContineousWorkingRepository.ContineousWorkingDashboard(SWM);

            if (sqlList == null || sqlList.Count == 0)
                return new List<ContineousWorkingRowViewModel>();

            var firstValidRow = sqlList
                .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.DATETO));

            if (firstValidRow == null)
                return new List<ContineousWorkingRowViewModel>();

            DateTime sampleDate = DateTime.ParseExact(
                firstValidRow.DATETO,
                "dd-MMM-yyyy",
                CultureInfo.InvariantCulture
            );

            string sapDateParam = new DateTime(sampleDate.Year, sampleDate.Month, 1)
                .ToString("dd.MM.yyyy");

            Dictionary<string, List<EmployeeAttendance>> sapCache =
                new Dictionary<string, List<EmployeeAttendance>>();

            List<ContineousWorkingRowViewModel> finalList =
                new List<ContineousWorkingRowViewModel>();

            foreach (var row in sqlList)
            {
                if (string.IsNullOrWhiteSpace(row.PUNCHDATE) ||
                    string.IsNullOrWhiteSpace(row.DATETO))
                    continue;

                DateTime fromDate;
                DateTime toDate;

                try
                {
                    fromDate = DateTime.ParseExact(
                        row.PUNCHDATE,
                        "dd-MMM-yyyy",
                        CultureInfo.InvariantCulture
                    );

                    toDate = DateTime.ParseExact(
                        row.DATETO,
                        "dd-MMM-yyyy",
                        CultureInfo.InvariantCulture
                    );
                }
                catch
                {
                    continue; 
                }

                if (fromDate > toDate)
                    continue;

                string empCodeKey = row.ADEMPCODE.ToString();
                                
                if (!sapCache.ContainsKey(empCodeKey))
                {
                    var sapList = await getAttendanceData(empCodeKey, sapDateParam, "x");
                    sapList ??= new List<EmployeeAttendance>();

                    sapCache[empCodeKey] = sapList
                        .Where(x => x.EMP_LDATE != DateTime.MinValue)
                        .ToList();
                }

                
                var sapDict = sapCache[empCodeKey]
                    .Where(x => x.EMP_LDATE != DateTime.MinValue)
                    .GroupBy(x => x.EMP_LDATE.Date)
                    .ToDictionary(g => g.Key, g => g.First());

                int counter = row.CONTINUOUSDAYS;

                for (DateTime d = toDate; d >= fromDate; d = d.AddDays(-1))
                {
                    var newRow = new ContineousWorkingRowViewModel
                    {
                        ADEMPCODE = row.ADEMPCODE,
                        ASSOCIATE_NAME = row.ASSOCIATE_NAME,
                        OPERATION = row.OPERATION,
                        DIVISION = row.DIVISION,
                        DEPARTMENT = row.DEPARTMENT,
                        SECTION = row.SECTION,
                        DESIGNATION = row.DESIGNATION,
                        //LOCATION = row.LOCATION,
                        SITE_DESCRIP = row.SITE_DESCRIP,
                        STATUS = row.STATUS,
                        CONTINUOUS_WORKING_FROM = row.PUNCHDATE,
                        PUNCHDATE = d.ToString("dd-MMM-yyyy"),
                        DATETO = row.DATETO,
                        CONTINUOUSDAYS = counter
                    };

                    if (sapDict.TryGetValue(d.Date, out var sap))
                    {
                        if (!string.IsNullOrWhiteSpace(sap.EMP_IN_TIME) &&
                            sap.EMP_IN_TIME != "000000" &&
                            sap.EMP_IN_TIME.Length == 6)
                        {
                            newRow.PunchIN = DateTime.ParseExact(
                                sap.EMP_IN_TIME,
                                "HHmmss",
                                CultureInfo.InvariantCulture
                            ).ToString("hh:mm tt");
                        }

                        if (!string.IsNullOrWhiteSpace(sap.EMP_OUT_TIME) &&
                            sap.EMP_OUT_TIME != "000000" &&
                            sap.EMP_OUT_TIME.Length == 6)
                        {
                            newRow.PunchOUT = DateTime.ParseExact(
                                sap.EMP_OUT_TIME,
                                "HHmmss",
                                CultureInfo.InvariantCulture
                            ).ToString("hh:mm tt");
                        }

                        if (!string.IsNullOrWhiteSpace(sap.EMP_SHIFT))
                        {
                            var shift = sap.EMP_SHIFT.ToUpper();
                            newRow.SHIFT = (shift == "OFF" || shift == "LOFF")
                                ? "OFF"
                                : shift.Length >= 3 ? shift[2].ToString() : shift;
                        }
                    }

                    finalList.Add(newRow);
                    counter--;
                }
            }

            return finalList
                .GroupBy(x => new { x.ADEMPCODE, x.PUNCHDATE })
                .Select(g => g.First())
                .OrderBy(x => x.ADEMPCODE)
                .ThenBy(x => DateTime.ParseExact(
                    x.PUNCHDATE,
                    "dd-MMM-yyyy",
                    CultureInfo.InvariantCulture
                ))
                .ToList();
        }

        

        public async Task<List<EmployeeAttendance>> getAttendanceData(string userid, string strdate, string strcheck)
        {
            getAttendanceData apipayload = new getAttendanceData
            {
                userid = userid,
                strdate = strdate,
                strcheck = strcheck
            };
            try
            {
                //Configure HttpClientHandler to perform SSL validation
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, certChain, sslPolicyErrors) =>
                    {  // Allow if there are no SSL policy errors
                        if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
                        {
                            return true;
                        }
                        // Optionally log or handle specific SSL errors
                        // For example, you might allow RemoteCertificateNameMismatch in dev environments
                        if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch)
                        {
                            return true; // Be cautious about using this in production
                        }

                        // Deny connection if other errors are present
                        return false;
                    }
                };

                //**********************Generate JWT Token *********************************************
                string username = _sapConfig.UserName;
                string passkey = _sapConfig.Passkey;
                string payload = JsonSerializer.Serialize(apipayload);
                string secretkey = ComputeMD5Hash(_sapConfig.Passkey);

                var token = GenerateToken(username, payload, secretkey);
                using var httpClient = new HttpClient(handler);

                var Jobj = new { token };

                string jsonString = JsonSerializer.Serialize(Jobj);

                //// Set up the POST request
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(_sapConfig.URL + "overstay/GetOverstayReport", content);
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("SAP Raw Response: " + responseBody);

                SAPResponse sAPResponse = JsonSerializer.Deserialize<SAPResponse>(responseBody);
                List<EmployeeAttendance> responseJson = null;
                if (sAPResponse.status == "Success")
                {
                    responseJson = JsonSerializer.Deserialize<List<EmployeeAttendance>>(sAPResponse.message);
                }
                return responseJson;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        private string GenerateToken(string username, string payload, string secrectkey)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secrectkey));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                    {
                    new Claim(JwtRegisteredClaimNames.Sub, username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("Payload",payload)
                };

                var token = new JwtSecurityToken(issuer: _sapConfig.Issuer,
                                                  audience: _sapConfig.Audience,
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


    }
}
