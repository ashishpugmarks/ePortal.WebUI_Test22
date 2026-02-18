using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.Separation
{
    public class MyResignationRequestModel
    {
        public ResignationStatusViewModel ResignationStatusModel { get; set; }
        public HRSPResigDetailViewModel HRSPResigDetailModel { get; set; }
        public AssociateHeaderViewModel AssociateHeaderData { get; set; }
        public HeaderClearanceViewModel HeaderClearanceData { get; set; }
        public ExitInterviewQuestionnaireViewModel InterviewQuestionsData { get; set; }
    }
    public class ResignationStatusViewModel
    {
        public string hd_Resign { get; set; }
        public string lbldptstatus { get; set; }
        public string lbldptstatusColor { get; set; }
        public string lbldivstatus { get; set; }
        public string lbldivstatusColor { get; set; }
        public string lblopstatus { get; set; }
        public string lblopstatusColor { get; set; }
        public string lbldirstatus { get; set; }
        public string lbldirstatusColor { get; set; }
        public string lblhrstatus { get; set; }
        public string lblhrstatusColor { get; set; }
    }
    public class HRSPResigDetailViewModel
    {
        public string lblhrname { get; set; }
        public string lblhrdesg { get; set; }
        public string exitinterview { get; set; }
        public bool ShowExitInterview { get; set; }
        public string uimghrUrl { get; set; }
        public bool Showuimghr { get; set; }
        public string lblacptstaus { get; set; }
        public string lblacptstausColor { get; set; }
        public string lblacptname { get; set; }
        public string lblacptdesg { get; set; }
        public string lblacptnameColor { get; set; }
        public string uimgacptUrl { get; set; }
        public bool Showuimgacpt { get; set; }
        public bool ShowLinkEdit { get; set; } = true;
        public bool ShowAnchWithdraw { get; set; } = true;
        public string lblassociatename { get; set; }
        public string lblassociatedesg { get; set; }
        public string uimgassociateUrl { get; set; }
        public bool Showuimgassociate { get; set; }
        public bool pnldpt { get; set; }
        public string lbldptname { get; set; }
        public string lbldptdesg { get; set; }
        public string uimgdptUrl { get; set; }
        public bool Showuimgdpt { get; set; }
        public bool pnldiv { get; set; }
        public string lbldivname { get; set; }
        public string lbldivdesg { get; set; }
        public string uimgdivUrl { get; set; }
        public bool Showuimgdiv { get; set; }
        public bool pnlop { get; set; }

        public string lblopname { get; set; }
        public string lblopdesg { get; set; }
        public string uimgopUrl { get; set; }
        public bool Showuimgop { get; set; }
        public bool pnldir { get; set; }
        public string lbldirname { get; set; }
        public string lbldirdesg { get; set; }
        public string uimgdirUrl { get; set; }
        public bool Showuimgdir { get; set; }
        public bool Appdetailsdiv { get; set; }
        public bool nodata { get; set; }
        public bool Resig_Dev { get; set; }

    }
    public class AssociateHeaderViewModel
    {
      public bool anchpendingass { get; set; }
      public bool anchapproveass { get; set; }
      public bool ALLPROCESSSTATUSDIV { get; set; }
    }
    public class HeaderClearanceViewModel
    {
        public List<ClearanceHeaderItem> rep_headerclr { get; set; } = new List<ClearanceHeaderItem>();
        public bool clrprocessdiv { get; set; }
        public bool formidiv { get; set; }
        public string forminamelbl { get; set; }
        public bool a1 { get; set; }
        public bool a2 { get; set; }
        public bool lnkformi { get; set; }
        public bool lnkformiview { get; set; }
    }
    public class ClearanceHeaderItem
    {
        public string DESCRIPTION { get; set; }
        public int pending { get; set; }
        public int complete { get; set; }
        public string STATUS { get; set; }
        public string RESIGNATIONID { get; set; }
        public string CLEARENCEHEADERID { get; set; }
    }
    public class ExitInterviewQuestionnaireViewModel
    {
        public string namelbl { get; set; }
        public string forminamelbl { get; set; }
        public string hd_resignid { get; set; }
        public bool anchorpendingatuser { get; set; }
        public bool anchorcompleted { get; set; }
        public bool HRDIV { get; set; }
        public bool aninterview { get; set; }
        public bool linkanswerview { get; set; }
        public bool exitinterques_div { get; set; }
        public bool anchpendinghr { get; set; }
        public bool anchcompleted { get; set; }
        public bool exitinterview { get; set; }
    }

}
