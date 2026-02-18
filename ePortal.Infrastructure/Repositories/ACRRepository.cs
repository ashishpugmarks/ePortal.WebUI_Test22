using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;



namespace ePortal.Infrastructure.Repositories
{
    public class ACRRepository
    {
        private EPortalDBContext _AcrDBContext;
        private SYKI _Syki;
        public ACRRepository(EPortalDBContext objEPortalDBContext)
        {
            _AcrDBContext = objEPortalDBContext;
            _Syki = _AcrDBContext.SYKI.Where(x => x.ACTIVE == 1).FirstOrDefault();
        }

        public List<VendorViewModel> GetVendorList()
        {
            var iList = (from data in _AcrDBContext.FINVENDORMASTERMST
                         where data.STATUS == 1
                         select new VendorViewModel
                         {
                             VENDORCODE = data.VENDORCODE,
                             VENDORNAME = data.NAME1,
                             VENDOREMAIL = data.EMAIL1
                         }).OrderBy(o => o.VENDORNAME).ToList();
            return iList;
        }

        public Tuple<short, long> SaveACRRequest(ACRHeaderViewModel model)
        {
            short retVal = 0; long retHeaderId = 0;
            Tuple<short, long> _retVal_tuple;
            using (var transaction = _AcrDBContext.Database.BeginTransaction())
            {
                try
                {
                    DGIT_ACRHEADER DPH = new DGIT_ACRHEADER();
                    int FlagAdd = 0;
                    if (model.ACRHEADERID > 0)
                    {
                        DPH = _AcrDBContext.DGIT_ACRHEADER.Where(x => x.ACRHEADERID == model.ACRHEADERID).SingleOrDefault();
                    }
                    else
                    {
                        if (_AcrDBContext.DGIT_ACRHEADER.Where(x => x.ACRHEADERID != model.ACRHEADERID && x.ACRNO == model.ACRNO && (x.PROCESS_STATUS == 1 || x.PROCESS_STATUS == 0)).FirstOrDefault() != null)
                        {
                            retVal = 2;
                            return _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId); //// -- record already exist.
                        }

                        DPH = new DGIT_ACRHEADER();
                        if (_AcrDBContext.DGIT_ACRHEADER.FirstOrDefault() == null)
                        {
                            DPH.ACRHEADERID = 1;
                        }
                        else
                        {
                            DPH.ACRHEADERID = _AcrDBContext.DGIT_ACRHEADER.Max(x => x.ACRHEADERID) + 1;
                        }
                        FlagAdd = 1;
                    }
                    if (model.IsFinalSubmit == 0)
                    {
                        DPH.ACRNO = model.ACRNO;
                        DPH.PO_NUMBER = model.PONO;
                        DPH.AMOUNT = model.AMOUNT;
                        DPH.SUPPLIER_CODE = model.SUPPLIER_CODE;
                        DPH.SUPPLIER_NAME = model.SUPPLIER_NAME;
                        DPH.REMARK = model.REMARK;
                        DPH.ACR_DESC = model.ACR_DESC;
                        //Addded by aumento for new Payment processing site----------
                        DPH.SYSITEID = model.SYSITEID;
                        //-------------------End Payment processing site-------------
                    }
                    else
                    {
                        if (_AcrDBContext.DGIT_ACRDETAIL.Where(x => x.ACRHEADERID == model.ACRHEADERID && x.DOC_TYPE == "ACR").FirstOrDefault() == null)
                        {
                            retVal = 3;
                            return _retVal_tuple = new Tuple<short, long>(retVal, retHeaderId); //// -- PO file not exist.
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
                    _AcrDBContext.Entry(DPH).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _AcrDBContext.SaveChanges();

                    /// --- Save PO Detail --- ///
                    if (model.acrDetail != null)
                    {
                        if (model.acrDetail.Count > 0)
                        {
                            SaveACRDetails(model.ADDEDBY, DPH.ACRHEADERID, model.acrDetail);
                        }
                    }

                    ///// --- Save PR Mapping Dtl --- ///
                    //if (model.poMapping != null)
                    //{
                    //    if (model.poMapping.Count > 0)
                    //    {
                    //        SavePOMapping(model.ADDEDBY, DPH.ACRHEADERID, model.poMapping);
                    //    }
                    //}

                    /// --- Save PO Approval Auth Seq ---///
                    if (model.acrAuthSeq != null)
                    {
                        if (model.acrAuthSeq.Count > 0)
                        {
                            SaveAppAuthSeq(model.ADDEDBY, DPH.ACRHEADERID, model.acrAuthSeq);

                            /// --- Save ACR Approval Authority --- ///
                            //MBP

                            //========================Change Done on 27082022 For Add Other Category by (Aumento)=================================
                            ACRAppAuthSeqViewModel seqModel = model.acrAuthSeq.OrderBy(o => o.APP_SEQ).FirstOrDefault();
                            //ACRAppAuthSeqViewModel seqModel = model.acrAuthSeq.FirstOrDefault();
                            //List<ACRAppAuthSeqViewModel> seqModel = new List<ACRAppAuthSeqViewModel>();

                            //ACRAppAuthSeqViewModel seqModel = model.acrAuthSeq.OrderBy(o => o.ADEMPCODE).FirstOrDefault();
                            //seqModel = model.acrAuthSeq.OrderBy(o => o.ADEMPCODE).Take(2).ToList();

                            if (seqModel != null)
                            {
                                SaveACRAppHis(model.ADDEDBY, DPH.ACRHEADERID, seqModel);
                                //SaveACRAppHisNew(model.ADDEDBY, DPH.ACRHEADERID, seqModel);
                            }
                            //===========================================================================================================================
                        }
                    }

                    if (model.skipAuthList != null)
                    {
                        if (model.skipAuthList.Count > 0)
                        {
                            SaveSkipAuth(model.ADDEDBY, DPH.ACRHEADERID, model.skipAuthList);
                        }
                    }

                    transaction.Commit();
                    retVal = 1;
                    retHeaderId = DPH.ACRHEADERID;
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

        public short SaveACRDetails(long AddedBy, long ACRHEADERID, List<ACRDetailViewModel> PDVMList)
        {
            short retVal = 0;
            foreach (ACRDetailViewModel PDVM in PDVMList)
            {
                DGIT_ACRDETAIL DPD = new DGIT_ACRDETAIL();
                int FlagAdd = 0;
                if (ACRHEADERID > 0)
                {
                    //DPD = _AcrDBContext.DGIT_ACRDETAIL.Where(x => x.ACRHEADERID == ACRHEADERID && x.DOC_TYPE == PDVM.DOC_TYPE && x.FILENAME == PDVM.FILENAME).SingleOrDefault();
                    //if (DPD != null)
                    //{
                    //    _AcrDBContext.DGIT_ACRDETAIL.Remove(DPD);
                    //    _AcrDBContext.SaveChanges();
                    //}
                    if (PDVM.DOC_TYPE == "ACR")
                    {
                        DPD = _AcrDBContext.DGIT_ACRDETAIL.Where(x => x.ACRHEADERID == ACRHEADERID && x.DOC_TYPE == PDVM.DOC_TYPE).FirstOrDefault();
                        if (DPD != null)
                        {
                            _AcrDBContext.DGIT_ACRDETAIL.Remove(DPD);
                            _AcrDBContext.SaveChanges();
                        }
                    }

                    DPD = new DGIT_ACRDETAIL();
                    if (_AcrDBContext.DGIT_ACRDETAIL.FirstOrDefault() == null)
                    {
                        DPD.ACRDTL_ID = 1;
                    }
                    else
                    {
                        DPD.ACRDTL_ID = _AcrDBContext.DGIT_ACRDETAIL.Max(x => x.ACRDTL_ID) + 1;
                    }
                    FlagAdd = 1;

                    DPD.ACRHEADERID = ACRHEADERID;
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
                    _AcrDBContext.Entry(DPD).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _AcrDBContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }

        //public void SavePOMapping(long AddedBy, long ACRHEADERID, List<POMappingViewModel> mappingList)
        //{
        //    //string[] strArray = _prNos[0].Split(',');
        //    List<DGIT_POPRLINK_DTL> prList = _AcrDBContext.DGIT_POPRLINK_DTL.Where(x => x.ACRHEADERID == ACRHEADERID).ToList();
        //    if (prList.Count > 0)
        //    {
        //        _AcrDBContext.DGIT_POPRLINK_DTL.RemoveRange(prList);
        //        _AcrDBContext.SaveChanges();
        //    }
        //    foreach (POMappingViewModel obj in mappingList)
        //    {
        //        DGIT_POPRLINK_DTL DPLD = new DGIT_POPRLINK_DTL();
        //        int FlagAdd = 0;
        //        if (!string.IsNullOrEmpty(obj.PRNO))
        //        {
        //            DPLD = _AcrDBContext.DGIT_POPRLINK_DTL.Where(x => x.PRNO == obj.PRNO && x.ACRHEADERID == ACRHEADERID).SingleOrDefault();
        //            if (DPLD == null)
        //            {
        //                DPLD = new DGIT_POPRLINK_DTL();
        //                if (_AcrDBContext.DGIT_POPRLINK_DTL.ToList().Count == 0)
        //                {
        //                    DPLD.POPRLNK_ID = 1;
        //                }
        //                else
        //                {
        //                    DPLD.POPRLNK_ID = _AcrDBContext.DGIT_POPRLINK_DTL.Max(x => x.POPRLNK_ID) + 1;
        //                }
        //                FlagAdd = 1;
        //            }
        //            DPLD.ACRHEADERID = ACRHEADERID;
        //            DPLD.PRNO = obj.PRNO;
        //            DPLD.STATUS = 1;
        //            if (FlagAdd == 1)
        //            {
        //                DPLD.ADDEDBY = AddedBy;
        //                DPLD.ADDEDDATE = DateTime.Now;
        //            }
        //            else
        //            {
        //                DPLD.UPDATEDBY = AddedBy;
        //                DPLD.UPDATEDATE = DateTime.Now;
        //            }
        //            _AcrDBContext.Entry(DPLD).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
        //            _AcrDBContext.SaveChanges();
        //        }
        //    }
        //}

        public void SaveAppAuthSeq(long AddedBy, long ACRHEADERID, List<ACRAppAuthSeqViewModel> PSVMList)
        {
            //// --- Delete recode ---////
            try
            {
                List<DGIT_ACRAPPAUTHSEQ> SeqList = _AcrDBContext.DGIT_ACRAPPAUTHSEQ.Where(t => t.ACRID == ACRHEADERID).ToList();
                if (SeqList.Count > 0)
                {
                    _AcrDBContext.DGIT_ACRAPPAUTHSEQ.RemoveRange(SeqList);
                    _AcrDBContext.SaveChanges();
                }
                //Change on 23092022 by (Aumento)============================================================
                //foreach (ACRAppAuthSeqViewModel PSVM in PSVMList.OrderBy(o => o.APP_SEQ).ToList())
                foreach (ACRAppAuthSeqViewModel PSVM in PSVMList.ToList())
                //=========================================================================================
                {
                    DGIT_ACRAPPAUTHSEQ DAAS = new DGIT_ACRAPPAUTHSEQ();
                    if (_AcrDBContext.DGIT_ACRAPPAUTHSEQ.FirstOrDefault() == null)
                    {
                        DAAS.ACRAPPAUTH_ID = 1;
                    }
                    else
                    {
                        DAAS.ACRAPPAUTH_ID = _AcrDBContext.DGIT_ACRAPPAUTHSEQ.Max(x => x.ACRAPPAUTH_ID) + 1;
                    }
                    DAAS.ACRID = ACRHEADERID;
                    DAAS.ADEMPCODE = PSVM.ADEMPCODE;
                    DAAS.APP_SEQ = PSVM.APP_SEQ;
                    DAAS.APPTYPE = PSVM.APPTYPE;
                    DAAS.STATUS = 1;
                    DAAS.ADDEDBY = AddedBy;
                    DAAS.ADDEDDATE = DateTime.Now;
                    _AcrDBContext.Entry(DAAS).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                    _AcrDBContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //========================New added below for List Record Done on 27082022 For Add Other Category by (Aumeto)================================================================
        public void SaveACRAppHisNew(long AddedBy, long ACRHEADERID, List<ACRAppAuthSeqViewModel> PSVMList)
        {
            try
            {
                DGIT_ACRAPPHISTORY DPAH = new DGIT_ACRAPPHISTORY();
                int FlagAdd = 0;
                // DPAH = _AcrDBContext.DGIT_ACRAPPHISTORY.Where(d => d.ADEMPCODE == PSVM.ADEMPCODE && d.ACRID == ACRHEADERID).FirstOrDefault();
                foreach (ACRAppAuthSeqViewModel PSVM in PSVMList.OrderBy(o => o.APP_SEQ).ToList())
                {
                    DPAH = _AcrDBContext.DGIT_ACRAPPHISTORY.Where(d => d.ADEMPCODE == PSVM.ADEMPCODE && d.ACRID == ACRHEADERID && d.APPROVAL_STATUS == 0).FirstOrDefault();
                    if (DPAH == null)
                    {
                        DPAH = new DGIT_ACRAPPHISTORY();
                        if (_AcrDBContext.DGIT_ACRAPPHISTORY.FirstOrDefault() == null)
                        {
                            DPAH.ACRAPPHISTORY_ID = 1;
                        }
                        else
                        {
                            DPAH.ACRAPPHISTORY_ID = _AcrDBContext.DGIT_ACRAPPHISTORY.Max(x => x.ACRAPPHISTORY_ID) + 1;
                        }
                        FlagAdd = 1;
                    }
                    DPAH.ACRID = ACRHEADERID;
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
                    _AcrDBContext.Entry(DPAH).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _AcrDBContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //======================================================================================================================================================

        public void SaveACRAppHis(long AddedBy, long ACRHEADERID, ACRAppAuthSeqViewModel PSVM)
        {
            try
            {
                DGIT_ACRAPPHISTORY DPAH = new DGIT_ACRAPPHISTORY();
                int FlagAdd = 0;
                // DPAH = _AcrDBContext.DGIT_ACRAPPHISTORY.Where(d => d.ADEMPCODE == PSVM.ADEMPCODE && d.ACRID == ACRHEADERID).FirstOrDefault();
                DPAH = _AcrDBContext.DGIT_ACRAPPHISTORY.Where(d => d.ADEMPCODE == PSVM.ADEMPCODE && d.ACRID == ACRHEADERID && d.APPROVAL_STATUS == 0).FirstOrDefault();
                if (DPAH == null)
                {
                    DPAH = new DGIT_ACRAPPHISTORY();
                    if (_AcrDBContext.DGIT_ACRAPPHISTORY.FirstOrDefault() == null)
                    {
                        DPAH.ACRAPPHISTORY_ID = 1;
                    }
                    else
                    {
                        DPAH.ACRAPPHISTORY_ID = _AcrDBContext.DGIT_ACRAPPHISTORY.Max(x => x.ACRAPPHISTORY_ID) + 1;
                    }
                    FlagAdd = 1;
                }
                DPAH.ACRID = ACRHEADERID;
                DPAH.ADEMPCODE = PSVM.ADEMPCODE;
                DPAH.APPROVAL_STATUS = 0;
                DPAH.APPROVAL_REMARK = "";
                //Added By Aumento as on 31072024 for SR76166====================================
                DPAH.APP_SEQ = PSVM.APP_SEQ;
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
                _AcrDBContext.Entry(DPAH).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                _AcrDBContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SaveSkipAuth(long AddedBy, long ACRHEADERID, List<ACRAuthSkipViewModel> PASList)
        {
            //// --- Delete recode ---////
            List<DGIT_ACRAPPSKIP> skipList = _AcrDBContext.DGIT_ACRAPPSKIP.Where(t => t.ACRID == ACRHEADERID).ToList();
            if (skipList.Count > 0)
            {
                _AcrDBContext.DGIT_ACRAPPSKIP.RemoveRange(skipList);
                _AcrDBContext.SaveChanges();
            }

            foreach (ACRAuthSkipViewModel PSVM in PASList)
            {
                DGIT_ACRAPPSKIP DPAS = new DGIT_ACRAPPSKIP();
                if (_AcrDBContext.DGIT_ACRAPPSKIP.FirstOrDefault() == null)
                {
                    DPAS.ACRAPPSKIP_ID = 1;
                }
                else
                {
                    DPAS.ACRAPPSKIP_ID = _AcrDBContext.DGIT_ACRAPPSKIP.Max(x => x.ACRAPPSKIP_ID) + 1;
                }
                DPAS.ACRID = ACRHEADERID;
                DPAS.ADEMPCODE = PSVM.ADEMPCODE;
                DPAS.SKIPREMARK = PSVM.SKIPREMARK;
                DPAS.STATUS = 1;
                DPAS.ADDEDBY = AddedBy;
                DPAS.ADDEDDATE = DateTime.Now;
                _AcrDBContext.Entry(DPAS).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                _AcrDBContext.SaveChanges();
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
            var _obj = (from data in _AcrDBContext.ADEMPLOYEE.Where(v => v.ADEMPCODE == empCode && v.ACTIVE == 1)
                        join _VW in _AcrDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VW.ADEMPCODE
                        join _Desg in _AcrDBContext.ADDESIGNATION on _VW.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
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

        //============Change Done on 27082022 For Add Other Category by (Aumento)==========================================================================================
        public Employee_Details GetOAuthEmpById(long empCode, int designationId, string designation, Employee_Details empDtl)
        {
            long? _FnDesigId = 0;
            long _opId = Convert.ToInt64(empDtl._OpId != null ? empDtl._OpId : 0);
            //int[] Desig_Array = { 1, 2, 4, 6, 8 };
            //if (Desig_Array.Contains(designationId))
            //{
            //    if (designationId == 1)
            //        _FnDesigId = 1;

            //    if (designationId == 2)
            //        _FnDesigId = 2;

            //    if (designationId == 4)
            //        _FnDesigId = 3;

            //    if (designationId == 6)
            //        _FnDesigId = 4;

            //    if (designationId == 8)
            //    {
            //        _FnDesigId = 4;
            //        _opId = 0;
            //    }
            //}
            //Change Done on 20092022 for get employee number as per Designation selection by (Aumento)========================================
            int desg = 0;

            int fundesg = 0;

            if (designationId == 1)
                fundesg = 1;//section Manager

            if (designationId == 2)
                fundesg = 2;//Department Manager

            if (designationId == 3)
                desg = 1;

            if (designationId == 4)
                fundesg = 3;//Division Head

            if (designationId == 5)
                desg = 22;

            if (designationId == 6)
                fundesg = 4;//Operating Head

            if (designationId == 7)
                desg = 30;

            if (designationId == 9)
                desg = 27;

            //if (designationId == "10")
            //    desg = 27;


            if (designationId == 11)
                desg = 31;

            //============================================================================================================================

            Employee_Details empdata_ans = new Employee_Details();

            if (desg > 0)
            {
                var _obj = (from data in _AcrDBContext.ADEMPLOYEE.Where(v => v.ADEMPCODE == empCode && v.ACTIVE == 1)
                            join _VW in _AcrDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VW.ADEMPCODE
                            join _Desg in _AcrDBContext.ADDESIGNATION on _VW.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
                            where _VW.SYKI == _Syki.SYKIID && _VW.ADDESIGNATIONID == desg
                            //&& (Desig_Array.Contains(designationId) ? _VW.ADFUNCTIONALDESIGNATIONID == _FnDesigId
                            //&& (_opId > 0 ? _VW.OPERATIONID == _opId : 1 == 1) : (designation == "Director 2" ? (_Desg.DESCRIP == "Director" || _Desg.DESCRIP == "Senior Director") : _Desg.DESCRIP == designation))
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
                //return _obj;
                empdata_ans = _obj;
            }
            else if (fundesg > 0)
            {
                var _obj_Desg = (from data in _AcrDBContext.ADEMPLOYEE.Where(v => v.ADEMPCODE == empCode && v.ACTIVE == 1)
                                 join _VW in _AcrDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VW.ADEMPCODE
                                 //change done for other authority by (Aumento)=================================================================================================
                                 //join _Desg in _AcrDBContext.ADDESIGNATION on _VW.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
                                 join _Desg_Fndesg in _AcrDBContext.ADFUNCTIONALDESIGNATION on _VW.ADFUNCTIONALDESIGNATIONID equals _Desg_Fndesg.ADFUNCTIONALDESIGNATIONID
                                 where _VW.SYKI == _Syki.SYKIID && _VW.ADFUNCTIONALDESIGNATIONID == fundesg
                                 //=============================================================================================================================================
                                 //&& (Desig_Array.Contains(designationId) ? _VW.ADFUNCTIONALDESIGNATIONID == _FnDesigId
                                 //&& (_opId > 0 ? _VW.OPERATIONID == _opId : 1 == 1) : (designation == "Director 2" ? (_Desg.DESCRIP == "Director" || _Desg.DESCRIP == "Senior Director") : _Desg.DESCRIP == designation))
                                 select new Employee_Details
                                 {
                                     _ECode = data.ADEMPCODE,
                                     _EFirstName = data.FIRSTNAME,
                                     _ELastName = data.LASTNAME,
                                     _EName = data.FIRSTNAME + " " + data.LASTNAME,
                                     _EmailId = data.EMAILID,
                                     _DesigId = _VW.ADDESIGNATIONID,
                                     //_Desig = _Desg.DESCRIP,
                                     _Desig = _Desg_Fndesg.DESCRIP,
                                     _FnDesigId = _VW.ADFUNCTIONALDESIGNATIONID,
                                 }).FirstOrDefault();
                empdata_ans = _obj_Desg;
            }
            return empdata_ans;

        }
        //public Employee_Details GetOAuthEmpById(long empCode, int designationId, string designation, Employee_Details empDtl)
        //{
        //    long? _FnDesigId = 0;

        //    var _obj = (from data in _AcrDBContext.ADEMPLOYEE.Where(v => v.ADEMPCODE == empCode && v.ACTIVE == 1)
        //                join _VW in _AcrDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VW.ADEMPCODE
        //                join _Desg in _AcrDBContext.ADDESIGNATION on _VW.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
        //                where _VW.SYKI == _Syki.SYKIID
        //                //&& (Desig_Array.Contains(designationId) ? _VW.ADFUNCTIONALDESIGNATIONID == _FnDesigId
        //                //&& (_opId > 0 ? _VW.OPERATIONID == _opId : 1 == 1) : (designation == "Director 2" ? (_Desg.DESCRIP == "Director" || _Desg.DESCRIP == "Senior Director") : _Desg.DESCRIP == designation))
        //                select new Employee_Details
        //                {
        //                    _ECode = data.ADEMPCODE,
        //                    _EFirstName = data.FIRSTNAME,
        //                    _ELastName = data.LASTNAME,
        //                    _EName = data.FIRSTNAME + " " + data.LASTNAME,
        //                    _EmailId = data.EMAILID,
        //                    _DesigId = _VW.ADDESIGNATIONID,
        //                    _Desig = _Desg.DESCRIP,
        //                    _FnDesigId = _VW.ADFUNCTIONALDESIGNATIONID,
        //                }).FirstOrDefault();
        //    return _obj;
        //}

        //==================================================================================================================================================================================



        //public Employee_Details GetAuthEmpById(long empCode)
        //{
        //    var _obj = (from data in _AcrDBContext.ADEMPLOYEE.Where(v => v.ADEMPCODE == empCode && v.ACTIVE == 1)
        //                join _VW in _AcrDBContext.VW_ASSOCIATELVLDETAILS on data.ADEMPCODE equals _VW.ADEMPCODE
        //                join _Desg in _AcrDBContext.ADDESIGNATION on _VW.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
        //                where _VW.SYKI == _Syki.SYKIID && _VW.FUNCTIONALDESIGNATION != null
        //                select new Employee_Details
        //                {
        //                    _ECode = data.ADEMPCODE,
        //                    _EFirstName = data.FIRSTNAME,
        //                    _ELastName = data.LASTNAME,
        //                    _EName = data.FIRSTNAME + " " + data.LASTNAME,
        //                    _EmailId = data.EMAILID,
        //                    _DesigId = _VW.ADDESIGNATIONID,
        //                    _Desig = _Desg.DESCRIP,
        //                    _FnDesigId = _VW.ADFUNCTIONALDESIGNATIONID
        //                }).FirstOrDefault();
        //    return _obj;
        //}

        //public ACRHeaderViewModel GetACRRequestById(long id)
        //{
        //    var _obj = (from data in _AcrDBContext.DGIT_ACRHEADER.Where(x => x.ACRHEADERID == id)
        //                join _AddBy in _AcrDBContext.ADEMPLOYEE on data.ADDEDBY equals _AddBy.ADEMPCODE
        //                join _VWAssociate in _AcrDBContext.VW_ASSOCIATELVLDETAILS on data.ADDEDBY equals _VWAssociate.ADEMPCODE
        //                //Addded by aumento for new Payment processing site----------
        //                join _SKI in _AcrDBContext.SYSITE on data.SYSITEID equals _SKI.SYSITEID                         
        //                //-------------------End Payment processing site-------------
        //                where _VWAssociate.SYKI == _Syki.SYKIID && _SKI.ACTIVE==1
        //                select new ACRHeaderViewModel
        //                {
        //                    ACRHEADERID = data.ACRHEADERID,
        //                    ACRNO = data.ACRNO,
        //                    AMOUNT = data.AMOUNT,
        //                    PONO = data.PO_NUMBER,
        //                    SUPPLIER_CODE = data.SUPPLIER_CODE,
        //                    SUPPLIER_NAME = data.SUPPLIER_NAME,
        //                    ACR_DESC = data.ACR_DESC,
        //                    REMARK = data.REMARK,
        //                    ADDEDBYNAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
        //                    DATEADDED = data.DATEADDED,
        //                    //Addded by aumento for new Payment processing site----------
        //                    SYSITEDESC = _SKI.DESCRIP + " Finance",
        //                    SYSITEID = data.SYSITEID,
        //                    //-------------------End Payment processing site-------------
        //                    acrDetail = (from _PODetail in _AcrDBContext.DGIT_ACRDETAIL.Where(d => d.ACRHEADERID == data.ACRHEADERID)
        //                                 where _PODetail.STATUS == 1
        //                                 select new ACRDetailViewModel
        //                                 {
        //                                     ACRDTL_ID = _PODetail.ACRDTL_ID,
        //                                     ACRHEADERID = _PODetail.ACRHEADERID,
        //                                     DOC_TYPE = _PODetail.DOC_TYPE,
        //                                     ADDITIONAL_INFO = _PODetail.ADDITIONAL_INFO,
        //                                     FILENAME = _PODetail.FILENAME,
        //                                 }).ToList(),
        //                    //poMapping = (from _POMapp in _AcrDBContext.DGIT_POPRLINK_DTL.Where(l => l.ACRHEADERID == data.ACRHEADERID)
        //                    //             where _POMapp.STATUS == 1
        //                    //             select new POMappingViewModel
        //                    //             {
        //                    //                 POPRLNK_ID = _POMapp.POPRLNK_ID,
        //                    //                 ACRHEADERID = _POMapp.ACRHEADERID,
        //                    //                 PRNO = _POMapp.PRNO,
        //                    //             }).ToList(),
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
        //                    acrAuthSeq = (from _POSeq in _AcrDBContext.DGIT_ACRAPPAUTHSEQ.Where(x => x.ACRID == data.ACRHEADERID)
        //                                  join _AppSeqEmp in _AcrDBContext.ADEMPLOYEE on _POSeq.ADEMPCODE equals _AppSeqEmp.ADEMPCODE
        //                                  join _Vw in _AcrDBContext.VW_ASSOCIATELVLDETAILS on _POSeq.ADEMPCODE equals _Vw.ADEMPCODE
        //                                  join _Desg in _AcrDBContext.ADDESIGNATION on _Vw.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
        //                                  where _Vw.SYKI == _Syki.SYKIID
        //                                  select new ACRAppAuthSeqViewModel
        //                                  {
        //                                      ACRAPPAUTH_ID = _POSeq.ACRAPPAUTH_ID,
        //                                      ACRID = _POSeq.ACRID,
        //                                      ADEMPCODE = _POSeq.ADEMPCODE,
        //                                      STATUS = _POSeq.STATUS,
        //                                      APP_SEQ = _POSeq.APP_SEQ,
        //                                      ADEMPNAME = _AppSeqEmp.FIRSTNAME + " " + _AppSeqEmp.LASTNAME,
        //                                      ADDESIGNATION = _Desg.DESCRIP,
        //                                      ADDEDDATE = _POSeq.ADDEDDATE,
        //                                      UPDATEBY = _POSeq.UPDATEBY,
        //                                      UPDATEDATE = _POSeq.UPDATEDATE,
        //                                      APPTYPE = _POSeq.APPTYPE,
        //                   //Change done as on 23092022 By (Aumento)======================================================
        //                                      //}).ToList(),
        //                                  }).OrderBy(o => o.APP_SEQ).ToList(),
        //                    //}).OrderBy(b => b.APP_SEQ).ToList(),
        //                    //=================================================================================================


        //                    acrAppHis = (from _POAppHis in _AcrDBContext.DGIT_ACRAPPHISTORY.Where(x => x.ACRID == data.ACRHEADERID)
        //                                 join _AppEmp in _AcrDBContext.ADEMPLOYEE on _POAppHis.ADEMPCODE equals _AppEmp.ADEMPCODE
        //                                 //Added By Aumento as on 20022024 start
        //                                 join _AppSeq in _AcrDBContext.DGIT_ACRAPPAUTHSEQ on _POAppHis.ACRID equals _AppSeq.ACRID 
        //                                 where _AppSeq.ADEMPCODE == _POAppHis.ADEMPCODE
        //                                 //Added By Aumento as on 31072024 for SR76166====================================
        //                                 && _AppSeq.APP_SEQ == _POAppHis.APP_SEQ
        //                                 //===============================================================================  

        //                                 //Added By Aumento as on 20022024 end
        //                                 select new ACRAppHistoryViewModel
        //                                 {
        //                                     ACRAPPHISTORY_ID = _POAppHis.ACRAPPHISTORY_ID,
        //                                     ACRID = _POAppHis.ACRID,
        //                                     ADEMPCODE = _POAppHis.ADEMPCODE,
        //                                     APPROVAL_STATUS = _POAppHis.APPROVAL_STATUS,
        //                                     APPROVAL_REMARK = _POAppHis.APPROVAL_REMARK,
        //                                     APPEMP_NAME = _AppEmp.FIRSTNAME + " " + _AppEmp.LASTNAME + " - [" + _AppEmp.ADEMPCODE + "]",
        //                                     APP_EMAIL = _AppEmp.EMAILID,
        //                                     APPEMP_CODE = _AppEmp.ADEMPCODE.ToString(),
        //                                     ADDEDDATE = _POAppHis.ADDEDDATE,
        //                                     UPDATEBY = _POAppHis.UPDATEBY,
        //                                     UPDATEDATE = _POAppHis.UPDATEDATE,
        //                                     APPROVALDATE = _POAppHis.APP_DATE,
        //                                     //Below added on 13092022 for Login emp name by (Aumento)=====================================================================================
        //                                     ADDEDBY = _POAppHis.ADDEDBY,
        //                                     ADDEDBYName = _AcrDBContext.ADEMPLOYEE.Where(a => a.ADEMPCODE == _POAppHis.ADDEDBY).Select(a => a.FIRSTNAME + " " + a.LASTNAME).FirstOrDefault()
        //                                     //================================================================================================================================================
        //                                     ,APPTYPE = _AppSeq.APPTYPE //Added By Aumento as on 20022024 
        //                                     //Added By Aumento as on 31072024 for SR76166====================================
        //                                     ,APP_SEQ = _AppSeq.APP_SEQ 
        //                                     //===============================================================================  
        //                                 }).OrderBy(o => o.ACRAPPHISTORY_ID).ToList(),






        //                    skipAuthList = (from _AuthSkip in _AcrDBContext.DGIT_ACRAPPSKIP.Where(x => x.ACRID == data.ACRHEADERID)
        //                                    join _AppSkipEmp in _AcrDBContext.ADEMPLOYEE on _AuthSkip.ADEMPCODE equals _AppSkipEmp.ADEMPCODE
        //                                    select new ACRAuthSkipViewModel
        //                                    {
        //                                        ACRAPPSKIP_ID = _AuthSkip.ACRAPPSKIP_ID,
        //                                        ACRID = _AuthSkip.ACRID,
        //                                        ADEMPCODE = _AuthSkip.ADEMPCODE,
        //                                        STATUS = _AuthSkip.STATUS,
        //                                        ADEMPNAME = _AppSkipEmp.FIRSTNAME + " " + _AppSkipEmp.LASTNAME,
        //                                        ADDEDDATE = _AuthSkip.ADDEDDATE,
        //                                        SKIPREMARK = _AuthSkip.SKIPREMARK
        //                                    }).ToList(),
        //                }).FirstOrDefault();

        //    if (_obj.acrAuthSeq.Count > 0)
        //    {
        //        long lastSendBackAppHisId = 0;
        //        DGIT_ACRAPPHISTORY lastSendBackAppHis = _AcrDBContext.DGIT_ACRAPPHISTORY.Where(e => e.ACRID == _obj.ACRHEADERID && e.APPROVAL_STATUS == 2).OrderByDescending(o => o.ACRAPPHISTORY_ID).FirstOrDefault();
        //        if (lastSendBackAppHis != null)
        //        {
        //            lastSendBackAppHisId = lastSendBackAppHis.ACRAPPHISTORY_ID;
        //        }
        //        //int i = 0;//SM
        //        foreach (ACRAppAuthSeqViewModel obj in _obj.acrAuthSeq)
        //        {
        //            bool IsBeforeSendBackRecord = _AcrDBContext.DGIT_ACRAPPHISTORY.Any(w => w.ACRID == obj.ACRID && w.ACRAPPHISTORY_ID <= lastSendBackAppHisId && w.ADEMPCODE == obj.ADEMPCODE);
        //            //Added By Aumento as on 31072024 for SR76166========================================================================================
        //            //if ((IsBeforeSendBackRecord ? (!_obj.acrAppHis.Any(r => r.ACRID == obj.ACRID && r.ADEMPCODE == obj.ADEMPCODE && r.ACRAPPHISTORY_ID > lastSendBackAppHisId)) : (!_obj.acrAppHis.Any(x => x.ACRID == obj.ACRID && x.ADEMPCODE == obj.ADEMPCODE))))
        //            if ((IsBeforeSendBackRecord ? (!_obj.acrAppHis.Any(r => r.ACRID == obj.ACRID && r.ADEMPCODE == obj.ADEMPCODE && r.ACRAPPHISTORY_ID > lastSendBackAppHisId)) : (!_obj.acrAppHis.Any(x => x.ACRID == obj.ACRID && x.ADEMPCODE == obj.ADEMPCODE && x.APP_SEQ==obj.APP_SEQ))))
        //            //====================================================================================================================================
        //            {
        //                // i = i + 1;//SM
        //                _obj.acrAppHis.Add(new ACRAppHistoryViewModel
        //                {                            
        //                    ACRAPPHISTORY_ID = 0,
        //                    //ACRAPPHISTORY_ID = i,//SM
        //                    ACRID = obj.ACRID,
        //                    ADEMPCODE = obj.ADEMPCODE,
        //                    APPROVAL_STATUS = 0,
        //                    APPROVAL_REMARK = "",
        //                    APPEMP_NAME = obj.ADEMPNAME + "[" + obj.ADEMPCODE + "]",
        //                    //Added By Aumento as on 31072024 for SR76166====================================
        //                    APP_SEQ = obj.APP_SEQ 
        //                    //===============================================================================
        //                });
        //            }
        //        }
        //    }
        //    //Added By Aumento as on 31072024 for SR76166========================================================================================
        //    if (_obj != null)   
        //    {
        //        // _obj.acrAppHis = _obj.acrAppHis.AsEnumerable().OrderBy(r => r.APP_SEQ).ToList();
        //        _obj.acrAppHis = _obj.acrAppHis.AsEnumerable().OrderBy(r => r.APPROVALDATE.HasValue ? r.APPROVALDATE.Value : DateTime.MaxValue)
        //                                                      .ThenBy(r => r.APP_SEQ)
        //                                                      .ToList();
        //    }
        //    //====================================================================================================================================
        //    return _obj;
        //}
        public ACRHeaderViewModel GetACRRequestById(long id)
        {
            var headerData = (from h in _AcrDBContext.DGIT_ACRHEADER
                              where h.ACRHEADERID == id
                              join emp in _AcrDBContext.ADEMPLOYEE on h.ADDEDBY equals emp.ADEMPCODE into empJoin
                              from emp in empJoin.DefaultIfEmpty()
                              join vw in _AcrDBContext.VW_ASSOCIATELVLDETAILS on h.ADDEDBY equals vw.ADEMPCODE into vwJoin
                              from vw in vwJoin.DefaultIfEmpty()
                              join site in _AcrDBContext.SYSITE on h.SYSITEID equals site.SYSITEID into siteJoin
                              from site in siteJoin.DefaultIfEmpty()
                              where vw.ADEMPCODE != null && vw.SYKI == _Syki.SYKIID && site != null && site.ACTIVE == 1
                              select new
                              {
                                  Header = h,
                                  Employee = emp,
                                  VW = vw,
                                  Site = site
                              }).FirstOrDefault();



            var acrDetail = (from _PODetail in _AcrDBContext.DGIT_ACRDETAIL.Where(d => d.ACRHEADERID == headerData.Header.ACRHEADERID)
                             where _PODetail.STATUS == 1
                             select new ACRDetailViewModel
                             {
                                 ACRDTL_ID = _PODetail.ACRDTL_ID,
                                 ACRHEADERID = _PODetail.ACRHEADERID,
                                 DOC_TYPE = _PODetail.DOC_TYPE,
                                 ADDITIONAL_INFO = _PODetail.ADDITIONAL_INFO,
                                 FILENAME = _PODetail.FILENAME,
                             }).ToList();

            var acrAuthSeq = (from seq in _AcrDBContext.DGIT_ACRAPPAUTHSEQ
                              join emp in _AcrDBContext.ADEMPLOYEE on seq.ADEMPCODE equals emp.ADEMPCODE
                              join vw in _AcrDBContext.VW_ASSOCIATELVLDETAILS on seq.ADEMPCODE equals vw.ADEMPCODE
                              join desg in _AcrDBContext.ADDESIGNATION on vw.ADDESIGNATIONID equals desg.ADDESIGNATIONID
                              where seq.ACRID == headerData.Header.ACRHEADERID && vw.SYKI == _Syki.SYKIID
                              orderby seq.APP_SEQ
                              select new ACRAppAuthSeqViewModel
                              {
                                  ACRAPPAUTH_ID = seq.ACRAPPAUTH_ID,
                                  ACRID = seq.ACRID,
                                  ADEMPCODE = seq.ADEMPCODE,
                                  STATUS = seq.STATUS,
                                  APP_SEQ = seq.APP_SEQ,
                                  ADEMPNAME = emp.FIRSTNAME + " " + emp.LASTNAME,
                                  ADDESIGNATION = desg.DESCRIP,
                                  ADDEDDATE = seq.ADDEDDATE,
                                  UPDATEBY = seq.UPDATEBY,
                                  UPDATEDATE = seq.UPDATEDATE,
                                  APPTYPE = seq.APPTYPE,
                                  //Change done as on 23092022 By (Aumento)======================================================
                                  //}).ToList(),
                              }).OrderBy(o => o.APP_SEQ).ToList();

            var acrAppHis =


(
 from his in _AcrDBContext.DGIT_ACRAPPHISTORY
 join emp in _AcrDBContext.ADEMPLOYEE
 on his.ADEMPCODE equals emp.ADEMPCODE
 join seq in _AcrDBContext.DGIT_ACRAPPAUTHSEQ
 on new { his.ACRID, his.ADEMPCODE, his.APP_SEQ }
 equals new { seq.ACRID, seq.ADEMPCODE, seq.APP_SEQ }
 where his.ACRID == headerData.Header.ACRHEADERID
 orderby his.ACRAPPHISTORY_ID
 select new ACRAppHistoryViewModel
 {


     ACRAPPHISTORY_ID = his.ACRAPPHISTORY_ID,
     ACRID = his.ACRID,
     ADEMPCODE = his.ADEMPCODE,
     APPROVAL_STATUS = his.APPROVAL_STATUS,
     APPROVAL_REMARK = his.APPROVAL_REMARK,
     APPEMP_NAME = emp.FIRSTNAME + " " + emp.LASTNAME + " - [" + emp.ADEMPCODE + "]",
     APP_EMAIL = emp.EMAILID,
     APPEMP_CODE = emp.ADEMPCODE.ToString(),
     ADDEDDATE = his.ADDEDDATE,
     UPDATEBY = his.UPDATEBY,
     UPDATEDATE = his.UPDATEDATE,
     APPROVALDATE = his.APP_DATE,
     //Below added on 13092022 for Login emp name by (Aumento)=====================================================================================
     ADDEDBY = his.ADDEDBY,
     ADDEDBYName = _AcrDBContext.ADEMPLOYEE.Where(a => a.ADEMPCODE == his.ADDEDBY).Select(a => a.FIRSTNAME + " " + a.LASTNAME).FirstOrDefault()
                                             //================================================================================================================================================
                                             ,
     APPTYPE = seq.APPTYPE //Added By Aumento as on 20022024 
                           //Added By Aumento as on 31072024 for SR76166====================================
                                             ,
     APP_SEQ = seq.APP_SEQ
     //===============================================================================  
 }).OrderBy(o => o.ACRAPPHISTORY_ID).ToList();

            var skipAuthList = (from skip in _AcrDBContext.DGIT_ACRAPPSKIP
                                join emp in _AcrDBContext.ADEMPLOYEE on skip.ADEMPCODE equals emp.ADEMPCODE
                                where skip.ACRID == headerData.Header.ACRHEADERID
                                select new ACRAuthSkipViewModel
                                {

                                    ACRAPPSKIP_ID = skip.ACRAPPSKIP_ID,
                                    ACRID = skip.ACRID,
                                    ADEMPCODE = skip.ADEMPCODE,
                                    STATUS = skip.STATUS,
                                    ADEMPNAME = emp.FIRSTNAME + " " + emp.LASTNAME,
                                    ADDEDDATE = skip.ADDEDDATE,
                                    SKIPREMARK = skip.SKIPREMARK
                                }).ToList();

            var _obj = new ACRHeaderViewModel
            {
                ACRHEADERID = headerData.Header.ACRHEADERID,
                ACRNO = headerData.Header.ACRNO,
                PROCESS_STATUS = headerData.Header.PROCESS_STATUS, //Added by TTL on 05-May-2025 | SR97294 > CR6254 -- ACRFA file was not getting generated due to this was ommitted

                AMOUNT = headerData.Header.AMOUNT,
                PONO = headerData.Header.PO_NUMBER,
                SUPPLIER_CODE = headerData.Header.SUPPLIER_CODE,
                SUPPLIER_NAME = headerData.Header.SUPPLIER_NAME,
                ACR_DESC = headerData.Header.ACR_DESC,
                REMARK = headerData.Header.REMARK,
                ADDEDBYNAME = headerData.Employee.FIRSTNAME + " " + headerData.Employee.LASTNAME,
                DATEADDED = headerData.Header.DATEADDED,
                SYSITEDESC = headerData.Site.DESCRIP + " Finance",
                SYSITEID = headerData.Header.SYSITEID,




                acrDetail = acrDetail,
                acrAuthSeq = acrAuthSeq,
                acrAppHis = acrAppHis,
                skipAuthList = skipAuthList,
                Emp_Detail = new Employee_Details
                {
                    _ECode = headerData.Employee.ADEMPCODE,
                    _EName = headerData.Employee.FIRSTNAME + " " + headerData.Employee.LASTNAME,
                    _EmailId = headerData.Employee.EMAILID,
                    _DOB = DateTime.Now,
                    _SecDescrip = headerData.VW.SECTION,
                    _DepDesc = headerData.VW.DEPARTMENT,
                    _DivDesc = headerData.VW.DIVISION,
                    _OpDesc = headerData.VW.OPERATION,
                    _SiteId = headerData.VW.SYSITEID
                }
            };


            if (_obj.acrAuthSeq.Count > 0)
            {
                long lastSendBackAppHisId = 0;
                DGIT_ACRAPPHISTORY lastSendBackAppHis = _AcrDBContext.DGIT_ACRAPPHISTORY.Where(e => e.ACRID == _obj.ACRHEADERID && e.APPROVAL_STATUS == 2).OrderByDescending(o => o.ACRAPPHISTORY_ID).FirstOrDefault();
                if (lastSendBackAppHis != null)
                {
                    lastSendBackAppHisId = lastSendBackAppHis.ACRAPPHISTORY_ID;
                }
                //int i = 0;//SM
                foreach (ACRAppAuthSeqViewModel obj in _obj.acrAuthSeq)
                {
                    bool IsBeforeSendBackRecord = _AcrDBContext.DGIT_ACRAPPHISTORY.Where(w => w.ACRID == obj.ACRID && w.ACRAPPHISTORY_ID <= lastSendBackAppHisId && w.ADEMPCODE == obj.ADEMPCODE).FirstOrDefault() == null ? false : true;

                    bool condition;

                    if (IsBeforeSendBackRecord)
                    {
                        condition = _obj.acrAppHis
                        .Count(r =>
                        r.ACRID == obj.ACRID &&
                        r.ADEMPCODE == obj.ADEMPCODE &&
                        r.ACRAPPHISTORY_ID > lastSendBackAppHisId) == 0;
                    }
                    else
                    {
                        condition = _obj.acrAppHis
                        .Count(x =>
                        x.ACRID == obj.ACRID &&
                        x.ADEMPCODE == obj.ADEMPCODE &&
                        x.APP_SEQ == obj.APP_SEQ) == 0;
                    }

                    //Added By Aumento as on 31072024 for SR76166========================================================================================
                    //if ((IsBeforeSendBackRecord ? (!_obj.acrAppHis.Any(r => r.ACRID == obj.ACRID && r.ADEMPCODE == obj.ADEMPCODE && r.ACRAPPHISTORY_ID > lastSendBackAppHisId)) : (!_obj.acrAppHis.Any(x => x.ACRID == obj.ACRID && x.ADEMPCODE == obj.ADEMPCODE))))
                    // if ((IsBeforeSendBackRecord ? (!_obj.acrAppHis.Any(r => r.ACRID == obj.ACRID && r.ADEMPCODE == obj.ADEMPCODE && r.ACRAPPHISTORY_ID > lastSendBackAppHisId)) : (!_obj.acrAppHis.Any(x => x.ACRID == obj.ACRID && x.ADEMPCODE == obj.ADEMPCODE && x.APP_SEQ==obj.APP_SEQ))))
                    if (condition)
                    //====================================================================================================================================
                    {
                        // i = i + 1;//SM
                        _obj.acrAppHis.Add(new ACRAppHistoryViewModel
                        {
                            ACRAPPHISTORY_ID = 0,
                            //ACRAPPHISTORY_ID = i,//SM
                            ACRID = obj.ACRID,
                            ADEMPCODE = obj.ADEMPCODE,
                            APPROVAL_STATUS = 0,
                            APPROVAL_REMARK = "",
                            APPEMP_NAME = obj.ADEMPNAME + "[" + obj.ADEMPCODE + "]",
                            //Added By Aumento as on 31072024 for SR76166====================================
                            APP_SEQ = obj.APP_SEQ
                            //===============================================================================
                        });
                    }
                }
            }
            //Added By Aumento as on 31072024 for SR76166========================================================================================
            if (_obj != null)
            {
                // _obj.acrAppHis = _obj.acrAppHis.AsEnumerable().OrderBy(r => r.APP_SEQ).ToList();
                _obj.acrAppHis = _obj.acrAppHis.AsEnumerable().OrderBy(r => r.APPROVALDATE.HasValue ? r.APPROVALDATE.Value : DateTime.MaxValue)
                                                              .ThenBy(r => r.APP_SEQ)
                                                              .ToList();
            }
            //====================================================================================================================================
            return _obj;
        }
        public short ACRApproval(ACRAppHistoryViewModel PHVM)
        {
            short retVal = 0;
            using (var transaction = _AcrDBContext.Database.BeginTransaction())
            {
                try
                {
                    DGIT_ACRAPPHISTORY DPAH = new DGIT_ACRAPPHISTORY();
                    if (PHVM.ACRID > 0)
                    {
                        //Added By Aumento as on 31072024 for SR76166========================================================================================                        
                        int CurrApp_seq = 0;
                        int NextApp_seq = 0;
                        Int64 ACRAPPAUTHID = 0;
                        //=====================================================================================================================================

                        /////////// Update Approval Status //////////
                        // DPAH = _AcrDBContext.DGIT_ACRAPPHISTORY.Where(x => x.ACRID == PHVM.ACRID && x.ADEMPCODE == PHVM.ADEMPCODE).FirstOrDefault();
                        DPAH = _AcrDBContext.DGIT_ACRAPPHISTORY.Where(x => x.ACRID == PHVM.ACRID && x.ADEMPCODE == PHVM.ADEMPCODE && x.APPROVAL_STATUS == 0).FirstOrDefault();
                        if (DPAH != null)
                        {
                            DPAH.APPROVAL_STATUS = PHVM.APPROVAL_STATUS;
                            DPAH.APPROVAL_REMARK = PHVM.APPROVAL_REMARK;
                            DPAH.UPDATEBY = PHVM.UPDATEBY;
                            DPAH.APP_DATE = DateTime.Now;
                            DPAH.UPDATEDATE = DateTime.Now;
                            _AcrDBContext.Entry(DPAH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _AcrDBContext.SaveChanges();

                            //Added By Aumento as on 31072024 for SR76166========================================================================================                        
                            CurrApp_seq = Int16.Parse(DPAH.APP_SEQ.ToString());
                            //Int64 CurrauthIds = Convert.ToInt64(_AcrDBContext.DGIT_ACRAPPAUTHSEQ.Where(s => s.ACRID == PHVM.ACRID && s.APP_SEQ == CurrApp_seq ).Select(s => s.ACRAPPAUTH_ID).FirstOrDefault()); //Change by aumento as on 21082024 For SR78429
                            Int64 CurrauthIds = Convert.ToInt64(_AcrDBContext.DGIT_ACRAPPAUTHSEQ.Where(s => s.ACRID == PHVM.ACRID && s.APP_SEQ == CurrApp_seq && s.ADEMPCODE == PHVM.UPDATEBY).Select(s => s.ACRAPPAUTH_ID).Max());//Change by aumento as on 21082024 For SR78429
                            ACRAPPAUTHID = CurrauthIds + 1;
                            //=====================================================================================================================================
                        }

                        /// --- Save PO Approval Authority --- ///
                        ACRAppAuthSeqViewModel seqModel = new ACRAppAuthSeqViewModel();
                        if (PHVM.APPROVAL_STATUS == 1)
                        {

                            //Below Change added on 12092022 for add two req.with same Department by (Aumento) =========================================================================================================================================
                            //short nextSeq = Convert.ToInt16(_AcrDBContext.DGIT_ACRAPPAUTHSEQ.Where(s => s.ACRID == PHVM.ACRID && s.ADEMPCODE == PHVM.ADEMPCODE).Select(s => s.APP_SEQ).FirstOrDefault() + 1);


                            //Below Change added on 12092022 by Aumento for add two req.with same Department =========================================================================================================================================
                            //short currSeq = nextSeq;
                            //currSeq--;
                            //short countInSameSeq = Convert.ToInt16(_AcrDBContext.DGIT_ACRAPPAUTHSEQ.Where(s => s.ACRID == PHVM.ACRID && s.APP_SEQ == currSeq).Select(s => s.APP_SEQ).Count());
                            //if (countInSameSeq == 2)
                            //{
                            //    int OtherADEMPCODE = Convert.ToInt32(_AcrDBContext.DGIT_ACRAPPAUTHSEQ.Where(s => s.ACRID == PHVM.ACRID && s.APP_SEQ == currSeq && s.ADEMPCODE != PHVM.ADEMPCODE).Select(s => s.ADEMPCODE).FirstOrDefault());
                            //    DGIT_ACRAPPHISTORY DPAH_FOR_SECOND_ADEMPCODE = new DGIT_ACRAPPHISTORY();
                            //    DPAH_FOR_SECOND_ADEMPCODE = _AcrDBContext.DGIT_ACRAPPHISTORY.Where(x => x.ACRID == PHVM.ACRID && x.ADEMPCODE == OtherADEMPCODE).FirstOrDefault();
                            //    if (DPAH_FOR_SECOND_ADEMPCODE == null)
                            //    {
                            //        nextSeq = currSeq;
                            //    }
                            //}
                            //========================================================================================================================================================================================================================
                            //seqModel = (from data in _AcrDBContext.DGIT_ACRAPPAUTHSEQ.Where(a => a.APP_SEQ >= nextSeq && a.ACRID == PHVM.ACRID && a.ADEMPCODE != PHVM.ADEMPCODE).ToList().OrderBy(m => m.APP_SEQ)

                            //Added By Aumento as on 31072024 for SR76166=================================================================================================================================================================                        
                            //seqModel = (from data in _AcrDBContext.DGIT_ACRAPPAUTHSEQ.Where(a => a.ACRAPPAUTH_ID >= ACRAPPAUTHID && a.ACRID == PHVM.ACRID && a.ADEMPCODE != PHVM.ADEMPCODE).ToList().OrderBy(m => m.ACRAPPAUTH_ID)
                            seqModel = (from data in _AcrDBContext.DGIT_ACRAPPAUTHSEQ.Where(a => a.ACRAPPAUTH_ID == ACRAPPAUTHID && a.ACRID == PHVM.ACRID && a.ADEMPCODE != PHVM.ADEMPCODE).ToList().OrderBy(m => m.ACRAPPAUTH_ID)
                                            //============================================================================================================================================================================================================
                                        select new ACRAppAuthSeqViewModel
                                        {
                                            ACRAPPAUTH_ID = data.ACRAPPAUTH_ID,
                                            ACRID = data.ACRID,
                                            ADEMPCODE = data.ADEMPCODE,
                                            APP_SEQ = data.APP_SEQ,
                                            STATUS = data.STATUS,
                                            //Added by aumento as on 29102024 for the SR82874===============
                                            APPTYPE = data.APPTYPE,
                                            //=============================================================
                                        }).FirstOrDefault();
                            if (seqModel != null)
                            {
                                SaveACRAppHis(PHVM.ADDEDBY, PHVM.ACRID, seqModel);
                            }
                        }

                        //////// Update Process Status ////////
                        DGIT_ACRHEADER DPH = new DGIT_ACRHEADER(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                        DPH = _AcrDBContext.DGIT_ACRHEADER.Where(x => x.ACRHEADERID == PHVM.ACRID).SingleOrDefault();
                        if (DPH != null)
                        {
                            //Added by aumento as on 29102024 for the SR82874======================
                            //if (PHVM.APPROVAL_STATUS == 1 && seqModel == null)
                            if (PHVM.APPROVAL_STATUS == 1 && (seqModel == null || seqModel.APPTYPE == 3))
                            //======================================================================
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
                            _AcrDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _AcrDBContext.SaveChanges();
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

        public short ACRCancel(ACRHeaderViewModel PHVM)
        {
            short retVal = 0;
            using (var transaction = _AcrDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (PHVM.ACRHEADERID > 0)
                    {
                        //////// Update Process Status ////////
                        DGIT_ACRHEADER DPH = new DGIT_ACRHEADER(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                        DPH = _AcrDBContext.DGIT_ACRHEADER.Where(x => x.ACRHEADERID == PHVM.ACRHEADERID).SingleOrDefault();
                        if (DPH != null)
                        {
                            DPH.PROCESS_STATUS = 4;
                            DPH.UPDATEDBY = PHVM.UPDATEDBY;
                            DPH.UPDATEDATE = DateTime.Now;
                            _AcrDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _AcrDBContext.SaveChanges();
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

        public List<ACRDetailViewModel> GetAttachmentDetail(long _ACRHEADERID)
        {
            return (from _PODetail in _AcrDBContext.DGIT_ACRDETAIL.Where(d => d.ACRHEADERID == _ACRHEADERID)
                    where _PODetail.STATUS == 1
                    select new ACRDetailViewModel
                    {
                        ACRDTL_ID = _PODetail.ACRDTL_ID,
                        ACRHEADERID = _PODetail.ACRHEADERID,
                        DOC_TYPE = _PODetail.DOC_TYPE,
                        ADDITIONAL_INFO = _PODetail.ADDITIONAL_INFO,
                        FILENAME = _PODetail.FILENAME,
                    }).ToList();
        }

        public short DeleteAttachment(string fileName, string docType, long ACRHEADERID)
        {
            short retVal = 0;
            if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(docType) && ACRHEADERID > 0)
            {
                DGIT_ACRDETAIL DT = _AcrDBContext.DGIT_ACRDETAIL.Where(x => x.FILENAME == fileName && x.DOC_TYPE == docType && x.ACRHEADERID == ACRHEADERID).FirstOrDefault();
                if (DT != null)
                {
                    _AcrDBContext.DGIT_ACRDETAIL.Remove(DT);
                    _AcrDBContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }

        public POHeaderViewModel GetPODetailByPOId(string poNo)
        {
            var POHeaderID = (from data in _AcrDBContext.DGIT_POHEADER.Where(m => m.PROCESS_STATUS == 2 && m.PONO == poNo)
                              group data by data.PONO into g
                              select new { PONO = g.Key, MaxUid = g.Max(s => s.POHEADERID) }
                              ).ToList().Select(m => m.MaxUid).ToArray();

            var _obj = (from data in _AcrDBContext.DGIT_POHEADER
                        where POHeaderID.Contains(data.POHEADERID) && data.PROCESS_STATUS == 2
                        select new POHeaderViewModel
                        {
                            IsFinalSubmit = 2,
                            poDetail = (from _POMapp in _AcrDBContext.DGIT_PODETAIL.Where(l => l.POHEADERID == data.POHEADERID)
                                            //change done on 19102022 by (Aumento)======================
                                            //where _POMapp.STATUS == 1 && _POMapp.DOC_TYPE != "PO"
                                        where _POMapp.STATUS == 1 && _POMapp.DOC_TYPE == "POA"
                                        //===========================================================


                                        select new PODetailViewModel
                                        {
                                            ADDITIONAL_INFO = _POMapp.ADDITIONAL_INFO,
                                            DOC_TYPE = _POMapp.DOC_TYPE,
                                            FILENAME = _POMapp.FILENAME,
                                        }).ToList(),
                        }).FirstOrDefault();
            return _obj;
        }

        public long GetPRStatusByPOId(string pono)
        {
            int retval = 0;
            var _obj = (from _PR in _AcrDBContext.DGIT_POHEADER
                        where _PR.PONO == pono && (_PR.PROCESS_STATUS != 2 && _PR.PROCESS_STATUS != 3 && _PR.PROCESS_STATUS != 4)
                        select _PR).ToList();

            if (_obj != null && _obj.Count > 0)
            {
                retval = 1; //Return 1 if any of PP approval not finished for related PONO
            }
            return retval;
        }

        public List<Employee_Details> AutocompleteDesignation(string Key)
        {
            List<Employee_Details> portalUserDtos = new List<Employee_Details>();
            portalUserDtos = (from data in _AcrDBContext.ADDESIGNATION
                              where (data.DESCRIP.ToUpper().Contains(Key.ToUpper())) && data.ACTIVE == 1
                              select new Employee_Details
                              {
                                  _Desig = data.DESCRIP,
                              }).ToList();

            portalUserDtos.AddRange((from data in _AcrDBContext.ADFUNCTIONALDESIGNATION
                                     where (data.DESCRIP.ToUpper().Contains(Key.ToUpper())) && data.ACTIVE == 1
                                     select new Employee_Details
                                     {
                                         _Desig = data.DESCRIP,
                                     }).ToList());
            return portalUserDtos;
        }

        //============Change Done on 27082022 by Aumento Team For Add Other Category==========================================================================================       
        //public List<Employee_Details> PortalAutocompleteSuggestions(string Key, string designation)
        //{
        //    int isSearchDesg = 0;

        //    //bool isNumeric = int.TryParse(Key, out n);
        //    //long _value = (isNumeric ? Convert.ToInt64(Key) : 0);
        //    var oDesg = _AcrDBContext.ADDESIGNATION.Where(m => m.ACTIVE == 1 && m.DESCRIP.ToUpper().Contains(designation.ToUpper())).ToList();
        //    var oFDesg = _AcrDBContext.ADFUNCTIONALDESIGNATION.Where(m => m.ACTIVE == 1 && m.DESCRIP.ToUpper().Contains(designation.ToUpper())).ToList();

        //    if (oDesg.Count > 0 || oFDesg.Count > 0)
        //    {
        //        isSearchDesg = 1;
        //    }
        //    long strKIID = (long)_AcrDBContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
        //    var empdata = (from userdata in _AcrDBContext.ADEMPDIVDEPTSECT.Where(m => m.SYKI == strKIID)
        //                   join emp in _AcrDBContext.ADEMPLOYEE.Where(m => m.ACTIVE == 1) on userdata.ADEMPCODE equals emp.ADEMPCODE
        //                   join d in _AcrDBContext.ADDESIGNATION.Where(m => m.ACTIVE == 1) on userdata.ADDESIGNATIONID equals d.ADDESIGNATIONID
        //                   join fg in _AcrDBContext.ADFUNCTIONALDESIGNATION.Where(m => m.ACTIVE == 1) on userdata.ADFUNCTIONALDESIGNATIONID equals fg.ADFUNCTIONALDESIGNATIONID into ls
        //                   from fg in ls.DefaultIfEmpty()
        //                   where (isSearchDesg == 1 ? (fg.DESCRIP.ToUpper().Contains(designation.ToUpper()) || d.DESCRIP.ToUpper().Contains(designation.ToUpper())) : true)
        //                   select new
        //                   {
        //                       ADEMPCODE = emp.ADEMPCODE,
        //                       FIRSTNAME = emp.FIRSTNAME,
        //                       LASTNAME = emp.LASTNAME,
        //                       _ENAME = emp.FIRSTNAME + " " + emp.LASTNAME,
        //                   }
        //                   ).ToList();
        //    List<Employee_Details> portalUserDtos = new List<Employee_Details>();
        //    portalUserDtos = (from userdata in empdata
        //                      where (userdata.ADEMPCODE.ToString().StartsWith(Key) || userdata.FIRSTNAME.ToUpper().Contains(Key.ToUpper()) || userdata.LASTNAME.ToUpper().Contains(Key.ToUpper()) || (userdata._ENAME.ToUpper()).Contains(Key.ToUpper()))
        //                      select new Employee_Details
        //                      {
        //                          _ECode = userdata.ADEMPCODE,
        //                          _EFirstName = userdata.FIRSTNAME,
        //                          _ELastName = userdata.LASTNAME,
        //                      }).ToList();
        //    //foreach (var portaluser in portalUserequery)
        //    //{
        //    //    portalUserDtos.Add(Mapper.Map<PORTALUSER, PortalUser>(portaluser));
        //    //}
        //    return portalUserDtos;
        //}
        //============================================================================================================================


        //============Change Done on 19092022 For Add Other Category by (Aumento)==========================================================================================
        public List<Employee_Details> PortalAutocompleteSuggestions(string Key, string designation)
        {
            int isSearchDesg = 0;

            //Change Done on 20092022 for get employee number as per Designation selection by by (Aumento)========================================
            int desg = 0;

            int fundesg = 0;

            if (designation == "1")
                fundesg = 1;//section Manager

            if (designation == "2")
                fundesg = 2;//Department Manager

            if (designation == "3")
                desg = 1;

            if (designation == "4")
                fundesg = 3;//Division Head

            if (designation == "5")
                desg = 22;

            if (designation == "6")
                fundesg = 4;//Operating Head

            if (designation == "7")
                desg = 30;

            if (designation == "9")
                desg = 27;

            //if (designation == "10")
            //    desg = 27;


            if (designation == "11")
                desg = 31;

            //============================================================================================================================

            //bool isNumeric = int.TryParse(Key, out n);
            //long _value = (isNumeric ? Convert.ToInt64(Key) : 0);
            var oDesg = _AcrDBContext.ADDESIGNATION.Where(m => m.ACTIVE == 1 && m.ADDESIGNATIONID == desg).ToList();
            var oFDesg = _AcrDBContext.ADFUNCTIONALDESIGNATION.Where(m => m.ACTIVE == 1 && m.ADFUNCTIONALDESIGNATIONID == fundesg).ToList();

            if (oDesg.Count > 0 || oFDesg.Count > 0)
            {
                isSearchDesg = 1;
            }
            long strKIID = (long)_AcrDBContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
            List<Employee_Details> empdata_ans = new List<Employee_Details>();
            if (desg > 0)
            {
                var empdata = (from userdata in _AcrDBContext.ADEMPDIVDEPTSECT.Where(m => m.SYKI == strKIID && m.ADDESIGNATIONID == desg)
                               join emp in _AcrDBContext.ADEMPLOYEE.Where(m => m.ACTIVE == 1) on userdata.ADEMPCODE equals emp.ADEMPCODE
                               //join d in _AcrDBContext.ADDESIGNATION.Where(m => m.ACTIVE == 1 ) on userdata.ADDESIGNATIONID equals d.ADDESIGNATIONID
                               join d in _AcrDBContext.ADDESIGNATION.Where(m => m.ACTIVE == 1 && m.ADDESIGNATIONID == desg) on userdata.ADDESIGNATIONID equals d.ADDESIGNATIONID
                               join fg in _AcrDBContext.ADFUNCTIONALDESIGNATION.Where(m => m.ACTIVE == 1) on userdata.ADFUNCTIONALDESIGNATIONID equals fg.ADFUNCTIONALDESIGNATIONID into ls
                               from fg in ls.DefaultIfEmpty()
                                   //where (isSearchDesg == 1 ? (fg.DESCRIP.ToUpper().Contains(designation.ToUpper()) || d.DESCRIP.ToUpper().Contains(designation.ToUpper())) : true)
                               select new
                               {
                                   ADEMPCODE = emp.ADEMPCODE,
                                   FIRSTNAME = emp.FIRSTNAME,
                                   LASTNAME = emp.LASTNAME,
                                   _ENAME = emp.FIRSTNAME + " " + emp.LASTNAME,
                               }
                           ).ToList();



                List<Employee_Details> portalUserDtos = new List<Employee_Details>();
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
                //empdata_ans = portalUserDtos;
                return portalUserDtos;
            }
            else if (fundesg > 0)
            {

                var empdata_fundesg = (from userdata in _AcrDBContext.ADEMPDIVDEPTSECT.Where(m => m.SYKI == strKIID && m.ADFUNCTIONALDESIGNATIONID == fundesg)
                                       join emp in _AcrDBContext.ADEMPLOYEE.Where(m => m.ACTIVE == 1) on userdata.ADEMPCODE equals emp.ADEMPCODE
                                       //join d in _AcrDBContext.ADDESIGNATION.Where(m => m.ACTIVE == 1 ) on userdata.ADDESIGNATIONID equals d.ADDESIGNATIONID
                                       join d in _AcrDBContext.ADDESIGNATION.Where(m => m.ACTIVE == 1) on userdata.ADFUNCTIONALDESIGNATIONID equals d.ADDESIGNATIONID
                                       //join d in _AcrDBContext.ADDESIGNATION.Where(m => m.ACTIVE == 1 && m.ADDESIGNATIONID == desg) on userdata.ADDESIGNATIONID equals d.ADDESIGNATIONID
                                       //join fg in _AcrDBContext.ADFUNCTIONALDESIGNATION.Where(m => m.ACTIVE == 1) on userdata.ADFUNCTIONALDESIGNATIONID equals fg.ADFUNCTIONALDESIGNATIONID into ls
                                       join fg in _AcrDBContext.ADFUNCTIONALDESIGNATION.Where(m => m.ACTIVE == 1) on userdata.ADFUNCTIONALDESIGNATIONID equals fg.ADFUNCTIONALDESIGNATIONID into ls
                                       from fg in ls.DefaultIfEmpty()
                                           //where (isSearchDesg == 1 ? (fg.DESCRIP.ToUpper().Contains(designation.ToUpper()) || d.DESCRIP.ToUpper().Contains(designation.ToUpper())) : true)
                                       select new
                                       {
                                           ADEMPCODE = emp.ADEMPCODE,
                                           FIRSTNAME = emp.FIRSTNAME,
                                           LASTNAME = emp.LASTNAME,
                                           _ENAME = emp.FIRSTNAME + " " + emp.LASTNAME,
                                       }
                           ).ToList();
                List<Employee_Details> portalUserDtos_fundesg = new List<Employee_Details>();
                portalUserDtos_fundesg = (from userdata in empdata_fundesg
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
                empdata_ans = portalUserDtos_fundesg;
            }

            return empdata_ans;

        }
        //==================================================================================================================================================================================
        //Below Code added by Aumento Start
        public List<PR_Div_Dep_SecViewModel> GetKiLIST()
        {
            var KILIST = (from data in _AcrDBContext.SYKI
                          select new PR_Div_Dep_SecViewModel
                          {
                              Text = data.KICODE,
                              Value = (long)(data.SYKIID)
                          }
                      ).ToList();
            return KILIST.OrderByDescending(m => m.Value).ToList();
        }
        public SearchACRViewModel ACRDashboard(SearchACRViewModel VM, long plantId)
        {
            DateTime applicabledate = DateTime.ParseExact("08-MAR-2024", "dd-MMM-yyyy", null);
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

            List<ACRHeaderViewModel> obj = new List<ACRHeaderViewModel>();
            var acrHeaders = (from data in _AcrDBContext.VW_DGIT_ACRHEADER.Where(y => y.DATEADDED >= applicabledate)
                              join emp in _AcrDBContext.VW_ASSOCIATELVLDETAILS on new { addedby = data.ADDEDBY, SYKIID = data.SYKIID } equals new { addedby = emp.ADEMPCODE, SYKIID = emp.SYKI }
                              // added by aumento start :: SR111540
                              join po in _AcrDBContext.DGIT_POHEADER on data.PO_NUMBER equals po.PONO into poJoin
                              from poData in poJoin.DefaultIfEmpty()
                              join vendor in _AcrDBContext.FINVENDORMASTERMST on poData.VENDORID equals vendor.VENDORCODE into vendorJoin
                              from vendorData in vendorJoin.DefaultIfEmpty()
                                  // added by aumento end :: SR111540
                                  //join _vendor in _AcrDBContext.FINVENDORMASTERMST on data.VENDORCODE equals _vendor.VENDORCODE into _vendorJoin
                                  //from _vendorData in _vendorJoin.DefaultIfEmpty()
                              join _ed in _AcrDBContext.ADEMPLOYEE on data.ADDEDBY equals _ed.ADEMPCODE
                              join _site in _AcrDBContext.DGIT_SMPLANTSITEMAP on (data.SYSITEID ?? emp.SYSITEID) equals _site.SITEID
                              //join _site in _SmDBContext.DGIT_SMPLANTSITEMAP on emp.SYSITEID equals _site.SITEID
                              join _docRevBy in _AcrDBContext.ADEMPLOYEE on (long)data.DOC_REV_BY equals _docRevBy.ADEMPCODE into _docRevByJoin
                              from _docEmp in _docRevByJoin.DefaultIfEmpty()
                              where
                    (string.IsNullOrEmpty(VM.Startdate) || data.DATEADDED >= ReqDateFrom) &&
                    (string.IsNullOrEmpty(VM.ENDDATE) || data.DATEADDED <= ReqDateTo) &&
                (emp.SYKI == VM.KIID) &&
                (VM.OperationID == 0 || emp.OPERATIONID == VM.OperationID) &&
                 (VM.DivisionID == 0 || emp.DIVISIONID == VM.DivisionID) &&
                 (VM.DEPTID == 0 || emp.DEPARTMENTID == VM.DEPTID) &&
                 (VM.SECID == 0 || emp.SECTIONID == VM.SECID) &&
                 (VM.ecode == 0 || data.ADDEDBY == VM.ecode) &&
                 (VM.Doc_Status == -1 || data.DOC_STATUS == VM.Doc_Status) &&
                (VM.ReqStatus == -1 ||
                    (VM.ReqStatus == 0
                        ? (data.PROCESS_STATUS == 2 || data.PROCESS_STATUS == 5 || data.PROCESS_STATUS == 8)
                        : data.PROCESS_STATUS == VM.ReqStatus)) &&
               (_site.PLANTID == plantId) &&
                              (data.ACRNO == VM.ACRNo || VM.ACRNo == "") && // && added by aumento  :: SR111540
                              (string.IsNullOrEmpty(VM.VENDORCODE) || vendorData.VENDORCODE == VM.VENDORCODE) &&// added by aumento :: SR111540
                              (string.IsNullOrEmpty(VM.PONO) || data.PO_NUMBER == VM.PONO) // aaded by aumento :: SR111540
                              select new ACRHeaderViewModel
                              {
                                  ACRHEADERID = data.ACRHEADERID,
                                  PROCESS_STATUS = data.PROCESS_STATUS,
                                  ADDEDBYNAME = _ed.FIRSTNAME + " " + _ed.LASTNAME,
                                  DATEADDED = data.DATEADDED,
                                  ACRNO = data.ACRNO,
                                  PONO = data.PO_NUMBER,
                                  AMOUNT = data.AMOUNT,
                                  STATUS = data.STATUS,
                                  PA_GENERATED = data.PA_GENERATED,
                                  // VENDORCODE = (_vendorData == null ? "" : _vendorData.VENDORCODE),
                                  // VENDORNAME = (_vendorData == null ? "" : _vendorData.NAME1),
                                  VENDORCODE = vendorData.VENDORCODE ?? "", // added by aumento :: SR111540
                                  VENDORNAME = vendorData.NAME1 ?? "",      // added by aumento :: SR111540
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
                              })
                              .GroupBy(x => x.ACRHEADERID).Select(g => g.First()) //Added by Aumento:: CR7448_V1
                              .ToList();
            var acrDetails = _AcrDBContext.DGIT_ACRDETAIL
   .Where(d => (d.DOC_TYPE == "ACRA" || d.DOC_TYPE == "ACRFA" || d.DOC_TYPE == "ACR") && d.STATUS == 1)
   .Select(d => new ACRDetailViewModel
   {
       ACRDTL_ID = d.ACRDTL_ID,
       ACRHEADERID = d.ACRHEADERID,
       DOC_TYPE = d.DOC_TYPE,
       ADDITIONAL_INFO = d.ADDITIONAL_INFO,
       FILENAME = d.FILENAME
   }).ToList();

            foreach (var header in acrHeaders)
            {
                header.acrDetail = acrDetails
                .Where(d => d.ACRHEADERID == header.ACRHEADERID)
                .OrderBy(d => d.ACRDTL_ID)
                .ToList();
            }
            obj = acrHeaders;

            if (!string.IsNullOrEmpty(VM.ACRNo))
            {
                VM.SearchResult = (from data in obj
                                   where (data.acrDetail.Where(x => x.ACRHEADERID == data.ACRHEADERID).Count() > 0 ? true : false)
                                   || (data.ACRNO == VM.ACRNo ? true : false)
                                   select data
                                   ).ToList();
            }
            else
            {
                VM.SearchResult = obj.OrderByDescending(o => o.DATEADDED).ToList();
            }

            return VM;
        }
        public short SavePARequest(VM_ACR_PaymentAdvise_Master model)
        {
            short retVal = 0; /*long retHeaderId = 0*/;
            VendorViewModel vendordtls = new VendorViewModel();
            foreach (string ses_no in model.reqno_list)
            {
                using (var transaction = _AcrDBContext.Database.BeginTransaction())
                {
                    try
                    {
                        long decimal_ses = (long)Convert.ToDouble(ses_no);
                        DGIT_ACR_PAYMENTADVISEMASTER DPH = new DGIT_ACR_PAYMENTADVISEMASTER();
                        int FlagAdd = 0;
                        if (model.PAYMENTADVISE_NO.Length > 0)
                        {
                            DPH = new DGIT_ACR_PAYMENTADVISEMASTER();
                            decimal ses_decimal = Convert.ToDecimal(ses_no);
                            //vendordtls = (from _sesdtls in _AcrDBContext.DGIT_SESMRN_HEADER
                            //              join _vendordtls in _AcrDBContext.FINVENDORMASTERMST on _sesdtls.VENDORCODE equals _vendordtls.VENDORCODE
                            //              where _sesdtls.SMHEADERID == decimal_ses
                            //              select new VendorViewModel
                            //              {
                            //                  VENDORCODE = _vendordtls.VENDORCODE,
                            //                  VENDORNAME = _vendordtls.NAME1
                            //              }).FirstOrDefault();
                            if (_AcrDBContext.DGIT_ACR_PAYMENTADVISEMASTER.Where(x => x.PAYMENTADVISE_NO == model.PAYMENTADVISE_NO && x.ACR_NO == ses_decimal && x.ISDELETE == 0).FirstOrDefault() != null)
                            {
                                retVal = 2;
                                return retVal; //// -- record already exist.
                            }
                            if (_AcrDBContext.DGIT_ACR_PAYMENTADVISEMASTER.Where(x => x.ACR_NO == ses_decimal && x.ISDELETE == 1).FirstOrDefault() != null)
                            {
                                DPH = _AcrDBContext.DGIT_ACR_PAYMENTADVISEMASTER.Where(x => x.ACR_NO == ses_decimal).FirstOrDefault();

                            }
                            else if (_AcrDBContext.DGIT_ACR_PAYMENTADVISEMASTER.FirstOrDefault() == null)
                            {
                                DPH.PAHEADER_ID = 1;
                                FlagAdd = 1;
                            }
                            else
                            {
                                DPH.PAHEADER_ID = _AcrDBContext.DGIT_ACR_PAYMENTADVISEMASTER.Max(x => x.PAHEADER_ID) + 1;
                                FlagAdd = 1;
                            }
                            DPH.ACR_NO = (long)Convert.ToDouble(ses_no);
                        }
                        DPH.ISDELETE = 0;
                        DPH.STATUS = 1;
                        DPH.REMARK = model.REMARK == null ? "" : model.REMARK;
                        DPH.DOCUMENT_PATH = model.DOCUMENT_PATH;
                        DPH.PAYMENTADVISE_NO = model.PAYMENTADVISE_NO;
                        DPH.PA_DATE = model.PA_DATE;
                        DPH.ISSEND = 0;
                        //DPH.VENDOR_CODE = vendordtls.VENDORCODE;
                        //DPH.VENDOR_NAME = vendordtls.VENDORNAME;
                        if (FlagAdd == 1)
                        {
                            DPH.ADDEDBY = (long)model.ADDEDBY;
                            DPH.DATEADDED = DateTime.Now;
                        }
                        else
                        {
                            DPH.UPDATEBY = model.UPDATEBY;
                            DPH.UPDATEDDATE = DateTime.Now;
                        }
                        _AcrDBContext.Entry(DPH).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                        _AcrDBContext.SaveChanges();

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
                using (var transaction = _AcrDBContext.Database.BeginTransaction())
                {
                    try
                    {
                        if (model != null)
                        {
                            long ses_id = (long)Convert.ToDouble(ses_no);

                            DGIT_ACRHEADER DSAH;
                            DSAH = _AcrDBContext.DGIT_ACRHEADER.Where(x => x.ACRHEADERID == ses_id).SingleOrDefault();
                            DSAH.PA_GENERATED = 1;
                            _AcrDBContext.Entry(DSAH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _AcrDBContext.SaveChanges();
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

        public short UpdateDocStatus(long smheaderId, long updatedBy)
        {
            short retVal = 0;
            using (var transaction = _AcrDBContext.Database.BeginTransaction())
            {
                try
                {
                    if (smheaderId > 0)
                    {
                        //////// Update Doc Status ////////
                        DGIT_ACRHEADER DPH = new DGIT_ACRHEADER();
                        DPH = _AcrDBContext.DGIT_ACRHEADER.Where(x => x.ACRHEADERID == smheaderId).SingleOrDefault();
                        if (DPH != null)
                        {
                            DPH.DOC_STATUS = 1;
                            DPH.DOC_REV_BY = updatedBy;
                            DPH.DOC_REV_DATE = DateTime.Now;
                            DPH.PROCESS_STATUS = 5; //// 5-Pending at Finance
                            DPH.UPDATEDBY = updatedBy;
                            DPH.UPDATEDATE = DateTime.Now;
                            _AcrDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _AcrDBContext.SaveChanges();

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

        public short SavePADetails(long AddedBy, List<VM_ACR_PADetailViewModel> PDVMList)
        {
            short retVal = 0;
            foreach (VM_ACR_PADetailViewModel PDVM in PDVMList)
            {
                DGIT_ACR_PAYMENTADVISE_DTL DPD = new DGIT_ACR_PAYMENTADVISE_DTL();
                int FlagAdd = 0;
                if (PDVM.PAYMENTADVISE_NO.Length > 0)
                {
                    if (PDVM.DOC_TYPE == "PA")
                    {
                        DPD = _AcrDBContext.DGIT_ACR_PAYMENTADVISE_DTL.Where(x => x.PAYMENTADVISE_NO == PDVM.PAYMENTADVISE_NO && x.DOC_TYPE == PDVM.DOC_TYPE).FirstOrDefault();
                        if (DPD != null)
                        {
                            _AcrDBContext.DGIT_ACR_PAYMENTADVISE_DTL.Remove(DPD);
                            _AcrDBContext.SaveChanges();
                        }
                    }

                    DPD = new DGIT_ACR_PAYMENTADVISE_DTL();
                    if (_AcrDBContext.DGIT_ACR_PAYMENTADVISE_DTL.Count() == 0)
                    {
                        DPD.PA_DTL_ID = 1;
                        FlagAdd = 1;
                    }
                    else
                    {
                        DPD.PA_DTL_ID = _AcrDBContext.DGIT_ACR_PAYMENTADVISE_DTL.Max(x => x.PA_DTL_ID) + 1;
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
                    _AcrDBContext.Entry(DPD).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _AcrDBContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }
        public List<VM_ACR_PADetailViewModel> GetPAAttachmentDetail(string PA_HeaderID)
        {
            return (from _SMDetail in _AcrDBContext.DGIT_ACR_PAYMENTADVISE_DTL.Where(d => d.PAYMENTADVISE_NO == PA_HeaderID)
                    where _SMDetail.STATUS == 1
                    select new VM_ACR_PADetailViewModel
                    {
                        PA_DTL_ID = _SMDetail.PA_DTL_ID,
                        PAYMENTADVISE_NO = _SMDetail.PAYMENTADVISE_NO,
                        DOC_TYPE = _SMDetail.DOC_TYPE,
                        ADDITIONAL_INFO = _SMDetail.ADDITIONAL_INFO,
                        FILENAME = _SMDetail.FILENAME,
                        STATUS = (short)_SMDetail.STATUS
                    }).ToList();
        }
        public List<VM_ACR_PaymentAdvise_Master> GetPADetails(VM_ACR_PaymentAdvise_Master obj)
        {
            DateTime ReqDateFrom = DateTime.Now.Date;
            DateTime ReqDateTo = DateTime.Now.Date;
            List<VM_ACR_PaymentAdvise_Master> result = new List<VM_ACR_PaymentAdvise_Master>();
            if (!string.IsNullOrEmpty(obj.Startdate))
            {
                ReqDateFrom = DateTime.ParseExact(obj.Startdate, "dd-MMM-yyyy", null);
            }
            if (!string.IsNullOrEmpty(obj.ENDDATE))
            {
                ReqDateTo = DateTime.ParseExact(obj.ENDDATE + " 23:59:59", "dd-MMM-yyyy HH:mm:ss", null);
            }
            if ((obj.PAYMENTADVISE_NO == "" || obj.PAYMENTADVISE_NO == null) && obj.ACRNO == 0)
            {
                result = (from _SMDetail in _AcrDBContext.DGIT_ACR_PAYMENTADVISEMASTER
                          where _SMDetail.STATUS == 1
                          && (!string.IsNullOrEmpty(obj.Startdate) ? (_SMDetail.PA_DATE >= ReqDateFrom) : true)
                          && (!string.IsNullOrEmpty(obj.ENDDATE) ? (_SMDetail.PA_DATE <= ReqDateTo) : true)
                          select new VM_ACR_PaymentAdvise_Master
                          {
                              PAYMENTADVISE_NO = _SMDetail.PAYMENTADVISE_NO,
                              ACRNO = (long)_SMDetail.ACR_NO,
                              DOCUMENT_PATH = _SMDetail.DOCUMENT_PATH,
                              STATUS = _SMDetail.STATUS,
                              ISDELETE = _SMDetail.ISDELETE,
                              REMARK = _SMDetail.REMARK,
                              PA_DATE = _SMDetail.PA_DATE
                          }).ToList();
            }
            else
            {
                //decimal PANO = Convert.ToDecimal(obj.PAYMENTADVISE_NO);
                decimal acr_no = Convert.ToDecimal(obj.ACRNO);
                if (obj.PAYMENTADVISE_NO != "" && obj.PAYMENTADVISE_NO != null && acr_no != 0)
                {
                    result = (from _SMDetail in _AcrDBContext.DGIT_ACR_PAYMENTADVISEMASTER
                              where (_SMDetail.PAYMENTADVISE_NO == obj.PAYMENTADVISE_NO && _SMDetail.ACR_NO == acr_no)
                              && _SMDetail.STATUS == 1
                              && (!string.IsNullOrEmpty(obj.Startdate) ? (_SMDetail.PA_DATE >= ReqDateFrom) : true)
                              && (!string.IsNullOrEmpty(obj.ENDDATE) ? (_SMDetail.PA_DATE <= ReqDateTo) : true)
                              select new VM_ACR_PaymentAdvise_Master
                              {
                                  PAYMENTADVISE_NO = _SMDetail.PAYMENTADVISE_NO,
                                  ACRNO = (long)_SMDetail.ACR_NO,
                                  DOCUMENT_PATH = _SMDetail.DOCUMENT_PATH,
                                  STATUS = _SMDetail.STATUS,
                                  ISDELETE = _SMDetail.ISDELETE,
                                  REMARK = _SMDetail.REMARK,
                                  PA_DATE = _SMDetail.PA_DATE
                              }).ToList();
                }
                else
                {
                    result = (from _SMDetail in _AcrDBContext.DGIT_ACR_PAYMENTADVISEMASTER
                              join _sesdtls in _AcrDBContext.DGIT_ACRHEADER on _SMDetail.ACR_NO equals _sesdtls.ACRHEADERID
                              where (string.IsNullOrEmpty(obj.PAYMENTADVISE_NO) ? true : _SMDetail.PAYMENTADVISE_NO == obj.PAYMENTADVISE_NO)
                             && (obj.ACRNO == 0 ? true : _SMDetail.ACR_NO == obj.ACRNO)
                             && _SMDetail.STATUS == 1
                             && (!string.IsNullOrEmpty(obj.Startdate) ? (_SMDetail.PA_DATE >= ReqDateFrom) : true)
                             && (!string.IsNullOrEmpty(obj.ENDDATE) ? (_SMDetail.PA_DATE <= ReqDateTo) : true)
                              select new VM_ACR_PaymentAdvise_Master
                              {
                                  PAYMENTADVISE_NO = _SMDetail.PAYMENTADVISE_NO,
                                  ACRNO = (long)_SMDetail.ACR_NO,
                                  DOCUMENT_PATH = _SMDetail.DOCUMENT_PATH,
                                  STATUS = _SMDetail.STATUS,
                                  ISDELETE = _SMDetail.ISDELETE,
                                  REMARK = _SMDetail.REMARK,
                                  PA_DATE = _SMDetail.PA_DATE
                              }).ToList();
                }
            }
            result = result.OrderByDescending(o => o.PA_DATE).ToList();
            return result;

        }

        public short DeletePAAttachment(string fileName, string docType, long ACRHEADERID)
        {
            short retVal = 0;
            if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(docType) && ACRHEADERID > 0)
            {
                DGIT_ACR_PAYMENTADVISE_DTL DT = _AcrDBContext.DGIT_ACR_PAYMENTADVISE_DTL.Where(x => x.FILENAME == fileName && x.DOC_TYPE == docType && x.PA_DTL_ID == ACRHEADERID).FirstOrDefault();
                if (DT != null)
                {
                    _AcrDBContext.DGIT_ACR_PAYMENTADVISE_DTL.Remove(DT);
                    _AcrDBContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }
        public short deletePAReq(DeleteACRPA obj)
        {
            long decimal_Ses = (long)Convert.ToDecimal(obj.ACR_NO);
            try
            {
                DGIT_ACR_PAYMENTADVISEMASTER model = _AcrDBContext.DGIT_ACR_PAYMENTADVISEMASTER.Where(x => x.ACR_NO == decimal_Ses && x.PAYMENTADVISE_NO == (obj.PAYMENTADVISE_NO)).FirstOrDefault();
                model.ISDELETE = 1;
                model.REMARK = obj.REMARK;
                _AcrDBContext.Entry(model).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                DGIT_ACRHEADER model1 = _AcrDBContext.DGIT_ACRHEADER.Where(x => x.ACRHEADERID == decimal_Ses).FirstOrDefault();
                model1.PA_GENERATED = 0;
                _AcrDBContext.Entry(model).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _AcrDBContext.SaveChanges();
                return 1;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        public short ACRFinApproval(ACRAppHistoryViewModel PHVM, List<ACRDetailViewModel> SmDetailList)
        {
            short retVal = 0;
            using (var transaction = _AcrDBContext.Database.BeginTransaction())
            {
                try
                {
                    int FlagAdd = 0;

                    DGIT_ACRAPPHISTORY DPAH = new DGIT_ACRAPPHISTORY();
                    if (PHVM.ACRID > 0)
                    {
                        #region Update App Seq Table
                        int seqFlagAdd = 0;
                        //Added by aumento as on 26102024 for the SR82874==================
                        short appSeq = 0;
                        //End change========================================================

                        DGIT_ACRAPPAUTHSEQ objAppSeq = _AcrDBContext.DGIT_ACRAPPAUTHSEQ.Where(x => x.ACRID == PHVM.ACRID && x.ADEMPCODE == 1 && x.APPTYPE == 3 && x.STATUS == 1).FirstOrDefault();
                        if (objAppSeq == null)
                        {
                            objAppSeq = new DGIT_ACRAPPAUTHSEQ();
                            objAppSeq = _AcrDBContext.DGIT_ACRAPPAUTHSEQ.Where(x => x.ACRID == PHVM.ACRID && x.ADEMPCODE == PHVM.ADEMPCODE && x.APPTYPE == 3 && x.STATUS == 1).FirstOrDefault();
                            if (objAppSeq == null)
                            {
                                objAppSeq = new DGIT_ACRAPPAUTHSEQ();
                                if (_AcrDBContext.DGIT_ACRAPPAUTHSEQ.Count() == 0)
                                {
                                    objAppSeq.ACRAPPAUTH_ID = 1;
                                }
                                else
                                {
                                    objAppSeq.ACRAPPAUTH_ID = _AcrDBContext.DGIT_ACRAPPAUTHSEQ.Max(x => x.ACRAPPAUTH_ID) + 1;
                                }
                                seqFlagAdd = 1;
                                objAppSeq.ACRID = PHVM.ACRID;
                                //Added by aumento as on 26102024 for for the SR82874================
                                appSeq = (_AcrDBContext.DGIT_ACRAPPAUTHSEQ.Where(h => h.ACRID == PHVM.ACRID && h.STATUS == 1).ToList().Count == 0 ? (short)1 : Convert.ToInt16(_AcrDBContext.DGIT_ACRAPPAUTHSEQ.Where(n => n.ACRID == PHVM.ACRID && n.STATUS == 1).Max(x => x.APP_SEQ) + 1));
                                //End change========================================================
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
                        _AcrDBContext.Entry(objAppSeq).State = seqFlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                        _AcrDBContext.SaveChanges();
                        #endregion

                        #region Update Approval History
                        //Added by aumento as on 30102024 for the SR82874================
                        DGIT_ACRAPPAUTHSEQ objAppSeq1 = _AcrDBContext.DGIT_ACRAPPAUTHSEQ.Where(x => x.ACRID == PHVM.ACRID && x.ADEMPCODE == PHVM.ADEMPCODE && x.APPTYPE == 3 && x.STATUS == 1).FirstOrDefault();
                        //DPAH = _AcrDBContext.DGIT_ACRAPPHISTORY.Where(x => x.ACRID == PHVM.ACRID && x.ADEMPCODE == 1 && x.APPROVAL_STATUS == 0).FirstOrDefault();
                        DPAH = _AcrDBContext.DGIT_ACRAPPHISTORY.Where(x => x.ACRID == PHVM.ACRID && (x.ADEMPCODE == 1 || x.ADEMPCODE == objAppSeq1.ADEMPCODE) && x.APPROVAL_STATUS == 0).FirstOrDefault();
                        //==================================================================
                        if (DPAH == null)
                        {
                            DPAH = new DGIT_ACRAPPHISTORY();
                            if (_AcrDBContext.DGIT_ACRAPPHISTORY.Count() == 0)
                            {
                                DPAH.ACRAPPHISTORY_ID = 1;
                            }
                            else
                            {
                                DPAH.ACRAPPHISTORY_ID = _AcrDBContext.DGIT_ACRAPPHISTORY.Max(x => x.ACRAPPHISTORY_ID) + 1;
                            }
                            FlagAdd = 1;
                            DPAH.ACRID = PHVM.ACRID;
                            DPAH.ADDEDBY = PHVM.ADDEDBY;
                            DPAH.ADDEDDATE = DateTime.Now;
                        }

                        DPAH.ADEMPCODE = PHVM.ADEMPCODE;
                        DPAH.APPROVAL_STATUS = PHVM.APPROVAL_STATUS;
                        DPAH.APPROVAL_REMARK = PHVM.APPROVAL_REMARK;
                        DPAH.APP_DATE = DateTime.Now;
                        //Added by aumento as on 26102024 for the SR82874================
                        if (appSeq > 0)
                        {
                            DPAH.APP_SEQ = appSeq;
                        }
                        //End change=========================================================
                        if (FlagAdd == 0)
                        {
                            DPAH.UPDATEBY = PHVM.UPDATEBY;
                            DPAH.UPDATEDATE = DateTime.Now;
                        }
                        _AcrDBContext.Entry(DPAH).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                        _AcrDBContext.SaveChanges();
                        #endregion

                        #region Update Process Status 
                        DGIT_ACRHEADER DPH = new DGIT_ACRHEADER();
                        DPH = _AcrDBContext.DGIT_ACRHEADER.Where(x => x.ACRHEADERID == PHVM.ACRID).SingleOrDefault();
                        if (DPH != null)
                        {
                            if (PHVM.APPROVAL_STATUS == 1)
                            {
                                DPH.PROCESS_STATUS = 9; //// --- 9 Request Approval Completed
                                                        /////Added by TTL on 15-May-2025 | SR97294 > CR6254 - Start
                                ///(Approved ACRs were not accessible to approvers in Approval history due to this isue)
                                DGIT_ACRDETAIL SM_DTL = _AcrDBContext.DGIT_ACRDETAIL.Where(x => x.ACRHEADERID == PHVM.ACRID && x.DOC_TYPE == "ACRA" && x.STATUS == 0).FirstOrDefault();
                                if (SM_DTL != null)
                                {
                                    SM_DTL.STATUS = 1;
                                    SM_DTL.UPDATEDBY = PHVM.UPDATEBY;
                                    SM_DTL.UPDATEDATE = DateTime.Now;
                                    _AcrDBContext.Entry(SM_DTL).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                    _AcrDBContext.SaveChanges();
                                }
                                /////Added by TTL on 15-May-2025 | SR97294 > CR6254 - End

                            }
                            else if (PHVM.APPROVAL_STATUS == 4)
                            {
                                DPH.PROCESS_STATUS = 6; //// --- 6 Hold by Finance,
                            }
                            //else if (PHVM.APPROVAL_STATUS == 5)
                            //{
                            //    DPH.PROCESS_STATUS = 7; //// --- 7 Forword to Taxation,
                            //}
                            else if (PHVM.APPROVAL_STATUS == 2)
                            {
                                DPH.PROCESS_STATUS = 0;

                                //// --- Deactive Document Status inCase of Sendback ---////
                                DGIT_ACRDETAIL SM_DTL = _AcrDBContext.DGIT_ACRDETAIL.Where(x => x.ACRHEADERID == PHVM.ACRID && x.DOC_TYPE == "ACRA").FirstOrDefault();
                                if (SM_DTL != null)
                                {
                                    SM_DTL.STATUS = 0;
                                    SM_DTL.UPDATEDBY = PHVM.UPDATEBY;
                                    SM_DTL.UPDATEDATE = DateTime.Now;
                                    _AcrDBContext.Entry(SM_DTL).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                    _AcrDBContext.SaveChanges();
                                }
                            }
                            else if (PHVM.APPROVAL_STATUS == 3)
                            {
                                DPH.PROCESS_STATUS = 3;
                            }

                            DPH.UPDATEDBY = PHVM.UPDATEBY;
                            DPH.UPDATEDATE = DateTime.Now;
                            _AcrDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _AcrDBContext.SaveChanges();
                        }
                        #endregion

                        //#region Add Taxation Authority
                        //if (PHVM.APPROVAL_STATUS == 5) //// -- Forward to taxation
                        //{
                        //    short resSeq = SaveTaxationAuthority(PHVM);
                        //    if (resSeq == 0)
                        //    {
                        //        retVal = -1;
                        //        transaction.Rollback();
                        //    }
                        //}
                        //#endregion

                        //// --- Add Finance Attachment --- ////
                        if (SmDetailList.Count > 0)
                        {
                            SaveACRDetails(PHVM.ADDEDBY, PHVM.ACRID, SmDetailList);
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
        public IEnumerable<SYSITE> Bind_SYSite()
        {
            IEnumerable<SYSITE> iList;

            iList = (from data in _AcrDBContext.SYSITE.Where(x => x.ACTIVE == 1 && (x.SYSITEID == 3 || x.SYSITEID == 6 || x.SYSITEID == 8 || x.SYSITEID == 21 || x.SYSITEID == 22)).ToList()
                     select new SYSITE
                     {
                         SYSITEID = data.SYSITEID,
                         DESCRIP = data.DESCRIP + " Finance",
                     });
            return iList;
        }
        //==================================================================================================================================================================================
        //Below Code added by Aumento End

        //Added by aumento as on 28092024 for the SR82874 ===============================================
        public long GetACRNextApprovalId(long ACRID, long ecode)
        {
            long retval = 0;
            var _obj = (from _PO in _AcrDBContext.DGIT_ACRHEADER
                        join _PAH in _AcrDBContext.DGIT_ACRAPPHISTORY on _PO.ACRHEADERID equals _PAH.ACRID
                        where _PO.PROCESS_STATUS == 1 && _PAH.APPROVAL_STATUS == 0 && _PAH.ADEMPCODE == ecode
                        select _PO).OrderBy(p => p.DATEADDED).Where(m => m.ACRHEADERID > ACRID).ToList();

            if (_obj != null && _obj.Count > 0)
            {
                retval = _obj.FirstOrDefault().ACRHEADERID; //Return 1 if any of PR approval not finished for related PO
            }
            return retval;
        }
        //Ended by aumento as on 28092024 for the SR82874 ===============================================

        // start added by aumento :: SR111504
        public LibResult AutocompleteSuggestionsVendor(string Key)
        {
            LibResult res = new LibResult();
            dynamic Vendor = null;
            try
            {
                Vendor = (from i in _AcrDBContext.FINVENDORMASTERMST.Where(x => x.VENDORCODE.Contains(Key) || x.NAME1.Contains(Key))
                          select new
                          {
                              i.VENDORCODE,
                              i.NAME1,
                              i.NAME2,
                              i.NAME3
                          })
                          .GroupBy(x => x.VENDORCODE).Select(g => g.First()) //Added by Aumento::  CR7448_V1
                          .Take(200).ToList();

                res.resultObject = new { VendorList = Vendor };
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LibResult AutocompleteSuggestionsPONumber(string Key)
        {
            LibResult res = new LibResult();
            dynamic PONumber = null;
            try
            {
                PONumber = (from i in _AcrDBContext.DGIT_POHEADER.Where(x => x.PONO.Contains(Key))
                            select new
                            {
                                i.PONO
                            })
                            .GroupBy(x => x.PONO).Select(g => g.First()) //Added by Aumento:: CR7448_V1
                            .Take(200).ToList();
                res.resultObject = new { PONumberList = PONumber };
                return res;
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }
        // end added by aumento :: SR111504

    }
}

