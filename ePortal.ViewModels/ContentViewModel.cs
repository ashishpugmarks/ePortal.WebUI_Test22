using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class ContentViewModel
    {
        public long ATTACHMENTID { get; set; }
        public long PROCESSID { get; set; }
        public string SUBJECT { get; set; }
        public string BRIEF { get; set; }
        public string DESCRIPTION { get; set; }
        public byte[] BANNER { get; set; }
        public string BANNER_CONTENTTYPE { get; set; }
        public string BANNER_NAME { get; set; }
        public byte[] ATTACHMENT1 { get; set; }
        public string ATTACHMENT1_CONTENTTYPE { get; set; }
        public string ATTACHMENT1_NAME { get; set; }
        public byte[] ATTACHMENT2 { get; set; }
        public string ATTACHMENT2_CONTENTTYPE { get; set; }
        public string ATTACHMENT2_NAME { get; set; }
        public System.DateTime START_DATE { get; set; }
        public System.DateTime END_DATE { get; set; }
        public short STATUS { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public long CREATED_BY { get; set; }
        public Nullable<long> MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string FUNCTIONAL_DESIGNATION { get; set; }
        public string DESIGNATION { get; set; }
        public string SITE { get; set; }
        public byte[] POPUPHEADER { get; set; }
        public string POPUPHEADER_CONTENTTYPE { get; set; }
        public string POPUPHEADER_NAME { get; set; }

        public string START_DATE_STR { get { return START_DATE.ToString("dd-MMM-yyyy"); }  }
        public string START_TIME_STR { get { return START_DATE.ToString("hh:mm"); } }

        public string OPERATION { get; set; }
        public string DIVISION { get; set; }
        public string DEPARTMENT { get; set; }
    }
}
