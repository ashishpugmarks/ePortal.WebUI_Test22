using ePortal.Application.Contracts;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using iText.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.StyledXmlParser.Jsoup.Helper;
using System.Net;
using ePortal.ViewModels.DataExchange.IOM;
using Microsoft.AspNetCore.Authorization;

namespace ePortal.WebUI.Controllers
{
    public class PIOMController : Controller
    {
        private readonly IPIOMService _pIomService;
        private readonly ISessionService _sessionService;
        private readonly string _userId;
        private readonly string _userName;
        private readonly Employee_Details _EmpDetails;

        public PIOMController(IPIOMService PiomService, ISessionService sessionService)
        {
            _pIomService = PiomService;
            _sessionService = sessionService;
            _userId = _sessionService.Get<string>("userID").ToString();
            _userName = _sessionService.Get<string>("userName").ToString();
            _EmpDetails = _sessionService.Get<Employee_Details>("Employee");
        }
        [HttpPost]
        public ActionResult PIOMRequest([FromBody] IOMPHeaderViewModel model)
        {
            short retVal = 0;
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                model.iompDetail = new List<IOMPDetailViewModel>();
                model.ADDEDBY = Convert.ToInt64(_userId.ToString());
                model.UPDATEDBY = Convert.ToInt64(_userId.ToString());


                Tuple<short, long> retVal_tuple = _pIomService.SavePIOMRequest(model);
                retVal = retVal_tuple.Item1;

            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }
        [HttpPost]
        public ActionResult UploadIOMAttachment(IOMPDetailViewModel formData)
        {
            short retVal = 0; long _headerId = 0;
            List<IOMPDetailViewModel> poDtlList = new List<IOMPDetailViewModel>();
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (formData.IOMHEADERID > 0)
                {
                    _headerId = formData.IOMHEADERID;
                }
                else
                {
                    IOMPHeaderViewModel model = new IOMPHeaderViewModel();
                    model.IsFinalSubmit = 0;
                    model.PROCESS_STATUS = 0;
                    model.IOMDesc = formData.IOMDESC;
                    model.ADDEDBY = Convert.ToInt64(_userId.ToString());
                    model.UPDATEDBY = Convert.ToInt64(_userId.ToString());
                    model.STATUS = 1;
                    Tuple<short, long> retVal_tuple = _pIomService.SavePIOMRequest(model);
                    if (retVal_tuple.Item1 == 1 && retVal_tuple.Item2 > 0)
                    {
                        _headerId = retVal_tuple.Item2;
                    }
                    else
                    {
                        return Json(new { res = retVal_tuple.Item1, headerId = _headerId, iomAttachment = poDtlList.Where(w => w.DOC_TYPE == "IOM").FirstOrDefault() });
                    }
                }
                if (formData.FILE.Length > 0 && !string.IsNullOrEmpty(formData.DOC_TYPE))
                {
                    //string path = Server.MapPath("~/Uploads/DGIT_IOM/");
                    string path = Path.Combine(serverpath.getFileUploadPath(), "DGIT_IOM");
                    string pathtosave = DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString();
                    if (!Directory.Exists(path + @"\" + pathtosave)) { Directory.CreateDirectory(path + @"\" + pathtosave); }
                    FileViewModel _file = GetUploadFile(formData.FILE, formData.DOC_TYPE, _headerId.ToString());
                    poDtlList.Add(new IOMPDetailViewModel
                    {
                        IOMHEADERID = formData.IOMHEADERID,
                        FILENAME = pathtosave + @"\" + _file.FileName,
                        FILE_CONTENTTYPE = _file.FileContentType,
                        FILE_BYTE = _file.File,
                        DOC_TYPE = formData.DOC_TYPE,
                        ADDITIONAL_INFO = formData.ADDITIONAL_INFO,
                        ADDEDBY = Convert.ToInt64(_userId.ToString()),
                    });
                    Tuple<short, List<IOMPDetailViewModel>> _ret_tuple = _pIomService.SavePIOMAttachment(Convert.ToInt64(_userId.ToString()), _headerId, poDtlList);
                    retVal = _ret_tuple.Item1;
                    if (retVal == 1)
                    {
                        if (poDtlList.Count > 0)
                        {
                            foreach (IOMPDetailViewModel obj in poDtlList)
                            {
                                if (obj.IsDeleted == 0 && obj.FILE_BYTE != null)
                                {
                                    System.IO.File.WriteAllBytes(Path.Combine(path, obj.FILENAME), obj.FILE_BYTE.ToArray());
                                }
                            }
                        }
                    }
                    poDtlList = _ret_tuple.Item2;
                }

            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(new { res = retVal, headerId = _headerId, iomAttachment = poDtlList.Where(w => w.DOC_TYPE == "IOM").FirstOrDefault() });
        }

        public ActionResult DeleteParallelAppAuthority([FromQuery] string eCode, [FromQuery] string header, [FromQuery] string ISALL, [FromQuery] int stageId)
        {
            short retval = 0;
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                List<IOMAppAuthSeqViewModel> AuthSeqList = new List<IOMAppAuthSeqViewModel>();

                if (TempData["APPROVAL_Parallel_AUTH_LIST"] != null)
                {
                    AuthSeqList = JsonConvert.DeserializeObject<List<IOMAppAuthSeqViewModel>>(TempData["APPROVAL_Parallel_AUTH_LIST"].ToString());
                }

                if (ISALL == "0")
                {
                    if (AuthSeqList.Count > 0)
                    {
                        // Remove item from the specified stage
                        //AuthSeqList.RemoveAll(r => r.ADEMPCODE == Convert.ToInt64(eCode) && r.Header == header && r.StageId == stageId);

                        var AuthSeqListOrdrBy = AuthSeqList.OrderBy(o => o.APP_SEQ).ToList();
                        TempData["APPROVAL_Parallel_AUTH_LIST"] = JsonConvert.SerializeObject(AuthSeqListOrdrBy);
                        retval = 1;

                        //var thisStageList = AuthSeqList.Where(x => x.StageId == stageId).OrderBy(o => o.APP_SEQ).ToList();
                        //return Json(new { RESULT = retval, SEQ_LIST = thisStageList });
                    }
                }
                else if (ISALL == "1")
                {
                    // Clear all approval data for all stages
                    AuthSeqList.Clear();
                    TempData["APPROVAL_Parallel_AUTH_LIST"] = JsonConvert.SerializeObject(AuthSeqList);
                    TempData["Designation"] = JsonConvert.SerializeObject("");
                    TempData.Keep();
                    retval = 1;

                    return Json(new { RESULT = retval, SEQ_LIST = new List<IOMAppAuthSeqViewModel>() });
                }

                return Json(new { RESULT = retval, SEQ_LIST = new List<IOMAppAuthSeqViewModel>() });
            }
            catch (Exception ex)
            {
                return Json(new { RESULT = -1 });
            }
        }
        private FileViewModel GetUploadFile(IFormFile file, string DocType, string headerid)
        {
            try
            {
                FileViewModel FVM = new FileViewModel();
                if (file != null && file.Length > 0)
                {
                    byte[] bytes;
                    //using (BinaryReader br = new BinaryReader(file.InputStream))
                    //{
                    //    bytes = br.ReadBytes(file.ContentLength);
                    //}
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        bytes = ms.ToArray();
                    }
                    //string _FileName = file.FileName.Substring(file.FileName.LastIndexOf("\\") + 1);
                    string _FileName = Path.GetFileName(file.FileName);
                    string strExtensionName = Path.GetExtension(file.FileName);
                    //FVM.FileContentType = MimeMapping.GetMimeMapping(_FileName);
                    FVM.FileContentType = file.ContentType;
                    FVM.FileName = DocType + headerid + "_" + DateTime.Now.ToString("ddMMyyHHmmss") + strExtensionName;
                    FVM.File = bytes;
                }
                return FVM;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Preview Approval note

        //[HttpPost]
        //public ActionResult PreviewPIOM([FromBody] IOMPHeaderViewModel oHeader)
        //{
        //    try
        //    {

        //        List<IOMPSIGLIST> DocSiglist = new List<IOMPSIGLIST>();

        //        if (oHeader.Stages != null && oHeader.Stages.Any())
        //        {
        //            foreach (var stage in oHeader.Stages.OrderBy(s => s.StageOrder))
        //            {
        //                foreach (var group in stage.ParallelGroups.OrderBy(g => g.GroupOrder))
        //                {
        //                    IOMPSIGLIST objsig = new IOMPSIGLIST
        //                    {
        //                        StageID = stage.StageID,
        //                        StageName = stage.StageName,
        //                        StageOrder = stage.StageOrder,
        //                        ParallelGroupID = group.ParallelGroupID,
        //                        GroupName = group.GroupName,
        //                        GroupOrder = group.GroupOrder,
        //                        appList = new List<IOMPAppHistoryViewModel>()
        //                    };

        //                    int seqCounter = 1;

        //                    foreach (var approver in group.Approvers.OrderBy(a => a.APP_SEQ == 0 ? seqCounter++ : a.APP_SEQ))
        //                    {
        //                        objsig.appList.Add(new IOMPAppHistoryViewModel
        //                        {
        //                            EmpName = approver.ADEMPNAME,
        //                            ApprovalDate = DateTime.Now,
        //                            StageID = stage.StageID,
        //                            StageName = stage.StageName,
        //                            ParallelGroupID = group.ParallelGroupID,
        //                            GroupName = group.GroupName,
        //                            SequenceNo = approver.APP_SEQ == 0 ? seqCounter : approver.APP_SEQ,
        //                            Header = approver.Header
        //                        });
        //                    }

        //                    DocSiglist.Add(objsig);
        //                }
        //            }
        //        }


        //        string fileName = oHeader.IOMATTACHMENT;
        //        string path = Path.Combine(serverpath.getFileUploadPath(), "DGIT_IOM");
        //        if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }

        //        string srcPath = Path.Combine(path, fileName);
        //        string destfilename = "IOM" + oHeader.IOMHEADERID + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".pdf";
        //        var path2 = Path.Combine(serverpath.getFileUploadPath(), "DGIT_IOM", "Temp");
        //        string destPath = Path.Combine(path2, destfilename);
        //        if (!Directory.Exists(path2)) { Directory.CreateDirectory(path2); }

        //        // ========== GENERATE PDF ==========
        //        IOMAnnotationPdfPreview(srcPath, destPath, DocSiglist);

        //        string file_path = "../../../Uploads/DGIT_IOM/Temp/";
        //        string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"288px\"></object>";
        //        string _path = string.Format(embed, file_path + destfilename);

        //        return Json(new { FILEPATH = _path });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { FILEPATH = "" });
        //    }
        //}

        //private void IOMAnnotationPdfPreview(
        //    string srcPath,
        //    string dstPath,
        //    List<IOMPSIGLIST> objlist)
        //{
        //    var initiatorName = _userName.ToString();
        //    var writer = new PdfWriter(dstPath);
        //    var pdfResult = new PdfDocument(new PdfReader(srcPath), writer);
        //    var pdfDoc = new Document(pdfResult);

        //    PdfFont f = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
        //    PdfExtGState gs1 = new PdfExtGState().SetFillOpacity(0.5f);
        //    Paragraph watermark = new Paragraph("Preview")
        //        .SetFont(f).SetFontSize(40)
        //        .SetFontColor(ColorConstants.LIGHT_GRAY);
        //    for (int pageno = 1; pageno <= pdfResult.GetNumberOfPages(); pageno++)
        //    {
        //        PdfPage page = pdfResult.GetPage(pageno);
        //        var pageSize = page.GetPageSizeWithRotation();

        //        float x = (pageSize.GetLeft() + pageSize.GetRight()) / 2;
        //        float y = (pageSize.GetTop() + pageSize.GetBottom()) / 2;

        //        PdfCanvas over = new PdfCanvas(page);
        //        over.SaveState();
        //        over.SetExtGState(gs1);

        //        pdfDoc.ShowTextAligned(watermark, x, y, pageno,
        //            TextAlignment.CENTER, VerticalAlignment.MIDDLE, 0);

        //        over.RestoreState();
        //    }

        //    // Get the new page size for positioning
        //    var newPageSize = pdfResult.GetLastPage().GetPageSizeWithRotation();
        //    float leftX = 20f; // Left margin
        //    float bottomY = 20f; // Bottom margin
        //    float width = newPageSize.GetWidth() - 40f; // Full width minus margins
        //    int lastPageNumber = pdfResult.GetNumberOfPages();

        //    // Create a fixed-position div to hold all the content at the bottom of the last page
        //    Div contentDiv = new Div()
        //        .SetFixedPosition(lastPageNumber, leftX, bottomY, width);

        //    // Removed maxApprovers calculation as we manage dynamically now
        //    float[] initColWidths = new float[] { 2, 2 };
        //    Table initTable = new Table(UnitValue.CreatePercentArray(initColWidths)).SetWidth(UnitValue.CreatePercentValue(100));

        //    initTable.AddCell(new Cell().SetFont(f).SetFontSize(6).Add(new Paragraph("Initiator")).SetBackgroundColor(new DeviceGray(0.9f)));
        //    initTable.AddCell(new Cell().SetFont(f).SetFontSize(6).Add(new Paragraph(initiatorName)).SetBackgroundColor(new DeviceGray(0.9f)));

        //    contentDiv.Add(initTable);

        //    // Get distinct stages ordered
        //    var stages = objlist.Select(o => new { o.StageOrder, o.StageName })
        //                        .Distinct()
        //                        .OrderBy(s => s.StageOrder);

        //    foreach (var stage in stages)
        //    {
        //        // Stage header as paragraph
        //        Paragraph stageHeader = new Paragraph($"{stage.StageName}")
        //            .SetFont(f).SetFontSize(8)
        //            .SetBackgroundColor(new DeviceGray(0.85f))
        //            .SetTextAlignment(TextAlignment.CENTER)
        //            .SetMarginTop(10).SetMarginBottom(0);
        //        contentDiv.Add(stageHeader);

        //        // Get groups for this stage ordered
        //        var groups = objlist.Where(o => o.StageOrder == stage.StageOrder)
        //                            .Select(o => new { o.GroupOrder, o.GroupName })
        //                            .Distinct()
        //                            .OrderBy(g => g.GroupOrder);

        //        foreach (var group in groups)
        //        {
        //            // Group header paragraph
        //            Paragraph groupHeader = new Paragraph($"{group.GroupName}")
        //                .SetFont(f).SetFontSize(7)
        //                .SetBackgroundColor(new DeviceGray(0.92f))
        //                .SetTextAlignment(TextAlignment.LEFT)
        //                .SetMarginTop(0).SetMarginBottom(0);
        //            contentDiv.Add(groupHeader);

        //            // Get approvers for this stage + group as single list
        //            var groupData = objlist
        //                .Where(o => o.StageOrder == stage.StageOrder && o.GroupOrder == group.GroupOrder)
        //                .SelectMany(o => o.appList ?? new List<IOMPAppHistoryViewModel>())
        //                .ToList();

        //            int columnsPerRow = 8;
        //            int totalApprovers = groupData.Count;
        //            int totalRows = (int)Math.Ceiling(totalApprovers / (double)columnsPerRow);

        //            for (int row = 0; row < totalRows; row++)
        //            {
        //                int recordsInRow = Math.Min(columnsPerRow, totalApprovers - row * columnsPerRow);
        //                float[] groupColWidths = Enumerable.Repeat(1f, recordsInRow).ToArray();
        //                Table approverTable = new Table(UnitValue.CreatePercentArray(groupColWidths))
        //                    .SetWidth(UnitValue.CreatePercentValue(100));

        //                // Header row
        //                for (int col = 0; col < recordsInRow; col++)
        //                {
        //                    int index = row * columnsPerRow + col;
        //                    var sig = groupData[index];
        //                    approverTable.AddCell(new Cell()
        //                        .SetFont(f).SetFontSize(6)
        //                        .Add(new Paragraph($"{sig.Header ?? ""}"))
        //                        .SetTextAlignment(TextAlignment.CENTER)
        //                        .SetBackgroundColor(new DeviceGray(0.9f))
        //                        .SetMarginTop(0).SetMarginBottom(0));
        //                }

        //                // Name row
        //                for (int col = 0; col < recordsInRow; col++)
        //                {
        //                    int index = row * columnsPerRow + col;
        //                    var sig = groupData[index];
        //                    approverTable.AddCell(new Cell()
        //                        .SetFont(f).SetFontSize(6)
        //                        .Add(new Paragraph($"{sig.EmpName}"))
        //                        .SetTextAlignment(TextAlignment.CENTER)
        //                        .SetMarginTop(0).SetMarginBottom(0));
        //                }

        //                contentDiv.Add(approverTable);
        //            }
        //        }
        //    }

        //    // Add the fixed-position div to the document
        //    pdfDoc.Add(contentDiv);

        //    pdfDoc.Close();
        //    pdfResult.Close();
        //}

        [HttpPost]
        public ActionResult PreviewPIOM([FromBody] IOMPHeaderViewModel oHeader)
        {
            try
            {
                List<IOMPSIGLIST> DocSiglist = new List<IOMPSIGLIST>();
                List<IOMPAppHistoryViewModel> allApprovers = new List<IOMPAppHistoryViewModel>();

                if (oHeader.Stages?.Any() == true)
                {
                    allApprovers = oHeader.Stages
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

                // ✅ Get data from DB
                var data = _pIomService.GetPIOMRequestById(oHeader.IOMHEADERID);
                var getdata = data.iompDetail
                    .Where(x => x.DOC_TYPE == "IOM")
                    .Select(s => s.FILENAME)
                    .FirstOrDefault();

                // ✅ Use model attachment if available, else fallback to DB value
                string fileName = !string.IsNullOrEmpty(oHeader.IOMATTACHMENT)
                    ? oHeader.IOMATTACHMENT
                    : getdata;

                // ✅ If still null/empty, return warning
                if (string.IsNullOrEmpty(fileName))
                {
                    return Json(new { FILEPATH = "", MESSAGE = "IOM attachment not found." });
                }

                // ✅ Continue PDF generation
                string path = Path.Combine(serverpath.getFileUploadPath(), "DGIT_IOM");
                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }

                string srcPath = Path.Combine(path, fileName);
                string destfilename = "IOM" + oHeader.IOMHEADERID + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".pdf";
                var path2 = Path.Combine(serverpath.getFileUploadPath(), "DGIT_IOM", "Temp");
                string destPath = Path.Combine(path2, destfilename);
                if (!Directory.Exists(path2)) { Directory.CreateDirectory(path2); }

                // ========== GENERATE PDF ==========
                IOMAnnotationPdfPreview(srcPath, destPath, DocSiglist);

                string file_path = "../../../Uploads/DGIT_IOM/Temp/";
                string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"288px\"></object>";
                string _path = string.Format(embed, file_path + destfilename);

                return Json(new { FILEPATH = _path });
            }
            catch (Exception ex)
            {
                return Json(new { FILEPATH = "" });
            }
        }

        private void IOMAnnotationPdfPreview(string srcPath, string dstPath, List<IOMPSIGLIST> objlist)
        {
            try
            {
                var writer = new PdfWriter(dstPath);
                var pdfResult = new PdfDocument(new PdfReader(srcPath), writer);
                var pdfDoc = new Document(pdfResult);
                PdfFont f = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                var initiatorName = _userName?.ToString() ?? "Unknown";
                Paragraph paragraph = new Paragraph("Preview")
                    .SetFont(f)
                    .SetFontSize(70);

                PdfExtGState gs1 = new PdfExtGState().SetFillOpacity(0.5f);
                const float tablePadding = 5f;
                const int maxSignatoriesPerTable = 10;
                const float tableHeightEstimate = 50f;


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

                // Add tables only to the last page if objlist has items
                int lastPageNo = pdfResult.GetNumberOfPages();
                if (lastPageNo > 0 && objlist != null && objlist.Count > 0)
                {
                    iText.Kernel.Geom.Rectangle lastPageSize = pdfResult.GetLastPage().GetPageSizeWithRotation();
                    float leftX = 10f;
                    float bottomY = 20f;
                    float contentWidth = lastPageSize.GetWidth() - 20f;
                    float currentY = bottomY;

                    // --- Compute total tables for stacking ---
                    int totalTables = (int)Math.Ceiling((objlist.Count) / (double)maxSignatoriesPerTable);
                    // Calculate where to start the topmost table (so that last is at bottomY)
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

                        // Headers
                        if (isFirstTable)
                        {
                            // Initiator header
                            var initiatorHeader = new Cell()
                                .SetBackgroundColor(new DeviceGray(0.75f))
                                .SetFont(f)
                                .SetFontSize(7)
                                .Add(new Paragraph("Initiator"))
                                .SetTextAlignment(TextAlignment.CENTER)
                                .SetVerticalAlignment(VerticalAlignment.MIDDLE);
                            table.AddHeaderCell(initiatorHeader);
                        }
                        // Signatory headers for this chunk
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

                        // Data row
                        if (isFirstTable)
                        {
                            // Initiator data
                            var initiatorData = new Cell()
                                .SetBackgroundColor(new DeviceGray(0.9f))
                                .SetTextAlignment(TextAlignment.CENTER)
                                .SetFont(f)
                                .SetFontSize(6)
                                .Add(new Paragraph(initiatorName))
                                .SetVerticalAlignment(VerticalAlignment.MIDDLE);
                            table.AddCell(initiatorData);
                        }
                        // Signatory data for this chunk
                        for (int i = 0; i < signatoriesInThisTable; i++)
                        {
                            var item = objlist[startIndex + i];

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
                            
                        // Stack from bottom up: decrement Y for each table
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
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in IOMAnnotationPdfPreview: " + ex.Message);
            }
        }


        //private void IOMAnnotationPdfPreview(string srcPath, string dstPath, List<IOMPSIGLIST> objlist)
        //{
        //    try
        //    {
        //        var writer = new PdfWriter(dstPath);
        //        var pdfResult = new PdfDocument(new PdfReader(srcPath), writer);
        //        var pdfDoc = new Document(pdfResult);
        //        PdfFont f = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
        //        var initiatorName = _userName?.ToString() ?? "Unknown"; // Fallback if _userName is null
        //        Paragraph paragraph = new Paragraph("Preview")
        //            .SetFont(f)
        //            .SetFontSize(40);

        //        PdfExtGState gs1 = new PdfExtGState().SetFillOpacity(0.5f);
        //        const float tablePadding = 5f; // Space between tables

        //        // First, add watermark to all pages
        //        for (int pageno = 1; pageno <= pdfResult.GetNumberOfPages(); pageno++)
        //        {
        //            PdfPage page = pdfResult.GetPage(pageno);
        //            iText.Kernel.Geom.Rectangle pageSize = page.GetPageSizeWithRotation();
        //            page.SetIgnorePageRotationForContent(true);
        //            float x = (pageSize.GetLeft() + pageSize.GetRight()) / 2;
        //            float y = (pageSize.GetTop() + pageSize.GetBottom()) / 2;
        //            PdfCanvas over = new PdfCanvas(page);
        //            over.SaveState();
        //            over.SetExtGState(gs1);
        //            pdfDoc.ShowTextAligned(paragraph, x, y, pageno, TextAlignment.CENTER, VerticalAlignment.TOP, 0);
        //            over.RestoreState();
        //        }

        //        // Then, add tables only to the last page using a fixed-position Div at the bottom
        //        int lastPageNo = pdfResult.GetNumberOfPages();
        //        if (lastPageNo > 0 && objlist != null && objlist.Count > 0)
        //        {
        //            iText.Kernel.Geom.Rectangle lastPageSize = pdfResult.GetLastPage().GetPageSizeWithRotation();
        //            float leftX = 10f;
        //            float bottomY = 20f;
        //            float contentWidth = lastPageSize.GetWidth() - 20f;

        //            Div contentDiv = new Div()
        //                .SetFixedPosition(lastPageNo, leftX, bottomY, contentWidth);

        //            int currentIdx = 0;
        //            bool includeInitiator = true; // Only for first table

        //            while (currentIdx < objlist.Count)
        //            {
        //                bool isFirstTable = includeInitiator;
        //                int approversInThisChunk;
        //                if (isFirstTable)
        //                {
        //                    approversInThisChunk = Math.Min(9, objlist.Count - currentIdx);
        //                    includeInitiator = false; // Set false after deciding this chunk
        //                }
        //                else
        //                {
        //                    approversInThisChunk = Math.Min(10, objlist.Count - currentIdx);
        //                }

        //                if (approversInThisChunk == 0) break; // Safety

        //                int numCols = isFirstTable ? approversInThisChunk + 1 : approversInThisChunk;

        //                // Dynamic column widths summing to 100%
        //                float[] columnWidths = new float[numCols];
        //                float colWidth = 100f / numCols;
        //                for (int j = 0; j < numCols; j++)
        //                {
        //                    columnWidths[j] = colWidth;
        //                }

        //                Table table = new Table(UnitValue.CreatePercentArray(columnWidths))
        //                    .SetWidth(UnitValue.CreatePercentValue(100));

        //                // Header row
        //                int colOffset = 0;
        //                if (isFirstTable)
        //                {
        //                    var initiatorHeader = new Cell()
        //                        .SetBackgroundColor(new DeviceGray(0.75f))
        //                        .SetFont(f)
        //                        .SetFontSize(7)
        //                        .Add(new Paragraph("Initiator"))
        //                        .SetTextAlignment(TextAlignment.CENTER)
        //                        .SetVerticalAlignment(VerticalAlignment.MIDDLE);
        //                    table.AddHeaderCell(initiatorHeader);
        //                    colOffset = 1;
        //                }

        //                int startIdx = currentIdx;
        //                int endIdx = currentIdx + approversInThisChunk;
        //                for (int i = startIdx; i < endIdx; i++)
        //                {
        //                    var headerCell = new Cell()
        //                        .SetBackgroundColor(new DeviceGray(0.75f))
        //                        .SetFont(f)
        //                        .SetFontSize(7)
        //                        .Add(new Paragraph(objlist[i].Designation))
        //                        .SetTextAlignment(TextAlignment.CENTER)
        //                        .SetVerticalAlignment(VerticalAlignment.MIDDLE);
        //                    table.AddHeaderCell(headerCell);
        //                }

        //                // Data row
        //                if (isFirstTable)
        //                {
        //                    var initiatorData = new Cell()
        //                        .SetBackgroundColor(new DeviceGray(0.9f))
        //                        .SetTextAlignment(TextAlignment.CENTER)
        //                        .SetFont(f)
        //                        .SetFontSize(6)
        //                        .Add(new Paragraph(initiatorName))
        //                        .SetVerticalAlignment(VerticalAlignment.MIDDLE);
        //                    table.AddCell(initiatorData);
        //                }

        //                for (int i = startIdx; i < endIdx; i++)
        //                {
        //                    string content = "";
        //                    foreach (var sig in objlist[i].appList)
        //                    {
        //                        string appdate = sig.ApprovalDate != null ? sig.ApprovalDate.Value.ToString("dd-MMM-yyyy") : "Pending";
        //                        content += sig.EmpName + "\n" + "Test" + "\n";
        //                    }
        //                    var dataCell = new Cell()
        //                        .SetTextAlignment(TextAlignment.CENTER)
        //                        .SetFont(f)
        //                        .SetFontSize(6)
        //                        .Add(new Paragraph(content))
        //                        .SetVerticalAlignment(VerticalAlignment.MIDDLE);
        //                    table.AddCell(dataCell);
        //                }

        //                // Add padding below except for the last table
        //                bool isLastTable = (endIdx >= objlist.Count);
        //                if (!isLastTable)
        //                {
        //                    table.SetMarginBottom(tablePadding);
        //                }

        //                contentDiv.Add(table);

        //                currentIdx = endIdx; // Move to next
        //            }

        //            // Add the fixed-position div to the document
        //            pdfDoc.Add(contentDiv);
        //        }

        //        pdfDoc.Close();
        //        pdfResult.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log or handle error (customize as needed)
        //        Console.WriteLine("Error in IOMAnnotationPdfPreview: " + ex.Message);
        //        // Optionally: throw new Exception("PDF annotation failed: " + ex.Message);
        //    }
        //}

        #endregion


        [HttpPost]
        public ActionResult UploadAttachment(IOMPDetailViewModel formData)
        {
            short retVal = 0;
            List<IOMPDetailViewModel> poDtlList = new List<IOMPDetailViewModel>();
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (formData.IOMHEADERID > 0)
                {
                    if (formData.FILE.Length > 0 && !string.IsNullOrEmpty(formData.DOC_TYPE))
                    {
                        //string path = Server.MapPath("~/Uploads/DGIT_IOM/");
                        string path = Path.Combine(serverpath.getFileUploadPath(), "DGIT_IOM");
                        if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                        string pathtosave = DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString();
                        FileViewModel _file = GetUploadFile(formData.FILE, formData.DOC_TYPE, formData.IOMHEADERID.ToString());
                        poDtlList.Add(new IOMPDetailViewModel
                        {
                            IOMHEADERID = formData.IOMHEADERID,
                            FILENAME = pathtosave + @"\" + _file.FileName,
                            FILE_CONTENTTYPE = _file.FileContentType,
                            FILE_BYTE = _file.File,
                            DOC_TYPE = formData.DOC_TYPE,
                            ADDITIONAL_INFO = formData.ADDITIONAL_INFO,
                            ADDEDBY = Convert.ToInt64(_userId.ToString()),
                        });
                        Tuple<short, List<IOMPDetailViewModel>> _ret_tuple = _pIomService.SavePIOMAttachment(Convert.ToInt64(_userId.ToString()), formData.IOMHEADERID, poDtlList);
                        retVal = _ret_tuple.Item1;
                        if (retVal == 1)
                        {
                            //string path = Server.MapPath("~/Uploads/DGIT_IOM/");
                            //if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                            if (poDtlList.Count > 0)
                            {
                                foreach (IOMPDetailViewModel obj in poDtlList)
                                {
                                    if (obj.IsDeleted == 0 && obj.FILE_BYTE != null)
                                    {
                                        System.IO.File.WriteAllBytes(Path.Combine(path, obj.FILENAME), obj.FILE_BYTE.ToArray());
                                    }
                                }
                            }
                        }
                        poDtlList = _ret_tuple.Item2;
                    }
                }

            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(new { res = retVal, attachmentList = poDtlList.Where(w => w.DOC_TYPE != "IOM").ToList() });
        }

        [HttpPost]
        public ActionResult DeleteAttachment(string fileName, string docType, long iomHeaderId)
        {
            short retVal = 0;
            List<IOMPDetailViewModel> poDtlList = new List<IOMPDetailViewModel>();
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (iomHeaderId > 0)
                {
                    Tuple<short, List<IOMPDetailViewModel>> _ret_tuple = _pIomService.DeletePIOMAttachment(fileName, docType, iomHeaderId);
                    retVal = _ret_tuple.Item1;
                    poDtlList = _ret_tuple.Item2;
                    if (retVal == 1)
                    {
                        //string path = Server.MapPath("~/Uploads/DGIT_IOM/");
                        string path = Path.Combine(serverpath.getFileUploadPath(), "DGIT_IOM");
                        if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                        if (System.IO.File.Exists(System.IO.Path.Combine(path, fileName)))
                        {
                            System.IO.File.Delete(System.IO.Path.Combine(path, fileName));
                        }
                    }
                }
                //else
                //{
                //    if (TempData["IOM_ATTACHMENT_LIST"] != null)
                //    {
                //        poDtlList = (List<IOMDetailViewModel>)TempData["IOM_ATTACHMENT_LIST"];
                //    }
                //    List<IOMDetailViewModel> newPoDTLList = poDtlList.Where(a => a.FILENAME != fileName).ToList();
                //    poDtlList = new List<IOMDetailViewModel>(newPoDTLList);
                //    retVal = 1;
                //    TempData["IOM_ATTACHMENT_LIST"] = poDtlList;
                //}
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(new { res = retVal, attachmentList = poDtlList.Where(w => w.DOC_TYPE != "IOM").ToList() });
        }

        public IActionResult PIOMViewDetail(string id)
        {
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long _ReqId;
            try
            {
                //_ReqId = Convert.ToInt64(Server.UrlDecode(Encryption.Decrypt(id)));
                  _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));



               // _ReqId = Convert.ToInt64(Encryption.Decrypt(Server.UrlDecode(id))); // Server.UrlDecode(Encryption.Decrypt(id))
            }
            catch (Exception ex)
            {
                _ReqId = Convert.ToInt64(Encryption.Decrypt(id)); // Server.UrlDecode(Encryption.Decrypt(id))
            }

            var request = _pIomService.GetPIOMRequestById(_ReqId);
            if (request == null)
            {
                return NotFound("Request not found.");
            }

            return View("PIOMViewDetail", request);
        }

        public ActionResult GetPDF(string fileName)
        {
            try
            {
                string file_path = "../../../Uploads/DGIT_IOM/";
                string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"488px\"></object>";
                string _path = string.Format(embed, file_path + fileName);
                return Json(new
                {
                    FILEPATH = _path,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    FILEPATH = "",
                });
            }
        }

        public ActionResult PIOMTwoDocsView(long id)
        {
            IOMPHeaderViewModel obj = _pIomService.GetPIOMRequestById(id);
            IOMPDetailViewModel item = new IOMPDetailViewModel();
            item.FILENAME = "Select";
            obj.iompDetail.Insert(0, item);


            return View("PIOMTwoDocsView", obj.iompDetail);
        }

        public ActionResult GetTwoPDF(string fileName)
        {
            try
            {
                string file_path = "../../../Uploads/DGIT_IOM/";
                string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"488px\"></object>";
                string _path = string.Format(embed, file_path + fileName);
                return Json(new
                {
                    FILEPATH = _path,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    FILEPATH = "",
                });
            }
        }

        public ActionResult PIOMMultiDocsView(long id)
        {
            IOMPHeaderViewModel obj = _pIomService.GetPIOMRequestById(id);
            IOMPDetailViewModel item = new IOMPDetailViewModel();
            item.FILENAME = "Select";
            obj.iompDetail.Insert(0, item);

            return View("PIOMMultiDocsView", obj.iompDetail);
        }

        public ActionResult GetMultiPDF(string fileName)
        {
            try
            {
                string file_path = "../../../Uploads/DGIT_IOM/";
                string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"260px\"></object>";
                string _path = string.Format(embed, file_path + fileName);
                return Json(new
                {
                    FILEPATH = _path,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    FILEPATH = "",
                });
            }
        }

        [HttpGet]
        public ActionResult PIOMApproval(string id)
        {
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (!long.TryParse(id, out var reqId) || !long.TryParse(_userId?.ToString(), out var userId))
            {
                return Json(new { success = false, message = "Invalid request or data not found." });

            }

            var obj = _pIomService.GetPIOMRequestById(reqId);

            if (obj == null)
            {
                return Json(new { success = false, message = "PIOM request not found." });
            }

            var currentUserId = Convert.ToInt64(_userId);

            var userActiveApproval = obj.iompAppHis
                .FirstOrDefault(h =>
                    h.EmpCode == currentUserId &&
                    (h.ApprovalStatus == 0 || h.ApprovalStatus == 5) && h.IOMAPPHISTORY_ID != 0);

            var isEnable = (userActiveApproval != null) ? 1 : 0;

            obj.ISENABLE = isEnable.ToString();

            return View("PIOMApproval", obj);
        }

        [HttpPost]
        public async Task<ActionResult> PIOMApproval([FromBody] IOMPAppHistoryViewModel PHVM)
        {
            short retVal = 0;
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                Employee_Details _Employee_Details = _EmpDetails;
                PHVM.UpdatedBy = Convert.ToInt64(_userId.ToString());
                PHVM.EmpCode = Convert.ToInt64(_userId.ToString());
                PHVM.AddedBy = Convert.ToInt64(_userId.ToString());

                long _ReqId = PHVM.IOMID;
                IOMPHeaderViewModel iomObj = _pIomService.GetPIOMRequestById(_ReqId);
                long userid = Convert.ToInt64(_userId.ToString());

                bool isEligibleForApproval = false;

                if (iomObj != null && iomObj.Stages != null)
                {
                    var userActiveApproval = iomObj.iompAppHis
                    .FirstOrDefault(h =>
                    h.EmpCode == userid &&
                    (h.ApprovalStatus == 0 || h.ApprovalStatus == 5) && h.IOMAPPHISTORY_ID != 0);


                    if (userActiveApproval != null)
                    {
                        isEligibleForApproval = true;
                    }
                }

                if (!isEligibleForApproval)
                {
                    retVal = 2;
                    return Json(retVal);
                }

                retVal = _pIomService.PIOMApproval(PHVM, _Employee_Details, iomObj);
                if (retVal == 1)
                {
                    var IOM_Dtl = _pIomService.GetPIOMRequestById(PHVM.IOMID);
                    if (IOM_Dtl != null && IOM_Dtl.PROCESS_STATUS == 2)
                    {
                        // Check if any approval is pending (ApprovalStatus == 0)
                        var pendingApproval = IOM_Dtl.iompAppHis.FirstOrDefault(M => M.ApprovalStatus == 0);
                        if (pendingApproval == null)
                        {
                            // No pending approvals, generate the approval document
                            var objiomfile = IOM_Dtl.iompDetail.Where(m => m.DOC_TYPE == "IOM");
                            if (objiomfile == null || !objiomfile.Any())
                                throw new Exception("IOM File Not Found");

                            string path = Path.Combine(serverpath.getFileUploadPath(), "DGIT_IOM");
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);

                            var filenameObj = objiomfile.FirstOrDefault();
                            string srcFile = Path.Combine(path, filenameObj.FILENAME);
                            string pathtosave = Path.Combine(DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString());
                            string STRFILENAME = Path.Combine(pathtosave, "IOM_" + PHVM.IOMID + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".pdf");
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

                            // ✅ Get data from DB
                            var data = _pIomService.GetPIOMRequestById(IOM_Dtl.IOMHEADERID);
                            var getdata = data.iompDetail
                                .Where(x => x.DOC_TYPE == "IOM")
                                .Select(s => s.FILENAME)
                                .FirstOrDefault();

                            // ✅ Use model attachment if available, else fallback to DB value
                            string fileName = IOM_Dtl.IOMATTACHMENT;

                            string _date = IOM_Dtl.DATEADDED != null ? IOM_Dtl.DATEADDED.ToString("dd-MMM-yyyy") : "";

                            // Call your PDF annotation method (commented out for placeholder)
                            //Thread bgThread = new Thread(async () =>
                            //{
                            await PIOMAnnotationPdf(PHVM, _Employee_Details, srcFile, strDstFile, DocSiglist, _date, iomObj.ADDEDBYNAME, false);
                            //});

                            // Add approval document info
                            string addInfo = "Approved IOM";
                            var extensionFileCount = IOM_Dtl.iompDetail.Count(m => m.DOC_TYPE == "IOMA" && m.IOMHEADERID == PHVM.IOMID);
                            if (extensionFileCount > 0)
                            {
                                addInfo = "Extended Approval Document";
                            }

                            var attachmentObj = new List<IOMPDetailViewModel>
                            {
                                new IOMPDetailViewModel
                                {
                                    DOC_TYPE = "IOMA",
                                    FILENAME = STRFILENAME,
                                    ADDITIONAL_INFO = addInfo
                                }
                            };

                            // Save the attachment
                            _pIomService.SavePIOMAttachment(PHVM.AddedBy, PHVM.IOMID, attachmentObj);
                        }
                    }
                }

                //retVal = 0;
                return Json(retVal);
            }
            catch (Exception ex)
            {
                retVal = -1; // Error code
                return Json(retVal);
            }
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

        [HttpGet]
        public ActionResult EditPIOMRequestUploadBeforeHold(string id)
        {
            
            if (string.IsNullOrEmpty(_userId))
                return RedirectToAction("Index", "Login");

            long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));  // Server.UrlDecode(Encryption.Decrypt(id))

            var headerViewModel = _pIomService.GetPIOMRequestById(_ReqId);
            if (headerViewModel == null)
                return NotFound();
            if (headerViewModel.ADDEDBY.ToString() != _userId)
                return RedirectToAction("Home", "Home");

            const short HoldStatus = 5;

            var holdDateTime = headerViewModel.iompAppHis
                .Where(h => h.ApprovalStatus == HoldStatus && h.ApprovalDate.HasValue)
                .OrderByDescending(h => h.IOMAPPHISTORY_ID)
                .Select(h => h.ApprovalDate.Value)
                .FirstOrDefault();

            // If no Hold found, skip filtering
            if (holdDateTime != default)
            {
                headerViewModel.iompDetail = headerViewModel.iompDetail?
                    .Where(d => d.ADDEDDATE > holdDateTime)
                    .ToList();
            }

            return View(headerViewModel);
        }

        [HttpPost]
        public ActionResult EditIOMRequestUploadBeforeHold([FromBody] IOMPHeaderViewModel model)
        {
            if (_userId == null)
                return RedirectToAction("Index", "Login");

            try
            {
                long reqId = model.IOMHEADERID;
                var existingRequest = _pIomService.GetPIOMRequestById(reqId);

                //if (existingRequest == null || existingRequest.PROCESS_STATUS != 5)
                //    return Json((short)-2);

                model.iompDetail ??= new List<IOMPDetailViewModel>();
                long userId = Convert.ToInt64(_userId);

                model.ADDEDBY = userId;
                model.UPDATEDBY = userId;

                short result = _pIomService.SendMailByApprovalAuthorityForHoldRequest(model, _EmpDetails);

                return Json(result);
            }
            catch (Exception)
            {
                return Json((short)-1);
            }
        }

        [HttpGet]
        public ActionResult EditPIOMRequest(string id)
        {
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // long _ReqId = Convert.ToInt64(id);
            long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id))); // Server.UrlDecode(Encryption.Decrypt(id))
            IOMPHeaderViewModel PHVM = _pIomService.GetPIOMRequestById(_ReqId);

            if (PHVM.ISEDITABLE != 1)
            {
                return RedirectToAction("Home", "Home");
            }

            // Assign sequential order to stages, groups, and approvers
            if (PHVM.Stages != null && PHVM.Stages.Any())
            {
                int stageOrder = 0;
                foreach (var stage in PHVM.Stages.OrderBy(s => s.StageOrder))
                {
                    stage.StageOrder = ++stageOrder;

                    int groupOrder = 0;
                    foreach (var group in stage.ParallelGroups.OrderBy(pg => pg.GroupOrder))
                    {
                        group.GroupOrder = ++groupOrder;

                        short approverSeq = 1;
                        foreach (var approver in group.Approvers.OrderBy(a => a.APP_SEQ))
                        {
                            approver.APP_SEQ = approverSeq++;
                        }
                    }
                }
            }
            else
            {
                // Fallback: empty stages or approvers initialization
                PHVM.Stages = new List<IOMStageViewModel>();
            }
            ViewBag.strId = id;
            return View(PHVM);
        }

        [HttpPost]
        public ActionResult EditPIOMRequest([FromBody] IOMPHeaderViewModel model)
        {
            short retVal = 0;
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                model.iompDetail = new List<IOMPDetailViewModel>();
                model.ADDEDBY = Convert.ToInt64(_userId.ToString());
                model.UPDATEDBY = Convert.ToInt64(_userId.ToString());

                Tuple<short, long> retVal_tuple = _pIomService.SavePIOMRequest(model);
                retVal = retVal_tuple.Item1;
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult PIOMCancel(string id)
        {
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id))); //(Server.UrlDecode(Encryption.Decrypt(id)));
            //long _ReqId = Convert.ToInt64(id);
            return View("PIOMCancel", _pIomService.GetPIOMRequestById(_ReqId));
        }

        [HttpPost]
        public ActionResult PIOMCancel([FromBody] IOMPHeaderViewModel PHVM)
        {
            short retVal = 0;
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details _Employee_Details = _EmpDetails;
                PHVM.UPDATEDBY = Convert.ToInt64(_userId.ToString());
                PHVM.ADDEDBY = Convert.ToInt64(_userId.ToString());
                retVal = _pIomService.PIOMCancel(PHVM);
            }
            catch (Exception ex)
            {
                retVal = -1;
            }
            return Json(retVal);
        }

        [HttpGet]
        public ActionResult GetPIOMPrevAuthority()
        {
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                Employee_Details _Login_Employee_Details = _EmpDetails;
                List<IOMPAppAuthSeqViewModel> AuthSeqList = new List<IOMPAppAuthSeqViewModel>();
                List<IOMPHeaderViewModel> prevAuthority = _pIomService.GetPIOMPrevAuthority(Convert.ToInt64(_userId.ToString()));
                return Json(new
                {
                    HeaderList = prevAuthority,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    ECODE = 0,
                    ENAME = "",
                    HeaderList = new List<IOMPHeaderViewModel>()
                });
            }
        }
        [HttpPost]

        public JsonResult ReGenerateDoc(int id)
        {
            short retVal = 0;
            string Error = "";

            try
            {

                Employee_Details _Employee_Details = _EmpDetails;
                IOMPHeaderViewModel IOM_Dtl = _pIomService.GetPIOMRequestById(Convert.ToInt64(id));

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

                            _pIomService.SavePIOMAttachment(Convert.ToInt64(_userId.ToString()), IOM_Dtl.IOMHEADERID, obj);
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


        public ActionResult PIOMAdditinalApproval(string id)
        {
            if (_userId == null)
            {
                return RedirectToAction("Index", "Login");
            }
            long _ReqId;
            _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));
            //_ReqId = Convert.ToInt64(id);
            IOMPHeaderViewModel data = _pIomService.GetPIOMRequestById(_ReqId);

            //if (data.PROCESS_STATUS != 2)
            //{
            //    return RedirectToAction("Home", "Home");
            //}

            if (data.ADDEDBY == Convert.ToInt64(_userId))
            {
                return View("PIOMAdditinalApproval", data);
            }
            else
            {
                IOMPHeaderViewModel ohdr = new IOMPHeaderViewModel();
                return View("PIOMAdditinalApproval", ohdr);
            }
        }

        [HttpPost]
        public ActionResult PIOMAdditinalApproval([FromBody] IOMPHeaderViewModel model)
        {
            short retVal = 0;
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                model.iompDetail = new List<IOMPDetailViewModel>();
                model.ADDEDBY = Convert.ToInt64(_userId.ToString());
                model.UPDATEDBY = Convert.ToInt64(_userId.ToString());
                model.IsFinalSubmit = 1;
                model.PROCESS_STATUS = 1;
                Tuple<short, long> retVal_tuple = _pIomService.SaveAdditionalPIOMRequest(model);
                retVal = retVal_tuple.Item1;

            }
            catch
            {
                retVal = -1;
            }
            return Json(retVal);
        }
        [HttpGet]
        public ActionResult PIOMNextApproval(string IOMID_PARAM)
        {
            string retVal = "";
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                long userid = Convert.ToInt64(_userId.ToString());
                long IOMidcurr = 0, IOMIDNext;
                if (!string.IsNullOrEmpty(IOMID_PARAM))
                {
                    IOMidcurr = Convert.ToInt64(IOMID_PARAM);
                    IOMIDNext = _pIomService.GetPIOMNextApprovalId(IOMidcurr, userid);
                    retVal = IOMIDNext == 0 ? "" : IOMIDNext.ToString();
                }
            }
            catch (Exception ex)
            {
                retVal = "";
            }
            return Json(retVal);
        }

    }
}
