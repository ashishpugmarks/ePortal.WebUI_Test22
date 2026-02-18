using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{


    public class VPF_CreateDetailViewModel
    {
        public int ID { get; set; }

        [DisplayName("Emp-Code")]
        public long EMPLOYEECODE { get; set; }       

        [Required(ErrorMessage ="Request type is Required")]
        [DisplayName("Request Type")]
        public Nullable<short> REQUESTTYPE { get; set; }

        [DisplayName("VPF Contribution(%)")]
        [Required(ErrorMessage ="Contribution is Required")]
        [Range(typeof(int), "6", "100", ErrorMessage = "Contribution should be between 6% and 100%")]
        public Nullable<int> VPFCONTRIBUTION { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [Required(ErrorMessage = "Effective Date is Required")]
        [DisplayName("Effective Date")]
        public string EFFECTIVEDATE { get; set; }

        [DisplayName("Cancel Remarks")]
        [Required(ErrorMessage = "Remarks is Required")]
        public string CANCELREMARKS { get; set; }

        [DisplayName("Status")]
        public Nullable<short> STATUS { get; set; }
        public Nullable<System.DateTime> DATEADDED { get; set; }
        public Nullable<System.DateTime> MODIFIEDDATE { get; set; }
        public Nullable<long> MODIFIEDBY { get; set; }


        [Required(ErrorMessage = "Status is Required")]
        [DisplayName("Approval Status")]
        public Nullable<long> APPROVESTATUS { get; set; }
        public Nullable<long> APPROVALAUTHID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode= true)]
        [DisplayName("Approval Date")]
        public Nullable<System.DateTime> APPROVALDATE { get; set; }


        [Required(ErrorMessage = "Remarks is Required")]
        [DisplayName("Approval Remarks")]
        public string APPROVALREMARKS { get; set; }

        public string KI { get; set; }

        [DisplayName("Basic Salary")]
        public string BASICSALARY { get; set; }

        [DisplayName("Current VPF")]
        public string CURRENTVPF { get; set; }

        [DisplayName("Employee Name")]
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }

        public string ErrorMsg { get; set; }
        public string ErrorId { get; set; }

        public List<VPF_CreateDetailViewModel> VPF_List { get; set; }

        public virtual SearchViewModel SearchViewModel { get; set; }
    }

    public class VPF_ViewModel
    {
        public int ID { get; set; }
        public Nullable<int> EMPLOYEECODE { get; set; }
        public Nullable<byte> REQUESTTYPE { get; set; }
        public Nullable<int> VPFCONTRIBUTION { get; set; }
        public Nullable<System.DateTime> EFFECTIVEDATE { get; set; }
        public string CANCELREMARKS { get; set; }
        public Nullable<byte> STATUS { get; set; }
        public Nullable<System.DateTime> DATEADDED { get; set; }
        public Nullable<System.DateTime> MODIFIEDDATE { get; set; }
        public Nullable<long> MODIFIEDBY { get; set; }
        public Nullable<byte> APPROVESTATUS { get; set; }
        public Nullable<byte> APPROVALAUTHID { get; set; }
        public Nullable<System.DateTime> APPROVALDATE { get; set; }
    }

}
