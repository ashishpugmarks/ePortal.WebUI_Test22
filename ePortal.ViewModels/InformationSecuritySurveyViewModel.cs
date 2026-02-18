using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ePortal.ViewModels
{
    public class InformationSecuritySurveyViewModel
    {
        public string SurveyHeading { get; set; }
        public string SurveyHeadingHindi { get; set; }
        public string SurveyHeadingKannada { get; set; }
        public string SurveyHeadingGujarati { get; set; }
        public string SurveyHeadingJapanese { get; set; }
        public string SurveyDescription { get; set; }
        public string SurveyDescriptionHindi { get; set; }
        public string SurveyDescriptionKannada { get; set; }
        public string SurveyDescriptionGujarati { get; set; }
        public string SurveyDescriptionJapanese { get; set; }
        public string SURVEYRESULTMAIL { get; set; }
        public string SURVEYACKMAIL { get; set; }
        public string FEEDBACKSTATUS { get; set; }
        public string ISREMARKSMANDATORY { get; set; }
        public string SURVEYTYPE { get; set; }
        public string skipcount { get; set; }
        public List<Question> Questions { get; set; }
        public string SurveyNumber { get; set; }
        public bool ISFAMILYDECREQ { get; set; }
    }
    public class Question
    {
        public int Id { get; set; }
        [AllowHtml]
        public string Description { get; set; }
        public string DescriptionHindi { get; set; }
        public string DescriptionKannada { get; set; }
        public string DescriptionGujarati { get; set; }
        public string DescriptionJapanese { get; set; }
        public string OptionType { get; set; }
        public List<AnsOptions> Options { get; set; }
        public string SelectedOption { get; set; }
        public List<string> SelectedOptions { get; set; }
        public int IsSubQuestion { get; set; }
        public int SubQuestionParentId { get; set; }
        public string TriggerOptionIds { get; set; }
        public string IsVisible { get; set; }

    }
    public class AnsOptions
    {
        [AllowHtml]
        public string ANSDESCRIPTION { get; set; }
        public string ANSDESCRIPTIONHINDI { get; set; }
        public string ANSDESCRIPTIONKANNADA { get; set; }
        public string ANSDESCRIPTIONGUJARATI { get; set; }
        public string ANSDESCRIPTIONJAPANESE { get; set; }
        public string HRSECURITYANSLISTID { get; set; }
        public bool ISFAMILYDECREQ { get; set; }
    }

}
