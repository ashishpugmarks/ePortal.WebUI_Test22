using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace ePortal.Infrastructure.Repositories
{
    public class SMRepository
    {
        private EPortalDBContext _SmDBContext;
        private SYKI _Syki;
        public SMRepository(EPortalDBContext objEPortalDBContext)
        {
            _SmDBContext = objEPortalDBContext;
            _Syki = _SmDBContext.SYKI.Where(x => x.ACTIVE == 1).FirstOrDefault();
        }

        public VendorViewModel GetVendorByCode(string vcode)
        {
            var obj = (from data in _SmDBContext.FINVENDORMASTERMST
                       where data.STATUS == 1
                       && data.VENDORCODE == vcode
                       select new VendorViewModel
                       {
                           VENDORCODE = data.VENDORCODE,
                           VENDORNAME = data.NAME1,
                           VENDOREMAIL = data.EMAIL1
                       }).OrderBy(o => o.VENDORNAME).FirstOrDefault();
            return obj;
        }

        public List<VendorViewModel> VendorAutocompleteSuggestions(string Key)
        {
            List<VendorViewModel> vList = new List<VendorViewModel>();
            vList = (from data in _SmDBContext.FINVENDORMASTERMST
                     where (data.VENDORCODE.ToUpper().Contains(Key.ToUpper()) || data.NAME1.ToUpper().Contains(Key.ToUpper())) && data.STATUS == 1
                     select new VendorViewModel
                     {
                         VENDORCODE = data.VENDORCODE,
                         VENDORNAME = data.NAME1,
                         VENDOREMAIL = data.EMAIL1
                     }).ToList();
            return vList;
        }
       
        //public SmOpMapViewModel GetIOCGAuthority(short smType, long orgLevelId, decimal amount)
        //{
        //    long _amount = Convert.ToInt64(amount);
            
        //    var obj = (from data in _SmDBContext.DGIT_SMOPERATION_MAP
        //               join _Approver1 in _SmDBContext.ADEMPLOYEE on data.APPROVER1 equals _Approver1.ADEMPCODE
        //               join _VW_AP1 in _SmDBContext.VW_ASSOCIATELVLDETAILS.Where(m => m.ACTIVE == 1 && m.SYKI == _Syki.SYKIID) on data.APPROVER1 equals _VW_AP1.ADEMPCODE into _vwapp1
        //               from vw_ap1 in _vwapp1.DefaultIfEmpty()
        //               join _Desg_AP1 in _SmDBContext.ADDESIGNATION on vw_ap1.ADDESIGNATIONID equals _Desg_AP1.ADDESIGNATIONID into _DesgAp1Join
        //               from DESG_AP1 in _DesgAp1Join.DefaultIfEmpty()
        //               where //vw_ap1.SYKI == _Syki.SYKIID
        //               //&& 
        //               data.ACTIVE == 1 && data.SYKIID == _Syki.SYKIID
        //               && (_amount >= data.STARTAMOUNT && _amount <= data.ENDAMOUNT)
        //               && data.ADORGLEVELID == orgLevelId
        //               && data.SESORMRN == smType
        //               select new SmOpMapViewModel
        //               {
        //                   SMMAP_MSTID = data.SMMAP_MSTID,
        //                   ADORGLEVELID = data.ADORGLEVELID,
        //                   APPROVER1 = vw_ap1==null?0: vw_ap1.ADEMPCODE,//data.APPROVER1,
        //                   APPROVER1Name = _Approver1.FIRSTNAME + " " + _Approver1.LASTNAME,
        //                   APPROVER1Desg = DESG_AP1 == null ? "" : DESG_AP1.DESCRIP,
        //                   APPROVER2 = data.APPROVER2,
        //                   //APPROVER2Name = data.APPROVER2 == null ? "" : _SmDBContext.ADEMPLOYEE.Where(r => r.ADEMPCODE == data.APPROVER2).Select(s => s.FIRSTNAME + " " + s.LASTNAME).FirstOrDefault(),
        //                   //APPROVER2Desg = data.APPROVER2 == null ? "" : (from data_AP2 in _SmDBContext.VW_ASSOCIATELVLDETAILS.Where(v => v.ADEMPCODE == data.APPROVER2 && v.SYKI == _Syki.SYKIID)
        //                   //                                               join _Desg_AP2 in _SmDBContext.ADDESIGNATION on _VW_AP1.ADDESIGNATIONID equals _Desg_AP2.ADDESIGNATIONID into _DesgAp2Join
        //                   //                                               from DESG_AP2 in _DesgAp2Join.DefaultIfEmpty()
        //                   //                                               select new { DESG_AP2.DESCRIP}).ToString(),
        //                   ACTIVE = data.ACTIVE,
        //                   DATEADDED = data.DATEADDED,
        //                   ADDEDBY = data.ADDEDBY,
        //                   DATELSTMOD = data.DATELSTMOD,
        //                   MODIFIEDBY = data.MODIFIEDBY,
        //                   STARTAMOUNT = data.STARTAMOUNT,
        //                   ENDAMOUNT = data.ENDAMOUNT,
        //                   SYKIID = data.SYKIID,
        //                   SESORMRN = data.SESORMRN
        //               }).OrderBy(o => o.SMMAP_MSTID).FirstOrDefault();
        //    if (obj != null)
        //    {
        //        if (obj.APPROVER2 != null)
        //        {
        //            var APPROVER2_OBJ = (from data_AP2 in _SmDBContext.ADEMPLOYEE.Where(f => f.ADEMPCODE == obj.APPROVER2)
        //                                 join _VW_AP2 in _SmDBContext.VW_ASSOCIATELVLDETAILS.Where(m => m.ACTIVE == 1 && m.SYKI == _Syki.SYKIID) on data_AP2.ADEMPCODE equals _VW_AP2.ADEMPCODE into _vwapp2
        //                                 from VW_AP2 in _vwapp2.DefaultIfEmpty()
        //                                 join _Desg_AP2 in _SmDBContext.ADDESIGNATION on VW_AP2.ADDESIGNATIONID equals _Desg_AP2.ADDESIGNATIONID into _DesgAp2Join
        //                                 from DESG_AP2 in _DesgAp2Join.DefaultIfEmpty()
        //                                     //where VW_AP2.SYKI == _Syki.SYKIID
        //                                 select new { ADEMPCODE = (VW_AP2==null?0: VW_AP2.ADEMPCODE), Approver2Name = data_AP2.FIRSTNAME + " " + data_AP2.LASTNAME, Approver2Desg = (DESG_AP2 == null ? "" : DESG_AP2.DESCRIP) }).FirstOrDefault();
        //            if (APPROVER2_OBJ != null && APPROVER2_OBJ.ADEMPCODE > 0)
        //            {
        //                obj.APPROVER2Name = APPROVER2_OBJ.Approver2Name;
        //                obj.APPROVER2Desg = APPROVER2_OBJ.Approver2Desg;
        //                obj.APPROVER2 = APPROVER2_OBJ.ADEMPCODE;
        //            }
        //            else
        //            {
        //                obj.APPROVER2 = 0;
        //            }
        //        }
        //    }

        //    return obj;
        //}

        public SmOpMapViewModel GetIOCGAuthority(short smType, long orgLevelId, decimal amount)
        {
            long _amount = Convert.ToInt64(amount);

            var vwDetails = _SmDBContext.VW_ASSOCIATELVLDETAILS
                .Where(m => m.ACTIVE == 1 && m.SYKI == _Syki.SYKIID)
                .ToList();

            var baseQuery = _SmDBContext.DGIT_SMOPERATION_MAP
                .Where(data => data.ACTIVE == 1
                            && data.SYKIID == _Syki.SYKIID
                            && _amount >= data.STARTAMOUNT
                            && _amount <= data.ENDAMOUNT
                            && data.ADORGLEVELID == orgLevelId
                            && data.SESORMRN == smType)
                .AsEnumerable();

            var obj = (from data in baseQuery
                       join approver1 in _SmDBContext.ADEMPLOYEE.AsEnumerable()
                           on data.APPROVER1 equals approver1.ADEMPCODE
                       join vw_ap1 in vwDetails
                           on data.APPROVER1 equals vw_ap1.ADEMPCODE into vwApp1Group
                       from vw_ap1 in vwApp1Group.DefaultIfEmpty()
                       join desg_ap1 in _SmDBContext.ADDESIGNATION.AsEnumerable()
                           on vw_ap1?.ADDESIGNATIONID equals desg_ap1.ADDESIGNATIONID into desgAp1Group
                       from desg_ap1 in desgAp1Group.DefaultIfEmpty()
                       select new SmOpMapViewModel
                       {
                           SMMAP_MSTID = data.SMMAP_MSTID,
                           ADORGLEVELID = data.ADORGLEVELID,
                           APPROVER1 = vw_ap1?.ADEMPCODE ?? 0,
                           APPROVER1Name = approver1.FIRSTNAME + " " + approver1.LASTNAME,
                           APPROVER1Desg = desg_ap1?.DESCRIP ?? "",
                           APPROVER2 = data.APPROVER2,
                           ACTIVE = data.ACTIVE,
                           DATEADDED = data.DATEADDED,
                           ADDEDBY = data.ADDEDBY,
                           DATELSTMOD = data.DATELSTMOD,
                           MODIFIEDBY = data.MODIFIEDBY,
                           STARTAMOUNT = data.STARTAMOUNT,
                           ENDAMOUNT = data.ENDAMOUNT,
                           SYKIID = data.SYKIID,
                           SESORMRN = data.SESORMRN
                       }).OrderBy(o => o.SMMAP_MSTID).FirstOrDefault();


            if (obj != null)
            {
                if (obj.APPROVER2 != null)
                {
                    var APPROVER2_OBJ = (from data_AP2 in _SmDBContext.ADEMPLOYEE
                        .Where(f => f.ADEMPCODE == obj.APPROVER2)
                        .AsEnumerable()
                                        join vw_ap2 in vwDetails
                                            on data_AP2.ADEMPCODE equals vw_ap2.ADEMPCODE into vwApp2Group
                                        from vw_ap2 in vwApp2Group.DefaultIfEmpty()
                                        join desg_ap2 in _SmDBContext.ADDESIGNATION
                                            .AsEnumerable()
                                            on vw_ap2?.ADDESIGNATIONID equals desg_ap2.ADDESIGNATIONID into desgAp2Group
                                        from desg_ap2 in desgAp2Group.DefaultIfEmpty()
                                        select new
                                        {
                                            ADEMPCODE = vw_ap2?.ADEMPCODE ?? 0,
                                            Approver2Name = data_AP2.FIRSTNAME + " " + data_AP2.LASTNAME,
                                            Approver2Desg = desg_ap2?.DESCRIP ?? ""
                                        }).FirstOrDefault();
                    if (APPROVER2_OBJ != null && APPROVER2_OBJ.ADEMPCODE > 0)
                    {
                        obj.APPROVER2Name = APPROVER2_OBJ.Approver2Name;
                        obj.APPROVER2Desg = APPROVER2_OBJ.Approver2Desg;
                        obj.APPROVER2 = APPROVER2_OBJ.ADEMPCODE;
                    }
                    else
                    {
                        obj.APPROVER2 = 0;
                    }
                }
            }

            return obj;
        }
        public Tuple<short, long> SaveSMRequest(SMHeaderViewModel model)
        {
            short retVal = 0; long retHeaderId = 0;
            Tuple<short, long> _retVal_tuple;
            using (var transaction = _SmDBContext.Database.BeginTransaction())
            {
                try
                {
                    DGIT_SESMRN_HEADER DPH = new DGIT_SESMRN_HEADER();
                    int FlagAdd = 0;
                    /* new condition as per invoice logic*/
                    //if (model.IsFinalSubmit == 0)
                    //{
                    var sykiobjtmp = (from data in _SmDBContext.SYKI
                                      select new
                                      {
                                          startyear = data.FINANCIALYEAR.Substring(0, 4),
                                          ENDyear = data.FINANCIALYEAR.Substring(5, 4)
                                      }
                                 ).ToList();

                    var sykiobj = (from data in sykiobjtmp
                                   select new
                                   {
                                       startyear = DateTime.ParseExact("01-APR-" + data.startyear, "dd-MMM-yyyy", null),
                                       ENDyear = DateTime.ParseExact("31-MAR-" + data.ENDyear, "dd-MMM-yyyy", null)
                                   }
                                     ).ToList();

                    var invfinyr = sykiobj.Where(m => model.INVOICEDATE.Value.Date>= m.startyear.Date  && model.INVOICEDATE.Value.Date<=m.ENDyear.Date).FirstOrDefault();
                    //}
                    if (model.SMHEADERID > 0)
                    {
                        if (model.IsFinalSubmit == 0)
                        {
                            if (invfinyr != null)
                            {
                                if (_SmDBContext.DGIT_SESMRN_HEADER.Where(x => x.SMHEADERID != model.SMHEADERID && x.INVOICEDATE.Value >= invfinyr.startyear.Date && x.INVOICEDATE.Value <= invfinyr.ENDyear.Date && x.VENDORCODE == model.VENDORCODE && x.INVOICENO.ToUpper() == model.INVOICENO.ToUpper().Trim()  && (x.PROCESS_STATUS == 1 || x.PROCESS_STATUS == 0 || x.PROCESS_STATUS == 2 || (x.PROCESS_STATUS >= 5 && x.PROCESS_STATUS <= 9))).FirstOrDefault()!=null)
                                {
                                    retVal = 2;
                                    return _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId); //// -- record already exist.
                                }
                            }
                        }
                        //if (_SmDBContext.DGIT_SESMRN_HEADER.Where(x => x.SMHEADERID != model.SMHEADERID && x.SES_MRN_NO == model.SM_NO.Trim() && x.SM_TYPE == model.SM_TYPE && (x.PROCESS_STATUS == 1 || x.PROCESS_STATUS == 0 || x.PROCESS_STATUS == 2 || (x.PROCESS_STATUS >= 5 && x.PROCESS_STATUS <= 9))))
                        //{
                        //    retVal = 2;
                        //    return _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId); //// -- record already exist.
                        //}
                        DPH = _SmDBContext.DGIT_SESMRN_HEADER.Where(x => x.SMHEADERID == model.SMHEADERID).SingleOrDefault();
                    }
                    else
                    {
                        if (invfinyr != null)
                        {
                            //var objisSESExist = _SmDBContext.DGIT_SESMRN_HEADER.Where(x => x.INVOICENO.ToUpper() == model.INVOICENO.ToUpper().Trim() && x.SM_TYPE == model.SM_TYPE && (x.PROCESS_STATUS == 1 || x.PROCESS_STATUS == 0 || x.PROCESS_STATUS == 2 || (x.PROCESS_STATUS >= 5 && x.PROCESS_STATUS <= 9)));
                            //foreach (var tobj in objisSESExist)
                            //{
                            //    if (tobj.INVOICEDATE.Value.Date >= invfinyr.startyear.Date && tobj.INVOICEDATE.Value.Date <= invfinyr.ENDyear.Date)
                            //    {
                            //        retVal = 2;
                            //        return _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId); //// -- record already exist.
                            //    }
                            //}
                            if (_SmDBContext.DGIT_SESMRN_HEADER.Where(x => x.INVOICEDATE.Value >= invfinyr.startyear.Date && x.INVOICEDATE.Value <= invfinyr.ENDyear.Date && x.VENDORCODE == model.VENDORCODE && x.INVOICENO.ToUpper() == model.INVOICENO.ToUpper().Trim()  && (x.PROCESS_STATUS == 1 || x.PROCESS_STATUS == 0 || x.PROCESS_STATUS == 2 || (x.PROCESS_STATUS >= 5 && x.PROCESS_STATUS <= 9))).FirstOrDefault()!=null)
                            {
                                retVal = 2;
                                return _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId); //// -- record already exist.
                            }
                        }
                        //if (_SmDBContext.DGIT_SESMRN_HEADER.Where(x => x.SES_MRN_NO == model.SM_NO.Trim() && x.SM_TYPE == model.SM_TYPE && (x.PROCESS_STATUS == 1 || x.PROCESS_STATUS == 0 || x.PROCESS_STATUS == 2 || (x.PROCESS_STATUS >= 5 && x.PROCESS_STATUS <= 9))))
                        //{
                        //    retVal = 2;
                        //    return _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId); //// -- record already exist.
                        //}


                        DPH = new DGIT_SESMRN_HEADER();
                        if (_SmDBContext.DGIT_SESMRN_HEADER.Count() == 0)
                        {
                            DPH.SMHEADERID = 1;
                        }
                        else
                        {
                            DPH.SMHEADERID = _SmDBContext.DGIT_SESMRN_HEADER.Max(x => x.SMHEADERID) + 1;
                        }
                        FlagAdd = 1;
                    }
                    if (model.IsFinalSubmit == 0)
                    {
                        DPH.SM_TYPE = model.SM_TYPE;
                        //DPH.SES_MRN_NO = "8676687126";
                         DPH.SES_MRN_NO = model.SM_NO;
                        DPH.AMOUNT = model.AMOUNT;
                        DPH.PONUMBER = model.PONUMBER;
                        DPH.VENDORCODE = model.VENDORCODE;
                        DPH.INVOICENO = model.INVOICENO;
                        DPH.INVOICEDATE = model.INVOICEDATE;
                        DPH.INVAMOUNT = model.INVAMOUNT;
                        DPH.INVSIGTYPE = model.INVSIGTYPE;
                        DPH.NATUREOFEXPENSE = model.REMARK;
                        //Below Added by aumento for SESMRN as on 23112023======
                        DPH.SYSITEID = Int32.Parse(model.SYSITE);
                        if (model.IPServiceMatDeclaration == true)
                        {
                            DPH.IS_IPRELATEDPR = 1;
                        }
                        else
                        {
                            DPH.IS_IPRELATEDPR = null;
                        }
                        //======================================================
                    }
                    else
                    {
                        if (_SmDBContext.DGIT_SESMRN_DTL.Where(x => x.SMHEADERID == model.SMHEADERID && x.DOC_TYPE == "SM").FirstOrDefault()==null)
                        {
                            retVal = 3;
                            return _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId); //// -- SM file not exist.
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
                    DPH.PA_GENERATED = 0;
                    _SmDBContext.Entry(DPH).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _SmDBContext.SaveChanges();

                    /// --- Save SM Detail --- ///
                    if (model.smDetail != null)
                    {
                        if (model.smDetail.Count > 0)
                        {
                            SaveSMDetails(model.ADDEDBY, DPH.SMHEADERID, model.smDetail);
                        }
                    }

                    /// --- Save SM Approval Auth Seq ---///
                    if (model.smAuthSeq != null)
                    {
                        if (model.smAuthSeq.Count > 0)
                        {
                            SaveAppAuthSeq(model.ADDEDBY, DPH.SMHEADERID, model.smAuthSeq);

                            /// --- Save SM Approval Authority --- ///
                            SMAppAuthSeqViewModel seqModel = model.smAuthSeq.OrderBy(o => o.APP_SEQ).FirstOrDefault();
                            if (seqModel != null)
                            {
                                SaveSMAppHis(model.ADDEDBY, DPH.SMHEADERID, seqModel);
                            }
                        }
                    }

                    //if (model.skipAuthList != null)
                    //{
                    //    if (model.skipAuthList.Count > 0)
                    //    {
                    //        SaveSkipAuth(model.ADDEDBY, DPH.SMHEADERID, model.skipAuthList);
                    //    }
                    //}

                    transaction.Commit();
                    retVal = 1;
                    retHeaderId = DPH.SMHEADERID;
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

        public short SaveSMDetails(long AddedBy, long SMHEADERID, List<SMDetailViewModel> PDVMList)
        {
            short retVal = 0;
            foreach (SMDetailViewModel PDVM in PDVMList)
            {
                DGIT_SESMRN_DTL DPD = new DGIT_SESMRN_DTL();
                int FlagAdd = 0;
                if (SMHEADERID > 0)
                {
                    //if (PDVM.DOC_TYPE == "SM")
                    //{
                    //    DPD = _SmDBContext.DGIT_SESMRN_DTL.Where(x => x.SMHEADERID == SMHEADERID && x.DOC_TYPE == PDVM.DOC_TYPE).FirstOrDefault();
                    //    if (DPD != null)
                    //    {
                    //        _SmDBContext.DGIT_SESMRN_DTL.Remove(DPD);
                    //        _SmDBContext.SaveChanges();
                    //    }
                    //}

                    if (PDVM.DOC_TYPE == "SM")
                    {
                        int smcnt = _SmDBContext.DGIT_SESMRN_HEADER.Where(x => x.SES_MRN_NO == PDVM.SMNo && (x.PROCESS_STATUS != 3 && x.PROCESS_STATUS != 4)).Count();
                        if (smcnt != 0)
                        {
                            retVal = 4;
                            return retVal;
                        }
                        smcnt = (from data in _SmDBContext.DGIT_SESMRN_DTL
                                 join hdr in _SmDBContext.DGIT_SESMRN_HEADER on data.SMHEADERID equals hdr.SMHEADERID
                                 where (hdr.PROCESS_STATUS != 3
                                 && hdr.PROCESS_STATUS != 4)
                                 && data.SMMO == PDVM.SMNo
                                 && (hdr.INVOICENO != "" || hdr.INVOICENO != null)
                                 select new
                                 { data.SMHEADERID }
                                  ).Count();
                        if (smcnt != 0)
                        {
                            retVal = 4;
                            return retVal;
                        }
                    }

                    DPD = new DGIT_SESMRN_DTL();
                    if (_SmDBContext.DGIT_SESMRN_DTL.Count() == 0)
                    {
                        DPD.SMDTL_ID = 1;
                    }
                    else
                    {
                        DPD.SMDTL_ID = _SmDBContext.DGIT_SESMRN_DTL.Max(x => x.SMDTL_ID) + 1;
                    }
                    FlagAdd = 1;

                    DPD.SMHEADERID = SMHEADERID;
                    DPD.DOC_TYPE = PDVM.DOC_TYPE;
                    DPD.FILENAME = PDVM.FILENAME;
                    DPD.ADDITIONAL_INFO = PDVM.ADDITIONAL_INFO;
                    DPD.SMMO = PDVM.SMNo;
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
                    _SmDBContext.Entry(DPD).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _SmDBContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }

        public void SaveAppAuthSeq(long AddedBy, long SMHEADERID, List<SMAppAuthSeqViewModel> PSVMList)
        {
            List<DGIT_SMAPPAUTH_SEQ> FinAuthList = _SmDBContext.DGIT_SMAPPAUTH_SEQ.Where(p => p.SMHEADERID == SMHEADERID && (p.APPTYPE == 3 || p.APPTYPE == 4) && p.STATUS == 1).OrderBy(o => o.APP_SEQ).ToList();

            //// --- Delete recode ---////
            List<DGIT_SMAPPAUTH_SEQ> SeqList = _SmDBContext.DGIT_SMAPPAUTH_SEQ.Where(t => t.SMHEADERID == SMHEADERID).ToList();
            if (SeqList.Count > 0)
            {
                _SmDBContext.DGIT_SMAPPAUTH_SEQ.RemoveRange(SeqList);
                _SmDBContext.SaveChanges();
            }


            //// -- Update Finance Approval Seq -- ////
            if (FinAuthList.Count > 0)
            {
                SMAppAuthSeqViewModel robj = PSVMList.Where(r => r.APPTYPE == 3 && r.ADEMPCODE == 1).FirstOrDefault();
                if (robj != null)
                {
                    PSVMList.Remove(robj);
                }

                foreach (DGIT_SMAPPAUTH_SEQ finAuthObj in FinAuthList)
                {
                    PSVMList.Add(new SMAppAuthSeqViewModel
                    {
                        ADEMPCODE = finAuthObj.ADEMPCODE,
                        APP_SEQ = Convert.ToInt16(PSVMList.Max(p => p.APP_SEQ) + 1),
                        APPTYPE = finAuthObj.APPTYPE,
                    });
                }
            }

            foreach (SMAppAuthSeqViewModel PSVM in PSVMList.OrderBy(o => o.APP_SEQ).ToList())
            {
                DGIT_SMAPPAUTH_SEQ DAAS = new DGIT_SMAPPAUTH_SEQ();
                if (_SmDBContext.DGIT_SMAPPAUTH_SEQ.Count() == 0)
                {
                    DAAS.SMAPPSEQ_ID = 1;
                }
                else
                {
                    DAAS.SMAPPSEQ_ID = _SmDBContext.DGIT_SMAPPAUTH_SEQ.Max(x => x.SMAPPSEQ_ID) + 1;
                }
                DAAS.SMHEADERID = SMHEADERID;
                DAAS.ADEMPCODE = PSVM.ADEMPCODE;
                DAAS.APP_SEQ = PSVM.APP_SEQ;
                DAAS.APPTYPE = PSVM.APPTYPE;
                DAAS.STATUS = 1;
                DAAS.ADDEDBY = AddedBy;
                DAAS.ADDEDDATE = DateTime.Now;
                _SmDBContext.Entry(DAAS).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                _SmDBContext.SaveChanges();
            }
        }

        public void SaveSMAppHis(long AddedBy, long SMHEADERID, SMAppAuthSeqViewModel PSVM)
        {
            DGIT_SMAPPHISTORY DPAH = new DGIT_SMAPPHISTORY();
            int FlagAdd = 0;
            DPAH = _SmDBContext.DGIT_SMAPPHISTORY.Where(d => d.ADEMPCODE == PSVM.ADEMPCODE && d.SMHEADERID == SMHEADERID && d.APPROVAL_STATUS == 0).FirstOrDefault();
            if (DPAH == null)
            {
                DPAH = new DGIT_SMAPPHISTORY();
                if (_SmDBContext.DGIT_SMAPPHISTORY.Count() == 0)
                {
                    DPAH.SMAPPHISTORY_ID = 1;
                }
                else
                {
                    DPAH.SMAPPHISTORY_ID = _SmDBContext.DGIT_SMAPPHISTORY.Max(x => x.SMAPPHISTORY_ID) + 1;
                }
                FlagAdd = 1;
            }
            DPAH.SMHEADERID = SMHEADERID;
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
            _SmDBContext.Entry(DPAH).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
            _SmDBContext.SaveChanges();
        }

        //public void SaveSkipAuth(long AddedBy, long SMHEADERID, List<PRAPPSKIPViewModel> PASList)
        //{
        //    //// --- Delete recode ---////
        //    List<DGIT_PRAPPSKIP> skipList = _SmDBContext.DGIT_PRAPPSKIP.Where(t => t.SMHEADERID == SMHEADERID).ToList();
        //    if (skipList.Count > 0)
        //    {
        //        _SmDBContext.DGIT_PRAPPSKIP.RemoveRange(skipList);
        //        _SmDBContext.SaveChanges();
        //    }

        //    foreach (PRAPPSKIPViewModel PSVM in PASList)
        //    {
        //        DGIT_PRAPPSKIP DPAS = new DGIT_PRAPPSKIP();
        //        if (_SmDBContext.DGIT_PRAPPSKIP.ToList().Count == 0)
        //        {
        //            DPAS.PRAPPSKIP_ID = 1;
        //        }
        //        else
        //        {
        //            DPAS.PRAPPSKIP_ID = _SmDBContext.DGIT_PRAPPSKIP.Max(x => x.PRAPPSKIP_ID) + 1;
        //        }
        //        DPAS.SMHEADERID = SMHEADERID;
        //        DPAS.ADEMPCODE = PSVM.ADEMPCODE;
        //        DPAS.SKIPREMARK = PSVM.SKIPREMARK;
        //        DPAS.STATUS = 1;
        //        DPAS.ADDEDBY = AddedBy;
        //        DPAS.ADDEDDATE = DateTime.Now;
        //        _SmDBContext.Entry(DPAS).State = Microsoft.EntityFrameworkCore.EntityState.Added;
        //        _SmDBContext.SaveChanges();
        //    }
        //}

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
            var _obj = (from data in _SmDBContext.ADEMPLOYEE.Where(v => v.ADEMPCODE == empCode && v.ACTIVE == 1)
                        join _VW in _SmDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VW.ADEMPCODE
                        join _Desg in _SmDBContext.ADDESIGNATION on _VW.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
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

        //public SMHeaderViewModel GetSMRequestById(long id)
        //{

        //    var _obj = (from data in _SmDBContext.DGIT_SESMRN_HEADER.Where(x => x.SMHEADERID == id)
        //                join _vendor in _SmDBContext.FINVENDORMASTERMST on data.VENDORCODE equals _vendor.VENDORCODE into _vendorJoin
        //                from _vendorData in _vendorJoin.DefaultIfEmpty()
        //                //added by aumento as on 23112023 for SEMRN==================
        //                join _SKI in _SmDBContext.SYSITE on data.SYSITEID equals _SKI.SYSITEID into _sYSITEjoin
        //                from __sYSITEData in _sYSITEjoin.DefaultIfEmpty()
        //                //============================================================
        //                join _AddBy in _SmDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
        //                join _VWAssociate in _SmDBContext.VW_ASSOCIATELVLDETAILS on data.ADDEDBY equals _VWAssociate.ADEMPCODE
        //                where _VWAssociate.SYKI == _Syki.SYKIID
        //                select new SMHeaderViewModel
        //                {
        //                    SMHEADERID = data.SMHEADERID,
        //                    SM_TYPE = data.SM_TYPE,
        //                    SM_NO = data.SES_MRN_NO,
        //                    PROCESS_STATUS = data.PROCESS_STATUS,
        //                    PONUMBER = data.PONUMBER,
        //                    VENDORCODE = data.VENDORCODE,
        //                    VENDORNAME = (_vendorData == null ? "" : _vendorData.NAME1),
        //                    INVOICENO = data.INVOICENO,
        //                    INVOICEDATE = data.INVOICEDATE,
        //                    INVAMOUNT = data.INVAMOUNT,
        //                    INVSIGTYPE = data.INVSIGTYPE,
        //                    AMOUNT = data.AMOUNT,
        //                    REMARK = data.NATUREOFEXPENSE,
        //                    ADDEDBYNAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
        //                    DATEADDED = data.DATEADDED,
        //                    Document_Status = (data.DOC_STATUS == null ? 0 : data.DOC_STATUS),
        //                    Doc_Rev_Date = data.DOC_REV_DATE,
        //                    SYSITE1 = data.SYSITEID,//added by aumento as on 23112023 for SEMRN
        //                    SYSITE = __sYSITEData.DESCRIP,//added by aumento as on 23112023 for SEMRN
        //                    IPServiceMatDeclaration = (data.IS_IPRELATEDPR == 1 ? true : false),
        //                    //Doc_Rev_By = (data.DOC_REV_BY == null ? "" : data.DOC_REV_BY.ToString()),
        //                    smDetail = (from _smDetail in _SmDBContext.DGIT_SESMRN_DTL.Where(d => d.SMHEADERID == data.SMHEADERID)
        //                                where _smDetail.STATUS == 1
        //                                select new SMDetailViewModel
        //                                {
        //                                    SMDTL_ID = _smDetail.SMDTL_ID,
        //                                    SMHEADERID = _smDetail.SMHEADERID,
        //                                    DOC_TYPE = _smDetail.DOC_TYPE,
        //                                    ADDITIONAL_INFO = _smDetail.ADDITIONAL_INFO,
        //                                    FILENAME = _smDetail.FILENAME,
        //                                    SMNo = _smDetail.SMMO
        //                                }).OrderBy(d => d.SMDTL_ID).ToList(),
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
        //                    smAuthSeq = (from _smSeq in _SmDBContext.DGIT_SMAPPAUTH_SEQ.Where(x => x.SMHEADERID == data.SMHEADERID)
        //                                 join _AppSeqEmp in _SmDBContext.ADEMPLOYEE on _smSeq.ADEMPCODE equals _AppSeqEmp.ADEMPCODE
        //                                 join _Vw in _SmDBContext.VW_ASSOCIATELVLDETAILS on _smSeq.ADEMPCODE equals _Vw.ADEMPCODE
        //                                 join _Desg in _SmDBContext.ADDESIGNATION on _Vw.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
        //                                 where _Vw.SYKI == _Syki.SYKIID
        //                                 select new SMAppAuthSeqViewModel
        //                                 {
        //                                     SMAPPSEQID = _smSeq.SMAPPSEQ_ID,
        //                                     SMHEADERID = _smSeq.SMHEADERID,
        //                                     ADEMPCODE = _smSeq.ADEMPCODE,
        //                                     STATUS = _smSeq.STATUS,
        //                                     APP_SEQ = _smSeq.APP_SEQ,
        //                                     ADEMPNAME = _AppSeqEmp.FIRSTNAME + " " + _AppSeqEmp.LASTNAME,
        //                                     ADDESIGNATION = _Desg.DESCRIP,
        //                                     ADDEDDATE = _smSeq.ADDEDDATE,
        //                                     UPDATEBY = _smSeq.UPDATEBY,
        //                                     UPDATEDATE = _smSeq.UPDATEDATE,
        //                                     APPTYPE = _smSeq.APPTYPE,
        //                                     FUNDESG_NAME = _Vw.FUNCTIONALDESIGNATION
        //                                 }).OrderBy(b => b.APP_SEQ).ToList(),
        //                    smAppHis = (from _POAppHis in _SmDBContext.DGIT_SMAPPHISTORY.Where(x => x.SMHEADERID == data.SMHEADERID)
        //                                join _AppEmp in _SmDBContext.ADEMPLOYEE on _POAppHis.ADEMPCODE equals _AppEmp.ADEMPCODE
        //                                join _AppSeq in _SmDBContext.DGIT_SMAPPAUTH_SEQ on _POAppHis.SMHEADERID equals _AppSeq.SMHEADERID
        //                                where _AppSeq.ADEMPCODE == _POAppHis.ADEMPCODE
        //                                select new SMAppHistoryViewModel
        //                                {
        //                                    SMAPPHISTORYID = _POAppHis.SMAPPHISTORY_ID,
        //                                    SMHEADERID = _POAppHis.SMHEADERID,
        //                                    ADEMPCODE = _POAppHis.ADEMPCODE,
        //                                    APPROVAL_STATUS = _POAppHis.APPROVAL_STATUS,
        //                                    APPROVAL_REMARK = _POAppHis.APPROVAL_REMARK,
        //                                    APPEMP_NAME = _AppEmp.FIRSTNAME + " " + _AppEmp.LASTNAME + " - [" + _AppEmp.ADEMPCODE + "]",
        //                                    APP_EMAIL = _AppEmp.EMAILID,
        //                                    ADDEDDATE = _POAppHis.ADDEDDATE,
        //                                    UPDATEBY = _POAppHis.UPDATEBY,
        //                                    UPDATEDATE = _POAppHis.UPDATEDATE,
        //                                    APPROVALDATE = _POAppHis.APP_DATE,
        //                                    APPEMP_CODE = _AppEmp.ADEMPCODE.ToString(),
        //                                    APPTYPE = _AppSeq.APPTYPE,
        //                                }).OrderBy(o => o.SMAPPHISTORYID).ToList(),
        //                }).FirstOrDefault();

        //    if (_obj.smAuthSeq.Count > 0)
        //    {
        //        foreach (SMAppAuthSeqViewModel obj in _obj.smAuthSeq)
        //        {
        //            if (!_obj.smAppHis.Where(x => x.SMHEADERID == obj.SMHEADERID && x.ADEMPCODE == obj.ADEMPCODE))
        //            {
        //                if (obj.APPTYPE == 2) //// 2 for IOCG Approval
        //                {
        //                    if (!_obj.smAppHis.Where(x => x.SMHEADERID == obj.SMHEADERID && x.APPTYPE == 2 && x.APPROVAL_STATUS != 0))
        //                    {
        //                        _obj.smAppHis.Add(new SMAppHistoryViewModel
        //                        {
        //                            SMAPPHISTORYID = 0,
        //                            SMHEADERID = obj.SMHEADERID,
        //                            ADEMPCODE = obj.ADEMPCODE,
        //                            APPROVAL_STATUS = 0,
        //                            APPROVAL_REMARK = "",
        //                            APPEMP_NAME = obj.ADEMPNAME + "[" + obj.ADEMPCODE + "]",
        //                            APPTYPE = obj.APPTYPE,
        //                        });
        //                    }
        //                }
        //                else
        //                {
        //                    _obj.smAppHis.Add(new SMAppHistoryViewModel
        //                    {
        //                        SMAPPHISTORYID = 0,
        //                        SMHEADERID = obj.SMHEADERID,
        //                        ADEMPCODE = obj.ADEMPCODE,
        //                        APPROVAL_STATUS = 0,
        //                        APPROVAL_REMARK = "",
        //                        APPEMP_NAME = obj.ADEMPNAME + "[" + obj.ADEMPCODE + "]",
        //                        APPTYPE = obj.APPTYPE,
        //                    });
        //                }
        //            }
        //        }
        //    }
        //    if (_obj != null)
        //    {
        //        //_obj.STR_INVDATE = _obj.INVOICEDATE.ToString("dd-MMM-yyyy");
        //    }
        //    return _obj;
        //}
        public SMHeaderViewModel GetSMRequestById(long id)
        {
           
            var headerData = _SmDBContext.DGIT_SESMRN_HEADER.FirstOrDefault(x => x.SMHEADERID == id);
            if (headerData == null) return null;

            var vendorData = _SmDBContext.FINVENDORMASTERMST.FirstOrDefault(v => v.VENDORCODE == headerData.VENDORCODE);
            var siteData = _SmDBContext.SYSITE.FirstOrDefault(s => s.SYSITEID == headerData.SYSITEID);
            var addedByEmp = _SmDBContext.ADEMPLOYEE.FirstOrDefault(e => e.ADEMPCODE == headerData.ADDEDBY);
            var associateDetails = _SmDBContext.VW_ASSOCIATELVLDETAILS.FirstOrDefault(a => a.ADEMPCODE == headerData.ADDEDBY && a.SYKI == _Syki.SYKIID);

            var _obj = new SMHeaderViewModel
            {
                SMHEADERID = headerData.SMHEADERID,
                SM_TYPE = headerData.SM_TYPE,
                SM_NO = headerData.SES_MRN_NO,
                PROCESS_STATUS = headerData.PROCESS_STATUS,
                PONUMBER = headerData.PONUMBER,
                VENDORCODE = headerData.VENDORCODE,
                VENDORNAME = vendorData?.NAME1 ?? "",
                INVOICENO = headerData.INVOICENO,
                INVOICEDATE = headerData.INVOICEDATE,
                INVAMOUNT = headerData.INVAMOUNT,
                INVSIGTYPE = headerData.INVSIGTYPE,
                AMOUNT = headerData.AMOUNT,
                REMARK = headerData.NATUREOFEXPENSE,
                ADDEDBYNAME = $"{addedByEmp?.FIRSTNAME} {addedByEmp?.LASTNAME}",
                DATEADDED = headerData.DATEADDED,
                Document_Status = headerData.DOC_STATUS ?? 0,
                Doc_Rev_Date = headerData.DOC_REV_DATE,
                SYSITE1 = headerData.SYSITEID,
                SYSITE = siteData?.DESCRIP ?? "",
                IPServiceMatDeclaration = headerData.IS_IPRELATEDPR == 1
            };

            // Emp_Detail
            _obj.Emp_Detail = new Employee_Details
            {
                _ECode = addedByEmp.ADEMPCODE,
                _EName = $"{addedByEmp?.FIRSTNAME} {addedByEmp?.LASTNAME}",
                _EmailId = addedByEmp?.EMAILID,
                _DOB = DateTime.Now,
                _SecDescrip = associateDetails?.SECTION,
                _DepDesc = associateDetails?.DEPARTMENT,
                _DivDesc = associateDetails?.DIVISION,
                _OpDesc = associateDetails?.OPERATION,
                _SiteId = associateDetails?.SYSITEID
            };

            // smDetail
            _obj.smDetail = _SmDBContext.DGIT_SESMRN_DTL
                .Where(d => d.SMHEADERID == id && d.STATUS == 1)
                .Select(d => new SMDetailViewModel
                {
                    SMDTL_ID = d.SMDTL_ID,
                    SMHEADERID = d.SMHEADERID,
                    DOC_TYPE = d.DOC_TYPE,
                    ADDITIONAL_INFO = d.ADDITIONAL_INFO,
                    FILENAME = d.FILENAME,
                    SMNo = d.SMMO
                }).OrderBy(d => d.SMDTL_ID).ToList();

            // smAuthSeq
            _obj.smAuthSeq = (from seq in _SmDBContext.DGIT_SMAPPAUTH_SEQ
                                   join emp in _SmDBContext.ADEMPLOYEE on seq.ADEMPCODE equals emp.ADEMPCODE
                                   join vw in _SmDBContext.VW_ASSOCIATELVLDETAILS on seq.ADEMPCODE equals vw.ADEMPCODE
                                   join desg in _SmDBContext.ADDESIGNATION on vw.ADDESIGNATIONID equals desg.ADDESIGNATIONID
                                   where seq.SMHEADERID == id && vw.SYKI == _Syki.SYKIID
                                   select new SMAppAuthSeqViewModel
                                   {
                                       SMAPPSEQID = seq.SMAPPSEQ_ID,
                                       SMHEADERID = seq.SMHEADERID,
                                       ADEMPCODE = seq.ADEMPCODE,
                                       STATUS = seq.STATUS,
                                       APP_SEQ = seq.APP_SEQ,
                                       ADEMPNAME = $"{emp.FIRSTNAME} {emp.LASTNAME}",
                                       ADDESIGNATION = desg.DESCRIP,
                                       ADDEDDATE = seq.ADDEDDATE,
                                       UPDATEBY = seq.UPDATEBY,
                                       UPDATEDATE = seq.UPDATEDATE,
                                       APPTYPE = seq.APPTYPE,
                                       FUNDESG_NAME = vw.FUNCTIONALDESIGNATION
                                   }).OrderBy(x => x.APP_SEQ).ToList();

            // smAppHis
            _obj.smAppHis = (from his in _SmDBContext.DGIT_SMAPPHISTORY
                                  join emp in _SmDBContext.ADEMPLOYEE on his.ADEMPCODE equals emp.ADEMPCODE
                                  join seq in _SmDBContext.DGIT_SMAPPAUTH_SEQ on his.SMHEADERID equals seq.SMHEADERID
                                  where his.SMHEADERID == id && seq.ADEMPCODE == his.ADEMPCODE
                                  select new SMAppHistoryViewModel
                                  {
                                      SMAPPHISTORYID = his.SMAPPHISTORY_ID,
                                      SMHEADERID = his.SMHEADERID,
                                      ADEMPCODE = his.ADEMPCODE,
                                      APPROVAL_STATUS = his.APPROVAL_STATUS,
                                      APPROVAL_REMARK = his.APPROVAL_REMARK,
                                      APPEMP_NAME = $"{emp.FIRSTNAME} {emp.LASTNAME} - [{emp.ADEMPCODE}]",
                                      APP_EMAIL = emp.EMAILID,
                                      ADDEDDATE = his.ADDEDDATE,
                                      UPDATEBY = his.UPDATEBY,
                                      UPDATEDATE = his.UPDATEDATE,
                                      APPROVALDATE = his.APP_DATE,
                                      APPEMP_CODE = emp.ADEMPCODE.ToString(),
                                      APPTYPE = seq.APPTYPE
                                  }).OrderBy(x => x.SMAPPHISTORYID).ToList();

            // Add missing history entries if needed
            foreach (var obj in _obj.smAuthSeq)
            {
                bool hasHistory = _obj.smAppHis.Any(x => x.SMHEADERID == obj.SMHEADERID && x.ADEMPCODE == obj.ADEMPCODE);

                if (!hasHistory)
                {
                    if (obj.APPTYPE == 2) // IOCG Approval
                    {
                        bool hasIOCGApproval = _obj.smAppHis.Any(x =>
                            x.SMHEADERID == obj.SMHEADERID &&
                            x.APPTYPE == 2 &&
                            x.APPROVAL_STATUS != 0);

                        if (!hasIOCGApproval)
                        {
                            _obj.smAppHis.Add(new SMAppHistoryViewModel
                            {
                                SMAPPHISTORYID = 0,
                                SMHEADERID = obj.SMHEADERID,
                                ADEMPCODE = obj.ADEMPCODE,
                                APPROVAL_STATUS = 0,
                                APPROVAL_REMARK = "",
                                APPEMP_NAME = $"{obj.ADEMPNAME}[{obj.ADEMPCODE}]",
                                APPTYPE = obj.APPTYPE
                            });
                        }
                    }
                    else // For all other APPTYPEs
                    {
                        _obj.smAppHis.Add(new SMAppHistoryViewModel
                        {
                            SMAPPHISTORYID = 0,
                            SMHEADERID = obj.SMHEADERID,
                            ADEMPCODE = obj.ADEMPCODE,
                            APPROVAL_STATUS = 0,
                            APPROVAL_REMARK = "",
                            APPEMP_NAME = $"{obj.ADEMPNAME}[{obj.ADEMPCODE}]",
                            APPTYPE = obj.APPTYPE
                        });
                    }
                }
            }




            if (_obj != null)
            {
                //_obj.STR_INVDATE = _obj.INVOICEDATE.ToString("dd-MMM-yyyy");
            }
            return _obj;
        }

        public short SMApproval(SMAppHistoryViewModel PHVM, List<SMDetailViewModel> SmDetailList)
        {
            short retVal = 0;
            using (var transaction = _SmDBContext.Database.BeginTransaction())
            {
                try
                {
                    DGIT_SMAPPHISTORY DPAH = new DGIT_SMAPPHISTORY();
                    if (PHVM.SMHEADERID > 0)
                    {
                        /////////// Update Approval Status //////////
                        DPAH = _SmDBContext.DGIT_SMAPPHISTORY.Where(x => x.SMHEADERID == PHVM.SMHEADERID && x.ADEMPCODE == PHVM.ADEMPCODE && x.APPROVAL_STATUS == 0).FirstOrDefault();
                        if (DPAH != null)
                        {
                            DPAH.APPROVAL_STATUS = PHVM.APPROVAL_STATUS;
                            DPAH.APPROVAL_REMARK = PHVM.APPROVAL_REMARK;
                            DPAH.UPDATEBY = PHVM.UPDATEBY;
                            DPAH.APP_DATE = DateTime.Now;
                            DPAH.UPDATEDATE = DateTime.Now;
                            _SmDBContext.Entry(DPAH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _SmDBContext.SaveChanges();
                        }
                        else
                        {
                            retVal = -1;
                            transaction.Rollback();
                            return retVal;
                        }

                        /// --- Save SM Approval Authority --- ///
                        SMAppAuthSeqViewModel seqModel = new SMAppAuthSeqViewModel();
                        if (PHVM.APPROVAL_STATUS == 1)
                        {
                            //// --- Add IOCG Attachment --- ////
                            if (SmDetailList.Count > 0)
                            {
                                SaveSMDetails(PHVM.ADDEDBY, PHVM.SMHEADERID, SmDetailList);
                            }
                            //// --- End --- ////

                            short nextSeq = Convert.ToInt16(_SmDBContext.DGIT_SMAPPAUTH_SEQ.Where(s => s.SMHEADERID == PHVM.SMHEADERID && s.ADEMPCODE == PHVM.ADEMPCODE).Select(s => s.APP_SEQ).FirstOrDefault() + 1);
                            seqModel = (from data in _SmDBContext.DGIT_SMAPPAUTH_SEQ.Where(a => a.APP_SEQ >= nextSeq && a.SMHEADERID == PHVM.SMHEADERID)
                                        select new SMAppAuthSeqViewModel
                                        {
                                            SMAPPSEQID = data.SMAPPSEQ_ID,
                                            SMHEADERID = data.SMHEADERID,
                                            ADEMPCODE = data.ADEMPCODE,
                                            APP_SEQ = data.APP_SEQ,
                                            STATUS = data.STATUS,
                                            APPTYPE = data.APPTYPE
                                        }).FirstOrDefault();
                            if (seqModel != null)
                            {
                                var isapphiscreated = _SmDBContext.DGIT_SMAPPHISTORY.Where(a => a.ADEMPCODE == seqModel.ADEMPCODE && a.SMHEADERID == PHVM.SMHEADERID && a.APPROVAL_STATUS == 0).FirstOrDefault();
                                if (seqModel.APPTYPE == 1) //// --- 1 - USER AUTHORITY, 2 - IOCG AUTHORITY, 3 - Finance Approval --- ////
                                {
                                    SaveSMAppHis(PHVM.ADDEDBY, PHVM.SMHEADERID, seqModel);
                                }
                                else if (seqModel.APPTYPE == 2)
                                {
                                    if (isapphiscreated == null)
                                    {
                                        var objseq = (from data in _SmDBContext.DGIT_SMAPPAUTH_SEQ.Where(a => a.APPTYPE == 2 && a.SMHEADERID == PHVM.SMHEADERID)
                                                      select new SMAppAuthSeqViewModel
                                                      {
                                                          SMAPPSEQID = data.SMAPPSEQ_ID,
                                                          SMHEADERID = data.SMHEADERID,
                                                          ADEMPCODE = data.ADEMPCODE,
                                                          APP_SEQ = data.APP_SEQ,
                                                          STATUS = data.STATUS,
                                                          APPTYPE = data.APPTYPE
                                                      }).ToList();
                                        foreach (var o in objseq)
                                        {
                                            SaveSMAppHis(PHVM.ADDEDBY, PHVM.SMHEADERID, o);
                                        }
                                    }
                                }
                                else if (seqModel.APPTYPE == 3)
                                {
                                    if (isapphiscreated == null)
                                    {
                                        var finSeqList = (from data in _SmDBContext.DGIT_SMAPPAUTH_SEQ.Where(a => a.APPTYPE == 3 && a.SMHEADERID == PHVM.SMHEADERID && a.ADEMPCODE == 1)
                                                          select new SMAppAuthSeqViewModel
                                                          {
                                                              SMAPPSEQID = data.SMAPPSEQ_ID,
                                                              SMHEADERID = data.SMHEADERID,
                                                              ADEMPCODE = data.ADEMPCODE,
                                                              APP_SEQ = data.APP_SEQ,
                                                              STATUS = data.STATUS,
                                                              APPTYPE = data.APPTYPE
                                                          }).ToList();
                                        foreach (var objFinSeq in finSeqList)
                                        {
                                            SaveSMAppHis(PHVM.ADDEDBY, PHVM.SMHEADERID, objFinSeq);
                                        }
                                    }
                                }
                            }
                        }

                        //// --- Check Condition in case of IOCG Approval --- ////
                        if (_SmDBContext.DGIT_SMAPPAUTH_SEQ.Where(a => a.ADEMPCODE == DPAH.ADEMPCODE && a.SMHEADERID == DPAH.SMHEADERID && a.APPTYPE == 2).FirstOrDefault()!=null)
                        {
                            List<SMAppAuthSeqViewModel> seqObjList = (from data in _SmDBContext.DGIT_SMAPPAUTH_SEQ.Where(g => g.APPTYPE == 2 && g.SMHEADERID == DPAH.SMHEADERID && g.ADEMPCODE != DPAH.ADEMPCODE)
                                                                      select new SMAppAuthSeqViewModel
                                                                      {
                                                                          SMHEADERID = data.SMHEADERID,
                                                                          ADEMPCODE = data.ADEMPCODE,
                                                                      }).ToList();
                            if (seqObjList.Count > 0)
                            {
                                foreach (SMAppAuthSeqViewModel seqObj in seqObjList)
                                {
                                    DGIT_SMAPPHISTORY REMOVE_OBJ = (from RDATA in _SmDBContext.DGIT_SMAPPHISTORY.Where(d => d.ADEMPCODE == seqObj.ADEMPCODE && d.SMHEADERID == seqObj.SMHEADERID && d.APPROVAL_STATUS == 0)
                                                                    join _AppSeq in _SmDBContext.DGIT_SMAPPAUTH_SEQ on RDATA.SMHEADERID equals _AppSeq.SMHEADERID
                                                                    where _AppSeq.APPTYPE == 2
                                                                    select RDATA).FirstOrDefault();
                                    if (REMOVE_OBJ != null)
                                    {
                                        _SmDBContext.DGIT_SMAPPHISTORY.Remove(REMOVE_OBJ);
                                        _SmDBContext.SaveChanges();
                                    }
                                }
                            }
                            seqModel = null;
                        }
                        //// --- End --- ////

                        //////// Update Process Status ////////
                        DGIT_SESMRN_HEADER DPH = new DGIT_SESMRN_HEADER(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                        DPH = _SmDBContext.DGIT_SESMRN_HEADER.Where(x => x.SMHEADERID == PHVM.SMHEADERID).SingleOrDefault();
                        if (DPH != null)
                        {
                            if (PHVM.APPROVAL_STATUS == 1 && seqModel == null)
                            {
                                DPH.PROCESS_STATUS = 2;
                            }
                            else if (PHVM.APPROVAL_STATUS == 1 && (seqModel != null && seqModel.APPTYPE==3) && DPH.PROCESS_STATUS==1)
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
                            DPH.UPDATEDBY = PHVM.UPDATEBY;
                            DPH.UPDATEDATE = DateTime.Now;
                            _SmDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _SmDBContext.SaveChanges();
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

        public short SMCancel(SMHeaderViewModel PHVM)
        {
            short retVal = 0;
            using (var transaction = _SmDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (PHVM.SMHEADERID > 0)
                    {
                        //////// Update Process Status ////////
                        DGIT_SESMRN_HEADER DPH = new DGIT_SESMRN_HEADER(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                        DPH = _SmDBContext.DGIT_SESMRN_HEADER.Where(x => x.SMHEADERID == PHVM.SMHEADERID).SingleOrDefault();
                        if (DPH != null)
                        {
                            DPH.PROCESS_STATUS = 4;
                            DPH.UPDATEDBY = PHVM.UPDATEDBY;
                            DPH.UPDATEDATE = DateTime.Now;
                            _SmDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _SmDBContext.SaveChanges();
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

        public List<SMDetailViewModel> GetAttachmentDetail(long _SMHEADERID)
        {
            return (from _SMDetail in _SmDBContext.DGIT_SESMRN_DTL.Where(d => d.SMHEADERID == _SMHEADERID)
                    where _SMDetail.STATUS == 1
                    select new SMDetailViewModel
                    {
                        SMDTL_ID = _SMDetail.SMDTL_ID,
                        SMHEADERID = _SMDetail.SMHEADERID,
                        DOC_TYPE = _SMDetail.DOC_TYPE,
                        ADDITIONAL_INFO = _SMDetail.ADDITIONAL_INFO,
                        FILENAME = _SMDetail.FILENAME,
                    }).ToList();
        }

        public short DeleteAttachment(string fileName, string docType, long SMHEADERID)
        {
            short retVal = 0;
            if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(docType) && SMHEADERID > 0)
            {
                DGIT_SESMRN_DTL DT = _SmDBContext.DGIT_SESMRN_DTL.Where(x => x.FILENAME == fileName && x.DOC_TYPE == docType && x.SMHEADERID == SMHEADERID).FirstOrDefault();
                if (DT != null)
                {
                    _SmDBContext.DGIT_SESMRN_DTL.Remove(DT);
                    _SmDBContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }

        public POHeaderViewModel GetPODetailByPOId(string poNo)
        {
            var _obj = (from data in _SmDBContext.DGIT_POHEADER
                        where data.PONO == poNo
                        select new POHeaderViewModel
                        {
                            IsFinalSubmit = 2,
                            poDetail = (from _POMapp in _SmDBContext.DGIT_PODETAIL.Where(l => l.POHEADERID == data.POHEADERID)
                                        where _POMapp.STATUS == 1 && _POMapp.DOC_TYPE != "PO"
                                        select new PODetailViewModel
                                        {
                                            ADDITIONAL_INFO = _POMapp.ADDITIONAL_INFO,
                                            DOC_TYPE = _POMapp.DOC_TYPE,
                                            FILENAME = _POMapp.FILENAME,
                                        }).ToList(),
                        }).FirstOrDefault();
            return _obj;
        }

        public SearchSMViewModel SMDashboard(SearchSMViewModel VM, long plantId)
        {
            DateTime applicabledate = DateTime.ParseExact("20-NOV-2021", "dd-MMM-yyyy", null);
            DateTime ReqDateFrom = DateTime.Now.Date;
            DateTime InvoiceDate = DateTime.Now.Date;
            DateTime ReqDateTo = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(VM.Startdate))
            {
                ReqDateFrom = DateTime.ParseExact(VM.Startdate, "dd-MMM-yyyy", null);
            }
            if (!string.IsNullOrEmpty(VM.INVOICEDATE))
            {
                InvoiceDate = DateTime.ParseExact(VM.INVOICEDATE, "dd-MMM-yyyy", null);
            }
            if (!string.IsNullOrEmpty(VM.ENDDATE))
            {
                ReqDateTo = DateTime.ParseExact(VM.ENDDATE + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
            }

            List<SMHeaderViewModel> obj = new List<SMHeaderViewModel>();

           
            obj = (from data in _SmDBContext.VW_DGIT_SMHEADER.Where(y => y.DATEADDED >= applicabledate)
                   join emp in _SmDBContext.VW_ASSOCIATELVLDETAILS on new { addedby = data.ADDEDBY, SYKIID = data.SYKIID } equals new { addedby = emp.ADEMPCODE, SYKIID = emp.SYKI }
                   join _vendor in _SmDBContext.FINVENDORMASTERMST on data.VENDORCODE equals _vendor.VENDORCODE into _vendorJoin
                   from _vendorData in _vendorJoin.DefaultIfEmpty()
                   join _ed in _SmDBContext.ADEMPLOYEE on data.ADDEDBY equals _ed.ADEMPCODE
                   //changed by aumento as on 25112023 for SESMRN======================================================================
                   join _site in _SmDBContext.DGIT_SMPLANTSITEMAP on (data.SYSITEID ?? emp.SYSITEID) equals _site.SITEID
                   // join _site in _SmDBContext.DGIT_SMPLANTSITEMAP on emp.SYSITEID equals _site.SITEID
                   //==================================================================================================================
                   join _docRevBy in _SmDBContext.ADEMPLOYEE on (long)data.DOC_REV_BY equals _docRevBy.ADEMPCODE into _docRevByJoin
                   from _docEmp in _docRevByJoin.DefaultIfEmpty()
                   where (string.IsNullOrEmpty(VM.Startdate) || (data.DATEADDED >= ReqDateFrom) )
                   && (string.IsNullOrEmpty(VM.ENDDATE) || (data.DATEADDED <= ReqDateTo))
                   && emp.SYKI == VM.KIID
                   && (VM.OperationID == 0 || emp.OPERATIONID == VM.OperationID)
                   && (VM.DivisionID == 0 || emp.DIVISIONID == VM.DivisionID)
                   && (VM.DEPTID == 0 || emp.DEPARTMENTID == VM.DEPTID)
                   && (VM.SECID == 0 || emp.SECTIONID == VM.SECID)
                   //&& (string.IsNullOrEmpty(VM.SMNo) || data.SES_MRN_NO == VM.SMNo)
                   && (VM.ecode == 0 || data.ADDEDBY == VM.ecode)
                   && (VM.RequestNumber == 0 || data.SMHEADERID == VM.RequestNumber)
                   && (string.IsNullOrEmpty(VM.INVOICEDATE) || data.INVOICEDATE == InvoiceDate)
                   && (string.IsNullOrEmpty(VM.INVOICENO) || data.INVOICENO == VM.INVOICENO)
                  && (string.IsNullOrEmpty(VM.VendorCode) || data.VENDORCODE == VM.VendorCode)
                   && (string.IsNullOrEmpty(VM.VendorName) || (_vendorData.NAME1.ToUpper() == VM.VendorName.ToUpper()))
                   && (VM.Doc_Status == -1 || data.DOC_STATUS == VM.Doc_Status)
                   && (VM.ReqStatus == -1 || (VM.ReqStatus == 0 ? (data.PROCESS_STATUS == 2 || data.PROCESS_STATUS == 5 || data.PROCESS_STATUS == 8) : data.PROCESS_STATUS == VM.ReqStatus))
                  && _site.PLANTID == plantId
                   select new SMHeaderViewModel
                   {
                       SMHEADERID = data.SMHEADERID,
                       PROCESS_STATUS = data.PROCESS_STATUS,
                       ADDEDBYNAME = _ed.FIRSTNAME + " " + _ed.LASTNAME,
                       DATEADDED = data.DATEADDED,
                       SM_NO = data.SES_MRN_NO,
                       SM_TYPE = data.SM_TYPE,
                       PONUMBER = data.PONUMBER,
                       AMOUNT = data.AMOUNT,
                       STATUS = data.STATUS,
                       INVOICENO = data.INVOICENO,
                       INVAMOUNT = data.INVAMOUNT,
                       INVOICEDATE = data.INVOICEDATE,
                       PA_GENERATED = data.PA_GENERATED,
                       VENDORCODE = (_vendorData == null ? "" : _vendorData.VENDORCODE),
                       VENDORNAME = (_vendorData == null ? "" : _vendorData.NAME1),
                       Document_Status = (data.DOC_STATUS == null ? 0 : data.DOC_STATUS),
                       Doc_Rev_Date = data.DOC_REV_DATE,
                       Doc_Rev_By = data.DOC_REV_BY, //== null ? "" : data.DOC_REV_BY.ToString(), //(_docEmp == null ? "" : (_docEmp.FIRSTNAME + " " + _docEmp.LASTNAME + " - " + _docEmp.ADEMPCODE)),
                       Emp_Detail = new Employee_Details
                       {
                           _ECode = _ed.ADEMPCODE,
                           _EName = _ed.FIRSTNAME + " " + _ed.LASTNAME,
                           _OpDesc = emp.OPERATION,
                           _DivDesc = emp.DIVISION,
                           _DepDesc = emp.DEPARTMENT,
                           _SecDescrip = emp.SECTION

                       }
                   }).ToList();

            foreach (var header in obj)
            {
                var details = (from _smDetail in _SmDBContext.DGIT_SESMRN_DTL
                               where _smDetail.SMHEADERID == header.SMHEADERID
                                     && (_smDetail.DOC_TYPE == "SMA" || _smDetail.DOC_TYPE == "SMFA" || _smDetail.DOC_TYPE == "SM")
                                     && _smDetail.STATUS == 1
                               orderby _smDetail.SMDTL_ID
                               select new SMDetailViewModel
                               {
                                   SMDTL_ID = _smDetail.SMDTL_ID,
                                   SMHEADERID = _smDetail.SMHEADERID,
                                   DOC_TYPE = _smDetail.DOC_TYPE,
                                   ADDITIONAL_INFO = _smDetail.ADDITIONAL_INFO,
                                   FILENAME = _smDetail.FILENAME,
                                   SMNo = _smDetail.SMMO
                               }).ToList();

                header.smDetail = details;
            }


            if (!string.IsNullOrEmpty(VM.SMNo))
            {
                VM.SearchResult = (from data in obj
                                   from sm in data.smDetail
                                   where (data.INVOICENO != "0" && sm.SMNo == VM.SMNo)
                                      || (data.INVOICENO == "0" && data.SM_NO == VM.SMNo)
                                   select data).Distinct().ToList();
            }
            else
            {
                VM.SearchResult = obj.OrderBy(o => o.DATEADDED).ToList(); 
            }
            return VM;
        }

        public short UpdateDocStatus(long smheaderId, long updatedBy)
        {
            short retVal = 0;
            using (var transaction = _SmDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (smheaderId > 0)
                    {
                        //////// Update Doc Status ////////
                        DGIT_SESMRN_HEADER DPH = new DGIT_SESMRN_HEADER();
                        DPH = _SmDBContext.DGIT_SESMRN_HEADER.Where(x => x.SMHEADERID == smheaderId).SingleOrDefault();
                        if (DPH != null)
                        {
                            DPH.DOC_STATUS = 1;
                            DPH.DOC_REV_BY = updatedBy;
                            DPH.DOC_REV_DATE = DateTime.Now;
                            DPH.PROCESS_STATUS = 5; //// 5-Pending at Finance
                            DPH.UPDATEDBY = updatedBy;
                            DPH.UPDATEDATE = DateTime.Now;
                            _SmDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _SmDBContext.SaveChanges();

                            retVal = 1;
                        }
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

        public short SMFinApproval(SMAppHistoryViewModel PHVM, List<SMDetailViewModel> SmDetailList)
        {
            short retVal = 0;
            using (var transaction = _SmDBContext.Database.BeginTransaction())
            {
                try
                {
                    int FlagAdd = 0;
                    DGIT_SMAPPHISTORY DPAH = new DGIT_SMAPPHISTORY();
                    if (PHVM.SMHEADERID > 0)
                    {
                        #region Update App Seq Table
                        int seqFlagAdd = 0;
                        DGIT_SMAPPAUTH_SEQ objAppSeq = _SmDBContext.DGIT_SMAPPAUTH_SEQ.Where(x => x.SMHEADERID == PHVM.SMHEADERID && x.ADEMPCODE == 1 && x.APPTYPE == 3 && x.STATUS == 1).FirstOrDefault();
                        if (objAppSeq == null)
                        {
                            objAppSeq = new DGIT_SMAPPAUTH_SEQ();
                            objAppSeq = _SmDBContext.DGIT_SMAPPAUTH_SEQ.Where(x => x.SMHEADERID == PHVM.SMHEADERID && x.ADEMPCODE == PHVM.ADEMPCODE && x.APPTYPE == 3 && x.STATUS == 1).FirstOrDefault();
                            if (objAppSeq == null)
                            {
                                objAppSeq = new DGIT_SMAPPAUTH_SEQ();
                                if (_SmDBContext.DGIT_SMAPPAUTH_SEQ.Count() == 0)
                                {
                                    objAppSeq.SMAPPSEQ_ID = 1;
                                }
                                else
                                {
                                    objAppSeq.SMAPPSEQ_ID = _SmDBContext.DGIT_SMAPPAUTH_SEQ.Max(x => x.SMAPPSEQ_ID) + 1;
                                }
                                seqFlagAdd = 1;
                                objAppSeq.SMHEADERID = PHVM.SMHEADERID;
                                short appSeq = (_SmDBContext.DGIT_SMAPPAUTH_SEQ.Where(h => h.SMHEADERID == PHVM.SMHEADERID && h.STATUS == 1).ToList().Count == 0 ? (short)1 : Convert.ToInt16(_SmDBContext.DGIT_SMAPPAUTH_SEQ.Where(n => n.SMHEADERID == PHVM.SMHEADERID && n.STATUS == 1).Max(x => x.APP_SEQ) + 1));
                                objAppSeq.APP_SEQ = appSeq;
                                objAppSeq.APPTYPE = 3; //// 3-Finance Approval
                                objAppSeq.STATUS = 1;
                                objAppSeq.ADDEDBY = PHVM.ADDEDBY;
                                objAppSeq.ADDEDDATE = DateTime.Now;
                            }
                        }

                        objAppSeq.ADEMPCODE = PHVM.ADEMPCODE;
                        objAppSeq.UPDATEBY = PHVM.UPDATEBY;
                        objAppSeq.UPDATEDATE = DateTime.Now;
                        _SmDBContext.Entry(objAppSeq).State = seqFlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                        _SmDBContext.SaveChanges();
                        #endregion

                        #region Update Approval History
                        DPAH = _SmDBContext.DGIT_SMAPPHISTORY.Where(x => x.SMHEADERID == PHVM.SMHEADERID && x.ADEMPCODE == 1 && x.APPROVAL_STATUS == 0).FirstOrDefault();
                        if (DPAH == null)
                        {
                            DPAH = new DGIT_SMAPPHISTORY();
                            if (_SmDBContext.DGIT_SMAPPHISTORY.Count() == 0)
                            {
                                DPAH.SMAPPHISTORY_ID = 1;
                            }
                            else
                            {
                                DPAH.SMAPPHISTORY_ID = _SmDBContext.DGIT_SMAPPHISTORY.Max(x => x.SMAPPHISTORY_ID) + 1;
                            }
                            FlagAdd = 1;
                            DPAH.SMHEADERID = PHVM.SMHEADERID;
                            DPAH.ADDEDBY = PHVM.ADDEDBY;
                            DPAH.ADDEDDATE = DateTime.Now;
                        }

                        DPAH.ADEMPCODE = PHVM.ADEMPCODE;
                        DPAH.APPROVAL_STATUS = PHVM.APPROVAL_STATUS;
                        DPAH.APPROVAL_REMARK = PHVM.APPROVAL_REMARK;
                        DPAH.APP_DATE = DateTime.Now;
                        if (FlagAdd == 0)
                        {
                            DPAH.UPDATEBY = PHVM.UPDATEBY;
                            DPAH.UPDATEDATE = DateTime.Now;
                        }
                        _SmDBContext.Entry(DPAH).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                        _SmDBContext.SaveChanges();
                        #endregion

                        #region Update Process Status 
                        DGIT_SESMRN_HEADER DPH = new DGIT_SESMRN_HEADER();
                        DPH = _SmDBContext.DGIT_SESMRN_HEADER.Where(x => x.SMHEADERID == PHVM.SMHEADERID).SingleOrDefault();
                        if (DPH != null)
                        {
                            if (PHVM.APPROVAL_STATUS == 1)
                            {
                                DPH.PROCESS_STATUS = 9; //// --- 9 Request Approval Completed
                            }
                            else if (PHVM.APPROVAL_STATUS == 4)
                            {
                                DPH.PROCESS_STATUS = 6; //// --- 6 Hold by Finance,
                            }
                            else if (PHVM.APPROVAL_STATUS == 5)
                            {
                                DPH.PROCESS_STATUS = 7; //// --- 7 Forword to Taxation,
                            }
                            else if (PHVM.APPROVAL_STATUS == 2)
                            {
                                DPH.PROCESS_STATUS = 0;

                                //// --- Deactive Document Status inCase of Sendback ---////
                                DGIT_SESMRN_DTL SM_DTL = _SmDBContext.DGIT_SESMRN_DTL.Where(x => x.SMHEADERID == PHVM.SMHEADERID && x.DOC_TYPE == "SMA").FirstOrDefault();
                                if (SM_DTL != null)
                                {
                                    SM_DTL.STATUS = 0;
                                    SM_DTL.UPDATEDBY = PHVM.UPDATEBY;
                                    SM_DTL.UPDATEDATE = DateTime.Now;
                                    _SmDBContext.Entry(SM_DTL).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                    _SmDBContext.SaveChanges();
                                }
                            }
                            else if (PHVM.APPROVAL_STATUS == 3)
                            {
                                DPH.PROCESS_STATUS = 3;
                            }

                            DPH.UPDATEDBY = PHVM.UPDATEBY;
                            DPH.UPDATEDATE = DateTime.Now;
                            _SmDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _SmDBContext.SaveChanges();
                        }
                        #endregion

                        #region Add Taxation Authority
                        if (PHVM.APPROVAL_STATUS == 5) //// -- Forward to taxation
                        {
                            short resSeq = SaveTaxationAuthority(PHVM);
                            if (resSeq == 0)
                            {
                                retVal = -1;
                                transaction.Rollback();
                            }
                        }
                        #endregion

                        //// --- Add Finance Attachment --- ////
                        if (SmDetailList.Count > 0)
                        {
                            SaveSMDetails(PHVM.ADDEDBY, PHVM.SMHEADERID, SmDetailList);
                        }
                        //// --- End --- ////

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

        public short SaveTaxationAuthority(SMAppHistoryViewModel SAHVM)
        {
            short retVal = 0;
            if (SAHVM.TAXATION_AUTHORITY.Length > 0)
            {
                foreach (string strAuthority in SAHVM.TAXATION_AUTHORITY)
                {
                    //// --- Add Taxation Authority in Seq Table --- ////
                    DGIT_SMAPPAUTH_SEQ DAAS = new DGIT_SMAPPAUTH_SEQ();
                    if (_SmDBContext.DGIT_SMAPPAUTH_SEQ.Count() == 0)
                    {
                        DAAS.SMAPPSEQ_ID = 1;
                    }
                    else
                    {
                        DAAS.SMAPPSEQ_ID = _SmDBContext.DGIT_SMAPPAUTH_SEQ.Max(x => x.SMAPPSEQ_ID) + 1;
                    }
                    short appSeq = (_SmDBContext.DGIT_SMAPPAUTH_SEQ.Where(h => h.SMHEADERID == SAHVM.SMHEADERID && h.STATUS == 1).ToList().Count == 0 ? (short)1 : Convert.ToInt16(_SmDBContext.DGIT_SMAPPAUTH_SEQ.Where(n => n.SMHEADERID == SAHVM.SMHEADERID && n.STATUS == 1).Max(x => x.APP_SEQ) + 1));
                    DAAS.SMHEADERID = SAHVM.SMHEADERID;
                    DAAS.ADEMPCODE = Convert.ToInt64(strAuthority);
                    DAAS.APP_SEQ = appSeq;
                    DAAS.APPTYPE = 4; //// 4-Taxation Approval
                    DAAS.STATUS = 1;
                    DAAS.ADDEDBY = SAHVM.ADDEDBY;
                    DAAS.ADDEDDATE = DateTime.Now;
                    _SmDBContext.Entry(DAAS).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                    _SmDBContext.SaveChanges();

                    //// --- Add Taxation Authority in History Table --- ////
                    SMAppAuthSeqViewModel SASVM = new SMAppAuthSeqViewModel();
                    SASVM.ADEMPCODE = Convert.ToInt64(strAuthority);
                    SaveSMAppHis(SAHVM.ADDEDBY, SAHVM.SMHEADERID, SASVM);
                }
                retVal = 1;
            }
            return retVal;
        }

        public SearchSMViewModel SMTaxationDashboard(SearchSMViewModel VM, long loginUser)
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

            List<SMHeaderViewModel> obj = new List<SMHeaderViewModel>();

            obj = (from data in _SmDBContext.DGIT_SESMRN_HEADER
                   join _AppHis in _SmDBContext.DGIT_SMAPPHISTORY on data.SMHEADERID equals _AppHis.SMHEADERID
                   join _vendor in _SmDBContext.FINVENDORMASTERMST on data.VENDORCODE equals _vendor.VENDORCODE into _vendorJoin
                   from _vendorData in _vendorJoin.DefaultIfEmpty()
                   join _ed in _SmDBContext.ADEMPLOYEE on data.ADDEDBY equals _ed.ADEMPCODE
                   join emp in _SmDBContext.VW_ASSOCIATELVLDETAILS on data.ADDEDBY equals emp.ADEMPCODE
                   join _docRevBy in _SmDBContext.ADEMPLOYEE on data.DOC_REV_BY equals _docRevBy.ADEMPCODE into _docRevByJoin
                   from _docEmp in _docRevByJoin.DefaultIfEmpty()
                   where (string.IsNullOrEmpty(VM.Startdate) ||(data.DATEADDED >= ReqDateFrom))
                   && (string.IsNullOrEmpty(VM.ENDDATE) || (data.DATEADDED <= ReqDateTo))
                   && emp.SYKI == _Syki.SYKIID
                   && (string.IsNullOrEmpty(VM.SMNo) || data.SES_MRN_NO == VM.SMNo)
                   && (VM.ecode == 0 || data.ADDEDBY == VM.ecode)
                   && (VM.ReqStatus == -1 || _AppHis.APPROVAL_STATUS == VM.ReqStatus)
                   && (VM.ReqStatus == 0 || data.PROCESS_STATUS == 7)
                   && _AppHis.ADEMPCODE == loginUser
                   select new SMHeaderViewModel
                   {
                       SMHEADERID = data.SMHEADERID,
                       ADDEDBYNAME = _ed.FIRSTNAME + " " + _ed.LASTNAME,
                       DATEADDED = data.DATEADDED,
                       SM_NO = data.SES_MRN_NO,
                       SM_TYPE = data.SM_TYPE,
                       PONUMBER = data.PONUMBER,
                       AMOUNT = data.AMOUNT,
                       INVOICENO = data.INVOICENO,
                       INVAMOUNT = data.INVAMOUNT,
                       STATUS = data.STATUS,
                       VENDORCODE = (_vendorData == null ? "" : _vendorData.VENDORCODE),
                       VENDORNAME = (_vendorData == null ? "" : _vendorData.NAME1),
                       Document_Status = (data.DOC_STATUS == null ? 0 : data.DOC_STATUS),
                       Doc_Rev_Date = data.DOC_REV_DATE,
                       PROCESS_STATUS = data.PROCESS_STATUS,
                       TaxationAppStatus = _AppHis.APPROVAL_STATUS,
                       Emp_Detail = new Employee_Details
                       {
                           _ECode = _ed.ADEMPCODE,
                           _EName = _ed.FIRSTNAME + " " + _ed.LASTNAME,
                           _OpDesc = emp.OPERATION,
                           _DivDesc = emp.DIVISION,
                           _DepDesc = emp.DEPARTMENT,
                           _SecDescrip = emp.SECTION
                       }
                   }).ToList();
            VM.SearchResult = obj.OrderBy(o => o.DATEADDED).ToList(); ;
            return VM;
        }

        public short SMTaxationApproval(SMAppHistoryViewModel PHVM)
        {
            short retVal = 0;
            using (var transaction = _SmDBContext.Database.BeginTransaction())
            {
                try
                {
                    int FlagAdd = 0;
                    DGIT_SMAPPHISTORY DPAH = new DGIT_SMAPPHISTORY();
                    if (PHVM.SMHEADERID > 0)
                    {
                        /////////// Update Approval Status //////////
                        DPAH = _SmDBContext.DGIT_SMAPPHISTORY.Where(x => x.SMHEADERID == PHVM.SMHEADERID && x.ADEMPCODE == PHVM.ADEMPCODE && x.APPROVAL_STATUS == 0).FirstOrDefault();
                        if (DPAH == null)
                        {
                            DPAH = new DGIT_SMAPPHISTORY();
                            if (_SmDBContext.DGIT_SMAPPHISTORY.Count() == 0)
                            {
                                DPAH.SMAPPHISTORY_ID = 1;
                            }
                            else
                            {
                                DPAH.SMAPPHISTORY_ID = _SmDBContext.DGIT_SMAPPHISTORY.Max(x => x.SMAPPHISTORY_ID) + 1;
                            }
                            FlagAdd = 1;
                            DPAH.SMHEADERID = PHVM.SMHEADERID;
                            DPAH.ADDEDBY = PHVM.ADDEDBY;
                        }

                        DPAH.ADEMPCODE = PHVM.ADEMPCODE;
                        DPAH.APPROVAL_STATUS = PHVM.APPROVAL_STATUS;
                        DPAH.APPROVAL_REMARK = PHVM.APPROVAL_REMARK;
                        DPAH.APP_DATE = DateTime.Now;
                        if (FlagAdd == 0)
                        {
                            DPAH.UPDATEBY = PHVM.UPDATEBY;
                            DPAH.UPDATEDATE = DateTime.Now;
                        }
                        _SmDBContext.Entry(DPAH).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                        _SmDBContext.SaveChanges();

                        #region Update Process Status 
                        int _PendingAppCount = (from data in _SmDBContext.DGIT_SMAPPAUTH_SEQ.Where(s => s.SMHEADERID == PHVM.SMHEADERID && s.APPTYPE == 4 && s.STATUS == 1)
                                                join _AppHis in _SmDBContext.DGIT_SMAPPHISTORY on data.SMHEADERID equals _AppHis.SMHEADERID
                                                where data.ADEMPCODE == _AppHis.ADEMPCODE && _AppHis.APPROVAL_STATUS == 0
                                                select data).Count();
                        if (_PendingAppCount == 0)
                        {
                            DGIT_SESMRN_HEADER DPH = new DGIT_SESMRN_HEADER();
                            DPH = _SmDBContext.DGIT_SESMRN_HEADER.Where(x => x.SMHEADERID == PHVM.SMHEADERID).SingleOrDefault();
                            if (DPH != null)
                            {
                                if (PHVM.APPROVAL_STATUS == 1)
                                {
                                    DPH.PROCESS_STATUS = 8; //// 8-Taxation Complete
                                }

                                DPH.UPDATEDBY = PHVM.UPDATEBY;
                                DPH.UPDATEDATE = DateTime.Now;
                                _SmDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                _SmDBContext.SaveChanges();
                            }
                        }
                        #endregion

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

        public List<SMTaxationAuthViewModel> GetTaxationAuthority(short TaxTypeId, long plantId)
        {
            string strIndirectTax = "Indirect Tax";
            string strDirectTax = "Direct Tax";
            var iList = (from data in _SmDBContext.DGIT_SMTAXAEMP_MST
                         join _vW in _SmDBContext.VW_ASSOCIATELVLDETAILS on data.ECODE equals _vW.ADEMPCODE
                         join _emp in _SmDBContext.ADEMPLOYEE on data.ECODE equals _emp.ADEMPCODE
                         where data.ACTIVE == 1
                         && _vW.SYPLANTID == data.SYPLANTID
                         && _vW.SYKI == _Syki.SYKIID
                         && (TaxTypeId == 3 ? true : data.EMPTYPE == TaxTypeId)
                         && data.SYPLANTID == plantId
                         select new SMTaxationAuthViewModel
                         {
                             SMTAXEMPID = data.SMTAXEMPID,
                             SYPLANTID = data.SYPLANTID,
                             ECODE = data.ECODE,
                             ENAME = _emp.FIRSTNAME + " " + _emp.LASTNAME + " (" + (data.EMPTYPE == 1 ? strIndirectTax : strDirectTax) + ")",
                             EMPTYPE = data.EMPTYPE,
                         }).ToList();
            return iList.OrderBy(o => o.EMPTYPE).ThenBy(t => t.ENAME).ToList();
        }
        public short UpdateTaxationAuth([FromBody]List<SMAppHistoryViewModel> iList, long updatedBy)
        {
            short retVal = 0;
            using (var transaction = _SmDBContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (SMAppHistoryViewModel model in iList)
                    {
                        if (model.SMHEADERID > 0)
                        {
                            //////// Update Taxation Authority ////////
                            DGIT_SMAPPHISTORY DSAH = new DGIT_SMAPPHISTORY();
                            DSAH = _SmDBContext.DGIT_SMAPPHISTORY.Where(x => x.SMHEADERID == model.SMHEADERID && x.ADEMPCODE == model.ADEMPCODE && x.APPROVAL_STATUS == 0).FirstOrDefault();
                            if (DSAH != null)
                            {
                                DSAH.ADEMPCODE = model.SELECTED_ADEMPCODE;
                                DSAH.UPDATEBY = updatedBy;
                                DSAH.UPDATEDATE = DateTime.Now;
                                _SmDBContext.Entry(DSAH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                _SmDBContext.SaveChanges();
                            }

                            DGIT_SMAPPAUTH_SEQ DSAS = new DGIT_SMAPPAUTH_SEQ();
                            DSAS = _SmDBContext.DGIT_SMAPPAUTH_SEQ.Where(x => x.SMHEADERID == model.SMHEADERID && x.ADEMPCODE == model.ADEMPCODE && x.APPTYPE == 4).FirstOrDefault();
                            if (DSAS != null)
                            {
                                DSAS.ADEMPCODE = model.SELECTED_ADEMPCODE;
                                DSAS.UPDATEBY = updatedBy;
                                DSAS.UPDATEDATE = DateTime.Now;
                                _SmDBContext.Entry(DSAH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                _SmDBContext.SaveChanges();
                            }
                            retVal = 1;
                        }
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

        public List<PR_Div_Dep_SecViewModel> BindDepartment(long div_Id, long op_Id)
        {
            List<PR_Div_Dep_SecViewModel> iList = new List<PR_Div_Dep_SecViewModel>();
            var iColl = (from data in _SmDBContext.VW_ASSOCIATELVLDETAILS
                         where data.ACTIVE == 1
                         && (data.SYKI == _Syki.SYKIID && data.DEPARTMENTID != null && data.DEPARTMENTID != 0)
                         && (data.DIVISIONID == (div_Id == 0 ? data.DIVISIONID : div_Id))
                         && (data.OPERATIONID == (op_Id == 0 ? data.OPERATIONID : op_Id))
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
            var iColl = (from data in _SmDBContext.VW_ASSOCIATELVLDETAILS
                         where data.ACTIVE == 1
                         && (data.SYKI == _Syki.SYKIID && data.SECTION != null && data.SECTIONID != 0)
                         && (data.DEPARTMENTID == (dep_Id == 0 ? data.DEPARTMENTID : dep_Id))
                         && (data.DIVISIONID == (div_Id == 0 ? data.DIVISIONID : div_Id))
                         && (data.OPERATIONID == (op_Id == 0 ? data.OPERATIONID : op_Id))
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
        public SearchIOCGMaster BindIOCGMasterList(SearchIOCGMaster _SM)
        {
            var iColl = (from data in _SmDBContext.VW_SMIOCGAPPROVER_LIST
                         where data.ACTIVE == _SM.STATUS
                         && (data.SYKIID == _SM.KI)
                         && (_SM.OperationID == 0 || data.OPERID == _SM.OperationID)
                         && (_SM.DivisionID == 0 || data.DIVID == _SM.DivisionID)
                         && (_SM.DEPTID == 0 || data.DEPTID == _SM.DEPTID)
                         && (_SM.SECID == 0 || data.SECID == _SM.SECID)
                         select new VM_VW_SMIOCGAPPROVER_LIST
                         {
                             ACTIVE = data.ACTIVE,
                             ADORGLEVELID = data.ADORGLEVELID,
                             APP1_NAME = data.APP1_NAME,
                             APP2_NAME = data.APP2_NAME,
                             APPROVER1 = data.APPROVER1,
                             APPROVER2 = data.APPROVER2,
                             DEPTID = data.DEPTID,
                             DIVID = data.DIVID,
                             ENDAMOUNT = data.ENDAMOUNT,
                             OPERID = data.OPERID,
                             ORGLEVEL_1 = data.ORGLEVEL_1,
                             ORGLEVEL_2 = data.ORGLEVEL_2,
                             ORGLEVEL_3 = data.ORGLEVEL_3,
                             ORGNAME = data.ORGNAME,
                             SECID = data.SECID,
                             SESORMRN = data.SESORMRN,
                             SMMAP_MSTID = data.SMMAP_MSTID,
                             STARTAMOUNT = data.STARTAMOUNT,
                             SYKIID = data.SYKIID,
                             UPDBY = data.UPDBY,
                             UPDDATE = data.UPDDATE
                         }).Distinct().ToList();

            _SM.SearchResult = iColl;
            _SM.curr_KI = (long)_Syki.SYKIID;
            return _SM;
        }
        public List<VM_VW_SMIOCGAPPROVER_LIST> GetIOCGMasterByID(VM_VW_SMIOCGAPPROVER_LIST _SM)
        {
            var iColl = (from data in _SmDBContext.VW_SMIOCGAPPROVER_LIST
                         where data.SMMAP_MSTID == _SM.SMMAP_MSTID
                         select new VM_VW_SMIOCGAPPROVER_LIST
                         {
                             ACTIVE = data.ACTIVE,
                             ADORGLEVELID = data.ADORGLEVELID,
                             APP1_NAME = data.APP1_NAME,
                             APP2_NAME = data.APP2_NAME,
                             APPROVER1 = data.APPROVER1,
                             APPROVER2 = data.APPROVER2,
                             DEPTID = data.DEPTID,
                             DIVID = data.DIVID,
                             ENDAMOUNT = data.ENDAMOUNT,
                             OPERID = data.OPERID,
                             ORGLEVEL_1 = data.ORGLEVEL_1,
                             ORGLEVEL_2 = data.ORGLEVEL_2,
                             ORGLEVEL_3 = data.ORGLEVEL_3,
                             ORGNAME = data.ORGNAME,
                             SECID = data.SECID,
                             SESORMRN = data.SESORMRN,
                             SMMAP_MSTID = data.SMMAP_MSTID,
                             STARTAMOUNT = data.STARTAMOUNT,
                             SYKIID = data.SYKIID,
                             UPDBY = data.UPDBY,
                             UPDDATE = data.UPDDATE
                         }).Distinct().ToList();

            return iColl;
        }
        public List<PR_Div_Dep_SecViewModel> GetKiLIST()
        {
            var KILIST = (from data in _SmDBContext.SYKI
                          select new PR_Div_Dep_SecViewModel
                          {
                              Text = data.KICODE,
                              Value = (long)(data.SYKIID)
                          }
                      ).ToList();
            return KILIST.OrderByDescending(m=>m.Value).ToList();
        }
        public Tuple<short, string> SaveIOCGMasterData(VM_VW_SMIOCGAPPROVER_LIST model)
        {
            short retVal = 0;
            Tuple<short, string> _retVal_tuple;
            using (var transaction = _SmDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (model != null)
                    {
                        #region "Validate Request Whether Master already exist or not"
                        long adorglevelid = 0;
                        if (model.SECID != null && model.SECID != 0)
                        {
                            adorglevelid = (long)model.SECID;
                        }
                        else if (model.DEPTID != null && model.DEPTID != 0)
                        {
                            adorglevelid = (long)model.DEPTID;
                        }
                        else if (model.DIVID != null && model.DIVID != 0)
                        {
                            adorglevelid = (long)model.DIVID;
                        }
                        else if (model.OPERID != null && model.OPERID != 0)
                        {
                            adorglevelid = (long)model.OPERID;
                        }

                        var objSearchList = _SmDBContext.DGIT_SMOPERATION_MAP.Where(m => m.ADORGLEVELID == adorglevelid && m.SESORMRN == model.SESORMRN && m.ACTIVE == 1 && m.SYKIID == _Syki.SYKIID).ToList();
                        foreach (var obj in objSearchList)
                        {
                            if (model.STARTAMOUNT >= obj.STARTAMOUNT || model.STARTAMOUNT <= obj.ENDAMOUNT)
                            {
                                _retVal_tuple = new Tuple<short, string>(-1, "Master data already exist.");
                                return _retVal_tuple;
                            }
                            if (model.ENDAMOUNT >= obj.STARTAMOUNT || model.ENDAMOUNT <= obj.ENDAMOUNT)
                            {
                                _retVal_tuple = new Tuple<short, string>(-1, "Master data already exist.");
                                return _retVal_tuple;
                            }
                        }

                        #endregion


                        //////// Update Taxation Authority ////////
                        DGIT_SMOPERATION_MAP DSAH = new DGIT_SMOPERATION_MAP();
                        if (_SmDBContext.DGIT_SESMRN_HEADER.Count() == 0)
                        {
                            DSAH.SMMAP_MSTID = 1;
                        }
                        else
                        {
                            DSAH.SMMAP_MSTID = _SmDBContext.DGIT_SMOPERATION_MAP.Max(x => x.SMMAP_MSTID) + 1;
                        }
                        DSAH.ACTIVE = 1;
                        DSAH.ADDEDBY = (long)model.UPDBY;
                        DSAH.DATEADDED = DateTime.Now;
                        DSAH.ADORGLEVELID = adorglevelid;
                        DSAH.APPROVER1 = model.APPROVER1;
                        DSAH.APPROVER2 = model.APPROVER2;
                        DSAH.ENDAMOUNT = model.ENDAMOUNT;
                        DSAH.SESORMRN = model.SESORMRN;
                        DSAH.STARTAMOUNT = model.STARTAMOUNT;
                        DSAH.SYKIID = (long)_Syki.SYKIID;
                        _SmDBContext.Entry(DSAH).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                        _SmDBContext.SaveChanges();
                        retVal = 1;
                    }
                    transaction.Commit();
                    _retVal_tuple = new Tuple<short, string>(retVal, "Record Saved Successfully.");
                    return _retVal_tuple;
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                    _retVal_tuple = new Tuple<short, string>(retVal, ex.InnerException.Message.ToString());
                    return _retVal_tuple;
                }
                
            }
        }
        public Tuple<short, string> EditIOCGMasterData(VM_VW_SMIOCGAPPROVER_LIST model)
        {
            short retVal = 0;
            Tuple<short, string> _retVal_tuple;
            using (var transaction = _SmDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (model != null)
                    {
                        #region "Validate Request Whether Master already exist or not"
                        long adorglevelid = 0;
                        if (model.SECID != null && model.SECID != 0)
                        {
                            adorglevelid = (long)model.SECID;
                        }
                        else if (model.DEPTID != null && model.DEPTID != 0)
                        {
                            adorglevelid = (long)model.DEPTID;
                        }
                        else if (model.DIVID != null && model.DIVID != 0)
                        {
                            adorglevelid = (long)model.DIVID;
                        }
                        else if (model.OPERID != null && model.OPERID != 0)
                        {
                            adorglevelid = (long)model.OPERID;
                        }

                        var objSearchList = _SmDBContext.DGIT_SMOPERATION_MAP.Where(m => m.ADORGLEVELID == adorglevelid && m.SESORMRN == model.SESORMRN && m.ACTIVE == 1 && m.SYKIID == _Syki.SYKIID && m.SMMAP_MSTID!=model.SMMAP_MSTID).ToList();
                        foreach (var obj in objSearchList)
                        {
                            if (model.STARTAMOUNT >= obj.STARTAMOUNT || model.STARTAMOUNT <= obj.ENDAMOUNT)
                            {
                                _retVal_tuple = new Tuple<short, string>(-1, "Master data already exist.");
                                return _retVal_tuple;
                            }
                            if (model.ENDAMOUNT >= obj.STARTAMOUNT || model.ENDAMOUNT <= obj.ENDAMOUNT)
                            {
                                _retVal_tuple = new Tuple<short, string>(-1, "Master data already exist.");
                                return _retVal_tuple;
                            }
                        }

                        #endregion


                        //////// Update Taxation Authority ////////
                        DGIT_SMOPERATION_MAP DSAH;
                        DSAH = _SmDBContext.DGIT_SMOPERATION_MAP.Where(x => x.SMMAP_MSTID == model.SMMAP_MSTID).SingleOrDefault();
                        DSAH.ACTIVE = model.ACTIVE;
                        DSAH.MODIFIEDBY = (long)model.UPDBY;
                        DSAH.DATELSTMOD = DateTime.Now;
                        DSAH.ADORGLEVELID = adorglevelid;
                        DSAH.APPROVER1 = model.APPROVER1;
                        DSAH.APPROVER2 = model.APPROVER2;
                        DSAH.ENDAMOUNT = model.ENDAMOUNT;
                        DSAH.SESORMRN = model.SESORMRN;
                        DSAH.STARTAMOUNT = model.STARTAMOUNT;
                        DSAH.SYKIID = (long)_Syki.SYKIID;
                        _SmDBContext.Entry(DSAH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        _SmDBContext.SaveChanges();
                        retVal = 1;
                    }
                    transaction.Commit();
                    _retVal_tuple = new Tuple<short, string>(retVal, "Record Saved Successfully.");
                    return _retVal_tuple;
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                    _retVal_tuple = new Tuple<short, string>(retVal, ex.Message.ToString());
                    return _retVal_tuple;
                }

            }
        }

        public short SavePADetails(long AddedBy, List<VM_PADetailViewModel> PDVMList)
        {
            short retVal = 0;
            foreach (VM_PADetailViewModel PDVM in PDVMList)
            {
                DGIT_PAYMENTADVISE_DTL DPD = new DGIT_PAYMENTADVISE_DTL();
                int FlagAdd = 0;
                if (PDVM.PAYMENTADVISE_NO.Length > 0)
                {
                    if (PDVM.DOC_TYPE == "PA")
                    {
                        DPD = _SmDBContext.DGIT_PAYMENTADVISE_DTL.Where(x => x.PAYMENTADVISE_NO == PDVM.PAYMENTADVISE_NO && x.DOC_TYPE == PDVM.DOC_TYPE).FirstOrDefault();
                        if (DPD != null)
                        {
                            _SmDBContext.DGIT_PAYMENTADVISE_DTL.Remove(DPD);
                            _SmDBContext.SaveChanges();
                        }
                    }

                    DPD = new DGIT_PAYMENTADVISE_DTL();
                    if (_SmDBContext.DGIT_PAYMENTADVISE_DTL.Count() == 0)
                    {
                        DPD.PA_DTL_ID = 1;
                        FlagAdd = 1;
                    }
                    else
                    {
                        DPD.PA_DTL_ID = _SmDBContext.DGIT_PAYMENTADVISE_DTL.Max(x => x.PA_DTL_ID) + 1;
                        FlagAdd = 1;
                    }
                    DPD.PAYMENTADVISE_NO = PDVM.PAYMENTADVISE_NO;
                    DPD.PA_DATE = PDVM.PA_DATE;
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
                    _SmDBContext.Entry(DPD).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _SmDBContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }
        public List<VM_PADetailViewModel> GetPAAttachmentDetail(string PA_HeaderID)
        {
            return (from _SMDetail in _SmDBContext.DGIT_PAYMENTADVISE_DTL.Where(d => d.PAYMENTADVISE_NO == PA_HeaderID)
                    where _SMDetail.STATUS == 1
                    select new VM_PADetailViewModel
                    {
                        PA_DTL_ID = _SMDetail.PA_DTL_ID,
                        PAYMENTADVISE_NO = _SMDetail.PAYMENTADVISE_NO,
                        DOC_TYPE = _SMDetail.DOC_TYPE,
                        ADDITIONAL_INFO = _SMDetail.ADDITIONAL_INFO,
                        FILENAME = _SMDetail.FILENAME,
                        STATUS = (short)_SMDetail.STATUS
                    }).ToList();
        }
        public short DeletePAAttachment(string fileName, string docType, long SMHEADERID)
        {
            short retVal = 0;
            if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(docType) && SMHEADERID > 0)
            {
                DGIT_PAYMENTADVISE_DTL DT = _SmDBContext.DGIT_PAYMENTADVISE_DTL.Where(x => x.FILENAME == fileName && x.DOC_TYPE == docType && x.PA_DTL_ID == SMHEADERID).FirstOrDefault();
                if (DT != null)
                {
                    _SmDBContext.DGIT_PAYMENTADVISE_DTL.Remove(DT);
                    _SmDBContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }
        public short deletePAReq(DeletePA obj)
        {
            long decimal_Ses = (long)Convert.ToDecimal(obj.SES_NO);
            try
            {
                DGIT_SES_PAYMENTADVISEMASTER model = _SmDBContext.DGIT_SES_PAYMENTADVISEMASTER.Where(x => x.SES_MRN_NO == decimal_Ses && x.PAYMENTADVISE_NO==(obj.PAYMENTADVISE_NO)).FirstOrDefault();
                model.ISDELETE = 1;
                model.REMARK = obj.REMARK;
                _SmDBContext.Entry(model).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                DGIT_SESMRN_HEADER model1 = _SmDBContext.DGIT_SESMRN_HEADER.Where(x => x.SMHEADERID == decimal_Ses).FirstOrDefault();
                model1.PA_GENERATED = 0;
                _SmDBContext.Entry(model).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _SmDBContext.SaveChanges();
                return 1;
            }
            catch(Exception ex)
            {
                return -1;
            }
        }
        public short SavePARequest(VM_PaymentAdvise_Master model)
        {
            short retVal = 0; /*long retHeaderId = 0*/;
            VendorViewModel vendordtls = new VendorViewModel();
            //Tuple<short, long> _retVal_tuple;
            foreach (string ses_no in model.reqno_list)
            {
                using (var transaction = _SmDBContext.Database.BeginTransaction())
                {
                    try
                    {
                        long decimal_ses = (long)Convert.ToDouble(ses_no);
                        DGIT_SES_PAYMENTADVISEMASTER DPH = new DGIT_SES_PAYMENTADVISEMASTER();
                        //var result = (from data in _SmDBContext.DGIT_SESMRN_HEADER.Where(x => x.SMHEADERID == decimal_ses)
                        //              select new VM_Combine_PDF
                        //              {
                        //                  SES_NO = data.SES_MRN_NO,
                        //                  PONO = data.PONUMBER
                        //              }).FirstOrDefault();
                        int FlagAdd = 0;
                        if (model.PAYMENTADVISE_NO.Length > 0)
                        {
                            DPH = new DGIT_SES_PAYMENTADVISEMASTER();
                            decimal ses_decimal = Convert.ToDecimal(ses_no);
                            vendordtls = (from _sesdtls in _SmDBContext.DGIT_SESMRN_HEADER
                                                          join _vendordtls in _SmDBContext.FINVENDORMASTERMST on _sesdtls.VENDORCODE equals _vendordtls.VENDORCODE
                                                          where _sesdtls.SMHEADERID == decimal_ses
                                                          select new VendorViewModel
                                                          {
                                                              VENDORCODE = _vendordtls.VENDORCODE,
                                                              VENDORNAME = _vendordtls.NAME1
                                                          }).FirstOrDefault();
                            if (_SmDBContext.DGIT_SES_PAYMENTADVISEMASTER.Where(x => x.PAYMENTADVISE_NO == model.PAYMENTADVISE_NO && x.SES_MRN_NO == ses_decimal && x.ISDELETE==0).FirstOrDefault()!=null)
                            {
                                retVal = 2;
                                return retVal; //// -- record already exist.
                            }
                            if(_SmDBContext.DGIT_SES_PAYMENTADVISEMASTER.Where(x=>x.SES_MRN_NO == ses_decimal && x.ISDELETE == 1).FirstOrDefault()!=null){
                                DPH = _SmDBContext.DGIT_SES_PAYMENTADVISEMASTER.Where(x => x.SES_MRN_NO == ses_decimal).FirstOrDefault();
                                
                            }
                            else if (_SmDBContext.DGIT_SES_PAYMENTADVISEMASTER.Count() == 0)
                            {
                                DPH.PAHEADER_ID = 1;
                                FlagAdd = 1;
                            }
                            else
                            {
                                DPH.PAHEADER_ID = _SmDBContext.DGIT_SES_PAYMENTADVISEMASTER.Max(x => x.PAHEADER_ID) + 1;
                                FlagAdd = 1;
                            }
                            DPH.SES_MRN_NO = (long)Convert.ToDouble(ses_no);
                        }
                        DPH.ISDELETE = 0;
                        DPH.STATUS = 1;
                        DPH.REMARK = model.REMARK==null ? "":model.REMARK;
                        DPH.DOCUMENT_PATH = model.DOCUMENT_PATH;
                        DPH.PAYMENTADVISE_NO = model.PAYMENTADVISE_NO;
                        DPH.PA_DATE = model.PA_DATE;
                        DPH.ISSEND = 0;
                        DPH.VENDOR_CODE = vendordtls.VENDORCODE;
                        DPH.VENDOR_NAME = vendordtls.VENDORNAME;
                        if (FlagAdd == 1)
                        {
                            DPH.ADDEDBY = (short?)model.ADDEDBY;
                            DPH.DATEADDED = DateTime.Now;
                        }
                        else
                        {
                            DPH.UPDATEBY = model.UPDATEBY;
                            DPH.UPDATEDDATE = DateTime.Now;
                        }
                        _SmDBContext.Entry(DPH).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                        _SmDBContext.SaveChanges();

                        //--- Save send to Dms status ---




                        transaction.Commit();
                        retVal = 1;

                    }
                    catch (Exception ex)
                    {
                        retVal = -1;
                        transaction.Rollback();
                    }

                }

                using (var transaction = _SmDBContext.Database.BeginTransaction())
                {
                    try
                    {
                        if (model != null)
                        {
                            long ses_id = (long)Convert.ToDouble(ses_no);
                            //var result = (from data in _SmDBContext.DGIT_SESMRN_HEADER.Where(x => x.SMHEADERID == ses_id)
                            //              select new VM_Combine_PDF
                            //              {
                            //                  SES_NO = data.SES_MRN_NO,
                            //                  PONO = data.PONUMBER
                            //              }).FirstOrDefault();
                            //string ses_No = result.SES_NO;
                            //var PHVM = (from data in _SmDBContext.DGIT_SESMRN_HEADER.Where(x => x.SES_MRN_NO == ses_No)
                            //            select new DGIT_SESMRN_HEADER
                            //            {
                            //                SEND_TO_DMS = data.SEND_TO_DMS
                            //            }
                            //                          ).ToList();
                            //PHVM.SEND_TO_DMS = 1;
                            
                            DGIT_SESMRN_HEADER DSAH;
                            DSAH = _SmDBContext.DGIT_SESMRN_HEADER.Where(x => x.SMHEADERID == ses_id).SingleOrDefault();
                            DSAH.PA_GENERATED = 1;
                            _SmDBContext.Entry(DSAH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _SmDBContext.SaveChanges();
                            retVal = 1;
                        }
                        transaction.Commit();
                        
                    }
                    catch (Exception ex)
                    {
                        retVal = -1;
                        transaction.Rollback();
                        
                    }

                }
            }
            return retVal;
        }

        public List<VM_PaymentAdvise_Master> GetPADetails(VM_PaymentAdvise_Master obj)
        {
            DateTime ReqDateFrom = DateTime.Now.Date;
            DateTime ReqDateTo = DateTime.Now.Date;
            List<VM_PaymentAdvise_Master> result = new List<VM_PaymentAdvise_Master>();
            if (!string.IsNullOrEmpty(obj.Startdate))
            {
                ReqDateFrom = DateTime.ParseExact(obj.Startdate, "dd-MMM-yyyy", null);
            }
            if (!string.IsNullOrEmpty(obj.ENDDATE))
            {
                ReqDateTo = DateTime.ParseExact(obj.ENDDATE + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
            }
            if ((obj.PAYMENTADVISE_NO == "" || obj.PAYMENTADVISE_NO == null) && obj.SES_MRN_NO == 0 && (obj.VENDORCODE == "" || obj.VENDORCODE == null) && (obj.Invoiceno == "" || obj.Invoiceno == null))
            {
                result= (from _SMDetail in _SmDBContext.DGIT_SES_PAYMENTADVISEMASTER
                          where _SMDetail.STATUS == 1
                         && (string.IsNullOrEmpty(obj.Startdate) || (_SMDetail.PA_DATE >= ReqDateFrom))
                        && (string.IsNullOrEmpty(obj.ENDDATE) || (_SMDetail.PA_DATE <= ReqDateTo))
                          select new VM_PaymentAdvise_Master
                          {
                              PAYMENTADVISE_NO = _SMDetail.PAYMENTADVISE_NO,
                              SES_MRN_NO = (long)_SMDetail.SES_MRN_NO,
                              DOCUMENT_PATH = _SMDetail.DOCUMENT_PATH,
                              STATUS = _SMDetail.STATUS,
                              ISDELETE = _SMDetail.ISDELETE,
                              REMARK = _SMDetail.REMARK,
                              PA_DATE = _SMDetail.PA_DATE,
                              VENDORCODE = _SMDetail.VENDOR_CODE,
                              VENDORNAME = _SMDetail.VENDOR_NAME,
                              Invoiceno = (_SmDBContext.DGIT_SESMRN_HEADER.Where(x => x.SMHEADERID == (long)_SMDetail.SES_MRN_NO).FirstOrDefault().INVOICENO)

                          }).ToList();
            }
            else
            {
                //decimal PANO = Convert.ToDecimal(obj.PAYMENTADVISE_NO);
                decimal ses_mrn_no = Convert.ToDecimal(obj.SES_MRN_NO);
                if (obj.PAYMENTADVISE_NO != "" && obj.PAYMENTADVISE_NO != null && ses_mrn_no != 0 && obj.VENDORCODE != "" && obj.VENDORCODE != null)
                {
                    result= (from _SMDetail in _SmDBContext.DGIT_SES_PAYMENTADVISEMASTER
                              where (_SMDetail.PAYMENTADVISE_NO == obj.PAYMENTADVISE_NO && _SMDetail.SES_MRN_NO == ses_mrn_no && _SMDetail.VENDOR_CODE == obj.VENDORCODE)
                              && _SMDetail.STATUS == 1
                           && (string.IsNullOrEmpty(obj.Startdate) || (_SMDetail.PA_DATE >= ReqDateFrom))
                        && (string.IsNullOrEmpty(obj.ENDDATE) || (_SMDetail.PA_DATE <= ReqDateTo))
                              select new VM_PaymentAdvise_Master
                            {
                                PAYMENTADVISE_NO = _SMDetail.PAYMENTADVISE_NO,
                                SES_MRN_NO = (long)_SMDetail.SES_MRN_NO,
                                DOCUMENT_PATH = _SMDetail.DOCUMENT_PATH,
                                STATUS = _SMDetail.STATUS,
                                ISDELETE = _SMDetail.ISDELETE,
                                PA_DATE = _SMDetail.PA_DATE,
                                REMARK = _SMDetail.REMARK,
                                VENDORCODE = _SMDetail.VENDOR_CODE,
                                VENDORNAME = _SMDetail.VENDOR_NAME,
                                Invoiceno = (_SmDBContext.DGIT_SESMRN_HEADER.Where(x => x.SMHEADERID == (long)_SMDetail.SES_MRN_NO).FirstOrDefault().INVOICENO)
                            }).ToList();
                }
                else
                {
                    result= (from _SMDetail in _SmDBContext.DGIT_SES_PAYMENTADVISEMASTER
                             join _sesdtls in _SmDBContext.DGIT_SESMRN_HEADER on _SMDetail.SES_MRN_NO equals _sesdtls.SMHEADERID
                             where (string.IsNullOrEmpty(obj.PAYMENTADVISE_NO) || _SMDetail.PAYMENTADVISE_NO == obj.PAYMENTADVISE_NO)
                            && (obj.SES_MRN_NO == 0 || _SMDetail.SES_MRN_NO == obj.SES_MRN_NO)
                            && (string.IsNullOrEmpty(obj.VENDORCODE) || _SMDetail.VENDOR_CODE == obj.VENDORCODE)
                            && _SMDetail.STATUS == 1
                            && (string.IsNullOrEmpty(obj.Startdate) || (_SMDetail.PA_DATE >= ReqDateFrom))
                        && (string.IsNullOrEmpty(obj.ENDDATE) || (_SMDetail.PA_DATE <= ReqDateTo))
                            && (string.IsNullOrEmpty(obj.Invoiceno) || (_sesdtls.INVOICENO==obj.Invoiceno))
                             select new VM_PaymentAdvise_Master
                            {
                                PAYMENTADVISE_NO = _SMDetail.PAYMENTADVISE_NO,
                                SES_MRN_NO = (long)_SMDetail.SES_MRN_NO,
                                DOCUMENT_PATH = _SMDetail.DOCUMENT_PATH,
                                STATUS = _SMDetail.STATUS,
                                ISDELETE = _SMDetail.ISDELETE,
                                 PA_DATE = _SMDetail.PA_DATE,
                                 REMARK = _SMDetail.REMARK,
                                VENDORCODE = _SMDetail.VENDOR_CODE,
                                VENDORNAME = _SMDetail.VENDOR_NAME,
                                Invoiceno = (_SmDBContext.DGIT_SESMRN_HEADER.Where(x => x.SMHEADERID == (long)_SMDetail.SES_MRN_NO).FirstOrDefault().INVOICENO)
                            }).ToList();
                }
            }
            return result;
            
        }

        public long GetSMNO(string InvoiceNo,DateTime InvoiceDate,string VendorCode)
        {
            try
            {
                var result = (from data in _SmDBContext.DGIT_SESMRN_HEADER.Where(x => x.INVOICENO == InvoiceNo && x.VENDORCODE == VendorCode && x.INVOICEDATE == InvoiceDate && x.PA_GENERATED==0)
                              select new VM_Combine_PDF
                              {
                                  SES_NO = (data.SMHEADERID)
                              }).FirstOrDefault();
                return result.SES_NO;
            }
            catch(Exception ex)
            {
                return -1;
            }
        }

        public short ISIOCGBLOCK(Employee_Details _Employee_Details)
        {
            short _RetVal = 0;
            long lngop = string.IsNullOrEmpty(_Employee_Details.Operation_Id) ? 0 : Convert.ToInt64(_Employee_Details.Operation_Id);
            long lngdiv = string.IsNullOrEmpty(_Employee_Details.Division_Id) ? 0 : Convert.ToInt64(_Employee_Details.Division_Id);
            long lngdpt = string.IsNullOrEmpty(_Employee_Details.Department_Id) ? 0 : Convert.ToInt64(_Employee_Details.Department_Id);
            long lngsec = string.IsNullOrEmpty(_Employee_Details.Section_Id) ? 0 : Convert.ToInt64(_Employee_Details.Section_Id);

            var result = (from _SMDetail in _SmDBContext.DGIT_SMIOCGBLOCK
                      where (_SMDetail.ADORGLEVELID == lngop
                      || _SMDetail.ADORGLEVELID == lngdiv
                      || _SMDetail.ADORGLEVELID == lngdpt
                      || _SMDetail.ADORGLEVELID == lngsec)
                      && _SMDetail.STATUS==1
                          select _SMDetail
                      );
            if (result.Count() > 0)
            {
                _RetVal = 1;
            }
            return _RetVal;
        }

        //============Below added by aumento on 16052023 for SR50703==========================================================================================
        public List<A00ADORGLEVELList> GetPortalAutocompleteSuggestionsForORG(long ddOLvType)
        {


            long strKIID = (long)_SmDBContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
            List<A00ADORGLEVELList> empdata_ans = new List<A00ADORGLEVELList>();


            var empdata = (from data in _SmDBContext.ADORGLEVEL
                           where data.SYKIID == strKIID &&
                           data.ADORGLEVELTYPEID == ddOLvType
                           select new
                           {
                               ACTIVE = data.ACTIVE,
                               ADORGLEVELID = data.ADORGLEVELID,
                               LEVELDESCRIP = data.LEVELDESCRIP,
                           }).ToList();



            List<A00ADORGLEVELList> portalUserDtos = new List<A00ADORGLEVELList>();
            portalUserDtos = (from userdata in empdata
                                  // where (userdata.ADORGLEVELID.ToString().Contains(ddOLvType.ToString()))
                              select new A00ADORGLEVELList
                              {
                                  ADORGLEVELID = userdata.ADORGLEVELID,
                                  LEVELDESCRIP = userdata.LEVELDESCRIP,
                              }).ToList();

            return portalUserDtos;
        }

        public List<A00ADORGLEVELList> PortalAutocompleteSuggestionsForORG_New(long ddOLvType)
        {
            List<A00ADORGLEVELList> dclist = new List<A00ADORGLEVELList>();
            try
            {
                long strKIID = (long)_SmDBContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;

                return (from _data in _SmDBContext.ADORGLEVEL.Where(x => x.ADORGLEVELTYPEID == ddOLvType && x.SYKIID == strKIID)
                        select new A00ADORGLEVELList
                        {

                            ADORGLEVELID = _data.ADORGLEVELID,
                            LEVELDESCRIP = _data.ADORGLEVELID + " - " + _data.LEVELDESCRIP,

                        }).ToList().OrderBy(o => o.LEVELDESCRIP).ToList();
            }
            catch (Exception e)
            {
                e.Message.ToString();
            }
            return dclist;
        }

        public Tuple<short, long> SaveAdorglevel(long AddedBy, string ddOLvType, string Organization)
        {
            short retVal = 0; long retHeaderId = 0;
            Tuple<short, long> _retVal_tuple;
            int FlagAdd = 0;
            long max = 0;
            using (var transaction = _SmDBContext.Database.BeginTransaction())
            {
                try
                {
                    var count = _SmDBContext.DGIT_SMIOCGBLOCK.Count();
                    if (count == 0)
                    {
                        max = 1;
                    }
                    else
                    {

                        max = _SmDBContext.DGIT_SMIOCGBLOCK.Max(i => i.SMIOCGBLOCKID) + 1;

                    }
                    FlagAdd = 1;
                    //long SMIOCGBLOCKID = long.Parse(ddOLvType);
                    long ADORGLEVELID = long.Parse(Organization);
                    var ds = (from post in _SmDBContext.DGIT_SMIOCGBLOCK.Where(a => a.ADORGLEVELID == ADORGLEVELID)
                              select post).ToList();
                    if (ds.Count == 0)
                    {
                        if (ddOLvType != "")
                        {

                            DGIT_SMIOCGBLOCK PSM = new DGIT_SMIOCGBLOCK();

                            PSM.SMIOCGBLOCKID = max;
                            //PSM.SMIOCGBLOCKID = long.Parse(ddOLvType);
                            PSM.ADORGLEVELID = long.Parse(Organization);
                            PSM.STATUS = 1;
                            PSM.ADDEDBY = AddedBy;
                            PSM.DATEADDED = DateTime.Now;

                            //_POADBContext.Entry(PSM).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                            _SmDBContext.Entry(PSM).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _SmDBContext.SaveChanges();

                            //_POADBContext.Entry(PSM).State = Microsoft.EntityFrameworkCore.EntityState.Detached;



                            transaction.Commit();
                        }
                        retVal = 1;
                    }
                    else
                    {
                        retVal = 0;
                    }


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

        public List<SMDGIT_SMIOCGBLOCKViewModel> GetSMIOCGBLOCKData()
        {
            try
            {
                var query =
                  (from post in _SmDBContext.DGIT_SMIOCGBLOCK
                   join adl in _SmDBContext.ADORGLEVEL on post.ADORGLEVELID equals adl.ADORGLEVELID
                   join adlt in _SmDBContext.ADORGLEVELTYPE on adl.ADORGLEVELTYPEID equals adlt.ADORGLEVELTYPEID
                   orderby adl.LEVELDESCRIP
                   select new
                   {
                       SRNO = post.SMIOCGBLOCKID,
                       SMIOCGBLOCKID = post.SMIOCGBLOCKID,
                       LEVELTYPE = adlt.LEVELTYPE,
                       ADORGLEVELID = post.ADORGLEVELID,
                       LEVEL = adl.LEVELDESCRIP,
                       STATUS = post.STATUS,
                   }).ToList();

                List<SMDGIT_SMIOCGBLOCKViewModel> portalUserDtos = new List<SMDGIT_SMIOCGBLOCKViewModel>();
                portalUserDtos = (from userdata in query
                                  select new SMDGIT_SMIOCGBLOCKViewModel
                                  {
                                      SRNO = userdata.SRNO,
                                      SMIOCGBLOCKID = userdata.SMIOCGBLOCKID,
                                      LevelType = userdata.LEVELTYPE,
                                      ADORGLEVELID = userdata.ADORGLEVELID,
                                      LevelDesc = userdata.ADORGLEVELID + " - " + userdata.LEVEL,
                                      STATUS = userdata.STATUS,
                                  }).ToList();

                return portalUserDtos;
            }
            catch (Exception ex)
            {

                throw ex;
            }



        }

        public List<SMDGIT_SMIOCGBLOCKViewModel> UpdateStatus(long SMIOCGBLOCKID, long ADORGLEVELID, long SRNO)
        {
            try
            {
                int FlagAddUpdate = 0;


                //short retVal = 0;
                //int FlagAddUpdate = 0;
                if (SMIOCGBLOCKID != null && ADORGLEVELID != null && SRNO != null)
                {
                    
                    DGIT_SMIOCGBLOCK DT = _SmDBContext.DGIT_SMIOCGBLOCK.Where(x => x.ADORGLEVELID == ADORGLEVELID).FirstOrDefault();
                    if (DT.STATUS == 1)
                    {
                        DT.STATUS = 0;
                    }
                    else
                    {
                        DT.STATUS = 1;
                    }
                    //_SmDBContext.SaveChanges();
                    _SmDBContext.Entry(DT).State = FlagAddUpdate == 0 ? Microsoft.EntityFrameworkCore.EntityState.Modified : Microsoft.EntityFrameworkCore.EntityState.Added;
                    _SmDBContext.SaveChanges();

                    //}
                }

                return (from post in _SmDBContext.DGIT_SMIOCGBLOCK
                        join adl in _SmDBContext.ADORGLEVEL on post.ADORGLEVELID equals adl.ADORGLEVELID
                        join adlt in _SmDBContext.ADORGLEVELTYPE on adl.ADORGLEVELTYPEID equals adlt.ADORGLEVELTYPEID
                        orderby adl.LEVELDESCRIP
                        select new SMDGIT_SMIOCGBLOCKViewModel
                        {
                            SRNO = post.SMIOCGBLOCKID,
                            SMIOCGBLOCKID = post.SMIOCGBLOCKID,
                            LevelType = post.SMIOCGBLOCKID + " - " + adlt.LEVELTYPE,
                            ADORGLEVELID = post.ADORGLEVELID,
                            LevelDesc = post.ADORGLEVELID + " - " + adl.LEVELDESCRIP,
                            STATUS = post.STATUS,
                        }).ToList();
            }
            catch (Exception ex)
            {
                //return retVal;
                return (from post in _SmDBContext.DGIT_SMIOCGBLOCK
                        join adl in _SmDBContext.ADORGLEVEL on post.ADORGLEVELID equals adl.ADORGLEVELID
                        join adlt in _SmDBContext.ADORGLEVELTYPE on adl.ADORGLEVELTYPEID equals adlt.ADORGLEVELTYPEID
                        orderby adl.LEVELDESCRIP
                        select new SMDGIT_SMIOCGBLOCKViewModel
                        {
                            SRNO = post.SMIOCGBLOCKID,
                            SMIOCGBLOCKID = post.SMIOCGBLOCKID,
                            LevelType = post.SMIOCGBLOCKID + " - " + adlt.LEVELTYPE,
                            ADORGLEVELID = post.ADORGLEVELID,
                            LevelDesc = post.ADORGLEVELID + " - " + adl.LEVELDESCRIP,
                            STATUS = post.STATUS,
                        }).ToList();
            }
        }


        public short SaveUpdateStatus(List<SMDGIT_SMIOCGBLOCKViewModel> ITDList)
        {

            short retVal = 0; long retHeaderId = 0;
            Tuple<short, long> _retVal_tuple;


            using (var transaction = _SmDBContext.Database.BeginTransaction())
            {
                try
                {
                    int FlagAdd = 0;
                    if (ITDList.Count() > 0)
                    {
                        foreach (var item1 in ITDList)
                        {
                            long ADORGLEVELID = item1.ADORGLEVELID;
                            List<DGIT_SMIOCGBLOCK> DT = _SmDBContext.DGIT_SMIOCGBLOCK.Where(x => x.ADORGLEVELID == ADORGLEVELID).ToList();
                            foreach (var item in DT)
                            {
                                DGIT_SMIOCGBLOCK pr = item;
                                if (item.STATUS == 1)
                                {
                                    pr.STATUS = 0;
                                }
                                else
                                {
                                    pr.STATUS = 1;
                                }

                                _SmDBContext.SaveChanges();

                                _SmDBContext.Entry(pr).State = FlagAdd == 0 ? Microsoft.EntityFrameworkCore.EntityState.Modified : Microsoft.EntityFrameworkCore.EntityState.Added;
                                _SmDBContext.SaveChanges();
                            }
                            
                        }
                        transaction.Commit();
                    }
                    retVal = 1;
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }

                return retVal;
            }
        }

        //=====================================================================================================================================================

        //Added by aumento for SESMRN as on 23112023===========================
        public IEnumerable<SYSITE> Bind_SYSite()
        {
            IEnumerable<SYSITE> iList;
            
            iList = (from data in _SmDBContext.SYSITE.Where(x => x.ACTIVE == 1 &&(x.SYSITEID==3 || x.SYSITEID == 6 || x.SYSITEID == 8 || x.SYSITEID == 21 || x.SYSITEID == 22)).ToList()
                     select new SYSITE
                     {
                         SYSITEID = data.SYSITEID,
                         DESCRIP = data.DESCRIP + " Finance",
                     });
            return iList;
        }
        public long GETSYSITEUSERID(string UserId)
        {
            int retval = 0;
            long UserId1 = Convert.ToInt64(UserId.ToString());
            var ID = _SmDBContext.VW_ASSOCIATELVLDETAILS.Where(x => x.ADEMPCODE == UserId1 && x.ACTIVE == 1).Select(x => x.SYSITEID).FirstOrDefault();

            
            return Int64.Parse(ID.ToString());
        }

        public SearchSMViewModel SMIPDashboard(SearchSMViewModel VM, long plantId)
        {
            DateTime applicabledate = DateTime.ParseExact("20-NOV-2021", "dd-MMM-yyyy", null);
            DateTime ReqDateFrom = DateTime.Now.Date;
            DateTime InvoiceDate = DateTime.Now.Date;
            DateTime ReqDateTo = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(VM.Startdate))
            {
                ReqDateFrom = DateTime.ParseExact(VM.Startdate, "dd-MMM-yyyy", null);
            }
            if (!string.IsNullOrEmpty(VM.INVOICEDATE))
            {
                InvoiceDate = DateTime.ParseExact(VM.INVOICEDATE, "dd-MMM-yyyy", null);
            }
            if (!string.IsNullOrEmpty(VM.ENDDATE))
            {
                ReqDateTo = DateTime.ParseExact(VM.ENDDATE + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
            }

            List<SMHeaderViewModel> obj = new List<SMHeaderViewModel>();

         obj = (from data in _SmDBContext.VW_DGIT_SMHEADER.Where(y => y.DATEADDED >= applicabledate && y.IS_IPRELATEDPR == 1)
                              join emp in _SmDBContext.VW_ASSOCIATELVLDETAILS on new { addedby = data.ADDEDBY, SYKIID = data.SYKIID } equals new { addedby = emp.ADEMPCODE, SYKIID = emp.SYKI }
                              join _vendor in _SmDBContext.FINVENDORMASTERMST on data.VENDORCODE equals _vendor.VENDORCODE into _vendorJoin
                              from _vendorData in _vendorJoin.DefaultIfEmpty()
                              join _ed in _SmDBContext.ADEMPLOYEE on data.ADDEDBY equals _ed.ADEMPCODE
                              join _site in _SmDBContext.DGIT_SMPLANTSITEMAP on (data.SYSITEID ?? emp.SYSITEID) equals _site.SITEID
                              join _docRevBy in _SmDBContext.ADEMPLOYEE on (long)data.DOC_REV_BY equals _docRevBy.ADEMPCODE into _docRevByJoin
                              from _docEmp in _docRevByJoin.DefaultIfEmpty()
                              where (string.IsNullOrEmpty(VM.Startdate) || (data.DATEADDED >= ReqDateFrom))
                              && (string.IsNullOrEmpty(VM.ENDDATE) || (data.DATEADDED <= ReqDateTo))
                              && emp.SYKI == VM.KIID
                              && (VM.OperationID == 0 || emp.OPERATIONID == VM.OperationID)
                              && (VM.DivisionID == 0 || emp.DIVISIONID == VM.DivisionID)
                              && (VM.DEPTID == 0 || emp.DEPARTMENTID == VM.DEPTID)
                              && (VM.SECID == 0 || emp.SECTIONID == VM.SECID)
                              && (VM.ecode == 0 || data.ADDEDBY == VM.ecode)
                              && (VM.RequestNumber == 0 || data.SMHEADERID == VM.RequestNumber)
                              && (string.IsNullOrEmpty(VM.INVOICEDATE) || data.INVOICEDATE == InvoiceDate)
                              && (string.IsNullOrEmpty(VM.INVOICENO) || data.INVOICENO == VM.INVOICENO)
                              && (string.IsNullOrEmpty(VM.VendorCode) || data.VENDORCODE == VM.VendorCode)
                              && (string.IsNullOrEmpty(VM.VendorName) || (_vendorData.NAME1.ToUpper() == VM.VendorName.ToUpper()))
                              && (VM.ReqStatus == -1 || (VM.ReqStatus == 0 ? (data.PROCESS_STATUS == 1 || data.PROCESS_STATUS == 2 || data.PROCESS_STATUS == 5 || data.PROCESS_STATUS == 8) : data.PROCESS_STATUS == VM.ReqStatus))
                              select new SMHeaderViewModel
                              {
                                  SMHEADERID = data.SMHEADERID,
                                  PROCESS_STATUS = data.PROCESS_STATUS,
                                  ADDEDBYNAME = _ed.FIRSTNAME + " " + _ed.LASTNAME,
                                  DATEADDED = data.DATEADDED,
                                  SM_NO = data.SES_MRN_NO,
                                  SM_TYPE = data.SM_TYPE,
                                  PONUMBER = data.PONUMBER,
                                  AMOUNT = data.AMOUNT,
                                  STATUS = data.STATUS,
                                  INVOICENO = data.INVOICENO,
                                  INVAMOUNT = data.INVAMOUNT,
                                  INVOICEDATE = data.INVOICEDATE,
                                  PA_GENERATED = data.PA_GENERATED,
                                  VENDORCODE = (_vendorData == null ? "" : _vendorData.VENDORCODE),
                                  VENDORNAME = (_vendorData == null ? "" : _vendorData.NAME1),
                                  Document_Status = (data.DOC_STATUS == null ? 0 : data.DOC_STATUS),
                                  Doc_Rev_Date = data.DOC_REV_DATE,
                                  Doc_Rev_By = data.DOC_REV_BY,
                                  Emp_Detail = new Employee_Details
                                  {
                                      _ECode = _ed.ADEMPCODE,
                                      _EName = _ed.FIRSTNAME + " " + _ed.LASTNAME,
                                      _OpDesc = emp.OPERATION,
                                      _DivDesc = emp.DIVISION,
                                      _DepDesc = emp.DEPARTMENT,
                                      _SecDescrip = emp.SECTION
                                  }
                              }).ToList();

            var headerIds = obj.Select(h => h.SMHEADERID).ToList();

            var smDetails = _SmDBContext.DGIT_SESMRN_DTL
                .Where(d => headerIds.Contains(d.SMHEADERID) &&
                            (d.DOC_TYPE == "SMA" || d.DOC_TYPE == "SMFA" || d.DOC_TYPE == "SM") &&
                            d.STATUS == 1)
                .Select(d => new SMDetailViewModel
                {
                    SMDTL_ID = d.SMDTL_ID,
                    SMHEADERID = d.SMHEADERID,
                    DOC_TYPE = d.DOC_TYPE,
                    ADDITIONAL_INFO = d.ADDITIONAL_INFO,
                    FILENAME = d.FILENAME,
                    SMNo = d.SMMO
                }).ToList();

            foreach (var header in obj)
            {
                header.smDetail = smDetails
                    .Where(d => d.SMHEADERID == header.SMHEADERID)
                    .OrderBy(d => d.SMDTL_ID)
                    .ToList();
            }



            if (!string.IsNullOrEmpty(VM.SMNo))
            {
                VM.SearchResult = (from data in obj
                                   where (data.INVOICENO != "0" && data.smDetail.Where(x => x.SMNo == VM.SMNo).Count() > 0)
                                      || (data.INVOICENO == "0" && data.SM_NO == VM.SMNo)
                                   select data
                  ).ToList();

            }
            else
            {
                VM.SearchResult = obj.OrderBy(o => o.DATEADDED).ToList();
            }
            return VM;
        }
        //===========================================================

    }
}
