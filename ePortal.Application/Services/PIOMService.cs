using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;
using ePortal.Shared;
using ePortal.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Services
{
    public class PIOMService : IPIOMService
    {
        private readonly PIOMRepository _PIOMRepo;
        private readonly CommonRepository _CommonRepo;
        private readonly Tuple<short, long> _retVal_tuple;
        private readonly ILogger<PIOMService> _logger;

        public PIOMService(PIOMRepository piomRepo, CommonRepository commonRepo, ILogger<PIOMService> logger)
        {
            _PIOMRepo = piomRepo;
            _CommonRepo = commonRepo;
            _retVal_tuple = new Tuple<short, long>((short)0, 0L);
            _logger = logger;
        }
        public Tuple<short, long> SavePIOMRequest(IOMPHeaderViewModel model)
        {
            Tuple<short, long> _tuple = _PIOMRepo.SavePIOMRequest(model);
            if (_tuple.Item1 == 1 && _tuple.Item2 > 0 && model.IsFinalSubmit == 1)
            {
                SendMailByRequestor(_tuple.Item2);
            }
            return _tuple;
        }
        public short SendMailByRequestor(long iomHeaderId)
        {
            short retVal = 0;
            try
            {
                _logger.LogInformation("SendMailByRequestor started for IOM Header ID: {IOMHeaderId}", iomHeaderId);

                IOMPHeaderViewModel PHVM = _PIOMRepo.GetPIOMRequestById(iomHeaderId);
                if (PHVM == null)
                {
                    _logger.LogWarning("No PIOM Request found for ID: {IOMHeaderId}", iomHeaderId);
                    return -1;
                }

                if (PHVM.iompAppHis == null || PHVM.iompAppHis.Count == 0)
                {
                    _logger.LogInformation("No approval history records found for IOM Header ID: {IOMHeaderId}", iomHeaderId);
                    return 0;
                }

                List<IOMPAppHistoryViewModel> Auth_obj = PHVM.iompAppHis
                    .Where(a => a.IOMAPPHISTORY_ID > 0 && a.ApprovalStatus == 0 && !string.IsNullOrEmpty(a.Email))
                    .ToList();

                _logger.LogInformation("Found {Count} pending approval records with valid emails.", Auth_obj.Count);

                foreach (var seq in Auth_obj)
                {
                    try
                    {
                        commanEmail sendMail = new commanEmail
                        {
                            MailFrom = "portal.admin@honda.hmsi.in",
                            MailTo = serverpath.isTestServer() ? serverpath.getTestEMail() : seq.Email
                        };

                        string struid = seq.EmpCode.ToString();
                        string strid = iomHeaderId.ToString();

                        string strSubject = $"IOM Request from - {PHVM.Emp_Detail._EName}, Employee Code - {PHVM.Emp_Detail._ECode}";
                        string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                         "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>IOM Request from " + PHVM.Emp_Detail._EName +
                                         " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +
                                         "<tr><td><table cellpadding=4 cellspaci" +
                                         "ng=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + seq.EmpName +
                                         " - ["+ seq.EmpCode + "] San,</br></br>" + PHVM.Emp_Detail._EName +
                                         " San has raised a IOM Approval Request in Employee Portal. Below are the details :</td></tr>" +
                                         "<tr><td width=125 height=22 valign=top>IOM description :</td><td width=389 valign=top>" + PHVM.IOMDesc + "</td></tr>" +
                                         "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() +
                                         "/Login/EmailApproval/?Wid=" + struid + "&C=PIOM&A=PIOMApproval&Tid=" + strid + " > Employee Portal</a> link to approve the request.</td></tr>" +
                                         "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                        sendMail.MailSubject = strSubject;
                        sendMail.MailBody = strBody;

                        _logger.LogInformation("Sending IOM email to: {Email}, Subject: {Subject}", sendMail.MailTo, strSubject);

                        Thread bgThread = new Thread(() =>
                        {
                            try
                            {
                                sendMail.Send();
                                _logger.LogInformation("Email successfully sent to: {Email}", sendMail.MailTo);
                            }
                            catch (Exception mailEx)
                            {
                                _logger.LogError(mailEx, "Failed to send email to: {Email}", sendMail.MailTo);
                            }
                        });

                        bgThread.Start();
                        retVal = 1;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while processing email for employee: {EmpCode}", seq.EmpCode);
                        retVal = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in SendMailByRequestor for IOM Header ID: {IOMHeaderId}", iomHeaderId);
                retVal = -1;
            }

            _logger.LogInformation("SendMailByRequestor completed for IOM Header ID: {IOMHeaderId} with return value: {ReturnValue}", iomHeaderId, retVal);
            return retVal;
        }

        public Tuple<short, List<IOMPDetailViewModel>> SavePIOMAttachment(long addedDate, long iomHid, List<IOMPDetailViewModel> modelList)
        {
            List<IOMPDetailViewModel> iList = new List<IOMPDetailViewModel>();
            short retVal = _PIOMRepo.SaveIOMDetails(addedDate, iomHid, modelList);
            if (retVal == 1)
            {
                iList = _PIOMRepo.GetAttachmentDetail(iomHid);
            }
            Tuple<short, List<IOMPDetailViewModel>> _tuple = new Tuple<short, List<IOMPDetailViewModel>>(retVal, iList);
            return _tuple;
        }

        public Tuple<short, List<IOMPDetailViewModel>> DeletePIOMAttachment(string fileName, string docType, long iomHid)
        {
            List<IOMPDetailViewModel> iList = new List<IOMPDetailViewModel>();
            short retVal = _PIOMRepo.DeleteAttachment(fileName, docType, iomHid);
            if (retVal == 1)
            {
                iList = _PIOMRepo.GetAttachmentDetail(iomHid);
            }
            Tuple<short, List<IOMPDetailViewModel>> _tuple = new Tuple<short, List<IOMPDetailViewModel>>(retVal, iList);
            return _tuple;
        }

        public IOMPHeaderViewModel GetPIOMRequestById(long id)
        {
            return _PIOMRepo.GetPIOMRequestById(id);
        }

        public short PIOMApproval(IOMPAppHistoryViewModel IHVM, Employee_Details emp_dtl, IOMPHeaderViewModel iomObj)
        {
            short retVal = _PIOMRepo.PIOMApproval(IHVM, iomObj, emp_dtl);
            return retVal;
        }

        public short SendMailByApprovalAuthorityForHoldRequest(IOMPHeaderViewModel Model, Employee_Details employeeDetails)
        {
            {
                short retVal = 0;
                try
                {
                    long iomHeaderId = Model.IOMHEADERID;
                    IOMPHeaderViewModel PHVM = _PIOMRepo.GetPIOMRequestById(iomHeaderId);
                    #region  Send mail to approver after hold  Requester Upload Doc               
                    var Auth_objlist = PHVM.iompAppHis
                  .GroupBy(a => new { a.IOMID, a.Seq_id })  // group by IOM + sequence
                  .Select(g => g.OrderByDescending(x => x.IOMAPPHISTORY_ID).FirstOrDefault()) // get latest record per group
                  .Where(a => a != null
                           && a.ApprovalStatus == 5        // only hold status
                           && !string.IsNullOrEmpty(a.Email)) // email must be present
                  .ToList();
                    foreach (var Auth_obj in Auth_objlist)
                    {
                        if (Auth_obj != null)
                        {
                            Auth_obj.AddedBy = employeeDetails._ECode;
                            Auth_obj.UpdatedBy = employeeDetails._ECode;
                            //Hold to Pending Approval Document - Added by Bhupesh NTT for CR-5233                   
                            var holdtopendingRetVal = _PIOMRepo.SaveApprovalHoldToPending(Auth_obj);
                            //Hold to Pending Approval Document - Added by Bhupesh NTT for CR-5233 
                            commanEmail sendMail = new commanEmail();
                            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                            if (serverpath.isTestServer())
                                sendMail.MailTo = serverpath.getTestEMail();
                            else
                                sendMail.MailTo = Auth_obj.Email;
                            string struid = Auth_obj.EmpCode.ToString();
                            string strid = iomHeaderId.ToString();
                            string strSubject = "IOM Request from - " + PHVM.Emp_Detail._EName + ", Employee Code - " + PHVM.Emp_Detail._ECode;
                            string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                             "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>IOM Request from " + PHVM.Emp_Detail._EName + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +
                                              "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + Auth_obj.EmpCode + " San ,</br></br>" + PHVM.Emp_Detail._EName + " San has uploaded documents in IOM approval request as a justification which is hold by you in Employee Portal. Please take appropriate action. Below are the details: </td></tr>" +
                                             "<tr><td width=125 height=22 valign=top>IOM Description :</td><td width=389 valign=top>" + PHVM.IOMDesc + "</td></tr>" +

                                            "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "/Login/EmailApproval/?Wid=" + struid + "&C=PIOM&A=PIOMApproval&Tid=" + strid + " > Employee Portal</a> link to approve the request.</td></tr>" +
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
                    }
                    
                    #endregion
                }
                catch (Exception ex)
                {
                    retVal = -1;
                }
                return retVal;
            }
        }

        public short PIOMCancel(IOMPHeaderViewModel PHVM)
        {
            return _PIOMRepo.PIOMCancel(PHVM);
        }

        public List<IOMPHeaderViewModel> GetPIOMPrevAuthority(long Adempcode)
        {
            return _PIOMRepo.GetPIOMPrevAuthority(Adempcode);
        }

        public short SendMailAnnotatedPdfOnFinalApproval(
        IOMPAppHistoryViewModel IHVM,
        short approvalStatus,
        Employee_Details employeeDetails,
        string attachment)
        {
            long iomHeaderId = long.Parse(IHVM.IOMID.ToString());
            short retVal = 0;

            try
            {
                string RequestStatus = approvalStatus == 1 ? "Approved"
                                    : approvalStatus == 2 ? "Send back"
                                    : approvalStatus == 3 ? "Rejected"
                                    : approvalStatus == 5 ? "Hold"
                                    : "";

                _logger.LogInformation("Starting SendMailAnnotatedPdfOnFinalApproval | IOMID: {IOMID}, Status: {Status}, EmpCode: {EmpCode}",
                    iomHeaderId, RequestStatus, employeeDetails._ECode);

                IOMPHeaderViewModel PHVM = _PIOMRepo.GetPIOMRequestById(iomHeaderId);

                if (PHVM == null)
                {
                    _logger.LogWarning("No PIOM Header found for IOMID: {IOMID}", iomHeaderId);
                    return -1;
                }

                #region Send mail to requestor with attachment
                if (!string.IsNullOrEmpty(PHVM.Emp_Detail._EmailId))
                {
                    _logger.LogInformation("Preparing email for Requestor: {EmailId}", PHVM.Emp_Detail._EmailId);

                    commanEmail sendMail = new commanEmail
                    {
                        MailFrom = "portal.admin@honda.hmsi.in",
                        MailTo = PHVM.Emp_Detail._EmailId,
                        MailSubject = $"IOM Approval Status - {RequestStatus}"
                    };

                    string strBody =
                        "<table cellpadding=0 cellspacing=0 style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                        "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>MSI Approval Status " + RequestStatus +
                        " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +
                        "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                        "<tr><td valign=top colspan=2>Your MSI request has been <b>" + RequestStatus +
                        "</b> by " + employeeDetails.Employee_Name + " San. The request details are as follows:</td></tr>" +
                        "<tr><td width=125 height=22 valign=top>MSI Description</td><td width=389 valign=top>" + PHVM.IOMDesc + "</td></tr>" +
                        (approvalStatus == 5
                            ? "<tr style='background-color:yellow;'><td width=125 height=22 valign=top>Remarks</td><td width=389 valign=top>" +
                              IHVM.ApprovalRemark + "</td></tr>"
                            : "") +
                        "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() +
                        "Login/Index>Employee Portal</a> to view the approval history.</td></tr>" +
                        "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                    sendMail.MailBody = strBody;

                    if (!string.IsNullOrEmpty(attachment))
                    {
                        sendMail.AttachmentFilePath.Add(attachment);
                        _logger.LogInformation("Attachment added for IOMID: {IOMID} | Path: {Attachment}", iomHeaderId, attachment);
                    }

                    try
                    {
                        _logger.LogInformation("Starting background email thread for IOMID: {IOMID}", iomHeaderId);
                        Thread bgThread = new Thread(() =>
                        {
                            try
                            {
                                sendMail.Send();
                                _logger.LogInformation("Email sent successfully to {EmailId} for IOMID: {IOMID}", PHVM.Emp_Detail._EmailId, iomHeaderId);
                            }
                            catch (Exception mailEx)
                            {
                                _logger.LogError(mailEx, "Error sending email in background thread for IOMID: {IOMID}", iomHeaderId);
                            }
                        });
                        bgThread.Start();

                        retVal = 1;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred while starting email thread for IOMID: {IOMID}", iomHeaderId);
                        retVal = -1;
                    }
                }
                else
                {
                    _logger.LogWarning("No email ID found for Requestor | IOMID: {IOMID}", iomHeaderId);
                }
                #endregion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in SendMailAnnotatedPdfOnFinalApproval | IOMID: {IOMID}", iomHeaderId);
                retVal = -1;
            }

            _logger.LogInformation("Completed SendMailAnnotatedPdfOnFinalApproval | IOMID: {IOMID}, ReturnValue: {RetVal}", iomHeaderId, retVal);
            return retVal;
        }

        public Tuple<short, long> SaveAdditionalPIOMRequest(IOMPHeaderViewModel model)
        {
            Tuple<short, long> _tuple = _PIOMRepo.SaveAdditionalIOMRequest(model);
            if (_tuple.Item1 == 1 && _tuple.Item2 > 0 && model.IsFinalSubmit == 1)
            {
                SendMailByRequestor(_tuple.Item2);
            }
            return _tuple;
        }
        public long GetPIOMNextApprovalId(long IOMID, long ecode)
        {
            return _PIOMRepo.GetPIOMNextApprovalId(IOMID, ecode);
        }

    }
}
