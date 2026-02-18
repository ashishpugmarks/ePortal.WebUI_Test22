using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.CustomerMgmt
{
    public class CustomerMasterDataMng
    {
            
            public short ID { get; set; }            
            public string? FIELDNAME { get; set; }            
            public string? ServiceType { get; set; }            
            public string? AG_5AG1 { get; set; }
            public string? AG_5AG3 { get; set; }
            public string? AG_5AGV { get; set; }
            public string? AG_5AGG { get; set; }
            public string? AG_5AG6 { get; set; }
            public string? AG_5AGW { get; set; }
            public string? AG_5AG2 { get; set; }
            public string? AG_5AG5 { get; set; }

            // VARCHAR2(150)
            public string? GROUP_NAME { get; set; }

            // NVARCHAR2(100)
            public string? CLIENTIDLABEL { get; set; }

            // NVARCHAR2(100)
            public string? CLIENTIDTEXTLABEL { get; set; }

            // CHAR(1)
            public string? IS_ACTION{ get; set; }

            // NVARCHAR2(50)
            public string? CreatedBy { get; set; }

            // TIMESTAMP(6)
            public DateTime? CreatedDate { get; set; }

            // NVARCHAR2(50)
            public string? UpdatedBy { get; set; }

            // TIMESTAMP(6)
            public DateTime? UpdatedDate { get; set; }

        }
}
