using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Shared.Configuration
{
    public class GeneralSettings
    {
        public string APIAddress { get; set; }
        public string SAP_Connection_Path { get; set; }
        public string Get_Server_Path { get; set; }
        public string Get_FileUpload_Path { get; set; }
        public string Get_PhotoPath { get; set; }
        public string Get_Test_EMail { get; set; }
        public int IdleTimeoutMinutes { get; set; }
    }

    public class ApiSettings
    {
        public string BaseUrl { get; set; }
        public string verify_sso { get; set; }
    }

    public class FCMSettings
    {
        public string FCMToken { get; set; }
        public string FCMNotificationUrl { get; set; }
    }
    
    public class EmailSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public int SendEmail { get; set; }
    }
}
