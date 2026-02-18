using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Shared.Configuration
{
    public class DbConnectionOptions
    {
        public string OracleDbConnection { get; set; }
        public string cnConn11G { get; set; }
        public string cnConnDPR3 { get; set; }
        public string cnConnDPR6 { get; set; }
        public string cnConnDPR8 { get; set; }
        public string cnConnDPR21 { get; set; }
        public string cnConnSLS { get; set; }
    }
}
