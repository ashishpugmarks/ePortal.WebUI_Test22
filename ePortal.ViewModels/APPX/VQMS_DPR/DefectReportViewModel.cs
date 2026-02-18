using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.VQMS_DPR
{
    public class DefectReportViewModel
    {
        public List<DefectRowViewModel> Rows { get; set; } = new();
        public decimal TotalShiftA { get; set; }
        public decimal TotalShiftB { get; set; }
        public decimal GrandTotal { get; set; }
    }
    public class DefectRowViewModel
    {
        public string DefectDescription { get; set; }
        public string Category { get; set; }
        public string Section { get; set; }
        public decimal AShift { get; set; }
        public decimal BShift { get; set; }
        public decimal Total { get; set; }
    }

    
}

