using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{

    public class ThirdpartyEmployeeViewModel
    {        
        public long? ID { get; set; }
        public long? Ecode { get; set; }
        public string? AssociateName { get; set; }
        public string? Designation { get; set; }
        public string? CompanyName { get; set; }
        public string? ProjectManagerName { get; set; }
        public string? AccoountManagerName { get; set; }
        public string? AssociateCompanyId { get; set; }
        public string? ProjectManagerMailID { get; set; }
        public string? AccountManagerMailID { get; set; }
        public long? CreatedBy { get; set; }
        public System.DateTime? Created_Date { get; set; }
        public long? Modified_By { get; set; }
        public System.DateTime? Modified_Date { get; set; }
        public long? Ownership { get; set; }

        public string? Operation { get; set; }
        public string? Division { get; set; }
        public string? Department { get; set; }
        public string? Section { get; set; }
        public long? Is_Deboarded { get; set; }
    }

    //Added by TTL SR94104 - CR6022
    public class ThirdpartyEmpUploadModel : ThirdpartyEmployeeViewModel
    {
        public string OwnerEmpCode { get; set; }
        public string OwnerEmpName { get; set; }
        public int ErrorFlag { get; set; }
    }
    public class TableDataModel
    {
        public List<string[]> TableData { get; set; }
    }
    // End by TTL SR94104 - CR6022

    public class SelectOwnShipData
    {

        public string Text { get; set; }

        public long Value { get; set; }
    }
}
