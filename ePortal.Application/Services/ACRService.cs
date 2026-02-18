using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.Infrastructure.Repositories;
using ePortal.Shared;
using ePortal.ViewModels;
using Microsoft.Extensions.Configuration;

namespace ePortal.Application.Services
{
    public class ACRService : IACRService
    {
      private  ACRRepository _ACRRepo;
        private CommonRepository _CommonRepo;
        private readonly IConfiguration _settings;
        //private EmailCore _sendMail;
        Tuple<short, long> _retVal_tuple;

        public ACRService(ACRRepository objACRRepository, CommonRepository objCommonRepository, IConfiguration objIConfiguration)
        {
            _ACRRepo = objACRRepository;
            _CommonRepo = objCommonRepository;
            //_sendMail = objcommanEmail;
            _retVal_tuple = new Tuple<short, long>((short)0, 0L);
            _settings = objIConfiguration;
        }


        public List<VendorViewModel> GetVendorList()
        {
            return _ACRRepo.GetVendorList();
        }

        //public Employee_Details GetAuthEmpById(long empCode)
        //{
        //    return _ACRRepo.GetAuthEmpById(empCode);
        //}

        public Employee_Details GetAuthEmpById(long empCode, int designationId, string designation, Employee_Details empDtl)
        {
            return _ACRRepo.GetAuthEmpById(empCode, designationId, designation, empDtl);
        }
        //============Change Done on For Add Other Category By (Aumento)==========================================================================================
        public Employee_Details GetOAuthEmpById(long empCode, int designationId, string designation, Employee_Details empDtl)
        {
            return _ACRRepo.GetOAuthEmpById(empCode, designationId, designation, empDtl);
        }
        //=====================================================================================================================================================================


        public ACRHeaderViewModel GetACRRequestById(long id)
        {
            return _ACRRepo.GetACRRequestById(id);
        }

        public short ACRApproval(ACRAppHistoryViewModel PHVM, Employee_Details emp_dtl)
        {
            short retVal = _ACRRepo.ACRApproval(PHVM);
            if (retVal == 1)
            {
                _sendMailByApprovalAuthority(PHVM.ACRID, PHVM.APPROVAL_STATUS, emp_dtl);
            }
            return retVal;
        }

        public short ACRCancel(ACRHeaderViewModel PHVM)
        {
            return _ACRRepo.ACRCancel(PHVM);
        }

        public Tuple<short, long> SaveACRRequest(ACRHeaderViewModel model)
        {
            Tuple<short, long> _tuple = _ACRRepo.SaveACRRequest(model);
            if (_tuple.Item1 == 1 && _tuple.Item2 > 0 && model.IsFinalSubmit == 1)
            {
                _sendMailByRequestor(_tuple.Item2);
            }
            return _tuple;
        }
        public List<ACRAppAuthSeqViewModel> GetDefaultAuthority(long loginUser)
        {
            List<ACRAppAuthSeqViewModel> AuthList = new List<ACRAppAuthSeqViewModel>();
            try
            {
                UserApprovalAuthority Obj = _CommonRepo.CheckApprovalAuthority(loginUser);
                if (Obj != null)
                {
                    if (Obj.SectionManager != 0 && Obj.SectionManager != null)
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.SectionManager));
                        AuthList.Add(new ACRAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = Emp_Dtl._Desig,
                            APP_SEQ = 1,
                            APPTYPE = 1,
                        });
                    }
                    if (Obj.DepartmentManager != 0 && Obj.DepartmentManager != null)
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.DepartmentManager));
                        AuthList.Add(new ACRAppAuthSeqViewModel
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
                        AuthList.Add(new ACRAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = Emp_Dtl._Desig,
                            APP_SEQ = 3,
                            APPTYPE = 1,
                            FNDESID = 2,
                        });
                    }
                    if (Obj.DivisionHead != 0 && Obj.DivisionHead != null && Obj.DivisionHead != loginUser)
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.DivisionHead));
                        AuthList.Add(new ACRAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = Emp_Dtl._Desig,
                            APP_SEQ = 4,
                            APPTYPE = 1,
                            FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                        });
                    }
                    if (Obj.EXECoordinator != 0 && Obj.EXECoordinator != null && Obj.EXECoordinator != loginUser)
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.EXECoordinator));
                        AuthList.Add(new ACRAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = Emp_Dtl._Desig,
                            APP_SEQ = 5,
                            APPTYPE = 1,
                            FNDESID = 4,
                        });
                    }
                    if (Obj.OperationHead != 0 && Obj.OperationHead != null && Obj.OperationHead != loginUser)
                    {
                        Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.OperationHead));
                        AuthList.Add(new ACRAppAuthSeqViewModel
                        {
                            ADEMPCODE = Emp_Dtl._ECode,
                            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                            ADDESIGNATION = Emp_Dtl._Desig,
                            APP_SEQ = 6,
                            APPTYPE = 1,
                            FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                        });
                    }
                    //if (Obj.Director != 0 && Obj.Director != null && Obj.Director != loginUser)
                    //{
                    //    short atype;
                    //    Employee_Details Emp_Dtl = _CommonRepo.GetDirectorDetailById(Convert.ToInt64(Obj.Director), Convert.ToInt64(Obj.OperationId), out atype);
                    //    if (atype != 2)
                    //    {
                    //        AuthList.Add(new ACRAppAuthSeqViewModel
                    //        {
                    //            ADEMPCODE = Emp_Dtl._ECode,
                    //            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                    //            ADDESIGNATION = Emp_Dtl._Desig,
                    //            APP_SEQ = Convert.ToInt16(AuthList.Count + 1),
                    //            APPTYPE = atype,
                    //            FNDESID = 6,
                    //        });
                    //    }
                    //}
                    //if (Obj.Director2 != 0 && Obj.Director2 != null && Obj.Director2 != loginUser)
                    //{
                    //    short atype;
                    //    Employee_Details Emp_Dtl = _CommonRepo.GetDirectorDetailById(Convert.ToInt64(Obj.Director2), Convert.ToInt64(Obj.OperationId), out atype);
                    //    if (atype != 2)
                    //    {
                    //        AuthList.Add(new ACRAppAuthSeqViewModel
                    //        {
                    //            ADEMPCODE = Emp_Dtl._ECode,
                    //            ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                    //            ADDESIGNATION = Emp_Dtl._Desig,
                    //            APP_SEQ = Convert.ToInt16(AuthList.Count + 1),
                    //            APPTYPE = atype,
                    //            FNDESID = 7,
                    //        });
                    //    }
                    //}

                }
            }
            catch
            {
                AuthList = new List<ACRAppAuthSeqViewModel>();
            }
            return AuthList.OrderBy(m => m.APPTYPE).OrderBy(n => n.APP_SEQ).ToList();
        }

        public Tuple<short, List<ACRDetailViewModel>> SaveAttachment(long addedDate, long poHid, List<ACRDetailViewModel> modelList)
        {
            List<ACRDetailViewModel> iList = new List<ACRDetailViewModel>();
            short retVal = _ACRRepo.SaveACRDetails(addedDate, poHid, modelList);
            if (retVal == 1)
            {
                iList = _ACRRepo.GetAttachmentDetail(poHid);
            }
            Tuple<short, List<ACRDetailViewModel>> _tuple = new Tuple<short, List<ACRDetailViewModel>>(retVal, iList);
            return _tuple;
        }

        public Tuple<short, List<ACRDetailViewModel>> DeleteAttachment(string fileName, string docType, long poHid)
        {
            List<ACRDetailViewModel> iList = new List<ACRDetailViewModel>();
            short retVal = _ACRRepo.DeleteAttachment(fileName, docType, poHid);
            if (retVal == 1)
            {
                iList = _ACRRepo.GetAttachmentDetail(poHid);
            }
            Tuple<short, List<ACRDetailViewModel>> _tuple = new Tuple<short, List<ACRDetailViewModel>>(retVal, iList);
            return _tuple;
        }

        public short _sendMailByRequestor(long poHeaderId)
        {
            short retVal = 0;
            try
            {
                EmailCore sendMail = new EmailCore();
                ACRHeaderViewModel PHVM = _ACRRepo.GetACRRequestById(poHeaderId);
                if (PHVM.acrAppHis.Count > 0)
                {
                    ACRAppHistoryViewModel Auth_obj = PHVM.acrAppHis.Where(a => a.ACRAPPHISTORY_ID > 0 && a.APPROVAL_STATUS == 0 && !string.IsNullOrEmpty(a.APP_EMAIL)).FirstOrDefault();
                    if (Auth_obj != null)
                    {
                       
                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        if (serverpath.isTestServer())
                            sendMail.MailTo = serverpath.getTestEMail();
                        else
                            sendMail.MailTo = Auth_obj.APP_EMAIL;
                        string struid = (Auth_obj.APPEMP_CODE).ToString();
                        string strid = poHeaderId.ToString();

                        string strSubject = "ACR Request from - " + PHVM.Emp_Detail._EName + ", Employee Code - " + PHVM.Emp_Detail._ECode;
                        string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                         "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>ACR Request from " + PHVM.Emp_Detail._EName + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +

                                         "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + Auth_obj.APPEMP_NAME + " San ,</br></br>" + PHVM.Emp_Detail._EName + " San has raised a ACR Approval Request in Employee Portal. Below are the details :</td></tr>" +
                                         "<tr><td width=125 height=22 valign=top>ACR Number</td><td width=389 valign=top>" + PHVM.ACRNO + "</td></tr>" +
                                         "<tr><td width=125 height=23 valign=top>Amount</td><td width=389 valign=top>" + PHVM.AMOUNT + "</td></tr>" +
                                         "<tr><td width=125 height=23 valign=top>PO Number</td><td width=389 valign=top>" + PHVM.PONO + "</td></tr>" +
                                         "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "/Login/EmailApproval/?Wid=" + struid + "&C=ACR&A=ACRApproval&Tid=" + strid + " > Employee Portal</a> for approval process.</td></tr>" +
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

        public short _sendMailByApprovalAuthority(long poHeaderId, short approvalStatus, Employee_Details employeeDetails)
        {
            short retVal = 0;
            try
            {
                EmailCore sendMail = new EmailCore();
                string RequestStatus = approvalStatus == 1 ? "Approved" : approvalStatus == 2 ? "Send back" : approvalStatus == 3 ? "Rejected" : "";

                #region Send mail next approval authority
                ACRHeaderViewModel PHVM = _ACRRepo.GetACRRequestById(poHeaderId);
                if (PHVM.acrAppHis.Count > 0 && approvalStatus == 1)
                {
                    ACRAppHistoryViewModel Auth_obj = PHVM.acrAppHis.Where(a => a.ACRAPPHISTORY_ID > 0 && a.APPROVAL_STATUS == 0 && !string.IsNullOrEmpty(a.APP_EMAIL)).FirstOrDefault();
                    if (Auth_obj != null)
                    {
                       
                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        if (serverpath.isTestServer())
                            sendMail.MailTo = serverpath.getTestEMail();
                        else
                            sendMail.MailTo = Auth_obj.APP_EMAIL;
                        string struid = (Auth_obj.APPEMP_CODE).ToString();
                        string strid = poHeaderId.ToString();
                        string strSubject = "Advance Cheque Request Approval Request from - " + PHVM.Emp_Detail._EName + ", Employee Code - " + PHVM.Emp_Detail._ECode;
                        string strBody = string.Empty;
                        // if (struid == "70000167")
                        // { 
                        //      strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                        //                      "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>PO Request from " + PHVM.Emp_Detail._EName + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +

                        //                      "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear" + Auth_obj.APPEMP_NAME + " San,</br>" + PHVM.Emp_Detail._EName + " San has raised a PO Approval Request in Employee Portal. Below are the details :</td></tr>" +
                        //                      "<tr><td width=125 height=22 valign=top>PO Number</td><td width=389 valign=top>" + PHVM.PONO + "</td></tr>" +
                        //                      "<tr><td width=125 height=23 valign=top>Vendor Code</td><td width=389 valign=top>" + PHVM.VENDORID + "</td></tr>" +
                        //                      "<tr><td width=125 height=23 valign=top>Vendor Name</td><td width=389 valign=top>" + PHVM.VENDORNAME + "</td></tr>" +
                        //                      "<tr><td width=125 height=23 valign=top>Vendor Email</td><td width=389 valign=top>" + PHVM.VENDORMAILID + "</td></tr>" +
                        //                      //"<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "Login/Index> Employee Portal</a> for approval process.</td></tr>" +
                        //                      "<tr><td valign=top colspan=2>To digital sign the PO, please insert  digital token in your laptop and click on <a href=" + @"file://E:\PO\DigitalSignatureWindowApp.exe"+ "> PO Approval</a> link.</td></tr>" +
                        //     "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";
                        //}
                        // else
                        // {
                        strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                 "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Advance Cheque Request Approval Request from " + PHVM.Emp_Detail._EName + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +

                                 // "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td width=125 height=22 valign=top>PO Number</td><td width=389 valign=top>" + PHVM.PONO + "</td></tr>" +
                                 "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + Auth_obj.APPEMP_NAME + " San ,</br></br>" + PHVM.Emp_Detail._EName + " San has raised a Advance Cheque Request Approval Request in Employee Portal. Below are the details :</td></tr>" +
                                  "<tr><td width=125 height=22 valign=top>ACR Number</td><td width=389 valign=top>" + PHVM.ACRNO + "</td></tr>" +
                                 "<tr><td width=125 height=23 valign=top>Amount</td><td width=389 valign=top>" + PHVM.AMOUNT + "</td></tr>" +
                                 "<tr><td width=125 height=23 valign=top>PO Number</td><td width=389 valign=top>" + PHVM.PONO + "</td></tr>" +
                                 //"<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "Login/Index> Employee Portal</a> for approval process.</td></tr>" +
                                 "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "/Login/EmailApproval/?Wid=" + struid + "&C=ACR&A=ACRApproval&Tid=" + strid + " > Employee Portal</a> link to approve the request.</td></tr>" +
                                 "<tr><td colspan=2>Note: It is a system generated Email, please do not reply.</td></tr></table></td></tr></table>";

                        //}

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
                   
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    sendMail.MailTo = PHVM.Emp_Detail._EmailId;

                    string strSubject = "Advance Cheque Request Approval Status - " + RequestStatus;
                    string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>Advance Cheque Request Approval Status " + RequestStatus + " - Emp Code (" + PHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +

                                     "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                                     "<tr><td valign=top colspan =2>Your Advance Cheque Request has been <b>" + RequestStatus + "</b> by " + employeeDetails.Employee_Name + " San. The request details are as follows:</td></tr>" +
                                     "<tr><td width=125 height=22 valign=top>ACR Number</td><td width=389 valign=top>" + PHVM.ACRNO + "</td></tr>" +
                                     "<tr><td width=125 height=23 valign=top>Amount</td><td width=389 valign=top>" + PHVM.AMOUNT + "</td> </tr>" +
                                     "<tr><td width=125 valign=top>PO Number</td><td width=389 valign=top>" + PHVM.PONO + "</td></tr>" +
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

        public POHeaderViewModel GetPODetailByPOId(string poNo)
        {
            return _ACRRepo.GetPODetailByPOId(poNo);
        }

        public long GetPRStatusByPOId(string pono)
        {
            return _ACRRepo.GetPRStatusByPOId(pono);
        }
        //============Change Done on 27082022 by Aumento Team For Add Other Category==========================================================================================
        public List<Employee_Details> PortalAutocompleteSuggestions(string term, string designation)
        {
            return _ACRRepo.PortalAutocompleteSuggestions(term, designation);
        }
        public List<Employee_Details> AutocompleteDesignation(string term)
        {
            return _ACRRepo.AutocompleteDesignation(term);
        }
        //=====================================================================================================================================================================
        //Below Added By Aumento Start
        public List<PR_Div_Dep_SecViewModel> BindKI()
        {
            return _ACRRepo.GetKiLIST();
        }
        public SearchACRViewModel ACRDashboard(SearchACRViewModel SSM, long plantId)
        {
            return _ACRRepo.ACRDashboard(SSM, plantId);
        }
        public short PARequest(VM_ACR_PaymentAdvise_Master data)
        {
            short retval = 1;
            retval = _ACRRepo.SavePARequest(data);
            return retval;
        }
        public short UpdateDocStatus(long smheaderId, long updatedBy, Employee_Details emp_dtl)
        {
            //return _ACRRepo.UpdateDocStatus(smheaderId, updatedBy);
            //Added By Aumento as on 28022024 Start
            short retVal = _ACRRepo.UpdateDocStatus(smheaderId, updatedBy);
            if (retVal == 1)
            {
                _sendMailForDocRecivedByFinanceAuthority(smheaderId, emp_dtl);
            }
            return retVal;
            //Added By Aumento as on 28022024 End
        }
        public Tuple<short, List<VM_ACR_PADetailViewModel>> SavePAAttachment(long addedDate, List<VM_ACR_PADetailViewModel> modelList, string PA_HeaderID)
        {
            List<VM_ACR_PADetailViewModel> iList = new List<VM_ACR_PADetailViewModel>();
            short retVal = _ACRRepo.SavePADetails(addedDate, modelList);
            if (retVal == 1)
            {
                iList = _ACRRepo.GetPAAttachmentDetail(PA_HeaderID);
            }
            Tuple<short, List<VM_ACR_PADetailViewModel>> _tuple = new Tuple<short, List<VM_ACR_PADetailViewModel>>(retVal, iList);
            return _tuple;
        }
        public List<VM_ACR_PaymentAdvise_Master> GetPADetails(VM_ACR_PaymentAdvise_Master obj)
        {
            return _ACRRepo.GetPADetails(obj);
        }
        public Tuple<short, List<VM_ACR_PADetailViewModel>> DeletePAAttachment(string fileName, string docType, long PA_HeaderID, string PA_ID)
        {
            List<VM_ACR_PADetailViewModel> iList = new List<VM_ACR_PADetailViewModel>();
            short retVal = _ACRRepo.DeletePAAttachment(fileName, docType, PA_HeaderID);
            if (retVal == 1)
            {
                iList = _ACRRepo.GetPAAttachmentDetail(PA_ID);
            }
            Tuple<short, List<VM_ACR_PADetailViewModel>> _tuple = new Tuple<short, List<VM_ACR_PADetailViewModel>>(retVal, iList);
            return _tuple;
        }
        public short deletePAReq(DeleteACRPA obj)
        {
            return _ACRRepo.deletePAReq(obj);
        }
        public List<VM_ACR_PADetailViewModel> PAdocs(string PA_ID)
        {
            return _ACRRepo.GetPAAttachmentDetail(PA_ID);
        }
        public short ACRFinApproval(ACRAppHistoryViewModel SHVM, Employee_Details emp_dtl, List<ACRDetailViewModel> DetailList)
        {
            short retVal = _ACRRepo.ACRFinApproval(SHVM, DetailList);
            if (retVal == 1)
            {
                _sendMailByFinanceAuthority(SHVM.ACRID, SHVM.APPROVAL_STATUS, emp_dtl);
            }
            return retVal;
        }
        //Updated By Aumento as on 16022024 Start
        public short _sendMailByFinanceAuthority(long smHeaderId, short approvalStatus, Employee_Details employeeDetails)
        {
            short retVal = 0;
            try
            {
                EmailCore sendMail = new EmailCore();
                string RequestStatus = approvalStatus == 1 ? "Approved" : approvalStatus == 2 ? "Send back" : approvalStatus == 3 ? "Rejected" : approvalStatus == 4 ? "Hold" : approvalStatus == 5 ? "Forwarded to Taxation" : "";

                #region Send mail for Taxation Authority
                ACRHeaderViewModel SHVM = _ACRRepo.GetACRRequestById(smHeaderId);
                if (SHVM.acrAppHis.Count > 0 && approvalStatus == 5)                
                {
                    List<ACRAppHistoryViewModel> Auth_List = SHVM.acrAppHis.Where(a => a.ACRAPPHISTORY_ID > 0 && a.APPROVAL_STATUS == 0 && !string.IsNullOrEmpty(a.APP_EMAIL)).ToList();
                    foreach (ACRAppHistoryViewModel Auth_obj in Auth_List)
                    {
                        if (Auth_obj != null)
                        {

                            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                            if (serverpath.isTestServer())
                                sendMail.MailTo = serverpath.getTestEMail();
                            else
                                sendMail.MailTo = Auth_obj.APP_EMAIL;

                            string struid = (Auth_obj.APPEMP_CODE).ToString();
                            string strid = smHeaderId.ToString();
                            string reqType = "Advance Cheque"; //SHVM.SM_TYPE == 1 ? "Service Entry Sheet" : "Material Receipt Note";

                            string strSubject = reqType + " Request - " + SHVM.ACRNO + ", Requestor - " + SHVM.Emp_Detail._EName + "(" + SHVM.Emp_Detail._ECode + ")";
                            string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                         "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>" + reqType + " Request For Review</font></b></td></tr>" +
                                         "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + Auth_obj.APPEMP_NAME +
                                         " San ,</br></br>  " + reqType + " approval request in Employee Portal for review. </br></br><b> Below are the details : </b></br></td></tr>" +
                                         "<tr><td width=125 height=23 valign=top>ACR Number: </td><td width=389 valign=top>" + SHVM.ACRNO + "</td></tr>" +
                                         "<tr><td width=125 height=23 valign=top>PO Number: </td><td width=389 valign=top>" + SHVM.PONO + "</td></tr>" +
                                         "<tr><td width=125 height=23 valign=top>Amount: </td><td width=389 valign=top>" + SHVM.AMOUNT + "</td></tr>" +
                                         "<tr><td width=125 height=23 valign=top>Remarks: </td><td width=389 valign=top>" + SHVM.REMARK + "</td></tr>" +
                                         "<tr><td valign=top colspan=2>Please login <a href=" + serverpath.getServerPath() + "Login/Index> Employee Portal</a> to view the request.</td></tr>" +
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
                else
                {
                    #region Send mail for requestor
                    if (!string.IsNullOrEmpty(SHVM.Emp_Detail._EmailId))
                    {
                        
                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        sendMail.MailTo = SHVM.Emp_Detail._EmailId;
                        
                        string reqType = "Advance Cheque ";// SHVM.SM_TYPE == 1 ? "Service Entry Sheet" : "Material Receipt Note";

                        string strSubject = reqType + " Approval Status - " + RequestStatus;
                        string strBody = 
                        "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>" + reqType + " Approval Status " + RequestStatus + " - Emp Code (" + SHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +
                                     "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                                     "<tr><td valign=top colspan =2>Your " + reqType + " request has been <b>" + RequestStatus + "</b> by " + employeeDetails.Employee_Name + " San. The request details are as follows:</td></tr>" +
                                     "<tr><td width=125 height=22 valign=top>ACR Number: </td><td width=389 valign=top>" + SHVM.ACRNO + "</td></tr>" +
                                     "<tr><td width=125 height=22 valign=top>PO Number: </td><td width=389 valign=top>" + SHVM.PONO + "</td></tr>" +
                                     "<tr><td width=125 height=22 valign=top>Amount: </td><td width=389 valign=top>" + SHVM.AMOUNT + "</td></tr>" +
                                     "<tr><td width=125 valign=top>Remarks: </td><td width=389 valign=top>" + SHVM.REMARK + "</td></tr>" +
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
                #endregion
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }
        //Updated By Aumento as on 16022024 End
        public IEnumerable<SYSITE> Bind_SYSite()
        {
            return _ACRRepo.Bind_SYSite();
        }
        //Added By Aumento as on 28022024 Start
        public short _sendMailForDocRecivedByFinanceAuthority(long smHeaderId, Employee_Details employeeDetails)
        {
            short retVal = 0;
            try
            {
                EmailCore sendMail = new EmailCore();
                ACRHeaderViewModel SHVM = _ACRRepo.GetACRRequestById(smHeaderId);             
                    #region Send mail for requestor
                    if (!string.IsNullOrEmpty(SHVM.Emp_Detail._EmailId))
                    {
                    String DocStatus = "Recevied";

                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        sendMail.MailTo = SHVM.Emp_Detail._EmailId;                       
                        string reqType = "Advance Cheque ";

                        string strSubject = reqType + " Document Status - " + DocStatus;
                        string strBody =
                        "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>" + reqType + " Document Status " + DocStatus + " - Emp Code (" + SHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +
                                     "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                                     "<tr><td valign=top colspan =2>Your " + reqType + " request Document <b>" + DocStatus + "</b> by " + employeeDetails.Employee_Name + " San. The request details are as follows:</td></tr>" +
                                     "<tr><td width=125 height=22 valign=top>ACR Number: </td><td width=389 valign=top>" + SHVM.ACRNO + "</td></tr>" +
                                     "<tr><td width=125 height=22 valign=top>PO Number: </td><td width=389 valign=top>" + SHVM.PONO + "</td></tr>" +
                                     "<tr><td width=125 height=22 valign=top>Amount: </td><td width=389 valign=top>" + SHVM.AMOUNT + "</td></tr>" +
                                     "<tr><td width=125 valign=top>Remarks: </td><td width=389 valign=top>" + SHVM.REMARK + "</td></tr>" +
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
        //Added By Aumento as on 28022024 End

        //Added by aumento as on 28092024 for the SR80813 ===============================================
        public long GetACRNextApprovalId(long ACRID, long ecode)
        {
            return _ACRRepo.GetACRNextApprovalId(ACRID, ecode);
        }
        //Ended by aumento as on 28092024 for the SR80813 ===============================================
        // start Aumento added :: SR111540
        public LibResult AutocompleteSuggestionsVendor(string Key)
        {
            return _ACRRepo.AutocompleteSuggestionsVendor(Key);
        }
        public LibResult AutocompleteSuggestionsPONumber(string Key)
        {
            return _ACRRepo.AutocompleteSuggestionsPONumber(Key);
        }
        // end Aumento added :: SR111540
    }
}

