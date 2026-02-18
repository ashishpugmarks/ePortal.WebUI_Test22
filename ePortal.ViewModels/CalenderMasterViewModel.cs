using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNetCore.Http;

namespace ePortal.ViewModels
{
    public partial class CalenderMasterViewModel
    {
        [DisplayName("Sr.No")]
        public long SRNO { get; set; }

        [Required(ErrorMessage = "Please Select Calendar")]

        [DisplayName("Calendar")]
        public string CALENDER { get; set; }
        //public Nullable<long> SelectedLocation { get; set; }
        public int[] SelectedLocation { get; set; }
        public string[] SelectedLocationDesc { get; set; }

        public List<int> SelectedLocationDesc1 { get; set; }

        //[Required(ErrorMessage = "Please Select Location")]

        [DisplayName("Location")] 
        public string LOCATION { get; set; }
        public string LOCATIONDESC { get; set; }

        [DisplayName("Financial Year")]
        public string FINANCIALYEAR { get; set; }

        [Required(ErrorMessage = "Please Select Financial Year")]
        public string SelectedFinancialYear { get; set; }
        //public List<SelectListItem> FinancialYears { get; set; }

        public List<string> FinancialYears { get; set; }  

        [DisplayName("Calendar File")]
        public byte[] CALENDER_BLOB { get; set; }
        public string CALENDER_CONTENTTYPE { get; set; }

        [DisplayName("Calendar File")]
        public string CALENDER_UPLOAD { get; set; }

        [Required(ErrorMessage = "Please Select App_Auth1")]

        [DisplayName("App Auth1")]
        public Nullable<long> APP_AUTH1 { get; set; }

        public string APP_AUTH1_NAME { get; set; }

        [DisplayName("App Auth1 Remarks")]
        public string APPAUTH1_REMARKS { get; set; }

        [DisplayName("App Auth1 Date")]
        public Nullable<System.DateTime> APPAUTH1_DATE { get; set; }

        [Required(ErrorMessage = "Please Select App_Auth2")]

        [DisplayName("App Auth2")]
        public Nullable<long> APP_AUTH2 { get; set; }
        public string APP_AUTH2_NAME { get; set; }

        [DisplayName("App Auth2 Remarks")]
        public string APPAUTH2_REMARKS { get; set; }

        [DisplayName("App Auth2 Date")]
        public Nullable<System.DateTime> APPAUTH2_DATE { get; set; }

        [DisplayName("Status")]
        public Nullable<long> STATUS { get; set; }

        [DisplayName("Flag")]
        public Nullable<long> FLAG { get; set; }

        public Nullable<long> CREATED_BY1 { get; set; }

        [DisplayName("Created By")]
        public string CREATED_BY { get; set; }
        public string CREATED_BY_USER { get; set; }
        

        [DisplayName("Created Date")]
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string FrmCREATED_DATE { get; set; }

        [DisplayName("Modified By")]
        public string MODIFIED_BY { get; set; }

        [DisplayName("Modified Date")]
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }

        [Required(ErrorMessage = "Please Upload Calendar")]

        public IFormFile CALENDERFILE { get; set; }
        public byte[] FILE_BYTE { get; set; }
        public Int16 IsDeleted { get; set; }

        [DisplayName("Last Approved Date")]
        public string LAST_APPPROVED_ON { get; set; }

        public virtual CalenderSearchViewModel SearchViewModel { get; set; }

        public List<CalenderMasterViewModel> CalenderMaster { get; set; }
        public List<CALENDERMASTERMAPPINGTRNViewModel> CALENDERMASTERMAPPINGTRN { get; set; }



    }

    public class CALENDERMASTERMAPPINGTRNViewModel
    {

        [DisplayName("Calendar Mapping ID")]
        public long CALENDERMAPPINGID { get; set; }

        [DisplayName("Calendar SrNo")]
        public Nullable<long> CAL_MAS_SRNO { get; set; }

        [DisplayName("Status")]
        public Nullable<long> STATUS { get; set; }

        [DisplayName("App Auth Ecode")]
        public Nullable<long> APPAUTH_ECODE { get; set; }

        public string APPAUTH_NAME { get; set; }

        [DisplayName("App Auth Date")]
        public Nullable<System.DateTime> APPAUTH_DATE { get; set; }

        [DisplayName("App Auth Remarks")]
        public string APPAUTH_REMARKS { get; set; }

        [DisplayName("Created By")]
        public Nullable<long> CREATEDBY { get; set; }

        [DisplayName("Created Date")]
        public Nullable<System.DateTime> CREATEDDATE { get; set; }

        [DisplayName("Modified By")]
        public Nullable<long> MODIFIEDBY { get; set; }

        [DisplayName("Modified Date")]
        public Nullable<System.DateTime> MODIFIEDDATE { get; set; }

        [DisplayName("App Level")]
        public Nullable<long> APP_LEVEL { get; set; }
                
    }
}
