using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace ePortal.ViewModels
{
    public partial class ISMSMasterViewModel
    {

        [DisplayName("Sr.No")]
        public long SRNO { get; set; }

        [DisplayName("ISMS File")]
        public byte[]? ISMS_BLOB { get; set; }
        public string? ISMS_CONTENTTYPE { get; set; }        

        [DisplayName("ISMS File")]
        public string? ISMS_UPLOAD { get; set; }

        [Required(ErrorMessage = "Please Select App Auth1")]

        [DisplayName("App Auth1")]
        public long? APP_AUTH1 { get; set; }

        public string? APP_AUTH1_NAME { get; set; }

        [Required(ErrorMessage = "Please Select App Auth2")]

        [DisplayName("App Auth2")]
        public long? APP_AUTH2 { get; set; }

        public string? APP_AUTH2_NAME { get; set; }

        [DisplayName("Status")]
        public long? STATUS { get; set; }

        [DisplayName("Created Date")]
        public System.DateTime? CREATED_DATE { get; set; }
        public string? strCREATED_DATE { get; set; }

        [DisplayName("Created By")]
        public string? CREATED_BY { get; set; }
        public long? CREATED_BY1 { get; set; }

        [DisplayName("Modified Date")]
        public System.DateTime? MODIFIED_DATE { get; set; }

        [DisplayName("Modified By")]
        public string? MODIFIED_BY { get; set; }


        [Required(ErrorMessage = "Please Upload ISMS File")]

        public IFormFile? ISMSFILE { get; set; }
        public byte[]? FILE_BYTE { get; set; }
        public Int16? IsDeleted { get; set; }

        [DisplayName("Last Approved Date")]
        public string? LAST_APPPROVED_ON { get; set; }

        [DisplayName("Flag")]
        public long? FLAG { get; set; }

        public string? APPAUTH1_REMARKS { get; set; }
        public string? APPAUTH2_REMARKS { get; set; }

        public virtual ISMSMasterViewModel? SearchViewModel { get; set; }
        public List<ISMSMasterViewModel>? ISMSMaster { get; set; }

        public List<ISMSHDRDTLViewModel>? ISMSHDRDTLModel { get; set; }

    }

    public class ISMSHDRDTLViewModel
    {
        [DisplayName("ISMS Mapping ID")]
        public long ISMSMAPPINGID { get; set; }

        [DisplayName("ISMS SrNo")]
        public long? ISMS_MST_SRNO { get; set; }

        [DisplayName("Status")]
        public long? STATUS {get; set; }

        [DisplayName("App Auth Ecode")]
        public long? APPAUTH_ECODE { get; set; }

        [DisplayName("App Auth Name")]
        public string APPAUTH_NAME { get; set; }

        [DisplayName("App Auth Date")]
        public System.DateTime? APPAUTH_DATE { get; set; }

        [DisplayName("App Auth Remarks")]
        public string APPAUTH_REMARKS { get; set; }

        [DisplayName("Created By")]
        public long? BY { get; set; }

        [DisplayName("Created Date")]
        public System.DateTime? CREATED_DATE { get; set; }

        [DisplayName("Modified By")]
        public long? MODIFIED_BY { get; set; }

        [DisplayName("Modified Date")]
        public System.DateTime? MODIFIED_DATE { get; set; }

        [DisplayName("App Level")]
        public long? APP_LEVEL { get; set; }
                
    }
}
