using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace ePortal.ViewModels
{
    public class AssetRegisterDocumentControlViewModel
    {
        [DisplayName("Sr. No.")]
        public long SRNO { get; set; }

        [Required(ErrorMessage = "Please Select SYKI ID")]

        [DisplayName("SYKI ID")]
        public Nullable<long> SYKIID { get; set; }

        [Required(ErrorMessage = "Please Enter Document Title")]

        [DisplayName("Document Title")]
        public string DOCUMENT_TITLE { get; set; }

        [Required(ErrorMessage = "Please Enter Date Of Release")]

        [DisplayName("Date Of Release")]        
        public Nullable<System.DateTime> DATE_OF_RELEASE { get; set; }
        public string DATE_OF_RELEASE1 { get; set; }

        [Required(ErrorMessage = "Please Enter Doc Version No")]

        [DisplayName("Doc Version No")]
        public string DOC_VERSION_NO { get; set; }

        [Required(ErrorMessage = "Please Enter Document Number")]

        [DisplayName("Document Number")]
        public string DOCUMENT_NUMBER { get; set; }


        [DisplayName("Created By")]
        public Nullable<long> CREATED_BY { get; set; }

        [DisplayName("Created Date")]
        public Nullable<System.DateTime> CREATED_DATE { get; set; }

        [DisplayName("Modified By")]
        public Nullable<long> MODIFIED_BY { get; set; }

        [DisplayName("Modified Date")]
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string Msg { get; set; }
        public string KICode { get; set; }
        public string AMENDMENT { get; set; }

    }

 }
