using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using ePortal.Persistence.TravelBilling.Interface;
using Oracle.ManagedDataAccess.Client;

namespace ePortal.Persistence.TravelBilling.Services
{
    public class cTravelBilling : IcTravelBilling
    {

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


        private readonly IDataManagement oDataMgmt;
        private readonly ICommonFunctions objcmn;

        public cTravelBilling(IDataManagement _oDataMgmt, ICommonFunctions _objcmn)
        {
            oDataMgmt = _oDataMgmt;
            objcmn = _objcmn;
        }
        #endregion

        // Admin
        //public DataTable GetTravelBillingListForAdmin(DateTime FromDate, DateTime ToDate, string RequestId, string TicketNo, string EmployeeCode, string SiteId, string InvoiceNo, string StatusId)
        //{
        //    try
        //    {
        //        dt = new DataTable();

        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_ADTOURREQUEST_GET";

        //        oCmd.Parameters.Add("FROMDATE_", OracleDbType.Varchar2).Value = FromDate.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("TODATE_", OracleDbType.Varchar2).Value = ToDate.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = RequestId;
        //        oCmd.Parameters.Add("TICKETNO_", OracleDbType.Varchar2).Value = TicketNo;
        //        oCmd.Parameters.Add("EMPLOYEECODE_", OracleDbType.Varchar2).Value = EmployeeCode;
        //        oCmd.Parameters.Add("SITEID_", OracleDbType.Varchar2).Value = SiteId;
        //        oCmd.Parameters.Add("INVOICENO_", OracleDbType.Varchar2).Value = InvoiceNo;
        //        oCmd.Parameters.Add("STATUSID_", OracleDbType.Varchar2).Value = StatusId;

        //        oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //        dt = oDataMgmt.GetDataTable(oCmd);

        //        return (dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public DataTable GetTravelBillingCreditNoteDebitNoteListForAdmin(DateTime FromDate, DateTime ToDate, string RequestId, string TicketNo, string EmployeeCode, string SiteId, string InvoiceNo, string StatusId)
        //{
        //    try
        //    {
        //        dt = new DataTable();

        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_ADTOURREQUEST_CRDR_GET";

        //        oCmd.Parameters.Add("FROMDATE_", OracleDbType.Varchar2).Value = FromDate.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("TODATE_", OracleDbType.Varchar2).Value = ToDate.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = RequestId;
        //        oCmd.Parameters.Add("TICKETNO_", OracleDbType.Varchar2).Value = TicketNo;
        //        oCmd.Parameters.Add("EMPLOYEECODE_", OracleDbType.Varchar2).Value = EmployeeCode;
        //        oCmd.Parameters.Add("SITEID_", OracleDbType.Varchar2).Value = SiteId;
        //        oCmd.Parameters.Add("INVOICENO_", OracleDbType.Varchar2).Value = InvoiceNo;
        //        oCmd.Parameters.Add("STATUSID_", OracleDbType.Varchar2).Value = StatusId;

        //        oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //        dt = oDataMgmt.GetDataTable(oCmd);

        //        return (dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public DataTable GetTravelBillingInvoiceData(string SearchInvoiceNo)
        //{
        //    try
        //    {
        //        dt = new DataTable();

        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_ADTOURREQUEST_CRDR_GET_INVOICEDATA";
        //        oCmd.Parameters.Add("INVOICENO_", OracleDbType.Varchar2).Value = SearchInvoiceNo;

        //        oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //        dt = oDataMgmt.GetDataTable(oCmd);

        //        return (dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public DataTable GetTravelBillingListForAdminExport(DateTime FromDate, DateTime ToDate, string RequestId, string TicketNo, string EmployeeCode, string SiteId, string InvoiceNo, string StatusId)
        //{
        //    try
        //    {
        //        dt = new DataTable();

        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_ADTOURREQUEST_GET_EXPORT";

        //        oCmd.Parameters.Add("FROMDATE_", OracleDbType.Varchar2).Value = FromDate.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("TODATE_", OracleDbType.Varchar2).Value = ToDate.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = RequestId;
        //        oCmd.Parameters.Add("TICKETNO_", OracleDbType.Varchar2).Value = TicketNo;
        //        oCmd.Parameters.Add("EMPLOYEECODE_", OracleDbType.Varchar2).Value = EmployeeCode;
        //        oCmd.Parameters.Add("SITEID_", OracleDbType.Varchar2).Value = SiteId;
        //        oCmd.Parameters.Add("INVOICENO_", OracleDbType.Varchar2).Value = InvoiceNo;
        //        oCmd.Parameters.Add("STATUSID_", OracleDbType.Varchar2).Value = StatusId;

        //        oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //        dt = oDataMgmt.GetDataTable(oCmd);

        //        return (dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public DataTable GetTravelBillingBySrNoForAdmin(int RequestId)
        //{
        //    try
        //    {
        //        dt = new DataTable();

        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_ADTOURREQUESTBYSRNO";

        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Int32).Value = RequestId;
        //        oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //        dt = oDataMgmt.GetDataTable(oCmd);

        //        return (dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public bool UpdateadTravelBillingForAdmin(ADTRAVELBILLINGFORADMINVIEW inputJson, string ModifiedBy)
        //{
        //    try
        //    {
        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_UPDATEADTOURREQUEST";

        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Int32).Value = Convert.ToInt32(inputJson.REQUESTID);
        //        oCmd.Parameters.Add("VENDOR_CODE_", OracleDbType.Varchar2).Value = inputJson.VENDOR_CODE;
        //        oCmd.Parameters.Add("VENDOR_", OracleDbType.Varchar2).Value = inputJson.VENDOR;
        //        oCmd.Parameters.Add("VENDOR_GST_NO_", OracleDbType.Varchar2).Value = inputJson.VENDOR_GST_NO;
        //        oCmd.Parameters.Add("INVOICE_NO_", OracleDbType.Varchar2).Value = inputJson.INVOICE_NO;
        //        oCmd.Parameters.Add("INVOICE_DATE_", OracleDbType.Varchar2).Value = inputJson.INVOICE_DATE?.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("RAILWAY_GST_NO_", OracleDbType.Varchar2).Value = inputJson.RAILWAY_GST_NO;
        //        oCmd.Parameters.Add("RAILWAY_INVOICE_NO_", OracleDbType.Varchar2).Value = inputJson.RAILWAY_INVOICE_NO;
        //        oCmd.Parameters.Add("RAILWAY_INVOICE_DATE_", OracleDbType.Varchar2).Value = inputJson.RAILWAY_INVOICE_DATE?.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("RAILWAY_CONVENIENCE_FEE_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.RAILWAY_CONVENIENCE_FEE);
        //        oCmd.Parameters.Add("RAILWAY_IRTC_GST_CODE_", OracleDbType.Varchar2).Value = inputJson.RAILWAY_IRTC_GST_CODE;
        //        oCmd.Parameters.Add("RAILWAY_IRTC_CGST_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.RAILWAY_IRTC_CGST);
        //        oCmd.Parameters.Add("RAILWAY_IRTC_SGST_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.RAILWAY_IRTC_SGST);
        //        oCmd.Parameters.Add("RAILWAY_IRTC_IGST_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.RAILWAY_IRTC_IGST);
        //        oCmd.Parameters.Add("INVOICE_DOCUMENT_", OracleDbType.Varchar2).Value = inputJson.INVOICE_DOCUMENT;
        //        oCmd.Parameters.Add("BASE_FARE_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.BASE_FARE);
        //        oCmd.Parameters.Add("YQ_YR_TAX_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.YQ_YR_TAX);
        //        oCmd.Parameters.Add("TDS_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.TDS);
        //        oCmd.Parameters.Add("PSF_IRTC_CHARGE_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.PSF_IRTC_CHARGE);
        //        oCmd.Parameters.Add("SERVICE_SAC_", OracleDbType.Varchar2).Value = inputJson.SERVICE_SAC;
        //        oCmd.Parameters.Add("SERVICE_CHARGE_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.SERVICE_CHARGE);
        //        oCmd.Parameters.Add("SERVICE_CHARGE_GST_CODE_", OracleDbType.Varchar2).Value = inputJson.SERVICE_CHARGE_GST_CODE;
        //        oCmd.Parameters.Add("SERVICE_CHARGE_CGST_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.SERVICE_CHARGE_CGST);
        //        oCmd.Parameters.Add("SERVICE_CHARGE_SGST_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.SERVICE_CHARGE_SGST);
        //        oCmd.Parameters.Add("SERVICE_CHARGE_IGST_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.SERVICE_CHARGE_IGST);
        //        oCmd.Parameters.Add("GST_CODE_", OracleDbType.Varchar2).Value = inputJson.GST_CODE;
        //        oCmd.Parameters.Add("CGST_AMOUNT_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.CGST_AMOUNT);
        //        oCmd.Parameters.Add("SGST_AMOUNT_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.SGST_AMOUNT);
        //        oCmd.Parameters.Add("IGST_AMOUNT_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.IGST_AMOUNT);
        //        oCmd.Parameters.Add("SAC_CODE_", OracleDbType.Varchar2).Value = inputJson.SAC_CODE;
        //        oCmd.Parameters.Add("OTHER_DOCUMENT_", OracleDbType.Varchar2).Value = inputJson.OTHER_DOCUMENT;
        //        oCmd.Parameters.Add("COST_CENTER_", OracleDbType.Varchar2).Value = inputJson.COST_CENTER;
        //        oCmd.Parameters.Add("DISCOUNT_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.DISCOUNT);
        //        oCmd.Parameters.Add("ROUND_OFF_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.ROUND_OFF);
        //        oCmd.Parameters.Add("INVOICE_AMOUNT_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.INVOICE_AMOUNT);
        //        oCmd.Parameters.Add("TOTAL_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.TOTAL);
        //        oCmd.Parameters.Add("DESCRIPTION_", OracleDbType.Varchar2).Value = inputJson.DESCRIPTION;
        //        oCmd.Parameters.Add("MODIFIEDBY_", OracleDbType.Varchar2).Value = ModifiedBy;
        //        oCmd.Parameters.Add("BILLINGSYPLANTID_", OracleDbType.Varchar2).Value = inputJson.BILLINGSYPLANTID;
        //        oCmd.Parameters.Add("BILLINGSYSITEID_", OracleDbType.Varchar2).Value = inputJson.BILLINGSYSITEID;
        //        oCmd.Parameters.Add("RAILWAYOTHERCHARGE_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.RAILWAYOTHERCHARGE);
        //        oCmd.Parameters.Add("IRCTCCHARGE_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.IRCTCCHARGE);
        //        oCmd.Parameters.Add("AIRINVOICEDETAIL_", OracleDbType.Varchar2).Value = inputJson.AIRINVOICEDETAIL;
        //        oCmd.Parameters.Add("OTHERTAX_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.OTHERTAX);
        //        oCmd.Parameters.Add("ACTUALINVOICEAMOUNT_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.ACTUALINVOICEAMOUNT);

        //        oDataMgmt.ExecuteQuery(oCmd);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public bool UpdateadTravelBillingForCreditNoteDebitNoteAdmin(ADTRAVELBILLINGFORADMINVIEW inputJson, string ModifiedBy)
        //{
        //    try
        //    {
        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_INUPCREDITDEBITNOTE";

        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Int32).Value = Convert.ToInt32(inputJson.REQUESTID);
        //        oCmd.Parameters.Add("VENDOR_CODE_", OracleDbType.Varchar2).Value = inputJson.VENDOR_CODE;
        //        oCmd.Parameters.Add("VENDOR_", OracleDbType.Varchar2).Value = inputJson.VENDOR;
        //        oCmd.Parameters.Add("VENDOR_GST_NO_", OracleDbType.Varchar2).Value = inputJson.VENDOR_GST_NO;
        //        oCmd.Parameters.Add("INVOICE_NO_", OracleDbType.Varchar2).Value = inputJson.INVOICE_NO;
        //        oCmd.Parameters.Add("INVOICE_DATE_", OracleDbType.Varchar2).Value = inputJson.INVOICE_DATE?.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("RAILWAY_GST_NO_", OracleDbType.Varchar2).Value = inputJson.RAILWAY_GST_NO;
        //        oCmd.Parameters.Add("RAILWAY_INVOICE_NO_", OracleDbType.Varchar2).Value = inputJson.RAILWAY_INVOICE_NO;
        //        oCmd.Parameters.Add("RAILWAY_INVOICE_DATE_", OracleDbType.Varchar2).Value = inputJson.RAILWAY_INVOICE_DATE?.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("RAILWAY_CONVENIENCE_FEE_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.RAILWAY_CONVENIENCE_FEE);
        //        oCmd.Parameters.Add("RAILWAY_IRTC_GST_CODE_", OracleDbType.Varchar2).Value = inputJson.RAILWAY_IRTC_GST_CODE;
        //        oCmd.Parameters.Add("RAILWAY_IRTC_CGST_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.RAILWAY_IRTC_CGST);
        //        oCmd.Parameters.Add("RAILWAY_IRTC_SGST_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.RAILWAY_IRTC_SGST);
        //        oCmd.Parameters.Add("RAILWAY_IRTC_IGST_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.RAILWAY_IRTC_IGST);
        //        oCmd.Parameters.Add("INVOICE_DOCUMENT_", OracleDbType.Varchar2).Value = inputJson.INVOICE_DOCUMENT;
        //        oCmd.Parameters.Add("BASE_FARE_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.BASE_FARE);
        //        oCmd.Parameters.Add("YQ_YR_TAX_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.YQ_YR_TAX);
        //        oCmd.Parameters.Add("TDS_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.TDS);
        //        oCmd.Parameters.Add("PSF_IRTC_CHARGE_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.PSF_IRTC_CHARGE);
        //        oCmd.Parameters.Add("SERVICE_SAC_", OracleDbType.Varchar2).Value = inputJson.SERVICE_SAC;
        //        oCmd.Parameters.Add("SERVICE_CHARGE_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.SERVICE_CHARGE);
        //        oCmd.Parameters.Add("SERVICE_CHARGE_GST_CODE_", OracleDbType.Varchar2).Value = inputJson.SERVICE_CHARGE_GST_CODE;
        //        oCmd.Parameters.Add("SERVICE_CHARGE_CGST_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.SERVICE_CHARGE_CGST);
        //        oCmd.Parameters.Add("SERVICE_CHARGE_SGST_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.SERVICE_CHARGE_SGST);
        //        oCmd.Parameters.Add("SERVICE_CHARGE_IGST_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.SERVICE_CHARGE_IGST);
        //        oCmd.Parameters.Add("GST_CODE_", OracleDbType.Varchar2).Value = inputJson.GST_CODE;
        //        oCmd.Parameters.Add("CGST_AMOUNT_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.CGST_AMOUNT);
        //        oCmd.Parameters.Add("SGST_AMOUNT_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.SGST_AMOUNT);
        //        oCmd.Parameters.Add("IGST_AMOUNT_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.IGST_AMOUNT);
        //        oCmd.Parameters.Add("SAC_CODE_", OracleDbType.Varchar2).Value = inputJson.SAC_CODE;
        //        oCmd.Parameters.Add("OTHER_DOCUMENT_", OracleDbType.Varchar2).Value = inputJson.OTHER_DOCUMENT;
        //        oCmd.Parameters.Add("COST_CENTER_", OracleDbType.Varchar2).Value = inputJson.COST_CENTER;
        //        oCmd.Parameters.Add("DISCOUNT_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.DISCOUNT);
        //        oCmd.Parameters.Add("ROUND_OFF_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.ROUND_OFF);
        //        oCmd.Parameters.Add("INVOICE_AMOUNT_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.INVOICE_AMOUNT);
        //        oCmd.Parameters.Add("TOTAL_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.TOTAL);
        //        oCmd.Parameters.Add("DESCRIPTION_", OracleDbType.Varchar2).Value = inputJson.DESCRIPTION;
        //        oCmd.Parameters.Add("MODIFIEDBY_", OracleDbType.Varchar2).Value = ModifiedBy;
        //        oCmd.Parameters.Add("BILLINGSYPLANTID_", OracleDbType.Varchar2).Value = inputJson.BILLINGSYPLANTID;
        //        oCmd.Parameters.Add("BILLINGSYSITEID_", OracleDbType.Varchar2).Value = inputJson.BILLINGSYSITEID;
        //        oCmd.Parameters.Add("RAILWAYOTHERCHARGE_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.RAILWAYOTHERCHARGE);
        //        oCmd.Parameters.Add("IRCTCCHARGE_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.IRCTCCHARGE);
        //        oCmd.Parameters.Add("AIRINVOICEDETAIL_", OracleDbType.Varchar2).Value = inputJson.AIRINVOICEDETAIL;
        //        oCmd.Parameters.Add("TRANSACTIONTYPE_", OracleDbType.Varchar2).Value = inputJson.TRANSACTIONTYPE;
        //        oCmd.Parameters.Add("CRDRREFNO_", OracleDbType.Varchar2).Value = inputJson.CRDRREFNO;
        //        oCmd.Parameters.Add("ADDEDBY_", OracleDbType.Varchar2).Value = ModifiedBy;
        //        oCmd.Parameters.Add("OTHERTAX_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.OTHERTAX);
        //        oCmd.Parameters.Add("ACTUALINVOICEAMOUNT_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.ACTUALINVOICEAMOUNT);
        //        oCmd.Parameters.Add("TRANFLAG_", OracleDbType.Varchar2).Value = inputJson.TranFlag;
        //        oDataMgmt.ExecuteQuery(oCmd);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public DataTable CheckDuplicateCreditNoteDebitNote(ADTRAVELBILLINGFORADMINVIEW inputJson, string ModifiedBy)
        //{
        //    try
        //    {
        //        dt = new DataTable();
        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_CHECKDUPLICATECREDITDEBITNOTE";
        //        oCmd.Parameters.Add("INVOICE_NO_", OracleDbType.Varchar2).Value = inputJson.INVOICE_NO;
        //        oCmd.Parameters.Add("CRDRREFNO_", OracleDbType.Varchar2).Value = inputJson.CRDRREFNO;
        //        oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //        dt = oDataMgmt.GetDataTable(oCmd);

        //        return (dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public bool TravelBillingSubmitDataForAdmin(string SrNo, string ModifiedBy)
        //{
        //    try
        //    {
        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_TRAVELBILLINGSUBMITDATAFORADMIN";

        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = SrNo;
        //        oCmd.Parameters.Add("MODIFIEDBY_", OracleDbType.Varchar2).Value = ModifiedBy;

        //        oDataMgmt.ExecuteQuery(oCmd);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        ////HMSI Admin
        //public DataTable GetTravelBillingListForHMSIAdmin(DateTime FromDate, DateTime ToDate, string RequestId, string TicketNo, string EmployeeCode, string SiteId, string InvoiceNo, string StatusId, string TranasactionType)
        //{
        //    try
        //    {
        //        dt = new DataTable();

        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_ADTOURREQUEST_GETFORHMSIADMIN";

        //        oCmd.Parameters.Add("FROMDATE_", OracleDbType.Varchar2).Value = FromDate.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("TODATE_", OracleDbType.Varchar2).Value = ToDate.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = RequestId;
        //        oCmd.Parameters.Add("TICKETNO_", OracleDbType.Varchar2).Value = TicketNo;
        //        oCmd.Parameters.Add("EMPLOYEECODE_", OracleDbType.Varchar2).Value = EmployeeCode;
        //        oCmd.Parameters.Add("SITEID_", OracleDbType.Varchar2).Value = SiteId;
        //        oCmd.Parameters.Add("INVOICENO_", OracleDbType.Varchar2).Value = InvoiceNo;
        //        oCmd.Parameters.Add("STATUSID_", OracleDbType.Varchar2).Value = StatusId;
        //        oCmd.Parameters.Add("TRANSACTIONTYPE_", OracleDbType.Varchar2).Value = TranasactionType;

        //        oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //        dt = oDataMgmt.GetDataTable(oCmd);

        //        return (dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public DataTable GetTravelBillingReportForHMSIAdmin(DateTime FromDate, DateTime ToDate, string RequestId, string TicketNo, string EmployeeCode, string SiteId, string InvoiceNo, string StatusId, string TranasactionType)
        //{
        //    try
        //    {
        //        dt = new DataTable();

        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_GETREPORTDATAFORHMSIADMIN";

        //        oCmd.Parameters.Add("FROMDATE_", OracleDbType.Varchar2).Value = FromDate.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("TODATE_", OracleDbType.Varchar2).Value = ToDate.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = RequestId;
        //        oCmd.Parameters.Add("TICKETNO_", OracleDbType.Varchar2).Value = TicketNo;
        //        oCmd.Parameters.Add("EMPLOYEECODE_", OracleDbType.Varchar2).Value = EmployeeCode;
        //        oCmd.Parameters.Add("SITEID_", OracleDbType.Varchar2).Value = SiteId;
        //        oCmd.Parameters.Add("INVOICENO_", OracleDbType.Varchar2).Value = InvoiceNo;
        //        oCmd.Parameters.Add("STATUSID_", OracleDbType.Varchar2).Value = StatusId;
        //        oCmd.Parameters.Add("TRANSACTIONTYPE_", OracleDbType.Varchar2).Value = TranasactionType;

        //        oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //        dt = oDataMgmt.GetDataTable(oCmd);

        //        return (dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public DataTable GetTravelBillingReportForFinance(DateTime FromDate, DateTime ToDate, string RequestId, string TicketNo, string EmployeeCode, string SiteId, string InvoiceNo, string StatusId, string TranasactionType)
        //{
        //    try
        //    {
        //        dt = new DataTable();

        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_GETREPORTDATAFORFINANCE";

        //        oCmd.Parameters.Add("FROMDATE_", OracleDbType.Varchar2).Value = FromDate.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("TODATE_", OracleDbType.Varchar2).Value = ToDate.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = RequestId;
        //        oCmd.Parameters.Add("TICKETNO_", OracleDbType.Varchar2).Value = TicketNo;
        //        oCmd.Parameters.Add("EMPLOYEECODE_", OracleDbType.Varchar2).Value = EmployeeCode;
        //        oCmd.Parameters.Add("SITEID_", OracleDbType.Varchar2).Value = SiteId;
        //        oCmd.Parameters.Add("INVOICENO_", OracleDbType.Varchar2).Value = InvoiceNo;
        //        oCmd.Parameters.Add("STATUSID_", OracleDbType.Varchar2).Value = StatusId;
        //        oCmd.Parameters.Add("TRANSACTIONTYPE_", OracleDbType.Varchar2).Value = TranasactionType;

        //        oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //        dt = oDataMgmt.GetDataTable(oCmd);

        //        return (dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public bool TravelBillingSubmitDataForHMSIAdmin(string SrNo, string ModifiedBy)
        //{
        //    try
        //    {
        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_TRAVELBILLINGSUBMITDATAFORHMSIADMIN";

        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = SrNo;
        //        oCmd.Parameters.Add("MODIFIEDBY_", OracleDbType.Varchar2).Value = ModifiedBy;

        //        oDataMgmt.ExecuteQuery(oCmd);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public bool TravelBillingSendBackDataForHMSIAdmin(string SrNo, string ModifiedBy)
        //{
        //    try
        //    {
        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_TRAVELBILLINGSENDBACKDATAFORHMSIADMIN";

        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = SrNo;
        //        oCmd.Parameters.Add("MODIFIEDBY_", OracleDbType.Varchar2).Value = ModifiedBy;

        //        oDataMgmt.ExecuteQuery(oCmd);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ////Admin Cheque Request
        //public DataTable GetAdminChequeRequestList(DateTime FromDate, DateTime ToDate, string RequestId, string TicketNo, string EmployeeCode, string LocationId, string SiteId, string InvoiceNo, string StatusId, string TranasactionType)
        //{
        //    try
        //    {
        //        dt = new DataTable();

        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_ADTOURREQUEST_GETFORADMINCHEQUEREQUEST";

        //        oCmd.Parameters.Add("FROMDATE_", OracleDbType.Varchar2).Value = FromDate.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("TODATE_", OracleDbType.Varchar2).Value = ToDate.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = RequestId;
        //        oCmd.Parameters.Add("TICKETNO_", OracleDbType.Varchar2).Value = TicketNo;
        //        oCmd.Parameters.Add("EMPLOYEECODE_", OracleDbType.Varchar2).Value = EmployeeCode;
        //        oCmd.Parameters.Add("LOCATIONID_", OracleDbType.Varchar2).Value = LocationId;
        //        oCmd.Parameters.Add("SITEID_", OracleDbType.Varchar2).Value = SiteId;
        //        oCmd.Parameters.Add("INVOICENO_", OracleDbType.Varchar2).Value = InvoiceNo;
        //        oCmd.Parameters.Add("STATUSID_", OracleDbType.Varchar2).Value = StatusId;
        //        oCmd.Parameters.Add("TRANSACTIONTYPE_", OracleDbType.Varchar2).Value = TranasactionType;

        //        oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //        dt = oDataMgmt.GetDataTable(oCmd);

        //        return (dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public bool GenerateChequeRequest(string SrNo, string ModifiedBy)
        //{
        //    try
        //    {
        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_TRAVELBILLINGGENERATECHEQUEREQUEST";

        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = SrNo;
        //        oCmd.Parameters.Add("MODIFIEDBY_", OracleDbType.Varchar2).Value = ModifiedBy;

        //        oDataMgmt.ExecuteQuery(oCmd);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ////Cheque Request Approval
        //public DataTable GetChequeRequestApprovalList(DateTime FromDate, DateTime ToDate, string RequestId, string InvoiceNo, string EmployeeCode, string LocationId, string SiteId, string StatusId)
        //{
        //    try
        //    {
        //        dt = new DataTable();

        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_ADTOURREQUEST_GETCHEQUEREQAPPROVA";

        //        oCmd.Parameters.Add("FROMDATE_", OracleDbType.Varchar2).Value = FromDate.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("TODATE_", OracleDbType.Varchar2).Value = ToDate.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = RequestId;
        //        oCmd.Parameters.Add("INVOICENO_", OracleDbType.Varchar2).Value = InvoiceNo;
        //        oCmd.Parameters.Add("EMPLOYEECODE_", OracleDbType.Varchar2).Value = EmployeeCode;
        //        oCmd.Parameters.Add("LOCATIONID_", OracleDbType.Varchar2).Value = LocationId;
        //        oCmd.Parameters.Add("SITEID_", OracleDbType.Varchar2).Value = SiteId;
        //        oCmd.Parameters.Add("STATUSID_", OracleDbType.Varchar2).Value = StatusId;

        //        oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //        dt = oDataMgmt.GetDataTable(oCmd);

        //        return (dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public DataTable GetChequeRequestBySrNoForAdmin(string RequestId)
        //{
        //    try
        //    {
        //        dt = new DataTable();

        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_ADCHEQUEREQUESTBYSRNO";

        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = RequestId;
        //        oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //        dt = oDataMgmt.GetDataTable(oCmd);

        //        return (dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public DataSet GetPrintChequeRequest(string RequestId)
        //{
        //    try
        //    {
        //        ds = new DataSet();

        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_GETPRINTCHEQUEREQUEST";

        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = RequestId;
        //        oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        //        oCmd.Parameters.Add("CUR_INTIATOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        //        oCmd.Parameters.Add("CUR_APPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        //        oCmd.Parameters.Add("CUR_FINANCE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //        ds = oDataMgmt.GetDataSet(oCmd);

        //        return (ds);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public DataSet GetPrintSummary(string RequestId)
        //{
        //    try
        //    {
        //        ds = new DataSet();

        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_GETPRINTSUMMARY";

        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = RequestId;
        //        oCmd.Parameters.Add("CUR_TBL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //        ds = oDataMgmt.GetDataSet(oCmd);

        //        return (ds);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public DataTable GetTourBillingRequestByChequeRequestNo(string id)
        //{
        //    try
        //    {
        //        dt = new DataTable();

        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_ADTOURBILLINGREQUESTBYCHEQUEREQ";

        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = id;

        //        oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //        dt = oDataMgmt.GetDataTable(oCmd);

        //        return (dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public DataTable GetChequeRequestApprovalById(string id)
        //{
        //    try
        //    {
        //        dt = new DataTable();

        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_ADCHEQUEREQAPPROVALBYCHEQUEREQ";

        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = id;

        //        oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //        dt = oDataMgmt.GetDataTable(oCmd);

        //        return (dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public bool DeleteadTourBillingReqForChequeReq(string SrNo, string ChequeReqID, string ModifiedBy)
        //{
        //    try
        //    {
        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_DELETEADTOURBILLINGREQFORCHEQUEREQ";

        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = SrNo;
        //        oCmd.Parameters.Add("CHEQUEREQID_", OracleDbType.Varchar2).Value = ChequeReqID;
        //        oCmd.Parameters.Add("MODIFIEDBY_", OracleDbType.Varchar2).Value = ModifiedBy;

        //        oDataMgmt.ExecuteQuery(oCmd);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public bool SaveChequeRequestApproval(ADTRAVELBILLINGFORCHEQUEREQ inputJson, string xmlString, string ModifiedBy)
        //{
        //    try
        //    {
        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_SAVECHEQUEREQAPPROVAL";

        //        oCmd.Parameters.Add("CHEQUEREQID_", OracleDbType.Varchar2).Value = inputJson.REQUESTID;
        //        oCmd.Parameters.Add("DESCRIPTIONS_", OracleDbType.Varchar2).Value = inputJson.DESCRIPTIONS;
        //        oCmd.Parameters.Add("BUDGETED_", OracleDbType.Varchar2).Value = inputJson.BUDGETED;
        //        oCmd.Parameters.Add("INVOICE_NO_", OracleDbType.Varchar2).Value = inputJson.INVOICE_NO;
        //        oCmd.Parameters.Add("INVOICE_DATE_", OracleDbType.Varchar2).Value = inputJson.INVOICE_DATE?.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("REV_CHARGE_", OracleDbType.Varchar2).Value = inputJson.REV_CHARGE;
        //        oCmd.Parameters.Add("SAC_CODE_", OracleDbType.Varchar2).Value = inputJson.SAC_CODE;
        //        oCmd.Parameters.Add("GST_CODE_", OracleDbType.Varchar2).Value = inputJson.GST_CODE;
        //        oCmd.Parameters.Add("REMARKS_", OracleDbType.Varchar2).Value = inputJson.REMARKS;
        //        oCmd.Parameters.Add("APPROVALTYPE_", OracleDbType.Varchar2).Value = inputJson.APPROVALTYPE;
        //        oCmd.Parameters.Add("APPROVALDATA", OracleDbType.Varchar2).Value = xmlString;
        //        oCmd.Parameters.Add("MODIFIEDBY_", OracleDbType.Varchar2).Value = ModifiedBy;
        //        oCmd.Parameters.Add("ADMINDOCUMENT_", OracleDbType.Varchar2).Value = inputJson.ADMINDOCUMENT;

        //        oDataMgmt.ExecuteQuery(oCmd);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public bool SendBackChequeRequestApproval(string RequestId, string ModifiedBy)
        //{
        //    try
        //    {
        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_SENDBACKCHEQUEREQAPPROVAL";

        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = RequestId;
        //        oCmd.Parameters.Add("MODIFIEDBY_", OracleDbType.Varchar2).Value = ModifiedBy;

        //        oDataMgmt.ExecuteQuery(oCmd);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public DataTable GetHeadList(int UserId, string RequestId)
        //{
        //    try
        //    {
        //        dt = new DataTable();

        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_ADGETHEADLIST";

        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = RequestId;
        //        oCmd.Parameters.Add("USERID", OracleDbType.Varchar2).Value = UserId;
        //        oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //        dt = oDataMgmt.GetDataTable(oCmd);

        //        return (dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ////Request Approval
        //public DataTable GetRequestApprovalList(DateTime FromDate, DateTime ToDate, string RequestId, string InvoiceNo, string EmployeeCode, string LocationId, string SiteId, string StatusId, string UserId)
        //{
        //    try
        //    {
        //        dt = new DataTable();

        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_ADTOURREQUEST_GETREQUESTAPPROVAL";

        //        oCmd.Parameters.Add("FROMDATE_", OracleDbType.Varchar2).Value = FromDate.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("TODATE_", OracleDbType.Varchar2).Value = ToDate.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = RequestId;
        //        oCmd.Parameters.Add("INVOICENO_", OracleDbType.Varchar2).Value = InvoiceNo;
        //        oCmd.Parameters.Add("EMPLOYEECODE_", OracleDbType.Varchar2).Value = EmployeeCode;
        //        oCmd.Parameters.Add("LOCATIONID_", OracleDbType.Varchar2).Value = LocationId;
        //        oCmd.Parameters.Add("SITEID_", OracleDbType.Varchar2).Value = SiteId;
        //        oCmd.Parameters.Add("USERID_", OracleDbType.Varchar2).Value = UserId;
        //        oCmd.Parameters.Add("STATUSID_", OracleDbType.Varchar2).Value = StatusId;

        //        oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //        dt = oDataMgmt.GetDataTable(oCmd);

        //        return (dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public DataTable GetRequestApprovalBySrNo(string RequestId, string UserId)
        //{
        //    try
        //    {
        //        dt = new DataTable();

        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_ADREQUESTAPPROVALBYSRNO";

        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = RequestId;
        //        oCmd.Parameters.Add("EMP_CODE_", OracleDbType.Varchar2).Value = UserId;
        //        oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //        dt = oDataMgmt.GetDataTable(oCmd);

        //        return (dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public DataTable GetRequestApprovalById(string id)
        //{
        //    try
        //    {
        //        dt = new DataTable();

        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_ADREQAPPROVALBYREQUESTID";

        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = id;

        //        oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //        dt = oDataMgmt.GetDataTable(oCmd);

        //        return (dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public bool SubmitRequestApproval(string REQUESTID, int RNUM, int SRNO, string REMARKS, int STATUS, string USERID)
        //{
        //    try
        //    {
        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_SUBMITREQUESTAPPROVAL";

        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = REQUESTID;
        //        oCmd.Parameters.Add("RNUM_", OracleDbType.Int32).Value = RNUM;
        //        oCmd.Parameters.Add("SRNO_", OracleDbType.Int32).Value = SRNO;
        //        oCmd.Parameters.Add("REMARKS_", OracleDbType.Varchar2).Value = REMARKS;
        //        oCmd.Parameters.Add("STATUS_", OracleDbType.Int32).Value = STATUS;
        //        oCmd.Parameters.Add("USERID_", OracleDbType.Varchar2).Value = USERID;

        //        oDataMgmt.ExecuteQuery(oCmd);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //Finance Request Approval
        public DataTable ChequeRequestApprovalList(long UserId)
        {
            try
            {
                dt = new DataTable();

                OracleCommand oCmd = new OracleCommand();
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.BindByName = true;
                oCmd.CommandText = "PKG_TOURBILLING.SPROC_CHEQUEREQUESTAPPROVALLIST";

                oCmd.Parameters.Add("USERID", OracleDbType.Long).Value = UserId;
                oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                dt = oDataMgmt.GetDataTable(oCmd);

                return (dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public DataTable GetFinanceApprovalList(DateTime FromDate, DateTime ToDate, string RequestId, string InvoiceNo, string EmployeeCode, string LocationId, string SiteId, string StatusId)
        //{
        //    try
        //    {
        //        dt = new DataTable();

        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_ADTOURREQUEST_GETFINANCEAPPROVA";

        //        oCmd.Parameters.Add("FROMDATE_", OracleDbType.Varchar2).Value = FromDate.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("TODATE_", OracleDbType.Varchar2).Value = ToDate.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = RequestId;
        //        oCmd.Parameters.Add("INVOICENO_", OracleDbType.Varchar2).Value = InvoiceNo;
        //        oCmd.Parameters.Add("EMPLOYEECODE_", OracleDbType.Varchar2).Value = EmployeeCode;
        //        oCmd.Parameters.Add("LOCATIONID_", OracleDbType.Varchar2).Value = LocationId;
        //        oCmd.Parameters.Add("SITEID_", OracleDbType.Varchar2).Value = SiteId;
        //        oCmd.Parameters.Add("STATUSID_", OracleDbType.Varchar2).Value = StatusId;

        //        oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //        dt = oDataMgmt.GetDataTable(oCmd);

        //        return (dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public DataTable GetFinanceApprovalBySrNo(string RequestId)
        //{
        //    try
        //    {
        //        dt = new DataTable();

        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_ADGETFINANCEAPPROVABYSRNO";

        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = RequestId;
        //        oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //        dt = oDataMgmt.GetDataTable(oCmd);

        //        return (dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public DataTable GetTourBillingRequestForFinance(string id)
        //{
        //    try
        //    {
        //        dt = new DataTable();

        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_ADFINANCEREQUESTBYCHEQUEREQ";

        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = id;

        //        oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //        dt = oDataMgmt.GetDataTable(oCmd);

        //        return (dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public bool UpdateadTravelBillingForFinance(ADTRAVELBILLINGFORADMINVIEW inputJson, string ModifiedBy)
        //{
        //    try
        //    {
        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_UPDATEADTOURREQBYFINANCE";

        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Int32).Value = Convert.ToInt32(inputJson.REQUESTID);
        //        oCmd.Parameters.Add("VENDOR_CODE_", OracleDbType.Varchar2).Value = inputJson.VENDOR_CODE;
        //        oCmd.Parameters.Add("VENDOR_", OracleDbType.Varchar2).Value = inputJson.VENDOR;
        //        oCmd.Parameters.Add("VENDOR_GST_NO_", OracleDbType.Varchar2).Value = inputJson.VENDOR_GST_NO;
        //        oCmd.Parameters.Add("INVOICE_NO_", OracleDbType.Varchar2).Value = inputJson.INVOICE_NO;
        //        oCmd.Parameters.Add("INVOICE_DATE_", OracleDbType.Varchar2).Value = inputJson.INVOICE_DATE?.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("RAILWAY_GST_NO_", OracleDbType.Varchar2).Value = inputJson.RAILWAY_GST_NO;
        //        oCmd.Parameters.Add("RAILWAY_INVOICE_NO_", OracleDbType.Varchar2).Value = inputJson.RAILWAY_INVOICE_NO;
        //        oCmd.Parameters.Add("RAILWAY_INVOICE_DATE_", OracleDbType.Varchar2).Value = inputJson.RAILWAY_INVOICE_DATE?.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("RAILWAY_CONVENIENCE_FEE_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.RAILWAY_CONVENIENCE_FEE);
        //        oCmd.Parameters.Add("RAILWAY_IRTC_GST_CODE_", OracleDbType.Varchar2).Value = inputJson.RAILWAY_IRTC_GST_CODE;
        //        oCmd.Parameters.Add("RAILWAY_IRTC_CGST_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.RAILWAY_IRTC_CGST);
        //        oCmd.Parameters.Add("RAILWAY_IRTC_SGST_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.RAILWAY_IRTC_SGST);
        //        oCmd.Parameters.Add("RAILWAY_IRTC_IGST_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.RAILWAY_IRTC_IGST);
        //        oCmd.Parameters.Add("INVOICE_DOCUMENT_", OracleDbType.Varchar2).Value = inputJson.INVOICE_DOCUMENT;
        //        oCmd.Parameters.Add("BASE_FARE_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.BASE_FARE);
        //        oCmd.Parameters.Add("YQ_YR_TAX_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.YQ_YR_TAX);
        //        oCmd.Parameters.Add("TDS_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.TDS);
        //        oCmd.Parameters.Add("PSF_IRTC_CHARGE_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.PSF_IRTC_CHARGE);
        //        oCmd.Parameters.Add("SERVICE_SAC_", OracleDbType.Varchar2).Value = inputJson.SERVICE_SAC;
        //        oCmd.Parameters.Add("SERVICE_CHARGE_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.SERVICE_CHARGE);
        //        oCmd.Parameters.Add("SERVICE_CHARGE_GST_CODE_", OracleDbType.Varchar2).Value = inputJson.SERVICE_CHARGE_GST_CODE;
        //        oCmd.Parameters.Add("SERVICE_CHARGE_CGST_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.SERVICE_CHARGE_CGST);
        //        oCmd.Parameters.Add("SERVICE_CHARGE_SGST_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.SERVICE_CHARGE_SGST);
        //        oCmd.Parameters.Add("SERVICE_CHARGE_IGST_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.SERVICE_CHARGE_IGST);
        //        oCmd.Parameters.Add("GST_CODE_", OracleDbType.Varchar2).Value = inputJson.GST_CODE;
        //        oCmd.Parameters.Add("CGST_AMOUNT_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.CGST_AMOUNT);
        //        oCmd.Parameters.Add("SGST_AMOUNT_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.SGST_AMOUNT);
        //        oCmd.Parameters.Add("IGST_AMOUNT_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.IGST_AMOUNT);
        //        oCmd.Parameters.Add("SAC_CODE_", OracleDbType.Varchar2).Value = inputJson.SAC_CODE;
        //        oCmd.Parameters.Add("OTHER_DOCUMENT_", OracleDbType.Varchar2).Value = inputJson.OTHER_DOCUMENT;
        //        oCmd.Parameters.Add("COST_CENTER_", OracleDbType.Varchar2).Value = inputJson.COST_CENTER;
        //        oCmd.Parameters.Add("DISCOUNT_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.DISCOUNT);
        //        oCmd.Parameters.Add("ROUND_OFF_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.ROUND_OFF);
        //        oCmd.Parameters.Add("INVOICE_AMOUNT_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.INVOICE_AMOUNT);
        //        oCmd.Parameters.Add("TOTAL_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.TOTAL);
        //        oCmd.Parameters.Add("DESCRIPTION_", OracleDbType.Varchar2).Value = inputJson.DESCRIPTION;
        //        oCmd.Parameters.Add("MODIFIEDBY_", OracleDbType.Varchar2).Value = ModifiedBy;
        //        oCmd.Parameters.Add("BILLINGSYPLANTID_", OracleDbType.Varchar2).Value = inputJson.BILLINGSYPLANTID;
        //        oCmd.Parameters.Add("BILLINGSYSITEID_", OracleDbType.Varchar2).Value = inputJson.BILLINGSYSITEID;
        //        oCmd.Parameters.Add("RAILWAYOTHERCHARGE_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.RAILWAYOTHERCHARGE);
        //        oCmd.Parameters.Add("IRCTCCHARGE_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.IRCTCCHARGE);
        //        oCmd.Parameters.Add("AIRINVOICEDETAIL_", OracleDbType.Varchar2).Value = inputJson.AIRINVOICEDETAIL;
        //        oCmd.Parameters.Add("TRANSACTIONTYPE_", OracleDbType.Varchar2).Value = inputJson.TRANSACTIONTYPE;
        //        oCmd.Parameters.Add("CRDRREFNO_", OracleDbType.Varchar2).Value = inputJson.CRDRREFNO;
        //        oCmd.Parameters.Add("OTHERTAX_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.OTHERTAX);
        //        oCmd.Parameters.Add("ACTUALINVOICEAMOUNT_", OracleDbType.Decimal).Value = Convert.ToDecimal(inputJson.ACTUALINVOICEAMOUNT);
        //        oDataMgmt.ExecuteQuery(oCmd);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public bool SaveApprovalFinance(ADFINANCEDATA inputJson, string ModifiedBy)
        //{
        //    try
        //    {
        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_APPROVALFINANCE";

        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = inputJson.REQUESTID;
        //        oCmd.Parameters.Add("POSINGDATE_", OracleDbType.Varchar2).Value = inputJson.POSINGDATE?.ToString("dd-MMM-yyyy");
        //        oCmd.Parameters.Add("HEADERTEXT_", OracleDbType.Varchar2).Value = inputJson.HEADERTEXT;
        //        oCmd.Parameters.Add("OTHER_DOCUMENT_", OracleDbType.Varchar2).Value = inputJson.OTHER_DOCUMENT;
        //        oCmd.Parameters.Add("BUDGETED_", OracleDbType.Varchar2).Value = inputJson.BUDGETED;
        //        oCmd.Parameters.Add("TDS_DEDUCTION_", OracleDbType.Decimal).Value = string.IsNullOrEmpty(inputJson.TDSDEDUCTION) ? 0 : decimal.Parse(inputJson.TDSDEDUCTION.Trim());
        //        oCmd.Parameters.Add("ADVANCE_ADJ_", OracleDbType.Decimal).Value = string.IsNullOrEmpty(inputJson.ADVANCEADJ) ? 0 : decimal.Parse(inputJson.ADVANCEADJ.Trim());
        //        oCmd.Parameters.Add("REV_CHARGE_", OracleDbType.Varchar2).Value = inputJson.REVERSECHARGE;
        //        oCmd.Parameters.Add("SAC_CODE_", OracleDbType.Varchar2).Value = inputJson.SACCODE;
        //        oCmd.Parameters.Add("GST_CODE_", OracleDbType.Varchar2).Value = inputJson.GST_CODE;
        //        oCmd.Parameters.Add("REMARKS_", OracleDbType.Varchar2).Value = inputJson.REMARKS;
        //        oCmd.Parameters.Add("APPROVALS_SRNO_", OracleDbType.Varchar2).Value = inputJson.ApprovalsSrNo;
        //        oCmd.Parameters.Add("SENDBACKS_SRNO_", OracleDbType.Varchar2).Value = inputJson.SendBacksSrNo;
        //        oCmd.Parameters.Add("MODIFIEDBY_", OracleDbType.Varchar2).Value = ModifiedBy;

        //        oDataMgmt.ExecuteQuery(oCmd);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public bool FinanceRequestCompleted(string RequestId, string ModifiedBy)
        //{
        //    try
        //    {
        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_FINANCEREQUESTCOMPLETED";

        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = RequestId;
        //        oCmd.Parameters.Add("MODIFIEDBY_", OracleDbType.Varchar2).Value = ModifiedBy;

        //        oDataMgmt.ExecuteQuery(oCmd);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public DataTable GetTicketSummaryRpt(string RequestId, string StatusId)
        //{
        //    try
        //    {
        //        dt = new DataTable();

        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_GETTICKETSUMMARYRPT";

        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = RequestId;
        //        oCmd.Parameters.Add("STATUSID_", OracleDbType.Varchar2).Value = StatusId;
        //        oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

        //        dt = oDataMgmt.GetDataTable(oCmd);

        //        return (dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public DataTable GetTicketDetailsPostToSAP(string RequestId)
        //{
        //    try
        //    {
        //        dt = new DataTable();

        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_ADTOURREQUEST_SAP_DETAIL_GET";
        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = RequestId;
        //        oCmd.Parameters.Add("CUR_TOUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        //        dt = oDataMgmt.GetDataTable(oCmd);

        //        return (dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public bool UpdateSAPPostingDate(int SrNo, string RequestId, int Id, string SAPDocumentId, string SAPResponse, string ModifiedBy)
        //{
        //    try
        //    {
        //        OracleCommand oCmd = new OracleCommand();
        //        oCmd.CommandType = CommandType.StoredProcedure;
        //        oCmd.BindByName = true;
        //        oCmd.CommandText = "PKG_TOURBILLING.SPROC_UPDATESAPPOSTINGDATE";

        //        oCmd.Parameters.Add("SRNO_", OracleDbType.Int32).Value = SrNo;
        //        oCmd.Parameters.Add("REQUESTID_", OracleDbType.Varchar2).Value = RequestId;
        //        oCmd.Parameters.Add("ID_", OracleDbType.Int32).Value = Id;
        //        oCmd.Parameters.Add("SAPDOCUMENTID_", OracleDbType.Varchar2).Value = SAPDocumentId;
        //        oCmd.Parameters.Add("SAPRESPONSE_", OracleDbType.Varchar2).Value = SAPResponse;
        //        oCmd.Parameters.Add("MODIFIEDBY_", OracleDbType.Varchar2).Value = ModifiedBy;

        //        oDataMgmt.ExecuteQuery(oCmd);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

    }
}
