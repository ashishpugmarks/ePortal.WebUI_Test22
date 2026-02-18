using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web;
using Microsoft.AspNetCore.Http;


namespace ePortal.ViewModels
{
    public partial class ScreenSaverViewModel
    {
        [DisplayName("ID")]
        public int SAVERID { get; set; }

        [DisplayName("Department Name")]
        public string? DEPARTNAME { get; set; }

        [Required]
        [DisplayName("Status")]
        public Int16 STATUS { get; set; }

        //Below Added by Aumento For SR66258--------------------------------
        public string? STATUS1 { get; set; }

        //------------------------------------------------------------------

        [DisplayName("Last Revised Date")]
        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}")]
        public string? LASTREVISEDDATE { get; set; }
     
        public IFormFile? FILE_CONTENTTYPE { get; set; }
        public string? PoliciesContentType { get; set; }
        [Required]
        [DisplayName("Upload File")]
        public IFormFile FILE_NAME { get; set; }
        //[Required]
        public string? PoliciesFileName { get; set; }
        public byte[]? Policies { get; set; }
        public List<ScreenSaverViewModel>? ScreenSaver { get; set; }
        public int ADDEDBY { get; set; }

        public virtual ScreenViewModel? SearchViewModel { get; set; }
        public int MODIFIED_BY { get; set; }
    }

    
}
