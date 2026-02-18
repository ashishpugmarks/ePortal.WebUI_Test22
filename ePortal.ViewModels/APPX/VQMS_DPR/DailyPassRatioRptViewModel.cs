using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.VQMS_DPR
{
    public class DailyPassRatioRptViewModel
    {
        public bool IsChar03Visible { get; set; } = true;
        public bool IsChar04Visible { get; set; } = true ;
        public List<Result> DPRCnt01 { get; set; } = new List<Result>();
        public List<Result> DPRPer01 { get; set; } = new List<Result>();
        public List<Result> Sectionwise01 { get; set; } = new List<Result>();
        public List<Result> DPRCnt02 { get; set; } = new List<Result>();
        public List<Result> DPRPer02 { get; set; } = new List<Result>();
        public List<Result> Sectionwise02 { get; set; } = new List<Result>(); 
        public List<Result> DPRCnt03 { get; set; } = new List<Result>();
        public List<Result> DPRPer03 { get; set; } = new List<Result>();
        public List<Result> Sectionwise03 { get; set; } = new List<Result>();
        public List<Result> DPRCnt04 { get; set; } = new List<Result>();
        public List<Result> DPRPer04 { get; set; } = new List<Result>();
        public List<Result> Sectionwise04 { get; set; } = new List<Result>();
    }
    public class Result{
        public string PASS_TYPE { get; set; }
        public int CNT { get; set; } = 0;
        public decimal PER { get; set; } = 0;
        public string DEFECTSECTION { get; set; }
        public decimal CM { get; set; }
    }
}
