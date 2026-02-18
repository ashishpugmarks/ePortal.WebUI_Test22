using ePortal.Persistence.Interface;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Interface
{
    public interface IHumanResource
    {
        
        public DataSet getNewJoinee();

        public DataSet GETNEWJOINING_EMPLOYEE(string ecode, string ename, string plantid);


        public string passwordUpdate_Admin(string strEmpCode, string strEType, string empmodBy);


        //// Get My Voice List
        public DataTable GetMyVoiceList(string status);

        public string UpdateMyVoiceStatus(string EmpCode, string ReqId, string EmailStatus, string FileName);
       
       
    }
}
