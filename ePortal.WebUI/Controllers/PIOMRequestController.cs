using ePortal.Application.Contracts;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.ViewModels.DataExchange.IOM;
using ePortal.WebUI.Helpers;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using static ePortal.WebUI.Controllers.TokenBridgeController;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ePortal.WebUI.Controllers
{
    public class PIOMRequestController : Controller
    {
        private readonly IPIOMService _pIomService;
        private readonly ISessionService _sessionService;
        private readonly IConfiguration _settings;

        public PIOMRequestController(IPIOMService PiomService, ISessionService sessionService, IConfiguration settings)
        {
            _pIomService = PiomService;
            _sessionService = sessionService;
            _settings = settings;

        }
        [HttpPost]
        [AllowAnonymous]
        public JsonResult ReGenerateDoc([FromBody] RegenerateDocModel model)
        {
            short retVal = 0;
            string Error = "";
            string tokenUserId = "";
            try
            {

                // ===========================================
                //  🔥 INLINE JWT TOKEN VERIFICATION (Same as HandleFromNewApp)
                // ===========================================
                string authHeader = HttpContext.Request.Headers["Authorization"];

                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Json(new { res = -1, err_msg = "Unauthorized: Token Missing" });
                }

                string token = authHeader.Replace("Bearer ", "").Trim();

                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(_settings["Switch_New_Old_New:SecretKey"].ToString());

                    // 🔥 SAME VALIDATION PARAMETERS AS HandleFromNewApp
                    var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = "OldApp",

                        ValidateAudience = true,
                        ValidAudience = "NewApp",

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),

                        //ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1)

                    }, out SecurityToken validatedToken);

                    // 🔥 SAME decrypt logic
                    var encData = principal.FindFirst("encData")?.Value;

                    if (string.IsNullOrEmpty(encData))
                        return Json(new { res = -1, err_msg = "Invalid Token: encData missing" });

                    var json = SecurityHelper.AESDecrypt(encData, _settings["Switch_New_Old_New:SecretKey"].ToString());

                    var user = JsonConvert.DeserializeObject<UserSessionModel>(json);
                    if (user == null)
                    {
                        return Json(new { res = -1, err_msg = "Invalid Token: user data invalid" });
                    }
                     tokenUserId = user.userId;

                    if (string.IsNullOrEmpty(tokenUserId))
                        return Json(new { res = -1, err_msg = "Invalid Token: UserId missing" });

                    // 🍀 VERIFIED USER ID IS READY TO USE BELOW
                }
                catch (SecurityTokenException ex)
                {
                    return Json(new { res = -1, err_msg = "Unauthorized: Invalid or expired token" });
                }
                catch (Exception ex)
                {
                    return Json(new { res = -1, err_msg = "Token Validation Error: " + ex.Message });
                }


                // ===========================================
                //  🔥 JWT Verification Completed
                // ===========================================




                IOMPHeaderViewModel IOM_Dtl = _pIomService.GetPIOMRequestById(Convert.ToInt64(model.id));

                if (IOM_Dtl != null)
                {
                    if (IOM_Dtl.PROCESS_STATUS == 2)
                    {
                        var OBJAPP = IOM_Dtl.iompAppHis.Where(M => M.ApprovalStatus == 0).FirstOrDefault();
                        if (OBJAPP == null)
                        {
                            var objiomfile = IOM_Dtl.iompDetail.Where(m => m.DOC_TYPE == "IOM");
                            if (objiomfile == null || objiomfile.Count() == 0)
                            {
                                throw new Exception("IOM File Not Found");
                            }

                            string path = Path.Combine(serverpath.getFileUploadPath(), "DGIT_IOM");
                            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }

                            string srcFile = Path.Combine(path, objiomfile.FirstOrDefault().FILENAME);
                            string STRFILENAME = "IOM_" + IOM_Dtl.IOMHEADERID + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".pdf";
                            string strDstFile = Path.Combine(path, STRFILENAME);

                            List<IOMPSIGLIST> DocSiglist = new List<IOMPSIGLIST>();
                            List<IOMPAppHistoryViewModel> allApprovers = new List<IOMPAppHistoryViewModel>();

                            if (IOM_Dtl.Stages?.Any() == true)
                            {
                                allApprovers = IOM_Dtl.Stages
                                    .OrderBy(s => s.StageOrder)
                                    .SelectMany(stage => stage.ParallelGroups.OrderBy(g => g.GroupOrder))
                                    .SelectMany(group => group.Approvers)
                                    .Select(a => new IOMPAppHistoryViewModel
                                    {
                                        ApprovalDate = DateTime.Now,
                                        EmpName = a.ADEMPNAME,
                                        SequenceNo = a.APP_SEQ,
                                        Header = a.Header
                                    })
                                    .ToList();

                                DocSiglist = allApprovers
                                    .Select(a => new IOMPSIGLIST
                                    {
                                        Designation = a.Header,
                                        appList = new List<IOMPAppHistoryViewModel> { a }
                                    })
                                    .ToList();
                            }

                            string _date = IOM_Dtl.DATEADDED != null ? IOM_Dtl.DATEADDED.ToString("dd-MMM-yyyy") : "";

                            Thread bgThread = new Thread(async () =>
                                await PIOMAnnotationPdf(null, null, srcFile, strDstFile, DocSiglist, _date, IOM_Dtl.ADDEDBYNAME, true)
                            );
                            bgThread.Start();

                            List<IOMPDetailViewModel> obj = new List<IOMPDetailViewModel>()
                    {
                        new IOMPDetailViewModel()
                        {
                            DOC_TYPE = "IOMA",
                            FILENAME = STRFILENAME,
                            ADDITIONAL_INFO = "Approved PIOM"
                        }
                    };

                            _pIomService.SavePIOMAttachment(Convert.ToInt64(tokenUserId), IOM_Dtl.IOMHEADERID, obj);
                            retVal = 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                retVal = -1;
            }

            return Json(new { res = retVal, err_msg = Error });
        }
        private async Task PIOMAnnotationPdf(IOMPAppHistoryViewModel IHVM, Employee_Details emp_dtl, string srcPath, string dstPath, List<IOMPSIGLIST> objlist, string _date = "", string initiator = "", bool forRegenerate = true)
        {
            try
            {
                var writer = new PdfWriter(dstPath);
                var pdfResult = new PdfDocument(new PdfReader(srcPath), writer);
                var pdfDoc = new Document(pdfResult);
                PdfFont f = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                var initiatorName = initiator ?? "Unknown";
                Paragraph paragraph = new Paragraph("Approved")
                    .SetFont(f)
                    .SetFontSize(70);

                PdfExtGState gs1 = new PdfExtGState().SetFillOpacity(0.5f);
                const float tablePadding = 5f;
                const int maxSignatoriesPerTable = 10;
                const float tableHeightEstimate = 50f;

                // Add watermark to all pages
                for (int pageno = 1; pageno <= pdfResult.GetNumberOfPages(); pageno++)
                {
                    PdfPage page = pdfResult.GetPage(pageno);
                    iText.Kernel.Geom.Rectangle pageSize = page.GetPageSizeWithRotation();
                    page.SetIgnorePageRotationForContent(true);
                    float x = (pageSize.GetLeft() + pageSize.GetRight()) / 2;
                    float y = (pageSize.GetTop() + pageSize.GetBottom()) / 2;
                    PdfCanvas over = new PdfCanvas(page);
                    over.SaveState();
                    over.SetExtGState(gs1);
                    float rotationAngle = (float)(Math.PI / 4);
                    pdfDoc.ShowTextAligned(paragraph, x, y, pageno, TextAlignment.CENTER, VerticalAlignment.MIDDLE, rotationAngle);
                    over.RestoreState();
                }

                // Add table only to the last page if objlist has items
                int lastPageNo = pdfResult.GetNumberOfPages();
                if (lastPageNo > 0 && objlist != null && objlist.Count > 0)
                {
                    iText.Kernel.Geom.Rectangle lastPageSize = pdfResult.GetLastPage().GetPageSizeWithRotation();
                    float leftX = 10f;
                    float bottomY = 20f;
                    float contentWidth = lastPageSize.GetWidth() - 20f;

                    // Calculate total tables needed
                    int totalTables = (int)Math.Ceiling(objlist.Count / (double)maxSignatoriesPerTable);
                    float startY = bottomY + (tableHeightEstimate + tablePadding) * (totalTables - 1);

                    int startIndex = 0;
                    bool isFirstTable = true;
                    int tblCounter = 0;
                    while (startIndex < objlist.Count)
                    {
                        int remaining = objlist.Count - startIndex;
                        int signatoriesInThisTable = Math.Min(maxSignatoriesPerTable, remaining);
                        int numCols = isFirstTable ? signatoriesInThisTable + 1 : signatoriesInThisTable;

                        if (numCols == 0) break;

                        float[] columnWidths = new float[numCols];
                        for (int j = 0; j < numCols; j++)
                            columnWidths[j] = 100f / numCols;

                        Table table = new Table(UnitValue.CreatePercentArray(columnWidths))
                            .SetWidth(UnitValue.CreatePercentValue(100));

                        // Header Row
                        if (isFirstTable)
                        {
                            var initiatorHeader = new Cell()
                                .SetBackgroundColor(new DeviceGray(0.75f))
                                .SetFont(f)
                                .SetFontSize(7)
                                .Add(new Paragraph("Initiator"))
                                .SetTextAlignment(TextAlignment.CENTER)
                                .SetVerticalAlignment(VerticalAlignment.MIDDLE);
                            table.AddHeaderCell(initiatorHeader);
                        }
                        for (int i = 0; i < signatoriesInThisTable; i++)
                        {
                            var item = objlist[startIndex + i];
                            var headerCell = new Cell()
                                .SetBackgroundColor(new DeviceGray(0.75f))
                                .SetFont(f)
                                .SetFontSize(7)
                                .Add(new Paragraph(item.Designation))
                                .SetTextAlignment(TextAlignment.CENTER)
                                .SetVerticalAlignment(VerticalAlignment.MIDDLE);
                            table.AddHeaderCell(headerCell);
                        }

                        // Data Row
                        if (isFirstTable)
                        {
                            var initiatorData = new Cell()
                                .SetBackgroundColor(new DeviceGray(0.9f))
                                .SetTextAlignment(TextAlignment.CENTER)
                                .SetFont(f)
                                .SetFontSize(6)
                                .Add(new Paragraph(initiatorName))
                                .SetVerticalAlignment(VerticalAlignment.MIDDLE);
                            table.AddCell(initiatorData);
                        }
                        for (int i = 0; i < signatoriesInThisTable; i++)
                        {
                            var item = objlist[startIndex + i];
                            string empNames = string.Join("\n", item.appList.Select(a => a.EmpName));


                            string content = string.Join("\n", item.appList
                           .Select(s => $"{s.EmpName}\n{s.ApprovalDate?.ToString("dd-MMM-yyyy")}"));
                            var dataCell = new Cell()
                                .SetTextAlignment(TextAlignment.CENTER)
                                .SetFont(f)
                                .SetFontSize(6)
                                .Add(new Paragraph(content))
                                .SetVerticalAlignment(VerticalAlignment.MIDDLE);

                            table.AddCell(dataCell);
                        }

                        // Position table from the bottom upwards
                        float tblY = startY - tblCounter * (tableHeightEstimate + tablePadding);
                        Div contentDiv = new Div()
                            .SetFixedPosition(lastPageNo, leftX, tblY, contentWidth);
                        table.SetMarginBottom(tablePadding);
                        contentDiv.Add(table);
                        pdfDoc.Add(contentDiv);

                        startIndex += signatoriesInThisTable;
                        isFirstTable = false;
                        tblCounter++;
                    }
                }

                pdfDoc.Close();
                pdfResult.Close();
                await Task.FromResult(0);

                if (!forRegenerate)
                {
                    // Added by TTL CR5813 | Send pdf to initiator
                    _pIomService.SendMailAnnotatedPdfOnFinalApproval(IHVM, IHVM.ApprovalStatus, emp_dtl, dstPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in IOMAnnotationPdfPreview: " + ex.Message);
            }
        }

    }
}
