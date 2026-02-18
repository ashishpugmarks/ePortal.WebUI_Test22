using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace ePortal.Persistence.Admin.Services
{
    public class Department:IDepartment
    {
        #region"Instance variables"
        private readonly IDataManagement oDataMgmt;
        private readonly IConnectionString objCnStr;
        DataSet ds = new DataSet();
        DataRow[] _datarow;

        public Department(IDataManagement _oDataMgmt, IConnectionString _objCnStr)
        {
            oDataMgmt = _oDataMgmt;
            objCnStr = _objCnStr;
        }


        
        //DataManagement oDataMgmt = new DataManagement();
        #endregion

        #region"Get Functions"
        public DataSet GetAllDepartment()
        {
            OracleCommand oCmd = new OracleCommand();
            DataSet ds;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MASTERS.SPROC_DEPARTMENT_GET";
            oCmd.Parameters.Add("CUR_DEPTLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }

        public DataSet GetAllDepartmentCoordinators()
        {
            OracleCommand oCmd = new OracleCommand();
            DataSet ds;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MASTERS.SPROC_DEPTCOORDINATOR_GET";
            oCmd.Parameters.Add("CUR_DEPTLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }
        public DataSet GetCompleteDepartmentList()
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
                    strSql = " select distinct a.DEPTID as addepartmentid,a.DEPT as deptname, a.OPID as advpid,a.DIVID as addivisionid from vw_orgleveldetails a where a.DEPTID is not null ";
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
        public DataSet GetFilterDepartment(int OperationID, int divisionID)
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
                    //strSql = "SELECT  addepartmentid, descrip as deptname ";
                    //strSql = strSql + " FROM addepartment WHERE";
                    //if (OperationID != 0)
                    //    strSql = strSql + " advpid=" + OperationID.ToString() + "and";
                    //if (divisionID != 0)
                    //    strSql = strSql + " addivisionid=" + divisionID.ToString() + "and";
                    //strSql = strSql + " ACTIVE= 1 ORDER BY descrip";

                    strSql = "SELECT DISTINCT  D.DEPTID AS addepartmentid,D.DEPT AS deptname ";
                    strSql = strSql + " FROM VW_ORGLEVELDETAILS D WHERE 1= 1 ";
                    if (OperationID != 0)
                        strSql = strSql + " and D.OPID=" + OperationID.ToString() + " ";
                    if (divisionID != 0)
                        strSql = strSql + " and D.DIVID=" + divisionID.ToString() + " ";
                    strSql = strSql + " and D.DEPTID IS NOT NULL ORDER BY D.DEPT";

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(ds);
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
        public DataSet GetFilterDepartment(int OperationID, int divisionID, String strKI)
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
                    strSql = "SELECT distinct DEPTID AS addepartmentid, DEPT AS deptname ,ACTIVE ";
                    strSql = strSql + " FROM VW_ORGLEVELDETAILS_KI WHERE 1 = 1 ";
                    if (OperationID != 0)
                        strSql = strSql + " and OPID=" + OperationID.ToString() + " ";
                    if (divisionID != 0)
                        strSql = strSql + " and DIVID=" + divisionID.ToString() + " ";
                    strSql = strSql + " and sykiid = " + strKI + " AND DEPTID IS NOT NULL ORDER BY DEPT";
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(ds);
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
        public DataSet GetAllADVP()
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
                    strSql = " SELECT distinct A.ADORGLEVELID AS ADVPID,A.LEVELDESCRIP AS DESCRIP,A.LEVELDESCRIP AS INITIALDESCRIP FROM VW_OPERATION A WHERE A.ACTIVE = 1 ";
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(ds);
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
        public DataSet GetAllADVP(String strKI)
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
                    strSql = " SELECT A.ADORGLEVELID AS ADVPID,A.LEVELDESCRIP AS DESCRIP,A.LEVELDESCRIP AS INITIALDESCRIP,A.ACTIVE FROM VW_OPERATION A WHERE A.SYKIID = " + strKI;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(ds);
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
        public DataSet GetAllDesignation()
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
                    strSql = " select desg.addesignationid,desg.descrip from addesignation desg where desg.active=1 order by descrip";
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(ds);
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
        public DataSet GetDesignationForOJT()
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
                    strSql = " SELECT desg.addesignationid,desg.descrip FROM ADDESIGNATION DESG WHERE DESG.GROUPLEVEL IS NOT NULL ORDER BY DESG.DISPLAYORDER";
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(ds);
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
        //public string getConnectingString()
        //{
        //    string strCn = string.Empty;
        //    strCn = ConfigurationManager.ConnectionStrings["cnConn"].ConnectionString;
        //    return strCn;
        //}
        public DataSet getDetails(string addepartmentid)
        {

            OracleCommand oCmd = new OracleCommand();
            DataSet ds;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MASTERS.SPROC_DEPARTMENTBYID_GET";
            oCmd.Parameters.Add("DEPTARTMENT_ID", OracleDbType.Varchar2).Value = addepartmentid;
            oCmd.Parameters.Add("CUR_DEPTLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);

        }
        public DataTable GetEmployeeDetails(int EmpCode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                string qry = string.Empty;
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    DataTable dt = new DataTable();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_MASTERS.SPROC_EMPLOYEEDETAILS_GET";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = EmpCode;
                    objCmd.Parameters.Add("CUR_EMPDGET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(dt);
                    return dt;
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
        public DataSet GetDesignationForShowSkill(int GroupLevel)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                string qry = string.Empty;
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    DataSet ds = new DataSet();
                    objCmd.Connection = objCn;
                    if (GroupLevel >= 14 && GroupLevel <= 15) //(AE TO EX)
                    {
                        qry = "SELECT DESG.ADDESIGNATIONID,DESG.DESCRIP FROM  ADDESIGNATION DESG WHERE  DESG.DISPLAYORDER >=13 AND DESG.DISPLAYORDER <=15 AND GROUPLEVEL>0";
                    }
                    else if (GroupLevel >= 10 && GroupLevel <= 13) //(SE TO SM)
                    {
                        qry = "SELECT DESG.ADDESIGNATIONID,DESG.DESCRIP FROM  ADDESIGNATION DESG WHERE  DESG.DISPLAYORDER >=9 AND GROUPLEVEL>0";
                    }
                    else if (GroupLevel >= 1 && GroupLevel <= 9) //(SM TO VP)
                    {
                        qry = "SELECT DESG.ADDESIGNATIONID,DESG.DESCRIP FROM  ADDESIGNATION DESG WHERE  DESG.DISPLAYORDER >=1 AND GROUPLEVEL>0";
                    }
                    objCmd.CommandText = qry;
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(ds);
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
        public DataSet GetAssociateDepartment(string strEmpCode)
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
                    //strSql = " Select a.addepartmentid, a.descrip as deptname, a.initialdescrip, " +
                    //         " a.dateadded, a.active  from addepartment a " +
                    //         " left join addepthead DH ON DH.ADDEPARTMENTID = A.ADDEPARTMENTID AND DH.ACTIVE = 1 " +
                    //         " where a.active = 1 AND DH.departmentheadid = " + strEmpCode + " order by a.descrip";

                    strSql = " SELECT distinct A.ADORGLEVELID AS ADDEPARTMENTID,A.LEVELDESCRIP AS DEPTNAME,A.LEVELDESCRIP AS INITIALDESCRIP, " +
                             " A.DATEADDED AS DATEADDED,A.ACTIVE " +
                             " FROM VW_DEPARTMENT A " +
                             " LEFT JOIN ADORGLEVELHEAD DH ON DH.ADORGLEVELID = A.ADORGLEVELID AND DH.ISACTIVE = 1 " +
                             " WHERE A.ACTIVE = 1 AND DH.ADEMPCODE = " + strEmpCode + " ORDER BY A.LEVELDESCRIP  ";

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
        #endregion

        #region"Insert/Update Function"
        public int AddDepartment(string strDepDesc, string strDepIniDesc, string strEmpCode, string strDivisionID, string strStatus, string strCreatedBy, string strID, string strOperationID)
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

                    string strSql = "PKG_MASTERS.SPROC_UPDATE_DEPARTMENT";
                    objCmd.Parameters.Add("ADDEPARTMENTID_IN", OracleDbType.Varchar2).Value = strID;
                    objCmd.Parameters.Add("DESCRIP_IN", OracleDbType.Varchar2).Value = strDepDesc;
                    objCmd.Parameters.Add("INITIALDESCRIP_IN", OracleDbType.Varchar2).Value = strDepIniDesc;
                    objCmd.Parameters.Add("ADDIVISIONID_IN", OracleDbType.Varchar2).Value = strDivisionID;
                    objCmd.Parameters.Add("ADOPERATIONID_IN", OracleDbType.Varchar2).Value = strOperationID;
                    objCmd.Parameters.Add("BY_IN", OracleDbType.Varchar2).Value = strCreatedBy;
                    objCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = strStatus;
                    objCmd.Parameters.Add("DEPARTMENTHEADID_IN", OracleDbType.Varchar2).Value = strEmpCode;
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


        //INSERT-UPDATAE ORGANISATION CHART------------
        public int AddOrganisationChart(string strDescription, string strFilename, string strAddedby,
            string strStatus, string strModifyby, string strOrgchartId, string strFlag)
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

                    string strSql = "PKG_MASTERS.SPROC_SET_ORGCHART";
                    objCmd.Parameters.Add("DESCRIPTION_IN", OracleDbType.Varchar2).Value = strDescription;
                    objCmd.Parameters.Add("FILENAME_IN", OracleDbType.Varchar2).Value = strFilename;
                    objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = strAddedby;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
                    objCmd.Parameters.Add("MODIFEDBY_IN", OracleDbType.Varchar2).Value = strModifyby;
                    objCmd.Parameters.Add("ORGCHARTID_IN", OracleDbType.Varchar2).Value = strOrgchartId;
                    objCmd.Parameters.Add("FLAG_IN", OracleDbType.Varchar2).Value = strFlag;
                    objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2).Value = ParameterDirection.Output;
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


        //get ORGANISATION CHART INFO------------
        public DataTable GetOrganisationChart()
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MASTERS.SPROC_GET_ORGCHART";
            oCmd.Parameters.Add("CUR_VPLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            DataTable dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        #endregion
    }
}
