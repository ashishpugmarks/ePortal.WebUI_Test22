using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;


namespace ePortal.ViewModels
{
    public class ADPOWERSCOPEMASTERViewModel
    {
        [DisplayName("Sr. No.")]
        public long SRNO { get; set; }
        
        [DisplayName("Employee Code")]
        public string ADEMPCODE { get; set; }
        
        [DisplayName("Power")]
        public string POWER { get; set; }

        [DisplayName("Scope")]
        public string SCOPE { get; set; }

        [DisplayName("Remarks")]
        public string REMARKS { get; set; }        

        [DisplayName("Request Date")]
        public System.DateTime DATEADDED { get; set; }

        [DisplayName("Request By")]
        public long ADDEDBY { get; set; }

        [DisplayName("Updated By")]
        public Nullable<long> MODIFIEDBY { get; set; }

        [DisplayName("Power Of Attorney")]
        public string ATTACHMENTFILENAME { get; set; }

        public IFormFile FILE { get; set; }
        public byte[] FILE_BYTE { get; set; }
        public string FILE_CONTENTTYPE { get; set; }

        public List<ADPOWERSCOPEMASTERViewModel> ADPOWERSCOPEDetail { get; set; }

        //public List<POAPowerScopeADPOWERMASTERViewModel> POAPowerMst { get; set; }

    }
    public class POAPowerScopeADPOWERMASTERViewModel
    {
        [DisplayName("SNo")]
        public long SRNO { get; set; }

        [DisplayName("POWER CODE")]
        public string POWERCODE { get; set; }

        [DisplayName("POWER NAME")]
        public string POWERNAME { get; set; }
        public string PNAME { get; set; }



        //============Change Done on 27082022 For Add Other Category by (Aumento)================================================================================
        //[DisplayName("Header")]
        //public string Header { get; set; }
        //==========================================================================================================================================================
    }

    public class Root
    {
        public string SrNo { get; set; }
        public string ADEMPCODE { get; set; }
        public long ADEMPCODE_ { get; set; }
        public string Power { get; set; }
        public string Scope { get; set; }
        public string Remarks { get; set; }
        public string ATTACHMENTFILENAME { get; set; }

        //Below Added for the SR56054 ====================      
        public Nullable<long> operationID { get; set; }
        public string operation { get; set; }
        public Nullable<long> DivisionID { get; set; }
        public string Division { get; set; }
        public Nullable<long> DepartmentID { get; set; }
        public string Department { get; set; }
        public Nullable<long> SectionID { get; set; }
        public string Section { get; set; }
        //================================================       
    }


    public class AttachmentModel
    {
        public string POWERCODE { get; set; }
        public string POWERNAME { get; set; }
        public string SRNO { get; set; }
    }

}

