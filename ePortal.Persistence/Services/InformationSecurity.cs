using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Persistence.Interface;
using Oracle.ManagedDataAccess.Client;

namespace ePortal.Persistence.Services
{
    public class InformationSecurity: IInformationSecurity
    {
        DataSet ds = new DataSet();
        DataRow[] _datarow;
        string qry;

        private readonly IDataManagement oDataMgmt;  //DataManagement oDataMgmt = new DataManagement();
        private readonly IConnectionString objCnStr;
        

        public InformationSecurity(IDataManagement _oDataMgmt, IConnectionString _objCnStr)
        {
            oDataMgmt = _oDataMgmt;
            objCnStr = _objCnStr;
        }

        public DataTable GetInformationSecurityProc_Formats()
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_PROCEDUREFORMATS_GET";
            oCmd.Parameters.Add("CUR_PROCLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataTable GetInformationSecurityPolicy_Guidelines()
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_POLICYGUIDELINE_GET";
            oCmd.Parameters.Add("CUR_PROCLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public DataTable GetInformationSecurityPolicy_Awereness()
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_POLICYAwereness_GET";
            oCmd.Parameters.Add("CUR_PROCLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET THE QUESTION 
        /// </summary>
        /// <param name="SurveyNumber"></param>
        /// <returns></returns>
        public DataTable GetInformationSecurityQuiz_Question(int SurveyNumber)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_HR_GETQUESTION";
            oCmd.Parameters.Add("CUR_GETQUESTION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("SURVEYNUM_IN", OracleDbType.Int32).Value = SurveyNumber;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public DataTable GetInformationSecurityQuiz_QuestionFixed(int SurveyNumber, long ecode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_HR_GETQUESTION_FIXED";
            oCmd.Parameters.Add("CUR_GETQUESTION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("SURVEYNUM_IN", OracleDbType.Int32).Value = SurveyNumber;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = ecode;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET ANSWER FROM DATABASE
        /// </summary>
        /// <returns></returns>
        public DataTable GetInformationSecurityQuiz_Answer(int QuesID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_HR_GETANSWER";
            oCmd.Parameters.Add("CUR_GETANSWER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("QUESTIONID_IN", OracleDbType.Int32).Value = QuesID;

            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// SAVE QUIZ DATA
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="QuestingID"></param>
        /// <param name="AnswerID"></param>
        /// <returns></returns>
        public string SaveQuizData(string UserID, string QuestingID, string AnswerID, string strremark)
        {
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;
            string errMsg = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_QUIZDATA_SET";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = UserID;
                    objCmd.Parameters.Add("QUESTIONID_IN", OracleDbType.Varchar2).Value = QuestingID;
                    objCmd.Parameters.Add("ANSWERID_IN", OracleDbType.Varchar2).Value = AnswerID;
                    objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = strremark;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    //IF ERROR OCCURED
                    if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                    {
                        errMsg = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }
            return (errMsg);
        }

        public string SaveSurveyremarks(string UserID, string SurveyID, string strremark)
        {
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;
            string errMsg = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_SURVEYCOMMENT_SET";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = UserID;
                    objCmd.Parameters.Add("SURVEYID_IN", OracleDbType.Varchar2).Value = SurveyID;
                    objCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = strremark;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    //IF ERROR OCCURED
                    if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                    {
                        errMsg = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }
            return (errMsg);
        }
        public string Surveyskip(string UserID, string SurveyID, string strskipcount)
        {
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;
            string errMsg = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_SURVEYSKIPCOUNT_SET";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = UserID;
                    objCmd.Parameters.Add("SURVEYID_IN", OracleDbType.Varchar2).Value = SurveyID;
                    objCmd.Parameters.Add("SKIPCOUNT_IN", OracleDbType.Varchar2).Value = strskipcount;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    //IF ERROR OCCURED
                    if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                    {
                        errMsg = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }
            return (errMsg);
        }

        public int GetSurveyskipCount(int SurveyNumber, string UserID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            int Result;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_SURVEYSKIPCOUNT_GET";
            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("SURVEYNUM_IN", OracleDbType.Int32).Value = SurveyNumber;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = UserID;

            oDataMgmt.ExecuteQuery(oCmd);
            Result = Convert.ToInt16(oCmd.Parameters["RESULT_OUT"].Value.ToString());
            return (Result);
        }
        /// <summary>
        /// CHECK ANSWER FROM DATABASE
        /// </summary>
        /// <returns></returns>
        public DataTable CheckInformationSecurityQuiz_Answer(int EMPCODE, int SurveyID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_HR_CHECKANSWER";
            oCmd.Parameters.Add("CUR_GETANSWER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = EMPCODE;
            oCmd.Parameters.Add("SURVEYID_IN", OracleDbType.Int32).Value = SurveyID;

            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET INFORMATION SECURITY SURVEY RESULT
        /// </summary>
        /// <param name="strDesignationId"></param>
        /// <param name="strDivisionId"></param>
        /// <param name="strDepartmentId"></param>
        /// <param name="strSectionId"></param>
        /// <param name="strOperationId"></param>
        /// <param name="strAssoEmpCode"></param>
        /// <param name="strAssoEmpName"></param>
        /// <param name="SurveyNumber"></param>
        /// <returns></returns>
        public DataTable GetInfoSecSurveyRpt(string strDesignationId, string strDivisionId, string strDepartmentId,
                                             string strSectionId, string strOperationId, string strAssoEmpCode,
                                             string strAssoEmpName, string SurveyNumber)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_SURVEYREPORT_GET";
            oCmd.Parameters.Add("CUR_PROCLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            oCmd.Parameters.Add("DESIGNATIONID_IN", OracleDbType.Varchar2).Value = strDesignationId;
            if (strOperationId != "0")
            {
                oCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Varchar2).Value = strOperationId;
            }
            if (strDivisionId != "0")
            {
                oCmd.Parameters.Add("DIVISIONID_IN", OracleDbType.Varchar2).Value = strDivisionId;
            }
            if (strDepartmentId != "0")
            {
                oCmd.Parameters.Add("DEPARTMENTID_IN", OracleDbType.Varchar2).Value = strDepartmentId;
            }
            if (strSectionId != "0")
            {
                oCmd.Parameters.Add("SECTIONID_IN", OracleDbType.Varchar2).Value = strSectionId;
            }
            if (strAssoEmpCode != "")
            {
                oCmd.Parameters.Add("ASSOEMPCODE_IN", OracleDbType.Varchar2).Value = strAssoEmpCode;
            }
            if (strAssoEmpName != "")
            {
                oCmd.Parameters.Add("ASSOEMPNAME_IN", OracleDbType.Varchar2).Value = strAssoEmpName;
            }
            oCmd.Parameters.Add("SURVEYNUM_IN", OracleDbType.Int32).Value = SurveyNumber;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }


        public DataTable GetEMPWISESurveyRpt(string strDesignationId, string strDivisionId, string strDepartmentId,
                                           string strSectionId, string strOperationId, string strAssoEmpCode,
                                           string strAssoEmpName, string SurveyNumber)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_EMPWISEREPORT_GET";
            oCmd.Parameters.Add("CUR_PROCLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            oCmd.Parameters.Add("DESIGNATIONID_IN", OracleDbType.Varchar2).Value = strDesignationId;
            if (strOperationId != "0")
            {
                oCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Varchar2).Value = strOperationId;
            }
            if (strDivisionId != "0")
            {
                oCmd.Parameters.Add("DIVISIONID_IN", OracleDbType.Varchar2).Value = strDivisionId;
            }
            if (strDepartmentId != "0")
            {
                oCmd.Parameters.Add("DEPARTMENTID_IN", OracleDbType.Varchar2).Value = strDepartmentId;
            }
            if (strSectionId != "0")
            {
                oCmd.Parameters.Add("SECTIONID_IN", OracleDbType.Varchar2).Value = strSectionId;
            }
            if (strAssoEmpCode != "")
            {
                oCmd.Parameters.Add("ASSOEMPCODE_IN", OracleDbType.Varchar2).Value = strAssoEmpCode;
            }
            if (strAssoEmpName != "")
            {
                oCmd.Parameters.Add("ASSOEMPNAME_IN", OracleDbType.Varchar2).Value = strAssoEmpName;
            }
            oCmd.Parameters.Add("SURVEYNUM_IN", OracleDbType.Int32).Value = SurveyNumber;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// CHECK QUIZ COMPLETED BY ASSOCIATE OR NOT
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="SurveyNumber"></param>
        /// <returns></returns>
        public Boolean IsQuizCompleted(string UserID, int SurveyNumber)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            int PendingAnsCount;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_QUIZCOMPLETE_GET";
            oCmd.Parameters.Add("ANS_COUNT", OracleDbType.Int32).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = UserID;
            oCmd.Parameters.Add("SURVEYNUM_IN", OracleDbType.Int32).Value = SurveyNumber;


            oDataMgmt.ExecuteQuery(oCmd);

            PendingAnsCount = Convert.ToInt16(oCmd.Parameters["ANS_COUNT"].Value.ToString());
            if (PendingAnsCount == 0)
                return true;
            else
                return false;

        }

        /// <summary>
        /// CHECK ASSOCIATE CLEAR THE QUIZ OR NOT
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="SurveyNumber"></param>
        /// <returns></returns>
        public Boolean IsQuizClearByAssociate(string UserID, int SurveyNumber)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            int Result;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_QUIZCLEARED_GET";
            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = UserID;
            oCmd.Parameters.Add("SURVEYNUM_IN", OracleDbType.Int32).Value = SurveyNumber;


            oDataMgmt.ExecuteQuery(oCmd);

            Result = Convert.ToInt16(oCmd.Parameters["RESULT_OUT"].Value.ToString());
            if (Result == 0)
                return false;
            else
                return true;

        }

        public Boolean IsSelfdeclarationCompleted(string UserID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            int PendingAnsCount;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_DECLARATIONCOMPLETE_GET";
            oCmd.Parameters.Add("ANS_COUNT", OracleDbType.Int32).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = UserID;

            oDataMgmt.ExecuteQuery(oCmd);

            PendingAnsCount = Convert.ToInt16(oCmd.Parameters["ANS_COUNT"].Value.ToString());
            if (PendingAnsCount == 0)
                return true;
            else
                return false;

        }

        /// <summary>
        /// GET INFORMATION SECURITY MODULE OVERALL RESULT
        /// </summary>
        /// <returns></returns>
        public DataTable GetInfoSecurityOvelAllResult(string strDesignationId,
                                            string strDivisionId, string strDepartmentId, string strSectionId, string strOperationId,
                                            string strAssoEmpCode, string strAssoEmpName, string SurveyNumber)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_QUIZOVELALLRESULT_GET";
            oCmd.Parameters.Add("CUR_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            oCmd.Parameters.Add("DESIGNATIONID_IN", OracleDbType.Varchar2).Value = strDesignationId;
            if (strOperationId != "0")
            {
                oCmd.Parameters.Add("OPERATIONID_IN", OracleDbType.Varchar2).Value = strOperationId;
            }
            if (strDivisionId != "0")
            {
                oCmd.Parameters.Add("DIVISIONID_IN", OracleDbType.Varchar2).Value = strDivisionId;
            }
            if (strDepartmentId != "0")
            {
                oCmd.Parameters.Add("DEPARTMENTID_IN", OracleDbType.Varchar2).Value = strDepartmentId;
            }
            if (strSectionId != "0")
            {
                oCmd.Parameters.Add("SECTIONID_IN", OracleDbType.Varchar2).Value = strSectionId;
            }
            if (strAssoEmpCode != "")
            {
                oCmd.Parameters.Add("ASSOEMPCODE_IN", OracleDbType.Varchar2).Value = strAssoEmpCode;
            }
            if (strAssoEmpName != "")
            {
                oCmd.Parameters.Add("ASSOEMPNAME_IN", OracleDbType.Varchar2).Value = strAssoEmpName;
            }
            oCmd.Parameters.Add("SURVEYNUM_IN", OracleDbType.Int32).Value = SurveyNumber;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET ACTIVE SURVEY NUBBER
        /// </summary>
        /// <returns></returns>
        public DataTable GetActiveSurvey()
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_ACTIVESURVEY_GET";
            oCmd.Parameters.Add("CUR_SURVEY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        //=================================================================================================
        //GET ACTIVE SURVEY NUMBER (WITH TEAM MEMBER CHANGE) on 25-11-2022(Aumento)
        //=================================================================================================
        public DataTable GetActiveSurveyWithTM(int EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_ACTIVESURVEY_GET_WITH_TM";
            oCmd.Parameters.Add("EMPCODE", EmpCode);
            oCmd.Parameters.Add("CUR_SURVEY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        //=================================================================================================
        public int GetActiveSurvey_Reteststatus()
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            int SurveyReteststatus = 0;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_ACTIVESURVEYRETEST_GET";
            oCmd.Parameters.Add("CUR_SURVEY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);
            if (dt.Rows.Count > 0)
                SurveyReteststatus = Convert.ToInt16(dt.Rows[0]["HRSECURITYSURVEYID"]);

            return (SurveyReteststatus);
        }


        /// <summary>
        /// GET TOTAL QUS IN PARTICULAR SURVEY
        /// </summary>
        /// <param name="SurveyNumber"></param>
        /// <returns></returns>
        public int GetQuestionCount(int SurveyNumber)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            int Result;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_SURVEYQUSCOUNT_GET";
            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("SURVEYNUM_IN", OracleDbType.Int32).Value = SurveyNumber;

            oDataMgmt.ExecuteQuery(oCmd);
            Result = Convert.ToInt16(oCmd.Parameters["RESULT_OUT"].Value.ToString());
            return (Result);
        }

        /// <summary>
        /// GET SURVEY DESCRIPTION
        /// </summary>
        /// <param name="SurveyNumber"></param>
        /// <returns></returns>
        public string GetSurveyDescription(int SurveyNumber)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            string SurveyDesc = string.Empty;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_SURVEYDESC_GET";
            oCmd.Parameters.Add("SURVEYDESC_IN", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("SURVEYNUM_IN", OracleDbType.Int32).Value = SurveyNumber;
            oDataMgmt.ExecuteQuery(oCmd);

            SurveyDesc = Convert.ToString(oCmd.Parameters["SURVEYDESC_IN"].Value);
            return (SurveyDesc);
        }

        /// <summary>
        /// GET ACTIVE SURVEY LIST
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public DataTable GetSurveyList(string UserID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_SURVEYLIST_GET";
            oCmd.Parameters.Add("CUR_SURVEY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = UserID;

            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataTable GetALLSurveyList()
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_ALLSURVEYLIST_GET";
            oCmd.Parameters.Add("CUR_SURVEY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataTable GetPartSurveyList(string surveyid)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_PARTSURVEYLIST_GET";
            oCmd.Parameters.Add("SURVEYID_IN", OracleDbType.Varchar2).Value = surveyid;
            oCmd.Parameters.Add("CUR_SURVEY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataTable GetQuesDtls(string quesid)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_QUESDTLS_GET";
            oCmd.Parameters.Add("QUESID_IN", OracleDbType.Varchar2).Value = quesid;
            oCmd.Parameters.Add("CUR_SURVEY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataTable GetAnsDtls(string ansid)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_ANSDTLS_GET";
            oCmd.Parameters.Add("ANSID_IN", OracleDbType.Varchar2).Value = ansid;
            oCmd.Parameters.Add("CUR_SURVEY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }


        /// <summary>
        /// GET SURVEY RESULT QUSTION WISE
        /// </summary>
        /// <param name="SurveyID"></param>
        /// <param name="EmpCode"></param>
        /// <returns></returns>
        public DataTable GetSurveyResult(string SurveyID, string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_SURVEYRESULT_GET";
            oCmd.Parameters.Add("CUR_SURVEY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("SURVEYID_IN", OracleDbType.Int32).Value = SurveyID;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = EmpCode;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// GET SURVEY RESULT IN (%)
        /// </summary>
        /// <param name="SurveyNumber"></param>
        /// <param name="EmpCode"></param>
        /// <returns></returns>
        public int GetSurveyResultInPercentage(string SurveyNumber, string EmpCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            int Result;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_SURVEYPERRESULT_GET";
            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("SURVEYNUM_IN", OracleDbType.Varchar2).Value = SurveyNumber;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;

            oDataMgmt.ExecuteQuery(oCmd);
            Result = Convert.ToInt16(oCmd.Parameters["RESULT_OUT"].Value.ToString());
            return (Result);
        }

        public string AddEditSurveyDtls(string userid, string surveyid, string title, string descrip, string status, string fromdate, string todate, string retest, string stremail, string strfcomment)
        {
            OracleCommand cmd = new OracleCommand();
            string strErrMsg = string.Empty;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_ADDEDITSURVEYDTLS";
            cmd.Parameters.Add("UserID_IN", OracleDbType.Varchar2).Value = userid;
            cmd.Parameters.Add("SURVEYID_IN", OracleDbType.Varchar2).Value = surveyid;
            cmd.Parameters.Add("TITLE_IN", OracleDbType.Varchar2).Value = title;
            cmd.Parameters.Add("DESCRIP_IN", OracleDbType.Varchar2).Value = descrip;
            cmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            cmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = fromdate;
            cmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = todate;
            cmd.Parameters.Add("RETEST_IN", OracleDbType.Int32).Value = retest;
            cmd.Parameters.Add("RESULTEMAIL_IN", OracleDbType.Varchar2).Value = stremail;
            cmd.Parameters.Add("FEEDBACKCOMMENT_IN", OracleDbType.Varchar2).Value = strfcomment;
            cmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 2).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

            //DataManagement oDataMgmt = new DataManagement();
            oDataMgmt.ExecuteQuery(cmd);
            strErrMsg = Convert.ToString(cmd.Parameters["RESULT_OUT"].Value);
            return strErrMsg;
        }

        public string AddEditSurveyQues(string userID, string sid, string quesid, string desc, string status)
        {
            OracleCommand cmd = new OracleCommand();
            string strErrMsg = string.Empty;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_ADDEDITQUESDTLS";
            cmd.Parameters.Add("UserID_IN", OracleDbType.Varchar2).Value = userID;
            cmd.Parameters.Add("SURVEYID_IN", OracleDbType.Varchar2).Value = sid;
            cmd.Parameters.Add("QUESID_IN", OracleDbType.Varchar2).Value = quesid;
            cmd.Parameters.Add("DESCRIP_IN", OracleDbType.Varchar2).Value = desc;
            cmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            cmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 2).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

            //DataManagement oDataMgmt = new DataManagement();
            oDataMgmt.ExecuteQuery(cmd);
            strErrMsg = Convert.ToString(cmd.Parameters["RESULT_OUT"].Value);
            return strErrMsg;
        }

        /// <summary>
        /// ADD AND UPDATE ANS DETAILS FOR A QUES IN A SURVEY
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="ansid"></param>
        /// <param name="quesid"></param>
        /// <param name="desc"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public string AddEditSurveyAns(string userID, string ansid, string quesid, string desc, string status, string IsCorrect)
        {
            OracleCommand cmd = new OracleCommand();
            string strErrMsg = string.Empty;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_ADDEDITANSDTLS_SET";
            cmd.Parameters.Add("USERID_IN", OracleDbType.Varchar2).Value = userID;
            cmd.Parameters.Add("ANSID_IN", OracleDbType.Varchar2).Value = ansid;
            cmd.Parameters.Add("QUESID_IN", OracleDbType.Varchar2).Value = quesid;
            cmd.Parameters.Add("DESCRIP_IN", OracleDbType.Varchar2).Value = desc;
            cmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = status;
            cmd.Parameters.Add("ISCORRECT_IN", OracleDbType.Varchar2).Value = IsCorrect;
            cmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 2).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

            //DataManagement oDataMgmt = new DataManagement();
            oDataMgmt.ExecuteQuery(cmd);
            strErrMsg = Convert.ToString(cmd.Parameters["RESULT_OUT"].Value) + "#" + Convert.ToString(cmd.Parameters["ERRMSG"].Value);
            return strErrMsg;
        }

        //-----------ADDED BY ALOK------------------
        public string UPDATEQuizData(string UserID, string QuestingID, string AnswerID)
        {
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;
            string errMsg = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_UPDATEQUIZDATA_SET";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = UserID;
                    objCmd.Parameters.Add("QUESTIONID_IN", OracleDbType.Varchar2).Value = QuestingID;
                    objCmd.Parameters.Add("ANSWERID_IN", OracleDbType.Varchar2).Value = AnswerID;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    //IF ERROR OCCURED
                    if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                    {
                        errMsg = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }
            return (errMsg);
        }
        public DataTable GetEmail()
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.GET_EMAIL";
            oCmd.Parameters.Add("CUR_HD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public DataTable GetALLANSList(string strqus)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_ANSLIST_GET";
            oCmd.Parameters.Add("QUESID_IN", OracleDbType.Varchar2).Value = strqus;
            oCmd.Parameters.Add("CUR_SURVEY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public DataTable GetALLQUSList(string strsurvey)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_QUESLIST_GET";
            oCmd.Parameters.Add("SURVEYID_IN", OracleDbType.Varchar2).Value = strsurvey;
            oCmd.Parameters.Add("CUR_SURVEY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public DataTable GetFAILASSO_EMAIL(string strsurvey, string strcutoff)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.Parameters.Add("SURVEY_IN", OracleDbType.Int32).Value = strsurvey;
            oCmd.Parameters.Add("CUTOFF_IN", OracleDbType.Varchar2).Value = strcutoff;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_FAILASSO_EMAIL_GET";
            oCmd.Parameters.Add("CUR_HD", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }


        #region "SAP  implementer partner selection quiz data"
        public Boolean ISSAPPartSelectionQuizFilled(string strUserID)
        {
            //ConnectionString objCnStr = new ConnectionString(); 
            //string strCn = objCnStr.getConnectingString(); 
            OracleCommand objCmd;
            //using (OracleConnection objCn = new OracleConnection())
            //{
            //    objCn.ConnectionString = strCn;
            //    objCn.Open();
            try
            {
                objCmd = new OracleCommand();
                //objCmd.Connection = objCn;
                objCmd.CommandType = System.Data.CommandType.Text;
                objCmd.CommandText = " select count(s.id),t.head from(" +
                                     " select e.adempcode,case when dvh.divisionheadid=e.adempcode then 'DH'" +
                                     " when oh.vpheadid=e.adempcode then 'OH' " +
                                     " when e.adempcode in(5143,5284,3444,4875,125,70000063,3774,4036,4320,3998,4074,3904,2679,2689,32,117,860,858,695,3617,4367,224,3773,2037,1994,72,3787,3730,2203,2278)" +
                                     "  then 'OH' " +
                                     " else ''  end as head " +
                                     " from ademployee e left join addivisionhead dvh on dvh.divisionheadid=e.adempcode and dvh.active=1" +
                                     " left join adophead oh on oh.vpheadid=e.adempcode and oh.active=1" +
                                     " where e.adempcode=" + strUserID + ")t  left join  sissappartnerselection s  on s.empcode=t.adempcode" +
                                     " where t.head='DH' or t.head='OH' group by t.head";
                DataTable DT = oDataMgmt.GetDataTable(objCmd);
                if (DT.Rows.Count > 0 && DT.Rows[0][0].ToString() == "0")
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public string SAPPartnerSelectionQuizData(string strQuestingID, string strUserID, string stracc_ans, string stribm_ans, string strhp_ans, string strcsc_ans, string strcap_ans)
        {
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;
            string errMsg = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_SAPPARTNERSELECTION_QUIZ";
                    objCmd.Parameters.Add("V_QID", OracleDbType.Int32).Value = strQuestingID;
                    objCmd.Parameters.Add("V_EMPCODE", OracleDbType.Int32).Value = strUserID;
                    objCmd.Parameters.Add("V_ACC_COM", OracleDbType.Int32).Value = stracc_ans;
                    objCmd.Parameters.Add("V_IBM_COM", OracleDbType.Int32).Value = stribm_ans;
                    objCmd.Parameters.Add("V_HP_COM", OracleDbType.Int32).Value = strhp_ans;
                    objCmd.Parameters.Add("V_CSC_COM", OracleDbType.Int32).Value = strcsc_ans;
                    objCmd.Parameters.Add("V_CAP_COM", OracleDbType.Int32).Value = strcap_ans;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    //IF ERROR OCCURED
                    if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                    {
                        errMsg = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }
            return (errMsg);
        }
        public DataSet SAPPARTNERREPORT()
        {
            OracleCommand oCmd = new OracleCommand();
            DataSet ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_SAPIMPREPORT_GET";
            oCmd.Parameters.Add("CUR_OP", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_DIV", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_OTH", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_CUM", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }
        #endregion

        #region "IT Servey quiz data"
        public DataTable Get_QUSReport(int QuesID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_QUSWISEREPORT_GET";
            oCmd.Parameters.Add("CUR_GETANSWER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("QUESTIONID_IN", OracleDbType.Int32).Value = QuesID;

            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public DataTable Get_UserWiseReport()
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_ASSOCIATEREPORT_GET";
            oCmd.Parameters.Add("CUR_GETANSWER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public DataTable Get_WorstFeedbackReport(int VpID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_WORSTFEEDBACKREPORT_GET";
            oCmd.Parameters.Add("CUR_GETANSWER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("VPID_IN", OracleDbType.Int32).Value = VpID;

            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public DataTable Get_Operation(int VpID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_OPERATION_GET";
            oCmd.Parameters.Add("CUR_OPERATION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("VPID_IN", OracleDbType.Int32).Value = VpID;

            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        #endregion

        public string SaveITSelfDeclaration(string UserID)
        {
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;
            string errMsg = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_ITDECLARATION_SET";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = UserID;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    //IF ERROR OCCURED
                    if (objCmd.Parameters["RESULT_OUT"].Value.ToString() == "0")
                    {
                        errMsg = Convert.ToString(objCmd.Parameters["ERRMSG"].Value);
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }
            return (errMsg);
        }


        public string AddITSelftrequest(string strDateFrom, string UserID, string strDescription, string status, string Attachment)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    //objCmd.Connection = objCn;
                    string strSql = "PKG_INFORMATIONSECURITY.SPROC_INERT_IT_SELF_DEC";
                    objCmd.Parameters.Add("EMPLOYEE_IN", OracleDbType.Int32).Value = UserID;
                    objCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = strDateFrom;
                    objCmd.Parameters.Add("REASON_IN", OracleDbType.Varchar2).Value = strDescription;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = status;
                    objCmd.Parameters.Add("ATTACHMENT_IN", OracleDbType.Varchar2).Value = Attachment;
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

        public DataTable GetALLQUSList()
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_SIS_SELF_RECSLIST_GET";
            oCmd.Parameters.Add("CUR_SURVEY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataTable GetSISSELFRECSList(String ID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_SIS_IT_SELF_LISTBYID_GET";
            oCmd.Parameters.Add("ITSELFDECLCONDITIONID_IN", OracleDbType.Varchar2).Value = ID;
            oCmd.Parameters.Add("CUR_SURVEY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public string UpdateITSelftrequest(string strDateFrom, string UserID, string strDescription, string status, string Attachment, string ITSELFDECLCONDITIONID, string FilePath)
        {

            string Filepath;

            if (Attachment != "")
            {
                Filepath = Attachment;
            }
            else if (Attachment == "" && FilePath != "")
            {
                Filepath = FilePath;
            }

            else
            {
                Filepath = Attachment;
            }
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    //objCmd.Connection = objCn;
                    string strSql = "PKG_INFORMATIONSECURITY.SPROC_UPDATE_IT_SELF_DEC";
                    objCmd.Parameters.Add("EMPLOYEE_IN", OracleDbType.Int32).Value = UserID;
                    objCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = strDateFrom;
                    objCmd.Parameters.Add("REASON_IN", OracleDbType.Varchar2).Value = strDescription;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = status;
                    objCmd.Parameters.Add("ATTACHMENT_IN", OracleDbType.Varchar2).Value = Filepath;
                    objCmd.Parameters.Add("ITSELFDECLCONDITIONID_IN", OracleDbType.Int32).Value = ITSELFDECLCONDITIONID;
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

        public int CheckStatus()
        {
            int TxnNumber = 0;
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
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
                    objCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_COUNT_GET_STATUS";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;

                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();
                    TxnNumber = System.Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                }
                catch (Exception ex)
                {

                    objTxn.Rollback();
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

        public int CheckStatusUpdationTime(String ID)
        {
            int TxnNumber = 0;
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
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
                    objCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_COUNT_GET_STATUSBYID";
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.Parameters.Add("ITSELFDECLCONDITIONID_IN", OracleDbType.Int32).Value = ID;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 4).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();
                    TxnNumber = System.Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                }
                catch (Exception ex)
                {

                    objTxn.Rollback();
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

        public DataTable GetResult_Question(int SurveyNumber)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_EXPORTQUSWISEREPORT_GET";
            oCmd.Parameters.Add("CUR_GETANSWER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("SURVEYID_IN", OracleDbType.Int32).Value = SurveyNumber;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        //Added by Aumento as on 19-06-2024 
        public string GetSurveyErrorEmailId()
        {
            string emailid = string.Empty;

            //ConnectionString objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            OracleTransaction objTxn;

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                objTxn = objCn.BeginTransaction();
                try
                {
                    OracleCommand objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_HR_SURVEY_EMAIL_GET";
                    objCmd.Parameters.Add("SURVEY_EMAIL", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();

                    emailid = objCmd.Parameters["SURVEY_EMAIL"].Value.ToString();
                }
                catch (Exception ex)
                {
                    objTxn.Rollback();
                }
                finally
                {
                    if (objCn != null)
                    {
                        objCn.Close();
                    }
                }
            }

            return emailid;
        }
        ////Survey Result in Excel start
        public DataTable GetQuesRpt(string strSurveyHeaderID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_INFORMATIONSECURITY.SPROC_QUIZQUESRESULT"; //Need to create/update procedure
            oCmd.Parameters.Add("CUR_GETANSWER", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("SURVEYID_IN", OracleDbType.Varchar2).Value = strSurveyHeaderID;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        ////Survey Result in Excel end
    }
}
