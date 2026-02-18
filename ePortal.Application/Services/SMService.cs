using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePortal.ViewModels;
using System.Web;
using ePortal.DomainClasses;
using ePortal.Application.Contracts;
using ePortal.Infrastructure.Repositories;
using ePortal.Shared;

namespace ePortal.Application.Services
{
    public class SMService : ISMService
    {

        private readonly SMRepository _SMRepo;
        private readonly CommonRepository _CommonRepo;
        Tuple<short, long> _retVal_tuple;

        public SMService(SMRepository objSMRepo, CommonRepository objCommonRepo)
        {
            _SMRepo = objSMRepo;
            _CommonRepo = objCommonRepo;
            _retVal_tuple = new Tuple<short, long>((short)0, 0L);
        }

        public Employee_Details GetAuthEmpById(long empCode, int designationId, string designation, Employee_Details empDtl)
        {
            return _SMRepo.GetAuthEmpById(empCode, designationId, designation, empDtl);
        }

        public SMHeaderViewModel GetSMRequestById(long id)
        {
            return _SMRepo.GetSMRequestById(id);
        }

        public short SMApproval(SMAppHistoryViewModel SHVM, Employee_Details emp_dtl, List<SMDetailViewModel> SmDetailList)
        {
            short retVal = _SMRepo.SMApproval(SHVM, SmDetailList);
            if (retVal == 1)
            {
                SendMailByApprovalAuthority(SHVM.SMHEADERID, SHVM.APPROVAL_STATUS, emp_dtl);
            }
            return retVal;
        }

        public short SMCancel(SMHeaderViewModel SHVM)
        {
            return _SMRepo.SMCancel(SHVM);
        }

        public Tuple<short, long> SaveSMRequest(SMHeaderViewModel model)
        {
            Tuple<short, long> _tuple = _SMRepo.SaveSMRequest(model);
            if (_tuple.Item1 == 1 && _tuple.Item2 > 0 && model.IsFinalSubmit == 1)
            {
                SendMailByRequestor(_tuple.Item2);
            }
            return _tuple;
        }

        public Tuple<short, List<SMAppAuthSeqViewModel>> GetDefaultAuthority(long loginUser, decimal amount, short SM_Type, Employee_Details _Employee_Details)
        {
            List<SMAppAuthSeqViewModel> AuthList = new List<SMAppAuthSeqViewModel>();
            Tuple<short, List<SMAppAuthSeqViewModel>> _tuple;
            short ISIOCGBlock = 0;
            try
            {
                decimal _valSesAmt = 0; decimal _valMrnAmt = 0;
                string strValue = _CommonRepo.GetParameterValue("SES_MRN_VALIDATION");
                if (!string.IsNullOrEmpty(strValue))
                {
                    string[] strArray = strValue.Split(',');
                    foreach (string str in strArray)
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            string[] strSES = str.Split('~');
                            if (strSES[0] == SM_Type.ToString())
                            {
                                if (SM_Type == 1) { _valSesAmt = Convert.ToDecimal(strSES[1]); }
                                else { _valMrnAmt = Convert.ToDecimal(strSES[1]); }
                            }
                        }
                    }
                }
                else
                {
                    AuthList = new List<SMAppAuthSeqViewModel>();
                    _tuple = new Tuple<short, List<SMAppAuthSeqViewModel>>(ISIOCGBlock, AuthList);
                    return _tuple;
                    //return AuthList = new List<SMAppAuthSeqViewModel>();
                }

                UserApprovalAuthority Obj = _CommonRepo.CheckApprovalAuthority(loginUser);
                if (Obj != null)
                {
                    bool IsSecMgr = false; bool IsDepMgr = false; bool IsDivHd = false;
                    if (SM_Type == 1)
                    {
                        if (Obj.SectionManager != 0 && Obj.SectionManager != null && Obj.SectionManager != loginUser)
                        {
                            Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.SectionManager));
                            AuthList.Add(new SMAppAuthSeqViewModel
                            {
                                ADEMPCODE = Emp_Dtl._ECode,
                                ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                                ADDESIGNATION = Emp_Dtl._Desig,
                                APP_SEQ = (AuthList.Count == 0 ? (short)1 : Convert.ToInt16(AuthList.Max(x => x.APP_SEQ) + 1)),
                                APPTYPE = 1,
                                FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                            });
                            IsSecMgr = true;
                        }
                        if (Obj.DepartmentManager != 0 && Obj.DepartmentManager != null && Obj.DepartmentManager != loginUser)
                        {
                            Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.DepartmentManager));
                            AuthList.Add(new SMAppAuthSeqViewModel
                            {
                                ADEMPCODE = Emp_Dtl._ECode,
                                ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                                ADDESIGNATION = Emp_Dtl._Desig,
                                APP_SEQ = (AuthList.Count == 0 ? (short)1 : Convert.ToInt16(AuthList.Max(x => x.APP_SEQ) + 1)),
                                APPTYPE = 1,
                                FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                            });
                            IsDepMgr = true;
                        }
                        if (IsDepMgr ? (amount > _valSesAmt) : true)
                        {
                            if (Obj.DivisionHead != 0 && Obj.DivisionHead != null && Obj.DivisionHead != loginUser)
                            {
                                Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.DivisionHead));
                                AuthList.Add(new SMAppAuthSeqViewModel
                                {
                                    ADEMPCODE = Emp_Dtl._ECode,
                                    ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                                    ADDESIGNATION = Emp_Dtl._Desig,
                                    APP_SEQ = (AuthList.Count == 0 ? (short)1 : Convert.ToInt16(AuthList.Max(x => x.APP_SEQ) + 1)),
                                    APPTYPE = 1,
                                    FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                                });
                                IsDivHd = true;
                            }
                            if (Obj.OperationHead != 0 && Obj.OperationHead != null && !IsDivHd)
                            {
                                Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.OperationHead));
                                AuthList.Add(new SMAppAuthSeqViewModel
                                {
                                    ADEMPCODE = Emp_Dtl._ECode,
                                    ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                                    ADDESIGNATION = Emp_Dtl._Desig,
                                    APP_SEQ = (AuthList.Count == 0 ? (short)1 : Convert.ToInt16(AuthList.Max(x => x.APP_SEQ) + 1)),
                                    APPTYPE = 1,
                                    FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                                });
                            }
                        }

                        //// --- IOCG Authority --- ////
                        AuthList = GetIOCGAppAuth(AuthList, SM_Type, amount, _Employee_Details);
                        //// --- End --- ////
                    }
                    else if (SM_Type == 2)
                    {
                        if (Obj.SectionManager != 0 && Obj.SectionManager != null && Obj.SectionManager != loginUser)
                        {
                            Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.SectionManager));
                            AuthList.Add(new SMAppAuthSeqViewModel
                            {
                                ADEMPCODE = Emp_Dtl._ECode,
                                ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                                ADDESIGNATION = Emp_Dtl._Desig,
                                APP_SEQ = (AuthList.Count == 0 ? (short)1 : Convert.ToInt16(AuthList.Max(x => x.APP_SEQ) + 1)),
                                APPTYPE = 1,
                                FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                            });
                            IsSecMgr = true;
                        }
                        if ((IsSecMgr ? (amount > _valMrnAmt) : true))
                        {
                            if (Obj.DepartmentManager != 0 && Obj.DepartmentManager != null && Obj.DepartmentManager != loginUser)
                            {
                                Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.DepartmentManager));
                                AuthList.Add(new SMAppAuthSeqViewModel
                                {
                                    ADEMPCODE = Emp_Dtl._ECode,
                                    ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                                    ADDESIGNATION = Emp_Dtl._Desig,
                                    APP_SEQ = (AuthList.Count == 0 ? (short)1 : Convert.ToInt16(AuthList.Max(x => x.APP_SEQ) + 1)),
                                    APPTYPE = 1,
                                    FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                                });
                                IsDepMgr = true;
                            }
                            if (Obj.DivisionHead != 0 && Obj.DivisionHead != null && Obj.DivisionHead != loginUser && !IsDepMgr)
                            {
                                Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.DivisionHead));
                                AuthList.Add(new SMAppAuthSeqViewModel
                                {
                                    ADEMPCODE = Emp_Dtl._ECode,
                                    ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                                    ADDESIGNATION = Emp_Dtl._Desig,
                                    APP_SEQ = (AuthList.Count == 0 ? (short)1 : Convert.ToInt16(AuthList.Max(x => x.APP_SEQ) + 1)),
                                    APPTYPE = 1,
                                    FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                                });
                                IsDivHd = true;
                            }
                            if (Obj.OperationHead != 0 && Obj.OperationHead != null && !IsDivHd && !IsDepMgr)
                            {
                                Employee_Details Emp_Dtl = _CommonRepo.GetEmpDetailById(Convert.ToInt64(Obj.OperationHead));
                                AuthList.Add(new SMAppAuthSeqViewModel
                                {
                                    ADEMPCODE = Emp_Dtl._ECode,
                                    ADEMPNAME = Emp_Dtl._EFirstName + " " + Emp_Dtl._ELastName,
                                    ADDESIGNATION = Emp_Dtl._Desig,
                                    APP_SEQ = (AuthList.Count == 0 ? (short)1 : Convert.ToInt16(AuthList.Max(x => x.APP_SEQ) + 1)),
                                    APPTYPE = 1,
                                    FNDESID = Convert.ToInt16(Emp_Dtl._FnDesigId == null ? 0 : Emp_Dtl._FnDesigId),
                                });
                            }
                        }

                        //// --- IOCG Authority --- ////
                        AuthList = GetIOCGAppAuth(AuthList, SM_Type, amount, _Employee_Details);
                        //// --- End --- ////
                    }
                }
                if (AuthList.Where(m => m.APPTYPE == 2).Count() == 0)
                {
                    ISIOCGBlock = _SMRepo.ISIOCGBLOCK(_Employee_Details);
                }
                /// ---  Add Finance Approval ---///
                if (AuthList.Count > 0)
                {
                    AuthList.Add(new SMAppAuthSeqViewModel
                    {
                        ADEMPCODE = 1,
                        ADEMPNAME = "Finance Approval",
                        ADDESIGNATION = "",
                        APP_SEQ = (AuthList.Count == 0 ? (short)1 : Convert.ToInt16(AuthList.Max(x => x.APP_SEQ) + 1)),
                        APPTYPE = 3,
                        FNDESID = 0,
                    });
                }
                // START Added by Aumento :: SR81670 and SR81635
                var distinctAuthList = AuthList
                    .GroupBy(x => new { x.ADEMPCODE, x.ADEMPNAME, x.ADDESIGNATION, x.APPTYPE })
                    .Select(g => g.First())
                    .ToList();


                short APPSEQ = 1;
                foreach (var record in distinctAuthList)
                {
                    record.APP_SEQ = APPSEQ;
                    APPSEQ++;
                }

                AuthList = distinctAuthList;
                //END Added by Aumento :: SR81670 and SR81635

            }
            catch (Exception ex)
            {
                AuthList = new List<SMAppAuthSeqViewModel>();
            }
            _tuple = new Tuple<short, List<SMAppAuthSeqViewModel>>(ISIOCGBlock, AuthList.OrderBy(m => m.APPTYPE).OrderBy(n => n.APP_SEQ).ToList());
            return _tuple;
            //return AuthList.OrderBy(m => m.APPTYPE).OrderBy(n => n.APP_SEQ).ToList();
        }

        public List<SMAppAuthSeqViewModel> GetIOCGAppAuth(List<SMAppAuthSeqViewModel> AuthList, short SM_Type, decimal amount, Employee_Details _Employee_Details)
        {
            #region Section
            if (_Employee_Details._SecId != 0 && _Employee_Details._SecId != null)
            {
                SmOpMapViewModel opMapModel = _SMRepo.GetIOCGAuthority(SM_Type, Convert.ToInt64(_Employee_Details._SecId), amount);
                if (opMapModel != null)
                {
                    if (opMapModel.APPROVER1 != null && opMapModel.APPROVER1 > 0)
                    {
                        AuthList.Add(new SMAppAuthSeqViewModel
                        {
                            ADEMPCODE = (long)opMapModel.APPROVER1,
                            ADEMPNAME = opMapModel.APPROVER1Name,
                            ADDESIGNATION = opMapModel.APPROVER1Desg,
                            APP_SEQ = (AuthList.Count == 0 ? (short)1 : Convert.ToInt16(AuthList.Max(x => x.APP_SEQ) + 1)),
                            APPTYPE = 2, //// 2 - IOCG Authority
                            FNDESID = 0,
                        });
                    }
                    if (opMapModel.APPROVER2 != 0 && opMapModel.APPROVER2 != null)
                    {
                        AuthList.Add(new SMAppAuthSeqViewModel
                        {
                            ADEMPCODE = Convert.ToInt64(opMapModel.APPROVER2),
                            ADEMPNAME = opMapModel.APPROVER2Name,
                            ADDESIGNATION = opMapModel.APPROVER2Desg,
                            APP_SEQ = (AuthList.Count == 0 ? (short)1 : Convert.ToInt16(AuthList.Max(x => x.APP_SEQ) + 1)),
                            APPTYPE = 2, //// 2 - IOCG Authority
                            FNDESID = 0,
                        });
                    }
                    return AuthList;
                }
            }
            #endregion

            #region Department
            if (_Employee_Details._DepId != 0 && _Employee_Details._DepId != null)
            {
                SmOpMapViewModel opMapModel = _SMRepo.GetIOCGAuthority(SM_Type, Convert.ToInt64(_Employee_Details._DepId), amount);
                if (opMapModel != null)
                {
                    if (opMapModel.APPROVER1 != null && opMapModel.APPROVER1 > 0)
                    {
                        AuthList.Add(new SMAppAuthSeqViewModel
                        {
                            ADEMPCODE = (long)opMapModel.APPROVER1,
                            ADEMPNAME = opMapModel.APPROVER1Name,
                            ADDESIGNATION = opMapModel.APPROVER1Desg,
                            APP_SEQ = (AuthList.Count == 0 ? (short)1 : Convert.ToInt16(AuthList.Max(x => x.APP_SEQ) + 1)),
                            APPTYPE = 2, //// 2 - IOCG Authority
                            FNDESID = 0,
                        });
                    }
                    if (opMapModel.APPROVER2 != 0 && opMapModel.APPROVER2 != null)
                    {
                        AuthList.Add(new SMAppAuthSeqViewModel
                        {
                            ADEMPCODE = Convert.ToInt64(opMapModel.APPROVER2),
                            ADEMPNAME = opMapModel.APPROVER2Name,
                            ADDESIGNATION = opMapModel.APPROVER2Desg,
                            APP_SEQ = (AuthList.Count == 0 ? (short)1 : Convert.ToInt16(AuthList.Max(x => x.APP_SEQ) + 1)),
                            APPTYPE = 2, //// 2 - IOCG Authority
                            FNDESID = 0,
                        });
                    }
                    return AuthList;
                }
            }
            #endregion

            #region Division
            if (_Employee_Details._DivId != 0 && _Employee_Details._DivId != null)
            {
                SmOpMapViewModel opMapModel = _SMRepo.GetIOCGAuthority(SM_Type, Convert.ToInt64(_Employee_Details._DivId), amount);
                if (opMapModel != null)
                {
                    if (opMapModel.APPROVER1 != null && opMapModel.APPROVER1 > 0)
                    {
                        AuthList.Add(new SMAppAuthSeqViewModel
                        {
                            ADEMPCODE = (long)opMapModel.APPROVER1,
                            ADEMPNAME = opMapModel.APPROVER1Name,
                            ADDESIGNATION = opMapModel.APPROVER1Desg,
                            APP_SEQ = (AuthList.Count == 0 ? (short)1 : Convert.ToInt16(AuthList.Max(x => x.APP_SEQ) + 1)),
                            APPTYPE = 2, //// 2 - IOCG Authority
                            FNDESID = 0,
                        });
                    }
                    if (opMapModel.APPROVER2 != 0 && opMapModel.APPROVER2 != null)
                    {
                        AuthList.Add(new SMAppAuthSeqViewModel
                        {
                            ADEMPCODE = Convert.ToInt64(opMapModel.APPROVER2),
                            ADEMPNAME = opMapModel.APPROVER2Name,
                            ADDESIGNATION = opMapModel.APPROVER2Desg,
                            APP_SEQ = (AuthList.Count == 0 ? (short)1 : Convert.ToInt16(AuthList.Max(x => x.APP_SEQ) + 1)),
                            APPTYPE = 2, //// 2 - IOCG Authority
                            FNDESID = 0,
                        });
                    }
                    return AuthList;
                }
            }
            #endregion

            #region Operation
            if (_Employee_Details._OpId != 0 && _Employee_Details._OpId != null)
            {
                SmOpMapViewModel opMapModel = _SMRepo.GetIOCGAuthority(SM_Type, Convert.ToInt64(_Employee_Details._OpId), amount);
                if (opMapModel != null)
                {
                    if (opMapModel.APPROVER1 != null && opMapModel.APPROVER1 > 0)
                    {
                        AuthList.Add(new SMAppAuthSeqViewModel
                        {
                            ADEMPCODE = (long)opMapModel.APPROVER1,
                            ADEMPNAME = opMapModel.APPROVER1Name,
                            ADDESIGNATION = opMapModel.APPROVER1Desg,
                            APP_SEQ = (AuthList.Count == 0 ? (short)1 : Convert.ToInt16(AuthList.Max(x => x.APP_SEQ) + 1)),
                            APPTYPE = 2, //// 2 - IOCG Authority
                            FNDESID = 0,
                        });
                    }
                    if (opMapModel.APPROVER2 != 0 && opMapModel.APPROVER2 != null)
                    {
                        AuthList.Add(new SMAppAuthSeqViewModel
                        {
                            ADEMPCODE = Convert.ToInt64(opMapModel.APPROVER2),
                            ADEMPNAME = opMapModel.APPROVER2Name,
                            ADDESIGNATION = opMapModel.APPROVER2Desg,
                            APP_SEQ = (AuthList.Count == 0 ? (short)1 : Convert.ToInt16(AuthList.Max(x => x.APP_SEQ) + 1)),
                            APPTYPE = 2, //// 2 - IOCG Authority
                            FNDESID = 0,
                        });
                    }
                    return AuthList;
                }
            }
            #endregion

            return AuthList;
        }

        public Tuple<short, List<SMDetailViewModel>> SaveAttachment(long addedDate, long poHid, List<SMDetailViewModel> modelList)
        {
            List<SMDetailViewModel> iList = new List<SMDetailViewModel>();
            short retVal = _SMRepo.SaveSMDetails(addedDate, poHid, modelList);
            if (retVal == 1)
            {
                iList = _SMRepo.GetAttachmentDetail(poHid);
            }
            Tuple<short, List<SMDetailViewModel>> _tuple = new Tuple<short, List<SMDetailViewModel>>(retVal, iList);
            return _tuple;
        }

        public Tuple<short, List<SMDetailViewModel>> DeleteAttachment(string fileName, string docType, long poHid)
        {
            List<SMDetailViewModel> iList = new List<SMDetailViewModel>();
            short retVal = _SMRepo.DeleteAttachment(fileName, docType, poHid);
            if (retVal == 1)
            {
                iList = _SMRepo.GetAttachmentDetail(poHid);
            }
            Tuple<short, List<SMDetailViewModel>> _tuple = new Tuple<short, List<SMDetailViewModel>>(retVal, iList);
            return _tuple;
        }

        public short SendMailByRequestor(long smHeaderId)
        {
            short retVal = 0;
            try
            {
                SMHeaderViewModel SHVM = _SMRepo.GetSMRequestById(smHeaderId);
                if (SHVM.smAppHis.Count > 0)
                {
                    SMAppHistoryViewModel Auth_obj = SHVM.smAppHis.Where(a => a.SMAPPHISTORYID > 0 && a.APPROVAL_STATUS == 0 && !string.IsNullOrEmpty(a.APP_EMAIL)).FirstOrDefault();
                    if (Auth_obj != null)
                    {
                        EmailCore sendMail = new EmailCore();
                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        if (serverpath.isTestServer())
                            sendMail.MailTo = serverpath.getTestEMail();
                        else
                            sendMail.MailTo = Auth_obj.APP_EMAIL;

                        string struid = (Auth_obj.APPEMP_CODE).ToString();
                        string strid = smHeaderId.ToString();
                        string reqType = SHVM.SM_TYPE == 1 ? "SES" : "MRN";

                        string strSubject = reqType + " Request - " + SHVM.SM_NO + ", Requestor - " + SHVM.Emp_Detail._EName + "(" + SHVM.Emp_Detail._ECode + ")";
                        string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                         "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>" + reqType + " Request from " + SHVM.Emp_Detail._EName + " - Emp Code (" + SHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +

                                         "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + Auth_obj.APPEMP_NAME + " San ,</br></br>" + SHVM.Emp_Detail._EName + " San has raised a " + reqType + " Approval Request in Employee Portal. Below are the details :</br></td></tr>" +
                                         "<tr><td width=125 height=23 valign=top>SES/MRN Number: </td><td width=389 valign=top>" + SHVM.SM_NO + "</td></tr>" +
                                         "<tr><td width=125 height=23 valign=top>PO Number: </td><td width=389 valign=top>" + SHVM.PONUMBER + "</td></tr>" +
                                         "<tr><td width=125 height=23 valign=top>Amount: </td><td width=389 valign=top>" + (SHVM.AMOUNT <= 200000 ? "<= 2 Lakh" : "> 2 Lakh") + "</td></tr>" +
                                         "<tr><td width=125 height=23 valign=top>Remarks: </td><td width=389 valign=top>" + SHVM.REMARK + "</td></tr>" +
                                          "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "/Login/EmailApproval/?Wid=" + struid + "&C=SM&A=SMApproval&Tid=" + strid + " > Employee Portal</a> link to approve the request.</td></tr>" +
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

        public short SendMailByApprovalAuthority(long smHeaderId, short approvalStatus, Employee_Details employeeDetails)
        {
            short retVal = 0;
            try
            {
                string RequestStatus = approvalStatus == 1 ? "Approved" : approvalStatus == 2 ? "Send back" : approvalStatus == 3 ? "Rejected" : "";

                #region Send mail next approval authority
                SMHeaderViewModel SHVM = _SMRepo.GetSMRequestById(smHeaderId);
                if (SHVM.smAppHis.Count > 0 && approvalStatus == 1)
                {
                    SMAppHistoryViewModel Auth_obj = SHVM.smAppHis.Where(a => a.SMAPPHISTORYID > 0 && a.APPROVAL_STATUS == 0 && !string.IsNullOrEmpty(a.APP_EMAIL)).FirstOrDefault();
                    if (Auth_obj != null)
                    {
                        EmailCore sendMail = new EmailCore();
                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        if (serverpath.isTestServer())
                            sendMail.MailTo = serverpath.getTestEMail();
                        else
                            sendMail.MailTo = Auth_obj.APP_EMAIL;

                        string struid = (Auth_obj.APPEMP_CODE).ToString();
                        string strid = smHeaderId.ToString();
                        string reqType = SHVM.SM_TYPE == 1 ? "SES" : "MRN";

                        string strSubject = reqType + " Request - " + SHVM.SM_NO + ", Requestor - " + SHVM.Emp_Detail._EName + "(" + SHVM.Emp_Detail._ECode + ")";
                        string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                         "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>" + reqType + " Request from " + SHVM.Emp_Detail._EName + " - Emp Code (" + SHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +
                                         "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + Auth_obj.APPEMP_NAME + " San ,</br></br>" + SHVM.Emp_Detail._EName + " San has raised a " + reqType + " Approval Request in Employee Portal. Below are the details :</br></td></tr>" +
                                         "<tr><td width=125 height=23 valign=top>SES/MRN Number: </td><td width=389 valign=top>" + SHVM.SM_NO + "</td></tr>" +
                                         "<tr><td width=125 height=23 valign=top>PO Number: </td><td width=389 valign=top>" + SHVM.PONUMBER + "</td></tr>" +
                                         "<tr><td width=125 height=23 valign=top>Amount: </td><td width=389 valign=top>" + (SHVM.AMOUNT <= 200000 ? "<= 2 Lakh" : "> 2 Lakh") + "</td></tr>" +
                                         "<tr><td width=125 height=23 valign=top>Remarks: </td><td width=389 valign=top>" + SHVM.REMARK + "</td></tr>" +
                                         "<tr><td valign=top colspan=2>Please click on <a href=" + serverpath.getServerPath() + "/Login/EmailApproval/?Wid=" + struid + "&C=SM&A=SMApproval&Tid=" + strid + " > Employee Portal</a> link to approve the request.</td></tr>" +
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
                if (!string.IsNullOrEmpty(SHVM.Emp_Detail._EmailId))
                {
                    EmailCore sendMail = new EmailCore();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    sendMail.MailTo = SHVM.Emp_Detail._EmailId;
                    string reqType = SHVM.SM_TYPE == 1 ? "SES" : "MRN";

                    string strSubject = reqType + " Approval Status - " + RequestStatus;
                    string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                     "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>" + reqType + " Approval Status " + RequestStatus + " - Emp Code (" + SHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +

                                     "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                                     "<tr><td valign=top colspan =2>Your " + reqType + " request has been <b>" + RequestStatus + "</b> by " + employeeDetails.Employee_Name + " San. The request details are as follows:</td></tr>" +
                                     "<tr><td width=125 height=22 valign=top>SES/MRN Number: </td><td width=389 valign=top>" + SHVM.SM_NO + "</td></tr>" +
                                     "<tr><td width=125 height=22 valign=top>PO Number: </td><td width=389 valign=top>" + SHVM.PONUMBER + "</td></tr>" +
                                     "<tr><td width=125 height=22 valign=top>Amount: </td><td width=389 valign=top>" + (SHVM.AMOUNT <= 200000 ? "<= 2 Lakh" : "> 2 Lakh") + "</td></tr>" +
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

        public POHeaderViewModel GetPODetailByPOId(string poNo)
        {
            return _SMRepo.GetPODetailByPOId(poNo);
        }

        public VendorViewModel GetVendorByCode(string vcode)
        {
            return _SMRepo.GetVendorByCode(vcode);
        }

        public List<VendorViewModel> VendorAutocompleteSuggestions(string term)
        {
            return _SMRepo.VendorAutocompleteSuggestions(term);
        }

        public SearchSMViewModel SMDashboard(SearchSMViewModel SSM, long plantId)
        {
            return _SMRepo.SMDashboard(SSM, plantId);
        }

        public short UpdateDocStatus(long smheaderId, long updatedBy)
        {
            return _SMRepo.UpdateDocStatus(smheaderId, updatedBy);
        }

        public short SMFinApproval(SMAppHistoryViewModel SHVM, Employee_Details emp_dtl, List<SMDetailViewModel> SmDetailList)
        {
            short retVal = _SMRepo.SMFinApproval(SHVM, SmDetailList);
            if (retVal == 1)
            {
                SendMailByFinanceAuthority(SHVM.SMHEADERID, SHVM.APPROVAL_STATUS, emp_dtl);
            }
            return retVal;
        }
        public short SendMailByFinanceAuthority(long smHeaderId, short approvalStatus, Employee_Details employeeDetails)
        {
            short retVal = 0;
            try
            {
                string RequestStatus = approvalStatus == 1 ? "Approved" : approvalStatus == 2 ? "Send back" : approvalStatus == 3 ? "Rejected" : approvalStatus == 4 ? "Hold" : approvalStatus == 5 ? "Forwarded to Taxation" : "";

                #region Send mail for Taxation Authority
                SMHeaderViewModel SHVM = _SMRepo.GetSMRequestById(smHeaderId);
                if (SHVM.smAppHis.Count > 0 && approvalStatus == 5)
                {
                    List<SMAppHistoryViewModel> Auth_List = SHVM.smAppHis.Where(a => a.SMAPPHISTORYID > 0 && a.APPROVAL_STATUS == 0 && !string.IsNullOrEmpty(a.APP_EMAIL)).ToList();
                    foreach (SMAppHistoryViewModel Auth_obj in Auth_List)
                    {
                        if (Auth_obj != null)
                        {
                            EmailCore sendMail = new EmailCore();
                            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                            if (serverpath.isTestServer())
                                sendMail.MailTo = serverpath.getTestEMail();
                            else
                                sendMail.MailTo = Auth_obj.APP_EMAIL;

                            string struid = (Auth_obj.APPEMP_CODE).ToString();
                            string strid = smHeaderId.ToString();
                            string reqType = SHVM.SM_TYPE == 1 ? "Service Entry Sheet" : "Material Receipt Note";

                            string strSubject = reqType + " Request - " + SHVM.SM_NO + ", Requestor - " + SHVM.Emp_Detail._EName + "(" + SHVM.Emp_Detail._ECode + ")";
                            string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                             "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>" + reqType + " Request For Taxation Review</font></b></td></tr>" +
                                             "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + Auth_obj.APPEMP_NAME + " San ,</br></br> Finance has forwarded you " + reqType + " approval request in Employee Portal for Taxation review. </br></br><b> Below are the details : </b></br></td></tr>" +
                                             "<tr><td width=125 height=23 valign=top>SES/MRN Number: </td><td width=389 valign=top>" + SHVM.SM_NO + "</td></tr>" +
                                             "<tr><td width=125 height=23 valign=top>PO Number: </td><td width=389 valign=top>" + SHVM.PONUMBER + "</td></tr>" +
                                             "<tr><td width=125 height=23 valign=top>Amount: </td><td width=389 valign=top>" + (SHVM.AMOUNT <= 200000 ? "<= 2 Lakh" : "> 2 Lakh") + "</td></tr>" +
                                             //"<tr><td width=125 height=23 valign=top>Remarks: </td><td width=389 valign=top>" + SHVM.REMARK + "</td></tr>" +
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
                        EmailCore sendMail = new EmailCore();
                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        sendMail.MailTo = SHVM.Emp_Detail._EmailId;
                        string reqType = SHVM.SM_TYPE == 1 ? "Service Entry Sheet" : "Material Receipt Note";

                        string strSubject = reqType + " Approval Status - " + RequestStatus;
                        string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                         "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>" + reqType + " Approval Status " + RequestStatus + " - Emp Code (" + SHVM.Emp_Detail._ECode + ")</font></b></td></tr>" +

                                         "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px>" +
                                         "<tr><td valign=top colspan =2>Your " + reqType + " request has been <b>" + RequestStatus + "</b> by " + employeeDetails.Employee_Name + " San. The request details are as follows:</td></tr>" +
                                         "<tr><td width=125 height=22 valign=top>SES/MRN Number: </td><td width=389 valign=top>" + SHVM.SM_NO + "</td></tr>" +
                                         "<tr><td width=125 height=22 valign=top>PO Number: </td><td width=389 valign=top>" + SHVM.PONUMBER + "</td></tr>" +
                                         "<tr><td width=125 height=22 valign=top>Amount: </td><td width=389 valign=top>" + (SHVM.AMOUNT <= 200000 ? "<= 2 Lakh" : "> 2 Lakh") + "</td></tr>" +
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

        public SearchSMViewModel SMTaxationDashboard(SearchSMViewModel SSM, long loginUser)
        {
            return _SMRepo.SMTaxationDashboard(SSM, loginUser);
        }

        public short SMTaxationApproval(SMAppHistoryViewModel SHVM)
        {
            return _SMRepo.SMTaxationApproval(SHVM);
        }

        public List<SMTaxationAuthViewModel> GetTaxationAuthority(short TaxTypeId, long plantId)
        {
            return _SMRepo.GetTaxationAuthority(TaxTypeId, plantId);
        }

        public short UpdateTaxationAuth(List<SMAppHistoryViewModel> iList, long updatedBy)
        {
            var retVal = _SMRepo.UpdateTaxationAuth(iList, updatedBy);
            if (retVal == 1)
            {
                SendMailForTaxationAuth(iList);
            }
            return retVal;
        }

        public short SendMailForTaxationAuth(List<SMAppHistoryViewModel> iList)
        {
            short retVal = 0;
            try
            {
                #region Send mail for Taxation Authority
                foreach (SMAppHistoryViewModel hisObj in iList)
                {
                    SMHeaderViewModel SHVM = _SMRepo.GetSMRequestById(hisObj.SMHEADERID);
                    if (SHVM.smAppHis.Count > 0 && SHVM.PROCESS_STATUS == 7)
                    {
                        SMAppHistoryViewModel Auth_obj = SHVM.smAppHis.Where(a => a.SMAPPHISTORYID > 0 && a.APPROVAL_STATUS == 0 && !string.IsNullOrEmpty(a.APP_EMAIL) && a.ADEMPCODE == hisObj.SELECTED_ADEMPCODE).FirstOrDefault();
                        if (Auth_obj != null)
                        {
                            EmailCore sendMail = new EmailCore();
                            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                            if (serverpath.isTestServer())
                                sendMail.MailTo = serverpath.getTestEMail();
                            else
                                sendMail.MailTo = Auth_obj.APP_EMAIL;

                            string reqType = SHVM.SM_TYPE == 1 ? "Service Entry Sheet" : "Material Receipt Note";
                            string strSubject = reqType + " Request - " + SHVM.SM_NO + ", Requestor - " + SHVM.Emp_Detail._EName + "(" + SHVM.Emp_Detail._ECode + ")";
                            string strBody = "<table cellpadding=0 cellspacing=0  style='border-style:solid; border-color:#009933; border-width:1px; width:600px;'>" +
                                             "<tr bgcolor=#009933><td height=30>&nbsp;<b><font color=#FFFFFF>" + reqType + " Request For Taxation Review </font></b></td></tr>" +
                                             "<tr><td><table cellpadding=4 cellspacing=0 border=0 width=600px><tr><td colspan=2 width=514 valign=top> Dear " + Auth_obj.APPEMP_NAME + " San ,</br></br> Finance has forwarded you " + reqType + " approval request in Employee Portal for Taxation review. </br></br><b> Below are the details : </b></br></td></tr>" +
                                             "<tr><td width=125 height=23 valign=top>SES/MRN Number: </td><td width=389 valign=top>" + SHVM.SM_NO + "</td></tr>" +
                                             "<tr><td width=125 height=23 valign=top>PO Number: </td><td width=389 valign=top>" + SHVM.PONUMBER + "</td></tr>" +
                                             "<tr><td width=125 height=23 valign=top>Amount: </td><td width=389 valign=top>" + (SHVM.AMOUNT <= 200000 ? "<= 2 Lakh" : "> 2 Lakh") + "</td></tr>" +
                                             //"<tr><td width=125 height=23 valign=top>Remarks: </td><td width=389 valign=top>" + SHVM.REMARK + "</td></tr>" +
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
                #endregion
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }

        public List<PR_Div_Dep_SecViewModel> BindDepartment(long div_Id, long op_Id)
        {
            return _SMRepo.BindDepartment(div_Id, op_Id);
        }

        public List<PR_Div_Dep_SecViewModel> BindSection(long dep_Id, long div_Id, long op_Id)
        {
            return _SMRepo.BindSection(dep_Id, div_Id, op_Id);
        }

        public SearchIOCGMaster BindIOCGMasterList(SearchIOCGMaster _SM)
        {
            return _SMRepo.BindIOCGMasterList(_SM);
        }
        public List<PR_Div_Dep_SecViewModel> BindKI()
        {
            return _SMRepo.GetKiLIST();
        }
        public List<VM_VW_SMIOCGAPPROVER_LIST> GetIOCGMasterByID(VM_VW_SMIOCGAPPROVER_LIST _SM)
        {
            return _SMRepo.GetIOCGMasterByID(_SM);
        }
        public Tuple<short, string> SaveIOCGMasterData(VM_VW_SMIOCGAPPROVER_LIST _SM)
        {
            return _SMRepo.SaveIOCGMasterData(_SM);
        }
        public Tuple<short, string> EditIOCGMasterData(VM_VW_SMIOCGAPPROVER_LIST _SM)
        {
            return _SMRepo.EditIOCGMasterData(_SM);
        }
        public Tuple<short, List<VM_PADetailViewModel>> SavePAAttachment(long addedDate, List<VM_PADetailViewModel> modelList, string PA_HeaderID)
        {
            List<VM_PADetailViewModel> iList = new List<VM_PADetailViewModel>();
            short retVal = _SMRepo.SavePADetails(addedDate, modelList);
            if (retVal == 1)
            {
                iList = _SMRepo.GetPAAttachmentDetail(PA_HeaderID);
            }
            Tuple<short, List<VM_PADetailViewModel>> _tuple = new Tuple<short, List<VM_PADetailViewModel>>(retVal, iList);
            return _tuple;
        }
        public short PARequest(VM_PaymentAdvise_Master data)
        {
            short retval = 1;
            retval = _SMRepo.SavePARequest(data);
            return retval;
        }
        public Tuple<short, List<VM_PADetailViewModel>> DeletePAAttachment(string fileName, string docType, long PA_HeaderID, string PA_ID)
        {
            List<VM_PADetailViewModel> iList = new List<VM_PADetailViewModel>();
            short retVal = _SMRepo.DeletePAAttachment(fileName, docType, PA_HeaderID);
            if (retVal == 1)
            {
                iList = _SMRepo.GetPAAttachmentDetail(PA_ID);
            }
            Tuple<short, List<VM_PADetailViewModel>> _tuple = new Tuple<short, List<VM_PADetailViewModel>>(retVal, iList);
            return _tuple;
        }
        public List<VM_PADetailViewModel> PAdocs(string PA_ID)
        {
            return _SMRepo.GetPAAttachmentDetail(PA_ID);
        }
        public List<VM_PaymentAdvise_Master> GetPADetails(VM_PaymentAdvise_Master obj)
        {
            return _SMRepo.GetPADetails(obj);
        }

        public short deletePAReq(DeletePA obj)
        {
            return _SMRepo.deletePAReq(obj);
        }
        public long GetSMNO(string InvoiceNo, DateTime InvoiceDate, string VendorCode)
        {
            return _SMRepo.GetSMNO(InvoiceNo, InvoiceDate, VendorCode);
        }
        //============Below added by aumento on 16052023 for SR50703==========================================================================================
        public List<A00ADORGLEVELList> PortalAutocompleteSuggestionsForORG(long ddOLvType)
        {
            return _SMRepo.GetPortalAutocompleteSuggestionsForORG(ddOLvType);
        }


        public Tuple<short, List<A00ADORGLEVELList>> PortalAutocompleteSuggestionsForORG_New(long ddOLvType)
        {

            List<A00ADORGLEVELList> iList = new List<A00ADORGLEVELList>();

            iList = _SMRepo.PortalAutocompleteSuggestionsForORG_New(ddOLvType);

            Tuple<short, List<A00ADORGLEVELList>> _tuple = new Tuple<short, List<A00ADORGLEVELList>>(0, iList);
            return _tuple;


        }

        public Tuple<short, long> SaveAdorglevel(long AddedBy, string ddOLvType, string Organization)
        {

            return _SMRepo.SaveAdorglevel(AddedBy, ddOLvType, Organization);

        }

        public List<SMDGIT_SMIOCGBLOCKViewModel> GetSMIOCGBLOCKData()
        {
            return _SMRepo.GetSMIOCGBLOCKData();
        }

        public Tuple<short, List<SMDGIT_SMIOCGBLOCKViewModel>> UpdateStatus(long SMIOCGBLOCKID, long ADORGLEVELID, long SRNO)
        {

            List<SMDGIT_SMIOCGBLOCKViewModel> iList = new List<SMDGIT_SMIOCGBLOCKViewModel>();

            iList = _SMRepo.UpdateStatus(SMIOCGBLOCKID, ADORGLEVELID, SRNO);

            Tuple<short, List<SMDGIT_SMIOCGBLOCKViewModel>> _tuple = new Tuple<short, List<SMDGIT_SMIOCGBLOCKViewModel>>(0, iList);
            return _tuple;


        }

        public Tuple<short, List<SMDGIT_SMIOCGBLOCKViewModel>> SaveUpdateStatus(List<SMDGIT_SMIOCGBLOCKViewModel> modelList)
        {
            List<SMDGIT_SMIOCGBLOCKViewModel> iList = new List<SMDGIT_SMIOCGBLOCKViewModel>();
            short retVal = _SMRepo.SaveUpdateStatus(modelList);

            Tuple<short, List<SMDGIT_SMIOCGBLOCKViewModel>> _tuple = new Tuple<short, List<SMDGIT_SMIOCGBLOCKViewModel>>(retVal, iList);
            return _tuple;
        }

        //=====================================================================================================================================================================

        //Added by aumento for SESMRN as on 23112023===========================
        public IEnumerable<SYSITE> Bind_SYSite()
        {
            return _SMRepo.Bind_SYSite();
        }
        public long GETSYSITEUSERID(string UserId)
        {
            return _SMRepo.GETSYSITEUSERID(UserId);
        }
        public SearchSMViewModel SMIPDashboard(SearchSMViewModel VM, long plantId)
        {
            return _SMRepo.SMIPDashboard(VM, plantId);
        }
        //======================================================================

    }
}
