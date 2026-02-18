using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.Infrastructure.Repositories;
using ePortal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ePortal.Infrastructure.Repositories
{
    public class PORepository
    {
        private EPortalDBContext _PoDBContext;
        private ICProcessBDContext _ICDBContext;
        CommonRepository _CommonRepo;
        private SYKI _Syki;
        public PORepository(EPortalDBContext objEPortalDBContext, ICProcessBDContext icProcessBDContext, CommonRepository commonRepo)
        {

            _ICDBContext = icProcessBDContext;
            _PoDBContext = objEPortalDBContext;
            _Syki = _PoDBContext.SYKI.Where(x => x.ACTIVE == 1).FirstOrDefault();
            _CommonRepo = commonRepo;
        }

        public List<VendorViewModel> GetVendorList()
        {
            var iList = (from data in _PoDBContext.FINVENDORMASTERMST
                         where data.STATUS == 1
                         select new VendorViewModel
                         {
                             VENDORCODE = data.VENDORCODE,
                             VENDORNAME = data.NAME1,
                             VENDOREMAIL = data.EMAIL1
                         }).OrderBy(o => o.VENDORNAME).ToList();
            return iList;
        }

        public Tuple<short, long> SavePORequest(POHeaderViewModel model)
        {
            short retVal = 0; long retHeaderId = 0;
            Tuple<short, long> _retVal_tuple;
            using (IDbContextTransaction transaction = _PoDBContext.Database.BeginTransaction())
            {
                try
                {
                    DGIT_POHEADER DPH = new DGIT_POHEADER();
                    int FlagAdd = 0;
                    if (model.POHEADERID > 0)
                    {
                        //if (_PoDBContext.DGIT_POHEADER.Any(x => x.POHEADERID != model.POHEADERID && x.PONO == model.PONO && (x.PROCESS_STATUS == 1 || x.PROCESS_STATUS == 0)))
                        var existingPo = _PoDBContext.DGIT_POHEADER.FirstOrDefault(x => x.POHEADERID != model.POHEADERID && x.PONO == model.PONO && (x.PROCESS_STATUS == 1 || x.PROCESS_STATUS == 0));
                        if (existingPo != null)
                        {
                            retVal = 2;
                            return _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId); //// -- record already exist.
                        }
                        DPH = _PoDBContext.DGIT_POHEADER.Where(x => x.POHEADERID == model.POHEADERID).SingleOrDefault();

                    }
                    else
                    {
                        //if (_PoDBContext.DGIT_POHEADER.Any(x => x.POHEADERID != model.POHEADERID && x.PONO == model.PONO && (x.PROCESS_STATUS == 1 || x.PROCESS_STATUS == 0)))
                        var existingPo = _PoDBContext.DGIT_POHEADER.FirstOrDefault(x => x.POHEADERID != model.POHEADERID && x.PONO == model.PONO && (x.PROCESS_STATUS == 1 || x.PROCESS_STATUS == 0));
                        if (existingPo != null)
                        {
                            retVal = 2;
                            return _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId); //// -- record already exist.
                        }

                        DPH = new DGIT_POHEADER();
                        if (_PoDBContext.DGIT_POHEADER.Count() == 0)
                        {
                            DPH.POHEADERID = 1;
                        }
                        else
                        {
                            DPH.POHEADERID = _PoDBContext.DGIT_POHEADER.Max(x => x.POHEADERID) + 1;
                        }
                        FlagAdd = 1;
                    }
                    if (model.IsFinalSubmit == 0)
                    {

                        DPH.PONO = model.PONO;
                        DPH.VENDORID = model.VENDORID;
                        DPH.VENDORMAILID = model.VENDORMAILID;
                        DPH.CONTRACT_NO = model.CONTRACT_NO;
                        DPH.HIGH_URGENCY = (model.HIGH_URGENCY == 1 ? (short)1 : (short)0);
                        //DPH.REMARK = model.REMARK;
                        DPH.PO_DESC = model.PO_DESC;
                        DPH.VERSION_NO = model.VERSIONNO;
                        DPH.PLANTID = model.Plant;
                    }
                    else
                    {

                        var poDetail = _PoDBContext.DGIT_PODETAIL
                        .Where(x => x.POHEADERID == model.POHEADERID && x.DOC_TYPE == "PO")
                        .FirstOrDefault();

                        //var poDetail = _PoDBContext.DGIT_PODETAIL.FirstOrDefault(x => x.POHEADERID == model.POHEADERID && x.DOC_TYPE == "PO");

                        if (poDetail == null)
                        //if (!_PoDBContext.DGIT_PODETAIL.Any(x => x.POHEADERID == model.POHEADERID && x.DOC_TYPE == "PO"))
                        {
                            retVal = 3;
                            return _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId); //// -- PO file not exist.
                        }
                    }
                    DPH.PROCESS_STATUS = model.PROCESS_STATUS ?? 0;
                    DPH.STATUS = model.STATUS ?? 0;
                    DPH.REMARK = model.REMARK;
                    if (FlagAdd == 1)
                    {
                        DPH.ADDEDBY = model.ADDEDBY ?? 0;
                        DPH.DATEADDED = DateTime.Now;
                    }
                    else
                    {
                        DPH.UPDATEDBY = model.UPDATEDBY;
                        DPH.UPDATEDATE = DateTime.Now;
                    }
                    _PoDBContext.Entry(DPH).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                    _PoDBContext.SaveChanges();

                    /// --- Save PO Detail --- ///
                    if (model.poDetail != null)
                    {
                        if (model.poDetail.Count > 0)
                        {
                            SavePODetails(model.ADDEDBY ?? 0, DPH.POHEADERID, model.poDetail);
                        }
                    }

                    /// --- Save PR Mapping Dtl --- ///
                    if (model.poMapping != null)
                    {
                        if (model.poMapping.Count > 0)
                        {
                            SavePOMapping(model.ADDEDBY ?? 0, DPH.POHEADERID, model.poMapping);
                        }
                    }

                    /// --- Save PO Approval Auth Seq ---///
                    if (model.poAuthSeq != null)
                    {
                        if (model.poAuthSeq.Count > 0)
                        {
                            SaveAppAuthSeq(model.ADDEDBY ?? 0, DPH.POHEADERID, model.poAuthSeq);

                            /// --- Save PO Approval Authority --- ///
                            POAppAuthSeqViewModel seqModel = model.poAuthSeq.OrderBy(o => o.APP_SEQ).FirstOrDefault();
                            if (seqModel != null)
                            {
                                SavePOAppHis(model.ADDEDBY ?? 0, DPH.POHEADERID, seqModel);
                            }
                        }
                    }

                    if (model.poAuthSkipList != null)
                    {
                        if (model.poAuthSkipList.Count > 0)
                        {
                            SaveSkipAuth(model.ADDEDBY ?? 0, DPH.POHEADERID, model.poAuthSkipList);
                        }
                    }

                    transaction.Commit();
                    retVal = 1;
                    retHeaderId = DPH.POHEADERID;
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

        public short SavePODetails(long AddedBy, long POHeaderId, List<PODetailViewModel> PDVMList)
        {
            short retVal = 0;
            foreach (PODetailViewModel PDVM in PDVMList)
            {
                DGIT_PODETAIL DPD = new DGIT_PODETAIL();
                int FlagAdd = 0;
                if (POHeaderId > 0)
                {
                    //DPD = _PoDBContext.DGIT_PODETAIL.Where(x => x.POHEADERID == POHeaderId && x.DOC_TYPE == PDVM.DOC_TYPE && x.FILENAME == PDVM.FILENAME).SingleOrDefault();
                    //if (DPD != null)
                    //{
                    //    _PoDBContext.DGIT_PODETAIL.Remove(DPD);
                    //    _PoDBContext.SaveChanges();
                    //}
                    if (PDVM.DOC_TYPE == "PO")
                    {
                        DPD = _PoDBContext.DGIT_PODETAIL.Where(x => x.POHEADERID == POHeaderId && x.DOC_TYPE == PDVM.DOC_TYPE).FirstOrDefault();
                        if (DPD != null)
                        {
                            _PoDBContext.DGIT_PODETAIL.Remove(DPD);
                            _PoDBContext.SaveChanges();
                        }
                    }

                    DPD = new DGIT_PODETAIL();
                    if (_PoDBContext.DGIT_PODETAIL.Count() == 0)
                    {
                        DPD.PODTL_ID = 1;
                    }
                    else
                    {
                        DPD.PODTL_ID = _PoDBContext.DGIT_PODETAIL.Max(x => x.PODTL_ID) + 1;
                    }
                    FlagAdd = 1;

                    DPD.POHEADERID = POHeaderId;
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
                    _PoDBContext.Entry(DPD).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                    _PoDBContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }

        public void SavePOMapping(long AddedBy, long POHeaderId, List<POMappingViewModel> mappingList)
        {
            //string[] strArray = _prNos[0].Split(',');
            List<DGIT_POPRLINK_DTL> prList = _PoDBContext.DGIT_POPRLINK_DTL.Where(x => x.POHEADERID == POHeaderId).ToList();
            if (prList.Count > 0)
            {
                _PoDBContext.DGIT_POPRLINK_DTL.RemoveRange(prList);
                _PoDBContext.SaveChanges();
            }
            foreach (POMappingViewModel obj in mappingList)
            {
                DGIT_POPRLINK_DTL DPLD = new DGIT_POPRLINK_DTL();
                int FlagAdd = 0;
                if (!string.IsNullOrEmpty(obj.PRNO))
                {
                    DPLD = _PoDBContext.DGIT_POPRLINK_DTL.Where(x => x.PRNO == obj.PRNO && x.POHEADERID == POHeaderId).SingleOrDefault();
                    if (DPLD == null)
                    {
                        DPLD = new DGIT_POPRLINK_DTL();
                        if (_PoDBContext.DGIT_POPRLINK_DTL.Count() == 0)
                        {
                            DPLD.POPRLNK_ID = 1;
                        }
                        else
                        {
                            DPLD.POPRLNK_ID = _PoDBContext.DGIT_POPRLINK_DTL.Max(x => x.POPRLNK_ID) + 1;
                        }
                        FlagAdd = 1;
                    }
                    DPLD.POHEADERID = POHeaderId;
                    DPLD.PRNO = obj.PRNO;
                    DPLD.STATUS = 1;
                    if (FlagAdd == 1)
                    {
                        DPLD.ADDEDBY = AddedBy;
                        DPLD.ADDEDDATE = DateTime.Now;
                    }
                    else
                    {
                        DPLD.UPDATEDBY = AddedBy;
                        DPLD.UPDATEDATE = DateTime.Now;
                    }
                    _PoDBContext.Entry(DPLD).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
                    _PoDBContext.SaveChanges();
                }
            }
        }

        public void SaveAppAuthSeq(long AddedBy, long POHeaderId, List<POAppAuthSeqViewModel> PSVMList)
        {
            //// --- Delete recode ---////
            List<DGIT_POAPPAUTHSEQ> SeqList = _PoDBContext.DGIT_POAPPAUTHSEQ.Where(t => t.POID == POHeaderId).ToList();
            if (SeqList.Count > 0)
            {
                _PoDBContext.DGIT_POAPPAUTHSEQ.RemoveRange(SeqList);
                _PoDBContext.SaveChanges();
            }

            foreach (POAppAuthSeqViewModel PSVM in PSVMList.OrderBy(o => o.APP_SEQ).ToList())
            {
                DGIT_POAPPAUTHSEQ DAAS = new DGIT_POAPPAUTHSEQ();
                if (_PoDBContext.DGIT_POAPPAUTHSEQ.Count() == 0)
                {
                    DAAS.POAPPAUTH_ID = 1;
                }
                else
                {
                    DAAS.POAPPAUTH_ID = _PoDBContext.DGIT_POAPPAUTHSEQ.Max(x => x.POAPPAUTH_ID) + 1;
                }
                DAAS.POID = POHeaderId;
                DAAS.ADEMPCODE = PSVM.ADEMPCODE;
                DAAS.APP_SEQ = PSVM.APP_SEQ;
                DAAS.APPTYPE = PSVM.APPTYPE;
                DAAS.STATUS = 1;
                DAAS.ADDEDBY = AddedBy;
                DAAS.ADDEDDATE = DateTime.Now;
                _PoDBContext.Entry(DAAS).State = EntityState.Added;
                _PoDBContext.SaveChanges();
            }
        }

        public void SavePOAppHis(long AddedBy, long POHeaderId, POAppAuthSeqViewModel PSVM)
        {
            DGIT_POAPPHISTORY DPAH = new DGIT_POAPPHISTORY();
            int FlagAdd = 0;
            // DPAH = _PoDBContext.DGIT_POAPPHISTORY.Where(d => d.ADEMPCODE == PSVM.ADEMPCODE && d.POID == POHeaderId).FirstOrDefault();
            DPAH = _PoDBContext.DGIT_POAPPHISTORY.Where(d => d.ADEMPCODE == PSVM.ADEMPCODE && d.POID == POHeaderId && d.APPROVAL_STATUS == 0).FirstOrDefault();
            if (DPAH == null)
            {
                DPAH = new DGIT_POAPPHISTORY();
                if (_PoDBContext.DGIT_POAPPHISTORY.Count() == 0)
                {
                    DPAH.POAPPHISTORY_ID = 1;
                }
                else
                {
                    DPAH.POAPPHISTORY_ID = _PoDBContext.DGIT_POAPPHISTORY.Max(x => x.POAPPHISTORY_ID) + 1;
                }
                FlagAdd = 1;
            }
            DPAH.POID = POHeaderId;
            DPAH.ADEMPCODE = PSVM.ADEMPCODE;
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
            _PoDBContext.Entry(DPAH).State = FlagAdd == 1 ? EntityState.Added : EntityState.Modified;
            _PoDBContext.SaveChanges();
        }

        public void SaveSkipAuth(long AddedBy, long POHEADERID, List<POAppSkipViewModel> PASList)
        {
            //// --- Delete recode ---////
            List<DGIT_POAPPSKIP> skipList = _PoDBContext.DGIT_POAPPSKIP.Where(t => t.POID == POHEADERID).ToList();
            if (skipList.Count > 0)
            {
                _PoDBContext.DGIT_POAPPSKIP.RemoveRange(skipList);
                _PoDBContext.SaveChanges();
            }

            foreach (POAppSkipViewModel PSVM in PASList)
            {
                DGIT_POAPPSKIP DPAS = new DGIT_POAPPSKIP();
                if (_PoDBContext.DGIT_POAPPSKIP.Count() == 0)
                {
                    DPAS.POAPPSKIP_ID = 1;
                }
                else
                {
                    DPAS.POAPPSKIP_ID = _PoDBContext.DGIT_POAPPSKIP.Max(x => x.POAPPSKIP_ID) + 1;
                }
                DPAS.POID = POHEADERID;
                DPAS.ADEMPCODE = PSVM.ADEMPCODE;
                DPAS.SKIPREMARK = PSVM.SKIPREMARK;
                DPAS.STATUS = 1;
                DPAS.ADDEDBY = AddedBy;
                DPAS.ADDEDDATE = DateTime.Now;
                _PoDBContext.Entry(DPAS).State = EntityState.Added;
                _PoDBContext.SaveChanges();
            }
        }

        //Below added by aumento as on 02092024 for SR70820===================================================
        public List<long> GetValidEmployeeCodes(long empCode)
        {
            // Step 1: Fetch OPERATIONID based on the given employee code and conditions
            var operationId = _PoDBContext.VW_ASSOCIATELVLDETAILS
                .Where(a => a.ADEMPCODE == empCode && a.SYKI == _Syki.SYKIID && a.ACTIVE == 1)
                .Select(a => a.OPERATIONID)
                .FirstOrDefault();

            // Check if operationId is null or default
            if (operationId == 0)
            {
                // Return an empty list if no operationId is found
                return new List<long>();
            }

            // Step 2: Fetch DISTINCT DEPARTMENTID and DIVISIONID based on the OPERATIONID and other conditions
            var departmentIds = _PoDBContext.VW_ASSOCIATELVLDETAILS
                .Where(a => a.OPERATIONID == operationId && a.SYKI == _Syki.SYKIID && a.ACTIVE == 1 && a.DEPARTMENTID != null)
                .Select(a => a.DEPARTMENTID)
                .Distinct()
                .ToList();

            var divisionIds = _PoDBContext.VW_ASSOCIATELVLDETAILS
                .Where(a => a.OPERATIONID == operationId && a.SYKI == _Syki.SYKIID && a.ACTIVE == 1 && a.DIVISIONID != null)
                .Select(a => a.DIVISIONID)
                .Distinct()
                .ToList();

            //added by aumento for SR82542
            var sectionHeadIds = _PoDBContext.VW_ASSOCIATELVLDETAILS
                .Where(a => a.OPERATIONID == operationId && a.SYKI == _Syki.SYKIID && a.ACTIVE == 1 && a.SECTIONID != null)
                .Select(a => a.SECTIONID)
                .Distinct()
                .ToList();

            //added by aumento for SR82542
            // Step 3: Fetch all ADORGLEVELHEAD records with ADEMPCODE where ADORGLEVELID matches either DEPARTMENTID or DIVISIONID
            var adOrgLevelHeadCodes = _PoDBContext.ADORGLEVELHEAD
                //.Where(b => (departmentIds.Contains(b.ADORGLEVELID) || divisionIds.Contains(b.ADORGLEVELID)) && b.ISACTIVE == 1)
                .Where(b => (departmentIds.Contains(b.ADORGLEVELID) || divisionIds.Contains(b.ADORGLEVELID) || sectionHeadIds.Contains(b.ADORGLEVELID)) && b.ISACTIVE == 1) //added by aumento for SR82542
                .Select(b => b.ADEMPCODE)
                .ToList();

            // Return the list of valid employee codes
            return adOrgLevelHeadCodes;
        }


        public Employee_Details GetAuthEmpById(long empCode, long loginempcode)
        {

            var validEmployeeCodes = GetValidEmployeeCodes(loginempcode);
            if (!validEmployeeCodes.Contains(empCode))
            {
                // Return an empty Employee_Details object if empCode is not valid
                return new Employee_Details();
            }

            var _obj = (from data in _PoDBContext.ADEMPLOYEE.Where(v => v.ADEMPCODE == empCode && v.ACTIVE == 1)
                        join _VW in _PoDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VW.ADEMPCODE
                        join _Desg in _PoDBContext.ADDESIGNATION on _VW.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
                        where _VW.SYKI == _Syki.SYKIID && _VW.FUNCTIONALDESIGNATION != null
                        select new Employee_Details
                        {
                            _ECode = data.ADEMPCODE,
                            _EFirstName = data.FIRSTNAME,
                            _ELastName = data.LASTNAME,
                            _EName = data.FIRSTNAME + " " + data.LASTNAME,
                            _EmailId = data.EMAILID,
                            _DesigId = _VW.ADDESIGNATIONID,
                            _Desig = (_VW.FUNCTIONALDESIGNATION == "" || _VW.FUNCTIONALDESIGNATION == null) ? _Desg.DESCRIP : _VW.FUNCTIONALDESIGNATION,
                            _FnDesigId = _VW.ADFUNCTIONALDESIGNATIONID,
                            _FnDesig = _VW.FUNCTIONALDESIGNATION
                        }).FirstOrDefault();

            return _obj;

            //var _obj = (from data in _PoDBContext.ADEMPLOYEE.Where(v => v.ADEMPCODE == empCode && v.ACTIVE == 1)
            //            join _VW in _PoDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VW.ADEMPCODE
            //            join _Desg in _PoDBContext.ADDESIGNATION on _VW.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
            //            where _VW.SYKI == _Syki.SYKIID && _VW.FUNCTIONALDESIGNATION != null
            //            select new Employee_Details
            //            {
            //                _ECode = data.ADEMPCODE,
            //                _EFirstName = data.FIRSTNAME,
            //                _ELastName = data.LASTNAME,
            //                _EName = data.FIRSTNAME + " " + data.LASTNAME,
            //                _EmailId = data.EMAILID,
            //                _DesigId = _VW.ADDESIGNATIONID,
            //                _Desig = (_VW.FUNCTIONALDESIGNATION == "" || _VW.FUNCTIONALDESIGNATION == null) ? _Desg.DESCRIP : _VW.FUNCTIONALDESIGNATION,
            //                _FnDesigId = _VW.ADFUNCTIONALDESIGNATIONID,
            //                _FnDesig = _VW.FUNCTIONALDESIGNATION
            //            }).FirstOrDefault();
            //return _obj;
        }
        //=====================================================================================================

        public POHeaderViewModel GetPORequestById(long id)
        {
            //var _obj = (from data in _PoDBContext.DGIT_POHEADER.Where(x => x.POHEADERID == id)
            //            join _Vendor in _PoDBContext.FINVENDORMASTERMST on data.VENDORID equals _Vendor.VENDORCODE
            //            join _AddBy in _PoDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
            //            join _VWA in _PoDBContext.VW_ASSOCIATELVLDETAILS.Where(m => m.SYKI == _Syki.SYKIID) on data.ADDEDBY equals _VWA.ADEMPCODE into _vwdt
            //            from _VWAssociate in _vwdt.DefaultIfEmpty()
            //            //join _VWAssociate in _PoDBContext.VW_ASSOCIATELVLDETAILS on data.ADDEDBY equals _VWAssociate.ADEMPCODE
            //            //where _VWAssociate.SYKI == _Syki.SYKIID
            //            select new POHeaderViewModel
            //            {
            //                POHEADERID = data.POHEADERID,
            //                PONO = data.PONO,
            //                VENDORID = data.VENDORID,
            //                VENDORNAME = _Vendor.NAME1,
            //                VENDORMAILID = data.VENDORMAILID,
            //                CONTRACT_NO = data.CONTRACT_NO,
            //                HIGH_URGENCY = (data.HIGH_URGENCY == 1 ? true : false),
            //                PO_DESC = data.PO_DESC,
            //                REMARK = data.REMARK,
            //                ADDEDBYNAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
            //                DATEADDED = data.DATEADDED,
            //                PROCESS_STATUS=data.PROCESS_STATUS,
            //                Plant=(string.IsNullOrEmpty(data.PLANTID)?"":data.PLANTID),
            //                VERSIONNO= data.VERSION_NO,
            //                poDetail = (from _PODetail in _PoDBContext.DGIT_PODETAIL.Where(d => d.POHEADERID == data.POHEADERID)
            //                            where _PODetail.STATUS == 1
            //                            select new PODetailViewModel
            //                            {
            //                                PODTL_ID = _PODetail.PODTL_ID,
            //                                POHEADERID = _PODetail.POHEADERID,
            //                                DOC_TYPE = _PODetail.DOC_TYPE,
            //                                ADDITIONAL_INFO = _PODetail.ADDITIONAL_INFO,
            //                                FILENAME = _PODetail.FILENAME,
            //                            }).ToList(),
            //                poMapping = (from _POMapp in _PoDBContext.DGIT_POPRLINK_DTL.Where(l => l.POHEADERID == data.POHEADERID)
            //                             where _POMapp.STATUS == 1
            //                             select new POMappingViewModel
            //                             {
            //                                 POPRLNK_ID = _POMapp.POPRLNK_ID,
            //                                 POHEADERID = _POMapp.POHEADERID,
            //                                 PRNO = _POMapp.PRNO,
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
            //                poAuthSeq = (from _POSeq in _PoDBContext.DGIT_POAPPAUTHSEQ.Where(x => x.POID == data.POHEADERID)
            //                             join _AppSeqEmp in _PoDBContext.ADEMPLOYEE on _POSeq.ADEMPCODE equals _AppSeqEmp.ADEMPCODE
            //                             //join _Vw in _PoDBContext.VW_ASSOCIATELVLDETAILS on _POSeq.ADEMPCODE equals _Vw.ADEMPCODE
            //                             join _VWD in _PoDBContext.VW_ASSOCIATELVLDETAILS.Where(m => m.SYKI == _Syki.SYKIID) on _POSeq.ADEMPCODE equals _VWD.ADEMPCODE into _VWTMP
            //                             from _Vw in _VWTMP.DefaultIfEmpty()
            //                             join _Desgt in _PoDBContext.ADDESIGNATION on _Vw.ADDESIGNATIONID equals _Desgt.ADDESIGNATIONID into _desgtmp
            //                             from _Desg in _desgtmp.DefaultIfEmpty()
            //                            //where _Vw.SYKI == _Syki.SYKIID
            //                             select new POAppAuthSeqViewModel
            //                             {
            //                                 POAPPAUTH_ID = _POSeq.POAPPAUTH_ID,
            //                                 POID = _POSeq.POID,
            //                                 ADEMPCODE = _POSeq.ADEMPCODE,
            //                                 STATUS = _POSeq.STATUS,
            //                                 APP_SEQ = _POSeq.APP_SEQ,
            //                                 ADEMPNAME = _AppSeqEmp.FIRSTNAME + " " + _AppSeqEmp.LASTNAME,
            //                                 ADDESIGNATION = _Desg.DESCRIP,
            //                                 ADDEDDATE = _POSeq.ADDEDDATE,
            //                                 UPDATEBY = _POSeq.UPDATEBY,
            //                                 UPDATEDATE = _POSeq.UPDATEDATE,
            //                                 APPTYPE = _POSeq.APPTYPE,
            //                             }).OrderBy(b => b.APP_SEQ).ToList(),
            //                poAppHis = (from _POAppHis in _PoDBContext.DGIT_POAPPHISTORY.Where(x => x.POID == data.POHEADERID)
            //                            join _AppEmp in _PoDBContext.ADEMPLOYEE on _POAppHis.ADEMPCODE equals _AppEmp.ADEMPCODE
            //                            select new POAppHistoryViewModel
            //                            {
            //                                POAPPHISTORY_ID = _POAppHis.POAPPHISTORY_ID,
            //                                POID = _POAppHis.POID,
            //                                ADEMPCODE = _POAppHis.ADEMPCODE,
            //                                APPROVAL_STATUS = _POAppHis.APPROVAL_STATUS,
            //                                APPROVAL_REMARK = _POAppHis.APPROVAL_REMARK,
            //                                APPEMP_NAME = _AppEmp.FIRSTNAME + " " + _AppEmp.LASTNAME + " - [" + _AppEmp.ADEMPCODE + "]",
            //                                APP_EMAIL = _AppEmp.EMAILID,
            //                                APPEMP_CODE = _AppEmp.ADEMPCODE.ToString(),
            //                                ADDEDDATE = _POAppHis.ADDEDDATE,
            //                                UPDATEBY = _POAppHis.UPDATEBY,
            //                                UPDATEDATE = _POAppHis.UPDATEDATE,
            //                                APPROVALDATE = _POAppHis.APP_DATE
            //                            }).OrderBy(o => o.POAPPHISTORY_ID).ToList(),
            //                poAuthSkipList = (from _AuthSkip in _PoDBContext.DGIT_POAPPSKIP.Where(x => x.POID == data.POHEADERID)
            //                                  join _AppSkipEmp in _PoDBContext.ADEMPLOYEE on _AuthSkip.ADEMPCODE equals _AppSkipEmp.ADEMPCODE
            //                                  select new POAppSkipViewModel
            //                                  {
            //                                      POAPPSKIP_ID = _AuthSkip.POAPPSKIP_ID,
            //                                      POID = _AuthSkip.POID,
            //                                      ADEMPCODE = _AuthSkip.ADEMPCODE,
            //                                      STATUS = _AuthSkip.STATUS,
            //                                      ADEMPNAME = _AppSkipEmp.FIRSTNAME + " " + _AppSkipEmp.LASTNAME,
            //                                      ADDEDDATE = _AuthSkip.ADDEDDATE,
            //                                      SKIPREMARK = _AuthSkip.SKIPREMARK
            //                                  }).ToList(),
            //            }).FirstOrDefault();

            var var_poAuthSkipList = (from _AuthSkip in _PoDBContext.DGIT_POAPPSKIP.Where(x => x.POID == id)
                                      join _AppSkipEmp in _PoDBContext.ADEMPLOYEE on _AuthSkip.ADEMPCODE equals _AppSkipEmp.ADEMPCODE
                                      select new POAppSkipViewModel
                                      {
                                          POAPPSKIP_ID = _AuthSkip.POAPPSKIP_ID,
                                          POID = _AuthSkip.POID,
                                          ADEMPCODE = _AuthSkip.ADEMPCODE,
                                          STATUS = _AuthSkip.STATUS,
                                          ADEMPNAME = _AppSkipEmp.FIRSTNAME + " " + _AppSkipEmp.LASTNAME,
                                          ADDEDDATE = _AuthSkip.ADDEDDATE,
                                          SKIPREMARK = _AuthSkip.SKIPREMARK
                                      }).ToList();
            var var_poAppHis = (from _POAppHis in _PoDBContext.DGIT_POAPPHISTORY.Where(x => x.POID == id)
                                join _AppEmp in _PoDBContext.ADEMPLOYEE on _POAppHis.ADEMPCODE equals _AppEmp.ADEMPCODE
                                select new POAppHistoryViewModel
                                {
                                    POAPPHISTORY_ID = _POAppHis.POAPPHISTORY_ID,
                                    POID = _POAppHis.POID,
                                    ADEMPCODE = _POAppHis.ADEMPCODE,
                                    APPROVAL_STATUS = _POAppHis.APPROVAL_STATUS,
                                    APPROVAL_REMARK = _POAppHis.APPROVAL_REMARK,
                                    APPEMP_NAME = _AppEmp.FIRSTNAME + " " + _AppEmp.LASTNAME + " - [" + _AppEmp.ADEMPCODE + "]",
                                    APP_EMAIL = _AppEmp.EMAILID,
                                    APPEMP_CODE = _AppEmp.ADEMPCODE.ToString(),
                                    ADDEDDATE = _POAppHis.ADDEDDATE,
                                    UPDATEBY = _POAppHis.UPDATEBY,
                                    UPDATEDATE = _POAppHis.UPDATEDATE,
                                    APPROVALDATE = _POAppHis.APP_DATE
                                }).OrderBy(o => o.POAPPHISTORY_ID).ToList();
            var varpoAuthSeq = (from _POSeq in _PoDBContext.DGIT_POAPPAUTHSEQ.Where(x => x.POID == id)
                                join _AppSeqEmp in _PoDBContext.ADEMPLOYEE on _POSeq.ADEMPCODE equals _AppSeqEmp.ADEMPCODE
                                //join _Vw in _PoDBContext.VW_ASSOCIATELVLDETAILS on _POSeq.ADEMPCODE equals _Vw.ADEMPCODE
                                join _VWD in _PoDBContext.VW_ASSOCIATELVLDETAILS.Where(m => m.SYKI == _Syki.SYKIID) on _POSeq.ADEMPCODE equals _VWD.ADEMPCODE into _VWTMP
                                from _Vw in _VWTMP.DefaultIfEmpty()
                                join _Desgt in _PoDBContext.ADDESIGNATION on _Vw.ADDESIGNATIONID equals _Desgt.ADDESIGNATIONID into _desgtmp
                                from _Desg in _desgtmp.DefaultIfEmpty()
                                    //where _Vw.SYKI == _Syki.SYKIID
                                select new POAppAuthSeqViewModel
                                {
                                    POAPPAUTH_ID = _POSeq.POAPPAUTH_ID,
                                    POID = _POSeq.POID,
                                    ADEMPCODE = _POSeq.ADEMPCODE,
                                    STATUS = _POSeq.STATUS,
                                    APP_SEQ = _POSeq.APP_SEQ,
                                    ADEMPNAME = _AppSeqEmp.FIRSTNAME + " " + _AppSeqEmp.LASTNAME,
                                    ADDESIGNATION = _Desg.DESCRIP,
                                    ADDEDDATE = _POSeq.ADDEDDATE,
                                    UPDATEBY = _POSeq.UPDATEBY,
                                    UPDATEDATE = _POSeq.UPDATEDATE,
                                    APPTYPE = _POSeq.APPTYPE,
                                }).OrderBy(b => b.APP_SEQ).ToList();
            var var_poDetail = (from _PODetail in _PoDBContext.DGIT_PODETAIL.Where(d => d.POHEADERID == id)
                                where _PODetail.STATUS == 1
                                select new PODetailViewModel
                                {
                                    PODTL_ID = _PODetail.PODTL_ID,
                                    POHEADERID = _PODetail.POHEADERID,
                                    DOC_TYPE = _PODetail.DOC_TYPE,
                                    ADDITIONAL_INFO = _PODetail.ADDITIONAL_INFO,
                                    FILENAME = _PODetail.FILENAME,
                                }).ToList();
            var var_poMapping = (from _POMapp in _PoDBContext.DGIT_POPRLINK_DTL.Where(l => l.POHEADERID == id)
                                 where _POMapp.STATUS == 1
                                 select new POMappingViewModel
                                 {
                                     POPRLNK_ID = _POMapp.POPRLNK_ID,
                                     POHEADERID = _POMapp.POHEADERID,
                                     PRNO = _POMapp.PRNO,
                                 }).ToList();
            var _obj = (from data in _PoDBContext.DGIT_POHEADER.Where(x => x.POHEADERID == id)
                        join _Vendor in _PoDBContext.FINVENDORMASTERMST on data.VENDORID equals _Vendor.VENDORCODE
                        join _AddBy in _PoDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
                        join _VWA in _PoDBContext.VW_ASSOCIATELVLDETAILS.Where(m => m.SYKI == _Syki.SYKIID) on data.ADDEDBY equals _VWA.ADEMPCODE into _vwdt
                        from _VWAssociate in _vwdt.DefaultIfEmpty()
                            //join _VWAssociate in _PoDBContext.VW_ASSOCIATELVLDETAILS on data.ADDEDBY equals _VWAssociate.ADEMPCODE
                            //where _VWAssociate.SYKI == _Syki.SYKIID
                        select new POHeaderViewModel
                        {
                            POHEADERID = data.POHEADERID,
                            PONO = data.PONO,
                            VENDORID = data.VENDORID,
                            VENDORNAME = _Vendor.NAME1,
                            VENDORMAILID = data.VENDORMAILID,
                            CONTRACT_NO = data.CONTRACT_NO,
                            HIGH_URGENCY = (data.HIGH_URGENCY == 1 ? (short)1 : (short)0),
                            PO_DESC = data.PO_DESC,
                            REMARK = data.REMARK,
                            ADDEDBYNAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                            DATEADDED = data.DATEADDED,
                            PROCESS_STATUS = data.PROCESS_STATUS,
                            Plant = (string.IsNullOrEmpty(data.PLANTID) ? "" : data.PLANTID),
                            VERSIONNO = data.VERSION_NO,
                            poDetail = var_poDetail,
                            poMapping = var_poMapping,
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
                            poAuthSeq = varpoAuthSeq,
                            poAppHis = var_poAppHis,
                            poAuthSkipList = var_poAuthSkipList
                        }).FirstOrDefault();

            if (_obj.poAuthSeq.Count > 0)
            {
                long lastSendBackAppHisId = 0;
                DGIT_POAPPHISTORY lastSendBackAppHis = _PoDBContext.DGIT_POAPPHISTORY.Where(e => e.POID == _obj.POHEADERID && e.APPROVAL_STATUS == 2).OrderByDescending(o => o.POAPPHISTORY_ID).FirstOrDefault();
                if (lastSendBackAppHis != null)
                {
                    lastSendBackAppHisId = lastSendBackAppHis.POAPPHISTORY_ID;
                }
                foreach (POAppAuthSeqViewModel obj in _obj.poAuthSeq)
                {

                    //bool IsBeforeSendBackRecord = _PoDBContext.DGIT_POAPPHISTORY.Any(w => w.POID == obj.POID && w.POAPPHISTORY_ID <= lastSendBackAppHisId && w.ADEMPCODE == obj.ADEMPCODE);
                    var rs = _PoDBContext.DGIT_POAPPHISTORY.Where(w => w.POID == obj.POID && w.POAPPHISTORY_ID <= lastSendBackAppHisId && w.ADEMPCODE == obj.ADEMPCODE).ToList();
                    bool IsBeforeSendBackRecord = rs.Count > 0 ? true : false;

                    //if ((IsBeforeSendBackRecord ? (!_obj.poAppHis.Any(r => r.POID == obj.POID && r.ADEMPCODE == obj.ADEMPCODE && r.POAPPHISTORY_ID > lastSendBackAppHisId)) : (!_obj.poAppHis.Any(x => x.POID == obj.POID && x.ADEMPCODE == obj.ADEMPCODE))))

                    var poAppHisRecord = IsBeforeSendBackRecord
                        ? _obj.poAppHis.FirstOrDefault(r => r.POID == obj.POID && r.ADEMPCODE == obj.ADEMPCODE && r.POAPPHISTORY_ID > lastSendBackAppHisId)
                        : _obj.poAppHis.FirstOrDefault(x => x.POID == obj.POID && x.ADEMPCODE == obj.ADEMPCODE);

                    if (poAppHisRecord == null)
                    {
                        _obj.poAppHis.Add(new POAppHistoryViewModel
                        {
                            POAPPHISTORY_ID = 0,
                            POID = obj.POID,
                            ADEMPCODE = obj.ADEMPCODE,
                            APPROVAL_STATUS = 0,
                            APPROVAL_REMARK = "",
                            APPEMP_NAME = obj.ADEMPNAME + "[" + obj.ADEMPCODE + "]",
                        });
                    }
                }
            }
            return _obj;
        }

        public short POApproval(POAppHistoryViewModel PHVM)
        {
            short retVal = 0;
            using (IDbContextTransaction transaction = _PoDBContext.Database.BeginTransaction())
            {
                try
                {
                    DGIT_POAPPHISTORY DPAH = new DGIT_POAPPHISTORY();
                    if (PHVM.POID > 0)
                    {
                        /////////// Update Approval Status //////////
                        // DPAH = _PoDBContext.DGIT_POAPPHISTORY.Where(x => x.POID == PHVM.POID && x.ADEMPCODE == PHVM.ADEMPCODE).FirstOrDefault();
                        DPAH = _PoDBContext.DGIT_POAPPHISTORY.Where(x => x.POID == PHVM.POID && x.ADEMPCODE == PHVM.ADEMPCODE && x.APPROVAL_STATUS == 0).FirstOrDefault();
                        if (DPAH != null)
                        {
                            DPAH.APPROVAL_STATUS = PHVM.APPROVAL_STATUS;
                            DPAH.APPROVAL_REMARK = PHVM.APPROVAL_REMARK;
                            DPAH.UPDATEBY = PHVM.UPDATEBY;
                            DPAH.APP_DATE = DateTime.Now;
                            DPAH.UPDATEDATE = DateTime.Now;
                            _PoDBContext.Entry(DPAH).State = EntityState.Modified;
                            _PoDBContext.SaveChanges();
                        }

                        /// --- Save PO Approval Authority --- ///
                        POAppAuthSeqViewModel seqModel = new POAppAuthSeqViewModel();
                        if (PHVM.APPROVAL_STATUS == 1)
                        {
                            short nextSeq = Convert.ToInt16(_PoDBContext.DGIT_POAPPAUTHSEQ.Where(s => s.POID == PHVM.POID && s.ADEMPCODE == PHVM.ADEMPCODE).Select(s => s.APP_SEQ).FirstOrDefault() + 1);
                            long _nextEmpAuth = _PoDBContext.DGIT_POAPPAUTHSEQ.Where(a => a.APP_SEQ == nextSeq && a.POID == PHVM.POID).Select(s => s.ADEMPCODE).FirstOrDefault();
                            if (_nextEmpAuth == PHVM.ADEMPCODE)
                            {
                                nextSeq = Convert.ToInt16(nextSeq + 1);
                            }
                            seqModel = (from data in _PoDBContext.DGIT_POAPPAUTHSEQ.Where(a => a.APP_SEQ == nextSeq && a.POID == PHVM.POID)
                                        select new POAppAuthSeqViewModel
                                        {
                                            POAPPAUTH_ID = data.POAPPAUTH_ID,
                                            POID = data.POID,
                                            ADEMPCODE = data.ADEMPCODE,
                                            APP_SEQ = data.APP_SEQ,
                                            STATUS = data.STATUS,
                                        }).FirstOrDefault();
                            if (seqModel != null)
                            {
                                SavePOAppHis(PHVM.ADDEDBY, PHVM.POID, seqModel);
                            }
                        }

                        //////// Update Process Status ////////
                        DGIT_POHEADER DPH = new DGIT_POHEADER(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                        DPH = _PoDBContext.DGIT_POHEADER.Where(x => x.POHEADERID == PHVM.POID).SingleOrDefault();
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
                            else
                            {
                                DPH.PROCESS_STATUS = 1;
                            }
                            //DPH.HIGH_URGENCY = PHVM.High_Urgency == true ? (short)1 : (short)0;
                            DPH.HIGH_URGENCY = PHVM.High_Urgency > 0 ? (short)1 : (short)0;
                            DPH.UPDATEDBY = PHVM.UPDATEBY;
                            DPH.UPDATEDATE = DateTime.Now;
                            _PoDBContext.Entry(DPH).State = EntityState.Modified;
                            _PoDBContext.SaveChanges();
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

        public short POCancel(POHeaderViewModel PHVM)
        {
            short retVal = 0;
            using (IDbContextTransaction transaction = _PoDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (PHVM.POHEADERID > 0)
                    {
                        //////// Update Process Status ////////
                        DGIT_POHEADER DPH = new DGIT_POHEADER(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                        DPH = _PoDBContext.DGIT_POHEADER.Where(x => x.POHEADERID == PHVM.POHEADERID).SingleOrDefault();
                        if (DPH != null)
                        {
                            DPH.PROCESS_STATUS = 4;
                            DPH.UPDATEDBY = PHVM.UPDATEDBY;
                            DPH.UPDATEDATE = DateTime.Now;
                            _PoDBContext.Entry(DPH).State = EntityState.Modified;
                            _PoDBContext.SaveChanges();
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

        public List<PODetailViewModel> GetAttachmentDetail(long _POHEADERID)
        {
            return (from _PODetail in _PoDBContext.DGIT_PODETAIL.Where(d => d.POHEADERID == _POHEADERID)
                    where _PODetail.STATUS == 1
                    select new PODetailViewModel
                    {
                        PODTL_ID = _PODetail.PODTL_ID,
                        POHEADERID = _PODetail.POHEADERID,
                        DOC_TYPE = _PODetail.DOC_TYPE,
                        ADDITIONAL_INFO = _PODetail.ADDITIONAL_INFO,
                        FILENAME = _PODetail.FILENAME,
                    }).ToList();
        }

        public short DeleteAttachment(string fileName, string docType, long poHeaderId)
        {
            short retVal = 0;
            if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(docType) && poHeaderId > 0)
            {
                DGIT_PODETAIL DT = _PoDBContext.DGIT_PODETAIL.Where(x => x.FILENAME == fileName && x.DOC_TYPE == docType && x.POHEADERID == poHeaderId).FirstOrDefault();
                if (DT != null)
                {
                    _PoDBContext.DGIT_PODETAIL.Remove(DT);
                    _PoDBContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }

        public PRHeaderViewModel GetPRDetailByPOId(long poid)
        {
            var objprlist = _PoDBContext.DGIT_POPRLINK_DTL.Where(m => m.POHEADERID == poid).Select(m => m.PRNO).ToArray();
            var PRHeaderID = (from data in _PoDBContext.DGIT_PRHEADER.Where(m => m.PROCESS_STATUS == 2 && objprlist.Contains(m.INDENT_NO))
                              group data by data.INDENT_NO into g
                              select new { INDENT_NO = g.Key, MaxUid = g.Max(s => s.PRHEADERID) }
                              ).ToList().Select(m => m.MaxUid).ToArray();

            var _obj = (from data in _PoDBContext.DGIT_PRHEADER
                        where PRHeaderID.Contains(data.PRHEADERID)
                        select new PRHeaderViewModel
                        {
                            IndentNo = data.INDENT_NO,
                            IsFinalSubmit = 2,
                            prDetail = (from _POMapp in _PoDBContext.DGIT_PRDETAIL.Where(l => l.PRHEADERID == data.PRHEADERID)
                                        where _POMapp.STATUS == 1 && _POMapp.DOC_TYPE != "PR"
                                        select new PRDetailViewModel
                                        {
                                            ADDITIONAL_INFO = _POMapp.ADDITIONAL_INFO,
                                            DOC_TYPE = _POMapp.DOC_TYPE,
                                            FILENAME = _POMapp.FILENAME,
                                        }).ToList(),

                        }).FirstOrDefault();


            return _obj;
        }

        public long GetPRStatusByPOId(string[] pono)
        {
            int retval = 0;
            var _obj = (from _PR in _PoDBContext.DGIT_PRHEADER
                        where pono.Contains(_PR.INDENT_NO) && (_PR.PROCESS_STATUS != 2 && _PR.PROCESS_STATUS != 3 && _PR.PROCESS_STATUS != 4)
                        select _PR).ToList();

            if (_obj != null && _obj.Count > 0)
            {
                retval = 1; //Return 1 if any of PR approval not finished for related PO
            }
            return retval;
        }

        public long GetPONextApprovalId(long poid, long ecode)
        {
            long retval = 0;


            var APPPOType = _PoDBContext.DGIT_POHEADER.Where(m => m.POHEADERID == poid).FirstOrDefault();

            if (APPPOType.HIGH_URGENCY == 1)
            {
                var _objishigh = (from _PO in _PoDBContext.DGIT_POHEADER
                                  join _PAH in _PoDBContext.DGIT_POAPPHISTORY on _PO.POHEADERID equals _PAH.POID
                                  where _PO.PROCESS_STATUS == 1 && _PAH.APPROVAL_STATUS == 0 && _PAH.ADEMPCODE == ecode && _PO.HIGH_URGENCY == 1
                                  select _PO).OrderBy(p => p.POHEADERID).Where(m => m.POHEADERID > poid).ToList();
                if (_objishigh != null && _objishigh.Count > 0)
                {
                    retval = _objishigh.FirstOrDefault().POHEADERID;
                    return retval;
                }
                else
                {
                    var _objishigh1 = (from _PO in _PoDBContext.DGIT_POHEADER
                                       join _PAH in _PoDBContext.DGIT_POAPPHISTORY on _PO.POHEADERID equals _PAH.POID
                                       where _PO.PROCESS_STATUS == 1 && _PAH.APPROVAL_STATUS == 0 && _PAH.ADEMPCODE == ecode && _PO.HIGH_URGENCY != 1
                                       select _PO).OrderBy(p => p.POHEADERID).ToList();
                    if (_objishigh1 != null && _objishigh1.Count > 0)
                    {
                        retval = _objishigh1.FirstOrDefault().POHEADERID;
                        return retval;
                    }
                }
            }

            var _obj = (from _PO in _PoDBContext.DGIT_POHEADER
                        join _PAH in _PoDBContext.DGIT_POAPPHISTORY on _PO.POHEADERID equals _PAH.POID
                        where _PO.PROCESS_STATUS == 1 && _PAH.APPROVAL_STATUS == 0 && _PAH.ADEMPCODE == ecode && _PO.HIGH_URGENCY != 1
                        select _PO).OrderBy(p => p.POHEADERID).Where(m => m.POHEADERID > poid).ToList();

            if (_obj != null && _obj.Count > 0)
            {
                retval = _obj.FirstOrDefault().POHEADERID; //Return 1 if any of PR approval not finished for related PO
            }

            return retval;
        }

        public Search_VW_PODASHBOARD_VWMODEL GetPODASHBOARD(Search_VW_PODASHBOARD_VWMODEL _OBJSEARCH)
        {
            var obj = (from _PODetail in _PoDBContext.VW_DGIT_PODASHBOARD
                       where (string.IsNullOrEmpty(_OBJSEARCH.PONO) ? true : _PODetail.PONO == _OBJSEARCH.PONO)
                       && (string.IsNullOrEmpty(_OBJSEARCH.PRNO) ? true : _PODetail.PRNO == _OBJSEARCH.PRNO)
                       && (string.IsNullOrEmpty(_OBJSEARCH.VENDORNAME) ? true : _PODetail.VENDORNAME.Contains(_OBJSEARCH.VENDORNAME))
                       && (_OBJSEARCH.ADDEDBY == 0 ? true : _PODetail.ADDEDBY == _OBJSEARCH.ADDEDBY)
                       && (string.IsNullOrEmpty(_OBJSEARCH.ADDEDBYNAME) ? true : _PODetail.ADDEDBYNAME.Contains(_OBJSEARCH.ADDEDBYNAME))
                       && (_OBJSEARCH.StartDate.Year == 1 ? true : _PODetail.UPDATEDATE >= _OBJSEARCH.StartDate)
                       && (_OBJSEARCH.EndDate.Year == 1 ? true : _PODetail.UPDATEDATE <= _OBJSEARCH.EndDate)
                    && (_OBJSEARCH.VERSION_NO == 0 ? true : _PODetail.VERSION_NO <= _OBJSEARCH.VERSION_NO)
                       && (string.IsNullOrEmpty(_OBJSEARCH.PLANTID.Trim()) ? true : _PODetail.PLANTID == _OBJSEARCH.PLANTID)
                       select new VW_PODASHBOARD_VIEWMODEL
                       {
                           ADDEDBY = _PODetail.ADDEDBY,
                           POHEADERID = _PODetail.POHEADERID,
                           ADDEDBYNAME = _PODetail.ADDEDBYNAME,
                           PONO = _PODetail.PONO,
                           PRNO = _PODetail.PRNO,
                           UPDATEDATE = _PODetail.UPDATEDATE,
                           VENDORNAME = _PODetail.VENDORNAME,
                           FILENAME = _PODetail.FILENAME,
                           PLANTID = _PODetail.PLANTID,
                           VERSION_NO = _PODetail.VERSION_NO
                       }).ToList();
            _OBJSEARCH.searchResult = obj.ToList();
            return _OBJSEARCH;
        }

        public short UpdateMailCNT(long poid)
        {
            short retVal = 0;
            using (IDbContextTransaction transaction = _PoDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (poid > 0)
                    {
                        //////// Update Process Status ////////
                        DGIT_POHEADER DPH = new DGIT_POHEADER(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                        DPH = _PoDBContext.DGIT_POHEADER.Where(x => x.POHEADERID == poid).SingleOrDefault();
                        if (DPH != null)
                        {
                            DPH.MAILCNT = (short)(DPH.MAILCNT + 1);
                            //DPH.UPDATEDBY = PHVM.UPDATEDBY;
                            //DPH.UPDATEDATE = DateTime.Now;
                            _PoDBContext.Entry(DPH).State = EntityState.Modified;
                            _PoDBContext.SaveChanges();
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

        public SearchPO POALLUserReport(SearchPO VM)
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

            List<VM_VW_DGIT_POREPORT> obj = new List<VM_VW_DGIT_POREPORT>();
            var orglevel = _ICDBContext.ICADORGLEVELs.Where(m => m.SYKIID == VM.KIID);

            var isspecial = (from data in _ICDBContext.DGIT_ICRPTRIGHTS
                             where data.ADEMPCODE == VM.loginid
                             && data.REPORTTYPE == 3
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

                obj = (from data in _ICDBContext.VW_DGIT_POREPORT
                       where (!string.IsNullOrEmpty(VM.Startdate) ? (data.DATEADDED >= ReqDateFrom) : true)
                       && (!string.IsNullOrEmpty(VM.ENDDATE) ? (data.DATEADDED <= ReqDateTo) : true)
                       && (VM.ecode == 0 ? true : data.ADDEDBY == VM.ecode)
                       && (VM.Status == -1 ? true : data.PROCESS_STATUS == VM.Status)
                       && (string.IsNullOrEmpty(VM.ITEM_DETAIL) ? true : data.PO_DESC.ToLower().Contains(VM.ITEM_DETAIL.ToLower()))
                       && data.SYKIID == VM.KIID
                       && data.PROCESS_STATUS != 4
                       && (data.OPERATIONID == VM.OperationID)
                       && (VM.DivisionID != 0 ? data.DIVISIONID == VM.DivisionID : true)
                       && (VM.DEPTID != 0 ? data.DEPARTMENTID == VM.DEPTID : true)
                       && (VM.SECID != 0 ? data.SECTIONID == VM.SECID : true)
                       && (string.IsNullOrEmpty(VM.ADDEDBYNAME) ? true : data.ADDEDBYNAME.ToLower().Contains(VM.ADDEDBYNAME.Trim().ToLower()))
                       select new VM_VW_DGIT_POREPORT
                       {
                           ADDEDBY = data.ADDEDBY,
                           DATEADDED = data.DATEADDED,
                           DEPARTMENT = data.DEPARTMENT,
                           DEPARTMENTID = data.DEPARTMENTID,
                           DIVISION = data.DIVISION,
                           DIVISIONID = data.DIVISIONID,
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
                           ADDEDBYNAME = data.ADDEDBYNAME,
                           POHEADERID = data.POHEADERID,
                           PO_DESC = data.PO_DESC,
                           PONO = data.PONO,
                           VENDORID = data.VENDORID,
                           VENDORMAILID = data.VENDORMAILID,
                           REMARK = data.REMARK,
                           VENDORNAME = data.VENDORNAME,
                           VERSION_NO = data.VERSION_NO,
                           CONTRACT_NO = data.CONTRACT_NO,
                           HIGH_URGENCY = data.HIGH_URGENCY,
                           MAILCNT = data.MAILCNT,
                           PLANTID = data.PLANTID
                       }).ToList();

                List<VM_VW_DGIT_POREPORT> finallist = new List<VM_VW_DGIT_POREPORT>();
                if (VM.SECID > 0 && sectionids.Count() > 0)
                {
                    finallist.AddRange(obj.Where(m => sectionids.Contains(Convert.ToInt64(m.SECTIONID))));
                }
                //else if (VM.DEPTID > 0 && departmentids.Count() > 0)
                //{
                //    if (departmentids.Contains(VM.DEPTID))
                //    {
                //        finallist.AddRange(obj.Where(m => (Convert.ToInt64(m.DEPARTMENTID)) == VM.DEPTID));
                //    }
                //    else
                //    {
                //        var seclst = orglevel.Where(d => sectionids.Contains(d.ADORGLEVELID) && d.PARENTLEVELID == VM.DEPTID).Select(m => m.ADORGLEVELID);
                //        finallist.AddRange(obj.Where(m => seclst.Contains(Convert.ToInt64(m.SECTIONID))));
                //    }

                //}
                else if (VM.DEPTID > 0 && departmentids.Count() > 0)
                {
                    if (departmentids.Contains(VM.DEPTID))
                    {
                        finallist.AddRange(obj.Where(m => (Convert.ToInt64(m.DEPARTMENTID)) == VM.DEPTID));
                    }
                    else
                    {
                        var seclst = orglevel.Where(d => sectionids.Contains(d.ADORGLEVELID)).Select(m => m.ADORGLEVELID);
                        {
                            finallist.AddRange(obj.Where(m => seclst.Contains(Convert.ToInt64(m.SECTIONID)) && m.DEPARTMENTID == VM.DEPTID));
                        }
                    }

                }
                else if (VM.DivisionID > 0 && divisionids.Count() > 0)
                {
                    if (divisionids.Contains(VM.DivisionID))
                    {
                        finallist.AddRange(obj.Where(m => Convert.ToInt64(m.DIVISIONID) == VM.DivisionID));
                    }
                    else
                    {
                        var deptlst = orglevel.Where(d => departmentids.Contains(d.ADORGLEVELID)).Select(m => m.ADORGLEVELID);
                        if (deptlst.Count() > 0)
                        {
                            finallist.AddRange(obj.Where(m => deptlst.Contains(Convert.ToInt64(m.DEPARTMENTID)) && m.DIVISIONID == VM.DivisionID));
                        }

                        var seclst = orglevel.Where(d => sectionids.Contains(d.ADORGLEVELID)).Select(m => m.ADORGLEVELID);
                        if (seclst.Count() > 0)
                        {
                            finallist.AddRange(obj.Where(m => seclst.Contains(Convert.ToInt64(m.SECTIONID)) && m.DIVISIONID == VM.DivisionID));
                        }

                    }

                }

                else if (VM.OperationID > 0)
                {
                    if (divisionids.Count > 0)
                    {
                        finallist.AddRange(obj.Where(m => divisionids.Contains(Convert.ToInt64(m.DIVISIONID))));
                    }
                    if (departmentids.Count > 0)
                    {
                        finallist.AddRange(obj.Where(m => departmentids.Contains(Convert.ToInt64(m.DEPARTMENTID))));
                    }
                    if (sectionids.Count > 0)
                    {
                        finallist.AddRange(obj.Where(m => sectionids.Contains(Convert.ToInt64(m.SECTIONID))));
                    }

                }
                obj = finallist.Distinct().ToList();
            }
            else
            {
                obj = (from data in _ICDBContext.VW_DGIT_POREPORT
                       where (!string.IsNullOrEmpty(VM.Startdate) ? (data.DATEADDED >= ReqDateFrom) : true)
                       && (!string.IsNullOrEmpty(VM.ENDDATE) ? (data.DATEADDED <= ReqDateTo) : true)
                       && (VM.OperationID != 0 ? (data.OPERATIONID == VM.OperationID) : true)
                       && (VM.DivisionID != 0 ? data.DIVISIONID == VM.DivisionID : true)
                       && (VM.DEPTID != 0 ? data.DEPARTMENTID == VM.DEPTID : true)
                       && (VM.SECID != 0 ? data.SECTIONID == VM.SECID : true)
                       && (VM.ecode == 0 ? true : data.ADDEDBY == VM.ecode)
                       && (VM.Status == -1 ? true : data.PROCESS_STATUS == VM.Status)
                       && (string.IsNullOrEmpty(VM.ITEM_DETAIL) ? true : data.PO_DESC.ToLower().Contains(VM.ITEM_DETAIL.ToLower()))
                       && data.SYKIID == VM.KIID
                       && data.PROCESS_STATUS != 4
                       && (data.OPERATIONID == VM.OperationID)
                       && (VM.DivisionID != 0 ? data.DIVISIONID == VM.DivisionID : true)
                       && (VM.DEPTID != 0 ? data.DEPARTMENTID == VM.DEPTID : true)
                       && (VM.SECID != 0 ? data.SECTIONID == VM.SECID : true)
                       && (string.IsNullOrEmpty(VM.ADDEDBYNAME) ? true : data.ADDEDBYNAME.ToLower().Contains(VM.ADDEDBYNAME.Trim().ToLower()))
                       select new VM_VW_DGIT_POREPORT
                       {
                           ADDEDBY = data.ADDEDBY,
                           DATEADDED = data.DATEADDED,
                           DEPARTMENT = data.DEPARTMENT,
                           DEPARTMENTID = data.DEPARTMENTID,
                           DIVISION = data.DIVISION,
                           DIVISIONID = data.DIVISIONID,
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
                           ADDEDBYNAME = data.ADDEDBYNAME,
                           POHEADERID = data.POHEADERID,
                           PO_DESC = data.PO_DESC,
                           PONO = data.PONO,
                           VENDORID = data.VENDORID,
                           VENDORMAILID = data.VENDORMAILID,
                           REMARK = data.REMARK,
                           VENDORNAME = data.VENDORNAME,
                           VERSION_NO = data.VERSION_NO,
                           CONTRACT_NO = data.CONTRACT_NO,
                           HIGH_URGENCY = data.HIGH_URGENCY,
                           MAILCNT = data.MAILCNT,
                           PLANTID = data.PLANTID
                       }).ToList();
            }



            VM.SearchResult = obj.OrderByDescending(m => m.DATEADDED).ThenBy(m => m.UPDATEDATE).ToList();
            return VM;
        }

        public Tuple<long, List<PR_Div_Dep_SecViewModel>> BindOperation(long Loginempcode, long KIID)
        {
            var isspecial = (from data in _ICDBContext.DGIT_ICRPTRIGHTS
                             where data.ADEMPCODE == Loginempcode
                             && data.REPORTTYPE == 3
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

                var iColl = (from data in _PoDBContext.VW_ASSOCIATELVLDETAILS.Where(e => e.ACTIVE == 1
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
                             && data.REPORTTYPE == 3
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
                var iColl = (from data in _PoDBContext.VW_ASSOCIATELVLDETAILS.Where(e => e.ACTIVE == 1
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
                             && data.REPORTTYPE == 3
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
                var iColl = (from data in _PoDBContext.VW_ASSOCIATELVLDETAILS.Where(e => e.ACTIVE == 1
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
                             && data.REPORTTYPE == 3
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
                var iColl = (from data in _PoDBContext.VW_ASSOCIATELVLDETAILS.Where(e => e.ACTIVE == 1
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

        // added by aumento for SR82542
        public List<POAppAuthSeqViewModel> GetPOAuthorityHis_ById(long _POHEADERID, long userid)
        {
            List<POAppAuthSeqViewModel> AuthList = new List<POAppAuthSeqViewModel>();
            //CommonRepository _CommonRepo = new CommonRepository();
            try
            {
                UserApprovalAuthority _Obj = _CommonRepo.CheckApprovalAuthority(userid);
                if (_Obj != null)
                {
                    var Obj = _PoDBContext.DGIT_POAPPAUTHSEQ.Where(k => k.POID == _POHEADERID).Count();

                    if (Obj > 0)
                    {
                        for (int i = 1; i <= Obj; i++)
                        {
                            var Emp_Dtl = _PoDBContext.DGIT_POAPPAUTHSEQ.Where(k => k.POID == _POHEADERID && k.APP_SEQ == i).FirstOrDefault();
                            Employee_Details EmpDt = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Emp_Dtl.ADEMPCODE));

                            AuthList.Add(new POAppAuthSeqViewModel
                            {
                                ADEMPCODE = Emp_Dtl.ADEMPCODE,
                                ADEMPNAME = EmpDt._EFirstName + " " + EmpDt._ELastName,
                                ADDESIGNATION = string.IsNullOrEmpty(EmpDt._FnDesig) ? EmpDt._Desig : EmpDt._FnDesig,
                                //APP_SEQ = Convert.ToInt16(AuthList.Count + 1),
                                APP_SEQ = Emp_Dtl.APP_SEQ,
                                //APPTYPE = 1,
                                APPTYPE = Emp_Dtl.APPTYPE, //Bug fixed of App type on 02-May-2025 > CR6173
                                FNDESID = Convert.ToInt16(EmpDt._FnDesigId == null ? 0 : EmpDt._FnDesigId),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AuthList = new List<POAppAuthSeqViewModel>();
            }
            AuthList = AuthList.OrderBy(n => n.APP_SEQ).OrderBy(m => m.APPTYPE).ToList();
            for (Int16 i = 0; i <= AuthList.Count - 1; i++)
            {
                AuthList[i].APP_SEQ = Convert.ToInt16(i + 1);
            }
            return AuthList;
        }
        // added by aumento for SR82542
    }
}