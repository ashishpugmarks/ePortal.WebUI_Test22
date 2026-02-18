using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Interface
{
    public interface IDepartment
    {
        DataSet GetAllDepartment();
        DataSet GetAllDepartmentCoordinators();        
        DataSet GetCompleteDepartmentList();
        DataSet GetFilterDepartment(int OperationID, int divisionID);
        DataSet GetFilterDepartment(int OperationID, int divisionID, String strKI);
        DataSet GetAllADVP();
        DataSet GetAllADVP(String strKI);
        DataSet GetAllDesignation();
        DataSet GetDesignationForOJT();
        DataSet getDetails(string addepartmentid);
        DataTable GetEmployeeDetails(int EmpCode);
        DataSet GetDesignationForShowSkill(int GroupLevel);
        DataSet GetAssociateDepartment(string strEmpCode);
        int AddDepartment(string strDepDesc, string strDepIniDesc, string strEmpCode, string strDivisionID, string strStatus, string strCreatedBy, string strID, string strOperationID);
        int AddOrganisationChart(string strDescription, string strFilename, string strAddedby,
                    string strStatus, string strModifyby, string strOrgchartId, string strFlag);
        DataTable GetOrganisationChart();

    }
}
