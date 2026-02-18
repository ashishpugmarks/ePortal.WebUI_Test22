using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    [SessionTimeout]
    public class CalenderMasterController : Controller
    {
        private readonly ICalenderMaster _objCalenderMasterService;
        private readonly ISessionService _sessionService;
        private readonly ILogger<CalenderMasterController> _logger;
        private readonly IAppConfigurationService _env;

        public CalenderMasterController(ICalenderMaster objCalenderMasterService, ISessionService sessionService, ILogger<CalenderMasterController> logger, IAppConfigurationService env)
        {
            _objCalenderMasterService = objCalenderMasterService;
            _sessionService = sessionService;
            _logger = logger;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }      

        public ActionResult CalenderMaster()
        {
            var model = new MigratedPageViewModel
            {
                Title = "Calendar",
                NewUrl = "https://newapp.example.com/asr/calendar",
                Message = "This page has been migrated to the Success Master."
            };
            return View("MigratedMessage", model);

            //IEnumerable<SYSITE> SYSiteItems = _objCalenderMasterService.Bind_SYSite();
            //ViewBag.SYSITE = new MultiSelectList(SYSiteItems, "SYSITEID", "DESCRIP");
            //CalenderMasterViewModel viewModel = new CalenderMasterViewModel();
            //// Populate FinancialYears property here
            ////viewModel.FinancialYears = GetFinancialYearsFrom(2009);
            //viewModel.FinancialYears = GetFinancialYears(2022, 3);
            //CalenderSearchViewModel searchViewModel = new CalenderSearchViewModel();
            //viewModel.CalenderMaster = _objCalenderMasterService.GetCalenderMasterList(viewModel.SearchViewModel);
            ////if(viewModel.CalenderMaster.Count==0)
            ////{
            ////    viewModel.CalenderMaster = "No Record Found";
            ////}
            //return View("CalenderMaster", viewModel);

        }

        public ActionResult CalenderMasterForUser()
        {
            var model = new MigratedPageViewModel
            {
                Title = "Calendar",
                NewUrl = "https://newapp.example.com/asr/calendar",
                Message = "This page has been migrated to the Success Master."
            };
            return View("MigratedMessage", model);

            //IEnumerable<SYSITE> SYSiteItems = _objCalenderMasterService.Bind_SYSite();
            //ViewBag.SYSITE = new MultiSelectList(SYSiteItems, "SYSITEID", "DESCRIP");
            //CalenderMasterViewModel viewModel = new CalenderMasterViewModel();
            //// Populate FinancialYears property here                    
            //viewModel.FinancialYears = GetFinancialYearsFrom(2009);

            //CalenderSearchViewModel searchViewModel = new CalenderSearchViewModel();
            //DateTime currentDate = DateTime.Now;

            //int startYear = currentDate.Month >= 4 ? currentDate.Year : currentDate.Year - 1;
            //string currentFinancialYear = $"{startYear}-{startYear + 1}";

            //viewModel.SelectedFinancialYear = currentFinancialYear;
            //viewModel.CalenderMaster = _objCalenderMasterService.GetCalenderMasterListForUser(viewModel.SearchViewModel);



            ////if(viewModel.CalenderMaster.Count==0)
            ////{
            ////    viewModel.CalenderMaster = "No Record Found";
            ////}
            //return View("CalenderMasterForUser", viewModel);

        }

        [HttpPost]
        public ActionResult CalenderMasterForUser(CalenderMasterViewModel CMVM)
        {
            try
            {               
                IEnumerable<SYSITE> SYSiteItems = _objCalenderMasterService.Bind_SYSite();
                ViewBag.SYSITE = new MultiSelectList(SYSiteItems, "SYSITEID", "DESCRIP");
                CalenderMasterViewModel viewModel = new CalenderMasterViewModel();
                // Populate FinancialYears property here
                viewModel.FinancialYears = GetFinancialYearsFrom(2009);
                //viewModel.FinancialYears = GetFinancialYears(2022, 3);
                CalenderSearchViewModel searchViewModel = new CalenderSearchViewModel();
                if (CMVM.SelectedFinancialYear != null)
                {
                    CMVM.SearchViewModel.SelectedFinancialYear = CMVM.SelectedFinancialYear;
                }

                //CMVM.SelectedFinancialYear = SelectedFinancialYear;
                viewModel.CalenderMaster = _objCalenderMasterService.GetCalenderMasterListForUser(CMVM.SearchViewModel);

                return View("CalenderMasterForUser", viewModel);

                //IEnumerable<SYSITE> SYSiteItems = _objCalenderMasterService.Bind_SYSite();
                //ViewBag.SYSITE = new MultiSelectList(SYSiteItems, "SYSITEID", "DESCRIP");
                ////CalenderMasterViewModel CMVM = new CalenderMasterViewModel();
                ////searchViewModel. = GetFinancialYearsFrom(2009);
                ////CalenderMasterViewModel CMVM = new CalenderMasterViewModel();
                //CMVM.CalenderMaster = _objCalenderMasterService.GetCalenderMasterList(CMVM.SearchViewModel);
                //CMVM.SearchViewModel.SelectedFinancialYear = CMVM.SearchViewModel.SelectedFinancialYear;
                //return View(CMVM);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", Convert.ToString(ex));
                return RedirectToAction("ErrorPage");
            }
        }


        [HttpPost]
        public ActionResult CalenderMaster(CalenderMasterViewModel CMVM)
        {
            try
            {                
                IEnumerable<SYSITE> SYSiteItems = _objCalenderMasterService.Bind_SYSite();
                ViewBag.SYSITE = new MultiSelectList(SYSiteItems, "SYSITEID", "DESCRIP");
                CalenderMasterViewModel viewModel = new CalenderMasterViewModel();
                // Populate FinancialYears property here
                //viewModel.FinancialYears = GetFinancialYearsFrom(2009);
                viewModel.FinancialYears = GetFinancialYears(2022, 3);
                CalenderSearchViewModel searchViewModel = new CalenderSearchViewModel();
                viewModel.CalenderMaster = _objCalenderMasterService.GetCalenderMasterList(CMVM.SearchViewModel);

                return View("CalenderMaster", viewModel);

                //IEnumerable<SYSITE> SYSiteItems = _objCalenderMasterService.Bind_SYSite();
                //ViewBag.SYSITE = new MultiSelectList(SYSiteItems, "SYSITEID", "DESCRIP");
                ////CalenderMasterViewModel CMVM = new CalenderMasterViewModel();
                ////searchViewModel. = GetFinancialYearsFrom(2009);
                ////CalenderMasterViewModel CMVM = new CalenderMasterViewModel();
                //CMVM.CalenderMaster = _objCalenderMasterService.GetCalenderMasterList(CMVM.SearchViewModel);
                //CMVM.SearchViewModel.SelectedFinancialYear = CMVM.SearchViewModel.SelectedFinancialYear;
                //return View(CMVM);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", Convert.ToString(ex));
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpGet]
        public ActionResult CreateCalenderMaster()
        {            
            try
            {                
                CalenderMasterViewModel viewModel = new CalenderMasterViewModel();


                viewModel.FinancialYears = GetFinancialYears(2022, 3);


                IEnumerable<SYSITE> SYSiteItems = _objCalenderMasterService.Bind_SYSite();
                ViewBag.Location = new MultiSelectList(SYSiteItems, "SYSITEID", "DESCRIP");
                IEnumerable<Employee_Details> items1 = _objCalenderMasterService.BindAppAuth1();
                ViewBag.AppAuth1 = new SelectList(items1, "_ECode", "_EName");
                IEnumerable<Employee_Details> items2 = _objCalenderMasterService.BindAppAuth1();
                ViewBag.AppAuth2 = new SelectList(items2, "_ECode", "_EName");
                return PartialView("_CreateCalenderMaster", viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", Convert.ToString(ex));
                return RedirectToAction("ErrorPage");
            }
        }

        public static List<string> GetFinancialYears(int startYear, int numberOfYears)
        {
            List<string> financialYears = new List<string>();

            // Assuming financial years start from April 1st
            //int startMonth = 4;

            // Generate financial years (e.g., "2009-2010", "2010-2011", ...)
            for (int i = 0; i < numberOfYears; i++)
            {
                int year = startYear + i;
                string financialYear = $"{year}-{(year + 1).ToString("D4")}";
                financialYears.Add(financialYear);
            }

            return financialYears;

            //List<SelectListItem> financialYears = new List<SelectListItem>();           

            //for (int i = startYear; i <= DateTime.Now.Year; i++)
            //{
            //    string financialYear = $"{i}-{i + 1}";
            //    financialYears.Add(new SelectListItem { Text = financialYear, Value = financialYear });
            //}

            //return financialYears;
        }

        public static List<string> GetFinancialYearsFrom(int startYear)
        {
            List<string> financialYears = new List<string>();

            // Assuming financial years start from April 1st


            // Generate financial years(e.g., "2009-2010", "2010-2011", ...)
            for (int year = startYear; year <= DateTime.Now.Year; year++)
            {
                string financialYear = $"{year}-{(year + 1).ToString("D4")}";
                financialYears.Add(financialYear);
            }

            //List<SelectListItem> financialYears = new List<SelectListItem>();

            //for (int i = startYear; i <= DateTime.Now.Year; i++)
            //{
            //    string financialYear = $"{i}-{i + 1}";
            //    financialYears.Add(new SelectListItem { Text = financialYear, Value = financialYear });
            //}

            return financialYears;
        }


        //[HttpPost]
        //public ActionResult UploadCalender(HttpPostedFileBase FILE, string DOC_TYPE)
        //{
        //    short retVal = 0;
        //    try
        //    {               
        //        CalenderMasterViewModel CMAtt = new CalenderMasterViewModel();
        //        if (FILE.ContentLength > 0 && !string.IsNullOrEmpty(DOC_TYPE))
        //        {
        //            FileViewModel _file = GetUploadFile(FILE, DOC_TYPE);
        //            CMAtt.CALENDER_UPLOAD = _file.FileName;
        //            CMAtt.CALENDER_CONTENTTYPE = _file.FileContentType;
        //            CMAtt.FILE_BYTE = _file.File;
        //            //POAtt.DOC_TYPE = DOC_TYPE;
        //            TempData["CALENDER_ATTACHMENT"] = CMAtt;
        //            retVal = 1;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        retVal = -1;
        //    }
        //    return Json(retVal);
        //}

        [HttpPost]
        public ActionResult UploadCalender(IFormFile FILE, string DOC_TYPE)
        {
            short retVal = 0;
            try
            {
                CalenderMasterViewModel CMAtt = new CalenderMasterViewModel();
                if (FILE != null && FILE.Length > 0 && !string.IsNullOrEmpty(DOC_TYPE))
                {
                    FileViewModel _file = GetUploadFile(FILE, DOC_TYPE);
                    CMAtt.CALENDER_UPLOAD = _file.FileName;
                    CMAtt.CALENDER_CONTENTTYPE = _file.FileContentType;
                    CMAtt.FILE_BYTE = _file.File;
                    //POAtt.DOC_TYPE = DOC_TYPE;
                    TempData["CALENDER_ATTACHMENT"] = CMAtt;
                    retVal = 1;
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        //private FileViewModel GetUploadFile(HttpPostedFileBase file, string DocType)
        //{
        //    try
        //    {
        //        FileViewModel FVM = new FileViewModel();
        //        if (file != null && file.ContentLength > 0)
        //        {
        //            byte[] bytes;
        //            using (BinaryReader br = new BinaryReader(file.InputStream))
        //            {
        //                bytes = br.ReadBytes(file.ContentLength);
        //            }
        //            string _FileName = file.FileName.Substring(file.FileName.LastIndexOf("\\") + 1);
        //            FVM.FileContentType = MimeMapping.GetMimeMapping(_FileName);
        //            //FVM.FileName = DocType + "_" + PONo + ".pdf";
        //            FVM.FileName = DocType + "_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
        //            FVM.File = bytes;
        //        }
        //        return FVM;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        private FileViewModel GetUploadFile(IFormFile file, string DocType)
        {
            try
            {
                FileViewModel FVM = new FileViewModel();
                if (file != null && file.Length > 0)
                {                   
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        FVM.File = ms.ToArray();
                    }

                    string[] splitFile = file.FileName.Split('.');
                    //FVM.ATTACHMENT_NAME = Path.GetFileName(file.FileName);//splitFile[0];
                    FVM.FileContentType = file.ContentType;
                    FVM.FileName = DocType + "_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
                }

                //if (file != null && file.ContentLength > 0)
                //{
                //    byte[] bytes;
                //    using (BinaryReader br = new BinaryReader(file.InputStream))
                //    {
                //        bytes = br.ReadBytes(file.ContentLength);
                //    }
                //    string _FileName = file.FileName.Substring(file.FileName.LastIndexOf("\\") + 1);
                //    FVM.FileContentType = MimeMapping.GetMimeMapping(_FileName);
                //    //FVM.FileName = DocType + "_" + PONo + ".pdf";
                //    FVM.FileName = DocType + "_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
                //    FVM.File = bytes;
                //}
                return FVM;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //private FileViewModel GetUploadFile(HttpPostedFileBase file)
        //{
        //    try
        //    {
        //        FileViewModel FVM = new FileViewModel();
        //        if (file != null && file.ContentLength > 0)
        //        {
        //            byte[] bytes;
        //            using (BinaryReader br = new BinaryReader(file.InputStream))
        //            {
        //                bytes = br.ReadBytes(file.ContentLength);
        //            }
        //            FVM.FileName = file.FileName.Substring(file.FileName.LastIndexOf("\\") + 1);
        //            FVM.FileContentType = MimeMapping.GetMimeMapping(FVM.FileName);
        //            FVM.File = bytes;
        //        }
        //        return FVM;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        [HttpPost]
        public ActionResult CreateCalenderMaster(CalenderMasterViewModel AVM, FormCollection form)
        {
            try
            {               
                if (ModelState.IsValid)
                {
                    AVM.LOCATION = form["Loc"];
                    string Location1 = AVM.LOCATION.Replace(",", "_");
                    var locations = Location1.ToString().Replace("\"", "");
                    string CalenderType = "CAL_" + AVM.SelectedFinancialYear + "_" + AVM.CALENDER + "_L-" + locations;
                    FileViewModel _calenderFile = GetUploadFile(AVM.CALENDERFILE, CalenderType);
                    string baseFileName = _calenderFile.FileName;

                    AVM.CALENDER_UPLOAD = "CAL_" + AVM.SelectedFinancialYear + "_" + AVM.CALENDER + "_L-" + locations + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + System.IO.Path.GetExtension(_calenderFile.FileName);
                    AVM.CALENDER_CONTENTTYPE = _calenderFile.FileContentType;
                    AVM.CALENDER_BLOB = _calenderFile.File;
                    AVM.FILE_BYTE = _calenderFile.File;
                    AVM.CREATED_BY = _sessionService.Get<string>("userID");


                    AVM.LOCATION = locations.Replace("_", ",");


                    Int16 obj = _objCalenderMasterService.SaveProcessAttachment_Trn(AVM);

                    //string FileName = Server.MapPath("~/Uploads/ASR/Calendar/" + "CAL_" + AVM.SelectedFinancialYear + "_" + AVM.CALENDER + "_L-" + AVM.LOCATION + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".pdf");

                    //string FileName = Server.MapPath("~/Uploads/ASR/Calendar/" + AVM.CALENDER_UPLOAD);
                    string FileName = _env.GetGeneralSettings().Get_FileUpload_Path + "/ASR/Calendar/" + AVM.CALENDER_UPLOAD;

                    //string path = Server.MapPath(("~/Uploads/ASR/Calendar/"));
                    string path = _env.GetGeneralSettings().Get_FileUpload_Path + "/ASR/Calendar/" ;

                    if (obj == 1)
                    {
                        Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");
                        
                        short retVal = _objCalenderMasterService.SendMailByApprovalAuthority(AVM, _Employee_Details);
                        //string FileName = Server.MapPath("~/Uploads/ITD/" + SiteName +"/"+ formData.ADEMPCODE + "_"+ formData.SUB_D_TYPE_ID + "_"+ formData.FINYEAR+".pdf");
                        //string path = Server.MapPath("~/Uploads/ITD/" + SiteName+"/");
                        if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                        if (obj > 0)
                        {
                            //foreach (CalenderMasterViewModel i in AVM)
                            //{
                            if (AVM.IsDeleted == 0 && AVM.FILE_BYTE != null)
                            {
                                System.IO.File.WriteAllBytes(FileName, AVM.FILE_BYTE.ToArray());
                            }
                            //}
                        }
                    }
                    if (obj > 0)
                    {
                        TempData["AlertMessage"] = obj;
                    }
                    else
                    {
                        TempData["AlertMessage"] = obj;
                    }
                }
                else
                {
                    var modelErrors = new List<string>();
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var modelError in modelState.Errors)
                        {
                            modelErrors.Add(modelError.ErrorMessage);
                        }
                    }
                    TempData["AlertMessage"] = modelErrors;
                }
                return RedirectToAction("CalenderMaster");

            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = ex.Message;
                return RedirectToAction("ErrorPage");
            }
        }



        [HttpGet]
        public ActionResult EditCalenderMaster(int id)
        {           

            try
            {                

                IEnumerable<SYSITE> items = _objCalenderMasterService.Bind_SYSite();
                ViewBag.Location = new MultiSelectList(items, "SYSITEID", "DESCRIP");
                IEnumerable<Employee_Details> items1 = _objCalenderMasterService.BindAppAuth1();
                ViewBag.AppAuth1 = new SelectList(items1, "_ECode", "_EName");
                IEnumerable<Employee_Details> items2 = _objCalenderMasterService.BindAppAuth1();
                ViewBag.AppAuth2 = new SelectList(items2, "_ECode", "_EName");
                CalenderMasterViewModel model = _objCalenderMasterService.GetCalenderMasterDetails(id);
                // var model = _objCalenderMasterService.GetCalenderMasterDetails(id);
                string inputID = model.LOCATION;
                //string inputString = model.LOCATIONDESC;

                // Split the string into an array of strings
                string[] locationIDS = inputID.Split(',');
                //string[] locationStrings = inputString.Split(',');

                // Convert the array of strings to an array of integers
                int[] locationIds1 = Array.ConvertAll(locationIDS, int.Parse);
                // string[] locationStrings1 =locationStrings;

                //List<string> locationStrings11= new List<string> { "SYSITEID", "DESCRIP" };

                //locationStrings11 = locationStrings1.ToList();

                // Assign the array of integers to your model property
                //model.SelectedLocation = locationIds1;
                model.SelectedLocationDesc1 = locationIds1.ToList();
                // model.LOCATION = model.LOCATION;

                return PartialView("_EditCalenderMaster", model);

                //return PartialView("_EditCalenderMaster", _objCalenderMasterService.GetCalenderMasterDetails(id));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", Convert.ToString(ex));
                return RedirectToAction("ErrorPage");
            }

        }

        [HttpPost]
        public ActionResult EditCalenderMaster(CalenderMasterViewModel AVM, FormCollection form)
        {
            try
            {
               
                ModelState.Remove("CALENDERFILE");


                //if (ModelState.IsValid)
                //{
                //HttpFileCollectionBase file = Request.Files;
                //FileViewModel bannerFile = GetUploadFile(file[0],"CALENDER");
                AVM.LOCATION = form["Loc"];
                string Location1 = AVM.LOCATION.Replace(",", "_");
                var locations = Location1.ToString().Replace("\"", "");

                string CalenderType = "CAL_" + AVM.SelectedFinancialYear + "_" + AVM.CALENDER + "_L-" + locations;
                FileViewModel _calenderFile = GetUploadFile(AVM.CALENDERFILE, CalenderType);

                AVM.CALENDER_UPLOAD = "CAL_" + AVM.SelectedFinancialYear + "_" + AVM.CALENDER + "_L-" + locations + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + System.IO.Path.GetExtension(_calenderFile.FileName);
                AVM.CALENDER_CONTENTTYPE = _calenderFile.FileContentType;
                AVM.CALENDER_BLOB = _calenderFile.File;
                AVM.FILE_BYTE = _calenderFile.File;
                AVM.MODIFIED_BY = _sessionService.Get<string>("userID");
                //AVM.LOCATION = AVM.LOCATION;
                AVM.LOCATION = locations.Replace("_", ",");

                // string locationInput = AVM.LOCATION;
                //string[] locationIDS = locationInput.Split(',');
                //int[] locationIds1 = Array.ConvertAll(locationIDS, int.Parse);                    
                //AVM.SelectedLocationDesc1 = locationIds1.ToList();

                Int16 obj = _objCalenderMasterService.UpdateProcessAttachment_Trn(AVM);

                //string FileName = Server.MapPath("~/Uploads/ASR/Calendar/" + AVM.CALENDER_UPLOAD);
                //string path = Server.MapPath(("~/Uploads/ASR/Calendar/"));

                string FileName = _env.GetGeneralSettings().Get_FileUpload_Path + "/ASR/Calendar/" + AVM.CALENDER_UPLOAD;
                string path = _env.GetGeneralSettings().Get_FileUpload_Path + "/ASR/Calendar/";


                if (obj == 1)
                {
                    //string FileName = Server.MapPath("~/Uploads/ITD/" + SiteName +"/"+ formData.ADEMPCODE + "_"+ formData.SUB_D_TYPE_ID + "_"+ formData.FINYEAR+".pdf");
                    //string path = Server.MapPath("~/Uploads/ITD/" + SiteName+"/");
                    if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                    if (obj > 0)
                    {
                        //foreach (CalenderMasterViewModel i in AVM)
                        //{
                        if (AVM.IsDeleted == 0 && AVM.FILE_BYTE != null)
                        {
                            System.IO.File.WriteAllBytes(FileName, AVM.FILE_BYTE.ToArray());
                        }
                        //}
                    }
                }

                if (obj > 0)
                {
                    return Json("success");
                }
                else
                {
                    return Json("failed");
                }

                //}
                //else
                //{
                //    var modelErrors = new List<string>();
                //    foreach (var modelState in ModelState.Values)
                //    {
                //        foreach (var modelError in modelState.Errors)
                //        {
                //            modelErrors.Add(modelError.ErrorMessage);
                //        }
                //    }
                //    return Json(modelErrors);
                //}

            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult GetCalenderMasterDetails(int id)
        {
            try
            {
                return PartialView("_CalenderMasterViewDetails", _objCalenderMasterService.GetCalenderMasterDetails(id));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", Convert.ToString(ex));
                return RedirectToAction("ErrorPage");
            }

        }

        public FileResult Download(Int64 id, string type)
        {
            FileViewModel file = _objCalenderMasterService.GetFileForDownload(id, type);
            return File(file.File, file.FileContentType, file.FileName);
        }

        [HttpGet]
        public ActionResult CalenderMasterApproval(string id)
        {           
            long _ReqId = Convert.ToInt64(id); // Server.UrlDecode(Encryption.Decrypt(id))
            CalenderMasterViewModel obj = _objCalenderMasterService.GetCalenderMSTRequestById(_ReqId);
            long userid = Convert.ToInt64(_sessionService.Get<string>("userID"));

            return View("CalenderMasterApproval", obj);
        }

        [HttpGet]
        public ActionResult CalenderMasterApprovalHistory(string id)
        {           
            long _ReqId = Convert.ToInt64(id); // Server.UrlDecode(Encryption.Decrypt(id))
            CalenderMasterViewModel obj = _objCalenderMasterService.GetCalenderMSTRequestById(_ReqId);
            long userid = Convert.ToInt64(_sessionService.Get<string>("userID"));

            return View("CalenderMasterApprovalHistory", obj);
        }

        [HttpPost]
        public ActionResult CalenderMasterApproval(CALENDERMASTERMAPPINGTRNViewModel PHVM)
        {
            short retVal = 0;
            try
            {
                Employee_Details _Employee_Details = _sessionService.Get<Employee_Details>("Employee");                

                PHVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));

                retVal = _objCalenderMasterService.CalenderMasterAppr(PHVM, _Employee_Details);

            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult DeactiveReq(string id)
        {
            short retVal = 0;
            try
            {               
                long _ReqId = Convert.ToInt64(id); // Server.UrlDecode(Encryption.Decrypt(id))
                retVal = _objCalenderMasterService.DeactiveReqById(_ReqId);
                //CalenderMasterViewModel obj = _objCalenderMasterService.DeactiveReqById(_ReqId);
                long userid = Convert.ToInt64(_sessionService.Get<string>("userID"));

            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);

        }

        public class MigratedPageViewModel
        {
            public string Title { get; set; } = "";
            public string NewUrl { get; set; } = "";
            public string Message { get; set; } = "";
        }
    }
}
