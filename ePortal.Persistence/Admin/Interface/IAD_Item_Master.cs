// Auto-generated interface for AD_ITEM_MASTER
using ePortal.ViewModels.APPX.Stationary;
using System.Data;


namespace ePortal.Persistence.Admin.Interface
{
    public interface  IAD_ITEM_MASTER
    {
       
        DataTable GetItem_List(string strItem_descrip, string strItem_type, string strstatus, int PlantID);
        DataTable GetItem_ListbyID(string strITEMID);
        DataTable GetUser_CategorybyID(string strU_CategoryID);
        DataTable GetNextIssue_List(string from_date, string to_date, string strempcode, string PlantID);

        // Item Master - Insert/Update
        int InsertUpdate_ItemMaster(string strITEMID, string strItemtype_Code,
            string strItemCode, string strDesc, string strActive,
            string strUOM, string strLife, string strlifedesc,
            string strBy, string size_id, string usercategory_id, string strSubcategoryID);

        // Add Item Category functions
        int Add_Size_Category(string struid, string strSub_Category, string XMLSIZE, string strBy, int status);
        int Add_User_Category(string struid, string User_category, string strBy, int status, string strplantid);
        int Add_Main_Category(string struid, string Main_category, string strBy, int status, string PLANTID);
        int Add_Sub_Category(string struid, string Main_category_id, string Sub_category, string strBy, int status);

        // Stock Master - Get Data
        DataTable GetItem_Type();
        DataTable GetItem_Category(string strUId);
        DataTable GetUser_Category(int status, string strplant);
        DataTable GetMain_Category(int status, string PlantID);
        DataTable GetMain_CategoryByid(string strUId);
        DataTable Getsub_Category(int status, int PlantID);
        DataTable Getsub_CategoryByid(string strUId);
        DataTable GetSub_CategoryBymaincategory(string strUId);
        DataTable GetEmp_ListByDept(string strUId);
        DataTable GetEmp_ListByDept(string strUId, string strItemID, string strStartDate, string strEndDate);
        DataTable Getsize_Category(int status, string PLANTID);
        DataTable GetSize_categoryByid(string strcategoryID);
        DataTable GetSize_Bysubcategory(string strUId);
        DataTable GetSize_Bysubcategory(string strSubCategory, string strUserCategory);
        DataSet GetItem_CategorybyID(string strCategoryID);
        DataTable GetItemList_BYTYPE(string strItemtype, string strItemcode, string strItemID);
        DataTable GetItemList_BYCategory(string strSubcategoryID, string strusercategory_id);
        DataTable GetItemList();
        DataTable GetStock_List(string from_date, string to_date, string item_id, string Bill_no, string Po_no, string status, string strPlantid);
        DataTable GetAvailableStock_List(string item_id, string PlantID);
        DataTable GetStock_ListBYID(string strStock_ID);

        // Stock Master - Insert/Update
        int InsertUpdate_StockMaster(string strStockID, string strItem_id, string strSTOCK, string STR_DATE, string STR_REMARKS,
            string stradd_By, string STK_ACTIVE, string strBill_No, string strPO_NO, string strPlant_ID, string strSubcategoryID);

        // Stock Issue - Get Data
        DataTable GetStock_IssueDetail(string stremp_id, string Status, string Issue_Against, string v_Date_from, string v_Date_to, string PlantID);
        DataTable GetIssueDetail_BYIssueid(string strissueid);
        DataTable GetUser_Detail(string empcode);
        DataTable GetTax_User_Detail(string empcode);
        double GetAvail_Stock(string item_id);

        // Stock Issue - Insert/Update
        int InsertUpdate_StockIssue(string strStk_IssueID, string strEmp_code, string strItem_id, string strqty, string Strlocker_no, string Str_REMARKS,
            string stradded_By, string Str_ACTIVE, string casual_Visitor, string stremp_type, string Issue_against, string Issue_date, string strcode);

        // Bulk upload (CR6916)
        string BulkUpload_UniformStockOut(BulkUploadStockOutModel stockOut);
        int InsertBulkIssue(string strXML);

        // Bulk Item Functions
        int Insert_Bulkitem(string struid, string strEmpcode_id, string XMLSIZE, string strBy, int status, string strDept);
        int InsertUpdate_BulkitemStockIssue(string stradded_By, string Str_ACTIVE, string Issue_date, string Str_departmentid);
        DataTable Getview_stock(string strdeptid);
        DataTable Getview_EMP(string strdeptid);

        // Uniform helpers
        DataTable getEmployee(string strOperation, string strDivision, string strDeptID);
        DataTable getStaffAssociates(string strOperation, string strDivision, string strDeptID, string strSiteID);

        // Reports
        DataTable Get_MonthlyReport(string strmonth, string strplant, string strKi);
        DataTable GetKI_New();
    }
}
