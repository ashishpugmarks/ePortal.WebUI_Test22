using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class ChroniclesViewModel
    {
        public string HRC_TITLE { get; set; }
        public string HRC_DESCRIPTION { get; set; }
        public string HRC_DOC_NAME { get; set; }
        public string HRC_DOC_TYPE { get; set; }
        public string? HRC_VALID_FROM { get; set; }
        public string? HRC_VALID_TILL { get; set; }
        public string? HRC_CREATED_BY { get; set; }
        public string? HRC_STATUS { get; set; }
        public int HRC_ROWID { get; set; }

    }

    public class ChroniclesViewAddModel
    {
        [Required(ErrorMessage = "Title is required")]
        public string HRC_TITLE { get; set; }

        public string HRC_DESCRIPTION { get; set; }

        [DataType(DataType.Date)]
        public DateTime? HRC_VALID_FROM { get; set; }

        [DataType(DataType.Date)]
        public DateTime? HRC_VALID_TILL { get; set; }

        [Required(ErrorMessage = "Document file is required")]
        public IFormFile HRC_DOC_NAME { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string HRC_STATUS { get; set; }
        public int? HRC_ROWID { get; set; }
    }
}
