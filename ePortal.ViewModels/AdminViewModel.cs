using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class EmailGroupViewModel
    {
        [DisplayName("SNo.")]
        public long EMAILGROUPMSTID { get; set; }

        [DisplayName("Group Name")]
        public string GROUP_NAME { get; set; }

        [DisplayName("Group Type")]
        public string? GROUP_TYPE_NAME { get; set; }
        public short? GROUP_TYPE { get; set; }

        [DisplayName("Location/Operation")]
        public string GROUP_VALUE_STR { get; set; }

        public long[]? GROUP_VALUE_ARRAY { get; set; }

        //[DisplayName("Status")]
        //public bool STATUS { get; set; }
        [DisplayName("Status")]
        public short? STATUS { get; set; }

        [DisplayName("Added By")]
        public long? ADDED_BY { get; set; }
        public string? ADDED_NAME { get; set; }

        [DisplayName("Added Date")]
        public DateTime? ADDED_DATE { get; set; }
        public long? UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }

        [DisplayName("Remark")]
        public string REMARK { get; set; }
        public List<EmailGroupMappViewModel>? EmailMapp_List { get; set; }
    }

    public class EmailGroupMappViewModel
    {
        public long EMAILGROUPMAPID { get; set; }
        public long GROUP_VALUE { get; set; }
        public string GROUP_MAPP_NAME { get; set; }
        public long EMAILGROUPMSTID { get; set; }
        public short STATUS { get; set; }
        public long ADDED_BY { get; set; }
        public System.DateTime ADDED_DATE { get; set; }
        public Nullable<long> UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
    }

    public class SelectListViewModel
    {
        public String Text { get; set; }
        public long Value { get; set; }
    }

    public class MyVoiceViewModel
    {

        [DisplayName("Request ID")]
        public string ID { get; set; }
        [DisplayName("Ecode")]
        public string ECODE { get; set; }
        [DisplayName("Employee Name")]
        public string EMP_NAME { get; set; }
        public string EMAILID { get; set; }

        [DisplayName("Idea")]
        public string IDEA { get; set; }
        [DisplayName("Status")]
        public string STATUS { get; set; }
        [DisplayName("Implemented On")]
        public string IMPLEMENTED_ON { get; set; }
        [DisplayName("Mail Status")]
        public string EMAIL_STATUS { get; set; }

        public string CERTIFICATE_NAME { get; set; }
    }

}
