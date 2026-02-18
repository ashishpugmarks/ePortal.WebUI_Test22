using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ePortal.ViewModels
{
    public class BenevolentViewModel
    {
        public long EmpCode { get; set; }
        public string EmpName { get; set; }
        public string Opration { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }

        public long ASR_BENEVOLENTID { get; set; }

        public long EMPLOYEECODE { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime DEMISEDATE { get; set; }

        public String DEMISEREASONE { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime FROM_DT { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime TO_DT { get; set; }

        public int STATUS { get; set; }

        public long ASR_BENEVOLENT_DTID { get; set; }

        public decimal AMOUNT { get; set; }

        public long CONSENTBYEMPLOYEE { get; set; }

        public string CONSENTBYEmpName { get; set; }
        public string ContributionPeriod { get; set; }

        public string CONSENTBYEmpOpration { get; set; }

    }

    public class BENEVOLENT_MST
    {
        [DisplayName("ID")]
        public long ASR_BENEVOLENTID { get; set; }

        [Required]
        public long EMPLOYEECODE { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime DEMISEDATE { get; set; }

        [Required]
        public String DEMISEREASONE { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime FROM_DT { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime TO_DT { get; set; }

        public int STATUS { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime DATEADDED { get; set; }

        public long ADDEDBY { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime DATELSTMOD { get; set; }

        public long MODIFIEDBY { get; set; }
    }

    public class BENEVOLENT_DT
    {
        public long ASR_BENEVOLENT_DTID { get; set; }

        public long ASR_BENEVOLENTID { get; set; }

        public decimal AMOUNT { get; set; }

        public long CONSENTBYEMPLOYEE { get; set; }
        public short STATUS { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime DATEADDED { get; set; }

        public long ADDEDBY { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime DATELSTMOD { get; set; }

        public long MODIFIEDBY { get; set; }
    }

    public class SearchBenevolent
    {
        [DisplayName("Deceased Employee Code ")]
        public long DemiseEmpCode { get; set; }

        [DisplayName("From Date")]
        public string From_Dt { get; set; }

        [DisplayName("To Date ")]
        public string To_Dt { get; set; }

        [DisplayName("Contributor Employee Code")]
        public long ContEmpCode { get; set; }

        public long UserId { get; set; }

        public int Status { get; set; }

    }
}
