using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ePortal.ViewModels
{
    public class EmployeeViewModel
    {
        [Required, Range(0, int.MaxValue, ErrorMessage = "This is compulsory")]
        public int EmpId { get; set; }

        [MaxLength(20), DataType(DataType.Text)]
        public string Name { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid number for Age")]
        public int Age { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid number for Department Id")]
        public int DepartmentId { get; set; }
    }

    public class ADEmployeeViewModel
    {
        public long EmpId { get; set; }

        public string EmpName { get; set; }

        public string Dob { get; set; }

        public string Email { get; set; }

        public string Gender { get; set; }
    }

    public class EmployeeDetailViewModel
    {
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string Designation { get; set; }
        public string JobTitle { get; set; }
        public string DOB { get; set; }
        public string Operation { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string FunctionalDesignation { get; set; }
        public string Site { get; set; }
        public string Location { get; set; }
        public string Mobile { get; set; }
        public string Extension { get; set; }
        public string DirectLine { get; set; }
        public string BloodGroup { get; set; }
        public string Email { get; set; }
        public string SeatNo { get; set; }
        public string ImageUrl { get; set; }
    }
}
