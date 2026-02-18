using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using Oracle.ManagedDataAccess.Client;

namespace ePortal.Persistence.Admin.Services
{
    public class PMS:IPMS
    {
        private readonly IDataManagement oDataMgmt;
        private readonly IConnectionString objCnStr;
        DataSet ds = new DataSet();
        DataRow[] _datarow;

        public PMS(IDataManagement _oDataMgmt, IConnectionString _objCnStr)
        {
            oDataMgmt = _oDataMgmt;
            objCnStr = _objCnStr;
        }


        #region "version1"

        public DataTable GetEvaluatorReviewer(string strEmpCode, string strkiid)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPORC_EVALUATORREVIEWER_GET";
                    objCmd.Parameters.Add("CUR_KI", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = strkiid;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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

        public string RoleResponsibilityEntry(string strEmpCode, string strRole, string strResponsibility,
            string strKi, string strstatus)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_ROLERESPONSIBILITY_ENTRY";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("KI_IN", OracleDbType.Int32).Value = strKi;
                    objCmd.Parameters.Add("ROLE_IN", OracleDbType.Varchar2).Value = strRole;
                    objCmd.Parameters.Add("RESPONSIBILITY_IN", OracleDbType.Varchar2).Value = strResponsibility;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = strstatus;

                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string RoleResponsibilityUpdate(string strRequestid, string strRole, string strResponsibility, string strstatus)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_ROLERESP_UPDATE";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("ROLE_IN", OracleDbType.Varchar2).Value = strRole;
                    objCmd.Parameters.Add("RESPONSIBILITY_IN", OracleDbType.Varchar2).Value = strResponsibility;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = strstatus;

                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public DataTable GetDetailById(string strId, string strEmpCode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPORC_DETAILBYID_GET";
                    objCmd.Parameters.Add("CUR_DETAIL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("KRAID_IN", OracleDbType.Int32).Value = strId;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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

        public string TargetEntry(string strRequestid, string strApplicability, string strCategory,
            string strWeightage, string strTarget, string strProgram)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_TARGET_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("APPLICABILITY_IN", OracleDbType.Varchar2).Value = strApplicability;
                    objCmd.Parameters.Add("CATEGORY_IN", OracleDbType.Varchar2).Value = strCategory;
                    objCmd.Parameters.Add("WEIGHTAGE_IN", OracleDbType.Varchar2).Value = strWeightage;
                    objCmd.Parameters.Add("TARGET_IN", OracleDbType.Varchar2).Value = strTarget;
                    objCmd.Parameters.Add("PROGRAM_IN", OracleDbType.Int32).Value = strProgram;

                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public DataTable GetTarget(string strId)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPORC_TARGET_GET";
                    objCmd.Parameters.Add("CUR_TARGET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("KRAID_IN", OracleDbType.Int32).Value = strId;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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

        public string CheckProrgram(string strRequestid)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_CHECKPROGRAM_GET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;

                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                    return strErrMsg;

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

        public string UpdateTabStatus(string strRequestid)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_TABSTATUS_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string SetIndividualInitiative(string strRequestid, string strImprovement, string strSelfDev, string strCulture, string strAssociateDev, string strButton)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_INDIVIDUALINITIATIVE_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("IMPROVEMENT_IN", OracleDbType.Varchar2).Value = strImprovement;
                    objCmd.Parameters.Add("SELFDEV_IN", OracleDbType.Varchar2).Value = strSelfDev;
                    objCmd.Parameters.Add("CULTURE_IN", OracleDbType.Varchar2).Value = strCulture;
                    objCmd.Parameters.Add("ASSOCIATEDEV_IN", OracleDbType.Varchar2).Value = strAssociateDev;
                    objCmd.Parameters.Add("BUTTONCLICK_IN", OracleDbType.Varchar2).Value = strButton;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public DataTable GetTargetByID(string strTargetId)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPORC_TARGETBYID_GET";
                    objCmd.Parameters.Add("CUR_TARGET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("TARGETID_IN", OracleDbType.Int32).Value = strTargetId;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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

        public string UpdateTargetEntry(string strRequestid, string strApplicability, string strCategory,
            string strWeightage, string strTarget, string strProgram)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_UPDATETARGET_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("APPLICABILITY_IN", OracleDbType.Varchar2).Value = strApplicability;
                    objCmd.Parameters.Add("CATEGORY_IN", OracleDbType.Varchar2).Value = strCategory;
                    objCmd.Parameters.Add("WEIGHTAGE_IN", OracleDbType.Varchar2).Value = strWeightage;
                    objCmd.Parameters.Add("TARGET_IN", OracleDbType.Varchar2).Value = strTarget;
                    objCmd.Parameters.Add("PROGRAM_IN", OracleDbType.Int32).Value = strProgram;

                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string DeleteTargetEntry(string strRequestid)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_DELETETARGET_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string SendGSByApprasier(string strRequestid, string strComment, string strButton,
            string strEmpCode, string str2waycomment1, string str2waycomment2, string str2waycomment3)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_SENDGSBYAPPRASIER_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("COMMENT_IN", OracleDbType.Varchar2).Value = strComment;
                    objCmd.Parameters.Add("BUTTONCLICK_IN", OracleDbType.Varchar2).Value = strButton;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("TWOWAYCOM1_IN", OracleDbType.Varchar2).Value = str2waycomment1;
                    objCmd.Parameters.Add("TWOWAYCOM2_IN", OracleDbType.Varchar2).Value = str2waycomment2;
                    objCmd.Parameters.Add("TWOWAYCOM3_IN", OracleDbType.Varchar2).Value = str2waycomment3;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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
        public string SendGSByApprasier2(string strRequestid, string strComment, string strButton,
                string strEmpCode, string str2waycomment1, string str2waycomment2, string str2waycomment3)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_SENDGSBYAPPRASIER2_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("COMMENT_IN", OracleDbType.Varchar2).Value = strComment;
                    objCmd.Parameters.Add("BUTTONCLICK_IN", OracleDbType.Varchar2).Value = strButton;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("TWOWAYCOM1_IN", OracleDbType.Varchar2).Value = str2waycomment1;
                    objCmd.Parameters.Add("TWOWAYCOM2_IN", OracleDbType.Varchar2).Value = str2waycomment2;
                    objCmd.Parameters.Add("TWOWAYCOM3_IN", OracleDbType.Varchar2).Value = str2waycomment3;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string ReadGuidlines(string strKiId, string strEmpCode)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_APPRASIALENTRY_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("KIID_IN", OracleDbType.Int32).Value = strKiId;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("KRAID_OUT", OracleDbType.Int32, 6).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["KRAID_OUT"].Value) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string SendGSByReviewer(string strRequestid, string strComment, string strEmpCode, string str2waycom1, string str2waycom2, string str2waycom3)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_SENDGSBYREVIEWER_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("COMMENT_IN", OracleDbType.Varchar2).Value = strComment;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("TWOWAYCOM1_IN", OracleDbType.Varchar2).Value = str2waycom1;
                    objCmd.Parameters.Add("TWOWAYCOM2_IN", OracleDbType.Varchar2).Value = str2waycom2;
                    objCmd.Parameters.Add("TWOWAYCOM3_IN", OracleDbType.Varchar2).Value = str2waycom3;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string SendGSByReviewerToRev2(string strRequestid, string strComment, string strEmpCode, string str2waycom1, string str2waycom2, string str2waycom3)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_SENDGSBYREVTOREV2_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("COMMENT_IN", OracleDbType.Varchar2).Value = strComment;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("TWOWAYCOM1_IN", OracleDbType.Varchar2).Value = str2waycom1;
                    objCmd.Parameters.Add("TWOWAYCOM2_IN", OracleDbType.Varchar2).Value = str2waycom2;
                    objCmd.Parameters.Add("TWOWAYCOM3_IN", OracleDbType.Varchar2).Value = str2waycom3;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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


        public string SetResultsAndRatings(string strRequestid, string strRating, string strResults)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_MYRATINGBYASSOCIATE_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("RESULT_IN", OracleDbType.Varchar2).Value = strResults;
                    objCmd.Parameters.Add("RATING_IN", OracleDbType.Varchar2).Value = strRating;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string SetMYTabStatus(string strRequestid)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_MYTABSTATUS_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string SetMYIndividualInitiative(string strRequestid, string strImprovement, string strSelfDev,
        string strCulture, string strAssociateDev, string strButton, string strImpRating, string strSDRating,
        string strHCRating, string strADRating)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_MYINDIINITIATIVE_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("IMPROVEMENT_IN", OracleDbType.Varchar2).Value = strImprovement;
                    objCmd.Parameters.Add("SELFDEV_IN", OracleDbType.Varchar2).Value = strSelfDev;
                    objCmd.Parameters.Add("CULTURE_IN", OracleDbType.Varchar2).Value = strCulture;
                    objCmd.Parameters.Add("ASSOCIATEDEV_IN", OracleDbType.Varchar2).Value = strAssociateDev;
                    objCmd.Parameters.Add("BUTTONCLICK_IN", OracleDbType.Varchar2).Value = strButton;
                    objCmd.Parameters.Add("IMPRATING_IN", OracleDbType.Varchar2).Value = strImpRating;
                    objCmd.Parameters.Add("SDRATING_IN", OracleDbType.Varchar2).Value = strSDRating;
                    objCmd.Parameters.Add("HCRATING_IN", OracleDbType.Varchar2).Value = strHCRating;
                    objCmd.Parameters.Add("ADRATING_IN", OracleDbType.Varchar2).Value = strADRating;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string SetMYEvalRatings(string strRequestid, string strRating, string strComments)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_MYEVALRATING_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("COMMENT_IN", OracleDbType.Varchar2).Value = strComments;
                    objCmd.Parameters.Add("RATING_IN", OracleDbType.Varchar2).Value = strRating;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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
        public string SetMYEval2Ratings(string strRequestid, string strRating, string strComments)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_MYEVAL2RATING_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("COMMENT_IN", OracleDbType.Varchar2).Value = strComments;
                    objCmd.Parameters.Add("RATING_IN", OracleDbType.Varchar2).Value = strRating;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string SetMYEvalTabStatus(string strRequestid)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_MYEVALTABSTATUS_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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
        public string SetMYEval2TabStatus(string strRequestid)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_MYEVAL2TABSTATUS_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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


        public string SetMYIndIniByEval(string strRequestid, string strImprovement, string strSelfDev,
        string strCulture, string strAssociateDev, string strButton, string strImpRating, string strSDRating,
        string strHCRating, string strADRating, string strFeedBack, string strEvalCode, string str2waycom1, string str2waycom2, string str2waycom3)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_MYINDINIEVAL_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("IMPROVEMENT_IN", OracleDbType.Varchar2).Value = strImprovement;
                    objCmd.Parameters.Add("SELFDEV_IN", OracleDbType.Varchar2).Value = strSelfDev;
                    objCmd.Parameters.Add("CULTURE_IN", OracleDbType.Varchar2).Value = strCulture;
                    objCmd.Parameters.Add("ASSOCIATEDEV_IN", OracleDbType.Varchar2).Value = strAssociateDev;
                    objCmd.Parameters.Add("BUTTONCLICK_IN", OracleDbType.Varchar2).Value = strButton;
                    objCmd.Parameters.Add("IMPRATING_IN", OracleDbType.Varchar2).Value = strImpRating;
                    objCmd.Parameters.Add("SDRATING_IN", OracleDbType.Varchar2).Value = strSDRating;
                    objCmd.Parameters.Add("HCRATING_IN", OracleDbType.Varchar2).Value = strHCRating;
                    objCmd.Parameters.Add("ADRATING_IN", OracleDbType.Varchar2).Value = strADRating;
                    objCmd.Parameters.Add("FEEDBACK_IN", OracleDbType.Varchar2).Value = strFeedBack;
                    objCmd.Parameters.Add("EVALCODE_IN", OracleDbType.Varchar2).Value = strEvalCode;
                    objCmd.Parameters.Add("TWOWAYCOM1_IN", OracleDbType.Varchar2).Value = str2waycom1;
                    objCmd.Parameters.Add("TWOWAYCOM2_IN", OracleDbType.Varchar2).Value = str2waycom2;
                    objCmd.Parameters.Add("TWOWAYCOM3_IN", OracleDbType.Varchar2).Value = str2waycom3;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string SetMYRevRatings(string strRequestid, string strRating, string strComments)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_MYREVRATING_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("COMMENT_IN", OracleDbType.Varchar2).Value = strComments;
                    objCmd.Parameters.Add("RATING_IN", OracleDbType.Varchar2).Value = strRating;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string SetMYRevTabStatus(string strRequestid)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_MYREVTABSTATUS_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string SetMYRev2Ratings(string strRequestid, string strRating, string strComments)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_MYREV2RATING_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("COMMENT_IN", OracleDbType.Varchar2).Value = strComments;
                    objCmd.Parameters.Add("RATING_IN", OracleDbType.Varchar2).Value = strRating;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string SetMYRev2TabStatus(string strRequestid)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_MYREV2TABSTATUS_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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


        public string SetMYIndIniByRev(string strRequestid, string strImprovement, string strSelfDev,
        string strCulture, string strAssociateDev, string strButton, string strImpRating, string strSDRating,
        string strHCRating, string strADRating, string str2waycom1, string str2waycom2, string str2waycom3, string strEcode)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_MYINDINIREV_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("IMPROVEMENT_IN", OracleDbType.Varchar2).Value = strImprovement;
                    objCmd.Parameters.Add("SELFDEV_IN", OracleDbType.Varchar2).Value = strSelfDev;
                    objCmd.Parameters.Add("CULTURE_IN", OracleDbType.Varchar2).Value = strCulture;
                    objCmd.Parameters.Add("ASSOCIATEDEV_IN", OracleDbType.Varchar2).Value = strAssociateDev;
                    objCmd.Parameters.Add("BUTTONCLICK_IN", OracleDbType.Varchar2).Value = strButton;
                    objCmd.Parameters.Add("IMPRATING_IN", OracleDbType.Varchar2).Value = strImpRating;
                    objCmd.Parameters.Add("SDRATING_IN", OracleDbType.Varchar2).Value = strSDRating;
                    objCmd.Parameters.Add("HCRATING_IN", OracleDbType.Varchar2).Value = strHCRating;
                    objCmd.Parameters.Add("ADRATING_IN", OracleDbType.Varchar2).Value = strADRating;
                    objCmd.Parameters.Add("TWOWAYCOM1_IN", OracleDbType.Varchar2).Value = str2waycom1;
                    objCmd.Parameters.Add("TWOWAYCOM2_IN", OracleDbType.Varchar2).Value = str2waycom2;
                    objCmd.Parameters.Add("TWOWAYCOM3_IN", OracleDbType.Varchar2).Value = str2waycom3;
                    objCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strEcode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string SetFYAssRatings(string strRequestid, string strRating, string strComments)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_FYASSORATING_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("COMMENT_IN", OracleDbType.Varchar2).Value = strComments;
                    objCmd.Parameters.Add("RATING_IN", OracleDbType.Int32).Value = strRating;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string SetFYTabStatus(string strRequestid)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_FYTABSTATUS_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string SetFYIndIni(string strRequestid, string strImprovement, string strSelfDev,
        string strCulture, string strAssociateDev, string strButton, string strImpRating, string strSDRating,
        string strHCRating, string strADRating, string strAchievement)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_FYINDINI_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("IMPROVEMENT_IN", OracleDbType.Varchar2).Value = strImprovement;
                    objCmd.Parameters.Add("SELFDEV_IN", OracleDbType.Varchar2).Value = strSelfDev;
                    objCmd.Parameters.Add("CULTURE_IN", OracleDbType.Varchar2).Value = strCulture;
                    objCmd.Parameters.Add("ASSOCIATEDEV_IN", OracleDbType.Varchar2).Value = strAssociateDev;
                    objCmd.Parameters.Add("BUTTONCLICK_IN", OracleDbType.Varchar2).Value = strButton;
                    objCmd.Parameters.Add("IMPRATING_IN", OracleDbType.Varchar2).Value = strImpRating;
                    objCmd.Parameters.Add("SDRATING_IN", OracleDbType.Varchar2).Value = strSDRating;
                    objCmd.Parameters.Add("HCRATING_IN", OracleDbType.Varchar2).Value = strHCRating;
                    objCmd.Parameters.Add("ADRATING_IN", OracleDbType.Varchar2).Value = strADRating;
                    objCmd.Parameters.Add("ACHIEVEMENT_IN", OracleDbType.Varchar2).Value = strAchievement;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string SetFYEvalRatings(string strRequestid, string strRating, string strComments)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_FYEVALRATING_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("COMMENT_IN", OracleDbType.Varchar2).Value = strComments;
                    objCmd.Parameters.Add("RATING_IN", OracleDbType.Int32).Value = strRating;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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
        public string SetFYEval2Ratings(string strRequestid, string strRating, string strComments)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_FYEVAL2RATING_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("COMMENT_IN", OracleDbType.Varchar2).Value = strComments;
                    objCmd.Parameters.Add("RATING_IN", OracleDbType.Int32).Value = strRating;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string SetFYEvalTabStatus(string strRequestid)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_FYEVALTABSTATUS_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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
        public string SetFYEval2TabStatus(string strRequestid)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_FYEVAL2TABSTATUS_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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


        public string SetFYIndIniApprasier(string strRequestid, string strImprovement, string strSelfDev,
        string strCulture, string strAssociateDev, string strButton, string strImpRating, string strSDRating,
        string strHCRating, string strADRating, string strFeedBack, string strEvalCode, string strEvalAchCom,
        string str2waycom1, string str2waycom2, string str2waycom3)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_FYINDINIAPPRASIER_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("IMPROVEMENT_IN", OracleDbType.Varchar2).Value = strImprovement;
                    objCmd.Parameters.Add("SELFDEV_IN", OracleDbType.Varchar2).Value = strSelfDev;
                    objCmd.Parameters.Add("CULTURE_IN", OracleDbType.Varchar2).Value = strCulture;
                    objCmd.Parameters.Add("ASSOCIATEDEV_IN", OracleDbType.Varchar2).Value = strAssociateDev;
                    objCmd.Parameters.Add("BUTTONCLICK_IN", OracleDbType.Varchar2).Value = strButton;
                    objCmd.Parameters.Add("IMPRATING_IN", OracleDbType.Varchar2).Value = strImpRating;
                    objCmd.Parameters.Add("SDRATING_IN", OracleDbType.Varchar2).Value = strSDRating;
                    objCmd.Parameters.Add("HCRATING_IN", OracleDbType.Varchar2).Value = strHCRating;
                    objCmd.Parameters.Add("ADRATING_IN", OracleDbType.Varchar2).Value = strADRating;
                    objCmd.Parameters.Add("FEEDBACK_IN", OracleDbType.Varchar2).Value = strFeedBack;
                    objCmd.Parameters.Add("EVALCODE_IN", OracleDbType.Varchar2).Value = strEvalCode;
                    objCmd.Parameters.Add("EVALACHVCOM_IN", OracleDbType.Varchar2).Value = strEvalAchCom;
                    objCmd.Parameters.Add("TWOWAYCOM1_IN", OracleDbType.Varchar2).Value = str2waycom1;
                    objCmd.Parameters.Add("TWOWAYCOM2_IN", OracleDbType.Varchar2).Value = str2waycom2;
                    objCmd.Parameters.Add("TWOWAYCOM3_IN", OracleDbType.Varchar2).Value = str2waycom3;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string SetFYRevRatings(string strRequestid, string strRating, string strComments)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_FYREVRATING_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("COMMENT_IN", OracleDbType.Varchar2).Value = strComments;
                    objCmd.Parameters.Add("RATING_IN", OracleDbType.Int32).Value = strRating;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string SetFYRevTabStatus(string strRequestid)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_FYREVTABSTATUS_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string SetFYRev2Ratings(string strRequestid, string strRating, string strComments)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_FYREV2RATING_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("COMMENT_IN", OracleDbType.Varchar2).Value = strComments;
                    objCmd.Parameters.Add("RATING_IN", OracleDbType.Int32).Value = strRating;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string SetFYRev2TabStatus(string strRequestid)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_FYREV2TABSTATUS_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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


        public string SetFYIndIniReviewer(string strRequestid, string strImprovement, string strSelfDev,
        string strCulture, string strAssociateDev, string strButton, string strImpRating, string strSDRating,
        string strHCRating, string strADRating, string strRevAchCom, string str2waycom1, string str2waycom2, string str2waycom3, string strEcode)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_FYINDINIREVIEWER_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("IMPROVEMENT_IN", OracleDbType.Varchar2).Value = strImprovement;
                    objCmd.Parameters.Add("SELFDEV_IN", OracleDbType.Varchar2).Value = strSelfDev;
                    objCmd.Parameters.Add("CULTURE_IN", OracleDbType.Varchar2).Value = strCulture;
                    objCmd.Parameters.Add("ASSOCIATEDEV_IN", OracleDbType.Varchar2).Value = strAssociateDev;
                    objCmd.Parameters.Add("BUTTONCLICK_IN", OracleDbType.Varchar2).Value = strButton;
                    objCmd.Parameters.Add("IMPRATING_IN", OracleDbType.Varchar2).Value = strImpRating;
                    objCmd.Parameters.Add("SDRATING_IN", OracleDbType.Varchar2).Value = strSDRating;
                    objCmd.Parameters.Add("HCRATING_IN", OracleDbType.Varchar2).Value = strHCRating;
                    objCmd.Parameters.Add("ADRATING_IN", OracleDbType.Varchar2).Value = strADRating;
                    objCmd.Parameters.Add("REVCOMONACHV_IN", OracleDbType.Varchar2).Value = strRevAchCom;
                    objCmd.Parameters.Add("TWOWAYCOM1_IN", OracleDbType.Varchar2).Value = str2waycom1;
                    objCmd.Parameters.Add("TWOWAYCOM2_IN", OracleDbType.Varchar2).Value = str2waycom2;
                    objCmd.Parameters.Add("TWOWAYCOM3_IN", OracleDbType.Varchar2).Value = str2waycom3;
                    objCmd.Parameters.Add("ECODE_IN", OracleDbType.Varchar2).Value = strEcode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string AddActivity(string strRequestid, string strActivity)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_ACTIVITY_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("ACTIVITY_IN", OracleDbType.Varchar2).Value = strActivity;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public DataTable getAppraisalRequest(string strEmpCode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPORC_PMSSTATUSFORAPPROVAL_GET";
                    objCmd.Parameters.Add("CUR_TARGET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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
        /// GET PMS STATUS DATA FOR SELF
        /// </summary>
        /// <param name="strEmpCode"></param>
        /// <param name="strSykiID"></param>
        /// <returns></returns>
        public DataTable getSelfPMSStatus(string strEmpCode, string strSykiID)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPORC_SELFPMSSTATUS_GET";
                    objCmd.Parameters.Add("CUR_SELFPMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("SYKIID_IN", OracleDbType.Int32).Value = strSykiID;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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
        /// GET PMS STATUS DATA FOR EVALUATING EMPLOYEES
        /// </summary>
        /// <param name="strEmpCode"></param>
        /// <param name="strSykiID"></param>
        /// <returns></returns>
        public DataTable getEvalPMSStatus(string strEmpCode, string strSykiID)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPORC_EVALPMSSTATUS_GET";
                    objCmd.Parameters.Add("CUR_EVALPMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("SYKIID_IN", OracleDbType.Int32).Value = strSykiID;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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
        /// GET PMS STATUS DATA FOR REVIEWING EMPLOYEE'S
        /// </summary>
        /// <param name="strEmpCode"></param>
        /// <param name="strSykiID"></param>
        /// <returns></returns>
        public DataTable getReviewPMSStatus(string strEmpCode, string strSykiID)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPORC_REVIEWPMSSTATUS_GET";
                    objCmd.Parameters.Add("CUR_REVIEWPMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("SYKIID_IN", OracleDbType.Int32).Value = strSykiID;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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

        public DataTable getPartBFinalScore(string strTransid)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPROC_PARTBFINALSCORE_GET";
                    objCmd.Parameters.Add("CUR_SCORE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("KRAID_IN", OracleDbType.Int32).Value = strTransid;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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

        public DataTable getPartAFinalScore(string strTransid)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPROC_PARTAFINALSCORE_GET";
                    objCmd.Parameters.Add("CUR_SCORE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("KRAID_IN", OracleDbType.Int32).Value = strTransid;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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

        public DataTable getPMSStatByKiAndECode(string strEmpCode, string strSykiID)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPORC_PMSSTATUS_GETBYKIID";
                    objCmd.Parameters.Add("CUR_STATUS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("SYKIID_IN", OracleDbType.Int32).Value = strSykiID;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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

        public string SetMYComment1(string strRequestid, string strComment, string strEmpCode)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_MYCOMMENT1_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("COMMENT_IN", OracleDbType.Varchar2).Value = strComment;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string SetMYComment2(string strRequestid, string strComment, string strEmpCode)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_MYCOMMENT2_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("COMMENT_IN", OracleDbType.Varchar2).Value = strComment;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string SetFYComment1(string strRequestid, string strComment, string strEmpCode)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_FYCOMMENT1_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("COMMENT_IN", OracleDbType.Varchar2).Value = strComment;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string SetFYComment2(string strRequestid, string strComment, string strEmpCode)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_FYCOMMENT2_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("COMMENT_IN", OracleDbType.Varchar2).Value = strComment;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public DataTable GetPMSUserTimePrd(string strEmpCode, string strKiId)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPORC_PMSUSERTIMEPERIOD_GET";
                    objCmd.Parameters.Add("CUR_TIMEPRD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = strKiId;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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

        public DataTable GetActivityByID(string strTargetId)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPORC_ACTIVITYBYTARGETID_GET";
                    objCmd.Parameters.Add("CUR_ACTIVITY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("TARGETID_IN", OracleDbType.Int32).Value = strTargetId;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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

        public DataTable GetOperaton()
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKJ_Ojt.SPROC_OPERATION_GET";
                    objCmd.Parameters.Add("CUR_OJT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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

        public string GetOperatonId(string strEmpCode, string strki)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_OPERATIONID_GET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("KI_IN", OracleDbType.Int32).Value = strki;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    return Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
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

        public DataTable GetKi()
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKJ_Ojt.SPROC_KILIST_GET";
                    objCmd.Parameters.Add("CUR_OJT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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

        public string GetKIId()
        {
           // ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_CURRRNTKIID_GET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    return Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
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

        public DataTable GetDivision(string strOperationId)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPORC_DIVISION_GET";
                    objCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Int32).Value = strOperationId;
                    objCmd.Parameters.Add("CUR_DIVISION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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

        public DataTable GetDesignation()
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPROC_DESIGNATION_GET";
                    objCmd.Parameters.Add("CUR_DESIGNATION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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

        public DataTable GetDptByDivId(string strDivisionId)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPROC_DEPTBYDIVVID_GET";
                    objCmd.Parameters.Add("DIVISIONID_IN", OracleDbType.Int32).Value = strDivisionId;
                    objCmd.Parameters.Add("CUR_DEPARTMENT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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

        public DataTable GetSect(string strOperationId, string strDivisionId, string strDepartmentId)
        {
           // ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                string strSql;
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    DataTable objDt = new DataTable();
                    objCmd.Connection = objCn;
                    strSql = "SELECT distinct SEC.SECTIONID as ADSECTIONID, SEC.SECTION as DESCRIP ";
                    strSql = strSql + " FROM vw_orgleveldetails SEC where 1=1 ";
                    if (strOperationId != "0")
                        strSql = strSql + " and SEC.OPID = " + strOperationId;
                    if (strDepartmentId != "0")
                        strSql = strSql + " AND SEC.DEPTID = " + strDepartmentId;
                    if (strDivisionId != "0")
                        strSql = strSql + " AND SEC.DIVID    = " + strDivisionId;
                    strSql = strSql + "  ORDER BY SEC.SECTION";
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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
        public DataTable GetSect(string strOperationId, string strDivisionId, string strDepartmentId, string strKI)
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
                    DataTable objDt = new DataTable();
                    objCmd.Connection = objCn;
                    strSql = " SELECT distinct SEC.SECTIONID as ADSECTIONID, SEC.SECTION as DESCRIP ";
                    strSql = strSql + " from vw_orgleveldetails_ki SEC WHERE 1=1 ";
                    if (strOperationId != "0")
                        strSql = strSql + " AND SEC.OPID = " + strOperationId;
                    if (strDepartmentId != "0")
                        strSql = strSql + " AND SEC.DEPTID = " + strDepartmentId;
                    if (strDivisionId != "0")
                        strSql = strSql + " AND SEC.DIVID = " + strDivisionId;
                    strSql = strSql + " AND SEC.SYKIID = " + strKI + " ORDER BY SEC.SECTION";
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.Text;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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

        public DataTable GetOHReport(string strEmpCode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPROC_OHREPORT_GET";
                    objCmd.Parameters.Add("CUR_OH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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

        public DataTable GetPmsRating(string strKiId)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPROC_PMSRATING_GET";
                    objCmd.Parameters.Add("CUR_RATING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("SYKIID_IN", OracleDbType.Int32).Value = strKiId;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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

        public DataTable GetSecHlfOHReport(string strEmpCode, string strKiId, string strDesignationId,
          string strOperationId, string strDivisionId, string strDepartmentId, string strSectionId, string strstatus, string strAssoEmpCode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPROC_SECONDHALFOHREPORT_GET";
                    objCmd.Parameters.Add("CUR_OH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("KIID_IN", OracleDbType.Int32).Value = strKiId;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = strstatus;
                    objCmd.Parameters.Add("DESIGNATIONID_IN", OracleDbType.Varchar2).Value = strDesignationId;
                    if (strOperationId != "0")
                    {
                        objCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Varchar2).Value = strOperationId;
                    }
                    if (strDivisionId != "0")
                    {
                        objCmd.Parameters.Add("DIVISIONID_IN", OracleDbType.Varchar2).Value = strDivisionId;
                    }
                    if (strDepartmentId != "0")
                    {
                        objCmd.Parameters.Add("DEPARTMENTID_IN", OracleDbType.Varchar2).Value = strDepartmentId;
                    }
                    if (strSectionId != "0")
                    {
                        objCmd.Parameters.Add("SECTIONID_IN", OracleDbType.Varchar2).Value = strSectionId;
                    }

                    objCmd.Parameters.Add("ASSOEMPCODE_IN", OracleDbType.Varchar2).Value = strAssoEmpCode;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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

        public DataTable GetFinalRating(string strKiId)
        {
           // ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPROC_PMSFINALRATING_GET";
                    objCmd.Parameters.Add("CUR_RATING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("SYKIID_IN", OracleDbType.Int32).Value = strKiId;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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

        public string SetSHOPRankRating(string strKraId, string strHalfRating, string strEmpCode, string strType)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_SHOPRANKRATING_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("KRAID_IN", OracleDbType.Varchar2).Value = strKraId;
                    objCmd.Parameters.Add("SHRANKRATING_IN", OracleDbType.Varchar2).Value = strHalfRating;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("TYPE_IN", OracleDbType.Int32).Value = strType;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
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
        /// UPDATE RANK   RATING BY OPERATING HEAD AT ONE SAVE BUTTON
        /// </summary>
        /// <param name="strKraId"></param>
        /// <param name="strSHRank"></param>
        /// <param name="strSHRating"></param>
        /// <param name="strFinalRank"></param>
        /// <param name="strFinalRating"></param>
        /// <param name="strPromotion"></param>
        /// <param name="strEmpCode"></param>
        /// <returns></returns>
        public string SetSHOPRankRatingV2(string strKraId, string strSHRank, string strSHRating, string strFinalRank,
                                            string strFinalRating, string strPromotion, string strEmpCode)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_OPHEADSHRANKRATING_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("KRAID_IN", OracleDbType.Varchar2).Value = strKraId;
                    objCmd.Parameters.Add("SHRANK_IN", OracleDbType.Varchar2).Value = strSHRank;
                    objCmd.Parameters.Add("SHRATING_IN", OracleDbType.Varchar2).Value = strSHRating;
                    objCmd.Parameters.Add("FINALRANK_IN", OracleDbType.Varchar2).Value = strFinalRank;
                    objCmd.Parameters.Add("FINALRATING_IN", OracleDbType.Varchar2).Value = strFinalRating;
                    objCmd.Parameters.Add("PROMOTED_IN", OracleDbType.Varchar2).Value = strPromotion;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
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
        /// FUNCTION TO SAVE HF PROMOTION DATA
        /// </summary>
        /// <param name="strKraId"></param>
        /// <param name="VacancyPos"></param>
        /// <param name="PromotionReason"></param>
        /// <param name="Responsbility"></param>
        /// <returns></returns>
        public string SetPromotionData(string strKraId, string VacancyPos, string PromotionReason, string Responsbility)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_OPHEADPROMOTION_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("PROMOTIONID_IN", OracleDbType.Varchar2).Value = strKraId;
                    objCmd.Parameters.Add("VACANCYPOS_IN", OracleDbType.Varchar2).Value = VacancyPos;
                    objCmd.Parameters.Add("PROMOTIONREASON_IN", OracleDbType.Varchar2).Value = PromotionReason;
                    objCmd.Parameters.Add("RESPONSBILITY_IN", OracleDbType.Varchar2).Value = Responsbility;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
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

        public DataTable GetProAssoDtls(string strEmpCode, string strKI)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPROC_PROMOTEDASSODTLS_GET";
                    objCmd.Parameters.Add("CUR_OH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("SYKI_IN", OracleDbType.Int32).Value = strKI;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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

        public string SetFYOHRating(string strKiId, string strEmpCode)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

           // objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_OHFYRATING_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("SYKIID_IN", OracleDbType.Varchar2).Value = strKiId;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
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

        public string GetCountOfAllFilled(string strEmpCode)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_ISALLFILLED_GET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    return Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
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
        /// ASSOCIATE COUNT WHOSE FINAL RATING NOT SUBMIT BY PRESIDENT
        /// </summary>
        /// <param name="strEmpCode"></param>
        /// <returns></returns>
        public string GetCountOfAllFilledByPresident(string strEmpCode)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_PENDINGASSOBYPR_GET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    return Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
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

        public DataTable GetPresidentReport(string strEmpCode, string strKiId, string strDesignationId,
           string strDivisionId, string strDepartmentId, string strOperationId)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPROC_PRESIDENTREPORT_GET";
                    objCmd.Parameters.Add("CUR_OH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("KIID_IN", OracleDbType.Int32).Value = strKiId;
                    objCmd.Parameters.Add("DESIGNATIONID_IN", OracleDbType.Varchar2).Value = strDesignationId;
                    if (strDivisionId != "0")
                    {
                        objCmd.Parameters.Add("DIVISIONID_IN", OracleDbType.Varchar2).Value = strDivisionId;
                    }
                    if (strDepartmentId != "0")
                    {
                        objCmd.Parameters.Add("DEPARTMENTID_IN", OracleDbType.Varchar2).Value = strDepartmentId;
                    }
                    if (strOperationId != "0")
                    {
                        objCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Varchar2).Value = strOperationId;
                    }
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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

        public string SetPresRankRating(string strKraId, string strSHRank, string strSHRating, string strFinalRank,
                                        string strFinalRating, string strPromotion, string strEmpCode)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_PRESRANKRATING_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("KRAID_IN", OracleDbType.Varchar2).Value = strKraId;
                    objCmd.Parameters.Add("SHRANK_IN", OracleDbType.Varchar2).Value = strSHRank;
                    objCmd.Parameters.Add("SHRATING_IN", OracleDbType.Varchar2).Value = strSHRating;
                    objCmd.Parameters.Add("FINALRANK_IN", OracleDbType.Varchar2).Value = strFinalRank;
                    objCmd.Parameters.Add("FINALRATING_IN", OracleDbType.Varchar2).Value = strFinalRating;
                    objCmd.Parameters.Add("PROMOTION_IN", OracleDbType.Varchar2).Value = strPromotion;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
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

        public string FinalSubmitByPresident(string strEmpCode)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_STATUSBYPRESIDENT_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
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

        public DataTable GetFilterDesignation()
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
                    DataTable ds = new DataTable();
                    objCmd.Connection = objCn;
                    strSql = "SELECT A.ADDESIGNATIONID AS ID, A.DESCRIP AS DESIGNATION FROM ADDESIGNATION A";
                    strSql = strSql + " WHERE A.ADDESIGNATIONID IN (SELECT * FROM TABLE(CAST(SPLIT(FN_PARAMVALUE_GET('PMS_GetFilterDesignation')) AS SPLIT_TBL))) AND A.ACTIVE = 1 ";
                    strSql = strSql + " ORDER BY A.DESCRIP";
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

        public DataTable GetHRFinalReport(string strEmpCode, string strKiId, string strDesignationId,
           string strDivisionId, string strDepartmentId, string strSectionId, string strOperationId, string strAssoEmpCode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPROC_HRFINALREPORT_GET";
                    objCmd.Parameters.Add("CUR_HR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("KIID_IN", OracleDbType.Int32).Value = strKiId;
                    objCmd.Parameters.Add("DESIGNATIONID_IN", OracleDbType.Varchar2).Value = strDesignationId;
                    if (strOperationId != "0")
                    {
                        objCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Varchar2).Value = strOperationId;
                    }
                    if (strDivisionId != "0")
                    {
                        objCmd.Parameters.Add("DIVISIONID_IN", OracleDbType.Varchar2).Value = strDivisionId;
                    }
                    if (strDepartmentId != "0")
                    {
                        objCmd.Parameters.Add("DEPARTMENTID_IN", OracleDbType.Varchar2).Value = strDepartmentId;
                    }
                    if (strSectionId != "0")
                    {
                        objCmd.Parameters.Add("SECTIONID_IN", OracleDbType.Varchar2).Value = strSectionId;
                    }
                    if (strAssoEmpCode != "")
                    {
                        objCmd.Parameters.Add("ASSOEMPCODE_IN", OracleDbType.Varchar2).Value = strAssoEmpCode;
                    }
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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

        public string SetFinalRatingByHR(string strKiId, string strRankRating, string strCode, string strType, string strEmpCode)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_HRFINALRANKRATING_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = strKiId;
                    objCmd.Parameters.Add("RANKRATING_IN", OracleDbType.Varchar2).Value = strRankRating;
                    objCmd.Parameters.Add("CODE_IN", OracleDbType.Int32).Value = strCode;
                    objCmd.Parameters.Add("TYPE_IN", OracleDbType.Int32).Value = strType;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
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

        public string GetApprasialStatus(string strKiId)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

           // objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_FINALYEARSTATUS_GET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = strKiId;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                    return strErrMsg;
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

        public string FinalSubmitByHR(string strEmpCode, string strKiId)
        {
           // ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_FINALSTATUSBYHR_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = strKiId;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
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

        public DataTable GetOPMYReport(string strEmpCode, string strKiId, string strDesignationId,
           string strDivisionId, string strDepartmentId, string strSectionId, string strOperationId, string strAssoEmpCode, string strstatus)
        {
           // ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPROC_OPMYREPORT_GET";
                    objCmd.Parameters.Add("CUR_HR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("KIID_IN", OracleDbType.Int32).Value = strKiId;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = strstatus;
                    objCmd.Parameters.Add("DESIGNATIONID_IN", OracleDbType.Varchar2).Value = strDesignationId;
                    if (strOperationId != "0")
                    {
                        objCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Varchar2).Value = strOperationId;
                    }
                    if (strDivisionId != "0")
                    {
                        objCmd.Parameters.Add("DIVISIONID_IN", OracleDbType.Varchar2).Value = strDivisionId;
                    }
                    if (strDepartmentId != "0")
                    {
                        objCmd.Parameters.Add("DEPARTMENTID_IN", OracleDbType.Varchar2).Value = strDepartmentId;
                    }
                    if (strSectionId != "0")
                    {
                        objCmd.Parameters.Add("SECTIONID_IN", OracleDbType.Varchar2).Value = strSectionId;
                    }
                    if (strAssoEmpCode != "")
                    {
                        objCmd.Parameters.Add("ASSOEMPCODE_IN", OracleDbType.Varchar2).Value = strAssoEmpCode;
                    }
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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
        /// GET PMS DATA FOR HR TO UPDATE RARTING
        /// </summary>
        /// <param name="strEmpCode"></param>
        /// <param name="strKiId"></param>
        /// <param name="strDesignationId"></param>
        /// <param name="strDivisionId"></param>
        /// <param name="strDepartmentId"></param>
        /// <param name="strSectionId"></param>
        /// <param name="strOperationId"></param>
        /// <param name="strAssoEmpCode"></param>
        /// <param name="PMSEligibility"></param>
        /// <returns></returns>
        public DataTable GetHRMYReport(string strEmpCode, string strKiId, string strDesignationId,
                                        string strDivisionId, string strDepartmentId, string strSectionId,
                                        string strOperationId, string strAssoEmpCode, string PMSEligibility)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPROC_HRMYREPORT_GET";
                    objCmd.Parameters.Add("CUR_HR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("KIID_IN", OracleDbType.Int32).Value = strKiId;
                    objCmd.Parameters.Add("DESIGNATIONID_IN", OracleDbType.Varchar2).Value = strDesignationId;
                    if (strOperationId != "0")
                    {
                        objCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Varchar2).Value = strOperationId;
                    }
                    if (strDivisionId != "0")
                    {
                        objCmd.Parameters.Add("DIVISIONID_IN", OracleDbType.Varchar2).Value = strDivisionId;
                    }
                    if (strDepartmentId != "0")
                    {
                        objCmd.Parameters.Add("DEPARTMENTID_IN", OracleDbType.Varchar2).Value = strDepartmentId;
                    }
                    if (strSectionId != "0")
                    {
                        objCmd.Parameters.Add("SECTIONID_IN", OracleDbType.Varchar2).Value = strSectionId;
                    }
                    if (PMSEligibility != "0")
                    {
                        objCmd.Parameters.Add("ELIGIBILITY_IN", OracleDbType.Varchar2).Value = PMSEligibility;
                    }
                    if (strAssoEmpCode != "")
                    {
                        objCmd.Parameters.Add("ASSOEMPCODE_IN", OracleDbType.Varchar2).Value = strAssoEmpCode;
                    }
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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
        public DataTable GetHRPMSScore(string strKiId, string strOperationId, string strDivisionId, string strDepartmentId, string strSectionId, string strAssoEmpCode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPROC_PMS_EMPSCORE_GET";
                    objCmd.Parameters.Add("CUR_PMS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("KICODE_IN", OracleDbType.Int32).Value = strKiId;
                    if (strOperationId != "0")
                    {
                        objCmd.Parameters.Add("OPID_IN", OracleDbType.Varchar2).Value = strOperationId;
                    }
                    if (strDivisionId != "0")
                    {
                        objCmd.Parameters.Add("DIVID_IN", OracleDbType.Varchar2).Value = strDivisionId;
                    }
                    if (strDepartmentId != "0")
                    {
                        objCmd.Parameters.Add("DPTID_IN", OracleDbType.Varchar2).Value = strDepartmentId;
                    }
                    if (strSectionId != "0")
                    {
                        objCmd.Parameters.Add("SECID_IN", OracleDbType.Varchar2).Value = strSectionId;
                    }
                    if (strAssoEmpCode != "")
                    {
                        objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strAssoEmpCode;
                    }
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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


        public string SetMYOPRankRating(string strKraId, string strRankRating, string strEmpCode, string strType)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_MYOPRANKRATING_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("KRAID_IN", OracleDbType.Varchar2).Value = strKraId;
                    objCmd.Parameters.Add("RANKRATING_IN", OracleDbType.Varchar2).Value = strRankRating;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("TYPE_IN", OracleDbType.Int32).Value = strType;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
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
        /// SET RANK AND RATING OP HEAD
        /// </summary>
        /// <param name="strKraId"></param>
        /// <param name="strRank"></param>
        /// <param name="strRating"></param>
        /// <param name="strEmpCode"></param>
        /// <param name="strType"></param>
        /// <returns></returns>
        public string SetMYOPRankRatingV2(string strKraId, string strRank, string strRating, string strEmpCode)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

           // objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_MYOPRANKRATING_SETV2";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("KRAID_IN", OracleDbType.Varchar2).Value = strKraId;
                    objCmd.Parameters.Add("RANK_IN", OracleDbType.Varchar2).Value = strRank;
                    objCmd.Parameters.Add("RATING_IN", OracleDbType.Varchar2).Value = strRating;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
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
        /// UPDATE MID YEAR RATING BY HR
        /// </summary>
        /// <param name="strRating"></param>
        /// <param name="strEmpCode"></param>
        /// <param name="strKIID"></param>
        /// <param name="strUserID"></param>
        /// <returns></returns>
        public string SetMYHRRating(string strRating, string strEmpCode, string strKIID, string strUserID)
        {
           // ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

           // objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_MYHRRATING_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("RATING_IN", OracleDbType.Varchar2).Value = strRating;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                    objCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = strKIID;
                    objCmd.Parameters.Add("USERID_IN", OracleDbType.Varchar2).Value = strUserID;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
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
        /// GET ASSOCIATE COUNT WHOSE PMS STATUS NOT REACH AT OPHEAD 
        /// </summary>
        /// <param name="strKiId"></param>
        /// <param name="strEmpCode"></param>
        /// <param name="strPMSPeriod"></param>
        /// <returns></returns>
        public string GetMYApprasialStatus(string strKiId, string strEmpCode, string strPMSPeriod)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_MIDYEARSTATUS_GET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = strKiId;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                    objCmd.Parameters.Add("PMSPERIOD_IN", OracleDbType.Varchar2).Value = strPMSPeriod;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                    return strErrMsg;
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
        /// GET ASSOCIATE COUNT WHOSE PMS STATUS NOT REACH AT PRESIDENT 
        /// </summary>
        /// <param name="strKiId"></param>
        /// <param name="strEmpCode"></param>
        /// <returns></returns>
        public string GetAssociatePendingForPresidentRating(string strKiId, string strEmpCode)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_PENDINGASSOFORPR_GET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = strKiId;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                    return strErrMsg;
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
        /// GET ASSOCIATE COUNT WHOSE PMS STATUS NOT REACH AT HR 
        /// </summary>
        /// <param name="strKiId"></param>
        /// <param name="strPMSPeriod"></param>
        /// <returns></returns>
        public string GetAssociateNotReadyForHRRating(string strKiId, string strPMSPeriod)//, string str_CheckEmpCode)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_HRPENDIGASSOCIATE_GET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = strKiId;
                    objCmd.Parameters.Add("PMSPERIOD_IN", OracleDbType.Varchar2).Value = strPMSPeriod;
                    //objCmd.Parameters.Add("CHECKEMPCODE_IN", OracleDbType.Varchar2).Value = str_CheckEmpCode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                    return strErrMsg;
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
        /// GET ASSOCIATE COUNT WHOSE PMS RATING NOT SET BY HR 
        /// </summary>
        /// <param name="strKiId"></param>
        /// <param name="strPMSPeriod"></param>
        /// <returns></returns>
        public string GetPendingHRRating(string strKiId, string strPMSPeriod)//, string str_CheckEmpCode)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_HRPENDIGRATING_GET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = strKiId;
                    objCmd.Parameters.Add("PMSPERIOD_IN", OracleDbType.Varchar2).Value = strPMSPeriod;
                    //objCmd.Parameters.Add("CHECKEMPCODE_IN", OracleDbType.Varchar2).Value = str_CheckEmpCode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                    return strErrMsg;
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
        /// CHECK MID YEAR RATING FILLED BY OPERATING HEAD OR NOT
        /// </summary>
        /// <param name="strKiId"></param>
        /// <param name="strEmpCode"></param>
        /// <returns></returns>
        public string GetMYApprasialRating(string strKiId, string strEmpCode)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

           // objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_MIDYEARRATING_GET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = strKiId;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                    return strErrMsg;
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

        public string MYFinalSubmissionByOP(string strEmpCode, string strKiId)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_MYSTATUSBYOP_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = strKiId;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
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

        public string MYFinalSubmissionByHR(string strEmpCode, string strKiId)//, string str_CheckEmpCode)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_MYSTATUSBYHR_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = strKiId;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    //objCmd.Parameters.Add("CHECKEMPCODE_IN", OracleDbType.Int32).Value = str_CheckEmpCode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
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
        /// GET ALL PMS DATA FOR ASSOCIATE
        /// </summary>
        /// <param name="strEmpCode"></param>
        /// <param name="strKiId"></param>
        /// <param name="strDesignationId"></param>
        /// <param name="strDivisionId"></param>
        /// <param name="strDepartmentId"></param>
        /// <param name="strSectionId"></param>
        /// <param name="strOperationId"></param>
        /// <param name="strAssoEmpCode"></param>
        /// <param name="strAssoEmpName"></param>
        /// <returns></returns>
        public DataTable GetHROverAllReport(string strEmpCode, string strKiId, string strDesignationId,
                                            string strDivisionId, string strDepartmentId, string strSectionId, string strOperationId,
                                            string strAssoEmpCode, string strAssoEmpName, string strEligibilityStatus)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPROC_HROVERALLREPORT_GET";
                    objCmd.Parameters.Add("CUR_HR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                    objCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = strKiId;
                    objCmd.Parameters.Add("DESIGNATIONID_IN", OracleDbType.Varchar2).Value = strDesignationId;
                    objCmd.Parameters.Add("ELIGIBILITYSTATUS_IN", OracleDbType.Varchar2).Value = strEligibilityStatus;
                    if (strOperationId != "0")
                    {
                        objCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Varchar2).Value = strOperationId;
                    }
                    if (strDivisionId != "0")
                    {
                        objCmd.Parameters.Add("DIVISIONID_IN", OracleDbType.Varchar2).Value = strDivisionId;
                    }
                    if (strDepartmentId != "0")
                    {
                        objCmd.Parameters.Add("DEPARTMENTID_IN", OracleDbType.Varchar2).Value = strDepartmentId;
                    }
                    if (strSectionId != "0")
                    {
                        objCmd.Parameters.Add("SECTIONID_IN", OracleDbType.Varchar2).Value = strSectionId;
                    }
                    if (strAssoEmpCode != "")
                    {
                        objCmd.Parameters.Add("ASSOEMPCODE_IN", OracleDbType.Varchar2).Value = strAssoEmpCode;
                    }
                    if (strAssoEmpName != "")
                    {
                        objCmd.Parameters.Add("ASSOEMPNAME_IN", OracleDbType.Varchar2).Value = strAssoEmpName;
                    }
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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
        /// UPLOAD PMS DATA FOR ASSOCIATE
        /// </summary>
        /// <param name="strEmpCode"></param>
        /// <param name="KiID"></param>
        /// <param name="AssociateID"></param>
        /// <param name="EvaluatorID"></param>
        /// <param name="ReviewerID"></param>
        /// <param name="Comment1ID"></param>
        /// <param name="OPHeadID"></param>
        /// <param name="PresentID"></param>
        /// <param name="GSStartDate"></param>
        /// <param name="GSEndDate"></param>
        /// <param name="FHStartDate"></param>
        /// <param name="FHEndDate"></param>
        /// <param name="SHStartDate"></param>
        /// <param name="SHEndDate"></param>
        /// <param name="EligilbeStatus"></param>
        /// <param name="EvalGSStartDate"></param>
        /// <param name="EvalGSEndDate"></param>
        /// <param name="RevGSStartDate"></param>
        /// <param name="RevGSEndDate"></param>
        /// <param name="EvalFHStartDate"></param>
        /// <param name="EvalFHEndDate"></param>
        /// <param name="RevFHStartDate"></param>
        /// <param name="RevFHEndDate"></param>
        /// <param name="Comment1FHStartDate"></param>
        /// <param name="Comment1FHEndDate"></param>
        /// <param name="OPHeadFHStartDate"></param>
        /// <param name="OPHeadFHEndDate"></param>
        /// <param name="EvalSHStartDate"></param>
        /// <param name="EvalSHEndDate"></param>
        /// <param name="RevSHStartDate"></param>
        /// <param name="RevSHEndDate"></param>
        /// <param name="Comment1SHStartDate"></param>
        /// <param name="Comment1SHEndDate"></param>
        /// <param name="OPHeadSHStartDate"></param>
        /// <param name="OPHeadSHEndDate"></param>
        /// <returns></returns>
        public string UploadFile(string strEmpCode, string KiID, string AssociateID, string EvaluatorID, string ReviewerID,
                                    string Comment1ID, string OPHeadID, string PresentID, string GSStartDate, string GSEndDate,
                                    string FHStartDate, string FHEndDate, string SHStartDate, string SHEndDate, string EligilbeStatus,
                                    string EvalGSStartDate, string EvalGSEndDate, string RevGSStartDate, string RevGSEndDate,
                                    string EvalFHStartDate, string EvalFHEndDate, string RevFHStartDate, string RevFHEndDate,
                                    string Comment1FHStartDate, string Comment1FHEndDate, string OPHeadFHStartDate, string OPHeadFHEndDate,
                                    string EvalSHStartDate, string EvalSHEndDate, string RevSHStartDate, string RevSHEndDate,
                                    string Comment1SHStartDate, string Comment1SHEndDate, string OPHeadSHStartDate, string OPHeadSHEndDate,
                                    string Evaluator2ID, string Reviewer2ID, string Eval2GSStartDate, string Eval2GSEndDate, string Rev2GSStartDate, string Rev2GSEndDate,
                                    string Eval2FHStartDate, string Eval2FHEndDate, string Rev2FHStartDate, string Rev2FHEndDate,
                                    string Eval2SHStartDate, string Eval2SHEndDate, string Rev2SHStartDate, string Rev2SHEndDate)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_UPLOADFILE_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                    objCmd.Parameters.Add("ASSOEMPCODE_IN", OracleDbType.Varchar2).Value = AssociateID;
                    objCmd.Parameters.Add("SYKIID_IN", OracleDbType.Varchar2).Value = KiID;
                    objCmd.Parameters.Add("EVALCODE_IN", OracleDbType.Varchar2).Value = EvaluatorID;
                    objCmd.Parameters.Add("EVAL2CODE_IN", OracleDbType.Varchar2).Value = Evaluator2ID;
                    objCmd.Parameters.Add("REVCODE_IN", OracleDbType.Varchar2).Value = ReviewerID;
                    objCmd.Parameters.Add("REV2CODE_IN", OracleDbType.Varchar2).Value = Reviewer2ID;
                    objCmd.Parameters.Add("COM1CODE_IN", OracleDbType.Varchar2).Value = Comment1ID;
                    objCmd.Parameters.Add("OHCODE_IN", OracleDbType.Varchar2).Value = OPHeadID;
                    objCmd.Parameters.Add("PRECODE_IN", OracleDbType.Varchar2).Value = PresentID;
                    objCmd.Parameters.Add("GSSTARTDATE_IN", OracleDbType.Varchar2).Value = GSStartDate;
                    objCmd.Parameters.Add("GSENDDATE_IN", OracleDbType.Varchar2).Value = GSEndDate;
                    objCmd.Parameters.Add("FHSTARTDATE_IN", OracleDbType.Varchar2).Value = FHStartDate;
                    objCmd.Parameters.Add("FHENDDATE_IN", OracleDbType.Varchar2).Value = FHEndDate;
                    objCmd.Parameters.Add("SHSTARTDATE_IN", OracleDbType.Varchar2).Value = SHStartDate;
                    objCmd.Parameters.Add("SHENDDATE_IN", OracleDbType.Varchar2).Value = SHEndDate;
                    objCmd.Parameters.Add("ELIGIBLESTATUS_IN", OracleDbType.Varchar2).Value = EligilbeStatus;

                    //GS EVALUATOR/REVIEWER
                    objCmd.Parameters.Add("EVALGSSTARTDATE_IN", OracleDbType.Varchar2).Value = EvalGSStartDate;
                    objCmd.Parameters.Add("EVALGSENDDATE_IN", OracleDbType.Varchar2).Value = EvalGSEndDate;
                    objCmd.Parameters.Add("REVGSSTARTDATE_IN", OracleDbType.Varchar2).Value = RevGSStartDate;
                    objCmd.Parameters.Add("REVGSENDDATE_IN", OracleDbType.Varchar2).Value = RevGSEndDate;
                    //------------------------------------------------------------------------------------
                    objCmd.Parameters.Add("EVAL2GSSTARTDATE_IN", OracleDbType.Varchar2).Value = Eval2GSStartDate;
                    objCmd.Parameters.Add("EVAL2GSENDDATE_IN", OracleDbType.Varchar2).Value = Eval2GSEndDate;
                    objCmd.Parameters.Add("REV2GSSTARTDATE_IN", OracleDbType.Varchar2).Value = Rev2GSStartDate;
                    objCmd.Parameters.Add("REV2GSENDDATE_IN", OracleDbType.Varchar2).Value = Rev2GSEndDate;
                    //------------------------------------------------------------------------------------

                    //FH EVALUATOR/REVIEWER/COMMENT1/OPHEAD
                    objCmd.Parameters.Add("EVALFHSTARTDATE_IN", OracleDbType.Varchar2).Value = EvalFHStartDate;
                    objCmd.Parameters.Add("EVALFHENDDATE_IN", OracleDbType.Varchar2).Value = EvalFHEndDate;
                    objCmd.Parameters.Add("REVFHSTARTDATE_IN", OracleDbType.Varchar2).Value = RevFHStartDate;
                    objCmd.Parameters.Add("REVFHENDDATE_IN", OracleDbType.Varchar2).Value = RevFHEndDate;
                    //-----------------------------------------------------------------------------------
                    objCmd.Parameters.Add("EVAL2FHSTARTDATE_IN", OracleDbType.Varchar2).Value = Eval2FHStartDate;
                    objCmd.Parameters.Add("EVAL2FHENDDATE_IN", OracleDbType.Varchar2).Value = Eval2FHEndDate;
                    objCmd.Parameters.Add("REV2FHSTARTDATE_IN", OracleDbType.Varchar2).Value = Rev2FHStartDate;
                    objCmd.Parameters.Add("REV2FHENDDATE_IN", OracleDbType.Varchar2).Value = Rev2FHEndDate;
                    //--------------------------------------------------------------------------------------
                    objCmd.Parameters.Add("COMM1FHSTARTDATE_IN", OracleDbType.Varchar2).Value = Comment1FHStartDate;
                    objCmd.Parameters.Add("COMM1FHENDDATE_IN", OracleDbType.Varchar2).Value = Comment1FHEndDate;
                    objCmd.Parameters.Add("OPHEADFHSTARTDATE_IN", OracleDbType.Varchar2).Value = OPHeadFHStartDate;
                    objCmd.Parameters.Add("OPHEADFHENDDATE_IN", OracleDbType.Varchar2).Value = OPHeadFHEndDate;

                    //SH EVALUATOR/REVIEWER/COMMENT1/OPHEAD
                    objCmd.Parameters.Add("EVALSHSTARTDATE_IN", OracleDbType.Varchar2).Value = EvalSHStartDate;
                    objCmd.Parameters.Add("EVALSHENDDATE_IN", OracleDbType.Varchar2).Value = EvalSHEndDate;
                    objCmd.Parameters.Add("REVSHSTARTDATE_IN", OracleDbType.Varchar2).Value = RevSHStartDate;
                    objCmd.Parameters.Add("REVSHENDDATE_IN", OracleDbType.Varchar2).Value = RevSHEndDate;
                    //------------------------------------
                    objCmd.Parameters.Add("EVAL2SHSTARTDATE_IN", OracleDbType.Varchar2).Value = Eval2SHStartDate;
                    objCmd.Parameters.Add("EVAL2SHENDDATE_IN", OracleDbType.Varchar2).Value = Eval2SHEndDate;
                    objCmd.Parameters.Add("REV2SHSTARTDATE_IN", OracleDbType.Varchar2).Value = Rev2SHStartDate;
                    objCmd.Parameters.Add("REV2SHENDDATE_IN", OracleDbType.Varchar2).Value = Rev2SHEndDate;
                    //----------------------------------------------------------
                    objCmd.Parameters.Add("COMM1SHSTARTDATE_IN", OracleDbType.Varchar2).Value = Comment1SHStartDate;
                    objCmd.Parameters.Add("COMM1SHENDDATE_IN", OracleDbType.Varchar2).Value = Comment1SHEndDate;
                    objCmd.Parameters.Add("OPHEADSHSTARTDATE_IN", OracleDbType.Varchar2).Value = OPHeadSHStartDate;
                    objCmd.Parameters.Add("OPHEADSHENDDATE_IN", OracleDbType.Varchar2).Value = OPHeadSHEndDate;

                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }
            return strErrMsg;
        }

        public DataTable GetKiList()
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPROC_KILIST_GET";
                    objCmd.Parameters.Add("CUR_KI", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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
        /// GET PMS APPROVAL AUTHORITIES FOR ASSOCIATE AGAINST MATRIX SET BY HR
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="KIID"></param>
        /// <returns></returns>
        public DataTable GetPMSAuthoritiesMatrix(string userID, string KIID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            //DataManagement oDataMgmt = new DataManagement();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_HRPMS.SPROC_PMSAUTHORITYMATRIX_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = userID;
            oCmd.Parameters.Add("SYKIID_IN", OracleDbType.Varchar2).Value = KIID;
            oCmd.Parameters.Add("CUR_APPAUTH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET PMS APPROVAL AUTHORITIES FOR ASSOCIATE FROM KRAUTHORITY TABLE KI WISE
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="SYKI"></param>
        /// <returns></returns>
        public DataTable GetPMSAuthorities(string userID, string SYKI)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            //DataManagement oDataMgmt = new DataManagement();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_HRPMS.SPROC_PMSAUTHORITY_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = userID;
            oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = SYKI;
            oCmd.Parameters.Add("CUR_APPAUTH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET PMS SCHEDULE ACTIVITY BY HR
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="SYKI"></param>
        /// <param name="GSStartDate"></param>
        /// <param name="GSEndDate"></param>
        /// <param name="FHStartDate"></param>
        /// <param name="FHEndDate"></param>
        /// <param name="SHStartDate"></param>
        /// <param name="SHEndDate"></param>
        /// <returns></returns>
        public string GetScheduleActivity(string UserID, string SYKI, string GSStartDate, string GSEndDate, string FHStartDate, string FHEndDate,
                                           string SHStartDate, string SHEndDate)
        {

            string PMSActivity = string.Empty;
            OracleCommand oCmd = new OracleCommand();
            //DataManagement oDataMgmt = new DataManagement();

            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_HRPMS.SPROC_PMSACTIVITY_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = UserID;
            oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = SYKI;
            oCmd.Parameters.Add("GSSTARTDATE_IN", OracleDbType.Varchar2).Value = GSStartDate;
            oCmd.Parameters.Add("GSENDDATE_IN", OracleDbType.Varchar2).Value = GSEndDate;
            oCmd.Parameters.Add("FHSTARTDATE_IN", OracleDbType.Varchar2).Value = FHStartDate;
            oCmd.Parameters.Add("FHENDDATE_IN", OracleDbType.Varchar2).Value = FHEndDate;
            oCmd.Parameters.Add("SHSTARTDATE_IN", OracleDbType.Varchar2).Value = SHStartDate;
            oCmd.Parameters.Add("SHENDDATE_IN", OracleDbType.Varchar2).Value = SHEndDate;
            oCmd.Parameters.Add("GS_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("FH_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("SH_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //EXECUTE ORACLE COMMAND
            oDataMgmt.ExecuteQuery(oCmd);

            //GET VALUE FROM OUTPUT PARAMETER
            string isGSChanged = string.Empty;
            string isFHChanged = string.Empty;
            string isSHChanged = string.Empty;

            //IF CHANGED VLAUE=1 ELSE VALUE=0
            isGSChanged = Convert.ToString(oCmd.Parameters["GS_OUT"].Value);
            isFHChanged = Convert.ToString(oCmd.Parameters["FH_OUT"].Value);
            isSHChanged = Convert.ToString(oCmd.Parameters["SH_OUT"].Value);

            //GS AND FH CHAGED
            if (isGSChanged == "1" && isFHChanged == "1")
                PMSActivity = "GSFH";
            //GS AND SH CHAGED
            else if (isGSChanged == "1" && isSHChanged == "1")
                PMSActivity = "GSSH";
            else if (isGSChanged == "1")
                PMSActivity = "GS";
            else if (isFHChanged == "1")
                PMSActivity = "FH";
            else if (isSHChanged == "1")
                PMSActivity = "SH";

            return (PMSActivity.Trim());
        }

        /// <summary>
        /// CHECK PMS AUTHORITY IS CHANGED OR NOT
        /// </summary>
        /// <param name="AssociateID"></param>
        /// <param name="sykiid"></param>
        /// <param name="EvaluatorID"></param>
        /// <param name="ReviewerID"></param>
        /// <param name="Comment1ID"></param>
        /// <param name="OPHeadID"></param>
        /// <returns></returns>
        public Boolean isAuthorityChanged(string AssociateID, string sykiid, string EvaluatorID,
                                            string ReviewerID, string Comment1ID, string OPHeadID)
        {

            Boolean AuthChanged = false;
            OracleCommand oCmd = new OracleCommand();
            //DataManagement oDataMgmt = new DataManagement();

            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_HRPMS.SPROC_PMSAUTHORITY_CHANGE";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = AssociateID;
            oCmd.Parameters.Add("SYKI_IN", OracleDbType.Varchar2).Value = sykiid;
            oCmd.Parameters.Add("EVALUATOR_IN", OracleDbType.Varchar2).Value = EvaluatorID;
            oCmd.Parameters.Add("REVIEWER_IN", OracleDbType.Varchar2).Value = ReviewerID;
            oCmd.Parameters.Add("COMMENT1_IN", OracleDbType.Varchar2).Value = Comment1ID;
            oCmd.Parameters.Add("OPHEAD_IN", OracleDbType.Varchar2).Value = OPHeadID;
            oCmd.Parameters.Add("AUTHCHANGE_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            //EXECUTE ORACLE COMMAND
            oDataMgmt.ExecuteQuery(oCmd);

            //GET VALUE FROM OUTPUT PARAMETER
            string isAuthChanged = string.Empty;

            //IF CHANGED VLAUE=0 ELSE VALUE=1
            isAuthChanged = Convert.ToString(oCmd.Parameters["AUTHCHANGE_OUT"].Value);

            //IF AUTHORITY CHANGED
            if (isAuthChanged == "0")
                AuthChanged = true;

            return (AuthChanged);
        }

        //public string SetGSAsso2waycom(string strRequestid, string strIscommunicated, string str2waycomments, string strEmpcode)
        //{
        //    ConnectionString objCnStr;
        //    string strCn;
        //    OracleCommand objCmd;
        //    string strErrMsg;

        //    objCnStr = new ConnectionString();
        //    strCn = objCnStr.getConnectingString();

        //    using (OracleConnection objCn = new OracleConnection())
        //    {
        //        objCn.ConnectionString = strCn;
        //        try
        //        {
        //            objCn.Open();
        //            objCmd = new OracleCommand();
        //            objCmd.Connection = objCn;
        //            objCmd.CommandText = "PKG_HRPMS.SPROC_GSASSO2WAYCOM_SET";
        //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;

        //            objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
        //            objCmd.Parameters.Add("ISCOMMUNICATED_IN", OracleDbType.Varchar2).Value = strIscommunicated;
        //            objCmd.Parameters.Add("TWOWAYCOM_IN", OracleDbType.Varchar2).Value = str2waycomments;
        //            objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
        //            objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
        //            objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;

        //            objCmd.ExecuteNonQuery();
        //            strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
        //            return strErrMsg;

        //        }
        //        finally
        //        {
        //            if (objCn != null)
        //            {
        //                objCn.Close();
        //            }
        //        }
        //    }
        //}

        public string SetGSAsso2waycom(string strRequestid, string strIscommunicated,
            string str2waycomments1, string str2waycomments2, string str2waycomments3, string strEmpcode)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_GSASSO2WAYCOM_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("ISCOMMUNICATED_IN", OracleDbType.Varchar2).Value = strIscommunicated;
                    objCmd.Parameters.Add("TWOWAYCOM_IN1", OracleDbType.Varchar2).Value = str2waycomments1;
                    objCmd.Parameters.Add("TWOWAYCOM_IN2", OracleDbType.Varchar2).Value = str2waycomments2;
                    objCmd.Parameters.Add("TWOWAYCOM_IN3", OracleDbType.Varchar2).Value = str2waycomments3;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        //public string SetMYAsso2waycom(string strRequestid, string strIscommunicated, string str2waycomments, string strEmpcode)
        //{
        //    ConnectionString objCnStr;
        //    string strCn;
        //    OracleCommand objCmd;
        //    string strErrMsg;

        //    objCnStr = new ConnectionString();
        //    strCn = objCnStr.getConnectingString();

        //    using (OracleConnection objCn = new OracleConnection())
        //    {
        //        objCn.ConnectionString = strCn;
        //        try
        //        {
        //            objCn.Open();
        //            objCmd = new OracleCommand();
        //            objCmd.Connection = objCn;
        //            objCmd.CommandText = "PKG_HRPMS.SPROC_MYASSO2WAYCOM_SET";
        //            objCmd.CommandType = System.Data.CommandType.StoredProcedure;

        //            objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
        //            objCmd.Parameters.Add("ISCOMMUNICATED_IN", OracleDbType.Varchar2).Value = strIscommunicated;
        //            objCmd.Parameters.Add("TWOWAYCOM_IN", OracleDbType.Varchar2).Value = str2waycomments;
        //            objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
        //            objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
        //            objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;

        //            objCmd.ExecuteNonQuery();
        //            strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
        //            return strErrMsg;

        //        }
        //        finally
        //        {
        //            if (objCn != null)
        //            {
        //                objCn.Close();
        //            }
        //        }
        //    }
        //}

        public string SetMYAsso2waycom(string strRequestid, string strIscommunicated,
            string str2waycomments1, string str2waycomments2, string str2waycomments3, string strEmpcode)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_MYASSO2WAYCOM_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("ISCOMMUNICATED_IN", OracleDbType.Varchar2).Value = strIscommunicated;
                    objCmd.Parameters.Add("TWOWAYCOM_IN1", OracleDbType.Varchar2).Value = str2waycomments1;
                    objCmd.Parameters.Add("TWOWAYCOM_IN2", OracleDbType.Varchar2).Value = str2waycomments2;
                    objCmd.Parameters.Add("TWOWAYCOM_IN3", OracleDbType.Varchar2).Value = str2waycomments3;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public string SetFYAsso2waycom(string strRequestid, string strIscommunicated, string str2waycomments1, string str2waycomments2, string str2waycomments3, string strEmpcode)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPROC_FYASSO2WAYCOM_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Int32).Value = strRequestid;
                    objCmd.Parameters.Add("ISCOMMUNICATED_IN", OracleDbType.Varchar2).Value = strIscommunicated;
                    objCmd.Parameters.Add("TWOWAYCOM_IN1", OracleDbType.Varchar2).Value = str2waycomments1;
                    objCmd.Parameters.Add("TWOWAYCOM_IN2", OracleDbType.Varchar2).Value = str2waycomments2;
                    objCmd.Parameters.Add("TWOWAYCOM_IN3", OracleDbType.Varchar2).Value = str2waycomments3;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;

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

        public DataTable GetActivitiesOfTarget(string strTargetId)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPORC_ACTIVITIESOFTARGET_GET";
                    objCmd.Parameters.Add("CUR_TARGET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("TARGETID_IN", OracleDbType.Int32).Value = strTargetId;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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

        public DataTable GetTarget1(string strId)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPORC_TARGET_GET1";
                    objCmd.Parameters.Add("CUR_TARGET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("KRAID_IN", OracleDbType.Int32).Value = strId;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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
        /// SET FINAL RATING BY HR NOW ALL PART UPDATED
        /// </summary>
        /// <param name="strKiId"></param>
        /// <param name="strSHRating"></param>
        /// <param name="strFinalRating"></param>
        /// <param name="strPromoted"></param>
        /// <param name="strEmpCode"></param>
        /// <param name="strUserID"></param>
        /// <returns></returns>
        public string SetFinalRatingByHRV2(string strKiId, string strSHRating, string strFinalRating, string strPromoted,
                                            string strEmpCode, string strUserID)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

           // objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_HRFINALRATING_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = strKiId;
                    objCmd.Parameters.Add("SHRATING_IN", OracleDbType.Varchar2).Value = strSHRating;
                    objCmd.Parameters.Add("FINALRATING_IN", OracleDbType.Varchar2).Value = strFinalRating;
                    objCmd.Parameters.Add("PROMOTED_IN", OracleDbType.Varchar2).Value = strPromoted;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strEmpCode;
                    objCmd.Parameters.Add("USERID_IN", OracleDbType.Varchar2).Value = strUserID;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
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

        public string SetMYOPRankRatingbyHR(string strEmpcode, string strRank, string strRating, string strsyki, string strAddedby)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_MYOPRANKRATING_BYHR_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
                    objCmd.Parameters.Add("RANK_IN", OracleDbType.Varchar2).Value = strRank;
                    objCmd.Parameters.Add("RATING_IN", OracleDbType.Varchar2).Value = strRating;
                    objCmd.Parameters.Add("SYKIID_IN", OracleDbType.Varchar2).Value = strsyki;
                    objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = strAddedby;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
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

        public string SetSHOPRankRatingByHR(string strEmpcode, string strkiId, string strSHRank, string strSHRating, string strFinalRank,
                                           string strFinalRating, string strPromotion, string strAddedby)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_OPSHRANKRATING_BYHR_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
                    objCmd.Parameters.Add("SYKIID_IN", OracleDbType.Varchar2).Value = strkiId;
                    objCmd.Parameters.Add("SHRANK_IN", OracleDbType.Varchar2).Value = strSHRank;
                    objCmd.Parameters.Add("SHRATING_IN", OracleDbType.Varchar2).Value = strSHRating;
                    objCmd.Parameters.Add("FINALRANK_IN", OracleDbType.Varchar2).Value = strFinalRank;
                    objCmd.Parameters.Add("FINALRATING_IN", OracleDbType.Varchar2).Value = strFinalRating;
                    objCmd.Parameters.Add("PROMOTED_IN", OracleDbType.Varchar2).Value = strPromotion;
                    objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32).Value = strAddedby;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
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

        public DataTable getManagePMS(string strEmpCode, string strSykiID, string strOperationId, string strDivisionId, string strDepartmentId, string strSectionId)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPORC_MANAGEPMS_GETBYKIID";
                    objCmd.Parameters.Add("CUR_STATUS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("SYKIID_IN", OracleDbType.Int32).Value = strSykiID;
                    if (strOperationId != "0")
                    {
                        objCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Varchar2).Value = strOperationId;
                    }
                    if (strDivisionId != "0")
                    {
                        objCmd.Parameters.Add("DIVISIONID_IN", OracleDbType.Varchar2).Value = strDivisionId;
                    }
                    if (strDepartmentId != "0")
                    {
                        objCmd.Parameters.Add("DEPARTMENTID_IN", OracleDbType.Varchar2).Value = strDepartmentId;
                    }
                    if (strSectionId != "0")
                    {
                        objCmd.Parameters.Add("SECTIONID_IN", OracleDbType.Varchar2).Value = strSectionId;
                    }

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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
        public string MYFinalSubmissionOP_BYHR(string strKraId)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_MYSTATUSBYOP_HR_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("KRAID_IN", OracleDbType.Varchar2).Value = strKraId;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
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
        public string SHFinalSubmissionOP_BYHR(string strKraId)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_SHSTATUSBYOP_HR_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("KRAID_IN", OracleDbType.Varchar2).Value = strKraId;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
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

        public DataTable GetTOWWAYCOMMUNICATION(string KIID, string OpCode, string DivCode, string DeptCode, string SecCode, string EmpCode)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            DataTable objDt = new DataTable();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRPMS.SPROC_TOWWAYCOMMUNICATION_GET";
                    objCmd.Parameters.Add("CUR_HR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("KIID_IN", OracleDbType.Int32).Value = KIID;
                    if (OpCode != "0")
                    {
                        objCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Varchar2).Value = OpCode;
                    }
                    if (DivCode != "0")
                    {
                        objCmd.Parameters.Add("DIVISIONID_IN", OracleDbType.Varchar2).Value = DivCode;
                    }
                    if (DeptCode != "0")
                    {
                        objCmd.Parameters.Add("DEPARTMENTID_IN", OracleDbType.Varchar2).Value = DeptCode;
                    }
                    if (SecCode != "0")
                    {
                        objCmd.Parameters.Add("SECTIONID_IN", OracleDbType.Varchar2).Value = SecCode;
                    }
                    if (EmpCode != "0")
                    {
                        objCmd.Parameters.Add("ASSOEMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
                    }

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                    objAdr.Fill(objDt);
                    return objDt;
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

        public string SetMYOPRatingUpload(string strEmpcode, string strRank, string strRating, string strsyki, string strAddedby)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_MYOPRANKRATING_UPLOAD";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
                    objCmd.Parameters.Add("RANK_IN", OracleDbType.Varchar2).Value = strRank;
                    objCmd.Parameters.Add("RATING_IN", OracleDbType.Varchar2).Value = strRating;
                    objCmd.Parameters.Add("SYKIID_IN", OracleDbType.Varchar2).Value = strsyki;
                    objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = strAddedby;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
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

        public string SetSHOPRatingUpload(string strEmpcode, string strkiId, string strSHRank, string strSHRating, string strFinalRank,
                                            string strFinalRating, string strPromotion, string strAddedby)
        {
           // ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_OPSHRANKRATING_UPLOAD";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
                    objCmd.Parameters.Add("SYKIID_IN", OracleDbType.Varchar2).Value = strkiId;
                    objCmd.Parameters.Add("SHRANK_IN", OracleDbType.Varchar2).Value = strSHRank;
                    objCmd.Parameters.Add("SHRATING_IN", OracleDbType.Varchar2).Value = strSHRating;
                    objCmd.Parameters.Add("FINALRANK_IN", OracleDbType.Varchar2).Value = strFinalRank;
                    objCmd.Parameters.Add("FINALRATING_IN", OracleDbType.Varchar2).Value = strFinalRating;
                    objCmd.Parameters.Add("PROMOTED_IN", OracleDbType.Varchar2).Value = strPromotion;
                    objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32).Value = strAddedby;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
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

        public string MYSTATUSSubmissionByHR(string strkraId)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_MYSTATUSSUBMITBYHR_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("KRAID_IN", OracleDbType.Varchar2).Value = strkraId;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
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

        #region "Version2"
        //FUNCTION TO UPDATE PMS MASTERS WRITTEN BY SANTOSH

        /// <summary>
        /// UPDATE PMS AUTHROTIES FOR ASSOCIATE FOR GIVEN KI
        /// </summary>
        /// <param name="AssociateID"></param>
        /// <param name="KIID"></param>
        /// <param name="EmpZone"></param>
        /// <param name="EvaluatorID"></param>
        /// <param name="ReviewerID"></param>
        /// <param name="Comment1ID"></param>
        /// <param name="OPHeadID"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public string UpdatePMSAuthorities(string AssociateID, string KIID, string EmpZone, string EvaluatorID, string Evaluator2ID, string ReviewerID, string Reviewer2ID,
                                                   string Comment1ID, string OPHeadID, string UserID)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_PMSAUTHORITIES_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("USERID_IN", OracleDbType.Varchar2).Value = AssociateID;
                    objCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = KIID;
                    objCmd.Parameters.Add("EMPZONE_IN", OracleDbType.Varchar2).Value = EmpZone;
                    objCmd.Parameters.Add("EVALUATORID_IN", OracleDbType.Varchar2).Value = EvaluatorID;
                    objCmd.Parameters.Add("EVALUATOR2ID_IN", OracleDbType.Varchar2).Value = Evaluator2ID;
                    objCmd.Parameters.Add("REVIEWERID_IN", OracleDbType.Varchar2).Value = ReviewerID;
                    objCmd.Parameters.Add("REVIEWER2ID_IN", OracleDbType.Varchar2).Value = Reviewer2ID;
                    objCmd.Parameters.Add("COMMENT1ID_IN", OracleDbType.Varchar2).Value = Comment1ID;
                    objCmd.Parameters.Add("OPHEADID_IN", OracleDbType.Varchar2).Value = OPHeadID;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = UserID;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
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
        /// UPDATE USER PMS AND OJT PERIOD FOR ASSOCIATE
        /// </summary>
        /// <param name="AssociateID"></param>
        /// <param name="KIID"></param>
        /// <param name="GSStartDate"></param>
        /// <param name="GSEndDate"></param>
        /// <param name="FHStartDate"></param>
        /// <param name="FHEndDate"></param>
        /// <param name="SHStartDate"></param>
        /// <param name="SHEndDate"></param>
        /// <param name="EligibleStatus"></param>
        /// <param name="ActiveStatus"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public string UpdatePMSUserPeriod(string AssociateID, string KIID, string GSStartDate, string GSEndDate,
                                            string FHStartDate, string FHEndDate, string SHStartDate, string SHEndDate,
                                            string EligibleStatus, string ActiveStatus, string UserID)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_PMSPERIOD_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("USERID_IN", OracleDbType.Varchar2).Value = AssociateID;
                    objCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = KIID;
                    objCmd.Parameters.Add("GSSTARTDATE_IN", OracleDbType.Varchar2).Value = GSStartDate;
                    objCmd.Parameters.Add("GSENDDATE_IN", OracleDbType.Varchar2).Value = GSEndDate;
                    objCmd.Parameters.Add("FHSTARTDATE_IN", OracleDbType.Varchar2).Value = FHStartDate;
                    objCmd.Parameters.Add("FHENDDATE_IN", OracleDbType.Varchar2).Value = FHEndDate;
                    objCmd.Parameters.Add("SHSTARTDATE_IN", OracleDbType.Varchar2).Value = SHStartDate;
                    objCmd.Parameters.Add("SHENDDATE_IN", OracleDbType.Varchar2).Value = SHEndDate;
                    objCmd.Parameters.Add("ELIGIBLESTATUS_IN", OracleDbType.Varchar2).Value = EligibleStatus;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = ActiveStatus;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = UserID;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
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
        /// UPDATE PMS STATUS OF ASSOCIATE
        /// </summary>
        /// <param name="AssociateID"></param>
        /// <param name="KIID"></param>
        /// <param name="GSStatus"></param>
        /// <param name="FHStatus"></param>
        /// <param name="SHStatus"></param>
        /// <param name="EligibleStatus"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public string UpdatePMSStatus(string AssociateID, string KIID, string GSStatus, string FHStatus,
                                        string SHStatus, string UserID)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_PMSSTATUS_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("USERID_IN", OracleDbType.Varchar2).Value = AssociateID;
                    objCmd.Parameters.Add("KIID_IN", OracleDbType.Varchar2).Value = KIID;
                    objCmd.Parameters.Add("GSSTATUS_IN", OracleDbType.Varchar2).Value = GSStatus;
                    objCmd.Parameters.Add("FHSTATUS_IN", OracleDbType.Varchar2).Value = FHStatus;
                    objCmd.Parameters.Add("SHSTATUS_IN", OracleDbType.Varchar2).Value = SHStatus;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = UserID;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
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


        public string UpdatePastexp(string AssociateID, string pastexp, string UserID)
        {
            //ConnectionString objCnStr;
            string strCn;
            OracleCommand objCmd;
            string strErrMsg;

            //objCnStr = new ConnectionString();
            strCn = objCnStr.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandText = "PKG_HRPMS.SPORC_PASTEXP_SET";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.Parameters.Add("USERID_IN", OracleDbType.Varchar2).Value = AssociateID;
                    objCmd.Parameters.Add("PASTEXP_IN", OracleDbType.Varchar2).Value = pastexp;
                    objCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = UserID;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    return strErrMsg;
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
    }
}
