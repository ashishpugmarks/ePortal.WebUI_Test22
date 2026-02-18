using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Shared.Interface;

namespace ePortal.Persistence
{
    /// <summary>
    /// Summary description for ConnectionString
    /// </summary>
    public class ConnectionString
    {
        private static IAppConfigurationService _configService;

        public ConnectionString(IAppConfigurationService configService)
        {
            _configService = configService;
        }


        //public void DoSomething()
        //{
        //    var appName = _configService.GetGeneralSettings().AppName;  //Test code to get appsetting.json setting
        //    var connection = _configService.GetConnectionString("HRDatabase"); //Test code to get connection string
        //}

        public string getConnectingString()
        {
            string strCn = string.Empty;
            //strCn = ConfigurationManager.ConnectionStrings["cnConn"].ConnectionString;
            strCn = _configService.GetConnectionString("cnConn");
            return strCn;
        }

        public string getConnectingStringVQMS(String Factory)
        {
            string strCn = string.Empty;
            switch (Factory)
            {
                case "3":
                    strCn = _configService.GetConnectionString("cnConn3");
                    break;
                case "6":
                    strCn = _configService.GetConnectionString("cnConn6");
                    break;
                case "8":
                    strCn = _configService.GetConnectionString("cnConn8");
                    break;
                case "21":
                    strCn = _configService.GetConnectionString("cnConn21");
                    break;
                default:
                    strCn = _configService.GetConnectionString("cnConn");
                    break;
            }
            return strCn;
        }

        public interface IContentPlaceHolders
        {
            IList GetContentPlaceHolders();
        }

        public string strConnectionString()
        {
            //return ConfigurationManager.AppSettings["Get_Connection_String"].ToString();
            return _configService.GetConnectionString("Get_Connection_String");
        }
    }
}
