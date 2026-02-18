using ePortal.ViewModels;
using System.Data;
using ePortal.Application.Contracts;
using ePortal.Shared.Interface;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using ePortal.Persistence.Interface;
using System.Reflection;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter]
    [SessionTimeout]
    public class VPFController : Controller
    {
        private readonly IVpfCreate _objVpfCreateServ;
        private readonly IEportalESS objess;

        private readonly ISessionService _sessionService;
        private readonly ILogger<IOMController> _logger;
        private readonly string _userId;
        private readonly string _userName;
        private readonly Employee_Details _EmpDetails;
        public VPFController(IVpfCreate objVpfCreate, ISessionService sessionService, ILogger<IOMController> logger, IEportalESS _objess)
        {
            _objVpfCreateServ = objVpfCreate;
            objess = _objess;

            _sessionService = sessionService;
            _logger = logger;
            _userId = _sessionService.Get<string>("userID").ToString();
            _userName = _sessionService.Get<string>("userName").ToString();
            _EmpDetails = _sessionService.Get<Employee_Details>("Employee");
        }

        // GET: VPF
        [HttpGet]
        public async Task<ActionResult> VPF_CreateDetails()
        {
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                //EportalESS objess = new EportalESS();//SAP Connection Object
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                string fromdate = (DateTime.Now.AddYears(-1).ToString("yyyy")) + DateTime.Now.ToString("MM") + "01";
                string todate = DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MM") + "01";
                dt1 = await objess.GetPayrollResultList(_userId.ToString(), fromdate.ToString(), todate.ToString());
                DataView dv = dt1.DefaultView;
                dv.Sort = "sequencenumber desc";
                DataTable sortedDT = dv.ToTable();
                string strseq = sortedDT.Rows[0]["sequencenumber"].ToString();
                dt = await objess.GetPaySlip(_userId.ToString(), strseq, "");

                //DataRow dr;

                string headerstr;
                //string headerstr1;
                headerstr = dt.Rows[1][1].ToString();
                string data = ((getBetween(headerstr, "|Basic", "|PaySlip").Trim()).Split('.')[0]).Replace(",", "");
                char[] chr1 = new char[2];
                chr1[0] = '|';
                chr1[1] = ' ';
                string[] row1 = headerstr.Split(chr1);
                //string basic = row1[44].Replace(",", "") ;
                char[] chr = new char[2];
                chr[0] = '|';
                chr[1] = ':';
                string[] row6 = dt.Rows[8][1].ToString().Split(chr);
                string cvpf = row6[6];
                VPF_CreateDetailViewModel vpfDetail = new VPF_CreateDetailViewModel();
                vpfDetail.BASICSALARY = data.ToString();
                vpfDetail.CURRENTVPF = cvpf.ToString();
                if (DateTime.Today.Date.Day > 15)
                    vpfDetail.EFFECTIVEDATE = DateTime.ParseExact("01/" + DateTime.Today.AddMonths(1).ToString("MMM") + "/" + DateTime.Today.AddMonths(1).ToString("yyyy"), "dd/MMM/yyyy", null).ToString("dd-MMM-yyyy");
                else
                    vpfDetail.EFFECTIVEDATE = DateTime.ParseExact("01/" + DateTime.Today.ToString("MMM") + "/" + DateTime.Today.ToString("yyyy"), "dd/MMM/yyyy", null).ToString("dd-MMM-yyyy");

                return View(vpfDetail);
                //return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.ToString()); 
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }

        [HttpPost]
        public ActionResult VPF_CreateDetails(VPF_CreateDetailViewModel CDVM)
        {
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                ModelState.Keys.Except(Request.Form.Keys).ToList().ForEach(key => ModelState.Remove(key));
                
                ModelState.Remove("CANCELREMARKS");
                ModelState.Remove("APPROVESTATUS");
                ModelState.Remove("APPROVALREMARKS");
                ModelState.Remove("APPROVALDATE");
                if (CDVM.REQUESTTYPE == 3)
                {
                    ModelState.Remove("VPFCONTRIBUTION");
                }
                if (!ModelState.IsValid)
                {
                    return View(CDVM);
                }



                CDVM.DATEADDED = DateTime.Now;
                CDVM.EMPLOYEECODE = Convert.ToInt32(_userId);
                //Convert.ToInt64(Session["UserId"]);
                //CDVM.VPFCONTRIBUTION=

                _objVpfCreateServ.SaveVPF_Create_Detail(CDVM);

                if (CDVM.ErrorMsg != null)
                {
                    ViewBag.Message = string.Format(CDVM.ErrorMsg);

                    return View(CDVM);
                }
                else
                {

                }

                return RedirectToAction("GetVpfDetail");


            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.ToString());
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }


        public ActionResult GetVpfDetail()
        {
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                var VpfDetails = _objVpfCreateServ.GetVpfDetail(Convert.ToInt32(_userId));
                return View(VpfDetails);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.ToString());
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        public async Task<ActionResult> Edit(long id)
        {
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                //EportalESS objess = new EportalESS();//SAP Connection Object
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                string fromdate = (DateTime.Now.AddYears(-1).ToString("yyyy")) + DateTime.Now.ToString("MM") + "01";
                string todate = DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MM") + "01";
                dt1 = await objess.GetPayrollResultList(_userId.ToString(), fromdate.ToString(), todate.ToString());
                DataView dv = dt1.DefaultView;
                dv.Sort = "sequencenumber desc";
                DataTable sortedDT = dv.ToTable();
                string strseq = sortedDT.Rows[0]["sequencenumber"].ToString();
                dt = await objess.GetPaySlip(_userId.ToString(), strseq, "");

                DataRow dr;

                string headerstr;
                headerstr = dt.Rows[1][1].ToString();
                string data = ((getBetween(headerstr, "|Basic", "|PaySlip").Trim()).Split('.')[0]).Replace(",", "");
                char[] chr1 = new char[2];
                chr1[0] = '|';
                chr1[1] = ' ';
                string[] row1 = headerstr.Split(chr1);
                // string basic = row1[44].Replace(",", "");
                char[] chr = new char[2];
                chr[0] = '|';
                chr[1] = ':';
                string[] row6 = dt.Rows[7][1].ToString().Split(chr);
                string cvpf = row6[6];
                //VPF_CreateDetailViewModel vpfDetail = new VPF_CreateDetailViewModel();
                var vpfDetail = _objVpfCreateServ.EditVPF_Detail(id);
                //string[] row3 = basic.Split('.');
                //string basicsalary = row3[0];
                vpfDetail.BASICSALARY = data.ToString();
                vpfDetail.CURRENTVPF = cvpf.ToString();



                return View(vpfDetail);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.ToString());
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        public async Task<ActionResult> Cancel(long id)
        {
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                //EportalESS objess = new EportalESS();//SAP Connection Object
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                string fromdate = (DateTime.Now.AddYears(-1).ToString("yyyy")) + DateTime.Now.ToString("MM") + "01";
                string todate = DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MM") + "01";
                dt1 = await objess.GetPayrollResultList(_userId.ToString(), fromdate.ToString(), todate.ToString());
                DataView dv = dt1.DefaultView;
                dv.Sort = "sequencenumber desc";
                DataTable sortedDT = dv.ToTable();
                string strseq = sortedDT.Rows[0]["sequencenumber"].ToString();
                dt = await objess.GetPaySlip(_userId.ToString(), strseq, "");

                DataRow dr;

                string headerstr;
                headerstr = dt.Rows[1][1].ToString();
                string data = ((getBetween(headerstr, "|Basic", "|PaySlip").Trim()).Split('.')[0]).Replace(",", "");
                char[] chr1 = new char[2];
                chr1[0] = '|';
                chr1[1] = ' ';
                string[] row1 = headerstr.Split(chr1);
                //string basic = row1[44].Replace(",", "");
                char[] chr = new char[2];
                chr[0] = '|';
                chr[1] = ':';
                string[] row6 = dt.Rows[7][1].ToString().Split(chr);
                string cvpf = row6[6];
                //VPF_CreateDetailViewModel vpfDetail = new VPF_CreateDetailViewModel();
                var vpfDetail = _objVpfCreateServ.EditVPF_Detail(id);
                //string[] row3 = basic.Split('.');
                //string basicsalary = row3[0];
                vpfDetail.BASICSALARY = data.ToString();
                vpfDetail.CURRENTVPF = cvpf.ToString();

                //vpfDetail = _objVpfCreateServ.EditVPF_Detail(id);
                return View(vpfDetail);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.ToString());
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost]
        public ActionResult Cancel(VPF_CreateDetailViewModel CDVM)
        {
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                ModelState.Keys.Except(Request.Form.Keys).ToList().ForEach(key => ModelState.Remove(key));
                ModelState.Remove("APPROVESTATUS");
                ModelState.Remove("APPROVALREMARKS");
                ModelState.Remove("APPROVALDATE");
                ModelState.Remove("EFFECTIVEDATE");
                ModelState.Remove("VPFCONTRIBUTION");
                if (!ModelState.IsValid)
                {
                    return View(CDVM);
                }
                CDVM.MODIFIEDDATE = DateTime.Now;
                CDVM.MODIFIEDBY = Convert.ToInt32(_userId);
                _objVpfCreateServ.Cancel_VPF_Detail(CDVM);
                //var vpfDetail = _objVpfCreateServ.EditVPF_Detail(id);
                return RedirectToAction("GetVpfDetail");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.ToString());
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost]
        public ActionResult Edit(VPF_CreateDetailViewModel CDVM)
        {
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                ModelState.Keys.Except(Request.Form.Keys).ToList().ForEach(key => ModelState.Remove(key));
                ModelState.Remove("CANCELREMARKS");
                ModelState.Remove("APPROVESTATUS");
                ModelState.Remove("APPROVALREMARKS");
                ModelState.Remove("APPROVALDATE");
                if (CDVM.REQUESTTYPE == 3)
                {
                    ModelState.Remove("VPFCONTRIBUTION");
                }
                //ModelState.Remove("");
                //ModelState.Remove("");
                if (!ModelState.IsValid)
                {
                    return View(CDVM);
                }
                CDVM.MODIFIEDDATE = DateTime.Now;
                CDVM.MODIFIEDBY = Convert.ToInt32(_userId);
                _objVpfCreateServ.Update_VPF_Detail(CDVM);
                //var vpfDetail = _objVpfCreateServ.EditVPF_Detail(id);
                return RedirectToAction("GetVpfDetail");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.ToString());
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return RedirectToAction("ErrorPage");
            }

        }

        [HttpGet]
        public ActionResult IRVpfDetail()
        {
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                SearchViewModel SearchViewModel = new SearchViewModel();
                SearchViewModel.Status = 2;
                VPF_CreateDetailViewModel VPFList = new VPF_CreateDetailViewModel();
                VPFList.VPF_List = _objVpfCreateServ.IRVpfDetail(SearchViewModel);

                return View(VPFList);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.ToString());
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost]
        public ActionResult IRVpfDetail(VPF_CreateDetailViewModel VDVM, string type)
        {
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (type == "Search")
                {
                    VPF_CreateDetailViewModel VPFList = new VPF_CreateDetailViewModel();
                    VPFList.VPF_List = _objVpfCreateServ.IRVpfDetail(VDVM.SearchViewModel);
                    return View(VPFList);
                }
                else
                {
                    string listExport = "";
                    string file_Name = "VPFReport.xls";
                    listExport = _objVpfCreateServ.IRVpfDetailExcel(VDVM.SearchViewModel);
                    Response.Headers.Add("content-disposition", "attachment; filename=" + file_Name);
                    this.Response.ContentType = "application/vnd.ms-excel";
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(listExport.ToString());
                    return File(buffer, "application/vnd.ms-excel");                   
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.ToString());
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        public async Task<ActionResult> Approve(long id)
        {
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                var vpfDetail = _objVpfCreateServ.EditVPF_Detail(id);

                //EportalESS objess = new EportalESS();//SAP Connection Object
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                string fromdate = (DateTime.Now.AddYears(-1).ToString("yyyy")) + DateTime.Now.ToString("MM") + "01";
                string todate = DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MM") + "01";
                dt1 = await objess.GetPayrollResultList(vpfDetail.EMPLOYEECODE.ToString(), fromdate.ToString(), todate.ToString());
                DataView dv = dt1.DefaultView;
                dv.Sort = "sequencenumber desc";
                DataTable sortedDT = dv.ToTable();
                string strseq = sortedDT.Rows[0]["sequencenumber"].ToString();
                dt = await objess.GetPaySlip(vpfDetail.EMPLOYEECODE.ToString(), strseq, "");

                DataRow dr;

                string headerstr;
                headerstr = dt.Rows[1][1].ToString();
                string data = ((getBetween(headerstr, "|Basic", "|PaySlip").Trim()).Split('.')[0]).Replace(",", "");
                char[] chr1 = new char[2];
                chr1[0] = '|';
                chr1[1] = ' ';
                string[] row1 = headerstr.Split(chr1);
                //string basic = row1[44].Replace(",", "");
                char[] chr = new char[2];
                chr[0] = '|';
                chr[1] = ':';
                string[] row6 = dt.Rows[7][1].ToString().Split(chr);
                string cvpf = row6[6];
                //VPF_CreateDetailViewModel vpfDetail = new VPF_CreateDetailViewModel();

                //string[] row3 = basic.Split('.');
                //string basicsalary = row3[0];
                if (string.IsNullOrEmpty(data))
                {
                    vpfDetail.BASICSALARY = vpfDetail.BASICSALARY;
                    vpfDetail.CURRENTVPF = vpfDetail.CURRENTVPF;
                }
                else
                {
                    vpfDetail.BASICSALARY = data.ToString();
                    vpfDetail.CURRENTVPF = cvpf.ToString();
                }


                //var vpfDetail = _objVpfCreateServ.EditVPF_Detail(id);
                return View(vpfDetail);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.ToString());
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost]
        public ActionResult Approve(VPF_CreateDetailViewModel CDVM)
        {
            try
            {
                if (_userId == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                ModelState.Keys.Except(Request.Form.Keys).ToList().ForEach(key => ModelState.Remove(key));
                ModelState.Remove("REQUESTTYPE");
                ModelState.Remove("CANCELREMARKS");
                ModelState.Remove("APPROVALDATE");
                ModelState.Remove("EFFECTIVEDATE");
                ModelState.Remove("VPFCONTRIBUTION");
                if (!ModelState.IsValid)
                {
                    return View(CDVM);
                }
                CDVM.APPROVALDATE = DateTime.Now;
                CDVM.APPROVALAUTHID = Convert.ToInt32(_userId);

                //CDVM.APPROVESTATUS=
                //_objVpfCreateServ.Update_VPF_Detail(CDVM);
                var vpfDetail = _objVpfCreateServ.ApproveVPF_Detail(CDVM);
                return RedirectToAction("IRVpfDetail");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.ToString());
                _logger.LogError(ex, "Error in Method:  {MethodName}, Logged in User Id: {UserId}, Message: {ErrorMessage}", MethodBase.GetCurrentMethod()?.Name, _userId, ex.Message);
                return RedirectToAction("ErrorPage");
            }
        }


    }
}