using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels.APPX.CustomerMgmt
{
    public class CMApprovalAuthority
    {
        public long MASTERDATAAPPROVALID { get; set; }
        public string? REQUEST_TYPE { get; set; }
        public string? AUTHORIZATIONTYPE { get; set; }
        public string? EMPNAME { get; set; }
        public long? CREATEDBY { get; set; }
        public string? CREATIONDATE { get; set; }
        public string? MODIFICATIONDATE { get; set; }
        public long? MODIFIEDBY { get; set; }
        public short ACTIVE { get; set; }
    }
    public class CustomerReqApproverDetails
    {
        public string INITIATOR { get; set; }
        public string GEN_REQUEST_NO { get; set; }
        public string Recommendation { get; set; }
        public string? APPROVAL { get; set; }
        public string? FINANCE { get; set; }
        public string? FINANCE2 { get; set; }
        public string? SEC_ACTION { get; set; }
        public string? DEPT_ACTION { get; set; }
        public string? FIN_ACTION { get; set; }
        public string? FIN1_ACTION { get; set; }
        public string? REQUEST_TYPE { get; set; }
        public string? DETAIL_STATUS { get; set; }
        public string? REQUESTERID { get; set; }
        public string? Status { get; set; }
        public string? REQUESTDATE { get; set; }
    }
    public class SwitchApprovalAuthority
    {
        public long REF_ID { get; set; }
        public string? CM_REQUEST_NO { get; set; }
        public string? PRE_APPROVER { get; set; }
        public string? NEW_APPROVER { get; set; }
        public string? PRE_APPROVER1 { get; set; }
        public string? NEW_APPROVER1 { get; set; }
        public string? APPR_ROLE { get; set; }
        public string? CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string? REMARKS { get; set; }
        public string? ATTACHED_DOC { get; set; }
        public string? APPROVER_ROLE { get; set; }
    }
    public class CMApproverMatrx
    {
        public string? ActionType { get; set; }
        public string? FinSwitchAppr { get; set; }
        public long? REF_ID { get; set; }
        public string? MasterType {  get; set; }
        public string? RequestType { get;set; }
        public long? AuthorizationAuth {  get; set; }
        public short? AuthActive {  get; set; }
        public string? CustomerRequestNo {  get; set; }
        public string? ApproverType { get;set;}
        public long? CurrentApprover { get; set; }
        public long? NewApproverDate { get; set; }
        public string? Remarks {  get; set; }
        public long? CreatedBy { get; set; }
        public string? CreatedDate { get;set; }
        public long? ModifyBy { get; set; }
        public string? ModifyDate { get; set; }
        public IFormFile? UploadFp { get; set; }

    }

}
