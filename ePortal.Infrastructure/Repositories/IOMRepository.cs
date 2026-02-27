using ePortal.DomainClasses;
using ePortal.ViewModels;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.Infrastructure.DbContexts;
using ePortal.Shared.Interface;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ePortal.Persistence.Services;
using System.Data;
using ePortal.Persistence.Interface;
/*Added by TTL against SR109950 > CR7268 - Start*/
using System.Globalization;
using Oracle.ManagedDataAccess.Client; /*CR7675*/
/*Added by TTL against SR109950 > CR7268 - End*/

namespace ePortal.Infrastructure.Repositories
{
    public class IOMRepository
    {
        private readonly EPortalDBContext _IOMDBContext;
        private readonly ICProcessBDContext _ICDBContext;
        private readonly ICommonFunctions _ObjCommn;
        private readonly SYKI _Syki;

        public IOMRepository(EPortalDBContext iomDbContext, ICProcessBDContext icDbContext, ICommonFunctions ObjCommn)
        {
            _IOMDBContext = iomDbContext;
            _ICDBContext = icDbContext;
            _ObjCommn = ObjCommn;
            _Syki = _IOMDBContext.SYKI.Where(x => x.ACTIVE == 1).FirstOrDefault();
        }

        public Tuple<short, long> SaveIOMRequest(IOMHeaderViewModel model)
        {
            short retVal = 0; long retHeaderId = 0;
            Tuple<short, long> _retVal_tuple;
            var IOMDirectorDESGIDlst = _IOMDBContext.SYPARAMETERS.Where(m => m.PARAMNAME == "APPNOTE_DIRECTOR_DESG");
            string IOMDirectorDESGID = "";
            if ((model.APP_TYPE == 1 && model.iomAuthSeq.Where(m => m.Header == "Director" || m.Header == "Senior Director").Count() > 0) || (model.APP_TYPE == 2))
            {
                if (IOMDirectorDESGIDlst.Count() > 0)
                {
                    IOMDirectorDESGID = IOMDirectorDESGIDlst.FirstOrDefault().PARAMVALUE;
                }
            }

            if (model.iomAuthSeq != null)
            {
                if (IOMDirectorDESGID != "")
                {
                    long[] arrIOMDirectorDESGID = (Array.ConvertAll(IOMDirectorDESGID.Split(','), Int64.Parse)).ToArray();
                    long[] seqecodelist;
                    if (model.APP_TYPE == 1)
                        seqecodelist = model.iomAuthSeq.Where(m => (m.Header == "Director" || m.Header == "Senior Director")).Select(n => n.ADEMPCODE).ToArray();
                    else
                        seqecodelist = model.iomAuthSeq.Select(n => n.ADEMPCODE).ToArray();

                    var DirectorRecord = (from data in _IOMDBContext.VW_ASSOCIATELVLDETAILS.Where(m => m.SYKI == _Syki.SYKIID && m.ACTIVE == 1 && seqecodelist.Contains(m.ADEMPCODE))
                                          select new
                                          {
                                              ADEMPCODE = data.ADEMPCODE,
                                              ADDESIGNATIONID = (Int64)(data.ADDESIGNATIONID),
                                              OPERATIONID = data.OPERATIONID
                                          }
                                           ).Where(m => arrIOMDirectorDESGID.Contains(m.ADDESIGNATIONID));
                    if (DirectorRecord.Count() > 0)
                    {

                        foreach (var direc in DirectorRecord)
                        {
                            var operdir = _IOMDBContext.VW_ASSOCIATELVLDETAILS.Where(m => m.SYKI == _Syki.SYKIID && m.ACTIVE == 1 && m.OPERATIONID == direc.OPERATIONID && (m.ADDESIGNATIONID == 27 || m.ADDESIGNATIONID == 34));
                            long IOMOPDirCNT = DirectorRecord.Where(m => m.OPERATIONID == direc.OPERATIONID).Count();
                            if (operdir.Count() != IOMOPDirCNT)
                            {
                                _retVal_tuple = new Tuple<short, long>(2, direc.ADEMPCODE);
                                return _retVal_tuple;
                            }
                        }

                    }
                }
            }

            using (var transaction = _IOMDBContext.Database.BeginTransaction())
            {
                try
                {
                    DGIT_IOMHEADER DPH = new DGIT_IOMHEADER();
                    int FlagAdd = 0;
                    if (model.IOMHEADERID > 0)
                    {
                        DPH = _IOMDBContext.DGIT_IOMHEADER.Where(x => x.IOMHEADERID == model.IOMHEADERID).SingleOrDefault();
                    }
                    else
                    {
                        //if (_IOMDBContext.DGIT_IOMHEADER.Any(x => x.IOMHEADERID != model.IOMHEADERID))
                        //{
                        //    retVal = 2;
                        //    return _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId); //// -- record already exist.
                        //}
                        DPH = new DGIT_IOMHEADER();
                        if (_IOMDBContext.DGIT_IOMHEADER.Count() == 0)
                        {
                            DPH.IOMHEADERID = 1;
                        }
                        else
                        {
                            DPH.IOMHEADERID = _IOMDBContext.DGIT_IOMHEADER.Max(x => x.IOMHEADERID) + 1;
                        }
                        FlagAdd = 1;
                    }
                    DPH.IOM_DESC = model.IOMDesc;
                    DPH.PROCESS_STATUS = model.PROCESS_STATUS;
                    DPH.STATUS = model.STATUS;
                    DPH.APP_TYPE = model.APP_TYPE;
                    DPH.IOMCATID = model.IOMCATMSTID;
                    //Added by TTL on 18th July 2025 against SR101913 > CR6738 - Start
                    if (model.IOMCATMSTID == 4)
                    {
                        DPH.ARIBARFPID = model.ARIBARFPID?.Trim();//Modified by TTL on 28th Oct 2025 against SR109889 > CR7269
                    }
                    else
                    {
                        DPH.ARIBARFPID = null;
                    }
                    //Added by TTL on 18th July 2025 against SR101913 > CR6738 - End
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
                    _IOMDBContext.Entry(DPH).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                    _IOMDBContext.SaveChanges();
                    //--- Save PR-Link Details
                    //Added by TTL on 28th Oct 2025 against SR109889 > CR7269 - Start
                    if (model.IOMCATMSTID == 4)
                    {
                        if (model.IndentNos?.Count != 0)
                        {
                            SaveIOMPRLink(model.ADDEDBY, DPH.IOMHEADERID, model.IndentNos);
                        }
                    }
                    //Added by TTL on 28th Oct 2025 against SR109889 > CR7269 - End
                    /// --- Save IOM Detail --- ///
                    if (model.iomDetail != null)
                    {
                        if (model.iomDetail.Count > 0)
                        {
                            SaveIOMDetails(model.ADDEDBY, DPH.IOMHEADERID, model.iomDetail);
                        }
                    }

                    /// --- Save IOM Approval Auth Seq ---///
                    if (model.iomAuthSeq != null)
                    {
                        if (model.iomAuthSeq.Count > 0)
                        {
                            SaveAppAuthSeq(model.ADDEDBY, DPH.IOMHEADERID, model.iomAuthSeq);

                            /// --- Save IOM Approval Authority --- ///
                            IOMAppAuthSeqViewModel seqModel = model.iomAuthSeq.OrderBy(o => o.APP_SEQ).FirstOrDefault();
                            if (seqModel != null)
                            {
                                SaveIOMAppHis(model.ADDEDBY, DPH.IOMHEADERID, seqModel);
                            }
                        }
                    }

                    /// --- Save IOM Approval Header ---///
                    if (model.iomAppHeaderList != null)
                    {
                        if (model.iomAppHeaderList.Count > 0)
                        {
                            SaveAppHeader(model.ADDEDBY, DPH.IOMHEADERID, model.iomAppHeaderList);
                        }
                    }

                    transaction.Commit();
                    retVal = 1;
                    retHeaderId = DPH.IOMHEADERID;
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                    throw;
                }
                _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId);
                return _retVal_tuple;
            }
        }

        public short SaveIOMDetails(long AddedBy, long IOMHEADERID, List<IOMDetailViewModel> PDVMList)
        {
            short retVal = 0;
            foreach (IOMDetailViewModel PDVM in PDVMList)
            {
                DGIT_IOMDETAIL DPD = new DGIT_IOMDETAIL();
                int FlagAdd = 0;
                if (IOMHEADERID > 0)
                {
                    if (PDVM.DOC_TYPE == "IOM")
                    {
                        DPD = _IOMDBContext.DGIT_IOMDETAIL.Where(x => x.IOMHEADERID == IOMHEADERID && x.DOC_TYPE == PDVM.DOC_TYPE).FirstOrDefault();
                        if (DPD != null)
                        {
                            _IOMDBContext.DGIT_IOMDETAIL.Remove(DPD);
                            _IOMDBContext.SaveChanges();
                        }
                    }

                    DPD = new DGIT_IOMDETAIL();
                    if (_IOMDBContext.DGIT_IOMDETAIL.Count() == 0)
                    {
                        DPD.IOMDTL_ID = 1;
                    }
                    else
                    {
                        DPD.IOMDTL_ID = _IOMDBContext.DGIT_IOMDETAIL.Max(x => x.IOMDTL_ID) + 1;
                    }
                    FlagAdd = 1;

                    DPD.IOMHEADERID = IOMHEADERID;
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
                    _IOMDBContext.Entry(DPD).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                    _IOMDBContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }

        public void SaveAppAuthSeq(long AddedBy, long IOMHEADERID, List<IOMAppAuthSeqViewModel> PSVMList)
        {
            //// --- Delete recode ---////
            List<DGIT_IOMAPPAUTHSEQ> SeqList = _IOMDBContext.DGIT_IOMAPPAUTHSEQ.Where(t => t.IOMHEADERID == IOMHEADERID).ToList();
            if (SeqList.Count > 0)
            {
                _IOMDBContext.DGIT_IOMAPPAUTHSEQ.RemoveRange(SeqList);
                _IOMDBContext.SaveChanges();
            }

            //List<DGIT_IOMAPPHISTORY> HisList = _IOMDBContext.DGIT_IOMAPPHISTORY.Where(t => t.IOMHEADERID == IOMHEADERID).ToList();
            //if (HisList.Count > 0)
            //{
            //    _IOMDBContext.DGIT_IOMAPPHISTORY.RemoveRange(HisList);
            //    _IOMDBContext.SaveChanges();
            //}


            foreach (IOMAppAuthSeqViewModel PSVM in PSVMList.OrderBy(o => o.APP_SEQ).ToList())
            {
                DGIT_IOMAPPAUTHSEQ DAAS = new DGIT_IOMAPPAUTHSEQ();
                if (_IOMDBContext.DGIT_IOMAPPAUTHSEQ.Count() == 0)
                {
                    DAAS.IOMAPPAUTH_ID = 1;
                }
                else
                {
                    DAAS.IOMAPPAUTH_ID = _IOMDBContext.DGIT_IOMAPPAUTHSEQ.Max(x => x.IOMAPPAUTH_ID) + 1;
                }
                DAAS.IOMHEADERID = IOMHEADERID;
                DAAS.IOMAPPHEADER = PSVM.Header;
                DAAS.ADEMPCODE = PSVM.ADEMPCODE;
                DAAS.APP_SEQ = PSVM.APP_SEQ;
                DAAS.APPTYPE = PSVM.APPTYPE;
                DAAS.STATUS = 1;
                DAAS.ADDEDBY = AddedBy;
                DAAS.ADDEDDATE = DateTime.Now;
                _IOMDBContext.Entry(DAAS).State = EntityState.Added;
                _IOMDBContext.SaveChanges();
            }
        }

        public void SaveIOMAppHis(long AddedBy, long IOMHEADERID, IOMAppAuthSeqViewModel PSVM)
        {
            DGIT_IOMAPPHISTORY DPAH = new DGIT_IOMAPPHISTORY();
            int FlagAdd = 0;
            DPAH = _IOMDBContext.DGIT_IOMAPPHISTORY.Where(d => d.ADEMPCODE == PSVM.ADEMPCODE && d.IOMHEADERID == IOMHEADERID && d.APPROVAL_STATUS == 0).FirstOrDefault();
            int APPauthseq = _IOMDBContext.DGIT_IOMAPPAUTHSEQ.Where(m => m.IOMHEADERID == IOMHEADERID && m.ADEMPCODE == PSVM.ADEMPCODE).Count();
            if (DPAH == null && APPauthseq > 0)
            {
                DPAH = new DGIT_IOMAPPHISTORY();
                if (_IOMDBContext.DGIT_IOMAPPHISTORY.Count() == 0)
                {
                    DPAH.IOMAPPHISTORY_ID = 1;
                }
                else
                {
                    DPAH.IOMAPPHISTORY_ID = _IOMDBContext.DGIT_IOMAPPHISTORY.Max(x => x.IOMAPPHISTORY_ID) + 1;
                }
                FlagAdd = 1;

                DPAH.IOMHEADERID = IOMHEADERID;
                DPAH.ADEMPCODE = PSVM.ADEMPCODE;
                DPAH.APPTYPE = PSVM.APPTYPE;
                DPAH.APPROVAL_STATUS = 0;
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
                _IOMDBContext.Entry(DPAH).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                _IOMDBContext.SaveChanges();
            }
        }
        // Added by Aumento 13072024

        public void SaveIOMAppHisApproval(long AddedBy, long IOMHEADERID, IOMAppAuthSeqViewModel PSVM, string Remark, short ApprovalStatus)
        {
            DGIT_IOMAPPHISTORY DPAH = new DGIT_IOMAPPHISTORY();
            int FlagAdd = 0;
            DPAH = _IOMDBContext.DGIT_IOMAPPHISTORY.Where(d => d.ADEMPCODE == PSVM.ADEMPCODE && d.IOMHEADERID == IOMHEADERID && d.APPROVAL_STATUS == 0).FirstOrDefault();
            int APPauthseq = _IOMDBContext.DGIT_IOMAPPAUTHSEQ.Where(m => m.IOMHEADERID == IOMHEADERID && m.ADEMPCODE == PSVM.ADEMPCODE).Count();
            if (DPAH == null && APPauthseq > 0)
            {
                DPAH = new DGIT_IOMAPPHISTORY();
                if (_IOMDBContext.DGIT_IOMAPPHISTORY.Count() == 0)
                {
                    DPAH.IOMAPPHISTORY_ID = 1;
                }
                else
                {
                    DPAH.IOMAPPHISTORY_ID = _IOMDBContext.DGIT_IOMAPPHISTORY.Max(x => x.IOMAPPHISTORY_ID) + 1;
                }
                FlagAdd = 1;

                DPAH.IOMHEADERID = IOMHEADERID;
                DPAH.ADEMPCODE = PSVM.ADEMPCODE;
                DPAH.APPTYPE = PSVM.APPTYPE;
                DPAH.APPROVAL_STATUS = ApprovalStatus;
                DPAH.APPROVAL_REMARK = Remark;
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
                _IOMDBContext.Entry(DPAH).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                _IOMDBContext.SaveChanges();
            }
        }

        // Added by Aumento 13072024

        public void SaveAppHeader(long AddedBy, long IOMHEADERID, List<IOMAppHeaderViewModel> AHVMList)
        {
            //// --- Delete recode ---////
            List<DGIT_IOMAPPHEADER> HeaderList = _IOMDBContext.DGIT_IOMAPPHEADER.Where(t => t.IOMHEADERID == IOMHEADERID).ToList();
            if (HeaderList.Count > 0)
            {
                _IOMDBContext.DGIT_IOMAPPHEADER.RemoveRange(HeaderList);
                _IOMDBContext.SaveChanges();
            }

            foreach (IOMAppHeaderViewModel AHVM in AHVMList.OrderBy(o => o.Seq_Order).ToList())
            {
                DGIT_IOMAPPHEADER IAH = new DGIT_IOMAPPHEADER();
                if (_IOMDBContext.DGIT_IOMAPPHEADER.Count() == 0)
                {
                    IAH.IOMAPPHEADERID = 1;
                }
                else
                {
                    IAH.IOMAPPHEADERID = _IOMDBContext.DGIT_IOMAPPHEADER.Max(x => x.IOMAPPHEADERID) + 1;
                }
                IAH.IOMHEADERID = IOMHEADERID;
                IAH.IOMAPPHEADER = AHVM.IOMAPPHEADER;
                IAH.APPHEADERDESC = AHVM.APPHEADERDESC;
                IAH.STATUS = 1;
                IAH.ADDEDBY = AddedBy;
                IAH.ADDEDDATE = DateTime.Now;
                _IOMDBContext.Entry(IAH).State = EntityState.Added;
                _IOMDBContext.SaveChanges();
            }
        }

        public Employee_Details GetAuthEmpById(long empCode, Employee_Details empDtl)
        {
            var _obj = (from data in _IOMDBContext.ADEMPLOYEE.Where(v => v.ADEMPCODE == empCode && v.ACTIVE == 1)
                        join _VW in _IOMDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VW.ADEMPCODE
                        join _Desg in _IOMDBContext.ADDESIGNATION on _VW.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
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

        //public IOMHeaderViewModel GetIOMRequestById(long id)
        //{
        //    var _obj = (from data in _IOMDBContext.DGIT_IOMHEADER.AsQueryable().AsNoTracking().Where(x => x.IOMHEADERID == id) //add keyword AsNoTracking()
        //                join _AddBy in _IOMDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
        //                join _VWA in _IOMDBContext.VW_ASSOCIATELVLDETAILS.AsQueryable().AsNoTracking().Where(m=>m.SYKI== _Syki.SYKIID) on data.ADDEDBY equals _VWA.ADEMPCODE into _vwdt
        //                from _VWAssociate in _vwdt.DefaultIfEmpty()
        //                join _cat in _IOMDBContext.DGIT_IOMCATMST on data.IOMCATID equals _cat.IOMCATMSTID into _calleft
        //                from _catldata in _calleft.DefaultIfEmpty()
        //                //where _VWAssociate.SYKI == _Syki.SYKIID
        //                select new IOMHeaderViewModel
        //                {
        //                    IOMHEADERID = data.IOMHEADERID,
        //                    IOMDesc = data.IOM_DESC,
        //                    APP_TYPE=data.APP_TYPE,
        //                    //Remark = data.REMARK,
        //                    ADDEDBYNAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
        //                    DATEADDED = data.DATEADDED,
        //                    PROCESS_STATUS=data.PROCESS_STATUS,
        //                    IOMCATMSTID=data.IOMCATID,
        //                    IOMCATDESC=_catldata.CATDESC,
        //                    ishighlighted=(_catldata==null?(short)0: _catldata.ISHIGHLIGHTED),
        //                    ADDEDBY=data.ADDEDBY,
        //                    iomDetail = (from _IOMDetail in _IOMDBContext.DGIT_IOMDETAIL.AsQueryable().AsNoTracking().Where(d => d.IOMHEADERID == data.IOMHEADERID)
        //                                 where _IOMDetail.STATUS == 1
        //                                 select new IOMDetailViewModel
        //                                 {
        //                                     IOMDTL_ID = _IOMDetail.IOMDTL_ID,
        //                                     IOMHEADERID = _IOMDetail.IOMHEADERID,
        //                                     DOC_TYPE = _IOMDetail.DOC_TYPE,
        //                                     ADDITIONAL_INFO = _IOMDetail.ADDITIONAL_INFO,
        //                                     FILENAME = _IOMDetail.FILENAME,
        //                                     ADDEDBY = _IOMDetail.ADDEDBY,    //  Added by Aumento 13072024
        //                                     ADDEDDATE = _IOMDetail.ADDEDDATE //  Added by Aumento 13072024
        //                                 }).ToList(),
        //                    Emp_Detail = new Employee_Details
        //                    {
        //                        _ECode = _AddBy.ADEMPCODE,
        //                        _EName = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
        //                        _EmailId = _AddBy.EMAILID,
        //                        _DOB = DateTime.Now,
        //                        _SecDescrip = _VWAssociate.SECTION,
        //                        _DepDesc = _VWAssociate.DEPARTMENT,
        //                        _DivDesc = _VWAssociate.DIVISION,
        //                        _OpDesc = _VWAssociate.OPERATION,
        //                        _SiteId = _VWAssociate.SYSITEID
        //                    },
        //                    iomAuthSeq = (from _POSeq in _IOMDBContext.DGIT_IOMAPPAUTHSEQ.AsQueryable().AsNoTracking().Where(x => x.IOMHEADERID == data.IOMHEADERID)
        //                                  join _AppSeqEmp in _IOMDBContext.ADEMPLOYEE on _POSeq.ADEMPCODE equals _AppSeqEmp.ADEMPCODE
        //                                  join _VWD in _IOMDBContext.VW_ASSOCIATELVLDETAILS.AsQueryable().AsNoTracking().Where(m=>m.SYKI== _Syki.SYKIID) on _POSeq.ADEMPCODE equals _VWD.ADEMPCODE into _VWTMP
        //                                  from _Vw in _VWTMP.DefaultIfEmpty()
        //                                  join _Desgt in _IOMDBContext.ADDESIGNATION on _Vw.ADDESIGNATIONID equals _Desgt.ADDESIGNATIONID into _desgtmp
        //                                  from _Desg in _desgtmp.DefaultIfEmpty()
        //                                  //where _Vw.SYKI == _Syki.SYKIID
        //                                  select new IOMAppAuthSeqViewModel
        //                                  {
        //                                      IOMAPPAUTH_ID = _POSeq.IOMAPPAUTH_ID,
        //                                      IOMID = _POSeq.IOMHEADERID,
        //                                      ADEMPCODE = _POSeq.ADEMPCODE,
        //                                      STATUS = _POSeq.STATUS,
        //                                      APP_SEQ = _POSeq.APP_SEQ,
        //                                      Header = _POSeq.IOMAPPHEADER,
        //                                      ADEMPNAME = _AppSeqEmp.FIRSTNAME + " " + _AppSeqEmp.LASTNAME,
        //                                      ADDESIGNATION = string.IsNullOrEmpty(_Desg.DESCRIP)?"(Inactive Employee)": _Desg.DESCRIP,
        //                                      ADDEDDATE = _POSeq.ADDEDDATE,
        //                                      UPDATEBY = _POSeq.UPDATEBY,
        //                                      UPDATEDATE = _POSeq.UPDATEDATE,
        //                                      APPTYPE = _POSeq.APPTYPE,
        //                                  }).OrderBy(b => b.APP_SEQ).ToList(),
        //                    iomAppHis = (from _POAppHis in _IOMDBContext.DGIT_IOMAPPHISTORY.AsQueryable().AsNoTracking().Where(x => x.IOMHEADERID == data.IOMHEADERID)
        //                                 join _AppEmp in _IOMDBContext.ADEMPLOYEE on _POAppHis.ADEMPCODE equals _AppEmp.ADEMPCODE
        //                                 select new IOMAppHistoryViewModel
        //                                 {
        //                                     IOMAPPHISTORY_ID = _POAppHis.IOMAPPHISTORY_ID,
        //                                     IOMID = _POAppHis.IOMHEADERID,
        //                                     ADEMPCODE = _POAppHis.ADEMPCODE,
        //                                     APPROVAL_STATUS = _POAppHis.APPROVAL_STATUS,
        //                                     APPROVAL_REMARK = _POAppHis.APPROVAL_REMARK,
        //                                     APPEMP_NAME = _AppEmp.FIRSTNAME + " " + _AppEmp.LASTNAME + " - [" + _AppEmp.ADEMPCODE + "]",
        //                                     APP_EMAIL = _AppEmp.EMAILID,
        //                                     ADDEDDATE = _POAppHis.ADDEDDATE,
        //                                     UPDATEBY = _POAppHis.UPDATEBY,
        //                                     UPDATEDATE = _POAppHis.UPDATEDATE,
        //                                     APPROVALDATE = _POAppHis.APP_DATE,
        //                                     APPTYPE=_POAppHis.APPTYPE
        //                                 }).OrderBy(o => o.IOMAPPHISTORY_ID).ToList(),
        //                    iomAppHeaderList = (from _AppHeader in _IOMDBContext.DGIT_IOMAPPHEADER.AsQueryable().AsNoTracking().Where(x => x.IOMHEADERID == data.IOMHEADERID)
        //                                        select new IOMAppHeaderViewModel
        //                                        {
        //                                            IOMAPPHEADERID = _AppHeader.IOMAPPHEADERID,
        //                                            IOMHEADERID = _AppHeader.IOMHEADERID,
        //                                            IOMAPPHEADER = _AppHeader.IOMAPPHEADER,
        //                                            APPHEADERDESC = _AppHeader.APPHEADERDESC,
        //                                            STATUS = _AppHeader.STATUS,
        //                                            ADDEDBY = _AppHeader.ADDEDBY,
        //                                            ADDEDDATE = _AppHeader.ADDEDDATE,
        //                                            UPDATEBY = _AppHeader.UPDATEBY,
        //                                            UPDATEDATE = _AppHeader.UPDATEDATE,
        //                                        }).OrderBy(b => b.IOMAPPHEADERID).ToList(),
        //                }).FirstOrDefault();

        //    if (_obj.iomAuthSeq.Count > 0)
        //    {
        //        long lastSendBackAppHisId = 0;
        //        DGIT_IOMAPPHISTORY lastSendBackAppHis = _IOMDBContext.DGIT_IOMAPPHISTORY.Where(e => e.IOMHEADERID == _obj.IOMHEADERID && e.APPROVAL_STATUS == 2).OrderByDescending(o => o.IOMAPPHISTORY_ID).FirstOrDefault();
        //        if (lastSendBackAppHis != null)
        //        {
        //            lastSendBackAppHisId = lastSendBackAppHis.IOMAPPHISTORY_ID;
        //        }
        //        foreach (IOMAppAuthSeqViewModel obj in _obj.iomAuthSeq)
        //        {
        //            bool IsBeforeSendBackRecord = _IOMDBContext.DGIT_IOMAPPHISTORY.Any(w => w.IOMHEADERID == obj.IOMID && w.IOMAPPHISTORY_ID <= lastSendBackAppHisId && w.ADEMPCODE == obj.ADEMPCODE);
        //            if ((IsBeforeSendBackRecord ? (!_obj.iomAppHis.Any(r => r.IOMID == obj.IOMID && r.ADEMPCODE == obj.ADEMPCODE && r.IOMAPPHISTORY_ID > lastSendBackAppHisId)) : (!_obj.iomAppHis.Any(x => x.IOMID == obj.IOMID && x.ADEMPCODE == obj.ADEMPCODE))))
        //            {
        //                _obj.iomAppHis.Add(new IOMAppHistoryViewModel
        //                {
        //                    IOMAPPHISTORY_ID = 0,
        //                    IOMID = obj.IOMID,
        //                    ADEMPCODE = obj.ADEMPCODE,
        //                    APPROVAL_STATUS = 0,
        //                    APPROVAL_REMARK = "",
        //                    APPEMP_NAME = obj.ADEMPNAME + "[" + obj.ADEMPCODE + "]",
        //                    APPTYPE=obj.APPTYPE,
        //                });
        //            }
        //        }
        //    }
        //    return _obj;
        //}
        public IOMHeaderViewModel GetIOMRequestById(long id)
        {
            /*Added by TTL against SR109950 > CR7268 - Start*/
            var ki = (from sk in _IOMDBContext.SYKI select new {
                sk.FINANCIALYEAR,
                sk.SYKIID
            }).ToList()
            .Select(x=> new
            {
                StartDt = DateTime.ParseExact($"01-Apr-{x.FINANCIALYEAR.Split('-')[0]}", "dd-MMM-yyyy", CultureInfo.InvariantCulture),
                EndDt = DateTime.ParseExact($"31-Mar-{x.FINANCIALYEAR.Split('-')[1]}", "dd-MMM-yyyy", CultureInfo.InvariantCulture),
                KiNum = x.SYKIID
            });
            var IOMDateAdded = _IOMDBContext.DGIT_IOMHEADER.FirstOrDefault(x => x.IOMHEADERID == id).DATEADDED.Date;
            var requestKI = ki.FirstOrDefault(x => IOMDateAdded >= x.StartDt && IOMDateAdded <= x.EndDt).KiNum;

            /*Added by TTL against SR109950 > CR7268 - End*/

            var _obj = (from data in _IOMDBContext.DGIT_IOMHEADER.Where(x => x.IOMHEADERID == id)
                        join _AddBy in _IOMDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                        join _VWA in _IOMDBContext.VW_ASSOCIATELVLDETAILS.Where(m => m.SYKI == requestKI) on data.ADDEDBY equals _VWA.ADEMPCODE into _vwdt
                        from _VWAssociate in _vwdt.DefaultIfEmpty()
                        join _cat in _IOMDBContext.DGIT_IOMCATMST on data.IOMCATID equals _cat.IOMCATMSTID into _calleft
                        from _catldata in _calleft.DefaultIfEmpty()
                            //where _VWAssociate.SYKI == _Syki.SYKIID
                        select new IOMHeaderViewModel
                        {
                            IOMHEADERID = data.IOMHEADERID,
                            IOMDesc = data.IOM_DESC,
                            APP_TYPE = data.APP_TYPE,
                            //Remark = data.REMARK,
                            ADDEDBYNAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                            DATEADDED = data.DATEADDED,
                            PROCESS_STATUS = data.PROCESS_STATUS,
                            IOMCATMSTID = data.IOMCATID,
                            IOMCATDESC = _catldata.CATDESC,
                            ishighlighted = (_catldata == null ? (short)0 : _catldata.ISHIGHLIGHTED),
                            ADDEDBY = data.ADDEDBY,
                            ARIBARFPID = data.ARIBARFPID, //Added by TTL on 18th July 2025 against SR101913 > CR6738
                            Emp_Detail = new Employee_Details
                            {
                                _ECode = _AddBy.ADEMPCODE,
                                _EName = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                                _EmailId = _AddBy.EMAILID ?? "",
                                _DOB = DateTime.Now,
                                _SecDescrip = _VWAssociate.SECTION,
                                _DepDesc = _VWAssociate.DEPARTMENT,
                                _DivDesc = _VWAssociate.DIVISION,
                                _OpDesc = _VWAssociate.OPERATION,
                                _SiteId = _VWAssociate.SYSITEID
                            }
                        }).FirstOrDefault();
            if (_obj != null)
            {
                _obj.iomDetail = (from _IOMDetail in _IOMDBContext.DGIT_IOMDETAIL.Where(d => d.IOMHEADERID == _obj.IOMHEADERID)
                                  where _IOMDetail.STATUS == 1
                                  select new IOMDetailViewModel
                                  {
                                      IOMDTL_ID = _IOMDetail.IOMDTL_ID,
                                      IOMHEADERID = _IOMDetail.IOMHEADERID,
                                      DOC_TYPE = _IOMDetail.DOC_TYPE,
                                      ADDITIONAL_INFO = _IOMDetail.ADDITIONAL_INFO,
                                      FILENAME = _IOMDetail.FILENAME,
                                      ADDEDBY = _IOMDetail.ADDEDBY,    //  Added by Aumento 13072024
                                      ADDEDDATE = _IOMDetail.ADDEDDATE //  Added by Aumento 13072024
                                  }).ToList();

                _obj.iomAuthSeq = (from _POSeq in _IOMDBContext.DGIT_IOMAPPAUTHSEQ.Where(x => x.IOMHEADERID == _obj.IOMHEADERID)
                                   join _AppSeqEmp in _IOMDBContext.ADEMPLOYEE on _POSeq.ADEMPCODE equals _AppSeqEmp.ADEMPCODE
                                   join _VWD in _IOMDBContext.VW_ASSOCIATELVLDETAILS.Where(m=> m.SYKI == requestKI) on _POSeq.ADEMPCODE equals _VWD.ADEMPCODE into _VWTMP
                                   from _Vw in _VWTMP.DefaultIfEmpty()
                                   join _Desgt in _IOMDBContext.ADDESIGNATION on _Vw.ADDESIGNATIONID equals _Desgt.ADDESIGNATIONID into _desgtmp
                                   from _Desg in _desgtmp.DefaultIfEmpty()
                                        //where _Vw.SYKI == _Syki.SYKIID
                                   select new { _POSeq, _AppSeqEmp, _Desg })
                                   .AsEnumerable()
                                   .Select(s => new IOMAppAuthSeqViewModel
                                   {
                                       IOMAPPAUTH_ID = s._POSeq.IOMAPPAUTH_ID,
                                       IOMID = s._POSeq.IOMHEADERID,
                                       ADEMPCODE = s._POSeq.ADEMPCODE,
                                       STATUS = s._POSeq.STATUS,
                                       APP_SEQ = s._POSeq.APP_SEQ,
                                       Header = s._POSeq.IOMAPPHEADER,
                                       ADEMPNAME = s._AppSeqEmp.FIRSTNAME + " " + s._AppSeqEmp.LASTNAME,
                                       ADDESIGNATION = string.IsNullOrEmpty(s._Desg?.DESCRIP) ? "(Inactive Employee)" : s._Desg.DESCRIP,
                                       ADDEDDATE = s._POSeq.ADDEDDATE,
                                       UPDATEBY = s._POSeq.UPDATEBY,
                                       UPDATEDATE = s._POSeq.UPDATEDATE,
                                       APPTYPE = s._POSeq.APPTYPE,
                                   }).OrderBy(b => b.APP_SEQ).ToList();

                _obj.iomAppHis = (from _POAppHis in _IOMDBContext.DGIT_IOMAPPHISTORY.Where(x => x.IOMHEADERID == _obj.IOMHEADERID)
                                  join _AppEmp in _IOMDBContext.ADEMPLOYEE on _POAppHis.ADEMPCODE equals _AppEmp.ADEMPCODE
                                  select new { _POAppHis, _AppEmp })
                                  .AsEnumerable()
                                  .Select(x => new IOMAppHistoryViewModel
                                  {
                                      IOMAPPHISTORY_ID = x._POAppHis.IOMAPPHISTORY_ID,
                                      IOMID = x._POAppHis.IOMHEADERID,
                                      ADEMPCODE = x._POAppHis.ADEMPCODE,
                                      APPROVAL_STATUS = x._POAppHis.APPROVAL_STATUS,
                                      APPROVAL_REMARK = x._POAppHis.APPROVAL_REMARK,
                                      APPEMP_NAME = x._AppEmp.FIRSTNAME + " " + x._AppEmp.LASTNAME + " - [" + x._AppEmp.ADEMPCODE + "]",
                                      APP_EMAIL = x._AppEmp.EMAILID ?? "",
                                      ADDEDDATE = x._POAppHis.ADDEDDATE,
                                      UPDATEBY = x._POAppHis.UPDATEBY,
                                      UPDATEDATE = x._POAppHis.UPDATEDATE,
                                      APPROVALDATE = x._POAppHis.APP_DATE,
                                      APPTYPE = x._POAppHis.APPTYPE
                                  }).OrderBy(o => o.IOMAPPHISTORY_ID).ToList();

                _obj.iomAppHeaderList = (from _AppHeader in _IOMDBContext.DGIT_IOMAPPHEADER.Where(x => x.IOMHEADERID == _obj.IOMHEADERID)
                                         select new IOMAppHeaderViewModel
                                         {
                                             IOMAPPHEADERID = _AppHeader.IOMAPPHEADERID,
                                             IOMHEADERID = _AppHeader.IOMHEADERID,
                                             IOMAPPHEADER = _AppHeader.IOMAPPHEADER,
                                             APPHEADERDESC = _AppHeader.APPHEADERDESC,
                                             STATUS = _AppHeader.STATUS,
                                             ADDEDBY = _AppHeader.ADDEDBY,
                                             ADDEDDATE = _AppHeader.ADDEDDATE,
                                             UPDATEBY = _AppHeader.UPDATEBY,
                                             UPDATEDATE = _AppHeader.UPDATEDATE,
                                         }).OrderBy(b => b.IOMAPPHEADERID).ToList();
                _obj.IndentNos = (
                                from _AppHeader in _IOMDBContext.DGIT_IOMHEADER.Where(x => x.IOMHEADERID == id)
                                join dpil in _IOMDBContext.DGIT_PR_IOM_LINKING on _AppHeader.IOMHEADERID equals dpil.DPIL_IOM_HDR_ID
                                select dpil.DPIL_INDENT_NO
                                ).ToList();
                _obj.PRDocDetails = GetPRDocumentsByIndentNo(_obj.IndentNos);
            }
            //var _obj = (from data in _IOMDBContext.DGIT_IOMHEADER.Where(x => x.IOMHEADERID == id)
            //            join _AddBy in _IOMDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
            //            join _VWA in _IOMDBContext.VW_ASSOCIATELVLDETAILS.Where(m => m.SYKI == _Syki.SYKIID) on data.ADDEDBY equals _VWA.ADEMPCODE into _vwdt
            //            from _VWAssociate in _vwdt.DefaultIfEmpty()
            //            join _cat in _IOMDBContext.DGIT_IOMCATMST on data.IOMCATID equals _cat.IOMCATMSTID into _calleft
            //            from _catldata in _calleft.DefaultIfEmpty()
            //                //where _VWAssociate.SYKI == _Syki.SYKIID
            //            select new IOMHeaderViewModel
            //            {
            //                IOMHEADERID = data.IOMHEADERID,
            //                IOMDesc = data.IOM_DESC,
            //                APP_TYPE = data.APP_TYPE,
            //                //Remark = data.REMARK,
            //                ADDEDBYNAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
            //                DATEADDED = data.DATEADDED,
            //                PROCESS_STATUS = data.PROCESS_STATUS,
            //                IOMCATMSTID = data.IOMCATID,
            //                IOMCATDESC = _catldata.CATDESC,
            //                ishighlighted = (_catldata == null ? (short)0 : _catldata.ISHIGHLIGHTED),
            //                ADDEDBY = data.ADDEDBY,
            //                iomDetail = (from _IOMDetail in _IOMDBContext.DGIT_IOMDETAIL.Where(d => d.IOMHEADERID == data.IOMHEADERID)
            //                             where _IOMDetail.STATUS == 1
            //                             select new IOMDetailViewModel
            //                             {
            //                                 IOMDTL_ID = _IOMDetail.IOMDTL_ID,
            //                                 IOMHEADERID = _IOMDetail.IOMHEADERID,
            //                                 DOC_TYPE = _IOMDetail.DOC_TYPE,
            //                                 ADDITIONAL_INFO = _IOMDetail.ADDITIONAL_INFO,
            //                                 FILENAME = _IOMDetail.FILENAME,
            //                                 ADDEDBY = _IOMDetail.ADDEDBY,    //  Added by Aumento 13072024
            //                                 ADDEDDATE = _IOMDetail.ADDEDDATE //  Added by Aumento 13072024
            //                             }).ToList(),
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
            //                iomAuthSeq = (from _POSeq in _IOMDBContext.DGIT_IOMAPPAUTHSEQ.Where(x => x.IOMHEADERID == data.IOMHEADERID)
            //                              join _AppSeqEmp in _IOMDBContext.ADEMPLOYEE on _POSeq.ADEMPCODE equals _AppSeqEmp.ADEMPCODE
            //                              join _VWD in _IOMDBContext.VW_ASSOCIATELVLDETAILS.Where(m => m.SYKI == _Syki.SYKIID) on _POSeq.ADEMPCODE equals _VWD.ADEMPCODE into _VWTMP
            //                              from _Vw in _VWTMP.DefaultIfEmpty()
            //                              join _Desgt in _IOMDBContext.ADDESIGNATION on _Vw.ADDESIGNATIONID equals _Desgt.ADDESIGNATIONID into _desgtmp
            //                              from _Desg in _desgtmp.DefaultIfEmpty()
            //                                  //where _Vw.SYKI == _Syki.SYKIID
            //                              select new IOMAppAuthSeqViewModel
            //                              {
            //                                  IOMAPPAUTH_ID = _POSeq.IOMAPPAUTH_ID,
            //                                  IOMID = _POSeq.IOMHEADERID,
            //                                  ADEMPCODE = _POSeq.ADEMPCODE,
            //                                  STATUS = _POSeq.STATUS,
            //                                  APP_SEQ = _POSeq.APP_SEQ,
            //                                  Header = _POSeq.IOMAPPHEADER,
            //                                  ADEMPNAME = _AppSeqEmp.FIRSTNAME + " " + _AppSeqEmp.LASTNAME,
            //                                  ADDESIGNATION = string.IsNullOrEmpty(_Desg.DESCRIP) ? "(Inactive Employee)" : _Desg.DESCRIP,
            //                                  ADDEDDATE = _POSeq.ADDEDDATE,
            //                                  UPDATEBY = _POSeq.UPDATEBY,
            //                                  UPDATEDATE = _POSeq.UPDATEDATE,
            //                                  APPTYPE = _POSeq.APPTYPE,
            //                              }).OrderBy(b => b.APP_SEQ).ToList(),
            //                iomAppHis = (from _POAppHis in _IOMDBContext.DGIT_IOMAPPHISTORY.Where(x => x.IOMHEADERID == data.IOMHEADERID)
            //                             join _AppEmp in _IOMDBContext.ADEMPLOYEE on _POAppHis.ADEMPCODE equals _AppEmp.ADEMPCODE
            //                             select new IOMAppHistoryViewModel
            //                             {
            //                                 IOMAPPHISTORY_ID = _POAppHis.IOMAPPHISTORY_ID,
            //                                 IOMID = _POAppHis.IOMHEADERID,
            //                                 ADEMPCODE = _POAppHis.ADEMPCODE,
            //                                 APPROVAL_STATUS = _POAppHis.APPROVAL_STATUS,
            //                                 APPROVAL_REMARK = _POAppHis.APPROVAL_REMARK,
            //                                 APPEMP_NAME = _AppEmp.FIRSTNAME + " " + _AppEmp.LASTNAME + " - [" + _AppEmp.ADEMPCODE + "]",
            //                                 APP_EMAIL = _AppEmp.EMAILID,
            //                                 ADDEDDATE = _POAppHis.ADDEDDATE,
            //                                 UPDATEBY = _POAppHis.UPDATEBY,
            //                                 UPDATEDATE = _POAppHis.UPDATEDATE,
            //                                 APPROVALDATE = _POAppHis.APP_DATE,
            //                                 APPTYPE = _POAppHis.APPTYPE
            //                             }).OrderBy(o => o.IOMAPPHISTORY_ID).ToList(),
            //                iomAppHeaderList = (from _AppHeader in _IOMDBContext.DGIT_IOMAPPHEADER.Where(x => x.IOMHEADERID == data.IOMHEADERID)
            //                                    select new IOMAppHeaderViewModel
            //                                    {
            //                                        IOMAPPHEADERID = _AppHeader.IOMAPPHEADERID,
            //                                        IOMHEADERID = _AppHeader.IOMHEADERID,
            //                                        IOMAPPHEADER = _AppHeader.IOMAPPHEADER,
            //                                        APPHEADERDESC = _AppHeader.APPHEADERDESC,
            //                                        STATUS = _AppHeader.STATUS,
            //                                        ADDEDBY = _AppHeader.ADDEDBY,
            //                                        ADDEDDATE = _AppHeader.ADDEDDATE,
            //                                        UPDATEBY = _AppHeader.UPDATEBY,
            //                                        UPDATEDATE = _AppHeader.UPDATEDATE,
            //                                    }).OrderBy(b => b.IOMAPPHEADERID).ToList(),
            //            }).FirstOrDefault();

            if (_obj.iomAuthSeq.Count > 0)
            {
                long lastSendBackAppHisId = 0;
                //Added by TTL on 26-June-2025 against SR101846 > CR6625 - Start
                //DGIT_IOMAPPHISTORY lastSendBackAppHis = _IOMDBContext.DGIT_IOMAPPHISTORY.Where(e => e.IOMHEADERID == _obj.IOMHEADERID && e.APPROVAL_STATUS == 2).OrderByDescending(o => o.IOMAPPHISTORY_ID).FirstOrDefault();
                DGIT_IOMAPPHISTORY lastSendBackAppHis = (from h in _obj.iomAppHis
                                                         where h.IOMID == _obj.IOMHEADERID && h.APPROVAL_STATUS == 2
                                                         orderby h.IOMAPPHISTORY_ID descending
                                                         select new DGIT_IOMAPPHISTORY
                                                         {
                                                             ADEMPCODE = h.ADEMPCODE,
                                                             IOMAPPHISTORY_ID = h.IOMAPPHISTORY_ID,
                                                             ADDEDBY = h.ADDEDBY,
                                                             ADDEDDATE = h.ADDEDDATE,
                                                             APPROVAL_REMARK = h.APPROVAL_REMARK,
                                                             APPROVAL_STATUS = h.APPROVAL_STATUS,
                                                             APPTYPE = h.APPTYPE,
                                                             APP_DATE = h.APPROVALDATE,
                                                             IOMHEADERID = h.IOMID,
                                                             UPDATEBY = h.UPDATEBY,
                                                             UPDATEDATE = h.UPDATEDATE
                                                         }).FirstOrDefault();
                //Added by TTL on 26-June-2025 against SR101846 > CR6625 - End
                if (lastSendBackAppHis != null)
                {
                    lastSendBackAppHisId = lastSendBackAppHis.IOMAPPHISTORY_ID;
                }
                foreach (IOMAppAuthSeqViewModel obj in _obj.iomAuthSeq)
                {
                    //Added by TTL on 26-June-2025 against SR101846 > CR6625 - Start
                    //bool IsBeforeSendBackRecord = _IOMDBContext.DGIT_IOMAPPHISTORY.Any(w => w.IOMHEADERID == obj.IOMID && w.IOMAPPHISTORY_ID <= lastSendBackAppHisId && w.ADEMPCODE == obj.ADEMPCODE); //Commented by TTL on 26-June-2025 against SR101846 > CR6625
                    bool IsBeforeSendBackRecord = (from h in _obj.iomAppHis
                                                   where h.IOMID == _obj.IOMHEADERID && h.IOMAPPHISTORY_ID <= lastSendBackAppHisId && h.ADEMPCODE == obj.ADEMPCODE
                                                   select new DGIT_IOMAPPHISTORY
                                                   {
                                                       IOMAPPHISTORY_ID = h.IOMAPPHISTORY_ID
                                                   }).Any();
                    //Added by TTL on 26-June-2025 against SR101846 > CR6625 - End
                    if ((IsBeforeSendBackRecord ? (!_obj.iomAppHis.Any(r => r.IOMID == obj.IOMID && r.ADEMPCODE == obj.ADEMPCODE && r.IOMAPPHISTORY_ID > lastSendBackAppHisId)) : (!_obj.iomAppHis.Any(x => x.IOMID == obj.IOMID && x.ADEMPCODE == obj.ADEMPCODE))))
                    {
                        _obj.iomAppHis.Add(new IOMAppHistoryViewModel
                        {
                            IOMAPPHISTORY_ID = 0,
                            IOMID = obj.IOMID,
                            ADEMPCODE = obj.ADEMPCODE,
                            APPROVAL_STATUS = 0,
                            APPROVAL_REMARK = "",
                            APPEMP_NAME = obj.ADEMPNAME + "[" + obj.ADEMPCODE + "]",
                            APPTYPE = obj.APPTYPE,
                        });
                    }
                }
            }

            return _obj;
        }
        //Hold to Pending Approval Document - Added by Bhupesh NTT for CR-5233 
        public short SaveApprovalHoldToPending(IOMAppHistoryViewModel PHVM)
        {
            int FlagAdd = 0;
            short retVal = 0;
            using (var transaction = _IOMDBContext.Database.BeginTransaction())
            {
                try
                {
                    //Check if Approval Note is already pending with the approver - Changing due to duplicate approvals adding in history with 0 approval status
                    //Changed by TTL on 14-June-2025 against SR99343 > CR6531 - Start
                    bool isAlreadyPending = IsAlreadyPending(PHVM.IOMID, PHVM.ADEMPCODE);
                    if (isAlreadyPending)
                    {
                        return -1;
                    }
                    //Changed by TTL on 14-June-2025 against SR99343 > CR6531 - End
                    DGIT_IOMAPPHISTORY DPAH = new DGIT_IOMAPPHISTORY();
                    if (PHVM.IOMID > 0)
                    {
                        FlagAdd = 1;
                        DPAH.IOMAPPHISTORY_ID = _IOMDBContext.DGIT_IOMAPPHISTORY.Max(x => x.IOMAPPHISTORY_ID) + 1;
                        DPAH.IOMHEADERID = PHVM.IOMID;
                        DPAH.ADEMPCODE = PHVM.ADEMPCODE;
                        DPAH.APPTYPE = PHVM.APPTYPE;
                        DPAH.APPROVAL_STATUS = 0;
                        DPAH.APPROVAL_REMARK = "";
                        if (FlagAdd == 1)
                        {
                            DPAH.ADDEDBY = PHVM.ADDEDBY;
                            DPAH.ADDEDDATE = DateTime.Now;
                        }
                        else
                        {
                            DPAH.UPDATEBY = PHVM.UPDATEBY;
                            DPAH.UPDATEDATE = DateTime.Now;
                        }
                        _IOMDBContext.Entry(DPAH).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                        _IOMDBContext.SaveChanges();
                        DGIT_IOMHEADER DPH = new DGIT_IOMHEADER();
                        DPH = _IOMDBContext.DGIT_IOMHEADER.Where(x => x.IOMHEADERID == PHVM.IOMID).SingleOrDefault();
                        DPH.PROCESS_STATUS = 1;
                        DPH.UPDATEDBY = PHVM.UPDATEBY;
                        DPH.UPDATEDATE = DateTime.Now;
                        _IOMDBContext.Entry(DPH).State = EntityState.Modified;
                        _IOMDBContext.SaveChanges();
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
        public short IOMApproval(IOMAppHistoryViewModel PHVM)
        {
            short retVal = 0;
            using (var transaction = _IOMDBContext.Database.BeginTransaction())
            {
                try
                {
                    DGIT_IOMAPPHISTORY DPAH = new DGIT_IOMAPPHISTORY();
                    int DPAH_Approval_Status = -1;
                    if (PHVM.IOMID > 0)
                    {
                        /////////// Update Approval Status //////////
                        //DPAH = _IOMDBContext.DGIT_IOMAPPHISTORY.Where(x => x.IOMHEADERID == PHVM.IOMID && x.ADEMPCODE == PHVM.ADEMPCODE && x.APPROVAL_STATUS == 0).FirstOrDefault(); //   Commented by Aumento 12072024

                        DPAH = _IOMDBContext.DGIT_IOMAPPHISTORY.Where(x => x.IOMHEADERID == PHVM.IOMID && x.ADEMPCODE == PHVM.ADEMPCODE && (x.APPROVAL_STATUS == 0 || x.APPROVAL_STATUS == 5)).OrderByDescending(x => x.IOMAPPHISTORY_ID).FirstOrDefault(); // { || x.APPROVAL_STATUS == 5}  Updated by Aumento 12072024
                        DPAH_Approval_Status = DPAH != null ? DPAH.APPROVAL_STATUS : DPAH_Approval_Status;
                        if (DPAH != null && DPAH.APPROVAL_STATUS == 0)
                        {
                            DPAH.APPROVAL_STATUS = PHVM.APPROVAL_STATUS;
                            DPAH.APPROVAL_REMARK = PHVM.APPROVAL_REMARK;
                            DPAH.UPDATEBY = PHVM.UPDATEBY;
                            DPAH.APP_DATE = DateTime.Now;
                            DPAH.UPDATEDATE = DateTime.Now;
                            _IOMDBContext.Entry(DPAH).State = EntityState.Modified;
                            _IOMDBContext.SaveChanges();
                        }

                        if (DPAH != null && DPAH_Approval_Status == 5 && (PHVM.APPROVAL_STATUS == 5 || PHVM.APPROVAL_STATUS == 1 || PHVM.APPROVAL_STATUS == 2 || PHVM.APPROVAL_STATUS == 3))
                        {
                            short Seq = Convert.ToInt16(_IOMDBContext.DGIT_IOMAPPAUTHSEQ.Where(s => s.IOMHEADERID == PHVM.IOMID && s.ADEMPCODE == PHVM.ADEMPCODE).Select(s => s.APP_SEQ).FirstOrDefault());
                            var seqModel_ = (from data in _IOMDBContext.DGIT_IOMAPPAUTHSEQ.Where(a => a.APP_SEQ == Seq && a.IOMHEADERID == PHVM.IOMID)
                                             select new IOMAppAuthSeqViewModel
                                             {
                                                 IOMAPPAUTH_ID = data.IOMAPPAUTH_ID,
                                                 IOMID = data.IOMHEADERID,
                                                 ADEMPCODE = data.ADEMPCODE,
                                                 APP_SEQ = data.APP_SEQ,
                                                 Header = data.IOMAPPHEADER,
                                                 STATUS = data.STATUS,
                                                 APPTYPE = data.APPTYPE
                                             }).FirstOrDefault();

                            SaveIOMAppHisApproval(PHVM.ADDEDBY, PHVM.IOMID, seqModel_, PHVM.APPROVAL_REMARK, PHVM.APPROVAL_STATUS);

                        }

                        /// --- Save PO Approval Authority --- ///
                        IOMAppAuthSeqViewModel seqModel = new IOMAppAuthSeqViewModel();
                        if (PHVM.APPROVAL_STATUS == 1)
                        {
                            short nextSeq = Convert.ToInt16(_IOMDBContext.DGIT_IOMAPPAUTHSEQ.Where(s => s.IOMHEADERID == PHVM.IOMID && s.ADEMPCODE == PHVM.ADEMPCODE).Select(s => s.APP_SEQ).FirstOrDefault() + 1);
                            seqModel = (from data in _IOMDBContext.DGIT_IOMAPPAUTHSEQ.Where(a => a.APP_SEQ == nextSeq && a.IOMHEADERID == PHVM.IOMID)
                                        select new IOMAppAuthSeqViewModel
                                        {
                                            IOMAPPAUTH_ID = data.IOMAPPAUTH_ID,
                                            IOMID = data.IOMHEADERID,
                                            ADEMPCODE = data.ADEMPCODE,
                                            APP_SEQ = data.APP_SEQ,
                                            Header = data.IOMAPPHEADER,
                                            STATUS = data.STATUS,
                                            APPTYPE = data.APPTYPE
                                        }).FirstOrDefault();
                            if (seqModel != null)
                            {
                                SaveIOMAppHis(PHVM.ADDEDBY, PHVM.IOMID, seqModel);
                            }
                        }

                        //////// Update Process Status ////////
                        DGIT_IOMHEADER DPH = new DGIT_IOMHEADER(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                        DPH = _IOMDBContext.DGIT_IOMHEADER.Where(x => x.IOMHEADERID == PHVM.IOMID).SingleOrDefault();
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
                            //  -> Added by Aumento 12072024
                            else if (PHVM.APPROVAL_STATUS == 5)
                            {
                                DPH.PROCESS_STATUS = 5;
                            }
                            //  <- Added by Aumento 12072024
                            else
                            {
                                DPH.PROCESS_STATUS = 1;
                            }
                            DPH.UPDATEDBY = PHVM.UPDATEBY;
                            DPH.UPDATEDATE = DateTime.Now;
                            _IOMDBContext.Entry(DPH).State = EntityState.Modified;
                            _IOMDBContext.SaveChanges();
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

        public short IOMCancel(IOMHeaderViewModel PHVM)
        {
            short retVal = 0;
            using (var transaction = _IOMDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (PHVM.IOMHEADERID > 0)
                    {
                        //////// Update Process Status ////////
                        DGIT_IOMHEADER DPH = new DGIT_IOMHEADER(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                        DPH = _IOMDBContext.DGIT_IOMHEADER.Where(x => x.IOMHEADERID == PHVM.IOMHEADERID).SingleOrDefault();
                        if (DPH != null)
                        {
                            DPH.PROCESS_STATUS = 4;
                            DPH.UPDATEDBY = PHVM.UPDATEDBY;
                            DPH.UPDATEDATE = DateTime.Now;
                            _IOMDBContext.Entry(DPH).State = EntityState.Modified;
                            _IOMDBContext.SaveChanges();
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

        public List<IOMDetailViewModel> GetAttachmentDetail(long _IOMHEADERID)
        {
            return (from _IOMDetail in _IOMDBContext.DGIT_IOMDETAIL.Where(d => d.IOMHEADERID == _IOMHEADERID)
                    where _IOMDetail.STATUS == 1
                    select new IOMDetailViewModel
                    {
                        IOMDTL_ID = _IOMDetail.IOMDTL_ID,
                        IOMHEADERID = _IOMDetail.IOMHEADERID,
                        DOC_TYPE = _IOMDetail.DOC_TYPE,
                        ADDITIONAL_INFO = _IOMDetail.ADDITIONAL_INFO,
                        FILENAME = _IOMDetail.FILENAME,
                    }).ToList();
        }

        public short DeleteAttachment(string fileName, string docType, long IOMHEADERID)
        {
            short retVal = 0;
            if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(docType) && IOMHEADERID > 0)
            {
                DGIT_IOMDETAIL DT = _IOMDBContext.DGIT_IOMDETAIL.Where(x => x.FILENAME == fileName && x.DOC_TYPE == docType && x.IOMHEADERID == IOMHEADERID).FirstOrDefault();
                if (DT != null)
                {
                    _IOMDBContext.DGIT_IOMDETAIL.Remove(DT);
                    _IOMDBContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }

        public List<Employee_Details> PortalAutocompleteSuggestions(string Key, string designation)
        {
            int isSearchDesg = 0;
            //bool isNumeric = int.TryParse(Key, out n);
            //long _value = (isNumeric ? Convert.ToInt64(Key) : 0);
            List<Employee_Details> portalUserDtos = new List<Employee_Details>();
            try
            {
                var oDesg = _IOMDBContext.ADDESIGNATION.Where(m => m.ACTIVE == 1 && m.DESCRIP.ToUpper() == designation.ToUpper()).ToList();
                //var oFDesg = _IOMDBContext.ADFUNCTIONALDESIGNATION.Where(m => m.ACTIVE == 1 && m.DESCRIP.ToUpper().Contains(designation.ToUpper())).ToList();
                var oFDesg = _IOMDBContext.ADFUNCTIONALDESIGNATION.Where(m => m.ACTIVE == 1 && !string.IsNullOrEmpty(designation) && m.DESCRIP.ToUpper().Contains(designation.ToUpper())).ToList();

                if (oDesg.Count > 0 || oFDesg.Count > 0)
                {
                    isSearchDesg = 1;
                }
                long strKIID = (long)_IOMDBContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
                var empdata = (from userdata in _IOMDBContext.ADEMPDIVDEPTSECT.Where(m => m.SYKI == strKIID)
                               join emp in _IOMDBContext.ADEMPLOYEE.Where(m => m.ACTIVE == 1) on userdata.ADEMPCODE equals emp.ADEMPCODE
                               join d in _IOMDBContext.ADDESIGNATION.Where(m => m.ACTIVE == 1) on userdata.ADDESIGNATIONID equals d.ADDESIGNATIONID
                               join fg in _IOMDBContext.ADFUNCTIONALDESIGNATION.Where(m => m.ACTIVE == 1) on userdata.ADFUNCTIONALDESIGNATIONID equals fg.ADFUNCTIONALDESIGNATIONID into ls
                               from fg in ls.DefaultIfEmpty()
                               //where (isSearchDesg == 1 ? (fg.DESCRIP.ToUpper().Contains(designation.ToUpper()) || d.DESCRIP.ToUpper() == designation.ToUpper()) : true)
                               where isSearchDesg != 1 || fg.DESCRIP.ToUpper().Contains(designation.ToUpper()) || d.DESCRIP.ToUpper() == designation.ToUpper()
                               select new
                               {
                                   ADEMPCODE = emp.ADEMPCODE,
                                   FIRSTNAME = emp.FIRSTNAME,
                                   LASTNAME = emp.LASTNAME,
                                   _ENAME = emp.FIRSTNAME + " " + emp.LASTNAME,
                               }
                               ).ToList();

                portalUserDtos = (from userdata in empdata
                                  where (userdata.ADEMPCODE.ToString().StartsWith(Key) || userdata.FIRSTNAME.ToUpper().Contains(Key.ToUpper()) || userdata.LASTNAME.ToUpper().Contains(Key.ToUpper()) || (userdata._ENAME.ToUpper()).Contains(Key.ToUpper()))
                                  select new Employee_Details
                                  {
                                      _ECode = userdata.ADEMPCODE,
                                      _EFirstName = userdata.FIRSTNAME,
                                      _ELastName = userdata.LASTNAME,
                                  }).ToList();
                //foreach (var portaluser in portalUserequery)
                //{
                //    portalUserDtos.Add(Mapper.Map<PORTALUSER, PortalUser>(portaluser));
                //}

            }
            catch (Exception ex)
            {
                portalUserDtos = null;
                throw;
            }
            return portalUserDtos;
        }
        public List<Employee_Details> AutocompleteDesignation(string Key)
        {
            List<Employee_Details> portalUserDtos = new List<Employee_Details>();
            portalUserDtos = (from data in _IOMDBContext.ADDESIGNATION
                              where (data.DESCRIP.ToUpper().Contains(Key.ToUpper())) && data.ACTIVE == 1
                              select new Employee_Details
                              {
                                  _Desig = data.DESCRIP,
                              }).ToList();

            portalUserDtos.AddRange((from data in _IOMDBContext.ADFUNCTIONALDESIGNATION
                                     where (data.DESCRIP.ToUpper().Contains(Key.ToUpper())) && data.ACTIVE == 1
                                     select new Employee_Details
                                     {
                                         _Desig = data.DESCRIP,
                                     }).ToList());
            return portalUserDtos;
        }

        public long GetIOMNextApprovalId(long IOMID, long ecode)
        {
            //Changed by TTL to maintain uniformity and sync between pending approval list. #CR7675
            long retval = 0;
            try
            {
                using var cmd = _IOMDBContext.Database.GetDbConnection().CreateCommand();
                cmd.CommandText = "PKG_POREQUEST.SPROC_AD_PENDINGIOMAPPROVAL";
                cmd.CommandType = CommandType.StoredProcedure;

                var oracleCmd = (OracleCommand)cmd;
                oracleCmd.BindByName = true;

                oracleCmd.Parameters.Add("SUPEMPCODE_IN", OracleDbType.Int32).Value = ecode;
                oracleCmd.Parameters.Add("CUR_PENDINGAPPROVAL", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                if (oracleCmd.Connection.State != ConnectionState.Open)
                    oracleCmd.Connection.Open();

                var ds = new DataSet();
                using var da = new OracleDataAdapter(oracleCmd);
                da.Fill(ds);

                DataTable dtApprovals = ds.Tables[0];
                if (dtApprovals != null & dtApprovals.Rows.Count > 0)
                {
                    // Find the target row
                    DataRow foundRow = dtApprovals.AsEnumerable()
                                         .FirstOrDefault(r => r.Field<long>("IOMHEADERID") == IOMID);

                    if (foundRow != null)
                    {
                        // Get the index of the found row
                        int currentIndex = dtApprovals.Rows.IndexOf(foundRow);

                        // Check if there is a next row available
                        if (currentIndex < dtApprovals.Rows.Count - 1)
                        {
                            DataRow nextRow = dtApprovals.Rows[currentIndex + 1];
                            // You now have the next row
                            retval = nextRow.Field<long>("IOMHEADERID");
                        }
                        else
                        {
                            retval = 0;
                        }
                    }
                    else
                    {
                        retval = 0;
                    }
                }
                else
                {
                    retval = 0;
                }
            }
            catch
            {
                retval = 0;
                return retval;
            }

            /*
            int iscurrhigh = (from _PO in _IOMDBContext.DGIT_IOMHEADER.Where(m => m.IOMHEADERID == IOMID)
                              join _CAT in _IOMDBContext.DGIT_IOMCATMST on _PO.IOMCATID equals _CAT.IOMCATMSTID into _catTMP
                              from _catd in _catTMP.DefaultIfEmpty()
                              select new
                              {
                                  ISHIGHLIGHTED = (_catd == null ? 0 : _catd.ISHIGHLIGHTED)
                              }
                           ).FirstOrDefault().ISHIGHLIGHTED;

            var _obj = (from _PO in _IOMDBContext.DGIT_IOMHEADER
                        join _PAH in _IOMDBContext.DGIT_IOMAPPHISTORY on _PO.IOMHEADERID equals _PAH.IOMHEADERID
                        join _CAT in _IOMDBContext.DGIT_IOMCATMST on _PO.IOMCATID equals _CAT.IOMCATMSTID into _catTMP
                        from _catd in _catTMP.DefaultIfEmpty()
                        where _PO.PROCESS_STATUS == 1 && _PAH.APPROVAL_STATUS == 0 && _PAH.ADEMPCODE == ecode
                        select new
                        {
                            _PO.IOMHEADERID,
                            _PO.DATEADDED,
                            ISHIGHLIGHTED = (_catd == null ? 0 : _catd.ISHIGHLIGHTED)
                        }
                        ).OrderByDescending(M => M.ISHIGHLIGHTED).ThenBy(p => p.DATEADDED);

            if (_obj != null && _obj.Count() > 0)
            {
                if (iscurrhigh == 1)
                {
                    var lsthigh = _obj.Where(m => m.ISHIGHLIGHTED == 1 && m.IOMHEADERID > IOMID);
                    if (lsthigh.Count() > 0)
                    {
                        retval = lsthigh.FirstOrDefault().IOMHEADERID;

                    }
                    else
                    {
                        var lstnonhigh = _obj.Where(m => m.ISHIGHLIGHTED == 0);
                        retval = lstnonhigh.FirstOrDefault().IOMHEADERID;
                    }
                }
                if (iscurrhigh == 0)
                {
                    var lst = _obj.ToList();
                    var lstnonhigh = _obj.Where(m => m.ISHIGHLIGHTED == 0 && m.IOMHEADERID > IOMID);
                    retval = lstnonhigh.FirstOrDefault().IOMHEADERID;
                }
                //retval = _obj.FirstOrDefault().IOMHEADERID; //Return 1 if any of PR approval not finished for related PO
            } */
            return retval;
        }
        //Changed by TTL on 18th July 2025 against SR101913 > CR6738 - Start
        public List<VM_DGIT_IOMCATMST> BindIOMCategory(string divisionId = "")
        {
            SYPARAMETERS objParams = new SYPARAMETERS();
            List<string> divisionIds = new List<string>();
            if (!string.IsNullOrEmpty(divisionId))
            {
                objParams = _IOMDBContext.SYPARAMETERS.Where(x => x.PARAMNAME == "IOM_CAPT_SNC").FirstOrDefault();
                //divisionIds = objParams.PARAMVALUE.Split(',').Select(x => x.Trim()).ToList();
            }
            List<VM_DGIT_IOMCATMST> objCategories =  (from Odata in _IOMDBContext.DGIT_IOMCATMST
                    where Odata.STATUS == 1
                    select new VM_DGIT_IOMCATMST
                    {
                        CATDESC = Odata.CATDESC,
                        IOMCATMSTID = Odata.IOMCATMSTID,
                    }).ToList();
            if (!divisionIds.Contains(divisionId)) //Show Capital Sancation category to specific divisions only which have been added in SYPARAMETERS Table
            {
                objCategories.Remove(objCategories.Where(x => x.IOMCATMSTID == 4).FirstOrDefault());
            }
            return objCategories;
        }
        //Changed by TTL on 18th July 2025 against SR101913 > CR6738 - Start


        public Tuple<short, long> SaveAdditionalIOMRequest(IOMHeaderViewModel model)
        {
            short retVal = 0; long retHeaderId = 0;
            Tuple<short, long> _retVal_tuple;
            using (var transaction = _IOMDBContext.Database.BeginTransaction())
            {
                try
                {
                    DGIT_IOMHEADER DPH = new DGIT_IOMHEADER();
                    if (model.IOMHEADERID > 0)
                    {
                        DPH = _IOMDBContext.DGIT_IOMHEADER.Where(x => x.IOMHEADERID == model.IOMHEADERID).SingleOrDefault();
                    }

                    DPH.PROCESS_STATUS = model.PROCESS_STATUS;
                    DPH.UPDATEDBY = model.UPDATEDBY;
                    DPH.UPDATEDATE = DateTime.Now;

                    _IOMDBContext.Entry(DPH).State = EntityState.Modified;
                    _IOMDBContext.SaveChanges();

                    /// --- Save IOM Approval Auth Seq ---///
                    if (model.iomAuthSeq != null)
                    {
                        if (model.iomAuthSeq.Count > 0)
                        {
                            IOMAppAuthSeqViewModel seqModel = SaveAdditionalAppAuthSeq(model.ADDEDBY, DPH.IOMHEADERID, model.iomAuthSeq);

                            /// --- Save IOM Approval Authority --- ///
                            //IOMAppAuthSeqViewModel seqModel = model.iomAuthSeq.OrderBy(o => o.APP_SEQ).FirstOrDefault();
                            if (seqModel != null || seqModel.IOMID != 0)
                            {
                                SaveIOMAppHis(model.ADDEDBY, DPH.IOMHEADERID, seqModel);
                            }
                        }
                    }

                    transaction.Commit();
                    retVal = 1;
                    retHeaderId = DPH.IOMHEADERID;
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Authority already exist in approval.")
                    {
                        retVal = -2;
                    }
                    else
                    {
                        retVal = -1;
                    }
                    transaction.Rollback();
                }
                _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId);
                return _retVal_tuple;
            }
        }

        public IOMAppAuthSeqViewModel SaveAdditionalAppAuthSeq(long AddedBy, long IOMHEADERID, List<IOMAppAuthSeqViewModel> PSVMList)
        {
            //// --- Delete recode ---////
            IOMAppAuthSeqViewModel objret = new IOMAppAuthSeqViewModel();
            List<DGIT_IOMAPPAUTHSEQ> SeqList = _IOMDBContext.DGIT_IOMAPPAUTHSEQ.Where(t => t.IOMHEADERID == IOMHEADERID).ToList();
            short maxappseq = (short)(SeqList.Max(m => m.APP_SEQ) + 1);
            short maxapptype = (short)(SeqList.Max(m => m.APPTYPE) + 1);
            foreach (IOMAppAuthSeqViewModel PSVM in PSVMList.OrderBy(o => o.APP_SEQ).ToList())
            {
                if (SeqList.Where(m => m.ADEMPCODE == PSVM.ADEMPCODE).Count() > 0)
                {
                    throw new Exception("Authority already exist in approval.");
                }
                DGIT_IOMAPPAUTHSEQ DAAS = new DGIT_IOMAPPAUTHSEQ();
                if (_IOMDBContext.DGIT_IOMAPPAUTHSEQ.Count() == 0)
                {
                    DAAS.IOMAPPAUTH_ID = 1;
                }
                else
                {
                    DAAS.IOMAPPAUTH_ID = _IOMDBContext.DGIT_IOMAPPAUTHSEQ.Max(x => x.IOMAPPAUTH_ID) + 1;
                }
                DAAS.IOMHEADERID = IOMHEADERID;
                DAAS.IOMAPPHEADER = PSVM.Header;
                DAAS.ADEMPCODE = PSVM.ADEMPCODE;
                DAAS.APP_SEQ = maxappseq;
                PSVM.APP_SEQ = maxappseq;
                maxappseq += 1;
                DAAS.APPTYPE = maxapptype;//PSVM.APPTYPE;
                PSVM.APPTYPE = maxapptype;
                DAAS.STATUS = 1;
                DAAS.ADDEDBY = AddedBy;
                DAAS.ADDEDDATE = DateTime.Now;
                _IOMDBContext.Entry(DAAS).State = EntityState.Added;
                _IOMDBContext.SaveChanges();
                if (objret.IOMID == 0)
                {
                    objret = PSVM;
                    objret.IOMID = IOMHEADERID;
                }
            }

            return objret;
        }

        //Below button added by aumento as 15062023 for SR50547------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public List<IOMAppAuthSeqViewModel> GetPrevAuthority(string AppType, long Adempcode)
        {
            Int64 max = 0;
            int intapptype = Convert.ToInt16(AppType);
            // max = Int32.Parse(_IOMDBContext.DGIT_IOMHEADER.Where(i=>i.ADDEDBY== Adempcode && i.STATUS==1 && (i.PROCESS_STATUS== 1 || i.PROCESS_STATUS == 2) && i.APP_TYPE== Int32.Parse(AppType)).Max(i => i.IOMHEADERID).ToString());
            var objmax = (from data in _IOMDBContext.DGIT_IOMHEADER.Where(i => i.ADDEDBY == Adempcode && i.STATUS == 1 && (i.PROCESS_STATUS == 1 || i.PROCESS_STATUS == 2))
                          select new
                          {
                              data.IOMHEADERID,
                              data.APP_TYPE,
                          }
                   ).Where(m => m.APP_TYPE == intapptype);
            if (objmax.Count() > 0)
            {
                max = objmax.Max(m => m.IOMHEADERID);
            }
            var _obj = (from data in _IOMDBContext.DGIT_IOMAPPAUTHSEQ.Where(x => x.IOMHEADERID == max)
                        join _AddBy in _IOMDBContext.ADEMPLOYEE on data.ADEMPCODE equals _AddBy.ADEMPCODE
                        join _VW in _IOMDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VW.ADEMPCODE
                        join _Desg in _IOMDBContext.ADDESIGNATION on _VW.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
                        where _VW.SYKI == _Syki.SYKIID
                        select new IOMAppAuthSeqViewModel
                        {
                            Header = data.IOMAPPHEADER,
                            ADEMPCODE = data.ADEMPCODE,
                            APP_SEQ = data.APP_SEQ,
                            ADEMPNAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                            ADDESIGNATION = _Desg.DESCRIP,
                        }).ToList();
            return _obj;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public SearchIOM IOMUserReport(SearchIOM VM)
        {
            DateTime ReqDateFrom = DateTime.Now.Date;
            DateTime ReqDateTo = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(VM.Startdate))
            {
                ReqDateFrom = DateTime.ParseExact(VM.Startdate, "dd-MMM-yyyy", null);
            }
            if (!string.IsNullOrEmpty(VM.ENDDATE))
            {
                ReqDateTo = DateTime.ParseExact(VM.ENDDATE + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
            }

            List<VM_VW_DGIT_IOMREPORT> obj = new List<VM_VW_DGIT_IOMREPORT>();
            var orglevel = _ICDBContext.ICADORGLEVELs.Where(m => m.SYKIID == VM.KIID);

            var isspecial = (from data in _ICDBContext.DGIT_ICRPTRIGHTS
                             where data.ADEMPCODE == VM.loginid
                             && data.REPORTTYPE == 2
                             && data.STATUS == 1
                             && data.KIID == VM.KIID
                             select data
                          );
            if (isspecial.Count() > 0)
            {
                List<DGIT_ICRPTRIGHTS> iList = isspecial.ToList();
                List<long?> searchop = iList.Where(m => m.OPERATIONID != null).Select(m => m.OPERATIONID).ToList();
                List<long> operationids = iList.Where(m => m.OPERATIONID != null && m.DIVISIONID == null && m.DEPARTMENTID == null && m.SECTIONID == null).Select(m => (long)m.OPERATIONID).ToList();
                List<long> divisionids = iList.Where(m => m.DIVISIONID != null && m.DEPARTMENTID == null && m.SECTIONID == null).Select(m => (long)m.DIVISIONID).ToList();
                divisionids.AddRange(orglevel.Where(m => operationids.Contains(((long)m.PARENTLEVELID))).Select(m => (m.ADORGLEVELID)).ToList());
                List<long> departmentids = iList.Where(m => m.DEPARTMENTID != null && m.SECTIONID == null).Select(m => (long)m.DEPARTMENTID).ToList();
                departmentids.AddRange(orglevel.Where(m => divisionids.Contains(((long)m.PARENTLEVELID))).Select(m => (m.ADORGLEVELID)).ToList());
                List<long> sectionids = iList.Where(m => m.SECTIONID != null).Select(m => (long)m.SECTIONID).ToList();
                sectionids.AddRange(orglevel.Where(m => departmentids.Contains(((long)m.PARENTLEVELID))).Select(m => (m.ADORGLEVELID)).ToList());

                obj = (from data in _ICDBContext.VW_DGIT_IOMREPORT
                           //where (!string.IsNullOrEmpty(VM.Startdate) ? (data.DATEADDED >= ReqDateFrom) : true)
                       where (string.IsNullOrEmpty(VM.Startdate) || (data.DATEADDED >= ReqDateFrom))
                       //&& (!string.IsNullOrEmpty(VM.ENDDATE) ? (data.DATEADDED <= ReqDateTo) : true)
                       && (string.IsNullOrEmpty(VM.ENDDATE) || (data.DATEADDED <= ReqDateTo))
                       && (VM.ecode == 0 ? true : data.ADDEDBY == VM.ecode)
                       && (VM.Status == -1 ? true : data.PROCESS_STATUS == VM.Status)
                       //&& (string.IsNullOrEmpty(VM.ITEM_DETAIL) ? true : data.IOM_DESC.ToLower().Contains(VM.ITEM_DETAIL.ToLower()))
                       && (string.IsNullOrEmpty(VM.ITEM_DETAIL) || data.IOM_DESC.ToLower().Contains(VM.ITEM_DETAIL.ToLower()))
                       //&& (VM.IOMCATMSTID != 0 ? data.IOMCATID == VM.IOMCATMSTID : true)
                       && (VM.IOMCATMSTID == 0 || data.IOMCATID == VM.IOMCATMSTID)
                       && data.SYKIID == VM.KIID
                       //&& searchop.Contains(data.OPERATIONID)
                       && data.PROCESS_STATUS != 4
                       && (data.OPERATIONID == VM.OperationID)
                       //&& (VM.DivisionID != 0 ? data.DIVISIONID == VM.DivisionID : true)
                       && (VM.DivisionID == 0 || data.DIVISIONID == VM.DivisionID)
                       //&& (VM.DEPTID != 0 ? data.DEPARTMENTID == VM.DEPTID : true)
                       && (VM.DEPTID == 0 || data.DEPARTMENTID == VM.DEPTID)
                       //&& (VM.SECID != 0 ? data.SECTIONID == VM.SECID : true)
                       && (VM.SECID == 0 || data.SECTIONID == VM.SECID)
                       //&& (string.IsNullOrEmpty(VM.ADDEDBYNAME) ? true : data.ADDEDBYNAME.ToLower().Contains(VM.ADDEDBYNAME.Trim().ToLower()))
                       && (string.IsNullOrEmpty(VM.ADDEDBYNAME) || data.ADDEDBYNAME.ToLower().Contains(VM.ADDEDBYNAME.Trim().ToLower()))
                       select new VM_VW_DGIT_IOMREPORT
                       {
                           ADDEDBY = data.ADDEDBY,
                           APP_TYPE = data.APP_TYPE,
                           CATDESC = data.CATDESC,
                           DATEADDED = data.DATEADDED,
                           DEPARTMENT = data.DEPARTMENT,
                           DEPARTMENTID = data.DEPARTMENTID,
                           DIVISION = data.DIVISION,
                           DIVISIONID = data.DIVISIONID,
                           IOMCATID = data.IOMCATID,
                           IOMHEADERID = data.IOMHEADERID,
                           IOM_DESC = data.IOM_DESC,
                           KICODE = data.KICODE,
                           OPERATION = data.OPERATION,
                           OPERATIONID = data.OPERATIONID,
                           PROCESS_STATUS = data.PROCESS_STATUS,
                           SECTION = data.SECTION,
                           SECTIONID = data.SECTIONID,
                           STATUS = data.STATUS,
                           SYKIID = data.SYKIID,
                           UPDATEDATE = data.UPDATEDATE,
                           UPDATEDBY = data.UPDATEDBY,
                           ADDEDBYNAME = data.ADDEDBYNAME
                       }).ToList();

                List<VM_VW_DGIT_IOMREPORT> finallist = new List<VM_VW_DGIT_IOMREPORT>();
                if (VM.SECID > 0 && sectionids.Count() > 0)
                {
                    //if(obj.Where(m => sectionids.Contains(Convert.ToInt64(m.SECTIONID))).Count() > 0)
                    finallist.AddRange(obj.Where(m => sectionids.Contains(Convert.ToInt64(m.SECTIONID))));
                }
                else if (VM.DEPTID > 0 && departmentids.Count() > 0)
                {
                    if (departmentids.Contains(VM.DEPTID))
                    {
                        //if (obj.Where(m => (Convert.ToInt64(m.DEPARTMENTID)) == VM.DEPTID).Count() > 0)
                        finallist.AddRange(obj.Where(m => (Convert.ToInt64(m.DEPARTMENTID)) == VM.DEPTID));
                    }
                    else
                    {
                        var seclst = orglevel.Where(d => sectionids.Contains(d.ADORGLEVELID)).Select(m => m.ADORGLEVELID);
                        //if (obj.Where(m => seclst.Contains(Convert.ToInt64(m.SECTIONID)) && m.DEPARTMENTID == VM.DEPTID).Count() > 0)
                        {
                            finallist.AddRange(obj.Where(m => seclst.Contains(Convert.ToInt64(m.SECTIONID)) && m.DEPARTMENTID == VM.DEPTID));
                        }
                    }

                }
                else if (VM.DivisionID > 0 && divisionids.Count() > 0)
                {
                    if (divisionids.Contains(VM.DivisionID))
                    {
                        //if(obj.Where(m => Convert.ToInt64(m.DIVISIONID) == VM.DivisionID).Count()>0)
                        finallist.AddRange(obj.Where(m => Convert.ToInt64(m.DIVISIONID) == VM.DivisionID));
                    }
                    else
                    {
                        var deptlst = orglevel.Where(d => departmentids.Contains(d.ADORGLEVELID)).Select(m => m.ADORGLEVELID);
                        if (deptlst.Count() > 0)
                        {
                            //if(obj.Where(m => deptlst.Contains(Convert.ToInt64(m.DEPARTMENTID)) && m.DIVISIONID == VM.DivisionID).Count()>0)
                            finallist.AddRange(obj.Where(m => deptlst.Contains(Convert.ToInt64(m.DEPARTMENTID)) && m.DIVISIONID == VM.DivisionID));
                        }

                        var seclst = orglevel.Where(d => sectionids.Contains(d.ADORGLEVELID)).Select(m => m.ADORGLEVELID);
                        if (seclst.Count() > 0)
                        {
                            //if(obj.Where(m => seclst.Contains(Convert.ToInt64(m.SECTIONID)) && m.DIVISIONID == VM.DivisionID).Count()>0)
                            finallist.AddRange(obj.Where(m => seclst.Contains(Convert.ToInt64(m.SECTIONID)) && m.DIVISIONID == VM.DivisionID));
                        }

                    }

                }
                else if (VM.OperationID > 0)
                {
                    if (divisionids.Count > 0)
                    {
                        //if(obj.Where(m => divisionids.Contains(Convert.ToInt64(m.DIVISIONID))).Count()>0)
                        finallist.AddRange(obj.Where(m => divisionids.Contains(Convert.ToInt64(m.DIVISIONID))));
                    }
                    if (departmentids.Count > 0)
                    {
                        //if(obj.Where(m => departmentids.Contains(Convert.ToInt64(m.DEPARTMENTID))).Count()>0)
                        finallist.AddRange(obj.Where(m => departmentids.Contains(Convert.ToInt64(m.DEPARTMENTID))));
                    }
                    if (sectionids.Count > 0)
                    {
                        //if(obj.Where(m => sectionids.Contains(Convert.ToInt64(m.SECTIONID))).Count()>0)
                        finallist.AddRange(obj.Where(m => sectionids.Contains(Convert.ToInt64(m.SECTIONID))));
                    }

                }
                obj = finallist.Distinct().ToList();
            }
            else
            {
                obj = (from data in _ICDBContext.VW_DGIT_IOMREPORT
                           //where (!string.IsNullOrEmpty(VM.Startdate) ? (data.DATEADDED >= ReqDateFrom) : true)
                       where (string.IsNullOrEmpty(VM.Startdate) || (data.DATEADDED >= ReqDateFrom))
                       //&& (!string.IsNullOrEmpty(VM.ENDDATE) ? (data.DATEADDED <= ReqDateTo) : true)
                       && (string.IsNullOrEmpty(VM.ENDDATE) || (data.DATEADDED <= ReqDateTo))
                       //&& (VM.OperationID != 0 ? (data.OPERATIONID == VM.OperationID) : true)
                       && (VM.OperationID == 0 || (data.OPERATIONID == VM.OperationID))
                       && (VM.DivisionID == 0 || data.DIVISIONID == VM.DivisionID)
                       //&& (VM.DEPTID != 0 ? data.DEPARTMENTID == VM.DEPTID : true)
                       && (VM.DEPTID == 0 || data.DEPARTMENTID == VM.DEPTID)
                       //&& (VM.SECID != 0 ? data.SECTIONID == VM.SECID : true)
                       && (VM.SECID == 0 || data.SECTIONID == VM.SECID)
                       //&& (VM.ecode == 0 ? true : data.ADDEDBY == VM.ecode)
                       && (VM.ecode == 0 || data.ADDEDBY == VM.ecode)
                       && (VM.Status == -1 || data.PROCESS_STATUS == VM.Status)
                       //&& (string.IsNullOrEmpty(VM.ITEM_DETAIL) ? true : data.IOM_DESC.ToLower().Contains(VM.ITEM_DETAIL.ToLower()))
                       && (string.IsNullOrEmpty(VM.ITEM_DETAIL) || data.IOM_DESC.ToLower().Contains(VM.ITEM_DETAIL.ToLower()))
                       //&& (VM.IOMCATMSTID != 0 ? data.IOMCATID == VM.IOMCATMSTID : true)
                       && (VM.IOMCATMSTID == 0 || data.IOMCATID == VM.IOMCATMSTID)
                       && data.SYKIID == VM.KIID
                       && data.PROCESS_STATUS != 4
                       && (data.OPERATIONID == VM.OperationID)
                       //&& (VM.DivisionID != 0 ? data.DIVISIONID == VM.DivisionID : true)
                       && (VM.DivisionID == 0 || data.DIVISIONID == VM.DivisionID)
                       //&& (VM.DEPTID != 0 ? data.DEPARTMENTID == VM.DEPTID : true)
                       && (VM.DEPTID == 0 || data.DEPARTMENTID == VM.DEPTID)
                       //&& (VM.SECID != 0 ? data.SECTIONID == VM.SECID : true)
                       && (VM.SECID == 0 || data.SECTIONID == VM.SECID)
                       //&& (string.IsNullOrEmpty(VM.ADDEDBYNAME) ? true : data.ADDEDBYNAME.ToLower().Contains(VM.ADDEDBYNAME.Trim().ToLower()))
                       && (string.IsNullOrEmpty(VM.ADDEDBYNAME) || data.ADDEDBYNAME.ToLower().Contains(VM.ADDEDBYNAME.Trim().ToLower()))
                       select new VM_VW_DGIT_IOMREPORT
                       {
                           ADDEDBY = data.ADDEDBY,
                           APP_TYPE = data.APP_TYPE,
                           CATDESC = data.CATDESC,
                           DATEADDED = data.DATEADDED,
                           DEPARTMENT = data.DEPARTMENT,
                           DEPARTMENTID = data.DEPARTMENTID,
                           DIVISION = data.DIVISION,
                           DIVISIONID = data.DIVISIONID,
                           IOMCATID = data.IOMCATID,
                           IOMHEADERID = data.IOMHEADERID,
                           IOM_DESC = data.IOM_DESC,
                           KICODE = data.KICODE,
                           OPERATION = data.OPERATION,
                           OPERATIONID = data.OPERATIONID,
                           PROCESS_STATUS = data.PROCESS_STATUS,
                           SECTION = data.SECTION,
                           SECTIONID = data.SECTIONID,
                           STATUS = data.STATUS,
                           SYKIID = data.SYKIID,
                           UPDATEDATE = data.UPDATEDATE,
                           UPDATEDBY = data.UPDATEDBY,
                           ADDEDBYNAME = data.ADDEDBYNAME
                       }).ToList();
            }



            VM.SearchResult = obj.OrderByDescending(m => m.DATEADDED).ThenBy(m => m.UPDATEDATE).ToList();
            return VM;
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

                var iColl = (from data in _IOMDBContext.VW_ASSOCIATELVLDETAILS.Where(e => e.ACTIVE == 1
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

        public Tuple<long, List<PR_Div_Dep_SecViewModel>> BindDivision(long Loginempcode, long KIID, long op_Id)
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
                var iColl = (from data in _IOMDBContext.VW_ASSOCIATELVLDETAILS.Where(e => e.ACTIVE == 1
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

        public Tuple<long, List<PR_Div_Dep_SecViewModel>> BindDepartment(long Loginempcode, long KIID, long op_Id, long Div_id)
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
                             && data.DIVISIONID == Div_id
                             select data
                            );

                var oplist1 = (from data in isspecial
                               join op in _ICDBContext.ICADORGLEVELs on data.DEPARTMENTID equals op.ADORGLEVELID
                               where (data.DEPARTMENTID != null || data.DEPARTMENTID != 0)
                                     && op.ADORGLEVELTYPEID == 3
                                     && op.SYKIID == KIID
                               select op
                                  );

                if (oplist1.Count() == 0)
                {
                    oplist1 = (from data in _ICDBContext.ICADORGLEVELs
                               join op in isspecial on data.PARENTLEVELID equals op.DIVISIONID
                               where (op.DIVISIONID != null || op.DIVISIONID != 0)
                                     && data.ADORGLEVELTYPEID == 3
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
                //var iColl = (from data in _IOMDBContext.VW_ASSOCIATELVLDETAILS.Where(e => e.ACTIVE == 1
                //             && e.SYKI == KIID && (e.DEPARTMENTID != null || e.DEPARTMENTID != 0)
                //             && e.ADEMPCODE == Loginempcode)
                //             select new
                //             {
                //                 data.DEPARTMENTID,
                //                 data.DEPARTMENT
                //             }).Distinct().ToList();
                var iColl = (from data in _IOMDBContext.VW_ASSOCIATELVLDETAILS.Where(e => e.ACTIVE == 1
                             && e.SYKI == KIID && (e.OPERATIONID == op_Id && e.DIVISIONID == Div_id && e.DEPARTMENTID != null))
                                 //&& e.ADEMPCODE == Loginempcode)
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
            }
            Tuple<long, List<PR_Div_Dep_SecViewModel>> _tuple = new Tuple<long, List<PR_Div_Dep_SecViewModel>>(isspecialright, iList);
            return _tuple;
        }

        public Tuple<long, List<PR_Div_Dep_SecViewModel>> BindSection(long Loginempcode, long KIID, long op_Id, long divid, long deptid)
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
                isspecial = (from data in isspecial
                             where
                             data.OPERATIONID == op_Id
                             && data.DIVISIONID == divid
                             && data.DEPARTMENTID == deptid
                             select data
                            );
                isspecialright = 1;

                var oplist1 = (from data in isspecial
                               join op in _ICDBContext.ICADORGLEVELs on data.SECTIONID equals op.ADORGLEVELID
                               where (data.SECTIONID != null || data.SECTIONID != 0)
                                     && op.ADORGLEVELTYPEID == 4
                                     && op.SYKIID == KIID
                               select op
                                  );

                if (oplist1.Count() == 0)
                {
                    oplist1 = (from data in _ICDBContext.ICADORGLEVELs
                               join op in isspecial on data.PARENTLEVELID equals op.DEPARTMENTID
                               where (op.DEPARTMENTID != null || op.DEPARTMENTID != 0)
                                     && data.ADORGLEVELTYPEID == 4
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
                var iColl = (from data in _IOMDBContext.VW_ASSOCIATELVLDETAILS.Where(e => e.ACTIVE == 1
                             && e.SYKI == KIID && (e.SECTION != null && e.DIVISIONID == divid && e.OPERATIONID == op_Id && e.DEPARTMENTID == deptid)
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

        public List<PR_Div_Dep_SecViewModel> GetKiLIST(long ecode)
        {
            var KILIST = (from data in _ICDBContext.SYKI1
                          join vw in _ICDBContext.VW_ASSOCIATELVLDETAILS1 on data.SYKIID equals ((decimal)vw.SYKI)
                          where vw.ADEMPCODE == ecode
                          select new PR_Div_Dep_SecViewModel
                          {
                              Text = data.KICODE,
                              Value = (long)(data.SYKIID)
                          }
                      ).ToList();
            return KILIST.OrderByDescending(m => m.Value).ToList();
        }

        public Employee_Details GetEmployeeDetail(long ecode, long KIID)
        {
            var KILIST = (from data in _ICDBContext.VW_ASSOCIATELVLDETAILS1
                          where data.SYKI == KIID && data.ADEMPCODE == ecode
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
        //Added by Aumento for SR99176
        public List<PR_Div_Dep_SecViewModel> GetKICodeLIst()
        {
            List<PR_Div_Dep_SecViewModel> Kilist_ = new List<PR_Div_Dep_SecViewModel>();
            List<PR_Div_Dep_SecViewModel> Kilist = new List<PR_Div_Dep_SecViewModel>();

            try
            {
                Kilist_ = (from k in _IOMDBContext.SYKI select new PR_Div_Dep_SecViewModel { Text = k.KICODE, Value = (long)k.SYKIID }).ToList();
                Kilist = (from k in _IOMDBContext.SYKI orderby k.SYKIID descending select new PR_Div_Dep_SecViewModel { Text = k.KICODE, Value = (long)k.SYKIID }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Kilist;
        }

        public List<Employee_Details> GetAllOperation(long KIID)
        {
            var OperationList = (from data in _ICDBContext.VW_ASSOCIATELVLDETAILS1
                                 where data.SYKI == KIID &&
                                 data.OPERATIONID != null
                                 select new Employee_Details
                                 {
                                     _OpDesc = data.OPERATION,
                                     _OpId = data.OPERATIONID
                                 }).ToList();
            return OperationList;
        }
        public List<Employee_Details> GetDivisionByOp(long OpCode, long KIID)
        {
            var DivisionList = (from data in _ICDBContext.VW_ASSOCIATELVLDETAILS1
                                where data.SYKI == KIID &&
                                data.OPERATIONID == OpCode &&
                                data.DIVISIONID != null
                                select new Employee_Details
                                {
                                    _DivDesc = data.DIVISION,
                                    _DivId = data.DIVISIONID
                                }).ToList();
            return DivisionList;
        }
        public List<Employee_Details> GetDepartmentByDiv(long DivId, long OpCode, long KIID)
        {
            var DepartmentList = (from data in _ICDBContext.VW_ASSOCIATELVLDETAILS1
                                  where data.SYKI == KIID && data.OPERATIONID == OpCode &&
                                  data.DIVISIONID == DivId && data.DEPARTMENTID != null
                                  select new Employee_Details
                                  {
                                      _DepDesc = data.DEPARTMENT,
                                      _DepId = data.DEPARTMENTID
                                  }).ToList();
            return DepartmentList;
        }
        public List<Employee_Details> GetSectionByDept(long DeptId, long DivId, long OpCode, long KIID)
        {
            var SectionList = (from data in _ICDBContext.VW_ASSOCIATELVLDETAILS1
                               where data.SYKI == KIID && data.OPERATIONID == OpCode &&
                               data.DIVISIONID == DivId && data.DEPARTMENTID == DeptId
                               && data.SECTIONID != null
                               select new Employee_Details
                               {
                                   _SecDescrip = data.SECTION,
                                   _SecId = data.SECTIONID
                               }).ToList();
            return SectionList;
        }
        public SearchIOM IOMAuditReport(SearchIOM VM)
        {
            DateTime ReqDateFrom = DateTime.Now.Date;
            DateTime ReqDateTo = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(VM.Startdate))
            {
                ReqDateFrom = DateTime.ParseExact(VM.Startdate, "dd-MMM-yyyy", null);
            }
            if (!string.IsNullOrEmpty(VM.ENDDATE))
            {
                ReqDateTo = DateTime.ParseExact(VM.ENDDATE + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
            }

            List<VM_VW_DGIT_IOMREPORT> obj = new List<VM_VW_DGIT_IOMREPORT>();
            var orglevel = _ICDBContext.ICADORGLEVELs.Where(m => m.SYKIID == VM.KIID);

            obj = (from data in _ICDBContext.VW_DGIT_IOMREPORT
                   where (!string.IsNullOrEmpty(VM.Startdate) ? (data.DATEADDED >= ReqDateFrom) : true)
                   && (!string.IsNullOrEmpty(VM.ENDDATE) ? (data.DATEADDED <= ReqDateTo) : true)
                   && (VM.OperationID != 0 ? (data.OPERATIONID == VM.OperationID) : true)
                   && (VM.DivisionID != 0 ? data.DIVISIONID == VM.DivisionID : true)
                   && (VM.DEPTID != 0 ? data.DEPARTMENTID == VM.DEPTID : true)
                   && (VM.SECID != 0 ? data.SECTIONID == VM.SECID : true)
                   && (VM.ecode == 0 ? true : data.ADDEDBY == VM.ecode)
                   //&& (VM.Status == -1 ? true : data.PROCESS_STATUS == VM.Status) // Commented by Aumneto for SR102468
                    && data.PROCESS_STATUS == VM.Status // Added by Aumneto for SR102468
                   && (string.IsNullOrEmpty(VM.ITEM_DETAIL) ? true : data.IOM_DESC.ToLower().Contains(VM.ITEM_DETAIL.ToLower()))
                   && (VM.IOMCATMSTID != 0 ? data.IOMCATID == VM.IOMCATMSTID : true)
                   && data.SYKIID == VM.KIID
                   && (string.IsNullOrEmpty(VM.ADDEDBYNAME) ? true : data.ADDEDBYNAME.ToLower().Contains(VM.ADDEDBYNAME.Trim().ToLower()))
                   select new VM_VW_DGIT_IOMREPORT
                   {
                       ADDEDBY = data.ADDEDBY,
                       APP_TYPE = data.APP_TYPE,
                       CATDESC = data.CATDESC,
                       DATEADDED = data.DATEADDED,
                       DEPARTMENT = data.DEPARTMENT,
                       DEPARTMENTID = data.DEPARTMENTID,
                       DIVISION = data.DIVISION,
                       DIVISIONID = data.DIVISIONID,
                       IOMCATID = data.IOMCATID,
                       IOMHEADERID = data.IOMHEADERID,
                       IOM_DESC = data.IOM_DESC,
                       KICODE = data.KICODE,
                       OPERATION = data.OPERATION,
                       OPERATIONID = data.OPERATIONID,
                       PROCESS_STATUS = data.PROCESS_STATUS,
                       SECTION = data.SECTION,
                       SECTIONID = data.SECTIONID,
                       STATUS = data.STATUS,
                       SYKIID = data.SYKIID,
                       UPDATEDATE = data.UPDATEDATE,
                       UPDATEDBY = data.UPDATEDBY,
                       ADDEDBYNAME = data.ADDEDBYNAME
                   }).ToList();

            VM.SearchResult = obj.OrderByDescending(m => m.DATEADDED).ThenBy(m => m.UPDATEDATE).ToList();
            return VM;
        }
        //Added by Aumento for SR99176

        //Changed by TTL on 14-June-2025 against SR99343 > CR6531 - Start
        bool IsAlreadyPending(long iomHeaderId, long adEmpCode)
        {
            try
            {
                return _IOMDBContext.DGIT_IOMAPPHISTORY.Any(x => x.IOMHEADERID == iomHeaderId && x.ADEMPCODE == adEmpCode && x.APPROVAL_STATUS == 0);
            }
            catch (Exception)
            {
                return false;
            }
        }
        //Changed by TTL on 14-June-2025 against SR99343 > CR6531 - End

        // Added by TTL on 17-July-2025 against SR93758 > CR5975 - Start
        #region IOMDashboardChart
        public Tuple<long, List<PR_Div_Dep_SecViewModel>> BindIOMGraphOperation(long Loginempcode, long KIID)
        {
            long isspecialright = 0;
            bool IsDefaultOp = true;
            List<PR_Div_Dep_SecViewModel> iList = new List<PR_Div_Dep_SecViewModel>();

            var ADDESIGNATIONID = _IOMDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == Loginempcode && x.SYKI == KIID).Select(x => x.ADDESIGNATIONID)?.FirstOrDefault();
            var syParametersData = _IOMDBContext.SYPARAMETERS.Where(x => x.PARAMNAME == "IOM_GRAPH_OP")?.FirstOrDefault();
            if ((ADDESIGNATIONID ?? 0) > 0 && syParametersData != null && !string.IsNullOrWhiteSpace(syParametersData.PARAMVALUE) && syParametersData.PARAMVALUE.Split(',').Select(x => x.Trim()).Contains(ADDESIGNATIONID.ToString()))
            {
                isspecialright = 1;

                var result = (from org in _IOMDBContext.ADORGLEVEL
                              where org.SYKIID == KIID
                                 && org.ADORGLEVELTYPEID == 1
                                 && (from orgc in _IOMDBContext.ADORGCOORDINATOR
                                     where (orgc.COORDINATOR == Loginempcode || orgc.EXECOORDINATOR == Loginempcode || orgc.DIRECTOR == Loginempcode || orgc.DIRECTOR2 == Loginempcode || orgc.OPHEAD == Loginempcode)
                                        && orgc.ISACTIVE == 1
                                     select orgc.ADORGLEVELID).Contains(org.ADORGLEVELID)
                              select org).Distinct().ToList();

                if (result?.Any() ?? false)
                {
                    IsDefaultOp = false;
                    foreach (var obj in result)
                    {
                        iList.Add(new PR_Div_Dep_SecViewModel
                        {
                            Value = Convert.ToInt64(obj.ADORGLEVELID),
                            Text = obj.LEVELDESCRIP
                        });
                    }
                }
            }

            if (IsDefaultOp)
            {
                var iColl = (from data in _IOMDBContext.VW_ASSOCIATELVLDETAILS.Where(e => e.ACTIVE == 1
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
        private SearchIOM IOMUserReportGraphData(SearchIOM VM)
        {
            DateTime ReqDateFrom = DateTime.Now.Date;
            DateTime ReqDateTo = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(VM.Startdate))
                ReqDateFrom = DateTime.ParseExact(VM.Startdate, "dd-MMM-yyyy", null);
            if (!string.IsNullOrEmpty(VM.ENDDATE))
                ReqDateTo = DateTime.ParseExact(VM.ENDDATE + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);

            List<VM_VW_DGIT_IOMREPORT> obj = new List<VM_VW_DGIT_IOMREPORT>();
            short[] cycleEndProcessStatus = new short[] { 2, 3 };

            DataTable dtReport = _ObjCommn.IOMUserReportGraphData(new SearchIOMsproc()
            {
                StartDateIn = !string.IsNullOrEmpty(VM.Startdate) ? ReqDateFrom.ToString("dd-MMM-yyyy") : null,
                EndDateIn = !string.IsNullOrEmpty(VM.ENDDATE) ? ReqDateTo.ToString("dd-MMM-yyyy") : null,
                OprnIdIn = VM.OperationID,
                DivIdIn = VM.DivisionID,
                DeptIdIn = VM.DEPTID,
                SecIdIn = VM.SECID,
                AddedByIn = VM.ecode,
                ProcStatusesIn = VM.FilterStatus.Length > 0 ? string.Join(",", VM.FilterStatus) : null,
                PendingAtUsersIn = VM.PendingWithType == "Selected" ? (VM.PendingAtUsers.Length > 0 ? string.Join(",", VM.PendingAtUsers) : null) : VM.PendingWithType == "Self" ? VM.loginid.ToString() : "", //Changed by TTL on 28-July-2025 against SR104160 > CR6821 - Start
                ItemDetailIn = !string.IsNullOrEmpty(VM.ITEM_DETAIL) ? VM.ITEM_DETAIL.ToLower() : null,
                CatmstIdIn = VM.IOMCATMSTID,
                KiidIn = VM.KIID,
                AddedbyNameIn = !string.IsNullOrEmpty(VM.ADDEDBYNAME) ? VM.ADDEDBYNAME.Trim().ToLower() : null
            });

            obj = (from d in dtReport.AsEnumerable()
                   select new VM_VW_DGIT_IOMREPORT
                   {
                       APP_TYPE = d.Field<short>("APP_TYPE"),
                       CATDESC = d.Field<string>("CATDESC"),
                       DEPARTMENT = d.Field<string>("DEPARTMENT"),
                       DEPARTMENTID = d.Field<long?>("DEPARTMENTID"),
                       DIVISION = d.Field<string>("DIVISION"),
                       DIVISIONID = d.Field<long?>("DIVISIONID"),
                       IOMCATID = d.Field<long?>("IOMCATID"),
                       IOMHEADERID = d.Field<long>("IOMHEADERID"),
                       IOM_DESC = d.Field<string>("IOM_DESC"),
                       KICODE = d.Field<string>("KICODE"),
                       OPERATION = d.Field<string>("OPERATION"),
                       OPERATIONID = d.Field<long?>("OPERATIONID"),
                       PROCESS_STATUS = d.Field<short>("PROCESS_STATUS"),
                       SECTION = d.Field<string>("SECTION"),
                       SECTIONID = d.Field<long?>("SECTIONID"),
                       STATUS = d.Field<short>("STATUS"),
                       SYKIID = d.Field<decimal>("SYKIID"),
                       SYSITEID = d.Field<long>("SYSITEID"),
                       ADDEDBYNAME = d.Field<string>("ADDEDBYNAME"),
                       ADDEDBY = d.Field<long>("ADDEDBY"),
                       UPDATEDBY = d.Field<long?>("UPDATEDBY"),
                       PENDINGWITH_ECODE = d.Field<decimal?>("PENDINGWITH_ECODE"),
                       LAST_MODIFIED_BY_ECODE = d.Field<decimal?>("LAST_MODIFIED_BY_ECODE"),
                       DATEADDED = d.Field<DateTime>("DATEADDED"),
                       UPDATEDATE = d.Field<DateTime?>("UPDATEDATE"),
                       LEADSTARTDATE = d.Field<DateTime?>("LEADSTARTDATE"),
                       LEADENDDATE = d.Field<DateTime?>("LEADENDDATE"),
                       LAST_MODIFIED_ON = d.Field<DateTime?>("LAST_MODIFIED_ON"),
                       PENDING_SINCE = d.Field<DateTime?>("PENDING_SINCE"),
                       HOLIDAYSINCELEADTIME = d.Field<decimal>("HOLIDAYSINCELEADTIME"),
                       HOLIDAYSINCEPENDING = d.Field<decimal>("HOLIDAYSINCEPENDING"),
                       CYCLETIME = d.Field<decimal>("CYCLETIME"),
                       PENDINGTIME = d.Field<decimal>("PENDINGTIME"),
                       PENDINGWITH_NAME = d.Field<string>("PENDINGWITH_NAME"),
                       LASTMODIFIED_NAME = d.Field<string>("LASTMODIFIED_NAME"),
                   }).ToList();
            VM.SearchResult = obj;
            return VM;
        }
        public Tuple<List<IOMDashboardGraphViewModel>, long> GetIOMDashboardGraphData(SearchIOM VM)
        {
            List<IOMDashboardGraphViewModel> res = new List<IOMDashboardGraphViewModel>();
            SearchIOM data = IOMUserReportGraphData(VM);
            var resultSet = data.SearchResult.OrderBy(m => m.PROCESS_STATUS).ToList();
            var result = resultSet
                          .GroupBy(x =>
                             (VM.OperationID == 0 || (VM.OperationID > 0 && VM.HasDivisionListData == -1)) ? new { Label = x.OPERATION, Id = x.OPERATIONID } :
                             (VM.DivisionID == 0 || (VM.DivisionID > 0 && VM.HasDeptListData == -1)) ? new { Label = x.DIVISION, Id = x.DIVISIONID } :
                             (VM.DEPTID == 0 || (VM.DEPTID > 0 && VM.HasSecListData == -1)) ? new { Label = x.DEPARTMENT, Id = x.DEPARTMENTID } :
                                                 new { Label = x.SECTION, Id = x.SECTIONID })
                        .Select(group => new IOMDashboardGraphViewModel
                        {
                            Group = new IOMGraphGroupModel
                            {
                                DepartmentLabel = group.Key.Label,
                                id = group.Key.Id,
                                count = group.Count()
                            },
                            Data = group
                                .GroupBy(x => new { x.PROCESS_STATUS })
                                .Select(statusGroup => new IOMGraphDataModel
                                {
                                    Status = new IOMGraphStatusModel
                                    {
                                        id = statusGroup.Key.PROCESS_STATUS,
                                        desc = getIOMProcessStatusDesc(statusGroup.Key.PROCESS_STATUS)
                                    },
                                    RecordsByRange = new List<IOMDashboardGraphRecordCountByRange>
                                    {
                                        new IOMDashboardGraphRecordCountByRange
                                        {
                                            Range = IOMGraphDashboardRange.RangeMap[1],
                                            //RecordCount = statusGroup.Count(x =>
                                            //    (((x.LEADENDDATE.Date - x.LEADSTARTDATE.Date).Days + 1) - x.HOLIDAYCOUNT) >= 0 &&
                                            //    (((x.LEADENDDATE.Date - x.LEADSTARTDATE.Date).Days + 1) - x.HOLIDAYCOUNT) <= 5)
                                            RecordCount = statusGroup.Count(x=> x.CYCLETIME >= 0 && x.CYCLETIME <= 5)
                                        },
                                        new IOMDashboardGraphRecordCountByRange
                                        {
                                            Range = IOMGraphDashboardRange.RangeMap[2],
                                            //RecordCount = statusGroup.Count(x =>
                                            //    (((x.LEADENDDATE.Date - x.LEADSTARTDATE.Date).Days + 1) - x.HOLIDAYCOUNT) > 5 &&
                                            //    (((x.LEADENDDATE.Date - x.LEADSTARTDATE.Date).Days + 1) - x.HOLIDAYCOUNT) <= 10)
                                            RecordCount = statusGroup.Count(x=> x.CYCLETIME > 5 && x.CYCLETIME <= 10)
                                        },
                                        new IOMDashboardGraphRecordCountByRange
                                        {
                                            Range = IOMGraphDashboardRange.RangeMap[3],
                                            //RecordCount = statusGroup.Count(x =>
                                            //    (((x.LEADENDDATE.Date - x.LEADSTARTDATE.Date).Days + 1) - x.HOLIDAYCOUNT) > 10 &&
                                            //    (((x.LEADENDDATE.Date - x.LEADSTARTDATE.Date).Days + 1) - x.HOLIDAYCOUNT) <= 15)
                                            RecordCount = statusGroup.Count(x=> x.CYCLETIME > 10 && x.CYCLETIME <= 15)
                                        },
                                        new IOMDashboardGraphRecordCountByRange
                                        {
                                            Range = IOMGraphDashboardRange.RangeMap[4],
                                            //RecordCount = statusGroup.Count(x =>
                                            //    (((x.LEADENDDATE.Date - x.LEADSTARTDATE.Date).Days + 1) - x.HOLIDAYCOUNT) > 15)
                                            RecordCount = statusGroup.Count(x=> x.CYCLETIME > 15)
                                        }
                                    }
                                }).ToList()
                        })
                        .OrderByDescending(x => x.Group.count).ToList();


            if (result?.Any() ?? false)
            {
                if (VM.OperationID == 0)
                {
                    Tuple<long, List<PR_Div_Dep_SecViewModel>> OPtemp = BindIOMGraphOperation(VM.loginid, VM.KIID);
                    if (OPtemp.Item1 == 1 && OPtemp.Item2.Count > 0)
                    {
                        var opids = new HashSet<long>(OPtemp.Item2.Select(x => x.Value));
                        result = result.FindAll(x => opids.Contains(x.Group.id ?? 0));
                    }
                    else
                    {
                        result = new List<IOMDashboardGraphViewModel>();
                    }
                }
                else if (VM.OperationID > 0 && VM.HasDivisionListData == -1)
                {
                    result = result.FindAll(x => x.Group.id == VM.OperationID);
                }
                else if (VM.DivisionID > 0 && VM.HasDeptListData == -1)
                {
                    result = result.FindAll(x => x.Group.id == VM.DivisionID);
                }
                else if (VM.DEPTID > 0 && VM.HasSecListData == -1)
                {
                    result = result.FindAll(x => x.Group.id == VM.DEPTID);
                }
                else if (VM.SECID > 0)
                {
                    result = result.FindAll(x => x.Group.id == VM.SECID);
                }
            }

            if (result?.Any() ?? false)
            {
                if (result.Any(x => string.IsNullOrWhiteSpace(x.Group.DepartmentLabel)))
                {
                    long adorgLevelID = VM.SECID == 0 ? VM.DEPTID : VM.DEPTID == 0 ? VM.DivisionID : VM.DivisionID == 0 ? VM.OperationID : 0;
                    string departmentLabel = _IOMDBContext.ADORGLEVEL.Where(x => x.ADORGLEVELID == adorgLevelID).Select(x => x.LEVELDESCRIP).FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(departmentLabel))
                        departmentLabel = "Unknown";

                    foreach (var item in result.Where(x => string.IsNullOrWhiteSpace(x.Group.DepartmentLabel)))
                    {
                        if (string.IsNullOrWhiteSpace(item.Group.DepartmentLabel))
                        {
                            item.Group.DepartmentLabel = departmentLabel;
                            item.Group.id = -1;
                        }
                    }
                }

                result = result
                     .Select(group => new
                     {
                         Group = group.Group,
                         FilteredData = group.Data
                             .Where(dataItem => dataItem.RecordsByRange.Any(r => r.RecordCount > 0))
                             .ToList()
                     })
                     .Where(x => x.FilteredData.Any())
                     .Select(x => new IOMDashboardGraphViewModel
                     {
                         Group = x.Group,
                         Data = x.FilteredData
                     })
                     .ToList();


            }
            if (result == null)
            {
                return new Tuple<List<IOMDashboardGraphViewModel>, long>(new List<IOMDashboardGraphViewModel>(), 0);
            }
            else
            {
                return new Tuple<List<IOMDashboardGraphViewModel>, long>(result, resultSet.Count);
            }
        }
        public string getIOMProcessStatusDesc(long? PROCESS_STATUS)
        {
            switch (PROCESS_STATUS)
            {
                case 0: return "Initiator";
                case 1: return "WIP";
                case 2: return "Completed";
                case 3: return "Rejected";
                case 5: return "Hold";
                // Add more cases as needed
                default: return "Unknown Status";
            }
        }
        public SearchIOMDashboadReportList GetIOMDashboadReportList(SearchIOMDashboadReportList model)
        {
            SearchIOM data = IOMUserReportGraphData(model.searchIOM);
            if (data != null && data.SearchResult != null && data.SearchResult.Count > 0)
            {
                var searchResult = (from x in data.SearchResult
                                    where (model.deptLavelId <= 0 || (model.deptLavelId > 0 && (
                                       ((model.searchIOM.OperationID == 0 || (model.searchIOM.OperationID > 0 && model.searchIOM.HasDivisionListData == -1)) && x.OPERATIONID == model.deptLavelId)
                                       || ((model.searchIOM.DivisionID == 0 || (model.searchIOM.DivisionID > 0 && model.searchIOM.HasDeptListData == -1)) && x.DIVISIONID == model.deptLavelId)
                                       || ((model.searchIOM.DEPTID == 0 || (model.searchIOM.DEPTID > 0 && model.searchIOM.HasSecListData == -1)) && x.DEPARTMENTID == model.deptLavelId)
                                       || x.SECTIONID == model.deptLavelId
                                    ))) &&
                                  (
                                    (model.IOMGraphDashboardRangeId == 1 && x.CYCLETIME >= 0 && x.CYCLETIME <= 5)
                                  || (model.IOMGraphDashboardRangeId == 2 && x.CYCLETIME > 5 && x.CYCLETIME <= 10)
                                  || (model.IOMGraphDashboardRangeId == 3 && x.CYCLETIME > 10 && x.CYCLETIME <= 15)
                                  || (model.IOMGraphDashboardRangeId == 4 && x.CYCLETIME > 15)
                                  || (model.IOMGraphDashboardRangeId == 5 && x.CYCLETIME >= 0 && x.CYCLETIME <= 2)
                                  || (model.IOMGraphDashboardRangeId == 6 && x.CYCLETIME == 3)
                                  || (model.IOMGraphDashboardRangeId == 7 && x.CYCLETIME >= 4 && x.CYCLETIME <= 5)
                                  || (model.IOMGraphDashboardRangeId == 8 && x.CYCLETIME >= 6 && x.CYCLETIME <= 7)
                                  || (model.IOMGraphDashboardRangeId == 9 && x.CYCLETIME == 8)
                                  || (model.IOMGraphDashboardRangeId == 10 && x.CYCLETIME >= 9 && x.CYCLETIME <= 10)
                                  || (model.IOMGraphDashboardRangeId == 11 && x.CYCLETIME >= 11 && x.CYCLETIME <= 12)
                                  || (model.IOMGraphDashboardRangeId == 12 && x.CYCLETIME == 13)
                                  || (model.IOMGraphDashboardRangeId == 13 && x.CYCLETIME >= 14 && x.CYCLETIME <= 15)
                                  || (model.IOMGraphDashboardRangeId == 14 && x.CYCLETIME >= 16)
                                  )
                                    select new IOMDashboadReportList
                                    {
                                        IOMHEADERID = x.IOMHEADERID,
                                        STATUS = x.STATUS,
                                        PROCESS_STATUS = x.PROCESS_STATUS,
                                        DATEADDED = x.DATEADDED,
                                        ADDEDBY = x.ADDEDBY,
                                        IOMCATID = x.IOMCATID,
                                        SYKIID = x.SYKIID,
                                        KICODE = x.KICODE,
                                        OPERATIONID = x.OPERATIONID,
                                        OPERATION = x.OPERATION,
                                        DIVISIONID = x.DIVISIONID,
                                        DIVISION = x.DIVISION,
                                        DEPARTMENTID = x.DEPARTMENTID,
                                        DEPARTMENT = x.DEPARTMENT,
                                        SECTIONID = x.SECTIONID,
                                        SECTION = x.SECTION,
                                        CATDESC = x.CATDESC,
                                        ADDEDBYNAME = x.ADDEDBYNAME,
                                        CYCLETIME = x.CYCLETIME,
                                        LASTACTIONDATE = x.LAST_MODIFIED_ON,
                                        LASTACTIONTAKENBY = x.LAST_MODIFIED_BY_ECODE,
                                        LASTACTIONTAKENBYNAME = x.LAST_MODIFIED_BY_ECODE.HasValue && x.LAST_MODIFIED_ON.HasValue ? $"By {x.LASTMODIFIED_NAME} on {x.LAST_MODIFIED_ON.Value.ToString("dd-MMM-yyyy")}" : "",
                                        PendingAt = x.PENDINGWITH_ECODE > 0 ? $"{x.PENDINGWITH_NAME}-{x.PENDINGWITH_ECODE} [Since - {x.PENDINGTIME} {(x.PENDINGTIME > 1 ? "Working days" : "Working day")}]" : "",
                                        PendingAtEmpCode = x.PENDINGWITH_ECODE
                                    }).ToList();

                List<SYPARAMETERS> sysParams = _IOMDBContext.SYPARAMETERS.Where(x => new string[] { "IOM_DESG_GRP_SRM", "IOM_DESG_GRP_TRM" }.Contains(x.PARAMNAME)).ToList();
                List<long> SeniorManagementIds = sysParams.Where(x => x.PARAMNAME == "IOM_DESG_GRP_SRM").FirstOrDefault().PARAMVALUE.Split(',').Select(x => Convert.ToInt64(x)).ToList();
                List<long> TeamMemberIds = sysParams.Where(x => x.PARAMNAME == "IOM_DESG_GRP_TRM").FirstOrDefault().PARAMVALUE.Split(',').Select(x => Convert.ToInt64(x)).ToList();
                List<decimal> pendingWithEmployees = searchResult.Select(x => x.PendingAtEmpCode ?? 0).Distinct().ToList();
                List<decimal> lastActionTakenByEmployees = searchResult.Select(x => x.LASTACTIONTAKENBY ?? 0).Distinct().ToList();
                //var employeeDesignations = (from v in _IOMDBContext.VW_ASSOCIATELVLDETAILS
                //                            join d in _IOMDBContext.ADDESIGNATION on v.ADDESIGNATIONID equals d.ADDESIGNATIONID
                //                            where v.SYKI == model.searchIOM.KIID && (pendingWithEmployees.Contains(v.ADEMPCODE) || lastActionTakenByEmployees.Contains(v.ADEMPCODE))
                //                            select new
                //                            {
                //                                v.ADEMPCODE,
                //                                Designation = !string.IsNullOrEmpty(v.FUNCTIONALDESIGNATION) ? v.FUNCTIONALDESIGNATION : (SeniorManagementIds.Contains(v.ADDESIGNATIONID.Value) ? "Senior Management" : TeamMemberIds.Contains(v.ADDESIGNATIONID.Value) ? "Team Member" : "Undefined Designation")
                //                            }).ToList();
                var employee = (from v in _IOMDBContext.VW_ASSOCIATELVLDETAILS
                                join d in _IOMDBContext.ADDESIGNATION on v.ADDESIGNATIONID equals d.ADDESIGNATIONID
                                where v.SYKI == model.searchIOM.KIID && (pendingWithEmployees.Contains(v.ADEMPCODE) || lastActionTakenByEmployees.Contains(v.ADEMPCODE))
                                select v);
                var employeeDesignations = (from v in employee.AsEnumerable()
                                            select new
                                            {
                                                v.ADEMPCODE,
                                                Designation = !string.IsNullOrEmpty(v.FUNCTIONALDESIGNATION) ? v.FUNCTIONALDESIGNATION : (SeniorManagementIds.Contains(v.ADDESIGNATIONID ?? 0) ? "Senior Management" : TeamMemberIds.Contains(v.ADDESIGNATIONID ?? 0) ? "Team Member" : "Undefined Designation")
                                            }).ToList();
                searchResult.ForEach(x =>
                {
                    x.OrgLevel = employeeDesignations.Where(y => y.ADEMPCODE == (x.PendingAtEmpCode == 0 ? x.LASTACTIONTAKENBY : x.PendingAtEmpCode)).FirstOrDefault()?.Designation;
                });
                model.SearchResult = searchResult;
            }
            return model;
        }
        public SearchIOMDashboadReportList GetIOMDashboadReportListByDesignationGroup(SearchIOMDashboadReportList model)
        {
            SearchIOM data = IOMUserReportGraphData(model.searchIOM);
            if (data != null && data.SearchResult != null && data.SearchResult.Count > 0)
            {
                var searchResult = (from x in data.SearchResult
                                    where (model.deptLavelId <= 0 || (model.deptLavelId > 0 && (
                                       ((model.searchIOM.OperationID == 0 || (model.searchIOM.OperationID > 0 && model.searchIOM.HasDivisionListData == -1)) && x.OPERATIONID == model.deptLavelId)
                                       || ((model.searchIOM.DivisionID == 0 || (model.searchIOM.DivisionID > 0 && model.searchIOM.HasDeptListData == -1)) && x.DIVISIONID == model.deptLavelId)
                                       || ((model.searchIOM.DEPTID == 0 || (model.searchIOM.DEPTID > 0 && model.searchIOM.HasSecListData == -1)) && x.DEPARTMENTID == model.deptLavelId)
                                       || x.SECTIONID == model.deptLavelId
                                    ))) &&
                                  (
                                    (model.IOMGraphDashboardRangeId == 1 && x.CYCLETIME >= 0 && x.CYCLETIME <= 5)
                                  || (model.IOMGraphDashboardRangeId == 2 && x.CYCLETIME > 5 && x.CYCLETIME <= 10)
                                  || (model.IOMGraphDashboardRangeId == 3 && x.CYCLETIME > 10 && x.CYCLETIME <= 15)
                                  || (model.IOMGraphDashboardRangeId == 4 && x.CYCLETIME > 15)
                                  || (model.IOMGraphDashboardRangeId == 5 && x.CYCLETIME >= 0 && x.CYCLETIME <= 2)
                                  || (model.IOMGraphDashboardRangeId == 6 && x.CYCLETIME == 3)
                                  || (model.IOMGraphDashboardRangeId == 7 && x.CYCLETIME >= 4 && x.CYCLETIME <= 5)
                                  || (model.IOMGraphDashboardRangeId == 8 && x.CYCLETIME >= 6 && x.CYCLETIME <= 7)
                                  || (model.IOMGraphDashboardRangeId == 9 && x.CYCLETIME == 8)
                                  || (model.IOMGraphDashboardRangeId == 10 && x.CYCLETIME >= 9 && x.CYCLETIME <= 10)
                                  || (model.IOMGraphDashboardRangeId == 11 && x.CYCLETIME >= 11 && x.CYCLETIME <= 12)
                                  || (model.IOMGraphDashboardRangeId == 12 && x.CYCLETIME == 13)
                                  || (model.IOMGraphDashboardRangeId == 13 && x.CYCLETIME >= 14 && x.CYCLETIME <= 15)
                                  || (model.IOMGraphDashboardRangeId == 14 && x.CYCLETIME >= 16)
                                  )
                                    select new IOMDashboadReportList
                                    {
                                        IOMHEADERID = x.IOMHEADERID,
                                        STATUS = x.STATUS,
                                        PROCESS_STATUS = x.PROCESS_STATUS,
                                        DATEADDED = x.DATEADDED,
                                        ADDEDBY = x.ADDEDBY,
                                        IOMCATID = x.IOMCATID,
                                        SYKIID = x.SYKIID,
                                        KICODE = x.KICODE,
                                        OPERATIONID = x.OPERATIONID,
                                        OPERATION = x.OPERATION,
                                        DIVISIONID = x.DIVISIONID,
                                        DIVISION = x.DIVISION,
                                        DEPARTMENTID = x.DEPARTMENTID,
                                        DEPARTMENT = x.DEPARTMENT,
                                        SECTIONID = x.SECTIONID,
                                        SECTION = x.SECTION,
                                        CATDESC = x.CATDESC,
                                        ADDEDBYNAME = x.ADDEDBYNAME,
                                        CYCLETIME = x.CYCLETIME,
                                        LASTACTIONDATE = x.LAST_MODIFIED_ON,
                                        LASTACTIONTAKENBY = x.LAST_MODIFIED_BY_ECODE,
                                        LASTACTIONTAKENBYNAME = x.LASTMODIFIED_NAME,
                                        PendingAt = x.PENDINGWITH_ECODE > 0 ? $"{x.PENDINGWITH_NAME}-{x.PENDINGWITH_ECODE} [Since - {x.PENDINGTIME} {(x.PENDINGTIME > 1 ? "Working days" : "Working day")}]" : "",
                                        PendingAtEmpCode = x.PENDINGWITH_ECODE
                                    }).ToList();

                List<SYPARAMETERS> objSyParameters = _IOMDBContext.SYPARAMETERS.Where(x => new string[] { "IOM_DESG_GRP_SRM", "IOM_DESG_GRP_TRM" }.Contains(x.PARAMNAME)).ToList();
                List<long> SeniorManagementIds = objSyParameters.Where(x => x.PARAMNAME == "IOM_DESG_GRP_SRM").FirstOrDefault().PARAMVALUE.Split(',').Select(x => Convert.ToInt64(x)).ToList();
                List<long> TeamMemberIds = objSyParameters.Where(x => x.PARAMNAME == "IOM_DESG_GRP_TRM").FirstOrDefault().PARAMVALUE.Split(',').Select(x => Convert.ToInt64(x)).ToList();
                List<decimal> pendingWithEmployees = searchResult.Select(x => x.PendingAtEmpCode ?? 0).Distinct().ToList();
                List<decimal> lastActionTakenByEmployees = searchResult.Select(x => x.LASTACTIONTAKENBY ?? 0).Distinct().ToList();
                List<Tuple<int, string>> orgLevels = new List<Tuple<int, string>>()
                {
                    new Tuple<int, string>(1, "Senior Management"),
                    new Tuple<int, string>(2, "Operating Head"),
                    new Tuple<int, string>(3, "Division Head"),
                    new Tuple<int, string>(4, "Department Head"),
                    new Tuple<int, string>(5, "Section Head"),
                    new Tuple<int, string>(6, "Team Member"),
                    new Tuple<int, string>(7, "Undefined Designation")
                };
                //var employeeDesignations = (from v in _IOMDBContext.VW_ASSOCIATELVLDETAILS
                //                            join d in _IOMDBContext.ADDESIGNATION on v.ADDESIGNATIONID equals d.ADDESIGNATIONID
                //                            where v.SYKI == model.searchIOM.KIID && (pendingWithEmployees.Contains(v.ADEMPCODE) || lastActionTakenByEmployees.Contains(v.ADEMPCODE))
                //                            select new
                //                            {
                //                                v.ADEMPCODE,
                //                                Designation = !string.IsNullOrEmpty(v.FUNCTIONALDESIGNATION) ? v.FUNCTIONALDESIGNATION : (SeniorManagementIds.Contains(v.ADDESIGNATIONID ?? 0) ? "Senior Management" : TeamMemberIds.Contains(v.ADDESIGNATIONID ?? 0) ? "Team Member" : "Undefined Designation")
                //                            }).ToList();
                var employee = (from v in _IOMDBContext.VW_ASSOCIATELVLDETAILS
                                join d in _IOMDBContext.ADDESIGNATION on v.ADDESIGNATIONID equals d.ADDESIGNATIONID
                                where v.SYKI == model.searchIOM.KIID && (pendingWithEmployees.Contains(v.ADEMPCODE) || lastActionTakenByEmployees.Contains(v.ADEMPCODE))
                                select v);
                var employeeDesignations = (from v in employee.AsEnumerable()
                                            select new
                                            {
                                                v.ADEMPCODE,
                                                Designation = !string.IsNullOrEmpty(v.FUNCTIONALDESIGNATION) ? v.FUNCTIONALDESIGNATION : (SeniorManagementIds.Contains(v.ADDESIGNATIONID ?? 0) ? "Senior Management" : TeamMemberIds.Contains(v.ADDESIGNATIONID ?? 0) ? "Team Member" : "Undefined Designation")
                                            }).ToList();
                searchResult.ForEach(x =>
                {
                    x.OrgLevel = employeeDesignations.Where(y => y.ADEMPCODE == (x.PendingAtEmpCode == 0 ? x.LASTACTIONTAKENBY : x.PendingAtEmpCode)).FirstOrDefault()?.Designation;
                });

                List<GroupedIOMByDesignation> groupedByDesignation = new List<GroupedIOMByDesignation>();
                if (data.SearchResult.Select(x => x.PROCESS_STATUS).Distinct().Count() == 1 && data.SearchResult.Select(x => x.PROCESS_STATUS).Distinct().FirstOrDefault() == 2) //For completed status, chart will be different
                {
                    if (model.IOMGraphDashboardRangeId == 1) //data requested for 0-5 days
                    {
                        groupedByDesignation = (from g in searchResult
                                                group g by g.OrgLevel into g
                                                join l in orgLevels on g.Key equals l.Item2
                                                orderby l.Item1
                                                select new GroupedIOMByDesignation
                                                {
                                                    Designation = g.Key,
                                                    RecordsByRange = new List<IOMDashboardGraphRecordCountByRange>
                                                {
                                                    new IOMDashboardGraphRecordCountByRange
                                                    {
                                                        Range = IOMGraphDashboardRange.SubRangeMap_0_5[5],
                                                        RecordCount = g.Count(x=> x.CYCLETIME >= 0 && x.CYCLETIME <= 2)
                                                    },
                                                    new IOMDashboardGraphRecordCountByRange
                                                    {
                                                        Range = IOMGraphDashboardRange.SubRangeMap_0_5[6],
                                                        RecordCount = g.Count(x=> x.CYCLETIME == 3)
                                                    },
                                                    new IOMDashboardGraphRecordCountByRange
                                                    {
                                                        Range = IOMGraphDashboardRange.SubRangeMap_0_5[7],
                                                        RecordCount = g.Count(x=> x.CYCLETIME >= 4 && x.CYCLETIME <= 5)
                                                    }
                                                }
                                                }).ToList();
                    }
                    else if (model.IOMGraphDashboardRangeId == 2) //data requested for 6-10 days
                    {
                        groupedByDesignation = (from g in searchResult
                                                group g by g.OrgLevel into g
                                                join l in orgLevels on g.Key equals l.Item2
                                                orderby l.Item1
                                                select new GroupedIOMByDesignation
                                                {
                                                    Designation = g.Key,
                                                    RecordsByRange = new List<IOMDashboardGraphRecordCountByRange>
                                                {
                                                    new IOMDashboardGraphRecordCountByRange
                                                    {
                                                        Range = IOMGraphDashboardRange.SubRangeMap_6_10[8],
                                                        RecordCount = g.Count(x=> x.CYCLETIME >= 6 && x.CYCLETIME <= 7)
                                                    },
                                                    new IOMDashboardGraphRecordCountByRange
                                                    {
                                                        Range = IOMGraphDashboardRange.SubRangeMap_6_10[9],
                                                        RecordCount = g.Count(x=> x.CYCLETIME == 8)
                                                    },
                                                    new IOMDashboardGraphRecordCountByRange
                                                    {
                                                        Range = IOMGraphDashboardRange.SubRangeMap_6_10[10],
                                                        RecordCount = g.Count(x=> x.CYCLETIME >= 9 && x.CYCLETIME <= 10)
                                                    }
                                                }
                                                }).ToList();
                    }
                    else if (model.IOMGraphDashboardRangeId == 3) //data requested for 11-15 days
                    {
                        groupedByDesignation = (from g in searchResult
                                                group g by g.OrgLevel into g
                                                join l in orgLevels on g.Key equals l.Item2
                                                orderby l.Item1
                                                select new GroupedIOMByDesignation
                                                {
                                                    Designation = g.Key,
                                                    RecordsByRange = new List<IOMDashboardGraphRecordCountByRange>
                                                {
                                                    new IOMDashboardGraphRecordCountByRange
                                                    {
                                                        Range = IOMGraphDashboardRange.SubRangeMap_11_15[11],
                                                        RecordCount = g.Count(x=> x.CYCLETIME >= 11 && x.CYCLETIME <= 12)
                                                    },
                                                    new IOMDashboardGraphRecordCountByRange
                                                    {
                                                        Range = IOMGraphDashboardRange.SubRangeMap_11_15[12],
                                                        RecordCount = g.Count(x=> x.CYCLETIME == 13)
                                                    },
                                                    new IOMDashboardGraphRecordCountByRange
                                                    {
                                                        Range = IOMGraphDashboardRange.SubRangeMap_11_15[13],
                                                        RecordCount = g.Count(x=> x.CYCLETIME >= 14 && x.CYCLETIME <= 15)
                                                    }
                                                }
                                                }).ToList();
                    }
                    else if (model.IOMGraphDashboardRangeId == 4) //data requested for >15 days
                    {
                        groupedByDesignation = (from g in searchResult
                                                group g by g.OrgLevel into g
                                                join l in orgLevels on g.Key equals l.Item2
                                                orderby l.Item1
                                                select new GroupedIOMByDesignation
                                                {
                                                    Designation = g.Key,
                                                    RecordsByRange = new List<IOMDashboardGraphRecordCountByRange>
                                                {
                                                    new IOMDashboardGraphRecordCountByRange
                                                    {
                                                        Range = IOMGraphDashboardRange.SubRangeMap_GTE_16[14],
                                                        RecordCount = g.Count(x=> x.CYCLETIME >= 16)
                                                    }
                                                }
                                                }).ToList();
                    }
                }
                else
                {
                    groupedByDesignation = (from g in searchResult
                                            group g by g.OrgLevel into g
                                            join l in orgLevels on g.Key equals l.Item2
                                            orderby l.Item1
                                            select new GroupedIOMByDesignation
                                            {
                                                Designation = g.Key,
                                                CountOfIOMs = g.Count()
                                            }).ToList();
                }


                model.AggregatedSearchResult = groupedByDesignation;
                model.SearchResult = searchResult;
            }

            return model;
        }
        #endregion
        // Added by TTL on 17-July-2025 against SR93758 > CR5975 - End

        //Added by TTL on 28th Oct 2025 against SR109889 > CR7269 - Start
        public short SaveIOMPRLink(long AddedBy, long IOMHEADERID, List<string> SelectedIndentNos)
        {
            short retVal = 0;
            try
            {
                DGIT_PR_IOM_LINKING DPIL = _IOMDBContext.DGIT_PR_IOM_LINKING.FirstOrDefault(x => x.DPIL_IOM_HDR_ID == IOMHEADERID);
                if (DPIL != null)
                {
                    _IOMDBContext.DGIT_PR_IOM_LINKING.Remove(DPIL);
                    _IOMDBContext.SaveChanges();
                }
                foreach (string IndentNo in SelectedIndentNos)
                {
                    DPIL = new();
                    DPIL.DPIL_IOM_HDR_ID = IOMHEADERID;
                    DPIL.DPIL_INDENT_NO = IndentNo;
                    DPIL.COLUMN1 = AddedBy.ToString();
                    DPIL.COLUMN2 = DateTime.Now;
                    DPIL.COLUMN3 = AddedBy.ToString();
                    DPIL.DPIL_UPDATEDATE = DateTime.Now;
                    DPIL.DPIL_STATUS = 1;
                    _IOMDBContext.DGIT_PR_IOM_LINKING.Add(DPIL);
                }
                _IOMDBContext.SaveChanges();
                retVal = 1;
            }
            catch (Exception)
            {
                retVal = -1;
                throw;
            }
            return retVal;
        }

        public List<PRDetailViewModel> GetPRDocumentsByIndentNo(List<string> IndentNos)
        {
            try
            {
                var docList = (
                                from prh in _IOMDBContext.DGIT_PRHEADER
                                where IndentNos.Contains(prh.INDENT_NO) && prh.STATUS == 1 && prh.PROCESS_STATUS == 2
                                let latestPRHeaderId = (
                                    from inner in _IOMDBContext.DGIT_PRHEADER
                                    where inner.INDENT_NO == prh.INDENT_NO && inner.STATUS == 1 && inner.PROCESS_STATUS == 2
                                    orderby inner.PRHEADERID descending
                                    select inner.PRHEADERID
                                ).FirstOrDefault()
                                where prh.PRHEADERID == latestPRHeaderId
                                join prd in _IOMDBContext.DGIT_PRDETAIL on prh.PRHEADERID equals prd.PRHEADERID
                                select new PRDetailViewModel
                                {
                                    PRHEADERID = prh.PRHEADERID,
                                    PRNo = prh.INDENT_NO,
                                    ADDITIONAL_INFO = prd.ADDITIONAL_INFO,
                                    FILENAME = prd.FILENAME,
                                    DOC_TYPE = prd.DOC_TYPE
                                }
                            ).OrderBy(x => x.PRNo).ToList();

                return docList;
            }
            catch (Exception)
            {

                throw;
            }
        }
        //Added by TTL on 28th Oct 2025 against SR109889 > CR7269 - End
    }
}
