using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

/// <summary>
/// Summary description for Section
/// </summary>
public class Section : ISection
{
    private readonly IDataManagement oDataMgmt;
    private readonly IConnectionString objCnStr;
    public Section(IDataManagement _oDataMgmt, IConnectionString _objCnStr)
    {
        oDataMgmt = _oDataMgmt;
        objCnStr = _objCnStr;
    }

    #region"Instance Variables"
    DataSet ds = new DataSet();
    DataRow[] _datarow;
    //DataManagement oDataMgmt = new DataManagement();
    #endregion

    #region"Get Functions"
    public DataSet GetAllSection()
    {
        OracleCommand oCmd = new OracleCommand();
        DataSet ds;
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_MASTERS.SPROC_SECTION_GET";
        oCmd.Parameters.Add("CUR_SECTLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        ds = oDataMgmt.GetDataSet(oCmd);
        return (ds);
    }
    public DataSet GetFilterSection(string DepartmentID)
    {
        //ConnectionString objCnStr = new ConnectionString();
        string strConn = objCnStr.getConnectingString();
        using (OracleConnection objCn = new OracleConnection())
        {
            string strSql;
            objCn.ConnectionString = strConn;
            try
            {
                objCn.Open();
                OracleCommand objCmd = new OracleCommand();
                DataSet ds = new DataSet();
                objCmd.Connection = objCn;
                strSql = " SELECT DISTINCT SEC.SECTIONID as ADSECTIONID,SEC.SECTION AS SECTIONNAME FROM   VW_ORGLEVELDETAILS SEC WHERE 1= 1";
                if (DepartmentID.Trim() != "0")
                    strSql = strSql + " and SEC.DEPTID=" + DepartmentID + "  ";
                strSql = strSql + " and SEC.SECTIONID IS NOT NULL ORDER BY SEC.SECTION";

                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.BindByName = true;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                objAdr.Fill(ds, "addepartment");
                return ds;
            }
            finally
            {
                if (objCn != null)
                {
                    objCn.Close();
                }
            }
        }
    }

    public DataSet GetFilterSection(string OperationID,string DivisionID, string DepartmentID)
    {
        //ConnectionString objCnStr = new ConnectionString();
        string strConn = objCnStr.getConnectingString();
        using (OracleConnection objCn = new OracleConnection())
        {
            string strSql;
            objCn.ConnectionString = strConn;
            try
            {
                objCn.Open();
                OracleCommand objCmd = new OracleCommand();
                DataSet ds = new DataSet();
                objCmd.Connection = objCn;

                //strSql = " SELECT SEC.ADSECTIONID,SEC.DESCRIP AS SECTIONNAME,SEC.* FROM   ADSECTION SEC "
                //        + " LEFT JOIN ADDEPARTMENT DEPT ON DEPT.ADDEPARTMENTID = SEC.ADDEPARTMENTID"
                //        + " LEFT JOIN ADDIVISION DIV ON DIV.ADDIVISIONID = DEPT.ADDIVISIONID"
                //        + " LEFT JOIN ADVP OP ON OP.ADVPID = SEC.ADVPID WHERE 1=1  ";

                strSql = " SELECT SEC.SECTIONID AS ADSECTIONID,SEC.SECTION AS SECTIONNAME "
                       + " FROM VW_ORGLEVELDETAILS SEC WHERE 1=1 ";

                if (DepartmentID.Trim() != "0")
                    strSql = strSql + " AND  SEC.DEPTID=" + DepartmentID;

                if (OperationID.Trim() != "0")
                    strSql = strSql + " AND SEC.OPID =" + OperationID;

                if (DivisionID.Trim() != "0")
                    strSql = strSql + " AND SEC.DIVID=" + DivisionID;

                strSql = strSql + " AND  SEC.SECTIONID IS NOT NULL ORDER BY SEC.SECTION ";

                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.BindByName = true;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                objAdr.Fill(ds, "addepartment");
                return ds;
            }
            finally
            {
                if (objCn != null)
                {
                    objCn.Close();
                }
            }
        }
    }
    public DataSet GetFilterSection(int OperationID, int DivisionID, int DepartmentID, String strKI)
    {
        //ConnectionString objCnStr = new ConnectionString();
        string strConn = objCnStr.getConnectingString();
        using (OracleConnection objCn = new OracleConnection())
        {
            string strSql;
            objCn.ConnectionString = strConn;
            try
            {
                objCn.Open();
                OracleCommand objCmd = new OracleCommand();
                DataSet ds = new DataSet();
                objCmd.Connection = objCn;

                strSql = " SELECT SEC.SECTIONID AS ADSECTIONID,SEC.SECTION AS SECTIONNAME, SEC.ACTIVE"
                       + " FROM VW_ORGLEVELDETAILS_KI SEC WHERE 1=1 ";

                if (DepartmentID != 0)
                    strSql = strSql + " AND  SEC.DEPTID=" + DepartmentID.ToString();

                if (OperationID != 0)
                    strSql = strSql + " AND SEC.OPID =" + OperationID.ToString();

                if (DivisionID != 0)
                    strSql = strSql + " AND SEC.DIVID=" + DivisionID.ToString();

                strSql = strSql + " and SEC.sykiid = " + strKI + " AND  SEC.SECTIONID IS NOT NULL ORDER BY SEC.SECTION ";

                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.BindByName = true;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                objAdr.Fill(ds, "addepartment");
                return ds;
            }
            finally
            {
                if (objCn != null)
                {
                    objCn.Close();
                }
            }
        }
    }

    public DataSet getDetails(string strSectionID)
    {
        OracleCommand oCmd = new OracleCommand();
        DataSet ds;
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_MASTERS.SPROC_SECTIONBYID_GET";
        oCmd.Parameters.Add("SECTION_ID", OracleDbType.Varchar2).Value = strSectionID;
        oCmd.Parameters.Add("CUR_SECLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        ds = oDataMgmt.GetDataSet(oCmd);
        return (ds);

    }
    #endregion

    #region"Insert/Update Functions"
    public int AddSection(String strID, String strDesc, String strIniDesc, String strDeptID, String strCreatedBy, String strStatus, String strEmpCode, String strDivId, String strVpId)
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

                string strSql = "PKG_MASTERS.SPROC_UPDATE_SCTION";
                objCmd.Parameters.Add("ADSECTIONID_IN", OracleDbType.Varchar2).Value = strID;
                objCmd.Parameters.Add("DESCRIP_IN", OracleDbType.Varchar2).Value = strDesc;
                objCmd.Parameters.Add("INITIALDESCRIP_IN", OracleDbType.Varchar2).Value = strIniDesc;
                objCmd.Parameters.Add("ADDEPARTMENTID_IN", OracleDbType.Varchar2).Value = strDeptID;
                objCmd.Parameters.Add("BY_IN", OracleDbType.Varchar2).Value = strCreatedBy;
                objCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = strStatus;
                objCmd.Parameters.Add("SECTIONHEADID_IN", OracleDbType.Varchar2).Value = strEmpCode;
                objCmd.Parameters.Add("ADDIVISIONID_IN", OracleDbType.Varchar2).Value = strDivId;
                objCmd.Parameters.Add("ADOPERATIONID_IN", OracleDbType.Varchar2).Value = strVpId;

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
