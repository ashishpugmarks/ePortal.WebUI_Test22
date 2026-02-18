using System.Data;
using System.Net.Http.Headers;
using ePortal.Persistence.Interface;
using ePortal.Shared.Interface;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    [SessionTimeout]
    public class PDFMergeController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly ILogger<HomeController> _logger;
        private readonly IDataManagement odmt ;
        private readonly IWebHostEnvironment _env;
        public PDFMergeController(ISessionService sessionService, ILogger<HomeController> logger, IDataManagement _odmt, IWebHostEnvironment env)
        {
            odmt = _odmt;
            _sessionService = sessionService;
            _logger = logger;
            _env = env;
        }


        // GET: PDFMerge
        public ActionResult Index()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View("PdfMergeFile");
        }
        [HttpPost]
        public async Task<IActionResult> MergePdf(List<IFormFile> Files)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            int retval = 0;
            string empcode = _sessionService.Get<string>("userID");
            string pathtemp = Path.Combine(_env.WebRootPath, "Python_Uploads", "PDFMerge");
            Directory.CreateDirectory(pathtemp); // Ensure directory exists

            // Clean up old files
            try
            {
                var tmpfiles = Directory.GetFiles(pathtemp, "*.pdf", SearchOption.AllDirectories);
                foreach (var file in tmpfiles)
                {
                    var fi = new FileInfo(file);
                    if (fi.LastWriteTime < DateTime.Now.AddSeconds(-1))
                    {
                        fi.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
            }

            // Save uploaded files
            for (int i = 0; i < Files.Count; i++)
            {
                var file = Files[i];
                if (file != null && file.Length > 0)
                {
                    string filename = $"{empcode}_{i + 1}.pdf";
                    string filePath = Path.Combine(pathtemp, filename);

                    try
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(ex.Message);
                    }
                }
            }

            // Get API URL from database
            string tmpl_url = "";
            System.Data.DataTable dt = odmt.GetDataTable("select t.URL from python_links t where t.application_name='PDF_MERGE'");
            if (dt.Rows.Count > 0)
            {
                tmpl_url = dt.Rows[0].ItemArray[0].ToString();
            }

            // Call external API
            using (var client = new HttpClient())
            {
                string url = tmpl_url + empcode;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            
                    var responseTask = client.GetAsync(url);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    
                    var readTask = result.Content.ReadAsStringAsync().Result;
                    var resultContent = Newtonsoft.Json.JsonConvert.DeserializeObject<APIReturn>(readTask);
                if (resultContent?.error != null)
                    {
                        string error = resultContent.error.Replace(empcode + "_", "Document No. ");
                        error = error.Replace(".pdf", "");
                        return Json(error);
                    }                
            }

            string path = Path.Combine(pathtemp, $"{empcode}_merged.pdf");
            TempData["EXCELFILE"] = path;
            retval = 1;

            return Json(retval);
        }

        public ActionResult DownloadDocx()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {

                if (TempData["EXCELFILE"] == null)
                {
                    return View();
                }
                string file = (string)TempData["EXCELFILE"];

                byte[] pdfbytes = System.IO.File.ReadAllBytes(file);
                //HttpContext.Response.AddHeader("content-disposition", "attachment; filename=MergePdf.pdf");
                FileInfo fi = new FileInfo(file);
                try
                {
                    fi.Delete();
                }
                catch (Exception ex) { }

                //Response.ContentType = "application/pdf";
                return File(pdfbytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "ConvertedWord.docx");
            }
            catch (Exception ex)
            {

                return RedirectToAction("ErrorPage");
            }
        }
        public ActionResult DownloadPDF()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {

                if (TempData["EXCELFILE"] == null)
                {
                    return View();
                }
                string file = (string)TempData["EXCELFILE"];

                byte[] pdfbytes = System.IO.File.ReadAllBytes(file);
                //HttpContext.Response.AddHeader("content-disposition", "attachment; filename=MergePdf.pdf");
                FileInfo fi = new FileInfo(file);
                try
                {
                    fi.Delete();
                }
                catch (Exception ex) { }

                //Response.ContentType = "application/pdf";
                return File(pdfbytes, "application/pdf", "CompressedFile.pdf");
            }
            catch (Exception ex)
            {

                return RedirectToAction("ErrorPage");
            }
        }
        public ActionResult MergePdfDownload()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                //LogError("entered in function", "", "");

                if (TempData["EXCELFILE"] == null)
                {
                    return View();
                }
                //LogError("path", (string)TempData["EXCELFILE"], "");
                string file = (string)TempData["EXCELFILE"];
                //LogError("File bytes found", file, "");
                byte[] pdfbytes = System.IO.File.ReadAllBytes(file);
                //LogError("File bytes found", "", "");
                //HttpContext.Response.AddHeader("content-disposition", "attachment; filename=MergePdf.pdf");
                FileInfo fi = new FileInfo(file);
                try
                {
                    fi.Delete();
                }
                catch (Exception ex) { /*LogError("error", ex.Message, "");*/ }
                //LogError("pdf found and file deleted", ",", "");
                //Response.ContentType = "application/pdf";
                return File(pdfbytes, "application/pdf", "MergePdf.pdf");
            }
            catch (Exception ex)
            {

                return RedirectToAction("ErrorPage");
            }
        }
        public ActionResult EncryptedPDF()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {

                if (TempData["Encrypt"] == null)
                {
                    return View();
                }
                string file = (string)TempData["Encrypt"];

                byte[] pdfbytes = System.IO.File.ReadAllBytes(file);
                //HttpContext.Response.AddHeader("content-disposition", "attachment; filename=MergePdf.pdf");
                FileInfo fi = new FileInfo(file);
                try
                {
                    fi.Delete();
                }
                catch (Exception ex) { }

                //Response.ContentType = "application/pdf";
                return File(pdfbytes, "application/pdf", "EncryptedPDF.pdf");
            }
            catch (Exception ex)
            {

                return RedirectToAction("ErrorPage");
            }
        }
        public ActionResult DownloadImage()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {

                if (TempData["IMGFILE"] == null)
                {
                    return View();
                }
                string file = (string)TempData["IMGFILE"];

                byte[] pdfbytes = System.IO.File.ReadAllBytes(file);
                //HttpContext.Response.AddHeader("content-disposition", "attachment; filename=MergePdf.pdf");
                FileInfo fi = new FileInfo(file);

                try
                {
                    fi.Delete();
                }
                catch (Exception ex) { }

                //Response.ContentType = "application/pdf";
                return File(pdfbytes, "image/jpeg", "CompressImage.jpeg");
            }
            catch (Exception ex)
            {

                return RedirectToAction("ErrorPage");
            }
        }
        //PDF Comprosser

        public ActionResult Utilities()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
        public ActionResult PDFCompressor()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
        public ActionResult ImageCompressor()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
        public ActionResult PDFToWordConvertor()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
        public ActionResult PDFEncryption()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
        public ActionResult PDFToExcel()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PDFCompressor(IFormFile Files)        
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            int retval = 0;
            string pathtemp = Path.Combine(_env.WebRootPath, "Python_Uploads", "PdfCompress");
            Directory.CreateDirectory(pathtemp); 

            try
            {
                foreach (var file in Directory.GetFiles(pathtemp, "*", SearchOption.AllDirectories))
                {
                    var fi = new FileInfo(file);
                    if (fi.LastWriteTime < DateTime.Now.AddSeconds(-1))
                    {
                        fi.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
               
            }

            string Filename = DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
            if (Files != null && Files.Length > 0)
            {
                var filePath = Path.Combine(pathtemp, Filename);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Files.CopyToAsync(stream);
                }

                try
                {
                    using (var client = new HttpClient())
                    {
                        DataTable dt = odmt.GetDataTable("select t.URL from python_links t where t.application_name='PDF_COMPRESSOR'");
                        string tmpl_url = "";
                        if (dt.Rows.Count > 0)
                        {
                            tmpl_url = dt.Rows[0].ItemArray[0].ToString();

                        }
                        string url = tmpl_url + Filename;
                        var responseTask = client.GetAsync(url);
                        responseTask.Wait();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Exception in PDFCompressor" + ex.Message.ToString());
                }
            }

            //string path = Path.Combine(pathtemp, "new_" + Filename);
            string path = Path.Combine(pathtemp, Filename);
            TempData["EXCELFILE"] = path;
            retval = 1;
            return Json(retval);
        }
        [HttpPost]
        public async Task<IActionResult> ImageCompressor(IFormFile Files)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            int retval = 0;
            string pathtemp = Path.Combine(_env.WebRootPath, "Python_Uploads", "ImageCompress");
            Directory.CreateDirectory(pathtemp); // Ensure directory exists

            try
            {
                foreach (var file in Directory.GetFiles(pathtemp, "*", SearchOption.AllDirectories))
                {
                    var fi = new FileInfo(file);
                    if (fi.LastWriteTime < DateTime.Now.AddSeconds(-1))
                    {
                        fi.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
            }

            var doctype = Path.GetExtension(Files.FileName)?.TrimStart('.');
            string Filename = DateTime.Now.ToString("yyyyMMddHHmmss") + "." + doctype;
            if (Files != null && Files.Length > 0)
            {
                var filePath = Path.Combine(pathtemp, Filename);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Files.CopyToAsync(stream);
                }

                try
                {
                    using (var client = new HttpClient())
                    {
                        DataTable dt = odmt.GetDataTable("select t.URL from python_links t where t.application_name='IMAGE_COMPRESSOR'");
                        string tmpl_url = "";
                        if (dt.Rows.Count > 0)
                        {
                            tmpl_url = dt.Rows[0].ItemArray[0].ToString();

                        }

                        string url = tmpl_url + Filename;
                        var responseTask = client.GetAsync(url);
                        responseTask.Wait();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Exception in ImageCompressor" + ex.Message.ToString());
                }
            }

            //string path = Path.Combine(pathtemp, "new_" + Filename);
            string path = Path.Combine(pathtemp, Filename);
            TempData["IMGFILE"] = path;
            retval = 1;
            return Json(retval);
        }

        
        [HttpPost]
        public async Task<IActionResult> PDFToDocsConvertor(IFormFile Files)
        {
            int retval = 0;
            string pathtemp = Path.Combine(_env.WebRootPath, "Python_Uploads", "PdfToDocx");
            Directory.CreateDirectory(pathtemp); // Ensure directory exists

            try
            {
                foreach (var file in Directory.GetFiles(pathtemp, "*", SearchOption.AllDirectories))
                {
                    var fi = new FileInfo(file);
                    if (fi.LastWriteTime < DateTime.Now.AddSeconds(-1))
                    {
                        fi.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
            }

            string temp_filename = DateTime.Now.Date.ToString("yyyyMMddHHmmss");
            string Filename = temp_filename + ".pdf";
            if (Files != null && Files.Length > 0)
            {
                var filePath = Path.Combine(pathtemp, Filename);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Files.CopyToAsync(stream);
                }

                try
                {
                    using (var client = new HttpClient())
                    {
                        DataTable dt = odmt.GetDataTable("select t.URL from python_links t where t.application_name='PDF_TO_DOCS_CONVERTER'");
                        string tmpl_url = "";
                        if (dt.Rows.Count > 0)
                        {
                            tmpl_url = dt.Rows[0].ItemArray[0].ToString();

                        }
                        string url = tmpl_url + Filename;
                        var responseTask = client.GetAsync(url);
                        responseTask.Wait();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Exception in PDFToDocsConvertor" + ex.Message.ToString());
                }
            }

            string path = Path.Combine(pathtemp, "new_" + temp_filename + ".docx");
            //string path = Path.Combine(pathtemp, temp_filename + ".docx");
            TempData["EXCELFILE"] = path;
            retval = 1;
            return Json(retval);
        }
                

        [HttpPost]
        public async Task<IActionResult> PDFEncryption(PDFMergeFile formdata, IFormFile Files)
        {
            int retval = 0;
            string pathtemp = Path.Combine(_env.WebRootPath, "Python_Uploads", "Encryption");
            Directory.CreateDirectory(pathtemp); // Ensure directory exists

            try
            {
                foreach (var file in Directory.GetFiles(pathtemp, "*", SearchOption.AllDirectories))
                {
                    var fi = new FileInfo(file);
                    if (fi.LastWriteTime < DateTime.Now.AddSeconds(-1))
                    {
                        fi.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
            }


            string Filename = DateTime.Now.Date.ToString("yyyyMMddHHmmss") + ".pdf";
            if (Files != null && Files.Length > 0)
            {
                var filePath = Path.Combine(pathtemp, Filename);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Files.CopyToAsync(stream);
                }

                try
                {
                    using (var client = new HttpClient())
                    {
                        DataTable dt = odmt.GetDataTable("select t.URL from python_links t where t.application_name='Pdf_Encryption'");
                        string tmpl_url = "";
                        if (dt.Rows.Count > 0)
                        {
                            tmpl_url = dt.Rows[0].ItemArray[0].ToString();

                        }
                        string url = tmpl_url + Filename + "&pas=" + formdata.Pass;
                        var responseTask = client.GetAsync(url);
                        responseTask.Wait();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Exception in PDFEncryption" + ex.Message.ToString());
                }
            }

            //string path = Path.Combine(pathtemp, "new_" + Filename);
            string path = Path.Combine(pathtemp, Filename);
            TempData["Encrypt"] = path;

            retval = 1;
            return Json(retval);
        }

       

        [HttpPost]
        public async Task<IActionResult> PDFToExcel(IFormFile Files)
        {
            int retval = 0;
            string pathtemp = Path.Combine(_env.WebRootPath, "Python_Uploads", "PdfToExcel");
            Directory.CreateDirectory(pathtemp); // Ensure directory exists

            try
            {
                foreach (var file in Directory.GetFiles(pathtemp, "*", SearchOption.AllDirectories))
                {
                    var fi = new FileInfo(file);
                    if (fi.LastWriteTime < DateTime.Now.AddSeconds(-1))
                    {
                        fi.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
            }


            string Filename = DateTime.Now.Date.ToString("yyyyMMddHHmmss") + ".pdf";
            if (Files != null && Files.Length > 0)
            {
                var filePath = Path.Combine(pathtemp, Filename);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Files.CopyToAsync(stream);
                }

                try
                {
                    using (var client = new HttpClient())
                    {
                        DataTable dt = odmt.GetDataTable("select t.URL from python_links t where t.application_name='PDF_TO_EXCEL'");
                        string tmpl_url = "";
                        if (dt.Rows.Count > 0)
                        {
                            tmpl_url = dt.Rows[0].ItemArray[0].ToString();

                        }
                        string url = tmpl_url + Filename;
                        var responseTask = client.GetAsync(url);
                        responseTask.Wait();
                        var result = responseTask.Result;

                        var readTask = result.Content.ReadAsStringAsync().Result;
                        var resultContent = Newtonsoft.Json.JsonConvert.DeserializeObject<APIReturn>(readTask);
                        if (resultContent.error != null)
                        {
                            return Json(resultContent.error);
                        }

                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Exception in PDFToExcel" + ex.Message.ToString());
                }
            }

            string path = Path.Combine(pathtemp, "output.xlsx");
            TempData["EXLFILE"] = path;

            retval = 1;
            return Json(retval);
        }

        public ActionResult DownloadExcel()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {

                if (TempData["EXLFILE"] == null)
                {
                    return View();
                }
                string file = (string)TempData["EXLFILE"];

                byte[] pdfbytes = System.IO.File.ReadAllBytes(file);
                //HttpContext.Response.AddHeader("content-disposition", "attachment; filename=MergePdf.pdf");
                FileInfo fi = new FileInfo(file);

                try
                {
                    fi.Delete();
                }
                catch (Exception ex) { }

                //Response.ContentType = "application/pdf";
                return File(pdfbytes, "application/vnd.ms-excel", "ConvertedExcel.xlsx");
            }
            catch (Exception ex)
            {

                return RedirectToAction("ErrorPage");
            }
        }
        public class PDFMergeFile
        {
            public string Filename { get; set; }
            //public HttpPostedFileBase FILE { get; set; }

            public IFormFile FILE { get; set; }
            public byte[] FILE_BYTE { get; set; }
            public string FILE_CONTENTTYPE { get; set; }
            public string Pass { get; set; }
        }
        public class APIReturn
        {
            public string message { get; set; }
            public string error { get; set; }
        }
        public class PDFSplitData
        {
            public string Filename { get; set; }
            //public HttpPostedFileBase FILE { get; set; }
            public IFormFile FILE { get; set; }
            public byte[] FILE_BYTE { get; set; }
            public string FILE_CONTENTTYPE { get; set; }
            public string Start { get; set; }
            public string End { get; set; }
        }
        [HttpGet]
        public ActionResult PDFSplit()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> PDFSplit(PDFSplitData formdata, IFormFile Files)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            int retval = 0;
            string pathtemp = Path.Combine(_env.WebRootPath, "Python_Uploads", "PDFSplit");
            Directory.CreateDirectory(pathtemp); // Ensure directory exists

            try
            {
                foreach (var file in Directory.GetFiles(pathtemp, "*", SearchOption.AllDirectories))
                {
                    var fi = new FileInfo(file);
                    if (fi.LastWriteTime < DateTime.Now.AddSeconds(-1))
                    {
                        fi.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
            }


            string Filename = DateTime.Now.Date.ToString("yyyyMMddHHmmss") + ".pdf";
            if (Files != null && Files.Length > 0)
            {
                var filePath = Path.Combine(pathtemp, Filename);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Files.CopyToAsync(stream);
                }

                try
                {
                    using (var client = new HttpClient())
                    {
                        DataTable dt = odmt.GetDataTable("select t.URL from python_links t where t.application_name='PDF_SPLIT'");
                        string tmpl_url = "";
                        if (dt.Rows.Count > 0)
                        {
                            tmpl_url = dt.Rows[0].ItemArray[0].ToString();

                        }
                        var url = tmpl_url + Filename + "&s=" + formdata.Start + "&e=" + formdata.End;
                        var response = await client.GetAsync(url);
                        //var result = response.Result;


                        var readContent = await response.Content.ReadAsStringAsync();
                        var resultContent = Newtonsoft.Json.JsonConvert.DeserializeObject<APIReturn>(readContent);

                        if (resultContent.error != null)
                        {
                            return Json(resultContent.error);
                        }

                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Exception in PDFSplit" + ex.Message.ToString());
                }
            }

            string path = Path.Combine(pathtemp, Filename);
            TempData["SplitPdf"] = path;

            retval = 1;
            return Json(retval);
        }

        public ActionResult SplitPDF()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {


                if (TempData["SplitPdf"] == null)
                {

                    return View();
                }
                string file = (string)TempData["SplitPdf"];

                byte[] pdfbytes = System.IO.File.ReadAllBytes(file);
                //HttpContext.Response.AddHeader("content-disposition", "attachment; filename=MergePdf.pdf");
                FileInfo fi = new FileInfo(file);

                try
                {
                    fi.Delete();

                }
                catch (Exception ex) { }

                //Response.ContentType = "application/pdf";
                return File(pdfbytes, "application/pdf", "SplitPDF.pdf");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        //void LogError(String Msg, String Action, String StackTrace)
        //{
        //    //String IP = System.Configuration.ConfigurationManager.AppSettings["IISServerIP"].ToString();
        //    String dt = DateTime.Now.ToLongDateString() + "  " + DateTime.Now.ToLongTimeString();
        //    Msg = "\n" + dt + " " /*+ IP*/ + " " + Action + ": " + Msg;
        //    StackTrace = "\n " + StackTrace;
        //    // Commited 04-02-2023   
        //    System.IO.File.AppendAllText(@"E:\WebsiteHosting\ePortal_DynamicForm\KAIZEN_ERROR.txt", Msg);
        //    //System.IO.File.AppendAllText(Server.MapPath("~/Uploads/Dynamic_Log") + "/KAIZEN_ERROR.txt", "\n" + dt + " " + IP + " Demo:" + e.InnerException.Message.ToString());
        //    System.IO.File.AppendAllText(@"E:\WebsiteHosting\ePortal_DynamicForm\KAIZEN_ERROR.txt", StackTrace);

        //}
        [HttpGet]
        public ActionResult PDFSplitNEW()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PDFSplitNEW(PDFSplitData formdata, IFormFile Files)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            int retval = 0;
            string pathtemp = Path.Combine(_env.WebRootPath, "Python_Uploads", "PdfSplitNew");
            Directory.CreateDirectory(pathtemp); // Ensure directory exists

            try
            {
                foreach (var file in Directory.GetFiles(pathtemp, "*", SearchOption.AllDirectories))
                {
                    var fi = new FileInfo(file);
                    if (fi.LastWriteTime < DateTime.Now.AddSeconds(-1))
                    {
                        fi.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
            }


            string Filename = DateTime.Now.Date.ToString("yyyyMMddHHmmss") + ".pdf";
            if (Files != null && Files.Length > 0)
            {
                var filePath = Path.Combine(pathtemp, Filename);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Files.CopyToAsync(stream);
                }

                try
                {
                    using (var client = new HttpClient())
                    {
                        DataTable dt = odmt.GetDataTable("select t.URL from python_links t where t.application_name='PDF_SPLIT_NEW'");
                        string tmpl_url = "";
                        if (dt.Rows.Count > 0)
                        {
                            tmpl_url = dt.Rows[0].ItemArray[0].ToString();

                        }
                        var url = tmpl_url + Filename + "&count=" + formdata.Start;
                        var response = await client.GetAsync(url);
                        //var result = response.Result;


                        var readContent = await response.Content.ReadAsStringAsync();
                        var resultContent = Newtonsoft.Json.JsonConvert.DeserializeObject<APIReturn>(readContent);

                        if (resultContent.error != null)
                        {
                            return Json(resultContent.error);
                        }

                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Exception in PDFSplitNEW" + ex.Message.ToString());
                }
            }

            string path = Path.Combine(pathtemp, Filename);
            TempData["SplitPdfNew"] = path;

            retval = 1;
            return Json(retval);
        }

        public ActionResult SplitPDFNew()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {


                if (TempData["SplitPdfNew"] == null)
                {

                    return View();
                }
                string file = (string)TempData["SplitPdfNew"];

                byte[] zipBytes = System.IO.File.ReadAllBytes(file);
                //HttpContext.Response.AddHeader("content-disposition", "attachment; filename=MergePdf.pdf");
                FileInfo fi = new FileInfo(file);

                var result = File(zipBytes, "application/zip", "SplitPDF.zip");
                //try
                //{
                //    fi.Delete();

                //}
                //catch (Exception ex) { }

                //Response.ContentType = "application/pdf";
                return result;
            }
            catch (Exception ex)
            {


                return RedirectToAction("ErrorPage");
            }
        }

    }
}
