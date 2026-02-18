using Microsoft.AspNetCore.Mvc;
using ePortal.WebUI.Filters;
using ePortal.Application.APPX.Contracts;
using ePortal.ViewModels;
using ePortal.Shared.Interface;
using ePortal.Persistence.Admin.Interface;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using ePortal.Persistence.Interface;
using ePortal.Shared;
using ePortal.ViewModels.APPX.IOM;
using System.Net;


namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    [SessionTimeout]
    public class IOMContractController : Controller
    {
        private readonly ILogger<IOMContractController> _logger;
        private readonly IIOMContractService _iomContractService;
        private readonly IIOMContractRepos _iomContractRepos;
        private readonly ISessionService _sessionService;
        private readonly ISearchEmp _searchEmp;


        private readonly Employee_Details _empDetails;
        private readonly string _userId;
        public IOMContractController(
            ISessionService sessionService,
            IIOMContractService iomContractService,
            ISearchEmp searchEmp,
            IIOMContractRepos iomContractRepos,
            ILogger<IOMContractController> logger)
        {
            _logger = logger;
            _sessionService = sessionService;
            _iomContractService = iomContractService;
            _iomContractRepos = iomContractRepos;
            _searchEmp = searchEmp;


            _empDetails = _sessionService?.Get<Employee_Details>("Employee");
            _userId = _sessionService.Get<string>("userID").ToString();
           
        }


        #region IOMRequestForm

        [HttpGet]
        public IActionResult IOMEditRequestForm([FromQuery] string IOMID)
        {
            IOMRequestEditFormViewModel model = new IOMRequestEditFormViewModel();
            string IOM_ID = WebUtility.UrlDecode(Encryption.Decrypt(Convert.ToString(IOMID)));

            ViewBag.encIOMID = IOMID;
            ViewBag.ddlfromEnable = true; //
            ViewBag.lbl_appauthVisiable = true; //
            ViewBag.ddl_appauthVisiable = false; //

            string? OPHEADID = _empDetails.Operation_Id;
            if (!string.IsNullOrWhiteSpace(OPHEADID))
            {
                ViewBag.ddlfromEnable = false; //
            }

            #region FillOperation
            var dsGetAllOperation = _searchEmp.get_AllOperation();
            var ddlFromList = new List<SelectListItem>();
            if (dsGetAllOperation != null && dsGetAllOperation.Tables.Count > 0 && dsGetAllOperation.Tables[0].Rows.Count > 0)
            {
                ddlFromList = dsGetAllOperation.Tables[0].AsEnumerable()
                  .Select(row => new SelectListItem
                  {
                      Text = row["OPERATION"].ToString(),
                      Value = row["OPERATIONID"].ToString(),
                  }).ToList();
            }
            ddlFromList.Insert(0, new SelectListItem
            {
                Text = "-Select Operation-",
                Value = "0"
            });
            #endregion

            #region contract
            var dsGetContract = _iomContractRepos.GETCONTRACTMASTER(string.Empty, string.Empty, "1", string.Empty);
            var ddlcontractList = new List<SelectListItem>();
            if (dsGetContract != null && dsGetContract.Tables.Count > 0 && dsGetContract.Tables[0].Rows.Count > 0)
            {
                ddlcontractList = dsGetContract.Tables[0].AsEnumerable()
                  .Select(row => new SelectListItem
                  {
                      Text = row["CONTRATNAME"].ToString(),
                      Value = row["CONTRACTTYPEID"].ToString()
                  }).ToList();
            }
            ddlcontractList.Insert(0, new SelectListItem
            {
                Text = "-Select Contract-",
                Value = string.Empty
            });

            #endregion

            #region PopulateDateTime
            int intVal_day = 1;
            var optDayList = new List<SelectListItem>();
            var optMonthList = model.optMonthList;
            var optEDList = new List<SelectListItem>();

            var ddlreturndList = new List<SelectListItem>();
            var ddlreturn2dList = new List<SelectListItem>();

            var optYearList = new List<SelectListItem>();
            var optEYList = new List<SelectListItem>();
            var optEMList = model.optEMList;
            var ddlreturnyList = new List<SelectListItem>();
            var ddlreturn2yList = new List<SelectListItem>();

            do
            {
                optDayList.Add(new SelectListItem(intVal_day.ToString(), intVal_day.ToString()));
                optEDList.Add(new SelectListItem(intVal_day.ToString(), intVal_day.ToString()));
                ddlreturndList.Add(new SelectListItem(intVal_day.ToString(), intVal_day.ToString()));
                ddlreturn2dList.Add(new SelectListItem(intVal_day.ToString(), intVal_day.ToString()));

                intVal_day = intVal_day + 1;
            }
            while (intVal_day < 32);

            int currentYr = DateTime.Now.Year;
            for (int i = currentYr - 1; i <= (currentYr + 102); i++)
            {
                optYearList.Add(new SelectListItem(i.ToString(), i.ToString()));
            }
            for (int i = currentYr - 1; i <= (currentYr + 1); i++)
            {
                optEYList.Add(new SelectListItem(i.ToString(), i.ToString()));
                ddlreturnyList.Add(new SelectListItem(i.ToString(), i.ToString()));
                ddlreturn2yList.Add(new SelectListItem(i.ToString(), i.ToString()));
            }
            int intMonth = DateTime.Now.Month;
            int intDay = DateTime.Now.Day;
            int intYear = DateTime.Now.Year;

            var optDay = optDayList.FirstOrDefault(i => i.Value == intDay.ToString())?.Value;
            var optMonth = optMonthList.FirstOrDefault(i => i.Value == intMonth.ToString())?.Value;
            var optYear = optYearList.FirstOrDefault(i => i.Value == intYear.ToString())?.Value;

            var optED = optEDList.FirstOrDefault(i => i.Value == intDay.ToString())?.Value;
            var optEM = optEMList.FirstOrDefault(i => i.Value == intMonth.ToString())?.Value;
            var optEY = optEYList.FirstOrDefault(i => i.Value == intYear.ToString())?.Value;
            #endregion

            #region GetNextapprovalauthority
            string? hdauthcode = null;
            string? hdauthname = null;
            string? hdauthemailid = null;
            string? lbl_appauth = null;
            string? hdauthlevel = null;
            var ddl_appauthList = new List<SelectListItem>();

            DataSet ds = new DataSet();
            ds = _iomContractRepos.GetNextApprovalAuthority(_userId);
            if (ds.Tables[0].Rows[0]["AUTHLEVEL"].ToString() != "4")
            {
                hdauthcode = ds.Tables[0].Rows[0]["ADEMPCODE"].ToString();
                hdauthname = ds.Tables[0].Rows[0]["EMPNAME"].ToString();
                hdauthemailid = ds.Tables[0].Rows[0]["EMAILID"].ToString();
                lbl_appauth = ds.Tables[0].Rows[0]["EMPNAME"].ToString() + "[" + ds.Tables[0].Rows[0]["ADEMPCODE"].ToString() + "]";
                hdauthlevel = ds.Tables[0].Rows[0]["AUTHLEVEL"].ToString();
            }
            else
            {
                hdauthlevel = "4";
                ViewBag.lbl_appauthVisiable = false; //
                ViewBag.ddl_appauthVisiable = true; //

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ddl_appauthList = ds.Tables[0].AsEnumerable()
                      .Select(row => new SelectListItem
                      {
                          Text = row["EMPLOYEE"].ToString(),
                          Value = row["ADEMPCODE"].ToString(),
                      }).ToList();
                }
                ddl_appauthList.Insert(0, new SelectListItem("-Select-", ""));
            }


            #endregion

            #region ddlMasterAgreement
            var dsGetMasterAgreement = _iomContractRepos.GETCONTRACTLIST(string.Empty, string.Empty, string.Empty, string.Empty, _userId, string.Empty);
            var ddlmstagreementList = new List<SelectListItem>();
            if (dsGetMasterAgreement != null && dsGetMasterAgreement.Tables.Count > 0 && dsGetMasterAgreement.Tables[0].Rows.Count > 0)
            {
                ddlmstagreementList = dsGetMasterAgreement.Tables[0].AsEnumerable()
                  .Select(row => new SelectListItem
                  {
                      Text = row["VENDORNAME"].ToString(),
                      Value = row["AGREEMENTHEADERID"].ToString()
                  }).ToList();
            }
            ddlmstagreementList.Insert(0, new SelectListItem
            {
                Text = "-Select Master Agreement-",
                Value = string.Empty
            });
            #endregion


            model = new IOMRequestEditFormViewModel
            {
                ddlFromList = ddlFromList,
                ddlfrom = OPHEADID,

                ddlcontractList = ddlcontractList,
                ddlmstagreementList = ddlmstagreementList,

                optDay = optDay,
                optMonth = optMonth,
                optYear = optYear,
                optED = optED,
                optEM = optEM,
                optEY = optEY,

                optDayList = optDayList,
                optMonthList = optMonthList,
                optYearList = optYearList,
                optEDList = optEDList,
                optEMList = optEMList,
                optEYList = optEYList,

                ddlreturndList = ddlreturndList,
                ddlreturn2dList = ddlreturn2dList,
                ddlreturnyList = ddlreturnyList,
                ddlreturn2yList = ddlreturn2yList,

                txtTo = "Legal & Secretariat",

                #region approval authority binding
                hdauthcode = hdauthcode,
                hdauthname = hdauthname,
                hdauthemailid = hdauthemailid,
                lbl_appauth = lbl_appauth,
                hdauthlevel = hdauthlevel,
                ddl_appauthList = ddl_appauthList,
                #endregion

            };

            #region GetIOMdetail
            ds = _iomContractRepos.GetIOMFullDetail(IOM_ID, "");
            if (ds.Tables[1].Rows.Count > 0)
            {

                string agreementtype = ds.Tables[1].Rows[0]["AGREEMENTTYPEID"].ToString();
                model.ddlagreementList = new();
                //Fill Dropdown according the agreement type
                if (agreementtype == "1" || agreementtype == "2")
                {
                    model.ddlagreementList.Add(new SelectListItem { Text = "Drafting", Value = "1" });
                    model.ddlagreementList.Add(new SelectListItem { Text = "Vetting", Value = "2" });

                    model.ddlagreement = agreementtype;
                }
                else if (agreementtype == "3")
                {
                    model.ddlagreementList.Add(new SelectListItem { Text = "Renewal", Value = "3" });
                }
                else if (agreementtype == "4")
                {
                    model.ddlagreementList.Add(new SelectListItem { Text = "Amendment", Value = "4" });
                }
                else if (agreementtype == "5")
                {
                    model.ddlagreementList.Add(new SelectListItem { Text = "Verification", Value = "5" });
                }


                model.ddlcontract = ds.Tables[1].Rows[0]["CONTRACTTYPEID"].ToString();
                model.ddlTermyear = ds.Tables[1].Rows[0]["TERMSYEAR"].ToString();
                model.ddlTermmonth = ds.Tables[1].Rows[0]["TERMSMONTH"].ToString();
                model.txtvendorname = ds.Tables[1].Rows[0]["VENDORNAME"].ToString();
                model.ddlMannerOfPayment = ds.Tables[1].Rows[0]["MANNEROFPAYMENTID"].ToString();
                model.txtPurpose = ds.Tables[1].Rows[0]["PURPOSE"].ToString();
                model.txtRemarks = ds.Tables[1].Rows[0]["REMARKS"].ToString();

                DateTime effdate = Convert.ToDateTime(ds.Tables[1].Rows[0]["EFFECTIVEDATEDESC"]);
                model.optED = Convert.ToString(effdate.Day);
                model.optEM = Convert.ToString(effdate.Month);
                model.optEY = Convert.ToString(effdate.Year);

                DateTime dateofexp = Convert.ToDateTime(ds.Tables[1].Rows[0]["EXPIRYDATE"]);
                model.optDay = Convert.ToString(dateofexp.Day);
                model.optMonth = Convert.ToString(dateofexp.Month);
                model.optYear = Convert.ToString(dateofexp.Year);

                if (ds.Tables[1].Rows[0]["TRANINGFIELED"].ToString() == "0")
                {
                    ViewBag.trt1Visible = false;
                    ViewBag.trt2Visible = false;
                    ViewBag.trt3Visible = false;
                    ViewBag.trt4Visible = false;
                }
                else
                {
                    ViewBag.trt1Visible = true;
                    ViewBag.trt2Visible = true;
                    ViewBag.trt3Visible = true;
                    ViewBag.trt4Visible = true;


                    DateTime dateofrfrom = Convert.ToDateTime(ds.Tables[1].Rows[0]["RETURNFROMDATE"]);
                    model.ddlreturnd = dateofrfrom.Day.ToString();
                    model.ddlreturnm = dateofrfrom.Month.ToString();
                    model.ddlreturny = dateofrfrom.Year.ToString();

                    DateTime dateofrto = Convert.ToDateTime(ds.Tables[1].Rows[0]["RETURNTODATE"]);
                    model.ddlreturn2d = Convert.ToString(dateofrto.Day);
                    model.ddlreturn2m = Convert.ToString(dateofrto.Month);
                    model.ddlreturn2y = Convert.ToString(dateofrto.Year);

                    model.txtcontactecode = ds.Tables[1].Rows[0]["CONTRACTECODE"].ToString();
                    model.txtsuretyamount = ds.Tables[1].Rows[0]["SURETYAMOUNT"].ToString();
                    model.txtnamesurety = ds.Tables[1].Rows[0]["NAMEOFSURETY"].ToString();

                    
                }

                //////////Approval Note Attachment///////////////////////////////////////////
                string appnoteattchment = ds.Tables[1].Rows[0]["REFERENCEDOC"].ToString();
                if (string.IsNullOrEmpty(appnoteattchment.Trim()))
                {
                    ViewBag.linkappnoteVisible = false; //
                    ViewBag.linkappnotedelVisible = false; //
                    ViewBag.fileUploadAttachmentRAEnabled = true; //
                }
                else
                {
                    ViewBag.linkappnoteVisible = true; //
                    ViewBag.linkappnotedelVisible = true; //
                    ViewBag.fileUploadAttachmentRAEnabled = false; //
                    model.linkappnote = "../../Uploads/IOM/" + appnoteattchment;
                    model.linkappnoteText = appnoteattchment;
                }
                //Show/Hide Anti Beribery and NDA Document
                //if (agreementtype == "5")
                //{
                //    trndadocument.Visible = false;
                //}



                ////////////NDA Document Attachment///////////////////////////////////////////
                //string ndadocument = ds.Tables[1].Rows[0]["NDADOCUMENT"].ToString();
                //if (string.IsNullOrEmpty(ndadocument.Trim()))
                //{
                //    linkbtnndadocument.Visible = false;
                //    linkndadocumentdel.Visible = false;
                //    fileUploadndadocument.Enabled = true;
                //}
                //else
                //{
                //    linkbtnndadocument.Visible = true;
                //    linkndadocumentdel.Visible = true;
                //    fileUploadndadocument.Enabled = false;
                //    linkbtnndadocument.HRef = "../../Uploads/IOM/" + ndadocument;
                //    linkbtnndadocument.InnerText = ndadocument;
                //}
                //////////////////Agreement Attachment////////////////////////////
                if (ds.Tables[2].Rows.Count > 0)
                {
                    //Get Latest agreement
                    DataView dv = ds.Tables[2].DefaultView;
                    dv.RowFilter = "ATTACHMENTPATH IS NOT NULL  AND ATTACHMENTPATH <> ''";
                    dv.Sort = "IOMHISTORYID desc";
                    if (dv.Count > 0)
                    {
                        string referencedoc = dv[0]["ATTACHMENTPATH"].ToString();
                        if (string.IsNullOrEmpty(referencedoc))
                        {
                            ViewBag.linkbtnagreementVisible = false; //
                            ViewBag.linkagreedelVisible = false; //
                            ViewBag.fileUploadAttachmentAAEnabled = true; //
                        }
                        else
                        {
                            ViewBag.linkbtnagreementVisible = true; //
                            ViewBag.linkagreedelVisible = true; //
                            ViewBag.fileUploadAttachmentAAEnabled = false; //
                            model.linkbtnagreement = "../../Uploads/IOM/" + referencedoc;
                            model.linkbtnagreementText = referencedoc;
                        }
                    }
                    else
                    {
                        ViewBag.linkbtnagreementVisible = false;
                        ViewBag.linkagreedelVisible = false;
                        ViewBag.fileUploadAttachmentAAEnabled = true;
                    }
                }


                if (!string.IsNullOrEmpty(ds.Tables[1].Rows[0]["MSTAGR"].ToString()))
                {
                    model.rdomaster = "1";
                    model.ddlmstagreement = ds.Tables[1].Rows[0]["MASTERAGREEMENT"].ToString();
                }
                else
                {
                    ViewBag.tr10Visiable = false;
                    ViewBag.tr11Visiable = false;
                }
                // Start Added by Aumento :: SR79956
                if (ds.Tables[1].Rows[0]["IT_DECLARE"].ToString() == "1")
                {
                    model.CheckDeclaration = "1";
                }
                else
                {
                    model.CheckDeclaration = "0";
                }
                // End Added by Aumento :: SR79956
            }

            #endregion

            return View(model);
        }

        [HttpGet]
        public IActionResult IOMRequestForm()
        {
            IOMRequestFormViewModel model = new IOMRequestFormViewModel();
            ViewBag.ddlfromEnable = true;
            ViewBag.lbl_appauthVisiable = true;
            ViewBag.ddl_appauthVisiable = false;
            string? OPHEADID = _empDetails.Operation_Id;
            if (!string.IsNullOrWhiteSpace(OPHEADID))
            {
                ViewBag.ddlfromEnable = false;
            }

            #region FillOperation
            var dsGetAllOperation = _searchEmp.get_AllOperation();
            var ddlFromList = new List<SelectListItem>();
            if (dsGetAllOperation != null && dsGetAllOperation.Tables.Count > 0 && dsGetAllOperation.Tables[0].Rows.Count > 0)
            {
                ddlFromList = dsGetAllOperation.Tables[0].AsEnumerable()
                  .Select(row => new SelectListItem
                  {
                      Text = row["OPERATION"].ToString(),
                      Value = row["OPERATIONID"].ToString(),
                  }).ToList();
            }
            ddlFromList.Insert(0, new SelectListItem
            {
                Text = "-Select Operation-",
                Value = "0"
            });
            #endregion

            #region contract
            var dsGetContract = _iomContractRepos.GETCONTRACTMASTER(string.Empty, string.Empty, "1", string.Empty);
            var ddlcontractList = new List<SelectListItem>();
            if (dsGetContract != null && dsGetContract.Tables.Count > 0 && dsGetContract.Tables[0].Rows.Count > 0)
            {
                ddlcontractList = dsGetContract.Tables[0].AsEnumerable()
                  .Select(row => new SelectListItem
                  {
                      Text = row["CONTRATNAME"].ToString(),
                      Value = row["CONTRACTTYPEID"].ToString()
                  }).ToList();
            }
            ddlcontractList.Insert(0, new SelectListItem
            {
                Text = "-Select Contract-",
                Value = string.Empty
            });

            #endregion

            #region PopulateDateTime
            int intVal_day = 1;
            var optDayList = new List<SelectListItem>();
            var optMonthList = model.optMonthList;
            var optEDList = new List<SelectListItem>();

            var ddlreturndList = new List<SelectListItem>();
            var ddlreturn2dList = new List<SelectListItem>();

            var optYearList = new List<SelectListItem>();
            var optEYList = new List<SelectListItem>();
            var optEMList = model.optEMList;
            var ddlreturnyList = new List<SelectListItem>();
            var ddlreturn2yList = new List<SelectListItem>();

            do
            {
                optDayList.Add(new SelectListItem(intVal_day.ToString(), intVal_day.ToString()));
                optEDList.Add(new SelectListItem(intVal_day.ToString(), intVal_day.ToString()));
                ddlreturndList.Add(new SelectListItem(intVal_day.ToString(), intVal_day.ToString()));
                ddlreturn2dList.Add(new SelectListItem(intVal_day.ToString(), intVal_day.ToString()));

                intVal_day = intVal_day + 1;
            }
            while (intVal_day < 32);

            int currentYr = DateTime.Now.Year;
            for (int i = currentYr - 1; i <= (currentYr + 102); i++)
            {
                optYearList.Add(new SelectListItem(i.ToString(), i.ToString()));
            }
            for (int i = currentYr - 1; i <= (currentYr + 1); i++)
            {
                optEYList.Add(new SelectListItem(i.ToString(), i.ToString()));
                ddlreturnyList.Add(new SelectListItem(i.ToString(), i.ToString()));
                ddlreturn2yList.Add(new SelectListItem(i.ToString(), i.ToString()));
            }
            int intMonth = DateTime.Now.AddDays(15).Month;
            int intDay = DateTime.Now.AddDays(15).Day;
            int intYear = DateTime.Now.AddDays(15).Year;

            var optDay = optDayList.FirstOrDefault(i => i.Value == intDay.ToString())?.Value;
            var optMonth = optMonthList.FirstOrDefault(i => i.Value == intMonth.ToString())?.Value;
            var optYear = optYearList.FirstOrDefault(i => i.Value == intYear.ToString())?.Value;

            var optED = optEDList.FirstOrDefault(i => i.Value == intDay.ToString())?.Value;
            var optEM = optEMList.FirstOrDefault(i => i.Value == intMonth.ToString())?.Value;
            var optEY = optEYList.FirstOrDefault(i => i.Value == intYear.ToString())?.Value;
            #endregion

            #region GetNextapprovalauthority
            string? hdauthcode = null;
            string? hdauthname = null;
            string? hdauthemailid = null;
            string? lbl_appauth = null;
            string? hdauthlevel = null;
            var ddl_appauthList = new List<SelectListItem>();

            DataSet ds = new DataSet();
            ds = _iomContractRepos.GetNextApprovalAuthority(_userId);
            if (ds.Tables[0].Rows[0]["AUTHLEVEL"].ToString() != "4")
            {
                hdauthcode = ds.Tables[0].Rows[0]["ADEMPCODE"].ToString();
                hdauthname = ds.Tables[0].Rows[0]["EMPNAME"].ToString();
                hdauthemailid = ds.Tables[0].Rows[0]["EMAILID"].ToString();
                lbl_appauth = ds.Tables[0].Rows[0]["EMPNAME"].ToString() + "[" + ds.Tables[0].Rows[0]["ADEMPCODE"].ToString() + "]";
                hdauthlevel = ds.Tables[0].Rows[0]["AUTHLEVEL"].ToString();
            }
            else
            {
                hdauthlevel = "4";
                ViewBag.lbl_appauthVisiable = false;
                ViewBag.ddl_appauthVisiable = true;

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ddl_appauthList = ds.Tables[0].AsEnumerable()
                      .Select(row => new SelectListItem
                      {
                          Text = row["EMPLOYEE"].ToString(),
                          Value = row["ADEMPCODE"].ToString(),
                      }).ToList();
                }
                ddl_appauthList.Insert(0, new SelectListItem("-Select-", ""));
            }


            #endregion

            #region ddlMasterAgreement
            var dsGetMasterAgreement = _iomContractRepos.GETCONTRACTLIST(string.Empty, string.Empty, string.Empty, string.Empty, _userId, string.Empty);
            var ddlmstagreementList = new List<SelectListItem>();
            if (dsGetMasterAgreement != null && dsGetMasterAgreement.Tables.Count > 0 && dsGetMasterAgreement.Tables[0].Rows.Count > 0)
            {
                ddlmstagreementList = dsGetMasterAgreement.Tables[0].AsEnumerable()
                  .Select(row => new SelectListItem
                  {
                      Text = row["VENDORNAME"].ToString(),
                      Value = row["AGREEMENTHEADERID"].ToString()
                  }).ToList();
            }
            ddlmstagreementList.Insert(0, new SelectListItem
            {
                Text = "-Select Master Agreement-",
                Value = string.Empty
            });
            #endregion

            model = new IOMRequestFormViewModel
            {
                ddlFromList = ddlFromList,
                ddlfrom = OPHEADID,

                ddlcontractList = ddlcontractList,
                ddlmstagreementList = ddlmstagreementList,

                optDay = optDay,
                optMonth = optMonth,
                optYear = optYear,
                optED = optED,
                optEM = optEM,
                optEY = optEY,

                optDayList = optDayList,
                optMonthList = optMonthList,
                optYearList = optYearList,
                optEDList = optEDList,
                optEMList = optEMList,
                optEYList = optEYList,

                ddlreturndList = ddlreturndList,
                ddlreturn2dList = ddlreturn2dList,
                ddlreturnyList = ddlreturnyList,
                ddlreturn2yList = ddlreturn2yList,

                txtTo = "Legal & Secretariat",
                rdomaster = "0",

                #region approval authority binding
                hdauthcode = hdauthcode,
                hdauthname = hdauthname,
                hdauthemailid = hdauthemailid,
                lbl_appauth = lbl_appauth,
                hdauthlevel = hdauthlevel,
                ddl_appauthList = ddl_appauthList,
                #endregion

            };
            return View(model);
        }

        [HttpGet]
        public IActionResult IOMRequestForm_ddlcontract_OnChange(string ddlcontractValue)
        {
            DataSet ds = _iomContractRepos.GETCONTRACTMASTER("", "", "1", ddlcontractValue);
            object? response = null;

            if (ds.Tables[0].Rows[0]["TRANINGFIELED"].ToString() == "0")
            {
                response = new
                {
                    trt1_Visible = false,
                    trt2_Visible = false,
                    trt3_Visible = false,
                    trt4_Visible = false,
                    HDFTRINING_Value = "0",

                    txtcontactecode_Text = string.Empty,
                    txtnamesurety_Text = string.Empty,
                    txtsuretyamount_Text = string.Empty,
                    lblcontactname_Text = string.Empty
                };
            }
            else
            {
                response = new
                {
                    trt1_Visible = true,
                    trt2_Visible = true,
                    trt3_Visible = true,
                    trt4_Visible = true,
                    HDFTRINING_Value = "1"
                };
            }
            return Json(new
            {
                result = response
            });
        }

        [HttpGet]
        public IActionResult txtemployeecode_TextChanged(string txtcontactecode)
        {
            string lblcontactname_Text = string.Empty;
            string? showalertMsg = null;

            string empcode = txtcontactecode.Trim();
            if (!string.IsNullOrEmpty(empcode))
            {
                if (empcode.Length == 2)
                {
                    empcode = "0" + "0" + empcode;
                }
                else if (empcode.Length == 3)
                {
                    empcode = "0" + empcode;
                }
                DataSet objDs = _searchEmp.OfficialDetail(empcode);
                if (objDs.Tables[0].Rows.Count > 0)
                {
                    for (int Counter = 0; Counter < objDs.Tables[0].Rows.Count; Counter++)
                    {
                        lblcontactname_Text = objDs.Tables[0].Rows[Counter][1].ToString();
                    }
                }
                else
                {
                    showalertMsg = "Please enter Valid employee code";
                }
            }

            object response = new
            {
                showalertMsg,
                lblcontactname_Text,
            };

            return Json(new
            {
                result = response
            });
        }

        [HttpGet]
        public IActionResult IOMRequestForm_ddlmstagreement_OnChange(string selectedValue)
        {
            bool anch_mstagrfinaldoc_Visible = false;
            string? anch_mstagrfinaldoc_HRef = null;
            string? anch_mstagrfinaldoc_InnerText = null;

            string agreementid = selectedValue;
            if (!string.IsNullOrWhiteSpace(agreementid))
            {
                DataSet ds = _iomContractRepos.GETCONTRACTLIST("", "", "", "", _userId, agreementid);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    anch_mstagrfinaldoc_Visible = true;
                    anch_mstagrfinaldoc_HRef = "../../Uploads/IOM/" + ds.Tables[0].Rows[0]["FINALDOC"].ToString();
                    anch_mstagrfinaldoc_InnerText = ds.Tables[0].Rows[0]["FINALDOC"].ToString();
                }
            }

            object response = new
            {
                anch_mstagrfinaldoc_Visible,
                anch_mstagrfinaldoc_HRef,
                anch_mstagrfinaldoc_InnerText
            };

            return Json(new
            {
                result = response
            });
        }

        [HttpPost]
        public IActionResult IOMRequestFormSave([FromForm] IOMRequestFormSaveModel model)
        {
            var res = new IOMContractOperationResult();
            res = _iomContractService.Insert_IOMDetail(model);

            return Json(new
            {
                result = res
            });
        }

        [HttpPost]
        public IActionResult IOMRequestFormUpdate([FromForm] IOMRequestFormSaveModel model)
        {
            var res = new IOMContractOperationResult();
            res = _iomContractService.UPDATEIOMDETAIL(model);

            return Json(new
            {
                result = res
            });
        }





        #endregion

        #region ManageContract

        [HttpGet]
        public IActionResult ManageContract()
        {
            List<ManageContractViewModel> model = new();
            //Get IOM Pending Requests
            DataTable objiomdt = new DataTable();
            objiomdt = _iomContractRepos.GETIOMPENDINGREQUEST(_userId, "1,2,4,5,7,8,10,11,13,14,15,16,17,18,19,20,21,22,23,26");
            if (objiomdt != null && objiomdt.Rows.Count > 0)
            {
                model = objiomdt.AsEnumerable()
                       .Select(row => new ManageContractViewModel
                       {
                           IOMID =  row.Field<long>("IOMID"),
                           AGREEMENTTYPEDESC =  Convert.ToString(row.Field<object>("AGREEMENTTYPEDESC")),
                           CONTRACTTYPEDESC =  Convert.ToString(row.Field<object>("CONTRACTTYPEDESC")),
                           EFFECTIVEDATE = Convert.ToString(row.Field<object>("EFFECTIVEDATE")), 
                           EXPIRYDATE = Convert.ToString(row.Field<object>("EXPIRYDATE")), 
                           VENDORNAME =  Convert.ToString(row.Field<object>("VENDORNAME")),
                           STATUSNAME =  Convert.ToString(row.Field<object>("STATUSNAME")),
                           USERTYPE = Convert.ToString(row.Field<object>("USERTYPE")),
                           processstatusid = Convert.ToInt32(row.Field<object>("processstatusid"))
                       }).ToList();
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ViewIOMDetail([FromQuery] string IOMID)
        {
            ViewIOMDetail model = new();
            ViewBag.Prevcontractdiv = false;
            ViewBag.trrefdoc = false;
            ViewBag.trantibribery = true;
            ViewBag.trmstagr = false;
            ViewBag.trt1 = false;
            ViewBag.trt2 = false;
            ViewBag.trt3 = false;
            ViewBag.trt4 = false;

            DataSet ds = new DataSet();
            ds = _iomContractRepos.GetIOMFullDetail(IOMID, "");


            //Requestor Employee Details
            if (ds.Tables[0].Rows.Count > 0)
            {
                model.lbl_assname = ds.Tables[0].Rows[0]["ENAME"].ToString() + "[" + ds.Tables[0].Rows[0]["ADEMPCODE"].ToString() + "]";
                model.lbl_site = ds.Tables[0].Rows[0]["SITE"].ToString();
                model.lbl_op = ds.Tables[0].Rows[0]["OPERATION"].ToString();
                model.lbl_dept = ds.Tables[0].Rows[0]["DEPARTMENT"].ToString();
                model.lbl_div = ds.Tables[0].Rows[0]["DIVISION"].ToString();
                model.lbl_sec = ds.Tables[0].Rows[0]["SECTION"].ToString();
            }

            //IOM Details
            if (ds.Tables[1].Rows.Count > 0)
            {
                model.lblUNIQUEID = ds.Tables[1].Rows[0]["UNIQUEID"] == null ? "" : ds.Tables[1].Rows[0]["UNIQUEID"].ToString();
                model.lblfrom = ds.Tables[1].Rows[0]["IOMFROM"].ToString();
                model.lblto = "Legal & Secretariat";
                model.lblagreementtype = ds.Tables[1].Rows[0]["AGREEMENTTYPEDESC"].ToString();
                model.lblcontracttype = ds.Tables[1].Rows[0]["CONTRACTTYPEDESC"].ToString();
                model.lbleffdate = ds.Tables[1].Rows[0]["EFFECTIVEDATE"].ToString();
                model.lblexpdate = ds.Tables[1].Rows[0]["EXPIRYDATE"].ToString();
                if (ds.Tables[1].Rows[0]["TERMSYEAR"].ToString() == "100")
                {
                    model.lblterm = "Lifetime";
                }
                else
                {
                    model.lblterm = ds.Tables[1].Rows[0]["TERMSYEAR"].ToString() + " Year, " + ds.Tables[1].Rows[0]["TERMSMONTH"].ToString() + " Month";
                }
                model.lblvendorname = ds.Tables[1].Rows[0]["VENDORNAME"].ToString();
                model.lblmannerofpayment = ds.Tables[1].Rows[0]["MANNEROFPAYMENT"].ToString();
                model.Iblconsiderable = ds.Tables[1].Rows[0]["CONSIDERABLE"].ToString();
                model.lblpurpose = ds.Tables[1].Rows[0]["PURPOSE"].ToString();
                model.lblremarks = ds.Tables[1].Rows[0]["REMARKS"].ToString();
                if (string.IsNullOrEmpty(ds.Tables[1].Rows[0]["AGREEMENTHEADERID"].ToString()))
                {
                    ViewBag.Prevcontractdiv = false;
                }
                else
                {
                    ViewBag.Prevcontractdiv = true;
                    model.lblagreementid = ds.Tables[1].Rows[0]["AGREEMENTHEADERID"].ToString();
                    model.lblprevagrperiod = ds.Tables[1].Rows[0]["AGRPERIOD"].ToString();
                    model.prevappnotelink = "../../Uploads/IOM/" + ds.Tables[1].Rows[0]["PREVAPPNOTE"].ToString();
                    model.prevappnotelinkText = ds.Tables[1].Rows[0]["PREVAPPNOTE"].ToString();
                    model.prevfinaldoclink = "../../Uploads/IOM/" + ds.Tables[1].Rows[0]["PREVFINALDOC"].ToString();
                    model.prevfinaldoclink = ds.Tables[1].Rows[0]["PREVFINALDOC"].ToString();
                }
                //Get Approval Note
                string referencedoc = ds.Tables[1].Rows[0]["REFERENCEDOC"].ToString();
                if (string.IsNullOrEmpty(referencedoc.Trim()))
                {
                    ViewBag.trrefdoc = false;
                }
                else
                {
                    ViewBag.trrefdoc = true;
                    model.agreementlink = "../../Uploads/IOM/" + referencedoc;
                    model.agreementlinkText = referencedoc;
                }

                //Get Anti Bribery
                string antibribery = ds.Tables[1].Rows[0]["ANTIBRIBERY"].ToString();
                model.antibriberylink = "../../Uploads/IOM/" + antibribery;
                model.antibriberylinkText = antibribery;

                //Get NDA Document
                string ndadocument = ds.Tables[1].Rows[0]["NDADOCUMENT"].ToString();
                //ndadocumentlink.HRef = "../../Uploads/IOM/" + ndadocument;
                //ndadocumentlink.InnerText = ndadocument;

                //Show/Hide Anti Bribery and NDA Document
                if (antibribery == "")
                {
                    ViewBag.trantibribery = false;
                }

                //Master agreement
                if (ds.Tables[1].Rows[0]["MASTERAGREEMENT"].ToString().Trim() != "0")
                {
                    ViewBag.trmstagr = true;
                    model.lblmstagr = ds.Tables[1].Rows[0]["MSTAGR"].ToString();
                }
            }

            if (ds.Tables[2].Rows.Count > 0)
            {
                //Get IOM History 
                var grdApprovalDetails = ds.Tables[2];
                var grdApprovalDetailsList = new List<grdApprovalDetails>();
                if (grdApprovalDetails != null && grdApprovalDetails.Rows.Count > 0)
                {
                    grdApprovalDetailsList = grdApprovalDetails.AsEnumerable()
                           .Select(row => new grdApprovalDetails
                           {
                               ADDEDDATE = Convert.ToString(row.Field<object>("ADDEDDATE")),
                               ADDEDBY = Convert.ToString(row.Field<object>("ADDEDBY")),
                               STATUSNAME = Convert.ToString(row.Field<object>("STATUSNAME")),
                               REMARKS = Convert.ToString(row.Field<object>("REMARKS")),
                               ATTACHMENTPATH = Convert.ToString(row.Field<object>("ATTACHMENTPATH"))
                           }).ToList();
                }
                model.grdApprovalDetails = grdApprovalDetailsList;
            }

            //Get Communication History
            if (ds.Tables[4].Rows.Count > 0)
            {
                var grdCommunicationHistory = ds.Tables[4];

                var grdCommunicationHistoryList = new List<grdCommunicationHistory>();
                if (grdCommunicationHistory != null && grdCommunicationHistory.Rows.Count > 0)
                {
                    grdCommunicationHistoryList = grdCommunicationHistory.AsEnumerable()
                           .Select(row => new grdCommunicationHistory
                           {
                               ADDEDDATE = Convert.ToString(row.Field<object>("ADDEDDATE")),
                               ADDEDBY = Convert.ToString(row.Field<object>("ADDEDBY")),
                               USERTYPE = Convert.ToString(row.Field<object>("USERTYPE")),
                               REMARKS = Convert.ToString(row.Field<object>("REMARKS")),
                               ATTACHMENT = Convert.ToString(row.Field<object>("ATTACHMENT"))
                           }).ToList();
                }
                model.grdCommunicationHistory = grdCommunicationHistoryList;
            }

            //FOREIGN TRAINING
            if (ds.Tables[1].Rows[0]["TRANINGFIELED"].ToString() == "0")
            {
                ViewBag.trt1 = false;
                ViewBag.trt2 = false;
                ViewBag.trt3 = false;
                ViewBag.trt4 = false;
            }
            else
            {
                ViewBag.trt1 = true;
                ViewBag.trt2 = true;
                ViewBag.trt3 = true;
                ViewBag.trt4 = true;
                model.lblcontractcode = ds.Tables[1].Rows[0]["CONTRACTECODE"].ToString();
                model.lblcontractname = ds.Tables[1].Rows[0]["CONTRACTNAME"].ToString();
                model.lblsuretyamount = ds.Tables[1].Rows[0]["SURETYAMOUNT"].ToString();
                model.lblnameofsurety = ds.Tables[1].Rows[0]["NAMEOFSURETY"].ToString();
                model.lblretureddateF = ds.Tables[1].Rows[0]["RETURNFROMDATE"].ToString();
                model.lblretureddateT = ds.Tables[1].Rows[0]["RETURNTODATE"].ToString();
            }


            return View(model);
        }



        #endregion

        #region Manage Approval

        [HttpGet]
        public IActionResult ManageIOMApproval()
        {
            List<ManageIOMApprovalViewModel> model = new();
            DataTable dt = new DataTable();
            dt = _iomContractRepos.GET_MANAGEIOMDETAILS(_userId);
            if (dt != null && dt.Rows.Count > 0)
            {
                model = dt.AsEnumerable()
                       .Select(row => new ManageIOMApprovalViewModel
                       {
                           IOMID = row.Field<long>("IOMID"),
                           AGREEMENTTYPEDESC = Convert.ToString(row.Field<object>("AGREEMENTTYPEDESC")),
                           CONTRACTTYPEDESC = Convert.ToString(row.Field<object>("CONTRACTTYPEDESC")),
                           EFFECTIVEDATE = Convert.ToString(row.Field<object>("EFFECTIVEDATE")),
                           EXPIRYDATE = Convert.ToString(row.Field<object>("EXPIRYDATE")),
                           VENDORNAME = Convert.ToString(row.Field<object>("VENDORNAME")),
                           STATUSNAME = Convert.ToString(row.Field<object>("STATUSNAME")),
                           REQUESTDBY = Convert.ToString(row.Field<object>("REQUESTDBY")),
                           EMPDESIGNATION = Convert.ToString(row.Field<object>("EMPDESIGNATION"))
                       }).ToList();
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult IOMApprovalForm([FromQuery] string IOMID, [FromQuery] string DESIGN)
        {
            IOMApprovalFormViewModel model = new();
            ViewBag.encdesign = DESIGN;

            ViewBag.tblprevcontractVisiable = true;
            ViewBag.tblPrevContHistoryVisiable = true;
            ViewBag.trreferencedocVisiable = true;
            ViewBag.trantibriberyVisiable = true;
            ViewBag.trbackdateremarkVisiable = true;
            ViewBag.tr_ackVisiable = true;
            ViewBag.trmstagrVisiable = false;
            ViewBag.lbl_appauthVisiable = false;
            ViewBag.ddl_appauthVisiable = false;

            DataSet ds = new DataSet();
            string IOM_ID = WebUtility.UrlDecode(Encryption.Decrypt(IOMID));
            string Ecode = _userId;

            ds = _iomContractRepos.GetIOMFullDetail(IOM_ID, Ecode);

            //Requestor Employee Details
            if (ds.Tables[0].Rows.Count > 0)
            {
                model.hdreqname = ds.Tables[0].Rows[0]["ENAME"].ToString();
                model.hdreqcode = ds.Tables[0].Rows[0]["ADEMPCODE"].ToString();
                model.hdreqemail = ds.Tables[0].Rows[0]["EMAILID"].ToString();
                model.lbl_assname = ds.Tables[0].Rows[0]["ENAME"].ToString() + "[" + ds.Tables[0].Rows[0]["ADEMPCODE"].ToString() + "]";
                model.lbl_site = ds.Tables[0].Rows[0]["SITE"].ToString();
                model.lbl_op = ds.Tables[0].Rows[0]["OPERATION"].ToString();
                model.lbl_dept = ds.Tables[0].Rows[0]["DEPARTMENT"].ToString();
                model.lbl_div = ds.Tables[0].Rows[0]["DIVISION"].ToString();
                model.lbl_sec = ds.Tables[0].Rows[0]["SECTION"].ToString();
            }

            //IOM Details
            if (ds.Tables[1].Rows.Count > 0)
            {
                model.hdiomid = ds.Tables[1].Rows[0]["IOMID"].ToString();
                model.lblfrom = ds.Tables[1].Rows[0]["IOMFROM"].ToString();
                model.lblto = "Legal & Secretariat";
                model.lblagreementtype = ds.Tables[1].Rows[0]["AGREEMENTTYPEDESC"].ToString();
                model.lblcontracttype = ds.Tables[1].Rows[0]["CONTRACTTYPEDESC"].ToString();
                model.lbleffdate = ds.Tables[1].Rows[0]["EFFECTIVEDATE"].ToString();
                model.lblexpdate = ds.Tables[1].Rows[0]["EXPIRYDATE"].ToString();
                if (ds.Tables[1].Rows[0]["TERMSYEAR"].ToString() == "100")
                {
                    model.lblterm = "Lifetime";
                }
                else
                {
                    model.lblterm = ds.Tables[1].Rows[0]["TERMSYEAR"].ToString() + " Year, " + ds.Tables[1].Rows[0]["TERMSMONTH"].ToString() + " Month";
                }
                model.lblvendorname = ds.Tables[1].Rows[0]["VENDORNAME"].ToString();
                model.lblmannerofpayment = ds.Tables[1].Rows[0]["MANNEROFPAYMENT"].ToString();
                model.lblpurpose = ds.Tables[1].Rows[0]["PURPOSE"].ToString();
                model.lblremarks = ds.Tables[1].Rows[0]["REMARKS"].ToString();
                model.lblconsiderable = ds.Tables[1].Rows[0]["CONSIDERABLE"].ToString();
                model.hdbackdate = ds.Tables[1].Rows[0]["Backdate"].ToString();
                model.hddelaydate = ds.Tables[1].Rows[0]["DELAYDATE"].ToString();
                //Get Previous Contract Detail
                if (string.IsNullOrEmpty(ds.Tables[1].Rows[0]["AGREEMENTHEADERID"].ToString().Trim()))
                {
                    ViewBag.tblprevcontractVisiable = false;
                    ViewBag.tblPrevContHistoryVisiable = false;
                }
                else
                {
                    ViewBag.tblprevcontractVisiable = true;
                    ViewBag.tblPrevContHistoryVisiable = true;

                    model.lblagreementid = ds.Tables[1].Rows[0]["AGREEMENTHEADERID"].ToString();
                    model.lblprevagrperiod = ds.Tables[1].Rows[0]["AGRPERIOD"].ToString();

                    model.prevappnotelink = "../../Uploads/IOM/" + ds.Tables[1].Rows[0]["PREVAPPNOTE"].ToString();
                    model.prevappnotelinkText = ds.Tables[1].Rows[0]["PREVAPPNOTE"].ToString();
                    model.prevfinaldoclink = "../../Uploads/IOM/" + ds.Tables[1].Rows[0]["PREVFINALDOC"].ToString();
                    model.prevfinaldoclinkText = ds.Tables[1].Rows[0]["PREVFINALDOC"].ToString();

                    if (ds.Tables[1].Rows[0]["AGREEMENTTYPEDESC"].ToString() == "Amendment" || ds.Tables[1].Rows[0]["AGREEMENTTYPEDESC"].ToString() == "Renewal")
                    {
                        DataSet dss = _iomContractRepos.GETIOMBYAGREEMENTID(ds.Tables[1].Rows[0]["AGREEMENTHEADERID"].ToString());
                        if (dss.Tables[0].Rows.Count > 0)
                        {

                            ViewBag.tblPrevContHistoryVisiable = true;

                            var grdPrivContractHistory = dss.Tables[0];
                            var grdApprovalDetailsList = new List<grdPrivContractHistory>();
                            if (grdPrivContractHistory != null && grdPrivContractHistory.Rows.Count > 0)
                            {
                                grdApprovalDetailsList = grdPrivContractHistory.AsEnumerable()
                                       .Where(x => x.Field<Int64>("IOMID") != Convert.ToInt64(IOM_ID))
                                       .Select(row => new grdPrivContractHistory
                                       {
                                           IOMID = Convert.ToInt64(row.Field<object>("IOMID")),
                                           AGREEMENTTYPEDESC = Convert.ToString(row.Field<object>("AGREEMENTTYPEDESC")),
                                           VENDORNAME = Convert.ToString(row.Field<object>("VENDORNAME")),
                                           EFFECTIVEDATE = Convert.ToString(row.Field<object>("EFFECTIVEDATE")),
                                           EXPIRYDATE = Convert.ToString(row.Field<object>("EXPIRYDATE")),
                                           processstatusid = Convert.ToInt32(row.Field<object>("processstatusid")),
                                           STATUSNAME = Convert.ToString(row.Field<object>("STATUSNAME")),
                                           REQUESTDBY = Convert.ToString(row.Field<object>("REQUESTDBY")),
                                           finaldoc = Convert.ToString(row.Field<object>("finaldoc"))
                                       }).ToList();
                            }
                            model.grdPrivContractHistory = grdApprovalDetailsList;
                        }
                        else
                        {
                            ViewBag.tblPrevContHistoryVisiable = false;
                        }
                    }
                    else
                    {
                        ViewBag.tblPrevContHistoryVisiable = false;
                    }

                }
                //Get Approval Note
                string referencedoc = ds.Tables[1].Rows[0]["REFERENCEDOC"].ToString();
                if (string.IsNullOrEmpty(referencedoc.Trim()))
                {
                    ViewBag.trreferencedocVisiable = false;
                }
                else
                {
                    ViewBag.trreferencedocVisiable = true;

                    model.agreementlink = "../../Uploads/IOM/" + referencedoc;
                    model.agreementlinkText = referencedoc;
                }

                //Get Anti Bribery
                string antibribery = ds.Tables[1].Rows[0]["ANTIBRIBERY"].ToString();
                model.antibriberylink = "../../Uploads/IOM/" + antibribery;
                model.antibriberylinkText = antibribery;

                ////Get NDA Document
                //string ndadocument = ds.Tables[1].Rows[0]["NDADOCUMENT"].ToString();
                //ndadocumentlink.HRef = "../../Uploads/IOM/" + ndadocument;
                //ndadocumentlink.InnerText = ndadocument;

                //Show/Hide Anti Bribery and NDA Document
                if (antibribery.Trim() == "")
                {
                    ViewBag.trantibriberyVisiable = false;
                }
                //if (ndadocument.Trim() == "")
                //{
                //   trndadocument.Visible = false;
                //}
                string EMPDESIGNATION = WebUtility.UrlDecode(Encryption.Decrypt(Convert.ToString(DESIGN)));
                int backdate = Convert.ToInt32(model.hdbackdate);
                if ((backdate < 0) && (EMPDESIGNATION == "4"))
                {
                    ViewBag.trbackdateremarkVisiable = true;
                }
                else
                {
                    ViewBag.trbackdateremarkVisiable = false;
                }

                int delaydate = Convert.ToInt32(model.hddelaydate);
                if ((delaydate <= 0))
                {
                    ViewBag.tr_ackVisiable = true;
                }
                else
                {
                    ViewBag.tr_ackVisiable = false;
                }
                //Master agreement
                if (ds.Tables[1].Rows[0]["MASTERAGREEMENT"].ToString().Trim() != "0")
                {
                    ViewBag.trmstagrVisiable = true;
                    model.lblmstagr = ds.Tables[1].Rows[0]["MSTAGR"].ToString();
                }
            }

            //IOM History 
            if (ds.Tables[2].Rows.Count > 0)
            {
                var grdApprovalDetails = ds.Tables[2];
                var grdApprovalDetailsList = new List<grdApprovalDetails>();
                if (grdApprovalDetails != null && grdApprovalDetails.Rows.Count > 0)
                {
                    grdApprovalDetailsList = grdApprovalDetails.AsEnumerable()
                           .Select(row => new grdApprovalDetails
                           {
                               ADDEDDATE = Convert.ToString(row.Field<object>("ADDEDDATE")),
                               ADDEDBY = Convert.ToString(row.Field<object>("ADDEDBY")),
                               STATUSNAME = Convert.ToString(row.Field<object>("STATUSNAME")),
                               REMARKS = Convert.ToString(row.Field<object>("REMARKS")),
                               ATTACHMENTPATH = Convert.ToString(row.Field<object>("ATTACHMENTPATH"))
                           }).ToList();
                }
                model.grdApprovalDetails = grdApprovalDetailsList;

            }

            //FOREIGN TRAINING
            if (ds.Tables[1].Rows[0]["TRANINGFIELED"].ToString() == "0")
            {
                ViewBag.trt1Visiable = false;
                ViewBag.trt2Visiable = false;
                ViewBag.trt3Visiable = false;
                ViewBag.trt4Visiable = false;
            }
            else
            {
                ViewBag.trt1Visiable = true;
                ViewBag.trt2Visiable = true;
                ViewBag.trt3Visiable = true;
                ViewBag.trt4Visiable = true;
                model.lblcontractcode = ds.Tables[1].Rows[0]["CONTRACTECODE"].ToString();
                model.lblcontractname = ds.Tables[1].Rows[0]["CONTRACTNAME"].ToString();
                model.lblsuretyamount = ds.Tables[1].Rows[0]["SURETYAMOUNT"].ToString();
                model.lblnameofsurety = ds.Tables[1].Rows[0]["NAMEOFSURETY"].ToString();
                model.lblretureddateF = ds.Tables[1].Rows[0]["RETURNFROMDATE"].ToString();
                model.lblretureddateT = ds.Tables[1].Rows[0]["RETURNTODATE"].ToString();
            }

            //Next Approval Detail
            if (ds.Tables[3].Rows.Count > 0)
            {
                if (ds.Tables[3].Rows[0]["AUTHLEVEL"].ToString() != "4")
                {
                    model.lbl_appauth = ds.Tables[3].Rows[0]["EMPNAME"].ToString() + "[" + ds.Tables[3].Rows[0]["ADEMPCODE"].ToString() + "]";
                    model.hdauthname = ds.Tables[3].Rows[0]["EMPNAME"].ToString();
                    model.hdauthemailid = ds.Tables[3].Rows[0]["EMAILID"].ToString();
                    model.hdauthlevel = ds.Tables[3].Rows[0]["AUTHLEVEL"].ToString();
                }
                else
                {
                    model.hdauthlevel = "4";

                    ViewBag.lbl_appauthVisiable = false;
                    ViewBag.ddl_appauthVisiable = true;

                    var ddl_appauth = ds.Tables[3];
                    var ddlFromList = new List<SelectListItem>();

                    if (ddl_appauth != null && ddl_appauth.Rows.Count > 0)
                    {
                        ddlFromList = ddl_appauth.AsEnumerable()
                          .Select(row => new SelectListItem
                          {
                              Text = row["EMPLOYEE"].ToString(),
                              Value = row["ADEMPCODE"].ToString(),
                          }).ToList();
                    }
                    ddlFromList.Insert(0, new SelectListItem
                    {
                        Text = "-Select-",
                        Value = ""
                    });
                    model.ddl_appauthList = ddlFromList;
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult IOMApprovalFormSave([FromForm] IOMApprovalFormSave model)
        {
            var res = new IOMContractOperationResult();
            res = _iomContractService.IOMApprovalFormSave(model);

            return Json(new
            {
                result = res
            });
        }


        [HttpGet]
        public IActionResult SubmitFinalDocument([FromQuery] string IOMID)
        {
            SubmitFinalDocumentViewModel model = new();
            string IOM_ID = WebUtility.UrlDecode(Encryption.Decrypt(IOMID));

            DataSet ds = new DataSet();
            ds = _iomContractRepos.GetIOMFullDetail(IOM_ID, "");

            //Requestor Employee Details
            if (ds.Tables[0].Rows.Count > 0)
            {
                model.lbl_assname = ds.Tables[0].Rows[0]["ENAME"].ToString() + "[" + ds.Tables[0].Rows[0]["ADEMPCODE"].ToString() + "]";
                model.lbl_site = ds.Tables[0].Rows[0]["SITE"].ToString();
                model.lbl_op = ds.Tables[0].Rows[0]["OPERATION"].ToString();
                model.lbl_dept = ds.Tables[0].Rows[0]["DEPARTMENT"].ToString();
                model.lbl_div = ds.Tables[0].Rows[0]["DIVISION"].ToString();
                model.lbl_sec = ds.Tables[0].Rows[0]["SECTION"].ToString();
            }

            //IOM Details
            if (ds.Tables[1].Rows.Count > 0)
            {
                model.hdiomid = ds.Tables[1].Rows[0]["IOMID"].ToString();
                model.lblfrom = ds.Tables[1].Rows[0]["IOMFROM"].ToString();
                model.lblto = "Legal & Secretariat";
                model.lblagreementtype = ds.Tables[1].Rows[0]["AGREEMENTTYPEDESC"].ToString();
                model.lblcontracttype = ds.Tables[1].Rows[0]["CONTRACTTYPEDESC"].ToString();
                model.lbleffdate = ds.Tables[1].Rows[0]["EFFECTIVEDATE"].ToString();
                model.lblexpdate = ds.Tables[1].Rows[0]["EXPIRYDATE"].ToString();
                if (ds.Tables[1].Rows[0]["TERMSYEAR"].ToString() == "100")
                {
                    model.lblterm = "Lifetime";
                }
                else
                {
                    model.lblterm = ds.Tables[1].Rows[0]["TERMSYEAR"].ToString() + " Year, " + ds.Tables[1].Rows[0]["TERMSMONTH"].ToString() + " Month";
                }
                model.lblvendorname = ds.Tables[1].Rows[0]["VENDORNAME"].ToString();
                model.lblmannerofpayment = ds.Tables[1].Rows[0]["MANNEROFPAYMENT"].ToString();
                model.Iblconsiderable = ds.Tables[1].Rows[0]["CONSIDERABLE"].ToString();
                model.lblpurpose = ds.Tables[1].Rows[0]["PURPOSE"].ToString();
                model.lblremarks = ds.Tables[1].Rows[0]["REMARKS"].ToString();

                //Previous Contract Details
                if (string.IsNullOrEmpty(ds.Tables[1].Rows[0]["AGREEMENTHEADERID"].ToString()))
                {
                    ViewBag.PrevcontractdivVisible = false;
                }
                else
                {
                    ViewBag.PrevcontractdivVisible = true;
                    model.lblagreementid = ds.Tables[1].Rows[0]["AGREEMENTHEADERID"].ToString();
                    model.lblprevagrperiod = ds.Tables[1].Rows[0]["AGRPERIOD"].ToString();
                    model.prevappnotelink = "../../Uploads/IOM/" + ds.Tables[1].Rows[0]["PREVAPPNOTE"].ToString();
                    model.prevappnotelinkText = ds.Tables[1].Rows[0]["PREVAPPNOTE"].ToString();
                    model.prevfinaldoclink = "../../Uploads/IOM/" + ds.Tables[1].Rows[0]["PREVFINALDOC"].ToString();
                    model.prevfinaldoclinkText = ds.Tables[1].Rows[0]["PREVFINALDOC"].ToString();
                }

                //Get Approval Note
                string referencedoc = ds.Tables[1].Rows[0]["REFERENCEDOC"].ToString();
                if (string.IsNullOrEmpty(referencedoc.Trim()))
                {
                    ViewBag.trreferencedocVisible = false;
                }
                else
                {
                    ViewBag.trreferencedocVisible = true;
                    model.agreementlink = "../../Uploads/IOM/" + referencedoc;
                    model.agreementlinkText = referencedoc;
                }
                //Get Anti Bribery
                string antibribery = ds.Tables[1].Rows[0]["ANTIBRIBERY"].ToString();
                model.antibriberylink = "../../Uploads/IOM/" + antibribery;
                model.antibriberylinkText = antibribery;

                //Get NDA Document
                string ndadocument = ds.Tables[1].Rows[0]["NDADOCUMENT"].ToString();
                //ndadocumentlink.HRef = "../../Uploads/IOM/" + ndadocument;
                //ndadocumentlink.InnerText = ndadocument;

                //Show/Hide Anti Bribery and NDA Document
                if (antibribery == "")
                {
                    ViewBag.trantibriberyVisible = false;
                    //trndadocument.Visible = false;
                }

                //Master agreement
                if (!string.IsNullOrEmpty(ds.Tables[1].Rows[0]["MSTAGR"].ToString()))
                {
                    ViewBag.trmstagrVisible = true;
                    model.lblmstagr = ds.Tables[1].Rows[0]["MSTAGR"].ToString() + " [" + ds.Tables[1].Rows[0]["MASTERAGREEMENT"].ToString() + "]";
                }
            }

            //IOM History 
            if (ds.Tables[2].Rows.Count > 0)
            {

                //Get IOM History 
                var grdApprovalDetails = ds.Tables[2];
                var grdApprovalDetailsList = new List<grdApprovalDetails>();
                if (grdApprovalDetails != null && grdApprovalDetails.Rows.Count > 0)
                {
                    grdApprovalDetailsList = grdApprovalDetails.AsEnumerable()
                           .Select(row => new grdApprovalDetails
                           {
                               ADDEDDATE = Convert.ToString(row.Field<object>("ADDEDDATE")),
                               ADDEDBY = Convert.ToString(row.Field<object>("ADDEDBY")),
                               STATUSNAME = Convert.ToString(row.Field<object>("STATUSNAME")),
                               REMARKS = Convert.ToString(row.Field<object>("REMARKS")),
                               ATTACHMENTPATH = Convert.ToString(row.Field<object>("ATTACHMENTPATH"))
                           }).ToList();
                }
                model.grdApprovalDetails = grdApprovalDetailsList;
            }
            //FOREIGN TRAINING
            if (ds.Tables[1].Rows[0]["TRANINGFIELED"].ToString() == "0")
            {
                ViewBag.trt1Visible = false;
                ViewBag.trt2Visible = false;
                ViewBag.trt3Visible = false;
                ViewBag.trt4Visible = false;
            }
            else
            {
                ViewBag.trt1Visible = true;
                ViewBag.trt2Visible = true;
                ViewBag.trt3Visible = true;
                ViewBag.trt4Visible = true;
                model.lblcontractcode = ds.Tables[1].Rows[0]["CONTRACTECODE"].ToString();
                model.lblcontractname = ds.Tables[1].Rows[0]["CONTRACTNAME"].ToString();
                model.lblsuretyamount = ds.Tables[1].Rows[0]["SURETYAMOUNT"].ToString();
                model.lblnameofsurety = ds.Tables[1].Rows[0]["NAMEOFSURETY"].ToString();
                model.lblretureddateF = ds.Tables[1].Rows[0]["RETURNFROMDATE"].ToString();
                model.lblretureddateT = ds.Tables[1].Rows[0]["RETURNTODATE"].ToString();
            }


            return View(model);
        }

        [HttpPost]
        public IActionResult SubmitFinalDocumentSave([FromForm] SubmitFinalDocumentSave model)
        {
            var res = new IOMContractOperationResult();
            res = _iomContractService.SubmitFinalDocumentSave(model);

            return Json(new
            {
                result = res
            });
        }

        [HttpGet]
        public IActionResult UserAcknowledgement([FromQuery] string IOMID)
        {
            UserAcknowledgementViewModel model = new();
            string IOM_ID = WebUtility.UrlDecode(Encryption.Decrypt(IOMID));


            DataSet ds = new DataSet();
            ds = _iomContractRepos.GetIOMFullDetail(IOM_ID, "");

            //Requestor Employee Details
            if (ds.Tables[0].Rows.Count > 0)
            {
                model.lbl_assname = ds.Tables[0].Rows[0]["ENAME"].ToString() + "[" + ds.Tables[0].Rows[0]["ADEMPCODE"].ToString() + "]";
                model.lbl_site = ds.Tables[0].Rows[0]["SITE"].ToString();
                model.lbl_op = ds.Tables[0].Rows[0]["OPERATION"].ToString();
                model.lbl_dept = ds.Tables[0].Rows[0]["DEPARTMENT"].ToString();
                model.lbl_div = ds.Tables[0].Rows[0]["DIVISION"].ToString();
                model.lbl_sec = ds.Tables[0].Rows[0]["SECTION"].ToString();
            }

            //IOM Details
            if (ds.Tables[1].Rows.Count > 0)
            {
                model.hdiomid = ds.Tables[1].Rows[0]["IOMID"].ToString();
                model.lblUNIQUEID = ds.Tables[1].Rows[0]["UNIQUEID"] == null ? "" : ds.Tables[1].Rows[0]["UNIQUEID"].ToString();
                model.lblfrom = ds.Tables[1].Rows[0]["IOMFROM"].ToString();
                model.lblto = "Legal & Secretariat";
                model.lblagreementtype = ds.Tables[1].Rows[0]["AGREEMENTTYPEDESC"].ToString();
                model.lblcontracttype = ds.Tables[1].Rows[0]["CONTRACTTYPEDESC"].ToString();
                model.lbleffdate = ds.Tables[1].Rows[0]["EFFECTIVEDATE"].ToString();
                model.lblexpdate = ds.Tables[1].Rows[0]["EXPIRYDATE"].ToString();
                if (ds.Tables[1].Rows[0]["TERMSYEAR"].ToString() == "100")
                {
                    model.lblterm = "Lifetime";
                }
                else
                {
                    model.lblterm = ds.Tables[1].Rows[0]["TERMSYEAR"].ToString() + " Year, " + ds.Tables[1].Rows[0]["TERMSMONTH"].ToString() + " Month";
                }
                model.lblvendorname = ds.Tables[1].Rows[0]["VENDORNAME"].ToString();
                model.lblmannerofpayment = ds.Tables[1].Rows[0]["MANNEROFPAYMENT"].ToString();
                model.Iblconsiderable = ds.Tables[1].Rows[0]["CONSIDERABLE"].ToString();
                model.lblpurpose = ds.Tables[1].Rows[0]["PURPOSE"].ToString();
                model.lblremarks = ds.Tables[1].Rows[0]["REMARKS"].ToString();
                //Previous Contract Details
                if (string.IsNullOrEmpty(ds.Tables[1].Rows[0]["AGREEMENTHEADERID"].ToString()))
                {
                    ViewBag.PrevcontractdivVisible = false;
                }
                else
                {
                    ViewBag.PrevcontractdivVisible = true;
                    model.lblagreementid = ds.Tables[1].Rows[0]["AGREEMENTHEADERID"].ToString();
                    model.lblprevagrperiod = ds.Tables[1].Rows[0]["AGRPERIOD"].ToString();
                    model.prevappnotelink = "../../Uploads/IOM/" + ds.Tables[1].Rows[0]["PREVAPPNOTE"].ToString();
                    model.prevappnotelinkText = ds.Tables[1].Rows[0]["PREVAPPNOTE"].ToString();
                    model.prevfinaldoclink = "../../Uploads/IOM/" + ds.Tables[1].Rows[0]["PREVFINALDOC"].ToString();
                    model.prevfinaldoclinkText = ds.Tables[1].Rows[0]["PREVFINALDOC"].ToString();
                }
                //Get Approval Note
                string referencedoc = ds.Tables[1].Rows[0]["REFERENCEDOC"].ToString();
                if (string.IsNullOrEmpty(referencedoc.Trim()))
                {
                    ViewBag.trreferencedocVisible = false;
                }
                else
                {
                    ViewBag.trreferencedocVisible = true;
                    model.agreementlink = "../../Uploads/IOM/" + referencedoc;
                    model.agreementlinkText = referencedoc;
                }

                //Get Anti Bribery
                string antibribery = ds.Tables[1].Rows[0]["ANTIBRIBERY"].ToString();
                model.antibriberylink = "../../Uploads/IOM/" + antibribery;
                model.antibriberylinkText = antibribery;

                ////Get NDA Document
                //string ndadocument = ds.Tables[1].Rows[0]["NDADOCUMENT"].ToString();
                //ndadocumentlink.HRef = "../../Uploads/IOM/" + ndadocument;
                //ndadocumentlink.InnerText = ndadocument;

                //Show/Hide Anti Bribery and NDA Document
                if (antibribery.Trim() == "")
                {
                    ViewBag.trantibriberyVisible = false;
                    //trndadocument.Visible = false;
                }

                //Master agreement
                if (ds.Tables[1].Rows[0]["MASTERAGREEMENT"].ToString() != "0")
                {
                    ViewBag.trmstagrVisible = true;
                    model.lblmstagr = ds.Tables[1].Rows[0]["MSTAGR"].ToString();
                }
            }


            //IOM History 
            if (ds.Tables[2].Rows.Count > 0)
            {
                //Get IOM History 
                var grdApprovalDetails = ds.Tables[2];
                var grdApprovalDetailsList = new List<grdApprovalDetails>();
                if (grdApprovalDetails != null && grdApprovalDetails.Rows.Count > 0)
                {
                    grdApprovalDetailsList = grdApprovalDetails.AsEnumerable()
                           .Select(row => new grdApprovalDetails
                           {
                               ADDEDDATE = Convert.ToString(row.Field<object>("ADDEDDATE")),
                               ADDEDBY = Convert.ToString(row.Field<object>("ADDEDBY")),
                               STATUSNAME = Convert.ToString(row.Field<object>("STATUSNAME")),
                               REMARKS = Convert.ToString(row.Field<object>("REMARKS")),
                               ATTACHMENTPATH = Convert.ToString(row.Field<object>("ATTACHMENTPATH"))
                           }).ToList();
                }
                model.grdApprovalDetails = grdApprovalDetailsList;
            }
            //FOREIGN TRAINING
            if (ds.Tables[1].Rows[0]["TRANINGFIELED"].ToString() == "0")
            {
                ViewBag.trt1Visible = false;
                ViewBag.trt2Visible = false;
                ViewBag.trt3Visible = false;
                ViewBag.trt4Visible = false;
            }
            else
            {
                ViewBag.trt1Visible = true;
                ViewBag.trt2Visible = true;
                ViewBag.trt3Visible = true;
                ViewBag.trt4Visible = true;
                model.lblcontractcode = ds.Tables[1].Rows[0]["CONTRACTECODE"].ToString();
                model.lblcontractname = ds.Tables[1].Rows[0]["CONTRACTNAME"].ToString();
                model.lblsuretyamount = ds.Tables[1].Rows[0]["SURETYAMOUNT"].ToString();
                model.lblnameofsurety = ds.Tables[1].Rows[0]["NAMEOFSURETY"].ToString();
                model.lblretureddateF = ds.Tables[1].Rows[0]["RETURNFROMDATE"].ToString();
                model.lblretureddateT = ds.Tables[1].Rows[0]["RETURNTODATE"].ToString();
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult UserAcknowledgementSave([FromForm] UserAcknowledgementSave model)
        {
            var res = new IOMContractOperationResult();
            res = _iomContractService.UserAcknowledgementSave(model);

            return Json(new
            {
                result = res
            });
        }

        [HttpGet]
        public IActionResult UserCommunication([FromQuery] string IOMID)
        {
            UserCommunicationViewModel model = new();
            string IOM_ID = WebUtility.UrlDecode(Encryption.Decrypt(IOMID));

            DataSet ds = new DataSet();
            ds = _iomContractRepos.GetIOMFullDetail(IOM_ID, "");

            //Requestor Employee Details
            if (ds.Tables[0].Rows.Count > 0)
            {
                model.lbl_assname = ds.Tables[0].Rows[0]["ENAME"].ToString() + "[" + ds.Tables[0].Rows[0]["ADEMPCODE"].ToString() + "]";
                model.lbl_site = ds.Tables[0].Rows[0]["SITE"].ToString();
                model.lbl_op = ds.Tables[0].Rows[0]["OPERATION"].ToString();
                model.lbl_dept = ds.Tables[0].Rows[0]["DEPARTMENT"].ToString();
                model.lbl_div = ds.Tables[0].Rows[0]["DIVISION"].ToString();
                model.lbl_sec = ds.Tables[0].Rows[0]["SECTION"].ToString();
            }

            //IOM Details
            if (ds.Tables[1].Rows.Count > 0)
            {
                model.hdiomid = ds.Tables[1].Rows[0]["IOMID"].ToString();
                model.lblfrom = ds.Tables[1].Rows[0]["IOMFROM"].ToString();
                model.lblto = "Legal & Secretariat";
                model.lblagreementtype = ds.Tables[1].Rows[0]["AGREEMENTTYPEDESC"].ToString();
                model.lblcontracttype = ds.Tables[1].Rows[0]["CONTRACTTYPEDESC"].ToString();
                model.lbleffdate = ds.Tables[1].Rows[0]["EFFECTIVEDATE"].ToString();
                model.lblexpdate = ds.Tables[1].Rows[0]["EXPIRYDATE"].ToString();
                if (ds.Tables[1].Rows[0]["TERMSYEAR"].ToString() == "100")
                {
                    model.lblterm = "Lifetime";
                }
                else
                {
                    model.lblterm = ds.Tables[1].Rows[0]["TERMSYEAR"].ToString() + " Year, " + ds.Tables[1].Rows[0]["TERMSMONTH"].ToString() + " Month";
                }
                model.lblvendorname = ds.Tables[1].Rows[0]["VENDORNAME"].ToString();
                model.lblmannerofpayment = ds.Tables[1].Rows[0]["MANNEROFPAYMENT"].ToString();
                model.lblpurpose = ds.Tables[1].Rows[0]["PURPOSE"].ToString();
                model.lblremarks = ds.Tables[1].Rows[0]["REMARKS"].ToString();
                //Previous Contract Details
                if (string.IsNullOrEmpty(ds.Tables[1].Rows[0]["AGREEMENTHEADERID"].ToString()))
                {
                    ViewBag.PrevcontractdivVisible = false;
                }
                else
                {
                    ViewBag.PrevcontractdivVisible = true;
                    model.lblagreementid = ds.Tables[1].Rows[0]["AGREEMENTHEADERID"].ToString();
                    model.lblprevagrperiod = ds.Tables[1].Rows[0]["AGRPERIOD"].ToString();
                    model.prevappnotelink = "../Uploads/IOM/" + ds.Tables[1].Rows[0]["PREVAPPNOTE"].ToString();
                    model.prevappnotelinkText = ds.Tables[1].Rows[0]["PREVAPPNOTE"].ToString();
                    model.prevfinaldoclink = "../Uploads/IOM/" + ds.Tables[1].Rows[0]["PREVFINALDOC"].ToString();
                    model.prevfinaldoclinkText = ds.Tables[1].Rows[0]["PREVFINALDOC"].ToString();
                }
                //Get Approval Note
                string referencedoc = ds.Tables[1].Rows[0]["REFERENCEDOC"].ToString();
                if (string.IsNullOrEmpty(referencedoc.Trim()))
                {
                    ViewBag.trreferencedocVisible = false;
                }
                else
                {
                    ViewBag.trreferencedocVisible = true;
                    model.agreementlink = "../Uploads/IOM/" + referencedoc;
                    model.agreementlinkText = referencedoc;
                }

                //Get Anti Bribery
                string antibribery = ds.Tables[1].Rows[0]["ANTIBRIBERY"].ToString();
                model.antibriberylink = "../Uploads/IOM/" + antibribery;
                model.antibriberylinkText = antibribery;

                //Get NDA Document
                string ndadocument = ds.Tables[1].Rows[0]["NDADOCUMENT"].ToString();
                model.ndadocumentlink = "../Uploads/IOM/" + ndadocument;
                model.ndadocumentlinkText = ndadocument;

                //Show/Hide Anti Bribery and NDA Document
                if (antibribery.Trim() == "")
                {
                    ViewBag.trantibriberyVisible = false;
                    ViewBag.trndadocumentVisible = false;
                }

                //Master agreement
                if (ds.Tables[1].Rows[0]["MASTERAGREEMENT"].ToString() != "0")
                {
                    ViewBag.trmstagrVisible = true;
                    model.lblmstagr = ds.Tables[1].Rows[0]["MSTAGR"].ToString();
                }
            }
            if (ds.Tables[4].Rows.Count > 0)
            {
                //string usertype = "usertype";
                //var customers = from customer in ds.Tables[4].AsEnumerable()
                //                where customer.Field<object>("USERTYPE") == usertype
                //                select new
                //                {
                //                    ENAME = customer.Field<int>("ADDEDBY"),
                //                    EMAILID = customer.Field<object>("ENAME"),                            
                //                };
                DataRow[] dr = ds.Tables[4].AsEnumerable().Where(d => d["USERTYPE"].ToString() == "admin").OrderByDescending(s => s["COMMID"]).Take(1).ToArray();
                if (dr.Length > 0)
                {
                    model.HDENAME = dr[0]["ADDEDBY"].ToString();
                    model.HDEMAILID = dr[0]["EMAILID"].ToString();
                }
                DataRow[] max = ds.Tables[4].AsEnumerable().OrderByDescending(row => row["COMMID"]).Take(1).ToArray();
                if (max.Length > 0)
                {
                    if (max[0]["USERTYPE"].ToString() == "user")
                    {
                        ViewBag.btnCommunicationVisible = false;
                    }
                    else
                    {
                        ViewBag.btnCommunicationVisible = true;
                    }
                }

                var grdCommunicationHistory = ds.Tables[4];

                var grdCommunicationHistoryList = new List<grdCommunicationHistory>();
                if (grdCommunicationHistory != null && grdCommunicationHistory.Rows.Count > 0)
                {
                    grdCommunicationHistoryList = grdCommunicationHistory.AsEnumerable()
                           .Select(row => new grdCommunicationHistory
                           {
                               ADDEDDATE = Convert.ToString(row.Field<object>("ADDEDDATE")),
                               ADDEDBY = Convert.ToString(row.Field<object>("ADDEDBY")),
                               USERTYPE = Convert.ToString(row.Field<object>("USERTYPE")),
                               REMARKS = Convert.ToString(row.Field<object>("REMARKS")),
                               ATTACHMENT = Convert.ToString(row.Field<object>("ATTACHMENT"))
                           }).ToList();
                }
                model.grdCommunicationHistory = grdCommunicationHistoryList;

            }


            return View(model);
        }

        [HttpPost]
        public IActionResult UserCommunicationSave([FromForm] UserCommunicationSave model)
        {
            var res = new IOMContractOperationResult();
            res = _iomContractService.UserCommunicationSave(model);

            return Json(new
            {
                result = res
            });
        }
        #endregion

        #region ContractList

        [HttpGet]
        public IActionResult ContractList()
        {
            ContractListViewModel model = new();
            //Validate employee that employee are valid or not
            string functiondesignation = _empDetails.Functional_Designation_Id.ToString();
            if (functiondesignation == "0")
            {
                return RedirectToAction("Home", "Home");
            }

            var dsGetContractMasterList = _iomContractRepos.GETCONTRACTMASTER("", "", "1", "");
            var ddlcontracttypeList = new List<SelectListItem>();
            if (dsGetContractMasterList != null && dsGetContractMasterList.Tables.Count > 0 && dsGetContractMasterList.Tables[0].Rows.Count > 0)
            {
                ddlcontracttypeList = dsGetContractMasterList.Tables[0].AsEnumerable()
                  .Select(row => new SelectListItem
                  {
                      Text = row["CONTRATNAME"].ToString(),
                      Value = row["CONTRACTTYPEID"].ToString(),
                  }).ToList();
            }
            ddlcontracttypeList.Insert(0, new SelectListItem
            {
                Text = "-All Contract-",
                Value = ""
            });

            model.ddlcontracttypeList = ddlcontracttypeList;

            //Get contract list

            string operationid = _empDetails.Operation_Id.ToString();
            string contracttype = "";
            string vendorname = "";
            string expdatefrom = "";
            string expdateto = "";
            string ecode = _userId;
            //string uniqueid = 
            DataSet ds = _iomContractRepos.GETCONTRACTLIST(contracttype, vendorname, expdatefrom, expdateto, ecode, "");
            var grdcontractlistSource = ds.Tables[0];

            //var grdcontractlist = new List<grdcontractlist>();
            if (grdcontractlistSource != null && grdcontractlistSource.Rows.Count > 0)
            {
                var grdcontractlist = grdcontractlistSource.AsEnumerable()
                       .Select(row => new grdcontractlist
                       {
                           CONTRACTTYPEDESC = Convert.ToString(row.Field<object>("CONTRACTTYPEDESC")),
                           VENDORNAME = Convert.ToString(row.Field<object>("VENDORNAME")),
                           AGREEMENTDATE = Convert.ToString(row.Field<object>("AGREEMENTDATE")),
                           EXPIRYDATE = Convert.ToString(row.Field<object>("EXPIRYDATE")),
                           FINALDOC = Convert.ToString(row.Field<object>("FINALDOC")),
                           AGREEMENTHEADERID = Convert.ToString(row.Field<object>("AGREEMENTHEADERID")),
                           EXPDAYS = Convert.ToString(row.Field<object>("EXPDAYS")),
                       })
                       .AsQueryable();

                // Total items
                var totalItems = grdcontractlist.Count();
                model.filter = new ContractListViewModelFilter();
                model.filter.page = 1;
                model.filter.pageSize = 10;
                // Guard bounds
                if (model.filter.page < 1) model.filter.page = 1;
                var totalPages = (int)Math.Ceiling((double)totalItems / model.filter.pageSize);
                if (totalPages > 0 && model.filter.page > totalPages) model.filter.page = totalPages;

                // Slice
                var items = grdcontractlist
                    .Skip((model.filter.page - 1) * model.filter.pageSize)
                    .Take(model.filter.pageSize)
                    .ToList();

                // Pass data to view

                model.filter.totalItems = totalItems;
                model.filter.totalPages = totalPages;
                model.filter.hasPrevious = model.filter.page > 1;
                model.filter.hasNext = model.filter.page < model.filter.totalPages;

                model.grdcontractlist = items;
            }
            else
            {
                model.grdcontractlist = new List<grdcontractlist>();
            }
            

            return View(model);
        }

        [HttpPost]
        public IActionResult ContractList(ContractListViewModelFilter filter)
        {
            ContractListViewModel model = new();

            var dsGetContractMasterList = _iomContractRepos.GETCONTRACTMASTER("", "", "1", "");
            var ddlcontracttypeList = new List<SelectListItem>();
            if (dsGetContractMasterList != null && dsGetContractMasterList.Tables.Count > 0 && dsGetContractMasterList.Tables[0].Rows.Count > 0)
            {
                ddlcontracttypeList = dsGetContractMasterList.Tables[0].AsEnumerable()
                  .Select(row => new SelectListItem
                  {
                      Text = row["CONTRATNAME"].ToString(),
                      Value = row["CONTRACTTYPEID"].ToString(),
                  }).ToList();
            }
            ddlcontracttypeList.Insert(0, new SelectListItem
            {
                Text = "-All Contract-",
                Value = ""
            });

            //Get contract list
            string operationid = _empDetails.Operation_Id.ToString();
            string contracttype = filter.ddlcontracttype ?? string.Empty;
            string vendorname = filter.txtvendorname ?? string.Empty;
            string expdatefrom = filter.txtStarttDate ?? string.Empty;
            string expdateto = filter.txtEndtDate ?? string.Empty;
            string ecode = _userId;
            //string uniqueid = 
            DataSet ds = _iomContractRepos.GETCONTRACTLIST(contracttype, vendorname, expdatefrom, expdateto, ecode, "");
            var grdcontractlistSource = ds.Tables[0];

            //var grdcontractlist = new List<grdcontractlist>();
            if (grdcontractlistSource != null && grdcontractlistSource.Rows.Count > 0)
            {
                var grdcontractlist = grdcontractlistSource.AsEnumerable()
                       .Select(row => new grdcontractlist
                       {
                           CONTRACTTYPEDESC = Convert.ToString(row.Field<object>("CONTRACTTYPEDESC")),
                           VENDORNAME = Convert.ToString(row.Field<object>("VENDORNAME")),
                           AGREEMENTDATE = Convert.ToString(row.Field<object>("AGREEMENTDATE")),
                           EXPIRYDATE = Convert.ToString(row.Field<object>("EXPIRYDATE")),
                           FINALDOC = Convert.ToString(row.Field<object>("FINALDOC")),
                           AGREEMENTHEADERID = Convert.ToString(row.Field<object>("AGREEMENTHEADERID")),
                           EXPDAYS = Convert.ToString(row.Field<object>("EXPDAYS")),
                       }).AsQueryable();
                
                // Total items
                model.filter = new ContractListViewModelFilter();
                var totalItems = grdcontractlist.Count();
                model.filter.page = filter.page;
                model.filter.pageSize = filter.pageSize;
                // Guard bounds
                if (filter.page < 1) filter.page = 1;
                var totalPages = (int)Math.Ceiling((double)totalItems / filter.pageSize);
                if (totalPages > 0 && filter.page > totalPages) filter.page = totalPages;

                // Slice
                var items = grdcontractlist
                    .Skip((filter.page - 1) * filter.pageSize)
                    .Take(filter.pageSize)
                    .ToList();


                // Pass data to view

                model.filter.totalItems = totalItems;
                model.filter.totalPages = totalPages;
                model.filter.hasPrevious = filter.page > 1;
                model.filter.hasNext = filter.page < filter.totalPages;

                model.grdcontractlist = items;
            }
            else
            {
                model.grdcontractlist = new List<grdcontractlist>();
            }

            model.ddlcontracttypeList = ddlcontracttypeList;
            model.filter = filter;


            return View(model);
        }

        [HttpGet]
        public IActionResult loadContractListPartialView([FromQuery] string HDAGREEMENTID)
        {

            var modelList = new List<grdSubcontractlist>();
            DataSet ds = _iomContractRepos.GETIOMBYAGREEMENTID(HDAGREEMENTID);
            var grdcontractlistSource = ds.Tables[0];

            if (grdcontractlistSource != null && grdcontractlistSource.Rows.Count > 0)
            {
                modelList = grdcontractlistSource.AsEnumerable()
                       .Select(row => new grdSubcontractlist
                       {
                           IOMID = Convert.ToString(row.Field<object>("IOMID")),
                           AGREEMENTTYPEDESC = Convert.ToString(row.Field<object>("AGREEMENTTYPEDESC")),
                           VENDORNAME = Convert.ToString(row.Field<object>("VENDORNAME")),
                           EFFECTIVEDATE = Convert.ToString(row.Field<object>("EFFECTIVEDATE")),
                           EXPIRYDATE = Convert.ToString(row.Field<object>("EXPIRYDATE")),
                           processstatusid = Convert.ToString(row.Field<object>("processstatusid")),
                           REQUESTDBY = Convert.ToString(row.Field<object>("REQUESTDBY")),
                           STATUSNAME = Convert.ToString(row.Field<object>("STATUSNAME")),
                           UNIQUEID = Convert.ToString(row.Field<object>("UNIQUEID")),
                           finaldoc = Convert.ToString(row.Field<object>("finaldoc"))
                       }).ToList();
            }
            return PartialView("_ContractList", modelList);
        }

        #endregion

      

        [HttpGet]
        public IActionResult ContractRenewal([FromQuery] string AGREEMENTID)
        {
            ContractRenewalViewModel model = new();
            ViewBag.ddlfromEnable = true;

            string? OPHEADID = _empDetails.Operation_Id;
            if (!string.IsNullOrWhiteSpace(OPHEADID))
            {
                ViewBag.ddlfromEnable = false;
                model.ddlfrom = OPHEADID;
            }
            model.txtTo = "Legal & Secretariat";
            model.rdomaster = "0";
            model.ddlagreementList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Renewal", Value = "3" }
            };

            #region FillOperation
            var dsGetAllOperation = _searchEmp.get_AllOperation();
            var ddlFromList = new List<SelectListItem>();
            if (dsGetAllOperation != null && dsGetAllOperation.Tables.Count > 0 && dsGetAllOperation.Tables[0].Rows.Count > 0)
            {
                ddlFromList = dsGetAllOperation.Tables[0].AsEnumerable()
                  .Select(row => new SelectListItem
                  {
                      Text = row["OPERATION"].ToString(),
                      Value = row["OPERATIONID"].ToString(),
                  }).ToList();
            }
            ddlFromList.Insert(0, new SelectListItem
            {
                Text = "-Select Operation-",
                Value = "0"
            });
            model.ddlFromList = ddlFromList;
            #endregion

            #region PopulateDateTime
            int intVal_day = 1;
            var optDayList = new List<SelectListItem>();
            var optEDList = new List<SelectListItem>();
            do
            {
                optDayList.Add(new SelectListItem { Text = intVal_day.ToString(), Value = intVal_day.ToString() });
                optEDList.Add(new SelectListItem { Text = intVal_day.ToString(), Value = intVal_day.ToString() });
                intVal_day = intVal_day + 1;
            }
            while (intVal_day < 32);

            model.optDayList = optDayList;
            model.optEDList = optEDList;
            #endregion

            #region contract
            var dsGetContract = _iomContractRepos.GETCONTRACTMASTER(string.Empty, string.Empty, "1", string.Empty);
            var ddlcontractList = new List<SelectListItem>();
            if (dsGetContract != null && dsGetContract.Tables.Count > 0 && dsGetContract.Tables[0].Rows.Count > 0)
            {
                ddlcontractList = dsGetContract.Tables[0].AsEnumerable()
                  .Select(row => new SelectListItem
                  {
                      Text = row["CONTRATNAME"].ToString(),
                      Value = row["CONTRACTTYPEID"].ToString()
                  }).ToList();
            }
            ddlcontractList.Insert(0, new SelectListItem
            {
                Text = "-Select Contract-",
                Value = string.Empty
            });
            model.ddlcontractList = ddlcontractList;

            #endregion

            #region ddlMasterAgreement
            var dsGetMasterAgreement = _iomContractRepos.GETCONTRACTLIST(string.Empty, string.Empty, string.Empty, string.Empty, _userId, string.Empty);
            var ddlmstagreementList = new List<SelectListItem>();
            if (dsGetMasterAgreement != null && dsGetMasterAgreement.Tables.Count > 0 && dsGetMasterAgreement.Tables[0].Rows.Count > 0)
            {
                ddlmstagreementList = dsGetMasterAgreement.Tables[0].AsEnumerable()
                  .Select(row => new SelectListItem
                  {
                      Text = row["VENDORNAME"].ToString(),
                      Value = row["AGREEMENTHEADERID"].ToString()
                  }).ToList();
            }
            ddlmstagreementList.Insert(0, new SelectListItem
            {
                Text = "-Select Master Agreement-",
                Value = string.Empty
            });
            model.ddlmstagreementList = ddlmstagreementList;
            #endregion

            #region GetIOMdetail
            int currentYr = DateTime.Now.Year;
            string AGREEMENT_ID = WebUtility.UrlDecode(Encryption.Decrypt(Convert.ToString(AGREEMENTID)));
            string ecode = _userId;
            DataSet ds = _iomContractRepos.GETCONTRACTLIST("", "", "", "", ecode, AGREEMENT_ID);
            if (ds.Tables[0].Rows.Count > 0)
            {
                model.HDAGREEMENTID = ds.Tables[0].Rows[0]["AGREEMENTHEADERID"].ToString();
                model.ddlcontract = ds.Tables[0].Rows[0]["CONTRACTTYPEID"].ToString();
                model.ddlTermyear = ds.Tables[0].Rows[0]["TERMSYEAR"].ToString();
                model.ddlTermmonth = ds.Tables[0].Rows[0]["TERMSMONTH"].ToString();
                model.txtvendorname = ds.Tables[0].Rows[0]["VENDORNAME"].ToString();
                model.ddlMannerOfPayment = ds.Tables[0].Rows[0]["MANNEROFPAYMENTID"].ToString();
                model.txtPurpose = ds.Tables[0].Rows[0]["PURPOSE"].ToString();
                model.lblUNIQUEID = ds.Tables[0].Rows[0]["UNIQUEID"] == null ? "" : ds.Tables[0].Rows[0]["UNIQUEID"].ToString();

                string effdate = ds.Tables[0].Rows[0]["EFFECTIVEDATE"].ToString();
                string[] spliteffdate = effdate.Split(new Char[] { '/' });
                model.optED = Convert.ToString(Convert.ToInt32(spliteffdate[0]));
                model.optEM = Convert.ToString(Convert.ToInt32(spliteffdate[1]));
                int effyr = Convert.ToInt32(spliteffdate[2]);

                model.optEYList = new();
                for (int x = effyr - 1; x <= (currentYr + 1); x++)
                {
                    model.optEYList.Add(new SelectListItem { Text = x.ToString(), Value = x.ToString() });
                }
                model.optEY = effyr.ToString();

                string dateofexp = Convert.ToDateTime(ds.Tables[0].Rows[0]["EXPIRYDATEDESC"]).ToString("dd/MM/yyyy");
                string[] splitexpdate = dateofexp.Split(new Char[] { '/' });
                model.optDay = Convert.ToString(Convert.ToInt32(splitexpdate[0]));
                model.optMonth = Convert.ToString(Convert.ToInt32(splitexpdate[1]));
                int expyr = Convert.ToInt32(splitexpdate[2]);

                model.optYearList = new();
                for (int y = expyr - 100; y <= (expyr + 100); y++)
                {
                    model.optYearList.Add(new SelectListItem { Text = y.ToString(), Value = y.ToString() });
                }
                model.optYear = Convert.ToString(expyr);


                model.lblagreementid = ds.Tables[0].Rows[0]["AGREEMENTHEADERID"].ToString();
                string? APPNOTE = ds.Tables[0].Rows[0]["REFERENCEDOC"].ToString();
                model.appnotelink = "../../Uploads/IOM/" + APPNOTE;
                model.appnotelinkText = APPNOTE;

                string? FINALDOC = ds.Tables[0].Rows[0]["FINALDOC"].ToString();
                model.FINALDOCLINK = "../../Uploads/IOM/" + FINALDOC;
                model.FINALDOCLINKText = FINALDOC;

                ////Get Anti Bribery
                //string antibribery = ds.Tables[0].Rows[0]["ANTIBRIBERY"].ToString();
                //if (string.IsNullOrEmpty(antibribery.Trim()))
                //{
                //    antibriberylink.Visible = false;
                //    fileantibribery.Visible = true;
                //}
                //else
                //{
                //    antibriberylink.Visible = true;
                //    fileantibribery.Visible = false;
                //    antibriberylink.HRef = "../../Uploads/IOM/" + antibribery;
                //    antibriberylink.InnerText = antibribery;
                //}

                ////Get NDA Document
                //string ndadocument = ds.Tables[0].Rows[0]["NDADOCUMENT"].ToString();
                //if (string.IsNullOrEmpty(ndadocument.Trim()))
                //{
                //    ndadocumentlink.Visible = false;
                //    filendadocument.Visible = true;
                //}
                //else
                //{
                //    ndadocumentlink.Visible = true;
                //    filendadocument.Visible = false;
                //    ndadocumentlink.HRef = "../../Uploads/IOM/" + ndadocument;
                //    ndadocumentlink.InnerText = ndadocument;
                //}
                //MASTER AGREEMENT
                if (!string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["MASTERAGREEMENT"].ToString()))
                {
                    model.rdomaster = "1";
                    model.ddlmstagreement = Convert.ToString(ds.Tables[0].Rows[0]["MASTERAGREEMENT"]);
                }

                DataSet dsss = _iomContractRepos.GETIOMBYAGREEMENTID(ds.Tables[0].Rows[0]["AGREEMENTHEADERID"].ToString());
                if (dsss.Tables[0].Rows.Count > 0)
                {
                    var grdSource = dsss.Tables[0].AsEnumerable()
                              .Select(row => new grdPrivContractHistory
                              {
                                  IOMID = Convert.ToInt64(row.Field<object>("IOMID")),
                                  AGREEMENTTYPEDESC = Convert.ToString(row.Field<object>("AGREEMENTTYPEDESC")),
                                  VENDORNAME = Convert.ToString(row.Field<object>("VENDORNAME")),
                                  EFFECTIVEDATE = Convert.ToString(row.Field<object>("EFFECTIVEDATE")),
                                  EXPIRYDATE = Convert.ToString(row.Field<object>("EXPIRYDATE")),
                                  processstatusid = Convert.ToInt32(row.Field<object>("processstatusid")),
                                  STATUSNAME = Convert.ToString(row.Field<object>("STATUSNAME")),
                                  REQUESTDBY = Convert.ToString(row.Field<object>("REQUESTDBY")),
                                  finaldoc = Convert.ToString(row.Field<object>("finaldoc"))
                              }).ToList();
                    model.grdPrivContractHistory = grdSource;
                }

            }
            #endregion

            #region GetNextapprovalauthority
            DataSet dss = new DataSet();
            dss = _iomContractRepos.GetNextApprovalAuthority(_userId);
            if (dss.Tables[0].Rows[0]["AUTHLEVEL"].ToString() != "4")
            {
                model.hdauthcode = dss.Tables[0].Rows[0]["ADEMPCODE"].ToString();
                model.hdauthname = dss.Tables[0].Rows[0]["EMPNAME"].ToString();
                model.hdauthemailid = dss.Tables[0].Rows[0]["EMAILID"].ToString();
                model.lbl_appauth = dss.Tables[0].Rows[0]["EMPNAME"].ToString() + "[" + dss.Tables[0].Rows[0]["ADEMPCODE"].ToString() + "]";
                model.hdauthlevel = dss.Tables[0].Rows[0]["AUTHLEVEL"].ToString();
            }
            else
            {
                var ddl_appauthList = new List<SelectListItem>();

                model.hdauthlevel = "4";
                ViewBag.lbl_appauthVisiable = false;
                ViewBag.ddl_appauthVisiable = true;

                if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                {
                    ddl_appauthList = dss.Tables[0].AsEnumerable()
                      .Select(row => new SelectListItem
                      {
                          Text = row["EMPLOYEE"].ToString(),
                          Value = row["ADEMPCODE"].ToString(),
                      }).ToList();
                }
                ddl_appauthList.Insert(0, new SelectListItem("-Select-", ""));
                model.ddl_appauthList = ddl_appauthList;
            }


            #endregion


            return View(model);
        }

        [HttpPost]
        public IActionResult ContractRenewalSave([FromForm] ContractRenewalSave model)
        {
            var res = new IOMContractOperationResult();
            res = _iomContractService.ContractRenewalSave(model);

            return Json(new
            {
                result = res
            });
        }

        [HttpGet]
        public IActionResult CloseContract([FromQuery] string AGREEMENTID)
        {
            CloseContractViewModel model = new();

            string AGREEMENT_ID = WebUtility.UrlDecode(Encryption.Decrypt(AGREEMENTID));
            string ecode = _userId;
            DataSet ds = _iomContractRepos.GETCONTRACTLIST("", "", "", "", ecode, AGREEMENT_ID);

            //IOM Details
            if (ds.Tables[0].Rows.Count > 0)
            {
                model.HDAGREEMENTID = ds.Tables[0].Rows[0]["AGREEMENTHEADERID"].ToString();
                model.lblfrom = ds.Tables[0].Rows[0]["OPERATION"].ToString();
                model.lblto = "Legal & Secretariat";
                model.lblcontracttype = ds.Tables[0].Rows[0]["CONTRACTTYPEDESC"].ToString();
                model.lblexpdate = ds.Tables[0].Rows[0]["EXPIRYDATE"].ToString();
                if (ds.Tables[0].Rows[0]["TERMSYEAR"].ToString().Trim() == "100")
                {
                    model.lblterm = "Lifetime";
                }
                else
                {
                    model.lblterm = ds.Tables[0].Rows[0]["TERMSYEAR"].ToString() + " Year, " + ds.Tables[0].Rows[0]["TERMSMONTH"].ToString() + " Month";
                }
                model.lblvendorname = ds.Tables[0].Rows[0]["VENDORNAME"].ToString();
                model.lblmannerofpayment = ds.Tables[0].Rows[0]["MANNEROFPAYMENT"].ToString();
                model.lblpurpose = ds.Tables[0].Rows[0]["purpose"].ToString();
                model.AgreementlinkText = ds.Tables[0].Rows[0]["referencedoc"].ToString();
                model.Agreementlink = "../../Uploads/IOM/" + ds.Tables[0].Rows[0]["referencedoc"].ToString();
                model.approvalnotelinkText = ds.Tables[0].Rows[0]["FINALDOC"].ToString();
                model.approvalnotelink = "../../Uploads/IOM/" + ds.Tables[0].Rows[0]["FINALDOC"].ToString();
                model.antibriberylinkText = ds.Tables[0].Rows[0]["ANTIBRIBERY"].ToString();
                model.antibriberylink = "../../Uploads/IOM/" + ds.Tables[0].Rows[0]["ANTIBRIBERY"].ToString();
                model.ndadocumentlinkText = ds.Tables[0].Rows[0]["NDADOCUMENT"].ToString();
                model.ndadocumentlink = "../../Uploads/IOM/" + ds.Tables[0].Rows[0]["NDADOCUMENT"].ToString();

                //MASTER AGREEMENT
                if (ds.Tables[0].Rows[0]["MASTERAGREEMENT"].ToString().Trim() != "0")
                {
                    ViewBag.trmstagrVisible = true;
                    model.lblmstagr = ds.Tables[0].Rows[0]["MSTAGR"].ToString();
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult CloseContract([FromForm] CloseContractSave model)
        {
            var res = new IOMContractOperationResult();
            res = _iomContractService.CloseContractSave(model);

            return Json(new
            {
                result = res
            });
        }

        [HttpGet]
        public IActionResult ViewAgreement([FromQuery] string AGREEMENTID)
        {
            ViewAgreementViewModel model = new();

            string ecode = _userId;
            DataSet ds = _iomContractRepos.ViewAgreementDetail(AGREEMENTID);

            //IOM Details
            if (ds.Tables[0].Rows.Count > 0)
            {
                model.lblfrom = ds.Tables[0].Rows[0]["OPERATION"].ToString();
                model.lblto = "Legal & Secretariat";
                model.lblcontracttype = ds.Tables[0].Rows[0]["CONTRACTTYPEDESC"].ToString();
                model.lblexpdate = ds.Tables[0].Rows[0]["EXPIRYDATE"].ToString();
                if (ds.Tables[0].Rows[0]["TERMSYEAR"].ToString() == "100")
                {
                    model.lblterm = "Lifetime";
                }
                else
                {
                    model.lblterm = ds.Tables[0].Rows[0]["TERMSYEAR"].ToString() + " Year, " + ds.Tables[0].Rows[0]["TERMSMONTH"].ToString() + " Month";
                }
                model.lblvendorname = ds.Tables[0].Rows[0]["VENDORNAME"].ToString();
                model.lblmannerofpayment = ds.Tables[0].Rows[0]["MANNEROFPAYMENT"].ToString();
                model.lblpurpose = ds.Tables[0].Rows[0]["PURPOSE"].ToString();

                //Get Approval Note
                string approvalnote = ds.Tables[0].Rows[0]["REFERENCEDOC"].ToString();
                if (string.IsNullOrEmpty(approvalnote.Trim()))
                {
                    ViewBag.trappnoteVisible = false;
                }
                else
                {
                    ViewBag.trappnoteVisible = true;
                    model.appnotelink = "../../Uploads/IOM/" + approvalnote;
                    model.appnotelinkText = approvalnote;
                }

                //Get Final Document
                string finaldoc = ds.Tables[0].Rows[0]["FINALDOC"].ToString();
                if (string.IsNullOrEmpty(finaldoc.Trim()))
                {
                    ViewBag.trfinaldocVisible = false;
                }
                else
                {
                    ViewBag.trfinaldocVisible = true;
                    model.finaldoclink = "../../Uploads/IOM/" + finaldoc;
                    model.finaldoclinkText = finaldoc;
                }

                //Get Anti Bribery
                string antibribery = ds.Tables[0].Rows[0]["ANTIBRIBERY"].ToString();
                model.antibriberylink = "../../Uploads/IOM/" + antibribery;
                model.antibriberylinkText = antibribery;

                //Get NDA Document
                string ndadocument = ds.Tables[0].Rows[0]["NDADOCUMENT"].ToString();
                model.ndadocumentlink = "../../Uploads/IOM/" + ndadocument;
                model.ndadocumentlinkText = ndadocument;

                //Show/Hide Anti Bribery and NDA Document
                if (antibribery == "")
                {
                    ViewBag.trantibriberyVisible = false;
                    ViewBag.trndadocumentVisible = false;
                }
                //Master agreement
                if (ds.Tables[0].Rows[0]["MASTERAGREEMENT"].ToString() != "0")
                {
                    ViewBag.trmstagrVisible = true;
                    model.lblmstagr = ds.Tables[0].Rows[0]["MSTAGR"].ToString();
                }
            }

            return View(model);

        }

        [HttpGet]
        public IActionResult ContractAmendment([FromQuery] string AGREEMENTID)
        {
            ContractAmendmentViewModel model = new();
            ViewBag.ddlfromEnable = true;

            string? OPHEADID = _empDetails.Operation_Id;
            if (!string.IsNullOrWhiteSpace(OPHEADID))
            {
                ViewBag.ddlfromEnable = false;
                model.ddlfrom = OPHEADID;
            }
            model.txtTo = "Legal & Secretariat";
            model.rdomaster = "0";
            model.ddlagreementList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Amendment", Value = "4" }
            };

            #region FillOperation
            var dsGetAllOperation = _searchEmp.get_AllOperation();
            var ddlFromList = new List<SelectListItem>();
            if (dsGetAllOperation != null && dsGetAllOperation.Tables.Count > 0 && dsGetAllOperation.Tables[0].Rows.Count > 0)
            {
                ddlFromList = dsGetAllOperation.Tables[0].AsEnumerable()
                  .Select(row => new SelectListItem
                  {
                      Text = row["OPERATION"].ToString(),
                      Value = row["OPERATIONID"].ToString(),
                  }).ToList();
            }
            ddlFromList.Insert(0, new SelectListItem
            {
                Text = "-Select Operation-",
                Value = "0"
            });
            model.ddlFromList = ddlFromList;
            #endregion

            #region PopulateDateTime
            int intVal_day = 1;
            var optDayList = new List<SelectListItem>();
            var optEDList = new List<SelectListItem>();
            do
            {
                optDayList.Add(new SelectListItem { Text = intVal_day.ToString(), Value = intVal_day.ToString() });
                optEDList.Add(new SelectListItem { Text = intVal_day.ToString(), Value = intVal_day.ToString() });
                intVal_day = intVal_day + 1;
            }
            while (intVal_day < 32);

            model.optDayList = optDayList;
            model.optEDList = optEDList;
            #endregion

            #region contract
            var dsGetContract = _iomContractRepos.GETCONTRACTMASTER(string.Empty, string.Empty, "1", string.Empty);
            var ddlcontractList = new List<SelectListItem>();
            if (dsGetContract != null && dsGetContract.Tables.Count > 0 && dsGetContract.Tables[0].Rows.Count > 0)
            {
                ddlcontractList = dsGetContract.Tables[0].AsEnumerable()
                  .Select(row => new SelectListItem
                  {
                      Text = row["CONTRATNAME"].ToString(),
                      Value = row["CONTRACTTYPEID"].ToString()
                  }).ToList();
            }
            ddlcontractList.Insert(0, new SelectListItem
            {
                Text = "-Select Contract-",
                Value = string.Empty
            });
            model.ddlcontractList = ddlcontractList;

            #endregion

            #region ddlMasterAgreement
            var dsGetMasterAgreement = _iomContractRepos.GETCONTRACTLIST(string.Empty, string.Empty, string.Empty, string.Empty, _userId, string.Empty);
            var ddlmstagreementList = new List<SelectListItem>();
            if (dsGetMasterAgreement != null && dsGetMasterAgreement.Tables.Count > 0 && dsGetMasterAgreement.Tables[0].Rows.Count > 0)
            {
                ddlmstagreementList = dsGetMasterAgreement.Tables[0].AsEnumerable()
                  .Select(row => new SelectListItem
                  {
                      Text = row["VENDORNAME"].ToString(),
                      Value = row["AGREEMENTHEADERID"].ToString()
                  }).ToList();
            }
            ddlmstagreementList.Insert(0, new SelectListItem
            {
                Text = "-Select Master Agreement-",
                Value = string.Empty
            });
            model.ddlmstagreementList = ddlmstagreementList;
            #endregion

            #region GetIOMdetail
            int currentYr = DateTime.Now.Year;
            string AGREEMENT_ID = WebUtility.UrlDecode(Encryption.Decrypt(Convert.ToString(AGREEMENTID)));
            string ecode = _userId;
            DataSet ds = _iomContractRepos.GETCONTRACTLIST("", "", "", "", ecode, AGREEMENT_ID);
            if (ds.Tables[0].Rows.Count > 0)
            {
                model.HDAGREEMENTID = ds.Tables[0].Rows[0]["AGREEMENTHEADERID"].ToString();
                model.ddlcontract = ds.Tables[0].Rows[0]["CONTRACTTYPEID"].ToString();
                model.ddlTermyear = ds.Tables[0].Rows[0]["TERMSYEAR"].ToString();
                model.ddlTermmonth = ds.Tables[0].Rows[0]["TERMSMONTH"].ToString();
                model.txtvendorname = ds.Tables[0].Rows[0]["VENDORNAME"].ToString();
                model.ddlMannerOfPayment = ds.Tables[0].Rows[0]["MANNEROFPAYMENTID"].ToString();
                model.txtPurpose = ds.Tables[0].Rows[0]["PURPOSE"].ToString();
                model.lblUNIQUEID = ds.Tables[0].Rows[0]["UNIQUEID"] == null ? "" : ds.Tables[0].Rows[0]["UNIQUEID"].ToString();

                DateTime effdate = Convert.ToDateTime(ds.Tables[0].Rows[0]["AGREEMENTDATE"]);
                model.optED = Convert.ToString(effdate.Day);
                model.optEM = Convert.ToString(effdate.Month);
                int effyr = effdate.Year;

                model.optEYList = new();
                for (int x = effyr - 1; x <= (currentYr + 1); x++)
                {
                    model.optEYList.Add(new SelectListItem { Text = x.ToString(), Value = x.ToString() });
                }
                model.optEY = effyr.ToString();

                DateTime dateofexp = Convert.ToDateTime(ds.Tables[0].Rows[0]["EXPIRYDATE"]);
                model.optDay = Convert.ToString(dateofexp.Day);
                model.optMonth = Convert.ToString(dateofexp.Month);
                int expyr = dateofexp.Year;

                model.optYearList = new();
                for (int y = expyr - 100; y <= (expyr + 100); y++)
                {
                    model.optYearList.Add(new SelectListItem { Text = y.ToString(), Value = y.ToString() });
                }
                model.optYear = Convert.ToString(expyr);


                model.lblagreementid = ds.Tables[0].Rows[0]["AGREEMENTHEADERID"].ToString();
                string? APPNOTE = ds.Tables[0].Rows[0]["REFERENCEDOC"].ToString();
                model.appnotelink = "../../Uploads/IOM/" + APPNOTE;
                model.appnotelinkText = APPNOTE;

                string? FINALDOC = ds.Tables[0].Rows[0]["FINALDOC"].ToString();
                model.FINALDOCLINK = "../../Uploads/IOM/" + FINALDOC;
                model.FINALDOCLINKText = FINALDOC;

                //Get Anti Bribery
                string antibribery = ds.Tables[0].Rows[0]["ANTIBRIBERY"].ToString();
                //if (string.IsNullOrEmpty(antibribery.Trim()))
                //{
                //    antibriberylink.Visible = false;
                //    fileantibribery.Visible = true;
                //}
                //else
                //{
                //    antibriberylink.Visible = true;
                //    fileantibribery.Visible = false;
                //    antibriberylink.HRef = "../../Uploads/IOM/" + antibribery;
                //    antibriberylink.InnerText = antibribery;
                //}

                //Get NDA Document
                string ndadocument = ds.Tables[0].Rows[0]["NDADOCUMENT"].ToString();
                //if (string.IsNullOrEmpty(ndadocument.Trim()))
                //{
                //    ndadocumentlink.Visible = false;
                //    filendadocument.Visible = true;
                //}
                //else
                //{
                //    ndadocumentlink.Visible = true;
                //    filendadocument.Visible = false;
                //    ndadocumentlink.HRef = "../../Uploads/IOM/" + ndadocument;
                //    ndadocumentlink.InnerText = ndadocument;
                //}
                //MASTER AGREEMENT
                if (!string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["MASTERAGREEMENT"].ToString()))
                {
                    model.rdomaster = "1";
                    model.ddlmstagreement = Convert.ToString(ds.Tables[0].Rows[0]["MASTERAGREEMENT"]);
                }

                DataSet dsss = _iomContractRepos.GETIOMBYAGREEMENTID(ds.Tables[0].Rows[0]["AGREEMENTHEADERID"].ToString());
                if (dsss.Tables[0].Rows.Count > 0)
                {
                    var grdSource = dsss.Tables[0].AsEnumerable()
                              .Select(row => new grdPrivContractHistory
                              {
                                  IOMID = Convert.ToInt64(row.Field<object>("IOMID")),
                                  AGREEMENTTYPEDESC = Convert.ToString(row.Field<object>("AGREEMENTTYPEDESC")),
                                  VENDORNAME = Convert.ToString(row.Field<object>("VENDORNAME")),
                                  EFFECTIVEDATE = Convert.ToString(row.Field<object>("EFFECTIVEDATE")),
                                  EXPIRYDATE = Convert.ToString(row.Field<object>("EXPIRYDATE")),
                                  processstatusid = Convert.ToInt32(row.Field<object>("processstatusid")),
                                  STATUSNAME = Convert.ToString(row.Field<object>("STATUSNAME")),
                                  REQUESTDBY = Convert.ToString(row.Field<object>("REQUESTDBY")),
                                  finaldoc = Convert.ToString(row.Field<object>("finaldoc"))
                              }).ToList();
                    model.grdPrivContractHistory = grdSource;
                }

            }
            #endregion

            #region GetNextapprovalauthority
            DataSet dss = new DataSet();
            dss = _iomContractRepos.GetNextApprovalAuthority(_userId);
            if (dss.Tables[0].Rows[0]["AUTHLEVEL"].ToString() != "4")
            {
                model.hdauthcode = dss.Tables[0].Rows[0]["ADEMPCODE"].ToString();
                model.hdauthname = dss.Tables[0].Rows[0]["EMPNAME"].ToString();
                model.hdauthemailid = dss.Tables[0].Rows[0]["EMAILID"].ToString();
                model.lbl_appauth = dss.Tables[0].Rows[0]["EMPNAME"].ToString() + "[" + dss.Tables[0].Rows[0]["ADEMPCODE"].ToString() + "]";
                model.hdauthlevel = dss.Tables[0].Rows[0]["AUTHLEVEL"].ToString();
            }
            else
            {
                var ddl_appauthList = new List<SelectListItem>();

                model.hdauthlevel = "4";
                ViewBag.lbl_appauthVisiable = false;
                ViewBag.ddl_appauthVisiable = true;

                if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                {
                    ddl_appauthList = dss.Tables[0].AsEnumerable()
                      .Select(row => new SelectListItem
                      {
                          Text = row["EMPLOYEE"].ToString(),
                          Value = row["ADEMPCODE"].ToString(),
                      }).ToList();
                }
                ddl_appauthList.Insert(0, new SelectListItem("-Select-", ""));
                model.ddl_appauthList = ddl_appauthList;
            }


            #endregion


            return View(model);
        }

        [HttpPost]
        public IActionResult ContractAmendment([FromForm] ContractAmendmentSave model)
        {
            var res = new IOMContractOperationResult();
            res = _iomContractService.ContractAmendmentSave(model);

            return Json(new
            {
                result = res
            });

        }

        [HttpGet]
        public IActionResult VerificationRequestForm()
        {
            VerificationRequestFormViewModel model = new();
            ViewBag.ddlfromEnable = true;

            string? OPHEADID = _empDetails.Operation_Id;
            if (!string.IsNullOrWhiteSpace(OPHEADID))
            {
                ViewBag.ddlfromEnable = false;
                model.ddlfrom = OPHEADID;
            }

            model.txtTo = "Legal & Secretariat";
            //model.rdomaster = "0";
            model.ddlagreementList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Verification", Value = "5" }
            };


            #region FillOperation
            var dsGetAllOperation = _searchEmp.get_AllOperation();
            var ddlFromList = new List<SelectListItem>();
            if (dsGetAllOperation != null && dsGetAllOperation.Tables.Count > 0 && dsGetAllOperation.Tables[0].Rows.Count > 0)
            {
                ddlFromList = dsGetAllOperation.Tables[0].AsEnumerable()
                  .Select(row => new SelectListItem
                  {
                      Text = row["OPERATION"].ToString(),
                      Value = row["OPERATIONID"].ToString(),
                  }).ToList();
            }
            ddlFromList.Insert(0, new SelectListItem
            {
                Text = "-Select Operation-",
                Value = "0"
            });
            model.ddlFromList = ddlFromList;
            #endregion


            #region contract
            var dsGetContract = _iomContractRepos.GETCONTRACTMASTER(string.Empty, string.Empty, "1", string.Empty);
            var ddlcontractList = new List<SelectListItem>();
            if (dsGetContract != null && dsGetContract.Tables.Count > 0 && dsGetContract.Tables[0].Rows.Count > 0)
            {
                ddlcontractList = dsGetContract.Tables[0].AsEnumerable()
                  .Select(row => new SelectListItem
                  {
                      Text = row["CONTRATNAME"].ToString(),
                      Value = row["CONTRACTTYPEID"].ToString()
                  }).ToList();
            }
            ddlcontractList.Insert(0, new SelectListItem
            {
                Text = "-Select Contract-",
                Value = string.Empty
            });
            model.ddlcontractList = ddlcontractList;

            #endregion


            #region PopulateDateTime
            int intVal_day = 1;
            var optDayList = new List<SelectListItem>();
            var optEDList = new List<SelectListItem>();
            var optYearList = new List<SelectListItem>();
            var optEYList = new List<SelectListItem>();


            do
            {
                optDayList.Add(new SelectListItem { Text = intVal_day.ToString(), Value = intVal_day.ToString() });
                optEDList.Add(new SelectListItem { Text = intVal_day.ToString(), Value = intVal_day.ToString() });
                intVal_day = intVal_day + 1;
            }
            while (intVal_day < 32);

            int currentYr = DateTime.Now.Year;
            for (int i = currentYr - 1; i <= (currentYr + 102); i++)
            {
                optYearList.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }
            for (int i = currentYr - 1; i <= (currentYr + 1); i++)
            {
                optEYList.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }
            int intMonth = DateTime.Now.AddDays(15).Month;
            int intDay = DateTime.Now.AddDays(15).Day;
            int intYear = DateTime.Now.AddDays(15).Year;

            model.optDayList = optDayList;
            model.optEDList = optEDList;
            model.optYearList = optYearList;
            model.optEYList = optEYList;

            model.optDay = optDayList.FirstOrDefault(i => i.Value == intDay.ToString())?.Value;
            model.optMonth = model.optMonthList.FirstOrDefault(i => i.Value == intMonth.ToString())?.Value;
            model.optYear = optYearList.FirstOrDefault(i => i.Value == intYear.ToString())?.Value;

            model.optED = optEDList.FirstOrDefault(i => i.Value == intDay.ToString())?.Value;
            model.optEM = model.optEMList.FirstOrDefault(i => i.Value == intMonth.ToString())?.Value;
            model.optEY = optEYList.FirstOrDefault(i => i.Value == intYear.ToString())?.Value;
         
            #endregion

            #region GetNextapprovalauthority
            DataSet dss = new DataSet();
            dss = _iomContractRepos.GetNextApprovalAuthority(_userId);
            if (dss.Tables[0].Rows[0]["AUTHLEVEL"].ToString() != "4")
            {
                model.hdauthcode = dss.Tables[0].Rows[0]["ADEMPCODE"].ToString();
                model.hdauthname = dss.Tables[0].Rows[0]["EMPNAME"].ToString();
                model.hdauthemailid = dss.Tables[0].Rows[0]["EMAILID"].ToString();
                model.lbl_appauth = dss.Tables[0].Rows[0]["EMPNAME"].ToString() + "[" + dss.Tables[0].Rows[0]["ADEMPCODE"].ToString() + "]";
                model.hdauthlevel = dss.Tables[0].Rows[0]["AUTHLEVEL"].ToString();
            }
            else
            {
                var ddl_appauthList = new List<SelectListItem>();

                model.hdauthlevel = "4";
                ViewBag.lbl_appauthVisiable = false;
                ViewBag.ddl_appauthVisiable = true;

                if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                {
                    ddl_appauthList = dss.Tables[0].AsEnumerable()
                      .Select(row => new SelectListItem
                      {
                          Text = row["EMPLOYEE"].ToString(),
                          Value = row["ADEMPCODE"].ToString(),
                      }).ToList();
                }
                ddl_appauthList.Insert(0, new SelectListItem("-Select-", ""));
                model.ddl_appauthList = ddl_appauthList;
            }

            #endregion


            return View(model);
        }

        [HttpPost]
        public IActionResult VerificationRequestForm([FromForm] VerificationRequestFormSave model)
        {
            var res = new IOMContractOperationResult();
            res = _iomContractService.VerificationRequestFormSave(model);

            return Json(new
            {
                result = res
            });

        }


        #region IOMDashboard
        [HttpGet]
        public IActionResult IOMDashboard()
        {
            IOMDashboardViewModel model = new IOMDashboardViewModel();

            model.hdoperationid = _empDetails.Operation_Id;
            model.hddivision = _empDetails.Division_Id;
            model.hddepartment = _empDetails.Department_Id;

            var dsGetContract = _iomContractRepos.GETCONTRACTMASTER(string.Empty, string.Empty, "1", string.Empty);
            var ddlcontractList = new List<SelectListItem>();
            if (dsGetContract != null && dsGetContract.Tables.Count > 0 && dsGetContract.Tables[0].Rows.Count > 0)
            {
                ddlcontractList = dsGetContract.Tables[0].AsEnumerable()
                  .Select(row => new SelectListItem
                  {
                      Text = row["CONTRATNAME"].ToString(),
                      Value = row["CONTRACTTYPEID"].ToString()
                  }).ToList();
            }
            ddlcontractList.Insert(0, new SelectListItem
            {
                Text = "-Select Contract-",
                Value = string.Empty
            });

            model.ddlcontracttypeList = ddlcontractList;

            DataSet dS = _iomContractRepos.GetContractount_contracttype("", model.hdoperationid, model.hddivision, model.hddepartment);
            var grdwipdetailSource = dS.Tables[2];

            if(grdwipdetailSource != null && grdwipdetailSource.Rows.Count > 0)
            {
                model.grdwipdetail = grdwipdetailSource.AsEnumerable()
                             .Select(row => new grdwipdetail
                             {
                                 iomid = Convert.ToString(row.Field<object>("iomid")) ?? string.Empty,
                                 vendorname = Convert.ToString(row.Field<object>("vendorname")),
                                 agrementtype = Convert.ToString(row.Field<object>("agrementtype")),
                                 effectivedate = Convert.ToString(row.Field<object>("effectivedate")),
                                 EXPIRYDATE = Convert.ToString(row.Field<object>("EXPIRYDATE")),
                                 statusname = Convert.ToString(row.Field<object>("statusname")),
                                
                             }).ToList();
            }


            return View(model);
        }


        [HttpPost]
        public IActionResult GetIOMDashboardProgressChartData([FromForm] GetIOMDashboardProgressChartData_DTO dataModel)
        {
            IOMDashboardProgressChartViewModel model = new();

            DataSet dS = _iomContractRepos.GetContractount_contracttype(dataModel.ddlcontracttype, dataModel.hdoperationid, dataModel.hddivision, dataModel.hddepartment);
            var grdPendingcountSource = dS.Tables[0];
            var grdrenpendingSource = dS.Tables[1];

            if (grdPendingcountSource != null && grdPendingcountSource.Rows.Count > 0)
            {
                model.grdPendingcount = grdPendingcountSource.AsEnumerable()
                             .Select(row => new grdPendingcount
                             {
                                 contracttypeid = Convert.ToString(row.Field<object>("contracttypeid")) ?? string.Empty,
                                 ContractType = Convert.ToString(row.Field<object>("ContractType")),
                                 Cnt = Convert.ToString(row.Field<object>("Cnt")),
                             }).ToList();
            }

            if (grdrenpendingSource != null && grdrenpendingSource.Rows.Count > 0)
            {
                model.grdrenpending = grdrenpendingSource.AsEnumerable()
                             .Select(row => new grdrenpending
                             {
                                 contracttypeid = Convert.ToString(row.Field<object>("contracttypeid")) ?? string.Empty,
                                 ContractType = Convert.ToString(row.Field<object>("ContractType")),
                                 Cnt = Convert.ToString(row.Field<object>("Cnt")),
                             }).ToList();
            }


            return Json(new
            {
                result = model
            });
        }

        [HttpPost]
        public IActionResult GetIOMDashboardContractDetailsData([FromForm] IOMDashboardContractDetails_DTO dataModel)
        {
            IOMDashboardContractDetailsViewModel model = new();

            DataSet ds = _iomContractRepos.GetContrac_Detail(dataModel.contracttypeid, dataModel.hdoperationid, dataModel.VENDORNAME);
            var grddashdetailDataSource = new DataTable();
            if(dataModel.HDREQUESTTYPE == "Completed")
            {
                grddashdetailDataSource = ds.Tables[0];
            }
            else if(dataModel.HDREQUESTTYPE == "Pending")
            {
                grddashdetailDataSource = ds.Tables[1];
            }

            if (grddashdetailDataSource != null && grddashdetailDataSource.Rows.Count > 0)
            {
                model.grddashdetail = grddashdetailDataSource.AsEnumerable()
                             .Select(row => new grddashdetail
                             {
                                 ContractType = Convert.ToString(row.Field<object>("ContractType")),
                                 vendorname = Convert.ToString(row.Field<object>("vendorname")),
                                 dateofagreement = Convert.ToString(row.Field<object>("dateofagreement")),
                                 dateofexpiry = Convert.ToString(row.Field<object>("dateofexpiry")),
                                 finaldoc = Convert.ToString(row.Field<object>("finaldoc")),
                             }).ToList();
            }

            model.ddlvendorlist = new();
            if (model.grddashdetail.Any())
            {
                model.ddlvendorlist = model.grddashdetail
                  .Select(row => new SelectListItem
                  {
                      Text = row.vendorname,
                      Value = row.vendorname
                  }).ToList();
            }

            model.ddlvendorlist.Insert(0, new SelectListItem
            {
                Text = "-Select Vendor-",
                Value = string.Empty
            });

            return Json(new
            {
                result = model
            });
        }

        #endregion

    }
}
