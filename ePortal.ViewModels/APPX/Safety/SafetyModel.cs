using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.Safety
{
    public class SafetyModel
    {
        public PlantViewModel PlantViewModel { get; set; }
        public SfCategoryViewModel SfCategoryViewModel { get; set; }
        public ActViewModel ActViewModel { get; set; }
        public SfSectionViewModel SfSectionViewModel { get; set; }
        public List<ContractGridViewModel> ContractGridViewModel { get; set; }
        public SearchContractsViewModel SearchContractsViewModel { get; set; }
        public List<ReminderDetail> ReminderDetail { get; set; }
        public List<PendingViewModel> PendingViewModel { get; set; } = new();
        public List<ChartItem> ChartData { get; set; } = new();
        public List<DashboardDetail> DashboardDetail { get; set; }
    }
    public class PlantViewModel
    {
        public string SelectedPlantId { get; set; }
        public List<SelectListItem> Plants { get; set; }
        public bool Enabled { get; set; }
    }
    public class SfCategoryViewModel
    {
        public string SelectedCategoryId { get; set; }
        public List<SelectListItem> Categories { get; set; }
    }
    public class ActViewModel
    {
        public string SelectedActId { get; set; }
        public List<SelectListItem> Acts { get; set; }
    }
    public class SfSectionViewModel
    {
        public string SelectedSectionId { get; set; }
        public List<SelectListItem> Sections { get; set; }
    }
    public class ContractGridViewModel
    {
        public string SFContracts { get; set; }
        public string CategoryDes { get; set; }
        public string ActDes { get; set; }
        public string Requirement { get; set; }
        public string SFSection { get; set; }
        public string RespName { get; set; }
        public string FrqDes { get; set; }
        public string ExpiryDate { get; set; }
        public string DueDate { get; set; }
        public string CmpStatus { get; set; }
        public string Remark { get; set; }
        public string Attachment { get; set; }
        public string AttachFileSubmit { get; set; }
        public string EncryptedId { get; set; }
    }
    public class SearchContractsViewModel
    {
        public string Category { get; set; }
        public string Lact { get; set; }
        public string Cmbstatus { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string SfSection { get; set; }
        public string Plant { get; set; }

    }
    public class ReminderDetail
    {
        public string Remarks { get; set; }
        public string Status { get; set; }
        public string AddedBy { get; set; }
        public string AddedDate { get; set; }
    }
    public class PendingViewModel
    {
        public string Status { get; set; }
        public string Count { get; set; }
    }
    public class ChartItem 
    { 
      public string Status { get; set; } 
      public int Count { get; set; } 
    }
    public class DashboardDetail
    {          
        public string CategoryDes { get; set; }   
        public string ActDes { get; set; }    
        public string Requirement { get; set; }   
        public string ExpiryDate { get; set; } 
        public string ResponsiblePerson { get; set; } 
        public string RemStatus { get; set; }  
    }

}
