using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.VQMS_DPR
{
    public class FinalInspectionDefectViewModel
    {
        public ReportHeaderViewModel ReportHeader { get; set; } = new();
        public DefectReportViewModel DefectWiseReport { get; set; } = new();
        public SectionWiseReportViewModel SectionWiseReport { get; set; } = new();
        public CategoryWiseReportViewModel CategoryWiseReport { get; set; } = new();
        public List<SectionWiseGraphRow> SectionWiseGraph { get; set; } = new();
        public List<CategoryWiseGraphRow> CategoryWiseGraph { get; set; } = new();
    }
    public class SectionWiseGraphRow
    {
        public string Section { get; set; }
        public int TotalDefects { get; set; }
        public decimal CM { get; set; }
    }
    public class CategoryWiseGraphRow
    {
        public string Category { get; set; }
        public int TotalDefects { get; set; }
        public decimal CM
        {
            get; set;
        }
    }
}
