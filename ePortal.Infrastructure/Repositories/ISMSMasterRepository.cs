using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ePortal.Infrastructure.Repositories
{
    public class ISMSMasterRepository
    {
        private EPortalDBContext _ISMSMasterDBContext;
        public ISMSMasterRepository(EPortalDBContext objEPortalDBContext)
        {
            _ISMSMasterDBContext = objEPortalDBContext;
        }

        public IEnumerable<Employee_Details> BindAppAuth1()
        {

            long strKIID = (long)_ISMSMasterDBContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
            IEnumerable<Employee_Details> iList;

            var empdata_fundesg = (from userdata in _ISMSMasterDBContext.ADEMPDIVDEPTSECT.Where(m => m.SYKI == strKIID)
                                   join emp in _ISMSMasterDBContext.ADEMPLOYEE.Where(m => m.ACTIVE == 1) on userdata.ADEMPCODE equals emp.ADEMPCODE
                                   join d in _ISMSMasterDBContext.ADDESIGNATION.Where(m => m.ACTIVE == 1) on userdata.ADFUNCTIONALDESIGNATIONID equals d.ADDESIGNATIONID
                                   join fg in _ISMSMasterDBContext.ADFUNCTIONALDESIGNATION.Where(m => m.ACTIVE == 1) on userdata.ADFUNCTIONALDESIGNATIONID equals fg.ADFUNCTIONALDESIGNATIONID into ls
                                   from fg in ls.DefaultIfEmpty()
                                   select new
                                   {
                                       ADEMPCODE = emp.ADEMPCODE,
                                       FIRSTNAME = emp.FIRSTNAME,
                                       LASTNAME = emp.LASTNAME,
                                       _ENAME = emp.FIRSTNAME + " " + emp.LASTNAME,
                                   }
                           ).Distinct().ToList();

            iList = (from userdata in empdata_fundesg
                     select new Employee_Details
                     {
                         _ECode = userdata.ADEMPCODE,
                         _EName = userdata.ADEMPCODE + "-" + userdata.FIRSTNAME + " " + userdata.LASTNAME,
                     }).OrderBy(x => x._ECode).ToList();

            return iList;
        }

        public Int16 SaveProcessAttachment_Trn(ISMSMasterViewModel AVM)
        {
            Int16 retVal = 0;
            using (var transaction = _ISMSMasterDBContext.Database.BeginTransaction())
            {
                try
                {
                    long maxval = _ISMSMasterDBContext.ISMSHDRMST.Max(x => x.SRNO);
                    maxval = maxval + 1;


                    ISMSHDRMST CM = new ISMSHDRMST();
                    
                    CM.SRNO = maxval;
                    CM.ISMS_BLOB = AVM.ISMS_BLOB;
                    CM.ISMS_CONTENTTYPE = AVM.ISMS_CONTENTTYPE;
                    CM.ISMS_UPLOAD = AVM.ISMS_UPLOAD;
                    CM.STATUS = 2;//WIP
                    CM.APP_AUTH1 = AVM.APP_AUTH1;
                    CM.APP_AUTH2 = AVM.APP_AUTH2;
                    CM.CREATED_BY = AVM.CREATED_BY;
                    CM.CREATED_DATE = DateTime.Now;

                    _ISMSMasterDBContext.ISMSHDRMST.Add(CM);
                    _ISMSMasterDBContext.SaveChanges();

                    long maxValCMT = _ISMSMasterDBContext.ISMSHDRDTL.Max(x => x.ISMSMAPPINGID);
                    maxValCMT = maxValCMT + 1;

                    if (CM.APP_AUTH1 != null)
                    {
                        ISMSHDRDTL CPTM = new ISMSHDRDTL();
                        CPTM.ISMSMAPPINGID = maxValCMT;
                        CPTM.ISMS_MST_SRNO = CM.SRNO;
                        CPTM.STATUS = 0;
                        CPTM.APPAUTH_ECODE = CM.APP_AUTH1;                       
                        CPTM.CREATED_BY = long.Parse(CM.CREATED_BY);
                        CPTM.CREATED_DATE = DateTime.Now;
                        CPTM.APP_LEVEL = 1;
                        _ISMSMasterDBContext.ISMSHDRDTL.Add(CPTM);
                        _ISMSMasterDBContext.SaveChanges();
                        maxValCMT = maxValCMT + 1;
                    }

                    if (CM.APP_AUTH2 != null)
                    {
                        ISMSHDRDTL CPTM = new ISMSHDRDTL();
                        CPTM.ISMSMAPPINGID = maxValCMT;
                        CPTM.ISMS_MST_SRNO = CM.SRNO;
                        CPTM.APPAUTH_ECODE = CM.APP_AUTH2;                        
                        CPTM.CREATED_BY = long.Parse(CM.CREATED_BY);
                        CPTM.CREATED_DATE = DateTime.Now;
                        CPTM.APP_LEVEL = 2;
                        _ISMSMasterDBContext.ISMSHDRDTL.Add(CPTM);
                        _ISMSMasterDBContext.SaveChanges();
                        maxValCMT = maxValCMT + 1;
                    }
                    
                    transaction.Commit();
                    retVal = 1;
                    //return AVM;
                }
                catch (Exception ex)
                {
                    retVal = -1;
                    transaction.Rollback();
                }
                return retVal;
            }
        }

        public List<ISMSMasterViewModel> GetISMSMasterList()
        {
            List<ISMSMasterViewModel> iList = new List<ISMSMasterViewModel>();


            var iColl = (from data in _ISMSMasterDBContext.ISMSHDRMST
                         select data).ToList();

            if (iColl.Count > 0)
            {
                foreach (var obj in iColl)
                {

                   
                    var AppStatus = (from v in _ISMSMasterDBContext.ISMSHDRDTL where v.ISMS_MST_SRNO == obj.SRNO && v.APP_LEVEL == 1 select v.STATUS).FirstOrDefault();
                    var AppStatus2 = (from v in _ISMSMasterDBContext.ISMSHDRDTL where v.ISMS_MST_SRNO == obj.SRNO && v.APP_LEVEL == 2 select v.STATUS).FirstOrDefault();
                    var calH = (from v in _ISMSMasterDBContext.ISMSHDRMST where v.SRNO == obj.SRNO select v.STATUS).FirstOrDefault();

                    if (calH == 1 || calH == 0 || calH == 3)
                    {
                        AppStatus = 0;//not dispaly
                    }
                    else if (calH == 2 && (AppStatus == 1 || AppStatus2 == 1))
                    {
                        AppStatus = 0;//not dispaly
                    }
                    else if (calH == 2 && (AppStatus == 2 || AppStatus2 == 2))
                    {
                        AppStatus = 0;//not dispaly
                    }
                    else
                    {
                        AppStatus = 1;//dispaly
                    }
                    

                    var AppAuthDate = (from v in _ISMSMasterDBContext.ISMSHDRDTL where v.ISMS_MST_SRNO == obj.SRNO && v.APP_LEVEL == 2 select v.APPAUTH_DATE).FirstOrDefault();
                    string AppAuthDate1 = "";
                    if (AppAuthDate != null)
                    {
                        AppAuthDate1 = AppAuthDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt");
                    }
                    else
                    {
                        AppAuthDate1 = "Pending For Approval";
                    }

                    iList.Add(new ISMSMasterViewModel
                    {
                        SRNO = obj.SRNO,
                        ISMS_BLOB=obj.ISMS_BLOB,
                        ISMS_UPLOAD = obj.ISMS_UPLOAD,
                        STATUS = obj.STATUS,
                        FLAG = AppStatus,
                        LAST_APPPROVED_ON = AppAuthDate1,
                    });
                }
            }

            return iList;
        }


        public ISMSMasterViewModel GetISMSMasterDetails(int SrNo)
        {
            ISMSMasterViewModel PAM = new ISMSMasterViewModel();
            ISMSHDRMST CMV = new ISMSHDRMST();

            CMV = _ISMSMasterDBContext.ISMSHDRMST.Where(x => x.SRNO == SrNo).SingleOrDefault();          

            var ADEMPCODE = long.Parse(CMV.CREATED_BY);
        
            var APP_AUTH1_NAME = _ISMSMasterDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == CMV.APP_AUTH1).SingleOrDefault();
            var APP_AUTH2_NAME = _ISMSMasterDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == CMV.APP_AUTH2).SingleOrDefault();
            var CREATED_BY_USER = _ISMSMasterDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == ADEMPCODE).SingleOrDefault();            
            

            var iColl = (from data in _ISMSMasterDBContext.ISMSHDRMST
                         select data).ToList();

            PAM.SRNO = CMV.SRNO;         
            PAM.ISMS_BLOB = CMV.ISMS_BLOB;           
            PAM.APP_AUTH1 = CMV.APP_AUTH1;
            PAM.APP_AUTH1_NAME = (CMV.APP_AUTH1 + " - " + APP_AUTH1_NAME.FIRSTNAME + " " + APP_AUTH1_NAME.LASTNAME).ToString();
            var status1 = _ISMSMasterDBContext.ISMSHDRDTL.Where(x => x.APPAUTH_ECODE == CMV.APP_AUTH1 && x.ISMS_MST_SRNO == CMV.SRNO && x.APP_LEVEL == 1).Select(s => s.STATUS).SingleOrDefault();
            if (status1 == 0 || status1 == null)
            {
                PAM.APPAUTH1_REMARKS = " - Pending";
            }
            else if (status1 == 2)
            {
                PAM.APPAUTH1_REMARKS = " - Reject";
            }
            else
            {
                PAM.APPAUTH1_REMARKS = " - Approved";
            }
            PAM.APP_AUTH2 = CMV.APP_AUTH2;
            PAM.APP_AUTH2_NAME = (CMV.APP_AUTH2 + " - " + APP_AUTH2_NAME.FIRSTNAME + " " + APP_AUTH2_NAME.LASTNAME).ToString();
            var status2 = _ISMSMasterDBContext.ISMSHDRDTL.Where(x => x.APPAUTH_ECODE == CMV.APP_AUTH2 && x.ISMS_MST_SRNO == CMV.SRNO && x.APP_LEVEL == 2).Select(s => s.STATUS).SingleOrDefault();
            if (status2 == 0 || status2 == null)
            {
                PAM.APPAUTH2_REMARKS = " - Pending";
            }
            else if (status2 == 2)
            {
                PAM.APPAUTH2_REMARKS = " - Reject";
            }
            else
            {
                PAM.APPAUTH2_REMARKS = " - Approved";
            }
            PAM.ISMS_UPLOAD = CMV.ISMS_UPLOAD;            
            PAM.CREATED_BY = (CMV.CREATED_BY + " - " + CREATED_BY_USER.FIRSTNAME + " " + CREATED_BY_USER.LASTNAME).ToString();
            PAM.strCREATED_DATE = CMV.CREATED_DATE.Value.ToString("dd-MMM-yyyy");            
            PAM.STATUS = CMV.STATUS;
            
            return PAM;
        }

        public FileViewModel GetFileForDownload(Int64 id, string type)
        {
            FileViewModel flvm = new FileViewModel();
            ISMSHDRMST CCPT;
            //var id1 = long.Parse(id);
            CCPT = _ISMSMasterDBContext.ISMSHDRMST.Where(c => c.SRNO == id).SingleOrDefault();
            if (CCPT != null)
            {
                if (type == "ISMS")
                {
                    flvm.FileName = CCPT.ISMS_UPLOAD;
                    flvm.FileContentType = CCPT.ISMS_CONTENTTYPE;
                    flvm.File = CCPT.ISMS_BLOB;
                }
            }
            return flvm;
        }


        public Int16 UpdateProcessAttachment_Trn(ISMSMasterViewModel PAM)
        {
            Int16 retVal = 0;
            using (var transaction = _ISMSMasterDBContext.Database.BeginTransaction())
            {
                try
                {
                    ISMSHDRMST CPT = new ISMSHDRMST();

                    CPT = _ISMSMasterDBContext.ISMSHDRMST.Find(PAM.SRNO);                  
                    CPT.STATUS = 2;//WIP                                  
                    if (PAM.ISMS_UPLOAD != null)
                    {
                        CPT.ISMS_UPLOAD = PAM.ISMS_UPLOAD;
                        CPT.ISMS_CONTENTTYPE = PAM.ISMS_CONTENTTYPE;
                        CPT.ISMS_BLOB = PAM.ISMS_BLOB;
                    }
                    CPT.APP_AUTH1 = PAM.APP_AUTH1;
                    CPT.APP_AUTH2 = PAM.APP_AUTH2;

                    CPT.MODIFIED_DATE = DateTime.Now;
                    CPT.MODIFIED_BY = PAM.MODIFIED_BY;
                    _ISMSMasterDBContext.Entry(CPT).State = EntityState.Modified;
                    _ISMSMasterDBContext.SaveChanges();


                    ISMSHDRDTL CPTM = new ISMSHDRDTL();
                    CPTM = _ISMSMasterDBContext.ISMSHDRDTL.Find(PAM.SRNO);
                    
                    var temp = (from userdata in _ISMSMasterDBContext.ISMSHDRDTL.Where(m => m.ISMS_MST_SRNO == PAM.SRNO)
                                select new
                                {
                                    ISMSMAPPINGID = userdata.ISMSMAPPINGID,
                                    CAL_MAS_SRNO = userdata.ISMS_MST_SRNO,
                                }
                           ).ToList();


                    if (PAM.APP_AUTH1 != null)
                    {
                        CPTM = _ISMSMasterDBContext.ISMSHDRDTL.Find(temp[0].ISMSMAPPINGID);
                        CPTM.ISMS_MST_SRNO = CPT.SRNO;
                        CPTM.STATUS = 0;
                        CPTM.APPAUTH_ECODE = PAM.APP_AUTH1;                        
                        CPTM.MODIFIED_BY = long.Parse(PAM.MODIFIED_BY);
                        CPTM.MODIFIED_DATE = DateTime.Now;
                        CPTM.APP_LEVEL = 1;
                        _ISMSMasterDBContext.Entry(CPTM).State = EntityState.Modified;                       
                        _ISMSMasterDBContext.SaveChanges();
                    }
                    if (PAM.APP_AUTH2 != null)
                    {
                        CPTM = _ISMSMasterDBContext.ISMSHDRDTL.Find(temp[1].ISMSMAPPINGID);
                        CPTM.ISMS_MST_SRNO = CPT.SRNO;                       
                        CPTM.APPAUTH_ECODE = PAM.APP_AUTH2;                       
                        CPTM.MODIFIED_BY = long.Parse(PAM.MODIFIED_BY);
                        CPTM.MODIFIED_DATE = DateTime.Now;
                        CPTM.APP_LEVEL = 2;
                        _ISMSMasterDBContext.Entry(CPTM).State = EntityState.Modified;                        
                        _ISMSMasterDBContext.SaveChanges();
                    }
                    transaction.Commit();
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

        public ISMSMasterViewModel GetISMSMSTRequestById(long id)
        {
            ISMSMasterViewModel PAM = new ISMSMasterViewModel();
            ISMSHDRMST CPM = new ISMSHDRMST();

            ISMSHDRDTL CPAMT = new ISMSHDRDTL();
            ISMSHDRDTL CPAMT1 = new ISMSHDRDTL();
           
            CPM = _ISMSMasterDBContext.ISMSHDRMST.Where(x => x.SRNO == id).SingleOrDefault();
            
            CPAMT = _ISMSMasterDBContext.ISMSHDRDTL.Where(x => x.ISMS_MST_SRNO == id && x.APPAUTH_ECODE == CPM.APP_AUTH1).FirstOrDefault();
            CPAMT1 = _ISMSMasterDBContext.ISMSHDRDTL.Where(x => x.ISMS_MST_SRNO == id && x.APPAUTH_ECODE == CPM.APP_AUTH2).FirstOrDefault();


            PAM.SRNO = CPM.SRNO;
            PAM.STATUS = CPM.STATUS;
            PAM.CREATED_DATE = CPM.CREATED_DATE;
            PAM.CREATED_BY1 = Int64.Parse(CPM.CREATED_BY);
            PAM.CREATED_BY = _ISMSMasterDBContext.ADLOGINUSER.Where(x => x.ADEMPCODE == PAM.CREATED_BY1).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").SingleOrDefault();
            PAM.ISMS_UPLOAD = CPM.ISMS_UPLOAD;            
            PAM.ISMS_CONTENTTYPE = CPM.ISMS_CONTENTTYPE;
            PAM.ISMS_BLOB = CPM.ISMS_BLOB;
            PAM.APP_AUTH1 = CPM.APP_AUTH1;
            PAM.APP_AUTH1_NAME = _ISMSMasterDBContext.ADLOGINUSER.Where(x => x.ADEMPCODE == CPM.APP_AUTH1).Select(s => s.ADEMPCODE + "-" + s.FIRSTNAME + " " + s.LASTNAME + "").SingleOrDefault();

            PAM.APP_AUTH2 = CPM.APP_AUTH2;
            PAM.APP_AUTH2_NAME = _ISMSMasterDBContext.ADLOGINUSER.Where(x => x.ADEMPCODE == CPM.APP_AUTH2).Select(s => s.ADEMPCODE + "-" + s.FIRSTNAME + " " + s.LASTNAME + "").SingleOrDefault();

            PAM.ISMSHDRDTLModel = (from CPAMTDetail in _ISMSMasterDBContext.ISMSHDRDTL.Where(d => d.ISMS_MST_SRNO == CPM.SRNO)
                                            join _AppEmp in _ISMSMasterDBContext.ADEMPLOYEE on CPAMTDetail.APPAUTH_ECODE equals _AppEmp.ADEMPCODE
                                            select new ISMSHDRDTLViewModel
                                            {
                                                APP_LEVEL = CPAMTDetail.APP_LEVEL,
                                                ISMS_MST_SRNO = CPAMTDetail.ISMS_MST_SRNO,
                                                APPAUTH_ECODE = CPAMTDetail.APPAUTH_ECODE,
                                                APPAUTH_NAME = _AppEmp.FIRSTNAME + " " + _AppEmp.LASTNAME + "- [" + _AppEmp.ADEMPCODE + "]",
                                                APPAUTH_DATE = CPAMTDetail.APPAUTH_DATE,
                                                APPAUTH_REMARKS = CPAMTDetail.APPAUTH_REMARKS,
                                                STATUS = CPAMTDetail.STATUS,
                                            }).OrderBy(x => x.APP_LEVEL).ToList();

            return PAM;

        }


        public short ISMSMasterAppr(ISMSHDRDTLViewModel PHVM, Employee_Details emp_dtl)
        {
            short retVal = 0;
            using (var transaction = _ISMSMasterDBContext.Database.BeginTransaction())
            {
                try
                {
                    ISMSHDRDTL DPAH = new ISMSHDRDTL();
                    ISMSHDRDTL DPAH1 = new ISMSHDRDTL();
                    ISMSHDRDTL DPAH2 = new ISMSHDRDTL();
                    ISMSHDRMST PAT = new ISMSHDRMST();
                    if (PHVM.ISMS_MST_SRNO > 0)
                    {
                        DPAH = _ISMSMasterDBContext.ISMSHDRDTL.Where(x => x.ISMS_MST_SRNO == PHVM.ISMS_MST_SRNO && x.APPAUTH_ECODE == emp_dtl._ECode && x.STATUS == 0).FirstOrDefault();

                        if (DPAH != null)
                        {
                            DPAH.APPAUTH_DATE = DateTime.Now;
                            DPAH.APPAUTH_REMARKS = PHVM.APPAUTH_REMARKS;
                            DPAH.MODIFIED_BY = PHVM.MODIFIED_BY;
                            DPAH.MODIFIED_DATE = DateTime.Now;
                            DPAH.STATUS = PHVM.STATUS;
                            _ISMSMasterDBContext.Entry(DPAH).State = EntityState.Modified;
                            _ISMSMasterDBContext.SaveChanges();
                        }

                        var stsl1 = _ISMSMasterDBContext.ISMSHDRDTL.Where(x => x.ISMS_MST_SRNO == PHVM.ISMS_MST_SRNO && x.APPAUTH_ECODE == emp_dtl._ECode && x.ISMSMAPPINGID == DPAH.ISMSMAPPINGID).Select(x => x.ISMSMAPPINGID).FirstOrDefault();

                        var MAPPINGATTACHMENTID = _ISMSMasterDBContext.ISMSHDRDTL.Where(x => x.ISMS_MST_SRNO == PHVM.ISMS_MST_SRNO && x.APPAUTH_ECODE == emp_dtl._ECode && x.ISMSMAPPINGID == DPAH.ISMSMAPPINGID).Select(x => x.ISMSMAPPINGID + 1).FirstOrDefault();


                        DPAH1 = _ISMSMasterDBContext.ISMSHDRDTL.Where(x => x.ISMS_MST_SRNO == PHVM.ISMS_MST_SRNO && x.ISMSMAPPINGID == MAPPINGATTACHMENTID).FirstOrDefault();

                        PAT = _ISMSMasterDBContext.ISMSHDRMST.Where(x => x.SRNO == PHVM.ISMS_MST_SRNO).FirstOrDefault();
                        DPAH2 = _ISMSMasterDBContext.ISMSHDRDTL.Where(x => x.ISMS_MST_SRNO == PHVM.ISMS_MST_SRNO && x.APPAUTH_ECODE == emp_dtl._ECode && x.APP_LEVEL == 2).FirstOrDefault();

                        var CalH1 = _ISMSMasterDBContext.ISMSHDRMST.Where(x => x.ISMS_UPLOAD != PAT.ISMS_UPLOAD).Select(x => x.STATUS == 1).Count();
                        if (CalH1 >= 1)
                        {
                            if (DPAH2 != null)
                            {
                                //if (DPAH.STATUS != 2)
                                if (DPAH2.STATUS == 1)
                                {
                                    var CalH2 = _ISMSMasterDBContext.ISMSHDRMST.Where(x => x.ISMS_UPLOAD != PAT.ISMS_UPLOAD && x.STATUS == 1).ToList();

                                    var itemsToUpdate = from item in CalH2
                                                        where item.STATUS == 1
                                                        select item;

                                    foreach (var item in itemsToUpdate)
                                    {
                                        item.STATUS = 0; // De-Active
                                        _ISMSMasterDBContext.Entry(item).State = EntityState.Modified;
                                        _ISMSMasterDBContext.SaveChanges();
                                    }
                                }
                            }

                        }

                        if (PAT != null)
                        {
                            if (DPAH != null)
                            {
                                if (DPAH1 != null)
                                {
                                    if (DPAH.STATUS != 2)
                                    {
                                        DPAH1.STATUS = 0;//Pending
                                        _ISMSMasterDBContext.Entry(DPAH1).State = EntityState.Modified;
                                        _ISMSMasterDBContext.SaveChanges();
                                    }
                                }

                                if (DPAH.STATUS == 2)
                                {
                                    PAT.STATUS = 0;//De-Active
                                    _ISMSMasterDBContext.Entry(PAT).State = EntityState.Modified;
                                    _ISMSMasterDBContext.SaveChanges();
                                }
                            }

                        }

                        if (PAT != null)
                        {
                            if (DPAH2 != null)
                            {

                                if (DPAH2.STATUS == 1)
                                {
                                    PAT.STATUS = 1;//Active                                    
                                }
                                else if (DPAH2.STATUS == 2)
                                {
                                    PAT.STATUS = 0;//De-Active                                   
                                }
                                else
                                {
                                    PAT.STATUS = 2;//WIP                                    
                                }
                                _ISMSMasterDBContext.Entry(PAT).State = EntityState.Modified;
                                _ISMSMasterDBContext.SaveChanges();
                            }

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

        public short CancelReqById(long id)
        {

            using (var transaction = _ISMSMasterDBContext.Database.BeginTransaction())
            {
                short retVal = 0;
                try
                {
                    ISMSHDRMST CPM = new ISMSHDRMST();
                    CPM = _ISMSMasterDBContext.ISMSHDRMST.Where(x => x.SRNO == id && x.STATUS == 1).SingleOrDefault();

                    if (CPM != null)
                    {
                        CPM.STATUS = 0;
                        _ISMSMasterDBContext.Entry(CPM).State = EntityState.Modified;
                        _ISMSMasterDBContext.SaveChanges();
                    }
                    retVal = 1;
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

        //Added by aumento as on 08082024 for the SR72656 --------------------------------------------------------------------------------
        public string GetActiveLink()
        {

            using (var transaction = _ISMSMasterDBContext.Database.BeginTransaction())
            {
                var retVal = "";
                try
                {
                    //long maxval = _ISMSMasterDBContext.ISMSHDRMST.Where(x => x.STATUS == 1).Select(x => x.SRNO).DefaultIfEmpty(0).Max();
                    // retVal = _ISMSMasterDBContext.ISMSHDRMST.Where(x => x.SRNO == maxval).Select(x=>x.ISMS_UPLOAD).SingleOrDefault().ToString();                    



                    var maxRow = _ISMSMasterDBContext.ISMSHDRMST
                        .Where(x => x.STATUS == 1)
                        .OrderByDescending(x => x.SRNO)
                        .Select(x => new { x.SRNO, x.ISMS_UPLOAD })
                        .FirstOrDefault();

                    long maxval = maxRow?.SRNO ?? 0L;
                     retVal = maxRow?.ISMS_UPLOAD ?? string.Empty;

                }
                catch (Exception ex)
                {
                    retVal = "-1";
                    transaction.Rollback();
                }
                return retVal;
            }

        }
        //--------------------------------------------------------------------------------------------------------------------------------------
    }
}
