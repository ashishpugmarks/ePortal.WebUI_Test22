using System.Data;

namespace ePortal.Persistence.Admin.Interface
{
    public interface IEmpUserDetails
    {
        int AddUserDetail(string strExt, string strPhone, string strmobile, string stremailid, string strLoc,
            string strBlood, string strCreatedBy, string strJacketSize, string strTrouserSize, string strShoeSize,
             string strEmgNo, string strLockerNo, string strWinterJacketSize, string strPeremailid, string strJacketHalfFull, string strMobileCompany, string strMobileModel, string strMobileHeight, string strMobileWidth, string strMobileThick, string strMobileCompany2, string strMobileModel2, string strMobileHeight2, string strMobileWidth2, string strMobileThick2,
             Int16 lngISDOB, Int16 lngISBloodgrp, Int16 lngISMarital, Int16 lngISEMER, Int16 lngISPerMob, Int16 lngISPerEmail, Int64 lngPerMob, string strMarital);
        string updateEmpData(string strEmpModCode, string strDivision, string strDept, string strMod, string strSection,
                       string strDesig, string strJobTitle, string strEcode, string strActive);
        string updateStaffEmpData(string strEmpModCode, string strDivision, string strDept, string strMod, string strSection,
                   string strDesig, string strJobTitle, string strEcode, string strActive);
        int GetCount(string strEmpCode, string strUrl);
        int UpdateUserPancardno(string strEmpcode, string strPancardno, string strmobileno);
        Tuple<Int32, string> UpdateFamilyDeclaration(string strEmpcode, string strName, string strRelation, string strAddedby, Int32 lngISPastCurr);
        Int32 DeleteFamilyDeclaration(string strEmpcode, string headerid);
        Int32 SkipUserdetail(string strEmpcode);
        DataSet SelectDivision();
        DataSet SelectDepartment();
        DataSet SelectModule();
        DataSet SelectSection();
        DataSet SelectDesignation();
        DataSet SelectUserType();
        DataSet SelectProjectName();
        int GetUserSiteId(string strEmpCode);
        DataTable GetEmployeeDeclaration(string strEmpcode);









    }
}
