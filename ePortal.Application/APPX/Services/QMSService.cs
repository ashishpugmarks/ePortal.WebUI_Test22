using ePortal.Application.APPX.Contracts;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.ViewModels.APPX.IOM;
using ePortal.ViewModels.APPX.QMS;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Configuration;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ePortal.Application.APPX.Services
{
    public class QMSService : IQMSService
    {
        private readonly ILogger<QMSService> _logger;
        private readonly IQMSRepos _iQMSRepos;
        private readonly ISessionService _sessionService;
        private readonly Employee_Details _empDetails;


       // public string filePath = Path.Combine(serverpath.getFileUploadPath(), "IOM");
        private readonly string _userId;
        private readonly string _userName;
        private readonly string DIVHEAD;
        private readonly string depthead;

        public QMSService(
            IQMSRepos iQMSRepos,
            ISessionService sessionService,
            ISearchEmp searchEmp,
            ILogger<QMSService> logger)
        {
            _logger = logger;
            _iQMSRepos = iQMSRepos;
            _sessionService = sessionService;
            _userId = _sessionService.Get<string>("userID").ToString();
            _userName = _sessionService.Get<string>("userName").ToString();

            _empDetails = _sessionService?.Get<Employee_Details>("Employee");

            DIVHEAD = _empDetails?.Division_Id ?? string.Empty;
            depthead = _empDetails?.Department_Id ?? string.Empty;

        }
       
        public QMSOperationResult ISODOCDetail_Set(ISODocSaveModel model)
        {
            var result = new QMSOperationResult
            {
                Status = false
            };

            try
            {
                string strPlant = string.Empty;
                string strDept = string.Empty;
                string strSection = string.Empty;
                string strChangeType = string.Empty;
                string strIsoDocId = string.Empty;
                string strDocNo = string.Empty;
                string strDocTitle = string.Empty;
                string strReason = string.Empty;
                string filename = string.Empty;
                string StrAttachment = string.Empty;
                string strAppAuth = string.Empty;
                string strAppName = string.Empty;
                string strMessage = string.Empty;
                int strOption = 0;
                string strImpDate = string.Empty;
                string strRevDate = string.Empty;
                string strOldRevNo = string.Empty;
                string strNewRevNo = string.Empty;
                string strRevNoD = string.Empty;
                string strRevNoR = string.Empty;
                string strrevdocno = string.Empty;
                string strEmpcode = _userId;
                strPlant = model.ddlPlant;
                strDept = model.ddlDept;
                //strSection = ddlSection.SelectedValue;

                strSection = "";
                strDocNo = model.ddlformatnumberSelectedText + model.txtDocNo9 + model.txtDocNo + model.txtDocNo1 + model.txtDocNo2;
                strrevdocno = model.ddlformatnumberSelectedText + model.txtDocNo9 + model.txtDocNo + model.txtDocNo1 + model.txtDocNo2;
                strDocNo = strDocNo.ToUpper();
                strDocTitle = model.txtDocTitle;
               
                if (!string.IsNullOrWhiteSpace(model.hdnDocTitle))
                    strDocTitle = model.hdnDocTitle;
                if (model.IsChecked_rbtnAdd)
                {
                    strChangeType = "Addition";
                    strNewRevNo = model.txtDocNo1;
                    strImpDate = model.ddlDayA + "/" + model.ddlMonthASelectedText + "/" + model.ddlYearA;
                }
                else if (model.IsChecked_rbtnRev)
                {
                    strChangeType = "Revision";
                    strOldRevNo = model.txtCurrRevNoR;
                    strOldRevNo = model.hdnCurrRevNoR;
                    strNewRevNo = model.hdnNewRevNoR;
                    strRevDate = model.ddlRevDayR + "/" + model.ddlRevMonthRSelectedText + "/" + model.ddlRevYearR;
                    strImpDate = model.ddlImpDayR + "/" + model.ddlImpMonthRSelectedText + "/" + model.ddlImpYearR;
                    strDocNo = model.ddlformatnumberSelectedText + model.txtDocNo9 + model.txtDocNo + model.hdnNewRevNoR + model.txtDocNo2;
                    strDocNo = strDocNo.ToUpper();
                }
                else
                {
                    strChangeType = "Deletion";
                    strNewRevNo = model.txtCurrRevD;
                    strNewRevNo = model.hdnCurrRevNoR;
                    strImpDate = model.ddlDayD + "/" + model.ddlMonthDSelectedText + "/" + model.ddlYearD;
                }
                strReason = model.txtReason;
                //strAppAuth = Convert.ToString(ViewState["APPECODE"]);
                strAppAuth = model.hdstrAppAuth;
                strAppName = model.lblAppAuth;
                if (model.txtFileUpload != null && model.txtFileUpload.Length != 0)
                {
                    filename = model.txtFileUpload.FileName;
                    strMessage = uploadFile(model.txtFileUpload);

                    string[] strMessageArray = strMessage.Split(new Char[] { '@' });
                    string strMessageStatus = strMessageArray[0].ToString();
                    if (strMessageStatus == "1")
                    {
                        StrAttachment = strMessageArray[1];
                    }
                    else
                    {
                        result = new QMSOperationResult
                        {
                            Status = false,
                            Message = strMessageArray[1],
                            obj = new { errorpanel_Visible = true }
                        };
                        return result;
                    }
                }
                else
                {
                    //  StrAttachment = Convert.ToString(ViewState["FilenameExist"]); //vks
                    StrAttachment = model.hdFilenameExist;
                }

                // strIsoDocId = Convert.ToString(ViewState["IsoDocId"]); //vks
                strIsoDocId = model.hdIsoDocId;
                strOption = 1;     // 1 for insert

                int rev = Convert.ToInt32(model.txtDocNo1);

                string strResult = _iQMSRepos.ISODOCDetail_Set(strIsoDocId, strDocNo, strDocTitle, strChangeType,
                                                           strPlant, strDept, strSection, strOldRevNo,
                                                           strRevDate, strNewRevNo, strImpDate, StrAttachment,
                                                           strEmpcode, strEmpcode, strReason, strAppAuth,
                                                           "", strOption, strrevdocno.ToUpper());
                string[] strResStatus = strResult.Split(new Char[] { '#' });
                string errResult = Convert.ToString(strResStatus[0]);
                string errMsg = Convert.ToString(strResStatus[1]);
                if (errResult == "0")
                {

                    result = new QMSOperationResult
                    {
                        Status = false,
                        Message = errMsg,
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else
                {
                  //  string strappauthemailid = Convert.ToString(ViewState["APPEMAILID"]); //Email send to Approval authority          
                    string strappauthemailid = model.hdAPPEMAILID; //Email send to Approval authority         //vks
                    string[] strAppName1 = strAppName.Split('[');
                    SendMail(strappauthemailid, strAppName1[0].ToString(), strDocTitle, strDocNo, strChangeType, strReason);

                    if (rev != 00 && model.IsChecked_rbtnAdd)
                    {
                        result = new QMSOperationResult
                        {
                            Status = true,
                            Message = "Your Details Submitted Successfully. Revision no. is not 00",
                            obj = new { IsWindowAlert = true , redirectionTo = "ManageRequest#QMS" }
                        };

                       // ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Your Details Submitted Successfully. Revision no. is not 00');window.location.href ='../ManageRequest.aspx#QMS';", true);
                    }
                    else
                    {
                        result = new QMSOperationResult
                        {
                            Status = true,
                            Message = "Your Details Submitted Successfully.",
                            obj = new { IsWindowAlert = true, redirectionTo = "ManageRequest#QMS" }
                        };

                        //ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Your Details Submitted Successfully.');window.location.href ='../ManageRequest.aspx#QMS';", true);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                result = new QMSOperationResult
                {
                    Status = false,
                    Message = ex.Message,
                    obj = new { IsWindowAlert = true }
                };
            }
            return result;
        }


        public QMSOperationResult ISODOCDetail_UPDATE(ISODocUpdateModel model)
        {
            var result = new QMSOperationResult
            {
                Status = false
            };
            try
            {
                string strPlant = string.Empty;
                string strDept = string.Empty;
                string strSection = string.Empty;
                string strChangeType = string.Empty;
                string strIsoDocId = string.Empty;
                string strOldIsoDocId = string.Empty;
                string strDocNo = string.Empty;
                string strDocTitle = string.Empty;
                string strReason = string.Empty;
                string filename = string.Empty;
                string StrAttachment = string.Empty;
                string strAppAuth = string.Empty;
                string strMessage = string.Empty;
                int strOption = 0;

                string strImpDate = string.Empty;
                string strRevDate = string.Empty;
                string strOldRevNo = string.Empty;
                string strNewRevNo = string.Empty;

                string strRevNoD = string.Empty;
                string strRevNoR = string.Empty;
                string strEmpcode = _userId;

                strPlant = model.ddlPlant;
                strDept = model.ddlDept;
                strSection = model.ddlSection ?? string.Empty;
                strDocNo = model.ddlformatnumberSelectedText + model.txtDocNo9 + model.txtDocNo + model.txtDocNo1 + model.txtDocNo2;
                strDocNo = strDocNo.ToUpper();

                strDocTitle = model.txtDocTitle ?? string.Empty;

                if (model.IsChecked_rbtnAdd)
                {
                    strChangeType = "Addition";
                    strNewRevNo = model.txtDocNo1;
                    strImpDate = model.ddlDayA + "/" + model.ddlMonthASelectedText + "/" + model.ddlYearA;
                }
                else if (model.IsChecked_rbtnRev)
                {
                    strChangeType = "Revision";
                    strOldRevNo = model.txtCurrRevNoR ?? string.Empty;
                    strOldRevNo = model.hdnCurrRevNoR ?? string.Empty;
                    strNewRevNo = model.hdnNewRevNoR ?? string.Empty;
                    strRevDate = model.ddlRevDayR + "/" + model.ddlRevMonthRSelectedText  + "/" + model.ddlRevYearR;
                    strImpDate = model.ddlImpDayR + "/" + model.ddlImpMonthRSelectedText  + "/" + model.ddlImpYearR;
                    strDocTitle = model.hdnDocTitle ?? string.Empty;
                }
                else
                {
                    strChangeType = "Deletion";
                    strNewRevNo = model.txtCurrRevD ?? string.Empty;
                    strNewRevNo = model.hdnCurrRevNoR ?? string.Empty;
                    strDocTitle = model.hdnDocTitle ?? string.Empty;
                    strImpDate = model.ddlDayD + "/" + model.ddlMonthDSelectedText + "/" + model.ddlYearD;
                }

                strReason = model.txtReason ?? string.Empty;
                strAppAuth = model.hdstrAppAuth ?? string.Empty;

                if (model.txtFileUpload != null && model.txtFileUpload.Length != 0)
                {
                    filename = model.txtFileUpload.FileName;
                    strMessage = uploadFile(model.txtFileUpload);
                    string[] strMessageArray = strMessage.Split(new Char[] { '@' });
                    string strMessageStatus = strMessageArray[0].ToString();
                    if (strMessageStatus == "1")
                    {
                        StrAttachment = strMessageArray[1];
                    }
                    else
                    {
                        result = new QMSOperationResult
                        {
                            Status = false,
                            Message = strMessageArray[1],
                            obj = new { errorpanel_Visible = true }
                        };
                        return result;
                    }
                }
                else
                {
                    StrAttachment = model.hdFilenameExist ?? string.Empty;
                }

                strIsoDocId = model.hdIsoDocId ?? string.Empty;
                strOption = 2;     // 2 FOR UPDATE AND DELETE
                strOldIsoDocId = model.OldIsoDocId ?? string.Empty;

               

                string strResult = _iQMSRepos.ISODOCDetail_UPDATE(strIsoDocId, strDocNo, strDocTitle, strChangeType, strPlant, strDept, strSection, strOldRevNo, strRevDate,
                     strNewRevNo, strImpDate, StrAttachment, strEmpcode, strEmpcode, strReason, strAppAuth, strOldIsoDocId, strOption);


                string[] strResStatus = strResult.Split(new Char[] { '#' });
                string errResult = Convert.ToString(strResStatus[0]);
                string errMsg = Convert.ToString(strResStatus[1]);

                if (errResult == "0")
                {
                    result = new QMSOperationResult
                    {
                        Status = false,
                        Message = errMsg,
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else
                {
                    string message = string.Empty;
                    if (model.txtDocNo1 != "00" && model.IsChecked_rbtnAdd)
                    {
                        message = "Revision no. is not 00";
                    }
                    else
                    {
                        message = "Your Details Updated Successfully.";
                    }

                    result = new QMSOperationResult
                    {
                        Status = true,
                        Message = message,
                        obj = new { IsWindowAlert = true, redirectionTo = "ManageRequest#QMS" }
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                result = new QMSOperationResult
                {
                    Status = false,
                    Message = ex.Message,
                    obj = new { IsWindowAlert = true }
                };
            }

            return result;
        }


        public QMSOperationResult ISODocRequestAppReject(ISODocRequestAppRejectSaveModel model)
        {
            var result = new QMSOperationResult
            {
                Status = false
            };
            try
            {
                string strCancel = string.Empty;
                string strAppRemarks = string.Empty;
                string strAppStatus = string.Empty;
                string strEmpStatus = string.Empty;
                string strEmpcode = string.Empty;

                string RequestID = string.Empty;
                string RequestIDC = string.Empty;
                string RequestIDA = string.Empty;
                string strStatus = string.Empty;
                string Approvaltype = string.Empty;

                RequestIDC = model.Cancelid;
                RequestIDA = model.Appid;


                if (!string.IsNullOrWhiteSpace(RequestIDC))
                {
                    RequestID = model.Appid;
                    strCancel = model.txtCancallation ?? string.Empty;
                    strEmpStatus = "0";
                    strStatus = "0";
                    strEmpcode = _userId;
                    Approvaltype = "1";
                }
                else
                {
                    RequestID = model.Appid;
                    strAppRemarks = model.txtAPPRemarks ?? string.Empty;
                    strAppStatus = model.rblApprovalStatus ?? string.Empty;
                    if (strAppStatus == "1")
                        strStatus = "1";
                    else
                        strStatus = "0";
                    strEmpcode = _userId;
                    Approvaltype = "2";
                }
                string isnextappauthset = _iQMSRepos.CHECKISPLANTAPPAUTHSET(model.hdnplant ?? string.Empty);
                if (isnextappauthset == "0")
                {
                    result = new QMSOperationResult
                    {
                        Status = false,
                        Message = "QC-Plant Approval Authority is not set.",
                        obj = new { IsWindowAlert = true }
                    };
                    return result; //vks
                }

                string strResult = _iQMSRepos.ISODOCAPPROVAL(RequestID, strEmpcode, strStatus, strAppRemarks, "", Approvaltype);
                //string strResult = objqms.ISODOCRemark_Set(RequestID, strEmpcode, strCancel, strEmpStatus, strEmpcode, strAppRemarks, strAppStatus, "", "", "", strStatus, "", "");

                string[] strResStatus = strResult.Split(new Char[] { '#' });
                string errResult = Convert.ToString(strResStatus[0]);
                string errMsg = Convert.ToString(strResStatus[1]);

                if (errResult == "0")
                {
                    result = new QMSOperationResult
                    {
                        Status = false,
                        Message = errMsg,
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(RequestIDC))
                    {
                        result = new QMSOperationResult
                        {
                            Status = true,
                            Message = "QMS document is Cancelled Successfully.",
                            obj = new { IsWindowAlert = true, redirectionTo = "ManageRequest#QMS" }
                        };
                    }
                    else
                    {
                        if ((strAppStatus == "1") || (strAppStatus == "2"))
                        {
                            string strEmpEmail = model.EmpEmail ?? string.Empty;
                            SendMail_ISODocRequestAppReject(strEmpEmail, model.lblEmpCode ?? string.Empty, model.lblEmpName ?? string.Empty, model.lblDocNo ?? string.Empty, model.lblDocTitle ?? string.Empty, strAppRemarks, strAppStatus, model.chg ?? string.Empty);
                        }
                        string message = string.Empty;
                        if (strAppStatus == "1")
                        {
                            message = "QMS document is Approved Successfully.";
                        }
                        else
                        {
                            message = "QMS document is Rejected Successfully.";
                        }

                        result = new QMSOperationResult
                        {
                            Status = true,
                            Message = message,
                            obj = new { IsWindowAlert = true, redirectionTo = "ManageApproval#QMS" }
                        };

                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                result = new QMSOperationResult
                {
                    Status = false,
                    Message = ex.Message,
                    obj = new { IsWindowAlert = true }
                };
            }

            return result;
        }


        public IEnumerable<SubQcProcViewModel> SubQcProc(string ID)
        {
            IEnumerable<SubQcProcViewModel> list = new List<SubQcProcViewModel>();

            DataSet objDs = _iQMSRepos.getQmsstandards(ID, "1");
            if(objDs != null && objDs.Tables[0].Rows.Count > 0)
            {
                list = objDs.Tables[0].AsEnumerable()
                      .Select(row => new SubQcProcViewModel
                      {
                          QCQMSDOCID = Convert.ToString(row.Field<object>("QCQMSDOCID")),
                          DESCRIPTION = Convert.ToString(row.Field<object>("DESCRIPTION")),
                          FILENAME = Convert.ToString(row.Field<object>("FILENAME")),
                          STANDARDNO = Convert.ToString(row.Field<object>("STANDARDNO")),
                          REVISIONNO = Convert.ToString(row.Field<object>("REVISIONNO")),
                          APPLICABLEDATE = Convert.ToString(row.Field<object>("APPLICABLEDATE")),
                      }).ToList();
            }


            return list;
        }

        public IEnumerable<DeptSectProcViewModel> DeptSectProcList(string DEPTID, string PLANTID)
        {
            IEnumerable<DeptSectProcViewModel> list = new List<DeptSectProcViewModel>();

            DataSet objDs = _iQMSRepos.DeptSectProcList(DEPTID, PLANTID);
            if (objDs != null && objDs.Tables[0].Rows.Count > 0)
            {
                list = objDs.Tables[0].AsEnumerable()
                      .Select(row => new DeptSectProcViewModel
                      {
                          DOCNO = Convert.ToString(row.Field<object>("DOCNO")),
                          NEWREVISIONNO = Convert.ToString(row.Field<object>("NEWREVISIONNO")),
                          Description = Convert.ToString(row.Field<object>("Description")),
                          Filename = Convert.ToString(row.Field<object>("Filename")),
                      }).ToList();
            }


            return list;
        }

        public IEnumerable<GrdDeptProcViewModel> GrdDeptProcList(string? plant, string? DEPTID)
        {
            IEnumerable<GrdDeptProcViewModel> list = new List<GrdDeptProcViewModel>();

            DataTable dt = new DataTable();
            if ((!string.IsNullOrWhiteSpace(plant) && !string.IsNullOrWhiteSpace(DEPTID)) || !string.IsNullOrEmpty(DEPTID))
            {
                dt = _iQMSRepos.QMSDeptProc_ByPlantID(plant ?? string.Empty, DEPTID ?? string.Empty);
            }
            else
            {
                DataSet ds = _iQMSRepos.QMSDeptProc();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(plant))
                    {
                        ds.Tables[0].DefaultView.RowFilter = "PLANTID=" + plant + "";
                        dt = ds.Tables[0].DefaultView.ToTable();
                    }
                    else
                    {
                        dt = ds.Tables[0];
                    }
                }
            }

            if(dt.Rows.Count > 0)
            {
                list = dt.AsEnumerable()
                     .Select(row => new GrdDeptProcViewModel
                     {
                         Descrip = Convert.ToString(row.Field<object>("Descrip")),
                         Plant = Convert.ToString(row.Field<object>("Plant")),
                         ADDEPARTMENTID = Convert.ToString(row.Field<object>("ADDEPARTMENTID")),
                         PLANTID = Convert.ToString(row.Field<object>("PLANTID")),
                     }).ToList();
            }

            return list;
        }




        #region PRIVATE METHOD
        private void SendMail_ISODocRequestAppReject(string strEmail, string strEmpCode, string strEmpName, string strDocNo, string strDocTitle, string strAppRemarks, string strAppStatus, string chg)
        {
            commanEmail sendMail = new commanEmail();
            string str_Status = string.Empty;
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";

            if (serverpath.isTestServer())
                sendMail.MailTo = serverpath.getTestEMail();
            else
                sendMail.MailTo = strEmail;

            string strSubject = "QMS Document Change Request Apporval from  " + _userName + " [EmpCode - " + _userId + "].";
            if (strAppStatus == "1")
                str_Status = "This is to inform you that " + _userName + " - Emp Code (" + _userId + ") has <b><font color=green>Approved</font></b> your QMS Document Request. The details are as follows-";
            else
                str_Status = "This is to inform you that " + _userName + " - Emp Code (" + _userId + ") has <b><font color=red>Rejected</font></b> your QMS Document Request. The details are as follows-";


            string strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                 "<tr><td  height=35><img src=" + serverpath.getServerPath() + "assets//images//HondaLogo5.gif border=0 /></td>" +
                 "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                 "</td></tr>" +
                 "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "assets//images/Table_layout_04.gif height=30>&nbsp;" +
                 "<b>QMS Document Change Request Approval Information</b></td></tr><tr height=150><td colspan=2>" +
                 "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                 "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                 "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                 "<tr><td colspan=3>&nbsp;</td></tr><tr><td valign=top colspan=2><p><b>Dear " + strEmpName + " San,</b><br />" +
                 "<br />" + str_Status + "<br />" +
                 "<br /></td></tr><tr>" +
                "<td width=125 valign=top><b>Document Title</b></td><td width=389 valign=top>" + "<b>: </b>" + strDocTitle +
                "</td></tr><tr><td width=125 valign=top><b>Document No.</b></td>" + "<td width=389 valign=top>" + "<b>: </b>" + strDocNo + "</td></tr>" +
                "<tr><td width=125 valign=top><b>Change Type</b></td>" + "<td width=389 valign=top>" + "<b>: </b>" + chg + "</td></tr>" +
                "<td width=125 valign=top><b>Reason</b></td>" + "<td width=389 valign=top>" + "<b>: </b>" + strAppRemarks + "</td></tr>" +
                 "<tr valign=bottom> " +
                 "<td colspan=2><br /><b>Best Regards</b><br /> HMSI-QS Secretariat<br /><br /><strong>Note: It is a system generated email, please do not reply.</strong></td> " +
                 "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                 "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "assets//images//Table_layout_06.gif border=0 /></td> " +
                 "</tr></table> ";
            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            try
            {
                bool status = sendMail.Send();
            }
            catch (Exception ex)
            {
                //Response.Write("Exception Occured:   " + ex);
            }
        }

        private string uploadFile(IFormFile? txtFileUpload)
        {
            string filePath = serverpath.getFileUploadPath() + "ISODoc";
            string strReturnVal = "0@Invalid File Extension";
            Random rand = new Random();

            if (txtFileUpload != null && txtFileUpload.Length != 0)
            {
                try
                {
                    string exten = System.IO.Path.GetExtension(txtFileUpload.FileName).ToLower();
                    if (exten == ".pdf")
                    {
                        if (string.IsNullOrWhiteSpace(filePath))
                        {
                            strReturnVal = "0@Path not found";
                            return strReturnVal;
                        }
                        if (txtFileUpload.Length == 0)
                        {
                            strReturnVal = "0@Invalid file content";
                        }
                        else if (txtFileUpload.Length <= 83318784)    //9.9 mb
                        {
                            int a = rand.Next();
                            string fileNo = a.ToString() + exten.ToString();
                            string fullMpath = Path.Combine(filePath, fileNo);
                        
                            // Save file
                            using (var stream = new FileStream(fullMpath, FileMode.Create))
                            {
                                txtFileUpload.CopyTo(stream);
                            }
                            strReturnVal = "1@"+ fileNo;
                        }
                        else
                        {
                            strReturnVal = "0@Unable to upload, File exceeds maximum limit";
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                    strReturnVal = "0@There is some Error while Uploading";
                }
            }
            return strReturnVal;
        }
        private void SendMail(string strApprovalAuthEmail, string strappname, string strDocTitle, string strDocNo, string strChangeType, string strReason)
        {
            commanEmail sendMail = new commanEmail();
            sendMail.MailFrom = "portal.admin@honda.hmsi.in";

            if (serverpath.isTestServer())
                sendMail.MailTo = serverpath.getTestEMail();
            else
                sendMail.MailTo = strApprovalAuthEmail;

            strDocNo = strDocNo.Substring(0, 7) + "-" + strDocNo.Substring(7, 4) + "-" + strDocNo.Substring(11, 1) + "-" + strDocNo.Substring(12, 2) + "-" + strDocNo.Substring(14, 4);
            string strSubject = "QMS Document Change Request from - " + _userName + " (Employee Code: " + _userId + ")";
            string strBody = "<table cellpadding=0 cellspacing=0 border=0 width=600 class=smalltext>" +
                "<tr><td  height=35><img src=" + serverpath.getServerPath() + "assets//images//HondaLogo5.gif border=0 /></td>" +
                "<td align=right valign=bottom style='FONT-SIZE: 11px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica;'>" +
                "</td></tr>" +
                "<tr><td colspan=2 height=3></td></tr><tr><td colspan=2 bgcolor=#bcddf6 background=" + serverpath.getServerPath() + "assets//images/Table_layout_04.gif height=30>&nbsp;" +
                "<b>QMS Document Change Request for Approval</b></td></tr><tr height=150><td colspan=2>" +
                "<table cellpadding=0 cellspacing=0 border=0 width=100% bgcolor=#bcddf6><tr>" +
                "<td bgcolor=#bcddf6 width=6px>&nbsp;</td><td width=588 height=250 bgcolor=#FFFFFF valign=top>" +
                "<table cellpadding=3 cellspacing=0 border=0 width=100% style='FONT-SIZE: 12px; COLOR: #333333; FONT-FAMILY: Verdana, Arial, Helvetica'>" +
                "<tr><td valign=top colspan=2><p><b><br />Dear " + strappname + " San,</b><br />" +
                "<br />" +
                "QMS Document Change Request has been raised by " + _userName + " - Emp Code (" + _userId + "). The details are as follows:" + "<br />" +
                "<br /></td></tr><tr>" +
               "<td width=125 valign=top><b>Document Title</b></td><td width=389 valign=top>" + "<b>: </b>" + strDocTitle +
               "</td></tr><tr><td width=125 valign=top><b>Document No.</b></td>" + "<td width=389 valign=top>" + "<b>: </b>" + strDocNo + "</td></tr>" +
               "<tr><td width=125 valign=top><b>Change Type</b></td>" + "<td width=389 valign=top>" + "<b>: </b>" + strChangeType + "</td></tr>" +
                "</tr><tr><td width=125 valign=top><b>Reason</b></td>" + "<td width=389 valign=top>" + "<b>: </b>" + strReason + "</td></tr>" +
                "<tr valign=bottom>" +
                "<td colspan=2><br /><b>Best Regards</b><br /> HMSI-QS Secretariat<br /><br /><strong><br />Note: It is a system generated email, please do not reply.</strong></td> " +
                "</tr></table></td><td bgcolor=#bcddf6 colspan=2>&nbsp;</td> " +
                "</tr></table></td></tr><tr><td colspan=2><img src= " + serverpath.getServerPath() + "assets//images//Table_layout_06.gif border=0 /></td> " +
                "</tr></table> ";

            sendMail.MailSubject = strSubject;
            sendMail.MailBody = strBody;
            try
            {
                bool status = sendMail.Send();
            }
            catch (Exception ex)
            {
                //Response.Write("Exception Occured:   " + ex);
            }
        }
        #endregion
    }
}
