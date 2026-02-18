
#region Project Information Section

//Copyright © 2013. HMSI. All rights reserved.
//Requires Microsoft Visual Studio 2008 Professional (or greater).
//*****************************************************************************************************
// 
// Project Name 	:   Employee Portal.
// File Name        :   DPR_Reports.cs
// Description      :   DPR reports dynamic store procedure
// Purpose          :   To generate the DPR reports for 1F, 2F, and 3F
// Author			:   Sunil Lakhlan
// Author Email		:   sunil.kumar4@honda.hmsi.in
// Date Created		:   18/Aug/2013
// History			:   N/A
// Modified By		:   TTL in ePortal Upgradation
// Modified Date	:   28/11/2025
//
//*****************************************************************************************************

#endregion

#region Import NameSpace
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ePortal.Persistence.Interface;
using Oracle.ManagedDataAccess.Client;
using ePortal.Persistence.Services;
using ePortal.Persistence.VQMS.Interface;

#endregion

public class DPR : IDPR
{
    #region Instance Variables
    //DataManagement oDataMgmt = new DataManagement();

    string funGetShift = string.Empty;
    string tblPrd005 = string.Empty;

    string tblModelWiseMapping = string.Empty;
    string tblPrd_VQMS_Header = string.Empty;
    string tblFrameWiseDefect = string.Empty;
    string tblDefectMaster = string.Empty;
    string tblCodeType = string.Empty;
    string strSiteID = string.Empty;
    string strtime = string.Empty;
    //string strModel = string.Empty;


    DataSet objDS4 = new DataSet();
    //ConnectionStringDPR objCnStr = new ConnectionStringDPR();
    string strsql;
    string strConn;
    OracleConnection objCn;
    OracleCommand objCmd;

    private readonly IConnectionString objCnStr;
    private readonly IDataManagement oDataMgmt;
    private readonly ICommonFunctions objCommon;
    #endregion

    #region Constructor
    public DPR(IConnectionString conn, IDataManagement _oDataMgmt, ICommonFunctions _objCommon)
    {
        oDataMgmt = _oDataMgmt;
        objCnStr = conn;
        objCommon = _objCommon;
    }

    /// <summary>
    /// To initialize class data according to factory
    /// </summary>
    /// <param name="Factory"></param>
    //public DPR(string Factory)
    //{
    //    strConn = objCnStr.getConnectingStringVQMS(Factory);
    //    switch (Factory)
    //    {
    //        case "3":
    //            funGetShift = "GETSHIFT";
    //            tblPrd005 = "PRD005";
    //            tblModelWiseMapping = "modelwisemapping";
    //            tblPrd_VQMS_Header = "PRD_VQMS_HEADER";
    //            tblFrameWiseDefect = "FRAMEWISEDEFECT";
    //            tblDefectMaster = "DEFECTMASTER";
    //            tblCodeType = "CODETYPE";
    //            strSiteID = "3";
    //            strtime = "063000";
    //            break;
    //        case "6":
    //            funGetShift = "GETSHIFT";
    //            tblPrd005 = "PRD005";
    //            tblModelWiseMapping = "modelwisemapping";
    //            tblPrd_VQMS_Header = "PRD_VQMS_HEADER";
    //            tblFrameWiseDefect = "FRAMEWISEDEFECT";
    //            tblDefectMaster = "DEFECTMASTER";
    //            tblCodeType = "CODETYPE";
    //            strSiteID = "6";
    //            strtime = "060000";
    //            break;
    //        case "8":
    //            funGetShift = "GETSHIFT";
    //            tblPrd005 = "PRD005";
    //            tblModelWiseMapping = "modelwisemapping";
    //            tblPrd_VQMS_Header = "PRD_VQMS_HEADER";
    //            tblFrameWiseDefect = "FRAMEWISEDEFECT";
    //            tblDefectMaster = "DEFECTMASTER";
    //            tblCodeType = "CODETYPE";
    //            strSiteID = "8";
    //            strtime = "060000";
    //            break;
    //        case "21":
    //            funGetShift = "GETSHIFT";
    //            tblPrd005 = "PRD005";
    //            tblModelWiseMapping = "modelwisemapping";
    //            tblPrd_VQMS_Header = "PRD_VQMS_HEADER";
    //            tblFrameWiseDefect = "FRAMEWISEDEFECT";
    //            tblDefectMaster = "DEFECTMASTER";
    //            tblCodeType = "CODETYPE";
    //            strSiteID = "21";
    //            strtime = "060000";
    //            break;
    //        default:
    //            break;
    //    }
    //}
    public void InitializeFactory(string Factory)
    {
        strConn = objCnStr.getConnectingStringVQMS(Factory);
        switch (Factory)
        {
            case "3":
                funGetShift = "GETSHIFT";
                tblPrd005 = "PRD005";
                tblModelWiseMapping = "modelwisemapping";
                tblPrd_VQMS_Header = "PRD_VQMS_HEADER";
                tblFrameWiseDefect = "FRAMEWISEDEFECT";
                tblDefectMaster = "DEFECTMASTER";
                tblCodeType = "CODETYPE";
                strSiteID = "3";
                strtime = "063000";
                break;
            case "6":
                funGetShift = "GETSHIFT";
                tblPrd005 = "PRD005";
                tblModelWiseMapping = "modelwisemapping";
                tblPrd_VQMS_Header = "PRD_VQMS_HEADER";
                tblFrameWiseDefect = "FRAMEWISEDEFECT";
                tblDefectMaster = "DEFECTMASTER";
                tblCodeType = "CODETYPE";
                strSiteID = "6";
                strtime = "060000";
                break;
            case "8":
                funGetShift = "GETSHIFT";
                tblPrd005 = "PRD005";
                tblModelWiseMapping = "modelwisemapping";
                tblPrd_VQMS_Header = "PRD_VQMS_HEADER";
                tblFrameWiseDefect = "FRAMEWISEDEFECT";
                tblDefectMaster = "DEFECTMASTER";
                tblCodeType = "CODETYPE";
                strSiteID = "8";
                strtime = "060000";
                break;
            case "21":
                funGetShift = "GETSHIFT";
                tblPrd005 = "PRD005";
                tblModelWiseMapping = "modelwisemapping";
                tblPrd_VQMS_Header = "PRD_VQMS_HEADER";
                tblFrameWiseDefect = "FRAMEWISEDEFECT";
                tblDefectMaster = "DEFECTMASTER";
                tblCodeType = "CODETYPE";
                strSiteID = "21";
                strtime = "060000";
                break;
            default:
                break;
        }
    }
    #endregion


    #region"DashBoard"
    //public DataTable GetDPRStatusCount(String Date, String SiteID, string Shift, String Line)
    //{
    //    OracleCommand oCmd = new OracleCommand();
    //    DataTable dt = new DataTable();
    //    DataManagement oDataMgmt = new DataManagement();
    //    oCmd.CommandType = System.Data.CommandType.StoredProcedure;
    //    oCmd.CommandText = "PKG_VQMS_DPR.SPROC_DPRSTATUS_CNT_GET";
    //    oCmd.Parameters.Add("DATE_IN", OracleDbType.Varchar2).Value = Date;
    //    oCmd.Parameters.Add("SITEID_IN", OracleDbType.Int32).Value = SiteID;
    //    oCmd.Parameters.Add("SHIFT_IN", OracleDbType.Varchar2).Value = Shift;
    //    oCmd.Parameters.Add("LINE_IN", OracleDbType.Varchar2).Value = Line;
    //    oCmd.Parameters.Add("CUR_DETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

    //    dt = oDataMgmt.GetDataTable(oCmd);
    //    return (dt);
    //}

    public DataTable GetDPRStatusCount(String Date, String SiteID, string Shift, String Line)
    {
        //DPR ob = new DPR(SiteID);
        InitializeFactory(SiteID);
        DataSet objDS4 = new DataSet();
        using (objCn = new OracleConnection())
        {
            objCn.ConnectionString = strConn;
            try
            {
                objCn.Open();
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                string strSql = string.Empty;
                strSql = strSql + " SELECT *";
                strSql = strSql + " FROM (SELECT NVL(D.CNT, 0) CNT, 'DIRECT PASS' AS PASS_TYPE, 'B' SC";
                strSql = strSql + " FROM DUAL";
                strSql = strSql + " LEFT JOIN (SELECT COUNT(CHASSIS_NUMBER) AS CNT, PASS_TYPE";
                strSql = strSql + " FROM (SELECT A.CHASSIS_NUMBER,";
                strSql = strSql + " 'DIRECT PASS' AS PASS_TYPE,";
                strSql = strSql + " " + funGetShift + "(TO_CHAR(TO_DATE('" + Date + "'),";
                strSql = strSql + " 'YYYYMMDD'),";
                strSql = strSql + " A.PASS_TIME,";
                strSql = strSql + " TO_CHAR(A.PASS_DATE,";
                strSql = strSql + " 'YYYYMMDD'),";
                strSql = strSql + " '" + SiteID + "') ST_CODE";
                strSql = strSql + " FROM " + tblPrd_VQMS_Header + " A";
                strSql = strSql + " WHERE (('" + Line + "' IS NOT NULL AND";
                strSql = strSql + " A.LINE = '" + Line + "') OR";
                strSql = strSql + " ('" + Line + "' IS NULL AND 1 = 1))";
                strSql = strSql + " AND (A.PASS_DATE BETWEEN '" + Date + "' AND";
                strSql = strSql + " TO_CHAR(TO_DATE('" + Date + "') + 1))";
                strSql = strSql + " AND A.PASS_TYPE = 'D')";
                strSql = strSql + " WHERE (('" + Shift + "' IS NULL AND";
                strSql = strSql + " (ST_CODE = 'A' or ST_CODE = 'B')) OR";
                strSql = strSql + " ('" + Shift + "' IS NOT NULL AND '" + Shift + "' = 'A' AND";
                strSql = strSql + " ST_CODE = 'A') OR";
                strSql = strSql + " ('" + Shift + "' IS NOT NULL AND '" + Shift + "' = 'B' AND";
                strSql = strSql + " (ST_CODE = 'B')))";
                strSql = strSql + " ) D";
                strSql = strSql + " ON PASS_TYPE = D.PASS_TYPE";
                strSql = strSql + " UNION";
                strSql = strSql + " SELECT NVL(D.CNT, 0) CNT,";
                strSql = strSql + " 'STRAIGHT PASS' AS PASS_TYPE,";
                strSql = strSql + " 'C' SC";
                strSql = strSql + " FROM DUAL";
                strSql = strSql + " LEFT JOIN (SELECT COUNT(CHASSIS_NUMBER) AS CNT, PASS_TYPE";
                strSql = strSql + " FROM (SELECT A.CHASSIS_NUMBER,";
                strSql = strSql + " 'STRAIGHT PASS' AS PASS_TYPE,";
                strSql = strSql + " " + funGetShift + "(TO_CHAR(TO_DATE('" + Date + "'),";
                strSql = strSql + " 'YYYYMMDD'),";
                strSql = strSql + " A.PASS_TIME,";
                strSql = strSql + " TO_CHAR(A.PASS_DATE,";
                strSql = strSql + " 'YYYYMMDD'),";
                strSql = strSql + " '" + SiteID + "') ST_CODE";
                strSql = strSql + " FROM " + tblPrd_VQMS_Header + " A";
                strSql = strSql + " WHERE (('" + Line + "' IS NOT NULL AND";
                strSql = strSql + " A.LINE = '" + Line + "') OR";
                strSql = strSql + " ('" + Line + "' IS NULL AND 1 = 1))";
                strSql = strSql + " AND (A.PASS_DATE BETWEEN '" + Date + "' AND";
                strSql = strSql + " TO_CHAR(TO_DATE('" + Date + "') + 1))";
                strSql = strSql + " AND A.PASS_TYPE <> 'F')";
                strSql = strSql + " WHERE (('" + Shift + "' IS NULL AND";
                strSql = strSql + " (ST_CODE = 'A' or ST_CODE = 'B')) OR";
                strSql = strSql + " ('" + Shift + "' IS NOT NULL AND '" + Shift + "' = 'A' AND";
                strSql = strSql + " ST_CODE = 'A') OR";
                strSql = strSql + " ('" + Shift + "' IS NOT NULL AND '" + Shift + "' = 'B' AND";
                strSql = strSql + " (ST_CODE = 'B')))";
                strSql = strSql + " ) D";
                strSql = strSql + " ON PASS_TYPE = D.PASS_TYPE";
                strSql = strSql + " UNION";
                strSql = strSql + " SELECT NVL(D.CNT, 0) CNT, 'DEFECTIVE' AS PASS_TYPE, 'D' SC";
                strSql = strSql + " FROM DUAL";
                strSql = strSql + " LEFT JOIN (SELECT COUNT(CHASSIS_NUMBER) AS CNT, PASS_TYPE";
                strSql = strSql + " FROM (SELECT A.CHASSIS_NUMBER,";
                strSql = strSql + " 'DEFECTIVE' AS PASS_TYPE,";
                strSql = strSql + " " + funGetShift + "(TO_CHAR(TO_DATE('" + Date + "'),";
                strSql = strSql + " 'YYYYMMDD'),";
                strSql = strSql + " A.PASS_TIME,";
                strSql = strSql + " TO_CHAR(A.PASS_DATE,";
                strSql = strSql + " 'YYYYMMDD'),";
                strSql = strSql + " '" + SiteID + "') ST_CODE";
                strSql = strSql + " FROM " + tblPrd_VQMS_Header + " A";
                strSql = strSql + " WHERE (('" + Line + "' IS NOT NULL AND";
                strSql = strSql + " A.LINE = '" + Line + "') OR";
                strSql = strSql + " ('" + Line + "' IS NULL AND 1 = 1))";
                strSql = strSql + " AND (A.PASS_DATE BETWEEN '" + Date + "' AND";
                strSql = strSql + " TO_CHAR(TO_DATE('" + Date + "') + 1))";
                strSql = strSql + " AND A.PASS_TYPE = 'F')";
                strSql = strSql + " WHERE (('" + Shift + "' IS NULL AND";
                strSql = strSql + " (ST_CODE = 'A' or ST_CODE = 'B')) OR";
                strSql = strSql + " ('" + Shift + "' IS NOT NULL AND '" + Shift + "' = 'A' AND";
                strSql = strSql + " ST_CODE = 'A') OR";
                strSql = strSql + " ('" + Shift + "' IS NOT NULL AND '" + Shift + "' = 'B' AND";
                strSql = strSql + " (ST_CODE = 'B'))))D";
                strSql = strSql + " ON PASS_TYPE = D.PASS_TYPE";
                strSql = strSql + " UNION";
                strSql = strSql + " SELECT NVL(SUM(PRODUCTION_QTY), 0) PRODUCTION_QTY,";
                strSql = strSql + " 'TOTAL PRODUCTION' as PASSTYPE,";
                strSql = strSql + " 'A' SC";
                strSql = strSql + " FROM (SELECT '1' JC, COUNT(*) AS PRODUCTION_QTY";
                strSql = strSql + " FROM (SELECT K.AF_DAT,";
                strSql = strSql + " " + funGetShift + "(TO_CHAR(TO_DATE('" + Date + "'),";
                strSql = strSql + " 'YYYYMMDD'),";
                strSql = strSql + " TO_CHAR(K.AF_TIM,";
                strSql = strSql + " 'HH24MISS'),";
                strSql = strSql + " TO_NUMBER(K.AF_DAT),";
                strSql = strSql + " '" + SiteID + "') ST_CODE";
                strSql = strSql + " FROM " + tblPrd005 + "           K,";
                strSql = strSql + " " + tblModelWiseMapping + " M";
                strSql = strSql + " WHERE 1 = 1";
                strSql = strSql + " AND (K.MTOCD) = (M.MTOCODE)";
                strSql = strSql + " AND (('" + Line + "' IS NOT NULL AND";
                strSql = strSql + " K.PRD_LIN = '" + Line + "') OR";
                strSql = strSql + " ('" + Line + "' IS NULL AND 1 = 1))";
                strSql = strSql + " AND K.AF_DAT BETWEEN";
                strSql = strSql + " TO_CHAR(TO_DATE('" + Date + "'), 'YYYYMMDD') AND";
                strSql = strSql + " TO_CHAR(TO_DATE('" + Date + "') + 1,";
                strSql = strSql + " 'YYYYMMDD'))";
                strSql = strSql + " WHERE (('" + Shift + "' IS NULL AND";
                strSql = strSql + " (ST_CODE = 'A' or ST_CODE = 'B')) OR";
                strSql = strSql + " ('" + Shift + "' IS NOT NULL AND '" + Shift + "' = 'A' AND";
                strSql = strSql + " ST_CODE = 'A') OR";
                strSql = strSql + " ('" + Shift + "' IS NOT NULL AND '" + Shift + "' = 'B' AND";
                strSql = strSql + " (ST_CODE = 'B'))))P";
                strSql = strSql + " UNION";
                strSql = strSql + " SELECT COUNT(*) Not_Entered_Frames, PASSTYPE, SC";
                strSql = strSql + " FROM (SELECT A.CHASSIS_NUMBER,";
                strSql = strSql + " " + funGetShift + "(TO_CHAR(TO_DATE('" + Date + "'),";
                strSql = strSql + " 'YYYYMMDD'),";
                strSql = strSql + " A.PASS_TIME,";
                strSql = strSql + " TO_CHAR(A.PASS_DATE,";
                strSql = strSql + " 'YYYYMMDD'),";
                strSql = strSql + " '" + SiteID + "') ST_CODE,";
                strSql = strSql + " 'DEFECT NOT ENTERED' as PASSTYPE,";
                strSql = strSql + " 'E' SC";
                strSql = strSql + " FROM " + tblPrd_VQMS_Header + " A";
                strSql = strSql + " WHERE NOT EXISTS";
                strSql = strSql + " (SELECT MODELCODE";
                strSql = strSql + " FROM " + tblFrameWiseDefect + " B";
                strSql = strSql + " WHERE B.ENG_FRAME_TYPE = A.ENG_FRAME_TYPE";
                strSql = strSql + " AND A.CHASSIS_NUMBER = CHASSIS_NUMBER)";
                strSql = strSql + " AND A.PASS_TYPE <> 'D'";
                strSql = strSql + " AND (A.PASS_DATE BETWEEN '" + Date + "' AND";
                strSql = strSql + " TO_CHAR(TO_DATE('" + Date + "') + 1))";
                strSql = strSql + " AND (('" + Line + "' IS NOT NULL AND A.LINE = '" + Line + "') OR";
                strSql = strSql + " ('" + Line + "' IS NULL AND 1 = 1)))";
                strSql = strSql + " WHERE (('" + Shift + "' IS NULL AND";
                strSql = strSql + " (ST_CODE = 'A' or ST_CODE = 'B')) OR";
                strSql = strSql + " ('" + Shift + "' IS NOT NULL AND '" + Shift + "' = 'A' AND";
                strSql = strSql + " ST_CODE = 'A') OR";
                strSql = strSql + " ('" + Shift + "' IS NOT NULL AND '" + Shift + "' = 'B' AND";
                strSql = strSql + " (ST_CODE = 'B')))";
                strSql = strSql + " )";
                strSql = strSql + " ORDER BY SC";
                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.Text;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                objAdr.Fill(objDS4);
                return objDS4.Tables[0];
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

    //public DataTable GetDPRStatusPercentage(String Date, String SiteID, string Shift, String Line)
    //{
    //    OracleCommand oCmd = new OracleCommand();
    //    DataTable dt = new DataTable();
    //    DataManagement oDataMgmt = new DataManagement();
    //    oCmd.CommandType = System.Data.CommandType.StoredProcedure;
    //    oCmd.CommandText = "PKG_VQMS_DPR.SPROC_DPRSTATUS_PER_GET";
    //    oCmd.Parameters.Add("DATE_IN", OracleDbType.Varchar2).Value = Date;
    //    oCmd.Parameters.Add("SITEID_IN", OracleDbType.Int32).Value = SiteID;
    //    oCmd.Parameters.Add("SHIFT_IN", OracleDbType.Varchar2).Value = Shift;
    //    oCmd.Parameters.Add("LINE_IN", OracleDbType.Varchar2).Value = Line;
    //    oCmd.Parameters.Add("CUR_DETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

    //    dt = oDataMgmt.GetDataTable(oCmd);
    //    return (dt);
    //}

    public DataTable GetDPRStatusPercentage(String Date, String SiteID, string Shift, String Line)
    {
        //DPR ob = new DPR(SiteID);
        InitializeFactory(SiteID);
        DataSet objDS4 = new DataSet();
        using (objCn = new OracleConnection())
        {
            objCn.ConnectionString = strConn;
            try
            {
                objCn.Open();
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                string strSql = string.Empty;
                strSql = strSql + " SELECT TMP.PASS_TYPE,";
                strSql = strSql + " ROUND((CNT / DECODE(TCNT, 0, 1, TCNT)) * 100, 1) AS PER";
                strSql = strSql + " FROM (SELECT NVL(CNT, 0) CNT, PASS_TYPE, '1' as I";
                strSql = strSql + " FROM (SELECT NVL(D.CNT, 0) CNT, 'DIRECT PASS' AS PASS_TYPE";
                strSql = strSql + " FROM DUAL";
                strSql = strSql + " LEFT JOIN (SELECT COUNT(CHASSIS_NUMBER) AS CNT,";
                strSql = strSql + " PASS_TYPE";
                strSql = strSql + " FROM (SELECT A.CHASSIS_NUMBER,";
                strSql = strSql + " 'DIRECT PASS' AS PASS_TYPE,";
                strSql = strSql + " " + funGetShift + "(TO_CHAR(TO_DATE('" + Date + "'),";
                strSql = strSql + " 'YYYYMMDD'),";
                strSql = strSql + " A.PASS_TIME,";
                strSql = strSql + " TO_CHAR(A.PASS_DATE,";
                strSql = strSql + " 'YYYYMMDD'),";
                strSql = strSql + " '" + SiteID + "') ST_CODE";
                strSql = strSql + " FROM " + tblPrd_VQMS_Header + " A";
                strSql = strSql + " WHERE (('" + Line + "' IS NOT NULL AND";
                strSql = strSql + " A.LINE = '" + Line + "') OR";
                strSql = strSql + " ('" + Line + "' IS NULL AND 1 = 1))";
                strSql = strSql + " AND (A.PASS_DATE BETWEEN";
                strSql = strSql + " '" + Date + "' AND";
                strSql = strSql + " TO_CHAR(TO_DATE('" + Date + "') + 1))";
                strSql = strSql + " AND A.PASS_TYPE = 'D')";
                strSql = strSql + " WHERE (('" + Shift + "' IS NULL AND";
                strSql = strSql + " (ST_CODE = 'A' or ST_CODE = 'B')) OR";
                strSql = strSql + " ('" + Shift + "' IS NOT NULL AND";
                strSql = strSql + " '" + Shift + "' = 'A' AND ST_CODE = 'A') OR";
                strSql = strSql + " ('" + Shift + "' IS NOT NULL AND";
                strSql = strSql + " '" + Shift + "' = 'B' AND";
                strSql = strSql + " (ST_CODE = 'B')))";
                strSql = strSql + " ) D";
                strSql = strSql + " ON PASS_TYPE = D.PASS_TYPE";
                strSql = strSql + " UNION";
                strSql = strSql + " SELECT NVL(D.CNT, 0) CNT,";
                strSql = strSql + " 'STRAIGHT PASS' AS PASS_TYPE";
                strSql = strSql + " FROM DUAL";
                strSql = strSql + " LEFT JOIN (SELECT COUNT(CHASSIS_NUMBER) AS CNT,";
                strSql = strSql + " PASS_TYPE";
                strSql = strSql + " FROM (SELECT A.CHASSIS_NUMBER,";
                strSql = strSql + " 'STRAIGHT PASS' AS PASS_TYPE,";
                strSql = strSql + " " + funGetShift + "(TO_CHAR(TO_DATE('" + Date + "'),";
                strSql = strSql + " 'YYYYMMDD'),";
                strSql = strSql + " A.PASS_TIME,";
                strSql = strSql + " TO_CHAR(A.PASS_DATE,";
                strSql = strSql + " 'YYYYMMDD'),";
                strSql = strSql + " '" + SiteID + "') ST_CODE";
                strSql = strSql + " FROM " + tblPrd_VQMS_Header + " A";
                strSql = strSql + " WHERE (('" + Line + "' IS NOT NULL AND";
                strSql = strSql + " A.LINE = '" + Line + "') OR";
                strSql = strSql + " ('" + Line + "' IS NULL AND 1 = 1))";
                strSql = strSql + " AND (A.PASS_DATE BETWEEN";
                strSql = strSql + " '" + Date + "' AND";
                strSql = strSql + " TO_CHAR(TO_DATE('" + Date + "') + 1))";
                strSql = strSql + " AND A.PASS_TYPE <> 'F')";
                strSql = strSql + " WHERE (('" + Shift + "' IS NULL AND";
                strSql = strSql + " (ST_CODE = 'A' or ST_CODE = 'B')) OR";
                strSql = strSql + " ('" + Shift + "' IS NOT NULL AND";
                strSql = strSql + " '" + Shift + "' = 'A' AND ST_CODE = 'A') OR";
                strSql = strSql + " ('" + Shift + "' IS NOT NULL AND";
                strSql = strSql + " '" + Shift + "' = 'B' AND";
                strSql = strSql + " (ST_CODE = 'B')))";
                strSql = strSql + " ) D";
                strSql = strSql + " ON PASS_TYPE = D.PASS_TYPE";
                strSql = strSql + " UNION";
                strSql = strSql + " SELECT NVL(D.CNT, 0) CNT, 'DEFECTIVE' AS PASS_TYPE";
                strSql = strSql + " FROM DUAL";
                strSql = strSql + " LEFT JOIN (SELECT COUNT(CHASSIS_NUMBER) AS CNT,";
                strSql = strSql + " PASS_TYPE";
                strSql = strSql + " FROM (SELECT A.CHASSIS_NUMBER,";
                strSql = strSql + " 'DEFECTIVE' AS PASS_TYPE,";
                strSql = strSql + " " + funGetShift + "(TO_CHAR(TO_DATE('" + Date + "'),";
                strSql = strSql + " 'YYYYMMDD'),";
                strSql = strSql + " A.PASS_TIME,";
                strSql = strSql + " TO_CHAR(A.PASS_DATE,";
                strSql = strSql + " 'YYYYMMDD'),";
                strSql = strSql + " '" + SiteID + "') ST_CODE";
                strSql = strSql + " FROM " + tblPrd_VQMS_Header + " A";
                strSql = strSql + " WHERE (('" + Line + "' IS NOT NULL AND";
                strSql = strSql + " A.LINE = '" + Line + "') OR";
                strSql = strSql + " ('" + Line + "' IS NULL AND 1 = 1))";
                strSql = strSql + " AND (A.PASS_DATE BETWEEN";
                strSql = strSql + " '" + Date + "' AND";
                strSql = strSql + " TO_CHAR(TO_DATE('" + Date + "') + 1))";
                strSql = strSql + " AND A.PASS_TYPE = 'F')";
                strSql = strSql + " WHERE (('" + Shift + "' IS NULL AND";
                strSql = strSql + " (ST_CODE = 'A' or ST_CODE = 'B')) OR";
                strSql = strSql + " ('" + Shift + "' IS NOT NULL AND";
                strSql = strSql + " '" + Shift + "' = 'A' AND ST_CODE = 'A') OR";
                strSql = strSql + " ('" + Shift + "' IS NOT NULL AND";
                strSql = strSql + " '" + Shift + "' = 'B' AND";
                strSql = strSql + " (ST_CODE = 'B')))) D";
                strSql = strSql + " ON PASS_TYPE = D.PASS_TYPE)) TMP";
                strSql = strSql + " JOIN (SELECT COUNT(CHASSIS_NUMBER) AS TCNT, '1' as I";
                strSql = strSql + " FROM (select A.CHASSIS_NUMBER,";
                strSql = strSql + " " + funGetShift + "(TO_CHAR(TO_DATE('" + Date + "'),";
                strSql = strSql + " 'YYYYMMDD'),";
                strSql = strSql + " A.PASS_TIME,";
                strSql = strSql + " TO_CHAR(A.PASS_DATE,";
                strSql = strSql + " 'YYYYMMDD'),";
                strSql = strSql + " '" + SiteID + "') ST_CODE";
                strSql = strSql + " FROM " + tblPrd_VQMS_Header + " A";
                strSql = strSql + " WHERE (('" + Line + "' IS NOT NULL AND A.LINE = '" + Line + "') OR";
                strSql = strSql + " ('" + Line + "' IS NULL AND 1 = 1))";
                strSql = strSql + " AND (A.PASS_DATE BETWEEN '" + Date + "' AND";
                strSql = strSql + " TO_CHAR(TO_DATE('" + Date + "') + 1)))";
                strSql = strSql + " WHERE (ST_CODE = 'A' or ST_CODE = 'B')) tot";
                strSql = strSql + " on TMP.I = tot.I";
                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.Text;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                objAdr.Fill(objDS4);
                return objDS4.Tables[0];
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
    
    //public DataTable GetSectionWiseDefect(String Date, String SiteID, string Shift, String Line)
    //{
    //    OracleCommand oCmd = new OracleCommand();
    //    DataTable dt = new DataTable();
    //    DataManagement oDataMgmt = new DataManagement();

    //    oCmd.CommandType = System.Data.CommandType.StoredProcedure;
    //    oCmd.CommandText = "PKG_VQMS_DPR.SPROC_SECTIONWISEDEFECT_GET";
    //    oCmd.Parameters.Add("DATE_IN", OracleDbType.Varchar2).Value = Date;
    //    oCmd.Parameters.Add("SITEID_IN", OracleDbType.Int32).Value = SiteID;
    //    oCmd.Parameters.Add("SHIFT_IN", OracleDbType.Varchar2).Value = Shift;
    //    oCmd.Parameters.Add("LINE_IN", OracleDbType.Varchar2).Value = Line;
    //    oCmd.Parameters.Add("CUR_DETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

    //    dt = oDataMgmt.GetDataTable(oCmd);
    //    return (dt);
    //}

    public DataTable GetSectionWiseDefect(String Date, String SiteID, string Shift, String Line)
    {
        //DPR ob = new DPR(SiteID);
        InitializeFactory(SiteID);
        DataSet objDS4 = new DataSet();
        using (objCn = new OracleConnection())
        {
            objCn.ConnectionString = strConn;
            try
            {
                objCn.Open();
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;
                string strSql = string.Empty;
                strSql = strSql + " SELECT DEFECTSECTION,";
                strSql = strSql + " CNT,";
                strSql = strSql + " SUM(CNT) OVER(ORDER BY CNT DESC ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) Running_total,";
                strSql = strSql + " '0' AS JC,";
                strSql = strSql + " ROUND((SUM(CNT) OVER(ORDER BY CNT DESC ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) / TOTAL.TC) * 100,2) cm";
                strSql = strSql + " FROM (SELECT COUNT(CHASSIS_NUMBER) CNT, DEFECTSECTION";
                strSql = strSql + " FROM (SELECT A.DEFECTSECTION,";
                strSql = strSql + " a.CHASSIS_NUMBER,";
                strSql = strSql + " " + funGetShift + "(TO_CHAR(TO_DATE('" + Date + "'),'YYYYMMDD'),H.PASS_TIME,TO_CHAR(H.PASS_DATE,'YYYYMMDD'),'" + SiteID + "') ST_CODE";
                strSql = strSql + " FROM " + tblFrameWiseDefect + " A,";
                strSql = strSql + " " + tblPrd_VQMS_Header + " H,";
                strSql = strSql + " " + tblCodeType + "        F";
                strSql = strSql + " WHERE 1 = 1";
                strSql = strSql + " AND (H.ENG_FRAME_TYPE) = (A.ENG_FRAME_TYPE)";
                strSql = strSql + " AND (H.CHASSIS_NUMBER) = (A.CHASSIS_NUMBER)";
                strSql = strSql + " AND (A.DEFECTSECTION) = (F.SUBCODE)";
                strSql = strSql + " AND UPPER(F.CODE) = 'SECTION'";
                strSql = strSql + " AND (('" + Line + "' IS NOT NULL AND H.LINE = '" + Line + "') OR ('" + Line + "' IS NULL AND 1 = 1))";
                strSql = strSql + " AND (H.PASS_DATE BETWEEN '" + Date + "' AND TO_CHAR(TO_DATE('" + Date + "') + 1)))";
                strSql = strSql + " WHERE (('" + Shift + "' IS NULL AND (ST_CODE = 'A' or ST_CODE = 'B')) OR";
                strSql = strSql + " ('" + Shift + "' IS NOT NULL AND '" + Shift + "' = 'A' AND ST_CODE = 'A') OR";
                strSql = strSql + " ('" + Shift + "' IS NOT NULL AND '" + Shift + "' = 'B' AND (ST_CODE = 'B')))";
                strSql = strSql + " GROUP BY DEFECTSECTION) MD";
                strSql = strSql + " left JOIN (SELECT COUNT(CHASSIS_NUMBER) TC, '0' AS JC";
                strSql = strSql + " FROM (SELECT  a.CHASSIS_NUMBER,";
                strSql = strSql + " " + funGetShift + "(TO_CHAR(TO_DATE('" + Date + "'),'YYYYMMDD'),H.PASS_TIME,TO_CHAR(H.PASS_DATE,'YYYYMMDD'),'" + SiteID + "') ST_CODE";
                strSql = strSql + " FROM " + tblFrameWiseDefect + " A,";
                strSql = strSql + " " + tblPrd_VQMS_Header + " H,";
                strSql = strSql + " " + tblCodeType + "        F";
                strSql = strSql + " WHERE 1 = 1";
                strSql = strSql + " AND (H.ENG_FRAME_TYPE) = (A.ENG_FRAME_TYPE)";
                strSql = strSql + " AND (H.CHASSIS_NUMBER) = (A.CHASSIS_NUMBER)";
                strSql = strSql + " AND (A.DEFECTSECTION) = (F.SUBCODE)";
                strSql = strSql + " AND UPPER(F.CODE) = 'SECTION'";
                strSql = strSql + " AND (('" + Line + "' IS NOT NULL AND H.LINE = '" + Line + "') OR ('" + Line + "' IS NULL AND 1 = 1))";
                strSql = strSql + " AND (H.PASS_DATE BETWEEN '" + Date + "' AND TO_CHAR(TO_DATE('" + Date + "') + 1)))";
                strSql = strSql + " WHERE (('" + Shift + "' IS NULL AND (ST_CODE = 'A' or ST_CODE = 'B')) OR";
                strSql = strSql + " ('" + Shift + "' IS NOT NULL AND '" + Shift + "' = 'A' AND ST_CODE = 'A') OR";
                strSql = strSql + " ('" + Shift + "' IS NOT NULL AND '" + Shift + "' = 'B' AND (ST_CODE = 'B')))) TOTAL";
                strSql = strSql + " ON TOTAL.JC = JC";
                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.Text;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                objAdr.Fill(objDS4);
                return objDS4.Tables[0];
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

    #region Functions
    public DataTable GetLine(String SitedID)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_VQMS_DPR.SPROC_LINE_GET";
        oCmd.Parameters.Add("SITEID_IN", OracleDbType.Varchar2).Value = SitedID;
        oCmd.Parameters.Add("CUR_DETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }
    public DataTable GetModel(String SitedID)
    {
        OracleCommand oCmd = new OracleCommand();
        DataTable dt = new DataTable();
        oCmd.CommandType = System.Data.CommandType.StoredProcedure;
        oCmd.BindByName = true;
        oCmd.CommandText = "PKG_VQMS_DPR.SPROC_MODELS_GET";
        oCmd.Parameters.Add("SITEID_IN", OracleDbType.Varchar2).Value = SitedID;
        oCmd.Parameters.Add("CUR_DETAILS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        dt = oDataMgmt.GetDataTable(oCmd);
        return (dt);
    }
    public DataTable GetDefectTrend(String Month, String Year, String Model, String Factory)
    {
        string strMonthYear = DateTime.ParseExact("01-" + Month + "-" + Year, "dd-M-yyyy", null).ToString("MM-yyyy");

        string strNextMonfirstDay = DateTime.ParseExact("01-" + Month + "-" + Year, "dd-M-yyyy", null).AddMonths(1).ToString("dd-MMM-yyyy");
        string strFirstDay = DateTime.ParseExact("01-" + Month + "-" + Year, "dd-M-yyyy", null).ToString("dd-MMM-yyyy");

        Month = DateTime.ParseExact("01-" + Month + "-" + Year, "dd-M-yyyy", null).ToString("MM");
        using (objCn = new OracleConnection())
        {
            objCn.ConnectionString = strConn;
            try
            {
                objCn.Open();
                objCmd = new OracleCommand();






                string sqlString = "SELECT DFM.DEFECTDESCRIPTION," +
                "       C.DISC," +
                "       SC.SUBCODE," +
                "       DFM.DEFECTCODE AS DCODE," +
                "       T.*" +
                "  FROM " + tblDefectMaster + " DFM," +
                "  " + tblCodeType + " C" +
                "    ," +
                "  " + tblCodeType + " SC," +
                "  (SELECT DEFECTCODE," +
                "                    NVL(SUM(A_SHIFT_01), 0) A_SHIFT_01," +
                "                    NVL(SUM(B_SHIFT_01), 0) B_SHIFT_01," +
                "                    NVL(SUM(A_SHIFT_02), 0) A_SHIFT_02," +
                "                    NVL(SUM(B_SHIFT_02), 0) B_SHIFT_02," +
                "                    NVL(SUM(A_SHIFT_03), 0) A_SHIFT_03," +
                "                    NVL(SUM(B_SHIFT_03), 0) B_SHIFT_03," +
                "                    NVL(SUM(A_SHIFT_04), 0) A_SHIFT_04," +
                "                    NVL(SUM(B_SHIFT_04), 0) B_SHIFT_04," +
                "                    NVL(SUM(A_SHIFT_05), 0) A_SHIFT_05," +
                "                    NVL(SUM(B_SHIFT_05), 0) B_SHIFT_05," +
                "                    NVL(SUM(A_SHIFT_06), 0) A_SHIFT_06," +
                "                    NVL(SUM(B_SHIFT_06), 0) B_SHIFT_06," +
                "                    NVL(SUM(A_SHIFT_07), 0) A_SHIFT_07," +
                "                    NVL(SUM(B_SHIFT_07), 0) B_SHIFT_07," +
                "                    NVL(SUM(A_SHIFT_08), 0) A_SHIFT_08," +
                "                    NVL(SUM(B_SHIFT_08), 0) B_SHIFT_08," +
                "                    NVL(SUM(A_SHIFT_09), 0) A_SHIFT_09," +
                "                    NVL(SUM(B_SHIFT_09), 0) B_SHIFT_09," +
                "                    NVL(SUM(A_SHIFT_10), 0) A_SHIFT_10," +
                "                    NVL(SUM(B_SHIFT_10), 0) B_SHIFT_10," +
                "                    NVL(SUM(A_SHIFT_11), 0) A_SHIFT_11," +
                "                    NVL(SUM(B_SHIFT_11), 0) B_SHIFT_11," +
                "                    NVL(SUM(A_SHIFT_12), 0) A_SHIFT_12," +
                "                    NVL(SUM(B_SHIFT_12), 0) B_SHIFT_12," +
                "                    NVL(SUM(A_SHIFT_13), 0) A_SHIFT_13," +
                "                    NVL(SUM(B_SHIFT_13), 0) B_SHIFT_13," +
                "                    NVL(SUM(A_SHIFT_14), 0) A_SHIFT_14," +
                "                    NVL(SUM(B_SHIFT_14), 0) B_SHIFT_14," +
                "                    NVL(SUM(A_SHIFT_15), 0) A_SHIFT_15," +
                "                    NVL(SUM(B_SHIFT_15), 0) B_SHIFT_15," +
                "                    NVL(SUM(A_SHIFT_16), 0) A_SHIFT_16," +
                "                    NVL(SUM(B_SHIFT_16), 0) B_SHIFT_16," +
                "                    NVL(SUM(A_SHIFT_17), 0) A_SHIFT_17," +
                "                    NVL(SUM(B_SHIFT_17), 0) B_SHIFT_17," +
                "                    NVL(SUM(A_SHIFT_18), 0) A_SHIFT_18," +
                "                    NVL(SUM(B_SHIFT_18), 0) B_SHIFT_18," +
                "                    NVL(SUM(A_SHIFT_19), 0) A_SHIFT_19," +
                "                    NVL(SUM(B_SHIFT_19), 0) B_SHIFT_19," +
                "                    NVL(SUM(A_SHIFT_20), 0) A_SHIFT_20," +
                "                    NVL(SUM(B_SHIFT_20), 0) B_SHIFT_20," +
                "                    NVL(SUM(A_SHIFT_21), 0) A_SHIFT_21," +
                "                    NVL(SUM(B_SHIFT_21), 0) B_SHIFT_21," +
                "                    NVL(SUM(A_SHIFT_22), 0) A_SHIFT_22," +
                "                    NVL(SUM(B_SHIFT_22), 0) B_SHIFT_22," +
                "                    NVL(SUM(A_SHIFT_23), 0) A_SHIFT_23," +
                "                    NVL(SUM(B_SHIFT_23), 0) B_SHIFT_23," +
                "                    NVL(SUM(A_SHIFT_24), 0) A_SHIFT_24," +
                "                    NVL(SUM(B_SHIFT_24), 0) B_SHIFT_24," +
                "                    NVL(SUM(A_SHIFT_25), 0) A_SHIFT_25," +
                "                    NVL(SUM(B_SHIFT_25), 0) B_SHIFT_25," +
                "                    NVL(SUM(A_SHIFT_26), 0) A_SHIFT_26," +
                "                    NVL(SUM(B_SHIFT_26), 0) B_SHIFT_26," +
                "                    NVL(SUM(A_SHIFT_27), 0) A_SHIFT_27," +
                "                    NVL(SUM(B_SHIFT_27), 0) B_SHIFT_27," +
                "                    NVL(SUM(A_SHIFT_28), 0) A_SHIFT_28," +
                "                    NVL(SUM(B_SHIFT_28), 0) B_SHIFT_28," +
                "                    NVL(SUM(A_SHIFT_29), 0) A_SHIFT_29," +
                "                    NVL(SUM(B_SHIFT_29), 0) B_SHIFT_29," +
                "                    NVL(SUM(A_SHIFT_30), 0) A_SHIFT_30," +
                "                    NVL(SUM(B_SHIFT_30), 0) B_SHIFT_30," +
                "                    NVL(SUM(A_SHIFT_31), 0) A_SHIFT_31," +
                "                    NVL(SUM(B_SHIFT_31), 0) B_SHIFT_31" +
                "               FROM (SELECT NEWDT," +
                "                            DEFECTCODE," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '01' THEN" +
                "                               NVL(A_SHIFT, 0)" +
                "                            END AS A_SHIFT_01," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '01' THEN" +
                "                               NVL(B_SHIFT, 0)" +
                "                            END AS B_SHIFT_01," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '02' THEN" +
                "                               NVL(A_SHIFT, 0)" +
                "                            END AS A_SHIFT_02," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '02' THEN" +
                "                               NVL(B_SHIFT, 0)" +
                "                            END AS B_SHIFT_02," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '03' THEN" +
                "                               NVL(A_SHIFT, 0)" +
                "                            END AS A_SHIFT_03," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '03' THEN" +
                "                               NVL(B_SHIFT, 0)" +
                "                            END AS B_SHIFT_03," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '04' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_04," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '04' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_04," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '05' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_05," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '05' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_05," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '06' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_06," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '06' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_06," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '07' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_07," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '07' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_07," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '08' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_08," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '08' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_08," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '09' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_09," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '09' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_09," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '10' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_10," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '10' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_10," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '11' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_11," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '11' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_11," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '12' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_12," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '12' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_12," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '13' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_13," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '13' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_13," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '14' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_14," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '14' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_14," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '15' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_15," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '15' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_15," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '16' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_16," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '16' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_16," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '17' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_17," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '17' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_17," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '18' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_18," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '18' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_18," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '19' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_19," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '19' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_19," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '20' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_20," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '20' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_20," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '21' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_21," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '21' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_21," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '22' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_22," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '22' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_22," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '23' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_23," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '23' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_23," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '24' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_24," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '24' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_24," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '25' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_25," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '25' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_25," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '26' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_26," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '26' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_26," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '27' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_27," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '27' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_27," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '28' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_28," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '28' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_28," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '29' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_29," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '29' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_29," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '30' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_30," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '30' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_30," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '31' THEN" +
                "                               A_SHIFT" +
                "                            END AS A_SHIFT_31," +
                "                            CASE" +
                "                              WHEN TO_CHAR(NEWDT, 'DD') = '31' THEN" +
                "                               B_SHIFT" +
                "                            END AS B_SHIFT_31" +
                "                       FROM (SELECT NEWDT," +
                "                                    DEFECTCODE," +
                "                                    COUNT(A_SHIFT) A_SHIFT," +
                "                                    COUNT(B_SHIFT) B_SHIFT" +
                "                               FROM (SELECT DECODE(ST_CODE," +
                "                                                   'A'," +
                "                                                   NEWDT," +
                "                                                   'B'," +
                "                                                   NEWDT," +
                "                                                   'C'," +
                "                                                   NEWDT-1" +
                "                                                    ) AS NEWDT," +
                "                                            NEWDT AS NEWDT_OLD," +
                "                                            DEFECTCODE," +
                "                                            CASE" +
                "                                              WHEN ST_CODE = 'A' THEN" +
                "                                               PASS_DATE" +
                "                                            END AS A_SHIFT," +
                "                                            CASE" +
                "                                              WHEN ST_CODE = 'B' OR ST_CODE = 'C' " +
                "                                                    THEN" +
                "                                               PASS_DATE" +
                "                                            END AS B_SHIFT" +
                "                                       FROM (SELECT T.NEWDT," +
                "                                                    D.PASS_DATE," +
                "                                                    DEFECTCODE," +
                "                                                    " + funGetShift + "(TO_CHAR(TO_DATE(T.NEWDT)," +
                "                                                                     'YYYYMMDD')," +
                "                                                             D.PASS_TIME," +
                "                                                             TO_NUMBER(TO_CHAR(D.PASS_DATE," +
                "                                                                               'YYYYMMDD'))," +
                "                                                             " + Factory + ") ST_CODE" +
                "                                               FROM (SELECT TO_DATE((SELECT TRUNC(TO_DATE('" + strMonthYear + "','MM-YYYY'),'MM') - 1 AS DT FROM DUAL)) + ROWNUM AS NEWDT" +
                "                                                           FROM ALL_OBJECTS" +
                "                                                           WHERE ROWNUM <= (LAST_DAY(TO_DATE('" + strMonthYear + "', 'MM-YYYY')) -" +
                "                                                           TRUNC(TO_DATE('" + strMonthYear + "', 'MM-YYYY'))) + 2" +
                "                                                           ORDER BY 1) T," +
                "                                                           (SELECT H.PASS_DATE," +
                "                                                                H.PASS_TIME," +
                "                                                                D.DEFECTCODE," +
                "                                                                D.ENG_FRAME_TYPE" +
                "                                                           FROM " + tblPrd_VQMS_Header + " H," +
                "                                                                " + tblFrameWiseDefect + " D" +
                "                                                          WHERE D.MODELCODE =" +
                "                                                                '" + Model + "'                                                            AND H.ENG_FRAME_TYPE =" +
                "                                                                D.ENG_FRAME_TYPE" +
                "                                                            AND H.CHASSIS_NUMBER =" +
                "                                                                D.CHASSIS_NUMBER" +

                "                                                            AND " +
                "                                                            (" +
                "                                                               (H.PASS_TIME >= " + strtime + " AND H.PASS_DATE = '" + strFirstDay + "')" +
                "                                                                  OR " +
                "                                                               (H.PASS_DATE > '" + strFirstDay + "'and TO_CHAR(H.PASS_DATE, 'MM') = '" + Month + "' AND TO_CHAR(H.PASS_DATE, 'YYYY') = '" + Year + "')" +
                "                                                                  OR " +
                "                                                               (" +
                "                                                                (H.PASS_TIME < " + strtime + " AND H.PASS_DATE = '" + strNextMonfirstDay + "')" +
                "                                                               )" +
                "                                                             )) D" +


                //"                                                            AND ((TO_CHAR(H.PASS_DATE," +
                    //"                                                                          'MM') = '" + Month + "' AND" +
                    //"                                                                TO_CHAR(H.PASS_DATE," +
                    //"                                                                          'YYYY') =" +
                    //"                                                                '" + Year + "') OR" +
                    //"                                                                ((H.PASS_DATE =" +
                    //"                                                                '" + strNextMonfirstDay + "') AND" +
                    //"                                                                (H.PASS_TIME >=" +
                    //"                                                                000001 AND" +
                    //"                                                                H.PASS_TIME <=" +
                    //"                                                                062959)))) D" +



                "                                                 WHERE T.NEWDT = D.PASS_DATE(+)))" +
                "                              GROUP BY NEWDT, DEFECTCODE))" +
                "              GROUP BY DEFECTCODE) T" +
                " WHERE C.SUBCODE = DFM.DEFECTCATEGORY" +
                "   AND C.CODE = 'CATEGORY'" +
                "   AND DFM.MODELCODE = '" + Model + "'" +
                "   AND SC.SUBCODE = DFM.DEFECTSECTION" +
                "   AND SC.CODE = 'SECTION'" +
                "   AND DFM.DEFECTCODE (+) = T.DEFECTCODE";



                objCmd.Connection = objCn;
                objCmd.CommandText = sqlString;
                objCmd.CommandType = System.Data.CommandType.Text;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                objAdr.Fill(objDS4);
                return objDS4.Tables[0];

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
    //public DataTable GetDPRStatusCount(String Date, String SiteID, string Shift, String Line)
    //{


    //    Date = string.IsNullOrEmpty(Date) ? "null" : Date;
    //    SiteID = string.IsNullOrEmpty(SiteID) ? "null" : SiteID;
    //    Shift = string.IsNullOrEmpty(Shift) ? "null" : Shift;
    //    Line = string.IsNullOrEmpty(Line) ? "null" : Line;

    //    using (objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strConn;
    //        try
    //        {
    //            objCn.Open();
    //            objCmd = new OracleCommand();


    //            string sqlString = "SELECT * " +
    //            "          FROM (SELECT NVL(D.CNT, 0) CNT, 'DIRECT PASS' AS PASS_TYPE, 'B' SC" +
    //            "                  FROM DUAL," +
    //            "                  (SELECT COUNT(CHASSIS_NUMBER) AS CNT, PASS_TYPE" +
    //            "                              FROM (SELECT A.CHASSIS_NUMBER," +
    //            "                                           'DIRECT PASS' AS PASS_TYPE," +
    //            "                                           " + funGetShift + "(TO_CHAR(TO_DATE('" + Date + "')," +
    //            "                                                                      'YYYYMMDD')," +
    //            "                                                              A.PASS_TIME," +
    //            "                                                              TO_CHAR(A.PASS_DATE," +
    //            "                                                                      'YYYYMMDD')," +
    //            "                                                              " + SiteID + ") ST_CODE" +
    //            "                                      FROM " + tblPrd_VQMS_Header + " A" +
    //            "                                     WHERE ((" + Line + " IS NOT NULL AND" +
    //            "                                           A.LINE = " + Line + ") OR" +
    //            "                                           (" + Line + " IS NULL AND 1 = 1))" +
    //            "                                       AND (A.PASS_DATE BETWEEN DATE_IN AND" +
    //            "                                           TO_CHAR(TO_DATE('" + Date + "') + 1))" +
    //            "                                       AND A.PASS_TYPE = 'D')" +
    //            "                             WHERE ((" + Shift + " IS NULL AND" +
    //            "                                   (ST_CODE = 'A' or ST_CODE = 'B')) OR" +
    //            "                                   (" + Shift + " IS NOT NULL AND " + Shift + " = 'A' AND" +
    //            "                                   ST_CODE = 'A') OR" +
    //            "                                   (" + Shift + " IS NOT NULL AND " + Shift + " = 'B' AND" +
    //            "                                   (ST_CODE = 'B')))" +
    //            "" +
    //            "                            ) D" +
    //            "                    where PASS_TYPE (+) = D.PASS_TYPE" +
    //            "" +
    //            "                UNION" +
    //            "                SELECT NVL(D.CNT, 0) CNT," +
    //            "                       'STRAIGHT PASS' AS PASS_TYPE," +
    //            "                       'C' SC" +
    //            "                  FROM DUAL," +
    //            "                  (SELECT COUNT(CHASSIS_NUMBER) AS CNT, PASS_TYPE" +
    //            "                               FROM (SELECT A.CHASSIS_NUMBER," +
    //            "                                            'STRAIGHT PASS' AS PASS_TYPE," +
    //            "                                            " + funGetShift + "(TO_CHAR(TO_DATE(DATE_IN)," +
    //            "                                                                       'YYYYMMDD')," +
    //            "                                                               A.PASS_TIME," +
    //            "                                                               TO_CHAR(A.PASS_DATE," +
    //            "                                                                       'YYYYMMDD')," +
    //            "                                                               " + SiteID + ") ST_CODE" +
    //            "                                       FROM " + tblPrd_VQMS_Header + " A" +
    //            "                                      WHERE ((" + Line + " IS NOT NULL AND" +
    //            "                                            A.LINE = " + Line + ") OR" +
    //            "                                            (" + Line + " IS NULL AND 1 = 1))" +
    //            "                                        AND (A.PASS_DATE BETWEEN '" + Date + "' AND" +
    //            "                                            TO_CHAR(TO_DATE('" + Date + "') + 1))" +
    //            "                                        AND A.PASS_TYPE <> 'F')" +
    //            "                              WHERE ((" + Shift + " IS NULL AND" +
    //            "                                    (ST_CODE = 'A' or ST_CODE = 'B')) OR" +
    //            "                                    (" + Shift + " IS NOT NULL AND " + Shift + " = 'A' AND" +
    //            "                                    ST_CODE = 'A') OR" +
    //            "                                    (" + Shift + " IS NOT NULL AND " + Shift + " = 'B' AND" +
    //            "                                    (ST_CODE = 'B')))" +
    //            "" +
    //            "                             ) D" +
    //            "                    where PASS_TYPE (+) = D.PASS_TYPE" +
    //            "" +
    //            "                UNION" +
    //            "" +
    //            "                SELECT NVL(D.CNT, 0) CNT, 'DEFECTIVE' AS PASS_TYPE, 'D' SC" +
    //            "                  FROM DUAL," +
    //            "                  (SELECT COUNT(CHASSIS_NUMBER) AS CNT, PASS_TYPE" +
    //            "                               FROM (SELECT A.CHASSIS_NUMBER," +
    //            "                                            'DEFECTIVE' AS PASS_TYPE," +
    //            "                                            " + funGetShift + "(TO_CHAR(TO_DATE(DATE_IN)," +
    //            "                                                                       'YYYYMMDD')," +
    //            "                                                               A.PASS_TIME," +
    //            "                                                               TO_CHAR(A.PASS_DATE," +
    //            "                                                                       'YYYYMMDD')," +
    //            "                                                               " + SiteID + ") ST_CODE" +
    //            "                                       FROM " + tblPrd_VQMS_Header + " A" +
    //            "                                      WHERE ((" + Line + " IS NOT NULL AND" +
    //            "                                            A.LINE = " + Line + ") OR" +
    //            "                                            (" + Line + " IS NULL AND 1 = 1))" +
    //            "                                        AND (A.PASS_DATE BETWEEN " + Date + " AND" +
    //            "                                            TO_CHAR(TO_DATE(" + Date + ") + 1))" +
    //            "                                        AND A.PASS_TYPE = 'F')" +
    //            "                              WHERE ((" + Shift + " IS NULL AND" +
    //            "                                    (ST_CODE = 'A' or ST_CODE = 'B')) OR" +
    //            "                                    (" + Shift + " IS NOT NULL AND " + Shift + " = 'A' AND" +
    //            "                                    ST_CODE = 'A') OR" +
    //            "                                    (" + Shift + " IS NOT NULL AND " + Shift + " = 'B' AND" +
    //            "                                    (ST_CODE = 'B')))) D" +
    //            "                    where PASS_TYPE (+) = D.PASS_TYPE" +
    //            "" +
    //            "                UNION" +
    //            "" +
    //            "                SELECT NVL(SUM(PRODUCTION_QTY), 0) PRODUCTION_QTY," +
    //            "                       'TOTAL PRODUCTION' as PASSTYPE," +
    //            "                       'A' SC" +
    //            "                  FROM (SELECT '1' JC, COUNT(*) AS PRODUCTION_QTY" +
    //            "                          FROM (SELECT K.AF_DAT," +
    //            "                                       " + funGetShift + "(TO_CHAR(TO_DATE(DATE_IN)," +
    //            "                                                                  'YYYYMMDD')," +
    //            "                                                          to_char(K.AF_TIM," +
    //            "                                                                  'HH24MISS')," +
    //            "                                                          TO_NUMBER(K.AF_DAT)," +
    //            "                                                          " + SiteID + ") ST_CODE" +
    //            "                                  FROM " + tblPrd005 + "           K," +
    //            "                                       " + tblModelWiseMapping + " M" +
    //            "                                 WHERE 1 = 1" +
    //            "                                   AND (K.MTOCD) = (M.MTOCODE)" +
    //            "                                   AND ((" + Line + " IS NOT NULL AND" +
    //            "                                       K.PRD_LIN = " + Line + ") OR" +
    //            "                                       (" + Line + " IS NULL AND 1 = 1))" +
    //            "                                   AND TO_CHAR(K.AF_DAT) BETWEEN" +
    //            "                                       TO_CHAR(TO_DATE('" + Date + "'), 'YYYYMMDD') AND" +
    //            "                                       TO_CHAR(TO_DATE('" + Date + "') + 1," +
    //            "                                               'YYYYMMDD'))" +
    //            "                         WHERE ((" + Shift + " IS NULL AND" +
    //            "                               (ST_CODE = 'A' or ST_CODE = 'B')) OR" +
    //            "                               (" + Shift + " IS NOT NULL AND " + Shift + " = 'A' AND" +
    //            "                               ST_CODE = 'A') OR" +
    //            "                               (" + Shift + " IS NOT NULL AND " + Shift + " = 'B' AND" +
    //            "                               (ST_CODE = 'B')))) P" +
    //            "" +
    //            "                UNION" +
    //            "" +
    //            "                SELECT COUNT(*) Not_Entered_Frames, PASSTYPE, SC" +
    //            "                  FROM (SELECT A.CHASSIS_NUMBER," +
    //            "                               " + funGetShift + "(TO_CHAR(TO_DATE(DATE_IN)," +
    //            "                                                          'YYYYMMDD')," +
    //            "                                                  A.PASS_TIME," +
    //            "                                                  TO_CHAR(A.PASS_DATE," +
    //            "                                                          'YYYYMMDD')," +
    //            "                                                  " + SiteID + ") ST_CODE," +
    //            "                               'DEFECT NOT ENTERED' as PASSTYPE," +
    //            "                               'E' SC" +
    //            "                          FROM " + tblPrd_VQMS_Header + " A" +
    //            "                         WHERE NOT EXISTS" +
    //            "                         (SELECT MODELCODE" +
    //            "                                  FROM " + tblFrameWiseDefect + " B" +
    //            "                                 WHERE B.ENG_FRAME_TYPE = A.ENG_FRAME_TYPE" +
    //            "                                   AND A.CHASSIS_NUMBER = CHASSIS_NUMBER)" +
    //            "                           AND A.PASS_TYPE <> 'D'" +
    //            "                           AND (A.PASS_DATE BETWEEN '" + Date + "' AND" +
    //            "                               TO_CHAR(TO_DATE('" + Date + "') + 1))" +
    //            "                           AND ((" + Line + " IS NOT NULL AND A.LINE = " + Line + ") OR" +
    //            "                               (" + Line + " IS NULL AND 1 = 1)))" +
    //            "                 WHERE ((" + Shift + " IS NULL AND" +
    //            "                       (ST_CODE = 'A' or ST_CODE = 'B')) OR" +
    //            "                       (" + Shift + " IS NOT NULL AND " + Shift + " = 'A' AND" +
    //            "                       ST_CODE = 'A') OR" +
    //            "                       (" + Shift + " IS NOT NULL AND " + Shift + " = 'B' AND" +
    //            "                       (ST_CODE = 'B')))" +
    //            "" +
    //            "                )" +
    //            "         ORDER BY SC";







    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = sqlString;
    //            objCmd.CommandType = System.Data.CommandType.Text;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            objAdr.Fill(objDS4);
    //            return objDS4.Tables[0];
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
    //public DataTable GetDPRStatusPercentage(String Date, String SiteID, string Shift, String Line)
    //{

    //    Date = string.IsNullOrEmpty(Date) ? DBNull.Value.ToString() : Date;
    //    SiteID = string.IsNullOrEmpty(SiteID) ? DBNull.Value.ToString() : SiteID;
    //    Shift = string.IsNullOrEmpty(Shift) ? DBNull.Value.ToString() : Shift;
    //    Line = string.IsNullOrEmpty(Line) ? DBNull.Value.ToString() : Line;

    //    using (objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strConn;
    //        try
    //        {
    //            objCn.Open();
    //            objCmd = new OracleCommand();


    //            string sqlString = "SELECT TMP.PASS_TYPE," +
    //            "               ROUND((CNT / DECODE(TCNT, 0, 1, TCNT)) * 100, 1) AS PER" +
    //            "          FROM (SELECT NVL(CNT, 0) CNT, PASS_TYPE, '1' as I" +
    //            "                  FROM (SELECT NVL(D.CNT, 0) CNT, 'DIRECT PASS' AS PASS_TYPE" +
    //            "                          FROM DUAL," +
    //            "                          (SELECT COUNT(CHASSIS_NUMBER) AS CNT," +
    //            "                                           PASS_TYPE" +
    //            "                                      FROM (SELECT A.CHASSIS_NUMBER," +
    //            "                                                   'DIRECT PASS' AS PASS_TYPE," +
    //            "                                                   " + funGetShift + "(TO_CHAR(TO_DATE(DATE_IN)," +
    //            "                                                                              'YYYYMMDD')," +
    //            "                                                                      A.PASS_TIME," +
    //            "                                                                      TO_CHAR(A.PASS_DATE," +
    //            "                                                                              'YYYYMMDD')," +
    //            "                                                                      " + SiteID + ") ST_CODE" +
    //            "                                              FROM " + tblPrd_VQMS_Header + " A" +
    //            "                                             WHERE ((" + Line + " IS NOT NULL AND" +
    //            "                                                   A.LINE = " + Line + ") OR" +
    //            "                                                   (" + Line + " IS NULL AND 1 = 1))" +
    //            "                                               AND (A.PASS_DATE BETWEEN" +
    //            "                                                   '" + Date + "' AND" +
    //            "                                                   TO_CHAR(TO_DATE('" + Date + "') + 1))" +
    //            "                                               AND A.PASS_TYPE = 'D')" +
    //            "                                     WHERE ((" + Shift + " IS NULL AND" +
    //            "                                           (ST_CODE = 'A' or ST_CODE = 'B')) OR" +
    //            "                                           (" + Shift + " IS NOT NULL AND" +
    //            "                                           " + Shift + " = 'A' AND ST_CODE = 'A') OR" +
    //            "                                           (" + Shift + " IS NOT NULL AND" +
    //            "                                           " + Shift + " = 'B' AND" +
    //            "                                           (ST_CODE = 'B')))" +
    //            "" +
    //            "                                    ) D" +
    //            "                            where PASS_TYPE (+) = D.PASS_TYPE" +
    //            "" +
    //            "                        UNION" +
    //            "                        SELECT NVL(D.CNT, 0) CNT," +
    //            "                               'STRAIGHT PASS' AS PASS_TYPE" +
    //            "                          FROM DUAL," +
    //            "                          (SELECT COUNT(CHASSIS_NUMBER) AS CNT," +
    //            "                                            PASS_TYPE" +
    //            "                                       FROM (SELECT A.CHASSIS_NUMBER," +
    //            "                                                    'STRAIGHT PASS' AS PASS_TYPE," +
    //            "                                                    " + funGetShift + "(TO_CHAR(TO_DATE(DATE_IN)," +
    //            "                                                                               'YYYYMMDD')," +
    //            "                                                                       A.PASS_TIME," +
    //            "                                                                       TO_CHAR(A.PASS_DATE," +
    //            "                                                                               'YYYYMMDD')," +
    //            "                                                                       " + SiteID + ") ST_CODE" +
    //            "                                               FROM " + tblPrd_VQMS_Header + " A" +
    //            "                                              WHERE ((" + Line + " IS NOT NULL AND" +
    //            "                                                    A.LINE = " + Line + ") OR" +
    //            "                                                    (" + Line + " IS NULL AND 1 = 1))" +
    //            "                                                AND (A.PASS_DATE BETWEEN" +
    //            "                                                    '" + Date + "' AND" +
    //            "                                                    TO_CHAR(TO_DATE('" + Date + "') + 1))" +
    //            "                                                AND A.PASS_TYPE <> 'F')" +
    //            "                                      WHERE ((" + Shift + " IS NULL AND" +
    //            "                                            (ST_CODE = 'A' or ST_CODE = 'B')) OR" +
    //            "                                            (" + Shift + " IS NOT NULL AND" +
    //            "                                            " + Shift + " = 'A' AND ST_CODE = 'A') OR" +
    //            "                                            (" + Shift + " IS NOT NULL AND" +
    //            "                                            " + Shift + " = 'B' AND" +
    //            "                                            (ST_CODE = 'B')))" +
    //            "" +
    //            "                                     ) D" +
    //            "                            where PASS_TYPE (+) = D.PASS_TYPE" +
    //            "" +
    //            "                        UNION" +
    //            "                        SELECT NVL(D.CNT, 0) CNT, 'DEFECTIVE' AS PASS_TYPE" +
    //            "                          FROM DUAL," +
    //            "                          (SELECT COUNT(CHASSIS_NUMBER) AS CNT," +
    //            "                                            PASS_TYPE" +
    //            "                                       FROM (SELECT A.CHASSIS_NUMBER," +
    //            "                                                    'DEFECTIVE' AS PASS_TYPE," +
    //            "                                                    " + funGetShift + "(TO_CHAR(TO_DATE(DATE_IN)," +
    //            "                                                                               'YYYYMMDD')," +
    //            "                                                                       A.PASS_TIME," +
    //            "                                                                       TO_CHAR(A.PASS_DATE," +
    //            "                                                                               'YYYYMMDD')," +
    //            "                                                                       " + SiteID + ") ST_CODE" +
    //            "                                               FROM " + tblPrd_VQMS_Header + " A" +
    //            "                                              WHERE ((" + Line + " IS NOT NULL AND" +
    //            "                                                    A.LINE = " + Line + ") OR" +
    //            "                                                    (" + Line + " IS NULL AND 1 = 1))" +
    //            "                                                AND (A.PASS_DATE BETWEEN" +
    //            "                                                    '" + Date + "' AND" +
    //            "                                                    TO_CHAR(TO_DATE('" + Date + "') + 1))" +
    //            "                                                AND A.PASS_TYPE = 'F')" +
    //            "                                      WHERE ((" + Shift + " IS NULL AND" +
    //            "                                            (ST_CODE = 'A' or ST_CODE = 'B')) OR" +
    //            "                                            (" + Shift + " IS NOT NULL AND" +
    //            "                                            " + Shift + " = 'A' AND ST_CODE = 'A') OR" +
    //            "                                            (" + Shift + " IS NOT NULL AND" +
    //            "                                            " + Shift + " = 'B' AND" +
    //            "                                            (ST_CODE = 'B')))) D" +
    //            "                            where PASS_TYPE (+) = D.PASS_TYPE)) TMP" +
    //            "          , (SELECT COUNT(CHASSIS_NUMBER) AS TCNT, '1' as I" +
    //            "                  FROM (select A.CHASSIS_NUMBER," +
    //            "                               " + funGetShift + "(TO_CHAR(TO_DATE(DATE_IN)," +
    //            "                                                          'YYYYMMDD')," +
    //            "                                                  A.PASS_TIME," +
    //            "                                                  TO_CHAR(A.PASS_DATE," +
    //            "                                                          'YYYYMMDD')," +
    //            "                                                  " + SiteID + ") ST_CODE" +
    //            "                          FROM " + tblPrd_VQMS_Header + " A" +
    //            "                         WHERE ((" + Line + " IS NOT NULL AND A.LINE = " + Line + ") OR" +
    //            "                               (" + Line + " IS NULL AND 1 = 1))" +
    //            "                           AND (A.PASS_DATE BETWEEN '" + Date + "' AND" +
    //            "                               TO_CHAR(TO_DATE('" + Date + "') + 1)))" +
    //            "                 WHERE (ST_CODE = 'A' or ST_CODE = 'B')) tot" +
    //            "            where TMP.I = tot.I;";



    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = sqlString;
    //            objCmd.CommandType = System.Data.CommandType.Text;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            objAdr.Fill(objDS4);
    //            return objDS4.Tables[0];
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
    //public DataTable GetSectionWiseDefect(String Date, String SiteID, string Shift, String Line)
    //{
    //    Date = string.IsNullOrEmpty(Date) ? DBNull.Value.ToString() : Date;
    //    SiteID = string.IsNullOrEmpty(SiteID) ? DBNull.Value.ToString() : SiteID;
    //    Shift = string.IsNullOrEmpty(Shift) ? DBNull.Value.ToString() : Shift;
    //    Line = string.IsNullOrEmpty(Line) ? DBNull.Value.ToString() : Line;

    //    using (objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strConn;
    //        try
    //        {
    //            objCn.Open();
    //            objCmd = new OracleCommand();


    //            string sqlString = "SELECT DEFECTSECTION," +
    //            "               CNT," +
    //            "               SUM(CNT) OVER(ORDER BY CNT DESC ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) \"Running total\"," +
    //            "               '0' AS JC," +
    //            "               ROUND((SUM(CNT)" +
    //            "                      OVER(ORDER BY CNT DESC ROWS BETWEEN UNBOUNDED" +
    //            "                           PRECEDING AND CURRENT ROW) / TOTAL.TC) * 100," +
    //            "                     2) cm" +
    //            "          FROM (SELECT COUNT(CHASSIS_NUMBER) CNT, DEFECTSECTION" +
    //            "                  FROM (SELECT A.DEFECTSECTION," +
    //            "                               a.CHASSIS_NUMBER," +
    //            "                               " + funGetShift + "(TO_CHAR(TO_DATE(DATE_IN)," +
    //            "                                                          'YYYYMMDD')," +
    //            "                                                  H.PASS_TIME," +
    //            "                                                  TO_CHAR(H.PASS_DATE," +
    //            "                                                          'YYYYMMDD')," +
    //            "                                                  " + SiteID + ") ST_CODE" +
    //            "                          FROM " + tblFrameWiseDefect + " A," +
    //            "                               " + tblPrd_VQMS_Header + " H," +
    //            "                               " + tblCodeType + "         F" +
    //            "                         WHERE 1 = 1" +
    //            "                           AND (H.ENG_FRAME_TYPE) = (A.ENG_FRAME_TYPE)" +
    //            "                           AND (H.CHASSIS_NUMBER) = (A.CHASSIS_NUMBER)" +
    //            "                           AND (A.DEFECTSECTION) = (F.SUBCODE)" +
    //            "                           AND UPPER(F.CODE) = 'SECTION'" +
    //            "                           AND (('" + Line + "' IS NOT NULL AND H.LINE = '" + Line + "') OR" +
    //            "                               ('" + Line + "' IS NULL AND 1 = 1))" +
    //            "                           AND (H.PASS_DATE BETWEEN '" + Date + "' AND" +
    //            "                               TO_CHAR(TO_DATE('" + Date + "') + 1)))" +
    //            "                 WHERE (('" + Shift + "' IS NULL AND" +
    //            "                       (ST_CODE = 'A' or ST_CODE = 'B')) OR" +
    //            "                       ('" + Shift + "' IS NOT NULL AND '" + Shift + "' = 'A' AND" +
    //            "                       ST_CODE = 'A') OR" +
    //            "                       ('" + Shift + "' IS NOT NULL AND '" + Shift + "' = 'B' AND" +
    //            "                       (ST_CODE = 'B')))" +
    //            "                 GROUP BY DEFECTSECTION) MD" +
    //            "          , (" +
    //            "" +
    //            "                     SELECT COUNT(CHASSIS_NUMBER) TC, '0' AS JC" +
    //            "                       FROM (SELECT a.CHASSIS_NUMBER," +
    //            "                                     " + funGetShift + "(TO_CHAR(TO_DATE(DATE_IN)," +
    //            "                                                                'YYYYMMDD')," +
    //            "                                                        H.PASS_TIME," +
    //            "                                                        TO_CHAR(H.PASS_DATE," +
    //            "                                                                'YYYYMMDD')," +
    //            "                                                        SITEID_IN) ST_CODE" +
    //            "                                FROM " + tblFrameWiseDefect + " A," +
    //            "                                     " + tblPrd_VQMS_Header + " H," +
    //            "                                     " + tblCodeType + "         F" +
    //            "                              --JOIN HONDA.prd005@PRD PP ON (PP.EF_TYP) = (A.ENG_FRAME_TYPE) AND (PP.SHA_NO) = (A.CHASSIS_NUMBER)" +
    //            "                               WHERE 1 = 1" +
    //            "                                 AND (H.ENG_FRAME_TYPE) = (A.ENG_FRAME_TYPE)" +
    //            "                                 AND (H.CHASSIS_NUMBER) = (A.CHASSIS_NUMBER)" +
    //            "                                 AND (A.DEFECTSECTION) = (F.SUBCODE)" +
    //            "                                 AND UPPER(F.CODE) = 'SECTION'" +
    //            "                                 AND (('" + Line + "' IS NOT NULL AND H.LINE = '" + Line + "') OR" +
    //            "                                     ('" + Line + "' IS NULL AND 1 = 1))" +
    //            "                                 AND (H.PASS_DATE BETWEEN '" + Date + "' AND" +
    //            "                                     TO_CHAR(TO_DATE('" + Date + "') + 1)))" +
    //            "                      WHERE (('" + Shift + "' IS NULL AND" +
    //            "                            (ST_CODE = 'A' or ST_CODE = 'B')) OR" +
    //            "                            ('" + Shift + "' IS NOT NULL AND '" + Shift + "' = 'A' AND" +
    //            "                            ST_CODE = 'A') OR" +
    //            "                            ('" + Shift + "' IS NOT NULL AND '" + Shift + "' = 'B' AND" +
    //            "                            (ST_CODE = 'B')))) TOTAL" +
    //            "            where TOTAL.JC = JC;";



    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = sqlString;
    //            objCmd.CommandType = System.Data.CommandType.Text;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            objAdr.Fill(objDS4);
    //            return objDS4.Tables[0];
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
    public List<string> GetMisReportUser()
    {
        //CommonFunctions objCommon = new CommonFunctions();
        String _AdminUsers = objCommon.GetParameterValue("USERS_MIS_ROOT_MENU");
        String[] _users = _AdminUsers.Split(',');
        List<string> u = new List<string>();
        u.AddRange(_users);
        return u;
    }
    #endregion

    #region Final Inspection Report

    //public DataTable GetReportHeaderDetails(String fromDate, String toDate, String MTOMAPCODE, String strModel, String strSiteID)
    //{
    //    using (objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strConn;
    //        try
    //        {
    //            objCn.Open();
    //            objCmd = new OracleCommand();

    //            StringBuilder sqlStrBuild = new StringBuilder();

    //            sqlStrBuild.Append(" SELECT * FROM   ");
    //            sqlStrBuild.Append("           (    ");
    //            sqlStrBuild.Append("            (SELECT '1'JC,  ");
    //            sqlStrBuild.Append("              NVL(SUM(PRODUCTION_QTY_A),0)PRODUCTION_QTY_A,NVL(SUM(PRODUCTION_QTY_B),0)PRODUCTION_QTY_B, ");
    //            sqlStrBuild.Append("              NVL(SUM(PRODUCTION_QTY),0)PRODUCTION_QTY ");
    //            sqlStrBuild.Append("              FROM                                      ");
    //            sqlStrBuild.Append("              (                                        ");
    //            sqlStrBuild.Append("                    SELECT                             ");
    //            sqlStrBuild.Append("                        CASE WHEN ST_CODE = 'A' THEN COUNT(*) END PRODUCTION_QTY_A, ");
    //            sqlStrBuild.Append("                        CASE WHEN ST_CODE= 'B' THEN COUNT(*) END PRODUCTION_QTY_B, ");
    //            sqlStrBuild.Append("                        COUNT(*) AS PRODUCTION_QTY                                 ");
    //            sqlStrBuild.Append("                     FROM                                                          ");
    //            sqlStrBuild.Append("                     (                                                             ");
    //            sqlStrBuild.Append("                         SELECT K.AF_DAT,                                          ");
    //            sqlStrBuild.Append("                                " + funGetShift + "(TO_CHAR(TO_DATE('" + fromDate + "'),'YYYYMMDD'),to_char(K.AF_TIM,'HH24MISS'),TO_NUMBER(K.AF_DAT)," + strSiteID + ") ST_CODE  ");
    //            sqlStrBuild.Append("                            FROM " + tblPrd005 + "  K     ");
    //            sqlStrBuild.Append("                             ," + tblModelWiseMapping + " M ");
    //            sqlStrBuild.Append("                          WHERE K.MTOCD = M.MTOCODE          ");
    //            sqlStrBuild.Append("                             AND TO_CHAR(K.AF_DAT) BETWEEN TO_CHAR(TO_DATE('" + fromDate + "'),'YYYYMMDD') AND TO_CHAR(TO_DATE('" + fromDate + "')+1,'YYYYMMDD')   ");
    //            sqlStrBuild.Append("                             AND M.MTOMAPCODE = '" + strModel + "'");
    //            sqlStrBuild.Append("                     )                                         ");
    //            sqlStrBuild.Append("                     WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C')     ");
    //            sqlStrBuild.Append("                     GROUP BY ST_CODE                                       ");
    //            sqlStrBuild.Append("               ))P                                                          ");
    //            sqlStrBuild.Append("          )                                                                 ");
    //            sqlStrBuild.Append("           LEFT OUTER JOIN                                                  ");
    //            sqlStrBuild.Append("           (                                                                                                                                                                  ");
    //            sqlStrBuild.Append("                SELECT '1' AS JC,                                                                                                                                             ");
    //            sqlStrBuild.Append("                NVL(SUM(VQ_QTY_A),0)VQ_QTY_A,NVL(SUM(VQ_QTY_B),0)VQ_QTY_B,                                                                                                    ");
    //            sqlStrBuild.Append("                NVL(SUM(VQ_QTY),0)VQ_QTY                                                                                                                                      ");
    //            sqlStrBuild.Append("                FROM                                                                                                                                                          ");
    //            sqlStrBuild.Append("                (                                                                                                                                                             ");
    //            sqlStrBuild.Append("                        SELECT                                                                                                                                                ");
    //            sqlStrBuild.Append("                              CASE WHEN ST_CODE = 'A' THEN COUNT(*) END VQ_QTY_A,                                                                                             ");
    //            sqlStrBuild.Append("                              CASE WHEN ST_CODE= 'B'  THEN COUNT(*) END VQ_QTY_B,                                                                                             ");
    //            sqlStrBuild.Append("                              COUNT(*) AS VQ_QTY                                                                                                                              ");
    //            sqlStrBuild.Append("                           FROM                                                                                                                                               ");
    //            sqlStrBuild.Append("                           (                                                                                                                                                  ");
    //            sqlStrBuild.Append("                               SELECT K.AF_DAT,                                                                                                                               ");
    //            sqlStrBuild.Append("                                      " + funGetShift + "(TO_CHAR(TO_DATE('" + fromDate + "'),'YYYYMMDD'),to_char(K.RE_VQ_TIM,'HH24MISS'),TO_NUMBER(K.RE_VQ_DAT)," + strSiteID + ") ST_CODE   ");
    //            sqlStrBuild.Append("                                  FROM " + tblPrd005 + "  K                                                                                                                   ");
    //            sqlStrBuild.Append("                                  , " + tblModelWiseMapping + " M                                                                                                             ");
    //            sqlStrBuild.Append("                                  WHERE K.MTOCD = M.MTOCODE                                                                                                                   ");
    //            sqlStrBuild.Append("                                   AND TO_CHAR(K.RE_VQ_DAT) BETWEEN TO_CHAR(TO_DATE('" + fromDate + "'),'YYYYMMDD') AND TO_CHAR(TO_DATE('" + fromDate + "')+1,'YYYYMMDD')                   ");
    //            sqlStrBuild.Append("                                   AND M.MTOMAPCODE = '" + strModel + "'                                                                                                          ");
    //            sqlStrBuild.Append("                           )                                                                                                                                                  ");
    //            sqlStrBuild.Append("                           WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C')                                                                                                 ");
    //            sqlStrBuild.Append("                           GROUP BY ST_CODE                                                                                                                                   ");
    //            sqlStrBuild.Append("                 )                                                                                                                                                            ");
    //            sqlStrBuild.Append("            ) VQ ON VQ.JC = P.JC                                                                                                                                              ");
    //            sqlStrBuild.Append("           LEFT OUTER JOIN                                                                                                                                                    ");
    //            sqlStrBuild.Append("           (                                                                                                                                                                  ");
    //            sqlStrBuild.Append("                SELECT '1' AS JC,                                                                                                                                             ");
    //            sqlStrBuild.Append("                NVL(SUM(RVQ_QTY_A),0)RVQ_QTY_A,NVL(SUM(RVQ_QTY_B),0)RVQ_QTY_B,                                                                                                ");
    //            sqlStrBuild.Append("                NVL(SUM(RVQ_QTY),0)RVQ_QTY                                                                                                                                    ");
    //            sqlStrBuild.Append("                FROM                                                                                                                                                          ");
    //            sqlStrBuild.Append("                (                                                                                                                                                             ");
    //            sqlStrBuild.Append("                        SELECT                                                                                                                                                ");
    //            sqlStrBuild.Append("                            CASE WHEN ST_CODE = 'A' THEN COUNT(*) END RVQ_QTY_A,                                                                                              ");
    //            sqlStrBuild.Append("                            CASE WHEN ST_CODE= 'B'  THEN COUNT(*) END RVQ_QTY_B,                                                                                              ");
    //            sqlStrBuild.Append("                            COUNT(*) AS RVQ_QTY                                                                                                                               ");
    //            sqlStrBuild.Append("                         FROM                                                                                                                                                 ");
    //            sqlStrBuild.Append("                         (                                                                                                                                                    ");
    //            sqlStrBuild.Append("                             SELECT K.AF_DAT,                                                                                                                                 ");
    //            sqlStrBuild.Append("                                    " + funGetShift + "(TO_CHAR(TO_DATE('" + fromDate + "'),'YYYYMMDD'),to_char(K.RE_VQ_TIM,'HH24MISS'),TO_NUMBER(K.RE_VQ_DAT)," + strSiteID + ") ST_CODE         ");
    //            sqlStrBuild.Append("                                FROM " + tblPrd005 + " K                                                                                                                      ");
    //            sqlStrBuild.Append("                                ," + tblModelWiseMapping + " M                                                                                                                 ");
    //            sqlStrBuild.Append("                                WHERE K.MTOCD = M.MTOCODE                                                                                                                     ");
    //            sqlStrBuild.Append("                                AND TO_CHAR(K.RE_VQ_DAT) BETWEEN TO_CHAR(TO_DATE('" + fromDate + "'),'YYYYMMDD') AND TO_CHAR(TO_DATE('" + fromDate + "')+1,'YYYYMMDD')                              ");
    //            sqlStrBuild.Append("                                AND M.MTOMAPCODE = '" + strModel + "'");
    //            sqlStrBuild.Append("                         )                                                                                                                                                    ");
    //            sqlStrBuild.Append("                         WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C')                                                                                                   ");
    //            sqlStrBuild.Append("                         GROUP BY ST_CODE                                                                                                                                     ");
    //            sqlStrBuild.Append("                 )                                                                                                                                                            ");
    //            sqlStrBuild.Append("           ) RVQ ON RVQ.JC = VQ.JC                                                                                                                                            ");
    //            sqlStrBuild.Append("          LEFT OUTER JOIN                                                                                                                                                     ");
    //            sqlStrBuild.Append("          (                                                                                                                                                                   ");
    //            sqlStrBuild.Append("              SELECT NVL(D.SCNT_A,0) SCNT_A,NVL(D.SCNT_B,0) SCNT_B,'STRAIGHT PASS'AS PASS_TYPE ,'1' AS JC                                                                     ");
    //            sqlStrBuild.Append("               FROM DUAL                                                                                                                                                      ");
    //            sqlStrBuild.Append("               LEFT JOIN                                                                                                                                                      ");
    //            sqlStrBuild.Append("               (                                                                                                                                                              ");
    //            sqlStrBuild.Append("                  SELECT SUM(CNT_A)SCNT_A,SUM(CNT_B)SCNT_B,'STRAIGHT PASS' AS PASS_TYPE                                                                                       ");
    //            sqlStrBuild.Append("                    FROM                                                                                                                                                      ");
    //            sqlStrBuild.Append("                    (                                                                                                                                                         ");
    //            sqlStrBuild.Append("                      SELECT  CASE WHEN ST_CODE = 'A' THEN  COUNT(CHASSIS_NUMBER) END AS CNT_A,                                                                               ");
    //            sqlStrBuild.Append("                              CASE WHEN ST_CODE = 'B' THEN  COUNT(CHASSIS_NUMBER) END AS CNT_B                                                                                ");
    //            sqlStrBuild.Append("                         FROM                                                                                                                                                 ");
    //            sqlStrBuild.Append("                         (                                                                                                                                                    ");
    //            sqlStrBuild.Append("                           SELECT  A.CHASSIS_NUMBER,                                                                                                                          ");
    //            sqlStrBuild.Append("                             " + funGetShift + "(TO_CHAR(TO_DATE('" + fromDate + "'),'YYYYMMDD'),A.PASS_TIME,TO_NUMBER(TO_CHAR(A.PASS_DATE,'YYYYMMDD'))," + strSiteID + ") ST_CODE                ");
    //            sqlStrBuild.Append("                             FROM " + tblPrd_VQMS_Header + " A                                                                                                                 ");
    //            sqlStrBuild.Append("                             , " + tblPrd005 + " PP                                                                                                                            ");
    //            sqlStrBuild.Append("                             , " + tblModelWiseMapping + " M                                                                                                                   ");
    //            sqlStrBuild.Append("                           WHERE                                                                                                                                              ");
    //            sqlStrBuild.Append("                               PP.EF_TYP = A.ENG_FRAME_TYPE AND PP.SHA_NO = A.CHASSIS_NUMBER                                                                                  ");
    //            sqlStrBuild.Append("                            AND M.MTOCODE = PP.MTOCD                                                                                                                          ");
    //            sqlStrBuild.Append("                            AND A.PASS_TYPE <>'F'                                                                                                                             ");
    //            sqlStrBuild.Append("                            AND A.PASS_DATE BETWEEN '" + fromDate + "' AND TO_CHAR(TO_DATE('" + fromDate + "')+1)                                                                                   ");
    //            sqlStrBuild.Append("                             AND M.MTOMAPCODE = '" + strModel + "'");
    //            sqlStrBuild.Append("                        )WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C')                                                                                                   ");
    //            sqlStrBuild.Append("                        GROUP BY ST_CODE                                                                                                                                      ");
    //            sqlStrBuild.Append("                   )                                                                                                                                                          ");
    //            sqlStrBuild.Append("               )D ON PASS_TYPE = D.PASS_TYPE                                                                                                                                  ");
    //            sqlStrBuild.Append("           )SP ON SP.JC = VQ.JC                                                                                                                                               ");
    //            sqlStrBuild.Append("           LEFT OUTER JOIN                                                                                                                                                    ");
    //            sqlStrBuild.Append("           (                                                                                                                                                                  ");
    //            sqlStrBuild.Append("                SELECT NVL(D.DCNT_A,0) DCNT_A,NVL(D.DCNT_B,0) DCNT_B,'DIRECT PASS' AS PASS_TYPE ,'1' AS JC                                                                    ");
    //            sqlStrBuild.Append("                 FROM DUAL                                                                                                                                                    ");
    //            sqlStrBuild.Append("                 LEFT JOIN                                                                                                                                                    ");
    //            sqlStrBuild.Append("                 (                                                                                                                                                            ");
    //            sqlStrBuild.Append("                    SELECT SUM(CNT_A)DCNT_A,SUM(CNT_B)DCNT_B,'DIRECT PASS' AS PASS_TYPE                                                                                       ");
    //            sqlStrBuild.Append("                      FROM                                                                                                                                                    ");
    //            sqlStrBuild.Append("                      (                                                                                                                                                       ");
    //            sqlStrBuild.Append("                         SELECT  CASE WHEN ST_CODE = 'A' THEN  COUNT(CHASSIS_NUMBER) END AS CNT_A,                                                                            ");
    //            sqlStrBuild.Append("                              CASE WHEN ST_CODE = 'B' THEN  COUNT(CHASSIS_NUMBER) END AS CNT_B                                                                                ");
    //            sqlStrBuild.Append("                         FROM                                                                                                                                                 ");
    //            sqlStrBuild.Append("                         (                                                                                                                                                    ");
    //            sqlStrBuild.Append("                           SELECT  A.CHASSIS_NUMBER,                                                                                                                          ");
    //            sqlStrBuild.Append("                             " + funGetShift + "(TO_CHAR(TO_DATE('" + fromDate + "'),'YYYYMMDD'),A.PASS_TIME,TO_NUMBER(TO_CHAR(A.PASS_DATE,'YYYYMMDD'))," + strSiteID + ") ST_CODE                ");
    //            sqlStrBuild.Append("                             FROM " + tblPrd_VQMS_Header + " A");
    //            sqlStrBuild.Append("                             ," + tblPrd005 + " PP                                                                                                                             ");
    //            sqlStrBuild.Append("                             ," + tblModelWiseMapping + " M                                                                                                                    ");
    //            sqlStrBuild.Append("                           WHERE 1=1                                                                                                                                          ");
    //            sqlStrBuild.Append("                            AND PP.EF_TYP = A.ENG_FRAME_TYPE AND PP.SHA_NO = A.CHASSIS_NUMBER                                                                                 ");
    //            sqlStrBuild.Append("                            AND M.MTOCODE = PP.MTOCD                                                                                                                          ");
    //            sqlStrBuild.Append("                            AND A.PASS_TYPE ='D'                                                                                                                              ");
    //            sqlStrBuild.Append("                            AND A.PASS_DATE BETWEEN '" + fromDate + "' AND TO_CHAR(TO_DATE('" + fromDate + "')+1)                                                                                   ");
    //            sqlStrBuild.Append("                            AND M.MTOMAPCODE = '" + strModel + "'");
    //            sqlStrBuild.Append("                        )WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C')                                                                                                   ");
    //            sqlStrBuild.Append("                        GROUP BY ST_CODE                                                                                                                                      ");
    //            sqlStrBuild.Append("                     )                                                                                                                                                        ");
    //            sqlStrBuild.Append("                 )D ON PASS_TYPE = D.PASS_TYPE                                                                                                                                ");
    //            sqlStrBuild.Append("           )DP ON DP.JC = SP.JC                                                                                                                                               ");
    //            sqlStrBuild.Append("           LEFT OUTER JOIN                                                                                                                                                    ");
    //            sqlStrBuild.Append("           (                                                                                                                                                                  ");
    //            sqlStrBuild.Append("             SELECT  COUNT (*) Not_Entered_Frames,JC                                                                                                                          ");
    //            sqlStrBuild.Append("             FROM                                                                                                                                                             ");
    //            sqlStrBuild.Append("             (                                                                                                                                                                ");
    //            sqlStrBuild.Append("                SELECT  A.CHASSIS_NUMBER,                                                                                                                                     ");
    //            sqlStrBuild.Append("                   " + funGetShift + "(TO_CHAR(TO_DATE('" + fromDate + "'),'YYYYMMDD'),A.PASS_TIME,TO_CHAR(A.PASS_DATE,'YYYYMMDD')," + strSiteID + ")                                             ");
    //            sqlStrBuild.Append("                   ST_CODE,'1'JC                                                                                                                                              ");
    //            sqlStrBuild.Append("                 FROM " + tblPrd_VQMS_Header + " A");
    //            sqlStrBuild.Append("                 , " + tblPrd005 + " PP                                                                                                                                        ");
    //            sqlStrBuild.Append("                 , " + tblModelWiseMapping + " M                                                                                                                               ");
    //            sqlStrBuild.Append("                 WHERE                                                                                                                                                        ");
    //            sqlStrBuild.Append("                       PP.EF_TYP = A.ENG_FRAME_TYPE AND PP.SHA_NO = A.CHASSIS_NUMBER                                                                                          ");
    //            sqlStrBuild.Append("                   AND M.MTOCODE = PP.MTOCD                                                                                                                                   ");
    //            sqlStrBuild.Append("                   AND NOT EXISTS(SELECT MODELCODE FROM " + tblFrameWiseDefect + "  B WHERE B.ENG_FRAME_TYPE = A.ENG_FRAME_TYPE                                                ");
    //            sqlStrBuild.Append("                               AND  A.CHASSIS_NUMBER =  CHASSIS_NUMBER )                                                                                                      ");
    //            sqlStrBuild.Append("                   AND  A.PASS_TYPE<>'D'                                                                                                                                      ");
    //            sqlStrBuild.Append("                   AND (A.PASS_DATE BETWEEN '" + fromDate + "' AND TO_CHAR(TO_DATE('" + fromDate + "')+1))                                                                                          ");
    //            sqlStrBuild.Append("                   AND  M.MTOMAPCODE = '" + strModel + "'");
    //            sqlStrBuild.Append("              )WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C')                                                                                                             ");
    //            sqlStrBuild.Append("           )FN ON FN.JC = DP.JC ");

    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = sqlStrBuild.ToString();
    //            objCmd.CommandType = System.Data.CommandType.Text;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            objAdr.Fill(objDS4);
    //            return objDS4.Tables[0];

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

    //public DataTable GetReportHeaderDetailsFN(String fromDate, String toDate, String MTOMAPCODE)
    //{
    //    using (objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strConn;
    //        try
    //        {
    //            objCn.Open();
    //            objCmd = new OracleCommand();

    //            StringBuilder sqlStrBuild = new StringBuilder();

    //            sqlStrBuild.Append(" SELECT  COUNT (*) Not_Entered_Frames ");
    //            sqlStrBuild.Append("       FROM ");
    //            sqlStrBuild.Append("       (");
    //            sqlStrBuild.Append("          SELECT  A.CHASSIS_NUMBER,");
    //            sqlStrBuild.Append("             " + funGetShift + "(TO_CHAR(TO_DATE('" + fromDate + "'),'YYYYMMDD'),A.PASS_TIME,TO_CHAR(A.PASS_DATE,'YYYYMMDD')," + strSiteID + ") ");
    //            sqlStrBuild.Append("             ST_CODE,'1'JC");
    //            sqlStrBuild.Append("           FROM " + tblPrd_VQMS_Header + " A");
    //            sqlStrBuild.Append("           , " + tblPrd005 + " PP  ");
    //            sqlStrBuild.Append("           , " + tblModelWiseMapping + " M ");
    //            sqlStrBuild.Append("           WHERE ");
    //            sqlStrBuild.Append("                 PP.EF_TYP = A.ENG_FRAME_TYPE AND PP.SHA_NO = A.CHASSIS_NUMBER");
    //            sqlStrBuild.Append("             AND M.MTOCODE = PP.MTOCD   ");
    //            sqlStrBuild.Append("             AND NOT EXISTS(SELECT MODELCODE FROM " + tblFrameWiseDefect + "  B WHERE B.ENG_FRAME_TYPE = A.ENG_FRAME_TYPE ");
    //            sqlStrBuild.Append("                         AND  A.CHASSIS_NUMBER =  CHASSIS_NUMBER ) ");
    //            sqlStrBuild.Append("             AND  A.PASS_TYPE<>'D'  ");
    //            sqlStrBuild.Append("             AND (A.PASS_DATE BETWEEN '" + fromDate + "' AND TO_CHAR(TO_DATE('" + fromDate + "')+1)) ");
    //            sqlStrBuild.Append("            AND  M.MTOMAPCODE = '" + MTOMAPCODE + "' ");
    //            sqlStrBuild.Append("        )WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C') ");


    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = sqlStrBuild.ToString();
    //            objCmd.CommandType = System.Data.CommandType.Text;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            objAdr.Fill(objDS4);
    //            return objDS4.Tables[0];

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

    //public DataTable GetReportHeaderDetailsRVQ(String fromDate, String toDate, String MTOMAPCODE)
    //{
    //    //string strMonthYear = DateTime.ParseExact("01-" + Month + "-" + Year, "dd-M-yyyy", null).ToString("MM-yyyy");
    //    //string strNextMonfirstDay = DateTime.ParseExact("01-" + Month + "-" + Year, "dd-M-yyyy", null).AddMonths(1).ToString("dd-MMM-yyyy");
    //    //Month = DateTime.ParseExact("01-" + Month + "-" + Year, "dd-M-yyyy", null).ToString("MM");

    //    using (objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strConn;
    //        try
    //        {
    //            objCn.Open();
    //            objCmd = new OracleCommand();

    //            StringBuilder sqlStrBuild = new StringBuilder();
    //            sqlStrBuild.Append("SELECT NVL(SUM(CASE WHEN ST_CODE = 'A' THEN COUNT(*) END), 0) RVQ_QTY_A, NVL(SUM(CASE WHEN ST_CODE = 'B' THEN COUNT(*) END), 0) ");
    //            sqlStrBuild.Append(" RVQ_QTY_B, NVL(SUM(COUNT(*)), 0) AS RVQ_QTY FROM (SELECT K.AF_DAT, ");
    //            sqlStrBuild.Append(funGetShift);
    //            sqlStrBuild.Append(" (TO_CHAR(TO_DATE('31-Jul-2013'), 'YYYYMMDD'), to_char(K.RE_VQ_TIM, 'HH24MISS'), TO_NUMBER(K.RE_VQ_DAT), 8) ST_CODE FROM ");
    //            sqlStrBuild.Append(tblPrd005);
    //            sqlStrBuild.Append(" K, ");
    //            sqlStrBuild.Append(tblModelWiseMapping);
    //            sqlStrBuild.Append(" M WHERE K.MTOCD = M.MTOCODE AND K.RE_VQ_DAT BETWEEN TO_CHAR(TO_DATE(");
    //            sqlStrBuild.Append(fromDate);
    //            sqlStrBuild.Append(" ), 'YYYYMMDD') AND ");
    //            sqlStrBuild.Append(" TO_CHAR(TO_DATE( ");
    //            sqlStrBuild.Append(toDate);
    //            sqlStrBuild.Append(" ) + 1, 'YYYYMMDD')  AND M.MTOMAPCODE = ");
    //            sqlStrBuild.Append(MTOMAPCODE);
    //            sqlStrBuild.Append(" ) WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C') ");
    //            sqlStrBuild.Append(" GROUP BY ST_CODE ");

    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = sqlStrBuild.ToString();
    //            objCmd.CommandType = System.Data.CommandType.Text;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            objAdr.Fill(objDS4);
    //            return objDS4.Tables[0];

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

    //public DataTable GetReportHeaderDetailsProd(String fromDate, String toDate, String MTOMAPCODE)
    //{
    //    using (objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strConn;
    //        try
    //        {
    //            objCn.Open();
    //            objCmd = new OracleCommand();

    //            StringBuilder sqlStrBuild = new StringBuilder();
    //            sqlStrBuild.Append("SELECT NVL(SUM(CASE WHEN ST_CODE = 'A' THEN COUNT(*) END), 0) PRODUCTION_QTY_A, NVL(SUM(CASE WHEN ST_CODE = 'B' THEN COUNT(*) ");
    //            sqlStrBuild.Append(" END), 0) PRODUCTION_QTY_B, NVL(SUM(COUNT(*)), 0) AS PRODUCTION_QTY FROM (SELECT K.AF_DAT,");
    //            sqlStrBuild.Append(funGetShift);
    //            sqlStrBuild.Append(" (TO_CHAR(TO_DATE( ");
    //            sqlStrBuild.Append(fromDate);
    //            sqlStrBuild.Append(" ),  'YYYYMMDD'), to_char(K.AF_TIM, 'HH24MISS'), TO_NUMBER(K.AF_DAT), 8) ST_CODE FROM ");
    //            sqlStrBuild.Append(tblPrd005);
    //            sqlStrBuild.Append(" K, ");
    //            sqlStrBuild.Append(tblModelWiseMapping);
    //            sqlStrBuild.Append(" M WHERE K.MTOCD = M.MTOCODE  AND K.AF_DAT BETWEEN TO_CHAR(TO_DATE( ");
    //            sqlStrBuild.Append(fromDate);
    //            sqlStrBuild.Append(" ), 'YYYYMMDD') AND ");
    //            sqlStrBuild.Append(" TO_CHAR(TO_DATE( ");
    //            sqlStrBuild.Append(toDate);
    //            sqlStrBuild.Append(" ) + 1, 'YYYYMMDD') AND M.MTOMAPCODE = ");
    //            sqlStrBuild.Append(MTOMAPCODE);
    //            sqlStrBuild.Append(" )  WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C') ");
    //            sqlStrBuild.Append(" GROUP BY ST_CODE ");

    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = sqlStrBuild.ToString();
    //            objCmd.CommandType = System.Data.CommandType.Text;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            objAdr.Fill(objDS4);
    //            return objDS4.Tables[0];

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

    //public DataTable GetReportHeaderDetailsDP(String fromDate, String toDate, String mtomapCode)
    //{
    //    using (objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strConn;
    //        try
    //        {
    //            objCn.Open();
    //            objCmd = new OracleCommand();

    //            StringBuilder sqlStrBuild = new StringBuilder();
    //            sqlStrBuild.Append("SELECT NVL(SUM(CASE WHEN ST_CODE = 'A' THEN COUNT(CHASSIS_NUMBER) END), 0) AS CNT_A, NVL(SUM(CASE WHEN ST_CODE = 'B' THEN ");
    //            sqlStrBuild.Append(" COUNT(CHASSIS_NUMBER) END), 0) AS CNT_B, NVL(SUM(COUNT(*)), 0) AS TOT FROM (SELECT A.CHASSIS_NUMBER, ");
    //            sqlStrBuild.Append(funGetShift);
    //            sqlStrBuild.Append(" (TO_CHAR(TO_DATE( ");
    //            sqlStrBuild.Append(fromDate);
    //            sqlStrBuild.Append(" ), 'YYYYMMDD'), A.PASS_TIME, TO_NUMBER(TO_CHAR(A.PASS_DATE, 'YYYYMMDD')), 8) ST_CODE FROM ");
    //            sqlStrBuild.Append(tblPrd_VQMS_Header);
    //            sqlStrBuild.Append("  A, ");
    //            sqlStrBuild.Append(tblPrd005);
    //            sqlStrBuild.Append(" PP, ");
    //            sqlStrBuild.Append(tblModelWiseMapping);
    //            sqlStrBuild.Append("  M WHERE 1 = 1 AND PP.EF_TYP = A.ENG_FRAME_TYPE AND PP.SHA_NO = A.CHASSIS_NUMBER AND M.MTOCODE = PP.MTOCD AND A.PASS_TYPE = 'D'");
    //            sqlStrBuild.Append(" AND A.PASS_DATE BETWEEN ");
    //            sqlStrBuild.Append(fromDate);
    //            sqlStrBuild.Append(" AND TO_CHAR(TO_DATE( ");
    //            sqlStrBuild.Append(toDate);
    //            sqlStrBuild.Append(" ) + 1) AND M.MTOMAPCODE = ");
    //            sqlStrBuild.Append(mtomapCode);
    //            sqlStrBuild.Append(" ) WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C') GROUP BY ST_CODE ");

    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = sqlStrBuild.ToString();
    //            objCmd.CommandType = System.Data.CommandType.Text;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            objAdr.Fill(objDS4);
    //            return objDS4.Tables[0];

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

    //public DataTable GetReportDefectShopwise(String fromDate, String toDate, String mtomapCode, String category, String section, String strModel, String strSiteID)
    //{
    //    using (objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strConn;
    //        try
    //        {
    //            objCn.Open();
    //            objCmd = new OracleCommand();

    //            StringBuilder sqlStrBuild = new StringBuilder();

    //            sqlStrBuild.Append("SELECT JC,SECTION,A_SHIFT,B_SHIFT,TOTALDEFECTS, ");
    //            sqlStrBuild.Append("    ROUND((SUM(TOTALDEFECTS) OVER (ORDER BY  TOTALDEFECTS DESC) /SUM(TOTALDEFECTS) OVER (PARTITION BY JC) ) *100,2) cm ");
    //            sqlStrBuild.Append("            FROM ");
    //            sqlStrBuild.Append("              ( ");
    //            sqlStrBuild.Append("                    SELECT '1' jc,SUBCODE AS Section,count(A_SHIFT) A_SHIFT,count(B_SHIFT) B_SHIFT,count(A_SHIFT) + count(B_SHIFT) as TotalDefects ");
    //            sqlStrBuild.Append("                      FROM ( ");
    //            sqlStrBuild.Append("                                   SELECT DEFECTDESCRIPTION,DISC,SUBCODE, ");
    //            sqlStrBuild.Append("                                     CASE WHEN ST_CODE = 'A' THEN ENG_FRAME_TYPE END AS A_SHIFT, ");
    //            sqlStrBuild.Append("                                     CASE WHEN ST_CODE = 'B' THEN ENG_FRAME_TYPE END AS B_SHIFT ");
    //            sqlStrBuild.Append("                                     FROM ( ");
    //            sqlStrBuild.Append("                                            SELECT DFM.DEFECTDESCRIPTION,C.DISC,SC.SUBCODE,D.ENG_FRAME_TYPE, ");
    //            sqlStrBuild.Append("                                                   " + funGetShift + "(TO_CHAR(TO_DATE('" + fromDate + "'),'YYYYMMDD'),H.PASS_TIME,TO_NUMBER(TO_CHAR(H.PASS_DATE,'YYYYMMDD'))," + strSiteID + ") ST_CODE ");
    //            sqlStrBuild.Append("                                              FROM " + tblPrd_VQMS_Header + " H, ");
    //            sqlStrBuild.Append("                                                   " + tblFrameWiseDefect + " D, ");
    //            sqlStrBuild.Append("                                                   " + tblDefectMaster + "    DFM, ");
    //            sqlStrBuild.Append("                                                   " + tblCodeType + "         C, ");
    //            sqlStrBuild.Append("                                                   " + tblCodeType + "         SC ");
    //            sqlStrBuild.Append("                                             WHERE 1 = 1 ");
    //            sqlStrBuild.Append("                                               AND H.ENG_FRAME_TYPE = D.ENG_FRAME_TYPE ");
    //            sqlStrBuild.Append("                                               AND H.CHASSIS_NUMBER = D.CHASSIS_NUMBER ");
    //            sqlStrBuild.Append("                                               AND DFM.DEFECTCODE = D.DEFECTCODE ");
    //            sqlStrBuild.Append("                                               AND DFM.MODELCODE = D.MODELCODE ");
    //            sqlStrBuild.Append("                                               AND C.SUBCODE = D.DEFECTCATEGORY ");
    //            sqlStrBuild.Append("                                              AND C.CODE = 'CATEGORY' ");
    //            sqlStrBuild.Append("                                               AND SC.SUBCODE = D.DEFECTSECTION ");
    //            sqlStrBuild.Append("                                               AND SC.CODE = 'SECTION' ");
    //            sqlStrBuild.Append("                                               AND (H.PASS_DATE BETWEEN '" + fromDate + "' AND ");
    //            sqlStrBuild.Append("                                                   TO_CHAR(TO_DATE('" + fromDate + "') + 1)) ");
    //            sqlStrBuild.Append("                                               AND D.MODELCODE = '" + strModel + "'");
    //            sqlStrBuild.Append("                                            ) ");
    //            sqlStrBuild.Append("                              WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C') ");
    //            sqlStrBuild.Append("                           ) ");
    //            sqlStrBuild.Append("                   group by SUBCODE ");
    //            sqlStrBuild.Append("               )        ");


    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = sqlStrBuild.ToString();
    //            objCmd.CommandType = System.Data.CommandType.Text;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            objAdr.Fill(objDS4);
    //            return objDS4.Tables[0];

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

    //public DataTable GetReportDefectShopCnt(String fromDate, String toDate, String mtomapCode, String category, String section)
    //{
    //    using (objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strConn;
    //        try
    //        {
    //            objCn.Open();
    //            objCmd = new OracleCommand();

    //            StringBuilder sqlStrBuild = new StringBuilder();
    //            sqlStrBuild.Append("SELECT '1' jc, SUBCODE AS Section, count(A_SHIFT) A_SHIFT, count(B_SHIFT) B_SHIFT, count(A_SHIFT) + count(B_SHIFT) as TotalDefects ");
    //            sqlStrBuild.Append(" FROM (SELECT DEFECTDESCRIPTION, DISC, SUBCODE, CASE WHEN ST_CODE = 'A' THEN ENG_FRAME_TYPE END AS A_SHIFT, CASE WHEN ST_CODE = 'B' THEN ");
    //            sqlStrBuild.Append(" ENG_FRAME_TYPE END AS B_SHIFT FROM (SELECT DFM.DEFECTDESCRIPTION, C.DISC, SC.SUBCODE, D.ENG_FRAME_TYPE, ");
    //            sqlStrBuild.Append(funGetShift);
    //            sqlStrBuild.Append(" (TO_CHAR(TO_DATE( ");
    //            sqlStrBuild.Append(fromDate);
    //            sqlStrBuild.Append(" ), 'YYYYMMDD'), H.PASS_TIME, TO_NUMBER(TO_CHAR(H.PASS_DATE, 'YYYYMMDD')), 8) ST_CODE FROM ");
    //            sqlStrBuild.Append(tblPrd_VQMS_Header);
    //            sqlStrBuild.Append(" H, ");
    //            sqlStrBuild.Append(tblFrameWiseDefect);
    //            sqlStrBuild.Append(" D, ");
    //            sqlStrBuild.Append(tblDefectMaster);
    //            sqlStrBuild.Append(" DFM, ");
    //            sqlStrBuild.Append(tblCodeType);
    //            sqlStrBuild.Append(" C, ");
    //            sqlStrBuild.Append(tblCodeType);
    //            sqlStrBuild.Append(" SC WHERE 1 = 1 AND H.ENG_FRAME_TYPE = D.ENG_FRAME_TYPE AND H.CHASSIS_NUMBER = D.CHASSIS_NUMBER AND DFM.DEFECTCODE = D.DEFECTCODE ");
    //            sqlStrBuild.Append(" AND DFM.MODELCODE = D.MODELCODE AND C.SUBCODE = D.DEFECTCATEGORY AND C.CODE = ");
    //            sqlStrBuild.Append(category);
    //            sqlStrBuild.Append(" AND SC.SUBCODE = D.DEFECTSECTION ");
    //            sqlStrBuild.Append(" AND SC.CODE = ");
    //            sqlStrBuild.Append(section);
    //            sqlStrBuild.Append(" AND (H.PASS_DATE BETWEEN ");
    //            sqlStrBuild.Append(fromDate);
    //            sqlStrBuild.Append(" AND TO_CHAR(TO_DATE( ");
    //            sqlStrBuild.Append(toDate);
    //            sqlStrBuild.Append(" ) + 1)) AND D.MODELCODE = ");
    //            sqlStrBuild.Append(mtomapCode);
    //            sqlStrBuild.Append(" ) WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C')) group by SUBCODE ");

    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = sqlStrBuild.ToString();
    //            objCmd.CommandType = System.Data.CommandType.Text;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            objAdr.Fill(objDS4);
    //            return objDS4.Tables[0];

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

    //public DataTable GetReportDefectDetails(String fromDate, String toDate, String modelCode, String category, String section)
    //{
    //    using (objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strConn;
    //        try
    //        {
    //            objCn.Open();
    //            objCmd = new OracleCommand();

    //            StringBuilder sqlStrBuild = new StringBuilder();
    //            sqlStrBuild.Append("SELECT DEFECTDESCRIPTION, DISC AS Category, SUBCODE AS Section, count(A_SHIFT) A_SHIFT, count(B_SHIFT) B_SHIFT, ");
    //            sqlStrBuild.Append("count(A_SHIFT) + count(B_SHIFT) as Total FROM (SELECT DEFECTDESCRIPTION, DISC, SUBCODE, CASE WHEN ST_CODE = 'A' ");
    //            sqlStrBuild.Append(" THEN ENG_FRAME_TYPE END AS A_SHIFT, CASE WHEN ST_CODE = 'B' THEN ENG_FRAME_TYPE END AS B_SHIFT FROM (SELECT DFM.DEFECTDESCRIPTION, ");
    //            sqlStrBuild.Append(" C.DISC, SC.SUBCODE, D.ENG_FRAME_TYPE, ");
    //            sqlStrBuild.Append(funGetShift);
    //            sqlStrBuild.Append(" (TO_CHAR(TO_DATE( ");
    //            sqlStrBuild.Append(fromDate);
    //            sqlStrBuild.Append(" ), 'YYYYMMDD'), H.PASS_TIME, TO_NUMBER(TO_CHAR(H.PASS_DATE, 'YYYYMMDD')), '8') ST_CODE FROM ");
    //            sqlStrBuild.Append(tblPrd_VQMS_Header);
    //            sqlStrBuild.Append(" H, ");
    //            sqlStrBuild.Append(tblDefectMaster);
    //            sqlStrBuild.Append(" D, ");
    //            sqlStrBuild.Append(tblDefectMaster);
    //            sqlStrBuild.Append(" DFM, ");
    //            sqlStrBuild.Append(tblCodeType);
    //            sqlStrBuild.Append(" C, ");
    //            sqlStrBuild.Append(tblCodeType);
    //            sqlStrBuild.Append(" SC ");
    //            sqlStrBuild.Append(" WHERE (H.ENG_FRAME_TYPE) = (D.ENG_FRAME_TYPE) AND (H.CHASSIS_NUMBER) = (D.CHASSIS_NUMBER) AND (DFM.DEFECTCODE) = (D.DEFECTCODE) ");
    //            sqlStrBuild.Append(" AND (DFM.MODELCODE) = (D.MODELCODE) AND (C.SUBCODE) = (D.DEFECTCATEGORY) AND C.CODE = ");
    //            sqlStrBuild.Append(category);
    //            sqlStrBuild.Append(" AND (SC.SUBCODE) = (D.DEFECTSECTION) AND SC.CODE = ");
    //            sqlStrBuild.Append(section);
    //            sqlStrBuild.Append(" AND (H.PASS_DATE BETWEEN ");
    //            sqlStrBuild.Append(fromDate);
    //            sqlStrBuild.Append(" AND TO_CHAR(TO_DATE( ");
    //            sqlStrBuild.Append(toDate);
    //            sqlStrBuild.Append(" ) + 1)) AND D.MODELCODE = ");
    //            sqlStrBuild.Append(modelCode);
    //            sqlStrBuild.Append(" )  WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C')) GROUP BY DEFECTDESCRIPTION, DISC, SUBCODE ORDER BY TOTAL DESC ");

    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = sqlStrBuild.ToString();
    //            objCmd.CommandType = System.Data.CommandType.Text;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            objAdr.Fill(objDS4);
    //            return objDS4.Tables[0];

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

    //public DataTable GetReportDefectCategorywise(String fromDate, String toDate, String modelCode, String category, String section, String strModel, String strSiteID)
    //{
    //    using (objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strConn;
    //        try
    //        {
    //            objCn.Open();
    //            objCmd = new OracleCommand();

    //            StringBuilder sqlStrBuild = new StringBuilder();

    //            sqlStrBuild.Append(" SELECT JC,CATEGORY,A_SHIFT,B_SHIFT,TOTALDEFECTS,ROUND((SUM(TOTALDEFECTS) OVER (ORDER BY  TOTALDEFECTS DESC) /SUM(TOTALDEFECTS) OVER (PARTITION BY JC) ) *100,2)cm ");
    //            sqlStrBuild.Append("        FROM ");
    //            sqlStrBuild.Append("        (           ");
    //            sqlStrBuild.Append("          SELECT '1' jc,DISC AS Category,count(A_SHIFT) A_SHIFT,count(B_SHIFT) B_SHIFT,count(A_SHIFT) + count(B_SHIFT) as TotalDefects ");
    //            sqlStrBuild.Append("             FROM ( ");
    //            sqlStrBuild.Append("                   SELECT DEFECTDESCRIPTION,DISC,SUBCODE, ");
    //            sqlStrBuild.Append("                          CASE WHEN ST_CODE = 'A' THEN ENG_FRAME_TYPE END AS A_SHIFT, ");
    //            sqlStrBuild.Append("                          CASE WHEN ST_CODE = 'B' THEN ENG_FRAME_TYPE END AS B_SHIFT ");
    //            sqlStrBuild.Append("                     FROM ( ");
    //            sqlStrBuild.Append("                           SELECT DFM.DEFECTDESCRIPTION, ");
    //            sqlStrBuild.Append("                                  C.DISC, ");
    //            sqlStrBuild.Append("                                  SC.SUBCODE, ");
    //            sqlStrBuild.Append("                                  D.ENG_FRAME_TYPE, ");
    //            sqlStrBuild.Append("                                  " + funGetShift + "(TO_CHAR(TO_DATE('" + fromDate + "'),'YYYYMMDD'),H.PASS_TIME,TO_NUMBER(TO_CHAR(H.PASS_DATE,'YYYYMMDD'))," + strSiteID + ") ST_CODE ");
    //            sqlStrBuild.Append("                             FROM " + tblPrd_VQMS_Header + " H, ");
    //            sqlStrBuild.Append("                                  " + tblFrameWiseDefect + " D, ");
    //            sqlStrBuild.Append("                                  " + tblDefectMaster + "    DFM, ");
    //            sqlStrBuild.Append("                                  " + tblCodeType + "         C, ");
    //            sqlStrBuild.Append("                                  " + tblCodeType + "         SC ");
    //            sqlStrBuild.Append("                            WHERE 1 = 1 ");
    //            sqlStrBuild.Append("                              AND H.ENG_FRAME_TYPE = D.ENG_FRAME_TYPE ");
    //            sqlStrBuild.Append("                              AND H.CHASSIS_NUMBER = D.CHASSIS_NUMBER ");
    //            sqlStrBuild.Append("                              AND DFM.DEFECTCODE = D.DEFECTCODE ");
    //            sqlStrBuild.Append("                              AND DFM.MODELCODE = D.MODELCODE ");
    //            sqlStrBuild.Append("                              AND C.SUBCODE = D.DEFECTCATEGORY ");
    //            sqlStrBuild.Append("                              AND C.CODE = 'CATEGORY' ");
    //            sqlStrBuild.Append("                              AND SC.SUBCODE = D.DEFECTSECTION ");
    //            sqlStrBuild.Append("                              AND SC.CODE = 'SECTION' ");
    //            sqlStrBuild.Append("                              AND (H.PASS_DATE BETWEEN '" + fromDate + "' AND ");
    //            sqlStrBuild.Append("                                  TO_CHAR(TO_DATE('" + fromDate + "') + 1)) ");
    //            sqlStrBuild.Append("                              AND D.MODELCODE = '" + strModel + "' ");
    //            sqlStrBuild.Append("                            ) ");
    //            sqlStrBuild.Append("                    WHERE ST_CODE IS NOT NULL ");
    //            sqlStrBuild.Append("                      AND ST_CODE NOT IN ('C') ");
    //            sqlStrBuild.Append("                 ) ");
    //            sqlStrBuild.Append("            group by DISC ");
    //            sqlStrBuild.Append("        )");

    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = sqlStrBuild.ToString();
    //            objCmd.CommandType = System.Data.CommandType.Text;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            objAdr.Fill(objDS4);
    //            return objDS4.Tables[0];

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

    //public DataTable GetReportDefectCategorywiseCnt(String fromDate, String toDate, String modelCode)
    //{
    //    using (objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strConn;
    //        try
    //        {
    //            objCn.Open();
    //            objCmd = new OracleCommand();

    //            StringBuilder sqlStrBuild = new StringBuilder();
    //            sqlStrBuild.Append(" SELECT '1' jc, DISC AS Category, count(A_SHIFT) A_SHIFT, count(B_SHIFT) B_SHIFT, count(A_SHIFT) + count(B_SHIFT) as TotalDefects ");
    //            sqlStrBuild.Append(" FROM (SELECT DEFECTDESCRIPTION, DISC, SUBCODE, CASE WHEN ST_CODE = 'A' THEN ENG_FRAME_TYPE END AS A_SHIFT, CASE WHEN ST_CODE = 'B' THEN ");
    //            sqlStrBuild.Append(" ENG_FRAME_TYPE END AS B_SHIFT FROM (SELECT DFM.DEFECTDESCRIPTION, C.DISC, SC.SUBCODE, D.ENG_FRAME_TYPE, ");
    //            sqlStrBuild.Append(funGetShift);
    //            sqlStrBuild.Append(" (TO_CHAR(TO_DATE( ");
    //            sqlStrBuild.Append(fromDate);
    //            sqlStrBuild.Append(" ), 'YYYYMMDD'), H.PASS_TIME, TO_NUMBER(TO_CHAR(H.PASS_DATE, 'YYYYMMDD')), 8) ST_CODE FROM ");
    //            sqlStrBuild.Append(tblPrd_VQMS_Header);
    //            sqlStrBuild.Append(" H, ");
    //            sqlStrBuild.Append(tblFrameWiseDefect);
    //            sqlStrBuild.Append(" D, ");
    //            sqlStrBuild.Append(tblDefectMaster);
    //            sqlStrBuild.Append(" DFM, ");
    //            sqlStrBuild.Append(tblCodeType);
    //            sqlStrBuild.Append(" C, ");
    //            sqlStrBuild.Append(tblCodeType);
    //            sqlStrBuild.Append(" SC ");
    //            sqlStrBuild.Append(" WHERE 1 = 1 AND H.ENG_FRAME_TYPE = D.ENG_FRAME_TYPE AND H.CHASSIS_NUMBER = D.CHASSIS_NUMBER AND DFM.DEFECTCODE = D.DEFECTCODE ");
    //            sqlStrBuild.Append(" AND DFM.MODELCODE = D.MODELCODE AND C.SUBCODE = D.DEFECTCATEGORY AND C.CODE = ");
    //            sqlStrBuild.Append(category);
    //            sqlStrBuild.Append(" AND SC.SUBCODE = D.DEFECTSECTION AND SC.CODE = ");
    //            sqlStrBuild.Append(section);
    //            sqlStrBuild.Append(" AND (H.PASS_DATE BETWEEN ");
    //            sqlStrBuild.Append(fromDate);
    //            sqlStrBuild.Append(" AND TO_CHAR(TO_DATE( ");
    //            sqlStrBuild.Append(toDate);
    //            sqlStrBuild.Append(" ) + 1)) AND D.MODELCODE = ");
    //            sqlStrBuild.Append(modelCode);
    //            sqlStrBuild.Append(" ) WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C')) group by DISC ");

    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = sqlStrBuild.ToString();
    //            objCmd.CommandType = System.Data.CommandType.Text;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            objAdr.Fill(objDS4);
    //            return objDS4.Tables[0];

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

    //public DataTable GetReportHeaderDetailsVQ(String fromDate, String toDate, String motomapCode)
    //{
    //    using (objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strConn;
    //        try
    //        {
    //            objCn.Open();
    //            objCmd = new OracleCommand();

    //            StringBuilder sqlStrBuild = new StringBuilder();
    //            sqlStrBuild.Append("SELECT NVL(SUM(CASE WHEN ST_CODE = 'A' THEN COUNT(*) END),0) VQ_QTY_A, NVL(SUM(CASE WHEN ST_CODE= 'B' THEN COUNT(*) END),0) VQ_QTY_B, ");
    //            sqlStrBuild.Append(" NVL(SUM(COUNT(*)),0) AS VQ_QTY FROM ( SELECT K.AF_DAT,  ");
    //            sqlStrBuild.Append(funGetShift);
    //            sqlStrBuild.Append(" (TO_CHAR(TO_DATE( ");
    //            sqlStrBuild.Append(fromDate);
    //            sqlStrBuild.Append(" ),'YYYYMMDD'),to_char(K.RE_VQ_TIM,'HH24MISS'),TO_NUMBER(K.RE_VQ_DAT),8) ST_CODE FROM ");
    //            sqlStrBuild.Append(tblPrd005);
    //            sqlStrBuild.Append(" K, ");
    //            sqlStrBuild.Append(tblModelWiseMapping);
    //            sqlStrBuild.Append(" M ");
    //            sqlStrBuild.Append(" WHERE K.MTOCD = M.MTOCODE AND K.RE_VQ_DAT BETWEEN TO_CHAR(TO_DATE( ");
    //            sqlStrBuild.Append(fromDate);
    //            sqlStrBuild.Append(" ),'YYYYMMDD') AND TO_CHAR(TO_DATE( ");
    //            sqlStrBuild.Append(toDate);
    //            sqlStrBuild.Append(" )+1,'YYYYMMDD') AND M.MTOMAPCODE = ");
    //            sqlStrBuild.Append(motomapCode);
    //            sqlStrBuild.Append(" ) WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C')   GROUP BY ST_CODE ");

    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = sqlStrBuild.ToString();
    //            objCmd.CommandType = System.Data.CommandType.Text;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            objAdr.Fill(objDS4);
    //            return objDS4.Tables[0];

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

    //public DataTable GetReportHeaderDetailsSP(String fromDate, String toDate, String mtomapCode)
    //{
    //    using (objCn = new OracleConnection())
    //    {
    //        objCn.ConnectionString = strConn;
    //        try
    //        {
    //            objCn.Open();
    //            objCmd = new OracleCommand();

    //            StringBuilder sqlStrBuild = new StringBuilder();
    //            sqlStrBuild.Append(" SELECT NVL(SUM(CASE WHEN ST_CODE = 'A' THEN COUNT(CHASSIS_NUMBER) END), 0) AS CNT_A, NVL(SUM(CASE WHEN ST_CODE = 'B' THEN ");
    //            sqlStrBuild.Append(" COUNT(CHASSIS_NUMBER) END), 0) AS CNT_B, NVL(SUM(COUNT(*)), 0) TOT FROM (SELECT A.CHASSIS_NUMBER, ");
    //            sqlStrBuild.Append(funGetShift);
    //            sqlStrBuild.Append(" (TO_CHAR(TO_DATE( ");
    //            sqlStrBuild.Append(fromDate);
    //            sqlStrBuild.Append(" ), 'YYYYMMDD'), A.PASS_TIME, TO_NUMBER(TO_CHAR(A.PASS_DATE, 'YYYYMMDD')), 8) ST_CODE FROM  ");
    //            sqlStrBuild.Append(tblPrd_VQMS_Header);
    //            sqlStrBuild.Append(" A, ");
    //            sqlStrBuild.Append(tblPrd005);
    //            sqlStrBuild.Append(" PP, ");
    //            sqlStrBuild.Append(tblModelWiseMapping);
    //            sqlStrBuild.Append(" M ");
    //            sqlStrBuild.Append(" WHERE PP.EF_TYP = A.ENG_FRAME_TYPE AND PP.SHA_NO = A.CHASSIS_NUMBER AND M.MTOCODE = PP.MTOCD ");
    //            sqlStrBuild.Append(" AND A.PASS_TYPE <> 'F' AND A.PASS_DATE BETWEEN ");
    //            sqlStrBuild.Append(fromDate);
    //            sqlStrBuild.Append(" AND TO_CHAR(TO_DATE( ");
    //            sqlStrBuild.Append(toDate);
    //            sqlStrBuild.Append(" ) + 1) AND M.MTOMAPCODE = ");
    //            sqlStrBuild.Append(mtomapCode);
    //            sqlStrBuild.Append(" ) WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C') GROUP BY ST_CODE ");

    //            objCmd.Connection = objCn;
    //            objCmd.CommandText = sqlStrBuild.ToString();
    //            objCmd.CommandType = System.Data.CommandType.Text;
    //            OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
    //            objAdr.Fill(objDS4);
    //            return objDS4.Tables[0];

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
    #endregion

    #region"FIR"

    public DataTable GetReportHeaderDetails(String strDate, String strModel, String strSiteID)
    {
        DataSet objDS4 = new DataSet();

        string strsql;

        using (objCn = new OracleConnection())
        {
            objCn.ConnectionString = strConn;
            try
            {
                objCn.Open();
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;

                string strSql = string.Empty;


                strSql = strSql + " SELECT * FROM   ";
                strSql = strSql + "           (    ";
                strSql = strSql + "            (SELECT '1'JC,  ";
                strSql = strSql + "              NVL(SUM(PRODUCTION_QTY_A),0)PRODUCTION_QTY_A,NVL(SUM(PRODUCTION_QTY_B),0)PRODUCTION_QTY_B, ";
                strSql = strSql + "              NVL(SUM(PRODUCTION_QTY),0)PRODUCTION_QTY ";
                strSql = strSql + "              FROM                                      ";
                strSql = strSql + "              (                                        ";
                strSql = strSql + "                    SELECT                             ";
                strSql = strSql + "                        CASE WHEN ST_CODE = 'A' THEN COUNT(*) END PRODUCTION_QTY_A, ";
                strSql = strSql + "                        CASE WHEN ST_CODE= 'B' THEN COUNT(*) END PRODUCTION_QTY_B, ";
                strSql = strSql + "                        COUNT(*) AS PRODUCTION_QTY                                 ";
                strSql = strSql + "                     FROM                                                          ";
                strSql = strSql + "                     (                                                             ";
                strSql = strSql + "                         SELECT K.AF_DAT,                                          ";
                strSql = strSql + "                                " + funGetShift + "(TO_CHAR(TO_DATE('" + strDate + "'),'YYYYMMDD'),to_char(K.AF_TIM,'HH24MISS'),TO_NUMBER(K.AF_DAT)," + strSiteID + ") ST_CODE  ";
                strSql = strSql + "                            FROM " + tblPrd005 + "  K     ";
                strSql = strSql + "                             ," + tblModelWiseMapping + " M ";
                strSql = strSql + "                          WHERE K.MTOCD = M.MTOCODE          ";
                strSql = strSql + "                             AND TO_CHAR(K.AF_DAT) BETWEEN TO_CHAR(TO_DATE('" + strDate + "'),'YYYYMMDD') AND TO_CHAR(TO_DATE('" + strDate + "')+1,'YYYYMMDD')   ";
                strSql = strSql + "                             AND M.MTOMAPCODE = '" + strModel + "'";
                strSql = strSql + "                     )                                         ";
                strSql = strSql + "                     WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C')     ";
                strSql = strSql + "                     GROUP BY ST_CODE                                       ";
                strSql = strSql + "               ))P                                                          ";
                strSql = strSql + "          )                                                                 ";
                strSql = strSql + "           ,                                                  ";
                strSql = strSql + "           (                                                                                                                                                                  ";
                strSql = strSql + "                SELECT '1' AS JC,                                                                                                                                             ";
                strSql = strSql + "                NVL(SUM(VQ_QTY_A),0)VQ_QTY_A,NVL(SUM(VQ_QTY_B),0)VQ_QTY_B,                                                                                                    ";
                strSql = strSql + "                NVL(SUM(VQ_QTY),0)VQ_QTY                                                                                                                                      ";
                strSql = strSql + "                FROM                                                                                                                                                          ";
                strSql = strSql + "                (                                                                                                                                                             ";
                strSql = strSql + "                        SELECT                                                                                                                                                ";
                strSql = strSql + "                              CASE WHEN ST_CODE = 'A' THEN COUNT(*) END VQ_QTY_A,                                                                                             ";
                strSql = strSql + "                              CASE WHEN ST_CODE= 'B'  THEN COUNT(*) END VQ_QTY_B,                                                                                             ";
                strSql = strSql + "                              COUNT(*) AS VQ_QTY                                                                                                                              ";
                strSql = strSql + "                           FROM                                                                                                                                               ";
                strSql = strSql + "                           (                                                                                                                                                  ";
                strSql = strSql + "                               SELECT K.AF_DAT,                                                                                                                               ";
                strSql = strSql + "                                      " + funGetShift + "(TO_CHAR(TO_DATE('" + strDate + "'),'YYYYMMDD'),to_char(K.SHP_TIM,'HH24MISS'),TO_NUMBER(K.SHP_DAT)," + strSiteID + ") ST_CODE   ";
                strSql = strSql + "                                  FROM " + tblPrd005 + "  K                                                                                                                   ";
                strSql = strSql + "                                  , " + tblModelWiseMapping + " M                                                                                                             ";
                strSql = strSql + "                                  WHERE K.MTOCD = M.MTOCODE                                                                                                                   ";
                strSql = strSql + "                                   AND TO_CHAR(K.SHP_DAT) BETWEEN TO_CHAR(TO_DATE('" + strDate + "'),'YYYYMMDD') AND TO_CHAR(TO_DATE('" + strDate + "')+1,'YYYYMMDD')                   ";
                strSql = strSql + "                                   AND M.MTOMAPCODE = '" + strModel + "'                                                                                                          ";
                strSql = strSql + "                           )                                                                                                                                                  ";
                strSql = strSql + "                           WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C')                                                                                                 ";
                strSql = strSql + "                           GROUP BY ST_CODE                                                                                                                                   ";
                strSql = strSql + "                 )                                                                                                                                                            ";
                strSql = strSql + "            ) VQ,                                                                                                                                              ";
                strSql = strSql + "                                                                                                                                                               ";
                strSql = strSql + "           (                                                                                                                                                                  ";
                strSql = strSql + "                SELECT '1' AS JC,                                                                                                                                             ";
                strSql = strSql + "                NVL(SUM(RVQ_QTY_A),0)RVQ_QTY_A,NVL(SUM(RVQ_QTY_B),0)RVQ_QTY_B,                                                                                                ";
                strSql = strSql + "                NVL(SUM(RVQ_QTY),0)RVQ_QTY                                                                                                                                    ";
                strSql = strSql + "                FROM                                                                                                                                                          ";
                strSql = strSql + "                (                                                                                                                                                             ";
                strSql = strSql + "                        SELECT                                                                                                                                                ";
                strSql = strSql + "                            CASE WHEN ST_CODE = 'A' THEN COUNT(*) END RVQ_QTY_A,                                                                                              ";
                strSql = strSql + "                            CASE WHEN ST_CODE= 'B'  THEN COUNT(*) END RVQ_QTY_B,                                                                                              ";
                strSql = strSql + "                            COUNT(*) AS RVQ_QTY                                                                                                                               ";
                strSql = strSql + "                         FROM                                                                                                                                                 ";
                strSql = strSql + "                         (                                                                                                                                                    ";
                strSql = strSql + "                             SELECT K.AF_DAT,                                                                                                                                 ";
                strSql = strSql + "                                    " + funGetShift + "(TO_CHAR(TO_DATE('" + strDate + "'),'YYYYMMDD'),to_char(K.SHP_TIM,'HH24MISS'),TO_NUMBER(K.SHP_DAT)," + strSiteID + ") ST_CODE         ";
                strSql = strSql + "                                FROM " + tblPrd005 + " K                                                                                                                      ";
                strSql = strSql + "                                ," + tblModelWiseMapping + " M                                                                                                                 ";
                strSql = strSql + "                                WHERE K.MTOCD = M.MTOCODE                                                                                                                     ";
                strSql = strSql + "                                AND TO_CHAR(K.SHP_DAT) BETWEEN TO_CHAR(TO_DATE('" + strDate + "'),'YYYYMMDD') AND TO_CHAR(TO_DATE('" + strDate + "')+1,'YYYYMMDD')                              ";
                strSql = strSql + "                                AND M.MTOMAPCODE = '" + strModel + "'";
                strSql = strSql + "                         )                                                                                                                                                    ";
                strSql = strSql + "                         WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C')                                                                                                   ";
                strSql = strSql + "                         GROUP BY ST_CODE                                                                                                                                     ";
                strSql = strSql + "                 )                                                                                                                                                            ";
                strSql = strSql + "           ) RVQ                                                                                                                                           ";
                strSql = strSql + "          ,                                                                                                                                                     ";
                strSql = strSql + "          (                                                                                                                                                                   ";
                strSql = strSql + "              SELECT NVL(D.SCNT_A,0) SCNT_A,NVL(D.SCNT_B,0) SCNT_B,'STRAIGHT PASS'AS PASS_TYPE ,'1' AS JC                                                                     ";
                strSql = strSql + "               FROM DUAL,                                                                                                                                                     ";
                strSql = strSql + "               (                                                                                                                                                              ";
                strSql = strSql + "                  SELECT SUM(CNT_A)SCNT_A,SUM(CNT_B)SCNT_B,'STRAIGHT PASS' AS PASS_TYPE                                                                                       ";
                strSql = strSql + "                    FROM                                                                                                                                                      ";
                strSql = strSql + "                    (                                                                                                                                                         ";
                strSql = strSql + "                      SELECT  CASE WHEN ST_CODE = 'A' THEN  COUNT(CHASSIS_NUMBER) END AS CNT_A,                                                                               ";
                strSql = strSql + "                              CASE WHEN ST_CODE = 'B' THEN  COUNT(CHASSIS_NUMBER) END AS CNT_B                                                                                ";
                strSql = strSql + "                         FROM                                                                                                                                                 ";
                strSql = strSql + "                         (                                                                                                                                                    ";
                strSql = strSql + "                           SELECT  A.CHASSIS_NUMBER,                                                                                                                          ";
                strSql = strSql + "                             " + funGetShift + "(TO_CHAR(TO_DATE('" + strDate + "'),'YYYYMMDD'),A.PASS_TIME,TO_NUMBER(TO_CHAR(A.PASS_DATE,'YYYYMMDD'))," + strSiteID + ") ST_CODE                ";
                strSql = strSql + "                             FROM " + tblPrd_VQMS_Header + " A                                                                                                                 ";
                strSql = strSql + "                             , " + tblPrd005 + " PP                                                                                                                            ";
                strSql = strSql + "                             , " + tblModelWiseMapping + " M                                                                                                                   ";
                strSql = strSql + "                           WHERE                                                                                                                                              ";
                strSql = strSql + "                               PP.EF_TYP = A.ENG_FRAME_TYPE AND PP.SHA_NO = A.CHASSIS_NUMBER                                                                                  ";
                strSql = strSql + "                            AND M.MTOCODE = PP.MTOCD                                                                                                                          ";
                strSql = strSql + "                            AND A.PASS_TYPE <>'F'                                                                                                                             ";
                strSql = strSql + "                            AND A.PASS_DATE BETWEEN '" + strDate + "' AND TO_CHAR(TO_DATE('" + strDate + "')+1)                                                                                   ";
                strSql = strSql + "                             AND M.MTOMAPCODE = '" + strModel + "'";
                strSql = strSql + "                        )WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C')                                                                                                   ";
                strSql = strSql + "                        GROUP BY ST_CODE                                                                                                                                      ";
                strSql = strSql + "                   )                                                                                                                                                          ";
                strSql = strSql + "               )D                                                                                                                                  ";
                strSql = strSql + "           )SP                                                                                                                                              ";
                strSql = strSql + "           ,                                                                                                                                                   ";
                strSql = strSql + "           (                                                                                                                                                                  ";
                strSql = strSql + "                SELECT NVL(D.DCNT_A,0) DCNT_A,NVL(D.DCNT_B,0) DCNT_B,'DIRECT PASS' AS PASS_TYPE ,'1' AS JC                                                                    ";
                strSql = strSql + "                 FROM DUAL,                                                                                                                                                    ";
                strSql = strSql + "                 (                                                                                                                                                            ";
                strSql = strSql + "                    SELECT SUM(CNT_A)DCNT_A,SUM(CNT_B)DCNT_B,'DIRECT PASS' AS PASS_TYPE                                                                                       ";
                strSql = strSql + "                      FROM                                                                                                                                                    ";
                strSql = strSql + "                      (                                                                                                                                                       ";
                strSql = strSql + "                         SELECT  CASE WHEN ST_CODE = 'A' THEN  COUNT(CHASSIS_NUMBER) END AS CNT_A,                                                                            ";
                strSql = strSql + "                              CASE WHEN ST_CODE = 'B' THEN  COUNT(CHASSIS_NUMBER) END AS CNT_B                                                                                ";
                strSql = strSql + "                         FROM                                                                                                                                                 ";
                strSql = strSql + "                         (                                                                                                                                                    ";
                strSql = strSql + "                           SELECT  A.CHASSIS_NUMBER,                                                                                                                          ";
                strSql = strSql + "                             " + funGetShift + "(TO_CHAR(TO_DATE('" + strDate + "'),'YYYYMMDD'),A.PASS_TIME,TO_NUMBER(TO_CHAR(A.PASS_DATE,'YYYYMMDD'))," + strSiteID + ") ST_CODE                ";
                strSql = strSql + "                             FROM " + tblPrd_VQMS_Header + " A";
                strSql = strSql + "                             ," + tblPrd005 + " PP                                                                                                                             ";
                strSql = strSql + "                             ," + tblModelWiseMapping + " M                                                                                                                    ";
                strSql = strSql + "                           WHERE 1=1                                                                                                                                          ";
                strSql = strSql + "                            AND PP.EF_TYP = A.ENG_FRAME_TYPE AND PP.SHA_NO = A.CHASSIS_NUMBER                                                                                 ";
                strSql = strSql + "                            AND M.MTOCODE = PP.MTOCD                                                                                                                          ";
                strSql = strSql + "                            AND A.PASS_TYPE ='D'                                                                                                                              ";
                strSql = strSql + "                            AND A.PASS_DATE BETWEEN '" + strDate + "' AND TO_CHAR(TO_DATE('" + strDate + "')+1)                                                                                   ";
                strSql = strSql + "                            AND M.MTOMAPCODE = '" + strModel + "'";
                strSql = strSql + "                        )WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C')                                                                                                   ";
                strSql = strSql + "                        GROUP BY ST_CODE                                                                                                                                      ";
                strSql = strSql + "                     )                                                                                                                                                        ";
                strSql = strSql + "                 )D                                                                                                                                ";
                strSql = strSql + "           ) DP                                                                                                                                             ";
                strSql = strSql + "           ,                                                                                                                                                   ";
                strSql = strSql + "           (                                                                                                                                                                  ";
                strSql = strSql + "             SELECT  COUNT (*) Not_Entered_Frames,JC                                                                                                                          ";
                strSql = strSql + "             FROM                                                                                                                                                             ";
                strSql = strSql + "             (                                                                                                                                                                ";
                strSql = strSql + "                SELECT  A.CHASSIS_NUMBER,                                                                                                                                     ";
                strSql = strSql + "                   " + funGetShift + "(TO_CHAR(TO_DATE('" + strDate + "'),'YYYYMMDD'),A.PASS_TIME,TO_CHAR(A.PASS_DATE,'YYYYMMDD')," + strSiteID + ")                                             ";
                strSql = strSql + "                   ST_CODE,'1'JC                                                                                                                                              ";
                strSql = strSql + "                 FROM " + tblPrd_VQMS_Header + " A";
                strSql = strSql + "                 , " + tblPrd005 + " PP                                                                                                                                        ";
                strSql = strSql + "                 , " + tblModelWiseMapping + " M                                                                                                                               ";
                strSql = strSql + "                 WHERE                                                                                                                                                        ";
                strSql = strSql + "                       PP.EF_TYP = A.ENG_FRAME_TYPE AND PP.SHA_NO = A.CHASSIS_NUMBER                                                                                          ";
                strSql = strSql + "                   AND M.MTOCODE = PP.MTOCD                                                                                                                                   ";
                strSql = strSql + "                   AND NOT EXISTS(SELECT MODELCODE FROM " + tblFrameWiseDefect + "  B WHERE B.ENG_FRAME_TYPE = A.ENG_FRAME_TYPE                                                ";
                strSql = strSql + "                               AND  A.CHASSIS_NUMBER =  CHASSIS_NUMBER )                                                                                                      ";
                strSql = strSql + "                   AND  A.PASS_TYPE<>'D'                                                                                                                                      ";
                strSql = strSql + "                   AND (A.PASS_DATE BETWEEN '" + strDate + "' AND TO_CHAR(TO_DATE('" + strDate + "')+1))                                                                                          ";
                strSql = strSql + "                   AND  M.MTOMAPCODE = '" + strModel + "'";
                strSql = strSql + "              )WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C')                                                                                                             ";
                strSql = strSql + "           )FN where VQ.JC = P.JC and RVQ.JC = VQ.JC and PASS_TYPE = D.PASS_TYPE and SP.JC = VQ.JC and PASS_TYPE = D.PASS_TYPE and DP.JC = SP.JC and FN.JC = DP.JC ";




                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.Text;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                objAdr.Fill(objDS4);
                return objDS4.Tables[0];

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

    public DataTable GetReportHeaderDetailsProd(String strDate, String strModel, String strSiteID,string strline)
    {
        DataSet objDS4 = new DataSet();

        using (objCn = new OracleConnection())
        {
            objCn.ConnectionString = strConn;
            try
            {
                objCn.Open();
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;

                string strSql = string.Empty;


                strsql = "SELECT  "
                            + " NVL(SUM(CASE WHEN ST_CODE = 'A' THEN COUNT(*) END),0) PRODUCTION_QTY_A, "
                            + " NVL(SUM(CASE WHEN ST_CODE = 'B' THEN COUNT(*) END),0) PRODUCTION_QTY_B,"
                            + " NVL(SUM(COUNT(*)),0) AS PRODUCTION_QTY     "
                            + "   FROM "
                            + "  ("
                            + "      SELECT K.AF_DAT,"
                            + "              " + funGetShift + "(TO_CHAR(TO_DATE('" + strDate + "'),'YYYYMMDD'),to_char(K.AF_TIM,'HH24MISS'),TO_NUMBER(K.AF_DAT)," + strSiteID + ") ST_CODE"
                            + "          FROM " + tblPrd005 + "  K"
                            + "           ," + tblModelWiseMapping + " M "
                            + "        WHERE K.MTOCD = M.MTOCODE "
                            + "           AND K.AF_DAT BETWEEN TO_CHAR(TO_DATE('" + strDate + "'),'YYYYMMDD') AND TO_CHAR(TO_DATE('" + strDate + "')+1,'YYYYMMDD')"
                            + "           AND M.MTOMAPCODE = '" + strModel + "' AND K.RTN_DAT IS NOT NULL "
                            + "           AND K.PRD_LIN = '" + strline + "' "
                            + "   )"
                            + "   WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C') "
                            + "   GROUP BY ST_CODE  ";




                objCmd.CommandText = strsql;
                objCmd.CommandType = System.Data.CommandType.Text;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                objAdr.Fill(objDS4);
                return objDS4.Tables[0];

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

    public DataTable GetReportHeaderDetailsVQ(String strDate, String strModel, String strSiteID, string strline)
    {


        DataSet objDS4 = new DataSet();

        string strsql;



        using (objCn = new OracleConnection())
        {
            objCn.ConnectionString = strConn;
            try
            {
                objCn.Open();
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;

                string strSql = string.Empty;


                strSql = " SELECT  "
                          + "      NVL(SUM(CASE WHEN ST_CODE = 'A' THEN COUNT(*) END),0) VQ_QTY_A, "
                          + "      NVL(SUM(CASE WHEN ST_CODE= 'B'  THEN COUNT(*) END),0) VQ_QTY_B, "
                          + "      NVL(SUM(COUNT(*)),0) AS VQ_QTY      "
                          + "  FROM "
                          + "  ("
                          + "      SELECT K.AF_DAT,"
                          + "             " + funGetShift + "(TO_CHAR(TO_DATE('" + strDate + "'),'YYYYMMDD'),to_char(K.SHP_TIM,'HH24MISS'),TO_NUMBER(K.SHP_DAT)," + strSiteID + ") ST_CODE"
                          + "         FROM " + tblPrd005 + "  K"
                          + "         , " + tblModelWiseMapping + " M "
                          + "         WHERE K.MTOCD = M.MTOCODE"
                          + "          AND K.SHP_DAT BETWEEN TO_CHAR(TO_DATE('" + strDate + "'),'YYYYMMDD') AND TO_CHAR(TO_DATE('" + strDate + "')+1,'YYYYMMDD')"
                          + "          AND M.MTOMAPCODE = '" + strModel + "'"
                          + "           AND K.PRD_LIN = '" + strline + "' "
                          + "  )"
                          + "  WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C') "
                          + "  GROUP BY ST_CODE";


                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.Text;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                objAdr.Fill(objDS4);
                return objDS4.Tables[0];

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

    public DataTable GetReportHeaderDetailsRVQ(String strDate, String strModel, String strSiteID,string strline)
    {


        DataSet objDS4 = new DataSet();

        string strsql;




        using (objCn = new OracleConnection())
        {
            objCn.ConnectionString = strConn;
            try
            {
                objCn.Open();
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;

                string strSql = string.Empty;


                strSql = "SELECT "
                                  + " NVL(SUM(CASE WHEN ST_CODE = 'A' THEN COUNT(*) END),0) RVQ_QTY_A, "
                                  + " NVL(SUM(CASE WHEN ST_CODE = 'B' THEN COUNT(*) END),0) RVQ_QTY_B, "
                                  + " NVL(SUM(COUNT(*)),0) AS RVQ_QTY      "
                                  + " FROM  "
                                  + "  ( "
                                  + "     SELECT K.AF_DAT, "
                                  + "           " + funGetShift + "(TO_CHAR(TO_DATE('" + strDate + "'),'YYYYMMDD'),to_char(K.SHP_TIM,'HH24MISS'),TO_NUMBER(K.SHP_DAT)," + strSiteID + ") ST_CODE "
                                  + "      FROM " + tblPrd005 + "  K "
                                  + "     ," + tblModelWiseMapping + " M  "
                                  + "     WHERE K.MTOCD = M.MTOCODE "
                                  + "     AND K.SHP_DAT BETWEEN TO_CHAR(TO_DATE('" + strDate + "'),'YYYYMMDD') AND TO_CHAR(TO_DATE('" + strDate + "')+1,'YYYYMMDD') "
                                  + "     AND M.MTOMAPCODE = '" + strModel + "'"
                                  + "           AND K.PRD_LIN = '" + strline + "' "
                                  + " ) "
                                  + " WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C')  "
                                  + " GROUP BY ST_CODE";




                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.Text;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                objAdr.Fill(objDS4);
                return objDS4.Tables[0];

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

    public DataTable GetReportHeaderDetailsSP(String strDate, String strModel, String strSiteID,string strline)
    {

        DataSet objDS4 = new DataSet();

        string strsql;



        using (objCn = new OracleConnection())
        {
            objCn.ConnectionString = strConn;
            try
            {
                objCn.Open();
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;

                string strSql = string.Empty;


                strSql = "SELECT  NVL(SUM(CASE WHEN ST_CODE = 'A' THEN  COUNT(CHASSIS_NUMBER) END),0) AS CNT_A, "
                         + "      NVL(SUM(CASE WHEN ST_CODE = 'B' THEN  COUNT(CHASSIS_NUMBER) END),0) AS CNT_B, "
                         + "      NVL(SUM(COUNT(*)),0)  TOT "
                         + "   FROM "
                         + "   ("
                         + "        SELECT  A.CHASSIS_NUMBER,"
                         + "         " + funGetShift + "(TO_CHAR(TO_DATE('" + strDate + "'),'YYYYMMDD'),A.PASS_TIME,TO_NUMBER(TO_CHAR(A.PASS_DATE,'YYYYMMDD'))," + strSiteID + ") ST_CODE"
                         + "         FROM " + tblPrd_VQMS_Header + " A"
                         + "        , " + tblPrd005 + " PP  "
                         + "        , " + tblModelWiseMapping + " M                            "
                         + "      WHERE"
                         + "         PP.EF_TYP = A.ENG_FRAME_TYPE AND PP.SHA_NO = A.CHASSIS_NUMBER "
                         + "       AND M.MTOCODE = PP.MTOCD"
                         + "      AND A.PASS_TYPE <>'F' "
                         + "      AND A.PASS_DATE BETWEEN '" + strDate + "'  AND TO_CHAR(TO_DATE('" + strDate + "' )+1)"
                         + "      AND M.MTOMAPCODE = '" + strModel + "'"
                         + "      AND PP.PRD_LIN = '" + strline + "' "
                         + " )WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C') "
                         + " GROUP BY ST_CODE  ";

                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.Text;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                objAdr.Fill(objDS4);
                return objDS4.Tables[0];

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

    public DataTable GetReportHeaderDetailsDP(String strDate, String strModel, String strSiteID,string strline)
    {

        DataSet objDS4 = new DataSet();

        string strsql;

        using (objCn = new OracleConnection())
        {
            objCn.ConnectionString = strConn;
            try
            {
                objCn.Open();
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;

                string strSql = string.Empty;


                strSql = " SELECT  NVL(SUM(CASE WHEN ST_CODE = 'A' THEN  COUNT(CHASSIS_NUMBER) END),0) AS CNT_A, "
                         + "       NVL(SUM(CASE WHEN ST_CODE = 'B' THEN  COUNT(CHASSIS_NUMBER) END),0) AS CNT_B,  "
                         + "       NVL(SUM(COUNT(*)),0) AS TOT "
                         + "   FROM "
                         + "   ("
                         + "     SELECT  A.CHASSIS_NUMBER,"
                         + "       " + funGetShift + "(TO_CHAR(TO_DATE('" + strDate + "'),'YYYYMMDD'),A.PASS_TIME,TO_NUMBER(TO_CHAR(A.PASS_DATE,'YYYYMMDD'))," + strSiteID + ") ST_CODE"
                         + "       FROM " + tblPrd_VQMS_Header + " A"
                         + "       ," + tblPrd005 + " PP  "
                         + "      ," + tblModelWiseMapping + " M                           "
                         + "     WHERE 1=1"
                         + "      AND PP.EF_TYP = A.ENG_FRAME_TYPE AND PP.SHA_NO = A.CHASSIS_NUMBER"
                         + "      AND M.MTOCODE = PP.MTOCD  "
                         + "      AND A.PASS_TYPE ='D' "
                         + "      AND A.PASS_DATE BETWEEN '" + strDate + "'  AND TO_CHAR(TO_DATE('" + strDate + "')+1)"
                         + "      AND M.MTOMAPCODE = '" + strModel + "' "
                         + "      AND PP.PRD_LIN = '" + strline + "' "
                         + "   )WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C') "
                         + "   GROUP BY ST_CODE  ";

                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.Text;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                objAdr.Fill(objDS4);
                return objDS4.Tables[0];

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

    public DataTable GetReportHeaderDetailsFN(String strDate, String strModel, String strSiteID,string strline)
    {


        DataSet objDS4 = new DataSet();

        string strsql;

        using (objCn = new OracleConnection())
        {
            objCn.ConnectionString = strConn;
            try
            {
                objCn.Open();
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;

                string strSql = string.Empty;


                strSql = " SELECT  COUNT (*) Not_Entered_Frames "
                         + "       FROM "
                         + "       ("
                         + "          SELECT  A.CHASSIS_NUMBER,"
                         + "             " + funGetShift + "(TO_CHAR(TO_DATE('" + strDate + "'),'YYYYMMDD'),A.PASS_TIME,TO_CHAR(A.PASS_DATE,'YYYYMMDD')," + strSiteID + ") "
                         + "             ST_CODE,'1'JC"
                         + "           FROM " + tblPrd_VQMS_Header + " A"
                         + "           , " + tblPrd005 + " PP  "
                         + "           , " + tblModelWiseMapping + " M "
                         + "           WHERE "
                         + "                 PP.EF_TYP = A.ENG_FRAME_TYPE AND PP.SHA_NO = A.CHASSIS_NUMBER"
                         + "             AND M.MTOCODE = PP.MTOCD   "
                         + "             AND NOT EXISTS(SELECT MODELCODE FROM " + tblFrameWiseDefect + "  B WHERE B.ENG_FRAME_TYPE = A.ENG_FRAME_TYPE "
                         + "                         AND  A.CHASSIS_NUMBER =  CHASSIS_NUMBER ) "
                         + "             AND  A.PASS_TYPE<>'D'  "
                         + "             AND (A.PASS_DATE BETWEEN '" + strDate + "' AND TO_CHAR(TO_DATE('" + strDate + "')+1)) "
                         + "            AND  M.MTOMAPCODE = '" + strModel + "' "
                         + "           AND PP.PRD_LIN = '" + strline + "' "
                         + "        )WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C') ";


                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.Text;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                objAdr.Fill(objDS4);
                return objDS4.Tables[0];

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

    public DataTable GetReportDefectDetails(String strDate, String strModel, String strSiteID,string strline)
    {
        string strSql = string.Empty;


        DataSet objDS4 = new DataSet();


        using (objCn = new OracleConnection())
        {
            objCn.ConnectionString = strConn;
            try
            {
                objCn.Open();
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;

                strSql = strSql + "        SELECT DEFECTDESCRIPTION,DISC AS Category ,SUBCODE AS Section,count(A_SHIFT)A_SHIFT,count(B_SHIFT)B_SHIFT,count(A_SHIFT) + count(B_SHIFT) as Total                           ";
                strSql = strSql + "                    FROM                                                                                                                                                             ";
                strSql = strSql + "                    (                                                                                                                                                                ";
                strSql = strSql + "                      SELECT DEFECTDESCRIPTION,DISC,SUBCODE,                                                                                                                         ";
                strSql = strSql + "                              CASE WHEN ST_CODE = 'A' THEN ENG_FRAME_TYPE END AS A_SHIFT,                                                                                            ";
                strSql = strSql + "                              CASE WHEN ST_CODE = 'B' THEN ENG_FRAME_TYPE END AS B_SHIFT                                                                                             ";
                strSql = strSql + "                              FROM                                                                                                                                                   ";
                strSql = strSql + "                              (                                                                                                                                                      ";
                strSql = strSql + "                                SELECT DFM.DEFECTDESCRIPTION,C.DISC,SC.SUBCODE,D.ENG_FRAME_TYPE,                                                                                     ";
                strSql = strSql + "                                         " + funGetShift + "(TO_CHAR(TO_DATE('" + strDate + "'),'YYYYMMDD'),H.PASS_TIME,TO_NUMBER(TO_CHAR(H.PASS_DATE,'YYYYMMDD')),'" + strSiteID + "') ST_CODE           ";
                strSql = strSql + "                                    FROM " + tblPrd_VQMS_Header + " H                                                                                                                  ";
                strSql = strSql + "                                     , " + tblPrd005 + " PP                                                                                                                           ";
                strSql = strSql + "                                    , " + tblFrameWiseDefect + " D                                                                                                                   ";
                strSql = strSql + "                                    , " + tblDefectMaster + " DFM                                                                                                                    ";
                strSql = strSql + "                                    , " + tblCodeType + " C                                                                                                                          ";
                strSql = strSql + "                                    , " + tblCodeType + " SC                                                                                                                         ";
                strSql = strSql + "                                  WHERE                                                                                                                                              ";
                strSql = strSql + "                                        (H.ENG_FRAME_TYPE) = (D.ENG_FRAME_TYPE) AND (H.CHASSIS_NUMBER) = (D.CHASSIS_NUMBER)                                                          ";
                strSql = strSql + "                                    AND (DFM.DEFECTCODE) = (D.DEFECTCODE) AND (DFM.MODELCODE) = (D.MODELCODE)                                                                        ";
                strSql = strSql + "                                    AND PP.EF_TYP = H.ENG_FRAME_TYPE AND PP.SHA_NO = H.CHASSIS_NUMBER                                                                                ";
                strSql = strSql + "                                    AND (C.SUBCODE)  = (D.DEFECTCATEGORY) AND C.CODE  = 'CATEGORY'                                                                                   ";
                strSql = strSql + "                                    AND (SC.SUBCODE) =  (D.DEFECTSECTION)  AND SC.CODE = 'SECTION'                                                                                   ";
                strSql = strSql + "                                    AND PP.PRD_LIN = '" + strline + "'                                                                                                                   ";
                strSql = strSql + "                                    AND (H.PASS_DATE BETWEEN '" + strDate + "' AND TO_CHAR(TO_DATE('" + strDate + "')+1)) AND D.MODELCODE = '" + strModel + "'";
                strSql = strSql + "                               ) WHERE  ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C')                                                                                                 ";
                strSql = strSql + "                    )                                                                                                                                                                ";
                strSql = strSql + "          GROUP BY DEFECTDESCRIPTION,DISC,SUBCODE                                                                                                                                    ";
                strSql = strSql + "          ORDER BY TOTAL DESC                                                                                                                                             ";




                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.Text;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                objAdr.Fill(objDS4);
                return objDS4.Tables[0];

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

    public DataTable GetReportDefectShopwise(String strDate, String strModel, String strSiteID,string strline)
    {
        string strSql = string.Empty;

        DataSet objDS4 = new DataSet();

        using (objCn = new OracleConnection())
        {
            objCn.ConnectionString = strConn;
            try
            {
                objCn.Open();
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;

                strSql = "SELECT JC,SECTION,A_SHIFT,B_SHIFT,TOTALDEFECTS, "
                          + "    ROUND((SUM(TOTALDEFECTS) OVER (ORDER BY  TOTALDEFECTS DESC) /SUM(TOTALDEFECTS) OVER (PARTITION BY JC) ) *100,2) cm "
                          + "            FROM "
                          + "              ( "
                          + "                    SELECT '1' jc,SUBCODE AS Section,count(A_SHIFT) A_SHIFT,count(B_SHIFT) B_SHIFT,count(A_SHIFT) + count(B_SHIFT) as TotalDefects "
                          + "                      FROM ( "
                          + "                                   SELECT DEFECTDESCRIPTION,DISC,SUBCODE, "
                          + "                                     CASE WHEN ST_CODE = 'A' THEN ENG_FRAME_TYPE END AS A_SHIFT, "
                          + "                                     CASE WHEN ST_CODE = 'B' THEN ENG_FRAME_TYPE END AS B_SHIFT "
                          + "                                     FROM ( "
                          + "                                            SELECT DFM.DEFECTDESCRIPTION,C.DISC,SC.SUBCODE,D.ENG_FRAME_TYPE, "
                          + "                                                   " + funGetShift + "(TO_CHAR(TO_DATE('" + strDate + "'),'YYYYMMDD'),H.PASS_TIME,TO_NUMBER(TO_CHAR(H.PASS_DATE,'YYYYMMDD'))," + strSiteID + ") ST_CODE "
                          + "                                              FROM " + tblPrd_VQMS_Header + " H, "
                          + "                                                   " + tblFrameWiseDefect + " D, "
                          + "                                                   " + tblPrd005 + " PP,  "  
                          + "                                                   " + tblDefectMaster + "    DFM, "
                          + "                                                   " + tblCodeType + "         C, "
                          + "                                                   " + tblCodeType + "         SC "
                          + "                                             WHERE 1 = 1 "
                          + "                                               AND H.ENG_FRAME_TYPE = D.ENG_FRAME_TYPE "
                          + "                                               AND H.CHASSIS_NUMBER = D.CHASSIS_NUMBER "
                          + "                                               AND DFM.DEFECTCODE = D.DEFECTCODE "
                          + "                                               AND DFM.MODELCODE = D.MODELCODE "
                          + "                                               AND C.SUBCODE = D.DEFECTCATEGORY "
                          + "                                               AND PP.EF_TYP = H.ENG_FRAME_TYPE AND PP.SHA_NO = H.CHASSIS_NUMBER "
                          + "                                              AND C.CODE = 'CATEGORY' "
                          + "                                               AND SC.SUBCODE = D.DEFECTSECTION "
                          + "                                               AND SC.CODE = 'SECTION' "
                          + "                                               AND (H.PASS_DATE BETWEEN '" + strDate + "' AND "
                          + "                                                   TO_CHAR(TO_DATE('" + strDate + "') + 1)) "
                          + "                                               AND D.MODELCODE = '" + strModel + "'"
                          + "                                               AND PP.PRD_LIN = '" + strline + "' "
                          + "                                            ) "
                          + "                              WHERE ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C') "
                          + "                           ) "
                          + "                   group by SUBCODE "
                          + "               )        ";




                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.Text;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                objAdr.Fill(objDS4);
                return objDS4.Tables[0];

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

    public DataTable GetReportDefectCategorywise(String strDate, String strModel, String strSiteID,string strline)
    {
        string strSql = string.Empty;

        DataSet objDS4 = new DataSet();

        using (objCn = new OracleConnection())
        {
            objCn.ConnectionString = strConn;
            try
            {
                objCn.Open();
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;



                strSql = " SELECT JC,CATEGORY,A_SHIFT,B_SHIFT,TOTALDEFECTS,ROUND((SUM(TOTALDEFECTS) OVER (ORDER BY  TOTALDEFECTS DESC) /SUM(TOTALDEFECTS) OVER (PARTITION BY JC) ) *100,2)cm "
                         + "        FROM "
                         + "        (           "
                         + "          SELECT '1' jc,DISC AS Category,count(A_SHIFT) A_SHIFT,count(B_SHIFT) B_SHIFT,count(A_SHIFT) + count(B_SHIFT) as TotalDefects "
                         + "             FROM ( "
                         + "                   SELECT DEFECTDESCRIPTION,DISC,SUBCODE, "
                         + "                          CASE WHEN ST_CODE = 'A' THEN ENG_FRAME_TYPE END AS A_SHIFT, "
                         + "                          CASE WHEN ST_CODE = 'B' THEN ENG_FRAME_TYPE END AS B_SHIFT "
                         + "                     FROM ( "
                         + "                           SELECT DFM.DEFECTDESCRIPTION, "
                         + "                                  C.DISC, "
                         + "                                  SC.SUBCODE, "
                         + "                                  D.ENG_FRAME_TYPE, "
                         + "                                  " + funGetShift + "(TO_CHAR(TO_DATE('" + strDate + "'),'YYYYMMDD'),H.PASS_TIME,TO_NUMBER(TO_CHAR(H.PASS_DATE,'YYYYMMDD'))," + strSiteID + ") ST_CODE "
                         + "                             FROM " + tblPrd_VQMS_Header + " H, "
                          + "                                  " + tblPrd005 + " PP, "
                         + "                                  " + tblFrameWiseDefect + " D, "
                         + "                                  " + tblDefectMaster + "    DFM, "
                         + "                                  " + tblCodeType + "         C, "
                         + "                                  " + tblCodeType + "         SC "
                         + "                            WHERE 1 = 1 "
                         + "                              AND H.ENG_FRAME_TYPE = D.ENG_FRAME_TYPE "
                         + "                              AND H.CHASSIS_NUMBER = D.CHASSIS_NUMBER "
                         + "                              AND PP.EF_TYP = H.ENG_FRAME_TYPE AND PP.SHA_NO = H.CHASSIS_NUMBER "
                         + "                              AND DFM.DEFECTCODE = D.DEFECTCODE "
                         + "                              AND DFM.MODELCODE = D.MODELCODE "
                         + "                              AND C.SUBCODE = D.DEFECTCATEGORY "
                         + "                              AND C.CODE = 'CATEGORY' "
                         + "                              AND SC.SUBCODE = D.DEFECTSECTION "
                         + "                              AND SC.CODE = 'SECTION' "
                         + "                              AND (H.PASS_DATE BETWEEN '" + strDate + "' AND "
                         + "                                  TO_CHAR(TO_DATE('" + strDate + "') + 1)) "
                         + "                              AND D.MODELCODE = '" + strModel + "' "
                         + "                              AND PP.PRD_LIN = '" + strline + "' "
                         + "                            ) "
                         + "                    WHERE ST_CODE IS NOT NULL "
                         + "                      AND ST_CODE NOT IN ('C') "
                         + "                 ) "
                         + "            group by DISC "
                         + "        )";




                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.Text;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                objAdr.Fill(objDS4);
                return objDS4.Tables[0];

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

    public DataTable GetReportDefectShopCnt(String strDate, String strModel, String strSiteID,string strline)
    {
        string strSql = string.Empty;

        DataSet objDS4 = new DataSet();

        using (objCn = new OracleConnection())
        {
            objCn.ConnectionString = strConn;
            try
            {
                objCn.Open();
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;



                strSql = strSql + "             SELECT '1'jc, SUBCODE AS Section,count(A_SHIFT)A_SHIFT,count(B_SHIFT)B_SHIFT,count(A_SHIFT) + count(B_SHIFT) as TotalDefects                                    ";
                strSql = strSql + "                FROM                                                                                                                                                         ";
                strSql = strSql + "                (                                                                                                                                                            ";
                strSql = strSql + "                  SELECT DEFECTDESCRIPTION,DISC,SUBCODE,                                                                                                                     ";
                strSql = strSql + "                        CASE WHEN ST_CODE = 'A' THEN ENG_FRAME_TYPE END AS A_SHIFT,                                                                                          ";
                strSql = strSql + "                        CASE WHEN ST_CODE = 'B' THEN ENG_FRAME_TYPE END AS B_SHIFT                                                                                           ";
                strSql = strSql + "                        FROM                                                                                                                                                 ";
                strSql = strSql + "                        (                                                                                                                                                    ";
                strSql = strSql + "                          SELECT DFM.DEFECTDESCRIPTION,C.DISC,SC.SUBCODE,D.ENG_FRAME_TYPE,                                                                                   ";
                strSql = strSql + "                                   " + funGetShift + "(TO_CHAR(TO_DATE('" + strDate + "'),'YYYYMMDD'),H.PASS_TIME,TO_NUMBER(TO_CHAR(H.PASS_DATE,'YYYYMMDD'))," + strSiteID + ") ST_CODE         ";
                strSql = strSql + "                              FROM " + tblPrd_VQMS_Header + " H                                                                                                               ";
                strSql = strSql + "                              , " + tblPrd005 + " PP                                                                                                                  ";
                strSql = strSql + "                              , " + tblFrameWiseDefect + " D                                                                                                                  ";
                strSql = strSql + "                              , " + tblDefectMaster + " DFM                                                                                                                   ";
                strSql = strSql + "                              , " + tblCodeType + " C                                                                                                                          ";
                strSql = strSql + "                              , " + tblCodeType + " SC                                                                                                                         ";
                strSql = strSql + "                            WHERE 1=1                                                                                                                                        ";
                strSql = strSql + "                            AND H.ENG_FRAME_TYPE = D.ENG_FRAME_TYPE AND H.CHASSIS_NUMBER = D.CHASSIS_NUMBER                                                                  ";
                strSql = strSql + "                            AND DFM.DEFECTCODE = D.DEFECTCODE AND DFM.MODELCODE = D.MODELCODE                                                                                ";
                strSql = strSql + "                            AND PP.EF_TYP = H.ENG_FRAME_TYPE AND PP.SHA_NO = H.CHASSIS_NUMBER                                                                                 ";
                strSql = strSql + "                            AND C.SUBCODE  =  D.DEFECTCATEGORY AND C.CODE  = 'CATEGORY'                                                                                      ";
                strSql = strSql + "                            AND SC.SUBCODE =  D.DEFECTSECTION  AND SC.CODE = 'SECTION'                                                                                       ";
                strSql = strSql + "                            AND (H.PASS_DATE BETWEEN '" + strDate + "' AND TO_CHAR(TO_DATE('" + strDate + "')+1)) AND D.MODELCODE = '" + strModel + "'";
                strSql = strSql + "                            AND PP.PRD_LIN = '" + strline + "'                                                                                                                   ";
                strSql = strSql + "                         )WHERE  ST_CODE IS NOT NULL AND ST_CODE NOT IN ('C')                                                                                                ";
                strSql = strSql + "                )group by SUBCODE                                                                                                                          ";

                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.Text;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                objAdr.Fill(objDS4);
                return objDS4.Tables[0];

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

    public DataTable GetReportDefectCategorywiseCnt(String strDate, String strModel, String strSiteID,string strline)
    {
        string strSql = string.Empty;
        DataSet objDS4 = new DataSet();

        using (objCn = new OracleConnection())
        {
            objCn.ConnectionString = strConn;
            try
            {
                objCn.Open();
                objCmd = new OracleCommand();
                objCmd.Connection = objCn;




                strSql = strSql + "           SELECT '1'jc, DISC AS Category,count(A_SHIFT)A_SHIFT,count(B_SHIFT)B_SHIFT,count(A_SHIFT) + count(B_SHIFT) as TotalDefects                                       ";
                strSql = strSql + "                 FROM                                                                                                                                                       ";
                strSql = strSql + "                 (                                                                                                                                                          ";
                strSql = strSql + "                   SELECT DEFECTDESCRIPTION,DISC,SUBCODE,                                                                                                                   ";
                strSql = strSql + "                       CASE WHEN ST_CODE = 'A' THEN ENG_FRAME_TYPE END AS A_SHIFT,                                                                                          ";
                strSql = strSql + "                       CASE WHEN ST_CODE = 'B' THEN ENG_FRAME_TYPE END AS B_SHIFT                                                                                           ";
                strSql = strSql + "                       FROM                                                                                                                                                 ";
                strSql = strSql + "                       (                                                                                                                                                    ";
                strSql = strSql + "                         SELECT DFM.DEFECTDESCRIPTION,C.DISC,SC.SUBCODE,D.ENG_FRAME_TYPE,                                                                                   ";
                strSql = strSql + "                                  " + funGetShift + "(TO_CHAR(TO_DATE('" + strDate + "'),'YYYYMMDD'),H.PASS_TIME,TO_NUMBER(TO_CHAR(H.PASS_DATE,'YYYYMMDD'))," + strSiteID + ") ST_CODE ";
                strSql = strSql + "                             FROM " + tblPrd_VQMS_Header + " H                                                                                                               ";
                strSql = strSql + "                              , " + tblPrd005 + " PP                                                                                                                  ";
                strSql = strSql + "                             , " + tblFrameWiseDefect + " D                                                                                                                  ";
                strSql = strSql + "                             , " + tblDefectMaster + " DFM                                                                                                                   ";
                strSql = strSql + "                             , " + tblCodeType + " C                                                                                                                          ";
                strSql = strSql + "                             , " + tblCodeType + " SC                                                                                                                         ";
                strSql = strSql + "                           WHERE 1=1                                                                                                                                        ";
                strSql = strSql + "                           AND H.ENG_FRAME_TYPE = D.ENG_FRAME_TYPE AND H.CHASSIS_NUMBER = D.CHASSIS_NUMBER                                                                  ";
                strSql = strSql + "                            AND PP.EF_TYP = H.ENG_FRAME_TYPE AND PP.SHA_NO = H.CHASSIS_NUMBER                                                                                 ";
                strSql = strSql + "                           AND DFM.DEFECTCODE = D.DEFECTCODE AND DFM.MODELCODE = D.MODELCODE                                                                                ";
                strSql = strSql + "                           AND C.SUBCODE  =  D.DEFECTCATEGORY AND C.CODE  = 'CATEGORY'                                                                                      ";
                strSql = strSql + "                           AND SC.SUBCODE =  D.DEFECTSECTION  AND SC.CODE = 'SECTION'                                                                                       ";
                strSql = strSql + "                           AND (H.PASS_DATE BETWEEN '" + strDate + "' AND TO_CHAR(TO_DATE('" + strDate + "')+1)) AND D.MODELCODE = '" + strModel + "'";
                strSql = strSql + "                           AND PP.PRD_LIN = '" + strline + "'                                                                                                                   ";
                strSql = strSql + "                        )WHERE  ST_CODE IS NOT NULL  AND ST_CODE NOT IN ('C')                                                                                               ";
                strSql = strSql + "                 )group by DISC  ";

                objCmd.CommandText = strSql;
                objCmd.CommandType = System.Data.CommandType.Text;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                objAdr.Fill(objDS4);
                return objDS4.Tables[0];

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

    #region GetDefectReport

    public DataTable ShopWiseReprot(string V_DATEFROM, string V_DATETO)
    {
        using (objCn = new OracleConnection())
        {
            objCn.ConnectionString = strConn;
            try
            {
                objCn.Open();
                objCmd = new OracleCommand();

                StringBuilder sqlStrBuild = new StringBuilder();

                sqlStrBuild.Append("        SELECT COUNT(*), C.DISC, C.SUBCODE ");
                sqlStrBuild.Append("          FROM ");
                sqlStrBuild.Append(tblFrameWiseDefect);
                sqlStrBuild.Append(" D, ");
                sqlStrBuild.Append(tblCodeType);
                sqlStrBuild.Append(" C ");
                sqlStrBuild.Append("         WHERE TO_DATE(TO_CHAR(D.CREATEDATE,'MM-DD-YYYY') ||' '|| TO_CHAR(TO_DATE(D.CREATETIME,'HH24MISS'),'HH24:MI:SS'),'MM-DD-YYYY HH24:MI:SS') ");
                sqlStrBuild.Append("               BETWEEN ");
                sqlStrBuild.Append("                  TO_DATE(TO_CHAR(TO_DATE('");
                sqlStrBuild.Append(V_DATEFROM);
                sqlStrBuild.Append("' , 'DD-MON-YYYY'),'MM-DD-YYYY') || ' 06:30:00','MM-DD-YYYY HH24:MI:SS') ");
                sqlStrBuild.Append("                  AND");
                sqlStrBuild.Append("                  TO_DATE(TO_CHAR(TO_DATE('");
                sqlStrBuild.Append(V_DATETO);
                sqlStrBuild.Append("' , 'DD-MON-YYYY')+1,'MM-DD-YYYY') || ' 06:29:59','MM-DD-YYYY HH24:MI:SS') ");
                sqlStrBuild.Append("                  AND C.SUBCODE = D.DEFECTSECTION AND UPPER(C.CODE) = 'SECTION' ");
                sqlStrBuild.Append("         GROUP BY C.SUBCODE, C.DISC");


                objCmd.Connection = objCn;
                objCmd.CommandText = sqlStrBuild.ToString();
                objCmd.CommandType = System.Data.CommandType.Text;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                objAdr.Fill(objDS4);
                return objDS4.Tables[0];

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

    public DataTable ModelWiseReprot(string V_SHOP, string V_DATEFROM, string V_DATETO)
    {
        using (objCn = new OracleConnection())
        {
            objCn.ConnectionString = strConn;
            try
            {
                objCn.Open();
                objCmd = new OracleCommand();

                StringBuilder sqlStrBuild = new StringBuilder();

                sqlStrBuild.Append("        SELECT COUNT(*), D.MODELCODE, C.DISC, C.SUBCODE ");
                sqlStrBuild.Append("          FROM ");
                sqlStrBuild.Append(tblFrameWiseDefect);
                sqlStrBuild.Append(" D, ");
                sqlStrBuild.Append(tblCodeType);
                sqlStrBuild.Append(" C ");
                sqlStrBuild.Append("         WHERE  ");
                sqlStrBuild.Append("               TO_DATE(TO_CHAR(D.CREATEDATE,'MM-DD-YYYY') ||' '|| TO_CHAR(TO_DATE(D.CREATETIME,'HH24MISS'),'HH24:MI:SS'),'MM-DD-YYYY HH24:MI:SS') ");
                sqlStrBuild.Append("               BETWEEN ");
                sqlStrBuild.Append("                  TO_DATE(TO_CHAR(TO_DATE('");
                sqlStrBuild.Append(V_DATEFROM);
                sqlStrBuild.Append("', 'DD-MON-YYYY'),'MM-DD-YYYY') || ' 06:30:00','MM-DD-YYYY HH24:MI:SS') ");
                sqlStrBuild.Append("                  AND ");
                sqlStrBuild.Append("                  TO_DATE(TO_CHAR(TO_DATE('");
                sqlStrBuild.Append(V_DATETO);
                sqlStrBuild.Append("', 'DD-MON-YYYY')+1,'MM-DD-YYYY') || ' 06:29:59','MM-DD-YYYY HH24:MI:SS') ");
                sqlStrBuild.Append("           AND UPPER(C.SUBCODE) = UPPER('");
                sqlStrBuild.Append(V_SHOP);
                sqlStrBuild.Append("') ");
                sqlStrBuild.Append("           AND C.SUBCODE = D.DEFECTSECTION  AND UPPER(C.CODE) = 'SECTION' ");
                sqlStrBuild.Append("         GROUP BY C.SUBCODE, C.DISC, D.MODELCODE");


                objCmd.Connection = objCn;
                objCmd.CommandText = sqlStrBuild.ToString();
                objCmd.CommandType = System.Data.CommandType.Text;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                objAdr.Fill(objDS4);
                return objDS4.Tables[0];

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

    public DataTable DefectWiseReprot(string V_SHOP, string V_DATEFROM, string V_DATETO, string V_MODEL)
    {
        using (objCn = new OracleConnection())
        {
            objCn.ConnectionString = strConn;
            try
            {
                objCn.Open();
                objCmd = new OracleCommand();

                StringBuilder sqlStrBuild = new StringBuilder();

                sqlStrBuild.Append("           SELECT COUNT(*), D.DEFECTDESCRIPTION, D.MODELCODE, C.DISC, C.SUBCODE ");
                sqlStrBuild.Append("            FROM ");
                sqlStrBuild.Append(tblFrameWiseDefect);
                sqlStrBuild.Append(" D, ");
                sqlStrBuild.Append(tblCodeType);
                sqlStrBuild.Append(" C ");
                sqlStrBuild.Append("           WHERE  ");
                sqlStrBuild.Append("                 TO_DATE(TO_CHAR(D.CREATEDATE,'MM-DD-YYYY') ||' '|| TO_CHAR(TO_DATE(D.CREATETIME,'HH24MISS'),'HH24:MI:SS'),'MM-DD-YYYY HH24:MI:SS') ");
                sqlStrBuild.Append("                 BETWEEN ");
                sqlStrBuild.Append("                    TO_DATE(TO_CHAR(TO_DATE('");
                sqlStrBuild.Append(V_DATEFROM);
                sqlStrBuild.Append("', 'DD-MON-YYYY'),'MM-DD-YYYY') || ' 06:30:00','MM-DD-YYYY HH24:MI:SS') ");
                sqlStrBuild.Append("                    AND ");
                sqlStrBuild.Append("                    TO_DATE(TO_CHAR(TO_DATE('");
                sqlStrBuild.Append(V_DATETO);
                sqlStrBuild.Append("', 'DD-MON-YYYY')+1,'MM-DD-YYYY') || ' 06:29:59','MM-DD-YYYY HH24:MI:SS') ");
                sqlStrBuild.Append("             AND UPPER(C.SUBCODE) = UPPER('");
                sqlStrBuild.Append(V_SHOP);
                sqlStrBuild.Append("') ");
                sqlStrBuild.Append("             AND UPPER(D.MODELCODE) = UPPER('");
                sqlStrBuild.Append(V_MODEL);
                sqlStrBuild.Append("') ");
                sqlStrBuild.Append("             AND C.SUBCODE = D.DEFECTSECTION  AND UPPER(C.CODE) = 'SECTION' ");
                sqlStrBuild.Append("           GROUP BY C.SUBCODE, ");
                sqlStrBuild.Append("                    C.DISC, ");
                sqlStrBuild.Append("                    D.MODELCODE, ");
                sqlStrBuild.Append("                    D.DEFECTCODE, ");
                sqlStrBuild.Append("                    D.DEFECTDESCRIPTION ");


                objCmd.Connection = objCn;
                objCmd.CommandText = sqlStrBuild.ToString();
                objCmd.CommandType = System.Data.CommandType.Text;
                OracleDataAdapter objAdr = new OracleDataAdapter(objCmd);
                objAdr.Fill(objDS4);
                return objDS4.Tables[0];

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
