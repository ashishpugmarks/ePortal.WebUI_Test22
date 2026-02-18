using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Interface
{
    public interface ISection
    {
        int AddSection(string strID, string strDesc, string strIniDesc, string strDeptID, string strCreatedBy, string strStatus, string strEmpCode, string strDivId, string strVpId);
        DataSet GetAllSection();
        DataSet getDetails(string strSectionID);
        DataSet GetFilterSection(string DepartmentID);
        DataSet GetFilterSection(string OperationID, string DivisionID, string DepartmentID);
        DataSet GetFilterSection(int OperationID, int DivisionID, int DepartmentID, string strKI);
    }
}
