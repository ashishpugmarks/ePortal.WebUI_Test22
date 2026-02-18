using ePortal.DomainClasses;
using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Contracts
{
    public interface INSPPaymentServices
    {
        #region Request Page 
        List<NSP_PaymentHeaderViewModel> GetNSPPaymentList(string loginCode, short? status, string FromDate, string ToDate);


        List<object> GetLocationHelp();


        List<object> GetCostCenterHelp();


        LibResult SaveDetails(int LoginCode, NSP_PaymentHeaderViewModel Hd, List<NSP_PaymentDetailsViewModel> DtList, List<NSP_ApprovalAuthority> AppAuthList, string Flag);


        LibResult UpdateDetails(int LoginCode, NSP_PaymentHeaderViewModel Hd, List<NSP_PaymentDetailsViewModel> DtList, List<NSP_ApprovalAuthority> AppAuthList, string Flag);

        LibResult VendorAutocompleteSuggestions(string term, string Catagory);

        LibResult GETEmployee(string Key, string designation);


        bool DeleteWholeRequest(int id);


        bool NSPPaymentRequestDtRowDelete(int DetailID, int HeaderID);

        NSP_PaymentHeaderViewModel GetNSPEditDataById(long id, long loginCode);

        List<NSP_ApprovalAuthority> Gethead(long LoginCode, int RequestNo, string ApprovalType);


       

        LibResult GetReferenceNumber(string Key);

        long GetLoginEmpLocation(int Emplogin);



        #endregion

        #region  Approval

        List<NSP_PaymentHeaderViewModel> GetApprovalNSPPaymentList(long loginCode, short? status, string FromDate, string ToDate);


        NSP_PaymentHeaderViewModel GetNSPById(long id, long loginEmp, string Department);


        LibResult ApproveReject(int HEADERID, string EMPCODE, string Response, string Remark, long loginCode);


        #endregion

        #region  Finance 

        List<NSP_PaymentHeaderViewModel> GetNSPFinanceList(short? status, int? loginCode, string FromDate, string ToDate,  string InvoiceSearchKey); // InvoiceSearchKey ADDED BY AUMENTO :: SR88023-CR5305


        LibResult ApproveFinanceReject(NSP_PaymentHeaderViewModel obj, List<NSP_PaymentDetailsViewModel> detailsList, int loginCode);


        #endregion

        #region Taxation 
        List<NSP_PaymentHeaderViewModel> GetNSPTaxationList(short? status, int? loginCode, string FromDate, string ToDate);


        LibResult ApproveTaxationReject(NSP_PaymentHeaderViewModel obj, List<NSP_PaymentDetailsViewModel> detailsList, int loginEmp);


        #endregion

        #region Common Function
        List<NSP_ApprovalAuthority> GetNSPHistoryById(long id);

        object GetTDSTypeList();

        string GetESTax();
        object GetGSTRateList(string Type, string TaxType, string COSTCENTER , int Status);

        #endregion

        #region Report

        NSPReportData GetNSPReportDetail(int HeaderID);

        #endregion


        #region  POST and Reverse POst

        LibResult GetDataForPostInvoices(string Status, int LoginCode,string Mode, string FromDate, string ToDate);

        LibResult GetPostReverseRights(int LoginCode);
        Task<LibResult> PostAndReversePostInSap(POSTReverseRequest request, int LoginCode);

        List<NSP_PaymentDetails> GetPostInvoiceData(int HEADERID, string InvoiceNo, int detailID, string DocType, int LoginCode, string CostCenter, string BusinessCenter, string Status);
        List<NSP_PaymentDetails> GetPostInvoiceDataReverse(int HeaderID, string POSTEDDOCNO, string InvoiceNO);
        List<NSP_PaymentDetails> GEtPostedInvoiceHistory(int HEADERID, string InvoiceNo);

        List<object> GetBusinessCenterHelp();
        
            #endregion


            #region  MAster

        #region Role Master
        LibResult GetNSP_Role_Master_List_Data();
       
        LibResult GetNSP_Role_Master_Edit_Data(int Srno);

        LibResult NSP_Role_Master_SaveAndUpdate_Data(NSP_ROLE_MASTER Obj, string Mode, int loginCode);

        LibResult DeActive_Role_Master_Record(int SrNo,int loginCode);
        
        LibResult BindPlantName();
        #endregion

        #region TDS Master
        LibResult GetNSP_TDS_TAX_TYPE_MASTER_List_Data();
        LibResult GetNSP_TDS_TAX_TYPE_MASTER_Edit_Data(int Srno);
        LibResult DeActive_NSP_TDS_TAX_TYPE_MASTER_Record(int SrNo, int loginCode);

        LibResult NSP_NSP_TDS_TAX_TYPE_MASTER_SaveData(NSP_TDS_TAX_TYPE_MASTER Obj, int loginCode, string Mode);

        #endregion

        #region Business Profit Master
        LibResult GetNSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_List_Data();
        LibResult GetNSPBUPRO_MASTER_Edit_Data(int Srno);
        LibResult DeActive_NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_Record(int SrNo, int loginCode);
        LibResult NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_SaveData(NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER Obj, int loginCode, string Mode);

        #endregion

        #region GST Master
        LibResult GetNSP_GST_TAX_MASTER_List_Data();
        LibResult GetNSP_GST_TAX_MASTER_Edit_Data(int Srno);
        
        LibResult DeActive_NSP_GST_TAX_MASTER_Record(int SrNo, int loginCode);

        LibResult NSP_GST_TAX_MASTER_SaveData(NSP_GST_TAX_MASTER Obj, int loginCode, string Mode);



        #endregion

        #region Master Log View

        Task<List<ExpandoObject>> GetNSP_Master_Log_Data(int Srno, string TblName);
        
        #endregion

        #endregion

    }
}
