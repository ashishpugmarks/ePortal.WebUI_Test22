//------------------------------------------------------------------------------------------------------------------------------------------------------
//-----SR39197------------Change Start(Aumento)---------------------------------------------------
//--------------------------------------------------------------------------------------------------------------------------
using System.ComponentModel;

namespace ePortal.ViewModels
{
    public class UAR_AccessReviewViewModel
    {
        public int UARID { get; set; }
        [DisplayName("E.Code")]
        public long? ECODE { get; set; }
        [DisplayName("Employee")]
        public string EMPLOYEE { get; set; }
        [DisplayName("Operation")]
        public long OPERATIONID { get; set; }
        [DisplayName("Division")]
        public string DIVISIONID { get; set; }
        [DisplayName("Department")]
        public string DEPARTMENTID { get; set; }
        public long REVIEWER { get; set; }
        public string REVIEWERName { get; set; }
        public long REVIEWER1 { get; set; }
        public long REVIEWER2 { get; set; }
        public string LEVELDESC { get; set; }
        public string STATUS { get; set; }
        public string DIVISION { get; set; }
        public string DEPARTMENT { get; set; }
        public string USERTYPE { get; set; }
        public string ACCESSRIGHTS { get; set; }
        public string CREATIONDATE { get; set; }
        public string PARENTPATH { get; set; }
        public string SYUSERRIGHTSID { get; set; }
        public string Required { get; set; }
        public string EmailID { get; set; }
        public string SYKIID { get; set; }//Added for SR78412        
        public string ADDEDON { get; set; } //Added for SR92683 
        public string REVIEWED_ON { get; set; } //Added for SR92683 
        public string SELF_REVIEWED_ON { get; set; } //Added for SR92683 
    }
    public class UAR_ViewModel
    {
        public string ACCESSRIGHTS { get; set; }
        public string CREATIONDATE { get; set; }
        public string SYUSERRIGHTSID { get; set; }
        public string PARENTPATH { get; set; }
        public string Checked { get; set; }
        public string Isvisible { get; set; }
        public string ACCESSTYPE { get; set; } //Added for SR78412
    }

    //SR78412 Changes Start
    public class UAR_Regular_ViewModel
    {
        public string ACCESSRIGHTS { get; set; }
        public string CREATIONDATE { get; set; }
        public string SYUSERRIGHTSID { get; set; }
        public string PARENTPATH { get; set; }
        public string Checked { get; set; }
        public string Isvisible { get; set; }
        public string USERCHECKED { get; set; }
        public string ACCESSTYPE { get; set; }

    }
    public class ReviewRequestParameter
    {
        public string EmpCode { get; set; }
        public List<UAR_Regular_ViewModel> Data { get; set; }
        public string SelfRemarks { get; set; }
        public string REVIEWER { get; set; }
    }
    //SR78412 Changes End    
    public class SendBackRequestModel
    {
        public string EmpCode { get; set; }
        public string REVIEWER { get; set; }
    }
    public class ReviewRegularReviewerRequest { 
        public string EmpCode { get; set; } 
        public List<UAR_Regular_ViewModel> Data { get; set; } 
        public string Remarks { get; set; } 
    }
    public class ReviewRegularDataRequest
    {
        public string EmpCode { get; set; }
        public List<UAR_Regular_ViewModel> Data { get; set; }
        public string SelfRemarks { get; set; }
        public string REVIEWER { get; set; }
    }
    public class ReviewDataRequest
    {
        public string EmpCode { get; set; }
        public List<UAR_ViewModel> Data { get; set; }
    }
    public class DisableUserRequest
    {
        public string EmpCode { get; set; }
        public List<UAR_ViewModel> Data { get; set; }
    }


}
//------------------------------------------------------------------------------------------------------------------------------------------------------
//-----SR39197------------Change End(Aumento)---------------------------------------------------
//--------------------------------------------------------------------------------------------------------------------------
