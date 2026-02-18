using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.Training
{
    public class EvaluationListViewModel
    {

        public long Hrtraingdetailid { get; set; } = 0;
        public string EMPCODE { get; set; } = string.Empty;
        public string ENAME { get; set; } = string.Empty ;
        public string TRAINING { get; set; } = string.Empty;
        public string TRAININGPERIOD { get; set; } = string.Empty;
        public string TRAININGTIME { get; set; } = string.Empty;
        public string TRAININGVENUE { get; set; } = string.Empty;
        public long TRANSID { get; set; } = long.MinValue;
        public bool ISEVALUATIONFILLED { get; set; } = false;

    }
}
