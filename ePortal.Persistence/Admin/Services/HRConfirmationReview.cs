using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Persistence.Admin.Services
{
    public class HRConfirmationReview: IHRConfirmationReview
    {

        private readonly IDataManagement oDataMgmt;
        private readonly IConnectionString objCnStr;
        #region "ctor"
        public HRConfirmationReview(IDataManagement _oDataMgmt, IConnectionString _objCnStr)
        {
            oDataMgmt = _oDataMgmt;
            objCnStr = _objCnStr;
        }
        #endregion

        #region"Instance variables"
        DataSet ds = new DataSet();
        DataRow[] _datarow;
        //DataManagement oDataMgmt = new DataManagement();
        String strQry = String.Empty;
        #endregion

        #region "Get Data"

        /// <summary>
        /// Get HC Diagnosis List
        /// </summary>
        /// <returns>DataTable</returns>
        public DataTable GetTraitsList()
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_HRCONFIRMATIONREVIEW.SPROC_TRAITS_GET";
            oCmd.Parameters.Add("CUR_HRTRAITS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        /// <summary>
        /// Get Daignosis By ID
        /// </summary>
        /// <param name="strID">ID</param>
        /// <returns>DataTable</returns>
        public DataTable GetTraitsAttributes(String strTraits, String ConfirmationFormID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_HRCONFIRMATIONREVIEW.SPROC_TRAITSATTRIBYID_GET";
            oCmd.Parameters.Add("TRAITID_IN", OracleDbType.Varchar2).Value = strTraits;
            oCmd.Parameters.Add("CONFIRMATIONFORMID_IN", OracleDbType.Varchar2).Value = ConfirmationFormID;
            oCmd.Parameters.Add("CUR_HRTRAITS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataSet GetEmpDetails(String strAdempcode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataSet ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_HRCONFIRMATIONREVIEW.SPROC_EMPDETAILS_GET";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = strAdempcode;
            oCmd.Parameters.Add("CUR_HRDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_EMPSUPDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }

        public DataSet GetProcessDetails(String strProcessID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataSet ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_HRCONFIRMATIONREVIEW.SPROC_REVIEWFORMDETAILS_GET";
            oCmd.Parameters.Add("PROCESSID_IN", OracleDbType.Varchar2).Value = strProcessID;
            oCmd.Parameters.Add("CUR_HRDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            //oCmd.Parameters.Add("CUR_EMPSUPDETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }

        public DataTable GetConfirmationFormRatings(String confirmationFormID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_HRCONFIRMATIONREVIEW.SPROC_RATINGBYFORMID_GET";
            oCmd.Parameters.Add("CONFIRMATIONFORMID_IN", OracleDbType.Varchar2).Value = confirmationFormID;
            oCmd.Parameters.Add("CUR_HRFORM", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataTable GetEmployeeListByConfirmationDateMonth(String Year, String Month, String Ecode, String Status, String Site)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_HRCONFIRMATIONREVIEW.SPROC_EMPLISTBYCONFDT";
            oCmd.Parameters.Add("V_YEAR", OracleDbType.Varchar2).Value = Year;
            oCmd.Parameters.Add("V_MONTH", OracleDbType.Varchar2).Value = Month;
            oCmd.Parameters.Add("V_ECODE", OracleDbType.Varchar2).Value = Ecode;
            oCmd.Parameters.Add("V_STATUS", OracleDbType.Varchar2).Value = Status;
            oCmd.Parameters.Add("V_SITE", OracleDbType.Varchar2).Value = Site;
            oCmd.Parameters.Add("CUR_HRLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataTable GetConfirmationList(String Month, String UserCode, String OperationID, String DivisionID, String DepratmentID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_HRCONFIRMATIONREVIEW.SPROC_CONFIRMATIONLIST_GET";
            oCmd.Parameters.Add("V_MONTH", OracleDbType.Varchar2).Value = Month;
            oCmd.Parameters.Add("V_LOGINUSERCODE", OracleDbType.Varchar2).Value = UserCode;
            oCmd.Parameters.Add("V_OPERATIONID", OracleDbType.Varchar2).Value = OperationID;
            oCmd.Parameters.Add("V_DIVISIONID", OracleDbType.Varchar2).Value = DivisionID;
            oCmd.Parameters.Add("V_DEPARTMENTID", OracleDbType.Varchar2).Value = DepratmentID;
            oCmd.Parameters.Add("CUR_HRLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataTable GetConfirmationHistory(String Month, String OperationID, String DivisionID, String DepratmentID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_HRCONFIRMATIONREVIEW.SPROC_CONFIRMATIONHISTORY_GET";
            oCmd.Parameters.Add("V_MONTH", OracleDbType.Varchar2).Value = Month;
            oCmd.Parameters.Add("V_OPERATIONID", OracleDbType.Varchar2).Value = OperationID;
            oCmd.Parameters.Add("V_DIVISIONID", OracleDbType.Varchar2).Value = DivisionID;
            oCmd.Parameters.Add("V_DEPARTMENTID", OracleDbType.Varchar2).Value = DepratmentID;
            oCmd.Parameters.Add("CUR_HRLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataTable GetEmployeeMailDetails(String Adempcode, String ProcessID, String ReviewID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_HRCONFIRMATIONREVIEW.SPROC_EMPMAILID_GET";
            oCmd.Parameters.Add("V_ADMEPCODE", OracleDbType.Varchar2).Value = Adempcode;
            oCmd.Parameters.Add("V_PROCESSID", OracleDbType.Varchar2).Value = ProcessID;
            oCmd.Parameters.Add("V_REVIEWFORMID", OracleDbType.Varchar2).Value = ReviewID;
            oCmd.Parameters.Add("CUR_HRLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataTable GetPendingConfirmationWithReviewAuth(String Year, String Month, String Ecode, String Status, String Site)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_HRCONFIRMATIONREVIEW.SPROC_PENDINGCONFIRREVAUTH_GET";
            oCmd.Parameters.Add("V_YEAR", OracleDbType.Varchar2).Value = Year;
            oCmd.Parameters.Add("V_MONTH", OracleDbType.Varchar2).Value = Month;
            oCmd.Parameters.Add("V_ECODE", OracleDbType.Varchar2).Value = Ecode;
            oCmd.Parameters.Add("V_STATUS", OracleDbType.Varchar2).Value = Status;
            oCmd.Parameters.Add("V_SITE", OracleDbType.Varchar2).Value = Site;
            oCmd.Parameters.Add("CUR_HRLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public int ValidateReviewAuthUpdate(String Adempcode, out int IsValidUpdate)
        {
            String strErrMsg = String.Empty;
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.CommandText = "PKG_HRCONFIRMATIONREVIEW.SPROC_VALIDATE_AUTHUPDATE";
                    objCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Varchar2).Value = Adempcode;
                    objCmd.Parameters.Add("V_VALID", OracleDbType.Int32).Direction = ParameterDirection.Output;


                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();

                    IsValidUpdate = Convert.ToInt32(objCmd.Parameters["V_VALID"].Value.ToString());

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

        public string GetHRMailDetails()
        {
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.CommandText = "PKG_HRCONFIRMATIONREVIEW.SPROC_HRMAILDETAILS_GET";
                    objCmd.Parameters.Add("V_PARAMVALUE", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();

                    return Convert.ToString(objCmd.Parameters["V_PARAMVALUE"].Value);

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

        public int CheckConfirmationMenuLinkRights(String Adempcode)
        {
            int IsApplicable = 0;
            String strErrMsg = String.Empty;
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.CommandText = "PKG_HRCONFIRMATIONREVIEW.SPROC_CONFIRMATIONMENU_GET";
                    objCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Varchar2).Value = Adempcode;
                    objCmd.Parameters.Add("V_ISAPPLICABLE", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();

                    IsApplicable = Convert.ToInt32(objCmd.Parameters["V_ISAPPLICABLE"].Value.ToString());
                    return IsApplicable;
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

        public DataTable GetConfirmationpending(String UserCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_HRCONFIRMATIONREVIEW.SPROC_MANAGEPENCONF_GET";
            oCmd.Parameters.Add("V_LOGINUSERCODE", OracleDbType.Varchar2).Value = UserCode;
            oCmd.Parameters.Add("CUR_HRLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }
        public DataTable GetConfirmationApproved(String UserCode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.CommandText = "PKG_HRCONFIRMATIONREVIEW.SPROC_MANAGEAPPCONF_GET";
            oCmd.Parameters.Add("V_LOGINUSERCODE", OracleDbType.Varchar2).Value = UserCode;
            oCmd.Parameters.Add("CUR_HRLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.BindByName = true;
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        #endregion

        #region"INSERT-UPDATE FUNCTION"


        public void InsertConfirmationReviewTransaction(FormDetails details)
        {
            String strErrMsg = String.Empty;
            String ConfirmationID = String.Empty;
            int res = 0;
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;
            OracleTransaction tran;

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                tran = objCn.BeginTransaction();

                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    objCmd.Transaction = tran;

                    string strSql = "PKG_HRCONFIRMATIONREVIEW.SPROC_INSERT_CONFIRMATIONFORM";
                    objCmd.Parameters.Add("V_ID", OracleDbType.Varchar2).Value = details.ConfirmationID;
                    objCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Varchar2).Value = details.EmpCode;
                    objCmd.Parameters.Add("V_FUNCDESIG", OracleDbType.Varchar2).Value = details.FunctionDesignation;
                    objCmd.Parameters.Add("V_REVIEWDATE", OracleDbType.Varchar2).Value = details.ReviewDate;
                    objCmd.Parameters.Add("V_EXTENSION", OracleDbType.Varchar2).Value = details.Extended;
                    objCmd.Parameters.Add("V_ACTIVE", OracleDbType.Varchar2).Value = details.Active;
                    objCmd.Parameters.Add("V_BY", OracleDbType.Varchar2).Value = details.FormFilledBy;
                    objCmd.Parameters.Add("V_ACTIVETAB", OracleDbType.Varchar2).Value = details.ActiveTab;
                    objCmd.Parameters.Add("V_APPROVERCODE", OracleDbType.Varchar2).Value = details.ApproverCode;
                    objCmd.Parameters.Add("V_APPROVALSTATUS", OracleDbType.Varchar2).Value = details.ApprovalStatus;
                    objCmd.Parameters.Add("V_APPROVALREMARKS", OracleDbType.Varchar2).Value = details.ApprovalRemarks;
                    objCmd.Parameters.Add("V_CONFIRMATIONREVIEWID", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;

                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();

                    ConfirmationID = Convert.ToString(objCmd.Parameters["V_CONFIRMATIONREVIEWID"].Value);
                    int result = Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());

                    if (result != 1)
                    {
                        tran.Rollback();
                        throw new Exception("Failed to update Confirmation. Transaction will rollback now.");
                    }


                    details.ConfirmationID = ConfirmationID;
                    foreach (Ratings r in details.Rating)
                    {

                        objCmd.Parameters.Clear();

                        objCmd.Parameters.Add("V_HRTRAITSRATINGID", OracleDbType.Varchar2).Value = r.RatingID;
                        objCmd.Parameters.Add("V_HRCONFIRMATIONREVIEWID", OracleDbType.Varchar2).Value = details.ConfirmationID;
                        objCmd.Parameters.Add("V_HRTRAITSATTRIBUTESID", OracleDbType.Varchar2).Value = r.TraitAttributeID;
                        objCmd.Parameters.Add("V_RATING", OracleDbType.Varchar2).Value = r.Rating;
                        objCmd.Parameters.Add("V_REMARKS", OracleDbType.Varchar2).Value = r.Remark;
                        objCmd.Parameters.Add("V_ACTIVE", OracleDbType.Varchar2).Value = details.Active;
                        objCmd.Parameters.Add("V_BY", OracleDbType.Varchar2).Value = details.FormFilledBy;

                        objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                        objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;

                        objCmd.CommandText = "PKG_HRCONFIRMATIONREVIEW.SPROC_UPDATE_RATING";
                        objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        objCmd.BindByName = true;
                        objCmd.ExecuteNonQuery();

                        int subResult = Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                        if (subResult != 1)
                        {
                            tran.Rollback();
                            throw new Exception("Failed to update rating. Transaction will rollback now.");
                        }
                    }

                    tran.Commit();

                }
                catch (Exception ex)
                {
                    try
                    {
                        tran.Rollback();
                    }
                    catch (OracleException x)
                    {
                        if (tran.Connection != null)
                        {
                            throw new Exception("An exception of type " + x.GetType() + " was encountered while attempting to roll back the transaction.");
                        }
                    }

                    throw new Exception(ex.ToString());
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


        public int UpdateHRStatus(String strAdempCode, String strProcessID, String strReviewFormID,
                                  String strProcessStatus, String strHRStatus, String strRemarks,
                                  String HRApprovedStatus, String strByUserCode)
        {
            String strErrMsg = String.Empty;
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRCONFIRMATIONREVIEW.SPROC_UPDATE_HRSTATUS";
                    objCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strAdempCode;
                    objCmd.Parameters.Add("V_PROCESSID", OracleDbType.Int32).Value = strProcessID;
                    objCmd.Parameters.Add("V_REVIEWFORMID", OracleDbType.Int32).Value = strReviewFormID;
                    objCmd.Parameters.Add("V_PROCESSSTATUS", OracleDbType.Int32).Value = strProcessStatus;
                    objCmd.Parameters.Add("V_HRSTATUS", OracleDbType.Int32).Value = strHRStatus;
                    objCmd.Parameters.Add("V_HRAPPROVEDSTATUS", OracleDbType.Varchar2).Value = HRApprovedStatus;
                    objCmd.Parameters.Add("V_REMARKS", OracleDbType.Varchar2).Value = strRemarks;
                    objCmd.Parameters.Add("V_BY", OracleDbType.Int32).Value = strByUserCode;

                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
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

        public int UpdateReviewAuthorities(String HrConfirmationReviewID, String Adempcode, String Reviewer1,
                                            String Reviewer2, String Reviewer3, String strByUserCode)
        {
            String strErrMsg = String.Empty;
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRCONFIRMATIONREVIEW.SPROC_UPDATE_REVIEWAUTH";
                    objCmd.Parameters.Add("V_HRCONFIRMATIONAUTHORITYID", OracleDbType.Varchar2).Value = HrConfirmationReviewID;
                    objCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = Adempcode;
                    objCmd.Parameters.Add("V_REVIEWER1", OracleDbType.Varchar2).Value = Reviewer1;
                    objCmd.Parameters.Add("V_REVIEWER2", OracleDbType.Varchar2).Value = Reviewer2;
                    objCmd.Parameters.Add("V_REVIEWER3", OracleDbType.Varchar2).Value = Reviewer3;
                    objCmd.Parameters.Add("V_BY", OracleDbType.Int32).Value = strByUserCode;

                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
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

        public int InsertProcessRequest(String strAdempCode)
        {
            String strErrMsg = String.Empty;
            //ConnectionString objCnStr = new ConnectionString(); ;
            string strCn = objCnStr.getConnectingString(); ;
            OracleCommand objCmd;

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                try
                {
                    objCmd = new OracleCommand();
                    objCmd.Connection = objCn;
                    string strSql = "PKG_HRCONFIRMATIONREVIEW.SPROC_INSERT_PROCESSREQUEST";
                    objCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strAdempCode;
                    objCmd.Parameters.Add("V_ACTIVE", OracleDbType.Int32).Value = 1;
                    objCmd.Parameters.Add("V_BY", OracleDbType.Int32).Value = 1;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 3000).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
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
    }
}

public class FormDetails
{
    public string ConfirmationID { get; set; }
    public string EmpCode { get; set; }
    public string FunctionDesignation { get; set; }
    public string ReviewDate { get; set; }
    public string Extended { get; set; }
    public string FormFilledBy { get; set; }
    public string Active { get; set; }
    public List<Ratings> Rating { get; set; }
    public string ApproverCode { get; set; }
    public string ApprovalStatus { get; set; }
    public string ApprovalRemarks { get; set; }
    public string ActiveTab { get; set; }
}

public class Ratings
{
    public string RatingID { get; set; }
    public string ConfirmationID { get; set; }
    public string TraitID { get; set; }
    public string TraitAttributeID { get; set; }
    public string Rating { get; set; }
    public string Remark { get; set; }
}

