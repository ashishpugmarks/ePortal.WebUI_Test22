using Microsoft.AspNetCore.Mvc.Rendering;

namespace ePortal.ViewModels.APPX
{
    public class AppEmpSearchViewModel
    {
        public string EmpCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int OperationId { get; set; }
        public int DivisionId { get; set; }
        public int DepartmentId { get; set; }
        public int SectionId { get; set; }        
        
        // Dropdown data
        public IEnumerable<SelectListItem> Operations { get; set; }
        public IEnumerable<SelectListItem> Divisions { get; set; }
        public IEnumerable<SelectListItem> Departments { get; set; }
        public IEnumerable<SelectListItem> Sections { get; set; }       

        
        // For displaying tables
        public List<AppEmpSummaryItem> SummaryList { get; set; }        

    }

    public class AppEmpSummaryItem
    {
        public string Ecode { get; set; }
        public string Name { get; set; }
        public string Operation { get; set; }
        public string Designation { get; set; }
        public string DOB { get; set; }
        public string DOJ { get; set; }
        public string Location { get; set; }
    }

    public class EmpSearchViewModel
    {
        public string EmpCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BloodGroup { get; set; }
        public string EmailId { get; set; }
        public int OperationId { get; set; }
        public int DivisionId { get; set; }
        public int DepartmentId { get; set; }
        public int SectionId { get; set; }
        public int DesigantionId { get; set; }
        public int FunDesigantionId { get; set; }

        // Dropdown data
        public IEnumerable<SelectListItem> Operations { get; set; }
        public IEnumerable<SelectListItem> Divisions { get; set; }
        public IEnumerable<SelectListItem> Departments { get; set; }
        public IEnumerable<SelectListItem> Sections { get; set; }
        public IEnumerable<SelectListItem> Desigantions { get; set; }
        public IEnumerable<SelectListItem> FunDesigantions { get; set; }


        // For displaying tables
        public List<EmpSummaryItem> SummaryList { get; set; }

    }

    public class EmpSummaryItem
    {
        public string Ecode { get; set; }
        public string Name { get; set; }
        public string Operation { get; set; }
        public string Designation { get; set; }
        public string Mobile { get; set; }
        public string EmailId { get; set; }
        public string BloodGroup { get; set; }
        public string FunDesignation { get; set; }
      
    }
}
