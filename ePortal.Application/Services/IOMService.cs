using ePortal.ViewModels;
using System.Web;
using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;
using ePortal.Shared;

namespace ePortal.Application.Services
{
    public class IOMService : IIOMService
    {

        private readonly IOMRepository _IOMRepo;
        private readonly CommonRepository _CommonRepo;
        private readonly Tuple<short, long> _retVal_tuple;
        private readonly IFCMNotificationContract _fcmService;

        public IOMService(IOMRepository iomRepo, CommonRepository commonRepo, IFCMNotificationContract fcmService)
        {
            _IOMRepo = iomRepo;
            _CommonRepo = commonRepo;
            _fcmService = fcmService;
            _retVal_tuple = new Tuple<short, long>((short)0, 0L);
        }

        public Employee_Details GetAuthEmpById(long empCode, Employee_Details empDtl)
        {
            return _IOMRepo.GetAuthEmpById(empCode, empDtl);
        }

        public IOMHeaderViewModel GetIOMRequestById(long id)
        {
            return _IOMRepo.GetIOMRequestById(id);
        }

        public short IOMApproval(IOMAppHistoryViewModel IHVM, Employee_Details emp_dtl)
        {
            short retVal = _IOMRepo.IOMApproval(IHVM);
            if (retVal == 1)
            {
                //SendMailByApprovalAuthority(IHVM.IOMID, IHVM.APPROVAL_STATUS, emp_dtl);  //   Commented by Aumento 12072024

                //get process status: if 2 then don't send mail to requester, because it will be sent after document generation from controller //Added by TTL CR5813 (08-Mar-2025)
                IOMHeaderViewModel IOM_Dtl = GetIOMRequestById(IHVM.IOMID);
                SendMailByApprovalAuthority(IHVM, IHVM.APPROVAL_STATUS, emp_dtl, IOM_Dtl.PROCESS_STATUS);   // {IHVM.IOMID to IHVM}  Updated by Aumento 12072024
            }
            return retVal;
        }
        public void SendAnnotatedPdfOnFinalApproval(IOMAppHistoryViewModel IHVM, Employee_Details emp_dtl, string attachment) //Added by TTL CR5813 (08-Mar-2025)
        {
            SendMailAnnotatedPdfOnFinalApproval(IHVM, IHVM.APPROVAL_STATUS, emp_dtl, attachment);
        }

        public short IOMCancel(IOMHeaderViewModel PHVM)
        {
            return _IOMRepo.IOMCancel(PHVM);
        }

        public Tuple<short, long> SaveIOMRequest(IOMHeaderViewModel model)
        {
            Tuple<short, long> _tuple = _IOMRepo.SaveIOMRequest(model);
            if (_tuple.Item1 == 1 && _tuple.Item2 > 0 && model.IsFinalSubmit == 1)
            {
                SendMailByRequestor(_tuple.Item2);
            }
            return _tuple;
        }

        public List<IOMAppAuthSeqViewModel> GetDefaultAuthority(long loginUser)
        {
            List<IOMAppAuthSeqViewModel> AuthList = new List<IOMAppAuthSeqViewModel>();
            try
            {
                UserApprovalAuthority Obj = _CommonRepo.CheckApprovalAuthority(loginUser);
                if (Obj != null)
                {
                    if (Obj.SectionManager != 0 && Obj.SectionManager != null)
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.SectionManager));
                        AuthList.Add(new IOMAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = Emp_Dtl._Desig,
                            APP_SEQ = 1,
                            APPTYPE = 1,
                            FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                        });
                    }
                    if (Obj.DepartmentManager != 0 && Obj.DepartmentManager != null)
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.DepartmentManager));
                        AuthList.Add(new IOMAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = Emp_Dtl._Desig,
                            APP_SEQ = 2,
                            APPTYPE = 1,
                            FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                        });
                    }
                    if (Obj.Coordinator != 0 && Obj.Coordinator != null)
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.Coordinator));
                        AuthList.Add(new IOMAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = Emp_Dtl._Desig,
                            APP_SEQ = 3,
                            APPTYPE = 1,
                        });
                    }
                    if (Obj.DivisionHead != 0 && Obj.DivisionHead != null)
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.DivisionHead));
                        AuthList.Add(new IOMAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = Emp_Dtl._Desig,
                            APP_SEQ = 4,
                            APPTYPE = 1,
                            FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                        });
                    }
                    //if (Obj.EXECoordinator != 0 && Obj.EXECoordinator != null)
                    //{
                    //    Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.EXECoordinator));
                    //    AuthList.Add(new PRAppAuthSeqViewModel
                    //    {
                    //        ADEMPCODE = Emp_Dtl._ECode,
                    //        ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                    //        ADDESIGNATION = Emp_Dtl._Desig,
                    //        APP_SEQ = 5,
                    //        APPTYPE = 1,
                    //        FNDESID = 4,
                    //    });
                    //}
                    //if (Obj.OperationHead != 0 && Obj.OperationHead != null)
                    //{
                    //    Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.OperationHead));
                    //    AuthList.Add(new PRAppAuthSeqViewModel
                    //    {
                    //        ADEMPCODE = Emp_Dtl._ECode,
                    //        ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                    //        ADDESIGNATION = Emp_Dtl._Desig,
                    //        APP_SEQ = 6,
                    //        APPTYPE = 1,
                    //        FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                    //    });
                    //}
                    //if (Obj.Director != 0 && Obj.Director != null)
                    //{
                    //    short atype;
                    //    Employee_Details Emp_Dtl = _CommonRepo.GetDirectorDetailById(Convert.ToInt64(Obj.Director), Convert.ToInt64(Obj.OperationId), out atype);
                    //    AuthList.Add(new PRAppAuthSeqViewModel
                    //    {
                    //        ADEMPCODE = Emp_Dtl._ECode,
                    //        ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                    //        ADDESIGNATION = Emp_Dtl._Desig,
                    //        APP_SEQ = Convert.ToInt16(AuthList.Count + 1),
                    //        APPTYPE = atype,
                    //        FNDESID = 6,
                    //    });
                    //}
                    //if (Obj.Director2 != 0 && Obj.Director2 != null)
                    //{
                    //    short atype;
                    //    Employee_Details Emp_Dtl = _CommonRepo.GetDirectorDetailById(Convert.ToInt64(Obj.Director2), Convert.ToInt64(Obj.OperationId), out atype);
                    //    AuthList.Add(new PRAppAuthSeqViewModel
                    //    {
                    //        ADEMPCODE = Emp_Dtl._ECode,
                    //        ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                    //        ADDESIGNATION = Emp_Dtl._Desig,
                    //        APP_SEQ = Convert.ToInt16(AuthList.Count + 1),
                    //        APPTYPE = atype,
                    //        FNDESID = 7,
                    //    });
                    //}

                }
            }
            catch
            {
                AuthList = new List<IOMAppAuthSeqViewModel>();
            }
            return AuthList.OrderBy(m => m.APPTYPE).OrderBy(n => n.APP_SEQ).ToList();
        }

        public Tuple<short, List<IOMDetailViewModel>> SaveAttachment(long addedDate, long iomHid, List<IOMDetailViewModel> modelList)
        {
            List<IOMDetailViewModel> iList = new List<IOMDetailViewModel>();
            short retVal = _IOMRepo.SaveIOMDetails(addedDate, iomHid, modelList);
            if (retVal == 1)
            {
                iList = _IOMRepo.GetAttachmentDetail(iomHid);
            }
            Tuple<short, List<IOMDetailViewModel>> _tuple = new Tuple<short, List<IOMDetailViewModel>>(retVal, iList);
            return _tuple;
        }

        public Tuple<short, List<IOMDetailViewModel>> DeleteAttachment(string fileName, string docType, long iomHid)
        {
            List<IOMDetailViewModel> iList = new List<IOMDetailViewModel>();
            short retVal = _IOMRepo.DeleteAttachment(fileName, docType, iomHid);
            if (retVal == 1)
            {
                iList = _IOMRepo.GetAttachmentDetail(iomHid);
            }
            Tuple<short, List<IOMDetailViewModel>> _tuple = new Tuple<short, List<IOMDetailViewModel>>(retVal, iList);
            return _tuple;
        }

        public short SendMailByRequestor(long iomHeaderId)
        {
            short retVal = 0;
            try
            {
                IOMHeaderViewModel PHVM = _IOMRepo.GetIOMRequestById(iomHeaderId);
                if (PHVM.iomAppHis.Count > 0)
                {
                    IOMAppHistoryViewModel Auth_obj = PHVM.iomAppHis.Where(a => a.IOMAPPHISTORY_ID > 0 && a.APPROVAL_STATUS == 0 && !string.IsNullOrEmpty(a.APP_EMAIL)).FirstOrDefault();
                    if (Auth_obj != null)
                    {
                        commanEmail sendMail = new commanEmail();
                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        if (serverpath.isTestServer())
                            sendMail.MailTo = serverpath.getTestEMail();
                        else
                            sendMail.MailTo = Auth_obj.APP_EMAIL;

                        string struid = Auth_obj.ADEMPCODE.ToString();
                        string strid = iomHeaderId.ToString();

                        string strSubject = "IOM Request from - " + PHVM.Emp_Detail._EName + ", Employee Code - " + PHVM.Emp_Detail._ECode;
                        string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                         "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>IOM Request from " + PHVM.Emp_Detail._EName + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +
                                          "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + Auth_obj.APPEMP_NAME + " San ,</br></br>" + PHVM.Emp_Detail._EName + " San has raised a IOM Approval Request in Employee Portal. Below are the details :</td></tr>" +
                                         "<tr><td width=125 height=22 valign=top>IOM description :</td><td width=389 valign=top>" + PHVM.IOMDesc + "</td></tr>" +
                                         "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "/Login/EmailApproval/?Wid=" + struid + "&C=IOM&A=IOMApproval&Tid=" + strid + " > Employee Portal</a> link to approve the request.</td></tr>" +
                                         //"<tr><td valign=top colspan=2>Please click on <a href=" + "https://m.portal.honda2wheelersindia.com" + " > E-Portal Mobile App</a> link to approve the request.</td></tr>" + //CR5820-TTL, Commented for ePortal Mobile
                                         "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                        sendMail.MailSubject = strSubject;
                        sendMail.MailBody = strBody;

                        //FCM Notification Implementation || CR7770 @ START
                        try
                        {
                            //Send FCM Notification To Approver
                            var fCMNotificationModels = new List<FCMNotificationModel>();
                            foreach (var email in sendMail.MailTo.Split(","))
                            {
                                fCMNotificationModels.Add(new FCMNotificationModel
                                {
                                    title = "HMSI ePortal",
                                    body = string.Concat("Approval Note - ", PHVM.Emp_Detail._EName, "(", PHVM.Emp_Detail._ECode, ") has requested for approval."),
                                    accessToken = "self",
                                    loggedBy = PHVM.ADDEDBY.ToString(),
                                    userId = email
                                });
                            }
                            if (fCMNotificationModels.Count > 0)
                                _fcmService.SendFCMNotification(fCMNotificationModels);
                        }
                        catch (Exception) { }
                        //FCM Notification Implementation || CR7770 @ END

                        try
                        {
                            // bool status = sendMail.Send(); //Change1
                            Thread bgThread = new Thread(() => sendMail.Send());
                            bgThread.Start();
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
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }

        //public short SendMailByApprovalAuthority(long iomHeaderId short approvalStatus, Employee_Details employeeDetails) //   Commented by Aumento 12072024
        //Added by TTL CR5813 (08-Mar-2025) | PROCESS_STATUS parameter added, mail will not be sent to requester on final approval, this will be sent from controller after file generation.
        public short SendMailByApprovalAuthority(IOMAppHistoryViewModel IHVM, short approvalStatus, Employee_Details employeeDetails, short PROCESS_STATUS) //  { long iomHeaderId to  IOMAppHistoryViewModel IHVM }  Updated by Aumento 12072024
        {
            long iomHeaderId = long.Parse(IHVM.IOMID.ToString()); ///Added by Aumento 12072024
            short retVal = 0;
            try
            {


                //string RequestStatus = approvalStatus == 1 ? "Approved" : approvalStatus == 2 ? "Send back" : approvalStatus == 3 ? "Rejected" : ""; //   Commented by Aumento 12072024
                string RequestStatus = approvalStatus == 1 ? "Approved" : approvalStatus == 2 ? "Send back" : approvalStatus == 3 ? "Rejected" : approvalStatus == 5 ? "Hold" : ""; //  { approvalStatus == 4 ? "Hold"}  Updated by Aumento 12072024

                #region Send mail next approval authority
                IOMHeaderViewModel PHVM = _IOMRepo.GetIOMRequestById(iomHeaderId);

                if (PHVM.iomAppHis.Count > 0 && approvalStatus == 1)
                {
                    IOMAppHistoryViewModel Auth_obj = PHVM.iomAppHis.Where(a => a.IOMAPPHISTORY_ID > 0 && a.APPROVAL_STATUS == 0 && !string.IsNullOrEmpty(a.APP_EMAIL)).FirstOrDefault();
                    if (Auth_obj != null)
                    {
                        commanEmail sendMail = new commanEmail();
                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        if (serverpath.isTestServer())
                            sendMail.MailTo = serverpath.getTestEMail();
                        else
                            sendMail.MailTo = Auth_obj.APP_EMAIL;

                        string struid = Auth_obj.ADEMPCODE.ToString();
                        string strid = iomHeaderId.ToString();

                        string strSubject = "IOM Request from - " + PHVM.Emp_Detail._EName + ", Employee Code - " + PHVM.Emp_Detail._ECode;
                        string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                         "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>IOM Request from " + PHVM.Emp_Detail._EName + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +
                                          "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + Auth_obj.APPEMP_NAME + " San ,</br></br>" + PHVM.Emp_Detail._EName + " San has raised a IOM Approval Request in Employee Portal. Below are the details :</td></tr>" +
                                         "<tr><td width=125 height=22 valign=top>IOM Description :</td><td width=389 valign=top>" + PHVM.IOMDesc + "</td></tr>" +

                                        "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "/Login/EmailApproval/?Wid=" + struid + "&C=IOM&A=IOMApproval&Tid=" + strid + " > Employee Portal</a> link to approve the request.</td></tr>" +
                                         //"<tr><td valign=top colspan=2>Please click on <a href="+"https://m.portal.honda2wheelersindia.com"+" > E-Portal Mobile App</a> link to approve the request.</td></tr>" + //CR5820-TTL, Commented for ePortal Mobile
                                         "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                        sendMail.MailSubject = strSubject;
                        sendMail.MailBody = strBody;

                        //FCM Notification Implementation || CR7770 @ START
                        try
                        {
                            //Send FCM Notification To Approver
                            var fCMNotificationModels = new List<FCMNotificationModel>();
                            foreach (var email in sendMail.MailTo.Split(","))
                            {
                                fCMNotificationModels.Add(new FCMNotificationModel
                                {
                                    title = "HMSI ePortal",
                                    body = string.Concat("Approval Note - ", PHVM.Emp_Detail._EName, "(", PHVM.Emp_Detail._ECode, ") has requested for approval."),
                                    accessToken = "self",
                                    loggedBy = PHVM.ADDEDBY.ToString(),
                                    userId = email
                                });
                            }
                            if (fCMNotificationModels.Count > 0)
                                _fcmService.SendFCMNotification(fCMNotificationModels);
                        }
                        catch (Exception) { }
                        //FCM Notification Implementation || CR7770 @ END

                        try
                        {
                            //bool status = sendMail.Send(); //change2
                            Thread bgThread = new Thread(() => sendMail.Send());
                            bgThread.Start();
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
                #endregion

                #region Send mail for requestor
                if (!string.IsNullOrEmpty(PHVM.Emp_Detail._EmailId) && PROCESS_STATUS != 2) //Added by TTL CR5813 (08-Mar-2025) | PROCESS_STATUS
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    sendMail.MailTo = PHVM.Emp_Detail._EmailId; // /// temporary Commented by Aumento 12072024
                    string strSubject = "IOM Approval Status - " + RequestStatus;
                    string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>IOM Approval Status " + RequestStatus + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +

                                     "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                                     "<tr><td valign=top colspan =2>Your IOM request has been <b>" + RequestStatus + "</b> by " + employeeDetails.Employee_Name + " San. The request details are as follows:</td></tr>" +
                                     "<tr><td width=125 height=22 valign=top>IOM Description</td><td width=389 valign=top>" + PHVM.IOMDesc + "</td></tr>" +
                                     (approvalStatus == 5 ? "<tr style = 'background-color:yellow;'><td width=125 height=22 valign=top>Remarks </td><td width=389 valign=top>" + IHVM.APPROVAL_REMARK + "</td></tr>" : "") +  //   Added by Aumento 12072024
                                     "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "Login/Index> Employee Portal</a> to view the approval history.</td></tr>" +
                                     "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    try
                    {
                        // bool status = sendMail.Send(); //Change3
                        Thread bgThread = new Thread(() => sendMail.Send());
                        bgThread.Start();
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
        public short SendMailAnnotatedPdfOnFinalApproval(IOMAppHistoryViewModel IHVM, short approvalStatus, Employee_Details employeeDetails, string attachment)
        {
            long iomHeaderId = long.Parse(IHVM.IOMID.ToString());
            short retVal = 0;
            try
            {
                string RequestStatus = approvalStatus == 1 ? "Approved" : approvalStatus == 2 ? "Send back" : approvalStatus == 3 ? "Rejected" : approvalStatus == 5 ? "Hold" : "";
                IOMHeaderViewModel PHVM = _IOMRepo.GetIOMRequestById(iomHeaderId);

                #region Send mail to requestor with attachment
                if (!string.IsNullOrEmpty(PHVM.Emp_Detail._EmailId))
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    sendMail.MailTo = PHVM.Emp_Detail._EmailId; // /// temporary Commented by Aumento 12072024
                    string strSubject = "IOM Approval Status - " + RequestStatus;
                    string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>IOM Approval Status " + RequestStatus + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +

                                     "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                                     "<tr><td valign=top colspan =2>Your IOM request has been <b>" + RequestStatus + "</b> by " + employeeDetails.Employee_Name + " San. The request details are as follows:</td></tr>" +
                                     "<tr><td width=125 height=22 valign=top>IOM Description</td><td width=389 valign=top>" + PHVM.IOMDesc + "</td></tr>" +
                                     (approvalStatus == 5 ? "<tr style = 'background-color:yellow;'><td width=125 height=22 valign=top>Remarks </td><td width=389 valign=top>" + IHVM.APPROVAL_REMARK + "</td></tr>" : "") +  //   Added by Aumento 12072024
                                     "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "Login/Index> Employee Portal</a> to view the approval history.</td></tr>" +
                                     "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;

                    if (!string.IsNullOrEmpty(attachment))
                    {
                        sendMail.AttachmentFilePath.Add(attachment);
                    }

                    try
                    {
                        Thread bgThread = new Thread(() => sendMail.Send());
                        bgThread.Start();
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
        public short SendMailByApprovalAuthorityForHoldRequest(IOMHeaderViewModel Model, Employee_Details employeeDetails) //  Added by Aumento 13072024
        {
            short retVal = 0;
            try
            {
                long iomHeaderId = Model.IOMHEADERID;
                IOMHeaderViewModel PHVM = _IOMRepo.GetIOMRequestById(iomHeaderId);
                #region  Send mail to approver after hold  Requester Upload Doc               
                IOMAppHistoryViewModel Auth_obj = PHVM.iomAppHis.Where(a => a.IOMAPPHISTORY_ID > 0 && a.APPROVAL_STATUS == 5 && !string.IsNullOrEmpty(a.APP_EMAIL)).OrderByDescending(x => x.IOMAPPHISTORY_ID).FirstOrDefault();
                if (Auth_obj != null)
                {
                    Auth_obj.ADDEDBY = employeeDetails._ECode;
                    Auth_obj.UPDATEBY = employeeDetails._ECode;
                    //Hold to Pending Approval Document - Added by Bhupesh NTT for CR-5233                   
                    var holdtopendingRetVal = _IOMRepo.SaveApprovalHoldToPending(Auth_obj);
                    //Hold to Pending Approval Document - Added by Bhupesh NTT for CR-5233 
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    if (serverpath.isTestServer())
                        sendMail.MailTo = serverpath.getTestEMail();
                    else
                        sendMail.MailTo = Auth_obj.APP_EMAIL;
                    string struid = Auth_obj.ADEMPCODE.ToString();
                    string strid = iomHeaderId.ToString();
                    string strSubject = "IOM Request from - " + PHVM.Emp_Detail._EName + ", Employee Code - " + PHVM.Emp_Detail._ECode;
                    string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>IOM Request from " + PHVM.Emp_Detail._EName + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +
                                      "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + Auth_obj.APPEMP_NAME + " San ,</br></br>" + PHVM.Emp_Detail._EName + " San has uploaded documents in IOM approval request as a justification which is hold by you in Employee Portal. Please take appropriate action. Below are the details: </td></tr>" +
                                     "<tr><td width=125 height=22 valign=top>IOM Description :</td><td width=389 valign=top>" + PHVM.IOMDesc + "</td></tr>" +

                                    "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "/Login/EmailApproval/?Wid=" + struid + "&C=IOM&A=IOMApproval&Tid=" + strid + " > Employee Portal</a> link to approve the request.</td></tr>" +
                                     // "<tr><td valign=top colspan=2>Please click on <a href=" + "https://m.portal.honda2wheelersindia.com" + " > E-Portal Mobile App</a> link to approve the request.</td></tr>" + //CR5820-TTL, Commented for ePortal Mobile
                                     "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    try
                    {
                        // bool status = sendMail.Send(); //Change4
                        Thread bgThread = new Thread(() => sendMail.Send());
                        bgThread.Start();
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
        public List<Employee_Details> PortalAutocompleteSuggestions(string term, string designation)
        {
            return _IOMRepo.PortalAutocompleteSuggestions(term, designation);
        }
        public List<Employee_Details> AutocompleteDesignation(string term)
        {
            return _IOMRepo.AutocompleteDesignation(term);
        }
        public long GetIOMNextApprovalId(long IOMID, long ecode)
        {
            return _IOMRepo.GetIOMNextApprovalId(IOMID, ecode);
        }
        //Changed by TTL on 18th July 2025 against SR101913 > CR6738 - Start
        public List<VM_DGIT_IOMCATMST> BindIOMCategory(string divisionId = "")
        {
            return _IOMRepo.BindIOMCategory(divisionId);
        }
        //Changed by TTL on 18th July 2025 against SR101913 > CR6738 - End

        public Tuple<short, long> SaveAdditionalIOMRequest(IOMHeaderViewModel model)
        {
            Tuple<short, long> _tuple = _IOMRepo.SaveAdditionalIOMRequest(model);
            if (_tuple.Item1 == 1 && _tuple.Item2 > 0 && model.IsFinalSubmit == 1)
            {
                SendMailByRequestor(_tuple.Item2);
            }
            return _tuple;
        }

        //Below button added by aumento as 15062023 for SR50547--------------------------------------------------------------------------------------
        public List<IOMAppAuthSeqViewModel> PrevAuthority(string AppType, long Adempcode)
        {
            return _IOMRepo.GetPrevAuthority(AppType, Adempcode);
        }

        public SearchIOM IOMUserReport(SearchIOM VM)
        {
            return _IOMRepo.IOMUserReport(VM);
        }
        public Tuple<long, List<PR_Div_Dep_SecViewModel>> BindDivision(long op_Id, long LoginEcode, long kiid)
        {
            return _IOMRepo.BindDivision(LoginEcode, kiid, op_Id);
        }

        public Tuple<long, List<PR_Div_Dep_SecViewModel>> BindOperation(long Loginempcode, long KIID)
        {
            return _IOMRepo.BindOperation(Loginempcode, KIID);
        }

        public Tuple<long, List<PR_Div_Dep_SecViewModel>> BindDepartment(long Loginempcode, long KIID, long op_Id, long Div_id)
        {
            return _IOMRepo.BindDepartment(Loginempcode, KIID, op_Id, Div_id);
        }

        public Tuple<long, List<PR_Div_Dep_SecViewModel>> BindSection(long Loginempcode, long KIID, long op_Id, long divid, long deptid)
        {
            return _IOMRepo.BindSection(Loginempcode, KIID, op_Id, divid, deptid);
        }

        public List<PR_Div_Dep_SecViewModel> GetKiLIST(long ecode)
        {
            return _IOMRepo.GetKiLIST(ecode);
        }
        public Employee_Details GetEmployeeDetail(long ecode, long KIID)
        {
            return _IOMRepo.GetEmployeeDetail(ecode, KIID);
        }
        //Added by Aumento for SR99176
        public List<PR_Div_Dep_SecViewModel> GetKICodeLIst()
        {
            return _IOMRepo.GetKICodeLIst();
        }
        public List<Employee_Details> GetAllOperation(long KIID)
        {
            return _IOMRepo.GetAllOperation(KIID);
        }
        public List<Employee_Details> GetDivisionByOp(long OpCode, long KIID)
        {
            return _IOMRepo.GetDivisionByOp(OpCode, KIID);
        }
        public List<Employee_Details> GetDepartmentByDiv(long DivCode, long OpCode, long KIID)
        {
            return _IOMRepo.GetDepartmentByDiv(DivCode, OpCode, KIID);
        }
        public List<Employee_Details> GetSectionByDept(long DeptCode, long DivCode, long OpCode, long KIID)
        {
            return _IOMRepo.GetSectionByDept(DeptCode, DivCode, OpCode, KIID);
        }
        public SearchIOM IOMAuditReport(SearchIOM VM)
        {
            return _IOMRepo.IOMAuditReport(VM);
        }
        //Added by Aumento for SR99176    
        //-------------------------------------------------------------------------------------------------------------------------------------------

        // Added by TTL on 17-July-2025 against SR93758 > CR5975 - Start
        #region IOMDashboardChart
        public Tuple<List<IOMDashboardGraphViewModel>, long> IOMDashboardGraphData(SearchIOM VM)
        {
            return _IOMRepo.GetIOMDashboardGraphData(VM);
        }
        public SearchIOMDashboadReportList IOMDashboadReportList(SearchIOMDashboadReportList VM)
        {
            return _IOMRepo.GetIOMDashboadReportList(VM);
        }
        public SearchIOMDashboadReportList IOMDashboadReportListByDesignationGroup(SearchIOMDashboadReportList VM)
        {
            return _IOMRepo.GetIOMDashboadReportListByDesignationGroup(VM);
        }
        public Tuple<long, List<PR_Div_Dep_SecViewModel>> BindIOMGraphOperation(long Loginempcode, long KIID)
        {
            return _IOMRepo.BindIOMGraphOperation(Loginempcode, KIID);
        }
        #endregion
        // Added by TTL on 17-July-2025 against SR93758 > CR5975 - End

        //Added by TTL on 28-July-2025 against SR104160 > CR6821 - Start
        public List<DropdownList> GetEmployeeListAsPerOrgLevel(long orgLevelId, long? KIID)
        {
            return _CommonRepo.GetEmployeeListAsPerOrgLevel(orgLevelId, KIID);
        }
        public List<DropdownList> GetEmployeeListAsPerOrgLevel(long[] orgLevelId, long? KIID)
        {
            return _CommonRepo.GetEmployeeListAsPerOrgLevel(orgLevelId, KIID);
        }
        //Added by TTL on 28-July-2025 against SR104160 > CR6821 - End
        //Added by TTL on 28th Oct 2025 against SR109889 > CR7269 - Start
        public List<PRDetailViewModel> GetPRDocumentsByIndentNo(List<string> IndentNos)
        {
            return _IOMRepo.GetPRDocumentsByIndentNo(IndentNos);
        }
        //Added by TTL on 28th Oct 2025 against SR109889 > CR7269 - End
    }
}

