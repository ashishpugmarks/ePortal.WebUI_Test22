using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ePortal.Persistence.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ePortal.Persistence.Services
{
    public class SapDataProvider : ISapDataProvider
    {
        private readonly IConfiguration _configuration;

        public SapDataProvider(IConfiguration iConfiguration)
        {
            _configuration = iConfiguration;
        }

        public async Task<string> SendPostRequestAsync(string endpoint, object payload)
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

        private static string ComputeMD5Hash(string plainText)
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
