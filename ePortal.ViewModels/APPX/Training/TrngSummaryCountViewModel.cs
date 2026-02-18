using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.Training
{
    public class TrngSummaryCountViewModel
    {
        public long Id { get; set; } = 0;
        public string Training { get; set; } = string.Empty;
        public int Scheduled { get; set; }  = 0;
        public int Attended { get; set; }= 0;

    }
}
