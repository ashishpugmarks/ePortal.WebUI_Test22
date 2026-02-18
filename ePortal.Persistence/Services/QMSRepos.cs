using ePortal.Persistence.Interface;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace ePortal.Persistence.Services
{
    public class QMSRepos : IQMSRepos
    {
        private readonly IDataManagement oDataMgmt;
        public QMSRepos(IDataManagement _IDataManagement)
        {
            oDataMgmt = _IDataManagement;
        }

        public DataSet QMSDeptProc()
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandText = "PKG_QMS.SPROC_QC_DeptProc_GET";
            oCmd.Parameters.Add("CUR_QMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            return oDataMgmt.GetDataSet(oCmd);
        }

        //GET QMS DEPT PROCEDURE ACCORDING TO PLANTID
        public DataTable QMSDeptProc_ByPlantID(string strPlantId, string strDeptId)
        {


            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandText = "PKG_QMS.SPROC_QCDeptProc_Plant_GET";
            oCmd.Parameters.Add("PLANTID_IN", OracleDbType.Varchar2).Value = strPlantId;
            oCmd.Parameters.Add("DEPTID_IN", OracleDbType.Varchar2).Value = strDeptId;
            oCmd.Parameters.Add("CUR_QMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);

        }

        public DataSet DeptSectProcList(string strDeptCode, string PLANTID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandText = "PKG_QMS.SPROC_QC_DeptSectProc_GET";
            oCmd.Parameters.Add("CUR_QMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("DeptID_IN", OracleDbType.Int32).Value = strDeptCode;
            oCmd.Parameters.Add("PLANTID_IN", OracleDbType.Int32).Value = PLANTID;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            return oDataMgmt.GetDataSet(oCmd);
            oCmd.Dispose();
        }

        //Added by alok
        public DataSet getQmsstandards(string strparentid, string strstatus)
        {

            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandText = "PKG_QMS.SPROC_Getqms";
            oCmd.Parameters.Add("CUR_qms", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("PARENTID_IN", OracleDbType.Varchar2).Value = strparentid;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strstatus;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            return oDataMgmt.GetDataSet(oCmd);
            oCmd.Dispose();

        }
        public DataSet getQmsDetails()
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_QMS.SPROC_GETNEWQMS";
            oCmd.Parameters.Add("CUR_qms", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            return oDataMgmt.GetDataSet(oCmd);
            oCmd.Dispose();

        }

        public DataSet getQmsDetails1()
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_QMS.SPROC_Getqms1";
            oCmd.Parameters.Add("CUR_qms", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            return oDataMgmt.GetDataSet(oCmd);
            oCmd.Dispose();

        }
        public string QMSDetail(string Description, string Filename, string Department, string Section, int flagvalue, string Qmsid, int Status, string Remarks, string strPlant)
        {

            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_QMS.SPROC_NEWQMS";
            ocmd.Parameters.Add("FLAGVALUE_IN", OracleDbType.Int32, 2).Value = flagvalue;
            ocmd.Parameters.Add("QCQMSDOCID_IN", OracleDbType.Varchar2).Value = Qmsid;
            ocmd.Parameters.Add("DESCRIPTION_IN", OracleDbType.Varchar2).Value = Description;
            ocmd.Parameters.Add("FILENAME_IN", OracleDbType.Varchar2).Value = Filename;
            ocmd.Parameters.Add("DEPARTMENT_IN", OracleDbType.Varchar2).Value = Department;
            ocmd.Parameters.Add("SECTION_IN", OracleDbType.Varchar2).Value = Section;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Int32, 2).Value = Status;
            ocmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = Remarks;
            ocmd.Parameters.Add("PLANT_IN", OracleDbType.Varchar2).Value = strPlant;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            oDataMgmt.ExecuteQuery(ocmd);
            string Result = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);

            return Result;

        }

        public DataSet getQmsDepartmentalDetails()
        {

            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_QMS.SPROC_GETQMS_DETAILS";
            oCmd.Parameters.Add("CUR_qms", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            return oDataMgmt.GetDataSet(oCmd);
            oCmd.Dispose();

        }
        public DataSet getQmsSectionDetails(string strDepartment)
        {

            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_QMS.SPROC_GETQMS_SECTION";
            oCmd.Parameters.Add("ADDEPARTMENTID_IN", OracleDbType.Varchar2).Value = strDepartment;
            oCmd.Parameters.Add("CUR_qms", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            return oDataMgmt.GetDataSet(oCmd);
            oCmd.Dispose();

        }
        public DataSet getQms_Search(string strDepartment, string strSection, string strActive)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_QMS.SPROC_QMS_SEARCH";
            oCmd.Parameters.Add("ADDEPARTMENTID_IN", OracleDbType.Varchar2).Value = strDepartment;
            oCmd.Parameters.Add("ADSECTIONID_IN", OracleDbType.Varchar2).Value = strSection;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strActive;
            oCmd.Parameters.Add("CUR_qms", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            return oDataMgmt.GetDataSet(oCmd);
            oCmd.Dispose();

        }

        //New QMS Search with ISODocument table
        public DataSet getQms_Search_New(string strDepartment, string strSection, string description, string strActive, string QMSDOCID)
        {

            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_QMS.SPROC_MANAGEQMSDEPT";
            oCmd.Parameters.Add("ADDEPARTMENTID_IN", OracleDbType.Varchar2).Value = strDepartment;
            oCmd.Parameters.Add("ADSECTIONID_IN", OracleDbType.Varchar2).Value = strSection;
            oCmd.Parameters.Add("DESCRIPTION_IN", OracleDbType.Varchar2).Value = description;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strActive;
            oCmd.Parameters.Add("QMSDOCID_IN", OracleDbType.Varchar2).Value = QMSDOCID;
            oCmd.Parameters.Add("CUR_qms", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            return oDataMgmt.GetDataSet(oCmd);
            oCmd.Dispose();

        }


        // GET DEPT ACCORDING TO PLANT USED IN QMS DEPT AND PROCEDURE.
        public DataTable GetDeptartment_Plant(string strPlantID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_QMS.SPROC_DEPTQMS_GET";
            oCmd.Parameters.Add("PLANT_IN", OracleDbType.Varchar2).Value = strPlantID;
            oCmd.Parameters.Add("CUR_QMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);

        }


        //====================================================================//

        // GET DEPT NAME USING IN ISO DOC SYSTE
        public DataTable Get_DeptHead(string strEmpCode, string strChangeType)
        {

            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_QMS.SPROC_DEPTHEAD_GET11";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
            oCmd.Parameters.Add("CHANGETYPE_IN", OracleDbType.Varchar2).Value = strChangeType;
            oCmd.Parameters.Add("CUR_QMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);


        }

        //INSERT INTO ISO DOUCMENT TABLE INFORMATION
        public string ISODOCDetail_Set(string strIsoDocId, string strIsoDocNo, string strIsoDocTitle, string strChangeType,
                                       string strPlant, string strDept, string strSection, string strOldRevNo,
                                       string strOldRevDate, string strNewRevNo, string strNewRevDate, string strFilename,
                                       string strAddedBy, string strReqEmpCode, string strReason, string strAppAuthEcode,
                                       string strOldid, int strOption, string strrevdocno)
        {




            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_QMS.SPROC_ISODOC_SET";
            ocmd.Parameters.Add("ISODOCID_IN", OracleDbType.Varchar2).Value = strIsoDocId;
            ocmd.Parameters.Add("DOCNO_IN", OracleDbType.Varchar2).Value = strIsoDocNo;
            ocmd.Parameters.Add("DOCTITLE_IN", OracleDbType.Varchar2).Value = strIsoDocTitle;
            ocmd.Parameters.Add("CHANGETYPE_IN", OracleDbType.Varchar2).Value = strChangeType;
            ocmd.Parameters.Add("PLANT_IN", OracleDbType.Varchar2).Value = strPlant;
            ocmd.Parameters.Add("DEPARTMENT_IN", OracleDbType.Varchar2).Value = strDept;
            ocmd.Parameters.Add("SECTION_IN", OracleDbType.Varchar2).Value = strSection;
            ocmd.Parameters.Add("OLDREVISIONNO_IN", OracleDbType.Varchar2).Value = strOldRevNo;
            ocmd.Parameters.Add("OLDREVISIONDATE_IN", OracleDbType.Varchar2).Value = strOldRevDate;
            ocmd.Parameters.Add("NEWREVISIONNO_IN", OracleDbType.Varchar2).Value = strNewRevNo;
            ocmd.Parameters.Add("NEWREVISIONDATE_IN", OracleDbType.Varchar2).Value = strNewRevDate;
            ocmd.Parameters.Add("FILENAME_IN", OracleDbType.Varchar2).Value = strFilename;
            ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = strAddedBy;
            ocmd.Parameters.Add("REQEMPCODE_IN", OracleDbType.Varchar2).Value = strReqEmpCode;
            ocmd.Parameters.Add("REQEMPREMARK_IN", OracleDbType.Varchar2).Value = strReason;
            ocmd.Parameters.Add("APPRAUTHECODE_IN", OracleDbType.Varchar2).Value = strAppAuthEcode;
            ocmd.Parameters.Add("OLDISODOCID_IN", OracleDbType.Varchar2).Value = strOldid;
            ocmd.Parameters.Add("OPTION_IN", OracleDbType.Int32, 1).Value = strOption;
            ocmd.Parameters.Add("REVDOCNO_IN", OracleDbType.Varchar2).Value = strrevdocno;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            oDataMgmt.ExecuteQuery(ocmd);
            string Result = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);
            return Result;


        }

        //UPDATE INTO ISO DOUCMENT TABLE INFORMATION
        public string ISODOCDetail_UPDATE(string strIsoDocId, string strIsoDocNo, string strIsoDocTitle, string strChangeType,
            string strPlant, string strDept, string strSection, string strOldRevNo, string strOldRevDate, string strNewRevNo,
            string strNewRevDate, string strFilename, string strAddedBy,
            string strReqEmpCode, string strReason, string strAppAuthEcode, string strOldid, int strOption)
        {


            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_QMS.SPROC_ISODOC_UPDATE";
            ocmd.Parameters.Add("ISODOCID_IN", OracleDbType.Varchar2).Value = strIsoDocId;
            ocmd.Parameters.Add("DOCNO_IN", OracleDbType.Varchar2).Value = strIsoDocNo;
            ocmd.Parameters.Add("DOCTITLE_IN", OracleDbType.Varchar2).Value = strIsoDocTitle;
            ocmd.Parameters.Add("CHANGETYPE_IN", OracleDbType.Varchar2).Value = strChangeType;
            ocmd.Parameters.Add("PLANT_IN", OracleDbType.Varchar2).Value = strPlant;
            ocmd.Parameters.Add("DEPARTMENT_IN", OracleDbType.Varchar2).Value = strDept;
            ocmd.Parameters.Add("SECTION_IN", OracleDbType.Varchar2).Value = strSection;
            ocmd.Parameters.Add("OLDREVISIONNO_IN", OracleDbType.Varchar2).Value = strOldRevNo;
            ocmd.Parameters.Add("OLDREVISIONDATE_IN", OracleDbType.Varchar2).Value = strOldRevDate;
            ocmd.Parameters.Add("NEWREVISIONNO_IN", OracleDbType.Varchar2).Value = strNewRevNo;
            ocmd.Parameters.Add("NEWREVISIONDATE_IN", OracleDbType.Varchar2).Value = strNewRevDate;
            ocmd.Parameters.Add("FILENAME_IN", OracleDbType.Varchar2).Value = strFilename;
            ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = strAddedBy;
            ocmd.Parameters.Add("REQEMPCODE_IN", OracleDbType.Varchar2).Value = strReqEmpCode;
            ocmd.Parameters.Add("REQEMPREMARK_IN", OracleDbType.Varchar2).Value = strReason;
            ocmd.Parameters.Add("APPRAUTHECODE_IN", OracleDbType.Varchar2).Value = strAppAuthEcode;
            ocmd.Parameters.Add("OLDISODOCID_IN", OracleDbType.Varchar2).Value = strOldid;
            ocmd.Parameters.Add("OPTION_IN", OracleDbType.Int32, 1).Value = strOption;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            oDataMgmt.ExecuteQuery(ocmd);
            string Result = Convert.ToString(ocmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(ocmd.Parameters["ERRMSG"].Value);
            return Result;

        }

        //GET PENDING QMS DOCUMENT REQUEST LIST AGAINST EMPLOYEECODE............1
        public DataTable GetPendingRequestList(string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_QMS.SPROC_PENDINGREQLIST_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = EmpCode;
            oCmd.Parameters.Add("CUR_QMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        //GET QMS DOCUMENT DATA INFO USING DOC ID
        public DataTable ISODOCDetail_Get(string strIsodocid)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_QMS.SPROC_ISODOC_GET";
            oCmd.Parameters.Add("ISODOCID_IN", OracleDbType.Varchar2).Value = strIsodocid;
            oCmd.Parameters.Add("CUR_QMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        //GET PENDING APPROVAL LIST ..............................................2
        public DataTable GetPendingApprovalList(string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_QMS.SPROC_APPPENDINGLIST_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = EmpCode;
            oCmd.Parameters.Add("CUR_QMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        //GET HISTORY REQUEST LIST.........................................3
        public DataTable GetHistoryRequestList(string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_QMS.SPROC_HISTORYREQLIST_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = EmpCode;
            oCmd.Parameters.Add("CUR_QMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        //GET REQUEST DETAIL FOR HISTORY VIEW
        public DataTable GetRequestDetail(string RequestID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_QMS.SPROC_REQDETAIL_GET";
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = RequestID;
            oCmd.Parameters.Add("CUR_QMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        //GET HISTORY APPROVAL LIST
        public DataTable GetHistoryApprovalList(string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_QMS.SPROC_APPHISTORYLIST_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = EmpCode;
            oCmd.Parameters.Add("CUR_QMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        //SET CANCELLATION AND APPROVAL REMARKS FOR QMS DOC
        public string ISODOCRemark_Set(string strIsoDocId, string strEmpCode, string strEmpRemark, string strEmpStatus,
           string strAppAuthEcode, string strAppAuthRemark, string strAppAuthStatus, string strIsoAuthEcode, string strIsoAuthRemark,
            string strIsoAuthStatus, string strStatus, string strOldIsoDocId, string strReqType)
        {



            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_QMS.SPROC_ISODOCREMARK_SET";
            ocmd.Parameters.Add("ISODOCID_IN", OracleDbType.Varchar2).Value = strIsoDocId;
            ocmd.Parameters.Add("REQEMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
            ocmd.Parameters.Add("REQEMPREMARK_IN", OracleDbType.Varchar2).Value = strEmpRemark;
            ocmd.Parameters.Add("REQEMPSTATUS_IN", OracleDbType.Varchar2).Value = strEmpStatus;
            ocmd.Parameters.Add("APPRAUTHECODE_IN", OracleDbType.Varchar2).Value = strAppAuthEcode;
            ocmd.Parameters.Add("APPAUTHREMARK_IN", OracleDbType.Varchar2).Value = strAppAuthRemark;
            ocmd.Parameters.Add("APPAUTHSTATUS_IN", OracleDbType.Varchar2).Value = strAppAuthStatus;
            ocmd.Parameters.Add("ISOAPPECODE_IN", OracleDbType.Varchar2).Value = strIsoAuthEcode;
            ocmd.Parameters.Add("ISOAPPREMARK_IN", OracleDbType.Varchar2).Value = strIsoAuthRemark;
            ocmd.Parameters.Add("ISOAPPSTATUS_IN", OracleDbType.Varchar2).Value = strIsoAuthStatus;
            ocmd.Parameters.Add("OLDDOCNO_IN", OracleDbType.Varchar2).Value = strOldIsoDocId;
            ocmd.Parameters.Add("REQUESTTYPE_IN", OracleDbType.Varchar2).Value = strReqType;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            oDataMgmt.ExecuteQuery(ocmd);
            string MSG = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return MSG;

        }

        //GET DATATABLE OF ISO DOCUMENT DETAIL USED IN ADMIN END
        public DataTable ISODocumentInfo_Get(string EmpCode, string strRequstType, string strRequstStatus, string strFromdate, string strTodate,
            string strDept, string strPlant, string strSection, string strActive)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_QMS.SPROC_ADMININFO_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
            oCmd.Parameters.Add("REQUEST_IN", OracleDbType.Varchar2).Value = strRequstType;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strRequstStatus;
            oCmd.Parameters.Add("DATEFROM_IN", OracleDbType.Varchar2).Value = strFromdate;
            oCmd.Parameters.Add("DATETO_IN", OracleDbType.Varchar2).Value = strTodate;
            oCmd.Parameters.Add("DEPT_IN", OracleDbType.Varchar2).Value = strDept;
            oCmd.Parameters.Add("PLANT_IN", OracleDbType.Varchar2).Value = strPlant;
            oCmd.Parameters.Add("SECTION_IN", OracleDbType.Varchar2).Value = strSection;
            //oCmd.Parameters.Add("REQID_IN", OracleDbType.Varchar2).Value = strReqId;
            oCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = strActive;
            oCmd.Parameters.Add("CUR_QMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        //GET DATALIST FOR QMS APPROVAL IN FRONT END PAGE
        public DataTable QMSDocList()
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_QMS.SPROC_GETQMSAPP_DOCLIST";
            oCmd.Parameters.Add("CUR_QMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        //GET DEPT WISE INFORMATION IN FRONT END PAGE
        public DataTable SPROC_QMSAPPLIST_GET(string strDeptid)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_QMS.SPROC_QMSAPPLIST_GET";
            oCmd.Parameters.Add("DEPTID_IN", OracleDbType.Int32).Value = strDeptid;
            oCmd.Parameters.Add("CUR_QMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataTable Get_ISODOCDEPARTMENT(string plant)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_QMS.SPROC_ISODOCDEPARTMENT_GET";
            oCmd.Parameters.Add("PLANTID_IN", OracleDbType.Varchar2).Value = plant;
            oCmd.Parameters.Add("CUR_QMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        //GET QMS DOCUMENT DATA INFO USING DOC ID NEW
        public DataTable ISODOCDetail_Get_Detail(string strIsodocid)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_QMS.SPROC_ISODOC_GET_Detail";
            oCmd.Parameters.Add("ISODOCID_IN", OracleDbType.Varchar2).Value = strIsodocid;
            oCmd.Parameters.Add("CUR_QMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public string UPDATEQMSDOCDEPT(string QMSDOCID, string strDepartment, string strSection, string addedby)
        {
            try
            {
                OracleCommand ocmd = new OracleCommand();
                ocmd.CommandType = CommandType.StoredProcedure;
                ocmd.CommandText = "PKG_QMS.SPROC_UPDATEQMSDOCDEPT";
                ocmd.Parameters.Add("QMSDOCID_IN", OracleDbType.Varchar2).Value = QMSDOCID;
                ocmd.Parameters.Add("DEPT_IN", OracleDbType.Varchar2).Value = strDepartment;
                ocmd.Parameters.Add("SECTION_IN", OracleDbType.Varchar2).Value = strSection;
                ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = addedby;
                ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                ocmd.BindByName = true;
                oDataMgmt.ExecuteQuery(ocmd);
                string MSG = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString();
                return MSG;
            }
            catch (Exception ex)
            {
                string msg = "0#" + ex.Message;
                return msg;
            }
        }

        //GET QMS FORMAT NUMBER LIST
        public DataSet GETQMSFORMATNUMBERLIST(string formatnumberid, string formatnumber, string status)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.Parameters.Add("FORMATNUMBERID_IN", OracleDbType.Varchar2).Value = formatnumberid;
            oCmd.Parameters.Add("FORMATNUMBER_IN", OracleDbType.Varchar2).Value = formatnumber;
            oCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = status;
            oCmd.CommandText = "PKG_QMS.SPROC_QMSFORMATNUMBERLIST_GET";
            oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            return oDataMgmt.GetDataSet(oCmd);
            oCmd.Dispose();
        }

        // INSERT AND UPDATE QMS FORMAT NUMBER
        public string INSERTFORMATNUMBER(string formatnumber, string status, string formatnumberid, string addedby)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_QMS.SPROC_QMSFORMATNUMBER_INSERT";
            oCmd.Parameters.Add("FORMATNUMBER_IN", OracleDbType.Varchar2).Value = formatnumber;
            oCmd.Parameters.Add("FORMATNUMBERID_IN", OracleDbType.Varchar2).Value = formatnumberid;
            oCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = status;
            oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = addedby;
            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            oDataMgmt.ExecuteQuery(oCmd);
            string MSG = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return MSG;
            oCmd.Dispose();
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

        //GET DATATABLE OF ISO DOCUMENT DETAIL USED IN ADMIN HEAD END
        public DataTable ISODADMINHEADINFO_GET(string EmpCode, string strRequstType, string strRequstStatus, string strFromdate, string strTodate,
            string strDept, string strPlant, string strSection, string strActive)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_QMS.SPROC_ADMINHEADINFO_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
            oCmd.Parameters.Add("REQUEST_IN", OracleDbType.Varchar2).Value = strRequstType;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strRequstStatus;
            oCmd.Parameters.Add("DATEFROM_IN", OracleDbType.Varchar2).Value = strFromdate;
            oCmd.Parameters.Add("DATETO_IN", OracleDbType.Varchar2).Value = strTodate;
            oCmd.Parameters.Add("DEPT_IN", OracleDbType.Varchar2).Value = strDept;
            oCmd.Parameters.Add("PLANT_IN", OracleDbType.Varchar2).Value = strPlant;
            oCmd.Parameters.Add("SECTION_IN", OracleDbType.Varchar2).Value = strSection;
            oCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = strActive;
            oCmd.Parameters.Add("CUR_QMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        //SET CANCELLATION AND APPROVAL REMARKS FOR QMS DOC ADMIN HEAD
        public string ISODOCHEADRemark_Set(string strIsoDocId, string strEmpCode, string strEmpRemark, string strEmpStatus,
                                       string strIsoheadAuthEcode, string strIsoheadAuthRemark, string strIsoheadAuthStatus,
                                       string strStatus, string strOldIsoDocId, string strReqType)
        {

            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_QMS.SPROC_ISODOCHEADREMARK_SET";
            ocmd.Parameters.Add("ISODOCID_IN", OracleDbType.Varchar2).Value = strIsoDocId;
            ocmd.Parameters.Add("REQEMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
            ocmd.Parameters.Add("REQEMPREMARK_IN", OracleDbType.Varchar2).Value = strEmpRemark;
            ocmd.Parameters.Add("REQEMPSTATUS_IN", OracleDbType.Varchar2).Value = strEmpStatus;
            ocmd.Parameters.Add("ISOHEADECODE_IN", OracleDbType.Varchar2).Value = strIsoheadAuthEcode;
            ocmd.Parameters.Add("ISOHEADREMARK_IN", OracleDbType.Varchar2).Value = strIsoheadAuthRemark;
            ocmd.Parameters.Add("ISOHEADSTATUS_IN", OracleDbType.Varchar2).Value = strIsoheadAuthStatus;
            ocmd.Parameters.Add("OLDDOCNO_IN", OracleDbType.Varchar2).Value = strOldIsoDocId;
            ocmd.Parameters.Add("REQUESTTYPE_IN", OracleDbType.Varchar2).Value = strReqType;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strStatus;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            oDataMgmt.ExecuteQuery(ocmd);
            string MSG = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG"].Value.ToString();
            return MSG;

        }

        //GET QMS FORMAT NUMBER LIST
        public string CHECKQMSFORMATNUMBER(string formatnumber)
        {

            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_QMS.SPROC_CHECKQMSFORMATNUMBERLIST";
            ocmd.Parameters.Add("FORMATNUMBER_IN", OracleDbType.Varchar2).Value = formatnumber;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            oDataMgmt.ExecuteQuery(ocmd);
            string MSG = Convert.ToString(ocmd.Parameters[""].Value);
            return MSG;

        }

        //GET FINANCE APPROVAL AUTHORITY
        public DataSet GETAPPROVALAUTHORITY(string transid, string plant, string plantappauth, string hoappauth)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_QMS.SPROC_APPROVALAUTHORITY_GET";
            ocmd.Parameters.Add("TRANSID_IN", OracleDbType.Varchar2).Value = transid;
            ocmd.Parameters.Add("PLANT_IN", OracleDbType.Varchar2).Value = plant;
            ocmd.Parameters.Add("PLANTAPPAUTH_IN", OracleDbType.Varchar2).Value = plantappauth;
            ocmd.Parameters.Add("HOAPPAUTH_IN", OracleDbType.Varchar2).Value = hoappauth;
            ocmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            return oDataMgmt.GetDataSet(ocmd);
            ocmd.Dispose();
        }

        //INSERT FINANCE APPROVAL AUTHORITY
        public string INSERTAPPROVALAUTHORITY(string transid, string plant, string plantappauth, string hoappauth, string addedby)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_QMS.SPROC_INSERTAPPROVALAUTHORITY";
            ocmd.Parameters.Add("TRANSID_IN", OracleDbType.Varchar2).Value = transid;
            ocmd.Parameters.Add("PLANT_IN", OracleDbType.Varchar2).Value = plant;
            ocmd.Parameters.Add("PLANTAPPAUTH_IN", OracleDbType.Varchar2).Value = plantappauth;
            ocmd.Parameters.Add("HOAPPAUTH_IN", OracleDbType.Varchar2).Value = hoappauth;
            ocmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = addedby;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            oDataMgmt.ExecuteQuery(ocmd);
            string MSG = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return MSG;
        }

        public DataTable GetPlant(string ADEMPCOE)
        {
            DataTable objDt = new DataTable();
            OracleCommand cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "PKG_QMS.SPROC_PLANT_GET";
            cmd.Parameters.Add("CUR_PLANT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("ADEMPCOE_IN", OracleDbType.Varchar2).Value = ADEMPCOE;
            cmd.BindByName = true;
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }

        public DataTable GetHOAppAuthPlant(string ADEMPCOE)
        {
            DataTable objDt = new DataTable();
            OracleCommand cmd = new OracleCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "PKG_QMS.SPROC_HOAAPPAUTHPLANT_GET";
            cmd.Parameters.Add("CUR_PLANT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("ADEMPCOE_IN", OracleDbType.Varchar2).Value = ADEMPCOE;
            cmd.BindByName = true;
            objDt = oDataMgmt.GetDataTable(cmd);
            return (objDt);
        }

        public string CHECKISPLANTAPPAUTHSET(string plantid)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_QMS.SPROC_ISPLANTAPPAUTHSET_CHK";
            ocmd.Parameters.Add("PLANT_IN", OracleDbType.Varchar2).Value = plantid;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            oDataMgmt.ExecuteQuery(ocmd);
            return ocmd.Parameters["RESULT_OUT"].Value.ToString();
        }

        public string ISODOCAPPROVAL(string REQUESTID, string ECODE, string STATUS, string REMARKS, string CHANGETYPE, string APPROVALTYPE)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_QMS.SPROC_ISODOCUMENTAPPROVALS";
            ocmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = REQUESTID;
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
            ocmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = REMARKS;
            ocmd.Parameters.Add("CHANGETYPE_IN", OracleDbType.Varchar2).Value = CHANGETYPE;
            ocmd.Parameters.Add("APPROVALTYPE_IN", OracleDbType.Varchar2).Value = APPROVALTYPE;
            ocmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            oDataMgmt.ExecuteQuery(ocmd);
            string MSG = ocmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + ocmd.Parameters["ERRMSG_OUT"].Value.ToString();
            return MSG;
        }

        public DataSet GETPLANTHEADREQUEST(string REQECODE, string ENAME, string PLANT, string DEPT, string REQTYPE, string STATUS, string ECODE)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_QMS.SPROC_PLANTHEADREQUEST_GET";
            ocmd.Parameters.Add("REQECODE_IN", OracleDbType.Varchar2).Value = REQECODE;
            ocmd.Parameters.Add("ENAME_IN", OracleDbType.Varchar2).Value = ENAME;
            ocmd.Parameters.Add("PLANT_IN", OracleDbType.Varchar2).Value = PLANT;
            ocmd.Parameters.Add("DEPT_IN", OracleDbType.Varchar2).Value = DEPT;
            ocmd.Parameters.Add("REQTYPE_IN", OracleDbType.Varchar2).Value = REQTYPE;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
            ocmd.Parameters.Add("CUR_REQLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("CUR_PLANT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("CUR_DEPT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            return oDataMgmt.GetDataSet(ocmd);
            ocmd.Dispose();
        }

        public DataSet GETHOHEADREQUEST(string REQECODE, string ENAME, string PLANT, string DEPT, string REQTYPE, string STATUS, string ECODE)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_QMS.SPROC_HOHEADREQUEST_GET";
            ocmd.Parameters.Add("REQECODE_IN", OracleDbType.Varchar2).Value = REQECODE;
            ocmd.Parameters.Add("ENAME_IN", OracleDbType.Varchar2).Value = ENAME;
            ocmd.Parameters.Add("PLANT_IN", OracleDbType.Varchar2).Value = PLANT;
            ocmd.Parameters.Add("DEPT_IN", OracleDbType.Varchar2).Value = DEPT;
            ocmd.Parameters.Add("REQTYPE_IN", OracleDbType.Varchar2).Value = REQTYPE;
            ocmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = STATUS;
            ocmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = ECODE;
            ocmd.Parameters.Add("CUR_REQLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("CUR_PLANT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.Parameters.Add("CUR_DEPT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            return oDataMgmt.GetDataSet(ocmd);
            ocmd.Dispose();
        }

        public DataSet GETQMSHISTORY(string EMPCODE, string REQUESTTYPE)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandType = CommandType.StoredProcedure;
            ocmd.CommandText = "PKG_QMS.SPROC_QMSDOCUMENTHISTORY_GET";
            ocmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = EMPCODE;
            ocmd.Parameters.Add("REQUESTTYPE", OracleDbType.Varchar2).Value = REQUESTTYPE;
            ocmd.Parameters.Add("CUR_GETREQ", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ocmd.BindByName = true;
            return oDataMgmt.GetDataSet(ocmd);
            ocmd.Dispose();
        }
    }
}
