using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ePortal.Infrastructure.Repositories
{
    public class CorporateNewsRepository
    {
        private EPortalDBContext _CorporateNewsDBContext;
        private readonly CommonRepository _cmr;

        public CorporateNewsRepository(EPortalDBContext CorporateNewsDBContext, CommonRepository cmr)
        {
            _CorporateNewsDBContext = CorporateNewsDBContext;
            _cmr = cmr;
        }

        // Content Process Master
        #region Content Process Master
        public IEnumerable<ProcessMstViewModel> BindContentProcess()
        {
            IEnumerable<ProcessMstViewModel> iList;
            iList = (from data in _CorporateNewsDBContext.CM_PROCESS_MST.ToList().OrderBy(o => o.CREATED_DATE)
                     select new ProcessMstViewModel
                     {
                         PROCESSID = data.MSTPROCESSID,
                         PROCESS_NAME = data.PROCESS_NAME,
                         Upload_Banner = data.PROCESS_BANNER
                     });
            return iList;
        }
        #endregion
        // CorporateNews
        #region CorporateNews Master
        public List<CorporateNewsMasterViewModel> GetCorporateNewsMasterList()
        {
            //CommonRepository cmr = new CommonRepository();
            string Processids1 = _cmr.GetParameterValue("CORPORATENEWS_PRCID");
            string[] values = Processids1.Split(',');
            List<long> Processids = new List<long>();
            for (int i = 0; i < values.Length; i++)
            {
                long lngval = Convert.ToInt32(values[i].Trim().ToString().Split('~')[1]);
                Processids.Add(lngval);
            }
            List<CorporateNewsMasterViewModel> iList = new List<CorporateNewsMasterViewModel>();
            var iColl = (from data in _CorporateNewsDBContext.CM_PROCESSATTACHMENT_TRN
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
                    iList.Add(new CorporateNewsMasterViewModel
                    {
                        ATTACHMENTID = obj.ATTACHMENTID,
                        PROCESSID = obj.PROCESSID,
                        SUBJECT = CommonRepository.HtmlToText(obj.SUBJECT),
                        BRIEF = CommonRepository.HtmlToText(obj.BRIEF),
                        DESCRIPTION = CommonRepository.HtmlToText(obj.DESCRIPTION),
                        START_DATE = obj.START_DATE.ToString("dd-MMM-yyyy"),
                        END_DATE = obj.END_DATE.ToString("dd-MMM-yyyy"),
                        STATUS = obj.STATUS,
                        CreateDate = obj.CREATED_DATE.ToString("dd/MM/yyyy"),
                    });
                }
            }
            return iList;
        }

        public Int16 SaveProcessAttachment_Trn(CorporateNewsMasterViewModel AVM)
        {
            Int16 retVal = 0;
            using (var transaction = _CorporateNewsDBContext.Database.BeginTransaction())
            {
                try
                {
                    var maxId = _CorporateNewsDBContext.CM_PROCESSATTACHMENT_TRN.Max(x=>x.ATTACHMENTID);

                    CM_PROCESSATTACHMENT_TRN CPT = new CM_PROCESSATTACHMENT_TRN();
                    CPT.PROCESSID = AVM.PROCESSID;
                    CPT.START_DATE = DateTime.ParseExact(AVM.START_DATE, "dd-MMM-yyyy", null);
                    CPT.END_DATE = DateTime.ParseExact(AVM.END_DATE, "dd-MMM-yyyy", null);
                    CPT.SUBJECT = CommonRepository.TextToHtml(AVM.SUBJECT);
                    CPT.BRIEF = CommonRepository.TextToHtml(AVM.BRIEF);
                    CPT.DESCRIPTION = CommonRepository.TextToHtml(AVM.DESCRIPTION);
                    CPT.STATUS = 2; // status 2 = WIP
                    CPT.BANNER_NAME = AVM.BANNER_NAME;
                    CPT.BANNER_CONTENTTYPE = AVM.BANNER_CONTENTTYPE;
                    CPT.BANNER = AVM.BANNER;
                    CPT.ATTACHMENT1_NAME = AVM.ATTACHMENT1_NAME;
                    CPT.ATTACHMENT1_CONTENTTYPE = AVM.ATTACHMENT1_CONTENTTYPE;
                    CPT.ATTACHMENT1 = AVM.ATTACHMENT1;
                    CPT.ATTACHMENT2_NAME = AVM.ATTACHMENT2_NAME;
                    CPT.ATTACHMENT2_CONTENTTYPE = AVM.ATTACHMENT2_CONTENTTYPE;
                    CPT.ATTACHMENT2 = AVM.ATTACHMENT2;
                    CPT.CREATED_DATE = DateTime.Now;
                    CPT.CREATED_BY = AVM.CREATED_BY;
                    CPT.ATTACHMENTID = maxId + 1;
                    _CorporateNewsDBContext.CM_PROCESSATTACHMENT_TRN.Add(CPT);
                    _CorporateNewsDBContext.SaveChanges();
                    // Save for attachment approval
                    CorporateNewsMasterViewModel AAVM = new CorporateNewsMasterViewModel();
                    AAVM.ATTACHMENTID = CPT.ATTACHMENTID;
                    AAVM.INITIATED_BY = CPT.CREATED_BY;
                    AAVM.INITIATOR_REMARKS = AVM.INITIATOR_REMARKS;
                    SaveAttachmentApproval_Trn(AAVM);
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

//        var entity = await _CorporateNewsDBContext.CM_PROCESSATTACHMENT_TRN
//    .FirstOrDefaultAsync(x => x.ATTACHMENTID == PAM.ATTACHMENTID);

//if (entity != null)
//{
//    entity.SUBJECT = PAM.SUBJECT;
//    // assign other fields

//    _CorporateNewsDBContext.Update(entity);
//    await _CorporateNewsDBContext.SaveChangesAsync();
//}


        public Int16 UpdateProcessAttachment_Trn(CorporateNewsMasterViewModel PAM)
        {
            Int16 retVal = 0;
            using (var transaction = _CorporateNewsDBContext.Database.BeginTransaction())
            {
                try
                {
                    CM_PROCESSATTACHMENT_TRN CPT = new CM_PROCESSATTACHMENT_TRN();
                    CPT = _CorporateNewsDBContext.CM_PROCESSATTACHMENT_TRN.FirstOrDefault(x => x.ATTACHMENTID == PAM.ATTACHMENTID);

                    if (CPT != null)
                    {


                        CPT.PROCESSID = PAM.PROCESSID;
                        CPT.START_DATE = Convert.ToDateTime(PAM.START_DATE);
                        CPT.END_DATE = Convert.ToDateTime(PAM.END_DATE);
                        CPT.SUBJECT = CommonRepository.TextToHtml(PAM.SUBJECT);
                        CPT.BRIEF = CommonRepository.TextToHtml(PAM.BRIEF);
                        CPT.DESCRIPTION = CommonRepository.TextToHtml(PAM.DESCRIPTION);
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
                        if (PAM.ATTACHMENT2 != null)
                        {
                            CPT.ATTACHMENT2_NAME = PAM.ATTACHMENT2_NAME;
                            CPT.ATTACHMENT2_CONTENTTYPE = PAM.ATTACHMENT2_CONTENTTYPE;
                            CPT.ATTACHMENT2 = PAM.ATTACHMENT2;
                        }
                        CPT.MODIFIED_DATE = DateTime.Now;
                        CPT.MODIFIED_BY = PAM.MODIFIED_BY;
                       _CorporateNewsDBContext.Entry(CPT).State = EntityState.Modified;

                        //_CorporateNewsDBContext.Update(CPT);
                        _CorporateNewsDBContext.SaveChanges();

                    /// Save for AttachmentApproval       
                    CorporateNewsMasterViewModel AAVM = new CorporateNewsMasterViewModel();
                    AAVM.ATTACHMENTID = CPT.ATTACHMENTID;
                    AAVM.INITIATED_BY = CPT.CREATED_BY;
                    AAVM.INITIATOR_REMARKS = PAM.INITIATOR_REMARKS;
                    SaveAttachmentApproval_Trn(AAVM);
                    transaction.Commit();
                    retVal = 1;
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

        public CorporateNewsMasterViewModel GetCorporateNewsDetails(int id)
        {
            CorporateNewsMasterViewModel PAM = new CorporateNewsMasterViewModel();
            CM_PROCESSATTACHMENT_TRN CPM = new CM_PROCESSATTACHMENT_TRN();
            CM_PROCESS_MST MST = new CM_PROCESS_MST();
            string processName = string.Empty;

            //CPM = _CorporateNewsDBContext.CM_PROCESSATTACHMENT_TRN.Include("CM_PROCESS_MST").Where(x => x.ATTACHMENTID == id).SingleOrDefault();
            CPM = _CorporateNewsDBContext.CM_PROCESSATTACHMENT_TRN.Where(x => x.ATTACHMENTID == id).SingleOrDefault();
            MST = _CorporateNewsDBContext.CM_PROCESS_MST.Where(d => d.MSTPROCESSID == CPM.PROCESSID).SingleOrDefault();

            if (MST != null)
            {
                processName = MST.PROCESS_NAME;
            }
            
            PAM.ATTACHMENTID = CPM.ATTACHMENTID;
            PAM.PROCESSID = CPM.PROCESSID;
            PAM.CM_PROCESS_MST = new ProcessMstViewModel
            {
                //PROCESS_NAME = CPM.CM_PROCESS_MST.PROCESS_NAME,
                PROCESS_NAME = processName
            };
            PAM.STATUS = CPM.STATUS;
            PAM.CREATED_DATE = CPM.CREATED_DATE;
            PAM.User_Name = _CorporateNewsDBContext.ADLOGINUSER.Where(x => x.ADEMPCODE == CPM.CREATED_BY).Select(s => s.FIRSTNAME + " " + s.LASTNAME + "(" + s.ADEMPCODE + ")").SingleOrDefault();
            PAM.SUBJECT = CommonRepository.HtmlToText(CPM.SUBJECT);
            PAM.BRIEF = CommonRepository.HtmlToText(CPM.BRIEF);
            PAM.DESCRIPTION = CommonRepository.HtmlToText(CPM.DESCRIPTION);
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
            PAM.INITIATOR_REMARKS = _CorporateNewsDBContext.CM_PROCESSATTACHMENTAPP_TRN.Where(x => x.ATTACHMENTID == CPM.ATTACHMENTID).Select(s => s.INITIATOR_REMARKS).SingleOrDefault();
            return PAM;
        }




        #endregion
        public CM_PROCESSATTACHMENTAPP_TRN SaveAttachmentApproval_Trn(CorporateNewsMasterViewModel CNMVM)
        {
            CM_PROCESSATTACHMENTAPP_TRN cPAPPTRN = _CorporateNewsDBContext.CM_PROCESSATTACHMENTAPP_TRN.Where(x => x.ATTACHMENTID == CNMVM.ATTACHMENTID).FirstOrDefault();
            if (cPAPPTRN == null)
            {
                cPAPPTRN = new CM_PROCESSATTACHMENTAPP_TRN();
            }
            cPAPPTRN.ATTACHMENTID = CNMVM.ATTACHMENTID;
            cPAPPTRN.INITIATOR_REMARKS = CNMVM.INITIATOR_REMARKS;
            if (cPAPPTRN.ATTACHMENTAPPID == 0)
            {
                cPAPPTRN.INITIATED_BY = CNMVM.INITIATED_BY;
                cPAPPTRN.INITIATED_DATE = DateTime.Now;
                cPAPPTRN.STATUS = 1;

                var maxId = _CorporateNewsDBContext.CM_PROCESSATTACHMENTAPP_TRN.Max(x => x.ATTACHMENTAPPID);
                cPAPPTRN.ATTACHMENTAPPID = maxId + 1;

                _CorporateNewsDBContext.Entry(cPAPPTRN).State = EntityState.Added;
                
            }
            else
            {
                _CorporateNewsDBContext.Entry(cPAPPTRN).State = EntityState.Modified;
            }

            _CorporateNewsDBContext.SaveChanges();

            return cPAPPTRN;
        }

        public CorporateNewsApprovalViewModel GetCorporateNewsApprovalAuthorityById(int id)
        {
            var obj = (from data in _CorporateNewsDBContext.CM_PROCESSATTACHMENTAPP_TRN.Where(x => x.ATTACHMENTAPPID == id)
                       join tbTRN in _CorporateNewsDBContext.CM_PROCESSATTACHMENT_TRN on data.ATTACHMENTID equals tbTRN.ATTACHMENTID
                       join AD_INITIATED in _CorporateNewsDBContext.ADLOGINUSER on data.INITIATED_BY equals AD_INITIATED.ADEMPCODE into INITIATEDJoin
                       from INITIATED_User in INITIATEDJoin.DefaultIfEmpty()
                       join AD_AapAuth1 in _CorporateNewsDBContext.ADLOGINUSER on data.APPAUTH1_ECODE equals AD_AapAuth1.ADEMPCODE into AAPAUTH1Join
                       from AAPAUTH1_User in AAPAUTH1Join.DefaultIfEmpty()
                       select new
                       {
                           data.ATTACHMENTAPPID,
                           data.ATTACHMENTID,
                           data.STATUS,
                           tbTRN.SUBJECT,
                           tbTRN.BRIEF,
                           tbTRN.DESCRIPTION,
                           data.INITIATED_BY,
                           data.INITIATED_DATE,
                           data.INITIATOR_REMARKS,
                           data.APPAUTH1_ECODE,
                           data.APPAUTH1_DATE,
                           data.APPAUTH1_REMARKS,
                           INITIATED_User.FIRSTNAME,
                           INITIATED_User.LASTNAME,
                           AppAuth1_FirstName = AAPAUTH1_User.FIRSTNAME,
                           AppAuth1_LastName = AAPAUTH1_User.LASTNAME,
                           tbTRN.BANNER_NAME,
                           tbTRN.BANNER_CONTENTTYPE,
                           tbTRN.BANNER,
                           tbTRN.ATTACHMENT1_NAME,
                           tbTRN.ATTACHMENT1_CONTENTTYPE,
                           tbTRN.ATTACHMENT1,
                           tbTRN.ATTACHMENT2_NAME,
                           tbTRN.ATTACHMENT2_CONTENTTYPE,
                           tbTRN.ATTACHMENT2,
                       }).SingleOrDefault();

            CorporateNewsApprovalViewModel CAAVT = new CorporateNewsApprovalViewModel
            {
                ATTACHMENTAPPID = obj.ATTACHMENTAPPID,
                ATTACHMENTID = obj.ATTACHMENTID,
                SUBJECT = CommonRepository.HtmlToText(obj.SUBJECT),
                BRIEF = CommonRepository.HtmlToText(obj.BRIEF),
                DESCRIPTION = CommonRepository.HtmlToText(obj.DESCRIPTION),
                STATUS = obj.STATUS,
                INITIATED_User = !string.IsNullOrEmpty(obj.FIRSTNAME) ? obj.FIRSTNAME + " " + obj.LASTNAME + "(" + obj.INITIATED_BY + ")" : "",
                INITIATED_BY = obj.INITIATED_BY,
                INITIATED_DATE = obj.INITIATED_DATE,
                INITIATOR_REMARKS = obj.INITIATOR_REMARKS,
                APPAUTH1_ECODE = obj.APPAUTH1_ECODE,
                APPAUTH1_DATE = obj.APPAUTH1_DATE,
                APPAUTH1_REMARKS = obj.APPAUTH1_REMARKS,
                APPAUTH1_User = !string.IsNullOrEmpty(obj.AppAuth1_FirstName) ? obj.AppAuth1_FirstName + " " + obj.AppAuth1_LastName + "(" + obj.APPAUTH1_ECODE + ")" : "",
                BANNER_NAME = obj.BANNER_NAME,
                BANNER_CONTENTTYPE = obj.BANNER_CONTENTTYPE,
                BANNER = obj.BANNER,
                ATTACHMENT1_NAME = obj.ATTACHMENT1_NAME,
                ATTACHMENT1_CONTENTTYPE = obj.ATTACHMENT1_CONTENTTYPE,
                ATTACHMENT1 = obj.ATTACHMENT1,
                ATTACHMENT2_NAME = obj.ATTACHMENT2_NAME,
                ATTACHMENT2_CONTENTTYPE = obj.ATTACHMENT2_CONTENTTYPE,
                ATTACHMENT2 = obj.ATTACHMENT2
            };
            return CAAVT;
        }

        public IEnumerable<CorporateNewsApprovalViewModel> GetCorporateNewsApprovalList(Int64 UserId)
        {
            //CommonRepository cmr = new CommonRepository();
            string Processids1 = _cmr.GetParameterValue("CORPORATENEWS_PRCID");
            string[] values = Processids1.Split(',');
            List<long> Processids = new List<long>();
            for (int i = 0; i < values.Length; i++)
            {
                long lngval = Convert.ToInt32(values[i].Trim().ToString().Split('~')[1]);
                Processids.Add(lngval);
            }
            List<CorporateNewsApprovalViewModel> iList = new List<CorporateNewsApprovalViewModel>();
            var iColl = (from data in _CorporateNewsDBContext.CM_PROCESSATTACHMENTAPP_TRN//.Where(x => (x.APPAUTH1_ECODE == UserId && x.STATUS == 2) || (x.APPAUTH2_ECODE == UserId && x.STATUS == 3))
                         join tbTRN in _CorporateNewsDBContext.CM_PROCESSATTACHMENT_TRN on data.ATTACHMENTID equals tbTRN.ATTACHMENTID
                         join AD_INITIATED in _CorporateNewsDBContext.ADLOGINUSER on data.INITIATED_BY equals AD_INITIATED.ADEMPCODE into INITIATEDJoin
                         from INITIATED_User in INITIATEDJoin.DefaultIfEmpty()
                         join AD_APPROVED in _CorporateNewsDBContext.ADLOGINUSER on data.APPAUTH1_ECODE equals AD_APPROVED.ADEMPCODE into AD_AUTH_UserJoin
                         from AD_AUTH_User in AD_AUTH_UserJoin.DefaultIfEmpty()
                         where Processids.Contains(tbTRN.PROCESSID)
                         //join AD_HR in _CorporateNewsDBContext.ADLOGINUSER on data.HRAPP_ECODE equals AD_HR.ADEMPCODE into HRJoin
                         //from HR_User in HRJoin.DefaultIfEmpty()
                         orderby data.INITIATED_DATE descending
                         select new
                         {
                             data.ATTACHMENTAPPID,
                             data.ATTACHMENTID,
                             tbTRN.SUBJECT,
                             tbTRN.BRIEF,
                             tbTRN.DESCRIPTION,
                             data.STATUS,
                             data.INITIATED_BY,
                             data.INITIATED_DATE,
                             data.INITIATOR_REMARKS,
                             //data.HRAPP_ECODE,
                             //data.HRAPP_DATE,
                             //data.HRAPP_REMARKS,
                             data.APPAUTH1_ECODE,
                             data.APPAUTH1_DATE,
                             data.APPAUTH1_REMARKS,
                             INITIATED_User.FIRSTNAME,
                             INITIATED_User.LASTNAME,
                             AD_AUTHFIRSTNAME = AD_AUTH_User.FIRSTNAME,
                             AD_AUTHLASTNAME = AD_AUTH_User.LASTNAME,
                             tbTRN.BANNER_NAME,
                             tbTRN.ATTACHMENT1_NAME,
                             tbTRN.ATTACHMENT2_NAME

                             //HR_FirstName = HR_User.FIRSTNAME,
                             //HR_LastName = HR_User.FIRSTNAME,

                         }).ToList();

            if (iColl.Count > 0)
            {
                foreach (var obj in iColl)
                {
                    iList.Add(new CorporateNewsApprovalViewModel
                    {
                        ATTACHMENTAPPID = obj.ATTACHMENTAPPID,
                        ATTACHMENTID = obj.ATTACHMENTID,
                        SUBJECT = CommonRepository.HtmlToText(obj.SUBJECT),
                        BRIEF = CommonRepository.HtmlToText(obj.BRIEF),
                        DESCRIPTION = CommonRepository.HtmlToText(obj.DESCRIPTION),
                        STATUS = obj.STATUS,
                        INITIATED_User = !string.IsNullOrEmpty(obj.FIRSTNAME) ? obj.FIRSTNAME + " " + obj.LASTNAME + "(" + obj.INITIATED_BY + ")" : "",
                        INITIATED_BY = obj.INITIATED_BY,
                        INITIATED_DATE = obj.INITIATED_DATE,
                        INITIATOR_REMARKS = obj.INITIATOR_REMARKS,
                        APPAUTH1_ECODE = obj.APPAUTH1_ECODE,
                        APPAUTH1_DATE = obj.APPAUTH1_DATE,
                        APPAUTH1_REMARKS = obj.APPAUTH1_REMARKS,
                        APPAUTH1_User = !string.IsNullOrEmpty(obj.AD_AUTHFIRSTNAME) ? obj.AD_AUTHFIRSTNAME + " " + obj.AD_AUTHLASTNAME + "(" + obj.APPAUTH1_ECODE + ")" : "",
                        BANNER_NAME = obj.BANNER_NAME,
                        ATTACHMENT1_NAME = obj.ATTACHMENT1_NAME,
                        ATTACHMENT2_NAME = obj.ATTACHMENT2_NAME
                    });
                }
            }
            return iList;
        }

        public CorporateNewsApprovalViewModel UpdateStatus(CorporateNewsApprovalViewModel AAVM)
        {
            CM_PROCESSATTACHMENT_TRN PAT = new CM_PROCESSATTACHMENT_TRN();
            CM_PROCESSATTACHMENTAPP_TRN CPAT = new CM_PROCESSATTACHMENTAPP_TRN();

            CPAT = _CorporateNewsDBContext.CM_PROCESSATTACHMENTAPP_TRN.Find(AAVM.ATTACHMENTAPPID);
            if (CPAT != null)
            {
                CPAT.STATUS = AAVM.STATUS; //2 : Approved, 4: Rejected
                CPAT.APPAUTH1_ECODE = AAVM.APPAUTH1_ECODE;
                CPAT.APPAUTH1_DATE = DateTime.Now;
                CPAT.APPAUTH1_REMARKS = AAVM.APPAUTH1_REMARKS;
                _CorporateNewsDBContext.Entry(CPAT).State = EntityState.Modified;
                if (CPAT.STATUS == 2 || CPAT.STATUS == 4 || CPAT.STATUS == 3)
                {
                    PAT = _CorporateNewsDBContext.CM_PROCESSATTACHMENT_TRN.Find(CPAT.ATTACHMENTID);
                    if (PAT != null)
                    {
                        int status = CPAT.STATUS == 2 ? 1 : CPAT.STATUS == 4 ? 3 : CPAT.STATUS;  // 1:Approved,  3:Rejected
                        PAT.STATUS = Convert.ToInt16(status);
                        _CorporateNewsDBContext.Entry(PAT).State = EntityState.Modified;
                    }
                }
                _CorporateNewsDBContext.SaveChanges();
            }
            else if (AAVM.ATTACHMENTAPPID == 0 && AAVM.STATUS == 0)
            {
                PAT = _CorporateNewsDBContext.CM_PROCESSATTACHMENT_TRN.Find(AAVM.ATTACHMENTID);
                if (PAT != null)
                {
                    PAT.STATUS = AAVM.STATUS;
                    _CorporateNewsDBContext.Entry(PAT).State = EntityState.Modified;
                }
                _CorporateNewsDBContext.SaveChanges();
            }
            return AAVM;
        }

        public FileViewModel GetFileForDownload(Int64 id, string type)
        {
            FileViewModel flvm = new FileViewModel();
            CM_PROCESSATTACHMENT_TRN CCPT;
            CCPT = _CorporateNewsDBContext.CM_PROCESSATTACHMENT_TRN.Where(c => c.ATTACHMENTID == id).SingleOrDefault();
            if (CCPT != null)
            {
                if (type == "BANNER")
                {
                    flvm.FileName = CCPT.BANNER_NAME;
                    flvm.FileContentType = CCPT.BANNER_CONTENTTYPE;
                    flvm.File = CCPT.BANNER;
                }
                else if (type == "Attachment1")
                {
                    flvm.FileName = CCPT.ATTACHMENT1_NAME;
                    flvm.FileContentType = CCPT.ATTACHMENT1_CONTENTTYPE;
                    flvm.File = CCPT.ATTACHMENT1;
                }
                else if (type == "Attachment2")
                {
                    flvm.FileName = CCPT.ATTACHMENT2_NAME;
                    flvm.FileContentType = CCPT.ATTACHMENT2_CONTENTTYPE;
                    flvm.File = CCPT.ATTACHMENT2;
                }
            }

            return flvm;
        }
        public CorporateNewsMasterViewModel DeleteDocument(Int64 id, string type)
        {
            CorporateNewsMasterViewModel CNMV = new CorporateNewsMasterViewModel();
            CM_PROCESSATTACHMENT_TRN CCPT;
            CCPT = _CorporateNewsDBContext.CM_PROCESSATTACHMENT_TRN.Where(c => c.ATTACHMENTID == id).SingleOrDefault();
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

                _CorporateNewsDBContext.Entry(CCPT).State = EntityState.Modified;
                _CorporateNewsDBContext.SaveChanges();

                CNMV.BANNER_NAME = CCPT.BANNER_NAME;
                CNMV.ATTACHMENT1_NAME = CCPT.ATTACHMENT1_NAME;
                CNMV.ATTACHMENT2_NAME = CCPT.ATTACHMENT2_NAME;
            }
            return CNMV;
        }
        #region Newsletter
        //Added By Bhupesh - NTT for CR-4894
        public int GetNewsId()
        {
            var maxId = _CorporateNewsDBContext.NEWSLETTERS.Max(n => (int?)n.ID) ?? 0;
            return maxId + 1;
        }
        public void AddNewsLetter(NEWSLETTERS newsLetter)
        {
            try
            {
                newsLetter.ID = GetNewsId();
                _CorporateNewsDBContext.NEWSLETTERS.Add(newsLetter);
                _CorporateNewsDBContext.SaveChanges();
            }
            catch (Exception ex)
            {
                var message = ex.ToString();
            }
        }
        public List<NEWSLETTERS> GetAllNewsLetters()
        {
            return _CorporateNewsDBContext.NEWSLETTERS.Where(x => x.STATUS == "Active").ToList();

        }
        public NEWSLETTERS GetNewsLetterById(int id)
        {
            return _CorporateNewsDBContext.NEWSLETTERS.FirstOrDefault(n => n.ID == id);
        }
        public void UpdateNewsLetter(NEWSLETTERS newsletter)
        {
            var existingNewsletter = _CorporateNewsDBContext.NEWSLETTERS.Find(newsletter.ID);
            if (existingNewsletter != null)
            {
                existingNewsletter.DESCRIPTION = newsletter.DESCRIPTION;
                existingNewsletter.STATUS = newsletter.STATUS;
                existingNewsletter.DOCUMENT_NAME = newsletter.DOCUMENT_NAME;
                existingNewsletter.UPDATED_DATE = newsletter.UPDATED_DATE;
                existingNewsletter.UPDATED_BY = newsletter.UPDATED_BY;
            }
            _CorporateNewsDBContext.SaveChanges();
        }
        #endregion
    }
}


