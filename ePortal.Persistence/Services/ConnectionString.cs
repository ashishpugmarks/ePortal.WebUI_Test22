using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Persistence.Interface;
using ePortal.Shared.Interface;

namespace ePortal.Persistence.Services
{
    public class ConnectionString:IConnectionString
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
                    strCn = _configService.GetConnectionString("cnConnDPR3");
                    break;
                case "6":
                    strCn = _configService.GetConnectionString("cnConnDPR6");
                    break;
                case "8":
                    strCn = _configService.GetConnectionString("cnConnDPR8");
                    break;
                case "21":
                    strCn = _configService.GetConnectionString("cnConnDPR21");
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
