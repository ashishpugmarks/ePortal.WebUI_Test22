using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Interface
{
    public interface IMasterMgmtRepo
    {
        DataSet GetPPCApprovalUserList();
        DataSet GetFinanceApprovalUserList();
        DataSet GetPlantList();
        DataSet GetMasterPlantList();
        DataSet GetPlantDetails(string plantid);
        DataSet GetMaterialTypeList();
        DataSet GetMaterialGroupList();
        DataSet GetUOMList();
        DataSet GetValuationClassByMaterialType(int MaterialTypeId);
        DataSet GetMaterialGroupByMaterialType(int MaterialTypeId);
        DataSet GetValuationClassByMaterialTypeGroup(int MaterialTypeId, int MaterialGroupId);
        DataSet GetPPCApprovalRequiredOperationId();
        DataSet GetMasterData(string gropuname, string parentid = "0");
        DataSet GetMaterialCreationPendingDetails(int userId);
        DataSet GetMaterialUpdatePendingDetails(int userId);
        DataSet GetMaterialCreationRejDraftDetails(string mm_ID, int userId);
        DataSet GetMaterialCreationRejectedDetails(int userId);
        DataSet GetMaterialUpdationRejectedDetails(int userId);
        DataSet MMDRAFT_HeaderDetailId_GET(int _mmHeaderId);
        DataSet MMEXTENDDRAFT_HDDETAIL_GET(int userId);
        DataSet GetMaterialExtendRejectedDetails(int userId);
        DataSet MMExtendRejectDraft_HDetail_Get(string MMExtDetailId, int userId);
        DataSet GetValuationClassGet(int MaterialTypeId);
        DataSet GetMaterialGroupGet(int materialType_Id);
        DataSet GetMaterialTypevaluation(int materialType_Id, int valuationClass_Id);
        DataSet GetMaterialTypeGroupvaluation(int materialType_Id, int valuationClass_Id, int materialGroup_Id);
        DataSet GetApprovalAuthorizationList(string _authTypeId);
        DataSet GetApprovalAuthorizeTypeList();
        DataSet GetAllPlantList();
        DataSet GetMasterTypeList();
        DataSet GetApprovalMatrixList(string _AuthType);
        DataSet GetMaterialMasterMatrixList(int _typeId);
        DataSet BindMaterialMasterEditDetails(int selectedEntry, int _typeId);
        DataSet GetAutoSearch(string prefixText);
        DataTable SearchMMDetails(string mmId, string requestType, int materialTypeId, int materialGroup, int valuationClass, int plantCode, string status, string fromDate, string toDate, int userId, int RequestorId);
        DataTable SearchMaterialDetails(string MMDetailId, string requestType, int userId);
        DataTable SearchMMHeaderStatusDetails(string mmId, string requestType, int materialTypeId, int materialGroup, int valuationClass, int plantCode, string status, string fromDate, string toDate, int userId, int RequestorId, long MaterialCode);
        DataSet GetMaterialApprovalUserDetails(string mmId);

        // --- Insert/Update (Requests & Master Data) ---
        string SubmitMaterialMasterApprovalUserRequest(int MMRequestId, int user_Id, int PPCAuth_Id, int FINAuth_Id);
        DataTable SearchCreateExtendMMDeatails(string mmId, string requestType, int materialTypeId, int materialGroup, int valuationClass, int plantCode, string status, string fromDate, string toDate, int userId);
        DataTable SearchCreateExtendMMDeatailsDownload(string mmId, string requestType, int materialTypeId, int materialGroup, int valuationClass, int plantCode, string status, string fromDate, string toDate, int userId);
        DataSet GetEmpNameMatrixList(string emp_Code);
        DataSet GetOperationMatrixList();
        DataSet GetDivisionOperMatrixList(string _operationID);

        // Creation / Update / Extend requests
        string AddMarerialCodeCreationRequest(int MMHeaderId, int plantCode, string materialDescription, string materialSpecification, int materialType, int materialGroup, int valuationClass, int measurementUnit, decimal price, int userId, string emailId, int PPCApprovalAuthority, int financeApprovalAuthority, string strhsncode, string strindicator);
        string AddMarerialCodeCreationWithSalesRequest(int MMHeaderId, int plantCode, string materialDescription, string materialSpecification, int materialType, int materialGroup, int valuationClass, int measurementUnit, decimal price, int userId, string emailId, int PPCApprovalAuthority, int financeApprovalAuthority, string strhsncode, string strindicator, string trans_grp, string loading_grp, string baseunitmeasure, string salesorg, string distri_chann, string item_categ_grp, string availability, string profitcentre, string storage_loc, string tax_class, string gen_item_cat_grp, string mat_grp_pac_matls, string pack_mat_type, string Int_Material_No);
        string AddMarerialCodeUpdateRequest(int MMHeaderId, int plantCode, string materialDescription, string materialSpecification, int measurementUnit, int userId, string emailId, int PPCApprovalAuthority, int financeApprovalAuthority, string strhsncode, string strindicator, string trans_grp, string loading_grp, string baseunitmeasure, string salesorg, string distri_chann, string item_categ_grp, string availability, string profitcentre, string storage_loc, string tax_class, string gen_item_cat_grp, string mat_grp_pac_matls, string pack_mat_type, string material_code, string mpn_profile, string plant_sp_material_status, string materialType);
        string UpdateMarerialCodeCreationRequest(int MMHeaderId, int MMDetailId, int plantCode, string materialDescription, string materialSpecification, int materialType, int materialGroup, int valuationClass, int measurementUnit, decimal price, string emailId, int PPCApprovalAuthority, int financeApprovalAuthority, string strhsncode, string strindicator, int userId, string trans_grp, string loading_grp, string baseunitmeasure, string salesorg, string distri_chann, string item_categ_grp, string availability, string profitcentre, string storage_loc, string tax_class, string gen_item_cat_grp, string mat_grp_pac_matls, string pack_mat_type, string int_material_no);
        string UpdateMarerialCodeUpdateRequest(int MMHeaderId, int MMDetailId, int plantCode, string materialDescription, string materialSpecification, int measurementUnit, string emailId, int PPCApprovalAuthority, int financeApprovalAuthority, string strhsncode, string strindicator, int userId, string trans_grp, string loading_grp, string baseunitmeasure, string salesorg, string distri_chann, string item_categ_grp, string availability, string profitcentre, string storage_loc, string tax_class, string gen_item_cat_grp, string mat_grp_pac_matls, string pack_mat_type, string material_code, string mpn_profile, string plant_sp_mat, string materialType);
        string SubmitMaterialMasterRequest(int MMRequestId, int _ApprovalCode);
        string SubmitMaterialMasterUPDATERequest(int MMRequestId, int _ApprovalCode);
        string SubmitMaterialMasterRequestDelete(int MMRequestId);
        DataSet GetMaterialDetailsByMatCode(long mmCode, int plant, int userId);
        DataSet GetMaterialCodeExist(long mmCode, int plant, int userId);
        DataSet GetMaterialCodeDetailUpdate(long mmCode, int plant, int userId);

        // Extend requests
        string AddMarerialCodeExtendRequest(int MMExheaderId, long materialCode, int plantCode, int userId, string emailId);
        string AddMarerialCodeExtendRequestSales(int MMExheaderId, long materialCode, int plantCode, int userId, string emailId, string trans_grp, string loading_grp, string baseunitmeasure, string salesorg, string distri_chann, string item_categ_grp, string availability, string profitcentre, string storage_loc, string tax_class, string gen_item_cat_grp, string mat_grp_pac_matls, string pack_mat_typ, string EXTENDREQTYPE);
        DataSet GetMMExtendDetailById(int MMDetailId);
        DataSet GetMMCreationDetailById(int MMDetailId);
        DataSet GetISPURCHASISSALEById(int MATERIAL_TYPE_ID);
        string UpdateMarerialCodeExtendRequest(int MMExheaderId, int MMDetailId, long materialCode, int plantCode, string emailId);
        string UpdateMarerialCodeExtendRequestSales(int MMExheaderId, int MMDetailId, long materialCode, int plantCode, string emailId, string trans_grp, string loading_grp, string baseunitmeasure, string salesorg, string distri_chann, string item_categ_grp, string availability, string profitcentre, string storage_loc, string tax_class, string gen_item_cat_grp, string mat_grp_pac_matls, string pack_mat_typ, string EXTENDREQTYPE);
        string DeleteMarerialCodeExtendRequest(int MMDetailId);
        string DeleteMarerialCodeCreationRequest(int MMDetailId);

        // Approval flows & details
        DataSet GetMMApprovalRequestList(int userId);
        DataTable GetMMApprovalCreateRequestList(string MMDetailId, string requestType, int userId);
        DataSet GetMMApprovalCreationRequestList(int userId);
        DataSet GetMMApprovalUpdationRequestList(int userId);
        DataSet GetMMApprovalExtendRequestList(int userId);
        string ApproveMMRequest(string MMDetailIdList, string remarks, int empCode, string status);
        DataSet GetMMRequestDetail(int MMDetailId, int userId);
        DataSet GetMMApproveRequestDetail(string MMDetailId, string requestType, int userId);
        DataSet GetMMApprovalDetail(int MMDetailId, int userId);
        DataTable SearchMaterialApprovalDetails(string MMDetailId, string requestType, int userId);

        // Valuation & Group mappings
        string MaterialValuationMapping_Add(int materialType_Id, int valuationClass_Id, int user_Id);
        string MaterialGroupTypeValuationMapping_Add(int materialType_Id, int valuationClass_Id, int materialGroup_Id, int user_Id);
        string DeleteMaterialTypevaluationmapping(int MTVDetailId, int _userId);

        // Approval Authority (Matrix)
        string MaterialApprovalAuthority_Add(string _authType_Id, int _authEmp_Id, string _plantId, int _masterTypeId, int user_Id, int _operationID, int _divisionID, int ActiveAuthority);
        string DeleteApprovalMatrixData(int MApproveDetailId, int _userId);
        DataSet GetApprovalMatrixListEdit(int MMPPCFINDetailId);
        string UpdateApprovalAuthorityRequest(int MMApproverId, int ActiveAuthority, int EMPCODE_IN);

        // Master data (Material types)
        string AddMaterialMasterData(int masterType, string materialType, string materialDescription, string materialCode, string _SiteName, string _SiteDesc, int userId);
        string EditMaterialMasterData(int masterType, int materialId, string materialType, string materialDescription, string materialCode, string _SiteName, string _SiteDesc, int userId);
        string DeleteMaterialMasterData(int masterType, int MasterDetailId, int _userId);

        // CSV helper
        DataTable CSVToDataTableFromFile(string StrFileName, string StrColumnSeperator, string StrRowSeperator, int InTotal_Column);

        // Material code verification + update
        DataTable SearchMaterialCode_VerifyCreation(string _RequestNo);
        DataTable SearchMaterialCode_VerifyCreationBlank(string _RequestNo, string materialCode);
        string UpdateMarerialCodeCreateRequestNo(string RequestNo, string materialCode, int UserId);
        DataTable SearchMaterialCode_VerifyExtend(string _RequestNo);
        DataTable SearchMaterialCode_VerifyExtendBlank(string _RequestNo, string materialCode);
        string UpdateMarerialCodeExtendRequestNo(string RequestNo, string materialCode, int UserId);

        // SIS / PPC / FIN authority pages & SIS user matrix
        string showMMSISAuthPage(string empcode);
        string showMMPPCFINAuthPage(string empcode);
        string MaterialSISUserAuthority_Add(int _sisauthEmp_Id, string _sisEmp_Name, int user_Id);
        string DeleteSISUserMatrixData(int MMSISUserDetailId, int _userId);

        // Pending counts / header-detail deletes / indicators
        string Material_PendingCountData(int _userId);
        DataSet Material_PendingCountDataAll(string EMPCODE_IN);
        string UpdateMarerialDetailPPCApproveRequest(int MMDetailId, int materialType, int materialGroup, int valuationClass);
        string InsertMDPPCApproveRequestChangeHistory(int MMDetailId, int materialType, int materialGroup, int valuationClass);
        DataTable SearchMaterialDetailsPPCChangeHistory(string MMDetailId, string requestType, int userId, string entry_Date);
        DataSet GetOperationApprovalAuthorityList(int _userId);
        DataSet GetOperationApprovalAuthorityDefaultList(int _userId);
        DataSet GetMaterialCountDetails(int MMHeaderId);
        DataSet MM_HeaderDetailDeleteId_GET(int _mmDetailId);
        DataSet GetMMIndicatorList(string indicatorid, string indicator);

        // Updation (Aumento changes)
        DataSet GetMaterialUpdationDetails(int MaterialCode, int Plant);
        string UpdateMaterialUpdationDetails(int MaterialCode, int Plant, int HSNCode, int Indicator, string PlantSpMatStatus, string Description);
    }
}
