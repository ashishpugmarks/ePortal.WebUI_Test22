using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace ePortal.Persistence.Admin.Services
{
    public class FlightSchedule : IFlightSchedule
    {
        #region "Local Variables" 
        private readonly IDataManagement oDataMgmt;
        private readonly IConnectionString objCnStr;

        OracleCommand oCmd;
        DataSet ds;
        DataTable Dt;
        OracleConnection objConn;
        string strConn;
        string strErrMsg = string.Empty;
        #endregion

        public FlightSchedule(IDataManagement _oDataMgmt, IConnectionString _objCnStr)
        {
            oDataMgmt = _oDataMgmt;
            objCnStr = _objCnStr;
        }
        public int AddFlight(string fsid, string flightname, string attachment, string status, int AddedBy, string strServiceType)
        {
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    string strSql = "PKG_FLIGHT.SPROC_UPDATE_FLIGHT";
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = objConn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = strSql;
                    cmd.Parameters.Add("FSID_IN", OracleDbType.Varchar2).Value = fsid;
                    cmd.Parameters.Add("FLIGHTNAME_IN", OracleDbType.Varchar2).Value = flightname;
                    cmd.Parameters.Add("ATTACHMENT_IN", OracleDbType.Varchar2).Value = attachment;
                    cmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
                    cmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32).Value = AddedBy;
                    cmd.Parameters.Add("SERVICETYPE_IN", OracleDbType.Int32).Value = strServiceType;
                    cmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                    OracleDataAdapter oDa = new OracleDataAdapter(cmd);
                    cmd.BindByName = true;
                    cmd.ExecuteNonQuery();
                    if (cmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch (Exception ex)
                {
                    string strError = ex.Message;
                }
                finally
                {
                    if (objConn != null)
                    {
                        objConn.Close();
                    }
                }
            }
            return 0;
        }
        public DataSet GetFlightDetails()
        {
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    DataSet objDS = new DataSet();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = objConn;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "PKG_FLIGHT.SPROC_FLIGHT_GET";
                    cmd.Parameters.Add("CUR_DETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    OracleDataAdapter oDa = new OracleDataAdapter(cmd);
                    oDa.Fill(objDS);
                    return objDS;
                }

                finally
                {
                    if (objConn != null)
                    {
                        objConn.Close();
                    }
                }
            }
        }
    }
}

