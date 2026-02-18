using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.Training
{
    public class TrainingQuetionAnsViewModel
    {
        public int HRTRAININGQUEID { get; set; } = 0;
        public int TRAININGID { get; set; } = 0;
        public string QUESTION { get; set; } = string.Empty;
        public int ACTIVE { get; set; } = 0;
        public string SCORE1 { get; set; } = string.Empty;
    }
}
