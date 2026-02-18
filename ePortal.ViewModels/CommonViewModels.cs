using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class FileViewModel
    {
        public String FileName { get; set; }
        public String FileContentType { get; set; }
        public byte[] File { get; set; }
    }

    public class SearchViewModel
    {
        public String? Name { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public long? EmployeeCode { get; set; }
        public Int16? Status { get; set; }
    }

    public class ProcessMstViewModel
    {
        [DisplayName("Id")]
        public long PROCESSID { get; set; }

        [Required]
        [DisplayName("Content Process")]
        public string PROCESS_NAME { get; set; }

        [DisplayName("Status")]
        public Int16 STATUS { get; set; }

        [DisplayName("Created Date")]
        public System.DateTime CREATED_DATE { get; set; }
        public string CreateDate { get; set; }

        [DisplayName("Created By")]
        public long CREATED_BY { get; set; }
        public Nullable<long> MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }

        [DisplayName("Upload Banner")]
        public byte[] Upload_Banner { get; set; }
        public string BANNER_CONTENTTYPE { get; set; }

        [DisplayName("Banner Name")]
        public string BANNER_NAME { get; set; }

        [DisplayName("Banner")]
        public string DISPLAY_FILE { get; set; }
        public string User_Name { get; set; }
    }

    public class ScreenViewModel
    {
        [DisplayName("Id")]
        public long SAVERID { get; set; }
        public string? DepartmentName { get; set; }
        public DateTime? LASTREVISEDDATE { get; set; }
        public Int16? Status { get; set; }
        public string? FILE_NAME { get; set; }
        public string? FILE_CONTENTTYPE { get; set; }
        public byte[]? Policies { get; set; }
    }

    //Added by aumento Calendar Master =================================
    public class CalenderSearchViewModel
    {
        public string SelectedFinancialYear { get; set; }
        public List<string> FinancialYears { get; set; }
        public string FINANCIALYEAR { get; set; }
        public string CALENDER { get; set; }
        public string LOCATION { get; set; }
    }
    //================================================

    //Added by TTL on 28-July-2025 against SR104160 > CR6821 - Start
    public class DropdownList
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }
    //Added by TTL on 28-July-2025 against SR104160 > CR6821 - End
}
