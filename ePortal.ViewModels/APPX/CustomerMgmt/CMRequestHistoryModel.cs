using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.CustomerMgmt
{
    public class CMRequestHistoryModel
    {
         
            public string? CMHeaderId { get; set; }           
            public string UserId { get; set; }          
            public string? EName { get; set; }           
            public string? Role { get; set; }            
            public string? Action { get; set; }            
            public string? ActionDate { get; set; }
            public string? ActionRemarks { get; set; }          
            public int? SlrNo { get; set; }     

}
    public class CMRequestHistoryDetail
    {
        public string GenRequestNo { get; set; } 
        public string RequesterDetail { get; set; }      
        public string? RequestDate { get; set; }       
        public string SectionDetail { get; set; } 
        public string SectionStatus { get; set; } 
        public string? SectionRemarks { get; set; }
        public string? SectionApproveDate { get; set; }
        public string DeptDetail { get; set; } 
        public string DeptStatus { get; set; } 
        public string? DeptRemarks { get; set; }
        public string? DeptApproveDate { get; set; }
        public string FinDetail { get; set; } = string.Empty;
        public string FinStatus { get; set; } = string.Empty;
        public string? FinanceRemarks { get; set; }
        public string? FinDetail2 { get; set; }
        public string? FinStatus2 { get; set; }
        public string? FinanceRemarks2 { get; set; }
        public List<CMRequestHistoryModel>? Historymodle { get; set; }
    }

}
