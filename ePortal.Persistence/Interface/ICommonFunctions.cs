using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Interface
{
    public interface ICommonFunctions
    {
        DataTable GetDesignation(string strValue);
        DataTable GetDesignationFORKIOSK(string strValue);
        DataTable GetAllDesignation(string strValue);
        DataTable GetEmployeeOfficialDetails(string userID, string KiID);
        DataTable GetTempEmployeeDetails(string userID, string KiID);
        DataTable GetFuncDesig();
        DataTable GetAssociate_Details(string ADEMPCODE_IN, string FIRSTNAME_IN, string LASTNAME_IN, string BLOODGROUP_IN, string EMAILID_IN, string DESIGNATION_IN, string OPERATION_IN, string DIVISION_IN, string DEPARTMENT_IN, string SECTION_IN);
        String GetParameterValue(string strParmaName);
        DataTable GetSITE();
        DataTable GetSITECAL();
        DataTable GetPlant();
        DataTable GetShoeSizes();
        DataTable GetTrouserSizes();
        DataTable GetEmpDetails(string Ecode);
        string GetEMailID(string AdempCode);
        DataTable GetOperationDivDeptBySection(string strSectionID);
        DataTable GetLocation(String strsite);
        DataTable GetBank();
        DataTable GetLevels(Int32 LevelType, string ParentLevelID);
        DataTable GetAssociateDetails(string KI, string OperationID, string DivisionID, string DepartmentID, string SectionID, string EmpCode, string FirstName, string LastName, string Designation, string EMailID, string BloodGroup, string FunDesignation);
        DataTable GetApprovalMatrix(string EmpCode, String FunctionDesigForSelfApp, string ExcludeLvl);
        DataTable GetApprovalMatrixWithLeaveSup(string EmpCode);
        DataTable GetShiftTimings(string strPlantID);
        DataTable GetOrgLevelHead(string strLevelType, string strLevelID);
        DataTable GetDeptartment_Plant(string strPlantID);
        DataSet GetHOMEPAGECOUNT_GET(string EMPCODE_IN);
        DataSet GetAPPROVALCOUNT_GET(string EMPCODE_IN);
        DataSet GETKIOPERDIVDEPTSEC(string OPERATION, string DIVISION, string DEPARTMENT);
        int CheckRequestStatusForITSELF(string EMPCODE);
        string UPDATE_SAP_STATUS(string strType, string strTransId, string strSapStatus, string strSapRemarks, string strModifiedBy);
        DataTable GetApprenticeAssociateDetails(string KI, string OperationID, string DivisionID, string DepartmentID, string SectionID,
                                                     string EmpCode, string FirstName, string LastName);
        string EmailApprovalInsert(string strwid, string strcontroler, string straction, string strTid);
        DataSet EmailApprovalGet(long strEmpCode);
        DataSet GetGunctionDesID(string userID);
        DataSet ManageTransferApproval(string strSupEmpCode);
        DataSet ManageLookSeeApproval(string strSupEmpCode);
        DataSet ManageLookSeeSTLApproval(string strSupEmpCode);
        DataSet ManageRelocationApproval(string strSupEmpCode);
        DataSet ManageRelocationSTLApproval(string strSupEmpCode);
        DateTime GetHOverstayDate(string OverstayDate, string strAdempcode);
        string GetGender(string OverstayDate, string Adempcode);
        DataTable GetMobileCoverDetail(string strEcode, string strName, string strDesignation, string strLVL, string strPlant);
        DataTable GetEmployeeLoginMapIDDetails(string MapFlag, string EmpFlag);
        DataTable GetDealerData(string strSearch);

        DataTable IOMUserReportGraphData(SearchIOMsproc param);
        DataTable LC_GetApprovalHistory(int lctranno);
        string GetSupervisorEmpCodeByUserID(string strUserId);

    }
}
