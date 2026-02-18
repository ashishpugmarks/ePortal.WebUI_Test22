using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Interface
{
    public interface IDivision
    {
        DataTable GetAllDivision();
        DataSet GetFilterDivision(int operationID);
        DataSet GetFilterDivision(int operationID, String strKI);
        DataSet GetDivision();
        DataSet getDetails(string strDivisionID);
        DataSet GetFilterDivisionOJT(int operationID, string str_KIid);
        int AddDivision(string strDivDesc, string strDivIniDesc, string strDivHeadID, string strADVPID, string strStatus, string strCreatedBy, string strID);
    }
}
