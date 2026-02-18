using ePortal.Persistence.SIS.Interface;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.ViewModels.APPX.ISMS;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;


namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    [SessionTimeout]
    public class ISMSController : Controller
    {
        private readonly IISMS _objisms;
        private readonly ISessionService _sessionService;
        private readonly ILogger<HomeController> _logger;
        public ISMSController(IISMS objisms, ISessionService objSessionService, ILogger<HomeController> logger)
        {
            _objisms = objisms;
            _sessionService = objSessionService;
            _logger = logger;
        }

        [HttpGet]
        public JsonResult GetMenu()
        {
            int nettype = Convert.ToInt16(TempData["NetworkType"]);
            TempData.Keep("NetworkType");
            MenuViewModel OBJ = new MenuViewModel();
            List<MenuViewModel> iList = new List<MenuViewModel>();
            List<SIS_ISMS> _SIS_IsmsList = _objisms.GetSisIsmsList();
            List<ISMS_Info_Security> _InfoSecurityList = _objisms.GetInfoSecurityList();
            Employee_Details obj = _sessionService.Get<Employee_Details>("Employee");

            try
            {
                if (_sessionService.Get<string>("userID") != null && _sessionService.Get<string>("userID") != "")
                {
                    if (obj.Operation_Desc == "Strategic Information System")
                    {
                        foreach (var _menu in _SIS_IsmsList.Where(o => o.Active == "1" && o.ISDEFAULTMENU == 0).ToList())
                        //foreach (var _menu in _SIS_IsmsList)
                        {

                            iList.Add(new MenuViewModel
                            {
                                MenuId = _menu.ISMSID,
                                //MenuParentId = _menu.MENU_PARENT_ID,
                                Title = _menu.ISMS_DES,
                                ToolTip = _menu.MENU_TOOLTIP,
                                URL = _menu.MENU_URL,
                                Action = _menu.Active,
                                MenuLevel = _menu.MENU_LEVEL,
                                Controller = _menu.MENU_CONTROLLER,
                                MenuTarget = _menu.MENU_TARGET,
                                MenuIcon = null,
                                MenuContentType = _menu.MENUICON_CONTENT_TYPE

                            });
                            if (obj.Operation_Desc != "Strategic Information System")
                            {
                                foreach (var _submenu in _InfoSecurityList.Where(o => o.Document_type == _menu.ISMS_DES && o.Parent_ID == 2 && o.Active == "1").ToList())
                                {

                                    iList.Add(new MenuViewModel
                                    {

                                        MenuId = _submenu.Proc_id,
                                        MenuParentId = _menu.ISMSID,
                                        Title = _submenu.decription,
                                        ToolTip = _submenu.decription,
                                        URL = _submenu.MENU_URL,
                                        //URL = _submenu.Filename,
                                        Action = _submenu.Active,
                                        MenuLevel = 2,
                                        Controller = "",
                                        MenuTarget = _submenu.MENU_TARGET == "1" ? "_blank" : "",
                                        MenuIcon = null,
                                        MenuContentType = ""
                                    });
                                }

                            }
                            else
                            {
                                foreach (var _submenuhide in _InfoSecurityList.Where(o => o.Document_type == _menu.ISMS_DES && o.Active == "1").ToList())
                                {
                                    iList.Add(new MenuViewModel
                                    {

                                        MenuId = _submenuhide.Proc_id,
                                        MenuParentId = _menu.ISMSID,
                                        Title = _submenuhide.decription,
                                        ToolTip = _submenuhide.decription,
                                        URL = _submenuhide.MENU_URL,
                                        //URL = _submenu.Filename,
                                        Action = "0",
                                        MenuLevel = 2,
                                        Controller = "",
                                        MenuTarget = _submenuhide.MENU_TARGET == "1" ? "_blank" : "",
                                        MenuIcon = null,
                                        MenuContentType = ""
                                    });
                                }
                            }
                        }
                        //}
                    }
                    else
                    {
                        foreach (var _menu in _SIS_IsmsList.Where(o => o.MENU_PARENT_ID != 1).ToList())
                        {

                            iList.Add(new MenuViewModel
                            {
                                MenuId = _menu.ISMSID,
                                MenuParentId = _menu.MENU_PARENT_ID,
                                Title = _menu.ISMS_DES,
                                ToolTip = _menu.MENU_TOOLTIP,
                                URL = _menu.MENU_URL,
                                Action = _menu.Active,
                                MenuLevel = _menu.MENU_LEVEL,

                                Controller = _menu.MENU_CONTROLLER,
                                MenuTarget = _menu.MENU_TARGET,
                                MenuIcon = null,
                                MenuContentType = _menu.MENUICON_CONTENT_TYPE

                            });
                            if (obj.Operation_Desc != "Strategic Information System")
                            {
                                foreach (var _submenu in _InfoSecurityList.Where(o => o.Document_type == _menu.ISMS_DES && o.Parent_ID == 2 && o.Active == "1").ToList())
                                {

                                    iList.Add(new MenuViewModel
                                    {

                                        MenuId = _submenu.Proc_id,
                                        MenuParentId = _menu.ISMSID,
                                        Title = _submenu.decription,
                                        ToolTip = _submenu.decription,
                                        URL = _submenu.MENU_URL,
                                        //URL = _submenu.Filename,
                                        Action = _submenu.Active,
                                        MenuLevel = 2,
                                        Controller = "",
                                        MenuTarget = _submenu.MENU_TARGET == "1" ? "_blank" : "",
                                        MenuIcon = null,
                                        MenuContentType = ""
                                    });
                                }

                            }
                            else
                            {
                                foreach (var _submenuhide in _InfoSecurityList.Where(o => o.Document_type == _menu.ISMS_DES && o.Active == "1").ToList())
                                {
                                    iList.Add(new MenuViewModel
                                    {

                                        MenuId = _submenuhide.Proc_id,
                                        MenuParentId = _menu.ISMSID,
                                        Title = _submenuhide.decription,
                                        ToolTip = _submenuhide.decription,
                                        URL = _submenuhide.MENU_URL,
                                        //URL = _submenu.Filename,
                                        Action = "0",
                                        MenuLevel = 2,
                                        Controller = "",
                                        MenuTarget = _submenuhide.MENU_TARGET == "1" ? "_blank" : "",
                                        MenuIcon = null,
                                        MenuContentType = ""
                                    });
                                }
                            }
                        }
                    }
                    OBJ.SubMenuItems = iList;
                }
            }
            catch (Exception e)
            {
            }

            return Json(OBJ);

        }
        [HttpGet]
        public JsonResult GetAnnoncementList()
        {
            List<string> submenulists = new List<string>();
            DataTable dtannoncement = _objisms.GetISMSAnnonacement();
            dtannoncement = dtannoncement.DefaultView.ToTable();
            ISMSANNOUNANCEVM objannounce = new ISMSANNOUNANCEVM();
            List<ISMSANNOUNANCEVM> listannouncement = new List<ISMSANNOUNANCEVM>();
            if (dtannoncement.Rows.Count > 0)
            {

                foreach (DataRow dd in dtannoncement.Rows)
                {
                    listannouncement.Add(new ISMSANNOUNANCEVM
                    {

                        ANNOUCE_ID = Convert.ToDouble(dd["ANNOUCE_ID"]),
                        DESCRIPTION = dd["DESCRIPTION"].ToString(),
                        DETAILS = dd["DETAILS"].ToString(),
                        ACTIVE = Convert.ToInt32(dd["ACTIVE"]),
                        MENU_URL = dd["LVL"].ToString(),
                        YEARS = dd["YEARS"].ToString(),
                        MONTHS = dd["MONTHS"].ToString(),
                        BANNER_CONTENTTYPE = "",
                        BANNER_NAME = ""
                    });
                }
                objannounce.ISMSANNOUNANCEVMLISTItems = listannouncement;
            }
            string retvalstr = JsonConvert.SerializeObject(objannounce);
            return Json(retvalstr);
        }
        public string ParsePdf(string fileName)
        {
            using (PdfReader reader = new PdfReader(fileName))
            {
                StringBuilder sb = new StringBuilder();

                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                for (int page = 0; page < reader.NumberOfPages; page++)
                {
                    string text = PdfTextExtractor.GetTextFromPage(reader, page + 1, strategy);
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        sb.Append(Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(text))));
                    }
                }

                return sb.ToString();
            }
        }

        [HttpGet]
        public ActionResult ISMSHome()
        {
            DataTable Imgtable = new DataTable();
            Imgtable = _objisms.BindRepeater();
            ViewBag.ImgTable = Imgtable;

            DataTable odt = new DataTable();
            odt = _objisms.MailerofMonth();
            if (odt.Rows.Count > 0)
            {
                ViewBag.lblmmggtext = odt.Rows[0]["category"].ToString();
                ViewBag.lblmmggNavigateUrl = Url.Action("OpenForm", "ISMS",
                    new { FILENAME = odt.Rows[0]["FILE_NAME"].ToString() });
            }
            return View("ISMSHome");
        }
        //protected string Getpath()
        //{
        //    return Url.Content("~/ASPXView/Uploads/UploadSliderImage/");
        //}
        //[HttpGet]
        //public JsonResult GetTextSearch(string prefix)
        //{
        //    List<SISMenuVM> submenulist = new List<SISMenuVM>();
        //    List<string> submenulists = new List<string>();
        //    DataTable dtsubmenupolicy = _objisms.GetDescription_Get(prefix);

        //    if (dtsubmenupolicy.Rows.Count > 0)
        //    {
        //        foreach (DataRow dr in dtsubmenupolicy.Rows)
        //        {
        //            SISMenuVM objssi = new SISMenuVM();
        //            {
        //                objssi.decription = dr["decription"].ToString();
        //                objssi.Proc_id = Convert.ToDouble(dr["PROC_ID"]);
        //            };
        //            submenulist.Add(objssi);
        //        }
        //    }
        //    string retvalstr = JsonConvert.SerializeObject(submenulist);
        //    return Json(retvalstr);
        //}

        //protected void LinkButton2_Click(object sender, EventArgs e)
        //{
        //    ScriptManager.RegisterStartupScript(this, this.GetType(), "HidePopup", "$('#myAnnouncementsModal').modal('hide')", true);
        //}

        //private DataTable Search()
        //{
        //   DataTable odt = _objisms.GetISMSDTL("", ID, "", "");
        //    return (odt);
        //}
        //public DataTable createDataTable()
        //{
        //    DataTable dt = new DataTable();
        //    return dt;
        //}

        [HttpGet]
        public ActionResult lnkmailerofmonth_Click()
        {
            try
            {
                string userid = _sessionService.Get<string>("userID");
                return RedirectToAction("ISMSMailer", "ISMS", new { userID = userid });
            }
            catch (NullReferenceException ex)
            {
                //Response.Write(ex.ToString());
                return Content(ex.ToString());
            }
        }

        [HttpGet]
        public ActionResult lnkfaq_Click()
        {
            string userid = _sessionService.Get<string>("userID");
            return RedirectToAction("ISMS_FAQ", "ISMS", new { userID = userid });
        }

        //-------------ISMS_FAQ-----------------------
        [HttpGet]
        public ActionResult ISMS_FAQ() 
        {
            if (Request.Query["userID"].ToString() != null)
            {
              var faqmodel = BindFAQ();
              var catmodel =  FillCategoryFaq();

                FAQModel model = new FAQModel()
                {
                    FaqViewModel = faqmodel,
                    Categories= catmodel
                };
                return View("ISMS_FAQ", model);
            }
            return View("ISMS_FAQ");
        }
        public List<FaqViewModel> BindFAQ()
        {
            DataTable objDt = new DataTable();
            objDt = _objisms.GetISMSFAQUSER();
            //repAccordian.Visible = true;
            var faqList = new List<FaqViewModel>();
            foreach (DataRow row in objDt.Rows)
            {
                faqList.Add(new FaqViewModel
                {
                    Description = row["DESCRIPTION"].ToString(),
                    Answer = row["ANSWERS"].ToString()
                });
            }
            return faqList;
        }
        private List<FAQCategorViewModel> FillCategoryFaq()
        {
            DataTable dtSite = _objisms.GetCategory("2");
            var categories = dtSite.AsEnumerable()
             .Select(row => new FAQCategorViewModel
             {
                 CatId = Convert.ToInt32(row["CATID"]),
                 Category = row["CATEGORY"].ToString()
             })
             .ToList();

             return categories;
         }
        [HttpPost]
        public IActionResult ISMS_FAQ_SelectedIndexChanged(FAQModel model)
        {
            string strcat = model.SelectedCategoryId;
            if (strcat == "--Select Category--")
                strcat = "";

            var faqList = new List<FaqViewModel>();
            DataTable odt = _objisms.GetISMSFAQ_GETSearch("0", strcat, "", "", "2");
            if (strcat != "")
            {
               
                foreach (DataRow row in odt.Rows)
                {
                    faqList.Add(new FaqViewModel
                    {
                        Description = row["DESCRIPTION"].ToString(),
                        Answer = row["ANSWERS"].ToString()
                    });
                }
            }
            else
            {
                faqList= BindFAQ();
            }
            //model.Categories = FillCategoryFaq();
            //model.FaqViewModel = faqList;
            //return View("ISMS_FAQ", model);

            return PartialView("_FaqAccordionPartial", faqList);
        }

        //-----------ISMSMailer-----------------------

        [HttpGet]
        public ActionResult ISMSMailer()
        {
            if (Request.Query["userID"].ToString() != null)
            {
               var getmaillerdtls = GETMAILLERDETAILS();
               var catmodel = FillCategory_Search();
               var monthmodel =FillMonth();
               var yearmodel= FillYear();
                ISMSMailerModel model = new ISMSMailerModel()
                {
                    GetMaillerDetailsViewModel = getmaillerdtls,
                    CategoryViewModel = catmodel,
                    MonthCategoryViewModel= monthmodel,
                    YearCategoryViewModel= yearmodel
                };
                return View("ISMSMailer", model);
            }
            return View("ISMSMailer");
        }
        public List<GetMaillerDetailsViewModel> GETMAILLERDETAILS()
        {
            DataTable dt = new DataTable();
            dt = _objisms.GetMILLERDetail("0", "", "", "", "", "0");

            var mallerdtls = new List<GetMaillerDetailsViewModel>();
            foreach (DataRow row in dt.Rows)
            {
                mallerdtls.Add(new GetMaillerDetailsViewModel
                {
                    MailerId = Convert.ToInt32(row["MAILLERID"]),
                    FileName = row["FILE_NAME"].ToString(),
                    MailerDescription = row["MAILLER_DES"].ToString(),
                    DateValue = Convert.ToDateTime(row["datevalue"]),
                    MonthYear = row["montyear"].ToString(),
                    Year = Convert.ToInt32(row["YEARS"]),
                    Status = row["status"].ToString()
                });
            }
            return mallerdtls;
        }
        private List<CategoryViewModel> FillCategory_Search()
        {
            DataTable dtSite = _objisms.GetCategory("1");
            var categories = dtSite.AsEnumerable()
            .Select(row => new CategoryViewModel
            {
                CatId = Convert.ToInt32(row["CATID"]),
                CategoryName = row["CATEGORY"].ToString()
            })
            .ToList();

            return categories;
        }
        private List<MonthCategoryViewModel> FillMonth()
        {
            DataTable dtSite = _objisms.GetMonthCategory();

            var monthyear = dtSite.AsEnumerable()
            .Select(row => new MonthCategoryViewModel
            {
                MonthYear = row["montyear"].ToString()
            })
            .ToList();

            return monthyear;
        }
        private List<YearCategoryViewModel> FillYear()
        {
            DataTable dtSite = _objisms.GetMaileryearSearh();

            var year = dtSite.AsEnumerable()
            .Select(row => new YearCategoryViewModel
            {
                Year = row["years"].ToString()
            })
            .ToList();

            return year;
        }

        [HttpPost]
        public IActionResult ddcategorysearhIndexChange(string SelectedCategoryId, string SelectedCatIdText, string SelectedYearText,string SelectedMonthText)
        {
            var getmaillerdtls = new List<GetMaillerDetailsViewModel>();
            if (SelectedCategoryId == "0")
            {
               getmaillerdtls= GETMAILLERDETAILS();
            }
            else
            {
                getmaillerdtls = SearchCATMailler(SelectedCatIdText, SelectedYearText, SelectedMonthText);
            }

            var catmodel = FillCategory_Search();
            var monthmodel = FillMonth();
            var yearmodel = FillYear();

            ISMSMailerModel model = new ISMSMailerModel()
            {
                GetMaillerDetailsViewModel = getmaillerdtls,
                CategoryViewModel = catmodel,
                MonthCategoryViewModel = monthmodel,
                YearCategoryViewModel = yearmodel,
                SelectedCategoryId = SelectedCategoryId,
                SelectedYear = SelectedYearText,
                SelectedMonth=SelectedMonthText
            };

            return View("ISMSMailer", model);
        }
        [HttpPost]
        public IActionResult ddmonth_SelectedIndexChanged(string SelectedMonth, string SelectedCatIdText, string SelectedYearText, string SelectedMonthText, string SelectedCategoryId)
        {
            var getmaillerdtls = new List<GetMaillerDetailsViewModel>();
            if (SelectedMonth == "0")
            {
                getmaillerdtls = GETMAILLERDETAILS();
            }
            else
            {
                getmaillerdtls = SearchCATMailler(SelectedCatIdText, SelectedYearText, SelectedMonthText);
            }

            var catmodel = FillCategory_Search();
            var monthmodel = FillMonth();
            var yearmodel = FillYear();

            ISMSMailerModel model = new ISMSMailerModel()
            {
                GetMaillerDetailsViewModel = getmaillerdtls,
                CategoryViewModel = catmodel,
                MonthCategoryViewModel = monthmodel,
                YearCategoryViewModel = yearmodel,
                SelectedCategoryId = SelectedCategoryId,
                SelectedYear = SelectedYearText,
                SelectedMonth = SelectedMonthText
            };

            return View("ISMSMailer", model);
        }
        [HttpPost]
        public IActionResult ddyear_SelectedIndexChanged(string SelectedYear, string SelectedCatIdText, string SelectedYearText, string SelectedMonthText,string SelectedCategoryId)
        {
            var getmaillerdtls = new List<GetMaillerDetailsViewModel>();
            if (SelectedYear == "0")
            {
                getmaillerdtls = GETMAILLERDETAILS();
            }
            else
            {
                getmaillerdtls = SearchCATMailler(SelectedCatIdText, SelectedYearText, SelectedMonthText);
            }

            var catmodel = FillCategory_Search();
            var monthmodel = FillMonth();
            var yearmodel = FillYear();

            ISMSMailerModel model = new ISMSMailerModel()
            {
                GetMaillerDetailsViewModel = getmaillerdtls,
                CategoryViewModel = catmodel,
                MonthCategoryViewModel = monthmodel,
                YearCategoryViewModel = yearmodel,
                SelectedCategoryId = SelectedCategoryId,
                SelectedYear = SelectedYearText,
                SelectedMonth = SelectedMonthText
            };

            return View("ISMSMailer", model);
        }
        protected List<GetMaillerDetailsViewModel> SearchCATMailler(string SelectedCatIdText, string SelectedYearText, string SelectedMonthText)
        {
            string strDocType = SelectedCatIdText;
            string strmothyear = SelectedMonthText;
            string stryear = SelectedYearText;
            if (strDocType == "--Select Category--")
                strDocType = "";

            if (strmothyear == "--Select Month--")
                strmothyear = "";
            if (stryear == "--Select Year--")
                stryear = "";

           DataTable odt = _objisms.GetISMSMailler_GETSearchUser("0", strDocType.Trim(), stryear, strmothyear, "2");
            var mallerdtls = new List<GetMaillerDetailsViewModel>();
            foreach (DataRow row in odt.Rows)
            {
                mallerdtls.Add(new GetMaillerDetailsViewModel
                {
                    MailerId = Convert.ToInt32(row["MAILLERID"]),
                    FileName = row["FILE_NAME"].ToString(),
                    MailerDescription = row["MAILLER_DES"].ToString(),
                    DateValue = Convert.ToDateTime(row["datevalue"]),
                    MonthYear = row["montyear"].ToString(),
                    Year = Convert.ToInt32(row["YEARS"]),
                    Status = row["status"].ToString()
                });
            }
            return mallerdtls;
        }

        //----------------------OpenForm------------------
        [HttpGet]
        public IActionResult OpenForm()
        {
            string filePath = string.Empty;

            if (Request.Query["FILENAME"].ToString() != null)
            {
                string strisms = string.Empty;

                string v = Request.Query["FILENAME"].ToString();
                if (v != null)
                {
                    strisms = Request.Query["FILENAME"].ToString();
                    filePath = serverpath.getFileUploadPath() + "ISMS//" + strisms;
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