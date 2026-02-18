using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using ePortal.Shared;
using ePortal.ViewModels;
using ePortal.ViewModels.DataExchange.NSPPayment;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Objects; // ADDED BY AUMENTO :: SR88023-CR5305 :: 2.0
using System.Dynamic;
using System.Text.RegularExpressions;

namespace ePortal.Infrastructure.Repositories
{
    public class NSPPaymentRepository
    {
        private readonly LCModelDBContext _DB;
        private readonly ICommonFunctions _ObjCommn;
        private readonly IEportalESS RFC;
        private decimal _Syki;
        private SYKI_LC _Sykis;

        public NSPPaymentRepository(LCModelDBContext db, ICommonFunctions ObjCommn, IEportalESS _RFC)
        {
            _DB = db;
            _ObjCommn = ObjCommn;
            RFC = _RFC;
            _Syki = _DB.SYKI_LC.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();
            _Sykis = _DB.SYKI_LC.Where(x => x.ACTIVE == 1).FirstOrDefault();
        }

        #region Request Page 
        public List<NSP_PaymentHeaderViewModel> GetNSPPaymentList(string loginCode, short? status, string FromDate, string ToDate)
        {
            int loginCodeInt = Convert.ToInt32(loginCode);

            DateTime _fDate = DateTime.Parse(FromDate);
            DateTime _tDate = DateTime.Parse(ToDate);

            List<NSP_PaymentHeaderViewModel> Data = new List<NSP_PaymentHeaderViewModel>();
            if (status == 0)
            {
                Data = (from paymentHeader in _DB.NSP_PAYMENT_HEADER
                        where paymentHeader.ADDEDBY == loginCodeInt
                       && (paymentHeader.STATUS.HasValue && new short[] { 0, 5 }.Contains(paymentHeader.STATUS.Value))
                        && paymentHeader.ADDEDON >= _fDate
                        && paymentHeader.ADDEDON <= _tDate
                        orderby paymentHeader.REQUESTREFNO ascending
                        select new NSP_PaymentHeaderViewModel
                        {
                            HeaderID = paymentHeader.HEADERID,
                            RequestRefNo = paymentHeader.REQUESTREFNO,
                            RequestDate = paymentHeader.REQUESTDATE,
                            Category = paymentHeader.CATEGORY,
                            Location = paymentHeader.LOCATION,
                            STATUS = paymentHeader.STATUS,
                            TotalNetAmount = paymentHeader.TOTALNETAMOUNT,
                            VendorCode = paymentHeader.VENDORCODE,
                            VendorName = paymentHeader.VENDORNAME
                        }).ToList();
            }
            else
            {
                Data = (from paymentHeader in _DB.NSP_PAYMENT_HEADER
                        where paymentHeader.ADDEDBY == loginCodeInt
                        && (paymentHeader.STATUS.HasValue && new short[] { 1, 2, 3, 4, 6, 7 }.Contains(paymentHeader.STATUS.Value))
                        orderby paymentHeader.REQUESTREFNO ascending
                        select new NSP_PaymentHeaderViewModel
                        {
                            HeaderID = paymentHeader.HEADERID,
                            RequestRefNo = paymentHeader.REQUESTREFNO,
                            RequestDate = paymentHeader.REQUESTDATE,
                            Category = paymentHeader.CATEGORY,
                            Location = paymentHeader.LOCATION,
                            STATUS = paymentHeader.STATUS,
                            TotalNetAmount = paymentHeader.TOTALNETAMOUNT,
                            VendorCode = paymentHeader.VENDORCODE,
                            VendorName = paymentHeader.VENDORNAME
                        }).ToList();
            }
            return Data;
        }

        public List<object> GetLocationHelp()
        {
            List<object> LocationList = new List<object>();

            var Query = (from r in _DB.SYSITE_LC
                         where r.ACTIVE == 1
                         select new
                         {
                             text = r.DESCRIP,
                             value = r.SYSITEID
                         }).ToList();
            LocationList.AddRange(Query);
            return LocationList;
        }

        public List<object> GetCostCenterHelp()
        {
            List<object> CostCenterList = new List<object>();

            var Query = (from r in _DB.FINTBS_COST_CENTER_MST
                         where r.STATUS == 1
                         select new
                         {
                             text = r.COSTCENTER,
                             value = r.COSTCENTER
                         }).Distinct().ToList();

            CostCenterList.AddRange(Query);

            return CostCenterList;

        }

        public LibResult VendorAutocompleteSuggestions(string Key, string Category)
        {
            try
            {
                LibResult res = new LibResult();
                dynamic Employee = null;
                dynamic Vendor = null;
                dynamic Dealer = null;

                if (Category == "Vendor")
                {
                    Vendor = (from i in _DB.FINVENDORMASTERMST_LC.Where(x => x.VENDORCODE.Contains(Key) || x.NAME1.Contains(Key))
                              select new
                              {
                                  i.VENDORCODE,
                                  i.NAME1,
                                  i.NAME2,
                                  i.NAME3,
                                  i.STREET1,
                                  i.STREET2,
                                  i.STREET3,
                                  i.STREET4,
                                  i.BANKNAME,
                                  i.BANKACCOUNTNO,
                                  i.IFSCCODE,
                                  i.CITY,
                                  i.GSTIN,
                                  i.PANNO
                              }).Take(200).ToList();
                }
                else if (Category == "Dealer")
                {
                    //CommonFunctions cf = new CommonFunctions();

                    DataTable dt = _ObjCommn.GetDealerData(Key);
                    Dealer = ConvertDataTableToList(dt).Take(200).ToList();
                }
                else if (Category == "Employees")
                {
                    Employee = (from i in _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE.ToString().Contains(Key) || x.FIRSTNAME.Contains(Key) || x.LASTNAME.Contains(Key))
                                select new
                                {
                                    i.ADEMPCODE,
                                    i.FIRSTNAME,
                                    i.LASTNAME,
                                    i.PANCARDNO
                                }).Take(200).ToList();
                }
                else
                {
                    // Handle other cases if needed
                }

                var Data = new { VendorList = Vendor, EmployeeList = Employee, DealerList = Dealer };
                res.resultObject = Data;
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public LibResult SaveDetails(int LoginCode, NSP_PaymentHeaderViewModel Hd, List<NSP_PaymentDetailsViewModel> DtList, List<NSP_ApprovalAuthority> AppAuthList, string Flag)
        {
            LibResult res = new LibResult();
            short _Status = (Flag == "SAVE" ? (short)0 : (short)1);
            int HEADERID_; // ADDED BY AUMENTO :: SR88023-CR5305
            using (var transaction = _DB.Database.BeginTransaction())
            {
                try
                {
                    HEADERID_ = _DB.NSP_PAYMENT_HEADER.Count()>0 ? _DB.NSP_PAYMENT_HEADER.Max(x => x.HEADERID) + 1 : 1; // ADDED BY AUMENTO :: SR88023-CR5305
                    NSP_PAYMENT_HEADER exist = new NSP_PAYMENT_HEADER
                    {
                        //                        HEADERID = _DB.NSP_PAYMENT_HEADER.Any() ? _DB.NSP_PAYMENT_HEADER.Max(x => x.HEADERID) + 1 : 1, // COMMENTED BY AUMENTO :: SR88023-CR5305
                        //                        REQUESTREFNO = Hd.RequestRefNo, // COMMENTED BY AUMENTO :: SR88023-CR5305
                        HEADERID = HEADERID_, // ADDED BY AUMENTO :: SR88023-CR5305
                        REQUESTREFNO = HEADERID_, // ADDED BY AUMENTO :: SR88023-CR5305
                        REQUESTDATE = Hd.RequestDate,
                        CATEGORY = Hd.Category,
                        LOCATION = Hd.Location,
                        COSTCENTRE = Hd.CostCentre,
                        BUDGETED = Hd.Budgeted,
                        PURPOSEOFEXPENSES = Hd.PurposeofExpenses,
                        VENDORCODE = Hd.VendorCode,
                        VENDORNAME = Hd.VendorName,
                        VENDORPANNO = Hd.VendorPANNo,
                        VENDORADDRESS = Hd.VendorAddress,
                        BANKNAME = Hd.BankName,
                        BANKACCOUNTNO = Hd.BankAccountNo,
                        BANKIFSCCODE = Hd.BankIFSCCode,
                        ADDEDON = DateTime.Now,
                        ADDEDBY = LoginCode,
                        FY_REQUESTTYPE = "",
                        FY_EXRATE = 0,
                        FY_EXRATEDATE = DateTime.Now,
                        FY_TAXATION = "",
                        FY_GSTTYPE = "",
                        FY_GST = "",
                        FY_CGSTRATE = 0,
                        FY_SGSTRATE = 0,
                        FY_IGSTRATE = 0,
                        CGSTAMOUNT = 0,
                        SGSTAMOUNT = 0,
                        IGSTAMOUNT = 0,
                        FINAL_TOTAL = (decimal)Hd.TotalNetAmount,
                        APPROVED_FINAL_TOTAL = (decimal)Hd.TotalNetAmount,
                        TOTAL_GST_AMOUNT = 0,
                        FY_TDSREASON = "",
                        FY_TDS = "",
                        FY_TDSRATE = 0,
                        FY_TDS_AMOUNT = 0,
                        FY_GSTREASON = "",
                        STATUS = _Status,
                        TOTALNETAMOUNT = (decimal)Hd.TotalNetAmount,
                        TOTALNETAMOUNTINWORD = Hd.TotalNetAmountInWord,
                        CURRENCYTYPE = Hd.CurrencyType,
                        REQUESTTYPE = Hd.RequestType,
                        LOCATIONID = Hd.LOCATIONID,
                        PAY_PRO_LOCATION = Hd.PAY_PRO_LOCATION,
                        PAY_PRO_LOCATIONID = Hd.PAY_PRO_LOCATIONID,
                        BUDGETED_ATTACH = Hd.BUDGETED_ATTACH,
                        GSTIN = Hd.GSTIN,
                        TDSBASEAMOUNT = 0,
                        TDS_UNDER_SECTIONCODE = "",


                    };
                    _DB.NSP_PAYMENT_HEADER.Add(exist);
                    _DB.SaveChanges();

                    //var maxDetailID = _DB.NSP_PAYMENT_DETAIL.Select(x => x.DETAILID).DefaultIfEmpty(0).Max(); // COMMENTED BY AUMENTO :: SR88023-CR5305
                    //int nextDetailID = maxDetailID + 1;  // COMMENTED BY AUMENTO :: SR88023-CR5305

                    foreach (var detail in DtList)
                    {
                        //var NewDetailID = _DB.Database.SqlQuery<int>("SELECT SEQ_NSP_PAYMENT_DETAIL.NEXTVAL FROM DUAL").FirstOrDefault(); // ADDED BY AUMENTO :: SR88023-CR5305

                        var result = _DB.Set<NSP_NEXTVAL_MODEL>()
                            .FromSqlRaw("SELECT SEQ_NSP_PAYMENT_DETAIL.NEXTVAL AS NEXTVAL FROM DUAL")
                            .AsEnumerable()
                            .FirstOrDefault();
                        int NewDetailID = result?.NEXTVAL ?? 0;

                        var paymentDetail = new NSP_PAYMENT_DETAIL
                        {
                            DETAILID = NewDetailID, // ADDED BY AUMENTO :: SR88023-CR5305
                            HEADERID = exist.HEADERID,
                            INVOICEDATE = detail.InvoiceDate,
                            INVOICENO = detail.InvoiceNo,
                            DESCRIPTION = detail.Description,
                            CURRENCY = detail.Currency,
                            AMOUNT = (decimal)detail.Amount,
                            APPROVED_AMOUNT = detail.Amount,
                            PO_DOCUMENT = detail.PO_Document == null ? "" : detail.PO_Document,
                            INVOICE_DOCUMENT = detail.Invoice_Document == null ? "" : detail.Invoice_Document,
                            APPROVALNOTE_DOCUMENT = detail.ApprovalNote_Document == null ? "" : detail.ApprovalNote_Document,
                            OTHER_DOCUMENT = detail.Other_Document == null ? "" : detail.Other_Document,
                            BASE_AMOUNT = (decimal)detail.BaseAmount,
                            TAX_TYPE = detail.TaxType,
                            GST_TYPE = detail.GSTType,
                            CGSTRATE = detail.CGSTRate,
                            CGSTAMOUNT = detail.CGSTAmount,
                            SGSTRATE = detail.SGSTRate,
                            SGSTAMOUNT = detail.SGSTAmount,
                            IGSTRATE = detail.IGSTRate,
                            IGSTAMOUNT = detail.IGSTAmount,
                            TOTAL_GST_AMOUNT = (decimal)detail.TotalGSTAmount,
                            TOTAL_AMOUNT = (decimal)detail.TotalAmount,
                            NILGSTREASON = "",
                            NILTDSREASON = "",
                            TDS_BASE_AMOUNT = 0,
                            TDS = "No",
                            TDSRATE = 0,
                            TDS_AMOUNT = 0,
                            TOTAL_NET_AMOUNT = (decimal)detail.TotalAmount,
                            ADDEDBY = LoginCode,
                            ADDEDON = DateTime.Now,
                            ISPOSTED = "N",
                            GSTTAXCODE = detail.GSTTaxCode,
                            IS_ESI = "No",
                            ESI_RATE = 0,
                            ESI_AMOUNT = 0,
                            DISCOUNT_AMOUNT = 0

                        };
                        _DB.NSP_PAYMENT_DETAIL.Add(paymentDetail);
                        // nextDetailID++;   // COMMENTED BY AUMENTO :: SR88023-CR5305
                    }
                    LibResult AppAuth = InsertToNSP_ApprovalAuthority(AppAuthList, exist, LoginCode);

                    if (AppAuth.hasError == false && Flag == "SUBMIT")
                    {
                        NSP_APPROVAL_AUTHORITY NSP_ = new NSP_APPROVAL_AUTHORITY();

                        NSP_ = _DB.NSP_APPROVAL_AUTHORITY.Where(x => x.HEADERID == exist.HEADERID && x.APPSEQ == 1).FirstOrDefault();
                        NSP_.APPROVAL_STATUS = 1;
                        _DB.Entry(NSP_).State = EntityState.Modified;
                        // InsertToNSP_ApprovalAuthority_LogHistory(NSP_);

                        long Empcode = long.Parse(NSP_.EMPCODE.ToString());
                        ADEMPLOYEE_LC NextAuthority = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == Empcode && x.ACTIVE == 1).FirstOrDefault();
                        ADEMPLOYEE_LC Requestet = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == LoginCode && x.ACTIVE == 1).FirstOrDefault();
                        SendMailToNextAuthority(Requestet, NextAuthority, 1); // For Approval
                                                                              // START :: ADDED BY AUMENTO :: SR88023-CR5305
                        res.hasError = false;
                        res.errorMessage = "Request No. : " + HEADERID_.ToString() + " Submitted Successfully";
                    }
                    else
                    {
                        res.hasError = false;
                        res.errorMessage = "Saved Successfully with Request No. : " + HEADERID_.ToString();
                        // END :: ADDED BY AUMENTO :: SR88023-CR5305

                    }

                    _DB.SaveChanges();
                    transaction.Commit();
                }

                catch (Exception ex)
                {
                    Console.WriteLine("Error occurred: " + ex.Message);
                    transaction.Rollback();
                    res.hasError = true;
                    res.errorMessage = ex.Message.ToString();
                }
            }
            return res;
        }

        public LibResult UpdateDetails(int LoginCode, NSP_PaymentHeaderViewModel Hd, List<NSP_PaymentDetailsViewModel> DtList, List<NSP_ApprovalAuthority> AppAuthList, string Flag)
        {
            LibResult res = new LibResult();
            short _Status = (Flag == "SAVE" ? (short)0 : (short)1);
            int HEADERID_ = Hd.HeaderID; // ADDED BY AUMENTO :: SR88023-CR5305
            using (var transaction = _DB.Database.BeginTransaction())
            {
                try
                {
                    NSP_PAYMENT_HEADER exist = _DB.NSP_PAYMENT_HEADER.Where(x => x.HEADERID == Hd.HeaderID).FirstOrDefault();
                    if (exist != null)
                    {
                        exist.CATEGORY = Hd.Category;
                        exist.LOCATION = Hd.Location;
                        exist.COSTCENTRE = Hd.CostCentre;
                        exist.BUDGETED = Hd.Budgeted;
                        exist.PURPOSEOFEXPENSES = Hd.PurposeofExpenses;
                        exist.VENDORCODE = Hd.VendorCode;
                        exist.VENDORNAME = Hd.VendorName;
                        exist.VENDORPANNO = Hd.VendorPANNo;
                        exist.VENDORADDRESS = Hd.VendorAddress;
                        exist.BANKNAME = Hd.BankName;
                        exist.BANKACCOUNTNO = Hd.BankAccountNo;
                        exist.BANKIFSCCODE = Hd.BankIFSCCode;
                        exist.UPDATEDON = DateTime.Now;
                        exist.UPDATEDBY = LoginCode;
                        exist.FY_REQUESTTYPE = "";
                        exist.FY_EXRATE = 0;
                        exist.FY_EXRATEDATE = DateTime.Now;
                        exist.FY_TAXATION = "";
                        exist.FY_GSTTYPE = "";
                        exist.FY_GST = "";
                        exist.FY_CGSTRATE = 0;
                        exist.FY_SGSTRATE = 0;
                        exist.FY_IGSTRATE = 0;
                        exist.CGSTAMOUNT = 0;
                        exist.SGSTAMOUNT = 0;
                        exist.IGSTAMOUNT = 0;
                        exist.TOTAL_GST_AMOUNT = 0;
                        exist.FY_TDSREASON = "";
                        exist.FY_TDS = "";
                        exist.FY_TDSRATE = 0;
                        exist.FY_TDS_AMOUNT = 0;
                        exist.FY_GSTREASON = "";
                        exist.FINAL_TOTAL = Hd.TotalNetAmount;
                        exist.APPROVED_FINAL_TOTAL = Hd.TotalNetAmount;
                        exist.STATUS = _Status;
                        exist.TOTALNETAMOUNT = Hd.TotalNetAmount;
                        exist.TOTALNETAMOUNTINWORD = Hd.TotalNetAmountInWord;
                        exist.CURRENCYTYPE = Hd.CurrencyType;
                        exist.REQUESTTYPE = Hd.RequestType;
                        exist.LOCATIONID = Hd.LOCATIONID;
                        exist.PAY_PRO_LOCATION = Hd.PAY_PRO_LOCATION;
                        exist.PAY_PRO_LOCATIONID = Hd.PAY_PRO_LOCATIONID;
                        exist.FINANCEREMARK = "";
                        exist.TAXATIONREMARK = "";
                        exist.GSTIN = Hd.GSTIN;
                        exist.TDSBASEAMOUNT = 0;
                        exist.TDS_UNDER_SECTIONCODE = "";
                        if (Hd.BUDGETED_ATTACH != null && Hd.BUDGETED_ATTACH != "") { exist.BUDGETED_ATTACH = Hd.BUDGETED_ATTACH; }
                        _DB.Entry(exist).State = EntityState.Modified;
                    }
                    else
                    {
                        _DB.NSP_PAYMENT_HEADER.Add(exist);
                    }
                    //_DB.NSP_PAYMENT_HEADER.Add(exist);
                    _DB.SaveChanges();

                    //using (var TempDb = new LCEntities())
                    //{
                    List<NSP_PAYMENT_DETAIL> DeleList = _DB.NSP_PAYMENT_DETAIL.Where(x => x.HEADERID == Hd.HeaderID).ToList();
                    if (DeleList.Count > 0)
                    {
                        _DB.NSP_PAYMENT_DETAIL.RemoveRange(DeleList);
                        _DB.SaveChanges();
                    }
                    //}



                    //var maxDetailID = _DB.NSP_PAYMENT_DETAIL.Select(x => x.DETAILID).DefaultIfEmpty(0).Max();// COMMENTED BY AUMENTO :: SR88023-CR5305
                    //int nextDetailID = maxDetailID + 1; // COMMENTED BY AUMENTO :: SR88023-CR5305


                    foreach (var detail in DtList)
                    {

                        //var NewDetailID = _DB.Database.SqlQuery<int>("SELECT SEQ_NSP_PAYMENT_DETAIL.NEXTVAL FROM DUAL").FirstOrDefault();
                        var result = _DB.Set<NSP_NEXTVAL_MODEL>()
                          .FromSqlRaw("SELECT SEQ_NSP_PAYMENT_DETAIL.NEXTVAL AS NEXTVAL FROM DUAL")
                          .AsEnumerable()
                          .FirstOrDefault();
                        int NewDetailID = result?.NEXTVAL ?? 0;

                        var paymentDetail = new NSP_PAYMENT_DETAIL
                        {
                            DETAILID = NewDetailID, // ADDED BY AUMENTO :: SR88023-CR5305
                            HEADERID = exist.HEADERID,
                            INVOICEDATE = detail.InvoiceDate,
                            INVOICENO = detail.InvoiceNo,
                            DESCRIPTION = detail.Description,
                            CURRENCY = detail.Currency,
                            AMOUNT = detail.Amount,
                            APPROVED_AMOUNT = detail.Amount,
                            PO_DOCUMENT = detail.PO_Document == null ? "" : detail.PO_Document,
                            INVOICE_DOCUMENT = detail.Invoice_Document == null ? "" : detail.Invoice_Document,
                            APPROVALNOTE_DOCUMENT = detail.ApprovalNote_Document == null ? "" : detail.ApprovalNote_Document,
                            OTHER_DOCUMENT = detail.Other_Document == null ? "" : detail.Other_Document,
                            BASE_AMOUNT = (decimal)detail.BaseAmount,
                            TAX_TYPE = detail.TaxType,
                            GST_TYPE = detail.GSTType,
                            CGSTRATE = detail.CGSTRate,
                            CGSTAMOUNT = detail.CGSTAmount,
                            SGSTRATE = detail.SGSTRate,
                            SGSTAMOUNT = detail.SGSTAmount,
                            IGSTRATE = detail.IGSTRate,
                            IGSTAMOUNT = detail.IGSTAmount,
                            TOTAL_GST_AMOUNT = (decimal)detail.TotalGSTAmount,
                            TOTAL_AMOUNT = (decimal)detail.TotalAmount,
                            NILGSTREASON = "",
                            NILTDSREASON = "",
                            TDS_BASE_AMOUNT = 0,
                            TDS = "No",
                            TDSRATE = 0,
                            TDS_AMOUNT = 0,
                            TOTAL_NET_AMOUNT = (decimal)detail.TotalAmount,
                            ADDEDBY = LoginCode,
                            ADDEDON = DateTime.Now,
                            ISPOSTED = "N",
                            GSTTAXCODE = detail.GSTTaxCode,
                            IS_ESI = "No",
                            ESI_RATE = 0,
                            ESI_AMOUNT = 0,
                            DISCOUNT_AMOUNT = 0


                        };
                        _DB.NSP_PAYMENT_DETAIL.Add(paymentDetail);
                        //  nextDetailID++; // ADDED BY AUMENTO :: SR88023-CR5305
                    }


                    //using (var TempDb = new LCEntities())
                    //{
                    //    List<NSP_APPROVAL_AUTHORITY> DeleList = TempDb.NSP_APPROVAL_AUTHORITY.Where(x => x.HEADERID == Hd.HeaderID).ToList();
                    //    if (DeleList.Count > 0)
                    //    {
                    //        TempDb.NSP_APPROVAL_AUTHORITY.RemoveRange(DeleList);
                    //        TempDb.SaveChanges();
                    //    }
                    //}

                    LibResult AppAuth = InsertToNSP_ApprovalAuthority(AppAuthList, exist, LoginCode);

                    if (AppAuth.hasError == false && Flag == "SUBMIT")
                    {
                        NSP_APPROVAL_AUTHORITY NSP_ = new NSP_APPROVAL_AUTHORITY();

                        NSP_ = _DB.NSP_APPROVAL_AUTHORITY.Where(x => x.HEADERID == exist.HEADERID && x.APPSEQ == 1).FirstOrDefault();
                        NSP_.APPROVAL_STATUS = 1;
                        _DB.Entry(NSP_).State = EntityState.Modified;
                        //InsertToNSP_ApprovalAuthority_LogHistory(NSP_);

                        long Empcode = long.Parse(NSP_.EMPCODE.ToString());
                        ADEMPLOYEE_LC NextAuthority = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == Empcode && x.ACTIVE == 1).FirstOrDefault();
                        ADEMPLOYEE_LC Requestet = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == LoginCode && x.ACTIVE == 1).FirstOrDefault();
                        SendMailToNextAuthority(Requestet, NextAuthority, 1); // For Approval
                                                                              // START :: ADDED BY AUMENTO :: SR88023-CR5305
                        res.hasError = false;
                        res.errorMessage = "Request No. : " + HEADERID_.ToString() + " Submitted Successfully";
                    }
                    else
                    {
                        res.hasError = false;
                        res.errorMessage = "Request No. : " + HEADERID_.ToString() + " Updated Successfully";
                        // END :: ADDED BY AUMENTO :: SR88023-CR5305
                    }

                    _DB.SaveChanges();
                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occurred: " + ex.Message);
                    transaction.Rollback();
                    res.hasError = true;
                    res.errorMessage = ex.Message.ToString();
                }
            }
            return res;
        }

        public LibResult InsertToNSP_ApprovalAuthority(List<NSP_ApprovalAuthority> AppList, NSP_PAYMENT_HEADER Hd, int LoginCode)
        {
            LibResult res = new LibResult();
            try
            {
                //using (var _TempDB = new LCEntities())
                //{

                List<NSP_APPROVAL_AUTHORITY> DeleList = _DB.NSP_APPROVAL_AUTHORITY.Where(x => x.HEADERID == Hd.HEADERID).ToList();

                if (DeleList.Count > 0)
                {
                    _DB.NSP_APPROVAL_AUTHORITY.RemoveRange(DeleList);
                    _DB.SaveChanges();
                }
                //}

                //using (var _appDb = new LCEntities())
                //{
                var maxAuthorityID = _DB.NSP_APPROVAL_AUTHORITY.Select(x => x.APPROVALID).DefaultIfEmpty().Max();
                int nextAuthorityID = maxAuthorityID + 1;
                int seq = 0;
                foreach (var authority in AppList)
                {
                    seq = seq + 1;
                    var approvalAuthority = new NSP_APPROVAL_AUTHORITY
                    {
                        APPROVALID = nextAuthorityID,
                        HEADERID = Hd.HEADERID,
                        EMPHEAD = authority.EMPHEAD,
                        EMPCODE = authority.EMPCODE,
                        APPROVALTYPE = authority.APPROVALTYPE,
                        EMP_NAME = authority.EMP_NAME,
                        DESIGNATION = authority.DESIGNATION,
                        DEPARTMENT = "APPROVAL",
                        APPSEQ = seq,
                        APPROVAL_STATUS = null,
                        ADDEDBY = LoginCode,
                        ADDEDON = DateTime.Now
                    };
                    _DB.Entry(approvalAuthority).State = EntityState.Added;

                    nextAuthorityID++;
                    res.hasError = false;
                }


                _DB.SaveChanges();
                //}

            }
            catch (Exception ex)
            {
                res.hasError = true;

                res.errorMessage = ex.Message.ToString();
            }

            return res;
        }

        public LibResult GETEmployee(string Key, string designation)
        {
            LibResult Res = new LibResult();
            try
            {
                long lngsykiid = Convert.ToInt64(_DB.SYKI_LC.Where(s => s.ACTIVE == 1).FirstOrDefault().SYKIID);


                if (designation == "Team Member")
                {
                    var elist = (from data in _DB.ADEMPLOYEE_LC
                                 join dds in _DB.VW_ASSOCIATELVLDETAILS_LC.Where(g => g.SYKI == lngsykiid) on data.ADEMPCODE equals dds.ADEMPCODE into tempdds
                                 from dds in tempdds.DefaultIfEmpty()
                                 join dsg in _DB.ADDESIGNATION_LC on dds.ADDESIGNATIONID equals dsg.ADDESIGNATIONID into tempdsg
                                 from dsg in tempdsg.DefaultIfEmpty()
                                 where (data.ADEMPCODE.ToString().ToUpper().Contains(Key.ToUpper())
                                 && !new string[] { "Section Head", "Department Head", "Coordinator", "Division Head", "executive Coordinator", "Operation Head", "Director", "President & CEO" }.Contains(dsg.DESCRIP)
                                 && !new string[] { "Section Head", "Department Head", "Coordinator", "Division Head", "executive Coordinator", "Operation Head", "Director", "President & CEO" }.Contains(dds.FUNCTIONALDESIGNATION)
                                 || data.FIRSTNAME.ToUpper().Contains(Key.ToUpper())
                                 || data.LASTNAME.ToUpper().Contains(Key.ToUpper()))
                                 select new
                                 { name = data.FIRSTNAME + " " + data.LASTNAME, ecode = data.ADEMPCODE, designation = dsg.DESCRIP }
                        ).ToList();

                    Res.resultObject = elist;
                }
                else
                {
                    var elist = (from data in _DB.ADEMPLOYEE_LC
                                 join dds in _DB.VW_ASSOCIATELVLDETAILS_LC.Where(g => g.SYKI == lngsykiid) on data.ADEMPCODE equals dds.ADEMPCODE into tempdds
                                 from dds in tempdds.DefaultIfEmpty()
                                 join dsg in _DB.ADDESIGNATION_LC on dds.ADDESIGNATIONID equals dsg.ADDESIGNATIONID into tempdsg
                                 from dsg in tempdsg.DefaultIfEmpty()
                                 where (data.ADEMPCODE.ToString().ToUpper().Contains(Key.ToUpper())
                                 || data.FIRSTNAME.ToUpper().Contains(Key.ToUpper())
                                 || data.LASTNAME.ToUpper().Contains(Key.ToUpper()))
                                 && data.ACTIVE == 1 && (designation == "" || dds.FUNCTIONALDESIGNATION == designation) || (dsg.DESCRIP == designation)
                                 select new
                                 { name = data.FIRSTNAME + " " + data.LASTNAME, ecode = data.ADEMPCODE, designation = dsg.DESCRIP }
                        ).ToList();

                    Res.resultObject = elist;
                }



            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Res;
        }
        public LibResult GetReferenceNumber(string Key)
        {
            LibResult Res = new LibResult();
            try
            {
                var maxRequestRefNo = _DB.NSP_PAYMENT_HEADER.Max(h => h.REQUESTREFNO);
                Res.resultObject = maxRequestRefNo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Res;
        }

        public long GetLoginEmpLocation(int Emplogin)
        {
            var SYSITEID_ = (from i in _DB.VW_ASSOCIATELVLDETAILS_LC
                             where i.ACTIVE == 1 && i.SYKI == _Syki && i.ADEMPCODE == Emplogin
                             select i.SYSITEID).FirstOrDefault();
            long SYSITEID = SYSITEID_ ?? 0;
            return SYSITEID;
        }


        public List<NSP_ApprovalAuthority> Gethead(long logincode, int RequestNo, string ApprovalType) // long? opMappId,
        {
            List<NSP_ApprovalAuthority> emp = new List<NSP_ApprovalAuthority>();






            var sectionHeadData = (from _vw in _DB.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == logincode)
                                   join _adhead in _DB.ADORGLEVELHEAD_LC on _vw.SECTIONID equals _adhead.ADORGLEVELID
                                   where _vw.SYKI == _Syki && _vw.ACTIVE == 1 && _adhead.ISACTIVE == 1
                                   select new
                                   {
                                       EMPCODE = _adhead.ADEMPCODE,
                                       EMP_NAME = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == _adhead.ADEMPCODE).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").FirstOrDefault(),
                                       DESIGNATION = _DB.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == _adhead.ADEMPCODE && x.SYKI == _Syki && x.ACTIVE == 1).Select(s => s.FUNCTIONALDESIGNATION).FirstOrDefault(),
                                   }).Distinct().FirstOrDefault();

            if (sectionHeadData != null && sectionHeadData.EMPCODE != logincode)
            {
                var sectionHead = new NSP_ApprovalAuthority
                {
                    EMPCODE = Convert.ToInt32(sectionHeadData.EMPCODE),
                    EMP_NAME = sectionHeadData.EMP_NAME,
                    DESIGNATION = sectionHeadData.DESIGNATION
                };

                emp.Add(sectionHead);
            }


            var DepartmentHeadData = (from _vw in _DB.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == logincode)
                                          //join _ad in _DB.ADORGLEVEL on _vw.DEPARTMENTID equals _ad.ADORGLEVELID
                                      join _adhead in _DB.ADORGLEVELHEAD_LC on _vw.DEPARTMENTID equals _adhead.ADORGLEVELID
                                      where _vw.SYKI == _Syki && _vw.ACTIVE == 1 && _adhead.ISACTIVE == 1
                                      select new
                                      {
                                          EMPCODE = _adhead.ADEMPCODE,
                                          EMP_NAME = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == _adhead.ADEMPCODE).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").FirstOrDefault(),
                                          DESIGNATION = _DB.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == _adhead.ADEMPCODE && x.SYKI == _Syki && x.ACTIVE == 1).Select(s => s.FUNCTIONALDESIGNATION).FirstOrDefault()
                                      }).Distinct().FirstOrDefault();
            if (DepartmentHeadData != null && DepartmentHeadData.EMPCODE != logincode)
            {
                var DepartmentHead = new NSP_ApprovalAuthority
                {
                    EMPCODE = Convert.ToInt32(DepartmentHeadData.EMPCODE),
                    EMP_NAME = DepartmentHeadData.EMP_NAME,
                    DESIGNATION = DepartmentHeadData.DESIGNATION
                };

                emp.Add(DepartmentHead);
            }

            var DivisionHeadData = (from _vw in _DB.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == logincode)
                                        //join _ad in _DB.ADORGLEVEL on _vw.DIVISIONID equals _ad.ADORGLEVELID
                                        //join _adhead in _DB.ADORGLEVELHEAD_LC on _ad.ADORGLEVELID equals _adhead.ADORGLEVELID
                                    join _adhead in _DB.ADORGLEVELHEAD_LC on _vw.DIVISIONID equals _adhead.ADORGLEVELID
                                    where _vw.SYKI == _Syki && _vw.ACTIVE == 1 && _adhead.ISACTIVE == 1
                                    select new
                                    {
                                        EMPCODE = _adhead.ADEMPCODE,
                                        EMP_NAME = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == _adhead.ADEMPCODE).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").FirstOrDefault(),
                                        DESIGNATION = _DB.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == _adhead.ADEMPCODE && x.SYKI == _Syki && x.ACTIVE == 1).Select(s => s.FUNCTIONALDESIGNATION).FirstOrDefault()
                                    }).Distinct().FirstOrDefault();
            if (DivisionHeadData != null && DivisionHeadData.EMPCODE != logincode)
            {
                var DivisionHead = new NSP_ApprovalAuthority
                {
                    EMPCODE = Convert.ToInt32(DivisionHeadData.EMPCODE),
                    EMP_NAME = DivisionHeadData.EMP_NAME,
                    DESIGNATION = DivisionHeadData.DESIGNATION
                };
                emp.Add(DivisionHead);
            }
            var OperationHeadData = (from _vw in _DB.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == logincode)
                                     join _ad in _DB.ADORGLEVELHEAD_LC on _vw.OPERATIONID equals _ad.ADORGLEVELID
                                     // join _V4 in _DB.ADORGCOORDINATOR on (opMappId == null || opMappId == 0 ? _vw.OPERATIONID : opMappId) equals _V4.ADORGLEVELID into _V4Join
                                     // from V4 in _V4Join.Where(o => o.ISACTIVE == 1).DefaultIfEmpty()
                                     where _vw.SYKI == _Syki && _vw.ACTIVE == 1 && _ad.ISACTIVE == 1
                                     select new
                                     {
                                         EMPCODE = _ad.ADEMPCODE,
                                         EMP_NAME = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == _ad.ADEMPCODE).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").FirstOrDefault(),
                                         DESIGNATION = _DB.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == _ad.ADEMPCODE && x.SYKI == _Syki && x.ACTIVE == 1).Select(s => s.FUNCTIONALDESIGNATION).FirstOrDefault()
                                     }).Distinct().FirstOrDefault();

            if (OperationHeadData != null && OperationHeadData.EMPCODE != logincode)
            {
                var OperationHead = new NSP_ApprovalAuthority
                {
                    EMPCODE = Convert.ToInt32(OperationHeadData.EMPCODE),
                    EMP_NAME = OperationHeadData.EMP_NAME,
                    DESIGNATION = OperationHeadData.DESIGNATION
                };
                emp.Add(OperationHead);
            }
            foreach (var data in emp)
            {
                if (data.DESIGNATION == null)
                {
                    var DatEmp = long.Parse(data.EMPCODE.ToString());
                    var designation = (from emp1 in _DB.ADEMPLOYEE_LC.Where(v => v.ADEMPCODE == DatEmp && v.ACTIVE == 1)
                                       join _VW in _DB.VW_ASSOCIATELVLDETAILS_LC on emp1.ADEMPCODE equals _VW.ADEMPCODE
                                       join _Desg in _DB.ADDESIGNATION_LC on _VW.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
                                       where _VW.SYKI == _Syki
                                       select (_Desg.DESCRIP)).FirstOrDefault();
                    data.DESIGNATION = designation;

                }
            }


            return emp;
        }

        public NSP_PaymentHeaderViewModel GetNSPEditDataById(long id, long loginCode)
        {
            NSP_PaymentHeaderViewModel _obj = new NSP_PaymentHeaderViewModel();
            try
            {
                _obj = (from data in _DB.NSP_PAYMENT_HEADER
                        where data.HEADERID == id
                        where data.ADDEDBY == loginCode
                        orderby data.REQUESTREFNO ascending
                        select new NSP_PaymentHeaderViewModel
                        {
                            HeaderID = data.HEADERID,
                            RequestRefNo = data.REQUESTREFNO,
                            RequestDate = data.REQUESTDATE,
                            Category = data.CATEGORY,
                            Location = data.LOCATION,
                            STATUS = data.STATUS,
                            VendorCode = data.VENDORCODE,
                            VendorName = data.VENDORNAME,
                            VendorPANNo = data.VENDORPANNO,
                            VendorAddress = data.VENDORADDRESS,
                            BankName = data.BANKNAME,
                            BankAccountNo = data.BANKACCOUNTNO,
                            BankIFSCCode = data.BANKIFSCCODE,
                            CostCentre = data.COSTCENTRE,
                            Budgeted = data.BUDGETED,
                            PurposeofExpenses = data.PURPOSEOFEXPENSES,
                            CurrencyType = data.CURRENCYTYPE,
                            RequestType = data.REQUESTTYPE,
                            TotalNetAmount = data.TOTALNETAMOUNT,
                            TotalNetAmountInWord = data.TOTALNETAMOUNTINWORD,
                            FY_ExRate = data.FY_EXRATE,
                            FY_GST = data.FY_GST,
                            FY_SGSTRATE = data.FY_SGSTRATE,
                            FY_TDS = data.FY_TDS,
                            FY_TDSRate = data.FY_TDSRATE,
                            FY_RequestType = data.REQUESTTYPE,
                            FY_CGSTRATE = data.FY_CGSTRATE,
                            FY_GSTType = data.FY_GSTTYPE,
                            FY_TDSREASON = data.FY_TDSREASON,
                            TOTAL_GST_AMOUNT = data.TOTAL_GST_AMOUNT,
                            FinanceRemark = data.FINANCEREMARK,
                            LOCATIONID = data.LOCATIONID,
                            PAY_PRO_LOCATION = data.PAY_PRO_LOCATION,
                            PAY_PRO_LOCATIONID = data.PAY_PRO_LOCATIONID,
                            BUDGETED_ATTACH = data.BUDGETED_ATTACH,
                            GSTIN = data.GSTIN,
                            TDSBaseAmount = data.TDSBASEAMOUNT,
                            TDSUnderSectionCode = data.TDS_UNDER_SECTIONCODE
                        }).FirstOrDefault();


                List<NSP_ApprovalAuthority> appAuthority = (from approval in _DB.NSP_APPROVAL_AUTHORITY.
                                                            Where(d => d.HEADERID == id)
                                                            select new NSP_ApprovalAuthority
                                                            {
                                                                HEADERID = approval.HEADERID,
                                                                APPROVALTYPE = approval.APPROVALTYPE,
                                                                APPSEQ = approval.APPSEQ,
                                                                EMPCODE = approval.EMPCODE,
                                                                EMP_NAME = approval.EMP_NAME,
                                                                EMPHEAD = approval.EMPHEAD,
                                                                DESIGNATION = approval.DESIGNATION,
                                                                APPROVAL_STATUS = approval.APPROVAL_STATUS,
                                                                APPREMARK = approval.APPREMARK,
                                                                UPDATEDON = approval.UPDATEDON,
                                                                Department = approval.DEPARTMENT
                                                            }).OrderBy(x => x.APPSEQ).ToList();

                List<NSP_PaymentDetailsViewModel> appInvoiceDetails = (from paymentDetail in _DB.NSP_PAYMENT_DETAIL.Where(e => e.HEADERID == id)
                                                                       select new NSP_PaymentDetailsViewModel
                                                                       {
                                                                           HeaderID = paymentDetail.HEADERID,
                                                                           DetailID = paymentDetail.DETAILID,
                                                                           InvoiceNo = paymentDetail.INVOICENO,
                                                                           InvoiceDate = paymentDetail.INVOICEDATE,
                                                                           Description = paymentDetail.DESCRIPTION,
                                                                           Currency = paymentDetail.CURRENCY,
                                                                           Amount = paymentDetail.AMOUNT,
                                                                           PO_Document = paymentDetail.PO_DOCUMENT,
                                                                           Invoice_Document = paymentDetail.INVOICE_DOCUMENT,
                                                                           ApprovalNote_Document = paymentDetail.APPROVALNOTE_DOCUMENT,
                                                                           Other_Document = paymentDetail.OTHER_DOCUMENT,
                                                                           GSTType = paymentDetail.GST_TYPE,
                                                                           BaseAmount = paymentDetail.BASE_AMOUNT,
                                                                           CGSTRate = paymentDetail.CGSTRATE,
                                                                           CGSTAmount = paymentDetail.CGSTAMOUNT,
                                                                           SGSTRate = paymentDetail.SGSTRATE,
                                                                           SGSTAmount = paymentDetail.SGSTAMOUNT,
                                                                           IGSTRate = paymentDetail.IGSTRATE,
                                                                           IGSTAmount = paymentDetail.IGSTAMOUNT,
                                                                           TotalGSTAmount = paymentDetail.TOTAL_GST_AMOUNT,
                                                                           TotalAmount = paymentDetail.TOTAL_AMOUNT,
                                                                           GSTTaxCode = paymentDetail.GSTTAXCODE
                                                                       }).ToList();


                _obj.appAuthority = appAuthority;
                _obj.appInvoiceDetails = appInvoiceDetails;


            }
            catch (Exception ex)
            {

                throw ex;
            }
            return _obj;
        }

        public bool NSPPaymentRequestDtRowDelete(int DetailID, int HeaderID)
        {
            bool res = false;
            NSP_PAYMENT_DETAIL dtRow = _DB.NSP_PAYMENT_DETAIL.Where(x => x.HEADERID == HeaderID && x.DETAILID == DetailID).FirstOrDefault();

            _DB.NSP_PAYMENT_DETAIL.Remove(dtRow);
            _DB.SaveChanges();
            res = true;
            return res;
        }

        public bool DeleteWholeRequest(int id)
        {
            bool res = false;
            NSP_PAYMENT_HEADER CrROle = _DB.NSP_PAYMENT_HEADER.Find(id);

            List<NSP_PAYMENT_DETAIL> dtlist = _DB.NSP_PAYMENT_DETAIL.Where(x => x.HEADERID == id).ToList();
            List<NSP_APPROVAL_AUTHORITY> Applist = _DB.NSP_APPROVAL_AUTHORITY.Where(x => x.HEADERID == id).ToList();
            _DB.NSP_PAYMENT_HEADER.Remove(CrROle);

            foreach (var item in dtlist)
            {
                _DB.NSP_PAYMENT_DETAIL.Remove(item);
            }
            foreach (var item in Applist)
            {
                _DB.NSP_APPROVAL_AUTHORITY.Remove(item);
            }

            _DB.SaveChanges();
            res = true;
            return res;
        }


        #endregion Request Page 

        #region  Approval
        public List<NSP_PaymentHeaderViewModel> GetApprovalNSPPaymentList(long loginCode, short? status, string FromDate, string ToDate)
        {
            try
            {
                DateTime _fDate = new DateTime();
                DateTime _tDate = new DateTime();

                if (FromDate != "" && FromDate != "All" && ToDate != "" && ToDate != "All")
                {
                    _fDate = DateTime.Parse(FromDate);
                    _tDate = DateTime.Parse(ToDate);
                }


                var query = from approvalAuthority in _DB.NSP_APPROVAL_AUTHORITY
                            join paymentHeader in _DB.NSP_PAYMENT_HEADER on approvalAuthority.HEADERID equals paymentHeader.HEADERID
                            where approvalAuthority.EMPCODE == loginCode
                            && approvalAuthority.APPROVAL_STATUS == status && approvalAuthority.DEPARTMENT == "APPROVAL"
                                  && (FromDate == "All" && ToDate == "All" ||
                      paymentHeader.ADDEDON >= _fDate && paymentHeader.ADDEDON <= _tDate)
                            orderby paymentHeader.REQUESTREFNO ascending
                            select new NSP_PaymentHeaderViewModel
                            {
                                HeaderID = paymentHeader.HEADERID,
                                RequestRefNo = paymentHeader.REQUESTREFNO,
                                RequestDate = paymentHeader.REQUESTDATE,
                                Category = paymentHeader.CATEGORY,
                                Location = paymentHeader.LOCATION,
                                STATUS = paymentHeader.STATUS,
                                TotalNetAmount = paymentHeader.TOTALNETAMOUNT,
                                ApprovalStatus = approvalAuthority.APPROVAL_STATUS,
                                ReqCode = paymentHeader.ADDEDBY,
                                VendorCode = paymentHeader.VENDORCODE,
                                VendorName = paymentHeader.VENDORNAME

                            };
                Console.WriteLine(query.ToString());
                var result = query.ToList();
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"An error occurred in GetApprovalNSPPaymentList: {ex.Message}");
                throw;
            }
        }

        public NSP_PaymentHeaderViewModel GetNSPById(long id, long loginCode, string Department)
        {
            NSP_PaymentHeaderViewModel _obj = new NSP_PaymentHeaderViewModel();
            try
            {
                int MaxApprovalSrno = 0;
                List<long?> FinanceUser = new List<long?>();
                List<long?> RoleSiteID_List = new List<long?>(); // ADDED BY AUMENTO :: SR88023-CR5305 3.0
                                                                 // long? RoleSiteID_ = 0; // COMMENTED BY AUMENTO :: SR88023-CR5305 3.0
                if (Department == "APPROVAL")
                {
                    MaxApprovalSrno = _DB.NSP_APPROVAL_AUTHORITY.Where(x => x.EMPCODE == loginCode && x.HEADERID == id).Max(x => x.APPROVALID);
                    FinanceUser.Add(loginCode);
                }
                else
                {
                    // RoleSiteID_ = (from r in _DB.NSP_ROLE_MASTER where r.ACTIVE == 1 && r.ECODE == loginCode && r.DEPARTMENT.ToUpper() == Department.ToUpper() select r.SYSITEID).FirstOrDefault(); // UPDATED BY AUMENTO :: SR88023-CR5305
                    RoleSiteID_List = (from r in _DB.NSP_ROLE_MASTER where r.ACTIVE == 1 && r.ECODE == loginCode && r.DEPARTMENT.ToUpper() == Department.ToUpper() select r.SYSITEID).ToList(); // UPDATED BY AUMENTO :: SR88023-CR5305 3.0
                    FinanceUser = (from k in _DB.NSP_ROLE_MASTER where k.ACTIVE == 1 && RoleSiteID_List.Contains(k.SYSITEID) && k.DEPARTMENT.ToUpper() == Department.ToUpper() select k.ECODE).ToList();  // UPDATED BY AUMENTO :: SR88023-CR5305

                    MaxApprovalSrno = _DB.NSP_APPROVAL_AUTHORITY.Where(x => x.HEADERID == id && FinanceUser.Contains(x.EMPCODE)).Max(x => x.APPROVALID);
                }






                _obj = (from data in _DB.NSP_PAYMENT_HEADER
                        where data.HEADERID == id
                        orderby data.REQUESTREFNO ascending
                        select new NSP_PaymentHeaderViewModel
                        {
                            HeaderID = data.HEADERID,
                            RequestRefNo = data.REQUESTREFNO,
                            RequestDate = data.REQUESTDATE,
                            Category = data.CATEGORY,
                            Location = data.LOCATION,
                            STATUS = data.STATUS,
                            VendorCode = data.VENDORCODE,
                            VendorName = data.VENDORNAME,
                            VendorPANNo = data.VENDORPANNO,
                            VendorAddress = data.VENDORADDRESS,
                            BankName = data.BANKNAME,
                            BankAccountNo = data.BANKACCOUNTNO,
                            BankIFSCCode = data.BANKIFSCCODE,
                            CostCentre = data.COSTCENTRE,
                            Budgeted = data.BUDGETED,
                            PurposeofExpenses = data.PURPOSEOFEXPENSES,
                            CurrencyType = data.CURRENCYTYPE,
                            RequestType = data.REQUESTTYPE,
                            FY_RequestType = data.REQUESTTYPE,
                            TotalNetAmount = data.TOTALNETAMOUNT,
                            TotalNetAmountInWord = data.TOTALNETAMOUNTINWORD,
                            FINAL_TOTAL = data.FINAL_TOTAL,
                            APPROVED_FINAL_TOTAL = data.APPROVED_FINAL_TOTAL,
                            FY_ExRate = data.FY_EXRATE,
                            FY_ExRateDate = data.FY_EXRATEDATE,
                            FY_Taxation = data.FY_TAXATION,
                            FY_GSTType = data.FY_GSTTYPE,
                            FY_GST = data.FY_GST,
                            FY_SGSTRATE = data.FY_SGSTRATE,
                            SGSTAMOUNT = data.SGSTAMOUNT,
                            FY_CGSTRATE = data.FY_CGSTRATE,
                            CGSTAMOUNT = data.CGSTAMOUNT,
                            FY_IGSTRATE = data.FY_IGSTRATE,
                            IGSTAMOUNT = data.IGSTAMOUNT,
                            FY_GSTREASON = data.FY_GSTREASON,
                            TOTAL_GST_AMOUNT = data.TOTAL_GST_AMOUNT,
                            FY_TDS = data.FY_TDS,
                            FY_TDSRate = data.FY_TDSRATE,
                            FY_TDS_AMOUNT = data.FY_TDS_AMOUNT,
                            FY_TDSREASON = data.FY_TDSREASON,
                            FinanceRemark = data.FINANCEREMARK,
                            TAXATIONREMARK = data.TAXATIONREMARK,
                            LOCATIONID = data.LOCATIONID,
                            PAY_PRO_LOCATION = data.PAY_PRO_LOCATION,
                            PAY_PRO_LOCATIONID = data.PAY_PRO_LOCATIONID,
                            BUDGETED_ATTACH = data.BUDGETED_ATTACH,
                            POSTING_DATE = data.POSTING_DATE,
                            GSTIN = data.GSTIN,
                            TDSBaseAmount = data.TDSBASEAMOUNT,
                            TDSUnderSectionCode = data.TDS_UNDER_SECTIONCODE
                        }).FirstOrDefault();


                //var approvalQuery = _DB.NSP_APPROVAL_AUTHORITY.Where(x => x.EMPCODE == loginCode && x.HEADERID == id && x.APPROVALID == MaxApprovalSrno);
                var approvalQuery = _DB.NSP_APPROVAL_AUTHORITY.Where(x => x.HEADERID == id && x.APPROVALID == MaxApprovalSrno);

                var ApprovalStatus = Department != "APPROVAL" ?
                    approvalQuery.Where(x => x.EMPHEAD == Department).Select(x => x.APPROVAL_STATUS).FirstOrDefault() :
                    approvalQuery.Select(x => x.APPROVAL_STATUS).FirstOrDefault();

                var APPREMARK = Department != "APPROVAL" ?
                    approvalQuery.Where(x => x.EMPHEAD == Department).Select(x => x.APPREMARK).FirstOrDefault() :
                    approvalQuery.Select(x => x.APPREMARK).FirstOrDefault();

                var appAuthority = (from approval in _DB.NSP_APPROVAL_AUTHORITY.
                                                 Where(d => d.HEADERID == id)
                                    select new NSP_ApprovalAuthority
                                    {
                                        HEADERID = approval.HEADERID,
                                        APPROVALTYPE = approval.APPROVALTYPE,
                                        APPSEQ = approval.APPSEQ,
                                        EMPCODE = approval.EMPCODE,
                                        EMP_NAME = approval.EMP_NAME,
                                        EMPHEAD = approval.EMPHEAD,
                                        DESIGNATION = approval.DESIGNATION,
                                        APPROVAL_STATUS = approval.APPROVAL_STATUS,
                                        APPREMARK = approval.APPREMARK,
                                        UPDATEDON = approval.UPDATEDON,
                                        Department = approval.DEPARTMENT
                                    }).OrderBy(x => x.APPSEQ).ToList();


                var appInvoiceDetails = (from paymentDetail in _DB.NSP_PAYMENT_DETAIL.Where(e => e.HEADERID == id)
                                         select new NSP_PaymentDetailsViewModel
                                         {
                                             HeaderID = paymentDetail.HEADERID,
                                             DetailID = paymentDetail.DETAILID,
                                             InvoiceNo = paymentDetail.INVOICENO,
                                             InvoiceDate = paymentDetail.INVOICEDATE,
                                             Description = paymentDetail.DESCRIPTION,
                                             Currency = paymentDetail.CURRENCY,
                                             Amount = paymentDetail.AMOUNT,
                                             PO_Document = paymentDetail.PO_DOCUMENT,
                                             Invoice_Document = paymentDetail.INVOICE_DOCUMENT,
                                             ApprovalNote_Document = paymentDetail.APPROVALNOTE_DOCUMENT,
                                             Other_Document = paymentDetail.OTHER_DOCUMENT,
                                             APPROVED_AMOUNT = paymentDetail.APPROVED_AMOUNT,
                                             GSTType = paymentDetail.GST_TYPE,
                                             BaseAmount = paymentDetail.BASE_AMOUNT,
                                             CGSTRate = paymentDetail.CGSTRATE,
                                             CGSTAmount = paymentDetail.CGSTAMOUNT,
                                             SGSTRate = paymentDetail.SGSTRATE,
                                             SGSTAmount = paymentDetail.SGSTAMOUNT,
                                             IGSTRate = paymentDetail.IGSTRATE,
                                             IGSTAmount = paymentDetail.IGSTAMOUNT,
                                             TotalGSTAmount = paymentDetail.TOTAL_GST_AMOUNT,
                                             TotalAmount = paymentDetail.TOTAL_AMOUNT,
                                             TDS = paymentDetail.TDS,
                                             TaxType = paymentDetail.TAX_TYPE,
                                             TDSRate = paymentDetail.TDSRATE,
                                             TDSBaseAmount = paymentDetail.TDS_BASE_AMOUNT,
                                             TDSAmount = paymentDetail.TDS_AMOUNT,
                                             NilTDSReason = paymentDetail.NILTDSREASON,
                                             NilGSTReason = paymentDetail.NILGSTREASON,
                                             TDSType = paymentDetail.TDS_TYPE,
                                             TotalNetAmount =
                                             paymentDetail.TOTAL_NET_AMOUNT,
                                             GSTTaxCode = paymentDetail.GSTTAXCODE,
                                             Split = "0",
                                             RowSrNo = paymentDetail.DETAILID,
                                             COMMISSION = paymentDetail.COMMISSION,
                                             COMMISSIONAMOUNT = paymentDetail.COMMISSIONAMOUNT,
                                             CESSRATE = paymentDetail.CESSRATE,
                                             CESSAMOUNT = paymentDetail.CESSAMOUNT,
                                             IS_ESI = paymentDetail.IS_ESI,
                                             ESI_RATE = paymentDetail.ESI_RATE,
                                             ESI_AMOUNT = paymentDetail.ESI_AMOUNT,
                                             DISCOUNT_AMOUNT = paymentDetail.DISCOUNT_AMOUNT

                                         }).ToList();

                _obj.ApprovalStatus = ApprovalStatus;
                _obj.ApprovalRemarks = APPREMARK;
                _obj.appAuthority = appAuthority;
                _obj.appInvoiceDetails = appInvoiceDetails;



            }
            catch (Exception ex)
            {

                throw ex;
            }
            return _obj;
        }

        public LibResult ApproveReject(int HEADERID, string EMPCODE, string Response, string Remark, long loginEmp)
        {
            LibResult Res = new LibResult();

            NSP_PAYMENT_HEADER hd = new NSP_PAYMENT_HEADER();

            try
            {
                //using (var _db = new LCEntities())
                //{

                using (var transaction = _DB.Database.BeginTransaction())  // ADDED BY AUMENTO :: SR88023-CR5305
                {  // ADDED BY AUMENTO :: SR88023-CR5305
                    hd = _DB.NSP_PAYMENT_HEADER.Where(x => x.HEADERID == HEADERID).FirstOrDefault();

                    var MaxAPPROVALID = _DB.NSP_APPROVAL_AUTHORITY.Where(x => x.HEADERID == HEADERID && x.EMPCODE == loginEmp && x.DEPARTMENT == "APPROVAL").Select(x => x.APPROVALID).DefaultIfEmpty().Max();

                    var CurrentAuthority = _DB.NSP_APPROVAL_AUTHORITY.Where(x => x.HEADERID == hd.HEADERID && x.EMPCODE == loginEmp && x.APPROVALID == MaxAPPROVALID && x.APPROVAL_STATUS == 1).FirstOrDefault();

                    int? CurrentAuthAppSeq = CurrentAuthority.APPSEQ;
                    int? NextAuthAppSeq = CurrentAuthAppSeq + 1;
                    short CurAppStatus = short.Parse(Response);

                    CurrentAuthority.APPROVAL_STATUS = CurAppStatus;
                    CurrentAuthority.APPREMARK = Remark;
                    CurrentAuthority.UPDATEDBY = int.Parse(loginEmp.ToString());
                    CurrentAuthority.UPDATEDON = DateTime.Now;
                    _DB.Entry(CurrentAuthority).State = EntityState.Modified;
                    _DB.SaveChanges();


                    if (CurAppStatus == 2)
                    {
                        var NextAuthority = _DB.NSP_APPROVAL_AUTHORITY.Where(x => x.HEADERID == hd.HEADERID && x.APPSEQ == NextAuthAppSeq).FirstOrDefault();
                        if (NextAuthority != null)
                        {
                            NextAuthority.APPROVAL_STATUS = 1;
                            _DB.Entry(NextAuthority).State = EntityState.Modified;
                            _DB.SaveChanges();
                            ///InsertToNSP_ApprovalAuthority_LogHistory(NextAuthority);

                            /// mail 
                            long Requester_ = long.Parse(hd.ADDEDBY.ToString());
                            long NextEmpcode = long.Parse(NextAuthority.EMPCODE.ToString());
                            ADEMPLOYEE_LC NextAuthority_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == NextEmpcode && x.ACTIVE == 1).FirstOrDefault();
                            ADEMPLOYEE_LC Requestet_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == Requester_ && x.ACTIVE == 1).FirstOrDefault();
                            SendMailToNextAuthority(Requestet_, NextAuthority_, 1); // For Approval From Approval
                            /// mail 
                        }
                    }
                    else
                    {
                        short? HeaderStatus =
                           CurAppStatus == (short)3 ? (short)5 :
                           CurAppStatus == (short)4 ? (short)6 :
                           CurAppStatus == (short)5 ? (short)3 :
                           CurAppStatus == (short)6 ? (short)7 :
                           (short)CurAppStatus;
                        hd.STATUS = HeaderStatus;
                    }

                    Res.hasError = false;
                    Res.errorMessage = "Submitted successfully";
                    var ApprovalCount = _DB.NSP_APPROVAL_AUTHORITY.Where(k => k.HEADERID == hd.HEADERID && k.DEPARTMENT == "APPROVAL").Count();
                    var ApprovedCount = _DB.NSP_APPROVAL_AUTHORITY.Where(k => k.HEADERID == hd.HEADERID && k.DEPARTMENT == "APPROVAL" && k.APPROVAL_STATUS == 2).Count();
                    NSP_APPROVAL_AUTHORITY App = new NSP_APPROVAL_AUTHORITY();
                    if (ApprovalCount > 0 && ApprovalCount == ApprovedCount)
                    {
                        App = GetFinanceOrTaxation(hd, "Finance");
                        if (App != null && App.EMPCODE != null)  // UPDATED BY AUMENTO :: SR88023-CR5305
                        {
                            App.APPREMARK = "";
                            App.ADDEDBY = int.Parse(loginEmp.ToString());
                            App.ADDEDON = DateTime.Now;
                            App.APPROVAL_STATUS = 1;
                            hd.STATUS = 2;
                            _DB.Entry(App).State = EntityState.Added;
                            _DB.SaveChanges();


                            //InsertToNSP_ApprovalAuthority_LogHistory(App);

                            /// mail 
                            long Requester_ = long.Parse(hd.ADDEDBY.ToString());
                            long NextEmpcode = long.Parse(App.EMPCODE.ToString());
                            ADEMPLOYEE_LC NextAuthority_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == NextEmpcode && x.ACTIVE == 1).FirstOrDefault();
                            ADEMPLOYEE_LC Requestet_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == Requester_ && x.ACTIVE == 1).FirstOrDefault();
                            SendMailToNextAuthority(Requestet_, NextAuthority_, 2); // For Finance From Approval
                        }
                        // START :: ADDED BY AUMENTO :: SR88023-CR5305
                        else
                        {

                            Res.hasError = true;
                            Res.errorMessage = "Finance is not defined in Role master against the selected location for payment processing.";
                            return Res;
                        }
                        // END :: ADDED BY AUMENTO :: SR88023-CR5305/// mail 

                    }

                    _DB.Entry(hd).State = EntityState.Modified;
                    _DB.SaveChanges();

                    /// mail 
                    long Empcode = long.Parse(CurrentAuthority.EMPCODE.ToString());
                    long Requester = long.Parse(hd.ADDEDBY.ToString());
                    ADEMPLOYEE_LC CurrAuthority_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == Empcode && x.ACTIVE == 1).FirstOrDefault();
                    ADEMPLOYEE_LC Requestet = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == Requester && x.ACTIVE == 1).FirstOrDefault();
                    SendMailToRequester(Requestet, CurrAuthority_, CurAppStatus, 1, Remark); // From Approval
                    /// mail 

                    transaction.Commit();  // ADDED BY AUMENTO :: SR88023-CR5305
                }


                //}
            }
            catch (Exception ex)
            {
                Res.hasError = true;
                Res.errorMessage = ex.Message.ToString();
                throw ex;
            }
            return Res;
        }

        #endregion APProval

        #region  Finance 
        public List<NSP_PaymentHeaderViewModel> GetNSPFinanceList(short? status, int? loginCode, string FromDate, string ToDate, string InvoiceSearchKey) // InvoiceSearchKey ADDED BY AUMENTO :: SR88023-CR5305
        {
            // START :: ADDED BY AUMENTO :: SR88023-CR5305
            List<NSP_PaymentHeaderViewModel> result = new List<NSP_PaymentHeaderViewModel>();
            try
            {
                //var RoleSiteID_ = (from r in _DB.NSP_ROLE_MASTER where r.ACTIVE == 1 && r.ECODE == loginCode && r.DEPARTMENT == "Finance" select r.SYSITEID).FirstOrDefault(); 
                //var FinanceUser = (from r in _DB.NSP_ROLE_MASTER where r.ACTIVE == 1 && r.SYSITEID == RoleSiteID_ && r.DEPARTMENT == "Finance" select r.ECODE).ToList();       
                //int RoleSiteID = (RoleSiteID_ != null ? int.Parse(RoleSiteID_.ToString()) : 0);                                                                               

                var RoleSiteIDs = (from r in _DB.NSP_ROLE_MASTER where r.ACTIVE == 1 && r.ECODE == loginCode && r.DEPARTMENT == "Finance" select r.SYSITEID).ToList();
                var FinanceUser = (from r in _DB.NSP_ROLE_MASTER where r.ACTIVE == 1 && RoleSiteIDs.Contains(r.SYSITEID) && r.DEPARTMENT == "Finance" select r.ECODE).ToList();
                // END :: ADDED BY AUMENTO :: SR88023-CR5305




                DateTime _tDate = DateTime.Parse(ToDate);
                DateTime _fDate = DateTime.Parse(FromDate);
                // START :: ADDED BY AUMENTO :: SR88023-CR5305
                var HeaderIDs = (from hd in _DB.NSP_PAYMENT_HEADER
                                 join dt in _DB.NSP_PAYMENT_DETAIL on hd.HEADERID equals dt.HEADERID
                                 where hd.PAY_PRO_LOCATIONID != null
                                       && RoleSiteIDs.Contains(hd.PAY_PRO_LOCATIONID.Value)
                                       && (string.IsNullOrEmpty(InvoiceSearchKey)
                                           || dt.INVOICENO.StartsWith(InvoiceSearchKey)
                                           || dt.INVOICENO.EndsWith(InvoiceSearchKey)
                                           || dt.INVOICENO == InvoiceSearchKey)
                                       //&& EntityFunctions.TruncateTime(hd.ADDEDON) >= _fDate.Date  //UPDATED BY AUMENTO :: SR88023-CR5305 :: 2.0
                                       //&& EntityFunctions.TruncateTime(hd.ADDEDON) <= _tDate.Date //UPDATED BY AUMENTO :: SR88023-CR5305 :: 2.0
                                           && hd.ADDEDON >= _fDate.Date
                                        && hd.ADDEDON <= _tDate.Date.AddDays(1).AddTicks(-1) // include full day
                                 orderby hd.HEADERID ascending
                                 select hd.HEADERID).Distinct().ToList();

                // END :: ADDED BY AUMENTO :: SR88023-CR5305


                var query = (from approvalAuthority in _DB.NSP_APPROVAL_AUTHORITY
                             join paymentHeader in _DB.NSP_PAYMENT_HEADER on approvalAuthority.HEADERID equals paymentHeader.HEADERID
                             join emp in _DB.ADEMPLOYEE_LC on (long)paymentHeader.ADDEDBY equals emp.ADEMPCODE
                             // where paymentHeader.PAY_PRO_LOCATION == RoleSiteID  // COMMENTED BY AUMENTO :: SR88023-CR5305
                             where paymentHeader.PAY_PRO_LOCATIONID != null && RoleSiteIDs.Contains(paymentHeader.PAY_PRO_LOCATIONID.Value)  // ADDED BY AUMENTO :: SR88023-CR5305
                             && HeaderIDs.Contains(paymentHeader.HEADERID)  // ADDED BY AUMENTO :: SR88023-CR5305
                             && FinanceUser.Contains(approvalAuthority.EMPCODE) //approvalAuthority.EMPCODE == loginCode 
                             && approvalAuthority.DEPARTMENT == "Finance"
                             && approvalAuthority.APPROVAL_STATUS == status
                            // && EntityFunctions.TruncateTime(paymentHeader.ADDEDON) >= _fDate.Date //  UPDATED BY AUMENTO :: SR88023-CR5305 :: 2.0
                            //&& EntityFunctions.TruncateTime(paymentHeader.ADDEDON) <= _tDate.Date //  UPDATED BY AUMENTO :: SR88023-CR5305 :: 2.0
                            && paymentHeader.ADDEDON >= _fDate.Date
                            && paymentHeader.ADDEDON <= _tDate.Date.AddDays(1).AddTicks(-1) // include full day
                             orderby paymentHeader.REQUESTREFNO ascending
                             select new NSP_PaymentHeaderViewModel
                             {
                                 HeaderID = paymentHeader.HEADERID,
                                 RequestRefNo = paymentHeader.REQUESTREFNO,
                                 RequestDate = paymentHeader.REQUESTDATE,
                                 Category = paymentHeader.CATEGORY,
                                 Location = paymentHeader.LOCATION,
                                 STATUS = paymentHeader.STATUS,
                                 TotalNetAmount = paymentHeader.TOTALNETAMOUNT,
                                 VendorCode = paymentHeader.VENDORCODE,
                                 VendorName = paymentHeader.VENDORNAME
                             }).OrderBy(x => x.HeaderID).ToList();


                // START :: ADDED BY AUMENTO :: SR88023-CR5305
                //            Console.WriteLine(query.ToString());
                //            var result = query.ToList();
                Console.WriteLine(query.ToString());
                result = query.ToList();
            }
            catch (Exception ex)
            {

            }
            // END :: ADDED BY AUMENTO :: SR88023-CR5305
            return result;
        }

        // START :: ADDED BY AUMENTO :: SR88023-CR5305
        //        public LibResult ApproveFinanceReject(NSP_PaymentHeaderViewModel obj, List<NSP_PaymentDetailsViewModel> detailsList, int loginEmp)
        //        {
        //            LibResult Res = new LibResult();
        //            string Response = obj.response;
        //
        //            decimal HDSGST_AMOUNT = 0;
        //            decimal HDCGST_AMOUNT = 0;
        //            decimal HDIGST_AMOUNT = 0;
        //            decimal HDTOTALGST_AMOUNT = 0;
        //            decimal HDTDSAMOUNT = 0;
        //            decimal HDTDSBASEAMOUNT = 0;
        //
        //
        //
        //            NSP_PAYMENT_HEADER hd = new NSP_PAYMENT_HEADER();
        //            try
        //            {
        //                int HEADERID_ = int.Parse(obj.HeaderID.ToString());
        //
        //                var RoleSiteID_ = (from r in _DB.NSP_ROLE_MASTER where r.ACTIVE == 1 && r.ECODE == loginEmp && r.DEPARTMENT == "Finance" select r.SYSITEID).FirstOrDefault();
        //                var FinanceUser = (from k in _DB.NSP_ROLE_MASTER where k.ACTIVE == 1 && k.SYSITEID == RoleSiteID_ && k.DEPARTMENT == "Finance" select k.ECODE).ToList();
        //
        //                using (var _db = new LCEntities())
        //                {
        //                    hd = _db.NSP_PAYMENT_HEADER.Where(x => x.HEADERID == HEADERID_).FirstOrDefault();
        //
        //                    var MaxAPPROVALID = _db.NSP_APPROVAL_AUTHORITY.Where(x => x.HEADERID == HEADERID_ 
        //                    //&& x.EMPCODE == loginEmp 
        //                    &&  FinanceUser.Contains(x.EMPCODE)
        //                    && x.DEPARTMENT == "Finance").Select(x => x.APPROVALID).DefaultIfEmpty(0).Max();
        //
        //                    var CurrentAuthority = _DB.NSP_APPROVAL_AUTHORITY.Where(x => x.HEADERID == hd.HEADERID 
        //                   // && x.EMPCODE == loginEmp 
        //                    && x.APPROVALID == MaxAPPROVALID && new short?[] { 1, 6 }.Contains(x.APPROVAL_STATUS)).FirstOrDefault();
        //
        //
        //                    short CurAppStatus = short.Parse(Response);
        //                    var EmpData = (from i in _DB.VW_ASSOCIATELVLDETAILS_LC
        //                                   join j in _DB.ADEMPLOYEE_LC on i.ADEMPCODE equals j.ADEMPCODE
        //                                   join d in _DB.ADDESIGNATION_LC on i.ADDESIGNATIONID equals d.ADDESIGNATIONID
        //                                   where
        //                                   i.ACTIVE == 1
        //                                   && j.ACTIVE == 1
        //                                   && i.SYKI == _Syki
        //                                   && i.ADEMPCODE == loginEmp
        //                                   select new
        //                                   {
        //
        //                                       ECODE = i.ADEMPCODE,
        //                                       ENAME = j.FIRSTNAME.ToString() + " " + j.LASTNAME.ToString(),
        //                                       DesiHead = i.FUNCTIONALDESIGNATION,
        //                                       Desi = d.DESCRIP,
        //
        //                                   }
        //                              ).FirstOrDefault();
        //
        //                    CurrentAuthority.APPROVAL_STATUS = CurAppStatus;
        //                    CurrentAuthority.UPDATEDBY = int.Parse(loginEmp.ToString());
        //                    CurrentAuthority.UPDATEDON = DateTime.Now;
        //                    CurrentAuthority.APPREMARK = obj.FinanceRemark;
        //                    CurrentAuthority.EMPCODE = loginEmp;
        //                    CurrentAuthority.EMP_NAME = EmpData.ENAME;
        //                    CurrentAuthority.DESIGNATION = EmpData.DesiHead;
        //
        //
        //                    _db.Entry(CurrentAuthority).State = EntityState.Modified;
        //                    _db.SaveChanges();
        //
        //                    if (detailsList.Count > 0)
        //                    {
        //                        
        //                        foreach (var item in detailsList)
        //                        {
        //                            NSP_PAYMENT_DETAIL Dtitem = (from i in _db.NSP_PAYMENT_DETAIL where i.HEADERID == HEADERID_ && i.INVOICENO == item.InvoiceNo && i.DETAILID ==  item.DetailID
        //                                                         &&  item.Split == "0"  select i ).FirstOrDefault();
        //
        //                            if (Dtitem != null)
        //                            {
        //                                
        //                                HDSGST_AMOUNT = HDSGST_AMOUNT + (item.SGSTAmount == null ? 0 : decimal.Parse(item.SGSTAmount.ToString()));
        //                                HDCGST_AMOUNT = HDCGST_AMOUNT + (item.CGSTAmount == null ? 0 : decimal.Parse(item.CGSTAmount.ToString()));
        //                                HDIGST_AMOUNT = HDIGST_AMOUNT + (item.IGSTAmount == null ? 0 : decimal.Parse(item.IGSTAmount.ToString()));
        //                                HDTOTALGST_AMOUNT = HDTOTALGST_AMOUNT + (item.TotalGSTAmount == null ? 0 : decimal.Parse(item.TotalGSTAmount.ToString()));
        //                                HDTDSAMOUNT = HDTDSAMOUNT + (item.TDSAmount == null ? 0 : decimal.Parse(item.TDSAmount.ToString()));
        //                                HDTDSBASEAMOUNT = HDTDSBASEAMOUNT + (item.TDSBaseAmount == null ? 0 : decimal.Parse(item.TDSBaseAmount.ToString()));
        //
        //
        //                                Dtitem.APPROVED_AMOUNT = item.APPROVED_AMOUNT;
        //                                Dtitem.AMOUNT = item.Amount;
        //                                Dtitem.BASE_AMOUNT = item.BaseAmount;
        //                                Dtitem.TAX_TYPE = item.TaxType;
        //                                Dtitem.GST_TYPE = item.GSTType;
        //                                Dtitem.NILGSTREASON = item.NilGSTReason;
        //                                Dtitem.SGSTRATE = item.SGSTRate;
        //                                Dtitem.SGSTAMOUNT = item.SGSTAmount;
        //                                Dtitem.CGSTRATE = item.CGSTRate;
        //                                Dtitem.CGSTAMOUNT = item.CGSTAmount;
        //                                Dtitem.IGSTRATE = item.IGSTRate;
        //                                Dtitem.IGSTAMOUNT = item.IGSTAmount;
        //                                Dtitem.TOTAL_GST_AMOUNT = item.TotalGSTAmount;
        //                                Dtitem.TDS = item.TDS;
        //                                Dtitem.NILTDSREASON = item.NilTDSReason;
        //                                Dtitem.TDS_TYPE = item.TDSType;
        //                                Dtitem.TDS_BASE_AMOUNT = item.TDSBaseAmount;
        //                                Dtitem.TDSRATE = item.TDSRate;
        //                                Dtitem.TDS_AMOUNT = item.TDSAmount;
        //                                Dtitem.TOTAL_NET_AMOUNT = item.TotalNetAmount;
        //                                Dtitem.ISPOSTED = "N";
        //                                Dtitem.GSTTAXCODE = item.GSTTaxCode;
        //                                Dtitem.COMMISSION = item.COMMISSION;
        //                                Dtitem.COMMISSIONAMOUNT = item.COMMISSIONAMOUNT;
        //                                Dtitem.CESSRATE = item.CESSRATE;
        //                                Dtitem.CESSAMOUNT = item.CESSAMOUNT;
        //                                Dtitem.IS_ESI = item.IS_ESI;
        //                                Dtitem.ESI_RATE = item.ESI_RATE;
        //                                Dtitem.ESI_AMOUNT = item.ESI_AMOUNT;
        //                                Dtitem.DISCOUNT_AMOUNT = item.DISCOUNT_AMOUNT;
        //                                
        //                                _db.Entry(Dtitem).State = EntityState.Modified;
        //                            }
        //
        //                            else
        //                            {
        //                                
        //                                HDSGST_AMOUNT = HDSGST_AMOUNT + (item.SGSTAmount == null ? 0 : decimal.Parse(item.SGSTAmount.ToString()));
        //                                HDCGST_AMOUNT = HDCGST_AMOUNT + (item.CGSTAmount == null ? 0 : decimal.Parse(item.CGSTAmount.ToString()));
        //                                HDIGST_AMOUNT = HDIGST_AMOUNT + (item.IGSTAmount == null ? 0 : decimal.Parse(item.IGSTAmount.ToString()));
        //                                HDTOTALGST_AMOUNT = HDTOTALGST_AMOUNT + (item.TotalGSTAmount == null ? 0 : decimal.Parse(item.TotalGSTAmount.ToString()));
        //                                HDTDSAMOUNT = HDTDSAMOUNT + (item.TDSAmount == null ? 0 : decimal.Parse(item.TDSAmount.ToString()));
        //                                HDTDSBASEAMOUNT = HDTDSBASEAMOUNT + (item.TDSBaseAmount == null ? 0 : decimal.Parse(item.TDSBaseAmount.ToString()));
        //                                var NewDt = new NSP_PAYMENT_DETAIL();
        //
        //                                NSP_PAYMENT_DETAIL splieddt = (from i in _db.NSP_PAYMENT_DETAIL
        //                                                             where i.HEADERID == HEADERID_ && i.INVOICENO == item.InvoiceNo && i.DETAILID == item.DetailID
        //                                                             select i).FirstOrDefault();
        //
        //                                var MAxDtID = _db.NSP_PAYMENT_DETAIL.Max(x => x.DETAILID);
        //                                MAxDtID++;                          
        //                                NewDt.DETAILID = MAxDtID;
        //                                NewDt.HEADERID = splieddt.HEADERID;
        //                                NewDt.INVOICENO = splieddt.INVOICENO;
        //                                NewDt.INVOICEDATE = splieddt.INVOICEDATE;
        //                                NewDt.CURRENCY = splieddt.CURRENCY;
        //                                NewDt.APPROVED_AMOUNT =item.APPROVED_AMOUNT;
        //                                NewDt.BASE_AMOUNT = item.BaseAmount;
        //                                NewDt.TAX_TYPE = item.TaxType;
        //                                NewDt.GST_TYPE = item.GSTType;
        //                                NewDt.NILGSTREASON = item.NilGSTReason;
        //                                NewDt.SGSTRATE = item.SGSTRate;
        //                                NewDt.SGSTAMOUNT = item.SGSTAmount;
        //                                NewDt.CGSTRATE = item.CGSTRate;
        //                                NewDt.CGSTAMOUNT = item.CGSTAmount;
        //                                NewDt.IGSTRATE = item.IGSTRate;
        //                                NewDt.IGSTAMOUNT = item.IGSTAmount;
        //                                NewDt.TOTAL_GST_AMOUNT = item.TotalGSTAmount;
        //                                NewDt.TDS = item.TDS;
        //                                NewDt.NILTDSREASON = item.NilTDSReason;
        //                                NewDt.TDS_TYPE = item.TDSType;
        //                                NewDt.TDS_BASE_AMOUNT = item.TDSBaseAmount;
        //                                NewDt.TDSRATE = item.TDSRate;
        //                                NewDt.TDS_AMOUNT = item.TDSAmount;
        //                                NewDt.TOTAL_NET_AMOUNT = item.TotalNetAmount;
        //                                NewDt.ISPOSTED = "N";
        //                                NewDt.TOTAL_AMOUNT = item.TotalAmount;
        //                                NewDt.GSTTAXCODE = item.GSTTaxCode;
        //                                NewDt.ADDEDBY = loginEmp;
        //                                NewDt.ADDEDON = DateTime.Now;
        //                                NewDt.AMOUNT = item.Amount;
        //                                NewDt.APPROVALNOTE_DOCUMENT = splieddt.APPROVALNOTE_DOCUMENT;
        //                                NewDt.INVOICE_DOCUMENT = splieddt.INVOICE_DOCUMENT;
        //                                NewDt.OTHER_DOCUMENT = splieddt.OTHER_DOCUMENT;
        //                                NewDt.PO_DOCUMENT = splieddt.PO_DOCUMENT;
        //
        //                                NewDt.COMMISSION = item.COMMISSION;
        //                                NewDt.COMMISSIONAMOUNT = item.COMMISSIONAMOUNT;
        //                                NewDt.CESSRATE = item.CESSRATE;
        //                                NewDt.CESSAMOUNT = item.CESSAMOUNT;
        //                                NewDt.IS_ESI = item.IS_ESI;
        //                                NewDt.ESI_RATE = item.ESI_RATE;
        //                                NewDt.ESI_AMOUNT = item.ESI_AMOUNT;
        //                                NewDt.DISCOUNT_AMOUNT = item.DISCOUNT_AMOUNT;
        //                                _db.Entry(NewDt).State = EntityState.Added;
        //
        //                            }
        //                        }
        //                        _db.SaveChanges();
        //                    }
        //
        //
        //
        //                    if (CurAppStatus == 2)
        //                    {
        //
        //                        hd.STATUS = 4;
        //                        hd.POSTING_DATE = obj.POSTING_DATE;
        //
        //                    }
        //                    else if (CurAppStatus == 5)
        //                    {
        //                        NSP_APPROVAL_AUTHORITY App = new NSP_APPROVAL_AUTHORITY();
        //
        //                        App = GetFinanceOrTaxation(hd, "Taxation");
        //                        if (App.EMPCODE != null)
        //                        {
        //                            App.APPREMARK = "";
        //                            App.ADDEDBY = Int32.Parse(loginEmp.ToString());
        //                            App.ADDEDON = DateTime.Now;
        //                            App.APPROVAL_STATUS = 1;
        //                            hd.STATUS = 3;
        //                            _db.Entry(App).State = EntityState.Added;
        //                            _db.SaveChanges();
        //                            // InsertToNSP_ApprovalAuthority_LogHistory(App);
        //
        //
        //                            /// mail 
        //                            long Requester_ = long.Parse(hd.ADDEDBY.ToString());
        //                            long NextEmpcode = long.Parse(App.EMPCODE.ToString());
        //                            ADEMPLOYEE_LC Taxation = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == NextEmpcode && x.ACTIVE == 1).FirstOrDefault();
        //                            ADEMPLOYEE_LC Finance = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == loginEmp && x.ACTIVE == 1).FirstOrDefault();
        //                            ADEMPLOYEE_LC Requestet_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == Requester_ && x.ACTIVE == 1).FirstOrDefault();
        //                            SendMailToFromTaxation(Requestet_, Finance, Taxation, "Taxation"); // For taxation From Finance
        //                        }                                                      /// mail 
        //
        //                    }
        //                    else
        //                    {
        //                        short? HeaderStatus =
        //                            CurAppStatus == (short)3 ? (short)5 :                  // STATUS	       HEADER	AUTHORITY
        //                            CurAppStatus == (short)4 ? (short)6 :                  // Pending			0			1
        //                            CurAppStatus == (short)5 ? (short)3 :                  // Approve			4			2
        //                            CurAppStatus == (short)6 ? (short)7 :                  // SendBack		    5			3
        //                            (short)CurAppStatus;                                   // Reject			6			4
        //                        hd.STATUS = HeaderStatus;                                  // SentToTaxation	3			5
        //                                                                                   // Hold			    7			6
        //                    }
        //
        //
        //
        //                    hd.COSTCENTRE = obj.CostCentre;
        //                    hd.FY_EXRATE = obj.FY_ExRate;
        //                    hd.FY_EXRATEDATE = obj.FY_ExRateDate;
        //                    hd.FY_TAXATION = obj.FY_Taxation;
        //
        //                    //hd.FY_GSTTYPE = obj.FY_GSTType;
        //                    //hd.FY_GST = obj.FY_GST;
        //                    //hd.FY_SGSTRATE = obj.FY_SGSTRATE;
        //                    //hd.SGSTAMOUNT = obj.SGSTAMOUNT;
        //                    //hd.FY_CGSTRATE = obj.FY_CGSTRATE;
        //                    //hd.CGSTAMOUNT = obj.CGSTAMOUNT;
        //                    //hd.FY_IGSTRATE = obj.FY_IGSTRATE;
        //                    //hd.IGSTAMOUNT = obj.IGSTAMOUNT;
        //                    //hd.TOTAL_GST_AMOUNT = obj.TOTAL_GST_AMOUNT;
        //                    //hd.FY_GSTREASON = obj.FY_GSTREASON;
        //                    //hd.FY_TDS = obj.FY_TDS;
        //                    //hd.FY_TDSRATE = obj.FY_TDSRate;
        //                    //hd.FY_TDS_AMOUNT = obj.FY_TDS_AMOUNT;
        //                    //hd.FY_TDSREASON = obj.FY_TDSREASON;
        //                    hd.SGSTAMOUNT = HDSGST_AMOUNT;
        //                    hd.CGSTAMOUNT = HDCGST_AMOUNT;
        //                    hd.IGSTAMOUNT = HDIGST_AMOUNT;
        //                    hd.TOTAL_GST_AMOUNT = HDTOTALGST_AMOUNT;
        //                    hd.FY_TDS_AMOUNT = HDTDSAMOUNT;
        //                    hd.TDSBASEAMOUNT = HDTDSBASEAMOUNT;
        //
        //
        //
        //                    hd.FINAL_TOTAL = obj.FINAL_TOTAL;
        //                    hd.APPROVED_FINAL_TOTAL = obj.FINAL_TOTAL;
        //                    hd.FINANCEREMARK = obj.FinanceRemark;
        //
        //                    //hd.TDSBASEAMOUNT = obj.TDSBaseAmount;
        //                    //hd.TDS_UNDER_SECTIONCODE = obj.TDSUnderSectionCode;
        //
        //                    Res.hasError = false;
        //                    Res.errorMessage = "Submitted Successfully";
        //
        //                    _db.Entry(hd).State = EntityState.Modified;
        //                    _db.SaveChanges();
        //
        //
        //
        //
        //                    if (CurAppStatus != 5)
        //                    {
        //                        /// mail 
        //                        long Empcode = long.Parse(CurrentAuthority.EMPCODE.ToString());
        //                        long Requester = long.Parse(hd.ADDEDBY.ToString());
        //                        ADEMPLOYEE_LC CurrAuthority_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == Empcode && x.ACTIVE == 1).FirstOrDefault();
        //                        ADEMPLOYEE_LC Requestet = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == Requester && x.ACTIVE == 1).FirstOrDefault();
        //                        SendMailToRequester(Requestet, CurrAuthority_, CurAppStatus, 2, obj.FinanceRemark);  // From Finance
        //                                                                                                             /// mail
        //                    }
        //
        //                }
        //
        //
        //
        //
        //
        //
        //            }
        //            catch (Exception ex)
        //            {
        //                Res.hasError = true;
        //                Res.errorMessage = ex.Message.ToString();
        //                throw ex;
        //            }
        //            return Res;
        //        }

        public LibResult ApproveFinanceReject(NSP_PaymentHeaderViewModel obj, List<NSP_PaymentDetailsViewModel> detailsList, int loginEmp)
        {
            LibResult Res = new LibResult();
            string Response = obj.response;

            decimal HDSGST_AMOUNT = 0;
            decimal HDCGST_AMOUNT = 0;
            decimal HDIGST_AMOUNT = 0;
            decimal HDTOTALGST_AMOUNT = 0;
            decimal HDTDSAMOUNT = 0;
            decimal HDTDSBASEAMOUNT = 0;

            NSP_PAYMENT_HEADER hd = new NSP_PAYMENT_HEADER();
            try
            {

                //using (var _db = new LCEntities())
                //{

                    using (var transaction = _DB.Database.BeginTransaction())
                    {

                        int HEADERID_ = int.Parse(obj.HeaderID.ToString());

                        var RoleSiteID_ = (from r in _DB.NSP_ROLE_MASTER where r.ACTIVE == 1 && r.ECODE == loginEmp && r.DEPARTMENT == "Finance" select r.SYSITEID).FirstOrDefault();
                        var FinanceUser = (from k in _DB.NSP_ROLE_MASTER where k.ACTIVE == 1 && k.SYSITEID == RoleSiteID_ && k.DEPARTMENT == "Finance" select k.ECODE).ToList();


                        hd = _DB.NSP_PAYMENT_HEADER.Where(x => x.HEADERID == HEADERID_).FirstOrDefault();

                        var MaxAPPROVALID = _DB.NSP_APPROVAL_AUTHORITY.Where(x => x.HEADERID == HEADERID_
                        //&& x.EMPCODE == loginEmp 
                        && FinanceUser.Contains(x.EMPCODE)
                        && x.DEPARTMENT == "Finance").Select(x => x.APPROVALID).DefaultIfEmpty().Max();

                        var CurrentAuthority = _DB.NSP_APPROVAL_AUTHORITY.Where(x => x.HEADERID == hd.HEADERID
                        // && x.EMPCODE == loginEmp 
                        && x.APPROVALID == MaxAPPROVALID && new short?[] { 1, 6 }.Contains(x.APPROVAL_STATUS)).FirstOrDefault();


                        short CurAppStatus = short.Parse(Response);
                        var EmpData = (from i in _DB.VW_ASSOCIATELVLDETAILS_LC
                                       join j in _DB.ADEMPLOYEE_LC on i.ADEMPCODE equals j.ADEMPCODE
                                       join d in _DB.ADDESIGNATION_LC on i.ADDESIGNATIONID equals d.ADDESIGNATIONID
                                       where
                                       i.ACTIVE == 1
                                       && j.ACTIVE == 1
                                       && i.SYKI == _Syki
                                       && i.ADEMPCODE == loginEmp
                                       select new
                                       {

                                           ECODE = i.ADEMPCODE,
                                           ENAME = j.FIRSTNAME.ToString() + " " + j.LASTNAME.ToString(),
                                           DesiHead = i.FUNCTIONALDESIGNATION,
                                           Desi = d.DESCRIP,

                                       }
                                  ).FirstOrDefault();

                        CurrentAuthority.APPROVAL_STATUS = CurAppStatus;
                        CurrentAuthority.UPDATEDBY = int.Parse(loginEmp.ToString());
                        CurrentAuthority.UPDATEDON = DateTime.Now;
                        CurrentAuthority.APPREMARK = obj.FinanceRemark;
                        CurrentAuthority.EMPCODE = loginEmp;
                        CurrentAuthority.EMP_NAME = EmpData.ENAME;
                        CurrentAuthority.DESIGNATION = EmpData.DesiHead;


                    _DB.Entry(CurrentAuthority).State = EntityState.Modified;
                    _DB.SaveChanges();

                        if (detailsList.Count > 0)
                        {
                            foreach (var item in detailsList)
                            {
                                // Only handle non-split records for updating
                                NSP_PAYMENT_DETAIL Dtitem = (from i in _DB.NSP_PAYMENT_DETAIL
                                                             where i.HEADERID == HEADERID_ && i.INVOICENO == item.InvoiceNo && i.DETAILID == item.DetailID
                                                             && item.Split == "0"
                                                             select i).FirstOrDefault();


                                if (Dtitem != null)
                                {

                                    HDSGST_AMOUNT += item.SGSTAmount ?? 0;
                                    HDCGST_AMOUNT += item.CGSTAmount ?? 0;
                                    HDIGST_AMOUNT += item.IGSTAmount ?? 0;
                                    HDTOTALGST_AMOUNT += item.TotalGSTAmount ?? 0;
                                    HDTDSAMOUNT += item.TDSAmount ?? 0;
                                    HDTDSBASEAMOUNT += item.TDSBaseAmount ?? 0;
                                    Dtitem.APPROVED_AMOUNT = item.APPROVED_AMOUNT;
                                    Dtitem.AMOUNT = item.Amount;
                                    Dtitem.BASE_AMOUNT = item.BaseAmount;
                                    Dtitem.TAX_TYPE = item.TaxType;
                                    Dtitem.GST_TYPE = item.GSTType;
                                    Dtitem.NILGSTREASON = item.NilGSTReason;
                                    Dtitem.SGSTRATE = item.SGSTRate;
                                    Dtitem.SGSTAMOUNT = item.SGSTAmount;
                                    Dtitem.CGSTRATE = item.CGSTRate;
                                    Dtitem.CGSTAMOUNT = item.CGSTAmount;
                                    Dtitem.IGSTRATE = item.IGSTRate;
                                    Dtitem.IGSTAMOUNT = item.IGSTAmount;
                                    Dtitem.TOTAL_GST_AMOUNT = item.TotalGSTAmount;
                                    Dtitem.TDS = item.TDS;
                                    Dtitem.NILTDSREASON = item.NilTDSReason;
                                    Dtitem.TDS_TYPE = item.TDSType;
                                    Dtitem.TDS_BASE_AMOUNT = item.TDSBaseAmount;
                                    Dtitem.TDSRATE = item.TDSRate;
                                    Dtitem.TDS_AMOUNT = item.TDSAmount;
                                    Dtitem.TOTAL_NET_AMOUNT = item.TotalNetAmount;
                                    Dtitem.ISPOSTED = "N";
                                    Dtitem.GSTTAXCODE = item.GSTTaxCode;
                                    Dtitem.COMMISSION = item.COMMISSION;
                                    Dtitem.COMMISSIONAMOUNT = item.COMMISSIONAMOUNT;
                                    Dtitem.CESSRATE = item.CESSRATE;
                                    Dtitem.CESSAMOUNT = item.CESSAMOUNT;
                                    Dtitem.IS_ESI = item.IS_ESI;
                                    Dtitem.ESI_RATE = item.ESI_RATE;
                                    Dtitem.ESI_AMOUNT = item.ESI_AMOUNT;
                                    Dtitem.DISCOUNT_AMOUNT = item.DISCOUNT_AMOUNT;
                                _DB.Entry(Dtitem).State = EntityState.Modified;
                                }
                                else
                                {
                                    // If the record doesn't exist (likely a split record), create a new split record

                                    HDSGST_AMOUNT += item.SGSTAmount ?? 0;
                                    HDCGST_AMOUNT += item.CGSTAmount ?? 0;
                                    HDIGST_AMOUNT += item.IGSTAmount ?? 0;
                                    HDTOTALGST_AMOUNT += item.TotalGSTAmount ?? 0;
                                    HDTDSAMOUNT += item.TDSAmount ?? 0;
                                    HDTDSBASEAMOUNT += item.TDSBaseAmount ?? 0;


                                    NSP_PAYMENT_DETAIL splieddt = (from i in _DB.NSP_PAYMENT_DETAIL
                                                                   where i.HEADERID == HEADERID_ && i.INVOICENO == item.InvoiceNo && i.DETAILID == item.DetailID
                                                                   select i).FirstOrDefault();
                                    if (splieddt != null)
                                    {

                                        var NewDt = new NSP_PAYMENT_DETAIL();

                                        //var NewDetailID = _DB.Database.SqlQuery<int>("SELECT SEQ_NSP_PAYMENT_DETAIL.NEXTVAL FROM DUAL").FirstOrDefault();
                                    var result = _DB.Set<NSP_NEXTVAL_MODEL>()
                                                 .FromSqlRaw("SELECT SEQ_NSP_PAYMENT_DETAIL.NEXTVAL AS NEXTVAL FROM DUAL")
                                                 .AsEnumerable()
                                                 .FirstOrDefault();
                                    int NewDetailID = result?.NEXTVAL ?? 0;
                                    //var MaxDetailID = _db.NSP_PAYMENT_DETAIL.Max(x => x.DETAILID);
                                    NewDt.DETAILID = NewDetailID;
                                        NewDt.HEADERID = splieddt.HEADERID;
                                        NewDt.INVOICENO = splieddt.INVOICENO;
                                        NewDt.INVOICEDATE = splieddt.INVOICEDATE;
                                        NewDt.CURRENCY = splieddt.CURRENCY;
                                        NewDt.APPROVED_AMOUNT = item.APPROVED_AMOUNT;
                                        NewDt.BASE_AMOUNT = item.BaseAmount;
                                        NewDt.TAX_TYPE = item.TaxType;
                                        NewDt.GST_TYPE = item.GSTType;
                                        NewDt.NILGSTREASON = item.NilGSTReason;
                                        NewDt.SGSTRATE = item.SGSTRate;
                                        NewDt.SGSTAMOUNT = item.SGSTAmount;
                                        NewDt.CGSTRATE = item.CGSTRate;
                                        NewDt.CGSTAMOUNT = item.CGSTAmount;
                                        NewDt.IGSTRATE = item.IGSTRate;
                                        NewDt.IGSTAMOUNT = item.IGSTAmount;
                                        NewDt.TOTAL_GST_AMOUNT = item.TotalGSTAmount;
                                        NewDt.TDS = item.TDS;
                                        NewDt.NILTDSREASON = item.NilTDSReason;
                                        NewDt.TDS_TYPE = item.TDSType;
                                        NewDt.TDS_BASE_AMOUNT = item.TDSBaseAmount;
                                        NewDt.TDSRATE = item.TDSRate;
                                        NewDt.TDS_AMOUNT = item.TDSAmount;
                                        NewDt.TOTAL_NET_AMOUNT = item.TotalNetAmount;
                                        NewDt.ISPOSTED = "N";
                                        NewDt.TOTAL_AMOUNT = item.TotalAmount;
                                        NewDt.GSTTAXCODE = item.GSTTaxCode;
                                        NewDt.ADDEDBY = loginEmp;
                                        NewDt.ADDEDON = DateTime.Now;
                                        NewDt.AMOUNT = item.Amount;
                                        NewDt.APPROVALNOTE_DOCUMENT = splieddt.APPROVALNOTE_DOCUMENT;
                                        NewDt.INVOICE_DOCUMENT = splieddt.INVOICE_DOCUMENT;
                                        NewDt.OTHER_DOCUMENT = splieddt.OTHER_DOCUMENT;
                                        NewDt.PO_DOCUMENT = splieddt.PO_DOCUMENT;
                                        NewDt.COMMISSION = item.COMMISSION;
                                        NewDt.COMMISSIONAMOUNT = item.COMMISSIONAMOUNT;
                                        NewDt.CESSRATE = item.CESSRATE;
                                        NewDt.CESSAMOUNT = item.CESSAMOUNT;
                                        NewDt.IS_ESI = item.IS_ESI;
                                        NewDt.ESI_RATE = item.ESI_RATE;
                                        NewDt.ESI_AMOUNT = item.ESI_AMOUNT;
                                        NewDt.DISCOUNT_AMOUNT = item.DISCOUNT_AMOUNT;


                                    _DB.Entry(NewDt).State = EntityState.Added;
                                    }
                                }
                            }

                        _DB.SaveChanges();
                        }



                        if (CurAppStatus == 2)
                        {

                            hd.STATUS = 4;
                            hd.POSTING_DATE = obj.POSTING_DATE;

                        }
                        else if (CurAppStatus == 5)
                        {
                            NSP_APPROVAL_AUTHORITY App = new NSP_APPROVAL_AUTHORITY();

                            App = GetFinanceOrTaxation(hd, "Taxation");
                            if (App != null && App.EMPCODE != null) // UPDATED BY AUMENTO :: SR88023-CR5305
                            {
                                App.APPREMARK = "";
                                App.ADDEDBY = Int32.Parse(loginEmp.ToString());
                                App.ADDEDON = DateTime.Now;
                                App.APPROVAL_STATUS = 1;
                                hd.STATUS = 3;
                            _DB.Entry(App).State = EntityState.Added;
                            _DB.SaveChanges();

                                /// mail 
                                long Requester_ = long.Parse(hd.ADDEDBY.ToString());
                                long NextEmpcode = long.Parse(App.EMPCODE.ToString());
                                ADEMPLOYEE_LC Taxation = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == NextEmpcode && x.ACTIVE == 1).FirstOrDefault();
                                ADEMPLOYEE_LC Finance = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == loginEmp && x.ACTIVE == 1).FirstOrDefault();
                                ADEMPLOYEE_LC Requestet_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == Requester_ && x.ACTIVE == 1).FirstOrDefault();
                                SendMailToFromTaxation(Requestet_, Finance, Taxation, "Taxation"); // For taxation From Finance
                            }
                            // START :: ADDED BY AUMENTO :: SR88023-CR5305
                            else
                            {

                                Res.hasError = true;
                                Res.errorMessage = "Taxation is not defined in Role master against the selected location for payment processing.";
                                return Res;
                            }
                            // END :: ADDED BY AUMENTO :: SR88023-CR5305
                            /// mail 

                        }
                        else
                        {
                            short? HeaderStatus =
                                CurAppStatus == (short)3 ? (short)5 :                  // STATUS	       HEADER	AUTHORITY
                                CurAppStatus == (short)4 ? (short)6 :                  // Pending			0			1
                                CurAppStatus == (short)5 ? (short)3 :                  // Approve			4			2
                                CurAppStatus == (short)6 ? (short)7 :                  // SendBack		    5			3
                                (short)CurAppStatus;                                   // Reject			6			4
                            hd.STATUS = HeaderStatus;                                  // SentToTaxation	3			5
                                                                                       // Hold			    7			6
                        }



                        hd.COSTCENTRE = obj.CostCentre;
                        hd.FY_EXRATE = obj.FY_ExRate;
                        hd.FY_EXRATEDATE = obj.FY_ExRateDate;
                        hd.FY_TAXATION = obj.FY_Taxation;
                        hd.SGSTAMOUNT = HDSGST_AMOUNT;
                        hd.CGSTAMOUNT = HDCGST_AMOUNT;
                        hd.IGSTAMOUNT = HDIGST_AMOUNT;
                        hd.TOTAL_GST_AMOUNT = HDTOTALGST_AMOUNT;
                        hd.FY_TDS_AMOUNT = HDTDSAMOUNT;
                        hd.TDSBASEAMOUNT = HDTDSBASEAMOUNT;



                        hd.FINAL_TOTAL = obj.FINAL_TOTAL;
                        hd.APPROVED_FINAL_TOTAL = obj.FINAL_TOTAL;
                        hd.FINANCEREMARK = obj.FinanceRemark;
                        Res.hasError = false;
                        Res.errorMessage = "Submitted Successfully";
                        _DB.Entry(hd).State = EntityState.Modified;
                        _DB.SaveChanges();

                        if (CurAppStatus != 5)
                        {
                            /// mail 
                            long Empcode = long.Parse(CurrentAuthority.EMPCODE.ToString());
                            long Requester = long.Parse(hd.ADDEDBY.ToString());
                            ADEMPLOYEE_LC CurrAuthority_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == Empcode && x.ACTIVE == 1).FirstOrDefault();
                            ADEMPLOYEE_LC Requestet = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == Requester && x.ACTIVE == 1).FirstOrDefault();
                            SendMailToRequester(Requestet, CurrAuthority_, CurAppStatus, 2, obj.FinanceRemark);  // From Finance
                            /// mail
                        }
                        transaction.Commit();
                    }


                //}
            }
            catch (Exception ex)
            {
                Res.hasError = true;
                Res.errorMessage = ex.Message.ToString();
                throw ex;
            }
            return Res;
        }
        // END :: ADDED BY AUMENTO :: SR88023-CR5305

        #endregion  Finane

        #region Taxation 
        public List<NSP_PaymentHeaderViewModel> GetNSPTaxationList(short? status, int? loginCode, string FromDate, string ToDate)
        {
            //  START :: ADDED BY AUMENTO :: SR88023-CR5305 :: 2.0
            //var RoleSiteID_ = (from r in _DB.NSP_ROLE_MASTER where r.ACTIVE == 1 && r.ECODE == loginCode && r.DEPARTMENT == "Taxation" select r.SYSITEID).FirstOrDefault();
            //var FinanceUser = (from r in _DB.NSP_ROLE_MASTER where r.ACTIVE == 1 && r.SYSITEID == RoleSiteID_ && r.DEPARTMENT == "Taxation" select r.ECODE).ToList();
            //int RoleSiteID = (RoleSiteID_ != null ? int.Parse(RoleSiteID_.ToString()) : 0);

            var RoleSiteIDs = (from r in _DB.NSP_ROLE_MASTER where r.ACTIVE == 1 && r.ECODE == loginCode && r.DEPARTMENT == "Taxation" select r.SYSITEID).ToList();
            var FinanceUser = (from r in _DB.NSP_ROLE_MASTER where r.ACTIVE == 1 && RoleSiteIDs.Contains(r.SYSITEID) && r.DEPARTMENT == "Taxation" select r.ECODE).ToList();
            //  END :: ADDED BY AUMENTO :: SR88023-CR5305 :: 2.0
            DateTime _fDate = DateTime.Parse(FromDate);
            DateTime _tDate = DateTime.Parse(ToDate);

            var query = from approvalAuthority in _DB.NSP_APPROVAL_AUTHORITY
                        join paymentHeader in _DB.NSP_PAYMENT_HEADER on approvalAuthority.HEADERID equals paymentHeader.HEADERID
                        // where paymentHeader.PAY_PRO_LOCATIONID == RoleSiteID //  COMMENTED BY AUMENTO :: SR88023-CR5305 :: 2.0
                        where paymentHeader.PAY_PRO_LOCATIONID != null && RoleSiteIDs.Contains(paymentHeader.PAY_PRO_LOCATIONID.Value)  // ADDED BY AUMENTO :: SR88023-CR5305 :: 2.0
                         && FinanceUser.Contains(approvalAuthority.EMPCODE) //approvalAuthority.EMPCODE == loginCode 
                        && approvalAuthority.APPROVAL_STATUS == status && approvalAuthority.DEPARTMENT == "Taxation"
                          //&& EntityFunctions.TruncateTime(paymentHeader.ADDEDON) >= _fDate.Date //  UPDATED BY AUMENTO :: SR88023-CR5305 :: 2.0
                          //&& EntityFunctions.TruncateTime(paymentHeader.ADDEDON) <= _tDate.Date //  UPDATED BY AUMENTO :: SR88023-CR5305 :: 2.0

                            && paymentHeader.ADDEDON >= _fDate.Date
                            && paymentHeader.ADDEDON <= _tDate.Date.AddDays(1).AddTicks(-1) // include full day

                        orderby paymentHeader.REQUESTREFNO ascending

                        select new NSP_PaymentHeaderViewModel
                        {
                            HeaderID = paymentHeader.HEADERID,
                            RequestRefNo = paymentHeader.REQUESTREFNO,
                            RequestDate = paymentHeader.REQUESTDATE,
                            Category = paymentHeader.CATEGORY,
                            Location = paymentHeader.LOCATION,
                            STATUS = paymentHeader.STATUS,
                            TotalNetAmount = paymentHeader.TOTALNETAMOUNT,
                            VendorCode = paymentHeader.VENDORCODE,
                            VendorName = paymentHeader.VENDORNAME
                        };
            Console.WriteLine(query.ToString());
            var result = query.OrderBy(x => x.HeaderID).ToList();
            return result;
        }

        public LibResult ApproveTaxationReject(NSP_PaymentHeaderViewModel obj, List<NSP_PaymentDetailsViewModel> detailsList, int loginEmp)
        {
            LibResult Res = new LibResult();
            string Response = obj.response;

            decimal HDSGST_AMOUNT = 0;
            decimal HDCGST_AMOUNT = 0;
            decimal HDIGST_AMOUNT = 0;
            decimal HDTOTALGST_AMOUNT = 0;
            decimal HDTDSAMOUNT = 0;
            decimal HDTDSBASEAMOUNT = 0;

            NSP_PAYMENT_HEADER hd = new NSP_PAYMENT_HEADER();
            try
            {
                //  START :: ADDED BY AUMENTO :: SR88023-CR5305 :: 2.0
                //using (var _db = new LCEntities())
                //{

                    using (var transaction = _DB.Database.BeginTransaction())
                    {
                        int HEADERID_ = int.Parse(obj.HeaderID.ToString());

                        var RoleSiteID_ = (from r in _DB.NSP_ROLE_MASTER where r.ACTIVE == 1 && r.ECODE == loginEmp && r.DEPARTMENT == "Taxation" select r.SYSITEID).FirstOrDefault();
                        var FinanceUser = (from k in _DB.NSP_ROLE_MASTER where k.ACTIVE == 1 && k.SYSITEID == RoleSiteID_ && k.DEPARTMENT == "Taxation" select k.ECODE).ToList();


                        hd = _DB.NSP_PAYMENT_HEADER.Where(x => x.HEADERID == HEADERID_).FirstOrDefault();

                        var MaxAPPROVALID = _DB.NSP_APPROVAL_AUTHORITY.Where(x => x.HEADERID == HEADERID_
                          && FinanceUser.Contains(x.EMPCODE)  //&& x.EMPCODE == loginEmp 
                        && x.DEPARTMENT == "Taxation").Select(x => x.APPROVALID).DefaultIfEmpty().Max();

                        var CurrentAuthority = _DB.NSP_APPROVAL_AUTHORITY.Where(x => x.HEADERID == hd.HEADERID
                        //&& x.EMPCODE == loginEmp 
                        && x.APPROVALID == MaxAPPROVALID && x.APPROVAL_STATUS == 1).FirstOrDefault();

                        short CurAppStatus = short.Parse(Response);
                        var EmpData = (from i in _DB.VW_ASSOCIATELVLDETAILS_LC
                                       join j in _DB.ADEMPLOYEE_LC on i.ADEMPCODE equals j.ADEMPCODE
                                       join d in _DB.ADDESIGNATION_LC on i.ADDESIGNATIONID equals d.ADDESIGNATIONID
                                       where
                                       i.ACTIVE == 1
                                       && j.ACTIVE == 1
                                       && i.SYKI == _Syki
                                       && i.ADEMPCODE == loginEmp
                                       select new
                                       {

                                           ECODE = i.ADEMPCODE,
                                           ENAME = j.FIRSTNAME.ToString() + " " + j.LASTNAME.ToString(),
                                           DesiHead = i.FUNCTIONALDESIGNATION,
                                           Desi = d.DESCRIP,

                                       }
                                  ).FirstOrDefault();
                        CurrentAuthority.APPROVAL_STATUS = CurAppStatus;
                        CurrentAuthority.APPREMARK = obj.TAXATIONREMARK;
                        CurrentAuthority.UPDATEDBY = int.Parse(loginEmp.ToString());
                        CurrentAuthority.UPDATEDON = DateTime.Now;
                        CurrentAuthority.EMPCODE = int.Parse(loginEmp.ToString());
                        CurrentAuthority.EMP_NAME = EmpData.ENAME;
                        CurrentAuthority.DESIGNATION = EmpData.DesiHead;
                        _DB.Entry(CurrentAuthority).State = EntityState.Modified;
                        _DB.SaveChanges();


                        if (detailsList.Count > 0)
                        {
                            foreach (var item in detailsList)
                            {
                                // Only handle non-split records for updating
                                NSP_PAYMENT_DETAIL Dtitem = (from i in _DB.NSP_PAYMENT_DETAIL
                                                             where i.HEADERID == HEADERID_ && i.INVOICENO == item.InvoiceNo && i.DETAILID == item.DetailID
                                                             && item.Split == "0"
                                                             select i).FirstOrDefault();


                                if (Dtitem != null)
                                {

                                    HDSGST_AMOUNT += item.SGSTAmount ?? 0;
                                    HDCGST_AMOUNT += item.CGSTAmount ?? 0;
                                    HDIGST_AMOUNT += item.IGSTAmount ?? 0;
                                    HDTOTALGST_AMOUNT += item.TotalGSTAmount ?? 0;
                                    HDTDSAMOUNT += item.TDSAmount ?? 0;
                                    HDTDSBASEAMOUNT += item.TDSBaseAmount ?? 0;
                                    Dtitem.APPROVED_AMOUNT = item.APPROVED_AMOUNT;
                                    Dtitem.AMOUNT = item.Amount;
                                    Dtitem.BASE_AMOUNT = item.BaseAmount;
                                    Dtitem.TAX_TYPE = item.TaxType;
                                    Dtitem.GST_TYPE = item.GSTType;
                                    Dtitem.NILGSTREASON = item.NilGSTReason;
                                    Dtitem.SGSTRATE = item.SGSTRate;
                                    Dtitem.SGSTAMOUNT = item.SGSTAmount;
                                    Dtitem.CGSTRATE = item.CGSTRate;
                                    Dtitem.CGSTAMOUNT = item.CGSTAmount;
                                    Dtitem.IGSTRATE = item.IGSTRate;
                                    Dtitem.IGSTAMOUNT = item.IGSTAmount;
                                    Dtitem.TOTAL_GST_AMOUNT = item.TotalGSTAmount;
                                    Dtitem.TDS = item.TDS;
                                    Dtitem.NILTDSREASON = item.NilTDSReason;
                                    Dtitem.TDS_TYPE = item.TDSType;
                                    Dtitem.TDS_BASE_AMOUNT = item.TDSBaseAmount;
                                    Dtitem.TDSRATE = item.TDSRate;
                                    Dtitem.TDS_AMOUNT = item.TDSAmount;
                                    Dtitem.TOTAL_NET_AMOUNT = item.TotalNetAmount;
                                    Dtitem.ISPOSTED = "N";
                                    Dtitem.GSTTAXCODE = item.GSTTaxCode;
                                    Dtitem.COMMISSION = item.COMMISSION;
                                    Dtitem.COMMISSIONAMOUNT = item.COMMISSIONAMOUNT;
                                    Dtitem.CESSRATE = item.CESSRATE;
                                    Dtitem.CESSAMOUNT = item.CESSAMOUNT;
                                    Dtitem.IS_ESI = item.IS_ESI;
                                    Dtitem.ESI_RATE = item.ESI_RATE;
                                    Dtitem.ESI_AMOUNT = item.ESI_AMOUNT;
                                    Dtitem.DISCOUNT_AMOUNT = item.DISCOUNT_AMOUNT;

                                    _DB.Entry(Dtitem).State = EntityState.Modified;

                                }
                                else
                                {
                                    // If the record doesn't exist (likely a split record), create a new split record

                                    HDSGST_AMOUNT += item.SGSTAmount ?? 0;
                                    HDCGST_AMOUNT += item.CGSTAmount ?? 0;
                                    HDIGST_AMOUNT += item.IGSTAmount ?? 0;
                                    HDTOTALGST_AMOUNT += item.TotalGSTAmount ?? 0;
                                    HDTDSAMOUNT += item.TDSAmount ?? 0;
                                    HDTDSBASEAMOUNT += item.TDSBaseAmount ?? 0;


                                    NSP_PAYMENT_DETAIL splieddt = (from i in _DB.NSP_PAYMENT_DETAIL
                                                                   where i.HEADERID == HEADERID_ && i.INVOICENO == item.InvoiceNo && i.DETAILID == item.DetailID
                                                                   select i).FirstOrDefault();
                                    if (splieddt != null)
                                    {

                                        var NewDt = new NSP_PAYMENT_DETAIL();

                                        //var NewDetailID = _DB.Database.SqlQuery<int>("SELECT SEQ_NSP_PAYMENT_DETAIL.NEXTVAL FROM DUAL").FirstOrDefault();
                                        var result = _DB.Set<NSP_NEXTVAL_MODEL>()
                                                     .FromSqlRaw("SELECT SEQ_NSP_PAYMENT_DETAIL.NEXTVAL AS NEXTVAL FROM DUAL")
                                                     .AsEnumerable()
                                                     .FirstOrDefault();
                                    int NewDetailID = result?.NEXTVAL ?? 0;
                                    //var MaxDetailID = _db.NSP_PAYMENT_DETAIL.Max(x => x.DETAILID);
                                    NewDt.DETAILID = NewDetailID;
                                        NewDt.HEADERID = splieddt.HEADERID;
                                        NewDt.INVOICENO = splieddt.INVOICENO;
                                        NewDt.INVOICEDATE = splieddt.INVOICEDATE;
                                        NewDt.CURRENCY = splieddt.CURRENCY;
                                        NewDt.APPROVED_AMOUNT = item.APPROVED_AMOUNT;
                                        NewDt.BASE_AMOUNT = item.BaseAmount;
                                        NewDt.TAX_TYPE = item.TaxType;
                                        NewDt.GST_TYPE = item.GSTType;
                                        NewDt.NILGSTREASON = item.NilGSTReason;
                                        NewDt.SGSTRATE = item.SGSTRate;
                                        NewDt.SGSTAMOUNT = item.SGSTAmount;
                                        NewDt.CGSTRATE = item.CGSTRate;
                                        NewDt.CGSTAMOUNT = item.CGSTAmount;
                                        NewDt.IGSTRATE = item.IGSTRate;
                                        NewDt.IGSTAMOUNT = item.IGSTAmount;
                                        NewDt.TOTAL_GST_AMOUNT = item.TotalGSTAmount;
                                        NewDt.TDS = item.TDS;
                                        NewDt.NILTDSREASON = item.NilTDSReason;
                                        NewDt.TDS_TYPE = item.TDSType;
                                        NewDt.TDS_BASE_AMOUNT = item.TDSBaseAmount;
                                        NewDt.TDSRATE = item.TDSRate;
                                        NewDt.TDS_AMOUNT = item.TDSAmount;
                                        NewDt.TOTAL_NET_AMOUNT = item.TotalNetAmount;
                                        NewDt.ISPOSTED = "N";
                                        NewDt.TOTAL_AMOUNT = item.TotalAmount;
                                        NewDt.GSTTAXCODE = item.GSTTaxCode;
                                        NewDt.ADDEDBY = loginEmp;
                                        NewDt.ADDEDON = DateTime.Now;
                                        NewDt.AMOUNT = item.Amount;
                                        NewDt.APPROVALNOTE_DOCUMENT = splieddt.APPROVALNOTE_DOCUMENT;
                                        NewDt.INVOICE_DOCUMENT = splieddt.INVOICE_DOCUMENT;
                                        NewDt.OTHER_DOCUMENT = splieddt.OTHER_DOCUMENT;
                                        NewDt.PO_DOCUMENT = splieddt.PO_DOCUMENT;
                                        NewDt.COMMISSION = item.COMMISSION;
                                        NewDt.COMMISSIONAMOUNT = item.COMMISSIONAMOUNT;
                                        NewDt.CESSRATE = item.CESSRATE;
                                        NewDt.CESSAMOUNT = item.CESSAMOUNT;
                                        NewDt.IS_ESI = item.IS_ESI;
                                        NewDt.ESI_RATE = item.ESI_RATE;
                                        NewDt.ESI_AMOUNT = item.ESI_AMOUNT;
                                        NewDt.DISCOUNT_AMOUNT = item.DISCOUNT_AMOUNT;

                                        _DB.Entry(NewDt).State = EntityState.Added;
                                    }
                                }
                            }

                            _DB.SaveChanges();
                        }


                        if (CurAppStatus == 2)
                        {
                            NSP_APPROVAL_AUTHORITY App = new NSP_APPROVAL_AUTHORITY();

                            App = GetFinanceOrTaxation(hd, "Finance");
                            if (App != null && App.EMPCODE != null) // UPDATED BY AUMENTO :: SR88023-CR5305
                            {
                                App.APPREMARK = "";
                                App.ADDEDBY = Int32.Parse(loginEmp.ToString());
                                App.ADDEDON = DateTime.Now;
                                App.APPROVAL_STATUS = 1;
                                hd.STATUS = 2;
                                _DB.Entry(App).State = EntityState.Added;
                                _DB.SaveChanges();
                                // InsertToNSP_ApprovalAuthority_LogHistory(App);

                                /// mail 
                                long Requester_ = long.Parse(hd.ADDEDBY.ToString());
                                long NextEmpcode = long.Parse(App.EMPCODE.ToString());
                                ADEMPLOYEE_LC Finance = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == NextEmpcode && x.ACTIVE == 1).FirstOrDefault();
                                ADEMPLOYEE_LC Taxation = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == loginEmp && x.ACTIVE == 1).FirstOrDefault();
                                ADEMPLOYEE_LC Requestet_ = _DB.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == Requester_ && x.ACTIVE == 1).FirstOrDefault();
                                SendMailToFromTaxation(Requestet_, Finance, Taxation, "Finance");  // For Finance  From taxation
                                /// mail 
                            }
                            // START :: ADDED BY AUMENTO :: SR88023-CR5305
                            else
                            {

                                Res.hasError = true;
                                Res.errorMessage = "Finance is not defined in Role master against the selected location for payment processing.";
                                return Res;
                            }
                            // END :: ADDED BY AUMENTO :: SR88023-CR5305

                        }
                        else
                        {
                            short? HeaderStatus =
                                CurAppStatus == (short)3 ? (short)5 :                  // STATUS	       HEADER	AUTHORITY
                                CurAppStatus == (short)4 ? (short)6 :                  // Pending			0			1
                                CurAppStatus == (short)5 ? (short)3 :                  // Approve			4			2
                                CurAppStatus == (short)6 ? (short)7 :                  // SendBack		    5			3
                                (short)CurAppStatus;                                   // Reject			6			4
                            hd.STATUS = HeaderStatus;                                  // SentToTaxation	3			5
                                                                                       // Hold			    7			6
                        }


                        //hd.FY_GSTTYPE = obj.FY_GSTType;
                        //hd.FY_GST = obj.FY_GST;
                        //hd.FY_SGSTRATE = obj.FY_SGSTRATE;
                        //hd.SGSTAMOUNT = obj.SGSTAMOUNT;
                        //hd.FY_CGSTRATE = obj.FY_CGSTRATE;
                        //hd.CGSTAMOUNT = obj.CGSTAMOUNT;
                        //hd.FY_IGSTRATE = obj.FY_IGSTRATE;
                        //hd.IGSTAMOUNT = obj.IGSTAMOUNT;
                        //hd.TOTAL_GST_AMOUNT = obj.TOTAL_GST_AMOUNT;
                        //hd.FY_GSTREASON = obj.FY_GSTREASON;

                        //hd.FY_TDS = obj.FY_TDS;
                        //hd.FY_TDSRATE = obj.FY_TDSRate;
                        //hd.FY_TDS_AMOUNT = obj.FY_TDS_AMOUNT;
                        //hd.FY_TDSREASON = obj.FY_TDSREASON;
                        hd.SGSTAMOUNT = HDSGST_AMOUNT;
                        hd.CGSTAMOUNT = HDCGST_AMOUNT;
                        hd.IGSTAMOUNT = HDIGST_AMOUNT;
                        hd.TOTAL_GST_AMOUNT = HDTOTALGST_AMOUNT;
                        hd.FY_TDS_AMOUNT = HDTDSAMOUNT;
                        hd.TDSBASEAMOUNT = HDTDSBASEAMOUNT;

                        hd.FINAL_TOTAL = obj.FINAL_TOTAL;
                        hd.APPROVED_FINAL_TOTAL = obj.FINAL_TOTAL;
                        hd.TAXATIONREMARK = obj.TAXATIONREMARK;
                        //hd.TDSBASEAMOUNT = obj.TDSBaseAmount;
                        //hd.TDS_UNDER_SECTIONCODE = obj.TDSUnderSectionCode;
                        Res.hasError = false;
                        Res.errorMessage = "Submitted Successfully";

                        _DB.Entry(hd).State = EntityState.Modified;
                        _DB.SaveChanges();

                        transaction.Commit();
                    }


                //}
                //  END :: ADDED BY AUMENTO :: SR88023-CR5305 :: 2.0
            }
            catch (Exception ex)
            {
                Res.hasError = true;
                Res.errorMessage = ex.Message.ToString();
                throw ex;
            }
            return Res;
        }

        #endregion Taxation

        #region Common Function
        public List<NSP_ApprovalAuthority> GetNSPHistoryById(long id)
        {
            List<NSP_ApprovalAuthority> Ilist = new List<NSP_ApprovalAuthority>();


            try
            {

                Ilist = (from approval in _DB.NSP_APPROVAL_AUTHORITY.Where(d => d.HEADERID == id)
                         select new NSP_ApprovalAuthority
                         {
                             HEADERID = approval.HEADERID,
                             EMPCODE = approval.EMPCODE,
                             EMP_NAME = approval.EMP_NAME,
                             EMPHEAD = approval.EMPHEAD,
                             DESIGNATION = approval.DESIGNATION,
                             APPREMARK = approval.APPREMARK,
                             UPDATEDON = approval.UPDATEDON,
                             ADDEDON = approval.ADDEDON,
                             APPROVAL_STATUS = approval.APPROVAL_STATUS,
                             APPSEQ = approval.APPSEQ,
                             APPROVALID = approval.APPROVALID,
                             Department = approval.DEPARTMENT
                         }).OrderBy(n => n.APPROVALID).ToList();



            }
            catch (Exception ex)
            {


                throw ex;
            }



            return Ilist;
        }

        public NSP_APPROVAL_AUTHORITY GetFinanceOrTaxation(NSP_PAYMENT_HEADER hd, string DepartHead)
        {
            NSP_APPROVAL_AUTHORITY App = new NSP_APPROVAL_AUTHORITY();

            //string StrRequetorSiteId = (from i in _DB.VW_ASSOCIATELVLDETAILS_LC where i.ACTIVE == 1 && i.SYKI == _Syki && i.ADEMPCODE == hd.ADDEDBY select i.SYSITEID).FirstOrDefault().ToString();
            //long RequetorSiteId = long.Parse(StrRequetorSiteId);
            long RequetorSiteId = long.Parse(hd.PAY_PRO_LOCATIONID.ToString());

            //string StrECode = (from k in _DB.NSP_ROLE_MASTER where k.ACTIVE == 1 && k.SYSITEID == RequetorSiteId && k.DEPARTMENT == DepartHead select k.ECODE).FirstOrDefault().ToString();
            //if (StrECode != "")
            //{
            var StrECode = _DB.NSP_ROLE_MASTER.Where(k => k.ACTIVE == 1 && k.SYSITEID == RequetorSiteId && k.DEPARTMENT == DepartHead).Select(k => k.ECODE).FirstOrDefault();   // UPDATED BY AUMENTO :: SR88023-CR5305

            if (StrECode != null)
            {


                int eCode_ = int.Parse(StrECode.ToString());  // UPDATED BY AUMENTO :: SR88023-CR5305

                var EmpData = (from i in _DB.VW_ASSOCIATELVLDETAILS_LC
                               join j in _DB.ADEMPLOYEE_LC on i.ADEMPCODE equals j.ADEMPCODE
                               join d in _DB.ADDESIGNATION_LC on i.ADDESIGNATIONID equals d.ADDESIGNATIONID
                               where
                               i.ACTIVE == 1
                               && j.ACTIVE == 1
                               && i.SYKI == _Syki
                               && i.ADEMPCODE == eCode_
                               select new
                               {

                                   ECODE = i.ADEMPCODE,
                                   ENAME = j.FIRSTNAME.ToString() + " " + j.LASTNAME.ToString(),
                                   DesiHead = (d.DESCRIP == null ? "" : d.DESCRIP),   // UPDATED BY AUMENTO :: SR88023-CR5305
                                   Desi = d.DESCRIP,

                               }
                               ).FirstOrDefault();


                int appseqmax = _DB.NSP_APPROVAL_AUTHORITY.Where(x => x.HEADERID == hd.HEADERID).Select(x => x.APPSEQ).Max(x => x.Value);


                int approvalidmax = _DB.NSP_APPROVAL_AUTHORITY.Select(x => x.APPROVALID).Max();

                App.EMPCODE = eCode_;
                App.EMP_NAME = EmpData.ENAME;
                App.HEADERID = hd.HEADERID;
                App.EMPHEAD = DepartHead.ToUpper();
                App.DESIGNATION = (EmpData.DesiHead == null || EmpData.DesiHead == "" ? EmpData.Desi : EmpData.DesiHead);
                App.APPSEQ = appseqmax + 1;
                App.APPROVALID = approvalidmax + 1;
                App.APPROVALTYPE = "1";
                App.APPROVAL_STATUS = 1;
                App.APPREMARK = "";
                App.DEPARTMENT = DepartHead;
            }
            return App;
        }

        public object GetTDSTypeList()
        {
            object Data;
            Data = _DB.NSP_TDS_TAX_TYPE_MASTER.Where(x => x.TAX_TYPE != "ES" && x.ACTIVE == 1).Select(x =>
                new
                {
                    text = x.TAX_TYPE,
                    value = x.WITHHOLDING_TAX_NAME,
                    Rate = x.RATE
                }).ToList();

            return Data;
        }

        public string GetESTax()
        {
            try
            {

                var rate = _DB.NSP_TDS_TAX_TYPE_MASTER
                    .Where(x => x.TAX_TYPE == "ES" && x.ACTIVE == 1).Select(x => x.RATE).FirstOrDefault().ToString();

                return rate ?? "0";
            }
            catch (Exception ex)
            {
                return "0";
            }
        }





        public object GetGSTRateList(string Type, string TaxType, string COSTCENTER, int Status)
        {
            var ProfitCenter = _DB.FINTBS_COST_CENTER_MST.Where(x => x.COSTCENTER == COSTCENTER).Select(x => x.PROFITCENTER).FirstOrDefault();
            object Data;
            Data = _DB.NSP_GST_TAX_MASTER.Where(x => x.TYPE == Type && x.TAXTYPE == TaxType & x.PROFITCENTER == ProfitCenter && (Status == 1 ? x.ACTIVE == 1 : true)).Select(x =>
              new
              {
                  TaxCode = x.TAXCODE,
                  Rate1 = x.VATRATE1,
                  Rate2 = x.VATRATE2,
                  CessRate = x.CESS_RATE
              }).ToList();

            return Data;
        }

        public List<Dealer> ConvertDataTableToList(DataTable dataTable)
        {
            var dealerList = new List<Dealer>();

            foreach (DataRow row in dataTable.Rows)
            {
                var dealer = new Dealer
                {
                    DealerCode = row["DEALER_CODE"].ToString(),
                    DealerName = row["DEALER_NAME"].ToString(),
                    DealerBank = row["DEALER_BANK"].ToString(),
                    DealerAccCode = row["DEALER_ACCODE"].ToString(),
                    Address = row["ADDRESS"].ToString(),
                    PANNO = row["PANNO"].ToString()
                };

                dealerList.Add(dealer);
            }

            return dealerList;
        }




        #endregion

        #region Report
        public NSPReportData GetNSPReportDetail(int HeaderID)
        {
            NSPReportData MainData = new NSPReportData();

            NSPReportData_Initiate_DT Initiate_DT = new NSPReportData_Initiate_DT();
            List<NSPReportData_Invoice_DT> Invoice_DT = new List<NSPReportData_Invoice_DT>();
            List<NSPReportData_Approval_DT> Approval_DT = new List<NSPReportData_Approval_DT>();
            decimal TDSAmount = 0;
            decimal HDTotalAmount = 0;

            try
            {
                NSP_PaymentHeaderViewModel HD = (from data in _DB.NSP_PAYMENT_HEADER
                                                 where data.HEADERID == HeaderID
                                                 orderby data.REQUESTREFNO ascending
                                                 select new NSP_PaymentHeaderViewModel
                                                 {
                                                     HeaderID = data.HEADERID,
                                                     RequestRefNo = data.REQUESTREFNO,
                                                     RequestDate = data.REQUESTDATE,
                                                     Category = data.CATEGORY,
                                                     Location = data.LOCATION,
                                                     STATUS = data.STATUS,
                                                     VendorCode = data.VENDORCODE,
                                                     VendorName = data.VENDORNAME,
                                                     VendorPANNo = data.VENDORPANNO,
                                                     VendorAddress = data.VENDORADDRESS,
                                                     BankName = data.BANKNAME,
                                                     BankAccountNo = data.BANKACCOUNTNO,
                                                     BankIFSCCode = data.BANKIFSCCODE,
                                                     CostCentre = data.COSTCENTRE,
                                                     Budgeted = data.BUDGETED,
                                                     PurposeofExpenses = data.PURPOSEOFEXPENSES,
                                                     CurrencyType = data.CURRENCYTYPE,
                                                     RequestType = data.REQUESTTYPE,
                                                     FY_RequestType = data.REQUESTTYPE,
                                                     TotalNetAmount = data.TOTALNETAMOUNT,
                                                     TotalNetAmountInWord = data.TOTALNETAMOUNTINWORD,
                                                     FINAL_TOTAL = data.FINAL_TOTAL,
                                                     APPROVED_FINAL_TOTAL = data.APPROVED_FINAL_TOTAL,
                                                     FY_ExRate = data.FY_EXRATE,
                                                     FY_ExRateDate = data.FY_EXRATEDATE,
                                                     FY_Taxation = data.FY_TAXATION,
                                                     FY_GSTType = data.FY_GSTTYPE,
                                                     FY_GST = data.FY_GST,
                                                     FY_SGSTRATE = data.FY_SGSTRATE,
                                                     SGSTAMOUNT = data.SGSTAMOUNT,
                                                     FY_CGSTRATE = data.FY_CGSTRATE,
                                                     CGSTAMOUNT = data.CGSTAMOUNT,
                                                     FY_IGSTRATE = data.FY_IGSTRATE,
                                                     IGSTAMOUNT = data.IGSTAMOUNT,
                                                     FY_GSTREASON = data.FY_GSTREASON,
                                                     TOTAL_GST_AMOUNT = data.TOTAL_GST_AMOUNT,
                                                     FY_TDS = data.FY_TDS,
                                                     FY_TDSRate = data.FY_TDSRATE,
                                                     FY_TDS_AMOUNT = data.FY_TDS_AMOUNT,
                                                     FY_TDSREASON = data.FY_TDSREASON,
                                                     FinanceRemark = data.FINANCEREMARK,
                                                     TAXATIONREMARK = data.TAXATIONREMARK,
                                                     LOCATIONID = data.LOCATIONID,
                                                     PAY_PRO_LOCATION = data.PAY_PRO_LOCATION,
                                                     PAY_PRO_LOCATIONID = data.PAY_PRO_LOCATIONID,
                                                     BUDGETED_ATTACH = data.BUDGETED_ATTACH,
                                                     POSTING_DATE = data.POSTING_DATE,
                                                     ADDEDBY = data.ADDEDBY ?? data.UPDATEDBY ?? 0,
                                                     ADDEDON = data.ADDEDON ?? DateTime.MinValue

                                                 }).FirstOrDefault();


                List<NSP_PaymentDetailsViewModel> Dt = (from paymentDetail in _DB.NSP_PAYMENT_DETAIL.Where(e => e.HEADERID == HeaderID)
                                                        select new NSP_PaymentDetailsViewModel
                                                        {
                                                            HeaderID = paymentDetail.HEADERID,
                                                            DetailID = paymentDetail.DETAILID,
                                                            InvoiceNo = paymentDetail.INVOICENO,
                                                            InvoiceDate = paymentDetail.INVOICEDATE,
                                                            Description = paymentDetail.DESCRIPTION,
                                                            Currency = paymentDetail.CURRENCY,
                                                            Amount = paymentDetail.AMOUNT,
                                                            PO_Document = paymentDetail.PO_DOCUMENT,
                                                            Invoice_Document = paymentDetail.INVOICE_DOCUMENT,
                                                            ApprovalNote_Document = paymentDetail.APPROVALNOTE_DOCUMENT,
                                                            Other_Document = paymentDetail.OTHER_DOCUMENT,
                                                            APPROVED_AMOUNT = paymentDetail.APPROVED_AMOUNT,
                                                            TDSAmount = paymentDetail.TDS_AMOUNT,
                                                            TotalNetAmount = paymentDetail.TOTAL_NET_AMOUNT
                                                        }).ToList();


                foreach (var item in Dt)
                {
                    NSPReportData_Invoice_DT itemDt = new NSPReportData_Invoice_DT();
                    var TotalAmount = (decimal.Parse(item.TotalNetAmount.ToString()) + decimal.Parse(item.TDSAmount.ToString()));
                    itemDt.InvoiceNo = item.InvoiceNo;
                    itemDt.InvoiceDate = item.InvoiceDate.ToString();
                    itemDt.Description = (item.Description == null ? "" : item.Description);
                    itemDt.Currency = item.Currency;
                    itemDt.Amount = TotalAmount.ToString();
                    itemDt.Exrate = "1"; ;
                    itemDt.AmountinINR = TotalAmount.ToString();


                    TDSAmount = TDSAmount + (item.TDSAmount == null ? 0 : decimal.Parse(item.TDSAmount.ToString()));
                    HDTotalAmount = HDTotalAmount + TotalAmount;
                    Invoice_DT.Add(itemDt);
                }


                MainData.FY_RequestTranType = "NEW";
                MainData.RequestRefNo = HD.HeaderID;
                MainData.RequestDate = HD.ADDEDON.ToString();
                MainData.Category = HD.Category;
                MainData.VendorCode = HD.VendorCode;
                MainData.VendorName = HD.VendorName;
                MainData.VendorPANNo = HD.VendorPANNo;
                MainData.VendorAddress = HD.VendorAddress;
                MainData.CostCentre = HD.CostCentre;
                MainData.Budgeted = HD.Budgeted.ToUpper() == "YES" ? "Budgeted" : "Non-Budgeted";
                MainData.Location = HD.Location;
                MainData.PurposeofExpenses = HD.PurposeofExpenses;
                MainData.SBIForexRateDate = "";
                MainData.TotalAmount = HDTotalAmount.ToString();
                MainData.AdvancePaid_Adjustments = 0;
                MainData.NetPayableAmount = (HDTotalAmount - HD.FY_TDS_AMOUNT).ToString();
                MainData.Invoice_DT = Invoice_DT;
                MainData.TDSDeduction = TDSAmount;

                List<NSP_ApprovalAuthority> Auth = (from approval in _DB.NSP_APPROVAL_AUTHORITY.
                                                      Where(d => d.HEADERID == HeaderID && d.DEPARTMENT == "APPROVAL")
                                                    select new NSP_ApprovalAuthority
                                                    {
                                                        HEADERID = approval.HEADERID,
                                                        APPROVALTYPE = approval.APPROVALTYPE,
                                                        APPSEQ = approval.APPSEQ,
                                                        EMPCODE = approval.EMPCODE,
                                                        EMP_NAME = approval.EMP_NAME,
                                                        EMPHEAD = approval.EMPHEAD,
                                                        DESIGNATION = approval.DESIGNATION,
                                                        APPROVAL_STATUS = approval.APPROVAL_STATUS,
                                                        APPREMARK = approval.APPREMARK,
                                                        UPDATEDON = approval.UPDATEDON
                                                    }).OrderBy(x => x.APPSEQ).ToList();



                foreach (var item in Auth)
                {
                    NSPReportData_Approval_DT itemDt = new NSPReportData_Approval_DT();
                    itemDt.Designation = item.DESIGNATION;
                    itemDt.App_Title = item.APPROVALTYPE == "1" ? item.DESIGNATION : item.EMPHEAD.ToString();
                    itemDt.App_Name = item.EMPCODE.ToString() + "-" + item.EMP_NAME;
                    itemDt.App_Date = item.UPDATEDON != null ? item.UPDATEDON.ToString() : "";
                    Approval_DT.Add(itemDt);

                }

                MainData.Approval_DT = Approval_DT;

                long RequeCode = long.Parse(HD.ADDEDBY.ToString());

                var obj = (from i in _DB.VW_ASSOCIATELVLDETAILS_LC
                           join r in _DB.ADEMPLOYEE_LC on i.ADEMPCODE equals r.ADEMPCODE
                           join d in _DB.ADDESIGNATION_LC on i.ADDESIGNATIONID equals d.ADDESIGNATIONID
                           join s in _DB.SYSITE_LC on i.SYSITEID equals s.SYSITEID
                           where i.ADEMPCODE == RequeCode
                           && i.ACTIVE == 1 && r.ACTIVE == 1 && d.ACTIVE == 1 && s.ACTIVE == 1
                           && i.SYKI == _Syki

                           select new NSPReportData_Initiate_DT
                           {
                               IntiatorName = r.FIRSTNAME + "" + r.LASTNAME,
                               Designation = d.DESCRIP,
                               Department = i.DEPARTMENT,
                               Division = i.DIVISION,
                               Operation = i.OPERATION,
                               Dt_Location = s.DESCRIP
                           }).FirstOrDefault();


                Initiate_DT.IntiatorName = obj.IntiatorName;
                Initiate_DT.Designation = obj.Designation;
                Initiate_DT.Department = obj.Department;
                Initiate_DT.Division = obj.Division;
                Initiate_DT.Operation = obj.Operation;
                Initiate_DT.Dt_Location = obj.Dt_Location;

                MainData.Initiate_DT = Initiate_DT;
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return MainData;
        }
        #endregion

        #region mail
        public void SendMailToFromTaxation(ADEMPLOYEE_LC REQUESTER, ADEMPLOYEE_LC Finance, ADEMPLOYEE_LC Taxation, string To)
        {

            try
            {


                if (To == "Finance")
                {
                    #region Send mail to Finance from Taxation authority

                    commanEmail NextsendMail = new commanEmail();
                    NextsendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    NextsendMail.MailTo = Finance.EMAILID.ToString(); // Uncomment when Upload in VSS

                    //NextsendMail.MailTo = "Suresh.Jangid@honda.hmsi.in"; // comment when Upload in VSS

                    string strSubject_ = "Non SAP Payment Request from - " + REQUESTER.FIRSTNAME.ToString() + ", Employee Code - " + REQUESTER.ADEMPCODE.ToString();
                    string strBody_ = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Non SAP Payment Request from " + REQUESTER.FIRSTNAME.ToString() + " - Emp Code (" + REQUESTER.ADEMPCODE.ToString() + ")</font></b></td></tr>" +
                                      "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + Finance.FIRSTNAME.ToString() + " ,</br></br></td></tr><tr><td colspan=2 width=514 valign=top>" + Taxation.FIRSTNAME.ToString() + " San Approved Request of " + REQUESTER.FIRSTNAME + "San For Non SAP Payment Approval Request (Taxation) In  a  in Employee Portal. Below are the details :</td></tr>" +
                                    "<tr><td width=125 height=22 valign=top>Type :</td><td width=389 valign=top>Taxation</td></tr>" +
                                     "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "Login/EmailApproval/?Wid=" + Finance.ADEMPCODE.ToString() + "&C=NSP&A=NSPApproval&Tid=" + 0 + "> Employee Portal</a> link to approve the request.</td></tr>" +
                                     "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                    NextsendMail.MailSubject = strSubject_;
                    NextsendMail.MailBody = strBody_;
                    try
                    {
                        bool status = NextsendMail.Send();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }




                    #endregion
                }
                else
                {
                    #region Send mail to Taxation from Finance authority

                    commanEmail NextsendMail = new commanEmail();
                    NextsendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    NextsendMail.MailTo = Taxation.EMAILID.ToString(); // Uncomment when Upload in VSS

                    //NextsendMail.MailTo = "Suresh.Jangid@honda.hmsi.in"; // comment when Upload in VSS

                    string strSubject_ = "Non SAP Payment Request from - " + REQUESTER.FIRSTNAME.ToString() + ", Employee Code - " + REQUESTER.ADEMPCODE.ToString();
                    string strBody_ = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Non SAP Payment Request from " + REQUESTER.FIRSTNAME.ToString() + " - Emp Code (" + REQUESTER.ADEMPCODE.ToString() + ")</font></b></td></tr>" +
                                      "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + Taxation.FIRSTNAME.ToString() + " ,</br></br></td></tr><tr><td colspan=2 width=514 valign=top>" + Finance.FIRSTNAME.ToString() + " San Sent You Request of " + REQUESTER.FIRSTNAME + " San For Non SAP Payment Approval Request In  a  in Employee Portal. Below are the details :</td></tr>" +
                                    "<tr><td width=125 height=22 valign=top>Type :</td><td width=389 valign=top>Finance</td></tr>" +
                                     "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "Login/EmailApproval/?Wid=" + Taxation.ADEMPCODE.ToString() + "&C=NSP&A=NSPApproval&Tid=" + 0 + "> Employee Portal</a> link to approve the request.</td></tr>" +
                                     "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                    NextsendMail.MailSubject = strSubject_;
                    NextsendMail.MailBody = strBody_;
                    try
                    {
                        bool status = NextsendMail.Send();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }




                    #endregion
                }




            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void SendMailToNextAuthority(ADEMPLOYEE_LC REQUESTER, ADEMPLOYEE_LC NEXTAPPRVAL, int REQTYPE)
        {

            try
            {
                string strReqType = REQTYPE == 1 ? "Approval" : REQTYPE == 2 ? "Finance" : REQTYPE == 3 ? "Taxation" : "";

                #region Send mail next approval authority

                commanEmail NextsendMail = new commanEmail();
                NextsendMail.MailFrom = "portal.admin@honda.hmsi.in";
                NextsendMail.MailTo = NEXTAPPRVAL.EMAILID.ToString(); // Uncomment when Upload in VSS

                //NextsendMail.MailTo = "Suresh.Jangid@honda.hmsi.in"; // Uncomment when Upload in VSS

                string strSubject_ = "Non SAP Payment Request from - " + REQUESTER.FIRSTNAME.ToString() + ", Employee Code - " + REQUESTER.ADEMPCODE.ToString();
                string strBody_ = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                 "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Non SAP Payment Request from " + REQUESTER.FIRSTNAME.ToString() + " - Emp Code (" + REQUESTER.ADEMPCODE.ToString() + ")</font></b></td></tr>" +
                                  "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + NEXTAPPRVAL.FIRSTNAME.ToString() + " ,</br></br></td></tr><tr><td colspan=2 width=514 valign=top>" + REQUESTER.FIRSTNAME.ToString() + " San has raised a Non SAP Payment Approval Request in Employee Portal. Below are the details :</td></tr>" +
                                "<tr><td width=125 height=22 valign=top>Type :</td><td width=389 valign=top>" + strReqType + "</td></tr>" +
                                 "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "Login/EmailApproval/?Wid=" + NEXTAPPRVAL.ADEMPCODE.ToString() + "&C=NSP&A=NSPApproval&Tid=" + 0 + "> Employee Portal</a> link to approve the request.</td></tr>" +
                                 "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                NextsendMail.MailSubject = strSubject_;
                NextsendMail.MailBody = strBody_;
                try
                {
                    bool status = NextsendMail.Send();
                }
                catch (Exception ex)
                {
                    throw ex;
                }




                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void SendMailToRequester(ADEMPLOYEE_LC REQUESTER, ADEMPLOYEE_LC Curr_Approval, int approvalStatus, int REQTYPE, string Remarks)
        {
            try
            {
                string RequestStatus = approvalStatus == 2 ? "Approved" : approvalStatus == 3 ? "Send back" : approvalStatus == 4 ? "Rejected" : approvalStatus == 5 ? "Sent to Taxation" : approvalStatus == 6 ? "Hold" : "";
                string strReqType = REQTYPE == 1 ? "Approval" : REQTYPE == 2 ? "Finance" : REQTYPE == 3 ? "Taxation" : "";

                #region Send mail for requestor
                commanEmail sendMail = new commanEmail();
                sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                sendMail.MailTo = REQUESTER.EMAILID.ToString(); // Uncomment when Upload in VSS

                //sendMail.MailTo = "Suresh.Jangid@honda.hmsi.in"; // comment when Upload in VSS

                string strSubject = "Non SAP Payment Approval Status - " + RequestStatus;
                string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                 "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Non SAP Payment Approval Status " + RequestStatus + " - Emp Code (" + REQUESTER.ADEMPCODE.ToString() + ")</font></b></td></tr>" +
                                  "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                                 "<tr><td valign=top colspan =2>Your Non SAP Payment request has been <b>" + RequestStatus + "</b> by " + Curr_Approval.FIRSTNAME + " San. The request details are as follows:</td></tr>" +
                                "<tr><td width=125 height=22 valign=top>Type :</td><td width=389 valign=top>" + strReqType + "</td></tr>" +
                                 "<tr><td width=125 height=22 valign=top>Remarks :</td><td width=389 valign=top>" + Remarks + "</td></tr>" +
                                 "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "Login/EmailApproval?Wid=" + REQUESTER.ADEMPCODE.ToString() + "&C=NSP&A=NSPApproval&Tid=" + 0 + "> Employee Portal</a> to view the approval history.</td></tr>" +
                                 "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

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


                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion


        #region  POST and Reverse POst

        public LibResult GetDataForPostInvoices(string Status, int LoginCode, string Mode, string FromDate, string ToDate)
        {
            LibResult result = new LibResult();

            var RoleSiteIDs = (from r in _DB.NSP_ROLE_MASTER where r.ACTIVE == 1 && r.ECODE == LoginCode && r.DEPARTMENT == "Finance" select r.SYSITEID).ToList(); // ADDED BY AUMENTO :: SR88023-CR5305 :: 2.0
            var FinanceUser = (from r in _DB.NSP_ROLE_MASTER where r.ACTIVE == 1 && RoleSiteIDs.Contains(r.SYSITEID) && r.DEPARTMENT == "Finance" select r.ECODE).ToList(); // ADDED BY AUMENTO :: SR88023-CR5305 :: 2.0

            try
            {
                DateTime _fDate = DateTime.Parse(FromDate);
                DateTime _tDate = DateTime.Parse(ToDate);
                //using (var db = new LCEntities())
                //{
                    if (Mode == "POST")
                    {
                        var data = (from hd in _DB.NSP_PAYMENT_HEADER
                                    join dt in _DB.NSP_PAYMENT_DETAIL on hd.HEADERID equals dt.HEADERID
                                    join app in _DB.NSP_APPROVAL_AUTHORITY on hd.HEADERID equals app.HEADERID
                                    where ///app.EMPCODE == LoginCode // COMMENTED BY AUMENTO :: SR88023-CR5305 2.O

                                     hd.PAY_PRO_LOCATIONID != null && RoleSiteIDs.Contains(hd.PAY_PRO_LOCATIONID.Value)  // ADDED BY AUMENTO :: SR88023-CR5305 2.O
                                    && FinanceUser.Contains(app.EMPCODE) // ADDED BY AUMENTO :: SR88023-CR5305 2.O

                                         && app.APPROVAL_STATUS == 2
                                          && hd.STATUS == 4
                                          && app.DEPARTMENT == "Finance"
                                          && (dt.ISPOSTED == Status || dt.ISPOSTED == null)
                                                  //&& EntityFunctions.TruncateTime(hd.ADDEDON) >= _fDate.Date  // UPDATED BY AUMENTO :: SR88023-CR5305 2.O
                                                  //&& EntityFunctions.TruncateTime(hd.ADDEDON) <= _tDate.Date  // UPDATED BY AUMENTO :: SR88023-CR5305 2.O
                                                  && hd.ADDEDON >= _fDate.Date
                                                    && hd.ADDEDON <= _tDate.Date.AddDays(1).AddTicks(-1) // include full day
                                           && hd.CATEGORY != "Others"
                                    select new { hd, dt })
                                    .AsEnumerable()
                                    .GroupBy(g => new
                                    {
                                        g.hd.HEADERID,
                                        g.dt.INVOICENO,
                                        g.hd.VENDORCODE,
                                        g.hd.VENDORNAME,
                                        g.hd.CATEGORY,
                                        g.hd.COSTCENTRE,
                                        g.hd.SAP_STATUS,
                                        g.hd.GSTIN,
                                        g.dt.INVOICEDATE,
                                        g.hd.REQUESTREFNO
                                    }).Select(g => new NSP_PaymentDetailsViewModel
                                    {
                                        SAP_STATUS = g.Key.SAP_STATUS ?? "Pending",
                                        HeaderID = g.Key.HEADERID,
                                        InvoiceNo = g.Key.INVOICENO,
                                        RequestRefNo = g.Key.REQUESTREFNO,
                                        VENDORCODE = g.Key.VENDORCODE,
                                        VENDORNAME = g.Key.VENDORNAME,
                                        CATEGORY = g.Key.CATEGORY,
                                        InvoiceDate = g.Max(x => x.dt.INVOICEDATE), // Adjust as needed
                                        Amount = g.Sum(x => x.dt.AMOUNT),
                                        APPROVED_AMOUNT = g.Sum(x => x.dt.APPROVED_AMOUNT),
                                        TotalNetAmount = g.Sum(x => x.dt.TOTAL_NET_AMOUNT),
                                        BaseAmount = g.Max(x => x.dt.BASE_AMOUNT), // Adjust as needed
                                        ISPOSTED = g.Max(x => x.dt.ISPOSTED) ?? "N",
                                        ISREVERSEPOSTED = g.Max(x => x.dt.ISREVERSEPOSTED) ?? "N",
                                        POSTDOCNO = g.Max(x => x.dt.POSTDOCNO), // Adjust as needed
                                        POSTDOCDATE = g.Max(x => x.dt.POSTDOCDATE), // Adjust as needed
                                        REVERSEPOSTDOCNO = g.Max(x => x.dt.REVERSEPOSTDOCNO), // Adjust as needed
                                        REVERSEPOSTDOCDATE = g.Max(x => x.dt.REVERSEPOSTDOCDATE), // Adjust as needed
                                        POSTREVERSEMESSAGE = g.Max(x => x.dt.POSTREVERSEMESSAGE), // Adjust as needed
                                        CostCenter = g.Key.COSTCENTRE,
                                        PostingDtFound = false,
                                        GSTIN = g.Key.GSTIN
                                    }).Distinct().OrderBy(x => x.HeaderID).ToList();


                        result.resultObject = data;
                        result.hasError = false;


                    }
                    else
                    {

                        var data = (from hd in _DB.NSP_PAYMENT_HEADER
                                    join dt in _DB.NSP_PAYMENT_DETAIL on hd.HEADERID equals dt.HEADERID
                                    join app in _DB.NSP_APPROVAL_AUTHORITY on hd.HEADERID equals app.HEADERID
                                    where ///app.EMPCODE == LoginCode // COMMENTED BY AUMENTO :: SR88023-CR5305 2.O

                                     hd.PAY_PRO_LOCATIONID != null && RoleSiteIDs.Contains(hd.PAY_PRO_LOCATIONID.Value)  // ADDED BY AUMENTO :: SR88023-CR5305 2.O
                                    && FinanceUser.Contains(app.EMPCODE) // ADDED BY AUMENTO :: SR88023-CR5305 2.O
                                          && app.APPROVAL_STATUS == 2
                                          && hd.STATUS == 4
                                          && app.DEPARTMENT == "Finance"
                                          && dt.ISPOSTED == "Y" && dt.ISREVERSEPOSTED == Status
                                                  //&& EntityFunctions.TruncateTime(hd.ADDEDON) >= _fDate.Date  // UPDATED BY AUMENTO :: SR88023-CR5305 2.O
                                                  //&& EntityFunctions.TruncateTime(hd.ADDEDON) <= _tDate.Date // UPDATED BY AUMENTO :: SR88023-CR5305 2.O
                                          && hd.ADDEDON >= _fDate.Date
                                          && hd.ADDEDON <= _tDate.Date.AddDays(1).AddTicks(-1) // include full day
                                          && hd.CATEGORY != "Others"
                                    select new { hd, dt })
                                    .AsEnumerable()
                                    .GroupBy(g => new
                                        {
                                        g.hd.HEADERID,
                                        g.dt.INVOICENO,
                                        g.hd.VENDORCODE,
                                        g.hd.VENDORNAME,
                                        g.hd.CATEGORY,
                                        g.hd.COSTCENTRE,
                                        g.hd.SAP_STATUS,
                                        g.hd.GSTIN,
                                        g.dt.INVOICEDATE,
                                        g.hd.REQUESTREFNO
                                    }).Select(g => new NSP_PaymentDetailsViewModel
                                    {
                                        SAP_STATUS = g.Key.SAP_STATUS ?? "Pending",
                                        HeaderID = g.Key.HEADERID,
                                        InvoiceNo = g.Key.INVOICENO,
                                        RequestRefNo = g.Key.REQUESTREFNO,
                                        VENDORCODE = g.Key.VENDORCODE,
                                        VENDORNAME = g.Key.VENDORNAME,
                                        CATEGORY = g.Key.CATEGORY,
                                        InvoiceDate = g.Max(x => x.dt.INVOICEDATE), // Adjust as needed
                                        Amount = g.Sum(x => x.dt.AMOUNT),
                                        APPROVED_AMOUNT = g.Sum(x => x.dt.APPROVED_AMOUNT),
                                        TotalNetAmount = g.Sum(x => x.dt.TOTAL_NET_AMOUNT),
                                        BaseAmount = g.Max(x => x.dt.BASE_AMOUNT), // Adjust as needed
                                        ISPOSTED = g.Max(x => x.dt.ISPOSTED) ?? "N",
                                        ISREVERSEPOSTED = g.Max(x => x.dt.ISREVERSEPOSTED) ?? "N",
                                        POSTDOCNO = g.Max(x => x.dt.POSTDOCNO), // Adjust as needed
                                        POSTDOCDATE = g.Max(x => x.dt.POSTDOCDATE), // Adjust as needed
                                        REVERSEPOSTDOCNO = g.Max(x => x.dt.REVERSEPOSTDOCNO), // Adjust as needed
                                        REVERSEPOSTDOCDATE = g.Max(x => x.dt.REVERSEPOSTDOCDATE), // Adjust as needed
                                        POSTREVERSEMESSAGE = g.Max(x => x.dt.POSTREVERSEMESSAGE), // Adjust as needed
                                        CostCenter = g.Key.COSTCENTRE,
                                        PostingDtFound = false,
                                        GSTIN = g.Key.GSTIN
                                    }).Distinct().OrderBy(x => x.HeaderID).ToList();

                        result.resultObject = data;
                        result.hasError = false;
                    }
                //}

            }
            catch (Exception ex)
            {
                result.hasError = true;
                result.errorMessage = ex.Message.ToString();
            }

            return result;
        }

        public LibResult GetPostReverseRights(int LoginCode)
        {
            LibResult res = new LibResult();
            try
            {

                //var data = _DB.NSP_ROLE_MASTER.Where(x => x.ECODE == LoginCode && x.DEPARTMENT == "Finance").Select(x => new {
                //           Post = x.POST == null ? "N" : x.POST,Reverse = x.REVERSE == null ? "N" : x.REVERSE}).FirstOrDefault();
                var data = _DB.NSP_ROLE_MASTER.Where(x => x.ECODE == LoginCode && x.ACTIVE == 1 && x.DEPARTMENT == "Finance").FirstOrDefault();


                res.resultObject = data;
                res.hasError = false;
            }
            catch (Exception ex)
            {

                res.resultObject = null;
                res.hasError = true;
                res.errorMessage = ex.ToString();
            }

            return res;

        }

        public async Task<LibResult> PostAndReversePostInSap(POSTReverseRequest request, int LoginCode)
        {
            int HEADERID = request.RequestNo;
            string InvoiceNo = request.InvoiceNo;
            int detailID = request.DtId;
            string Mode = request.Mode;
            List<NSP_PaymentDetails> DataList = request.PostingDt;

            LibResult res = new LibResult();
            //EportalESS es = new EportalESS();

            try
            {
                // START: ADDED BY AUMENTO: SR88023-CR5305: CHANGE 2.0 : AS DISCUSSED IN MEETING AS ON 25-DEC-2024
                using (var transaction = _DB.Database.BeginTransaction())
                {


                    var TDSGlList = _DB.NSP_TDS_TAX_TYPE_MASTER.Select(x => x.GLACC).ToList();
                    var StrTDSGlList = TDSGlList.Select(item => item?.ToString() ?? string.Empty).ToList();

                    //NSP_PaymentDetails TDSRecord = request.PostingDt.Where(x => !(x.ItemText.StartsWith("TDS") || x.ItemText.Contains("TDS")) || !StrTDSGlList.Contains(x.GLAccount.ToString())).ToList().FirstOrDefault();

                    //request.PostingDt = request.PostingDt.Where(x => !(x.ItemText.StartsWith("TDS") || x.ItemText.Contains("TDS")) || !StrTDSGlList.Contains(x.GLAccount.ToString())).ToList();

                    if (Mode == "POST")
                    {
                        // Remove TDS lines
                        request.PostingDt = request.PostingDt
                        .Where(x => !(x.ItemText.StartsWith("TDS") || x.ItemText.Contains("TDS")) ||
                                    !StrTDSGlList.Contains(x.GLAccount?.ToString()))
                        .ToList();

                        // Calculate the sum of debit entries excluding "Invoice Amount"
                        decimal invoiceAmount = request.PostingDt
                            .Where(x => (x.ZType.StartsWith("DEBIT") || x.ItemText.Contains("DEBIT")) &&
                                        !x.ItemText.Contains("Invoice Amount"))
                            .Sum(x => x.AmountDocumentCurrency);

                        // Update "Invoice Amount" entries if the calculated amount is non-zero
                        if (invoiceAmount != 0)
                        {
                            foreach (var item in request.PostingDt.Where(x =>
                                x.ItemText.Contains("Invoice Amount") &&
                                x.TDS_BASE_AMOUNT != 0 &&
                                x.ZType == "CREDIT" &&
                                !string.IsNullOrEmpty(x.LIFNR)))
                            {
                                item.AmountDocumentCurrency = invoiceAmount;
                            }
                        }
                    }


                    // END: ADDED BY AUMENTO: SR88023-CR5305: CHANGE 2.0 : AS DISCUSSED IN MEETING AS ON 25-DEC-2024

                    var HD = _DB.NSP_PAYMENT_HEADER.FirstOrDefault(i => i.HEADERID == HEADERID);
                    var DTList = _DB.NSP_PAYMENT_DETAIL.Where(i => i.HEADERID == HEADERID && i.INVOICENO == InvoiceNo).ToList();

                    string SuccessMsg = string.Empty;
                    if (Mode == "POST")
                    {
                        SuccessMsg = await RFC.PostInvoiceInSAP(request.PostingDt, DTList[0].INVOICEDATE);  // UPDATED BY AUMENTO: SR88023-CR5305: CHANGE 2.0 : AS DISCUSSED IN MEETING AS ON 25-DEC-2024
                    }
                    else
                    {
                        SuccessMsg = await RFC.ReversePostedInvoiceInSAP(request.DocNo, request.FiscalYear, request.Reason, request.PostingDate);
                    }
                    string[] Status = SuccessMsg.Split(new Char[] { '#' });
                    string errResult = Convert.ToString(Status[0]);
                    string DocMsg = Convert.ToString(Status[1]);
                    string DocumnetNo = Mode == "POST" ? RemoveParts(DocMsg) : DocMsg;
                    if (errResult == "S" && DocumnetNo != "ERR")
                    {
                        res.hasError = false;
                        res.errorMessage = DocumnetNo;

                    }
                    else
                    {
                        res.hasError = true;
                        res.errorMessage = DocMsg;

                    }

                    foreach (var DT in DTList)
                    {


                        if (DT != null && res.hasError == false)
                        {
                            if (Mode == "POST")
                            {
                                DT.ISPOSTED = "Y";
                                DT.POSTDOCDATE = DateTime.Now;
                                DT.POSTDOCNO = DocumnetNo;
                                DT.ISREVERSEPOSTED = "N";


                            }
                            else
                            {
                                DT.ISREVERSEPOSTED = "Y";
                                DT.REVERSEPOSTDOCDATE = DateTime.Now;
                                DT.REVERSEPOSTDOCNO = DocumnetNo;
                                DT.ISPOSTED = "N";

                            }



                            DT.UPDATEDBY = LoginCode;
                            DT.UPDATEDON = DateTime.Now;

                            _DB.Entry(DT).State = EntityState.Modified;


                            res.hasError = false;
                            res.errorMessage = (Mode == "POST" ? "Invoice Posted With Doc No " + DT.POSTDOCNO.ToString() : "Invoice Reverse Posted With Doc No " + DT.REVERSEPOSTDOCNO.ToString());
                        }
                        else
                        {
                            res.hasError = true;

                        }
                    }

                    _DB.SaveChanges();
                    // START: ADDED BY AUMENTO: SR88023-CR5305: CHANGE 2.0 : AS DISCUSSED IN MEETING AS ON 25-DEC-2024
                    //InsertToNSP_PostingDtPaymentDetails(HEADERID, InvoiceNo, detailID, DataList, LoginCode, Mode, DocumnetNo, DTList[0].INVOICEDATE);
                    if (errResult == "S" && DocumnetNo != "ERR")
                    {
                        InsertToNSP_PostingDtPaymentDetails(HEADERID, InvoiceNo, detailID, request.PostingDt, LoginCode, Mode, DocumnetNo, DTList[0].INVOICEDATE);
                    }

                    transaction.Commit();
                }
                // END: ADDED BY AUMENTO: SR88023-CR5305: CHANGE 2.0 : AS DISCUSSED IN MEETING AS ON 25-DEC-2024
            }
            catch (Exception ex)
            {
                res.hasError = true;
                res.errorMessage = ex.ToString();
            }

            return res;
        }

        private string RemoveParts(string inputString)
        {

            string pattern = @"Document no\. (\d+) 50AG";

            Match match = Regex.Match(inputString, pattern);
            if (match.Success)
            {
                string middlePart = match.Groups[1].Value;
                string outputString = inputString.Replace(match.Value, middlePart);
                return outputString;
            }
            else
            {
                return "ERR";
            }
        }

        public List<NSP_PaymentDetails> GetPostInvoiceData(int HEADERID, string InvoiceNo, int detailID, string DocType, int LoginCode, string CostCenter, string BusinessCenter, string Status)
        {
            var DataList = new List<NSP_PaymentDetails>();


            if (DocType == "KG")
            {
                DataList = GetENTRYForKG(HEADERID, InvoiceNo, detailID, DocType, LoginCode, CostCenter, BusinessCenter);
            }
            else if (DocType == "KR")
            {
                DataList = GetENTRYForKR(HEADERID, InvoiceNo, detailID, DocType, LoginCode, CostCenter, BusinessCenter);
            }
            else
            {
                DataList = GetENTRYForPL(HEADERID, InvoiceNo, detailID, DocType, LoginCode, CostCenter, BusinessCenter);

            }
            if (Status == "Y")
            {
                var DataListReverse = new List<NSP_PaymentDetails>();
                foreach (var item in DataList)
                {
                    item.ZType = (item.ZType == "CREDIT" ? "DEBIT" : "CREDIT");
                    DataListReverse.Add(item);
                }
                return DataListReverse;
            }
            else
            {
                return DataList;
            }

        }

        public List<NSP_PaymentDetails> GetPostInvoiceDataReverse(int HeaderID, string POSTEDDOCNO, string InvoceNo)
        {
            var DataListReverse = new List<NSP_PaymentDetails>();
            try
            {
                var DataList = (from i in _DB.NSP_POSTING_DETAIL
                                where i.HEADERID == HeaderID && i.INVOICENO == InvoceNo && i.POSTDOCNO == POSTEDDOCNO

                                select new NSP_PaymentDetails
                                {
                                    BusinessCenter = i.BUSINESSCENTER,
                                    BusinessPlace = i.BUSINESSPLACE,
                                    CostCenter = i.COSTCENTER,
                                    ProfitCenter = i.PROFITCENTER,
                                    AllocNumber = "",
                                    ItemText = i.ITEMTEXT,
                                    LIFNR = i.LIFNR,
                                    GLAccount = i.GLACCOUNT,
                                    TaxCode = i.TAXCODE,
                                    ZType = i.ZTYPE,
                                    AmountDocumentCurrency = i.AMOUNTDOCUMENTCURRENCY,
                                    WTType = i.WTTYPE,
                                    WTCode = i.WTCODE,
                                    SPGLInd = i.SPGLIND,
                                    Currency = i.CURRENCY,
                                    DocType = i.DOCTYPE,
                                    HeaderText = i.HEADERTEXT,
                                    RefDocNo = i.REFDOCNO,
                                    PostingDocNo = i.POSTDOCNO,
                                    Mode = i.POSTMODE,
                                    SRNO = i.SRNO,
                                    KUNNR = i.KUNNR,
                                    DISCOUNT_AMOUNT = i.DISCOUNT_AMOUNT,
                                    DOCDATE = i.DOCDATE,
                                    TDS_BASE_AMOUNT = i.TDS_BASE_AMOUNT,
                                }).OrderBy(x => x.SRNO).ToList();

                foreach (var item in DataList)
                {
                    item.ZType = (item.ZType == "CREDIT" ? "DEBIT" : "CREDIT");
                    DataListReverse.Add(item);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return DataListReverse;


        }

        public List<NSP_PaymentDetails> GetENTRYForKG(int HEADERID, string InvoiceNo, int detailID, string DocType, int LoginCode, string CostCenter, string BusinessCenter)
        {
            var DataList = new List<NSP_PaymentDetails>();

            var paymentHeader = _DB.NSP_PAYMENT_HEADER.FirstOrDefault(i => i.HEADERID == HEADERID);
            var DTList = _DB.NSP_PAYMENT_DETAIL.Where(i => i.HEADERID == HEADERID && i.INVOICENO == InvoiceNo).ToList();
            var ProfitCenter = _DB.NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER.Where(x => x.BUSINESS_CENTER == BusinessCenter && x.ACTIVE == 1).Select(x => x.PROFIT_CENTER).FirstOrDefault() ?? "";
            var BusinessPlace = _DB.NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER.Where(x => x.BUSINESS_CENTER == BusinessCenter && x.ACTIVE == 1).Select(x => x.BUSINESS_PLACE).FirstOrDefault() ?? "";
            var DtCount = DTList.Count;

            var TotalBaseAmount = DTList.Sum(x => x.BASE_AMOUNT);
            var TotalTdsAmount = DTList.Sum(x => x.TDS_AMOUNT);
            var TotalOfTotalNetAmount = DTList.Sum(x => x.TOTAL_NET_AMOUNT);
            var TotalOfGStAmount = DTList.Sum(x => x.TOTAL_GST_AMOUNT);
            var Cesstotal = DTList.Sum(x => x.CESSAMOUNT) ?? 0;
            TotalBaseAmount = TotalBaseAmount + Cesstotal;

            var TDS_BASE_AMOUNT = DTList.Sum(x => x.TDS_BASE_AMOUNT) ?? 0;
            var DISCOUNT_AMOUNT = DTList.Sum(x => x.AMOUNT) ?? 0;
            DateTime DocDate = DTList.Max(x => x.INVOICEDATE) ?? DateTime.Now;

            var ESI_AMOUNT = DTList.Sum(x => x.ESI_AMOUNT) ?? 0;
            var ESIGL = _DB.NSP_TDS_TAX_TYPE_MASTER.Where(x => x.TAX_TYPE == "ES" && x.ACTIVE == 1).Select(x => x.GLACC).FirstOrDefault().ToString() ?? "";
            bool TOTALBASE = true;
            bool TOTALTDS = true;
            bool ESIENTRY = true;


            foreach (var paymentDetail in DTList)
            {
                if (DtCount > 1)
                {

                    var GSTObj = _DB.NSP_GST_TAX_MASTER.Where(x => x.TAXCODE == paymentDetail.GSTTAXCODE && x.TYPE == paymentDetail.GST_TYPE && x.TAXTYPE == paymentDetail.TAX_TYPE && x.PROFITCENTER == ProfitCenter && x.ACTIVE == 1).FirstOrDefault();
                    var SgstGL = GSTObj != null ? GSTObj.TYPE == "SGST/CGST" ? (GSTObj.GLACC1 != null ? GSTObj.GLACC1.ToString() : "") : "" : "";
                    var CgstGL = GSTObj != null ? GSTObj.TYPE == "SGST/CGST" ? (GSTObj.GLACC1 != null ? GSTObj.GLACC2.ToString() : "") : "" : "";

                    var SGSTPAYABLE = GSTObj != null ? GSTObj.PAYABLE_GLACC1.ToString() ?? "" : "";
                    var CGSTPAYABLE = GSTObj != null ? GSTObj.PAYABLE_GLACC2.ToString() ?? "" : "";
                    decimal SGSTAMOUNT_ = (paymentDetail.SGSTAMOUNT == null ? 0 : paymentDetail.SGSTAMOUNT) ?? 0;
                    decimal CGSTAMOUNT_ = (paymentDetail.CGSTAMOUNT == null ? 0 : paymentDetail.CGSTAMOUNT) ?? 0;
                    var Isdeducted = GSTObj != null ? GSTObj.ISDEDUCTED : "";
                    var IgstGL = GSTObj != null ? GSTObj.TYPE == "IGST" ? (GSTObj.GLACC1 != null ? GSTObj.GLACC1.ToString() : "") : "" : "";
                    decimal IGSTAMOUNT_ = (paymentDetail.IGSTAMOUNT == null ? 0 : paymentDetail.IGSTAMOUNT) ?? 0;
                    var TotalGStAmount = paymentDetail.TOTAL_GST_AMOUNT;
                    var TDSWithHolding = _DB.NSP_TDS_TAX_TYPE_MASTER.Where(x => x.TAX_TYPE == paymentDetail.TDS_TYPE && x.ACTIVE == 1).Select(x => x.WITHHOLDING_TAX_NAME).FirstOrDefault() ?? "";
                    var TDSGL = _DB.NSP_TDS_TAX_TYPE_MASTER.Where(x => x.TAX_TYPE == paymentDetail.TDS_TYPE && x.ACTIVE == 1).Select(x => x.GLACC).FirstOrDefault().ToString() ?? "";

                    decimal BASE_AMOUNT_ = Isdeducted == "N" ? decimal.Parse((TotalBaseAmount ?? 0).ToString()) +
                        decimal.Parse(TotalOfGStAmount.ToString()) : (TotalBaseAmount ?? 0);

                    decimal DtNEtTotal_AMOUNT_ = TotalOfTotalNetAmount ?? 0;

                    var LINFN = "5FF000DUMY";
                    var taxCode = paymentDetail.GSTTAXCODE;
                    string ReqCode = paymentHeader.VENDORCODE;

                    string REqLifn = paymentHeader.CATEGORY != "Dealer" ? ReqCode.PadLeft(10, '0') : ReqCode;

                    var CessGL = GSTObj != null ? (GSTObj.CESS_GLACC != null ? GSTObj.CESS_GLACC.ToString() : "") : "";
                    var CessAmount = (paymentDetail.CESSAMOUNT == null ? 0 : paymentDetail.CESSAMOUNT) ?? 0;

                    var ItemTextForDummy = "Base Amount:" + paymentHeader.PURPOSEOFEXPENSES;
                    var ItemTextForReqInvoice = "Invoice Amount :" + paymentHeader.PURPOSEOFEXPENSES;

                    var DummyGLEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, CostCenter, ProfitCenter, LINFN, ItemTextForDummy, paymentDetail, "", "DEBIT", "", BASE_AMOUNT_, "", "", taxCode);
                    var ReqGLEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, REqLifn, ItemTextForReqInvoice, paymentDetail, TDSWithHolding, "CREDIT", "", DtNEtTotal_AMOUNT_, paymentDetail.TDS_TYPE, "", paymentDetail.GSTTAXCODE, TDS_BASE_AMOUNT, DISCOUNT_AMOUNT);

                    if (TOTALBASE == true)
                    {
                        DataList.Add(DummyGLEntry);
                        DataList.Add(ReqGLEntry);
                        TOTALBASE = false;
                    }

                    if (paymentDetail.TAX_TYPE != "GST-NIL" && paymentDetail.GST_TYPE == "SGST/CGST")
                    {
                        var ItemTextForSGST = "SGST: " + paymentHeader.PURPOSEOFEXPENSES;
                        var ItemTextForCGST = "CGST: " + paymentHeader.PURPOSEOFEXPENSES;
                        var ItemTextForReSGST = "PAYABLE SGST: " + paymentHeader.PURPOSEOFEXPENSES;
                        var ItemTextForReCGST = "PAYABLE CGST: " + paymentHeader.PURPOSEOFEXPENSES;

                        var SGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForSGST, paymentDetail, "", "DEBIT", SgstGL, SGSTAMOUNT_, "", "", "");
                        var ReSGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForReSGST, paymentDetail, "", "CREDIT", SGSTPAYABLE, SGSTAMOUNT_, "", "", "");
                        var CGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForCGST, paymentDetail, "", "DEBIT", CgstGL, CGSTAMOUNT_, "", "", "");
                        var ReCGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForReCGST, paymentDetail, "", "CREDIT", CGSTPAYABLE, CGSTAMOUNT_, "", "", "");


                        if (SGSTAMOUNT_ > 0 && SgstGL != "" && Isdeducted == "Y")
                        {
                            DataList.Add(SGSTEntry);
                        }
                        if (CGSTAMOUNT_ > 0 && CgstGL != "" && Isdeducted == "Y")
                        {
                            DataList.Add(CGSTEntry);
                        }




                        if (paymentDetail.TAX_TYPE == "RCM Charges")
                        {
                            if (SGSTAMOUNT_ > 0 && SGSTPAYABLE != "")
                            {
                                DataList.Add(ReSGSTEntry);
                            }
                            if (CGSTAMOUNT_ > 0 && CGSTPAYABLE != "")
                            {
                                DataList.Add(ReCGSTEntry);
                            }

                        }

                    }

                    if (paymentDetail.TAX_TYPE != "GST-NIL" && paymentDetail.GST_TYPE == "IGST")
                    {
                        var ItemTextForIGST = "IGST: " + paymentHeader.PURPOSEOFEXPENSES;
                        var ItemTextForReIGST = "PAYABLE IGST: " + paymentHeader.PURPOSEOFEXPENSES;
                        var IGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForIGST, paymentDetail, "", "DEBIT", IgstGL, IGSTAMOUNT_, "", "", "");
                        var ReIGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForReIGST, paymentDetail, "", "CREDIT", SGSTPAYABLE, IGSTAMOUNT_, "", "", "");

                        if (IGSTAMOUNT_ > 0 && IgstGL != "" && Isdeducted == "Y")
                        {
                            DataList.Add(IGSTEntry);
                        }


                        if (paymentDetail.TAX_TYPE == "RCM Charges")
                        {
                            if (IGSTAMOUNT_ > 0 && SGSTPAYABLE != "")
                            {
                                DataList.Add(ReIGSTEntry);
                            }
                        }
                    }

                    if (paymentDetail.TDS == "Yes" && paymentDetail.TDS_TYPE != null)
                    {
                        decimal TDS_AMOUNT_ = TotalTdsAmount ?? 0;
                        var ItemTextForTDS = "TDS: " + paymentHeader.PURPOSEOFEXPENSES;
                        var TDsEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForTDS, paymentDetail, "", "CREDIT", TDSGL, TDS_AMOUNT_, "", "", "");

                        if (TDS_AMOUNT_ > 0 && TOTALTDS == true)
                        {
                            DataList.Add(TDsEntry);
                            TOTALTDS = false;
                        }

                    }

                    if (CessGL != "" && CessAmount != 0)
                    {
                        var ItemTextForCess = "Cess Amount: " + paymentHeader.PURPOSEOFEXPENSES;
                        var CessEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForCess, paymentDetail, "", "CREDIT", CessGL, CessAmount, "", "", "");
                        DataList.Add(CessEntry);
                    }


                    if (paymentDetail.IS_ESI == "Yes" && ESIENTRY == true && ESIGL != "" && ESI_AMOUNT != 0)
                    {
                        var ItemTextForTDS = "ES: " + paymentHeader.PURPOSEOFEXPENSES;
                        var ESI_Entry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForTDS, paymentDetail, "", "CREDIT", ESIGL, ESI_AMOUNT, "", "", "");
                        ESIENTRY = false;
                        DataList.Add(ESI_Entry);
                    }





                }
                else
                {
                    var GSTObj = _DB.NSP_GST_TAX_MASTER.Where(x => x.TAXCODE == paymentDetail.GSTTAXCODE && x.TYPE == paymentDetail.GST_TYPE && x.TAXTYPE == paymentDetail.TAX_TYPE && x.PROFITCENTER == ProfitCenter && x.ACTIVE == 1).FirstOrDefault();
                    var SgstGL = GSTObj != null ? GSTObj.TYPE == "SGST/CGST" ? (GSTObj.GLACC1 != null ? GSTObj.GLACC1.ToString() : "") : "" : "";
                    var CgstGL = GSTObj != null ? GSTObj.TYPE == "SGST/CGST" ? (GSTObj.GLACC1 != null ? GSTObj.GLACC2.ToString() : "") : "" : "";
                    var SGSTPAYABLE = GSTObj != null ? GSTObj.PAYABLE_GLACC1.ToString() ?? "" : "";
                    var CGSTPAYABLE = GSTObj != null ? GSTObj.PAYABLE_GLACC2.ToString() ?? "" : "";
                    decimal SGSTAMOUNT_ = (paymentDetail.SGSTAMOUNT == null ? 0 : paymentDetail.SGSTAMOUNT) ?? 0;
                    decimal CGSTAMOUNT_ = (paymentDetail.CGSTAMOUNT == null ? 0 : paymentDetail.CGSTAMOUNT) ?? 0;
                    var Isdeducted = GSTObj != null ? GSTObj.ISDEDUCTED : "";
                    var IgstGL = GSTObj != null ? GSTObj.TYPE == "IGST" ? (GSTObj.GLACC1 != null ? GSTObj.GLACC1.ToString() : "") : "" : "";
                    decimal IGSTAMOUNT_ = (paymentDetail.IGSTAMOUNT == null ? 0 : paymentDetail.IGSTAMOUNT) ?? 0;
                    var TotalGStAmount = paymentDetail.TOTAL_GST_AMOUNT;
                    var TDSWithHolding = _DB.NSP_TDS_TAX_TYPE_MASTER.Where(x => x.TAX_TYPE == paymentDetail.TDS_TYPE && x.ACTIVE == 1).Select(x => x.WITHHOLDING_TAX_NAME).FirstOrDefault() ?? "";
                    var TDSGL = _DB.NSP_TDS_TAX_TYPE_MASTER.Where(x => x.TAX_TYPE == paymentDetail.TDS_TYPE && x.ACTIVE == 1).Select(x => x.GLACC).FirstOrDefault().ToString() ?? "";


                    decimal BASE_AMOUNT_ = Isdeducted == "N" ? decimal.Parse((TotalBaseAmount ?? 0).ToString()) +
                         decimal.Parse(TotalOfGStAmount.ToString()) : (TotalBaseAmount ?? 0);
                    decimal DtNEtTotal_AMOUNT_ = TotalOfTotalNetAmount ?? 0;

                    var LINFN = "5FF000DUMY";
                    var taxCode = paymentDetail.GSTTAXCODE;
                    string ReqCode = paymentHeader.VENDORCODE;

                    string REqLifn = paymentHeader.CATEGORY != "Dealer" ? ReqCode.PadLeft(10, '0') : ReqCode;
                    var CessGL = GSTObj != null ? (GSTObj.CESS_GLACC != null ? GSTObj.CESS_GLACC.ToString() : "") : "";
                    var CessAmount = (paymentDetail.CESSAMOUNT == null ? 0 : paymentDetail.CESSAMOUNT) ?? 0;

                    var ItemTextForDummy = "Base Amount:" + paymentHeader.PURPOSEOFEXPENSES;
                    var ItemTextForReqInvoice = "Invoice Amount :" + paymentHeader.PURPOSEOFEXPENSES;

                    var DummyGLEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, CostCenter, ProfitCenter, LINFN, ItemTextForDummy, paymentDetail, "", "DEBIT", "", BASE_AMOUNT_, "", "", taxCode);
                    var ReqGLEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, REqLifn, ItemTextForReqInvoice, paymentDetail, TDSWithHolding, "CREDIT", "", DtNEtTotal_AMOUNT_, paymentDetail.TDS_TYPE, "", paymentDetail.GSTTAXCODE, TDS_BASE_AMOUNT, DISCOUNT_AMOUNT);
                    DataList.Add(DummyGLEntry);
                    DataList.Add(ReqGLEntry);


                    if (paymentDetail.TAX_TYPE != "GST-NIL" && paymentDetail.GST_TYPE == "SGST/CGST")
                    {

                        var ItemTextForSGST = "SGST: " + paymentHeader.PURPOSEOFEXPENSES;
                        var ItemTextForCGST = "CGST: " + paymentHeader.PURPOSEOFEXPENSES;
                        var ItemTextForReSGST = "PAYABLE SGST: " + paymentHeader.PURPOSEOFEXPENSES;
                        var ItemTextForReCGST = "PAYABLE CGST: " + paymentHeader.PURPOSEOFEXPENSES;

                        var SGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForSGST, paymentDetail, "", "DEBIT", SgstGL, SGSTAMOUNT_, "", "", "");
                        var ReSGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForReSGST, paymentDetail, "", "CREDIT", SGSTPAYABLE, SGSTAMOUNT_, "", "", "");
                        var CGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForCGST, paymentDetail, "", "DEBIT", CgstGL, CGSTAMOUNT_, "", "", "");
                        var ReCGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForReCGST, paymentDetail, "", "CREDIT", CGSTPAYABLE, CGSTAMOUNT_, "", "", "");
                        if (SGSTAMOUNT_ > 0 && SgstGL != "" && Isdeducted == "Y")
                        {
                            DataList.Add(SGSTEntry);
                        }
                        if (CGSTAMOUNT_ > 0 && CgstGL != "" && Isdeducted == "Y")
                        {
                            DataList.Add(CGSTEntry);
                        }

                        if (paymentDetail.TAX_TYPE == "RCM Charges")
                        {
                            if (SGSTAMOUNT_ > 0 && SGSTPAYABLE != "")
                            {
                                DataList.Add(ReSGSTEntry);
                            }
                            if (CGSTAMOUNT_ > 0 && CGSTPAYABLE != "")
                            {
                                DataList.Add(ReCGSTEntry);
                            }

                        }

                    }

                    if (paymentDetail.TAX_TYPE != "GST-NIL" && paymentDetail.GST_TYPE == "IGST")
                    {
                        var ItemTextForIGST = "IGST: " + paymentHeader.PURPOSEOFEXPENSES;
                        var ItemTextForReIGST = "PAYABLE IGST: " + paymentHeader.PURPOSEOFEXPENSES;
                        var IGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForIGST, paymentDetail, "", "DEBIT", IgstGL, IGSTAMOUNT_, "", "", "");
                        var ReIGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForReIGST, paymentDetail, "", "CREDIT", SGSTPAYABLE, IGSTAMOUNT_, "", "", "");

                        if (IGSTAMOUNT_ > 0 && IgstGL != "" && Isdeducted == "Y")
                        {
                            DataList.Add(IGSTEntry);
                        }


                        if (paymentDetail.TAX_TYPE == "RCM Charges")
                        {
                            if (IGSTAMOUNT_ > 0 && SGSTPAYABLE != "")
                            {
                                DataList.Add(ReIGSTEntry);
                            }
                        }
                    }

                    if (paymentDetail.TDS == "Yes" && paymentDetail.TDS_TYPE != null)
                    {
                        var ItemTextForTDS = "TDS: " + paymentHeader.PURPOSEOFEXPENSES;
                        decimal TDS_AMOUNT_ = paymentDetail.TDS_AMOUNT ?? 0;
                        var TDsEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForTDS, paymentDetail, "", "CREDIT", TDSGL, TDS_AMOUNT_, "", "", "");

                        if (TDS_AMOUNT_ > 0)
                        {
                            DataList.Add(TDsEntry);
                        }

                    }



                    if (CessGL != "" && CessAmount != 0)
                    {
                        var ItemTextForCess = "Cess Amount: " + paymentHeader.PURPOSEOFEXPENSES;
                        var CessEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForCess, paymentDetail, "", "CREDIT", CessGL, CessAmount, "", "", "");
                        DataList.Add(CessEntry);
                    }

                    if (paymentDetail.IS_ESI == "Yes" && ESIENTRY == true && ESIGL != "" && ESI_AMOUNT != 0)
                    {
                        var ItemTextForTDS = "ES: " + paymentHeader.PURPOSEOFEXPENSES;
                        var ESI_Entry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForTDS, paymentDetail, "", "CREDIT", ESIGL, ESI_AMOUNT, "", "", "");
                        ESIENTRY = false;
                        DataList.Add(ESI_Entry);
                    }

                }

            }

            return DataList;
        }

        public List<NSP_PaymentDetails> GetENTRYForKR(int HEADERID, string InvoiceNo, int detailID, string DocType, int LoginCode, string CostCenter, string BusinessCenter)
        {
            var DataList = new List<NSP_PaymentDetails>();

            var paymentHeader = _DB.NSP_PAYMENT_HEADER.FirstOrDefault(i => i.HEADERID == HEADERID);
            var DTList = _DB.NSP_PAYMENT_DETAIL.Where(i => i.HEADERID == HEADERID && i.INVOICENO == InvoiceNo).ToList();
            var ProfitCenter = _DB.NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER.Where(x => x.BUSINESS_CENTER == BusinessCenter && x.ACTIVE == 1).Select(x => x.PROFIT_CENTER).FirstOrDefault() ?? "";
            var BusinessPlace = _DB.NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER.Where(x => x.BUSINESS_CENTER == BusinessCenter && x.ACTIVE == 1).Select(x => x.BUSINESS_PLACE).FirstOrDefault() ?? "";
            var DtCount = DTList.Count;

            var TotalBaseAmount = DTList.Sum(x => x.BASE_AMOUNT);
            var TotalTdsAmount = DTList.Sum(x => x.TDS_AMOUNT);
            var TotalOfTotalNetAmount = DTList.Sum(x => x.TOTAL_NET_AMOUNT);
            var Cesstotal = DTList.Sum(x => x.CESSAMOUNT) ?? 0;
            TotalBaseAmount = TotalBaseAmount + Cesstotal;

            var TDS_BASE_AMOUNT = DTList.Sum(x => x.TDS_BASE_AMOUNT) ?? 0;
            var DISCOUNT_AMOUNT = DTList.Sum(x => x.AMOUNT) ?? 0;
            DateTime DocDate = DTList.Max(x => x.INVOICEDATE) ?? DateTime.Now;

            bool TOTALNET = true;
            bool TOTALTDS = true;
            var ESI_AMOUNT = DTList.Sum(x => x.ESI_AMOUNT) ?? 0;
            var ESIGL = _DB.NSP_TDS_TAX_TYPE_MASTER.Where(x => x.TAX_TYPE == "ES" && x.ACTIVE == 1).Select(x => x.GLACC).FirstOrDefault().ToString() ?? "";
            bool ESIENTRY = true;

            foreach (var paymentDetail in DTList)
            {
                if (DtCount > 1)
                {


                    var GSTObj = _DB.NSP_GST_TAX_MASTER.Where(x => x.TAXCODE == paymentDetail.GSTTAXCODE && x.TYPE == paymentDetail.GST_TYPE && x.TAXTYPE == paymentDetail.TAX_TYPE && x.PROFITCENTER == ProfitCenter && x.ACTIVE == 1).FirstOrDefault();
                    var SgstGL = GSTObj != null ? GSTObj.TYPE == "SGST/CGST" ? (GSTObj.GLACC1 != null ? GSTObj.GLACC1.ToString() : "") : "" : "";
                    var CgstGL = GSTObj != null ? GSTObj.TYPE == "SGST/CGST" ? (GSTObj.GLACC1 != null ? GSTObj.GLACC2.ToString() : "") : "" : "";
                    var SGSTPAYABLE = GSTObj != null ? GSTObj.PAYABLE_GLACC1.ToString() ?? "" : "";
                    var CGSTPAYABLE = GSTObj != null ? GSTObj.PAYABLE_GLACC2.ToString() ?? "" : "";
                    decimal SGSTAMOUNT_ = (paymentDetail.SGSTAMOUNT == null ? 0 : paymentDetail.SGSTAMOUNT) ?? 0;
                    decimal CGSTAMOUNT_ = (paymentDetail.CGSTAMOUNT == null ? 0 : paymentDetail.CGSTAMOUNT) ?? 0;
                    var Isdeducted = GSTObj != null ? GSTObj.ISDEDUCTED : "";
                    var IgstGL = GSTObj != null ? GSTObj.TYPE == "IGST" ? (GSTObj.GLACC1 != null ? GSTObj.GLACC1.ToString() : "") : "" : "";
                    decimal IGSTAMOUNT_ = (paymentDetail.IGSTAMOUNT == null ? 0 : paymentDetail.IGSTAMOUNT) ?? 0;
                    var TotalGStAmount = paymentDetail.TOTAL_GST_AMOUNT;
                    var TDSWithHolding = _DB.NSP_TDS_TAX_TYPE_MASTER.Where(x => x.TAX_TYPE == paymentDetail.TDS_TYPE && x.ACTIVE == 1).Select(x => x.WITHHOLDING_TAX_NAME).FirstOrDefault() ?? "";
                    var TDSGL = _DB.NSP_TDS_TAX_TYPE_MASTER.Where(x => x.TAX_TYPE == paymentDetail.TDS_TYPE && x.ACTIVE == 1).Select(x => x.GLACC).FirstOrDefault().ToString() ?? "";

                    decimal BASE_AMOUNT_ = Isdeducted == "N" ? decimal.Parse((paymentDetail.BASE_AMOUNT ?? 0).ToString()) +
                        decimal.Parse(TotalGStAmount.ToString()) : (paymentDetail.BASE_AMOUNT ?? 0);
                    decimal DtNEtTotal_AMOUNT_ = TotalOfTotalNetAmount ?? 0;

                    // decimal BASE_AMOUNT_ = paymentDetail.BASE_AMOUNT ?? 0;

                    var LINFN = "";
                    var taxCode = "";

                    string ReqCode = paymentHeader.VENDORCODE;
                    string REqLifn = paymentHeader.CATEGORY != "Dealer" ? ReqCode.PadLeft(10, '0') : ReqCode;

                    var CessGL = GSTObj != null ? (GSTObj.CESS_GLACC != null ? GSTObj.CESS_GLACC.ToString() : "") : "";
                    var CessAmount = (paymentDetail.CESSAMOUNT == null ? 0 : paymentDetail.CESSAMOUNT) ?? 0;

                    var ItemTextForDummy = "Base Amount:" + paymentHeader.PURPOSEOFEXPENSES;
                    var ItemTextForReqInvoice = "Invoice Amount :" + paymentHeader.PURPOSEOFEXPENSES;

                    var DummyGLEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, CostCenter, ProfitCenter, LINFN, ItemTextForDummy, paymentDetail, "", "DEBIT", "", BASE_AMOUNT_, "", "", taxCode);
                    var ReqGLEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, REqLifn, ItemTextForReqInvoice, paymentDetail, TDSWithHolding, "CREDIT", "", DtNEtTotal_AMOUNT_, paymentDetail.TDS_TYPE, "", paymentDetail.GSTTAXCODE, TDS_BASE_AMOUNT, DISCOUNT_AMOUNT);

                    DataList.Add(DummyGLEntry);
                    if (TOTALNET == true)
                    {
                        DataList.Add(ReqGLEntry);
                        TOTALNET = false;
                    }



                    if (paymentDetail.TAX_TYPE != "GST-NIL" && paymentDetail.GST_TYPE == "SGST/CGST")
                    {

                        var ItemTextForSGST = "SGST: " + paymentHeader.PURPOSEOFEXPENSES;
                        var ItemTextForCGST = "CGST: " + paymentHeader.PURPOSEOFEXPENSES;
                        var ItemTextForReSGST = "PAYABLE SGST: " + paymentHeader.PURPOSEOFEXPENSES;
                        var ItemTextForReCGST = "PAYABLE CGST: " + paymentHeader.PURPOSEOFEXPENSES;

                        var SGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForSGST, paymentDetail, "", "DEBIT", SgstGL, SGSTAMOUNT_, "", "", "");
                        var ReSGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForReSGST, paymentDetail, "", "CREDIT", SGSTPAYABLE, SGSTAMOUNT_, "", "", "");
                        var CGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForCGST, paymentDetail, "", "DEBIT", CgstGL, CGSTAMOUNT_, "", "", "");
                        var ReCGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForReCGST, paymentDetail, "", "CREDIT", CGSTPAYABLE, CGSTAMOUNT_, "", "", "");
                        if (SGSTAMOUNT_ > 0 && SgstGL != "" && Isdeducted == "Y")
                        {
                            DataList.Add(SGSTEntry);
                        }
                        if (CGSTAMOUNT_ > 0 && CgstGL != "" && Isdeducted == "Y")
                        {
                            DataList.Add(CGSTEntry);
                        }

                        if (paymentDetail.TAX_TYPE == "RCM Charges")
                        {
                            if (SGSTAMOUNT_ > 0 && SGSTPAYABLE != "")
                            {
                                DataList.Add(ReSGSTEntry);
                            }
                            if (CGSTAMOUNT_ > 0 && CGSTPAYABLE != "")
                            {
                                DataList.Add(ReCGSTEntry);
                            }

                        }

                    }

                    if (paymentDetail.TAX_TYPE != "GST-NIL" && paymentDetail.GST_TYPE == "IGST")
                    {
                        var ItemTextForIGST = "IGST: " + paymentHeader.PURPOSEOFEXPENSES;
                        var ItemTextForReIGST = "PAYABLE IGST: " + paymentHeader.PURPOSEOFEXPENSES;
                        var IGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForIGST, paymentDetail, "", "DEBIT", IgstGL, IGSTAMOUNT_, "", "", "");
                        var ReIGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForReIGST, paymentDetail, "", "CREDIT", SGSTPAYABLE, IGSTAMOUNT_, "", "", "");

                        if (IGSTAMOUNT_ > 0 && IgstGL != "" && Isdeducted == "Y")
                        {
                            DataList.Add(IGSTEntry);
                        }


                        if (paymentDetail.TAX_TYPE == "RCM Charges")
                        {
                            if (IGSTAMOUNT_ > 0 && SGSTPAYABLE != "")
                            {
                                DataList.Add(ReIGSTEntry);
                            }
                        }
                    }

                    if (paymentDetail.TDS == "Yes" && paymentDetail.TDS_TYPE != null)
                    {
                        decimal TDS_AMOUNT_ = TotalTdsAmount ?? 0;
                        var ItemTextForTDS = "TDS: " + paymentHeader.PURPOSEOFEXPENSES;
                        var TDsEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForTDS, paymentDetail, "", "CREDIT", TDSGL, TDS_AMOUNT_, "", "", "");

                        if (TDS_AMOUNT_ > 0 && TOTALTDS == true)
                        {
                            DataList.Add(TDsEntry);
                            TOTALTDS = false;
                        }

                    }


                    if (CessGL != "" && CessAmount != 0)
                    {
                        var ItemTextForCess = "Cess Amount: " + paymentHeader.PURPOSEOFEXPENSES;
                        var CessEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForCess, paymentDetail, "", "CREDIT", CessGL, CessAmount, "", "", "");
                        DataList.Add(CessEntry);
                    }
                    if (paymentDetail.IS_ESI == "Yes" && ESIENTRY == true && ESIGL != "" && ESI_AMOUNT != 0)
                    {
                        var ItemTextForTDS = "ES: " + paymentHeader.PURPOSEOFEXPENSES;
                        var ESI_Entry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForTDS, paymentDetail, "", "CREDIT", ESIGL, ESI_AMOUNT, "", "", "");
                        ESIENTRY = false;
                        DataList.Add(ESI_Entry);
                    }
                }
                else
                {

                    var GSTObj = _DB.NSP_GST_TAX_MASTER.Where(x => x.TAXCODE == paymentDetail.GSTTAXCODE && x.TYPE == paymentDetail.GST_TYPE && x.TAXTYPE == paymentDetail.TAX_TYPE && x.PROFITCENTER == ProfitCenter && x.ACTIVE == 1).FirstOrDefault();
                    var SgstGL = GSTObj != null ? GSTObj.TYPE == "SGST/CGST" ? (GSTObj.GLACC1 != null ? GSTObj.GLACC1.ToString() : "") : "" : "";
                    var CgstGL = GSTObj != null ? GSTObj.TYPE == "SGST/CGST" ? (GSTObj.GLACC1 != null ? GSTObj.GLACC2.ToString() : "") : "" : "";
                    var SGSTPAYABLE = GSTObj != null ? GSTObj.PAYABLE_GLACC1.ToString() ?? "" : "";
                    var CGSTPAYABLE = GSTObj != null ? GSTObj.PAYABLE_GLACC2.ToString() ?? "" : "";
                    decimal SGSTAMOUNT_ = (paymentDetail.SGSTAMOUNT == null ? 0 : paymentDetail.SGSTAMOUNT) ?? 0;
                    decimal CGSTAMOUNT_ = (paymentDetail.CGSTAMOUNT == null ? 0 : paymentDetail.CGSTAMOUNT) ?? 0;
                    var Isdeducted = GSTObj != null ? GSTObj.ISDEDUCTED : "";
                    var IgstGL = GSTObj != null ? GSTObj.TYPE == "IGST" ? (GSTObj.GLACC1 != null ? GSTObj.GLACC1.ToString() : "") : "" : "";
                    decimal IGSTAMOUNT_ = (paymentDetail.IGSTAMOUNT == null ? 0 : paymentDetail.IGSTAMOUNT) ?? 0;
                    var TotalGStAmount = paymentDetail.TOTAL_GST_AMOUNT;
                    var TDSWithHolding = _DB.NSP_TDS_TAX_TYPE_MASTER.Where(x => x.TAX_TYPE == paymentDetail.TDS_TYPE && x.ACTIVE == 1).Select(x => x.WITHHOLDING_TAX_NAME).FirstOrDefault() ?? "";
                    var TDSGL = _DB.NSP_TDS_TAX_TYPE_MASTER.Where(x => x.TAX_TYPE == paymentDetail.TDS_TYPE && x.ACTIVE == 1).Select(x => x.GLACC).FirstOrDefault().ToString() ?? "";

                    decimal BASE_AMOUNT_ = Isdeducted == "N" ? decimal.Parse((paymentDetail.BASE_AMOUNT ?? 0).ToString()) +
                        decimal.Parse(TotalGStAmount.ToString()) : (paymentDetail.BASE_AMOUNT ?? 0);
                    decimal DtNEtTotal_AMOUNT_ = TotalOfTotalNetAmount ?? 0;
                    // decimal BASE_AMOUNT_ = paymentDetail.BASE_AMOUNT ?? 0;

                    var LINFN = "";
                    var taxCode = "";

                    string ReqCode = paymentHeader.VENDORCODE;
                    string REqLifn = paymentHeader.CATEGORY != "Dealer" ? ReqCode.PadLeft(10, '0') : ReqCode;

                    var CessGL = GSTObj != null ? (GSTObj.CESS_GLACC != null ? GSTObj.CESS_GLACC.ToString() : "") : "";
                    var CessAmount = (paymentDetail.CESSAMOUNT == null ? 0 : paymentDetail.CESSAMOUNT) ?? 0;

                    var ItemTextForDummy = "Base Amount:" + paymentHeader.PURPOSEOFEXPENSES;
                    var ItemTextForReqInvoice = "Invoice Amount :" + paymentHeader.PURPOSEOFEXPENSES;

                    var DummyGLEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, CostCenter, ProfitCenter, LINFN, ItemTextForDummy, paymentDetail, "", "DEBIT", "", BASE_AMOUNT_, "", "", taxCode);
                    var ReqGLEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, REqLifn, ItemTextForReqInvoice, paymentDetail, TDSWithHolding, "CREDIT", "", DtNEtTotal_AMOUNT_, paymentDetail.TDS_TYPE, "", paymentDetail.GSTTAXCODE, TDS_BASE_AMOUNT, DISCOUNT_AMOUNT);
                    DataList.Add(DummyGLEntry);
                    DataList.Add(ReqGLEntry);


                    if (paymentDetail.TAX_TYPE != "GST-NIL" && paymentDetail.GST_TYPE == "SGST/CGST")
                    {
                        var ItemTextForSGST = "SGST: " + paymentHeader.PURPOSEOFEXPENSES;
                        var ItemTextForCGST = "CGST: " + paymentHeader.PURPOSEOFEXPENSES;
                        var ItemTextForReSGST = "PAYABLE SGST: " + paymentHeader.PURPOSEOFEXPENSES;
                        var ItemTextForReCGST = "PAYABLE CGST: " + paymentHeader.PURPOSEOFEXPENSES;

                        var SGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForSGST, paymentDetail, "", "DEBIT", SgstGL, SGSTAMOUNT_, "", "", "");
                        var ReSGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForReSGST, paymentDetail, "", "CREDIT", SGSTPAYABLE, SGSTAMOUNT_, "", "", "");
                        var CGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForCGST, paymentDetail, "", "DEBIT", CgstGL, CGSTAMOUNT_, "", "", "");
                        var ReCGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForReCGST, paymentDetail, "", "CREDIT", CGSTPAYABLE, CGSTAMOUNT_, "", "", "");
                        if (SGSTAMOUNT_ > 0 && SgstGL != "" && Isdeducted == "Y")
                        {
                            DataList.Add(SGSTEntry);
                        }
                        if (CGSTAMOUNT_ > 0 && CgstGL != "" && Isdeducted == "Y")
                        {
                            DataList.Add(CGSTEntry);
                        }

                        if (paymentDetail.TAX_TYPE == "RCM Charges")
                        {
                            if (SGSTAMOUNT_ > 0 && SGSTPAYABLE != "")
                            {
                                DataList.Add(ReSGSTEntry);
                            }
                            if (CGSTAMOUNT_ > 0 && CGSTPAYABLE != "")
                            {
                                DataList.Add(ReCGSTEntry);
                            }

                        }

                    }

                    if (paymentDetail.TAX_TYPE != "GST-NIL" && paymentDetail.GST_TYPE == "IGST")
                    {
                        var ItemTextForIGST = "IGST: " + paymentHeader.PURPOSEOFEXPENSES;
                        var ItemTextForReIGST = "PAYABLE IGST: " + paymentHeader.PURPOSEOFEXPENSES;

                        var IGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForIGST, paymentDetail, "", "DEBIT", IgstGL, IGSTAMOUNT_, "", "", "");
                        var ReIGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForReIGST, paymentDetail, "", "CREDIT", SGSTPAYABLE, IGSTAMOUNT_, "", "", "");

                        if (IGSTAMOUNT_ > 0 && IgstGL != "" && Isdeducted == "Y")
                        {
                            DataList.Add(IGSTEntry);
                        }


                        if (paymentDetail.TAX_TYPE == "RCM Charges")
                        {
                            if (IGSTAMOUNT_ > 0 && SGSTPAYABLE != "")
                            {
                                DataList.Add(ReIGSTEntry);
                            }
                        }
                    }

                    if (paymentDetail.TDS == "Yes" && paymentDetail.TDS_TYPE != null)
                    {
                        decimal TDS_AMOUNT_ = paymentDetail.TDS_AMOUNT ?? 0;
                        var ItemTextForTDS = "TDS: " + paymentHeader.PURPOSEOFEXPENSES;
                        var TDsEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForTDS, paymentDetail, "", "CREDIT", TDSGL, TDS_AMOUNT_, "", "", "");

                        if (TDS_AMOUNT_ > 0)
                        {
                            DataList.Add(TDsEntry);
                        }

                    }


                    if (CessGL != "" && CessAmount != 0)
                    {
                        var ItemTextForCess = "Cess Amount: " + paymentHeader.PURPOSEOFEXPENSES;
                        var CessEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForCess, paymentDetail, "", "CREDIT", CessGL, CessAmount, "", "", "");
                        DataList.Add(CessEntry);
                    }

                    if (paymentDetail.IS_ESI == "Yes" && ESIENTRY == true && ESIGL != "" && ESI_AMOUNT != 0)
                    {
                        var ItemTextForTDS = "ES: " + paymentHeader.PURPOSEOFEXPENSES;
                        var ESI_Entry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForTDS, paymentDetail, "", "CREDIT", ESIGL, ESI_AMOUNT, "", "", "");
                        ESIENTRY = false;
                        DataList.Add(ESI_Entry);
                    }
                }

            }

            return DataList;
        }

        public List<NSP_PaymentDetails> GetENTRYForPL(int HEADERID, string InvoiceNo, int detailID, string DocType, int LoginCode, string CostCenter, string BusinessCenter)
        {
            var DataList = new List<NSP_PaymentDetails>();

            var paymentHeader = _DB.NSP_PAYMENT_HEADER.FirstOrDefault(i => i.HEADERID == HEADERID);
            var paymentDetail = _DB.NSP_PAYMENT_DETAIL.FirstOrDefault(i => i.HEADERID == HEADERID && i.INVOICENO == InvoiceNo);
            var ProfitCenter = _DB.NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER.Where(x => x.BUSINESS_CENTER == BusinessCenter && x.ACTIVE == 1).Select(x => x.PROFIT_CENTER).FirstOrDefault() ?? "";
            var BusinessPlace = _DB.NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER.Where(x => x.BUSINESS_CENTER == BusinessCenter && x.ACTIVE == 1).Select(x => x.BUSINESS_PLACE).FirstOrDefault() ?? "";


            var GSTObj = _DB.NSP_GST_TAX_MASTER.Where(x => x.TAXCODE == paymentDetail.GSTTAXCODE && x.TYPE == paymentDetail.GST_TYPE && x.TAXTYPE == paymentDetail.TAX_TYPE && x.PROFITCENTER == ProfitCenter && x.ACTIVE == 1).FirstOrDefault();
            var SgstGL = GSTObj != null ? GSTObj.TYPE == "SGST/CGST" ? (GSTObj.GLACC1 != null ? GSTObj.GLACC1.ToString() : "") : "" : "";
            var CgstGL = GSTObj != null ? GSTObj.TYPE == "SGST/CGST" ? (GSTObj.GLACC1 != null ? GSTObj.GLACC2.ToString() : "") : "" : "";
            var SGSTPAYABLE = GSTObj != null ? GSTObj.PAYABLE_GLACC1.ToString() ?? "" : "";
            var CGSTPAYABLE = GSTObj != null ? GSTObj.PAYABLE_GLACC2.ToString() ?? "" : "";
            decimal SGSTAMOUNT_ = (paymentDetail.SGSTAMOUNT == null ? 0 : paymentDetail.SGSTAMOUNT) ?? 0;
            decimal CGSTAMOUNT_ = (paymentDetail.CGSTAMOUNT == null ? 0 : paymentDetail.CGSTAMOUNT) ?? 0;
            var Isdeducted = GSTObj != null ? GSTObj.ISDEDUCTED : "";
            var IgstGL = GSTObj != null ? GSTObj.TYPE == "IGST" ? (GSTObj.GLACC1 != null ? GSTObj.GLACC1.ToString() : "") : "" : "";
            decimal IGSTAMOUNT_ = (paymentDetail.IGSTAMOUNT == null ? 0 : paymentDetail.IGSTAMOUNT) ?? 0;
            var TotalGStAmount = paymentDetail.TOTAL_GST_AMOUNT;
            var TDSWithHolding = _DB.NSP_TDS_TAX_TYPE_MASTER.Where(x => x.TAX_TYPE == paymentDetail.TDS_TYPE && x.ACTIVE == 1).Select(x => x.WITHHOLDING_TAX_NAME).FirstOrDefault() ?? "";
            var TDSGL = _DB.NSP_TDS_TAX_TYPE_MASTER.Where(x => x.TAX_TYPE == paymentDetail.TDS_TYPE && x.ACTIVE == 1).Select(x => x.GLACC).FirstOrDefault().ToString() ?? "";

            var TDS_BASE_AMOUNT = paymentDetail.TDS_BASE_AMOUNT ?? 0;
            var DISCOUNT_AMOUNT = paymentDetail.AMOUNT ?? 0;
            DateTime DocDate = paymentDetail.INVOICEDATE ?? DateTime.Now;

            var CessGL = GSTObj != null ? (GSTObj.CESS_GLACC != null ? GSTObj.CESS_GLACC.ToString() : "") : "";
            var CessAmount = (paymentDetail.CESSAMOUNT == null ? 0 : paymentDetail.CESSAMOUNT) ?? 0;
            var ESI_AMOUNT = (paymentDetail.ESI_AMOUNT == null ? 0 : paymentDetail.ESI_AMOUNT) ?? 0;
            var ESIGL = _DB.NSP_TDS_TAX_TYPE_MASTER.Where(x => x.TAX_TYPE == "ES" && x.ACTIVE == 1).Select(x => x.GLACC).FirstOrDefault().ToString() ?? "";
            bool ESIENTRY = true;

            decimal BASE_AMOUNT_ = paymentDetail.BASE_AMOUNT ?? 0;
            decimal DtNEtTotal_AMOUNT_ = paymentDetail.TOTAL_NET_AMOUNT ?? 0;
            string ReqCode = paymentHeader.VENDORCODE;

            string REqLifn = paymentHeader.CATEGORY != "Dealer" ? ReqCode.PadLeft(10, '0') : ReqCode;

            var ItemTextForDummy = "Base Amount: " + paymentHeader.PURPOSEOFEXPENSES;
            var ItemTextForReqInvoice = "Invoice Amount :" + paymentHeader.PURPOSEOFEXPENSES;
            var DummyGLEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, CostCenter, ProfitCenter, "", ItemTextForDummy, paymentDetail, "", "DEBIT", "", BASE_AMOUNT_, "", "", "");
            var ReqGLEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, REqLifn, ItemTextForReqInvoice, paymentDetail, TDSWithHolding, "CREDIT", "", DtNEtTotal_AMOUNT_, paymentDetail.TDS_TYPE, "", paymentDetail.GSTTAXCODE, TDS_BASE_AMOUNT, DISCOUNT_AMOUNT);

            if (paymentHeader.CATEGORY == "Employees")
            {
                DummyGLEntry.ZType = "CREDIT";
                ReqGLEntry.ZType = "DEBIT";

            }

            if (paymentHeader.CATEGORY == "Dealer")
            {
                ReqGLEntry.LIFNR = "";
                ReqGLEntry.KUNNR = REqLifn;

            }




            DataList.Add(DummyGLEntry);
            DataList.Add(ReqGLEntry);

            if (paymentDetail.TAX_TYPE != "GST-NIL" && paymentDetail.GST_TYPE == "SGST/CGST")
            {
                var ItemTextForSGST = "SGST: " + paymentHeader.PURPOSEOFEXPENSES;
                var ItemTextForCGST = "CGST: " + paymentHeader.PURPOSEOFEXPENSES;
                var ItemTextForReSGST = "PAYABLE SGST: " + paymentHeader.PURPOSEOFEXPENSES;
                var ItemTextForReCGST = "PAYABLE CGST: " + paymentHeader.PURPOSEOFEXPENSES;

                var SGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForSGST, paymentDetail, "", "DEBIT", SgstGL, SGSTAMOUNT_, "", "", "");
                var ReSGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForReSGST, paymentDetail, "", "CREDIT", SGSTPAYABLE, SGSTAMOUNT_, "", "", "");
                var CGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForCGST, paymentDetail, "", "DEBIT", CgstGL, CGSTAMOUNT_, "", "", "");
                var ReCGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForReCGST, paymentDetail, "", "CREDIT", CGSTPAYABLE, CGSTAMOUNT_, "", "", "");
                if (SGSTAMOUNT_ > 0 && SgstGL != "" && Isdeducted == "Y")
                {
                    DataList.Add(SGSTEntry);
                }
                if (CGSTAMOUNT_ > 0 && CgstGL != "" && Isdeducted == "Y")
                {
                    DataList.Add(CGSTEntry);
                }

                if (paymentDetail.TAX_TYPE == "RCM Charges")
                {
                    if (SGSTAMOUNT_ > 0 && SGSTPAYABLE != "")
                    {
                        DataList.Add(ReSGSTEntry);
                    }
                    if (CGSTAMOUNT_ > 0 && CGSTPAYABLE != "")
                    {
                        DataList.Add(ReCGSTEntry);
                    }

                }

            }

            if (paymentDetail.TAX_TYPE != "GST-NIL" && paymentDetail.GST_TYPE == "IGST")
            {
                var ItemTextForIGST = "IGST: " + paymentHeader.PURPOSEOFEXPENSES;
                var ItemTextForReIGST = "PAYABLE IGST: " + paymentHeader.PURPOSEOFEXPENSES;

                var IGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForIGST, paymentDetail, "", "DEBIT", IgstGL, IGSTAMOUNT_, "", "", "");
                var ReIGSTEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForReIGST, paymentDetail, "", "CREDIT", SGSTPAYABLE, IGSTAMOUNT_, "", "", "");

                if (IGSTAMOUNT_ > 0 && IgstGL != "" && Isdeducted == "Y")
                {
                    DataList.Add(IGSTEntry);
                }


                if (paymentDetail.TAX_TYPE == "RCM Charges")
                {
                    if (IGSTAMOUNT_ > 0 && SGSTPAYABLE != "")
                    {
                        DataList.Add(ReIGSTEntry);
                    }
                }
            }

            if (paymentDetail.TDS == "Yes" && paymentDetail.TDS_TYPE != null)
            {
                decimal TDS_AMOUNT_ = paymentDetail.TDS_AMOUNT ?? 0;
                var ItemTextForTDS = "TDS: " + paymentHeader.PURPOSEOFEXPENSES;
                var TDsEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForTDS, paymentDetail, "", "CREDIT", TDSGL, TDS_AMOUNT_, "", "", "");

                if (TDS_AMOUNT_ > 0)
                {
                    DataList.Add(TDsEntry);
                }

            }


            if (CessGL != "" && CessAmount != 0)
            {
                var ItemTextForCess = "Cess Amount: " + paymentHeader.PURPOSEOFEXPENSES;
                var CessEntry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForCess, paymentDetail, "", "CREDIT", CessGL, CessAmount, "", "", "");
                DataList.Add(CessEntry);
            }

            if (paymentDetail.IS_ESI == "Yes" && ESIENTRY == true && ESIGL != "" && ESI_AMOUNT != 0)
            {
                var ItemTextForTDS = "ES: " + paymentHeader.PURPOSEOFEXPENSES;
                var ESI_Entry = CreatePaymentDetails(DocType, BusinessPlace, BusinessCenter, "", ProfitCenter, "", ItemTextForTDS, paymentDetail, "", "CREDIT", ESIGL, ESI_AMOUNT, "", "", "");
                ESIENTRY = false;
                DataList.Add(ESI_Entry);
            }

            return DataList;
        }

        private NSP_PaymentDetails CreatePaymentDetails(string DocType, string BusinessPlace, string businessCenter, string costCenter, string profitCenter, string lifnr, string itemText, NSP_PAYMENT_DETAIL paymentDetail, string wtType, string zType, string glAccount = "", decimal amountDocumentCurrency = 0, string WTCode = "", string SPGLInd = "", string taxCode = "", decimal TDSBaseAmount = 0, decimal DiscountAmount = 0)
        {
            return new NSP_PaymentDetails
            {
                BusinessCenter = businessCenter,
                BusinessPlace = BusinessPlace,
                CostCenter = costCenter,
                ProfitCenter = profitCenter,
                AllocNumber = "",
                ItemText = itemText,
                LIFNR = lifnr,
                GLAccount = (glAccount == null ? "" : glAccount),
                TaxCode = (taxCode == null ? "" : taxCode),
                ZType = zType,
                AmountDocumentCurrency = amountDocumentCurrency,
                // START: ADDED BY AUMENTO: SR88023-CR5305: CHANGE 2.0 : AS DISCUSSED IN MEETING AS ON 25-DEC-2024
                //WTType = wtType,
                //WTCode = WTCode,
                WTType = WTCode,
                WTCode = "",
                // END: ADDED BY AUMENTO: SR88023-CR5305: CHANGE 2.0 : AS DISCUSSED IN MEETING AS ON 25-DEC-2024
                SPGLInd = SPGLInd,
                Currency = (paymentDetail.CURRENCY == null ? "" : paymentDetail.CURRENCY),
                DocType = DocType,
                HeaderText = itemText,
                RefDocNo = (paymentDetail.INVOICENO == null ? "" : paymentDetail.INVOICENO),
                DISCOUNT_AMOUNT = DiscountAmount,
                TDS_BASE_AMOUNT = TDSBaseAmount
            };
        }

        public List<NSP_PaymentDetails> GEtPostedInvoiceHistory(int HEADERID, string InvoiceNo)
        {
            var DataList = new List<NSP_PaymentDetails>();

            try
            {
                DataList = (from i in _DB.NSP_POSTING_DETAIL
                            where i.HEADERID == HEADERID && i.INVOICENO == InvoiceNo

                            select new NSP_PaymentDetails
                            {
                                BusinessCenter = i.BUSINESSCENTER,
                                BusinessPlace = i.BUSINESSPLACE,
                                CostCenter = i.COSTCENTER,
                                ProfitCenter = i.PROFITCENTER,
                                AllocNumber = "",
                                ItemText = i.ITEMTEXT,
                                LIFNR = i.LIFNR,
                                GLAccount = i.GLACCOUNT,
                                TaxCode = i.TAXCODE,
                                ZType = i.ZTYPE,
                                AmountDocumentCurrency = i.AMOUNTDOCUMENTCURRENCY,
                                WTType = i.WTTYPE,
                                WTCode = i.WTCODE,
                                SPGLInd = i.SPGLIND,
                                Currency = i.CURRENCY,
                                DocType = i.DOCTYPE,
                                HeaderText = i.HEADERTEXT,
                                RefDocNo = i.REFDOCNO,
                                PostingDocNo = i.POSTDOCNO,
                                Mode = i.POSTMODE,
                                SRNO = i.SRNO,
                                KUNNR = i.KUNNR,
                                DISCOUNT_AMOUNT = i.DISCOUNT_AMOUNT,
                                DOCDATE = i.DOCDATE,
                                TDS_BASE_AMOUNT = i.TDS_BASE_AMOUNT
                            }).OrderBy(x => x.SRNO).ToList();

            }
            catch (Exception ex)
            {


            }
            return DataList;

        }


        private LibResult InsertToNSP_PostingDtPaymentDetails(int HEADERID, string InvoiceNo, int detailID, List<NSP_PaymentDetails> PostingDt, int LoginCode, string Mode, string PostDocNo, DateTime? DocDate)
        {
            LibResult res = new LibResult();

            try
            {
                //using (var _db = new LCEntities())
                //{
                    var maxSrno = _DB.NSP_POSTING_DETAIL.Count()>0 ? _DB.NSP_POSTING_DETAIL.Max(x => x.SRNO) : 0;

                    foreach (var item in PostingDt)
                    {
                        maxSrno++;
                        NSP_POSTING_DETAIL newEntry = new NSP_POSTING_DETAIL
                        {
                            SRNO = maxSrno,
                            HEADERID = HEADERID,
                            INVOICENO = InvoiceNo,
                            DTID = detailID,
                            DOCTYPE = item.DocType,
                            POSTMODE = Mode,
                            POSTDOCNO = PostDocNo,
                            BUSINESSCENTER = item.BusinessCenter,
                            BUSINESSPLACE = item.BusinessPlace,
                            COSTCENTER = item.CostCenter,
                            PROFITCENTER = item.ProfitCenter,
                            ALLOCNUMBER = item.AllocNumber,
                            ITEMTEXT = item.ItemText,
                            LIFNR = item.LIFNR,
                            GLACCOUNT = item.GLAccount,
                            TAXCODE = item.TaxCode,
                            ZTYPE = item.ZType,
                            AMOUNTDOCUMENTCURRENCY = item.AmountDocumentCurrency,
                            WTTYPE = item.WTType,
                            WTCODE = item.WTCode,
                            SPGLIND = item.SPGLInd,
                            CURRENCY = item.Currency,
                            HEADERTEXT = item.HeaderText,
                            REFDOCNO = item.RefDocNo,
                            POSTEDBY = LoginCode,
                            POSTEDON = DateTime.Now,
                            KUNNR = item.KUNNR,
                            DOCDATE = DocDate,
                            DISCOUNT_AMOUNT = item.DISCOUNT_AMOUNT,
                            TDS_BASE_AMOUNT = item.TDS_BASE_AMOUNT
                        };
                        _DB.NSP_POSTING_DETAIL.Add(newEntry);

                    }

                    _DB.SaveChanges();

                    res.hasError = false;
                    res.errorMessage = "Details inserted successfully.";
                //}

            }
            catch (Exception ex)
            {

                res.hasError = true;
                res.errorMessage = $"An error occurred: {ex.Message}";
            }

            return res;
        }

        public List<object> GetBusinessCenterHelp()
        {
            List<object> BusinessCenterList = new List<object>();

            var Query = (from r in _DB.NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER
                         where r.ACTIVE == 1
                         select new
                         {
                             text = r.BUSINESS_CENTER,
                             value = r.PROFIT_CENTER
                         }).Distinct().ToList();

            BusinessCenterList.AddRange(Query);

            return BusinessCenterList;

        }

        #endregion

        #region  MAster

        #region Role Master
        public LibResult GetNSP_Role_Master_List_Data()
        {
            LibResult result = new LibResult();

            try
            {
                //using (var db = new LCEntities())
                //{
                    var data = (from role in _DB.NSP_ROLE_MASTER
                                join employee in _DB.ADEMPLOYEE_LC on role.ECODE equals employee.ADEMPCODE
                                where employee.ACTIVE == 1
                                select new
                                {
                                    SRNO = role.SRNO,
                                    DEPARTMENT = role.DEPARTMENT,
                                    SYSITEID = role.SYSITEID,
                                    ECODE = role.ECODE,
                                    FIRSTNAME = employee.FIRSTNAME,
                                    LASTNAME = employee.LASTNAME,
                                    DESIGNATION = role.DESIGNATION,
                                    SYSITTDESCRIPTION = role.SYSITTDESCRIPTION,
                                    SEQUENCE_NO = role.SEQUENCE_NO,
                                    ACTIVE = role.ACTIVE
                                }).ToList();

                    result.resultObject = data;
                    result.hasError = false;
                //}
            }
            catch (Exception ex)
            {
                result.hasError = true;
                result.errorMessage = ex.Message.ToString();
            }

            return result;
        }

        public LibResult GetNSP_Role_Master_Edit_Data(int Srno)
        {
            LibResult Res = new LibResult();

            try
            {

                var Data = (from i in _DB.NSP_ROLE_MASTER where i.SRNO == Srno select i).FirstOrDefault();

                Res.resultObject = Data;
                Res.hasError = false;


            }
            catch (Exception ex)
            {
                Res.hasError = true;
                Res.errorMessage = ex.Message.ToString();
            }

            return Res;
        }

        public LibResult NSP_Role_Master_SaveAndUpdate_Data(NSP_ROLE_MASTER Obj, string Mode, int loginCode)
        {
            LibResult Res = new LibResult();
            try
            {
                if (Mode == "SAVE")
                {
                    var Exist = _DB.NSP_ROLE_MASTER.Where(x => x.SYSITEID == Obj.SYSITEID && x.ACTIVE == 1 && x.ECODE == Obj.ECODE && x.DEPARTMENT == Obj.DEPARTMENT).ToList().Count();

                    if (Exist > 0)
                    {
                        Res.hasError = true;
                        Res.errorMessage = "Record alereat exist!!";
                    }
                    else
                    {
                        var MaxSrno = _DB.NSP_ROLE_MASTER.Select(x => x.SRNO).DefaultIfEmpty().Max();

                        int? maxSeqNo = _DB.NSP_ROLE_MASTER.Where(x => x.SYSITEID == Obj.SYSITEID && x.ACTIVE == 1).Select(x => (int?)x.SEQUENCE_NO).Max().GetValueOrDefault();

                        decimal Seqno = maxSeqNo.GetValueOrDefault() + 0.1m;

                        var SiteDesc = _DB.SYSITE_LC.Where(x => x.SYSITEID == Obj.SYSITEID && x.ACTIVE == 1).Select(x => x.DESCRIP).FirstOrDefault().ToString();
                        var Ecode = long.Parse(Obj.ECODE.ToString());
                        long? nullableDesignationId = _DB.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == Ecode && x.SYKI == _Syki && x.ACTIVE == 1).Select(x => x.ADDESIGNATIONID).FirstOrDefault();

                        long DesignationId = nullableDesignationId.HasValue ? (long)nullableDesignationId.Value : 0L;
                        var Designation = DesignationId == 0L ? "" : _DB.ADDESIGNATION_LC.Where(x => x.ADDESIGNATIONID == DesignationId && x.ACTIVE == 1).Select(x => x.DESCRIP).FirstOrDefault().ToString();

                        MaxSrno++;
                        Obj.SRNO = MaxSrno;
                        Obj.SYSITTDESCRIPTION = SiteDesc;
                        Obj.DESIGNATION = Designation;
                        Obj.ADDEDBY = loginCode;
                        Obj.SEQUENCE_NO = Seqno;
                        Obj.ADDEDON = DateTime.Now;
                        Obj.ACTIVE = 1;
                        _DB.Entry(Obj).State = EntityState.Added;
                        _DB.SaveChanges();
                        var LogInsert = InsertToNSP_ROLE_MASTER_LOG(Obj, "ADD");
                    }

                    Res.hasError = false;
                    Res.errorMessage = "Record Inserted Successfully";

                }

                else
                {
                    var Exist = _DB.NSP_ROLE_MASTER.Where(x => x.SYSITEID == Obj.SYSITEID && x.ACTIVE == 1 && x.ECODE == Obj.ECODE && x.DEPARTMENT == Obj.DEPARTMENT).ToList().Count();

                    if (Exist > 1)
                    {
                        Res.hasError = true;
                        Res.errorMessage = "Record alereat exist!!";
                    }
                    else
                    {
                        var ExistObj = _DB.NSP_ROLE_MASTER.Where(x => x.SRNO == Obj.SRNO).FirstOrDefault();
                        var SiteDesc = _DB.SYSITE_LC.Where(x => x.SYSITEID == Obj.SYSITEID && x.ACTIVE == 1).Select(x => x.DESCRIP).FirstOrDefault().ToString();
                        var Ecode = long.Parse(Obj.ECODE.ToString());
                        long? nullableDesignationId = _DB.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == Ecode && x.SYKI == _Syki && x.ACTIVE == 1).Select(x => x.ADDESIGNATIONID).FirstOrDefault();

                        long DesignationId = nullableDesignationId.HasValue ? (long)nullableDesignationId.Value : 0L;
                        var Designation = DesignationId == 0L ? "" : _DB.ADDESIGNATION_LC.Where(x => x.ADDESIGNATIONID == DesignationId && x.ACTIVE == 1).Select(x => x.DESCRIP).FirstOrDefault().ToString();


                        ExistObj.DEPARTMENT = Obj.DEPARTMENT;
                        ExistObj.ECODE = Obj.ECODE;
                        ExistObj.DESIGNATION = Designation;
                        ExistObj.SYSITTDESCRIPTION = SiteDesc;
                        ExistObj.POST = Obj.POST;
                        ExistObj.REVERSE = Obj.REVERSE;
                        ExistObj.SYSITEID = Obj.SYSITEID;


                        ExistObj.UPDATEDBY = loginCode;
                        ExistObj.UPDATEDON = DateTime.Now;
                        _DB.Entry(ExistObj).State = EntityState.Modified;
                        _DB.SaveChanges();
                        var LogInsert = InsertToNSP_ROLE_MASTER_LOG(ExistObj, "UPDATE");
                    }

                    Res.hasError = false;
                    Res.errorMessage = "Record Updated Successfully";

                }




            }
            catch (Exception ex)
            {
                Res.hasError = true;
                Res.errorMessage = ex.Message.ToString();
            }

            return Res;
        }

        public LibResult DeActive_Role_Master_Record(int SrNo, int loginCode)
        {
            LibResult res = new LibResult();
            try
            {

                var DeleteRecord = _DB.NSP_ROLE_MASTER.Find(SrNo);
                short ActiveStatus = (DeleteRecord.ACTIVE == (short)1 ? (short)0 : (short)1);
                string SavedMsg = (DeleteRecord.ACTIVE == (short)1 ? "Record DeActived Successfully" : "Record Actived Successfully");
                string Status = (DeleteRecord.ACTIVE == (short)1 ? "DEACTIVED" : "ACTIVED");
                if (DeleteRecord != null)
                {
                    DeleteRecord.ACTIVE = ActiveStatus;
                    DeleteRecord.UPDATEDBY = loginCode;
                    DeleteRecord.UPDATEDON = DateTime.Now;
                    _DB.Entry(DeleteRecord).State = EntityState.Modified;

                    _DB.SaveChanges();
                    res.hasError = false;
                    res.errorMessage = SavedMsg;
                    var LogInsert = InsertToNSP_ROLE_MASTER_LOG(DeleteRecord, Status);
                }
                else
                {
                    res.hasError = true;
                    res.errorMessage = "Record not found";
                }
            }
            catch (Exception ex)
            {
                res.hasError = true;
                res.errorMessage = ex.Message.ToString();
            }

            return res;
        }

        public LibResult BindPlantName()
        {
            LibResult Res = new LibResult();
            try
            {
                //using (var db = new LCEntities())
                //{
                    var Data = (from r in _DB.SYSITE_LC
                                select new
                                {
                                    Text = r.DESCRIP,
                                    Value = r.SYSITEID
                                }).Distinct()
                                  .Where(item => !string.IsNullOrEmpty(item.Text))
                                  .ToList();
                    Res.resultObject = Data;
                //}
            }
            catch (Exception ex)
            {
                Res.hasError = true;
                Res.errorMessage = ex.ToString();
            }
            return Res;
        }

        private bool InsertToNSP_ROLE_MASTER_LOG(NSP_ROLE_MASTER NSP, string Mode)
        {
            try
            {
                //using (var db = new LCEntities())
                //{
                    NSP_ROLE_MASTER_LOG NspLog = new NSP_ROLE_MASTER_LOG();
                    var maxSrno = _DB.NSP_ROLE_MASTER_LOG.DefaultIfEmpty().Max(x => x == null ? 0 : x.SRNO);
                    NspLog.SRNO = maxSrno + 1;
                    NspLog.MAINTBLSRNO = NSP.SRNO;
                    NspLog.DEPARTMENT = NSP.DEPARTMENT;
                    NspLog.SYSITEID = NSP.SYSITEID;
                    NspLog.ECODE = NSP.ECODE;
                    NspLog.ADDEDBY = NSP.ADDEDBY;
                    NspLog.ADDEDON = NSP.ADDEDON;
                    NspLog.UPDATEDBY = NSP.UPDATEDBY;
                    NspLog.UPDATEDON = NSP.UPDATEDON;
                    NspLog.DESIGNATION = NSP.DESIGNATION;
                    NspLog.SEQUENCE_NO = NSP.SEQUENCE_NO;
                    NspLog.SYSITTDESCRIPTION = NSP.SYSITTDESCRIPTION;
                    NspLog.POST = NSP.POST;
                    NspLog.REVERSE = NSP.REVERSE;
                    NspLog.ACTIVE = NSP.ACTIVE;
                    NspLog.LOGMODE = Mode;
                    _DB.NSP_ROLE_MASTER_LOG.Add(NspLog);
                    _DB.SaveChanges();
                //}
            }
            catch (Exception ex)
            {

                return false;
            }

            return true;
        }


        #endregion

        #region TDS Master
        public LibResult GetNSP_TDS_TAX_TYPE_MASTER_List_Data()
        {
            LibResult result = new LibResult();

            try
            {
                //using (var db = new LCEntities())
                //{
                    var data = _DB.NSP_TDS_TAX_TYPE_MASTER.ToList();
                    result.resultObject = data;
                    result.hasError = false;
                //}
            }
            catch (Exception ex)
            {
                result.hasError = true;
                result.errorMessage = ex.Message.ToString();
            }

            return result;
        }

        public LibResult GetNSP_TDS_TAX_TYPE_MASTER_Edit_Data(int Srno)
        {
            LibResult Res = new LibResult();

            try
            {

                var Data = (from i in _DB.NSP_TDS_TAX_TYPE_MASTER where i.SRNO == Srno select i).FirstOrDefault();

                Res.resultObject = Data;
                Res.hasError = false;


            }
            catch (Exception ex)
            {
                Res.hasError = true;
                Res.errorMessage = ex.Message.ToString();
            }

            return Res;
        }


        public LibResult DeActive_NSP_TDS_TAX_TYPE_MASTER_Record(int SrNo, int loginCode)
        {
            LibResult res = new LibResult();
            try
            {

                var DeleteRecord = _DB.NSP_TDS_TAX_TYPE_MASTER.Find(SrNo);
                short ActiveStatus = (DeleteRecord.ACTIVE == (short)1 ? (short)0 : (short)1);
                string SavedMsg = (DeleteRecord.ACTIVE == (short)1 ? "Record DeActived Successfully" : "Record Actived Successfully");
                string Status = (DeleteRecord.ACTIVE == (short)1 ? "DEACTIVED" : "ACTIVED");
                if (DeleteRecord != null)
                {
                    DeleteRecord.ACTIVE = ActiveStatus;
                    DeleteRecord.UPDATEDBY = loginCode;
                    DeleteRecord.UPDATEDON = DateTime.Now;
                    _DB.Entry(DeleteRecord).State = EntityState.Modified;

                    _DB.SaveChanges();
                    res.hasError = false;
                    res.errorMessage = SavedMsg;
                    var LogInsert = InsertToNSP_NSP_TDS_TAX_TYPE_MASTER_LOG(DeleteRecord, Status);
                }
                else
                {
                    res.hasError = true;
                    res.errorMessage = "Record not found";
                }
            }
            catch (Exception ex)
            {
                res.hasError = true;
                res.errorMessage = ex.Message.ToString();
            }

            return res;
        }
        public LibResult NSP_NSP_TDS_TAX_TYPE_MASTER_SaveData(NSP_TDS_TAX_TYPE_MASTER Obj, int loginCode, string Mode)
        {
            LibResult Res = new LibResult();
            try
            {
                if (Mode == "ADD")
                {


                    var Exist = _DB.NSP_TDS_TAX_TYPE_MASTER.Where(x => x.TAX_TYPE == Obj.TAX_TYPE && x.WITHHOLDING_TAX_NAME == Obj.WITHHOLDING_TAX_NAME && x.ACTIVE == 1).ToList().Count();

                    if (Exist > 0)
                    {
                        Res.hasError = true;
                        Res.errorMessage = "Record alereat exist!!";
                    }
                    else
                    {
                        var MaxSrno = _DB.NSP_TDS_TAX_TYPE_MASTER.Select(x => x.SRNO).DefaultIfEmpty().Max();

                        MaxSrno++;
                        Obj.SRNO = MaxSrno;
                        Obj.TAX_TYPE = Obj.TAX_TYPE;
                        Obj.WITHHOLDING_TAX_NAME = Obj.WITHHOLDING_TAX_NAME;
                        Obj.ADDEDBY = loginCode;
                        Obj.ADDEDON = DateTime.Now;
                        Obj.ACTIVE = 1;
                        _DB.Entry(Obj).State = EntityState.Added;
                        _DB.SaveChanges();
                        var LogInsert = InsertToNSP_NSP_TDS_TAX_TYPE_MASTER_LOG(Obj, "ADD");
                    }
                    Res.hasError = false;
                    Res.errorMessage = "Record Inserted Successfully";
                }
                else
                {
                    var Exist = _DB.NSP_TDS_TAX_TYPE_MASTER.Where(x => x.TAX_TYPE == Obj.TAX_TYPE && x.WITHHOLDING_TAX_NAME == Obj.WITHHOLDING_TAX_NAME && x.ACTIVE == 1).ToList().Count();

                    if (Exist > 1)
                    {
                        Res.hasError = true;
                        Res.errorMessage = "Record alereat exist!!";
                    }
                    else
                    {
                        var ExistObj = _DB.NSP_TDS_TAX_TYPE_MASTER.Where(x => x.SRNO == Obj.SRNO).FirstOrDefault();


                        ExistObj.TAX_TYPE = Obj.TAX_TYPE;
                        ExistObj.WITHHOLDING_TAX_NAME = Obj.WITHHOLDING_TAX_NAME;
                        ExistObj.RATE = Obj.RATE;
                        ExistObj.GLACC = Obj.GLACC;
                        ExistObj.UPDATEDBY = loginCode;
                        ExistObj.UPDATEDON = DateTime.Now;
                        _DB.Entry(ExistObj).State = EntityState.Modified;
                        _DB.SaveChanges();
                        var LogInsert = InsertToNSP_NSP_TDS_TAX_TYPE_MASTER_LOG(ExistObj, "UPDATE");
                    }
                    Res.hasError = false;
                    Res.errorMessage = "Record Updated Successfully";
                }
            }
            catch (Exception ex)
            {
                Res.hasError = true;
                Res.errorMessage = ex.Message.ToString();
            }

            return Res;
        }

        private bool InsertToNSP_NSP_TDS_TAX_TYPE_MASTER_LOG(NSP_TDS_TAX_TYPE_MASTER NSP, string Mode)
        {
            try
            {
                //using (var db = new LCEntities())
                //{
                    NSP_TDS_TAX_TYPE_MASTER_LOG NspLog = new NSP_TDS_TAX_TYPE_MASTER_LOG();
                    var maxSrno = _DB.NSP_TDS_TAX_TYPE_MASTER_LOG.DefaultIfEmpty().Max(x => x == null ? 0 : x.SRNO);
                    NspLog.SRNO = maxSrno + 1;
                    NspLog.MAINTBLSRNO = NSP.SRNO;
                    NspLog.TAX_TYPE = NSP.TAX_TYPE;
                    NspLog.WITHHOLDING_TAX_NAME = NSP.WITHHOLDING_TAX_NAME;
                    NspLog.RATE = NSP.RATE;
                    NspLog.ADDEDBY = NSP.ADDEDBY;
                    NspLog.ADDEDON = NSP.ADDEDON;
                    NspLog.UPDATEDBY = NSP.UPDATEDBY;
                    NspLog.UPDATEDON = NSP.UPDATEDON;
                    NspLog.GLACC = NSP.GLACC;
                    NspLog.ACTIVE = NSP.ACTIVE;
                    NspLog.LOGMODE = Mode;
                    _DB.NSP_TDS_TAX_TYPE_MASTER_LOG.Add(NspLog);
                    _DB.SaveChanges();
                //}
            }
            catch (Exception ex)
            {

                return false;
            }

            return true;
        }
        #endregion

        #region Business Profit Master
        public LibResult GetNSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_List_Data()
        {
            LibResult result = new LibResult();

            try
            {
                //using (var db = new LCEntities())
                //{
                    var data = _DB.NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER.ToList();
                    result.resultObject = data;
                    result.hasError = false;
                //}
            }
            catch (Exception ex)
            {
                result.hasError = true;
                result.errorMessage = ex.Message.ToString();
            }

            return result;
        }


        public LibResult DeActive_NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_Record(int SrNo, int loginCode)
        {
            LibResult res = new LibResult();
            try
            {

                var DeleteRecord = _DB.NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER.Find(SrNo);
                short ActiveStatus = (DeleteRecord.ACTIVE == (short)1 ? (short)0 : (short)1);
                string SavedMsg = (DeleteRecord.ACTIVE == (short)1 ? "Record DeActived Successfully" : "Record Actived Successfully");
                string Status = (DeleteRecord.ACTIVE == (short)1 ? "DEACTIVED" : "ACTIVED");
                if (DeleteRecord != null)
                {
                    DeleteRecord.ACTIVE = ActiveStatus;
                    DeleteRecord.UPDATEDBY = loginCode;
                    DeleteRecord.UPDATEDON = DateTime.Now;
                    _DB.Entry(DeleteRecord).State = EntityState.Modified;

                    _DB.SaveChanges();
                    res.hasError = false;
                    res.errorMessage = SavedMsg;
                    var LogInsert = InsertToNSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_LOG(DeleteRecord, Status);
                }
                else
                {
                    res.hasError = true;
                    res.errorMessage = "Record not found";
                }
            }
            catch (Exception ex)
            {
                res.hasError = true;
                res.errorMessage = ex.Message.ToString();
            }

            return res;
        }
        public LibResult GetNSPBUPRO_MASTER_Edit_Data(int Srno)
        {
            LibResult Res = new LibResult();

            try
            {

                var Data = (from i in _DB.NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER where i.SRNO == Srno select i).FirstOrDefault();

                Res.resultObject = Data;
                Res.hasError = false;


            }
            catch (Exception ex)
            {
                Res.hasError = true;
                Res.errorMessage = ex.Message.ToString();
            }

            return Res;
        }

        public LibResult NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_SaveData(NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER Obj, int loginCode, string Mode)
        {
            LibResult Res = new LibResult();
            try
            {
                if (Mode == "ADD")
                {
                    var Exist = _DB.NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER.Where(x => x.BUSINESS_CENTER == Obj.BUSINESS_CENTER && x.BUSINESS_PLACE == Obj.BUSINESS_PLACE && x.PROFIT_CENTER == Obj.PROFIT_CENTER && x.ACTIVE == 1).ToList().Count();

                    if (Exist > 0)
                    {
                        Res.hasError = true;
                        Res.errorMessage = "Record alereat exist!!";
                    }
                    else
                    {
                        var MaxSrno = _DB.NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER.Select(x => x.SRNO).DefaultIfEmpty().Max();

                        MaxSrno++;
                        Obj.SRNO = MaxSrno;
                        Obj.BUSINESS_CENTER = Obj.BUSINESS_CENTER;
                        Obj.BUSINESS_PLACE = Obj.BUSINESS_PLACE;
                        Obj.PROFIT_CENTER = Obj.PROFIT_CENTER;
                        Obj.ADDEDBY = loginCode;
                        Obj.ADDEDON = DateTime.Now;
                        Obj.ACTIVE = 1;
                        _DB.Entry(Obj).State = EntityState.Added;
                        _DB.SaveChanges();
                        var LogInsert = InsertToNSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_LOG(Obj, "ADD");
                    }
                    Res.hasError = false;
                    Res.errorMessage = "Record Inserted Successfully";
                }
                else
                {
                    var Exist = _DB.NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER.Where(x => x.BUSINESS_CENTER == Obj.BUSINESS_CENTER && x.BUSINESS_PLACE == Obj.BUSINESS_PLACE && x.PROFIT_CENTER == Obj.PROFIT_CENTER && x.ACTIVE == 1).ToList().Count();

                    if (Exist > 1)
                    {
                        Res.hasError = true;
                        Res.errorMessage = "Record alereat exist!!";
                    }
                    else
                    {
                        var ExistObj = _DB.NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER.Where(x => x.SRNO == Obj.SRNO).FirstOrDefault();


                        ExistObj.BUSINESS_CENTER = Obj.BUSINESS_CENTER;
                        ExistObj.BUSINESS_PLACE = Obj.BUSINESS_PLACE;
                        ExistObj.PROFIT_CENTER = Obj.PROFIT_CENTER;
                        ExistObj.UPDATEDBY = loginCode;
                        ExistObj.UPDATEDON = DateTime.Now;
                        _DB.Entry(ExistObj).State = EntityState.Modified;
                        _DB.SaveChanges();
                        var LogInsert = InsertToNSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_LOG(ExistObj, "UPDATE");
                    }
                    Res.hasError = false;
                    Res.errorMessage = "Record Updated Successfully";
                }


            }
            catch (Exception ex)
            {
                Res.hasError = true;
                Res.errorMessage = ex.Message.ToString();
            }

            return Res;
        }

        private bool InsertToNSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_LOG(NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER NSP, string Mode)
        {
            try
            {
                //using (var db = new LCEntities())
                //{
                    NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_LOG NspLog = new NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_LOG();
                    var maxSrno = _DB.NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_LOG.DefaultIfEmpty().Max(x => x == null ? 0 : x.SRNO);
                    NspLog.SRNO = maxSrno + 1;
                    NspLog.MAINTBLSRNO = NSP.SRNO;
                    NspLog.BUSINESS_CENTER = NSP.BUSINESS_CENTER;
                    NspLog.BUSINESS_PLACE = NSP.BUSINESS_PLACE;
                    NspLog.PROFIT_CENTER = NSP.PROFIT_CENTER;
                    NspLog.ADDEDBY = NSP.ADDEDBY;
                    NspLog.ADDEDON = NSP.ADDEDON;
                    NspLog.UPDATEDBY = NSP.UPDATEDBY;
                    NspLog.UPDATEDON = NSP.UPDATEDON;
                    NspLog.ACTIVE = NSP.ACTIVE;
                    NspLog.LOGMODE = Mode;
                    _DB.NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_LOG.Add(NspLog);
                    _DB.SaveChanges();
                //}
            }
            catch (Exception ex)
            {

                return false;
            }

            return true;
        }

        #endregion

        #region GST Master
        public LibResult GetNSP_GST_TAX_MASTER_List_Data()
        {
            LibResult result = new LibResult();

            try
            {
                //using (var db = new LCEntities())
                //{
                    var data = _DB.NSP_GST_TAX_MASTER.ToList();
                    result.resultObject = data;
                    result.hasError = false;
                //}
            }
            catch (Exception ex)
            {
                result.hasError = true;
                result.errorMessage = ex.Message.ToString();
            }

            return result;
        }


        public LibResult GetNSP_GST_TAX_MASTER_Edit_Data(int Srno)
        {
            LibResult Res = new LibResult();

            try
            {

                var Data = (from i in _DB.NSP_GST_TAX_MASTER where i.SRNO == Srno select i).FirstOrDefault();

                Res.resultObject = Data;
                Res.hasError = false;


            }
            catch (Exception ex)
            {
                Res.hasError = true;
                Res.errorMessage = ex.Message.ToString();
            }

            return Res;
        }

        public LibResult DeActive_NSP_GST_TAX_MASTER_Record(int SrNo, int loginCode)
        {
            LibResult res = new LibResult();
            try
            {

                var DeleteRecord = _DB.NSP_GST_TAX_MASTER.Find(SrNo);
                short ActiveStatus = (DeleteRecord.ACTIVE == (short)1 ? (short)0 : (short)1);
                string SavedMsg = (DeleteRecord.ACTIVE == (short)1 ? "Record DeActived Successfully" : "Record Actived Successfully");
                string Status = (DeleteRecord.ACTIVE == (short)1 ? "DEACTIVED" : "ACTIVED");
                if (DeleteRecord != null)
                {
                    DeleteRecord.ACTIVE = ActiveStatus;
                    DeleteRecord.UPDATEDBY = loginCode;
                    DeleteRecord.UPDATEDON = DateTime.Now;
                    _DB.Entry(DeleteRecord).State = EntityState.Modified;

                    _DB.SaveChanges();
                    res.hasError = false;
                    res.errorMessage = SavedMsg;
                    var LogInsert = InsertToNSP_GST_TAX_MASTER_LOG(DeleteRecord, Status);
                }
                else
                {
                    res.hasError = true;
                    res.errorMessage = "Record not found";
                }
            }
            catch (Exception ex)
            {
                res.hasError = true;
                res.errorMessage = ex.Message.ToString();
            }

            return res;
        }
        public LibResult NSP_GST_TAX_MASTER_SaveData(NSP_GST_TAX_MASTER Obj, int loginCode, string Mode)
        {
            LibResult Res = new LibResult();
            try
            {
                if (Mode == "ADD")
                {
                    var Exist = _DB.NSP_GST_TAX_MASTER.Where(x => x.TYPE == Obj.TYPE
                && x.TAXTYPE == Obj.TAXTYPE && x.TAXCODE == Obj.TAXCODE
                && x.GLACC1 == Obj.GLACC1
                && x.GLACC2 == Obj.GLACC2 && x.PROFITCENTER == Obj.PROFITCENTER
                 && x.PAYABLE_GLACC1 == Obj.PAYABLE_GLACC1 && x.PAYABLE_GLACC2 == Obj.PAYABLE_GLACC2 && x.CESS_GLACC == Obj.CESS_GLACC && x.CESS_RATE == Obj.CESS_RATE
                && x.VATRATE1 == Obj.VATRATE1 && x.VATRATE2 == Obj.VATRATE2 && x.ACTIVE == 1).ToList().Count();

                    if (Exist > 0)
                    {
                        Res.hasError = true;
                        Res.errorMessage = "Record alereat exist!!";
                    }
                    else
                    {
                        var MaxSrno = _DB.NSP_GST_TAX_MASTER.Select(x => x.SRNO).DefaultIfEmpty().Max();

                        MaxSrno++;
                        Obj.SRNO = MaxSrno;
                        Obj.TYPE = Obj.TYPE;
                        Obj.TAXCODE = Obj.TAXCODE;
                        Obj.VATRATE1 = Obj.VATRATE1;
                        Obj.VATRATE2 = Obj.VATRATE2;
                        Obj.GLACC1 = Obj.GLACC1;
                        Obj.GLACC2 = Obj.GLACC2;
                        Obj.PROFITCENTER = Obj.PROFITCENTER;
                        Obj.TAXTYPE = Obj.TAXTYPE;
                        Obj.ISDEDUCTED = Obj.ISDEDUCTED;
                        Obj.PAYABLE_GLACC1 = Obj.PAYABLE_GLACC1;
                        Obj.PAYABLE_GLACC2 = Obj.PAYABLE_GLACC2;
                        Obj.CESS_RATE = Obj.CESS_RATE;
                        Obj.CESS_GLACC = Obj.CESS_GLACC;
                        Obj.ACTIVE = 1;
                        Obj.ADDEDBY = loginCode;
                        Obj.ADDEDON = DateTime.Now;
                        _DB.Entry(Obj).State = EntityState.Added;
                        _DB.SaveChanges();
                        var LogInsert = InsertToNSP_GST_TAX_MASTER_LOG(Obj, "ADD");
                    }
                    Res.hasError = false;
                    Res.errorMessage = "Record Inserted Successfully";
                }
                else
                {
                    var Exist = _DB.NSP_GST_TAX_MASTER.Where(x => x.TYPE == Obj.TYPE
                && x.TAXTYPE == Obj.TAXTYPE && x.TAXCODE == Obj.TAXCODE
                && x.GLACC1 == Obj.GLACC1
                && x.GLACC2 == Obj.GLACC2 && x.PROFITCENTER == Obj.PROFITCENTER
                && x.VATRATE1 == Obj.VATRATE1 && x.VATRATE2 == Obj.VATRATE2 && x.ACTIVE == 1).ToList().Count();

                    if (Exist > 1)
                    {
                        Res.hasError = true;
                        Res.errorMessage = "Record alereat exist!!";
                    }
                    else
                    {
                        var ExistObj = _DB.NSP_GST_TAX_MASTER.Where(x => x.SRNO == Obj.SRNO).FirstOrDefault();


                        ExistObj.TYPE = Obj.TYPE;
                        ExistObj.TAXCODE = Obj.TAXCODE;
                        ExistObj.VATRATE1 = Obj.VATRATE1;
                        ExistObj.VATRATE2 = Obj.VATRATE2;
                        ExistObj.GLACC1 = Obj.GLACC1;
                        ExistObj.GLACC2 = Obj.GLACC2;
                        ExistObj.PROFITCENTER = Obj.PROFITCENTER;
                        ExistObj.TAXTYPE = Obj.TAXTYPE;
                        ExistObj.UPDATEDBY = loginCode;
                        ExistObj.UPDATEDON = DateTime.Now;
                        ExistObj.ISDEDUCTED = Obj.ISDEDUCTED;
                        ExistObj.PAYABLE_GLACC1 = Obj.PAYABLE_GLACC1;
                        ExistObj.PAYABLE_GLACC2 = Obj.PAYABLE_GLACC1;
                        ExistObj.CESS_RATE = Obj.CESS_RATE;
                        ExistObj.CESS_GLACC = Obj.CESS_GLACC;
                        _DB.Entry(ExistObj).State = EntityState.Modified;
                        _DB.SaveChanges();
                        var LogInsert = InsertToNSP_GST_TAX_MASTER_LOG(ExistObj, "UPDATE");
                    }
                    Res.hasError = false;
                    Res.errorMessage = "Record Updated Successfully";
                }

            }
            catch (Exception ex)
            {
                Res.hasError = true;
                Res.errorMessage = ex.Message.ToString();
            }

            return Res;
        }

        private bool InsertToNSP_GST_TAX_MASTER_LOG(NSP_GST_TAX_MASTER NSP, string Mode)
        {
            try
            {
                //using (var db = new LCEntities())
                //{
                    NSP_GST_TAX_MASTER_LOG NspLog = new NSP_GST_TAX_MASTER_LOG();
                    var maxSrno = _DB.NSP_GST_TAX_MASTER_LOG.DefaultIfEmpty().Max(x => x == null ? 0 : x.SRNO);
                    NspLog.SRNO = maxSrno + 1;
                    NspLog.MAINTBLSRNO = NSP.SRNO;
                    NspLog.TYPE = NSP.TYPE;
                    NspLog.TAXCODE = NSP.TAXCODE;
                    NspLog.VATRATE1 = NSP.VATRATE1;
                    NspLog.VATRATE2 = NSP.VATRATE2;
                    NspLog.GLACC1 = NSP.GLACC1;
                    NspLog.GLACC2 = NSP.GLACC2;
                    NspLog.PROFITCENTER = NSP.PROFITCENTER;
                    NspLog.TAXTYPE = NSP.TAXTYPE;
                    NspLog.ISDEDUCTED = NSP.ISDEDUCTED;
                    NspLog.PAYABLE_GLACC1 = NSP.PAYABLE_GLACC1;
                    NspLog.CESS_RATE = NSP.CESS_RATE;
                    NspLog.CESS_GLACC = NSP.CESS_GLACC;
                    NspLog.PAYABLE_GLACC2 = NSP.PAYABLE_GLACC2;
                    NspLog.ADDEDBY = NSP.ADDEDBY;
                    NspLog.ADDEDON = NSP.ADDEDON;
                    NspLog.UPDATEDBY = NSP.UPDATEDBY;
                    NspLog.UPDATEDON = NSP.UPDATEDON;
                    NspLog.ACTIVE = NSP.ACTIVE;
                    NspLog.LOGMODE = Mode;
                    _DB.NSP_GST_TAX_MASTER_LOG.Add(NspLog);
                    _DB.SaveChanges();
                //}
            }
            catch (Exception ex)
            {

                return false;
            }

            return true;
        }
        #endregion

        #region Master Log View

        public async Task<List<ExpandoObject>> GetNSP_Master_Log_Data(int Srno, string TblName)
        {

            var dynamicList = new List<ExpandoObject>();

            try
            {
                if (TblName == "ROLE_MASTER")
                {
                    dynamicList = await GetNSP_Role_Master_List_Data_Log(Srno);
                }
                else if (TblName == "TDS_MASTER")
                {
                    dynamicList = await GetNSP_TDS_Master_List_Data_Log(Srno);
                }
                else if (TblName == "BUPRO_MASTER")
                {
                    dynamicList = await GetNSP_BusineessProfit_Master_List_Data_Log(Srno);
                }
                else if (TblName == "GST_MASTER")
                {
                    dynamicList = await GetNSP_GST_Master_List_Data_Log(Srno);
                }
                else
                {

                }


            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dynamicList;
        }


        private async Task<List<ExpandoObject>> GetNSP_Role_Master_List_Data_Log(int Srno)
        {

            var dynamicList = new List<ExpandoObject>();

            try
            {
                //using (var db = new LCEntities())
                //{
                    var data = await (from role in _DB.NSP_ROLE_MASTER_LOG
                                      where role.MAINTBLSRNO == Srno
                                      select new
                                      {
                                          role.SRNO,
                                          role.DEPARTMENT,
                                          role.SYSITEID,
                                          role.ECODE,
                                          role.SYSITTDESCRIPTION,
                                          role.SEQUENCE_NO,
                                          role.ADDEDBY,
                                          role.ADDEDON,
                                          role.UPDATEDBY,
                                          role.UPDATEDON,
                                          role.ACTIVE,
                                          role.MAINTBLSRNO,
                                          role.LOGMODE,
                                          role.POST,
                                          role.REVERSE
                                      }).OrderByDescending(x => x.SRNO).ToListAsync();

                    int srNo = 0;
                    foreach (var record in data)
                    {
                        dynamic expando = new ExpandoObject();
                        var expandoDict = expando as IDictionary<string, object>;
                        srNo++;


                        var empFullName = GetEmpFullName(record.ECODE);
                        var addedByFullName = record.ADDEDBY != null ? GetEmpFullName(record.ADDEDBY) : "";
                        var updatedByFullName = record.UPDATEDBY != null ? GetEmpFullName(record.UPDATEDBY) : "";

                        expandoDict["SrNo"] = srNo;
                        expandoDict["Emp. Code"] = record.ECODE;
                        expandoDict["Emp. Name"] = empFullName;
                        expandoDict["Department"] = record.DEPARTMENT;
                        expandoDict["Right"] = record.POST == "Y" ? "POST" : "REVERSE";
                        expandoDict["Location"] = record.SYSITTDESCRIPTION;
                        expandoDict["Mode"] = record.LOGMODE;
                        expandoDict["Status"] = record.ACTIVE == 1 ? "Active" : "DeActive";
                        expandoDict["Added By"] = record.ADDEDBY != null ? $"{record.ADDEDBY}-{addedByFullName}" : "";
                        expandoDict["Added On"] = record.ADDEDON?.ToString("dd-MMM-yyyy hh:mm:ss tt") ?? "";
                        expandoDict["Updated By"] = record.UPDATEDBY != null ? $"{record.UPDATEDBY}-{updatedByFullName}" : "";
                        expandoDict["Updated On"] = record.UPDATEDON?.ToString("dd-MMM-yyyy hh:mm:ss tt") ?? "";

                        dynamicList.Add(expando);
                    }


                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dynamicList;
        }

        private async Task<List<ExpandoObject>> GetNSP_TDS_Master_List_Data_Log(int Srno)
        {

            var dynamicList = new List<ExpandoObject>();

            try
            {
                //using (var db = new LCEntities())
                //{
                    var data = await (from role in _DB.NSP_TDS_TAX_TYPE_MASTER_LOG
                                      where role.MAINTBLSRNO == Srno
                                      select new
                                      {
                                          role.SRNO,
                                          role.TAX_TYPE,
                                          role.WITHHOLDING_TAX_NAME,
                                          role.RATE,
                                          role.GLACC,
                                          role.ADDEDBY,
                                          role.ADDEDON,
                                          role.UPDATEDBY,
                                          role.UPDATEDON,
                                          role.ACTIVE,
                                          role.MAINTBLSRNO,
                                          role.LOGMODE,
                                      }).OrderByDescending(x => x.SRNO).ToListAsync();

                    int srNo = 0;
                    foreach (var record in data)
                    {
                        dynamic expando = new ExpandoObject();
                        var expandoDict = expando as IDictionary<string, object>;
                        srNo++;

                        var addedByFullName = record.ADDEDBY != null ? GetEmpFullName(record.ADDEDBY) : "";
                        var updatedByFullName = record.UPDATEDBY != null ? GetEmpFullName(record.UPDATEDBY) : "";

                        expandoDict["SrNo"] = srNo;
                        expandoDict["Tax Type"] = record.TAX_TYPE;
                        expandoDict["Withholding Tax Name"] = record.WITHHOLDING_TAX_NAME;
                        expandoDict["Rate"] = record.RATE;
                        expandoDict["GL Acc."] = record.GLACC;
                        expandoDict["Mode"] = record.LOGMODE;
                        expandoDict["Status"] = record.ACTIVE == 1 ? "Active" : "DeActive";
                        expandoDict["Added By"] = record.ADDEDBY != null ? $"{record.ADDEDBY}-{addedByFullName}" : "";
                        expandoDict["Added On"] = record.ADDEDON?.ToString("dd-MMM-yyyy hh:mm:ss tt") ?? "";
                        expandoDict["Updated By"] = record.UPDATEDBY != null ? $"{record.UPDATEDBY}-{updatedByFullName}" : "";
                        expandoDict["Updated On"] = record.UPDATEDON?.ToString("dd-MMM-yyyy hh:mm:ss tt") ?? "";

                        dynamicList.Add(expando);
                    }


                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dynamicList;
        }

        private async Task<List<ExpandoObject>> GetNSP_BusineessProfit_Master_List_Data_Log(int Srno)
        {

            var dynamicList = new List<ExpandoObject>();

            try
            {
                //using (var db = new LCEntities())
                //{
                    var data = await (from role in _DB.NSP_FIN_BUSINESS_PROFIT_CENTER_MASTER_LOG
                                      where role.MAINTBLSRNO == Srno
                                      select new
                                      {
                                          role.SRNO,
                                          role.BUSINESS_CENTER,
                                          role.BUSINESS_PLACE,
                                          role.PROFIT_CENTER,
                                          role.ADDEDBY,
                                          role.ADDEDON,
                                          role.UPDATEDBY,
                                          role.UPDATEDON,
                                          role.ACTIVE,
                                          role.MAINTBLSRNO,
                                          role.LOGMODE,
                                      }).OrderByDescending(x => x.SRNO).ToListAsync();

                    int srNo = 0;
                    foreach (var record in data)
                    {
                        dynamic expando = new ExpandoObject();
                        var expandoDict = expando as IDictionary<string, object>;
                        srNo++;

                        var addedByFullName = record.ADDEDBY != null ? GetEmpFullName(record.ADDEDBY) : "";
                        var updatedByFullName = record.UPDATEDBY != null ? GetEmpFullName(record.UPDATEDBY) : "";

                        expandoDict["SrNo"] = srNo;
                        expandoDict["Business Center"] = record.BUSINESS_CENTER;
                        expandoDict["Business Place"] = record.BUSINESS_PLACE;
                        expandoDict["Profit Center"] = record.PROFIT_CENTER;
                        expandoDict["Mode"] = record.LOGMODE;
                        expandoDict["Status"] = record.ACTIVE == 1 ? "Active" : "DeActive";
                        expandoDict["Added By"] = record.ADDEDBY != null ? $"{record.ADDEDBY}-{addedByFullName}" : "";
                        expandoDict["Added On"] = record.ADDEDON?.ToString("dd-MMM-yyyy hh:mm:ss tt") ?? "";
                        expandoDict["Updated By"] = record.UPDATEDBY != null ? $"{record.UPDATEDBY}-{updatedByFullName}" : "";
                        expandoDict["Updated On"] = record.UPDATEDON?.ToString("dd-MMM-yyyy hh:mm:ss tt") ?? "";

                        dynamicList.Add(expando);
                    }


                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dynamicList;
        }

        private async Task<List<ExpandoObject>> GetNSP_GST_Master_List_Data_Log(int Srno)
        {

            var dynamicList = new List<ExpandoObject>();

            try
            {
                //using (var db = new LCEntities())
                //{
                    var data = await (from role in _DB.NSP_GST_TAX_MASTER_LOG
                                      where role.MAINTBLSRNO == Srno
                                      select new
                                      {
                                          role.SRNO,
                                          role.TYPE,
                                          role.TAXCODE,
                                          role.VATRATE1,
                                          role.VATRATE2,
                                          role.GLACC1,
                                          role.GLACC2,
                                          role.PROFITCENTER,
                                          role.TAXTYPE,
                                          role.ISDEDUCTED,
                                          role.PAYABLE_GLACC1,
                                          role.CESS_RATE,
                                          role.CESS_GLACC,
                                          role.PAYABLE_GLACC2,
                                          role.ADDEDBY,
                                          role.ADDEDON,
                                          role.UPDATEDBY,
                                          role.UPDATEDON,
                                          role.ACTIVE,
                                          role.MAINTBLSRNO,
                                          role.LOGMODE,
                                      }).OrderByDescending(x => x.SRNO).ToListAsync();

                    int srNo = 0;
                    foreach (var record in data)
                    {
                        dynamic expando = new ExpandoObject();
                        var expandoDict = expando as IDictionary<string, object>;
                        srNo++;

                        var addedByFullName = record.ADDEDBY != null ? GetEmpFullName(record.ADDEDBY) : "";
                        var updatedByFullName = record.UPDATEDBY != null ? GetEmpFullName(record.UPDATEDBY) : "";

                        expandoDict["SrNo"] = srNo;
                        expandoDict["Type"] = record.TYPE;
                        expandoDict["Tax Code"] = record.TAXCODE;
                        expandoDict["VATRATE1"] = record.VATRATE1 != null ? record.VATRATE1.ToString() : "";
                        expandoDict["VATRATE2"] = record.VATRATE2 != null ? record.VATRATE2.ToString() : "";
                        expandoDict["GL ACC.1"] = record.GLACC1 != null ? record.GLACC1.ToString() : "";
                        expandoDict["GL ACC.2"] = record.GLACC2 != null ? record.GLACC2.ToString() : "";
                        expandoDict["Profit Center"] = record.PROFITCENTER != null ? record.PROFITCENTER.ToString() : "";
                        expandoDict["TAXTYPE"] = record.TAXTYPE != null ? record.TAXTYPE.ToString() : "";
                        expandoDict["Is Deducted"] = record.ISDEDUCTED != null ? record.ISDEDUCTED.ToString() : "";
                        expandoDict["Payable GL Acc.1"] = record.PAYABLE_GLACC1 != null ? record.PAYABLE_GLACC1.ToString() : "";
                        expandoDict["Payable GL Acc.2"] = record.PAYABLE_GLACC2 != null ? record.PAYABLE_GLACC2.ToString() : "";
                        expandoDict["Cess Rate"] = record.CESS_RATE != null ? record.CESS_RATE.ToString() : "";
                        expandoDict["Cess GL Acc."] = record.CESS_GLACC != null ? record.CESS_GLACC.ToString() : "";
                        expandoDict["Mode"] = record.LOGMODE;
                        expandoDict["Status"] = record.ACTIVE == 1 ? "Active" : "DeActive";
                        expandoDict["Added By"] = record.ADDEDBY != null ? $"{record.ADDEDBY}-{addedByFullName}" : "";
                        expandoDict["Added On"] = record.ADDEDON?.ToString("dd-MMM-yyyy hh:mm:ss tt") ?? "";
                        expandoDict["Updated By"] = record.UPDATEDBY != null ? $"{record.UPDATEDBY}-{updatedByFullName}" : "";
                        expandoDict["Updated On"] = record.UPDATEDON?.ToString("dd-MMM-yyyy hh:mm:ss tt") ?? "";

                        dynamicList.Add(expando);
                    }


                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dynamicList;
        }

        private string GetEmpFullName(long? Ecode)
        {
            string Fullname = string.Empty;
            try
            {
                //using (var DB = new LCEntities())
                //{
                    var Emp = _DB.ADEMPLOYEE_LC.Where(x => x.ACTIVE == 1 && x.ADEMPCODE == Ecode).FirstOrDefault();

                    Fullname = Emp != null ? $"{Emp.FIRSTNAME} {Emp.LASTNAME}" : "";
                //}


            }
            catch (Exception ex)
            {

                throw ex;
            }

            return Fullname;
        }
        #endregion

        #endregion

    }

    #region list extension menthod

    public static class ListExtensions
    {
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(values);
            }
            return table;
        }

    }






    #endregion


}




