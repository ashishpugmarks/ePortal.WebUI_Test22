using ePortal.Shared.Interface;
using Microsoft.Extensions.Configuration;

namespace ePortal.Shared
{
    public class AppConfig
    {
    }

    /// <summary>
    /// To GET SERVER PATH ON THE BASE OF USER NAME/SERVER
    /// </summary>
    public static class serverpath
    {        
        private static IAppConfigurationService _configService;

        public static void Initialize(IAppConfigurationService configService)
        {
            _configService = configService;
        }

        //public void DoSomething()
        //{
        //    var appName = _configService.GetGeneralSettings().AppName;  //Test code to get appsetting.json setting
        //    var connection = _configService.GetConnectionString("HRDatabase"); //Test code to get connection string
        //}

        public static string getServerPath()
        {
            string strName = string.Empty;
            //strName = ConfigurationManager.AppSettings["Get_Server_Path"].ToString();
            strName = _configService.GetGeneralSettings().Get_Server_Path;
            return strName;
        }
        public static string getFileUploadPath(string Path)
        {
            string strPath = string.Empty;
            string finalPath = string.Empty;
            //strName = ConfigurationManager.AppSettings["Get_Server_Path"].ToString();
            strPath = _configService.GetGeneralSettings().Get_FileUpload_Path+ Path;
            finalPath = _configService.MapPath(strPath);          
            return strPath;
            
        }

        public static string getFileUploadPath()
        {
            string strPath = string.Empty;
            string finalPath = string.Empty;

            //strPath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["Get_FileUpload_Path"].ToString());
            strPath =_configService.GetGeneralSettings().Get_FileUpload_Path;
            finalPath = _configService.MapPath(strPath);           
            return finalPath;
        }

        public static string getPhotoPath()
        {
            string strPath = string.Empty;
            string finalPath = string.Empty;
            //strPath = HttpContext.Current.Server.MapPath(_configService.GetGeneralSettings().Get_PhotoPath);

            strPath = _configService.GetGeneralSettings().Get_PhotoPath;
            finalPath = _configService.MapPath(strPath);
            return finalPath;
        }

        public static Boolean isTestServer()
        {
            bool boolTestServer;
            if (System.Environment.UserName == "akhileshgupta1")
                boolTestServer = true;
            else if (System.Environment.UserName == "vimalbansal")
                boolTestServer = true;
            else if (System.Environment.UserName == "avinashsingh")
                boolTestServer = true;
            else if (System.Environment.UserName == "santoshbisht")
                boolTestServer = true;
            //else if (HttpContext.Current.Request.ServerVariables["SERVER_NAME"].ToLower() == "blmgp01")
            //    boolTestServer = true;
            //else if (HttpContext.Current.Request.ServerVariables["SERVER_NAME"] == "hmsi-11-029022")
            //    boolTestServer = true;
            else if (_configService.GetServerName() == "blmgp01")
                boolTestServer = true;
            else if (_configService.GetServerName() == "hmsi-11-029022")
                boolTestServer = true;
            else
                boolTestServer = false;
            return boolTestServer;
        }

        public static string getFileUploadPage()
        {
            string strPath = string.Empty;
            if (System.Environment.UserName == "ofsisakhilesh")
                strPath = getServerPath() + "Admin/IndustrialRelation/FileUpload.aspx";
            //else if (HttpContext.Current.Request.ServerVariables["SERVER_NAME"].ToLower() == "blmgp01")
            //    strPath = getServerPath() + "Admin/IndustrialRelation/FileUpload.aspx";
            else if (_configService.GetServerName() == "blmgp01")
                strPath = getServerPath() + "Admin/IndustrialRelation/FileUpload.aspx";
            else
                strPath = getServerPath() + "Admin/IndustrialRelation/FileUpload.aspx";
            return strPath;
        }

        public static string getTestEMail()
        {
            return _configService.GetGeneralSettings().Get_Test_EMail;
        }

        public static string getConnectingString()
        {
            string strCn = string.Empty;
            strCn = _configService.GetConnectionString("cnConn");
            return strCn;
        }
    }

    //public class ConnectionString
    //{
    //    private static IAppConfigurationService _configService;

    //    public static void Initialize(IAppConfigurationService configService)
    //    {
    //        _configService = configService;
    //    }


    //    public string getConnectingString()
    //    {
    //        string strCn = string.Empty;
    //        strCn = ConfigurationManager.ConnectionStrings["cnConn"].ConnectionString;
    //        return strCn;
    //    }

    //    public string getConnectingStringVQMS(String Factory)
    //    {
    //        string strCn = string.Empty;
    //        switch (Factory)
    //        {
    //            case "3":
    //                strCn = ConfigurationManager.ConnectionStrings["cnConn3"].ConnectionString;
    //                break;
    //            case "6":
    //                strCn = ConfigurationManager.ConnectionStrings["cnConn6"].ConnectionString;
    //                break;
    //            case "8":
    //                strCn = ConfigurationManager.ConnectionStrings["cnConn8"].ConnectionString;
    //                break;
    //            case "21":
    //                strCn = ConfigurationManager.ConnectionStrings["cnConn21"].ConnectionString;
    //                break;
    //            default:
    //                strCn = ConfigurationManager.ConnectionStrings["cnConn"].ConnectionString;
    //                break;
    //        }
    //        return strCn;
    //    }

    //    public interface IContentPlaceHolders
    //    {
    //        IList GetContentPlaceHolders();
    //    }

    //    public string strConnectionString()
    //    {
    //        return ConfigurationManager.AppSettings["Get_Connection_String"].ToString();
    //    }
    //}
}
