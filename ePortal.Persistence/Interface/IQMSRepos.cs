using System.Data;

namespace ePortal.Persistence.Interface
{
    public interface IQMSRepos
    {

        DataSet QMSDeptProc();
        DataTable QMSDeptProc_ByPlantID(string strPlantId, string strDeptId);
        DataSet DeptSectProcList(string strDeptCode, string PLANTID);
        DataSet getQmsstandards(string strparentid, string strstatus);
        DataSet getQmsDetails();
        DataSet getQmsDetails1();
        string QMSDetail(string Description, string Filename, string Department, string Section, int flagvalue, string Qmsid, int Status, string Remarks, string strPlant);
        DataSet getQmsDepartmentalDetails();
        DataSet getQmsSectionDetails(string strDepartment);
        DataSet getQms_Search(string strDepartment, string strSection, string strActive);
        DataSet getQms_Search_New(string strDepartment, string strSection, string description, string strActive, string QMSDOCID);
        DataTable GetDeptartment_Plant(string strPlantID);
        DataTable Get_DeptHead(string strEmpCode, string strChangeType);
        string ISODOCDetail_Set(string strIsoDocId, string strIsoDocNo, string strIsoDocTitle, string strChangeType, string strPlant, string strDept, string strSection, string strOldRevNo, string strOldRevDate, string strNewRevNo, string strNewRevDate, string strFilename, string strAddedBy, string strReqEmpCode, string strReason, string strAppAuthEcode, string strOldid, int strOption, string strrevdocno);
        string ISODOCDetail_UPDATE(string strIsoDocId, string strIsoDocNo, string strIsoDocTitle, string strChangeType, string strPlant, string strDept, string strSection, string strOldRevNo, string strOldRevDate, string strNewRevNo, string strNewRevDate, string strFilename, string strAddedBy, string strReqEmpCode, string strReason, string strAppAuthEcode, string strOldid, int strOption);
        DataTable GetPendingRequestList(string EmpCode);
        DataTable ISODOCDetail_Get(string strIsodocid);
        DataTable GetPendingApprovalList(string EmpCode);
        DataTable GetHistoryRequestList(string EmpCode);
        DataTable GetRequestDetail(string RequestID);
        DataTable GetHistoryApprovalList(string EmpCode);
        string ISODOCRemark_Set(string strIsoDocId, string strEmpCode, string strEmpRemark, string strEmpStatus, string strAppAuthEcode, string strAppAuthRemark, string strAppAuthStatus, string strIsoAuthEcode, string strIsoAuthRemark, string strIsoAuthStatus, string strStatus, string strOldIsoDocId, string strReqType);
        DataTable ISODocumentInfo_Get(string EmpCode, string strRequstType, string strRequstStatus, string strFromdate, string strTodate, string strDept, string strPlant, string strSection, string strActive);
        DataTable QMSDocList();
        DataTable SPROC_QMSAPPLIST_GET(string strDeptid);
        DataTable Get_ISODOCDEPARTMENT(string plant);
        DataTable ISODOCDetail_Get_Detail(string strIsodocid);
        string UPDATEQMSDOCDEPT(string QMSDOCID, string strDepartment, string strSection, string addedby);
        DataSet GETQMSFORMATNUMBERLIST(string formatnumberid, string formatnumber, string status);
        string INSERTFORMATNUMBER(string formatnumber, string status, string formatnumberid, string addedby);
        string GetCurrentPage(Uri uri);
        DataTable ISODADMINHEADINFO_GET(string EmpCode, string strRequstType, string strRequstStatus, string strFromdate, string strTodate, string strDept, string strPlant, string strSection, string strActive);
        string ISODOCHEADRemark_Set(string strIsoDocId, string strEmpCode, string strEmpRemark, string strEmpStatus, string strIsoheadAuthEcode, string strIsoheadAuthRemark, string strIsoheadAuthStatus, string strStatus, string strOldIsoDocId, string strReqType);
        string CHECKQMSFORMATNUMBER(string formatnumber);
        DataSet GETAPPROVALAUTHORITY(string transid, string plant, string plantappauth, string hoappauth);
        string INSERTAPPROVALAUTHORITY(string transid, string plant, string plantappauth, string hoappauth, string addedby);
        DataTable GetPlant(string ADEMPCOE);
        DataTable GetHOAppAuthPlant(string ADEMPCOE);
        string CHECKISPLANTAPPAUTHSET(string plantid);
        string ISODOCAPPROVAL(string REQUESTID, string ECODE, string STATUS, string REMARKS, string CHANGETYPE, string APPROVALTYPE);
        DataSet GETPLANTHEADREQUEST(string REQECODE, string ENAME, string PLANT, string DEPT, string REQTYPE, string STATUS, string ECODE);
        DataSet GETHOHEADREQUEST(string REQECODE, string ENAME, string PLANT, string DEPT, string REQTYPE, string STATUS, string ECODE);

    }
}
