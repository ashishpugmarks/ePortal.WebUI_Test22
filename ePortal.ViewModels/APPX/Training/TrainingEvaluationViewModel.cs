using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.Training
{
    public class TrainingEvaluationViewModel
    {
        public bool IsBtnSubbmitVisible { get; set; } = true;
        public bool IsbtnResetVisible { get; set; } = true;
        public bool IsgridEvalutionVisible { get; set; } = true;
        public string Redirection { get; set; } = string.Empty;
        public bool IsANSWERGRIDVisible { get; set; } = true;
        //public string Trainer1 { get; set; } = string.Empty;
        //public string Trainer2 { get; set; } = string.Empty;
        public TrainningDetailsViewModel TrngDetails { get; set; } = new TrainningDetailsViewModel();
        public List<TrainingQuetionAnsViewModel> gridEvalution { get; set; } = new List<TrainingQuetionAnsViewModel>();
        public List<TrainingQuetionAnsViewModel> ANSWERGRID { get; set; } = new List<TrainingQuetionAnsViewModel>();
    }
}
