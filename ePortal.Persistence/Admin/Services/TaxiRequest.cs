using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using ePortal.Shared;
using ePortal.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;




namespace ePortal.Persistence.Admin.Services
{
    public class TaxiRequest : ITexiRequest
    {
        #region "Local Variables"
        //DataManagement oDataMgmt = new DataManagement();
        //ConnectionString

        private readonly IDataManagement oDataMgmt;
        private readonly IConnectionString objConn;
        private readonly ICommonFunctions _objCommon;
        
        OracleCommand oCmd;
        DataSet ds;
        DataTable Dt;
        //OracleConnection objConn;
        string strConn;
        string errMsg = string.Empty;
        #endregion

        public TaxiRequest(IDataManagement _oDataMgmt, IConnectionString _objConn, ICommonFunctions objCommon)
        {
            oDataMgmt = _oDataMgmt;
            objConn = _objConn;
            _objCommon = objCommon;
        }

        #region "Data insert/update/get"
        /// <summary>
        /// RETURN DATA SET FOR MANAGE VEHICLE REQUEST PAGE
        /// </summary>
        /// <param name="strEmpCode"></param>
        /// <returns></returns>
        public List<SelectListItem> ManageVehicleRequest(string strEmpCode)
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_AD_PENDINGREQUEST";
            oCmd.Parameters.Add("CUR_PENDINGVEHICLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
            ds = oDataMgmt.GetDataSet(oCmd);
            var list = new List<SelectListItem>();
            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new SelectListItem
                    {
                        Value = row["VehicleID"].ToString(),       // Replace with actual column name
                        Text = row["VehicleName"].ToString()       // Replace with actual column name
                    });
                }
            }
            return list;
        }

        /// <summary>
        /// RETURN DATA SET FOR MANAGET REQUEST HISTORY PAGE
        /// </summary>
        /// <param name="strEmpCode"></param>
        /// <returns></returns>
        public DataSet ManageVehicleRequest1(string strEmpCode)
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_AD_PENDINGREQUEST1";
            oCmd.Parameters.Add("CUR_PENDINGVEHICLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        /// <summary>
        /// ADD VEHICALE REQUEST INTO DATABASE
        /// </summary>
        /// <param name="strEmpCode"></param>
        /// <param name="strExtNo"></param>
        /// <param name="strPhoneNo"></param>
        /// <param name="strDateFrom"></param>
        /// <param name="strDateTo"></param>
        /// <param name="strReportingPlace"></param>
        /// <param name="strReportingTime"></param>
        /// <param name="strPurposeVisit"></param>
        /// <param name="strPlaceVisit"></param>
        /// <param name="strShift"></param>
        /// <param name="strNoofPerson"></param>
        /// <param name="strRemarks"></param>
        /// <param name="strAppCode"></param>
        /// <returns></returns>
        public string AddVechilerequest(string strEmpCode, string strExtNo, string strPhoneNo, string strDateFrom,
                                        string strDateTo, string strReportingPlace, string strReportingTime, string strPurposeVisit,
                                        string strPlaceVisit, string strShift, string strNoofPerson, string strRemarks, string strAppCode, string strsiteid)
        {

            var strConn = objConn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand oCmd = new OracleCommand();
                    oCmd.Connection = objCn;
                    oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_AD_VEHICLEREUQESTENTRY";
                    oCmd.CommandType = CommandType.StoredProcedure;
                    oCmd.BindByName = true;
                    oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = strEmpCode;
                    oCmd.Parameters.Add("EXTENSION_IN", OracleDbType.Int64).Value = strExtNo;
                    oCmd.Parameters.Add("PHONENO_IN", OracleDbType.Int64).Value = strPhoneNo;
                    oCmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = strDateTo;
                    oCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = strDateFrom;
                    oCmd.Parameters.Add("REPORTPLACE_IN", OracleDbType.Varchar2).Value = strReportingPlace;
                    oCmd.Parameters.Add("REPORTTIME_IN", OracleDbType.Varchar2).Value = strReportingTime;
                    oCmd.Parameters.Add("PURPOSEVISIT_IN", OracleDbType.Varchar2).Value = strPurposeVisit;
                    oCmd.Parameters.Add("PLACEVISIT_IN", OracleDbType.Varchar2).Value = strPlaceVisit;
                    oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    oCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    oCmd.Parameters.Add("SHIFT_IN", OracleDbType.Varchar2).Value = strShift;
                    oCmd.Parameters.Add("NOOFPERSON_IN", OracleDbType.Int64).Value = strNoofPerson;
                    oCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = strRemarks;
                    oCmd.Parameters.Add("APPCODE_IN", OracleDbType.Int64).Value = strAppCode;
                    oCmd.Parameters.Add("SYSITEID_IN", OracleDbType.Int64).Value = strsiteid;
                    oCmd.Parameters.Add("ISAPPROVALREQUIRED", OracleDbType.Int64).Direction = ParameterDirection.Output;

                    oCmd.ExecuteNonQuery();
                    errMsg = Convert.ToString(oCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(oCmd.Parameters["ISAPPROVALREQUIRED"].Value) + "#" + Convert.ToString(oCmd.Parameters["ERRMSG"].Value);
                    return errMsg;

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
        /// PENDING VEHICLE REQUEST
        /// </summary>
        /// <param name="strSupEmpCode"></param>
        /// <returns></returns>
        public DataSet PendingVehicleRequest(string strSupEmpCode)
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_AD_APPROVEDVEHICLEREUQ";
            oCmd.Parameters.Add("CUR_PENDINGAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("SUPEMPCODE_IN", OracleDbType.Int32).Value = strSupEmpCode;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        /// <summary>
        /// EDIT TAXI APPROVAL
        /// </summary>
        /// <param name="Requestid"></param>
        /// <param name="Ecode"></param>
        /// <returns></returns>
        public DataSet EditTaxiApproval(string Requestid, string Ecode)
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_AD_TAXIAPPROVALBYID";

            oCmd.Parameters.Add("CUR_TRANS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("TRANSID_IN", OracleDbType.Int32).Value = Requestid;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = Ecode;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        /// <summary>
        /// UPDATE TAXI APPROVAL
        /// </summary>
        /// <param name="strSupervisorEmpCode"></param>
        /// <param name="strID"></param>
        /// <param name="strApprovalStatus"></param>
        /// <param name="strRemarks"></param>
        /// <param name="strsupsupempcode"></param>
        /// <param name="stroperation"></param>
        /// <returns></returns>
        public string UpdateTaxiApproval(string strSupervisorEmpCode, string strID, string strApprovalStatus, string strRemarks,
            string strsupsupempcode, string stroperation)
        {

            strConn = objConn.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    oCmd = new OracleCommand();
                    oCmd.Connection = objCn;
                    oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_AD_UPDATETAXIAPPROVAL";
                    oCmd.CommandType = CommandType.StoredProcedure;
                    oCmd.BindByName = true;
                    oCmd.Parameters.Add("TRANSID_IN", OracleDbType.Int32).Value = strID;
                    oCmd.Parameters.Add("SUPEMPCODE_IN", OracleDbType.Int32).Value = strSupervisorEmpCode;
                    oCmd.Parameters.Add("APPROVALSTATUS_IN", OracleDbType.Int32).Value = strApprovalStatus;
                    oCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = strRemarks;
                    oCmd.Parameters.Add("SUPSUPEMPCODE_IN", OracleDbType.Varchar2).Value = strsupsupempcode;
                    oCmd.Parameters.Add("OPERATION_IN", OracleDbType.Varchar2).Value = stroperation;
                    oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    oCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 5000).Direction = ParameterDirection.Output;

                    oCmd.ExecuteNonQuery();
                    errMsg = Convert.ToString(oCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(oCmd.Parameters["ERRMSG"].Value);
                    return errMsg;

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
        /// UPDATE TAXI REQUEST APPROVAL
        /// </summary>
        /// <param name="strAdminEmpCode"></param>
        /// <param name="strID"></param>
        /// <param name="strApprovalStatus"></param>
        /// <param name="strRemarks"></param>
        /// <param name="strTaxiNo"></param>
        /// <param name="strVendorName"></param>
        /// <param name="strAmt"></param>
        /// <returns></returns>
        public string UpdateAdminTaxiApproval(string strAdminEmpCode, string strID, string strApprovalStatus, string strRemarks,
            string strTaxiNo, string strVendorName, string strAmt, string strtaxitype)
        {

            strConn = objConn.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    oCmd = new OracleCommand();
                    oCmd.Connection = objCn;
                    oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_AD_ADMINTAXIAPPROVAL";
                    oCmd.CommandType = CommandType.StoredProcedure;
                    oCmd.BindByName = true;
                    oCmd.Parameters.Add("TRANSID_IN", OracleDbType.Int32).Value = strID;
                    oCmd.Parameters.Add("SUPEMPCODE_IN", OracleDbType.Int32).Value = strAdminEmpCode;
                    oCmd.Parameters.Add("APPROVALSTATUS_IN", OracleDbType.Int32).Value = strApprovalStatus;
                    oCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = strRemarks;
                    oCmd.Parameters.Add("TAXINO_IN", OracleDbType.Varchar2).Value = strTaxiNo;
                    oCmd.Parameters.Add("vendorid_in", OracleDbType.Int32).Value = strVendorName;
                    oCmd.Parameters.Add("AMMOUNT_IN", OracleDbType.Varchar2).Value = strAmt;
                    oCmd.Parameters.Add("TAXITYPE_IN", OracleDbType.Varchar2).Value = strtaxitype;

                    oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    oCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 5000).Direction = ParameterDirection.Output;

                    oCmd.ExecuteNonQuery();
                    errMsg = Convert.ToString(oCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(oCmd.Parameters["ERRMSG"].Value);
                    return errMsg;

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
        /// ADMIN VEHICLE REQUEST
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public DataSet AdminVehicleRequest(string Type)
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_AD_VEHICLEAPPROVAL";
            oCmd.Parameters.Add("CUR_PENDINGAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("REQUESTTYPE_IN", OracleDbType.Varchar2).Value = Type;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        /// <summary>
        /// DATA OF VEHICLE REQUEST BY REQUEST ID
        /// </summary>
        /// <param name="strTransID"></param>
        /// <returns></returns>
        public VehicleReleased VehicleRequestById(string strTransID, string Did)
        {
            VehicleReleased data=new VehicleReleased();
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_AD_ADMIN_TAXI";
            oCmd.Parameters.Add("CUR_BYID_ADMIN", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("TRANSID_IN", OracleDbType.Int32).Value = strTransID;
            oCmd.Parameters.Add("CUR_APPROVAL_INFO", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            ds = oDataMgmt.GetDataSet(oCmd);
            DataView objdtview = new DataView();
           
            DataSet objTaxiDs = new DataSet();
            ds.Tables[0].DefaultView.RowFilter = "ADVECHICLEDETAILID=" + Did.ToString();
            objdtview = ds.Tables[0].DefaultView;

            //TaxiRequest objTaxi = new TaxiRequest();
            //DataSet objTaxiDs = objTaxi.VehicleRequestById(Request.QueryString["id"]);
            //FILL TAXI REQUEST DETAILS
            objTaxiDs.Tables.Add(objdtview.ToTable());
            if (objTaxiDs.Tables[0].Rows.Count > 0)
            {
                 data = new VehicleReleased
                {


                    Name = objTaxiDs.Tables[0].Rows[0][2].ToString(),
                    Designation = objTaxiDs.Tables[0].Rows[0][7].ToString(),
                    Dept = objTaxiDs.Tables[0].Rows[0][5].ToString(),
                    Email = objTaxiDs.Tables[0].Rows[0][3].ToString(),
                    Ext = objTaxiDs.Tables[0].Rows[0][8].ToString(),
                    Phoneno = objTaxiDs.Tables[0].Rows[0][12].ToString(),
                    FromDate = objTaxiDs.Tables[0].Rows[0][9].ToString(),
                    ToDate = objTaxiDs.Tables[0].Rows[0][10].ToString(),
                    Time = objTaxiDs.Tables[0].Rows[0][13].ToString(),
                    Purpose = objTaxiDs.Tables[0].Rows[0][14].ToString(),
                    Place = objTaxiDs.Tables[0].Rows[0][15].ToString(),
                    //strAdminApp = objTaxiDs.Tables[0].Rows[0][16].ToString(),
                    ReportingSite = objTaxiDs.Tables[0].Rows[0]["SITE_NAME"].ToString(),
                    ReportingPlace = objTaxiDs.Tables[0].Rows[0]["address"].ToString(),
                    SYSITEID = objTaxiDs.Tables[0].Rows[0]["SYSITEID"].ToString(),
                    RELEASEMETERREADING = objTaxiDs.Tables[0].Rows[0]["RELEASEMETERREADING"].ToString(),
                    METERREADING = objTaxiDs.Tables[0].Rows[0]["METERREADING"].ToString(),
                    BUSROUTEID = objTaxiDs.Tables[0].Rows[0]["RELEASEDAT"].ToString(),
                    //if (ddlboardingpoint.Items.FindByValue(strreleasrat) != null)
                    //{
                    //    ddlboardingpoint.SelectedValue = strreleasrat,
                    //}

                    VISITE_PLACE = objTaxiDs.Tables[0].Rows[0]["VISITE_PLACE"].ToString(),
                    TAXICONDITIONSTATUS = objTaxiDs.Tables[0].Rows[0]["TAXICONDITIONSTATUS"].ToString(),
                    REMARKS = objTaxiDs.Tables[0].Rows[0]["REQUESTERREMARKS"].ToString(),
                    SLIPNO = objTaxiDs.Tables[0].Rows[0]["SLIPNO"].ToString(),
                    ADVEHICLEREQUESTID = strTransID,
                    ADVECHICLEDETAILID = Did
                };
                
            }
            return data;
        }

        /// <summary>
        /// GET ALL THE TAXI REQUEST DATA
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public DataSet get_AllTaxiRequest(string Type)
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_TAXI_EXCEL_EXPORT";
            oCmd.Parameters.Add("CUR_EXCEL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("REQUESTTYPE_IN", OracleDbType.Varchar2).Value = Type;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        /// <summary>
        /// UPDATE TAXI REQUEST FROM EDIT PAGE
        /// </summary>
        /// <param name="strShift"></param>
        /// <param name="strExtNo"></param>
        /// <param name="strPhoneNo"></param>
        /// <param name="strFrmdate"></param>
        /// <param name="strTodate"></param>
        /// <param name="strReportingPlace"></param>
        /// <param name="strReportingTime"></param>
        /// <param name="strPurposeVisit"></param>
        /// <param name="strPlaceVisit"></param>
        /// <param name="strNoofPerson"></param>
        /// <param name="strRemarks"></param>
        /// <param name="strAppAuth"></param>
        /// <param name="strID"></param>
        /// <returns></returns>
        public string UpdateTaxiRequest(string strShift, string strExtNo, string strPhoneNo, string strFrmdate,
            string strTodate, string strReportingPlace, string strReportingTime, string strPurposeVisit,
            string strPlaceVisit, string strNoofPerson, string strRemarks, string strAppAuth, string strID)
        {
            strConn = objConn.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    oCmd = new OracleCommand();
                    oCmd.Connection = objCn;
                    oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_AD_UPDATETAXIREQUEST";
                    oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    oCmd.BindByName = true;
                    oCmd.Parameters.Add("TRANSID_IN", OracleDbType.Int32).Value = strID;
                    oCmd.Parameters.Add("SHIFT_IN", OracleDbType.Varchar2).Value = strShift;
                    oCmd.Parameters.Add("EXTNO_IN", OracleDbType.Int32).Value = strExtNo;
                    oCmd.Parameters.Add("PHONENO_IN", OracleDbType.Varchar2).Value = strPhoneNo;
                    oCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = strFrmdate;
                    oCmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = strTodate;
                    oCmd.Parameters.Add("REPORTINGPLACE_IN", OracleDbType.Varchar2).Value = strReportingPlace;
                    oCmd.Parameters.Add("REPORTINGTIME_IN", OracleDbType.Varchar2).Value = strReportingTime;
                    oCmd.Parameters.Add("VISITPURPOSE_IN", OracleDbType.Varchar2).Value = strPurposeVisit;
                    oCmd.Parameters.Add("VISITPLACE_IN", OracleDbType.Varchar2).Value = strPlaceVisit;
                    oCmd.Parameters.Add("NOOFPERSON_IN", OracleDbType.Int32).Value = strNoofPerson;
                    oCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = strRemarks;
                    oCmd.Parameters.Add("APPAUTHCODE_IN", OracleDbType.Int32).Value = strAppAuth;
                    
                    oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    oCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 5000).Direction = ParameterDirection.Output;

                    oCmd.ExecuteNonQuery();
                    errMsg = Convert.ToString(oCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(oCmd.Parameters["ERRMSG"].Value);
                    return errMsg;

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
        /// CANCEL TAXI REQUEST
        /// </summary>
        /// <param name="strID"></param>
        /// <param name="strCancelRequest"></param>
        /// <returns></returns>
        public string CancelTaxiRequest(string strID, string strCancelRequest)
        {

            strConn = objConn.getConnectingString();

            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    oCmd = new OracleCommand();
                    oCmd.Connection = objCn;
                    oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_AD_CANCELTAXIREQUEST";
                    oCmd.CommandType = CommandType.StoredProcedure;
                    oCmd.BindByName = true;
                    oCmd.Parameters.Add("TRANSID_IN", OracleDbType.Int32).Value = strID;
                    oCmd.Parameters.Add("CANCELREQUEST_IN", OracleDbType.Varchar2).Value = strCancelRequest;

                    oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    oCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 5000).Direction = ParameterDirection.Output;

                    oCmd.ExecuteNonQuery();
                    errMsg = Convert.ToString(oCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(oCmd.Parameters["ERRMSG"].Value);
                    return errMsg;

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
        /// GET TAXI RECORDS
        /// </summary>
        /// <param name="struserid"></param>
        /// <param name="strfromdate"></param>
        /// <param name="strtodate"></param>
        /// <returns></returns>
        public DataSet GetTaxiRecord(string struserid, string strfromdate, string strtodate)
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_TAXI_RECORD";
            oCmd.Parameters.Add("CUR_TRANS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("USERID_IN", OracleDbType.Int32).Value = struserid;
            oCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Date).Value = strfromdate;
            oCmd.Parameters.Add("TODATE_IN", OracleDbType.Date).Value = strtodate;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        /// <summary>
        /// CHECK THE STATUS OF REQUEST
        /// </summary>
        /// <param name="struserid"></param>
        /// <returns></returns>
        public int CheckStatus(string struserid)
        {

            strConn = objConn.getConnectingString();
            using (OracleConnection objConn = new OracleConnection())
            {
                objConn.ConnectionString = strConn;
                try
                {
                    objConn.Open();
                    oCmd = new OracleCommand();
                    oCmd.Parameters.Add("USERID_IN", OracleDbType.Int32).Value = struserid;
                    oCmd.Parameters.Add("COUNT_OUT", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_USER_STATUS";
                    oCmd.Connection = objConn;
                    oCmd.CommandType = CommandType.StoredProcedure;
                    oCmd.BindByName = true;
                    oCmd.ExecuteNonQuery();
                    return Convert.ToInt32(oCmd.Parameters["COUNT_OUT"].Value.ToString());
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
        /// GET DIVISION HEAD FROM DATABASE
        /// </summary>
        /// <param name="strrequesterempcode"></param>
        /// <returns></returns>
        public DataSet getDivisionHead(string strrequesterempcode)
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_DIVHEAD_GET";
            oCmd.Parameters.Add("CUR_TRANS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("REQUESTEREMPCODE_IN", OracleDbType.Int32).Value = strrequesterempcode;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        /// <summary>
        /// GET ALL DIVISION HEAD DATA
        /// </summary>
        /// <param name="strrequesterempcode"></param>
        /// <returns></returns>
        public DataSet get_AllDivHead(string strrequesterempcode)
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_ALLDIVHEAD_GET";
            oCmd.Parameters.Add("CUR_TRANS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("REQUESTEREMPCODE_IN", OracleDbType.Int32).Value = strrequesterempcode;
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }

        /// <summary>
        /// GET TAXI VENDOR LIST
        /// </summary>
        /// <returns></returns>
        public DataTable GetVendor()
        {

            oCmd = new OracleCommand();
            Dt = new DataTable();
            oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_VENDORNAME_GET";
            oCmd.Parameters.Add("CUR_TRANS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            Dt = oDataMgmt.GetDataTable(oCmd);
            return Dt;
        }

        /// <summary>
        /// ADD VEHICLE REQUEST BY ADMIN 
        /// </summary>
        /// <param name="strEmpCode"></param>
        /// <param name="strExtNo"></param>
        /// <param name="strPhoneNo"></param>
        /// <param name="strDateFrom"></param>
        /// <param name="strDateTo"></param>
        /// <param name="strReportingPlace"></param>
        /// <param name="strReportingTime"></param>
        /// <param name="strPurposeVisit"></param>
        /// <param name="strPlaceVisit"></param>
        /// <param name="strShift"></param>
        /// <param name="strNoofPerson"></param>
        /// <param name="strRemarks"></param>
        /// <returns></returns>
        public string AdminsideAddVechilerequest(string strEmpCode, string strExtNo, string strPhoneNo, string strDateFrom,
            string strDateTo, string strReportingPlace, string strReportingTime, string strPurposeVisit,
            string strPlaceVisit, string strShift, string strNoofPerson, string strRemarks, string strsite, string strappcode)
        {

            strConn = objConn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                string strErrMsg;
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand oCmd = new OracleCommand();
                    oCmd.Connection = objCn;
                    oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_AD_ADMINVEHICLEREUQEST";
                    oCmd.CommandType = CommandType.StoredProcedure;
                    oCmd.BindByName = true;
                    oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
                    oCmd.Parameters.Add("EXTENSION_IN", OracleDbType.Varchar2).Value = strExtNo;
                    oCmd.Parameters.Add("PHONENO_IN", OracleDbType.Varchar2).Value = strPhoneNo;
                    oCmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = strDateTo;
                    oCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = strDateFrom;
                    oCmd.Parameters.Add("REPORTPLACE_IN", OracleDbType.Varchar2).Value = strReportingPlace;
                    oCmd.Parameters.Add("REPORTTIME_IN", OracleDbType.Varchar2).Value = strReportingTime;
                    oCmd.Parameters.Add("PURPOSEVISIT_IN", OracleDbType.Varchar2).Value = strPurposeVisit;
                    oCmd.Parameters.Add("PLACEVISIT_IN", OracleDbType.Varchar2).Value = strPlaceVisit;
                    oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    oCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    oCmd.Parameters.Add("SHIFT_IN", OracleDbType.Varchar2).Value = strShift;
                    oCmd.Parameters.Add("NOOFPERSON_IN", OracleDbType.Int32).Value = strNoofPerson;
                    oCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = strRemarks;
                    oCmd.Parameters.Add("SITE_IN", OracleDbType.Varchar2).Value = strsite;
                    oCmd.Parameters.Add("APPCODE_IN", OracleDbType.Varchar2).Value = strappcode;


                    oCmd.ExecuteNonQuery();
                    strErrMsg = Convert.ToString(oCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(oCmd.Parameters["ERRMSG"].Value);
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
        /// GET DATA OF TAXI APPROVAL HISTORY FOR DEPARTMENT MANAGER
        /// </summary>
        /// <param name="strEmpCode"></param>
        /// <returns></returns>
        public DataSet VehicleApprovalHistory(string strEmpCode)
        {
            oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_AD_VEHICLEAPPHISTORY";
            oCmd.Parameters.Add("CUR_PENDINGVEHICLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpCode;
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            ds = oDataMgmt.GetDataSet(oCmd);
            return ds;
        }


        /// <summary>
        /// CHECK USER IS ELIGIBLE FOR TAXI REQUEST OR NOT
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public string isValidForTaxiBooking(string userID)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            string strStatus = string.Empty;
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_TAXIBOOKING_VALID";
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = userID;
            oCmd.Parameters.Add("CUR_TAXIBOOKING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            if (dt.Rows.Count > 0)
                strStatus = "YES";
            else
                strStatus = "NO";
            return strStatus;
        }

        /// <summary>
        /// ADDED NEW FUNCTION FOR TAXI APPROVAL AUTHIRITY LIST ON 18 MAY 2010 BY SANTOSH
        /// </summary>
        /// <param name="strEmpcode"></param>
        /// <returns></returns>
        public DataTable get_TaxiApprovalAuthList(string strEmpcode)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_GET_TAXIAPP";
            oCmd.Parameters.Add("CUR_TAXIAPP", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpcode;
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);

            
            return dt;
            
        }

        public string AddVechiledetail(string strvehiclerequestid, string strvehicleno, string strmeterreading, string strdrivername,
                                         string strdriverno, string strAppCode)
        {

            strConn = objConn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand oCmd = new OracleCommand();
                    oCmd.Connection = objCn;
                    oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_AD_VEHICLEDETAILENTRY";
                    oCmd.CommandType = CommandType.StoredProcedure;
                    oCmd.BindByName = true;
                    oCmd.Parameters.Add("VEHICLEID_IN", OracleDbType.Varchar2).Value = strvehiclerequestid;
                    oCmd.Parameters.Add("VEHICLENO_IN", OracleDbType.Varchar2).Value = strvehicleno;
                    oCmd.Parameters.Add("METERREADING_IN", OracleDbType.Varchar2).Value = strmeterreading;
                    oCmd.Parameters.Add("DRIVERNAME_IN", OracleDbType.Varchar2).Value = strdrivername;
                    oCmd.Parameters.Add("DRIVERNO_IN", OracleDbType.Varchar2).Value = strdriverno;
                    oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strAppCode;
                    oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    oCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    oCmd.ExecuteNonQuery();
                    errMsg = Convert.ToString(oCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(oCmd.Parameters["ERRMSG"].Value);
                    return errMsg;

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
        public string UpdateVechiledetail(string strvehiclerequestid, string strvehicleno, string strmeterreading, string strdrivername,
                                            string strdriverno, string strAppCode, string strreleasedby, string strapprovedby, string strremark, string slipno)
        {

            strConn = objConn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand oCmd = new OracleCommand();
                    oCmd.Connection = objCn;
                    oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_AD_VEHICLEDETAILUPDATE";
                    oCmd.CommandType = CommandType.StoredProcedure;
                    oCmd.BindByName = true;
                    oCmd.Parameters.Add("VEHICLEID_IN", OracleDbType.Varchar2).Value = strvehiclerequestid;
                    oCmd.Parameters.Add("VEHICLENO_IN", OracleDbType.Varchar2).Value = strvehicleno;
                    oCmd.Parameters.Add("METERREADING_IN", OracleDbType.Varchar2).Value = strmeterreading;
                    oCmd.Parameters.Add("DRIVERNAME_IN", OracleDbType.Varchar2).Value = strdrivername;
                    oCmd.Parameters.Add("DRIVERNO_IN", OracleDbType.Varchar2).Value = strdriverno;
                    oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strAppCode;
                    oCmd.Parameters.Add("RELEASEDBY_IN", OracleDbType.Int32).Value = strreleasedby == "" ? DBNull.Value : strreleasedby;
                    oCmd.Parameters.Add("APPROVEDBY_IN", OracleDbType.Int32).Value = strapprovedby == "" ? DBNull.Value : strapprovedby;
                    oCmd.Parameters.Add("REMARK_IN", OracleDbType.Varchar2).Value = strremark;
                    oCmd.Parameters.Add("SLIPNO_IN", OracleDbType.Varchar2).Value = slipno;
                    oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    oCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    oCmd.ExecuteNonQuery();
                    errMsg = Convert.ToString(oCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(oCmd.Parameters["ERRMSG"].Value);
                    return errMsg;

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


        public string UpdateVechileReleased(string strvehiclerequestid, string strkmused, string strreleaseat, string strvisiteplace, string strtaxicondition, string strremarks)
        {

            strConn = objConn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand oCmd = new OracleCommand();
                    oCmd.Connection = objCn;
                    oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_AD_VEHICLERELEASEUPDATE";
                    oCmd.CommandType = CommandType.StoredProcedure;
                    oCmd.BindByName = true;
                    oCmd.Parameters.Add("VEHICLEID_IN", OracleDbType.Varchar2).Value = strvehiclerequestid;
                    oCmd.Parameters.Add("KMUSED_IN", OracleDbType.Varchar2).Value = strkmused;
                    oCmd.Parameters.Add("RELEASEDAT_IN", OracleDbType.Varchar2).Value = strreleaseat;
                    oCmd.Parameters.Add("VISITEDPLACE_IN", OracleDbType.Varchar2).Value = strvisiteplace;
                    oCmd.Parameters.Add("TAXICONDITION_IN", OracleDbType.Varchar2).Value = strtaxicondition;
                    oCmd.Parameters.Add("REMARKS_IN", OracleDbType.Varchar2).Value = strremarks;
                    oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    oCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    oCmd.ExecuteNonQuery();
                    errMsg = Convert.ToString(oCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(oCmd.Parameters["ERRMSG"].Value);
                    return errMsg;

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
        public DataTable get_TaxiRequestGateList(string strEmpcode, int strsite, string strsysdate)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_REQLISTGATE_GET";
            oCmd.Parameters.Add("CUR_REQLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
            oCmd.Parameters.Add("SITEID_IN", OracleDbType.Int32).Value = strsite;
            oCmd.Parameters.Add("SYSDATE_IN", OracleDbType.Varchar2).Value = strsysdate;
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return dt;
        }
        public DataTable get_TaxiRelease(string strEmpcode, string strsite, string strreqid, string strstatus, string strfromdate, string strtodate)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_TAXIRELEASESTATUS_GET";
            oCmd.Parameters.Add("CUR_REQLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
            oCmd.Parameters.Add("SITEID_IN", OracleDbType.Varchar2).Value = strsite;
            oCmd.Parameters.Add("REQID_IN", OracleDbType.Varchar2).Value = strreqid;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Varchar2).Value = strstatus;
            oCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = strfromdate;
            oCmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = strtodate;
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return dt;
        }
        public DataTable Get_TaxiusesReport(string strEmpcode, string strsite, string strfromdate, string strtodate, string strvenderid)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_TAXIUSESREPORT_GET";
            oCmd.Parameters.Add("CUR_REQLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strEmpcode;
            oCmd.Parameters.Add("SITEID_IN", OracleDbType.Varchar2).Value = strsite;
            oCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = strfromdate;
            oCmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = strtodate;
            oCmd.Parameters.Add("VRNDERID_IN", OracleDbType.Varchar2).Value = strvenderid;
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return dt;
        }
        public DataTable Get_TaxiConditionReport(string strsite, string strfromdate, string strtodate, string strvenderid)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_TAXICONDITIONREPORT_GET";
            oCmd.Parameters.Add("CUR_REQLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("SITEID_IN", OracleDbType.Varchar2).Value = strsite;
            oCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = strfromdate;
            oCmd.Parameters.Add("TODATE_IN", OracleDbType.Varchar2).Value = strtodate;
            oCmd.Parameters.Add("VRNDERID_IN", OracleDbType.Varchar2).Value = strvenderid;
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return dt;
        }
        public DataTable Get_TaxiAdminApprovalList()
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_GET_ADMINTAXIAPPLIST";
            oCmd.Parameters.Add("CUR_ADMINTAXIAPP", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return dt;
        }
        public DataTable Get_TaxiTypeList(string strsiteid, string strtaxitypeid)
        {
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_AD_TAXITYPE_GET";
            oCmd.Parameters.Add("CUR_TAXITYPE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("SYSITEID_IN", OracleDbType.Varchar2).Value = strsiteid;
            oCmd.Parameters.Add("TAXITYPEID_IN", OracleDbType.Varchar2).Value = strtaxitypeid;
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return dt;
        }
        public string AddUpdateTaxiType(string stryaxitypeid, string strtaxitype, string strrate4_40, string strrate8_80,
                                           string strrate_perkm, string strrate_perhr, string strrate_night, string strsite,
                                           string strstatus, string straddedby)
        {

            strConn = objConn.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    OracleCommand oCmd = new OracleCommand();
                    oCmd.Connection = objCn;
                    oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_AD_TAXITYPE_UPDATE";
                    oCmd.CommandType = CommandType.StoredProcedure;
                    oCmd.BindByName = true;
                    oCmd.Parameters.Add("TAXITYIEID_IN", OracleDbType.Varchar2).Value = stryaxitypeid;
                    oCmd.Parameters.Add("TAXITYPE_IN", OracleDbType.Varchar2).Value = strtaxitype;
                    oCmd.Parameters.Add("RATE_4_40_IN", OracleDbType.Varchar2).Value = strrate4_40;
                    oCmd.Parameters.Add("RATE_8_80_IN", OracleDbType.Varchar2).Value = strrate8_80;
                    oCmd.Parameters.Add("RATE_PERKM_IN", OracleDbType.Varchar2).Value = strrate_perkm;
                    oCmd.Parameters.Add("RATE_PERHR_IN", OracleDbType.Varchar2).Value = strrate_perhr;
                    oCmd.Parameters.Add("RATE_NIGHTCHARGE_IN", OracleDbType.Varchar2).Value = strrate_night;
                    oCmd.Parameters.Add("SITEID_IN", OracleDbType.Varchar2).Value = strsite;
                    oCmd.Parameters.Add("ACTIVE_IN", OracleDbType.Varchar2).Value = strstatus;
                    oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Varchar2).Value = straddedby;
                    oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    oCmd.Parameters.Add("ERRMSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                    oCmd.ExecuteNonQuery();
                    errMsg = Convert.ToString(oCmd.Parameters["RESULT_OUT"].Value.ToString()) + "#" + Convert.ToString(oCmd.Parameters["ERRMSG"].Value);
                    return errMsg;

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

        public Tuple<string, string> SaveTaxiRequest(TaxiRequestRequest obj,string UserName,string UserId)
        {
            //VARIABLE DECLARATION
            Tuple<string, string> retVal_tuple;
            string strAppCode = obj.SUPERVISORADEMPCODE;
            string strDateFrom = obj.DATEOFTRAVELFROM;
            string strDateTo = obj.DATEOFTRAVELTO;

            string strExtNo = obj.EXTENSIONO;// txtExtNo.Text;
            string strPhoneNo = obj.PHONENO.ToString();// txtPhoneNo.Text;
            string strReportingPlace = obj.ADDRESS;// txtReportingPlace.Text;
            string strReportingTime = obj.REPORTINGHOUR.Trim() + ":" + obj.REPORTINGMIN.Trim();// drpLstHour.SelectedItem.Value.Trim() + ":" + drpLstMinute.SelectedItem.Value.Trim(); //+ " " + dlstTime.SelectedItem.Value;
            string strPurposeVisit = obj.PURPOSEOFVISIT;// txtPurposeVisit.Text;
            string strPlaceVisit = obj.PLACEOFVISIT;// txtPlaceVisit.Text;
            string strShift = obj.SHIFT;// cboshift.SelectedItem.Value;
            string strNoofPerson = obj.NOOFPERSON.ToString();// noOfPerson.Text;
            string strRemarks = obj.REMARKS;// txtRemarks.Text;
            string strsiteid = obj.SITE;// ddlsite.SelectedValue;

            //ASSIGN VALUES TO VARIABLES
            

            //ADD VEHICLE REQUEST TO DATABASE
            string strResult = AddVechilerequest(UserId, strExtNo, strPhoneNo, strDateFrom, strDateTo,
                                                                strReportingPlace, strReportingTime, strPurposeVisit, strPlaceVisit,
                                                                strShift, strNoofPerson, strRemarks, strAppCode, strsiteid);

            //GET RETURN VALUES AFTER ADDITION IN DATABASE
            string[] leaveStatus = strResult.Split(new char[] { '#' });
            string errResult = Convert.ToString(leaveStatus[0]);
            string errMsg = Convert.ToString(leaveStatus[2]);
            string strMailSent = Convert.ToString(leaveStatus[1]);

            //IF RETURN VALUE IS 0 OR 2 THEN SHOW ERROR MESSAGE
            if (errResult == "0" || errResult == "2")
            {
                //errorpanel.Style.Add(HtmlTextWriterStyle.Display, "inline");
                //status.Text = errMsg;
                return retVal_tuple = new Tuple<string, string>(errResult, errMsg);
                
            }

            //IF THERE IS NO ERROR THEN SEND EMAIL TO APPROVING AUTHORITY
            if (errResult == "1")
            {
                if (strMailSent == "1")
                {
                    string strApprovalAuthority = string.Empty;
                    string strApprovalAuthCode = string.Empty;
                    string strApprovalAuthEmail = string.Empty;
                    string strApprovalAuthName = string.Empty;

                    
                    string strConn = objConn.getConnectingString();

                    OracleConnection objCn = new OracleConnection();
                    OracleCommand objCmd = new OracleCommand("PKG_GETAPPROVARCODE.SPROC_AD_APPROVARCODE", objCn);
                    objCmd.CommandType = CommandType.StoredProcedure;

                    objCmd.Parameters.Add("CUR_APPROVARCODE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = UserId;
                    objCmd.Parameters.Add("APPROVALTYPE_IN", OracleDbType.Int64).Value = "4";
                    using (objCn)
                    {
                        objCn.ConnectionString = strConn;
                        try
                        {
                            objCn.Open();
                            OracleDataReader objReader = objCmd.ExecuteReader();
                            if (objReader.HasRows)
                            {
                                while (objReader.Read())
                                {
                                    strApprovalAuthCode = objReader["adempcode"].ToString();
                                    strApprovalAuthEmail = objReader["emailid"].ToString().Trim();
                                    strApprovalAuthName = objReader["empname"].ToString();
                                }
                            }
                        }
                        finally
                        {
                            if (objCn != null)
                            {
                                objCn.Close();
                            }
                        }
                    }

                    //SEND EMAIL TO APPROVING AUTHORITY
                    if (!string.IsNullOrEmpty(strApprovalAuthEmail))
                    {
                        commanEmail sendMail = new commanEmail();
                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";

                        if (serverpath.isTestServer())
                            sendMail.MailTo = serverpath.getTestEMail();
                        else
                        {

                            //IF SELECT APPROVING AUTHORIY IS OTHER THEN DEFAULT AUTHORITY THEN CC TO ACTUAL AUTHORITY
                            if (strAppCode != strApprovalAuthCode)
                            {
                                DataTable dt = new DataTable();
                                dt = _objCommon.GetEmployeeOfficialDetails(strAppCode, string.Empty);
                                if (dt.Rows.Count > 0)
                                {
                                    sendMail.MailTo = dt.Rows[0]["EMAILID"].ToString();
                                    strApprovalAuthName = dt.Rows[0]["EMPNAME"].ToString();
                                    sendMail.MailCc = strApprovalAuthEmail;
                                }
                            }
                            else
                            {
                                sendMail.MailTo = strApprovalAuthEmail;
                            }
                        }

                        string strSubject = "Taxi Request from - " + UserName + ", Employee Code - " + UserId;
                        string strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                                         "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                                         "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                         "</td></tr>" +
                                         "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                                         "<b>Taxi Request </b></td></tr><tr height=150><td colspan=2>" +
                                         "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                         "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                         "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                         "<tr><td colspan=3>&nbsp;</td></tr><tr><td valign=top colspan=2><p><b>Dear " + strApprovalAuthName + " San,</b><br />" +
                                         "<br />" + UserName + " - Emp Code (" + UserId + ") " +
                                         "has given a Taxi Request, the details are as follows:<br />" +
                                         "<br />" +
                                         "<tr><td width=125 height=23 valign=top>From Date:</td><td width=389 valign=top>" + strDateFrom + "</td>" +
                                         "</tr><tr><td width=125 height=23 valign=top>To Date:</td><td width=389 valign=top>" + strDateTo + "<br></td></tr>" +
                                         "<tr><td valign=top colspan=2>Please login <a href='" + serverpath.getServerPath() + "index.aspx'>Employee Portal</a> for further action.</td></tr></p>" +
                                         "</td><td width=20>&nbsp;</td></tr><tr valign=bottom> " +
                                         "<td colspan=2><b>Best Regards</b><br /> Team Portal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                                         "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                         "</tr></table></td></tr><tr><td colspan=2 bgcolor=#bcddf6>&nbsp;</td> " +
                                         "</tr></table> ";

                        sendMail.MailSubject = strSubject;
                        sendMail.MailBody = strBody;
                        try
                        {
                            bool status = sendMail.Send();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        
                            return retVal_tuple = new Tuple<string, string>(errResult, "E-mail has been sent sucessfully");
                        
                        
                    }
                }
               
                //Response.Redirect("../ManageRequest.aspx", false);
                //Context.ApplicationInstance.CompleteRequest();
            }
            return retVal_tuple = new Tuple<string, string>(errResult, "Save Successfully");
        }
        public VehicleRequestModel VehicleRequestDetails(string strTransCode)
        {
            oCmd = new OracleCommand();
            
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;
            
            oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_AD_VEHICLE_HISTORY";
            oCmd.Parameters.Add("CUR_HISTORY", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("TRANSID_IN", OracleDbType.Int32).Value = strTransCode;
            ds = oDataMgmt.GetDataSet(oCmd);
            var objReader=ds.Tables[0].Rows[0];
            VehicleRequestModel data = new VehicleRequestModel
            {
                RequestId = objReader["ADVEHICLEREQUESTID"].ToString(),
                Employee = objReader["EMPLOYEE"].ToString(),
                Department = objReader["DEPTNAME"].ToString(),
                ReportingSite = objReader["REPORTING_SITE"].ToString(),
                NumberOfPersons = objReader["NOOFPERSON"].ToString(),
                ApplicationDate = objReader["DATEADDED"].ToString(),
                FromDate = objReader["FROMDATE"].ToString(),
                ToDate = objReader["TODATE"].ToString(),
                Time = objReader["TIME"].ToString(),
                Remark = objReader["REMARK"].ToString(),
                SupervisorName = objReader["SUPERVISORNAME"].ToString(),
                SupervisorEmail = objReader["SUPERVISOREMAILID"].ToString(),
                SupervisorStatus = objReader["SUPERVISORSTATUS"].ToString(),
                SupervisorRemarks = objReader["SUPERVISORREMARKS"].ToString(),
                SupervisorApprovedDate = objReader["SUPERVISORAPPROVEDDATE"].ToString(),
                AdminName = objReader["ADMINNAME"].ToString(),
                AdminStatus = objReader["ADMINSTATUS"].ToString(),
                TaxiNumber = objReader["TAXINO"].ToString(),
                AdminRemarks = objReader["ADMINREMARKS"].ToString(),
                AdminApprovalDate = objReader["ADMINAPPROVALDATE"].ToString(),
                VendorName = objReader["VENDORNAME"].ToString(),
                SupSupervisorName = objReader["SUPSUPERVISORNAME"].ToString(),
                SupSupervisorStatus = objReader["SUPSUPSTATUS"].ToString(),
                SupSupervisorDate = objReader["SUPSUPDATE"].ToString(),
                SupSupervisorEmpCode = objReader["SUPSUPEMPCODE"].ToString(),
                SupSupervisorEmail = objReader["SUPSUPEMPEMAIL"].ToString(),
                SupSupervisorRemarks = objReader["SUPSUPREM"].ToString(),
                SupEcode = objReader["SUPECODE"].ToString(),
                ActiveStatus = objReader["ACTIVESTATUS"].ToString(),
                PurposeOfVisit = objReader["PURPOSEOFVISIT"].ToString(),
                PlaceOfVisit = objReader["PLACEOFVISIT"].ToString(),
                ReportingPlace = objReader["ADDRESS"].ToString()
            };
            if (string.IsNullOrEmpty(data.AdminName.Trim()) && data.AdminName != "")
                data.AdminStatus = "";

            if (data.SupEcode != data.SupSupervisorEmpCode && data.SupEcode != "" && data.SupSupervisorEmpCode != "")
            {
                data.IsEnabled = true;
                data.RecAuth = "Recommendation Authority:";
                data.RecEmail = "Recommendation Authority Email:";
                data.RecStatus = "Recommendation Status:";
                data.RecRem = "Recommendation Remarks:";
                data.RecDate = "Recommendation Date:";
            }
            else
            {
                data.IsEnabled = false;
                data.RecAuth = "Approval Authority:";
                data.RecEmail = "Approval Authority Email:";
                data.RecStatus = "Approval Status:";
                data.RecRem = "Approval Remarks:";
                data.RecDate = "Approval Date:";
                
            }
            
            return data;
        }

        public TaxiRequestViewModel CancelRequestDetails(string strRequestCode)
        {
            oCmd = new OracleCommand();

            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;

            oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_AD_EDITTAXIREQUEST";
            oCmd.Parameters.Add("CUR_EDITVEHICLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("TRANSID_IN", OracleDbType.Int32).Value = strRequestCode;
            ds = oDataMgmt.GetDataSet(oCmd);
            var objReader = ds.Tables[0].Rows[0];
            TaxiRequestViewModel data = new TaxiRequestViewModel
            {
                REQUESTID = strRequestCode,
                SHIFT = objReader["SHIFT"].ToString(),
                EXTENSIONO = objReader["EXTENSIONO"].ToString(),
                PHONENO = long.Parse(objReader["PHONENO"].ToString()),
                DATEOFTRAVELFROM = objReader["DATEOFTRAVELFROM"].ToString(),
                DATEOFTRAVELTO = objReader["DATETO"].ToString(),
                REPORTINGTIME = objReader["REPORTINGTIME"].ToString(),
                ADDRESS = objReader["ADDRESS"].ToString(),
                NOOFPERSON =int.Parse( objReader["NOOFPERSON"].ToString()),
                PURPOSEOFVISIT = objReader["PURPOSEOFVISIT"].ToString(),
                PLACEOFVISIT = objReader["PLACEOFVISIT"].ToString(),
                REMARKS = objReader["REMARKS"].ToString(),
                SUPERVISORADEMPCODE = objReader["SUPERVISORADEMPCODE"].ToString(),
                SUPERVISORADEMPNAME = objReader["SUPERVISORADEMPCODE"].ToString() + " [ " + objReader["SUPEMPNAME"].ToString() + "]"

            };
            

          

            return data;
        }

        public Tuple<string, string> SubmitCancelTaxiRequest(TaxiRequestRequest objModel,string userId, string userName)
        {
            //strRequestID = Request.QueryString["id"];
            string strAppCode = objModel.SUPERVISORADEMPCODE;

            DateTime dtcrrentdate = DateTime.Today;
            DateTime dtfromdate = Convert.ToDateTime(objModel.DATEOFTRAVELFROM);
            string strsystime = String.Format("{0:hhmm}", DateTime.Now);
            string strfromtime = objModel.REPORTINGTIME.Replace(":", "");
            if (!string.IsNullOrEmpty(objModel.REQUESTID))
            {
                if (dtcrrentdate > dtfromdate)
                {
                    return new Tuple<string, string>("Error", "You can not cancel back date Taxi request");
                    
                }

                if (dtcrrentdate == dtfromdate && (Convert.ToInt32(strsystime) > Convert.ToInt32(strfromtime)))
                {
                    
                    return new Tuple<string, string>("Error", "You can not cancel Taxi request after Taxi reporting time");
                    
                }
                
                
                string strResult = CancelTaxiRequest(objModel.REQUESTID, objModel.REMARKS);
                string[] strStatus = strResult.Split(new Char[] { '#' });
                string errResult = Convert.ToString(strStatus[0]);
                string errMsg = Convert.ToString(strStatus[1]);

                //IF THERE IS ANY ERROR THEN SHOW ERROR MESSAGE
                if (errResult == "0" || errResult == "2")
                {
                    
                    return new Tuple<string, string>("Error", errMsg);
                }

                //IF THERE IS NOT ANY ERROR THEN SEND EMAIL TO RESPECTIVE AUTH
                //SELECT APPROVAL AUTHORITY EMAIL, DEPT HEAD EMAIL
                if (errResult == "1")
                {
                    string strApprovalAuthority = string.Empty;
                    string strApprovalAuthCode = string.Empty;
                    string strApprovalAuthEmail = string.Empty;
                    string strApprovalAuthName = string.Empty;

                   
                    string strConn = objConn.getConnectingString();

                    OracleConnection objCn = new OracleConnection();
                    OracleCommand objCmd = new OracleCommand("PKG_GETAPPROVARCODE.SPROC_AD_APPROVARCODE", objCn);
                    objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    objCmd.Parameters.Add("CUR_APPROVARCODE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int64).Value = userId;
                    objCmd.Parameters.Add("APPROVALTYPE_IN", OracleDbType.Int64).Value = "4";
                    using (objCn)
                    {
                        objCn.ConnectionString = strConn;
                        try
                        {
                            objCn.Open();
                            OracleDataReader objReader = objCmd.ExecuteReader();
                            if (objReader.HasRows)
                            {
                                while (objReader.Read())
                                {
                                    strApprovalAuthCode = objReader["adempcode"].ToString();
                                    strApprovalAuthEmail = objReader["emailid"].ToString().Trim();
                                    strApprovalAuthName = objReader["empname"].ToString();
                                }
                            }
                        }
                        finally
                        {
                            if (objCn != null)
                            {
                                objCn.Close();
                            }
                        }
                    }

                    //SEND EMAIL TO APPROVING AUTHORITY
                    if (!string.IsNullOrEmpty(strApprovalAuthEmail))
                    {
                        commanEmail sendMail = new commanEmail();
                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";

                        if (serverpath.isTestServer())
                            sendMail.MailTo = serverpath.getTestEMail();
                        else
                        {

                            //IF SELECT APPROVING AUTHORIY IS OTHER THEN DEFAULT AUTHORITY(DPT HEAD) THEN CC TO ACTUAL AUTHORITY
                            if (strAppCode != strApprovalAuthCode)
                            {
                                DataTable dt = new DataTable();
                                dt = _objCommon.GetEmployeeOfficialDetails(strAppCode, string.Empty);
                                if (dt.Rows.Count > 0)
                                {
                                    sendMail.MailTo = dt.Rows[0]["EMAILID"].ToString();
                                    strApprovalAuthName = dt.Rows[0]["EMPNAME"].ToString();
                                    sendMail.MailCc = strApprovalAuthEmail;
                                }
                            }
                            else
                            {
                                sendMail.MailTo = strApprovalAuthEmail;
                            }

                        }
                      var  strSubject = "Taxi Request - Cancelled By " + userName + " - (" + userId + ")";
                      var  strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                                         "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                                         "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                         "</td></tr>" +
                                         "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                                         "<b>Taxi Request </b></td></tr><tr height=150><td colspan=2>" +
                                         "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                         "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                         "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                         "<tr><td colspan=3>&nbsp;</td></tr><tr><td valign=top colspan=2><p><b>Dear " + strApprovalAuthName + " San,</b><br />" +
                                         "<br />" + userName + " - Emp Code (" + userId + ") " +
                                         "has cancelled the Taxi Request, the details are as follows:<br />" +
                                         "<br />" +
                                         "<tr><td width=125 height=23 valign=top>Request ID:</td><td width=389 valign=top>" + objModel.REQUESTID + "</td>" +
                                         "</tr><tr><td width=125 height=23 valign=top>Reason:</td><td width=389 valign=top>" + objModel.REMARKS + "<br></td></tr>" +
                                         "<tr><td valign=top colspan=2></td></tr></p>" +
                                         "</td><td width=20>&nbsp;</td></tr><tr valign=bottom> " +
                                         "<td colspan=2><b>Best Regards</b><br /> Team Portal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                                         "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                         "</tr></table></td></tr><tr><td colspan=2 bgcolor=#bcddf6>&nbsp;</td> " +
                                         "</tr></table> ";

                        sendMail.MailSubject = strSubject;
                        sendMail.MailBody = strBody;
                        try
                        {
                            bool status1 = sendMail.Send();
                        }
                        catch (Exception ex)
                        {
                            return new Tuple<string, string>("Error", "Exception Occured:   " + ex);
                           
                        }
                    }
                    
                }

            }
            return new Tuple<string, string>("Success", "Save Successfully");
        }
        public TaxiRequestViewModel EditRequestDetails(string strRequestCode)
        {
            oCmd = new OracleCommand();

            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.BindByName = true;

            oCmd.CommandText = "PKG_VHICLEREQUEST.SPROC_AD_EDITTAXIREQUEST";
            oCmd.Parameters.Add("CUR_EDITVEHICLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("TRANSID_IN", OracleDbType.Int32).Value = strRequestCode.ToString();
            ds = oDataMgmt.GetDataSet(oCmd);
            var objReader = ds.Tables[0].Rows[0];
            TaxiRequestViewModel data = new TaxiRequestViewModel();
            if (objReader != null)
            {
                data = new TaxiRequestViewModel
                {
                    SHIFT = objReader["SHIFT"].ToString(),
                    EXTENSIONO = objReader["EXTENSIONO"].ToString(),
                    PHONENO = long.Parse(objReader["PHONENO"].ToString()),
                    DATEOFTRAVELFROM = objReader["DATEOFTRAVELFROM"].ToString(),
                    DATEOFTRAVELTO = objReader["DATETO"].ToString(),
                    REPORTINGTIME = objReader["REPORTTIME"].ToString(),
                    ADDRESS = objReader["ADDRESS"].ToString(),
                    NOOFPERSON =int.Parse( objReader["NOOFPERSON"].ToString()),
                    PURPOSEOFVISIT = objReader["PURPOSEOFVISIT"].ToString(),
                    PLACEOFVISIT = objReader["PLACEOFVISIT"].ToString(),
                    REMARKS = objReader["REMARKS"].ToString(),
                    SUPERVISORADEMPCODE = objReader["SUPERVISORADEMPCODE"].ToString(),
                    REPORTINGHOUR = objReader["RHOUR"].ToString(),
                    REPORTINGMIN = objReader["RMINUTE"].ToString(),
                    SITE = objReader["sysiteid"].ToString(),//sysiteid  
                    REQUESTID = strRequestCode
                };
            }




            return data;
        }

        public Tuple<string,string> EditTaxiRequest(TaxiRequestRequest obj, string UserName, string UserId)
        {
            //VARIABLE DECLARATION
            string strOldAppAuth = string.Empty;
            string OldApprovalAuthName = string.Empty;
            string OldApprovalAuthECode = string.Empty;
            string strApprovalAuthEmail = string.Empty;
            string strOldApprovalAuthEmail = string.Empty;



            //ASSIGN PAGE VALUES TO VARIABLES
            string strAppCode = obj.SUPERVISORADEMPCODE;
            string strDateFrom = obj.DATEOFTRAVELFROM;
            string strAppAuth = obj.SUPERVISORADEMPCODE;
            string strRequestID = obj.REQUESTID;
            string strDateTo = obj.DATEOFTRAVELTO;

            string strExtNo = obj.EXTENSIONO;// txtExtNo.Text;
            string strPhoneNo = obj.PHONENO.ToString();// txtPhoneNo.Text;
            string strReportingPlace = obj.ADDRESS;// txtReportingPlace.Text;
            string strReportingTime = obj.REPORTINGHOUR.Trim() + ":" + obj.REPORTINGMIN.Trim();// drpLstHour.SelectedItem.Value.Trim() + ":" + drpLstMinute.SelectedItem.Value.Trim(); //+ " " + dlstTime.SelectedItem.Value;
            string strPurposeVisit = obj.PURPOSEOFVISIT;// txtPurposeVisit.Text;
            string strPlaceVisit = obj.PLACEOFVISIT;// txtPlaceVisit.Text;
            string strShift = obj.SHIFT;// cboshift.SelectedItem.Value;
            string strNoofPerson = obj.NOOFPERSON.ToString();// noOfPerson.Text;
            string strRemarks = obj.REMARKS;// txtRemarks.Text;
            string strsiteid = obj.SITE;// ddlsite.SelectedValue;
            DataTable dt = new DataTable();
            //DATE TIME INFORMATION
            DateTimeFormatInfo dateTimeFormatterProvider = DateTimeFormatInfo.CurrentInfo.Clone() as DateTimeFormatInfo;
            dateTimeFormatterProvider.ShortDatePattern = "MM-dd-yyyy";

            DateTime objFrmDt = new DateTime();
            if (strDateFrom != null && strDateFrom.Trim() != "")
                objFrmDt = System.Convert.ToDateTime(strDateFrom, dateTimeFormatterProvider);

            string strFrmdate = objFrmDt.ToString("dd-MMM-yyyy");

            DateTime objToDt = new DateTime();
            if (strDateTo != null && strDateTo.Trim() != "")
                objToDt = System.Convert.ToDateTime(strDateTo, dateTimeFormatterProvider);

            string strTodate = objToDt.ToString("dd-MMM-yyyy");

            //FIND THE OLD APPROVAL AUTHORITY
            strConn = objConn.getConnectingString();

            if (!string.IsNullOrEmpty(strRequestID))
            {
                OracleConnection objCn = new OracleConnection();
                OracleCommand objCmd = new OracleCommand("PKG_VHICLEREQUEST.SPROC_AD_EDITTAXIREQUEST", objCn);
                objCmd.CommandType = System.Data.CommandType.StoredProcedure;

                objCmd.Parameters.Add("TRANSID_IN", OracleDbType.Int64).Value = strRequestID.Trim();
                objCmd.Parameters.Add("CUR_EDITVEHICLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                using (objCn)
                {
                    objCn.ConnectionString = strConn;
                    try
                    {
                        objCn.Open();
                        OracleDataReader objReader = objCmd.ExecuteReader();
                        if (objReader.HasRows)
                        {
                            while (objReader.Read())
                            {
                                strOldAppAuth = objReader["SUPERVISORADEMPCODE"].ToString();
                            }
                        }
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
                   

                //UPDATE THE TAXI REQUEST
                
                string strResult = UpdateTaxiRequest(strShift, strExtNo, strPhoneNo, strFrmdate, strTodate,
                    strReportingPlace, strReportingTime, strPurposeVisit, strPlaceVisit, strNoofPerson,
                    strRemarks, strAppAuth, strRequestID);

                //SYSTEM WILL RETURN ERROR MESSAGE IF THERE IS ANY ERROR
                string[] leaveStatus = strResult.Split(new Char[] { '#' });
                string errResult = Convert.ToString(leaveStatus[0]);
                string errMsg = Convert.ToString(leaveStatus[1]);
                string strAppAuthName = string.Empty;

                //IN CASE OF ERROR, VALIDATION SHOW THE ERROR MESSAGE
                if (errResult == "0" || errResult == "2")
                {
                //errorpanel.Style.Add(HtmlTextWriterStyle.Display, "inline");
                //status.Text = errMsg;
                return new Tuple<string, string>("Error", errMsg);
            }

                //IF THERE IS NOT ANY ERROR 
                if (errResult == "1")
                {
                    //SELECT APPROVAL AUTHORITY NAME AND EMAIL
                    dt = new DataTable();
                    dt = _objCommon.GetEmployeeOfficialDetails(strAppAuth, string.Empty);
                    if (dt.Rows.Count > 0)
                    {
                        strAppAuthName = dt.Rows[0]["EMPNAME"].ToString();
                        strApprovalAuthEmail = dt.Rows[0]["EMAILID"].ToString();
                    }

                    //IF OLD AND NEW APPROVAL AUTHORITY ARE SAME THEN SEND ONE EMAIL WITH UPDATED INFO
                    if (strOldAppAuth == strAppAuth)
                    {
                        if (!string.IsNullOrEmpty(strApprovalAuthEmail))
                        {
                            commanEmail sendMail = new commanEmail();
                            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                            sendMail.MailTo = strApprovalAuthEmail;

                           var strSubject = "Taxi Request from - " + UserName + ", Employee Code - " + UserId;
                           var strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                                         "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                                         "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                         "</td></tr>" +
                                         "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                                         "<b>Taxi Request </b></td></tr><tr height=150><td colspan=2>" +
                                         "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                         "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                         "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                         "<tr><td colspan=3>&nbsp;</td></tr><tr><td valign=top colspan=2><p><b>Dear " + strAppAuthName + " San,</b><br />" +
                                         "<br />" + UserName + " - Emp Code (" + UserId + ") " +
                                         "has been updated the Taxi Request, the details are as follows:<br />" +
                                         "<br />" +
                                         "<tr><td width=125 height=23 valign=top>From Date:</td><td width=389 valign=top>" + strFrmdate + "</td>" +
                                         "</tr><tr><td width=125 height=23 valign=top>To Date:</td><td width=389 valign=top>" + strTodate + "<br></td></tr>" +
                                         "<tr><td valign=top colspan=2>Please login <a href='" + serverpath.getServerPath() + "index.aspx'>Employee Portal</a> for further action.</td></tr></p>" +
                                         "</td><td width=20>&nbsp;</td></tr><tr valign=bottom> " +
                                         "<td colspan=2><b>Best Regards</b><br /> Team Portal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                                         "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                         "</tr></table></td></tr><tr><td colspan=2 bgcolor=#bcddf6>&nbsp;</td> " +
                                         "</tr></table> ";

                            sendMail.MailSubject = strSubject;
                            sendMail.MailBody = strBody;
                            try
                            {
                                bool status1 = sendMail.Send();
                            }
                            catch (Exception ex)
                            {
                            return new Tuple<string, string>("Error", ex.ToString());
                        }

                        }
                    }
                    else
                    {
                        //IF OLD AND NEW APPROVAL AUTHORITY ARE NOT SAME THEN GET OLD APPROVAL AUTH DETAILS
                        dt = new DataTable();
                        dt = _objCommon.GetEmployeeOfficialDetails(strOldAppAuth, string.Empty);
                        if (dt.Rows.Count > 0)
                        {
                            OldApprovalAuthName = dt.Rows[0]["EMPNAME"].ToString();
                            strOldApprovalAuthEmail = dt.Rows[0]["EMAILID"].ToString();
                            OldApprovalAuthECode = strOldAppAuth;
                        }
                    //SEND THE EMAIL TO NEW APPROVAL AUTHORITY
                    commanEmail sendMail = new commanEmail();
                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        sendMail.MailTo = strApprovalAuthEmail;

                       var strSubject = "Taxi Request from - " + UserName + ", Employee Code - " + UserId;
                       var strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                                         "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                                         "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                         "</td></tr>" +
                                         "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                                         "<b>Taxi Request </b></td></tr><tr height=150><td colspan=2>" +
                                         "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                         "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                         "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                         "<tr><td colspan=3>&nbsp;</td></tr><tr><td valign=top colspan=2><p><b>Dear " + strAppAuthName + " San,</b><br />" +
                                         "<br />" + UserName + " - Emp Code (" + UserId + ") " +
                                         "has given the Taxi Request, the details are as follows:<br />" +
                                         "<br />" +
                                         "<tr><td width=125 height=23 valign=top>From Date:</td><td width=389 valign=top>" + strFrmdate + "</td>" +
                                         "</tr><tr><td width=125 height=23 valign=top>To Date:</td><td width=389 valign=top>" + strTodate + "<br></td></tr>" +
                                         "<tr><td valign=top colspan=2>This communication has been sent to " + OldApprovalAuthName + ". </td></tr>" +
                                         "<tr><td valign=top colspan=2>Please login <a href='" + serverpath.getServerPath() + "index.aspx'>Employee Portal</a> for further action.</td></tr></p>" +
                                         "</td><td width=20>&nbsp;</td></tr><tr valign=bottom> " +
                                         "<td colspan=2><b>Best Regards</b><br /> Team Portal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                                         "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                         "</tr></table></td></tr><tr><td colspan=2 bgcolor=#bcddf6>&nbsp;</td> " +
                                         "</tr></table> ";

                        sendMail.MailSubject = strSubject;
                        sendMail.MailBody = strBody;
                        try
                        {
                            bool status2 = sendMail.Send();
                        }
                        catch (Exception ex)
                        {
                        return new Tuple<string, string>("Error", ex.ToString());
                    }

                    //SEND THE EMAIL TO OLD APPROVAL AUTHORITY (JUST INFORM)
                   
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        sendMail.MailTo = strOldApprovalAuthEmail;
                        strSubject = "Taxi Request [Updated] - " + UserName + ", Employee Code - " + UserId;
                        strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                                        "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images//HondaLogo5.gif border=0 /></td>" +
                                        "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                                        "</td></tr>" +
                                        "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                                        "<b>Taxi Request </b></td></tr><tr height=150><td colspan=2>" +
                                        "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                                        "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                                        "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                                        "<tr><td colspan=3>&nbsp;</td></tr><tr><td valign=top colspan=2><p><b>Dear " + OldApprovalAuthName + " San,</b><br />" +
                                        "<br />" + UserName + " - Emp Code (" +UserId + ") " +
                                        "has been updated the Taxi Request, the details are as follows:<br />" +
                                        "<br />" +
                                        "<tr><td width=125 height=23 valign=top>From Date:</td><td width=389 valign=top>" + strFrmdate + "</td>" +
                                        "</tr><tr><td width=125 height=23 valign=top>To Date:</td><td width=389 valign=top>" + strTodate + "<br></td></tr>" +
                                        "<tr><td valign=top colspan=2>Taxi request has been updated and sent to " + strAppAuthName + " San for approval process. </td></tr>" +
                                        "<tr><td valign=top colspan=2>Please login <a href='" + serverpath.getServerPath() + "index.aspx'>Employee Portal</a> for further action.</td></tr></p>" +
                                        "</td><td width=20>&nbsp;</td></tr><tr valign=bottom> " +
                                        "<td colspan=2><b>Best Regards</b><br /> Team Portal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                                        "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                                        "</tr></table></td></tr><tr><td colspan=2 bgcolor=#bcddf6>&nbsp;</td> " +
                                        "</tr></table> ";

                        sendMail.MailSubject = strSubject;
                        sendMail.MailBody = strBody;
                        try
                        {
                            bool status3 = sendMail.Send();
                        }
                        catch (Exception ex)
                        {
                        return new Tuple<string, string>("Error", ex.ToString());
                    }

                    }

                }
            return new Tuple<string, string>("Success", "Save Successfully");
        }

        public Tuple<string, string> TaxiApproval(TaxiRequestRequest obj, string UserName, string UserId)
        {
            string strsupsupempcode = string.Empty;
            string stroperation = string.Empty;
            string strRequestid = "";
            strRequestid = obj.REQUESTID;

            string strSupEmpCode = "";
            strSupEmpCode = UserId;

            string strApprovalStatus = "";
            strApprovalStatus = obj.APPRSTATUS;

            string strRemarks = "";
            strRemarks = obj.REMARKS;

            //SECOND LEVEL APPROVING AUTHORITY
            string MFG_OPID = _objCommon.GetParameterValue("HRKRA_ADVIPID_MFG");
            string TAPUKARA_ID = _objCommon.GetParameterValue("TAPUKARA_SITE_ID");
            if (obj.HDN_MFG_OPID == MFG_OPID && obj.HDN_TAPUKARA_ID != TAPUKARA_ID)
            {
                strsupsupempcode = obj.SUPERVISORADEMPCODE;
            }
            stroperation = obj.HDN_MFG_OPID;

            //UPDATE THE APPROVAL STATUS
            
            string EntryResult =UpdateTaxiApproval(strSupEmpCode, strRequestid, strApprovalStatus, strRemarks, strsupsupempcode, stroperation);

            string[] leaveStatus = EntryResult.Split(new Char[] { '#' });
            string errResult = Convert.ToString(leaveStatus[0]);
            string errMsg = Convert.ToString(leaveStatus[1]);

            if (errResult == "0")
            {
                //errorpanel.Style.Add(HtmlTextWriterStyle.Display, "inline");
                //status.Text = errMsg;
                return new Tuple<string, string>("Error", errMsg);
            }
            if (errResult == "1")
            {
                
                string strConn = objConn.getConnectingString();
                OracleConnection objCn;

                //GET THE STATUS OF REQEUST APPROVAL (IF PENDING THEN SEND MAIL TO APPROVAL AUTHORITY WITH REQEUST DETAILS)
                objCn = new OracleConnection();
                //OracleCommand objCmd2 = new OracleCommand("select NVL(a.issupersupervisorapproved,0) as issupersupervisorapproved  from advehiclerequest a where a.advehiclerequestid = " + strRequestid, objCn);
                OracleCommand objCmd2 = new OracleCommand("PKG_VHICLEREQUEST.SPROC_APPSTATUS_GET", objCn);
                objCmd2.Parameters.Add("CUR_TAXIAPPSTATUS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                objCmd2.Parameters.Add("REQUESTID_IN", OracleDbType.Int64).Value = strRequestid;
                objCmd2.CommandType = System.Data.CommandType.StoredProcedure;

                string strsupsupApproved = string.Empty;

                using (objCn)
                {
                    objCn.ConnectionString = strConn;
                    try
                    {
                        objCn.Open();
                        OracleDataReader objReader2 = objCmd2.ExecuteReader();
                        if (objReader2.HasRows)
                        {
                            while (objReader2.Read())
                            {
                                strsupsupApproved = objReader2["issupersupervisorapproved"].ToString().Trim();
                            }
                        }
                    }
                    finally
                    {
                        if (objCn != null)
                        {
                            objCn.Close();
                        }
                    }
                }

                if (!string.IsNullOrEmpty(strsupsupempcode) && strsupsupApproved == "0")
                {
                   

                    DataTable dt = new DataTable();
                    string strEmailid = string.Empty;
                    //GET EMAIL ID OF SUPER SUPERVISOR
                    dt = _objCommon.GetEmployeeOfficialDetails(strsupsupempcode, string.Empty);
                    if (dt.Rows.Count > 0)
                    {
                        strEmailid = dt.Rows[0]["EMAILID"].ToString().Trim();
                    }

                    //GET THE REQUEST DETAILS AND MAIL IT TO SUPER SUPERVISOR
                    objCn = new OracleConnection();
                    OracleCommand objCmd1 = new OracleCommand("PKG_VHICLEREQUEST.SPROC_AD_TAXIAPPROVALBYID", objCn);
                    objCmd1.Parameters.Add("CUR_TRANS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    objCmd1.Parameters.Add("TRANSID_IN", OracleDbType.Int64).Value = strRequestid;
                    objCmd1.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = obj.EMPCODE;

                    objCmd1.CommandType = System.Data.CommandType.StoredProcedure;
                    string strRequesterName = string.Empty;
                    string strRequestEcode = string.Empty;
                    string strFromDate = string.Empty;
                    string strtoDate = string.Empty;

                    using (objCn)
                    {
                        objCn.ConnectionString = strConn;
                        try
                        {
                            objCn.Open();
                            OracleDataReader objReader1 = objCmd1.ExecuteReader();
                            if (objReader1.HasRows)
                            {
                                while (objReader1.Read())
                                {
                                    strRequesterName = objReader1["empname"].ToString().Trim();
                                    strRequestEcode = objReader1["empcode"].ToString().Trim();
                                    strFromDate = objReader1["fromdate"].ToString().Trim();
                                    strtoDate = objReader1["todate"].ToString().Trim();
                                }
                            }
                        }
                        finally
                        {
                            if (objCn != null)
                            {
                                objCn.Close();
                            }
                        }
                    }
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";

                    if (serverpath.isTestServer())
                        sendMail.MailTo = serverpath.getTestEMail();
                    else
                        sendMail.MailTo = strEmailid;

                    string strSubject = "Taxi Request from - " + strRequesterName + ", Employee Code - " + strRequestEcode;
                    string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Taxi Request from " + strRequesterName + " - Emp Code (" + strRequestEcode + ")</font></b></td>" +
                                     "</tr><tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td width=125 height=22 valign=top>Employee Code</td><td width=389 valign=top>" + strRequestEcode + "</td>" +
                                     "</tr><tr><td width=125 height=23 valign=top>Employee Name</td><td width=389 valign=top>" + strRequesterName + "</td>" +
                                     " </tr><tr> " +
                                     "<td width=125 valign=top>From Date</td><td width=389 valign=top>" + strFromDate + "</td></tr><tr><td width=125 valign=top>To Date</td>" +
                                     "<td width=389 valign=top>" + strtoDate + "</td></tr>" +
                                     "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "index.aspx> Employee Portal</a> for approval process.</td></tr>" +
                                     "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    try
                    {
                        bool status = sendMail.Send();
                    }
                    catch (Exception ex)
                    {
                        return new Tuple<string, string>("Error", ex.ToString());
                    }
                    finally
                    {
                       
                    }
                } 
               
                
            }
            return new Tuple<string, string>("Success", "Record Saved Successfully");
        }


    }
        #endregion


    }

