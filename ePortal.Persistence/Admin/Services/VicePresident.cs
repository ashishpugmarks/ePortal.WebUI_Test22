using System;
using System.Data;
using System.Configuration;
using ePortal.Persistence.Interface;
using Oracle.ManagedDataAccess.Client;
using ePortal.Persistence.Admin.Interface;
/// <summary>
/// Summary description for AddVicePresident
/// </summary>
public class VicePresident : IVicePresident
{
    #region"Variables"
    DataSet ds = new DataSet();
    DataRow[] _datarow;
    private readonly IDataManagement oDataMgmt;
    private readonly IConnectionString objCnStr;
    #endregion
    public VicePresident(IDataManagement _oDataMgmt, IConnectionString _objCnStr)
    {
        oDataMgmt = _oDataMgmt;
        objCnStr = _objCnStr;
    }
    #region"Get Function"
    public DataTable GetAllVP()
    {

        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_MASTERS.SPROC_VP_GET";
        oCmd.Parameters.Add("CUR_VPLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);

    }
    public string getConnectingString()
    {
        string strCn = string.Empty;
        //strCn = ConfigurationManager.ConnectionStrings["cnConn"].ConnectionString;
        strCn = objCnStr.getConnectingString();
        return strCn;
    }
    public DataTable getDetails(string strAdVPID)
    {
                OracleCommand oCmd = new OracleCommand();
                DataTable objDt = new DataTable();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.BindByName = true;
                oCmd.CommandText = "PKG_MASTERS.SPROC_VPBYID_GET";
                oCmd.Parameters.Add("ADVPID_IN", OracleDbType.Varchar2).Value = strAdVPID;
                oCmd.Parameters.Add("CUR_VPLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                objDt = oDataMgmt.GetDataTable(oCmd);
                return (objDt);
       
    }
    #endregion

    #region"Insert/Update Function"
    public int AddVP(string strVpDesc, string strVpIniDesc, string strEmpCode, string strStatus, string strCreatedBy, string strID)
    {
        //ConnectionString objCnStr = new ConnectionString();
        string strConn = objCnStr.getConnectingString();
        using (OracleConnection objConn = new OracleConnection())
        {
            object Status1;
            objConn.ConnectionString = strConn;
            try
            {
                objConn.Open();
                OracleCommand objCmd = new OracleCommand();

                string strSql = "PKG_MASTERS.SPROC_UPDATE_VP";
                objCmd.Parameters.Add("ADVPID_IN", OracleDbType.Varchar2).Value = strID;
                objCmd.Parameters.Add("DESCRIP_IN", OracleDbType.Varchar2).Value = strVpDesc;
                objCmd.Parameters.Add("INITIALDESCRIP_IN", OracleDbType.Varchar2).Value = strVpIniDesc;
                objCmd.Parameters.Add("BY_IN", OracleDbType.Varchar2).Value = strCreatedBy;
                objCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = strStatus;
                objCmd.Parameters.Add("VPHEADID_IN", OracleDbType.Varchar2).Value = strEmpCode;
                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;

                objCmd.CommandText = strSql;
                objCmd.Connection = objConn;
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.BindByName = true;
                objCmd.ExecuteNonQuery();
                return Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
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
    #endregion
    
}
