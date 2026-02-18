using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.Separation
{
    public class ViewHistoryModel
    {
        public AssociatedetailsViewModel AssociatedetailsViewModel { get; set; }     
        public ViewHistoryParentModel ViewHistoryParentModel { get; set; }
        public List<ApprovalDetailsModel> ApprovalDetailsModel { get; set; }
    }
    public class AssociatedetailsViewModel
    {
        public string lbl_assname { get; set; }
        public string lbl_assecode { get; set; }
        public string lbl_joindate { get; set; }
        public string lbl_desg { get; set; }
        public string lbl_fundesg { get; set; }
        public string lbl_phone { get; set; }
        public string lbl_emailid { get; set; }
        public string lbl_op { get; set; }
        public string lbl_div { get; set; }
        public string lbl_dept { get; set; }
        public string lbl_sec { get; set; }
        public string lbl_site { get; set; }
        public string lbl_applydate { get; set; }
        public string lbl_reldate { get; set; }
        public string lbl_deptreldate { get; set; }
        public string lbl_subject { get; set; }
        public string lbl_reason { get; set; }
        public string RESIG_PROCESSSTATUS { get; set; }
    }
    public class ViewHistoryParentModel
    {
        public bool ShowPendClrForm { get; set; } = false;
        public bool ShowManageClrHeader { get; set; } = false;
        public bool ShowDeptClrForm { get; set; } = false;
        public List<PendingClearanceFormViewModel> PendingClearanceFormViewModel { get; set; }
        public List<ManageClearanceHeaderViewModel> ManageClearanceHeaderViewModel { get; set; }
        public DeptClearanceFormViewModel DeptClearanceFormViewModel { get; set; }
    }
    public class PendingClearanceFormViewModel
    {
        public int ClearanceHeaderId { get; set; }  
        public int ClearDetailId { get; set; }     
        public decimal Amount { get; set; }      
        public string Remarks { get; set; }        
        public string Status { get; set; }       
        public string Description { get; set; }     
        public string AttachDocument { get; set; }  
    }
    public class ManageClearanceHeaderViewModel
    {
        public int ClearanceSubHeaderId { get; set; }    
        public int ClearanceHeaderId { get; set; }          
        public string SubHeaderDescription { get; set; }    
        public decimal Amount { get; set; }            
        public string Remarks { get; set; }              
        public string Status { get; set; }             
        public string ESubmitBy { get; set; }             
        public string SubmitBy { get; set; }               
        public string SubmittedDate { get; set; }       
        public string SubmittedDate1 { get; set; }        
        public int ClearDetailId { get; set; }         
        public string ResignationId { get; set; }        
        public string ClearanceHeaderId1 { get; set; }
        public string HeaderDesc { get; set; }    
        public string HeadCode { get; set; }              
        public string HsubDate { get; set; }      
        public string HRRemarks { get; set; }           
        public int ClearanceHeaderId12 { get; set; }     
        public string HsStatus { get; set; }        
        public string AttachDocument { get; set; }
        public List<ManageClearanceHeaderChildViewModel> HeaderDetailsChildModel { get; set; }
    }
    public class ManageClearanceHeaderChildViewModel
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public string SubmitBy { get; set; }
        public string SubmitDate { get; set; }
    }
    public class DeptClearanceFormViewModel
    {
        public string lbl_confdoc { get; set; }
        public string lbl_otherdoc { get; set; }
        public string lbl_book { get; set; }
        public string lbl_itasset { get; set; }
        public string lbl_camera { get; set; }
        public string lbl_key { get; set; }
        public string lbl_travel { get; set; }
    }
    public class ApprovalDetailsModel
    {
        public string AppName { get; set; }          
        public string AppECode { get; set; }        
        public string AppRemark { get; set; }       
        public int AppStatus { get; set; }        
        public string Status { get; set; }           
        public string AppSubmittedDate { get; set; } 
        public string Designation { get; set; }  
        public int OrderNo { get; set; }
    }
}
