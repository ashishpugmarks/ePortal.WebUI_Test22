using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ePortal.Infrastructure.Repositories
{
    public class ICIBMRepository
    {        
        private readonly ICProcessBDContext _ICDBContext;
        private readonly SYKI1 _Syki;
        public ICIBMRepository(ICProcessBDContext icDbContext)
        {
            _ICDBContext = icDbContext;
            _Syki = _ICDBContext.SYKI1.Where(x => x.ACTIVE == 1).FirstOrDefault(); ;
        }
        public List<VM_DGIT_ICINVEST_MST> GetInvestEffectList()
        {
            return (from oData in _ICDBContext.DGIT_ICINVEST_MST.Where(m => m.STATUS == 1)
                    select new VM_DGIT_ICINVEST_MST
                    {
                        ICINVESTMSTID = oData.ICINVESTMSTID,
                        INVESTEFFECTNAME = oData.INVESTEFFECTNAME
                    }
                    ).ToList();
        }
        public Tuple<short, long> SaveICRequest(ICREQHEADER model)
        {
            short retVal = 0; long retHeaderId = 0;
            Tuple<short, long> _retVal_tuple;
            using (var transaction = _ICDBContext.Database.BeginTransaction())
            {
                try
                {
                    DGIT_ICREQHEADER DPH = new DGIT_ICREQHEADER();
                    int FlagAdd = 0;
                    if (model.ICREQID > 0)
                    {
                        DPH = _ICDBContext.DGIT_ICREQHEADER.Where(x => x.ICREQID == model.ICREQID).SingleOrDefault();
                    }
                    else
                    {
                        //if (_ICDBContext.DGIT_ICREQHEADER.Any(x => x.POHEADERID != model.POHEADERID && x.PONO == model.PONO && (x.PROCESS_STATUS == 1 || x.PROCESS_STATUS == 0)))
                        //{
                        //    retVal = 2;
                        //    return _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId); //// -- record already exist.
                        //}

                        DPH = new DGIT_ICREQHEADER();
                        if (_ICDBContext.DGIT_ICREQHEADER.Count() == 0)
                        {
                            DPH.ICREQID = 1;
                        }
                        else
                        {
                            DPH.ICREQID = _ICDBContext.DGIT_ICREQHEADER.Max(x => x.ICREQID) + 1;
                        }
                        FlagAdd = 1;
                    }
                    if (model.IsFinalSubmit == 0)
                    {
                        DPH.ADDEDBY = model.ADDEDBY;
                        DPH.BASIC_BUDGET = model.BASIC_BUDGET;
                        DPH.BASIC_PROPOSED = model.BASIC_PROPOSED;
                        DPH.CONTENTOFEXE = model.CONTENTOFEXE;
                        DPH.COSTSAVING = model.COSTSAVING;
                        DPH.DATEADDED = DateTime.Today;
                        DPH.DEPARTMENT = model.DEPARTMENT;
                        DPH.ICA00 = model.ICA00;
                        DPH.ICBACKGROUND = model.ICBACKGROUND;
                        DPH.ICPURPOSE = model.ICPURPOSE;
                        DPH.ICTITLE = model.ICTITLE;
                        DPH.IMPORT_LOCAL = model.IMPORT_LOCAL;
                        DPH.PAYBACK_PERIOD = model.PAYBACK_PERIOD;
                        DPH.REQUIREMENT = model.REQUIREMENT;
                        DPH.TARGET = model.TARGET;
                        DPH.TAX_BUDGET = model.TAX_BUDGET;
                        DPH.TAX_CREDIT_AVAIL = model.TAX_CREDIT_AVAIL;
                        DPH.TAX_PROPOSED = model.TAX_PROPOSED;
                        DPH.INVEST_EFFECT = model.INVEST_EFFECT;
                        DPH.GAVAMT = model.GAVAMT;
                        DPH.NAVAMT = model.NAVAMT;
                        DPH.NPLAMT = model.NPLAMT;
                        DPH.SRVAMT = model.SRVAMT;
                    }
                    else
                    {
                        //if (!_ICDBContext.DGIT_PODETAIL.Any(x => x.POHEADERID == model.POHEADERID && x.DOC_TYPE == "PO"))
                        //{
                        //    retVal = 3;
                        //    return _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId); //// -- PO file not exist.
                        //}
                    }
                    DPH.PROCESSSTATUS = model.PROCESSSTATUS;
                    DPH.STATUS = model.STATUS;
                    DPH.REMARK = model.REMARK;
                    if (FlagAdd == 1)
                    {
                        DPH.ADDEDBY = model.ADDEDBY;
                        DPH.DATEADDED = DateTime.Now;
                    }
                    else
                    {
                        DPH.MODIFIEDBY = model.MODIFIEDBY;
                        DPH.MODIFIEDDATE = DateTime.Now;
                    }
                    _ICDBContext.Entry(DPH).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _ICDBContext.SaveChanges();

                    /// --- Save PO Detail --- ///
                    if (model.ICDOCDETAIL != null)
                    {
                        if (model.ICDOCDETAIL.Count > 0)
                        {
                            SaveICDetails(model.ADDEDBY, DPH.ICREQID, model.ICDOCDETAIL);
                        }
                    }
                    if (model.ICREQ_SCHEDULE != null && model.ICREQ_SCHEDULE.Count > 0)
                    {
                        SaveMilestoneSchdule(model.ADDEDBY, DPH.ICREQID, model.ICREQ_SCHEDULE);
                    }
                    /// --- Save PO Approval Auth Seq ---///
                    if (model.ICAPPAUTHSEQ != null)
                    {
                        if (model.ICAPPAUTHSEQ.Count > 0)
                        {
                            List<VM_DGIT_ICAPPAUTHSEQ> newappauthseq = new List<VM_DGIT_ICAPPAUTHSEQ>();
                            if (model.ICAPPAUTHSEQ.Count > 0)
                            {
                                newappauthseq.Add(new VM_DGIT_ICAPPAUTHSEQ
                                {
                                    ADEMPCODE = 1,
                                    ADEMPNAME = "IC/IBM Admin Approval",
                                    ADDESIGNATION = "",
                                    APP_SEQ = 0,
                                    APPTYPE = 4,
                                });
                                foreach (var obj in model.ICAPPAUTHSEQ)
                                {
                                    obj.APP_SEQ = (short)(newappauthseq.Count);
                                    newappauthseq.Add(obj);
                                }
                                newappauthseq.Add(new VM_DGIT_ICAPPAUTHSEQ
                                {
                                    ADEMPCODE = 1,
                                    ADEMPNAME = "IC/IBM Admin Approval",
                                    ADDESIGNATION = "",
                                    APP_SEQ = (short)(newappauthseq.Count),//(model.ICAPPAUTHSEQ.Count == 0 ? (short)1 : Convert.ToInt16(model.ICAPPAUTHSEQ.Max(x => x.APP_SEQ) + 1)),
                                    APPTYPE = 3,
                                });
                                model.ICAPPAUTHSEQ = newappauthseq;
                            }
                            SaveAppAuthSeq(model.ADDEDBY, DPH.ICREQID, model.ICAPPAUTHSEQ);

                            /// --- Save PO Approval Authority --- ///
                            VM_DGIT_ICAPPAUTHSEQ seqModel = model.ICAPPAUTHSEQ.OrderBy(o => o.APP_SEQ).FirstOrDefault();
                            if (seqModel != null)
                            {
                                SaveICAppHis(model.ADDEDBY, DPH.ICREQID, seqModel);
                            }
                        }
                    }

                    transaction.Commit();
                    retVal = 1;
                    retHeaderId = DPH.ICREQID;
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
        public short SaveICDetails(long AddedBy, long ICHeaderId, List<VM_DGIT_ICDOCDETAIL> PDVMList)
        {
            short retVal = 0;
            foreach (VM_DGIT_ICDOCDETAIL PDVM in PDVMList)
            {
                DGIT_ICDOCDETAIL DPD = new DGIT_ICDOCDETAIL();
                int FlagAdd = 0;
                if (ICHeaderId > 0)
                {
                    //DPD = _PoDBContext.DGIT_PODETAIL.Where(x => x.POHEADERID == POHeaderId && x.DOC_TYPE == PDVM.DOC_TYPE && x.FILENAME == PDVM.FILENAME).SingleOrDefault();
                    //if (DPD != null)
                    //{
                    //    _PoDBContext.DGIT_PODETAIL.Remove(DPD);
                    //    _PoDBContext.SaveChanges();
                    //}
                    //if (PDVM.DOC_TYPE == "ICDOC")
                    //{
                    //    DPD = _ICDBContext.DGIT_ICDOCDETAIL.Where(x => x.ICREQID == ICHeaderId && x.DOC_TYPE == PDVM.DOC_TYPE).FirstOrDefault();
                    //    if (DPD != null)
                    //    {
                    //        _ICDBContext.DGIT_ICDOCDETAIL.Remove(DPD);
                    //        _ICDBContext.SaveChanges();
                    //    }
                    //}

                    DPD = new DGIT_ICDOCDETAIL();
                    if (_ICDBContext.DGIT_ICDOCDETAIL.Count() == 0)
                    {
                        DPD.ICDOC_ID = 1;
                    }
                    else
                    {
                        DPD.ICDOC_ID = _ICDBContext.DGIT_ICDOCDETAIL.Max(x => x.ICDOC_ID) + 1;
                    }
                    FlagAdd = 1;

                    DPD.ICREQID = ICHeaderId;
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
                    _ICDBContext.Entry(DPD).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _ICDBContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }
        public short SaveMilestoneSchdule(long AddedBy, long ICHeaderId, List<VM_DGIT_ICREQ_SCHEDULE> PDVMList)
        {
            short retVal = 0;
            List<DGIT_ICREQ_SCHEDULE> SeqList = _ICDBContext.DGIT_ICREQ_SCHEDULE.Where(t => t.ICREQID == ICHeaderId).ToList();
            if (SeqList.Count > 0)
            {
                _ICDBContext.DGIT_ICREQ_SCHEDULE.RemoveRange(SeqList);
                _ICDBContext.SaveChanges();
            }
            foreach (VM_DGIT_ICREQ_SCHEDULE PDVM in PDVMList)
            {
                DGIT_ICREQ_SCHEDULE DPD = new DGIT_ICREQ_SCHEDULE();
                int FlagAdd = 0;
                if (ICHeaderId > 0)
                {

                    DPD = new DGIT_ICREQ_SCHEDULE();
                    if (_ICDBContext.DGIT_ICREQ_SCHEDULE.Count() == 0)
                    {
                        DPD.ICREQSCHID = 1;
                    }
                    else
                    {
                        DPD.ICREQSCHID = _ICDBContext.DGIT_ICREQ_SCHEDULE.Max(x => x.ICREQSCHID) + 1;
                    }
                    FlagAdd = 1;

                    DPD.ICREQID = ICHeaderId;
                    DPD.TARGET = PDVM.TARGET;
                    DPD.SCHEDULE = PDVM.SCHEDULE;
                    if (FlagAdd == 1)
                    {
                        DPD.ADDEDBY = AddedBy;
                        DPD.DATEADDED = DateTime.Now;
                    }
                    else
                    {
                        DPD.MODIFIEDBY = AddedBy;
                        DPD.MODIFIEDDATE = DateTime.Now;
                    }
                    _ICDBContext.Entry(DPD).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _ICDBContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }
        public void SaveAppAuthSeq(long AddedBy, long ICHeaderId, List<VM_DGIT_ICAPPAUTHSEQ> PSVMList)
        {
            //// --- Delete recode ---////
            List<DGIT_ICAPPAUTHSEQ> SeqList = _ICDBContext.DGIT_ICAPPAUTHSEQ.Where(t => t.ICREQID == ICHeaderId).ToList();
            if (SeqList.Count > 0)
            {
                _ICDBContext.DGIT_ICAPPAUTHSEQ.RemoveRange(SeqList);
                _ICDBContext.SaveChanges();
            }

            foreach (VM_DGIT_ICAPPAUTHSEQ PSVM in PSVMList.OrderBy(o => o.APP_SEQ).ToList())
            {
                DGIT_ICAPPAUTHSEQ DAAS = new DGIT_ICAPPAUTHSEQ();
                if (_ICDBContext.DGIT_ICAPPAUTHSEQ.Count() == 0)
                {
                    DAAS.ICAPPAUTH_ID = 1;
                }
                else
                {
                    DAAS.ICAPPAUTH_ID = _ICDBContext.DGIT_ICAPPAUTHSEQ.Max(x => x.ICAPPAUTH_ID) + 1;
                }
                DAAS.ICREQID = ICHeaderId;
                DAAS.ADEMPCODE = PSVM.ADEMPCODE;
                DAAS.APP_SEQ = PSVM.APP_SEQ;
                DAAS.APPTYPE = PSVM.APPTYPE;
                DAAS.STATUS = 1;
                DAAS.ADDEDBY = AddedBy;
                DAAS.ADDEDDATE = DateTime.Now;
                _ICDBContext.Entry(DAAS).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                _ICDBContext.SaveChanges();
            }
        }

        public void SaveICAppHis(long AddedBy, long ICHeaderId, VM_DGIT_ICAPPAUTHSEQ PSVM)
        {
            DGIT_ICAPPHISTORY DPAH = new DGIT_ICAPPHISTORY();
            int FlagAdd = 0;
            // DPAH = _PoDBContext.DGIT_POAPPHISTORY.Where(d => d.ADEMPCODE == PSVM.ADEMPCODE && d.POID == POHeaderId).FirstOrDefault();
            DPAH = _ICDBContext.DGIT_ICAPPHISTORY.Where(d => d.ADEMPCODE == PSVM.ADEMPCODE && d.ICREQID == ICHeaderId && d.APPROVAL_STATUS == 0).FirstOrDefault();
            if (DPAH == null)
            {
                DPAH = new DGIT_ICAPPHISTORY();
                if (_ICDBContext.DGIT_ICAPPHISTORY.Count() == 0)
                {
                    DPAH.ICAPPHISTORY_ID = 1;
                }
                else
                {
                    DPAH.ICAPPHISTORY_ID = _ICDBContext.DGIT_ICAPPHISTORY.Max(x => x.ICAPPHISTORY_ID) + 1;
                }
                FlagAdd = 1;
            }
            DPAH.ICREQID = ICHeaderId;
            DPAH.ADEMPCODE = PSVM.ADEMPCODE;
            DPAH.APP_TYPE = PSVM.APPTYPE;
            DPAH.APPROVAL_STATUS = 0;
            DPAH.APP_SEQ = PSVM.APP_SEQ;
            DPAH.APPROVAL_REMARK = "";
            DPAH.APP_HEADER = PSVM.APP_HEADER;
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
            _ICDBContext.Entry(DPAH).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
            _ICDBContext.SaveChanges();
        }

        public List<VM_DGIT_ICDOCDETAIL> GetAttachmentDetail(long _ICHEADERID)
        {
            return (from _ICDetail in _ICDBContext.DGIT_ICDOCDETAIL.Where(d => d.ICREQID == _ICHEADERID)
                    where _ICDetail.STATUS == 1
                    select new VM_DGIT_ICDOCDETAIL
                    {
                        ICDOC_ID = _ICDetail.ICDOC_ID,
                        ICREQID = _ICDetail.ICREQID,
                        DOC_TYPE = _ICDetail.DOC_TYPE,
                        ADDITIONAL_INFO = _ICDetail.ADDITIONAL_INFO,
                        FILENAME = _ICDetail.FILENAME,
                    }).ToList();
        }
        public short DeleteAttachment(string fileName, string docType, long ICHeaderId)
        {
            short retVal = 0;
            if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(docType) && ICHeaderId > 0)
            {
                DGIT_ICDOCDETAIL DT = _ICDBContext.DGIT_ICDOCDETAIL.Where(x => x.FILENAME == fileName && x.DOC_TYPE == docType && x.ICREQID == ICHeaderId).FirstOrDefault();
                if (DT != null)
                {
                    _ICDBContext.DGIT_ICDOCDETAIL.Remove(DT);
                    _ICDBContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }

        public ICREQHEADER GetICRequestById(long id)
        {
            //var _obj = (from data in _ICDBContext.DGIT_ICREQHEADER.Where(x => x.ICREQID == id)
            //            join _AddBy in _ICDBContext.ADEMPLOYEEs1 on data.ADDEDBY equals _AddBy.ADEMPCODE
            //            join _VWAssociate in _ICDBContext.VW_ASSOCIATELVLDETAILS1 on data.ADDEDBY equals _VWAssociate.ADEMPCODE
            //            let syki = _ICDBContext.VW_ASSOCIATELVLDETAILS1.Where(x => x.ADEMPCODE == data.ADDEDBY).Max(x => x.SYKI) //SR97493 - Getting last active KI of initiator
            //            join _cy in _ICDBContext.DGIT_ICCONFIG_MST on data.ICCONFIGID equals _cy.ICCONFIGID into _cyl
            //            from _cycle in _cyl.DefaultIfEmpty()
            //                //where _VWAssociate.SYKI == _Syki.SYKIID //Resigned users which were not available in current Ki, request is not accessible due to this. SR97493
            //            where _VWAssociate.SYKI == syki //SR97493 - Getting last KI record of initiator
            //            select new ICREQHEADER
            //            {
            //                ADDEDBY = data.ADDEDBY,
            //                BASIC_BUDGET = data.BASIC_BUDGET,
            //                BASIC_PROPOSED = data.BASIC_PROPOSED,
            //                ICREQID = data.ICREQID,
            //                CONTENTOFEXE = data.CONTENTOFEXE,
            //                COSTSAVING = data.COSTSAVING,
            //                DEPARTMENT = data.DEPARTMENT,
            //                ICA00 = data.ICA00,
            //                ICBACKGROUND = data.ICBACKGROUND,
            //                ICPURPOSE = data.ICPURPOSE,
            //                ICTITLE = data.ICTITLE,
            //                IMPORT_LOCAL = data.IMPORT_LOCAL,
            //                INVEST_EFFECT = data.INVEST_EFFECT,
            //                PAYBACK_PERIOD = data.PAYBACK_PERIOD,
            //                REQUIREMENT = data.REQUIREMENT,
            //                STATUS = data.STATUS,
            //                TARGET = data.TARGET,
            //                TAX_BUDGET = data.TAX_BUDGET,
            //                TAX_CREDIT_AVAIL = data.TAX_CREDIT_AVAIL,
            //                TAX_PROPOSED = data.TAX_PROPOSED,
            //                GAVAMT = data.GAVAMT,
            //                SRVAMT = data.SRVAMT,
            //                NAVAMT = data.NAVAMT,
            //                NPLAMT = data.NPLAMT,
            //                REMARK = data.REMARK,
            //                ADDEDBYNAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
            //                DATEADDED = data.DATEADDED,
            //                PROCESSSTATUS = data.PROCESSSTATUS,
            //                iccycle = _cycle.ICMONTH,
            //                ISICIBMREQ = (data.ISIBMREQUIRED == 1 ? "Yes" : "No"),
            //                AUCCODE = data.AUCCODE,
            //                AUCCODEREMARK = data.AUCCODEREMARK,
            //                AUCCODEUPDBY = data.AUCCODEUPDBY,
            //                AUCCODEUPDDATE = data.AUCCODEUPDDATE,
            //                ICDOCDETAIL = (from _ICDetail in _ICDBContext.DGIT_ICDOCDETAIL.Where(d => d.ICREQID == data.ICREQID)
            //                               where _ICDetail.STATUS == 1
            //                               select new VM_DGIT_ICDOCDETAIL
            //                               {
            //                                   ICDOC_ID = _ICDetail.ICDOC_ID,
            //                                   ICREQID = _ICDetail.ICREQID,
            //                                   DOC_TYPE = _ICDetail.DOC_TYPE,
            //                                   ADDITIONAL_INFO = _ICDetail.ADDITIONAL_INFO,
            //                                   FILENAME = _ICDetail.FILENAME,
            //                               }).ToList(),
            //                ICREQ_SCHEDULE = (from _ICSch in _ICDBContext.DGIT_ICREQ_SCHEDULE.Where(l => l.ICREQID == data.ICREQID)
            //                                  select new VM_DGIT_ICREQ_SCHEDULE
            //                                  {
            //                                      ICREQID = _ICSch.ICREQID,
            //                                      ICREQSCHID = _ICSch.ICREQSCHID,
            //                                      SCHEDULE = _ICSch.SCHEDULE,
            //                                      TARGET = _ICSch.TARGET,
            //                                      //SCHEDULESTR= string.Format("dd-MMM-yyyy",_ICSch.SCHEDULE.ToString("dd-MMM-yyyy")),
            //                                  }).OrderBy(l => l.ICREQSCHID).ToList(),
            //                ICINVEST_EFFECT = (from _iceffect in _ICDBContext.DGIT_ICINVEST_MST.Where(m => m.STATUS == 1)
            //                                   select new VM_SELECTITEMLIST
            //                                   {
            //                                       Text = _iceffect.INVESTEFFECTNAME,
            //                                       Value = _iceffect.ICINVESTMSTID.ToString(),
            //                                   }).ToList(),
            //                Emp_Detail = new Employee_Details
            //                {
            //                    _ECode = _AddBy.ADEMPCODE,
            //                    _EName = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
            //                    _EmailId = _AddBy.EMAILID,
            //                    _DOB = DateTime.Now,
            //                    _SecDescrip = _VWAssociate.SECTION,
            //                    _DepDesc = _VWAssociate.DEPARTMENT,
            //                    _DivDesc = _VWAssociate.DIVISION,
            //                    _OpDesc = _VWAssociate.OPERATION,
            //                    _SiteId = _VWAssociate.SYSITEID
            //                },
            //                ICAPPAUTHSEQ = (from _ICSeq in _ICDBContext.DGIT_ICAPPAUTHSEQ.Where(x => x.ICREQID == data.ICREQID)
            //                                join _AppSeqEmp in _ICDBContext.ADEMPLOYEEs1 on _ICSeq.ADEMPCODE equals _AppSeqEmp.ADEMPCODE
            //                                join _Vw in _ICDBContext.VW_ASSOCIATELVLDETAILS1 on _ICSeq.ADEMPCODE equals _Vw.ADEMPCODE
            //                                join _Desg in _ICDBContext.ADDESIGNATION1 on _Vw.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
            //                                where _Vw.SYKI == _Syki.SYKIID
            //                                select new VM_DGIT_ICAPPAUTHSEQ
            //                                {
            //                                    ICREQID = _ICSeq.ICREQID,
            //                                    ICAPPAUTH_ID = _ICSeq.ICAPPAUTH_ID,
            //                                    ADEMPCODE = _ICSeq.ADEMPCODE,
            //                                    STATUS = _ICSeq.STATUS,
            //                                    APP_SEQ = _ICSeq.APP_SEQ,
            //                                    ADEMPNAME = _AppSeqEmp.FIRSTNAME + " " + _AppSeqEmp.LASTNAME,
            //                                    ADDESIGNATION = _Desg.DESCRIP,
            //                                    ADDEDDATE = _ICSeq.ADDEDDATE,
            //                                    UPDATEBY = _ICSeq.UPDATEBY,
            //                                    UPDATEDATE = _ICSeq.UPDATEDATE,
            //                                    APPTYPE = _ICSeq.APPTYPE,
            //                                    APP_HEADER = _ICSeq.APP_HEADER,
            //                                }).OrderBy(b => b.APP_SEQ).ToList(),
            //                ICAPPHISTORY = (from _POAppHis in _ICDBContext.DGIT_ICAPPHISTORY.Where(x => x.ICREQID == data.ICREQID)
            //                                join _AppEmp in _ICDBContext.ADEMPLOYEEs1 on _POAppHis.ADEMPCODE equals _AppEmp.ADEMPCODE
            //                                select new VM_DGIT_ICAPPHISTORY
            //                                {
            //                                    ICAPPHISTORY_ID = _POAppHis.ICAPPHISTORY_ID,
            //                                    ICREQID = _POAppHis.ICREQID,
            //                                    ADEMPCODE = _POAppHis.ADEMPCODE,
            //                                    APPROVAL_STATUS = _POAppHis.APPROVAL_STATUS,
            //                                    APPROVAL_REMARK = _POAppHis.APPROVAL_REMARK,
            //                                    APPEMP_NAME = _AppEmp.FIRSTNAME + " " + _AppEmp.LASTNAME + " - [" + _AppEmp.ADEMPCODE + "]",
            //                                    APP_EMAIL = _AppEmp.EMAILID,
            //                                    APPEMP_CODE = _AppEmp.ADEMPCODE.ToString(),
            //                                    ADDEDDATE = _POAppHis.ADDEDDATE,
            //                                    UPDATEBY = _POAppHis.UPDATEBY,
            //                                    UPDATEDATE = _POAppHis.UPDATEDATE,
            //                                    APP_DATE = _POAppHis.APP_DATE,
            //                                    APP_TYPE = _POAppHis.APP_TYPE,
            //                                    APP_HEADER = _POAppHis.APP_HEADER
            //                                }).OrderBy(o => o.ICAPPHISTORY_ID).ToList(),

            //            }).FirstOrDefault();

            var _obj = (from data in _ICDBContext.DGIT_ICREQHEADER.Where(x => x.ICREQID == id)
                        join _AddBy in _ICDBContext.ADEMPLOYEEs1 on data.ADDEDBY equals _AddBy.ADEMPCODE
                        join _VWAssociate in _ICDBContext.VW_ASSOCIATELVLDETAILS1 on data.ADDEDBY equals _VWAssociate.ADEMPCODE
                        let syki = _ICDBContext.VW_ASSOCIATELVLDETAILS1.Where(x => x.ADEMPCODE == data.ADDEDBY).Max(x => x.SYKI) //SR97493 - Getting last active KI of initiator
                        join _cy in _ICDBContext.DGIT_ICCONFIG_MST on data.ICCONFIGID equals _cy.ICCONFIGID into _cyl
                        from _cycle in _cyl.DefaultIfEmpty()
                            //where _VWAssociate.SYKI == _Syki.SYKIID //Resigned users which were not available in current Ki, request is not accessible due to this. SR97493
                        where _VWAssociate.SYKI == syki //SR97493 - Getting last KI record of initiator
                        select new ICREQHEADER
                        {
                            ADDEDBY = data.ADDEDBY,
                            BASIC_BUDGET = data.BASIC_BUDGET,
                            BASIC_PROPOSED = data.BASIC_PROPOSED,
                            ICREQID = data.ICREQID,
                            CONTENTOFEXE = data.CONTENTOFEXE,
                            COSTSAVING = data.COSTSAVING,
                            DEPARTMENT = data.DEPARTMENT,
                            ICA00 = data.ICA00,
                            ICBACKGROUND = data.ICBACKGROUND,
                            ICPURPOSE = data.ICPURPOSE,
                            ICTITLE = data.ICTITLE,
                            IMPORT_LOCAL = data.IMPORT_LOCAL,
                            INVEST_EFFECT = data.INVEST_EFFECT,
                            PAYBACK_PERIOD = data.PAYBACK_PERIOD,
                            REQUIREMENT = data.REQUIREMENT,
                            STATUS = data.STATUS,
                            TARGET = data.TARGET,
                            TAX_BUDGET = data.TAX_BUDGET,
                            TAX_CREDIT_AVAIL = data.TAX_CREDIT_AVAIL,
                            TAX_PROPOSED = data.TAX_PROPOSED,
                            GAVAMT = data.GAVAMT,
                            SRVAMT = data.SRVAMT,
                            NAVAMT = data.NAVAMT,
                            NPLAMT = data.NPLAMT,
                            REMARK = data.REMARK,
                            ADDEDBYNAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                            DATEADDED = data.DATEADDED,
                            PROCESSSTATUS = data.PROCESSSTATUS,
                            iccycle = _cycle.ICMONTH,
                            ISICIBMREQ = (data.ISIBMREQUIRED == 1 ? "Yes" : "No"),
                            AUCCODE = data.AUCCODE,
                            AUCCODEREMARK = data.AUCCODEREMARK,
                            AUCCODEUPDBY = data.AUCCODEUPDBY,
                            AUCCODEUPDDATE = data.AUCCODEUPDDATE,
                            Emp_Detail = new Employee_Details
                            {
                                _ECode = _AddBy.ADEMPCODE,
                                _EName = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                                _EmailId = _AddBy.EMAILID,
                                _DOB = DateTime.Now,
                                _SecDescrip = _VWAssociate.SECTION,
                                _DepDesc = _VWAssociate.DEPARTMENT,
                                _DivDesc = _VWAssociate.DIVISION,
                                _OpDesc = _VWAssociate.OPERATION,
                                _SiteId = _VWAssociate.SYSITEID
                            },
                        }).FirstOrDefault();

            if (_obj != null)
            {
                _obj.ICDOCDETAIL = (from _ICDetail in _ICDBContext.DGIT_ICDOCDETAIL.Where(d => d.ICREQID == _obj.ICREQID)
                                    where _ICDetail.STATUS == 1
                                    select new VM_DGIT_ICDOCDETAIL
                                    {
                                        ICDOC_ID = _ICDetail.ICDOC_ID,
                                        ICREQID = _ICDetail.ICREQID,
                                        DOC_TYPE = _ICDetail.DOC_TYPE,
                                        ADDITIONAL_INFO = _ICDetail.ADDITIONAL_INFO,
                                        FILENAME = _ICDetail.FILENAME,
                                    }).ToList();

                _obj.ICREQ_SCHEDULE = (from _ICSch in _ICDBContext.DGIT_ICREQ_SCHEDULE.Where(l => l.ICREQID == _obj.ICREQID)
                                       select new VM_DGIT_ICREQ_SCHEDULE
                                       {
                                           ICREQID = _ICSch.ICREQID,
                                           ICREQSCHID = _ICSch.ICREQSCHID,
                                           SCHEDULE = _ICSch.SCHEDULE,
                                           TARGET = _ICSch.TARGET,
                                           //SCHEDULESTR= string.Format("dd-MMM-yyyy",_ICSch.SCHEDULE.ToString("dd-MMM-yyyy")),
                                       }).OrderBy(l => l.ICREQSCHID).ToList();

                _obj.ICINVEST_EFFECT = (from _iceffect in _ICDBContext.DGIT_ICINVEST_MST.Where(m => m.STATUS == 1)
                                        select new VM_SELECTITEMLIST
                                        {
                                            Text = _iceffect.INVESTEFFECTNAME,
                                            Value = _iceffect.ICINVESTMSTID.ToString(),
                                        }).ToList();
                
                _obj.ICAPPAUTHSEQ = (from _ICSeq in _ICDBContext.DGIT_ICAPPAUTHSEQ.Where(x => x.ICREQID == _obj.ICREQID)
                                     join _AppSeqEmp in _ICDBContext.ADEMPLOYEEs1 on _ICSeq.ADEMPCODE equals _AppSeqEmp.ADEMPCODE
                                     join _Vw in _ICDBContext.VW_ASSOCIATELVLDETAILS1 on _ICSeq.ADEMPCODE equals _Vw.ADEMPCODE
                                     join _Desg in _ICDBContext.ADDESIGNATION1 on _Vw.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
                                     where _Vw.SYKI == _Syki.SYKIID
                                     select new VM_DGIT_ICAPPAUTHSEQ
                                     {
                                         ICREQID = _ICSeq.ICREQID,
                                         ICAPPAUTH_ID = _ICSeq.ICAPPAUTH_ID,
                                         ADEMPCODE = _ICSeq.ADEMPCODE,
                                         STATUS = _ICSeq.STATUS,
                                         APP_SEQ = _ICSeq.APP_SEQ,
                                         ADEMPNAME = _AppSeqEmp.FIRSTNAME + " " + _AppSeqEmp.LASTNAME,
                                         ADDESIGNATION = _Desg.DESCRIP,
                                         ADDEDDATE = _ICSeq.ADDEDDATE,
                                         UPDATEBY = _ICSeq.UPDATEBY,
                                         UPDATEDATE = _ICSeq.UPDATEDATE,
                                         APPTYPE = _ICSeq.APPTYPE,
                                         APP_HEADER = _ICSeq.APP_HEADER,
                                     }).OrderBy(b => b.APP_SEQ).ToList();

                _obj.ICAPPHISTORY = (from _POAppHis in _ICDBContext.DGIT_ICAPPHISTORY.Where(x => x.ICREQID == _obj.ICREQID)
                                     join _AppEmp in _ICDBContext.ADEMPLOYEEs1 on _POAppHis.ADEMPCODE equals _AppEmp.ADEMPCODE
                                     select new VM_DGIT_ICAPPHISTORY
                                     {
                                         ICAPPHISTORY_ID = _POAppHis.ICAPPHISTORY_ID,
                                         ICREQID = _POAppHis.ICREQID,
                                         ADEMPCODE = _POAppHis.ADEMPCODE,
                                         APPROVAL_STATUS = _POAppHis.APPROVAL_STATUS,
                                         APPROVAL_REMARK = _POAppHis.APPROVAL_REMARK,
                                         APPEMP_NAME = _AppEmp.FIRSTNAME + " " + _AppEmp.LASTNAME + " - [" + _AppEmp.ADEMPCODE + "]",
                                         APP_EMAIL = _AppEmp.EMAILID,
                                         APPEMP_CODE = _AppEmp.ADEMPCODE.ToString(),
                                         ADDEDDATE = _POAppHis.ADDEDDATE,
                                         UPDATEBY = _POAppHis.UPDATEBY,
                                         UPDATEDATE = _POAppHis.UPDATEDATE,
                                         APP_DATE = _POAppHis.APP_DATE,
                                         APP_TYPE = _POAppHis.APP_TYPE,
                                         APP_HEADER = _POAppHis.APP_HEADER
                                     }).OrderBy(o => o.ICAPPHISTORY_ID).ToList();

            }

            _obj.ICAPPHISTORY.AddRange(
                from data in _ICDBContext.DGIT_ICAPPHISTORY.Where(m => m.APPROVAL_STATUS == 0 && m.ICREQID == _obj.ICREQID && (m.APP_TYPE == (short)3 || m.APP_TYPE == (short)4)).ToList()
                select new VM_DGIT_ICAPPHISTORY
                {
                    ICAPPHISTORY_ID = data.ICAPPHISTORY_ID,
                    ICREQID = data.ICREQID,
                    ADEMPCODE = data.ADEMPCODE,
                    APPROVAL_STATUS = data.APPROVAL_STATUS,
                    APPROVAL_REMARK = data.APPROVAL_REMARK,
                    //APPEMP_NAME = _AppEmp.FIRSTNAME + " " + _AppEmp.LASTNAME + " - [" + _AppEmp.ADEMPCODE + "]",
                    //APP_EMAIL = _AppEmp.EMAILID,
                    //APPEMP_CODE = _AppEmp.ADEMPCODE.ToString(),
                    APP_TYPE = data.APP_TYPE,
                    APP_HEADER = data.APP_HEADER
                });
            if (_obj.ICAPPAUTHSEQ != null && _obj.ICAPPAUTHSEQ.Count > 0)
            {
                long lastSendBackAppHisId = 0;
                //DGIT_ICAPPHISTORY lastSendBackAppHis = _ICDBContext.DGIT_ICAPPHISTORY.Where(e => e.ICREQID == _obj.ICREQID && e.APPROVAL_STATUS == 2).OrderByDescending(o => o.ICAPPHISTORY_ID).FirstOrDefault();
                DGIT_ICAPPHISTORY lastSendBackAppHis = _ICDBContext.DGIT_ICAPPHISTORY.Where(e => e.ICREQID == _obj.ICREQID).OrderByDescending(o => o.ICAPPHISTORY_ID).FirstOrDefault();
                short? lastseqid = lastSendBackAppHis.APP_SEQ;
                if (lastSendBackAppHis != null)
                {
                    lastSendBackAppHisId = lastSendBackAppHis.ICAPPHISTORY_ID;
                }
                foreach (VM_DGIT_ICAPPAUTHSEQ obj in _obj.ICAPPAUTHSEQ)
                {
                    //bool IsBeforeSendBackRecord = _ICDBContext.DGIT_ICAPPHISTORY.Any(w => w.ICREQID == obj.ICREQID && w.ICAPPHISTORY_ID <= lastSendBackAppHisId && w.ADEMPCODE == obj.ADEMPCODE);
                    //if ((IsBeforeSendBackRecord ? (!_obj.ICAPPHISTORY.Any(r => r.ICREQID == obj.ICREQID && r.ADEMPCODE == obj.ADEMPCODE && r.ICAPPHISTORY_ID > lastSendBackAppHisId)) : (!_obj.ICAPPHISTORY.Any(x => x.ICREQID == obj.ICREQID && x.ADEMPCODE == obj.ADEMPCODE))))
                    if (obj.APP_SEQ > lastseqid)
                    {
                        _obj.ICAPPHISTORY.Add(new VM_DGIT_ICAPPHISTORY
                        {
                            ICAPPHISTORY_ID = 0,
                            ICREQID = obj.ICREQID,
                            ADEMPCODE = obj.ADEMPCODE,
                            APPROVAL_STATUS = 0,
                            APPROVAL_REMARK = "",
                            APPEMP_NAME = obj.ADEMPNAME + "[" + obj.ADEMPCODE + "]",
                            APP_TYPE = obj.APPTYPE,
                            APP_HEADER = obj.APP_HEADER
                        });
                    }
                }
            }
            foreach (var objs in _obj.ICREQ_SCHEDULE)
            {
                objs.SCHEDULESTR = objs.SCHEDULE.ToString("MMM-yyyy");
            }
            return _obj;
        }

        public short ICApproval(VM_DGIT_ICAPPHISTORY PHVM)
        {
            short retVal = 0;
            using (var transaction = _ICDBContext.Database.BeginTransaction())
            {
                try
                {
                    DGIT_ICAPPHISTORY DPAH = new DGIT_ICAPPHISTORY();
                    if (PHVM.ICREQID > 0)
                    {
                        /////////// Update Approval Status //////////
                        // DPAH = _PoDBContext.DGIT_POAPPHISTORY.Where(x => x.POID == PHVM.POID && x.ADEMPCODE == PHVM.ADEMPCODE).FirstOrDefault();
                        DPAH = _ICDBContext.DGIT_ICAPPHISTORY.Where(x => x.ICREQID == PHVM.ICREQID && x.ADEMPCODE == PHVM.ADEMPCODE && x.APPROVAL_STATUS == 0).FirstOrDefault();
                        if (DPAH != null)
                        {
                            PHVM.APP_SEQ = DPAH.APP_SEQ;
                            PHVM.APP_TYPE = DPAH.APP_TYPE;
                            DPAH.APPROVAL_STATUS = PHVM.APPROVAL_STATUS;
                            DPAH.APPROVAL_REMARK = PHVM.APPROVAL_REMARK;
                            DPAH.UPDATEBY = PHVM.UPDATEBY;
                            DPAH.APP_DATE = DateTime.Now;
                            DPAH.UPDATEDATE = DateTime.Now;
                            _ICDBContext.Entry(DPAH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _ICDBContext.SaveChanges();
                        }
                        var appcntlst = _ICDBContext.DGIT_ICAPPHISTORY.Where(x => x.ICREQID == PHVM.ICREQID && x.APP_SEQ == PHVM.APP_SEQ && x.APPROVAL_STATUS == 0);
                        if (appcntlst.Count() > 0)
                        {
                            if (PHVM.APPROVAL_STATUS != 1)
                            {
                                foreach (var obj in appcntlst)
                                {
                                    _ICDBContext.DGIT_ICAPPHISTORY.Remove(obj);
                                    _ICDBContext.SaveChanges();
                                }
                                if (PHVM.APP_TYPE == 2)
                                {
                                    var appseqlst = _ICDBContext.DGIT_ICAPPAUTHSEQ.Where(x => x.APP_SEQ >= PHVM.APP_SEQ);
                                    foreach (var data in appseqlst)
                                    {
                                        _ICDBContext.DGIT_ICAPPAUTHSEQ.Remove(data);
                                        _ICDBContext.SaveChanges();
                                    }
                                }
                                DGIT_ICREQHEADER DPH1 = new DGIT_ICREQHEADER(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                                DPH1 = _ICDBContext.DGIT_ICREQHEADER.Where(x => x.ICREQID == PHVM.ICREQID).SingleOrDefault();

                                if (DPH1 != null)
                                {
                                    if (PHVM.APPROVAL_STATUS == 2)
                                    {
                                        if (PHVM.APP_TYPE == 2)
                                            DPH1.PROCESSSTATUS = 2;
                                        else
                                            DPH1.PROCESSSTATUS = 5;
                                    }
                                    else if (PHVM.APPROVAL_STATUS == 3)
                                    {
                                        if (PHVM.APP_TYPE == 2)
                                            DPH1.PROCESSSTATUS = 2;
                                        else
                                            DPH1.PROCESSSTATUS = 3;
                                    }
                                    DPH1.MODIFIEDBY = PHVM.UPDATEBY;
                                    DPH1.MODIFIEDDATE = DateTime.Now;
                                    _ICDBContext.Entry(DPH1).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                    _ICDBContext.SaveChanges();
                                }
                            }

                            retVal = 1;
                            transaction.Commit();
                            return retVal;
                        }
                        /// --- Save PO Approval Authority --- ///
                        List<VM_DGIT_ICAPPAUTHSEQ> seqModel = new List<VM_DGIT_ICAPPAUTHSEQ>();
                        if (PHVM.APPROVAL_STATUS == 1)
                        {
                            short nextSeq = (short)(PHVM.APP_SEQ + 1);
                            long _nextEmpAuth = _ICDBContext.DGIT_ICAPPAUTHSEQ.Where(a => a.APP_SEQ == nextSeq && a.ICREQID == PHVM.ICREQID).Select(s => s.ADEMPCODE).FirstOrDefault();
                            if (_nextEmpAuth == PHVM.ADEMPCODE)
                            {
                                nextSeq = Convert.ToInt16(nextSeq + 1);
                            }
                            seqModel = (from data in _ICDBContext.DGIT_ICAPPAUTHSEQ.Where(a => a.APP_SEQ == nextSeq && a.ICREQID == PHVM.ICREQID)
                                        select new VM_DGIT_ICAPPAUTHSEQ
                                        {
                                            ICAPPAUTH_ID = data.ICAPPAUTH_ID,
                                            ICREQID = data.ICREQID,
                                            ADEMPCODE = data.ADEMPCODE,
                                            APP_SEQ = data.APP_SEQ,
                                            STATUS = data.STATUS,
                                            APPTYPE = data.APPTYPE,
                                            ISPARRALLEL = data.ISPARRALLEL,
                                            APP_HEADER = data.APP_HEADER,
                                        }).ToList();
                            if (seqModel != null)
                            {
                                foreach (var _obj in seqModel)
                                    SaveICAppHis(PHVM.ADDEDBY, PHVM.ICREQID, _obj);
                            }
                        }

                        //////// Update Process Status ////////
                        DGIT_ICREQHEADER DPH = new DGIT_ICREQHEADER(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                        DPH = _ICDBContext.DGIT_ICREQHEADER.Where(x => x.ICREQID == PHVM.ICREQID).SingleOrDefault();
                        if (DPH != null)
                        {
                            if (PHVM.APPROVAL_STATUS == 1 && (seqModel == null || seqModel.Count == 0) && PHVM.APP_TYPE == 1)
                            {
                                DPH.PROCESSSTATUS = 1;
                            }
                            else if (PHVM.APPROVAL_STATUS == 1 && (seqModel == null || seqModel.Count == 0) && PHVM.APP_TYPE == 2)
                            {
                                DPH.PROCESSSTATUS = 7;
                            }
                            else if (PHVM.APPROVAL_STATUS == 1 && (seqModel != null || seqModel.Count > 0) && PHVM.APP_TYPE == 2)
                            {
                                DPH.PROCESSSTATUS = 6;
                            }
                            else if (PHVM.APPROVAL_STATUS == 1 && (seqModel != null || seqModel.Count > 0) && PHVM.APP_TYPE == 1)
                            {
                                DPH.PROCESSSTATUS = 1;
                            }
                            else if (PHVM.APPROVAL_STATUS == 2 && PHVM.APP_TYPE == 1)
                            {
                                DPH.PROCESSSTATUS = 5;
                            }
                            else if (PHVM.APPROVAL_STATUS == 2 && PHVM.APP_TYPE == 2)
                            {

                                var appseqlst = _ICDBContext.DGIT_ICAPPAUTHSEQ.Where(x => x.APP_SEQ >= PHVM.APP_SEQ);
                                foreach (var data in appseqlst)
                                {
                                    _ICDBContext.DGIT_ICAPPAUTHSEQ.Remove(data);
                                    _ICDBContext.SaveChanges();
                                }

                                DPH.PROCESSSTATUS = 2;
                            }
                            else if (PHVM.APPROVAL_STATUS == 3 && PHVM.APP_TYPE == 1)
                            {
                                DPH.PROCESSSTATUS = 3;
                            }
                            else if (PHVM.APPROVAL_STATUS == 3 && PHVM.APP_TYPE == 2)
                            {
                                var appseqlst = _ICDBContext.DGIT_ICAPPAUTHSEQ.Where(x => x.APP_SEQ >= PHVM.APP_SEQ);
                                foreach (var data in appseqlst)
                                {
                                    _ICDBContext.DGIT_ICAPPAUTHSEQ.Remove(data);
                                    _ICDBContext.SaveChanges();
                                }
                                DPH.PROCESSSTATUS = 2;
                            }

                            DPH.MODIFIEDBY = PHVM.UPDATEBY;
                            DPH.MODIFIEDDATE = DateTime.Now;
                            _ICDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _ICDBContext.SaveChanges();
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
        public short ICFinTaxApproval(VM_DGIT_ICAPPHISTORY PHVM)
        {
            short retVal = 0;
            using (var transaction = _ICDBContext.Database.BeginTransaction())
            {
                try
                {
                    DGIT_ICAPPHISTORY DPAH = new DGIT_ICAPPHISTORY();
                    if (PHVM.ICREQID > 0)
                    {
                        /////////// Update Approval Status //////////
                        // DPAH = _PoDBContext.DGIT_POAPPHISTORY.Where(x => x.POID == PHVM.POID && x.ADEMPCODE == PHVM.ADEMPCODE).FirstOrDefault();
                        DPAH = _ICDBContext.DGIT_ICAPPHISTORY.Where(x => x.ICREQID == PHVM.ICREQID && x.ADEMPCODE == 1 && x.APPROVAL_STATUS == 0 && x.APP_TYPE == 4).FirstOrDefault();
                        if (DPAH != null)
                        {
                            PHVM.APP_SEQ = DPAH.APP_SEQ;
                            DPAH.APPROVAL_STATUS = PHVM.APPROVAL_STATUS;
                            DPAH.APPROVAL_REMARK = PHVM.APPROVAL_REMARK;
                            DPAH.ADEMPCODE = (long)PHVM.UPDATEBY;
                            DPAH.UPDATEBY = PHVM.UPDATEBY;
                            DPAH.APP_DATE = DateTime.Now;
                            DPAH.UPDATEDATE = DateTime.Now;
                            _ICDBContext.Entry(DPAH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _ICDBContext.SaveChanges();
                        }
                        DGIT_ICAPPAUTHSEQ DPSQ = new DGIT_ICAPPAUTHSEQ();
                        DPSQ = _ICDBContext.DGIT_ICAPPAUTHSEQ.Where(x => x.ICREQID == PHVM.ICREQID && x.ADEMPCODE == 1 && x.APPTYPE == 4).FirstOrDefault();

                        if (DPSQ != null)
                        {
                            DPSQ.ADEMPCODE = (long)PHVM.UPDATEBY;
                            _ICDBContext.Entry(DPSQ).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _ICDBContext.SaveChanges();
                        }
                        var appcntlst = _ICDBContext.DGIT_ICAPPHISTORY.Where(x => x.ICREQID == PHVM.ICREQID && x.APP_SEQ == PHVM.APP_SEQ && x.APPROVAL_STATUS == 0);
                        if (appcntlst.Count() > 0)
                        {
                            retVal = 1;
                            transaction.Commit();
                            return retVal;
                        }
                        /// --- Save PO Approval Authority --- ///
                        List<VM_DGIT_ICAPPAUTHSEQ> seqModel = new List<VM_DGIT_ICAPPAUTHSEQ>();
                        if (PHVM.APPROVAL_STATUS == 1)
                        {
                            short nextSeq = (short)(PHVM.APP_SEQ + 1);
                            long _nextEmpAuth = _ICDBContext.DGIT_ICAPPAUTHSEQ.Where(a => a.APP_SEQ == nextSeq && a.ICREQID == PHVM.ICREQID).Select(s => s.ADEMPCODE).FirstOrDefault();
                            if (_nextEmpAuth == PHVM.ADEMPCODE)
                            {
                                nextSeq = Convert.ToInt16(nextSeq + 1);
                            }
                            seqModel = (from data in _ICDBContext.DGIT_ICAPPAUTHSEQ.Where(a => a.APP_SEQ == nextSeq && a.ICREQID == PHVM.ICREQID)
                                        select new VM_DGIT_ICAPPAUTHSEQ
                                        {
                                            ICAPPAUTH_ID = data.ICAPPAUTH_ID,
                                            ICREQID = data.ICREQID,
                                            ADEMPCODE = data.ADEMPCODE,
                                            APP_SEQ = data.APP_SEQ,
                                            STATUS = data.STATUS,
                                            APPTYPE = data.APPTYPE,
                                            ISPARRALLEL = data.ISPARRALLEL,
                                            APP_HEADER = data.APP_HEADER,
                                        }).ToList();
                            if (seqModel != null)
                            {
                                foreach (var _obj in seqModel)
                                    SaveICAppHis(PHVM.ADDEDBY, PHVM.ICREQID, _obj);
                            }
                        }

                        //////// Update Process Status ////////
                        DGIT_ICREQHEADER DPH = new DGIT_ICREQHEADER(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                        DPH = _ICDBContext.DGIT_ICREQHEADER.Where(x => x.ICREQID == PHVM.ICREQID).SingleOrDefault();
                        if (DPH != null)
                        {
                            if (PHVM.APPROVAL_STATUS == 1)
                            {
                                DPH.TAX_CREDIT_AVAIL = PHVM.TAX_CREDIT_AVAIL;
                            }
                            DPH.MODIFIEDBY = PHVM.UPDATEBY;
                            DPH.MODIFIEDDATE = DateTime.Now;
                            _ICDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _ICDBContext.SaveChanges();
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
        public Employee_Details GetAuthEmpById(long empCode, Employee_Details empDtl)
        {
            var _obj = (from data in _ICDBContext.ADEMPLOYEEs1.Where(v => v.ADEMPCODE == empCode && v.ACTIVE == 1)
                        join _VW in _ICDBContext.VW_ASSOCIATELVLDETAILS1 on data.ADEMPCODE equals _VW.ADEMPCODE
                        join _Desg in _ICDBContext.ADDESIGNATION1 on _VW.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
                        where _VW.SYKI == _Syki.SYKIID
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
        public VM_ICIBM_FIDASHBOARD_Search GetFinanceDashboard(VM_ICIBM_FIDASHBOARD_Search searchobj)
        {
            var _obj = (from data in _ICDBContext.ICIBM_FINANCEDASHBOARD
                        where (searchobj.ADEMPCODE == 0 ? true : data.ADEMPCODE == searchobj.ADEMPCODE)
                        && (string.IsNullOrEmpty(searchobj.DEPARTMENT) ? true : data.DEPARTMENT == searchobj.DEPARTMENT)
                        && (string.IsNullOrEmpty(searchobj.ENAME) ? true : data.ENAME == searchobj.ENAME)
                        && (string.IsNullOrEmpty(searchobj.ICTITLE) ? true : data.ICTITLE == searchobj.ICTITLE)
                        && (string.IsNullOrEmpty(searchobj.OPERATION) ? true : data.OPERATION == searchobj.OPERATION)
                        //&& (searchobj.LASTAPPROVAL_FROM.Year == 1 ? true : data.LASTAPPROVAL >= searchobj.LASTAPPROVAL_FROM)
                        //&& (searchobj.LASTAPPROVAL_TO.Year == 1 ? true : data.LASTAPPROVAL <= searchobj.LASTAPPROVAL_TO)
                        && searchobj.LASTAPPROVAL_FROM.HasValue ? data.LASTAPPROVAL >= searchobj.LASTAPPROVAL_FROM.Value : true
                        && searchobj.LASTAPPROVAL_TO.HasValue ? data.LASTAPPROVAL >= searchobj.LASTAPPROVAL_TO.Value : true
                        && (searchobj.status == -1 ? true : data.PROCESSSTATUS == searchobj.status)
                        select new VM_ICIBM_FINANCEDASHBOARD
                        {
                            ICTYPE = data.ICTYPE,
                            REQUESTTYPE = data.REQUESTTYPE,
                            ADEMPCODE = data.ADEMPCODE,
                            DEPARTMENT = data.DEPARTMENT,
                            ENAME = data.ENAME,
                            ICREQID = data.ICREQID,
                            ICTITLE = data.ICTITLE,
                            LASTAPPROVAL_FROM = data.LASTAPPROVAL,
                            OPERATION = data.OPERATION,
                            status = data.PROCESSSTATUS,
                            BASIC_BUDGET = data.BASIC_BUDGET,
                            BASIC_PROPOSED = data.BASIC_PROPOSED,
                            GAVAMT = data.GAVAMT,
                            NAVAMT = data.NAVAMT,
                            NPLAMT = data.NPLAMT,
                            SRVAMT = data.SRVAMT
                        }).ToList();
            searchobj.Result = _obj;
            return searchobj;
        }
        public VM_ICIBM_FITAXDASHBOARD_Search GetFinanceTaxDashboard(VM_ICIBM_FITAXDASHBOARD_Search searchobj)
        {
            var _obj = (from data in _ICDBContext.ICIBM_TAXCREDITDASHBOARD
                        where (searchobj.ADEMPCODE == 0 ? true : data.ADEMPCODE == searchobj.ADEMPCODE)
                        && (string.IsNullOrEmpty(searchobj.DEPARTMENT) ? true : data.DEPARTMENT == searchobj.DEPARTMENT)
                        && (string.IsNullOrEmpty(searchobj.ENAME) ? true : data.ENAME == searchobj.ENAME)
                        && (string.IsNullOrEmpty(searchobj.ICTITLE) ? true : data.ICTITLE == searchobj.ICTITLE)
                        && (string.IsNullOrEmpty(searchobj.OPERATION) ? true : data.OPERATION == searchobj.OPERATION)
                        //&& (searchobj.LASTAPPROVAL_FROM.Year == 1 ? true : data.LASTAPPROVAL >= searchobj.LASTAPPROVAL_FROM)
                        //&& (searchobj.LASTAPPROVAL_TO.Year == 1 ? true : data.LASTAPPROVAL <= searchobj.LASTAPPROVAL_TO)
                         && searchobj.LASTAPPROVAL_FROM.HasValue ? data.LASTAPPROVAL >= searchobj.LASTAPPROVAL_FROM.Value : true
                        && searchobj.LASTAPPROVAL_TO.HasValue ? data.LASTAPPROVAL >= searchobj.LASTAPPROVAL_TO.Value : true
                         && (searchobj.status == -1 ? true : data.STATUS == searchobj.status)
                        && (data.SYPLANTID == searchobj.PlantID)
                        select new VM_ICIBM_FINANCETAXDASHBOARD
                        {
                            ICTYPE = data.ICTYPE,
                            REQUESTTYPE = data.REQUESTTYPE,
                            ADEMPCODE = data.ADEMPCODE,
                            DEPARTMENT = data.DEPARTMENT,
                            ENAME = data.ENAME,
                            ICREQID = data.ICREQID,
                            ICTITLE = data.ICTITLE,
                            OPERATION = data.OPERATION,
                            STATUS = data.STATUS,
                            PROCESSSTATUS = data.PROCESSSTATUS
                        }).ToList();
            searchobj.Result = _obj;
            return searchobj;
        }
        public VM_ICIBM_FIAUCDASHBOARD_Search GetFinanceAUCDashboard(VM_ICIBM_FIAUCDASHBOARD_Search searchobj)
        {
            var _obj = (from data in _ICDBContext.ICIBM_FINAUCDASHBOARD
                        where (searchobj.ADEMPCODE == 0 ? true : data.ADEMPCODE == searchobj.ADEMPCODE)
                        && (string.IsNullOrEmpty(searchobj.DEPARTMENT) ? true : data.DEPARTMENT == searchobj.DEPARTMENT)
                        && (string.IsNullOrEmpty(searchobj.ENAME) ? true : data.ENAME == searchobj.ENAME)
                        && (string.IsNullOrEmpty(searchobj.ICTITLE) ? true : data.ICTITLE == searchobj.ICTITLE)
                        && (string.IsNullOrEmpty(searchobj.OPERATION) ? true : data.OPERATION == searchobj.OPERATION)
                        //&& (searchobj.LASTAPPROVAL_FROM.Year == 1 ? true : data.LASTAPPROVAL >= searchobj.LASTAPPROVAL_FROM)
                        //&& (searchobj.LASTAPPROVAL_TO.Year == 1 ? true : data.LASTAPPROVAL <= searchobj.LASTAPPROVAL_TO)
                        && searchobj.LASTAPPROVAL_FROM.HasValue ? data.LASTAPPROVAL >= searchobj.LASTAPPROVAL_FROM.Value : true
                        && searchobj.LASTAPPROVAL_TO.HasValue ? data.LASTAPPROVAL >= searchobj.LASTAPPROVAL_TO.Value : true
                         && (searchobj.status == -1 ? true : data.STATUS == searchobj.status)
                        && (data.SYPLANTID == searchobj.PlantID)
                        select new VM_ICIBM_FINAUCDASHBOARD
                        {
                            ICTYPE = data.ICTYPE,
                            REQUESTTYPE = data.REQUESTTYPE,
                            ADEMPCODE = data.ADEMPCODE,
                            DEPARTMENT = data.DEPARTMENT,
                            ENAME = data.ENAME,
                            ICREQID = data.ICREQID,
                            ICTITLE = data.ICTITLE,
                            OPERATION = data.OPERATION,
                            STATUS = data.STATUS,
                            PROCESSSTATUS = data.PROCESSSTATUS,
                            LASTAPPROVAL = data.LASTAPPROVAL
                        }).ToList();
            searchobj.Result = _obj;
            return searchobj;
        }
        public List<VM_SELECTITEMLIST> GetCycle(VM_SELECTITEMLIST obj)
        {
            DateTime datesearch = DateTime.ParseExact("01-" + DateTime.Today.ToString("MMM-yyyy"), "dd-MMM-yyyy", null);
            var _objList = (from data in _ICDBContext.DGIT_ICCONFIG_MST
                            where data.ICMONTH >= datesearch.Date
                            select data
                            ).ToList();

            List<VM_SELECTITEMLIST> finallst = new List<VM_SELECTITEMLIST>();
            foreach (var objv in _objList)
            {
                finallst.Add(new VM_SELECTITEMLIST { Text = objv.ICMONTH.ToString("MMM-yyyy") + "(" + "IC Date :" + objv.ICDATE.ToString("dd-MMM-yyyy") + ")", Value = objv.ICCONFIGID.ToString() });
            }
            return finallst;
        }
        public short ICFinanceApproval(VM_DGIT_ICAPPHISTORY PHVM)
        {
            short retVal = 0;
            using (var transaction = _ICDBContext.Database.BeginTransaction())
            {
                try
                {
                    DGIT_ICAPPHISTORY DPAH = new DGIT_ICAPPHISTORY();
                    if (PHVM.ICREQID > 0)
                    {
                        /////////// Update Approval Status //////////
                        // DPAH = _PoDBContext.DGIT_POAPPHISTORY.Where(x => x.POID == PHVM.POID && x.ADEMPCODE == PHVM.ADEMPCODE).FirstOrDefault();
                        DPAH = _ICDBContext.DGIT_ICAPPHISTORY.Where(x => x.ICREQID == PHVM.ICREQID && x.ADEMPCODE == 1 && x.APPROVAL_STATUS == 0 && x.APP_TYPE == 3).FirstOrDefault();
                        if (DPAH != null)
                        {
                            PHVM.APP_SEQ = DPAH.APP_SEQ;
                            DPAH.APPROVAL_STATUS = PHVM.APPROVAL_STATUS;
                            DPAH.APPROVAL_REMARK = PHVM.APPROVAL_REMARK;
                            DPAH.ADEMPCODE = (long)PHVM.UPDATEBY;
                            DPAH.UPDATEBY = PHVM.UPDATEBY;
                            DPAH.APP_DATE = DateTime.Now;
                            DPAH.UPDATEDATE = DateTime.Now;
                            _ICDBContext.Entry(DPAH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _ICDBContext.SaveChanges();
                        }
                        DGIT_ICAPPAUTHSEQ DPSQ = new DGIT_ICAPPAUTHSEQ();
                        DPSQ = _ICDBContext.DGIT_ICAPPAUTHSEQ.Where(x => x.ICREQID == PHVM.ICREQID && x.ADEMPCODE == 1 && x.APPTYPE == 3).FirstOrDefault();

                        if (DPSQ != null)
                        {
                            DPSQ.ADEMPCODE = (long)PHVM.UPDATEBY;
                            _ICDBContext.Entry(DPSQ).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _ICDBContext.SaveChanges();
                        }
                        /// --- Save PO Approval Authority --- ///
                        VM_DGIT_ICAPPAUTHSEQ seqModel = new VM_DGIT_ICAPPAUTHSEQ();
                        if (PHVM.APPROVAL_STATUS == 1)
                        {
                            short nextSeq = Convert.ToInt16(_ICDBContext.DGIT_ICAPPAUTHSEQ.Where(s => s.ICREQID == PHVM.ICREQID && s.ADEMPCODE == PHVM.ADEMPCODE && s.APP_SEQ == PHVM.APP_SEQ).Select(s => s.APP_SEQ).FirstOrDefault() + 1);
                            long _nextEmpAuth = _ICDBContext.DGIT_ICAPPAUTHSEQ.Where(a => a.APP_SEQ == nextSeq && a.ICREQID == PHVM.ICREQID).Select(s => s.ADEMPCODE).FirstOrDefault();
                            if (_nextEmpAuth == PHVM.ADEMPCODE)
                            {
                                nextSeq = Convert.ToInt16(nextSeq + 1);
                            }
                            seqModel = (from data in _ICDBContext.DGIT_ICAPPAUTHSEQ.Where(a => a.APP_SEQ == nextSeq && a.ICREQID == PHVM.ICREQID)
                                        select new VM_DGIT_ICAPPAUTHSEQ
                                        {
                                            ICAPPAUTH_ID = data.ICAPPAUTH_ID,
                                            ICREQID = data.ICREQID,
                                            ADEMPCODE = data.ADEMPCODE,
                                            APP_SEQ = data.APP_SEQ,
                                            STATUS = data.STATUS,
                                            APPTYPE = data.APPTYPE,
                                        }).FirstOrDefault();
                            if (seqModel != null)
                            {
                                SaveICAppHis(PHVM.ADDEDBY, PHVM.ICREQID, seqModel);
                            }
                        }

                        //////// Update Process Status ////////
                        DGIT_ICREQHEADER DPH = new DGIT_ICREQHEADER(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                        DPH = _ICDBContext.DGIT_ICREQHEADER.Where(x => x.ICREQID == PHVM.ICREQID).SingleOrDefault();
                        if (DPH != null)
                        {
                            if (PHVM.APPROVAL_STATUS == 1 && seqModel == null)
                            {
                                DPH.PROCESSSTATUS = 2;
                                DPH.ISIBMREQUIRED = PHVM.ISICIBMREQ;
                                DPH.ICCONFIGID = PHVM.ICCYCLEID;
                            }
                            else if (PHVM.APPROVAL_STATUS == 2)
                            {
                                DPH.PROCESSSTATUS = 5;
                            }
                            else if (PHVM.APPROVAL_STATUS == 3)
                            {
                                DPH.PROCESSSTATUS = 3;
                            }
                            else
                            {
                                DPH.PROCESSSTATUS = 1;
                            }
                            DPH.MODIFIEDBY = PHVM.UPDATEBY;
                            DPH.MODIFIEDDATE = DateTime.Now;
                            _ICDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _ICDBContext.SaveChanges();
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

        public short RejectICRequest(VM_DGIT_ICAPPHISTORY PHVM)
        {
            using (var transaction = _ICDBContext.Database.BeginTransaction())
            {
                try
                {
                    DGIT_ICAPPHISTORY DPAH = new DGIT_ICAPPHISTORY();
                    int FlagAdd = 0;
                    // DPAH = _PoDBContext.DGIT_POAPPHISTORY.Where(d => d.ADEMPCODE == PSVM.ADEMPCODE && d.POID == POHeaderId).FirstOrDefault();
                    //if (DPAH == null)
                    {
                        DPAH = new DGIT_ICAPPHISTORY();
                        if (_ICDBContext.DGIT_ICAPPHISTORY.Count() == 0)
                        {
                            DPAH.ICAPPHISTORY_ID = 1;
                        }
                        else
                        {
                            DPAH.ICAPPHISTORY_ID = _ICDBContext.DGIT_ICAPPHISTORY.Max(x => x.ICAPPHISTORY_ID) + 1;
                        }
                        FlagAdd = 1;
                    }
                    DPAH.ICREQID = PHVM.ICREQID;
                    DPAH.ADEMPCODE = (long)PHVM.UPDATEBY;
                    DPAH.APP_TYPE = 3;
                    DPAH.APPROVAL_STATUS = PHVM.APPROVAL_STATUS;
                    DPAH.APPROVAL_REMARK = PHVM.APPROVAL_REMARK;
                    if (FlagAdd == 1)
                    {
                        DPAH.ADDEDBY = (long)PHVM.UPDATEBY;
                        DPAH.ADDEDDATE = DateTime.Now;
                    }
                    else
                    {
                        DPAH.UPDATEBY = (long)PHVM.UPDATEBY;
                        DPAH.UPDATEDATE = DateTime.Now;
                    }
                    _ICDBContext.Entry(DPAH).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _ICDBContext.SaveChanges();
                    //////// Update Process Status ////////
                    DGIT_ICREQHEADER DPH = new DGIT_ICREQHEADER(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                    DPH = _ICDBContext.DGIT_ICREQHEADER.Where(x => x.ICREQID == PHVM.ICREQID).SingleOrDefault();
                    if (DPH != null)
                    {
                        DPH.PROCESSSTATUS = (short)(PHVM.APPROVAL_STATUS == 3 ? 9 : 5);
                        DPH.MODIFIEDBY = PHVM.UPDATEBY;
                        DPH.MODIFIEDDATE = DateTime.Today;
                        _ICDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        _ICDBContext.SaveChanges();
                    }
                    transaction.Commit();
                    return 1;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return -1;
                }
            }
        }

        public short ICApprovalInitiation(VM_ICApprovalInitiation PHVM)
        {
            short retVal = 0;
            using (var transaction = _ICDBContext.Database.BeginTransaction())
            {
                try
                {
                    #region "IC Request"
                    string[] reqid = PHVM.ICREQID.Where(m => m.ICTYPE == "1").Select(m => m.ICREQID).ToArray();
                    for (int i = 0; i < reqid.Length; i++)
                    {
                        List<VM_DGIT_ICAPPAUTHSEQ> appauthseq = new List<VM_DGIT_ICAPPAUTHSEQ>();
                        if (reqid[i] == "") { continue; }
                        long ICREQID = Convert.ToInt64(reqid[i]);
                        short appseq = _ICDBContext.DGIT_ICAPPAUTHSEQ.Where(m => m.ICREQID == ICREQID).Max(m => m.APP_SEQ);
                        //appseq = (short)(appseq + 1);
                        foreach (var _app in PHVM.ICAPPAUTHSEQ)
                        {
                            VM_DGIT_ICAPPAUTHSEQ objapps = new VM_DGIT_ICAPPAUTHSEQ();
                            objapps.ADDEDBY = (long)PHVM.UPDATEDBY;
                            objapps.ADEMPCODE = _app.ADEMPCODE;
                            objapps.APPTYPE = _app.APPTYPE;
                            objapps.APP_HEADER = _app.APP_HEADER;
                            objapps.APP_SEQ = (short)(_app.APP_SEQ + appseq);
                            objapps.ISPARRALLEL = _app.ISPARRALLEL;
                            objapps.STATUS = _app.STATUS;
                            objapps.ICREQID = ICREQID;
                            appauthseq.Add(objapps);
                        }
                        SaveAppAuthSeq_WD((long)PHVM.UPDATEDBY, ICREQID, appauthseq);
                        long apphseq = appauthseq.OrderBy(m => m.APP_SEQ).FirstOrDefault().APP_SEQ;
                        var oAHLst = appauthseq.Where(m => m.APP_SEQ == apphseq).ToList();
                        foreach (var _objapp in oAHLst)
                        {
                            SaveICAppHis((long)PHVM.UPDATEDBY, ICREQID, _objapp);
                        }
                        //////// Update Process Status ////////
                        DGIT_ICREQHEADER DPH = new DGIT_ICREQHEADER(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                        DPH = _ICDBContext.DGIT_ICREQHEADER.Where(x => x.ICREQID == ICREQID).SingleOrDefault();
                        if (DPH != null)
                        {
                            DPH.PROCESSSTATUS = 6;
                            DPH.MODIFIEDBY = PHVM.UPDATEDBY;
                            DPH.MODIFIEDDATE = DateTime.Today;
                            _ICDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _ICDBContext.SaveChanges();
                        }
                    }
                    #endregion

                    #region "Asset Disposal IC Request"
                    reqid = PHVM.ICREQID.Where(m => m.ICTYPE == "2").Select(m => m.ICREQID).ToArray();
                    for (int i = 0; i < reqid.Length; i++)
                    {
                        List<VM_DGIT_ICAPPAUTHSEQ> appauthseq = new List<VM_DGIT_ICAPPAUTHSEQ>();
                        if (reqid[i] == "") { continue; }
                        long ICREQID = Convert.ToInt64(reqid[i]);
                        short appseq = _ICDBContext.DGIT_ICASSETAPPAUTHSEQ.Where(m => m.ICASSETREQID == ICREQID).Max(m => m.APP_SEQ);
                        //appseq = (short)(appseq + 1);
                        foreach (var _app in PHVM.ICAPPAUTHSEQ)
                        {
                            VM_DGIT_ICAPPAUTHSEQ objapps = new VM_DGIT_ICAPPAUTHSEQ();
                            objapps.ADDEDBY = (long)PHVM.UPDATEDBY;
                            objapps.ADEMPCODE = _app.ADEMPCODE;
                            objapps.APPTYPE = _app.APPTYPE;
                            objapps.APP_HEADER = _app.APP_HEADER;
                            objapps.APP_SEQ = (short)(_app.APP_SEQ + appseq);
                            objapps.ISPARRALLEL = _app.ISPARRALLEL;
                            objapps.STATUS = _app.STATUS;
                            objapps.ICREQID = ICREQID;
                            appauthseq.Add(objapps);
                        }
                        SaveAssetAppAuthSeq_WD((long)PHVM.UPDATEDBY, ICREQID, appauthseq);
                        long apphseq = appauthseq.OrderBy(m => m.APP_SEQ).FirstOrDefault().APP_SEQ;
                        var oAHLst = appauthseq.Where(m => m.APP_SEQ == apphseq).ToList();
                        foreach (var _objapp in oAHLst)
                        {
                            SaveICAssetAppHis((long)PHVM.UPDATEDBY, ICREQID, _objapp);
                        }
                        //////// Update Process Status ////////
                        DGIT_ICASSETREQHEADER DPH = new DGIT_ICASSETREQHEADER(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                        DPH = _ICDBContext.DGIT_ICASSETREQHEADER.Where(x => x.ICASSETREQID == ICREQID).SingleOrDefault();
                        if (DPH != null)
                        {
                            DPH.PROCESSSTATUS = 6;
                            DPH.MODIFIEDBY = PHVM.UPDATEDBY;
                            DPH.MODIFIEDDATE = DateTime.Today;
                            _ICDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _ICDBContext.SaveChanges();
                        }
                    }
                    #endregion

                    transaction.Commit();
                    retVal = 1;

                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
            }
            return retVal;
        }
        public void SaveAppAuthSeq_WD(long AddedBy, long ICHeaderId, List<VM_DGIT_ICAPPAUTHSEQ> PSVMList)
        {
            foreach (VM_DGIT_ICAPPAUTHSEQ PSVM in PSVMList.OrderBy(o => o.APP_SEQ).ToList())
            {
                DGIT_ICAPPAUTHSEQ DAAS = new DGIT_ICAPPAUTHSEQ();
                if (_ICDBContext.DGIT_ICAPPAUTHSEQ.Count() == 0)
                {
                    DAAS.ICAPPAUTH_ID = 1;
                }
                else
                {
                    DAAS.ICAPPAUTH_ID = _ICDBContext.DGIT_ICAPPAUTHSEQ.Max(x => x.ICAPPAUTH_ID) + 1;
                }
                DAAS.ICREQID = ICHeaderId;
                DAAS.ADEMPCODE = PSVM.ADEMPCODE;
                DAAS.APP_SEQ = PSVM.APP_SEQ;
                DAAS.APPTYPE = PSVM.APPTYPE;
                DAAS.STATUS = 1;
                DAAS.ADDEDBY = AddedBy;
                DAAS.ADDEDDATE = DateTime.Now;
                DAAS.APP_HEADER = PSVM.APP_HEADER;
                _ICDBContext.Entry(DAAS).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                _ICDBContext.SaveChanges();
            }
        }

        public List<VM_DGIT_ICAPPAUTHSEQ> GetICAuthority()
        {

            var objlist = (from data in _ICDBContext.DGIT_ICMEMBER_MST
                           join emp in _ICDBContext.ADEMPLOYEEs1 on data.ADEMPCODE equals emp.ADEMPCODE
                           join vw in _ICDBContext.VW_ASSOCIATELVLDETAILS1 on emp.ADEMPCODE equals vw.ADEMPCODE
                           join desg in _ICDBContext.ADDESIGNATION1 on vw.ADDESIGNATIONID equals desg.ADDESIGNATIONID
                           where data.STATUS == 1
                           && vw.SYKI == _Syki.SYKIID
                           select new VM_DGIT_ICAPPAUTHSEQ
                           {
                               ADDESIGNATION = (string.IsNullOrEmpty(vw.FUNCTIONALDESIGNATION) ? desg.DESCRIP : vw.FUNCTIONALDESIGNATION),
                               ADEMPCODE = data.ADEMPCODE,
                               ADEMPNAME = emp.FIRSTNAME + " " + emp.LASTNAME,
                               APPTYPE = data.APPTYPE,
                               ISPARRALLEL = data.ISPARRALLEL,
                               APP_HEADER = data.APPHEADER,
                               APP_SEQ = data.APP_SEQ
                           }).ToList();
            return objlist;
        }

        public Tuple<short, long> UpdateAUCCode(ICREQHEADER model)
        {
            short retVal = 0; long retHeaderId = 0;
            Tuple<short, long> _retVal_tuple;
            using (var transaction = _ICDBContext.Database.BeginTransaction())
            {
                try
                {
                    DGIT_ICREQHEADER DPH = new DGIT_ICREQHEADER();
                    int FlagAdd = 0;
                    if (model.ICREQID > 0)
                    {
                        DPH = _ICDBContext.DGIT_ICREQHEADER.Where(x => x.ICREQID == model.ICREQID).SingleOrDefault();
                    }
                    else
                    {
                        _retVal_tuple = new Tuple<short, long>(-1, retHeaderId);
                        return _retVal_tuple;
                    }

                    if (DPH == null)
                    {
                        _retVal_tuple = new Tuple<short, long>(-1, 0);
                        return _retVal_tuple;
                    }
                    DPH.AUCCODE = model.AUCCODE;
                    DPH.AUCCODEUPDBY = model.AUCCODEUPDBY;
                    DPH.AUCCODEUPDDATE = DateTime.Now;
                    DPH.AUCCODEREMARK = model.AUCCODEREMARK;
                    _ICDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _ICDBContext.SaveChanges();

                    transaction.Commit();
                    retVal = 1;
                    retHeaderId = DPH.ICREQID;
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

        //Asset Disposal 

        public Tuple<short, long> SaveAssetICRequest(ICASSETDISREQHEADER model)
        {
            short retVal = 0; long retHeaderId = 0;
            Tuple<short, long> _retVal_tuple;
            using (var transaction = _ICDBContext.Database.BeginTransaction())
            {
                try
                {
                    DGIT_ICASSETREQHEADER DPH = new DGIT_ICASSETREQHEADER();
                    int FlagAdd = 0;
                    if (model.ICREQID > 0)
                    {
                        DPH = _ICDBContext.DGIT_ICASSETREQHEADER.Where(x => x.ICASSETREQID == model.ICREQID).SingleOrDefault();
                    }
                    else
                    {
                        //if (_ICDBContext.DGIT_ICREQHEADER.Any(x => x.POHEADERID != model.POHEADERID && x.PONO == model.PONO && (x.PROCESS_STATUS == 1 || x.PROCESS_STATUS == 0)))
                        //{
                        //    retVal = 2;
                        //    return _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId); //// -- record already exist.
                        //}

                        DPH = new DGIT_ICASSETREQHEADER();
                        if (_ICDBContext.DGIT_ICASSETREQHEADER.Count() == 0)
                        {
                            DPH.ICASSETREQID = 1;
                        }
                        else
                        {
                            DPH.ICASSETREQID = _ICDBContext.DGIT_ICASSETREQHEADER.Max(x => x.ICASSETREQID) + 1;
                        }
                        FlagAdd = 1;
                    }
                    if (model.IsFinalSubmit == 0)
                    {
                        DPH.ADDEDBY = model.ADDEDBY;
                        DPH.BASIC_BUDGET = model.BASIC_BUDGET;
                        DPH.BASIC_PROPOSED = model.BASIC_PROPOSED;
                        DPH.CONTENTOFEXE = model.CONTENTOFEXE;
                        DPH.DATEADDED = DateTime.Today;
                        DPH.DEPARTMENT = model.DEPARTMENT;
                        DPH.ICA00 = model.ICA00;
                        DPH.ICBACKGROUND = model.ICBACKGROUND;
                        DPH.ICPURPOSE = model.ICPURPOSE;
                        DPH.ICTITLE = model.ICTITLE;
                        DPH.REQUIREMENT = model.REQUIREMENT;
                        DPH.TARGET = model.TARGET;
                        DPH.TAX_BUDGET = model.TAX_BUDGET;
                        DPH.TAX_PROPOSED = model.TAX_PROPOSED;
                    }
                    else
                    {
                        //if (!_ICDBContext.DGIT_PODETAIL.Any(x => x.POHEADERID == model.POHEADERID && x.DOC_TYPE == "PO"))
                        //{
                        //    retVal = 3;
                        //    return _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId); //// -- PO file not exist.
                        //}
                    }
                    DPH.PROCESSSTATUS = model.PROCESSSTATUS;
                    DPH.STATUS = model.STATUS;
                    DPH.REMARK = model.REMARK;
                    if (FlagAdd == 1)
                    {
                        DPH.ADDEDBY = model.ADDEDBY;
                        DPH.DATEADDED = DateTime.Now;
                    }
                    else
                    {
                        DPH.MODIFIEDBY = model.MODIFIEDBY;
                        DPH.MODIFIEDDATE = DateTime.Now;
                    }
                    _ICDBContext.Entry(DPH).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _ICDBContext.SaveChanges();

                    /// --- Save PO Detail --- ///
                    if (model.ICDOCDETAIL != null)
                    {
                        if (model.ICDOCDETAIL.Count > 0)
                        {
                            SaveICAssetDetails(model.ADDEDBY, DPH.ICASSETREQID, model.ICDOCDETAIL);
                        }
                    }
                    if (model.ICREQ_SCHEDULE != null && model.ICREQ_SCHEDULE.Count > 0)
                    {
                        SaveAssetMilestoneSchdule(model.ADDEDBY, DPH.ICASSETREQID, model.ICREQ_SCHEDULE);
                    }
                    /// --- Save PO Approval Auth Seq ---///
                    if (model.ICAPPAUTHSEQ != null)
                    {
                        if (model.ICAPPAUTHSEQ.Count > 0)
                        {
                            if (model.ICAPPAUTHSEQ.Count > 0)
                            {
                                model.ICAPPAUTHSEQ.Add(new VM_DGIT_ICAPPAUTHSEQ
                                {
                                    ADEMPCODE = 1,
                                    ADEMPNAME = "IC/IBM Admin Approval",
                                    ADDESIGNATION = "",
                                    APP_SEQ = (model.ICAPPAUTHSEQ.Count == 0 ? (short)1 : Convert.ToInt16(model.ICAPPAUTHSEQ.Max(x => x.APP_SEQ) + 1)),
                                    APPTYPE = 3,
                                });
                            }
                            SaveAssetAppAuthSeq(model.ADDEDBY, DPH.ICASSETREQID, model.ICAPPAUTHSEQ);

                            /// --- Save PO Approval Authority --- ///
                            VM_DGIT_ICAPPAUTHSEQ seqModel = model.ICAPPAUTHSEQ.OrderBy(o => o.APP_SEQ).FirstOrDefault();
                            if (seqModel != null)
                            {
                                SaveICAssetAppHis(model.ADDEDBY, DPH.ICASSETREQID, seqModel);
                            }
                        }
                    }

                    transaction.Commit();
                    retVal = 1;
                    retHeaderId = DPH.ICASSETREQID;
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

        public short SaveICAssetDetails(long AddedBy, long ICASSETHeaderId, List<VM_DGIT_ICDOCDETAIL> PDVMList)
        {
            short retVal = 0;
            foreach (VM_DGIT_ICDOCDETAIL PDVM in PDVMList)
            {
                DGIT_ICASSETDOCDETAIL DPD = new DGIT_ICASSETDOCDETAIL();
                int FlagAdd = 0;
                if (ICASSETHeaderId > 0)
                {
                    //DPD = _PoDBContext.DGIT_PODETAIL.Where(x => x.POHEADERID == POHeaderId && x.DOC_TYPE == PDVM.DOC_TYPE && x.FILENAME == PDVM.FILENAME).SingleOrDefault();
                    //if (DPD != null)
                    //{
                    //    _PoDBContext.DGIT_PODETAIL.Remove(DPD);
                    //    _PoDBContext.SaveChanges();
                    //}

                    //if (PDVM.DOC_TYPE == "ICDOC")
                    //{
                    //    DPD = _ICDBContext.DGIT_ICASSETDOCDETAIL.Where(x => x.ICASSETREQID == ICASSETHeaderId && x.DOC_TYPE == PDVM.DOC_TYPE).FirstOrDefault();
                    //    if (DPD != null)
                    //    {
                    //        _ICDBContext.DGIT_ICASSETDOCDETAIL.Remove(DPD);
                    //        _ICDBContext.SaveChanges();
                    //    }
                    //}

                    DPD = new DGIT_ICASSETDOCDETAIL();
                    if (_ICDBContext.DGIT_ICASSETDOCDETAIL.Count() == 0)
                    {
                        DPD.ICASSETDOC_ID = 1;
                    }
                    else
                    {
                        DPD.ICASSETDOC_ID = _ICDBContext.DGIT_ICASSETDOCDETAIL.Max(x => x.ICASSETDOC_ID) + 1;
                    }
                    FlagAdd = 1;

                    DPD.ICASSETREQID = ICASSETHeaderId;
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
                    _ICDBContext.Entry(DPD).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _ICDBContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }
        public short SaveAssetMilestoneSchdule(long AddedBy, long ICAssetHeaderId, List<VM_DGIT_ICREQ_SCHEDULE> PDVMList)
        {
            short retVal = 0;
            List<DGIT_ICASSETREQ_SCHEDULE> SeqList = _ICDBContext.DGIT_ICASSETREQ_SCHEDULE.Where(t => t.ICASSETREQID == ICAssetHeaderId).ToList();
            if (SeqList.Count > 0)
            {
                _ICDBContext.DGIT_ICASSETREQ_SCHEDULE.RemoveRange(SeqList);
                _ICDBContext.SaveChanges();
            }
            foreach (VM_DGIT_ICREQ_SCHEDULE PDVM in PDVMList)
            {
                DGIT_ICASSETREQ_SCHEDULE DPD = new DGIT_ICASSETREQ_SCHEDULE();
                int FlagAdd = 0;
                if (ICAssetHeaderId > 0)
                {

                    DPD = new DGIT_ICASSETREQ_SCHEDULE();
                    if (_ICDBContext.DGIT_ICASSETREQ_SCHEDULE.Count() == 0)
                    {
                        DPD.ICASSETREQSCHID = 1;
                    }
                    else
                    {
                        DPD.ICASSETREQSCHID = _ICDBContext.DGIT_ICASSETREQ_SCHEDULE.Max(x => x.ICASSETREQSCHID) + 1;
                    }
                    FlagAdd = 1;

                    DPD.ICASSETREQID = ICAssetHeaderId;
                    DPD.TARGET = PDVM.TARGET;
                    DPD.SCHEDULE = PDVM.SCHEDULE;
                    if (FlagAdd == 1)
                    {
                        DPD.ADDEDBY = AddedBy;
                        DPD.DATEADDED = DateTime.Now;
                    }
                    else
                    {
                        DPD.MODIFIEDBY = AddedBy;
                        DPD.MODIFIEDDATE = DateTime.Now;
                    }
                    _ICDBContext.Entry(DPD).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _ICDBContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }
        public void SaveAssetAppAuthSeq(long AddedBy, long ICAssetHeaderId, List<VM_DGIT_ICAPPAUTHSEQ> PSVMList)
        {
            //// --- Delete recode ---////
            List<DGIT_ICASSETAPPAUTHSEQ> SeqList = _ICDBContext.DGIT_ICASSETAPPAUTHSEQ.Where(t => t.ICASSETREQID == ICAssetHeaderId).ToList();
            if (SeqList.Count > 0)
            {
                _ICDBContext.DGIT_ICASSETAPPAUTHSEQ.RemoveRange(SeqList);
                _ICDBContext.SaveChanges();
            }

            foreach (VM_DGIT_ICAPPAUTHSEQ PSVM in PSVMList.OrderBy(o => o.APP_SEQ).ToList())
            {
                DGIT_ICASSETAPPAUTHSEQ DAAS = new DGIT_ICASSETAPPAUTHSEQ();
                if (_ICDBContext.DGIT_ICASSETAPPAUTHSEQ.Count() == 0)
                {
                    DAAS.ICASSETAPPAUTH_ID = 1;
                }
                else
                {
                    DAAS.ICASSETAPPAUTH_ID = _ICDBContext.DGIT_ICASSETAPPAUTHSEQ.Max(x => x.ICASSETAPPAUTH_ID) + 1;
                }
                DAAS.ICASSETREQID = ICAssetHeaderId;
                DAAS.ADEMPCODE = PSVM.ADEMPCODE;
                DAAS.APP_SEQ = PSVM.APP_SEQ;
                DAAS.APPTYPE = PSVM.APPTYPE;
                DAAS.STATUS = 1;
                DAAS.ADDEDBY = AddedBy;
                DAAS.ADDEDDATE = DateTime.Now;
                _ICDBContext.Entry(DAAS).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                _ICDBContext.SaveChanges();
            }
        }

        public void SaveICAssetAppHis(long AddedBy, long ICAssetHeaderId, VM_DGIT_ICAPPAUTHSEQ PSVM)
        {
            DGIT_ICASSETAPPHISTORY DPAH = new DGIT_ICASSETAPPHISTORY();
            int FlagAdd = 0;
            // DPAH = _PoDBContext.DGIT_POAPPHISTORY.Where(d => d.ADEMPCODE == PSVM.ADEMPCODE && d.POID == POHeaderId).FirstOrDefault();
            DPAH = _ICDBContext.DGIT_ICASSETAPPHISTORY.Where(d => d.ADEMPCODE == PSVM.ADEMPCODE && d.ICASSETREQID == ICAssetHeaderId && d.APPROVAL_STATUS == 0).FirstOrDefault();
            if (DPAH == null)
            {
                DPAH = new DGIT_ICASSETAPPHISTORY();
                if (_ICDBContext.DGIT_ICASSETAPPHISTORY.Count() == 0)
                {
                    DPAH.ICASSETAPPHISTORY_ID = 1;
                }
                else
                {
                    DPAH.ICASSETAPPHISTORY_ID = _ICDBContext.DGIT_ICASSETAPPHISTORY.Max(x => x.ICASSETAPPHISTORY_ID) + 1;
                }
                FlagAdd = 1;
            }
            DPAH.ICASSETREQID = ICAssetHeaderId;
            DPAH.ADEMPCODE = PSVM.ADEMPCODE;
            DPAH.APP_TYPE = PSVM.APPTYPE;
            DPAH.APPROVAL_STATUS = 0;
            DPAH.APP_SEQ = PSVM.APP_SEQ;
            DPAH.APPROVAL_REMARK = "";
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
            _ICDBContext.Entry(DPAH).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
            _ICDBContext.SaveChanges();
        }

        public List<VM_DGIT_ICDOCDETAIL> GetAssetAttachmentDetail(long _ICHEADERID)
        {
            return (from _ICDetail in _ICDBContext.DGIT_ICASSETDOCDETAIL.Where(d => d.ICASSETREQID == _ICHEADERID)
                    where _ICDetail.STATUS == 1
                    select new VM_DGIT_ICDOCDETAIL
                    {
                        ICDOC_ID = _ICDetail.ICASSETDOC_ID,
                        ICREQID = _ICDetail.ICASSETREQID,
                        DOC_TYPE = _ICDetail.DOC_TYPE,
                        ADDITIONAL_INFO = _ICDetail.ADDITIONAL_INFO,
                        FILENAME = _ICDetail.FILENAME,
                    }).ToList();
        }
        public short DeleteAssetAttachment(string fileName, string docType, long ICHeaderId)
        {
            short retVal = 0;
            if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(docType) && ICHeaderId > 0)
            {
                DGIT_ICASSETDOCDETAIL DT = _ICDBContext.DGIT_ICASSETDOCDETAIL.Where(x => x.FILENAME == fileName && x.DOC_TYPE == docType && x.ICASSETREQID == ICHeaderId).FirstOrDefault();
                if (DT != null)
                {
                    _ICDBContext.DGIT_ICASSETDOCDETAIL.Remove(DT);
                    _ICDBContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }

        public ICASSETDISREQHEADER GetICAssetRequestById(long id)
        {
            //var _obj = (from data in _ICDBContext.DGIT_ICASSETREQHEADER.Where(x => x.ICASSETREQID == id)
            //            join _AddBy in _ICDBContext.ADEMPLOYEEs1 on data.ADDEDBY equals _AddBy.ADEMPCODE
            //            join _VWAssociate in _ICDBContext.VW_ASSOCIATELVLDETAILS1 on data.ADDEDBY equals _VWAssociate.ADEMPCODE
            //            join _cy in _ICDBContext.DGIT_ICCONFIG_MST on data.ICCONFIGID equals _cy.ICCONFIGID into _cyl
            //            from _cycle in _cyl.DefaultIfEmpty()
            //            let syki = _ICDBContext.VW_ASSOCIATELVLDETAILS1.Where(x => x.ADEMPCODE == data.ADDEDBY).Max(x => x.SYKI) //SR97493 - Getting last active KI of initiator
            //            //where _VWAssociate.SYKI == _Syki.SYKIID
            //            where _VWAssociate.SYKI == syki //SR97493 - Getting last active KI of initiator
            //            select new ICASSETDISREQHEADER
            //            {
            //                ADDEDBY = data.ADDEDBY,
            //                BASIC_BUDGET = data.BASIC_BUDGET,
            //                BASIC_PROPOSED = data.BASIC_PROPOSED,
            //                ICREQID = data.ICASSETREQID,
            //                CONTENTOFEXE = data.CONTENTOFEXE,
            //                DEPARTMENT = data.DEPARTMENT,
            //                ICA00 = data.ICA00,
            //                ICBACKGROUND = data.ICBACKGROUND,
            //                ICPURPOSE = data.ICPURPOSE,
            //                ICTITLE = data.ICTITLE,
            //                REQUIREMENT = data.REQUIREMENT,
            //                STATUS = data.STATUS,
            //                TARGET = data.TARGET,
            //                TAX_BUDGET = data.TAX_BUDGET,
            //                TAX_PROPOSED = data.TAX_PROPOSED,
            //                REMARK = data.REMARK,
            //                ADDEDBYNAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
            //                DATEADDED = data.DATEADDED,
            //                PROCESSSTATUS = data.PROCESSSTATUS,
            //                iccycle = _cycle.ICMONTH,
            //                ISICIBMREQ = (data.ISIBMREQUIRED == 1 ? "Yes" : "No"),
            //                ICDOCDETAIL = (from _ICDetail in _ICDBContext.DGIT_ICASSETDOCDETAIL.Where(d => d.ICASSETREQID == data.ICASSETREQID)
            //                               where _ICDetail.STATUS == 1
            //                               select new VM_DGIT_ICDOCDETAIL
            //                               {
            //                                   ICDOC_ID = _ICDetail.ICASSETDOC_ID,
            //                                   ICREQID = _ICDetail.ICASSETREQID,
            //                                   DOC_TYPE = _ICDetail.DOC_TYPE,
            //                                   ADDITIONAL_INFO = _ICDetail.ADDITIONAL_INFO,
            //                                   FILENAME = _ICDetail.FILENAME,
            //                               }).ToList(),
            //                ICREQ_SCHEDULE = (from _ICSch in _ICDBContext.DGIT_ICASSETREQ_SCHEDULE.Where(l => l.ICASSETREQID == data.ICASSETREQID)
            //                                  select new VM_DGIT_ICREQ_SCHEDULE
            //                                  {
            //                                      ICREQID = _ICSch.ICASSETREQID,
            //                                      ICREQSCHID = _ICSch.ICASSETREQSCHID,
            //                                      SCHEDULE = _ICSch.SCHEDULE,
            //                                      TARGET = _ICSch.TARGET,
            //                                      //SCHEDULESTR= string.Format("dd-MMM-yyyy",_ICSch.SCHEDULE.ToString("dd-MMM-yyyy")),
            //                                  }).OrderBy(l => l.ICREQSCHID).ToList(),
            //                Emp_Detail = new Employee_Details
            //                {
            //                    _ECode = _AddBy.ADEMPCODE,
            //                    _EName = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
            //                    _EmailId = _AddBy.EMAILID,
            //                    _DOB = DateTime.Now,
            //                    _SecDescrip = _VWAssociate.SECTION,
            //                    _DepDesc = _VWAssociate.DEPARTMENT,
            //                    _DivDesc = _VWAssociate.DIVISION,
            //                    _OpDesc = _VWAssociate.OPERATION,
            //                    _SiteId = _VWAssociate.SYSITEID
            //                },
            //                ICAPPAUTHSEQ = (from _ICSeq in _ICDBContext.DGIT_ICASSETAPPAUTHSEQ.Where(x => x.ICASSETREQID == data.ICASSETREQID)
            //                                join _AppSeqEmp in _ICDBContext.ADEMPLOYEEs1 on _ICSeq.ADEMPCODE equals _AppSeqEmp.ADEMPCODE
            //                                join _Vw in _ICDBContext.VW_ASSOCIATELVLDETAILS1 on _ICSeq.ADEMPCODE equals _Vw.ADEMPCODE
            //                                join _Desg in _ICDBContext.ADDESIGNATION1 on _Vw.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
            //                                where _Vw.SYKI == _Syki.SYKIID
            //                                select new VM_DGIT_ICAPPAUTHSEQ
            //                                {
            //                                    ICREQID = _ICSeq.ICASSETREQID,
            //                                    ICAPPAUTH_ID = _ICSeq.ICASSETAPPAUTH_ID,
            //                                    ADEMPCODE = _ICSeq.ADEMPCODE,
            //                                    STATUS = _ICSeq.STATUS,
            //                                    APP_SEQ = _ICSeq.APP_SEQ,
            //                                    ADEMPNAME = _AppSeqEmp.FIRSTNAME + " " + _AppSeqEmp.LASTNAME,
            //                                    ADDESIGNATION = _Desg.DESCRIP,
            //                                    ADDEDDATE = _ICSeq.ADDEDDATE,
            //                                    UPDATEBY = _ICSeq.UPDATEBY,
            //                                    UPDATEDATE = _ICSeq.UPDATEDATE,
            //                                    APPTYPE = _ICSeq.APPTYPE,
            //                                }).OrderBy(b => b.APP_SEQ).ToList(),
            //                ICAPPHISTORY = (from _POAppHis in _ICDBContext.DGIT_ICASSETAPPHISTORY.Where(x => x.ICASSETREQID == data.ICASSETREQID)
            //                                join _AppEmp in _ICDBContext.ADEMPLOYEEs1 on _POAppHis.ADEMPCODE equals _AppEmp.ADEMPCODE
            //                                select new VM_DGIT_ICAPPHISTORY
            //                                {
            //                                    ICAPPHISTORY_ID = _POAppHis.ICASSETAPPHISTORY_ID,
            //                                    ICREQID = _POAppHis.ICASSETREQID,
            //                                    ADEMPCODE = _POAppHis.ADEMPCODE,
            //                                    APPROVAL_STATUS = _POAppHis.APPROVAL_STATUS,
            //                                    APPROVAL_REMARK = _POAppHis.APPROVAL_REMARK,
            //                                    APPEMP_NAME = _AppEmp.FIRSTNAME + " " + _AppEmp.LASTNAME + " - [" + _AppEmp.ADEMPCODE + "]",
            //                                    APP_EMAIL = _AppEmp.EMAILID,
            //                                    APPEMP_CODE = _AppEmp.ADEMPCODE.ToString(),
            //                                    ADDEDDATE = _POAppHis.ADDEDDATE,
            //                                    UPDATEBY = _POAppHis.UPDATEBY,
            //                                    UPDATEDATE = _POAppHis.UPDATEDATE,
            //                                    APP_DATE = _POAppHis.APP_DATE,
            //                                    APP_TYPE = _POAppHis.APP_TYPE,
            //                                }).OrderBy(o => o.ICAPPHISTORY_ID).ToList(),

            //            }).FirstOrDefault();


            var _obj = (from data in _ICDBContext.DGIT_ICASSETREQHEADER.Where(x => x.ICASSETREQID == id)
                        join _AddBy in _ICDBContext.ADEMPLOYEEs1 on data.ADDEDBY equals _AddBy.ADEMPCODE
                        join _VWAssociate in _ICDBContext.VW_ASSOCIATELVLDETAILS1 on data.ADDEDBY equals _VWAssociate.ADEMPCODE
                        join _cy in _ICDBContext.DGIT_ICCONFIG_MST on data.ICCONFIGID equals _cy.ICCONFIGID into _cyl
                        from _cycle in _cyl.DefaultIfEmpty()
                        let syki = _ICDBContext.VW_ASSOCIATELVLDETAILS1.Where(x => x.ADEMPCODE == data.ADDEDBY).Max(x => x.SYKI) //SR97493 - Getting last active KI of initiator
                        //where _VWAssociate.SYKI == _Syki.SYKIID
                        where _VWAssociate.SYKI == syki //SR97493 - Getting last active KI of initiator
                        select new ICASSETDISREQHEADER
                        {
                            ADDEDBY = data.ADDEDBY,
                            BASIC_BUDGET = data.BASIC_BUDGET,
                            BASIC_PROPOSED = data.BASIC_PROPOSED,
                            ICREQID = data.ICASSETREQID,
                            CONTENTOFEXE = data.CONTENTOFEXE,
                            DEPARTMENT = data.DEPARTMENT,
                            ICA00 = data.ICA00,
                            ICBACKGROUND = data.ICBACKGROUND,
                            ICPURPOSE = data.ICPURPOSE,
                            ICTITLE = data.ICTITLE,
                            REQUIREMENT = data.REQUIREMENT,
                            STATUS = data.STATUS,
                            TARGET = data.TARGET,
                            TAX_BUDGET = data.TAX_BUDGET,
                            TAX_PROPOSED = data.TAX_PROPOSED,
                            REMARK = data.REMARK,
                            ADDEDBYNAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                            DATEADDED = data.DATEADDED,
                            PROCESSSTATUS = data.PROCESSSTATUS,
                            iccycle = _cycle.ICMONTH,
                            ISICIBMREQ = (data.ISIBMREQUIRED == 1 ? "Yes" : "No"),
                            Emp_Detail = new Employee_Details
                            {
                                _ECode = _AddBy.ADEMPCODE,
                                _EName = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                                _EmailId = _AddBy.EMAILID,
                                _DOB = DateTime.Now,
                                _SecDescrip = _VWAssociate.SECTION,
                                _DepDesc = _VWAssociate.DEPARTMENT,
                                _DivDesc = _VWAssociate.DIVISION,
                                _OpDesc = _VWAssociate.OPERATION,
                                _SiteId = _VWAssociate.SYSITEID
                            },
                        }).FirstOrDefault();

            if (_obj != null) {
                _obj.ICDOCDETAIL = (from _ICDetail in _ICDBContext.DGIT_ICASSETDOCDETAIL.Where(d => d.ICASSETREQID == _obj.ICREQID)
                                    where _ICDetail.STATUS == 1
                                    select new VM_DGIT_ICDOCDETAIL
                                    {
                                        ICDOC_ID = _ICDetail.ICASSETDOC_ID,
                                        ICREQID = _ICDetail.ICASSETREQID,
                                        DOC_TYPE = _ICDetail.DOC_TYPE,
                                        ADDITIONAL_INFO = _ICDetail.ADDITIONAL_INFO,
                                        FILENAME = _ICDetail.FILENAME,
                                    }).ToList();

                _obj.ICREQ_SCHEDULE = (from _ICSch in _ICDBContext.DGIT_ICASSETREQ_SCHEDULE.Where(l => l.ICASSETREQID == _obj.ICREQID)
                                       select new VM_DGIT_ICREQ_SCHEDULE
                                       {
                                           ICREQID = _ICSch.ICASSETREQID,
                                           ICREQSCHID = _ICSch.ICASSETREQSCHID,
                                           SCHEDULE = _ICSch.SCHEDULE,
                                           TARGET = _ICSch.TARGET,
                                           //SCHEDULESTR= string.Format("dd-MMM-yyyy",_ICSch.SCHEDULE.ToString("dd-MMM-yyyy")),
                                       }).OrderBy(l => l.ICREQSCHID).ToList();

                _obj.ICAPPAUTHSEQ = (from _ICSeq in _ICDBContext.DGIT_ICASSETAPPAUTHSEQ.Where(x => x.ICASSETREQID == _obj.ICREQID)
                                     join _AppSeqEmp in _ICDBContext.ADEMPLOYEEs1 on _ICSeq.ADEMPCODE equals _AppSeqEmp.ADEMPCODE
                                     join _Vw in _ICDBContext.VW_ASSOCIATELVLDETAILS1 on _ICSeq.ADEMPCODE equals _Vw.ADEMPCODE
                                     join _Desg in _ICDBContext.ADDESIGNATION1 on _Vw.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
                                     where _Vw.SYKI == _Syki.SYKIID
                                     select new VM_DGIT_ICAPPAUTHSEQ
                                     {
                                         ICREQID = _ICSeq.ICASSETREQID,
                                         ICAPPAUTH_ID = _ICSeq.ICASSETAPPAUTH_ID,
                                         ADEMPCODE = _ICSeq.ADEMPCODE,
                                         STATUS = _ICSeq.STATUS,
                                         APP_SEQ = _ICSeq.APP_SEQ,
                                         ADEMPNAME = _AppSeqEmp.FIRSTNAME + " " + _AppSeqEmp.LASTNAME,
                                         ADDESIGNATION = _Desg.DESCRIP,
                                         ADDEDDATE = _ICSeq.ADDEDDATE,
                                         UPDATEBY = _ICSeq.UPDATEBY,
                                         UPDATEDATE = _ICSeq.UPDATEDATE,
                                         APPTYPE = _ICSeq.APPTYPE,
                                     }).OrderBy(b => b.APP_SEQ).ToList();

                _obj.ICAPPHISTORY = (from _POAppHis in _ICDBContext.DGIT_ICASSETAPPHISTORY.Where(x => x.ICASSETREQID == _obj.ICREQID)
                                     join _AppEmp in _ICDBContext.ADEMPLOYEEs1 on _POAppHis.ADEMPCODE equals _AppEmp.ADEMPCODE
                                     select new VM_DGIT_ICAPPHISTORY
                                     {
                                         ICAPPHISTORY_ID = _POAppHis.ICASSETAPPHISTORY_ID,
                                         ICREQID = _POAppHis.ICASSETREQID,
                                         ADEMPCODE = _POAppHis.ADEMPCODE,
                                         APPROVAL_STATUS = _POAppHis.APPROVAL_STATUS,
                                         APPROVAL_REMARK = _POAppHis.APPROVAL_REMARK,
                                         APPEMP_NAME = _AppEmp.FIRSTNAME + " " + _AppEmp.LASTNAME + " - [" + _AppEmp.ADEMPCODE + "]",
                                         APP_EMAIL = _AppEmp.EMAILID,
                                         APPEMP_CODE = _AppEmp.ADEMPCODE.ToString(),
                                         ADDEDDATE = _POAppHis.ADDEDDATE,
                                         UPDATEBY = _POAppHis.UPDATEBY,
                                         UPDATEDATE = _POAppHis.UPDATEDATE,
                                         APP_DATE = _POAppHis.APP_DATE,
                                         APP_TYPE = _POAppHis.APP_TYPE,
                                     }).OrderBy(o => o.ICAPPHISTORY_ID).ToList();
            }

            if (_obj.ICAPPAUTHSEQ.Count > 0)
            {
                long lastSendBackAppHisId = 0;
                DGIT_ICASSETAPPHISTORY lastSendBackAppHis = _ICDBContext.DGIT_ICASSETAPPHISTORY.Where(e => e.ICASSETREQID == _obj.ICREQID && e.APPROVAL_STATUS == 2).OrderByDescending(o => o.ICASSETAPPHISTORY_ID).FirstOrDefault();
                if (lastSendBackAppHis != null)
                {
                    lastSendBackAppHisId = lastSendBackAppHis.ICASSETAPPHISTORY_ID;
                }
                foreach (VM_DGIT_ICAPPAUTHSEQ obj in _obj.ICAPPAUTHSEQ)
                {
                    //bool IsBeforeSendBackRecord = _ICDBContext.DGIT_ICASSETAPPHISTORY.Any(w => w.ICASSETREQID == obj.ICREQID && w.ICASSETAPPHISTORY_ID <= lastSendBackAppHisId && w.ADEMPCODE == obj.ADEMPCODE);
                    //if ((IsBeforeSendBackRecord ? (!_obj.ICAPPHISTORY.Any(r => r.ICREQID == obj.ICREQID && r.ADEMPCODE == obj.ADEMPCODE && r.ICAPPHISTORY_ID > lastSendBackAppHisId)) : (!_obj.ICAPPHISTORY.Any(x => x.ICREQID == obj.ICREQID && x.ADEMPCODE == obj.ADEMPCODE))))
                    //{
                    //    _obj.ICAPPHISTORY.Add(new VM_DGIT_ICAPPHISTORY
                    //    {
                    //        ICAPPHISTORY_ID = 0,
                    //        ICREQID = obj.ICREQID,
                    //        ADEMPCODE = obj.ADEMPCODE,
                    //        APPROVAL_STATUS = 0,
                    //        APPROVAL_REMARK = "",
                    //        APPEMP_NAME = obj.ADEMPNAME + "[" + obj.ADEMPCODE + "]",
                    //        APP_TYPE = obj.APPTYPE
                    //    });
                    //}

                    var IsBeforeSendBackRecord = _ICDBContext.DGIT_ICASSETAPPHISTORY
                                               .FirstOrDefault(w => w.ICASSETREQID == obj.ICREQID
                                              && w.ICASSETAPPHISTORY_ID <= lastSendBackAppHisId
                                              && w.ADEMPCODE == obj.ADEMPCODE);

                    bool shouldAddHistory = false;

                    if (IsBeforeSendBackRecord != null)
                    {
                        var recordsWithHisId = _obj.ICAPPHISTORY
                            .FirstOrDefault(r => r.ICREQID == obj.ICREQID
                                              && r.ADEMPCODE == obj.ADEMPCODE
                                              && r.ICAPPHISTORY_ID > lastSendBackAppHisId);

                        shouldAddHistory = recordsWithHisId == null;
                    }
                    else
                    {
                        var recordsWithoutHisId = _obj.ICAPPHISTORY
                            .FirstOrDefault(x => x.ICREQID == obj.ICREQID
                                              && x.ADEMPCODE == obj.ADEMPCODE);

                        shouldAddHistory = recordsWithoutHisId == null;
                    }

                    if (shouldAddHistory)
                    {
                        _obj.ICAPPHISTORY.Add(new VM_DGIT_ICAPPHISTORY
                        {
                            ICAPPHISTORY_ID = 0,
                            ICREQID = obj.ICREQID,
                            ADEMPCODE = obj.ADEMPCODE,
                            APPROVAL_STATUS = 0,
                            APPROVAL_REMARK = "",
                            APPEMP_NAME = obj.ADEMPNAME + "[" + obj.ADEMPCODE + "]",
                            APP_TYPE = obj.APPTYPE
                        });
                    }

                }
            }
            foreach (var objs in _obj.ICREQ_SCHEDULE)
            {
                objs.SCHEDULESTR = objs.SCHEDULE.ToString("MMM-yyyy");
            }
            return _obj;
        }

        public short ICAssetApproval(VM_DGIT_ICAPPHISTORY PHVM)
        {
            short retVal = 0;
            using (var transaction = _ICDBContext.Database.BeginTransaction())
            {
                try
                {
                    DGIT_ICASSETAPPHISTORY DPAH = new DGIT_ICASSETAPPHISTORY();
                    if (PHVM.ICREQID > 0)
                    {
                        /////////// Update Approval Status //////////
                        // DPAH = _PoDBContext.DGIT_POAPPHISTORY.Where(x => x.POID == PHVM.POID && x.ADEMPCODE == PHVM.ADEMPCODE).FirstOrDefault();
                        DPAH = _ICDBContext.DGIT_ICASSETAPPHISTORY.Where(x => x.ICASSETREQID == PHVM.ICREQID && x.ADEMPCODE == PHVM.ADEMPCODE && x.APPROVAL_STATUS == 0).FirstOrDefault();
                        if (DPAH != null)
                        {
                            PHVM.APP_SEQ = DPAH.APP_SEQ;
                            PHVM.APP_TYPE = DPAH.APP_TYPE;
                            DPAH.APPROVAL_STATUS = PHVM.APPROVAL_STATUS;
                            DPAH.APPROVAL_REMARK = PHVM.APPROVAL_REMARK;
                            DPAH.UPDATEBY = PHVM.UPDATEBY;
                            DPAH.APP_DATE = DateTime.Now;
                            DPAH.UPDATEDATE = DateTime.Now;
                            _ICDBContext.Entry(DPAH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _ICDBContext.SaveChanges();
                        }
                        var appcntlst = _ICDBContext.DGIT_ICASSETAPPHISTORY.Where(x => x.ICASSETREQID == PHVM.ICREQID && x.APP_SEQ == PHVM.APP_SEQ && x.APPROVAL_STATUS == 0);
                        if (appcntlst.Count() > 0)
                        {
                            if (PHVM.APPROVAL_STATUS != 1)
                            {
                                foreach (var obj in appcntlst)
                                {
                                    _ICDBContext.DGIT_ICASSETAPPHISTORY.Remove(obj);
                                    _ICDBContext.SaveChanges();
                                }
                                if (PHVM.APP_TYPE == 2)
                                {
                                    var appseqlst = _ICDBContext.DGIT_ICAPPAUTHSEQ.Where(x => x.APP_SEQ >= PHVM.APP_SEQ);
                                    foreach (var data in appseqlst)
                                    {
                                        _ICDBContext.DGIT_ICAPPAUTHSEQ.Remove(data);
                                        _ICDBContext.SaveChanges();
                                    }
                                }
                                DGIT_ICASSETREQHEADER DPH1 = new DGIT_ICASSETREQHEADER(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                                DPH1 = _ICDBContext.DGIT_ICASSETREQHEADER.Where(x => x.ICASSETREQID == PHVM.ICREQID).SingleOrDefault();

                                if (DPH1 != null)
                                {
                                    if (PHVM.APPROVAL_STATUS == 2)
                                    {
                                        if (PHVM.APP_TYPE == 2)
                                            DPH1.PROCESSSTATUS = 2;
                                        else
                                            DPH1.PROCESSSTATUS = 5;
                                    }
                                    else if (PHVM.APPROVAL_STATUS == 3)
                                    {
                                        if (PHVM.APP_TYPE == 2)
                                            DPH1.PROCESSSTATUS = 2;
                                        else
                                            DPH1.PROCESSSTATUS = 3;
                                    }
                                    DPH1.MODIFIEDBY = PHVM.UPDATEBY;
                                    DPH1.MODIFIEDDATE = DateTime.Now;
                                    _ICDBContext.Entry(DPH1).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                    _ICDBContext.SaveChanges();
                                }
                            }

                            retVal = 1;
                            transaction.Commit();
                            return retVal;
                        }
                        /// --- Save PO Approval Authority --- ///
                        List<VM_DGIT_ICAPPAUTHSEQ> seqModel = new List<VM_DGIT_ICAPPAUTHSEQ>();
                        if (PHVM.APPROVAL_STATUS == 1)
                        {
                            short nextSeq = (short)(PHVM.APP_SEQ + 1);
                            long _nextEmpAuth = _ICDBContext.DGIT_ICASSETAPPAUTHSEQ.Where(a => a.APP_SEQ == nextSeq && a.ICASSETREQID == PHVM.ICREQID).Select(s => s.ADEMPCODE).FirstOrDefault();
                            if (_nextEmpAuth == PHVM.ADEMPCODE)
                            {
                                nextSeq = Convert.ToInt16(nextSeq + 1);
                            }
                            seqModel = (from data in _ICDBContext.DGIT_ICASSETAPPAUTHSEQ.Where(a => a.APP_SEQ == nextSeq && a.ICASSETREQID == PHVM.ICREQID)
                                        select new VM_DGIT_ICAPPAUTHSEQ
                                        {
                                            ICAPPAUTH_ID = data.ICASSETAPPAUTH_ID,
                                            ICREQID = data.ICASSETREQID,
                                            ADEMPCODE = data.ADEMPCODE,
                                            APP_SEQ = data.APP_SEQ,
                                            STATUS = data.STATUS,
                                            APPTYPE = data.APPTYPE,
                                            ISPARRALLEL = data.ISPARRALLEL,
                                            APP_HEADER = data.APP_HEADER,
                                        }).ToList();
                            if (seqModel != null)
                            {
                                foreach (var _obj in seqModel)
                                    SaveICAssetAppHis(PHVM.ADDEDBY, PHVM.ICREQID, _obj);
                            }
                        }

                        //////// Update Process Status ////////
                        DGIT_ICASSETREQHEADER DPH = new DGIT_ICASSETREQHEADER(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                        DPH = _ICDBContext.DGIT_ICASSETREQHEADER.Where(x => x.ICASSETREQID == PHVM.ICREQID).SingleOrDefault();
                        if (DPH != null)
                        {
                            if (PHVM.APPROVAL_STATUS == 1 && (seqModel == null || seqModel.Count == 0) && PHVM.APP_TYPE == 1)
                            {
                                DPH.PROCESSSTATUS = 1;
                            }
                            else if (PHVM.APPROVAL_STATUS == 1 && (seqModel == null || seqModel.Count == 0) && PHVM.APP_TYPE == 2)
                            {
                                DPH.PROCESSSTATUS = 7;
                            }
                            else if (PHVM.APPROVAL_STATUS == 1 && (seqModel != null || seqModel.Count > 0) && PHVM.APP_TYPE == 2)
                            {
                                DPH.PROCESSSTATUS = 6;
                            }
                            else if (PHVM.APPROVAL_STATUS == 1 && (seqModel != null || seqModel.Count > 0) && PHVM.APP_TYPE == 1)
                            {
                                DPH.PROCESSSTATUS = 1;
                            }
                            else if (PHVM.APPROVAL_STATUS == 2 && PHVM.APP_TYPE == 1)
                            {
                                DPH.PROCESSSTATUS = 5;
                            }
                            else if (PHVM.APPROVAL_STATUS == 2 && PHVM.APP_TYPE == 2)
                            {
                                var appseqlst = _ICDBContext.DGIT_ICAPPAUTHSEQ.Where(x => x.APP_SEQ >= PHVM.APP_SEQ);
                                foreach (var data in appseqlst)
                                {
                                    _ICDBContext.DGIT_ICAPPAUTHSEQ.Remove(data);
                                    _ICDBContext.SaveChanges();
                                }
                                DPH.PROCESSSTATUS = 2;
                            }
                            else if (PHVM.APPROVAL_STATUS == 3 && PHVM.APP_TYPE == 1)
                            {
                                DPH.PROCESSSTATUS = 3;
                            }
                            else if (PHVM.APPROVAL_STATUS == 3 && PHVM.APP_TYPE == 2)
                            {
                                var appseqlst = _ICDBContext.DGIT_ICAPPAUTHSEQ.Where(x => x.APP_SEQ >= PHVM.APP_SEQ);
                                foreach (var data in appseqlst)
                                {
                                    _ICDBContext.DGIT_ICAPPAUTHSEQ.Remove(data);
                                    _ICDBContext.SaveChanges();
                                }
                                DPH.PROCESSSTATUS = 2;
                            }
                            DPH.MODIFIEDBY = PHVM.UPDATEBY;
                            DPH.MODIFIEDDATE = DateTime.Now;
                            _ICDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _ICDBContext.SaveChanges();
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

        public short RejectICAssetRequest(VM_DGIT_ICAPPHISTORY PHVM)
        {
            using (var transaction = _ICDBContext.Database.BeginTransaction())
            {
                try
                {
                    DGIT_ICASSETAPPHISTORY DPAH = new DGIT_ICASSETAPPHISTORY();
                    int FlagAdd = 0;
                    // DPAH = _PoDBContext.DGIT_POAPPHISTORY.Where(d => d.ADEMPCODE == PSVM.ADEMPCODE && d.POID == POHeaderId).FirstOrDefault();
                    //if (DPAH == null)
                    {
                        DPAH = new DGIT_ICASSETAPPHISTORY();
                        if (_ICDBContext.DGIT_ICASSETAPPHISTORY.Count() == 0)
                        {
                            DPAH.ICASSETAPPHISTORY_ID = 1;
                        }
                        else
                        {
                            DPAH.ICASSETAPPHISTORY_ID = _ICDBContext.DGIT_ICASSETAPPHISTORY.Max(x => x.ICASSETAPPHISTORY_ID) + 1;
                        }
                        FlagAdd = 1;
                    }
                    DPAH.ICASSETREQID = PHVM.ICREQID;
                    DPAH.ADEMPCODE = (long)PHVM.UPDATEBY;
                    DPAH.APP_TYPE = 3;
                    DPAH.APPROVAL_STATUS = PHVM.APPROVAL_STATUS;
                    DPAH.APPROVAL_REMARK = PHVM.APPROVAL_REMARK;
                    if (FlagAdd == 1)
                    {
                        DPAH.ADDEDBY = (long)PHVM.UPDATEBY;
                        DPAH.ADDEDDATE = DateTime.Now;
                    }
                    else
                    {
                        DPAH.UPDATEBY = (long)PHVM.UPDATEBY;
                        DPAH.UPDATEDATE = DateTime.Now;
                    }
                    _ICDBContext.Entry(DPAH).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _ICDBContext.SaveChanges();
                    //////// Update Process Status ////////
                    DGIT_ICASSETREQHEADER DPH = new DGIT_ICASSETREQHEADER(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                    DPH = _ICDBContext.DGIT_ICASSETREQHEADER.Where(x => x.ICASSETREQID == PHVM.ICREQID).SingleOrDefault();
                    if (DPH != null)
                    {
                        DPH.PROCESSSTATUS = (short)(PHVM.APPROVAL_STATUS == 3 ? 9 : 5);
                        DPH.MODIFIEDBY = PHVM.UPDATEBY;
                        DPH.MODIFIEDDATE = DateTime.Today;
                        _ICDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        _ICDBContext.SaveChanges();
                    }
                    transaction.Commit();
                    return 1;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return -1;
                }
            }
        }

        public short ICAssetFinanceApproval(VM_DGIT_ICAPPHISTORY PHVM)
        {
            short retVal = 0;
            using (var transaction = _ICDBContext.Database.BeginTransaction())
            {
                try
                {
                    DGIT_ICASSETAPPHISTORY DPAH = new DGIT_ICASSETAPPHISTORY();
                    if (PHVM.ICREQID > 0)
                    {
                        /////////// Update Approval Status //////////
                        // DPAH = _PoDBContext.DGIT_POAPPHISTORY.Where(x => x.POID == PHVM.POID && x.ADEMPCODE == PHVM.ADEMPCODE).FirstOrDefault();
                        DPAH = _ICDBContext.DGIT_ICASSETAPPHISTORY.Where(x => x.ICASSETREQID == PHVM.ICREQID && x.ADEMPCODE == 1 && x.APPROVAL_STATUS == 0 && x.APP_TYPE == 3).FirstOrDefault();
                        if (DPAH != null)
                        {
                            DPAH.APPROVAL_STATUS = PHVM.APPROVAL_STATUS;
                            DPAH.APPROVAL_REMARK = PHVM.APPROVAL_REMARK;
                            DPAH.ADEMPCODE = (long)PHVM.UPDATEBY;
                            DPAH.UPDATEBY = PHVM.UPDATEBY;
                            DPAH.APP_DATE = DateTime.Now;
                            DPAH.UPDATEDATE = DateTime.Now;
                            _ICDBContext.Entry(DPAH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _ICDBContext.SaveChanges();
                        }
                        DGIT_ICASSETAPPAUTHSEQ DPSQ = new DGIT_ICASSETAPPAUTHSEQ();
                        DPSQ = _ICDBContext.DGIT_ICASSETAPPAUTHSEQ.Where(x => x.ICASSETREQID == PHVM.ICREQID && x.ADEMPCODE == 1 && x.APPTYPE == 3).FirstOrDefault();

                        if (DPSQ != null)
                        {
                            DPSQ.ADEMPCODE = (long)PHVM.UPDATEBY;
                            _ICDBContext.Entry(DPSQ).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _ICDBContext.SaveChanges();
                        }
                        /// --- Save PO Approval Authority --- ///
                        VM_DGIT_ICAPPAUTHSEQ seqModel = new VM_DGIT_ICAPPAUTHSEQ();
                        if (PHVM.APPROVAL_STATUS == 1)
                        {
                            short nextSeq = Convert.ToInt16(_ICDBContext.DGIT_ICASSETAPPAUTHSEQ.Where(s => s.ICASSETREQID == PHVM.ICREQID && s.ADEMPCODE == PHVM.ADEMPCODE).Select(s => s.APP_SEQ).FirstOrDefault() + 1);
                            long _nextEmpAuth = _ICDBContext.DGIT_ICASSETAPPAUTHSEQ.Where(a => a.APP_SEQ == nextSeq && a.ICASSETREQID == PHVM.ICREQID).Select(s => s.ADEMPCODE).FirstOrDefault();
                            if (_nextEmpAuth == PHVM.ADEMPCODE)
                            {
                                nextSeq = Convert.ToInt16(nextSeq + 1);
                            }
                            seqModel = (from data in _ICDBContext.DGIT_ICASSETAPPAUTHSEQ.Where(a => a.APP_SEQ == nextSeq && a.ICASSETREQID == PHVM.ICREQID)
                                        select new VM_DGIT_ICAPPAUTHSEQ
                                        {
                                            ICAPPAUTH_ID = data.ICASSETAPPAUTH_ID,
                                            ICREQID = data.ICASSETREQID,
                                            ADEMPCODE = data.ADEMPCODE,
                                            APP_SEQ = data.APP_SEQ,
                                            STATUS = data.STATUS,
                                            APPTYPE = data.APPTYPE,
                                        }).FirstOrDefault();
                            if (seqModel != null)
                            {
                                SaveICAssetAppHis(PHVM.ADDEDBY, PHVM.ICREQID, seqModel);
                            }
                        }

                        //////// Update Process Status ////////
                        DGIT_ICASSETREQHEADER DPH = new DGIT_ICASSETREQHEADER(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                        DPH = _ICDBContext.DGIT_ICASSETREQHEADER.Where(x => x.ICASSETREQID == PHVM.ICREQID).SingleOrDefault();
                        if (DPH != null)
                        {
                            if (PHVM.APPROVAL_STATUS == 1 && seqModel == null)
                            {
                                DPH.PROCESSSTATUS = 2;
                                DPH.ISIBMREQUIRED = PHVM.ISICIBMREQ;
                                DPH.ICCONFIGID = PHVM.ICCYCLEID;
                            }
                            else if (PHVM.APPROVAL_STATUS == 2)
                            {
                                DPH.PROCESSSTATUS = 5;
                            }
                            else if (PHVM.APPROVAL_STATUS == 3)
                            {
                                DPH.PROCESSSTATUS = 3;
                            }
                            else
                            {
                                DPH.PROCESSSTATUS = 1;
                            }
                            DPH.MODIFIEDBY = PHVM.UPDATEBY;
                            DPH.MODIFIEDDATE = DateTime.Now;
                            _ICDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _ICDBContext.SaveChanges();
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

        public void SaveAssetAppAuthSeq_WD(long AddedBy, long ICHeaderId, List<VM_DGIT_ICAPPAUTHSEQ> PSVMList)
        {
            foreach (VM_DGIT_ICAPPAUTHSEQ PSVM in PSVMList.OrderBy(o => o.APP_SEQ).ToList())
            {
                DGIT_ICASSETAPPAUTHSEQ DAAS = new DGIT_ICASSETAPPAUTHSEQ();
                if (_ICDBContext.DGIT_ICASSETAPPAUTHSEQ.Count() == 0)
                {
                    DAAS.ICASSETAPPAUTH_ID = 1;
                }
                else
                {
                    DAAS.ICASSETAPPAUTH_ID = _ICDBContext.DGIT_ICASSETAPPAUTHSEQ.Max(x => x.ICASSETAPPAUTH_ID) + 1;
                }
                DAAS.ICASSETREQID = ICHeaderId;
                DAAS.ADEMPCODE = PSVM.ADEMPCODE;
                DAAS.APP_SEQ = PSVM.APP_SEQ;
                DAAS.APPTYPE = PSVM.APPTYPE;
                DAAS.STATUS = 1;
                DAAS.ADDEDBY = AddedBy;
                DAAS.ADDEDDATE = DateTime.Now;
                _ICDBContext.Entry(DAAS).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                _ICDBContext.SaveChanges();
            }
        }

        public long SaveICMemberMaster(long AddedBy, List<VM_DGIT_ICAPPAUTHSEQ> PSVMList)
        {
            long retVal = 0;
            using (var transaction = _ICDBContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (var odel in _ICDBContext.DGIT_ICMEMBER_MST)
                    {
                        _ICDBContext.Entry(odel).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                        _ICDBContext.SaveChanges();
                    }
                    foreach (VM_DGIT_ICAPPAUTHSEQ PSVM in PSVMList.OrderBy(o => o.APP_SEQ).ToList())
                    {
                        DGIT_ICMEMBER_MST DAAS = new DGIT_ICMEMBER_MST();
                        if (_ICDBContext.DGIT_ICMEMBER_MST.Count() == 0)
                        {
                            DAAS.ICMEMBERMSTID = 1;
                        }
                        else
                        {
                            DAAS.ICMEMBERMSTID = _ICDBContext.DGIT_ICMEMBER_MST.Max(x => x.ICMEMBERMSTID) + 1;
                        }
                        DAAS.ADEMPCODE = PSVM.ADEMPCODE;
                        DAAS.APPHEADER = PSVM.APP_HEADER;
                        DAAS.APP_SEQ = PSVM.APP_SEQ;
                        DAAS.APPTYPE = 2;
                        DAAS.STATUS = 1;
                        DAAS.ADDEDBY = AddedBy;
                        DAAS.DATEADDED = DateTime.Now;
                        _ICDBContext.Entry(DAAS).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                        _ICDBContext.SaveChanges();
                    }
                    transaction.Commit();
                    retVal = 1;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    retVal = 0;
                }
                return retVal;
            }
        }

        public List<VM_DGIT_ICCONFIG_MST> GetICConfig()
        {
            return (from oData in _ICDBContext.DGIT_ICCONFIG_MST.Where(m => m.STATUS == 1).OrderBy(m => m.ICCONFIGID)
                    select new VM_DGIT_ICCONFIG_MST
                    {
                        ICCONFIGID = oData.ICCONFIGID,
                        ICMONTH = oData.ICMONTH,
                        ICDATE = oData.ICDATE,
                        LASTSUBDT = oData.LASTSUBDT,
                        MAILTO = oData.MAILTO,
                        ADDEDBY = oData.ADDEDBY,
                        MAILIDS = oData.MAILIDS
                    }
                    ).ToList();
        }
        public VM_DGIT_ICCONFIG_MST GetICConfig(VM_DGIT_ICCONFIG_MST data)
        {
            return (from oData in _ICDBContext.DGIT_ICCONFIG_MST.Where(m => m.ICCONFIGID == data.ICCONFIGID)
                    select new VM_DGIT_ICCONFIG_MST
                    {
                        ICCONFIGID = oData.ICCONFIGID,
                        ICMONTH = oData.ICMONTH,
                        ICDATE = oData.ICDATE,
                        LASTSUBDT = oData.LASTSUBDT,
                        MAILTO = oData.MAILTO,
                        ADDEDBY = oData.ADDEDBY,
                        MAILIDS = oData.MAILIDS,
                        ICMEETINGNO = oData.ICMEETINGNO,
                        MEETINGLOC = oData.MEETING_LOC,
                        MEETINGTIME = oData.MEETING_TIME,
                        CONTACTEMAIL = oData.EMAIL_CONTACT
                    }).First();
        }
        public short saveIC(VM_DGIT_ICCONFIG_MST data)
        {
            short retval = 0;
            try
            {
                DGIT_ICCONFIG_MST new_data = new DGIT_ICCONFIG_MST();
                if (_ICDBContext.DGIT_ICCONFIG_MST.Count() == 0)
                {
                    new_data.ICCONFIGID = 1;
                }
                else
                {
                    new_data.ICCONFIGID = _ICDBContext.DGIT_ICCONFIG_MST.Max(x => x.ICCONFIGID) + 1;
                }
                //new_data.ICCONFIGID = _ICDBContext.DGIT_ICCONFIG_MST.Max(x => x.ICCONFIGID) + 1;
                new_data.ICDATE = data.ICDATE;
                new_data.ICMONTH = data.ICMONTH;
                new_data.LASTSUBDT = data.LASTSUBDT;
                new_data.DATEADDED = DateTime.Today;
                new_data.MAILIDS = data.MAILIDS;
                new_data.ADDEDBY = data.ADDEDBY;
                new_data.MAILTO = (short)data.MAILTO;
                new_data.ICMEETINGNO = data.ICMEETINGNO;
                new_data.MEETING_TIME = data.MEETINGTIME;
                new_data.MEETING_LOC = data.MEETINGLOC;
                new_data.EMAIL_CONTACT = data.CONTACTEMAIL;
                new_data.STATUS = 1;
                new_data.MAILSENTFLAG = 0;
                _ICDBContext.DGIT_ICCONFIG_MST.Add(new_data);
                _ICDBContext.SaveChanges();
                retval = 1;
            }
            catch (Exception ex)
            {
                retval = -1;
            }
            return retval;
        }
        public short deleteIC(VM_DGIT_ICCONFIG_MST data)
        {
            short retval = 0;
            try
            {
                var DPD = _ICDBContext.DGIT_ICCONFIG_MST.Where(x => x.ICCONFIGID == data.ICCONFIGID).First();
                DPD.STATUS = 0;
                _ICDBContext.Entry(DPD).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _ICDBContext.SaveChanges();
                retval = 1;
            }
            catch (Exception ex)
            {
                retval = -1;
            }
            return retval;
        }
        public short UpdateIC(VM_DGIT_ICCONFIG_MST data)
        {
            short retval = 0;
            try
            {
                DGIT_ICCONFIG_MST new_data = _ICDBContext.DGIT_ICCONFIG_MST.Where(x => x.ICCONFIGID == data.ICCONFIGID).First();
                new_data.ICDATE = data.ICDATE;
                new_data.ICMONTH = data.ICMONTH;
                new_data.LASTSUBDT = data.LASTSUBDT;
                new_data.DATEADDED = DateTime.Today;
                new_data.MAILIDS = data.MAILIDS;
                new_data.ADDEDBY = data.ADDEDBY;
                new_data.MAILTO = (short)data.MAILTO;
                new_data.ICMEETINGNO = data.ICMEETINGNO;
                new_data.MEETING_TIME = data.MEETINGTIME;
                new_data.MEETING_LOC = data.MEETINGLOC;
                new_data.EMAIL_CONTACT = data.CONTACTEMAIL;
                new_data.STATUS = 1;
                _ICDBContext.Entry(new_data).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _ICDBContext.SaveChanges();
                retval = 1;
            }
            catch (Exception ex)
            {
                retval = -1;
            }
            return retval;
        }

        public short ICReqCancel(VM_DGIT_ICAPPHISTORY PHVM)
        {
            short retVal = 0;
            using (var transaction = _ICDBContext.Database.BeginTransaction())
            {
                try
                {
                    DGIT_ICAPPHISTORY DPAH = new DGIT_ICAPPHISTORY();
                    if (PHVM.ICREQID > 0)
                    {
                        //////// Update Process Status ////////
                        DGIT_ICREQHEADER DPH = new DGIT_ICREQHEADER(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                        DPH = _ICDBContext.DGIT_ICREQHEADER.Where(x => x.ICREQID == PHVM.ICREQID).SingleOrDefault();
                        if (DPH != null)
                        {
                            DPH.REMARK = PHVM.APPROVAL_REMARK;
                            DPH.PROCESSSTATUS = PHVM.APPROVAL_STATUS;
                            DPH.MODIFIEDBY = PHVM.UPDATEBY;
                            DPH.MODIFIEDDATE = DateTime.Now;
                            _ICDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _ICDBContext.SaveChanges();
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
        public short ICAssetReqCancel(VM_DGIT_ICAPPHISTORY PHVM)
        {
            short retVal = 0;
            using (var transaction = _ICDBContext.Database.BeginTransaction())
            {
                try
                {
                    DGIT_ICAPPHISTORY DPAH = new DGIT_ICAPPHISTORY();
                    if (PHVM.ICREQID > 0)
                    {
                        //////// Update Process Status ////////
                        DGIT_ICASSETREQHEADER DPH = new DGIT_ICASSETREQHEADER(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                        DPH = _ICDBContext.DGIT_ICASSETREQHEADER.Where(x => x.ICASSETREQID == PHVM.ICREQID).SingleOrDefault();
                        if (DPH != null)
                        {
                            DPH.REMARK = PHVM.APPROVAL_REMARK;
                            DPH.PROCESSSTATUS = PHVM.APPROVAL_STATUS;
                            DPH.MODIFIEDBY = PHVM.UPDATEBY;
                            DPH.MODIFIEDDATE = DateTime.Now;
                            _ICDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _ICDBContext.SaveChanges();
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

        public VM_ICIBM_PPCDASHBOARD_Search GetPPCDashboard(VM_ICIBM_PPCDASHBOARD_Search searchobj)
        {
            List<DGIT_ICRPTRIGHTS> iList = _ICDBContext.DGIT_ICRPTRIGHTS.Where(n => n.ADEMPCODE == searchobj.SearchBy && n.STATUS == 1 && n.REPORTTYPE == 1).ToList();
            List<long?> operationids = iList.Where(m => m.OPERATIONID != null && m.DIVISIONID == null && m.DEPARTMENTID == null && m.SECTIONID == null).Select(m => m.OPERATIONID).ToList();
            List<long?> divisionids = iList.Where(m => m.DIVISIONID != null && m.OPERATIONID == null && m.DEPARTMENTID == null && m.SECTIONID == null).Select(m => m.DIVISIONID).ToList();
            List<long?> departmentids = iList.Where(m => m.DEPARTMENTID != null && m.DIVISIONID == null && m.OPERATIONID == null && m.SECTIONID == null).Select(m => m.DEPARTMENTID).ToList();
            List<long?> sectionids = iList.Where(m => m.SECTIONID != null && m.DIVISIONID == null && m.DEPARTMENTID == null && m.OPERATIONID == null).Select(m => m.SECTIONID).ToList();
            var _obj = (from data in _ICDBContext.ICIBM_PPCDASHBOARD
                        where (searchobj.ADEMPCODE == 0 ? true : data.ADEMPCODE == searchobj.ADEMPCODE)
                        && (string.IsNullOrEmpty(searchobj.DEPARTMENT) ? true : data.DEPARTMENT == searchobj.DEPARTMENT)
                        && (string.IsNullOrEmpty(searchobj.ENAME) ? true : data.ENAME == searchobj.ENAME)
                        && (string.IsNullOrEmpty(searchobj.ICTITLE) ? true : data.ICTITLE == searchobj.ICTITLE)
                        && (string.IsNullOrEmpty(searchobj.OPERATION) ? true : data.OPERATION == searchobj.OPERATION)
                        //&& (searchobj.LASTAPPROVAL_FROM.Year == 1 ? true : data.LASTAPPROVAL >= searchobj.LASTAPPROVAL_FROM)
                        //Below Change added by aumento for the SR72573----------------------------------------
                        //&& (searchobj.LASTAPPROVAL_TO.Year == 1 ? true : data.LASTAPPROVAL <= searchobj.LASTAPPROVAL_TO)
                        && searchobj.LASTAPPROVAL_FROM.HasValue ? data.LASTAPPROVAL >= searchobj.LASTAPPROVAL_FROM.Value : true
                        && searchobj.LASTAPPROVAL_TO.HasValue ? data.LASTAPPROVAL >= searchobj.LASTAPPROVAL_TO.Value : true
                        //&& (searchobj.LASTAPPROVAL_TO.Year == 1 ? true : data.LASTAPPROVAL >= searchobj.LASTAPPROVAL_TO)
                        //--------------------------------------------------------------------------------------
                        //&& (searchobj.LASTAPPROVAL_TO.Year == 1 ? true : data.LASTAPPROVAL <= searchobj.LASTAPPROVAL_TO)                        
                        && (searchobj.status == 10 ? true : data.PROCESSSTATUS == searchobj.status)
                        && (searchobj.ICTYPE == data.ICTYPE)
                        //&& ((operationids.Count == 0 ? true : operationids.Contains(data.OPERATIONID))
                        //&& (divisionids.Count == 0 ? true : (divisionids.Contains(data.DIVISIONID) || (operationids.Count>0?operationids.Contains(data.OPERATIONID):false)) )
                        //&& (departmentids.Count == 0 ? true : (departmentids.Contains(data.DEPARTMENTID) || (divisionids.Count>0? divisionids.Contains(data.DIVISIONID):true) || (operationids.Count > 0 ? operationids.Contains(data.OPERATIONID) : true))))
                        //|| (sectionids.Count == 0 ? true : (sectionids.Contains(data.SECTIONID)|| departmentids.Contains(data.DEPARTMENTID) || divisionids.Contains(data.DIVISIONID) || operationids.Contains(data.OPERATIONID))))
                        select new VM_ICIBM_PPCDASHBOARD
                        {
                            ICREQID = data.ICREQID,
                            SCHEDULE = data.SCHEDULE,
                            PAYBACK_PERIOD = data.PAYBACK_PERIOD,
                            ICTYPE = data.ICTYPE,
                            REQUESTTYPE = data.REQUESTTYPE,
                            OPERATION = data.OPERATION,
                            DEPARTMENT = data.DEPARTMENT,
                            ENAME = data.ENAME,
                            ADEMPCODE = data.ADEMPCODE,
                            ICTITLE = data.ICTITLE,
                            BASIC_BUDGET = data.BASIC_BUDGET,
                            INVEST_EFFECT = data.INVEST_EFFECT,
                            COSTSAVING = data.COSTSAVING,
                            GAVAMT = data.GAVAMT,
                            NAVAMT = data.NAVAMT,
                            SRVAMT = data.SRVAMT,
                            NPLAMT = data.NPLAMT,
                            LASTAPPROVAL_FROM = data.LASTAPPROVAL,
                            status = data.PROCESSSTATUS,
                            OPERATIONID = data.OPERATIONID,
                            DIVISIONID = data.DIVISIONID,
                            DEPARTMENTID = data.DEPARTMENTID,
                            SECTIONID = data.SECTIONID
                        }).ToList();




            List<VM_ICIBM_PPCDASHBOARD> finallist = new List<VM_ICIBM_PPCDASHBOARD>();
            if (operationids.Count > 0)
            {
                finallist.AddRange(_obj.Where(m => operationids.Contains(m.OPERATIONID)));
            }
            if (divisionids.Count > 0)
            {
                finallist.AddRange(_obj.Where(m => divisionids.Contains(m.DIVISIONID)));
            }
            if (departmentids.Count > 0)
            {
                finallist.AddRange(_obj.Where(m => departmentids.Contains(m.DEPARTMENTID)));
            }
            if (sectionids.Count > 0)
            {
                finallist.AddRange(_obj.Where(m => sectionids.Contains(m.SECTIONID)));
            }
            searchobj.Result = finallist;
            return searchobj;
        }

        public List<VM_SELECTITEMLIST> ICInvestdtls()
        {
            List<VM_SELECTITEMLIST> icinvest_master = (from _iceffect in _ICDBContext.DGIT_ICINVEST_MST.Where(m => m.STATUS == 1)
                                                       select new VM_SELECTITEMLIST
                                                       {
                                                           Text = _iceffect.INVESTEFFECTNAME,
                                                           Value = _iceffect.ICINVESTMSTID.ToString(),
                                                       }).ToList();
            return icinvest_master;
        }



    }
}
