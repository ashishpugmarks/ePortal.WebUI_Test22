using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using ePortal.Persistence.TourRequest.Interface;
using ePortal.Shared;
using ePortal.ViewModels.APPX.TourRequest;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Collections;
using System.Data;

namespace ePortal.Persistence.TourRequest.Services
{
    public class TourSettlement : ITourSettlement
    {
        private readonly IConfiguration configuration;
        private readonly IDataManagement oDataMgmt;
        private readonly IConnectionString objCnStr;
        private readonly ICommonFunctions objcmn; 

        public TourSettlement(IConfiguration _configuration, IDataManagement _oDataMgmt, IConnectionString _objCnStr, ICommonFunctions _objcmn)
        {
            configuration = _configuration;
            oDataMgmt = _oDataMgmt;
            objCnStr = _objCnStr;
            objcmn = _objcmn;
        }



        #region "Local Variables"
        private string _ErrorMessage = string.Empty;
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set { _ErrorMessage = value; }
        }

        DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        DataRow[] _datarow;
        //DataManagement oDataMgmt = new DataManagement();
        //CommonFunctions objcmn = new CommonFunctions();
        #endregion

        #region "Insert Update Tour Settlement Detail"
        public void UpdateTBS(string strSettlementid, string strEmpcode, DateTime Planfrom_date, DateTime Planto_date, DateTime Actfrom_date, DateTime Actto_date, DateTime Actfrom_time, DateTime Actto_time, string Addedby, Tour_Day_Detail[] obj, Ticket_Detail[] objticket, string strAppEcode, out string strSettid)
        {
            String strErrMsg = String.Empty;
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            OracleTransaction tran;
            Int64 Settlementid;
            strSettid = "";
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                tran = objCn.BeginTransaction();
                try
                {
                    //insert or update TBS transaction detail
                    OracleCommand oCmd = new OracleCommand();
                    oCmd.Connection = objCn;
                    oCmd.Transaction = tran;
                    oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    oCmd.BindByName = true;
                    oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_UPDATETBSTRANS";
                    oCmd.Parameters.Add("V_SETTLEMENTID", OracleDbType.Int64).Direction = ParameterDirection.InputOutput;
                    oCmd.Parameters["V_SETTLEMENTID"].Value = strSettlementid;
                    oCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode;
                    oCmd.Parameters.Add("V_PLAN_FROMDATE", OracleDbType.Date).Value = Planfrom_date;
                    oCmd.Parameters.Add("V_PLAN_TODATE", OracleDbType.Date).Value = Planto_date;
                    oCmd.Parameters.Add("V_ACTUAL_FROMDATE", OracleDbType.Date).Value = Actfrom_date;
                    oCmd.Parameters.Add("V_ACTUAL_TODATE", OracleDbType.Date).Value = Actto_date;
                    oCmd.Parameters.Add("V_ACTUAL_FROMTIME", OracleDbType.Date).Value = Actfrom_time;
                    oCmd.Parameters.Add("V_ACTUAL_TOTIME", OracleDbType.Date).Value = Actto_time;
                    oCmd.Parameters.Add("V_APPEMPCODE", OracleDbType.Int32).Value = strAppEcode;
                    oCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = Addedby;
                    oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                    oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    oCmd.ExecuteNonQuery();
                    Settlementid = Convert.ToInt64(oCmd.Parameters["V_SETTLEMENTID"].Value.ToString());
                    strSettid = Settlementid.ToString();
                    int subResult = Convert.ToInt32(oCmd.Parameters["RESULT_OUT"].Value.ToString());
                    if (subResult != 1)
                    {
                        tran.Rollback();
                        throw new Exception("Failed to update. Transaction will rollback now.");
                    }
                    foreach (Ticket_Detail objtkt in objticket)
                    {
                        oCmd.Parameters.Clear();
                        oCmd.Transaction = tran;
                        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        oCmd.BindByName = true;
                        oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_UPDATETBSTKTDTL";
                        oCmd.Parameters.Add("V_TICKETID", OracleDbType.Int32).Direction = ParameterDirection.InputOutput;
                        oCmd.Parameters["V_TICKETID"].Value = objtkt.ID;
                        oCmd.Parameters.Add("V_SETTLEMENTID", OracleDbType.Int32).Value = Settlementid;
                        oCmd.Parameters.Add("V_TICKETDETAILID", OracleDbType.Int32).Value = objtkt.TICKETID;
                        oCmd.Parameters.Add("V_USERSTATUS", OracleDbType.Int32).Value = objtkt.TICKETSTATUS;
                        oCmd.Parameters.Add("V_SUBSTATUS", OracleDbType.Int32).Value = objtkt.TICKETSUBSTATUS == null ? (object)DBNull.Value : objtkt.TICKETSUBSTATUS;
                        oCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = Addedby;
                        oCmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = 1;
                        oCmd.Parameters.Add("V_TICKETFLAG", OracleDbType.Int32).Value = objtkt.TICKETFLAG;
                        oCmd.Parameters.Add("V_TICKETATTACHMENT", OracleDbType.Varchar2).Value = objtkt.TICKET_FILE_NAME;
                        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                        oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                        oCmd.ExecuteNonQuery();
                        subResult = Convert.ToInt32(oCmd.Parameters["RESULT_OUT"].Value.ToString());
                        if (subResult != 1)
                        {
                            tran.Rollback();
                            throw new Exception("Failed to update. Transaction will rollback now.");
                        }
                    }
                    double Settdaytotal = 0.0;
                    foreach (Tour_Day_Detail o in obj)
                    {
                        //Insert Tour Bill Settlement Day Detail
                        oCmd.Parameters.Clear();
                        oCmd.Transaction = tran;
                        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        oCmd.BindByName = true;
                        //oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_UPDATETBSDAYDETAIL";
                        oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.NEW_SPROC_UPDATETBSDAYDETAIL";
                        oCmd.Parameters.Add("V_TBSDAYDTLID", OracleDbType.Int32).Direction = ParameterDirection.InputOutput;
                        oCmd.Parameters["V_TBSDAYDTLID"].Value = o.ID;
                        oCmd.Parameters.Add("V_SETTLEMENTID", OracleDbType.Int32).Value = Settlementid;
                        oCmd.Parameters.Add("V_ADTOURREQUESTID", OracleDbType.Int32).Value = o.TourID;
                        oCmd.Parameters.Add("V_SETTLEMENTDATE", OracleDbType.Date).Value = o.Tourdate;
                        oCmd.Parameters.Add("V_TICKETFAIR", OracleDbType.Int32).Value = o.Ticket_Fair;
                        oCmd.Parameters.Add("V_HTL_AMOUNT", OracleDbType.Int32).Value = o.Hotel_Amt;
                        oCmd.Parameters.Add("V_BRKFAST_FLAG", OracleDbType.Int32).Value = o.BRK_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_BRKFAST_PER", OracleDbType.Int32).Value = o.BRK_PERC;
                        oCmd.Parameters.Add("V_LUNCH_FLAG", OracleDbType.Int32).Value = o.Lunch_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_LUNCH_PER", OracleDbType.Int32).Value = o.LUNCH_PERC;
                        oCmd.Parameters.Add("V_DINNER_FLAG", OracleDbType.Int32).Value = o.Dinner_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_DINNER_PER", OracleDbType.Int32).Value = o.DINNER_PERC;
                        oCmd.Parameters.Add("V_MISSALLOW_FLAG", OracleDbType.Int32).Value = o.MissAllow_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_MISSALLOW_PER", OracleDbType.Int32).Value = o.MISSALLOW_PERC;
                        oCmd.Parameters.Add("V_NTA_FLAG", OracleDbType.Int32).Value = o.NTA_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_NTA_AMOUNT", OracleDbType.Int32).Value = o.NTA_Amt;
                        oCmd.Parameters.Add("V_LA_AMOUNT", OracleDbType.Int32).Value = o.LA_Amt_Actual;
                        oCmd.Parameters.Add("V_HA_FLAG", OracleDbType.Int32).Value = o.HA_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_HA_AMOUNT", OracleDbType.Int32).Value = o.HA_Amt;
                        oCmd.Parameters.Add("V_OTHEXPAMOUNT", OracleDbType.Int32).Value = o.Miss_Amt;
                        oCmd.Parameters.Add("V_OTHEXPDETAIL", OracleDbType.Varchar2).Value = o.Exp_detail == null ? "" : o.Exp_detail;
                        oCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = Addedby;
                        oCmd.Parameters.Add("V_REMARK", OracleDbType.Varchar2).Value = o.Remark == null ? "" : o.Remark;

                        oCmd.Parameters.Add("V_FIN_TICKETFAIR", OracleDbType.Int32).Value = o.Fin_Ticket_Fair;
                        oCmd.Parameters.Add("V_FIN_HOTEL_AMOUNT", OracleDbType.Int32).Value = o.Fin_Hotel_Amt;
                        oCmd.Parameters.Add("V_FIN_BRKFAST_FLAG", OracleDbType.Int32).Value = o.Fin_BRK_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_FIN_LUNCH_FLAG", OracleDbType.Int32).Value = o.Fin_Lunch_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_FIN_DINNER_FLAG", OracleDbType.Int32).Value = o.Fin_Dinner_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_FIN_MISSALLOW_FLAG", OracleDbType.Int32).Value = o.Fin_MissAllow_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_FIN_NTA_FLAG", OracleDbType.Int32).Value = o.Fin_NTA_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_FIN_LA_AMOUNT", OracleDbType.Int32).Value = o.Fin_LA_Amt_Actual;
                        oCmd.Parameters.Add("V_FIN_HA_FLAG", OracleDbType.Int32).Value = o.Fin_HA_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_FIN_OTHEXPAMOUNT", OracleDbType.Int32).Value = o.Fin_Miss_Amt;
                        oCmd.Parameters.Add("V_FIN_REMARK", OracleDbType.Varchar2).Value = o.Fin_Remark == null ? "" : o.Fin_Remark;
                        oCmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = 1;

                        oCmd.Parameters.Add("V_HOTElAMTTOT", OracleDbType.Int32).Value = o.HotelAmtTot;
                        oCmd.Parameters.Add("V_OTHERAMT", OracleDbType.Int32).Value = o.OtherAmt;
                        //oCmd.Parameters.Add("V_CGSTRATE", OracleDbType.Int32).Value = o.CGSTPercent;
                        //oCmd.Parameters.Add("V_SGSTRATE", OracleDbType.Int32).Value = o.SGSTPercent;
                        oCmd.Parameters.Add("V_CGSTRATE", OracleDbType.Int32).Value = o.CGSTRateID;
                        oCmd.Parameters.Add("V_SGSTRATE", OracleDbType.Int32).Value = o.SGSTRateID;

                        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                        oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                        oCmd.ExecuteNonQuery();
                        subResult = Convert.ToInt32(oCmd.Parameters["RESULT_OUT"].Value.ToString());
                        if (subResult != 1)
                        {
                            tran.Rollback();
                            throw new Exception("Failed to update. Transaction will rollback now.");
                        }

                        //Insert Tour Bill Settlement Conveyance Detail
                        if (o.Conveyance != null)
                        {
                            foreach (Conveyance_Detail oConv in o.Conveyance)
                            {
                                oCmd.Parameters.Clear();
                                oCmd.Transaction = tran;
                                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                                oCmd.BindByName = true;
                                oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_UPDATETBSCONVDTL";
                                oCmd.Parameters.Add("V_CONVEYANCEID", OracleDbType.Int32).Direction = ParameterDirection.InputOutput;
                                oCmd.Parameters["V_CONVEYANCEID"].Value = oConv.ID;
                                oCmd.Parameters.Add("V_SETTLEMENTID", OracleDbType.Int32).Value = Settlementid;
                                oCmd.Parameters.Add("V_CONVEYANCE_DATE", OracleDbType.Date).Value = o.Tourdate;
                                oCmd.Parameters.Add("V_CITY_NAME", OracleDbType.Varchar2).Value = oConv.City;
                                oCmd.Parameters.Add("V_LOC_FROM", OracleDbType.Varchar2).Value = oConv.From_Loc;
                                oCmd.Parameters.Add("V_LOC_TO", OracleDbType.Varchar2).Value = oConv.To_Loc;
                                oCmd.Parameters.Add("V_MODEOFTRANSPORT", OracleDbType.Varchar2).Value = oConv.Mode_Tran;
                                oCmd.Parameters.Add("V_AMOUNT", OracleDbType.Int32).Value = oConv.Conv_Amt;
                                oCmd.Parameters.Add("V_FIN_AMOUNT", OracleDbType.Int32).Value = oConv.Fin_Conv_Amt;
                                oCmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = 1;
                                oCmd.Parameters.Add("V_ADDDEDBY", OracleDbType.Int32).Value = Addedby;
                                oCmd.Parameters.Add("V_CONV_FILE", OracleDbType.Varchar2).Value = oConv.CONV_FILE_NAME;
                                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                                oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                                oCmd.ExecuteNonQuery();
                                subResult = Convert.ToInt32(oCmd.Parameters["RESULT_OUT"].Value.ToString());
                                string err_msg = Convert.ToString(oCmd.Parameters["ERROR_MSG"].Value);
                                if (subResult != 1)
                                {
                                    tran.Rollback();
                                    throw new Exception("Failed to update. Transaction will rollback now.");
                                }
                            }
                        }
                        //Insert Tour Bill Settlement Visited Place Detail
                        if (o.Places != null)
                        {
                            foreach (Place_Detail oPlace in o.Places)
                            {
                                oCmd.Parameters.Clear();
                                oCmd.Transaction = tran;
                                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                                oCmd.BindByName = true;
                                oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_UPDATETBSVISITEDPLACE";
                                oCmd.Parameters.Add("V_VISITID", OracleDbType.Int32).Direction = ParameterDirection.InputOutput;
                                oCmd.Parameters["V_VISITID"].Value = oPlace.ID;
                                oCmd.Parameters.Add("V_SETTLEMENTID", OracleDbType.Int32).Value = Settlementid;
                                oCmd.Parameters.Add("V_VISITDATE", OracleDbType.Date).Value = o.Tourdate;
                                oCmd.Parameters.Add("V_CITY", OracleDbType.Varchar2).Value = oPlace.City;
                                oCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = Addedby;
                                oCmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = 1;
                                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                                oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                                oCmd.ExecuteNonQuery();
                                subResult = Convert.ToInt32(oCmd.Parameters["RESULT_OUT"].Value.ToString());
                                if (subResult != 1)
                                {
                                    tran.Rollback();
                                    throw new Exception("Failed to update. Transaction will rollback now.");
                                }
                            }
                        }
                        Settdaytotal += Convert.ToDouble(o.DayTotal == "" ? "0" : o.DayTotal);
                    }

                    oCmd.Parameters.Clear();
                    oCmd.Transaction = tran;
                    oCmd.CommandType = System.Data.CommandType.Text;
                    oCmd.CommandText = "UPDATE FINTBS_TRANS SET SETTDAYTOTAL=" + Settdaytotal.ToString() + " where SETTLEMENTID=" + Settlementid;
                    oCmd.ExecuteNonQuery();

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
        #endregion

        #region "Insert Update Tour Settlement Detail"
        public void NEWUpdateTBS(string strSettlementid, string strEmpcode, DateTime Planfrom_date, DateTime Planto_date, DateTime Actfrom_date, DateTime Actto_date, DateTime Actfrom_time, DateTime Actto_time, string Addedby, Tour_Day_Detail[] obj, Ticket_Detail[] objticket, string strAppEcode, out string strSettid, List<Hotel_Detail> objHotelList, string OTHERATTACHMENT, string OtherTKTFile)
        {
            String strErrMsg = String.Empty;
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            OracleTransaction tran;
            Int64 Settlementid;
            strSettid = "";
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                tran = objCn.BeginTransaction();
                try
                {
                    //insert or update TBS transaction detail
                    OracleCommand oCmd = new OracleCommand();
                    oCmd.Connection = objCn;
                    oCmd.Transaction = tran;
                    oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    oCmd.BindByName = true;
                    oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_UPDATETBSTRANS";
                    oCmd.Parameters.Add("V_SETTLEMENTID", OracleDbType.Int64).Direction = ParameterDirection.InputOutput;
                    oCmd.Parameters["V_SETTLEMENTID"].Value = strSettlementid;
                    oCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode;
                    oCmd.Parameters.Add("V_PLAN_FROMDATE", OracleDbType.Date).Value = Planfrom_date;
                    oCmd.Parameters.Add("V_PLAN_TODATE", OracleDbType.Date).Value = Planto_date;
                    oCmd.Parameters.Add("V_ACTUAL_FROMDATE", OracleDbType.Date).Value = Actfrom_date;
                    oCmd.Parameters.Add("V_ACTUAL_TODATE", OracleDbType.Date).Value = Actto_date;
                    oCmd.Parameters.Add("V_ACTUAL_FROMTIME", OracleDbType.Date).Value = Actfrom_time;
                    oCmd.Parameters.Add("V_ACTUAL_TOTIME", OracleDbType.Date).Value = Actto_time;
                    oCmd.Parameters.Add("V_APPEMPCODE", OracleDbType.Int32).Value = strAppEcode;
                    oCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = Addedby;
                    oCmd.Parameters.Add("V_OTHERATTACHMENT", OracleDbType.Varchar2).Value = OTHERATTACHMENT;
                    oCmd.Parameters.Add("V_OTHERTKTATTACHMENT", OracleDbType.Varchar2).Value = OtherTKTFile;
                    oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                    oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    oCmd.ExecuteNonQuery();
                    Settlementid = Convert.ToInt64(oCmd.Parameters["V_SETTLEMENTID"].Value.ToString());
                    strSettid = Settlementid.ToString();
                    int subResult = Convert.ToInt32(oCmd.Parameters["RESULT_OUT"].Value.ToString());
                    if (subResult != 1)
                    {
                        tran.Rollback();
                        throw new Exception("Failed to update. Transaction will rollback now.");
                    }
                    foreach (Ticket_Detail objtkt in objticket)
                    {
                        oCmd.Parameters.Clear();
                        oCmd.Transaction = tran;
                        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        oCmd.BindByName = true;
                        oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_UPDATETBSTKTDTL";
                        oCmd.Parameters.Add("V_TICKETID", OracleDbType.Int32).Direction = ParameterDirection.InputOutput;
                        oCmd.Parameters["V_TICKETID"].Value = objtkt.ID;
                        oCmd.Parameters.Add("V_SETTLEMENTID", OracleDbType.Int32).Value = Settlementid;
                        oCmd.Parameters.Add("V_TICKETDETAILID", OracleDbType.Int32).Value = objtkt.TICKETID;
                        oCmd.Parameters.Add("V_USERSTATUS", OracleDbType.Int32).Value = objtkt.TICKETSTATUS;
                        oCmd.Parameters.Add("V_SUBSTATUS", OracleDbType.Int32).Value = objtkt.TICKETSUBSTATUS == null ? (object)DBNull.Value : objtkt.TICKETSUBSTATUS;
                        oCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = Addedby;
                        oCmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = 1;
                        oCmd.Parameters.Add("V_TICKETFLAG", OracleDbType.Int32).Value = objtkt.TICKETFLAG;
                        oCmd.Parameters.Add("V_TICKETATTACHMENT", OracleDbType.Varchar2).Value = objtkt.TICKET_FILE_NAME;
                        //change done by aumento as on 28092023 for the SR50547======================================
                        //oCmd.Parameters.Add("V_ADMINVDOCNAME", OracleDbType.Varchar2).Value = objtkt.Adm_Inv_DocName;
                        oCmd.Parameters.Add("V_UPLOADTKTNAME", OracleDbType.Varchar2).Value = objtkt.UPLOAD_TICKET_FILE_NAME;
                        //============================================================================================
                        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                        oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                        oCmd.ExecuteNonQuery();
                        subResult = Convert.ToInt32(oCmd.Parameters["RESULT_OUT"].Value.ToString());
                        if (subResult != 1)
                        {
                            tran.Rollback();
                            throw new Exception("Failed to update. Transaction will rollback now.");
                        }
                    }
                    //INSRT update Hotel Details
                    foreach (Hotel_Detail Item in objHotelList)
                    {
                        oCmd.Parameters.Clear();
                        oCmd.Transaction = tran;
                        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        oCmd.BindByName = true;
                        oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.SPROC_INS_UPD_FIN_HotSetlDetCA";
                        oCmd.Parameters.Add("V_HSetlDetBokbyCAID", OracleDbType.Int64).Value = Item.HSETLDETBOKBYCAID;
                        // oCmd.Parameters["V_HSetlDetBokbyCAID"].Value = Item.HSetlDetBokbyCAID;
                        oCmd.Parameters.Add("V_ADTOURREQUESTID", OracleDbType.Int64).Value = Item.ADTOURREQUESTID;
                        //oCmd.Parameters.Add("V_SETTLEMENTID", OracleDbType.Int32).Value = Settlementid;
                        // oCmd.Parameters.Add("V_H_SETTLEMENTID", OracleDbType.Int32).Value = Item.HSETTLEMENTID;
                        oCmd.Parameters.Add("V_H_SETTLEMENTID", OracleDbType.Int64).Value = Settlementid;
                        oCmd.Parameters.Add("V_BookedBy", OracleDbType.Int16).Value = Item.BOOKEDBY;
                        oCmd.Parameters.Add("V_STATEID", OracleDbType.Int32).Value = Item.StateID; //== null ? (object)DBNull.Value : Item.TICKETSUBSTATUS;
                                                                                                   // oCmd.Parameters.Add("V_STAYINGCITYID", OracleDbType.Int32).Value = Item.StayingCityID;
                        oCmd.Parameters.Add("V_HOTELID", OracleDbType.Int32).Value = Item.HotelID;
                        oCmd.Parameters.Add("V_HOTEL_NAME", OracleDbType.Varchar2).Value = Item.HOTEL_NAME;
                        oCmd.Parameters.Add("V_GST_NO", OracleDbType.Varchar2).Value = Item.GST_NO;
                        oCmd.Parameters.Add("V_HOTEL_ADDRESS", OracleDbType.Varchar2).Value = Item.HOTEL_ADDRESS;
                        oCmd.Parameters.Add("V_CHECKINDATE", OracleDbType.Date).Value = Item.Checkindate;
                        oCmd.Parameters.Add("V_CHECKOUTDATE", OracleDbType.Date).Value = Item.Checkoutdate;
                        oCmd.Parameters.Add("V_HOTELSTAY", OracleDbType.Int16).Value = Item.HOTELSTAY == false ? 0 : 1;
                        oCmd.Parameters.Add("V_INVOICENUMBER", OracleDbType.Varchar2).Value = Item.INVOICENUMBER;
                        oCmd.Parameters.Add("V_INVOICEDATE", OracleDbType.Date).Value = Item.INVOICEDATE; // == null ? (object)DBNull.Value : Item.TICKETSUBSTATUS;
                        oCmd.Parameters.Add("V_STATUS", OracleDbType.Int16).Value = 1;
                        oCmd.Parameters.Add("V_CREATED_BY", OracleDbType.Int32).Value = Addedby;
                        oCmd.Parameters.Add("V_HOTELATTACHMENT", OracleDbType.Varchar2).Value = Item.HOTEL_FILE_NAME;
                        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                        oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                        oCmd.ExecuteNonQuery();
                        subResult = Convert.ToInt32(oCmd.Parameters["RESULT_OUT"].Value.ToString());

                        if (subResult != 1)
                        {
                            tran.Rollback();
                            throw new Exception("Failed to update. Transaction will rollback now.");
                        }
                        //// Added By vishal On 11-Jul-2022
                        else if (Item.BOOKEDBY == 2)
                        {
                            if (!string.IsNullOrEmpty(Item.HOTEL_FILE_NAME) && Item.HOTEL_FILE_BYTE != null)
                            {
                                System.IO.File.WriteAllBytes(Item.HOTEL_FILE_PATH + Item.HOTEL_FILE_NAME, Item.HOTEL_FILE_BYTE);
                            }
                        }
                        //// End
                    }
                    double Settdaytotal = 0.0;
                    foreach (Tour_Day_Detail o in obj)
                    {
                        //Insert Tour Bill Settlement Day Detail
                        oCmd.Parameters.Clear();
                        oCmd.Transaction = tran;
                        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        oCmd.BindByName = true;
                        //oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_UPDATETBSDAYDETAIL";
                        oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.NEW_SPROC_UPDATETBSDAYDETAIL";
                        oCmd.Parameters.Add("V_TBSDAYDTLID", OracleDbType.Int32).Direction = ParameterDirection.InputOutput;
                        oCmd.Parameters["V_TBSDAYDTLID"].Value = o.ID;
                        oCmd.Parameters.Add("V_SETTLEMENTID", OracleDbType.Int32).Value = Settlementid;
                        oCmd.Parameters.Add("V_ADTOURREQUESTID", OracleDbType.Int32).Value = o.TourID;
                        oCmd.Parameters.Add("V_SETTLEMENTDATE", OracleDbType.Date).Value = o.Tourdate;
                        oCmd.Parameters.Add("V_TICKETFAIR", OracleDbType.Int32).Value = o.Ticket_Fair;
                        oCmd.Parameters.Add("V_HTL_AMOUNT", OracleDbType.Int32).Value = o.Hotel_Amt;
                        oCmd.Parameters.Add("V_BRKFAST_FLAG", OracleDbType.Int32).Value = o.BRK_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_BRKFAST_PER", OracleDbType.Int32).Value = o.BRK_PERC;
                        oCmd.Parameters.Add("V_LUNCH_FLAG", OracleDbType.Int32).Value = o.Lunch_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_LUNCH_PER", OracleDbType.Int32).Value = o.LUNCH_PERC;
                        oCmd.Parameters.Add("V_DINNER_FLAG", OracleDbType.Int32).Value = o.Dinner_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_DINNER_PER", OracleDbType.Int32).Value = o.DINNER_PERC;
                        oCmd.Parameters.Add("V_MISSALLOW_FLAG", OracleDbType.Int32).Value = o.MissAllow_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_MISSALLOW_PER", OracleDbType.Int32).Value = o.MISSALLOW_PERC;
                        oCmd.Parameters.Add("V_NTA_FLAG", OracleDbType.Int32).Value = o.NTA_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_NTA_AMOUNT", OracleDbType.Int32).Value = o.NTA_Amt;
                        oCmd.Parameters.Add("V_LA_AMOUNT", OracleDbType.Int32).Value = o.LA_Amt_Actual;
                        oCmd.Parameters.Add("V_HA_FLAG", OracleDbType.Int32).Value = o.HA_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_HA_AMOUNT", OracleDbType.Int32).Value = o.HA_Amt;
                        oCmd.Parameters.Add("V_OTHEXPAMOUNT", OracleDbType.Int32).Value = o.Miss_Amt;
                        oCmd.Parameters.Add("V_OTHEXPDETAIL", OracleDbType.Varchar2).Value = o.Exp_detail == null ? "" : o.Exp_detail;
                        oCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = Addedby;
                        oCmd.Parameters.Add("V_REMARK", OracleDbType.Varchar2).Value = o.Remark == null ? "" : o.Remark;

                        oCmd.Parameters.Add("V_FIN_TICKETFAIR", OracleDbType.Int32).Value = o.Fin_Ticket_Fair;
                        oCmd.Parameters.Add("V_FIN_HOTEL_AMOUNT", OracleDbType.Int32).Value = o.Fin_Hotel_Amt;
                        oCmd.Parameters.Add("V_FIN_BRKFAST_FLAG", OracleDbType.Int32).Value = o.Fin_BRK_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_FIN_LUNCH_FLAG", OracleDbType.Int32).Value = o.Fin_Lunch_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_FIN_DINNER_FLAG", OracleDbType.Int32).Value = o.Fin_Dinner_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_FIN_MISSALLOW_FLAG", OracleDbType.Int32).Value = o.Fin_MissAllow_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_FIN_NTA_FLAG", OracleDbType.Int32).Value = o.Fin_NTA_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_FIN_LA_AMOUNT", OracleDbType.Int32).Value = o.Fin_LA_Amt_Actual;
                        oCmd.Parameters.Add("V_FIN_HA_FLAG", OracleDbType.Int32).Value = o.Fin_HA_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_FIN_OTHEXPAMOUNT", OracleDbType.Int32).Value = o.Fin_Miss_Amt;
                        oCmd.Parameters.Add("V_FIN_REMARK", OracleDbType.Varchar2).Value = o.Fin_Remark == null ? "" : o.Fin_Remark;
                        oCmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = 1;

                        oCmd.Parameters.Add("V_HOTElAMTTOT", OracleDbType.Int32).Value = o.HotelAmtTot;
                        oCmd.Parameters.Add("V_OTHERAMT", OracleDbType.Int32).Value = o.OtherAmt;
                        //oCmd.Parameters.Add("V_CGSTRATE", OracleDbType.Int32).Value = (o.CGSTPercent * 100);
                        //oCmd.Parameters.Add("V_SGSTRATE", OracleDbType.Int32).Value = (o.SGSTPercent * 100);

                        oCmd.Parameters.Add("V_CGSTRATE", OracleDbType.Int32).Value = o.CGSTRateID;
                        oCmd.Parameters.Add("V_SGSTRATE", OracleDbType.Int32).Value = o.SGSTRateID;

                        oCmd.Parameters.Add("V_MISCELLANEOUSAMT", OracleDbType.Int64).Value = o.Miscellaneous_Amt;
                        oCmd.Parameters.Add("V_TICKETBILL_ATTACHMENT", OracleDbType.Varchar2).Value = o.Ticket_FileName;

                        //SR68656 change start
                        oCmd.Parameters.Add("V_ACTUAL_FOOD_AMT", OracleDbType.Int64).Value = o.Actual_Food_Amt;
                        oCmd.Parameters.Add("V_ACTUAL_MIS_AMT", OracleDbType.Int64).Value = o.Actual_Mis_Amt;
                        //SR68656 change end

                        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                        oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                        oCmd.ExecuteNonQuery();
                        subResult = Convert.ToInt32(oCmd.Parameters["RESULT_OUT"].Value.ToString());
                        if (subResult != 1)
                        {
                            tran.Rollback();
                            throw new Exception("Failed to update. Transaction will rollback now.");
                        }
                        //// Added By vishal On 29-Jul-2022
                        else
                        {
                            if (!string.IsNullOrEmpty(o.Ticket_FileName) && o.Ticket_FileByte != null)
                            {
                                System.IO.File.WriteAllBytes(o.Ticket_FilePath + o.Ticket_FileName, o.Ticket_FileByte);
                            }
                        }
                        //// End

                        //Insert Tour Bill Settlement Conveyance Detail
                        if (o.Conveyance != null)
                        {
                            foreach (Conveyance_Detail oConv in o.Conveyance)
                            {
                                oCmd.Parameters.Clear();
                                oCmd.Transaction = tran;
                                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                                oCmd.BindByName = true;
                                oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_UPDATETBSCONVDTL";
                                oCmd.Parameters.Add("V_CONVEYANCEID", OracleDbType.Int32).Direction = ParameterDirection.InputOutput;
                                oCmd.Parameters["V_CONVEYANCEID"].Value = oConv.ID;
                                oCmd.Parameters.Add("V_SETTLEMENTID", OracleDbType.Int32).Value = Settlementid;
                                oCmd.Parameters.Add("V_CONVEYANCE_DATE", OracleDbType.Date).Value = o.Tourdate;
                                oCmd.Parameters.Add("V_CITY_NAME", OracleDbType.Varchar2).Value = oConv.City;
                                oCmd.Parameters.Add("V_LOC_FROM", OracleDbType.Varchar2).Value = oConv.From_Loc;
                                oCmd.Parameters.Add("V_LOC_TO", OracleDbType.Varchar2).Value = oConv.To_Loc;
                                oCmd.Parameters.Add("V_MODEOFTRANSPORT", OracleDbType.Varchar2).Value = oConv.Mode_Tran;
                                oCmd.Parameters.Add("V_AMOUNT", OracleDbType.Int32).Value = oConv.Conv_Amt;
                                oCmd.Parameters.Add("V_FIN_AMOUNT", OracleDbType.Int32).Value = oConv.Fin_Conv_Amt;
                                oCmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = 1;
                                oCmd.Parameters.Add("V_ADDDEDBY", OracleDbType.Int32).Value = Addedby;
                                oCmd.Parameters.Add("V_CONV_FILE", OracleDbType.Varchar2).Value = oConv.CONV_FILE_NAME;
                                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                                oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                                oCmd.ExecuteNonQuery();
                                subResult = Convert.ToInt32(oCmd.Parameters["RESULT_OUT"].Value.ToString());
                                string err_msg = Convert.ToString(oCmd.Parameters["ERROR_MSG"].Value);
                                if (subResult != 1)
                                {
                                    tran.Rollback();
                                    throw new Exception("Failed to update. Transaction will rollback now.");
                                }
                                //// Added By Vishal On 12-July-2022
                                else
                                {
                                    if (!string.IsNullOrEmpty(oConv.CONV_FILE_NAME) && oConv.CONV_FILE_BYTE != null)
                                    {
                                        System.IO.File.WriteAllBytes(oConv.CONV_FILE_PATH + oConv.CONV_FILE_NAME, oConv.CONV_FILE_BYTE);
                                    }
                                }
                                ////End
                            }
                        }
                        //Insert Tour Bill Settlement Visited Place Detail
                        if (o.Places != null)
                        {
                            foreach (Place_Detail oPlace in o.Places)
                            {
                                oCmd.Parameters.Clear();
                                oCmd.Transaction = tran;
                                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                                oCmd.BindByName = true;
                                oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_UPDATETBSVISITEDPLACE";
                                oCmd.Parameters.Add("V_VISITID", OracleDbType.Int32).Direction = ParameterDirection.InputOutput;
                                oCmd.Parameters["V_VISITID"].Value = oPlace.ID;
                                oCmd.Parameters.Add("V_SETTLEMENTID", OracleDbType.Int32).Value = Settlementid;
                                oCmd.Parameters.Add("V_VISITDATE", OracleDbType.Date).Value = o.Tourdate;
                                oCmd.Parameters.Add("V_CITY", OracleDbType.Varchar2).Value = oPlace.City;
                                oCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = Addedby;
                                oCmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = 1;
                                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                                oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                                oCmd.ExecuteNonQuery();
                                subResult = Convert.ToInt32(oCmd.Parameters["RESULT_OUT"].Value.ToString());
                                if (subResult != 1)
                                {
                                    tran.Rollback();
                                    throw new Exception("Failed to update. Transaction will rollback now.");
                                }
                            }
                        }
                        Settdaytotal += Convert.ToDouble(o.DayTotal == "" ? "0" : o.DayTotal);
                    }

                    oCmd.Parameters.Clear();
                    oCmd.Transaction = tran;
                    oCmd.CommandType = System.Data.CommandType.Text;
                    oCmd.CommandText = "UPDATE FINTBS_TRANS SET SETTDAYTOTAL=" + Settdaytotal.ToString() + " where SETTLEMENTID=" + Settlementid;
                    oCmd.ExecuteNonQuery();

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
        #endregion
        #region "Update Tour Settlement Detail Ticket Detail(By User)"
        public void UpdateTBSTktdtl_ByUser(/*GridViewRowCollection grcol,*/ string strAddedby, string strStlmntid)
        {
            String strErrMsg = String.Empty;
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            OracleTransaction tran;
            //using (OracleConnection objCn = new OracleConnection())
            //{
            //    objCn.ConnectionString = strCn;
            //    objCn.Open();
            //    tran = objCn.BeginTransaction();
            //    try
            //    {
            //        bool flag = false;
            //        foreach (GridViewRow gr in grcol)
            //        {
            //            string strUsrsubstatus = "";
            //            RadioButton rdbiom = (RadioButton)gr.FindControl("chkIOM");
            //            RadioButton rdbadjust = (RadioButton)gr.FindControl("chkadjust");
            //            HiddenField hdntktid = (HiddenField)gr.FindControl("hdntktid");
            //            HiddenField hdntktdtlid = (HiddenField)gr.FindControl("hdntktdtlid");
            //            HiddenField hdntktflag = (HiddenField)gr.FindControl("hdnTktflag");
            //            DropDownList ddluserstatus = (DropDownList)gr.FindControl("ddlstatus");
            //            string strTicketid = hdntktid.Value;
            //            string strUserstatus = ddluserstatus.SelectedValue;
            //            if (ddluserstatus.SelectedValue == "2")
            //            {
            //                strUsrsubstatus = rdbiom.Checked == true ? "1" : "2";
            //            }
            //            OracleCommand oCmd = new OracleCommand();
            //            oCmd.Connection = objCn;
            //            oCmd.Parameters.Clear();
            //            oCmd.Transaction = tran;
            //            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            //            oCmd.BindByName = true;
            //            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_UPDATETBSTKTDTL";
            //            oCmd.Parameters.Add("V_TICKETID", OracleDbType.Int32).Direction = ParameterDirection.InputOutput;
            //            oCmd.Parameters["V_TICKETID"].Value = hdntktid.Value;
            //            oCmd.Parameters.Add("V_SETTLEMENTID", OracleDbType.Int32).Value = strStlmntid;
            //            oCmd.Parameters.Add("V_TICKETDETAILID", OracleDbType.Int32).Value = hdntktdtlid.Value;
            //            oCmd.Parameters.Add("V_USERSTATUS", OracleDbType.Int32).Value = ddluserstatus.SelectedValue;
            //            oCmd.Parameters.Add("V_SUBSTATUS", OracleDbType.Int32).Value = strUsrsubstatus == "" ? (object)DBNull.Value : strUsrsubstatus;
            //            oCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = strAddedby;
            //            oCmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = 1;
            //            oCmd.Parameters.Add("V_TICKETFLAG", OracleDbType.Int32).Value = hdntktflag.Value;
            //            oCmd.Parameters.Add("V_TICKETATTACHMENT", OracleDbType.Varchar2).Value = "";
            //            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            //            oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            //            oCmd.ExecuteNonQuery();
            //            int subResult = Convert.ToInt32(oCmd.Parameters["RESULT_OUT"].Value.ToString());
            //            if (subResult != 1)
            //            {
            //                tran.Rollback();
            //                throw new Exception("Failed to update. Transaction will rollback now.");
            //            }
            //            if (ddluserstatus.SelectedValue != "1")
            //            {
            //                flag = true;
            //            }
            //        }
            //        string strAppstatus = "1";
            //        if (flag == true)
            //            strAppstatus = "0";

            //        string strResult = UpdateApproval("1", strStlmntid, strAppstatus, "By User", strAddedby);
            //        string[] errStatus = strResult.Split(new Char[] { '#' });
            //        string errResult = Convert.ToString(errStatus[0]);
            //        if (errResult == "1")
            //        {
            //            tran.Commit();
            //        }
            //        if (errResult == "0")
            //        {
            //            tran.Rollback();
            //        }

            //    }
            //    catch (Exception ex)
            //    {
            //        try
            //        {
            //            tran.Rollback();
            //        }
            //        catch (OracleException x)
            //        {
            //            if (tran.Connection != null)
            //            {
            //                throw new Exception("An exception of type " + x.GetType() + " was encountered while attempting to roll back the transaction.");
            //            }
            //        }
            //        throw new Exception(ex.ToString());
            //    }
            //    finally
            //    {
            //        if (objCn != null)
            //        {
            //            objCn.Close();
            //        }
            //    }
            //}
        }
        #endregion

        #region "Update Approval Status"
        public string UpdateApproval(string strAPPempcode, string strSettlementID, string strStatus, string strRemark, string strAddedby)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_UPDATETBSAPPTRANS";
            oCmd.Parameters.Add("V_FINTBS_APPTRANSID", OracleDbType.Int32).Value = 0;
            oCmd.Parameters.Add("V_SETTLEMENTID", OracleDbType.Int32).Value = strSettlementID;
            oCmd.Parameters.Add("V_APPEMPCODE", OracleDbType.Int32).Value = strAPPempcode;
            oCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = strAddedby;
            oCmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = strStatus;
            oCmd.Parameters.Add("V_APPREMARK", OracleDbType.Varchar2).Value = strRemark;
            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            oDataMgmt.ExecuteQuery(oCmd);
            string strErr = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERROR_MSG"].Value.ToString();
            return strErr;
        }
        #endregion

        #region "Cancel Tour Bill Settlement Request"
        public string CancelRequest(string strSettlementID, string strRemark, string strAddedby)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_CANCELREQUEST";
            oCmd.Parameters.Add("V_SETTLEMENTID", OracleDbType.Int32).Value = strSettlementID;
            oCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = strAddedby;
            oCmd.Parameters.Add("V_APPREMARK", OracleDbType.Varchar2).Value = strRemark;
            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            oDataMgmt.ExecuteQuery(oCmd);
            string strErr = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERROR_MSG"].Value.ToString();
            return strErr;
        }
        #endregion

        #region "Update Bill Attachment Status"
        public string UpdateBillFlag(string strSettlementID, string strTicketFlag, string strHotelFlag, string strTaxiFlag, string strBoardBill_Flag)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_UPDATETBSBILLFLAG";
            oCmd.Parameters.Add("V_SETTLEMENTID", OracleDbType.Int32).Value = strSettlementID;
            oCmd.Parameters.Add("V_BOARDBILL_FLAG", OracleDbType.Int32).Value = strBoardBill_Flag;
            oCmd.Parameters.Add("V_TAXIBILL_FLAG", OracleDbType.Int32).Value = strTaxiFlag;
            oCmd.Parameters.Add("V_HOTELBILL_FLAG", OracleDbType.Int32).Value = strHotelFlag;
            oCmd.Parameters.Add("V_TICKETBILL_FLAG", OracleDbType.Int32).Value = strTicketFlag;
            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            oDataMgmt.ExecuteQuery(oCmd);
            string strErr = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERROR_MSG"].Value.ToString();
            return strErr;
        }
        #endregion

        #region "Update Ticket Approval Status(By Admin)"
        public void UpdateTktAppDtl(/*GridViewRowCollection grcol, */string strAddedby, string strStlmntid, string strStatus)
        {
            String strErrMsg = String.Empty;
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            OracleTransaction tran;
            //using (OracleConnection objCn = new OracleConnection())
            //{
            //    objCn.ConnectionString = strCn;
            //    objCn.Open();
            //    tran = objCn.BeginTransaction();
            //    try
            //    {
            //        foreach (GridViewRow gr in grcol)
            //        {


            //            HiddenField hdntktid = (HiddenField)gr.FindControl("hdntktid");
            //            DropDownList ddladmin = (DropDownList)gr.FindControl("ddladminstatus");
            //            DropDownList ddlrefstatus = (DropDownList)gr.FindControl("ddlrefstatus");
            //            TextBox txtremark = (TextBox)gr.FindControl("txtremark");
            //            TextBox txtRefAmt = (TextBox)gr.FindControl("txtrefamount");
            //            string strTicketid = hdntktid.Value;
            //            string strRemark = txtremark.Text;
            //            string strAdminstatus = ddladmin.SelectedValue;
            //            string strIsrefAvail = ddlrefstatus.SelectedValue;
            //            string strRefamt = txtRefAmt.Text;
            //            OracleCommand oCmd = new OracleCommand();
            //            oCmd.Connection = objCn;
            //            oCmd.Transaction = tran;
            //            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            //            oCmd.BindByName = true;
            //            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_UPDATETBSTKTDTL_BYADMIN";
            //            oCmd.Parameters.Add("V_TICKETID", OracleDbType.Int32).Direction = ParameterDirection.InputOutput;
            //            oCmd.Parameters["V_TICKETID"].Value = strTicketid;
            //            oCmd.Parameters.Add("V_REMARK", OracleDbType.Varchar2).Value = strRemark;
            //            oCmd.Parameters.Add("V_ADMINSTATUS", OracleDbType.Int32).Value = strAdminstatus;
            //            oCmd.Parameters.Add("V_ISREFUNDAVAIL", OracleDbType.Int32).Value = strIsrefAvail == "" ? (object)DBNull.Value : strIsrefAvail;
            //            oCmd.Parameters.Add("V_REFUNDAMT", OracleDbType.Int32).Value = strRefamt == "" ? (object)DBNull.Value : strRefamt;
            //            oCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = strAddedby;
            //            oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
            //            oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            //            oCmd.ExecuteNonQuery();
            //            //strTicketid = Convert.ToInt32(oCmd.Parameters["V_TICKETID"].Value);

            //            int subResult = Convert.ToInt32(oCmd.Parameters["RESULT_OUT"].Value.ToString());
            //            if (subResult != 1)
            //            {
            //                tran.Rollback();
            //                throw new Exception("Failed to update. Transaction will rollback now.");
            //            }
            //        }
            //        string strResult = UpdateApproval("1", strStlmntid, strStatus, "By Admin", strAddedby);
            //        string[] errStatus = strResult.Split(new Char[] { '#' });
            //        string errResult = Convert.ToString(errStatus[0]);
            //        if (errResult == "1")
            //        {
            //            tran.Commit();
            //        }
            //        if (errResult == "0")
            //        {
            //            tran.Rollback();
            //        }

            //    }
            //    catch (Exception ex)
            //    {
            //        try
            //        {
            //            tran.Rollback();
            //        }
            //        catch (OracleException x)
            //        {
            //            if (tran.Connection != null)
            //            {
            //                throw new Exception("An exception of type " + x.GetType() + " was encountered while attempting to roll back the transaction.");
            //            }
            //        }
            //        throw new Exception(ex.ToString());
            //    }
            //    finally
            //    {
            //        if (objCn != null)
            //        {
            //            objCn.Close();
            //        }
            //    }
            //}
        }
        #endregion


        #region "Add Ticket Detail"


        public string AddTicketDetail(string strTourDtlid, string strEmpcode, string strTvlDate, string strTvlTime, string strFromcity, string strTocity, string strTvlmode,
                                      string strflightno, string strTvlclass, string strTktNo, string strTktAmt, string strRemark, string strIssuedate,
                                      string strPnrno, string strTkttype, string straddedby, string strStatus, string strTvlStatus,
                                      string strFlightName, string strPrefixCode, string strInvoiceNo, string strInvoiceDate, string strInoiceAmount, string strGSTAmount, string strInvTotalAmount,
                                      string strFlightDepTime, string strFlightDepDate, string strImpInfo, string TicketfileName, string TicketfileName2)
        {
            try
            {
                OracleCommand oCmd = new OracleCommand();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.BindByName = true;
                oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_UPDATETBSTKTBOOK_BYADMIN";
                oCmd.Parameters.Add("V_ADTOURREQDETAILID", OracleDbType.Int32).Direction = ParameterDirection.InputOutput;
                oCmd.Parameters["V_ADTOURREQDETAILID"].Value = strTourDtlid;
                oCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode;
                oCmd.Parameters.Add("V_TRAVELFROMDATE", OracleDbType.Varchar2).Value = strTvlDate;
                oCmd.Parameters.Add("V_TRAVELTIME", OracleDbType.Varchar2).Value = strTvlTime;
                oCmd.Parameters.Add("V_FROMCITYID", OracleDbType.Varchar2).Value = strFromcity;
                oCmd.Parameters.Add("V_TOCITYID", OracleDbType.Varchar2).Value = strTocity;
                oCmd.Parameters.Add("V_ADTRAVELMODEID", OracleDbType.Int32).Value = strTvlmode;
                oCmd.Parameters.Add("V_FLIGHTTRAINNO", OracleDbType.Varchar2).Value = strflightno;
                oCmd.Parameters.Add("V_ADTRAVELMODECLASSID", OracleDbType.Int32).Value = strTvlclass == "" ? (object)DBNull.Value : strTvlclass;
                oCmd.Parameters.Add("V_TICKETNO", OracleDbType.Varchar2).Value = strTktNo;
                oCmd.Parameters.Add("V_TICKETAMOUNT", OracleDbType.Int32).Value = strTktAmt;
                oCmd.Parameters.Add("V_REMARKS", OracleDbType.Varchar2).Value = strRemark;
                oCmd.Parameters.Add("V_TICKETISSUEDATE", OracleDbType.Varchar2).Value = strIssuedate;
                oCmd.Parameters.Add("V_PNRNO", OracleDbType.Varchar2).Value = strPnrno;
                oCmd.Parameters.Add("V_TICKETTYPE", OracleDbType.Varchar2).Value = strTkttype;
                oCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = straddedby;
                oCmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = strStatus;
                oCmd.Parameters.Add("V_TVLSTATUS", OracleDbType.Int32).Value = strTvlStatus;

                //Below fields added by the aumento as on 07062023 for SR50547====================================================================
                oCmd.Parameters.Add("V_FLIGHTNAME", OracleDbType.Varchar2).Value = strFlightName;
                oCmd.Parameters.Add("V_PREFIXCODE", OracleDbType.Varchar2).Value = strPrefixCode;
                oCmd.Parameters.Add("V_INVOICENO", OracleDbType.Varchar2).Value = strInvoiceNo;
                oCmd.Parameters.Add("V_INVOICEDATE", OracleDbType.Varchar2).Value = strInvoiceDate;
                oCmd.Parameters.Add("V_INVOICEAMOUNT", OracleDbType.Varchar2).Value = strInoiceAmount;
                oCmd.Parameters.Add("V_GSTAMOUNT", OracleDbType.Int32).Value = strGSTAmount;
                oCmd.Parameters.Add("V_INVTOTALAMOUNT", OracleDbType.Int32).Value = strInvTotalAmount;
                oCmd.Parameters.Add("V_FLIGHTDEPTIME", OracleDbType.Varchar2).Value = strFlightDepTime;
                oCmd.Parameters.Add("V_FLIGHTDEPDATE", OracleDbType.Varchar2).Value = strFlightDepDate;
                oCmd.Parameters.Add("V_IMPINFO", OracleDbType.Varchar2).Value = strImpInfo;
                oCmd.Parameters.Add("V_TicketfileName1", OracleDbType.Varchar2).Value = TicketfileName;
                oCmd.Parameters.Add("V_TicketfileName2", OracleDbType.Varchar2).Value = TicketfileName2;

                //=================================================================================================================================

                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oDataMgmt.ExecuteQuery(oCmd);
                string strErr = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERROR_MSG"].Value.ToString();
                return strErr;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();

            }
        }
        #endregion

        // #region "Add Ticket Detail"
        // public string AddTicketDetail(string strTourDtlid, string strEmpcode, string strTvlDate, string strTvlTime, string strFromcity, string strTocity, string strTvlmode,
        // string strflightno, string strTvlclass, string strTktNo, string strTktAmt, string strRemark, string strIssuedate,
        // string strPnrno, string strTkttype, string straddedby, string strStatus, string strTvlStatus
        // )
        // {
        // OracleCommand oCmd = new OracleCommand();
        // oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        // oCmd.BindByName = true;
        // oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_UPDATETBSTKTBOOK_BYADMIN";
        // oCmd.Parameters.Add("V_ADTOURREQDETAILID", OracleDbType.Int32).Direction = ParameterDirection.InputOutput;
        // oCmd.Parameters["V_ADTOURREQDETAILID"].Value = strTourDtlid;
        // oCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode;
        // oCmd.Parameters.Add("V_TRAVELFROMDATE", OracleDbType.Varchar2).Value = strTvlDate;
        // oCmd.Parameters.Add("V_TRAVELTIME", OracleDbType.Varchar2).Value = strTvlTime;
        // oCmd.Parameters.Add("V_FROMCITYID", OracleDbType.Varchar2).Value = strFromcity;
        // oCmd.Parameters.Add("V_TOCITYID", OracleDbType.Varchar2).Value = strTocity;
        // oCmd.Parameters.Add("V_ADTRAVELMODEID", OracleDbType.Int32).Value = strTvlmode;
        // oCmd.Parameters.Add("V_FLIGHTTRAINNO", OracleDbType.Varchar2).Value = strflightno;
        // oCmd.Parameters.Add("V_ADTRAVELMODECLASSID", OracleDbType.Int32).Value = strTvlclass == "" ? (object)DBNull.Value : strTvlclass;
        // oCmd.Parameters.Add("V_TICKETNO", OracleDbType.Varchar2).Value = strTktNo;
        // oCmd.Parameters.Add("V_TICKETAMOUNT", OracleDbType.Int32).Value = strTktAmt;
        // oCmd.Parameters.Add("V_REMARKS", OracleDbType.Varchar2).Value = strRemark;
        // oCmd.Parameters.Add("V_TICKETISSUEDATE", OracleDbType.Varchar2).Value = strIssuedate;
        // oCmd.Parameters.Add("V_PNRNO", OracleDbType.Varchar2).Value = strPnrno;
        // oCmd.Parameters.Add("V_TICKETTYPE", OracleDbType.Varchar2).Value = strTkttype;
        // oCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = straddedby;
        // oCmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = strStatus;
        // oCmd.Parameters.Add("V_TVLSTATUS", OracleDbType.Int32).Value = strTvlStatus;
        // oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
        // oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
        // oDataMgmt.ExecuteQuery(oCmd);
        // string strErr = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERROR_MSG"].Value.ToString();
        // return strErr;
        // }
        // #endregion

        #region "Update Bill Status"
        public string UpdateBillStatus(string straddedby, ArrayList strStlId, string strStatus)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                OracleCommand oCmd = new OracleCommand();
                oCmd.Connection = objCn;
                for (int i = 0; i < strStlId.Count; i++)
                {
                    oCmd.Parameters.Clear();
                    string strsettlementid = strStlId[i].ToString();
                    oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    oCmd.BindByName = true;
                    oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_UPDATETBSBILLSTATUS";
                    oCmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = strStatus;
                    oCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = straddedby;
                    oCmd.Parameters.Add("V_SETTLEMENTID", OracleDbType.Int32).Value = strsettlementid;
                    oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                    oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    oCmd.ExecuteNonQuery();
                    //oDataMgmt.ExecuteQuery(oCmd);
                    //string strErr = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERROR_MSG"].Value.ToString();
                    //return strErr;
                }
            }
            return "";
        }
        #endregion

        #region "Update Tour Settlement Detail By Payable"
        public void UpdateTBS_ByPayable(string strSettlementid, string strEmpcode, string Addedby, Tour_Day_Detail[] obj, Ticket_Detail[] objticket)
        {
            String strErrMsg = String.Empty;
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            OracleTransaction tran;
            Int64 Settlementid = Convert.ToInt64(strSettlementid);
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                tran = objCn.BeginTransaction();
                try
                {
                    //insert or update TBS transaction detail
                    OracleCommand oCmd = new OracleCommand();
                    oCmd.Connection = objCn;
                    oCmd.Transaction = tran;
                    int subResult = 0;
                    foreach (Ticket_Detail objtkt in objticket)
                    {
                        oCmd.Parameters.Clear();
                        oCmd.Transaction = tran;
                        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        oCmd.BindByName = true;
                        oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_UPDATETBSTKTDTL";
                        oCmd.Parameters.Add("V_TICKETID", OracleDbType.Int64).Direction = ParameterDirection.InputOutput;
                        oCmd.Parameters["V_TICKETID"].Value = objtkt.ID;
                        oCmd.Parameters.Add("V_SETTLEMENTID", OracleDbType.Int64).Value = Settlementid;
                        oCmd.Parameters.Add("V_TICKETDETAILID", OracleDbType.Int64).Value = objtkt.TICKETID;
                        oCmd.Parameters.Add("V_USERSTATUS", OracleDbType.Int32).Value = objtkt.TICKETSTATUS;
                        oCmd.Parameters.Add("V_SUBSTATUS", OracleDbType.Int32).Value = objtkt.TICKETSUBSTATUS == null ? (object)DBNull.Value : objtkt.TICKETSUBSTATUS;
                        oCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = Addedby;
                        oCmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = 1;
                        oCmd.Parameters.Add("V_TICKETFLAG", OracleDbType.Int32).Value = objtkt.TICKETFLAG;
                        oCmd.Parameters.Add("V_TICKETATTACHMENT", OracleDbType.Varchar2).Value = objtkt.TICKET_FILE_NAME;
                        oCmd.Parameters.Add("V_UPLOADTKTNAME", OracleDbType.Varchar2).Value = objtkt.UPLOAD_TICKET_FILE_NAME;
                        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                        oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                        oCmd.ExecuteNonQuery();
                        subResult = Convert.ToInt32(oCmd.Parameters["RESULT_OUT"].Value.ToString());
                        if (subResult != 1)
                        {
                            tran.Rollback();
                            throw new Exception("Failed to update. Transaction will rollback now.");
                        }
                    }
                    double Settdaytotal = 0.0;
                    foreach (Tour_Day_Detail o in obj)
                    {
                        //update Tour Bill Settlement Day Detail
                        oCmd.Parameters.Clear();
                        oCmd.Transaction = tran;
                        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        oCmd.BindByName = true;
                        oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_UPDATETBSDAYDETAIL";
                        oCmd.Parameters.Add("V_TBSDAYDTLID", OracleDbType.Int32).Direction = ParameterDirection.InputOutput;
                        oCmd.Parameters["V_TBSDAYDTLID"].Value = o.ID;
                        oCmd.Parameters.Add("V_SETTLEMENTID", OracleDbType.Int32).Value = Settlementid;
                        oCmd.Parameters.Add("V_ADTOURREQUESTID", OracleDbType.Int32).Value = o.TourID;
                        oCmd.Parameters.Add("V_SETTLEMENTDATE", OracleDbType.Date).Value = o.Tourdate;
                        oCmd.Parameters.Add("V_TICKETFAIR", OracleDbType.Int32).Value = o.Ticket_Fair;
                        oCmd.Parameters.Add("V_HTL_AMOUNT", OracleDbType.Int32).Value = o.Hotel_Amt;
                        oCmd.Parameters.Add("V_BRKFAST_FLAG", OracleDbType.Int32).Value = o.BRK_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_BRKFAST_PER", OracleDbType.Int32).Value = o.BRK_PERC;
                        oCmd.Parameters.Add("V_LUNCH_FLAG", OracleDbType.Int32).Value = o.Lunch_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_LUNCH_PER", OracleDbType.Int32).Value = o.LUNCH_PERC;
                        oCmd.Parameters.Add("V_DINNER_FLAG", OracleDbType.Int32).Value = o.Dinner_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_DINNER_PER", OracleDbType.Int32).Value = o.DINNER_PERC;
                        oCmd.Parameters.Add("V_MISSALLOW_FLAG", OracleDbType.Int32).Value = o.MissAllow_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_MISSALLOW_PER", OracleDbType.Int32).Value = o.MISSALLOW_PERC;
                        oCmd.Parameters.Add("V_NTA_FLAG", OracleDbType.Int32).Value = o.NTA_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_NTA_AMOUNT", OracleDbType.Int32).Value = o.NTA_Amt;
                        oCmd.Parameters.Add("V_LA_AMOUNT", OracleDbType.Int32).Value = o.LA_Amt_Actual;
                        oCmd.Parameters.Add("V_HA_FLAG", OracleDbType.Int32).Value = o.HA_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_HA_AMOUNT", OracleDbType.Int32).Value = o.HA_Amt;
                        oCmd.Parameters.Add("V_OTHEXPAMOUNT", OracleDbType.Int32).Value = o.Miss_Amt;
                        oCmd.Parameters.Add("V_OTHEXPDETAIL", OracleDbType.Varchar2).Value = o.Exp_detail == null ? "" : o.Exp_detail;
                        oCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = Addedby;
                        oCmd.Parameters.Add("V_REMARK", OracleDbType.Varchar2).Value = o.Remark == null ? "" : o.Remark;

                        oCmd.Parameters.Add("V_FIN_TICKETFAIR", OracleDbType.Int32).Value = o.Fin_Ticket_Fair;
                        oCmd.Parameters.Add("V_FIN_HOTEL_AMOUNT", OracleDbType.Int32).Value = o.Fin_Hotel_Amt;
                        oCmd.Parameters.Add("V_FIN_BRKFAST_FLAG", OracleDbType.Int32).Value = o.Fin_BRK_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_FIN_LUNCH_FLAG", OracleDbType.Int32).Value = o.Fin_Lunch_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_FIN_DINNER_FLAG", OracleDbType.Int32).Value = o.Fin_Dinner_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_FIN_MISSALLOW_FLAG", OracleDbType.Int32).Value = o.Fin_MissAllow_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_FIN_NTA_FLAG", OracleDbType.Int32).Value = o.Fin_NTA_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_FIN_LA_AMOUNT", OracleDbType.Int32).Value = o.Fin_LA_Amt_Actual;
                        oCmd.Parameters.Add("V_FIN_HA_FLAG", OracleDbType.Int32).Value = o.Fin_HA_Flag == true ? 1 : 0;
                        oCmd.Parameters.Add("V_FIN_OTHEXPAMOUNT", OracleDbType.Int32).Value = o.Fin_Miss_Amt;
                        oCmd.Parameters.Add("V_FIN_REMARK", OracleDbType.Varchar2).Value = o.Fin_Remark == null ? "" : o.Fin_Remark;

                        oCmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = 1;
                        oCmd.Parameters.Add("V_MISCELLANEOUSAMT", OracleDbType.Int64).Value = o.Miscellaneous_Amt;

                        //SR68656 change start
                        oCmd.Parameters.Add("V_FIN_ACTUAL_FOOD_AMT", OracleDbType.Int64).Value = o.Actual_Food_Amt;
                        oCmd.Parameters.Add("V_FIN_ACTUAL_MIS_AMT", OracleDbType.Int64).Value = o.Actual_Mis_Amt;
                        //SR68656 change end

                        oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                        oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                        oCmd.ExecuteNonQuery();
                        subResult = Convert.ToInt32(oCmd.Parameters["RESULT_OUT"].Value.ToString());
                        if (subResult != 1)
                        {
                            tran.Rollback();
                            throw new Exception("Failed to update. Transaction will rollback now.");
                        }

                        //Insert Tour Bill Settlement Conveyance Detail
                        if (o.Conveyance != null)
                        {
                            foreach (Conveyance_Detail oConv in o.Conveyance)
                            {
                                oCmd.Parameters.Clear();
                                oCmd.Transaction = tran;
                                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                                oCmd.BindByName = true;
                                oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_UPDATETBSCONVDTL";
                                oCmd.Parameters.Add("V_CONVEYANCEID", OracleDbType.Int32).Direction = ParameterDirection.InputOutput;
                                oCmd.Parameters["V_CONVEYANCEID"].Value = oConv.ID;
                                oCmd.Parameters.Add("V_SETTLEMENTID", OracleDbType.Int32).Value = Settlementid;
                                oCmd.Parameters.Add("V_CONVEYANCE_DATE", OracleDbType.Date).Value = o.Tourdate;
                                oCmd.Parameters.Add("V_CITY_NAME", OracleDbType.Varchar2).Value = oConv.City;
                                oCmd.Parameters.Add("V_LOC_FROM", OracleDbType.Varchar2).Value = oConv.From_Loc;
                                oCmd.Parameters.Add("V_LOC_TO", OracleDbType.Varchar2).Value = oConv.To_Loc;
                                oCmd.Parameters.Add("V_MODEOFTRANSPORT", OracleDbType.Varchar2).Value = oConv.Mode_Tran;
                                oCmd.Parameters.Add("V_AMOUNT", OracleDbType.Int32).Value = oConv.Conv_Amt;
                                oCmd.Parameters.Add("V_FIN_AMOUNT", OracleDbType.Int32).Value = oConv.Fin_Conv_Amt;
                                oCmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = 1;
                                oCmd.Parameters.Add("V_ADDDEDBY", OracleDbType.Int32).Value = Addedby;
                                oCmd.Parameters.Add("V_CONV_FILE", OracleDbType.Varchar2).Value = oConv.CONV_FILE_NAME;
                                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                                oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                                oCmd.ExecuteNonQuery();
                                subResult = Convert.ToInt32(oCmd.Parameters["RESULT_OUT"].Value.ToString());
                                string err_msg = Convert.ToString(oCmd.Parameters["ERROR_MSG"].Value);
                                if (subResult != 1)
                                {
                                    tran.Rollback();
                                    throw new Exception("Failed to update. Transaction will rollback now.");
                                }
                            }
                        }
                        Settdaytotal += Convert.ToDouble(o.Fin_DayTotal == "" ? "0" : o.Fin_DayTotal);
                    }

                    oCmd.Parameters.Clear();
                    oCmd.Transaction = tran;
                    oCmd.CommandType = System.Data.CommandType.Text;
                    oCmd.CommandText = "UPDATE FINTBS_TRANS SET SETTDAYTOTAL=" + Settdaytotal.ToString() + " where SETTLEMENTID=" + Settlementid;
                    oCmd.ExecuteNonQuery();

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

        #endregion

        #region "Update City Category"
        public string UpdateCityCategory(string strId, string strCity, string strCityCategory, string straddedby, string strStatus, string strEffectiveFrom, string strEffectiveTo)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                OracleCommand oCmd = new OracleCommand();
                oCmd.Connection = objCn;
                oCmd.Parameters.Clear();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.BindByName = true;
                oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_UPDATECITYCATEGORY";
                oCmd.Parameters.Add("V_ID", OracleDbType.Int32).Value = strId;
                oCmd.Parameters.Add("V_CITY", OracleDbType.Varchar2).Value = strCity;
                oCmd.Parameters.Add("V_SYCITYCATEGORYID", OracleDbType.Int32).Value = strCityCategory;
                oCmd.Parameters.Add("V_ACTIVE", OracleDbType.Int32).Value = strStatus;
                oCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = straddedby;
                oCmd.Parameters.Add("V_EFFECTIVEFROM", OracleDbType.Varchar2).Value = strEffectiveFrom;
                oCmd.Parameters.Add("V_EFFECTIVETO", OracleDbType.Varchar2).Value = strEffectiveTo;
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.ExecuteNonQuery();
                //oDataMgmt.ExecuteQuery(oCmd);
                string strErr = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERROR_MSG"].Value.ToString();
                return strErr;
            }

        }
        #endregion

        #region "Update Cost Center"
        public string UpdateCostCenter(string strXMLCostCenter, string straddedby)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                OracleCommand oCmd = new OracleCommand();
                oCmd.Connection = objCn;
                oCmd.Parameters.Clear();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.BindByName = true;
                oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_UPDATECOSTCENTRE";
                oCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = straddedby;
                oCmd.Parameters.Add("V_COSTCENTREXML", OracleDbType.Clob).Value = strXMLCostCenter;//"<COSTCENTRE><LVLCOSTCENTRE><ID>1</ID><NAME>CSC</NAME></LVLCOSTCENTRE></COSTCENTRE>";
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.ExecuteNonQuery();
                //oDataMgmt.ExecuteQuery(oCmd);
                string strErr = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERROR_MSG"].Value.ToString();
                return strErr;
            }

        }
        #endregion

        #region "Get Data Result"

        /// <summary>
        /// GET ACTIVE CITY LIST
        /// </summary>
        /// <returns></returns> 
        public DataTable GetPendingTourReq(string strEmpcode)
        {
            OracleCommand oCmd = new OracleCommand();
            dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_PENDINGTOUREQ_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpcode;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);

        }
        public DataTable GetUserTicketDetail(string strTourReqID)
        {
            OracleCommand oCmd = new OracleCommand();
            dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_USERTKTDTL_GET";
            oCmd.Parameters.Add("TOURREQID", OracleDbType.Varchar2).Value = strTourReqID;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);

        }
        public DataTable GetUserTourReqDtl(string strTourReqID)
        {
            OracleCommand oCmd = new OracleCommand();
            dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_USRTOURREQDETAIL";
            oCmd.Parameters.Add("TOURREQID", OracleDbType.Varchar2).Value = strTourReqID;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);

            //Below Added by aumento for BTC======================
            String FldName = "HOTELCOST";
            for (int i = 0; i < dt.Rows.Count - 2; i++)
            {
                if (dt.Rows[i + 1][FldName].ToString().Length == 0)
                {
                    dt.Rows[i + 1][FldName] = dt.Rows[i][FldName];
                    dt.Rows[i + 1].AcceptChanges();
                }
            }
            //====================================================
            return (dt);

        }
        public DataTable GetCityDtl(string strCityname, string strEmpcode, DateTime Tourdate)
        {
            OracleCommand oCmd = new OracleCommand();
            dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_CITYDETAIL";
            oCmd.Parameters.Add("TOUR_DATE", OracleDbType.Date).Value = Tourdate;
            oCmd.Parameters.Add("CityName", OracleDbType.Varchar2).Value = strCityname;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpcode;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);

        }
        public string CheckIsHoliday(string strDate, string strSiteID, string strempcode)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_ISHOLIDAYORNOT_GET";
            oCmd.Parameters.Add("DATE_IN", OracleDbType.Varchar2).Value = strDate;
            oCmd.Parameters.Add("SYSITEID_IN", OracleDbType.Varchar2).Value = strSiteID;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = strempcode;
            oCmd.Parameters.Add("ISWORKINGDAY", OracleDbType.Varchar2, 5).Direction = ParameterDirection.Output;
            oDataMgmt.ExecuteQuery(oCmd);
            return oCmd.Parameters["ISWORKINGDAY"].Value.ToString();

        }
        public DataTable GetPenSettlementList(string strEmpcode)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_PENSETTLEMENTLIST_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpcode;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }
        public DataTable GetTBSSettledList(string strEmpcode)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_TBSSETTLEDLIST_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpcode;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }
        public DataTable GetSettlementTranDtl(string strStlmntID)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_SETTLEMENTTRANDTL_GET";
            oCmd.Parameters.Add("V_TOURSETTID", OracleDbType.Int32).Value = strStlmntID;
            oCmd.Parameters.Add("CUR_TOURTRANSDTL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }
        public DataTable GetSettlementDayDtl(string strStlmntID)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_SETTLEMENTDAYDTL_GET";
            oCmd.Parameters.Add("V_TOURSETTID", OracleDbType.Int32).Value = strStlmntID;
            oCmd.Parameters.Add("CUR_TOURDAYDTL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }

        public DataTable GetHotelSettlementDayDtl(string strStlmntID)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.SPROC_H_SETTLEMENTDAYDTL_GET";
            oCmd.Parameters.Add("V_TOURSETTID", OracleDbType.Int32).Value = strStlmntID;
            oCmd.Parameters.Add("CUR_TOURDAYDTL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }

        public DataTable GetSettlementConvDtl(string strStlmntID)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_SETTLEMENTCONVDTL_GET";
            oCmd.Parameters.Add("V_TOURSETTID", OracleDbType.Int32).Value = strStlmntID;
            oCmd.Parameters.Add("CUR_TOURCONVEYANCE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }
        public DataTable GetSettlementTktDtl(string strStlmntID)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_SETTLEMENTTKTDTL_GET";
            oCmd.Parameters.Add("V_TOURSETTID", OracleDbType.Int32).Value = strStlmntID;
            oCmd.Parameters.Add("CUR_TOURTICKET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }
        public DataTable GetSettlementCityDtl(string strStlmntID)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_SETTLEMENTCITYDTL_GET";
            oCmd.Parameters.Add("V_TOURSETTID", OracleDbType.Int32).Value = strStlmntID;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }
        public DataTable GetApplist(string strEmpcode, string strLVL)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            //oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_APPAUTHORITYLIST_GET"; // Added by Aumento  ::  SR99261 : CR6440
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_APPAUTHORITYLIST_GET_TS"; // Added by Aumento  ::  SR99261 : CR6440
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Int32).Value = strEmpcode;
            oCmd.Parameters.Add("V_LVL", OracleDbType.Int32).Value = strLVL == "" ? (object)DBNull.Value : strLVL;
            oCmd.Parameters.Add("CUR_APPAUTHLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }
        public DataTable GetPendingApplist(string strEmpcode)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_PENDINGAPPLIST_GET";
            oCmd.Parameters.Add("V_APPEMPCODE", OracleDbType.Int32).Value = strEmpcode;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }
        public DataTable GetStlAppHistory(string strStlmntID)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_APPHISTORYLIST_GET";
            oCmd.Parameters.Add("V_SETTLEMENTID", OracleDbType.Int32).Value = strStlmntID;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }
        public DataTable GetPenStlATAdmin(string strEmpcode, string strEmpname, string strStlId, string strStatus, string strAddedby)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_PENSTLLISTATADMIN_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpcode == "" ? (object)DBNull.Value : strEmpcode;
            oCmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpname;
            oCmd.Parameters.Add("ADDEDBY_IN", OracleDbType.Int32).Value = strAddedby;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = strStatus;
            oCmd.Parameters.Add("SETTID_IN", OracleDbType.Int32).Value = strStlId == "" ? (object)DBNull.Value : strStlId;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }
        public DataTable GetBillReceiveList(string strEmpcode, string strEmpname, string strStlId, string strStatus)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_BILLRECEIVELIST_GET";
            oCmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = strStatus;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpcode == "" ? (object)DBNull.Value : strEmpcode;
            oCmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpname;
            oCmd.Parameters.Add("SETTID_IN", OracleDbType.Int32).Value = strStlId == "" ? (object)DBNull.Value : strStlId;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }
        public DataTable GetStlDtl_ATPAYBLE(string strAppcode, string strEmpcode, string strStatus, string strEmpname, string strSiteID, string strSecID, string strDeptID, string strDivID, string strOpID, string strStlId
        , string empType // Added By Kishan Dodiya
        )
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_STLDTLATPAYABLE_GET";
            oCmd.Parameters.Add("V_APPEMPCODE", OracleDbType.Int32).Value = strAppcode == "" ? (object)DBNull.Value : strAppcode;
            oCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode == "" ? (object)DBNull.Value : strEmpcode;
            oCmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = strStatus;
            oCmd.Parameters.Add("V_EMPNAME", OracleDbType.Varchar2).Value = strEmpname;
            oCmd.Parameters.Add("V_SITE", OracleDbType.Int32).Value = strSiteID == "" ? (object)DBNull.Value : strSiteID;
            oCmd.Parameters.Add("V_SECID", OracleDbType.Int32).Value = strSecID == "0" ? (object)DBNull.Value : strSecID;
            oCmd.Parameters.Add("V_DEPTID", OracleDbType.Int32).Value = strDeptID == "0" ? (object)DBNull.Value : strDeptID;
            oCmd.Parameters.Add("V_DIVID", OracleDbType.Int32).Value = strDivID == "0" ? (object)DBNull.Value : strDivID;
            oCmd.Parameters.Add("V_OPID", OracleDbType.Int32).Value = strOpID == "0" ? (object)DBNull.Value : strOpID;
            oCmd.Parameters.Add("V_STLID", OracleDbType.Int32).Value = strStlId == "" ? (object)DBNull.Value : strStlId;
            oCmd.Parameters.Add("EMPTYPE_IN", OracleDbType.Varchar2).Value = empType; // Added By Kishan Dodiya
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }

        //Change by aumento for SR60962============================================
        public DataTable NewGetStlDtl_ATPAYBLE(string strAppcode, string strEmpcode, string strStatus, string strEmpname, string strSiteID, string strSecID, string strDeptID, string strDivID, string strOpID, string strStlId, string FormDate, string ToDate
        , string empType // Added By Kishan Dodiya
        )
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.NEWSPROC_STLDTLATPAYABLE_GET";
            oCmd.Parameters.Add("V_APPEMPCODE", OracleDbType.Int32).Value = strAppcode == "" ? (object)DBNull.Value : strAppcode;
            oCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode == "" ? (object)DBNull.Value : strEmpcode;
            oCmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = strStatus;
            oCmd.Parameters.Add("V_EMPNAME", OracleDbType.Varchar2).Value = strEmpname;
            oCmd.Parameters.Add("V_SITE", OracleDbType.Int32).Value = strSiteID == "" ? (object)DBNull.Value : strSiteID;
            oCmd.Parameters.Add("V_SECID", OracleDbType.Int32).Value = strSecID == "0" ? (object)DBNull.Value : strSecID;
            oCmd.Parameters.Add("V_DEPTID", OracleDbType.Int32).Value = strDeptID == "0" ? (object)DBNull.Value : strDeptID;
            oCmd.Parameters.Add("V_DIVID", OracleDbType.Int32).Value = strDivID == "0" ? (object)DBNull.Value : strDivID;
            oCmd.Parameters.Add("V_OPID", OracleDbType.Int32).Value = strOpID == "0" ? (object)DBNull.Value : strOpID;
            oCmd.Parameters.Add("V_STLID", OracleDbType.Int32).Value = strStlId == "" ? (object)DBNull.Value : strStlId;

            oCmd.Parameters.Add("V_FormDate", OracleDbType.Varchar2).Value = FormDate; //Change by aumento for SR60962
            oCmd.Parameters.Add("V_ToDate", OracleDbType.Varchar2).Value = ToDate; //Change by aumento for SR60962
            oCmd.Parameters.Add("EMPTYPE_IN", OracleDbType.Varchar2).Value = empType; // // Added By Kishan Dodiya
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }
        // End  Change =========================================================
        public DataTable GetTktDtlBookedByAdmin(string strid, string strEmpcode, string strEmpname, string strfromdate, string strtodate)
        {
            OracleCommand oCmd = new OracleCommand();
            dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_BOOKEDTKTLIST_BYADM_GET";
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Int32).Value = strEmpcode == "" ? (object)DBNull.Value : strEmpcode;
            oCmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = strEmpname;
            oCmd.Parameters.Add("FROM_DATE", OracleDbType.Varchar2).Value = strfromdate;
            oCmd.Parameters.Add("TODATE", OracleDbType.Varchar2).Value = strtodate;
            oCmd.Parameters.Add("TOURDTLID_IN", OracleDbType.Int32).Value = strid == "" ? (object)DBNull.Value : strid;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);

        }
        public DataTable GetSettAppDtl(string strStlmntID)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_PRINTSETTAPP_GET";
            oCmd.Parameters.Add("V_TOURSETTID", OracleDbType.Int32).Value = strStlmntID;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }
        public string MailtoApprovalAuth(string strEcode, string strEname, string strSettlement)
        {

            DataTable dt = GetStlAppHistory(strSettlement);
            dt.DefaultView.RowFilter = "ADEMPCODE<>" + strEcode + " and APPSTATUS=0";
            if (dt.DefaultView.ToTable().Rows.Count == 0)
                return "3";

            //Mail Not to be send to Payble end
            if (dt.Rows[dt.Rows.Count - 1]["ADEMPCODE"].ToString() == "2" || dt.Rows[dt.Rows.Count - 1]["ADEMPCODE"].ToString() == "1")
                return "3";
            //if (dt.Rows[dt.Rows.Count - 1]["ADEMPCODE"].ToString() == objcmn.GetParameterValue("TBS_PAYABLEAPPAUTH"))
            //    return "3";
            //bool IsExist = objcmn.GetParameterValue("TBS_PAYABLEAPPAUTH").Contains(dt.Rows[dt.Rows.Count - 1]["ADEMPCODE"].ToString());
            //if (IsExist)
            //    return "3";

            string strappauth = dt.DefaultView.ToTable().Rows[dt.DefaultView.ToTable().Rows.Count - 1]["ADEMPCODE"].ToString();
            string strappauthname = dt.DefaultView.ToTable().Rows[dt.DefaultView.ToTable().Rows.Count - 1]["EMPNAME"].ToString();
            string strappauthmail = dt.DefaultView.ToTable().Rows[dt.DefaultView.ToTable().Rows.Count - 1]["EMAILID"].ToString();

            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
            if (serverpath.isTestServer())
                sendMail.MailTo = serverpath.getTestEMail();
            else
                sendMail.MailTo = strappauthmail;
            string strSubject = "Tour Bill Settlement Request from - " + strEname + ", Employee Code - " + strEcode;
            //string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
            //                 "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Tour Bill Settlement Request from " + strEname + " - Emp Code (" + strEcode + ")</font></b></td>" +
            //                 "</tr><tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td width=125 height=22 valign=top>Employee Code</td><td width=389 valign=top>" + strEcode + "</td>" +
            //                 "</tr><tr><td width=125 height=23 valign=top>Employee Name</td><td width=389 valign=top>" + strEname + "</td>" +
            //                 " </tr>" +
            //                 "<tr> " +
            //                 "<td width=125 valign=top>Plan Tour Period</td><td width=389 valign=top>" + dt.DefaultView.ToTable().Rows[dt.DefaultView.ToTable().Rows.Count - 1]["TOUR_PERIOD"].ToString() + "</td>" +
            //                 "</tr>" +
            //                 "<tr> " +
            //                 "<td width=125 valign=top>Actual Tour Period</td><td width=389 valign=top>" + dt.DefaultView.ToTable().Rows[dt.DefaultView.ToTable().Rows.Count - 1]["TOUR_PERIOD"].ToString() + "</td>" +
            //                 "</tr>" +
            //                 "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "index.aspx> Employee Portal</a> for approval process.</td></tr>" +
            //                 "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

            string strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                             "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images/HondaLogo5.gif border=0 /></td>" +
                             "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                             "</td></tr>" +
                             "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                             "</td></tr><tr height=150><td colspan=2>" +
                             "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                             "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                             "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                             "<tr><td colspan=2>&nbsp;</td></tr>" +
                             "<tr><td colspan=2><b>Dear " + strappauthname + " San,</b></td></tr>" +
                             "<tr><td valign=top colspan=2><b>" + strEname + " - Emp Code (" + strEcode + ") " + " has given a Tour Bill Settlement Request. The details are as follows:</b></td></tr>" +
                             "<tr><td colspan=2>&nbsp;</td></tr>" +
                             //"<tr><td align=Left valign=top width='20%'>Employee Code:</td><td valign=top >" + strEcode + "</td></tr>" +
                             //"<tr><td align=Left valign=top >Employee Name:</td><td valign=top >" + strEname + "</td></tr>" +
                             "<tr><td align=Left valign=top >Request Id:</td><td valign=top >" + strSettlement + "</td></tr>" +
                             "<tr><td align=Left valign=top >Plan Tour Period:</td><td valign=top >" + dt.DefaultView.ToTable().Rows[dt.DefaultView.ToTable().Rows.Count - 1]["TOUR_PERIOD"].ToString() + "</td></tr>" +
                             "<tr><td align=Left valign=top >Actual Tour Period:</td><td valign=top >" + dt.DefaultView.ToTable().Rows[dt.DefaultView.ToTable().Rows.Count - 1]["TOUR_PERIOD"].ToString() + "</td></tr>" +
                             //"<tr><td valign=top >Ext. No:</td><td valign=top >" + ExtNo + "</td></tr>" +
                             "<tr><td colspan=2>&nbsp;</td></tr>" +
                             "<tr><td colspan=2>Please login <a href=" + serverpath.getServerPath() + "index.aspx> Employee Portal</a> for your approval.</td></tr>" +
                             "<tr valign=bottom><td colspan=2><b>Best Regards</b><br /> Team - EPortal<br /><br /><strong>Note: It is a system generated Email, please do not reply.</strong></td> " +
                             "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                             "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images/Table_layout_06.gif border=0 /></td> " +
                             "</tr></table> ";

            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            try
            {
                bool status = sendMail.Send();
                return "1";//Failed to send Message
            }
            catch (Exception ex)
            {
                return "0";//Failed to send Message
            }
        }
        public string MailToAssociate(string strAppEcode, string strAppname, string strappemail, string strAppremark, string strStatus, string strSettlID)
        {
            string strBody = "";
            string strstatustxt = "";
            string strEcode = "";
            string strEname = "";
            string streemail = "";
            string strplntour_prd = "";
            string stracttour_prd = "";
            DataTable dt = this.GetSettlementTranDtl(strSettlID);
            if (dt.Rows[dt.Rows.Count - 1]["ADEMPCODE"].ToString() == "")
                return "3";
            if (dt.Rows.Count > 0)
            {
                strEcode = dt.Rows[0]["ADEMPCODE"].ToString();
                strEname = dt.Rows[0]["EMPNAME"].ToString();
                streemail = dt.Rows[0]["EMAILID"].ToString();
                strplntour_prd = DateTime.ParseExact(dt.Rows[0]["PLAN_FROMDATE"].ToString(), "dd.MM.yy", null).ToString("dd-MMM-yyyy") + " to " + DateTime.ParseExact(dt.Rows[0]["PLAN_TODATE"].ToString(), "dd.MM.yy", null).ToString("dd-MMM-yyyy");
                stracttour_prd = DateTime.ParseExact(dt.Rows[0]["ACTUAL_FROMDATE"].ToString(), "dd.MM.yy", null).ToString("dd-MMM-yyyy") + " to " + DateTime.ParseExact(dt.Rows[0]["ACTUAL_TODATE"].ToString(), "dd.MM.yy", null).ToString("dd-MMM-yyyy");
            }
            if (strStatus == "1")
            {
                strstatustxt = "approved";
                strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                              "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images/HondaLogo5.gif border=0 /></td>" +
                              "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                              "</td></tr>" +
                              "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                              "</td></tr><tr height=150><td colspan=2>" +
                              "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                              "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                              "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                               "<tr><td colspan=2><b>Dear " + strEname + " San,</b></td></tr>" +
                               "<tr><td colspan=2>&nbsp;</td></tr>" +
                              "<tr><td valign=top colspan=2><b>Your Tour Bill Settlement request has been <b>" + strstatustxt + "</b> by " + strAppname + " - Emp Code (" + strAppEcode + ") " + " San. The details are as follows:</td></tr>" +
                              "<tr><td colspan=2>&nbsp;</td></tr>" +
                   "<tr><td align=Left valign=top width='30%'>Request Id        :</td><td valign=top >" + strSettlID + "</td></tr>" +
                              "<tr><td align=Left valign=top >Plan Tour Period  :</td><td valign=top >" + strplntour_prd + "</td></tr>" +
                              "<tr><td align=Left valign=top >Actual Tour Period:</td><td valign=top >" + stracttour_prd + "</td></tr>" +
                              "<tr><td align=Left valign=top >Remarks           :</td><td valign=top >" + strAppremark.ToString() + "</td></tr>" +
                              "<tr><td colspan=2>&nbsp;</td></tr>" +
                              "<tr valign=bottom><td colspan=2><b>Best Regards</b><br /> Team - EPortal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                              "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                              "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images/Table_layout_06.gif border=0 /></td> " +
                              "</tr></table> ";
                //strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                //          "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Tour Bill Settlement Approval Status " + strstatustxt + " - Emp Code (" + strEcode + ")</font></b></td></tr>" +
                //          "<tr><td>" +
                //          "<table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                //          "<tr><td valign=top colspan =2>Your request has been <b>" + strstatustxt + "</b> by " + strAppname + " San. The Tour Bill Settlement details are as follows:</td></tr>" +
                //          "<tr><td width=125 height=22 valign=top>Employee Code</td><td width=389 valign=top>" + strEcode + "</td></tr>" +
                //          "<tr><td width=125 height=23 valign=top>Employee Name</td><td width=389 valign=top>" + strEname + "</td> </tr>" +
                //          "<tr> " +
                //          "<td width=125 valign=top>Plan Tour Period</td><td width=389 valign=top>" + strplntour_prd + "</td>" +
                //          "</tr>" +
                //          "<tr> " +
                //          "<td width=125 valign=top>Actual Tour Period</td><td width=389 valign=top>" + stracttour_prd + "</td>" +
                //          "</tr>" +
                //          "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "index.aspx> Employee Portal</a> to view the approval history.</td></tr>" +
                //          "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";
            }
            if (strStatus == "2")
            {
                strstatustxt = "return at your end";
                strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                          "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images/HondaLogo5.gif border=0 /></td>" +
                          "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                          "</td></tr>" +
                          "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                          "</td></tr><tr height=150><td colspan=2>" +
                          "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                          "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                          "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                          "<tr><td colspan=2><b>Dear " + strEname + " San,</b></td></tr>" +
                          "<tr><td colspan=2>&nbsp;</td></tr>" +
                          "<tr><td valign=top colspan=2><b>Your Tour Bill Settlement request has been <b>" + strstatustxt + "</b> by " + strAppname + " - Emp Code (" + strAppEcode + ") " + " San. </td></tr>" +
                          "<tr><td valign=top colspan=2>Please edit your tour bill settlement detail as per approval authority remark.The details are as follows:</td></tr>" +
                          "<tr><td colspan=2>&nbsp;</td></tr>" +
               "<tr><td align=Left valign=top width='30%'>Request Id        :</td><td valign=top >" + strSettlID + "</td></tr>" +
                          "<tr><td align=Left valign=top >Plan Tour Period  :</td><td valign=top >" + strplntour_prd + "</td></tr>" +
                          "<tr><td align=Left valign=top >Actual Tour Period:</td><td valign=top >" + stracttour_prd + "</td></tr>" +
                          "<tr><td align=Left valign=top >Remarks           :</td><td valign=top >" + strAppremark.ToString() + "</td></tr>" +
                          "<tr><td colspan=2>&nbsp;</td></tr>" +
                          "<tr valign=bottom><td colspan=2><b>Best Regards</b><br /> Team - EPortal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                          "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                          "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images/Table_layout_06.gif border=0 /></td> " +
                          "</tr></table> ";

                //strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                //         "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Tour Bill Settlement Approval Status " + strstatustxt + " - Emp Code (" + strEcode + ")</font></b></td></tr>" +
                //         "<tr><td>" +
                //         "<table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                //         "<tr><td valign=top colspan =2>Your request has been <b>" + strstatustxt + "</b> by " + strAppname + " San.</td></tr>" +
                //         "<tr><td valign=top colspan =2>Please edit your tour bill settlement detail as per approval authority remark.</td></tr>" +
                //         "<tr><td valign=top colspan =2>The Tour Bill Settlement details are as follows:</td></tr>" +

                //         "<tr><td width=125 height=22 valign=top>Employee Code</td><td width=389 valign=top>" + strEcode + "</td></tr>" +
                //         "<tr><td width=125 height=23 valign=top>Employee Name</td><td width=389 valign=top>" + strEname + "</td> </tr>" +
                //         "<tr> " +
                //         "<td width=125 valign=top>Plan Tour Period</td><td width=389 valign=top>" + strplntour_prd + "</td>" +
                //         "</tr>" +
                //         "<tr> " +
                //         "<td width=125 valign=top>Actual Tour Period</td><td width=389 valign=top>" + stracttour_prd + "</td>" +
                //         "</tr>" +

                //         "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "index.aspx> Employee Portal</a> to view the approval history.</td></tr>" +
                //         "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";
            }
            bool IsExist = objcmn.GetParameterValue("TBS_ADMINAPPAUTH").Contains(strAppEcode);
            //if (strStatus == "3" && objcmn.GetParameterValue("TBS_ADMINAPPAUTH") == strAppEcode)
            if (strStatus == "3" && IsExist)
            {
                strstatustxt = "hold";
                strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                          "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images/HondaLogo5.gif border=0 /></td>" +
                          "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                          "</td></tr>" +
                          "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                          "</td></tr><tr height=150><td colspan=2>" +
                          "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                          "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                          "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                          "<tr><td colspan=2><b>Dear " + strEname + " San,</b></td></tr>" +
                          "<tr><td colspan=2>&nbsp;</td></tr>" +
                          "<tr><td valign=top colspan=2><b>Your Tour Bill Settlement request has been <b>" + strstatustxt + "</b> by " + strAppname + " - Emp Code (" + strAppEcode + ") " + " San. </td></tr>" +
                          "<tr><td valign=top colspan=2>Please edit your tour bill settlement ticket detail as per approval authority remark.The details are as follows:</td></tr>" +
                          "<tr><td colspan=2>&nbsp;</td></tr>" +
               "<tr><td align=Left valign=top width='30%'>Request Id        :</td><td valign=top >" + strSettlID + "</td></tr>" +
                          "<tr><td align=Left valign=top >Plan Tour Period  :</td><td valign=top >" + strplntour_prd + "</td></tr>" +
                          "<tr><td align=Left valign=top >Actual Tour Period:</td><td valign=top >" + stracttour_prd + "</td></tr>" +
                          "<tr><td colspan=2>&nbsp;</td></tr>" +
                          "<tr valign=bottom><td colspan=2><b>Best Regards</b><br /> Team - EPortal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                          "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                          "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images/Table_layout_06.gif border=0 /></td> " +
                          "</tr></table> ";

                //strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                //         "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Tour Bill Settlement Approval Status " + strstatustxt + " - Emp Code (" + strEcode + ")</font></b></td></tr>" +
                //         "<tr><td>" +
                //         "<table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                //         "<tr><td valign=top colspan =2>Your request has been <b>" + strstatustxt + "</b> by Admin </td></tr>" +
                //         "<tr><td valign=top colspan =2>Please edit your tour bill settlement ticket detail as per approval authority remark.</td></tr>" +
                //         "<tr><td width=125 height=22 valign=top>Employee Code</td><td width=389 valign=top>" + strEcode + "</td></tr>" +
                //         "<tr><td width=125 height=23 valign=top>Employee Name</td><td width=389 valign=top>" + strEname + "</td> </tr>" +
                //         "<tr> " +
                //         "<td width=125 valign=top>Plan Tour Period</td><td width=389 valign=top>" + strplntour_prd + "</td>" +
                //         "</tr>" +
                //         "<tr> " +
                //         "<td width=125 valign=top>Actual Tour Period</td><td width=389 valign=top>" + stracttour_prd + "</td>" +
                //         "</tr>" +
                //         "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "index.aspx> Employee Portal</a> to view the approval history.</td></tr>" +
                //         "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";
            }
            IsExist = objcmn.GetParameterValue("TBS_PAYABLEAPPAUTH").Contains(strAppEcode);
            //if (strStatus == "3" && objcmn.GetParameterValue("TBS_PAYABLEAPPAUTH") == strAppEcode)
            if (strStatus == "3" && IsExist)
            {
                strstatustxt = "Hold";
                strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                          "<tr><td  height=35><img src=" + serverpath.getServerPath() + "Images/HondaLogo5.gif border=0 /></td>" +
                          "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                          "</td></tr>" +
                          "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "Images/Table_layout_04.gif height=30>&nbsp;" +
                          "</td></tr><tr height=150><td colspan=2>" +
                          "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                          "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                          "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                          "<tr><td colspan=2><b>Dear " + strEname + " San,</b></td></tr>" +
                          "<tr><td colspan=2>&nbsp;</td></tr>" +
                          "<tr><td valign=top colspan=2><b>Your Tour Bill Settlement request has been <b>" + strstatustxt + "</b> by " + strAppname + " - Emp Code (" + strAppEcode + ") " + " San. </td></tr>" +
                          "<tr><td valign=top colspan=2>Your tour bill settlement details are as follows:</td></tr>" +
                          "<tr><td colspan=2>&nbsp;</td></tr>" +
               "<tr><td align=Left valign=top width='30%'>Request Id        :</td><td valign=top >" + strSettlID + "</td></tr>" +
                          "<tr><td align=Left valign=top >Plan Tour Period  :</td><td valign=top >" + strplntour_prd + "</td></tr>" +
                          "<tr><td align=Left valign=top >Actual Tour Period:</td><td valign=top >" + stracttour_prd + "</td></tr>" +
                          "<tr><td align=Left valign=top >Hold Reason       :</td><td valign=top >" + strAppremark.ToString() + "</td></tr>" +
                          "<tr><td colspan=2>&nbsp;</td></tr>" +
                          "<tr valign=bottom><td colspan=2><b>Best Regards</b><br /> Team - EPortal<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                          "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                          "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "Images/Table_layout_06.gif border=0 /></td> " +
                          "</tr></table> ";
                //strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                //         "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Tour Bill Settlement Approval Status " + strstatustxt + " - Emp Code (" + strEcode + ")</font></b></td></tr>" +
                //         "<tr><td>" +
                //         "<table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                //         "<tr><td valign=top colspan =2>Your request has been <b>" + strstatustxt + "</b> by Finance </td></tr>" +
                //         "<tr><td valign=top colspan =2>Please edit your tour bill settlement detail as per approval authority remark.</td></tr>" +
                //         "<tr><td valign=top colspan =2>The Tour Bill Settlement details are as follows:</td></tr>" +

                //         "<tr><td width=125 height=22 valign=top>Employee Code</td><td width=389 valign=top>" + strEcode + "</td></tr>" +
                //         "<tr><td width=125 height=23 valign=top>Employee Name</td><td width=389 valign=top>" + strEname + "</td> </tr>" +
                //         "<tr> " +
                //         "<td width=125 valign=top>Plan Tour Period</td><td width=389 valign=top>" + strplntour_prd + "</td>" +
                //         "</tr>" +
                //         "<tr> " +
                //         "<td width=125 valign=top>Actual Tour Period</td><td width=389 valign=top>" + stracttour_prd + "</td>" +
                //         "</tr>" +

                //         "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "index.aspx> Employee Portal</a> to view the approval history.</td></tr>" +
                //         "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";
            }

            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";

            if (serverpath.isTestServer())
                sendMail.MailTo = serverpath.getTestEMail();
            else
                sendMail.MailTo = streemail;

            string strSubject = "Tour Bill Settlement Request from - " + strEname + ", Employee Code - " + strEcode;

            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            try
            {
                bool status = sendMail.Send();
                MailtoApprovalAuth(strEcode, strEname, strSettlID);
                return "1";//Failed to send Message
            }
            catch (Exception ex)
            {
                return "0";//Failed to send Message
            }

        }
        public DataTable GetManageAppHistory(string strEcode)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_MANAGEAPPHIS_GET";
            oCmd.Parameters.Add("V_APPCODE", OracleDbType.Int32).Value = strEcode;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }
        public DataTable GetTourDtl_BySettlementId(string strStlmntID)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_TOURDTL_AGAINSTSTLID";
            oCmd.Parameters.Add("V_SETTLEMENTID", OracleDbType.Int32).Value = strStlmntID;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }
        public List<string> GetTBSPilotUser()
        {
            //CommonFunctions objCommon = new CommonFunctions();
            String _TBSUsers = objcmn.GetParameterValue("TBS_PILOT_USERS");
            String[] _users = _TBSUsers.Split(',');
            List<string> u = new List<string>();
            u.AddRange(_users);
            return u;
        }
        public DataTable GetCityList(string strCityid, string strCityName, string strStatus, string strCategory)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_CITYLIST_GET";
            oCmd.Parameters.Add("V_CITYNAME", OracleDbType.Varchar2).Value = strCityName;
            oCmd.Parameters.Add("V_CITYCATEGORY", OracleDbType.Varchar2).Value = strCategory;
            oCmd.Parameters.Add("V_STATUS", OracleDbType.Varchar2).Value = strStatus;
            oCmd.Parameters.Add("V_ID", OracleDbType.Varchar2).Value = strCityid;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }
        public DataTable GetOrglvlList(string strOPID, string strDIVID, string strDEPID, string strSECID, string strStatus)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_ORGLISTLIST_GET";
            oCmd.Parameters.Add("V_OPID", OracleDbType.Varchar2).Value = strOPID;
            oCmd.Parameters.Add("V_DIVID", OracleDbType.Varchar2).Value = strDIVID;
            oCmd.Parameters.Add("V_DEPID", OracleDbType.Varchar2).Value = strDEPID;
            oCmd.Parameters.Add("V_SECID", OracleDbType.Varchar2).Value = strSECID;
            oCmd.Parameters.Add("V_STATUS", OracleDbType.Varchar2).Value = strStatus;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }
        public DataTable GetAssociatesExpenseDetails(string UserID, string RequestID, string EmpCode, string EmpName,
                                                      string FromDate, string TillDate)
        {

            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURBUDGET.SPROC_EXPENSEREPORT_GET";
            oCmd.Parameters.Add("CUR_EXPENSEREPORT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = RequestID;
            oCmd.Parameters.Add("EMPCODE_IN", OracleDbType.Varchar2).Value = EmpCode;
            oCmd.Parameters.Add("EMPNAME_IN", OracleDbType.Varchar2).Value = EmpName;
            oCmd.Parameters.Add("FROMDATE_IN", OracleDbType.Varchar2).Value = FromDate;
            oCmd.Parameters.Add("TILLDATE_IN", OracleDbType.Varchar2).Value = TillDate;
            //LOGGED EMPLOYEE ID
            oCmd.Parameters.Add("ADEMPCODE_IN", OracleDbType.Varchar2).Value = UserID;
            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public DataTable GetHotelamountDtl(string strStlmntID, string strfromdate, string strtodate, string strtodatec)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.SPROC_HOTELAMOUNTDTL_GET";
            oCmd.Parameters.Add("V_TOURSETTID", OracleDbType.Int32).Value = strStlmntID;
            oCmd.Parameters.Add("V_FROMDATE", OracleDbType.Varchar2).Value = strfromdate;
            oCmd.Parameters.Add("V_TODATE", OracleDbType.Varchar2).Value = strtodate;
            oCmd.Parameters.Add("V_TODATEC", OracleDbType.Varchar2).Value = strtodatec;
            oCmd.Parameters.Add("CUR_TOURDAYDTL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }
        #endregion

        #region "Add Hotel Settlement Detail"

        public string UpdateHotelDetailByFIN(Hotel_Detail obj_dt, string addedby)
        {
            OracleConnection objCn;
            OracleCommand objCmd;
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.CommandText = "PKG_TOURSETTLEMENTNEW.SPROC_UPDATE_FIN_HotSetlDetCA";
                    objCmd.Parameters.Add("V_HSetlDetBokbyCAID", OracleDbType.Int64).Value = obj_dt.HSETLDETBOKBYCAID;
                    objCmd.Parameters.Add("V_ADTOURREQUESTID", OracleDbType.Int64).Value = obj_dt.ADTOURREQUESTID;
                    objCmd.Parameters.Add("V_H_SETTLEMENTID", OracleDbType.Int64).Value = obj_dt.H_SETTLEMENTID;
                    objCmd.Parameters.Add("V_BookedBy", OracleDbType.Int16).Value = obj_dt.BOOKEDBY;
                    objCmd.Parameters.Add("V_STATEID", OracleDbType.Int32).Value = obj_dt.StateID;
                    objCmd.Parameters.Add("V_HOTELID", OracleDbType.Int32).Value = obj_dt.HotelID;
                    objCmd.Parameters.Add("V_HOTEL_NAME", OracleDbType.Varchar2).Value = obj_dt.HOTEL_NAME;
                    objCmd.Parameters.Add("V_GST_NO", OracleDbType.Varchar2).Value = obj_dt.GST_NO;
                    objCmd.Parameters.Add("V_HOTEL_ADDRESS", OracleDbType.Varchar2).Value = "";
                    objCmd.Parameters.Add("V_CHECKINDATE", OracleDbType.Date).Value = obj_dt.Checkindate;
                    objCmd.Parameters.Add("V_CHECKOUTDATE", OracleDbType.Date).Value = obj_dt.Checkoutdate;
                    objCmd.Parameters.Add("V_HOTELSTAY", OracleDbType.Int16).Value = obj_dt.HOTELSTAY == true ? 1 : 0;
                    objCmd.Parameters.Add("V_INVOICENUMBER", OracleDbType.Varchar2).Value = obj_dt.INVOICENUMBER;
                    objCmd.Parameters.Add("V_INVOICEDATE", OracleDbType.Date).Value = obj_dt.INVOICEDATE;
                    objCmd.Parameters.Add("V_STATUS", OracleDbType.Int16).Value = 1;
                    objCmd.Parameters.Add("V_CREATED_BY", OracleDbType.Int32).Value = addedby;
                    objCmd.Connection = objCn;
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    objCmd.ExecuteNonQuery();
                    string strErr = objCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + objCmd.Parameters["ERRMSG_OUT"].Value.ToString();
                    return strErr;
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


        //:: Start Added By Aumneto for SR99421 ::
        public string UpdateHotelGST(Hotel_Detail obj_dt, int addedBy)
        {
            OracleConnection objCn;
            OracleCommand objCmd;
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            using (objCn = new OracleConnection(strConn))
            {
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand("PKG_TOURSETTLEMENTNEW.UPDATE_HOTEL_GST", objCn);
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.Parameters.Add("V_GSTNO", OracleDbType.Varchar2).Value = obj_dt.GST_NO;
                    objCmd.Parameters.Add("V_HOTELID", OracleDbType.Int32).Value = obj_dt.HotelID;
                    objCmd.Parameters.Add("V_MODIFIED_DATE", OracleDbType.Date).Value = DateTime.Now;
                    objCmd.Parameters.Add("V_MODIFIED_BY", OracleDbType.Int32).Value = addedBy;
                    OracleParameter statusParam = new OracleParameter("STATUS_OUT", OracleDbType.Int32);
                    statusParam.Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add(statusParam);
                    objCmd.ExecuteNonQuery();
                    return statusParam.Value.ToString(); // Returns "1" if success, "0" if not
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
        public bool ValidateGST(string gstNo, int stateId)
        {
            OracleConnection objCn = null;
            OracleCommand objCmd = null;
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();

            try
            {
                objCn = new OracleConnection(strConn);
                objCn.Open();
                objCmd = new OracleCommand("PKG_TOURSETTLEMENTNEW.CHECK_GST_STATE_MATCH", objCn);
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("GST_NO_IN", OracleDbType.Varchar2).Value = gstNo;
                objCmd.Parameters.Add("STATE_ID_IN", OracleDbType.Int32).Value = stateId;
                objCmd.Parameters.Add("IS_MATCH_OUT", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                objCmd.ExecuteNonQuery();
                string result = objCmd.Parameters["IS_MATCH_OUT"].Value.ToString();
                return result.Equals("TRUE", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                objCmd?.Dispose();
                if (objCn != null)
                {
                    objCn.Close();
                    objCn.Dispose();
                }
            }
        }
        public DataTable GetHotelID(string RequestID)
        {
            DataTable dt = new DataTable();
            OracleConnection objCn = null;
            OracleCommand objCmd = null;
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();
            try
            {
                objCn = new OracleConnection(strConn);
                objCn.Open();
                objCmd = new OracleCommand("PKG_TOURSETTLEMENTNEW.GET_HOTELID_BY_DETAILS", objCn);
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("V_REQUESTID", OracleDbType.Int32).Value = Convert.ToInt32(RequestID);
                objCmd.Parameters.Add("RESULT_OUT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                using (OracleDataReader reader = objCmd.ExecuteReader())
                {
                    dt.Load(reader);
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCmd?.Dispose();
                if (objCn != null)
                {
                    objCn.Close();
                    objCn.Dispose();
                }
            }
        }

        //:: End Added By Aumneto for SR99421 ::
        #endregion
        #region "Delete Hotel Settlement Detail"
        public string DeleteHSETLDETBkbyAssociate(int HSETLDETBOKBYCAID, int ADTOURREQUESTID, string addedby)
        {
            OracleConnection objCn;
            OracleCommand objCmd;
            //ConnectionString objCnStr = new ConnectionString();
            string strConn = objCnStr.getConnectingString();

            using (objCn = new OracleConnection())
            {
                objCn.ConnectionString = strConn;
                try
                {
                    objCn.Open();
                    objCmd = new OracleCommand();
                    objCmd.CommandText = "PKG_TOURSETTLEMENTNEW.SPROC_HSETLDETBKBYASSO_DELETE";
                    objCmd.Parameters.Add("HSETLDETBOKBYCAID_IN", OracleDbType.Int32).Value = HSETLDETBOKBYCAID;
                    objCmd.Parameters.Add("ADTOURREQUESTID_IN", OracleDbType.Int32).Value = ADTOURREQUESTID;
                    objCmd.Parameters.Add("CREATED_BY_IN", OracleDbType.Int32).Value = addedby;
                    // objCmd.CommandText = strSql;
                    objCmd.Connection = objCn;
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                    objCmd.Parameters.Add("ERRMSG_OUT", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    oDataMgmt.ExecuteQuery(objCmd);
                    string strErr = objCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + objCmd.Parameters["ERRMSG_OUT"].Value.ToString();
                    return strErr;
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

        public DataSet GetGSTRateList()
        {
            OracleCommand oCmd = new OracleCommand();
            ds = new DataSet();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.SPROC_GST_Rate_MST_GET";
            oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            ds = oDataMgmt.GetDataSet(oCmd);
            return (ds);
        }

        #region "Get Hotel Details By Associate"
        public DataTable GetgrdHDetailsByAssociate(string strH_SETTLEMENTID, string strADTOURREQUESTID, string strSTATUS_IN)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.SPROC_TRN_HOTEL_SETTLEMENT_GET";
            oCmd.Parameters.Add("H_SETTLEMENTID_IN ", OracleDbType.Int32).Value = strH_SETTLEMENTID == "" ? null : strH_SETTLEMENTID;
            oCmd.Parameters.Add("ADTOURREQUESTID_IN ", OracleDbType.Varchar2).Value = strADTOURREQUESTID;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = strSTATUS_IN == "" ? null : strSTATUS_IN;
            oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }
        public DataTable GetgrdHSetDetailsByAssociate(long strH_SETTLEMENTID, long strADTOURREQUESTID, int strBOOKEDBY_IN)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.SPROC_HSETDETSByASSOCIATE_GET";
            oCmd.Parameters.Add("H_SETTLEMENTID_IN ", OracleDbType.Int32).Value = strH_SETTLEMENTID; // == "" ? null : strH_SETTLEMENTID;
            oCmd.Parameters.Add("ADTOURREQUESTID_IN ", OracleDbType.Int32).Value = strADTOURREQUESTID;
            oCmd.Parameters.Add("BOOKEDBY_IN", OracleDbType.Int32).Value = strBOOKEDBY_IN;
            oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }
        #endregion

        /// <summary>
        /// GET Hotel Settlement DETAILS AGAINST DETAIL ID
        /// </summary>
        ///<param name="RequestDetailID"></param>
        /// <returns></returns>
        public DataTable GetSingleHSettlementDetails(string HSettlementID)
        {
            string strSTATUS_IN = string.Empty;
            string strADTOURREQUESTID = string.Empty;
            OracleCommand oCmd = new OracleCommand();
            DataTable dt = new DataTable();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.SPROC_TRN_HOTEL_SETTLEMENT_GET";
            oCmd.Parameters.Add("H_SETTLEMENTID_IN ", OracleDbType.Int32).Value = HSettlementID == "" ? null : HSettlementID;
            oCmd.Parameters.Add("ADTOURREQUESTID_IN ", OracleDbType.Varchar2).Value = strADTOURREQUESTID;
            oCmd.Parameters.Add("STATUS_IN", OracleDbType.Int32).Value = strSTATUS_IN == "" ? null : strSTATUS_IN;
            oCmd.Parameters.Add("CUR_GETLIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            //oCmd.Parameters.Add("REQUESTID_IN", OracleDbType.Varchar2).Value = HSettlementID;
            //oCmd.Parameters.Add("CUR_REQBOOKING", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            //GET DATA FROM DATA ACCESS LAYER
            dt = oDataMgmt.GetDataTable(oCmd);
            return (dt);
        }

        public string UpdateTBS_ByFinance(string strSettlementid, string strEmpcode, string Addedby, Tour_Day_Detail[] obj, string ApprovalType)
        {
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            string strErr = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                OracleCommand oCmd = new OracleCommand();
                oCmd.Connection = objCn;
                oCmd.Parameters.Clear();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.BindByName = true;
                foreach (Tour_Day_Detail o in obj)
                {
                    //update Tour Bill Settlement Day Detail
                    oCmd.Parameters.Clear();
                    oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    oCmd.BindByName = true;
                    oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.SPROC_UPDTBSDAYDETAILFIN";
                    oCmd.Parameters.Add("V_TBSDAYDTLID", OracleDbType.Int32).Direction = ParameterDirection.InputOutput;
                    oCmd.Parameters["V_TBSDAYDTLID"].Value = o.ID;
                    oCmd.Parameters.Add("V_SETTLEMENTID", OracleDbType.Int32).Value = strSettlementid;
                    oCmd.Parameters.Add("V_ADTOURREQUESTID", OracleDbType.Int32).Value = o.TourID;
                    oCmd.Parameters.Add("V_SETTLEMENTDATE", OracleDbType.Date).Value = o.Tourdate;
                    oCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int32).Value = Addedby;
                    oCmd.Parameters.Add("V_FIN_HOTEL_AMOUNT", OracleDbType.Varchar2).Value = o.Fin_Hotel_Amt;
                    oCmd.Parameters.Add("V_FIN_OTHERAMT", OracleDbType.Varchar2).Value = o.FIN_OtherAmt;
                    oCmd.Parameters.Add("V_FIN_CGSTRATE", OracleDbType.Varchar2).Value = o.Fin_CGSTRateID;
                    oCmd.Parameters.Add("V_FIN_SGSTRATE", OracleDbType.Varchar2).Value = o.Fin_SGSTRateID;
                    oCmd.Parameters.Add("V_FIN_HOTELAMTTOT", OracleDbType.Varchar2).Value = o.Fin_HotelAmtTot;
                    //oCmd.Parameters.Add("V_MISCELLANEOUSAMT", OracleDbType.Varchar2).Value = o.Miscellaneous_Amt;

                    //-- SR51566-- Changes Start(Aumento)-----
                    oCmd.Parameters.Add("V_MISCELLANEOUSAMT", OracleDbType.Varchar2).Value = o.MissAllow_Amt;
                    //-- SR51566 -- Changes End(Aumento)-----

                    oCmd.Parameters.Add("V_APPROVALTYPE", OracleDbType.Int16).Value = ApprovalType;

                    //-- SR51566-- Changes Start(Aumento)-----
                    oCmd.Parameters.Add("V_CGSTRATE", OracleDbType.Varchar2).Value = o.CGSTRateID;
                    oCmd.Parameters.Add("V_SGSTRATE", OracleDbType.Varchar2).Value = o.SGSTRateID;
                    //-- SR51566 -- Changes End(Aumento)-----

                    oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                    oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    oCmd.ExecuteNonQuery();
                    strErr = oCmd.Parameters["RESULT_OUT"].Value.ToString() + "#" + oCmd.Parameters["ERROR_MSG"].Value.ToString();

                }
                return strErr;
            }
        }
        public DataSet GetSettlementhHotlDtl(string strStlmntID)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.SPROC_STLMNTHOTELDTLCOMP_GET";
            oCmd.Parameters.Add("V_TOURSETTID", OracleDbType.Int32).Value = strStlmntID;
            oCmd.Parameters.Add("CUR_HSETLDETBOKBYC", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            oCmd.Parameters.Add("CUR_HSETLDETBOKBYA", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataSet(oCmd);
        }


        //// Added By Vishal on 08-July-2021
        public void UploadSettlementAttachment(int ID, string ADTOURDTLID, string FILTERID, string DOCtype, string PATH, byte[] TICKET_FILE_BYTE, string TICKET_FILE_NAME, string ADDEDBY)
        {
            String strErrMsg = String.Empty;
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            OracleTransaction tran;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                tran = objCn.BeginTransaction();
                try
                {
                    OracleCommand oCmd = new OracleCommand();
                    oCmd.Connection = objCn;
                    oCmd.Parameters.Clear();
                    oCmd.Transaction = tran;
                    oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    oCmd.BindByName = true;
                    oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.SPROC_INSERT_SETTLEMENTFILE";
                    oCmd.Parameters.Add("V_TOURSETTATTACHMENTID", OracleDbType.Int32).Value = ID;
                    oCmd.Parameters.Add("V_ADTOURREQDTLID", OracleDbType.Int64).Value = ADTOURDTLID;
                    oCmd.Parameters.Add("V_DOCTYPE", OracleDbType.Varchar2).Value = DOCtype;
                    oCmd.Parameters.Add("V_FILTERID", OracleDbType.Varchar2).Value = FILTERID;
                    oCmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = 1;
                    oCmd.Parameters.Add("V_ADDEDBY", OracleDbType.Int64).Value = ADDEDBY;
                    oCmd.Parameters.Add("V_TICKETATTACHMENT", OracleDbType.Varchar2).Value = TICKET_FILE_NAME;
                    oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                    oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    oCmd.ExecuteNonQuery();
                    int result = Convert.ToInt32(oCmd.Parameters["RESULT_OUT"].Value.ToString());
                    if (result != 1)
                    {
                        tran.Rollback();
                        throw new Exception("Failed to update. Transaction will rollback now.");
                    }
                    else
                    {
                        if (TICKET_FILE_BYTE != null && !string.IsNullOrEmpty(TICKET_FILE_NAME))
                        {
                            System.IO.File.WriteAllBytes(PATH + TICKET_FILE_NAME, TICKET_FILE_BYTE);
                        }
                        else
                        {
                            tran.Rollback();
                            throw new Exception("File not upload. Transaction will rollback now.");
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

        public int DeleteAttachmentBySettlID(Int64 SettlID, string TicketID, string HotelID, string FileName, string DocType, string UpdatedBy)
        {
            int result = 0;
            String strErrMsg = String.Empty;
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            OracleTransaction tran;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                tran = objCn.BeginTransaction();
                try
                {
                    OracleCommand oCmd = new OracleCommand();
                    oCmd.Connection = objCn;
                    oCmd.Parameters.Clear();
                    oCmd.Transaction = tran;
                    oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    oCmd.BindByName = true;
                    oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.SPROC_DELETEFILE_BYSETTLID";
                    oCmd.Parameters.Add("V_TOURSETTLEMENTID", OracleDbType.Int64).Value = SettlID;
                    oCmd.Parameters.Add("V_FILENAME", OracleDbType.Varchar2).Value = FileName;
                    oCmd.Parameters.Add("V_TYPE", OracleDbType.Varchar2).Value = DocType;
                    oCmd.Parameters.Add("V_TICKETID", OracleDbType.Varchar2).Value = TicketID;
                    oCmd.Parameters.Add("V_HOTELID", OracleDbType.Varchar2).Value = HotelID;
                    oCmd.Parameters.Add("V_UPDATEDBY", OracleDbType.Varchar2).Value = UpdatedBy;
                    oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                    oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    oCmd.ExecuteNonQuery();
                    result = Convert.ToInt32(oCmd.Parameters["RESULT_OUT"].Value.ToString());
                    if (result != 1)
                    {
                        tran.Rollback();
                        throw new Exception("Failed to update. Transaction will rollback now.");
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
                return result;
            }
        }
        public int UploadAttachmentBySettlID(Int64 SettlID, string TicketID, string HotelID, string FileName, string DocType, string UpdatedBy)
        {
            int result = 0;
            String strErrMsg = String.Empty;
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            OracleTransaction tran;
            using (OracleConnection objCn = new OracleConnection())
            {
                objCn.ConnectionString = strCn;
                objCn.Open();
                tran = objCn.BeginTransaction();
                try
                {
                    OracleCommand oCmd = new OracleCommand();
                    oCmd.Connection = objCn;
                    oCmd.Parameters.Clear();
                    oCmd.Transaction = tran;
                    oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    oCmd.BindByName = true;
                    oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.SPROC_UPLOADFILE_BYSETTLID";
                    oCmd.Parameters.Add("V_TOURSETTLEMENTID", OracleDbType.Int64).Value = SettlID;
                    oCmd.Parameters.Add("V_FILENAME", OracleDbType.Varchar2).Value = FileName;
                    oCmd.Parameters.Add("V_DOCTYPE", OracleDbType.Varchar2).Value = DocType;
                    oCmd.Parameters.Add("V_TICKETID", OracleDbType.Varchar2).Value = TicketID;
                    oCmd.Parameters.Add("V_HOTELID", OracleDbType.Varchar2).Value = HotelID;
                    oCmd.Parameters.Add("V_UPDATEDBY", OracleDbType.Varchar2).Value = UpdatedBy;
                    oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                    oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                    oCmd.ExecuteNonQuery();
                    result = Convert.ToInt32(oCmd.Parameters["RESULT_OUT"].Value.ToString());
                    if (result != 1)
                    {
                        tran.Rollback();
                        throw new Exception("Failed to update. Transaction will rollback now.");
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
                return result;
            }
        }
        public DataTable GetFinaceAttachmentBySettlId(string strStlmntID)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENTNEW.SPROC_TBSATTACHMENT_BYSETTLID";
            oCmd.Parameters.Add("V_SETTLEMENTID", OracleDbType.Int32).Value = strStlmntID;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }

        //public int DeleteTempAttachmentByTourID(string strTourReqID, string path)
        //{
        //    int result = 0;
        //    try
        //    {
        //        OracleCommand oCmd = new OracleCommand();
        //        dt = new DataTable();
        //        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_USERTKTDTL_GET";
        //        oCmd.Parameters.Add("TOURREQID", OracleDbType.Varchar2).Value = strTourReqID;
        //        oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //        //GET DATA FROM DATA ACCESS LAYER
        //        dt = oDataMgmt.GetDataTable(oCmd);
        //        if (dt.Rows.Count > 0)
        //        {

        //            //if (System.IO.File.Exists(System.IO.Path.Combine(path_Hotel, hfHotelBill1.Value)))
        //            //{
        //            //    System.IO.File.Delete(System.IO.Path.Combine(path_Hotel, hfHotelBill1.Value));
        //            //}
        //        }
        //        result = 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        result = -1;
        //    }
        //    return (result);

        //}
        //// End

        //--SR49770 ----------------------------------Changes Start Create new a method Start---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public DataTable GetApprovalDtl_ATPAYBLE(string strAppcode, string strEmpcode, string strStatus, string strEmpname, string strSiteID, string strSecID, string strDeptID, string strDivID, string strOpID, string strStlId)
        {
            OracleCommand oCmd = new OracleCommand();
            oCmd.CommandType = System.Data.CommandType.StoredProcedure;
            oCmd.BindByName = true;
            oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_ApprovalDTLATPAYABLE_GET";
            oCmd.Parameters.Add("V_APPEMPCODE", OracleDbType.Int32).Value = strAppcode == "" ? (object)DBNull.Value : strAppcode;
            oCmd.Parameters.Add("V_ADEMPCODE", OracleDbType.Int32).Value = strEmpcode == "" ? (object)DBNull.Value : strEmpcode;
            oCmd.Parameters.Add("V_STATUS", OracleDbType.Int32).Value = strStatus;
            oCmd.Parameters.Add("V_EMPNAME", OracleDbType.Varchar2).Value = strEmpname;
            oCmd.Parameters.Add("V_SITE", OracleDbType.Int32).Value = strSiteID == "" ? (object)DBNull.Value : strSiteID;
            oCmd.Parameters.Add("V_SECID", OracleDbType.Int32).Value = strSecID == "0" ? (object)DBNull.Value : strSecID;
            oCmd.Parameters.Add("V_DEPTID", OracleDbType.Int32).Value = strDeptID == "0" ? (object)DBNull.Value : strDeptID;
            oCmd.Parameters.Add("V_DIVID", OracleDbType.Int32).Value = strDivID == "0" ? (object)DBNull.Value : strDivID;
            oCmd.Parameters.Add("V_OPID", OracleDbType.Int32).Value = strOpID == "0" ? (object)DBNull.Value : strOpID;
            oCmd.Parameters.Add("V_STLID", OracleDbType.Int32).Value = strStlId == "" ? (object)DBNull.Value : strStlId;
            oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
            return oDataMgmt.GetDataTable(oCmd);
        }
        //---SR49770 --- Added Type Parameter ---Start 
        public string InsertDATA(string strSettlementid, string SAPDOCNO, DateTime SAPPOSTINDDATE, int type)
        //---SR49770 --- Added Type Parameter ---End
        {
            //DataManagement dm = new DataManagement();
            //ConnectionString objCnStr = new ConnectionString();
            string strCn = objCnStr.getConnectingString();
            string strErr = string.Empty;
            using (OracleConnection objCn = new OracleConnection())
            {

                objCn.ConnectionString = strCn;
                objCn.Open();
                OracleCommand oCmd = new OracleCommand();
                oCmd.Connection = objCn;
                oCmd.Parameters.Clear();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.BindByName = true;
                //update Tour Bill Settlement Day Detail
                oCmd.Parameters.Clear();
                oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                oCmd.BindByName = true;
                oCmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_INSERTSAPDATA_GET";
                oCmd.Parameters.Add("V_STLID", OracleDbType.Int32).Value = strSettlementid;
                oCmd.Parameters.Add("V_SAPDOCNO", OracleDbType.Varchar2).Value = SAPDOCNO;
                oCmd.Parameters.Add("V_SAPPOSTINGDATE", OracleDbType.Date).Value = SAPPOSTINDDATE;
                //---SR49770 --- Added Type Parameter ---Start 
                oCmd.Parameters.Add("V_TYPE", OracleDbType.Int32).Value = type;
                //---SR49770 --- Added Type Parameter ---End
                oCmd.Parameters.Add("RESULT_OUT", OracleDbType.Int32, 1).Direction = ParameterDirection.Output;
                oCmd.Parameters.Add("ERROR_MSG", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
                oCmd.ExecuteNonQuery();
                strErr = oCmd.Parameters["RESULT_OUT"].Value.ToString();
                //return dm.GetDataTable(oCmd);
                return strErr;
            }
        }
        //--SR49770 ---------------------------------- Changes End Create new a method End---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    }

    #region "Class"
    [Serializable]
    public struct Contineous_Tour
    {
        string _strTourid;
        DateTime _date_from;
        DateTime _date_to;
        public DateTime DATEFROM
        {
            get { return _date_from; }
        }
        public DateTime DATETO
        {
            get { return _date_to; }
        }
        public string Tourid
        {
            get { return _strTourid; }
        }
        public Contineous_Tour(string strtourid, DateTime dt_from, DateTime dt_to)
        {
            _strTourid = strtourid;
            _date_from = dt_from;
            _date_to = dt_to;
        }
    }

    [Serializable]
    public class Hotel_Detail
    {
        long _HSetlDetBokbyCAID = 0;
        long _ADTOURREQDETAILID = 0;
        long _ADTOURREQUESTID = 0;
        // long _ADTOURREQUESTID = 0;
        long _H_SETTLEMENTID = 0;
        string _HOTEL_NAME = "";
        string _HOTEL_ADDRESS = "";
        string _State = "";
        int _BOOKEDBY = 0;
        string _City = "";
        string _GST_NO = "";
        DateTime _Checkindate;
        DateTime _Checkoutdate;
        bool _HOTELSTAY = false;
        string _HOTELSTAY1 = "";
        string _INVOICENUMBER = "";
        DateTime _INVOICEDATE;
        double _Total;
        string _HotelFileName;
        string _HotelCost = "";
        public long StateID { get; set; }
        public long StayingCityID { get; set; }
        public long HotelID { get; set; }
        public int CityID { get; set; }

        public bool STATUS { get; set; }

        //public string HotelCost { get; set; }//added by aumento for BTC

        public long ADTOURREQDETAILID
        {
            get { return _ADTOURREQDETAILID; }
            set { _ADTOURREQDETAILID = value; }
        }

        public long H_SETTLEMENTID
        {
            get { return _H_SETTLEMENTID; }
            set { _H_SETTLEMENTID = value; }
        }
        public long ADTOURREQUESTID
        {
            get { return _ADTOURREQUESTID; }
            set { _ADTOURREQUESTID = value; }
        }

        public long HSETLDETBOKBYCAID
        {
            get { return _HSetlDetBokbyCAID; }
            set { _HSetlDetBokbyCAID = value; }
        }
        public long HSETTLEMENTID
        {
            get { return _H_SETTLEMENTID; }
            set { _H_SETTLEMENTID = value; }
        }


        public string HOTEL_NAME
        {
            get { return _HOTEL_NAME; }
            set { _HOTEL_NAME = value; }
        }
        public string HOTEL_ADDRESS
        {
            get { return _HOTEL_ADDRESS; }
            set { _HOTEL_ADDRESS = value; }
        }

        public string State
        {
            get { return _State; }
            set { _State = value; }
        }

        public int BOOKEDBY
        {
            get { return _BOOKEDBY; }
            set { _BOOKEDBY = value; }
        }


        public string City
        {
            get { return _City; }
            set { _City = value; }
        }


        public string GST_NO
        {
            get { return _GST_NO; }
            set { _GST_NO = value; }
        }
        public DateTime Checkindate
        {
            get { return _Checkindate; }
            set { _Checkindate = value; }
        }
        public DateTime Checkoutdate
        {
            get { return _Checkoutdate; }
            set { _Checkoutdate = value; }
        }
        public DateTime INVOICEDATE
        {
            get { return _INVOICEDATE; }
            set { _INVOICEDATE = value; }
        }
        public bool HOTELSTAY
        {
            get { return _HOTELSTAY; }
            set { _HOTELSTAY = value; }
        }

        public string HOTELSTAY1
        {
            get { return _HOTELSTAY1; }
            set { _HOTELSTAY1 = value; }
        }
        public string INVOICENUMBER
        {
            get { return _INVOICENUMBER; }
            set { _INVOICENUMBER = value; }
        }
        public double Total
        {
            get { return _Total; }
            set { _Total = value; }
        }

        public string HOTEL_FILE_NAME
        {
            get { return _HotelFileName; }
            set { _HotelFileName = value; }
        }

        public string HotelCost
        {
            get { return _HotelCost; }
            set { _HotelCost = value; }
        }
        public byte[] HOTEL_FILE_BYTE { get; set; }
        public string HOTEL_FILE_PATH { get; set; }
        public string strHOTEL_FILE_BYTE { get; set; }
        public int IsuploadFile { get; set; }
    }

    [Serializable]
    public class Place_Detail
    {
        Int64 _ID;
        DateTime _date;
        double _Amt;
        string _strCityName;
        double _NTA;
        double _HA;
        double _LA;
        string _HOTELCOST;//BTC

        public Place_Detail()
        { }
        public Place_Detail(double Amt, string strCityName, DateTime Tourdate, double NTA, double HA, double LA, Int64 ID, string HOTELCOST)
        {
            _Amt = Amt;
            _strCityName = strCityName;
            _date = Tourdate;
            _NTA = NTA;
            _HA = HA;
            _LA = LA;
            _ID = ID;
            _HOTELCOST = HOTELCOST;//BTC
        }
        public DateTime TourDate
        {
            get { return _date; }
            set { _date = value; }
        }
        public double Amt
        {
            get { return _Amt; }
            set { _Amt = value; }
        }
        public string City
        {
            get { return _strCityName; }
            set { _strCityName = value; }
        }

        public string HotelCost
        {
            get { return _HOTELCOST; }
            set { _HOTELCOST = value; }//BTC
        }
        public double NTA
        {
            get { return _NTA; }
            set { _NTA = value; }
        }
        public double HA
        {
            get { return _HA; }
            set { _HA = value; }
        }
        public double LA
        {
            get { return _LA; }
            set { _LA = value; }
        }
        public Int64 ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
    }
    [Serializable]
    public class Conveyance_Detail
    {
        Int64 _ID;
        DateTime _date;
        double _Amt;
        string _strCityName;
        string _strFromloc;
        string _strToloc;
        string _strModofTran;
        double _Fin_Amt;
        string _Conv_FileName;
        byte[] _Conv_FileByte;
        string _Conv_FilePath;
        int _IsConvFileUpload;

        //--- SR51566 --Changes Start(Aumento)--
        string _CONV_FILE;
        //--- SR51566 --Changes End(Aumento)--

        public Conveyance_Detail()
        { }
        public Conveyance_Detail(double Amt, string strCityName, DateTime tourDate, string strFromloc, string strToloc, string strModofTran, Int64 ID, double Fin_Amt, string Conv_FileName, byte[] Conv_FileByte, string Conv_FilePath, int IsConvFileUpload)
        {
            _date = tourDate;
            _Amt = Amt;
            _strCityName = strCityName;
            _strFromloc = strFromloc;
            _strToloc = strToloc;
            _strModofTran = strModofTran;
            _ID = ID;
            _Fin_Amt = Fin_Amt;
            _Conv_FileName = Conv_FileName;
            _Conv_FileByte = Conv_FileByte;
            _Conv_FilePath = Conv_FilePath;
            _IsConvFileUpload = IsConvFileUpload;
        }
        public double Conv_Amt
        {
            get { return _Amt; }
            set { _Amt = value; }
        }
        public string City
        {
            get { return _strCityName; }
            set { _strCityName = value; }
        }
        public string From_Loc
        {
            get { return _strFromloc; }
            set { _strFromloc = value; }
        }
        public string To_Loc
        {
            get { return _strToloc; }
            set { _strToloc = value; }
        }
        public string Mode_Tran
        {
            get { return _strModofTran; }
            set { _strModofTran = value; }
        }
        public DateTime Tour_Date
        {
            get { return _date; }
            set { _date = value; }
        }
        public Int64 ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        public double Fin_Conv_Amt
        {
            get { return _Fin_Amt; }
            set { _Fin_Amt = value; }
        }

        public string CONV_FILE_NAME
        {
            get { return _Conv_FileName; }
            set { _Conv_FileName = value; }
        }
        public byte[] CONV_FILE_BYTE
        {
            get { return _Conv_FileByte; }
            set { _Conv_FileByte = value; }
        }
        public string CONV_FILE_PATH
        {
            get { return _Conv_FilePath; }
            set { _Conv_FilePath = value; }
        }

        public int ISCONV_FILEUPLOAD
        {
            get { return _IsConvFileUpload; }
            set { _IsConvFileUpload = value; }
        }

        //--- SR51566 --Changes Start(Aumento)--
        public string CONV_FILE
        {
            get { return _CONV_FILE; }
            set { _CONV_FILE = value; }
        }
        //--- SR51566 --Changes End(Aumento)--
    }
    [Serializable]
    public class Tour_Day_Detail
    {
        double BRK_PER = 0;
        double LUNCH_PER = 0;
        double DINNER_PER = 0;
        double MISSALLOW_PER = 0;
        DateTime _date;
        Place_Detail[] _objplaces;
        //Hotel_Detail[] H_Details;
        double _Ticket_Fare;
        Conveyance_Detail[] _objconv;
        double _HtlAmt;

        double _HotelAmtTot;
        double _OtherAmt;
        double _FIN_OtherAmt;
        double _CGSTPercent;
        double _FIN_CGSTPercent;
        double _SGSTPercent;
        double _FIN_SGSTPercent;

        double _DA;
        Boolean _BRK_Flag;
        Boolean _Lunch_Flag;
        Boolean _Dinner_Flag;
        Boolean _MissAllow_Flag;
        double _BRK_Amt;
        double _Lunch_Amt;
        double _Dinner_Amt;
        double _MissAllow_Amt;
        Boolean _NTA_Flag;
        Boolean _HA_Flag;
        double _NTA_Amt;
        double _LA_Amt;
        double _LA_Amt_Actual;
        double _HA_Amt;
        double _Miss_Amt;
        string _Exp_detail;
        string _Remark;
        string _CityName;
        string _HotelCost;//BTC
        double _total_ConvAmt;
        double _Fin_total_ConvAmt;
        Int64 _ID;
        Int64 _Tourid;
        //capturing the finance edited value 
        double _Fin_Ticket_Fare;
        double _Fin_HtlAmt;
        double _Fin_HotelAmtTot;
        Boolean _Fin_BRK_Flag;
        Boolean _Fin_Lunch_Flag;
        Boolean _Fin_Dinner_Flag;
        Boolean _Fin_MissAllow_Flag;
        Boolean _Fin_NTA_Flag;
        Boolean _Fin_HA_Flag;
        double _Fin_LA_Amt_Actual;
        double _Fin_Miss_Amt;
        double _Fin_MissAllow_Amt;
        string _Fin_Remark;
        DateTime _TourStartDate;
        public int CGSTRateID { get; set; }
        public int SGSTRateID { get; set; }

        public int Fin_CGSTRateID { get; set; }
        public int Fin_SGSTRateID { get; set; }
        public double Miscellaneous_Amt { get; set; }

        double _fin_act_food_amt = -1; //SR68656 change
        double _fin_act_mis_amt = -1; //SR68656 change

        public double lbl_Food_Amt { get; set; } //SR68656 change
        public double lbl_Mis_Amt { get; set; } //SR68656 change
        public double Actual_Food_Amt { get; set; } //SR68656 change
        public double Actual_Mis_Amt { get; set; } //SR68656 change
        public double fin_act_food_amt
        {
            get { return _fin_act_food_amt; }
            set { _fin_act_food_amt = value; }
        } //SR68656 change
        public double fin_act_mis_amt
        {
            get { return _fin_act_mis_amt; }
            set { _fin_act_mis_amt = value; }
        } //SR68656 change

        public string Ticket_FileName { get; set; }
        public byte[] Ticket_FileByte { get; set; }
        public string Ticket_FilePath { get; set; }

        private readonly IDataManagement odmgt; 

        public Tour_Day_Detail(DateTime date, Place_Detail[] objplaces, Conveyance_Detail[] objconv, double Tkt_Fare, double HtlAmt, double DA, Boolean BRK_Flag, Boolean Lunch_Flag, Boolean Dinner_Flag, Boolean NTA_Flag, Boolean HA_Flag, double NTA_Amt, double LA_Amt, double LA_Amt_Actual, double HA_Amt, double Miss_Amt, string Exp_detail, string Remark, Int64 ID, Int64 Tourid, double Fin_Ticket_Fare, double Fin_HtlAmt, Boolean Fin_BRK_Flag, Boolean Fin_Lunch_Flag, Boolean Fin_Dinner_Flag, Boolean Fin_NTA_Flag, Boolean Fin_HA_Flag, double Fin_LA_Amt_Actual, double Fin_Miss_Amt, string Fin_Remark, Boolean MissAllow_Flag, Boolean Fin_MissAllow_Flag, DateTime TourStartDate, string HotelCost, IDataManagement _odmgt)
        {
            _date = date;
            _objplaces = objplaces;
            _Ticket_Fare = Tkt_Fare;
            _objconv = objconv;
            _HtlAmt = HtlAmt;
            _DA = DA;
            _BRK_Flag = BRK_Flag;
            _Lunch_Flag = Lunch_Flag;
            _Dinner_Flag = Dinner_Flag;
            _MissAllow_Flag = MissAllow_Flag;
            _NTA_Flag = NTA_Flag;
            _HA_Flag = HA_Flag;
            _NTA_Amt = NTA_Amt;
            _LA_Amt = LA_Amt;
            _LA_Amt_Actual = LA_Amt_Actual;
            _HA_Amt = HA_Amt;
            _Miss_Amt = Miss_Amt;
            _Exp_detail = Exp_detail;
            _ID = ID;
            _Tourid = Tourid;
            _Remark = Remark;
            _Fin_Ticket_Fare = Fin_Ticket_Fare;
            _Fin_HtlAmt = Fin_HtlAmt;
            _Fin_BRK_Flag = Fin_BRK_Flag;
            _Fin_Lunch_Flag = Fin_Lunch_Flag;
            _Fin_Dinner_Flag = Fin_Dinner_Flag;
            _Fin_MissAllow_Flag = Fin_MissAllow_Flag;
            _Fin_NTA_Flag = Fin_NTA_Flag;
            _Fin_HA_Flag = Fin_HA_Flag;
            _Fin_LA_Amt_Actual = Fin_LA_Amt_Actual;
            _Fin_Miss_Amt = Fin_Miss_Amt;
            _Fin_Remark = Fin_Remark;
            _TourStartDate = TourStartDate;
            _HotelCost = HotelCost;//BTC
            odmgt = _odmgt; 
        }
        public Tour_Day_Detail()
        { }
        public Int64 TourID
        {
            get { return _Tourid; }
            set { _Tourid = value; }
        }
        public string HotelCost
        {
            get { return _HotelCost; }
            set { _HotelCost = value; }
        }
        public Int64 ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        public DateTime TourStart
        {
            get { return _TourStartDate; }
            set { _TourStartDate = value; }
        }
        public DateTime Tourdate
        {
            get { return _date; }
            set
            {

                _date = value;
                //DataManagement odmgt = new DataManagement();
                OracleCommand Ocmd = new OracleCommand();
                Ocmd.CommandText = "PKG_TOURSETTLEMENT.SPROC_GETDATYPE";
                if (_date.Date >= DateTime.ParseExact("01-MAR-2017", "dd-MMM-yyyy", null).Date && _TourStartDate.Date < DateTime.ParseExact("01-MAR-2017", "dd-MMM-yyyy", null).Date)
                {
                    Ocmd.Parameters.Add("TOUR_DATE", OracleDbType.Date).Value = _TourStartDate;
                }
                else
                {
                    Ocmd.Parameters.Add("TOUR_DATE", OracleDbType.Date).Value = _date;
                }

                Ocmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                Ocmd.CommandType = CommandType.StoredProcedure;
                DataTable dt = odmgt.GetDataTable(Ocmd);
                if (dt.Rows.Count > 0)
                {
                    dt.DefaultView.RowFilter = "TYPEDESC='BREAKFAST'";
                    if (dt.DefaultView.ToTable().Rows.Count > 0)
                        BRK_PER = Convert.ToDouble(dt.DefaultView.ToTable().Rows[0]["TYPEPER"]);
                    dt.DefaultView.RowFilter = "TYPEDESC='LUNCH'";
                    if (dt.DefaultView.ToTable().Rows.Count > 0)
                        LUNCH_PER = Convert.ToDouble(dt.DefaultView.ToTable().Rows[0]["TYPEPER"]);
                    dt.DefaultView.RowFilter = "TYPEDESC='DINNER'";
                    if (dt.DefaultView.ToTable().Rows.Count > 0)
                        DINNER_PER = Convert.ToDouble(dt.DefaultView.ToTable().Rows[0]["TYPEPER"]);
                    dt.DefaultView.RowFilter = "TYPEDESC='MISCELLANEOUS'";
                    if (dt.DefaultView.ToTable().Rows.Count > 0)
                        MISSALLOW_PER = Convert.ToDouble(dt.DefaultView.ToTable().Rows[0]["TYPEPER"]);
                }
            }
        }
        public Place_Detail[] Places
        {
            get { return _objplaces; }
            set { _objplaces = value; CalDA_City(); }
        }

        //public Place_Detail[] HotelCost
        //{
        //    get { return _HotelCost; }
        //    set { _objplaces = value; CalDA_City(); }
        //}


        //public Hotel_Detail[] Hotel_Details
        //{
        //    get { return H_Details; }
        //    set { H_Details = value; }
        //}

        public double Ticket_Fair
        {
            get { return _Ticket_Fare; }
            set { _Ticket_Fare = value; }
        }
        public double Hotel_Amt
        {
            get { return _HotelAmtTot; }
            set { _HotelAmtTot = value; }
        }
        public double HotelAmtTot
        {

            get
            {

                double total = 0.00;
                total += _HotelAmtTot;
                total += Hotel_Amt * (_CGSTPercent / 100);
                total += Hotel_Amt * (_SGSTPercent / 100);
                total += _OtherAmt;
                return (total);
            }

            set { _HtlAmt = value; }
        }

        public double OtherAmt
        {
            get { return _OtherAmt; }
            set { _OtherAmt = value; }
        }
        public double FIN_OtherAmt
        {
            get { return _FIN_OtherAmt; }
            set { _FIN_OtherAmt = value; }
        }

        public double CGSTPercent
        {
            get
            {
                //double total = 0.00; //get { return _CGSTPercent; }
                //total +=  (_CGSTPercent /100);
                //return total;
                return _CGSTPercent;
            }
            set { _CGSTPercent = value; }
        }
        public double SGSTPercent
        {
            //get { return _SGSTPercent; }
            get
            {
                //double total = 0.00;//get { return _CGSTPercent; }
                //total += (_SGSTPercent / 100);
                //return total;
                return _SGSTPercent;
            }
            set { _SGSTPercent = value; }
        }

        public double FIN_SGSTPercent
        {
            //get { return _SGSTPercent; }
            get
            {
                //double total = 0.00;//get { return _CGSTPercent; }
                //total += (_FIN_SGSTPercent / 100);
                //return total;
                return _FIN_CGSTPercent;
            }
            set { _FIN_SGSTPercent = value; }
        }
        public double FIN_CGSTPercent
        {
            //get { return _SGSTPercent; }
            get
            {
                //double total = 0.00;//get { return _CGSTPercent; }
                //total += (_FIN_CGSTPercent / 100);
                //return total;
                return _FIN_CGSTPercent;
            }
            set { _FIN_CGSTPercent = value; }
        }
        public Conveyance_Detail[] Conveyance
        {
            get { return _objconv; }
            set
            {
                _total_ConvAmt = 0.0;
                _Fin_total_ConvAmt = 0.0;
                _objconv = value; Conveyance_Detail[] objconv = _objconv;
                foreach (Conveyance_Detail obj in objconv)
                {
                    _total_ConvAmt += obj.Conv_Amt;
                    _Fin_total_ConvAmt += obj.Fin_Conv_Amt;
                }
            }
        }
        public double Conv_Amt
        {
            get { return _total_ConvAmt; }
        }
        public double DA
        {
            get { return _DA; }
        }

        public Boolean BRK_Flag
        {
            get { return _BRK_Flag; }
            set { _BRK_Flag = value; }
        }
        public Boolean Lunch_Flag
        {
            get { return _Lunch_Flag; }
            set { _Lunch_Flag = value; }
        }
        public Boolean Dinner_Flag
        {
            get { return _Dinner_Flag; }
            set { _Dinner_Flag = value; }
        }
        public Boolean MissAllow_Flag
        {
            get { return _MissAllow_Flag; }
            set { _MissAllow_Flag = value; }
        }
        public double BRK_Amt
        {
            get { return _BRK_Amt; }
            set { _BRK_Amt = value; } //SR68656 change
        }
        public double Lunch_Amt
        {
            get { return _Lunch_Amt; }
        }
        public double Dinner_Amt
        {
            get { return _Dinner_Amt; }
        }
        public double MissAllow_Amt
        {
            get { return _MissAllow_Amt; }
            set { _MissAllow_Amt = value; } //SR68656 change
        }

        public Boolean NTA_Flag
        {
            get { return _NTA_Flag; }
            set { _NTA_Flag = value; }
        }
        public Boolean HA_Flag
        {
            get { return _HA_Flag; }
            set { _HA_Flag = value; }
        }
        public double NTA_Amt
        {
            get { return _NTA_Amt; }
        }
        public double LA_Amt
        {
            get { return _LA_Amt; }
        }
        public double LA_Amt_Actual
        {
            get { return _LA_Amt_Actual; }
            set { _LA_Amt_Actual = value; }
        }
        public double HA_Amt
        {
            get { return _HA_Amt; }
        }
        public string Allow_total
        {
            get
            {
                double total = 0.00;
                if (_NTA_Flag == true)
                    total += _NTA_Amt;
                if (_HA_Flag == true)
                    total += _HA_Amt;
                return total.ToString();
            }
        }
        public double Miss_Amt
        {
            get { return _Miss_Amt; }
            set { _Miss_Amt = value; }
        }
        public string Exp_detail
        {
            get { return _Exp_detail; }
            set { _Exp_detail = value; }
        }
        public string Remark
        {
            get { return _Remark; }
            set { _Remark = value; }
        }
        public string CityName
        {
            get { return _CityName; }
        }

        //public string HotelCost
        //{
        //    get { return _HotelCost; }
        //}

        public string DayTotal
        {
            get
            {
                double total = 0.00;
                total += _Ticket_Fare;
                total += _total_ConvAmt;
                total += _HtlAmt;
                //if (_BRK_Flag == true) //Tour DA change
                //total += _BRK_Amt; //SR68656 change
                total += Actual_Food_Amt >= 0 ? Actual_Food_Amt : _BRK_Amt; //SR68656 change
                if (_Lunch_Flag == true)
                    total += _Lunch_Amt;
                if (Dinner_Flag == true)
                    total += _Dinner_Amt;
                //if (MissAllow_Flag == true) //Tour DA change
                //total += _MissAllow_Amt; //SR68656 change
                total += Actual_Mis_Amt >= 0 ? Actual_Mis_Amt : _MissAllow_Amt; //SR68656 change
                if (_NTA_Flag == true)
                    total += _NTA_Amt;
                if (_HA_Flag == true)
                    total += _HA_Amt;
                total += _LA_Amt_Actual;
                total += _Miss_Amt;
                total += Miscellaneous_Amt;

                return total.ToString();
            }
        }
        public void CalDA_City()
        {
            double DA = 0.0;
            double HA = 0.0;
            double LA = 0.0;
            double NTA = 0.0;
            string cityname = "";
            string HotelCost = "";

            foreach (Place_Detail obj in _objplaces)
            {
                cityname = cityname + obj.City + ",";
                HotelCost = obj.HotelCost;
                if (obj.Amt > DA)
                    DA = obj.Amt;
                if (obj.HA > HA)
                    HA = obj.HA;
                if (obj.LA > LA)
                    LA = obj.LA;
                if (obj.NTA > NTA)
                    NTA = obj.NTA;
            }
            _DA = DA;
            _HA_Amt = HA;
            _LA_Amt = LA;
            _NTA_Amt = NTA;
            _CityName = cityname;
            _HotelCost = HotelCost;
            _BRK_Amt = BRK_PER * _DA * 0.01;
            _Lunch_Amt = LUNCH_PER * _DA * 0.01;
            _Dinner_Amt = DINNER_PER * _DA * 0.01;
            _MissAllow_Amt = MISSALLOW_PER * _DA * 0.01;


        }
        public double BRK_PERC
        {
            get { return BRK_PER; }
            set { BRK_PER = value; }
        }
        public double LUNCH_PERC
        {
            get { return LUNCH_PER; }
            set { LUNCH_PER = value; }
        }
        public double DINNER_PERC
        {
            get { return DINNER_PER; }
            set { DINNER_PER = value; }
        }
        public double MISSALLOW_PERC
        {
            get { return MISSALLOW_PER; }
            set { MISSALLOW_PER = value; }
        }
        public double Fin_Ticket_Fair
        {
            get { return _Fin_Ticket_Fare; }
            set { _Fin_Ticket_Fare = value; }
        }
        public double Fin_Hotel_Amt
        {
            get { return _Fin_HtlAmt; }
            set { _Fin_HtlAmt = value; }
        }

        public double Fin_HotelAmtTot
        {
            get { return _Fin_HotelAmtTot; }
            set { _Fin_HotelAmtTot = value; }
        }
        public Boolean Fin_BRK_Flag
        {
            get { return _Fin_BRK_Flag; }
            set { _Fin_BRK_Flag = value; }
        }
        public Boolean Fin_Lunch_Flag
        {
            get { return _Fin_Lunch_Flag; }
            set { _Fin_Lunch_Flag = value; }
        }
        public Boolean Fin_Dinner_Flag
        {
            get { return _Fin_Dinner_Flag; }
            set { _Fin_Dinner_Flag = value; }
        }
        public Boolean Fin_MissAllow_Flag
        {
            get { return _Fin_MissAllow_Flag; }
            set { _Fin_MissAllow_Flag = value; }
        }
        public Boolean Fin_NTA_Flag
        {
            get { return _Fin_NTA_Flag; }
            set { _Fin_NTA_Flag = value; }
        }
        public Boolean Fin_HA_Flag
        {
            get { return _Fin_HA_Flag; }
            set { _Fin_HA_Flag = value; }
        }
        public double Fin_LA_Amt_Actual
        {
            get { return _Fin_LA_Amt_Actual; }
            set { _Fin_LA_Amt_Actual = value; }
        }
        public double Fin_Miss_Amt
        {
            get { return _Fin_Miss_Amt; }
            set { _Fin_Miss_Amt = value; }
        }
        public string Fin_Remark
        {
            get { return _Fin_Remark; }
            set { _Fin_Remark = value; }
        }
        public string Fin_DayTotal
        {
            get
            {
                double total = 0.00;
                total += _Fin_Ticket_Fare;
                total += _Fin_total_ConvAmt;

                //--SR51566 --Changes Start(Aumento)--
                //-----------------------------------------------------------------------------------------
                _Fin_HotelAmtTot = _FIN_OtherAmt + (2 * (Fin_Hotel_Amt * FIN_CGSTPercent / 100)) + Fin_Hotel_Amt;
                total += _Fin_HotelAmtTot;
                //-----------------------------------------------------------------------------------------
                //--SR51566 --Changes End(Aumento)--

                //if (_Fin_BRK_Flag == true) //Tour DA change
                //total += _BRK_Amt; //SR68656 change
                total += Actual_Food_Amt >= 0 ? Actual_Food_Amt : _BRK_Amt; //SR68656 change
                if (_Fin_Lunch_Flag == true)
                    total += _Lunch_Amt;
                if (_Fin_Dinner_Flag == true)
                    total += _Dinner_Amt;
                //if (_Fin_MissAllow_Flag == true) //Tour DA change
                //total += _MissAllow_Amt; //SR68656 change
                total += Actual_Mis_Amt >= 0 ? Actual_Mis_Amt : _MissAllow_Amt; //SR68656 change
                if (_Fin_NTA_Flag == true)
                    total += _NTA_Amt;
                if (_Fin_HA_Flag == true)
                    total += _HA_Amt;
                total += _Fin_LA_Amt_Actual;
                total += _Fin_Miss_Amt;
                total += Miscellaneous_Amt;

                return total.ToString();
            }
        }
        public double Fin_Conv_Amt
        {
            get { return _Fin_total_ConvAmt; }
        }
        public string Fin_Allow_total
        {
            get
            {
                double total = 0.00;
                if (_Fin_NTA_Flag == true)
                    total += _NTA_Amt;
                if (_Fin_HA_Flag == true)
                    total += _HA_Amt;
                return total.ToString();
            }
        }
    }
    
    
    [Serializable]
    public class Ticket_Detail
    {
        Int64 _ID;
        Int64 _TicketID;
        int _Ticket_Status;
        int _Ticket_SubStatus;
        int _Ticket_flag;
        string _TicketFileName;

        //--- SR51566 --Changes Start(Aumento)--
        string _TICKETATTACHMENT;
        //--- SR51566 --Changes End(Aumento)--

        //string _Adm_Inv_DocName;

        public Ticket_Detail()
        { }
        public Int64 ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        public Int64 TICKETID
        {
            get { return _TicketID; }
            set { _TicketID = value; }
        }
        public int TICKETSTATUS
        {
            get { return _Ticket_Status; }
            set { _Ticket_Status = value; }
        }
        public int TICKETSUBSTATUS
        {
            get { return _Ticket_SubStatus; }
            set { _Ticket_SubStatus = value; }
        }
        public int TICKETFLAG
        {
            get { return _Ticket_flag; }
            set { _Ticket_flag = value; }
        }
        public string TICKET_FILE_NAME
        {
            get { return _TicketFileName; }
            set { _TicketFileName = value; }
        }

        public byte[] TICKET_FILE_BYTE { get; set; }
        public string TICKET_FILE_PATH { get; set; }

        //change done by aumento as on 28092023 for the SR50547========================
        public string UPLOAD_TICKET_FILE_NAME { get; set; }
        //=============================================================================

        //--- SR51566 --Changes Start(Aumento)--
        public string TICKETATTACHMENT
        {
            get { return _TICKETATTACHMENT; }
            set { _TICKETATTACHMENT = value; }
        }
        //--- SR51566 --Changes End(Aumento)--
    }
    #endregion
}
