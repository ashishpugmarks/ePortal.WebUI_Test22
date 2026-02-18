using System;
using System.Collections.Generic;
using System.Linq;
using ePortal.DomainClasses;
using ePortal.ViewModels;
using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;
using System.Data;
using System.Threading.Tasks;
using ePortal.Shared;

namespace ePortal.Application.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly AnnouncementRepository _objAnnouncementRepositry;
        private readonly CommonRepository _CommonRepo;
        private readonly IApiClient _apiClient;

        public AnnouncementService(AnnouncementRepository objAnnouncementRepositry, CommonRepository CommonRepo, IApiClient apiClient)
        {
            _objAnnouncementRepositry = objAnnouncementRepositry;
            _CommonRepo = CommonRepo;
            _apiClient = apiClient;
        }
        // Content Process Master
        #region Content Process Master
        public IEnumerable<ProcessMstViewModel> BindContentProcess()
        {
            List<ProcessMstViewModel> items = new List<ProcessMstViewModel>();
            items = _objAnnouncementRepositry.BindContentProcess().ToList();
            var res = items.Where(x => x.PROCESSID == 6 || x.PROCESSID == 8).ToList(); // 6 : Announcement  8 : cir
            if (res != null)
            {
                items.Clear();
                foreach (ProcessMstViewModel obj in res)
                {
                    items.Add(new ProcessMstViewModel
                    {
                        PROCESSID = obj.PROCESSID,
                        PROCESS_NAME = obj.PROCESS_NAME
                    });

                }
                //items.Add(new ProcessMstViewModel
                //{
                //    PROCESSID = res.PROCESSID,
                //    PROCESS_NAME = res.PROCESS_NAME
                //});
            }
            return items;
        }

        public ProcessMstViewModel UpdateProcess_Mst(ProcessMstViewModel PVM)
        {
            return _objAnnouncementRepositry.UpdateProcess_Mst(PVM);
        }

        public ProcessMstViewModel GetEditProcess_MstById(int id)
        {
            return _objAnnouncementRepositry.GetEditProcess_MstById(id);
        }

        public IEnumerable<ProcessMstViewModel> GetProcess_Mst_List()
        {
            return _objAnnouncementRepositry.GetProcess_Mst_List();
        }

        public ProcessMstViewModel SaveProcess_Mst(ProcessMstViewModel PVM)
        {
            return _objAnnouncementRepositry.SaveProcess_Mst(PVM);
        }
        #endregion

        //// Process Attachment Trn
        //#region Process Attachment Trn
        public Int16 SaveProcessAttachment_Trn(AnnouncementMasterViewModel AVM)
        {
            return _objAnnouncementRepositry.SaveProcessAttachment_Trn(AVM);
        }

        public Int16 UpdateProcessAttachment_Trn(AnnouncementMasterViewModel AVM)
        {
            return _objAnnouncementRepositry.UpdateProcessAttachment_Trn(AVM);
        }

        //public AnnouncementMasterViewModel DeactiveAnnouncement(AnnouncementMasterViewModel AVM)
        //{
        //    return _objAnnouncementRepositry.DeactiveAnnouncement(AVM);
        //}
        public short DeactiveAnnouncement(AnnouncementMasterViewModel AVM)
        {
            return _objAnnouncementRepositry.DeactiveAnnouncement(AVM);
        }
        //public AnnouncementMasterViewModel CancelAnnouncement(AnnouncementMasterViewModel AVM)
        //{
        //    return _objAnnouncementRepositry.CancelAnnouncement(AVM);

        //}
        public short CancelAnnouncement(AnnouncementMasterViewModel AVM)
        {
            return _objAnnouncementRepositry.CancelAnnouncement(AVM);
        }


        public List<AnnouncementMasterViewModel> GetAnnouncementMasterList(SearchViewModel SVM, Int64 UserId)
        {
            List<AnnouncementMasterViewModel> AnnouncementList = _objAnnouncementRepositry.GetAnnouncementMasterList(UserId);
            //List<AnnouncementMasterViewModel> iList = new List<AnnouncementMasterViewModel>();

            if (SVM.Status == null && SVM.FromDate == null && SVM.ToDate == null)
            {
                return AnnouncementList;
            }
            else if (SVM.Status == -1)
            {
                AnnouncementList = AnnouncementList.Where(x => x.SUBJECT == (String.IsNullOrEmpty(SVM.Name) ? x.SUBJECT : SVM.Name) && (Convert.ToDateTime(x.START_DATE) >= (SVM.FromDate == null ? Convert.ToDateTime(x.START_DATE) : SVM.FromDate) && Convert.ToDateTime(x.END_DATE) <= (SVM.ToDate == null ? Convert.ToDateTime(x.END_DATE) : SVM.ToDate))).ToList();
            }

            else
            {
                AnnouncementList = AnnouncementList.Where(x => x.SUBJECT == (String.IsNullOrEmpty(SVM.Name) ? x.SUBJECT : SVM.Name) && (Convert.ToDateTime(x.START_DATE) >= (SVM.FromDate == null ? Convert.ToDateTime(x.START_DATE) : SVM.FromDate) && Convert.ToDateTime(x.END_DATE) <= (SVM.ToDate == null ? Convert.ToDateTime(x.END_DATE) : SVM.ToDate)) && x.STATUS == SVM.Status).ToList();
            }
            return AnnouncementList;
        }

        public List<AnnouncementMasterViewModel> GetAnnouncementArchiveList(SearchViewModel SVM, Int64 UserId)
        {
            List<AnnouncementMasterViewModel> AnnouncementList = _objAnnouncementRepositry.GetAnnouncementArchiveList(UserId);
            //List<AnnouncementMasterViewModel> iList = new List<AnnouncementMasterViewModel>();

            if (SVM.Status == null && SVM.FromDate == null && SVM.ToDate == null)
            {
                return AnnouncementList;
            }
            else if (SVM.Status == -1)
            {
                AnnouncementList = AnnouncementList.Where(x => x.SUBJECT == (String.IsNullOrEmpty(SVM.Name) ? x.SUBJECT : SVM.Name) && (Convert.ToDateTime(x.START_DATE) >= (SVM.FromDate == null ? Convert.ToDateTime(x.START_DATE) : SVM.FromDate) && Convert.ToDateTime(x.END_DATE) <= (SVM.ToDate == null ? Convert.ToDateTime(x.END_DATE) : SVM.ToDate))).ToList();
            }

            else
            {
                AnnouncementList = AnnouncementList.Where(x => x.SUBJECT == (String.IsNullOrEmpty(SVM.Name) ? x.SUBJECT : SVM.Name) && (Convert.ToDateTime(x.START_DATE) >= (SVM.FromDate == null ? Convert.ToDateTime(x.START_DATE) : SVM.FromDate) && Convert.ToDateTime(x.END_DATE) <= (SVM.ToDate == null ? Convert.ToDateTime(x.END_DATE) : SVM.ToDate)) && x.STATUS == SVM.Status).ToList();
            }
            return AnnouncementList;
        }

        public AnnouncementMasterViewModel GetEditProcessAttachmentById(int id)
        {
            return _objAnnouncementRepositry.GetEditProcessAttachmentById(id);
        }

        public AnnouncementMasterViewModel GetAnnouncementDetails(int id)
        {
            return _objAnnouncementRepositry.GetAnnouncementDetails(id);
        }

        public IEnumerable<ADFUNCTIONALDESIGNATION> Bind_ADFunctionalDesignation()
        {
            return _objAnnouncementRepositry.Bind_ADFunctionalDesignation();
        }
        public IEnumerable<ADDESIGNATION> Bind_ADDesignation()
        {
            return _objAnnouncementRepositry.Bind_ADDesignation();
        }

        public IEnumerable<SYSITE> Bind_SYSite()
        {
            return _objAnnouncementRepositry.Bind_SYSite();
        }

        public AnnouncementMasterViewModel DeleteAnnouncementFile(long id, string type)
        {
            return _objAnnouncementRepositry.DeleteAnnouncementFile(id, type);
        }

        //#endregion

        //#region Attachment Approval
        public IEnumerable<AnnouncementApprovalViewModel> GetAnnouncementApprovalAuthorityList()
        {
            return _objAnnouncementRepositry.GetAnnouncementApprovalAuthorityList();
        }

        public AnnouncementApprovalViewModel GetAnnouncementApprovalAuthorityById(int id)
        {
            return _objAnnouncementRepositry.GetAnnouncementApprovalAuthorityById(id);
        }
        //public async Task<AnnouncementApprovalViewModel> GetAnnouncementApprovalAuthorityById(int id)
        //{
        //    try
        //    {
        //        var response = await _apiClient.PostAsync<AnnouncementApprovalViewModel>(
        //            APIMapper.Announcement.GetAnnouncementApprovalAuthorityById,
        //            new { id }
        //        );

        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new AnnouncementApprovalViewModel();
        //    }
        //}

        ////public IEnumerable<EmployeeViewModel> BindEmployeeBy_Designation(string[] parametervalue)
        ////{
        ////    return objAnnouncementRepositry.BindAllEmploye(parametervalue);
        ////}

        public String IsEmployeeActive(Int64 Empcode)
        {
            return _objAnnouncementRepositry.IsEmployeeActive(Empcode);
        }

        public AnnouncementApprovalViewModel SaveHRApproval(AnnouncementApprovalViewModel AAVM)
        {
            return _objAnnouncementRepositry.SaveHRApproval(AAVM);
        }
        public IEnumerable<AnnouncementApprovalViewModel> GetAnnouncementApproval_List(Int64 UserId)
        {
            return _objAnnouncementRepositry.GetAnnouncementApprovalList(UserId);
        }
        //public async Task<IEnumerable<AnnouncementApprovalViewModel>> GetAnnouncementApproval_List(Int64 UserId)
        //{

        //var response = await _apiClient.PostAsync<AnnouncementApprovalViewModel>(APIMapper.Announcement.GetAnnouncementApprovalList,
        //    new { UserId });

        //return (IEnumerable<AnnouncementApprovalViewModel>)response;
        //}
        public AnnouncementApprovalViewModel UpdateStatus(AnnouncementApprovalViewModel AAVM)
        {
            return _objAnnouncementRepositry.UpdateStatus(AAVM);
        }
        //public async Task<AnnouncementApprovalViewModel> UpdateStatus(AnnouncementApprovalViewModel AAVM)
        //{
        //    try
        //    {
        //        var response = await _apiClient.PostAsync<AnnouncementApprovalViewModel>(
        //            APIMapper.Announcement.UpdateStatus,
        //            new { AAVM }
        //        );

        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new AnnouncementApprovalViewModel();
        //    }
        //}
        public FileViewModel GetFileForDownload(long AttachmentId, string Attachment)
        {
            return _objAnnouncementRepositry.GetFileForDownload(AttachmentId, Attachment);
        }
        //public async Task<FileViewModel> GetFileForDownload(long attachmentId, string attachment)
        //{
        //    try
        //    {
        //        var response = await _apiClient.PostAsync<FileViewModel>(
        //            APIMapper.Announcement.GetFileForDownload,
        //            new { attachmentId, attachment }
        //        );

        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new FileViewModel();
        //    }
        //}

        //#endregion

        //#region Communication Proess
        public List<CommunicationViewModel> CommunicationRequestList(long UserId)
        {
            return _objAnnouncementRepositry.CommunicationRequestList(UserId);
        }

        public IEnumerable<CommCategoryViewModel> BindCommCategory()
        {
            return _objAnnouncementRepositry.BindCommCategory();
        }

        public CommCategoryViewModel GetCommCategoryById(long id)
        {
            return _objAnnouncementRepositry.GetCommCategoryById(id);
        }

        public IEnumerable<CommMailTypeViewModel> BindCommMailType()
        {
            return _objAnnouncementRepositry.BindCommMailType();
        }

        public Tuple<short, long> SaveCommunicationRequest(CommunicationViewModel model)
        {
            return _objAnnouncementRepositry.SaveCommunicationRequest(model);
        }

        public Tuple<short, long> FinalSubmitRequest(CommunicationViewModel model)
        {
            Tuple<short, long> _tuple = _objAnnouncementRepositry.FinalSubmitRequest(model);
            if (_tuple.Item1 == 1 && _tuple.Item2 > 0 && model.IsFinalSubmit == 1)
            {
                //SendMailByRequestor(_tuple.Item2);
                //Added by TTL :: CR6879 
                SendMailToNextApprovalAuthority(_tuple.Item2);

            }
            return _tuple;
        }

        //Added by TTL :: CR6879
        #region Send mail next approval authority
        public short SendMailToNextApprovalAuthority(long HeaderId)
        {
            short retVal = 0;

            CommunicationViewModel PHVM = _objAnnouncementRepositry.GetCommunicationDetails(HeaderId);
            if (PHVM.commAppHis.Count > 0)
            {
                CommunicationAppHisViewModel Auth_obj = PHVM.commAppHis.Where(a => a.APPROVAL_STATUS == 0).FirstOrDefault();
                if (Auth_obj != null)
                {
                    string _EmailTo = string.Empty;
                    string _EmailCc = string.Empty;

                    _EmailTo = Auth_obj.APP_EMAIL;
                    _EmailCc = PHVM.Emp_Detail.EMail_Id;

                    if (!string.IsNullOrEmpty(_EmailTo))
                    {
                        commanEmail sendMail = new commanEmail();
                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        if (serverpath.isTestServer())
                            sendMail.MailTo = serverpath.getTestEMail();
                        else
                            sendMail.MailTo = _EmailTo;
                            sendMail.MailCc = _EmailCc;

                        string struid = Auth_obj.ADEMPCODE.ToString();
                        string strid = HeaderId.ToString();
                        string strReqType = PHVM.REQUEST_TYPE == 2 ? "Emergency" : "Normal";
                        string page = "CommunicationApproval";
                        string ADEMPNAME = Auth_obj.ADEMPNAME;
                        string dearName = string.IsNullOrWhiteSpace(ADEMPNAME) ? "" : ADEMPNAME.Split('-')[0].Trim();

                        string strSubject = "Communication Request from - " + PHVM.Emp_Detail._EName + ", Employee Code - " + PHVM.Emp_Detail._ECode;
                        string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                         "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Communication Request from " + PHVM.Emp_Detail._EName + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +
                                          "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + dearName + " San </br></br>" + PHVM.Emp_Detail._EName + " San has raised a Communication Approval Request in Employee Portal. Below are the details :</td></tr>" +
                                         "<tr><td width=125 height=22 valign=top>Subject :</td><td width=389 valign=top>" + PHVM.SUBJECT + "</td></tr>" +
                                         "<tr><td width=125 height=22 valign=top>Request Type :</td><td width=389 valign=top>" + strReqType + "</td></tr>" +
                                         "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "/Login/EmailApproval/?Wid=" + struid + "&C=Announcement&A=" + page + "&Tid=" + strid + " > Employee Portal</a> link to approve the request.</td></tr>" +
                                         "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";


                        sendMail.MailSubject = strSubject;
                        sendMail.MailBody = strBody;
                        try
                        {
                            bool status = sendMail.Send();
                        }
                        catch (Exception ex)
                        {
                            retVal = -1;
                        }
                        finally
                        {
                            retVal = 1;
                        }
                    }
                }
            }
            return retVal;
        }
        #endregion
        //Ended by TTL :: CR6879 
        public Tuple<short, List<CommunicationDtlViewModel>> SaveCommAttachment(long addedBy, long CommheaderId, List<CommunicationDtlViewModel> modelList)
        {
            List<CommunicationDtlViewModel> iList = new List<CommunicationDtlViewModel>();
            short retVal = _objAnnouncementRepositry.SaveCommAttachment(addedBy, CommheaderId, modelList);
            if (retVal == 1)
            {
                iList = _objAnnouncementRepositry.GetCommAttachmentDetail(CommheaderId);
            }
            Tuple<short, List<CommunicationDtlViewModel>> _tuple = new Tuple<short, List<CommunicationDtlViewModel>>(retVal, iList);
            return _tuple;
        }

        public List<CommunicationAppSeqViewModel> GetDefaultAuthority(long loginUser, long categoryId, Employee_Details _Employee_Details)
        {
            short APP_REQ_TILL = 0; bool BrandCommReq = true;
            CommCategoryViewModel cateDetail = _objAnnouncementRepositry.GetCommCategoryById(categoryId);
            if (cateDetail != null)
            {
                APP_REQ_TILL = cateDetail.APP_REQUIRED_TILL;
            }

            List<CommunicationAppSeqViewModel> AuthList = new List<CommunicationAppSeqViewModel>();
            try
            {
                UserApprovalAuthority Obj = _CommonRepo.CheckApprovalAuthority(loginUser);
                if (Obj != null)
                {
                    if (Obj.SectionManager != 0 && Obj.SectionManager != null)
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.SectionManager));
                        AuthList.Add(new CommunicationAppSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = string.IsNullOrEmpty(Emp_Dtl._FnDesig) ? Emp_Dtl._Desig : Emp_Dtl._FnDesig,
                            APP_SEQ = Convert.ToInt16(AuthList.Count + 1),
                            APPTYPE = 1, // Recommendation Authority
                            FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                        });
                    }
                    if (Obj.DepartmentManager != 0 && Obj.DepartmentManager != null && Obj.DepartmentManager != loginUser)
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.DepartmentManager));
                        AuthList.Add(new CommunicationAppSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = string.IsNullOrEmpty(Emp_Dtl._FnDesig) ? Emp_Dtl._Desig : Emp_Dtl._FnDesig,
                            APP_SEQ = Convert.ToInt16(AuthList.Count + 1),
                            APPTYPE = 1,// Recommendation Authority
                            FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                        });
                    }
                    if (Obj.Coordinator != 0 && Obj.Coordinator != null && Obj.Coordinator != loginUser && _Employee_Details._FnDesigId != 3 && categoryId == 1)  //// - categoryId - 1 => Policy
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.Coordinator));
                        AuthList.Add(new CommunicationAppSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = string.IsNullOrEmpty(Emp_Dtl._FnDesig) ? Emp_Dtl._Desig : Emp_Dtl._FnDesig,
                            APP_SEQ = Convert.ToInt16(AuthList.Count + 1),
                            APPTYPE = 1,// Recommendation Authority
                            FNDESID = 2,
                        });
                    }
                    if (Obj.DivisionHead != 0 && Obj.DivisionHead != null && Obj.DivisionHead != loginUser)
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.DivisionHead));
                        AuthList.Add(new CommunicationAppSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = string.IsNullOrEmpty(Emp_Dtl._FnDesig) ? Emp_Dtl._Desig : Emp_Dtl._FnDesig,
                            APP_SEQ = Convert.ToInt16(AuthList.Count + 1),
                            APPTYPE = 1,// Recommendation Authority
                            FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                        });
                    }
                    if (Obj.EXECoordinator != 0 && Obj.EXECoordinator != null && Obj.EXECoordinator != loginUser && categoryId == 1) //// - categoryId - 1 => Policy
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.EXECoordinator));
                        AuthList.Add(new CommunicationAppSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = string.IsNullOrEmpty(Emp_Dtl._FnDesig) ? Emp_Dtl._Desig : Emp_Dtl._FnDesig,
                            APP_SEQ = Convert.ToInt16(AuthList.Count + 1),
                            APPTYPE = 1,// Recommendation Authority
                            FNDESID = 3,
                        });
                    }
                    if (BrandCommReq)
                    {
                        AuthList.Add(new CommunicationAppSeqViewModel
                        {
                            ADEMPCODE = 1,
                            ADEMPNAME = "Corporate Communication Team",
                            ADDESIGNATION = "Corporate Communication",
                            APP_SEQ = Convert.ToInt16(AuthList.Count + 1),
                            APPTYPE = 2,// Corporate Communication Team
                            FNDESID = 0,
                        });
                    }
                    if (Obj.OperationHead != 0 && Obj.OperationHead != null && Obj.OperationHead != loginUser)
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.OperationHead));
                        AuthList.Add(new CommunicationAppSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = string.IsNullOrEmpty(Emp_Dtl._FnDesig) ? Emp_Dtl._Desig : Emp_Dtl._FnDesig,
                            APP_SEQ = Convert.ToInt16(AuthList.Count + 1),
                            APPTYPE = 3, // Approval Authority
                            FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                        });
                    }

                    if (APP_REQ_TILL == 6)
                    {
                        if (Obj.Director != 0 && Obj.Director != null && Obj.Director != loginUser)
                        {
                            short atype;
                            Employee_Details Emp_Dtl = _CommonRepo.GetDirectorDetailById(Convert.ToInt64(Obj.Director), Convert.ToInt64(Obj.OperationId), out atype);
                            AuthList.Add(new CommunicationAppSeqViewModel
                            {
                                ADEMPCODE = Emp_Dtl._ECode,
                                ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                                ADDESIGNATION = string.IsNullOrEmpty(Emp_Dtl._FnDesig) ? Emp_Dtl._Desig : Emp_Dtl._FnDesig,
                                APP_SEQ = Convert.ToInt16(AuthList.Count + 1),
                                APPTYPE = 3, // Approval Authority
                                FNDESID = 5,
                            });
                        }
                    }
                    if (APP_REQ_TILL == 7)
                    {
                        if (Obj.Director2 != 0 && Obj.Director2 != null && Obj.Director2 != loginUser)
                        {
                            short atype;
                            Employee_Details Emp_Dtl = _CommonRepo.GetDirectorDetailById(Convert.ToInt64(Obj.Director2), Convert.ToInt64(Obj.OperationId), out atype);
                            AuthList.Add(new CommunicationAppSeqViewModel
                            {
                                ADEMPCODE = Emp_Dtl._ECode,
                                ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                                ADDESIGNATION = string.IsNullOrEmpty(Emp_Dtl._FnDesig) ? Emp_Dtl._Desig : Emp_Dtl._FnDesig,
                                APP_SEQ = Convert.ToInt16(AuthList.Count + 1),
                                APPTYPE = 3, // Approval Authority
                                FNDESID = 6,
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AuthList = new List<CommunicationAppSeqViewModel>();
            }
            AuthList = AuthList.OrderBy(n => n.APP_SEQ).OrderBy(m => m.APPTYPE).ToList();
            for (Int16 i = 0; i <= AuthList.Count - 1; i++)
            {
                AuthList[i].APP_SEQ = Convert.ToInt16(i + 1);
            }
            return AuthList;
        }

        public Tuple<short, List<CommunicationDtlViewModel>> DeleteAttachment(string fileName, string docType, long commHid)
        {
            List<CommunicationDtlViewModel> iList = new List<CommunicationDtlViewModel>();
            short retVal = _objAnnouncementRepositry.DeleteAttachment(fileName, docType, commHid);
            if (retVal == 1)
            {
                iList = _objAnnouncementRepositry.GetCommAttachmentDetail(commHid);
            }
            Tuple<short, List<CommunicationDtlViewModel>> _tuple = new Tuple<short, List<CommunicationDtlViewModel>>(retVal, iList);
            return _tuple;
        }

        public CommunicationViewModel GetCommunicationDetails(long id)
        {
            return _objAnnouncementRepositry.GetCommunicationDetails(id);
        }

        public List<CommunicationDtlViewModel> GetCommunicationAttachments(long id)
        {
            return _objAnnouncementRepositry.GetCommAttachmentDetail(id);
        }

        public List<CommunicationViewModel> CommunicationApprovalList(long UserId)
        {
            return _objAnnouncementRepositry.CommunicationApprovalList(UserId);
        }

        public List<CommunicationViewModel> CommunicationApprovalHistory(long UserId)
        {
            return _objAnnouncementRepositry.CommunicationApprovalHistory(UserId);
        }

        public short CommunicationApproval(CommunicationAppHisViewModel PHVM, Employee_Details emp_dtl, List<CommunicationDtlViewModel> commDetailList)
        {
            short retVal = _objAnnouncementRepositry.CommunicationApproval(PHVM, commDetailList);
            if (retVal == 1)
            {
                SendMailByApprovalAuthority(PHVM.COMMUNICATIONID, PHVM.APPROVAL_STATUS, emp_dtl);
            }
            return retVal;
        }

        public short SendMailByApprovalAuthority(long HeaderId, short approvalStatus, Employee_Details employeeDetails)
        {
            short retVal = 0;
            try
            {
                string RequestStatus = approvalStatus == 1 ? "Approved" : approvalStatus == 2 ? "Send back" : approvalStatus == 3 ? "Rejected" : "";

                #region Send mail next approval authority
                CommunicationViewModel PHVM = _objAnnouncementRepositry.GetCommunicationDetails(HeaderId);
                if (PHVM.commAppHis.Count > 0 && approvalStatus == 1)
                {
                    CommunicationAppHisViewModel Auth_obj = PHVM.commAppHis.Where(a => a.APPROVAL_STATUS == 0).FirstOrDefault();
                    if (Auth_obj != null)
                    {
                        string _EmailTo = string.Empty;
                        _EmailTo = Auth_obj.APP_EMAIL;
                        if (Auth_obj.ADEMPCODE == 1) // Corporate Comm.
                        {
                            _EmailTo = _CommonRepo.GetParameterValue("CORPORATE_COMMUNICATION_TOMAILIDS");
                        }

                        if (!string.IsNullOrEmpty(_EmailTo))
                        {
                            commanEmail sendMail = new commanEmail();
                            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                            if (serverpath.isTestServer())
                                sendMail.MailTo = serverpath.getTestEMail();
                            else
                                sendMail.MailTo = _EmailTo;
                            //sendMail.MailTo = "vishal.saini@honda.hmsi.in, vaibhav.sharma@honda.hmsi.in";

                            string struid = Auth_obj.ADEMPCODE.ToString();
                            string strid = HeaderId.ToString();
                            string strReqType = PHVM.REQUEST_TYPE == 2 ? "Emergency" : "Normal";
                            string page = Auth_obj.ADEMPCODE == 1 ? "EditBrandCommunicationApproval" : "CommunicationApproval";
                            string dearName = Auth_obj.ADEMPCODE == 1 ? "Corporate Communication Team" : Auth_obj.ADEMPNAME + " San, ";

                            string strSubject = "Communication Request from - " + PHVM.Emp_Detail._EName + ", Employee Code - " + PHVM.Emp_Detail._ECode;
                            string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                             "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Communication Request from " + PHVM.Emp_Detail._EName + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +
                                              "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + dearName + " </br></br>" + PHVM.Emp_Detail._EName + " San has raised a Communication Approval Request in Employee Portal. Below are the details :</td></tr>" +
                                             "<tr><td width=125 height=22 valign=top>Subject :</td><td width=389 valign=top>" + PHVM.SUBJECT + "</td></tr>" +
                                             "<tr><td width=125 height=22 valign=top>Request Type :</td><td width=389 valign=top>" + strReqType + "</td></tr>" +
                                             "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "/Login/EmailApproval/?Wid=" + struid + "&C=Announcement&A=" + page + "&Tid=" + strid + " > Employee Portal</a> link to approve the request.</td></tr>" +
                                             //"<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "/Login/EmailApproval/?Wid=" + struid + "&C=Announcement&A=ArchivePolicyCircularDetails&Tid=" + strid + " > Employee Portal</a> link to view the request.</td></tr>" +
                                             "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                            sendMail.MailSubject = strSubject;
                            sendMail.MailBody = strBody;
                            try
                            {
                                bool status = sendMail.Send();
                            }
                            catch (Exception ex)
                            {
                                retVal = -1;
                            }
                            finally
                            {
                                retVal = 1;
                            }
                        }
                    }
                }
                #endregion

                #region Send mail for requestor
                if (!string.IsNullOrEmpty(PHVM.Emp_Detail._EmailId))
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    sendMail.MailTo = PHVM.Emp_Detail._EmailId;
                    string strReqType = PHVM.REQUEST_TYPE == 2 ? "Emergency" : "Normal";

                    string strSubject = "Communication Approval Status - " + RequestStatus;
                    string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Communication Approval Status " + RequestStatus + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +

                                     "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                                     "<tr><td valign=top colspan =2>Your Communication request has been <b>" + RequestStatus + "</b> by " + employeeDetails.Employee_Name + " San. The request details are as follows:</td></tr>" +
                                     "<tr><td width=125 height=22 valign=top>Subject : </td><td width=389 valign=top>" + PHVM.SUBJECT + "</td></tr>" +
                                     "<tr><td width=125 height=22 valign=top>Request Type :</td><td width=389 valign=top>" + strReqType + "</td></tr>" +
                                     "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "Login/Index> Employee Portal</a> to view the approval history.</td></tr>" +
                                     "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    try
                    {
                        bool status = sendMail.Send();
                    }
                    catch (Exception ex)
                    {
                        retVal = -1;
                    }
                    finally
                    {
                        retVal = 1;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }
         
        
        public Tuple<short, long> UpdateCommunicationRequest(CommunicationViewModel model)
        {
            return _objAnnouncementRepositry.SaveCommunicationRequest(model);
        }

        public List<CommunicationViewModel> ArchiveRequestList(int type, CommunicationViewModel model)
        {
            return _objAnnouncementRepositry.ArchiveRequestList(type, model);

        }

        public short CancelCommunicationRequest(long id, long userID)
        {
            return _objAnnouncementRepositry.CancelCommunicationRequest(id, userID);
        }

        public short DeactivateCommunicationRequest(long id, long userID)
        {
            return _objAnnouncementRepositry.DeactivateCommunicationRequest(id, userID);
        }

        public List<CommunicationViewModel> SelfPendingCommunicationList(long UserId)
        {
            return _objAnnouncementRepositry.SelfPendingCommunicationList(UserId);
        }

        public short InserViewData(long id, long userID)
        {
            return _objAnnouncementRepositry.InserViewData(id, userID);
        }

        public List<CommMailTypeViewModel> GetCommunicationTypeList()
        {
            return _objAnnouncementRepositry.GetCommunicationTypeList();
        }

        public CommMailTypeViewModel GetCommunicationTypeDtlById(long id)
        {
            return _objAnnouncementRepositry.GetCommunicationTypeDtlById(id);
        }

        public short SaveCommunicationType(CommMailTypeViewModel AOVM)
        {
            return _objAnnouncementRepositry.SaveCommunicationType(AOVM);
        }

        public List<Comm_Op_Div_DepViewModel> BindOperation(long typeId)
        {
            return _objAnnouncementRepositry.BindOperation(typeId);
        }

        public List<Comm_Op_Div_DepViewModel> BindDivision(long typeId, long op_Id)
        {
            return _objAnnouncementRepositry.BindDivision(typeId, op_Id);
        }

        public List<Comm_Op_Div_DepViewModel> BindDepartment(long typeId, long div_Id)
        {
            return _objAnnouncementRepositry.BindDepartment(typeId, div_Id);
        }

        public List<CommunicationViewModel> ValidateArchiveRequestList(long CommID)
        {
            return _objAnnouncementRepositry.ValidateArchiveRequestList(CommID);
        }

        //#endregion
        // Added by aumento for SR87026
        public List<SYKI_DGIT> GetKI()
        {
            return _objAnnouncementRepositry.GetKIByYear(); // No parameters needed, just fetch top 3
        }
        public DataTable _GetCommunicationReport(string startDate, string endDate, string KI)
        {
            return _objAnnouncementRepositry.GetCommunicationReport(startDate, endDate, KI);
        }
        // Added by aumento for SR87026
    }
}
