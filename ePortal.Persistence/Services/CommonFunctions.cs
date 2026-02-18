using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Persistence.Interface;
using ePortal.ViewModels;
using Oracle.ManagedDataAccess.Client;

namespace ePortal.Persistence.Services
{
    public class CommonFunctions : ICommonFunctions
    {
        //DataManagement oDataMgmt = new DataManagement();
        DataTable objDt;
        OracleCommand cmd;

        private readonly IConnectionString _conn;
        private readonly IDataManagement oDataMgmt;


        public CommonFunctions(IConnectionString conn, IDataManagement _oDataMgmt)
        {
            oDataMgmt = _oDataMgmt;
            _conn = conn;
        }

        /// <summary>
        /// This function is used to get designation.
        /// </summary>
        public DataTable GetDesignation(string strValue)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_COMMONMETHOD.SPROC_DESIGNATION_GET";
            cmd.Parameters.Add("VALUE_IN", OracleDbType.Varchar2).Value = strValue;
            cmd.Parameters.Add("CUR_DESIGNATION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            //oDataMgmt = new DataManagement();
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }

        public DataTable GetDesignationFORKIOSK(string strValue)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_COMMONMETHOD.SPROC_DESIGNATION_GETFORKIOSK";
            cmd.Parameters.Add("VALUE_IN", OracleDbType.Varchar2).Value = strValue;
            cmd.Parameters.Add("CUR_DESIGNATION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            //oDataMgmt = new DataManagement();
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }

        public DataTable GetAllDesignation(string strValue)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_COMMONMETHOD.SPROC_DESIGNATION_GETALL";
            cmd.Parameters.Add("VALUE_IN", OracleDbType.Varchar2).Value = strValue;
            cmd.Parameters.Add("CUR_DESIGNATION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            //oDataMgmt = new DataManagement();
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }

        /// <summary>
        /// GET EMPLOYEE OFFICIAL DETAIL LIKE DESIGNATION,SECTION,DEPARTMENT,DIVISION,OPERATION
        /// WRITTEN BY SANTOSH ON 3 NOV 2010
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="KiID"></param>
        /// <returns></returns>
        public DataTable GetEmployeeOfficialDetails(string userID, string KiID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_COMMONMETHOD.SPROC_EMPOFFICIALDETAIL_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = userID;
            oCmd.Parameters.Add("SYKIID_IN", OracleDbType.Varchar2).Value = KiID;
            oCmd.Parameters.Add("CUR_EMPOFFDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET EMPLOYEE IE. CASUAL,COMPANY CASUAL,APPRENTICE,CONTRATUAL DETAIL LIKE DESIGNATION,SECTION,DEPARTMENT,DIVISION,OPERATION
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="KiID"></param>
        /// <returns></returns>
        public DataTable GetTempEmployeeDetails(string userID, string KiID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_COMMONMETHOD.SPROC_TEMPEMPDETAIL_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = userID;
            oCmd.Parameters.Add("SYKIID_IN", OracleDbType.Varchar2).Value = KiID;
            oCmd.Parameters.Add("CUR_EMPOFFDETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataTable GetFuncDesig()
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_COMMONMETHOD.SPROC_FUNDESIG_GET";
            oCmd.Parameters.Add("CUR_FUNDESIG", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public DataTable GetAssociate_Details(string ADEMPCODE_IN, string FIRSTNAME_IN, string LASTNAME_IN, string BLOODGROUP_IN, string EMAILID_IN, string DESIGNATION_IN, string OPERATION_IN, string DIVISION_IN, string DEPARTMENT_IN, string SECTION_IN)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_COMMONMETHOD.SPROC_ASSOCIATE_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = ADEMPCODE_IN;
            oCmd.Parameters.Add("FIRSTNAME_IN", OracleDbType.Varchar2).Value = FIRSTNAME_IN;
            oCmd.Parameters.Add("LASTNAME_IN", OracleDbType.Varchar2).Value = LASTNAME_IN;
            oCmd.Parameters.Add("BLOODGROUP_IN", OracleDbType.Varchar2).Value = BLOODGROUP_IN;
            oCmd.Parameters.Add("EMAILID_IN", OracleDbType.Varchar2).Value = EMAILID_IN;
            oCmd.Parameters.Add("DESIGNATION_IN", OracleDbType.Int32).Value = DESIGNATION_IN;
            oCmd.Parameters.Add("OPERATION_IN", OracleDbType.Int32).Value = OPERATION_IN;
            oCmd.Parameters.Add("DIVISION_IN", OracleDbType.Int32).Value = DIVISION_IN;
            oCmd.Parameters.Add("DEPARTMENT_IN", OracleDbType.Int32).Value = DEPARTMENT_IN;
            oCmd.Parameters.Add("SECTION_IN", OracleDbType.Int32).Value = SECTION_IN;
            oCmd.Parameters.Add("CUR_ASSOCIATE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public String GetParameterValue(string strParmaName)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_COMMONMETHOD.SPROC_SYPARAMVALUE_GET";
            cmd.Parameters.Add("PARAM_IN", OracleDbType.Varchar2).Value = strParmaName;
            cmd.Parameters.Add("PARAM_VAL", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;

            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            oDataMgmt.ExecuteQuery(cmd);

            return Convert.ToString(cmd.Parameters["PARAM_VAL"].Value);
        }

        public DataTable GetSITE()
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_COMMONMETHOD.SPROC_SITE_GET";
            cmd.Parameters.Add("CUR_SITE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            //oDataMgmt = new DataManagement();
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }
        public DataTable GetSITECAL()
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_COMMONMETHOD.SPROC_SITECAL_GET";
            cmd.Parameters.Add("CUR_SITE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            //oDataMgmt = new DataManagement();
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }
        public DataTable GetPlant()
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_COMMONMETHOD.SPROC_PLANT_GET";
            cmd.Parameters.Add("CUR_PLANT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }
        public DataTable GetShoeSizes()
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_COMMONMETHOD.SPROC_SHOESIZES_GET";
            cmd.Parameters.Add("CUR_SIZES", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }
        public DataTable GetTrouserSizes()
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_COMMONMETHOD.SPROC_TROUSERSIZES_GET";
            cmd.Parameters.Add("CUR_SIZES", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }

        public DataTable GetEmpDetails(string Ecode)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_USERDETAIL.SPROC_EMPDETAIL_GET";
            cmd.Parameters.Add("CUR_EMPDTL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = Ecode;

            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }

        public string GetEMailID(string AdempCode)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_COMMONMETHOD.SPROC_EMAILID_GET";
            cmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = AdempCode;
            cmd.Parameters.Add("EMAILID_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            oDataMgmt.ExecuteQuery(cmd);

            return Convert.ToString(cmd.Parameters["EMAILID_OUT"].Value);
        }

        public DataTable GetOperationDivDeptBySection(string strSectionID)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_COMMONMETHOD.SPROC_DEPDIVOPBYSECTIONID_GET";
            cmd.Parameters.Add("SECTIONID_IN", OracleDbType.Varchar2).Value = strSectionID;
            cmd.Parameters.Add("CUR_DETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }
        public DataTable GetLocation(String strsite)
        {

            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_COMMONMETHOD.SPROC_LOCATION_GET";
            cmd.Parameters.Add("SITE_IN", OracleDbType.Varchar2).Value = strsite;
            cmd.Parameters.Add("CUR_LOCATION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }
        public DataTable GetBank()
        {

            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_COMMONMETHOD.SPROC_BANK_GET";
            cmd.Parameters.Add("CUR_BANK", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="LevelType"></param>
        /// <param name="ParentLevelID"></param>
        /// <returns></returns>
        public DataTable GetLevels(Int32 LevelType, string ParentLevelID)
        {

            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_COMMONMETHOD.SPROC_LEVELS_GET";
            cmd.Parameters.Add("LEVEL_TYPE", OracleDbType.Int32).Value = LevelType;
            cmd.Parameters.Add("LEVEL_ID", OracleDbType.Varchar2).Value = ParentLevelID;
            cmd.Parameters.Add("CUR_OPERATION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }
        /// <summary>
        /// 
        /// </summary>
        public DataTable GetAssociateDetails(string KI, string OperationID, string DivisionID, string DepartmentID, string SectionID,
                                             string EmpCode, string FirstName, string LastName, string Designation, string EMailID,
                                             string BloodGroup, string FunDesignation)
        {

            string strAssociateLowestLevel = string.Empty;

            if (OperationID == "" && DivisionID == "" && DepartmentID == "" && SectionID == "")
            {
                //Get All Data irrespective of any any levels
                strAssociateLowestLevel = "";
            }
            else if (SectionID != "")
            {
                strAssociateLowestLevel = SectionID;
            }
            else if (DepartmentID != "")
            {
                strAssociateLowestLevel = DepartmentID;
            }
            else if (DivisionID != "")
            {
                strAssociateLowestLevel = DivisionID;
            }
            else if (OperationID != "")
            {
                strAssociateLowestLevel = OperationID;
            }


            DataTable dt = new DataTable();
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_COMMONMETHOD.SPROC_ASSOCIATESEARCH_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
            oCmd.Parameters.Add("FIRSTNAME_IN", OracleDbType.Varchar2).Value = FirstName;
            oCmd.Parameters.Add("LASTNAME_IN", OracleDbType.Varchar2).Value = LastName;
            oCmd.Parameters.Add("BLOODGROUP_IN", OracleDbType.Varchar2).Value = BloodGroup;
            oCmd.Parameters.Add("EMAILID_IN", OracleDbType.Varchar2).Value = EMailID;
            oCmd.Parameters.Add("DESIGNATION_IN", OracleDbType.Varchar2).Value = Designation;
            oCmd.Parameters.Add("FUNDESIGNATION_IN", OracleDbType.Varchar2).Value = FunDesignation;
            oCmd.Parameters.Add("LVL", OracleDbType.Varchar2).Value = strAssociateLowestLevel;
            oCmd.Parameters.Add("KIID", OracleDbType.Varchar2).Value = 14;
            oCmd.Parameters.Add("CUR_ASSOCIATE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return dt;
        }

        public DataTable GetApprovalMatrix(string EmpCode, String FunctionDesigForSelfApp, string ExcludeLvl)
        {

            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_COMMONMETHOD.SPROC_APPROVAL_LEVELS";
            cmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
            cmd.Parameters.Add("SELF_APPROVAL", OracleDbType.Varchar2).Value = FunctionDesigForSelfApp;
            cmd.Parameters.Add("EXCLUDE_LVLTYPE", OracleDbType.Varchar2).Value = ExcludeLvl;
            cmd.Parameters.Add("CUR_OPERATION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }

        public DataTable GetApprovalMatrixWithLeaveSup(string EmpCode)
        {

            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_COMMONMETHOD.SPROC_APPAUTHORITIES_LEVELS";
            cmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
            cmd.Parameters.Add("CUR_LIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }

        public DataTable GetShiftTimings(string strPlantID)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_COMMONMETHOD.SPROC_SHIFTS_GET";
            cmd.Parameters.Add("PLANT_IN", OracleDbType.Varchar2).Value = strPlantID;
            cmd.Parameters.Add("CUR_SITE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }

        public DataTable GetOrgLevelHead(string strLevelType, string strLevelID)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_COMMONMETHOD.SPROC_ORGLEVELHEAD_GET";
            cmd.Parameters.Add("LEVELTYPE_IN", OracleDbType.Varchar2).Value = strLevelType;
            cmd.Parameters.Add("LEVELID_IN", OracleDbType.Varchar2).Value = strLevelID;
            cmd.Parameters.Add("CUR_HEAD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }

        // GET DEPT ACCORDING TO PLANT USED IN QMS DEPT AND PROCEDURE.
        public DataTable GetDeptartment_Plant(string strPlantID)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_COMMONMETHOD.SPROC_DEPTQMS_GET";
            cmd.Parameters.Add("PLANT_IN", OracleDbType.Varchar2).Value = strPlantID;
            cmd.Parameters.Add("CUR_SITE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }
        public DataSet GetHOMEPAGECOUNT_GET(string EMPCODE_IN)
        {
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_COMMONMETHOD.SPROC_HOMEPAGECOUNT_GET";
            cmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = EMPCODE_IN;
            cmd.Parameters.Add("CUR_HCGDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("CUR_UNIFORMDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("CUR_ITDECLARATIONCOUNT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("CUR_SURVEY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("CUR_EMPDESIGNATION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("CUR_FEEDBACKQUS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataSet(cmd);
            cmd.Dispose();
        }
        public DataSet GetAPPROVALCOUNT_GET(string EMPCODE_IN)
        {
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_COMMONMETHOD.SPROC_APPROVALCOUNT_GET";
            cmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = EMPCODE_IN;
            cmd.Parameters.Add("CMP_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("REC_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("REQ_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("REQBACK_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("OJT_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("LEAVE_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("ATTENDANCE_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("GUESTHOUSE_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("COORDINATER_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("KRAAUTHORITY_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("DEPTGOAL_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("PRESIDENTPAGE_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("TOURCOORDINATER_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("SEFTY_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("CONFIRMATION_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("MIS_RESULT", OracleDbType.RefCursor, 4000).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("IOMAPP_RESULT", OracleDbType.RefCursor, 4000).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("IOMREQ_RESULT", OracleDbType.RefCursor, 4000).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("NORMALIZATIONDIV_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("NORMALIZATIONOP_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("CUR_VENDORREQCNT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("CUR_VENDORAPPCNT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataSet(cmd);
            cmd.Dispose();
        }
        public DataSet GETKIOPERDIVDEPTSEC(string OPERATION, string DIVISION, string DEPARTMENT)
        {
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_CMPCOMMON.GETOPER_DIV_DEPT_SEC";
            cmd.Parameters.Add("OPER_IN", OracleDbType.Varchar2).Value = OPERATION;
            cmd.Parameters.Add("DIVI_IN", OracleDbType.Varchar2).Value = DIVISION;
            cmd.Parameters.Add("DEPT_IN", OracleDbType.Varchar2).Value = DEPARTMENT;
            cmd.Parameters.Add("CUR_KI", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("CUR_OPER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("CUR_DIV", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("CUR_DEPT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("CUR_SEC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataSet(cmd);
            cmd.Dispose();
        }

        public int CheckRequestStatusForITSELF(string EMPCODE)
        {
            int TxnNumber = 0;
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = _conn.getConnectingString(); ;
            OracleCommand objCmd;
            OracleTransaction objTxn;
            int rownum;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                objTxn = objCn.BeginTransaction();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.Transaction = objTxn;
                    objCmd.CommandText = "PKG_COMMONMETHOD.SPROC_SELF_IT_GET_STATUS";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = EMPCODE;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();
                    TxnNumber = System.Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());


                }
                catch (Exception ex)
                {
                    //ROLLBACK TRANSACTION
                    objTxn.Rollback();
                    //  ErrorMessage = ex.Message;

                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
                return (TxnNumber);
            }
        }

        public string UPDATE_SAP_STATUS(string strType, string strTransId, string strSapStatus, string strSapRemarks, string strModifiedBy)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_COMMONMETHOD.SPROC_UPDATE_SAP_STATUS";
            oCmd.Parameters.Add("TYPE_IN", OracleDbType.Varchar2).Value = strType;
            oCmd.Parameters.Add("TRANSID_IN", OracleDbType.Varchar2).Value = strTransId;
            oCmd.Parameters.Add("SAPSTATUS_IN", OracleDbType.Varchar2).Value = strSapStatus;
            oCmd.Parameters.Add("SAPREMARKS_IN", OracleDbType.Varchar2).Value = strSapRemarks;
            oCmd.Parameters.Add("MODIDIEDBY_ID", OracleDbType.Int32).Value = strModifiedBy;
            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
            oDataMgmt.ExecuteQuery(oCmd);
            string strMsg = Convert.ToString(oCmd.Parameters["RESULT_OUT"].Value) + "#" + Convert.ToString(oCmd.Parameters["ERROR_MSG"].Value);
            return strMsg;
        }

        /// <summary>
        /// 
        /// </summary>
        public DataTable GetApprenticeAssociateDetails(string KI, string OperationID, string DivisionID, string DepartmentID, string SectionID,
                                             string EmpCode, string FirstName, string LastName)
        {

            string strAssociateLowestLevel = string.Empty;

            if (OperationID == "" && DivisionID == "" && DepartmentID == "" && SectionID == "")
            {
                //Get All Data irrespective of any any levels
                strAssociateLowestLevel = "";
            }
            else if (SectionID != "")
            {
                strAssociateLowestLevel = SectionID;
            }
            else if (DepartmentID != "")
            {
                strAssociateLowestLevel = DepartmentID;
            }
            else if (DivisionID != "")
            {
                strAssociateLowestLevel = DivisionID;
            }
            else if (OperationID != "")
            {
                strAssociateLowestLevel = OperationID;
            }


            DataTable dt = new DataTable();
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_COMMONMETHOD.SPROC_APPRENTICESEARCH_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
            oCmd.Parameters.Add("FIRSTNAME_IN", OracleDbType.Varchar2).Value = FirstName;
            oCmd.Parameters.Add("LASTNAME_IN", OracleDbType.Varchar2).Value = LastName;
            //oCmd.Parameters.Add("BLOODGROUP_IN", OracleDbType.Varchar2).Value = BloodGroup;
            //oCmd.Parameters.Add("EMAILID_IN", OracleDbType.Varchar2).Value = EMailID;
            //oCmd.Parameters.Add("DESIGNATION_IN", OracleDbType.Varchar2).Value = Designation;
            // oCmd.Parameters.Add("FUNDESIGNATION_IN", OracleDbType.Varchar2).Value = FunDesignation;
            oCmd.Parameters.Add("LVL", OracleDbType.Varchar2).Value = strAssociateLowestLevel;
            oCmd.Parameters.Add("KIID", OracleDbType.Varchar2).Value = 14;
            oCmd.Parameters.Add("CUR_ASSOCIATE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return dt;
        }

        public string EmailApprovalInsert(string strwid, string strcontroler, string straction, string strTid)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = _conn.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_COMMONMETHOD.SPROC_EMAILAPPROVAL_SET";
                    objCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strwid;
                    objCmd.Parameters.Add("CNTR_NAME_IN", OracleDbType.Varchar2).Value = strcontroler;
                    objCmd.Parameters.Add("CNTR_ACTION_IN", OracleDbType.Varchar2).Value = straction;
                    objCmd.Parameters.Add("CNTR_VALUE_IN", OracleDbType.Varchar2).Value = strTid;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    return Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
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

        public DataSet EmailApprovalGet(long strEmpCode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = _conn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_COMMONMETHOD.SPROC_EMAILURL_GET";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("CUR_WINDOWLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
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

        //Change done by aumento on 24012023================================================================================================================

        /// <summary>
        /// GET EMPLOYEE OFFICIAL DETAIL LIKE DESIGNATION,SECTION,DEPARTMENT,DIVISION,OPERATION
        /// WRITTEN BY SANTOSH ON 3 NOV 2010
        /// </summary>
        /// <param name="userID"></param>

        /// <returns></returns>
        public DataSet GetGunctionDesID(string userID)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = _conn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_COMMONMETHOD.SPROC_ADFUNDESIG_BYEMP_GET";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = userID;
                    objCmd.Parameters.Add("CUR_WINDOWLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
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


        //=================================================================================================================================================

        public DataSet ManageTransferApproval(string strSupEmpCode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = _conn.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_COMMONMETHOD.SPROC_AD_PENDINGTRANSFERAPPROVAL";
                    objCmd.Parameters.Add("CUR_PENDINGAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("SUPEMPCODE_IN", OracleDbType.Int32).Value = strSupEmpCode;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
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

        public DataSet ManageLookSeeApproval(string strSupEmpCode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = _conn.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_COMMONMETHOD.SPROC_AD_PENDINGLOOKSEEAPPROVAL";
                    objCmd.Parameters.Add("CUR_PENDINGAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("SUPEMPCODE_IN", OracleDbType.Int32).Value = strSupEmpCode;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
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
        public DataSet ManageLookSeeSTLApproval(string strSupEmpCode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = _conn.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_COMMONMETHOD.SPROC_AD_PENDINGLOOKSEESTLAPPROVAL";
                    objCmd.Parameters.Add("CUR_PENDINGAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("SUPEMPCODE_IN", OracleDbType.Int32).Value = strSupEmpCode;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
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
        public DataSet ManageRelocationApproval(string strSupEmpCode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = _conn.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_COMMONMETHOD.SPROC_AD_PENDINGRELOCATIONAPPROVAL";
                    objCmd.Parameters.Add("CUR_PENDINGAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("SUPEMPCODE_IN", OracleDbType.Int32).Value = strSupEmpCode;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
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
        public DataSet ManageRelocationSTLApproval(string strSupEmpCode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = _conn.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_COMMONMETHOD.SPROC_AD_PENDINGRELOCATIONSTLAPPROVAL";
                    objCmd.Parameters.Add("CUR_PENDINGAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("SUPEMPCODE_IN", OracleDbType.Int32).Value = strSupEmpCode;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
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

        //Change done b ay Aumento on 18012023======--SR43825---====================================================
        public DateTime GetHOverstayDate(string OverstayDate, string strAdempcode)
        {
            DateTime OveDate;
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_COMMONMETHOD.SPROC_GET_HOLIDAY_OVERSTAYDATE";
            cmd.Parameters.Add("OverstayDate", OracleDbType.Varchar2).Value = OverstayDate;
            cmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strAdempcode;
            cmd.Parameters.Add("RESULT_OUT", OracleDbType.Date).Direction = ParameterDirection.Output;
            oDataMgmt.ExecuteQuery(cmd);
            OveDate = System.Convert.ToDateTime(cmd.Parameters["RESULT_OUT"].Value.ToString());
            //objDt = oDataMgmt.GetDataTable(cmd);
            return OveDate;
        }
        //===============================================================================================

        //Below New added by aumento as on 16062023 for the SR51079=====================================================================
        public string GetGender(string OverstayDate, string Adempcode)
        {
            string Flag;
            try
            {
                objDt = new DataTable();
                cmd = new OracleCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.BindByName = true;
                cmd.CommandText = "PKG_COMMONMETHOD.SPROC_GET_ISHOLIDAYODATE";
                cmd.Parameters.Add("OVERSTAYDATE", OracleDbType.Varchar2).Value = OverstayDate;
                cmd.Parameters.Add("ADEMPCODE", OracleDbType.Varchar2).Value = Adempcode;
                cmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(cmd);
                Flag = cmd.Parameters["RESULT_OUT"].Value.ToString();
                return Flag;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public DataTable GetMobileCoverDetail(string strEcode, string strName, string strDesignation, string strLVL, string strPlant)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_COMMONMETHOD.SPROC_ASSOCIATEMOBILECOVER_GET";
            cmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strEcode;
            cmd.Parameters.Add("FIRSTNAME_IN", OracleDbType.Varchar2).Value = strName;
            cmd.Parameters.Add("DESIGNATION_IN", OracleDbType.Varchar2).Value = strDesignation;
            cmd.Parameters.Add("LVL", OracleDbType.Varchar2).Value = strLVL;
            cmd.Parameters.Add("PLANTID_IN", OracleDbType.Varchar2).Value = strPlant;
            cmd.Parameters.Add("CUR_ASSOCIATE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }
        //========================================================================================================


        // Start Added by Aumento :: SR78338
        public DataTable GetEmployeeLoginMapIDDetails(string MapFlag, string EmpFlag)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "GetEmployeeLoginMapIDDetails";
            cmd.Parameters.Add("MapFlag", OracleDbType.Varchar2).Value = MapFlag;
            cmd.Parameters.Add("EmpFlag", OracleDbType.Varchar2).Value = EmpFlag;
            cmd.Parameters.Add("CUR_PENDINGDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }
        // End Added by Aumento :: SR78338

        // NON SAP Payment Module
        public DataTable GetDealerData(string strSearch)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "NSP_GET_DEALER_DETAILS_BY_SEARCH";
            cmd.Parameters.Add("SEARCH_STRING_", OracleDbType.Varchar2).Value = strSearch;
            cmd.Parameters.Add("TBL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            //oDataMgmt = new DataManagement();
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }
        // NON SAP Payment Module

        // START :: SR107643 :: LC ENHANCEMENT ::  AUMENTO
        public DataTable LC_GetApprovalHistory(int lctranno)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_OFFOUTDUTYTRANS.LC_GET_APPROVAL_HISTORY";
            cmd.Parameters.Add("p_lctranno", OracleDbType.Int32).Value = lctranno;
            cmd.Parameters.Add("p_approval_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            objDt = oDataMgmt.GetDataTable(cmd);

            return (objDt);
        }
        // END :: SR107643 :: LC ENHANCEMENT ::  AUMENTO

        //Added by aumento for SR92999
        public string GetSupervisorEmpCodeByUserID(string strUserId)
        {
            DataTable objDt = new DataTable();
            OracleCommand cmd = new OracleCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_VHICLEREQUEST.GET_SUPERVISOR_EMP_CODE";
            cmd.Parameters.Add("p_UserId", OracleDbType.Varchar2).Value = strUserId;
            cmd.Parameters.Add("p_SupervisorEmpCode", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
            oDataMgmt.ExecuteQuery(cmd);
            return Convert.ToString(cmd.Parameters["p_SupervisorEmpCode"].Value);
        }
        //Added by aumento for SR92999

        //Start: Expense Provision - Added by TTL
        public DataSet ManageExpenseProvisionApproval(long empCode, string deptCode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = _conn.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_EXPENSE_PROVISION.SPROC_PENDINGAPPROVAL";

                    // Input parameters
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = Convert.ToInt32(empCode);
                    objCmd.Parameters.Add("DEPT_IN", OracleDbType.Varchar2).Value = deptCode;

                    // Output parameter
                    objCmd.Parameters.Add("CUR_PENDINGPR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;


                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
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
        //END: Expense Provision - Added by TTL

        //Changed by TTL on 07-July-2025 against SR101846 > CR6643 - Start
        public DataTable GetProcesswisePendingCount(long empCode)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "SPROC_PROCESSWISE_PENDINGCOUNT";
            cmd.Parameters.Add("EMPCODE_IN", OracleDbType.Long).Value = empCode;
            cmd.Parameters.Add("TBL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            objDt = oDataMgmt.GetDataTable(cmd);
            return objDt;
        }
        //Changed by TTL on 07-July-2025 against SR101846 > CR6643 - End

        // Added by TTL on 17-July-2025 against SR93758 > CR5975 - Start
        #region IOMDasboardChart
        public DataTable IOMUserReportGraphData(SearchIOMsproc param)
        {
            objDt = new DataTable();
            cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_POREQUEST.SPROC_IOM_DASHBOARD";

            cmd.Parameters.Add("START_DATE_IN", OracleDbType.Date).Value = param.StartDateIn ?? Convert.DBNull;
            cmd.Parameters.Add("END_DATE_IN", OracleDbType.Date).Value = param.EndDateIn ?? Convert.DBNull;
            cmd.Parameters.Add("OPRN_ID_IN", OracleDbType.Long).Value = param.OprnIdIn;
            cmd.Parameters.Add("DIV_ID_IN", OracleDbType.Long).Value = param.DivIdIn;
            cmd.Parameters.Add("DEPT_ID_IN", OracleDbType.Long).Value = param.DeptIdIn;
            cmd.Parameters.Add("SEC_ID_IN", OracleDbType.Long).Value = param.SecIdIn;
            cmd.Parameters.Add("ADDED_BY_IN", OracleDbType.Long).Value = param.AddedByIn;
            cmd.Parameters.Add("CATMST_ID_IN", OracleDbType.Long).Value = param.CatmstIdIn;
            cmd.Parameters.Add("KIID_IN", OracleDbType.Long).Value = param.KiidIn;
            cmd.Parameters.Add("PROC_STATUSES_IN", OracleDbType.Varchar2).Value = param.ProcStatusesIn ?? Convert.DBNull;
            //Added by TTL on 28-July-2025 against SR104160 > CR6821 - Start
            cmd.Parameters.Add("PENDING_AT_USERS_IN", OracleDbType.Varchar2).Value = param.PendingAtUsersIn ?? Convert.DBNull;
            //Added by TTL on 28-July-2025 against SR104160 > CR6821 - End
            cmd.Parameters.Add("ITEM_DETAIL_IN", OracleDbType.Varchar2).Value = param.ItemDetailIn ?? Convert.DBNull;
            cmd.Parameters.Add("ADDEDBY_NAME_IN", OracleDbType.Varchar2).Value = param.AddedbyNameIn ?? Convert.DBNull;
            cmd.Parameters.Add("CUR_APPROVAL_NOTES", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            objDt = oDataMgmt.GetDataTable(cmd);
            return objDt;
        }
        #endregion
        // Added by TTL on 17-July-2025 against SR93758 > CR5975 - End
    }
}
