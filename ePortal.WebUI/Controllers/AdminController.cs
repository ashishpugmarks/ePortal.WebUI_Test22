using ePortal.Application.Contracts;
using ePortal.Persistence.Admin.Interface;
using ePortal.Persistence.Admin.Services;
using ePortal.Persistence.Interface;
using ePortal.Persistence.Services;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using ePortal.WebUI.Filters;
using Newtonsoft.Json.Linq;



namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    [SessionTimeout]
    public class AdminController : Controller
    {
        private IHumanResource OBJHR;
        private IAdminService _AdminService;
        private readonly IAppConfigurationService _env;
        private ISessionService _sessionService;
        ILogger<AdminController> _logger;
        public AdminController(IAdminService _adminService, IHumanResource humanResource, IAppConfigurationService env, ISessionService sessionService, ILogger<AdminController> logger)
        {
            _AdminService = _adminService;
            OBJHR=humanResource;
            _env = env;
            _sessionService = sessionService;
            _logger = logger;
          
        }
      

        [HttpGet]
        public ActionResult EmailGroupMapping()
        {
            

           // if (Session["userID"] == null)
             if (_sessionService.Get<string>("userID") == null)
             {
                return RedirectToAction("Index", "Login");
            }
            List<EmailGroupViewModel> iList = new List<EmailGroupViewModel>();
            iList = _AdminService.GetEmailGroupList();
            return View("EmailGroupMapping", iList);
        }

        [HttpGet]
        public ActionResult GroupMapping()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<SelectListViewModel> _LocationList = _AdminService.GetLocationOperationByTypeId(2);
            ViewBag.LocationList = new SelectList(_LocationList, "Value", "Text");
            List<SelectListViewModel> _OperationList = _AdminService.GetLocationOperationByTypeId(3);
            ViewBag.OperationList = new SelectList(_OperationList, "Value", "Text");
            return PartialView("_GroupMapping", new EmailGroupViewModel());
        }

        [HttpGet]
        public async Task<ActionResult> EditGroupMapping(long id)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<SelectListViewModel> _LocationList = _AdminService.GetLocationOperationByTypeId(2);
            //ViewBag.LocationList = new SelectList(_LocationList, "Value", "Text");

            ViewBag.LocationList = new MultiSelectList(_LocationList, "Value", "Text");

            List<SelectListViewModel> _OperationList = _AdminService.GetLocationOperationByTypeId(3);
           //ViewBag.OperationList = new SelectList(_OperationList, "Value", "Text");
            ViewBag.OperationList = new MultiSelectList(_OperationList, "Value", "Text");
            EmailGroupViewModel obj = _AdminService.GetDetailById(id);
            return  PartialView("_GroupMapping", obj);
        }

        [HttpPost]
        public ActionResult SaveEmailGroup([FromBody] EmailGroupViewModel model)
        {
            short retVal = 0; long hearderId = 0; string errmsg = "";
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                //model.ADDED_BY = Convert.ToInt64(Session["userID"].ToString());
                //model.UPDATED_BY = Convert.ToInt64(Session["userID"].ToString());
                model.ADDED_BY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                model.UPDATED_BY = Convert.ToInt64(_sessionService.Get<string>("userID"));

                Tuple<short, long> retVal_tuple = _AdminService.SaveEmailGroup(model);
                retVal = retVal_tuple.Item1;
                hearderId = retVal_tuple.Item2;
            }
            catch (Exception ex)
            {
                retVal = -1;
                errmsg = ex.StackTrace.ToString();
            }
            // return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }


        //// My Voice Method
        [HttpGet]
        public ActionResult MyVoiceCertificate()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<MyVoiceViewModel> iList = new List<MyVoiceViewModel>();
            string searchFilter = "0";
            DataTable dataTable = OBJHR.GetMyVoiceList(searchFilter);
            if (dataTable.Rows.Count > 0)
            {
                iList = (from DataRow row in dataTable.AsEnumerable()
                         select new MyVoiceViewModel
                         {
                             ID = row["IDEA_ID"].ToString(),
                             ECODE = row["ADEMPCODE"].ToString(),
                             EMP_NAME = row["EMP_NAME"].ToString(),
                             IDEA = Convert.ToString(row["IDEA"]),
                             IMPLEMENTED_ON = Convert.ToString(row["IMPLEMENTED"]),
                             STATUS = Convert.ToString(row["STATUS_NAME"]),
                             EMAIL_STATUS = Convert.ToString(row["EMAILSTATUS"]),
                             CERTIFICATE_NAME = string.IsNullOrEmpty(Convert.ToString(row["CERTIFICATE_NAME"])) ? "" : Convert.ToString(row["CERTIFICATE_NAME"]),
                             EMAILID = Convert.ToString(row["emailid"]),
                         }).OrderByDescending(o => o.IMPLEMENTED_ON).ToList();
            }
            return View("MyVoiceCertificate", iList);
        }


        [HttpPost]
        [AutoValidateAntiforgeryToken]
       public IActionResult MyVoiceCertificate(IFormCollection fc)
        {         
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            string searchFilter = fc["searchFilter"];
            List<MyVoiceViewModel> iList = new List<MyVoiceViewModel>();
            DataTable dataTable = OBJHR.GetMyVoiceList(searchFilter);
            if (dataTable.Rows.Count > 0)
            {
                iList = (from DataRow row in dataTable.AsEnumerable()
                         select new MyVoiceViewModel
                         {
                             ID = row["IDEA_ID"].ToString(),
                             ECODE = row["ADEMPCODE"].ToString(),
                             EMP_NAME = row["EMP_NAME"].ToString(),
                             IDEA = Convert.ToString(row["IDEA"]),
                             IMPLEMENTED_ON = Convert.ToString(row["IMPLEMENTED"]),
                             STATUS = Convert.ToString(row["STATUS_NAME"]),
                             EMAIL_STATUS = Convert.ToString(row["EMAILSTATUS"]),
                             CERTIFICATE_NAME = string.IsNullOrEmpty(Convert.ToString(row["CERTIFICATE_NAME"])) ? "" : Convert.ToString(row["CERTIFICATE_NAME"]),
                             EMAILID = Convert.ToString(row["emailid"]),
                         }).OrderByDescending(o => o.IMPLEMENTED_ON).ToList();
            }
            return View("MyVoiceCertificate", iList);
        }

        [HttpPost]
        // public ActionResult CertificateGeneration(string id, string name, string date, string mailTo, string certificateName)
        public ActionResult CertificateGeneration([FromBody] MyVoiceViewModel jobj)
        {
            string id="", name="", date = "", mailTo="", certificateName = "";
            if (jobj != null)
            {
                id = jobj.ID;
                name = jobj.EMP_NAME;
                date = jobj.IMPLEMENTED_ON;
                mailTo = jobj.EMAILID;
                certificateName = jobj.CERTIFICATE_NAME;
            }
            else
            {
                throw new Exception("Invalid Payload");
            }

            string retVal = ""; string emailStatus = "0"; string filePath = ""; string errmsg = "";
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                string strDSTFILENAME = "MyVoiceCertificate" + id + DateTime.Now.ToString("ddMMyyHHmmss") + ".pdf";
                string strSRCFILENAME = "TemplateCertificate.pdf";
                if (!string.IsNullOrEmpty(certificateName))
                {
                    //filePath = Server.MapPath("~/Uploads/MyVoice/" + certificateName);
                    filePath = _env.GetGeneralSettings().Get_FileUpload_Path + "MyVoice\\"+ certificateName;
                }
                else
                {
                    GetDocumentWithAppendedContent(name, date, strSRCFILENAME, strDSTFILENAME, 100); 
                    //filePath = Server.MapPath("~/Uploads/MyVoice/" + strDSTFILENAME);
                    filePath = _env.GetGeneralSettings().Get_FileUpload_Path + "MyVoice\\"+ strDSTFILENAME;
                }

                if (System.IO.File.Exists(filePath))
                {
                    if (!string.IsNullOrEmpty(mailTo))
                    {
                        short mailStatus = SendMail(name, mailTo, filePath);
                        if (mailStatus == 1)
                        {
                            if (string.IsNullOrEmpty(certificateName))
                            {
                                emailStatus = "1";
                                retVal = OBJHR.UpdateMyVoiceStatus(_sessionService.Get<string>("userID"), id.ToString(), emailStatus, strDSTFILENAME);
                                if (retVal != "1")
                                {
                                    retVal = "3"; // Mail sent succesfully, but status not updated
                                }
                                else
                                {
                                    retVal = "1"; // Mail sent succesfully, status has been updated.
                                }
                            }
                            else
                            {
                                retVal = "1"; // Mail sent succesfully
                            }
                        }
                        else
                        {
                            retVal = "2"; // Mail not send.
                        }
                    }
                    else
                    {
                        retVal = "4"; // Email ID not vaild
                    }
                }
                else
                {
                    retVal = "5"; // Certificate Generation failed.
                }
            }
            catch (Exception ex)
            {
                retVal = "-1";
                errmsg = ex.StackTrace.ToString();
            }
            //return Json(retVal, JsonRequestBehavior.AllowGet);
            return Json(retVal);
        }

        public void GetDocumentWithAppendedContent(string name, string date, string SRCfileName, string DSTfileName, int marginLeft)
        {
            int X = 0, Y = 0, APageNo = 0;
            GetSigLOCATION(SRCfileName, out X, out Y, out APageNo);
            Y += 90;

            //string srcPath = Path.Combine("Uploads", "MyVoice", SRCfileName);
            //string dstPath = Path.Combine("Uploads", "MyVoice", DSTfileName);
            string srcPath = _env.GetGeneralSettings().Get_FileUpload_Path + "MyVoice\\"+ SRCfileName;
            string dstPath = _env.GetGeneralSettings().Get_FileUpload_Path + "MyVoice\\"+ DSTfileName;

            using var writer = new PdfWriter(dstPath);
            using var pdfDoc = new PdfDocument(new PdfReader(srcPath), writer);
            var document = new iText.Layout.Document(pdfDoc);

            int pageCount = pdfDoc.GetNumberOfPages();
            for (int i = 1; i <= pageCount; i++)
            {
                PdfPage page = pdfDoc.GetPage(i);
                var canvas = new Canvas(page, page.GetPageSize());

                // Name
                var nameParagraph = new Paragraph(name)
     .SetFontSize(24)
     .SetFont(PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN));
                var nameDiv = new Div()
                .SetMargin(0)
                .SetPadding(0)
                .SetKeepTogether(true)
                .SetFontSize(24)
                .SetTextAlignment(TextAlignment.LEFT)
                .Add(nameParagraph)
                .SetFixedPosition(270, 325, 400);
                canvas.Add(nameDiv);

                // Date
                var dateParagraph = new Paragraph(date)
     .SetFontSize(16)
     .SetFont(PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN));
                var dateDiv = new Div()
                .SetMargin(0)
                .SetPadding(0)
                .SetKeepTogether(true)
                .SetFontSize(16)
                .SetTextAlignment(TextAlignment.LEFT)
                .Add(dateParagraph)
                .SetFixedPosition(365, 210, 250);
                canvas.Add(dateDiv);
            }

            document.Close();
        }

        public void GetSigLOCATION(string SRCfileName, out int X, out int Y, out int Pageno)
        {
            X = 0; Y = 0; Pageno = 0;

           // string path = Path.Combine(_env.WebRootPath, "Uploads", "MyVoice", SRCfileName);
            string path = _env.GetGeneralSettings().Get_FileUpload_Path+"MyVoice\\"+ SRCfileName;

            using var pdfReader = new PdfReader(path);
            using var pdfDoc = new PdfDocument(pdfReader);

            for (int pageNum = 1; pageNum <= pdfDoc.GetNumberOfPages(); pageNum++)
            {
                var page = pdfDoc.GetPage(pageNum);
                var strategy = new LocationTextExtractionStrategy();
                string text = PdfTextExtractor.GetTextFromPage(page, strategy);

                if (text.Contains("Signatories"))
                {
                    // Note: iText7's default strategy doesn't provide coordinates.
                    // For coordinates, you'd need a custom strategy or third-party library.
                    X = 100; // Placeholder
                    Y = 200; // Placeholder
                    Pageno = pageNum;
                    break;
                }
            }
        }

        //public void GetDocumentWithAppendedContent(string name, string date, string SRCfileName, string DSTfileName, int marginLeft)
        //{
        //    int X = 0, Y = 0, APageNo = 0;
        //    GetSigLOCATION(SRCfileName, out X, out Y, out APageNo);
        //    Y = Y + 90;

            //    //var stream = new MemoryStream();
            //   // string path = Server.MapPath("~/Uploads/MyVoice/" + SRCfileName);
            //    string path = Path.Combine(_env.WebRootPath, "Uploads/MyVoice", SRCfileName);
            //    //var writer = new PdfWriter(Server.MapPath("~/Uploads/MyVoice/" + DSTfileName));
            //    string filepath = Path.Combine(_env.WebRootPath, "Uploads/MyVoice/" + DSTfileName;
            //    var writer = new PdfWriter(filepath);

            //    var pdfResult = new PdfDocument(new PdfReader(path), writer);
            //    var document = new Document(pdfResult);

            //    int pagecount = pdfResult.GetNumberOfPages();
            //    for (int i = 1; i <= pagecount; i++)
            //    {
            //        Canvas canvas;
            //        PdfPage page = pdfResult.GetPage(i);

            //        var div = new Div();
            //        Paragraph pgr = new Paragraph();
            //        canvas = new Canvas(page, page.GetPageSize());
            //        pgr.Add(name);
            //        pgr.SetFontSize(24);
            //        PdfFont font = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
            //        pgr.SetFont(font);
            //        div = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(24).SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT);
            //        div.Add(pgr);
            //        div.SetFixedPosition(270, 325, 400);
            //        canvas.Add(div);

            //        var div2 = new Div();
            //        Paragraph pgr2 = new Paragraph();
            //        pgr2.Add(date);
            //        pgr2.SetFontSize(16);
            //        PdfFont font2 = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
            //        pgr.SetFont(font2);
            //        div2 = new Div().SetMargin(0).SetPadding(0).SetKeepTogether(true).SetFontSize(16).SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT);
            //        div2.Add(pgr2);
            //        div2.SetFixedPosition(365, 210, 250);
            //        canvas.Add(div2);
            //    }
            //    document.Close();
            //    pdfResult.Close();
            //}

            //public void GetSigLOCATION(string SRCfileName, out Int32 X, out Int32 Y, out Int32 Pageno)
            //{
            //    string path = Server.MapPath("~/Uploads/MyVoice/" + SRCfileName);

            //    X = 0; Y = 0; Pageno = 0;
            //    using (var reader = new iTextSharp.text.pdf.PdfReader(path))
            //    {

            //        for (int pagen = 1; pagen <= reader.NumberOfPages; pagen++)
            //        {
            //            var parser = new PdfReaderContentParser(reader);

            //            var strategy = parser.ProcessContent(pagen, new LocationTextExtractionStrategyWithPosition());
            //            var res = strategy.GetAbsoluteLocations();

            //            var searchResult = res.Where(p => p.Text.Contains("Signatories")).ToList();
            //            if (searchResult != null && searchResult.Count > 0)
            //            {
            //                foreach (var obj in searchResult)
            //                {
            //                    X = Convert.ToInt32(obj.Location.StartLocation[0]);
            //                    Y = Convert.ToInt32(obj.Location.StartLocation[1]);
            //                    Pageno = pagen;
            //                    break;
            //                }
            //            }
            //        }
            //        reader.Close();
            //    }
            //}

        public short SendMail(string name, string mailTo, string filePath)
        {
            short retVal = 0;
            try
            {
                
                EmailCore sendMail = new EmailCore();
                sendMail.MailFrom = "portal.admin@honda.hmsi.in";
                if (serverpath.isTestServer())
                    sendMail.MailTo = serverpath.getTestEMail();
                else
                   sendMail.MailTo = mailTo; //CR6852               

                string strSubject = "My Voice";
                string strBody = "<html>" +
                                    "<body>" +
                                        "<p>" +
                                            "Dear Associate," +
                                            "<br/><br/>" +
                                            "Greetings!" +
                                            "<br/><br/>" +
                                            "Thank you for sharing the idea. " +
                                            "<br/><br/>" +
                                            "We are delighted to inform you that your idea was duly accepted and has also been implemented. " +
                                            "<br/><br/>" +
                                            "It is a great achievement!Many congratulations for this commendable achievement!(Please find the certificate attached)" +
                                            "<br/><br/>" +
                                            "We wish you all the best for generating more new ideas and make our processes better." +
                                            "<br/><br/>" +
                                            "Best Regards," +
                                            "<br/>" +
                                            "HR&A Director" +
                                        "</p>" +
                                    "</body>" +
                                "</html>";

                sendMail.MailSubject = strSubject;
                sendMail.MailBody = strBody;
                List<string> Attachments = new List<string>();
                Attachments.Add(filePath);
                sendMail.AttachmentFilePath = Attachments;
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
            catch (Exception ex)
            {
                retVal = -1;
            }
            return retVal;
        }
    }
}
