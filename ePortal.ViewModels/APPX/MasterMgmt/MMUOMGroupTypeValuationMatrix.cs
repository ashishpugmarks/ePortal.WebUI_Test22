using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ePortal.ViewModels.APPX.MasterMgmt
{
    public class MMUOMGroupTypeValuationMatrix
    {
        public bool UomPanelVisible { get; set; }
        public bool GroupPanelVisible { get; set; }
        public bool TypePanelVisible { get; set; }
        public bool ValuationPanelVisible { get; set; }
        public bool SitePanelVisible { get; set; }
        public bool SubmitPanelVisible { get; set; }
        public string BtnSubmitText { get; set; }
        public bool DdlPlantMstEnabled { get; set; }
        public bool TrPlantNameVisible { get; set; }
        public bool TrSalesVisible { get; set; }
        public string SelectedPlant { get; set; } 
        public string HeadSalesText { get; set; }
        public List<SelectListItem> PlantList { get; set; } 
        public string SelectedPlantSL { get; set; } 
        public List<SelectListItem> PlantSLList { get; set; } 
        public string UOM { get; set; }
        public string UOMDesc { get; set; }
        public int SelectedMasterEntry { get; set; }
       
        public ColumnDetails Col1 { get; set; } = new ColumnDetails { Header = "Type", IsVisible = true };
        public ColumnDetails Col2 { get; set; } = new ColumnDetails { Header = "Description", IsVisible = true };
        public ColumnDetails Col3 { get; set; } = new ColumnDetails { Header = "Profit Center", IsVisible = true };
        public ColumnDetails Col4 { get; set; } = new ColumnDetails { Header = "SITENAME", IsVisible = true };
        public ColumnDetails Col5 { get; set; } = new ColumnDetails { Header = "SITEDESC", IsVisible = true };
        public ColumnDetails Col6 { get; set; } = new ColumnDetails { Header = "Edit", IsVisible = true };
        public ColumnDetails Col7 { get; set; } = new ColumnDetails { Header = "Delete", IsVisible = true };
        public List<MaterialGroupDetailsViewModel>? MaterialGroupRecords { get; set; }

    }
    public class SelectListItem
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }
   public class MaterialGroupDetailsViewModel
    {
        public string TYPE { get; set; }
        public string DESC { get; set; }
        public string Code { get; set; }
        public string SITENAME { get; set; }
        public string SITEDESC { get; set; }
        public int TYPEID { get; set; }
    }
    public class MaterialMatrixRow
    {
        public string Col0 { get; set; }
        public string Col1 { get; set; }
        public string Col2 { get; set; }
        public string Col3 { get; set; }
        public string Col4 { get; set; }
    }
    public class ColumnDetails
    {
        public string? Header { get; set; }
        public bool? IsVisible { get; set; } = true;
    }

}
