using System;
using System.Collections.Generic;

namespace ePortal.ViewModels.APPX.Separation
{
    public class ExitInterviewQuestionnaireModel
    {
        public List<ExitIntQuestModel> ExitIntQuestModel { get; set; } = new();
        public List<ExitIntAnsModel> ExitIntAnsModel { get; set; } = new();
        public EmpDetailsIntQuestModel EmpDetailsIntQuestModel { get; set; }
    }

    public class ExitIntQuestModel
    {
        public string SectionId { get; set; }
        public string SectionTitle { get; set; }
        public List<IntQuestModel> Questions { get; set; } = new();
    }

    public class IntQuestModel
    {
        public string ExitInterviewQusId { get; set; }
        public string QusType { get; set; } // "1" = Text, "2" = Radio, "3" = Checklist
        public string SectionId { get; set; }
        public string QusDesc { get; set; }
        public List<OptionIntQuestModel> Options { get; set; } = new();
    }

    public class OptionIntQuestModel
    {
        public string ExitInterviewOptionId { get; set; }
        public string OptionDesc { get; set; }
    }

    public class EmpDetailsIntQuestModel
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

    public class ExitIntAnsModel
    {
        public string SectionId { get; set; }
        public string SectionTitle { get; set; }
        public List<IntAnsModel> Questions { get; set; } = new();
    }

    public class IntAnsModel
    {
        public string ExitInterviewQusId { get; set; }
        public string QusType { get; set; } // "1" = Text, "2" = Radio, "3" = Checklist
        public string TextAns { get; set; }
        public string SelectedOption { get; set; }
        public List<string> SelectedOptions { get; set; } = new();
    }
}
