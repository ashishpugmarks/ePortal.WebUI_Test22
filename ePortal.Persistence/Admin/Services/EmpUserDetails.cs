using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using Oracle.ManagedDataAccess.Client;

namespace ePortal.Persistence.Admin.Services
{
    public class EmpUserDetails: IEmpUserDetails
    {
        #region "Local Variables"
        //DataManagement oDataMgmt = new DataManagement();
        //ConnectionString objCnStr = new ConnectionString();

        private readonly IDataManagement oDataMgmt;
        private readonly IConnectionString objCnStr;

        OracleCommand oCmd;
        DataSet ds;
        DataTable Dt;
        OracleConnection objConn;
        string strConn;
        string strErrMsg = string.Empty;
        #endregion

        public EmpUserDetails(IDataManagement _oDataMgmt, IConnectionString _objCnStr)
        {
            oDataMgmt = _oDataMgmt;
            objCnStr = _objCnStr;
        }

        #region "Insert/Update Functions"
        /// <summary>
        /// ADD USER DETAIL FROM MYDETAIL PAGE 
        /// </summary>
        /// <param name="strExt"></param>
        /// <param name="strPhone"></param>
        /// <param name="strmobile"></param>
        /// <param name="stremailid"></param>
        /// <param name="strLoc"></param>
        /// <param name="strBlood"></param>
        /// <param name="strCreatedBy"></param>
        /// <param name="strJacketSize"></param>
        /// <param name="strTrouserSize"></param>
        /// <param name="strShoeSize"></param>
        /// <param name="strEmgNo"></param>
        /// <returns></returns>
        //public int AddUserDetail(string strExt, string strPhone, string strmobile, string stremailid, string strLoc,
        //    string strBlood, string strCreatedBy, string strJacketSize, string strTrouserSize, string strShoeSize,
        //     string strEmgNo, string strLockerNo, string strWinterJacketSize)

        //Change done by Aumento on 27032023 for add Personal Email Id=============================================================================================
        public int AddUserDetail(string strExt, string strPhone, string strmobile, string stremailid, string strLoc,
            string strBlood, string strCreatedBy, string strJacketSize, string strTrouserSize, string strShoeSize,
             string strEmgNo, string strLockerNo, string strWinterJacketSize, string strPeremailid, string strJacketHalfFull, string strMobileCompany, string strMobileModel, string strMobileHeight, string strMobileWidth, string strMobileThick, string strMobileCompany2, string strMobileModel2, string strMobileHeight2, string strMobileWidth2, string strMobileThick2,
             Int16 lngISDOB, Int16 lngISBloodgrp, Int16 lngISMarital, Int16 lngISEMER, Int16 lngISPerMob, Int16 lngISPerEmail, Int64 lngPerMob, string strMarital)
        //===============================================================================================================================
        {

            strConn = objCnStr.getConnectingString();
            using (objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    OracleCommand objCmd = new OracleCommand();

                    string strSql = "PKG_USERDETAIL.SPROC_AD_USERDETAILENTRY";

                    objCmd.Parameters.Add("EXTNO_IN", OracleDbType.Varchar2).Value = strExt;
                    objCmd.Parameters.Add("PHONENO_IN", OracleDbType.Varchar2).Value = strPhone;
                    objCmd.Parameters.Add("MOBILE_IN", OracleDbType.Varchar2).Value = strmobile;
                    objCmd.Parameters.Add("EMAILID_IN", OracleDbType.Varchar2).Value = stremailid;
                    objCmd.Parameters.Add("LOCATION_IN", OracleDbType.Varchar2).Value = strLoc;
                    objCmd.Parameters.Add("BLOOD_IN", OracleDbType.Varchar2).Value = strBlood;
                    objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32).Value = strCreatedBy;
                    objCmd.Parameters.Add("JACKETSIZE_IN", OracleDbType.Varchar2).Value = strJacketSize;
                    objCmd.Parameters.Add("WINTERJACKETSIZE_IN", OracleDbType.Varchar2).Value = strWinterJacketSize;//Added by Pradeep Singh on 21 July 2018
                                                                                                                    //Added by aumento on 27032023 for add personal email id=====================================
                    objCmd.Parameters.Add("PEREMAILID_IN", OracleDbType.Varchar2).Value = strPeremailid;
                    //===========================================================================================
                    //Added by aumento on 11042023 for dropdown Half/full========================================
                    objCmd.Parameters.Add("JACKETHALFFULL_IN", OracleDbType.Varchar2).Value = strJacketHalfFull;
                    //===========================================================================================
                    objCmd.Parameters.Add("TROUSERSIZE_IN", OracleDbType.Varchar2).Value = strTrouserSize;
                    objCmd.Parameters.Add("SHOESIZE_IN", OracleDbType.Varchar2).Value = strShoeSize;
                    //objCmd.Parameters.Add("SITE_IN", OracleDbType.Int32).Value = strSite;
                    objCmd.Parameters.Add("EMGNO_IN", OracleDbType.Varchar2).Value = strEmgNo; //VBansal 7 Feb 2010 Pass the Emergency No to save in Database
                    objCmd.Parameters.Add("MOBILE_COMPANY_IN", OracleDbType.Varchar2).Value = strMobileCompany;
                    objCmd.Parameters.Add("MOBILE_MODEL_IN", OracleDbType.Varchar2).Value = strMobileModel;
                    objCmd.Parameters.Add("MOBILE_HEIGHT_IN", OracleDbType.Varchar2).Value = strMobileHeight;
                    objCmd.Parameters.Add("MOBILE_WIDTH_IN", OracleDbType.Varchar2).Value = strMobileWidth;
                    objCmd.Parameters.Add("MOBILE_THICK_IN", OracleDbType.Varchar2).Value = strMobileThick;
                    objCmd.Parameters.Add("MOBILE_COMPANY2_IN", OracleDbType.Varchar2).Value = strMobileCompany2;
                    objCmd.Parameters.Add("MOBILE_MODEL2_IN", OracleDbType.Varchar2).Value = strMobileModel2;
                    objCmd.Parameters.Add("MOBILE_HEIGHT2_IN", OracleDbType.Varchar2).Value = strMobileHeight2;
                    objCmd.Parameters.Add("MOBILE_WIDTH2_IN", OracleDbType.Varchar2).Value = strMobileWidth2;
                    objCmd.Parameters.Add("MOBILE_THICK2_IN", OracleDbType.Varchar2).Value = strMobileThick2;
                    objCmd.Parameters.Add("LOCKERNO_IN", OracleDbType.Varchar2).Value = strLockerNo.Trim();//Alok singh 2 July 2011 pass the Locker No to save in database

                    objCmd.Parameters.Add("ISDOBCONSENT_IN", OracleDbType.Int16).Value = lngISDOB;
                    objCmd.Parameters.Add("ISMARITALCONSENT_IN", OracleDbType.Int16).Value = lngISMarital;
                    objCmd.Parameters.Add("ISBLOODCONSENT_IN", OracleDbType.Int16).Value = lngISBloodgrp;
                    objCmd.Parameters.Add("ISPERMOBCONSENT_IN", OracleDbType.Int16).Value = lngISPerMob;
                    objCmd.Parameters.Add("ISPEREMAILCONSENT_IN", OracleDbType.Int16).Value = lngISPerEmail;
                    objCmd.Parameters.Add("ISEMERCONSENT_IN", OracleDbType.Int16).Value = lngISEMER;
                    objCmd.Parameters.Add("PERMOBNO_IN", OracleDbType.Int64).Value = lngPerMob;
                    objCmd.Parameters.Add("MARITALSTS_IN", OracleDbType.Varchar2).Value = strMarital;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.Connection = objConn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    return Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());



                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (objConn != null)
                    {
                        objConn.Close();
                    }
                }

            }
        }

        /// <summary>
        /// UPDATE ASSOCIATE DATA FROM MY DETAIL PAGE
        /// </summary>
        /// <param name="strEmpModCode"></param>
        /// <param name="strDivision"></param>
        /// <param name="strDept"></param>
        /// <param name="strMod"></param>
        /// <param name="strSection"></param>
        /// <param name="strDesig"></param>
        /// <param name="strJobTitle"></param>
        /// <param name="strEcode"></param>
        /// <param name="strActive"></param>
        /// <param name="strTargetstartdate"></param>
        /// <param name="strTargetenddate"></param>
        /// <param name="strMidStartdate"></param>
        /// <param name="strMidEnddate"></param>
        /// <param name="strFullStartdate"></param>
        /// <param name="strFullEnddate"></param>
        /// <returns></returns>
        //public string updateEmpData(string strEmpModCode, string strDivision, string strDept, string strMod, string strSection,
        //    string strDesig, string strJobTitle, string strEcode, string strActive, string strTargetstartdate,
        //    string strTargetenddate, string strMidStartdate, string strMidEnddate, string strFullStartdate, string strFullEnddate)
        public string updateEmpData(string strEmpModCode, string strDivision, string strDept, string strMod, string strSection,
               string strDesig, string strJobTitle, string strEcode, string strActive)
        {
            strConn = objCnStr.getConnectingString();
            using (objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    OracleCommand objCmd = new OracleCommand();

                    string strSql = "PKG_USERDETAIL.SPROC_AD_UPDATE_EMPDATA";

                    objCmd.Parameters.Add("DIVCODE_IN", OracleDbType.Varchar2).Value = strDivision;
                    objCmd.Parameters.Add("DEPTCODE_IN", OracleDbType.Varchar2).Value = strDept;
                    objCmd.Parameters.Add("MODUCODE_IN", OracleDbType.Varchar2).Value = strMod;
                    objCmd.Parameters.Add("SECTCODE_IN", OracleDbType.Varchar2).Value = strSection;
                    objCmd.Parameters.Add("DESIGNATION_IN", OracleDbType.Varchar2).Value = strDesig;
                    objCmd.Parameters.Add("JOBTITLE_IN", OracleDbType.Varchar2).Value = strJobTitle;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpModCode;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = strActive;
                    objCmd.Parameters.Add("EMPMODCODE_IN", OracleDbType.Int32).Value = strEcode;
                    //objCmd.Parameters.Add("KRATARGETSTARTDATE_IN", OracleDbType.Varchar2).Value = strTargetstartdate;
                    //objCmd.Parameters.Add("KRATARGETENDDATE_IN", OracleDbType.Varchar2).Value = strTargetenddate;
                    //objCmd.Parameters.Add("KRAMIDSTARTDATE_IN_IN", OracleDbType.Varchar2).Value = strMidStartdate;
                    //objCmd.Parameters.Add("KRAMIDENDDATE_IN_IN", OracleDbType.Varchar2).Value = strMidEnddate;
                    //objCmd.Parameters.Add("KRAFULLSTARTDATE_IN_IN", OracleDbType.Varchar2).Value = strFullStartdate;
                    //objCmd.Parameters.Add("KRAFULLENDDATE_IN_IN", OracleDbType.Varchar2).Value = strFullEnddate;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.Connection = objConn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG_OUT"].Value);
                    return strErrMsg;


                }
                finally
                {
                    if (objConn != null)
                    {
                        objConn.Close();
                    }
                }

            }
        }

        public string updateStaffEmpData(string strEmpModCode, string strDivision, string strDept, string strMod, string strSection,
           string strDesig, string strJobTitle, string strEcode, string strActive)
        {
            strConn = objCnStr.getConnectingString();
            using (objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    OracleCommand objCmd = new OracleCommand();

                    string strSql = "PKG_USERDETAIL.SPROC_STAFF_UPDATE_EMPDATA";

                    objCmd.Parameters.Add("DIVCODE_IN", OracleDbType.Varchar2).Value = strDivision;
                    objCmd.Parameters.Add("DEPTCODE_IN", OracleDbType.Varchar2).Value = strDept;
                    objCmd.Parameters.Add("MODUCODE_IN", OracleDbType.Varchar2).Value = strMod;
                    objCmd.Parameters.Add("SECTCODE_IN", OracleDbType.Varchar2).Value = strSection;
                    objCmd.Parameters.Add("DESIGNATION_IN", OracleDbType.Varchar2).Value = strDesig;
                    objCmd.Parameters.Add("JOBTITLE_IN", OracleDbType.Varchar2).Value = strJobTitle;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpModCode;
                    objCmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = strActive;
                    objCmd.Parameters.Add("EMPMODCODE_IN", OracleDbType.Int32).Value = strEcode;
                    //objCmd.Parameters.Add("KRATARGETSTARTDATE_IN", OracleDbType.Varchar2).Value = strTargetstartdate;
                    //objCmd.Parameters.Add("KRATARGETENDDATE_IN", OracleDbType.Varchar2).Value = strTargetenddate;
                    //objCmd.Parameters.Add("KRAMIDSTARTDATE_IN_IN", OracleDbType.Varchar2).Value = strMidStartdate;
                    //objCmd.Parameters.Add("KRAMIDENDDATE_IN_IN", OracleDbType.Varchar2).Value = strMidEnddate;
                    //objCmd.Parameters.Add("KRAFULLSTARTDATE_IN_IN", OracleDbType.Varchar2).Value = strFullStartdate;
                    //objCmd.Parameters.Add("KRAFULLENDDATE_IN_IN", OracleDbType.Varchar2).Value = strFullEnddate;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.Connection = objConn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(objCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(objCmd.Parameters["ERRMSG_OUT"].Value);
                    return strErrMsg;


                }
                finally
                {
                    if (objConn != null)
                    {
                        objConn.Close();
                    }
                }

            }
        }


        /// <summary>
        /// FOR THE USER RIGHTS VALIDATION
        /// </summary>
        /// <param name="strEmpCode"></param>
        /// <param name="strUrl"></param>
        /// <returns></returns>
        public int GetCount(string strEmpCode, string strUrl)
        {
            strConn = objCnStr.getConnectingString();
            int countNo;
            using (objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    OracleCommand objCmd = new OracleCommand();

                    string strSql = "PKG_USERDETAIL.SPROC_UserRights_Validate";

                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("URL_IN", OracleDbType.Varchar2).Value = strUrl;
                    objCmd.Parameters.Add("ISVALID", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.Connection = objConn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;

                    int test = objCmd.ExecuteNonQuery();

                    countNo = Convert.ToInt32(objCmd.Parameters["ISVALID"].Value.ToString());
                }
                finally
                {
                    if (objConn != null)
                    {
                        objConn.Close();
                    }
                }
                return Convert.ToInt32(countNo);
            }
        }

        public int UpdateUserPancardno(string strEmpcode, string strPancardno, string strmobileno)
        {

            strConn = objCnStr.getConnectingString();
            using (objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    OracleCommand objCmd = new OracleCommand();

                    string strSql = "PKG_USERDETAIL.SPROC_AD_UPDATE_EMPPANDATA";

                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpcode;
                    objCmd.Parameters.Add("PANCARDNO_IN", OracleDbType.Varchar2).Value = strPancardno.Trim();
                    objCmd.Parameters.Add("MOBILENO_IN", OracleDbType.Varchar2).Value = strmobileno;
                    objCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.Connection = objConn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    return Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());



                }
                finally
                {
                    if (objConn != null)
                    {
                        objConn.Close();
                    }
                }

            }
        }

        public Tuple<Int32, string> UpdateFamilyDeclaration(string strEmpcode, string strName, string strRelation, string strAddedby, Int32 lngISPastCurr)
        {

            strConn = objCnStr.getConnectingString();
            using (objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    OracleCommand objCmd = new OracleCommand();

                    string strSql = "PKG_USERDETAIL.SPROC_EMPFAMILYDECLARATION_SET";

                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = Convert.ToInt32(strEmpcode);
                    objCmd.Parameters.Add("FNAME_IN", OracleDbType.Varchar2).Value = strName;
                    objCmd.Parameters.Add("RELATIONSHIP_IN", OracleDbType.Varchar2).Value = strRelation;
                    objCmd.Parameters.Add("ISPASTORCURR_IN", OracleDbType.Int32).Value = lngISPastCurr;
                    objCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32).Value = Convert.ToInt32(strAddedby);
                    objCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.Connection = objConn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    Int32 resulout = Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                    strErrMsg = objCmd.Parameters["ERRMSG_OUT"].Value.ToString();

                    return new Tuple<Int32, string>(resulout, strErrMsg);

                }
                finally
                {
                    if (objConn != null)
                    {
                        objConn.Close();
                    }
                }

            }
        }

        public Int32 DeleteFamilyDeclaration(string strEmpcode, string headerid)
        {

            strConn = objCnStr.getConnectingString();
            using (objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    string strSql = "PKG_USERDETAIL.SPROC_EMPFAMILYDEC_DELETE";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = Convert.ToInt32(strEmpcode);
                    objCmd.Parameters.Add("HDRID_IN", OracleDbType.Int32).Value = Convert.ToInt32(headerid);
                    objCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.Connection = objConn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    Int32 resulout = Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                    strErrMsg = objCmd.Parameters["ERRMSG_OUT"].Value.ToString();

                    return resulout;
                }
                finally
                {
                    if (objConn != null)
                    {
                        objConn.Close();
                    }
                }

            }
        }

        public Int32 SkipUserdetail(string strEmpcode)
        {

            strConn = objCnStr.getConnectingString();
            using (objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    OracleCommand objCmd = new OracleCommand();
                    string strSql = "PKG_USERDETAIL.SPROC_EMPUSERDTLSKIP_UPDATE";
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = Convert.ToInt32(strEmpcode);
                    objCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.CommandText = strSql;
                    objCmd.Connection = objConn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;
                    objCmd.ExecuteNonQuery();
                    Int32 resulout = Convert.ToInt32(objCmd.Parameters["RESULT_OUT"].Value.ToString());
                    strErrMsg = objCmd.Parameters["ERRMSG_OUT"].Value.ToString();

                    return resulout;
                }
                finally
                {
                    if (objConn != null)
                    {
                        objConn.Close();
                    }
                }

            }
        }

        #endregion

        #region "Data Selection Functions"
        /// <summary>
        /// SHOW ALL THE DIVISION
        /// </summary>
        /// <returns></returns>
        public DataSet SelectDivision()
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandText = "PKG_USERDETAIL.SPROC_AD_DIVISION";
            oCmd.Parameters.Add("CUR_DIVISION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }

        /// <summary>
        /// SHOW ALL THE DEPARTMENT
        /// </summary>
        /// <returns></returns>
        public DataSet SelectDepartment()
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandText = "PKG_USERDETAIL.SPROC_AD_DEPARTMENT";
            oCmd.Parameters.Add("CUR_DEPARTMENT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }

        /// <summary>
        /// SHOW ALL THE MODULES
        /// </summary>
        /// <returns></returns>
        public DataSet SelectModule()
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandText = "PKG_USERDETAIL.SPROC_AD_MODULE";
            oCmd.Parameters.Add("CUR_MODULE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }

        /// <summary>
        /// SHOW ALL THE SECTION
        /// </summary>
        /// <returns></returns>
        public DataSet SelectSection()
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandText = "PKG_USERDETAIL.SPROC_AD_SECTION";
            oCmd.Parameters.Add("CUR_SECTION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }

        /// <summary>
        /// SHOW ALL THE DESIGNATION
        /// </summary>
        /// <returns></returns>
        public DataSet SelectDesignation()
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandText = "PKG_USERDETAIL.SPROC_AD_DESIGNATION";
            oCmd.Parameters.Add("CUR_DESIGNATION", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }

        /// <summary>
        /// SELECT USER TYPE FOR INDEX PAGE (ASSOCIATE/ADMIN LOGIN)
        /// </summary>
        /// <returns></returns>
        public DataSet SelectUserType()
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandText = "SPROC_USERTYPE_GET";
            oCmd.Parameters.Add("REFCSR_USERTYPE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }

        public DataSet SelectProjectName()
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandText = "SPROC_PROJECTNAME_GET";
            oCmd.Parameters.Add("CUR_PROJECTNAME", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }

        //Below Function Added By Aumento on 18042023
        public int GetUserSiteId(string strEmpCode)
        {
            strConn = objCnStr.getConnectingString();
            int countNo;
            using (objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    OracleCommand objCmd = new OracleCommand();

                    string strSql = "PKG_USERDETAIL.SPROC_GetUserSiteID";

                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    objCmd.Parameters.Add("ISVALID", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;

                    objCmd.CommandText = strSql;
                    objCmd.Connection = objConn;
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    objCmd.BindByName = true;

                    int test = objCmd.ExecuteNonQuery();

                    countNo = Convert.ToInt32(objCmd.Parameters["ISVALID"].Value.ToString());
                }
                finally
                {
                    if (objConn != null)
                    {
                        objConn.Close();
                    }
                }
                return Convert.ToInt32(countNo);
            }
        }

        public DataTable GetEmployeeDeclaration(string strEmpcode)
        {
            oCmd = new OracleCommand();
            DataTable ds = new DataTable();
            oCmd.CommandText = "PKG_USERDETAIL.SPROC_GET_EMPFAMILYDECLARATION";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = strEmpcode;
            oCmd.Parameters.Add("CUR_TRANS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataTable(oCmd);
            return (ds);
        }
        #endregion

    }
}
