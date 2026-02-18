using ePortal.Persistence.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Shared;
using System.Text.RegularExpressions;

namespace ePortal.Persistence.Services
{
    public class MasterMgmtRepo:IMasterMgmtRepo
    {
        #region "Local Variables"
        private string _ErrorMessage = string.Empty;
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set { _ErrorMessage = value; }
        }

        DataSet ds = new DataSet();
        DataRow[] _datarow;
        DataTable dt;
        IDataManagement oDataMgmt ;
        ICommonFunctions objcmn ;
        public MasterMgmtRepo(IDataManagement _oDataMgmt, ICommonFunctions _objcmn)
        {
            oDataMgmt = _oDataMgmt;
            objcmn = _objcmn;
        }

        #endregion
        /// <summary>
        /// Get list of finance approval user list
        /// </summary>
        /// <returns></returns>
        public DataSet GetPPCApprovalUserList()
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_PPCAPPROVALUSERLIST_GET";
            oCmd.Parameters.Add("CUR_PPCAPPROVALUSERLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //Get data from data access layer
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }

        public DataSet GetFinanceApprovalUserList()
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_FINAPPROVALUSERLIST_GET";
            oCmd.Parameters.Add("CUR_FINAPPROVALUSERLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //Get data from data access layer
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }

        /// <summary>
        /// GET ACTIVE PLANT LIST
        /// </summary>
        /// <returns></returns>
        public DataSet GetPlantList()
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_PLANTLIST_GET";
            oCmd.Parameters.Add("CUR_PLANTLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }


        /// <summary>
        /// GET ACTIVE PLANT LIST
        /// </summary>
        /// <returns></returns>
        public DataSet GetMasterPlantList()
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.MM_MASTERPLANTLIST_GET";
            oCmd.Parameters.Add("CUR_MASTERPLANTLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }
        ///GET PROFIT CENTRE
        ///Added by Ajit TTL
        public DataSet GetPlantDetails(string plantid)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.MM_PLANTPROFITCENTRE_GET";
            oCmd.Parameters.Add("CUR_MASTERPLANTLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("PLANTID_IN", OracleDbType.Varchar2).Value = plantid;
            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }

        /// <summary>
        /// GET ACTIVE PLANT LIST
        /// </summary>
        /// <returns></returns>
        public DataSet GetMaterialTypeList()
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MATERIALTYPELIST_GET";
            oCmd.Parameters.Add("CUR_MATERIALTYPELIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);

        }

        /// <summary>
        /// Get the list of Material Group
        /// </summary>
        /// <returns></returns>
        public DataSet GetMaterialGroupList()
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MATERIALGROUPLIST_GET";
            oCmd.Parameters.Add("CUR_MATERIALGROUPLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //Get data from data access layer
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        /// <summary>
        /// Get the list of Unit of Measurement
        /// </summary>
        /// <returns></returns>
        public DataSet GetUOMList()
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MATERIALUOMLIST_GET";
            oCmd.Parameters.Add("CUR_MATERIALUOMLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //Get data from data access layer
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        /// <summary>
        /// Get the list of Valuation Class according to Material Type
        /// </summary>
        /// <param name="MaterialTypeId"></param>
        /// <returns></returns>
        public DataSet GetValuationClassByMaterialType(int MaterialTypeId)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_VALUATIONCLS_BY_MTYPE";
            oCmd.Parameters.Add("P_MATERIALTYPEID", OracleDbType.Int32).Value = MaterialTypeId;
            oCmd.Parameters.Add("CUR_VALUATIONCLASSLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //Get data from data access layer
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }


        /// <summary>
        /// Get the list of Valuation Class according to Material Type
        /// </summary>
        /// <param name="MaterialTypeId"></param>
        /// <returns></returns>
        public DataSet GetMaterialGroupByMaterialType(int MaterialTypeId)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MATGROUP_BY_MTYPE";
            oCmd.Parameters.Add("P_MATERIALTYPEID", OracleDbType.Int32).Value = MaterialTypeId;
            oCmd.Parameters.Add("CUR_MATERIALGROUPLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //Get data from data access layer
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        /// <summary>
        /// Get the list of Valuation Class according to Material Type
        /// </summary>
        /// <param name="MaterialTypeId"></param>
        /// <returns></returns>
        public DataSet GetValuationClassByMaterialTypeGroup(int MaterialTypeId, int MaterialGroupId)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_VALCLASS_BY_MTYPEGRP";
            oCmd.Parameters.Add("P_MATERIALTYPEID", OracleDbType.Int32).Value = MaterialTypeId;
            oCmd.Parameters.Add("P_MATERIALGROUPID", OracleDbType.Int32).Value = MaterialGroupId;
            oCmd.Parameters.Add("CUR_VALUATIONCLASSLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //Get data from data access layer
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }
        /// <summary>
        /// Get department id for which PPC approval is required
        /// </summary>
        /// <returns></returns>
        public DataSet GetPPCApprovalRequiredOperationId()
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_PPCAPPROVALOPERATIONID";
            oCmd.Parameters.Add("CUR_PPCAPPROVALDEPID", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            DataSet ds = new DataSet();
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }
        /// <summary>
        /// Added by Ajit TTL
        /// Get Master Data
        /// </summary>
        /// <param name="gropuname"></param>
        /// <returns></returns>
        public DataSet GetMasterData(string gropuname, string parentid = "0")
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.MM_MASTERDATA";
            oCmd.Parameters.Add("GROUPNAME", OracleDbType.Varchar2).Value = gropuname.ToUpper().Trim();
            oCmd.Parameters.Add("PARENTID_IN", OracleDbType.Varchar2).Value = parentid.ToUpper().Trim();
            oCmd.Parameters.Add("CUR_MASTERDATA", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            DataSet ds = new DataSet();
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        /// <summary>
        /// Get list of material creation pending
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataSet GetMaterialCreationPendingDetails(int userId)
        {
            OracleCommand oCmd = new OracleCommand();
            DataSet ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMDRAFTHEADERDETAIL_GET";
            oCmd.Parameters.Add("CUR_MMCREATIONDRAFTHEADER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_MMCREATIONDRAFTDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = userId;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }
        //added by Ajit TTL for Update Material Request
        public DataSet GetMaterialUpdatePendingDetails(int userId)
        {
            OracleCommand oCmd = new OracleCommand();
            DataSet ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMDRAFTHEADERUPDATE_GET";
            oCmd.Parameters.Add("CUR_MMCREATIONDRAFTHEADER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_MMCREATIONDRAFTDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = userId;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        /// <summary>
        /// Get list of material creation pending
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataSet GetMaterialCreationRejDraftDetails(string mm_ID, int userId)
        {
            OracleCommand oCmd = new OracleCommand();
            DataSet ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMREJECTDRAFTDETAIL_GET";
            oCmd.Parameters.Add("CUR_MMCREATIONDRAFTHEADER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_MMCREATIONDRAFTDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("LMMHEADERID_IN", OracleDbType.Varchar2).Value = mm_ID;
            oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = userId;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }


        /// <summary>
        /// Get list of material creation Rejected Details
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataSet GetMaterialCreationRejectedDetails(int userId)
        {
            OracleCommand oCmd = new OracleCommand();
            DataSet ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMREJECTHEADERDETAIL_GET";
            oCmd.Parameters.Add("CUR_MMCREATIONDRAFTHEADER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_MMCREATIONDRAFTDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = userId;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }
        // added by Ajit TTL for update material request
        public DataSet GetMaterialUpdationRejectedDetails(int userId)
        {
            OracleCommand oCmd = new OracleCommand();
            DataSet ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMREJECTUPDATE_GET";
            oCmd.Parameters.Add("CUR_MMCREATIONDRAFTHEADER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_MMCREATIONDRAFTDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = userId;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        public DataSet MMDRAFT_HeaderDetailId_GET(int _mmHeaderId)
        {
            OracleCommand oCmd = new OracleCommand();
            DataSet ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MATERIALHEADERdETAIL_GET";
            oCmd.Parameters.Add("CUR_MATERIAL_CODEHEDDETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("MMHEADER_IN", OracleDbType.Int64).Value = _mmHeaderId;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }


        public DataSet MMEXTENDDRAFT_HDDETAIL_GET(int userId)
        {
            OracleCommand oCmd = new OracleCommand();
            DataSet ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.MMEXTENDDRAFT_HDDETAIL_GET";
            oCmd.Parameters.Add("CUR_MMEXTENDDRAFTHEADER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_MMEXTENDDRAFTDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = userId;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        /// <summary>
        /// Get list of material Extended Rejected Details
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataSet GetMaterialExtendRejectedDetails(int userId)
        {
            OracleCommand oCmd = new OracleCommand();
            DataSet ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.MMEXTREJREQ_HDDETAIL_GET";
            oCmd.Parameters.Add("CUR_MMCREATIONDRAFTHEADER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = userId;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        public DataSet MMExtendRejectDraft_HDetail_Get(string MMExtDetailId, int userId)
        {
            OracleCommand oCmd = new OracleCommand();
            DataSet ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.MMEXTREJDRAFT_HDDETAIL_GET";
            oCmd.Parameters.Add("CUR_MMCREATIONDRAFTDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = userId;
            oCmd.Parameters.Add("LMMHEADERID_IN", OracleDbType.Varchar2).Value = MMExtDetailId;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }


        /// <summary>
        /// Get the list of Valuation Class 
        /// </summary>
        /// <param name="MaterialTypeId"></param>
        /// <returns></returns>
        public DataSet GetValuationClassGet(int MaterialTypeId)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.MM_VALUATIONCLS_GET";
            oCmd.Parameters.Add("P_VALUATIONID", OracleDbType.Int32).Value = MaterialTypeId;
            oCmd.Parameters.Add("CUR_VALUATION_CLASSLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //Get data from data access layer
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        /// <summary>
        /// Get list of material Group master data
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataSet GetMaterialGroupGet(int materialType_Id)
        {
            OracleCommand oCmd = new OracleCommand();
            DataSet ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.MM_MATERIALGROUPCLS_GET";
            oCmd.Parameters.Add("CUR_MGROUP_CLASSLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("P_GROUPID", OracleDbType.Int64).Value = materialType_Id;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        /// <summary>
        /// Get list of material type valuation master mapping data
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataSet GetMaterialTypevaluation(int materialType_Id, int valuationClass_Id)
        {
            OracleCommand oCmd = new OracleCommand();
            DataSet ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.MM_TYPEVALUATIONMAP_GET";
            oCmd.Parameters.Add("CUR_MMTYPEVALDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("VMATERIALTYPEID", OracleDbType.Int64).Value = materialType_Id;
            oCmd.Parameters.Add("VVALUATIONID", OracleDbType.Int64).Value = valuationClass_Id;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        /// <summary>
        /// Get list of material type Group valuation master mapping data
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataSet GetMaterialTypeGroupvaluation(int materialType_Id, int valuationClass_Id, int materialGroup_Id)
        {
            OracleCommand oCmd = new OracleCommand();
            DataSet ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.MM_TYPEGROUPVALUATIONMAP_GET";
            oCmd.Parameters.Add("CUR_MMTYPEVALDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("VMATERIALTYPEID", OracleDbType.Int64).Value = materialType_Id;
            oCmd.Parameters.Add("VMATERIALGROUPID", OracleDbType.Int64).Value = materialGroup_Id;
            oCmd.Parameters.Add("VVALUATIONID", OracleDbType.Int64).Value = valuationClass_Id;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }
        /// <summary>
        /// GET ACTIVE Approval Authorization LIST
        /// </summary>
        /// <returns></returns>
        public DataSet GetApprovalAuthorizationList(string _authTypeId)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.MM_APPROVALAUTHORITY_GET";
            oCmd.Parameters.Add("VAUTHTYPE", OracleDbType.Varchar2).Value = _authTypeId;
            oCmd.Parameters.Add("CUR_MMAUTHORITYDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);

        }
        /// <summary>
        /// GET ACTIVE Approval Authorization Type LIST
        /// </summary>
        /// <returns></returns>
        public DataSet GetApprovalAuthorizeTypeList()
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.MM_APPROVAL_DEPT_GET";
            oCmd.Parameters.Add("CUR_MMAPPROVEDEPTDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);

        }

        /// <summary>
        /// GET ACTIVE PLANT LIST
        /// </summary>
        /// <returns></returns>
        public DataSet GetAllPlantList()
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.MM_ALLPLANTLIST_GET";
            oCmd.Parameters.Add("CUR_ALLPLANTLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);

        }

        /// <summary>
        /// GET ACTIVE  MASTER Type LIST
        /// </summary>
        /// <returns></returns>
        public DataSet GetMasterTypeList()
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.MM_MASTERTYPE_GET";
            oCmd.Parameters.Add("CUR_MMMASTERTYPEDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);

        }
        /// <summary>
        /// GET ACTIVE  Approval Matrix MASTER  Type LIST
        /// </summary>
        /// <returns></returns>
        public DataSet GetApprovalMatrixList(string _AuthType)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.MM_APPROVALMATRIX_GET";
            oCmd.Parameters.Add("CUR_MMAPPROVALDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("VAUTHORIZATIONTYPE", OracleDbType.Varchar2).Value = _AuthType;
            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }


        /// <summary>
        /// GET ACTIVE  Matrix MASTER  Type LIST
        /// </summary>
        /// <returns></returns>
        public DataSet GetMaterialMasterMatrixList(int _typeId)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.MM_MATERIALMASTER_GET";
            oCmd.Parameters.Add("CUR_MATERIALMASTERDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("VMASTERMATERIALTYPE", OracleDbType.Int64).Value = _typeId;
            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }


        /// <summary>
        /// GET ACTIVE  Matrix MASTER  Type LIST for edit
        /// </summary>
        /// <returns></returns>
        public DataSet BindMaterialMasterEditDetails(int selectedEntry, int _typeId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                ds = new DataSet();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.MM_MATERIALMASTEREDIT_GET";
                oCmd.Parameters.Add("CUR_MATERIALMASTEREDITDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("VMATERIALMASTERTYPE_IN", OracleDbType.Int64).Value = selectedEntry;
                oCmd.Parameters.Add("VMATERIALMASTERID_IN", OracleDbType.Int64).Value = _typeId;
                //GET DATA FROM DATA ACCESS LAYER
                ds = oDataMgmt.GetDataSet(oCmd);
                return (ds);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// GET ACTIVE MaterialCode  exist or not
        /// </summary>
        /// <returns></returns>
        public DataSet GetMaterialCodeExist(Int64 mmCode, int plant, int userId)
        {
            OracleCommand oCmd = new OracleCommand();
            DataSet ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.MMEXTEND_MATERIALCODE_GET"; //MMEXTENDDRAFT_HDDETAIL_GET
            oCmd.Parameters.Add("CUR_MATERIALCODEDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_MATERIALEXTENDDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("VMM_CODE", OracleDbType.Int64).Value = mmCode;
            oCmd.Parameters.Add("VPLANT", OracleDbType.Int64).Value = plant;
            oCmd.Parameters.Add("VUSERID_IN", OracleDbType.Int64).Value = userId;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }
        //Added by Ajit TTL for Material Update
        public DataSet GetMaterialCodeDetailUpdate(Int64 mmCode, int plant, int userId)
        {
            OracleCommand oCmd = new OracleCommand();
            DataSet ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.MMUPDATE_MATERIALCODE_GET";
            oCmd.Parameters.Add("CUR_MATERIALCODEDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_MATERIALEXTENDDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("VMM_CODE", OracleDbType.Int64).Value = mmCode;
            oCmd.Parameters.Add("VPLANT", OracleDbType.Int64).Value = plant;
            oCmd.Parameters.Add("VUSERID_IN", OracleDbType.Int64).Value = userId;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }
        public DataSet GetMaterialDetailsByMatCode(Int64 mmCode, int plant, int userId)
        {
            OracleCommand oCmd = new OracleCommand();
            DataSet ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.MMEXTEND_MATERIALCODE_GET"; //MMEXTENDDRAFT_HDDETAIL_GET
            oCmd.Parameters.Add("CUR_MATERIALCODEDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_MATERIALEXTENDDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("VMM_CODE", OracleDbType.Int64).Value = mmCode;
            oCmd.Parameters.Add("VPLANT", OracleDbType.Int64).Value = plant;
            oCmd.Parameters.Add("VUSERID_IN", OracleDbType.Int64).Value = userId;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        /// <summary>
        /// GET Autocomplete Search  LIST
        /// </summary>
        /// <returns></returns>
        public DataSet GetAutoSearch(string prefixText)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.MM_AUTOSEARCH";
            oCmd.Parameters.Add("CUR_AUTOSEARCH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("VPREFIX", OracleDbType.Varchar2).Value = prefixText;
            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }

        /// <summary>
        /// GET ACTIVE  MASTER Status Data
        /// </summary>
        /// <returns></returns>
        public DataTable SearchMMDetails(string mmId, string requestType, int materialTypeId, int materialGroup, int valuationClass, int plantCode, string status, string fromDate, string toDate, int userId, int RequestorId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                dt = new DataTable();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.MM_SEARCHMMDETAIL_GET"; // This procedure is also called in export to excel procedure.
                oCmd.Parameters.Add("CUR_MMDETAILSEARCH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("VMMID_IN", OracleDbType.Varchar2).Value = mmId;
                oCmd.Parameters.Add("VREQUESTOR_TYPE", OracleDbType.Varchar2).Value = requestType;
                oCmd.Parameters.Add("VMMTYPE_ID", OracleDbType.Int64).Value = materialTypeId;
                oCmd.Parameters.Add("VMMGROUP_ID", OracleDbType.Int64).Value = materialGroup;
                oCmd.Parameters.Add("VMMVALUATION_ID", OracleDbType.Int64).Value = valuationClass;
                oCmd.Parameters.Add("VPLANT_ID", OracleDbType.Int64).Value = plantCode;
                oCmd.Parameters.Add("VSTATUS", OracleDbType.Varchar2).Value = status;
                oCmd.Parameters.Add("V_FROMDATE", OracleDbType.Varchar2).Value = fromDate;
                oCmd.Parameters.Add("V_TODATE", OracleDbType.Varchar2).Value = toDate;
                oCmd.Parameters.Add("V_USERID", OracleDbType.Int64).Value = userId;
                oCmd.Parameters.Add("V_REQUESTORID", OracleDbType.Int64).Value = RequestorId;
                //GET DATA FROM DATA ACCESS LAYER
                // ds = oDataMgmt.GetDataSet(oCmd);
                dt = oDataMgmt.GetDataTable(oCmd);
                return dt;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// GET ACTIVE  MASTER Status Data
        /// </summary>
        /// <returns></returns>
        public DataTable SearchMaterialDetails(string MMDetailId, string requestType, int userId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                dt = new DataTable();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.MM_SEARCHMATERIALDETAIL_GET"; // This procedure is also called in export to excel procedure.
                oCmd.Parameters.Add("CUR_MMDETAILSEARCH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("VMMID_IN", OracleDbType.Varchar2).Value = MMDetailId;
                oCmd.Parameters.Add("VREQUESTOR_TYPE", OracleDbType.Varchar2).Value = requestType;
                oCmd.Parameters.Add("V_USERID", OracleDbType.Int64).Value = userId;
                //GET DATA FROM DATA ACCESS LAYER
                // ds = oDataMgmt.GetDataSet(oCmd);
                dt = oDataMgmt.GetDataTable(oCmd);
                return dt;
            }
            catch
            {
                throw;
            }
        }


        /// <summary>
        /// GET ACTIVE  MASTER hEADER Status Data
        /// </summary>
        /// <returns></returns>
        ///

        //Changed by Ajit TTL CR5865
        public DataTable SearchMMHeaderStatusDetails(string mmId, string requestType, int materialTypeId, int materialGroup, int valuationClass, int plantCode, string status, string fromDate, string toDate, int userId, int RequestorId, long MaterialCode)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                dt = new DataTable();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.MM_SEARCHMMHEADERSTATUS_GET"; // This procedure is also called in export to excel procedure.
                oCmd.Parameters.Add("CUR_MMDETAILSEARCH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("VMMID_IN", OracleDbType.Varchar2).Value = mmId;
                oCmd.Parameters.Add("VREQUESTOR_TYPE", OracleDbType.Varchar2).Value = requestType;
                oCmd.Parameters.Add("VMMTYPE_ID", OracleDbType.Int64).Value = materialTypeId;
                oCmd.Parameters.Add("VMMGROUP_ID", OracleDbType.Int64).Value = materialGroup;
                oCmd.Parameters.Add("VMMVALUATION_ID", OracleDbType.Int64).Value = valuationClass;
                oCmd.Parameters.Add("VPLANT_ID", OracleDbType.Int64).Value = plantCode;
                oCmd.Parameters.Add("VMATERIAL_CODE", OracleDbType.Int64).Value = MaterialCode;
                oCmd.Parameters.Add("VSTATUS", OracleDbType.Varchar2).Value = status;
                oCmd.Parameters.Add("V_FROMDATE", OracleDbType.Varchar2).Value = fromDate;
                oCmd.Parameters.Add("V_TODATE", OracleDbType.Varchar2).Value = toDate;
                oCmd.Parameters.Add("V_USERID", OracleDbType.Int64).Value = userId;
                oCmd.Parameters.Add("V_REQUESTORID", OracleDbType.Int64).Value = RequestorId;

                //GET DATA FROM DATA ACCESS LAYER
                // ds = oDataMgmt.GetDataSet(oCmd);
                dt = oDataMgmt.GetDataTable(oCmd);
                return dt;
            }
            catch
            {
                throw;
            }
        }


        /// <summary>
        /// GET ACTIVE  MASTER hEADER APPROVAL USER EDIT  Data
        /// </summary>
        /// <returns></returns>
        public DataSet GetMaterialApprovalUserDetails(string mmId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                ds = new DataSet();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.MM_APPROVALUSEREDIT_GET"; // This procedure is also called in export to excel procedure.
                oCmd.Parameters.Add("CUR_MMDETAILAPPROVE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("CUR_MMDETAILAPPROVEPPC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("CUR_MMDETAILAPPROVEFIN", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("VMMID_IN", OracleDbType.Varchar2).Value = mmId;
                //GET DATA FROM DATA ACCESS LAYER
                // ds = oDataMgmt.GetDataSet(oCmd);
                ds = oDataMgmt.GetDataSet(oCmd);
                return ds;
            }
            catch
            {
                throw;
            }
        }


        public string SubmitMaterialMasterApprovalUserRequest(int MMRequestId, int user_Id, int PPCAuth_Id, int FINAuth_Id)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMAPPROVALUPDATE_SUBMIT";
                oCmd.Parameters.Add("MMHEADERID_IN", OracleDbType.Int32).Value = MMRequestId;
                oCmd.Parameters.Add("MMUSERID_IN", OracleDbType.Int32).Value = user_Id;
                oCmd.Parameters.Add("MMPPCUSERID_IN", OracleDbType.Int32).Value = PPCAuth_Id;
                oCmd.Parameters.Add("MMFINUSERID_IN", OracleDbType.Int32).Value = FINAuth_Id;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("ERRORMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRORMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// GET ACTIVE  MASTER Status Data
        /// </summary>
        /// <returns></returns>
        public DataTable SearchCreateExtendMMDeatails(string mmId, string requestType, int materialTypeId, int materialGroup, int valuationClass, int plantCode, string status, string fromDate, string toDate, int userId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                dt = new DataTable();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.MM_CREATEEXTENDDETAIL_GET"; // This procedure is also called in export to excel procedure.
                oCmd.Parameters.Add("CUR_CREATEEXTENDSEARCH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("VMMID_IN", OracleDbType.Varchar2).Value = mmId;
                oCmd.Parameters.Add("VREQUESTOR_TYPE", OracleDbType.Varchar2).Value = requestType;
                oCmd.Parameters.Add("VMMTYPE_ID", OracleDbType.Int64).Value = materialTypeId;
                oCmd.Parameters.Add("VMMGROUP_ID", OracleDbType.Int64).Value = materialGroup;
                oCmd.Parameters.Add("VMMVALUATION_ID", OracleDbType.Int64).Value = valuationClass;
                oCmd.Parameters.Add("VPLANT_ID", OracleDbType.Int64).Value = plantCode;
                oCmd.Parameters.Add("VSTATUS", OracleDbType.Varchar2).Value = status;
                oCmd.Parameters.Add("V_FROMDATE", OracleDbType.Varchar2).Value = fromDate;
                oCmd.Parameters.Add("V_TODATE", OracleDbType.Varchar2).Value = toDate;
                oCmd.Parameters.Add("V_USERID", OracleDbType.Int64).Value = userId;
                //GET DATA FROM DATA ACCESS LAYER
                // ds = oDataMgmt.GetDataSet(oCmd);
                dt = oDataMgmt.GetDataTable(oCmd);
                return dt;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// GET ACTIVE  MASTER Status Data
        /// </summary>
        /// <returns></returns>
        public DataTable SearchCreateExtendMMDeatailsDownload(string mmId, string requestType, int materialTypeId, int materialGroup, int valuationClass, int plantCode, string status, string fromDate, string toDate, int userId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                dt = new DataTable();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.MM_CREATEEXTDOWNLOADDETAIL_GET"; // This procedure is also called in export to excel procedure.
                oCmd.Parameters.Add("CUR_CREATEEXTENDSEARCH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("VMMID_IN", OracleDbType.Varchar2).Value = mmId;
                oCmd.Parameters.Add("VREQUESTOR_TYPE", OracleDbType.Varchar2).Value = requestType;
                oCmd.Parameters.Add("VMMTYPE_ID", OracleDbType.Int64).Value = materialTypeId;
                oCmd.Parameters.Add("VMMGROUP_ID", OracleDbType.Int64).Value = materialGroup;
                oCmd.Parameters.Add("VMMVALUATION_ID", OracleDbType.Int64).Value = valuationClass;
                oCmd.Parameters.Add("VPLANT_ID", OracleDbType.Int64).Value = plantCode;
                oCmd.Parameters.Add("VSTATUS", OracleDbType.Varchar2).Value = status;
                oCmd.Parameters.Add("V_FROMDATE", OracleDbType.Varchar2).Value = fromDate;
                oCmd.Parameters.Add("V_TODATE", OracleDbType.Varchar2).Value = toDate;
                oCmd.Parameters.Add("V_USERID", OracleDbType.Int64).Value = userId;
                //GET DATA FROM DATA ACCESS LAYER
                // ds = oDataMgmt.GetDataSet(oCmd);
                dt = oDataMgmt.GetDataTable(oCmd);
                return dt;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// GET ACTIVE  Emplyee name MASTER  LIST
        /// </summary>
        /// <returns></returns>
        public DataSet GetEmpNameMatrixList(string emp_Code)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                ds = new DataSet();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.EMP_NAMESEARCH";
                oCmd.Parameters.Add("CUR_EMPNAMESEARCH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = emp_Code;

                //GET DATA FROM DATA ACCESS LAYER
                ds = oDataMgmt.GetDataSet(oCmd);
                return (ds);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// GET ACTIVE  Emplyee name MASTER  LIST
        /// </summary>
        /// <returns></returns>
        public DataSet GetOperationMatrixList()
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                ds = new DataSet();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.MM_OPERATIONSEARCH";
                oCmd.Parameters.Add("CUR_OPERATIONSEARCH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                //Get data from data access layer
                ds = oDataMgmt.GetDataSet(oCmd);
                return (ds);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// GET ACTIVE  Emplyee name MASTER  LIST
        /// </summary>
        /// <returns></returns>
        public DataSet GetDivisionOperMatrixList(string _operationID)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                ds = new DataSet();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.MM_DIVISIONOPERSEARCH";
                oCmd.Parameters.Add("CUR_DIVISIONSEARCH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("OPERATINOID_IN", OracleDbType.Int64).Value = _operationID;
                //GET DATA FROM DATA ACCESS LAYER
                ds = oDataMgmt.GetDataSet(oCmd);
                return (ds);
            }
            catch
            {
                throw;
            }
        }



        #region "INSERT/UPDATE QUERIES"

        public string AddMarerialCodeCreationRequest(int MMHeaderId, int plantCode, string materialDescription, string materialSpecification, int materialType, int materialGroup, int valuationClass, int measurementUnit, decimal price, int userId, string emailId, int PPCApprovalAuthority, int financeApprovalAuthority, string strhsncode, string strindicator)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MATERIALCODEREQUEST_ADD";
                oCmd.Parameters.Add("MMHEADERID_IN", OracleDbType.Int64).Value = MMHeaderId;
                oCmd.Parameters.Add("PLANTCODE_IN", OracleDbType.Int64).Value = plantCode;
                oCmd.Parameters.Add("MATERIALDESCRIPTION_IN", OracleDbType.Varchar2).Value = materialDescription;
                oCmd.Parameters.Add("MATERIALSPECIFICATION_IN", OracleDbType.Varchar2).Value = materialSpecification;
                oCmd.Parameters.Add("MATERIALTYPE_IN", OracleDbType.Int64).Value = materialType;
                oCmd.Parameters.Add("MATERIALGROUP_IN", OracleDbType.Int64).Value = materialGroup;
                oCmd.Parameters.Add("VALUATIONCLASS_IN", OracleDbType.Int64).Value = valuationClass;
                oCmd.Parameters.Add("MEASUREMENTUNIT_IN", OracleDbType.Int64).Value = measurementUnit;
                oCmd.Parameters.Add("PRICE_IN", OracleDbType.Int64).Value = price;
                oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = userId;
                oCmd.Parameters.Add("EMAILID_IN", OracleDbType.Varchar2).Value = emailId;
                oCmd.Parameters.Add("HSNCODE_IN", OracleDbType.Varchar2).Value = strhsncode;
                oCmd.Parameters.Add("INDICATOR_IN", OracleDbType.Varchar2).Value = strindicator;
                //oCmd.Parameters.Add("REQUESTTYPE_IN", OracleDbType.VarChar).Value = requestType;
                //oCmd.Parameters.Add("PPCAPPROVALAUTHORITY_IN", OracleDbType.Int64).Value = PPCApprovalAuthority;
                //oCmd.Parameters.Add("FINANCEAPPROVALAUTHORITY_IN", OracleDbType.Int64).Value = financeApprovalAuthority;
                oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }
        // Added by Ajit TTL for Create Material with Sales View
        public string AddMarerialCodeCreationWithSalesRequest(int MMHeaderId, int plantCode, string materialDescription, string materialSpecification, int materialType, int materialGroup, int valuationClass, int measurementUnit, decimal price, int userId, string emailId, int PPCApprovalAuthority, int financeApprovalAuthority, string strhsncode, string strindicator, string trans_grp, string loading_grp, string baseunitmeasure, string salesorg, string distri_chann, string item_categ_grp, string availability, string profitcentre, string storage_loc, string tax_class, string gen_item_cat_grp, string mat_grp_pac_matls, string pack_mat_type, string Int_Material_No)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MATERIALCODEREQUESTSALES_ADD";
                oCmd.Parameters.Add("MMHEADERID_IN", OracleDbType.Int64).Value = MMHeaderId;
                oCmd.Parameters.Add("PLANTCODE_IN", OracleDbType.Int64).Value = plantCode;
                oCmd.Parameters.Add("MATERIALDESCRIPTION_IN", OracleDbType.Varchar2).Value = materialDescription;
                oCmd.Parameters.Add("MATERIALSPECIFICATION_IN", OracleDbType.Varchar2).Value = materialSpecification;
                oCmd.Parameters.Add("MATERIALTYPE_IN", OracleDbType.Int64).Value = materialType;
                oCmd.Parameters.Add("MATERIALGROUP_IN", OracleDbType.Int64).Value = materialGroup;
                oCmd.Parameters.Add("VALUATIONCLASS_IN", OracleDbType.Int64).Value = valuationClass;
                oCmd.Parameters.Add("MEASUREMENTUNIT_IN", OracleDbType.Int64).Value = measurementUnit;
                oCmd.Parameters.Add("PRICE_IN", OracleDbType.Int64).Value = price;
                oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = userId;
                oCmd.Parameters.Add("EMAILID_IN", OracleDbType.Varchar2).Value = emailId;
                oCmd.Parameters.Add("HSNCODE_IN", OracleDbType.Varchar2).Value = strhsncode;
                oCmd.Parameters.Add("INDICATOR_IN", OracleDbType.Varchar2).Value = strindicator;
                //oCmd.Parameters.Add("REQUESTTYPE_IN", OracleDbType.VarChar).Value = requestType;
                //oCmd.Parameters.Add("PPCAPPROVALAUTHORITY_IN", OracleDbType.Int64).Value = PPCApprovalAuthority;
                //oCmd.Parameters.Add("FINANCEAPPROVALAUTHORITY_IN", OracleDbType.Int64).Value = financeApprovalAuthority;
                oCmd.Parameters.Add("TRANSPORTATIONGROUP_IN", OracleDbType.Varchar2).Value = trans_grp;
                oCmd.Parameters.Add("LOADINGGROUP_IN", OracleDbType.Varchar2).Value = loading_grp;
                oCmd.Parameters.Add("BASEUNITOFMEASURE_IN", OracleDbType.Varchar2).Value = baseunitmeasure;
                oCmd.Parameters.Add("PROFITCENTER_IN", OracleDbType.Varchar2).Value = profitcentre;
                oCmd.Parameters.Add("SALES_ORG_IN", OracleDbType.Varchar2).Value = salesorg;
                oCmd.Parameters.Add("DISTRI_CHN_IN", OracleDbType.Varchar2).Value = distri_chann;
                oCmd.Parameters.Add("ITEM_CATG_GRP_IN", OracleDbType.Varchar2).Value = item_categ_grp;
                oCmd.Parameters.Add("AVAIL_CHK_IN", OracleDbType.Varchar2).Value = availability;
                oCmd.Parameters.Add("STORAGE_LOC_IN", OracleDbType.Varchar2).Value = storage_loc;

                oCmd.Parameters.Add("TAX_CLASSIFICATION_IN", OracleDbType.Varchar2).Value = tax_class;
                oCmd.Parameters.Add("GEN_ITEM_CAT_GRP_IN", OracleDbType.Varchar2).Value = gen_item_cat_grp;
                oCmd.Parameters.Add("MAT_GRP_PACK_MATLS_IN", OracleDbType.Varchar2).Value = mat_grp_pac_matls;
                oCmd.Parameters.Add("PACKAGING_MAT_TYPE_IN", OracleDbType.Varchar2).Value = pack_mat_type;
                oCmd.Parameters.Add("INT_MATERIAL_NO_IN", OracleDbType.Varchar2).Value = Int_Material_No;

                oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }
        // Added by Ajit TTL for Update Materila request
        public string AddMarerialCodeUpdateRequest(int MMHeaderId, int plantCode, string materialDescription, string materialSpecification, int measurementUnit, int userId, string emailId, int PPCApprovalAuthority, int financeApprovalAuthority, string strhsncode, string strindicator, string trans_grp, string loading_grp, string baseunitmeasure, string salesorg, string distri_chann, string item_categ_grp, string availability, string profitcentre, string storage_loc, string tax_class, string gen_item_cat_grp, string mat_grp_pac_matls, string pack_mat_type, string material_code, string mpn_profile, string plant_sp_material_status, string materialType)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MATERIALCODEUPDATEREQ_ADD";
                oCmd.Parameters.Add("MMHEADERID_IN", OracleDbType.Int64).Value = MMHeaderId;
                oCmd.Parameters.Add("PLANTCODE_IN", OracleDbType.Int64).Value = plantCode;
                oCmd.Parameters.Add("MATERIALDESCRIPTION_IN", OracleDbType.Varchar2).Value = materialDescription;
                oCmd.Parameters.Add("MATERIALSPECIFICATION_IN", OracleDbType.Varchar2).Value = materialSpecification;
                oCmd.Parameters.Add("MEASUREMENTUNIT_IN", OracleDbType.Int64).Value = measurementUnit;
                oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = userId;
                oCmd.Parameters.Add("EMAILID_IN", OracleDbType.Varchar2).Value = emailId;
                oCmd.Parameters.Add("HSNCODE_IN", OracleDbType.Varchar2).Value = strhsncode;
                oCmd.Parameters.Add("INDICATOR_IN", OracleDbType.Varchar2).Value = strindicator;
                //oCmd.Parameters.Add("REQUESTTYPE_IN", OracleDbType.VarChar).Value = requestType;
                //oCmd.Parameters.Add("PPCAPPROVALAUTHORITY_IN", OracleDbType.Int64).Value = PPCApprovalAuthority;
                //oCmd.Parameters.Add("FINANCEAPPROVALAUTHORITY_IN", OracleDbType.Int64).Value = financeApprovalAuthority;
                oCmd.Parameters.Add("TRANSPORTATIONGROUP_IN", OracleDbType.Varchar2).Value = trans_grp;
                oCmd.Parameters.Add("LOADINGGROUP_IN", OracleDbType.Varchar2).Value = loading_grp;
                oCmd.Parameters.Add("BASEUNITOFMEASURE_IN", OracleDbType.Varchar2).Value = baseunitmeasure;
                oCmd.Parameters.Add("PROFITCENTER_IN", OracleDbType.Varchar2).Value = profitcentre;
                oCmd.Parameters.Add("SALES_ORG_IN", OracleDbType.Varchar2).Value = salesorg;
                oCmd.Parameters.Add("DISTRI_CHN_IN", OracleDbType.Varchar2).Value = distri_chann;
                oCmd.Parameters.Add("ITEM_CATG_GRP_IN", OracleDbType.Varchar2).Value = item_categ_grp;
                oCmd.Parameters.Add("AVAIL_CHK_IN", OracleDbType.Varchar2).Value = availability;
                oCmd.Parameters.Add("STORAGE_LOC_IN", OracleDbType.Varchar2).Value = storage_loc;

                oCmd.Parameters.Add("TAX_CLASSIFICATION_IN", OracleDbType.Varchar2).Value = tax_class;
                oCmd.Parameters.Add("GEN_ITEM_CAT_GRP_IN", OracleDbType.Varchar2).Value = gen_item_cat_grp;
                oCmd.Parameters.Add("MAT_GRP_PACK_MATLS_IN", OracleDbType.Varchar2).Value = mat_grp_pac_matls;
                oCmd.Parameters.Add("PACKAGING_MAT_TYPE_IN", OracleDbType.Varchar2).Value = pack_mat_type;

                oCmd.Parameters.Add("MPN_PROFILE_IN", OracleDbType.Varchar2).Value = mpn_profile;
                oCmd.Parameters.Add("PLANT_SP_MAT_STATUS_IN", OracleDbType.Varchar2).Value = plant_sp_material_status;
                oCmd.Parameters.Add("MATERIALCODE_IN", OracleDbType.Varchar2).Value = material_code;
                oCmd.Parameters.Add("MATERIALTYPE_IN", OracleDbType.Varchar2).Value = materialType;

                oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }
        // Modified by Ajit TTL for update with Salesview
        public string UpdateMarerialCodeCreationRequest(int MMHeaderId, int MMDetailId, int plantCode, string materialDescription, string materialSpecification, int materialType, int materialGroup, int valuationClass, int measurementUnit, decimal price, string emailId, int PPCApprovalAuthority, int financeApprovalAuthority, string strhsncode, string strindicator, int userId, string trans_grp, string loading_grp, string baseunitmeasure, string salesorg, string distri_chann, string item_categ_grp, string availability, string profitcentre, string storage_loc, string tax_class, string gen_item_cat_grp, string mat_grp_pac_matls, string pack_mat_type, string int_material_no)
        {
            try
            {

                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMCREATIONREQUEST_UPDATE";
                oCmd.Parameters.Add("MMHEADERID_IN", OracleDbType.Int64).Value = MMHeaderId;
                oCmd.Parameters.Add("MMDETAILID_IN", OracleDbType.Int64).Value = MMDetailId;
                oCmd.Parameters.Add("PLANTCODE_IN", OracleDbType.Int64).Value = plantCode;
                oCmd.Parameters.Add("MATERIALDESCRIPTION_IN", OracleDbType.Varchar2).Value = materialDescription;
                oCmd.Parameters.Add("MATERIALSPECIFICATION_IN", OracleDbType.Varchar2).Value = materialSpecification;
                oCmd.Parameters.Add("MATERIALTYPE_IN", OracleDbType.Int64).Value = materialType;
                oCmd.Parameters.Add("MATERIALGROUP_IN", OracleDbType.Int64).Value = materialGroup;
                oCmd.Parameters.Add("VALUATIONCLASS_IN", OracleDbType.Int64).Value = valuationClass;
                oCmd.Parameters.Add("MEASUREMENTUNIT_IN", OracleDbType.Int64).Value = measurementUnit;
                oCmd.Parameters.Add("PRICE_IN", OracleDbType.Int64).Value = price;
                oCmd.Parameters.Add("EMAILID_IN", OracleDbType.Varchar2).Value = emailId;
                oCmd.Parameters.Add("PPCAPPROVALAUTHORITY_IN", OracleDbType.Int64).Value = PPCApprovalAuthority;
                oCmd.Parameters.Add("FINANCEAPPROVALAUTHORITY_IN", OracleDbType.Int64).Value = financeApprovalAuthority;
                oCmd.Parameters.Add("HSNCODE_IN", OracleDbType.Varchar2).Value = strhsncode;
                oCmd.Parameters.Add("INDICATOR_IN", OracleDbType.Varchar2).Value = strindicator;

                oCmd.Parameters.Add("TRANSPORTATIONGROUP_IN", OracleDbType.Varchar2).Value = trans_grp;
                oCmd.Parameters.Add("LOADINGGROUP_IN", OracleDbType.Varchar2).Value = loading_grp;
                oCmd.Parameters.Add("BASEUNITOFMEASURE_IN", OracleDbType.Varchar2).Value = baseunitmeasure;
                oCmd.Parameters.Add("PROFITCENTER_IN", OracleDbType.Varchar2).Value = profitcentre;
                oCmd.Parameters.Add("SALES_ORG_IN", OracleDbType.Varchar2).Value = salesorg;
                oCmd.Parameters.Add("DISTRI_CHN_IN", OracleDbType.Varchar2).Value = distri_chann;
                oCmd.Parameters.Add("ITEM_CATG_GRP_IN", OracleDbType.Varchar2).Value = item_categ_grp;
                oCmd.Parameters.Add("AVAIL_CHK_IN", OracleDbType.Varchar2).Value = availability;
                oCmd.Parameters.Add("STORAGE_LOC_IN", OracleDbType.Varchar2).Value = storage_loc;
                oCmd.Parameters.Add("TAX_CLASSIFICATION_IN", OracleDbType.Varchar2).Value = tax_class;
                oCmd.Parameters.Add("GEN_ITEM_CAT_GRP_IN", OracleDbType.Varchar2).Value = gen_item_cat_grp;
                oCmd.Parameters.Add("MAT_GRP_PACK_MATLS_IN", OracleDbType.Varchar2).Value = mat_grp_pac_matls;
                oCmd.Parameters.Add("PACKAGING_MAT_TYPE_IN", OracleDbType.Varchar2).Value = pack_mat_type;
                oCmd.Parameters.Add("INT_MATERIAL_NO_IN", OracleDbType.Varchar2).Value = int_material_no;

                oCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                //oCmd.Parameters.Add("ERRMSG", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = userId;
                oDataMgmt.ExecuteQuery(oCmd);
                // return "1";
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRMSG"].Value.ToString();

            }
            catch
            {
                throw;
            }
        }

        //ADDED AJIT TTL FOR UPDATE_REQ UPDATE
        public string UpdateMarerialCodeUpdateRequest(int MMHeaderId, int MMDetailId, int plantCode, string materialDescription, string materialSpecification, int measurementUnit, string emailId, int PPCApprovalAuthority, int financeApprovalAuthority, string strhsncode, string strindicator, int userId, string trans_grp, string loading_grp, string baseunitmeasure, string salesorg, string distri_chann, string item_categ_grp, string availability, string profitcentre, string storage_loc, string tax_class, string gen_item_cat_grp, string mat_grp_pac_matls, string pack_mat_type, string material_code, string mpn_profile, string plant_sp_mat, string materialType)
        {
            try
            {

                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMUPDATEREQUEST_UPDATE";
                oCmd.Parameters.Add("MMHEADERID_IN", OracleDbType.Int64).Value = MMHeaderId;
                oCmd.Parameters.Add("MMDETAILID_IN", OracleDbType.Int64).Value = MMDetailId;
                oCmd.Parameters.Add("PLANTCODE_IN", OracleDbType.Int64).Value = plantCode;
                oCmd.Parameters.Add("MATERIALDESCRIPTION_IN", OracleDbType.Varchar2).Value = materialDescription;
                oCmd.Parameters.Add("MATERIALSPECIFICATION_IN", OracleDbType.Varchar2).Value = materialSpecification;

                oCmd.Parameters.Add("MEASUREMENTUNIT_IN", OracleDbType.Int64).Value = measurementUnit;

                oCmd.Parameters.Add("EMAILID_IN", OracleDbType.Varchar2).Value = emailId;
                oCmd.Parameters.Add("PPCAPPROVALAUTHORITY_IN", OracleDbType.Int64).Value = PPCApprovalAuthority;
                oCmd.Parameters.Add("FINANCEAPPROVALAUTHORITY_IN", OracleDbType.Int64).Value = financeApprovalAuthority;
                oCmd.Parameters.Add("HSNCODE_IN", OracleDbType.Varchar2).Value = strhsncode;
                oCmd.Parameters.Add("INDICATOR_IN", OracleDbType.Varchar2).Value = strindicator;

                oCmd.Parameters.Add("TRANSPORTATIONGROUP_IN", OracleDbType.Varchar2).Value = trans_grp;
                oCmd.Parameters.Add("LOADINGGROUP_IN", OracleDbType.Varchar2).Value = loading_grp;
                oCmd.Parameters.Add("BASEUNITOFMEASURE_IN", OracleDbType.Varchar2).Value = baseunitmeasure;
                oCmd.Parameters.Add("PROFITCENTER_IN", OracleDbType.Varchar2).Value = profitcentre;
                oCmd.Parameters.Add("SALES_ORG_IN", OracleDbType.Varchar2).Value = salesorg;
                oCmd.Parameters.Add("DISTRI_CHN_IN", OracleDbType.Varchar2).Value = distri_chann;
                oCmd.Parameters.Add("ITEM_CATG_GRP_IN", OracleDbType.Varchar2).Value = item_categ_grp;
                oCmd.Parameters.Add("AVAIL_CHK_IN", OracleDbType.Varchar2).Value = availability;
                oCmd.Parameters.Add("STORAGE_LOC_IN", OracleDbType.Varchar2).Value = storage_loc;
                oCmd.Parameters.Add("TAX_CLASSIFICATION_IN", OracleDbType.Varchar2).Value = tax_class;
                oCmd.Parameters.Add("GEN_ITEM_CAT_GRP_IN", OracleDbType.Varchar2).Value = gen_item_cat_grp;
                oCmd.Parameters.Add("MAT_GRP_PACK_MATLS_IN", OracleDbType.Varchar2).Value = mat_grp_pac_matls;
                oCmd.Parameters.Add("PACKAGING_MAT_TYPE_IN", OracleDbType.Varchar2).Value = pack_mat_type;

                oCmd.Parameters.Add("MPN_PROFILE_IN", OracleDbType.Varchar2).Value = mpn_profile;
                oCmd.Parameters.Add("PLANT_SP_MAT_STATUS_IN", OracleDbType.Varchar2).Value = plant_sp_mat;
                oCmd.Parameters.Add("MATERIALCODE_IN", OracleDbType.Varchar2).Value = material_code;
                oCmd.Parameters.Add("MATERIALTYPE_IN", OracleDbType.Varchar2).Value = materialType;

                oCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                //oCmd.Parameters.Add("ERRMSG", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = userId;
                oDataMgmt.ExecuteQuery(oCmd);
                // return "1";
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRMSG"].Value.ToString();

            }
            catch
            {
                throw;
            }
        }



        public string SubmitMaterialMasterRequest(int MMRequestId, int _ApprovalCode)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMREQUEST_SUBMIT";
                oCmd.Parameters.Add("MMHEADERID_IN", OracleDbType.Int32).Value = MMRequestId;
                oCmd.Parameters.Add("MMAPROVECODE_IN", OracleDbType.Int32).Value = _ApprovalCode;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("ERRORMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRORMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }
        //ADDED BY AJIT TTL
        public string SubmitMaterialMasterUPDATERequest(int MMRequestId, int _ApprovalCode)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                oCmd.CommandType = CommandType.StoredProcedure;
                //oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMUPDATEREQUEST_SUBMIT";
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMREQUEST_SUBMIT";
                oCmd.Parameters.Add("MMHEADERID_IN", OracleDbType.Int32).Value = MMRequestId;
                oCmd.Parameters.Add("MMAPROVECODE_IN", OracleDbType.Int32).Value = _ApprovalCode;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("ERRORMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRORMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }
        public string SubmitMaterialMasterRequestDelete(int MMRequestId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMREQUESTDELETE_SUBMIT";
                oCmd.Parameters.Add("MMHEADERID_IN", OracleDbType.Int32).Value = MMRequestId;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("ERRORMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRORMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }

        public string AddMarerialCodeExtendRequest(int MMExheaderId, Int64 materialCode, int plantCode, int userId, string emailId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMEXTENDREQUEST_ADD";
                oCmd.Parameters.Add("MMEXHEADERID_IN", OracleDbType.Int64).Value = MMExheaderId;
                oCmd.Parameters.Add("MATERIALCODE_IN", OracleDbType.Int64).Value = materialCode;
                oCmd.Parameters.Add("PLANTCODE_IN", OracleDbType.Int64).Value = plantCode;
                //oCmd.Parameters.Add("MMDESC_IN", OracleDbType.Varchar2, 100).Value = txt_description;
                //oCmd.Parameters.Add("MMSPEC", OracleDbType.Varchar2, 100).Value = txt_specification;
                oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = userId;
                oCmd.Parameters.Add("EMAILID_IN", OracleDbType.Varchar2, 100).Value = emailId;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("ERRORMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRORMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }
        //Added by Ajit TTL for Extend Material Request with Sales View
        public string AddMarerialCodeExtendRequestSales(int MMExheaderId, Int64 materialCode, int plantCode, int userId, string emailId, string trans_grp, string loading_grp, string baseunitmeasure, string salesorg, string distri_chann, string item_categ_grp, string availability, string profitcentre, string storage_loc, string tax_class, string gen_item_cat_grp, string mat_grp_pac_matls, string pack_mat_typ, string EXTENDREQTYPE)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMEXTENDREQUESTSALES_ADD";
                oCmd.Parameters.Add("MMEXHEADERID_IN", OracleDbType.Int64).Value = MMExheaderId;
                oCmd.Parameters.Add("MATERIALCODE_IN", OracleDbType.Int64).Value = materialCode;
                oCmd.Parameters.Add("PLANTCODE_IN", OracleDbType.Int64).Value = plantCode;
                oCmd.Parameters.Add("TRANSPORTATIONGROUP_IN", OracleDbType.Varchar2).Value = trans_grp;
                oCmd.Parameters.Add("LOADINGGROUP_IN", OracleDbType.Varchar2).Value = loading_grp;
                oCmd.Parameters.Add("BASEUNITOFMEASURE_IN", OracleDbType.Varchar2).Value = baseunitmeasure;
                oCmd.Parameters.Add("PROFITCENTER_IN", OracleDbType.Varchar2).Value = profitcentre;
                oCmd.Parameters.Add("SALES_ORG_IN", OracleDbType.Varchar2).Value = salesorg;
                oCmd.Parameters.Add("DISTRI_CHN_IN", OracleDbType.Varchar2).Value = distri_chann;
                oCmd.Parameters.Add("ITEM_CATG_GRP_IN", OracleDbType.Varchar2).Value = item_categ_grp;
                oCmd.Parameters.Add("AVAIL_CHK_IN", OracleDbType.Varchar2).Value = availability;
                oCmd.Parameters.Add("STORAGE_LOC_IN", OracleDbType.Varchar2).Value = storage_loc;
                oCmd.Parameters.Add("TAX_CLASSIFICATION_IN", OracleDbType.Varchar2).Value = tax_class;
                oCmd.Parameters.Add("GEN_ITEM_CAT_GRP_IN", OracleDbType.Varchar2).Value = gen_item_cat_grp;
                oCmd.Parameters.Add("MAT_GRP_PACK_MATLS_IN", OracleDbType.Varchar2).Value = mat_grp_pac_matls;
                oCmd.Parameters.Add("PACKAGING_MAT_TYPE_IN", OracleDbType.Varchar2).Value = pack_mat_typ;
                oCmd.Parameters.Add("EXTENDREQTYPE_IN", OracleDbType.Int64).Value = Convert.ToInt64(EXTENDREQTYPE);
                //oCmd.Parameters.Add("MMDESC_IN", OracleDbType.Varchar2, 100).Value = txt_description;
                //oCmd.Parameters.Add("MMSPEC", OracleDbType.Varchar2, 100).Value = txt_specification;
                oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = userId;
                oCmd.Parameters.Add("EMAILID_IN", OracleDbType.Varchar2, 100).Value = emailId;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("ERRORMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRORMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }
        public DataSet GetMMExtendDetailById(int MMDetailId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMEXTENDDETAILBYID_GET";
                oCmd.Parameters.Add("CUR_MMEXTENDDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("MMDETAILID_IN", OracleDbType.Int64).Value = MMDetailId;
                //oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                //oCmd.Parameters.Add("ERRORMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                ds = oDataMgmt.GetDataSet(oCmd);
                return ds;
                //if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                //return "1";
                //else
                //return oCmd.Parameters["ERRORMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }

        public DataSet GetMMCreationDetailById(int MMDetailId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMCREATIONDETAILBYID_GET";
                oCmd.Parameters.Add("CUR_MMEXTENDDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("MMDETAILID_IN", OracleDbType.Int64).Value = MMDetailId;
                ds = oDataMgmt.GetDataSet(oCmd);
                return ds;
            }
            catch
            {
                throw;
            }
        }

        ///ADDED BY AJIT TTL for Update Material Type with Sales View
        /// /////
        public DataSet GetISPURCHASISSALEById(int MATERIAL_TYPE_ID)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MATERIALTYPE_ISPURCHASE";
                oCmd.Parameters.Add("CUR_MATERIALTYPELIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("MATERIAL_ID", OracleDbType.Int64).Value = MATERIAL_TYPE_ID;
                ds = oDataMgmt.GetDataSet(oCmd);
                return ds;
            }
            catch
            {
                throw;
            }
        }
        /// 


        public string UpdateMarerialCodeExtendRequest(int MMExheaderId, int MMDetailId, Int64 materialCode, int plantCode, string emailId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMEXTENDREQUEST_UPDATE";
                oCmd.Parameters.Add("MMEXHEADERID_IN", OracleDbType.Int64).Value = MMExheaderId;
                oCmd.Parameters.Add("MMDETAILID_IN", OracleDbType.Int64).Value = MMDetailId;
                oCmd.Parameters.Add("MATERIALCODE_IN", OracleDbType.Int64).Value = materialCode;
                oCmd.Parameters.Add("PLANTCODE_IN", OracleDbType.Int64).Value = plantCode;
                oCmd.Parameters.Add("EMAILID_IN", OracleDbType.Varchar2, 100).Value = emailId;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("ERRORMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRORMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }
        // Added by Ajit TTL Extend Material Code with Sales View
        public string UpdateMarerialCodeExtendRequestSales(int MMExheaderId, int MMDetailId, Int64 materialCode, int plantCode, string emailId, string trans_grp, string loading_grp, string baseunitmeasure, string salesorg, string distri_chann, string item_categ_grp, string availability, string profitcentre, string storage_loc, string tax_class, string gen_item_cat_grp, string mat_grp_pac_matls, string pack_mat_typ, string EXTENDREQTYPE)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMEXTENDREQUESTSALSES_UPDATE";
                oCmd.Parameters.Add("MMEXHEADERID_IN", OracleDbType.Int64).Value = MMExheaderId;
                oCmd.Parameters.Add("MMDETAILID_IN", OracleDbType.Int64).Value = MMDetailId;
                oCmd.Parameters.Add("MATERIALCODE_IN", OracleDbType.Int64).Value = materialCode;
                oCmd.Parameters.Add("PLANTCODE_IN", OracleDbType.Int64).Value = plantCode;
                oCmd.Parameters.Add("EMAILID_IN", OracleDbType.Varchar2, 100).Value = emailId;

                oCmd.Parameters.Add("TRANSPORTATIONGROUP_IN", OracleDbType.Varchar2).Value = trans_grp;
                oCmd.Parameters.Add("LOADINGGROUP_IN", OracleDbType.Varchar2).Value = loading_grp;
                oCmd.Parameters.Add("BASEUNITOFMEASURE_IN", OracleDbType.Varchar2).Value = baseunitmeasure;
                oCmd.Parameters.Add("PROFITCENTER_IN", OracleDbType.Varchar2).Value = profitcentre;
                oCmd.Parameters.Add("SALES_ORG_IN", OracleDbType.Varchar2).Value = salesorg;
                oCmd.Parameters.Add("DISTRI_CHN_IN", OracleDbType.Varchar2).Value = distri_chann;
                oCmd.Parameters.Add("ITEM_CATG_GRP_IN", OracleDbType.Varchar2).Value = item_categ_grp;
                oCmd.Parameters.Add("AVAIL_CHK_IN", OracleDbType.Varchar2).Value = availability;
                oCmd.Parameters.Add("STORAGE_LOC_IN", OracleDbType.Varchar2).Value = storage_loc;
                oCmd.Parameters.Add("TAX_CLASSIFICATION_IN", OracleDbType.Varchar2).Value = tax_class;
                oCmd.Parameters.Add("GEN_ITEM_CAT_GRP_IN", OracleDbType.Varchar2).Value = gen_item_cat_grp;
                oCmd.Parameters.Add("MAT_GRP_PACK_MATLS_IN", OracleDbType.Varchar2).Value = mat_grp_pac_matls;
                oCmd.Parameters.Add("PACKAGING_MAT_TYPE_IN", OracleDbType.Varchar2).Value = pack_mat_typ;
                oCmd.Parameters.Add("EXTENDREQTYPE_IN", OracleDbType.Int64).Value = Convert.ToInt64(EXTENDREQTYPE);
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("ERRORMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRORMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }
        public string DeleteMarerialCodeExtendRequest(int MMDetailId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMEXTENDREQUEST_DELETE";
                oCmd.Parameters.Add("MMDETAILID_IN", OracleDbType.Int64).Value = MMDetailId;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("ERRORMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRORMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }

        public string DeleteMarerialCodeCreationRequest(int MMDetailId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMCREATIONREQUEST_DELETE";
                oCmd.Parameters.Add("MMDETAILID_IN", OracleDbType.Int64).Value = MMDetailId;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("ERRORMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRORMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }

        public DataSet GetMMApprovalRequestList(int userId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.MMREQUESTFORAPPROVAL_GET";
                oCmd.Parameters.Add("CUR_MMREQUESTAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = userId;
                ds = oDataMgmt.GetDataSet(oCmd);
                return ds;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// GET ACTIVE  MASTER Status Data
        /// </summary>
        /// <returns></returns>
        public DataTable GetMMApprovalCreateRequestList(string MMDetailId, string requestType, int userId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                dt = new DataTable();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.MMREQUESTFORAPPMATERIAL_GET"; // This procedure is also called in export to excel procedure.
                oCmd.Parameters.Add("CUR_MMREQUESTAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("MMDETAILID_IN", OracleDbType.Varchar2).Value = MMDetailId;
                oCmd.Parameters.Add("VREQUESTORTYPE_IN", OracleDbType.Varchar2).Value = requestType;
                oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = userId;
                //GET DATA FROM DATA ACCESS LAYER
                // ds = oDataMgmt.GetDataSet(oCmd);
                dt = oDataMgmt.GetDataTable(oCmd);
                return dt;
            }
            catch
            {
                throw;
            }
        }


        public DataSet GetMMApprovalCreationRequestList(int userId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.MMREQUESTFORAPPROVALCREATE_GET";
                oCmd.Parameters.Add("CUR_MMREQUESTAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = userId;
                ds = oDataMgmt.GetDataSet(oCmd);
                return ds;
            }
            catch
            {
                throw;
            }
        }
        //Added by Ajit TTL
        public DataSet GetMMApprovalUpdationRequestList(int userId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.MMREQUESTFORAPPROVALUPDATE_GET";
                oCmd.Parameters.Add("CUR_MMREQUESTAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = userId;
                ds = oDataMgmt.GetDataSet(oCmd);
                return ds;
            }
            catch
            {
                throw;
            }
        }

        public DataSet GetMMApprovalExtendRequestList(int userId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.MMREQUESTFORAPPROVALEXTEND_GET";
                oCmd.Parameters.Add("CUR_MMREQUESTAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = userId;
                ds = oDataMgmt.GetDataSet(oCmd);
                return ds;
            }
            catch
            {
                throw;
            }
        }

        public string ApproveMMRequest(string MMDetailIdList, string remarks, int empCode, string status)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMREQUEST_APPROVE";
                oCmd.Parameters.Add("MMDETAILIDLIST_IN", OracleDbType.Varchar2).Value = MMDetailIdList;
                oCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = remarks;
                oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = empCode;
                oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("ERRORMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRORMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }

        public DataSet GetMMRequestDetail(int MMDetailId, int userId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMREQUESTDETAIL_GET";
                oCmd.Parameters.Add("CUR_MMREQUESTDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("MMDETAILID_IN", OracleDbType.Int64).Value = MMDetailId;
                oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = userId;
                ds = oDataMgmt.GetDataSet(oCmd);
                return ds;
            }
            catch
            {
                throw;
            }
        }

        public DataSet GetMMApproveRequestDetail(string MMDetailId, string requestType, int userId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMREQUESTDETAPPROVE_GET";
                oCmd.Parameters.Add("CUR_MMREQUESTDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("MMDETAILID_IN", OracleDbType.Varchar2).Value = MMDetailId;
                oCmd.Parameters.Add("MMREQUESTTYPE", OracleDbType.Varchar2).Value = requestType;
                oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = userId;
                ds = oDataMgmt.GetDataSet(oCmd);
                return ds;
            }
            catch
            {
                throw;
            }
        }
        public DataSet GetMMApprovalDetail(int MMDetailId, int userId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMAPPROVALDETAIL_GET";
                oCmd.Parameters.Add("CUR_MMAPPROVALDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("MMDETAILID_IN", OracleDbType.Int64).Value = MMDetailId;
                oCmd.Parameters.Add("USERID_IN", OracleDbType.Int64).Value = userId;
                ds = oDataMgmt.GetDataSet(oCmd);
                return ds;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// GET ACTIVE  MASTER Status Data
        /// </summary>
        /// <returns></returns>
        public DataTable SearchMaterialApprovalDetails(string MMDetailId, string requestType, int userId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                dt = new DataTable();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.MM_SEARCHMATAPPROVALDETAIL_GET"; // This procedure is also called in export to excel procedure.
                oCmd.Parameters.Add("CUR_MMDETAILSEARCH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("VMMID_IN", OracleDbType.Varchar2).Value = MMDetailId;
                oCmd.Parameters.Add("VREQUESTOR_TYPE", OracleDbType.Varchar2).Value = requestType;
                oCmd.Parameters.Add("V_USERID", OracleDbType.Int64).Value = userId;
                //GET DATA FROM DATA ACCESS LAYER
                // ds = oDataMgmt.GetDataSet(oCmd);
                dt = oDataMgmt.GetDataTable(oCmd);
                return dt;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// To Add Valuation mapping data 
        /// </summary>
        /// <returns></returns>
        public string MaterialValuationMapping_Add(int materialType_Id, int valuationClass_Id, int user_Id)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ods = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.MM_TYPEVALUATIONMAP_ADD";
                oCmd.Parameters.Add("VMATERIALTYPEID", OracleDbType.Int64).Value = materialType_Id;
                oCmd.Parameters.Add("VVALUATIONID", OracleDbType.Int64).Value = valuationClass_Id;
                oCmd.Parameters.Add("VUSERID_IN", OracleDbType.Int64).Value = user_Id;
                oCmd.Parameters.Add("ERRORMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRORMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// To Add Valuation mapping data 
        /// </summary>
        /// <returns></returns>
        public string MaterialGroupTypeValuationMapping_Add(int materialType_Id, int valuationClass_Id, int materialGroup_Id, int user_Id)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ods = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.MM_TYPEGROUPVALUATIONMAP_ADD";
                oCmd.Parameters.Add("VMATERIALTYPEID", OracleDbType.Int64).Value = materialType_Id;
                oCmd.Parameters.Add("VVALUATIONID", OracleDbType.Int64).Value = valuationClass_Id;
                oCmd.Parameters.Add("VMATERIALGROUPID", OracleDbType.Int64).Value = materialGroup_Id;
                oCmd.Parameters.Add("VUSERID_IN", OracleDbType.Int64).Value = user_Id;
                oCmd.Parameters.Add("ERRORMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRORMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// To Deactivate Valuation mapping data
        /// </summary>
        /// <returns></returns>
        public string DeleteMaterialTypevaluationmapping(int MTVDetailId, int _userId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.MM_MTYPEVALUATIONMAP_DELETE";
                oCmd.Parameters.Add("VMATERIALVALTYPEID", OracleDbType.Int64).Value = MTVDetailId;
                oCmd.Parameters.Add("VUSERID_IN", OracleDbType.Int64).Value = _userId;
                oCmd.Parameters.Add("ERRORMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRORMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// To Add Approval Authority Details data 
        /// </summary>
        /// <returns></returns>
        public string MaterialApprovalAuthority_Add(string _authType_Id, int _authEmp_Id, string _plantId, int _masterTypeId, int user_Id, int _operationID, int _divisionID, int ActiveAuthority)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ods = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.MM_APPROVALMATRIX_ADD";
                oCmd.Parameters.Add("VMASTERTYPEID", OracleDbType.Int64).Value = _masterTypeId;
                oCmd.Parameters.Add("VPLANTID", OracleDbType.Varchar2).Value = _plantId;
                oCmd.Parameters.Add("VEMPCODE", OracleDbType.Int64).Value = _authEmp_Id;
                oCmd.Parameters.Add("VAUTHORIZATIONTYPE", OracleDbType.Varchar2).Value = _authType_Id;
                oCmd.Parameters.Add("VUSERID_IN", OracleDbType.Int64).Value = user_Id;
                oCmd.Parameters.Add("VOPERATIONID_IN", OracleDbType.Int64).Value = _operationID;
                oCmd.Parameters.Add("VDIVISIONID_IN", OracleDbType.Int64).Value = _divisionID;
                oCmd.Parameters.Add("VMMAUTHORITYID_IN", OracleDbType.Int64).Value = ActiveAuthority;
                oCmd.Parameters.Add("ERRORMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRORMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// To Deactivate Approval Matrix data
        /// </summary>
        /// <returns></returns>
        public string DeleteApprovalMatrixData(int MApproveDetailId, int _userId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.MM_APPROVALMATRIX_DELETE";
                oCmd.Parameters.Add("VMASTERDATAAPPROVALID", OracleDbType.Int64).Value = MApproveDetailId;
                oCmd.Parameters.Add("VUSERID_IN", OracleDbType.Int64).Value = _userId;
                oCmd.Parameters.Add("ERRORMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRORMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// To Added Material Master data
        /// </summary>
        /// <returns></returns>
        public string AddMaterialMasterData(int masterType, string materialType, string materialDescription, string materialCode, string _SiteName, string _SiteDesc, int userId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.MM_MATERIALTYPEDESC_ADD";
                oCmd.Parameters.Add("VMASTERTYPE_IN", OracleDbType.Int64).Value = masterType;
                oCmd.Parameters.Add("VMATERIALTYPE_IN", OracleDbType.Varchar2).Value = materialType;
                oCmd.Parameters.Add("VMATERIALDESC_IN", OracleDbType.Varchar2).Value = materialDescription;
                oCmd.Parameters.Add("VMATERIALCODE_IN", OracleDbType.Varchar2).Value = materialCode;
                oCmd.Parameters.Add("VPLANTSITENAME_IN", OracleDbType.Varchar2).Value = _SiteName;
                oCmd.Parameters.Add("VPLANTSITEDESC_IN", OracleDbType.Varchar2).Value = _SiteDesc;
                oCmd.Parameters.Add("VUSERID_IN", OracleDbType.Int64).Value = userId;
                oCmd.Parameters.Add("ERRORMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRORMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }


        /// <summary>
        /// To Edit Material Master data
        /// </summary>
        /// <returns></returns>
        public string EditMaterialMasterData(int masterType, int materialId, string materialType, string materialDescription, string materialCode, string _SiteName, string _SiteDesc, int userId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.MM_MATERIALMASTER_UPDATE";
                oCmd.Parameters.Add("VMATERIALMASTERTYPE_IN", OracleDbType.Int64).Value = masterType;
                oCmd.Parameters.Add("VMATERIALMASTERID_IN", OracleDbType.Varchar2).Value = materialId;
                oCmd.Parameters.Add("VMATERIALTYPE_IN", OracleDbType.Varchar2).Value = materialType;
                oCmd.Parameters.Add("VMATERIALDESC_IN", OracleDbType.Varchar2).Value = materialDescription;
                oCmd.Parameters.Add("VMATERIALCODE_IN", OracleDbType.Varchar2).Value = materialCode;
                oCmd.Parameters.Add("VPLANTSITENAME_IN", OracleDbType.Varchar2).Value = _SiteName;
                oCmd.Parameters.Add("VPLANTSITEDESC_IN", OracleDbType.Varchar2).Value = _SiteDesc;
                oCmd.Parameters.Add("VUSERID_IN", OracleDbType.Int64).Value = userId;
                oCmd.Parameters.Add("ERRORMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRORMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// To Deactivate Material Master data
        /// </summary>
        /// <returns></returns>
        public string DeleteMaterialMasterData(int masterType, int MasterDetailId, int _userId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.MM_MATERIALMASTER_DELETE";
                oCmd.Parameters.Add("VMATERIALMASTERTYPE_IN", OracleDbType.Int64).Value = masterType;
                oCmd.Parameters.Add("VMATERIALMASTERID_IN", OracleDbType.Int64).Value = MasterDetailId;
                oCmd.Parameters.Add("VUSERID_IN", OracleDbType.Int64).Value = _userId;
                oCmd.Parameters.Add("ERRORMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRORMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }
        #region "Added by Sushanta on 25 Oct 2016 for excel upload"
        //public static DataTable FillDataTableFromExcelFile(string strFileName, string strFileExtension, string strFilepath, int TotalCoumns)
        //{
        //    string connectionString = "";
        //    DataTable dtExcel = new DataTable();
        //    if (strFileExtension == ".xls")
        //    {
        //        connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strFilepath + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
        //    }
        //    else if (strFileExtension == ".xlsx")
        //    {
        //        connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + strFilepath + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
        //    }
        //    OleDbConnection con = new OleDbConnection(connectionString);
        //    OleDbCommand cmd = new OleDbCommand();
        //    cmd.CommandType = System.Data.CommandType.Text;
        //    cmd.Connection = con;
        //    OleDbDataAdapter dAdapter = new OleDbDataAdapter(cmd);
        //    DataTable dtExcelRecords = new DataTable();
        //    con.Open();
        //    DataTable dtExcelSheetName = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
        //    string getExcelSheetName = dtExcelSheetName.Rows[0]["Table_Name"].ToString();
        //    cmd.CommandText = "SELECT * FROM [" + getExcelSheetName + "]";
        //    dAdapter.SelectCommand = cmd;
        //    dAdapter.Fill(dtExcelRecords);


        //    int intExcelColoums = dtExcelRecords.Columns.Count;
        //    con.Close();
        //    //if (intExcelColoums != TotalCoumns)
        //    //{
        //    //   throw new Exception("Column Int64 does not match with the uploaded Excel file.");
        //    //   dtExcelRecords.Reset();
        //    //}
        //    return dtExcelRecords;

        //}


        //public DataTable FillDatasetExcel(string filepath)
        //{
        //    string path = filepath;
        //    // Prepare cnnection string  
        //    string connectionstring = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 12.0 Xml;HDR=YES;FMT=Delimited';";
        //    byte[] Bytes = fileuploader.FileBytes;
        //    File.WriteAllBytes(path, Bytes);
        //    OleDbConnection connection = new OleDbConnection();
        //    connection.ConnectionString = connectionstring;
        //    OleDbCommand command = new OleDbCommand();
        //    command.CommandType = CommandType.Text;
        //    command.Connection = connection;
        //    command.CommandText = "Select * FROM [Sheet1$]";
        //    connection.Open();
        //    // create data table object  
        //    DataTable dt = new DataTable();
        //    // Execute Reader and load the data into datatable  
        //    dt.Load(command.ExecuteReader());
        //    // Close the connection  
        //    connection.Close();
        //    return dt;
        //}


        public  DataTable CSVToDataTableFromFile(string StrFileName, string StrColumnSeperator, string StrRowSeperator, int InTotal_Column)
        {

            StreamReader sr = new StreamReader(StrFileName);
            DataTable dttmp = new DataTable();

            try
            {
                string StrCSvText = sr.ReadToEnd();
                //string[] strvalue = Regex.Split(StrCSvText, "\r\n");
                string[] strRows = Regex.Split(StrCSvText, StrRowSeperator);

                for (int colNo = 1; colNo <= InTotal_Column; colNo++)
                {
                    dttmp.Columns.Add("Coloumn_" + colNo.ToString());
                }

                char[] chSep = { Convert.ToChar(StrColumnSeperator) };
                for (int i = 0; i < strRows.Length; i++)
                {
                    string[] strCols = strRows[i].Split(chSep, StringSplitOptions.None);

                    if (strCols[i].ToString() != "")
                    {
                        if (strCols.Length != InTotal_Column)
                        {
                            throw new Exception("Column number does not match for the row number # " + (i + 1).ToString() + " in the uploaded csv file.");
                        }
                        else
                        {
                            dttmp.Rows.Add(strCols);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dttmp;

        }
        #endregion

        /// <summary>
        /// GET ACTIVE  Material MASTER Material Code Status Data
        /// </summary>
        /// <returns></returns>
        public DataTable SearchMaterialCode_VerifyCreation(string _RequestNo)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                dt = new DataTable();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MATERIALCODE_VERIFY"; // This procedure is also called in export to excel procedure.
                oCmd.Parameters.Add("CUR_MATERIAL_CODELIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("MMREQUESTNO", OracleDbType.Varchar2).Value = _RequestNo;
                //GET DATA FROM DATA ACCESS LAYER
                // ds = oDataMgmt.GetDataSet(oCmd);
                dt = oDataMgmt.GetDataTable(oCmd);
                return dt;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// GET ACTIVE  Material MASTER Material Code Status Data
        /// </summary>
        /// <returns></returns>
        public DataTable SearchMaterialCode_VerifyCreationBlank(string _RequestNo, string materialCode)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                dt = new DataTable();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MATCODEBLANK_VERIFY"; // This procedure is also called in export to excel procedure.
                oCmd.Parameters.Add("CUR_MATERIAL_CODELIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("MMREQUESTNO", OracleDbType.Varchar2).Value = _RequestNo;
                oCmd.Parameters.Add("MATERIALCODE_IN", OracleDbType.Varchar2).Value = materialCode;
                //GET DATA FROM DATA ACCESS LAYER
                // ds = oDataMgmt.GetDataSet(oCmd);
                dt = oDataMgmt.GetDataTable(oCmd);
                return dt;
            }
            catch
            {
                throw;
            }
        }
        public string UpdateMarerialCodeCreateRequestNo(string RequestNo, string materialCode, int UserId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMMATERIALCODE_UPDATECRE";
                oCmd.Parameters.Add("MMREQUEST_IN", OracleDbType.Varchar2).Value = RequestNo;
                oCmd.Parameters.Add("MATERIALCODE_IN", OracleDbType.Varchar2).Value = materialCode;
                oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = UserId;
                oCmd.Parameters.Add("ERRMSG", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                return "1";
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// GET ACTIVE  Material MASTER Material Code Status Data
        /// </summary>
        /// <returns></returns>
        public DataTable SearchMaterialCode_VerifyExtend(string _RequestNo)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                dt = new DataTable();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MATERIALCODE_VEREXT"; // This procedure is also called in export to excel procedure.
                oCmd.Parameters.Add("CUR_MATERIAL_CODEEXTLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("MMREQUESTNO", OracleDbType.Varchar2).Value = _RequestNo;
                //GET DATA FROM DATA ACCESS LAYER
                // ds = oDataMgmt.GetDataSet(oCmd);
                dt = oDataMgmt.GetDataTable(oCmd);
                return dt;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// GET ACTIVE  Material MASTER Material Code Status Data
        /// </summary>
        /// <returns></returns>
        public DataTable SearchMaterialCode_VerifyExtendBlank(string _RequestNo, string materialCode)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                dt = new DataTable();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MATCODEBLNK_VEREXT"; // This procedure is also called in export to excel procedure.
                oCmd.Parameters.Add("CUR_MATERIAL_CODEEXTLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("MMREQUESTNO", OracleDbType.Varchar2).Value = _RequestNo;
                oCmd.Parameters.Add("MATERIALCODE_IN", OracleDbType.Varchar2).Value = materialCode;
                //GET DATA FROM DATA ACCESS LAYER
                // ds = oDataMgmt.GetDataSet(oCmd);
                dt = oDataMgmt.GetDataTable(oCmd);
                return dt;
            }
            catch
            {
                throw;
            }
        }

        public string UpdateMarerialCodeExtendRequestNo(string RequestNo, string materialCode, int UserId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMMATERIALCODE_UPDATEEXT";
                oCmd.Parameters.Add("MMREQUEST_IN", OracleDbType.Varchar2).Value = RequestNo;
                oCmd.Parameters.Add("MATERIALCODE_IN", OracleDbType.Varchar2).Value = materialCode;
                oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = UserId;
                oCmd.Parameters.Add("ERRMSG", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                return "1";
            }
            catch
            {
                throw;
            }
        }


        #endregion

        public string showMMSISAuthPage(string empcode)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMSISAUTHORITY";
                oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = empcode;
                oCmd.Parameters.Add("SISAUTH_OUT", OracleDbType.Int64, 4).Direction = ParameterDirection.Output;

                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["SISAUTH_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["SISAUTH_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }

        public string showMMPPCFINAuthPage(string empcode)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMPPCFINAUTHORITY";
                oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = empcode;
                oCmd.Parameters.Add("SISAUTH_OUT", OracleDbType.Int64, 4).Direction = ParameterDirection.Output;

                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["SISAUTH_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["SISAUTH_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// To Add SIS User Authority Details data 
        /// </summary>
        /// <returns></returns>
        public string MaterialSISUserAuthority_Add(int _sisauthEmp_Id, string _sisEmp_Name, int user_Id)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ods = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.MM_MMSISUSERMATRIX_ADD";
                oCmd.Parameters.Add("VEMPCODE", OracleDbType.Int64).Value = _sisauthEmp_Id;
                oCmd.Parameters.Add("VEMPNAME", OracleDbType.Varchar2, 500).Value = _sisEmp_Name;
                oCmd.Parameters.Add("VUSERID_IN", OracleDbType.Int64).Value = user_Id;
                oCmd.Parameters.Add("ERRORMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRORMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// To Deactivate SIS USer List data
        /// </summary>
        /// <returns></returns>
        public string DeleteSISUserMatrixData(int MMSISUserDetailId, int _userId)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.MM_SISUSERMATRIX_DELETE";
                oCmd.Parameters.Add("VSISAPPROVALID", OracleDbType.Int64).Value = MMSISUserDetailId;
                oCmd.Parameters.Add("VUSERID_IN", OracleDbType.Int64).Value = _userId;
                oCmd.Parameters.Add("ERRORMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRORMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }


        /// <summary>
        /// To Deactivate SIS USer List data
        /// </summary>
        /// <returns></returns>
        public string Material_PendingCountData(int _userId)
        {
            object objCount;
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MATERIALPENDINGCOUNT";
                oCmd.Parameters.Add("VUSERID_IN", OracleDbType.Int64).Value = _userId;
                oCmd.Parameters.Add("P_RESULT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                objCount = oCmd.Parameters["P_RESULT"].Value;

            }
            catch
            {
                throw;
            }
            return objCount.ToString();
        }



        public string UpdateMarerialDetailPPCApproveRequest(int MMDetailId, int materialType, int materialGroup, int valuationClass)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMPPCAPPROVEREQ_UPDATE";
                //oCmd.Parameters.Add("MMHEADERID_IN", OracleDbType.Int64).Value = MMHeaderId;
                oCmd.Parameters.Add("MMDETAILID_IN", OracleDbType.Int64).Value = MMDetailId;
                oCmd.Parameters.Add("MATERIALTYPE_IN", OracleDbType.Int64).Value = materialType;
                oCmd.Parameters.Add("MATERIALGROUP_IN", OracleDbType.Int64).Value = materialGroup;
                oCmd.Parameters.Add("VALUATIONCLASS_IN", OracleDbType.Int64).Value = valuationClass;
                oCmd.Parameters.Add("ERRMSG", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                return "1";
            }
            catch
            {
                throw;
            }
        }

        public string InsertMDPPCApproveRequestChangeHistory(int MMDetailId, int materialType, int materialGroup, int valuationClass)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMPPCCHANGE_HISTORY";
                //oCmd.Parameters.Add("MMHEADERID_IN", OracleDbType.Int64).Value = MMHeaderId;
                oCmd.Parameters.Add("MMDETAILID_IN", OracleDbType.Int64).Value = MMDetailId;
                oCmd.Parameters.Add("MATERIALTYPE_IN", OracleDbType.Int64).Value = materialType;
                oCmd.Parameters.Add("MATERIALGROUP_IN", OracleDbType.Int64).Value = materialGroup;
                oCmd.Parameters.Add("VALUATIONCLASS_IN", OracleDbType.Int64).Value = valuationClass;
                oCmd.Parameters.Add("ERRMSG", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                return "1";
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// GET ACTIVE  MASTER Status Data
        /// </summary>
        /// <returns></returns>
        public DataTable SearchMaterialDetailsPPCChangeHistory(string MMDetailId, string requestType, int userId, string entry_Date)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                dt = new DataTable();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.MM_PPCCHANGEHISTORY_GET"; // This procedure is also called in export to excel procedure.
                oCmd.Parameters.Add("CUR_MMDETAILSEARCH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("VMMID_IN", OracleDbType.Varchar2).Value = MMDetailId;
                oCmd.Parameters.Add("VREQUESTOR_TYPE", OracleDbType.Varchar2).Value = requestType;
                oCmd.Parameters.Add("V_USERID", OracleDbType.Int64).Value = userId;
                oCmd.Parameters.Add("V_ENTRYDATE", OracleDbType.Varchar2).Value = entry_Date;
                //GET DATA FROM DATA ACCESS LAYER
                // ds = oDataMgmt.GetDataSet(oCmd);
                dt = oDataMgmt.GetDataTable(oCmd);
                return dt;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get the list of Operation wise all approval authority SM
        /// </summary>
        /// <param name="MaterialTypeId"></param>
        /// <returns></returns>
        public DataSet GetOperationApprovalAuthorityList(int _userId)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.MM_SECTIONREPORTINGSEARCH";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = _userId;
            oCmd.Parameters.Add("CUR_SECTIONREPORTINGSEARCH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //Get data from data access layer
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        /// <summary>
        /// Get the list of Operation wise approval authority SM default userwise
        /// </summary>
        /// <param name="MaterialTypeId"></param>
        /// <returns></returns>
        public DataSet GetOperationApprovalAuthorityDefaultList(int _userId)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.MM_SECTMGNAMEGET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = _userId;
            oCmd.Parameters.Add("CUR_SECTMGNAMEGET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //Get data from data access layer
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        /// <summary>
        /// Get list of material creation Details count
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataSet GetMaterialCountDetails(int MMHeaderId)
        {
            OracleCommand oCmd = new OracleCommand();
            DataSet ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMDRAFTHEADERCOUNT_GET";
            oCmd.Parameters.Add("CUR_MMCREATIONDRAFTDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("MMHEADERID_IN", OracleDbType.Int64).Value = MMHeaderId;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        public DataSet Material_PendingCountDataAll(string EMPCODE_IN)
        {
            OracleCommand cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "PKG_MATERIALMASTER.SPROC_MATERIALPENDINGCOUNTALL";
            cmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = EMPCODE_IN;
            cmd.Parameters.Add("CUR_MMREQUESTCOUNT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("CUR_MMREQUESTCOUNTCREATE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("CUR_MMREQUESTCOUNTEXTEND", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("CUR_MMREQUESTCOUNTAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataSet(cmd);
            cmd.Dispose();
        }

        /// <summary>
        /// GET EDIT Approval Matrix MASTER  Type LIST
        /// </summary>
        /// <returns></returns>
        public DataSet GetApprovalMatrixListEdit(int MMPPCFINDetailId)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.MM_APPROVALMATRIX_EDITGET";
            oCmd.Parameters.Add("CUR_MMAPPROVALDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("PPCFINAPROVALID", OracleDbType.Int64).Value = MMPPCFINDetailId;
            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }
        /// <summary>
        /// GET UPDATE Approval Matrix MASTER  Type LIST
        /// </summary>
        /// <returns></returns>
        public string UpdateApprovalAuthorityRequest(int MMApproverId, int ActiveAuthority, int EMPCODE_IN)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_APPROVEAUTHORITY_UPDATE";
                oCmd.Parameters.Add("MMAPPROVERID_IN", OracleDbType.Int64).Value = MMApproverId;
                oCmd.Parameters.Add("MMAUTHORITYDETAILID_IN", OracleDbType.Int64).Value = ActiveAuthority;
                oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = EMPCODE_IN;
                oCmd.Parameters.Add("ERRMSG", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                return "1";
            }
            catch
            {
                throw;
            }
        }

        public DataSet MM_HeaderDetailDeleteId_GET(int _mmDetailId)
        {
            OracleCommand oCmd = new OracleCommand();
            DataSet ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMREQUESTITEM_DELETE";
            oCmd.Parameters.Add("CUR_MMHEADERITEMDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_MMHEADERDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("MMDETAILID_IN", OracleDbType.Int64).Value = _mmDetailId;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }
        public DataSet GetMMIndicatorList(string indicatorid, string indicator)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MMINDICATER_GET";
            oCmd.Parameters.Add("MMINDICATERID_IN", OracleDbType.Varchar2).Value = indicatorid;
            oCmd.Parameters.Add("MMINDICATER_IN", OracleDbType.Varchar2).Value = indicator;
            oCmd.Parameters.Add("CUR_INDICATERLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);

        }

        //Change done by Aumento on 18032023===============================================
        public DataSet GetMaterialUpdationDetails(int MaterialCode, int Plant)
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_GetMaterialUpdaDetails";
            oCmd.Parameters.Add("MATERIALCODE_IN", OracleDbType.Int64).Value = MaterialCode;
            oCmd.Parameters.Add("PLANT_IN", OracleDbType.Int64).Value = Plant;
            oCmd.Parameters.Add("CUR_INDICATERLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);

        }

        /// <summary>
        /// To Edit Material Master data
        /// </summary>
        /// <returns></returns>
        /// Added by Ajit TTL for Update Material Details
        public string UpdateMaterialUpdationDetails(int MaterialCode, int Plant, int HSNCode, int Indicator, string PlantSpMatStatus, string Description)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                DataSet ds = new DataSet();
                //oDataMgmt = new DataManagement();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MaterialDetails_UPDATE";
                oCmd.Parameters.Add("MATERIALCODE_IN", OracleDbType.Int64).Value = MaterialCode;
                oCmd.Parameters.Add("PLANT_IN", OracleDbType.Int64).Value = Plant;
                oCmd.Parameters.Add("HSNCODE_IN", OracleDbType.Int64).Value = HSNCode;
                oCmd.Parameters.Add("INDICATOR_IN", OracleDbType.Int64).Value = Indicator;
                oCmd.Parameters.Add("PLANTSPMTST_IN", OracleDbType.Varchar2).Value = PlantSpMatStatus;
                oCmd.Parameters.Add("DESC_IN", OracleDbType.Varchar2).Value = Description;
                oCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
                    return "1";
                else
                    return oCmd.Parameters["ERRORMSG_OUT"].Value.ToString();
            }
            catch
            {
                throw;
            }
        }


        //public DataSet UpdateMaterialUpdationDetails(int MaterialCode, int Plant,int HSNCode, int Indicator,string PlantSpMatStatus,string Description)
        //{
        //    OracleCommand oCmd = new OracleCommand();
        //    ds = new DataSet();
        //    oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        //    oCmd.CommandText = "PKG_MATERIALMASTER.SPROC_MaterialDetails_UPDATE";
        //    oCmd.Parameters.Add("MATERIALCODE_IN", OracleDbType.Int64).Value = MaterialCode;
        //    oCmd.Parameters.Add("PLANT_IN", OracleDbType.Int64).Value = Plant;
        //    oCmd.Parameters.Add("HSNCODE_IN", OracleDbType.Int64).Value = HSNCode;
        //    oCmd.Parameters.Add("INDICATOR_IN", OracleDbType.Int64).Value = Indicator;
        //    oCmd.Parameters.Add("PLANTSPMTST_IN", OracleDbType.Varchar2).Value = PlantSpMatStatus;
        //    oCmd.Parameters.Add("DESC_IN", OracleDbType.Varchar2).Value = Description;
        //    oCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;        
        //    oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int64).Direction = ParameterDirection.Output;
        //    oDataMgmt.ExecuteQuery(oCmd);
        //    if (oCmd.Parameters["RESULT_OUT"].Value.ToString() == "1")
        //        return "1";
        //    else
        //        return oCmd.Parameters["ERRORMSG_OUT"].Value.ToString();
        //    //GET DATA FROM DATA ACCESS LAYER
        //    ds = oDataMgmt.GetDataSet(oCmd);
        //    return (ds);

        //}

        //==============================================================================================

    }
}
