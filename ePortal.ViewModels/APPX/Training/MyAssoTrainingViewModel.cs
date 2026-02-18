using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.Training
{
    public class MyAssoTrainingViewModel
    {
        public string OperationId { get; set; } = string.Empty;
        public string OperationText { get; set; } = string.Empty;
        public IEnumerable<SelectListItem> Divisions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Departments { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Sections { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Associates { get; set; } = new List<SelectListItem>();

        public string SecCode { get; set; } = "0";
        public string DeptCode { get; set; } = "0";
        public string DivCode { get; set; } = "0";
        public string OPCode { get; set; } = "0";
        public string Desig { get; set; } = string.Empty;
        public string AssociateId { get; set; } = string.Empty;

        public List<TodaysTrainingViewModel>? TodaysTraining { get; set; }
        public List<TrngSummaryCountViewModel>? SummaryCount { get; set; }
        public List<ExternalTrngViewModel>? ExternalTrng { get; set; }
        public List<InternalTrngViewModel>? InternalTrng { get; set; }
    }
}
