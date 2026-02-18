using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;
using ePortal.Shared;
using ePortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.Application.Services
{
    public class ICIBMService : IICIBMService
    {
        //ICIBMRepository _ICRepo;
        //CommonRepository _CommonRepo;
        //Tuple<short, long> _retVal_tuple;

        //public ICIBMService()
        //{
        //    _ICRepo = new ICIBMRepository();
        //    _CommonRepo = new CommonRepository();
        //    _retVal_tuple = new Tuple<short, long>((short)0, 0L);
        //}
        private readonly ICIBMRepository _ICRepo;
        private readonly CommonRepository _CommonRepo;
        private Tuple<short, long> _retVal_tuple;

        public ICIBMService(ICIBMRepository icRepo, CommonRepository commonRepo)
        {
            _ICRepo = icRepo;
            _CommonRepo = commonRepo;
            _retVal_tuple = new Tuple<short, long>((short)0, 0L);
        }
        public List<VM_DGIT_ICINVEST_MST> GetInvestEffectList()
        {
            return _ICRepo.GetInvestEffectList();
        }
        public Tuple<short, long> SaveICRequest(ICREQHEADER model)
        {
            Tuple<short, long> _tuple = _ICRepo.SaveICRequest(model);
            if (_tuple.Item1 == 1 && _tuple.Item2 > 0 && model.IsFinalSubmit == 1)
            {
                SendMailByRequestor(_tuple.Item2);
            }
            return _tuple;
        }
        public Tuple<short, List<VM_DGIT_ICDOCDETAIL>> SaveAttachment(long addedDate, long ICHid, List<VM_DGIT_ICDOCDETAIL> modelList)
        {
            List<VM_DGIT_ICDOCDETAIL> iList = new List<VM_DGIT_ICDOCDETAIL>();
            short retVal = _ICRepo.SaveICDetails(addedDate, ICHid, modelList);
            if (retVal == 1)
            {
                iList = _ICRepo.GetAttachmentDetail(ICHid);
            }
            Tuple<short, List<VM_DGIT_ICDOCDETAIL>> _tuple = new Tuple<short, List<VM_DGIT_ICDOCDETAIL>>(retVal, iList);
            return _tuple;
        }
        public Tuple<short, List<VM_DGIT_ICDOCDETAIL>> DeleteAttachment(string fileName, string docType, long poHid)
        {
            List<VM_DGIT_ICDOCDETAIL> iList = new List<VM_DGIT_ICDOCDETAIL>();
            short retVal = _ICRepo.DeleteAttachment(fileName, docType, poHid);
            if (retVal == 1)
            {
                iList = _ICRepo.GetAttachmentDetail(poHid);
            }
            Tuple<short, List<VM_DGIT_ICDOCDETAIL>> _tuple = new Tuple<short, List<VM_DGIT_ICDOCDETAIL>>(retVal, iList);
            return _tuple;
        }
        public ICREQHEADER GetICRequestById(long id)
        {
            return _ICRepo.GetICRequestById(id);
        }
        public short ICApproval(VM_DGIT_ICAPPHISTORY PHVM, Employee_Details emp_dtl)
        {
            short retVal = _ICRepo.ICApproval(PHVM);
            if (retVal == 1)
            {   // Mail Functionality Stop as per requirement 
                SendMailByApprovalAuthority(PHVM.ICREQID, PHVM.APPROVAL_STATUS, emp_dtl);
            }
            return retVal;
        }
        public short ICFinTaxApproval(VM_DGIT_ICAPPHISTORY PHVM, Employee_Details emp_dtl)
        {
            short retVal = _ICRepo.ICFinTaxApproval(PHVM);
            if (retVal == 1)
            {   // Mail Functionality Stop as per requirement 
                SendMailByApprovalAuthority(PHVM.ICREQID, PHVM.APPROVAL_STATUS, emp_dtl);
            }
            return retVal;
        }
        public Employee_Details GetAuthEmpById(long empCode, Employee_Details empDtl)
        {
            return _ICRepo.GetAuthEmpById(empCode, empDtl);
        }
        public VM_ICIBM_FIDASHBOARD_Search GetFinanceDashboard(VM_ICIBM_FIDASHBOARD_Search obj)
        {
            return _ICRepo.GetFinanceDashboard(obj);
        }
        public VM_ICIBM_FITAXDASHBOARD_Search GetFinanceTAXDashboard(VM_ICIBM_FITAXDASHBOARD_Search obj)
        {
            return _ICRepo.GetFinanceTaxDashboard(obj);
        }
        public VM_ICIBM_FIAUCDASHBOARD_Search GetFinanceAUCDashboard(VM_ICIBM_FIAUCDASHBOARD_Search obj)
        {
            return _ICRepo.GetFinanceAUCDashboard(obj);
        }
        public List<VM_SELECTITEMLIST> GetCycle(VM_SELECTITEMLIST obj)
        {
            return _ICRepo.GetCycle(obj);
        }
        public short ICFinanceApproval(VM_DGIT_ICAPPHISTORY PHVM, Employee_Details emp_dtl)
        {
            short retVal = _ICRepo.ICFinanceApproval(PHVM);
            if (retVal == 1)
            {
                SendMailByFinanceAuthority(PHVM.ICREQID, PHVM.APPROVAL_STATUS, emp_dtl);
            }
            return retVal;
        }
        public short RejectICRequest(VM_DGIT_ICAPPHISTORY PHVM, Employee_Details emp_dtl)
        {
            short retVal = _ICRepo.RejectICRequest(PHVM);
            if (retVal == 1)
            {   // Mail Functionality Stop as per requirement 
                SendMailByFinanceICReject(PHVM.ICREQID, PHVM.APPROVAL_STATUS, emp_dtl);
            }
            return retVal;
        }
        public short ICApprovalInitiation(VM_ICApprovalInitiation PHVM, Employee_Details emp_dtl)
        {
            short retVal = _ICRepo.ICApprovalInitiation(PHVM);
            if (retVal == 1)
            {   // Mail Functionality Stop as per requirement 
                // SendMailByApprovalAuthority(PHVM.POID, PHVM.APPROVAL_STATUS, emp_dtl);
            }
            return retVal;
        }
        public List<VM_DGIT_ICAPPAUTHSEQ> GetICAuthority()
        {
            return _ICRepo.GetICAuthority();
        }

        public Tuple<short, long> UpdateAUCCode(ICREQHEADER model)
        {
            Tuple<short, long> _tuple = _ICRepo.UpdateAUCCode(model);
            SendMailByFinanceAuthority_AUCCode(model.ICREQID);
            return _tuple;
        }

        //Save Asset IC Request
        public Tuple<short, long> SaveAssetICRequest(ICASSETDISREQHEADER model)
        {
            Tuple<short, long> _tuple = _ICRepo.SaveAssetICRequest(model);
            if (_tuple.Item1 == 1 && _tuple.Item2 > 0 && model.IsFinalSubmit == 1)
            {
               SendMailByRequestor_ASSET(_tuple.Item2);
            }
            return _tuple;
        }
        public Tuple<short, List<VM_DGIT_ICDOCDETAIL>> SaveAssetAttachment(long addedDate, long ICHid, List<VM_DGIT_ICDOCDETAIL> modelList)
        {
            List<VM_DGIT_ICDOCDETAIL> iList = new List<VM_DGIT_ICDOCDETAIL>();
            short retVal = _ICRepo.SaveICAssetDetails(addedDate, ICHid, modelList);
            if (retVal == 1)
            {
                iList = _ICRepo.GetAssetAttachmentDetail(ICHid);
            }
            Tuple<short, List<VM_DGIT_ICDOCDETAIL>> _tuple = new Tuple<short, List<VM_DGIT_ICDOCDETAIL>>(retVal, iList);
            return _tuple;
        }
        public Tuple<short, List<VM_DGIT_ICDOCDETAIL>> DeleteAssetAttachment(string fileName, string docType, long poHid)
        {
            List<VM_DGIT_ICDOCDETAIL> iList = new List<VM_DGIT_ICDOCDETAIL>();
            short retVal = _ICRepo.DeleteAssetAttachment(fileName, docType, poHid);
            if (retVal == 1)
            {
                iList = _ICRepo.GetAssetAttachmentDetail(poHid);
            }
            Tuple<short, List<VM_DGIT_ICDOCDETAIL>> _tuple = new Tuple<short, List<VM_DGIT_ICDOCDETAIL>>(retVal, iList);
            return _tuple;
        }
        public ICASSETDISREQHEADER GetICAssetRequestById(long id)
        {
            return _ICRepo.GetICAssetRequestById(id);
        }
        public short ICAssetApproval(VM_DGIT_ICAPPHISTORY PHVM, Employee_Details emp_dtl)
        {
            short retVal = _ICRepo.ICAssetApproval(PHVM);
            if (retVal == 1)
            {   // Mail Functionality Stop as per requirement 
                SendMailByApprovalAuthority_ASSET(PHVM.ICREQID, PHVM.APPROVAL_STATUS, emp_dtl);
            }
            return retVal;
        }
        public short RejectICAssetRequest(VM_DGIT_ICAPPHISTORY PHVM, Employee_Details emp_dtl)
        {
            short retVal = _ICRepo.RejectICAssetRequest(PHVM);
            if (retVal == 1)
            {   // Mail Functionality Stop as per requirement 
                SendMailByFinanceICReject_ASSET(PHVM.ICREQID, PHVM.APPROVAL_STATUS, emp_dtl);
            }
            return retVal;
        }
        public short ICAssetFinanceApproval(VM_DGIT_ICAPPHISTORY PHVM, Employee_Details emp_dtl)
        {
            short retVal = _ICRepo.ICAssetFinanceApproval(PHVM);
            if (retVal == 1)
            {   // Mail Functionality Stop as per requirement 
                SendMailByFinanceAuthority_ASSET(PHVM.ICREQID, PHVM.APPROVAL_STATUS, emp_dtl);
            }
            return retVal;
        }
        public long SaveICMemberMaster(long AddedBy, List<VM_DGIT_ICAPPAUTHSEQ> PSVMList)
        {
            long retVal = _ICRepo.SaveICMemberMaster(AddedBy, PSVMList);
            if (retVal == 1)
            {   // Mail Functionality Stop as per requirement 
                // SendMailByApprovalAuthority(PHVM.POID, PHVM.APPROVAL_STATUS, emp_dtl);
            }
            return retVal;

        }
        public List<VM_DGIT_ICCONFIG_MST> GetICConfig()
        {
            return _ICRepo.GetICConfig();
        }
        public short saveIC(VM_DGIT_ICCONFIG_MST data)
        {
            short retval = _ICRepo.saveIC(data);
            return retval;
        }
        public VM_DGIT_ICCONFIG_MST GetICConfig(VM_DGIT_ICCONFIG_MST data)
        {
            return _ICRepo.GetICConfig(data);
        }
        public short deleteIC(VM_DGIT_ICCONFIG_MST data)
        {
            short retval = _ICRepo.deleteIC(data);
            return retval;
        }
        public short updateIC(VM_DGIT_ICCONFIG_MST data)
        {
            return _ICRepo.UpdateIC(data);
        }

        public short SendMailByRequestor(long poHeaderId)
        {
            short retVal = 0;
            try
            {
                ICREQHEADER PHVM = _ICRepo.GetICRequestById(poHeaderId);
                if (PHVM.ICAPPHISTORY.Count > 0)
                {
                    VM_DGIT_ICAPPHISTORY Auth_obj = PHVM.ICAPPHISTORY.Where(a => a.ICAPPHISTORY_ID > 0 && a.APPROVAL_STATUS == 0 && !string.IsNullOrEmpty(a.APP_EMAIL)).FirstOrDefault();
                    if (Auth_obj != null)
                    {
                        commanEmail sendMail = new commanEmail();
                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        if (serverpath.isTestServer())
                            sendMail.MailTo = serverpath.getTestEMail();
                        else
                            sendMail.MailTo = Auth_obj.APP_EMAIL;
                        string struid = (Auth_obj.APPEMP_CODE).ToString();
                        string strid = poHeaderId.ToString();

                        string strSubject = "Investment Committee Approval Request Request from - " + PHVM.Emp_Detail._EName + ", Employee Code - " + PHVM.Emp_Detail._ECode;
                        string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                         "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Investment Committee Request from " + PHVM.Emp_Detail._EName + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +

                                         "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + Auth_obj.APPEMP_NAME + " San ,</br></br>" + PHVM.Emp_Detail._EName + " San has raised a IC Approval Request in Employee Portal. Below are the details :</td></tr>" +
                                        "<tr><td width=125 height=22 valign=top>IC Title</td><td width=389 valign=top>" + PHVM.ICTITLE + "</td></tr>" +
                                        "<tr><td width=125 height=23 valign=top>Department</td><td width=389 valign=top>" + PHVM.DEPARTMENT + "</td></tr>" +
                                        "<tr><td width=125 height=23 valign=top>Background</td><td width=389 valign=top>" + PHVM.ICBACKGROUND + "</td></tr>" +
                                        //"<tr><td width=125 height=23 valign=top>Vendor Email</td><td width=389 valign=top>" + PHVM.VENDORMAILID + "</td></tr>" +
                                        "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "/Login/EmailApproval/?Wid=" + struid + "&C=ICIBM&A=ICUserApproval&Tid=" + strid + " > Employee Portal</a> link to approve the request.</td></tr>" +
                                         // "<tr><td valign=top colspan=2>Please click on <a href=" + "https://m.portal.honda2wheelersindia.com" + " > E-Portal Mobile App</a> link to approve the request.</td></tr>" + //CR5820-TTL, Commented for ePortal Mobile
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
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }

        public short SendMailByApprovalAuthority(long poHeaderId, short approvalStatus, Employee_Details employeeDetails)
        {
            short retVal = 0;
            try
            {
                string RequestStatus = approvalStatus == 1 ? "Approved" : approvalStatus == 2 ? "Send back" : approvalStatus == 3 ? "Rejected" : "";

                #region Send mail next approval authority
                ICREQHEADER PHVM = _ICRepo.GetICRequestById(poHeaderId);
                if (PHVM.ICAPPHISTORY.Count > 0 && approvalStatus == 1)
                {
                    VM_DGIT_ICAPPHISTORY Auth_obj = PHVM.ICAPPHISTORY.Where(a => a.ICAPPHISTORY_ID > 0 && a.APPROVAL_STATUS == 0 && !string.IsNullOrEmpty(a.APP_EMAIL)).FirstOrDefault();
                    if (Auth_obj != null)
                    {
                        var APPTYPE = PHVM.ICAPPAUTHSEQ.Where(x => x.ADEMPCODE == Convert.ToInt64(Auth_obj.APPEMP_CODE)).FirstOrDefault().APPTYPE;

                        commanEmail sendMail = new commanEmail();
                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        if (serverpath.isTestServer())
                            sendMail.MailTo = serverpath.getTestEMail();
                        else
                            sendMail.MailTo = Auth_obj.APP_EMAIL;
                        string struid = (Auth_obj.APPEMP_CODE).ToString();
                        string strid = poHeaderId.ToString();
                        string strSubject = "Investment Committee Approval Request from - " + PHVM.Emp_Detail._EName + ", Employee Code - " + PHVM.Emp_Detail._ECode;
                        string strBody = string.Empty;

                        strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                        "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>IC Request from " + PHVM.Emp_Detail._EName + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +
                                         "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + Auth_obj.APPEMP_NAME + " San,</br>" + PHVM.Emp_Detail._EName + " San has raised a IC Approval Request in Employee Portal. Below are the details :</td></tr>" +
                                        "<tr><td width=125 height=22 valign=top>IC Title</td><td width=389 valign=top>" + PHVM.ICTITLE + "</td></tr>" +
                                        "<tr><td width=125 height=23 valign=top>Department</td><td width=389 valign=top>" + PHVM.DEPARTMENT + "</td></tr>" +
                                        "<tr><td width=125 height=23 valign=top>Background</td><td width=389 valign=top>" + PHVM.ICBACKGROUND + "</td></tr>" +
                                  //"<tr><td width=125 height=23 valign=top>Vendor Email</td><td width=389 valign=top>" + PHVM.VENDORMAILID + "</td></tr>" +
                                  //"<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "Login/Index> Employee Portal</a> for approval process.</td></tr>" +
                                  "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "/Login/EmailApproval/?Wid=" + struid + "&C=ICIBM&A=ICUserApproval&Tid=" + strid + " > Employee Portal</a> link to approve the request.</td></tr>" +
                                  //"<tr><td valign=top colspan=2>Please click on <a href=" + "https://m.portal.honda2wheelersindia.com" + " > E-Portal Mobile App</a> link to approve the request.</td></tr>" + //CR5820-TTL, Commented for ePortal Mobile
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
                #endregion

                #region Send mail for requestor
                if (!string.IsNullOrEmpty(PHVM.Emp_Detail._EmailId))
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    sendMail.MailTo = PHVM.Emp_Detail._EmailId;

                    string strSubject = "Investment Committee Approval Status - " + RequestStatus;
                    string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Investment Committee Approval Status " + RequestStatus + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +

                                     "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                                     "<tr><td valign=top colspan =2>Your Investment Committee request has been <b>" + RequestStatus + "</b> by " + employeeDetails.Employee_Name + " San. The request details are as follows:</td></tr>" +
                                      "<tr><td width=125 height=22 valign=top>IC Title</td><td width=389 valign=top>" + PHVM.ICTITLE + "</td></tr>" +
                                      "<tr><td width=125 height=23 valign=top>Department</td><td width=389 valign=top>" + PHVM.DEPARTMENT + "</td></tr>" +
                                      "<tr><td width=125 height=23 valign=top>Background</td><td width=389 valign=top>" + PHVM.ICBACKGROUND + "</td></tr>" +
                                     //"<tr><td width=125 valign=top>Vendor Email</td><td width=389 valign=top>" + PHVM.VENDORMAILID + "</td></tr>" +
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

        public short SendMailByRequestor_ASSET(long poHeaderId)
        {
            short retVal = 0;
            try
            {
                ICASSETDISREQHEADER PHVM = _ICRepo.GetICAssetRequestById(poHeaderId);
                if (PHVM.ICAPPHISTORY.Count > 0)
                {
                    VM_DGIT_ICAPPHISTORY Auth_obj = PHVM.ICAPPHISTORY.Where(a => a.ICAPPHISTORY_ID > 0 && a.APPROVAL_STATUS == 0 && !string.IsNullOrEmpty(a.APP_EMAIL)).FirstOrDefault();
                    if (Auth_obj != null)
                    {
                        commanEmail sendMail = new commanEmail();
                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        if (serverpath.isTestServer())
                            sendMail.MailTo = serverpath.getTestEMail();
                        else
                            sendMail.MailTo = Auth_obj.APP_EMAIL;
                        string struid = (Auth_obj.APPEMP_CODE).ToString();
                        string strid = poHeaderId.ToString();

                        string strSubject = "Investment Committee Approval Request Request from - " + PHVM.Emp_Detail._EName + ", Employee Code - " + PHVM.Emp_Detail._ECode;
                        string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                         "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Investment Committee Request from " + PHVM.Emp_Detail._EName + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +

                                         "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + Auth_obj.APPEMP_NAME + " San ,</br></br>" + PHVM.Emp_Detail._EName + " San has raised a IC Approval Request in Employee Portal. Below are the details :</td></tr>" +
                                        "<tr><td width=125 height=22 valign=top>IC Title</td><td width=389 valign=top>" + PHVM.ICTITLE + "</td></tr>" +
                                        "<tr><td width=125 height=23 valign=top>Department</td><td width=389 valign=top>" + PHVM.DEPARTMENT + "</td></tr>" +
                                        "<tr><td width=125 height=23 valign=top>Background</td><td width=389 valign=top>" + PHVM.ICBACKGROUND + "</td></tr>" +
                                        //"<tr><td width=125 height=23 valign=top>Vendor Email</td><td width=389 valign=top>" + PHVM.VENDORMAILID + "</td></tr>" +
                                        "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "/Login/EmailApproval/?Wid=" + struid + "&C=ICIBM&A=ICUserApproval&Tid=" + strid + " > Employee Portal</a> link to approve the request.</td></tr>" +
                                        //"<tr><td valign=top colspan=2>Please click on <a href=" + "https://m.portal.honda2wheelersindia.com" + " > E-Portal Mobile App</a> link to approve the request.</td></tr>" + //CR5820-TTL, Commented for ePortal Mobile
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
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }

        public short SendMailByApprovalAuthority_ASSET(long poHeaderId, short approvalStatus, Employee_Details employeeDetails)
        {
            short retVal = 0;
            try
            {
                string RequestStatus = approvalStatus == 1 ? "Approved" : approvalStatus == 2 ? "Send back" : approvalStatus == 3 ? "Rejected" : "";

                #region Send mail next approval authority
                ICASSETDISREQHEADER PHVM = _ICRepo.GetICAssetRequestById(poHeaderId);
                if (PHVM.ICAPPHISTORY.Count > 0 && approvalStatus == 1)
                {
                    VM_DGIT_ICAPPHISTORY Auth_obj = PHVM.ICAPPHISTORY.Where(a => a.ICAPPHISTORY_ID > 0 && a.APPROVAL_STATUS == 0 && !string.IsNullOrEmpty(a.APP_EMAIL)).FirstOrDefault();
                    if (Auth_obj != null)
                    {
                        var APPTYPE = PHVM.ICAPPAUTHSEQ.Where(x => x.ADEMPCODE == Convert.ToInt64(Auth_obj.APPEMP_CODE)).FirstOrDefault().APPTYPE;

                        commanEmail sendMail = new commanEmail();
                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        if (serverpath.isTestServer())
                            sendMail.MailTo = serverpath.getTestEMail();
                        else
                            sendMail.MailTo = Auth_obj.APP_EMAIL;
                        string struid = (Auth_obj.APPEMP_CODE).ToString();
                        string strid = poHeaderId.ToString();
                        string strSubject = "Investment Committee Approval Request from - " + PHVM.Emp_Detail._EName + ", Employee Code - " + PHVM.Emp_Detail._ECode;
                        string strBody = string.Empty;

                        strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                        "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>IC Request from " + PHVM.Emp_Detail._EName + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +
                                         "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + Auth_obj.APPEMP_NAME + " San,</br>" + PHVM.Emp_Detail._EName + " San has raised a IC Approval Request in Employee Portal. Below are the details :</td></tr>" +
                                        "<tr><td width=125 height=22 valign=top>IC Title</td><td width=389 valign=top>" + PHVM.ICTITLE + "</td></tr>" +
                                        "<tr><td width=125 height=23 valign=top>Department</td><td width=389 valign=top>" + PHVM.DEPARTMENT + "</td></tr>" +
                                        "<tr><td width=125 height=23 valign=top>Background</td><td width=389 valign=top>" + PHVM.ICBACKGROUND + "</td></tr>" +
                                  //"<tr><td width=125 height=23 valign=top>Vendor Email</td><td width=389 valign=top>" + PHVM.VENDORMAILID + "</td></tr>" +
                                  //"<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "Login/Index> Employee Portal</a> for approval process.</td></tr>" +
                                  "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "/Login/EmailApproval/?Wid=" + struid + "&C=ICIBM&A=ICUserApproval&Tid=" + strid + " > Employee Portal</a> link to approve the request.</td></tr>" +
                                  //"<tr><td valign=top colspan=2>Please click on <a href=" + "https://m.portal.honda2wheelersindia.com" + " > E-Portal Mobile App</a> link to approve the request.</td></tr>" + //CR5820-TTL, Commented for ePortal Mobile
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
                #endregion

                #region Send mail for requestor
                if (!string.IsNullOrEmpty(PHVM.Emp_Detail._EmailId))
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    sendMail.MailTo = PHVM.Emp_Detail._EmailId;

                    string strSubject = "Investment Committee Approval Status - " + RequestStatus;
                    string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Investment Committee Approval Status " + RequestStatus + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +

                                     "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                                     "<tr><td valign=top colspan =2>Your Investment Committee request has been <b>" + RequestStatus + "</b> by " + employeeDetails.Employee_Name + " San. The request details are as follows:</td></tr>" +
                                      "<tr><td width=125 height=22 valign=top>IC Title</td><td width=389 valign=top>" + PHVM.ICTITLE + "</td></tr>" +
                                      "<tr><td width=125 height=23 valign=top>Department</td><td width=389 valign=top>" + PHVM.DEPARTMENT + "</td></tr>" +
                                      "<tr><td width=125 height=23 valign=top>Background</td><td width=389 valign=top>" + PHVM.ICBACKGROUND + "</td></tr>" +
                                     //"<tr><td width=125 valign=top>Vendor Email</td><td width=389 valign=top>" + PHVM.VENDORMAILID + "</td></tr>" +
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

        public short SendMailByFinanceICReject(long poHeaderId, short approvalStatus, Employee_Details employeeDetails)
        {
            short retVal = 0;
            try
            {
                string RequestStatus = approvalStatus == 1 ? "Approved" : approvalStatus == 2 ? "Send back" : approvalStatus == 3 ? "Rejected" : "";
                ICREQHEADER PHVM = _ICRepo.GetICRequestById(poHeaderId);

                #region Send mail for requestor
                if (!string.IsNullOrEmpty(PHVM.Emp_Detail._EmailId))
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    sendMail.MailTo = PHVM.Emp_Detail._EmailId;

                    string strSubject = "Investment Committee Approval Status - " + RequestStatus;
                    string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Investment Committee Approval Status " + RequestStatus + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +

                                     "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                                     "<tr><td valign=top colspan =2>Your Investment Committee request has been <b>" + RequestStatus + "</b> by Finance - " + employeeDetails.Employee_Name + " San. The request details are as follows:</td></tr>" +
                                      "<tr><td width=125 height=22 valign=top>IC Title</td><td width=389 valign=top>" + PHVM.ICTITLE + "</td></tr>" +
                                      "<tr><td width=125 height=23 valign=top>Department</td><td width=389 valign=top>" + PHVM.DEPARTMENT + "</td></tr>" +
                                      "<tr><td width=125 height=23 valign=top>Background</td><td width=389 valign=top>" + PHVM.ICBACKGROUND + "</td></tr>" +
                                     //"<tr><td width=125 valign=top>Vendor Email</td><td width=389 valign=top>" + PHVM.VENDORMAILID + "</td></tr>" +
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
        public short SendMailByFinanceICReject_ASSET(long poHeaderId, short approvalStatus, Employee_Details employeeDetails)
        {
            short retVal = 0;
            try
            {
                string RequestStatus = approvalStatus == 1 ? "Approved" : approvalStatus == 2 ? "Send back" : approvalStatus == 3 ? "Rejected" : "";
                ICASSETDISREQHEADER PHVM = _ICRepo.GetICAssetRequestById(poHeaderId);

                #region Send mail for requestor
                if (!string.IsNullOrEmpty(PHVM.Emp_Detail._EmailId))
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    sendMail.MailTo = PHVM.Emp_Detail._EmailId;

                    string strSubject = "Investment Committee Approval Status - " + RequestStatus;
                    string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Investment Committee Approval Status " + RequestStatus + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +

                                     "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                                     "<tr><td valign=top colspan =2>Your Investment Committee request has been <b>" + RequestStatus + "</b> by Finance - " + employeeDetails.Employee_Name + " San. The request details are as follows:</td></tr>" +
                                      "<tr><td width=125 height=22 valign=top>IC Title</td><td width=389 valign=top>" + PHVM.ICTITLE + "</td></tr>" +
                                      "<tr><td width=125 height=23 valign=top>Department</td><td width=389 valign=top>" + PHVM.DEPARTMENT + "</td></tr>" +
                                      "<tr><td width=125 height=23 valign=top>Background</td><td width=389 valign=top>" + PHVM.ICBACKGROUND + "</td></tr>" +
                                     //"<tr><td width=125 valign=top>Vendor Email</td><td width=389 valign=top>" + PHVM.VENDORMAILID + "</td></tr>" +
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

        public short SendMailByFinanceAuthority(long poHeaderId, short approvalStatus, Employee_Details employeeDetails)
        {
            short retVal = 0;
            try
            {
                string RequestStatus = approvalStatus == 1 ? "Approved" : approvalStatus == 2 ? "Send back" : approvalStatus == 3 ? "Rejected" : "";
                ICREQHEADER PHVM = _ICRepo.GetICRequestById(poHeaderId);

                #region Send mail for requestor
                if (!string.IsNullOrEmpty(PHVM.Emp_Detail._EmailId))
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    sendMail.MailTo = PHVM.Emp_Detail._EmailId;

                    string strSubject = "Investment Committee Approval Status - " + RequestStatus;
                    string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Investment Committee Approval Status " + RequestStatus + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +

                                     "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                                     "<tr><td valign=top colspan =2>Your Investment Committee request has been <b>" + RequestStatus + "</b> by " + employeeDetails.Employee_Name + "(Finance) San. The request details are as follows:</td></tr>" +
                                      "<tr><td width=125 height=22 valign=top>IC Title</td><td width=389 valign=top>" + PHVM.ICTITLE + "</td></tr>" +
                                      "<tr><td width=125 height=23 valign=top>Department</td><td width=389 valign=top>" + PHVM.DEPARTMENT + "</td></tr>" +
                                      "<tr><td width=125 height=23 valign=top>Background</td><td width=389 valign=top>" + PHVM.ICBACKGROUND + "</td></tr>" +
                                     "<tr><td width=125 valign=top>IC Month</td><td width=389 valign=top>" + PHVM.iccycle.Value.ToString("MMM-yyyy") + "</td></tr>" +
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

        public short SendMailByFinanceAuthority_AUCCode(long poHeaderId)
        {
            short retVal = 0;
            try
            {
                //string RequestStatus = approvalStatus == 1 ? "Approved" : approvalStatus == 2 ? "Send back" : approvalStatus == 3 ? "Rejected" : "";
                ICREQHEADER PHVM = _ICRepo.GetICRequestById(poHeaderId);

                #region Send mail for requestor
                if (!string.IsNullOrEmpty(PHVM.Emp_Detail._EmailId))
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    sendMail.MailTo = PHVM.Emp_Detail._EmailId;
                    sendMail.MailCc = "mukesh.guljani@honda.hmsi.in";

                    string strSubject = "Investment Committee Approval Request - AUC Code has been updated by Finance";
                    string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Investment Committee Approval Request - AUC Code has been updated by Finance</font></b></td></tr>" +

                                     "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                                     "<tr><td valign=top colspan =2>Your Investment Committee request AUC Code has been updated by finance." + "The request details are as follows:</td></tr>" +
                                      "<tr><td width=125 height=22 valign=top>IC Title</td><td width=389 valign=top>" + PHVM.ICTITLE + "</td></tr>" +
                                      "<tr><td width=125 height=23 valign=top>Department</td><td width=389 valign=top>" + PHVM.DEPARTMENT + "</td></tr>" +
                                      "<tr><td width=125 height=23 valign=top>Background</td><td width=389 valign=top>" + PHVM.ICBACKGROUND + "</td></tr>" +
                                     "<tr><td width=125 valign=top>AUC Code</td><td width=389 valign=top>" + PHVM.AUCCODE + "</td></tr>" +
                                      "<tr><td width=125 valign=top>AUC Finance Remark</td><td width=389 valign=top>" + PHVM.AUCCODEREMARK + "</td></tr>" +
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
        public short SendMailByFinanceAuthority_ASSET(long poHeaderId, short approvalStatus, Employee_Details employeeDetails)
        {
            short retVal = 0;
            try
            {
                string RequestStatus = approvalStatus == 1 ? "Approved" : approvalStatus == 2 ? "Send back" : approvalStatus == 3 ? "Rejected" : "";
                ICASSETDISREQHEADER PHVM = _ICRepo.GetICAssetRequestById(poHeaderId);

                #region Send mail for requestor
                if (!string.IsNullOrEmpty(PHVM.Emp_Detail._EmailId))
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    sendMail.MailTo = PHVM.Emp_Detail._EmailId;

                    string strSubject = "Investment Committee Asset Disposal Approval Status - " + RequestStatus;
                    string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Investment Committee Asset Disposal Approval Status " + RequestStatus + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +

                                     "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                                     "<tr><td valign=top colspan =2>Your Investment Committee Asset Disposal request has been <b>" + RequestStatus + "</b> by " + employeeDetails.Employee_Name + "(Finance) San. The request details are as follows:</td></tr>" +
                                      "<tr><td width=125 height=22 valign=top>IC Title</td><td width=389 valign=top>" + PHVM.ICTITLE + "</td></tr>" +
                                      "<tr><td width=125 height=23 valign=top>Department</td><td width=389 valign=top>" + PHVM.DEPARTMENT + "</td></tr>" +
                                      "<tr><td width=125 height=23 valign=top>Background</td><td width=389 valign=top>" + PHVM.ICBACKGROUND + "</td></tr>" +
                                     "<tr><td width=125 valign=top>IC Month</td><td width=389 valign=top>" + PHVM.iccycle.Value.ToString("MMM-yyyy") + "</td></tr>" +
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
        public Tuple<short, List<VM_DGIT_ICDOCDETAIL>> SaveAssetAttachmentFinal(long addedDate, long ICHid, List<VM_DGIT_ICDOCDETAIL> modelList)
        {
            List<VM_DGIT_ICDOCDETAIL> iList = new List<VM_DGIT_ICDOCDETAIL>();
            iList = _ICRepo.GetAssetAttachmentDetail(ICHid);
            iList = iList.Where(m => m.DOC_TYPE == "ICDOC").ToList();
            short retVal = _ICRepo.SaveICAssetDetails(addedDate, ICHid, modelList);
            //if (retVal == 1)
            //{
            //    iList = _ICRepo.GetAssetAttachmentDetail(ICHid);
            //}
            Tuple<short, List<VM_DGIT_ICDOCDETAIL>> _tuple = new Tuple<short, List<VM_DGIT_ICDOCDETAIL>>(retVal, iList);
            return _tuple;
        }
        public Tuple<short, List<VM_DGIT_ICDOCDETAIL>> SaveAttachmentFinal(long addedDate, long ICHid, List<VM_DGIT_ICDOCDETAIL> modelList)
        {
            List<VM_DGIT_ICDOCDETAIL> iList = new List<VM_DGIT_ICDOCDETAIL>();
            iList = _ICRepo.GetAttachmentDetail(ICHid);
            iList = iList.Where(m => m.DOC_TYPE == "ICDOC").ToList();

            short retVal = _ICRepo.SaveICDetails(addedDate, ICHid, modelList);
            //if (retVal == 1)
            //{
            //    iList = _ICRepo.GetAttachmentDetail(ICHid);
            //}
            Tuple<short, List<VM_DGIT_ICDOCDETAIL>> _tuple = new Tuple<short, List<VM_DGIT_ICDOCDETAIL>>(retVal, iList);
            return _tuple;
        }

        public short ICReqCancel(VM_DGIT_ICAPPHISTORY PHVM)
        {
            short retVal = _ICRepo.ICReqCancel(PHVM);
            return retVal;
        }
        public short ICAssetReqCancel(VM_DGIT_ICAPPHISTORY PHVM)
        {
            short retVal = _ICRepo.ICAssetReqCancel(PHVM);
            return retVal;
        }
        public VM_ICIBM_PPCDASHBOARD_Search GetPPCDashboard(VM_ICIBM_PPCDASHBOARD_Search obj)
        {
            return _ICRepo.GetPPCDashboard(obj);
        }
        public List<VM_SELECTITEMLIST> GetICInvestdtls()
        {
            return _ICRepo.ICInvestdtls();
        }

    }
}
