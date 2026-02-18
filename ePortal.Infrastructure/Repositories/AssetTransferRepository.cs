using ePortal.DomainClasses;
using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
//using Spire.Xls;
using System.Web;
using ePortal.Infrastructure.DbContexts;
using ePortal.Shared;


namespace ePortal.Infrastructure.Repositories
{
    public class AssetTransferRepository
    {
        //private LCEntities _atdb;
        private readonly LCModelDBContext _atdb;

        public AssetTransferRepository(LCModelDBContext dbContext)
        {
            _atdb = dbContext;
        }

        public Tuple<int, long, int, string> GetAssetCode(string AssetCode)
        {
            LibResult Res = new LibResult();
            try
            {

                //using (var db = new LCEntities())
                //{

                var Data = (from a in _atdb.AT_ASSET_TRANSFER_DETAIL
                            join h in _atdb.AT_ASSSET_TRANSFER_HEADER on a.TRAN_NO equals h.TRAN_NO
                            where a.ASSET_CODE.Contains(AssetCode) && a.FULLY_PARTIAL != "Partial"
                            && h.STATUS != "Rejected" && h.STATUS != "Cancelled by User" && h.STATUS != "Cancelled by Admin"
                            select a.ASSET_CODE).Count();


                var Requestcode = (from a in _atdb.AT_ASSET_TRANSFER_DETAIL
                                   where a.ASSET_CODE.Contains(AssetCode)
                                   select new
                                   {
                                       User = a.ADDEDBY,
                                       ReqNo = a.TRAN_NO

                                   }).FirstOrDefault();

                return Tuple.Create(Data, Requestcode.User, Requestcode.ReqNo, "");

                //}
            }
            catch (Exception ex)
            {
                return Tuple.Create(0, (long)0, 0, ex.ToString());


            }

        }

        #region  Capitalize Asset

        #region Approval Operation Mapping

        public List<AT_APPROVAL_OPERATION_MAPPING> ATOMGetDataList()
        {
            List<AT_APPROVAL_OPERATION_MAPPING> ilist = new List<AT_APPROVAL_OPERATION_MAPPING>();

            //ilist = _atdb.AT_APPROVAL_OPERATION_MAPPING.OrderBy(o => o.SRNO).ToList();
            ilist = _atdb.AT_APPROVAL_OPERATION_MAPPING
    .OrderBy(o => o.APPROVAL_TYPE)
    .ThenBy(o => o.OPERATION_ID)
    .ThenBy(o => o.DIVISION_ID)
    .ToList();

            return ilist;


        }

        public AT_APPROVAL_OPERATION_MAPPING ATOMEdit(int id)
        {

            try
            {
                var _obj = (from data in _atdb.AT_APPROVAL_OPERATION_MAPPING.Where(v => v.SRNO == id)
                            select data).FirstOrDefault();

                return _obj;

            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        public LibResult ATOMUpdate(int srno, string ApproveType, int Operation, int SYKIID, int Division, int EvpAuth, int LoginCode)
        {
            LibResult res = new LibResult();
            try
            {
                var sykiid = (from s in _atdb.SYKI_LC
                              where s.ACTIVE == 1
                              select s.SYKIID).FirstOrDefault();

                //using (var DB_ = new LCEntities())
                //{
                var Exist = _atdb.AT_APPROVAL_OPERATION_MAPPING.Where(x => x.APPROVAL_TYPE == ApproveType
                                          && x.SRNO == srno
                                         ).Count();

                AT_APPROVAL_OPERATION_MAPPING at_comman = _atdb.AT_APPROVAL_OPERATION_MAPPING.Find(srno);

                var Opscript = (from r in _atdb.VW_ASSOCIATELVLDETAILS_LC where r.OPERATIONID == Operation && r.ACTIVE == 1 && r.SYKI == sykiid select r.OPERATION).FirstOrDefault().ToString();
                var Divison1 = "";
                if (Division != 0)
                {
                    Divison1 = (from r in _atdb.VW_ASSOCIATELVLDETAILS_LC where r.DIVISIONID == Division && r.ACTIVE == 1 && r.SYKI == sykiid select r.DIVISION).FirstOrDefault().ToString();
                }
                at_comman.SYKIID = SYKIID;
                at_comman.OPERATION_ID = Operation;
                at_comman.OPERATION = Opscript;
                at_comman.DIVISION_ID = Division;
                at_comman.DIVISION = Divison1;
                at_comman.EVP_AUTHORITY = EvpAuth;
                at_comman.UPDATEDBY = LoginCode;
                at_comman.UPDATEDON = DateTime.Now;

                _atdb.Entry(at_comman).State = EntityState.Modified;
                _atdb.SaveChanges();


                res.hasError = false;
                //}

            }

            catch (Exception ex)
            {
                res.hasError = true;
                res.errorMessage = ex.ToString();

            }
            return res;
        }

        public LibResult ATOMAdd(string ApprovalType, int OperationID, int SYKIID, int DivisionID, int EvpAuth, int LoginCode)
        {
            LibResult res = new LibResult();
            try
            {
                var sykiid = (from s in _atdb.SYKI_LC
                              where s.ACTIVE == 1
                              select s.SYKIID).FirstOrDefault();

                //using (var db = new LCEntities())
                //{
                var Exists = _atdb.AT_APPROVAL_OPERATION_MAPPING.Where(x => x.APPROVAL_TYPE == ApprovalType
                                                    && x.OPERATION_ID == OperationID
                                                     && x.DIVISION_ID == DivisionID
                                                      && x.EVP_AUTHORITY == EvpAuth
                                                    && x.SYKIID == SYKIID
                                                   ).Count();


                if (Exists > 0)
                {
                    res.hasError = true;
                    res.errorMessage = "Record Already Exists";
                }
                else
                {
                    var Srno = _atdb.AT_APPROVAL_OPERATION_MAPPING.Select(x => x.SRNO).DefaultIfEmpty().Max();
                    var Opscript = (from r in _atdb.VW_ASSOCIATELVLDETAILS_LC where r.OPERATIONID == OperationID && r.ACTIVE == 1 && r.SYKI == sykiid select r.OPERATION).FirstOrDefault().ToString();
                    var Divison = "";
                    if (DivisionID != 0)
                    {
                        Divison = (from r in _atdb.VW_ASSOCIATELVLDETAILS_LC where r.DIVISIONID == DivisionID && r.ACTIVE == 1 && r.SYKI == sykiid select r.DIVISION).FirstOrDefault().ToString();
                    }
                    var KICODE = (from r in _atdb.SYKI_LC where r.SYKIID == SYKIID && r.ACTIVE == 1 select r.KICODE).FirstOrDefault().ToString();

                    AT_APPROVAL_OPERATION_MAPPING _At = new AT_APPROVAL_OPERATION_MAPPING();

                    _At.SRNO = Srno + 1;
                    _At.APPROVAL_TYPE = ApprovalType;
                    _At.OPERATION = Opscript;
                    _At.OPERATION_ID = OperationID;
                    if (Divison == "")
                    {
                        _At.DIVISION = " ";
                    }
                    else
                    {
                        _At.DIVISION = Divison;
                    }
                    if (DivisionID == 0)
                    {
                        _At.DIVISION_ID = 0;
                    }
                    else
                    {
                        _At.DIVISION_ID = DivisionID;
                    }
                    _At.EVP_AUTHORITY = EvpAuth;
                    _At.SYKI = KICODE;
                    _At.SYKIID = SYKIID;


                    _At.ADDBY = LoginCode;
                    _At.ADDON = DateTime.Now;

                    _atdb.Entry(_At).State = EntityState.Added;
                    _atdb.SaveChanges();
                    res.hasError = false;
                    res.errorMessage = "Record Inserted successfully";

                }

                //}
            }
            catch (Exception ex)
            {
                res.hasError = true;
                res.errorMessage = ex.ToString();

            }
            return res;
        }

        public LibResult GetOperationlist()
        {
            LibResult Res = new LibResult();
            try
            {
                var sykiid = (from s in _atdb.SYKI_LC
                              where s.ACTIVE == 1
                              select s.SYKIID).FirstOrDefault();

                //using (var db = new LCEntities())
                //{
                var Data = (from r in _atdb.VW_ASSOCIATELVLDETAILS_LC
                            where r.ACTIVE == 1 && r.SYKI == sykiid
                            select new
                            {
                                Text = r.OPERATION,
                                Value = r.OPERATIONID
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

        public LibResult GetDivisionlist(long op_Id)
        {
            LibResult Res = new LibResult();
            try
            {
                var sykiid = (from s in _atdb.SYKI_LC
                              where s.ACTIVE == 1
                              select s.SYKIID).FirstOrDefault();


                //using (var db = new LCEntities())
                //{
                var Data = (from r in _atdb.VW_ASSOCIATELVLDETAILS_LC
                            where r.ACTIVE == 1 && r.SYKI == sykiid && r.DIVISIONID != null && r.DIVISIONID != 0
                             && r.OPERATIONID == (op_Id == 0 ? r.OPERATIONID : op_Id)
                            select new
                            {
                                Text = r.DIVISION,
                                Value = r.DIVISIONID
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

        public LibResult GetEVPUser(long div_id)
        {
            LibResult Res = new LibResult();
            try
            {
                var sykiid = (from s in _atdb.SYKI_LC
                              where s.ACTIVE == 1
                              select s.SYKIID).FirstOrDefault();


                //using (var db = new LCEntities())
                //{
                var Data = (from r in _atdb.VW_ASSOCIATELVLDETAILS_LC
                            join name in _atdb.ADEMPLOYEE_LC on r.ADEMPCODE equals name.ADEMPCODE
                            where r.ACTIVE == 1 && r.SYKI == sykiid
                             && r.DIVISIONID == (div_id == 0 ? r.DIVISIONID : div_id)
                            select new
                            {
                                Text = name.FIRSTNAME + " " + name.LASTNAME,
                                Value = r.ADEMPCODE
                            }).Distinct().ToList();


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

        public LibResult GetSykilist()
        {
            LibResult Res = new LibResult();
            try
            {

                //using (var db = new LCEntities())
                //{
                var Data = (from r in _atdb.SYKI_LC
                            where r.ACTIVE == 1
                            select new
                            {
                                Text = r.KICODE,
                                Value = r.SYKIID
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

        public LibResult GetSykilistforatom()
        {
            LibResult Res = new LibResult();
            try
            {

                //using (var db = new LCEntities())
                //{
                var Data = (from r in _atdb.SYKI_LC
                                //where r.ACTIVE == 1
                            select new
                            {
                                Text = r.KICODE,
                                Value = r.SYKIID
                            }).Distinct()
                             .Where(item => !string.IsNullOrEmpty(item.Text))
                             .OrderByDescending(item => item.Value)
                             .Take(3).OrderBy(item => item.Value)
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


        public bool ATOMDelete(int id)
        {
            bool res = false;
            AT_APPROVAL_OPERATION_MAPPING AT = _atdb.AT_APPROVAL_OPERATION_MAPPING.Find(id);
            _atdb.AT_APPROVAL_OPERATION_MAPPING.Remove(AT);
            _atdb.SaveChanges();
            res = true;
            return res;
        }

        public LibResult ATOMSearch(string ApprovalType, int Operation, int Syki)
        {

            LibResult Res = new LibResult();
            try
            {

                //using (var db = new LCEntities())
                //{
                var Data = (from r in _atdb.AT_APPROVAL_OPERATION_MAPPING
                            where (r.APPROVAL_TYPE == ApprovalType || ApprovalType == "All")
                            && (r.OPERATION_ID == Operation || Operation == 0)
                            && (r.SYKIID == Syki || Syki == 0)
                            select r).OrderBy(o => o.APPROVAL_TYPE)
                            .ThenBy(o => o.OPERATION_ID)
                            .ThenBy(o => o.DIVISION_ID)
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

        #endregion

        #region capitalize Detail
        public LibResult Requestor_Detail(string AssetType, string Transferor, int LoginCode)
        {
            LibResult res = new LibResult();
            try
            {

                var sykiid = (from s in _atdb.SYKI_LC
                              where s.ACTIVE == 1
                              select s.SYKIID).FirstOrDefault();

                //long Tran = long.Parse(Transferor.ToString());

                var d = (from a in _atdb.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == LoginCode && x.ACTIVE == 1)
                         join vw in _atdb.VW_ASSOCIATELVLDETAILS_LC on a.ADEMPCODE equals vw.ADEMPCODE
                         where vw.SYKI == sykiid && vw.ACTIVE == 1
                         select new AT_AssetRequestorDetail
                         {
                             ASSET_TYPE = AssetType,
                             //Requested_By = a.FIRSTNAME + " " + a.LASTNAME + " - " + "[" + LoginCode + "]",
                             Requested_By = LoginCode + " - " + a.FIRSTNAME + " " + a.LASTNAME,
                             OPERATION = vw.OPERATION,
                             DIVISION = vw.DIVISION,
                             DEPARTMENT = vw.DEPARTMENT,
                             SECTION = vw.SECTION
                         }).FirstOrDefault();

                res.resultObject = d;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return res;
        }

        public LibResult GetTransferor(string AssetType, int LoginCode)
        {
            LibResult res = new LibResult();
            try
            {

                var sykiid = (from s in _atdb.SYKI_LC
                              where s.ACTIVE == 1
                              select s.SYKIID).FirstOrDefault();

                var d = (from a in _atdb.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == LoginCode && x.ACTIVE == 1)
                         select new
                         {
                             Empcode = a.ADEMPCODE,
                             EmpName = a.FIRSTNAME + "  " + a.LASTNAME
                         }).FirstOrDefault();

                res.resultObject = d;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return res;
        }

        public bool CapitalizedEditDelete(int id)
        {
            try
            {
                bool res = false;
                AT_ASSET_TRANSFER_DETAIL AT = _atdb.AT_ASSET_TRANSFER_DETAIL.Find(id);
                _atdb.AT_ASSET_TRANSFER_DETAIL.Remove(AT);
                _atdb.SaveChanges();
                res = true;
                return res;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        // START :: ADDED BY AUMENTO :: SR89802-CR5424
        //public LibResult SaveTransaction(AT_ASSSET_TRANSFER_HEADER hd, List<AT_ASSET_TRANSFER_DETAIL> dt, List<AT_APPROVAL_AUTHORITY> _at, string flag, string AssetType, int LoginCode)
        //{
        //    LibResult res = new LibResult();
        //    using (var db = new LCEntities())

        //            try
        //    {
        //        var maxtranNo = _atdb.AT_ASSSET_TRANSFER_HEADER.Select(x => x.SRNO).DefaultIfEmpty(0).Max();
        //        var tranNo = maxtranNo + 1;
        //        var total = (from s in dt select s.NET_BLOCK).Sum();
        //        if (total != 0)
        //        {
        //            hd.TOTAL = total;
        //        }
        //        else
        //        {
        //            hd.TOTAL = 0;
        //        }

        //        res = SaveHdTransaction(tranNo, hd, flag, AssetType, LoginCode);

        //        if (res.hasError == false)
        //        {
        //            res = SaveDtTransaction(tranNo, dt, LoginCode, AssetType);
        //        }

        //        res = SaveAtTransaction(tranNo, _at, LoginCode, flag);
        //        res.errorMessage = "Record Inserted successfully";
        //    }
        //    catch (Exception ex)
        //    {
        //        res.hasError = true;
        //        res.errorMessage = ex.ToString();

        //    }
        //    return res;
        //}

        public LibResult SaveTransaction(AT_ASSSET_TRANSFER_HEADER hd, List<AT_ASSET_TRANSFER_DETAIL> dt, List<AT_APPROVAL_AUTHORITY> _at, string flag, string AssetType, int LoginCode)
        {
            LibResult res = new LibResult();
            //using (var db = new LCEntities())
            //{
                using (var transaction = _atdb.Database.BeginTransaction())
                {
                    try
                    {

                        var maxtranNo = _atdb.AT_ASSSET_TRANSFER_HEADER.Select(x => x.SRNO).DefaultIfEmpty().Max();
                        var tranNo = maxtranNo + 1;


                        var total = (from s in dt select s.NET_BLOCK).Sum();
                        hd.TOTAL = total != 0 ? total : 0;


                        res = SaveHdTransaction(tranNo, hd, flag, AssetType, LoginCode, _atdb);
                        if (res.hasError) throw new Exception(res.errorMessage);


                        res = SaveDtTransaction(tranNo, dt, LoginCode, AssetType, _atdb);
                        if (res.hasError) throw new Exception(res.errorMessage);


                        res = SaveAtTransaction(tranNo, _at, LoginCode, flag, _atdb);
                        if (res.hasError) throw new Exception(res.errorMessage);


                        transaction.Commit();
                        res.hasError = false;
                        res.errorMessage = "Transaction saved successfully.";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        res.hasError = true;
                        res.errorMessage = "Transaction failed: " + ex.Message;
                    }
                }
            //}

            return res;
        }

        // START :: ADDED BY AUMENTO :: SR89802-CR5424

        // public LibResult SaveHdTransaction(int TranNo, AT_ASSSET_TRANSFER_HEADER hd, string flag, string AssetType, int LoginCode)  // COMMENTED BY AUMENTO :: SR89802-CR5424
        public LibResult SaveHdTransaction(int TranNo, AT_ASSSET_TRANSFER_HEADER hd, string flag, string AssetType, int LoginCode, LCModelDBContext NewDB)  //  ADDED BY AUMENTO :: SR89802-CR5424
        {
            LibResult res = new LibResult();
            var initiate = false;
            try
            {
                //using (var db = new LCEntities())  // COMMENTED BY AUMENTO :: SR89802-CR5424
                //{ // COMMENTED BY AUMENTO :: SR89802-CR5424
                //var Exists = db.AT_ASSSET_TRANSFER_HEADER.Where(x => x.TRAN_TYPE == hd.TRAN_TYPE  // COMMENTED BY AUMENTO :: SR89802-CR5424
                var Exists = NewDB.AT_ASSSET_TRANSFER_HEADER.Where(x => x.TRAN_TYPE == hd.TRAN_TYPE //  ADDED BY AUMENTO :: SR89802-CR5424
                                                      && x.TRAN_NO == hd.TRAN_NO
                                                        && x.TRANSFEROR == hd.TRANSFEROR
                                                        && x.APPROVAL_TYPE == hd.APPROVAL_TYPE
                                                        && x.TRANSFREE == hd.TRANSFREE
                                                        && x.TRAN_DATE == hd.TRAN_DATE
                                                        && x.ASSET_TYPE == AssetType
                                                        && x.TOTAL == hd.TOTAL
                                                       ).Count();
                if (Exists > 0)
                {
                    res.hasError = true;
                    res.errorMessage = "record alredy exist...";
                }
                else
                {

                    //var Srno = db.AT_ASSSET_TRANSFER_HEADER.Select(x => x.SRNO).DefaultIfEmpty(0).Max(); // COMMENTED BY AUMENTO :: SR89802-CR5424
                    var Srno = NewDB.AT_ASSSET_TRANSFER_HEADER.Select(x => x.SRNO).DefaultIfEmpty().Max(); //  ADDED BY AUMENTO :: SR89802-CR5424
                    AT_ASSSET_TRANSFER_HEADER _At = new AT_ASSSET_TRANSFER_HEADER();

                    _At.SRNO = Srno + 1;
                    _At.TRAN_TYPE = hd.TRAN_TYPE;
                    _At.TRAN_NO = TranNo;//hd.TRAN_NO;
                    _At.TRANSFEROR = hd.TRANSFEROR;
                    _At.APPROVAL_TYPE = hd.APPROVAL_TYPE;
                    _At.TRANSFREE = hd.TRANSFREE;
                    _At.TRAN_DATE = hd.TRAN_DATE;
                    _At.ASSET_TYPE = AssetType;
                    _At.TOTAL = hd.TOTAL;
                    _At.REQ_ECODE = LoginCode;
                    _At.STATUS = flag == "SUBMIT" ? "Pending At Taxation" : "Initiate";
                    _At.PDFPATH = "-";
                    _At.GETIN_DOC_PATH = "-";
                    _At.GETOUT_DOC_PATH = "-";
                    _At.NEW_LOCATION = hd.NEW_LOCATION;
                    _At.TRANSFREE_TYPE = hd.TRANSFREE_TYPE;
                    if (hd.REMARKS == null || hd.REMARKS == "")
                    {
                        _At.REMARKS = " ";
                    }
                    else
                    {
                        _At.REMARKS = hd.REMARKS;
                    }
                    _At.COMPANYDEFINE_AUTHORITY = hd.COMPANYDEFINE_AUTHORITY;
                    _At.DURATION = hd.DURATION == null ? "" : hd.DURATION;
                    //_At.GATEOUTSTAMP = LoginCode;

                    _At.ADDEDBY = LoginCode;
                    _At.ADDEDON = DateTime.Now;

                    //db.Entry(_At).State = EntityState.Added; // COMMENTED BY AUMENTO :: SR89802-CR5424
                    //db.SaveChanges(); // COMMENTED BY AUMENTO :: SR89802-CR5424
                    NewDB.Entry(_At).State = EntityState.Added;  //  ADDED BY AUMENTO :: SR89802-CR5424
                    NewDB.SaveChanges();  //  ADDED BY AUMENTO :: SR89802-CR5424
                    initiate = true;


                    res.hasError = false;
                    // }
                    if (initiate == true)
                    {
                        //LibResult rees = intiatlog(hd, LoginCode); // COMMENTED BY AUMENTO :: SR89802-CR5424
                        LibResult rees = intiatlog(hd, LoginCode, NewDB);  //  ADDED BY AUMENTO :: SR89802-CR5424
                    }
                }
            }
            catch (Exception ex)
            {
                res.hasError = true;
                res.errorMessage = ex.ToString();

            }
            return res;
        }
        // public LibResult intiatlog(AT_ASSSET_TRANSFER_HEADER hd, int LoginCode)   // COMMENTED BY AUMENTO :: SR89802-CR5424
        public LibResult intiatlog(AT_ASSSET_TRANSFER_HEADER hd, int LoginCode, 
            LCModelDBContext NewDB)  //  ADDED BY AUMENTO :: SR89802-CR5424
        {
            LibResult res = new LibResult();
            //using (var db = new LCEntities()) // COMMENTED BY AUMENTO :: SR89802-CR5424
            //{                                 // COMMENTED BY AUMENTO :: SR89802-CR5424

            //var Srno1 = db.AT_APPROVAL_AUTHORITY_LOG.Select(x => x.SRNO).DefaultIfEmpty(0).Max();  // COMMENTED BY AUMENTO :: SR89802-CR5424
            var Srno1 = NewDB.AT_APPROVAL_AUTHORITY_LOG.Select(x => x.SRNO).DefaultIfEmpty().Max(); //  ADDED BY AUMENTO :: SR89802-CR5424
            AT_APPROVAL_AUTHORITY_LOG _AT = new AT_APPROVAL_AUTHORITY_LOG();

            var sykiid = (from s in _atdb.SYKI_LC
                          where s.ACTIVE == 1
                          select s.SYKIID).FirstOrDefault();
            // START ::  ADDED BY AUMENTO :: SR89802-CR5424
            //var designation = (from emp1 in _atdb.ADEMPLOYEE_LC.Where(v => v.ADEMPCODE == LoginCode && v.ACTIVE == 1)
            //                   join _VW in _atdb.VW_ASSOCIATELVLDETAILS_LC on emp1.ADEMPCODE equals _VW.ADEMPCODE
            //                   join _Desg in _atdb.ADDESIGNATION_LC on _VW.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
            //                   where _VW.SYKI == sykiid
            //                   select (_Desg.DESCRIP)).FirstOrDefault();

            var designation = (from emp1 in NewDB.ADEMPLOYEE_LC.Where(v => v.ADEMPCODE == LoginCode && v.ACTIVE == 1)
                               join _VW in NewDB.VW_ASSOCIATELVLDETAILS_LC on emp1.ADEMPCODE equals _VW.ADEMPCODE
                               join _Desg in NewDB.ADDESIGNATION_LC on _VW.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
                               where _VW.SYKI == sykiid
                               select (_Desg.DESCRIP)).FirstOrDefault();
            // END ::  ADDED BY AUMENTO :: SR89802-CR5424


            //var getheader = (from vw in _atdb.VW_ASSOCIATELVLDETAILS_LC // COMMENTED BY AUMENTO :: SR89802-CR5424
            var getheader = (from vw in NewDB.VW_ASSOCIATELVLDETAILS_LC   //  ADDED BY AUMENTO :: SR89802-CR5424
                             where vw.ADEMPCODE == LoginCode && vw.ACTIVE == 1 && vw.SYKI == sykiid
                             select new
                             {
                                 fd = vw.FUNCTIONALDESIGNATION,
                                 Section = vw.SECTION,
                                 Department = vw.DEPARTMENT,
                                 Division = vw.DIVISION,
                                 Operation = vw.OPERATION
                             }).FirstOrDefault();

            var concatenatedString = "";
            if (getheader != null)
            {
                if (getheader.fd == "Section Head" || getheader.fd == null)
                {
                    if (getheader.Section == null)
                    {
                        concatenatedString = getheader.Department;
                    }
                    else
                    {
                        concatenatedString = getheader.Section;
                    }
                }

                else if (getheader.fd == "Department Head" || getheader.fd == null)
                {
                    if (getheader.Department == null)
                    {
                        concatenatedString = getheader.Division;
                    }
                    else
                    {
                        concatenatedString = getheader.Department;
                    }

                }
                else if (getheader.fd == "Division Head" || getheader.fd == null)
                {
                    if (getheader.Division == null)
                    {
                        concatenatedString = getheader.Operation;
                    }
                    else
                    {
                        concatenatedString = getheader.Division;
                    }

                }
                else if (getheader.fd == "Operating Head" || getheader.fd == null)
                {
                    concatenatedString = getheader.Operation;
                }

            }

            _AT.SRNO = Srno1 + 1;
            _AT.TRAN_NO = hd.TRAN_NO;
            _AT.EMP_CODE = LoginCode;
            _AT.APPROVAL_STATUS = "Initiate";
            _AT.EMP_NAME = _atdb.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == LoginCode).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").FirstOrDefault();
            // _AT.DEPARTMENT = _atdb.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == LoginCode).Select(s => s.DEPARTMENT).FirstOrDefault();
            _AT.DESIGNATION = designation.ToString();
            _AT.DEPARTMENT = concatenatedString;
            _AT.REMARKS = !string.IsNullOrEmpty(hd.REMARKS) ? hd.REMARKS : " ";
            _AT.ADDEDBY = LoginCode;
            _AT.ADDEDON = DateTime.Now;
            _AT.UPDATEDBY = LoginCode;
            _AT.UPDATEDON = DateTime.Now;


            //db.Entry(_AT).State = EntityState.Added; // COMMENTED BY AUMENTO :: SR89802-CR5424
            //db.SaveChanges();                        // COMMENTED BY AUMENTO :: SR89802-CR5424
            NewDB.Entry(_AT).State = EntityState.Added;  //  ADDED BY AUMENTO :: SR89802-CR5424
            NewDB.SaveChanges();                        //  ADDED BY AUMENTO :: SR89802-CR5424
            return res;
            // }// COMMENTED BY AUMENTO :: SR89802-CR5424
        }

        //public LibResult SaveDtTransaction(int TranNo, List<AT_ASSET_TRANSFER_DETAIL> dt, int LoginCode,string AssetType) // COMMENTED BY AUMENTO :: SR89802-CR5424
        public LibResult SaveDtTransaction(int TranNo, List<AT_ASSET_TRANSFER_DETAIL> dt, int LoginCode, string AssetType, LCModelDBContext NewDB)  //  ADDED BY AUMENTO :: SR89802-CR5424
        {
            LibResult res = new LibResult();
            try
            {
                foreach (var d in dt)
                {
                    var newfilename = "";
                    if (d.ASSET_ATTACH != "")
                    {
                        if (AssetType == "Capitalized Asset")
                        {
                            var filename = d.ASSET_ATTACH.Split('\\')[2];
                            newfilename = "ASSET_UPLOAD_" + d.ASSET_CODE + "_" + d.TRAN_NO + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + filename;

                        }
                        else
                        {
                            var filename = d.ASSET_ATTACH.Split('\\')[2];
                            newfilename = "ASSET_UPLOAD_" + d.SUBAUCCODE + "_" + d.TRAN_NO + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + filename;
                        }
                    }
                    //using (var _DB = new LCEntities()) // COMMENTED BY AUMENTO :: SR89802-CR5424
                    //{ // COMMENTED BY AUMENTO :: SR89802-CR5424

                    //var Exist = _DB.AT_ASSET_TRANSFER_DETAIL.Where(x => x.TRAN_NO == d.TRAN_NO  // COMMENTED BY AUMENTO :: SR89802-CR5424
                    var Exist = NewDB.AT_ASSET_TRANSFER_DETAIL.Where(x => x.TRAN_NO == d.TRAN_NO  // ADDED BY AUMENTO :: SR89802-CR5424
                                                     && x.ASSET_CODE == d.ASSET_CODE
                                                      && x.PO_NO == d.PO_NO && x.PO_DATE == d.PO_DATE
                                                      && x.INV_NO == d.INV_NO && x.INV_DATE == d.INV_DATE
                                                      && x.FULLY_PARTIAL == d.FULLY_PARTIAL
                                                      && x.QTY == d.QTY && x.CURRENT_LOCATION == d.CURRENT_LOCATION
                                                      && x.NEW_LOCATION == d.NEW_LOCATION
                                                      && x.ORIGINAL_COST == d.ORIGINAL_COST
                                                      && x.NET_BLOCK == d.NET_BLOCK
                                                      && x.DEPRECIATION == d.DEPRECIATION
                                                      && x.VENDER_NAME == d.VENDER_NAME
                                                      && x.VENDOR_CODE == d.VENDOR_CODE
                                                      && x.LICENSE_NO == d.LICENSE_NO
                                                      && x.LICENSE_DATE == d.LICENSE_DATE
                                                      && x.HSN_CODE == d.HSN_CODE
                                                      && x.BASIC_PRICE == d.BASIC_PRICE
                                                      && x.INVOICE_VALUE == d.INVOICE_VALUE
                                                     ).Count();

                    if (Exist > 0)
                    {
                        res.hasError = true;
                        res.errorMessage = "record alredy exist...";
                    }
                    else
                    {
                        //var srno = _DB.AT_ASSET_TRANSFER_DETAIL.Select(x => x.SRNO).DefaultIfEmpty(0).Max(); // COMMENTED BY AUMENTO :: SR89802-CR5424
                        var srno = NewDB.AT_ASSET_TRANSFER_DETAIL.Select(x => x.SRNO).DefaultIfEmpty().Max();   // ADDED BY AUMENTO :: SR89802-CR5424
                        AT_ASSET_TRANSFER_DETAIL At_Detail = new AT_ASSET_TRANSFER_DETAIL();

                        At_Detail.SRNO = srno + 1;
                        At_Detail.TRAN_NO = TranNo;
                        At_Detail.ASSET_CODE = d.ASSET_CODE;
                        At_Detail.PO_NO = d.PO_NO == null ? " " : d.PO_NO;
                        At_Detail.PO_DATE = d.PO_DATE == DateTime.MinValue ? null : d.PO_DATE;
                        At_Detail.FULLY_PARTIAL = d.FULLY_PARTIAL;
                        At_Detail.QTY = d.QTY;
                        At_Detail.CURRENT_LOCATION = d.CURRENT_LOCATION;
                        At_Detail.NEW_LOCATION = d.NEW_LOCATION;
                        At_Detail.ORIGINAL_COST = d.ORIGINAL_COST;
                        At_Detail.DEPRECIATION = d.DEPRECIATION;
                        At_Detail.NET_BLOCK = d.NET_BLOCK;
                        if (d.INV_NO == null)
                        {
                            At_Detail.INV_NO = " ";
                        }
                        else
                        {
                            At_Detail.INV_NO = d.INV_NO;
                        }
                        At_Detail.INV_DATE = d.INV_DATE == DateTime.MinValue ? null : d.INV_DATE;
                        if (d.ASSET_SERIAL_NO == null)
                        {
                            At_Detail.ASSET_SERIAL_NO = "";

                        }
                        else
                        {
                            At_Detail.ASSET_SERIAL_NO = d.ASSET_SERIAL_NO;
                        }
                        At_Detail.ASSET_CLASS = d.ASSET_CLASS;
                        At_Detail.CAPITALIZED_DATE = d.CAPITALIZED_DATE;
                        if (d.VENDER_NAME == null)
                        {
                            At_Detail.VENDER_NAME = " ";
                        }
                        else
                        {
                            At_Detail.VENDER_NAME = d.VENDER_NAME;
                        }
                        At_Detail.VENDOR_CODE = d.VENDOR_CODE == null ? "" : d.VENDOR_CODE;
                        if (d.ASSETMAIN_NO_TEXT == null)
                        {
                            At_Detail.ASSETMAIN_NO_TEXT = " ";
                        }
                        else
                        {
                            At_Detail.ASSETMAIN_NO_TEXT = d.ASSETMAIN_NO_TEXT;
                        }
                        if (d.LICENSE_NO == null)
                        {
                            At_Detail.LICENSE_NO = " ";
                        }
                        else
                        {
                            At_Detail.LICENSE_NO = d.LICENSE_NO;
                        }
                        At_Detail.LICENSE_DATE = d.LICENSE_DATE;

                        At_Detail.HSN_CODE = d.HSN_CODE;
                        At_Detail.DESCRIPTION = d.DESCRIPTION;
                        At_Detail.NEW_ASSET_CODE = "";
                        if (d.ASSET_ATTACH == null)
                        {
                            At_Detail.ASSET_ATTACH = "-";
                        }
                        else
                        {
                            At_Detail.ASSET_ATTACH = newfilename;
                        }
                        if (d.BASIC_PRICE == 0)
                        {
                            At_Detail.BASIC_PRICE = 0;

                        }
                        else
                        {
                            At_Detail.BASIC_PRICE = d.BASIC_PRICE;
                        }
                        At_Detail.INVOICE_VALUE = d.INVOICE_VALUE == 0 ? 0 : d.INVOICE_VALUE;
                        At_Detail.ASSET_CLS_DESC = d.ASSET_CLS_DESC == null ? "" : d.ASSET_CLS_DESC;
                        At_Detail.SUBAUCCODE = d.SUBAUCCODE == null ? "" : d.SUBAUCCODE;
                        At_Detail.EODC_CLEAREANSE = d.EODC_CLEAREANSE == null ? "" : d.EODC_CLEAREANSE;

                        At_Detail.ADDEDBY = LoginCode;
                        At_Detail.ADDEDON = DateTime.Now;

                        //_DB.Entry(At_Detail).State = EntityState.Added; // COMMENTED BY AUMENTO :: SR89802-CR5424
                        //_DB.SaveChanges(); // COMMENTED BY AUMENTO :: SR89802-CR5424
                        NewDB.Entry(At_Detail).State = EntityState.Added;  // ADDED BY AUMENTO :: SR89802-CR5424
                        NewDB.SaveChanges();  // ADDED BY AUMENTO :: SR89802-CR5424
                    }


                }
                // } // COMMENTED BY AUMENTO :: SR89802-CR5424
                res.hasError = false;

            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        // Log or inspect validation errors
                        Console.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                    }
                }
            }
            return res;
        }

        //public LibResult SaveAtTransaction(int TranNo, List<AT_APPROVAL_AUTHORITY> at, int LoginCode, string flag) // COMMENTED BY AUMENTO :: SR89802-CR5424
        public LibResult SaveAtTransaction(int TranNo, List<AT_APPROVAL_AUTHORITY> at, int LoginCode, string flag, LCModelDBContext NewDB)  //  ADDED BY AUMENTO :: SR89802-CR5424
        {
            LibResult res = new LibResult();
            try
            {
                var _hd = _atdb.AT_ASSSET_TRANSFER_HEADER.Where(x => x.TRAN_NO == TranNo).FirstOrDefault();
                var Status = "Approved";
                foreach (var d in at)
                {
                    //using (var DB_ = new LCEntities()) // COMMENTED BY AUMENTO :: SR89802-CR5424
                    //{ // COMMENTED BY AUMENTO :: SR89802-CR5424
                    //var Exist = DB_.AT_APPROVAL_AUTHORITY.Where(x => x.TRAN_NO == d.TRAN_NO  // COMMENTED BY AUMENTO :: SR89802-CR5424
                    var Exist = NewDB.AT_APPROVAL_AUTHORITY.Where(x => x.TRAN_NO == d.TRAN_NO //  ADDED BY AUMENTO :: SR89802-CR5424
                                                 && x.EMP_CODE == d.EMP_CODE
                                                  && x.APPROVAL_STATUS == d.APPROVAL_STATUS
                                                  && x.EMP_NAME == d.EMP_NAME
                                                  && x.DEPARTMENT == d.DEPARTMENT
                                                  && x.DESIGNATION == d.DESIGNATION
                                                 ).Count();
                    if (Exist > 0)
                    {
                        res.hasError = true;
                        res.errorMessage = "record alredy exist...";
                    }
                    else
                    {
                        //var maxSrno = DB_.AT_APPROVAL_AUTHORITY.Select(x => (int?)x.SRNO).Max() ?? 0; // COMMENTED BY AUMENTO :: SR89802-CR5424
                        var maxSrno = NewDB.AT_APPROVAL_AUTHORITY.Select(x => (int?)x.SRNO).Max() ?? 0; //  ADDED BY AUMENTO :: SR89802-CR5424

                        AT_APPROVAL_AUTHORITY At_authority = new AT_APPROVAL_AUTHORITY
                        {
                            SRNO = maxSrno + 1,
                            TRAN_NO = TranNo,
                            EMP_CODE = d.EMP_CODE,
                            APPROVAL_STATUS = d.APPROVAL_STATUS,
                            EMP_NAME = d.EMP_NAME,
                            DEPARTMENT = d.DEPARTMENT,
                            DESIGNATION = d.DESIGNATION,
                            REMARKS = " ",
                            ADDEDBY = LoginCode,
                            ADDEDON = DateTime.Now
                        };


                        var min = at.Where(x => x.DEPARTMENT == "Taxation" && x.TRAN_NO == d.TRAN_NO).Select(x => (int?)x.SRNO).Min();
                        if (d.DEPARTMENT == "Taxation" && d.SRNO == min)
                        {
                            At_authority.APPROVAL_STATUS = flag == "SUBMIT" ? "Pending" : "-";
                            if (flag == "SUBMIT")
                            {
                                SendMailByApprovalAuthority(_hd, At_authority.EMP_CODE, Status);
                            }
                        }
                        else
                        {
                            At_authority.APPROVAL_STATUS = "-";
                        }

                        //SendMailByApprovalAuthority(hd_, appauth1.EMP_CODE, Status);

                        //DB_.AT_APPROVAL_AUTHORITY.Add(At_authority); // COMMENTED BY AUMENTO :: SR89802-CR5424
                        //DB_.SaveChanges(); // COMMENTED BY AUMENTO :: SR89802-CR5424
                        NewDB.AT_APPROVAL_AUTHORITY.Add(At_authority); //  ADDED BY AUMENTO :: SR89802-CR5424
                        NewDB.SaveChanges(); //  ADDED BY AUMENTO :: SR89802-CR5424


                    }
                    //} // COMMENTED BY AUMENTO :: SR89802-CR5424

                    res.hasError = false;

                }
            }
            catch (Exception ex)
            {
                res.hasError = true;
                res.errorMessage = ex.ToString();

            }
            return res;
        }

        public Employee_Details GetAuthEmpById(int empCode, Employee_Details empDtl, int TranNo, string Department)
        {
            var sykiid = (from s in _atdb.SYKI_LC
                          where s.ACTIVE == 1
                          select s.SYKIID).FirstOrDefault();

            var _obj = (from data in _atdb.ADEMPLOYEE_LC.Where(v => v.ADEMPCODE == empCode && v.ACTIVE == 1)
                        join _VW in _atdb.VW_ASSOCIATELVLDETAILS_LC on data.ADEMPCODE equals _VW.ADEMPCODE
                        join _Desg in _atdb.ADDESIGNATION_LC on _VW.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
                        where _VW.SYKI == sykiid
                        select new Employee_Details
                        {
                            _ECode = data.ADEMPCODE,
                            _EFirstName = data.FIRSTNAME,
                            _ELastName = data.LASTNAME,
                            _EName = data.FIRSTNAME + " " + data.LASTNAME,
                            _Desig = _VW.FUNCTIONALDESIGNATION == null ? _Desg.DESCRIP : _VW.FUNCTIONALDESIGNATION,
                            _DepDesc = _VW.DEPARTMENT

                        }).FirstOrDefault();

            if (Department == "Finance")
            {
                var desc = (from vw in _atdb.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == empCode && x.ACTIVE == 1)
                            join des in _atdb.ADDESIGNATION_LC on vw.ADDESIGNATIONID equals des.ADDESIGNATIONID
                            where vw.SYKI == sykiid
                            select des.DESCRIP).FirstOrDefault();

                _obj._Desig = desc;
            }

            return _obj;
        }

        public LibResult CapitalizeListDelete(int id, string flag)
        {
            LibResult res = new LibResult();
            var delete = _atdb.AT_ASSSET_TRANSFER_HEADER.Where(i => i.TRAN_NO == id).FirstOrDefault();

            if (flag == "CapitalizedList" || flag == "NonCapitalizeList")
            {
                delete.STATUS = "Cancelled by User";
                _atdb.Entry(delete).State = EntityState.Modified;
                _atdb.SaveChanges();
            }
            else
            {
                delete.STATUS = "Cancelled by Admin";
                _atdb.Entry(delete).State = EntityState.Modified;
                _atdb.SaveChanges();
            }

            res.resultObject = delete;

            return res;
        }


        public List<AT_AssetTransferHeaderViewModel> GetCapitalizeList(int LoginCode, string flag)
        {

            List<AT_AssetTransferHeaderViewModel> ilist = new List<AT_AssetTransferHeaderViewModel>();

            var value = _atdb.AT_ASSSET_TRANSFER_HEADER.Where(x => x.REQ_ECODE == LoginCode).ToList();

            foreach (var d in value)
            {
                if (d.STATUS == "Initiate")
                {
                    ilist = (from data in _atdb.AT_ASSSET_TRANSFER_HEADER.Where(x => x.REQ_ECODE == LoginCode)
                                 //join a in _atdb.AT_APPROVAL_AUTHORITY on data.TRAN_NO equals a.TRAN_NO
                             where data.STATUS == "Initiate" && data.ASSET_TYPE == flag

                             select new AT_AssetTransferHeaderViewModel
                             {
                                 SRNO = data.SRNO,
                                 TRAN_NO = data.TRAN_NO,
                                 TRAN_TYPE = data.TRAN_TYPE,
                                 ASSET_TYPE = data.ASSET_TYPE,
                                 TRAN_DATE = data.TRAN_DATE,
                                 TRANSFEROR = data.TRANSFEROR,
                                 TRANSFREE = data.TRANSFREE,
                                 TOTAL = data.TOTAL,
                                 APPAUTHSRNO = _atdb.AT_APPROVAL_AUTHORITY.Where(x => x.TRAN_NO == data.TRAN_NO && x.APPROVAL_STATUS == "-").Select(y => y.SRNO).FirstOrDefault(),
                                 TRANSFREE_TYPE = data.TRANSFREE_TYPE,
                                 STATUS = data.STATUS,
                                 PDFPATH = data.PDFPATH,
                                 GETOUT_DOC_PATH = data.GETOUT_DOC_PATH,
                                 GETIN_DOC_PATH = data.GETIN_DOC_PATH,
                                 DURATION = data.DURATION

                             }).OrderBy(x => x.TRAN_NO).ToList();

                    foreach (var i in ilist)
                    {
                        if (i.TRANSFREE_TYPE == "Vendor")
                        {
                            i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                            i.TRANSFREE = i.TRANSFREE + "-" + GetVendorName(i.TRANSFREE);
                        }
                        else
                        {
                            i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                            i.TRANSFREE = i.TRANSFREE + "-" + GetEmpName(int.Parse(i.TRANSFREE));
                        }

                    }

                }
            }

            return ilist;

        }

        public List<AT_AssetTransferHeaderViewModel> CapitalizeListSearch(string TransactionType, /*string AssetType,*/ int TransactionNo, string TransfereeUser, string Status, int LoginCode, string flag)
        {

            try
            {
                #region MyRegion
                List<AT_AssetTransferHeaderViewModel> Data = new List<AT_AssetTransferHeaderViewModel>();

                if (Status == "Initiate" || Status == "Completed" || Status == "Rejected" || Status == "Cancelled by User" || Status == "Cancelled by Admin")
                {
                    Data = (from r in _atdb.AT_ASSSET_TRANSFER_HEADER
                            //where (r.GATEOUTSTAMP == null ? r.REQ_ECODE == LoginCode : r.GATEOUTSTAMP == LoginCode || r.GATEOUTSTAMP == LoginCode)
                            where (r.GATEOUTSTAMP == LoginCode || (r.GATEOUTSTAMP == null && r.REQ_ECODE == LoginCode))
                             && (r.ASSET_TYPE == flag)
                           && (r.TRAN_NO == TransactionNo || TransactionNo == 0)
                            && (r.TRANSFREE == TransfereeUser || TransfereeUser == "0")
                            && (r.TRAN_TYPE == TransactionType || TransactionType == "All")
                            && (r.STATUS == Status)


                            select new AT_AssetTransferHeaderViewModel
                            {
                                SRNO = r.SRNO,
                                TRAN_NO = r.TRAN_NO,
                                TRAN_DATE = r.TRAN_DATE,
                                TRAN_TYPE = r.TRAN_TYPE,
                                ASSET_TYPE = r.ASSET_TYPE,
                                TRANSFEROR = r.TRANSFEROR,
                                TRANSFREE = r.TRANSFREE,
                                APPAUTHSRNO = _atdb.AT_APPROVAL_AUTHORITY.Where(x => x.TRAN_NO == r.TRAN_NO && x.APPROVAL_STATUS == "Pending").Select(y => y.SRNO).FirstOrDefault(),
                                TOTAL = r.TOTAL,
                                STATUS = r.STATUS,
                                PDFPATH = r.PDFPATH,
                                GETOUT_DOC_PATH = r.GETOUT_DOC_PATH,
                                GETIN_DOC_PATH = r.GETIN_DOC_PATH,
                                DURATION = r.DURATION
                            }).OrderBy(O => O.TRAN_NO).ToList();

                }
                else
                {
                    Data = (from r in _atdb.AT_ASSSET_TRANSFER_HEADER
                            //where (r.GATEOUTSTAMP == null ? r.REQ_ECODE == LoginCode : r.GATEOUTSTAMP == LoginCode || r.GATEOUTSTAMP == LoginCode)
                            where (r.GATEOUTSTAMP == LoginCode || (r.GATEOUTSTAMP == null && r.REQ_ECODE == LoginCode))
                             && (r.ASSET_TYPE == flag)
                           && (r.TRAN_NO == TransactionNo || TransactionNo == 0)
                            && (r.TRANSFREE == TransfereeUser || TransfereeUser == "0")
                            && (r.TRAN_TYPE == TransactionType || TransactionType == "All")
                            && (r.STATUS == "Pending At Taxation" || r.STATUS == "Pending At Approve" || r.STATUS == "Pending At Finance" || r.STATUS == "Pending At GateOutStamp" || r.STATUS == "Pending At Security" || r.STATUS == "UpdateNewAssetCode" || r.STATUS == "Pending At GateInStamp" || r.STATUS == "Pending At Finance TM")

                            select new AT_AssetTransferHeaderViewModel
                            {
                                SRNO = r.SRNO,
                                TRAN_NO = r.TRAN_NO,
                                TRAN_DATE = r.TRAN_DATE,
                                TRAN_TYPE = r.TRAN_TYPE,
                                ASSET_TYPE = r.ASSET_TYPE,
                                TRANSFEROR = r.TRANSFEROR,
                                TRANSFREE = r.TRANSFREE,
                                APPAUTHSRNO = _atdb.AT_APPROVAL_AUTHORITY.Where(x => x.TRAN_NO == r.TRAN_NO && x.APPROVAL_STATUS == "Pending").Select(y => y.SRNO).FirstOrDefault(),
                                TOTAL = r.TOTAL,
                                STATUS = r.STATUS,
                                PDFPATH = r.PDFPATH,
                                TRANSFREE_TYPE = r.TRANSFREE_TYPE,
                                GETOUT_DOC_PATH = r.GETOUT_DOC_PATH,
                                GETIN_DOC_PATH = r.GETIN_DOC_PATH,
                                DURATION = r.DURATION

                            }).OrderBy(x => x.TRAN_NO).ToList();


                }
                foreach (var i in Data)
                {
                    if (i.TRANSFREE_TYPE == "Vendor")
                    {
                        i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                        i.TRANSFREE = i.TRANSFREE + "-" + GetVendorName(i.TRANSFREE);
                    }
                    else
                    {
                        i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                        i.TRANSFREE = i.TRANSFREE + "-" + GetEmpName(int.Parse(i.TRANSFREE));
                    }
                }
                return Data;


            }
            catch (Exception ex)
            {
                throw ex;

            }


            #endregion


        }

        public List<AT_AssetTransferHeaderViewModel> CapitalizeStatusSearch(string Status, int LoginCode, string flag)
        {
            try
            {
                List<AT_AssetTransferHeaderViewModel> Data = new List<AT_AssetTransferHeaderViewModel>();

                if (Status == "Initiate" || Status == "Completed" || Status == "Rejected" || Status == "Cancelled by User" || Status == "Cancelled by Admin")
                {
                    Data = (from r in _atdb.AT_ASSSET_TRANSFER_HEADER
                            //where (r.GATEOUTSTAMP == null ? r.REQ_ECODE == LoginCode : r.GATEOUTSTAMP == LoginCode || r.GATEOUTSTAMP == LoginCode)
                            where (r.GATEOUTSTAMP == LoginCode || (r.GATEOUTSTAMP == null && r.REQ_ECODE == LoginCode))
                          && (r.STATUS == Status)
                          && r.ASSET_TYPE == flag

                            select new AT_AssetTransferHeaderViewModel
                            {
                                APPROVAL_TYPE = r.APPROVAL_TYPE,
                                SRNO = r.SRNO,
                                TRAN_NO = r.TRAN_NO,
                                TRAN_DATE = r.TRAN_DATE,
                                TRAN_TYPE = r.TRAN_TYPE,
                                ASSET_TYPE = r.ASSET_TYPE,
                                TRANSFEROR = r.TRANSFEROR,
                                TRANSFREE = r.TRANSFREE,
                                APPAUTHSRNO = _atdb.AT_APPROVAL_AUTHORITY.Where(x => x.TRAN_NO == r.TRAN_NO && (x.APPROVAL_STATUS == "Pending" || x.APPROVAL_STATUS == "-" || x.APPROVAL_STATUS == "Rejected")).Select(y => y.SRNO).FirstOrDefault(),
                                TOTAL = r.TOTAL,
                                STATUS = r.STATUS,
                                PDFPATH = r.PDFPATH,
                                GETOUT_DOC_PATH = r.GETOUT_DOC_PATH,
                                GETIN_DOC_PATH = r.GETIN_DOC_PATH,
                                TRANSFREE_TYPE = r.TRANSFREE_TYPE,
                                DURATION = r.DURATION
                            }).OrderBy(x => x.TRAN_NO).ToList();


                }
                else
                {
                    Data = (from r in _atdb.AT_ASSSET_TRANSFER_HEADER
                            //where (r.GATEOUTSTAMP == null ? r.REQ_ECODE == LoginCode : r.GATEOUTSTAMP == LoginCode || r.GATEOUTSTAMP == LoginCode)
                            where (r.GATEOUTSTAMP == LoginCode || (r.GATEOUTSTAMP == null && r.REQ_ECODE == LoginCode))
                           && (r.STATUS == "Pending At Taxation" || r.STATUS == "Pending At Finance TM" || r.STATUS == "Pending At Approve" || r.STATUS == "Pending At Finance" || r.STATUS == "Pending At GateOutStamp" || r.STATUS == "Pending At Security" || r.STATUS == "UpdateNewAssetCode" || r.STATUS == "Pending At GateInStamp")
                             && r.ASSET_TYPE == flag
                            //(r.STATUS != "Initiate" && Status != "Completed" && Status != "Reject")
                            select new AT_AssetTransferHeaderViewModel
                            {
                                APPROVAL_TYPE = r.APPROVAL_TYPE,
                                SRNO = r.SRNO,
                                TRAN_NO = r.TRAN_NO,
                                TRAN_DATE = r.TRAN_DATE,
                                TRAN_TYPE = r.TRAN_TYPE,
                                ASSET_TYPE = r.ASSET_TYPE,
                                TRANSFEROR = r.TRANSFEROR,
                                TRANSFREE = r.TRANSFREE,
                                APPAUTHSRNO = _atdb.AT_APPROVAL_AUTHORITY.Where(x => x.TRAN_NO == r.TRAN_NO && (x.APPROVAL_STATUS == "Pending" || x.APPROVAL_STATUS == "Rejected")).Select(y => y.SRNO).FirstOrDefault(),
                                TOTAL = r.TOTAL,
                                STATUS = r.STATUS,
                                PDFPATH = r.PDFPATH,
                                TRANSFREE_TYPE = r.TRANSFREE_TYPE,
                                GETOUT_DOC_PATH = r.GETOUT_DOC_PATH,
                                GETIN_DOC_PATH = r.GETIN_DOC_PATH,
                                DURATION = r.DURATION
                            }).OrderBy(x => x.TRAN_NO).ToList();


                }
                foreach (var i in Data)
                {
                    if (i.TRANSFREE_TYPE == "Vendor")
                    {
                        i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                        i.TRANSFREE = i.TRANSFREE + "-" + GetVendorName(i.TRANSFREE);
                    }
                    else
                    {
                        i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                        i.TRANSFREE = i.TRANSFREE + "-" + GetEmpName(int.Parse(i.TRANSFREE));
                    }
                }
                return Data;

            }
            catch (Exception ex)
            {
                throw ex;

            }

        }

        public LibResult AutocomplitTranNo()
        {
            LibResult res = new LibResult();
            try
            {
                //using (var db = new LCEntities())
                //{
                int tranno = 0;
                var maxvalue = _atdb.AT_ASSSET_TRANSFER_HEADER.Select(x => x.SRNO).DefaultIfEmpty().Max();

                if (maxvalue == 0)
                {
                    tranno = 0001;
                }
                else
                {
                    tranno = maxvalue + 1;
                }
                res.resultObject = tranno;
                //}
            }
            catch (Exception ex)
            {
                res.hasError = true;
                res.errorMessage = ex.ToString();
            }
            return res;
        }

        public List<Employee_Details> AutocompleteSuggestions(string Key, string department)
        {
            // var value= (from r in _atdb.ADEMPLOYEE.Where(m=>m.ACTIVE==1 && m.ADEMPCODE.ToString().Contains(Key))

            var Data = (from r in _atdb.ADEMPLOYEE_LC
                        where (r.ADEMPCODE.ToString().ToUpper().Contains(Key.ToUpper()) ||
                               r.FIRSTNAME.ToString().ToUpper().Contains(Key.ToUpper()) ||
                               r.LASTNAME.ToString().ToUpper().Contains(Key.ToUpper()))
                                && (r.ACTIVE == 1)

                        select new Employee_Details
                        {
                            _ECode = r.ADEMPCODE,
                            _EFirstName = r.FIRSTNAME,
                            _ELastName = r.LASTNAME
                        }).ToList();

            List<Employee_Details> portalUserDtos = Data;
            return portalUserDtos;
        }

        public LibResult GetDepartment()
        {
            LibResult Res = new LibResult();
            try
            {
                //using (var db = new LCEntities())
                //{
                var Data = (from r in _atdb.VW_ASSOCIATELVLDETAILS_LC
                            select new
                            {
                                Text = r.DEPARTMENT,
                                Value = r.DEPARTMENT
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
        public AT_ASSSET_TRANSFER_HEADER CapitalizedEdit(int id)
        {

            var _obj = (from data in _atdb.AT_ASSSET_TRANSFER_HEADER.Where(v => v.SRNO == id)
                        select data).FirstOrDefault();

            if (_obj.TRANSFREE_TYPE == "Vendor")
            {
                _obj.TRANSFEROR = _obj.TRANSFEROR + "-" + GetEmpName(int.Parse(_obj.TRANSFEROR));
                _obj.TRANSFREE = _obj.TRANSFREE + "-" + GetVendorName(_obj.TRANSFREE);
            }
            else
            {
                _obj.TRANSFEROR = _obj.TRANSFEROR + "-" + GetEmpName(int.Parse(_obj.TRANSFEROR));
                _obj.TRANSFREE = _obj.TRANSFREE + "-" + GetEmpName(int.Parse(_obj.TRANSFREE));
            }



            return _obj;
        }

        public LibResult CapitalizedDetail(int id, int LoginCode, int APPAUTHSRNO)
        {
            LibResult res = new LibResult();
            try
            {
                //using (var db = new LCEntities())
                //{
                var d = (from a in _atdb.AT_ASSET_TRANSFER_DETAIL
                         where a.TRAN_NO == id
                         select a).OrderBy(x => x.SRNO).ToList();

                var ApprovalStatus = (from r in _atdb.AT_APPROVAL_AUTHORITY where r.TRAN_NO == id && r.SRNO == APPAUTHSRNO select r.APPROVAL_STATUS).FirstOrDefault();

                var HeaderStatus = (from r in _atdb.AT_ASSSET_TRANSFER_HEADER where r.TRAN_NO == id select r.STATUS).FirstOrDefault();

                var IsCom = (from r in _atdb.AT_ASSSET_TRANSFER_HEADER where r.TRAN_NO == id select r.COMPANYDEFINE_AUTHORITY).FirstOrDefault();

                var newlocation = (from r in _atdb.AT_ASSET_TRANSFER_DETAIL where r.TRAN_NO == id select r.NEW_LOCATION).FirstOrDefault();

                var Data = new
                {
                    DtData = d,
                    Status = ApprovalStatus,
                    HeaderStatus = HeaderStatus,
                    NewLocation = newlocation,
                    IsCompany = IsCom
                };

                res.resultObject = Data;
                //}
            }
            catch (Exception)
            {

                throw;
            }
            return res;
        }

        public List<AT_AssetTransferAuthorityViewModel> CapitalizedAuthority(int id)
        {
            var data = (from auth in _atdb.AT_APPROVAL_AUTHORITY.Where(v => v.TRAN_NO == id)
                        orderby auth.SRNO
                        select new AT_AssetTransferAuthorityViewModel
                        {
                            SrNo = auth.SRNO,
                            ADEMPCODE = auth.EMP_CODE,
                            DEPARTMENT = auth.DEPARTMENT,
                            APPROVAL_STATUS = auth.APPROVAL_STATUS,
                            TRAN_NO = auth.TRAN_NO,
                            ADEMPNAME = auth.EMP_NAME,
                            ADDESIGNATION = auth.DESIGNATION,
                            REMARKS = auth.REMARKS,


                        }).ToList();
            return data;
        }

        public LibResult EditTransaction(AT_ASSSET_TRANSFER_HEADER hd, List<AT_ASSET_TRANSFER_DETAIL> dt, List<AT_APPROVAL_AUTHORITY> _at, string flag, int LoginCode, string AssetType)
        {
            LibResult res = new LibResult();
            try
            {
                _atdb.Database.BeginTransaction();
                var total = (from s in dt select s.NET_BLOCK).Sum();
                hd.TOTAL = total;

                res = EditHeader(hd, flag, LoginCode, AssetType);
                if (res.hasError == false)
                {
                    res = EditDetail(dt, LoginCode, AssetType);
                }
                long tranno = 0;
                tranno = hd.TRAN_NO;
                res = EditAuthority(_at, LoginCode, tranno, flag);


                _atdb.Database.CommitTransaction();
                res.errorMessage = "Record Updated successfully";
            }
            catch (Exception ex)
            {
                _atdb.Database.RollbackTransaction();
                res.hasError = true;
                res.errorMessage = ex.ToString();

            }
            return res;
        }

        public LibResult EditHeader(AT_ASSSET_TRANSFER_HEADER hd, string flag, int LoginCode, string AssetType)
        {
            LibResult Res = new LibResult();
            try
            {
                //using (var db = new LCEntities())
                //{
                var Exists = _atdb.AT_ASSSET_TRANSFER_HEADER.Where(x => x.TRAN_NO == hd.TRAN_NO).Count();
                if (Exists > 1)
                {
                    Res.hasError = true;
                    Res.errorMessage = "Record Already Exists Please check and Update Another Employee";
                }

                else
                {
                    AT_ASSSET_TRANSFER_HEADER crmst = _atdb.AT_ASSSET_TRANSFER_HEADER.Find(hd.SRNO);

                    crmst.TRAN_TYPE = hd.TRAN_TYPE;
                    crmst.TRAN_NO = hd.TRAN_NO;
                    crmst.TRANSFEROR = hd.TRANSFEROR;
                    crmst.APPROVAL_TYPE = hd.APPROVAL_TYPE;
                    crmst.TRANSFREE = hd.TRANSFREE;
                    crmst.TRAN_DATE = hd.TRAN_DATE;
                    crmst.ASSET_TYPE = AssetType;
                    crmst.TOTAL = hd.TOTAL;
                    crmst.STATUS = flag == "SUBMIT" ? "Pending At Taxation" : "Initiate";
                    crmst.NEW_LOCATION = hd.NEW_LOCATION;
                    crmst.TRANSFREE_TYPE = hd.TRANSFREE_TYPE;
                    crmst.REMARKS = hd.REMARKS;
                    crmst.COMPANYDEFINE_AUTHORITY = hd.COMPANYDEFINE_AUTHORITY;
                    crmst.DURATION = hd.DURATION == null ? "" : hd.DURATION;


                    crmst.UPDATEDBY = LoginCode;
                    crmst.UPDATEDON = DateTime.Now;


                    _atdb.Entry(crmst).State = EntityState.Modified;
                    _atdb.SaveChanges();
                    Res.hasError = false;
                    Res.errorMessage = "Record Updated successfully";
                }

                //}
            }
            catch (Exception ex)
            {
                Res.hasError = true;
                Res.errorMessage = ex.ToString();

            }
            return Res;
        }

        public LibResult EditDetail(List<AT_ASSET_TRANSFER_DETAIL> dt, int LoginCode, string AssetType)
        {
            LibResult res = new LibResult();
            try
            {
                foreach (var d in dt)
                {
                    var newfilename = "";
                    if (d.ASSET_ATTACH != null && d.ASSET_ATTACH != "")
                    {
                        if (AssetType == "Capitalized Asset")
                        {
                            var filename = d.ASSET_ATTACH.Split('\\')[2];
                            newfilename = "ASSET_UPLOAD_" + d.ASSET_CODE + "_" + d.TRAN_NO + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + filename;

                        }
                        else
                        {
                            var filename = d.ASSET_ATTACH.Split('\\')[2];
                            newfilename = "ASSET_UPLOAD_" + d.SUBAUCCODE + "_" + d.TRAN_NO + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + filename;
                        }
                    }
                    //using (var _DB = new LCEntities())
                    //{
                    {
                        var Exist = _atdb.AT_ASSET_TRANSFER_DETAIL.Where(x => x.TRAN_NO == d.TRAN_NO
                                                  && x.ASSET_CODE == d.ASSET_CODE
                                                 && x.SRNO == d.SRNO
                                                 ).Count();

                        if (Exist > 0)
                        {
                            AT_ASSET_TRANSFER_DETAIL At_Detail = _atdb.AT_ASSET_TRANSFER_DETAIL.Where(x => x.TRAN_NO == d.TRAN_NO && x.ASSET_CODE == d.ASSET_CODE && x.SRNO == d.SRNO).FirstOrDefault();

                            At_Detail.TRAN_NO = d.TRAN_NO;
                            At_Detail.ASSET_CODE = d.ASSET_CODE;
                            if (d.PO_NO == null || d.PO_NO == "")
                            {
                                At_Detail.PO_NO = "";
                            }
                            else
                            {
                                At_Detail.PO_NO = d.PO_NO;
                            }
                            At_Detail.PO_DATE = d.PO_DATE == DateTime.MinValue ? null : d.PO_DATE;
                            At_Detail.FULLY_PARTIAL = d.FULLY_PARTIAL;
                            At_Detail.QTY = d.QTY == 0 ? 0 : d.QTY;
                            At_Detail.CURRENT_LOCATION = d.CURRENT_LOCATION;
                            At_Detail.NEW_LOCATION = d.NEW_LOCATION;
                            At_Detail.ORIGINAL_COST = d.ORIGINAL_COST;
                            At_Detail.DEPRECIATION = d.DEPRECIATION;
                            At_Detail.NET_BLOCK = d.NET_BLOCK;
                            At_Detail.INV_NO = d.INV_NO == null ? "" : d.INV_NO;
                            At_Detail.INV_DATE = d.INV_DATE == DateTime.MinValue ? null : d.INV_DATE;
                            if (d.ASSET_SERIAL_NO == null || d.ASSET_SERIAL_NO == "")
                            {
                                At_Detail.ASSET_SERIAL_NO = "";
                            }
                            else
                            {
                                At_Detail.ASSET_SERIAL_NO = d.ASSET_SERIAL_NO;
                            }
                            At_Detail.ASSET_CLASS = d.ASSET_CLASS;
                            At_Detail.CAPITALIZED_DATE = d.CAPITALIZED_DATE;
                            At_Detail.VENDER_NAME = d.VENDER_NAME == null ? " " : d.VENDER_NAME;
                            At_Detail.VENDOR_CODE = d.VENDOR_CODE == null ? " " : d.VENDOR_CODE;
                            At_Detail.ASSETMAIN_NO_TEXT = d.ASSETMAIN_NO_TEXT == null ? " " : d.ASSETMAIN_NO_TEXT;
                            At_Detail.LICENSE_NO = d.LICENSE_NO == null ? " " : d.LICENSE_NO;
                            At_Detail.LICENSE_DATE = d.LICENSE_DATE;
                            At_Detail.HSN_CODE = d.HSN_CODE;
                            At_Detail.DESCRIPTION = d.DESCRIPTION;
                            At_Detail.NEW_ASSET_CODE = d.NEW_ASSET_CODE == null ? "" : d.NEW_ASSET_CODE;
                            At_Detail.BASIC_PRICE = d.BASIC_PRICE;
                            At_Detail.INVOICE_VALUE = d.INVOICE_VALUE;
                            At_Detail.ASSET_CLS_DESC = d.ASSET_CLS_DESC == null ? "" : d.ASSET_CLS_DESC;
                            At_Detail.SUBAUCCODE = d.SUBAUCCODE;
                            if (d.ASSET_ATTACH == null || d.ASSET_ATTACH == "")
                            {
                                At_Detail.ASSET_ATTACH = At_Detail.ASSET_ATTACH;
                            }
                            else
                            {
                                At_Detail.ASSET_ATTACH = newfilename;

                            }
                            At_Detail.EODC_CLEAREANSE = d.EODC_CLEAREANSE == null ? "" : d.EODC_CLEAREANSE;

                            At_Detail.UPDATEDBY = LoginCode;
                            At_Detail.UPDATEDON = DateTime.Now;
                            //At_Detail.ASSET_ATTACH = d.ASSET_ATTACH == null ? "-" : d.ASSET_ATTACH;



                            _atdb.Entry(At_Detail).State = EntityState.Modified;
                            _atdb.SaveChanges();
                        }
                        else
                        {
                            var srno = _atdb.AT_ASSET_TRANSFER_DETAIL.Select(x => x.SRNO).DefaultIfEmpty().Max();
                            AT_ASSET_TRANSFER_DETAIL At_Detail = new AT_ASSET_TRANSFER_DETAIL();

                            At_Detail.SRNO = srno + 1;
                            At_Detail.TRAN_NO = d.TRAN_NO;
                            At_Detail.ASSET_CODE = d.ASSET_CODE;
                            At_Detail.PO_NO = d.PO_NO == null ? " " : d.PO_NO;
                            At_Detail.PO_DATE = d.PO_DATE == DateTime.MinValue ? null : d.PO_DATE;
                            At_Detail.FULLY_PARTIAL = d.FULLY_PARTIAL;
                            At_Detail.QTY = d.QTY;
                            At_Detail.CURRENT_LOCATION = d.CURRENT_LOCATION;
                            At_Detail.NEW_LOCATION = d.NEW_LOCATION;
                            At_Detail.ORIGINAL_COST = d.ORIGINAL_COST;
                            At_Detail.DEPRECIATION = d.DEPRECIATION;
                            At_Detail.NET_BLOCK = d.NET_BLOCK;
                            if (d.INV_NO == null)
                            {
                                At_Detail.INV_NO = " ";
                            }
                            else
                            {
                                At_Detail.INV_NO = d.INV_NO;
                            }
                            At_Detail.INV_DATE = d.INV_DATE == DateTime.MinValue ? null : d.INV_DATE;
                            if (d.ASSET_SERIAL_NO == null)
                            {
                                At_Detail.ASSET_SERIAL_NO = "";

                            }
                            else
                            {
                                At_Detail.ASSET_SERIAL_NO = d.ASSET_SERIAL_NO;
                            }
                            At_Detail.ASSET_CLASS = d.ASSET_CLASS;
                            At_Detail.CAPITALIZED_DATE = d.CAPITALIZED_DATE;
                            if (d.VENDER_NAME == null)
                            {
                                At_Detail.VENDER_NAME = " ";
                            }
                            else
                            {
                                At_Detail.VENDER_NAME = d.VENDER_NAME;
                            }
                            At_Detail.VENDOR_CODE = d.VENDOR_CODE == null ? "" : d.VENDOR_CODE;
                            if (d.ASSETMAIN_NO_TEXT == null)
                            {
                                At_Detail.ASSETMAIN_NO_TEXT = " ";
                            }
                            else
                            {
                                At_Detail.ASSETMAIN_NO_TEXT = d.ASSETMAIN_NO_TEXT;
                            }
                            if (d.LICENSE_NO == null)
                            {
                                At_Detail.LICENSE_NO = " ";
                            }
                            else
                            {
                                At_Detail.LICENSE_NO = d.LICENSE_NO;
                            }
                            At_Detail.LICENSE_DATE = d.LICENSE_DATE;

                            At_Detail.HSN_CODE = d.HSN_CODE;
                            At_Detail.DESCRIPTION = d.DESCRIPTION;
                            At_Detail.NEW_ASSET_CODE = "";
                            if (d.ASSET_ATTACH == null || d.ASSET_ATTACH == "")
                            {
                                At_Detail.ASSET_ATTACH = At_Detail.ASSET_ATTACH;
                            }
                            else
                            {
                                At_Detail.ASSET_ATTACH = newfilename;
                            }
                            if (d.BASIC_PRICE == 0)
                            {
                                At_Detail.BASIC_PRICE = 0;

                            }
                            else
                            {
                                At_Detail.BASIC_PRICE = d.BASIC_PRICE;
                            }
                            At_Detail.INVOICE_VALUE = d.INVOICE_VALUE == 0 ? 0 : d.INVOICE_VALUE;
                            At_Detail.ASSET_CLS_DESC = d.ASSET_CLS_DESC == null ? "" : d.ASSET_CLS_DESC;

                            At_Detail.ADDEDBY = LoginCode;
                            At_Detail.ADDEDON = DateTime.Now;

                            _atdb.Entry(At_Detail).State = EntityState.Added;
                            _atdb.SaveChanges();
                        }


                    }
                    //}
                }
                res.hasError = false;

            }
            catch (Exception ex)
            {
                res.hasError = true;
                res.errorMessage = ex.ToString();

            }
            return res;
        }

        public LibResult EditDetailTaxation(List<AT_ASSET_TRANSFER_DETAIL> dt, int LoginCode)
        {
            LibResult res = new LibResult();
            try
            {

                foreach (var d in dt)
                {
                    //var a = (from i in _atdb.AT_ASSET_TRANSFER_DETAIL.Where(x=>x.TRAN_NO==d.TRAN_NO && x.ASSET_CODE == d.ASSET_CODE)
                    //         select d.SRNO)

                    //using (var _DB = new LCEntities())
                    //{
                    {
                        var Exist = _atdb.AT_ASSET_TRANSFER_DETAIL.Where(x => x.TRAN_NO == d.TRAN_NO
                                                  && x.ASSET_CODE == d.ASSET_CODE
                                            && x.SRNO == d.SRNO
                                              ).Count();

                        if (Exist > 1)
                        {
                            res.hasError = true;
                            res.errorMessage = "record alredy exist...";
                        }
                        else
                        {
                            //AT_ASSET_TRANSFER_DETAIL At_Detail = _DB.AT_ASSET_TRANSFER_DETAIL.Where(x => x.TRAN_NO == d.TRAN_NO && x.ASSET_CODE == d.ASSET_CODE).FirstOrDefault();

                            AT_ASSET_TRANSFER_DETAIL At_Detail = _atdb.AT_ASSET_TRANSFER_DETAIL.Find(d.SRNO);

                            At_Detail.LICENSE_NO = d.LICENSE_NO == null ? " " : d.LICENSE_NO;
                            At_Detail.NEW_ASSET_CODE = d.NEW_ASSET_CODE == null ? "" : d.NEW_ASSET_CODE;
                            At_Detail.EODC_CLEAREANSE = d.EODC_CLEAREANSE == null ? "" : d.EODC_CLEAREANSE;

                            _atdb.Entry(At_Detail).State = EntityState.Modified;
                            _atdb.SaveChanges();
                        }

                    }
                    //}
                }
                res.hasError = false;

            }
            catch (Exception ex)
            {
                res.hasError = true;
                res.errorMessage = ex.ToString();

            }
            return res;
        }

        public LibResult EditAuthority(List<AT_APPROVAL_AUTHORITY> at, int LoginCode, long tranno, string flag)
        {
            LibResult res = new LibResult();
            try
            {
                var _hd = _atdb.AT_ASSSET_TRANSFER_HEADER.Where(x => x.TRAN_NO == tranno).FirstOrDefault();
                var Status = "Approved";

                var auth = _atdb.AT_APPROVAL_AUTHORITY.Where(x => x.TRAN_NO == tranno).ToList();
                if (auth != null)
                {
                    foreach (var data in auth)
                    {
                        _atdb.AT_APPROVAL_AUTHORITY.Remove(data);
                        _atdb.SaveChanges();
                    }
                }

                foreach (var d in at)
                {
                    //using (var DB_ = new LCEntities())
                    //{
                    var Exist = _atdb.AT_APPROVAL_AUTHORITY.Where(x => x.TRAN_NO == d.TRAN_NO
                                              && x.EMP_NAME == d.EMP_NAME
                                              && x.EMP_CODE == d.EMP_CODE
                                              && x.DEPARTMENT == d.DEPARTMENT
                                             ).Count();
                    if (Exist == 1)
                    {
                        res.hasError = true;
                        res.errorMessage = "Record alrady exist";
                    }
                    else
                    {
                        var srno = _atdb.AT_APPROVAL_AUTHORITY.Select(x => (int?)x.SRNO).Max() ?? 0;

                        AT_APPROVAL_AUTHORITY At_authority = new AT_APPROVAL_AUTHORITY
                        {
                            SRNO = srno + 1,
                            TRAN_NO = d.TRAN_NO,
                            EMP_CODE = d.EMP_CODE,
                            EMP_NAME = d.EMP_NAME,
                            DEPARTMENT = d.DEPARTMENT,
                            DESIGNATION = d.DESIGNATION,
                            REMARKS = " ",
                            ADDEDBY = LoginCode,
                            ADDEDON = DateTime.Now
                        };


                        var min = at.Where(x => x.DEPARTMENT == "Taxation" && x.TRAN_NO == d.TRAN_NO).Select(x => (int?)x.SRNO).Min();
                        if (d.DEPARTMENT == "Taxation" && d.SRNO == min)
                        {
                            At_authority.APPROVAL_STATUS = flag == "SUBMIT" ? "Pending" : "-";
                            if (flag == "SUBMIT")
                            {
                                SendMailByApprovalAuthority(_hd, At_authority.EMP_CODE, Status);
                            }
                        }
                        else
                        {
                            At_authority.APPROVAL_STATUS = "-";
                        }

                        _atdb.AT_APPROVAL_AUTHORITY.Add(At_authority);
                        _atdb.SaveChanges();
                    }


                    //}
                }

                res.hasError = false;

            }
            catch (Exception ex)
            {
                res.hasError = true;
                res.errorMessage = ex.ToString();

            }
            return res;
        }
        #endregion

        #region approval List Function

        public short UpdateApproval(string remarks, int LoginCode, int TranNo, string Status, List<AT_ASSET_TRANSFER_DETAIL> dt)
        {
            try
            {
                _atdb.Database.BeginTransaction();
                short retVal = 0;
                var _db = _atdb;
                //using (var _db = new LCEntities())
                //{
                AT_ASSSET_TRANSFER_HEADER hd_ = new AT_ASSSET_TRANSFER_HEADER();
                AT_APPROVAL_AUTHORITY appauth = new AT_APPROVAL_AUTHORITY();
                appauth = _db.AT_APPROVAL_AUTHORITY.Where(v => v.TRAN_NO == TranNo && v.EMP_CODE == LoginCode && v.APPROVAL_STATUS == "Pending").FirstOrDefault();

                if (Status == "Approved")
                {
                    if (appauth != null)
                    {
                        if (appauth.REMARKS == null)
                        {
                            appauth.REMARKS = "";
                        }
                        else
                        {
                            appauth.REMARKS = remarks;
                        }

                        appauth.APPROVAL_STATUS = Status;
                        appauth.UPDATEDBY = long.Parse(LoginCode.ToString());
                        appauth.UPDATEDON = DateTime.Now;

                        _db.Entry(appauth).State = EntityState.Modified;
                        _db.SaveChanges();

                        var Header = _db.AT_ASSSET_TRANSFER_HEADER.Where(h => h.TRAN_NO == TranNo).FirstOrDefault();
                        hd_ = Header;

                        if (Header.STATUS == "Pending At GateInStamp")
                        {
                            Header.STATUS = "Completed";
                            _db.Entry(Header).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                        if (Header != null && (Header.STATUS == "Pending At Taxation" || Header.STATUS == "Pending At Approve" || Header.STATUS == "UpdateNewAssetCode"))
                        {
                            if (Header.STATUS == "Pending At Taxation" || Header.STATUS == "UpdateNewAssetCode")
                            {
                                //LibResult res = EditDetail(dt, LoginCode);
                                LibResult res = EditDetailTaxation(dt, LoginCode);

                            }
                            if (Header.STATUS == "UpdateNewAssetCode" && Header != null && appauth.APPROVAL_STATUS == "Approved")
                            {
                                if (Header.TRAN_TYPE == "Returnable Transfer")
                                {
                                    Header.STATUS = "Completed";
                                    _db.Entry(Header).State = EntityState.Modified;
                                    _db.SaveChanges();
                                }
                                else
                                {
                                    Header.STATUS = "Pending At GateInStamp";
                                    _db.Entry(Header).State = EntityState.Modified;
                                    _db.SaveChanges();
                                }


                            }
                            var gettaxation = _db.AT_APPROVAL_AUTHORITY.Where(h => h.TRAN_NO == TranNo && h.DEPARTMENT == "Taxation").Count();

                            var gettaxation_app = _db.AT_APPROVAL_AUTHORITY.Where(h => h.TRAN_NO == TranNo && h.DEPARTMENT == "Taxation" && h.APPROVAL_STATUS == "Approved").Count();
                            if (gettaxation_app == gettaxation && Header.STATUS == "Pending At Taxation")
                            {
                                Header.STATUS = "Pending At Finance TM";
                                _db.Entry(Header).State = EntityState.Modified;
                                _db.SaveChanges();
                            }
                        }


                        if (Header.STATUS == "Pending At Security" && Header != null)
                        {
                            Header.STATUS = "UpdateNewAssetCode";
                            _db.Entry(Header).State = EntityState.Modified;
                            _db.SaveChanges();

                        }
                        retVal = 1;

                    }
                    LibResult re = InsertIntoAppAuthLog(appauth);
                    var nextno = appauth.SRNO + 1;
                    var appauth1 = _db.AT_APPROVAL_AUTHORITY.Where(v => v.SRNO == nextno && v.TRAN_NO == TranNo && v.APPROVAL_STATUS == "-").FirstOrDefault();
                    if (appauth1 != null)
                    {
                        appauth1.APPROVAL_STATUS = "Pending";
                        appauth1.REMARKS = " ";
                        appauth1.UPDATEDBY = long.Parse(LoginCode.ToString());
                        appauth1.UPDATEDON = DateTime.Now;

                        _db.Entry(appauth1).State = EntityState.Modified;
                        _db.SaveChanges();


                        if (appauth1.DEPARTMENT == "Finance" || appauth1.DEPARTMENT == "Finance Approval")
                        {

                            var gettaxation = _db.AT_APPROVAL_AUTHORITY.Where(h => h.TRAN_NO == TranNo && h.DEPARTMENT == "User Approval").Count();

                            var gettaxation_app = _db.AT_APPROVAL_AUTHORITY.Where(h => h.TRAN_NO == TranNo && h.DEPARTMENT == "User Approval" && h.APPROVAL_STATUS == "Approved").Count();
                            var Header = _db.AT_ASSSET_TRANSFER_HEADER.Where(h => h.TRAN_NO == TranNo).FirstOrDefault();
                            if (gettaxation_app == gettaxation && Header.STATUS == "Pending At Approve")
                            {
                                Header.STATUS = "Pending At Finance";
                                _db.Entry(Header).State = EntityState.Modified;
                                _db.SaveChanges();
                            }

                            retVal = 1;
                        }


                        SendMailByApprovalAuthority(hd_, appauth1.EMP_CODE, Status);


                        //LibResult re = InsertIntoAppAuthLog(appauth);

                    }
                }
                if (Status == "Sent Back")
                {
                    appauth.REMARKS = remarks;
                    appauth.APPROVAL_STATUS = Status;
                    appauth.UPDATEDBY = long.Parse(LoginCode.ToString());
                    appauth.UPDATEDON = DateTime.Now;

                    _db.Entry(appauth).State = EntityState.Modified;
                    _db.SaveChanges();
                    LibResult res = InsertIntoAppAuthLog(appauth);

                    var Header = _db.AT_ASSSET_TRANSFER_HEADER.Where(h => h.TRAN_NO == TranNo).FirstOrDefault();
                    hd_ = Header;
                    Header.STATUS = "Initiate";
                    _db.Entry(Header).State = EntityState.Modified;
                    _db.SaveChanges();

                    var authorityUpdate = _db.AT_APPROVAL_AUTHORITY.Where(h => h.TRAN_NO == TranNo).ToList();
                    foreach (var data in authorityUpdate)
                    {
                        data.APPROVAL_STATUS = "-";
                        data.REMARKS = " ";
                        _db.Entry(data).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                    SendMailByApprovalAuthority(hd_, appauth.EMP_CODE, Status);


                }
                if (Status == "Rejected")
                {
                    appauth.REMARKS = remarks;
                    appauth.APPROVAL_STATUS = Status;
                    appauth.UPDATEDBY = long.Parse(LoginCode.ToString());
                    appauth.UPDATEDON = DateTime.Now;

                    _db.Entry(appauth).State = EntityState.Modified;
                    _db.SaveChanges();
                    LibResult res = InsertIntoAppAuthLog(appauth);

                    var Header = _db.AT_ASSSET_TRANSFER_HEADER.Where(h => h.TRAN_NO == TranNo).FirstOrDefault();
                    hd_ = Header;
                    Header.STATUS = Status;
                    _db.Entry(Header).State = EntityState.Modified;
                    _db.SaveChanges();

                    SendMailByApprovalAuthority(hd_, appauth.EMP_CODE, Status);
                }
                //}
                _atdb.Database.CommitTransaction();
                return retVal;

            }
            catch (Exception ex)
            {
                if (_atdb.Database.CurrentTransaction != null)
                    _atdb.Database.RollbackTransaction();
                throw ex;
            }
        }


        public AT_AssetTransferHeaderViewModel CapitalizedAssetTransferEdit(int id)
        {

            var _obj = (from data in _atdb.AT_ASSSET_TRANSFER_HEADER.Where(v => v.TRAN_NO == id)

                        select new AT_AssetTransferHeaderViewModel
                        {
                            TRAN_NO = data.TRAN_NO,
                            TRANSFEROR = data.TRANSFEROR,
                            APPROVAL_TYPE = data.APPROVAL_TYPE,
                            TRANSFREE = data.TRANSFREE,
                            DURATION = data.DURATION
                        }).FirstOrDefault();

            return _obj;
        }

        public List<AT_AssetTransferApprovalViewModel> ApprovalListSearchData(string TransactionType, string AssetType, int TransactionNo, string Status, int LoginCode, string flag)
        {
            List<AT_AssetTransferApprovalViewModel> obj = new List<AT_AssetTransferApprovalViewModel>();

            if (flag == "User Approval")
            {
                obj = (from data in _atdb.AT_ASSSET_TRANSFER_HEADER
                       join a in _atdb.AT_APPROVAL_AUTHORITY on data.TRAN_NO equals a.TRAN_NO
                       where a.EMP_CODE == LoginCode //&& (data.STATUS == "Pending At Approve" || data.STATUS == "Rejected")
                     && (data.TRAN_NO == TransactionNo || TransactionNo == 0)
                       && (data.TRAN_TYPE == TransactionType || TransactionType == "All")
                       && (data.ASSET_TYPE == AssetType || AssetType == "All")
                       && (a.APPROVAL_STATUS == Status)
                         && a.DEPARTMENT == flag

                       select new AT_AssetTransferApprovalViewModel
                       {
                           SRNO = data.SRNO,
                           TRAN_NO = data.TRAN_NO,
                           TRAN_TYPE = data.TRAN_TYPE,
                           ASSET_TYPE = data.ASSET_TYPE,
                           TRAN_DATE = data.TRAN_DATE,
                           TRANSFEROR = data.TRANSFEROR,
                           TRANSFREE = data.TRANSFREE,
                           TOTAL = data.TOTAL,
                           APPROVAL_STATUS = a.APPROVAL_STATUS,
                           APPAUTHSRNO = a.SRNO,
                           HEADERSTATUS = data.STATUS,
                           PDFPATH = data.PDFPATH,
                           GETOUT_DOC_PATH = data.GETOUT_DOC_PATH,
                           GETIN_DOC_PATH = data.GETIN_DOC_PATH,
                           TRANSFREE_TYPE = data.TRANSFREE_TYPE
                       }).OrderBy(O => O.TRAN_NO).ToList();

            }
            else
            {

                obj = (from data in _atdb.AT_ASSSET_TRANSFER_HEADER
                       join a in _atdb.AT_APPROVAL_AUTHORITY on data.TRAN_NO equals a.TRAN_NO
                       where a.EMP_CODE == LoginCode //&& (data.STATUS == "Pending At Taxation"  || data.STATUS == "Pending At Finance TM" || data.STATUS == "Rejected")

                       && (data.TRAN_NO == TransactionNo || TransactionNo == 0)
                       && (data.TRAN_TYPE == TransactionType || TransactionType == "All")
                       && (data.ASSET_TYPE == AssetType || AssetType == "All")
                       && (a.APPROVAL_STATUS == Status)
                       && a.DEPARTMENT == flag

                       select new AT_AssetTransferApprovalViewModel
                       {
                           SRNO = data.SRNO,
                           TRAN_NO = data.TRAN_NO,
                           TRAN_TYPE = data.TRAN_TYPE,
                           ASSET_TYPE = data.ASSET_TYPE,
                           TRAN_DATE = data.TRAN_DATE,
                           TRANSFEROR = data.TRANSFEROR,
                           TRANSFREE = data.TRANSFREE,
                           TOTAL = data.TOTAL,
                           APPROVAL_STATUS = a.APPROVAL_STATUS,
                           APPAUTHSRNO = a.SRNO,
                           HEADERSTATUS = data.STATUS,
                           PDFPATH = data.PDFPATH,
                           GETOUT_DOC_PATH = data.GETOUT_DOC_PATH,
                           TRANSFREE_TYPE = data.TRANSFREE_TYPE,
                           GETIN_DOC_PATH = data.GETIN_DOC_PATH

                       }).OrderBy(O => O.TRAN_NO).ToList();


            }
            foreach (var i in obj)
            {
                if (i.TRANSFREE_TYPE == "Vendor")
                {
                    i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                    i.TRANSFREE = i.TRANSFREE + "-" + GetVendorName(i.TRANSFREE);
                }
                else
                {
                    i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                    i.TRANSFREE = i.TRANSFREE + "-" + GetEmpName(int.Parse(i.TRANSFREE));
                }
            }


            return obj;
        }


        public List<AT_AssetTransferApprovalViewModel> ApprovalListSearchDataStatus(string Status, int LoginCode, string flag)
        {
            List<AT_AssetTransferApprovalViewModel> obj = new List<AT_AssetTransferApprovalViewModel>();

            if (flag == "User Approval")
            {
                obj = (from data in _atdb.AT_ASSSET_TRANSFER_HEADER
                       join a in _atdb.AT_APPROVAL_AUTHORITY on data.TRAN_NO equals a.TRAN_NO
                       where a.EMP_CODE == LoginCode && a.APPROVAL_STATUS == Status && a.DEPARTMENT == flag//&& (data.STATUS == "Pending At Approve" || data.STATUS == "Rejected")
                       select new AT_AssetTransferApprovalViewModel
                       {
                           SRNO = data.SRNO,
                           TRAN_NO = data.TRAN_NO,
                           TRAN_TYPE = data.TRAN_TYPE,
                           ASSET_TYPE = data.ASSET_TYPE,
                           TRAN_DATE = data.TRAN_DATE,
                           TRANSFEROR = data.TRANSFEROR,
                           TRANSFREE = data.TRANSFREE,
                           TOTAL = data.TOTAL,
                           APPAUTHSRNO = a.SRNO,
                           APPROVAL_STATUS = a.APPROVAL_STATUS,
                           HEADERSTATUS = data.STATUS,
                           PDFPATH = data.PDFPATH,
                           GETOUT_DOC_PATH = data.GETOUT_DOC_PATH,
                           GETIN_DOC_PATH = data.GETIN_DOC_PATH,
                           TRANSFREE_TYPE = data.TRANSFREE_TYPE

                       }).OrderBy(O => O.TRAN_NO).ToList();

            }
            else if (flag == "Finance" || flag == "GateInStamp" || flag == "Update Asset Code" || flag == "Security")
            {
                obj = (from data in _atdb.AT_ASSSET_TRANSFER_HEADER
                       join a in _atdb.AT_APPROVAL_AUTHORITY on data.TRAN_NO equals a.TRAN_NO
                       where a.EMP_CODE == LoginCode && a.APPROVAL_STATUS == Status && a.DEPARTMENT == flag
                       select new AT_AssetTransferApprovalViewModel
                       {
                           SRNO = data.SRNO,
                           TRAN_NO = data.TRAN_NO,
                           TRAN_TYPE = data.TRAN_TYPE,
                           ASSET_TYPE = data.ASSET_TYPE,
                           TRAN_DATE = data.TRAN_DATE,
                           TRANSFEROR = data.TRANSFEROR,
                           TRANSFREE = data.TRANSFREE,
                           TOTAL = data.TOTAL,
                           APPAUTHSRNO = a.SRNO,
                           APPROVAL_STATUS = a.APPROVAL_STATUS,
                           HEADERSTATUS = data.STATUS,
                           PDFPATH = data.PDFPATH,
                           GETOUT_DOC_PATH = data.GETOUT_DOC_PATH,
                           GETIN_DOC_PATH = data.GETIN_DOC_PATH,
                           TRANSFREE_TYPE = data.TRANSFREE_TYPE

                       }).OrderBy(O => O.TRAN_NO).ToList();

            }

            else
            {


                obj = (from data in _atdb.AT_ASSSET_TRANSFER_HEADER
                       join a in _atdb.AT_APPROVAL_AUTHORITY on data.TRAN_NO equals a.TRAN_NO
                       where a.EMP_CODE == LoginCode && a.APPROVAL_STATUS == Status && a.DEPARTMENT == flag //&& (data.STATUS == "Pending At Taxation"|| data.STATUS == "Pending At Finance TM" || data.STATUS == "Rejected")
                       select new AT_AssetTransferApprovalViewModel
                       {
                           SRNO = data.SRNO,
                           TRAN_NO = data.TRAN_NO,
                           TRAN_TYPE = data.TRAN_TYPE,
                           ASSET_TYPE = data.ASSET_TYPE,
                           TRAN_DATE = data.TRAN_DATE,
                           TRANSFEROR = data.TRANSFEROR,
                           TRANSFREE = data.TRANSFREE,
                           TOTAL = data.TOTAL,
                           APPAUTHSRNO = a.SRNO,
                           APPROVAL_STATUS = a.APPROVAL_STATUS,
                           HEADERSTATUS = data.STATUS,
                           PDFPATH = data.PDFPATH,
                           TRANSFREE_TYPE = data.TRANSFREE_TYPE,
                           GETOUT_DOC_PATH = data.GETOUT_DOC_PATH,
                           GETIN_DOC_PATH = data.GETIN_DOC_PATH


                       }).OrderBy(O => O.TRAN_NO).ToList();

            }
            foreach (var i in obj)
            {
                if (i.TRANSFREE_TYPE == "Vendor")
                {
                    i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                    i.TRANSFREE = i.TRANSFREE + "-" + GetVendorName(i.TRANSFREE);
                }
                else
                {
                    i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                    i.TRANSFREE = i.TRANSFREE + "-" + GetEmpName(int.Parse(i.TRANSFREE));
                }
            }


            return obj;
        }

        public List<AT_AssetTransferApprovalViewModel> GetApprovalList(int LoginCode)
        {
            List<AT_AssetTransferApprovalViewModel> ilist = new List<AT_AssetTransferApprovalViewModel>();

            ilist = (from data in _atdb.AT_ASSSET_TRANSFER_HEADER
                     join a in _atdb.AT_APPROVAL_AUTHORITY on data.TRAN_NO equals a.TRAN_NO
                     where a.EMP_CODE == LoginCode && a.APPROVAL_STATUS == "Pending" && a.DEPARTMENT == "User Approval"
                     select new AT_AssetTransferApprovalViewModel
                     {
                         SRNO = data.SRNO,
                         TRAN_NO = data.TRAN_NO,
                         TRAN_TYPE = data.TRAN_TYPE,
                         ASSET_TYPE = data.ASSET_TYPE,
                         TRAN_DATE = data.TRAN_DATE,
                         TRANSFEROR = data.TRANSFEROR,
                         TRANSFREE = data.TRANSFREE,
                         TOTAL = data.TOTAL,
                         APPAUTHSRNO = a.SRNO,
                         APPROVAL_STATUS = a.APPROVAL_STATUS,
                         HEADERSTATUS = data.STATUS,
                         PDFPATH = data.PDFPATH,
                         TRANSFREE_TYPE = data.TRANSFREE_TYPE,
                         GETOUT_DOC_PATH = data.GETOUT_DOC_PATH,
                         GETIN_DOC_PATH = data.GETIN_DOC_PATH

                     }).OrderBy(O => O.TRAN_NO).ToList();

            foreach (var i in ilist)
            {
                if (i.TRANSFREE_TYPE == "Vendor")
                {
                    i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                    i.TRANSFREE = i.TRANSFREE + "-" + GetVendorName(i.TRANSFREE);
                }
                else
                {
                    i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                    i.TRANSFREE = i.TRANSFREE + "-" + GetEmpName(int.Parse(i.TRANSFREE));
                }
            }

            return ilist;
        }

        public List<AT_AssetTransferApprovalViewModel> SecurityGetApprovalList(int LoginCode)
        {
            List<AT_AssetTransferApprovalViewModel> ilist = new List<AT_AssetTransferApprovalViewModel>();

            ilist = (from data in _atdb.AT_ASSSET_TRANSFER_HEADER
                     join a in _atdb.AT_APPROVAL_AUTHORITY on data.TRAN_NO equals a.TRAN_NO
                     where a.EMP_CODE == LoginCode && a.APPROVAL_STATUS == "Pending" && (data.STATUS == "Pending At Security")
                     select new AT_AssetTransferApprovalViewModel
                     {
                         SRNO = data.SRNO,
                         TRAN_NO = data.TRAN_NO,
                         TRAN_TYPE = data.TRAN_TYPE,
                         ASSET_TYPE = data.ASSET_TYPE,
                         TRAN_DATE = data.TRAN_DATE,
                         TRANSFEROR = data.TRANSFEROR,
                         TRANSFREE = data.TRANSFREE,
                         TOTAL = data.TOTAL,
                         APPAUTHSRNO = a.SRNO,
                         APPROVAL_STATUS = a.APPROVAL_STATUS,
                         HEADERSTATUS = data.STATUS,
                         PDFPATH = data.PDFPATH,
                         TRANSFREE_TYPE = data.TRANSFREE_TYPE,
                         GETOUT_DOC_PATH = data.GETOUT_DOC_PATH,
                         GETIN_DOC_PATH = data.GETIN_DOC_PATH

                     }).OrderBy(O => O.TRAN_NO).ToList();

            foreach (var i in ilist)
            {
                if (i.TRANSFREE_TYPE == "Vendor")
                {
                    i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                    i.TRANSFREE = i.TRANSFREE + "-" + GetVendorName(i.TRANSFREE);
                }
                else
                {
                    i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                    i.TRANSFREE = i.TRANSFREE + "-" + GetEmpName(int.Parse(i.TRANSFREE));
                }
            }

            return ilist;
        }
        #endregion

        #region capitalize asset form load get data

        public List<AT_AssetTransferAuthorityViewModel> Gethead(long logincode, int TranNo, string TranType, int Transfree, string ApprovalType, long? opMappId)
        {
            List<AT_AssetTransferAuthorityViewModel> emp = new List<AT_AssetTransferAuthorityViewModel>();


            var sykiid = (from s in _atdb.SYKI_LC
                          where s.ACTIVE == 1
                          select s.SYKIID).FirstOrDefault();

            var department = (from _vw in _atdb.VW_ASSOCIATELVLDETAILS_LC
                              where _vw.ACTIVE == 1 && _vw.SYKI == sykiid && _vw.ADEMPCODE == logincode
                              select _vw.FUNCTIONALDESIGNATION).FirstOrDefault();


            var Taxation = (from emp1 in _atdb.ADEMPLOYEE_LC.Where(v => v.ADEMPCODE == logincode && v.ACTIVE == 1)
                            join _vw in _atdb.VW_ASSOCIATELVLDETAILS_LC on emp1.ADEMPCODE equals _vw.ADEMPCODE
                            join _v in _atdb.AT_COMMON_APPROVAL_MASTER on _vw.SYSITEID equals _v.SYSITEiD
                            where _vw.SYKI == sykiid && _vw.ACTIVE == 1 && emp1.ACTIVE == 1 && _v.DEPARTMENT == "Taxation"
                            orderby _v.SEQUENCE_NO
                            select new AT_AssetTransferAuthorityViewModel
                            {

                                SEQUENCE_NO = _v.SEQUENCE_NO,
                                ADEMPCODE = _v.ECODE,
                                ADEMPNAME = _atdb.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == _v.ECODE).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").FirstOrDefault(),
                                DEPARTMENT = _v.DEPARTMENT,
                                //ADDESIGNATION=   _atdb.ADDESIGNATION.Where(x=>x.ADDESIGNATIONID == _vw.ADDESIGNATIONID).Select(s => s.DESCRIP).FirstOrDefault(),
                                // ADDESIGNATION = _ad.DESCRIP,
                                TRAN_NO = TranNo,
                                APPROVAL_STATUS = "-"
                            }).ToList();

            if (Taxation != null)
            {

                emp.AddRange(Taxation);
            }
            decimal seqno = decimal.Parse("3.0");
            var FFTM1 = (from emp1 in _atdb.ADEMPLOYEE_LC.Where(v => v.ADEMPCODE == logincode && v.ACTIVE == 1)
                         join _vw in _atdb.VW_ASSOCIATELVLDETAILS_LC on emp1.ADEMPCODE equals _vw.ADEMPCODE
                         join _v in _atdb.AT_COMMON_APPROVAL_MASTER on _vw.SYSITEID equals _v.SYSITEiD
                         where _vw.SYKI == sykiid && _vw.ACTIVE == 1 && emp1.ACTIVE == 1 && _v.DEPARTMENT == "Finance" && _v.SEQUENCE_NO == seqno
                         orderby _v.SEQUENCE_NO
                         select new AT_AssetTransferAuthorityViewModel
                         {
                             SEQUENCE_NO = _v.SEQUENCE_NO,
                             ADEMPCODE = _v.ECODE,
                             ADEMPNAME = _atdb.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == _v.ECODE).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").FirstOrDefault(),
                             DEPARTMENT = "Finance",
                             //ADDESIGNATION = _v.DESIGNATION,
                             TRAN_NO = TranNo,
                             APPROVAL_STATUS = "-"
                         }).ToList();

            if (FFTM1 != null)
            {
                emp.AddRange(FFTM1);
            }

            var SectionHead = (from _vw in _atdb.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == logincode)
                               join _ad in _atdb.ADORGLEVEL_LC on _vw.SECTIONID equals _ad.ADORGLEVELID
                               join _adhead in _atdb.ADORGLEVELHEAD_LC on _ad.ADORGLEVELID equals _adhead.ADORGLEVELID
                               where _vw.SYKI == sykiid && _ad.SYKIID == sykiid && _vw.ACTIVE == 1 && _ad.ACTIVE == 1 && _adhead.ISACTIVE == 1
                               select new AT_AssetTransferAuthorityViewModel
                               {
                                   ADEMPCODE = _adhead.ADEMPCODE,
                                   ADEMPNAME = _atdb.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == _adhead.ADEMPCODE).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").FirstOrDefault(),
                                   //DEPARTMENT = _vw.SECTION,
                                   DEPARTMENT = "User Approval",
                                   ADDESIGNATION = _atdb.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == _adhead.ADEMPCODE && x.SYKI == sykiid && x.ACTIVE == 1).Select(s => s.FUNCTIONALDESIGNATION).FirstOrDefault(),
                                   TRAN_NO = TranNo,
                                   APPROVAL_STATUS = "-"
                               }).Distinct().FirstOrDefault();


            if (SectionHead != null)
            {
                if (department != SectionHead.ADDESIGNATION)
                {
                    emp.Add(SectionHead);
                }

            }
            var DepartmentHead = (from _vw in _atdb.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == logincode)
                                  join _ad in _atdb.ADORGLEVEL_LC on _vw.DEPARTMENTID equals _ad.ADORGLEVELID
                                  join _adhead in _atdb.ADORGLEVELHEAD_LC on _ad.ADORGLEVELID equals _adhead.ADORGLEVELID
                                  where _vw.SYKI == sykiid && _ad.SYKIID == sykiid && _vw.ACTIVE == 1 && _ad.ACTIVE == 1 && _adhead.ISACTIVE == 1
                                  select new AT_AssetTransferAuthorityViewModel
                                  {
                                      ADEMPCODE = _adhead.ADEMPCODE,
                                      ADEMPNAME = _atdb.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == _adhead.ADEMPCODE).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").FirstOrDefault(),
                                      //DEPARTMENT = _vw.DEPARTMENT,
                                      DEPARTMENT = "User Approval",
                                      ADDESIGNATION = _atdb.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == _adhead.ADEMPCODE && x.SYKI == sykiid && x.ACTIVE == 1).Select(s => s.FUNCTIONALDESIGNATION).FirstOrDefault(),
                                      TRAN_NO = TranNo,
                                      APPROVAL_STATUS = "-"
                                  }).Distinct().FirstOrDefault();
            if (DepartmentHead != null)
            {
                if (department != DepartmentHead.ADDESIGNATION)
                {
                    emp.Add(DepartmentHead);
                }

            }


            var Coordinator = (from _vw in _atdb.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == logincode)
                                   //join _ad in _atdb.ADORGCOORDINATOR on _vw.DEPARTMENTID equals _ad.ADORGLEVELID
                               join _V3 in _atdb.ADORGCOORDINATOR_LC on _vw.DIVISIONID equals _V3.ADORGLEVELID into _V3Join
                               from V3 in _V3Join.Where(d => d.ISACTIVE == 1).DefaultIfEmpty()
                               join _V4 in _atdb.ADORGCOORDINATOR_LC on _vw.OPERATIONID equals _V4.ADORGLEVELID into _V4Join
                               from V4 in _V4Join.Where(o => o.ISACTIVE == 1).DefaultIfEmpty()
                               where _vw.SYKI == sykiid && _vw.ACTIVE == 1
                               select new AT_AssetTransferAuthorityViewModel
                               {

                                   ADEMPCODE = V3.COORDINATOR == null ? V4.COORDINATOR : V3.COORDINATOR,
                                   ADEMPNAME = _atdb.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == (V3.COORDINATOR == null ? V4.COORDINATOR : V3.COORDINATOR)).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").FirstOrDefault(),
                                   DEPARTMENT = "User Approval",
                                   //ADDESIGNATION = _vw.FUNCTIONALDESIGNATION,
                                   //ADDESIGNATION = "",
                                   TRAN_NO = TranNo,
                                   APPROVAL_STATUS = "-"
                               }).Distinct().FirstOrDefault();


            if (Coordinator.ADEMPCODE != null)
            {
                emp.Add(Coordinator);

            }

            var DivisionHead = (from _vw in _atdb.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == logincode)
                                join _ad in _atdb.ADORGLEVEL_LC on _vw.DIVISIONID equals _ad.ADORGLEVELID
                                join _adhead in _atdb.ADORGLEVELHEAD_LC on _ad.ADORGLEVELID equals _adhead.ADORGLEVELID
                                where _vw.SYKI == sykiid && _ad.SYKIID == sykiid && _vw.ACTIVE == 1 && _ad.ACTIVE == 1 && _adhead.ISACTIVE == 1
                                select new AT_AssetTransferAuthorityViewModel
                                {
                                    ADEMPCODE = _adhead.ADEMPCODE,
                                    ADEMPNAME = _atdb.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == _adhead.ADEMPCODE).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").FirstOrDefault(),
                                    //DEPARTMENT = _vw.DIVISION,
                                    DEPARTMENT = "User Approval",
                                    ADDESIGNATION = _atdb.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == _adhead.ADEMPCODE && x.SYKI == sykiid && x.ACTIVE == 1).Select(s => s.FUNCTIONALDESIGNATION).FirstOrDefault(),
                                    TRAN_NO = TranNo,
                                    APPROVAL_STATUS = "-"
                                }).Distinct().FirstOrDefault();
            if (DivisionHead != null)
            {
                if (department != DivisionHead.ADDESIGNATION)
                {
                    emp.Add(DivisionHead);
                }

            }

            var Exe_Coordinator = (from _vw in _atdb.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == logincode)
                                       //join _ad in _atdb.ADORGCOORDINATOR on _vw.DIVISIONID equals _ad.ADORGLEVELID
                                       //join _V3 in _atdb.ADORGCOORDINATOR on _vw.DIVISIONID equals _V3.ADORGLEVELID into _V3Join
                                       //from V3 in _V3Join.Where(d => d.ISACTIVE == 1).DefaultIfEmpty()
                                   join _V4 in _atdb.ADORGCOORDINATOR_LC on (opMappId == null || opMappId == 0 ? _vw.OPERATIONID : opMappId) equals _V4.ADORGLEVELID into _V4Join
                                   from V4 in _V4Join.Where(o => o.ISACTIVE == 1).DefaultIfEmpty()
                                   where _vw.SYKI == sykiid && _vw.ACTIVE == 1
                                   select new AT_AssetTransferAuthorityViewModel
                                   {
                                       ADEMPCODE = V4.EXECOORDINATOR,
                                       ADEMPNAME = _atdb.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == V4.EXECOORDINATOR).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").FirstOrDefault(),
                                       //DEPARTMENT = _vw.DEPARTMENT,
                                       DEPARTMENT = "User Approval",
                                       //ADDESIGNATION = "Executive Coordinator",
                                       TRAN_NO = TranNo,
                                       APPROVAL_STATUS = "-"
                                   }).Distinct().FirstOrDefault();
            if (Exe_Coordinator.ADEMPCODE != null)
            {
                emp.Add(Exe_Coordinator);
            }

            var OperationHead = (from _vw in _atdb.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == logincode)
                                 join _ad in _atdb.ADORGLEVEL_LC on _vw.OPERATIONID equals _ad.ADORGLEVELID
                                 //join _adhead in _atdb.ADORGLEVELHEAD on _ad.ADORGLEVELID equals _adhead.ADORGLEVELID
                                 join _V4 in _atdb.ADORGCOORDINATOR_LC on (opMappId == null || opMappId == 0 ? _vw.OPERATIONID : opMappId) equals _V4.ADORGLEVELID into _V4Join
                                 from V4 in _V4Join.Where(o => o.ISACTIVE == 1).DefaultIfEmpty()
                                 where _vw.SYKI == sykiid && _ad.SYKIID == sykiid && _vw.ACTIVE == 1 && _ad.ACTIVE == 1 //&& _adhead.ISACTIVE == 1
                                 select new AT_AssetTransferAuthorityViewModel
                                 {
                                     ADEMPCODE = V4.OPHEAD,
                                     ADEMPNAME = _atdb.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == V4.OPHEAD).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").FirstOrDefault(),

                                     DEPARTMENT = "User Approval",
                                     ADDESIGNATION = _atdb.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == V4.OPHEAD && x.SYKI == sykiid && x.ACTIVE == 1).Select(s => s.FUNCTIONALDESIGNATION).FirstOrDefault(),
                                     TRAN_NO = TranNo,
                                     APPROVAL_STATUS = "-"
                                 }).Distinct().FirstOrDefault();

            if (OperationHead != null)
            {
                if (department != OperationHead.ADDESIGNATION)
                {
                    emp.Add(OperationHead);
                }
            }


            if (ApprovalType == "EBQ")
            {

                var EVPUser = (from _vw in _atdb.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == logincode)
                               join _ad in _atdb.AT_APPROVAL_OPERATION_MAPPING on _vw.DIVISIONID equals _ad.DIVISION_ID
                               where _vw.SYKI == sykiid && _ad.SYKIID == sykiid && _vw.ACTIVE == 1
                               select new AT_AssetTransferAuthorityViewModel
                               {
                                   ADEMPCODE = _ad.EVP_AUTHORITY,
                                   ADEMPNAME = _atdb.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == _ad.EVP_AUTHORITY).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").FirstOrDefault(),

                                   DEPARTMENT = "User Approval",
                                   TRAN_NO = TranNo,
                                   APPROVAL_STATUS = "-"
                               }).Distinct().FirstOrDefault();

                if (EVPUser.ADEMPCODE != 0)
                {
                    emp.Add(EVPUser);

                }

                var Director = (from data in _atdb.ADEMPLOYEE_LC
                                join _VW in _atdb.VW_ASSOCIATELVLDETAILS_LC on data.ADEMPCODE equals _VW.ADEMPCODE
                                //join _AdCoor in _atdb.ADORGCOORDINATOR on _VW.OPERATIONID equals _AdCoor.ADORGLEVELID into _AdCoorJoin
                                //from AdCoordinator in _AdCoorJoin.Where(o => o.ISACTIVE == 1).DefaultIfEmpty()
                                join _V4 in _atdb.ADORGCOORDINATOR_LC on (opMappId == null || opMappId == 0 ? _VW.OPERATIONID : opMappId) equals _V4.ADORGLEVELID into _V4Join
                                from V4 in _V4Join.Where(o => o.ISACTIVE == 1).DefaultIfEmpty()
                                where data.ADEMPCODE == logincode && data.ACTIVE == 1 && _VW.SYKI == sykiid
                                select new AT_AssetTransferAuthorityViewModel
                                {
                                    ADEMPCODE = V4.DIRECTOR,
                                    ADEMPNAME = _atdb.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == V4.DIRECTOR).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").FirstOrDefault(),

                                    DEPARTMENT = "User Approval",
                                    TRAN_NO = TranNo,
                                    APPROVAL_STATUS = "-"
                                }).FirstOrDefault();


                if (Director != null)
                {
                    emp.Add(Director);
                }

                var Director2 = (from _vw in _atdb.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == logincode)
                                     //join _ad in _atdb.ADORGCOORDINATOR on _vw.DIVISIONID equals _ad.ADORGLEVELID
                                     //join _V3 in _atdb.ADORGCOORDINATOR on _vw.DIVISIONID equals _V3.ADORGLEVELID into _V3Join
                                     //from V3 in _V3Join.Where(d => d.ISACTIVE == 1).DefaultIfEmpty()
                                 join _V4 in _atdb.ADORGCOORDINATOR_LC on (opMappId == null || opMappId == 0 ? _vw.OPERATIONID : opMappId) equals _V4.ADORGLEVELID into _V4Join
                                 from V4 in _V4Join.Where(o => o.ISACTIVE == 1).DefaultIfEmpty()
                                 where _vw.SYKI == sykiid && _vw.ACTIVE == 1
                                 select new AT_AssetTransferAuthorityViewModel
                                 {
                                     ADEMPCODE = V4.ISSKIP == 1 ? null : V4.DIRECTOR2,
                                     ADEMPNAME = V4.ISSKIP == 1 ? "" : _atdb.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == V4.DIRECTOR2).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").FirstOrDefault(),

                                     DEPARTMENT = "User Approval",
                                     TRAN_NO = TranNo,
                                     APPROVAL_STATUS = "-"
                                 }).Distinct().FirstOrDefault();
                if (Director2.ADEMPCODE != null)
                {
                    emp.Add(Director2);
                }


                var cpo = (from emp1 in _atdb.ADEMPLOYEE_LC.Where(v => v.ADEMPCODE == logincode && v.ACTIVE == 1)
                           join _vw in _atdb.VW_ASSOCIATELVLDETAILS_LC on emp1.ADEMPCODE equals _vw.ADEMPCODE
                           join _v in _atdb.AT_COMMON_APPROVAL_MASTER on _vw.SYSITEID equals _v.SYSITEiD
                           where _vw.SYKI == sykiid && _vw.ACTIVE == 1 && emp1.ACTIVE == 1 && _v.DEPARTMENT == "Chief Production Officer"
                           orderby _v.SEQUENCE_NO
                           select new AT_AssetTransferAuthorityViewModel
                           {

                               SEQUENCE_NO = _v.SEQUENCE_NO,
                               ADEMPCODE = _v.ECODE,
                               ADEMPNAME = _atdb.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == _v.ECODE).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").FirstOrDefault(),
                               DEPARTMENT = "User Approval",
                               TRAN_NO = TranNo,
                               APPROVAL_STATUS = "-"
                           }).FirstOrDefault();

                if (cpo != null)
                {

                    emp.Add(cpo);
                }
            }

            // var finance_teammember = _atdb.AT_COMMON_APPROVAL_MASTER.Where(x => x.DEPARTMENT == "Finance" && x.DEPARTMENT == "Team Meamber").FirstOrDefault();

            var finance_teammember = (from emp1 in _atdb.ADEMPLOYEE_LC.Where(v => v.ADEMPCODE == logincode && v.ACTIVE == 1)
                                      join _vw in _atdb.VW_ASSOCIATELVLDETAILS_LC on emp1.ADEMPCODE equals _vw.ADEMPCODE
                                      join _v in _atdb.AT_COMMON_APPROVAL_MASTER on _vw.SYSITEID equals _v.SYSITEiD
                                      where _vw.SYKI == sykiid && _vw.ACTIVE == 1 && emp1.ACTIVE == 1 && _v.DEPARTMENT == "Finance" //&& _v.DESIGNATION == "Team Member"
                                      select new AT_AssetTransferAuthorityViewModel
                                      {
                                          ADEMPCODE = _v.ECODE
                                      }).FirstOrDefault();

            if (finance_teammember != null)
            {
                var finance_DepartmentHead = (from _vw in _atdb.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == finance_teammember.ADEMPCODE)
                                              join _ad in _atdb.ADORGLEVEL_LC on _vw.DEPARTMENTID equals _ad.ADORGLEVELID
                                              join _adhead in _atdb.ADORGLEVELHEAD_LC on _ad.ADORGLEVELID equals _adhead.ADORGLEVELID
                                              where _vw.SYKI == sykiid && _ad.SYKIID == sykiid && _vw.ACTIVE == 1 && _ad.ACTIVE == 1 && _adhead.ISACTIVE == 1
                                              select new AT_AssetTransferAuthorityViewModel
                                              {
                                                  ADEMPCODE = _adhead.ADEMPCODE,
                                                  ADEMPNAME = _atdb.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == _adhead.ADEMPCODE).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").FirstOrDefault(),
                                                  DEPARTMENT = "Finance Approval",
                                                  TRAN_NO = TranNo,
                                                  APPROVAL_STATUS = "-"
                                              }).Distinct().FirstOrDefault();

                //var finance_DepartmentHead = (from _vw in _atdb.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == finance_teammember.ADEMPCODE)
                //                              where _vw.SYKI == sykiid && _vw.ACTIVE == 1
                //                              select new AT_AssetTransferAuthorityViewModel
                //                              {
                //                                  ADEMPCODE = _vw.SUPERVISOREMPCODE,
                //                                  ADEMPNAME = _atdb.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == _vw.SUPERVISOREMPCODE).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").FirstOrDefault(),
                //                                  DEPARTMENT = "Finance Approval",
                //                                  //ADDESIGNATION = _atdb.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == _vw.SUPERVISOREMPCODE && x.SYKI == sykiid && x.ACTIVE == 1).Select(s => s.FUNCTIONALDESIGNATION).FirstOrDefault(),
                //                                  TRAN_NO = TranNo,
                //                                  APPROVAL_STATUS = "-"
                //                              }).Distinct().FirstOrDefault();

                if (finance_DepartmentHead != null)
                {
                    emp.Add(finance_DepartmentHead);
                }
                var finance_divion = (from _vw in _atdb.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == finance_teammember.ADEMPCODE)
                                      join _ad in _atdb.ADORGLEVEL_LC on _vw.DIVISIONID equals _ad.ADORGLEVELID
                                      join _adhead in _atdb.ADORGLEVELHEAD_LC on _ad.ADORGLEVELID equals _adhead.ADORGLEVELID
                                      where _vw.SYKI == sykiid && _ad.SYKIID == sykiid && _vw.ACTIVE == 1 && _ad.ACTIVE == 1 && _adhead.ISACTIVE == 1
                                      select new AT_AssetTransferAuthorityViewModel
                                      {
                                          ADEMPCODE = _adhead.ADEMPCODE,
                                          ADEMPNAME = _atdb.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == _adhead.ADEMPCODE).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").FirstOrDefault(),
                                          DEPARTMENT = "Finance Approval",
                                          TRAN_NO = TranNo,
                                          APPROVAL_STATUS = "-"
                                      }).Distinct().FirstOrDefault();

                //var finance_divion = (from _vw in _atdb.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == finance_teammember.ADEMPCODE)
                //                      where _vw.SYKI == sykiid && _vw.ACTIVE == 1
                //                      select new AT_AssetTransferAuthorityViewModel
                //                      {
                //                          ADEMPCODE = _vw.SUPSUPERVISOREMPCODE,
                //                          ADEMPNAME = _atdb.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == _vw.SUPSUPERVISOREMPCODE).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").FirstOrDefault(),
                //                          DEPARTMENT = "Finance Approval",
                //                          //ADDESIGNATION = _atdb.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == _vw.SUPSUPERVISOREMPCODE && x.SYKI == sykiid && x.ACTIVE == 1).Select(s => s.FUNCTIONALDESIGNATION).FirstOrDefault(),
                //                          TRAN_NO = TranNo,
                //                          APPROVAL_STATUS = "-"
                //                      }).Distinct().FirstOrDefault();
                if (finance_divion != null)
                {

                    emp.Add(finance_divion);

                }
            }


            var initiator = (from data in _atdb.ADEMPLOYEE_LC.Where(v => v.ADEMPCODE == logincode && v.ACTIVE == 1)
                             join _VW in _atdb.VW_ASSOCIATELVLDETAILS_LC on data.ADEMPCODE equals _VW.ADEMPCODE
                             join _Desg in _atdb.ADDESIGNATION_LC on _VW.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
                             where _VW.SYKI == sykiid && _VW.ACTIVE == 1
                             select new AT_AssetTransferAuthorityViewModel
                             {
                                 ADEMPCODE = data.ADEMPCODE,
                                 ADEMPNAME = _atdb.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == data.ADEMPCODE).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").FirstOrDefault(),
                                 DEPARTMENT = "GateOutStamp",
                                 TRAN_NO = TranNo,
                                 APPROVAL_STATUS = "-"

                             }).FirstOrDefault();

            if (initiator != null)
            {
                emp.Add(initiator);
            }


            var Security = (from emp1 in _atdb.ADEMPLOYEE_LC.Where(v => v.ADEMPCODE == logincode && v.ACTIVE == 1)
                            join _vw in _atdb.VW_ASSOCIATELVLDETAILS_LC on emp1.ADEMPCODE equals _vw.ADEMPCODE
                            join addes in _atdb.ADDESIGNATION_LC on _vw.ACTIVE equals addes.ADDESIGNATIONID
                            join _v in _atdb.AT_COMMON_APPROVAL_MASTER on _vw.SYSITEID equals _v.SYSITEiD
                            where _vw.SYKI == sykiid && _vw.ACTIVE == 1 && emp1.ACTIVE == 1 && _v.DEPARTMENT == "Security"
                            orderby _v.SEQUENCE_NO
                            select new AT_AssetTransferAuthorityViewModel
                            {
                                SEQUENCE_NO = _v.SEQUENCE_NO,
                                ADEMPCODE = _v.ECODE,
                                ADEMPNAME = _atdb.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == _v.ECODE).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").FirstOrDefault(),
                                DEPARTMENT = _v.DEPARTMENT,
                                TRAN_NO = TranNo,
                                APPROVAL_STATUS = "-",
                            }).ToList();

            if (Security != null)
            {
                emp.AddRange(Security);
            }

            decimal seqno1 = decimal.Parse("3.0");
            var FFTM3 = (from emp1 in _atdb.ADEMPLOYEE_LC.Where(v => v.ADEMPCODE == logincode && v.ACTIVE == 1)
                         join _vw in _atdb.VW_ASSOCIATELVLDETAILS_LC on emp1.ADEMPCODE equals _vw.ADEMPCODE
                         join _v in _atdb.AT_COMMON_APPROVAL_MASTER on _vw.SYSITEID equals _v.SYSITEiD
                         where _vw.SYKI == sykiid && _vw.ACTIVE == 1 && emp1.ACTIVE == 1 && _v.DEPARTMENT == "Finance" && _v.SEQUENCE_NO == seqno1
                         orderby _v.SEQUENCE_NO
                         select new AT_AssetTransferAuthorityViewModel
                         {
                             SEQUENCE_NO = _v.SEQUENCE_NO,
                             ADEMPCODE = _v.ECODE,
                             ADEMPNAME = _atdb.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == _v.ECODE).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").FirstOrDefault(),
                             DEPARTMENT = "Update Asset Code",
                             TRAN_NO = TranNo,
                             APPROVAL_STATUS = "-"
                         }).ToList();

            if (FFTM3 != null)
            {
                emp.AddRange(FFTM3);
            }


            if (TranType == "Permanent Transfer" && Transfree != 0)
            {
                var transfreePlant = _atdb.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == Transfree
                                                       && x.SYKI == sykiid && x.ACTIVE == 1).Select(x => x.SYSITEID).FirstOrDefault();

                //var site = (from a in _atdb.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == Transfree)
                //            join s in _atdb.SYSITE on a.SYSITEID equals s.SYSITEID
                //            where a.SYKI == sykiid && a.ACTIVE == 1
                //            select s.DESCRIP).FirstOrDefault();

                decimal seqno_ = decimal.Parse("3.0");
                var GetTransfreeFFTM = (from i in _atdb.AT_COMMON_APPROVAL_MASTER
                                        where i.SYSITEiD == transfreePlant
                                            && i.DEPARTMENT == "Finance"
                                            && i.SEQUENCE_NO == seqno_
                                        select i).FirstOrDefault();

                //var transfreeEname;
                if (GetTransfreeFFTM != null)
                {
                    var transfreeEname = _atdb.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == GetTransfreeFFTM.ECODE && x.ACTIVE == 1).FirstOrDefault();

                    var MaxSrno = _atdb.AT_APPROVAL_AUTHORITY.Select(x => x.SRNO).DefaultIfEmpty().Max();
                    AT_AssetTransferAuthorityViewModel FFTM2 = new AT_AssetTransferAuthorityViewModel();

                    FFTM2.SrNo = MaxSrno + 1;
                    FFTM2.TRAN_NO = TranNo;
                    FFTM2.ADEMPCODE = GetTransfreeFFTM.ECODE;
                    FFTM2.ADEMPNAME = transfreeEname.FIRSTNAME.ToString() + " " + transfreeEname.LASTNAME.ToString();
                    FFTM2.DEPARTMENT = "GateInStamp";
                    FFTM2.ADDESIGNATION = GetTransfreeFFTM.DESIGNATION;
                    FFTM2.APPROVAL_STATUS = "-";
                    FFTM2.REMARKS = " ";
                    FFTM2.ADDEDBY = logincode;
                    FFTM2.ADDEDON = DateTime.Now;

                    if (FFTM2 != null)
                    {
                        emp.Add(FFTM2);
                    }
                }



            }
            foreach (var data in emp)
            {
                if (data.ADDESIGNATION == null)
                {
                    var designation = (from emp1 in _atdb.ADEMPLOYEE_LC.Where(v => v.ADEMPCODE == data.ADEMPCODE && v.ACTIVE == 1)
                                       join _VW in _atdb.VW_ASSOCIATELVLDETAILS_LC on emp1.ADEMPCODE equals _VW.ADEMPCODE
                                       join _Desg in _atdb.ADDESIGNATION_LC on _VW.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
                                       where _VW.SYKI == sykiid
                                       select (_Desg.DESCRIP)).FirstOrDefault();
                    data.ADDESIGNATION = designation;

                }
            }

            return emp;
        }

        public LibResult Maxsrno()
        {
            LibResult res = new LibResult();
            var srno = _atdb.AT_ASSET_TRANSFER_DETAIL.Select(x => x.SRNO).DefaultIfEmpty().Max();
            res.resultObject = srno;
            return res;
        }


        public LibResult BindApprovalType(int LoginCode)
        {
            LibResult res = new LibResult();
            try
            {
                //using (var db = new LCEntities())
                //{

                var sykiid = (from s in _atdb.SYKI_LC
                              where s.ACTIVE == 1
                              select s.SYKIID).FirstOrDefault();

                var associateDetails = (from vw in _atdb.VW_ASSOCIATELVLDETAILS_LC
                                        where vw.ADEMPCODE == LoginCode && vw.ACTIVE == 1 && vw.SYKI == sykiid
                                        select new
                                        {
                                            division = vw.DIVISIONID,
                                            operation = vw.OPERATIONID

                                        }).FirstOrDefault();

                // Then retrieve AT_APPROVAL_OPERATION_MAPPING
                var approvaltype = (from oid in _atdb.AT_APPROVAL_OPERATION_MAPPING
                                    where oid.DIVISION_ID == associateDetails.division && oid.OPERATION_ID == associateDetails.operation
                                    select oid.APPROVAL_TYPE).FirstOrDefault();

                res.resultObject = approvaltype;
                //}
            }
            catch (Exception ex)
            {
                res.hasError = true;
                res.errorMessage = ex.ToString();
            }
            return res;
        }
        #endregion

        #region  taxation approval list 

        public List<AT_AssetTransferApprovalViewModel> GetTaxationApprovalList(int LoginCode)
        {
            var obj = (from data in _atdb.AT_ASSSET_TRANSFER_HEADER
                       join a in _atdb.AT_APPROVAL_AUTHORITY on data.TRAN_NO equals a.TRAN_NO
                       where a.EMP_CODE == LoginCode && a.DEPARTMENT == "Taxation" && a.APPROVAL_STATUS == "Pending"
                       select new AT_AssetTransferApprovalViewModel
                       {
                           SRNO = data.SRNO,
                           TRAN_NO = data.TRAN_NO,
                           TRAN_TYPE = data.TRAN_TYPE,
                           ASSET_TYPE = data.ASSET_TYPE,
                           TRAN_DATE = data.TRAN_DATE,
                           TRANSFEROR = data.TRANSFEROR,
                           TRANSFREE = data.TRANSFREE,
                           TOTAL = data.TOTAL,
                           APPAUTHSRNO = a.SRNO,
                           APPROVAL_STATUS = a.APPROVAL_STATUS,
                           HEADERSTATUS = data.STATUS,
                           PDFPATH = data.PDFPATH,
                           TRANSFREE_TYPE = data.TRANSFREE_TYPE,
                           GETOUT_DOC_PATH = data.GETOUT_DOC_PATH,
                           GETIN_DOC_PATH = data.GETIN_DOC_PATH
                       }).OrderBy(O => O.TRAN_NO).ToList();

            foreach (var i in obj)
            {
                if (i.TRANSFREE_TYPE == "Vendor")
                {
                    i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                    i.TRANSFREE = i.TRANSFREE + "-" + GetVendorName(i.TRANSFREE);
                }
                else
                {
                    i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                    i.TRANSFREE = i.TRANSFREE + "-" + GetEmpName(int.Parse(i.TRANSFREE));
                }
            }
            return obj;
        }

        public AT_ASSSET_TRANSFER_HEADER Get_AT_ASSSET_TRANSFER_HEADER_ModelData(int id)
        {

            var _obj = (from data in _atdb.AT_ASSSET_TRANSFER_HEADER.Where(v => v.SRNO == id)
                        select data).FirstOrDefault();




            if (_obj.TRANSFREE_TYPE == "Vendor")
            {
                _obj.TRANSFEROR = _obj.TRANSFEROR + "-" + GetEmpName(int.Parse(_obj.TRANSFEROR));
                _obj.TRANSFREE = _obj.TRANSFREE + "-" + GetVendorName(_obj.TRANSFREE);
            }
            else
            {
                _obj.TRANSFEROR = _obj.TRANSFEROR + "-" + GetEmpName(int.Parse(_obj.TRANSFEROR));
                _obj.TRANSFREE = _obj.TRANSFREE + "-" + GetEmpName(int.Parse(_obj.TRANSFREE));
            }

            return _obj;


        }
        #endregion

        #region Finance approval list

        public List<AT_AssetTransferApprovalViewModel> GetFinanceApprovalList(int LoginCode)
        {
            var obj = (from data in _atdb.AT_ASSSET_TRANSFER_HEADER
                       join a in _atdb.AT_APPROVAL_AUTHORITY on data.TRAN_NO equals a.TRAN_NO
                       where a.EMP_CODE == LoginCode && a.DEPARTMENT == "Finance"
                       && a.APPROVAL_STATUS == "Pending"
                       select new AT_AssetTransferApprovalViewModel
                       {
                           SRNO = data.SRNO,
                           TRAN_NO = data.TRAN_NO,
                           TRAN_TYPE = data.TRAN_TYPE,
                           ASSET_TYPE = data.ASSET_TYPE,
                           TRAN_DATE = data.TRAN_DATE,
                           TRANSFEROR = data.TRANSFEROR,
                           TRANSFREE = data.TRANSFREE,
                           TOTAL = data.TOTAL,
                           APPAUTHSRNO = a.SRNO,
                           APPROVAL_STATUS = a.APPROVAL_STATUS,
                           HEADERSTATUS = data.STATUS,
                           PDFPATH = data.PDFPATH,
                           TRANSFREE_TYPE = data.TRANSFREE_TYPE,
                           GETOUT_DOC_PATH = data.GETOUT_DOC_PATH,
                           GETIN_DOC_PATH = data.GETIN_DOC_PATH
                       }).OrderBy(O => O.TRAN_NO).ToList();

            foreach (var i in obj)
            {
                if (i.TRANSFREE_TYPE == "Vendor")
                {
                    i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                    i.TRANSFREE = i.TRANSFREE + "-" + GetVendorName(i.TRANSFREE);
                }
                else
                {
                    i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                    i.TRANSFREE = i.TRANSFREE + "-" + GetEmpName(int.Parse(i.TRANSFREE));
                }
            }

            return obj;
        }

        public List<AT_AssetTransferApprovalViewModel> GetFinanceApproveList(int LoginCode)
        {
            var obj = (from data in _atdb.AT_ASSSET_TRANSFER_HEADER
                       join a in _atdb.AT_APPROVAL_AUTHORITY on data.TRAN_NO equals a.TRAN_NO
                       where a.EMP_CODE == LoginCode && a.DEPARTMENT == "Finance Approval"
                       && a.APPROVAL_STATUS == "Pending"
                       select new AT_AssetTransferApprovalViewModel
                       {
                           SRNO = data.SRNO,
                           TRAN_NO = data.TRAN_NO,
                           TRAN_TYPE = data.TRAN_TYPE,
                           ASSET_TYPE = data.ASSET_TYPE,
                           TRAN_DATE = data.TRAN_DATE,
                           TRANSFEROR = data.TRANSFEROR,
                           TRANSFREE = data.TRANSFREE,
                           TOTAL = data.TOTAL,
                           APPAUTHSRNO = a.SRNO,
                           APPROVAL_STATUS = a.APPROVAL_STATUS,
                           HEADERSTATUS = data.STATUS,
                           PDFPATH = data.PDFPATH,
                           TRANSFREE_TYPE = data.TRANSFREE_TYPE,
                           GETOUT_DOC_PATH = data.GETOUT_DOC_PATH,
                           GETIN_DOC_PATH = data.GETIN_DOC_PATH
                       }).OrderBy(O => O.TRAN_NO).ToList();

            foreach (var i in obj)
            {
                if (i.TRANSFREE_TYPE == "Vendor")
                {
                    i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                    i.TRANSFREE = i.TRANSFREE + "-" + GetVendorName(i.TRANSFREE);
                }
                else
                {
                    i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                    i.TRANSFREE = i.TRANSFREE + "-" + GetEmpName(int.Parse(i.TRANSFREE));
                }
            }

            return obj;
        }

        public short UpdateFinanceApproval(string remarks, int LoginCode, int TranNo, string Status)
        {
            try
            {
                _atdb.Database.BeginTransaction();
                short retVal = 0;
                var _db = _atdb;
                //using (var _db = new LCEntities())
                //{
                AT_ASSSET_TRANSFER_HEADER hd_ = new AT_ASSSET_TRANSFER_HEADER();
                AT_APPROVAL_AUTHORITY appauth = new AT_APPROVAL_AUTHORITY();
                appauth = _db.AT_APPROVAL_AUTHORITY.Where(v => v.TRAN_NO == TranNo && v.EMP_CODE == LoginCode && (v.DEPARTMENT == "Finance Approval" || v.DEPARTMENT == "Finance")).FirstOrDefault();
                var nextno = appauth.SRNO + 1;
                var appauth1 = _db.AT_APPROVAL_AUTHORITY.Where(v => v.SRNO == nextno && v.APPROVAL_STATUS == "-").FirstOrDefault();
                if (Status == "Approved")
                {
                    if (appauth != null)
                    {
                        appauth.REMARKS = remarks;
                        appauth.APPROVAL_STATUS = Status;
                        appauth.UPDATEDBY = long.Parse(LoginCode.ToString());
                        appauth.UPDATEDON = DateTime.Now;

                        _db.Entry(appauth).State = EntityState.Modified;
                        _db.SaveChanges();

                        retVal = 1;
                    }
                    if (appauth1 != null)
                    {

                        appauth1.APPROVAL_STATUS = "Pending";
                        appauth1.REMARKS = " ";
                        appauth1.UPDATEDBY = long.Parse(LoginCode.ToString());
                        appauth1.UPDATEDON = DateTime.Now;

                        _db.Entry(appauth1).State = EntityState.Modified;
                        _db.SaveChanges();

                        var Header = _db.AT_ASSSET_TRANSFER_HEADER.Where(h => h.TRAN_NO == TranNo).FirstOrDefault();
                        hd_ = Header;
                        if (appauth1.DEPARTMENT == "User Approval")
                        {
                            Header.STATUS = "Pending At Approve";
                            hd_ = Header;
                            _db.Entry(Header).State = EntityState.Modified;
                            _db.SaveChanges();
                        }

                        if (appauth1.DEPARTMENT == "Finance")
                        {
                            decimal seqno = decimal.Parse("3.0");
                            var financesequence = _atdb.AT_COMMON_APPROVAL_MASTER.Where(x => x.ECODE == appauth.EMP_CODE).Select(x => x.SEQUENCE_NO).FirstOrDefault();

                            if (financesequence == seqno)
                            {

                                Header.STATUS = "Pending At Finance TM";

                                _db.Entry(Header).State = EntityState.Modified;
                                _db.SaveChanges();
                            }
                        }

                        retVal = 1;
                        SendMailByApprovalAuthority(hd_, appauth1.EMP_CODE, Status);

                    }
                    LibResult re = InsertIntoAppAuthLog(appauth);
                    var getfinanceCount = _db.AT_APPROVAL_AUTHORITY.Where(x => x.TRAN_NO == TranNo && x.DEPARTMENT == "Finance Approval").Count();
                    //bool CreatePdf = false;
                    var getfinanceApproveCount = _db.AT_APPROVAL_AUTHORITY.Where(x => x.TRAN_NO == TranNo && x.DEPARTMENT == "Finance Approval" && x.APPROVAL_STATUS == "Approved").Count();
                    if (getfinanceCount == getfinanceApproveCount)
                    {
                        var UpdateheadStatus = _db.AT_ASSSET_TRANSFER_HEADER.Where(x => x.TRAN_NO == TranNo).FirstOrDefault();
                        if (UpdateheadStatus != null)
                        {
                            UpdateheadStatus.STATUS = "Pending At GateOutStamp";
                            _db.Entry(UpdateheadStatus).State = EntityState.Modified;
                            _db.SaveChanges();

                            //CreatePdf = true;
                            retVal = 2;
                        }
                    }


                }

                if (Status == "Sent Back")
                {
                    appauth.REMARKS = remarks;
                    appauth.APPROVAL_STATUS = Status;
                    appauth.UPDATEDBY = long.Parse(LoginCode.ToString());
                    appauth.UPDATEDON = DateTime.Now;

                    _db.Entry(appauth).State = EntityState.Modified;
                    _db.SaveChanges();
                    LibResult res = InsertIntoAppAuthLog(appauth);

                    var Header = _db.AT_ASSSET_TRANSFER_HEADER.Where(h => h.TRAN_NO == TranNo).FirstOrDefault();
                    Header.STATUS = "Initiate";
                    _db.Entry(Header).State = EntityState.Modified;
                    _db.SaveChanges();

                    var authorityUpdate = _db.AT_APPROVAL_AUTHORITY.Where(h => h.TRAN_NO == TranNo).ToList();
                    foreach (var data in authorityUpdate)
                    {
                        data.APPROVAL_STATUS = "-";
                        data.REMARKS = " ";
                        _db.Entry(data).State = EntityState.Modified;
                        _db.SaveChanges();
                    }



                }
                if (Status == "Rejected")
                {
                    appauth.REMARKS = remarks;
                    appauth.APPROVAL_STATUS = Status;
                    appauth.UPDATEDBY = long.Parse(LoginCode.ToString());
                    appauth.UPDATEDON = DateTime.Now;

                    _db.Entry(appauth).State = EntityState.Modified;
                    _db.SaveChanges();
                    LibResult res = InsertIntoAppAuthLog(appauth);

                    var Header = _db.AT_ASSSET_TRANSFER_HEADER.Where(h => h.TRAN_NO == TranNo).FirstOrDefault();
                    Header.STATUS = Status;
                    _db.Entry(Header).State = EntityState.Modified;
                    _db.SaveChanges();
                }

                _atdb.Database.CommitTransaction();
                //}
                return retVal;

            }
            catch (Exception ex)
            {
                _atdb.Database.RollbackTransaction();
                throw ex;
            }
        }


        #endregion

        #region  final finance approve

        public List<AT_AssetTransferApprovalViewModel> GetFinalFinanceApprovalList(int LoginCode, string flag)
        {
            var obj = (from data in _atdb.AT_ASSSET_TRANSFER_HEADER
                       join a in _atdb.AT_APPROVAL_AUTHORITY on data.TRAN_NO equals a.TRAN_NO
                       where a.EMP_CODE == LoginCode && a.DEPARTMENT == flag
                       && a.APPROVAL_STATUS == "Pending"
                       select new AT_AssetTransferApprovalViewModel
                       {
                           SRNO = data.SRNO,
                           TRAN_NO = data.TRAN_NO,
                           TRAN_TYPE = data.TRAN_TYPE,
                           ASSET_TYPE = data.ASSET_TYPE,
                           TRAN_DATE = data.TRAN_DATE,
                           TRANSFEROR = data.TRANSFEROR,
                           TRANSFREE = data.TRANSFREE,
                           TOTAL = data.TOTAL,
                           APPAUTHSRNO = a.SRNO,
                           APPROVAL_STATUS = a.APPROVAL_STATUS,
                           HEADERSTATUS = data.STATUS,
                           PDFPATH = data.PDFPATH,
                           TRANSFREE_TYPE = data.TRANSFREE_TYPE,
                           GETOUT_DOC_PATH = data.GETOUT_DOC_PATH,
                           GETIN_DOC_PATH = data.GETIN_DOC_PATH
                       }).OrderBy(O => O.TRAN_NO).ToList();

            foreach (var i in obj)
            {

                if (i.TRANSFREE_TYPE == "Vendor")
                {
                    i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                    i.TRANSFREE = i.TRANSFREE + "-" + GetVendorName(i.TRANSFREE);
                }
                else
                {
                    i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                    i.TRANSFREE = i.TRANSFREE + "-" + GetEmpName(int.Parse(i.TRANSFREE));
                }
            }

            return obj;
        }


        #endregion

        #region Upload
        public string GetEmpName(int Emp)
        {
            var N = (from data in _atdb.ADEMPLOYEE_LC.Where(v => v.ADEMPCODE == Emp)
                     select
                         data.FIRSTNAME + " " + data.LASTNAME
                     ).FirstOrDefault();

            return N;
        }
        public string GetVendorName(string Emp)
        {
            var N = (from data in _atdb.FINVENDORMASTERMST_LC.Where(v => v.VENDORCODE == Emp)
                     select
                         data.NAME1
                     ).FirstOrDefault();

            return N;
        }

        public LibResult UploadFile(int TranNo, string filepath, long EmpCode)
        {
            try
            {

            LibResult res = new LibResult();
            _atdb.Database.BeginTransaction();
            var GetOutStamp = _atdb.AT_ASSSET_TRANSFER_HEADER.Where(a => a.TRAN_NO == TranNo).FirstOrDefault();

            if (GetOutStamp != null)
            {
                GetOutStamp.GETOUT_DOC_PATH = Path.GetFileName(filepath);
                GetOutStamp.GETIN_DOC_PATH = "Test";
                GetOutStamp.STATUS = "Pending At Security";
                //GetOutStamp.PDFPATH = "-";
                _atdb.Entry(GetOutStamp).State = EntityState.Modified;
                _atdb.SaveChanges();
            }
            if (GetOutStamp.STATUS == "Pending At Security")
            {
                //using (var db = new LCEntities())
                //{
                var Initiater = _atdb.AT_APPROVAL_AUTHORITY.Where(p => p.DEPARTMENT == "GateOutStamp" && p.TRAN_NO == TranNo).FirstOrDefault();
                if (Initiater != null)
                {
                    Initiater.APPROVAL_STATUS = "Approved";
                    Initiater.REMARKS = " ";
                    Initiater.UPDATEDBY = long.Parse(EmpCode.ToString());
                    Initiater.UPDATEDON = DateTime.Now;

                    _atdb.Entry(Initiater).State = EntityState.Modified;
                    _atdb.SaveChanges();
                    LibResult re = InsertIntoAppAuthLog(Initiater);
                }

                var statusToUpdate = _atdb.AT_APPROVAL_AUTHORITY.Where(p => p.DEPARTMENT == "Security" && p.TRAN_NO == TranNo).FirstOrDefault();

                if (statusToUpdate != null)
                {
                    statusToUpdate.APPROVAL_STATUS = "Pending";
                    statusToUpdate.REMARKS = " ";
                    statusToUpdate.UPDATEDBY = long.Parse(EmpCode.ToString());
                    statusToUpdate.UPDATEDON = DateTime.Now;

                    _atdb.Entry(statusToUpdate).State = EntityState.Modified;
                    _atdb.SaveChanges();
                }
                SendMailByApprovalAuthority(GetOutStamp, statusToUpdate.EMP_CODE, Initiater.APPROVAL_STATUS);

                //}
            }
                _atdb.Database.CommitTransaction();
                return res;

            }
            catch (Exception)
            {
                _atdb.Database.RollbackTransaction();
                throw;
            }
        }

        public LibResult GetinstampUpload(int TranNo, string filepath, long EmpCode)
        {
            LibResult res = new LibResult();

            var GetInStamp = _atdb.AT_ASSSET_TRANSFER_HEADER.Where(a => a.TRAN_NO == TranNo).FirstOrDefault();

            if (GetInStamp != null)
            {
                //GetOutStamp.GETOUT_DOC_PATH = Path.GetFileName(filepath);
                GetInStamp.GETIN_DOC_PATH = Path.GetFileName(filepath);
                //GetInStamp.STATUS = "Completed";
                //GetOutStamp.PDFPATH = "-";
                _atdb.Entry(GetInStamp).State = EntityState.Modified;
                _atdb.SaveChanges();
            }
            //if (GetInStamp.STATUS == "Completed")
            //{
            //    using (var db = new LCEntities())
            //    {
            //        var Initiater = db.AT_APPROVAL_AUTHORITY.Where(p => p.DEPARTMENT == "GateInStamp" && p.TRAN_NO == TranNo).FirstOrDefault();
            //        if (Initiater != null)
            //        {
            //            Initiater.APPROVAL_STATUS = "Approve";
            //            Initiater.REMARKS = "-";
            //            Initiater.UPDATEDBY = long.Parse(EmpCode.ToString());
            //            Initiater.UPDATEDON = DateTime.Now;

            //            db.Entry(Initiater).State = EntityState.Modified;
            //            db.SaveChanges();

            //        }

            //LibResult re = InsertIntoAppAuthLog(Initiater);
            //}
            //}

            return res;
        }

        public LibResult InsertIntoAppAuthLog(AT_APPROVAL_AUTHORITY at)
        {
            LibResult res = new LibResult();

            //using (var db = new LCEntities()) //Open the Comment When we create Approval auth log in sql
            //{
            AT_APPROVAL_AUTHORITY_LOG _AT = new AT_APPROVAL_AUTHORITY_LOG();

            var Srno = _atdb.AT_APPROVAL_AUTHORITY_LOG.Select(x => x.SRNO).DefaultIfEmpty().Max();
            _AT.SRNO = Srno + 1;
            _AT.TRAN_NO = at.TRAN_NO;
            _AT.EMP_CODE = at.EMP_CODE;
            _AT.APPROVAL_STATUS = at.APPROVAL_STATUS;
            _AT.EMP_NAME = at.EMP_NAME;
            _AT.DEPARTMENT = at.DEPARTMENT;
            _AT.DESIGNATION = at.DESIGNATION;
            if (at.REMARKS == "")
            {
                _AT.REMARKS = " ";
            }
            else
            {
                _AT.REMARKS = at.REMARKS;
            }
            _AT.ADDEDBY = at.ADDEDBY;
            _AT.ADDEDON = at.ADDEDON;
            _AT.UPDATEDBY = at.UPDATEDBY;
            _AT.UPDATEDON = at.UPDATEDON;

            _atdb.Entry(_AT).State = EntityState.Added;
            _atdb.SaveChanges();
            //}

            return res;
        }

        public List<object> GetAppAuthLogHistory(int TranNo)
        {
            LibResult res = new LibResult();
            //var ilist = _atdb.AT_APPROVAL_AUTHORITY_LOG.Where(x => x.TRAN_NO == TranNo).ToList(); //Open the Comment When we create Approval auth log in sql
            ////var ilist = _atdb.AT_APPROVAL_AUTHORITY_LOG.Where(x => x.TRAN_NO == TranNo).ToList();
            //res.resultObject = ilist;

            //return res;
            var logs = _atdb.AT_APPROVAL_AUTHORITY_LOG
                           .Where(x => x.TRAN_NO == TranNo)
                           .OrderBy(x => x.SRNO)
                           .ToList();

            var authorities = _atdb.AT_APPROVAL_AUTHORITY
                                   .Where(x => x.TRAN_NO == TranNo && x.APPROVAL_STATUS == "Pending")
                                   .OrderBy(x => x.SRNO)
                                   .ToList();

            var combinedList = logs.Cast<object>().Union(authorities.Cast<object>()).ToList();


            return combinedList;

        }



        public List<AT_APPROVAL_AUTHORITY_LOG> History(int LoginCode, int TranNo)
        {
            try
            {
                List<AT_APPROVAL_AUTHORITY_LOG> logs = _atdb.AT_APPROVAL_AUTHORITY_LOG
                                   .Where(x => x.TRAN_NO == TranNo)
                                   .OrderBy(x => x.SRNO)
                                   .ToList();

                List<AT_APPROVAL_AUTHORITY> authorities = _atdb.AT_APPROVAL_AUTHORITY
                                      .Where(x => x.TRAN_NO == TranNo && x.APPROVAL_STATUS == "Pending")
                                       //  .Where(x => x.TRAN_NO == TranNo && (x.APPROVAL_STATUS == "Pending" || x.APPROVAL_STATUS == "-")) ////- Modified by Vishal Saini on 25-10-24
                                       .OrderBy(x => x.SRNO)
                                       .ToList();


                List<AT_APPROVAL_AUTHORITY_LOG> logsAuth = new List<AT_APPROVAL_AUTHORITY_LOG>();


                foreach (var item in authorities)
                {
                    AT_APPROVAL_AUTHORITY_LOG a = new AT_APPROVAL_AUTHORITY_LOG();

                    a.EMP_CODE = item.EMP_CODE;
                    a.EMP_NAME = item.EMP_NAME;
                    a.DEPARTMENT = item.DEPARTMENT;
                    a.DESIGNATION = item.DESIGNATION;
                    a.APPROVAL_STATUS = item.APPROVAL_STATUS;
                    a.ADDEDBY = item.ADDEDBY;
                    a.ADDEDON = item.ADDEDON;
                    a.UPDATEDBY = item.UPDATEDBY;
                    if (a.APPROVAL_STATUS != "Pending" || a.APPROVAL_STATUS != "-") ////- Modified by Vishal Saini on 25-10-24
                    {
                        a.UPDATEDON = item.UPDATEDON;
                    }

                    a.REMARKS = " ";
                    a.TRAN_NO = item.TRAN_NO;

                    logsAuth.Add(a);

                }



                var combinedList = logs.Cast<AT_APPROVAL_AUTHORITY_LOG>().Union(logsAuth.Cast<AT_APPROVAL_AUTHORITY_LOG>()).ToList();

                return combinedList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        #endregion

        #region Transfree Type

        public LibResult GetNewLocation(string Key, string TransfreeType)
        {

            LibResult res = new LibResult();
            if (TransfreeType == "HMSI" || TransfreeType == "Other")
            {
                var HmsiSiteList = (from h in _atdb.SYSITE_LC
                                    where h.ACTIVE == 1
                                    select new
                                    {
                                        text = h.DESCRIP,
                                        value = h.DESCRIP,
                                        value1 = h.SITEADDRESS
                                    }

                             ).ToList();


                //var HmsiSiteList = (from i in _atdb.SYSITE
                //                    where i.ACTIVE == 1
                //                    select new { i.DESCRIP, i.SITEADDRESS }
                //                                        ).Where(v =>
                //                       !string.IsNullOrEmpty(v.DESCRIP) &&
                //                       !string.IsNullOrEmpty(v.SITEADDRESS)
                //                    ).ToList();

                var HmsiUser = (from r in _atdb.ADEMPLOYEE_LC
                                where (r.ADEMPCODE.ToString().ToUpper().Contains(Key.ToUpper()) ||
                                       r.FIRSTNAME.ToString().ToUpper().Contains(Key.ToUpper()) ||
                                       r.LASTNAME.ToString().ToUpper().Contains(Key.ToUpper()))
                                        && (r.ACTIVE == 1)

                                select new
                                {
                                    value = r.ADEMPCODE,
                                    text = r.FIRSTNAME + " " + r.LASTNAME

                                }).Take(500).ToList();

                var Data = new { USERLIST = HmsiUser, LocationList = HmsiSiteList, VendorList = "" };
                res.resultObject = Data;
            }

            else if (TransfreeType == "Vendor")
            {
                //var Vendor = (from i in _atdb.FINVENDORMASTERMST
                //              where
                //                (!string.IsNullOrEmpty(i.VENDORCODE).ToString().ToUpper().Contains(Key.ToUpper()) ||
                //                !string.IsNullOrEmpty(i.NAME1).ToString().ToUpper().Contains(Key.ToUpper()))
                //              select new { i.VENDORCODE, i.NAME1, i.STREET1,i.STREET2 , i.STREET3,i.STREET4, i.CITY 
                //              }).Where(v =>
                //                        !string.IsNullOrEmpty(v.STREET1) &&
                //                        !string.IsNullOrEmpty(v.STREET2) &&
                //                        !string.IsNullOrEmpty(v.STREET3) &&
                //                        !string.IsNullOrEmpty(v.STREET4) &&
                //                        !string.IsNullOrEmpty(v.CITY)
                //                     ).Take(500).ToList();

                var Vendor = (from i in _atdb.FINVENDORMASTERMST_LC.Where(x => x.VENDORCODE.Contains(Key) || x.NAME1.Contains(Key))
                              where i.STATUS == 1
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
                                  i.CITY,
                                  i.GSTIN,
                                  i.PANNO
                              }).Take(200).ToList();

                var Data = new { USERLIST = "", LocationList = "", VendorList = Vendor };

                res.resultObject = Data;

            }

            return res;
        }


        #endregion

        #region CommanApproval Master

        public LibResult AutocomplitName(int Ecode)
        {
            LibResult res = new LibResult();
            try
            {
                //using (var db = new LCEntities())
                //{
                var N = (from data in _atdb.ADEMPLOYEE_LC.Where(v => v.ADEMPCODE == Ecode && v.ACTIVE == 1)
                         select
                             data.FIRSTNAME + " " + data.LASTNAME
                 ).FirstOrDefault();

                var name = Ecode + "-" + N;


                res.resultObject = name;
                //}
            }
            catch (Exception ex)
            {
                res.hasError = true;
                res.errorMessage = ex.ToString();
            }
            return res;
        }

        public List<AT_COMMON_APPROVAL_MASTER> GetCommanApprovalList()
        {
            try
            {
                List<AT_COMMON_APPROVAL_MASTER> ilist = new List<AT_COMMON_APPROVAL_MASTER>();
                ilist = _atdb.AT_COMMON_APPROVAL_MASTER.OrderBy(x => x.SYSITEiD).ToList();
                return ilist;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public LibResult BindPlantName()
        {
            LibResult Res = new LibResult();
            try
            {
                //using (var db = new LCEntities())
                //{
                var Data = (from r in _atdb.SYSITE_LC
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

        public AT_COMMON_APPROVAL_MASTER AT_CommanApprovalEdit(int id)
        {

            try
            {
                var _obj = (from data in _atdb.AT_COMMON_APPROVAL_MASTER.Where(v => v.SRNO == id)
                            select data).FirstOrDefault();



                return _obj;

            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        public bool CommanApprovalDelete(int id)
        {
            bool res = false;
            AT_COMMON_APPROVAL_MASTER AT = _atdb.AT_COMMON_APPROVAL_MASTER.Find(id);
            _atdb.AT_COMMON_APPROVAL_MASTER.Remove(AT);
            _atdb.SaveChanges();
            res = true;
            return res;
        }

        public LibResult GetCommanApprovalEdit(int srno, decimal Sysiteid, string Department, int ECode, decimal Sequenceno, int LoginCode)
        {
            LibResult res = new LibResult();
            try
            {

                //using (var DB_ = new LCEntities())
                //{
                var Exist = _atdb.AT_COMMON_APPROVAL_MASTER.Where(x => x.SYSITEiD == Sysiteid
                                          && x.SRNO == srno
                                         ).Count();
                AT_COMMON_APPROVAL_MASTER at_comman = _atdb.AT_COMMON_APPROVAL_MASTER.Find(srno);

                var sykiid = (from s in _atdb.SYKI_LC
                              where s.ACTIVE == 1
                              select s.SYKIID).FirstOrDefault();

                var designation = (from emp1 in _atdb.ADEMPLOYEE_LC.Where(v => v.ADEMPCODE == ECode && v.ACTIVE == 1)
                                   join _VW in _atdb.VW_ASSOCIATELVLDETAILS_LC on emp1.ADEMPCODE equals _VW.ADEMPCODE
                                   join _Desg in _atdb.ADDESIGNATION_LC on _VW.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
                                   where _VW.SYKI == sykiid
                                   select (_Desg.DESCRIP)).FirstOrDefault();

                at_comman.DEPARTMENT = Department;
                at_comman.ECODE = ECode;
                at_comman.DESIGNATION = designation;
                at_comman.SEQUENCE_NO = Sequenceno;
                at_comman.UPDATEDBY = LoginCode;
                at_comman.UPDATEDON = DateTime.Now;

                _atdb.Entry(at_comman).State = EntityState.Modified;
                _atdb.SaveChanges();


                res.hasError = false;
                //}

            }

            catch (Exception ex)
            {
                res.hasError = true;
                res.errorMessage = ex.ToString();

            }
            return res;
        }

        public LibResult GetCommanApprovalSave(int srno, int Sysiteid, string Department, int ECode, decimal Sequenceno, int LoginCode, string Mode)
        {
            LibResult res = new LibResult();
            try
            {
                if (Mode == "ADD")
                {
                    //using (var DB_ = new LCEntities())
                    //{
                    var Exist = _atdb.AT_COMMON_APPROVAL_MASTER.Where(x => x.SYSITEiD == Sysiteid
                                              && x.DEPARTMENT == Department
                                              && x.ECODE == ECode
                                              && x.SEQUENCE_NO == Sequenceno
                                             ).Count();
                    if (Exist > 0)
                    {
                        res.hasError = true;
                        res.errorMessage = "record alredy exist...";
                    }
                    else
                    {
                        var maxSrno = _atdb.AT_COMMON_APPROVAL_MASTER.Select(x => (int?)x.SRNO).Max() ?? 0;
                        var sykiid = (from s in _atdb.SYKI_LC
                                      where s.ACTIVE == 1
                                      select s.SYKIID).FirstOrDefault();

                        var designation = (from emp1 in _atdb.ADEMPLOYEE_LC.Where(v => v.ADEMPCODE == ECode && v.ACTIVE == 1)
                                           join _VW in _atdb.VW_ASSOCIATELVLDETAILS_LC on emp1.ADEMPCODE equals _VW.ADEMPCODE
                                           join _Desg in _atdb.ADDESIGNATION_LC on _VW.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
                                           where _VW.SYKI == sykiid
                                           select (_Desg.DESCRIP)).FirstOrDefault();

                        var Sitedes = _atdb.SYSITE_LC.Where(x => x.SYSITEID == Sysiteid).Select(x => x.DESCRIP).FirstOrDefault();

                        AT_COMMON_APPROVAL_MASTER at_comman = new AT_COMMON_APPROVAL_MASTER
                        {
                            SRNO = maxSrno + 1,
                            DEPARTMENT = Department,
                            SYSITEiD = Sysiteid,
                            ECODE = ECode,
                            DESIGNATION = designation,
                            SEQUENCE_NO = Sequenceno,
                            ADDEDBY = LoginCode,
                            ADDEDON = DateTime.Now,
                            SYSITTDESCRIPTION = Sitedes

                        };

                        _atdb.AT_COMMON_APPROVAL_MASTER.Add(at_comman);
                        _atdb.SaveChanges();


                    }
                    res.hasError = false;
                    // }
                }
                else
                {
                    //using (var DB_ = new LCEntities())
                    //{
                    var Exist = _atdb.AT_COMMON_APPROVAL_MASTER.Where(x => x.SYSITEiD == Sysiteid
                                            && x.DEPARTMENT == Department
                                              && x.ECODE == ECode
                                              && x.SEQUENCE_NO == Sequenceno
                                             ).Count();
                    if (Exist > 1)
                    {
                        res.hasError = true;
                        res.errorMessage = "record alredy exist...";
                    }
                    else
                    {
                        AT_COMMON_APPROVAL_MASTER at_comman = _atdb.AT_COMMON_APPROVAL_MASTER.Find(srno);

                        at_comman.DEPARTMENT = Department;
                        at_comman.ECODE = ECode;
                        at_comman.SEQUENCE_NO = Sequenceno;

                        _atdb.Entry(at_comman).State = EntityState.Modified;
                        _atdb.SaveChanges();


                    }
                    res.hasError = false;
                    //}
                }


            }

            catch (Exception ex)
            {
                res.hasError = true;
                res.errorMessage = ex.ToString();

            }
            return res;
        }

        public List<AT_COMMON_APPROVAL_MASTER> GetCommanApprovalSearch(string Department)
        {
            try
            {
                List<AT_COMMON_APPROVAL_MASTER> ilist = new List<AT_COMMON_APPROVAL_MASTER>();
                ilist = _atdb.AT_COMMON_APPROVAL_MASTER.Where(d => d.DEPARTMENT == Department || Department == "All").OrderBy(x => x.SYSITEiD).ToList();
                return ilist;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region asset Attach

        public bool UploadFileAndSave(int TranNo, string AssetCode, string file)
        {
            bool res = false;
            try
            {

                AT_ASSET_TRANSFER_DETAIL dtRecord = _atdb.AT_ASSET_TRANSFER_DETAIL.FirstOrDefault(i => i.SUBAUCCODE == AssetCode && i.TRAN_NO == TranNo);

                if (dtRecord != null)
                {
                    dtRecord.ASSET_ATTACH = file.ToString();
                    dtRecord.NEW_ASSET_CODE = "";

                    _atdb.Entry(dtRecord).State = EntityState.Modified;
                    _atdb.SaveChanges();
                    res = true;
                }
                else
                {

                    res = false;
                }

            }
            catch (Exception ex)
            {

                return res;
            }
            return res;
        }

        public bool UploadFileAndSaveCap(int TranNo, string AssetCode, string file)
        {
            bool res = false;
            try
            {

                AT_ASSET_TRANSFER_DETAIL dtRecord = new AT_ASSET_TRANSFER_DETAIL();


                dtRecord = _atdb.AT_ASSET_TRANSFER_DETAIL.FirstOrDefault(i => i.ASSET_CODE == AssetCode && i.TRAN_NO == TranNo);

                //if(dtRecord == null)
                //{
                //    int asset = Convert.ToInt32(AssetCode);
                //    dtRecord = _atdb.AT_ASSET_TRANSFER_DETAIL.FirstOrDefault(i => i.SRNO == asset && i.TRAN_NO == TranNo);
                //}


                if (dtRecord != null)
                {
                    dtRecord.ASSET_ATTACH = file.ToString();
                    dtRecord.NEW_ASSET_CODE = "";

                    _atdb.Entry(dtRecord).State = EntityState.Modified;
                    _atdb.SaveChanges();
                    res = true;
                }
                else
                {

                    res = false;
                }

            }
            catch (Exception ex)
            {

                return res;
            }
            return res;
        }


        #endregion

        #region Mail
        public void SendMailByApprovalAuthority(AT_ASSSET_TRANSFER_HEADER hd, long NextApprovalUser, string approvalStatus)
        {

            string transfree = hd.TRANSFREE;
            long transferor = long.Parse(hd.TRANSFEROR);
            if (hd.TRANSFREE_TYPE == "Vendor")
            {
                FINVENDORMASTERMST_LC Transfree = _atdb.FINVENDORMASTERMST_LC.Where(x => x.VENDORCODE == transfree).FirstOrDefault();

            }
            //ADEMPLOYEE Transfree = _atdb.ADEMPLOYEE.Where(x => x.ADEMPCODE == transfree && x.ACTIVE == 1).FirstOrDefault();
            ADEMPLOYEE_LC Transferor = _atdb.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == transferor && x.ACTIVE == 1).FirstOrDefault();
            ADEMPLOYEE_LC NextApproval = _atdb.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == NextApprovalUser && x.ACTIVE == 1).FirstOrDefault();

            string req_type = hd.ASSET_TYPE + " " + hd.TRAN_TYPE;
            try
            {

                if (approvalStatus == "Approved")
                {
                    #region Send mail next Approval Authority

                    commanEmail NextsendMail = new commanEmail();
                    NextsendMail.MailFrom = "portal.admin@honda.hmsi.in";

                    NextsendMail.MailTo = NextApproval.EMAILID.ToString();
                    //NextsendMail.MailTo = "vishal.saini@honda.hmsi.in";
                    //.MailTo = "Ashok.Bisnoi@honda.hmsi.in";

                    string strSubject_ = "Asset Transfer Request from - " + Transferor.FIRSTNAME.ToString() + " " + Transferor.LASTNAME.ToString() + ", Employee Code - " + Transferor.ADEMPCODE.ToString();
                    string strBody_ = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Asset Transfer Request from " + Transferor.FIRSTNAME.ToString() + " " + Transferor.LASTNAME.ToString() + " - Emp Code (" + Transferor.ADEMPCODE.ToString() + ")</font></b></td></tr>" +
                                      "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + NextApproval.FIRSTNAME.ToString() + "  San ,</br></br></td></tr><tr><td colspan=2 width=514 valign=top>" + Transferor.FIRSTNAME.ToString() + " San has raised a Asset Transfer Approval Request in Employee Portal. Below are the details :</td></tr>" +
                                    "<tr><td width=125 height=22 valign=top>Request Type :</td><td width=850 valign=top>" + req_type + "</td></tr>" +
                                     "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "> Employee Portal</a> link to approve the request.</td></tr>" +
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
                if (approvalStatus == "Sent Back" || approvalStatus == "Rejected")
                {
                    #region Send mail next approval authority

                    commanEmail NextsendMail = new commanEmail();
                    NextsendMail.MailFrom = "portal.admin@honda.hmsi.in";

                    NextsendMail.MailTo = NextApproval.EMAILID.ToString();
                    //NextsendMail.MailTo = "vishal.saini@honda.hmsi.in";
                    //NextsendMail.MailTo = "Ashok.Bisnoi@honda.hmsi.in";


                    string strSubject_ = "Asset Transfer Request from - " + Transferor.FIRSTNAME.ToString() + " " + Transferor.LASTNAME.ToString() + ", Employee Code - " + Transferor.ADEMPCODE.ToString();
                    string strBody_ = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Asset Transfer Request from " + Transferor.FIRSTNAME.ToString() + " " + Transferor.LASTNAME.ToString() + " - Emp Code (" + Transferor.ADEMPCODE.ToString() + ")</font></b></td></tr>" +
                                      "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + NextApproval.FIRSTNAME.ToString() + "  San ,</br></br></td></tr><tr><td colspan=2 width=514 valign=top>" + Transferor.FIRSTNAME.ToString() + " San has raised a Asset Transfer Approval Request in Employee Portal. Below are the details :</td></tr>" +
                                    "<tr><td width=125 height=22 valign=top>Request Type :</td><td width=850 valign=top>" + req_type + "</td></tr>" +
                                     "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "> Employee Portal</a> link to approve the request.</td></tr>" +
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


        #endregion


        #endregion

        #region NonCapitalize Asset

        public AT_ASSSET_TRANSFER_HEADER NonCapitalizedEdit(int id)
        {

            var _obj = (from data in _atdb.AT_ASSSET_TRANSFER_HEADER.Where(v => v.SRNO == id)
                        select data).FirstOrDefault();

            if (_obj.TRANSFREE_TYPE == "Vendor")
            {
                _obj.TRANSFEROR = _obj.TRANSFEROR + "-" + GetEmpName(int.Parse(_obj.TRANSFEROR));
                _obj.TRANSFREE = _obj.TRANSFREE + "-" + GetVendorName(_obj.TRANSFREE);
            }
            else
            {
                _obj.TRANSFEROR = _obj.TRANSFEROR + "-" + GetEmpName(int.Parse(_obj.TRANSFEROR));
                _obj.TRANSFREE = _obj.TRANSFREE + "-" + GetEmpName(int.Parse(_obj.TRANSFREE));
            }



            return _obj;
        }
        #endregion

        #region Asset Transfer Form
        public GetAssetFormHeader Get_AssetRegister_Header_Detail(long? UserID, long Tran_No, string filename)
        {
            try
            {

            GetAssetFormHeader g = new GetAssetFormHeader();

            var sykiid = (from s in _atdb.SYKI_LC
                          where s.ACTIVE == 1
                          select s.SYKIID).FirstOrDefault();

            var header = _atdb.AT_ASSSET_TRANSFER_HEADER.Where(x => x.TRAN_NO == Tran_No).FirstOrDefault();

            var HeadData = _atdb.AT_ASSSET_TRANSFER_HEADER.Where(x => x.TRAN_NO == Tran_No).Select(s => s.TRAN_TYPE).FirstOrDefault();

            var duration = _atdb.AT_ASSSET_TRANSFER_HEADER.Where(x => x.TRAN_NO == Tran_No).Select(s => s.DURATION).FirstOrDefault();
            if (duration == null)
            {
                duration = "";
            }

            var remarks = header.REMARKS;

            header.PDFPATH = filename;
            _atdb.Entry(header).State = EntityState.Modified;
            _atdb.SaveChanges();


            var Transfree = "";
            var Transferor = header.TRANSFEROR + " - " + GetEmpName(int.Parse(header.TRANSFEROR));

            if (header.TRANSFREE_TYPE == "Vendor")
            {
                Transfree = header.TRANSFREE + " - " + GetVendorName(header.TRANSFREE);
            }
            else
            {
                Transfree = header.TRANSFREE + " - " + GetEmpName(int.Parse(header.TRANSFREE));
            }


            var getheader = (from vw in _atdb.VW_ASSOCIATELVLDETAILS_LC
                             where vw.ADEMPCODE == header.REQ_ECODE && vw.ACTIVE == 1 && vw.SYKI == sykiid
                             select new
                             {
                                 fd = vw.FUNCTIONALDESIGNATION,
                                 Section = vw.SECTION,
                                 Department = vw.DEPARTMENT,
                                 Division = vw.DIVISION,
                                 Operation = vw.OPERATION
                             }).FirstOrDefault();
            var concatenatedString = "";
            if (getheader != null)
            {

                if (getheader.fd == "Section Head")
                {
                    concatenatedString = string.Join(" / ",
                  new[] { getheader.Section, getheader.Department, getheader.Division, getheader.Operation }
                  .Where(x => x != null));
                }
                else if (getheader.fd == "Department Head")
                {
                    concatenatedString = string.Join(" / ",
                  new[] { getheader.Department, getheader.Division, getheader.Operation }
                  .Where(x => x != null));
                }
                else if (getheader.fd == "Division Head")
                {
                    concatenatedString = string.Join(" / ",
                  new[] { getheader.Division, getheader.Operation }
                  .Where(x => x != null));
                }
                else if (getheader.fd == "Operating Head")
                {
                    concatenatedString = string.Join(" / ",
                  new[] { getheader.Division, getheader.Operation }
                  .Where(x => x != null));
                }
                else
                {
                    concatenatedString = string.Join(" / ",
                  new[] { getheader.Section, getheader.Department, getheader.Division, getheader.Operation }
                  .Where(x => x != null));
                }
                g.Designation = concatenatedString;
            }

            var plantcode = (from w in _atdb.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == header.REQ_ECODE && x.ACTIVE == 1 && x.SYKI == sykiid)
                             join a in _atdb.SYSITE_LC on w.SYSITEID equals a.SYSITEID
                             select a.DESCRIP).FirstOrDefault();
            if (plantcode != null)
            {
                g.Plant = plantcode;
            }


            var get = new GetAssetFormHeader
            {
                Tran_Type = HeadData,
                Plant = plantcode,
                Designation = concatenatedString,
                Transferor = Transferor,
                Transferee = Transfree,
                Remarks = remarks,
                Duration = duration

            };

            return get;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool filesave(string filename, long Tran_No)
        {
            bool res = false;

            var header = _atdb.AT_ASSSET_TRANSFER_HEADER.Where(x => x.TRAN_NO == Tran_No).FirstOrDefault();


            header.PDFPATH = filename;
            _atdb.Entry(header).State = EntityState.Modified;
            _atdb.SaveChanges();
            res = true;

            return res;
        }

        public List<AT_ASSET_TRANSFER_DETAIL> DetailRecord(long? UserID, long Tran_No)
        {
            try
            {
                List<AT_ASSET_TRANSFER_DETAIL> Dt = new List<AT_ASSET_TRANSFER_DETAIL>();

                Dt = _atdb.AT_ASSET_TRANSFER_DETAIL.Where(x => x.TRAN_NO == Tran_No).ToList();

                return Dt;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public GetAuthDetail GetAuthorityDetail(long? UserID, long Tran_No)
        {
            try
            {
                GetAuthDetail at = new GetAuthDetail();

                var sykiid = (from s in _atdb.SYKI_LC
                              where s.ACTIVE == 1
                              select s.SYKIID).FirstOrDefault();

                var Auth = _atdb.AT_APPROVAL_AUTHORITY.Where(x => x.TRAN_NO == Tran_No).ToList();

                var header = _atdb.AT_ASSSET_TRANSFER_HEADER.Where(x => x.TRAN_NO == Tran_No).FirstOrDefault();
                decimal seqno = decimal.Parse("3.0");
                var Common = _atdb.AT_COMMON_APPROVAL_MASTER.Where(x => x.DEPARTMENT == "Finance" && x.SEQUENCE_NO == seqno).ToList();

                at.Preparedbydate = header.ADDEDON;

                var taxationinvoice = _atdb.AT_APPROVAL_AUTHORITY.Where(x => x.TRAN_NO == Tran_No).Select(x => x.SRNO).DefaultIfEmpty().Min();

                foreach (var data in Auth)
                {
                    if (data.DEPARTMENT == "Taxation" && data.SRNO == taxationinvoice)
                    {
                        if (data.REMARKS == null)
                        {
                            at.Remarks = " ";
                        }
                        else
                        {
                            at.Remarks = data.REMARKS;
                        }

                    }

                    if (data.DESIGNATION == "Department Head" && data.DEPARTMENT == "User Approval")
                    {
                        at.DepartmentHead = data.EMP_NAME + "-" + data.EMP_CODE.ToString();
                        at.DepartmentHeaddate = data.UPDATEDON;
                    }
                    if (data.DESIGNATION == "Division Head" && data.DEPARTMENT == "User Approval")
                    {
                        at.DivisionHead = data.EMP_NAME + "-" + data.EMP_CODE.ToString();
                        at.DivisionHeaddate = data.UPDATEDON;
                    }
                    if (data.DESIGNATION == "Operating Head" && data.DEPARTMENT == "User Approval")
                    {
                        at.OperatingHead = data.EMP_NAME + "-" + data.EMP_CODE.ToString();
                        at.OperatingHeaddate = data.UPDATEDON;
                    }
                    if (data.DESIGNATION == "Executive Vice President" && data.DEPARTMENT == "User Approval")
                    {
                        at.ExecutiveVicePresident = data.EMP_NAME + "-" + data.EMP_CODE.ToString();
                        at.ExecutiveVicePresidentdate = data.UPDATEDON;
                    }
                    var Dir = _atdb.AT_APPROVAL_AUTHORITY.Where(x => x.TRAN_NO == Tran_No && x.DESIGNATION == "Director").Count();

                    var Director = _atdb.AT_APPROVAL_AUTHORITY.Where(x => x.TRAN_NO == Tran_No && x.DESIGNATION == "Director").Select(x => x.SRNO).DefaultIfEmpty().Min();

                    if (Dir > 1)
                    {
                        var Director2 = _atdb.AT_APPROVAL_AUTHORITY.Where(x => x.TRAN_NO == Tran_No && x.DESIGNATION == "Director").Select(x => x.SRNO).DefaultIfEmpty().Max();

                        if (data.DESIGNATION == "Director" && data.DEPARTMENT == "User Approval" && data.SRNO == Director2)
                        {
                            at.Directortwo = data.EMP_NAME + "-" + data.EMP_CODE.ToString();
                            at.Directortwodate = data.UPDATEDON;
                        }
                    }

                    if (data.DESIGNATION == "Director" && data.DEPARTMENT == "User Approval" && data.SRNO == Director)
                    {
                        at.Director = data.EMP_NAME + "-" + data.EMP_CODE.ToString();
                        at.Directordate = data.UPDATEDON;
                    }


                    if (data.DESIGNATION == "Chief Production Officer & Director" && data.DEPARTMENT == "User Approval")
                    {
                        at.CPO = data.EMP_NAME + "-" + data.EMP_CODE.ToString();
                        at.CPOdate = data.UPDATEDON;
                    }
                    if (data.DESIGNATION == "Coordinator" && data.DEPARTMENT == "User Approval")
                    {
                        at.Coordinator = data.EMP_NAME + "-" + data.EMP_CODE.ToString();
                        at.Coordinatordate = data.UPDATEDON;
                    }
                    if (data.DESIGNATION == "Executive Coordinator" && data.DEPARTMENT == "User Approval")
                    {
                        at.ExecutiveCoordinator = data.EMP_NAME + "-" + data.EMP_CODE.ToString();
                        at.ExecutiveCoordinatordate = data.UPDATEDON;
                    }

                    if (data.DEPARTMENT == "Finance" || data.DEPARTMENT == "Finance Approval")
                    {
                        //var query = (from _vw1 in _atdb.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == data.EMP_CODE && x.ACTIVE == 1)
                        //             join _vw2 in _atdb.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ACTIVE == 1 && x.SYKI == sykiid)
                        //             on _vw1.SUPERVISOREMPCODE equals _vw2.ADEMPCODE
                        //             select new
                        //             {
                        //                 Empcode = _vw1.SUPERVISOREMPCODE,
                        //                 Department = _vw2.FUNCTIONALDESIGNATION

                        //             }).FirstOrDefault();

                        var query = (from _vw1 in _atdb.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == data.EMP_CODE && x.ACTIVE == 1 && x.SYKI == sykiid)
                                     select new
                                     {
                                         Empcode = _vw1.ADEMPCODE,
                                         Department = _vw1.FUNCTIONALDESIGNATION
                                     }).FirstOrDefault();

                        var name = "";
                        if (query != null)
                        {
                            name = (from a in _atdb.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == query.Empcode && x.ACTIVE == 1)
                                    select a.FIRSTNAME + " " + a.LASTNAME).FirstOrDefault();
                        }


                        var update = _atdb.AT_APPROVAL_AUTHORITY.Where(x => x.EMP_CODE == query.Empcode && data.DEPARTMENT == "Finance" && x.TRAN_NO == data.TRAN_NO).Select(s => s.UPDATEDON).FirstOrDefault();
                        var Fupdate = _atdb.AT_APPROVAL_AUTHORITY.Where(x => x.EMP_CODE == query.Empcode && data.DEPARTMENT == "Finance Approval" && x.TRAN_NO == data.TRAN_NO).Select(s => s.UPDATEDON).FirstOrDefault();

                        if (data.DEPARTMENT == "Finance" && query.Department == "Department Head")
                        {
                            at.FDepartmentHead = name + "-" + query.Empcode.ToString();
                            at.FDepartmentHeaddate = update;
                        }
                        else if (data.DEPARTMENT == "Finance Approval" && query.Department == "Department Head")
                        {
                            at.FDepartmentHead = name + "-" + query.Empcode.ToString();
                            at.FDepartmentHeaddate = Fupdate;
                        }

                        if (data.DEPARTMENT == "Finance Approval" && query.Department == "Division Head")
                        {
                            at.FDivisionHead = name + "-" + query.Empcode.ToString();
                            at.FDivisionHeaddate = Fupdate;
                        }

                        if (data.DEPARTMENT == "Finance" && query.Department == "Section Head")
                        {
                            at.FSectionHead = name + "-" + query.Empcode.ToString();
                            at.FSectionHeaddate = update;
                        }

                        var req_siteid = _atdb.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ACTIVE == 1 && x.SYKI == sykiid && x.ADEMPCODE == header.REQ_ECODE).Select(y => y.SYSITEID).FirstOrDefault();
                        foreach (var com in Common)
                        {
                            if (data.DEPARTMENT == "Finance" && com.ECODE == data.EMP_CODE && req_siteid == com.SYSITEiD)
                            {
                                at.TeamMember = data.EMP_NAME + "-" + data.EMP_CODE.ToString();
                                at.TeamMemberdate = update;
                            }
                        }
                    }
                }

                return at;
            }

            catch (Exception ex)
            {

                throw ex;
            }

        }
        #endregion

        #region MIS Report

        public List<AT_AssetTransferMISReportViewModel> MISReportSearch(string TransectionType, string AssetType, DateTime FromDate, DateTime ToDate, int LoginCode, string Plant)
        {
            try
            {
                List<AT_AssetTransferMISReportViewModel> Data = new List<AT_AssetTransferMISReportViewModel>();


                var sykiid = (from s in _atdb.SYKI_LC
                              where s.ACTIVE == 1
                              select s.SYKIID).FirstOrDefault();


                Data = (from h in _atdb.AT_ASSSET_TRANSFER_HEADER
                        join d in _atdb.AT_ASSET_TRANSFER_DETAIL on h.TRAN_NO equals d.TRAN_NO
                        join a in _atdb.VW_ASSOCIATELVLDETAILS_LC on h.REQ_ECODE equals a.ADEMPCODE
                        join p in _atdb.SYSITE_LC on a.SYSITEID equals p.SYSITEID
                        where /*d.ADDEDBY == LoginCode*/
                              /*&&*/ (h.TRAN_TYPE == TransectionType || TransectionType == "All")
                              && (h.ASSET_TYPE == AssetType || AssetType == "All")
                              && (p.DESCRIP == Plant || Plant == "- Please select Plant Name -")
                              && a.SYKI == sykiid
                              && h.TRAN_DATE >= FromDate && h.TRAN_DATE <= ToDate
                        select new AT_AssetTransferMISReportViewModel
                        {

                            Plant = p.DESCRIP,
                            TRAN_NO = h.TRAN_NO,
                            TRAN_DATE = h.TRAN_DATE,
                            TRAN_TYPE = h.TRAN_TYPE,
                            ASSET_TYPE = h.ASSET_TYPE,
                            TRANSFREE_TYPE = h.TRANSFREE_TYPE,
                            TRANSFEROR = h.TRANSFEROR,
                            OPERATION = a.OPERATION,
                            DIVISION = a.DIVISION,
                            DEPARTMENT = a.DEPARTMENT,
                            TRANSFREE = h.TRANSFREE,
                            ASSET_CODE = d.ASSET_CODE,
                            ASSET_CLS_DESC = d.ASSET_CLS_DESC,
                            DESCRIPTION = d.DESCRIPTION,
                            ASSETMAIN_NO_TEXT = d.ASSETMAIN_NO_TEXT,
                            VENDOR_CODE = d.VENDOR_CODE,
                            VENDER_NAME = d.VENDER_NAME,
                            PO_NO = d.PO_NO,
                            PO_DATE = d.PO_DATE,
                            INV_NO = d.INV_NO,
                            INV_DATE = d.INV_DATE,
                            FULLY_PARTIAL = d.FULLY_PARTIAL,
                            QTY = d.QTY,
                            CURRENT_LOCATION = d.CURRENT_LOCATION,
                            NEW_LOCATION = d.NEW_LOCATION,
                            ORIGINAL_COST = d.ORIGINAL_COST,
                            DEPRECIATION = d.DEPRECIATION,
                            NET_BLOCK = d.NET_BLOCK,
                            BASIC_PRICE = d.BASIC_PRICE,
                            INVOICE_VALUE = d.INVOICE_VALUE,
                            NEW_ASSET_CODE = d.NEW_ASSET_CODE,
                            EODC_CLEAREANSE = d.EODC_CLEAREANSE,
                            STATUS = h.STATUS
                        }).OrderBy(x => x.TRAN_NO).ToList();


                var srno = 1;

                foreach (var i in Data)
                {
                    var r = srno++;
                    if (i.TRANSFREE_TYPE == "Vendor")
                    {
                        i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                        i.TRANSFREE = i.TRANSFREE + "-" + GetVendorName(i.TRANSFREE);
                    }
                    else
                    {
                        i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                        i.TRANSFREE = i.TRANSFREE + "-" + GetEmpName(int.Parse(i.TRANSFREE));
                    }
                    i.SRNO = r;
                }

                return Data;


            }
            catch (Exception ex)
            {
                throw ex;

            }





        }

        public LibResult GetPlanForMISReport()
        {

            LibResult res = new LibResult();

            try
            {
                var Data = (from h in _atdb.SYSITE_LC
                            where h.ACTIVE == 1
                            select new
                            {
                                Text = h.DESCRIP,
                                Value = h.DESCRIP
                            }).ToList();

                res.resultObject = Data;



                return res;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region Manage Asset Transfer By Admin

        public List<AT_AssetTransferHeaderViewModel> ManageAssetTransfer(int LoginCode, string flag)
        {

            List<AT_AssetTransferHeaderViewModel> ilist = new List<AT_AssetTransferHeaderViewModel>();

            var value = _atdb.AT_ASSSET_TRANSFER_HEADER.Where(x => x.REQ_ECODE == LoginCode).ToList();

            foreach (var d in value)
            {
                ilist = (from r in _atdb.AT_ASSSET_TRANSFER_HEADER
                         where r.STATUS == "Pending At Taxation" || r.STATUS == "Pending At Finance TM" || r.STATUS == "Pending At Approve" || r.STATUS == "Pending At Finance" || r.STATUS == "Pending At GateOutStamp" || r.STATUS == "Pending At Security" || r.STATUS == "UpdateNewAssetCode" || r.STATUS == "Pending At GateInStamp"
                          && r.ASSET_TYPE == flag
                         select new AT_AssetTransferHeaderViewModel
                         {
                             APPROVAL_TYPE = r.APPROVAL_TYPE,
                             SRNO = r.SRNO,
                             TRAN_NO = r.TRAN_NO,
                             TRAN_DATE = r.TRAN_DATE,
                             TRAN_TYPE = r.TRAN_TYPE,
                             ASSET_TYPE = r.ASSET_TYPE,
                             TRANSFEROR = r.TRANSFEROR,
                             TRANSFREE = r.TRANSFREE,
                             APPAUTHSRNO = _atdb.AT_APPROVAL_AUTHORITY.Where(x => x.TRAN_NO == r.TRAN_NO && (x.APPROVAL_STATUS == "Pending" || x.APPROVAL_STATUS == "Rejected")).Select(y => y.SRNO).FirstOrDefault(),
                             TOTAL = r.TOTAL,
                             STATUS = r.STATUS,
                             PDFPATH = r.PDFPATH,
                             TRANSFREE_TYPE = r.TRANSFREE_TYPE,
                             GETOUT_DOC_PATH = r.GETOUT_DOC_PATH,
                             GETIN_DOC_PATH = r.GETIN_DOC_PATH,
                             DURATION = r.DURATION
                         }).OrderBy(x => x.TRAN_NO).ToList();

                foreach (var i in ilist)
                {
                    if (i.TRANSFREE_TYPE == "Vendor")
                    {
                        i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                        i.TRANSFREE = i.TRANSFREE + "-" + GetVendorName(i.TRANSFREE);
                    }
                    else
                    {
                        i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                        i.TRANSFREE = i.TRANSFREE + "-" + GetEmpName(int.Parse(i.TRANSFREE));
                    }

                }
            }

            return ilist;

        }

        public AT_APPROVAL_AUTHORITY AuthEdit(int id)
        {

            try
            {
                var _obj = (from data in _atdb.AT_APPROVAL_AUTHORITY.Where(v => v.SRNO == id)
                            select data).FirstOrDefault();

                return _obj;

            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        public LibResult AuthoritySave(int Empcode, string EmpName, int srno, string Department, int LoginCode)
        {
            LibResult res = new LibResult();
            try
            {
                _atdb.Database.BeginTransaction();
                var sykiid = (from s in _atdb.SYKI_LC
                              where s.ACTIVE == 1
                              select s.SYKIID).FirstOrDefault();

                //using (var db = new LCEntities())
                //{
                var Exists = _atdb.AT_APPROVAL_AUTHORITY.Where(x => x.EMP_CODE == Empcode
                                                    && x.EMP_NAME == EmpName
                                                    && x.SRNO == srno
                                                   ).Count();



                AT_APPROVAL_AUTHORITY at_comman = _atdb.AT_APPROVAL_AUTHORITY.Find(srno);

                if (Department == "GateOutStamp")
                {
                    var Gateout = (from h in _atdb.AT_ASSSET_TRANSFER_HEADER
                                   where at_comman.TRAN_NO == h.TRAN_NO
                                   select h).FirstOrDefault();

                    AT_ASSSET_TRANSFER_HEADER Hd = _atdb.AT_ASSSET_TRANSFER_HEADER.Find(Convert.ToInt32(Gateout.TRAN_NO));
                    Hd.GATEOUTSTAMP = Empcode;
                    _atdb.Entry(Hd).State = EntityState.Modified;
                    _atdb.SaveChanges();

                    at_comman.EMP_CODE = Empcode;
                    at_comman.EMP_NAME = EmpName;
                    at_comman.UPDATEDBY = LoginCode;
                    at_comman.UPDATEDON = DateTime.Now;

                    _atdb.Entry(at_comman).State = EntityState.Modified;
                    _atdb.SaveChanges();

                }
                else
                {
                    at_comman.EMP_CODE = Empcode;
                    at_comman.EMP_NAME = EmpName;
                    at_comman.UPDATEDBY = LoginCode;
                    at_comman.UPDATEDON = DateTime.Now;

                    _atdb.Entry(at_comman).State = EntityState.Modified;
                    _atdb.SaveChanges();
                }

                _atdb.Database.CommitTransaction();
                res.hasError = false;
                res.errorMessage = "Record Updated successfully";

                //}
            }
            catch (Exception ex)
            {
                _atdb.Database.RollbackTransaction();
                res.hasError = true;
                res.errorMessage = ex.ToString();

            }
            return res;
        }


        public long? GetOperationMappId(List<long?> _orgList)
        {
            long? mappIds = new long?();
            if (_orgList.Count > 0)
                mappIds = _atdb.AT_AST_OPERATIONMAPP.Where(data => data.ACTIVE == 1 && _orgList.Contains(data.ORGID)).Select(data => data.OPERATIONID).FirstOrDefault();
            return mappIds;
        }

        public LibResult ManageRequestor_Detail(string AssetType, string Transferor, int AddedBy, int LoginCode)
        {
            LibResult res = new LibResult();
            try
            {

                var sykiid = (from s in _atdb.SYKI_LC
                              where s.ACTIVE == 1
                              select s.SYKIID).FirstOrDefault();

                //long Tran = long.Parse(Transferor.ToString());

                var d = (from a in _atdb.ADEMPLOYEE_LC.Where(x => x.ADEMPCODE == AddedBy && x.ACTIVE == 1)
                         join vw in _atdb.VW_ASSOCIATELVLDETAILS_LC on a.ADEMPCODE equals vw.ADEMPCODE
                         where vw.SYKI == sykiid && vw.ACTIVE == 1
                         select new AT_AssetRequestorDetail
                         {
                             ASSET_TYPE = AssetType,
                             //Requested_By = a.FIRSTNAME + " " + a.LASTNAME + " - " + "[" + LoginCode + "]",
                             Requested_By = AddedBy + " - " + a.FIRSTNAME + " " + a.LASTNAME,
                             OPERATION = vw.OPERATION,
                             DIVISION = vw.DIVISION,
                             DEPARTMENT = vw.DEPARTMENT,
                             SECTION = vw.SECTION
                         }).FirstOrDefault();

                res.resultObject = d;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return res;
        }
        #endregion

        //SR109866-CR7276
        #region Capitalized Asset transfer Report
        public List<AT_AssetTransferHeaderViewModel> GetCapitalizeAssetTransferReportList(int LoginCode, string flag)
        {

            List<AT_AssetTransferHeaderViewModel> ilist = new List<AT_AssetTransferHeaderViewModel>();
            try
            {

                var _SYKIID = _atdb.SYKI_LC.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();

                var DivId = _atdb.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == LoginCode && x.SYKI == _SYKIID).Select(x => x.DIVISIONID).FirstOrDefault();

                var Emplist = _atdb.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.SYKI == _SYKIID && x.DIVISIONID == DivId).Select(x => x.ADEMPCODE).ToList();

                ilist = (from data in _atdb.AT_ASSSET_TRANSFER_HEADER
                         where Emplist.Contains(data.REQ_ECODE)
                            && data.ASSET_TYPE == flag
                         select new AT_AssetTransferHeaderViewModel
                         {
                             SRNO = data.SRNO,
                             TRAN_NO = data.TRAN_NO,
                             TRAN_TYPE = data.TRAN_TYPE,
                             ASSET_TYPE = data.ASSET_TYPE,
                             TRAN_DATE = data.TRAN_DATE,
                             TRANSFEROR = data.TRANSFEROR,
                             TRANSFREE = data.TRANSFREE,
                             TOTAL = data.TOTAL,
                             APPAUTHSRNO = _atdb.AT_APPROVAL_AUTHORITY
                                            .Where(x => x.TRAN_NO == data.TRAN_NO && x.APPROVAL_STATUS == "-")
                                            .Select(y => y.SRNO)
                                            .FirstOrDefault(),
                             TRANSFREE_TYPE = data.TRANSFREE_TYPE,
                             STATUS = data.STATUS,
                             PDFPATH = data.PDFPATH,
                             GETOUT_DOC_PATH = data.GETOUT_DOC_PATH,
                             GETIN_DOC_PATH = data.GETIN_DOC_PATH,
                             DURATION = data.DURATION
                         })
                        .OrderBy(x => x.TRAN_NO)
                        .ToList();

                    foreach (var i in ilist)
                    {
                        if (i.TRANSFREE_TYPE == "Vendor")
                        {
                            i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                            i.TRANSFREE = i.TRANSFREE + "-" + GetVendorName(i.TRANSFREE);
                        }
                        else
                        {
                            i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                            i.TRANSFREE = i.TRANSFREE + "-" + GetEmpName(int.Parse(i.TRANSFREE));
                        }

                    }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return ilist;
        }

        public List<AT_AssetTransferHeaderViewModel> CapitalizeAssetTransferReportListSearch(string TransactionType, /*string AssetType,*/ int TransactionNo, string TransfereeUser, string Status, int LoginCode, string flag)
        {

            try
            {
                #region MyRegion
                List<AT_AssetTransferHeaderViewModel> Data = new List<AT_AssetTransferHeaderViewModel>();

                var _SYKIID = _atdb.SYKI_LC.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();

                var DivId = _atdb.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == LoginCode && x.SYKI == _SYKIID).Select(x => x.DIVISIONID).FirstOrDefault();

                var Emplist = _atdb.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.SYKI == _SYKIID && x.DIVISIONID == DivId).Select(x => x.ADEMPCODE).ToList();



                if (Status == "Initiate" || Status == "Completed" || Status == "Rejected" || Status == "Cancelled by User" || Status == "Cancelled by Admin")
                {
                    Data = (from r in _atdb.AT_ASSSET_TRANSFER_HEADER
                            where ( Emplist.Contains(r.REQ_ECODE))
                             && (r.ASSET_TYPE == flag)
                           && (r.TRAN_NO == TransactionNo || TransactionNo == 0)
                            && (r.TRANSFREE == TransfereeUser || TransfereeUser == "0")
                            && (r.TRAN_TYPE == TransactionType || TransactionType == "All")
                            && (r.STATUS == Status)


                            select new AT_AssetTransferHeaderViewModel
                            {
                                SRNO = r.SRNO,
                                TRAN_NO = r.TRAN_NO,
                                TRAN_DATE = r.TRAN_DATE,
                                TRAN_TYPE = r.TRAN_TYPE,
                                ASSET_TYPE = r.ASSET_TYPE,
                                TRANSFEROR = r.TRANSFEROR,
                                TRANSFREE = r.TRANSFREE,
                                APPAUTHSRNO = _atdb.AT_APPROVAL_AUTHORITY.Where(x => x.TRAN_NO == r.TRAN_NO && x.APPROVAL_STATUS == "Pending").Select(y => y.SRNO).FirstOrDefault(),
                                TOTAL = r.TOTAL,
                                STATUS = r.STATUS,
                                PDFPATH = r.PDFPATH,
                                GETOUT_DOC_PATH = r.GETOUT_DOC_PATH,
                                GETIN_DOC_PATH = r.GETIN_DOC_PATH,
                                DURATION = r.DURATION
                            }).OrderBy(O => O.TRAN_NO).ToList();

                }
                else
                {
                    Data = (from r in _atdb.AT_ASSSET_TRANSFER_HEADER
                            where ( Emplist.Contains(r.REQ_ECODE))
                            && (r.ASSET_TYPE == flag)
                          && (r.TRAN_NO == TransactionNo || TransactionNo == 0)
                           && (r.TRANSFREE == TransfereeUser || TransfereeUser == "0")
                           && (r.TRAN_TYPE == TransactionType || TransactionType == "All")
                           && (r.STATUS == "Pending At Taxation" || r.STATUS == "Pending At Approve" || r.STATUS == "Pending At Finance" || r.STATUS == "Pending At GateOutStamp" || r.STATUS == "Pending At Security" || r.STATUS == "UpdateNewAssetCode" || r.STATUS == "Pending At GateInStamp" || r.STATUS == "Pending At Finance TM")

                            select new AT_AssetTransferHeaderViewModel
                            {
                                SRNO = r.SRNO,
                                TRAN_NO = r.TRAN_NO,
                                TRAN_DATE = r.TRAN_DATE,
                                TRAN_TYPE = r.TRAN_TYPE,
                                ASSET_TYPE = r.ASSET_TYPE,
                                TRANSFEROR = r.TRANSFEROR,
                                TRANSFREE = r.TRANSFREE,
                                APPAUTHSRNO = _atdb.AT_APPROVAL_AUTHORITY.Where(x => x.TRAN_NO == r.TRAN_NO && x.APPROVAL_STATUS == "Pending").Select(y => y.SRNO).FirstOrDefault(),
                                TOTAL = r.TOTAL,
                                STATUS = r.STATUS,
                                PDFPATH = r.PDFPATH,
                                TRANSFREE_TYPE = r.TRANSFREE_TYPE,
                                GETOUT_DOC_PATH = r.GETOUT_DOC_PATH,
                                GETIN_DOC_PATH = r.GETIN_DOC_PATH,
                                DURATION = r.DURATION

                            }).OrderBy(x => x.TRAN_NO).ToList();


                }
                foreach (var i in Data)
                {
                    if (i.TRANSFREE_TYPE == "Vendor")
                    {
                        i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                        i.TRANSFREE = i.TRANSFREE + "-" + GetVendorName(i.TRANSFREE);
                    }
                    else
                    {
                        i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                        i.TRANSFREE = i.TRANSFREE + "-" + GetEmpName(int.Parse(i.TRANSFREE));
                    }
                }
                return Data;


            }
            catch (Exception ex)
            {
                throw ex;

            }


            #endregion


        }
        public List<AT_AssetTransferHeaderViewModel> CapitalizeAssetTransferReportStatusSearch(string Status, int LoginCode, string flag)
        {
            try
            {
                List<AT_AssetTransferHeaderViewModel> Data = new List<AT_AssetTransferHeaderViewModel>();

                var _SYKIID = _atdb.SYKI_LC.Where(x => x.ACTIVE == 1).Select(x => x.SYKIID).FirstOrDefault();

                var DivId = _atdb.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.ADEMPCODE == LoginCode && x.SYKI == _SYKIID).Select(x => x.DIVISIONID).FirstOrDefault();

                var Emplist = _atdb.VW_ASSOCIATELVLDETAILS_LC.Where(x => x.SYKI == _SYKIID && x.DIVISIONID == DivId).Select(x => x.ADEMPCODE).ToList();


                if (Status == "Initiate" || Status == "Completed" || Status == "Rejected" || Status == "Cancelled by User" || Status == "Cancelled by Admin")
                {
                    Data = (from r in _atdb.AT_ASSSET_TRANSFER_HEADER
                            where ( Emplist.Contains(r.REQ_ECODE))
                          && (r.STATUS == Status)
                          && r.ASSET_TYPE == flag

                            select new AT_AssetTransferHeaderViewModel
                            {
                                APPROVAL_TYPE = r.APPROVAL_TYPE,
                                SRNO = r.SRNO,
                                TRAN_NO = r.TRAN_NO,
                                TRAN_DATE = r.TRAN_DATE,
                                TRAN_TYPE = r.TRAN_TYPE,
                                ASSET_TYPE = r.ASSET_TYPE,
                                TRANSFEROR = r.TRANSFEROR,
                                TRANSFREE = r.TRANSFREE,
                                APPAUTHSRNO = _atdb.AT_APPROVAL_AUTHORITY.Where(x => x.TRAN_NO == r.TRAN_NO && (x.APPROVAL_STATUS == "Pending" || x.APPROVAL_STATUS == "-" || x.APPROVAL_STATUS == "Rejected")).Select(y => y.SRNO).FirstOrDefault(),
                                TOTAL = r.TOTAL,
                                STATUS = r.STATUS,
                                PDFPATH = r.PDFPATH,
                                GETOUT_DOC_PATH = r.GETOUT_DOC_PATH,
                                GETIN_DOC_PATH = r.GETIN_DOC_PATH,
                                TRANSFREE_TYPE = r.TRANSFREE_TYPE,
                                DURATION = r.DURATION
                            }).OrderBy(x => x.TRAN_NO).ToList();


                }
                else
                {
                    Data = (from r in _atdb.AT_ASSSET_TRANSFER_HEADER
                            where ( Emplist.Contains(r.REQ_ECODE))
                           && (r.STATUS == "Pending At Taxation" || r.STATUS == "Pending At Finance TM" || r.STATUS == "Pending At Approve" || r.STATUS == "Pending At Finance" || r.STATUS == "Pending At GateOutStamp" || r.STATUS == "Pending At Security" || r.STATUS == "UpdateNewAssetCode" || r.STATUS == "Pending At GateInStamp")
                             && r.ASSET_TYPE == flag
                            select new AT_AssetTransferHeaderViewModel
                            {
                                APPROVAL_TYPE = r.APPROVAL_TYPE,
                                SRNO = r.SRNO,
                                TRAN_NO = r.TRAN_NO,
                                TRAN_DATE = r.TRAN_DATE,
                                TRAN_TYPE = r.TRAN_TYPE,
                                ASSET_TYPE = r.ASSET_TYPE,
                                TRANSFEROR = r.TRANSFEROR,
                                TRANSFREE = r.TRANSFREE,
                                APPAUTHSRNO = _atdb.AT_APPROVAL_AUTHORITY.Where(x => x.TRAN_NO == r.TRAN_NO && (x.APPROVAL_STATUS == "Pending" || x.APPROVAL_STATUS == "Rejected")).Select(y => y.SRNO).FirstOrDefault(),
                                TOTAL = r.TOTAL,
                                STATUS = r.STATUS,
                                PDFPATH = r.PDFPATH,
                                TRANSFREE_TYPE = r.TRANSFREE_TYPE,
                                GETOUT_DOC_PATH = r.GETOUT_DOC_PATH,
                                GETIN_DOC_PATH = r.GETIN_DOC_PATH,
                                DURATION = r.DURATION
                            }).OrderBy(x => x.TRAN_NO).ToList();


                }
                foreach (var i in Data)
                {
                    if (i.TRANSFREE_TYPE == "Vendor")
                    {
                        i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                        i.TRANSFREE = i.TRANSFREE + "-" + GetVendorName(i.TRANSFREE);
                    }
                    else
                    {
                        i.TRANSFEROR = i.TRANSFEROR + "-" + GetEmpName(int.Parse(i.TRANSFEROR));
                        i.TRANSFREE = i.TRANSFREE + "-" + GetEmpName(int.Parse(i.TRANSFREE));
                    }
                }
                return Data;

            }
            catch (Exception ex)
            {
                throw ex;

            }

        }
        #endregion
        //SR109866-CR7276

    }
}
