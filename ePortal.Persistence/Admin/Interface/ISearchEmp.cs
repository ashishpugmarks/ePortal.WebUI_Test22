using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Interface
{
    public interface ISearchEmp
    {
        DataSet GetAllEmployee();
        DataTable GetEmpWindowIDList(string EmpCode, string EmpName, string EmpWindowID, string OPERATION, string DIVISION, string DEPARTMENT);
        DataSet GetSeatNo(string EmpCode);
        DataSet get_AllDept();
        DataSet get_AllDept(int OperationID, int DivisionID);
        DataSet get_AllDesgination();
        DataSet get_AllDiv();
        DataSet get_AllDiv(int OperationID);
        DataSet get_AllModule();
        DataSet get_AllOperation();
        DataSet get_AllSection();
        DataSet get_AllSection(int OperationID, int DivisionID, int DepartmentID);
        DataSet OfficialDetail(string strEmpId);
    }
}
