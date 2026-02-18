using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.VQMS_DPR
{
    public class SectionWiseRowViewModel
    {
        public string Section { get; set; }
        public decimal AShift { get; set; }
        public decimal BShift { get; set; }
        public decimal TotalDefects { get; set; }
        public decimal Dpv { get; set; }   // Defects per 1000 Vehicles
    }

    public class SectionWiseReportViewModel
    {
        public List<SectionWiseRowViewModel> Rows { get; set; } = new();
        public decimal TotalShiftA { get; set; }
        public decimal TotalShiftB { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal TotalDpv { get; set; }
        public string lblTotalDPV { get; set; }
    }
}
