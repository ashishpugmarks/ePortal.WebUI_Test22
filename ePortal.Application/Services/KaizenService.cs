using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.Infrastructure.Repositories;
using ePortal.Shared;
using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Services
{
    public class KaizenService : IKaizenService
    {
        private readonly KaizenRepository _KZRepo;
        public KaizenService(KaizenRepository KZRepo)
        {
            _KZRepo = KZRepo;
        }
        //public List<KaizenViewModel> GetOrgLevelList(SearchKZViewModel obj, long empcode)
        //{
        //    return _KZRepo.GetOrgLevelList(obj, empcode);
        //}
        //public Tuple<string, string> GetKaizenData(string tmpid)
        //{
        //    return _KZRepo.GetKaizenData(tmpid);
        //}
        //public VM_D_TRAN GetApprovalKaizenData(string tmpid,string ecode)
        //{
        //    return _KZRepo.GetApprovalKaizenData(tmpid,ecode);
        //}
        //public short SaveKaizenMarks(KaizenMarks obj)
        //{
        //    return _KZRepo.SaveKaizenMarks(obj);
        //}
        //public KaizenGrade Kaizenexist(string tmpid,long empcode)
        //{
        //    return _KZRepo.Kaizenexist(tmpid,empcode);
        //}
        //public KaizenGrade KaizenDeptCommexist(string tmpid, long empcode)
        //{
        //    return _KZRepo.KaizenDeptCommexist(tmpid, empcode);
        //}
        //public List<VM_Operation_UserList> DeptCommitteeKaizenUsers(long KaizenID)
        //{
        //    return _KZRepo.DeptCommitteeKaizenUsers(KaizenID);
        //}
        //public string GetApproverData(string tmpid)
        //{
        //    return _KZRepo.GetApproverData(tmpid);
        //}
        //public List<VM_EXCEL_DATA> excelexport(List<string> ids)
        //{
        //    return _KZRepo.excelexport(ids);
        //}
        //public List<KaizenViewModel> GetOperationLevelList(SearchKZViewModel obj, long empcode)
        //{
        //    return _KZRepo.GetOperationLevelList(obj, empcode);
        //}
        //public KaizenMarks EvalutorMarks(long empcode, long tmpid)
        //{
        //    return _KZRepo.EvalutorMarks(empcode, tmpid);
        //}
        //public List<KaizenMarks> AllEvalutorMarks( long tmpid)
        //{
        //    return _KZRepo.AllEvalutorMarks(tmpid);
        //}
        //public List<KaizenViewModel> Adminrights(long? opid)
        //{
        //    return _KZRepo.Adminrights(opid);
        //}
        //public List<VM_Operation_UserList> OperationUser(string OP_id)
        //{
        //    return _KZRepo.GetOpUserList(OP_id);
        //}
        //public List<KaizenViewModel> AdminArea(string opid)
        //{
        //    return _KZRepo.AdminArea(opid);
        //}
        //public VM_Operation_UserList EditDeptCommittee(string OP_id,long Committee_ID)
        //{
        //    return _KZRepo.EditDeptCommittee(OP_id, Committee_ID);
        //}
        //public Tuple<short, string> SaveDeptCommittee(long CommitteeID, long user1, long user2, long user3, long user4, string Committee_name, long addedby, string OP_ID,int isUpdate)
        //{
        //    return _KZRepo.SaveDeptCommittee(CommitteeID, user1, user2, user3, user4, Committee_name, addedby, OP_ID, isUpdate);
        //}
        //public string DeleteCommittee(long id, long empcode)
        //{
        //    return _KZRepo.DeleteCommittee(id, empcode);
        //}
        ////public string Adduser(long empcode, long opid,long userid)
        ////{
        ////    return _KZRepo.Adduser(empcode, opid, userid);
        ////}
        //public List<Employee_Details> PortalAutocompleteSuggestions(string Key)
        //{
        //    return _KZRepo.PortalAutocompleteSuggestions(Key);
        //}

        //public List<KaizenViewModel> GetCrossCommiteeLevelList(SearchKZViewModel obj, long empcode)
        //{
        //    return _KZRepo.GetCrossCommiteeLevelList(obj, empcode);
        //}
        //public string GetDivCommitteeKaizenData(string tmpid)
        //{
        //    return _KZRepo.GetDivCommitteeKaizenData(tmpid);
        //}
        //public KaizenGrade DivCommitteeKaizenexist(string tmpid, long empcode)
        //{
        //    return _KZRepo.DivCommitteeKaizenexist(tmpid, empcode);
        //}

        //public List<SearchHRAdmin> PlantList()
        //{
        //    return _KZRepo.PlantList();
        //}
        //public List<SearchHRAdmin> HRAdminRightList(long syplantID, long empcode)
        //{
        //    return _KZRepo.HRAdminRightList(syplantID, empcode);
        //}
        //public short DeleteHRAdminRight(long ID, long empcode)
        //{
        //    return _KZRepo.DeleteHRAdminRight(ID, empcode);
        //}
        //public short AddHRAdminUser(long syPlantId, long empcode, long addedby)
        //{
        //    return _KZRepo.AddHRAdminUser(syPlantId, empcode, addedby);
        //}
        //public List<VM_Operation_UserList> DeptCommitteeList(long empcode, string opid)
        //{
        //    return _KZRepo.DeptCommitteeList(empcode, opid);
        //}
        public bool isPopupEnableForDeptCommittee(long ecode)
        {
            return _KZRepo.isPopupEnableForDeptCommittee(ecode);
        }
        //public List<VM_EXCEL_DATA> Scoreexcelexport(List<string> ids)
        //{
        //    return _KZRepo.Scoreexcelexport(ids);
        //}
        //public string SenbackDeptCommittee(KaizenMarks obj)
        //{
        //    string retval = _KZRepo.SenbackDeptCommittee(obj);
        //    if (retval == "1")
        //    {
        //        List<string> Mails = _KZRepo.MailEmployee(obj.KAIZENID,obj.ADDEDBY);
        //        EmailCore sendMail = new EmailCore();
        //        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
        //        sendMail.MailTo = Mails[0];
        //        string CC = "";
        //        for(var i = 1; i < Mails.Count-1; i++)
        //        {
        //            CC = CC + Mails[i]+",";
        //        }
        //        CC = CC.Substring(0, CC.Length - 1);
        //        sendMail.MailCc = CC;
        //        string strSubject = "Kaizen Form Status Sendback ";
        //        string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
        //                            "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Kaizen Form Status Sendback</td></tr>" +
        //                            "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
        //                            "<tr><td valign=top colspan =2>Your Kaizen form request had been <b>sent back</b> by Dept Head Committee member.</td></tr>" +
        //                            "<tr><td valign=top colspan =2>The request details are as follows:</td></tr>" +
        //                            "<tr><td width=125 height=22 valign=top>Request Number</td><td width=389 valign=top>" + obj.KAIZENID+ "</td></tr>" +
        //                            "<tr><td width=125 valign=top>Theme </td><td width=389 valign=top>" + obj.Theme+ "</td></tr>"+
        //                            "<tr><td width=125 valign=top>Sendback By </td><td width=389 valign=top>" + obj.ADDEDBY+"-"+Mails[Mails.Count-1]+ "</td></tr>" +
        //                            "<tr><td width=125 valign=top>Sendback Remark </td><td width=389 valign=top>" + obj.Remark+ "</td></tr>";

        //        strBody = strBody + "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "/Login/Index> Employee Portal</a> to view the approval history.</td></tr>" +
        //                            "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";
        //        sendMail.MailSubject = strSubject;
        //        sendMail.MailBody = strBody;
        //        try
        //        {
        //            bool status = sendMail.Send();
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //        finally
        //        {

        //        }
        //    }


        //    return retval;

        //}
        //public List<VM_DivCommitee_UserList> DivCommiteeUser(string OP_id)
        //{
        //    return _KZRepo.GetDivCommiteeUsers(OP_id);
        //}
        //public Tuple<short, string> SaveDivCommittee(long CommitteeID, long user1, long user2, long user3, long user4, string Committee_name, long addedby, string OP_ID,int isUpdate)
        //{
        //    return _KZRepo.SaveDivCommittee(CommitteeID, user1, user2, user3, user4, Committee_name, addedby, OP_ID,isUpdate);
        //}
        //public VM_DivCommitee_UserList EditDivCommittee(string OP_id, long Committee_ID)
        //{
        //    return _KZRepo.EditDivCommittee(OP_id, Committee_ID);
        //}
        //public string DeleteDivCommittee(long id, long empcode)
        //{
        //    return _KZRepo.DeleteDivCommittee(id, empcode);
        //}
        //public DeptAvgMarks DeptCommitteeAvgMarks(string id)
        //{
        //    return _KZRepo.DeptCommitteeAvgMarks(id);
        //}
        //public short SaveDivKaizenMarks(KaizenMarks obj)
        //{
        //    return _KZRepo.SaveDivKaizenMarks(obj);
        //}
        //public List<TopCoreSteeringKaizen> CoreCommitteeKaizen(long empcode,string period,long syki)
        //{
        //    return _KZRepo.CoreCommitteeKaizen(empcode,period,syki);
        //}
        //public KaizenMarks DivEvalutorMarks(long empcode, long tmpid)
        //{
        //    return _KZRepo.DivEvalutorMarks(empcode, tmpid);
        //}
        //public short SaveDivRank(DivRank obj)
        //{
        //    return _KZRepo.SaveDivRank(obj);
        //}
        //public List<KAIZEN_KIViewModel> GetKaizenKiLIST(long ecode)
        //{
        //    return _KZRepo.GetKaizenKiLIST(ecode);
        //}
        //public List<KaizenLibrary_Model> KaizenLibraryData(SearchKZViewModel obj)
        //{
        //    return _KZRepo.KaizenLibraryData(obj);
        //}
        //public List<KaizenMarks> AllDivEvalutorMarks(long tmpid)
        //{
        //    return _KZRepo.AllDivEvalutorMarks(tmpid);
        //}

        //public List<VM_DivCommitee_UserList> GetDivLevelList(SearchKZViewModel obj, long empcode)
        //{
        //    return _KZRepo.GetDivLevelList(obj,empcode);
        //}
        //public  List<VM_Operation_UserList> DivCommitteeKaizenUsers(string KaizenID)
        //{
        //    return _KZRepo.DivCommitteeKaizenUsers(KaizenID);
        //}
        //public Kaizen_Lirbary_VM KaizenLibraryForm(string KaizenID)
        //{
        //    return _KZRepo.KaizenLibraryForm(KaizenID);
        //}
        //public List<VM_Operation_UserList> DeptCommitteeKaizenUser_(long KaizenID,long empcode)
        //{
        //    return _KZRepo.DeptCommitteeKaizenUser_(KaizenID,empcode);
        //}
        public bool isPopupEnableForDivCommittee(long ecode)
        {
            return _KZRepo.isPopupEnableForDivCommittee(ecode);
        }
    }
}
