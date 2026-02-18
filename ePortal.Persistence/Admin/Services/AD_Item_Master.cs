using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using ePortal.ViewModels.APPX.Stationary;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

/// <summary>
/// Summary description for AD_ITEM_MASTER
/// </summary>
public class AD_ITEM_MASTER : IAD_ITEM_MASTER
{
    #region "Instance Variables & Constructor"
    private readonly IDataManagement oDataMgmt;
    private readonly IConnectionString objConn;
    private readonly ICommonFunctions objCommon;

    OracleCommand oCmd;
    DataSet ds = new DataSet();
    DataTable Dt;
    DataRow[] _datarow;
    string strConn;
    string errMsg = string.Empty;
    string strQry = string.Empty;

    public AD_ITEM_MASTER(IDataManagement _oDataMgmt, IConnectionString _objConn, ICommonFunctions _objCommon)
    {
        oDataMgmt = _oDataMgmt;
        objConn = _objConn;
        objCommon = _objCommon;
    }
    #endregion

    #region "Item Master"
    #region "Get Data"

    /// <summary>
    /// Get Item List
    /// </summary>
    /// <returns>DataTable</returns>



    public DataTable GetItem_List(String strItem_descrip, String strItem_type, String strstatus, int PlantID)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_ITEMLIST_GET";
        oCmd.Parameters.Add("V_Item_Descrip", OracleDbType.Varchar2).Value = strItem_descrip;
        oCmd.Parameters.Add("V_Item_Type", OracleDbType.Varchar2).Value = strItem_type;
        oCmd.Parameters.Add("V_Status", OracleDbType.Varchar2).Value = strstatus;
        oCmd.Parameters.Add("V_PlantID", OracleDbType.Int32).Value = PlantID;
        oCmd.Parameters.Add("CUR_MFLINE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }
    public DataTable GetItem_ListbyID(String strITEMID)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_ITEMLISTBYID_GET";
        oCmd.Parameters.Add("Item_Master_id", OracleDbType.Int32).Value = strITEMID;
        oCmd.Parameters.Add("CUR_MFLINE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }
    public DataTable GetUser_CategorybyID(String strU_CategoryID)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_USERCATEGORY_GETBYID";
        oCmd.Parameters.Add("V_category_id", OracleDbType.Int32).Value = strU_CategoryID;
        oCmd.Parameters.Add("CUR_MFLINE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }
    public DataTable GetNextIssue_List(String from_date, String to_date, String strempcode, String PlantID)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_NextIssue_List";
        oCmd.Parameters.Add("PlantId_IN", OracleDbType.Int32).Value = PlantID;
        oCmd.Parameters.Add("Emp_Code", OracleDbType.Varchar2).Value = strempcode;
        oCmd.Parameters.Add("V_FROM_DATE", OracleDbType.Varchar2).Value = from_date;
        oCmd.Parameters.Add("V_TO_DATE", OracleDbType.Varchar2).Value = to_date;
        oCmd.Parameters.Add("CUR_MFLINE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }
    #endregion
    #region"Item Master INSERT-UPDATE FUNCTION"

    public int InsertUpdate_ItemMaster(String strITEMID, String strItemtype_Code,
        String strItemCode, String strDesc, String strActive,
        String strUOM, String strLife, String strlifedesc,
        String strBy, String size_id, String usercategory_id,string strSubcategoryID)
    {
        String strErrMsg = String.Empty;
        string strCn = objConn.getConnectingString();
        OracleCommand objCmd;

        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strCn;
            objCn.Open();
            try
            {
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                string strSql = "PKG_AdStockMGMT.SPROC_UPDATE_ITEM";
                objCmd.Parameters.Add("Item_Master_id", OracleDbType.Int32).Value = strITEMID;
                objCmd.Parameters.Add("Itemtype_Code", OracleDbType.Varchar2).Value = strItemtype_Code;
                objCmd.Parameters.Add("Item_Code", OracleDbType.Varchar2).Value = strItemCode;
                objCmd.Parameters.Add("Description", OracleDbType.Varchar2).Value = strDesc;
                objCmd.Parameters.Add("item_Active", OracleDbType.Int32).Value = strActive;
                objCmd.Parameters.Add("item_UOM", OracleDbType.Varchar2).Value = strUOM;
                objCmd.Parameters.Add("item_Life", OracleDbType.Varchar2).Value = strLife;
                objCmd.Parameters.Add("item_lifedescrip", OracleDbType.Varchar2).Value = strlifedesc;
                objCmd.Parameters.Add("add_By", OracleDbType.Int32).Value = strBy; 
                objCmd.Parameters.Add("V_size_id", OracleDbType.Int32).Value = Convert.DBNull;
                objCmd.Parameters.Add("V_usercategory_id", OracleDbType.Int32).Value = usercategory_id;
                objCmd.Parameters.Add("V_SUBCATEGORY_ID",OracleDbType.Int32).Value = strSubcategoryID;
                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;

                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.ExecuteNonQuery();

                strErrMsg = Convert.ToString(objCmd.Parameters["ERROR_MSG"].Value);
                return Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                objCmd.Dispose();
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
    #endregion
    #region"ADD Item Category insert fuction"
    public int Add_Size_Category(String struid, String strSub_Category, String XMLSIZE, String strBy, int status)
    {
        String strErrMsg = String.Empty;
        string strCn = objConn.getConnectingString();
        OracleCommand objCmd;

        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strCn;
            objCn.Open();
            try
            {
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                string strSql = "PKG_AdStockMGMT.SPROC_Add_SizeCategory";
                objCmd.Parameters.Add("v_size_id", OracleDbType.Int32).Value = struid;
                objCmd.Parameters.Add("v_Sub_category_id", OracleDbType.Int32).Value = strSub_Category;
                objCmd.Parameters.Add("XMLSIZE_IN", OracleDbType.Varchar2).Value = XMLSIZE;
                objCmd.Parameters.Add("v_addedby", OracleDbType.Int32).Value = strBy;
                objCmd.Parameters.Add("v_status", OracleDbType.Int32).Value = status;
                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int16).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.BindByName = true;
                objCmd.ExecuteNonQuery();
                strErrMsg = Convert.ToString(objCmd.Parameters["ERROR_MSG"].Value);
                return Convert.ToInt16(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                objCmd.Dispose();
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
    public int Add_User_Category(String struid, String User_category, String strBy, int status, string strplantid)
    {
        String strErrMsg = String.Empty;
        string strCn = objConn.getConnectingString();
        OracleCommand objCmd;

        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strCn;
            objCn.Open();
            try
            {
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                string strSql = "PKG_AdStockMGMT.SPROC_ADD_USERCATEGORY";
                objCmd.Parameters.Add("v_user_category_id", OracleDbType.Int32).Value = struid;
                objCmd.Parameters.Add("v_user_category", OracleDbType.Varchar2).Value = User_category;
                objCmd.Parameters.Add("v_addedby", OracleDbType.Int32).Value = strBy;
                objCmd.Parameters.Add("v_status", OracleDbType.Int32).Value = status;
                objCmd.Parameters.Add("v_plantid", OracleDbType.Varchar2).Value = strplantid;
                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 11).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.ExecuteNonQuery();
                strErrMsg = Convert.ToString(objCmd.Parameters["ERROR_MSG"].Value);
                return Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                objCmd.Dispose();
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
    public int Add_Main_Category(String struid, String Main_category, String strBy, int status, string PLANTID)
    {
        String strErrMsg = String.Empty;
        string strCn = objConn.getConnectingString();
        OracleCommand objCmd;

        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strCn;
            objCn.Open();
            try
            {
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                string strSql = "PKG_AdStockMGMT.SPROC_ADD_MAINCATEGORY";
                objCmd.Parameters.Add("v_PLANTID", OracleDbType.Int32).Value = PLANTID;
                objCmd.Parameters.Add("v_main_category_id", OracleDbType.Int32).Value = struid;
                objCmd.Parameters.Add("v_Main_category", OracleDbType.Varchar2).Value = Main_category;
                objCmd.Parameters.Add("v_addedby", OracleDbType.Int32).Value = strBy;
                objCmd.Parameters.Add("v_status", OracleDbType.Int32).Value = status;
                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 11).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.ExecuteNonQuery();
                strErrMsg = Convert.ToString(objCmd.Parameters["ERROR_MSG"].Value);
                return Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                objCmd.Dispose();
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
    public int Add_Sub_Category(String struid, String Main_category_id, String Sub_category, String strBy, int status)
    {
        String strErrMsg = String.Empty;
        string strCn = objConn.getConnectingString();
        OracleCommand objCmd;

        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strCn;
            objCn.Open();
            try
            {
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                string strSql = "PKG_AdStockMGMT.SPROC_ADD_SUBCATEGORY";
                objCmd.Parameters.Add("v_sub_category_id", OracleDbType.Int32).Value = struid;
                objCmd.Parameters.Add("v_Main_category_id", OracleDbType.Int32).Value = Main_category_id;
                objCmd.Parameters.Add("v_Sub_category", OracleDbType.Varchar2).Value = Sub_category;
                objCmd.Parameters.Add("v_addedby", OracleDbType.Int32).Value = strBy;
                objCmd.Parameters.Add("v_status", OracleDbType.Int32).Value = status;
                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 11).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.ExecuteNonQuery();
                strErrMsg = Convert.ToString(objCmd.Parameters["ERROR_MSG"].Value);
                return Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                objCmd.Dispose();
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
    #region "Stock Master"
    #region "Get Data"

    /// <summary>
    /// Get Item List
    /// </summary>
    /// <returns>DataTable</returns>

    public DataTable GetItem_Type()
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_DISTINCT_ITEMTYPE_GET";
        oCmd.Parameters.Add("CUR_ITEMTYPE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }
    public DataTable GetItem_Category(String strUId)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_MAINCATEGORY_GET";
        oCmd.Parameters.Add("v_maincategory_id", OracleDbType.Int32).Value = strUId;
        oCmd.Parameters.Add("CUR_MFLINE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }
    public DataTable GetUser_Category(int status, string strplant)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_USERCATEGORY_GET";
        oCmd.Parameters.Add("V_Status", OracleDbType.Int32).Value = status;
        oCmd.Parameters.Add("V_plantid", OracleDbType.Varchar2).Value = strplant;
        oCmd.Parameters.Add("CUR_MFLINE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }
    public DataTable GetMain_Category(int status, string PlantID)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_MAINCATEGORY_GET";
        oCmd.Parameters.Add("PlantID_IN", OracleDbType.Int32).Value = PlantID;
        oCmd.Parameters.Add("V_Status", OracleDbType.Int32).Value = status;
        oCmd.Parameters.Add("CUR_MFLINE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }
    public DataTable GetMain_CategoryByid(String strUId)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_MAINCATEGORY_GETBYID";
        oCmd.Parameters.Add("v_maincategory_id", OracleDbType.Int32).Value = strUId;
        oCmd.Parameters.Add("CUR_MFLINE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }
    public DataTable Getsub_Category(int status, int PlantID)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_SUBCATEGORY_GET";
        oCmd.Parameters.Add("V_Status", OracleDbType.Int32).Value = status;
        oCmd.Parameters.Add("V_PlantID", OracleDbType.Int32).Value = PlantID;
        oCmd.Parameters.Add("CUR_MFLINE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }
    public DataTable Getsub_CategoryByid(String strUId)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_SUBCATEGORY_GETBYID";
        oCmd.Parameters.Add("v_sub_category_id", OracleDbType.Int32).Value = strUId;
        oCmd.Parameters.Add("CUR_MFLINE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }
    public DataTable GetSub_CategoryBymaincategory(String strUId)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_SUBCATEGORY_Maincategory";
        oCmd.Parameters.Add("v_Main_category_id", OracleDbType.Int32).Value = strUId;
        oCmd.Parameters.Add("CUR_MFLINE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }
    public DataTable GetEmp_ListByDept(String strUId)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_Emplist_BYDEPT";
        oCmd.Parameters.Add("v_dept_id", OracleDbType.Int32).Value = strUId;
        oCmd.Parameters.Add("CUR_MFLINE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }
    public DataTable GetEmp_ListByDept(String strUId, String strItemID, String strStartDate, String strEndDate)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_EMP_BYDEPT";
        oCmd.Parameters.Add("DEPTID", OracleDbType.Int32).Value = strUId;
        oCmd.Parameters.Add("ITEMID", OracleDbType.Int32).Value = strItemID;
        oCmd.Parameters.Add("STARTDATE", OracleDbType.Varchar2).Value = strStartDate;
        oCmd.Parameters.Add("ENDDATE", OracleDbType.Varchar2).Value = strEndDate;
        oCmd.Parameters.Add("CUR_MFLINE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }
    public DataTable Getsize_Category(int status, string PLANTID)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_Size_GET";
        oCmd.Parameters.Add("PLANTID_IN", OracleDbType.Int32).Value = PLANTID;
        oCmd.Parameters.Add("V_Status", OracleDbType.Int32).Value = status;
        oCmd.Parameters.Add("CUR_MFLINE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }
    public DataTable GetSize_categoryByid(String strcategoryID)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_Size_GETBYID";
        oCmd.Parameters.Add("v_size_id", OracleDbType.Int32).Value = strcategoryID;
        oCmd.Parameters.Add("CUR_MFLINE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }
    public DataTable GetSize_Bysubcategory(String strUId)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_Size_GETBYSUBCATEGORY";
        oCmd.Parameters.Add("v_subcategory_id", OracleDbType.Int32).Value = strUId;
        oCmd.Parameters.Add("CUR_MFLINE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }
    public DataTable GetSize_Bysubcategory(String strSubCategory, String strUserCategory)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_SIZE_BYSUBCATEGORY_GET";
        oCmd.Parameters.Add("V_USERCATEGORY_ID", OracleDbType.Int32).Value = strUserCategory;
        oCmd.Parameters.Add("V_SUBCATEGORY_ID", OracleDbType.Int32).Value = strSubCategory;
        oCmd.Parameters.Add("CUR_MFLINE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }
    public DataSet GetItem_CategorybyID(String strCategoryID)
    {
        OracleCommand oCmd = new OracleCommand();
        DataSet ds = new DataSet();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_ADITEMTYPE_MASTER.SPROC_ITEMCATEGORY_GETBYID";
        oCmd.Parameters.Add("V_category_id", OracleDbType.Int32).Value = strCategoryID;
        oCmd.Parameters.Add("CUR_MFLINE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        oCmd.Parameters.Add("CUR_SIZE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        ds = oDataMgmt.GetDataSet(oCmd);
        oCmd.Dispose();
        return (ds);
    }
    public DataTable GetItemList_BYTYPE(String strItemtype, String strItemcode, String strItemID)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_ITEMBY_ITEMTYPE_GET";
        oCmd.Parameters.Add("ITEM_TYPE", OracleDbType.Varchar2).Value = strItemtype;
        oCmd.Parameters.Add("ITEM_ID", OracleDbType.Varchar2).Value = strItemID;
        oCmd.Parameters.Add("ITEM_CODE", OracleDbType.Varchar2).Value = strItemcode;
        oCmd.Parameters.Add("CUR_ITEM", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }
    public DataTable GetItemList_BYCategory(String strSubcategoryID, String strusercategory_id)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_ITEMTYPE_GET";
        //oCmd.Parameters.Add("v_Size_id", OracleDbType.Int32).Value = strsize_id;
        oCmd.Parameters.Add("V_SUBCATEGORY_ID", OracleDbType.Int32).Value = strSubcategoryID;
        oCmd.Parameters.Add("v_usercategory_id", OracleDbType.Int32).Value = strusercategory_id;
        oCmd.Parameters.Add("CUR_ITEM", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }
    public DataTable GetItemList()
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_ITEMlist_GET";
        oCmd.Parameters.Add("CUR_ITEM", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }
    public DataTable GetStock_List(String from_date, String to_date, String item_id, String Bill_no, String Po_no, String status, String strPlantid)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_StockLIST_GET";
        oCmd.Parameters.Add("V_FROM_DATE", OracleDbType.Varchar2).Value = from_date;
        oCmd.Parameters.Add("V_PLANTID", OracleDbType.Int32).Value = strPlantid;
        oCmd.Parameters.Add("V_TO_DATE", OracleDbType.Varchar2).Value = to_date;
        oCmd.Parameters.Add("V_ITEM_ID", OracleDbType.Varchar2).Value = item_id;
        oCmd.Parameters.Add("V_Bill_NO", OracleDbType.Varchar2).Value = Bill_no;
        oCmd.Parameters.Add("V_PO_NO", OracleDbType.Varchar2).Value = Po_no;
        oCmd.Parameters.Add("V_STATUS", OracleDbType.Varchar2).Value = status;
        oCmd.Parameters.Add("Cur_ItemStock", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }
    public DataTable GetAvailableStock_List(String item_id, String PlantID)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_Available_Stock_List";
        oCmd.Parameters.Add("PlantID_IN", OracleDbType.Int32).Value = PlantID;
        oCmd.Parameters.Add("V_ITEM_ID", OracleDbType.Varchar2).Value = item_id;
        oCmd.Parameters.Add("CUR_AStock", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }
    public DataTable GetStock_ListBYID(String strStock_ID)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_StockLIST_BYID_GET";
        oCmd.Parameters.Add("StrStock_ID", OracleDbType.Int32).Value = strStock_ID;
        oCmd.Parameters.Add("Cur_ItemStock", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }
    #endregion
    #region"Stock INSERT-UPDATE FUNCTION"


    public int InsertUpdate_StockMaster(String strStockID, String strItem_id, String strSTOCK, String STR_DATE, String STR_REMARKS,
                                String stradd_By, String STK_ACTIVE, String strBill_No, String strPO_NO, String strPlant_ID,string strSubcategoryID)
    {
        String strErrMsg = String.Empty;
        string strCn = objConn.getConnectingString();
        OracleCommand objCmd;

        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strCn;
            objCn.Open();
            try
            {
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                string strSql = "PKG_AdStockMGMT.SPROC_UPDATE_ITEM_STOCK";
                objCmd.Parameters.Add("PlantID_IN", OracleDbType.Int32).Value = strPlant_ID;
                objCmd.Parameters.Add("STOCK_NM", OracleDbType.Int32).Value = strStockID;
                objCmd.Parameters.Add("Item_id", OracleDbType.Int32).Value = strItem_id;
                objCmd.Parameters.Add("STOCK", OracleDbType.Int32).Value = strSTOCK;
                objCmd.Parameters.Add("STR_DATE", OracleDbType.Varchar2).Value = STR_DATE;
                //objCmd.Parameters.Add("add_By", OracleDbType.Int32).Value = stradd_By; //CR 6235
                objCmd.Parameters.Add("STR_REMARKS", OracleDbType.Varchar2).Value = STR_REMARKS;
                objCmd.Parameters.Add("add_By", OracleDbType.Int32).Value = stradd_By; //CR 6235
                objCmd.Parameters.Add("STK_ACTIVE", OracleDbType.Varchar2).Value = STK_ACTIVE;
                objCmd.Parameters.Add("STR_BillNO", OracleDbType.Varchar2).Value = strBill_No;
                objCmd.Parameters.Add("STRPO_NO", OracleDbType.Varchar2).Value = strPO_NO;
                //objCmd.Parameters.Add("V_SUBCATEGORY_ID", OracleDbType.Varchar2).Value = strSubcategoryID; //CR 6235
                objCmd.Parameters.Add("V_SUBCATEGORY_ID", OracleDbType.Int32).Value = strSubcategoryID; //CR 6235
                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.ExecuteNonQuery();
                strErrMsg = Convert.ToString(objCmd.Parameters["ERROR_MSG"].Value);
                return Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
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
    #endregion
    #region "Stock Issue"
    #region "Get Data"
    public DataTable GetStock_IssueDetail(String stremp_id, String Status, String Issue_Against, String v_Date_from, String v_Date_to, String PlantID)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_StockIssue_BYID_GET";
        oCmd.Parameters.Add("EMP_Code", OracleDbType.Varchar2).Value = stremp_id;
        oCmd.Parameters.Add("V_Status", OracleDbType.Varchar2).Value = Status;
        oCmd.Parameters.Add("V_issue_against", OracleDbType.Varchar2).Value = Issue_Against;
        oCmd.Parameters.Add("V_Date_From", OracleDbType.Varchar2).Value = v_Date_from;
        oCmd.Parameters.Add("V_Date_To", OracleDbType.Varchar2).Value = v_Date_to;
        oCmd.Parameters.Add("PLANT_IN", OracleDbType.Varchar2).Value = PlantID;
        oCmd.Parameters.Add("CUR_StockIssue", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }
    public DataTable GetIssueDetail_BYIssueid(String strissueid)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_Issue_BYIssueID_GET";
        oCmd.Parameters.Add("IssueId", OracleDbType.Varchar2).Value = strissueid;
        oCmd.Parameters.Add("CUR_StockIssue", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }

    public DataTable GetUser_Detail(String empcode)
    {
        DataTable dt = new DataTable();
        OracleCommand objCmd = new OracleCommand();
        objCmd.CommandText = "SPROC_USER_DETAILS";
        objCmd.CommandType = System.Data.CommandType.StoredProcedure;
        objCmd.Parameters.Add("CSR_USERDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = empcode;
        dt = oDataMgmt.GetDataTable(objCmd);
        objCmd.Dispose();
        return (dt);
    }
    public DataTable GetTax_User_Detail(String empcode)
    {
        DataTable dt = new DataTable();
        OracleCommand objCmd = new OracleCommand();
        objCmd.CommandText = "SPROC_TAX_USER_DETAILS";
        objCmd.CommandType = System.Data.CommandType.StoredProcedure;
        objCmd.Parameters.Add("CSR_USERDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = empcode;
        dt = oDataMgmt.GetDataTable(objCmd);
        objCmd.Dispose();
        return (dt);
    }
    public double GetAvail_Stock(string item_id)
    {
        DataTable dt;
        OracleCommand objCmd = new OracleCommand();
        objCmd.CommandText = "select avail_stock(" + item_id + ") from dual";
        objCmd.CommandType = System.Data.CommandType.Text;
        dt = oDataMgmt.GetDataTable(objCmd);
        objCmd.Dispose();
        return Convert.ToDouble(dt.Rows[0][0]);

    }
    #endregion
    #region"Stock Issue INSERT-UPDATE FUNCTION"


    public int InsertUpdate_StockIssue(String strStk_IssueID, String strEmp_code, String strItem_id, String strqty, String Strlocker_no, String Str_REMARKS,
                                String stradded_By, String Str_ACTIVE, String casual_Visitor, String stremp_type, String Issue_against, String Issue_date, String strcode)
    {
        String strErrMsg = String.Empty;
        string strCn = objConn.getConnectingString();
        OracleCommand objCmd;

        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strCn;
            objCn.Open();
            try
            {
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                string strSql = "PKG_AdStockMGMT.SPROC_UPDATE_ItemIssue";
                objCmd.Parameters.Add("v_aditemissueid", OracleDbType.Int32).Value = strStk_IssueID;
                objCmd.Parameters.Add("v_adempcode", OracleDbType.Varchar2).Value = strEmp_code;
                objCmd.Parameters.Add("v_aditemmasterid", OracleDbType.Int32).Value = strItem_id;
                objCmd.Parameters.Add("v_qty", OracleDbType.Int32).Value = strqty;
                objCmd.Parameters.Add("v_shoelockerno", OracleDbType.Varchar2).Value = Strlocker_no;
                objCmd.Parameters.Add("v_remarks", OracleDbType.Varchar2).Value = Str_REMARKS;
                objCmd.Parameters.Add("v_issueby", OracleDbType.Int32).Value = stradded_By;
                objCmd.Parameters.Add("v_active", OracleDbType.Varchar2).Value = Str_ACTIVE;
                objCmd.Parameters.Add("v_emp_name", OracleDbType.Varchar2).Value = casual_Visitor;
                objCmd.Parameters.Add("v_emp_type", OracleDbType.Varchar2).Value = stremp_type;
                objCmd.Parameters.Add("v_issue_against", OracleDbType.Varchar2).Value = Issue_against;
                objCmd.Parameters.Add("v_issuedate", OracleDbType.Varchar2).Value = Issue_date;
                objCmd.Parameters.Add("v_code", OracleDbType.Varchar2).Value = strcode;

                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.ExecuteNonQuery();
                strErrMsg = Convert.ToString(objCmd.Parameters["ERROR_MSG"].Value);
                return Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                objCmd.Dispose();
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
    //sa CR6916 Changes by TTL
    public string BulkUpload_UniformStockOut(BulkUploadStockOutModel stockOut)
    {
        String strErrMsg = String.Empty;
        string strCn = objConn.getConnectingString();
        OracleCommand objCmd;

        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strCn;
            objCn.Open();
            try
            {
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                string strSql = "PKG_AdStockMGMT.SPROC_Uniform_StockOut";
                objCmd.Parameters.Add("CapQty", OracleDbType.Int32).Value = stockOut.CapQty;
                objCmd.Parameters.Add("TrouserQty", OracleDbType.Int32).Value = stockOut.TrouserQty;
                objCmd.Parameters.Add("JacketQty", OracleDbType.Int32).Value = stockOut.JacketQty;
                objCmd.Parameters.Add("Issueby", OracleDbType.Int32).Value = stockOut.Issueby;
                objCmd.Parameters.Add("Adempcode", OracleDbType.Varchar2).Value = stockOut.Adempcode;
                objCmd.Parameters.Add("IssueDate", OracleDbType.Varchar2).Value = stockOut.IssueDate;

                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.ExecuteNonQuery();
                return strErrMsg = Convert.ToString(objCmd.Parameters["ERROR_MSG"].Value);
                // Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                objCmd.Dispose();
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
    //sa CR6916 Changes by TTL

    public int InsertBulkIssue(string strXML)
    {
        String strErrMsg = String.Empty;
        string strCn = objConn.getConnectingString();
        OracleCommand objCmd;

        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strCn;
            objCn.Open();
            try
            {
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                string strSql = "PKG_AdStockMGMT.SPROC_BulkIssue";

                objCmd.Parameters.Add("XMLITEM_IN", OracleDbType.Varchar2).Value = strXML;

                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;

                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.BindByName = true;
                objCmd.ExecuteNonQuery();

                strErrMsg = Convert.ToString(objCmd.Parameters["ERROR_MSG"].Value);
                return Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                objCmd.Dispose();
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
    #endregion
    #region" Bulk Item Fuction"
    public int Insert_Bulkitem(String struid, String strEmpcode_id, String XMLSIZE, String strBy, int status, String strDept)
    {
        String strErrMsg = String.Empty;
        string strCn = objConn.getConnectingString();
        OracleCommand objCmd;

        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strCn;
            objCn.Open();
            try
            {
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                string strSql = "PKG_AdStockMGMT.SPROC_BulkIssue_Item";
                objCmd.Parameters.Add("v_BulkIssue_Item_id", OracleDbType.Int32).Value = struid;
                objCmd.Parameters.Add("v_Empcode_id", OracleDbType.Int32).Value = strEmpcode_id;
                objCmd.Parameters.Add("XMLITEM_IN", OracleDbType.Varchar2).Value = XMLSIZE;
                objCmd.Parameters.Add("v_addedby", OracleDbType.Int32).Value = strBy;
                objCmd.Parameters.Add("v_status", OracleDbType.Int32).Value = status;
                objCmd.Parameters.Add("v_addedby", OracleDbType.Int32).Value = strDept;
                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.ExecuteNonQuery();
                strErrMsg = Convert.ToString(objCmd.Parameters["ERROR_MSG"].Value);
                return Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                objCmd.Dispose();
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
    public int InsertUpdate_BulkitemStockIssue(String stradded_By, String Str_ACTIVE, String Issue_date, String Str_departmentid)
    {
        String strErrMsg = String.Empty;
        string strCn = objConn.getConnectingString();
        OracleCommand objCmd;

        using (OracleConnection objCn = new OracleConnection())
        {
            objCn.ConnectionString = strCn;
            objCn.Open();
            try
            {
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                string strSql = "PKG_AdStockMGMT.SPROC_UPDATE_BulkIssue";
                objCmd.Parameters.Add("v_issueby", OracleDbType.Int32).Value = stradded_By;
                objCmd.Parameters.Add("v_active", OracleDbType.Varchar2).Value = Str_ACTIVE;
                objCmd.Parameters.Add("v_issuedate", OracleDbType.Varchar2).Value = Issue_date;
                objCmd.Parameters.Add("v_departmentid", OracleDbType.Int32).Value = Str_departmentid;
                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                objCmd.ExecuteNonQuery();
                strErrMsg = Convert.ToString(objCmd.Parameters["ERROR_MSG"].Value);
                return Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                objCmd.Dispose();
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
    public DataTable Getview_stock(String strdeptid)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_VIEWSTOCK_GET";
        oCmd.Parameters.Add("V_DEPARTMENT_ID", OracleDbType.Int32).Value = strdeptid;
        oCmd.Parameters.Add("CUR_STOCK", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }
    public DataTable Getview_EMP(String strdeptid)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_VIEWEMP_GET";
        oCmd.Parameters.Add("V_DEPARTMENT_ID", OracleDbType.Int32).Value = strdeptid;
        oCmd.Parameters.Add("CUR_EMP", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }
    #endregion

    public DataTable getEmployee(string strOperation, string strDivision, string strDeptID)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_ADUNIFORM_MASTER.SPROC_GetDeptEmployees";
        oCmd.Parameters.Add("OPID_IN", OracleDbType.Varchar2).Value = strOperation;
        oCmd.Parameters.Add("DIVID_IN", OracleDbType.Varchar2).Value = strDivision;
        oCmd.Parameters.Add("DEPTID_IN", OracleDbType.Varchar2).Value = strDeptID;
        oCmd.Parameters.Add("CUR_EMPDTL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);

    }

    public DataTable getStaffAssociates(string strOperation, string strDivision, string strDeptID, string strSiteID)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_ADUNIFORM_MASTER.SPROC_GetStaffAssociates";
        oCmd.Parameters.Add("OPID_IN", OracleDbType.Varchar2).Value = strOperation;
        oCmd.Parameters.Add("DIVID_IN", OracleDbType.Varchar2).Value = strDivision;
        oCmd.Parameters.Add("DEPTID_IN", OracleDbType.Varchar2).Value = strDeptID;
        oCmd.Parameters.Add("SITEID_IN", OracleDbType.Varchar2).Value = strSiteID;
        oCmd.Parameters.Add("CUR_EMPDTL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);

    }
    public DataTable Get_MonthlyReport(string strmonth, string strplant, string strKi)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.CommandText = "PKG_AdStockMGMT.SPROC_MONTLY_REPORT";
        oCmd.Parameters.Add("MONTH_IN", OracleDbType.Varchar2).Value = strmonth;
        oCmd.Parameters.Add("PLANT_IN", OracleDbType.Varchar2).Value = strplant;
        oCmd.Parameters.Add("KI_IN", OracleDbType.Varchar2).Value = strKi;
        oCmd.Parameters.Add("CUR_MREPORT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        dt = oDataMgmt.GetDataTable(oCmd);
        oCmd.Dispose();
        return (dt);
    }

    public DataTable GetKI_New()
    {
        DataTable dt;
        string qry = "SELECT * FROM SYKI ki ORDER BY KICODE";
        dt = oDataMgmt.GetDataTable(qry);
        return (dt);
    }
}