using ePortal.ViewModels;
using System;
using System.Collections.Generic;

namespace ePortal.Application.Contracts
{
    public interface IKaizenService
    {
        //List<KaizenViewModel> GetOrgLevelList(SearchKZViewModel obj,long empcode);
        //Tuple<string, string> GetKaizenData(string tmpid);
        //VM_D_TRAN GetApprovalKaizenData(string tmpid,string ecode);
        //short SaveKaizenMarks(KaizenMarks obj);
        //KaizenGrade Kaizenexist(string tmpid,long empcode);
        //KaizenGrade KaizenDeptCommexist(string tmpid, long empcode);
        //List<VM_Operation_UserList> DeptCommitteeKaizenUsers(long KaizenID);
        //string GetApproverData(string tmpid);
        //List<VM_EXCEL_DATA> excelexport(List<string> ids);
        //List<VM_EXCEL_DATA> Scoreexcelexport(List<string> ids);
        //List<KaizenViewModel> GetOperationLevelList(SearchKZViewModel obj, long empcode);
        //KaizenMarks EvalutorMarks(long empcode, long tmpid);
        //List<KaizenMarks> AllEvalutorMarks(long tmpid);
        //List<KaizenViewModel> Adminrights(long? opid);
        //List<VM_Operation_UserList> OperationUser(string OP_id);
        //List<KaizenViewModel> AdminArea(string opid);
        //VM_Operation_UserList EditDeptCommittee(string OP_id, long Committee_ID);
        //Tuple<short, string> SaveDeptCommittee(long CommitteeID, long user1, long user2, long user3, long user4, string Committee_name, long addedby, string OP_ID,int isUpdate);
        //string DeleteCommittee(long id, long empcode);
        ////string Adduser(long empcode, long opid,long usercode);
        //List<Employee_Details> PortalAutocompleteSuggestions(string Key);
        //List<KaizenViewModel> GetCrossCommiteeLevelList(SearchKZViewModel obj, long empcode);
        //string GetDivCommitteeKaizenData(string tmpid);
        //KaizenGrade DivCommitteeKaizenexist(string tmpid, long empcode);
        //List<SearchHRAdmin> PlantList();
        //List<SearchHRAdmin> HRAdminRightList(long syplantID, long empcode);
        //short DeleteHRAdminRight(long ID, long empcode);
        //short AddHRAdminUser(long syPlantId, long empcode, long addedby);
        bool isPopupEnableForDeptCommittee(long ecode);
        //List<VM_Operation_UserList> DeptCommitteeList(long empcode,string op_ID);
        //string SenbackDeptCommittee(KaizenMarks obj);
        //List<VM_DivCommitee_UserList> DivCommiteeUser(string OP_id);
        //Tuple<short, string> SaveDivCommittee(long CommitteeID, long user1, long user2, long user3, long user4, string Committee_name, long addedby, string OP_ID,int isUpdate);
        //VM_DivCommitee_UserList EditDivCommittee(string OP_id, long Committee_ID);
        //string DeleteDivCommittee(long id, long empcode);
        //DeptAvgMarks DeptCommitteeAvgMarks(string id);
        //short SaveDivKaizenMarks(KaizenMarks obj);
        //List<TopCoreSteeringKaizen> CoreCommitteeKaizen(long empcode,string period,long syki);
        //KaizenMarks DivEvalutorMarks(long empcode, long tmpid);
        //short SaveDivRank(DivRank obj);
        //List<KAIZEN_KIViewModel> GetKaizenKiLIST(long ecode);
        //List<KaizenLibrary_Model> KaizenLibraryData(SearchKZViewModel obj);
        //List<KaizenMarks> AllDivEvalutorMarks(long tmpid);
        //List<VM_DivCommitee_UserList> GetDivLevelList(SearchKZViewModel obj, long empcode);
        //List<VM_Operation_UserList> DivCommitteeKaizenUsers(string KaizenID);
        //Kaizen_Lirbary_VM KaizenLibraryForm(string KaizenID);
        //List<VM_Operation_UserList> DeptCommitteeKaizenUser_(long KaizenID,long empcode);
        bool isPopupEnableForDivCommittee(long ecode);
        }
}
