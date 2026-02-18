

using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ePortal.Infrastructure.Repositories
{
    public class PRRepository
    {

        //Added by aumento as on 19092024 for the SR71870============================================================
        private ICProcessBDContext _ICDBContext;
        //===========================================================================================================
        private EPortalDBContext _PrDBContext;
        private SYKI _Syki;
        private CommonRepository Comres;
        public PRRepository(EPortalDBContext objEPortalDBContext , ICProcessBDContext icProcessBDContext, CommonRepository comres)
        {
            //Added by aumento as on 19092024 for the SR71870============================================================
            _ICDBContext = icProcessBDContext;
            //===========================================================================================================
            _PrDBContext = objEPortalDBContext;
            _Syki = _PrDBContext.SYKI.Where(x => x.ACTIVE == 1).FirstOrDefault();
            Comres = comres;
        }

        public Tuple<short, long> SavePRRequest(PRHeaderViewModel model)
        {
            short retVal = 0; long retHeaderId = 0;
            Tuple<short, long> _retVal_tuple;
            using (var transaction  = _PrDBContext.Database.BeginTransaction())
            {
                try
                {
                    DGIT_PRHEADER DPH = new DGIT_PRHEADER();
                    int FlagAdd = 0;
                    if (model.PRHEADERID > 0)
                    {
                        DPH = _PrDBContext.DGIT_PRHEADER.Where(x => x.PRHEADERID == model.PRHEADERID).SingleOrDefault();
                    }
                    else
                    {
                        if (_PrDBContext.DGIT_PRHEADER.Where(x => x.PRHEADERID != model.PRHEADERID && x.INDENT_NO == model.IndentNo && (x.PROCESS_STATUS == 1 || x.PROCESS_STATUS == 0)).Count() != 0)
                        {
                            retVal = 2;
                            return _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId); //// -- record already exist.
                        }

                        /// --- Check PO is exist --- ///
                        //var PORecordExist = (from data in _PrDBContext.DGIT_POHEADER.Where(n => (n.PROCESS_STATUS != 4 || n.PROCESS_STATUS != 3))
                        var PORecordExist = (from data in _PrDBContext.DGIT_POHEADER.Where(n => (n.PROCESS_STATUS != 4 && n.PROCESS_STATUS != 3))
                                             join _POPR in _PrDBContext.DGIT_POPRLINK_DTL on data.POHEADERID equals _POPR.POHEADERID into _POPRH
                                             from _POPRHH in _POPRH.DefaultIfEmpty()
                                             where _POPRHH.PRNO == model.IndentNo
                                             select data).Count();

                        if (PORecordExist > 0)
                        {
                            retVal = 4;
                            return _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId); //// -- PO record already exist.
                        }

                        bool BuyerRecordExist = false;
                        /// --- Check PR In Progress --- ///
                        var BuyerRecordObj = (from data in _PrDBContext.DGIT_PRHEADER.Where(m => m.INDENT_NO == model.IndentNo && m.PROCESS_STATUS == 2)
                                              join _Buyer in _PrDBContext.DGIT_PRBUYER_MAP.Where(b => b.STATUS == 1) on data.PRHEADERID equals _Buyer.PRHEADERID into _BuyerJoin
                                              from _BuyerMap in _BuyerJoin.DefaultIfEmpty()
                                              join _PRStatus in _PrDBContext.DGIT_PRPURSTATUS on _BuyerMap.PRBUYERID equals _PRStatus.PRBUYERID into _PRStatusJoin
                                              from _PRBuyerStatus in _PRStatusJoin.DefaultIfEmpty()
                                                  //where (_PRBuyerStatus != null ? (_PRBuyerStatus.STATUS != 2) : true)
                                              select new
                                              {
                                                  PRHEADER_ID = data.PRHEADERID,
                                                  BUYER_ID = _BuyerMap == null ? 0 : _BuyerMap.PRBUYERID,
                                                  PRSTATUS_ID = _PRBuyerStatus == null ? 0 : _PRBuyerStatus.PRPURSTATUSID,
                                                  PRSTATUS = _PRBuyerStatus == null ? 0 : _PRBuyerStatus.STATUS,
                                              }).ToList().OrderByDescending(o => o.PRHEADER_ID).ThenByDescending(t => t.PRSTATUS_ID).FirstOrDefault();
                        if (BuyerRecordObj != null)
                        {
                            if (BuyerRecordObj.BUYER_ID > 0)
                            {
                                BuyerRecordExist = (BuyerRecordObj.PRSTATUS_ID == 0 ? true : (BuyerRecordObj.PRSTATUS == 2 ? false : true));
                            }
                        }

                        if (BuyerRecordExist)
                        {
                            retVal = 5;
                            return _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId); //// -- PR already Processed.
                        }


                        DPH = new DGIT_PRHEADER();
                        if (_PrDBContext.DGIT_PRHEADER.Count() == 0)
                        {
                            DPH.PRHEADERID = 1;
                        }
                        else
                        {
                            DPH.PRHEADERID = _PrDBContext.DGIT_PRHEADER.Max(x => x.PRHEADERID) + 1;
                        }
                        FlagAdd = 1;
                    }
                    if (model.IsFinalSubmit == 0)
                    {
                        DPH.INDENT_NO = model.IndentNo;
                        DPH.INDENT_DATE = model.IndentDate ?? DateTime.MinValue;
                        DPH.INDENT_AMOUNT = model.IndentAmount;
                        DPH.ITEM_DETAIL = model.ItemDetail;
                        DPH.REMARK = model.Remark;
                        DPH.ARIBASTATUS = Convert.ToInt16(model.ARIBASTATUS);
                        DPH.ARIBARFPID = model.ARIBARFPID;
                        DPH.ARIBABUYER = model.ARIBABuyer_ECODE;
                        DPH.PRTYE = (short)model.PRTYPE;
                        DPH.PRCAT = (short)model.CATID;
                        //Added by aumento for the SR73841============
                        DPH.ARIBASRNO = model.ARIBASRNO;
                        //============================================
                        DPH.RELEASEPLANTID = model.PR_ReleaseLocation; //  Added by Aumento ::  SR68003
                        DPH.PRCATEGORY = model.SLA_Category; //  Added by ttl ::  SR108972 > CR7273
                    }
                    else
                    {
                        if (_PrDBContext.DGIT_PRDETAIL.Where(x => x.PRHEADERID == model.PRHEADERID && x.DOC_TYPE == "PR").Count() == 0)
                        {
                            retVal = 3;
                            return _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId); //// -- PR file not exist.
                        }
                    }
                    DPH.PROCESS_STATUS = model.PROCESS_STATUS;
                    DPH.STATUS = model.STATUS;
                    if (FlagAdd == 1)
                    {
                        DPH.ADDEDBY = model.ADDEDBY;
                        DPH.DATEADDED = DateTime.Now;
                    }
                    else
                    {
                        DPH.UPDATEDBY = model.UPDATEDBY;
                        DPH.UPDATEDATE = DateTime.Now;
                    }
                    //16-July-2022 - SIS PR Change
                    if (model.ITServiceMatSISAppStatus == true)
                    {
                        DPH.IS_SISPR = 1;
                    }
                    else
                    {
                        DPH.IS_SISPR = null;
                    }

                    if (model.NonITServiceMatDeclaration == true)
                    {
                        DPH.IS_NONSISPR = 1;
                    }
                    else
                    {
                        DPH.IS_NONSISPR = null;
                    }
                    if (model.IPServiceMatDeclaration == true)
                    {
                        DPH.IS_IPRELATEDPR = 1;
                    }
                    else
                    {
                        DPH.IS_IPRELATEDPR = null;
                    }
                    //16-July-2022 - SIS PR Change

                    _PrDBContext.Entry(DPH).State = (FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified);
                    _PrDBContext.SaveChanges();

                    /// --- Save PR Detail --- ///
                    if (model.prDetail != null)
                    {
                        if (model.prDetail.Count > 0)
                        {
                            SavePRDetails(model.ADDEDBY, DPH.PRHEADERID, model.prDetail);
                        }
                    }

                    /// --- Save PR Approval Auth Seq ---///
                    if (model.prAuthSeq != null)
                    {
                        if (model.prAuthSeq.Count > 0)
                        {
                            SaveAppAuthSeq(model.ADDEDBY, DPH.PRHEADERID, model.prAuthSeq);

                            /// --- Save PR Approval Authority --- ///
                            PRAppAuthSeqViewModel isparr = model.prAuthSeq.OrderBy(o => o.APP_SEQ).FirstOrDefault();

                            List<PRAppAuthSeqViewModel> seqModel = model.prAuthSeq.Where(m => m.APP_SEQ == isparr.APP_SEQ).ToList();

                            if (seqModel != null)
                            {
                                SavePRAppHis(model.ADDEDBY, DPH.PRHEADERID, seqModel);
                            }
                        }
                    }

                    if (model.skipAuthList != null)
                    {
                        if (model.skipAuthList.Count > 0)
                        {
                            SaveSkipAuth(model.ADDEDBY, DPH.PRHEADERID, model.skipAuthList);
                        }
                    }

                    transaction.Commit();
                    retVal = 1;
                    retHeaderId = DPH.PRHEADERID;
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId);
                return _retVal_tuple;
            }
        }

        public short SavePRDetails(long AddedBy, long PRHEADERID, List<PRDetailViewModel> PDVMList)
        {
            short retVal = 0;
            foreach (PRDetailViewModel PDVM in PDVMList)
            {
                DGIT_PRDETAIL DPD = new DGIT_PRDETAIL();
                int FlagAdd = 0;
                if (PRHEADERID > 0)
                {
                    if (PDVM.DOC_TYPE == "PR")
                    {
                        DPD = _PrDBContext.DGIT_PRDETAIL.Where(x => x.PRHEADERID == PRHEADERID && x.DOC_TYPE == PDVM.DOC_TYPE).FirstOrDefault();
                        if (DPD != null)
                        {
                            _PrDBContext.DGIT_PRDETAIL.Remove(DPD);
                            _PrDBContext.SaveChanges();
                        }
                    }

                    DPD = new DGIT_PRDETAIL();
                    if (_PrDBContext.DGIT_PRDETAIL.Count() == 0)
                    {
                        DPD.PRDTL_ID = 1;
                    }
                    else
                    {
                        DPD.PRDTL_ID = _PrDBContext.DGIT_PRDETAIL.Max(x => x.PRDTL_ID) + 1;
                    }
                    FlagAdd = 1;

                    DPD.PRHEADERID = PRHEADERID;
                    DPD.DOC_TYPE = PDVM.DOC_TYPE;
                    DPD.FILENAME = PDVM.FILENAME;
                    DPD.ADDITIONAL_INFO = PDVM.ADDITIONAL_INFO;
                    DPD.STATUS = 1;
                    if (FlagAdd == 1)
                    {
                        DPD.ADDEDBY = AddedBy;
                        DPD.ADDEDDATE = DateTime.Now;
                    }
                    else
                    {
                        DPD.UPDATEDBY = AddedBy;
                        DPD.UPDATEDATE = DateTime.Now;
                    }
                    _PrDBContext.Entry(DPD).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _PrDBContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }

        public void SaveAppAuthSeq(long AddedBy, long PRHEADERID, List<PRAppAuthSeqViewModel> PSVMList)
        {
            //// --- Delete recode ---////
            List<DGIT_PRAPPAUTHSEQ> SeqList = _PrDBContext.DGIT_PRAPPAUTHSEQ.Where(t => t.PRID == PRHEADERID).ToList();
            if (SeqList.Count > 0)
            {
                _PrDBContext.DGIT_PRAPPAUTHSEQ.RemoveRange(SeqList);
                _PrDBContext.SaveChanges();
            }

            foreach (PRAppAuthSeqViewModel PSVM in PSVMList.OrderBy(o => o.APP_SEQ).ToList())
            {
                DGIT_PRAPPAUTHSEQ DAAS = new DGIT_PRAPPAUTHSEQ();
                if (_PrDBContext.DGIT_PRAPPAUTHSEQ.Count() == 0)
                {
                    DAAS.PRAPPAUTH_ID = 1;
                }
                else
                {
                    DAAS.PRAPPAUTH_ID = _PrDBContext.DGIT_PRAPPAUTHSEQ.Max(x => x.PRAPPAUTH_ID) + 1;
                }
                DAAS.PRID = PRHEADERID;
                DAAS.ADEMPCODE = PSVM.ADEMPCODE;
                DAAS.APP_SEQ = PSVM.APP_SEQ;
                DAAS.APPTYPE = PSVM.APPTYPE;
                DAAS.STATUS = 1;
                DAAS.ADDEDBY = AddedBy;
                DAAS.ADDEDDATE = DateTime.Now;
                DAAS.PRINTORDER = PSVM.PRINTORDER;
                DAAS.ISPARALLELAPP = PSVM.ISPARALELLAPP;
                DAAS.ACTIONFOR = PSVM.ACTIONFOR;
                DAAS.IS_SISPR = PSVM.IS_SISPR; //SIS PR Change
                DAAS.APP_TYPEINFO = PSVM.APP_TYPEINFO; //SIS PR Change
                _PrDBContext.Entry(DAAS).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                _PrDBContext.SaveChanges();
            }
        }

        public void SavePRAppHis(long AddedBy, long PRHEADERID, List<PRAppAuthSeqViewModel> PSVMList)
        {
            foreach (var PSVM in PSVMList)
            {
                DGIT_PRAPPHISTORY DPAH = new DGIT_PRAPPHISTORY();
                int FlagAdd = 0;
                DPAH = _PrDBContext.DGIT_PRAPPHISTORY.Where(d => d.ADEMPCODE == PSVM.ADEMPCODE && d.PRID == PRHEADERID && d.APPROVAL_STATUS == 0).FirstOrDefault();
                if (DPAH == null)
                {
                    DPAH = new DGIT_PRAPPHISTORY();
                    if (_PrDBContext.DGIT_PRAPPHISTORY.Count() == 0)
                    {
                        DPAH.PRAPPHISTORY_ID = 1;
                    }
                    else
                    {
                        DPAH.PRAPPHISTORY_ID = _PrDBContext.DGIT_PRAPPHISTORY.Max(x => x.PRAPPHISTORY_ID) + 1;
                    }
                    FlagAdd = 1;
                }
                DPAH.PRID = PRHEADERID;
                DPAH.ADEMPCODE = PSVM.ADEMPCODE;
                DPAH.APPROVAL_STATUS = 0;
                DPAH.APPROVAL_REMARK = "";
                DPAH.ACTIONFOR = PSVM.ACTIONFOR;
                DPAH.APP_TYPEINFO = PSVM.APP_TYPEINFO; //SIS PR Change
                if (FlagAdd == 1)
                {
                    DPAH.ADDEDBY = AddedBy;
                    DPAH.ADDEDDATE = DateTime.Now;
                }
                else
                {
                    DPAH.UPDATEBY = AddedBy;
                    DPAH.UPDATEDATE = DateTime.Now;
                }
                //Added By Aumento as on 02102024 for SR81488====================================
                DPAH.APP_SEQ_HIS = PSVM.APP_SEQ;
                //===============================================================================
                _PrDBContext.Entry(DPAH).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                _PrDBContext.SaveChanges();
            }
        }


        //Added by Aumento as on 23072024
        public void SavePRAppHistoryApproval(long AddedBy, long PRHEADERID, PRAppAuthSeqViewModel PSVM, string Remarks, short AppStatus)
        {
            //Remarks = AppStatus == (short)4 ? "" : Remarks;
            //AppStatus = AppStatus == (short)4 ? (short)0 : AppStatus;
            try
            {
                //using (var _PrDBContext = new ePortalEntities2())
                //{

                DGIT_PRAPPHISTORY DPAH = new DGIT_PRAPPHISTORY();
                int FlagAdd = 1;
                var data = _PrDBContext.DGIT_PRAPPHISTORY.Where(d => d.ADEMPCODE == PSVM.ADEMPCODE && d.PRID == PRHEADERID && (d.APPROVAL_STATUS == 0 || d.APPROVAL_STATUS == 4)).FirstOrDefault();
                // DPAH = _PrDBContext.DGIT_PRAPPHISTORY.Where(d => d.ADEMPCODE == PSVM.ADEMPCODE && d.PRID == PRHEADERID && d.APPROVAL_STATUS == 0).FirstOrDefault();

                DPAH = new DGIT_PRAPPHISTORY();
                if (_PrDBContext.DGIT_PRAPPHISTORY.Count() == 0)
                {
                    DPAH.PRAPPHISTORY_ID = 1;
                }
                else
                {
                    DPAH.PRAPPHISTORY_ID = (_PrDBContext.DGIT_PRAPPHISTORY.Max(x => x.PRAPPHISTORY_ID)) + 1;

                }
                FlagAdd = 1;
                //var _AddedBy = AppStatus.ToString() == "0" ? (long?)null : AddedBy;
                var _AddedBy = AppStatus.ToString() == "4" ? (long?)null : AddedBy;
                if (data != null)
                {
                    DPAH.PRID = data.PRID;
                    DPAH.HOLD_BY = _AddedBy;
                    DPAH.HOLD_DATE = DateTime.Now;
                    DPAH.HOLD_REMARKS = Remarks;
                    DPAH.APP_DATE = DateTime.Now;
                    //DPAH.APP_DATE = data.APP_DATE;
                    //DPAH.APP_DATE = AppStatus == 0 ? null : data.APP_DATE;
                    DPAH.PRID = PRHEADERID;
                    DPAH.ADEMPCODE = PSVM.ADEMPCODE;
                    DPAH.APPROVAL_STATUS = AppStatus;
                    DPAH.APPROVAL_REMARK = Remarks;
                    DPAH.ACTIONFOR = PSVM.ACTIONFOR;
                    DPAH.APP_TYPEINFO = PSVM.APP_TYPEINFO; //SIS PR Change
                                                           //Added By Aumento as on 02102024 for SR81488====================================
                    DPAH.APP_SEQ_HIS = PSVM.APP_SEQ;
                    //===============================================================================
                    if (FlagAdd == 1)
                    {
                        DPAH.ADDEDBY = AddedBy;
                        DPAH.ADDEDDATE = DateTime.Now;
                    }
                    else
                    {
                        DPAH.UPDATEBY = AddedBy;
                        DPAH.UPDATEDATE = DateTime.Now;
                    }
                    _PrDBContext.Entry(DPAH).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _PrDBContext.SaveChanges();

                }
            }
            //}
            catch (Exception ex)
            {

                throw ex;
            }
        }
        //Added by Aumento as on 23072024
        public void SavePRSkipAppHis(long AddedBy, long PRHEADERID, PRAppAuthSeqViewModel PSVM)
        {
            DGIT_PRAPPHISTORY DPAH = new DGIT_PRAPPHISTORY();
            int FlagAdd = 0;
            DPAH = _PrDBContext.DGIT_PRAPPHISTORY.Where(d => d.ADEMPCODE == PSVM.ADEMPCODE && d.PRID == PRHEADERID && d.APPROVAL_STATUS == 0).FirstOrDefault();
            if (DPAH == null)
            {
                DPAH = new DGIT_PRAPPHISTORY();
                if (_PrDBContext.DGIT_PRAPPHISTORY.Count() == 0)
                {
                    DPAH.PRAPPHISTORY_ID = 1;
                }
                else
                {
                    DPAH.PRAPPHISTORY_ID = _PrDBContext.DGIT_PRAPPHISTORY.Max(x => x.PRAPPHISTORY_ID) + 1;
                }
                FlagAdd = 1;
            }
            DPAH.PRID = PRHEADERID;
            DPAH.ADEMPCODE = PSVM.ADEMPCODE;
            DPAH.APPROVAL_STATUS = 1;
            DPAH.APPROVAL_REMARK = "Approval Skip";
            DPAH.APP_DATE = DateTime.Now;
            if (FlagAdd == 1)
            {
                DPAH.ADDEDBY = AddedBy;
                DPAH.ADDEDDATE = DateTime.Now;
            }
            else
            {
                DPAH.UPDATEBY = AddedBy;
                DPAH.UPDATEDATE = DateTime.Now;
            }
            _PrDBContext.Entry(DPAH).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
            _PrDBContext.SaveChanges();
        }

        public void SaveSkipAuth(long AddedBy, long PRHEADERID, List<PRAPPSKIPViewModel> PASList)
        {
            //// --- Delete recode ---////
            List<DGIT_PRAPPSKIP> skipList = _PrDBContext.DGIT_PRAPPSKIP.Where(t => t.PRID == PRHEADERID).ToList();
            if (skipList.Count > 0)
            {
                _PrDBContext.DGIT_PRAPPSKIP.RemoveRange(skipList);
                _PrDBContext.SaveChanges();
            }

            foreach (PRAPPSKIPViewModel PSVM in PASList)
            {
                DGIT_PRAPPSKIP DPAS = new DGIT_PRAPPSKIP();
                if (_PrDBContext.DGIT_PRAPPSKIP.Count() == 0)
                {
                    DPAS.PRAPPSKIP_ID = 1;
                }
                else
                {
                    DPAS.PRAPPSKIP_ID = _PrDBContext.DGIT_PRAPPSKIP.Max(x => x.PRAPPSKIP_ID) + 1;
                }
                DPAS.PRID = PRHEADERID;
                DPAS.ADEMPCODE = PSVM.ADEMPCODE;
                DPAS.SKIPREMARK = PSVM.SKIPREMARK;
                DPAS.STATUS = 1;
                DPAS.ADDEDBY = AddedBy;
                DPAS.ADDEDDATE = DateTime.Now;
                _PrDBContext.Entry(DPAS).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                _PrDBContext.SaveChanges();
            }
        }

        public Employee_Details GetAuthEmpById(long empCode, int designationId, string designation, Employee_Details empDtl)
        {
            long? _FnDesigId = 0;
            long _opId = Convert.ToInt64(empDtl._OpId != null ? empDtl._OpId : 0);
            int[] Desig_Array = { 1, 2, 4, 6, 8 };
            if (Desig_Array.Contains(designationId))
            {
                if (designationId == 1)
                    _FnDesigId = 1;

                if (designationId == 2)
                    _FnDesigId = 2;

                if (designationId == 4)
                    _FnDesigId = 3;

                if (designationId == 6)
                    _FnDesigId = 4;

                if (designationId == 8)
                {
                    _FnDesigId = 4;
                    _opId = 0;
                }
            }
            var _obj = (from data in _PrDBContext.ADEMPLOYEE.Where(v => v.ADEMPCODE == empCode && v.ACTIVE == 1)
                        join _VW in _PrDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VW.ADEMPCODE
                        join _Desg in _PrDBContext.ADDESIGNATION on _VW.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
                        where _VW.SYKI == _Syki.SYKIID
                        && (Desig_Array.Contains(designationId) ? _VW.ADFUNCTIONALDESIGNATIONID == _FnDesigId
                        && (_opId > 0 ? _VW.OPERATIONID == _opId : 1 == 1) : (designation == "Director 2" ? (_Desg.DESCRIP == "Director" || _Desg.DESCRIP == "Senior Director") : _Desg.DESCRIP == designation))
                        select new Employee_Details
                        {
                            _ECode = data.ADEMPCODE,
                            _EFirstName = data.FIRSTNAME,
                            _ELastName = data.LASTNAME,
                            _EName = data.FIRSTNAME + " " + data.LASTNAME,
                            _EmailId = data.EMAILID,
                            _DesigId = _VW.ADDESIGNATIONID,
                            _Desig = _Desg.DESCRIP,
                            _FnDesigId = _VW.ADFUNCTIONALDESIGNATIONID,
                        }).FirstOrDefault();
            return _obj;
        }
        //added by aumento for SR86919
        public short AddNewEntryWithApprovalStatus(long prid, long currentUserId)
        {
            short retVal = 0;
            // change by aumento for Incident : 209904 IN :: SR80255
            using (var transaction = _PrDBContext.Database.BeginTransaction())
            {
                try
                {
                    var lastEntry = _PrDBContext.DGIT_PRAPPHISTORY
                        .Where(r => r.PRID == prid && r.APPROVAL_STATUS == 4)
                        .OrderByDescending(r => r.PRAPPHISTORY_ID)
                        .FirstOrDefault();
                    if (lastEntry != null)
                    {
                        long maxId = _PrDBContext.DGIT_PRAPPHISTORY.Max(x => x.PRAPPHISTORY_ID);
                        maxId++;
                        DGIT_PRAPPHISTORY newEntry = new DGIT_PRAPPHISTORY
                        {
                            PRAPPHISTORY_ID = maxId,
                            PRID = lastEntry.PRID,
                            ADEMPCODE = lastEntry.ADEMPCODE,
                            ADDEDBY = currentUserId,
                            ADDEDDATE = DateTime.Now,
                            APPROVAL_STATUS = 0,
                            APP_SEQ_HIS = lastEntry.APP_SEQ_HIS
                        };

                        _PrDBContext.DGIT_PRAPPHISTORY.Add(newEntry);

                        var prHeader = _PrDBContext.DGIT_PRHEADER
                            .Where(h => h.PRHEADERID == prid)
                            .FirstOrDefault();

                        if (prHeader != null)
                        {
                            prHeader.PROCESS_STATUS = 1;
                            prHeader.UPDATEDBY = currentUserId;
                            prHeader.UPDATEDATE = DateTime.Now;
                        }
                        _PrDBContext.SaveChanges();
                        transaction.Commit();
                        retVal = 1;
                    }
                    else
                    {
                        retVal = -2;
                    }
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                return retVal;
                // change by aumento for Incident : 209904 IN :: SR80255
            }
        }


        //added by aumento for SR86919
        public PRHeaderViewModel GetPRRequestById(long id)
        {
            var _obj = (from data in _PrDBContext.DGIT_PRHEADER.Where(x => x.PRHEADERID == id)
                        join _AddBy in _PrDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                        join _VWAssociate in _PrDBContext.VW_ASSOCIATELVLDETAILS on data.ADDEDBY equals _VWAssociate.ADEMPCODE
                        //join _Category in _PrDBContext.DGIT_PRCAT_MST on data.PRCAT equals _Category.CATID into _PRCat
                        //from _PRCategory in _PRCat.DefaultIfEmpty()
                        where _VWAssociate.SYKI == _Syki.SYKIID
                        select new PRHeaderViewModel
                        {
                            PRHEADERID = data.PRHEADERID,
                            IndentNo = data.INDENT_NO,
                            IndentDate = data.INDENT_DATE,
                            IndentAmount = data.INDENT_AMOUNT,
                            PROCESS_STATUS = data.PROCESS_STATUS,
                            PRTYPE = data.PRTYE,
                            ItemDetail = data.ITEM_DETAIL,
                            Remark = data.REMARK,
                            ADDEDBYNAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                            DATEADDED = data.DATEADDED,
                            CATID = (short)(data.PRCAT == null ? 0 : data.PRCAT),
                            IS_SISPR = data.IS_SISPR, //SIS PR Change
                            IS_NONSISPR = data.IS_NONSISPR, //SIS PR Change
                            //IPServiceMatDeclaration = (data.IS_IPRELATEDPR == 1 ? true : false),
                            IPServiceMatDeclaration = Convert.ToBoolean(data.IS_IPRELATEDPR ?? 0),
                            ARIBARFPID = data.ARIBARFPID,
                            ARIBASTATUS = data.ARIBASTATUS,
                            ARIBABuyer_ECODE = (long)(data.ARIBABUYER == null ? 0 : data.ARIBABUYER),
                            //Added by aumento for the SR73841============
                            ARIBASRNO = data.ARIBASRNO,
                            PRReleaseLocation = _PrDBContext.SYPLANT.Where(_Location => _Location.SYPLANTID == data.RELEASEPLANTID).Select(_Location => _Location.PLANTNAME).FirstOrDefault() ?? "",
                            //Added by ttl for the SR108972 > CR7273============
                            SLA_Category = (short)(data.PRCATEGORY == null ? 0 : data.PRCATEGORY),
                            //============================================
                            //prDetail = (from _PrDetail in _PrDBContext.DGIT_PRDETAIL.Where(d => d.PRHEADERID == data.PRHEADERID)
                            //            where _PrDetail.STATUS == 1
                            //            select new PRDetailViewModel
                            //            {
                            //                PRDTL_ID = _PrDetail.PRDTL_ID,
                            //                PRHEADERID = _PrDetail.PRHEADERID,
                            //                DOC_TYPE = _PrDetail.DOC_TYPE,
                            //                ADDITIONAL_INFO = _PrDetail.ADDITIONAL_INFO,
                            //                FILENAME = _PrDetail.FILENAME,
                            //                ADDEDDATE =  _PrDetail.ADDEDDATE //  added by Aumento as on 16072024
                            //            }).ToList(),
                            Emp_Detail = new Employee_Details
                            {
                                _ECode = _AddBy.ADEMPCODE,
                                _EName = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                                _EmailId = _AddBy.EMAILID,
                                _DOB = DateTime.Now,
                                _SecDescrip = _VWAssociate.SECTION,
                                _DepDesc = _VWAssociate.DEPARTMENT,
                                _DivDesc = _VWAssociate.DIVISION,
                                _OpId = _VWAssociate.OPERATIONID,
                                _OpDesc = _VWAssociate.OPERATION,
                                _SiteId = _VWAssociate.SYSITEID
                            },
                        }).FirstOrDefault();

            if (_obj != null)
            {
                _obj.prDetail = (from _PrDetail in _PrDBContext.DGIT_PRDETAIL
                                where _PrDetail.PRHEADERID == _obj.PRHEADERID && _PrDetail.STATUS == 1
                                select new PRDetailViewModel
                                {
                                    PRDTL_ID = _PrDetail.PRDTL_ID,
                                    PRHEADERID = _PrDetail.PRHEADERID,
                                    DOC_TYPE = _PrDetail.DOC_TYPE,
                                    ADDITIONAL_INFO = _PrDetail.ADDITIONAL_INFO,
                                    FILENAME = _PrDetail.FILENAME,
                                    ADDEDDATE = _PrDetail.ADDEDDATE
                                }).ToList();

                _obj.prAuthSeq = (from _POSeq in _PrDBContext.DGIT_PRAPPAUTHSEQ.Where(x => x.PRID == _obj.PRHEADERID)
                             join _AppSeqEmp in _PrDBContext.ADEMPLOYEE on _POSeq.ADEMPCODE equals _AppSeqEmp.ADEMPCODE
                             join _Vw in _PrDBContext.VW_ASSOCIATELVLDETAILS on _POSeq.ADEMPCODE equals _Vw.ADEMPCODE
                             join _Desg in _PrDBContext.ADDESIGNATION on _Vw.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
                             where _Vw.SYKI == _Syki.SYKIID
                             select new PRAppAuthSeqViewModel
                             {
                                 PRAPPAUTH_ID = _POSeq.PRAPPAUTH_ID,
                                 PRID = _POSeq.PRID,
                                 ADEMPCODE = _POSeq.ADEMPCODE,
                                 STATUS = _POSeq.STATUS,
                                 APP_SEQ = _POSeq.APP_SEQ,
                                 ADEMPNAME = _AppSeqEmp.FIRSTNAME + " " + _AppSeqEmp.LASTNAME,
                                 ADDESIGNATION = _Desg.DESCRIP,
                                 ADDEDDATE = _POSeq.ADDEDDATE,
                                 UPDATEBY = _POSeq.UPDATEBY,
                                 UPDATEDATE = _POSeq.UPDATEDATE,
                                 APPTYPE = _POSeq.APPTYPE,
                                 PRINTORDER = (short)(_POSeq.PRINTORDER == null ? 0 : _POSeq.PRINTORDER),
                                 ISPARALELLAPP = _POSeq.ISPARALLELAPP,
                                 ACTIONFOR = _POSeq.ACTIONFOR,
                                 IS_SISPR = _POSeq.IS_SISPR, //SIS PR Change
                                 APP_TYPEINFO = _POSeq.APP_TYPEINFO //SIS PR Change
                             }).OrderBy(b => b.APP_SEQ).ToList();

                _obj.prAppHis = (from _POAppHis in _PrDBContext.DGIT_PRAPPHISTORY.Where(x => x.PRID == _obj.PRHEADERID)
                            join _AppEmp in _PrDBContext.ADEMPLOYEE on _POAppHis.ADEMPCODE equals _AppEmp.ADEMPCODE
                            //Added By Aumento as on 02102024 for SR81488====================================
                            join _AppSeq in _PrDBContext.DGIT_PRAPPAUTHSEQ on _POAppHis.PRID equals _AppSeq.PRID
                            where _AppSeq.ADEMPCODE == _POAppHis.ADEMPCODE
                            && _AppSeq.APP_SEQ == _POAppHis.APP_SEQ_HIS
                            //=============================================================================== 
                            select new PRAppHistoryViewModel
                            {
                                PRAPPHISTORY_ID = _POAppHis.PRAPPHISTORY_ID,
                                PRID = _POAppHis.PRID,
                                ADEMPCODE = _POAppHis.ADEMPCODE,
                                APPROVAL_STATUS = _POAppHis.APPROVAL_STATUS,
                                APPROVAL_REMARK = _POAppHis.APPROVAL_REMARK,
                                APPEMP_NAME = _AppEmp.FIRSTNAME + " " + _AppEmp.LASTNAME + " - [" + _AppEmp.ADEMPCODE + "]",
                                APP_EMAIL = _AppEmp.EMAILID,
                                ADDEDDATE = _POAppHis.ADDEDDATE,
                                UPDATEBY = _POAppHis.UPDATEBY,
                                UPDATEDATE = _POAppHis.UPDATEDATE,
                                APPROVALDATE = _POAppHis.APP_DATE,
                                APPEMP_CODE = _AppEmp.ADEMPCODE.ToString(),
                                ACTIONFOR = _POAppHis.ACTIONFOR,
                                HOLD_BY = _POAppHis.HOLD_BY, //SIS PR Change
                                HOLD_DATE = _POAppHis.HOLD_DATE, //SIS PR Change
                                HOLD_REMARKS = _POAppHis.HOLD_REMARKS, //SIS PR Change
                                APP_TYPEINFO = _POAppHis.APP_TYPEINFO, //SIS PR Change
                                                                       //Added By Aumento as on 02102024 for SR81488====================================                                           
                                APP_SEQ = (short)_POAppHis.APP_SEQ_HIS, //  <!--Added by Aumento ::  SR68003-->
                                                                        //===============================================================================
                            }).OrderBy(o => o.PRAPPHISTORY_ID).ToList();


                _obj.prBuyerHis = (from _prBuyermap in _PrDBContext.DGIT_PRBUYER_MAP.Where(x => x.PRHEADERID == _obj.PRHEADERID && x.STATUS == 1)
                                          join _PrBuyerHis in _PrDBContext.DGIT_PRPURSTATUS on _prBuyermap.PRBUYERID equals _PrBuyerHis.PRBUYERID into _PRbuyer
                                          from _prbmap in _PRbuyer.DefaultIfEmpty()
                                              //join _prBuyermap in _PrDBContext.DGIT_PRBUYER_MAP.Where(x => x.PRHEADERID == data.PRHEADERID) on _PrBuyerHis.PRBUYERID equals _prBuyermap.PRBUYERID
                                          join _BuyerEmp in _PrDBContext.ADEMPLOYEE on _prBuyermap.BUYER_ECODE equals _BuyerEmp.ADEMPCODE
                                          select new PRPURStatusViewModel
                                          {
                                              PRPURSTATUSID = _prbmap != null ? _prbmap.PRPURSTATUSID : 0,
                                              PRHEADERID = _prBuyermap.PRHEADERID,
                                              ADDEDBY = _prBuyermap.ADDEDBY, //_prbmap != null ? _prbmap.ADDEDBY:0,
                                              ADDBY_NAME = _BuyerEmp.FIRSTNAME + " " + _BuyerEmp.LASTNAME + " - [" + _BuyerEmp.ADEMPCODE + "]",
                                              STATUS = _prbmap != null ? _prbmap.STATUS : (short)0,
                                              REMARK = _prbmap == null ? "" : _prbmap.REMARK,
                                              DATEADDED = _prbmap != null ? _prbmap.DATEADDED : _prBuyermap.DATEADDED,
                                          }).OrderBy(o => o.PRPURSTATUSID).ToList();

                _obj.skipAuthList = (from _AuthSkip in _PrDBContext.DGIT_PRAPPSKIP.Where(x => x.PRID == _obj.PRHEADERID)
                                            join _AppSkipEmp in _PrDBContext.ADEMPLOYEE on _AuthSkip.ADEMPCODE equals _AppSkipEmp.ADEMPCODE
                                            select new PRAPPSKIPViewModel
                                            {
                                                PRAPPSKIP_ID = _AuthSkip.PRAPPSKIP_ID,
                                                PRID = _AuthSkip.PRID,
                                                ADEMPCODE = _AuthSkip.ADEMPCODE,
                                                STATUS = _AuthSkip.STATUS,
                                                ADEMPNAME = _AppSkipEmp.FIRSTNAME + " " + _AppSkipEmp.LASTNAME,
                                                ADDEDDATE = _AuthSkip.ADDEDDATE,
                                                SKIPREMARK = _AuthSkip.SKIPREMARK
                                            }).ToList();            
            }

            //if (_obj.prAuthSeq.Count > 0)
            if (_obj.prAuthSeq != null && _obj.prAuthSeq.Count > 0)
            {
                long lastSendBackAppHisId = 0, maxapphisid = 0;
                short curr_seq = 0;
                DGIT_PRAPPHISTORY lastSendBackAppHis = _PrDBContext.DGIT_PRAPPHISTORY.Where(e => e.PRID == _obj.PRHEADERID && e.APPROVAL_STATUS == 2).OrderByDescending(o => o.PRAPPHISTORY_ID).FirstOrDefault();
                if (lastSendBackAppHis != null)
                {
                    lastSendBackAppHisId = lastSendBackAppHis.PRAPPHISTORY_ID;
                }
                var maxapphislst = _PrDBContext.DGIT_PRAPPHISTORY.Where(m => m.PRID == _obj.PRHEADERID && m.PRAPPHISTORY_ID > lastSendBackAppHisId).ToList();
                if (maxapphislst.Count() > 0)
                {
                    maxapphisid = maxapphislst.Max(m => m.PRAPPHISTORY_ID);
                }

                if (maxapphisid != 0)
                {
                    var ecodecurentapproval = _PrDBContext.DGIT_PRAPPHISTORY.Where(m => m.PRAPPHISTORY_ID == maxapphisid).FirstOrDefault().ADEMPCODE;
                    long appcnt = _PrDBContext.DGIT_PRAPPHISTORY.Where(m => m.PRID == _obj.PRHEADERID && m.PRAPPHISTORY_ID >= lastSendBackAppHisId && m.ADEMPCODE == ecodecurentapproval && (m.APPROVAL_STATUS == 1 || m.APPROVAL_STATUS == 0)).Count();
                    var appseqlst = _PrDBContext.DGIT_PRAPPAUTHSEQ.Where(s => s.PRID == _obj.PRHEADERID && s.ADEMPCODE == ecodecurentapproval).ToList();

                    if (appcnt <= 1)
                    {
                        curr_seq = appseqlst.OrderBy(m => m.APP_SEQ).FirstOrDefault().APP_SEQ;
                    }
                    else
                    {
                        curr_seq = appseqlst.OrderBy(m => m.APP_SEQ).ToArray()[appcnt - 1].APP_SEQ;
                    }
                }
                if (_obj.PROCESS_STATUS == 1  || _obj.PROCESS_STATUS == 5)
                {
                    //SIS PR Change
                    var currseqdtl = _obj.prAuthSeq.Where(m => m.APP_SEQ == curr_seq && m.IS_SISPR == 1).ToList();
                    if (currseqdtl.Count > 0)
                    {
                        _obj.IS_SISPRAPPROVAL = true;
                    }
                    //SIS PR Change

                    foreach (PRAppAuthSeqViewModel obj in _obj.prAuthSeq.Where(m => m.APP_SEQ > curr_seq).OrderBy(m => m.APP_SEQ))
                    {
                        //bool IsBeforeSendBackRecord = _PrDBContext.DGIT_PRAPPHISTORY.Any(w => w.PRID == obj.PRID && w.PRAPPHISTORY_ID <= lastSendBackAppHisId && w.ADEMPCODE == obj.ADEMPCODE);
                        //if ((IsBeforeSendBackRecord ? (!_obj.prAppHis.Any(r => r.PRID == obj.PRID && r.ADEMPCODE == obj.ADEMPCODE && r.PRAPPHISTORY_ID > lastSendBackAppHisId)) : (!_obj.prAppHis.Any(x => x.PRID == obj.PRID && x.ADEMPCODE == obj.ADEMPCODE))))
                        //{
                        //if (_obj.prAppHis.Where(m => m.APP_SEQ == obj.APP_SEQ && m.ADEMPCODE == obj.ADEMPCODE).Count() == 0)
                        //{

                        //New added (If condition and Int32 var) - PR Hold case - Aumento 22072024
                        Int32 appInfoRowCount = _obj.prAppHis.Where(x => x.APP_TYPEINFO == obj.APP_TYPEINFO && obj.ISPARALELLAPP == 1 && x.APPROVAL_STATUS == 4).Count();
                        //if (appInfoRowCount <= 1)
                        if (true) //Changed by TTL on 15-Sep-2025 against SR107472 > CR7069
                        {
                            _obj.prAppHis.Add(new PRAppHistoryViewModel
                            {
                                PRAPPHISTORY_ID = 0,
                                PRID = obj.PRID,
                                ADEMPCODE = obj.ADEMPCODE,
                                APPROVAL_STATUS = 0,
                                APPROVAL_REMARK = "",
                                APPEMP_NAME = obj.ADEMPNAME + " - [" + obj.ADEMPCODE + "]",
                                APP_SEQ = obj.APP_SEQ,
                                ACTIONFOR = obj.ACTIONFOR,
                                APP_TYPEINFO = obj.APP_TYPEINFO, //SIS PR Change
                            });
                        }
                        //}
                        //if (obj.ISPARALELLAPP == 1)
                        //{
                        //    var objp = _obj.prAuthSeq.Where(m => m.APP_SEQ == obj.APP_SEQ && m.ADEMPCODE != obj.ADEMPCODE).ToList();
                        //    foreach (var authlst in objp)
                        //    {
                        //        if (authlst != null && _obj.prAppHis.Where(m => m.APP_SEQ == obj.APP_SEQ && m.ADEMPCODE == authlst.ADEMPCODE).Count() == 0)
                        //        {
                        //            _obj.prAppHis.Add(new PRAppHistoryViewModel
                        //            {
                        //                PRAPPHISTORY_ID = 0,
                        //                PRID = obj.PRID,
                        //                ADEMPCODE = authlst.ADEMPCODE,
                        //                APPROVAL_STATUS = 0,
                        //                APPROVAL_REMARK = "",
                        //                APPEMP_NAME = authlst.ADEMPNAME + "[" + authlst.ADEMPCODE + "]",
                        //                APP_SEQ = authlst.APP_SEQ,
                        //            });
                        //        }
                        //    }
                        //}
                        //}
                    }
                }
                //if (_obj.PROCESS_STATUS == 0)
                //{
                //    foreach (PRAppAuthSeqViewModel obj in _obj.prAuthSeq)
                //    {
                //        _obj.prAppHis.Add(new PRAppHistoryViewModel
                //        {
                //            PRAPPHISTORY_ID = 0,
                //            PRID = obj.PRID,
                //            ADEMPCODE = obj.ADEMPCODE,
                //            APPROVAL_STATUS = 0,
                //            APPROVAL_REMARK = "",
                //            APPEMP_NAME = obj.ADEMPNAME + "[" + obj.ADEMPCODE + "]",
                //            APP_SEQ = obj.APP_SEQ,
                //        });
                //    }

                //}

            }
            if (_obj.CATID > 0)
            {
                var _catObj = _PrDBContext.DGIT_PRCAT_MST.Where(w => w.CATID == _obj.CATID).Select(s => s.CATDESC).FirstOrDefault();
                if (_catObj != null)
                {
                    _obj.CATEGORY = _catObj.ToString();
                }
            }

            return _obj;
        }

        //public short PRApproval(PRAppHistoryViewModel PHVM)
        public Tuple<short, short> PRApproval(PRAppHistoryViewModel PHVM)  //// Added By Aumento :: SR80255
        {
            short retVal = 0;
            short BuyerAssign = 0; //// Added By Aumento :: SR80255
            using (var transaction = _PrDBContext.Database.BeginTransaction())
            {
                try
                {
                    DGIT_PRAPPHISTORY DPAH = new DGIT_PRAPPHISTORY();
                    int DPAH_Approval_Status = -1; // added by Aumento as on 23072024
                    if (PHVM.PRID > 0)
                    {
                        //Added By Aumento as on 02102024 for SR81488========================================================================================                        
                        int CurrApp_seq = 0;
                        int NextApp_seq = 0;
                        Int64 PRAPPAUTHID = 0;
                        //=====================================================================================================================================

                        /////////// Update Approval Status //////////
                        //SIS PR Change - Start
                        //DPAH = _PrDBContext.DGIT_PRAPPHISTORY.Where(x => x.PRID == PHVM.PRID && x.ADEMPCODE == PHVM.ADEMPCODE && (x.APPROVAL_STATUS == 0)).FirstOrDefault();  // added by Aumento as on 23072024
                        DPAH = _PrDBContext.DGIT_PRAPPHISTORY.Where(x => x.PRID == PHVM.PRID && x.ADEMPCODE == PHVM.ADEMPCODE && (x.APPROVAL_STATUS == 0 || x.APPROVAL_STATUS == 4)).OrderByDescending(m=>m.PRAPPHISTORY_ID).FirstOrDefault();  // added by Aumento as on 23072024
                        DPAH_Approval_Status = DPAH != null ? DPAH.APPROVAL_STATUS : DPAH_Approval_Status;            // added by Aumento as on 23072024                                                                                                                                                                  //////    DPAH_Approval_Status = DPAH != null ? DPAH.APPROVAL_STATUS : DPAH_Approval_Status;


                        //if (DPAH != null && DPAH.APPROVAL_STATUS == 0 && PHVM.APPROVAL_STATUS != 4)  // { PHVM.APPROVAL_STATUS != 4}added by Aumento as on 23072024
                        //{
                        //    DPAH.APPROVAL_STATUS = PHVM.APPROVAL_STATUS;
                        //    DPAH.APPROVAL_REMARK = PHVM.APPROVAL_REMARK;
                        //    DPAH.UPDATEBY = PHVM.UPDATEBY;
                        //    DPAH.APP_DATE = DateTime.Now;
                        //    DPAH.UPDATEDATE = DateTime.Now;
                        //    _PrDBContext.Entry(DPAH).State = EntityState.Modified;
                        //    _PrDBContext.SaveChanges();
                        //}




                        if (DPAH != null && DPAH.APPROVAL_STATUS == 0)
                        {
                            DPAH.APPROVAL_STATUS = PHVM.APPROVAL_STATUS;
                            DPAH.APPROVAL_REMARK = PHVM.APPROVAL_REMARK;
                            DPAH.UPDATEBY = PHVM.UPDATEBY;
                            DPAH.APP_DATE = DateTime.Now;
                            DPAH.UPDATEDATE = DateTime.Now;
                            _PrDBContext.Entry(DPAH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _PrDBContext.SaveChanges();
                        }



                        // Added by Aumento as on 23072024
                        //if (DPAH != null && (DPAH_Approval_Status == 4 || DPAH_Approval_Status== 0) && (PHVM.APPROVAL_STATUS == 4 || PHVM.APPROVAL_STATUS == 2 || PHVM.APPROVAL_STATUS == 3)) //New added
                        if (DPAH != null && (DPAH_Approval_Status == 4) && (PHVM.APPROVAL_STATUS == 4 || PHVM.APPROVAL_STATUS == 2 || PHVM.APPROVAL_STATUS == 3 || PHVM.APPROVAL_STATUS == 1)) //New added
                        {
                            short Seq = Convert.ToInt16(_PrDBContext.DGIT_PRAPPAUTHSEQ.Where(s => s.PRID == PHVM.PRID && s.ADEMPCODE == PHVM.ADEMPCODE).Select(s => s.APP_SEQ).FirstOrDefault());
                            var seqModel_ = (from data in _PrDBContext.DGIT_PRAPPAUTHSEQ.Where(a => a.APP_SEQ == Seq && a.PRID == PHVM.PRID)
                                             select new PRAppAuthSeqViewModel
                                             {
                                                 PRAPPAUTH_ID = data.PRAPPAUTH_ID,
                                                 PRID = data.PRID,
                                                 ADEMPCODE = data.ADEMPCODE,
                                                 APP_SEQ = data.APP_SEQ,
                                                 STATUS = data.STATUS,
                                                 APPTYPE = data.APPTYPE,
                                                 APP_TYPEINFO = data.APP_TYPEINFO,
                                             }).FirstOrDefault();

                            SavePRAppHistoryApproval(PHVM.ADDEDBY, PHVM.PRID, seqModel_, PHVM.APPROVAL_REMARK, PHVM.APPROVAL_STATUS);




                            var DPH_ = _PrDBContext.DGIT_PRHEADER.Where(x => x.PRHEADERID == PHVM.PRID).SingleOrDefault();
                            if (DPH_ != null)
                            {

                                if (PHVM.APPROVAL_STATUS == 2)
                                {
                                    DPH_.PROCESS_STATUS = 0;
                                }
                                else if (PHVM.APPROVAL_STATUS == 3)
                                {
                                    DPH_.PROCESS_STATUS = 3;
                                }
                                else if (PHVM.APPROVAL_STATUS == 4)
                                {
                                    DPH_.PROCESS_STATUS = 5;
                                }
                                else
                                {
                                    DPH_.PROCESS_STATUS = 1;
                                }
                                DPH_.UPDATEDBY = PHVM.UPDATEBY;
                                DPH_.UPDATEDATE = DateTime.Now;
                                _PrDBContext.Entry(DPH_).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                _PrDBContext.SaveChanges();
                            }
                            // Added by Aumento as on 23072024

                            // Commented by Aumento as on 23072024
                            //var objlist = _PrDBContext.DGIT_PRAPPHISTORY.Where(x => x.PRID == PHVM.PRID && (x.APPROVAL_STATUS == 0));
                            //foreach (var obj in objlist)
                            //{
                            //    if (obj.ADEMPCODE == PHVM.ADEMPCODE)
                            //    {
                            //        obj.HOLD_DATE = DateTime.Now;
                            //        obj.HOLD_REMARKS = PHVM.APPROVAL_REMARK;
                            //        obj.HOLD_BY = PHVM.ADEMPCODE;
                            //        obj.UPDATEBY = PHVM.UPDATEBY;
                            //        obj.UPDATEDATE = DateTime.Now;
                            //        _PrDBContext.Entry(obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            //        _PrDBContext.SaveChanges();
                            //    }
                            //    else
                            //    {
                            //        obj.HOLD_BY = PHVM.ADEMPCODE;
                            //        obj.UPDATEBY = PHVM.UPDATEBY;
                            //        obj.UPDATEDATE = DateTime.Now;
                            //        _PrDBContext.Entry(obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            //        _PrDBContext.SaveChanges();
                            //    }
                            //}
                            // Commented by Aumento as on 23072024


                            if(PHVM.APPROVAL_STATUS == 4)
                            {
                                retVal = 1;
                                transaction.Commit();
                                //return retVal;
                                return Tuple.Create(retVal, BuyerAssign); //// Added By Aumento :: SR80255
                            }

                        }





                        //SIS PR Change - End


                        /// --- Save PO Approval Authority --- ///
                        PRAppAuthSeqViewModel seqModel = new PRAppAuthSeqViewModel();
                        var sendbackrecSeq = _PrDBContext.DGIT_PRAPPHISTORY.Where(m => m.PRID == PHVM.PRID && m.APPROVAL_STATUS == 2).ToList().OrderByDescending(m => m.PRAPPHISTORY_ID).FirstOrDefault();
                        long sendbackid = sendbackrecSeq != null ? sendbackrecSeq.PRAPPHISTORY_ID : 0;
                        //long appcnt1 = getAppCnt(PHVM.PRID, sendbackid, PHVM.ADEMPCODE, 1);
                        long appcnt = _PrDBContext.DGIT_PRAPPHISTORY.Where(m => m.PRID == PHVM.PRID && m.PRAPPHISTORY_ID >= sendbackid && m.ADEMPCODE == PHVM.ADEMPCODE && m.APPROVAL_STATUS == 1).Distinct().Count();
                        var appseqlst = _PrDBContext.DGIT_PRAPPAUTHSEQ.Where(s => s.PRID == PHVM.PRID && s.ADEMPCODE == PHVM.ADEMPCODE).ToList();
                        short curr_seq = 0, IS_PARR = 0;
                        if (appcnt <= 1)
                        {
                            curr_seq = appseqlst.OrderBy(m => m.APP_SEQ).FirstOrDefault().APP_SEQ;
                            IS_PARR = appseqlst.OrderBy(m => m.APP_SEQ).FirstOrDefault().ISPARALLELAPP;
                        }
                        else
                        {
                            curr_seq = appseqlst.OrderBy(m => m.APP_SEQ).ToArray()[appcnt - 1].APP_SEQ;
                            IS_PARR = appseqlst.OrderBy(m => m.APP_SEQ).ToArray()[appcnt - 1].ISPARALLELAPP;
                        }
                        //// --- Check Condition in case of Paralell Approval --- ////
                        if (IS_PARR == 1)
                        {
                            //short crntSeq = Convert.ToInt16(_PrDBContext.DGIT_PRAPPAUTHSEQ.Where(s => s.PRID == DPAH.PRID && s.ADEMPCODE == DPAH.ADEMPCODE).Select(s => s.APP_SEQ).FirstOrDefault());
                            List<PRAppAuthSeqViewModel> seqObjList = (from data in _PrDBContext.DGIT_PRAPPAUTHSEQ
                                                                      where data.PRID == DPAH.PRID && data.ADEMPCODE != DPAH.ADEMPCODE && data.ISPARALLELAPP == 1 && data.APP_SEQ == curr_seq
                                                                      select new PRAppAuthSeqViewModel
                                                                      {
                                                                          PRID = data.PRID,
                                                                          ADEMPCODE = data.ADEMPCODE,
                                                                          APP_SEQ = data.APP_SEQ
                                                                      }).ToList();
                            if (seqObjList.Count > 0)
                            {
                                foreach (PRAppAuthSeqViewModel seqObj in seqObjList)
                                {
                                    DGIT_PRAPPHISTORY REMOVE_OBJ = (from RDATA in _PrDBContext.DGIT_PRAPPHISTORY
                                                                    where RDATA.ADEMPCODE == seqObj.ADEMPCODE && RDATA.PRID == seqObj.PRID && (RDATA.APPROVAL_STATUS == 0)
                                                                    //join _AppSeq in _PrDBContext.DGIT_PRAPPAUTHSEQ on RDATA.PRID equals _AppSeq.PRID
                                                                    //where _AppSeq.ISPARALLELAPP == 1
                                                                    select RDATA).FirstOrDefault();
                                    if (REMOVE_OBJ != null)
                                    {
                                        _PrDBContext.DGIT_PRAPPHISTORY.Remove(REMOVE_OBJ);
                                        _PrDBContext.SaveChanges();

                                        //// --- Remove Seq --- ///
                                        DGIT_PRAPPAUTHSEQ REM_SEQ_OBJ = _PrDBContext.DGIT_PRAPPAUTHSEQ.Where(g => g.ADEMPCODE == REMOVE_OBJ.ADEMPCODE && g.PRID == seqObj.PRID && g.APP_SEQ == seqObj.APP_SEQ).FirstOrDefault();
                                        if (REM_SEQ_OBJ != null)
                                        {
                                            _PrDBContext.DGIT_PRAPPAUTHSEQ.Remove(REM_SEQ_OBJ);
                                            _PrDBContext.SaveChanges();
                                        }
                                    }
                                }
                            }
                            //seqModel = null;
                        }
                        //// --- End --- ////
                        if (PHVM.APPROVAL_STATUS == 1)
                        {

                            short nextSeq = 0;
                            if (appcnt <= 1)
                            {
                                nextSeq = (short)(appseqlst.OrderBy(m => m.APP_SEQ).FirstOrDefault().APP_SEQ + (short)1);
                            }
                            else
                            {
                                nextSeq = (short)(appseqlst.OrderBy(m => m.APP_SEQ).ToArray()[appcnt - 1].APP_SEQ + 1);
                            }
                            seqModel = (from data in _PrDBContext.DGIT_PRAPPAUTHSEQ.Where(a => a.APP_SEQ >= nextSeq && a.PRID == PHVM.PRID).ToList().OrderBy(m => m.APP_SEQ)
                                        select new PRAppAuthSeqViewModel
                                        {
                                            PRAPPAUTH_ID = data.PRAPPAUTH_ID,
                                            PRID = data.PRID,
                                            ADEMPCODE = data.ADEMPCODE,
                                            APP_SEQ = data.APP_SEQ,
                                            STATUS = data.STATUS,
                                            ISPARALELLAPP = data.ISPARALLELAPP,
                                            ACTIONFOR = data.ACTIONFOR,
                                            APP_TYPEINFO = data.APP_TYPEINFO //SIS PR Change
                                        }).FirstOrDefault();
                            if (seqModel != null)
                            {


                                //var ISalreadyapprove = _PrDBContext.DGIT_PRAPPHISTORY.Where(x => x.PRID == PHVM.PRID && x.ADEMPCODE == seqModel.ADEMPCODE && x.APPROVAL_STATUS == 1 && x.PRAPPHISTORY_ID > sendbackid).FirstOrDefault();
                                if (seqModel != null /*&& ISalreadyapprove == null*/)
                                {
                                    if (seqModel.ISPARALELLAPP == 1)
                                    {
                                        List<PRAppAuthSeqViewModel> newSeqModelList = (from data in _PrDBContext.DGIT_PRAPPAUTHSEQ.Where(m => m.PRID == PHVM.PRID).OrderBy(m => m.APP_SEQ).Where(a => a.APP_SEQ == seqModel.APP_SEQ && a.ISPARALLELAPP == 1)
                                                                                       select new PRAppAuthSeqViewModel
                                                                                       {
                                                                                           PRAPPAUTH_ID = data.PRAPPAUTH_ID,
                                                                                           PRID = data.PRID,
                                                                                           ADEMPCODE = data.ADEMPCODE,
                                                                                           APP_SEQ = data.APP_SEQ,
                                                                                           STATUS = data.STATUS,
                                                                                           ISPARALELLAPP = data.ISPARALLELAPP,
                                                                                           ACTIONFOR = data.ACTIONFOR,
                                                                                           APP_TYPEINFO = data.APP_TYPEINFO //SIS PR Change
                                                                                       }).ToList();
                                        if (newSeqModelList.Count > 0)
                                        {
                                            //foreach (PRAppAuthSeqViewModel objSeqModel in newSeqModelList)
                                            //{
                                            //    SavePRAppHis(PHVM.ADDEDBY, PHVM.PRID, objSeqModel);
                                            //}
                                            SavePRAppHis(PHVM.ADDEDBY, PHVM.PRID, newSeqModelList);
                                        }
                                    }
                                    else
                                    {
                                        List<PRAppAuthSeqViewModel> seqModellst = new List<PRAppAuthSeqViewModel>();
                                        seqModellst.Add(seqModel);
                                        SavePRAppHis(PHVM.ADDEDBY, PHVM.PRID, seqModellst);
                                        //    break;
                                    }
                                }
                                //else
                                //{
                                //    SavePRSkipAppHis(PHVM.ADDEDBY, PHVM.PRID, seqModel);
                                //    seqModel = (from data in _PrDBContext.DGIT_PRAPPAUTHSEQ.Where(a => a.APP_SEQ > seqModel.APP_SEQ && a.PRID == PHVM.PRID).ToList().OrderBy(m => m.APP_SEQ)
                                //                select new PRAppAuthSeqViewModel
                                //                {
                                //                    PRAPPAUTH_ID = data.PRAPPAUTH_ID,
                                //                    PRID = data.PRID,
                                //                    ADEMPCODE = data.ADEMPCODE,
                                //                    APP_SEQ = data.APP_SEQ,
                                //                    STATUS = data.STATUS,
                                //                    ISPARALELLAPP = data.ISPARALLELAPP
                                //                }).FirstOrDefault();

                                //    if (seqModel != null)
                                //    {
                                //        SavePRAppHis(PHVM.ADDEDBY, PHVM.PRID, seqModel);
                                //        //    break;
                                //    }
                                //}
                            }
                        }


                        //////// Update Process Status ////////
                        DGIT_PRHEADER DPH = new DGIT_PRHEADER(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                        DPH = _PrDBContext.DGIT_PRHEADER.Where(x => x.PRHEADERID == PHVM.PRID).SingleOrDefault();
                        
                        ////  Start Added By Aumento :: SR80255

                        var seqEntries = _PrDBContext.DGIT_PRAPPAUTHSEQ.Where(x => x.PRID == PHVM.PRID).ToList();

                        if (seqEntries.Any())
                        {

                            ////  Start Added By Aumento :: SR93953
                            //var maxSeqEntry = seqEntries.OrderByDescending(x => x.APP_SEQ).FirstOrDefault();
                            //var maxSeqNo = maxSeqEntry?.APP_SEQ ?? 0;
                            //var maxSeqAppCodeList = seqEntries.Where(x => x.APP_SEQ == maxSeqNo).Select(x => x.ADEMPCODE).ToList();
                            //var maxAppHisId = _PrDBContext.DGIT_PRAPPHISTORY.Where(x => x.PRID == PHVM.PRID && maxSeqAppCodeList.Contains(x.ADEMPCODE)).OrderByDescending(x => x.PRAPPHISTORY_ID).Select(x => x.PRAPPHISTORY_ID).FirstOrDefault();
                            //var maxAppStatus = maxAppHisId != 0 ? _PrDBContext.DGIT_PRAPPHISTORY.Where(x => x.PRAPPHISTORY_ID == maxAppHisId).Select(x => x.APPROVAL_STATUS).FirstOrDefault() : 0;


                            var maxSeqNo = seqEntries.Max(x => x.APP_SEQ);
                            var maxSeqAppCodeList = seqEntries.Where(x => x.APP_SEQ == maxSeqNo).Select(x => x.ADEMPCODE).ToList();
                            var maxAppHisId = _PrDBContext.DGIT_PRAPPHISTORY.Where(x => x.PRID == PHVM.PRID && maxSeqAppCodeList.Contains(x.ADEMPCODE) && x.APP_SEQ_HIS == maxSeqNo).OrderByDescending(x => x.PRAPPHISTORY_ID).Select(x => x.PRAPPHISTORY_ID).FirstOrDefault();
                            var maxAppStatus = maxAppHisId != 0 ? _PrDBContext.DGIT_PRAPPHISTORY.Where(x => x.PRAPPHISTORY_ID == maxAppHisId).Select(x => x.APPROVAL_STATUS).FirstOrDefault() : 0;

                            ////  Start Added By Aumento :: SR93953


                            if (DPH.ARIBARFPID != null && DPH.ARIBASRNO != null && DPH.ARIBABUYER != null && maxAppStatus == 1)
                            {
                                //var newBuyerId = _PrDBContext.DGIT_PRBUYER_MAP.Any() ? _PrDBContext.DGIT_PRBUYER_MAP.Max(x => x.PRBUYERID) + 1 : 1;

                                var maxBuyerId = _PrDBContext.DGIT_PRBUYER_MAP
                                    .OrderByDescending(x => x.PRBUYERID)
                                    .Select(x => x.PRBUYERID)
                                    .FirstOrDefault();

                                var newBuyerId = maxBuyerId + 1;

                                BuyerAssign = 1;
                                var buyerMap = new DGIT_PRBUYER_MAP
                                {
                                    PRBUYERID = newBuyerId,
                                    PRHEADERID = PHVM.PRID,
                                    STATUS = 1,
                                    BUYER_ECODE = DPH.ARIBABUYER ?? 0,
                                    ADDEDBY = DPH.ADDEDBY,
                                    //DATEADDED = DPH.DATEADDED
                                    DATEADDED = DateTime.Now
                                };

                                _PrDBContext.DGIT_PRBUYER_MAP.Add(buyerMap);
                                _PrDBContext.SaveChanges();


                            }

                        }


                        //// End Added By Aumento :: SR80255
                        if (DPH != null)
                        {
                            if (PHVM.APPROVAL_STATUS == 1 && seqModel == null)
                            {
                                DPH.PROCESS_STATUS = 2;
                            }
                            else if (PHVM.APPROVAL_STATUS == 2)
                            {
                                DPH.PROCESS_STATUS = 0;
                            }
                            else if (PHVM.APPROVAL_STATUS == 3)
                            {
                                DPH.PROCESS_STATUS = 3;
                            }
                            else if (PHVM.APPROVAL_STATUS == 4)
                            {
                                DPH.PROCESS_STATUS = 5;
                            }
                            else
                            {
                                DPH.PROCESS_STATUS = 1;
                            }
                            DPH.UPDATEDBY = PHVM.UPDATEBY;
                            DPH.UPDATEDATE = DateTime.Now;
                            _PrDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _PrDBContext.SaveChanges();
                        }
                        retVal = 1;
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                // return retVal;
                return Tuple.Create(retVal, BuyerAssign); //// Added By Aumento :: SR80255
            }
        }

        public short PRCancel(PRHeaderViewModel PHVM)
        {
            short retVal = 0;
            using (var transaction = _PrDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (PHVM.PRHEADERID > 0)
                    {
                        //////// Update Process Status ////////
                        DGIT_PRHEADER DPH = new DGIT_PRHEADER(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                        DPH = _PrDBContext.DGIT_PRHEADER.Where(x => x.PRHEADERID == PHVM.PRHEADERID).SingleOrDefault();
                        if (DPH != null)
                        {
                            DPH.PROCESS_STATUS = 3; //added by aumento for SR86919
                            //DPH.PROCESS_STATUS = 4;
                            DPH.UPDATEDBY = PHVM.UPDATEDBY;
                            DPH.UPDATEDATE = DateTime.Now;
                            _PrDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _PrDBContext.SaveChanges();
                        }
                        retVal = 1;
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                return retVal;
            }
        }

        public List<PRDetailViewModel> GetAttachmentDetail(long _PRHEADERID)
        {
            return (from _PODetail in _PrDBContext.DGIT_PRDETAIL.Where(d => d.PRHEADERID == _PRHEADERID)
                    where _PODetail.STATUS == 1
                    select new PRDetailViewModel
                    {
                        PRDTL_ID = _PODetail.PRDTL_ID,
                        PRHEADERID = _PODetail.PRHEADERID,
                        DOC_TYPE = _PODetail.DOC_TYPE,
                        ADDITIONAL_INFO = _PODetail.ADDITIONAL_INFO,
                        FILENAME = _PODetail.FILENAME,
                    }).ToList();
        }

        public short DeleteAttachment(string fileName, string docType, long PRHEADERID)
        {
            short retVal = 0;
            if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(docType) && PRHEADERID > 0)
            {
                DGIT_PRDETAIL DT = _PrDBContext.DGIT_PRDETAIL.Where(x => x.FILENAME == fileName && x.DOC_TYPE == docType && x.PRHEADERID == PRHEADERID).FirstOrDefault();
                if (DT != null)
                {
                    _PrDBContext.DGIT_PRDETAIL.Remove(DT);
                    _PrDBContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }

        public VW_DGIT_PRADDAUTH_MAP GetPRAdditionalApp(long PRType, long ORGlvlid)
        {
            VW_DGIT_PRADDAUTH_MAP obj = new VW_DGIT_PRADDAUTH_MAP();
            DGIT_PRADDAUTH_MAP DT = _PrDBContext.DGIT_PRADDAUTH_MAP.Where(x => x.PRTYPEID == PRType && x.ORGLVLID == ORGlvlid).FirstOrDefault();
            if (DT != null)
            {
                obj = (from data in _PrDBContext.ADORGLEVELHEAD.Where(m => m.ADORGLEVELID == DT.RES_ORGLVLID && m.ISACTIVE == 1)
                       join og in _PrDBContext.ADORGLEVEL on data.ADORGLEVELID equals og.ADORGLEVELID
                       select new VW_DGIT_PRADDAUTH_MAP
                       {
                           ADEMPCODE = data.ADEMPCODE,
                           ADORGLEVELTYPEID = og.ADORGLEVELTYPEID,

                       }).ToList().FirstOrDefault();
            }
            return obj;
        }

        public SearchIndent PRDashboard(SearchIndent VM)
        {
            DateTime ReqDateFrom = DateTime.Now.Date;
            DateTime ReqDateTo = DateTime.Now.Date;
            DateTime reportfrom = DateTime.ParseExact("20-JAN-2021", "dd-MMM-yyyy", null);
            if (!string.IsNullOrEmpty(VM.Startdate))
            {
                ReqDateFrom = DateTime.Parse(VM.Startdate).Date;
                string strReqDateFrom = ReqDateFrom.ToString("dd-MMM-yyyy");
                ReqDateFrom = DateTime.ParseExact(strReqDateFrom, "dd-MMM-yyyy", null);

            }
            if (!string.IsNullOrEmpty(VM.ENDDATE))
            {
                ReqDateTo = DateTime.Parse(VM.ENDDATE).Date;
                string strReqDateTo = ReqDateTo.ToString("dd-MMM-yyyy");

                ReqDateTo = DateTime.ParseExact(strReqDateTo + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            }

            List<PRHeaderViewModel> obj = new List<PRHeaderViewModel>();

            var maxIds = _PrDBContext.DGIT_PRHEADER
                .Where(f => f.PROCESS_STATUS == 0 || f.PROCESS_STATUS == 1 || (f.PROCESS_STATUS == 2 && f.PRCAT == VM.CATID))
                .GroupBy(f => f.INDENT_NO)
                .Select(g => g.Max(m => m.PRHEADERID));
            
            // Step 3: Main query
         obj = (
           from data in _PrDBContext.DGIT_PRHEADER.Where(m => m.DATEADDED > reportfrom && m.PROCESS_STATUS == 2)
                 .Join(maxIds,
                          data => data.PRHEADERID,
                          maxId => maxId,
                          (data, maxId) => data)
           join PRD in _PrDBContext.DGIT_PRDETAIL.Where(m => m.DOC_TYPE == "PRA") on data.PRHEADERID equals PRD.PRHEADERID
           join emp in _PrDBContext.VW_ASSOCIATELVLDETAILS.Where(v => v.SYKI == _Syki.SYKIID) on data.ADDEDBY equals emp.ADEMPCODE
           join pl in _PrDBContext.SYPLANT on emp.SYPLANTID equals pl.SYPLANTID
           join _ed in _PrDBContext.ADEMPLOYEE on data.ADDEDBY equals _ed.ADEMPCODE
           join _prBM in _PrDBContext.DGIT_PRBUYER_MAP.Where(m => m.STATUS == 1) on data.PRHEADERID equals _prBM.PRHEADERID into _prmJoin
           from PR_BS in _prmJoin.DefaultIfEmpty()
           join _PO in (
                 from link in _PrDBContext.DGIT_POPRLINK_DTL
                 join po in _PrDBContext.DGIT_POHEADER
                     on link.POHEADERID equals po.POHEADERID
                 where link.STATUS == 1
                       && po.PROCESS_STATUS != 3
                       && po.PROCESS_STATUS != 4
                 select new
                 {
                     PRNO = link.PRNO,
                     POHEADERID = (long?)link.POHEADERID,
                     po
                 }
             ) on data.INDENT_NO.Trim() equals _PO.PRNO into _POH
           from _POHH in _POH.DefaultIfEmpty()
           where emp.SYKI == _Syki.SYKIID
               && (string.IsNullOrEmpty(VM.Startdate) || data.UPDATEDATE >= ReqDateFrom)
               && (string.IsNullOrEmpty(VM.ENDDATE) || data.UPDATEDATE <= ReqDateTo)
               && (VM.OperationID != 0 ? emp.OPERATIONID == VM.OperationID : VM.MapOperationIDs.Contains((long)emp.OPERATIONID))
               && (VM.DivisionID == 0 || emp.DIVISIONID == VM.DivisionID)
               && (VM.DEPTID == 0 || emp.DEPARTMENTID == VM.DEPTID)
               && (VM.SECID == 0 || emp.SECTIONID == VM.SECID)
               && (string.IsNullOrEmpty(VM.IndentNo.Trim()) || data.INDENT_NO.Trim() == VM.IndentNo.Trim())
               && (VM.ecode == 0 || data.ADDEDBY == VM.ecode)
               && (VM.PlantID == 0 || emp.SYPLANTID == VM.PlantID)
               && data.PRCAT == VM.CATID
           select new PRHeaderViewModel
           {
               ADDEDBYNAME = _ed.FIRSTNAME + " " + _ed.LASTNAME,
               DATEADDED = data.DATEADDED,
               IndentAmount = data.INDENT_AMOUNT,
               IndentNo = data.INDENT_NO.Trim(),
               ARIBARFPID = data.ARIBARFPID,
               ARIBASTATUS = data.ARIBASTATUS,
               ARIBABuyer_ECODE = (long)(data.ARIBABUYER ?? 0),
               PRHEADERID = data.PRHEADERID,
               ItemDetail = data.ITEM_DETAIL,
               PRStatus = (PR_BS == null ? (short)0 : (short)1),
               UPDATEDATE = data.UPDATEDATE,
               //PROCESS_STATUS = (_POHH != null && (_POHH.PROCESS_STATUS == 0 || _POHH.PROCESS_STATUS == 1 || _POHH.PROCESS_STATUS == 2) ? (short)0 : (short)1),
               //PROCESS_STATUS = (_POHH != null ? (short)0 : (short)1),
               PROCESS_STATUS = (_POHH.po != null ? (short)0 : (short)1),
               PRATTACHMENT = PRD.FILENAME,
               //Added by aumento for the SR73841============
               ARIBASRNO = data.ARIBASRNO,
               //============================================ 
               Emp_Detail = new Employee_Details
               {
                   _ECode = _ed.ADEMPCODE,
                   _EName = _ed.FIRSTNAME + " " + _ed.LASTNAME,
                   _OpDesc = emp.OPERATION,
                   _DivDesc = emp.DIVISION,
                   _DepDesc = emp.DEPARTMENT,
                   _SecDescrip = emp.SECTION,
                   _Desig = pl.PLANTNAME,
               }
           })
            //.Where(w => VM.PRStatus == 0 ?
            //    (w.PRStatus == 0 && w.PROCESS_STATUS == 1) :
            //    ((w.BUYER_MODEL != null && w.BUYER_MODEL.Count > 0 && w.PROCESS_STATUS == 1) || w.PROCESS_STATUS == 0))
            //.OrderBy(o => o.UPDATEDATE)
            .ToList();
            
            var headerIds = obj.Select(o => o.PRHEADERID).ToList();
            var buyerModels = (
                    from bm in _PrDBContext.DGIT_PRBUYER_MAP
                    where headerIds.Contains(bm.PRHEADERID) && bm.STATUS == 1
                    join emp in _PrDBContext.ADEMPLOYEE on bm.BUYER_ECODE equals emp.ADEMPCODE
                    select new
                    {
                        bm.PRHEADERID,
                        Model = new PRBuyerMapViewModel
                        {
                            PRBUYERID = bm.PRBUYERID,
                            BUYER_ECODE = bm.BUYER_ECODE,
                            BUYER_NAME = emp.FIRSTNAME + " " + emp.LASTNAME,
                            ADDEDBY = bm.ADDEDBY,
                            DATEADDED = bm.DATEADDED
                        }
                    }
                ).ToList();
            var buyerLookup = buyerModels
                .GroupBy(x => x.PRHEADERID)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Model).OrderBy(m => m.PRBUYERID).ToList());


            // Fetch all buyer models in one go
            var buyerModels_new = (
                from bm in _PrDBContext.DGIT_PRBUYER_MAP
                where headerIds.Contains(bm.PRHEADERID)
                join emp in _PrDBContext.ADEMPLOYEE on bm.BUYER_ECODE equals emp.ADEMPCODE
                select new
                {
                    bm.PRHEADERID,
                    Model = new PRBuyerMapViewModel
                    {
                        PRBUYERID = bm.PRBUYERID,
                        BUYER_ECODE = bm.BUYER_ECODE,
                        BUYER_NAME = emp.FIRSTNAME + " " + emp.LASTNAME,
                        ADDEDBY = bm.ADDEDBY,
                        DATEADDED = bm.DATEADDED
                    }
                }
            ).ToList();

            // Group by PRHEADERID
            var buyerLookup_new = buyerModels_new
                .GroupBy(x => x.PRHEADERID)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Model).OrderBy(m => m.PRBUYERID).ToList());


            var holdStatuses = (
                    from bm in _PrDBContext.DGIT_PRBUYER_MAP
                    join ps in _PrDBContext.DGIT_PRPURSTATUS on bm.PRBUYERID equals ps.PRBUYERID
                    where headerIds.Contains(bm.PRHEADERID)
                          && bm.STATUS == 1
                          && ps.STATUS == 1
                    select new
                    {
                        bm.PRHEADERID,
                        Model = new PRPURStatusViewModel
                        {
                            PRPURSTATUSID = ps.PRPURSTATUSID,
                            DATEADDED = ps.DATEADDED,
                            REMARK = ps.REMARK,
                            STATUS = ps.STATUS
                        }
                    }
                ).ToList();

            var holdLookup = holdStatuses
                .GroupBy(x => x.PRHEADERID)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Model).OrderBy(m => m.PRPURSTATUSID).ToList());


            var unholdStatuses = (
                        from bm in _PrDBContext.DGIT_PRBUYER_MAP
                        join ps in _PrDBContext.DGIT_PRPURSTATUS on bm.PRBUYERID equals ps.PRBUYERID
                        where headerIds.Contains(bm.PRHEADERID)
                              && bm.STATUS == 1
                              && ps.STATUS == 4
                        select new
                        {
                            bm.PRHEADERID,
                            Model = new PRPURStatusViewModel
                            {
                                PRPURSTATUSID = ps.PRPURSTATUSID,
                                DATEADDED = ps.DATEADDED,
                                REMARK = null,   // default value
                                STATUS = ps.STATUS
                            }
                        }
                    ).ToList();

            var unholdLookup = unholdStatuses
                .GroupBy(x => x.PRHEADERID)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Model).OrderBy(m => m.PRPURSTATUSID).ToList());

            var sendBackStatuses = (
                    from bm in _PrDBContext.DGIT_PRBUYER_MAP
                    join ps in _PrDBContext.DGIT_PRPURSTATUS on bm.PRBUYERID equals ps.PRBUYERID
                    where headerIds.Contains(bm.PRHEADERID)
                          && bm.STATUS == 1
                          && ps.STATUS == 2
                    select new
                    {
                        bm.PRHEADERID,
                        Model = new PRPURStatusViewModel
                        {
                            PRPURSTATUSID = ps.PRPURSTATUSID,
                            DATEADDED = ps.DATEADDED,
                            REMARK = ps.REMARK,
                            STATUS = ps.STATUS
                        }
                    }
                ).ToList();

            var sendBackLookup = sendBackStatuses
                    .GroupBy(x => x.PRHEADERID)
                    .ToDictionary(g => g.Key,g => g.Select(x => x.Model).OrderBy(m => m.PRPURSTATUSID).ToList());

            foreach (var item in obj)
            {
                if (buyerLookup.TryGetValue(item.PRHEADERID, out var models))
                    item.BUYER_MODEL = models;
                else
                    item.BUYER_MODEL = new List<PRBuyerMapViewModel>();

                if (buyerLookup_new.TryGetValue(item.PRHEADERID, out var models_new))
                    item.BUYER_MODEL_NEW = models_new;
                else
                    item.BUYER_MODEL_NEW = new List<PRBuyerMapViewModel>();

                if (holdLookup.TryGetValue(item.PRHEADERID, out var modelsBuyerHis))
                    item.prBuyerHis_HOLD = modelsBuyerHis;
                else
                    item.prBuyerHis_HOLD = new List<PRPURStatusViewModel>();

                if (unholdLookup.TryGetValue(item.PRHEADERID, out var modelsUNHOLD))
                    item.prBuyerHis_UNHOLD = modelsUNHOLD;
                else
                    item.prBuyerHis_UNHOLD = new List<PRPURStatusViewModel>();

                if (sendBackLookup.TryGetValue(item.PRHEADERID, out var modelsSendBack))
                {
                    item.prBuyerHis_SENDACK = modelsSendBack;
                }
                else
                {
                    item.prBuyerHis_SENDACK = new List<PRPURStatusViewModel>();
                }
            }

            obj = obj
             .Where(w => VM.PRStatus == 0
                 ? (w.PRStatus == 0 && w.PROCESS_STATUS == 1)
                 : ((w.BUYER_MODEL != null && w.BUYER_MODEL.Count > 0 && w.PROCESS_STATUS == 1) || w.PROCESS_STATUS == 0))
             .OrderBy(o => o.UPDATEDATE)
             .ToList();
            VM.SearchResult = obj;
            return VM;
        }

        public List<ADORGLEVEL> GetOrgLevelList(long typeId)
        {
            var iList = from data in _PrDBContext.ADORGLEVEL
                        where data.ACTIVE == 1 && data.SYKIID == _Syki.SYKIID && data.ADORGLEVELTYPEID == typeId
                        select data;
            return iList.ToList();
        }

        public Tuple<short, List<ADORGLEVEL>> BindOperationForOPMap(long loginUser)
        {
            short catid = 0;
            Tuple<short, List<ADORGLEVEL>> _retVal_tuple;
            var iList = from data in _PrDBContext.DGIT_PROPERATIONMAP
                        join orgLvl in _PrDBContext.ADORGLEVEL on data.ADORGLVLID equals orgLvl.ADORGLEVELID
                        where data.STATUS == 1
                        && data.ADEMPCODE == loginUser
                        && orgLvl.ADORGLEVELTYPEID == 1
                        select orgLvl;

            var dobj = _PrDBContext.DGIT_PROPERATIONMAP.Where(n => n.ADEMPCODE == loginUser && n.STATUS == 1).ToList();
            if (dobj.Count > 0)
            {
                catid = dobj.Select(m => m.PRCAT).Distinct().FirstOrDefault();
            }
            return _retVal_tuple = new Tuple<short, List<ADORGLEVEL>>(catid, iList.ToList());
        }

        public List<PR_Div_Dep_SecViewModel> BindDivision(long op_Id)
        {
            List<PR_Div_Dep_SecViewModel> iList = new List<PR_Div_Dep_SecViewModel>();
            var iColl = (from data in _PrDBContext.VW_ASSOCIATELVLDETAILS.Where(e => e.ACTIVE == 1
                         && e.SYKI == _Syki.SYKIID && e.DIVISIONID != null && e.DIVISIONID != 0
                         && e.OPERATIONID == (op_Id == 0 ? e.OPERATIONID : op_Id))
                         select new
                         {
                             data.DIVISIONID,
                             data.DIVISION
                         }).Distinct().ToList();
            foreach (var obj in iColl)
            {
                iList.Add(new PR_Div_Dep_SecViewModel
                {
                    Value = Convert.ToInt64(obj.DIVISIONID == null ? 0 : obj.DIVISIONID),
                    Text = obj.DIVISION
                });
            }
            return iList;
        }

        public List<PR_Div_Dep_SecViewModel> BindDepartment(long div_Id, long op_Id)
        {
            List<PR_Div_Dep_SecViewModel> iList = new List<PR_Div_Dep_SecViewModel>();
            var iColl = (from data in _PrDBContext.VW_ASSOCIATELVLDETAILS.Where(e => e.ACTIVE == 1
                         && e.SYKI == _Syki.SYKIID && e.DEPARTMENTID != null && e.DEPARTMENTID != 0
                         && e.DIVISIONID == (div_Id == 0 ? e.DIVISIONID : div_Id)
                         /*&& e.OPERATIONID == (op_Id == 0 ? e.OPERATIONID : op_Id)*/)
                         select new
                         {
                             data.DEPARTMENTID,
                             data.DEPARTMENT
                         }).Distinct().ToList();
            foreach (var obj in iColl)
            {
                iList.Add(new PR_Div_Dep_SecViewModel
                {
                    Value = Convert.ToInt64(obj.DEPARTMENTID == null ? 0 : obj.DEPARTMENTID),
                    Text = obj.DEPARTMENT
                });
            }
            return iList;
        }

        public List<PR_Div_Dep_SecViewModel> BindSection(long dep_Id, long div_Id, long op_Id)
        {
            List<PR_Div_Dep_SecViewModel> iList = new List<PR_Div_Dep_SecViewModel>();
            var iColl = (from data in _PrDBContext.VW_ASSOCIATELVLDETAILS.Where(e => e.ACTIVE == 1
                         && e.SYKI == _Syki.SYKIID && e.SECTION != null && e.SECTIONID != 0
                         && e.DEPARTMENTID == (dep_Id == 0 ? e.DEPARTMENTID : dep_Id)
                         //&& e.DIVISIONID == (div_Id == 0 ? e.DIVISIONID : div_Id)
                         //&& e.OPERATIONID == (op_Id == 0 ? e.OPERATIONID : op_Id)
                         )
                         select new
                         {
                             data.SECTIONID,
                             data.SECTION
                         }).Distinct().ToList();
            foreach (var obj in iColl)
            {
                iList.Add(new PR_Div_Dep_SecViewModel
                {
                    Value = Convert.ToInt64(obj.SECTIONID == null ? 0 : obj.SECTIONID),
                    Text = obj.SECTION
                });
            }
            return iList;
        }

        public List<PR_Div_Dep_SecViewModel> BindPlant(long Plant_Id)
        {
            List<PR_Div_Dep_SecViewModel> iList = new List<PR_Div_Dep_SecViewModel>();
            var iColl = (from data in _PrDBContext.SYPLANT
                         where (Plant_Id == 0 ? true : data.SYPLANTID == Plant_Id)
                         && data.ACTIVE == 1
                         select new
                         {
                             data.SYPLANTID,
                             data.PLANTNAME
                         }).Distinct().ToList();
            foreach (var obj in iColl)
            {
                iList.Add(new PR_Div_Dep_SecViewModel
                {
                    Value = obj.SYPLANTID,
                    Text = obj.PLANTNAME
                });
            }
            return iList;
        }

        public PRPURStatusViewModel GetPRPURStatusById(long id)
        {
            var _obj = (from data in _PrDBContext.DGIT_PRPURSTATUS
                        join _prs in _PrDBContext.DGIT_PRBUYER_MAP on data.PRBUYERID equals _prs.PRBUYERID
                        where _prs.PRHEADERID == id && _prs.STATUS == 1
                        select new PRPURStatusViewModel
                        {
                            PRPURSTATUSID = data.PRPURSTATUSID,
                            PRHEADERID = _prs.PRHEADERID,
                            STATUS = data.STATUS,
                            REMARK = data.REMARK,
                            DATEADDED = data.DATEADDED,
                            ADDEDBY = data.ADDEDBY
                        }).FirstOrDefault();
            return _obj;
        }

        public short UpdatePRStatus(PRPURStatusViewModel PPVM)
        {
            short retval = 0;
            try
            {
                DGIT_PRPURSTATUS DPRS = new DGIT_PRPURSTATUS();
                long BUYERID = _PrDBContext.DGIT_PRBUYER_MAP.Where(w => w.PRHEADERID == PPVM.PRHEADERID && w.STATUS == 1 && w.BUYER_ECODE == PPVM.ADDEDBY).Select(s => s.PRBUYERID).FirstOrDefault();


                DPRS = new DGIT_PRPURSTATUS();
                if (_PrDBContext.DGIT_PRPURSTATUS.Count() == 0)
                {
                    DPRS.PRPURSTATUSID = 1;
                }
                else
                {
                    DPRS.PRPURSTATUSID = _PrDBContext.DGIT_PRPURSTATUS.Max(x => x.PRPURSTATUSID) + 1;
                }

                DPRS.PRBUYERID = BUYERID;
                DPRS.STATUS = PPVM.STATUS;
                DPRS.REMARK = PPVM.REMARK;
                DPRS.ADDEDBY = PPVM.ADDEDBY;
                DPRS.DATEADDED = DateTime.Now;
                _PrDBContext.Entry(DPRS).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                _PrDBContext.SaveChanges();

                retval = 1;
                var objBuyerList = _PrDBContext.DGIT_PRBUYER_MAP.Where(m => m.PRHEADERID == PPVM.PRHEADERID && m.STATUS == 1 && m.PRBUYERID != BUYERID).ToList();
                foreach (var obj in objBuyerList)
                {
                    obj.STATUS = 0;
                    obj.MODIFIEDDATE = DateTime.Now;
                    obj.MODIFIEDBY = PPVM.ADDEDBY;
                    _PrDBContext.Entry(obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _PrDBContext.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                retval = -1;
            }
            return retval;
        }

        public short ChangeSLACategory(long PRID, short selectedSLACategory, long Updatedby)
        {
            short retVal = 0;
            using (var transaction = _PrDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (PRID > 0)
                    {
                        // Update SLA Category //
                        DGIT_PRHEADER DPH = new DGIT_PRHEADER();
                        DPH = _PrDBContext.DGIT_PRHEADER.Where(x => x.PRHEADERID == PRID).SingleOrDefault();
                        if (DPH != null)
                        {
                            DPH.PRCATEGORY = selectedSLACategory; 
                            DPH.CATUPDATEDBY = Updatedby;
                            DPH.CATUPDATEDATE = DateTime.Now;
                            _PrDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _PrDBContext.SaveChanges();
                        }
                        retVal = 1;
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                return retVal;
            }
        }

        //public SearchIndentUser PRUserDashboard(SearchIndentUser VM)
        //{
        //    DateTime ReqDateFrom = DateTime.Now.Date;
        //    DateTime ReqDateTo = DateTime.Now.Date;
        //    DateTime reportfrom = DateTime.ParseExact("15-JUL-2021", "dd-MMM-yyyy", null);
        //    if (!string.IsNullOrEmpty(VM.Startdate))
        //    {
        //        ReqDateFrom = DateTime.ParseExact(VM.Startdate, "dd-MMM-yyyy", null);
        //    }
        //    if (!string.IsNullOrEmpty(VM.ENDDATE))
        //    {
        //        ReqDateTo = DateTime.ParseExact(VM.ENDDATE + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
        //    }
        //    var objDisprlist = (from data in _PrDBContext.DGIT_PRHEADER
        //                        group data by new { data.INDENT_NO } into g
        //                        select new
        //                        {
        //                            PRNO = g.Key.INDENT_NO,
        //                            PRHEADERID = g.Max(m => m.PRHEADERID)
        //                        }
        //               ).ToList();

        //    long[] lstprids = objDisprlist.Select(m => m.PRHEADERID).ToArray();
        //    var prapphistory = (from _prhl in _PrDBContext.DGIT_PRAPPHISTORY
        //                        where
        //                       (from _PRAppHis in _PrDBContext.DGIT_PRAPPHISTORY.Where(x => lstprids.Contains(x.PRID))
        //                        group _PRAppHis by new { _PRAppHis.PRID } into g
        //                        select new
        //                        {
        //                            PRAPPHISTORY_ID = g.Max(t => t.PRAPPHISTORY_ID),
        //                        }).ToList().Select(m => m.PRAPPHISTORY_ID).Contains(_prhl.PRAPPHISTORY_ID)
        //                        select _prhl).ToList();

        //    List<PRHeaderViewModel> obj = new List<PRHeaderViewModel>();
        //    obj = (from data in _PrDBContext.DGIT_PRHEADER.Where(z => lstprids.Contains(z.PRHEADERID) && (z.PROCESS_STATUS == 2 || z.PROCESS_STATUS == 1 || z.PROCESS_STATUS == 0))
        //           join emp in _PrDBContext.VW_ASSOCIATELVLDETAILS on data.ADDEDBY equals emp.ADEMPCODE
        //           join _ed in _PrDBContext.ADEMPLOYEE on data.ADDEDBY equals _ed.ADEMPCODE
        //           //join _prBM in _PrDBContext.DGIT_PRBUYER_MAP.Where(m => m.STATUS == 1) on data.PRHEADERID equals _prBM.PRHEADERID into _prmJoin
        //           //from PR_BS in _prmJoin.DefaultIfEmpty()
        //           //join _prStatus in _PrDBContext.DGIT_PRPURSTATUS on PR_BS.PRBUYERID equals _prStatus.PRBUYERID into _prStJoin
        //           //from PR_Status in _prStJoin.DefaultIfEmpty()
        //           where (!string.IsNullOrEmpty(VM.Startdate) ? (data.DATEADDED >= ReqDateFrom) : true)
        //           && (!string.IsNullOrEmpty(VM.ENDDATE) ? (data.DATEADDED <= ReqDateTo) : true)
        //           && emp.SYKI == _Syki.SYKIID
        //           && (VM.OperationID != 0 ? emp.OPERATIONID == VM.OperationID : true)
        //           && (VM.DivisionID != 0 ? emp.DIVISIONID == VM.DivisionID : true)
        //           && (VM.DEPTID != 0 ? emp.DEPARTMENTID == VM.DEPTID : true)
        //           && (VM.SECID != 0 ? emp.SECTIONID == VM.SECID : true)
        //           && (string.IsNullOrEmpty(VM.IndentNo) ? true : data.INDENT_NO == VM.IndentNo)
        //           && (VM.ecode == 0 ? true : data.ADDEDBY == VM.ecode)
        //           select new PRHeaderViewModel
        //           {
        //               ADDEDBYNAME = _ed.FIRSTNAME + " " + _ed.LASTNAME,
        //               DATEADDED = data.DATEADDED,
        //               IndentAmount = data.INDENT_AMOUNT,
        //               IndentNo = data.INDENT_NO,
        //               PRHEADERID = data.PRHEADERID,
        //               ItemDetail = data.ITEM_DETAIL,
        //               PROCESS_STATUS = data.PROCESS_STATUS == 2 ? (short)3 : (short)1,
        //               //PRStatus = PR_Status == null ? (short)0 : PR_Status.STATUS,
        //               Emp_Detail = new Employee_Details
        //               {
        //                   _ECode = _ed.ADEMPCODE,
        //                   _EName = _ed.FIRSTNAME + " " + _ed.LASTNAME,
        //                   _OpDesc = emp.OPERATION,
        //                   _DivDesc = emp.DIVISION,
        //                   _DepDesc = emp.DEPARTMENT,
        //                   _SecDescrip = emp.SECTION
        //               }
        //           }).ToList();
        //    //if (VM.PRStatus != -1)
        //    //{
        //    //    obj = obj.Where(w => w.PRStatus == VM.PRStatus).ToList();
        //    //}
        //    foreach (var t in obj)
        //    {
        //        var appdate = prapphistory.Where(m => m.PRID == t.PRHEADERID).FirstOrDefault();
        //        if (appdate != null)
        //        {
        //            if (appdate.APP_DATE != null)
        //                t.UPDATEDATE = appdate.APP_DATE;
        //            else
        //                t.UPDATEDATE = DateTime.Today;
        //        }

        //    }
        //    VM.SearchResult = obj.Where(o => o.UPDATEDATE > reportfrom).OrderBy(o => o.UPDATEDATE).ToList();
        //    var LstPRNO = VM.SearchResult.Select(m => m.IndentNo.Trim()).Distinct().ToList();
        //    //Added to get PO deatil of PR
        //    List<POMappingViewModel> objpolist = (from data in _PrDBContext.DGIT_POPRLINK_DTL
        //                                          join POH in _PrDBContext.DGIT_POHEADER on data.POHEADERID equals POH.POHEADERID
        //                                          where LstPRNO.Contains(data.PRNO)
        //                                          group data by new { data.PRNO, POH.PONO } into g
        //                                          select new POMappingViewModel
        //                                          {
        //                                              PONO = g.Key.PONO,
        //                                              POPRLNK_ID = g.Max(m => m.POPRLNK_ID),
        //                                              PRNO = g.Key.PRNO,
        //                                              POHEADERID = g.Max(m => m.POHEADERID)
        //                                          }
        //               ).ToList();

        //    long[] objprlnk = objpolist.Select(b => b.POPRLNK_ID).ToArray();
        //    List<POHeaderViewModel> objpodtl = new List<POHeaderViewModel>();
        //    objpodtl = (from data in _PrDBContext.DGIT_POHEADER
        //                join prlnk in _PrDBContext.DGIT_POPRLINK_DTL on data.POHEADERID equals prlnk.POHEADERID
        //                where objprlnk.Contains(prlnk.POPRLNK_ID)
        //                && (data.PROCESS_STATUS == 0 || data.PROCESS_STATUS == 1 || data.PROCESS_STATUS == 2)
        //                select new POHeaderViewModel
        //                {
        //                    POHEADERID = data.POHEADERID,
        //                    PONO = data.PONO,
        //                    PROCESS_STATUS = data.PROCESS_STATUS,
        //                    DATEADDED = data.DATEADDED
        //                }
        //               ).ToList();

        //    long[] arrhdlst = objpodtl.Select(n => n.POHEADERID).ToArray();
        //    var pohuid = (from _POAppHis in _PrDBContext.DGIT_POAPPHISTORY.Where(x => arrhdlst.Contains(x.POID))
        //                  group _POAppHis by new { _POAppHis.POID } into g
        //                  select new POAppHistoryViewModel
        //                  {
        //                      POAPPHISTORY_ID = g.Max(t => t.POAPPHISTORY_ID),
        //                  }).ToList();
        //    long[] aph = pohuid.Select(m => m.POAPPHISTORY_ID).ToArray();
        //    var poAppHis = (from _POAppHis in _PrDBContext.DGIT_POAPPHISTORY.Where(x => aph.Contains(x.POAPPHISTORY_ID))
        //                    select new POAppHistoryViewModel
        //                    {
        //                        POAPPHISTORY_ID = _POAppHis.POAPPHISTORY_ID,
        //                        POID = _POAppHis.POID,
        //                        APPROVAL_STATUS = _POAppHis.APPROVAL_STATUS,
        //                        ADDEDDATE = _POAppHis.ADDEDDATE,
        //                        APPROVALDATE = _POAppHis.APP_DATE
        //                    }).ToList();

        //    foreach (var objpo in objpodtl)
        //    {
        //        objpo.PRNO_ARRAY = objpolist.Where(m => m.POHEADERID == objpo.POHEADERID).Select(m => m.PRNO).ToArray();
        //        objpo.poAppHis = poAppHis.Where(m => m.POID == objpo.POHEADERID).ToList();
        //    }
        //    foreach (var prd_v in VM.SearchResult)
        //    {
        //        if (prd_v.PROCESS_STATUS == 3)
        //        {
        //            var lstpo = objpodtl.Where(m => m.PRNO_ARRAY.Contains(prd_v.IndentNo)).ToList().OrderBy(m => m.PROCESS_STATUS);
        //            if (lstpo != null && lstpo.Count() > 0)
        //            {
        //                int prst = lstpo.FirstOrDefault().PROCESS_STATUS == 0 ? 5 : lstpo.FirstOrDefault().PROCESS_STATUS == 1 ? 5 : lstpo.FirstOrDefault().PROCESS_STATUS == 2 ? 7 : 3;
        //                prd_v.PROCESS_STATUS = Convert.ToInt16(prst);
        //            }
        //        }
        //    }
        //    VM.PODeatil = objpodtl;
        //    if (VM.PRStatus != -1)
        //        VM.SearchResult = VM.SearchResult.Where(m => m.PROCESS_STATUS == VM.PRStatus).ToList();
        //    return VM;
        //}

        public SearchIndentUser PRUserDashboard(SearchIndentUser VM)
        {
            DateTime ReqDateFrom = DateTime.Now.Date;
            DateTime ReqDateTo = DateTime.Now.Date;
            DateTime reportfrom = DateTime.ParseExact("15-JUL-2021", "dd-MMM-yyyy", null);
            if (!string.IsNullOrEmpty(VM.Startdate))
            {
                ReqDateFrom = DateTime.ParseExact(VM.Startdate, "dd-MMM-yyyy", null);
            }
            if (!string.IsNullOrEmpty(VM.ENDDATE))
            {
                ReqDateTo = DateTime.ParseExact(VM.ENDDATE + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
            }


            List<VM_VW_PRUSERDASHBOARD> obj = new List<VM_VW_PRUSERDASHBOARD>();
            obj = (from data in _PrDBContext.VW_PRUSERDASHBOARD
                   where (!string.IsNullOrEmpty(VM.Startdate) ? (data.DATEADDED >= ReqDateFrom) : true)
                   && (!string.IsNullOrEmpty(VM.ENDDATE) ? (data.DATEADDED <= ReqDateTo) : true)
                   && (VM.OperationID != 0 ? data.OPERATIONID == VM.OperationID : true)
                   && (VM.DivisionID != 0 ? data.DIVISIONID == VM.DivisionID : true)
                   && (VM.DEPTID != 0 ? data.DEPARTMENTID == VM.DEPTID : true)
                   && (VM.SECID != 0 ? data.SECTIONID == VM.SECID : true)
                   && (string.IsNullOrEmpty(VM.IndentNo) ? true : data.INDENT_NO == VM.IndentNo)
                   && (VM.ecode == 0 ? true : data.ADDEDBY == VM.ecode)
                   && (VM.PRStatus != -1 ? data.POSTATUS == VM.PRStatus : true)
                   &&(string.IsNullOrEmpty(VM.ITEM_DETAIL)?true:data.ITEM_DETAIL.ToLower().Trim().Contains(VM.ITEM_DETAIL.ToLower().Trim()))
                   select new VM_VW_PRUSERDASHBOARD
                   {
                       ADDEDBYNAME = data.ADDEDBYNAME,
                       DATEADDED = data.DATEADDED,
                       INDENT_AMOUNT = data.INDENT_AMOUNT,
                       INDENT_NO = data.INDENT_NO,
                       ARIBARFPID = data.ARIBARFPID,
                       ARIBASTATUS = data.ARIBASTATUS,
                       ARIBABuyer_ECODE = (long)(data.ARIBABUYER == null ? 0 : data.ARIBABUYER),
                       PRHEADERID = data.PRHEADERID,
                       ITEM_DETAIL = data.ITEM_DETAIL,
                       POSTATUS = data.POSTATUS,
                       BUYERNAME = data.BUYERNAME,
                       BUYERSTATUS = data.BUYERSTATUS,
                       ADDEDBY = data.ADDEDBY,
                       DEPARTMENTID = data.DEPARTMENTID,
                       DIVISIONID = data.DIVISIONID,
                       OPERATIONID = data.OPERATIONID,
                       POAPPDATE = data.POAPPDATE,
                       POAPPROVALSTATUS = data.POAPPROVALSTATUS,
                       PONO = data.PONO,
                       PRAPPDATE = data.PRAPPDATE,
                       SECTIONID = data.SECTIONID,
                       UPDATEDATE = data.UPDATEDATE,
                       DEPARTMENT = data.DEPARTMENT,
                       DIVISION = data.DIVISION,
                       OPERATION = data.OPERATION,
                       SECTION = data.SECTION,
                       BUYSITEID = data.BUYSITEID,
                       //Added by aumento for the SR73841============
                       ARIBASRNO = data.ARIBASRNO,
                       //============================================ 
                   }).ToList();
            VM.PRUSERDASHBOARD = obj.ToList();

            List<VM_HMSIHOLIDAYS> objcalender = new List<VM_HMSIHOLIDAYS>();
            objcalender = (from data in _PrDBContext.HMSIHOLIDAYS.Where(m => m.ACTIVE == 1 && m.MONTHDATEYEAR < DateTime.Today.Date)
                           select new VM_HMSIHOLIDAYS
                           {
                               ACTIVE = data.ACTIVE,
                               ADDEDBY = data.ADDEDBY,
                               DATEADDED = data.DATEADDED,
                               DATELSTMOD = data.DATELSTMOD,
                               HMSIHOLIDAYSID = data.HMSIHOLIDAYSID,
                               HOLIDAYDESCRIPTION = data.HOLIDAYDESCRIPTION,
                               ISHALFDAY = data.ISHALFDAY,
                               MODIFIEDBY = data.MODIFIEDBY,
                               MONTHDATEYEAR = data.MONTHDATEYEAR,
                               SYSITEID = data.SYSITEID
                           }
                         ).ToList();

            VM.BYE_Cal = objcalender;
            return VM;
        }

        public SearchIndentUser PRAgeingHistory(SearchIndentUser VM)
        {
            var objDisprlist = (from data in _PrDBContext.DGIT_PRHEADER
                                where data.INDENT_NO == VM.IndentNo
                                group data by new { data.INDENT_NO } into g
                                select new
                                {
                                    PRNO = g.Key.INDENT_NO,
                                    PRHEADERID = g.Max(m => m.PRHEADERID)
                                }
                       ).ToList();

            long[] lstprids = objDisprlist.Select(m => m.PRHEADERID).ToArray();

            //List<PRAppHistoryViewModel> prapphistory = (from _prhl in _PrDBContext.DGIT_PRAPPHISTORY
            //                                            where
            //                                           (from _PRAppHis in _PrDBContext.DGIT_PRAPPHISTORY.Where(x => lstprids.Contains(x.PRID))
            //                                            group _PRAppHis by new { _PRAppHis.PRID } into g
            //                                            select new
            //                                            {
            //                                                PRAPPHISTORY_ID = g.Max(t => t.PRAPPHISTORY_ID),
            //                                            }).ToList().Select(m => m.PRAPPHISTORY_ID).Contains(_prhl.PRAPPHISTORY_ID)
            //                                            select new PRAppHistoryViewModel
            //                                            {
            //                                                APPROVALDATE = _prhl.APP_DATE,
            //                                                APPROVAL_REMARK = "PR Approval Date"
            //                                            }).ToList();


            var latestHistoryIds = _PrDBContext.DGIT_PRAPPHISTORY
                .Where(x => lstprids.Contains(x.PRID))
                .GroupBy(x => x.PRID)
                .Select(g => g.Max(t => t.PRAPPHISTORY_ID))
                .ToList(); 


            List<PRAppHistoryViewModel> prapphistory = (from _prhl in _PrDBContext.DGIT_PRAPPHISTORY
                       where latestHistoryIds.Contains(_prhl.PRAPPHISTORY_ID)
                       select new PRAppHistoryViewModel
                       {
                           APPROVALDATE = _prhl.APP_DATE,
                           APPROVAL_REMARK = "PR Approval Date"
                       }).ToList();

            //Added to get PO deatil of PR
            List<POMappingViewModel> objpolist = (from data in _PrDBContext.DGIT_POPRLINK_DTL
                                                  join POH in _PrDBContext.DGIT_POHEADER on data.POHEADERID equals POH.POHEADERID
                                                  where data.PRNO == VM.IndentNo
                                                  group data by new { data.PRNO, POH.PONO } into g
                                                  select new POMappingViewModel
                                                  {
                                                      PONO = g.Key.PONO,
                                                      POPRLNK_ID = g.Max(m => m.POPRLNK_ID),
                                                      PRNO = g.Key.PRNO,
                                                      POHEADERID = g.Max(m => m.POHEADERID)
                                                  }
                       ).ToList();

            long[] objprlnk = objpolist.Select(b => b.POPRLNK_ID??0).ToArray();

            List<POHeaderViewModel> objpodtl = new List<POHeaderViewModel>();
            objpodtl = (from data in _PrDBContext.DGIT_POHEADER
                        join prlnk in _PrDBContext.DGIT_POPRLINK_DTL on data.POHEADERID equals prlnk.POHEADERID
                        where objprlnk.Contains(prlnk.POPRLNK_ID)
                        && (data.PROCESS_STATUS == 0 || data.PROCESS_STATUS == 1 || data.PROCESS_STATUS == 2)
                        select new POHeaderViewModel
                        {
                            POHEADERID = data.POHEADERID,
                            PONO = data.PONO,
                            PROCESS_STATUS = data.PROCESS_STATUS,
                            DATEADDED = data.DATEADDED,
                            poAppHis = (from _pohis in _PrDBContext.DGIT_POAPPHISTORY
                                        join _emp in _PrDBContext.ADEMPLOYEE on _pohis.ADEMPCODE equals _emp.ADEMPCODE
                                        where _pohis.POID == data.POHEADERID
                                        select new POAppHistoryViewModel
                                        {
                                            POAPPHISTORY_ID = _pohis.POAPPHISTORY_ID,
                                            ADDEDDATE = _pohis.ADDEDDATE,
                                            ADEMPCODE = _pohis.ADEMPCODE,
                                            APPROVALDATE = _pohis.APP_DATE,
                                            APPROVAL_REMARK = _pohis.APPROVAL_REMARK,
                                            APPROVAL_STATUS = _pohis.APPROVAL_STATUS,
                                            APPEMP_NAME = _emp.FIRSTNAME + " " + _emp.LASTNAME
                                        }
                                      ).ToList()
                        }
                       ).ToList();

            var _prsubhis = prapphistory.FirstOrDefault();
            int _dateNum = _prsubhis.APPROVALDATE.Value.Day;
            if (_dateNum > 0 && _dateNum <= 10)
            {
                _prsubhis.APPROVALDATE = DateTime.ParseExact("11-" + string.Format("{0:MMM-yyyy}", _prsubhis.APPROVALDATE), "dd-MMM-yyyy", null);
            }
            else if (_dateNum > 10 && _dateNum <= 20)
            {
                _prsubhis.APPROVALDATE = DateTime.ParseExact("21-" + string.Format("{0:MMM-yyyy}", _prsubhis.APPROVALDATE), "dd-MMM-yyyy", null);
            }
            else if (_dateNum > 20 && _dateNum <= 31)
            {
                _prsubhis.APPROVALDATE = DateTime.ParseExact("01-" + string.Format("{0:MMM-yyyy}", _prsubhis.APPROVALDATE.Value.AddMonths(1)), "dd-MMM-yyyy", null);

            }
            VM.LastPRApproval = _prsubhis;
            VM.PODeatil = objpodtl;

            List<VM_VW_PRUSERDASHBOARD> BYE_HIS = new List<VM_VW_PRUSERDASHBOARD>();
            BYE_HIS = (from data in _PrDBContext.VW_PRUSERDASHBOARD
                       join po in _PrDBContext.DGIT_POPRLINK_DTL on data.INDENT_NO equals po.PRNO
                       where lstprids.Contains(data.PRHEADERID)
                       select new VM_VW_PRUSERDASHBOARD
                       {
                           ADDEDBY = data.ADDEDBY,
                           BUYERNAME = data.BUYERNAME,
                           BUYERSTATUS = data.BUYERSTATUS,
                           BUYSITEID = data.BUYSITEID,
                           PRHEADERID = data.PRHEADERID,
                           PONO = data.PONO
                       }).ToList();

            List<VM_HMSIHOLIDAYS> objcalender = new List<VM_HMSIHOLIDAYS>();
            objcalender = (from data in _PrDBContext.HMSIHOLIDAYS.Where(m => m.ACTIVE == 1 && m.MONTHDATEYEAR < DateTime.Today.Date)
                           select new VM_HMSIHOLIDAYS
                           {
                               ACTIVE = data.ACTIVE,
                               ADDEDBY = data.ADDEDBY,
                               DATEADDED = data.DATEADDED,
                               DATELSTMOD = data.DATELSTMOD,
                               HMSIHOLIDAYSID = data.HMSIHOLIDAYSID,
                               HOLIDAYDESCRIPTION = data.HOLIDAYDESCRIPTION,
                               ISHALFDAY = data.ISHALFDAY,
                               MODIFIEDBY = data.MODIFIEDBY,
                               MONTHDATEYEAR = data.MONTHDATEYEAR,
                               SYSITEID = data.SYSITEID
                           }
                         ).ToList();

            VM.BYE_Cal = objcalender;
            VM.BYE_History = BYE_HIS;
            return VM;
        }

        public long GetPRNextApprovalId(long PRID, long ecode)
        {
            long retval = 0;
            var _obj = (from _PO in _PrDBContext.DGIT_PRHEADER
                        join _PAH in _PrDBContext.DGIT_PRAPPHISTORY on _PO.PRHEADERID equals _PAH.PRID
                        where _PO.PROCESS_STATUS == 1 && _PAH.APPROVAL_STATUS == 0 && _PAH.ADEMPCODE == ecode
                        select _PO).OrderBy(p => p.DATEADDED).Where(m => m.PRHEADERID > PRID).ToList();

            if (_obj != null && _obj.Count > 0)
            {
                retval = _obj.FirstOrDefault().PRHEADERID; //Return 1 if any of PR approval not finished for related PO
            }
            return retval;
        }

        public short AssignBuyer(PRBuyerMapViewModel PBVM)
        {
            short retval = 0;
            try
            {
                DGIT_PRBUYER_MAP DPBS = new DGIT_PRBUYER_MAP();
                //// --- Update Buyer Status --- ////
                if (PBVM.PRBUYERID > 0)
                {
                    DPBS = _PrDBContext.DGIT_PRBUYER_MAP.Where(x => x.PRBUYERID == PBVM.PRBUYERID).FirstOrDefault();
                    DPBS.STATUS = 0;
                    DPBS.MODIFIEDBY = PBVM.ADDEDBY;
                    DPBS.MODIFIEDDATE = DateTime.Now;
                    _PrDBContext.Entry(DPBS).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _PrDBContext.SaveChanges();
                }

                /// --- Add New Buyer --- ///
                DPBS = new DGIT_PRBUYER_MAP();
                if (_PrDBContext.DGIT_PRBUYER_MAP.Count() == 0)
                {
                    DPBS.PRBUYERID = 1;
                }
                else
                {
                    DPBS.PRBUYERID = _PrDBContext.DGIT_PRBUYER_MAP.Max(x => x.PRBUYERID) + 1;
                }
                DPBS.PRHEADERID = PBVM.PRHEADERID;
                DPBS.STATUS = PBVM.STATUS;
                DPBS.BUYER_ECODE = PBVM.BUYER_ECODE;
                DPBS.ADDEDBY = PBVM.ADDEDBY;
                DPBS.DATEADDED = DateTime.Now;
                _PrDBContext.Entry(DPBS).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                _PrDBContext.SaveChanges();
                retval = 1;
            }
            catch (Exception ex)
            {
                retval = -1;
            }
            return retval;
        }

        public PRBuyerMapViewModel GetPRBuyerByHeaderId(long id)
        {
            var _obj = (from data in _PrDBContext.DGIT_PRBUYER_MAP
                        join _buyerMst in _PrDBContext.DGIT_PRBUYERMST on data.PRBUYERID equals _buyerMst.PRBUYERMSTID
                        where data.PRHEADERID == id && data.STATUS == 1
                        select new PRBuyerMapViewModel
                        {
                            PRBUYERID = data.PRBUYERID,
                            PRHEADERID = data.PRHEADERID,
                            BUYER_ECODE = data.BUYER_ECODE,
                            STATUS = data.STATUS,
                            DATEADDED = data.DATEADDED,
                            ADDEDBY = data.ADDEDBY
                        }).FirstOrDefault();
            return _obj;
        }

        public List<PRBuyerMstViewModel> GetBuyerMstList(long? GPType, long? ADOrgLevelId)
        {
            var iList = (from data in _PrDBContext.DGIT_PRBUYERMST
                         join _Emp in _PrDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
                         where data.PRCAT == GPType
                         && data.ADORGLEVELID == ADOrgLevelId
                         && data.ACTIVE == 1
                         && data.SYKIID == _Syki.SYKIID
                         select new PRBuyerMstViewModel
                         {
                             PRBUYERMSTID = data.PRBUYERMSTID,
                             PRCAT = data.PRCAT,
                             ADEMPCODE = data.ADEMPCODE,
                             ADEMPNAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - " + _Emp.ADEMPCODE,
                             DATEADDED = data.DATEADDED,
                             ADDEDBY = data.ADDEDBY
                         }).ToList();
            return iList.OrderBy(o => o.ADEMPNAME).ToList();
        }

        public short PRChangeCategory(long PRID, short selectedCategory, long Updatedby)
        {
            short retVal = 0;
            using (var transaction = _PrDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (PRID > 0)
                    {
                        //////// Update Process Status ////////
                        DGIT_PRHEADER DPH = new DGIT_PRHEADER(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                        DPH = _PrDBContext.DGIT_PRHEADER.Where(x => x.PRHEADERID == PRID).SingleOrDefault();
                        if (DPH != null)
                        {
                            DPH.PRCAT = selectedCategory; //DPH.PRCAT == 1 ? (short)2 : (short)1;
                                                          //DPH.UPDATEDBY = Updatedby;
                                                          //DPH.UPDATEDATE = DateTime.Now;
                            DPH.CATUPDATEDBY = Updatedby;
                            DPH.CATUPDATEDATE = DateTime.Now;
                            _PrDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _PrDBContext.SaveChanges();
                        }
                        retVal = 1;
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                return retVal;
            }
        }

        public short UpdateAllocationStatus(long PRID, long Updatedby)
        {
            short retVal = 0;
            using (var transaction = _PrDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (PRID > 0)
                    {
                        //////// Update Process Status ////////
                        DGIT_PRBUYER_MAP DPM = new DGIT_PRBUYER_MAP();
                        DPM = _PrDBContext.DGIT_PRBUYER_MAP.Where(x => x.PRHEADERID == PRID && x.STATUS == 1).FirstOrDefault();
                        if (DPM != null)
                        {
                            DPM.STATUS = 0;
                            DPM.MODIFIEDBY = Updatedby;
                            DPM.MODIFIEDDATE = DateTime.Now;
                            _PrDBContext.Entry(DPM).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _PrDBContext.SaveChanges();
                        }
                        retVal = 1;
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                return retVal;
            }
        }

        public SearchIndent PRBuyerDashboard(SearchIndent VM)
        {
            DateTime ReqDateFrom = DateTime.Now.Date;
            DateTime ReqDateTo = DateTime.Now.Date;
            DateTime reportfrom = DateTime.ParseExact("20-JAN-2021", "dd-MMM-yyyy", null);
            if (!string.IsNullOrEmpty(VM.Startdate))
            {
                //ReqDateFrom = DateTime.ParseExact(VM.Startdate, "dd-MMM-yyyy", null);
                ReqDateFrom = DateTime.Parse(VM.Startdate).Date;
                string strReqDateFrom = ReqDateFrom.ToString("dd-MMM-yyyy");
                ReqDateFrom = DateTime.ParseExact(strReqDateFrom, "dd-MMM-yyyy", null);

            }
            if (!string.IsNullOrEmpty(VM.ENDDATE))
            {
                //ReqDateTo = DateTime.ParseExact(VM.ENDDATE + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
                ReqDateTo = DateTime.Parse(VM.ENDDATE).Date;
                string strReqDateTo = ReqDateTo.ToString("dd-MMM-yyyy");

                ReqDateTo = DateTime.ParseExact(strReqDateTo + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            }

            long[] prapphistory = (from _PRAppHis in _PrDBContext.DGIT_PRAPPHISTORY
                                   join _PRM in _PrDBContext.DGIT_PRBUYER_MAP on _PRAppHis.PRID equals _PRM.PRHEADERID
                                   where _PRM.BUYER_ECODE == VM.BuyerID
                                   && _PRM.STATUS == 1
                                   group _PRAppHis by new { _PRAppHis.PRID } into g
                                   select new
                                   {
                                       PRAPPHISTORY_ID = g.Max(t => t.PRAPPHISTORY_ID),
                                   }).Select(m => m.PRAPPHISTORY_ID).ToArray();
            long[] LNGPRPURSTATUSID = (from _PRAppHis in _PrDBContext.DGIT_PRBUYER_MAP
                                       join _PRM in _PrDBContext.DGIT_PRPURSTATUS on _PRAppHis.PRBUYERID equals _PRM.PRBUYERID
                                       where _PRAppHis.BUYER_ECODE == VM.BuyerID
                                       && _PRAppHis.STATUS == 1
                                       group _PRM by new { _PRM.PRBUYERID } into g
                                       select new
                                       {
                                           PRPURSTATUSID = g.Max(t => t.PRPURSTATUSID),
                                       }).Select(m => m.PRPURSTATUSID).ToArray();
            List<PRHeaderViewModel> obj = new List<PRHeaderViewModel>();
            obj = (from data in _PrDBContext.DGIT_PRHEADER
                   join PRM in _PrDBContext.DGIT_PRBUYER_MAP.Where(m => m.STATUS == 1) on data.PRHEADERID equals PRM.PRHEADERID
                   join PRD in _PrDBContext.DGIT_PRDETAIL.Where(m => m.DOC_TYPE == "PRA") on data.PRHEADERID equals PRD.PRHEADERID
                   join emp in _PrDBContext.VW_ASSOCIATELVLDETAILS on data.ADDEDBY equals emp.ADEMPCODE
                   join pl in _PrDBContext.SYPLANT on emp.SYPLANTID equals pl.SYPLANTID
                   join _ed in _PrDBContext.ADEMPLOYEE on data.ADDEDBY equals _ed.ADEMPCODE
                   join _prapp in _PrDBContext.DGIT_PRAPPHISTORY.Where(m => prapphistory.Contains(m.PRAPPHISTORY_ID)) on data.PRHEADERID equals _prapp.PRID into _prah
                   from PR_AH in _prah.DefaultIfEmpty()
                   join PRP in _PrDBContext.DGIT_PRPURSTATUS.Where(m => LNGPRPURSTATUSID.Contains(m.PRPURSTATUSID)) on PRM.PRBUYERID equals PRP.PRBUYERID into _PRPS
                   from PRPS in _PRPS.DefaultIfEmpty()
                   join _POPR in _PrDBContext.DGIT_POPRLINK_DTL.Where(n => n.STATUS == 1) on data.INDENT_NO.Trim() equals _POPR.PRNO into _PRP
                   from PR_PO in _PRP.DefaultIfEmpty()

                       //join _PO in _PrDBContext.DGIT_POHEADER.Where(n => (n.PROCESS_STATUS != 4 || n.PROCESS_STATUS != 3)) on PR_PO.POHEADERID equals _PO.POHEADERID into _POH //Send back case correction
                       //from _POHH in _POH.DefaultIfEmpty()


                   join _PO in
                    (from _data2 in _PrDBContext.DGIT_POPRLINK_DTL
                     join _data3 in _PrDBContext.DGIT_POHEADER on _data2.POHEADERID equals _data3.POHEADERID
                     where _data3.PROCESS_STATUS != 3 && _data3.PROCESS_STATUS != 4
                     select new { _data2.POHEADERID, _data2.PRNO }
                     ) on PR_PO.PRNO equals _PO.PRNO into _POH
                   from _POHH in _POH.DefaultIfEmpty()


            where data.PROCESS_STATUS == 2
                   && emp.SYKI == _Syki.SYKIID
                   && (string.IsNullOrEmpty(VM.IndentNo.Trim()) ? true : data.INDENT_NO.Trim() == VM.IndentNo.Trim())
                   && (VM.ecode == 0 ? true : data.ADDEDBY == VM.ecode)
                   && PRM.BUYER_ECODE == VM.BuyerID
                   //&& data.PRCAT == VM.CATID
                   select new PRHeaderViewModel
                   {
                       ADDEDBYNAME = _ed.FIRSTNAME + " " + _ed.LASTNAME,
                       DATEADDED = data.DATEADDED,
                       IndentAmount = data.INDENT_AMOUNT,
                       IndentNo = data.INDENT_NO.Trim(),
                       PRHEADERID = data.PRHEADERID,
                       ItemDetail = data.ITEM_DETAIL,
                       ARIBARFPID = data.ARIBARFPID,
                       ARIBASTATUS = data.ARIBASTATUS,
                       ARIBABuyer_ECODE = (long)(data.ARIBABUYER == null ? 0 : data.ARIBABUYER),
                       //PRStatus = (_POHH != null ? (short)3 : PRPS != null ? PRPS.STATUS : (short)0),                       
                       PRStatus = (short)(PRPS != null ? (PRPS.STATUS == 2 ? (short)2 : (_POHH.POHEADERID != null ? (short)3 : PRPS.STATUS)) : (_POHH.POHEADERID != null ? (short)3 : (short)0)),                     
                       UPDATEDATE = PR_AH.APP_DATE,
                       PRATTACHMENT = PRD.FILENAME,
                       //Added by aumento for the SR73841============
                       ARIBASRNO = data.ARIBASRNO,
                       SLA_Category = (short)(data.PRCATEGORY == null ? 0 : data.PRCATEGORY),
                       //============================================
                       //BUYER_MODEL = (from _PRBuyerStatus in _PrDBContext.DGIT_PRBUYER_MAP.Where(h => h.PRHEADERID == data.PRHEADERID && h.STATUS == 1)
                       //               join _prBuy in _PrDBContext.ADEMPLOYEE on _PRBuyerStatus.BUYER_ECODE equals _prBuy.ADEMPCODE
                       //               select new PRBuyerMapViewModel
                       //               {
                       //                   BUYER_ECODE = _PRBuyerStatus.BUYER_ECODE,
                       //                   BUYER_NAME = _prBuy.FIRSTNAME + " " + _prBuy.LASTNAME,
                       //                   ADDEDBY = _PRBuyerStatus.ADDEDBY,
                       //                   DATEADDED = _PRBuyerStatus.DATEADDED
                       //               }).ToList(),
                       Emp_Detail = new Employee_Details
                       {
                           _ECode = _ed.ADEMPCODE,
                           _EName = _ed.FIRSTNAME + " " + _ed.LASTNAME,
                           _OpDesc = emp.OPERATION,
                           _DivDesc = emp.DIVISION,
                           _DepDesc = emp.DEPARTMENT,
                           _SecDescrip = emp.SECTION,
                           _Desig = pl.PLANTNAME,
                           _PlantId = pl.SYPLANTID, //Added by TTL on 24-May-2025 against SR99130 > CR6361
                           _SiteId = emp.SYSITEID  //Added by TTL on 24-May-2025 against SR99130 > CR6361
                       }
                   }).ToList().Distinct().ToList();


                    foreach (var item in obj)
                    {
                        item.BUYER_MODEL = (from _PRBuyerStatus in _PrDBContext.DGIT_PRBUYER_MAP.Where(h => h.PRHEADERID == item.PRHEADERID && h.STATUS == 1)
                                       join _prBuy in _PrDBContext.ADEMPLOYEE on _PRBuyerStatus.BUYER_ECODE equals _prBuy.ADEMPCODE
                                       select new PRBuyerMapViewModel
                                       {
                                           BUYER_ECODE = _PRBuyerStatus.BUYER_ECODE,
                                           BUYER_NAME = _prBuy.FIRSTNAME + " " + _prBuy.LASTNAME,
                                           ADDEDBY = _PRBuyerStatus.ADDEDBY,
                                           DATEADDED = _PRBuyerStatus.DATEADDED
                                       }).ToList();

                     }

            if (VM.PRStatus != -1)
            {
                obj = obj.Where(w => w.PRStatus == VM.PRStatus).ToList();
            }
            if (!string.IsNullOrEmpty(VM.Startdate))
            {
                obj = obj.Where(m => m.UPDATEDATE >= ReqDateFrom).ToList();
            }
            if (!string.IsNullOrEmpty(VM.ENDDATE))
            {
                obj = obj.Where(m => m.UPDATEDATE <= ReqDateTo).ToList();
            }

            VM.SearchResult = obj.OrderBy(o => o.UPDATEDATE).ToList();
            return VM;
        }

        public List<PR_DGIT_PRADDAPP_MST> GetPRAddApproverMaster(long PRType, long ORGlvlid)
        {
            List<PR_DGIT_PRADDAPP_MST> obj = new List<PR_DGIT_PRADDAPP_MST>();
            obj = (from data in _PrDBContext.DGIT_PRADDAPP_MST
                   where data.STATUS == 1 && data.ADORGLVLID == ORGlvlid
                   select new PR_DGIT_PRADDAPP_MST
                   {
                       PRADDAPP_MST_ID = data.PRADDAPP_MST_ID,
                       ADORGLVLID = data.ADORGLVLID,
                       APPROVER = data.APPROVER,
                       FUNCTIONDESID = data.FUNCTIONDESID,
                       ACTUALDESGID = data.ACTUALDESGID,
                       ISPRINTREQUIRED = data.ISPRINTREQUIRED,
                       ISPARALELLAPP = data.ISPARALELLAPP,
                       AMOUNTRANGE_FROM = data.AMOUNTRANGE_FROM,
                       AMOUNTRANGE_TO = data.AMOUNTRANGE_TO,
                       INDENTTYPE = data.INDENTTYPE,
                       ISADDEDINLAST = data.ISADDEDINLAST,
                       APPSEQ = data.APPSEQ,
                       ACTIONFOR = data.ACTIONFOR,
                       IS_SISPR = data.IS_SISPR, //SIS PR Change
                       APP_TYPEINFO = data.APP_TYPEINFO //SIS PR Change
                   }).ToList();
            return obj;
        }

        public List<DGIT_PRCAT_MST> BindPRCategory()
        {
            return _PrDBContext.DGIT_PRCAT_MST.Where(w => w.STATUS == 1).OrderBy(o => o.CATID).ToList();
        }
        public List<PRBuyerMstViewModel> GetABuyerMstList(long? ADOrgLevelId)
        {
            var iList = (from data in _PrDBContext.DGIT_PRBUYERMST
                         join _Emp in _PrDBContext.ADEMPLOYEE on data.ADEMPCODE equals _Emp.ADEMPCODE
                         where data.ADORGLEVELID == ADOrgLevelId
                         && data.ACTIVE == 1
                         && data.SYKIID == _Syki.SYKIID
                         select new PRBuyerMstViewModel
                         {
                             PRBUYERMSTID = data.PRBUYERMSTID,
                             PRCAT = data.PRCAT,
                             ADEMPCODE = data.ADEMPCODE,
                             ADEMPNAME = _Emp.FIRSTNAME + " " + _Emp.LASTNAME + " - " + _Emp.ADEMPCODE,
                             DATEADDED = data.DATEADDED,
                             ADDEDBY = data.ADDEDBY
                         }).ToList();
            return iList.OrderBy(o => o.ADEMPNAME).ToList();
        }
        public List<PR_DGIT_PRADDAPP_MST> Get_PRADDAPP_MST_List(PR_DGIT_PRADDAPP_MST PR)
        {
            long strKIID = (long)_PrDBContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
            List<PR_DGIT_PRADDAPP_MST> data = (from a in _PrDBContext.DGIT_PRADDAPP_MST
                                               join b in _PrDBContext.ADORGLEVEL on a.ADORGLVLID equals b.ADORGLEVELID
                                               where
                                               (PR.ADORGLVLID != 0 ? a.ADORGLVLID == PR.ADORGLVLID : true)
                                               &&
                                               (PR.FUNCTIONDESID != null ? b.ADORGLEVELTYPEID == PR.FUNCTIONDESID : true)
                                               && b.SYKIID == strKIID
                                               && a.STATUS == 1
                                               orderby a.PRADDAPP_MST_ID
                                               select new PR_DGIT_PRADDAPP_MST
                                               {
                                                   PRADDAPP_MST_ID = a.PRADDAPP_MST_ID,
                                                   ADORGLVLID = a.ADORGLVLID,
                                                   APPROVER = a.APPROVER,
                                                   FUNCTIONDESID = a.FUNCTIONDESID,
                                                   ACTUALDESGID = a.ACTUALDESGID,
                                                   AMOUNTRANGE_FROM = a.AMOUNTRANGE_FROM,
                                                   AMOUNTRANGE_TO = a.AMOUNTRANGE_TO,
                                                   INDENTTYPE = a.INDENTTYPE,
                                                   ISADDEDINLAST = a.ISADDEDINLAST,
                                                   APPSEQ = a.APPSEQ,
                                                   ACTIONFOR = a.ACTIONFOR,
                                                   ISPRINTREQUIRED = a.ISPRINTREQUIRED,
                                                   STATUS = a.STATUS,
                                                   IS_SISPR = a.IS_SISPR,
                                                   APP_TYPEINFO = a.APP_TYPEINFO,
                                                   ISPARALELLAPP=a.ISPARALELLAPP,
                                                   PLANTID = a.PLANTID // ADDED BY Aumento :: SR68003
                                               }).ToList();

            return data;
        }

        public List<Employee_Details> PortalAutocompleteSuggestionsFunDesig(string Key)
        {
            long strKIID = (long)_PrDBContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
            var empdata = (from a in _PrDBContext.ADFUNCTIONALDESIGNATION.Where(m => m.ACTIVE == 1)
                               //into ls
                               //from fg in ls.DefaultIfEmpty()
                               //where (isSearchDesg == 1 ? (fg.DESCRIP.ToUpper().Contains(designation.ToUpper()) || d.DESCRIP.ToUpper().Contains(designation.ToUpper())) : true)
                           select new
                           {
                               ADFUNCTIONALDESIGNATIONID = a.ADFUNCTIONALDESIGNATIONID,
                               DESCRIP = a.DESCRIP
                           }
                           ).ToList();
            List<Employee_Details> portalUserDtos = new List<Employee_Details>();
            portalUserDtos = (from userdata in empdata
                              where (userdata.ADFUNCTIONALDESIGNATIONID.ToString().StartsWith(Key) || userdata.DESCRIP.ToUpper().Contains(Key.ToUpper()))
                              select new Employee_Details
                              {
                                  _DesigId = userdata.ADFUNCTIONALDESIGNATIONID,
                                  _Desig = userdata.DESCRIP

                              }).ToList();

            return portalUserDtos;
        }

        public List<Employee_Details> PortalAutocompleteSuggestionsActDesig(string Key)
        {
            long strKIID = (long)_PrDBContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
            var empdata = (from a in _PrDBContext.ADDESIGNATION.Where(m => m.ACTIVE == 1)
                               //into ls
                               //from fg in ls.DefaultIfEmpty()
                               //where (isSearchDesg == 1 ? (fg.DESCRIP.ToUpper().Contains(designation.ToUpper()) || d.DESCRIP.ToUpper().Contains(designation.ToUpper())) : true)
                           select new
                           {
                               ADDESIGNATIONID = a.ADDESIGNATIONID,
                               DESCRIP = a.DESCRIP
                           }
                           ).ToList();
            List<Employee_Details> portalUserDtos = new List<Employee_Details>();
            portalUserDtos = (from userdata in empdata
                              where (userdata.ADDESIGNATIONID.ToString().StartsWith(Key) || userdata.DESCRIP.ToUpper().Contains(Key.ToUpper()))
                              select new Employee_Details
                              {
                                  _DesigId = userdata.ADDESIGNATIONID,
                                  _Desig = userdata.DESCRIP

                              }).ToList();

            return portalUserDtos;
        }

        public long AddEditDgitPraddapp_mst(List<PR_DGIT_PRADDAPP_MST> data)
        {
            long id = 0;
            try
            {
                foreach (var item in data)
                {
                    if (item.PRADDAPP_MST_ID == 0)
                    {
                        long PRADDAPP_MST_ID = _PrDBContext.DGIT_PRADDAPP_MST.OrderByDescending(u => u.PRADDAPP_MST_ID).Select(a => a.PRADDAPP_MST_ID).FirstOrDefault();
                        DGIT_PRADDAPP_MST pr = new DGIT_PRADDAPP_MST();
                        pr.PRADDAPP_MST_ID = PRADDAPP_MST_ID + 1;
                        pr.ADORGLVLID = item.ADORGLVLID;
                        pr.APPROVER = item.APPROVER;
                        pr.FUNCTIONDESID = item.FUNCTIONDESID;
                        pr.ACTUALDESGID = item.ACTUALDESGID;
                        pr.ISPRINTREQUIRED = item.ISPRINTREQUIRED;
                        pr.AMOUNTRANGE_FROM = item.AMOUNTRANGE_FROM;
                        pr.AMOUNTRANGE_TO = item.AMOUNTRANGE_TO;
                        //pr.INDENTTYPE = item.INDENTTYPE;
                        //pr.ISADDEDINLAST = item.ISADDEDINLAST;
                        if (item.INDENTTYPE == 1)
                        {
                            pr.INDENTTYPE = item.INDENTTYPE;
                        }
                        else
                        {
                            pr.INDENTTYPE = null;
                        }
                        if (item.ISADDEDINLAST > 0)
                        {
                            pr.ISADDEDINLAST = item.ISADDEDINLAST;
                        }
                        else
                        {
                            pr.ISADDEDINLAST = null;
                        }
                        pr.APPSEQ = item.APPSEQ;
                        pr.ACTIONFOR = item.ACTIONFOR;
                        pr.ADDEDDATE = DateTime.Now;
                        pr.ADDEDBY = item.ADDEDBY;
                        pr.STATUS = item.STATUS;
                        if (item.IS_SISPR != 0 || item.IS_SISPR!=null)
                        {
                            pr.IS_SISPR = item.IS_SISPR;
                        }
                        else
                        {
                            pr.IS_SISPR = null;
                        }
                        pr.APP_TYPEINFO = item.APP_TYPEINFO;
                        //pr.IS_SISPR = item.IS_SISPR;
                        pr.ISPARALELLAPP = item.ISPARALELLAPP;
                        pr.PLANTID = item.PLANTID; //Added by Aumento::SR68003 
                        _PrDBContext.DGIT_PRADDAPP_MST.Add(pr);
                        //_PrDBContext.
                        _PrDBContext.SaveChanges();
                    }
                    else
                    {
                        DGIT_PRADDAPP_MST pr = _PrDBContext.DGIT_PRADDAPP_MST.Find(item.PRADDAPP_MST_ID);
                        if (item.STATUS == 0)
                        {
                            pr.STATUS = item.STATUS;
                            pr.MODIFIEDDATE = DateTime.Now;
                            pr.MODIFIEDBY = item.ADDEDBY;
                            _PrDBContext.SaveChanges();
                        }
                        else
                        {
                            if (pr.ADORGLVLID != item.ADORGLVLID ||
                            pr.APPROVER != item.APPROVER ||
                            pr.FUNCTIONDESID != item.FUNCTIONDESID ||
                            pr.ACTUALDESGID != item.ACTUALDESGID ||
                            pr.ISPRINTREQUIRED != item.ISPRINTREQUIRED ||
                            pr.AMOUNTRANGE_FROM != item.AMOUNTRANGE_FROM ||
                            pr.AMOUNTRANGE_TO != item.AMOUNTRANGE_TO ||
                            pr.INDENTTYPE != item.INDENTTYPE ||
                            pr.ISADDEDINLAST != item.ISADDEDINLAST ||
                            pr.APPSEQ != item.APPSEQ ||
                            pr.ACTIONFOR != item.ACTIONFOR ||
                            pr.STATUS != item.STATUS ||
                            pr.IS_SISPR != item.IS_SISPR ||
                            pr.APP_TYPEINFO != item.APP_TYPEINFO ||
                            pr.PLANTID != item.PLANTID)  // ADDED BY Aumento :: SR68003
                            {
                                pr.ADORGLVLID = item.ADORGLVLID;
                                pr.APPROVER = item.APPROVER;
                                pr.FUNCTIONDESID = item.FUNCTIONDESID;
                                pr.ACTUALDESGID = item.ACTUALDESGID;
                                pr.ISPRINTREQUIRED = item.ISPRINTREQUIRED;
                                pr.AMOUNTRANGE_FROM = item.AMOUNTRANGE_FROM;
                                pr.AMOUNTRANGE_TO = item.AMOUNTRANGE_TO;

                                if (item.INDENTTYPE == 1)
                                {
                                    pr.INDENTTYPE = item.INDENTTYPE;
                                }
                                else
                                {
                                    pr.INDENTTYPE = null;
                                }
                                if (item.ISADDEDINLAST > 0)
                                {
                                    pr.ISADDEDINLAST = item.ISADDEDINLAST;
                                }
                                else
                                {
                                    pr.ISADDEDINLAST = null;
                                }
                                pr.APPSEQ = item.APPSEQ;
                                pr.ACTIONFOR = item.ACTIONFOR;
                                pr.MODIFIEDDATE = DateTime.Now;
                                pr.MODIFIEDBY = item.ADDEDBY;
                                pr.STATUS = item.STATUS;
                                if (item.IS_SISPR != 0 || item.IS_SISPR != null)
                                {
                                    pr.IS_SISPR = item.IS_SISPR;
                                }
                                else
                                {
                                    pr.IS_SISPR = null;
                                }
                                pr.APP_TYPEINFO = item.APP_TYPEINFO;
                                pr.ISPARALELLAPP = item.ISPARALELLAPP;
                                pr.PLANTID = item.PLANTID; //Added by Aumento::SR68003
                                _PrDBContext.Entry(pr).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                _PrDBContext.SaveChanges();
                            }

                        }
                    }
                }

            }
            catch (Exception)
            {

                id = 1;
            }
            return id;
        }
        public List<AD_orglevel_type> getOrgLevelType()
        {
            List<AD_orglevel_type> data = (from a in _PrDBContext.ADORGLEVELTYPE
                                           where a.ACTIVE == 1
                                           select new AD_orglevel_type
                                           {
                                               ADORGLEVELTYPEID = a.ADORGLEVELTYPEID,
                                               LEVELTYPE = a.LEVELTYPE
                                           }).ToList();
            return data;
        }
        public List<AD_orglevel> GetOrgUnitData(int id)
        {
            long strKIID = (long)_PrDBContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
            List<AD_orglevel> data = (from a in _PrDBContext.ADORGLEVEL
                                      where a.ADORGLEVELTYPEID == id
                                      && a.ACTIVE == 1
                                      && a.SYKIID == strKIID
                                      select new AD_orglevel
                                      {
                                          ADORGLEVELID = a.ADORGLEVELID,
                                          LEVELDESCRIP = a.LEVELDESCRIP
                                      }).ToList();
            return data;
        }

        //================================================Start=================================================
        //                                 Allocator Data Management 29-08-2022 (Aumento)
        //======================================================================================================
        public List<DGIT_PRCAT_MST> getDgitPRCatMst()
        {
            return _PrDBContext.DGIT_PRCAT_MST.Where(a => a.STATUS == 1).OrderBy(a => a.CATID).ToList();
        }

        public int Get_EmployeeMap_Count(PR_dgit_properationmap PR)
        {
            return _PrDBContext.DGIT_PROPERATIONMAP.Where(a => a.PRCAT != PR.PRCAT && a.ADEMPCODE == PR.ADEMPCODE && a.STATUS == 1).Count();
        }
        public List<PR_dgit_properationmap> Get_Properationmap_List(PR_dgit_properationmap PR)
        {
            //int count = _PrDBContext.DGIT_PROPERATIONMAP.Where(a => a.PRCAT != PR.PRCAT && a.ADEMPCODE == PR.ADEMPCODE && a.STATUS ==1).Count();
            List<PR_dgit_properationmap> data = new List<PR_dgit_properationmap>();
            //if (count == 0)
            //{
            data = (from a in _PrDBContext.DGIT_PROPERATIONMAP
                    where a.ADEMPCODE == PR.ADEMPCODE
                    && a.PRCAT == PR.PRCAT
                    select new PR_dgit_properationmap
                    {
                        PROPMAPID = a.PROPMAPID,
                        PRCAT = a.PRCAT,
                        ADEMPCODE = a.ADEMPCODE,
                        ADORGLVLID = a.ADORGLVLID,
                        STATUS = a.STATUS
                    }).ToList();
            //}
            return data;
        }

        public List<AD_orglevel> GetOperationList()
        {
            long strKIID = (long)_PrDBContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
            List<AD_orglevel> OrgLevel = (from a in _PrDBContext.ADORGLEVEL
                                          where a.SYKIID == strKIID
                                          && a.ADORGLEVELTYPEID == 1
                                          select new AD_orglevel
                                          {
                                              ADORGLEVELID = a.ADORGLEVELID,
                                              LEVELDESCRIP = a.LEVELDESCRIP
                                          }).ToList();
            return OrgLevel;
        }

        public long SaveAllocatorMaster(List<PR_dgit_properationmap> data)
        {
            long id = 0;
            try
            {
                foreach (var item in data)
                {
                    if (item.PROPMAPID == 0 && item.STATUS == 1)
                    {
                        long PROPMAPID = _PrDBContext.DGIT_PROPERATIONMAP.OrderByDescending(u => u.PROPMAPID).Select(a => a.PROPMAPID).FirstOrDefault();
                        DGIT_PROPERATIONMAP pr = new DGIT_PROPERATIONMAP();
                        pr.PROPMAPID = PROPMAPID + 1;
                        pr.PRCAT = item.PRCAT;
                        pr.ADEMPCODE = item.ADEMPCODE;
                        pr.ADORGLVLID = item.ADORGLVLID;
                        pr.DATEADDED = DateTime.Now;
                        pr.ADDEDBY = item.ADDEDBY;
                        pr.STATUS = item.STATUS;
                        _PrDBContext.DGIT_PROPERATIONMAP.Add(pr);
                        _PrDBContext.SaveChanges();
                    }
                    else
                    {

                        if (item.STATUS == 0 && item.PROPMAPID > 0)
                        {
                            DGIT_PROPERATIONMAP pr = _PrDBContext.DGIT_PROPERATIONMAP.Find(item.PROPMAPID);
                            pr.STATUS = item.STATUS;
                            _PrDBContext.SaveChanges();
                        }
                        if (item.PROPMAPID > 0)
                        {
                            DGIT_PROPERATIONMAP st = _PrDBContext.DGIT_PROPERATIONMAP.Find(item.PROPMAPID);
                            if (st.STATUS != item.STATUS && item.PROPMAPID > 0)
                            {
                                st.STATUS = item.STATUS;
                                _PrDBContext.SaveChanges();
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                id = 1;
            }
            return id;
        }
        public List<PR_dgit_properationmap> getPR_properationmapReport(PR_dgit_properationmap pr)
        {
            long strKIID = (long)_PrDBContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
            List<PR_dgit_properationmap> data = (from a in _PrDBContext.DGIT_PROPERATIONMAP
                                                 join b in _PrDBContext.DGIT_PRCAT_MST on a.PRCAT equals b.CATID
                                                 join c in _PrDBContext.ADEMPLOYEE on a.ADEMPCODE equals c.ADEMPCODE
                                                 join d in _PrDBContext.ADORGLEVEL.Where(a => a.ADORGLEVELTYPEID == 1 && a.SYKIID == strKIID) on a.ADORGLVLID equals d.ADORGLEVELID
                                                 where /*a.ADEMPCODE == pr.ADEMPCODE*/
                                                 /*&&*/ a.PRCAT == pr.PRCAT
                                                 && a.STATUS == 1
                                                 orderby a.PROPMAPID ascending
                                                 select new PR_dgit_properationmap
                                                 {
                                                     PROPMAPID = a.PROPMAPID,
                                                     PRCAT = a.PRCAT,
                                                     CAtDESC = b.CATDESC,
                                                     ADEMPCODE = a.ADEMPCODE,
                                                     EMPNAME = c.FIRSTNAME + " " + c.LASTNAME,
                                                     ADORGLVLID = a.ADORGLVLID,
                                                     ORGDESC = d.LEVELDESCRIP
                                                 }).ToList();
            return data;
        }
        //=======================================End(Allocator)===============================

        //================================================Start=================================================
        //                                 Buyer Data Management 29-08-2022 (Aumento)
        //======================================================================================================
        public List<AD_orglevel> PortalAutocompleteOrgLevel(string Key)
        {
            long strKIID = (long)_PrDBContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
            var data = (from a in _PrDBContext.ADORGLEVEL
                        where a.SYKIID == strKIID
                        && a.ADORGLEVELTYPEID == 1
                        select new AD_orglevel
                        {
                            ADORGLEVELID = a.ADORGLEVELID,
                            LEVELDESCRIP = a.LEVELDESCRIP
                        }).ToList();
            List<AD_orglevel> portalUserDtos = new List<AD_orglevel>();
            portalUserDtos = (from userdata in data
                              where (userdata.ADORGLEVELID.ToString().StartsWith(Key) || userdata.LEVELDESCRIP.ToUpper().Contains(Key.ToUpper()))
                              select new AD_orglevel
                              {
                                  ADORGLEVELID = userdata.ADORGLEVELID,
                                  LEVELDESCRIP = userdata.LEVELDESCRIP

                              }).ToList();

            return portalUserDtos;
        }

        public List<PRBuyerMstViewModel> getPRBuyerMstList(PRBuyerMstViewModel obj)
        {
            long strKIID = (long)_PrDBContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
            List<PRBuyerMstViewModel> data = (from a in _PrDBContext.DGIT_PRBUYERMST
                                              join b in _PrDBContext.ADORGLEVEL on a.ADORGLEVELID equals b.ADORGLEVELID
                                              where a.ADORGLEVELID == obj.ADORGLEVELID
                                              && b.ADORGLEVELTYPEID == 1
                                              && a.PRCAT == obj.PRCAT
                                              && a.ACTIVE == 1
                                              && a.SYKIID == strKIID
                                              select new PRBuyerMstViewModel
                                              {
                                                  PRBUYERMSTID = a.PRBUYERMSTID,
                                                  ADEMPCODE = a.ADEMPCODE,
                                                  ADEMPNAME = _PrDBContext.ADEMPLOYEE.Where(m => m.ADEMPCODE == a.ADEMPCODE).Select(n => n.FIRSTNAME + " " + n.LASTNAME).FirstOrDefault()
                                              }).ToList();

            return data;
        }

        public long SaveBuyerMaster(List<PRBuyerMstViewModel> data)
        {
            long id = 0;
            try
            {
                foreach (var item in data)
                {
                    if (item.PRBUYERMSTID == 0 && item.ACTIVE == 1)
                    {
                        long strKIID = (long)_PrDBContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
                        long PROPMAPID = _PrDBContext.DGIT_PRBUYERMST.OrderByDescending(u => u.PRBUYERMSTID).Select(a => a.PRBUYERMSTID).FirstOrDefault();
                        DGIT_PRBUYERMST pr = new DGIT_PRBUYERMST();
                        int count = _PrDBContext.DGIT_PRBUYERMST.Where(a => a.ADEMPCODE == item.ADEMPCODE && a.PRCAT == item.PRCAT && a.ADORGLEVELID == item.ADORGLEVELID && a.SYKIID == strKIID).Count();
                        if (count > 0)
                        {
                            pr = _PrDBContext.DGIT_PRBUYERMST.Where(a => a.ADEMPCODE == item.ADEMPCODE && a.PRCAT == item.PRCAT && a.ADORGLEVELID == item.ADORGLEVELID && a.SYKIID == strKIID).FirstOrDefault();
                            pr.ACTIVE = item.ACTIVE;
                            _PrDBContext.SaveChanges();
                        }
                        else
                        {
                            pr.PRBUYERMSTID = PROPMAPID + 1;
                            pr.PRCAT = item.PRCAT;
                            pr.ADEMPCODE = item.ADEMPCODE;
                            pr.ADORGLEVELID = item.ADORGLEVELID;
                            pr.DATEADDED = DateTime.Now;
                            pr.ADDEDBY = item.ADDEDBY;
                            pr.ACTIVE = item.ACTIVE;
                            pr.SYKIID = strKIID;
                            _PrDBContext.DGIT_PRBUYERMST.Add(pr);
                            _PrDBContext.SaveChanges();
                        }
                    }
                    else
                    {
                        if (item.ACTIVE == 0 && item.PRBUYERMSTID > 0)
                        {
                            DGIT_PRBUYERMST pr = _PrDBContext.DGIT_PRBUYERMST.Find(item.PRBUYERMSTID);
                            pr.ACTIVE = item.ACTIVE;
                            _PrDBContext.SaveChanges();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                id = 1;
            }
            return id;
        }

        public List<PRBuyerMstViewModel> getPR_BuyermapReport(PRBuyerMstViewModel pr)
        {
            long strKIID = (long)_PrDBContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
            List<PRBuyerMstViewModel> data = (from a in _PrDBContext.DGIT_PRBUYERMST
                                              join b in _PrDBContext.DGIT_PRCAT_MST on a.PRCAT equals b.CATID
                                              join c in _PrDBContext.ADEMPLOYEE on a.ADEMPCODE equals c.ADEMPCODE
                                              join d in _PrDBContext.ADORGLEVEL.Where(a => a.ADORGLEVELTYPEID == 1 && a.SYKIID == strKIID) on a.ADORGLEVELID equals d.ADORGLEVELID
                                              //where a.ADORGLEVELID == pr.ADORGLEVELID
                                              //&& 
                                              where a.PRCAT == pr.PRCAT
                                              && a.ACTIVE == 1
                                              orderby a.PRBUYERMSTID ascending
                                              select new PRBuyerMstViewModel
                                              {
                                                  PRBUYERMSTID = a.PRBUYERMSTID,
                                                  PRCAT = b.CATID,
                                                  CATNAME = b.CATDESC,
                                                  ADEMPCODE = a.ADEMPCODE,
                                                  ADEMPNAME = c.FIRSTNAME + " " + c.LASTNAME,
                                                  ADORGLEVELID = a.ADORGLEVELID,
                                                  LEVELDESCRIP = d.LEVELDESCRIP
                                              }).ToList();
            return data;
        }

        public List<Employee_Details> PortalAutocompleteSuggestionsEmployee(string Key, int Catid, int Orgid)
        {
            long strKIID = (long)_PrDBContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
            PRBuyerMstViewModel obj = new PRBuyerMstViewModel();
            obj.PRCAT = Catid;
            obj.ADORGLEVELID = Orgid;
            List<PRBuyerMstViewModel> BuyerMst = new List<PRBuyerMstViewModel>();
            if (Catid != 0 && Orgid != 0)
            {
                BuyerMst = getPRBuyerMstList(obj);
            }
            var EmpCode = BuyerMst.Select(a => a.ADEMPCODE);
            var empdata = (from emp in _PrDBContext.ADEMPLOYEE.Where(m => m.ACTIVE == 1)
                           where !EmpCode.Contains(emp.ADEMPCODE)
                           //join m in BuyerMst on emp.ADEMPCODE equals m.ADEMPCODE
                           //where (isSearchDesg == 1 ? (fg.DESCRIP.ToUpper().Contains(designation.ToUpper()) || d.DESCRIP.ToUpper().Contains(designation.ToUpper())) : true)
                           select new
                           {
                               ADEMPCODE = emp.ADEMPCODE,
                               FIRSTNAME = emp.FIRSTNAME,
                               LASTNAME = emp.LASTNAME,
                               _ENAME = emp.FIRSTNAME + " " + emp.LASTNAME
                           }
                          ).ToList();
            List<Employee_Details> portalUserDtos = new List<Employee_Details>();
            portalUserDtos = (from userdata in empdata
                              where (userdata.ADEMPCODE.ToString().StartsWith(Key) || userdata.FIRSTNAME.ToUpper().Contains(Key.ToUpper()) || userdata.LASTNAME.ToUpper().Contains(Key.ToUpper()) || (userdata._ENAME.ToUpper()).Contains(Key.ToUpper()))
                              orderby userdata.ADEMPCODE
                              select new Employee_Details
                              {
                                  _ECode = userdata.ADEMPCODE,
                                  _EFirstName = userdata.FIRSTNAME,
                                  _ELastName = userdata.LASTNAME
                              }).ToList();

            return portalUserDtos;
        }
        //=======================================End(Buyer)===============================

        //-- SR52365==============================
        public Tuple<long, long, long, long, List<PR_Div_Dep_SecViewModel>> BindDivision(long op_Id, long _syKi, long adempcode)
        {
            List<PR_Div_Dep_SecViewModel> iList = new List<PR_Div_Dep_SecViewModel>();
            long opID = 0, divID = 0, deptID = 0, secID = 0;
            Tuple<long, long, long, long, List<PR_Div_Dep_SecViewModel>> _tuple;
            var vwAssociateDetails = _PrDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == adempcode && x.SYKI == _syKi && x.ACTIVE == 1).FirstOrDefault();
            opID = vwAssociateDetails.OPERATIONID == null ? 0 : (long)vwAssociateDetails.OPERATIONID;
            divID = vwAssociateDetails.DIVISIONID == null ? 0 : (long)vwAssociateDetails.DIVISIONID;
            deptID = vwAssociateDetails.DEPARTMENTID == null ? 0 : (long)vwAssociateDetails.DEPARTMENTID;
            secID = vwAssociateDetails.SECTIONID == null ? 0 : (long)vwAssociateDetails.SECTIONID;
            op_Id = opID;

            iList = (from data in _PrDBContext.ADORGLEVEL.Where(x => x.SYKIID == _syKi && x.ADORGLEVELTYPEID == 2 && x.PARENTLEVELID == op_Id && (x.ACTIVE == 1 || x.ACTIVE == 2)) //(ADORGLEVELTYPE=2 for Division head)
                     select new PR_Div_Dep_SecViewModel
                     {
                         Value = data.ADORGLEVELID,
                         Text = data.LEVELDESCRIP
                     }).OrderBy(x => x.Text).ToList();

            return _tuple = new Tuple<long, long, long, long, List<PR_Div_Dep_SecViewModel>>(opID, divID, deptID, secID, iList);
        }

        public Tuple<long, long, long, long, List<PR_Div_Dep_SecViewModel>> BindDepartment(long divId, long _syKi, long adempcode)
        {
            long opID = 0, divID = 0, deptID = 0, secID = 0;
            var vwAssociateDetails = _PrDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == adempcode && x.SYKI == _syKi && (x.ACTIVE == 1 || x.ACTIVE == 2)).FirstOrDefault();
            opID = vwAssociateDetails.OPERATIONID == null ? 0 : (long)vwAssociateDetails.OPERATIONID;
            divID = vwAssociateDetails.DIVISIONID == null ? 0 : (long)vwAssociateDetails.DIVISIONID;
            deptID = vwAssociateDetails.DEPARTMENTID == null ? 0 : (long)vwAssociateDetails.DEPARTMENTID;
            secID = vwAssociateDetails.SECTIONID == null ? 0 : (long)vwAssociateDetails.SECTIONID;

            List<PR_Div_Dep_SecViewModel> iList = new List<PR_Div_Dep_SecViewModel>();

            iList = (from data in _PrDBContext.ADORGLEVEL.Where(x => x.SYKIID == _syKi && x.ADORGLEVELTYPEID == 3 && x.PARENTLEVELID == divId) //(ADORGLEVELTYPE=3 for department head)
                     select new PR_Div_Dep_SecViewModel
                     {
                         Value = data.ADORGLEVELID,
                         Text = data.LEVELDESCRIP
                     }).OrderBy(x => x.Text).ToList();

            Tuple<long, long, long, long, List<PR_Div_Dep_SecViewModel>> _tuple;
            return _tuple = new Tuple<long, long, long, long, List<PR_Div_Dep_SecViewModel>>(opID, divID, deptID, secID, iList);
        }

        public Tuple<long, long, long, long, List<PR_Div_Dep_SecViewModel>> BindSection(long divId, long depId, long _syKi, long adempcode)
        {
            long opID = 0, divID = 0, deptID = 0, secID = 0;
            var vwAssociateDetails = _PrDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == adempcode && x.SYKI == _syKi && (x.ACTIVE == 1 || x.ACTIVE == 2)).FirstOrDefault();
            opID = vwAssociateDetails.OPERATIONID == null ? 0 : (long)vwAssociateDetails.OPERATIONID;
            divID = vwAssociateDetails.DIVISIONID == null ? 0 : (long)vwAssociateDetails.DIVISIONID;
            deptID = vwAssociateDetails.DEPARTMENTID == null ? 0 : (long)vwAssociateDetails.DEPARTMENTID;
            secID = vwAssociateDetails.SECTIONID == null ? 0 : (long)vwAssociateDetails.SECTIONID;

            List<PR_Div_Dep_SecViewModel> iList = new List<PR_Div_Dep_SecViewModel>();

            iList = (from data in _PrDBContext.ADORGLEVEL.Where(x => x.SYKIID == _syKi && x.ADORGLEVELTYPEID == 4 && x.PARENTLEVELID == depId) //(ADORGLEVELTYPE=4 for section head)
                     select new PR_Div_Dep_SecViewModel
                     {
                         Value = data.ADORGLEVELID,
                         Text = data.LEVELDESCRIP
                     }).OrderBy(x => x.Text).ToList();

            Tuple<long, long, long, long, List<PR_Div_Dep_SecViewModel>> _tuple;
            return _tuple = new Tuple<long, long, long, long, List<PR_Div_Dep_SecViewModel>>(opID, divID, deptID, secID, iList);
        }

        public List<SYKI> GetKICodeList(long userId)
        {
            List<decimal> syki = _PrDBContext.VW_ASSOCIATELVLDETAILS.Where(a => a.ADEMPCODE == userId).Select(x => (decimal)x.SYKI).ToList();
            var iList = (from tmp in syki
                         join b in _PrDBContext.SYKI on tmp equals b.SYKIID
                         select new SYKI
                         {
                             SYKIID = b.SYKIID,
                             KICODE = b.KICODE,
                         }).ToList();
            return iList.OrderByDescending(o => o.SYKIID).ToList();
        }
        //==============================

        public SearchIndentUser PRPIUserDashboard(SearchIndentUser VM)
        {
            DateTime ReqDateFrom = DateTime.Now.Date;
            DateTime ReqDateTo = DateTime.Now.Date;
            DateTime reportfrom = DateTime.ParseExact("15-JUL-2021", "dd-MMM-yyyy", null);
            if (!string.IsNullOrEmpty(VM.Startdate))
            {
                //ReqDateFrom = DateTime.ParseExact(VM.Startdate, "dd-MMM-yyyy", null);
                ReqDateFrom = DateTime.Parse(VM.Startdate, null , System.Globalization.DateTimeStyles.RoundtripKind);
            }
            if (!string.IsNullOrEmpty(VM.ENDDATE))
            {
                //ReqDateTo = DateTime.ParseExact(VM.ENDDATE + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
                DateTime endDate = DateTime.Parse(VM.ENDDATE, null, System.Globalization.DateTimeStyles.RoundtripKind);
                ReqDateTo = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);
            }


            List<VM_VW_PRUSERDASHBOARD> obj = new List<VM_VW_PRUSERDASHBOARD>();
            obj = (from data in _PrDBContext.VW_PRUSERDASHBOARD.Where(m=>m.IS_IPRELATEDPR==1)
                   where (!string.IsNullOrEmpty(VM.Startdate) ? (data.DATEADDED >= ReqDateFrom) : true)
                   && (!string.IsNullOrEmpty(VM.ENDDATE) ? (data.DATEADDED <= ReqDateTo) : true)
                   && (VM.OperationID != 0 ? data.OPERATIONID == VM.OperationID : true)
                   && (VM.DivisionID != 0 ? data.DIVISIONID == VM.DivisionID : true)
                   && (VM.DEPTID != 0 ? data.DEPARTMENTID == VM.DEPTID : true)
                   && (VM.SECID != 0 ? data.SECTIONID == VM.SECID : true)
                   && (string.IsNullOrEmpty(VM.IndentNo) ? true : data.INDENT_NO == VM.IndentNo)
                   && (VM.ecode == 0 ? true : data.ADDEDBY == VM.ecode)
                   && (VM.PRStatus != -1 ? data.POSTATUS == VM.PRStatus : true)
                   && (string.IsNullOrEmpty(VM.ITEM_DETAIL) ? true : data.ITEM_DETAIL.ToLower().Trim().Contains(VM.ITEM_DETAIL.ToLower().Trim()))
                   select new VM_VW_PRUSERDASHBOARD
                   {
                       ADDEDBYNAME = data.ADDEDBYNAME,
                       DATEADDED = data.DATEADDED,
                       INDENT_AMOUNT = data.INDENT_AMOUNT,
                       INDENT_NO = data.INDENT_NO,
                       PRHEADERID = data.PRHEADERID,
                       ITEM_DETAIL = data.ITEM_DETAIL,
                       POSTATUS = data.POSTATUS,
                       BUYERNAME = data.BUYERNAME,
                       BUYERSTATUS = data.BUYERSTATUS,
                       ADDEDBY = data.ADDEDBY,
                       DEPARTMENTID = data.DEPARTMENTID,
                       DIVISIONID = data.DIVISIONID,
                       OPERATIONID = data.OPERATIONID,
                       POAPPDATE = data.POAPPDATE,
                       POAPPROVALSTATUS = data.POAPPROVALSTATUS,
                       PONO = data.PONO,
                       PRAPPDATE = data.PRAPPDATE,
                       SECTIONID = data.SECTIONID,
                       UPDATEDATE = data.UPDATEDATE,
                       DEPARTMENT = data.DEPARTMENT,
                       DIVISION = data.DIVISION,
                       OPERATION = data.OPERATION,
                       SECTION = data.SECTION
                   }).ToList();
            VM.PRUSERDASHBOARD = obj.ToList();
            return VM;
        }

        //Added by aumento as on 19092024 for the SR71870============================================================



        public List<SYKI> GetKICodeList_ForPRDBOperationwise(long userId)

        {

            List<decimal> syki = _PrDBContext.VW_ASSOCIATELVLDETAILS.Where(a => a.ADEMPCODE == userId).Select(x => (decimal)x.SYKI).ToList();

            var iList = (from tmp in syki

                         join b in _PrDBContext.SYKI on tmp equals b.SYKIID

                         select new SYKI

                         {

                             SYKIID = b.SYKIID,

                             KICODE = b.KICODE,

                         }).ToList();

            return iList.OrderByDescending(o => o.SYKIID).ToList();

        }



        public Tuple<long, List<PR_Div_Dep_SecViewModel>> BindOperation(long Loginempcode, long KIID)

        {

            var isspecial = (from data in _ICDBContext.DGIT_ICRPTRIGHTS

                             where data.ADEMPCODE == Loginempcode

                             && data.REPORTTYPE == 2

                             && data.KIID == KIID

                             select data

                            );



            long isspecialright = 0;



            List<PR_Div_Dep_SecViewModel> iList = new List<PR_Div_Dep_SecViewModel>();

            if (isspecial.Count() > 0)

            {

                isspecialright = 1;



                var oplist1 = (from data in isspecial

                               join op in _ICDBContext.ICADORGLEVELs on data.OPERATIONID equals op.ADORGLEVELID

                               where (data.OPERATIONID != null || data.OPERATIONID != 0)

                                     && op.ADORGLEVELTYPEID == 1

                                     && op.SYKIID == KIID

                               select op

                                  );





                foreach (var obj in oplist1.Distinct())

                {

                    iList.Add(new PR_Div_Dep_SecViewModel

                    {

                        Value = Convert.ToInt64(obj.ADORGLEVELID == 0 ? 0 : obj.ADORGLEVELID),

                        Text = obj.LEVELDESCRIP

                    });

                }





            }

            else

            {

                var iColl = (from data in _PrDBContext.VW_ASSOCIATELVLDETAILS.Where(e => e.ACTIVE == 1

                             && e.SYKI == KIID && (e.OPERATIONID != null && e.OPERATIONID != 0)

                             && e.ADEMPCODE == Loginempcode)

                             select new

                             {

                                 data.OPERATIONID,

                                 data.OPERATION

                             }).Distinct().ToList();



                foreach (var obj in iColl)

                {

                    iList.Add(new PR_Div_Dep_SecViewModel

                    {

                        Value = Convert.ToInt64(obj.OPERATIONID == null ? 0 : obj.OPERATIONID),

                        Text = obj.OPERATION

                    });

                }

            }

            Tuple<long, List<PR_Div_Dep_SecViewModel>> _tuple = new Tuple<long, List<PR_Div_Dep_SecViewModel>>(isspecialright, iList);

            return _tuple;

        }



        public Tuple<long, List<PR_Div_Dep_SecViewModel>> BindDivision_PRDB(long Loginempcode, long KIID, long op_Id)

        {

            var isspecial = (from data in _ICDBContext.DGIT_ICRPTRIGHTS

                             where data.ADEMPCODE == Loginempcode

                             && data.REPORTTYPE == 2

                             && data.KIID == KIID

                             select data

                            );

            long isspecialright = 0;



            List<PR_Div_Dep_SecViewModel> iList = new List<PR_Div_Dep_SecViewModel>();

            if (isspecial.Count() > 0)

            {

                isspecialright = 1;

                isspecial = (from data in isspecial

                             where data.OPERATIONID == op_Id

                             select data

                            );



                var oplist1 = (from data in isspecial

                               join op in _ICDBContext.ICADORGLEVELs on data.DIVISIONID equals op.ADORGLEVELID

                               where (data.DIVISIONID != null || data.DIVISIONID != 0)

                                     && op.ADORGLEVELTYPEID == 2

                                     && op.SYKIID == KIID

                               select op

                                  );







                if (oplist1.Count() == 0)

                {

                    oplist1 = (from data in _ICDBContext.ICADORGLEVELs

                               join op in isspecial on data.PARENTLEVELID equals op.OPERATIONID

                               where (op.OPERATIONID != null || op.OPERATIONID != 0)

                                     && data.ADORGLEVELTYPEID == 2

                                     && data.SYKIID == KIID

                               select data

                                  );





                }



                foreach (var obj in oplist1.Distinct())

                {

                    iList.Add(new PR_Div_Dep_SecViewModel

                    {

                        Value = Convert.ToInt64(obj.ADORGLEVELID == 0 ? 0 : obj.ADORGLEVELID),

                        Text = obj.LEVELDESCRIP

                    });

                }





            }

            else

            {



                var iColl = (from data in _PrDBContext.VW_ASSOCIATELVLDETAILS.Where(e => e.ACTIVE == 1

                             && e.SYKI == KIID && (e.DIVISIONID != null && e.OPERATIONID == op_Id))

                                 //&& e.ADEMPCODE == Loginempcode)

                             select new

                             {

                                 data.DIVISIONID,

                                 data.DIVISION

                             }).Distinct().ToList();



                foreach (var obj in iColl)

                {

                    iList.Add(new PR_Div_Dep_SecViewModel

                    {

                        Value = Convert.ToInt64(obj.DIVISIONID == null ? 0 : obj.DIVISIONID),

                        Text = obj.DIVISION

                    });

                }

            }

            Tuple<long, List<PR_Div_Dep_SecViewModel>> _tuple = new Tuple<long, List<PR_Div_Dep_SecViewModel>>(isspecialright, iList);

            return _tuple;

        }



        public Tuple<long, List<PR_Div_Dep_SecViewModel>> BindSection(long ecode, long kIID, long operationID, long divisionID, long dEPTID)

        {

            var isspecial = (from data in _ICDBContext.DGIT_ICRPTRIGHTS

                             where data.ADEMPCODE == ecode

                             && data.REPORTTYPE == 2

                             && data.KIID == kIID

                             select data

                            );

            long isspecialright = 0;



            List<PR_Div_Dep_SecViewModel> iList = new List<PR_Div_Dep_SecViewModel>();

            if (isspecial.Count() > 0)

            {

                isspecial = (from data in isspecial

                             where

                             data.OPERATIONID == operationID

                             && data.DIVISIONID == divisionID

                             && data.DEPARTMENTID == dEPTID

                             select data

                            );

                isspecialright = 1;



                var oplist1 = (from data in isspecial

                               join op in _ICDBContext.ICADORGLEVELs on data.SECTIONID equals op.ADORGLEVELID

                               where (data.SECTIONID != null || data.SECTIONID != 0)

                                     && op.ADORGLEVELTYPEID == 4

                                     && op.SYKIID == kIID

                               select op

                                  );



                //var oplist1 = (from data in isspecial

                //               join op in _PrDBContext.ADORGLEVEL on data.SECTIONID equals op.ADORGLEVELID

                //               where (data.SECTIONID != null || data.SECTIONID != 0)

                //                     && op.ADORGLEVELTYPEID == 4

                //                     && op.SYKIID == kIID

                //               select op

                //                  );



                if (oplist1.Count() == 0)

                {

                    oplist1 = (from data in _ICDBContext.ICADORGLEVELs

                               join op in isspecial on data.PARENTLEVELID equals op.DEPARTMENTID

                               where (op.DEPARTMENTID != null || op.DEPARTMENTID != 0)

                                     && data.ADORGLEVELTYPEID == 4

                                     && data.SYKIID == kIID

                               select data

                                  );



                    //oplist1 = (from data in _PrDBContext.ADORGLEVEL

                    //           join op in isspecial on data.PARENTLEVELID equals op.DEPARTMENTID

                    //           where (op.DEPARTMENTID != null || op.DEPARTMENTID != 0)

                    //                 && data.ADORGLEVELTYPEID == 4

                    //                 && data.SYKIID == kIID

                    //           select data

                    //              );

                }



                foreach (var obj in oplist1.Distinct())

                {

                    iList.Add(new PR_Div_Dep_SecViewModel

                    {

                        Value = Convert.ToInt64(obj.ADORGLEVELID == 0 ? 0 : obj.ADORGLEVELID),

                        Text = obj.LEVELDESCRIP

                    });

                }





            }

            else

            {

                var iColl = (from data in _PrDBContext.VW_ASSOCIATELVLDETAILS.Where(e => e.ACTIVE == 1

                             && e.SYKI == kIID && (e.SECTION != null && e.DIVISIONID == divisionID && e.OPERATIONID == operationID && e.DEPARTMENTID == dEPTID)

                             //&& e.ADEMPCODE == Loginempcode

                             )

                             select new

                             {

                                 data.SECTIONID,

                                 data.SECTION

                             }).Distinct().ToList();



                foreach (var obj in iColl)

                {

                    iList.Add(new PR_Div_Dep_SecViewModel

                    {

                        Value = Convert.ToInt64(obj.SECTIONID == null ? 0 : obj.SECTIONID),

                        Text = obj.SECTION

                    });

                }

            }

            Tuple<long, List<PR_Div_Dep_SecViewModel>> _tuple = new Tuple<long, List<PR_Div_Dep_SecViewModel>>(isspecialright, iList);

            return _tuple;

        }



        public Tuple<long, List<PR_Div_Dep_SecViewModel>> BindDepartment(long ecode, long kIID, long operationID, long divisionID)

        {

            var isspecial = (from data in _ICDBContext.DGIT_ICRPTRIGHTS

                             where data.ADEMPCODE == ecode

                             && data.REPORTTYPE == 2

                             && data.KIID == kIID

                             select data

                           );

            long isspecialright = 0;



            List<PR_Div_Dep_SecViewModel> iList = new List<PR_Div_Dep_SecViewModel>();

            if (isspecial.Count() > 0)

            {

                isspecialright = 1;

                isspecial = (from data in isspecial

                             where data.OPERATIONID == operationID

                             && data.DIVISIONID == divisionID

                             select data

                            );



                var oplist1 = (from data in isspecial

                               join op in _ICDBContext.ICADORGLEVELs on data.DEPARTMENTID equals op.ADORGLEVELID

                               where (data.DEPARTMENTID != null || data.DEPARTMENTID != 0)

                                     && op.ADORGLEVELTYPEID == 3

                                     && op.SYKIID == kIID

                               select op

                                  );



                //var oplist1 = (from data in isspecial

                //               join op in _PrDBContext.ADORGLEVEL on data.DEPARTMENTID equals op.ADORGLEVELID

                //               where (data.DEPARTMENTID != null || data.DEPARTMENTID != 0)

                //                     && op.ADORGLEVELTYPEID == 3

                //                     && op.SYKIID == kIID

                //               select op

                //                  );



                if (oplist1.Count() == 0)

                {

                    oplist1 = (from data in _ICDBContext.ICADORGLEVELs

                               join op in isspecial on data.PARENTLEVELID equals op.DIVISIONID

                               where (op.DIVISIONID != null || op.DIVISIONID != 0)

                                     && data.ADORGLEVELTYPEID == 3

                                     && data.SYKIID == kIID

                               select data

                                  );



                    //oplist1 = (from data in _PrDBContext.ADORGLEVEL

                    //           join op in isspecial on data.PARENTLEVELID equals op.DIVISIONID

                    //           where (op.DIVISIONID != null || op.DIVISIONID != 0)

                    //                 && data.ADORGLEVELTYPEID == 3

                    //                 && data.SYKIID == kIID

                    //           select data

                    //                 );



                }





                foreach (var obj in oplist1.Distinct())

                {

                    iList.Add(new PR_Div_Dep_SecViewModel

                    {

                        Value = Convert.ToInt64(obj.ADORGLEVELID == 0 ? 0 : obj.ADORGLEVELID),

                        Text = obj.LEVELDESCRIP

                    });

                }





            }

            else

            {

                var iColl = (from data in _ICDBContext.VW_ASSOCIATELVLDETAILS1.Where(e => e.ACTIVE == 1

                             && e.SYKI == kIID && (e.DEPARTMENTID != null || e.DEPARTMENTID != 0)

                             && e.ADEMPCODE == ecode)

                             select new

                             {

                                 data.DEPARTMENTID,

                                 data.DEPARTMENT

                             }).Distinct().ToList();

                //var iColl = (from data in _PrDBContext.VW_ASSOCIATELVLDETAILS.Where(e => e.ACTIVE == 1

                //             && e.SYKI == kIID && (e.OPERATIONID == operationID && e.DIVISIONID == divisionID && e.DEPARTMENTID != null))

                //                 //&& e.ADEMPCODE == Loginempcode)

                //             select new

                //             {

                //                 data.DEPARTMENTID,

                //                 data.DEPARTMENT

                //             }).Distinct().ToList();



                foreach (var obj in iColl)

                {

                    iList.Add(new PR_Div_Dep_SecViewModel

                    {

                        Value = Convert.ToInt64(obj.DEPARTMENTID == null ? 0 : obj.DEPARTMENTID),

                        Text = obj.DEPARTMENT

                    });

                }

            }

            Tuple<long, List<PR_Div_Dep_SecViewModel>> _tuple = new Tuple<long, List<PR_Div_Dep_SecViewModel>>(isspecialright, iList);

            return _tuple;

        }



        public Employee_Details GetEmployeeDetail(long ecode, long kIID)

        {   //from data in _ICDBContext.VW_ASSOCIATELVLDETAILS1

            var KILIST = (from data in _ICDBContext.VW_ASSOCIATELVLDETAILS1

                          where data.SYKI == kIID && data.ADEMPCODE == ecode

                          select new Employee_Details

                          {

                              _DepDesc = data.DEPARTMENT,

                              _DepId = data.DEPARTMENTID,

                              _DesigId = data.ADDESIGNATIONID,

                              _DivDesc = data.DIVISION,

                              _DivId = data.DIVISIONID,

                              _ECode = data.ADEMPCODE,

                              _FnDesig = data.FUNCTIONALDESIGNATION,

                              _FnDesigId = data.ADFUNCTIONALDESIGNATIONID,

                              _OpDesc = data.OPERATION,

                              _OpId = data.OPERATIONID,

                              _SecDescrip = data.SECTION,

                              _SecId = data.SECTIONID,

                              _PlantId = data.SYPLANTID,

                              _SiteId = data.SYSITEID

                          }

                      ).FirstOrDefault();

            return KILIST;

        }



        public SearchIndentUser PRDashboadReport(SearchIndentUser VM)

        {

            DateTime ReqDateFrom = DateTime.Now.Date;

            DateTime ReqDateTo = DateTime.Now.Date;

            DateTime reportfrom = DateTime.ParseExact("15-JUL-2021", "dd-MMM-yyyy", null);

            if (!string.IsNullOrEmpty(VM.Startdate))

            {

                ReqDateFrom = DateTime.ParseExact(VM.Startdate, "dd-MMM-yyyy", null);

            }

            if (!string.IsNullOrEmpty(VM.ENDDATE))

            {

                ReqDateTo = DateTime.ParseExact(VM.ENDDATE + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);

            }





            List<VM_VW_PRUSERDASHBOARD> obj = new List<VM_VW_PRUSERDASHBOARD>();

            obj = (from data in _PrDBContext.VW_PRUSERDASHBOARD

                   where (!string.IsNullOrEmpty(VM.Startdate) ? (data.DATEADDED >= ReqDateFrom) : true)

                   && (!string.IsNullOrEmpty(VM.ENDDATE) ? (data.DATEADDED <= ReqDateTo) : true)

                   && (VM.OperationID != 0 ? data.OPERATIONID == VM.OperationID : true)

                   && (VM.DivisionID != 0 ? data.DIVISIONID == VM.DivisionID : true)

                   && (VM.DEPTID != 0 ? data.DEPARTMENTID == VM.DEPTID : true)

                   && (VM.SECID != 0 ? data.SECTIONID == VM.SECID : true)

                   && (string.IsNullOrEmpty(VM.IndentNo) ? true : data.INDENT_NO == VM.IndentNo)

                   && (VM.ecode == 0 ? true : data.ADDEDBY == VM.ecode)

                   && (VM.PRStatus != -1 ? data.POSTATUS == VM.PRStatus : true)

                   && (string.IsNullOrEmpty(VM.ITEM_DETAIL) ? true : data.ITEM_DETAIL.ToLower().Trim().Contains(VM.ITEM_DETAIL.ToLower().Trim()))

                   select new VM_VW_PRUSERDASHBOARD

                   {

                       ADDEDBYNAME = data.ADDEDBYNAME,

                       DATEADDED = data.DATEADDED,

                       INDENT_AMOUNT = data.INDENT_AMOUNT,

                       INDENT_NO = data.INDENT_NO,

                       ARIBARFPID = data.ARIBARFPID,

                       ARIBASTATUS = data.ARIBASTATUS,

                       ARIBABuyer_ECODE = (long)(data.ARIBABUYER == null ? 0 : data.ARIBABUYER),

                       PRHEADERID = data.PRHEADERID,

                       ITEM_DETAIL = data.ITEM_DETAIL,

                       POSTATUS = data.POSTATUS,

                       BUYERNAME = data.BUYERNAME,

                       BUYERSTATUS = data.BUYERSTATUS,

                       ADDEDBY = data.ADDEDBY,

                       DEPARTMENTID = data.DEPARTMENTID,

                       DIVISIONID = data.DIVISIONID,

                       OPERATIONID = data.OPERATIONID,

                       POAPPDATE = data.POAPPDATE,

                       POAPPROVALSTATUS = data.POAPPROVALSTATUS,

                       PONO = data.PONO,

                       PRAPPDATE = data.PRAPPDATE,

                       SECTIONID = data.SECTIONID,

                       UPDATEDATE = data.UPDATEDATE,

                       DEPARTMENT = data.DEPARTMENT,

                       DIVISION = data.DIVISION,

                       OPERATION = data.OPERATION,

                       SECTION = data.SECTION,

                       BUYSITEID = data.BUYSITEID,

                       //Added by aumento for the SR73841============

                       ARIBASRNO = data.ARIBASRNO,

                       //============================================ 

                   }).ToList();

            VM.PRDASHBOARDEPORT = obj.ToList();



            List<VM_HMSIHOLIDAYS> objcalender = new List<VM_HMSIHOLIDAYS>();

            objcalender = (from data in _PrDBContext.HMSIHOLIDAYS.Where(m => m.ACTIVE == 1 && m.MONTHDATEYEAR < DateTime.Today.Date)

                           select new VM_HMSIHOLIDAYS

                           {

                               ACTIVE = data.ACTIVE,

                               ADDEDBY = data.ADDEDBY,

                               DATEADDED = data.DATEADDED,

                               DATELSTMOD = data.DATELSTMOD,

                               HMSIHOLIDAYSID = data.HMSIHOLIDAYSID,

                               HOLIDAYDESCRIPTION = data.HOLIDAYDESCRIPTION,

                               ISHALFDAY = data.ISHALFDAY,

                               MODIFIEDBY = data.MODIFIEDBY,

                               MONTHDATEYEAR = data.MONTHDATEYEAR,

                               SYSITEID = data.SYSITEID

                           }

                         ).ToList();



            VM.BYE_Cal = objcalender;

            return VM;

        }



        //===========================================================================================================


        // START Added by Aumento ::  SR68003
        public object GetPRReleasePlants()
        {
            object data;

            data = _PrDBContext.SYPLANT.Where(x => x.ACTIVE == 1).Select(x =>
                 new
                 {
                     text = x.PLANTNAME,
                     value = x.SYPLANTID
                 }

                ).ToList();


            return data;

        }

        public List<PRAppAuthSeqViewModel> GetMappedFinanceUser(long? PlantID)
        {
            List<PRAppAuthSeqViewModel> resList = new List<PRAppAuthSeqViewModel>();
            //CommonRepository Comres = new CommonRepository();
            short APP_SEQNO = 1;

            try
            {
                List<DGIT_PRADDAPP_MST> PlantMappedFinanceUsers = _PrDBContext.DGIT_PRADDAPP_MST.Where(x => x.PLANTID == PlantID).ToList();

                if (PlantMappedFinanceUsers.Count > 0)
                {
                    foreach (var item in PlantMappedFinanceUsers)
                    {
                        Employee_Details Emp_Dtl = Comres.GetEmpDetailById(Convert.ToInt64(item.APPROVER));
                        short fnDesigId = Convert.ToInt16(Emp_Dtl._FnDesigId ?? 0);
                        resList.Add(new PRAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = Emp_Dtl._Desig,
                            APP_SEQ = 1,
                            APPTYPE = 2,
                            FNDESID = fnDesigId,
                            PRINTORDER = 1,
                            ISPARALELLAPP = (short)(PlantMappedFinanceUsers.Count > 1 ? 1 : 0),
                            ADDESIGNATIONID = Convert.ToInt16(Emp_Dtl._DesigId),
                            ADACTUAL_FUNCDESGID = fnDesigId


                        });

                    }
                }
            }
            catch (Exception ex)
            {

                resList = new List<PRAppAuthSeqViewModel>();
            }

            return resList;
        }


        // END Added by Aumento ::  SR68003


        //// Start Added By Aumento :: SR80255
        public Employee_Details GetDataForSendMailAfterAutoAssignBuyer(long id)
        {
            Employee_Details eobj = null;

            try
            {

                eobj = (from data in _PrDBContext.DGIT_PRHEADER.Where(x => x.PRHEADERID == id)
                        join _AddBy in _PrDBContext.ADEMPLOYEE on data.ARIBABUYER equals _AddBy.ADEMPCODE
                        join _VWAssociate in _PrDBContext.VW_ASSOCIATELVLDETAILS on data.ARIBABUYER equals _VWAssociate.ADEMPCODE
                        where _VWAssociate.SYKI == _Syki.SYKIID
                        select new Employee_Details
                        {
                            _EmailId = _AddBy.EMAILID,
                            _ECode = _AddBy.ADEMPCODE,
                            _EFirstName = _AddBy.FIRSTNAME,
                            _ELastName = _AddBy.LASTNAME,
                            _EName = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME
                        }).FirstOrDefault();
            }
            catch (Exception ex)
            {

                Console.WriteLine($"An error occurred: {ex.Message}");
                eobj = null;
            }

            return eobj;
        }
        //// End Added By Aumento :: SR80255

        //Added by TTL on 24-May-2025 against SR99130 > CR6361 - Start
        public List<DateTime> GetHolidaysByPlant(DateTime startDate, DateTime endDate, long siteId)
        {
            List<DateTime> countOfHolidays = _PrDBContext.HMSIHOLIDAYS.Where(h => h.SYSITEID == siteId && (h.MONTHDATEYEAR >= startDate && h.MONTHDATEYEAR <= endDate) && h.ACTIVE == 1)
                                                .Select(x=> x.MONTHDATEYEAR)
                                                .ToList();
            return countOfHolidays;
        }
        //Added by TTL on 24-May-2025 against SR99130 > CR6361 - End
    }
}
