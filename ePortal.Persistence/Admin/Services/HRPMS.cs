using ePortal.Persistence;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using ePortal.Shared;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Text.RegularExpressions;

/// <summary>
/// Summary description for SearchEmp
/// </summary>
public class HRPMS : IHRPMS
{
    DataSet objDs;

    private readonly IConnectionString _conn;
    private readonly IDataManagement oDataMgmt;


    public HRPMS(IConnectionString conn, IDataManagement _oDataMgmt)
    {
        oDataMgmt = _oDataMgmt;
        _conn = conn;
    }

    public string GetCurrentPage(Uri uri)
    {
        string[] segment = uri.Segments;
        string page = string.Empty;
        if (0 < segment.Length)
        {
            page = segment[segment.Length - 1];
        }
        return page;
    }

    public DataSet GETCOMPENTENCYHEADER(string compenheaderid, string compenheaderdesc, string status)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.Parameters.Add("COMPHEADMSTID_IN", OracleDbType.Varchar2).Value = compenheaderid;
        oCmd.Parameters.Add("COMHEADDESC_IN", OracleDbType.Varchar2).Value = compenheaderdesc;
        oCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = status;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_COMPETENCYHEADER_GET";
        oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        return oDataMgmt.GetDataSet(oCmd);
        oCmd.Dispose();
    }

    public string INSERTCOMPENTENCYHEADER(string compenheaderdesc, string status, string compenheaderid, string addedby)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_COMPETENCYHEADER_INSERT";
        oCmd.Parameters.Add("COMHEADDESC_IN", OracleDbType.Varchar2).Value = compenheaderdesc;
        oCmd.Parameters.Add("COMPHEADMSTID_IN", OracleDbType.Varchar2).Value = compenheaderid;
        oCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = status;
        oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = addedby;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public DataSet GETCOMPENLEVEL(string comlevelid, string comleveldesc, string status, string comheaderid, string empcode, string syki)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.Parameters.Add("COMPETENCYLEVELID_IN", OracleDbType.Varchar2).Value = comlevelid;
        oCmd.Parameters.Add("COMPETENCYLEVEL_IN", OracleDbType.Varchar2).Value = comleveldesc;
        oCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = status;
        oCmd.Parameters.Add("COMPHEADMSTID_IN", OracleDbType.Varchar2).Value = comheaderid;
        oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = empcode;
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = syki;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_COMPETENCYLEVEL_GET";
        oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        return oDataMgmt.GetDataSet(oCmd);
        oCmd.Dispose();
    }

    public string INSERTCOMPENTENCYLEVEL(string compenleveldesc, string status, string compenheaderid, string addedby, string compenlevelid)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_COMPETENCYLEVEL_INSERT";
        oCmd.Parameters.Add("COMPETENCYLEVELDESC_IN", OracleDbType.Varchar2).Value = compenleveldesc;
        oCmd.Parameters.Add("COMPHEADMSTID_IN", OracleDbType.Varchar2).Value = compenheaderid;
        oCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = status;
        oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = addedby;
        oCmd.Parameters.Add("COMPETENCYLEVELID_IN", OracleDbType.Varchar2).Value = compenlevelid;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public DataTable GEHEADDESIGMAP(string itemid)
    {
        DataTable objDt = new DataTable();
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_HEADDESIGMAP_GET";
        oCmd.Parameters.Add("VARDESIGID", OracleDbType.Varchar2).Value = itemid;
        oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        objDt = oDataMgmt.GetDataTable(oCmd);
        return (objDt);
    }

    public DataTable GETDESIGNATION()
    {
        DataTable objDt = new DataTable();
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_DESIGNATION_GET";
        oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        objDt = oDataMgmt.GetDataTable(oCmd);
        return (objDt);
    }

    public DataTable GEHEADDESIGMAPBYID(string desigid)
    {
        DataTable objDt = new DataTable();
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_HEADDESIGMAPBYID_GET";
        oCmd.Parameters.Add("DESIGID_IN", OracleDbType.Varchar2).Value = desigid;
        oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        objDt = oDataMgmt.GetDataTable(oCmd);
        return (objDt);
    }

    public string INSERTHEADERDESIGNATIONMAPPING(string addedby, string desigid, string XMLSITEIN)
    {
        //ConnectionString objCnStr = new ConnectionString();
        string strConn = _conn.getConnectingString();
        string MSG = string.Empty;
        using (OracleConnection objConn = new OracleConnection())
        {
            objConn.ConnectionString = strConn;
            try
            {
                objConn.Open();
                OracleCommand objCmd = new OracleCommand();
                string strSql = "PKG_HR_PMS.SPROC_HEADERDESIGNATION_SET";
                objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = addedby;
                objCmd.Parameters.Add("DESIGID_IN", OracleDbType.Varchar2).Value = desigid;
                objCmd.Parameters.Add("XMLSERVICE_TYPE", OracleDbType.Varchar2).Value = XMLSITEIN;
                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                objCmd.CommandText = strSql;
                objCmd.Connection = objConn;
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.BindByName = true;
                objCmd.ExecuteNonQuery();
                MSG = objCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + objCmd.Parameters["ERRMSG"].Value.ToString();
            }
            finally
            {
                if (objConn != null)
                {
                    objConn.Close();
                }
            }
            return MSG;
        }
    }

    public string UPDATEHEADERDESIGNATIONMAPPING(string addedby, string desigid, string XMLSITEIN)
    {
        //ConnectionString objCnStr = new ConnectionString();
        string strConn = _conn.getConnectingString();
        string MSG = string.Empty;
        using (OracleConnection objConn = new OracleConnection())
        {
            objConn.ConnectionString = strConn;
            try
            {
                objConn.Open();
                OracleCommand objCmd = new OracleCommand();
                string strSql = "PKG_HR_PMS.SPROC_UPDATEHEADDESIG_SET";
                objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = addedby;
                objCmd.Parameters.Add("DESIGID_IN", OracleDbType.Varchar2).Value = desigid;
                objCmd.Parameters.Add("XMLSERVICE_TYPE", OracleDbType.Varchar2).Value = XMLSITEIN;
                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                objCmd.CommandText = strSql;
                objCmd.Connection = objConn;
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.BindByName = true;
                objCmd.ExecuteNonQuery();
                MSG = objCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + objCmd.Parameters["ERRMSG"].Value.ToString();
            }
            finally
            {
                if (objConn != null)
                {
                    objConn.Close();
                }
            }
            return MSG;
        }
    }

    public DataTable GetKiList()
    {
        //ConnectionString objCnStr = new ConnectionString();
        string strConn = _conn.getConnectingString();
        DataTable objDt = new DataTable();

        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strConn;
            try
            {
                objCn.Open();
                OracleCommand objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                string strSql = "PKG_HR_PMS.SPROC_KILIST_GET";
                objCmd.Parameters.Add("CUR_KI", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.BindByName = true;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                objAdr.Fill(objDt);
                return objDt;
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

    public string GetKIId()
    {
        ////ConnectionString objCnStr;
        string strCn;
        OracleCommand objCmd;
        //objCnStr = new ConnectionString();
        strCn = _conn.getConnectingString();
        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strCn;
            try
            {
                objCn.Open();
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                objCmd.CommandText = "PKG_HR_PMS.SPORC_CURRRNTKIID_GET";
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.BindByName = true;
                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                objCmd.ExecuteNonQuery();
                return Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
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

    public DataTable GetPMSAuthorities(string userID, string SYKI)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        //DataManagement oDataMgmt = new DataManagement();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_PMSAUTHORITY_GET";
        oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = userID;
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = SYKI;
        oCmd.Parameters.Add("CUR_APPAUTH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }

    public DataTable GetPMSAuthoritiesMatrix(string userID, string KIID)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        //DataManagement oDataMgmt = new DataManagement();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_PMSAUTHORITYMATRIX_GET";
        oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = userID;
        oCmd.Parameters.Add("SYKIID_IN", OracleDbType.Varchar2).Value = KIID;
        oCmd.Parameters.Add("CUR_APPAUTH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }

    public DataTable GetHROverAllReport(string strKiId, string strDesignationId,
                                        string strDivisionId, string strDepartmentId, string strSectionId, string strOperationId,
                                        string strAssoEmpCode, string strAssoEmpName, string strEligibilityStatus)
    {
        //ConnectionString objCnStr = new ConnectionString();
        string strConn = _conn.getConnectingString();
        DataTable objDt = new DataTable();

        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strConn;
            try
            {
                objCn.Open();
                OracleCommand objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                string strSql = "PKG_HR_PMS.SPROC_HROVERALLREPORT_GET";
                objCmd.Parameters.Add("CUR_HR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = strKiId;
                objCmd.Parameters.Add("DESIGNATIONID_IN", OracleDbType.Varchar2).Value = strDesignationId;
                objCmd.Parameters.Add("ELIGIBILITYSTATUS_IN", OracleDbType.Varchar2).Value = strEligibilityStatus;
                if (strOperationId != "0")
                {
                    objCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Varchar2).Value = strOperationId;
                }
                if (strDivisionId != "0")
                {
                    objCmd.Parameters.Add("DIVISIONID_IN", OracleDbType.Varchar2).Value = strDivisionId;
                }
                if (strDepartmentId != "0")
                {
                    objCmd.Parameters.Add("DEPARTMENTID_IN", OracleDbType.Varchar2).Value = strDepartmentId;
                }
                if (strSectionId != "0")
                {
                    objCmd.Parameters.Add("SECTIONID_IN", OracleDbType.Varchar2).Value = strSectionId;
                }
                if (strAssoEmpCode != "")
                {
                    objCmd.Parameters.Add("ASSOEMPCODE_IN", OracleDbType.Varchar2).Value = strAssoEmpCode;
                }
                if (strAssoEmpName != "")
                {
                    objCmd.Parameters.Add("ASSOEMPNAME_IN", OracleDbType.Varchar2).Value = strAssoEmpName;
                }
                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.BindByName = true;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                objAdr.Fill(objDt);
                return objDt;
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

    public DataSet GETAMANAGEPMS(string SYKI, string ECODE, string OPERATION, string DIVISION, string DEPARTMENT, string SECTION)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_MANAGEPMS";
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = SYKI;
        oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Varchar2).Value = OPERATION;
        oCmd.Parameters.Add("DIVISIONID_IN", OracleDbType.Varchar2).Value = DIVISION;
        oCmd.Parameters.Add("DEPARTMENTID_IN", OracleDbType.Varchar2).Value = DEPARTMENT;
        oCmd.Parameters.Add("SECTIONID_IN", OracleDbType.Varchar2).Value = SECTION;
        oCmd.Parameters.Add("CUR_SELFPROC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_EVALPROC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_REVPROC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        return oDataMgmt.GetDataSet(oCmd);
        oCmd.Dispose();
    }

    public DataSet GETGOALSETTINGFORMDATA(string ECODE, string SYKI)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_GETGOALSETTINGFRM_DATA";
        oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = SYKI;
        oCmd.Parameters.Add("CUR_ASSDET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_PMSTRANS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_ACTIVITY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_COMPETENCIESHEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_COMPETENCIESLVL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_TWOWAYCOMMENT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        return oDataMgmt.GetDataSet(oCmd);
        oCmd.Dispose();
    }

    public string INSERTGOALSETTINGROLE(string role, string ecode, string HRPMSID, string SYKI)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_GOALSETTINGROLE_INSERT";
        oCmd.Parameters.Add("ROLE_IN", OracleDbType.Varchar2).Value = role;
        oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ecode;
        oCmd.Parameters.Add("HRPMSID_IN", OracleDbType.Varchar2).Value = HRPMSID;
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = SYKI;
        oCmd.Parameters.Add("PMSTRANSID_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["PMSTRANSID_OUT"].Value.ToString() + "#" + oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public string INSERTACTIVITYLIST(string HRPMSID, string GOALID, string ACTIVITYNAME, string ACTIVITYCONTROLITEM, string ACTIVITYTARGET, string ACTIVITYWEIGHTAGE, string ECODE)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_ACTIVITYLIST_INSERT";
        oCmd.Parameters.Add("HRPMSID_IN", OracleDbType.Varchar2).Value = HRPMSID;
        oCmd.Parameters.Add("GOALID_IN", OracleDbType.Varchar2).Value = GOALID;
        oCmd.Parameters.Add("ACTIVITYNAME", OracleDbType.Varchar2).Value = ACTIVITYNAME;
        oCmd.Parameters.Add("CONTROLITEM_IN", OracleDbType.Varchar2).Value = ACTIVITYCONTROLITEM;
        oCmd.Parameters.Add("TARGET_IN", OracleDbType.Varchar2).Value = ACTIVITYTARGET;
        oCmd.Parameters.Add("WEIGHTAGE_IN", OracleDbType.Varchar2).Value = ACTIVITYWEIGHTAGE;
        oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public string DELETEACTIVITY(string ACTIVITYID, string ECODE)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_ACTIVITY_DELETED";
        oCmd.Parameters.Add("ACTIVITYID_IN", OracleDbType.Varchar2).Value = ACTIVITYID;
        oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public string OTHERACTIVITYINSERTED(string ECODE, string weightage, string otheractivityxml, string HRPMSID)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_OTHERACTIVITY_INSERTED";
        oCmd.Parameters.Add("XMLACTIVITYLIST_IN", OracleDbType.Varchar2).Value = otheractivityxml;
        oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("WEIGHTAGE_IN", OracleDbType.Varchar2).Value = weightage;
        oCmd.Parameters.Add("HRPMSID_IN", OracleDbType.Varchar2).Value = HRPMSID;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public string GOALSETTINGUPDATE(string ECODE, string HRPMSID)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_GOALSETTING_INSERTED";
        oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("HRPMSID_IN", OracleDbType.Varchar2).Value = HRPMSID;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public DataSet GETGOALSETTINGDETAIL(string HRPMSID)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_PMSDATA_GET";
        oCmd.Parameters.Add("PMSID_IN", OracleDbType.Varchar2).Value = HRPMSID;
        oCmd.Parameters.Add("CUR_ASSDET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_PMSTRANS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_ACTIVITY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_COMPETENCIESHEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_COMPETENCIESLVL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_TWOWAYCOMMENT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        return oDataMgmt.GetDataSet(oCmd);
        oCmd.Dispose();
    }

    public string GOALSETTINGSTATUS_UPDATE(string HRPMSID, string BUTTONID, string COMMENT, string ECODE, string TWOWAYCONFIRMATION)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPORC_GOALSETTINGSTATUS_SET";
        oCmd.Parameters.Add("PMSID_IN", OracleDbType.Varchar2).Value = HRPMSID;
        oCmd.Parameters.Add("BUTTONID_IN", OracleDbType.Varchar2).Value = BUTTONID;
        oCmd.Parameters.Add("COMMENT_IN", OracleDbType.Varchar2).Value = COMMENT;
        oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("CONFIRMATION_IN", OracleDbType.Varchar2).Value = TWOWAYCONFIRMATION;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public string UploadFile(string strEmpCode, string KiID, string AssociateID, string EvaluatorID, string ReviewerID,
                               string DivHeadID, string OPHeadID, string EligilbeStatus, string GSStartDate, string GSEndDate,
                               string EvalGSStartDate, string EvalGSEndDate, string RevGSStartDate, string RevGSEndDate,
                               string FHStartDate, string FHEndDate, string EvalFHStartDate, string EvalFHEndDate,
                               string RevFHStartDate, string RevFHEndDate, string SHStartDate, string SHEndDate,
                               string EvalSHStartDate, string EvalSHEndDate, string RevSHStartDate, string RevSHEndDate,
                               string GSStatus, string FHStatus, string SHStatus)
    {
       // //ConnectionString objCnStr;
        string strCn;
        OracleCommand objCmd;
        string strErrMsg;

        //objCnStr = new ConnectionString();
        strCn = _conn.getConnectingString();

        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strCn;
            try
            {
                objCn.Open();
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                objCmd.CommandText = "PKG_HR_PMS.SPORC_UPLOADFILE_SET";
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.BindByName = true;
                objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                objCmd.Parameters.Add("ASSOEMPCODE_IN", OracleDbType.Varchar2).Value = AssociateID;
                objCmd.Parameters.Add("SYKIID_IN", OracleDbType.Varchar2).Value = KiID;
                objCmd.Parameters.Add("EVALCODE_IN", OracleDbType.Varchar2).Value = EvaluatorID;
                objCmd.Parameters.Add("REVCODE_IN", OracleDbType.Varchar2).Value = ReviewerID;
                objCmd.Parameters.Add("DIVHEADCODE_IN", OracleDbType.Varchar2).Value = DivHeadID;
                objCmd.Parameters.Add("OHCODE_IN", OracleDbType.Varchar2).Value = OPHeadID;
                objCmd.Parameters.Add("ELIGIBLESTATUS_IN", OracleDbType.Varchar2).Value = EligilbeStatus;

                //GS EVALUATOR/REVIEWER
                objCmd.Parameters.Add("GSSTARTDATE_IN", OracleDbType.Varchar2).Value = GSStartDate;
                objCmd.Parameters.Add("GSENDDATE_IN", OracleDbType.Varchar2).Value = GSEndDate;
                objCmd.Parameters.Add("EVALGSSTARTDATE_IN", OracleDbType.Varchar2).Value = EvalGSStartDate;
                objCmd.Parameters.Add("EVALGSENDDATE_IN", OracleDbType.Varchar2).Value = EvalGSEndDate;
                objCmd.Parameters.Add("REVGSSTARTDATE_IN", OracleDbType.Varchar2).Value = RevGSStartDate;
                objCmd.Parameters.Add("REVGSENDDATE_IN", OracleDbType.Varchar2).Value = RevGSEndDate;
                //------------------------------------------------------------------------------------


                //FH EVALUATOR/REVIEWER/COMMENT1/OPHEAD
                objCmd.Parameters.Add("FHSTARTDATE_IN", OracleDbType.Varchar2).Value = FHStartDate;
                objCmd.Parameters.Add("FHENDDATE_IN", OracleDbType.Varchar2).Value = FHEndDate;
                objCmd.Parameters.Add("EVALFHSTARTDATE_IN", OracleDbType.Varchar2).Value = EvalFHStartDate;
                objCmd.Parameters.Add("EVALFHENDDATE_IN", OracleDbType.Varchar2).Value = EvalFHEndDate;
                objCmd.Parameters.Add("REVFHSTARTDATE_IN", OracleDbType.Varchar2).Value = RevFHStartDate;
                objCmd.Parameters.Add("REVFHENDDATE_IN", OracleDbType.Varchar2).Value = RevFHEndDate;

                //--------------------------------------------------------------------------------------

                //SH EVALUATOR/REVIEWER/COMMENT1/OPHEAD
                objCmd.Parameters.Add("SHSTARTDATE_IN", OracleDbType.Varchar2).Value = SHStartDate;
                objCmd.Parameters.Add("SHENDDATE_IN", OracleDbType.Varchar2).Value = SHEndDate;
                objCmd.Parameters.Add("EVALSHSTARTDATE_IN", OracleDbType.Varchar2).Value = EvalSHStartDate;
                objCmd.Parameters.Add("EVALSHENDDATE_IN", OracleDbType.Varchar2).Value = EvalSHEndDate;
                objCmd.Parameters.Add("REVSHSTARTDATE_IN", OracleDbType.Varchar2).Value = RevSHStartDate;
                objCmd.Parameters.Add("REVSHENDDATE_IN", OracleDbType.Varchar2).Value = RevSHEndDate;

                //--------------------------------------------------------------------------------------
                objCmd.Parameters.Add("GSSTATUS_IN", OracleDbType.Varchar2).Value = GSStatus;
                objCmd.Parameters.Add("FHSTATUS_IN", OracleDbType.Varchar2).Value = FHStatus;
                objCmd.Parameters.Add("SHSTATUS_IN", OracleDbType.Varchar2).Value = SHStatus;
                //--------------------------------------------------------------------------------------

                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;

                objCmd.ExecuteNonQuery();
                strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
            }
            finally
            {
                if (objCn != null)
                {
                    objCn.Close();
                }
            }
        }
        return strErrMsg;
    }

    public DataSet GETHRPMSREPORT(string SYKI, string OPERATION, string DIVISION, string DEPARTMENT, string SECTION, string DESIGNATION, string ECODE, string ENAME, string SITE)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_HRPMSREPORT_GET";
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = SYKI;
        oCmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = OPERATION;
        oCmd.Parameters.Add("DIVISION_IN", OracleDbType.Varchar2).Value = DIVISION;
        oCmd.Parameters.Add("DEPARTMENT_IN", OracleDbType.Varchar2).Value = DEPARTMENT;
        oCmd.Parameters.Add("SECTION_IN", OracleDbType.Varchar2).Value = SECTION;
        oCmd.Parameters.Add("DESIGNATION_IN", OracleDbType.Varchar2).Value = DESIGNATION;
        oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("ENAME_IN", OracleDbType.Varchar2).Value = ENAME;
        oCmd.Parameters.Add("SITE_IN", OracleDbType.Varchar2).Value = SITE;
        oCmd.Parameters.Add("CUR_PMSRPT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        return oDataMgmt.GetDataSet(oCmd);
        oCmd.Dispose();
    }

    public string INSERT_FIRSTHALF_ACTIVITYRESULT(string HRPMSID, string Ecode, string XMLACTIVITY)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPORC_INSERTFHACTRES_SET";
        oCmd.Parameters.Add("PMSID_IN", OracleDbType.Varchar2).Value = HRPMSID;
        oCmd.Parameters.Add("XMLACTIVITY_IN", OracleDbType.Varchar2).Value = XMLACTIVITY;
        oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = Ecode;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public string INSERT_FIRSTHALF_SENDTOEVALUATOR(string HRPMSID, string ASSOCIATECOMMENT, string ECODE)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPORC_FHSTATUSUPDATE_SET";
        oCmd.Parameters.Add("PMSID_IN", OracleDbType.Varchar2).Value = HRPMSID;
        oCmd.Parameters.Add("ASSCOMMENT_IN", OracleDbType.Varchar2).Value = ASSOCIATECOMMENT;
        oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public string FIRSTHALFEVALSUBMIT(string HRPMSID, string BUTTONID, string PARTACOMMENT, string PARTBCOMMENT, string ECODE, string XMLPERFASSESSMENT,
        string XMLCOMPETENCIES, string FLAGAUTH, string FINALTOTALEVALSCORE, string FINALRATING, string FINALFHSCORE, string EVALOTHACTSCORE, string FEEDBACKCOMMENT, string FINALTOTALREVSCORE)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPORC_FIRSTHALFEVALSUBMIT_SET";
        oCmd.Parameters.Add("PMSID_IN", OracleDbType.Varchar2).Value = HRPMSID;
        oCmd.Parameters.Add("BUTTONID_IN", OracleDbType.Varchar2).Value = BUTTONID;
        oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("XMLPERFASSESSMENT_IN", OracleDbType.Varchar2).Value = XMLPERFASSESSMENT;
        oCmd.Parameters.Add("OTHACTSCORE_IN", OracleDbType.Varchar2).Value = EVALOTHACTSCORE;
        oCmd.Parameters.Add("PARTACOMMENT_IN", OracleDbType.Varchar2).Value = PARTACOMMENT;
        oCmd.Parameters.Add("XMLCOMPETENCIES_IN", OracleDbType.Varchar2).Value = XMLCOMPETENCIES;
        oCmd.Parameters.Add("PARTBCOMMENT_IN", OracleDbType.Varchar2).Value = PARTBCOMMENT;
        oCmd.Parameters.Add("FEEDBACKCOMMENT_IN", OracleDbType.Varchar2).Value = FEEDBACKCOMMENT;
        oCmd.Parameters.Add("FINALTTLSCORE_IN", OracleDbType.Varchar2).Value = FINALTOTALEVALSCORE;
        oCmd.Parameters.Add("FINALRATING_IN", OracleDbType.Varchar2).Value = FINALRATING;
        oCmd.Parameters.Add("FINALFHSCORE_IN", OracleDbType.Varchar2).Value = FINALFHSCORE;
        oCmd.Parameters.Add("FLAGAUTH_IN", OracleDbType.Varchar2).Value = FLAGAUTH;
        oCmd.Parameters.Add("FINALREVSCORE_IN", OracleDbType.Varchar2).Value = FINALTOTALREVSCORE;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public DataSet GETCOMPENLEVELWITHSCORE(string HRPMSID, string COMPHEADID)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_COMPLVLWITHSCORE_GET";
        oCmd.Parameters.Add("HRPMSID_IN", OracleDbType.Varchar2).Value = HRPMSID;
        oCmd.Parameters.Add("COMPHEADMSTID_IN", OracleDbType.Varchar2).Value = COMPHEADID;
        oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        return oDataMgmt.GetDataSet(oCmd);
        oCmd.Dispose();
    }

    public string INSERT_SECONDHALF_ACTIVITYRESULT(string HRPMSID, string Ecode, string XMLACTIVITY)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPORC_INSERTSHACTRES_SET";
        oCmd.Parameters.Add("PMSID_IN", OracleDbType.Varchar2).Value = HRPMSID;
        oCmd.Parameters.Add("XMLACTIVITY_IN", OracleDbType.Varchar2).Value = XMLACTIVITY;
        oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = Ecode;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public string INSERT_SECONDHALF_SENDTOEVALUATOR(string HRPMSID, string ASSCOMMENT, string ECODE)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPORC_SHSTATUSUPDATE_SET";
        oCmd.Parameters.Add("PMSID_IN", OracleDbType.Varchar2).Value = HRPMSID;
        oCmd.Parameters.Add("ASSCOMMENT_IN", OracleDbType.Varchar2).Value = ASSCOMMENT;
        oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public DataSet GETFHDIVHEADREQDETAILS(string SYKI, string ECODE, string NORMLEVEL)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_FHDIVHEADREQ_GET";
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = SYKI;
        oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("NORMLEVEL_IN", OracleDbType.Varchar2).Value = NORMLEVEL;
        oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_RATLST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_PERIOD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        return oDataMgmt.GetDataSet(oCmd);
        oCmd.Dispose();
    }

    public DataSet GETDESIGNATIONWEIGTAGE(string DWMTRANSID, string DESIGNATION, string SYKI)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_DESIGNATIONWEIGHTAGE_GET";
        oCmd.Parameters.Add("DWMTRANSID_IN", OracleDbType.Varchar2).Value = DWMTRANSID;
        oCmd.Parameters.Add("DESIGNATION_IN", OracleDbType.Varchar2).Value = DESIGNATION;
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = SYKI;
        oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        return oDataMgmt.GetDataSet(oCmd);
        oCmd.Dispose();
    }

    public string INSERTDESIGNATIONWEIGHTAGE(string dwntransid, string desigid, string syki, string addedby, string PARTAWEIGHT, string PARTBWEIGHT)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_DESIGNWEIGHTTMST_INSERT";
        oCmd.Parameters.Add("DWMTRANSID_IN", OracleDbType.Varchar2).Value = dwntransid;
        oCmd.Parameters.Add("DESIGNATIONID_IN", OracleDbType.Varchar2).Value = desigid;
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = syki;
        oCmd.Parameters.Add("PARTAWEIGHT_IN", OracleDbType.Varchar2).Value = PARTAWEIGHT;
        oCmd.Parameters.Add("PARTBWEIGHT_IN", OracleDbType.Varchar2).Value = PARTBWEIGHT;
        oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = addedby;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public DataSet GETPMSDESIGNATIONWEIGHT(string HRPMSID)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_PMSDESIGNATIONWT_GET";
        oCmd.Parameters.Add("PMSID_IN", OracleDbType.Varchar2).Value = HRPMSID;
        oCmd.Parameters.Add("CUR_DESIGWT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        return oDataMgmt.GetDataSet(oCmd);
        oCmd.Dispose();
    }

    public string FIRSTHALF_TWOWAYCOMM(string HRPMSID, string COMMENT, string ECODE, string twowayconfirmation)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_FH2WAYCONFIRM_INSERT";
        oCmd.Parameters.Add("HRPMSID_IN", OracleDbType.Varchar2).Value = HRPMSID;
        oCmd.Parameters.Add("COMMENT_IN", OracleDbType.Varchar2).Value = COMMENT;
        oCmd.Parameters.Add("TWOWAYCONFIRM_IN", OracleDbType.Varchar2).Value = twowayconfirmation;
        oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public string FIRSTHALFREVIEWER_SUBMIT(string HRPMSID, string BUTTONID, string ECODE, string XMLACTIVITY, string XMLCOMPETENCY, string FINALTOTALREVSCORE, string FINALRATING, string FINALFHSCORE, string FEEDBACKCOMMENT, string REVOTHACTSCORE)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_FHREVIEWER_INSERT";
        oCmd.Parameters.Add("HRPMSID_IN", OracleDbType.Varchar2).Value = HRPMSID;
        oCmd.Parameters.Add("BUTTONID_IN", OracleDbType.Varchar2).Value = BUTTONID;
        oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("XMLACTIVITY_IN", OracleDbType.Varchar2).Value = XMLACTIVITY;
        oCmd.Parameters.Add("XMLCOMPETENCY_IN", OracleDbType.Varchar2).Value = XMLCOMPETENCY;
        oCmd.Parameters.Add("FINALTTLREVSCORE_IN", OracleDbType.Varchar2).Value = FINALTOTALREVSCORE;
        oCmd.Parameters.Add("FINALRATING_IN", OracleDbType.Varchar2).Value = FINALRATING;
        oCmd.Parameters.Add("FINALFHSCORE_IN", OracleDbType.Varchar2).Value = FINALFHSCORE;
        oCmd.Parameters.Add("FEEDBACKCOMMENT_IN", OracleDbType.Varchar2).Value = FEEDBACKCOMMENT;
        oCmd.Parameters.Add("REVOTHACTSCORE_IN", OracleDbType.Varchar2).Value = REVOTHACTSCORE;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public string SECONDHALFEVALSUBMIT(string HRPMSID, string BUTTONID, string EVALOTHACTSCORE, string PARTAEVALCOMMENT, string XMLACTIVITY,
    string XMLCOMPETENCY, string PARTBEVALCOMMENT, string EVALFEEDBACKCOMMENT, string FLAGAUTH, string FINALTOTALEVALSCORE, string FINALRATING, string FINALSHSCORE, string ECODE, string FINALTOTALREVSCORE)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_SHEVALUATOR_INSERT";
        oCmd.Parameters.Add("HRPMSID_IN", OracleDbType.Varchar2).Value = HRPMSID;
        oCmd.Parameters.Add("BUTTONID_IN", OracleDbType.Varchar2).Value = BUTTONID;
        oCmd.Parameters.Add("EVALOTHACTSCORE_IN", OracleDbType.Varchar2).Value = EVALOTHACTSCORE;
        oCmd.Parameters.Add("PARTAEVALCOMMENT_IN", OracleDbType.Varchar2).Value = PARTAEVALCOMMENT;
        oCmd.Parameters.Add("XMLACTIVITY_IN", OracleDbType.Varchar2).Value = XMLACTIVITY;
        oCmd.Parameters.Add("XMLCOMPETENCY_IN", OracleDbType.Varchar2).Value = XMLCOMPETENCY;
        oCmd.Parameters.Add("PARTBEVALCOMMENT_IN", OracleDbType.Varchar2).Value = PARTBEVALCOMMENT;
        oCmd.Parameters.Add("EVALFEEDBACKCOMMENT_IN", OracleDbType.Varchar2).Value = EVALFEEDBACKCOMMENT;
        oCmd.Parameters.Add("FLAGAUTH_IN", OracleDbType.Varchar2).Value = FLAGAUTH;
        oCmd.Parameters.Add("FINALTOTALEVALSCORE_IN", OracleDbType.Varchar2).Value = FINALTOTALEVALSCORE;
        oCmd.Parameters.Add("FINALRATING_IN", OracleDbType.Varchar2).Value = FINALRATING;
        oCmd.Parameters.Add("FINALSHSCORE_IN", OracleDbType.Varchar2).Value = FINALSHSCORE;
        oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("FINALREVSCORE_IN", OracleDbType.Varchar2).Value = FINALTOTALREVSCORE;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public string SECONDHALFREVIEWER_SUBMIT(string HRPMSID, string BUTTONID, string ECODE, string XMLACTIVITY, string REVOTHACTSCORE,
        string XMLCOMPETENCY, string REVFEEDBACKCOMMENT, string FINALTOTALREVSCORE, string FINALRATING, string FINALSHSCORE)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_SHREVIEWER_INSERT";
        oCmd.Parameters.Add("HRPMSID_IN", OracleDbType.Varchar2).Value = HRPMSID;
        oCmd.Parameters.Add("BUTTONID_IN", OracleDbType.Varchar2).Value = BUTTONID;
        oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("XMLACTIVITY_IN", OracleDbType.Varchar2).Value = XMLACTIVITY;
        oCmd.Parameters.Add("REVOTHACTSCORE_IN", OracleDbType.Varchar2).Value = REVOTHACTSCORE;
        oCmd.Parameters.Add("XMLCOMPETENCY_IN", OracleDbType.Varchar2).Value = XMLCOMPETENCY;
        oCmd.Parameters.Add("REVFEEDBACKCOMMENT_IN", OracleDbType.Varchar2).Value = REVFEEDBACKCOMMENT;
        oCmd.Parameters.Add("FINALTOTALREVSCORE_IN", OracleDbType.Varchar2).Value = FINALTOTALREVSCORE;
        oCmd.Parameters.Add("FINALRATING_IN", OracleDbType.Varchar2).Value = FINALRATING;
        oCmd.Parameters.Add("FINALSHSCORE_IN", OracleDbType.Varchar2).Value = FINALSHSCORE;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public string SECONDHALFS_SELF_TWOWAYCOMM(string HRPMSID, string COMMENT, string ECODE, string twowayconfirmation)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_SH2WAYCONFIRM_INSERT";
        oCmd.Parameters.Add("HRPMSID_IN", OracleDbType.Varchar2).Value = HRPMSID;
        oCmd.Parameters.Add("COMMENT_IN", OracleDbType.Varchar2).Value = COMMENT;
        oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("TWOWAYCONFIRM_IN", OracleDbType.Varchar2).Value = twowayconfirmation;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public string NORMRATING_SUBMIT(string PMSID, string BUTTONID, string RATING, string ECODE)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_NORMRATING_INSERT";
        oCmd.Parameters.Add("HRPMSID_IN", OracleDbType.Varchar2).Value = PMSID;
        oCmd.Parameters.Add("BUTTONID_IN", OracleDbType.Varchar2).Value = BUTTONID;
        oCmd.Parameters.Add("RATING_IN", OracleDbType.Varchar2).Value = RATING;
        oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public string NORMALIZATION_FINALSUBMIT(string BUTTONID, string SYKI, string ECODE)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_NORM_FINALSUBMIT";
        oCmd.Parameters.Add("BUTTONID_IN", OracleDbType.Varchar2).Value = BUTTONID;
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = SYKI;
        oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public DataSet GETNORMRATINGDETAIL(string syki)
    {
        DataTable objDt = new DataTable();
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_NORMRATINGLIST_GET";
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = syki;
        oCmd.Parameters.Add("CUR_RATLST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        return oDataMgmt.GetDataSet(oCmd);
        oCmd.Dispose();
    }

    public string INSERTRATINGNORM(string xmlratlist, string SYKI, string ADDEDBY)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_RATINGNORMMST_INSERT";
        oCmd.Parameters.Add("XMLRATLIST_IN", OracleDbType.Varchar2).Value = xmlratlist;
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = SYKI;
        oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = ADDEDBY;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public DataSet GETKIOPERDIVDEPTSEC(string OPERATION, string DIVISION, string DEPARTMENT)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.GETOPER_DIV_DEPT_SEC";
        oCmd.Parameters.Add("OPER_IN", OracleDbType.Varchar2).Value = OPERATION;
        oCmd.Parameters.Add("DIVI_IN", OracleDbType.Varchar2).Value = DIVISION;
        oCmd.Parameters.Add("DEPT_IN", OracleDbType.Varchar2).Value = DEPARTMENT;
        oCmd.Parameters.Add("CUR_KI", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_OPER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_DIV", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_DEPT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_SEC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        return oDataMgmt.GetDataSet(oCmd);
        oCmd.Dispose();
    }

    public string FHNORM_REMARKSSUBMIT(string SUBMITBY, string PMSID, string REMARK, string ECODE)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_FHNORMREMARKS_INSERT";
        oCmd.Parameters.Add("SUBMITBY_IN", OracleDbType.Varchar2).Value = SUBMITBY;
        oCmd.Parameters.Add("PMSID_IN", OracleDbType.Varchar2).Value = PMSID;
        oCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = REMARK;
        oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public DataSet GET_NORMALIZATION_DIVDEPTSEC(string SYKI, string ECODE, string NORMTYPE, string NORMLEVEL)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_NORMDIVDEPTSEC_GET";
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = SYKI;
        oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("NORMTYPE_IN", OracleDbType.Varchar2).Value = NORMTYPE;
        oCmd.Parameters.Add("NORMLEVEL_IN", OracleDbType.Varchar2).Value = NORMLEVEL;
        oCmd.Parameters.Add("CUR_DIV", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_DEPT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_SEC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_DESIG", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_OPH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        return oDataMgmt.GetDataSet(oCmd);
        oCmd.Dispose();
    }

    public int CHECK_SUBMITBTNENABLED(DataSet ds, string status, string Startdate, string EndDate)
    {
        int CNTROWS = ds.Tables[0].Rows.Count;
        int STATUSCNT = 0;
        int STATUS2WAY = 0;
        for (int i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
        {
            if (ds.Tables[0].Rows[i]["MYRSTATUS"].ToString() == status)
            {
                STATUSCNT = STATUSCNT + 1;
                if (string.IsNullOrEmpty(ds.Tables[0].Rows[i]["MY2WAYCONFIRMATION"].ToString()))
                {
                    STATUS2WAY = STATUS2WAY + 1;
                }
            }
        }

        DateTime dateTime = DateTime.Now;
        if ((dateTime.Date < (Convert.ToDateTime(Startdate).Date) || dateTime.Date > (Convert.ToDateTime(EndDate).Date)))
        { STATUSCNT = STATUSCNT + 1; }

        if (CNTROWS == STATUSCNT)
        {
            if (STATUS2WAY > 0)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }
        else
        {
            if (STATUS2WAY > 0)
            {
                return 2;
            }
            else
            {
                return 0;
            }
        }
    }

    public DataSet GETFHOPERATINGHEADREQDETAILS(string SYKI, string ECODE, string NORMLEVEL)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_FHOPERATINGHEADREQ_GET";
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = SYKI;
        oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("NORMLEVEL_IN", OracleDbType.Varchar2).Value = NORMLEVEL;
        oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_RATLST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_PERIOD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        return oDataMgmt.GetDataSet(oCmd);
        oCmd.Dispose();
    }

    public DataSet GETHRPMSNORMALIZATIONREPORT(string SYKI, string OPERATION, string DIVISION, string DEPARTMENT, string SECTION, string DESIGNATION, string ECODE, string ENAME, string SITE, string RATING, string DIVHEADECODE, string OPHEADECODE)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_HRPMSNORMALIZREPORT_GET";
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = SYKI;
        oCmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = OPERATION;
        oCmd.Parameters.Add("DIVISION_IN", OracleDbType.Varchar2).Value = DIVISION;
        oCmd.Parameters.Add("DEPARTMENT_IN", OracleDbType.Varchar2).Value = DEPARTMENT;
        oCmd.Parameters.Add("SECTION_IN", OracleDbType.Varchar2).Value = SECTION;
        oCmd.Parameters.Add("DESIGNATION_IN", OracleDbType.Varchar2).Value = DESIGNATION;
        oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("ENAME_IN", OracleDbType.Varchar2).Value = ENAME;
        oCmd.Parameters.Add("SITE_IN", OracleDbType.Varchar2).Value = SITE;
        oCmd.Parameters.Add("RATING_IN", OracleDbType.Varchar2).Value = RATING;
        oCmd.Parameters.Add("DHECODE_IN", OracleDbType.Varchar2).Value = DIVHEADECODE;
        oCmd.Parameters.Add("OHECODE_IN", OracleDbType.Varchar2).Value = OPHEADECODE;
        oCmd.Parameters.Add("CUR_PMSRPT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        return oDataMgmt.GetDataSet(oCmd);
        oCmd.Dispose();
    }

    public DataSet GETDIVHEAD_OPHEAD(string SYKI)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_NORMDIVHEAD_OPHEAD_GET";
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = SYKI;
        oCmd.Parameters.Add("CUR_DIVHEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_OPHEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        return oDataMgmt.GetDataSet(oCmd);
        oCmd.Dispose();
    }

    public string UploadHRRating(string ADEDDBY, string SYKI, string ECODE, string HRRATING)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPORC_UPLOADHRRATING_SET";
        oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = ADEDDBY;
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = SYKI;
        oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("HRRATING_IN", OracleDbType.Varchar2).Value = HRRATING;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public string RemoveSpecialCharacters(string str)
    {
        return Regex.Replace(str, "[^a-zA-Z0-9_. ]+", "", RegexOptions.Compiled);
    }

    public DataTable NORMALIZATIONPERIOD_GET(string strki)
    {
        DataTable objDt = new DataTable();
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_NORMALIZATIONPERIOD_GET";
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = strki;
        oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        objDt = oDataMgmt.GetDataTable(oCmd);
        return (objDt);
    }

    public string NORMALIZATIONPERIOD_SET(string strki, string strdivfhs, string strdivfhe, string stropfhs, string stropfhe, string strdivshs, string strdivshe, string stropshs, string stropshe, string straddedby)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPORC_NORMALIZATIONPERIOD_SET";
        oCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = strki;
        oCmd.Parameters.Add("DIVFHSTARTDATE_IN", OracleDbType.Varchar2).Value = strdivfhs;
        oCmd.Parameters.Add("DIVFHENDDATE_IN", OracleDbType.Varchar2).Value = strdivfhe;
        oCmd.Parameters.Add("OPFHSTARTDATE_IN", OracleDbType.Varchar2).Value = stropfhs;
        oCmd.Parameters.Add("OPFHENDDATE_IN", OracleDbType.Varchar2).Value = stropfhe;
        oCmd.Parameters.Add("DIVSHSTARTDATE_IN", OracleDbType.Varchar2).Value = strdivshs;
        oCmd.Parameters.Add("DIVSHENDDATE_IN", OracleDbType.Varchar2).Value = strdivshe;
        oCmd.Parameters.Add("OPSHSTARTDATE_IN", OracleDbType.Varchar2).Value = stropshs;
        oCmd.Parameters.Add("OPSHENDDATE_IN", OracleDbType.Varchar2).Value = stropshe;
        oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = straddedby;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public string NORMALIZATION_UPLOADEXCEL(string ASSOCIATEID, string DIVHEADRATING, string SUBMITTYPE, string ADDEDBY)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPORC_FHNORMRATINGUPLOAD_SET";
        oCmd.Parameters.Add("ASSOCIATEID_IN", OracleDbType.Varchar2).Value = ASSOCIATEID;
        oCmd.Parameters.Add("RATING_IN", OracleDbType.Varchar2).Value = DIVHEADRATING;
        oCmd.Parameters.Add("SUBMITTYPE_IN", OracleDbType.Varchar2).Value = SUBMITTYPE;
        oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = ADDEDBY;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public void GETEVALREVSCORE(string HRPMSID, out string strFH_EVAL_PARTASCORE, out string strFH_REV_PARTASCORE, out string strSH_EVAL_PARTASCORE, out string strSH_REV_PARTASCORE,
       out string strFH_EVAL_PARTBSCORE, out string strFH_REV_PARTBSCORE, out string strSH_EVAL_PARTBSCORE, out string strSH_REV_PARTABCORE)
    {
        //HRPMS pms = new HRPMS();
        DataSet ds = GETGOALSETTINGDETAIL(HRPMSID);

        strFH_EVAL_PARTASCORE = "";
        strFH_REV_PARTASCORE = "";
        strSH_EVAL_PARTASCORE = "";
        strSH_REV_PARTASCORE = "";
        strFH_EVAL_PARTBSCORE = "";
        strFH_REV_PARTBSCORE = "";
        strSH_EVAL_PARTBSCORE = "";
        strSH_REV_PARTABCORE = "";
        //VARIABLE FOR PART A
        decimal FH_EVAL_PARTASCORE = 0;
        decimal FH_REV_PARTASCORE = 0;
        decimal SH_EVAL_PARTASCORE = 0;
        decimal SH_REV_PARTASCORE = 0;

        //VARIABLE FOR PART B
        decimal FH_EVAL_PARTBSCORE = 0;
        decimal FH_REV_PARTBSCORE = 0;
        decimal SH_EVAL_PARTBSCORE = 0;
        decimal SH_REV_PARTABCORE = 0;

        //VARIABLE OTHERACTIVITY WEIGHTAGE & SCORE
        int OTHERACTIVITY_WEIGHT = 0;
        int FH_EVAL_PARTASCORE_OTHACT = 0;
        int FH_REV_PARTASCORE_OTHACT = 0;
        int SH_EVAL_PARTASCORE_OTHACT = 0;
        int SH_REV_PARTASCORE_OTHACT = 0;

        //VARIABLE FOR WEIGHTAGE
        string PARTAWT = string.Empty;
        string PARTBWT = string.Empty;

        int PARTBROWCNT = 0;

        //Get PART A AND PART B WEIGHTAGE
        if (ds.Tables[0].Rows.Count > 0)
        {
            PARTAWT = ds.Tables[0].Rows[0]["PARTAWEIGHTAGE"].ToString();
            PARTBWT = ds.Tables[0].Rows[0]["PARTBWEIGHTAGE"].ToString();
        }

        //GET OTHER ACTIVITY SCORES
        if (ds.Tables[1].Rows.Count > 0)
        {
            FH_EVAL_PARTASCORE_OTHACT = Convert.ToInt32(Encryption.Decrypt(ds.Tables[1].Rows[0]["FHEVALSCORE"].ToString()));
            FH_REV_PARTASCORE_OTHACT = Convert.ToInt32(Encryption.Decrypt(ds.Tables[1].Rows[0]["FHREVSCORE"].ToString()));
            SH_EVAL_PARTASCORE_OTHACT = Convert.ToInt32(Encryption.Decrypt(ds.Tables[1].Rows[0]["SHEVALSCORE"].ToString()));
            SH_REV_PARTASCORE_OTHACT = Convert.ToInt32(Encryption.Decrypt(ds.Tables[1].Rows[0]["SHREVSCORE"].ToString()));
        }

        //Calculation Score 
        if (ds.Tables[2].Rows.Count > 0)
        {
            DataView dv_OtherActivity = new DataView(ds.Tables[2]);
            dv_OtherActivity.RowFilter = "ACTIVITYTYPE=2";
            if (dv_OtherActivity.Count > 0)
            {
                OTHERACTIVITY_WEIGHT = Convert.ToInt32(dv_OtherActivity[0]["WEIGHTAGE"]);
            }

            //Calculate PART A Score
            decimal PARTASCORE = 0;
            DataView DVPARTA = new DataView(ds.Tables[2]);
            DVPARTA.RowFilter = "ACTIVITYTYPE=1";
            for (int i = 0; i < DVPARTA.Count; i++)
            {
                FH_EVAL_PARTASCORE = FH_EVAL_PARTASCORE + ((Convert.ToDecimal(DVPARTA[i]["WEIGHTAGE"]) * Convert.ToDecimal(Encryption.Decrypt(DVPARTA[i]["MYEVLTSCORE"].ToString()))) / 100);
                FH_REV_PARTASCORE = FH_REV_PARTASCORE + ((Convert.ToDecimal(DVPARTA[i]["WEIGHTAGE"]) * Convert.ToDecimal(Encryption.Decrypt(DVPARTA[i]["MYRVWRSCORE"].ToString()))) / 100);
                SH_EVAL_PARTASCORE = SH_EVAL_PARTASCORE + ((Convert.ToDecimal(DVPARTA[i]["WEIGHTAGE"]) * Convert.ToDecimal(Encryption.Decrypt(DVPARTA[i]["FYEVLTSCORE"].ToString()))) / 100);
                SH_REV_PARTASCORE = SH_REV_PARTASCORE + ((Convert.ToDecimal(DVPARTA[i]["WEIGHTAGE"]) * Convert.ToDecimal(Encryption.Decrypt(DVPARTA[i]["FYRVWRSCORE"].ToString()))) / 100);
            }
            FH_EVAL_PARTASCORE = FH_EVAL_PARTASCORE + ((Convert.ToDecimal(OTHERACTIVITY_WEIGHT) * Convert.ToInt32(FH_EVAL_PARTASCORE_OTHACT)) / 100);
            FH_REV_PARTASCORE = FH_REV_PARTASCORE + ((Convert.ToDecimal(OTHERACTIVITY_WEIGHT) * Convert.ToInt32(FH_REV_PARTASCORE_OTHACT)) / 100);
            SH_EVAL_PARTASCORE = SH_EVAL_PARTASCORE + ((Convert.ToDecimal(OTHERACTIVITY_WEIGHT) * Convert.ToInt32(SH_EVAL_PARTASCORE_OTHACT)) / 100);
            SH_REV_PARTASCORE = SH_REV_PARTASCORE + ((Convert.ToDecimal(OTHERACTIVITY_WEIGHT) * Convert.ToInt32(SH_REV_PARTASCORE_OTHACT)) / 100);

            strFH_EVAL_PARTASCORE = FH_EVAL_PARTASCORE.ToString();
            strFH_REV_PARTASCORE = FH_REV_PARTASCORE.ToString();
            strSH_EVAL_PARTASCORE = SH_EVAL_PARTASCORE.ToString();
            strSH_REV_PARTASCORE = SH_REV_PARTASCORE.ToString();
            ////Calculate PART B Score
            for (int i = 0; i < ds.Tables[4].Rows.Count; i++)
            {
                FH_EVAL_PARTBSCORE = FH_EVAL_PARTBSCORE + Convert.ToDecimal(Encryption.Decrypt(ds.Tables[4].Rows[i]["MYEVLTSCORE"].ToString()));
                FH_REV_PARTBSCORE = FH_REV_PARTBSCORE + Convert.ToDecimal(Encryption.Decrypt(ds.Tables[4].Rows[i]["MYRVWRSCORE"].ToString()));
                SH_EVAL_PARTBSCORE = SH_EVAL_PARTBSCORE + Convert.ToDecimal(Encryption.Decrypt(ds.Tables[4].Rows[i]["FYEVLTSCORE"].ToString()));
                SH_REV_PARTABCORE = SH_REV_PARTABCORE + Convert.ToDecimal(Encryption.Decrypt(ds.Tables[4].Rows[i]["FYRVWRSCORE"].ToString()));

                PARTBROWCNT = PARTBROWCNT + 1;
            }
            FH_EVAL_PARTBSCORE = FH_EVAL_PARTBSCORE / PARTBROWCNT;
            FH_REV_PARTBSCORE = FH_REV_PARTBSCORE / PARTBROWCNT;
            SH_EVAL_PARTBSCORE = SH_EVAL_PARTBSCORE / PARTBROWCNT;
            SH_REV_PARTABCORE = SH_REV_PARTABCORE / PARTBROWCNT;

            strFH_EVAL_PARTBSCORE = FH_EVAL_PARTBSCORE.ToString();
            strFH_REV_PARTBSCORE = FH_REV_PARTBSCORE.ToString();
            strSH_EVAL_PARTBSCORE = SH_EVAL_PARTBSCORE.ToString();
            strSH_REV_PARTABCORE = SH_REV_PARTABCORE.ToString();
        }
    }

    public DataSet GETANNUALRATINGMATRIX(string SYKI, string RATINGID, string STATUS)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_ANNUALRATINGMATRIX_GET";
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = SYKI;
        oCmd.Parameters.Add("RATINGID_IN", OracleDbType.Varchar2).Value = RATINGID;
        oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
        oCmd.Parameters.Add("CUR_RATLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        return oDataMgmt.GetDataSet(oCmd);
        oCmd.Dispose();
    }

    public string INSERTANNUALRATING(string RATINGID, string SYKI, string FIRSTHALF, string SECONDHALF, string ANNUAL, string STATUS, string ADDEDBY)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_ANNUALRATINGHR_INSERT";
        oCmd.Parameters.Add("RATINGID_IN", OracleDbType.Varchar2).Value = RATINGID;
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = SYKI;
        oCmd.Parameters.Add("FIRSTHALFRAT", OracleDbType.Varchar2).Value = FIRSTHALF;
        oCmd.Parameters.Add("SECONDHALFRAT", OracleDbType.Varchar2).Value = SECONDHALF;
        oCmd.Parameters.Add("ANNUALRAT", OracleDbType.Varchar2).Value = ANNUAL;
        oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
        oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = ADDEDBY;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
    }

    public DataSet GETSHDIVHEADREQDETAILS(string SYKI, string ECODE, string NORMLEVEL)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_SHDIVHEADREQ_GET";
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = SYKI;
        oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("NORMLEVEL_IN", OracleDbType.Varchar2).Value = NORMLEVEL;
        oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_RATLST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_PERIOD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        return oDataMgmt.GetDataSet(oCmd);
        oCmd.Dispose();
    }

    public DataSet GETSHOPERATINGHEADREQDETAILS(string SYKI, string ECODE, string NORMLEVEL)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_SHOPERATINGHEADREQ_GET";
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = SYKI;
        oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("NORMLEVEL_IN", OracleDbType.Varchar2).Value = NORMLEVEL;
        oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_RATLST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_PERIOD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        return oDataMgmt.GetDataSet(oCmd);
        oCmd.Dispose();
    }

    public int CHECK_SHSUBMITBTNENABLED(DataSet ds, string status, string Startdate, string EndDate)
    {
        int CNTROWS = ds.Tables[0].Rows.Count;
        int STATUSCNT = 0;
        int STATUS2WAY = 0;
        for (int i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
        {
            if (ds.Tables[0].Rows[i]["FYRSTATUS"].ToString() == status)
            {
                STATUSCNT = STATUSCNT + 1;
                if (string.IsNullOrEmpty(ds.Tables[0].Rows[i]["FY2WAYCONFIRMATION"].ToString()))
                {
                    STATUS2WAY = STATUS2WAY + 1;
                }
            }

        }

        DateTime dateTime = DateTime.Now;
        if ((dateTime.Date < (Convert.ToDateTime(Startdate).Date) || dateTime.Date > (Convert.ToDateTime(EndDate).Date)))
        { STATUSCNT = STATUSCNT + 1; }

        if (CNTROWS == STATUSCNT)
        {
            if (STATUS2WAY > 0)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }
        else
        {
            if (STATUS2WAY > 0)
            {
                return 2;
            }
            else
            {
                return 0;
            }
        }
    }

    public string UploadSHHRRating(string ADEDDBY, string SYKI, string ECODE, string HRRATING, string ANNUALRATING, string PROMOTION)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPORC_UPLOADSHHRRATING_SET";
        oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = ADEDDBY;
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = SYKI;
        oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("HRRATING_IN", OracleDbType.Varchar2).Value = HRRATING;
        oCmd.Parameters.Add("ANNUALRATING_IN", OracleDbType.Varchar2).Value = ANNUALRATING;
        oCmd.Parameters.Add("PROMOTION_IN", OracleDbType.Varchar2).Value = PROMOTION;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public DataSet GETHRPMSSHNORMALIZATIONREPORT(string SYKI, string OPERATION, string DIVISION, string DESIGNATION, string ECODE, string ENAME, string SITE, string RATING, string DIVHEADECODE, string OPHEADECODE)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_HRPMSSHNORMALIZRPT_GET";
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = SYKI;
        oCmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = OPERATION;
        oCmd.Parameters.Add("DIVISION_IN", OracleDbType.Varchar2).Value = DIVISION;
        oCmd.Parameters.Add("DESIGNATION_IN", OracleDbType.Varchar2).Value = DESIGNATION;
        oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("ENAME_IN", OracleDbType.Varchar2).Value = ENAME;
        oCmd.Parameters.Add("SITE_IN", OracleDbType.Varchar2).Value = SITE;
        oCmd.Parameters.Add("RATING_IN", OracleDbType.Varchar2).Value = RATING;
        oCmd.Parameters.Add("DHECODE_IN", OracleDbType.Varchar2).Value = DIVHEADECODE;
        oCmd.Parameters.Add("OHECODE_IN", OracleDbType.Varchar2).Value = OPHEADECODE;
        oCmd.Parameters.Add("CUR_PMSRPT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        return oDataMgmt.GetDataSet(oCmd);
        oCmd.Dispose();
    }

    public string ANNUALRATING_SUBMIT(string PMSID, string BUTTONID, string RATING, string ECODE)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_ANNUALRATING_INSERT";
        oCmd.Parameters.Add("HRPMSID_IN", OracleDbType.Varchar2).Value = PMSID;
        oCmd.Parameters.Add("BUTTONID_IN", OracleDbType.Varchar2).Value = BUTTONID;
        oCmd.Parameters.Add("RATING_IN", OracleDbType.Varchar2).Value = RATING;
        oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public string PROMOTION_SUBMIT(string PMSID, string BUTTONID, string PROMOTION, string ECODE)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_PROMOTION_INSERT";
        oCmd.Parameters.Add("HRPMSID_IN", OracleDbType.Varchar2).Value = PMSID;
        oCmd.Parameters.Add("BUTTONID_IN", OracleDbType.Varchar2).Value = BUTTONID;
        oCmd.Parameters.Add("PROMOTION_IN", OracleDbType.Varchar2).Value = PROMOTION;
        oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public DataSet GETANNUALRATINGREPORT(string SYKI, string OPERATION, string DIVISION, string DESIGNATION, string ECODE, string ENAME)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_ANNUALRATINGRPT_GET";
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = SYKI;
        oCmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = OPERATION;
        oCmd.Parameters.Add("DIVISION_IN", OracleDbType.Varchar2).Value = DIVISION;
        oCmd.Parameters.Add("DESIGNATION_IN", OracleDbType.Varchar2).Value = DESIGNATION;
        oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("ENAME_IN", OracleDbType.Varchar2).Value = ENAME;
        oCmd.Parameters.Add("CUR_PMSRPT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        return oDataMgmt.GetDataSet(oCmd);
        oCmd.Dispose();
    }

    public DataSet GETTWOWAYCOMMUNICATIONRPT(string SYKI, string OPERATION, string DIVISION, string DEPARTMENT, string SECTION, string DESIGNATION, string ECODE, string ENAME)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_TWOWAYCOMMRPT_GET";
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = SYKI;
        oCmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = OPERATION;
        oCmd.Parameters.Add("DIVISION_IN", OracleDbType.Varchar2).Value = DIVISION;
        oCmd.Parameters.Add("DEPARTMENT_IN", OracleDbType.Varchar2).Value = DEPARTMENT;
        oCmd.Parameters.Add("SECTION_IN", OracleDbType.Varchar2).Value = SECTION;
        oCmd.Parameters.Add("DESIGNATION_IN", OracleDbType.Varchar2).Value = DESIGNATION;
        oCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("ENAME_IN", OracleDbType.Varchar2).Value = ENAME;
        oCmd.Parameters.Add("CUR_PMSRPT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        return oDataMgmt.GetDataSet(oCmd);
        oCmd.Dispose();
    }

    public string NORMALIZATIONREMARKS_SUBMIT(string PMSID, string BUTTONID, string REMARKS, string ECODE)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_SHNORMREMARKS_INSERT";
        oCmd.Parameters.Add("HRPMSID_IN", OracleDbType.Varchar2).Value = PMSID;
        oCmd.Parameters.Add("BUTTONID_IN", OracleDbType.Varchar2).Value = BUTTONID;
        oCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = REMARKS;
        oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
        oDataMgmt.ExecuteQuery(oCmd);
        string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
        return MSG;
        oCmd.Dispose();
    }

    public DataSet GETELIGIBILITYRPT(string SYKI, string OPERATION, string DESIGNATION)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_ELIGIBILITYRPT_GET";
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = SYKI;
        oCmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = OPERATION;
        oCmd.Parameters.Add("DESIGNATION_IN", OracleDbType.Varchar2).Value = DESIGNATION;
        oCmd.Parameters.Add("CUR_ELIGRPT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        return oDataMgmt.GetDataSet(oCmd);
        oCmd.Dispose();
    }

    public DataSet GET_PMSDIVISIONHEAD(string SYKI, string OPHEAD)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_PMSDIVISIONHEAD_GET";
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = SYKI;
        oCmd.Parameters.Add("OPHEDAD_IN", OracleDbType.Varchar2).Value = OPHEAD;
        oCmd.Parameters.Add("CUR_DIVLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        return oDataMgmt.GetDataSet(oCmd);
        oCmd.Dispose();
    }

    public DataTable GET_Compantancy_Report(string Ki, string strdesg)
    {
        DataTable ObjRep = new DataTable();
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandText = "PKG_HR_PMS.SPROC_PMSPARTBREPORT_GET";
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = Ki;
        oCmd.Parameters.Add("OPHEDAD_IN", OracleDbType.Varchar2).Value = strdesg;
        oCmd.Parameters.Add("CUR_DIVLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        ObjRep = oDataMgmt.GetDataTable(oCmd);
        return (ObjRep);
    }

    public DataTable GET_Reward_Report(string Ki, string empid)
    {
        DataTable ObjRep = new DataTable();
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandText = "PKG_HR_PMS.SPROC_PMS_REWARDREPORT_GET";
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = Ki;
        oCmd.Parameters.Add("EMP_IN", OracleDbType.Varchar2).Value = empid;
        oCmd.Parameters.Add("CUR_REPLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        ObjRep = oDataMgmt.GetDataTable(oCmd);
        return (ObjRep);
    }

    public DataTable GETDESIGNATIONForReport()
    {
        DataTable objDt = new DataTable();
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_PMS_REPORT_DES_GET";
        oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        objDt = oDataMgmt.GetDataTable(oCmd);
        return (objDt);
    }

    public DataSet GETSHDIVPLANDETAILS(string ECODE, string SYKI)
    {
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_HR_PMS.SPROC_PMS_PLANDIV_GET";
        oCmd.Parameters.Add("OPECODE_IN", OracleDbType.Varchar2).Value = ECODE;
        oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = SYKI;
        oCmd.Parameters.Add("CUR_GETJELIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_GETAMLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        return oDataMgmt.GetDataSet(oCmd);
        oCmd.Dispose();
    }

}
