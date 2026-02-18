using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.Training
{
    public class InternalTrngViewModel
    {
        public string EMPNAME { get; set; } = string.Empty;
        public string SECTION { get; set; } = string.Empty;
        public string VENUE { get; set; } = string.Empty;
        public string TRAININGPERIOD { get; set; } = string.Empty;
        public string TRAININGTIME { get; set; } = string.Empty;
        public string HRTRAININGID { get; set; } = string.Empty;
        public int HRTRAININGSTART { get; set; }
    }
}
