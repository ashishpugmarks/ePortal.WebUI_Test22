
using ePortal.ViewModels;
using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;
using ePortal.Shared;
using ePortal.Shared.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ePortal.Application.Services
{
    public class POService : IPOService
    {
        private readonly PORepository _PORepo;
        private readonly CommonRepository _CommonRepo;
        Tuple<short, long> _retVal_tuple;
        private readonly IAppConfigurationService _env;

        //public POService()
        //{
        //    _PORepo = new PORepository();
        //    _CommonRepo = new CommonRepository();
        //    _retVal_tuple = new Tuple<short, long>((short)0, 0L);
        //}
        public POService(PORepository PORepo, CommonRepository CommonRepo, IAppConfigurationService env)
        {
            _PORepo = PORepo;
            _CommonRepo = CommonRepo;
            _env = env;
            _retVal_tuple = new Tuple<short, long>((short)0, 0L);
        }
        public List<VendorViewModel> GetVendorList()
        {
            return _PORepo.GetVendorList();
        }        

        //Below added by aumento as on 02092024 for SR70820================================================================
        //public List<Employee_Details> GetAuthEmpById(long empCode)
        //{
        //    return _PORepo.GetAuthEmpById(empCode);
        //}


        public Employee_Details GetAuthEmpById(long empCode, long loginempcode)
        {
            return _PORepo.GetAuthEmpById(empCode, loginempcode);
        }
        //====================================================================================================================

        public POHeaderViewModel GetPORequestById(long id)
        {
            return _PORepo.GetPORequestById(id);
        }

        //public short POApproval(POAppHistoryViewModel PHVM)
        //{
        //    return _PORepo.POApproval(PHVM);
        //}
        public short POApproval(POAppHistoryViewModel PHVM, Employee_Details emp_dtl)
        {
            short retVal = _PORepo.POApproval(PHVM);
            if (retVal == 1)
            {   // Mail Functionality Stop as per requirement 
                // SendMailByApprovalAuthority(PHVM.POID, PHVM.APPROVAL_STATUS, emp_dtl);
            }
            return retVal;
        }

        public short POCancel(POHeaderViewModel PHVM)
        {
            return _PORepo.POCancel(PHVM);
        }

        //public Tuple<short, long> SavePORequest(POHeaderViewModel model)
        //{
        //    return _PORepo.SavePORequest(model);
        //}
        public Tuple<short, long> SavePORequest(POHeaderViewModel model)
        {
            Tuple<short, long> _tuple = _PORepo.SavePORequest(model);
            //if (_tuple.Item1 == 1 && _tuple.Item2 > 0 && model.IsFinalSubmit == 1)
            //{
            //    SendMailByRequestor(_tuple.Item2);
            //}
            return _tuple;
        }
        public List<POAppAuthSeqViewModel> GetDefaultAuthority(long loginUser)
        {
            List<POAppAuthSeqViewModel> AuthList = new List<POAppAuthSeqViewModel>();
            try
            {
                UserApprovalAuthority Obj = _CommonRepo.CheckApprovalAuthority(loginUser);
                if (Obj != null)
                {
                    if (Obj.SectionManager != 0 && Obj.SectionManager != null)
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.SectionManager));
                        AuthList.Add(new POAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = string.IsNullOrEmpty(Emp_Dtl._FnDesig) ? Emp_Dtl._Desig : Emp_Dtl._FnDesig,
                            APP_SEQ = Convert.ToInt16(AuthList.Count + 1),
                            APPTYPE = 1,
                            FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                        });
                    }
                    if (Obj.DepartmentManager != 0 && Obj.DepartmentManager != null && Obj.DepartmentManager != loginUser)
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.DepartmentManager));
                        AuthList.Add(new POAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = string.IsNullOrEmpty(Emp_Dtl._FnDesig) ? Emp_Dtl._Desig : Emp_Dtl._FnDesig,
                            APP_SEQ = Convert.ToInt16(AuthList.Count + 1),
                            APPTYPE = 1,
                            FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                        });
                    }
                    if (Obj.Coordinator != 0 && Obj.Coordinator != null && Obj.Coordinator != loginUser)
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.Coordinator));
                        AuthList.Add(new POAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = string.IsNullOrEmpty(Emp_Dtl._FnDesig) ? Emp_Dtl._Desig : Emp_Dtl._FnDesig,
                            APP_SEQ = Convert.ToInt16(AuthList.Count + 1),
                            APPTYPE = 1,
                            FNDESID = 2,
                        });
                    }
                    if (Obj.DivisionHead != 0 && Obj.DivisionHead != null && Obj.DivisionHead != loginUser)
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.DivisionHead));
                        AuthList.Add(new POAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = string.IsNullOrEmpty(Emp_Dtl._FnDesig) ? Emp_Dtl._Desig : Emp_Dtl._FnDesig,
                            APP_SEQ = Convert.ToInt16(AuthList.Count + 1),
                            APPTYPE = 1,
                            FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                        });
                    }
                    if (Obj.EXECoordinator != 0 && Obj.EXECoordinator != null && Obj.EXECoordinator != loginUser)
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.EXECoordinator));
                        AuthList.Add(new POAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = string.IsNullOrEmpty(Emp_Dtl._FnDesig) ? Emp_Dtl._Desig : Emp_Dtl._FnDesig,
                            APP_SEQ = Convert.ToInt16(AuthList.Count + 1),
                            APPTYPE = 1,
                            FNDESID = 4,
                        });
                    }
                    if (Obj.OperationHead != 0 && Obj.OperationHead != null && Obj.OperationHead != loginUser)
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.OperationHead));
                        AuthList.Add(new POAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = string.IsNullOrEmpty(Emp_Dtl._FnDesig) ? Emp_Dtl._Desig : Emp_Dtl._FnDesig,
                            APP_SEQ = Convert.ToInt16(AuthList.Count + 1),
                            APPTYPE = 1,
                            FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                        });
                    }

                    if (Obj.Director != 0 && Obj.Director != null && Obj.Director != loginUser)
                    {
                        short atype;
                        Employee_Details Emp_Dtl = _CommonRepo.GetDirectorDetailById(Convert.ToInt64(Obj.Director), Convert.ToInt64(Obj.OperationId), out atype);
                        //if (atype != 2)
                        //{
                        AuthList.Add(new POAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = string.IsNullOrEmpty(Emp_Dtl._FnDesig) ? Emp_Dtl._Desig : Emp_Dtl._FnDesig,
                            APP_SEQ = Convert.ToInt16(AuthList.Count + 1),
                            APPTYPE = atype,
                            FNDESID = 6,
                        });
                        // }
                    }
                    if (Obj.Director2 != 0 && Obj.Director2 != null && Obj.Director2 != loginUser)
                    {
                        short atype;
                        Employee_Details Emp_Dtl = _CommonRepo.GetDirectorDetailById(Convert.ToInt64(Obj.Director2), Convert.ToInt64(Obj.OperationId), out atype);
                        //if (atype != 2)
                        //{
                        AuthList.Add(new POAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = string.IsNullOrEmpty(Emp_Dtl._FnDesig) ? Emp_Dtl._Desig : Emp_Dtl._FnDesig,
                            APP_SEQ = Convert.ToInt16(AuthList.Count + 1),
                            APPTYPE = atype,
                            FNDESID = 7,
                        });
                        // }
                    }


                }
            }
            catch (Exception ex)
            {
                AuthList = new List<POAppAuthSeqViewModel>();
            }
            AuthList = AuthList.OrderBy(n => n.APP_SEQ).OrderBy(m => m.APPTYPE).ToList();
            for (Int16 i = 0; i <= AuthList.Count - 1; i++)
            {
                AuthList[i].APP_SEQ = Convert.ToInt16(i + 1);
            }
            return AuthList;
        }

        public Tuple<short, List<PODetailViewModel>> SaveAttachment(long addedDate, long poHid, List<PODetailViewModel> modelList)
        {
            List<PODetailViewModel> iList = new List<PODetailViewModel>();
            short retVal = _PORepo.SavePODetails(addedDate, poHid, modelList);
            if (retVal == 1)
            {
                iList = _PORepo.GetAttachmentDetail(poHid);
            }
            Tuple<short, List<PODetailViewModel>> _tuple = new Tuple<short, List<PODetailViewModel>>(retVal, iList);
            return _tuple;
        }

        public Tuple<short, List<PODetailViewModel>> DeleteAttachment(string fileName, string docType, long poHid)
        {
            List<PODetailViewModel> iList = new List<PODetailViewModel>();
            short retVal = _PORepo.DeleteAttachment(fileName, docType, poHid);
            if (retVal == 1)
            {
                iList = _PORepo.GetAttachmentDetail(poHid);
            }
            Tuple<short, List<PODetailViewModel>> _tuple = new Tuple<short, List<PODetailViewModel>>(retVal, iList);
            return _tuple;
        }

        public short SendMailByRequestor(long poHeaderId)
        {
            short retVal = 0;
            try
            {
                POHeaderViewModel PHVM = _PORepo.GetPORequestById(poHeaderId);
                if (PHVM.poAppHis.Count > 0)
                {
                    POAppHistoryViewModel Auth_obj = PHVM.poAppHis.Where(a => a.POAPPHISTORY_ID > 0 && a.APPROVAL_STATUS == 0 && !string.IsNullOrEmpty(a.APP_EMAIL)).FirstOrDefault();
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

                        string strSubject = "PO Request from - " + PHVM.Emp_Detail._EName + ", Employee Code - " + PHVM.Emp_Detail._ECode;
                        string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                         "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>PO Request from " + PHVM.Emp_Detail._EName + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +

                                         "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + Auth_obj.APPEMP_NAME + " San ,</br></br>" + PHVM.Emp_Detail._EName + " San has raised a PO Approval Request in Employee Portal. Below are the details :</td></tr>" +
                                         "<tr><td width=125 height=22 valign=top>PO Number</td><td width=389 valign=top>" + PHVM.PONO + "</td></tr>" +
                                         "<tr><td width=125 height=23 valign=top>Vendor Code</td><td width=389 valign=top>" + PHVM.VENDORID + "</td></tr>" +
                                         "<tr><td width=125 height=23 valign=top>Vendor Name</td><td width=389 valign=top>" + PHVM.VENDORNAME + "</td></tr>" +
                                         "<tr><td width=125 height=23 valign=top>Vendor Email</td><td width=389 valign=top>" + PHVM.VENDORMAILID + "</td></tr>" +
                                         "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "/Login/EmailApproval/?Wid=" + struid + "&C=PO&A=POApproval&Tid=" + strid + " > Employee Portal</a> for approval process.</td></tr>" +
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

        public short SendMailByApprovalAuthority(long poHeaderId, short approvalStatus, Employee_Details employeeDetails)
        {
            short retVal = 0;
            try
            {
                string RequestStatus = approvalStatus == 1 ? "Approved" : approvalStatus == 2 ? "Send back" : approvalStatus == 3 ? "Rejected" : "";

                #region Send mail next approval authority
                POHeaderViewModel PHVM = _PORepo.GetPORequestById(poHeaderId);
                if (PHVM.poAppHis.Count > 0 && approvalStatus == 1)
                {
                    POAppHistoryViewModel Auth_obj = PHVM.poAppHis.Where(a => a.POAPPHISTORY_ID > 0 && a.APPROVAL_STATUS == 0 && !string.IsNullOrEmpty(a.APP_EMAIL)).FirstOrDefault();
                    if (Auth_obj != null)
                    {
                        var APPTYPE = PHVM.poAuthSeq.Where(x => x.ADEMPCODE == Convert.ToInt64(Auth_obj.APPEMP_CODE)).FirstOrDefault().APPTYPE;

                        commanEmail sendMail = new commanEmail();
                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        if (serverpath.isTestServer())
                            sendMail.MailTo = serverpath.getTestEMail();
                        else
                            sendMail.MailTo = Auth_obj.APP_EMAIL;
                        string struid = (Auth_obj.APPEMP_CODE).ToString();
                        string strid = poHeaderId.ToString();
                        string strSubject = "PO Approval Request from - " + PHVM.Emp_Detail._EName + ", Employee Code - " + PHVM.Emp_Detail._ECode;
                        string strBody = string.Empty;
                        if (APPTYPE == 2)
                        {
                            strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                            "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>PO Request from " + PHVM.Emp_Detail._EName + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +

                                            "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear" + Auth_obj.APPEMP_NAME + " San,</br>" + PHVM.Emp_Detail._EName + " San has raised a PO Approval Request in Employee Portal. Below are the details :</td></tr>" +
                                            "<tr><td width=125 height=22 valign=top>PO Number</td><td width=389 valign=top>" + PHVM.PONO + "</td></tr>" +
                                            "<tr><td width=125 height=23 valign=top>Vendor Code</td><td width=389 valign=top>" + PHVM.VENDORID + "</td></tr>" +
                                            "<tr><td width=125 height=23 valign=top>Vendor Name</td><td width=389 valign=top>" + PHVM.VENDORNAME + "</td></tr>" +
                                            "<tr><td width=125 height=23 valign=top>Vendor Email</td><td width=389 valign=top>" + PHVM.VENDORMAILID + "</td></tr>" +
                                            //"<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "Login/Index> Employee Portal</a> for approval process.</td></tr>" +
                                            "<tr><td valign=top colspan=2>To digital sign the PO, please insert  digital token in your laptop and click on <a href=" + @"file://D:\WindowApp\DigitalSignatureWindowApp.exe" + "> PO Approval</a> link.</td></tr>" +
                           "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";
                        }
                        else
                        {
                            strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                      "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>PO Approval Request from " + PHVM.Emp_Detail._EName + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +
                                      // "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td width=125 height=22 valign=top>PO Number</td><td width=389 valign=top>" + PHVM.PONO + "</td></tr>" +
                                      "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + Auth_obj.APPEMP_NAME + " San ,</br></br>" + PHVM.Emp_Detail._EName + " San has raised a PO Approval Request in Employee Portal. Below are the details :</td></tr>" +
                                       "<tr><td width=125 height=22 valign=top>PO Number</td><td width=389 valign=top>" + PHVM.PONO + "</td></tr>" +
                                      "<tr><td width=125 height=23 valign=top>Vendor Code</td><td width=389 valign=top>" + PHVM.VENDORID + "</td></tr>" +
                                      "<tr><td width=125 height=23 valign=top>Vendor Name</td><td width=389 valign=top>" + PHVM.VENDORNAME + "</td></tr>" +
                                      "<tr><td width=125 height=23 valign=top>Vendor Email</td><td width=389 valign=top>" + PHVM.VENDORMAILID + "</td></tr>" +
                                      //"<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "Login/Index> Employee Portal</a> for approval process.</td></tr>" +
                                      "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "/Login/EmailApproval/?Wid=" + struid + "&C=PO&A=POApproval&Tid=" + strid + " > Employee Portal</a> link to approve the request.</td></tr>" +
                                      //"<tr><td valign=top colspan=2>Please click on <a href=" + "https://m.portal.honda2wheelersindia.com" + " > E-Portal Mobile App</a> link to approve the request.</td></tr>" + //CR5820-TTL, Commented for ePortal Mobile
                                      "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";
                        }

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

                    string strSubject = "PO Approval Status - " + RequestStatus;
                    string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>PO Approval Status " + RequestStatus + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +

                                     "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                                     "<tr><td valign=top colspan =2>Your PO request has been <b>" + RequestStatus + "</b> by " + employeeDetails.Employee_Name + " San. The request details are as follows:</td></tr>" +
                                     "<tr><td width=125 height=22 valign=top>PO Number</td><td width=389 valign=top>" + PHVM.PONO + "</td></tr>" +
                                     "<tr><td width=125 height=23 valign=top>Vendor Code</td><td width=389 valign=top>" + PHVM.VENDORID + "</td> </tr>" +
                                     "<tr><td width=125 valign=top>Vendor Name</td><td width=389 valign=top>" + PHVM.VENDORNAME + "</td></tr>" +
                                     "<tr><td width=125 valign=top>Vendor Email</td><td width=389 valign=top>" + PHVM.VENDORMAILID + "</td></tr>" +
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

        public PRHeaderViewModel GetPRDetailByPOId(long poid)
        {
            return _PORepo.GetPRDetailByPOId(poid);
        }

        public long GetPRStatusByPOId(string[] pono)
        {
            return _PORepo.GetPRStatusByPOId(pono);
        }
        public long GetPONextApprovalId(long poid, long ecode)
        {
            return _PORepo.GetPONextApprovalId(poid, ecode);
        }
        public Search_VW_PODASHBOARD_VWMODEL GetPODASHBOARD(Search_VW_PODASHBOARD_VWMODEL objSearch)
        {
            return _PORepo.GetPODASHBOARD(objSearch);
        }

        public short SentMailToVendor(long poHeaderId)
        {
            short retVal = 0;
            try
            {

                #region Send mail next approval authority
                POHeaderViewModel PHVM = _PORepo.GetPORequestById(poHeaderId);
                PODetailViewModel pd = PHVM.poDetail.Where(m => m.DOC_TYPE == "POA").FirstOrDefault();
                string strCCMail = "";
                if (PHVM.poAppHis.Count > 0 && PHVM.PROCESS_STATUS == 2 && pd != null && !string.IsNullOrEmpty(PHVM.VENDORMAILID))
                {
                    strCCMail = PHVM.Emp_Detail.EMail_Id + ",";
                    List<POAppHistoryViewModel> Auth_obj = PHVM.poAppHis.Where(a => !string.IsNullOrEmpty(a.APP_EMAIL) && a.APPROVAL_STATUS == 1).OrderBy(m => m.POAPPHISTORY_ID).ToList();

                    foreach (var obj in Auth_obj.Take(2))
                    {
                        strCCMail = strCCMail + obj.APP_EMAIL;
                    }
                    if (Auth_obj != null)
                    {
                        //ePortal.Core.EmailCore sendMail = new ePortal.Core.EmailCore();
                        EmailCore sendMail = new EmailCore();
                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        if (serverpath.isTestServer())
                            sendMail.MailTo = serverpath.getTestEMail();
                        else
                            sendMail.MailTo = PHVM.VENDORMAILID;

                        sendMail.MailCc = strCCMail;
                        //string struid = (Auth_obj.APPEMP_CODE).ToString();
                        string strid = poHeaderId.ToString();
                        string strSubject = "Approved PO Copy : PO NO. - " + PHVM.PONO;
                        string strBody = string.Empty;
                        strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                        "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF></font></b></td></tr>" +
                                        "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Hi," + "</br>" + " Please find the digitally approved PO copy . PO details are as follows:</td></tr>" +
                                        "<tr><td width=125 height=22 valign=top>PO Number</td><td width=389 valign=top>" + PHVM.PONO + "</td></tr>" +
                                        "<tr><td width=125 height=23 valign=top>Amendment No.</td><td width=389 valign=top>" + PHVM.VERSIONNO + "</td></tr>" +
                       "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                        sendMail.MailSubject = strSubject;
                        sendMail.MailBody = strBody;
                        sendMail.attContentType = "application/pdf";
                        //string path = HttpContext.Current.Server.MapPath("~/Uploads/PO/" + pd.FILENAME);
                        string path = _env.GetGeneralSettings().Get_FileUpload_Path + "/PO/" + pd.FILENAME;
                        sendMail.Mailattachement = new FileStream(path, FileMode.Open, FileAccess.Read);//File.ReadAllBytes(path);
                        sendMail.attachementName = pd.FILENAME;

                        try
                        {
                            bool status = sendMail.Send();
                            retVal=_PORepo.UpdateMailCNT(PHVM.POHEADERID);
                        }
                        catch (Exception ex)
                        {
                            retVal = -1;
                        }
                        finally
                        {
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

        public SearchPO POALLUserReport(SearchPO VM)
        {
            return _PORepo.POALLUserReport(VM);
        }
        public Tuple<long, List<PR_Div_Dep_SecViewModel>> BindDivision(long op_Id, long LoginEcode, long kiid)
        {
            return _PORepo.BindDivision(LoginEcode, kiid, op_Id);
        }

        public Tuple<long, List<PR_Div_Dep_SecViewModel>> BindOperation(long Loginempcode, long KIID)
        {
            return _PORepo.BindOperation(Loginempcode, KIID);
        }

        public Tuple<long, List<PR_Div_Dep_SecViewModel>> BindDepartment(long Loginempcode, long KIID, long op_Id, long Div_id)
        {
            return _PORepo.BindDepartment(Loginempcode, KIID, op_Id, Div_id);
        }

        public Tuple<long, List<PR_Div_Dep_SecViewModel>> BindSection(long Loginempcode, long KIID, long op_Id, long divid, long deptid)
        {
            return _PORepo.BindSection(Loginempcode, KIID, op_Id, divid, deptid);
        }

        public List<PR_Div_Dep_SecViewModel> GetKiLIST(long ecode)
        {
            return _PORepo.GetKiLIST(ecode);
        }

        public Employee_Details GetEmployeeDetail(long ecode, long KIID)
        {
            return _PORepo.GetEmployeeDetail(ecode, KIID);
        }
        // added by aumento for SR82542
        public List<POAppAuthSeqViewModel> GetPOAuthorityHis_ById(long id, long loginUser)
        {
            return _PORepo.GetPOAuthorityHis_ById(id, loginUser);           
        }
        // added by aumento for SR82542
    }
}
