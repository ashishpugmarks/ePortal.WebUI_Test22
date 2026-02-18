
using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ePortal.Infrastructure.Repositories
{
    public class CreativeMasterRepository
    {
        private EPortalDBContext _CreativeMasterDBContext;
        CommonRepository cmr;
        public CreativeMasterRepository(EPortalDBContext objEPortalDBContext, CommonRepository objcmr)
        {
            _CreativeMasterDBContext = objEPortalDBContext;
            this.cmr = objcmr;
        }

        // Content Process Master
        // #region Content Process Master
        public IEnumerable<ProcessMstViewModel> BindContentProcess()
        {
            IEnumerable<ProcessMstViewModel> iList;
            //iList = (from data in _CreativeMasterDBContext.CM_PROCESS_MST.ToList().OrderBy(o => o.CREATED_DATE)
            iList = (from data in _CreativeMasterDBContext.CM_PROCESS_MST.Where(x => x.MSTPROCESSID == 1)
                     select new ProcessMstViewModel
                     {
                         PROCESSID = data.MSTPROCESSID,
                         PROCESS_NAME = data.PROCESS_NAME,
                         Upload_Banner = data.PROCESS_BANNER
                     }).ToList();
            return iList;
        }
        //#endregion
        // CorporateNews
        #region CorporateNews Master
        public List<CreativeMasterViewModel> GetCorporateNewsMasterList() //Aumento
        {
            
            string Processids1 = cmr.GetParameterValue("CORPORATENEWS_PRCID");
            string[] values = Processids1.Split(',');
            List<long> Processids = new List<long>();
            //for (int i = 0; i < values.Length; i++)
            //{
            //    long lngval = Convert.ToInt32(values[i].Trim().ToString().Split('~')[1]);
            //    Processids.Add(lngval);
            //}
            Processids.Add(1);
            List<CreativeMasterViewModel> iList = new List<CreativeMasterViewModel>();
            var iColl = (from data in _CreativeMasterDBContext.CM_PROCESSATTACHMENT_TRN
                             //join AD_User in _CorporateNewsDBContext.ADLOGINUSER on data.CREATED_BY equals AD_User.ADEMPCODE
                         where Processids.Contains(data.PROCESSID)
                         orderby data.CREATED_DATE descending
                         select new
                         {
                             data.ATTACHMENTID,
                             data.PROCESSID,
                             data.SUBJECT,
                             data.DESCRIPTION,
                             data.BRIEF,
                             data.STATUS,
                             data.START_DATE,
                             data.END_DATE,
                             data.CREATED_DATE,
                             //AD_User.AD_User.FIRSTNAME + " " + AD_User.LASTNAME
                         }).ToList();
            if (iColl.Count > 0)
            {
                foreach (var obj in iColl)
                {
                    iList.Add(new CreativeMasterViewModel
                    {
                        ATTACHMENTID = obj.ATTACHMENTID,
                        PROCESSID = obj.PROCESSID,
                        SUBJECT = CommonRepository.HtmlToText(obj.SUBJECT),
                        BRIEF = CommonRepository.HtmlToText(obj.BRIEF),
                        //DESCRIPTION = CommonRepository.HtmlToText(obj.DESCRIPTION),
                        START_DATE = obj.START_DATE.ToString("dd-MMM-yyyy"),
                        END_DATE = obj.END_DATE.ToString("dd-MMM-yyyy"),
                        STATUS = obj.STATUS,
                        CreateDate = obj.CREATED_DATE.ToString("dd-MMM-yyyy"),
                    });
                }
            }
            return iList;
        }
        #endregion

        public Int16 SaveProcessAttachment_Trn(CreativeMasterViewModel AVM)
        {
            Int16 retVal = 0;
            using (var transaction = _CreativeMasterDBContext.Database.BeginTransaction())
            {
                try
                {
                    long maxval = _CreativeMasterDBContext
    .CM_PROCESSATTACHMENTAPPMAPPING_TRN
    .Select(x => (long?)x.MAPPINGATTACHMENTID)
    .Max() ?? 0;
    maxval = maxval + 1;


                    CM_PROCESSATTACHMENT_TRN CPT = new CM_PROCESSATTACHMENT_TRN();
                    CPT.PROCESSID = AVM.PROCESSID;
                    CPT.START_DATE = DateTime.ParseExact(AVM.START_DATE, "dd-MMM-yyyy", null);
                    CPT.END_DATE = DateTime.ParseExact(AVM.END_DATE, "dd-MMM-yyyy", null);
                    CPT.SUBJECT = CommonRepository.TextToHtml(AVM.SUBJECT);
                    CPT.BRIEF = CommonRepository.TextToHtml(AVM.BRIEF);
                    //CPT.DESCRIPTION = CommonRepository.TextToHtml(AVM.DESCRIPTION);
                    CPT.STATUS = 2; // status 2 = WIP
                    CPT.BANNER_NAME = AVM.BANNER_NAME;
                    CPT.BANNER_CONTENTTYPE = AVM.BANNER_CONTENTTYPE;
                    CPT.BANNER = AVM.BANNER;
                    CPT.ATTACHMENT1_NAME = AVM.ATTACHMENT1_NAME;
                    CPT.ATTACHMENT1_CONTENTTYPE = AVM.ATTACHMENT1_CONTENTTYPE;
                    CPT.ATTACHMENT1 = AVM.ATTACHMENT1;
                    //CPT.ATTACHMENT2_NAME = AVM.ATTACHMENT2_NAME;
                    //CPT.ATTACHMENT2_CONTENTTYPE = AVM.ATTACHMENT2_CONTENTTYPE;
                    //CPT.ATTACHMENT2 = AVM.ATTACHMENT2;
                    CPT.CREATED_DATE = DateTime.Now;
                    CPT.CREATED_BY = AVM.CREATED_BY;
                    _CreativeMasterDBContext.CM_PROCESSATTACHMENT_TRN.Add(CPT);
                    _CreativeMasterDBContext.SaveChanges();
                    // Save for attachment approval
                    CreativeMasterViewModel AAVM = new CreativeMasterViewModel();
                    AAVM.ATTACHMENTID = CPT.ATTACHMENTID;
                    AAVM.INITIATED_BY = CPT.CREATED_BY;
                    AAVM.APPAUTH1_ECODE = AVM.APPAUTH1_ECODE;
                    //string[] parts = AVM.APPAUTH1_ECODE.Split('-');
                    AAVM.APPAUTH2_ECODE = AVM.APPAUTH2_ECODE;
                    AAVM.INITIATOR_REMARKS = AVM.INITIATOR_REMARKS;
                    SaveAttachmentApproval_Trn(AAVM);
                    if (AVM.APPAUTH1_ECODE != null)
                    {
                        CM_PROCESSATTACHMENTAPPMAPPING_TRN CPTM = new CM_PROCESSATTACHMENTAPPMAPPING_TRN();
                        CPTM.MAPPINGATTACHMENTID = maxval;
                        CPTM.ATTACHMENTID = CPT.ATTACHMENTID;// CPT.ATTACHMENTID;

                        CPTM.STATUS = 0;
                        CPTM.APPAUTH_ECODE = AVM.APPAUTH1_ECODE;
                        //CPTM.APPAUTH_DATE = DateTime.Now;
                        //CPTM.APPAUTH_REMARKS = AVM.INITIATOR_REMARKS;
                        CPTM.CREATED_BY = AVM.CREATED_BY;
                        CPTM.CREATED_DATE = DateTime.Now;
                        CPTM.APP_LEVEL = 1;
                        _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Add(CPTM);
                        _CreativeMasterDBContext.SaveChanges();
                        maxval = maxval + 1;
                    }
                    if (AVM.APPAUTH2_ECODE != null)
                    {
                        CM_PROCESSATTACHMENTAPPMAPPING_TRN CPTM = new CM_PROCESSATTACHMENTAPPMAPPING_TRN();
                        CPTM.MAPPINGATTACHMENTID = maxval;
                        CPTM.ATTACHMENTID = CPT.ATTACHMENTID;
                        //CPTM.STATUS = 0;
                        CPTM.APPAUTH_ECODE = AVM.APPAUTH2_ECODE;
                        //CPTM.APPAUTH_DATE = DateTime.Now;
                        //CPTM.APPAUTH_REMARKS = AVM.INITIATOR_REMARKS;
                        CPTM.CREATED_BY = AVM.CREATED_BY;
                        CPTM.CREATED_DATE = DateTime.Now;
                        CPTM.APP_LEVEL = 2;
                        _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Add(CPTM);
                        _CreativeMasterDBContext.SaveChanges();
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

        public Int16 UpdateProcessAttachment_Trn(CreativeMasterViewModel PAM)
        {
            Int16 retVal = 0;
            using (var transaction = _CreativeMasterDBContext.Database.BeginTransaction())
            {
                try
                {
                    var status = 0;
                    var iList = _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Where(x => x.ATTACHMENTID == PAM.ATTACHMENTID).ToList();

                    foreach (var obj in iList)
                    {
                        if (obj.STATUS == 1)
                        {
                            status = 1;
                            break;
                        }

                    }
                    if (status != 1)
                    {
                        CM_PROCESSATTACHMENT_TRN CPT = new CM_PROCESSATTACHMENT_TRN();

                        CPT = _CreativeMasterDBContext.CM_PROCESSATTACHMENT_TRN.Find(PAM.ATTACHMENTID);
                        CPT.PROCESSID = PAM.PROCESSID;
                        CPT.START_DATE = Convert.ToDateTime(PAM.START_DATE);
                        CPT.END_DATE = Convert.ToDateTime(PAM.END_DATE);
                        CPT.SUBJECT = CommonRepository.TextToHtml(PAM.SUBJECT);
                        CPT.BRIEF = CommonRepository.TextToHtml(PAM.BRIEF);
                        //CPT.DESCRIPTION = CommonRepository.TextToHtml(PAM.DESCRIPTION);
                        //CPT.STATUS = true;
                        if (PAM.BANNER != null)
                        {
                            CPT.BANNER_NAME = PAM.BANNER_NAME;
                            CPT.BANNER_CONTENTTYPE = PAM.BANNER_CONTENTTYPE;
                            CPT.BANNER = PAM.BANNER;
                        }
                        if (PAM.ATTACHMENT1 != null)
                        {
                            CPT.ATTACHMENT1_NAME = PAM.ATTACHMENT1_NAME;
                            CPT.ATTACHMENT1_CONTENTTYPE = PAM.ATTACHMENT1_CONTENTTYPE;
                            CPT.ATTACHMENT1 = PAM.ATTACHMENT1;
                        }
                        //if (PAM.ATTACHMENT2 != null)
                        //{
                        //    CPT.ATTACHMENT2_NAME = PAM.ATTACHMENT2_NAME;
                        //    CPT.ATTACHMENT2_CONTENTTYPE = PAM.ATTACHMENT2_CONTENTTYPE;
                        //    CPT.ATTACHMENT2 = PAM.ATTACHMENT2;
                        //}
                        CPT.MODIFIED_DATE = DateTime.Now;
                        CPT.MODIFIED_BY = PAM.MODIFIED_BY;
                        _CreativeMasterDBContext.Entry(CPT).State = EntityState.Modified;
                        _CreativeMasterDBContext.SaveChanges();

                        /// Save for AttachmentApproval       
                        CreativeMasterViewModel AAVM = new CreativeMasterViewModel();
                        AAVM.ATTACHMENTID = CPT.ATTACHMENTID;
                        AAVM.INITIATED_BY = CPT.CREATED_BY;
                        AAVM.APPAUTH1_ECODE = PAM.APPAUTH1_ECODE;
                        AAVM.APPAUTH2_ECODE = PAM.APPAUTH2_ECODE;
                        AAVM.INITIATOR_REMARKS = PAM.INITIATOR_REMARKS;
                        SaveAttachmentApproval_Trn(AAVM);
                        CM_PROCESSATTACHMENTAPPMAPPING_TRN CPTM = new CM_PROCESSATTACHMENTAPPMAPPING_TRN();
                        //CPTM = _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Find(PAM.ATTACHMENTID);

                        //List<CreativeMasterViewModel> iList = new List<CreativeMasterViewModel>();
                        var temp = (from userdata in _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Where(m => m.ATTACHMENTID == PAM.ATTACHMENTID)
                                    select new
                                    {
                                        MAPPINGATTACHMENTID = userdata.MAPPINGATTACHMENTID,
                                        ATTACHMENTID = userdata.ATTACHMENTID,
                                    }
                               ).ToList();


                        if (PAM.APPAUTH1_ECODE != null)
                        {
                            CPTM = _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Find(temp[0].MAPPINGATTACHMENTID);
                            CPTM.ATTACHMENTID = CPT.ATTACHMENTID;
                            CPTM.STATUS = 0;
                            CPTM.APPAUTH_ECODE = PAM.APPAUTH1_ECODE;
                            //CPTM.APPAUTH_DATE = DateTime.Now;
                            //CPTM.APPAUTH_REMARKS = PAM.INITIATOR_REMARKS;
                            CPTM.MODIFIED_BY = PAM.MODIFIED_BY;
                            CPTM.MODIFIED_DATE = DateTime.Now;
                            CPTM.APP_LEVEL = 1;
                            _CreativeMasterDBContext.Entry(CPTM).State = EntityState.Modified;
                            // _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Add(CPTM);
                            _CreativeMasterDBContext.SaveChanges();
                        }
                        if (PAM.APPAUTH2_ECODE != null)
                        {
                            CPTM = _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Find(temp[1].MAPPINGATTACHMENTID);
                            CPTM.ATTACHMENTID = CPT.ATTACHMENTID;
                            //CPTM.STATUS = 0;
                            CPTM.APPAUTH_ECODE = PAM.APPAUTH2_ECODE;
                            //CPTM.APPAUTH_DATE = DateTime.Now;
                            //CPTM.APPAUTH_REMARKS = PAM.INITIATOR_REMARKS;
                            CPTM.MODIFIED_BY = PAM.MODIFIED_BY;
                            CPTM.MODIFIED_DATE = DateTime.Now;
                            CPTM.APP_LEVEL = 2;
                            _CreativeMasterDBContext.Entry(CPTM).State = EntityState.Modified;
                            //_CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Add(CPTM);
                            _CreativeMasterDBContext.SaveChanges();
                        }
                        transaction.Commit();
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
                return retVal;
            }
        }

        public CreativeMasterViewModel GetCorporateNewsDetails(int id)
        {
            CreativeMasterViewModel PAM = new CreativeMasterViewModel();
            CM_PROCESSATTACHMENT_TRN CPM = new CM_PROCESSATTACHMENT_TRN();
            CM_PROCESSATTACHMENTAPP_TRN CPAM = new CM_PROCESSATTACHMENTAPP_TRN();
            //CPM = _CreativeMasterDBContext.CM_PROCESSATTACHMENT_TRN.Include("CM_PROCESS_MST").Where(x => x.ATTACHMENTID == id).SingleOrDefault();
            CPM = _CreativeMasterDBContext.CM_PROCESSATTACHMENT_TRN.Where(x => x.ATTACHMENTID == id).SingleOrDefault();
            CPAM = _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPP_TRN.Where(x => x.ATTACHMENTID == id).SingleOrDefault();
            CM_PROCESS_MST CPROCESS = _CreativeMasterDBContext.CM_PROCESS_MST.Where(x => x.MSTPROCESSID == CPM.PROCESSID).SingleOrDefault();


            PAM.ATTACHMENTID = CPM.ATTACHMENTID;
            PAM.PROCESSID = CPM.PROCESSID;
            PAM.CM_PROCESS_MST = new ProcessMstViewModel
            {
                //PROCESS_NAME = CPM.CM_PROCESS_MST.PROCESS_NAME,
                PROCESS_NAME = CPROCESS.PROCESS_NAME
            };
            PAM.STATUS = CPM.STATUS;
            PAM.CREATED_DATE = CPM.CREATED_DATE;
            PAM.User_Name = _CreativeMasterDBContext.ADLOGINUSER.Where(x => x.ADEMPCODE == CPM.CREATED_BY).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "(" + s.ADEMPCODE + ")").SingleOrDefault();
            PAM.SUBJECT = CommonRepository.HtmlToText(CPM.SUBJECT);
            PAM.BRIEF = CommonRepository.HtmlToText(CPM.BRIEF);
            //PAM.DESCRIPTION = CommonRepository.HtmlToText(CPM.DESCRIPTION);
            PAM.DESCRIPTION = CPM.DESCRIPTION;
            PAM.START_DATE = CPM.START_DATE.ToString("dd-MMM-yyyy");
            PAM.END_DATE = CPM.END_DATE.ToString("dd-MMM-yyyy");

            PAM.BANNER_NAME = CPM.BANNER_NAME;
            PAM.BANNER_CONTENTTYPE = CPM.BANNER_CONTENTTYPE;
            PAM.BANNER = CPM.BANNER;

            PAM.ATTACHMENT1_NAME = CPM.ATTACHMENT1_NAME;
            PAM.ATTACHMENT1_CONTENTTYPE = CPM.ATTACHMENT1_CONTENTTYPE;
            PAM.ATTACHMENT1 = CPM.ATTACHMENT1;

            PAM.ATTACHMENT2_NAME = CPM.ATTACHMENT2_NAME;
            PAM.ATTACHMENT2_CONTENTTYPE = CPM.ATTACHMENT2_CONTENTTYPE;
            PAM.ATTACHMENT2 = CPM.ATTACHMENT2;
            if(CPAM!= null)
            { 
                PAM.APPAUTH1_ECODE = CPAM.APPAUTH1_ECODE;
                var status1 = _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Where(x => x.APPAUTH_ECODE == CPAM.APPAUTH1_ECODE && x.ATTACHMENTID == CPM.ATTACHMENTID && x.APP_LEVEL==1 ).Select(s => s.STATUS).SingleOrDefault();
                if (status1 == 0 || status1 == null)
                {
                    PAM.AppAuth1_status = " - Pending";
                }
                else if(status1 == 2)
                {
                    PAM.AppAuth1_status = " - Reject";
                }
                else
                {
                    PAM.AppAuth1_status = " - Approved";
                }

                PAM.APPAUTH1_User = _CreativeMasterDBContext.ADLOGINUSER.Where(x => x.ADEMPCODE == CPAM.APPAUTH1_ECODE).Select(s => s.ADEMPCODE + "-" + s.FIRSTNAME + " " + s.LASTNAME + "").SingleOrDefault();
                //PAM.APPAUTH1_User = CPAM.APPAUTH1_ECODE+" "+ CPAM.APPAUTH1_ECODE.ToString();
                PAM.APPAUTH2_ECODE = CPAM.APPAUTH2_ECODE;
                var status2 = _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Where(x => x.APPAUTH_ECODE == CPAM.APPAUTH2_ECODE && x.ATTACHMENTID == CPM.ATTACHMENTID && x.APP_LEVEL == 2).Select(s => s.STATUS).SingleOrDefault();
                if (status2 == 0 || status2 == null)
                {
                    PAM.AppAuth2_status = " - Pending";
                }
                else if (status2 == 2)
                {
                    PAM.AppAuth2_status = " - Reject";
                }
                else
                {
                    PAM.AppAuth2_status = " - Approved";
                }
                PAM.APPAUTH2_User = _CreativeMasterDBContext.ADLOGINUSER.Where(x => x.ADEMPCODE == CPAM.APPAUTH2_ECODE).Select(s => s.ADEMPCODE + "-" + s.FIRSTNAME + " " + s.LASTNAME + "").SingleOrDefault();
            }
            //PAM.APPAUTH2_User = CPAM.APPAUTH2_ECODE +" "+CPAM.APPAUTH2_ECODE.ToString();
            PAM.INITIATOR_REMARKS = _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPP_TRN.Where(x => x.ATTACHMENTID == CPM.ATTACHMENTID).Select(s => s.INITIATOR_REMARKS).SingleOrDefault();
            return PAM;
        }

        //public CreativeMasterViewModel GetCreativeMSTRequestById(long id)
        //{
        //    var _obj = (from CPM in _CreativeMasterDBContext.CM_PROCESSATTACHMENT_TRN.Include("CM_PROCESS_MST").Where(x => x.ATTACHMENTID == id)
        //                join CPAM in _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPP_TRN on CPM.ATTACHMENTID equals CPAM.ATTACHMENTID
        //                join CPAMT in _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN on CPM.ATTACHMENTID equals CPAMT.ATTACHMENTID
        //                //where _VWAssociate.SYKI == _Syki.SYKIID
        //                select new CreativeMasterViewModel
        //                {
        //                    ATTACHMENTID = CPM.ATTACHMENTID,
        //                    PROCESSID = CPM.PROCESSID,
        //                    CM_PROCESS_MST = new ProcessMstViewModel
        //                    {
        //                        PROCESS_NAME = CPM.CM_PROCESS_MST.PROCESS_NAME,
        //                    },
        //                    STATUS = CPM.STATUS,
        //                    CREATED_DATE = CPM.CREATED_DATE,
        //                    CREATED_BY = CPM.CREATED_BY,
        //                    User_Name = _CreativeMasterDBContext.ADLOGINUSER.Where(x => x.ADEMPCODE == CPM.CREATED_BY).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").SingleOrDefault(),
        //                    SUBJECT = CommonRepository.HtmlToText(CPM.SUBJECT),
        //                    BRIEF = CommonRepository.HtmlToText(CPM.BRIEF),
        //                    DESCRIPTION = CPM.DESCRIPTION,
        //                    START_DATE = CPM.START_DATE.ToString("dd-MMM-yyyy"),
        //                    END_DATE = CPM.END_DATE.ToString("dd-MMM-yyyy"),

        //                    BANNER_NAME = CPM.BANNER_NAME,
        //                    BANNER_CONTENTTYPE = CPM.BANNER_CONTENTTYPE,
        //                    BANNER = CPM.BANNER,

        //                    ATTACHMENT1_NAME = CPM.ATTACHMENT1_NAME,
        //                    ATTACHMENT1_CONTENTTYPE = CPM.ATTACHMENT1_CONTENTTYPE,
        //                    ATTACHMENT1 = CPM.ATTACHMENT1,

        //                    ATTACHMENT2_NAME = CPM.ATTACHMENT2_NAME,
        //                    ATTACHMENT2_CONTENTTYPE = CPM.ATTACHMENT2_CONTENTTYPE,
        //                    ATTACHMENT2 = CPM.ATTACHMENT2,
        //                    APPAUTH1_ECODE = CPAM.APPAUTH1_ECODE,
        //                    APPAUTH1_User = _CreativeMasterDBContext.ADLOGINUSER.Where(x => x.ADEMPCODE == CPAM.APPAUTH1_ECODE).Select(s => s.ADEMPCODE + "-" + s.FIRSTNAME + " " + s.LASTNAME + "").SingleOrDefault(),

        //                    APPAUTH2_ECODE = CPAM.APPAUTH2_ECODE,
        //                    APPAUTH2_User = _CreativeMasterDBContext.ADLOGINUSER.Where(x => x.ADEMPCODE == CPAM.APPAUTH2_ECODE).Select(s => s.ADEMPCODE + "-" + s.FIRSTNAME + " " + s.LASTNAME + "").SingleOrDefault(),

        //                    INITIATOR_REMARKS = _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPP_TRN.Where(x => x.ATTACHMENTID == CPM.ATTACHMENTID).Select(s => s.INITIATOR_REMARKS).SingleOrDefault(),

        //                    CM_Processattachmentappmapping_Trn = (from CPAMTDetail in _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Where(d => d.ATTACHMENTID == CPM.ATTACHMENTID)
        //                                                              // where _IOMDetail.STATUS == 1
        //                                                          select new CM_Processattachmentappmapping_TrnViewModel
        //                                                          {
        //                                                              APP_LEVEL = CPAMTDetail.APP_LEVEL,
        //                                                              ATTACHMENTID = CPAMTDetail.ATTACHMENTID,
        //                                                              APPAUTH_ECODE = CPAMTDetail.APPAUTH_ECODE,
        //                                                              APPAUTH_DATE = CPAMTDetail.APPAUTH_DATE,
        //                                                              APPAUTH_REMARKS = CPAMTDetail.APPAUTH_REMARKS,
        //                                                              STATUS = CPAMTDetail.STATUS,
        //                                                          }).OrderBy(o => o.APP_LEVEL).ToList(),

        //                    //iomDetail = (from _IOMDetail in _IOMDBContext.DGIT_IOMDETAIL.Where(d => d.IOMHEADERID == data.IOMHEADERID)
        //                    //             where _IOMDetail.STATUS == 1
        //                    //             select new IOMDetailViewModel
        //                    //             {
        //                    //                 IOMDTL_ID = _IOMDetail.IOMDTL_ID,
        //                    //                 IOMHEADERID = _IOMDetail.IOMHEADERID,
        //                    //                 DOC_TYPE = _IOMDetail.DOC_TYPE,
        //                    //                 ADDITIONAL_INFO = _IOMDetail.ADDITIONAL_INFO,
        //                    //                 FILENAME = _IOMDetail.FILENAME,
        //                    //             }).ToList(),
        //                    //Emp_Detail = new Employee_Details
        //                    //{
        //                    //    _ECode = _AddBy.ADEMPCODE,
        //                    //    _EName = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
        //                    //    _EmailId = _AddBy.EMAILID,
        //                    //    _DOB = DateTime.Now,
        //                    //    _SecDescrip = _VWAssociate.SECTION,
        //                    //    _DepDesc = _VWAssociate.DEPARTMENT,
        //                    //    _DivDesc = _VWAssociate.DIVISION,
        //                    //    _OpDesc = _VWAssociate.OPERATION,
        //                    //    _SiteId = _VWAssociate.SYSITEID
        //                    //},
        //                    //iomAuthSeq = (from _POSeq in _IOMDBContext.DGIT_IOMAPPAUTHSEQ.Where(x => x.IOMHEADERID == data.IOMHEADERID)
        //                    //              join _AppSeqEmp in _IOMDBContext.ADEMPLOYEE on _POSeq.ADEMPCODE equals _AppSeqEmp.ADEMPCODE
        //                    //              join _Vw in _IOMDBContext.VW_ASSOCIATELVLDETAILS on _POSeq.ADEMPCODE equals _Vw.ADEMPCODE
        //                    //              join _Desg in _IOMDBContext.ADDESIGNATION on _Vw.ADDESIGNATIONID equals _Desg.ADDESIGNATIONID
        //                    //              where _Vw.SYKI == _Syki.SYKIID
        //                    //              select new IOMAppAuthSeqViewModel
        //                    //              {
        //                    //                  IOMAPPAUTH_ID = _POSeq.IOMAPPAUTH_ID,
        //                    //                  IOMID = _POSeq.IOMHEADERID,
        //                    //                  ADEMPCODE = _POSeq.ADEMPCODE,
        //                    //                  STATUS = _POSeq.STATUS,
        //                    //                  APP_SEQ = _POSeq.APP_SEQ,
        //                    //                  Header = _POSeq.IOMAPPHEADER,
        //                    //                  ADEMPNAME = _AppSeqEmp.FIRSTNAME + " " + _AppSeqEmp.LASTNAME,
        //                    //                  ADDESIGNATION = _Desg.DESCRIP,
        //                    //                  ADDEDDATE = _POSeq.ADDEDDATE,
        //                    //                  UPDATEBY = _POSeq.UPDATEBY,
        //                    //                  UPDATEDATE = _POSeq.UPDATEDATE,
        //                    //                  APPTYPE = _POSeq.APPTYPE,
        //                    //              }).OrderBy(b => b.APP_SEQ).ToList(),
        //                    //iomAppHis = (from _POAppHis in _IOMDBContext.DGIT_IOMAPPHISTORY.Where(x => x.IOMHEADERID == data.IOMHEADERID)
        //                    //             join _AppEmp in _IOMDBContext.ADEMPLOYEE on _POAppHis.ADEMPCODE equals _AppEmp.ADEMPCODE
        //                    //             select new IOMAppHistoryViewModel
        //                    //             {
        //                    //                 IOMAPPHISTORY_ID = _POAppHis.IOMAPPHISTORY_ID,
        //                    //                 IOMID = _POAppHis.IOMHEADERID,
        //                    //                 ADEMPCODE = _POAppHis.ADEMPCODE,
        //                    //                 APPROVAL_STATUS = _POAppHis.APPROVAL_STATUS,
        //                    //                 APPROVAL_REMARK = _POAppHis.APPROVAL_REMARK,
        //                    //                 APPEMP_NAME = _AppEmp.FIRSTNAME + " " + _AppEmp.LASTNAME + " - [" + _AppEmp.ADEMPCODE + "]",
        //                    //                 APP_EMAIL = _AppEmp.EMAILID,
        //                    //                 ADDEDDATE = _POAppHis.ADDEDDATE,
        //                    //                 UPDATEBY = _POAppHis.UPDATEBY,
        //                    //                 UPDATEDATE = _POAppHis.UPDATEDATE,
        //                    //                 APPROVALDATE = _POAppHis.APP_DATE
        //                    //             }).OrderBy(o => o.IOMAPPHISTORY_ID).ToList(),
        //                    //iomAppHeaderList = (from _AppHeader in _IOMDBContext.DGIT_IOMAPPHEADER.Where(x => x.IOMHEADERID == data.IOMHEADERID)
        //                    //                    select new IOMAppHeaderViewModel
        //                    //                    {
        //                    //                        IOMAPPHEADERID = _AppHeader.IOMAPPHEADERID,
        //                    //                        IOMHEADERID = _AppHeader.IOMHEADERID,
        //                    //                        IOMAPPHEADER = _AppHeader.IOMAPPHEADER,
        //                    //                        APPHEADERDESC = _AppHeader.APPHEADERDESC,
        //                    //                        STATUS = _AppHeader.STATUS,
        //                    //                        ADDEDBY = _AppHeader.ADDEDBY,
        //                    //                        ADDEDDATE = _AppHeader.ADDEDDATE,
        //                    //                        UPDATEBY = _AppHeader.UPDATEBY,
        //                    //                        UPDATEDATE = _AppHeader.UPDATEDATE,
        //                    //                    }).OrderBy(b => b.IOMAPPHEADERID).ToList(),
        //                }).FirstOrDefault();

        //    if (_obj.CM_Processattachmentappmapping_Trn.Count > 0)
        //    {
        //        long lastSendBackAppHisId = 0;
        //        CM_PROCESSATTACHMENTAPPMAPPING_TRN lastSendBackAppHis = _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Where(e => e.ATTACHMENTID == _obj.ATTACHMENTID && e.STATUS == 2).OrderByDescending(o => o.APP_LEVEL).FirstOrDefault();
        //        if (lastSendBackAppHis != null)
        //        {
        //            lastSendBackAppHisId = lastSendBackAppHis.ATTACHMENTID;
        //        }
        //        foreach (CM_Processattachmentappmapping_TrnViewModel obj in _obj.CM_Processattachmentappmapping_Trn)
        //        {
        //            bool IsBeforeSendBackRecord = _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Any(w => w.ATTACHMENTID == obj.ATTACHMENTID && w.ATTACHMENTID <= lastSendBackAppHisId);
        //            if ((IsBeforeSendBackRecord ? (!_obj.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Any(r => r.IOMID == obj.IOMID && r.ADEMPCODE == obj.ADEMPCODE && r.IOMAPPHISTORY_ID > lastSendBackAppHisId)) : (!_obj.iomAppHis.Any(x => x.IOMID == obj.IOMID && x.ADEMPCODE == obj.ADEMPCODE))))
        //            {
        //                _obj.CM_Processattachmentappmapping_TrnViewModel.Add(new CM_Processattachmentappmapping_TrnViewModel
        //                {
        //                    IOMAPPHISTORY_ID = 0,
        //                    IOMID = obj.IOMID,
        //                    ADEMPCODE = obj.ADEMPCODE,
        //                    APPROVAL_STATUS = 0,
        //                    APPROVAL_REMARK = "",
        //                    APPEMP_NAME = obj.ADEMPNAME + "[" + obj.ADEMPCODE + "]",
        //                });
        //            }
        //        }
        //    }
        //    return _obj;

        //}


        public CreativeMasterViewModel GetCreativeMSTRequestById(long id)
        {
            CreativeMasterViewModel PAM = new CreativeMasterViewModel();
            CM_PROCESSATTACHMENT_TRN CPM = new CM_PROCESSATTACHMENT_TRN();
            CM_PROCESSATTACHMENTAPP_TRN CPAM = new CM_PROCESSATTACHMENTAPP_TRN();
            CM_PROCESSATTACHMENTAPPMAPPING_TRN CPAMT = new CM_PROCESSATTACHMENTAPPMAPPING_TRN();
            CM_PROCESSATTACHMENTAPPMAPPING_TRN CPAMT1 = new CM_PROCESSATTACHMENTAPPMAPPING_TRN();
            CPM = _CreativeMasterDBContext.CM_PROCESSATTACHMENT_TRN.Where(x => x.ATTACHMENTID == id).SingleOrDefault();
            CPAM = _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPP_TRN.Where(x => x.ATTACHMENTID == id).SingleOrDefault();
            CPAMT = _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Where(x => x.ATTACHMENTID == id && x.APPAUTH_ECODE == CPAM.APPAUTH1_ECODE ).FirstOrDefault();
            CPAMT1 = _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Where(x => x.ATTACHMENTID == id && x.APPAUTH_ECODE == CPAM.APPAUTH2_ECODE ).FirstOrDefault();
            CM_PROCESS_MST CPROCESS = _CreativeMasterDBContext.CM_PROCESS_MST.Where(x => x.MSTPROCESSID == CPM.PROCESSID).SingleOrDefault();



            PAM.ATTACHMENTID = CPM.ATTACHMENTID;
            PAM.PROCESSID = CPM.PROCESSID;
            PAM.CM_PROCESS_MST = new ProcessMstViewModel
            {
                //PROCESS_NAME = CPM.CM_PROCESS_MST.PROCESS_NAME,
                PROCESS_NAME= CPROCESS.PROCESS_NAME
            };
            PAM.STATUS = CPM.STATUS;
            PAM.CREATED_DATE = CPM.CREATED_DATE;
            PAM.CREATED_BY = CPM.CREATED_BY;
            PAM.User_Name = _CreativeMasterDBContext.ADLOGINUSER.Where(x => x.ADEMPCODE == CPM.CREATED_BY).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "").SingleOrDefault();
            PAM.SUBJECT = CommonRepository.HtmlToText(CPM.SUBJECT);
            PAM.BRIEF = CommonRepository.HtmlToText(CPM.BRIEF);
            //PAM.DESCRIPTION = CommonRepository.HtmlToText(CPM.DESCRIPTION);
            PAM.DESCRIPTION = CPM.DESCRIPTION;
            PAM.START_DATE = CPM.START_DATE.ToString("dd-MMM-yyyy");
            PAM.END_DATE = CPM.END_DATE.ToString("dd-MMM-yyyy");

            PAM.BANNER_NAME = CPM.BANNER_NAME;
            PAM.BANNER_CONTENTTYPE = CPM.BANNER_CONTENTTYPE;
            PAM.BANNER = CPM.BANNER;

            PAM.ATTACHMENT1_NAME = CPM.ATTACHMENT1_NAME;
            PAM.ATTACHMENT1_CONTENTTYPE = CPM.ATTACHMENT1_CONTENTTYPE;
            PAM.ATTACHMENT1 = CPM.ATTACHMENT1;

            PAM.ATTACHMENT2_NAME = CPM.ATTACHMENT2_NAME;
            PAM.ATTACHMENT2_CONTENTTYPE = CPM.ATTACHMENT2_CONTENTTYPE;
            PAM.ATTACHMENT2 = CPM.ATTACHMENT2;
            PAM.APPAUTH1_ECODE = CPAM.APPAUTH1_ECODE;
            PAM.APPAUTH1_User = _CreativeMasterDBContext.ADLOGINUSER.Where(x => x.ADEMPCODE == CPAM.APPAUTH1_ECODE).Select(s => s.ADEMPCODE + "-" + s.FIRSTNAME + " " + s.LASTNAME + "").SingleOrDefault();
            //PAM.APPAUTH1_User = CPAM.APPAUTH1_ECODE+" "+ CPAM.APPAUTH1_ECODE.ToString();
            PAM.APPAUTH2_ECODE = CPAM.APPAUTH2_ECODE;
            PAM.APPAUTH2_User = _CreativeMasterDBContext.ADLOGINUSER.Where(x => x.ADEMPCODE == CPAM.APPAUTH2_ECODE).Select(s => s.ADEMPCODE + "-" + s.FIRSTNAME + " " + s.LASTNAME + "").SingleOrDefault();
            //PAM.APPAUTH2_User = CPAM.APPAUTH2_ECODE +" "+CPAM.APPAUTH2_ECODE.ToString();
            PAM.INITIATOR_REMARKS = _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPP_TRN.Where(x => x.ATTACHMENTID == CPM.ATTACHMENTID).Select(s => s.INITIATOR_REMARKS).SingleOrDefault();
            PAM.CM_Processattachmentappmapping_Trn = (from CPAMTDetail in _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Where(d => d.ATTACHMENTID == CPM.ATTACHMENTID)
                                                      join _AppEmp in _CreativeMasterDBContext.ADEMPLOYEE on CPAMTDetail.APPAUTH_ECODE equals _AppEmp.ADEMPCODE
                                                      select new CM_Processattachmentappmapping_TrnViewModel
                                                      {
                                                          APP_LEVEL = CPAMTDetail.APP_LEVEL,
                                                          ATTACHMENTID = CPAMTDetail.ATTACHMENTID,
                                                          APPAUTH_ECODE = CPAMTDetail.APPAUTH_ECODE,
                                                          APPAUTH_NAME = _AppEmp.FIRSTNAME + " " + _AppEmp.LASTNAME + "- [" + _AppEmp.ADEMPCODE + "]",
                                                          APPAUTH_DATE = CPAMTDetail.APPAUTH_DATE,
                                                          APPAUTH_REMARKS = CPAMTDetail.APPAUTH_REMARKS,
                                                          STATUS = CPAMTDetail.STATUS,
                                                      }).OrderBy(x => x.APP_LEVEL).ToList();

            return PAM;

        }








        public CM_PROCESSATTACHMENTAPP_TRN SaveAttachmentApproval_Trn(CreativeMasterViewModel CNMVM)
        {
            int actType = 1;
            CM_PROCESSATTACHMENTAPP_TRN cPAPPTRN = _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPP_TRN.Where(x => x.ATTACHMENTID == CNMVM.ATTACHMENTID).FirstOrDefault();
            if (cPAPPTRN == null)
            {
                cPAPPTRN = new CM_PROCESSATTACHMENTAPP_TRN();
              
            }
            cPAPPTRN.ATTACHMENTID = CNMVM.ATTACHMENTID;
            cPAPPTRN.APPAUTH1_ECODE = CNMVM.APPAUTH1_ECODE;
            cPAPPTRN.APPAUTH2_ECODE = CNMVM.APPAUTH2_ECODE;
            cPAPPTRN.INITIATOR_REMARKS = CNMVM.INITIATOR_REMARKS;
            if (cPAPPTRN.ATTACHMENTAPPID == 0)
           
                {
                cPAPPTRN.INITIATED_BY = CNMVM.INITIATED_BY;
                cPAPPTRN.INITIATED_DATE = DateTime.Now;
                cPAPPTRN.STATUS = 1;
            }
            _CreativeMasterDBContext.Entry(cPAPPTRN).State = cPAPPTRN.ATTACHMENTAPPID == 0 ? EntityState.Added : EntityState.Modified;
            _CreativeMasterDBContext.SaveChanges();

            return cPAPPTRN;
        }

        //public CorporateNewsApprovalViewModel GetCorporateNewsApprovalAuthorityById(int id)
        //{
        //    var obj = (from data in _CorporateNewsDBContext.CM_PROCESSATTACHMENTAPP_TRN.Where(x => x.ATTACHMENTAPPID == id)
        //               join tbTRN in _CorporateNewsDBContext.CM_PROCESSATTACHMENT_TRN on data.ATTACHMENTID equals tbTRN.ATTACHMENTID
        //               join AD_INITIATED in _CorporateNewsDBContext.ADLOGINUSER on data.INITIATED_BY equals AD_INITIATED.ADEMPCODE into INITIATEDJoin
        //               from INITIATED_User in INITIATEDJoin.DefaultIfEmpty()
        //               join AD_AapAuth1 in _CorporateNewsDBContext.ADLOGINUSER on data.APPAUTH1_ECODE equals AD_AapAuth1.ADEMPCODE into AAPAUTH1Join
        //               from AAPAUTH1_User in AAPAUTH1Join.DefaultIfEmpty()
        //               select new
        //               {
        //                   data.ATTACHMENTAPPID,
        //                   data.ATTACHMENTID,
        //                   data.STATUS,
        //                   tbTRN.SUBJECT,
        //                   tbTRN.BRIEF,
        //                   tbTRN.DESCRIPTION,
        //                   data.INITIATED_BY,
        //                   data.INITIATED_DATE,
        //                   data.INITIATOR_REMARKS,
        //                   data.APPAUTH1_ECODE,
        //                   data.APPAUTH1_DATE,
        //                   data.APPAUTH1_REMARKS,
        //                   INITIATED_User.FIRSTNAME,
        //                   INITIATED_User.LASTNAME,
        //                   AppAuth1_FirstName = AAPAUTH1_User.FIRSTNAME,
        //                   AppAuth1_LastName = AAPAUTH1_User.LASTNAME,
        //                   tbTRN.BANNER_NAME,
        //                   tbTRN.BANNER_CONTENTTYPE,
        //                   tbTRN.BANNER,
        //                   tbTRN.ATTACHMENT1_NAME,
        //                   tbTRN.ATTACHMENT1_CONTENTTYPE,
        //                   tbTRN.ATTACHMENT1,
        //                   tbTRN.ATTACHMENT2_NAME,
        //                   tbTRN.ATTACHMENT2_CONTENTTYPE,
        //                   tbTRN.ATTACHMENT2,
        //               }).SingleOrDefault();
        //    CorporateNewsApprovalViewModel CAAVT = new CorporateNewsApprovalViewModel
        //    {
        //        ATTACHMENTAPPID = obj.ATTACHMENTAPPID,
        //        ATTACHMENTID = obj.ATTACHMENTID,
        //        SUBJECT = CommonRepository.HtmlToText(obj.SUBJECT),
        //        BRIEF = CommonRepository.HtmlToText(obj.BRIEF),
        //        DESCRIPTION = CommonRepository.HtmlToText(obj.DESCRIPTION),
        //        STATUS = obj.STATUS,
        //        INITIATED_User = !string.IsNullOrEmpty(obj.FIRSTNAME) ? obj.FIRSTNAME + " " + obj.LASTNAME + "(" + obj.INITIATED_BY + ")" : "",
        //        INITIATED_BY = obj.INITIATED_BY,
        //        INITIATED_DATE = obj.INITIATED_DATE,
        //        INITIATOR_REMARKS = obj.INITIATOR_REMARKS,
        //        APPAUTH1_ECODE = obj.APPAUTH1_ECODE,
        //        APPAUTH1_DATE = obj.APPAUTH1_DATE,
        //        APPAUTH1_REMARKS = obj.APPAUTH1_REMARKS,
        //        APPAUTH1_User = !string.IsNullOrEmpty(obj.AppAuth1_FirstName) ? obj.AppAuth1_FirstName + " " + obj.AppAuth1_LastName + "(" + obj.APPAUTH1_ECODE + ")" : "",
        //        BANNER_NAME = obj.BANNER_NAME,
        //        BANNER_CONTENTTYPE = obj.BANNER_CONTENTTYPE,
        //        BANNER = obj.BANNER,
        //        ATTACHMENT1_NAME = obj.ATTACHMENT1_NAME,
        //        ATTACHMENT1_CONTENTTYPE = obj.ATTACHMENT1_CONTENTTYPE,
        //        ATTACHMENT1 = obj.ATTACHMENT1,
        //        ATTACHMENT2_NAME = obj.ATTACHMENT2_NAME,
        //        ATTACHMENT2_CONTENTTYPE = obj.ATTACHMENT2_CONTENTTYPE,
        //        ATTACHMENT2 = obj.ATTACHMENT2
        //    };
        //    return CAAVT;
        //}

        //public IEnumerable<CorporateNewsApprovalViewModel> GetCorporateNewsApprovalList(Int64 UserId)
        //{
        //    CommonRepository cmr = new CommonRepository();
        //    string Processids1 = cmr.GetParameterValue("CORPORATENEWS_PRCID");
        //    string[] values = Processids1.Split(',');
        //    List<long> Processids = new List<long>();
        //    for (int i = 0; i < values.Length; i++)
        //    {
        //        long lngval = Convert.ToInt32(values[i].Trim().ToString().Split('~')[1]);
        //        Processids.Add(lngval);
        //    }         
        //    List<CorporateNewsApprovalViewModel> iList = new List<CorporateNewsApprovalViewModel>();
        //    var iColl = (from data in _CorporateNewsDBContext.CM_PROCESSATTACHMENTAPP_TRN//.Where(x => (x.APPAUTH1_ECODE == UserId && x.STATUS == 2) || (x.APPAUTH2_ECODE == UserId && x.STATUS == 3))
        //                 join tbTRN in _CorporateNewsDBContext.CM_PROCESSATTACHMENT_TRN on data.ATTACHMENTID equals tbTRN.ATTACHMENTID
        //                 join AD_INITIATED in _CorporateNewsDBContext.ADLOGINUSER on data.INITIATED_BY equals AD_INITIATED.ADEMPCODE into INITIATEDJoin
        //                 from INITIATED_User in INITIATEDJoin.DefaultIfEmpty()
        //                 join AD_APPROVED in _CorporateNewsDBContext.ADLOGINUSER on data.APPAUTH1_ECODE equals AD_APPROVED.ADEMPCODE into AD_AUTH_UserJoin
        //                 from AD_AUTH_User in AD_AUTH_UserJoin.DefaultIfEmpty()
        //                 where Processids.Contains(tbTRN.PROCESSID)
        //                 //join AD_HR in _CorporateNewsDBContext.ADLOGINUSER on data.HRAPP_ECODE equals AD_HR.ADEMPCODE into HRJoin
        //                 //from HR_User in HRJoin.DefaultIfEmpty()
        //                 orderby data.INITIATED_DATE descending
        //                 select new
        //                 {
        //                     data.ATTACHMENTAPPID,
        //                     data.ATTACHMENTID,
        //                     tbTRN.SUBJECT,
        //                     tbTRN.BRIEF,
        //                     tbTRN.DESCRIPTION,
        //                     data.STATUS,
        //                     data.INITIATED_BY,
        //                     data.INITIATED_DATE,
        //                     data.INITIATOR_REMARKS,
        //                     //data.HRAPP_ECODE,
        //                     //data.HRAPP_DATE,
        //                     //data.HRAPP_REMARKS,
        //                     data.APPAUTH1_ECODE,
        //                     data.APPAUTH1_DATE,
        //                     data.APPAUTH1_REMARKS,
        //                     INITIATED_User.FIRSTNAME,
        //                     INITIATED_User.LASTNAME,
        //                     AD_AUTHFIRSTNAME = AD_AUTH_User.FIRSTNAME,
        //                     AD_AUTHLASTNAME = AD_AUTH_User.LASTNAME,
        //                     tbTRN.BANNER_NAME,
        //                     tbTRN.ATTACHMENT1_NAME,
        //                     tbTRN.ATTACHMENT2_NAME

        //                     //HR_FirstName = HR_User.FIRSTNAME,
        //                     //HR_LastName = HR_User.FIRSTNAME,

        //                 }).ToList();
        //    if (iColl.Count > 0)
        //    {
        //        foreach (var obj in iColl)
        //        {
        //            iList.Add(new CorporateNewsApprovalViewModel
        //            {
        //                ATTACHMENTAPPID = obj.ATTACHMENTAPPID,
        //                ATTACHMENTID = obj.ATTACHMENTID,
        //                SUBJECT = CommonRepository.HtmlToText(obj.SUBJECT),
        //                BRIEF = CommonRepository.HtmlToText(obj.BRIEF),
        //                DESCRIPTION = CommonRepository.HtmlToText(obj.DESCRIPTION),
        //                STATUS = obj.STATUS,
        //                INITIATED_User = !string.IsNullOrEmpty(obj.FIRSTNAME) ? obj.FIRSTNAME + " " + obj.LASTNAME + "(" + obj.INITIATED_BY + ")" : "",
        //                INITIATED_BY = obj.INITIATED_BY,
        //                INITIATED_DATE = obj.INITIATED_DATE,
        //                INITIATOR_REMARKS = obj.INITIATOR_REMARKS,
        //                APPAUTH1_ECODE = obj.APPAUTH1_ECODE,
        //                APPAUTH1_DATE = obj.APPAUTH1_DATE,
        //                APPAUTH1_REMARKS = obj.APPAUTH1_REMARKS,
        //                APPAUTH1_User = !string.IsNullOrEmpty(obj.AD_AUTHFIRSTNAME) ? obj.AD_AUTHFIRSTNAME + " " + obj.AD_AUTHLASTNAME + "(" + obj.APPAUTH1_ECODE + ")" : "",
        //                BANNER_NAME = obj.BANNER_NAME,
        //                ATTACHMENT1_NAME = obj.ATTACHMENT1_NAME,
        //                ATTACHMENT2_NAME = obj.ATTACHMENT2_NAME
        //            });
        //        }
        //    }
        //    return iList;
        //}

        //public CorporateNewsApprovalViewModel UpdateStatus(CorporateNewsApprovalViewModel AAVM)
        //{
        //    CM_PROCESSATTACHMENT_TRN PAT = new CM_PROCESSATTACHMENT_TRN();
        //    CM_PROCESSATTACHMENTAPP_TRN CPAT = new CM_PROCESSATTACHMENTAPP_TRN();

        //    CPAT = _CorporateNewsDBContext.CM_PROCESSATTACHMENTAPP_TRN.Find(AAVM.ATTACHMENTAPPID);
        //    if (CPAT != null)
        //    {
        //        CPAT.STATUS = AAVM.STATUS; //2 : Approved, 4: Rejected
        //        CPAT.APPAUTH1_ECODE = AAVM.APPAUTH1_ECODE;
        //        CPAT.APPAUTH1_DATE = DateTime.Now;
        //        CPAT.APPAUTH1_REMARKS = AAVM.APPAUTH1_REMARKS;
        //        _CorporateNewsDBContext.Entry(CPAT).State = EntityState.Modified;
        //        if (CPAT.STATUS == 2 || CPAT.STATUS == 4 || CPAT.STATUS == 3)
        //        {
        //            PAT = _CorporateNewsDBContext.CM_PROCESSATTACHMENT_TRN.Find(CPAT.ATTACHMENTID);
        //            if (PAT != null)
        //            {
        //                int status = CPAT.STATUS == 2 ? 1 : CPAT.STATUS == 4 ? 3 : CPAT.STATUS;  // 1:Approved,  3:Rejected
        //                PAT.STATUS = Convert.ToInt16(status);
        //                _CorporateNewsDBContext.Entry(PAT).State = EntityState.Modified;
        //            }
        //        }
        //        _CorporateNewsDBContext.SaveChanges();
        //    }
        //    else if (AAVM.ATTACHMENTAPPID == 0 && AAVM.STATUS == 0)
        //    {
        //        PAT = _CorporateNewsDBContext.CM_PROCESSATTACHMENT_TRN.Find(AAVM.ATTACHMENTID);
        //        if (PAT != null)
        //        {
        //            PAT.STATUS = AAVM.STATUS;
        //            _CorporateNewsDBContext.Entry(PAT).State = EntityState.Modified;
        //        }
        //        _CorporateNewsDBContext.SaveChanges();
        //    }
        //    return AAVM;
        //}

        //public FileViewModel GetFileForDownload(Int64 id, string type)
        //{
        //    FileViewModel flvm = new FileViewModel();
        //    CM_PROCESSATTACHMENT_TRN CCPT;
        //    CCPT = _CorporateNewsDBContext.CM_PROCESSATTACHMENT_TRN.Where(c => c.ATTACHMENTID == id).SingleOrDefault();
        //    if (CCPT != null)
        //    {
        //        if (type == "BANNER")
        //        {
        //            flvm.FileName = CCPT.BANNER_NAME;
        //            flvm.FileContentType = CCPT.BANNER_CONTENTTYPE;
        //            flvm.File = CCPT.BANNER;
        //        }
        //        else if (type == "Attachment1")
        //        {
        //            flvm.FileName = CCPT.ATTACHMENT1_NAME;
        //            flvm.FileContentType = CCPT.ATTACHMENT1_CONTENTTYPE;
        //            flvm.File = CCPT.ATTACHMENT1;
        //        }
        //        else if (type == "Attachment2")
        //        {
        //            flvm.FileName = CCPT.ATTACHMENT2_NAME;
        //            flvm.FileContentType = CCPT.ATTACHMENT2_CONTENTTYPE;
        //            flvm.File = CCPT.ATTACHMENT2;
        //        }
        //    }

        //    return flvm;
        //}
        //public CorporateNewsMasterViewModel DeleteDocument(Int64 id, string type)
        //{
        //    CorporateNewsMasterViewModel CNMV = new CorporateNewsMasterViewModel();
        //    CM_PROCESSATTACHMENT_TRN CCPT;
        //    CCPT = _CorporateNewsDBContext.CM_PROCESSATTACHMENT_TRN.Where(c => c.ATTACHMENTID == id).SingleOrDefault();
        //    if (CCPT != null)
        //    {

        //        if (type == "BANNER")
        //        {
        //            CCPT.BANNER = null;
        //            CCPT.BANNER_CONTENTTYPE = null;
        //            CCPT.BANNER_NAME = null;

        //        }
        //        else if (type == "Attachment1")
        //        {
        //            CCPT.ATTACHMENT1_NAME = null;
        //            CCPT.ATTACHMENT1_CONTENTTYPE = null;
        //            CCPT.ATTACHMENT1 = null;

        //        }
        //        else if (type == "Attachment2")
        //        {
        //            CCPT.ATTACHMENT2_NAME = null;
        //            CCPT.ATTACHMENT2_CONTENTTYPE = null;
        //            CCPT.ATTACHMENT2 = null;
        //            CNMV.ATTACHMENT2 = null;
        //        }

        //        _CorporateNewsDBContext.Entry(CCPT).State = EntityState.Modified;
        //        _CorporateNewsDBContext.SaveChanges();

        //        CNMV.BANNER_NAME = CCPT.BANNER_NAME;
        //        CNMV.ATTACHMENT1_NAME = CCPT.ATTACHMENT1_NAME;
        //        CNMV.ATTACHMENT2_NAME = CCPT.ATTACHMENT2_NAME;
        //    }
        //    return CNMV;
        //}

        public List<Employee_Details> PortalAutocompleteSuggestions(string Key)
        {
            //var oDesg = _AcrDBContext.ADDESIGNATION.Where(m => m.ACTIVE == 1 && m.DESCRIP.ToUpper().Contains(designation.ToUpper())).ToList();
            //var oFDesg = _AcrDBContext.ADFUNCTIONALDESIGNATION.Where(m => m.ACTIVE == 1 && m.DESCRIP.ToUpper().Contains(designation.ToUpper())).ToList();

            //if (oDesg.Count > 0 || oFDesg.Count > 0)
            //{
            //    isSearchDesg = 1;
            //}
            long strKIID = (long)_CreativeMasterDBContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
            List<Employee_Details> empdata_ans = new List<Employee_Details>();
            //if (desg > 0)
            //{
            //    var empdata = (from userdata in _AcrDBContext.ADEMPDIVDEPTSECT.Where(m => m.SYKI == strKIID && m.ADDESIGNATIONID == desg)
            //                   join emp in _AcrDBContext.ADEMPLOYEE.Where(m => m.ACTIVE == 1) on userdata.ADEMPCODE equals emp.ADEMPCODE
            //                   //join d in _AcrDBContext.ADDESIGNATION.Where(m => m.ACTIVE == 1 ) on userdata.ADDESIGNATIONID equals d.ADDESIGNATIONID
            //                   join d in _AcrDBContext.ADDESIGNATION.Where(m => m.ACTIVE == 1 && m.ADDESIGNATIONID == desg) on userdata.ADDESIGNATIONID equals d.ADDESIGNATIONID
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
            //               ).ToList();



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
            //    //empdata_ans = portalUserDtos;
            //    return portalUserDtos;
            //}
            //else if (fundesg > 0)
            //{
            var empdata_fundesg = (from userdata in _CreativeMasterDBContext.ADEMPDIVDEPTSECT.Where(m => m.SYKI == strKIID)
                                   join emp in _CreativeMasterDBContext.ADEMPLOYEE.Where(m => m.ACTIVE == 1) on userdata.ADEMPCODE equals emp.ADEMPCODE
                                   //join d in _AcrDBContext.ADDESIGNATION.Where(m => m.ACTIVE == 1 ) on userdata.ADDESIGNATIONID equals d.ADDESIGNATIONID
                                   join d in _CreativeMasterDBContext.ADDESIGNATION.Where(m => m.ACTIVE == 1) on userdata.ADFUNCTIONALDESIGNATIONID equals d.ADDESIGNATIONID
                                   //join d in _AcrDBContext.ADDESIGNATION.Where(m => m.ACTIVE == 1 && m.ADDESIGNATIONID == desg) on userdata.ADDESIGNATIONID equals d.ADDESIGNATIONID
                                   //join fg in _AcrDBContext.ADFUNCTIONALDESIGNATION.Where(m => m.ACTIVE == 1) on userdata.ADFUNCTIONALDESIGNATIONID equals fg.ADFUNCTIONALDESIGNATIONID into ls
                                   join fg in _CreativeMasterDBContext.ADFUNCTIONALDESIGNATION.Where(m => m.ACTIVE == 1) on userdata.ADFUNCTIONALDESIGNATIONID equals fg.ADFUNCTIONALDESIGNATIONID into ls
                                   from fg in ls.DefaultIfEmpty()
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
                                      where (userdata.ADEMPCODE.ToString().Equals(Key))
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
            // }

            return empdata_ans;

        }

        public IEnumerable<Employee_Details> BindAppAuth1()
        {

            long strKIID = (long)_CreativeMasterDBContext.SYKI.Where(m => m.ACTIVE == 1).FirstOrDefault().SYKIID;
            IEnumerable<Employee_Details> iList;

            var empdata_fundesg = (from userdata in _CreativeMasterDBContext.ADEMPDIVDEPTSECT.Where(m => m.SYKI == strKIID)
                                   join emp in _CreativeMasterDBContext.ADEMPLOYEE.Where(m => m.ACTIVE == 1) on userdata.ADEMPCODE equals emp.ADEMPCODE
                                   //join d in _AcrDBContext.ADDESIGNATION.Where(m => m.ACTIVE == 1 ) on userdata.ADDESIGNATIONID equals d.ADDESIGNATIONID
                                   join d in _CreativeMasterDBContext.ADDESIGNATION.Where(m => m.ACTIVE == 1) on userdata.ADFUNCTIONALDESIGNATIONID equals d.ADDESIGNATIONID
                                   //join d in _AcrDBContext.ADDESIGNATION.Where(m => m.ACTIVE == 1 && m.ADDESIGNATIONID == desg) on userdata.ADDESIGNATIONID equals d.ADDESIGNATIONID
                                   //join fg in _AcrDBContext.ADFUNCTIONALDESIGNATION.Where(m => m.ACTIVE == 1) on userdata.ADFUNCTIONALDESIGNATIONID equals fg.ADFUNCTIONALDESIGNATIONID into ls
                                   join fg in _CreativeMasterDBContext.ADFUNCTIONALDESIGNATION.Where(m => m.ACTIVE == 1) on userdata.ADFUNCTIONALDESIGNATIONID equals fg.ADFUNCTIONALDESIGNATIONID into ls
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
                         _EName =  userdata.FIRSTNAME + " " + userdata.LASTNAME + "-" + userdata.ADEMPCODE ,
                         // _EName = userdata.ADEMPCODE + "-" + userdata.FIRSTNAME + " " + userdata.LASTNAME,
                     }).OrderBy(x => x._EName).ToList();
            //iList = (from data in _CreativeMasterDBContext.SYSITE.Where(x => x.ACTIVE == 1).ToList()
            //         select new SYSITE
            //         {
            //             SYSITEID = data.SYSITEID,
            //             DESCRIP = data.DESCRIP,
            //         });
            return iList;
        }

        public short CreativeMasterAppr(CM_Processattachmentappmapping_TrnViewModel PHVM, Employee_Details emp_dtl)
        {
            short retVal = 0;
            using (var transaction = _CreativeMasterDBContext.Database.BeginTransaction())
            {
                try
                {
                    CM_PROCESSATTACHMENTAPPMAPPING_TRN DPAH = new CM_PROCESSATTACHMENTAPPMAPPING_TRN();
                    CM_PROCESSATTACHMENTAPPMAPPING_TRN DPAH1 = new CM_PROCESSATTACHMENTAPPMAPPING_TRN();
                    CM_PROCESSATTACHMENTAPPMAPPING_TRN DPAH2 = new CM_PROCESSATTACHMENTAPPMAPPING_TRN();
                    CM_PROCESSATTACHMENT_TRN PAT = new CM_PROCESSATTACHMENT_TRN();
                    if (PHVM.ATTACHMENTID > 0)
                    {
                        DPAH = _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Where(x => x.ATTACHMENTID == PHVM.ATTACHMENTID && x.APPAUTH_ECODE == emp_dtl._ECode && x.STATUS == 0).FirstOrDefault();

                        if (DPAH != null)
                        {
                            DPAH.APPAUTH_DATE = DateTime.Now;
                            DPAH.APPAUTH_REMARKS = PHVM.APPAUTH_REMARKS;
                            DPAH.MODIFIED_BY = PHVM.MODIFIED_BY;
                            DPAH.MODIFIED_DATE = DateTime.Now;
                            DPAH.STATUS = PHVM.STATUS;
                            _CreativeMasterDBContext.Entry(DPAH).State = EntityState.Modified;
                            _CreativeMasterDBContext.SaveChanges();
                        }


                        var MAPPINGATTACHMENTID = _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Where(x => x.ATTACHMENTID == PHVM.ATTACHMENTID && x.APPAUTH_ECODE == emp_dtl._ECode).Select(x => x.MAPPINGATTACHMENTID + 1).FirstOrDefault();
                        DPAH1 = _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Where(x => x.ATTACHMENTID == PHVM.ATTACHMENTID && x.MAPPINGATTACHMENTID == MAPPINGATTACHMENTID).FirstOrDefault();
                        if (DPAH1 != null)
                        {
                            DPAH1.STATUS = 0;//Pending
                            _CreativeMasterDBContext.Entry(DPAH).State = EntityState.Modified;
                            _CreativeMasterDBContext.SaveChanges();
                        }
                        PAT = _CreativeMasterDBContext.CM_PROCESSATTACHMENT_TRN.Where(x => x.ATTACHMENTID == PHVM.ATTACHMENTID).FirstOrDefault();
                        DPAH2 = _CreativeMasterDBContext.CM_PROCESSATTACHMENTAPPMAPPING_TRN.Where(x => x.ATTACHMENTID == PHVM.ATTACHMENTID && x.APPAUTH_ECODE == emp_dtl._ECode && x.APP_LEVEL == 2).FirstOrDefault();
                        if (PAT != null)
                        {
                            if (DPAH != null)
                            {
                                if (DPAH.STATUS == 2)
                                {
                                    PAT.STATUS = 0;//De-Active
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
                                else
                                {
                                    PAT.STATUS = 2;//WIP
                                }
                                if (DPAH2.STATUS == 2)
                                {
                                    PAT.STATUS = 0;//De-Active
                                }
                                
                            }
                            
                            _CreativeMasterDBContext.Entry(PAT).State = EntityState.Modified;
                            _CreativeMasterDBContext.SaveChanges();
                        }
                        retVal = 1;

                    }

                    /////////// Update Approval Status //////////




                    /// --- Save PO Approval Authority --- ///
                    //    IOMAppAuthSeqViewModel seqModel = new IOMAppAuthSeqViewModel();
                    //    if (PHVM.APPROVAL_STATUS == 1)
                    //    {
                    //        short nextSeq = Convert.ToInt16(_IOMDBContext.DGIT_IOMAPPAUTHSEQ.Where(s => s.IOMHEADERID == PHVM.IOMID && s.ADEMPCODE == PHVM.ADEMPCODE).Select(s => s.APP_SEQ).FirstOrDefault() + 1);
                    //        seqModel = (from data in _IOMDBContext.DGIT_IOMAPPAUTHSEQ.Where(a => a.APP_SEQ == nextSeq && a.IOMHEADERID == PHVM.IOMID)
                    //                    select new IOMAppAuthSeqViewModel
                    //                    {
                    //                        IOMAPPAUTH_ID = data.IOMAPPAUTH_ID,
                    //                        IOMID = data.IOMHEADERID,
                    //                        ADEMPCODE = data.ADEMPCODE,
                    //                        APP_SEQ = data.APP_SEQ,
                    //                        Header = data.IOMAPPHEADER,
                    //                        STATUS = data.STATUS,
                    //                    }).FirstOrDefault();
                    //        if (seqModel != null)
                    //        {
                    //            SaveIOMAppHis(PHVM.ADDEDBY, PHVM.IOMID, seqModel);
                    //        }
                    //    }

                    //    //////// Update Process Status ////////
                    //    DGIT_IOMHEADER DPH = new DGIT_IOMHEADER(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                    //    DPH = _IOMDBContext.DGIT_IOMHEADER.Where(x => x.IOMHEADERID == PHVM.IOMID).SingleOrDefault();
                    //    if (DPH != null)
                    //    {
                    //        if (PHVM.APPROVAL_STATUS == 1 && seqModel == null)
                    //        {
                    //            DPH.PROCESS_STATUS = 2;
                    //        }
                    //        else if (PHVM.APPROVAL_STATUS == 2)
                    //        {
                    //            DPH.PROCESS_STATUS = 0;
                    //        }
                    //        else if (PHVM.APPROVAL_STATUS == 3)
                    //        {
                    //            DPH.PROCESS_STATUS = 3;
                    //        }
                    //        else
                    //        {
                    //            DPH.PROCESS_STATUS = 1;
                    //        }
                    //        DPH.UPDATEDBY = PHVM.UPDATEBY;
                    //        DPH.UPDATEDATE = DateTime.Now;
                    //        _IOMDBContext.Entry(DPH).State = EntityState.Modified;
                    //        _IOMDBContext.SaveChanges();
                    //    }
                    //    retVal = 1;
                    //}
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

        public CreativeMasterViewModel DeleteDocument(Int64 id, string type)
        {
            CreativeMasterViewModel CNMV = new CreativeMasterViewModel();
            CM_PROCESSATTACHMENT_TRN CCPT;
            CCPT = _CreativeMasterDBContext.CM_PROCESSATTACHMENT_TRN.Where(c => c.ATTACHMENTID == id).SingleOrDefault();
            if (CCPT != null)
            {

                if (type == "BANNER")
                {
                    CCPT.BANNER = null;
                    CCPT.BANNER_CONTENTTYPE = null;
                    CCPT.BANNER_NAME = null;

                }
                else if (type == "Attachment1")
                {
                    CCPT.ATTACHMENT1_NAME = null;
                    CCPT.ATTACHMENT1_CONTENTTYPE = null;
                    CCPT.ATTACHMENT1 = null;

                }
                else if (type == "Attachment2")
                {
                    CCPT.ATTACHMENT2_NAME = null;
                    CCPT.ATTACHMENT2_CONTENTTYPE = null;
                    CCPT.ATTACHMENT2 = null;
                    CNMV.ATTACHMENT2 = null;
                }

                _CreativeMasterDBContext.Entry(CCPT).State = EntityState.Modified;
                _CreativeMasterDBContext.SaveChanges();

                CNMV.BANNER_NAME = CCPT.BANNER_NAME;
                CNMV.ATTACHMENT1_NAME = CCPT.ATTACHMENT1_NAME;
                CNMV.ATTACHMENT2_NAME = CCPT.ATTACHMENT2_NAME;
            }
            return CNMV;
        }

        //public IEnumerable<Employee_Details> BindContentProcess()
        //{
        //    IEnumerable<ProcessMstViewModel> iList;
        //    //iList = (from data in _CreativeMasterDBContext.CM_PROCESS_MST.ToList().OrderBy(o => o.CREATED_DATE)
        //    iList = (from data in _CreativeMasterDBContext.CM_PROCESS_MST.Where(x => x.MSTPROCESSID == 1)
        //             select new ProcessMstViewModel
        //             {
        //                 PROCESSID = data.MSTPROCESSID,
        //                 PROCESS_NAME = data.PROCESS_NAME,
        //                 Upload_Banner = data.PROCESS_BANNER
        //             }).ToList();
        //    return iList;
        //}

    }
}
