using ePortal.Persistence;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using Oracle.ManagedDataAccess.Client;
using System.Data;

/// <summary>
/// Summary description for SearchEmp
/// </summary>
public class SearchEmp : ISearchEmp
{
    DataSet objDs;

    private readonly IConnectionString _conn;
    private readonly IDataManagement oDataMgmt;


    public SearchEmp(IConnectionString conn, IDataManagement _oDataMgmt)
    {
        oDataMgmt = _oDataMgmt;
        _conn = conn;
    }

    /// <summary>
    /// GET ALL EMPLOYEE DATA
    /// </summary>
    /// <returns></returns>
    public DataSet GetAllEmployee()
    {
        objDs = new DataSet();
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_SEARCHEMPLOYEE.SPROC_AD_GETALLEMPLOYEE";
        oCmd.Parameters.Add("CUR_EMPDTL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        //GET DATA FROM DATA ACCESS LAYER
        objDs = oDataMgmt.GetDataSet(oCmd);
        return (objDs);
    }

    /// <summary>
    /// GET OFFICIAL DETAIL
    /// </summary>
    /// <param name="strEmpId"></param>
    /// <returns></returns>
    public DataSet OfficialDetail(string strEmpId)
    {
        objDs = new DataSet();
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_SEARCHEMPLOYEE.SPROC_AD_OFFICIALDETAIL";
        oCmd.Parameters.Add("TRANSID_IN", OracleDbType.Int32).Value = strEmpId;
        oCmd.Parameters.Add("CUR_EMPDTL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        //GET DATA FROM DATA ACCESS LAYER
        objDs = oDataMgmt.GetDataSet(oCmd);
        return (objDs);
    }

    /// <summary>
    /// GET ALL THE DIVISION WITH OUT ANY CONDITION
    /// </summary>
    /// <returns></returns>
    public DataSet get_AllDiv()
    {
        objDs = new DataSet();
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_COMMONMETHOD.SPROC_DIVISION_GET";
        oCmd.Parameters.Add("ADVPID_IN", OracleDbType.Varchar2).Value = null;
        oCmd.Parameters.Add("CUR_DIVISION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        //GET DATA FROM DATA ACCESS LAYER
        objDs = oDataMgmt.GetDataSet(oCmd);
        return (objDs);
    }

    /// <summary>
    /// GET ALL DEPARTMENT WITHOUT ANY CONDITION
    /// </summary>
    /// <returns></returns>
    public DataSet get_AllDept()
    {
        objDs = new DataSet();
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_COMMONMETHOD.SPROC_DEPARTMENT_GET";
        oCmd.Parameters.Add("ADVPID_IN", OracleDbType.Varchar2).Value = null;
        oCmd.Parameters.Add("ADDIVISIONID_IN", OracleDbType.Varchar2).Value = null;
        oCmd.Parameters.Add("CUR_DEPARTMENT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        //GET DATA FROM DATA ACCESS LAYER
        objDs = oDataMgmt.GetDataSet(oCmd);
        return (objDs);
    }

    public DataSet get_AllModule()
    {
        //ConnectionString objCnStr = new ConnectionString();
        string strCn = _conn.getConnectingString();
        string strSql = string.Empty;
        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strCn;
            try
            {
                objCn.Open();
                OracleCommand objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                strSql = " select a.admoduleid as moduleid, a.descrip as module from admodule a where a.active = 1 order by a.descrip ";

                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.BindByName = true;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                DataSet objDs = new DataSet();
                objAdr.Fill(objDs);
                return objDs;
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

    /// <summary>
    /// GET ALL SECTION WITHOUT ANY CONDITION
    /// </summary>
    /// <returns></returns>
    public DataSet get_AllSection()
    {
        objDs = new DataSet();
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_COMMONMETHOD.SPROC_SECTION_GET";
        oCmd.Parameters.Add("ADVPID_IN", OracleDbType.Varchar2).Value = null;
        oCmd.Parameters.Add("ADDIVISIONID_IN", OracleDbType.Varchar2).Value = null;
        oCmd.Parameters.Add("ADDEPARTMENT_IN", OracleDbType.Varchar2).Value = null;
        oCmd.Parameters.Add("CUR_SECTION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        //GET DATA FROM DATA ACCESS LAYER
        objDs = oDataMgmt.GetDataSet(oCmd);
        return (objDs);
    }

    /// <summary>
    /// GET ALL THE DESIGNATION FROM DESIGNATION TABLE
    /// </summary>
    /// <returns></returns>
    public DataSet get_AllDesgination()
    {

        objDs = new DataSet();
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_COMMONMETHOD.SPROC_DESIGNATION_GET";
        oCmd.Parameters.Add("CUR_DESIGNATION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        //GET DATA FROM DATA ACCESS LAYER
        objDs = oDataMgmt.GetDataSet(oCmd);
        return (objDs);
    }

    /// <summary>
    /// GET ALL THE OPERATION LIST
    /// </summary>
    /// <returns></returns>
    public DataSet get_AllOperation()
    {
        objDs = new DataSet();
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_COMMONMETHOD.SPROC_OPERATION_GET";
        oCmd.Parameters.Add("CUR_OPERATION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.BindByName = true;
        //GET DATA FROM DATA ACCESS LAYER
        objDs = oDataMgmt.GetDataSet(oCmd);
        return (objDs);
    }

    /// <summary>
    /// GET DIVISION LIST ON THE BASE OF OPERATION
    /// </summary>
    /// <param name="OperationID"></param>
    /// <returns></returns>
    public DataSet get_AllDiv(int OperationID)
    {
        objDs = new DataSet();
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_COMMONMETHOD.SPROC_DIVISION_GET";
        if(OperationID !=0)
            oCmd.Parameters.Add("ADVPID_IN", OracleDbType.Varchar2).Value = OperationID;
        oCmd.Parameters.Add("CUR_DIVISION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        //GET DATA FROM DATA ACCESS LAYER

        objDs = oDataMgmt.GetDataSet(oCmd);
        return (objDs);
    }

    /// <summary>
    /// GET ALL THE DEPARTMENT LIST ON THE BASE OF OPERATION, DIVISION
    /// </summary>
    /// <param name="OperationID"></param>
    /// <param name="DivisionID"></param>
    /// <returns></returns>
    public DataSet get_AllDept(int OperationID, int DivisionID)
    {
        objDs = new DataSet();
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_COMMONMETHOD.SPROC_DEPARTMENT_GET";
        if (OperationID != 0)
            oCmd.Parameters.Add("ADVPID_IN", OracleDbType.Varchar2).Value = OperationID;
        if(DivisionID !=0 )
            oCmd.Parameters.Add("ADDIVISIONID_IN", OracleDbType.Varchar2).Value = DivisionID;
        oCmd.Parameters.Add("CUR_DEPARTMENT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        //GET DATA FROM DATA ACCESS LAYER
        objDs = oDataMgmt.GetDataSet(oCmd);
        return (objDs);
    }

    /// <summary>
    /// GET ALL THE SECTION NAME IN THE BASE OF OPERATION, DIVISION, DEPARTMENT
    /// </summary>
    /// <param name="OperationID"></param>
    /// <param name="DivisionID"></param>
    /// <param name="DepartmentID"></param>
    /// <returns></returns>
    public DataSet get_AllSection(int OperationID, int DivisionID, int DepartmentID)
    {
        objDs = new DataSet();
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_COMMONMETHOD.SPROC_SECTION_GET";
        if (OperationID != 0)
            oCmd.Parameters.Add("ADVPID_IN", OracleDbType.Varchar2).Value = OperationID;
        if (DivisionID != 0)
            oCmd.Parameters.Add("ADDIVISIONID_IN", OracleDbType.Varchar2).Value = DivisionID;
        if (DepartmentID !=0)
            oCmd.Parameters.Add("ADDEPARTMENT_IN", OracleDbType.Varchar2).Value = DepartmentID;
        oCmd.Parameters.Add("CUR_SECTION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        //GET DATA FROM DATA ACCESS LAYER
        objDs = oDataMgmt.GetDataSet(oCmd);
        return (objDs);
    }

    #region GET EMP WINDOW ID DTL
    public DataTable GetEmpWindowIDList(string EmpCode, string EmpName, string EmpWindowID, string OPERATION, string DIVISION, string DEPARTMENT)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_COMMONMETHOD.SPROC_EMPWINDOWIDLIST_GET";
        oCmd.Parameters.Add("CUR_WINDOWLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
        oCmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = EmpName;
        oCmd.Parameters.Add("WINDOWID_IN", OracleDbType.Varchar2).Value = EmpWindowID;
        oCmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = OPERATION;
        oCmd.Parameters.Add("DIVISION_IN", OracleDbType.Varchar2).Value = DIVISION;
        oCmd.Parameters.Add("DEPARTMENT_IN", OracleDbType.Varchar2).Value = DEPARTMENT;

        //GET DATA FROM DATA ACCESS LAYER
        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }
    #endregion

    //Seat no.visible start
    public DataSet GetSeatNo(string EmpCode)
    {
        objDs = new DataSet();
        OracleCommand oCmd = new OracleCommand();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_SEARCHEMPLOYEE.SPROC_EMPCURRSEAT_GET";
        oCmd.Parameters.Add("CUR_EMPDTL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
        oCmd.BindByName = true;
        objDs = oDataMgmt.GetDataSet(oCmd);
        return (objDs);
    }
    //Seat no.visible end
}
