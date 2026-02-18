using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Persistence.Interface;
using ePortal.ViewModels;
using Oracle.ManagedDataAccess.Client;

namespace ePortal.Persistence.Services
{
    public class SurveyConfig: ISurveyConfig
    {       
        DataTable dt;       
        private readonly IDataManagement oDataMgmt;

        public SurveyConfig(IDataManagement _oDataMgmt)
        {
            oDataMgmt = _oDataMgmt;            
        }

        public List<SurveyAutofillViewModel> FetchHRSurveyAutoFillList()
        {
            List<SurveyAutofillViewModel> objmenu = new List<SurveyAutofillViewModel>();
            DataTable _objdt = new DataTable();
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "SP_HR_SURVEY_AUTOFILL_LIST";
            ocmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            _objdt = oDataMgmt.GetDataTable(ocmd);
            if (_objdt.Rows.Count > 0)
            {
                for (int i = 0; i < _objdt.Rows.Count; i++)
                {
                    objmenu.Add(new SurveyAutofillViewModel
                    {
                        AUTOFILLID = Convert.ToInt64(_objdt.Rows[i]["AUTOFILLID"]),
                        EMPLOYEECODE = Convert.ToInt64(_objdt.Rows[i]["EMPLOYEECODE"]),
                        EMPLOYEENAME = _objdt.Rows[i]["EMPLOYEENAME"].ToString(),
                        STATUS = Convert.ToInt16(_objdt.Rows[i]["STATUS"]),
                    });
                }
            }
            return objmenu;
        }

        public string UpdateHRSurveyAutoFillList(int autoFillId, int modifiedBy, int status)
        {
            string strErrMsg;
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "SP_HR_SURVEY_AUTOFILL_LIST_UPDATE";
            ocmd.Parameters.Add("P_AUTOFILL_ID", OracleDbType.Int64).Value = autoFillId;
            ocmd.Parameters.Add("P_MODIFIED_BY", OracleDbType.Int64).Value = modifiedBy;
            ocmd.Parameters.Add("P_STATUS", OracleDbType.Int32).Value = status;

            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            oDataMgmt.ExecuteQuery(ocmd);
            strErrMsg = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);
            return strErrMsg;
        }

        public List<HrSurveyEmployeeDetailViewModel> GetHrSurveyEmployeeDetails()
        {
            List<HrSurveyEmployeeDetailViewModel> objmenu = new List<HrSurveyEmployeeDetailViewModel>();
            DataTable _objdt = new DataTable();
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "SP_HR_SURVEY_GET_EMPLOYEE_DETAILS";
            ocmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            _objdt = oDataMgmt.GetDataTable(ocmd);
            if (_objdt.Rows.Count > 0)
            {
                for (int i = 0; i < _objdt.Rows.Count; i++)
                {
                    objmenu.Add(new HrSurveyEmployeeDetailViewModel
                    {
                        EMPLOYEEID = Convert.ToInt64(_objdt.Rows[i]["EMPLOYEEID"]),
                        EMPLOYEENAME = _objdt.Rows[i]["EMPLOYEENAME"].ToString(),
                    });
                }
            }
            return objmenu;
        }

        public string AddUserHrSurveyAutoFill(long employeeId, long createdBy, long modifiedBy, int status)
        {
            string strErrMsg;
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "SP_HR_SURVEY_AUTOFILL_LIST_ADD";
            ocmd.Parameters.Add("P_EMPLOYEE_ID", OracleDbType.Int64).Value = employeeId;
            ocmd.Parameters.Add("P_CREATED_BY", OracleDbType.Int64).Value = createdBy;
            ocmd.Parameters.Add("P_MODIFIED_BY", OracleDbType.Int64).Value = modifiedBy;
            ocmd.Parameters.Add("P_STATUS", OracleDbType.Int32).Value = status;

            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            oDataMgmt.ExecuteQuery(ocmd);
            strErrMsg = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);
            return strErrMsg;
        }

        public bool GetHRSurveyAutoFillList(string userId)
        {
            string strErrMsg;
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "SP_HR_SURVEY_AUTOFILL_LIST_DETAIL";
            ocmd.Parameters.Add("P_EMPLOYEE_ID", OracleDbType.Int64).Value = userId;

            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            oDataMgmt.ExecuteQuery(ocmd);
            strErrMsg = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString());
            if (strErrMsg == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<DivisionViewModel> GetAllDivision()
        {
            List<DivisionViewModel> objDivisionModel = new List<DivisionViewModel>();
            DataTable _objdt = new DataTable();

            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_MASTERS.SPROC_DIVISION_GET";
            ocmd.Parameters.Add("CUR_DIVLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            _objdt = oDataMgmt.GetDataTable(ocmd);
            if (_objdt.Rows.Count > 0)
            {
                for (int i = 0; i < _objdt.Rows.Count; i++)
                {
                    objDivisionModel.Add(new DivisionViewModel
                    {
                        ADDIVISIONID = Convert.ToInt64(_objdt.Rows[i]["ADDIVISIONID"]),
                        DESCRIP = _objdt.Rows[i]["DESCRIP"].ToString(),
                        ADVPID = Convert.ToInt64(_objdt.Rows[i]["ADVPID"]),
                        ACTIVE = Convert.ToInt16(_objdt.Rows[i]["ACTIVE"]),
                    });
                }
            }
            return objDivisionModel;
        }
        //*****************aDDED BY TTL AJIT
        public List<DivisionViewModel> GetAllDivisionBYOperation(long OperationID)
        {
            List<DivisionViewModel> objDivisionModel = new List<DivisionViewModel>();
            DataTable _objdt = new DataTable();

            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_MASTERS.SPROC_DIVISIONBYOPERATIONID_GET";
            ocmd.Parameters.Add("CUR_DIVLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Int64).Value = OperationID;
            _objdt = oDataMgmt.GetDataTable(ocmd);
            if (_objdt.Rows.Count > 0)
            {
                for (int i = 0; i < _objdt.Rows.Count; i++)
                {
                    objDivisionModel.Add(new DivisionViewModel
                    {
                        ADDIVISIONID = Convert.ToInt64(_objdt.Rows[i]["ADDIVISIONID"]),
                        DESCRIP = _objdt.Rows[i]["DESCRIP"].ToString(),
                        ADVPID = Convert.ToInt64(_objdt.Rows[i]["ADVPID"]),
                        ACTIVE = Convert.ToInt16(_objdt.Rows[i]["ACTIVE"]),
                    });
                }
            }
            return objDivisionModel;
        }
        public List<OperationViewModel> GetAllOperation()
        {
            List<OperationViewModel> objDivisionModel = new List<OperationViewModel>();
            DataTable _objdt = new DataTable();

            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.BindByName = true;
            ocmd.CommandText = "PKG_COMMONMETHOD.SPROC_OPERATION_GET";
            ocmd.Parameters.Add("CUR_OPERATION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            _objdt = oDataMgmt.GetDataTable(ocmd);
            if (_objdt.Rows.Count > 0)
            {
                for (int i = 0; i < _objdt.Rows.Count; i++)
                {
                    objDivisionModel.Add(new OperationViewModel
                    {
                        OPERATIONID = Convert.ToInt64(_objdt.Rows[i]["OPERATIONID"]),
                        OPERATION = _objdt.Rows[i]["OPERATION"].ToString(),
                    });
                }
            }
            return objDivisionModel;
        }
    }
}
