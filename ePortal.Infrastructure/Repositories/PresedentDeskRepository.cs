using ePortal.DomainClasses;
using ePortal.Infrastructure.DbContexts;
using ePortal.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ePortal.Infrastructure.Repositories
{
    public class PresedentDeskRepository
    {       
        private EPortalDBContext _pdDBContext;
        
        public PresedentDeskRepository(EPortalDBContext pdDBContext)
        {
            _pdDBContext = pdDBContext;          
        }

        public PresidentDeskViewModel GetPresidentDetail(PresidentDeskSearchModel objSearchModel)
        {
            PresidentDeskViewModel pDV = new PresidentDeskViewModel();

            var objdata = (from data in _pdDBContext.CM_PRESIDENT_MST
                           join aLU in _pdDBContext.ADLOGINUSER on data.CREATED_BY equals aLU.ADEMPCODE into tempLoginUser
                           from aLU in tempLoginUser.DefaultIfEmpty()
                           join aLU1 in _pdDBContext.ADLOGINUSER on data.MODIFIED_BY equals aLU1.ADEMPCODE into tempLoginUser1
                           from aLU1 in tempLoginUser1.DefaultIfEmpty()
                           where
                          objSearchModel.PresidentId == 0 ? true : data.PRESIDENT_ID == objSearchModel.PresidentId
                           select new
                           {
                               data.PHOTO_CONTENTTYPE,
                               data.PHOTO_NAME,
                               data.PRESIDENT_ID,
                               data.PRESIDENT_NAME,
                               data.PRESIDENT_PHOTO,
                               data.PRESIDENT_TITLE,
                               data.REMARKS,
                               data.STATUS,
                               data.ACTIVE_FROM,
                               data.ACTIVE_TO,
                               aLUFirstName = aLU.FIRSTNAME,
                               aLULastName = aLU.LASTNAME,
                               aLU1FirstName = aLU1.FIRSTNAME,
                               aLU1LastName = aLU1.LASTNAME
                           }).ToList();
            if (objdata.Count > 0)
            {

                foreach (var data in objdata)
                {
                    pDV.PresidentDetail.Add(new PresidentDesk_MSTViewModel
                    {
                        PHOTO_CONTENTTYPE = data.PHOTO_CONTENTTYPE,
                        PHOTO_NAME = data.PHOTO_NAME,
                        PRESIDENT_ID = data.PRESIDENT_ID,
                        PRESIDENT_NAME = data.PRESIDENT_NAME,
                        PRESIDENT_PHOTO = data.PRESIDENT_PHOTO,
                        PRESIDENT_TITLE = data.PRESIDENT_TITLE,
                        REMARKS = data.REMARKS,
                        STATUS = data.STATUS,
                        ACTIVE_FROM = data.ACTIVE_FROM.ToString("dd-MMM-yyyy"),
                        ACTIVE_TO = data.ACTIVE_TO == null ? "" : Convert.ToDateTime(data.ACTIVE_TO).ToString("dd-MMM-yyyy"),
                        CREATED_BY_NAME = data.aLUFirstName + " " + data.aLULastName,
                        MODIFIED_BY_NAME = data.aLU1FirstName + " " + data.aLU1LastName
                        //pDV.PresidentDetail.Add(pDMVM);
                    });
                }
            }

            var objTRN = (from data in _pdDBContext.CM_PRESIDENTMSG_TRN
                          join cPM in _pdDBContext.CM_PRESIDENT_MST on data.PRESIDENT_ID equals cPM.PRESIDENT_ID into tempPresident
                          from cPM in tempPresident.DefaultIfEmpty()
                          join cPMAPP in _pdDBContext.CM_PRESIDENTMSGAPP_TRN on data.PRESIDENTMSG_ID equals cPMAPP.PRESIDENTMSG_ID into tempPresidentMSGAPP
                          from cPMAPP in tempPresidentMSGAPP.DefaultIfEmpty()
                          join aLU in _pdDBContext.ADLOGINUSER on data.CREATED_BY equals aLU.ADEMPCODE into tempLoginUser
                          from aLU in tempLoginUser.DefaultIfEmpty()
                          join aLU1 in _pdDBContext.ADLOGINUSER on data.MODIFIED_BY equals aLU1.ADEMPCODE into tempLoginUser1
                          from aLU1 in tempLoginUser1.DefaultIfEmpty()
                          where //data.STATUS == objsearch.STATUS
                         objSearchModel.PresidentMsgId == 0 ? true : data.PRESIDENTMSG_ID == objSearchModel.PresidentMsgId
                          orderby data.CREATED_DATE descending
                          select new
                          {
                              data.BRIEF,
                              data.MESSAGE,
                              data.CREATED_DATE,
                              data.MODIFIED_DATE,
                              data.VALID_FROM,
                              data.VALID_TO,
                              data.STATUS,
                              data.PRESIDENTMSG_ID,
                              data.ATTACHMENT_NAME,
                              data.ATTACHMENT_CONTENTTYPE,
                              //data.ATTACHMENT,
                              cPM.PRESIDENT_ID,    //data.CM_PRESIDENT_MST.PRESIDENT_ID,
                              cPM.PRESIDENT_NAME,
                              aLUFirstName = aLU.FIRSTNAME,
                              aLULastName = aLU.LASTNAME,
                              aLU1FirstName = aLU1.FIRSTNAME,
                              aLU1LastName = aLU1.LASTNAME,
                              cPMAPP.INITIATOR_REMARKS
                          }).ToList();

            if (objTRN.Count > 0)
            {

                foreach (var data in objTRN)
                {
                    pDV.President_TRNDTL.Add(new PresidentDesk_TRNViewModel
                    {
                        BRIEF = data.BRIEF,
                        MESSAGE = CommonRepository.HtmlToText(data.MESSAGE),
                        VALID_FROM = data.VALID_FROM.ToString("dd-MMM-yyyy"),
                        VALID_TO = data.VALID_TO.ToString("dd-MMM-yyyy"),
                        STATUS = data.STATUS,
                        PRESIDENTMSG_ID = data.PRESIDENTMSG_ID,
                        ATTACHMENT_NAME = data.ATTACHMENT_NAME,
                        ATTACHMENT_CONTENTTYPE = data.ATTACHMENT_CONTENTTYPE,
                        //ATTACHMENT=data.ATTACHMENT,
                        PRESIDENT_ID = data.PRESIDENT_ID,
                        CREATED_BY_NAME = data.aLUFirstName + " " + data.aLULastName,
                        CREATED_DATE = data.CREATED_DATE,
                        MODIFIED_BY_NAME = data.aLU1FirstName + " " + data.aLU1LastName,
                        MODIFIED_DATE = data.MODIFIED_DATE,
                        PRESIDENT_NAME = data.PRESIDENT_NAME,
                        INITIATOR_REMARKS = data.INITIATOR_REMARKS
                    });
                }
            }

            var objAppTRN = (from data in _pdDBContext.CM_PRESIDENTMSGAPP_TRN
                             join aLU in _pdDBContext.ADLOGINUSER on data.CREATED_BY equals aLU.ADEMPCODE into tempLoginUser
                             from aLU in tempLoginUser.DefaultIfEmpty()
                             join aLU1 in _pdDBContext.ADLOGINUSER on data.APPROVED_BY equals aLU1.ADEMPCODE into tempLoginUser1
                             from aLU1 in tempLoginUser1.DefaultIfEmpty()
                             join cPT in _pdDBContext.CM_PRESIDENTMSG_TRN on data.PRESIDENTMSG_ID equals cPT.PRESIDENTMSG_ID into tempPresident
                             from cPT in tempPresident.DefaultIfEmpty()
                             join cPM in _pdDBContext.CM_PRESIDENT_MST on cPT.PRESIDENT_ID equals cPM.PRESIDENT_ID into tempPresident1
                             from cPM in tempPresident1.DefaultIfEmpty()
                             where //data.STATUS == 2
                            objSearchModel.PresidentMsgAppId == 0 ? true : data.MESSAGEAPP_ID == objSearchModel.PresidentMsgAppId
                             orderby data.CREATED_DATE descending
                             select new
                             {
                                 data.MESSAGEAPP_ID,
                                 data.PRESIDENTMSG_ID,
                                 data.STATUS,
                                 data.INITIATOR_REMARKS,
                                 data.CREATED_BY,
                                 data.CREATED_DATE,
                                 aLUFirstName = aLU.FIRSTNAME,
                                 aLULastName = aLU.LASTNAME,
                                 aLU1FirstName = aLU1.FIRSTNAME,
                                 aLU1LastName = aLU1.LASTNAME,
                                 data.APPROVED_BY,
                                 data.APPROVED_DATE,
                                 data.APPAUTH_REMARKS,
                                 cPM.PRESIDENT_TITLE,
                                 cPM.PRESIDENT_NAME,
                                 cPT.BRIEF,
                                 cPT.MESSAGE,
                                 cPT.ATTACHMENT_NAME,
                                 cPT.ATTACHMENT_CONTENTTYPE
                                 //data.CM_PRESIDENTMSG_TRN.ATTACHMENT
                             }
                         ).ToList();

            if (objAppTRN.Count > 0)
            {

                foreach (var data in objAppTRN)
                {
                    pDV.PresidentMsgApp_TRNDTL.Add(new PresidentMsgAPP_TRNViewModel
                    {
                        MESSAGEAPP_ID = data.MESSAGEAPP_ID,
                        PRESIDENTMSG_ID = data.PRESIDENTMSG_ID,
                        STATUS = data.STATUS,
                        INITIATOR_REMARKS = data.INITIATOR_REMARKS,
                        CREATED_BY = data.CREATED_BY,
                        CREATED_DATE = data.CREATED_DATE,
                        INITIATOR_NAME = data.aLUFirstName + " " + data.aLULastName,
                        APPROVED_BY_NAME = data.aLU1FirstName + " " + data.aLU1LastName,
                        APPROVED_BY = data.APPROVED_BY,
                        APPROVED_DATE = data.APPROVED_DATE,
                        APPAUTH_REMARKS = data.APPAUTH_REMARKS,
                        PRESIDENT_TITLE = data.PRESIDENT_TITLE,
                        PRESIDENT_NAME = data.PRESIDENT_NAME,
                        BRIEF = data.BRIEF,
                        MESSAGE = CommonRepository.HtmlToText(data.MESSAGE),
                        ATTACHMENT_NAME = data.ATTACHMENT_NAME,
                        ATTACHMENT_CONTENTTYPE = data.ATTACHMENT_CONTENTTYPE
                        //ATTACHMENT = data.ATTACHMENT,
                    });
                }
            }

            //pDV.PresidentDetail = (List<PresidentDesk_MSTViewModel>)objdata;
            //pDV.President_TRNDTL = (List<PresidentDesk_TRNViewModel>)objTRN;
            //pDV.PresidentMsgApp_TRNDTL = (List<PresidentMsgAPP_TRNViewModel>)objAppTRN;
            return pDV;
        }
        public PresidentDesk_MSTViewModel InsertUpdatePresidentDetail(PresidentDesk_MSTViewModel pDMModel)
        {
            var pMT = _pdDBContext.CM_PRESIDENT_MST.Where(x => x.PRESIDENT_ID == pDMModel.PRESIDENT_ID).FirstOrDefault();

            if (pMT == null)
            {
                pMT = new CM_PRESIDENT_MST();
            }
            var pMTTITLE = _pdDBContext.CM_PRESIDENT_MST.Where(x => x.PRESIDENT_TITLE == pDMModel.PRESIDENT_TITLE && x.STATUS == 1 && x.PRESIDENT_ID != pDMModel.PRESIDENT_ID).FirstOrDefault();
            if (pMTTITLE == null)
            {
                string DefaultDate = "31-Dec-9999";
                //pM.PRESIDENT_ID = pDTModel.PRESIDENT_ID;
                pMT.PRESIDENT_NAME = pDMModel.PRESIDENT_NAME;
                pMT.PRESIDENT_TITLE = pDMModel.PRESIDENT_TITLE;
                pMT.ACTIVE_FROM = DateTime.ParseExact(pDMModel.ACTIVE_FROM, "dd-MMM-yyyy", null);
                pMT.ACTIVE_TO = DateTime.ParseExact(pDMModel.ACTIVE_TO, "dd-MMM-yyyy", null) == null ? Convert.ToDateTime(DefaultDate) : DateTime.ParseExact(pDMModel.ACTIVE_TO, "dd-MMM-yyyy", null);
                if (pMT.PRESIDENT_ID == 0)
                {
                    pMT.CREATED_BY = Convert.ToInt64(pDMModel.CREATED_BY);
                    pMT.CREATED_DATE = DateTime.Now;
                    pMT.STATUS = 1;
                }
                else
                {
                    pMT.MODIFIED_BY = pDMModel.MODIFIED_BY;
                    pMT.MODIFIED_DATE = DateTime.Now;
                    pMT.STATUS = pDMModel.STATUS;
                }
                pMT.REMARKS = pDMModel.REMARKS;
                if (pDMModel.PRESIDENT_PHOTO != null)
                {
                    pMT.PHOTO_NAME = pDMModel.PHOTO_NAME;
                    pMT.PHOTO_CONTENTTYPE = pDMModel.PHOTO_CONTENTTYPE;
                    pMT.PRESIDENT_PHOTO = pDMModel.PRESIDENT_PHOTO;
                }
                //_pdDBContext.CM_PRESIDENTMSG_TRN.Add(pVDTM);
                _pdDBContext.Entry(pMT).State = pDMModel.PRESIDENT_ID == 0 ? EntityState.Added : EntityState.Modified;
                _pdDBContext.SaveChanges();
            }
            else
            {
                pDMModel.ValidationMessage = "Already Exists";
            }

            return pDMModel;
        }

        public Int16 InsertUpdatePresidentDeskMessage(PresidentDesk_TRNViewModel pDTModel)
        {
            Int16 retVal = 0;
            using (var transaction = _pdDBContext.Database.BeginTransaction())
            {
                try
                {
                    CM_PRESIDENTMSG_TRN pVDTM = _pdDBContext.CM_PRESIDENTMSG_TRN.Where(x => x.PRESIDENTMSG_ID == pDTModel.PRESIDENTMSG_ID).FirstOrDefault();


                    if (pVDTM == null)
                    {
                        pVDTM = new CM_PRESIDENTMSG_TRN();

                    }
                    pVDTM.PRESIDENT_ID = pDTModel.PRESIDENT_ID;
                    pVDTM.BRIEF = pDTModel.BRIEF;
                    pVDTM.MESSAGE = CommonRepository.TextToHtml(pDTModel.MESSAGE);
                    pVDTM.VALID_FROM = DateTime.ParseExact(pDTModel.VALID_FROM, "dd-MMM-yyyy", null);
                    pVDTM.VALID_TO = DateTime.ParseExact(pDTModel.VALID_TO, "dd-MMM-yyyy", null);
                    if (pVDTM.PRESIDENTMSG_ID == 0)
                    {
                        pVDTM.CREATED_BY = Convert.ToInt64(pDTModel.CREATED_BY);
                        pVDTM.CREATED_DATE = DateTime.Now;
                        pVDTM.STATUS = 2;
                    }
                    else
                    {
                        pVDTM.MODIFIED_BY = pDTModel.MODIFIED_BY;
                        pVDTM.MODIFIED_DATE = DateTime.Now;
                        pVDTM.STATUS = pDTModel.STATUS;
                    }
                    pVDTM.SYKIID = _pdDBContext.SYKI.Where(s => s.ACTIVE == 1).Single().SYKIID;
                    if (pDTModel.ATTACHMENT != null)
                    {
                        pVDTM.ATTACHMENT_NAME = pDTModel.ATTACHMENT_NAME;
                        pVDTM.ATTACHMENT_CONTENTTYPE = pDTModel.ATTACHMENT_CONTENTTYPE;
                        pVDTM.ATTACHMENT = pDTModel.ATTACHMENT;
                    }
                    //_pdDBContext.CM_PRESIDENTMSG_TRN.Add(pVDTM);
                    _pdDBContext.Entry(pVDTM).State = pVDTM.PRESIDENTMSG_ID == 0 ? EntityState.Added : EntityState.Modified;                   
                    _pdDBContext.SaveChanges();

                    PresidentMsgAPP_TRNViewModel pMSGAPPTRN = new PresidentMsgAPP_TRNViewModel();
                    pMSGAPPTRN.PRESIDENTMSG_ID = pVDTM.PRESIDENTMSG_ID;
                    pMSGAPPTRN.CREATED_BY = pVDTM.CREATED_BY;
                    pMSGAPPTRN.CREATED_DATE = pVDTM.CREATED_DATE;
                    pMSGAPPTRN.INITIATOR_REMARKS = pDTModel.INITIATOR_REMARKS;
                    InsertUpdatePresidentMsgApproval(pMSGAPPTRN);
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

        public PresidentMsgAPP_TRNViewModel InsertUpdatePresidentMsgApproval(PresidentMsgAPP_TRNViewModel pDMAPPModel)
        {
            try
            {

                CM_PRESIDENTMSGAPP_TRN pMSGAPPTRN = _pdDBContext.CM_PRESIDENTMSGAPP_TRN.Where(x => x.PRESIDENTMSG_ID == pDMAPPModel.PRESIDENTMSG_ID).FirstOrDefault();
                CM_PRESIDENTMSG_TRN pVDTM = _pdDBContext.CM_PRESIDENTMSG_TRN.Where(x => x.PRESIDENTMSG_ID == pDMAPPModel.PRESIDENTMSG_ID).FirstOrDefault();
                if (pMSGAPPTRN == null)
                {
                    pMSGAPPTRN = new CM_PRESIDENTMSGAPP_TRN();
                }
                pMSGAPPTRN.PRESIDENTMSG_ID = Convert.ToInt64(pDMAPPModel.PRESIDENTMSG_ID);
                pMSGAPPTRN.INITIATOR_REMARKS = Convert.ToString(pDMAPPModel.INITIATOR_REMARKS);
                if (pMSGAPPTRN.MESSAGEAPP_ID == 0)
                {
                    pMSGAPPTRN.CREATED_BY = Convert.ToInt64(pDMAPPModel.CREATED_BY);
                    pMSGAPPTRN.CREATED_DATE = pDMAPPModel.CREATED_DATE;
                    pMSGAPPTRN.STATUS = 1;
                }
                else if (pDMAPPModel.STATUS > 1)
                {
                    pMSGAPPTRN.APPROVED_BY = pDMAPPModel.APPROVED_BY;
                    pMSGAPPTRN.APPROVED_DATE = DateTime.Now;
                    pMSGAPPTRN.APPAUTH_REMARKS = pDMAPPModel.APPAUTH_REMARKS;
                    pMSGAPPTRN.STATUS = pDMAPPModel.STATUS;
                    if (pDMAPPModel.STATUS == 2)
                    {
                        pVDTM.STATUS = 1;
                    }
                    else if (pDMAPPModel.STATUS == 4)
                    {
                        pVDTM.STATUS = 3;
                    }

                }
                _pdDBContext.Entry(pMSGAPPTRN).State = pMSGAPPTRN.MESSAGEAPP_ID == 0 ? EntityState.Added : EntityState.Modified;
                _pdDBContext.SaveChanges();
                return pDMAPPModel;
            }
            catch
            {
                throw;
            }
        }

        public FileViewModel GetFileForDownload(Int64 id)
        {
            FileViewModel flvm = new FileViewModel();
            CM_PRESIDENTMSG_TRN CCPT;
            CCPT = _pdDBContext.CM_PRESIDENTMSG_TRN.Where(c => c.PRESIDENTMSG_ID == id).SingleOrDefault();
            if (CCPT != null)
            {
                flvm.FileName = CCPT.ATTACHMENT_NAME;
                flvm.FileContentType = CCPT.ATTACHMENT_CONTENTTYPE;
                flvm.File = CCPT.ATTACHMENT;
            }
            return flvm;
        }
        public PresidentDesk_TRNViewModel DeleteDocument(Int64 id)
        {
            PresidentDesk_TRNViewModel pDVM = new PresidentDesk_TRNViewModel();
            CM_PRESIDENTMSG_TRN CPT;
            CPT = _pdDBContext.CM_PRESIDENTMSG_TRN.Where(c => c.PRESIDENTMSG_ID == id).SingleOrDefault();
            if (CPT != null)
            {


                CPT.ATTACHMENT = null;
                CPT.ATTACHMENT_CONTENTTYPE = null;
                CPT.ATTACHMENT_NAME = null;

                _pdDBContext.Entry(CPT).State = EntityState.Modified;
                _pdDBContext.SaveChanges();

                pDVM.ATTACHMENT_NAME = CPT.ATTACHMENT_NAME;
            }
            return pDVM;
        }
    }
}
