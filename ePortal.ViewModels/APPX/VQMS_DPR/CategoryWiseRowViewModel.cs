using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.VQMS_DPR
{
    public class CategoryWiseRowViewModel
    {
        public string Category { get; set; }
        public decimal AShift { get; set; }
        public decimal BShift { get; set; }
        public decimal TotalDefects { get; set; }
    }

    public class CategoryWiseReportViewModel
    {
        public List<CategoryWiseRowViewModel> Rows { get; set; } = new();
        public decimal TotalShiftA { get; set; }
        public decimal TotalShiftB { get; set; }
        public decimal GrandTotal { get; set; }
    }
}
