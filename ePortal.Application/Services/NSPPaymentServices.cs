using ePortal.DomainClasses;
using ePortal.ViewModels;
using System.Data;
using System.Dynamic;
using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;

namespace ePortal.Application.Services
{
    public class NSPPaymentServices : INSPPaymentServices
    {
        NSPPaymentRepository Repo;
        public NSPPaymentServices(NSPPaymentRepository _Repo)
        {
            Repo = _Repo;
        }

        #region Request Page 
        public List<NSP_PaymentHeaderViewModel> GetNSPPaymentList(string loginCode, short? status, string FromDate, string ToDate)
        {
            return Repo.GetNSPPaymentList(loginCode, status, FromDate, ToDate);
        }

        public List<object> GetLocationHelp()
        {
            return Repo.GetLocationHelp();
        }

        public List<object> GetCostCenterHelp()
        {
            return Repo.GetCostCenterHelp();
        }

        public LibResult SaveDetails(int LoginCode, NSP_PaymentHeaderViewModel Hd, List<NSP_PaymentDetailsViewModel> DtList, List<NSP_ApprovalAuthority> AppAuthList, string Flag)
        {
            return Repo.SaveDetails(LoginCode, Hd, DtList, AppAuthList, Flag);
        }

        public LibResult UpdateDetails(int LoginCode, NSP_PaymentHeaderViewModel Hd, List<NSP_PaymentDetailsViewModel> DtList, List<NSP_ApprovalAuthority> AppAuthList, string Flag)
        {
            return Repo.UpdateDetails(LoginCode, Hd, DtList, AppAuthList, Flag);
        }
        public LibResult VendorAutocompleteSuggestions(string term, string Catagory)
        {
            return Repo.VendorAutocompleteSuggestions(term, Catagory);
        }
        public LibResult GETEmployee(string Key, string designation)
        {
            return Repo.GETEmployee(Key, designation);
        }

        public bool DeleteWholeRequest(int id)
        {
            return Repo.DeleteWholeRequest(id);
        }

        public bool NSPPaymentRequestDtRowDelete(int DetailID, int HeaderID)
        {
            return Repo.NSPPaymentRequestDtRowDelete(DetailID, HeaderID);
        }
        public NSP_PaymentHeaderViewModel GetNSPEditDataById(long id, long loginCode)
        {
            return Repo.GetNSPEditDataById(id, loginCode);
        }
        public List<NSP_ApprovalAuthority> Gethead(long LoginCode, int RequestNo, string ApprovalType)
        {
            return Repo.Gethead(LoginCode, RequestNo, ApprovalType);
        }


        public LibResult GetReferenceNumber(string Key)
        {
            return Repo.GetReferenceNumber(Key);
        }
        public long GetLoginEmpLocation(int Emplogin)
        {
            return Repo.GetLoginEmpLocation(Emplogin);
        }


        #endregion

        #region  Approval

        public List<NSP_PaymentHeaderViewModel> GetApprovalNSPPaymentList(long loginCode, short? status, string FromDate, string ToDate)
        {
            return Repo.GetApprovalNSPPaymentList(loginCode, status,FromDate,ToDate);
        }

        public NSP_PaymentHeaderViewModel GetNSPById(long id, long loginEmp, string Department)
        {
            return Repo.GetNSPById(id, loginEmp, Department);
        }

        public LibResult ApproveReject(int HEADERID, string EMPCODE, string Response, string Remark, long loginCode)
        {
            return Repo.ApproveReject(HEADERID, EMPCODE, Response, Remark, loginCode);
        }

        #endregion

        #region  Finance 

        public List<NSP_PaymentHeaderViewModel> GetNSPFinanceList(short? status, int? loginCode,  string FromDate, string ToDate, string InvoiceSearchKey) // InvoiceSearchKey ADDED BY AUMENTO :: SR88023-CR5305
        {
            return Repo.GetNSPFinanceList(status, loginCode, FromDate, ToDate , InvoiceSearchKey); // InvoiceSearchKey ADDED BY AUMENTO :: SR88023-CR5305
        }

        public LibResult ApproveFinanceReject(NSP_PaymentHeaderViewModel obj, List<NSP_PaymentDetailsViewModel> detailsList, int loginCode)
        {
            return Repo.ApproveFinanceReject(obj, detailsList, loginCode);
        }

        #endregion

        #region Taxation 
        public List<NSP_PaymentHeaderViewModel> GetNSPTaxationList(short? status, int? loginCode,  string FromDate, string ToDate)
        {
            return Repo.GetNSPTaxationList(status, loginCode,FromDate,ToDate);
        }

        public LibResult ApproveTaxationReject(NSP_PaymentHeaderViewModel obj, List<NSP_PaymentDetailsViewModel> detailsList, int loginEmp)
        {
            return Repo.ApproveTaxationReject(obj, detailsList, loginEmp);
        }

        #endregion

        #region Common Function
        public List<NSP_ApprovalAuthority> GetNSPHistoryById(long id)
        {
            return Repo.GetNSPHistoryById(id);
        }

        public object GetTDSTypeList()
        {
            return Repo.GetTDSTypeList();
        }


        public string GetESTax()
        {
            return Repo.GetESTax();
        }

        public object GetGSTRateList(string Type, string TaxType, string COSTCENTER, int Status)
        {
            return Repo.GetGSTRateList(Type,TaxType,COSTCENTER,Status);
        }

        #endregion

        #region Report

        public NSPReportData GetNSPReportDetail(int HeaderID)
        {
            return Repo.GetNSPReportDetail(HeaderID);
        }
        #endregion


        #region  POST and Reverse POst

        public LibResult GetDataForPostInvoices(string Status, int LoginCode, string Mode,string FromDate, string ToDate)
        {
            return Repo.GetDataForPostInvoices(Status, LoginCode, Mode,FromDate,ToDate);
        }

        public LibResult GetPostReverseRights(int LoginCode)
        {
            return Repo.GetPostReverseRights(LoginCode);
        }

        public async Task<LibResult> PostAndReversePostInSap(POSTReverseRequest request, int LoginCode)
        {
            return await Repo.PostAndReversePostInSap(request, LoginCode);
        }

        public List<NSP_PaymentDetails> GetPostInvoiceData(int HEADERID, string InvoiceNo, int detailID, string DocType, int LoginCode, string CostCenter, string BusinessCenter, string Status)
        {
           return Repo.GetPostInvoiceData( HEADERID, InvoiceNo,  detailID, DocType, LoginCode, CostCenter, BusinessCenter , Status);
        }

        public List<NSP_PaymentDetails> GetPostInvoiceDataReverse(int HeaderID, string POSTEDDOCNO, string InvoiceNO)
        {
            return Repo.GetPostInvoiceDataReverse(HeaderID, POSTEDDOCNO, InvoiceNO);
        }

        public List<NSP_PaymentDetails> GEtPostedInvoiceHistory(int HEADERID, string InvoiceNo)
        {
            return Repo.GEtPostedInvoiceHistory(HEADERID, InvoiceNo);
        }
        public List<object> GetBusinessCenterHelp()
        {
            return Repo.GetBusinessCenterHelp();
        }

        #endregion

        #region  MAster

        #region Role Master
        public LibResult GetNSP_Role_Master_List_Data()
        {
            return Repo.GetNSP_Role_Master_List_Data();
        }

      

        public LibResult GetNSP_Role_Master_Edit_Data(int Srno)
        {
            return Repo.GetNSP_Role_Master_Edit_Data(Srno);
        }

        public LibResult NSP_Role_Master_SaveAndUpdate_Data(NSP_ROLE_MASTER Obj, string Mode, int loginCode)
        {

            return Repo.NSP_Role_Master_SaveAndUpdate_Data(Obj, Mode, loginCode);
        }

        public LibResult DeActive_Role_Master_Record(int SrNo,int loginCode)
        {
            return Repo.DeActive_Role_Master_Record(SrNo,loginCode);
        }

        public LibResult BindPlantName()
        {

            return Repo.BindPlantName();
        }
        #endregion

        #region TDS Master
        public LibResult GetNSP_TDS_TAX_TYPE_MASTER_List_Data()
        {
            return Repo.GetNSP_TDS_TAX_TYPE_MASTER_List_Data();
        }

        public LibResult GetNSP_TDS_TAX_TYPE_MASTER_Edit_Data(int Srno)
        {
            return Repo.GetNSP_TDS_TAX_TYPE_MASTER_Edit_Data(Srno);
        }


        public LibResult DeActive_NSP_TDS_TAX_TYPE_MASTER_Record(int SrNo, int loginCode)
        {
            return Repo.DeActive_NSP_TDS_TAX_TYPE_MASTER_Record(SrNo, loginCode);
        }

        public LibResult NSP_NSP_TDS_TAX_TYPE_MASTER_SaveData(NSP_TDS_TAX_TYPE_MASTER Obj, int loginCode, string Mode)
        {
            return Repo.NSP_NSP_TDS_TAX_TYPE_MASTER_SaveData(Obj,loginCode,Mode);
        }


        #endregion

        #region Business Profit Master
        public LibResult GetNSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_List_Data()
        {
            return Repo.GetNSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_List_Data(); ;
        }

        public LibResult GetNSPBUPRO_MASTER_Edit_Data(int Srno)
        {
            return Repo.GetNSPBUPRO_MASTER_Edit_Data(Srno);
        }
        public LibResult DeActive_NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_Record(int SrNo, int loginCode)
        {
            return Repo.DeActive_NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_Record(SrNo,loginCode);
        }

        public LibResult NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_SaveData(NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER Obj, int loginCode, string Mode)
        {
           
            return Repo.NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_SaveData(Obj,loginCode,Mode);
        }

        #region GST Master
        public LibResult GetNSP_GST_TAX_MASTER_List_Data()
        {
            return Repo.GetNSP_GST_TAX_MASTER_List_Data() ;
        }
        public LibResult GetNSP_GST_TAX_MASTER_Edit_Data(int Srno)
        {
            return Repo.GetNSP_GST_TAX_MASTER_Edit_Data(Srno);
        
        }
        public LibResult DeActive_NSP_GST_TAX_MASTER_Record(int SrNo, int loginCode)
        {
            return Repo.DeActive_NSP_GST_TAX_MASTER_Record(SrNo,loginCode);
        }

        public LibResult NSP_GST_TAX_MASTER_SaveData(NSP_GST_TAX_MASTER Obj, int loginCode, string Mode)
        {
            return Repo.NSP_GST_TAX_MASTER_SaveData(Obj,loginCode,Mode);
        }


        #endregion
        #endregion

        #region Master Log View

        public async Task<List<ExpandoObject>> GetNSP_Master_Log_Data(int Srno, string TblName)
        {
            return await Repo.GetNSP_Master_Log_Data(Srno,TblName);
        }
        #endregion

        #endregion
    }
}

