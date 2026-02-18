using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using ePortal.Persistence.TravelBilling.Interface;
using ePortal.ViewModels;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

public class cCostCenterMaster: IcCostCenterMaster
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

    public cCostCenterMaster(IDataManagement _oDataMgmt, ICommonFunctions _objcmn)
    {
        oDataMgmt = _oDataMgmt;
        objcmn = _objcmn;
    }
    #endregion

    public DataTable GetCostCenterMasterList(string strSearchString)
    {
        try
        {
            dt = new DataTable();

            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURBILLING.SPROC_ADCOSTCENTERMASTER_GET";

            oCmd.Parameters.Add("ADCOSTCENTERNAME_", OracleDbType.Varchar2).Value = strSearchString;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);

            return (dt);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetCostCenterMasterLogList(string ADCOSTCENTERID)
    {
        try
        {
            dt = new DataTable();

            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURBILLING.SPROC_ADCOSTCENTERMASTER_LOG_GET";

            oCmd.Parameters.Add("ADCOSTCENTERID_", OracleDbType.Varchar2).Value = ADCOSTCENTERID;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);

            return (dt);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public bool DeleteCostCenterBySrNo(string SrNo, string UserID)
    {
        try
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURBILLING.SPROC_DELETEADCOSTCENTERBYSRNO";

            oCmd.Parameters.Add("ADCOSTCENTERID_", OracleDbType.Int32).Value = Convert.ToInt32(SrNo);
            oCmd.Parameters.Add("USERID_", OracleDbType.Varchar2).Value = UserID;

            oDataMgmt.ExecuteQuery(oCmd);

            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public bool InsertUpdateadCostCenterMaster(ADCOSTCENTERMASTER inputJson)
    {
        try
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURBILLING.SPROC_INSERTUPDATEADCOSTCENTERMASTER";

            oCmd.Parameters.Add("ADCOSTCENTERID_", OracleDbType.Int32).Value = Convert.ToInt32(inputJson.ADCOSTCENTERID);
            oCmd.Parameters.Add("ADCOSTCENTERCODE_", OracleDbType.Int32).Value = Convert.ToInt32(inputJson.ADCOSTCENTERCODE);
            oCmd.Parameters.Add("ADCOSTCENTERNAME_", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(inputJson.ADCOSTCENTERNAME) ? "" : inputJson.ADCOSTCENTERNAME.Trim();
            oCmd.Parameters.Add("ADSYSITE_", OracleDbType.Varchar2).Value = inputJson.ADSYSITE;
            oCmd.Parameters.Add("ADDIVISION_", OracleDbType.Varchar2).Value = inputJson.ADDIVISION;
            oCmd.Parameters.Add("ADDEDBY_", OracleDbType.Varchar2).Value = inputJson.ADDEDBY;

            oDataMgmt.ExecuteQuery(oCmd);

            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetCostCenterMasterBySrNo(int id)
    {
        try
        {
            dt = new DataTable();

            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURBILLING.SPROC_ADCOSTCENTERMASTERBYSRNO";

            oCmd.Parameters.Add("ADCOSTCENTERID_", OracleDbType.Int32).Value = id;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);

            return (dt);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public bool ImportCostCenterMaster(ADCOSTCENTERMASTER inputJson)
    {
        try
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURBILLING.SPROC_IMPORTADCOSTCENTERMASTER";

            oCmd.Parameters.Add("ADCOSTCENTERCODE_", OracleDbType.Int32).Value = Convert.ToInt32(inputJson.ADCOSTCENTERCODE);
            oCmd.Parameters.Add("ADCOSTCENTERNAME_", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(inputJson.ADCOSTCENTERNAME) ? "" : inputJson.ADCOSTCENTERNAME.Trim();
            oCmd.Parameters.Add("ADSYSITE_", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(inputJson.ADSYSITE) ? "" : inputJson.ADSYSITE.Trim();
            oCmd.Parameters.Add("ADDIVISION_", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(inputJson.ADDIVISION) ? "" : inputJson.ADDIVISION.Trim();
            oCmd.Parameters.Add("ADDEDBY_", OracleDbType.Varchar2).Value = inputJson.ADDEDBY;

            oDataMgmt.ExecuteQuery(oCmd);

            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
