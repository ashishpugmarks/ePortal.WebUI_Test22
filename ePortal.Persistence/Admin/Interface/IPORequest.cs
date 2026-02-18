using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Interface
{
    public interface IPORequest
    {
        public DataSet ManagePORequest(string strEmpCode);
        public DataSet ManageICApproval(string strSupEmpCode);
        public DataSet ManageICRequest(string strEmpCode);
        public DataSet ManageACRRequest(string strEmpCode);
        public DataSet ManageACRApproval(string strSupEmpCode);
        public DataSet ManageACRRequestHis(string strEmpCode);
        public DataSet ManageACRApprovalHis(string strEmpCode);
    }
}
