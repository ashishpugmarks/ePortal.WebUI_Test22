using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.Separation
{
    public class ExitInterviewAnswersModel
    {
        public List<ExitInterviewQuestionModel> ExitInterviewQuestionModel { get; set; } = new();
        public List<ExitInterviewAnswerViewModel> ExitInterviewAnswerViewModel { get; set; } = new();
        public EmpDetailsModel EmpDetailsModel { get; set; }
    }
    public class ExitInterviewQuestionModel
    {
        public string SectionId { get; set; }
        public string SectionTitle { get; set; }
        public List<InterviewQuestionModel> Questions { get; set; } = new();
    }
    public class InterviewQuestionModel
    {
        public string ExitInterviewQusId { get; set; }
        public string QusType { get; set; } // "1" = Text, "2" = Radio, "3" = Checklist
        public string SectionId { get; set; }
        public string QusDesc { get; set; }
        public List<OptionModel> Options { get; set; } = new();
    }
    public class OptionModel
    {
        public string ExitInterviewOptionId { get; set; }
        public string OptionDesc { get; set; }
    }
    public class EmpDetailsModel
    {
        public string lbl_name { get; set; }
        public string lbl_ecode { get; set; }
        public string lbl_dept { get; set; }
        public string lbl_designation { get; set; }
        public string lbl_since { get; set; }
        public string lbl_loc { get; set; }
        public string lbl_supdesign { get; set; }
        public string lbl_supname { get; set; }
        public string lbl_doj { get; set; }
        public string lbl_lastdateemp { get; set; }
        public string lbl_EXINTDATE { get; set; }
        public string lbl_age { get; set; }
        public string lbl_qualification { get; set; }
    }
    public class ExitInterviewAnswerViewModel
    {
        public string SectionId { get; set; }
        public string SectionTitle { get; set; }
        public List<InterviewAnswerModel> Questions { get; set; } = new();
    }
    public class InterviewAnswerModel
    {
        public string ExitInterviewQusId { get; set; }
        public string QusType { get; set; } // "1" = Text, "2" = Radio, "3" = Checklist
        public string TextAns { get; set; }
        public string SelectedOption { get; set; }
        public List<string> SelectedOptions { get; set; } = new();
    }


}
