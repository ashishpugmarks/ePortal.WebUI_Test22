using ePortal.Persistence.Interface;
using ePortal.Persistence.TravelBilling.Interface;
using ePortal.ViewModels;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;


public class cHelpDataProvider: IcHelpDataProvider
{
    #region "Local Variables"
    private string _ErrorMessage = string.Empty;

    public string ErrorMessage
    {
        get { return _ErrorMessage; }
        set { _ErrorMessage = value; }
    }

    DataSet ds = new DataSet();

    DataTable dt = new DataTable();

    DataRow[] _datarow;

    //DataManagement oDataMgmt = new DataManagement();

    //CommonFunctions objcmn = new CommonFunctions();
    private readonly IDataManagement oDataMgmt;
    private readonly ICommonFunctions objcmn;
    #endregion
    public cHelpDataProvider(IDataManagement _oDataMgmt, ICommonFunctions _objcmn)
    {
        oDataMgmt = _oDataMgmt;
        objcmn = _objcmn;
    }
    public DataTable GetTaxCodeList()
    {
        try
        {
            dt = new DataTable();

            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURBILLING.SPROC_GETTAXCODES";

            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);

            return (dt);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public DataTable GetSYSitesList()
    {
        try
        {
            dt = new DataTable();

            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURBILLING.SPROC_GETSYSITES";

            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);

            return (dt);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public DataTable GetUserWiseSYSitesList(String UserID)
    {
        try
        {
            dt = new DataTable();

            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURBILLING.SPROC_GETUSERWISESYSITES";
            oCmd.Parameters.Add("USER_ID", OracleDbType.Varchar2).Value = UserID;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);

            return (dt);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public DataTable GetSYPlantsList()
    {
        try
        {
            dt = new DataTable();

            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURBILLING.SPROC_GETSYPLANT";

            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);

            return (dt);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public DataTable GetUserWiseSYPlantsList(String UserID)
    {
        try
        {
            dt = new DataTable();

            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURBILLING.SPROC_GETUSERWISESYPLANT";
            oCmd.Parameters.Add("USER_ID", OracleDbType.Varchar2).Value = UserID;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);

            return (dt);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetSYDivisionList()
    {
        try
        {
            dt = new DataTable();

            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURBILLING.SPROC_GETSYDIVISION";

            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);

            return (dt);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetADDesignationsList()
    {
        try
        {
            dt = new DataTable();

            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURBILLING.SPROC_GETADDESIGNATION";

            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);

            return (dt);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetADEmployeesList(string Term, string ADDesigId)
    {
        try
        {
            dt = new DataTable();

            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURBILLING.SPROC_GETADEMPLOYEE";

            oCmd.Parameters.Add("TERM", OracleDbType.Varchar2).Value = Term;
            oCmd.Parameters.Add("ADDESIGID", OracleDbType.Varchar2).Value = ADDesigId;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);

            return (dt);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetADVendorList(string Term)
    {
        try
        {
            dt = new DataTable();

            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURBILLING.SPROC_GETADVENDORMASTER";

            oCmd.Parameters.Add("TERM", OracleDbType.Varchar2).Value = Term;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);

            return (dt);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetCostCenterList(string Term, string SiteId)
    {
        try
        {
            dt = new DataTable();

            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURBILLING.SPROC_GETCOSTCENTER";

            oCmd.Parameters.Add("TERM", OracleDbType.Varchar2).Value = Term;
            oCmd.Parameters.Add("ADSYSITE_", OracleDbType.Varchar2).Value = SiteId;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);

            return (dt);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
