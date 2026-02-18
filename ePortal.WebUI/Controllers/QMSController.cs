using Microsoft.AspNetCore.Mvc;
using ePortal.WebUI.Filters;
using ePortal.Application.APPX.Contracts;
using ePortal.ViewModels;
using ePortal.Shared.Interface;
using ePortal.Persistence.Admin.Interface;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using ePortal.Persistence.Interface;
using ePortal.ViewModels.APPX.QMS;
using System.Dynamic;
using System.Text;
using ePortal.Shared;



namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    public class QMSController : Controller
    {
        private readonly ILogger<QMSController> _logger;
        private readonly IQMSService _QMSService;
        private readonly IQMSRepos _QMSRepos;
        private readonly ISessionService _sessionService;
        private readonly ISearchEmp _searchEmp;
        private readonly ICommonFunctions _objCom;

        private String CssClass_QcProc1 = String.Empty;
        private bool CssStyle_QcProc1 = false;


        private readonly Employee_Details _empDetails;
        private readonly string _userId;
        public QMSController(
            ISessionService sessionService,
            IQMSService QMSService,
            ISearchEmp searchEmp,
            IQMSRepos QMSRepos,
            ICommonFunctions objCom,
            ILogger<QMSController> logger)
        {
            _logger = logger;
            _sessionService = sessionService;
            _QMSService = QMSService;
            _QMSRepos = QMSRepos;
            _searchEmp = searchEmp;
            _objCom = objCom;


            _empDetails = _sessionService?.Get<Employee_Details>("Employee");
            _userId = _sessionService.Get<string>("userID").ToString();
            
        }

        [HttpGet]
        public IActionResult ISODoc()
        {
            ISODocViewModel model = new();

            #region Plant
            var dtPlant = _objCom.GetPlant();
            var ddlPlantList = new List<SelectListItem>();
            if (dtPlant != null && dtPlant.Rows.Count > 0)
            {
                ddlPlantList = dtPlant.AsEnumerable()
                  .Select(row => new SelectListItem
                  {
                      Text = row["PLANTNAME"].ToString(),
                      Value = row["SYPLANTID"].ToString(),
                  }).ToList();
            }
            ddlPlantList.Insert(0, new SelectListItem
            {
                Text = "-Select Plant-",
                Value = ""
            });
            model.ddlPlantList = ddlPlantList;
            #endregion

            #region Department
            var dsddlDeptList = _searchEmp.get_AllDept();
            var ddlDeptList = new List<SelectListItem>();
            if (dsddlDeptList != null && dsddlDeptList.Tables.Count > 0 && dsddlDeptList.Tables[0].Rows.Count > 0)
            {
                ddlDeptList = dsddlDeptList.Tables[0].AsEnumerable()
                  .Select(row => new SelectListItem
                  {
                      Text = row["department"].ToString(),
                      Value = row["departmentid"].ToString(),
                  }).ToList();
            }
            ddlDeptList.Insert(0, new SelectListItem
            {
                Text = "-Select Department-",
                Value = ""
            });
            model.ddlDeptList = ddlDeptList;

            #endregion

            #region Section
            var dsddlSectionList = _searchEmp.get_AllSection();
            var ddlSectionList = new List<SelectListItem>();
            if (dsddlSectionList != null && dsddlSectionList.Tables.Count > 0 && dsddlSectionList.Tables[0].Rows.Count > 0)
            {
                ddlSectionList = dsddlSectionList.Tables[0].AsEnumerable()
                  .Select(row => new SelectListItem
                  {
                      Text = row["section"].ToString(),
                      Value = row["sectionid"].ToString(),
                  }).ToList();
            }
            ddlSectionList.Insert(0, new SelectListItem
            {
                Text = "-Select Section-",
                Value = ""
            });
            model.ddlSectionList = ddlSectionList;

            #endregion

            #region BindDay
            for (int i = 1; i <= 31; i++)
            {
                model.ddlDayAList.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                model.ddlDayDList.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                model.ddlImpDayRList.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                model.ddlRevDayRList.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }

            int k = System.DateTime.Now.Year - 1;//changes....
            for (int j = 2014; j <= k + 2; j++)
            {
                model.ddlYearAList.Add(new SelectListItem { Text = j.ToString(), Value = j.ToString() });
                model.ddlImpYearRList.Add(new SelectListItem { Text = j.ToString(), Value = j.ToString() });
                model.ddlYearDList.Add(new SelectListItem { Text = j.ToString(), Value = j.ToString() });
            }

            int L = 2013;//changes....
            for (int M = L; M <= System.DateTime.Now.Year + 1; M++)
            {
                model.ddlRevYearRList.Add(new SelectListItem { Text = M.ToString(), Value = M.ToString() });
            }

            string CY = DateTime.Now.Year.ToString();
            model.ddlYearA = CY;
            model.ddlImpYearR = CY;
            model.ddlYearD = CY;
            model.ddlRevYearR = CY;
            #endregion

            #region AppAuthority
            string strChangetype = string.Empty;
            if (model.IsChecked_rbtnAdd)
                strChangetype = "1";
            else if (model.IsChecked_rbtnRev)
                strChangetype = "2";
            else
                strChangetype = "3";


            DataTable dt = _QMSRepos.Get_DeptHead(_userId, strChangetype);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                model.lblAppAuth = dr["EMPNAME"] + " (For Approval)";
                model.hdAPPECODE = dr["ADEMPCODE"].ToString();
                model.hdAPPEMAILID = dr["EMAILID"].ToString();
            }
            else
            {
                model.lblAppAuth = "No Authority is set for Approval]";
            }
            #endregion

            #region FormatMaster
            DataSet objDS = _QMSRepos.GETQMSFORMATNUMBERLIST("", "", "");
            var ddlformatnumberList = new List<SelectListItem>();
            if (objDS != null && objDS.Tables.Count > 0 && objDS.Tables[0].Rows.Count > 0)
            {
                ddlformatnumberList = objDS.Tables[0].AsEnumerable()
                  .Where(x => Convert.ToString(x.Field<object>("ACTIVE")) == "1")
                  .Select(row => new SelectListItem
                  {
                      Text = row["FORMATNUMBER"].ToString(),
                      Value = row["FORMATNUMBER"].ToString(),
                  }).ToList();
            }
            ddlformatnumberList.Insert(0, new SelectListItem
            {
                Text = "Select",
                Value = ""
            });
            model.ddlformatnumberList = ddlformatnumberList;
            #endregion
            model.txtNewRevNoR = "";

            return View(model);
        }

        [HttpPost]
        public IActionResult ISODoc([FromForm]ISODocSaveModel model)
        {
            var res = new QMSOperationResult();
            res = _QMSService.ISODOCDetail_Set(model);
            return Json(new
            {
                result = res
            });
        }

        [HttpPost]
        public IActionResult BindTextChanged_ISODoc([FromForm] BindTextChangedISO_DTO dto)
        {

            string ddlformatnumber = dto.ddlformatnumber;
            string txtDocNo9 = dto.txtDocNo9;
            string txtDocNo = dto.txtDocNo;
            string txtDocNo1 = dto.txtDocNo1;
            string txtDocNo2 = dto.txtDocNo2;
            bool IsChecked_rbtnAdd = dto.IsChecked_rbtnAdd;
            bool IsChecked_rbtnRev = dto.IsChecked_rbtnRev;
            bool IsChecked_rbtnDel = dto.IsChecked_rbtnDel;


            dynamic model = new ExpandoObject();
            dynamic subModel = new ExpandoObject();
            string strDocno = string.Empty;
            strDocno = ddlformatnumber + txtDocNo9 + txtDocNo + txtDocNo1 + txtDocNo2;
            strDocno = strDocno.ToUpper();
            if (string.IsNullOrEmpty(ddlformatnumber) || string.IsNullOrEmpty(txtDocNo9) || string.IsNullOrEmpty(txtDocNo) || string.IsNullOrEmpty(txtDocNo1) || string.IsNullOrEmpty(txtDocNo2))
            { }
            else
            {
                var objDt = _QMSRepos.ISODOCDetail_Get_Detail(strDocno);
                if (objDt.Rows.Count > 0)
                {
                    objDt.DefaultView.RowFilter = "DOCNO='" + strDocno + "'and Status=1 and CHANGETYPE not in('Deletion')";
                    objDt.DefaultView.Sort = "isodocid";
                    objDt = objDt.DefaultView.ToTable();

                    if (objDt.Rows.Count > 0)
                    {
                        DataRow dr = objDt.Rows[0];
                        string IsoDocId = Convert.ToString(dr["ISODOCID"]);
                        model.IsoDocId = IsoDocId;
                        if (IsChecked_rbtnAdd)
                        {
                            subModel = new ExpandoObject();
                            subModel = new
                            {
                                IsWindowAlert = true,
                                message = $"This Document no. {ddlformatnumber} - {txtDocNo9.ToUpper()} - {txtDocNo.ToUpper()} - {txtDocNo1.ToUpper()} - {txtDocNo2.ToUpper()} is alreday Exists. Please select Revision for this.",
                                IsoDocId = IsoDocId,

                                ddlformatnumber = "",
                                txtDocNo9 = string.Empty,
                                txtDocNo = string.Empty,
                                txtDocNo1 = string.Empty,
                                txtDocNo2 = string.Empty,
                                txtDocTitle = string.Empty,
                                txtRevNoA = string.Empty,
                                txtDocTitleReadOnly = false,
                            };

                            return Json(new
                            {
                                result = subModel
                            });

                        }
                        else if (IsChecked_rbtnRev)
                        {

                            objDt.DefaultView.RowFilter = "DOCNO='" + strDocno + "'and APPAUTHSTATUS=1 and ISOAPPSTATUS=1 and ISOHEADAPPSTATUS=1 and Status=1 and CHANGETYPE not in('Deletion')";
                            objDt = objDt.DefaultView.ToTable();
                            if (objDt.Rows.Count > 0)
                            {
                                model.txtCurrRevNoR = Convert.ToString(dr["NEWREVISIONNO"]);
                                model.hdnCurrRevNoR = Convert.ToString(dr["NEWREVISIONNO"]);    //textbox disabled so use.
                                string strdateOld = Convert.ToString(dr["NEWREVISIONDATE"]);

                                model.ddlRevDayR = Convert.ToString(Convert.ToDateTime(strdateOld).Day);
                                model.ddlRevMonthR = Convert.ToString(Convert.ToDateTime(strdateOld).Month);
                                model.ddlRevYearR = Convert.ToString(Convert.ToDateTime(strdateOld).Year);


                                int a = Convert.ToInt32(dr["NEWREVISIONNO"]) + 1;

                                if (a.ToString().Length == 1)
                                {
                                    model.txtNewRevNoR = "0" + a.ToString();
                                    model.hdnNewRevNoR = "0" + a.ToString();
                                }
                                else
                                {
                                    model.txtNewRevNoR = a.ToString();
                                    model.hdnNewRevNoR = a.ToString();
                                }


                                model.txtDocTitleReadOnly = true;

                                model.ddlRevDayREnabled = false;
                                model.ddlRevMonthREnabled = false;
                                model.ddlRevYearREnabled = false;
                            }
                            else
                            {
                                subModel = new ExpandoObject();
                                subModel = new
                                {
                                    IsWindowAlert = true,
                                    message = $"This Document no. {ddlformatnumber} - {txtDocNo9.ToUpper()} - {txtDocNo.ToUpper()} - {txtDocNo1.ToUpper()} - {txtDocNo2.ToUpper()} is pending for approval.",
                                    IsoDocId = IsoDocId,

                                    ddlformatnumber = "",
                                    txtDocNo9 = string.Empty,
                                    txtDocNo = string.Empty,
                                    txtDocNo1 = string.Empty,
                                    txtDocNo2 = string.Empty,
                                    txtDocTitle = string.Empty,
                                    txtRevNoA = string.Empty,
                                    txtDocTitleReadOnly = false,
                                    txtNewRevNoR = string.Empty,
                                    hdnNewRevNoR = string.Empty,

                                };

                                return Json(new
                                {
                                    result = subModel
                                });

                            }
                        }
                        else
                        {
                            objDt.DefaultView.RowFilter = "DOCNO='" + strDocno + "'and APPAUTHSTATUS=1 and ISOAPPSTATUS=1 AND ISOHEADAPPSTATUS=1 and Status=1 and CHANGETYPE not in('Deletion')";
                            objDt = objDt.DefaultView.ToTable();
                            if (objDt.Rows.Count > 0)
                            {
                                model.txtCurrRevD = Convert.ToString(dr["NEWREVISIONNO"]);
                                model.hdnCurrRevNoR = Convert.ToString(dr["NEWREVISIONNO"]);
                                model.txtDocTitleReadOnly = true;
                            }
                            else
                            {

                                subModel = new ExpandoObject();
                                subModel = new
                                {
                                    IsWindowAlert = true,
                                    message = $"This Document no. {ddlformatnumber} - {txtDocNo9.ToUpper()} - {txtDocNo.ToUpper()} - {txtDocNo1.ToUpper()} - {txtDocNo2.ToUpper()} does not Exists.",
                                    IsoDocId = IsoDocId,

                                    ddlformatnumber = "",
                                    txtDocNo9 = string.Empty,
                                    txtDocNo = string.Empty,
                                    txtDocNo1 = string.Empty,
                                    txtDocNo2 = string.Empty,
                                    txtDocTitle = string.Empty,
                                    txtRevNoA = string.Empty,
                                    txtDocTitleReadOnly = false,
                                    txtNewRevNoR = string.Empty,
                                    hdnNewRevNoR = string.Empty,

                                };

                                return Json(new
                                {
                                    result = subModel
                                });
                            }
                        }

                        model.txtDocTitle = Convert.ToString(dr["DOCTITLE"]);
                        model.hdnDocTitle = Convert.ToString(dr["DOCTITLE"]);
                        model.ddlDept = Convert.ToString(dr["DEPARTMENT"]);
                        model.ddlPlant = Convert.ToString(dr["PLANT"]);             //IF PLANT SECTION VALUE NOT COME
                        model.ddlSection = Convert.ToString(dr["SECTION"]);
                        string strfilee = Convert.ToString(dr["FILENAME"]);
                        if (!string.IsNullOrEmpty(strfilee))
                        {
                            model.FileListVisible = true;
                            model.FilenameExist = strfilee;
                            model.hdnFile = strfilee;       // hidden field for file validation.
                        }

                    }
                    else
                    {
                        if (IsChecked_rbtnAdd)
                        {
                            model.txtDocTitleReadOnly = false;
                        }
                        else if (IsChecked_rbtnRev)
                        {
                            model.txtDocTitleReadOnly = true;
                            model.ddlRevDayREnabled = false;
                            model.ddlRevMonthREnabled = false;
                            model.ddlRevYearREnabled = false;
                        }
                        else
                        {
                            model.txtDocTitleReadOnly = true;
                        }

                        model.txtDocTitle = string.Empty;
                        model.txtRevNoA = string.Empty;
                        model.txtCurrRevNoR = string.Empty;
                        model.txtCurrRevD = string.Empty;
                        model.ddlRevDayR = "1";
                        model.ddlRevMonthR = "1";
                        model.ddlRevYearRSelectedIndex = 0; 
                    }
                }
                else if (IsChecked_rbtnRev == true || IsChecked_rbtnDel == true)
                {
                    subModel = new ExpandoObject();
                    subModel = new
                    {
                        IsWindowAlert = true,
                        message = $"This Document no. {ddlformatnumber} - {txtDocNo9.ToUpper()} - {txtDocNo.ToUpper()} - {txtDocNo1.ToUpper()} - {txtDocNo2.ToUpper()} does not Exists. Please select Addition for this.",

                        ddlformatnumber = "",
                        txtDocNo9 = string.Empty,
                        txtDocNo = string.Empty,
                        txtDocNo1 = string.Empty,
                        txtDocNo2 = string.Empty,
                        txtDocTitle = string.Empty,
                        txtRevNoA = string.Empty,
                        txtDocTitleReadOnly = false,
                        txtNewRevNoR = string.Empty,
                        hdnNewRevNoR = string.Empty,
                    };

                    return Json(new
                    {
                        result = subModel
                    });
                }
            }

            return Json(new
            {
                result = model
            });
        }

        [HttpPost]
        public IActionResult AppAuthority([FromForm] AppAuthority_DTO dto)
        {
            string strChangetype = string.Empty;
            if (dto.IsChecked_rbtnAdd)
                strChangetype = "1";
            else if (dto.IsChecked_rbtnRev)
                strChangetype = "2";
            else
                strChangetype = "3";

            DataTable dt = _QMSRepos.Get_DeptHead(_userId, strChangetype);
            string lblAppAuth = string.Empty;
            string APPECODE = string.Empty;
            string APPEMAILID = string.Empty;
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                lblAppAuth= dr["EMPNAME"] + " (For Approval)";
                APPECODE = dr["ADEMPCODE"].ToString();
                APPEMAILID = dr["EMAILID"].ToString();
            }
            else
            {
                lblAppAuth = "No Authority is set for Approval]";
            }

            dynamic model = new ExpandoObject();
            model = new 
            {
                lblAppAuth = lblAppAuth,
                APPECODE = APPECODE,
                APPEMAILID = APPEMAILID
            };


            return Json(new
            {
                result = model
            });
        }

        [HttpGet]
        public IActionResult ISODocEdit([FromQuery]string id)
        {
            ISODocEditViewModel model = new();
            #region Plant
            var dtPlant = _objCom.GetPlant();
            var ddlPlantList = new List<SelectListItem>();
            if (dtPlant != null && dtPlant.Rows.Count > 0)
            {
                ddlPlantList = dtPlant.AsEnumerable()
                  .Select(row => new SelectListItem
                  {
                      Text = row["PLANTNAME"].ToString(),
                      Value = row["SYPLANTID"].ToString(),
                  }).ToList();
            }
            ddlPlantList.Insert(0, new SelectListItem
            {
                Text = "-Select Plant-",
                Value = ""
            });
            model.ddlPlantList = ddlPlantList;
            #endregion

            #region Department
            var dsddlDeptList = _searchEmp.get_AllDept();
            var ddlDeptList = new List<SelectListItem>();
            if (dsddlDeptList != null && dsddlDeptList.Tables.Count > 0 && dsddlDeptList.Tables[0].Rows.Count > 0)
            {
                ddlDeptList = dsddlDeptList.Tables[0].AsEnumerable()
                  .Select(row => new SelectListItem
                  {
                      Text = row["department"].ToString(),
                      Value = row["departmentid"].ToString(),
                  }).ToList();
            }
            ddlDeptList.Insert(0, new SelectListItem
            {
                Text = "-Select Department-",
                Value = ""
            });
            model.ddlDeptList = ddlDeptList;

            #endregion

            #region Section
            var dsddlSectionList = _searchEmp.get_AllSection();
            var ddlSectionList = new List<SelectListItem>();
            if (dsddlSectionList != null && dsddlSectionList.Tables.Count > 0 && dsddlSectionList.Tables[0].Rows.Count > 0)
            {
                ddlSectionList = dsddlSectionList.Tables[0].AsEnumerable()
                  .Select(row => new SelectListItem
                  {
                      Text = row["section"].ToString(),
                      Value = row["sectionid"].ToString(),
                  }).ToList();
            }
            ddlSectionList.Insert(0, new SelectListItem
            {
                Text = "-Select Section-",
                Value = ""
            });
            model.ddlSectionList = ddlSectionList;

            #endregion

            #region BindDay
            for (int i = 1; i <= 31; i++)
            {
                model.ddlDayAList.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                model.ddlDayDList.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                model.ddlImpDayRList.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                model.ddlRevDayRList.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }

            int k = System.DateTime.Now.Year;
            for (int j = 2014; j <= k + 2; j++)
            {
                model.ddlYearAList.Add(new SelectListItem { Text = j.ToString(), Value = j.ToString() });
                model.ddlImpYearRList.Add(new SelectListItem { Text = j.ToString(), Value = j.ToString() });
                model.ddlRevYearRList.Add(new SelectListItem { Text = j.ToString(), Value = j.ToString() });
                model.ddlYearDList.Add(new SelectListItem { Text = j.ToString(), Value = j.ToString() });
            }

            #endregion

            #region FormatMaster
            DataSet objDS = _QMSRepos.GETQMSFORMATNUMBERLIST("", "", "");
            var ddlformatnumberList = new List<SelectListItem>();
            if (objDS != null && objDS.Tables.Count > 0 && objDS.Tables[0].Rows.Count > 0)
            {
                ddlformatnumberList = objDS.Tables[0].AsEnumerable()
                  .Where(x => Convert.ToString(x.Field<object>("ACTIVE")) == "1")
                  .Select(row => new SelectListItem
                  {
                      Text = row["FORMATNUMBER"].ToString(),
                      Value = row["FORMATNUMBER"].ToString(),
                  }).ToList();
            }
            ddlformatnumberList.Insert(0, new SelectListItem
            {
                Text = "Select",
                Value = ""
            });
            model.ddlformatnumberList = ddlformatnumberList;
            #endregion

          

            #region BindQmsDoc
            string RequestID = id;
            model.hdIsoDocId = id;
            if (!string.IsNullOrEmpty(RequestID))
            {
                DataTable objDt = _QMSRepos.ISODOCDetail_Get(RequestID);
                if (objDt.Rows.Count > 0)
                {
                    DataRow dr = objDt.Rows[0];
                    model.ddlPlant = Convert.ToString(dr["PLANT"]);
                    model.ddlDept = Convert.ToString(dr["DEPARTMENT"]);
                    model.ddlSection = Convert.ToString(dr["SECTION"]);

                    model.OldIsoDocId = Convert.ToString(dr["PARENTID"]);
                    model.changeType_rdomaster = Convert.ToString(dr["CHANGETYPE"]);
                    model.ChangeType = Convert.ToString(dr["CHANGETYPE"]);
                    string ChangeType = model.ChangeType ?? string.Empty;

                    if (ChangeType == "Addition")
                    {
                        model.IsChecked_rbtnAdd = true;
                        model.IsChecked_rbtnRev = false;
                        model.IsChecked_rbtnDel = false;

                        model.txtRevNoA = Convert.ToString(dr["NEWREVISIONNO"]);
                        string strdatee = Convert.ToString(dr["NEWREVISIONDATE"]);
                        model.ddlDayA = Convert.ToString(Convert.ToDateTime(strdatee).Day);
                        model.ddlMonthA = Convert.ToString(Convert.ToDateTime(strdatee).Month);
                        model.ddlYearA = Convert.ToString(Convert.ToDateTime(strdatee).Year);

                    }
                    else if (ChangeType == "Revision")
                    {
                        model.IsChecked_rbtnAdd = false;
                        model.IsChecked_rbtnRev = true;
                        model.IsChecked_rbtnDel = false;

                        model.txtCurrRevNoR = Convert.ToString(dr["OLDREVISIONNO"]);
                        model.txtNewRevNoR = Convert.ToString(dr["NEWREVISIONNO"]);
                        string strdateOld = Convert.ToString(dr["OLDREVISIONDATE"]);

                        model.ddlRevDayR = Convert.ToString(Convert.ToDateTime(strdateOld).Day);
                        model.ddlRevMonthR = Convert.ToString(Convert.ToDateTime(strdateOld).Month);
                        model.ddlRevYearR = Convert.ToString(Convert.ToDateTime(strdateOld).Year);

                        string strdateNew = Convert.ToString(dr["NEWREVISIONDATE"]);
                        model.ddlImpDayR = Convert.ToString(Convert.ToDateTime(strdateNew).Day);
                        model.ddlImpMonthR = Convert.ToString(Convert.ToDateTime(strdateNew).Month);
                        model.ddlImpYearR = Convert.ToString(Convert.ToDateTime(strdateNew).Year);
                       
                    }
                    else
                    {
                        model.IsChecked_rbtnAdd = false;
                        model.IsChecked_rbtnRev = false;
                        model.IsChecked_rbtnDel = true;

                        model.txtCurrRevD = Convert.ToString(dr["NEWREVISIONNO"]);
                        string strdatee = Convert.ToString(dr["NEWREVISIONDATE"]);
                        model.ddlDayD = Convert.ToString(Convert.ToDateTime(strdatee).Day);
                        model.ddlMonthD = Convert.ToString(Convert.ToDateTime(strdatee).Month);
                        model.ddlYearD = Convert.ToString(Convert.ToDateTime(strdatee).Year);

                    }

                    string docno = Convert.ToString(dr["DOCNO"]);
                    model.ddlformatnumber = docno.Substring(0, 7);
                    model.txtDocNo9 = docno.Substring(7, 4);
                    model.txtDocNo = docno.Substring(11, 1);
                    model.txtDocNo1 = docno.Substring(12, 2);
                    model.txtDocNo2 = docno.Substring(14, 4);

                    model.txtDocTitle = Convert.ToString(dr["DOCTITLE"]);
                    model.txtReason = Convert.ToString(dr["REQEMPREMARK"]);
                    model.lblAppAuth = Convert.ToString(dr["APPAUTH"]);

                    string strfilee = Convert.ToString(dr["FILENAME"]);
                    if (!string.IsNullOrEmpty(strfilee))
                    {
                        model.hdFilenameExist = strfilee;
                        model.hdnFile = strfilee;
                    }
                }

            }

            #endregion

            #region AppAuthority
            string strChangetype = string.Empty;
            if (model.IsChecked_rbtnAdd)
                strChangetype = "1";
            else if (model.IsChecked_rbtnRev)
                strChangetype = "2";
            else
                strChangetype = "3";

            DataTable dt = _QMSRepos.Get_DeptHead(_userId, strChangetype);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                //model.lblAppAuth = dr["EMPNAME"] + " (For Approval)";
                model.hdAPPECODE = dr["ADEMPCODE"].ToString();
                model.hdAPPEMAILID = dr["EMAILID"].ToString();
            }
            else
            {
                model.lblAppAuth = "No Authority is set for Approval]";
            }
            #endregion

            return View(model);
        }

       
        [HttpPost]
        public IActionResult ISODocEdit([FromForm] ISODocUpdateModel model)
        {
            var res = new QMSOperationResult();
            res = _QMSService.ISODOCDetail_UPDATE(model);
            return Json(new
            {
                result = res
            });
        }

        [HttpPost]
        public IActionResult BindTextChanged_ISODocEdit([FromForm] BindTextChangedISO_DTO dto)
        {
            string ddlformatnumber = dto.ddlformatnumber;
            string txtDocNo9 = dto.txtDocNo9;
            string txtDocNo = dto.txtDocNo;
            string txtDocNo1 = dto.txtDocNo1;
            string txtDocNo2 = dto.txtDocNo2;
            bool IsChecked_rbtnAdd = dto.IsChecked_rbtnAdd;
            bool IsChecked_rbtnRev = dto.IsChecked_rbtnRev;
            bool IsChecked_rbtnDel = dto.IsChecked_rbtnDel;


            dynamic model = new ExpandoObject();
            dynamic subModel = new ExpandoObject();

            string strDocno = string.Empty;
            strDocno = ddlformatnumber + txtDocNo9 + txtDocNo + txtDocNo1 + txtDocNo2;
            strDocno = strDocno.ToUpper();
            if (string.IsNullOrWhiteSpace(ddlformatnumber) || string.IsNullOrWhiteSpace(txtDocNo9) || string.IsNullOrWhiteSpace(txtDocNo) || string.IsNullOrWhiteSpace(txtDocNo1) || string.IsNullOrWhiteSpace(txtDocNo2))
            { }
            else
            {
                DataTable objDt = _QMSRepos.ISODOCDetail_Get(string.Empty);
                if (objDt.Rows.Count > 0)
                {
                    objDt.DefaultView.RowFilter = "DOCNO='" + strDocno + "'and APPAUTHSTATUS=1 and ISOAPPSTATUS=1 AND ISOHEADAPPSTATUS=1 and Status=1 and CHANGETYPE not in('Deletion')";
                    objDt.DefaultView.Sort = "isodocid";
                    objDt = objDt.DefaultView.ToTable();

                    if (objDt.Rows.Count > 0)
                    {
                        DataRow dr = objDt.Rows[0];
                        string IsoDocId = Convert.ToString(dr["ISODOCID"]);
                        model.IsoDocId = IsoDocId;
                        if (IsChecked_rbtnAdd)
                        {
                            subModel = new ExpandoObject();
                            subModel = new
                            {
                                IsWindowAlert = true,
                                message = $"This Document no. {ddlformatnumber} - {txtDocNo9.ToUpper()} - {txtDocNo.ToUpper()} - {txtDocNo1.ToUpper()} - {txtDocNo2.ToUpper()} is alreday Exists. Please select Revision for this.",
                                IsoDocId = IsoDocId,

                                ddlformatnumber = "",
                                txtDocNo9 = string.Empty,
                                txtDocNo = string.Empty,
                                txtDocNo1 = string.Empty,
                                txtDocNo2 = string.Empty,
                                txtDocTitle = string.Empty,
                                txtRevNoA = string.Empty,
                                txtDocTitleReadOnly = false,
                            };

                            return Json(new
                            {
                                result = subModel
                            });
                        }
                        else if (IsChecked_rbtnRev)
                        {
                            model.txtCurrRevNoR = Convert.ToString(dr["NEWREVISIONNO"]);
                            model.hdnCurrRevNoR = Convert.ToString(dr["NEWREVISIONNO"]);    //textbox disabled so use.
                            string strdateOld = Convert.ToString(dr["NEWREVISIONDATE"]);

                            model.ddlRevDayR = Convert.ToString(Convert.ToDateTime(strdateOld).Day);
                            model.ddlRevMonthR = Convert.ToString(Convert.ToDateTime(strdateOld).Month);
                            model.ddlRevYearR = Convert.ToString(Convert.ToDateTime(strdateOld).Year);

                            int a = Convert.ToInt32(dr["NEWREVISIONNO"]) + 1;

                            if (a.ToString().Length == 1)
                            {
                                model.txtNewRevNoR = "0" + a.ToString();
                                model.hdnNewRevNoR = "0" + a.ToString();
                            }
                            else
                            {
                                model.txtNewRevNoR = a.ToString();
                                model.hdnNewRevNoR = a.ToString();
                            }

                            model.txtDocTitleReadOnly = true;

                            model.ddlRevDayREnabled = false;
                            model.ddlRevMonthREnabled = false;
                            model.ddlRevYearREnabled = false;

                        }
                        else
                        {
                            
                            model.txtCurrRevD = Convert.ToString(dr["NEWREVISIONNO"]);
                            model.hdnCurrRevNoR = Convert.ToString(dr["NEWREVISIONNO"]);
                            model.txtDocTitleReadOnly = true;
                        }
                        model.txtDocTitle = Convert.ToString(dr["DOCTITLE"]);
                        model.hdnDocTitle = Convert.ToString(dr["DOCTITLE"]);
                        model.ddlDept = Convert.ToString(dr["DEPARTMENT"]);
                        model.ddlPlant = Convert.ToString(dr["PLANT"]);             //IF PLANT SECTION VALUE NOT COME
                        model.ddlSection = Convert.ToString(dr["SECTION"]);

                    }
                    else
                    {
                        if (IsChecked_rbtnAdd)
                        {
                            model.txtDocTitleReadOnly = false;
                        }
                        else if (IsChecked_rbtnRev)
                        {
                            model.txtDocTitleReadOnly = true;
                            model.ddlRevDayREnabled = false;
                            model.ddlRevMonthREnabled = false;
                            model.ddlRevYearREnabled = false;
                        }
                        else
                        {
                            model.txtDocTitleReadOnly = false;
                        }

                        model.txtDocTitle = string.Empty;
                        model.txtCurrRevNoR = string.Empty;
                        //model.txtRevNoA = string.Empty;
                        model.txtCurrRevD = string.Empty;
                        model.ddlRevDayR = "1";
                        model.ddlRevMonthR = "1";
                        model.ddlRevYearRSelectedIndex = 0;
                    }
                }
            }

            return Json(new
            {
                result = model
            });
        }


        [HttpGet]
        public IActionResult ISODocRequestDetail([FromQuery]string id)
        {
            ISODocRequestDetailViewModel model = new();
            string strTransid = id;
            if (!string.IsNullOrWhiteSpace(strTransid))
            {
                DataTable dt = new DataTable();
                dt = _QMSRepos.GetRequestDetail(strTransid);
                if (dt.Rows.Count > 0)
                {
                    model.ltlReqID = dt.Rows[0]["ISODOCID"].ToString();
                    model.ltlEmployee = dt.Rows[0]["EMPNAME"].ToString();
                    model.ltlAppDate = dt.Rows[0]["DATEADDED"].ToString();
                    model.ltlReqStatus = dt.Rows[0]["Status"].ToString();

                    string strDocNo = Convert.ToString(dt.Rows[0]["DOCNO"]);
                    strDocNo = strDocNo.Substring(0, 7) + "-" + strDocNo.Substring(7, 4) + "-" + strDocNo.Substring(11, 1) + "-" + strDocNo.Substring(12, 2) + "-" + strDocNo.Substring(14, 4);
                    model.ltldocno = strDocNo;
                    model.ltldoctitle = dt.Rows[0]["DOCTITLE"].ToString();
                    model.ltlchangetype = dt.Rows[0]["CHANGETYPE"].ToString();
                    model.ltldepartment = dt.Rows[0]["DEPARTMENTNAME"].ToString();

                    model.ltlAppStatus = dt.Rows[0]["APPAUTHSTATUS"].ToString();
                    model.ltlStatus = dt.Rows[0]["ADMINSTATUS"].ToString();

                    model.ltlAppAuth = dt.Rows[0]["APPAUTHNAME"].ToString();
                    model.ltlDate = dt.Rows[0]["APPAUTHDATE"].ToString();
                    model.ltlAuthEmail = dt.Rows[0]["APPAUTHEMAILID"].ToString();
                    model.ltlAuthRem = dt.Rows[0]["APPAUTHREMARK"].ToString();


                    model.ltlAdminAuth = dt.Rows[0]["ADMINNAME"].ToString();

                    model.ltlAdminRem = dt.Rows[0]["ADMINREMARKS"].ToString();
                    model.ltlAdminAppDate = dt.Rows[0]["ADMINAPPDATE"].ToString();
                    model.ltladminemail = dt.Rows[0]["ADMINEMAILID"].ToString();

                    //Admin Head Details
                    model.ltladminheadauth = dt.Rows[0]["ADMINHEADNAME"].ToString();
                    model.ltladminheademail = dt.Rows[0]["ADMINHEADEMAILID"].ToString();
                    model.ltladminheadstatus = dt.Rows[0]["ADMINHEADSTATUS"].ToString();
                    model.ltladminheadremarks = dt.Rows[0]["ADMINHEADREMARKS"].ToString();
                    model.ltladminheaddate = dt.Rows[0]["ADMINHEADAPPDATE"].ToString();
                }
            }
            

            return View(model);
        }

        [HttpGet]
        public IActionResult ISODocRequestAppReject([FromQuery]string? Cancelid = null, [FromQuery]string? Appid = null)
        {
            ISODocRequestAppRejectViewModel model = new();

            ViewBag.FileListVisible = false;

            #region BindQmsDoc

            string RequestID = string.Empty;

            model.RequestIDC = Cancelid;
            model.RequestIDA = Appid;
            if (!string.IsNullOrWhiteSpace(model.RequestIDC))
            {
                RequestID = Cancelid;
                ViewBag.pnlCancelVisible = true;
                ViewBag.pnlApprovalVisible = false;
                model.lblHeader = "QMS Document Change Request For Cancellation";
            }
            else
            {
                RequestID = Appid;
                ViewBag.pnlCancelVisible = false;
                ViewBag.pnlApprovalVisible = true;
                model.lblHeader = "QMS Document Change Request For Approval";
            }

            model.IsoDocId = RequestID;

            if (!string.IsNullOrEmpty(RequestID))
            {
                DataTable objDt = _QMSRepos.ISODOCDetail_Get(RequestID);
                if (objDt.Rows.Count > 0)
                {
                    DataRow dr = objDt.Rows[0];
                    model.lblEmpName = Convert.ToString(dr["EmpName"]);
                    model.EMPEMAIL = Convert.ToString(dr["EMPEMAIL"]);
                    model.lblEmpCode = Convert.ToString(dr["REQEMPCODE"]);
                    model.lblMobile = Convert.ToString(dr["TMOBILE"]);
                    model.lblRequestDate = Convert.ToString(dr["DATEADDED"]);

                    model.lblPlant = Convert.ToString(dr["PLANTNAME"]);
                    model.hdnplant = Convert.ToString(dr["PLANT"]);
                    model.lblDept = Convert.ToString(dr["DepartmentName"]);

                    string str = Convert.ToString(dr["CHANGETYPE"]);
                    model.chg = str ?? string.Empty;
                    model.changeType_rdomaster = str;
                    if (str == "Addition")
                    {

                        model.lblRevisionNoA = Convert.ToString(dr["NEWREVISIONNO"]);
                        string strdatee = Convert.ToString(dr["NEWREVISIONDATE"]);
                        model.lblImpDateA = strdatee;
                    }
                    else if (str == "Revision")
                    {

                        model.lblCurrRevisonR = Convert.ToString(dr["OLDREVISIONNO"]);
                        model.lblNewRevisonR = Convert.ToString(dr["NEWREVISIONNO"]);
                        string strdateOld = Convert.ToString(dr["OLDREVISIONDATE"]);
                        model.lblRevisonDateR = strdateOld;
                        string strdateNew = Convert.ToString(dr["NEWREVISIONDATE"]);
                        model.lblImpDateR = strdateNew;
                    }
                    else
                    {

                        model.lblCurrRevisonD = Convert.ToString(dr["NEWREVISIONNO"]);
                        string strdatee = Convert.ToString(dr["NEWREVISIONDATE"]);
                        model.lblImpDateD = strdatee;
                    }

                    string docno = Convert.ToString(dr["DOCNO"]);
                    model.lblDocNo = docno.Substring(0, 7) + "-" + docno.Substring(7, 4) + "-" + docno.Substring(11, 1) + "-" + docno.Substring(12, 2) + "-" + docno.Substring(14, 4);
                    model.lblDocTitle = Convert.ToString(dr["DOCTITLE"]);
                    model.lblReason = Convert.ToString(dr["REQEMPREMARK"]);

                    string strfilee = Convert.ToString(dr["FILENAME"]);
                    if (!string.IsNullOrEmpty(strfilee))
                    {
                        ViewBag.FileListVisible = true;
                        model.hdFilenameExist = strfilee;
                    }
                }
            }

            #endregion


            if (model.chg == "Addition")
            {
                ViewBag.pnlAdditonVisible = true;
                ViewBag.pnlDeletionVisible = false;
                ViewBag.pnlRevisonVisible = false;
            }
            else if (model.chg == "Revision")
            {
                ViewBag.pnlAdditonVisible = false;
                ViewBag.pnlDeletionVisible = false;
                ViewBag.pnlRevisonVisible = true;
            }
            else
            {
                ViewBag.pnlAdditonVisible = false;
                ViewBag.pnlDeletionVisible = true;
                ViewBag.pnlRevisonVisible = false;
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult ISODocRequestAppReject([FromForm]ISODocRequestAppRejectSaveModel model)
        {
            var res = new QMSOperationResult();
            res = _QMSService.ISODocRequestAppReject(model);
            return Json(new
            {
                result = res
            });
        }

        [HttpGet]
        public IActionResult QcProc1()
        {
            QcProc1ViewModel model = new();

            DataSet ds = _QMSRepos.getQmsstandards("", "1");
            DataTable dt = ds.Tables[0];
            dt.DefaultView.RowFilter = "PARENTID=0";
            DataView dv = dt.DefaultView;

            StringBuilder strhtml = new StringBuilder();
            CssStyle_QcProc1 = false;
            int i = 0;
            foreach (DataRow rw in dv.ToTable().Rows)
            {
                i = i + 1;
                CssClass_QcProc1 = (CssStyle_QcProc1 == true ? "row1" : "row2");

                strhtml.Append("<tr class='"+ CssClass_QcProc1 + "' >")
                .Append("<td style='width:10%; text-align:center'>"+ i.ToString() + "</td>")
                .Append("<td style='width:70%; text-align:left; color:#0000FF'>")
                .Append("<a class='p1' data-filename='"+ rw["FILENAME"].ToString() + "' data-qcqmsdocid='"+ rw["QCQMSDOCID"].ToString() + "' >" + rw["DESCRIPTION"].ToString() + " </a>")
                .Append("</td>")
                .Append("<td style='width:20%; text-align:left'>"+ rw["REMARKS"].ToString() + "</td>")
                .Append("</tr>");
              
                CssStyle_QcProc1 = !CssStyle_QcProc1;
                if (rw["FILENAME"].ToString() != "")
                {
                    var sb = getStandardHtml(rw["QCQMSDOCID"].ToString(), dt, i.ToString(), CssStyle_QcProc1);
                    if(sb != null && sb.Length > 0)
                    {
                        strhtml.Append(sb.ToString());
                    }
                }
            }
            model.strhtml = strhtml;

            #region BindPlant
            List<SelectListItem> ddlPlantList = new();
            DataTable dtPlant = _objCom.GetPlant();
            if (dtPlant != null && dtPlant.Rows.Count > 0)
            {
                ddlPlantList = dtPlant.AsEnumerable()
                  .Select(row => new SelectListItem
                  {
                      Text = row["PLANTNAME"].ToString(),
                      Value = row["SYPLANTID"].ToString(),
                  }).ToList();
            }
            ddlPlantList.Insert(0, new SelectListItem
            {
                Text = "-ALL PLANT-",
                Value = ""
            });
            model.ddlPlantList = ddlPlantList;
            #endregion

            #region BindDepartment
            List<SelectListItem> ddlDeptList = new();
            DataTable dtDept = _QMSRepos.Get_ISODOCDEPARTMENT(model.ddlPlant ?? string.Empty);
            if (dtDept != null && dtDept.Rows.Count > 0)
            {
                ddlDeptList = dtDept.AsEnumerable()
                  .Select(row => new SelectListItem
                  {
                      Text = row["department"].ToString(),
                      Value = row["departmentid"].ToString(),
                  }).ToList();
            }
            ddlDeptList.Insert(0, new SelectListItem
            {
                Text = "-ALL Department-",
                Value = ""
            });
            model.ddlDeptList = ddlDeptList;
            #endregion

            model.QCPROCSESSION = _sessionService.Get<string>("QCPROCSESSION");
            if(model.QCPROCSESSION != null)
            {
               _sessionService.Remove("QCPROCSESSION");
            }

            return View(model);
        }

        private StringBuilder getStandardHtml(string strlvlID, DataTable dt, string rowseq, bool CssStyle)
        {
            StringBuilder strhtml = new StringBuilder();
            DataRow[] dr = dt.Select("PARENTID=" + strlvlID);
            decimal i = Convert.ToDecimal(rowseq);
            foreach (DataRow rw in dr)
            {
                i += 0.1m;
                CssClass_QcProc1 = (CssStyle == true ? "row1" : "row2");

                strhtml.Append("<tr class='" + CssClass_QcProc1 + "' >")
                .Append("<td style='width:10%; padding-left:42px'>" + i.ToString("0.0") + "</td>")
                .Append("<td style='width:70%; padding-left:25px;  text-align:left; color:#0000FF'>")
                .Append("<a class='p1' data-filename='" + rw["FILENAME"].ToString() + "' data-qcqmsdocid='" + rw["QCQMSDOCID"].ToString() + "' >" + rw["DESCRIPTION"].ToString() + " </a>")
                .Append("</td>")
                .Append("<td style='width:20%; text-align:left'>" + rw["REMARKS"].ToString() + "</td>")
                .Append("</tr>");

                CssStyle = !CssStyle;

                var sb = getStandardHtml(rw["QCQMSDOCID"].ToString(), dt, i.ToString(), CssStyle);
                if(sb != null && sb.Length > 0)
                {
                    strhtml.Append(sb.ToString());
                }
            }
            return strhtml;
        }

        [HttpPost]
        public IActionResult GetgrdDeptProcList([FromForm] GetgrdDeptProcList_DTO dto)
        {
            var list = _QMSService.GrdDeptProcList(dto.plant, dto.DEPTID);
            List<SelectListItem> ddlDept = new();
            if(dto.isChangedPlant)
            {
                DataTable dt = _QMSRepos.Get_ISODOCDEPARTMENT(dto.plant ?? string.Empty);
                if (dt != null && dt.Rows.Count > 0)
                {
                    ddlDept = dt.AsEnumerable()
                     .Select(row => new SelectListItem
                     {
                         Text = row["department"].ToString(),
                         Value = row["departmentid"].ToString(),
                     }).ToList();
                }

                ddlDept.Insert(0, new SelectListItem
                {
                    Text = "-ALL Department-",
                    Value = ""
                });
            }

            return Json(new
            {
                result = list,
                ddlDept = ddlDept
            });
        }

        [HttpGet]
        public IActionResult SubQcProc([FromQuery]string ID)
        {
            IEnumerable<SubQcProcViewModel> model = _QMSService.SubQcProc(ID);
            return View(model);
        }

        [HttpPost]
        public IActionResult QC_Proc1_QCPROCSESSION([FromForm] QCPROCSESSION dto)
        {
            string QCPROCSESSION_data = dto.DEPARTMENTID + "#" + dto.PLANTID+ "#" + dto.FILTERPLANT + "#" + dto.FILTERDEPTID + "#" + dto.DEPTNAME + "#" + dto.GRIDPAGENO;
            _sessionService.Set<string>("QCPROCSESSION", QCPROCSESSION_data);
            
            return Json(new
            {
               Status = 200
            });
        }



        [HttpGet]
        public IActionResult DeptSectProc()
        {
            string? _QCPROCSESSION = _sessionService.Get<string>("QCPROCSESSION");
            if (_QCPROCSESSION != null)
            {
                string[] sessionresult = _QCPROCSESSION.Split(new Char[] { '#' });

                string DEPTID = sessionresult[0];
                string PLANTID = sessionresult[1];
                IEnumerable<DeptSectProcViewModel> model = _QMSService.DeptSectProcList(DEPTID, PLANTID);
                return View(model);
            }
            return Redirect("/home/home");
        }

        [HttpGet]
        public IActionResult Qmsdetail()
        {
            IEnumerable<QmsdetailViewModel> modelList = new List<QmsdetailViewModel>();

            var dsgetQmsDetails = _QMSRepos.getQmsDetails1();
            DataTable dt = dsgetQmsDetails.Tables[0];

            if(dt != null && dt.Rows.Count > 0)
            {
                modelList = dt.AsEnumerable()
                  .Select(x => new QmsdetailViewModel
                  {
                      FILENAME = Convert.ToString(x.Field<object>("FILENAME")),
                      APPLICABLEDATE = Convert.ToString(x.Field<object>("APPLICABLEDATE")),
                  }).ToList();
            }


            return View(modelList);
        }

        [HttpGet]
        public IActionResult opendocument([FromQuery]string? docid, [FromQuery] string? FILENAME, [FromQuery] string? FILENAME1)
        {
            string strhcgid = string.Empty;
            string filePath = string.Empty;

            string? v = FILENAME;
            if (!string.IsNullOrWhiteSpace(v))
            {
                strhcgid = FILENAME ?? string.Empty;
                filePath = Path.Combine(serverpath.getFileUploadPath(), "QMS", strhcgid);
            }
            string? x = FILENAME1;
            if (!string.IsNullOrWhiteSpace(x))
            {
                strhcgid = FILENAME1 ?? string.Empty;
                filePath = Path.Combine(serverpath.getFileUploadPath(), "HMSIStandard", strhcgid);
            }
            string? y = docid;
            if (!string.IsNullOrWhiteSpace(y))
            {
                strhcgid = docid ?? string.Empty;
                filePath = Path.Combine(serverpath.getFileUploadPath(), "QMS", strhcgid);
            }
            string sFullPath = filePath;

            var _fileInfo = new FileInfo(sFullPath);
            if (!_fileInfo.Exists)
                return NotFound();

            string sContentType;
            switch ((_fileInfo.Extension ?? string.Empty).ToLowerInvariant())
            {
                case ".dwf":
                    sContentType = "Application/x-dwf";
                    break;
                case ".pdf":
                    sContentType = "Application/pdf";
                    break;
                case ".doc":
                    sContentType = "Application/vnd.ms-word";
                    break;
                case ".ppt":
                case ".pps":
                    sContentType = "Application/vnd.ms-powerpoint";
                    break;
                case ".xls":
                    sContentType = "Application/vnd.ms-excel";
                    break;
                default:
                    // Catch-all content type, let the browser figure it out
                    sContentType = "Application/octet-stream";
                    break;
            }

            Response.Headers.Remove("Content-Disposition");
            Response.Headers.Remove("Content-Length");

            Response.Headers["Content-Disposition"] = $"filename={_fileInfo.Name}";
            Response.Headers["Content-Length"] = _fileInfo.Length.ToString();
            Response.ContentType = sContentType;

            return PhysicalFile(_fileInfo.FullName, sContentType);
        }


    }
}
