using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;
using ePortal.Persistence;
using ePortal.Persistence.Interface;
using ePortal.Shared;
using ePortal.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;


namespace ePortal.Application.Services
{
    public class EmployeeLoginService : IEmpLoginService
    {
        private readonly EmployeeLoginRepository _objloginRepositry ;
        private readonly CommonRepository _objcommRespository;
        private readonly ICommonFunctions _commFunDAL;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<EmployeeLoginService> _logger;

        //public EmployeeLoginService(EmployeeLoginRepository objloginRepositry, CommonRepository objcommRespository)
        //{
        //    _objloginRepositry = objloginRepositry;
        //    _objcommRespository = objcommRespository;
        //}

        public EmployeeLoginService(EmployeeLoginRepository objloginRepositry, CommonRepository objcommRespository, ICommonFunctions commFunDAL, IConfiguration iConfiguration, IHttpContextAccessor httpContextAccessor, ILogger<EmployeeLoginService> logger)
        {
            _objloginRepositry = objloginRepositry;
            _objcommRespository = objcommRespository;
            _commFunDAL = commFunDAL;
            _configuration = iConfiguration;
            _httpContextAccessor = httpContextAccessor;
            _logger=logger;
        }


        public string Check_AutoLogin(string strUserID, string strToken, string Usertype)
        {
            return _objloginRepositry.Check_AutoLogin(strUserID, strToken, Usertype);
        }
        public string Check_MailLogin(string strUserID)
        {
            return _objloginRepositry.Check_MailLogin(strUserID);
        }
        public int Check_EmpLogin(string strUserID, string strPWD, string Client_IP, string Usertype)
        {
            return _objloginRepositry.Check_EmpLogin(strUserID, strPWD, Client_IP, Usertype);
        }
        public int Check_Switch_EmpLogin(string strUserID, string Client_IP, string Usertype)
        {
            return _objloginRepositry.Check_Switch_EmpLogin(strUserID, Client_IP, Usertype);
        }
        public Employee_Details GetEmpDetails(string Userid)
        {
            return _objloginRepositry.GetEmpdetail(Userid);
        }

        public string GetParameterValue(string strParmaName)
        {
            return _objcommRespository.GetParameterValue(strParmaName);
        }
        public List<ContentViewModel> GetContent(ContentViewModel objSearch)
        {
            List<ContentViewModel> objdata = _objloginRepositry.GetContent(objSearch);
            List<ContentViewModel> retList = new List<ContentViewModel>();
            foreach (var o in objdata)
            {
                ContentViewModel obj = new ContentViewModel();
                obj.ATTACHMENTID = o.ATTACHMENTID;
                obj.SUBJECT = o.SUBJECT;
                obj.BRIEF = o.BRIEF;
                obj.BANNER = o.BANNER;
                obj.BANNER_CONTENTTYPE = o.BANNER_CONTENTTYPE;
                obj.BANNER_NAME = o.BANNER_NAME;
                obj.DESCRIPTION = o.DESCRIPTION == null ? o.DESCRIPTION : CommonRepository.HtmlToText(o.DESCRIPTION);
                obj.POPUPHEADER = o.POPUPHEADER;
                obj.POPUPHEADER_CONTENTTYPE = o.POPUPHEADER_CONTENTTYPE;
                obj.POPUPHEADER_NAME = o.POPUPHEADER_NAME;
                retList.Add(obj);
            }
            return retList;
        }
        public ContentViewModel GetVideo(ContentViewModel objsearch)
        {
            ContentViewModel objdata = _objloginRepositry.GetVideo(objsearch);
            return objdata;
        }

        public List<ContentViewModel> GetPopupContent(ContentViewModel objSearch)
        {
            List<ContentViewModel> objdata = _objloginRepositry.GetPopupContent(objSearch);
            List<ContentViewModel> retList = new List<ContentViewModel>();
            foreach (var o in objdata)
            {
                ContentViewModel obj = new ContentViewModel();
                obj.ATTACHMENTID = o.ATTACHMENTID;
                obj.SUBJECT = o.SUBJECT == null ? o.SUBJECT : CommonRepository.HtmlToText(o.SUBJECT);
                obj.BRIEF = o.BRIEF == null ? o.BRIEF : CommonRepository.HtmlToText(o.BRIEF);
                obj.BANNER = o.BANNER;
                obj.BANNER_CONTENTTYPE = o.BANNER_CONTENTTYPE;
                obj.BANNER_NAME = o.BANNER_NAME;
                //obj.DESCRIPTION =CommonRepository.HtmlToText(o.DESCRIPTION);
                obj.DESCRIPTION = o.DESCRIPTION == null ? o.DESCRIPTION : CommonRepository.HtmlToText(o.DESCRIPTION);
                obj.ATTACHMENT1 = o.ATTACHMENT1;
                obj.ATTACHMENT1_CONTENTTYPE = o.ATTACHMENT1_CONTENTTYPE;
                obj.ATTACHMENT1_NAME = o.ATTACHMENT1_NAME;
                obj.ATTACHMENT2 = o.ATTACHMENT2;
                obj.ATTACHMENT2_CONTENTTYPE = o.ATTACHMENT2_CONTENTTYPE;
                obj.ATTACHMENT2_NAME = o.ATTACHMENT2_NAME;
                obj.POPUPHEADER = o.POPUPHEADER;
                obj.POPUPHEADER_CONTENTTYPE = o.POPUPHEADER_CONTENTTYPE;
                obj.POPUPHEADER_NAME = o.POPUPHEADER_NAME;
                obj.START_DATE = o.START_DATE;
                retList.Add(obj);
            }
            return retList;
        }

        public string UpdateEmployeePassword(long uid, string strPassword, long modby, string Usertype)
        {
            return _objloginRepositry.UpdateEmployeePassword(uid, strPassword, modby, Usertype);
        }
        public string UpdateEmployeeMobileno(long uid, string strmobileno, long modby)
        {
            return _objloginRepositry.UpdateEmployeeMobileno(uid, strmobileno, modby);
        }
        public ContentViewModel GetAttachement2(ContentViewModel objsearch)
        {
            ContentViewModel objdata = _objloginRepositry.GetAttachement2(objsearch);
            return objdata;
        }
        public string MailApprovalInsert(string strwid, string strcontroler, string straction, string strTid)
        {
            string strval=string.Empty;
            //CommonFunctions objcomm = new CommonFunctions();
            // strval = objcomm.EmailApprovalInsert(strwid, strcontroler, straction, strTid);
            return strval;
        }

        public DataSet MailApprovalGet(long uid)
        {
            DataSet ds=null;

            //legacy DAL
            //CommonFunctions objcomm = new CommonFunctions();
            ds = _commFunDAL.EmailApprovalGet(uid);

            //Moved to EF6
            //ds = _objloginRepositry.EmailApprovalGet(uid);
            return ds;
        }

        public string GenerateJwtToken(string userName)
        {
            // Create claims for the JWT
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(ClaimTypes.Role, "User"),
                new Claim("userid", userName)
            };

            // Generate signing key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Calculate token expiration time
            var expirationTime = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:ExpirationInMinutes"]));
            //var expirationTime = DateTime.UtcNow.AddDays(1);

            // Create the JWT token
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: expirationTime, // Use the calculated expiration time
                signingCredentials: creds);

            // Generate the JWT string
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            // Generate a refresh token
            var refreshToken = GenerateRefreshToken(); // Implement this method to generate a refresh token

            // Store the JWT and refresh token in the database
            _objloginRepositry.StoreJwtUserLog(userName, jwtToken, refreshToken); // Ensure this method is implemented correctly

            return jwtToken; // Return the JWT token
        }

        public string GenerateRefreshToken()
        {
            // Generate a random refresh token
            return Guid.NewGuid().ToString();
        }

        //Added By TTL against CR6695 as on 29-07-2025 
        public bool WriteCookie(string cookiesName, CookieDetails _userContext, int expireInDays)
        {
            if (_userContext != null)
            {
                var _userContextSerialize = JsonConvert.SerializeObject(_userContext);
                _userContextSerialize = Encryption.Encrypt(_userContextSerialize);

               // _logger.LogError("Write Cookie:  cookie=" + _userContextSerialize + "  encrypted cookie=" + _userContextSerialize);

                _httpContextAccessor.HttpContext.Response.Cookies.Append(cookiesName, _userContextSerialize, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(expireInDays),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });
            }
            return true;
        }

        //Added By TTL against CR6695 as on 29-07-2025 
        public bool DeleteCookie(string cookiesName)
        {
            bool status = false;
            try
            {
                if (_httpContextAccessor.HttpContext.Request.Cookies.ContainsKey(cookiesName))
                {
                    _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookiesName, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    });
                    status= true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in Delete Cookies: "+ex.Message.ToString());
            }
            
            return status;
        }                
    }
}
