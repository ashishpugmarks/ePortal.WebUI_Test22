using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public  class SapRfcConfig
    {
        public string URL { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Passkey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}
