

using ePortal.Application.Contracts;
using ePortal.DomainClasses;
using ePortal.Shared;
using ePortal.Shared.Interface;
using ePortal.ViewModels;
using ePortal.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ePortal.WebUI.Controllers
{
    [CSPFilter("",15)]
    public class WFHController : Controller
    {
        IWFHService _WFHService;
        IEmpLoginService _loginService;
        private readonly ISessionService _sessionService;
        public WFHController(IWFHService WFHService, IEmpLoginService objLogin, ISessionService sessionService)
        {
            _WFHService = WFHService;
            _loginService = objLogin;
            _sessionService = sessionService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult WFHRequest()
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            List<ShiftViewModel> _shiftList = _WFHService.GetShiftBySite((long)employeeDetails._SiteId, null);
            ViewBag.Shift = new SelectList(_shiftList, "SYSHIFTID", "CODE");

            WFHAppAuthViewModel _appAuth = _WFHService.GetApprovalAuth(employeeDetails._ECode);
            if (_appAuth != null)
            {
                TempData["APPROVAL_AUTHORITY"] = (WFHAppAuthViewModel)_appAuth;
            }
            else
            {
                TempData["APPROVAL_AUTHORITY"] = new WFHAppAuthViewModel();
            }

            //// ---- Get Active Date Parameter Value from SYPARAMETER Table ----////
            int checkNewValidation;
            DateTime _currentDate = DateTime.ParseExact(DateTime.Now.ToString("dd-MMM-yyyy"), "dd-MMM-yyyy", null);
            string date_parm = _loginService.GetParameterValue("WFH_ACTIVEDATE_VALIDATION");
            if (string.IsNullOrEmpty(date_parm))
            {
                checkNewValidation = 1;
            }
            else
            {
                DateTime _activeDate = DateTime.ParseExact(date_parm, "dd-MMM-yyyy", null);
                checkNewValidation = _currentDate >= _activeDate ? 1 : 0;
            }
            ViewBag.IsActiveDate = checkNewValidation;

            return View();
        }

        [HttpPost]
        public ActionResult WFHRequest([FromBody]ASRWFH_HEADER_ViewModel AVM)
        {
            short retval = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                AVM.ADEMPCODE = Convert.ToInt64(_sessionService.Get<string>("userID"));
                AVM.ADDEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                AVM.DATEADDED = DateTime.Now;
                /*Changed by TTL ON 31-July-2025 against SR104870 > CR6961 - Start*/
                var result = OffDaysValidation(AVM.STARTDATE.Value.ToString("dd-MMM-yyyy"), AVM.ENDDATE.Value.ToString("dd-MMM-yyyy"), null);
                if (result.ISVALID == false) { 
                    return Json(-5);
                }
                /*Changed by TTL ON 31-July-2025 against SR104870 > CR6961 - End*/
                retval = _WFHService.SaveWFHRequest(AVM);
            }
            catch
            {
                retval = -1;
            }
            return Json(retval);
        }

        [HttpGet]
        public ActionResult EditWFHRequest(string id)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                List<ShiftViewModel> _shiftList = _WFHService.GetShiftBySite((long)employeeDetails._SiteId, null);
                ViewBag.Shift = new SelectList(_shiftList, "SYSHIFTID", "CODE");

                WFHAppAuthViewModel _appAuth = _WFHService.GetApprovalAuth(employeeDetails._ECode);
                if (_appAuth != null)
                {
                    TempData["APPROVAL_AUTHORITY"] = (WFHAppAuthViewModel)_appAuth;
                }
                else
                {
                    TempData["APPROVAL_AUTHORITY"] = new WFHAppAuthViewModel();
                }
                long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id))); //// Convert.ToInt64(Encryption.Decrypt(Server.UrlDecode(id)));
                ASRWFH_HEADER_ViewModel AHVM = _WFHService.GetWFHRequestDetail(_ReqId);
                return View(AHVM);
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditWFHRequest([FromBody]ASRWFH_HEADER_ViewModel AVM)
        {
            short retval = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                AVM.ADEMPCODE = Convert.ToInt64(_sessionService.Get<string>("userID"));
                AVM.MODIFIEDBY = Convert.ToInt64(_sessionService.Get<string>("userID"));
                AVM.DATELSTMOD = DateTime.Now;
                /*Changed by TTL ON 31-July-2025 against SR104870 > CR6961 - Start*/
                var result = OffDaysValidation(AVM.STARTDATE.Value.ToString("dd-MMM-yyyy"), AVM.ENDDATE.Value.ToString("dd-MMM-yyyy"), AVM.ASRWFHID);
                if (result.ISVALID == false) { 
                    return Json(-5);
                }
                /*Changed by TTL ON 31-July-2025 against SR104870 > CR6961 - End*/
                retval = _WFHService.EditWFHRequest(AVM);
            }
            catch
            {
                retval = -1;
            }
            return Json(retval);
        }

        [HttpGet]
        public ActionResult WFHApproval(string id)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                ASRWFH_APPROVALHIS_ViewModel AAVM = new ASRWFH_APPROVALHIS_ViewModel();
                long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id)));
                AAVM.ASRWFH_HEADER = _WFHService.GetWFHRequestDetail(_ReqId);

                WFHAppAuthViewModel _appAuth = _WFHService.GetApprovalAuth(AAVM.ASRWFH_HEADER.ADEMPCODE);
                if (_appAuth != null)
                {
                    TempData["APPROVAL_AUTHORITY"] = (WFHAppAuthViewModel)_appAuth;
                }
                else
                {
                    TempData["APPROVAL_AUTHORITY"] = new WFHAppAuthViewModel();
                }
                return View(AAVM);
            }
            catch
            {
                return View();
            }
        }

        [HttpPut]
        public ActionResult WFHApproval([FromBody]ASRWFH_APPROVALHIS_ViewModel AAVM)
        {
            short retval = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                AAVM.APPADEMPCODE = Convert.ToInt64(_sessionService.Get<string>("userID"));
                AAVM.RECADEMPCODE = Convert.ToInt64(_sessionService.Get<string>("userID"));
                retval = _WFHService.UpdateWFHApproval(AAVM, employeeDetails);
            }
            catch
            {
                retval = -1;
            }
            return Json(retval);
        }

        [HttpGet]
        public ActionResult WFHCancelRequest(string id)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                ASRWFH_APPROVALHIS_ViewModel AAVM = new ASRWFH_APPROVALHIS_ViewModel();
                long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id))); //// Convert.ToInt64(Encryption.Decrypt(Server.UrlDecode(id)));
                AAVM.ASRWFH_HEADER = _WFHService.GetWFHRequestDetail(_ReqId);

                WFHAppAuthViewModel _appAuth = _WFHService.GetApprovalAuth(AAVM.ASRWFH_HEADER.ADEMPCODE);
                if (_appAuth != null)
                {
                    TempData["APPROVAL_AUTHORITY"] = (WFHAppAuthViewModel)_appAuth;
                }
                else
                {
                    TempData["APPROVAL_AUTHORITY"] = new WFHAppAuthViewModel();
                }
                return View(AAVM);
            }
            catch
            {
                return View();
            }
        }

        [HttpPut]
        public ActionResult WFHCancelRequest([FromBody]ASRWFH_APPROVALHIS_ViewModel AAVM)
        {
            short retval = 0;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                AAVM.APPADEMPCODE = Convert.ToInt64(_sessionService.Get<string>("userID"));
                AAVM.RECADEMPCODE = Convert.ToInt64(_sessionService.Get<string>("userID"));
                retval = _WFHService.UpdateWFHApproval(AAVM, employeeDetails);
            }
            catch
            {
                retval = -1;
            }
            return Json(retval);
        }

        [HttpGet]
        public ActionResult GetOffDays(string startDate, string endDate, long? reqId)
        {
            short offDays = 0; short offDaysByCurrentDate = 0; short totalDays = 0; short workingDays = 0; bool isValid = false; bool isRejectValid = false, isActiveDate = false;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                DateTime _currentDate = DateTime.ParseExact(DateTime.Now.ToString("dd-MMM-yyyy"), "dd-MMM-yyyy", null);
                DateTime _sdate;
                DateTime _edate;

                //// ---- Get Active Date Parameter Value from SYPARAMETER Table ----////
                bool checkNewValidation;
                string date_parm = _loginService.GetParameterValue("WFH_ACTIVEDATE_VALIDATION");
                if (string.IsNullOrEmpty(date_parm))
                {
                    checkNewValidation = true;
                }
                else
                {
                    DateTime _activeDate = DateTime.ParseExact(date_parm, "dd-MMM-yyyy", null);
                    checkNewValidation = _currentDate >= _activeDate ? true : false;
                }

                //// ---- Check Active Date validation ---- ////
                if (checkNewValidation)
                {
                    isActiveDate = true;

                    //// ---- Get Parameter Value from SYPARAMETER Table ----////
                    string parm_value = _loginService.GetParameterValue("WFH_REJECTED_VALIDATION");
                    parm_value = string.IsNullOrEmpty(parm_value) ? "0" : parm_value;
                    int previous_value = Convert.ToInt32(parm_value);

                    //// ---- Get Rejected Request ---- ////
                    ASRWFH_HEADER_ViewModel _Obj = _WFHService.GetRejectedRequest(startDate, endDate, Convert.ToInt64(_sessionService.Get<string>("userID").ToString()));

                    //// ---- This code use for edit request ---- ////
                    ASRWFH_HEADER_ViewModel _headerDtl = new ASRWFH_HEADER_ViewModel();
                    if (reqId != null)
                    {
                        _headerDtl = _WFHService.GetWFHRequestDetail(Convert.ToInt64(reqId));
                    }
                    string editReqStartDate = _headerDtl == null ? "" : Convert.ToDateTime(_headerDtl.STARTDATE).ToString("dd-MMM-yyyy");
                    DateTime _editReqStartDate = DateTime.ParseExact(editReqStartDate, "dd-MMM-yyyy", null);
                    //// ---- End ---- //// 

                    //// ---- Check Current Date and Previous date Validation ----////
                    if (!string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))
                    {
                        _sdate = DateTime.ParseExact(startDate, "dd-MMM-yyyy", null);
                        if (_Obj == null)
                        {
                            //// ---- Get Off Days By StartDate & Current Date ----////
                            offDaysByCurrentDate = _WFHService.GetOffDays(startDate, DateTime.Now.ToString("dd-MMM-yyyy"), (long)employeeDetails._SiteId, (long)employeeDetails._OpId);
                            previous_value = offDaysByCurrentDate == 0 ? previous_value : previous_value + offDaysByCurrentDate;

                            if (_currentDate == _sdate || _sdate >= _currentDate.AddDays(-previous_value))
                            {
                                isValid = true;
                                isRejectValid = true;
                            }
                            else
                            {
                                isValid = false;
                                isRejectValid = true;
                            }
                        }
                        else
                        {
                            string _rejectDM = _Obj.ASRWFH_APPROVALHIS.DATEISTMOD == null ? "" : Convert.ToDateTime(_Obj.ASRWFH_APPROVALHIS.DATEISTMOD).ToString("dd-MMM-yyyy");
                            if (!string.IsNullOrEmpty(_rejectDM))
                            {
                                //// ---- Get Off Days By RejectedDate & Current Date ----////
                                offDaysByCurrentDate = _WFHService.GetOffDays(_rejectDM, DateTime.Now.ToString("dd-MMM-yyyy"), (long)employeeDetails._SiteId, (long)employeeDetails._OpId);
                                parm_value = offDaysByCurrentDate == 0 ? parm_value : Convert.ToString(Convert.ToInt32(parm_value) + offDaysByCurrentDate);

                                DateTime _rDate_ = DateTime.ParseExact(_rejectDM, "dd-MMM-yyyy", null);
                                isValid = _currentDate == _rDate_ || (_currentDate >= _rDate_ && _currentDate <= _rDate_.AddDays(Convert.ToInt32(parm_value))) ? true : false;
                                isRejectValid = _currentDate == _rDate_ || (_currentDate >= _rDate_ && _currentDate <= _rDate_.AddDays(Convert.ToInt32(parm_value))) ? true : false;
                            }
                        }
                        return Json(new
                        {
                            OFFDAYS = offDays,
                            TOTALDAYS = totalDays,
                            WORKINGDAYS = workingDays,
                            ISVALID = isValid,
                            ISREJECTVALID = isRejectValid,
                            ISACTIVEDATE = isActiveDate,
                        });
                    }
                    //// ---- End ----////

                    _sdate = DateTime.ParseExact(startDate, "dd-MMM-yyyy", null);
                    _edate = DateTime.ParseExact(endDate, "dd-MMM-yyyy", null);

                    //// ---- Get Off Days StartDate & EndDate ---- ////
                    offDays = _WFHService.GetOffDays(startDate, endDate, (long)employeeDetails._SiteId, (long)employeeDetails._OpId);

                    totalDays = (short)((_edate.AddDays(1)) - _sdate).TotalDays;
                    workingDays = (short)(totalDays - offDays);
                    if (_Obj != null)
                    {
                        string rejectDM = _Obj.ASRWFH_APPROVALHIS.DATEISTMOD == null ? "" : Convert.ToDateTime(_Obj.ASRWFH_APPROVALHIS.DATEISTMOD).ToString("dd-MMM-yyyy");
                        if (!string.IsNullOrEmpty(rejectDM))
                        {
                            //// ---- Get Off Days By RejectedDate & Current Date ----////
                            offDaysByCurrentDate = _WFHService.GetOffDays(rejectDM, DateTime.Now.ToString("dd-MMM-yyyy"), (long)employeeDetails._SiteId, (long)employeeDetails._OpId);
                            parm_value = offDaysByCurrentDate == 0 ? parm_value : Convert.ToString(Convert.ToInt32(parm_value) + offDaysByCurrentDate);

                            DateTime _rDate = DateTime.ParseExact(rejectDM, "dd-MMM-yyyy", null);
                            isValid = (_sdate == _edate && (_currentDate == _rDate || (_currentDate >= _rDate && _currentDate <= _rDate.AddDays(Convert.ToInt32(parm_value))) || _editReqStartDate == _sdate)) ? true : false;
                            isRejectValid = (_currentDate == _rDate || (_currentDate >= _rDate && _currentDate <= _rDate.AddDays(Convert.ToInt32(parm_value))) || _editReqStartDate == _sdate) ? true : false;
                        }
                    }
                    else
                    {
                        //// ---- Get Off Days By StartDate & Current Date ----////
                        offDaysByCurrentDate = _WFHService.GetOffDays(startDate, DateTime.Now.ToString("dd-MMM-yyyy"), (long)employeeDetails._SiteId, (long)employeeDetails._OpId);
                        previous_value = offDaysByCurrentDate == 0 ? previous_value : previous_value + offDaysByCurrentDate;

                        isValid = (_sdate == _edate && (_currentDate == _sdate || _sdate >= _currentDate.AddDays(-previous_value)) || _editReqStartDate == _sdate) ? true : false;
                        isRejectValid = true;
                    }
                }
                else
                {
                    _sdate = DateTime.ParseExact(startDate, "dd-MMM-yyyy", null);
                    _edate = DateTime.ParseExact(endDate, "dd-MMM-yyyy", null);

                    //// ---- Get Off Days StartDate & EndDate ---- ////
                    offDays = _WFHService.GetOffDays(startDate, endDate, (long)employeeDetails._SiteId, (long)employeeDetails._OpId);

                    totalDays = (short)((_edate.AddDays(1)) - _sdate).TotalDays;
                    workingDays = (short)(totalDays - offDays);
                    isValid = _edate >= _sdate ? true : false;
                    isRejectValid = true;
                    isActiveDate = false;
                }
            }
            catch (Exception ex)
            {
                offDays = -1;
                totalDays = -1;
                workingDays = -1;
            }
            return Json(new
            {
                OFFDAYS = offDays,
                TOTALDAYS = totalDays,
                WORKINGDAYS = workingDays,
                ISVALID = isValid,
                ISREJECTVALID = isRejectValid,
                ISACTIVEDATE = isActiveDate,
            });
        }

        [HttpGet]
        public ActionResult GetHours(string startDate, string endDate, string startTime, string endTime)
        {
            double totalHours = 0; bool isValid = false;
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                DateTime _validSdate = DateTime.ParseExact(startDate, "dd-MMM-yyyy", null);
                DateTime _validEdate = DateTime.ParseExact(endDate, "dd-MMM-yyyy", null);

                string[] _start_HH_mm = startTime.Split(':');
                string[] _end_HH_mm = endTime.Split(':');
                startDate = startDate + " " + Convert.ToInt32(_start_HH_mm[0]).ToString("D2") + ":" + Convert.ToInt32(_start_HH_mm[1]).ToString("D2");
                endDate = endDate + " " + Convert.ToInt32(_end_HH_mm[0]).ToString("D2") + ":" + Convert.ToInt32(_end_HH_mm[1]).ToString("D2");
                DateTime _sdate = DateTime.ParseExact(startDate, "dd-MMM-yyyy HH:mm", null);
                DateTime _edate = DateTime.ParseExact(endDate, "dd-MMM-yyyy HH:mm", null);
                totalHours = (_edate - _sdate).TotalHours;
                if (_edate >= _sdate)
                {
                    isValid = _validSdate == _validEdate ? true : (_validEdate > _validSdate && _validSdate.AddDays(1) == _validEdate) ? true : false;
                }
                else
                {
                    isValid = false;
                }
            }
            catch (Exception ex)
            {
                totalHours = -1;
            }
            return Json(new
            {
                TOTALHOURS = totalHours,
                ISVALID = isValid
            });
        }

        [HttpGet]
        public ActionResult GetShift(long id)
        {
            string start_hh_mm = "";
            string SecHalfTime = "";
            string end_hh_mm = "";
            string halfDayHour = "";
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                ShiftViewModel _ShiftDetail = _WFHService.GetShiftBySite((long)employeeDetails._SiteId, id).FirstOrDefault();
                if (_ShiftDetail != null)
                {
                    start_hh_mm = ((DateTime)_ShiftDetail.START_TIME).ToString("HH:mm");
                    SecHalfTime = ((DateTime)_ShiftDetail.START_TIME).AddHours((double)_ShiftDetail.HALFDAYHOUR).ToString("HH:mm");
                    end_hh_mm = ((DateTime)_ShiftDetail.END_TIME).ToString("HH:mm");
                    halfDayHour = Convert.ToString(_ShiftDetail.HALFDAYHOUR);
                }
            }
            catch
            {
                start_hh_mm = "";
                end_hh_mm = "";
                halfDayHour = "";
            }
            return Json(new
            {
                STARTTIME = start_hh_mm,
                SEC_HALF_TIME = SecHalfTime,
                ENDTIME = end_hh_mm,
                HALFDAYHOUR = halfDayHour
            });
        }

        [HttpGet]
        public ActionResult WFHViewDetail(string id)
        {
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                ASRWFH_APPROVALHIS_ViewModel AAVM = new ASRWFH_APPROVALHIS_ViewModel();
                long _ReqId = Convert.ToInt64(WebUtility.UrlDecode(Encryption.Decrypt(id))); //// Convert.ToInt64(Encryption.Decrypt(Server.UrlDecode(id)));
                AAVM.ASRWFH_HEADER = _WFHService.GetWFHRequestDetail(_ReqId);

                WFHAppAuthViewModel _appAuth = _WFHService.GetViewApprovalAuth(_ReqId); //--WFH issue correction
                if (_appAuth != null)
                {
                    TempData["APPROVAL_AUTHORITY"] = (WFHAppAuthViewModel)_appAuth;
                }
                else
                {
                    TempData["APPROVAL_AUTHORITY"] = new WFHAppAuthViewModel();
                }
                return View(AAVM);
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult WFHReport()
        {
            WFHReportViewModel WRVM = new WFHReportViewModel();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                List<ADORGLEVEL> _opList = _WFHService.GetOrgLevelList((long)1);
                ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP");
                List<ADORGLEVEL> _divList = _WFHService.GetOrgLevelList((long)2);
                ViewBag.DivList = new SelectList(_divList, "ADORGLEVELID", "LEVELDESCRIP");
                List<ADORGLEVEL> _depList = _WFHService.GetOrgLevelList((long)3);
                ViewBag.DepList = new SelectList(_depList, "ADORGLEVELID", "LEVELDESCRIP");
                List<ADORGLEVEL> _secList = _WFHService.GetOrgLevelList((long)4);
                ViewBag.SecList = new SelectList(_secList, "ADORGLEVELID", "LEVELDESCRIP");
                WRVM.OpId = employeeDetails._OpId;
                WRVM.DivId = employeeDetails._DivId;
                WRVM.DepId = employeeDetails._DepId;
                WRVM.SecId = employeeDetails._SecId;
                return View(WRVM);
            }
            catch (Exception ex)
            {
                return View(WRVM);
            }
        }

        [HttpPost]
        public ActionResult WFHReport([FromBody] WFHReportViewModel WRVM)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            List<ASRWFH_HEADER_ViewModel> _headerList = _WFHService.GetWFHReport(WRVM);
            return PartialView("_GetWFHReport", (employeeDetails._FnDesigId == null ? _headerList.Where(h => h.ADEMPCODE == employeeDetails._ECode).ToList() : _headerList));
        }

        [HttpGet]
        public ActionResult WFHIRReport()
        {
            WFHReportViewModel WRVM = new WFHReportViewModel();
            try
            {
                if (_sessionService.Get<string>("userID") == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                List<ADORGLEVEL> _opList = _WFHService.GetOrgLevelList((long)1);
                ViewBag.OpList = new SelectList(_opList, "ADORGLEVELID", "LEVELDESCRIP");
                List<ADORGLEVEL> _divList = _WFHService.GetOrgLevelList((long)2);
                ViewBag.DivList = new SelectList(_divList, "ADORGLEVELID", "LEVELDESCRIP");
                List<ADORGLEVEL> _depList = _WFHService.GetOrgLevelList((long)3);
                ViewBag.DepList = new SelectList(_depList, "ADORGLEVELID", "LEVELDESCRIP");
                List<ADORGLEVEL> _secList = _WFHService.GetOrgLevelList((long)4);
                ViewBag.SecList = new SelectList(_secList, "ADORGLEVELID", "LEVELDESCRIP");

                WRVM.OpId = employeeDetails._OpId;
                WRVM.DivId = employeeDetails._DivId;
                WRVM.DepId = employeeDetails._DepId;
                WRVM.SecId = employeeDetails._SecId;
                return View(WRVM);
            }
            catch
            {
                return View(WRVM);
            }
        }

        [HttpPost]
        public ActionResult WFHIRReport([FromBody]WFHReportViewModel WRVM)
        {
            if (_sessionService.Get<string>("userID") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            //List<ASRWFH_HEADER_ViewModel> _headerList = _WFHService.GetWFHReport(WRVM);
            //return PartialView("_GetWFHReport", _headerList);
            //SR86752 Start
            List<ASRWFH_HEADER_ViewModel> _headerList = _WFHService.GetWFHAdminReport(WRVM);            
            return PartialView("_GetWFHAdminReport", _headerList);
            //SR86752 End
        }

        [HttpPost]
        public ActionResult ExportToExcel([FromBody]WFHReportViewModel WRVM)
        {
            short retVal = 0;
            try
            {
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                List<ASRWFH_HEADER_ViewModel> _headerList = _WFHService.GetWFHReport(WRVM);
                
                    _headerList = (employeeDetails._FnDesigId == null ? _headerList.Where(h => h.ADEMPCODE == employeeDetails._ECode).ToList() : _headerList);
                
                string str = this.excelHtml(_headerList);
                TempData.Remove("EXCELFILE");
                TempData["EXCELFILE"] = JsonSerializer.Serialize<string>(str);
                retVal = 1;
            }
            catch (Exception ex)
            {
                retVal = (short)-1;
            }
            return Json(retVal);
        }

        public ActionResult DownloadExcel()
        {
            try
            {
                if (TempData["EXCELFILE"] == null)
                {
                    return View();
                }
                
                string str = JsonSerializer.Deserialize<string>(TempData["EXCELFILE"].ToString());
                //HttpContext.Response.AddHeader("content-disposition", "attachment; filename=ACRReport.xls");
                Response.ContentType = "application/vnd.ms-excel";
                return File(Encoding.UTF8.GetBytes(str.ToString()), "application/vnd.ms-excel", "WFHReport.xls");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage");
            }
        }

        public string excelHtml(List<ASRWFH_HEADER_ViewModel> _headerList)
        {
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            string str = "";
            if (_headerList.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 11pt;font-family:Arial'>");
                stringBuilder.Append("<tr style='background-color: lightgray;'>");
                stringBuilder.Append("<th style='width:4%;text-align:center;border: 1px solid;'>S.No.</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Ecode</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Employee name</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Request Type</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Applied For</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Start Date</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>End Date</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Start Time</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>End Time</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>No. of days</th>");
                //SR86752 Start
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Request Type</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Specify remarks</th>");
                //SR86752 End
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Applied Date</th>");
                stringBuilder.Append("</tr>");
                int srNo = 1;
                foreach (ASRWFH_HEADER_ViewModel AHVM in _headerList)
                {
                    var _TotalDay = ((Convert.ToDateTime(AHVM.ENDDATE).AddDays(1)) - Convert.ToDateTime(AHVM.STARTDATE)).TotalDays;
                    var _WorkingDays = (_TotalDay - AHVM.OffDays);
                    string reqtype = AHVM.REQUESTTYPE == 1 ? "Work From Home" : "Remote Support";
                    string applyFor = AHVM.APPLYFOR == null ? "" : AHVM.APPLYFOR == 1 ? "Business Working Days" : "Off Days";
                    string RequestType = AHVM.WFH_REQUESTTYPE == 1 ? "As per roster" : AHVM.WFH_REQUESTTYPE == 2 ? "Apart from roster" : "";  //SR86752
                    stringBuilder.Append("<tr>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + srNo++ + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + AHVM.ADEMPCODE + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.EmpName + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + reqtype + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + applyFor + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + Convert.ToDateTime(AHVM.STARTDATE).ToString("dd-MMM-yyyy") + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + Convert.ToDateTime(AHVM.ENDDATE).ToString("dd-MMM-yyyy") + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.STARTTIME + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.ENDTIME + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + _WorkingDays + "</td>");
                    //SR86752 Start
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + RequestType + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.WFH_REQUESTTYPEREMARKS + "</td>");
                    //SR86752 End
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + Convert.ToDateTime(AHVM.DATEADDED).ToString("dd-MMM-yyyy") + "</td>");
                    stringBuilder.Append("</tr>");
                }
                stringBuilder.Append("</table>");
                str = stringBuilder.ToString();
            }
            return str;
        }

        public ActionResult BindDivisionByOperationId(long id)
        {
            List<WFHDivViewModel> divList = _WFHService.BindDivision(id);
            return Json(divList);
        }

        public ActionResult BindDeptByDivisionId(long id)
        {
            List<WFHDepViewModel> depList = _WFHService.BindDepartment(id, 0);
            return Json(depList);
        }

        public ActionResult BindSecByDepartmentId(long id)
        {
            List<WFHSecViewModel> SecList = _WFHService.BindSection(id, 0, 0);
            return Json(SecList);
        }
        //SR86752 Start      
        [HttpPost]
        public ActionResult AdminExportToExcel([FromBody] WFHReportViewModel WRVM)
        {
            short retVal = 0;
            try
            {
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                List<ASRWFH_HEADER_ViewModel> _headerList = _WFHService.GetWFHAdminReport(WRVM);               
                string str = this.AdminexcelHtml(_headerList);
                TempData.Remove("EXCELFILE");
                TempData["EXCELFILE"] = JsonSerializer.Serialize(str); ;
                retVal = 1;
            }
            catch (Exception ex)
            {
                retVal = (short)-1;
            }
            return Json(retVal);
        }
        public string AdminexcelHtml(List<ASRWFH_HEADER_ViewModel> _headerList)
        {
            Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
            string str = "";
            if (_headerList.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<table cellpadding='3' cellspacing='0' style='width:100%;margin-top:8px;border: 1px solid;border-collapse: collapse;font-size: 11pt;font-family:Arial'>");
                stringBuilder.Append("<tr style='background-color: lightgray;'>");
                stringBuilder.Append("<th style='width:4%;text-align:center;border: 1px solid;'>S.No.</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Ecode</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Employee name</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Request Type</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Applied For</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Start Date</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>End Date</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Start Time</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>End Time</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>No. of days</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Request Type</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Specify remarks</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Status</th>");
                stringBuilder.Append("<th style='text-align:center;border: 1px solid;'>Applied Date</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Recommended By</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Recommended Date</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Approval By</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>Approval Date</th>");
                stringBuilder.Append("<th style='width:6%;text-align:center;border: 1px solid;'>WFH Source</th>");
                stringBuilder.Append("</tr>");
                int srNo = 1;
                foreach (ASRWFH_HEADER_ViewModel AHVM in _headerList)
                {
                    var _TotalDay = ((Convert.ToDateTime(AHVM.ENDDATE).AddDays(1)) - Convert.ToDateTime(AHVM.STARTDATE)).TotalDays;
                    var _WorkingDays = (_TotalDay - AHVM.OffDays);
                    string reqtype = AHVM.REQUESTTYPE == 1 ? "Work From Home" : "Remote Support";
                    string applyFor = AHVM.APPLYFOR == null ? "" : AHVM.APPLYFOR == 1 ? "Business Working Days" : "Off Days";
                    string RequestType = AHVM.WFH_REQUESTTYPE == 1 ? "As per roster" : AHVM.WFH_REQUESTTYPE == 2 ? "Apart from roster" : "";
                    string Status = AHVM.STATUS == 0 ? "In Progress" : AHVM.STATUS == 1 ? "Approved" : "";
                    string WFH_SOURCE = AHVM.AUTO_WFH_STATUS == 1 ? "Auto" : AHVM.AUTO_WFH_STATUS == 2 ? "Cancelled by User" : "Self";
                    string RecDate =  AHVM.RECAPPROVEDDATE != null ? Convert.ToDateTime(AHVM.RECAPPROVEDDATE).ToString("dd-MMM-yyyy") : "";
                    string AppDate = AHVM.APPAPPROVEDDATE != null ? Convert.ToDateTime(AHVM.APPAPPROVEDDATE).ToString("dd-MMM-yyyy") : "";
                    stringBuilder.Append("<tr>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + srNo++ + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border: 1px solid;'>" + AHVM.ADEMPCODE + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + AHVM.EmpName + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + reqtype + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + applyFor + "</td>");
                    stringBuilder.Append("<td style='border:1px solid;'>" + Convert.ToDateTime(AHVM.STARTDATE).ToString("dd-MMM-yyyy") + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + Convert.ToDateTime(AHVM.ENDDATE).ToString("dd-MMM-yyyy") + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.STARTTIME + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.ENDTIME + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + _WorkingDays + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + RequestType + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.WFH_REQUESTTYPEREMARKS + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + Status + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + Convert.ToDateTime(AHVM.DATEADDED).ToString("dd-MMM-yyyy") + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.RECADEMPCODE + "-" + AHVM.RECOMMENDEDBY + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + RecDate + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AHVM.APPADEMPCODE + "-" + AHVM.APPROVALBY + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + AppDate + "</td>");
                    stringBuilder.Append("<td style='text-align:center;border:1px solid;'>" + WFH_SOURCE + "</td>");
                    stringBuilder.Append("</tr>");
                }
                stringBuilder.Append("</table>");
                str = stringBuilder.ToString();
            }
            return str;
        }
        //SR86752 End

        /*Added by TTL on 31-July-2025 - Start*/
        private WFHOffDays OffDaysValidation(string startDate, string endDate, long? reqId)
        {
            short offDays = 0; short offDaysByCurrentDate = 0; short totalDays = 0; short workingDays = 0; bool isValid = false; bool isRejectValid = false, isActiveDate = false;
            try
            {
                Employee_Details employeeDetails = _sessionService.Get<Employee_Details>("Employee");
                DateTime _currentDate = DateTime.ParseExact(DateTime.Now.ToString("dd-MMM-yyyy"), "dd-MMM-yyyy", null);
                DateTime _sdate;
                DateTime _edate;

                //// ---- Get Active Date Parameter Value from SYPARAMETER Table ----////
                bool checkNewValidation;
                string date_parm = _loginService.GetParameterValue("WFH_ACTIVEDATE_VALIDATION");
                if (string.IsNullOrEmpty(date_parm))
                {
                    checkNewValidation = true;
                }
                else
                {
                    DateTime _activeDate = DateTime.ParseExact(date_parm, "dd-MMM-yyyy", null);
                    checkNewValidation = _currentDate >= _activeDate ? true : false;
                }

                //// ---- Check Active Date validation ---- ////
                if (checkNewValidation)
                {
                    isActiveDate = true;

                    //// ---- Get Parameter Value from SYPARAMETER Table ----////
                    string parm_value = _loginService.GetParameterValue("WFH_REJECTED_VALIDATION");
                    parm_value = string.IsNullOrEmpty(parm_value) ? "0" : parm_value;
                    int previous_value = Convert.ToInt32(parm_value);

                    //// ---- Get Rejected Request ---- ////
                    ASRWFH_HEADER_ViewModel _Obj = _WFHService.GetRejectedRequest(startDate, endDate, Convert.ToInt64(_sessionService.Get<string>("userID").ToString()));

                    //// ---- This code use for edit request ---- ////
                    ASRWFH_HEADER_ViewModel _headerDtl = new ASRWFH_HEADER_ViewModel();
                    if (reqId != null)
                    {
                        _headerDtl = _WFHService.GetWFHRequestDetail(Convert.ToInt64(reqId));
                    }
                    string editReqStartDate = _headerDtl == null ? "" : Convert.ToDateTime(_headerDtl.STARTDATE).ToString("dd-MMM-yyyy");
                    DateTime _editReqStartDate = DateTime.ParseExact(editReqStartDate, "dd-MMM-yyyy", null);
                    //// ---- End ---- //// 

                    //// ---- Check Current Date and Previous date Validation ----////
                    if (!string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))
                    {
                        _sdate = DateTime.ParseExact(startDate, "dd-MMM-yyyy", null);
                        if (_Obj == null)
                        {
                            //// ---- Get Off Days By StartDate & Current Date ----////
                            offDaysByCurrentDate = _WFHService.GetOffDays(startDate, DateTime.Now.ToString("dd-MMM-yyyy"), (long)employeeDetails._SiteId, (long)employeeDetails._OpId);
                            previous_value = offDaysByCurrentDate == 0 ? previous_value : previous_value + offDaysByCurrentDate;

                            if (_currentDate == _sdate || _sdate >= _currentDate.AddDays(-previous_value))
                            {
                                isValid = true;
                                isRejectValid = true;
                            }
                            else
                            {
                                isValid = false;
                                isRejectValid = true;
                            }
                        }
                        else
                        {
                            string _rejectDM = _Obj.ASRWFH_APPROVALHIS.DATEISTMOD == null ? "" : Convert.ToDateTime(_Obj.ASRWFH_APPROVALHIS.DATEISTMOD).ToString("dd-MMM-yyyy");
                            if (!string.IsNullOrEmpty(_rejectDM))
                            {
                                //// ---- Get Off Days By RejectedDate & Current Date ----////
                                offDaysByCurrentDate = _WFHService.GetOffDays(_rejectDM, DateTime.Now.ToString("dd-MMM-yyyy"), (long)employeeDetails._SiteId, (long)employeeDetails._OpId);
                                parm_value = offDaysByCurrentDate == 0 ? parm_value : Convert.ToString(Convert.ToInt32(parm_value) + offDaysByCurrentDate);

                                DateTime _rDate_ = DateTime.ParseExact(_rejectDM, "dd-MMM-yyyy", null);
                                isValid = _currentDate == _rDate_ || (_currentDate >= _rDate_ && _currentDate <= _rDate_.AddDays(Convert.ToInt32(parm_value))) ? true : false;
                                isRejectValid = _currentDate == _rDate_ || (_currentDate >= _rDate_ && _currentDate <= _rDate_.AddDays(Convert.ToInt32(parm_value))) ? true : false;
                            }
                        }
                        return new WFHOffDays
                        {
                            OFFDAYS = offDays,
                            TOTALDAYS = totalDays,
                            WORKINGDAYS = workingDays,
                            ISVALID = isValid,
                            ISREJECTVALID = isRejectValid,
                            ISACTIVEDATE = isActiveDate,
                        };
                    }
                    //// ---- End ----////

                    _sdate = DateTime.ParseExact(startDate, "dd-MMM-yyyy", null);
                    _edate = DateTime.ParseExact(endDate, "dd-MMM-yyyy", null);

                    //// ---- Get Off Days StartDate & EndDate ---- ////
                    offDays = _WFHService.GetOffDays(startDate, endDate, (long)employeeDetails._SiteId, (long)employeeDetails._OpId);

                    totalDays = (short)((_edate.AddDays(1)) - _sdate).TotalDays;
                    workingDays = (short)(totalDays - offDays);
                    if (_Obj != null)
                    {
                        string rejectDM = _Obj.ASRWFH_APPROVALHIS.DATEISTMOD == null ? "" : Convert.ToDateTime(_Obj.ASRWFH_APPROVALHIS.DATEISTMOD).ToString("dd-MMM-yyyy");
                        if (!string.IsNullOrEmpty(rejectDM))
                        {
                            //// ---- Get Off Days By RejectedDate & Current Date ----////
                            offDaysByCurrentDate = _WFHService.GetOffDays(rejectDM, DateTime.Now.ToString("dd-MMM-yyyy"), (long)employeeDetails._SiteId, (long)employeeDetails._OpId);
                            parm_value = offDaysByCurrentDate == 0 ? parm_value : Convert.ToString(Convert.ToInt32(parm_value) + offDaysByCurrentDate);

                            DateTime _rDate = DateTime.ParseExact(rejectDM, "dd-MMM-yyyy", null);
                            isValid = (_sdate == _edate && (_currentDate == _rDate || (_currentDate >= _rDate && _currentDate <= _rDate.AddDays(Convert.ToInt32(parm_value))) || _editReqStartDate == _sdate)) ? true : false;
                            isRejectValid = (_currentDate == _rDate || (_currentDate >= _rDate && _currentDate <= _rDate.AddDays(Convert.ToInt32(parm_value))) || _editReqStartDate == _sdate) ? true : false;
                        }
                    }
                    else
                    {
                        //// ---- Get Off Days By StartDate & Current Date ----////
                        offDaysByCurrentDate = _WFHService.GetOffDays(startDate, DateTime.Now.ToString("dd-MMM-yyyy"), (long)employeeDetails._SiteId, (long)employeeDetails._OpId);
                        previous_value = offDaysByCurrentDate == 0 ? previous_value : previous_value + offDaysByCurrentDate;

                        isValid = (_sdate == _edate && (_currentDate == _sdate || _sdate >= _currentDate.AddDays(-previous_value)) || _editReqStartDate == _sdate) ? true : false;
                        isRejectValid = true;
                    }
                }
                else
                {
                    _sdate = DateTime.ParseExact(startDate, "dd-MMM-yyyy", null);
                    _edate = DateTime.ParseExact(endDate, "dd-MMM-yyyy", null);

                    //// ---- Get Off Days StartDate & EndDate ---- ////
                    offDays = _WFHService.GetOffDays(startDate, endDate, (long)employeeDetails._SiteId, (long)employeeDetails._OpId);

                    totalDays = (short)((_edate.AddDays(1)) - _sdate).TotalDays;
                    workingDays = (short)(totalDays - offDays);
                    isValid = _edate >= _sdate ? true : false;
                    isRejectValid = true;
                    isActiveDate = false;
                }
            }
            catch (Exception ex)
            {
                offDays = -1;
                totalDays = -1;
                workingDays = -1;
            }
            return new WFHOffDays
            {
                OFFDAYS = offDays,
                TOTALDAYS = totalDays,
                WORKINGDAYS = workingDays,
                ISVALID = isValid,
                ISREJECTVALID = isRejectValid,
                ISACTIVEDATE = isActiveDate,
            };
        }
        /*Added by TTL on 31-July-2025 against SR104870 > CR6961 - End*/
    }
}
