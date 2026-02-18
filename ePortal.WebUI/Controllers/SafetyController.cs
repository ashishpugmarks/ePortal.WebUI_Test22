using DocumentFormat.OpenXml.InkML;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Interface;
using ePortal.ViewModels.APPX.Safety;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using ePortal.ViewModels;
//using ePortal.ViewModels.APPX.IOM;
//using ePortal.ViewModels.APPX.VQMS_DPR;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    [SessionTimeout]
    public class SafetyController : Controller
    {
        private readonly ISafety objsafety;
        private readonly ICommonFunctions objCommon;
        private readonly IAD_ITEM_MASTER Obj_AD_ITEM_MASTER;
        private readonly ISessionService _sessionService;
        private readonly ILogger<HomeController> _logger;
        public SafetyController(ISafety Safety,ICommonFunctions CommonFunctions, IAD_ITEM_MASTER AD_ITEM_MASTER, ISessionService objSessionService, ILogger<HomeController> logger)
        {
            objsafety = Safety;
            objCommon = CommonFunctions;
            Obj_AD_ITEM_MASTER = AD_ITEM_MASTER;
            _sessionService = objSessionService;
            _logger = logger;
        }

        //------------SafetyContract-----------------
        [HttpGet]
        public IActionResult SafetyContract()
        {
            String strImg = String.Empty;
            String strActive = String.Empty;
            DataSet ds = new DataSet();
            DataRow[] _datarow;
            String UserSecID = String.Empty;
            String UserDptID = String.Empty;
            String UserDivID = String.Empty;
            String UserVpID = String.Empty;
            String strCode = String.Empty;

            strCode = _sessionService.Get<string>("userID");
            DataTable dtUserDetail = objCommon.GetEmployeeOfficialDetails(strCode, "");
            UserSecID = dtUserDetail.Rows[0]["ADSECTIONID"].ToString();
            UserDptID = dtUserDetail.Rows[0]["ADDEPARTMENTID"].ToString();
            UserDivID = dtUserDetail.Rows[0]["ADDIVISIONID"].ToString();
            UserVpID = dtUserDetail.Rows[0]["ADVPID"].ToString();

            var modelPlant=PopulatePlant();
            DataTable dtplant = Obj_AD_ITEM_MASTER.GetUser_Detail(strCode);
            DataRow dr = dtplant.Rows[0];
            string strPlantID = dr["syplantid"].ToString();
            modelPlant.SelectedPlantId = strPlantID; 
            modelPlant.Enabled = false;
            var modelCategory=FillCategory(strPlantID);
            var modelAct=FillAct(strPlantID);
            var modelSfSection=FillSfsection(strPlantID);
            //GetContracts();

            SafetyModel model = new SafetyModel
            {
                PlantViewModel = modelPlant,
                SfCategoryViewModel = modelCategory,
                ActViewModel = modelAct,
                SfSectionViewModel = modelSfSection
            };

           return View("SafetyContract", model);
        }

        // search button click
        [HttpPost]
        public IActionResult cmdSearch_Click(SearchContractsViewModel filters)
        {
            String UserSecID = String.Empty;
            String UserDptID = String.Empty;
            String UserDivID = String.Empty;
            String UserVpID = String.Empty;

            String strCode = String.Empty;

            strCode = _sessionService.Get<string>("userID");
            DataTable dtUserDetail = objCommon.GetEmployeeOfficialDetails(strCode, "");
            UserSecID = dtUserDetail.Rows[0]["ADSECTIONID"].ToString();
            UserDptID = dtUserDetail.Rows[0]["ADDEPARTMENTID"].ToString();
            UserDivID = dtUserDetail.Rows[0]["ADDIVISIONID"].ToString();
            UserVpID = dtUserDetail.Rows[0]["ADVPID"].ToString();

            var contractDtls = new List<ContractGridViewModel>();
            DataTable DT;
            DT = objsafety.GetContract("", filters.Category, filters.Lact, filters.Cmbstatus, UserSecID, UserDptID, UserDivID, UserVpID, filters.FromDate, filters.ToDate, filters.SfSection, filters.Plant);

                foreach (DataRow row in DT.Rows)
                {
                    contractDtls.Add(new ContractGridViewModel
                    {
                        SFContracts = row["SFCONTRACTS"].ToString(),
                        CategoryDes = row["CATEGORYDES"].ToString(),
                        ActDes = row["ACTDES"].ToString(),
                        Requirement = row["REQUIRMENT"].ToString(),
                        SFSection = row["SFSECTION"].ToString(),
                        RespName = row["RESPNAME"].ToString(),
                        FrqDes = row["FRQDES"].ToString(),
                        ExpiryDate = row["EXPIRYDATE"].ToString(),
                        DueDate = row["DUEDATE"].ToString(),
                        CmpStatus = row["CMPSTATUS"].ToString(),
                        Remark = row["REMARK"].ToString(),
                        Attachment = row["ATTACHMENT"].ToString(),
                        AttachFileSubmit = row["ATTACH_FILESUBMIT"].ToString(),
                        EncryptedId = Encryption.Encrypt(row["SFCONTRACTS"].ToString())
                    });
                }
            return Json(contractDtls);
        }
        //public void ValueHiddenField_ValueChanged(Object sender, EventArgs e)
        //{
        //    try
        //    {

        //        GetContracts();
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}
        #region"Function"
        public SfSectionViewModel FillSfsection(string strPlantID)
        {
            DataTable dt = new DataTable();
            dt = dt = objsafety.Get_SFDepartment(strPlantID);
            var sectionList = dt.AsEnumerable()
                        .Select(row => new SelectListItem
                        {
                            Text = row["SFSECDESCRIP"].ToString(),
                            Value = row["SFSECTIONID"].ToString()
                        })
                        .ToList();

            // Insert default option
            sectionList.Insert(0, new SelectListItem { Text = "--Select--", Value = "" });

            return new SfSectionViewModel
            {
                Sections = sectionList
            };


        }
        //public List<ContractGridViewModel> GetContracts()
        //{
        //    DataTable dt;
        //    dt = objsafety.GetContract("", ddlcategory.SelectedValue.ToString(), ddlact.SelectedValue.ToString(), cmbstatus.SelectedValue.ToString(), UserSecID, UserDptID, UserDivID, UserVpID, txtFromDate.Text, txtToDate.Text, ddlsfsection.SelectedValue.ToString(), ddlplant.SelectedValue.ToString());
        //    grdcnotract.DataSource = dt;
        //    grdcnotract.DataBind();
        //}
        //private string GetCurrentPage()
        //{
        //    Uri uri = Request.Url;
        //    string[] segment = uri.Segments;
        //    string page = string.Empty;
        //    if (0 < segment.Length)
        //    {
        //        page = segment[segment.Length - 1];
        //    }
        //    return page;
        //}
        //public String GetEncryptedString(String strNormalString)
        //{
        //    return Server.UrlEncode(Encryption.Encrypt(strNormalString));
        //}
        public SfCategoryViewModel FillCategory(string strPlantID)
        {
            DataTable dt;
            dt = objsafety.Get_Category("", "", "1", strPlantID);
            var categoryList = dt.AsEnumerable()
                         .Select(row => new SelectListItem
                         {
                             Text = row["DESCRIP"].ToString(),
                             Value = row["SFCATEGORYID"].ToString()
                         })
                         .ToList();

            categoryList.Insert(0, new SelectListItem { Text = "--Select--", Value = "" });

            return new SfCategoryViewModel
            {
                Categories = categoryList
            };

        }
        public ActViewModel FillAct(string strPlantID)
        {
            DataTable dt = new DataTable();
            dt = objsafety.Get_Act("", "", "1", strPlantID);
            var actList = dt.AsEnumerable()
                    .Select(row => new SelectListItem
                    {
                        Text = row["DESCRIP"].ToString(),
                        Value = row["SFACTID"].ToString()
                    })
                    .ToList();
            actList.Insert(0, new SelectListItem { Text = "--Select--", Value = "" });

            return new ActViewModel
            {
                Acts = actList
            };
        }
        public PlantViewModel PopulatePlant()
        {
            DataTable dt;
            dt = objCommon.GetPlant();

            var plantList = dt.AsEnumerable()
                      .Select(row => new SelectListItem
                      {
                          Text = row["PLANTNAME"].ToString(),
                          Value = row["SYPLANTID"].ToString()
                      })
                      .ToList();

            plantList.Insert(0, new SelectListItem { Text = "--Select--", Value = "-1" });

            var vm = new PlantViewModel
            {
                Plants = plantList
            };
            return vm;

        }
        #endregion

        //---------------ReminderDetail---------------
        [HttpGet]
        public IActionResult ReminderDetail()
        {
            string strContractId = Convert.ToString(Request.Query["cid"]);
            strContractId = Encryption.Decrypt(strContractId);
            string strUserId = _sessionService.Get<string>("userID");
            var model = new List<ReminderDetail>();
            DataTable objdt = objsafety.GetReminderdDetail(strContractId);

            foreach (DataRow row in objdt.Rows)
            {
                model.Add(new ReminderDetail
                {
                    Remarks = row["REMARKS"].ToString(),
                    Status = row["STATUS"].ToString(),
                    AddedBy = row["EMPNAME"].ToString(),
                    AddedDate = row["ADDEDDATE"].ToString(),
                   
                });
            }
            return View("ReminderDetail", model);

        }

        //-----------SafetyReport----------------------
        [HttpGet]
        public IActionResult SafetyReport()
        {
            Employee_Details Emp = new Employee_Details();
            Emp = _sessionService.Get<Employee_Details>("Employee");
            var modelPlant = PopulatePlant();
            DataTable dtplant = Obj_AD_ITEM_MASTER.GetUser_Detail(_sessionService.Get<string>("userID"));
            DataRow dr = dtplant.Rows[0];
            string strPlantID = dr["syplantid"].ToString();
            modelPlant.SelectedPlantId = strPlantID;
            modelPlant.Enabled = false;
            var modelSfSection = FillSfsection(strPlantID);
            //btnSearch_OnClick(sender, e);

            SafetyModel model = new SafetyModel
            {
                PlantViewModel = modelPlant,
                SfSectionViewModel = modelSfSection
            };
            return View("SafetyReport", model);
        }
        [HttpPost]
        public IActionResult btnSearch_OnClick(string ddlsfsection, string ddlplant)
        {
            string strCode = _sessionService.Get<string>("userID");
            DataTable dtUserDetail = objCommon.GetEmployeeOfficialDetails(strCode, "");
            string UserSecID = dtUserDetail.Rows[0]["ADSECTIONID"].ToString();
            string UserDptID = dtUserDetail.Rows[0]["ADDEPARTMENTID"].ToString();
            string UserDivID = dtUserDetail.Rows[0]["ADDIVISIONID"].ToString();
            string UserVpID = dtUserDetail.Rows[0]["ADVPID"].ToString();

            var pendingCnt = new List<PendingViewModel>();
            DataTable dtDashCount = objsafety.GetDashBoardCount(ddlsfsection, ddlplant, UserSecID, UserDptID, UserDivID, UserVpID);
            //grdPendingcount.DataSource = dtDashCount;
            //grdPendingcount.DataBind();

            foreach (DataRow row in dtDashCount.Rows)
            {
                pendingCnt.Add(new PendingViewModel
                {
                    Status = row["status"].ToString(),
                    Count = row["Cnt"].ToString(),
                   
                });
            }

            var chartData = dtDashCount.AsEnumerable()
                           //.Where(r => r.Field<int?>("Cnt") != null && r.Field<int>("Cnt") != 0)
                           .Where(r => r["Cnt"] != DBNull.Value && Convert.ToDecimal(r["Cnt"]) != 0)
                           .Select(r => new ChartItem
                           {
                               Status = r["STATUS"].ToString(),
                               //Count = Convert.ToInt32(r["Cnt"])
                               Count = Convert.ToInt32(Convert.ToDecimal(r["Cnt"]))
                           }).ToList();

            var result = new SafetyModel
            {
                PendingViewModel = pendingCnt,
                ChartData = chartData
            };
            return Json(result);
        }
        [HttpPost]
        public IActionResult grdPendingcount_RowCommand(string ddlsfsection, string ddlplant, string strstatus)
        {
            string strCode = _sessionService.Get<string>("userID");
            DataTable dtUserDetail = objCommon.GetEmployeeOfficialDetails(strCode, "");
            string UserSecID = dtUserDetail.Rows[0]["ADSECTIONID"].ToString();
            string UserDptID = dtUserDetail.Rows[0]["ADDEPARTMENTID"].ToString();
            string UserDivID = dtUserDetail.Rows[0]["ADDIVISIONID"].ToString();
            string UserVpID = dtUserDetail.Rows[0]["ADVPID"].ToString();

            var pendingCnt = new List<DashboardDetail>();
            DataTable dtdashdetail = objsafety.GetDashBoardDetail(ddlsfsection, ddlplant, UserSecID, UserDptID, UserDivID, UserVpID, strstatus);
            //grddashdetail.DataSource = dtdashdetail;
            //grddashdetail.DataBind();
            foreach (DataRow row in dtdashdetail.Rows)
            {
                pendingCnt.Add(new DashboardDetail
                {
                    CategoryDes = row["CATEGORYDES"].ToString(),
                    ActDes = row["ACTDES"].ToString(),
                    Requirement = row["REQUIRMENT"].ToString(),
                    ExpiryDate = row["EXPIRYDATE"].ToString(),
                    ResponsiblePerson = row["RESNAME"].ToString(),
                    RemStatus = row["REMSTATUS"].ToString(),
                });
            }
            return Json(pendingCnt);
        }

        //----------------------OpenForm------------------
        [HttpGet]
        public IActionResult OpenDocument()
        {

            if (Request.Query["SF_ID"].ToString() != null)
            {
                string strid = string.Empty;
                string strattach = string.Empty;
                string filePath = string.Empty;
                DataTable dt = new DataTable();

                strid = Request.Query["SF_ID"].ToString();
                strattach = Request.Query["attach"].ToString();

                dt = objsafety.GetContract(strid, "", "", "", "", "", "", "", "", "", "", "");
                if (strattach == "1")
                {
                    filePath = serverpath.getFileUploadPath() + "Safety\\" + dt.Rows[0]["ATTACHMENT"].ToString();
                }
                if (strattach == "2")
                {
                    filePath = serverpath.getFileUploadPath() + "Safety\\" + dt.Rows[0]["ATTACH_FILESUBMIT"].ToString();
                }
                if (strattach == "3")
                {
                    filePath = serverpath.getFileUploadPath() + "Safety\\" + dt.Rows[0]["APPROVALATTCHMENT"].ToString();
                }

                String sFullPath = String.Empty;
                sFullPath = filePath;

                if (sFullPath != null && sFullPath.Length > 4)
                {
                    if (sFullPath.Contains("\\"))
                    {
                        try
                        {
                            FileInfo _fileInfo = new FileInfo(sFullPath);
                            if (_fileInfo.Exists)
                            {
                                //Clear the whatever
                                //Response.Buffer = false;
                                //Response.Clear();
                                String sContentType = "";
                                // Determine the content type
                                switch (_fileInfo.Extension.ToLower())
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
                                    case ".docx":
                                        sContentType = "Application/vnd.ms-word";
                                        break;
                                    case ".pptx":
                                        sContentType = "Application/vnd.ms-powerpoint";
                                        break;
                                    case ".ppt":
                                        sContentType = "Application/vnd.ms-powerpoint";
                                        break;
                                    case ".rtf":
                                        sContentType = "Application/vnd.ms-word";
                                        break;
                                    case ".txt":
                                        sContentType = "Application/vnd.text";
                                        break;
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

                                //return File(filePath, sContentType, Path.GetFileName(filePath));
                                return File(System.IO.File.ReadAllBytes(filePath), sContentType, _fileInfo.Name);
                            }
                        }
                        catch (Exception ex)
                        {
                            return View("OpenForm");
                        }
                    }
                    return View("OpenForm");
                }
                return View("OpenForm");
            }
            return View("OpenForm");
        }
    }
}
