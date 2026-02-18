using ePortal.Application.APPX.Contracts;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.ViewModels.APPX.IOM;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace ePortal.Application.APPX.Services
{
    public class IOMContractService : IIOMContractService
    {
        private readonly ILogger<IOMContractService> _logger;
        private readonly IIOMContractRepos _iOMContractRepos;
        private readonly ISessionService _sessionService;
        private readonly Employee_Details _empDetails;


        public string filePath = Path.Combine(serverpath.getFileUploadPath(), "IOM");
        private readonly string _userId;
        private readonly string DIVHEAD;
        private readonly string depthead;

        public IOMContractService(
            IIOMContractRepos iOMContractRepos,
            ISessionService sessionService,
            ISearchEmp searchEmp,
            ILogger<IOMContractService> logger)
        {
            _logger = logger;
            _iOMContractRepos = iOMContractRepos;
            _sessionService = sessionService;
            _userId = _sessionService.Get<string>("userID").ToString();

            _empDetails = _sessionService?.Get<Employee_Details>("Employee");

            DIVHEAD = _empDetails?.Division_Id ?? string.Empty;
            depthead = _empDetails?.Department_Id ?? string.Empty;

        }


        public string RemoveSpecialChar(string str)
        {
            string newstr = Regex.Replace(str, @"[^0-9a-zA-Z ]+", "");
            return newstr;
        }

        public IOMContractOperationResult Insert_IOMDetail(IOMRequestFormSaveModel model)
        {
            var result = new IOMContractOperationResult
            {
                Status = false
            };

            try
            {
                string fromiom = model.ddlfrom;
                string agreementtype = model.ddlagreement;
                string agreementtypedesc = model.ddlagreementText;
                string Contracttype = model.ddlcontract;
                string Contracttypedesc = model.ddlcontractText;
                string EffectiveDate = Convert.ToString(model.optED + "-" + model.optEM + "-" + model.optEY);
                string Termmonth = model.ddlTermmonth;
                string Termyear = model.ddlTermyear;
                string Dateofexpiry = Convert.ToString(model.hdnexpiryday + "-" + model.hdnexpirymon + "-" + model.hdnexpiryyear);
                string Mannerofpayment = model.ddlMannerOfPayment;
                string Purpose = model.txtPurpose;
                string Remarks = model.txtRemarks;
                string VendorName = Convert.ToString(HttpUtility.HtmlEncode(model.txtvendorname));
                string ADDEDBY = _userId;
                string LSTMODIFYBY = _userId;
                string agreementfile = string.Empty;
                string approvalnotefile = string.Empty;
                string returnfromdate = Convert.ToString(model.ddlreturnd + "-" + model.ddlreturnm + "-" + model.ddlreturny);
                string returntodate = Convert.ToString(model.ddlreturn2d + "-" + model.ddlreturn2m + "-" + model.ddlreturn2y);

                //This block working on next approval authority is operating head
                string OPHEADECODE = string.Empty;
                if (model.hdauthlevel == "4" && !string.IsNullOrWhiteSpace(model.ddl_appauth))
                {
                    string AUTHVAL = model.ddl_appauth;
                    string[] AUTHARR = AUTHVAL.Split(new Char[] { '#' });
                    OPHEADECODE = Convert.ToString(AUTHARR[0]);
                    model.hdauthcode = Convert.ToString(AUTHARR[0]);
                    model.hdauthname = Convert.ToString(AUTHARR[1]);
                    model.hdauthemailid = Convert.ToString(AUTHARR[2]);
                }
                string addedby = _userId;
                string strrdomst = model.rdomaster;
                string strmstagr = model.ddlmstagreement;
                string strfilename = string.Empty;
                string fullpath = string.Empty;
                string file_name = string.Empty;
                string filename = string.Empty;
                string agreementid = string.Empty;
                string antibribery = string.Empty;
                string ndadocument = string.Empty;
                string considerable = string.Empty;

                //Agreement File
                agreementfile =  model.fileUploadAttachmentAA != null ? model.fileUploadAttachmentAA.FileName : string.Empty;

                //Reference File
                approvalnotefile = model.fileUploadAttachmentRA != null ? model.fileUploadAttachmentRA.FileName : string.Empty;

                ////NDA Document
                //string[] strndadocument;
                //strndadocument = hdnndadocument.Value.Split(new Char[] { '\\' });
                //ndadocument = strndadocument[strndadocument.Length - 1].ToString();

                string Cday = Convert.ToString(System.DateTime.Now.Day);
                string Cmonth = Convert.ToString(System.DateTime.Now.Month);
                string Cyear = Convert.ToString(System.DateTime.Now.Year);
                DateTime cdate = DateTime.Now;
                string strDate = Convert.ToString(model.optDay + "-" + model.optMonth + "-" + model.optYear);
                DateTime uselectdate = Convert.ToDateTime(strDate);
                DateTime uselectdateED = DateTime.Parse(EffectiveDate);

                if (string.IsNullOrWhiteSpace(model.lbl_appauth) && string.IsNullOrWhiteSpace(model.ddl_appauth))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Please select Next Approval Authority",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (Convert.ToInt32(model.ddlagreement) == 0)
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Agreement Type is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                   
                }
                else if (string.IsNullOrWhiteSpace(model.ddlcontract))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Contract Type is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (uselectdate < cdate)
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Date of expiry should not be less than Effective date",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (Convert.ToInt32(model.ddlTermmonth) == 0 && Convert.ToInt32(model.ddlTermyear) == 0)
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Term is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (string.IsNullOrWhiteSpace(model.txtvendorname))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Vendor name is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (string.IsNullOrWhiteSpace(model.ddlMannerOfPayment))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Manner of payment is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (string.IsNullOrWhiteSpace(strrdomst))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Please select Yes if you want to make master of this agreement otherwise select No",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (string.IsNullOrWhiteSpace(model.txtPurpose))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Purpose is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (string.IsNullOrWhiteSpace(model.txtRemarks))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Remarks is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (strrdomst == "1" && string.IsNullOrWhiteSpace(strmstagr))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Please select Master Agreement",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else
                {
                    bool IsCheckNextValidation = true;

                    if (agreementtype == "2")
                    {
                        if (string.IsNullOrEmpty(agreementfile.Trim()))
                        {
                            result = new IOMContractOperationResult
                            {
                                Status = false,
                                Message = "Agreement document attachment mandatory",
                                obj = new { errorpanel_Visible = true }
                            };
                            IsCheckNextValidation = false;
                        }
                        //else if (string.IsNullOrEmpty(approvalnotefile.Trim()))
                        //{
                        //    errorpanel.Visible = true;
                        //    status.Text = "Approval Note document attachment mandatory";
                        //    txtup.Focus();
                        //    return;
                        //}
                    }
                    //else if (agreementtype == "1")
                    //{
                    //    if (string.IsNullOrEmpty(approvalnotefile.Trim()))
                    //    {
                    //        errorpanel.Visible = true;
                    //        status.Text = "Approval Note document attachment mandatory";
                    //        txtup.Focus();
                    //        return;
                    //    }
                    //}
                    //if (agreementfile == approvalnotefile)
                    //{
                    //    errorpanel.Visible = true;
                    //    status.Text = "Agreement and approval note attachment should not be same";
                    //    txtup.Focus();
                    //    return;
                    //}

                    //if ((agreementtype == "2" || agreementtype == "1") && strrdomst == "0")
                    //{

                    //    //NDA Document
                    //    if (string.IsNullOrEmpty(ndadocument.Trim()))
                    //    {
                    //        errorpanel.Visible = true;
                    //        status.Text = "NDA Document attachment mandatory";
                    //        txtup.Focus();
                    //        return;
                    //    }
                    //}
                    //////////////////Agreement doc upload/////////////////////
                    if (IsCheckNextValidation && !string.IsNullOrWhiteSpace(agreementfile))
                    {
                        agreementfile = agreementuploadFile(model.fileUploadAttachmentAA, model.txtvendorname);
                        string[] a = agreementfile.Split(new Char[] { '@' });
                        string b = a[0].ToString();
                        if (b == "1")
                        {
                            agreementfile = a[1].ToString();
                        }
                        else
                        {
                            result = new IOMContractOperationResult
                            {
                                Status = false,
                                Message = a[1],
                                obj = new { errorpanel_Visible = false, IsWindowAlert = true }
                            };
                            IsCheckNextValidation = false;
                        }
                    }
                    //////////////////Reference doc upload/////////////////////
                    if (IsCheckNextValidation && !string.IsNullOrWhiteSpace(approvalnotefile))
                    {
                        approvalnotefile = refuploadFile(model.fileUploadAttachmentRA, model.txtvendorname);
                        string[] a = approvalnotefile.Split(new Char[] { '@' });
                        string b = a[0].ToString();
                        if (b == "1")
                        {
                            approvalnotefile = a[1].ToString();
                        }
                        else
                        {
                            result = new IOMContractOperationResult
                            {
                                Status = false,
                                Message = a[1],
                                obj = new { errorpanel_Visible = false, IsWindowAlert = true }
                            };
                            IsCheckNextValidation = false;
                        }
                    }


                    //////////////////// NDA Document doc upload is now considerable /////////////////////
                    //if (!string.IsNullOrEmpty(ndadocument.Trim()))
                    //{
                    //    fullpath = Path.Combine(filePath, ndadocument);
                    //    ndadocument = ndauploadFile(ndadocument, filePath);
                    //    string[] a = ndadocument.Split(new Char[] { '@' });
                    //    string b = a[0].ToString();
                    //    if (b == "1")
                    //    {
                    //        ndadocument = a[1].ToString();
                    //    }
                    //    else
                    //    {
                    //        errorpanel.Visible = false;
                    //        Response.Write("<script>alert('" + a[1] + "')</script>");
                    //        return;
                    //    }
                    //}
                    considerable = model.txtconsiderable;
                    /////////////////////////////////////////////////////////////////////

                    // Start Added by Aumento :: SR79956
                    int IT_DeclareValue = model.CheckDeclaration == "1" ? 1 : 0;

                    string selectedValue = model.ddlfromText;
                    if (selectedValue == "Information Technology")
                    {
                        if (IT_DeclareValue != 1)
                        {
                            result = new IOMContractOperationResult
                            {
                                Status = false,
                                Message = "You must check the declaration checkbox to proceed.",
                                obj = new { errorpanel_Visible = true }
                            };
                            IsCheckNextValidation = false;
                        }
                    }
                    // End Added by Aumento :: SR79956

                    if (IsCheckNextValidation)
                    {
                        string strErrMsg = _iOMContractRepos.Insert_IOMDetail(fromiom, agreementtype, Contracttype, EffectiveDate, Termmonth, Termyear, Dateofexpiry, Mannerofpayment, Purpose, Remarks, addedby, agreementfile, approvalnotefile, VendorName, agreementid, antibribery, considerable, model.hdauthlevel, strmstagr, depthead, DIVHEAD, OPHEADECODE, model.txtcontactecode, model.txtsuretyamount, model.txtnamesurety, returnfromdate, returntodate, IT_DeclareValue); // IT_DeclareValue  Added by Aumento :: SR79956

                        string[] outputmsg = strErrMsg.Split(new Char[] { '#' });
                        string errResult = Convert.ToString(outputmsg[0]);
                        string errMsg = Convert.ToString(outputmsg[1]);

                        if (errResult == "1")
                        {
                            result = new IOMContractOperationResult
                            {
                                Status = true,
                                Message = string.Empty,
                                obj = new { redirectionTo = "/IOMContract/ManageContract" }
                            };
                            SendMail(agreementtypedesc, Contracttypedesc, EffectiveDate, Dateofexpiry, VendorName, model.hdauthcode, model.hdauthname, model.hdauthemailid);
                        }
                        else
                        {
                            result = new IOMContractOperationResult
                            {
                                Status = false,
                                Message = RemoveSpecialChar(errMsg),
                                obj = new { IsWindowAlert = true }
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                result = new IOMContractOperationResult
                {
                    Status = false,
                    Message = RemoveSpecialChar(ex.Message),
                    obj = new { IsWindowAlert = true }
                };
            }

            return result;
        }


        public IOMContractOperationResult UPDATEIOMDETAIL(IOMRequestFormSaveModel model)
        {
            var result = new IOMContractOperationResult
            {
                Status = false
            };

            try
            {
                string authlevel = model.hdauthlevel;
                //This block working on next approval authority is operating head
                string OPHEADECODE = string.Empty;
                if (model.hdauthlevel == "4" && model.ddl_appauth != "")
                {
                    string AUTHVAL = model.ddl_appauth;
                    string[] AUTHARR = AUTHVAL.Split(new Char[] { '#' });
                    model.hdauthcode = Convert.ToString(AUTHARR[0]);
                    OPHEADECODE = Convert.ToString(AUTHARR[0]);
                    model.hdauthname = Convert.ToString(AUTHARR[1]);
                    model.hdauthemailid = Convert.ToString(AUTHARR[2]);
                }
                string fromiom = Convert.ToString(model.ddlfrom);
                string agreementtype = model.ddlagreement;
                string agreementtypedesc = model.ddlagreementText;
                string Contracttype = model.ddlcontract;
                string Contracttypedesc = model.ddlcontractText;
                string EffectiveDate = Convert.ToString(model.optED + "-" + model.optEM + "-" + model.optEY);
                string Termmonth = Convert.ToString(model.ddlTermmonth);
                string Termyear = Convert.ToString(model.ddlTermyear);
                string Dateofexpiry = string.Empty;
                if (string.IsNullOrEmpty(model.hdnexpiryday))
                {
                    Dateofexpiry = Convert.ToString(model.optDay + "-" + model.optMonth + "-" + model.optYear);
                }
                else
                {
                    Dateofexpiry = Convert.ToString(model.hdnexpiryday + "-" + model.hdnexpirymon + "-" + model.hdnexpiryyear);
                }
                string Mannerofpayment = Convert.ToString(model.ddlMannerOfPayment);
                string Purpose = Convert.ToString(model.txtPurpose);
                string Remarks = Convert.ToString(model.txtRemarks);
                string VendorName = Convert.ToString(model.txtvendorname);
                string ADDEDBY = Convert.ToString(_userId);
                string LSTMODIFYBY = Convert.ToString(_userId);
                string agreementfile = string.Empty;
                string approvalnotefile = string.Empty;
                string addedby = Convert.ToString(_userId);
                string strfilename = string.Empty;
                string fullpath = string.Empty;
                string file_name = string.Empty;
                string filename = string.Empty;
                string IOMID = WebUtility.UrlDecode(Encryption.Decrypt(model.hdIOM ?? string.Empty));
                string antibribery = string.Empty;
                string ndadocument = string.Empty;
                string strrdomst = model.rdomaster;
                string strmstagr = model.ddlmstagreement;
                string considerable = string.Empty;

                //Agreement File
                if (model.FileUploadAttachmentAAEnabled ?? false)
                {
                    agreementfile =  model.fileUploadAttachmentAA != null ? model.fileUploadAttachmentAA.FileName : string.Empty;
                }
                else
                {
                    agreementfile = model.hdnagreeFile ?? string.Empty;
                }

                //Reference File
                if (model.FileUploadAttachmentRAEnabled ?? false)
                {
                    approvalnotefile = model.fileUploadAttachmentRA != null ? model.fileUploadAttachmentRA.FileName : string.Empty;
                }
                else
                {
                    approvalnotefile = model.hdnrefFile ?? string.Empty;
                }



                ////NDA Document
                //string[] strndadocument;
                //strndadocument = hdnndadocument.Value.Split(new Char[] { '\\' });
                //ndadocument = strndadocument[strndadocument.Length - 1].ToString();
                //ndadocument = fileUploadndadocument.Text;
                considerable = model.txtconsiderable;


                string Cday = Convert.ToString(DateTime.Now.Day);
                string Cmonth = Convert.ToString(DateTime.Now.Month);
                string Cyear = Convert.ToString(DateTime.Now.Year);
                DateTime cdate = DateTime.Now;
                DateTime uselectdateED = DateTime.Parse(EffectiveDate);
                if (model.lbl_appauth == "" && model.ddl_appauth == "")
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Please select Next Approval Authority",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (Convert.ToInt32(model.ddlagreement) == 0)
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Agreement Type is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                   
                }

                else if (model.ddlMannerOfPayment == "")
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Manner Of Payment is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                }

                else if (model.ddlcontract == "")
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Contract Type is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (Convert.ToInt32(model.ddlTermmonth) == 0 && Convert.ToInt32(model.ddlTermyear) == 0)
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Term is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (string.IsNullOrEmpty(model.txtPurpose))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Purpose is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                   
                }
                else if (string.IsNullOrEmpty(model.txtRemarks))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Remarks is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (string.IsNullOrEmpty(model.txtvendorname))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Vendor Name is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (strrdomst == "1" && strmstagr == "0")
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Master Agreement is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                    
                }
                else
                {
                    if (agreementtype == "2")
                    {
                        if (string.IsNullOrWhiteSpace(agreementfile))
                        {
                            result = new IOMContractOperationResult
                            {
                                Status = false,
                                Message = "Agreement document is mandatory field",
                                obj = new { errorpanel_Visible = true }
                            };
                            return result;
                        }
                        else if (string.IsNullOrWhiteSpace(agreementfile) &&(model.FileUploadAttachmentAAEnabled ?? false))
                        {
                            result = new IOMContractOperationResult
                            {
                                Status = false,
                                Message = "Agreement document attachment mandatory",
                                obj = new { errorpanel_Visible = true }
                            };
                            return result;

                        }
                        //else if (string.IsNullOrEmpty(approvalnotefile.Trim()) && fileUploadAttachmentRA.Enabled == true)
                        //{
                        //    errorpanel.Visible = true;
                        //    status.Text = "Approval Note document attachment mandatory";
                        //    txtup.Focus();
                        //    return;
                        //}
                    }
                    else if (agreementtype == "1")
                    {
                        if (string.IsNullOrWhiteSpace(approvalnotefile) && (model.FileUploadAttachmentRAEnabled ?? false))
                        {
                            result = new IOMContractOperationResult
                            {
                                Status = false,
                                Message = "Approval Note document attachment mandatory",
                                obj = new { errorpanel_Visible = true }
                            };
                            return result;
                          
                        }
                    }

                    if (agreementfile == approvalnotefile && !string.IsNullOrEmpty(agreementfile) && !string.IsNullOrEmpty(approvalnotefile))
                    {
                        result = new IOMContractOperationResult
                        {
                            Status = false,
                            Message = "Agreement and approval note attachment should not be same",
                            obj = new { errorpanel_Visible = true }
                        };
                        return result;
                    }

                    ////IF not Verfication
                    //if (agreementtype != "5")
                    //{
                    //    if (strrdomst == "0")
                    //    {
                    //        //NDA Document
                    //        if (string.IsNullOrEmpty(ndadocument.Trim()) && fileUploadndadocument.Enabled == true)
                    //        {
                    //            errorpanel.Visible = true;
                    //            status.Text = "NDA document is mandatory field";
                    //            txtup.Focus();
                    //            return;
                    //        }
                    //    }
                    //}

                    //////////////////Agreement doc upload/////////////////////

                    if (model.fileUploadAttachmentAA != null)
                    {
                        agreementfile = agreementuploadFile(model.fileUploadAttachmentAA, model.txtvendorname);
                        string[] a = agreementfile.Split(new Char[] { '@' });
                        string b = a[0].ToString();
                        if (b == "1")
                        {
                            agreementfile = a[1].ToString();
                        }
                        else
                        {
                            result = new IOMContractOperationResult
                            {
                                Status = false,
                                Message = a[1],
                                obj = new { errorpanel_Visible = false, IsWindowAlert = true }
                            };
                            return result;

                        }
                    }
                    //////////////////Reference doc upload/////////////////////
                    if (model.fileUploadAttachmentRA != null)
                    {
                        approvalnotefile = refuploadFile(model.fileUploadAttachmentRA, model.txtvendorname);
                        string[] a = approvalnotefile.Split(new Char[] { '@' });
                        string b = a[0].ToString();
                        if (b == "1")
                        {
                            approvalnotefile = a[1].ToString();
                        }
                        else
                        {
                            result = new IOMContractOperationResult
                            {
                                Status = false,
                                Message = a[1],
                                obj = new { errorpanel_Visible = false, IsWindowAlert = true }
                            };
                            return result;
                        }
                    }




                    //////////////////NDA Document/////////////////////
                    //if (!string.IsNullOrEmpty(ndadocument.Trim()))
                    //{
                    //    fullpath = Path.Combine(filePath, ndadocument);
                    //    ndadocument = ndadocumentuploadFile(ndadocument, filePath);
                    //    string[] a = ndadocument.Split(new Char[] { '@' });
                    //    string b = a[0].ToString();
                    //    if (b == "1")
                    //    {
                    //        ndadocument = a[1].ToString();
                    //    }
                    //    else
                    //    {
                    //        Response.Write("<script>alert('" + a[1] + "')</script>");
                    //        return;
                    //    }
                    //}


                    //if (string.IsNullOrEmpty(approvalnotefile.Trim()))
                    //{
                    //    approvalnotefile = linkappnote.InnerText.Replace("[", "").Replace("]", "");
                    //}


                    //NDA Document
                    //if (string.IsNullOrEmpty(ndadocument.Trim()))
                    //{
                    //    ndadocument = linkbtnndadocument.InnerText.Replace("[", "").Replace("]", "");
                    //}



                    /////////////////////////////////////////////////////////////////////

                    // Start Added by Aumento :: SR79956
                    int IT_DeclareValue = model.CheckDeclaration == "1" ? 1 : 0;

                    string selectedValue = model.ddlfromText;
                    if (selectedValue == "Information Technology")
                    {
                        if (IT_DeclareValue != 1)
                        {
                            result = new IOMContractOperationResult
                            {
                                Status = false,
                                Message = "You must check the declaration checkbox to proceed.",
                                obj = new { errorpanel_Visible = true }
                            };
                            return result;
                        }
                    }
                    // End Added by Aumento :: SR79956
                    string strErrMsg = _iOMContractRepos.UPDATEIOMDETAIL(fromiom, agreementtype, Contracttype, EffectiveDate, Termmonth, Termyear, Dateofexpiry, Mannerofpayment, Purpose, Remarks, addedby, agreementfile, approvalnotefile, VendorName, IOMID, antibribery, considerable, authlevel, strmstagr, depthead, DIVHEAD, OPHEADECODE, IT_DeclareValue); // IT_DeclareValue  Added by Aumento :: SR79956
                    string[] outputmsg = strErrMsg.Split(new Char[] { '#' });
                    string errResult = Convert.ToString(outputmsg[0]);
                    string errMsg = Convert.ToString(outputmsg[1]);
                    if (errResult == "1")
                    {
                        SendMail(agreementtypedesc, Contracttypedesc, EffectiveDate, Dateofexpiry, VendorName, model.hdauthcode, model.hdauthname, model.hdauthemailid);

                        result = new IOMContractOperationResult
                        {
                            Status = true,
                            Message = string.Empty,
                            obj = new { redirectionTo = "/IOMContract/ManageContract" }
                        };
                    }
                    else
                    {
                        result = new IOMContractOperationResult
                        {
                            Status = false,
                            Message = RemoveSpecialChar(errMsg),
                            obj = new { IsWindowAlert = true }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                result = new IOMContractOperationResult
                {
                    Status = false,
                    Message = RemoveSpecialChar(ex.Message),
                    obj = new { IsWindowAlert = true }
                };
            }

            return result;
        }

        public IOMContractOperationResult IOMApprovalFormSave(IOMApprovalFormSave model)
        {
            var result = new IOMContractOperationResult
            {
                Status = false
            };

            try
            {
                string EMPDESIGNATION = WebUtility.UrlDecode(Encryption.Decrypt(Convert.ToString(model.EMPDESIGNATION)));
                string Empcode = _userId;
                if ((Convert.ToInt32(model.hdbackdate) < 0) && (EMPDESIGNATION == "4"))
                {
                    if (string.IsNullOrEmpty(model.txtbackdateremark))
                    {
                        result = new IOMContractOperationResult
                        {
                            Status = false,
                            Message = "Back Date Remarks is Mandatory Field",
                            obj = new { IsWindowAlert = true }
                        };
                        return result;
                    }
                }
                if ((Convert.ToInt32(model.hddelaydate) <= 0))
                {
                    if (model.chkTerms == false && model.rdoStatus == "1")
                    {
                        result = new IOMContractOperationResult
                        {
                            Status = false,
                            Message = "Acknowledgement is Mandatory, in case of delayed request",
                            obj = new { IsWindowAlert = true }
                        };
                        return result;
                    }
                }
                if (string.IsNullOrWhiteSpace(model.txt_Remarks))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Remarks is Mandatory Field",
                        obj = new { errorpanel_Visible = true }
                    };
                    return result;
                }
                if (string.IsNullOrWhiteSpace(model.lbl_appauth) && string.IsNullOrWhiteSpace(model.ddl_appauth))
                    {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Select Approving Authority",
                        obj = new { errorpanel_Visible = true }
                    };
                    return result;
                }
                else
                {
                    string strRemarks = model.txt_Remarks;
                    string status = model.rdoStatus;
                    string HDIOMID = model.hdiomid;
                    string backdateremark = model.txtbackdateremark;
                    string authlevel = model.hdauthlevel;
                    string OPHEADECODE = string.Empty;
                    if (model.hdauthlevel == "4" && model.ddl_appauth != "")
                    {
                        string AUTHVAL = model.ddl_appauth;
                        string[] AUTHARR = AUTHVAL.Split(new Char[] { '#' });
                        model.hdauthcode = Convert.ToString(AUTHARR[0]);
                        OPHEADECODE = Convert.ToString(AUTHARR[0]);
                        model.hdauthname = Convert.ToString(AUTHARR[1]);
                        model.hdauthemailid = Convert.ToString(AUTHARR[2]);
                    }
                    string strErrMsg = _iOMContractRepos.Update_IOMDetail(HDIOMID, Empcode, strRemarks, status, backdateremark, authlevel, OPHEADECODE);
                    
                    
                    string[] ResigStatus = strErrMsg.Split(new Char[] { '#' });
                    string errResult = Convert.ToString(ResigStatus[0]);
                    string errMsg = Convert.ToString(ResigStatus[1]);
                    if (errResult == "1")
                    {
                        SendMailIOMApprovalForm(model, status);
                        result = new IOMContractOperationResult
                        {
                            Status = true,
                            Message = string.Empty,
                            obj = new { redirectionTo = "/IOMContract/ManageIOMApproval" }
                        };
                    }
                    else
                    {
                        result = new IOMContractOperationResult
                        {
                            Status = false,
                            Message = RemoveSpecialChar(errMsg),
                            obj = new { IsWindowAlert = true }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                result = new IOMContractOperationResult
                {
                    Status = false,
                    Message = RemoveSpecialChar(ex.Message),
                    obj = new { IsWindowAlert = true }
                };
            }

            return result;
        }

        public IOMContractOperationResult SubmitFinalDocumentSave(SubmitFinalDocumentSave model)
        {
            var result = new IOMContractOperationResult
            {
                Status = false
            };

            try
            {
                string Empcode = _userId;
                if (string.IsNullOrWhiteSpace(model.txt_Remarks))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Remarks is Required Field",
                        obj = new { IsWindowAlert = true }
                    };
                    return result;
                }
                else
                {
                    //Final Document
                    string finaldoc = model.filefinaldoc != null ? model.filefinaldoc.FileName : string.Empty;
                    if (string.IsNullOrWhiteSpace(finaldoc))
                    {
                        result = new IOMContractOperationResult
                        {
                            Status = false,
                            Message = "Final Document is mandatory field",
                            obj = new { IsWindowAlert = true }
                        };
                        return result;
                    }

                    //////////////////Final doc upload/////////////////////
                    if (!string.IsNullOrWhiteSpace(finaldoc))
                    {
                        string fullpath = Path.Combine(filePath, finaldoc);
                        finaldoc = finaldocuploadfile(model.filefinaldoc, model.txtvendorname);
                        string[] a = finaldoc.Split(new Char[] { '@' });
                        string b = a[0].ToString();
                        if (b == "1")
                        {
                            finaldoc = a[1].ToString();
                        }
                        else
                        {
                            result = new IOMContractOperationResult
                            {
                                Status = false,
                                Message = a[1],
                                obj = new { IsWindowAlert = true }
                            };
                            return result;
                        }
                    }

                    string strRemarks = model.txt_Remarks;
                    string HDIOMID = model.hdiomid;
                    string strErrMsg = _iOMContractRepos.UPDATEFINALDOCUMENT(HDIOMID, Empcode, strRemarks, finaldoc);
                    string[] ResigStatus = strErrMsg.Split(new Char[] { '#' });
                    string errResult = Convert.ToString(ResigStatus[0]);
                    string errMsg = Convert.ToString(ResigStatus[1]);
                    if (errResult == "1")
                    {
                        result = new IOMContractOperationResult
                        {
                            Status = true,
                            Message = RemoveSpecialChar(errMsg),
                            obj = new { redirectionTo = "/IOMContract/ManageContract" }
                        };
                    }
                    else
                    {
                        result = new IOMContractOperationResult
                        {
                            Status = false,
                            Message = RemoveSpecialChar(errMsg),
                            obj = new { IsWindowAlert = true }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                result = new IOMContractOperationResult
                {
                    Status = false,
                    Message = RemoveSpecialChar(ex.Message),
                    obj = new { IsWindowAlert = true }
                };
            }

            return result;
        }

        public IOMContractOperationResult UserAcknowledgementSave(UserAcknowledgementSave model)
        {
            var result = new IOMContractOperationResult
            {
                Status = false
            };

            try
            {
                if (string.IsNullOrWhiteSpace(model.txt_Remarks))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Remarks is mandatory field",
                        obj = new { IsWindowAlert = true }
                    };
                }
                else
                {
                    string Empcode = _userId;
                    string strRemarks = model.txt_Remarks;
                    string HDIOMID = model.hdiomid;
                    string strErrMsg = _iOMContractRepos.UPDATEUSERACKNOWLEDGEMENT(HDIOMID, Empcode, strRemarks);
                    string[] ResigStatus = strErrMsg.Split(new Char[] { '#' });
                    string errResult = Convert.ToString(ResigStatus[0]);
                    string errMsg = Convert.ToString(ResigStatus[1]);
                    if (errResult == "1")
                    {

                        SendMailUserAcknowledgemen(model);

                        result = new IOMContractOperationResult
                        {
                            Status = true,
                            Message = RemoveSpecialChar(errMsg),
                            obj = new { redirectionTo = "/IOMContract/ManageContract" }
                        };
                    }
                    else
                    {
                        result = new IOMContractOperationResult
                        {
                            Status = false,
                            Message = RemoveSpecialChar(errMsg),
                            obj = new { IsWindowAlert = true }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                result = new IOMContractOperationResult
                {
                    Status = false,
                    Message = RemoveSpecialChar(ex.Message),
                    obj = new { IsWindowAlert = true }
                };
            }

            return result;
        }


        public IOMContractOperationResult UserCommunicationSave(UserCommunicationSave model)
        {
            var result = new IOMContractOperationResult
            {
                Status = false
            };

            try
            {
                string strRemarks = model.txtMessageRemarks;
                string HDIOMID = model.hdiomid;
                string Empcode = _userId;
                string userType = "user";
                string refDocument = string.Empty;

                //Agreement File
                refDocument = model.RefUploadDocument != null ? model.RefUploadDocument.FileName : string.Empty;
                if (string.IsNullOrWhiteSpace(strRemarks))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Remarks is mandatory field",
                        obj = new { IsWindowAlert = true }
                    };
                    return result;
                }
                else
                {
                    //////////////////Agreement doc upload/////////////////////
                    if (!string.IsNullOrWhiteSpace(refDocument.Trim()))
                    {
                        refDocument = uploadCommunicationFile(model.RefUploadDocument);
                        string[] a = refDocument.Split(new Char[] { '@' });
                        string b = a[0].ToString();
                        if (b == "1")
                        {
                            refDocument = a[1].ToString();
                        }
                        else
                        {
                            result = new IOMContractOperationResult
                            {
                                Status = false,
                                Message = a[1],
                                obj = new { IsWindowAlert = true }
                            };
                            return result;
                        }
                    }

                    string strErrMsg = _iOMContractRepos.SUBMITCOMMUNICATION(HDIOMID, Empcode, strRemarks, refDocument, userType, 2);
                    string[] outputstatus = strErrMsg.Split(new Char[] { '#' });
                    string errResult = Convert.ToString(outputstatus[0]);
                    string errMsg = Convert.ToString(outputstatus[1]);
                    if (errResult == "1")
                    {
                        SendCommunicationMail(model);
                        string IOMID = WebUtility.UrlEncode(Encryption.Encrypt(HDIOMID)).ToString();

                        result = new IOMContractOperationResult
                        {
                            Status = true,
                            Message = RemoveSpecialChar(errMsg),
                            obj = new { redirectionTo = "/IOMContract/UserCommunication?IOMID=" + IOMID }
                        };
                    }
                    else
                    {
                        result = new IOMContractOperationResult
                        {
                            Status = false,
                            Message = RemoveSpecialChar(errMsg),
                            obj = new { IsWindowAlert = true }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                result = new IOMContractOperationResult
                {
                    Status = false,
                    Message = RemoveSpecialChar(ex.Message),
                    obj = new { IsWindowAlert = true }
                };
            }

            return result;
        }
        public IOMContractOperationResult ContractRenewalSave(ContractRenewalSave model)
        {
            var result = new IOMContractOperationResult
            {
                Status = false
            };
            try
            {
                string? authlevel = model.hdauthlevel;
                //This block working on next approval authority is operating head
                string OPHEADECODE = string.Empty;
                if (model.hdauthlevel == "4" && !string.IsNullOrWhiteSpace(model.ddl_appauth))
                {
                    string AUTHVAL = model.ddl_appauth;
                    string[] AUTHARR = AUTHVAL.Split(new Char[] { '#' });
                    model.hdauthcode = Convert.ToString(AUTHARR[0]);
                    OPHEADECODE = Convert.ToString(AUTHARR[0]);
                    model.hdauthname = Convert.ToString(AUTHARR[1]);
                    model.hdauthemailid = Convert.ToString(AUTHARR[2]);
                }
                string agreementid = model.HDAGREEMENTID;
                string fromiom = Convert.ToString(model.ddlfrom ?? string.Empty);
                string agreementtype = Convert.ToString(model.ddlagreement ?? string.Empty);
                string Contracttype = Convert.ToString(model.ddlcontract ?? string.Empty);
                string Contracttypedesc = model.ddlcontractText ?? string.Empty;
                string EffectiveDate = Convert.ToString(model.optED + "-" + model.optEM + "-" + model.optEY);
                string Termmonth = Convert.ToString(model.ddlTermmonth ?? string.Empty);
                string Termyear = Convert.ToString(model.ddlTermyear ?? string.Empty);
                string Dateofexpiry = string.Empty;
                if (string.IsNullOrWhiteSpace(model.hdnexpiryday))
                {
                    Dateofexpiry = Convert.ToString(model.optDay + "-" + model.optMonth + "-" + model.optYear);
                }
                else
                {
                    Dateofexpiry = Convert.ToString(model.hdnexpiryday + "-" + model.hdnexpirymon + "-" + model.hdnexpiryyear);
                }
                string Mannerofpayment = Convert.ToString(model.ddlMannerOfPayment ?? string.Empty);
                string Purpose = Convert.ToString(model.txtPurpose ?? string.Empty);
                string Remarks = Convert.ToString(model.txtRemarks ?? string.Empty);
                string VendorName = Convert.ToString(model.txtvendorname ?? string.Empty);
                string ADDEDBY = _userId;
                string LSTMODIFYBY = _userId;
                string finaldoc = string.Empty;
                string addedby = _userId;
                string strfilename = string.Empty;
                string fullpath = string.Empty;
                string file_name = string.Empty;
                string filename = string.Empty;
                string considerable = string.Empty;
                //string[] strantribriberydoc;
                //string[] strndadocument;
                string antibriberyfile = string.Empty;
                string ndadocumentfile = string.Empty;
                string strrdomst = model.rdomaster;
                string strmstagr = model.ddlmstagreement;


                //Final Document
                finaldoc = model.fileuploadfinaldoc != null ? model.fileuploadfinaldoc.FileName : string.Empty;

                ////Antibribery Document
                //if (antibriberylink.InnerText.Trim().ToString() != "")
                //{
                //    antibriberyfile = antibriberylink.InnerText;
                //}
                //else
                //{
                //    strantribriberydoc = HDANTIBRIBERY.Value.Split(new Char[] { '\\' });
                //    antibriberyfile = strantribriberydoc[strantribriberydoc.Length - 1].ToString();
                //}

                ////NDA Document
                //if (ndadocumentlink.InnerText.Trim().ToString() != "")
                //{
                //    ndadocumentfile = ndadocumentlink.InnerText;
                //}
                //else
                //{
                //    strndadocument = HDNDADOCUMENT.Value.Split(new Char[] { '\\' });
                //    ndadocumentfile = strndadocument[strndadocument.Length - 1].ToString();
                //}

                string Cday = Convert.ToString(System.DateTime.Now.Day);
                string Cmonth = Convert.ToString(System.DateTime.Now.Month);
                string Cyear = Convert.ToString(System.DateTime.Now.Year);
                DateTime cdate = Convert.ToDateTime(Cmonth + "/" + Cday + "/" + Cyear);
                DateTime uselectdateED = DateTime.Parse(EffectiveDate);

                if (string.IsNullOrWhiteSpace(model.lbl_appauth) && string.IsNullOrWhiteSpace(model.ddl_appauth))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Please select Next Approval Authority",
                        obj = new { errorpanel_Visible = true }
                    };
                }

                else if (Convert.ToInt32(model.ddlagreement) == 0)
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Agreement Type is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (string.IsNullOrWhiteSpace(model.ddlMannerOfPayment))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Manner Of Payment is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (string.IsNullOrWhiteSpace(model.ddlcontract))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Contract Type is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                   
                }
                else if (Convert.ToInt32(model.ddlTermmonth) == 0 && Convert.ToInt32(model.ddlTermyear) == 0)
                {

                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Term is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                 
                }
                else if (string.IsNullOrWhiteSpace(model.txtPurpose))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Purpose is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (Convert.ToInt32(Purpose.Length) >= 501)
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "The text entered exceeds the maximum length in purpose",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (string.IsNullOrWhiteSpace(model.txtRemarks))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Remarks is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (Convert.ToInt32(Remarks.Length) >= 501)
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "The text entered exceeds the maximum length in remarks",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (string.IsNullOrEmpty(model.txtvendorname))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Vendor Name is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };

                }
                else if (strrdomst == "1" && strmstagr == "0")
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Master Agreement is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else
                {
                    if (strrdomst == "0")
                    {
                        //if (string.IsNullOrEmpty(antibriberyfile))
                        //{
                        //    errorpanel.Visible = true;
                        //    status.Text = "Antibribery document is mandatory field";
                        //    txtup.Focus();
                        //    return;
                        //}
                        //if (string.IsNullOrEmpty(ndadocumentfile))
                        //{
                        //    errorpanel.Visible = true;
                        //    status.Text = "NDA document is mandatory field";
                        //    txtup.Focus();
                        //    return;
                        //}
                    }
                    if (string.IsNullOrEmpty(finaldoc))
                    {
                        result = new IOMContractOperationResult
                        {
                            Status = false,
                            Message = "Final document is mandatory field",
                            obj = new { errorpanel_Visible = true }

                        };
                        return result;
                    }
                    //if (strrdomst == "0")
                    //{
                    //    //////////////////Antibribery document upload/////////////////////
                    //    //if (antibriberylink.InnerText.Trim().ToString() == "")
                    //    //{
                    //    //    fullpath = Path.Combine(filePath, antibriberyfile);
                    //    //    antibriberyfile = Antibriberyfileupload(antibriberyfile, filePath);
                    //    //    string[] a = antibriberyfile.Split(new Char[] { '@' });
                    //    //    string b = a[0].ToString();
                    //    //    if (b == "1")
                    //    //    {
                    //    //        antibriberyfile = a[1].ToString();
                    //    //    }
                    //    //    else
                    //    //    {
                    //    //        Response.Write("<script>alert('" + a[1] + "')</script>");
                    //    //        return;
                    //    //    }
                    //    //}
                    //    //////////////////NDA document upload/////////////////////
                    //    //if (ndadocumentlink.InnerText.Trim().ToString() == "")
                    //    //{
                    //    //    fullpath = Path.Combine(filePath, ndadocumentfile);
                    //    //    ndadocumentfile = Ndadocfileupload(ndadocumentfile, filePath);
                    //    //    string[] a = ndadocumentfile.Split(new Char[] { '@' });
                    //    //    string b = a[0].ToString();
                    //    //    if (b == "1")
                    //    //    {
                    //    //        ndadocumentfile = a[1].ToString();
                    //    //    }
                    //    //    else
                    //    //    {
                    //    //        Response.Write("<script>alert('" + a[1] + "')</script>");
                    //    //        return;
                    //    //    }
                    //    //}
                    //}
                    //////////////////Final document upload/////////////////////
                    if (!string.IsNullOrEmpty(finaldoc))
                    {
                        finaldoc = agreementuploadFile(model.fileuploadfinaldoc, model.txtvendorname);
                        string[] a = finaldoc.Split(new Char[] { '@' });
                        string b = a[0].ToString();
                        if (b == "1")
                        {
                            finaldoc = a[1].ToString();
                        }
                        else
                        {
                            result = new IOMContractOperationResult
                            {
                                Status = false,
                                Message = a[1],
                                obj = new { IsWindowAlert = true }
                            };
                            return result;
                        }
                    }
                    string approvalnotefile = model.appnotelinkText ?? string.Empty;
                    considerable = model.txtconsiderable ?? string.Empty;
                    /////////////////////////////////////////////////////////////////////
                    string strErrMsg = _iOMContractRepos.Insert_IOMDetail(fromiom, agreementtype, Contracttype, EffectiveDate, Termmonth, Termyear, Dateofexpiry, Mannerofpayment, Purpose, Remarks, addedby, finaldoc, approvalnotefile, VendorName, agreementid, antibriberyfile, considerable, authlevel, strmstagr, depthead, DIVHEAD, OPHEADECODE, "", "", "", "", "", 0); // 0  Added by Aumento :: SR79956
                    string[] outputmsg = strErrMsg.Split(new Char[] { '#' });
                    string errResult = Convert.ToString(outputmsg[0]);
                    string errMsg = Convert.ToString(outputmsg[1]);
                    if (errResult == "1")
                    {
                        SendMailContractRenewal(model.hdauthcode, model.hdauthname, model.hdauthemailid,  Contracttypedesc, EffectiveDate, Dateofexpiry, VendorName);
                        result = new IOMContractOperationResult
                        {
                            Status = true,
                            Message = "Contract Detail Successfully Submitted",
                            obj = new { IsWindowAlert = true, redirectionTo = "/IOMContract/ManageContract" }
                        };
                    }
                    else if (errResult == "2")
                    {
                        result = new IOMContractOperationResult
                        {
                            Status = false,
                            Message = "Sorry, this contract already processed for renewal or amendment!!!!!!",
                            obj = new { IsWindowAlert = true }
                        };
                    }
                    else
                    {
                        result = new IOMContractOperationResult
                        {
                            Status = false,
                            Message = RemoveSpecialChar(errMsg),
                            obj = new { IsWindowAlert = true }
                        };
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                result = new IOMContractOperationResult
                {
                    Status = false,
                    Message = RemoveSpecialChar(ex.Message),
                    obj = new { IsWindowAlert = true }
                };
            }

            return result;
        }
        public IOMContractOperationResult CloseContractSave(CloseContractSave model)
        {
            var result = new IOMContractOperationResult
            {
                Status = false
            };

            try
            {
                string agreementid = model.HDAGREEMENTID;
                string? closeremarks = model.txt_closeRemarks;
                string ecode = _userId;
                if (string.IsNullOrWhiteSpace(closeremarks))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Close remarks is mandatory field",
                        obj = new { IsWindowAlert = true }
                    };
                    return result;
                }

                string strErrMsg = _iOMContractRepos.closecontract(agreementid, closeremarks, ecode);
                string[] outputmsg = strErrMsg.Split(new Char[] { '#' });
                string errResult = Convert.ToString(outputmsg[0]);
                string errMsg = Convert.ToString(outputmsg[1]);
                if (errResult == "1")
                {
                    result = new IOMContractOperationResult
                    {
                        Status = true,
                        Message = "Contract successfully closed",
                        obj = new { IsWindowAlert = true, redirectionTo = "/IOMContract/ContractList" }
                    };
                }
                else
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = RemoveSpecialChar(errMsg),
                        obj = new { IsWindowAlert = true }

                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                result = new IOMContractOperationResult
                {
                    Status = false,
                    Message = RemoveSpecialChar(ex.Message),
                    obj = new { IsWindowAlert = true }
                };
            }
            return result;
        }
       
        public IOMContractOperationResult ContractAmendmentSave(ContractAmendmentSave model)
        {
            var result = new IOMContractOperationResult
            {
                Status = false
            };

            try
            {
                string? authlevel = model.hdauthlevel;
                //This block working on next approval authority is operating head
                string OPHEADECODE = string.Empty;
                if (model.hdauthlevel == "4" && !string.IsNullOrWhiteSpace(model.ddl_appauth))
                {
                    string AUTHVAL = model.ddl_appauth;
                    string[] AUTHARR = AUTHVAL.Split(new Char[] { '#' });
                    model.hdauthcode = Convert.ToString(AUTHARR[0]);
                    OPHEADECODE = Convert.ToString(AUTHARR[0]);
                    model.hdauthname = Convert.ToString(AUTHARR[1]);
                    model.hdauthemailid = Convert.ToString(AUTHARR[2]);
                }
                string agreementid = model.HDAGREEMENTID;
                string fromiom = Convert.ToString(model.ddlfrom ?? string.Empty);
                string agreementtype = Convert.ToString(model.ddlagreement ?? string.Empty);
                string Contracttype = Convert.ToString(model.ddlcontract ?? string.Empty);
                string Contracttypedesc = model.ddlcontractText ?? string.Empty;
                string EffectiveDate = Convert.ToString(model.optED + "-" + model.optEM + "-" + model.optEY);
                string Termmonth = Convert.ToString(model.ddlTermmonth ?? string.Empty);
                string Termyear = Convert.ToString(model.ddlTermyear ?? string.Empty);

                string Dateofexpiry = string.Empty;
                if (string.IsNullOrWhiteSpace(model.hdnexpiryday))
                {
                    Dateofexpiry = Convert.ToString(model.optDay + "-" + model.optMonth + "-" + model.optYear);
                }
                else
                {
                    Dateofexpiry = Convert.ToString(model.hdnexpiryday + "-" + model.hdnexpirymon + "-" + model.hdnexpiryyear);
                }

                string Mannerofpayment = Convert.ToString(model.ddlMannerOfPayment ?? string.Empty);
                string Purpose = Convert.ToString(model.txtPurpose ?? string.Empty);
                string Remarks = Convert.ToString(model.txtRemarks ?? string.Empty);
                string VendorName = Convert.ToString(model.txtvendorname ?? string.Empty);
                string ADDEDBY = _userId;
                string LSTMODIFYBY = _userId;

                string finaldoc = string.Empty;
                string addedby = _userId;
                string strfilename = string.Empty;
                string fullpath = string.Empty;
                string file_name = string.Empty;
                string considerable = string.Empty;

                //string[] strantribriberydoc;
                //string[] strndadocument;
                string antibriberyfile = string.Empty;
                string ndadocumentfile = string.Empty;
                string strrdomst = model.rdomaster;
                string strmstagr = model.ddlmstagreement;

                //Final Document
                finaldoc = model.fileuploadfinaldoc != null ? model.fileuploadfinaldoc.FileName : string.Empty;

                //Antibribery Document
                //if (antibriberylink.InnerText.ToString().Trim() != "")
                //{
                //    antibriberyfile = antibriberylink.InnerText;
                //}
                //else
                //{
                //    strantribriberydoc = HDANTIBRIBERY.Value.Split(new Char[] { '\\' });
                //    antibriberyfile = strantribriberydoc[strantribriberydoc.Length - 1].ToString();
                //}

                //NDA Document
                //if (ndadocumentlink.InnerText.ToString().Trim() != "")
                //{
                //    ndadocumentfile = ndadocumentlink.InnerText;
                //}
                //else
                //{
                //    strndadocument = HDNDADOCUMENT.Value.Split(new Char[] { '\\' });
                //    ndadocumentfile = strndadocument[strndadocument.Length - 1].ToString();
                //}

                string Cday = Convert.ToString(System.DateTime.Now.Day);
                string Cmonth = Convert.ToString(System.DateTime.Now.Month);
                string Cyear = Convert.ToString(System.DateTime.Now.Year);

                DateTime cdate = Convert.ToDateTime(Cmonth + "/" + Cday + "/" + Cyear);
                DateTime uselectdateED = DateTime.Parse(EffectiveDate);

                if (string.IsNullOrWhiteSpace(model.lbl_appauth) && string.IsNullOrWhiteSpace(model.ddl_appauth))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Please select Next Approval Authority",
                        obj = new { errorpanel_Visible = true }
                    };
                }

                else if (Convert.ToInt32(model.ddlagreement) == 0)
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Agreement Type is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                }

                else if (string.IsNullOrWhiteSpace(model.ddlMannerOfPayment))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Manner Of Payment is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                }

                else if (string.IsNullOrWhiteSpace(model.ddlcontract))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Contract Type is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };

                }
                else if (Convert.ToInt32(model.ddlTermmonth) == 0 && Convert.ToInt32(model.ddlTermyear) == 0)
                {

                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Term is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };

                }
                else if (string.IsNullOrWhiteSpace(model.txtPurpose))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Purpose is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (string.IsNullOrWhiteSpace(model.txtRemarks))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Remarks is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (string.IsNullOrEmpty(model.txtvendorname))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Vendor Name is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };

                }
                else if (strrdomst == "1" && strmstagr == "0")
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Master Agreement is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else
                {
                    //if (strrdomst == "0")
                    //{
                    //    if (string.IsNullOrEmpty(antibriberyfile.Trim()))
                    //    {
                    //        errorpanel.Visible = true;
                    //        status.Text = "Antibribery document is mandatory field";
                    //        txtup.Focus();
                    //        return;
                    //    }
                    //    if (string.IsNullOrEmpty(ndadocumentfile.Trim()))
                    //    {
                    //        errorpanel.Visible = true;
                    //        status.Text = "NDA document is mandatory field";
                    //        txtup.Focus();
                    //        return;
                    //    }
                    //}
                    if (model.fileuploadfinaldoc == null)
                    {
                        result = new IOMContractOperationResult
                        {
                            Status = false,
                            Message = "Document attachment is mandatory",
                            obj = new { errorpanel_Visible = true }

                        };
                        return result;
                    }
                    /////////////////Antibribery document upload/////////////////////
                    //if (antibriberylink.InnerText.ToString().Trim() == "")
                    //{
                    //    if (antibriberyfile.Trim() != "")
                    //    {
                    //        fullpath = Path.Combine(filePath, antibriberyfile);
                    //        antibriberyfile = Antibriberyfileupload(antibriberyfile, filePath);
                    //        string[] a = antibriberyfile.Split(new Char[] { '@' });
                    //        string b = a[0].ToString();
                    //        if (b == "1")
                    //        {
                    //            antibriberyfile = a[1].ToString();
                    //        }
                    //        else
                    //        {
                    //            Response.Write("<script>alert('" + a[1] + "')</script>");
                    //            return;
                    //        }
                    //    }
                    //}
                    //////////////////NDA document upload/////////////////////
                    //if (ndadocumentlink.InnerText.ToString().Trim() == "")
                    //{
                    //    if (ndadocumentfile.Trim() != "")
                    //    {
                    //        fullpath = Path.Combine(filePath, ndadocumentfile);
                    //        ndadocumentfile = Ndadocfileupload(ndadocumentfile, filePath);
                    //        string[] a = ndadocumentfile.Split(new Char[] { '@' });
                    //        string b = a[0].ToString();
                    //        if (b == "1")
                    //        {
                    //            ndadocumentfile = a[1].ToString();
                    //        }
                    //        else
                    //        {
                    //            Response.Write("<script>alert('" + a[1] + "')</script>");
                    //            return;
                    //        }
                    //    }
                    //}

                    //////////////////Final Document Upload/////////////////////
                    if (!string.IsNullOrWhiteSpace(finaldoc))
                    {
                        string strMessage = refuploadFileForAmendment(model.fileuploadfinaldoc, model.txtvendorname);
                        string[] filename = strMessage.Split('#');
                        if (filename[0].ToString() != "0")
                        {
                            finaldoc = filename[0].ToString();
                        }
                        else
                        {
                            result = new IOMContractOperationResult
                            {
                                Status = false,
                                Message = filename[1],
                                obj = new { IsWindowAlert = true }
                            };
                            return result;
                        }
                    }
                    considerable = model.txtconsiderable ?? string.Empty;
                    string approvalnotefile = model.appnotelinkText ?? string.Empty;
                    /////////////////////////////////////////////////////////////////////
                    string strErrMsg = _iOMContractRepos.Insert_IOMDetail(fromiom, agreementtype, Contracttype, EffectiveDate, Termmonth, Termyear, Dateofexpiry, Mannerofpayment, Purpose, Remarks, addedby, finaldoc, approvalnotefile, VendorName, agreementid, antibriberyfile, considerable, authlevel, strmstagr, depthead, DIVHEAD, OPHEADECODE, "", "", "", "", "", 0); // 0  Added by Aumento :: SR79956
                    string[] outputmsg = strErrMsg.Split(new Char[] { '#' });
                    string errResult = Convert.ToString(outputmsg[0]);
                    string errMsg = Convert.ToString(outputmsg[1]);
                    if (errResult == "1")
                    {
                        SendMailContractAmendment(model.hdauthcode, model.hdauthname, model.hdauthemailid, Contracttypedesc, EffectiveDate, Dateofexpiry, VendorName);
                        result = new IOMContractOperationResult
                        {
                            Status = true,
                            Message = "Contract Detail Successfully Submitted",
                            obj = new { IsWindowAlert = true, redirectionTo = "/IOMContract/ManageContract" }
                        };
                    }
                    else if (errResult == "2")
                    {
                        result = new IOMContractOperationResult
                        {
                            Status = false,
                            Message = "Sorry, this contract already processed for renewal or amendment!!!!!!",
                            obj = new { IsWindowAlert = true }
                        };
                    }
                    else
                    {
                        result = new IOMContractOperationResult
                        {
                            Status = false,
                            Message = RemoveSpecialChar(errMsg),
                            obj = new { IsWindowAlert = true }
                        };
                    }
                }

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                result = new IOMContractOperationResult
                {
                    Status = false,
                    Message = RemoveSpecialChar(ex.Message),
                    obj = new { IsWindowAlert = true }
                };
            }

            return result;
        }
        public IOMContractOperationResult VerificationRequestFormSave(VerificationRequestFormSave model)
        {
            var result = new IOMContractOperationResult
            {
                Status = false
            };
            try
            {
                string? authlevel = model.hdauthlevel;
                //This block working on next approval authority is operating head
                string OPHEADECODE = string.Empty;
                if (model.hdauthlevel == "4" && !string.IsNullOrWhiteSpace(model.ddl_appauth))
                {
                    string AUTHVAL = model.ddl_appauth;
                    string[] AUTHARR = AUTHVAL.Split(new Char[] { '#' });
                    model.hdauthcode = Convert.ToString(AUTHARR[0]);
                    OPHEADECODE = Convert.ToString(AUTHARR[0]);
                    model.hdauthname = Convert.ToString(AUTHARR[1]);
                    model.hdauthemailid = Convert.ToString(AUTHARR[2]);
                }

                string fromiom = Convert.ToString(model.ddlfrom ?? string.Empty);
                string agreementtype = Convert.ToString(model.ddlagreement ?? string.Empty);
                string agreementtypeDesc = Convert.ToString(model.ddlagreementText ?? string.Empty);
                string Contracttype = Convert.ToString(model.ddlcontract ?? string.Empty);
                string Contracttypedesc = model.ddlcontractText ?? string.Empty;
                string EffectiveDate = Convert.ToString(model.optED + "-" + model.optEM + "-" + model.optEY);
                string Termmonth = Convert.ToString(model.ddlTermmonth ?? string.Empty);
                string Termyear = Convert.ToString(model.ddlTermyear ?? string.Empty);
                string Dateofexpiry = Convert.ToString(model.hdnexpiryday + "-" + model.hdnexpirymon + "-" + model.hdnexpiryyear);
                string Mannerofpayment = Convert.ToString(model.ddlMannerOfPayment ?? string.Empty);
                string Purpose = Convert.ToString(model.txtPurpose ?? string.Empty);
                string Remarks = Convert.ToString(model.txtRemarks ?? string.Empty);
                string VendorName = Convert.ToString(model.txtvendorname ?? string.Empty);
                string ADDEDBY = _userId;
                string LSTMODIFYBY = _userId;

                string agreementfile = string.Empty;
                string approvalnotefile = string.Empty;
                string addedby = _userId;
                string strfilename = string.Empty;
                string fullpath = string.Empty;
                string file_name = string.Empty;
                string filename = string.Empty;
                string agreementid = string.Empty;

                //Agreement File
                agreementfile = model.fileUploadAttachmentAA != null ? model.fileUploadAttachmentAA.FileName : string.Empty;

                //Reference File
                approvalnotefile = model.fileUploadAttachmentRA != null ? model.fileUploadAttachmentRA.FileName : string.Empty;

                string Cday = Convert.ToString(DateTime.Now.Day);
                string Cmonth = Convert.ToString(DateTime.Now.Month);
                string Cyear = Convert.ToString(DateTime.Now.Year);
                DateTime cdate = Convert.ToDateTime(Cmonth + "/" + Cday + "/" + Cyear);
                string strDate = Convert.ToString(model.optDay + "-" + model.optMonth + "-" + model.optYear);
                DateTime uselectdate = Convert.ToDateTime(strDate);
                DateTime uselectdateED = DateTime.Parse(EffectiveDate);


                if (string.IsNullOrWhiteSpace(model.lbl_appauth) && string.IsNullOrWhiteSpace(model.ddl_appauth))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Please select Next Approval Authority",
                        obj = new { errorpanel_Visible = true }
                    };
                }

                else if (Convert.ToInt32(model.ddlagreement) == 0)
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Agreement Type is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (string.IsNullOrWhiteSpace(model.ddlcontract))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Contract Type is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };

                }
                else if (uselectdate < cdate)
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Date of expiry should not be less than Effective date",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (Convert.ToInt32(model.ddlTermmonth) == 0 && Convert.ToInt32(model.ddlTermyear) == 0)
                {

                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Term is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };

                }
                else if (string.IsNullOrEmpty(model.txtvendorname))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Vendor Name is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };

                }
                else if (string.IsNullOrWhiteSpace(model.ddlMannerOfPayment))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Manner Of Payment is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (string.IsNullOrWhiteSpace(model.txtPurpose))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Purpose is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (Convert.ToInt32(model.txtPurpose.Length) >= 501)
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "The text entered exceeds the maximum length in purpose",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (string.IsNullOrWhiteSpace(model.txtRemarks))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Remarks is mandatory field",
                        obj = new { errorpanel_Visible = true }
                    };
                }
                else if (string.IsNullOrWhiteSpace(agreementfile.Trim()))
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Agreement document attachment mandatory",
                        obj = new { errorpanel_Visible = true }
                    };
                    return result;
                }
                else if (agreementfile == approvalnotefile)
                {
                    result = new IOMContractOperationResult
                    {
                        Status = false,
                        Message = "Agreement and approval note attachment should not be same",
                        obj = new { errorpanel_Visible = true }
                    };
                    return result;
                }
                else
                {
                    //////////////////Agreement doc upload/////////////////////
                    if (!string.IsNullOrWhiteSpace(agreementfile))
                    {
                        agreementfile = agreementuploadFile(model.fileUploadAttachmentAA, model.txtvendorname);
                        string[] a = agreementfile.Split(new Char[] { '@' });
                        string b = a[0].ToString();
                        if (b == "1")
                        {
                            agreementfile = a[1].ToString();
                        }
                        else
                        {
                            result = new IOMContractOperationResult
                            {
                                Status = false,
                                Message = a[1],
                                obj = new { IsWindowAlert = true }
                            };
                            return result;
                        }
                    }
                    //////////////////Reference doc upload/////////////////////
                    if (!string.IsNullOrWhiteSpace(approvalnotefile))
                    {
                        approvalnotefile = refuploadFile(model.fileUploadAttachmentRA, model.txtvendorname);
                        string[] a = approvalnotefile.Split(new Char[] { '@' });
                        string b = a[0].ToString();
                        if (b == "1")
                        {
                            approvalnotefile = a[1].ToString();
                        }
                        else
                        {
                            result = new IOMContractOperationResult
                            {
                                Status = false,
                                Message = a[1],
                                obj = new { IsWindowAlert = true }
                            };
                            return result;
                        }
                    }

                    /////////////////////////////////////////////////////////////////////
                    string strErrMsg = _iOMContractRepos.Insert_IOMDetail(fromiom, agreementtype, Contracttype, EffectiveDate, Termmonth, Termyear, Dateofexpiry, Mannerofpayment, Purpose, Remarks, addedby, agreementfile, approvalnotefile, VendorName, agreementid, "", "", authlevel, "", depthead, DIVHEAD, OPHEADECODE, "", "", "", "", "", 0); // 0  Added by Aumento :: SR79956
                    string[] outputmsg = strErrMsg.Split(new Char[] { '#' });
                    string errResult = Convert.ToString(outputmsg[0]);
                    string errMsg = Convert.ToString(outputmsg[1]);
                    if (errResult == "1")
                    {
                        SendMailVerificationRequest(model.hdauthcode, model.hdauthname, model.hdauthemailid, agreementtypeDesc, Contracttypedesc, EffectiveDate, Dateofexpiry, VendorName);
                        result = new IOMContractOperationResult
                        {
                            Status = true,
                            Message = "",
                            obj = new { redirectionTo = "/IOMContract/ManageContract" }
                        };
                    }
                    else
                    {
                        result = new IOMContractOperationResult
                        {
                            Status = false,
                            Message = RemoveSpecialChar(errMsg),
                            obj = new { IsWindowAlert = true }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                result = new IOMContractOperationResult
                {
                    Status = false,
                    Message = RemoveSpecialChar(ex.Message),
                    obj = new { IsWindowAlert = true }
                };
            }
            return result;
        }

        #region PRIVATE METHOD
        //Agreement Upload
        private string agreementuploadFile(IFormFile? fileUploadAttachmentAA, string txtvendorname)
        {
            string vendorname = RemoveSpecialChar(txtvendorname.Trim().ToString().Replace(" ", ""));
            if (fileUploadAttachmentAA == null || string.IsNullOrWhiteSpace(fileUploadAttachmentAA.FileName))
            {
                return "0@Invalid agreement file name supplied";
            }
            if (fileUploadAttachmentAA == null || fileUploadAttachmentAA.Length == 0)
            {
                return "0@Invalid agreement file content";
            }

            // strFileName = Path.GetFileName(strFileName);

            string fullpath = Path.Combine(filePath, fileUploadAttachmentAA.FileName);

            string fileext = (System.IO.Path.GetExtension(fullpath)).ToString().ToLower().Trim();

            string ModifiedName = vendorname + "Agreement" + System.DateTime.Now.Day + System.DateTime.Now.Month + System.DateTime.Now.Year + System.DateTime.Now.Hour + System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Millisecond + fileext;
            string fullMpath = Path.Combine(filePath, ModifiedName);


            if (string.IsNullOrWhiteSpace(filePath))
            { return "0@Path not found"; }

            if (fileext.ToLower() != ".pdf" && fileext.ToLower() != ".doc")
            {
                return "0@Cannot upload agreement file because the file format or extension is invalid please select a valid file (doc or pdf)";
            }

            try
            {
                if (fileUploadAttachmentAA.Length <= 7344128) //7MB maximum limit
                {
                    // Save file
                    using (var stream = new FileStream(fullMpath, FileMode.Create))
                    {
                        fileUploadAttachmentAA.CopyTo(stream);
                    }
                    return "1@" + ModifiedName;
                }
                else
                {
                    return "0@Unable to upload,agreement file exceeds maximum limit";
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return "0@" + ex.Message + " Permission to upload agreement file denied";
            }
        }
        //Reference Upload
        private string refuploadFile(IFormFile? fileUploadAttachmentRA, string txtvendorname)
        {
            string vendorname = RemoveSpecialChar(txtvendorname.Trim().ToString().Replace(" ", ""));

            if (fileUploadAttachmentRA == null || string.IsNullOrWhiteSpace(fileUploadAttachmentRA.FileName))
            {
                return "0@Invalid reference file name supplied";
            }
            if (fileUploadAttachmentRA == null || fileUploadAttachmentRA.Length == 0)
            {
                return "0@Invalid reference file content";
            }

            string fullpath = Path.Combine(filePath, fileUploadAttachmentRA.FileName);
            string fileext = System.IO.Path.GetExtension(fullpath);

            string ModifiedName = vendorname + "ApprovalNote" + System.DateTime.Now.Day + System.DateTime.Now.Month + System.DateTime.Now.Year + System.DateTime.Now.Hour + System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Millisecond + fileext;
            string fullMpath = Path.Combine(filePath, ModifiedName);

            if (string.IsNullOrWhiteSpace(filePath))
            { return "0@Path not found"; }

            if (fileext.ToLower() != ".pdf" && fileext.ToLower() != ".doc")
            {
                return "0@Cannot upload agreement file because the file format or extension is invalid please select a valid file (doc or pdf)";
            }

            try
            {
                if (fileUploadAttachmentRA.Length <= 7344128)//7MB maximum limit
                {
                    // Save file
                    using (var stream = new FileStream(fullMpath, FileMode.Create))
                    {
                        fileUploadAttachmentRA.CopyTo(stream);
                    }
                    return "1@" + ModifiedName;
                }
                else
                {
                    return "0@Unable to upload,file exceeds maximum limit";
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return "0@" + ex.Message + " Permission to upload file denied";
            }
        }

        private string refuploadFileForAmendment(IFormFile? file, string txtvendorname)
        {
            string vendorname = RemoveSpecialChar(txtvendorname.Trim().ToString().Replace(" ", ""));
            string filename = file != null ? file.FileName : string.Empty;
            string fileext = Path.GetExtension(filename);

            if (string.IsNullOrWhiteSpace(filename))
            {
                return "0#Invalid file name supplied";
            }
            if (file == null || file.Length == 0)
            {
                return "0#Invalid file content";
            }
            if (file.Length >= 7344128) //7 MB Maximum
            {
                return "0#Unable to upload,file exceeds maximum limit";
            }
            if (fileext.ToLower() != ".pdf" && fileext.ToLower() != ".doc")
            {
                return "0#Cannot upload agreement file because the file format or extension is invalid please select a valid file (pdf)";
            }
            else
            {
                string finalfilename = vendorname + "DOCUMENT" + DateTime.Now.ToString("ddMMyyyyhhmmssfftt") + fileext;
                try
                {
                    string f = Path.Combine(filePath, finalfilename);
                    using (var stream = new FileStream(f, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    return finalfilename + "#Successfully Uploaded";
                }
                catch (UnauthorizedAccessException ex)
                {
                    return "0#Permission to upload attachment file denied";
                }
            }
        }

        private string finaldocuploadfile(IFormFile? filefinaldoc, string txtvendorname)
        {
            string vendorname = RemoveSpecialChar(txtvendorname.Replace(" ", ""));
            if (filefinaldoc == null || string.IsNullOrWhiteSpace(filefinaldoc.FileName))
            {
                return "0@Invalid agreement file name supplied";
            }

            if (filefinaldoc == null || filefinaldoc.Length == 0)
            {
                return "0@Invalid agreement file content";
            }

            string fullpath = Path.Combine(filePath, filefinaldoc.FileName);

            string fileext = (System.IO.Path.GetExtension(fullpath)).ToString().ToLower().Trim();

            string ModifiedName = vendorname + "Agreement" + System.DateTime.Now.Day + System.DateTime.Now.Month + System.DateTime.Now.Year + System.DateTime.Now.Hour + System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Millisecond + fileext;
            string fullMpath = Path.Combine(filePath, ModifiedName);

            if (fileext.ToLower() != ".pdf" && fileext.ToLower() != ".doc")
            {
                return "0@Cannot upload agreement file because the file format or extension is invalid please select a valid file (doc or pdf)";
            }


            if (string.IsNullOrWhiteSpace(filePath))
            { return "0@Path not found"; }

            try
            {
                if (filefinaldoc.Length <= 7344128) //7MB maximum limit
                {
                    // Save file
                    using (var stream = new FileStream(fullMpath, FileMode.Create))
                    {
                        filefinaldoc.CopyTo(stream);
                    }
                    return "1@" + ModifiedName;
                }
                else
                {
                    return "0@Unable to upload,agreement file exceeds maximum limit";
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return "0@" + ex.Message + " Permission to upload agreement file denied";
            }
        }
        private void SendMail(string agreementtype, string Contracttype, string EffectiveDate, string Dateofexpiry, string VendorName, string hdauthcode, string hdauthname, string hdauthemailid)
        {
            try
            {
                string ECODE = hdauthcode;
                string ENAME = hdauthname;
                string EMAILID = hdauthemailid;
                if (!string.IsNullOrWhiteSpace(EMAILID))
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    sendMail.MailTo = EMAILID;
                    string strSubject = "Pending for the Interoffice Memorandum Approval";
                    string strBody = "<div style='width:700px;border:2px skyblue solid;border-top-color:white;paddding-top:0px'><div style='width:700px;height:25px;background-color:skyblue;'><b>Interoffice Memorandum Request</b></div>"
                            + "<table cellpadding=0 cellspacing=0 border=0 width=700px >"
                            + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + ENAME + " San</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                            + "<tr><td colspan=2>&nbsp;&nbsp;Interoffice Memorandum Request is pending at your end. Please approve the same.</td></tr><tr><td colspan=2>&nbsp;</td></tr></table><br/>";

                    strBody = strBody + "<table style='border:1px solid #C1DAD7;border-collapse:collapse;margin-left:3px;' cellspacing='0' cellpadding='2' border='1'>"
                    + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Agreement Type</td>"
                    + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;" + agreementtype + "</td>"
                    + "<td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Contract Type</td>"
                    + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + Contracttype + "</td></tr>"
                    + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Effective Date</td>"
                    + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + EffectiveDate + "</td>"
                    + "<td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Expiry Date</td>"
                    + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + Dateofexpiry + "</td></tr>"
                    + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Vendor Name</td>"
                    + "<td style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' colspan='3'>&nbsp;" + VendorName + "</td></tr></table>";

                    strBody = strBody +
                    "<table cellpadding=0 cellspacing=0 border=0 ><tr><td><br/>&nbsp;&nbsp; Please login<a href=" + serverpath.getServerPath() + "> E-Portal</a> for approval process." +
                    "</td></tr><tr><td><b>&nbsp;&nbsp;Thank You<br/></b><br/></td></tr><tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr><tr><td>&nbsp;&nbsp;Team Legal<br/></td></tr><tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>" +
                    "</tr></table></td></tr><tr><td></td></tr><tr> " +
                    "</tr></table></div> ";
                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    sendMail.Send();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
            }
        }
        
        private void SendMailIOMApprovalForm(IOMApprovalFormSave model, string status)
        {
            try
            {
                ////////////////////////////////////////////////////////If Approve///////////////////////////////////////////////////
                if (status == "1")
                {
                    string[] arr;
                    string ENAME = string.Empty;
                    if (model.hdauthname != "")
                    {
                        ENAME = model.hdauthname;
                    }
                    else
                    {
                        arr = model.ddl_appauth.Split(new Char[] { '[' });
                        ENAME = arr[0];
                    }

                    string EMAILID = string.Empty;
                    if (model.hdauthemailid != "")
                    {
                        EMAILID = model.hdauthemailid;
                    }
                    else
                    {
                        EMAILID = model.ddl_appauth.ToString();
                    }


                    //Legal Mail
                    if (ENAME == "LEGAL")
                    {

                        DataSet dtEmail = _iOMContractRepos.GetUserWithLegalApprovalRights();
                        string _MailToIds = string.Empty;
                        foreach (DataRow item in dtEmail.Tables[0].Rows)
                        {
                            _MailToIds += Convert.ToString(item["EMAILID"]) + ",";
                        }
                        _MailToIds.Remove(_MailToIds.Length - 1);

                        if (!string.IsNullOrEmpty(_MailToIds))
                        {
                            commanEmail sendMail = new commanEmail();
                            sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                            sendMail.MailTo = _MailToIds;
                            string strSubject = "Pending for the Interoffice Memorandum Approval";
                            string strBody = "<div style='width:700px;border:2px skyblue solid;border-top-color:white;paddding-top:0px'><div style='width:700px;height:25px;background-color:skyblue;'><b>Interoffice Memorandum Request</b></div>"
                                            + "<table cellpadding=0 cellspacing=0 border=0 width=700px >"
                                            + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + "Legal Team" + " </b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                                            + "<tr><td colspan=2>&nbsp;&nbsp;Interoffice Memorandum Request is pending at your end. Please approve the same.</td></tr><tr><td colspan=2>&nbsp;</td></tr></table><br/>";

                            strBody = strBody + "<table style='border:1px solid #C1DAD7;border-collapse:collapse;margin-left:3px;' cellspacing='0' cellpadding='2' border='1'>"
                            + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Agreement Type</td>"
                            + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;" + model.lblagreementtype + "</td>"
                            + "<td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Contract Type</td>"
                            + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + model.lblcontracttype + "</td></tr>"
                            + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Effective Date</td>"
                            + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + model.lbleffdate + "</td>"
                            + "<td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Expiry Date</td>"
                            + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + model.lblexpdate + "</td></tr>"
                            + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Vendor Name</td>"
                            + "<td style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' colspan='3'>&nbsp;" + model.lblvendorname + "</td></tr></table><br/>";

                            strBody = strBody +
                            "<table cellpadding=0 cellspacing=0 border=0 ><tr><td><br/>&nbsp;&nbsp; Please login<a href=" + serverpath.getServerPath() + "> E-Portal</a> for approval process." +
                            "</td></tr><tr><td><b>&nbsp;&nbsp;Thank You<br/></b><br/></td></tr><tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr><tr><td>&nbsp;&nbsp;Team Legal<br/></td></tr><tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>" +
                            "</tr></table></td></tr><tr><td></td></tr><tr> " +
                            "</tr></table></div> ";
                            sendMail.MailSubject = strSubject;
                            sendMail.MailBody = strBody;
                            sendMail.Send();
                        }
                    }




                    if (!string.IsNullOrEmpty(model.hdauthemailid))
                    {
                        commanEmail sendMail = new commanEmail();
                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        sendMail.MailTo = model.hdauthemailid;
                        string strSubject = "Pending for the Interoffice Memorandum Approval";
                        string strBody = "<div style='width:700px;border:2px skyblue solid;border-top-color:white;paddding-top:0px'><div style='width:700px;height:25px;background-color:skyblue;'><b>Interoffice Memorandum Request</b></div>"
                                        + "<table cellpadding=0 cellspacing=0 border=0 width=700px >"
                                        + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + ENAME + " San</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                                        + "<tr><td colspan=2>&nbsp;&nbsp;Interoffice Memorandum Request is pending at your end. Please approve the same.</td></tr><tr><td colspan=2>&nbsp;</td></tr></table><br/>";

                        strBody = strBody + "<table style='border:1px solid #C1DAD7;border-collapse:collapse;margin-left:3px;' cellspacing='0' cellpadding='2' border='1'>"
                        + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Agreement Type</td>"
                        + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;" + model.lblagreementtype + "</td>"
                        + "<td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Contract Type</td>"
                        + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + model.lblcontracttype + "</td></tr>"
                        + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Effective Date</td>"
                        + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + model.lbleffdate + "</td>"
                        + "<td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Expiry Date</td>"
                        + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + model.lblexpdate + "</td></tr>"
                        + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Vendor Name</td>"
                        + "<td style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' colspan='3'>&nbsp;" + model.lblvendorname + "</td></tr></table><br/>";

                        strBody = strBody +
                        "<table cellpadding=0 cellspacing=0 border=0 ><tr><td><br/>&nbsp;&nbsp; Please login<a href=" + serverpath.getServerPath() + "> E-Portal</a> for approval process." +
                        "</td></tr><tr><td><b>&nbsp;&nbsp;Thank You<br/></b><br/></td></tr><tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr><tr><td>&nbsp;&nbsp;Team Legal<br/></td></tr><tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>" +
                        "</tr></table></td></tr><tr><td></td></tr><tr> " +
                        "</tr></table></div> ";
                        sendMail.MailSubject = strSubject;
                        sendMail.MailBody = strBody;
                        sendMail.Send();
                    }
                }
                ////////////////////////////////////////////////////////If Reject///////////////////////////////////////////////////
                else if (status == "2")
                {
                    if (!string.IsNullOrEmpty(model.hdreqemail))
                    {
                        commanEmail sendMail = new commanEmail();
                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        sendMail.MailTo = model.hdreqemail;
                        string strSubject = "Reject Interoffice Memorandum";
                        string strBody = "<div style='width:700px;border:2px skyblue solid;border-top-color:white;paddding:5px'><div style='width:700px;height:25px;background-color:skyblue;'><b>Interoffice Memorandum Request</b></div>"
                                        + "<table cellpadding=0 cellspacing=0 border=0 width=700px >"
                                        + "<tr><td colspan=2>&nbsp;&nbsp;<b>Dear " + model.hdreqname + " San</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                                        + "<tr><td colspan=2>&nbsp;&nbsp;Your Interoffice Memorandum rejected by approval authority</td></tr>"
                                        + "<tr><td colspan=2>&nbsp;</td></tr></table><br/>";

                        strBody = strBody + "<table style='border:1px solid #C1DAD7;border-collapse:collapse;margin-left:3px;' cellspacing='0' cellpadding='2' border='1'>"
                         + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Agreement Type</td>"
                         + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;" + model.lblagreementtype + "</td>"
                         + "<td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Contract Type</td>"
                         + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + model.lblcontracttype + "</td></tr>"
                         + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Effective Date</td>"
                         + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + model.lbleffdate + "</td>"
                         + "<td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Expiry Date</td>"
                         + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + model.lblexpdate + "</td></tr>"
                         + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Vendor Name</td>"
                         + "<td style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' colspan='3'>&nbsp;" + model.lblvendorname + "</td></tr>"
                         + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Remarks</td>"
                         + "<td style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' colspan='3'>&nbsp;" + model.txt_Remarks + "</td></tr></table><br/>";


                        strBody = strBody +
                        "<table cellpadding=0 cellspacing=0 border=0 ><tr><td><br/>&nbsp;&nbsp; Please login<a href=" + serverpath.getServerPath() + "> E-Portal</a> for approval process." +
                        "</td></tr><tr><td><b>&nbsp;&nbsp;Thank You<br/></b><br/></td></tr><tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr><tr><td>&nbsp;&nbsp;Team Legal<br/></td></tr><tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>" +
                        "</tr></table></td></tr><tr><td></td></tr><tr> " +
                        "</tr></table></div> ";
                        sendMail.MailSubject = strSubject;
                        sendMail.MailBody = strBody;
                        sendMail.Send();
                    }
                }
                /////////////////////////////////////////////////////////If Sendback///////////////////////////////////////////////////
                else if (status == "3")
                {
                    if (!string.IsNullOrEmpty(model.hdreqemail))
                    {
                        commanEmail sendMail = new commanEmail();
                        sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                        sendMail.MailTo = model.hdreqemail;
                        string strSubject = "Send Back Interoffice Memorandum";
                        string strBody = "<div style='width:700px;border:2px skyblue solid;border-top-color:white;paddding:5px'><div style='width:700px;height:25px;background-color:skyblue;'><b>Interoffice Memorandum Request</b></div>"
                                        + "<table cellpadding=0 cellspacing=0 border=0 width=700px >"
                                        + "<tr><td colspan=2>&nbsp;&nbsp;<b>Dear " + model.hdreqname + " San</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                                        + "<tr><td colspan=2>&nbsp;&nbsp;Your Interoffice Memorandum send back by approval authority</td></tr>"
                                        + "<tr><td colspan=2>&nbsp;</td></tr></table><br/>";

                        strBody = strBody + "<table style='border:1px solid #C1DAD7;border-collapse:collapse;margin-left:3px;' cellspacing='0' cellpadding='2' border='1'>"
                         + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Agreement Type</td>"
                         + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;" + model.lblagreementtype + "</td>"
                         + "<td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Contract Type</td>"
                         + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + model.lblcontracttype + "</td></tr>"
                         + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Effective Date</td>"
                         + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + model.lbleffdate + "</td>"
                         + "<td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Expiry Date</td>"
                         + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + model.lblexpdate + "</td></tr>"
                         + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Vendor Name</td>"
                         + "<td style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' colspan='3'>&nbsp;" + model.lblvendorname + "</td></tr>"
                         + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Remarks</td>"
                         + "<td style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' colspan='3'>&nbsp;" + model.txt_Remarks + "</td></tr></table><br/>";

                        strBody = strBody +
                        "<table cellpadding=0 cellspacing=0 border=0 ><tr><td><br/>&nbsp;&nbsp; Please login<a href=" + serverpath.getServerPath() + "> E-Portal</a> for approval process." +
                        "</td></tr><tr><td><b>&nbsp;&nbsp;Thank You<br/></b><br/></td></tr><tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr><tr><td>&nbsp;&nbsp;Team Legal<br/></td></tr><tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>" +
                        "</tr></table></td></tr><tr><td></td></tr><tr> " +
                        "</tr></table></div> ";
                        sendMail.MailSubject = strSubject;
                        sendMail.MailBody = strBody;
                        sendMail.Send();
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);

            }
        }
        
        private void SendMailUserAcknowledgemen(UserAcknowledgementSave model)
        {
            try
            {
                DataSet dtEmail = _iOMContractRepos.GetUserWithLegalApprovalRights();
                string _MailToIds = string.Empty;
                foreach (DataRow item in dtEmail.Tables[0].Rows)
                {
                    _MailToIds += Convert.ToString(item["EMAILID"]) + ",";
                }
                _MailToIds.Remove(_MailToIds.Length - 1);


                if (!string.IsNullOrWhiteSpace(_MailToIds))
                {
                    commanEmail sendMail = new commanEmail();
                    sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                    sendMail.MailTo = _MailToIds;
                    string strSubject = "User acknowledgement for the Interoffice Memorandum Approval - ";
                    string strBody = "<div style='width:700px;border:2px skyblue solid;border-top-color:white;paddding-top:0px'><div style='width:700px;height:25px;background-color:skyblue;'><b>Interoffice Memorandum Request</b></div>"
                                    + "<table cellpadding=0 cellspacing=0 border=0 width=700px >"
                                    + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + "Legal Team" + " </b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                                    + "<tr><td colspan=2>&nbsp;&nbsp;User has acknowledge Interoffice Memorandum. </td></tr> "
                                    + "<tr><td colspan=2>&nbsp;</td></tr></table><br/>";

                    strBody = strBody + "<table style='border:1px solid #C1DAD7;border-collapse:collapse;margin-left:3px;' cellspacing='0' cellpadding='2' border='1'>"
                    + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Agreement Type</td>"
                    + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;" + model.lblagreementtype + "</td>"
                    + "<td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Contract Type</td>"
                    + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + model.lblcontracttype + "</td></tr>"
                    + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Effective Date</td>"
                    + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + model.lbleffdate + "</td>"
                    + "<td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Expiry Date</td>"
                    + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + model.lblexpdate + "</td></tr>"
                    + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Vendor Name</td>"
                    + "<td style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' colspan='3'>&nbsp;" + model.lblvendorname + "</td></tr>"
                    + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Remark</td>"
                    + "<td style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' colspan='3'>&nbsp;" + model.txt_Remarks + "</td></tr>"
                    +
                    "</table><br/>";

                    strBody = strBody +
                    "<table cellpadding=0 cellspacing=0 border=0 ><tr><td><br/>&nbsp;&nbsp; Please login<a href=" + serverpath.getServerPath() + "> E-Portal</a> for more detail." +
                    "</td></tr><tr><td><b>&nbsp;&nbsp;Thank You<br/></b><br/></td></tr><tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr><tr><td>&nbsp;&nbsp;Team Legal<br/></td></tr><tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>" +
                    "</tr></table></td></tr><tr><td></td></tr><tr> " +
                    "</tr></table></div> ";
                    sendMail.MailSubject = strSubject;
                    sendMail.MailBody = strBody;
                    sendMail.Send();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);

            }
        }


        private string uploadCommunicationFile(IFormFile? RefUploadDocument)
        {

            if (RefUploadDocument == null || string.IsNullOrWhiteSpace(RefUploadDocument.FileName))
            {
                return "0@Invalid communication file name supplied";
            }

            if (RefUploadDocument == null || RefUploadDocument.Length == 0)
            {
                return "0@Invalid communication file content";
            }

            string fullpath = Path.Combine(filePath, RefUploadDocument.FileName);

            string fileext = (System.IO.Path.GetExtension(fullpath)).ToString().ToLower().Trim();

            string ModifiedName = "Comm" + System.DateTime.Now.Day + System.DateTime.Now.Month + System.DateTime.Now.Year + System.DateTime.Now.Hour + System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Millisecond + fileext;
            string fullMpath = Path.Combine(filePath, ModifiedName);

            if (fileext.ToLower() != ".pdf" && fileext.ToLower() != ".doc")
            {
                return "0@Cannot upload communication file because the file format or extension is invalid please select a valid file (doc or pdf)";
            }

            if (string.IsNullOrWhiteSpace(filePath))
            { return "0@Path not found"; }

            try
            {
                if (RefUploadDocument.Length <= 20480000)
                {
                    // Save file
                    using (var stream = new FileStream(fullMpath, FileMode.Create))
                    {
                        RefUploadDocument.CopyTo(stream);
                    }
                    return "1@" + ModifiedName;
                }
                else
                {
                    return "0@Unable to upload,communication file exceeds maximum limit";
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                return "0@" + ex.Message + " Permission to upload communication file denied";
            }
        }

        private void SendCommunicationMail(UserCommunicationSave model)
        {
            ////////////////////////////////////////////////////////If Approve///////////////////////////////////////////////////

            if (!string.IsNullOrWhiteSpace(model.HDEMAILID))
            {
                commanEmail sendMail = new commanEmail();
                sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                sendMail.MailTo = model.HDEMAILID;
                string strSubject = "Legal Communication";
                string strBody = "<div style='width:700px;border:2px skyblue solid;border-top-color:white;paddding-top:0px'><div style='width:700px;height:25px;background-color:skyblue;'><b>Legal Communication</b></div><br/>"
                                + "<table cellpadding=0 cellspacing=0 border=0 width=700px >"
                                + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + model.HDENAME + " San</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                                + "<tr><td colspan=2>&nbsp;&nbsp;Interoffice Memorandum communication acknowledgement. Please revert the same.</td></tr><tr><td colspan=2>&nbsp;</td></tr></table><br/>";

                strBody = strBody + "<table style='border:1px solid #C1DAD7;border-collapse:collapse;margin-left:3px;' cellspacing='0' cellpadding='2' border='1'>"
                 + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Requestor Name</td>"
                    + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;" + model.lbl_assname + "</td>"
                    + "<td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;IOM ID</td>"
                    + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;" + model.hdiomid + "</td></tr>"
                + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Agreement Type</td>"
                + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;" + model.lblagreementtype + "</td>"
                + "<td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Contract Type</td>"
                + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + model.lblcontracttype + "</td></tr>"
                + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Effective Date</td>"
                + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + model.lbleffdate + "</td>"
                + "<td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Expiry Date</td>"
                + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + model.lblexpdate + "</td></tr>"
                + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Vendor Name</td>"
                + "<td style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' colspan='3'>&nbsp;" + model.lblvendorname + "</td></tr>"
                + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Admin Remarks</td>"
                + "<td style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' colspan='3'>&nbsp;" + model.txtMessageRemarks + "</td></tr></table><br/>";
                strBody = strBody +
                    "<table cellpadding=0 cellspacing=0 border=0 ><tr><td><br/>&nbsp;&nbsp; Please login<a href=" + serverpath.getServerPath() + "Login> E-Portal</a> for Interoffice Memorandum communication process." +
                    "</td></tr><tr><td><b>&nbsp;&nbsp;Thank You<br/></b><br/></td></tr><tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr><tr><td>&nbsp;&nbsp;Team Legal<br/></td></tr><tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>" +
                    "</tr></table></td></tr><tr><td></td></tr><tr> " +
                    "</tr></table></div> ";
                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;
                sendMail.Send();
            }

        }

        private void SendMailContractRenewal(string? hdauthcode, string? hdauthname, string? hdauthemailid, string? Contracttype, string? EffectiveDate, string? Dateofexpiry, string? VendorName)
        {
            string? ECODE = hdauthcode;
            string? ENAME = hdauthname;
            string? EMAILID = hdauthemailid;
            if (!string.IsNullOrWhiteSpace(EMAILID))
            {
                commanEmail sendMail = new commanEmail();
                sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                sendMail.MailTo = EMAILID;
                string strSubject = "Pending for the Interoffice Memorandum Approval";
                string strBody = "<div style='width:700px;border:2px skyblue solid;border-top-color:white;paddding-top:0px'><div style='width:700px;height:25px;background-color:skyblue;'><b>Interoffice Memorandum Request</b></div>"
                                + "<table cellpadding=0 cellspacing=0 border=0 width=700px >"
                                + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + ENAME + " San</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                                + "<tr><td colspan=2>&nbsp;&nbsp;Interoffice Memorandum Request is pending at your end. Please approve the same.</td></tr><tr><td colspan=2>&nbsp;</td></tr></table><br/>";

                strBody = strBody + "<table style='border:1px solid #C1DAD7;border-collapse:collapse;margin-left:3px;' cellspacing='0' cellpadding='2' border='1'>"
                + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Agreement Type</td>"
                + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Renewal</td>"
                + "<td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Contract Type</td>"
                + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + Contracttype + "</td></tr>"
                + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Effective Date</td>"
                + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + EffectiveDate + "</td>"
                + "<td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Expiry Date</td>"
                + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + Dateofexpiry + "</td></tr>"
                + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;Vendor Name</td>"
                + "<td style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'  colspan='3'>&nbsp;" + VendorName + "</td></tr></table>";


                strBody = strBody +
                "<table cellpadding=0 cellspacing=0 border=0 ><tr><td><br/>&nbsp;&nbsp; Please login<a href=" + serverpath.getServerPath() + "> E-Portal</a> for approval process." +
                "</td></tr><tr><td><b>&nbsp;&nbsp;Thank You<br/></b><br/></td></tr><tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr><tr><td>&nbsp;&nbsp;Team Legal<br/></td></tr><tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>" +
                "</tr></table></td></tr><tr><td></td></tr><tr> " +
                "</tr></table></div> ";
                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;
                sendMail.Send();
            }
        }
        private void SendMailContractAmendment(string? hdauthcode, string? hdauthname, string? hdauthemailid, string? Contracttype, string? EffectiveDate, string? Dateofexpiry, string? VendorName)
        {
            string ECODE = hdauthcode ?? string.Empty;
            string ENAME = hdauthname ?? string.Empty;
            string EMAILID = hdauthemailid ?? string.Empty;
            if (!string.IsNullOrEmpty(EMAILID))
            {
                commanEmail sendMail = new commanEmail();
                sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                sendMail.MailTo = EMAILID;
                string strSubject = "Pending for the Interoffice Memorandum Approval";
                string strBody = "<div style='width:700px;border:2px skyblue solid;border-top-color:white;paddding-top:0px'><div style='width:700px;height:25px;background-color:skyblue;'><b>Interoffice Memorandum Request</b></div>"
                                + "<table cellpadding=0 cellspacing=0 border=0 width=700px >"
                                + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + ENAME + " San</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                                + "<tr><td colspan=2>&nbsp;&nbsp;Interoffice Memorandum Request is pending at your end. Please approve the same.</td></tr><tr><td colspan=2>&nbsp;</td></tr></table><br/>";

                strBody = strBody + "<table style='border:1px solid #C1DAD7;border-collapse:collapse;margin-left:3px;' cellspacing='0' cellpadding='2' border='1'>"
                + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Agreement Type</td>"
                + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Renewal</td>"
                + "<td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Contract Type</td>"
                + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + Contracttype + "</td></tr>"
                + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Effective Date</td>"
                + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + EffectiveDate + "</td>"
                + "<td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Expiry Date</td>"
                + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + Dateofexpiry + "</td></tr>"
                + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Vendor Name</td>"
                + "<td style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' colspan='3'>&nbsp;" + VendorName + "</td></tr></table>";


                strBody = strBody +
                "<table cellpadding=0 cellspacing=0 border=0 ><tr><td><br/>&nbsp;&nbsp; Please login<a href=" + serverpath.getServerPath() + "> E-Portal</a> for approval process." +
                "</td></tr><tr><td><b>&nbsp;&nbsp;Thank You<br/></b><br/></td></tr><tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr><tr><td>&nbsp;&nbsp;Team Legal<br/></td></tr><tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>" +
                "</tr></table></td></tr><tr><td></td></tr><tr> " +
                "</tr></table></div> ";
                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;
                sendMail.Send();
            }
        }

        private void SendMailVerificationRequest(string? hdauthcode, string? hdauthname, string? hdauthemailid, string? agreementtype, string? Contracttype, string? EffectiveDate, string? Dateofexpiry, string? VendorName)
        {
            string? ECODE = hdauthcode;
            string? ENAME = hdauthname;
            string? EMAILID = hdauthemailid;
            if (!string.IsNullOrEmpty(EMAILID))
            {
                commanEmail sendMail = new commanEmail();
                sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                sendMail.MailTo = EMAILID;
                string strSubject = "Pending for the Interoffice Memorandum Approval";
                string strBody = "<div style='width:700px;border:2px skyblue solid;border-top-color:white;paddding-top:0px'><div style='width:700px;height:25px;background-color:skyblue;'><b>Interoffice Memorandum Request</b></div>"
                                + "<table cellpadding=0 cellspacing=0 border=0 width=700px >"
                                + "<tr><td colspan=2><b>&nbsp;&nbsp;Dear " + ENAME + " San</b><br/></td></tr><tr><td colspan=2>&nbsp;</td></tr>"
                                + "<tr><td colspan=2>&nbsp;&nbsp;Interoffice Memorandum Request is pending at your end. Please approve the same.</td></tr><tr><td colspan=2>&nbsp;</td></tr></table><br/>";

                strBody = strBody + "<table style='border:1px solid #C1DAD7;border-collapse:collapse;margin-left:3px;' cellspacing='0' cellpadding='2' border='1'>"
                + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Agreement Type</td>"
                + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;" + agreementtype + "</td>"
                + "<td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Contract Type</td>"
                + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + Contracttype + "</td></tr>"
                + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Effective Date</td>"
                + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + EffectiveDate + "</td>"
                + "<td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Expiry Date</td>"
                + "<td width='220' style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;'>&nbsp;" + Dateofexpiry + "</td></tr>"
                + "<tr><td width='130' style='background-color:#CAE8EA;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' >&nbsp;Vendor Name</td>"
                + "<td style='background-color:white;border-bottom:1px solid #C1DAD7;border-right:1px solid #C1DAD7;border-top:1px solid #C1DAD7;font-size:10px;font-weight:bold;letter-spacing:1px;line-height:22px;text-align:left;font-family:Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;' colspan='3'>&nbsp;" + VendorName + "</td></tr></table>";

                strBody = strBody +
                "<table cellpadding=0 cellspacing=0 border=0 ><tr><td><br/>&nbsp;&nbsp; Please login<a href=" + serverpath.getServerPath() + "> E-Portal</a> for approval process." +
                "</td></tr><tr><td><b>&nbsp;&nbsp;Thank You<br/></b><br/></td></tr><tr><td><br /><b>&nbsp;&nbsp;Best Regards</b><br /></td></tr><tr><td>&nbsp;&nbsp;Team Legal<br/></td></tr><tr><td><strong>&nbsp;&nbsp;Note: It is a system generated email, please do not reply.</strong></td></tr>" +
                "</tr></table></td></tr><tr><td></td></tr><tr> " +
                "</tr></table></div> ";
                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;
                sendMail.Send();
            }
        }

        #endregion
    }
}
