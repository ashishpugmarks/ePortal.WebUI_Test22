using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;//Added by aumento for SR87026

namespace ePortal.Infrastructure.Repositories
{
    public class AnnouncementRepository
    {
        private readonly EPortalDGITDBContext _CommunicationDBContext;
        private readonly EPortalDBContext _AnnouncementDBContext;
        private SYKI _Syki;
        private readonly ISessionService _sessionService;
        public AnnouncementRepository(EPortalDGITDBContext CommunicationDBContext, EPortalDBContext AnnouncementDBContext, ISessionService sessionService)
        {
            _CommunicationDBContext = CommunicationDBContext;
            _AnnouncementDBContext = AnnouncementDBContext;
            _Syki = _AnnouncementDBContext.SYKI.Where(x => x.ACTIVE == 1).FirstOrDefault();
            _sessionService = sessionService;
        }

        // Content Process Master
        #region Content Process Master
        public IEnumerable<ProcessMstViewModel> BindContentProcess()
        {
            IEnumerable<ProcessMstViewModel> iList;
            iList = (from data in _AnnouncementDBContext.CM_PROCESS_MST.Where(x => x.STATUS == 1).ToList().OrderBy(o => o.CREATED_DATE)
                     select new ProcessMstViewModel
                     {
                         PROCESSID = data.MSTPROCESSID,
                         PROCESS_NAME = data.PROCESS_NAME,
                     });
            return iList;
        }
        public IEnumerable<ProcessMstViewModel> GetProcess_Mst_List()
        {
            IEnumerable<ProcessMstViewModel> iList;
            iList = (from data in _AnnouncementDBContext.CM_PROCESS_MST.ToList().OrderBy(o => o.CREATED_DATE)
                     join AD_User in _AnnouncementDBContext.ADLOGINUSER on data.CREATED_BY equals AD_User.ADEMPCODE
                     select new ProcessMstViewModel
                     {
                         PROCESSID = data.MSTPROCESSID,
                         PROCESS_NAME = data.PROCESS_NAME,
                         STATUS = (Int16)data.STATUS,
                         User_Name = AD_User.FIRSTNAME + " " + AD_User.LASTNAME,
                         CreateDate = data.CREATED_DATE.ToString("dd-MM-yyyy"),
                         DISPLAY_FILE = data.PROCESS_BANNER != null ? string.Format("data:image/jpeg;base64,{0}", Convert.ToBase64String(data.PROCESS_BANNER)) : "",
                         BANNER_CONTENTTYPE = data.BANNER_CONTENTTYPE,
                         BANNER_NAME = data.BANNER_NAME,
                     });
            return iList;
        }
        public ProcessMstViewModel SaveProcess_Mst(ProcessMstViewModel PVM)
        {
            CM_PROCESS_MST CPM = new CM_PROCESS_MST();
            CPM.PROCESS_NAME = PVM.PROCESS_NAME;
            CPM.STATUS = PVM.STATUS;
            CPM.BANNER_NAME = PVM.BANNER_NAME;
            CPM.BANNER_CONTENTTYPE = PVM.BANNER_CONTENTTYPE;
            CPM.PROCESS_BANNER = PVM.Upload_Banner;
            CPM.CREATED_DATE = DateTime.Now;
            CPM.CREATED_BY = PVM.CREATED_BY;
            _AnnouncementDBContext.CM_PROCESS_MST.Add(CPM);
            _AnnouncementDBContext.SaveChanges();
            return PVM;
        }

        public ProcessMstViewModel GetEditProcess_MstById(long id)
        {
            ProcessMstViewModel PVM = new ProcessMstViewModel();
            CM_PROCESS_MST CPM = new CM_PROCESS_MST();
            CPM = _AnnouncementDBContext.CM_PROCESS_MST.Find(id);
            if (CPM != null)
            {
                PVM.PROCESSID = CPM.MSTPROCESSID;
                PVM.PROCESS_NAME = CPM.PROCESS_NAME;
                PVM.STATUS = Convert.ToInt16(CPM.STATUS);
                PVM.BANNER_NAME = CPM.BANNER_NAME;
                PVM.BANNER_CONTENTTYPE = CPM.BANNER_CONTENTTYPE;
                PVM.Upload_Banner = CPM.PROCESS_BANNER;
            }
            return PVM;
        }

        public ProcessMstViewModel UpdateProcess_Mst(ProcessMstViewModel PVM)
        {
            CM_PROCESS_MST CPM = new CM_PROCESS_MST();
            CPM = _AnnouncementDBContext.CM_PROCESS_MST.Find(PVM.PROCESSID);
            if (CPM != null)
            {
                CPM.PROCESS_NAME = PVM.PROCESS_NAME;
                CPM.STATUS = PVM.STATUS;
                if (PVM.Upload_Banner != null)
                {
                    CPM.BANNER_NAME = PVM.BANNER_NAME;
                    CPM.BANNER_CONTENTTYPE = PVM.BANNER_CONTENTTYPE;
                    CPM.PROCESS_BANNER = PVM.Upload_Banner;
                }
                CPM.MODIFIED_DATE = DateTime.Now;
                CPM.MODIFIED_BY = PVM.MODIFIED_BY;
                _AnnouncementDBContext.Entry(CPM).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _AnnouncementDBContext.SaveChanges();
            }
            return PVM;
        }
        #endregion

        // Announcement
        #region Announcement Master
        public List<AnnouncementMasterViewModel> GetAnnouncementMasterList(Int64 UserId)
        {
            List<AnnouncementMasterViewModel> iList = new List<AnnouncementMasterViewModel>();
            var iColl = (from data in _AnnouncementDBContext.CM_PROCESSATTACHMENT_TRN.Where(x => (x.PROCESSID == 6 || x.PROCESSID == 8) && x.CREATED_BY == UserId)
                         join process in _AnnouncementDBContext.CM_PROCESS_MST on data.PROCESSID equals process.MSTPROCESSID
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
                             process.PROCESS_NAME
                         }).ToList();
            if (iColl.Count > 0)
            {
                foreach (var obj in iColl)
                {
                    iList.Add(new AnnouncementMasterViewModel
                    {
                        ATTACHMENTID = obj.ATTACHMENTID,
                        PROCESSID = obj.PROCESSID,
                        SUBJECT = obj.SUBJECT,
                        BRIEF = obj.BRIEF,
                        //DESCRIPTION = obj.DESCRIPTION,
                        DESCRIPTION = obj.DESCRIPTION == null ? obj.DESCRIPTION : CommonRepository.HtmlToText(obj.DESCRIPTION),
                        START_DATE = obj.START_DATE.ToString("dd-MMM-yyyy"),
                        END_DATE = obj.END_DATE.ToString("dd-MMM-yyyy"),
                        STATUS = (Int16)obj.STATUS,
                        CREATED_DATE = (DateTime)obj.CREATED_DATE,
                        ProcessName = obj.PROCESS_NAME
                    });
                }
            }
            return iList;
        }

        public List<AnnouncementMasterViewModel> GetAnnouncementArchiveList(Int64 UserId)
        {
            List<AnnouncementMasterViewModel> iList = new List<AnnouncementMasterViewModel>();
            var iColl = (from data in _AnnouncementDBContext.CM_PROCESSATTACHMENT_TRN.Where(x => x.PROCESSID == 8 && x.STATUS == 1)
                         join _proc in _AnnouncementDBContext.CM_PROCESS_MST on data.PROCESSID equals _proc.MSTPROCESSID
                         //join AD_User in _AnnouncementDBContext.ADLOGINUSER on data.CREATED_BY equals AD_User.ADEMPCODE
                         orderby data.CREATED_DATE descending
                         select new
                         {
                             data.ATTACHMENTID,
                             data.PROCESSID,
                             _proc.PROCESS_NAME,
                             data.SUBJECT,
                             data.DESCRIPTION,
                             data.BRIEF,
                             data.STATUS,
                             data.START_DATE,
                             data.END_DATE,
                             data.CREATED_DATE,
                             data.CIRCULARENO
                             //AD_User.AD_User.FIRSTNAME + " " + AD_User.LASTNAME
                         }).ToList();
            if (iColl.Count > 0)
            {
                foreach (var obj in iColl)
                {
                    iList.Add(new AnnouncementMasterViewModel
                    {
                        ATTACHMENTID = obj.ATTACHMENTID,
                        PROCESSID = obj.PROCESSID,
                        PROCESS_NAME = obj.PROCESS_NAME,
                        SUBJECT = obj.SUBJECT,
                        BRIEF = obj.BRIEF,
                        //DESCRIPTION = obj.DESCRIPTION,
                        DESCRIPTION = obj.DESCRIPTION == null ? obj.DESCRIPTION : CommonRepository.HtmlToText(obj.DESCRIPTION),
                        START_DATE = obj.START_DATE.ToString("dd-MMM-yyyy"),
                        END_DATE = obj.END_DATE.ToString("dd-MMM-yyyy"),
                        STATUS = (Int16)obj.STATUS,
                        CREATED_DATE = (DateTime)obj.CREATED_DATE,
                        CIRCULARENO = obj.CIRCULARENO
                    });
                }
            }
            return iList.OrderByDescending(o => o.CREATED_DATE).ToList();
        }


        public Int16 SaveProcessAttachment_Trn(AnnouncementMasterViewModel AVM)
        {
            Int16 retVal = 0;
            using (var transaction = _AnnouncementDBContext.Database.BeginTransaction())
            {
                try
                {


                    CM_PROCESSATTACHMENT_TRN CPT = new CM_PROCESSATTACHMENT_TRN();
                   

                    CPT.PROCESSID = AVM.PROCESSID;
                    CPT.START_DATE = DateTime.ParseExact(AVM.START_DATE, "dd-MMM-yyyy", null);
                    CPT.END_DATE = DateTime.ParseExact(AVM.END_DATE, "dd-MMM-yyyy", null);
                    CPT.SUBJECT = AVM.SUBJECT;
                    CPT.BRIEF = AVM.BRIEF;
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
                    //for send email by gaurav//
                    CPT.EMAILSTATUS = Convert.ToInt16(AVM.SendEmail);
                    //end
                    //for unique circular id//
                    if (AVM.PROCESSID == 8)
                    {
                        DateTime today = DateTime.Today;
                        int reqCount = _AnnouncementDBContext.CM_PROCESSATTACHMENT_TRN.Where(c => c.PROCESSID == 8 &&
                        c.CREATED_DATE.Month == today.Month &&
                        c.CREATED_DATE.Year == today.Year).Count() + 1;
                        CPT.CIRCULARENO = AVM.UniqueCirculareID + reqCount.ToString();
                    }
                    string fnIds = "";
                    if (AVM.FUNCTIONAL_DESIGNATION != null)
                    {
                        foreach (var val in AVM.FUNCTIONAL_DESIGNATION)
                        {
                            fnIds += val.ToString() + ",";
                        }
                        ;
                    }
                    string DesIds = "";
                    if (AVM.DESIGNATION != null)
                    {
                        foreach (var val in AVM.DESIGNATION)
                        {
                            DesIds += val.ToString() + ",";
                        }
                        ;
                    }
                    string SiteIds = "";
                    if (AVM.SITE != null)
                    {
                        foreach (var val in AVM.SITE)
                        {
                            SiteIds += val.ToString() + ",";
                        }
                        ;
                    }
                    CPT.FUNCTIONAL_DESIGNATION = fnIds;
                    CPT.DESIGNATION = DesIds;
                    CPT.SITE = SiteIds;
                    _AnnouncementDBContext.CM_PROCESSATTACHMENT_TRN.Add(CPT);
                    _AnnouncementDBContext.SaveChanges();

                    AnnouncementApprovalViewModel AAVM = new AnnouncementApprovalViewModel();
                    AAVM.INITIATED_BY = (long)CPT.CREATED_BY;
                    AAVM.INITIATOR_REMARKS = AVM.REMARKS;
                    AAVM.ATTACHMENTID = CPT.ATTACHMENTID;
                    SaveAttachmentApproval_Trn(AAVM);
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

        public Int16 UpdateProcessAttachment_Trn(AnnouncementMasterViewModel AVM)
        {
            Int16 retVal = 0;
            using (var transaction = _AnnouncementDBContext.Database.BeginTransaction())
            {
                try
                {
                    CM_PROCESSATTACHMENT_TRN CPT = new CM_PROCESSATTACHMENT_TRN();
                    CPT = _AnnouncementDBContext.CM_PROCESSATTACHMENT_TRN.Find(AVM.ATTACHMENTID);
                    if (CPT != null)
                    {
                        CPT.PROCESSID = AVM.PROCESSID;
                        CPT.START_DATE = DateTime.ParseExact(AVM.START_DATE, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
                        CPT.END_DATE = DateTime.ParseExact(AVM.END_DATE, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
                        //CPT.START_DATE = DateTime.ParseExact(AVM.START_DATE, "dd-MMM-yyyy", null);
                        //CPT.END_DATE = DateTime.ParseExact(AVM.END_DATE, "dd-MMM-yyyy", null);
                        CPT.SUBJECT = AVM.SUBJECT;
                        CPT.BRIEF = AVM.BRIEF;
                        CPT.DESCRIPTION = CommonRepository.TextToHtml(AVM.DESCRIPTION);
                        if (CPT.STATUS == 4)
                        {
                            CPT.STATUS = 2; // status 2 = WIP, status 4 = send back
                        }
                        if (AVM.BANNER != null)
                        {
                            CPT.BANNER_NAME = AVM.BANNER_NAME;
                            CPT.BANNER_CONTENTTYPE = AVM.BANNER_CONTENTTYPE;
                            CPT.BANNER = AVM.BANNER;
                        }
                        if (AVM.ATTACHMENT1 != null)
                        {
                            CPT.ATTACHMENT1_NAME = AVM.ATTACHMENT1_NAME;
                            CPT.ATTACHMENT1_CONTENTTYPE = AVM.ATTACHMENT1_CONTENTTYPE;
                            CPT.ATTACHMENT1 = AVM.ATTACHMENT1;
                        }
                        if (AVM.ATTACHMENT2 != null)
                        {
                            CPT.ATTACHMENT2_NAME = AVM.ATTACHMENT2_NAME;
                            CPT.ATTACHMENT2_CONTENTTYPE = AVM.ATTACHMENT2_CONTENTTYPE;
                            CPT.ATTACHMENT2 = AVM.ATTACHMENT2;
                        }
                        CPT.MODIFIED_DATE = DateTime.Now;
                        CPT.MODIFIED_BY = AVM.MODIFIED_BY;
                        string fnIds = "";
                        if (AVM.FUNCTIONAL_DESIGNATION != null)
                        {
                            foreach (var val in AVM.FUNCTIONAL_DESIGNATION)
                            {
                                fnIds += val.ToString() + ",";
                            }
                            ;
                        }
                        string DesIds = "";
                        if (AVM.DESIGNATION != null)
                        {
                            foreach (var val in AVM.DESIGNATION)
                            {
                                DesIds += val.ToString() + ",";
                            }
                            ;
                        }
                        string SiteIds = "";
                        if (AVM.SITE != null)
                        {
                            foreach (var val in AVM.SITE)
                            {
                                SiteIds += val.ToString() + ",";
                            }
                            ;
                        }
                        CPT.FUNCTIONAL_DESIGNATION = fnIds;
                        CPT.DESIGNATION = DesIds;
                        CPT.SITE = SiteIds;
                        CPT.EMAILSTATUS = Convert.ToInt16(AVM.SendEmail);
                        _AnnouncementDBContext.Entry(CPT).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        _AnnouncementDBContext.SaveChanges();

                        /// Save for AttachmentApproval
                        Int64 IsAttachment = _AnnouncementDBContext.CM_PROCESSATTACHMENTAPP_TRN.Where(x => x.ATTACHMENTID == AVM.ATTACHMENTID).Select(s => s.ATTACHMENTID).FirstOrDefault();
                        AnnouncementApprovalViewModel AAVM = new AnnouncementApprovalViewModel();
                        if (IsAttachment == 0)
                        {
                            AAVM.ATTACHMENTID = CPT.ATTACHMENTID;
                            AAVM.INITIATED_BY = (long)CPT.CREATED_BY;
                            AAVM.INITIATOR_REMARKS = AVM.REMARKS;
                            SaveAttachmentApproval_Trn(AAVM);
                        }
                        else
                        {
                            AAVM.ATTACHMENTID = CPT.ATTACHMENTID;
                            AAVM.INITIATED_BY = (long)CPT.CREATED_BY;
                            AAVM.INITIATOR_REMARKS = AVM.REMARKS;
                            UpdateAttachmentApproval_Trn(AAVM);
                        }
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

        //public AnnouncementMasterViewModel DeactiveAnnouncement([FromBody] AnnouncementMasterViewModel AVM)
        public short DeactiveAnnouncement([FromBody] AnnouncementMasterViewModel AVM)
        {
            short retVal = 0;

            CM_PROCESSATTACHMENT_TRN CPT = new CM_PROCESSATTACHMENT_TRN();
            CPT = _AnnouncementDBContext.CM_PROCESSATTACHMENT_TRN.Find(AVM.ATTACHMENTID);
            if (CPT != null)
            {
                CPT.STATUS = 0; // 0 For Deactive
                CPT.MODIFIED_DATE = DateTime.Now;
                CPT.MODIFIED_BY = AVM.MODIFIED_BY;
                _AnnouncementDBContext.Entry(CPT).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _AnnouncementDBContext.SaveChanges();
                retVal = 1;
            }
            return retVal;
            //return AVM;
        }

        //public AnnouncementMasterViewModel CancelAnnouncement([FromBody] AnnouncementMasterViewModel AVM)
        public short CancelAnnouncement(AnnouncementMasterViewModel AVM)
        {
            short retVal = 0;

            CM_PROCESSATTACHMENT_TRN CPT = new CM_PROCESSATTACHMENT_TRN();
            CM_PROCESSATTACHMENTAPP_TRN CPAT = new CM_PROCESSATTACHMENTAPP_TRN();
            CPT = _AnnouncementDBContext.CM_PROCESSATTACHMENT_TRN.Find(AVM.ATTACHMENTID);
            CPAT = _AnnouncementDBContext.CM_PROCESSATTACHMENTAPP_TRN.Where(x => x.ATTACHMENTID == AVM.ATTACHMENTID).SingleOrDefault();
            try
            {
                if (CPT != null && CPAT != null)
                {
                    CPT.STATUS = 5; // 5 For Cancel by user
                    CPT.MODIFIED_DATE = DateTime.Now;
                    CPT.MODIFIED_BY = AVM.MODIFIED_BY;
                    //// Update PROCESS ATTACHMENT APP_TRN table
                    CPAT.STATUS = 5; // 5 For Cancel by user
                    _AnnouncementDBContext.Entry(CPT).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _AnnouncementDBContext.Entry(CPAT).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _AnnouncementDBContext.SaveChanges();
                    retVal = 1;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
            //return AVM;
        }

        public AnnouncementMasterViewModel GetEditProcessAttachmentById(int id)
        {
            AnnouncementMasterViewModel AVM = new AnnouncementMasterViewModel();
            var CPM = (from data in _AnnouncementDBContext.CM_PROCESSATTACHMENT_TRN.Where(x => x.ATTACHMENTID == id)
                       join APP in _AnnouncementDBContext.CM_PROCESSATTACHMENTAPP_TRN on data.ATTACHMENTID equals APP.ATTACHMENTID
                       join PROCESS_MST in _AnnouncementDBContext.CM_PROCESS_MST on data.PROCESSID equals PROCESS_MST.MSTPROCESSID
                       select new
                       {
                           ATTACHMENTID = data.ATTACHMENTID,
                           PROCESSID = data.PROCESSID,
                           CM_PROCESS_MST = new ProcessMstViewModel
                           {
                               PROCESS_NAME = PROCESS_MST.PROCESS_NAME,
                           },
                           STATUS = data.STATUS,
                           CREATED_DATE = data.CREATED_DATE,
                           SUBJECT = data.SUBJECT,
                           BRIEF = data.BRIEF,
                           DESCRIPTION = data.DESCRIPTION,
                           START_DATE = data.START_DATE,
                           END_DATE = data.END_DATE,
                           REMARKS = APP.INITIATOR_REMARKS,
                           BANNER_NAME = data.BANNER_NAME,
                           BANNER_CONTENTTYPE = data.BANNER_CONTENTTYPE,
                           ATTACHMENT1_NAME = data.ATTACHMENT1_NAME,
                           ATTACHMENT1_CONTENTTYPE = data.ATTACHMENT1_CONTENTTYPE,
                           ATTACHMENT2_NAME = data.ATTACHMENT2_NAME,
                           ATTACHMENT2_CONTENTTYPE = data.ATTACHMENT2_CONTENTTYPE,
                           FnDesignationIds = data.FUNCTIONAL_DESIGNATION,
                           DesignationIds = data.DESIGNATION,
                           SiteIds = data.SITE,
                           EMAILSTATUS = data.EMAILSTATUS
                       }
                   ).SingleOrDefault();
            AVM.ATTACHMENTID = CPM.ATTACHMENTID;
            AVM.PROCESSID = CPM.PROCESSID;
            AVM.CM_PROCESS_MST = new ProcessMstViewModel
            {
                PROCESS_NAME = CPM.CM_PROCESS_MST.PROCESS_NAME,
            };
            AVM.STATUS = (Int16)CPM.STATUS;
            AVM.CREATED_DATE = (DateTime)CPM.CREATED_DATE;
            AVM.SUBJECT = CPM.SUBJECT;
            AVM.BRIEF = CPM.BRIEF;
            //AVM.DESCRIPTION = CPM.DESCRIPTION;
            AVM.DESCRIPTION = CPM.DESCRIPTION == null ? CPM.DESCRIPTION : CommonRepository.HtmlToText(CPM.DESCRIPTION);
            AVM.START_DATE = CPM.START_DATE.ToString("dd-MMM-yyyy");
            AVM.END_DATE = CPM.END_DATE.ToString("dd-MMM-yyyy");
            AVM.REMARKS = CPM.REMARKS;

            AVM.BANNER_NAME = CPM.BANNER_NAME;
            AVM.BANNER_CONTENTTYPE = CPM.BANNER_CONTENTTYPE;

            AVM.ATTACHMENT1_NAME = CPM.ATTACHMENT1_NAME;
            AVM.ATTACHMENT1_CONTENTTYPE = CPM.ATTACHMENT1_CONTENTTYPE;

            AVM.ATTACHMENT2_NAME = CPM.ATTACHMENT2_NAME;
            AVM.ATTACHMENT2_CONTENTTYPE = CPM.ATTACHMENT2_CONTENTTYPE;
            //if (!string.IsNullOrEmpty(CPM.FnDesignationIds))
            //{
            //    AVM.FUNCTIONAL_DESIGNATION = Array.ConvertAll(CPM.FnDesignationIds.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray(), long.Parse);
            //}
            //if (!string.IsNullOrEmpty(CPM.DesignationIds))
            //{
            //    AVM.DESIGNATION = Array.ConvertAll(CPM.DesignationIds.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray(), long.Parse);
            //}
            //if (!string.IsNullOrEmpty(CPM.SiteIds))
            //{
            //    AVM.SITE = Array.ConvertAll(CPM.SiteIds.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray(), long.Parse);
            //}
            AVM.FnDesignationDescrip = !string.IsNullOrEmpty(CPM.FnDesignationIds) ? CPM.FnDesignationIds : "";
            AVM.DesignationDescrip = !string.IsNullOrEmpty(CPM.DesignationIds) ? CPM.DesignationIds : "";
            AVM.SiteDescrip = !string.IsNullOrEmpty(CPM.SiteIds) ? CPM.SiteIds : "";
            AVM.SendEmail = CPM.EMAILSTATUS == 1 ? true : false;
            return AVM;
        }

        public AnnouncementMasterViewModel GetAnnouncementDetails(int id)
        {
            AnnouncementMasterViewModel AVM = new AnnouncementMasterViewModel();
            var obj = (from data in _AnnouncementDBContext.CM_PROCESSATTACHMENT_TRN.Where(x => x.ATTACHMENTID == id)
                       join APP in _AnnouncementDBContext.CM_PROCESSATTACHMENTAPP_TRN on data.ATTACHMENTID equals APP.ATTACHMENTID
                       join PROCESS_MST in _AnnouncementDBContext.CM_PROCESS_MST on data.PROCESSID equals PROCESS_MST.MSTPROCESSID

                       join AD_Created in _AnnouncementDBContext.ADLOGINUSER on data.CREATED_BY equals AD_Created.ADEMPCODE into CREATEDUSERJoin
                       from CREATED_User in CREATEDUSERJoin.DefaultIfEmpty()

                       join AD_INITIATED in _AnnouncementDBContext.ADLOGINUSER on APP.INITIATED_BY equals AD_INITIATED.ADEMPCODE into INITIATEDJoin
                       from INITIATED_User in INITIATEDJoin.DefaultIfEmpty()

                       join AD_HR in _AnnouncementDBContext.ADLOGINUSER on APP.HRAPP_ECODE equals AD_HR.ADEMPCODE into HRJoin
                       from HR_User in HRJoin.DefaultIfEmpty()

                       join AD_AapAuth1 in _AnnouncementDBContext.ADLOGINUSER on APP.APPAUTH1_ECODE equals AD_AapAuth1.ADEMPCODE into AAPAUTH1Join
                       from AAPAUTH1_User in AAPAUTH1Join.DefaultIfEmpty()

                       join AD_AapAuth2 in _AnnouncementDBContext.ADLOGINUSER on APP.APPAUTH2_ECODE equals AD_AapAuth2.ADEMPCODE into AAPAUTH2Join
                       from AAPAUTH2_User in AAPAUTH2Join.DefaultIfEmpty()
                       select new
                       {
                           ATTACHMENTID = data.ATTACHMENTID,
                           PROCESSID = data.PROCESSID,
                           PROCESS_NAME = PROCESS_MST.PROCESS_NAME,
                           STATUS = data.STATUS,
                           SUBJECT = data.SUBJECT,
                           BRIEF = data.BRIEF,
                           DESCRIPTION = data.DESCRIPTION,
                           START_DATE = data.START_DATE,
                           END_DATE = data.END_DATE,
                           BANNER = data.BANNER,
                           BANNER_NAME = data.BANNER_NAME,
                           BANNER_CONTENTTYPE = data.BANNER_CONTENTTYPE,
                           ATTACHMENT1_NAME = data.ATTACHMENT1_NAME,
                           ATTACHMENT1_CONTENTTYPE = data.ATTACHMENT1_CONTENTTYPE,
                           ATTACHMENT2_NAME = data.ATTACHMENT2_NAME,
                           ATTACHMENT2_CONTENTTYPE = data.ATTACHMENT2_CONTENTTYPE,
                           CREATED_DATE = data.CREATED_DATE,
                           CREATED_User = data.CREATED_DATE,
                           FnDesignationIds = data.FUNCTIONAL_DESIGNATION,
                           DesignationIds = data.DESIGNATION,
                           SiteIds = data.SITE,
                           CREATEDUser_FirstName = CREATED_User.FIRSTNAME,
                           CREATEDUser_LastName = CREATED_User.LASTNAME,
                           //// Approval table 
                           APPStatus = APP.STATUS,
                           REMARKS = APP.INITIATOR_REMARKS,
                           INITIATED_BY = APP.INITIATED_BY,
                           INITIATED_DATE = APP.INITIATED_DATE,
                           INITIATOR_REMARKS = APP.INITIATOR_REMARKS,
                           HRAPP_ECODE = APP.HRAPP_ECODE,
                           HRAPP_DATE = APP.HRAPP_DATE,
                           HRAPP_REMARKS = APP.HRAPP_REMARKS,
                           APPAUTH1_ECODE = APP.APPAUTH1_ECODE,
                           APPAUTH1_DATE = APP.APPAUTH1_DATE,
                           APPAUTH1_REMARKS = APP.APPAUTH1_REMARKS,
                           APPAUTH2_ECODE = APP.APPAUTH2_ECODE,
                           APPAUTH2_DATE = APP.APPAUTH2_DATE,
                           APPAUTH2_REMARKS = APP.APPAUTH2_REMARKS,
                           INITIATED_FirstName = INITIATED_User.FIRSTNAME,
                           INITIATED_LastName = INITIATED_User.LASTNAME,
                           HR_FirstName = HR_User.FIRSTNAME,
                           HR_LastName = HR_User.LASTNAME,
                           AppAuth1_FirstName = AAPAUTH1_User.FIRSTNAME,
                           AppAuth1_LastName = AAPAUTH1_User.LASTNAME,
                           AppAuth2_FirstName = AAPAUTH2_User.FIRSTNAME,
                           AppAuth2_LastName = AAPAUTH2_User.LASTNAME,
                       }
                    ).SingleOrDefault();
            if (obj != null)
            {
                AVM.ATTACHMENTID = obj.ATTACHMENTID;
                AVM.PROCESSID = obj.PROCESSID;
                AVM.CM_PROCESS_MST = new ProcessMstViewModel
                {
                    PROCESS_NAME = obj.PROCESS_NAME,
                };
                AVM.STATUS = (short)obj.STATUS;
                AVM.SUBJECT = obj.SUBJECT;
                AVM.BRIEF = obj.BRIEF;
                //AVM.DESCRIPTION = obj.DESCRIPTION;
                AVM.DESCRIPTION = obj.DESCRIPTION == null ? obj.DESCRIPTION : CommonRepository.HtmlToText(obj.DESCRIPTION);
                AVM.START_DATE = obj.START_DATE.ToString("dd-MMM-yyyy");
                AVM.END_DATE = obj.END_DATE.ToString("dd-MMM-yyyy");
                AVM.REMARKS = obj.REMARKS;

                AVM.BANNER = obj.BANNER;
                AVM.BANNER_NAME = obj.BANNER_NAME;
                AVM.BANNER_CONTENTTYPE = obj.BANNER_CONTENTTYPE;

                AVM.ATTACHMENT1_NAME = obj.ATTACHMENT1_NAME;
                AVM.ATTACHMENT1_CONTENTTYPE = obj.ATTACHMENT1_CONTENTTYPE;

                AVM.ATTACHMENT2_NAME = obj.ATTACHMENT2_NAME;
                AVM.ATTACHMENT2_CONTENTTYPE = obj.ATTACHMENT2_CONTENTTYPE;
                AVM.CREATED_DATE = (DateTime)obj.CREATED_DATE;
                AVM.User_Name = !string.IsNullOrEmpty(obj.CREATEDUser_FirstName) ? obj.CREATEDUser_FirstName + " " + obj.CREATEDUser_LastName + "(" + obj.CREATED_User + ")" : "";
                AVM.AnnouncementApp = new AnnouncementApprovalViewModel
                {
                    STATUS = obj.APPStatus,
                    INITIATED_User = !string.IsNullOrEmpty(obj.INITIATED_FirstName) ? obj.INITIATED_FirstName + " " + obj.INITIATED_LastName + "(" + obj.INITIATED_BY + ")" : "",
                    INITIATED_BY = obj.INITIATED_BY,
                    INITIATED_DATE = obj.INITIATED_DATE,
                    INITIATOR_REMARKS = obj.INITIATOR_REMARKS,
                    HRAPP_User = !string.IsNullOrEmpty(obj.HR_FirstName) ? obj.HR_FirstName + " " + obj.HR_LastName + "(" + obj.HRAPP_ECODE + ")" : "",
                    HRAPP_ECODE = obj.HRAPP_ECODE,
                    HRAPP_DATE = obj.HRAPP_DATE,
                    HRAPP_REMARKS = obj.HRAPP_REMARKS,
                    APPAUTH1_ECODE = obj.APPAUTH1_ECODE,
                    APPAUTH1_DATE = obj.APPAUTH1_DATE,
                    APPAUTH1_REMARKS = obj.APPAUTH1_REMARKS,
                    APPAUTH1_User = !string.IsNullOrEmpty(obj.AppAuth1_FirstName) ? obj.AppAuth1_FirstName + " " + obj.AppAuth1_LastName + "(" + obj.APPAUTH1_ECODE + ")" : "",
                    APPAUTH2_ECODE = obj.APPAUTH2_ECODE,
                    APPAUTH2_DATE = obj.APPAUTH2_DATE,
                    APPAUTH2_REMARKS = obj.APPAUTH2_REMARKS,
                    APPAUTH2_User = !string.IsNullOrEmpty(obj.AppAuth2_FirstName) ? obj.AppAuth2_FirstName + " " + obj.AppAuth2_LastName + "(" + obj.APPAUTH2_ECODE + ")" : ""
                };
                if (!string.IsNullOrEmpty(obj.FnDesignationIds))
                {
                    foreach (var fnDesId in obj.FnDesignationIds.Split(',').ToArray())
                    {
                        if (!string.IsNullOrEmpty(fnDesId))
                        {
                            long valId = Convert.ToInt64(fnDesId);
                            AVM.FnDesignationDescrip += _AnnouncementDBContext.ADFUNCTIONALDESIGNATION.Where(x => x.ADFUNCTIONALDESIGNATIONID == valId).Select(x => x.DESCRIP).SingleOrDefault() + ", ";
                        }
                    }
                }
                else
                {
                    AVM.FnDesignationDescrip = "N/A";
                }
                if (!string.IsNullOrEmpty(obj.DesignationIds))
                {
                    foreach (var DesId in obj.DesignationIds.Split(',').ToArray())
                    {
                        if (!string.IsNullOrEmpty(DesId))
                        {
                            long valId = Convert.ToInt64(DesId);
                            AVM.DesignationDescrip += _AnnouncementDBContext.ADDESIGNATION.Where(x => x.ADDESIGNATIONID == valId).Select(x => x.DESCRIP).SingleOrDefault() + ", ";
                        }
                    }
                }
                else
                {
                    AVM.DesignationDescrip = "N/A";
                }
                if (!string.IsNullOrEmpty(obj.SiteIds))
                {
                    foreach (var SiteId in obj.SiteIds.Split(',').ToArray())
                    {
                        if (!string.IsNullOrEmpty(SiteId))
                        {
                            long valId = Convert.ToInt64(SiteId);
                            AVM.SiteDescrip += _AnnouncementDBContext.SYSITE.Where(x => x.SYSITEID == valId).Select(x => x.DESCRIP).SingleOrDefault() + ", ";
                        }
                    }
                }
                else
                {
                    AVM.SiteDescrip = "N/A";
                }
            }

            return AVM;
        }

        public IEnumerable<ADFUNCTIONALDESIGNATION> Bind_ADFunctionalDesignation()
        {
            IEnumerable<ADFUNCTIONALDESIGNATION> iList;
            iList = (from data in _AnnouncementDBContext.ADFUNCTIONALDESIGNATION.Where(x => x.ACTIVE == 1).ToList()
                     select new ADFUNCTIONALDESIGNATION
                     {
                         ADFUNCTIONALDESIGNATIONID = data.ADFUNCTIONALDESIGNATIONID,
                         DESCRIP = data.DESCRIP,
                     });
            return iList;
        }
        public IEnumerable<ADDESIGNATION> Bind_ADDesignation()
        {
            IEnumerable<ADDESIGNATION> iList;
            iList = (from data in _AnnouncementDBContext.ADDESIGNATION.Where(x => x.ACTIVE == 1).ToList()
                     select new ADDESIGNATION
                     {
                         ADDESIGNATIONID = data.ADDESIGNATIONID,
                         DESCRIP = data.DESCRIP,
                     });
            return iList;
        }
        public IEnumerable<SYSITE> Bind_SYSite()
        {
            IEnumerable<SYSITE> iList;
            iList = (from data in _AnnouncementDBContext.SYSITE.Where(x => x.ACTIVE == 1).ToList()
                     select new SYSITE
                     {
                         SYSITEID = data.SYSITEID,
                         DESCRIP = data.DESCRIP,
                     });
            return iList;
        }

        public AnnouncementMasterViewModel DeleteAnnouncementFile(long id, string type)
        {
            AnnouncementMasterViewModel AVM = new AnnouncementMasterViewModel();
            CM_PROCESSATTACHMENT_TRN CPM = new CM_PROCESSATTACHMENT_TRN();
            CPM = _AnnouncementDBContext.CM_PROCESSATTACHMENT_TRN.Find(id);
            if (type == "Banner")
            {
                CPM.BANNER = null;
                CPM.BANNER_CONTENTTYPE = string.Empty;
                CPM.BANNER_NAME = string.Empty;
            }
            else if (type == "Attachment1")
            {
                CPM.ATTACHMENT1 = null;
                CPM.ATTACHMENT1_CONTENTTYPE = string.Empty;
                CPM.ATTACHMENT1_NAME = string.Empty;
            }
            else if (type == "Attachment2")
            {
                CPM.ATTACHMENT2 = null;
                CPM.ATTACHMENT2_CONTENTTYPE = string.Empty;
                CPM.ATTACHMENT2_NAME = string.Empty;
            }
            _AnnouncementDBContext.Entry(CPM).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _AnnouncementDBContext.SaveChanges();

            AVM.ATTACHMENTID = CPM.ATTACHMENTID;
            AVM.BANNER = CPM.BANNER;
            AVM.BANNER_NAME = CPM.BANNER_NAME;
            AVM.BANNER_CONTENTTYPE = CPM.BANNER_CONTENTTYPE;
            AVM.ATTACHMENT1 = CPM.ATTACHMENT1;
            AVM.ATTACHMENT1_NAME = CPM.ATTACHMENT1_NAME;
            AVM.ATTACHMENT1_CONTENTTYPE = CPM.ATTACHMENT1_CONTENTTYPE;
            AVM.ATTACHMENT2 = CPM.ATTACHMENT2;
            AVM.ATTACHMENT2_NAME = CPM.ATTACHMENT2_NAME;
            AVM.ATTACHMENT2_CONTENTTYPE = CPM.ATTACHMENT2_CONTENTTYPE;
            return AVM;

        }

        #endregion



        #region Announcement Approval
        //public IEnumerable<EmployeeViewModel> BindAllEmploye(string[] parameterValue)
        //{
        //    List<Int64?> Ids = new List<Int64?>();
        //    foreach (string arrItem in parameterValue)
        //    {
        //        Ids.Add(Convert.ToInt64(arrItem));
        //    }
        //    //List<EmployeeViewModel> iList3 = new List<EmployeeViewModel>();
        //    long SykiId = Convert.ToInt64(_AnnouncementDBContext.SYKI.Where(s => s.ACTIVE == 1).FirstOrDefault().SYKIID);
        //    List<Int64> empCode = _AnnouncementDBContext.VW_ASSOCIATELVLDETAILS.Where(g => g.SYKI == SykiId && Ids.Contains(g.ADDESIGNATIONID)).Select(s => s.ADEMPCODE).ToList();
        //    var iList1 = (from data in _AnnouncementDBContext.ADEMPLOYEE.Where(e => empCode.Contains(e.ADEMPCODE)).ToList().OrderBy(o => o.FIRSTNAME)
        //                                             select new
        //                                             {
        //                                                 EmpId = data.ADEMPCODE,
        //                                                 Name = data.FIRSTNAME + " " + data.LASTNAME,
        //                                             });

        //    var iList2 = (from data in _AnnouncementDBContext.ADORGLEVEL.Where(x => x.ACTIVE == 1 && x.SYKIID == SykiId && x.ADORGLEVELTYPEID == 1)
        //                                             join tbORGLEVELHEAD in _AnnouncementDBContext.ADORGLEVELHEAD.Where(h => h.ISACTIVE == 1) on data.ADORGLEVELID equals tbORGLEVELHEAD.ADORGLEVELID
        //                                             join tbLoginUser in _AnnouncementDBContext.ADLOGINUSER.Where(u => u.ACTIVE == 1) on tbORGLEVELHEAD.ADEMPCODE equals tbLoginUser.ADEMPCODE
        //                                             select new 
        //                                             {
        //                                                 EmpId = tbORGLEVELHEAD.ADEMPCODE,
        //                                                 Name = tbLoginUser.FIRSTNAME + " " + tbLoginUser.LASTNAME,
        //                                             });

        //    var iList3 = iList1.Concat(iList2);

        //    return (IEnumerable<EmployeeViewModel>)iList3;
        //}

        public string IsEmployeeActive(Int64 EmpCode)
        {
            string msgResult = "";
            //System.Boolean isExist = _AnnouncementDBContext.ADEMPLOYEE.Any(x => x.ADEMPCODE == EmpCode);
            //var isExist = _AnnouncementDBContext.ADEMPLOYEE.Any(x => x.ADEMPCODE == EmpCode);
            int count = _AnnouncementDBContext.ADEMPLOYEE
                .Count(x => x.ADEMPCODE == EmpCode);

            if (count > 0)
            {
                ADEMPLOYEE activeEmp = _AnnouncementDBContext.ADEMPLOYEE.Where(x => x.ADEMPCODE == EmpCode && x.ACTIVE == 1).SingleOrDefault();
                if (activeEmp != null)
                {
                    msgResult = !string.IsNullOrEmpty(activeEmp.FIRSTNAME) ? activeEmp.FIRSTNAME + " " + activeEmp.LASTNAME + "(" + activeEmp.ADEMPCODE + ")" : "";
                }
                else
                {
                    msgResult = "Deactive";
                }
            }
            return msgResult;
        }

        public CM_PROCESSATTACHMENTAPP_TRN SaveAttachmentApproval_Trn(AnnouncementApprovalViewModel AAVM)
        {
            CM_PROCESSATTACHMENTAPP_TRN CPAT = new CM_PROCESSATTACHMENTAPP_TRN();
           
            CPAT.ATTACHMENTID = AAVM.ATTACHMENTID;
            CPAT.STATUS = 1; // status 1 = Initiated
            CPAT.INITIATED_BY = AAVM.INITIATED_BY;
            CPAT.INITIATED_DATE = DateTime.Now;
            CPAT.INITIATOR_REMARKS = AAVM.INITIATOR_REMARKS.Trim();
            _AnnouncementDBContext.CM_PROCESSATTACHMENTAPP_TRN.Add(CPAT);
            _AnnouncementDBContext.SaveChanges();
            return CPAT;
        }

        public CM_PROCESSATTACHMENTAPP_TRN UpdateAttachmentApproval_Trn(AnnouncementApprovalViewModel AAVM)
        {
            CM_PROCESSATTACHMENTAPP_TRN CPAT = new CM_PROCESSATTACHMENTAPP_TRN();
            CPAT = _AnnouncementDBContext.CM_PROCESSATTACHMENTAPP_TRN.Where(x => x.ATTACHMENTID == AAVM.ATTACHMENTID).SingleOrDefault();
            if (CPAT != null)
            {
                int statusValue = CPAT.STATUS == 6 ? 1 : CPAT.STATUS == 7 ? 2 : CPAT.STATUS == 8 ? 2 : 1;
                if (CPAT.STATUS == 6)
                {
                    CPAT.HRAPP_DATE = null;
                    CPAT.HRAPP_ECODE = null;
                    CPAT.HRAPP_REMARKS = null;
                    CPAT.APPAUTH1_DATE = null;
                    CPAT.APPAUTH1_ECODE = null;
                    CPAT.APPAUTH1_REMARKS = null;
                    CPAT.APPAUTH2_DATE = null;
                    CPAT.APPAUTH2_ECODE = null;
                    CPAT.APPAUTH2_REMARKS = null;
                }
                if (CPAT.STATUS == 7 || CPAT.STATUS == 8)
                {
                    CPAT.APPAUTH1_DATE = null;
                    CPAT.APPAUTH1_REMARKS = null;
                    CPAT.APPAUTH2_DATE = null;
                    CPAT.APPAUTH2_REMARKS = null;
                }
                CPAT.STATUS = Convert.ToInt16(statusValue); // status 1 = Initiated, status 6 = send Back by Hr,  status 7 = send Back by Approval1,  status 8 = send Back by Approval2
                CPAT.INITIATED_BY = AAVM.INITIATED_BY;
                CPAT.INITIATOR_REMARKS = AAVM.INITIATOR_REMARKS;
                _AnnouncementDBContext.Entry(CPAT).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _AnnouncementDBContext.SaveChanges();
            }
            return CPAT;
        }

        public IEnumerable<AnnouncementApprovalViewModel> GetAnnouncementApprovalAuthorityList()
        {
            List<AnnouncementApprovalViewModel> iList = new List<AnnouncementApprovalViewModel>();

            var iColl = (from data in _AnnouncementDBContext.CM_PROCESSATTACHMENTAPP_TRN.Where(x => x.STATUS == 1) // && x.APPAUTH1_ECODE == null && x.APPAUTH2_ECODE == null
                         join tbTRN in _AnnouncementDBContext.CM_PROCESSATTACHMENT_TRN.Where(x => x.PROCESSID == 6 || x.PROCESSID == 8) on data.ATTACHMENTID equals tbTRN.ATTACHMENTID
                         join AD_INITIATED in _AnnouncementDBContext.ADLOGINUSER on data.INITIATED_BY equals AD_INITIATED.ADEMPCODE into INITIATEDJoin
                         from INITIATED_User in INITIATEDJoin.DefaultIfEmpty()
                         join AD_HR in _AnnouncementDBContext.ADLOGINUSER on data.HRAPP_ECODE equals AD_HR.ADEMPCODE into HRJoin
                         from HR_User in HRJoin.DefaultIfEmpty()
                         orderby data.INITIATED_DATE descending
                         select new
                         {
                             data.ATTACHMENTAPPID,
                             data.ATTACHMENTID,
                             tbTRN.SUBJECT,
                             tbTRN.BRIEF,
                             data.STATUS,
                             data.INITIATED_BY,
                             data.INITIATED_DATE,
                             data.INITIATOR_REMARKS,
                             data.HRAPP_ECODE,
                             data.HRAPP_DATE,
                             data.HRAPP_REMARKS,
                             data.APPAUTH1_ECODE,
                             data.APPAUTH1_DATE,
                             data.APPAUTH1_REMARKS,
                             data.APPAUTH2_ECODE,
                             data.APPAUTH2_DATE,
                             data.APPAUTH2_REMARKS,
                             INITIATED_User.FIRSTNAME,
                             INITIATED_User.LASTNAME,
                             HR_FirstName = HR_User.FIRSTNAME,
                             HR_LastName = HR_User.LASTNAME,

                         }).ToList();
            if (iColl.Count > 0)
            {
                foreach (var obj in iColl)
                {
                    iList.Add(new AnnouncementApprovalViewModel
                    {
                        ATTACHMENTAPPID = obj.ATTACHMENTAPPID,
                        ATTACHMENTID = obj.ATTACHMENTID,
                        AnnouncementTrn = new AnnouncementMasterViewModel
                        {
                            SUBJECT = obj.SUBJECT,
                            BRIEF = obj.BRIEF,
                        },
                        STATUS = obj.STATUS,
                        INITIATED_User = !string.IsNullOrEmpty(obj.FIRSTNAME) ? obj.FIRSTNAME + " " + obj.LASTNAME + "(" + obj.INITIATED_BY + ")" : "",
                        INITIATED_BY = obj.INITIATED_BY,
                        INITIATED_DATE = obj.INITIATED_DATE,
                        INITIATOR_REMARKS = obj.INITIATOR_REMARKS,
                        HRAPP_User = !string.IsNullOrEmpty(obj.HR_FirstName) ? obj.HR_FirstName + " " + obj.HR_LastName + "(" + obj.HRAPP_ECODE + ")" : "",
                        HRAPP_ECODE = obj.HRAPP_ECODE,
                        HRAPP_DATE = obj.HRAPP_DATE,
                        HRAPP_REMARKS = obj.HRAPP_REMARKS,
                        APPAUTH1_ECODE = obj.APPAUTH1_ECODE,
                        APPAUTH1_DATE = obj.APPAUTH1_DATE,
                        APPAUTH1_REMARKS = obj.APPAUTH1_REMARKS,
                        APPAUTH2_ECODE = obj.APPAUTH2_ECODE,
                        APPAUTH2_DATE = obj.APPAUTH2_DATE,
                        APPAUTH2_REMARKS = obj.APPAUTH2_REMARKS,
                    });
                }
            }
            return iList;
        }

        public AnnouncementApprovalViewModel GetAnnouncementApprovalAuthorityById(int id)
        {
            var obj = (from data in _AnnouncementDBContext.CM_PROCESSATTACHMENTAPP_TRN.Where(x => x.ATTACHMENTAPPID == id)
                       join tbTRN in _AnnouncementDBContext.CM_PROCESSATTACHMENT_TRN on data.ATTACHMENTID equals tbTRN.ATTACHMENTID

                       join AD_INITIATED in _AnnouncementDBContext.ADLOGINUSER on data.INITIATED_BY equals AD_INITIATED.ADEMPCODE into INITIATEDJoin
                       from INITIATED_User in INITIATEDJoin.DefaultIfEmpty()

                       join AD_HR in _AnnouncementDBContext.ADLOGINUSER on data.HRAPP_ECODE equals AD_HR.ADEMPCODE into HRJoin
                       from HR_User in HRJoin.DefaultIfEmpty()

                       join AD_AapAuth1 in _AnnouncementDBContext.ADLOGINUSER on data.APPAUTH1_ECODE equals AD_AapAuth1.ADEMPCODE into AAPAUTH1Join
                       from AAPAUTH1_User in AAPAUTH1Join.DefaultIfEmpty()

                       join AD_AapAuth2 in _AnnouncementDBContext.ADLOGINUSER on data.APPAUTH2_ECODE equals AD_AapAuth2.ADEMPCODE into AAPAUTH2Join
                       from AAPAUTH2_User in AAPAUTH2Join.DefaultIfEmpty()
                       select new
                       {
                           data.ATTACHMENTAPPID,
                           data.ATTACHMENTID,
                           data.STATUS,
                           tbTRN.SUBJECT,
                           tbTRN.BRIEF,
                           tbTRN.DESCRIPTION,
                           tbTRN.START_DATE,
                           tbTRN.END_DATE,
                           tbTRN.BANNER_NAME,
                           tbTRN.BANNER_CONTENTTYPE,
                           tbTRN.ATTACHMENT1_NAME,
                           tbTRN.ATTACHMENT1_CONTENTTYPE,
                           tbTRN.ATTACHMENT2_NAME,
                           tbTRN.ATTACHMENT2_CONTENTTYPE,
                           FnDesignationIds = tbTRN.FUNCTIONAL_DESIGNATION,
                           DesignationIds = tbTRN.DESIGNATION,
                           SiteIds = tbTRN.SITE,
                           data.INITIATED_BY,
                           data.INITIATED_DATE,
                           data.INITIATOR_REMARKS,
                           data.HRAPP_ECODE,
                           data.HRAPP_DATE,
                           data.HRAPP_REMARKS,
                           data.APPAUTH1_ECODE,
                           data.APPAUTH1_DATE,
                           data.APPAUTH1_REMARKS,
                           data.APPAUTH2_ECODE,
                           data.APPAUTH2_DATE,
                           data.APPAUTH2_REMARKS,
                           INITIATED_User.FIRSTNAME,
                           INITIATED_User.LASTNAME,
                           HR_FirstName = HR_User.FIRSTNAME,
                           HR_LastName = HR_User.LASTNAME,
                           AppAuth1_FirstName = AAPAUTH1_User.FIRSTNAME,
                           AppAuth1_LastName = AAPAUTH1_User.LASTNAME,
                           AppAuth2_FirstName = AAPAUTH2_User.FIRSTNAME,
                           AppAuth2_LastName = AAPAUTH2_User.LASTNAME,
                       }).SingleOrDefault();

            string FnDesignationDescrip = string.Empty;
            string DesignationDescrip = string.Empty;
            string SiteDescrip = string.Empty;
            if (!string.IsNullOrEmpty(obj?.FnDesignationIds))
            {
                foreach (var fnDesId in obj.FnDesignationIds.Split(',').ToArray())
                {
                    if (!string.IsNullOrEmpty(fnDesId))
                    {
                        long valId = Convert.ToInt64(fnDesId);
                        FnDesignationDescrip += _AnnouncementDBContext.ADFUNCTIONALDESIGNATION.Where(x => x.ADFUNCTIONALDESIGNATIONID == valId).Select(x => x.DESCRIP).SingleOrDefault() + ", ";
                    }
                }
            }
            if (!string.IsNullOrEmpty(obj?.DesignationIds))
            {
                foreach (var DesId in obj.DesignationIds.Split(',').ToArray())
                {
                    if (!string.IsNullOrEmpty(DesId))
                    {
                        long valId = Convert.ToInt64(DesId);
                        DesignationDescrip += _AnnouncementDBContext.ADDESIGNATION.Where(x => x.ADDESIGNATIONID == valId).Select(x => x.DESCRIP).SingleOrDefault() + ", ";
                    }
                }
            }
            if (!string.IsNullOrEmpty(obj?.SiteIds))
            {
                foreach (var SiteId in obj.SiteIds.Split(',').ToArray())
                {
                    if (!string.IsNullOrEmpty(SiteId))
                    {
                        long valId = Convert.ToInt64(SiteId);
                        SiteDescrip += _AnnouncementDBContext.SYSITE.Where(x => x.SYSITEID == valId).Select(x => x.DESCRIP).SingleOrDefault() + ", ";
                    }
                }
            }

            AnnouncementApprovalViewModel CAAVT = new AnnouncementApprovalViewModel
            {
                ATTACHMENTAPPID = obj.ATTACHMENTAPPID,
                ATTACHMENTID = obj.ATTACHMENTID,
                STATUS = obj.STATUS,
                AnnouncementTrn = new AnnouncementMasterViewModel
                {
                    SUBJECT = obj.SUBJECT,
                    BRIEF = obj.BRIEF,
                    START_DATE = obj.START_DATE.ToString("dd-MMM-yyyy"),
                    END_DATE = obj.END_DATE.ToString("dd-MMM-yyyy"),
                    DESCRIPTION = obj.DESCRIPTION,
                    BANNER_NAME = obj.BANNER_NAME,
                    BANNER_CONTENTTYPE = obj.BANNER_CONTENTTYPE,
                    ATTACHMENT1_NAME = obj.ATTACHMENT1_NAME,
                    ATTACHMENT1_CONTENTTYPE = obj.ATTACHMENT1_CONTENTTYPE,
                    ATTACHMENT2_NAME = obj.ATTACHMENT2_NAME,
                    ATTACHMENT2_CONTENTTYPE = obj.ATTACHMENT2_CONTENTTYPE,
                    FnDesignationDescrip = !string.IsNullOrEmpty(FnDesignationDescrip) ? FnDesignationDescrip : "N/A",
                    DesignationDescrip = !string.IsNullOrEmpty(DesignationDescrip) ? DesignationDescrip : "N/A",
                    SiteDescrip = !string.IsNullOrEmpty(SiteDescrip) ? SiteDescrip : "N/A",
                },
                INITIATED_User = !string.IsNullOrEmpty(obj.FIRSTNAME) ? obj.FIRSTNAME + " " + obj.LASTNAME + "(" + obj.INITIATED_BY + ")" : "",
                INITIATED_BY = obj.INITIATED_BY,
                INITIATED_DATE = obj.INITIATED_DATE,
                INITIATOR_REMARKS = obj.INITIATOR_REMARKS,
                HRAPP_User = !string.IsNullOrEmpty(obj.HR_FirstName) ? obj.HR_FirstName + " " + obj.HR_LastName + "(" + obj.HRAPP_ECODE + ")" : "",
                HRAPP_ECODE = obj.HRAPP_ECODE,
                HRAPP_DATE = obj.HRAPP_DATE,
                HRAPP_REMARKS = obj.HRAPP_REMARKS,
                APPAUTH1_ECODE = obj.APPAUTH1_ECODE,
                APPAUTH1_DATE = obj.APPAUTH1_DATE,
                APPAUTH1_REMARKS = obj.APPAUTH1_REMARKS,
                APPAUTH1_User = !string.IsNullOrEmpty(obj.AppAuth1_FirstName) ? obj.AppAuth1_FirstName + " " + obj.AppAuth1_LastName + "(" + obj.APPAUTH1_ECODE + ")" : "",
                APPAUTH2_ECODE = obj.APPAUTH2_ECODE,
                APPAUTH2_DATE = obj.APPAUTH2_DATE,
                APPAUTH2_REMARKS = obj.APPAUTH2_REMARKS,
                APPAUTH2_User = !string.IsNullOrEmpty(obj.AppAuth2_FirstName) ? obj.AppAuth2_FirstName + " " + obj.AppAuth2_LastName + "(" + obj.APPAUTH2_ECODE + ")" : ""
            };
            return CAAVT;
        }

        public AnnouncementApprovalViewModel SaveHRApproval(AnnouncementApprovalViewModel AAVM)
        {
            CM_PROCESSATTACHMENT_TRN PAT = new CM_PROCESSATTACHMENT_TRN();
            CM_PROCESSATTACHMENTAPP_TRN CPAT = new CM_PROCESSATTACHMENTAPP_TRN();
            CPAT = _AnnouncementDBContext.CM_PROCESSATTACHMENTAPP_TRN.Find(AAVM.ATTACHMENTAPPID);
            if (CPAT != null)
            {
                CPAT.STATUS = AAVM.STATUS;
                CPAT.HRAPP_ECODE = AAVM.HRAPP_ECODE;
                CPAT.HRAPP_DATE = DateTime.Now;
                CPAT.HRAPP_REMARKS = AAVM.HRAPP_REMARKS;
                CPAT.APPAUTH1_ECODE = AAVM.APPAUTH1_ECODE;
                CPAT.APPAUTH2_ECODE = AAVM.APPAUTH2_ECODE;
                _AnnouncementDBContext.Entry(CPAT).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                if (CPAT.STATUS == 9 || CPAT.STATUS == 6)// 9:Rejected,  6:SendBack
                {
                    PAT = _AnnouncementDBContext.CM_PROCESSATTACHMENT_TRN.Find(CPAT.ATTACHMENTID);
                    if (PAT != null)
                    {
                        int status = CPAT.STATUS == 9 ? 3 : 4;  // 3: Rejected,  4: SendBack
                        PAT.STATUS = Convert.ToInt16(status);
                        _AnnouncementDBContext.Entry(PAT).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    }
                }
                _AnnouncementDBContext.SaveChanges();
            }
            return AAVM;
        }

        public IEnumerable<AnnouncementApprovalViewModel> GetAnnouncementApprovalList(Int64 UserId)
        {
            List<AnnouncementApprovalViewModel> iList = new List<AnnouncementApprovalViewModel>();
            var iColl = (from data in _AnnouncementDBContext.CM_PROCESSATTACHMENTAPP_TRN.Where(x => (x.APPAUTH1_ECODE == UserId && x.STATUS == 2) || (x.APPAUTH2_ECODE == UserId && x.STATUS == 3))
                         join tbTRN in _AnnouncementDBContext.CM_PROCESSATTACHMENT_TRN.Where(x => x.PROCESSID == 6 || x.PROCESSID == 8) on data.ATTACHMENTID equals tbTRN.ATTACHMENTID
                         join AD_INITIATED in _AnnouncementDBContext.ADLOGINUSER on data.INITIATED_BY equals AD_INITIATED.ADEMPCODE into INITIATEDJoin
                         from INITIATED_User in INITIATEDJoin.DefaultIfEmpty()
                         join AD_HR in _AnnouncementDBContext.ADLOGINUSER on data.HRAPP_ECODE equals AD_HR.ADEMPCODE into HRJoin
                         from HR_User in HRJoin.DefaultIfEmpty()
                         orderby data.INITIATED_DATE descending
                         select new
                         {
                             data.ATTACHMENTAPPID,
                             data.ATTACHMENTID,
                             data.STATUS,
                             tbTRN.SUBJECT,
                             tbTRN.BRIEF,
                             tbTRN.DESCRIPTION,
                             tbTRN.START_DATE,
                             tbTRN.END_DATE,
                             tbTRN.BANNER_NAME,
                             tbTRN.BANNER_CONTENTTYPE,
                             tbTRN.ATTACHMENT1_NAME,
                             tbTRN.ATTACHMENT1_CONTENTTYPE,
                             tbTRN.ATTACHMENT2_NAME,
                             tbTRN.ATTACHMENT2_CONTENTTYPE,
                             FnDesignationIds = tbTRN.FUNCTIONAL_DESIGNATION,
                             DesignationIds = tbTRN.DESIGNATION,
                             SiteIds = tbTRN.SITE,
                             data.INITIATED_BY,
                             data.INITIATED_DATE,
                             data.INITIATOR_REMARKS,
                             data.HRAPP_ECODE,
                             data.HRAPP_DATE,
                             data.HRAPP_REMARKS,
                             data.APPAUTH1_ECODE,
                             data.APPAUTH1_DATE,
                             data.APPAUTH1_REMARKS,
                             data.APPAUTH2_ECODE,
                             data.APPAUTH2_DATE,
                             data.APPAUTH2_REMARKS,
                             INITIATED_User.FIRSTNAME,
                             INITIATED_User.LASTNAME,
                             HR_FirstName = HR_User.FIRSTNAME,
                             HR_LastName = HR_User.LASTNAME,

                         }).ToList();
            if (iColl.Count > 0)
            {
                foreach (var obj in iColl)
                {
                    string FnDesignationDescrip = string.Empty;
                    string DesignationDescrip = string.Empty;
                    string SiteDescrip = string.Empty;
                    if (!string.IsNullOrEmpty(obj.FnDesignationIds))
                    {
                        foreach (var fnDesId in obj.FnDesignationIds.Split(',').ToArray())
                        {
                            if (!string.IsNullOrEmpty(fnDesId))
                            {
                                long valId = Convert.ToInt64(fnDesId);
                                FnDesignationDescrip += _AnnouncementDBContext.ADFUNCTIONALDESIGNATION.Where(x => x.ADFUNCTIONALDESIGNATIONID == valId).Select(x => x.DESCRIP).SingleOrDefault() + ", ";
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(obj.DesignationIds))
                    {
                        foreach (var DesId in obj.DesignationIds.Split(',').ToArray())
                        {
                            if (!string.IsNullOrEmpty(DesId))
                            {
                                long valId = Convert.ToInt64(DesId);
                                DesignationDescrip += _AnnouncementDBContext.ADDESIGNATION.Where(x => x.ADDESIGNATIONID == valId).Select(x => x.DESCRIP).SingleOrDefault() + ", ";
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(obj.SiteIds))
                    {
                        foreach (var SiteId in obj.SiteIds.Split(',').ToArray())
                        {
                            if (!string.IsNullOrEmpty(SiteId))
                            {
                                long valId = Convert.ToInt64(SiteId);
                                SiteDescrip += _AnnouncementDBContext.SYSITE.Where(x => x.SYSITEID == valId).Select(x => x.DESCRIP).SingleOrDefault() + ", ";
                            }
                        }
                    }
                    iList.Add(new AnnouncementApprovalViewModel
                    {
                        ATTACHMENTAPPID = obj.ATTACHMENTAPPID,
                        ATTACHMENTID = obj.ATTACHMENTID,
                        AnnouncementTrn = new AnnouncementMasterViewModel
                        {
                            SUBJECT = obj.SUBJECT,
                            BRIEF = obj.BRIEF,
                            //DESCRIPTION = obj.DESCRIPTION,
                            DESCRIPTION = obj.DESCRIPTION == null ? obj.DESCRIPTION : CommonRepository.HtmlToText(obj.DESCRIPTION),
                            BANNER_NAME = obj.BANNER_NAME,
                            BANNER_CONTENTTYPE = obj.BANNER_CONTENTTYPE,
                            ATTACHMENT1_NAME = obj.ATTACHMENT1_NAME,
                            ATTACHMENT1_CONTENTTYPE = obj.ATTACHMENT1_CONTENTTYPE,
                            ATTACHMENT2_NAME = obj.ATTACHMENT2_NAME,
                            ATTACHMENT2_CONTENTTYPE = obj.ATTACHMENT2_CONTENTTYPE,
                            FnDesignationDescrip = !string.IsNullOrEmpty(FnDesignationDescrip) ? FnDesignationDescrip : "N/A",
                            DesignationDescrip = !string.IsNullOrEmpty(DesignationDescrip) ? DesignationDescrip : "N/A",
                            SiteDescrip = !string.IsNullOrEmpty(SiteDescrip) ? SiteDescrip : "N/A",
                        },
                        STATUS = obj.STATUS,
                        INITIATED_User = !string.IsNullOrEmpty(obj.FIRSTNAME) ? obj.FIRSTNAME + " " + obj.LASTNAME + "(" + obj.INITIATED_BY + ")" : "",
                        INITIATED_BY = obj.INITIATED_BY,
                        INITIATED_DATE = obj.INITIATED_DATE,
                        INITIATOR_REMARKS = obj.INITIATOR_REMARKS,
                        HRAPP_User = !string.IsNullOrEmpty(obj.HR_FirstName) ? obj.HR_FirstName + " " + obj.HR_LastName + "(" + obj.HRAPP_ECODE + ")" : "",
                        HRAPP_ECODE = obj.HRAPP_ECODE,
                        HRAPP_DATE = obj.HRAPP_DATE,
                        HRAPP_REMARKS = obj.HRAPP_REMARKS,
                        APPAUTH1_ECODE = obj.APPAUTH1_ECODE,
                        APPAUTH1_DATE = obj.APPAUTH1_DATE,
                        APPAUTH1_REMARKS = obj.APPAUTH1_REMARKS,
                        APPAUTH2_ECODE = obj.APPAUTH2_ECODE,
                        APPAUTH2_DATE = obj.APPAUTH2_DATE,
                        APPAUTH2_REMARKS = obj.APPAUTH2_REMARKS,
                    });
                }
            }
            return iList;
        }

        public AnnouncementApprovalViewModel UpdateStatus(AnnouncementApprovalViewModel AAVM)
        {
            CM_PROCESSATTACHMENT_TRN PAT = new CM_PROCESSATTACHMENT_TRN();
            CM_PROCESSATTACHMENTAPP_TRN CPAT = new CM_PROCESSATTACHMENTAPP_TRN();
            CPAT = _AnnouncementDBContext.CM_PROCESSATTACHMENTAPP_TRN.Find(AAVM.ATTACHMENTAPPID);
            if (CPAT != null)
            {
                if (CPAT.APPAUTH1_ECODE == CPAT.APPAUTH2_ECODE && CPAT.STATUS == 2) // this case approval 1 and approval 2 user are same
                {
                    CPAT.STATUS = 4;
                    CPAT.APPAUTH1_DATE = DateTime.Now;
                    CPAT.APPAUTH1_REMARKS = AAVM.APPAUTH1_REMARKS;
                    CPAT.APPAUTH2_DATE = DateTime.Now;
                    CPAT.APPAUTH2_REMARKS = AAVM.APPAUTH2_REMARKS;
                }
                else if (CPAT.APPAUTH1_ECODE == AAVM.APPAUTH1_ECODE && CPAT.STATUS == 2)
                {
                    CPAT.STATUS = AAVM.STATUS;
                    CPAT.APPAUTH1_DATE = DateTime.Now;
                    CPAT.APPAUTH1_REMARKS = AAVM.APPAUTH1_REMARKS;
                }
                else if (CPAT.APPAUTH2_ECODE == AAVM.APPAUTH2_ECODE && CPAT.STATUS == 3)
                {
                    CPAT.STATUS = AAVM.STATUS;
                    CPAT.APPAUTH2_DATE = DateTime.Now;
                    CPAT.APPAUTH2_REMARKS = AAVM.APPAUTH2_REMARKS;
                }

                _AnnouncementDBContext.Entry(CPAT).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                if (CPAT.STATUS == 10 || CPAT.STATUS == 7)
                {
                    PAT = _AnnouncementDBContext.CM_PROCESSATTACHMENT_TRN.Find(CPAT.ATTACHMENTID);
                    if (PAT != null)
                    {
                        int status = CPAT.STATUS == 10 ? 3 : 4;  // 2:WIP,  3:Rejected,  4:SendBack
                        PAT.STATUS = Convert.ToInt16(status);
                        _AnnouncementDBContext.Entry(PAT).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    }
                }
                else if (CPAT.STATUS == 11 || CPAT.STATUS == 8 || CPAT.STATUS == 4)
                {
                    PAT = _AnnouncementDBContext.CM_PROCESSATTACHMENT_TRN.Find(CPAT.ATTACHMENTID);
                    if (PAT != null)
                    {
                        int status = CPAT.STATUS == 4 ? 1 : CPAT.STATUS == 11 ? 3 : 4;  // 1:Active,  2:WIP,  3:Rejected,  4:SendBack
                        PAT.STATUS = Convert.ToInt16(status);
                        _AnnouncementDBContext.Entry(PAT).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    }
                }
                _AnnouncementDBContext.SaveChanges();
            }
            return AAVM;
        }

        public FileViewModel GetFileForDownload(Int64 AttachmentId, string Attachment)
        {
            FileViewModel file = new FileViewModel();
            CM_PROCESSATTACHMENT_TRN Item = _AnnouncementDBContext.CM_PROCESSATTACHMENT_TRN.Where(x => x.ATTACHMENTID == AttachmentId).SingleOrDefault();
            if (Item != null)
            {
                if (Attachment == "Attachment1")
                {
                    file.FileName = Item.ATTACHMENT1_NAME;
                    file.FileContentType = Item.ATTACHMENT1_CONTENTTYPE;
                    file.File = Item.ATTACHMENT1;
                }
                else if (Attachment == "Attachment2")
                {
                    file.FileName = Item.ATTACHMENT2_NAME;
                    file.FileContentType = Item.ATTACHMENT2_CONTENTTYPE;
                    file.File = Item.ATTACHMENT2;
                }
                else if (Attachment == "Banner")
                {
                    file.FileName = Item.BANNER_NAME;
                    file.FileContentType = Item.BANNER_CONTENTTYPE;
                    file.File = Item.BANNER;
                }
            }
            return file;
        }
        #endregion

        #region Communication Proess
        public IEnumerable<CommCategoryViewModel> BindCommCategory()
        {
            IEnumerable<CommCategoryViewModel> iList;
            iList = (from data in _CommunicationDBContext.CM_COMMCATEGORY_MST.Where(x => x.STATUS == 1)
                     select new CommCategoryViewModel
                     {
                         COMMCATEGORYID = data.COMMCATEGORYID,
                         CATEGORY_NAME = data.CATEGORY_NAME,
                     });
            return iList;
        }

        public CommCategoryViewModel GetCommCategoryById(long categoryId)
        {
            CommCategoryViewModel obj;
            obj = (from data in _CommunicationDBContext.CM_COMMCATEGORY_MST.Where(x => x.COMMCATEGORYID == categoryId)
                   select new CommCategoryViewModel
                   {
                       COMMCATEGORYID = data.COMMCATEGORYID,
                       CATEGORY_NAME = data.CATEGORY_NAME,
                       APP_REQUIRED_TILL = data.APP_REQUIRED_TILL,
                       ISREQ_TEMPLATE = data.ISREQ_TEMPLATE,
                       ISREQ_BANNER = data.ISREQ_BANNER,
                       ISREQ_OTHER = data.ISREQ_OTHER,
                   }).FirstOrDefault();
            return obj;
        }

        public IEnumerable<CommMailTypeViewModel> BindCommMailType()
        {
            IEnumerable<CommMailTypeViewModel> iList;
            iList = (from data in _CommunicationDBContext.CM_COMMTYPE_MST.Where(x => x.STATUS == 1)
                     select new CommMailTypeViewModel
                     {
                         COMMTYPEID = data.COMMTYPEID,
                         COMM_TYPE = data.COMM_TYPE,
                     }).OrderBy(o => o.COMM_TYPE);
            return iList;
        }

        public List<CommunicationViewModel> CommunicationRequestList(long userId)
        {
            var iList = (from data in _CommunicationDBContext.CM_COMMUNICATION_HDR
                         join _cat in _CommunicationDBContext.CM_COMMCATEGORY_MST on data.COMM_CATID equals _cat.COMMCATEGORYID
                         join _type in _CommunicationDBContext.CM_COMMTYPE_MST on data.COMM_MAILTYPEID equals _type.COMMTYPEID
                         join _AddBy in _CommunicationDBContext.ADEMPLOYEE on data.CREATED_BY equals _AddBy.ADEMPCODE
                         where data.CREATED_BY == userId
                         select new CommunicationViewModel
                         {
                             COMMUNICATIONID = data.COMMUNICATIONID,
                             COMM_CATID = _cat.COMMCATEGORYID,
                             CATEGORY = _cat.CATEGORY_NAME,
                             COMM_MAILTYPEID = _type.COMMTYPEID,
                             MAILTYPE = _type.COMM_TYPE,
                             MAILID = _type.COMM_EMAILID,
                             CONTENT = data.CONTENT,
                             SUBJECT = data.SUBJECT,
                             _SDATE = data.START_DATE,
                             _EDATE = data.END_DATE,
                             //EMAILSTATUS = data.EMAILSTATUS == 1 ? true : false,
                             EMAILSTATUS = Convert.ToBoolean(data.EMAILSTATUS),
                             CREATED_BY = data.CREATED_BY,
                             CREATED_BY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             CREATED_DATE = data.CREATED_DATE,
                             PROCESS_STATUS = data.PROCESS_STATUS,
                             STATUS = data.STATUS,
                             REQUEST_TYPE = data.REQUEST_TYPE,
                             commAppHis = (from _AppHis in _CommunicationDBContext.CM_COMMUNICATION_APPHISTORY.Where(x => x.COMMUNICATIONID == data.COMMUNICATIONID)
                                           join _AppEmp in _CommunicationDBContext.ADEMPLOYEE on _AppHis.ADEMPCODE equals _AppEmp.ADEMPCODE
                                           select new CommunicationAppHisViewModel
                                           {
                                               COMM_HISTORYID = _AppHis.COMM_HISTORYID,
                                               COMMUNICATIONID = _AppHis.COMMUNICATIONID,
                                               ADEMPCODE = _AppHis.ADEMPCODE,
                                               APPROVAL_STATUS = _AppHis.APPROVAL_STATUS,
                                               APPROVAL_REMARK = _AppHis.APPROVAL_REMARK,
                                               ADEMPNAME = _AppEmp.FIRSTNAME + " " + _AppEmp.LASTNAME + " - [" + _AppEmp.ADEMPCODE + "]",
                                               APP_EMAIL = _AppEmp.EMAILID,
                                               APPROVALDATE = _AppHis.APPROVAL_DATE,
                                               APPTYPE = _AppHis.APPTYPE
                                           }).OrderBy(o => o.COMM_HISTORYID).ToList(),
                         }).ToList();
            return iList;
        }

        public CommunicationViewModel GetCommunicationDetails(long id)
        {
            //var obj = (from data in _CommunicationDBContext.CM_COMMUNICATION_HDR.Where(x => x.COMMUNICATIONID == id)
            //           join _cat in _CommunicationDBContext.CM_COMMCATEGORY_MST on data.COMM_CATID equals _cat.COMMCATEGORYID
            //           join _type in _CommunicationDBContext.CM_COMMTYPE_MST on data.COMM_MAILTYPEID equals _type.COMMTYPEID
            //           join _AddBy in _CommunicationDBContext.ADEMPLOYEE on data.CREATED_BY equals _AddBy.ADEMPCODE
            //           join _VWA in _CommunicationDBContext.VW_ASSOCIATELVLDETAILS.Where(m => m.SYKI == _Syki.SYKIID) on data.CREATED_BY equals _VWA.ADEMPCODE into _vwdt
            //           from _VWAssociate in _vwdt.DefaultIfEmpty()
            //           select new CommunicationViewModel
            //           {
            //               COMMUNICATIONID = data.COMMUNICATIONID,
            //               COMM_CATID = _cat.COMMCATEGORYID,
            //               CATEGORY = _cat.CATEGORY_NAME,
            //               COMM_MAILTYPEID = _type.COMMTYPEID,
            //               MAILTYPE = _type.COMM_TYPE,
            //               MAILID = _type.COMM_EMAILID,
            //               CONTENT = data.CONTENT,
            //               SUBJECT = data.SUBJECT,
            //               _SDATE = data.START_DATE,
            //               _EDATE = data.END_DATE,
            //               EMAILSTATUS = data.EMAILSTATUS == 1 ? true : false,
            //               EMAIL_CONTENT = data.EMAIL_CONTENT,
            //               CREATED_BY = data.CREATED_BY,
            //               CREATED_BY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
            //               CREATED_DATE = data.CREATED_DATE,
            //               PROCESS_STATUS = data.PROCESS_STATUS,
            //               STATUS = data.STATUS,
            //               FnDesignationIds = data.FUNCTIONAL_DESIGNATION,
            //               DesignationIds = data.DESIGNATION,
            //               SiteIds = data.SITE,
            //               OPIds = data.OPERATION,
            //               DIVIds = data.DIVISION,
            //               DEPIds = data.DEPARTMENT,
            //               Recipient = data.RECIPIENT,
            //               REQUEST_TYPE = data.REQUEST_TYPE,
            //               commDetail = (from _CommDetail in _CommunicationDBContext.CM_COMMUNICATION_DTL.Where(d => d.COMMUNICATIONID == data.COMMUNICATIONID && d.STATUS == 1)
            //                             select new CommunicationDtlViewModel
            //                             {
            //                                 COMM_DTLID = _CommDetail.COMM_DTLID,
            //                                 COMMUNICATIONID = _CommDetail.COMMUNICATIONID,
            //                                 DOC_TYPE = _CommDetail.DOC_TYPE,
            //                                 ADDITIONAL_INFO = _CommDetail.ADDITIONAL_INFO,
            //                                 FILENAME = _CommDetail.FILENAME,
            //                             }).ToList(),
            //               Emp_Detail = new Employee_Details
            //               {
            //                   _ECode = _AddBy.ADEMPCODE,
            //                   _EName = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
            //                   _EmailId = _AddBy.EMAILID,
            //                   _DOB = DateTime.Now,
            //                   _SecDescrip = _VWAssociate.SECTION,
            //                   _DepDesc = _VWAssociate.DEPARTMENT,
            //                   _DivDesc = _VWAssociate.DIVISION,
            //                   _OpDesc = _VWAssociate.OPERATION,
            //                   _SiteId = _VWAssociate.SYSITEID
            //               },
            //               commAuthSeq = (from _AppSeq in _CommunicationDBContext.CM_COMMAPPAUTH_SEQ.Where(x => x.COMMUNICATIONID == data.COMMUNICATIONID)
            //                              join _AppAuthSeq in _CommunicationDBContext.ADEMPLOYEE on _AppSeq.ADEMPCODE equals _AppAuthSeq.ADEMPCODE into _AppSeqJoin
            //                              from _AppSeqEmp in _AppSeqJoin.DefaultIfEmpty()
            //                              join _VWD in _CommunicationDBContext.VW_ASSOCIATELVLDETAILS.Where(m => m.SYKI == _Syki.SYKIID) on _AppSeq.ADEMPCODE equals _VWD.ADEMPCODE into _VWTMP
            //                              from _Vw in _VWTMP.DefaultIfEmpty()
            //                              join _Desgt in _CommunicationDBContext.ADDESIGNATION on _Vw.ADDESIGNATIONID equals _Desgt.ADDESIGNATIONID into _desgtmp
            //                              from _Desg in _desgtmp.DefaultIfEmpty()
            //                              select new CommunicationAppSeqViewModel
            //                              {
            //                                  COMMAPPAUTH_ID = _AppSeq.COMMAPPAUTH_ID,
            //                                  COMMUNICATIONID = _AppSeq.COMMUNICATIONID,
            //                                  ADEMPCODE = _AppSeq.ADEMPCODE,
            //                                  STATUS = _AppSeq.STATUS,
            //                                  APP_SEQ = _AppSeq.APP_SEQ,
            //                                  FNAME = _AppSeqEmp == null ? " " : _AppSeqEmp.FIRSTNAME,
            //                                  LNAME = _AppSeqEmp == null ? " " : _AppSeqEmp.LASTNAME,
            //                                  //ADDESIGNATION = string.IsNullOrEmpty(_Desg.DESCRIP) ? "(Inactive Employee)" : _Desg.DESCRIP,
            //                                  ADDEDDATE = _AppSeq.ADDEDDATE,
            //                                  UPDATEBY = _AppSeq.UPDATEBY,
            //                                  UPDATEDATE = _AppSeq.UPDATEDATE,
            //                                  FNDESID = _Vw.ADFUNCTIONALDESIGNATIONID == null ? 0 : _Vw.ADFUNCTIONALDESIGNATIONID,
            //                                  ADDESIGNATION = ((_Vw.ADFUNCTIONALDESIGNATIONID == null || _Vw.ADFUNCTIONALDESIGNATIONID == 0) ? _Desg.DESCRIP : _Vw.FUNCTIONALDESIGNATION),
            //                                  APPTYPE = _AppSeq.APPTYPE,
            //                                  ISADDITIONAL_AUTH = _AppSeq.ISADDITIONAL_AUTH,
            //                              }).OrderBy(b => b.APP_SEQ).ToList(),
            //               commAppHis = (from _AppHis in _CommunicationDBContext.CM_COMMUNICATION_APPHISTORY.Where(x => x.COMMUNICATIONID == data.COMMUNICATIONID)
            //                             join _AppEmp in _CommunicationDBContext.ADEMPLOYEE on _AppHis.ADEMPCODE equals _AppEmp.ADEMPCODE
            //                             select new CommunicationAppHisViewModel
            //                             {
            //                                 COMM_HISTORYID = _AppHis.COMM_HISTORYID,
            //                                 COMMUNICATIONID = _AppHis.COMMUNICATIONID,
            //                                 ADEMPCODE = _AppHis.ADEMPCODE,
            //                                 APPROVAL_STATUS = _AppHis.APPROVAL_STATUS,
            //                                 APPROVAL_REMARK = _AppHis.APPROVAL_REMARK,
            //                                 ADEMPNAME = _AppEmp.FIRSTNAME + " " + _AppEmp.LASTNAME + " - [" + _AppEmp.ADEMPCODE + "]",
            //                                 APP_EMAIL = _AppEmp.EMAILID,
            //                                 ADDEDDATE = _AppHis.ADDEDDATE,
            //                                 UPDATEBY = _AppHis.UPDATEBY,
            //                                 UPDATEDATE = _AppHis.UPDATEDATE,
            //                                 APPROVALDATE = _AppHis.APPROVAL_DATE,
            //                                 APPTYPE = _AppHis.APPTYPE
            //                             }).OrderBy(o => o.COMM_HISTORYID).ToList(),
            //           }).FirstOrDefault();


            var obj = (from data in _CommunicationDBContext.CM_COMMUNICATION_HDR.Where(x => x.COMMUNICATIONID == id)
                       join _cat in _CommunicationDBContext.CM_COMMCATEGORY_MST on data.COMM_CATID equals _cat.COMMCATEGORYID
                       join _type in _CommunicationDBContext.CM_COMMTYPE_MST on data.COMM_MAILTYPEID equals _type.COMMTYPEID
                       join _AddBy in _CommunicationDBContext.ADEMPLOYEE on data.CREATED_BY equals _AddBy.ADEMPCODE
                       join _VWA in _CommunicationDBContext.VW_ASSOCIATELVLDETAILS.Where(m => m.SYKI == _Syki.SYKIID) on data.CREATED_BY equals _VWA.ADEMPCODE into _vwdt
                       from _VWAssociate in _vwdt.DefaultIfEmpty()
                       select new CommunicationViewModel
                       {
                           COMMUNICATIONID = data.COMMUNICATIONID,
                           COMM_CATID = _cat.COMMCATEGORYID,
                           CATEGORY = _cat.CATEGORY_NAME,
                           COMM_MAILTYPEID = _type.COMMTYPEID,
                           MAILTYPE = _type.COMM_TYPE,
                           MAILID = _type.COMM_EMAILID,
                           CONTENT = data.CONTENT,
                           SUBJECT = data.SUBJECT,
                           _SDATE = data.START_DATE,
                           _EDATE = data.END_DATE,
                           //EMAILSTATUS = data.EMAILSTATUS == 1 ? true : false,
                           EMAILSTATUS = Convert.ToBoolean(data.EMAILSTATUS),
                           EMAIL_CONTENT = data.EMAIL_CONTENT,
                           CREATED_BY = data.CREATED_BY,
                           CREATED_BY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                           CREATED_DATE = data.CREATED_DATE,
                           PROCESS_STATUS = data.PROCESS_STATUS,
                           STATUS = data.STATUS,
                           FnDesignationIds = data.FUNCTIONAL_DESIGNATION,
                           DesignationIds = data.DESIGNATION,
                           SiteIds = data.SITE,
                           OPIds = data.OPERATION,
                           DIVIds = data.DIVISION,
                           DEPIds = data.DEPARTMENT,
                           Recipient = data.RECIPIENT,
                           REQUEST_TYPE = data.REQUEST_TYPE,
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
                       }).FirstOrDefault(); // Materialize main query first

            // **Retrieve subqueries separately after fetching the main object**
            if (obj != null)
            {
                obj.commDetail = _CommunicationDBContext.CM_COMMUNICATION_DTL
                    .Where(d => d.COMMUNICATIONID == obj.COMMUNICATIONID && d.STATUS == 1)
                    .Select(_CommDetail => new CommunicationDtlViewModel
                    {
                        COMM_DTLID = _CommDetail.COMM_DTLID,
                        COMMUNICATIONID = _CommDetail.COMMUNICATIONID,
                        DOC_TYPE = _CommDetail.DOC_TYPE,
                        ADDITIONAL_INFO = _CommDetail.ADDITIONAL_INFO,
                        FILENAME = _CommDetail.FILENAME,
                    })
                    .ToList(); // Materialize separately
                obj.commAuthSeq = (from _AppSeq in _CommunicationDBContext.CM_COMMAPPAUTH_SEQ.Where(x => x.COMMUNICATIONID == obj.COMMUNICATIONID)
                                   join _AppAuthSeq in _CommunicationDBContext.ADEMPLOYEE on _AppSeq.ADEMPCODE equals _AppAuthSeq.ADEMPCODE into _AppSeqJoin
                                   from _AppSeqEmp in _AppSeqJoin.DefaultIfEmpty()
                                   join _VWD in _CommunicationDBContext.VW_ASSOCIATELVLDETAILS.Where(m => m.SYKI == _Syki.SYKIID) on _AppSeq.ADEMPCODE equals _VWD.ADEMPCODE into _VWTMP
                                   from _Vw in _VWTMP.DefaultIfEmpty()
                                   join _Desgt in _CommunicationDBContext.ADDESIGNATION on _Vw.ADDESIGNATIONID equals _Desgt.ADDESIGNATIONID into _desgtmp
                                   from _Desg in _desgtmp.DefaultIfEmpty()
                                   select new CommunicationAppSeqViewModel
                                   {
                                       COMMAPPAUTH_ID = _AppSeq.COMMAPPAUTH_ID,
                                       COMMUNICATIONID = _AppSeq.COMMUNICATIONID,
                                       ADEMPCODE = _AppSeq.ADEMPCODE,
                                       STATUS = _AppSeq.STATUS,
                                       APP_SEQ = _AppSeq.APP_SEQ,
                                       //FNAME = _AppSeqEmp == null ? " " : _AppSeqEmp.FIRSTNAME,
                                       //LNAME = _AppSeqEmp == null ? " " : _AppSeqEmp.LASTNAME,
                                       FNAME = _AppSeqEmp.FIRSTNAME,
                                       LNAME = _AppSeqEmp.LASTNAME,
                                       //ADDESIGNATION = string.IsNullOrEmpty(_Desg.DESCRIP) ? "(Inactive Employee)" : _Desg.DESCRIP,
                                       ADDEDDATE = _AppSeq.ADDEDDATE,
                                       UPDATEBY = _AppSeq.UPDATEBY,
                                       UPDATEDATE = _AppSeq.UPDATEDATE,
                                       FNDESID = _Vw.ADFUNCTIONALDESIGNATIONID == null ? 0 : _Vw.ADFUNCTIONALDESIGNATIONID,
                                       ADDESIGNATION = ((_Vw.ADFUNCTIONALDESIGNATIONID == null || _Vw.ADFUNCTIONALDESIGNATIONID == 0) ? _Desg.DESCRIP : _Vw.FUNCTIONALDESIGNATION),
                                       APPTYPE = _AppSeq.APPTYPE,
                                       ISADDITIONAL_AUTH = _AppSeq.ISADDITIONAL_AUTH,
                                   }).OrderBy(b => b.APP_SEQ).ToList();

                obj.commAppHis = (from _AppHis in _CommunicationDBContext.CM_COMMUNICATION_APPHISTORY.Where(x => x.COMMUNICATIONID == obj.COMMUNICATIONID)
                                  join _AppEmp in _CommunicationDBContext.ADEMPLOYEE on _AppHis.ADEMPCODE equals _AppEmp.ADEMPCODE
                                  select new CommunicationAppHisViewModel
                                  {
                                      COMM_HISTORYID = _AppHis.COMM_HISTORYID,
                                      COMMUNICATIONID = _AppHis.COMMUNICATIONID,
                                      ADEMPCODE = _AppHis.ADEMPCODE,
                                      APPROVAL_STATUS = _AppHis.APPROVAL_STATUS,
                                      APPROVAL_REMARK = _AppHis.APPROVAL_REMARK,
                                      ADEMPNAME = _AppEmp.FIRSTNAME + " " + _AppEmp.LASTNAME + " - [" + _AppEmp.ADEMPCODE + "]",
                                      APP_EMAIL = _AppEmp.EMAILID,
                                      ADDEDDATE = _AppHis.ADDEDDATE,
                                      UPDATEBY = _AppHis.UPDATEBY,
                                      UPDATEDATE = _AppHis.UPDATEDATE,
                                      APPROVALDATE = _AppHis.APPROVAL_DATE,
                                      APPTYPE = _AppHis.APPTYPE
                                  }).OrderBy(o => o.COMM_HISTORYID).ToList();
            }


            if (obj != null)
            {
                obj.START_DATE = obj._SDATE == null ? "" : Convert.ToDateTime(obj._SDATE).ToString("dd-MMM-yyyy");
                obj.END_DATE = obj._EDATE == null ? "" : Convert.ToDateTime(obj._EDATE).ToString("dd-MMM-yyyy");
                obj.CONTENT = obj.CONTENT == null ? obj.CONTENT : CommonRepository.HtmlToText(obj.CONTENT);
                //obj.EMAIL_CONTENT = obj.EMAIL_CONTENT == null ? obj.EMAIL_CONTENT : CommonRepository.HtmlToText(obj.EMAIL_CONTENT);
                obj.DesignationOption = 1;

                if (!string.IsNullOrEmpty(obj.FnDesignationIds))
                {
                    obj.DesignationOption = 2;
                    foreach (var fnDesId in obj.FnDesignationIds.Split(',').ToArray())
                    {
                        if (!string.IsNullOrEmpty(fnDesId))
                        {
                            long valId = Convert.ToInt64(fnDesId);
                            obj.FnDesignationDescrip += _CommunicationDBContext.ADFUNCTIONALDESIGNATION.Where(x => x.ADFUNCTIONALDESIGNATIONID == valId).Select(x => x.DESCRIP).SingleOrDefault() + ", ";
                        }
                    }
                }
                else
                {
                    obj.FnDesignationDescrip = "";
                }
                if (!string.IsNullOrEmpty(obj.DesignationIds))
                {
                    foreach (var DesId in obj.DesignationIds.Split(',').ToArray())
                    {
                        if (!string.IsNullOrEmpty(DesId))
                        {
                            long valId = Convert.ToInt64(DesId);
                            obj.DesignationDescrip += _CommunicationDBContext.ADDESIGNATION.Where(x => x.ADDESIGNATIONID == valId).Select(x => x.DESCRIP).SingleOrDefault() + ", ";
                        }
                    }
                }
                else
                {
                    obj.DesignationDescrip = "";
                }
                if (!string.IsNullOrEmpty(obj.SiteIds))
                {
                    foreach (var SiteId in obj.SiteIds.Split(',').ToArray())
                    {
                        if (!string.IsNullOrEmpty(SiteId))
                        {
                            long valId = Convert.ToInt64(SiteId);
                            obj.SiteDescrip += _CommunicationDBContext.SYSITE.Where(x => x.SYSITEID == valId).Select(x => x.DESCRIP).SingleOrDefault() + ", ";
                        }
                    }
                }
                else
                {
                    obj.SiteDescrip = "";
                }
                // -- Operation
                if (!string.IsNullOrEmpty(obj.OPIds))
                {
                    foreach (var opId in obj.OPIds.Split(',').ToArray())
                    {
                        if (!string.IsNullOrEmpty(opId))
                        {
                            long valId = Convert.ToInt64(opId);
                            obj.OPDescrip += _CommunicationDBContext.ADORGLEVEL.Where(x => x.ADORGLEVELID == valId).Select(x => x.LEVELDESCRIP).SingleOrDefault() + ", ";
                        }
                    }
                }
                else
                {
                    obj.OPDescrip = "";
                }

                // -- Division
                if (!string.IsNullOrEmpty(obj.DIVIds))
                {
                    foreach (var divId in obj.DIVIds.Split(',').ToArray())
                    {
                        if (!string.IsNullOrEmpty(divId))
                        {
                            long valId = Convert.ToInt64(divId);
                            obj.DIVDescrip += _CommunicationDBContext.ADORGLEVEL.Where(x => x.ADORGLEVELID == valId).Select(x => x.LEVELDESCRIP).SingleOrDefault() + ", ";
                        }
                    }
                }
                else
                {
                    obj.DIVDescrip = "";
                }

                // -- Department
                if (!string.IsNullOrEmpty(obj.DEPIds))
                {
                    foreach (var depId in obj.DEPIds.Split(',').ToArray())
                    {
                        if (!string.IsNullOrEmpty(depId))
                        {
                            long valId = Convert.ToInt64(depId);
                            obj.DEPDescrip += _CommunicationDBContext.ADORGLEVEL.Where(x => x.ADORGLEVELID == valId).Select(x => x.LEVELDESCRIP).SingleOrDefault() + ", ";
                        }
                    }
                }
                else
                {
                    obj.DEPDescrip = "";
                }

                if (obj.commAuthSeq.Count > 0)
                {
                    long lastSendBackAppHisId = 0;
                    //CM_COMMUNICATION_APPHISTORY lastSendBackAppHis = _CommunicationDBContext.CM_COMMUNICATION_APPHISTORY.Where(e => e.COMMUNICATIONID == obj.COMMUNICATIONID && (e.APPROVAL_STATUS == 2 || e.APPROVAL_STATUS == 4)).OrderByDescending(o => o.COMM_HISTORYID).FirstOrDefault();
                    CM_COMMUNICATION_APPHISTORY lastSendBackAppHis = _CommunicationDBContext.CM_COMMUNICATION_APPHISTORY.Where(e => e.COMMUNICATIONID == obj.COMMUNICATIONID && e.APPROVAL_STATUS == 2).OrderByDescending(o => o.COMM_HISTORYID).FirstOrDefault();
                    if (lastSendBackAppHis != null)
                    {
                        lastSendBackAppHisId = lastSendBackAppHis.COMM_HISTORYID;
                    }
                    foreach (CommunicationAppSeqViewModel _obj in obj.commAuthSeq)
                    {
                        _obj.ADEMPNAME = _obj.FNAME + " " + _obj.LNAME ;


                        //           var IsBeforeSendBackRecord1 = _CommunicationDBContext.CM_COMMUNICATION_APPHISTORY
                        //.Select(w => w.COMMUNICATIONID == _obj.COMMUNICATIONID &&
                        //    w.COMM_HISTORYID <= lastSendBackAppHisId &&
                        //    ((_obj.ADEMPCODE == 1 ? (w.APPTYPE == 2 ? 1 : 0) : (w.ADEMPCODE == _obj.ADEMPCODE ? 1 : 0)) == 1)).ToList();

                        var IsBeforeSendBackRecord1 = _CommunicationDBContext.CM_COMMUNICATION_APPHISTORY
    .Select(w => new
    {
        CommunicationIdMatch = (w.COMMUNICATIONID == _obj.COMMUNICATIONID ? 1 : 0),
        HistoryIdCheck = (w.COMM_HISTORYID <= lastSendBackAppHisId ? 1 : 0),
        AppTypeCheck = (_obj.ADEMPCODE == 1 ? (w.APPTYPE == 2 ? 1 : 0) : (w.ADEMPCODE == _obj.ADEMPCODE ? 1 : 0))
    }).ToList();

                        bool IsBeforeSendBackRecord = IsBeforeSendBackRecord1.Count() > 0 ? true : false;

                        //bool IsBeforeSendBackRecord = _CommunicationDBContext.CM_COMMUNICATION_APPHISTORY
                        //    .Any(w => w.COMMUNICATIONID == _obj.COMMUNICATIONID &&
                        //    w.COMM_HISTORYID <= lastSendBackAppHisId
                        //    && (_obj.ADEMPCODE == 1 ? w.APPTYPE == 2 : w.ADEMPCODE == _obj.ADEMPCODE));

                        if ((IsBeforeSendBackRecord ? (!obj.commAppHis.Any(r => r.COMMUNICATIONID == _obj.COMMUNICATIONID && (_obj.ADEMPCODE == 1 ? r.APPTYPE == 2 : r.ADEMPCODE == _obj.ADEMPCODE) && r.COMM_HISTORYID > lastSendBackAppHisId)) : (!obj.commAppHis.Any(x => x.COMMUNICATIONID == _obj.COMMUNICATIONID && (_obj.ADEMPCODE == 1 ? x.APPTYPE == 2 : x.ADEMPCODE == _obj.ADEMPCODE)))))
                        {
                            //bool isAddMember = (obj.commAppHis.Any(x => x.COMMUNICATIONID == _obj.COMMUNICATIONID && x.APPTYPE == 2) && _obj.ADEMPCODE == 1) ? false : true;
                            //if (isAddMember)
                            //{
                            obj.commAppHis.Add(new CommunicationAppHisViewModel
                            {
                                COMM_HISTORYID = 0,
                                COMMUNICATIONID = _obj.COMMUNICATIONID,
                                ADEMPCODE = _obj.ADEMPCODE,
                                APPROVAL_STATUS = 0,
                                APPROVAL_REMARK = "",
                                ADEMPNAME = _obj.ADEMPCODE == 1 ? "Corporate Communication Team" : _obj.ADEMPNAME + " - [" + _obj.ADEMPCODE + "]",
                            });
                            //}
                        }
                    }
                }
            }
            else
            {
                obj = new CommunicationViewModel();
            }

            return obj;
        }

        public Tuple<short, long> SaveCommunicationRequest(CommunicationViewModel model)
        {
            short retVal = 0; long retHeaderId = 0;
            Tuple<short, long> _retVal_tuple;

            using (var transaction = _CommunicationDBContext.Database.BeginTransaction())
            //using (DbContextTransaction transaction = (DbContextTransaction)_CommunicationDBContext.Database.BeginTransaction())
            {
                try
                {
                    int FlagAdd = 0;
                    CM_COMMUNICATION_HDR CCH = new CM_COMMUNICATION_HDR();
                    if (model.COMMUNICATIONID > 0)
                    {
                        CCH = _CommunicationDBContext.CM_COMMUNICATION_HDR.Where(x => x.COMMUNICATIONID == model.COMMUNICATIONID).SingleOrDefault();
                    }
                    else
                    {
                        CCH = new CM_COMMUNICATION_HDR();
                        if (_CommunicationDBContext.CM_COMMUNICATION_HDR.Count() == 0)
                        {
                            CCH.COMMUNICATIONID = 1;
                        }
                        else
                        {
                            CCH.COMMUNICATIONID = _CommunicationDBContext.CM_COMMUNICATION_HDR.Max(x => x.COMMUNICATIONID) + 1;
                        }
                        FlagAdd = 1;

                        ////// -- For unique circular id
                        //if (model.COMM_CATID == 2)
                        //{
                        DateTime today = DateTime.Today;
                        //int reqCount = _CommunicationDBContext.CM_COMMUNICATION_HDR.Where(c => c.COMM_CATID == model.COMM_CATID && c.CREATED_DATE.Month == today.Month && c.CREATED_DATE.Year == today.Year).Count() + 1;
                        int reqCount = _CommunicationDBContext.CM_COMMUNICATION_HDR.Where(c => c.CREATED_DATE.Month == today.Month && c.CREATED_DATE.Year == today.Year).Count() + 1;
                        CCH.REQUESTNO = model.REQUESTNO + reqCount.ToString();
                        //}
                    }
                    retHeaderId = CCH.COMMUNICATIONID;
                    CCH.COMM_CATID = model.COMM_CATID;
                    CCH.COMM_MAILTYPEID = model.COMM_MAILTYPEID;
                    CCH.REQUEST_TYPE = model.REQUEST_TYPE;
                    if (!string.IsNullOrEmpty(model.START_DATE))
                    {
                        CCH.START_DATE = DateTime.ParseExact(model.START_DATE, "dd-MMM-yyyy", null);
                    }
                    else
                    {
                        CCH.START_DATE = DateTime.Now;
                    }
                    if (!string.IsNullOrEmpty(model.END_DATE))
                    {
                        CCH.END_DATE = DateTime.ParseExact(model.END_DATE, "dd-MMM-yyyy", null);
                    }
                    else
                    {
                        CCH.END_DATE = DateTime.Now;
                    }
                    CCH.SUBJECT = model.SUBJECT;
                    CCH.SUBJECT_ORIG = CCH.SUBJECT;
                    //CCH.BRIEF = AVM.BRIEF;
                    CCH.CONTENT = CommonRepository.TextToHtml(model.CONTENT);
                    CCH.CONTENT_ORIG = CCH.CONTENT;
                    CCH.PROCESS_STATUS = model.PROCESS_STATUS;
                    CCH.STATUS = model.STATUS;
                    if (FlagAdd == 1)
                    {
                        CCH.CREATED_DATE = DateTime.Now;
                        CCH.CREATED_BY = model.CREATED_BY;
                    }
                    else
                    {
                        CCH.MODIFIED_DATE = DateTime.Now;
                        CCH.MODIFIED_BY = model.MODIFIED_BY;
                    }
                    CCH.EMAILSTATUS = Convert.ToInt16(model.EMAILSTATUS);
                    if (model.EMAILSTATUS)
                    {
                        CCH.EMAIL_CONTENT = model.EMAIL_CONTENT; // CommonRepository.TextToHtml(model.EMAIL_CONTENT);
                    }
                    else
                    {
                        CCH.EMAIL_CONTENT = "";
                    }
                    CCH.EMAIL_CONTENT_ORIG = CCH.EMAIL_CONTENT;

                    string fnIds = "";
                    if (model.DesignationOption == 2)
                    {
                        if (model.FUNCTIONAL_DESIGNATION != null)
                        {
                            foreach (var val in model.FUNCTIONAL_DESIGNATION)
                            {
                                fnIds += val.ToString() + ",";
                            };
                        }
                    }
                    string DesIds = "";
                    if (model.DesignationOption == 1)
                    {
                        if (model.DESIGNATION != null)
                        {
                            foreach (var val in model.DESIGNATION)
                            {
                                DesIds += val.ToString() + ",";
                            };
                        }
                    }
                    string SiteIds = "";
                    if (model.SITE != null)
                    {
                        foreach (var val in model.SITE)
                        {
                            SiteIds += val.ToString() + ",";
                        };
                    }
                    CCH.FUNCTIONAL_DESIGNATION = fnIds;
                    CCH.DESIGNATION = DesIds;
                    CCH.SITE = SiteIds;
                    string OpIds = "";
                    if (model.OPERATION != null)
                    {
                        foreach (var val in model.OPERATION)
                        {
                            if (val != 0)
                            {
                                OpIds += val.ToString() + ",";
                            }
                        };
                    }
                    string DivIds = "";
                    if (model.DIVISION != null)
                    {
                        foreach (var val in model.DIVISION)
                        {
                            if (val != 0)
                            {
                                DivIds += val.ToString() + ",";
                            }
                        };
                    }
                    string DepIds = "";
                    if (model.DEPARTMENT != null)
                    {
                        foreach (var val in model.DEPARTMENT)
                        {
                            if (val != 0)
                            {
                                DepIds += val.ToString() + ",";
                            }
                        };
                    }
                    CCH.OPERATION = OpIds;
                    CCH.DIVISION = DivIds;
                    CCH.DEPARTMENT = DepIds;

                    CCH.RECIPIENT = model.Recipient;
                    _CommunicationDBContext.Entry(CCH).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _CommunicationDBContext.SaveChanges();
                    transaction.Commit();
                    retVal = 1;
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

        public Tuple<short, long> FinalSubmitRequest(CommunicationViewModel model)
        {
            short retVal = 0; long retHeaderId = 0;
            Tuple<short, long> _retVal_tuple;
            using (var transaction = _CommunicationDBContext.Database.BeginTransaction())
            //using (DbContextTransaction transaction = (DbContextTransaction)_CommunicationDBContext.Database.BeginTransaction())
            {
                try
                {
                    //model.COMMUNICATIONID = _CommunicationDBContext.CM_COMMUNICATION_HDR.Max(c => c.COMMUNICATIONID);

                    CM_COMMUNICATION_HDR DPH = new CM_COMMUNICATION_HDR();
                    if (model.COMMUNICATIONID > 0)
                    {
                        DPH = _CommunicationDBContext.CM_COMMUNICATION_HDR.Where(x => x.COMMUNICATIONID == model.COMMUNICATIONID).SingleOrDefault();
                    }
                    DPH.PROCESS_STATUS = model.PROCESS_STATUS;
                    DPH.STATUS = model.STATUS;
                    DPH.MODIFIED_BY = model.MODIFIED_BY;
                    DPH.MODIFIED_DATE = DateTime.Now;
                    _CommunicationDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _CommunicationDBContext.SaveChanges();

                    ///// --- Save Attachment --- ///
                    //if (model.commDetail != null)
                    //{
                    //    if (model.commDetail.Count > 0)
                    //    {
                    //        SaveCommAttachment(model.CREATED_BY, DPH.COMMUNICATIONID, model.commDetail);
                    //    }
                    //}

                    /// --- Save Comm Approval Auth Seq ---///
                    if (model.commAuthSeq != null)
                    {
                        if (model.commAuthSeq.Count > 0)
                        {
                            SaveAppAuthSeq(model.CREATED_BY, DPH.COMMUNICATIONID, model.commAuthSeq, model.additionalCommAuthSeq);

                            /// --- Save Comm Approval Authority --- ///
                            CM_COMMAPPAUTH_SEQ seqModel = _CommunicationDBContext.CM_COMMAPPAUTH_SEQ.Where(w => w.COMMUNICATIONID == DPH.COMMUNICATIONID).OrderBy(o => o.APP_SEQ).FirstOrDefault();
                            if (seqModel != null)
                            {
                                SaveCommAppHis(model.CREATED_BY, DPH.COMMUNICATIONID, seqModel);
                            }
                        }
                    }

                    ///// --- Save IOM Approval Header ---///
                    //if (model.iomAppHeaderList != null)
                    //{
                    //    if (model.iomAppHeaderList.Count > 0)
                    //    {
                    //        SaveAppHeader(model.ADDEDBY, DPH.IOMHEADERID, model.iomAppHeaderList);
                    //    }
                    //}

                    transaction.Commit();
                    retVal = 1;
                    retHeaderId = DPH.COMMUNICATIONID;
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

        public short SaveCommAttachment(long AddedBy, long CommheaderId, List<CommunicationDtlViewModel> modelList)
        {
            short retVal = 0;
            foreach (CommunicationDtlViewModel PDVM in modelList)
            {
                CM_COMMUNICATION_DTL DPD = new CM_COMMUNICATION_DTL();
                int FlagAdd = 0;
                if (CommheaderId > 0)
                {
                    DPD = new CM_COMMUNICATION_DTL();
                    if (_CommunicationDBContext.CM_COMMUNICATION_DTL.Count() == 0)
                    {
                        DPD.COMM_DTLID = 1;
                    }
                    else
                    {
                        DPD.COMM_DTLID = _CommunicationDBContext.CM_COMMUNICATION_DTL.Max(x => x.COMM_DTLID) + 1;
                    }
                    FlagAdd = 1;

                    DPD.COMMUNICATIONID = CommheaderId;
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
                    _CommunicationDBContext.Entry(DPD).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _CommunicationDBContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }

        public void SaveAppAuthSeq(long AddedBy, long COMMUNICATIONID, List<CommunicationAppSeqViewModel> PSVMList, List<CommunicationAppSeqViewModel> AdditionalAuth)
        {
            //var existingRecord = _CommunicationDBContext.CM_COMMAPPAUTH_SEQ
            //.FirstOrDefault(x => x.COMMUNICATIONID == COMMUNICATIONID && x.ADEMPCODE == AddedBy);

            //if (existingRecord != null)
            //{
                //// --- Delete recode ---////
                List<CM_COMMAPPAUTH_SEQ> SeqList = _CommunicationDBContext.CM_COMMAPPAUTH_SEQ.Where(t => t.COMMUNICATIONID == COMMUNICATIONID).ToList();
                if (SeqList.Count > 0)
                {
                    _CommunicationDBContext.CM_COMMAPPAUTH_SEQ.RemoveRange(SeqList);
                    _CommunicationDBContext.SaveChanges();
                }

                CM_COMMAPPAUTH_SEQ DAAS = new CM_COMMAPPAUTH_SEQ();
                string[] strDesgination = { "Section Head", "Department Head", "Coordinator", "Division Head", "Executive Coordinator", "Corporate Communication", "Operating Head", "Director" };
                foreach (string strD in strDesgination)
                {
                    foreach (CommunicationAppSeqViewModel PSVM in PSVMList.Where(o => o.ADDESIGNATION == strD).ToList())
                    {
                        DAAS = new CM_COMMAPPAUTH_SEQ();
                        if (_CommunicationDBContext.CM_COMMAPPAUTH_SEQ.Count() == 0)
                        {
                            DAAS.COMMAPPAUTH_ID = 1;
                        }
                        else
                        {
                            DAAS.COMMAPPAUTH_ID = _CommunicationDBContext.CM_COMMAPPAUTH_SEQ.Max(x => x.COMMAPPAUTH_ID) + 1;
                        }
                        DAAS.COMMUNICATIONID = COMMUNICATIONID;
                        DAAS.ADEMPCODE = PSVM.ADEMPCODE;
                        DAAS.APP_SEQ = Convert.ToInt16(_CommunicationDBContext.CM_COMMAPPAUTH_SEQ.Where(s => s.COMMUNICATIONID == COMMUNICATIONID).Count() + 1); //PSVM.APP_SEQ;
                        DAAS.APPTYPE = PSVM.APPTYPE;
                        DAAS.ISADDITIONAL_AUTH = PSVM.ISADDITIONAL_AUTH;
                        DAAS.STATUS = 1;
                        DAAS.ADDEDBY = AddedBy;
                        DAAS.ADDEDDATE = DateTime.Now;
                        _CommunicationDBContext.Entry(DAAS).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                        _CommunicationDBContext.SaveChanges();
                    }
                    //// -- Additional Authority -- ///// 
                    if (AdditionalAuth != null)
                    {
                        //List<CommunicationAppSeqViewModel> _additionalAuthList = (PSVM.FNDESID == 0 ? AdditionalAuth.Where(a => a.ADDESIGNATION == PSVM.ADDESIGNATION).ToList() : AdditionalAuth.Where(a => a.FNDESID == PSVM.FNDESID).ToList());
                        List<CommunicationAppSeqViewModel> _additionalAuthList = AdditionalAuth.Where(a => a.ADDESIGNATION == strD).ToList();
                        foreach (CommunicationAppSeqViewModel _additionalAuthExist in _additionalAuthList)
                        {
                            DAAS = new CM_COMMAPPAUTH_SEQ();
                            DAAS.COMMAPPAUTH_ID = _CommunicationDBContext.CM_COMMAPPAUTH_SEQ.Max(x => x.COMMAPPAUTH_ID) + 1;
                            DAAS.COMMUNICATIONID = COMMUNICATIONID;
                            DAAS.ADEMPCODE = _additionalAuthExist.ADEMPCODE;
                            DAAS.APP_SEQ = Convert.ToInt16(_CommunicationDBContext.CM_COMMAPPAUTH_SEQ.Where(s => s.COMMUNICATIONID == COMMUNICATIONID).Count() + 1); //Convert.ToInt16(PSVM.APP_SEQ + 1); 
                            DAAS.APPTYPE = (strD == "Operating Head" || strD == "Director") ? (short)3 : (short)1; //PSVM.APPTYPE;
                            DAAS.ISADDITIONAL_AUTH = 1; // PSVM.ISADDITIONAL_AUTH;
                            DAAS.STATUS = 1;
                            DAAS.ADDEDBY = AddedBy;
                            DAAS.ADDEDDATE = DateTime.Now;
                            _CommunicationDBContext.Entry(DAAS).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                            _CommunicationDBContext.SaveChanges();

                            //AdditionalAuth.Remove(_additionalAuthExist);
                        }
                    }
               // }
            }
        }

        public void SaveCommAppHis(long AddedBy, long COMMUNICATIONID, CM_COMMAPPAUTH_SEQ PSVM)
        {
            CM_COMMUNICATION_APPHISTORY DPAH = new CM_COMMUNICATION_APPHISTORY();
            int FlagAdd = 0;
            DPAH = _CommunicationDBContext.CM_COMMUNICATION_APPHISTORY.Where(d => d.ADEMPCODE == PSVM.ADEMPCODE && d.COMMUNICATIONID == COMMUNICATIONID && d.APPROVAL_STATUS == 0).FirstOrDefault();
            int APPauthseq = _CommunicationDBContext.CM_COMMAPPAUTH_SEQ.Where(m => m.COMMUNICATIONID == COMMUNICATIONID && m.ADEMPCODE == PSVM.ADEMPCODE).Count();
            if (DPAH == null && APPauthseq > 0)
            {
                DPAH = new CM_COMMUNICATION_APPHISTORY();
                if (_CommunicationDBContext.CM_COMMUNICATION_APPHISTORY.Count() == 0)
                {
                    DPAH.COMM_HISTORYID = 1;
                }
                else
                {
                    DPAH.COMM_HISTORYID = _CommunicationDBContext.CM_COMMUNICATION_APPHISTORY.Max(x => x.COMM_HISTORYID) + 1;
                }
                FlagAdd = 1;

                DPAH.COMMUNICATIONID = COMMUNICATIONID;
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
                _CommunicationDBContext.Entry(DPAH).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                _CommunicationDBContext.SaveChanges();
            }
        }

        public List<CommunicationDtlViewModel> GetCommAttachmentDetail(long _COMMUNICATIONID)
        {
            return (from _detail in _CommunicationDBContext.CM_COMMUNICATION_DTL.Where(d => d.COMMUNICATIONID == _COMMUNICATIONID)
                    where _detail.STATUS == 1
                    select new CommunicationDtlViewModel
                    {
                        COMM_DTLID = _detail.COMM_DTLID,
                        COMMUNICATIONID = _detail.COMMUNICATIONID,
                        DOC_TYPE = _detail.DOC_TYPE,
                        ADDITIONAL_INFO = _detail.ADDITIONAL_INFO,
                        FILENAME = _detail.FILENAME,
                    }).ToList();
        }

        public short DeleteAttachment(string fileName, string docType, long _COMMUNICATIONID)
        {
            short retVal = 0;
            if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(docType) && _COMMUNICATIONID > 0)
            {
                CM_COMMUNICATION_DTL DT = _CommunicationDBContext.CM_COMMUNICATION_DTL.Where(x => x.FILENAME == fileName && x.DOC_TYPE == docType && x.COMMUNICATIONID == _COMMUNICATIONID).FirstOrDefault();
                if (DT != null)
                {
                    _CommunicationDBContext.CM_COMMUNICATION_DTL.Remove(DT);
                    _CommunicationDBContext.SaveChanges();
                    retVal = 1;
                }
            }
            return retVal;
        }

        public List<CommunicationViewModel> CommunicationApprovalList(long userId)
        {
            var iList = (from data in _CommunicationDBContext.CM_COMMUNICATION_HDR.Where(c => c.STATUS == 1 && c.PROCESS_STATUS == 1)
                         join _AppHis in _CommunicationDBContext.CM_COMMUNICATION_APPHISTORY on data.COMMUNICATIONID equals _AppHis.COMMUNICATIONID
                         join _cat in _CommunicationDBContext.CM_COMMCATEGORY_MST on data.COMM_CATID equals _cat.COMMCATEGORYID
                         join _type in _CommunicationDBContext.CM_COMMTYPE_MST on data.COMM_MAILTYPEID equals _type.COMMTYPEID
                         join _AddBy in _CommunicationDBContext.ADEMPLOYEE on data.CREATED_BY equals _AddBy.ADEMPCODE
                         //join _AppAuth in _CommunicationDBContext.ADEMPLOYEE on _AppHis.ADEMPCODE equals _AppAuth.ADEMPCODE
                         where _AppHis.ADEMPCODE == userId && _AppHis.APPROVAL_STATUS == 0
                         select new CommunicationViewModel
                         {
                             COMMUNICATIONID = data.COMMUNICATIONID,
                             COMM_CATID = _cat.COMMCATEGORYID,
                             CATEGORY = _cat.CATEGORY_NAME,
                             COMM_MAILTYPEID = _type.COMMTYPEID,
                             MAILTYPE = _type.COMM_TYPE,
                             MAILID = _type.COMM_EMAILID,
                             CONTENT = data.CONTENT,
                             SUBJECT = data.SUBJECT,
                             _SDATE = data.START_DATE,
                             _EDATE = data.END_DATE,
                             //EMAILSTATUS = data.EMAILSTATUS == 1 ? true : false,
                             EMAILSTATUS = Convert.ToBoolean(data.EMAILSTATUS),
                             CREATED_BY = data.CREATED_BY,
                             CREATED_BY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             CREATED_DATE = data.CREATED_DATE,
                             PROCESS_STATUS = data.PROCESS_STATUS,
                             STATUS = data.STATUS,
                             REQUEST_TYPE = data.REQUEST_TYPE,
                             //PENDINGAT_NAME = _AppAuth.FIRSTNAME + " " + _AppAuth.LASTNAME,
                         }).ToList();
            return iList;
        }

        public List<CommunicationViewModel> CommunicationApprovalHistory(long userId)
        {
            var iList = (from data in _CommunicationDBContext.CM_COMMUNICATION_HDR
                         join _AppHis in _CommunicationDBContext.CM_COMMUNICATION_APPHISTORY on data.COMMUNICATIONID equals _AppHis.COMMUNICATIONID
                         join _cat in _CommunicationDBContext.CM_COMMCATEGORY_MST on data.COMM_CATID equals _cat.COMMCATEGORYID
                         join _type in _CommunicationDBContext.CM_COMMTYPE_MST on data.COMM_MAILTYPEID equals _type.COMMTYPEID
                         join _AddBy in _CommunicationDBContext.ADEMPLOYEE on data.CREATED_BY equals _AddBy.ADEMPCODE
                         //join _AppAuth in _CommunicationDBContext.ADEMPLOYEE on _AppHis.ADEMPCODE equals _AppAuth.ADEMPCODE
                         where _AppHis.ADEMPCODE == userId && _AppHis.APPROVAL_STATUS != 0
                         select new CommunicationViewModel
                         {
                             COMMUNICATIONID = data.COMMUNICATIONID,
                             COMM_CATID = _cat.COMMCATEGORYID,
                             CATEGORY = _cat.CATEGORY_NAME,
                             COMM_MAILTYPEID = _type.COMMTYPEID,
                             MAILTYPE = _type.COMM_TYPE,
                             MAILID = _type.COMM_EMAILID,
                             CONTENT = data.CONTENT,
                             SUBJECT = data.SUBJECT,
                             _SDATE = data.START_DATE,
                             _EDATE = data.END_DATE,
                             //EMAILSTATUS = data.EMAILSTATUS == 1 ? true : false,
                             EMAILSTATUS = Convert.ToBoolean(data.EMAILSTATUS),
                             CREATED_BY = data.CREATED_BY,
                             CREATED_BY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             CREATED_DATE = data.CREATED_DATE,
                             PROCESS_STATUS = data.PROCESS_STATUS,
                             STATUS = data.STATUS,
                             REQUEST_TYPE = data.REQUEST_TYPE,
                             MAIL_SENT_STATUS = data.MAIL_SENT_STATUS
                             //PENDINGAT_NAME = _AppAuth.FIRSTNAME + " " + _AppAuth.LASTNAME,
                         }).ToList();
            return iList;
        }
        public short CommunicationApproval(CommunicationAppHisViewModel PHVM, List<CommunicationDtlViewModel> commDetailList)
        {
            short retVal = 0;
            //using (DbContextTransaction transaction = (DbContextTransaction)_CommunicationDBContext.Database.BeginTransaction())
            using (var transaction = _CommunicationDBContext.Database.BeginTransaction())
            {
                try
                {
                    CM_COMMUNICATION_APPHISTORY DPAH = new CM_COMMUNICATION_APPHISTORY();
                    if (PHVM.COMMUNICATIONID > 0)
                    {
                        /////////// Update Approval Status //////////
                        DPAH = _CommunicationDBContext.CM_COMMUNICATION_APPHISTORY.Where(x => x.COMMUNICATIONID == PHVM.COMMUNICATIONID && x.ADEMPCODE == (PHVM.APPTYPE == 2 ? 1 : PHVM.ADEMPCODE) && x.APPROVAL_STATUS == 0).FirstOrDefault();
                        if (DPAH != null)
                        {
                            if (PHVM.APPTYPE == 2)
                            {
                                DPAH.ADEMPCODE = PHVM.ADEMPCODE;
                            }
                            DPAH.APPROVAL_STATUS = PHVM.APPROVAL_STATUS;
                            DPAH.APPROVAL_REMARK = PHVM.APPROVAL_REMARK;
                            DPAH.UPDATEBY = PHVM.UPDATEBY;
                            DPAH.APPROVAL_DATE = DateTime.Now;
                            DPAH.UPDATEDATE = DateTime.Now;
                            _CommunicationDBContext.Entry(DPAH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _CommunicationDBContext.SaveChanges();
                        }

                        //if (PHVM.APPROVAL_STATUS == 4) //// - Send back to Brand & Comm Team
                        //{
                        //    CM_COMMAPPAUTH_SEQ seqModel = new CM_COMMAPPAUTH_SEQ();
                        //    seqModel = _CommunicationDBContext.CM_COMMAPPAUTH_SEQ.Where(a => a.APPTYPE == 2 && a.COMMUNICATIONID == PHVM.COMMUNICATIONID).FirstOrDefault();
                        //    if (seqModel != null)
                        //    {
                        //        SaveCommAppHis(PHVM.ADDEDBY, PHVM.COMMUNICATIONID, seqModel);
                        //    }
                        //}
                        //else
                        //{

                        //// --- Save Comm Approval Authority --- ///
                        CM_COMMAPPAUTH_SEQ seqModel = new CM_COMMAPPAUTH_SEQ();
                        if (PHVM.APPROVAL_STATUS == 1)
                        {
                            short nextSeq = Convert.ToInt16(_CommunicationDBContext.CM_COMMAPPAUTH_SEQ.Where(s => s.COMMUNICATIONID == PHVM.COMMUNICATIONID && s.ADEMPCODE == (PHVM.APPTYPE == 2 ? 1 : PHVM.ADEMPCODE)).Select(s => s.APP_SEQ).FirstOrDefault() + 1);
                            long _nextEmpAuth = _CommunicationDBContext.CM_COMMAPPAUTH_SEQ.Where(a => a.APP_SEQ == nextSeq && a.COMMUNICATIONID == PHVM.COMMUNICATIONID).Select(s => s.ADEMPCODE).FirstOrDefault();
                            if (_nextEmpAuth == (PHVM.APPTYPE == 2 ? 1 : PHVM.ADEMPCODE))
                            {
                                nextSeq = Convert.ToInt16(nextSeq + 1);
                            }
                            seqModel = _CommunicationDBContext.CM_COMMAPPAUTH_SEQ.Where(a => a.APP_SEQ == nextSeq && a.COMMUNICATIONID == PHVM.COMMUNICATIONID).FirstOrDefault();
                            if (seqModel != null)
                            {
                                SaveCommAppHis(PHVM.ADDEDBY, PHVM.COMMUNICATIONID, seqModel);
                            }
                        }
                        //////// Update Process Status ////////
                        CM_COMMUNICATION_HDR DPH = new CM_COMMUNICATION_HDR(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
                        DPH = _CommunicationDBContext.CM_COMMUNICATION_HDR.Where(x => x.COMMUNICATIONID == PHVM.COMMUNICATIONID).SingleOrDefault();
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
                            //// - In Case of Brand & Comm Approval - ////
                            if (PHVM.APPTYPE == 2 && PHVM.CHANGES_REQUIRED == 1)
                            {
                                DPH.SUBJECT = PHVM.SUBJECT;
                                DPH.CONTENT = CommonRepository.TextToHtml(PHVM.CONTENT);
                                DPH.EMAIL_CONTENT = PHVM.EMAIL_CONTENT;
                            }
                            DPH.MODIFIED_BY = PHVM.UPDATEBY;
                            DPH.MODIFIED_DATE = DateTime.Now;
                            //_CommunicationDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _CommunicationDBContext.SaveChanges();
                        }
                        //// --- Add Brand & Comm Attachment --- ////
                        if (commDetailList.Count > 0)
                        {
                            SaveCommAttachment(PHVM.ADDEDBY, PHVM.COMMUNICATIONID, commDetailList);
                        }
                        //// --- End --- ////

                        //}
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


        public Tuple<short, long> UpdateCommunicationRequest(CommunicationViewModel model)
        {
            short retVal = 0; long retHeaderId = 0;
            Tuple<short, long> _retVal_tuple;
            //using (DbContextTransaction transaction = (DbContextTransaction)_CommunicationDBContext.Database.BeginTransaction())
            using (var transaction = _CommunicationDBContext.Database.BeginTransaction())
            {
                try
                {
                    CM_COMMUNICATION_HDR CCH = new CM_COMMUNICATION_HDR();
                    if (model.COMMUNICATIONID > 0)
                    {
                        CCH = _CommunicationDBContext.CM_COMMUNICATION_HDR.Where(x => x.COMMUNICATIONID == model.COMMUNICATIONID).SingleOrDefault();
                        CCH.SUBJECT = model.SUBJECT;
                        CCH.CONTENT = CommonRepository.TextToHtml(model.CONTENT);
                        CCH.MODIFIED_DATE = DateTime.Now;
                        CCH.MODIFIED_BY = model.MODIFIED_BY;
                        CCH.EMAIL_CONTENT = model.EMAIL_CONTENT; // CommonRepository.TextToHtml(model.EMAIL_CONTENT);
                        _CommunicationDBContext.Entry(CCH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        _CommunicationDBContext.SaveChanges();
                        transaction.Commit();
                        retVal = 1;
                    }
                    else
                    {
                        retVal = 0;
                        transaction.Rollback();
                    }
                    retHeaderId = model.COMMUNICATIONID;
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
        public List<CommunicationViewModel> ArchiveRequestList(int type, CommunicationViewModel VM)
        {
            DateTime ReqDateFrom = DateTime.Now.Date;
            DateTime ReqDateTo = DateTime.Now.Date;

            if (!string.IsNullOrEmpty(VM.START_DATE))
            {
                if (DateTime.TryParse(VM.START_DATE, out DateTime parsedStartDate))
                {
                    ReqDateFrom = parsedStartDate.ToLocalTime();  // Convert UTC to local time if needed
                }
            }

            if (!string.IsNullOrEmpty(VM.END_DATE))
            {
                if (DateTime.TryParse(VM.END_DATE, out DateTime parsedEndDate))
                {
                    ReqDateTo = parsedEndDate.ToLocalTime().Date.AddHours(23).AddMinutes(59).AddSeconds(59);  // Set end of day
                }
            }

            List<CommunicationViewModel> iListNew = new List<CommunicationViewModel>();
            var iList = (from data in _CommunicationDBContext.CM_COMMUNICATION_HDR
                         join _cat in _CommunicationDBContext.CM_COMMCATEGORY_MST on data.COMM_CATID equals _cat.COMMCATEGORYID
                         join _type in _CommunicationDBContext.CM_COMMTYPE_MST on data.COMM_MAILTYPEID equals _type.COMMTYPEID
                         join _AddBy in _CommunicationDBContext.ADEMPLOYEE on data.CREATED_BY equals _AddBy.ADEMPCODE
                         where (!string.IsNullOrEmpty(VM.START_DATE) ? (data.MODIFIED_DATE >= ReqDateFrom) : true)
                         && (!string.IsNullOrEmpty(VM.END_DATE) ? (data.MODIFIED_DATE <= ReqDateTo) : true)
                         && data.PROCESS_STATUS == 2
                         && (type == 1 ? (data.COMM_CATID == 1 || data.COMM_CATID == 2 || data.COMM_CATID == 3) : data.COMM_CATID == 3)
                         //where data.PROCESS_STATUS == 2 && (type == 1 ? (data.COMM_CATID == 1 || data.COMM_CATID == 2) : data.COMM_CATID == 3)
                         select new CommunicationViewModel
                         {
                             COMMUNICATIONID = data.COMMUNICATIONID,
                             COMM_CATID = _cat.COMMCATEGORYID,
                             CATEGORY = _cat.CATEGORY_NAME,
                             COMM_MAILTYPEID = _type.COMMTYPEID,
                             MAILTYPE = _type.COMM_TYPE,
                             MAILID = _type.COMM_EMAILID,
                             CONTENT = data.CONTENT,
                             SUBJECT = data.SUBJECT,
                             _SDATE = data.START_DATE,
                             _EDATE = data.END_DATE,
                             //EMAILSTATUS = data.EMAILSTATUS == 1 ? true : false,
                             EMAILSTATUS = Convert.ToBoolean(data.EMAILSTATUS),
                             CREATED_BY = data.CREATED_BY,
                             CREATED_BY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             CREATED_DATE = data.CREATED_DATE,
                             MODIFIED_DATE = data.MODIFIED_DATE,
                             PROCESS_STATUS = data.PROCESS_STATUS,
                             STATUS = data.STATUS,
                             REQUESTNO = data.REQUESTNO,
                             Recipient = data.RECIPIENT,
                             REQUEST_TYPE = data.REQUEST_TYPE,
                             FnDesignationIds = data.FUNCTIONAL_DESIGNATION,
                             DesignationIds = data.DESIGNATION,
                             SiteIds = data.SITE,
                             OPIds = data.OPERATION,
                             DIVIds = data.DIVISION,
                             DEPIds = data.DEPARTMENT,
                         }).OrderByDescending(o => o.MODIFIED_DATE).ToList();



            if (iList.Count > 0)
            {
                //Employee_Details emp = (Employee_Details)Context.Session["Employee"];
                Employee_Details emp = _sessionService.Get<Employee_Details>("Employee");
                foreach (var obj in iList)
                {
                    bool fundesg = true;
                    bool desg = true;
                    bool site = true;
                    bool operation = true;
                    bool division = true;
                    bool department = true;

                    if (!string.IsNullOrEmpty(obj.FnDesignationIds))
                        fundesg = false;
                    if (!string.IsNullOrEmpty(obj.DesignationIds))
                        desg = false;
                    if (!string.IsNullOrEmpty(obj.SiteIds))
                        site = false;
                    if (!string.IsNullOrEmpty(obj.OPIds))
                        operation = false;
                    if (!string.IsNullOrEmpty(obj.DIVIds))
                        division = false;
                    if (!string.IsNullOrEmpty(obj.DEPIds))
                        department = false;

                    if (!string.IsNullOrEmpty(obj.FnDesignationIds) && obj.FnDesignationIds.TrimEnd(',').Split(',').Contains(emp.Functional_Designation_Id))
                        fundesg = true;
                    if (!string.IsNullOrEmpty(obj.DesignationIds) && obj.DesignationIds.TrimEnd(',').Split(',').Contains(emp.Designation_Id))
                        desg = true;
                    if (!string.IsNullOrEmpty(obj.SiteIds) && obj.SiteIds.TrimEnd(',').Split(',').Contains(emp._SiteId.ToString()))
                        site = true;
                    if (!string.IsNullOrEmpty(obj.OPIds) && obj.OPIds.TrimEnd(',').Split(',').Contains(emp._OpId.ToString()))
                        operation = true;
                    if (!string.IsNullOrEmpty(obj.DIVIds) && obj.DIVIds.TrimEnd(',').Split(',').Contains(emp._DivId.ToString()))
                        division = true;
                    if (!string.IsNullOrEmpty(obj.DEPIds) && obj.DEPIds.TrimEnd(',').Split(',').Contains(emp._DepId.ToString()))
                        department = true;

                    if (fundesg && desg && site && operation && division && department)
                    {
                        if (VM.COMM_CATID == 0 ? true : obj.COMM_CATID == VM.COMM_CATID)
                        {
                            iListNew.Add(obj);
                        }
                    }
                }
            }
            return iListNew;
        }

        public short CancelCommunicationRequest(long id, long userID)
        {
            short retval = 0;
            CM_COMMUNICATION_HDR DPH = new CM_COMMUNICATION_HDR(); ///////// Approval Status(0-Senback, 1-WIP, 2-Complete, 3-Reject, 4-Cancel)
            DPH = _CommunicationDBContext.CM_COMMUNICATION_HDR.Where(x => x.COMMUNICATIONID == id).SingleOrDefault();
            if (DPH != null)
            {
                DPH.PROCESS_STATUS = 4;
                DPH.MODIFIED_BY = userID;
                DPH.MODIFIED_DATE = DateTime.Now;
                _CommunicationDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _CommunicationDBContext.SaveChanges();

                retval = 1;
            }
            return retval;
        }

        public short DeactivateCommunicationRequest(long id, long userID)
        {
            short retval = 0;
            CM_COMMUNICATION_HDR DPH = new CM_COMMUNICATION_HDR();
            DPH = _CommunicationDBContext.CM_COMMUNICATION_HDR.Where(x => x.COMMUNICATIONID == id).SingleOrDefault();
            if (DPH != null)
            {
                DPH.PROCESS_STATUS = 5; // Cancelled by Admin
                DPH.MODIFIED_BY = userID;
                DPH.MODIFIED_DATE = DateTime.Now;
                //_CommunicationDBContext.Entry(DPH).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _CommunicationDBContext.SaveChanges();

                retval = 1;
            }
            return retval;
        }
        public List<CommunicationViewModel> SelfPendingCommunicationList(long userId)
        {
            List<CommunicationViewModel> iListNew = new List<CommunicationViewModel>();
            var iList = (from data in _CommunicationDBContext.CM_COMMUNICATION_HDR.Where(c => c.STATUS == 1 && c.PROCESS_STATUS == 2 && (c.COMM_CATID == 1 || c.COMM_CATID == 2))
                         join _cat in _CommunicationDBContext.CM_COMMCATEGORY_MST on data.COMM_CATID equals _cat.COMMCATEGORYID
                         //join _type in _CommunicationDBContext.CM_COMMTYPE_MST on data.COMM_MAILTYPEID equals _type.COMMTYPEID
                         //join _AddBy in _CommunicationDBContext.ADEMPLOYEE on data.CREATED_BY equals _AddBy.ADEMPCODE
                         where 0 == (_CommunicationDBContext.CM_COMMREQ_VIEWEMPMAPP.Count(c => c.COMMUNICATIONID == data.COMMUNICATIONID && c.ADEMPCODE == userId))
                         select new CommunicationViewModel
                         {
                             COMMUNICATIONID = data.COMMUNICATIONID,
                             CATEGORY = _cat.CATEGORY_NAME,
                             //MAILTYPE = _type.COMM_TYPE,
                             //MAILID = _type.COMM_EMAILID,
                             CONTENT = data.CONTENT,
                             SUBJECT = data.SUBJECT,
                             EMAILSTATUS = data.EMAILSTATUS == 1 ? true : false,
                             EMAIL_CONTENT = data.EMAIL_CONTENT,
                             CREATED_BY = data.CREATED_BY,
                             //CREATED_BY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             CREATED_DATE = data.CREATED_DATE,
                             FnDesignationIds = data.FUNCTIONAL_DESIGNATION,
                             DesignationIds = data.DESIGNATION,
                             SiteIds = data.SITE,
                             OPIds = data.OPERATION,
                             DIVIds = data.DIVISION,
                             DEPIds = data.DEPARTMENT,
                         }).OrderBy(o => o.CREATED_DATE).ToList();
            if (iList.Count > 0)
            {
                //Employee_Details emp = (Employee_Details)Context.Session["Employee"];
                Employee_Details emp = _sessionService.Get<Employee_Details>("Employee");
                foreach (var obj in iList)
                {
                    bool fundesg = true;
                    bool desg = true;
                    bool site = true;
                    bool operation = true;
                    bool division = true;
                    bool department = true;

                    if (!string.IsNullOrEmpty(obj.FnDesignationIds))
                        fundesg = false;
                    if (!string.IsNullOrEmpty(obj.DesignationIds))
                        desg = false;
                    if (!string.IsNullOrEmpty(obj.SiteIds))
                        site = false;
                    if (!string.IsNullOrEmpty(obj.OPIds))
                        operation = false;
                    if (!string.IsNullOrEmpty(obj.DIVIds))
                        division = false;
                    if (!string.IsNullOrEmpty(obj.DEPIds))
                        department = false;

                    if (!string.IsNullOrEmpty(obj.FnDesignationIds) && obj.FnDesignationIds.TrimEnd(',').Split(',').Contains(emp.Functional_Designation_Id))
                        fundesg = true;
                    if (!string.IsNullOrEmpty(obj.DesignationIds) && obj.DesignationIds.TrimEnd(',').Split(',').Contains(emp.Designation_Id))
                        desg = true;
                    if (!string.IsNullOrEmpty(obj.SiteIds) && obj.SiteIds.TrimEnd(',').Split(',').Contains(emp._SiteId.ToString()))
                        site = true;
                    if (!string.IsNullOrEmpty(obj.OPIds) && obj.OPIds.TrimEnd(',').Split(',').Contains(emp._OpId.ToString()))
                        operation = true;
                    if (!string.IsNullOrEmpty(obj.DIVIds) && obj.DIVIds.TrimEnd(',').Split(',').Contains(emp._DivId.ToString()))
                        division = true;
                    if (!string.IsNullOrEmpty(obj.DEPIds) && obj.DEPIds.TrimEnd(',').Split(',').Contains(emp._DepId.ToString()))
                        department = true;

                    if (fundesg && desg && site && operation && division && department)
                    {
                        iListNew.Add(obj);
                    }
                }
            }
            return iListNew;
        }

        public short InserViewData(long id, long userID)
        {
            short retval = 0; int FlagAdd = 0;
            CM_COMMREQ_VIEWEMPMAPP DPH = new CM_COMMREQ_VIEWEMPMAPP();
            DPH = _CommunicationDBContext.CM_COMMREQ_VIEWEMPMAPP.Where(x => x.COMMUNICATIONID == id && x.ADEMPCODE == userID).FirstOrDefault();
            if (DPH == null)
            {
                DPH = new CM_COMMREQ_VIEWEMPMAPP();
                if (_CommunicationDBContext.CM_COMMREQ_VIEWEMPMAPP.Count() == 0)
                {
                    DPH.COMMVIEW_MAPPID = 1;
                }
                else
                {
                    DPH.COMMVIEW_MAPPID = _CommunicationDBContext.CM_COMMREQ_VIEWEMPMAPP.Max(x => x.COMMVIEW_MAPPID) + 1;
                }
                FlagAdd = 1;
            }
            DPH.COMMUNICATIONID = id;
            DPH.ADEMPCODE = userID;
            if (FlagAdd == 1)
            {
                DPH.CREATED_BY = userID;
                DPH.CREATED_DATE = DateTime.Now;
            }
            else
            {
                DPH.MODIFIED_BY = userID;
                DPH.MODIFIED_DATE = DateTime.Now;
            }
            _CommunicationDBContext.Entry(DPH).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
            _CommunicationDBContext.SaveChanges();

            retval = 1;
            return retval;
        }

        public List<CommMailTypeViewModel> GetCommunicationTypeList()
        {
            var iList = (from data in _CommunicationDBContext.CM_COMMTYPE_MST
                         join _AddBy in _CommunicationDBContext.ADEMPLOYEE on data.CREATED_BY equals _AddBy.ADEMPCODE
                         select new CommMailTypeViewModel
                         {
                             COMMTYPEID = data.COMMTYPEID,
                             COMM_TYPE = data.COMM_TYPE,
                             COMM_EMAILID = data.COMM_EMAILID,
                             //STATUS = data.STATUS == 1 ? true : false,
                             STATUS = Convert.ToBoolean(data.STATUS),
                             CREATED_BY = data.CREATED_BY,
                             ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                             CREATED_DATE = data.CREATED_DATE,
                         });
            return iList.ToList();
        }

        public CommMailTypeViewModel GetCommunicationTypeDtlById(long id)
        {
            var _obj = (from data in _CommunicationDBContext.CM_COMMTYPE_MST.Where(x => x.COMMTYPEID == id)
                        join _AddBy in _CommunicationDBContext.ADEMPLOYEE on data.CREATED_BY equals _AddBy.ADEMPCODE
                        select new CommMailTypeViewModel
                        {
                            COMMTYPEID = data.COMMTYPEID,
                            COMM_TYPE = data.COMM_TYPE,
                            COMM_EMAILID = data.COMM_EMAILID,
                            //STATUS = data.STATUS == 1 ? true : false,
                            STATUS = Convert.ToBoolean(data.STATUS),
                            CREATED_BY = data.CREATED_BY,
                            ADDEDBY_NAME = _AddBy.FIRSTNAME + " " + _AddBy.LASTNAME,
                            CREATED_DATE = data.CREATED_DATE,
                        }).FirstOrDefault();
            return _obj;
        }

        public short SaveCommunicationType(CommMailTypeViewModel AOVM)
        {
            short retVal = 0;
            //using (DbContextTransaction transaction = (DbContextTransaction)_CommunicationDBContext.Database.BeginTransaction())
            using (var transaction = _CommunicationDBContext.Database.BeginTransaction())
            {
                try
                {
                    CM_COMMTYPE_MST SAO = new CM_COMMTYPE_MST();
                    int FlagAdd = 0;
                    if (AOVM.COMMTYPEID > 0)
                    {
                        int cnt = _CommunicationDBContext.CM_COMMTYPE_MST.Count(x => x.COMM_TYPE == AOVM.COMM_TYPE && x.COMMTYPEID != AOVM.COMMTYPEID);
                        //if (_CommunicationDBContext.CM_COMMTYPE_MST.Any(x => x.COMM_TYPE == AOVM.COMM_TYPE && x.COMMTYPEID != AOVM.COMMTYPEID))
                        if (cnt > 0)
                        {
                            return retVal = 2;
                        }
                        SAO = _CommunicationDBContext.CM_COMMTYPE_MST.Where(x => x.COMMTYPEID == AOVM.COMMTYPEID).SingleOrDefault();
                    }
                    else
                    {
                        int count = _CommunicationDBContext.CM_COMMTYPE_MST.Count(x => x.COMM_TYPE == AOVM.COMM_TYPE);

                        //if (_CommunicationDBContext.CM_COMMTYPE_MST.Any(x => x.COMM_TYPE == AOVM.COMM_TYPE))
                        if (count > 0)
                        {
                            return retVal = 2;
                        }

                        SAO = new CM_COMMTYPE_MST();
                        if (_CommunicationDBContext.CM_COMMTYPE_MST.ToList().Count == 0)
                        {
                            SAO.COMMTYPEID = 1;
                        }
                        else
                        {
                            SAO.COMMTYPEID = _CommunicationDBContext.CM_COMMTYPE_MST.Max(x => x.COMMTYPEID) + 1;
                        }
                        FlagAdd = 1;
                    }
                    SAO.COMM_TYPE = AOVM.COMM_TYPE;
                    SAO.COMM_EMAILID = AOVM.COMM_EMAILID;
                    SAO.STATUS = Convert.ToInt16(AOVM.STATUS == true ? 1 : 0);
                    if (FlagAdd == 1)
                    {
                        SAO.CREATED_BY = AOVM.CREATED_BY;
                        SAO.CREATED_DATE = DateTime.Now;
                    }
                    else
                    {
                        SAO.MODIFIED_BY = AOVM.MODIFIED_BY;
                        SAO.MODIFIED_DATE = DateTime.Now;
                    }
                    _CommunicationDBContext.Entry(SAO).State = FlagAdd == 1 ? Microsoft.EntityFrameworkCore.EntityState.Added : Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _CommunicationDBContext.SaveChanges();
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

        public List<Comm_Op_Div_DepViewModel> BindOperation(long typeId)
        {
            var iList = (from data in _AnnouncementDBContext.ADORGLEVEL
                         where data.ACTIVE == 1 && data.SYKIID == _Syki.SYKIID && data.ADORGLEVELTYPEID == typeId
                         select new Comm_Op_Div_DepViewModel
                         {
                             Value = data.ADORGLEVELID,
                             Text = data.LEVELDESCRIP
                         });
            return iList.ToList();
        }

        public List<Comm_Op_Div_DepViewModel> BindDivision(long typeId, long op_Id)
        {
            var iList = (from data in _AnnouncementDBContext.ADORGLEVEL
                         where data.ACTIVE == 1 && data.SYKIID == _Syki.SYKIID
                         && data.ADORGLEVELTYPEID == typeId && data.PARENTLEVELID == op_Id
                         select new Comm_Op_Div_DepViewModel
                         {
                             Value = data.ADORGLEVELID,
                             Text = data.LEVELDESCRIP
                         });
            return iList.ToList();
        }

        public List<Comm_Op_Div_DepViewModel> BindDepartment(long typeId, long div_Id)
        {
            var iList = (from data in _AnnouncementDBContext.ADORGLEVEL
                         where data.ACTIVE == 1 && data.SYKIID == _Syki.SYKIID
                         && data.ADORGLEVELTYPEID == typeId && data.PARENTLEVELID == div_Id
                         select new Comm_Op_Div_DepViewModel
                         {
                             Value = data.ADORGLEVELID,
                             Text = data.LEVELDESCRIP
                         });
            return iList.ToList();
        }

        public List<CommunicationViewModel> ValidateArchiveRequestList(long CommID)
        {
            List<CommunicationViewModel> iListNew = new List<CommunicationViewModel>();
            var iList = (from data in _CommunicationDBContext.CM_COMMUNICATION_HDR.Where(x => x.COMMUNICATIONID == CommID)
                         where data.PROCESS_STATUS == 2
                         select new CommunicationViewModel
                         {
                             COMMUNICATIONID = data.COMMUNICATIONID,
                             CONTENT = data.CONTENT,
                             SUBJECT = data.SUBJECT,
                             _SDATE = data.START_DATE,
                             _EDATE = data.END_DATE,
                             //EMAILSTATUS = data.EMAILSTATUS == 1 ? true : false,
                             EMAILSTATUS = Convert.ToBoolean(data.EMAILSTATUS),
                             CREATED_BY = data.CREATED_BY,
                             CREATED_DATE = data.CREATED_DATE,
                             PROCESS_STATUS = data.PROCESS_STATUS,
                             STATUS = data.STATUS,
                             REQUESTNO = data.REQUESTNO,
                             Recipient = data.RECIPIENT,
                             REQUEST_TYPE = data.REQUEST_TYPE,
                             FnDesignationIds = data.FUNCTIONAL_DESIGNATION,
                             DesignationIds = data.DESIGNATION,
                             SiteIds = data.SITE,
                             OPIds = data.OPERATION,
                             DIVIds = data.DIVISION,
                             DEPIds = data.DEPARTMENT,
                         }).ToList();
            if (iList.Count > 0)
            {
                //Employee_Details emp = (Employee_Details)Context.Session["Employee"];
                Employee_Details emp = _sessionService.Get<Employee_Details>("Employee");
                foreach (var obj in iList)
                {
                    bool fundesg = true;
                    bool desg = true;
                    bool site = true;
                    bool operation = true;
                    bool division = true;
                    bool department = true;

                    if (!string.IsNullOrEmpty(obj.FnDesignationIds))
                        fundesg = false;
                    if (!string.IsNullOrEmpty(obj.DesignationIds))
                        desg = false;
                    if (!string.IsNullOrEmpty(obj.SiteIds))
                        site = false;
                    if (!string.IsNullOrEmpty(obj.OPIds))
                        operation = false;
                    if (!string.IsNullOrEmpty(obj.DIVIds))
                        division = false;
                    if (!string.IsNullOrEmpty(obj.DEPIds))
                        department = false;

                    if (!string.IsNullOrEmpty(obj.FnDesignationIds) && obj.FnDesignationIds.TrimEnd(',').Split(',').Contains(emp.Functional_Designation_Id))
                        fundesg = true;
                    if (!string.IsNullOrEmpty(obj.DesignationIds) && obj.DesignationIds.TrimEnd(',').Split(',').Contains(emp.Designation_Id))
                        desg = true;
                    if (!string.IsNullOrEmpty(obj.SiteIds) && obj.SiteIds.TrimEnd(',').Split(',').Contains(emp._SiteId.ToString()))
                        site = true;
                    if (!string.IsNullOrEmpty(obj.OPIds) && obj.OPIds.TrimEnd(',').Split(',').Contains(emp._OpId.ToString()))
                        operation = true;
                    if (!string.IsNullOrEmpty(obj.DIVIds) && obj.DIVIds.TrimEnd(',').Split(',').Contains(emp._DivId.ToString()))
                        division = true;
                    if (!string.IsNullOrEmpty(obj.DEPIds) && obj.DEPIds.TrimEnd(',').Split(',').Contains(emp._DepId.ToString()))
                        department = true;

                    if (fundesg && desg && site && operation && division && department)
                    {
                        iListNew.Add(obj);
                    }
                }
            }
            return iListNew;
        }

        #endregion

        public DataTable GetCommunicationReport(string Sdate, string Edate, string KI)
        {
            try
            {
                // Parse the start and end dates outside the LINQ query
                DateTime startDate;
                DateTime endDate;

                if (!DateTime.TryParse(Sdate, out startDate) || !DateTime.TryParse(Edate, out endDate))
                {
                    // Handle invalid date format (you can throw an exception or return an empty list)
                    throw new ArgumentException("Invalid date format.");
                }

                // Parse KI outside the LINQ query
                long kiValue;
                if (!long.TryParse(KI, out kiValue))
                {
                    // Handle invalid KI (you could return an empty list, throw an exception, or handle it as needed)
                    throw new ArgumentException("Invalid KI value");
                }
                var query = from hdr in _CommunicationDBContext.CM_COMMUNICATION_HDR
                            join vw in _CommunicationDBContext.VW_ASSOCIATELVLDETAILS on hdr.CREATED_BY equals vw.ADEMPCODE
                            join emp in _CommunicationDBContext.ADEMPLOYEE on hdr.CREATED_BY equals emp.ADEMPCODE
                            join cm in _CommunicationDBContext.CM_COMMCATEGORY_MST on hdr.COMM_CATID equals cm.COMMCATEGORYID
                            join mt in _CommunicationDBContext.CM_COMMTYPE_MST on hdr.COMM_MAILTYPEID equals mt.COMMTYPEID
                            join ah in _CommunicationDBContext.CM_COMMUNICATION_APPHISTORY on hdr.COMMUNICATIONID equals ah.COMMUNICATIONID
                            join actionEmp in _CommunicationDBContext.ADEMPLOYEE on ah.ADEMPCODE equals actionEmp.ADEMPCODE // Join to get the name for ACTION_BY
                            where ah.APPTYPE == 2 &&
                                  new[] { 1, 2 }.Contains(ah.APPROVAL_STATUS) &&
                                  vw.SYKI == kiValue &&  // Use parsed kiValue here
                                  ah.APPROVAL_DATE >= startDate &&  // Use parsed startDate here
                                  ah.APPROVAL_DATE <= endDate  // Use parsed endDate here
                            select new
                            {
                                hdr.COMMUNICATIONID,
                                REQUESTOR = emp.FIRSTNAME + " " + emp.LASTNAME + " - " + emp.ADEMPCODE,
                                vw.OPERATION,
                                vw.DIVISION,
                                vw.DEPARTMENT,
                                vw.SECTION,
                                cm.CATEGORY_NAME,
                                mt.COMM_TYPE,
                                mt.COMM_EMAILID,
                                hdr.SUBJECT,
                                //ACTION_BY = ah.ADEMPCODE,
                                ACTION_BY = ah.ADEMPCODE + " - " + actionEmp.FIRSTNAME + " " + actionEmp.LASTNAME, // Getting first and last name of ACTION_BY
                                ACTION_DATE = ah.APPROVAL_DATE,
                                ACTION_STATUS = ah.APPROVAL_STATUS == 1 ? "Approved"
                                              : ah.APPROVAL_STATUS == 2 ? "Rejected"
                                              : "NA",
                                Remarks = ah.APPROVAL_REMARK
                            };

                // Fetch data from the database
                var result = query.ToList();
                // Create a new DataTable to store the results
                DataTable dt = new DataTable();

                // Add columns to DataTable based on the properties from the query result
                dt.Columns.Add("COMMUNICATIONID", typeof(int));
                dt.Columns.Add("CATEGORY", typeof(string));
                dt.Columns.Add("COMMUNICATION_TYPE", typeof(string));
                dt.Columns.Add("MAILID", typeof(string));
                dt.Columns.Add("SUBJECT", typeof(string));
                dt.Columns.Add("REQUESTOR", typeof(string));
                dt.Columns.Add("OPERATION_NAME", typeof(string));
                dt.Columns.Add("DIVISION_NAME", typeof(string));
                dt.Columns.Add("DEPARTMENT_NAME", typeof(string));
                dt.Columns.Add("SECTION_NAME", typeof(string));
                dt.Columns.Add("ACTION_STATUS", typeof(string));
                dt.Columns.Add("REMARK", typeof(string));
                dt.Columns.Add("ACTION_DATE", typeof(string));
                dt.Columns.Add("ACTION_BY", typeof(string));

                // Add rows to the DataTable based on the query result
                foreach (var item in result)
                {
                    DataRow row = dt.NewRow();
                    row["COMMUNICATIONID"] = item.COMMUNICATIONID;
                    row["CATEGORY"] = item.CATEGORY_NAME;
                    row["COMMUNICATION_TYPE"] = item.COMM_TYPE;
                    row["MAILID"] = item.COMM_EMAILID;
                    row["SUBJECT"] = item.SUBJECT;
                    row["REQUESTOR"] = item.REQUESTOR;
                    row["OPERATION_NAME"] = item.OPERATION;
                    row["DIVISION_NAME"] = item.DIVISION;
                    row["DEPARTMENT_NAME"] = item.DEPARTMENT;
                    row["SECTION_NAME"] = item.SECTION;
                    row["ACTION_STATUS"] = item.ACTION_STATUS;
                    row["REMARK"] = item.Remarks;
                    row["ACTION_DATE"] = item.ACTION_DATE.HasValue ? item.ACTION_DATE.Value.ToString("dd-MMM-yyyy") : "";
                    row["ACTION_BY"] = item.ACTION_BY;

                    dt.Rows.Add(row);
                }

                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching the communication report", ex);
            }
        }
        public List<SYKI_DGIT> GetKIByYear()
        {
            // Fetch the top 3 KI records from the database, ordered by SYKIID
            var query = _CommunicationDBContext.SYKI
                   .OrderByDescending(ki => ki.SYKIID) // Order by SYKIID descending
                   .Take(3)                            // Get the top 3 records
                   .ToList();
            return query;                        // Convert the result to a list
        }
        //Added by aumento for SR87026
    }
}